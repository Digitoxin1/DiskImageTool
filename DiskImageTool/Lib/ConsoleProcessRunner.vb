Imports System.Text
Imports System.Threading

Public Class ConsoleProcessRunner
    Implements IDisposable

    Private _ctr As CancellationTokenRegistration
    Private _ctsExternal As CancellationToken

    Private _disposed As Boolean
    Private _exitCode As Integer?

    Private _proc As Process
    Private _tcs As TaskCompletionSource(Of Integer)

    Public Event ErrorLineReceived(line As String)
    Public Event OutputLineReceived(line As String)
    Public Event ProcessExited(exitCode As Integer)
    Public Event ProcessFailed(message As String, ex As Exception)
    Public Event ProcessStarted(exePath As String, arguments As String)

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

    Public Property StandardErrorEncoding As Encoding
    Public Property StandardOutputEncoding As Encoding
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
                AddHandler proc.OutputDataReceived, Sub(sender, e)
                                                        If e.Data IsNot Nothing Then
                                                            SyncLock sbOut
                                                                sbOut.AppendLine(e.Data)
                                                                sbAll.AppendLine(e.Data)
                                                            End SyncLock
                                                        End If
                                                    End Sub
            End If

            If captureError Then
                AddHandler proc.ErrorDataReceived, Sub(sender, e)
                                                       If e.Data IsNot Nothing Then
                                                           SyncLock sbErr
                                                               sbErr.AppendLine(e.Data)
                                                               sbAll.AppendLine(e.Data)
                                                           End SyncLock
                                                       End If
                                                   End Sub
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
                RaiseOnContext(Sub() RaiseEvent ProcessFailed("Failed to kill process.", ex))
            End Try
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Public Function StartAsync(exePath As String, arguments As String, Optional ct As CancellationToken = Nothing) As Task(Of Integer)
        If _proc IsNot Nothing Then
            Throw New InvalidOperationException("Process is already started on this instance.")
        End If

        _tcs = New TaskCompletionSource(Of Integer)(TaskCreationOptions.RunContinuationsAsynchronously)
        _ctsExternal = ct
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

        If StandardOutputEncoding IsNot Nothing Then
            psi.StandardOutputEncoding = StandardOutputEncoding
        End If

        If StandardErrorEncoding IsNot Nothing Then
            psi.StandardErrorEncoding = StandardErrorEncoding
        End If

        _proc = New Process() With {
            .StartInfo = psi,
            .EnableRaisingEvents = True
        }

        ' Wire cancellation: kill process & children if requested
        If ct.CanBeCanceled Then
            _ctr = ct.Register(Sub()
                                   Try
                                       If _proc IsNot Nothing AndAlso Not _proc.HasExited Then
                                           _proc.Kill()
                                       End If
                                   Catch
                                   End Try
                               End Sub)
        End If

        ' Subscribe to events BEFORE Start()
        AddHandler _proc.OutputDataReceived, AddressOf OnOutputData
        AddHandler _proc.ErrorDataReceived, AddressOf OnErrorData
        AddHandler _proc.Exited, AddressOf OnExited

        Try
            If Not _proc.Start() Then
                Throw New InvalidOperationException("Failed to start process.")
            End If
        Catch ex As Exception
            RaiseOnContext(Sub() RaiseEvent ProcessFailed("Failed to start process.", ex))
            Cleanup()
            Throw
        End Try

        RaiseOnContext(Sub() RaiseEvent ProcessStarted(exePath, arguments))

        ' Begin async reads (line-buffered)
        _proc.BeginOutputReadLine()
        _proc.BeginErrorReadLine()

        Return _tcs.Task
    End Function

    Public Sub WriteInput(text As String)
        If _proc Is Nothing OrElse _proc.HasExited Then Return
        If Not _proc.StartInfo.RedirectStandardInput Then
            Throw New InvalidOperationException("StandardInput is not redirected. Enable it in the StartInfo setup if needed.")
        End If
        Try
            _proc.StandardInput.WriteLine(text)
        Catch ex As Exception
            RaiseOnContext(Sub() RaiseEvent ProcessFailed("Failed to write to StandardInput.", ex))
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

    Private Sub Cleanup()
        Try
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
        If e.Data Is Nothing Then Return
        RaiseOnContext(Sub() RaiseEvent ErrorLineReceived(e.Data))
    End Sub

    Private Sub OnExited(sender As Object, e As EventArgs)
        Dim code As Integer = 0
        Try
            code = _proc.ExitCode
        Catch
        End Try
        _exitCode = code

        RaiseOnContext(Sub() RaiseEvent ProcessExited(code))
        _tcs.TrySetResult(code)

        Cleanup()
    End Sub

    Private Sub OnOutputData(sender As Object, e As DataReceivedEventArgs)
        If e.Data Is Nothing Then Return
        RaiseOnContext(Sub() RaiseEvent OutputLineReceived(e.Data))
    End Sub

    Private Sub RaiseOnContext(action As Action)
        Dim ctx = EventContext
        If ctx IsNot Nothing Then
            ctx.Post(Sub(state) action(), Nothing)
        Else
            ' No context captured: invoke inline on current thread
            action()
        End If
    End Sub

    Public Structure ProcessResult
        Public ReadOnly ExitCode As Integer
        Public ReadOnly CombinedOutput As String
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
