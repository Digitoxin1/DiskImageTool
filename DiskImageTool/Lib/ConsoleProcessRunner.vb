Imports System.IO
Imports System.Text
Imports System.Threading

Public Class ConsoleProcessRunner
    Implements IDisposable

    Private _ctr As CancellationTokenRegistration

    Private _disposed As Boolean
    Private _exitCode As Integer?
    Private _proc As Process
    Private _rawErrTask As Task
    Private _rawOutTask As Task
    Private _state As ProcessStateEnum = ProcessStateEnum.Idle
    Private _tcs As TaskCompletionSource(Of Integer)
    Private _logFilePath As String
    Private _logWriter As StreamWriter
    Private ReadOnly _logLock As New Object()

    Private _useRawCapture As Boolean
    Public Enum ProcessStateEnum As Byte
        Idle = 0
        Running = 1
        Completed = 2
        [Error] = 3
        Aborted = 4
    End Enum

    Public Event ErrorDataReceived(data As String)
    Public Event OutputDataReceived(data As String)
    Public Event ProcessExited(exitCode As Integer)
    Public Event ProcessFailed(message As String, ex As Exception)
    Public Event ProcessStarted(exePath As String, arguments As String)
    Public Event ProcessStateChanged(state As ProcessStateEnum)

    Public Property EventContext As SynchronizationContext
    Public ReadOnly Property ExitCode As Integer?
        Get
            Return _exitCode
        End Get
    End Property

    Public ReadOnly Property IsRunning As Boolean
        Get
            Return _proc IsNot Nothing AndAlso Not _proc.HasExited
        End Get
    End Property

    Private Sub LogLine(line As String)
        If _logWriter Is Nothing Then Exit Sub
        Try
            SyncLock _logLock
                _logWriter.WriteLine(line)
                _logWriter.Flush()
            End SyncLock
        Catch
            SyncLock _logLock
                Try : _logWriter.Dispose() : Catch : End Try
                _logWriter = Nothing
            End SyncLock
        End Try
    End Sub

    Private Sub LogChunk(chunk As String)
        If _logWriter Is Nothing Then Exit Sub
        Try
            SyncLock _logLock
                _logWriter.Write(chunk)
                _logWriter.Flush()
            End SyncLock
        Catch
            SyncLock _logLock
                Try : _logWriter.Dispose() : Catch : End Try
                _logWriter = Nothing
            End SyncLock
        End Try
    End Sub

    Public Property StandardErrorEncoding As Encoding = Encoding.UTF8

    Public Property StandardOutputEncoding As Encoding = Encoding.UTF8

    Public ReadOnly Property State As ProcessStateEnum
        Get
            Return _state
        End Get
    End Property
    Public Property WorkingDirectory As String

    Public Shared Function RunProcess(fileName As String,
                                        arguments As String,
                                        Optional workingDirectory As String = Nothing,
                                        Optional captureOutput As Boolean = True,
                                        Optional captureError As Boolean = True,
                                        Optional timeoutMilliseconds As Integer = -1
                                    ) As ProcessResult

        Dim psi As New ProcessStartInfo(fileName, arguments) With {
            .UseShellExecute = False,
            .CreateNoWindow = True,
            .RedirectStandardOutput = captureOutput,
            .RedirectStandardError = captureError
        }

        If Not String.IsNullOrWhiteSpace(workingDirectory) Then
            psi.WorkingDirectory = workingDirectory
        End If

        If captureOutput Then
            psi.StandardOutputEncoding = Encoding.UTF8
        End If

        If captureError Then
            psi.StandardErrorEncoding = Encoding.UTF8
        End If

        Dim sbOut As New StringBuilder()
        Dim sbErr As New StringBuilder()
        Dim sbAll As New StringBuilder()

        Using proc As New Process()
            proc.StartInfo = psi

            If captureOutput Then
                AddHandler proc.OutputDataReceived, Sub(sender, e) CollectOutputLine(sbOut, sbAll, e)
            End If

            If captureError Then
                AddHandler proc.ErrorDataReceived, Sub(sender, e) CollectErrorLine(sbErr, sbAll, e)
            End If

            proc.Start()

            If captureOutput Then
                proc.BeginOutputReadLine()
            End If

            If captureError Then
                proc.BeginErrorReadLine()
            End If

            Dim timedOut As Boolean = False

            If timeoutMilliseconds >= 0 Then
                ' Timed wait
                If Not proc.WaitForExit(timeoutMilliseconds) Then
                    timedOut = True
                    Try
                        proc.Kill()
                    Catch
                        ' ignore kill failures
                    End Try
                Else
                    ' Ensure async readers flush remaining lines
                    proc.WaitForExit()
                End If
            Else
                ' No timeout: wait indefinitely
                proc.WaitForExit()
                proc.WaitForExit() ' once more to ensure async handlers finish
            End If

            Return New ProcessResult(
                exitCode:=If(timedOut, -1, proc.ExitCode),
                combined:=If(captureOutput Or captureError, sbAll.ToString(), String.Empty),
                stdOut:=If(captureOutput, sbOut.ToString(), String.Empty),
                stdErr:=If(captureError, sbErr.ToString(), String.Empty),
                timedOut:=timedOut
            )
        End Using

    End Function

    Public Sub Cancel()
        If _proc IsNot Nothing AndAlso Not _proc.HasExited Then
            Try
                _proc.Kill()
            Catch ex As Exception
                ' If kill fails, surface as a failure event but keep going
                SetProcessStateFailed("Failed to kill process.", ex)
            End Try
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Public Function StartAsync(exePath As String, arguments As String, Optional ct As CancellationToken = Nothing, Optional useRawCapture As Boolean = False, Optional logFile As String = Nothing) As Task(Of Integer)
        If _proc IsNot Nothing Then
            Throw New InvalidOperationException("Process is already started on this instance.")
        End If

        _logFilePath = logFile

        If _logWriter IsNot Nothing Then
            _logWriter.Dispose()
            _logWriter = Nothing
        End If

        If Not String.IsNullOrWhiteSpace(_logFilePath) Then
            Try
                _logWriter = New StreamWriter(_logFilePath, append:=True, encoding:=Encoding.UTF8)
            Catch
                _logWriter = Nothing
            End Try
        End If

        _useRawCapture = useRawCapture

        _tcs = New TaskCompletionSource(Of Integer)(TaskCreationOptions.RunContinuationsAsynchronously)
        _exitCode = Nothing

        Dim psi As New ProcessStartInfo(exePath, arguments) With {
            .UseShellExecute = False,
            .CreateNoWindow = True,
            .RedirectStandardOutput = True,
            .RedirectStandardError = True
        }

        If Not String.IsNullOrWhiteSpace(WorkingDirectory) Then
            psi.WorkingDirectory = WorkingDirectory
        End If

        psi.StandardOutputEncoding = StandardOutputEncoding
        psi.StandardErrorEncoding = StandardErrorEncoding

        _proc = New Process() With {
            .StartInfo = psi,
            .EnableRaisingEvents = True
        }

        ' Wire cancellation: kill process & children if requested
        If ct.CanBeCanceled Then
            _ctr = ct.Register(AddressOf CancelProcessCallback)
        End If

        ' Subscribe to events BEFORE Start()
        AddHandler _proc.Exited, AddressOf OnExited

        If Not _useRawCapture Then
            AddHandler _proc.OutputDataReceived, AddressOf OnOutputData
            AddHandler _proc.ErrorDataReceived, AddressOf OnErrorData
        End If

        Try
            If Not _proc.Start() Then
                Throw New InvalidOperationException("Failed to start process.")
            End If
        Catch ex As Exception
            SetProcessStateFailed("Failed to start process.", ex)
            Cleanup()
            Throw
        End Try

        SetProcessState(ProcessStateEnum.Running)
        RaiseOnContext(Sub() RaiseEvent ProcessStarted(exePath, arguments))

        ' Begin async reads, depending on mode
        If _useRawCapture Then
            _rawOutTask = Task.Run(AddressOf ReadRawOutput)
            _rawErrTask = Task.Run(AddressOf ReadRawError)
        Else
            ' line-buffered behavior
            _proc.BeginOutputReadLine()
            _proc.BeginErrorReadLine()
        End If


        Return _tcs.Task
    End Function

    Public Function StartAsyncRaw(exePath As String, arguments As String, Optional ct As CancellationToken = Nothing) As Task(Of Integer)
        Return StartAsync(exePath, arguments, ct, True)
    End Function

    Public Sub WriteInput(text As String)
        If _proc Is Nothing OrElse _proc.HasExited Then
            Return
        End If

        If Not _proc.StartInfo.RedirectStandardInput Then
            Throw New InvalidOperationException("StandardInput is not redirected. Enable it in the StartInfo setup if needed.")
        End If

        Try
            _proc.StandardInput.WriteLine(text)
        Catch ex As Exception
            SetProcessStateFailed("Failed to write to StandardInput.", ex)
        End Try
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not _disposed Then
            If disposing Then
                Try
                    Cancel()
                Catch
                End Try
                Cleanup()
            End If
            _disposed = True
        End If
    End Sub

    Private Shared Sub CollectErrorLine(sbErr As StringBuilder, sbAll As StringBuilder, e As DataReceivedEventArgs)
        If e.Data Is Nothing Then Return
        SyncLock sbErr
            sbErr.AppendLine(e.Data)
            sbAll.AppendLine(e.Data)
        End SyncLock
    End Sub

    Private Shared Sub CollectOutputLine(sbOut As StringBuilder, sbAll As StringBuilder, e As DataReceivedEventArgs)
        If e.Data Is Nothing Then Return
        SyncLock sbOut
            sbOut.AppendLine(e.Data)
            sbAll.AppendLine(e.Data)
        End SyncLock
    End Sub

    Private Sub CancelProcessCallback()
        Try
            If _proc IsNot Nothing AndAlso Not _proc.HasExited Then
                _proc.Kill()
            End If
        Catch
        End Try
    End Sub

    Private Sub Cleanup()
        Try
            ' Wait for raw readers to finish if in raw mode
            If _useRawCapture Then
                Try
                    _rawOutTask?.Wait()
                Catch
                End Try

                Try
                    _rawErrTask?.Wait()
                Catch
                End Try
            End If

            If _proc IsNot Nothing Then
                Try : _proc.CancelOutputRead() : Catch : End Try
                Try : _proc.CancelErrorRead() : Catch : End Try

                RemoveHandler _proc.OutputDataReceived, AddressOf OnOutputData
                RemoveHandler _proc.ErrorDataReceived, AddressOf OnErrorData
                RemoveHandler _proc.Exited, AddressOf OnExited

                _proc.Dispose()
            End If
        Finally
            _proc = Nothing
            _ctr.Dispose()
        End Try
    End Sub

    Private Sub OnErrorData(sender As Object, e As DataReceivedEventArgs)
        If e.Data Is Nothing Then
            Return
        End If
        LogLine(e.Data)
        RaiseOnContext(Sub() RaiseEvent ErrorDataReceived(e.Data))
    End Sub

    Private Sub OnExited(sender As Object, e As EventArgs)
        Dim code As Integer = 0
        Try
            code = _proc.ExitCode
        Catch
        End Try
        _exitCode = code

        If _logWriter IsNot Nothing Then
            _logWriter.Flush()
            _logWriter.Dispose()
            _logWriter = Nothing
        End If

        RaiseOnContext(Sub() RaiseEvent ProcessExited(code))
        If code = -1 Then
            SetProcessState(ProcessStateEnum.Aborted)
        Else
            SetProcessState(ProcessStateEnum.Completed)
        End If
        _tcs.TrySetResult(code)

        Cleanup()
    End Sub

    Private Sub OnOutputData(sender As Object, e As DataReceivedEventArgs)
        If e.Data Is Nothing Then Return
        LogLine(e.Data)
        RaiseOnContext(Sub() RaiseEvent OutputDataReceived(e.Data))
    End Sub

    Private Sub RaiseOnContext(action As Action)
        Dim ctx = EventContext

        If ctx Is Nothing Then
            action()
            Return
        End If

        If ctx Is SynchronizationContext.Current Then
            ' Already on UI thread — run inline
            action()
        Else
            ' Background thread — marshal to UI thread
            ctx.Post(Sub(state) action(), Nothing)
        End If
    End Sub

    Private Sub ReadRawError()
        Try
            Dim buffer(4095) As Byte
            Dim stream = _proc.StandardError.BaseStream

            Do
                Dim read = stream.Read(buffer, 0, buffer.Length)
                If read <= 0 Then
                    Exit Do
                End If

                Dim chunk = StandardErrorEncoding.GetString(buffer, 0, read)
                LogChunk(chunk)

                RaiseOnContext(Sub()
                                   RaiseEvent ErrorDataReceived(chunk)
                               End Sub)
            Loop
        Catch ex As Exception
            SetProcessStateFailed("Error reading standard error.", ex)
        End Try
    End Sub

    Private Sub ReadRawOutput()
        Try
            Dim buffer(4095) As Byte
            Dim stream = _proc.StandardOutput.BaseStream

            Do
                Dim read = stream.Read(buffer, 0, buffer.Length)
                If read <= 0 Then
                    Exit Do
                End If

                Dim chunk = StandardOutputEncoding.GetString(buffer, 0, read)
                LogChunk(chunk)

                RaiseOnContext(Sub()
                                   RaiseEvent OutputDataReceived(chunk)
                               End Sub)
            Loop
        Catch ex As Exception
            SetProcessStateFailed("Error reading standard output.", ex)
        End Try
    End Sub

    Private Sub SetProcessState(newState As ProcessStateEnum)
        If _state <> newState Then
            _state = newState
            RaiseOnContext(Sub() RaiseEvent ProcessStateChanged(_state))
        End If
    End Sub

    Private Sub SetProcessStateFailed(message As String, ex As Exception)
        RaiseOnContext(Sub() RaiseEvent ProcessFailed(message, ex))
        SetProcessState(ProcessStateEnum.Error)
    End Sub
    Public Structure ProcessResult
        Public ReadOnly CombinedOutput As String
        Public ReadOnly ExitCode As Integer
        Public ReadOnly StdErr As String
        Public ReadOnly StdOut As String
        Public ReadOnly TimedOut As Boolean

        Public Sub New(exitCode As Integer, stdOut As String, stdErr As String, combined As String, timedOut As Boolean)
            Me.CombinedOutput = combined
            Me.ExitCode = exitCode
            Me.StdOut = stdOut
            Me.StdErr = stdErr
            Me.TimedOut = timedOut
        End Sub
    End Structure
End Class
