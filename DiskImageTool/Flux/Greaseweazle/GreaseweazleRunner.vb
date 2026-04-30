Imports System.Threading
Imports Greaseweazle.Core
Imports Greaseweazle.Infrastructure

Namespace Flux.Greaseweazle

    ' Threading + state-machine adapter for the synchronous Greaseweazle.dll
    ' command Run() calls. Mirrors the public shape of ConsoleProcessRunner so
    ' consuming forms feel familiar, and reuses ConsoleProcessRunner.ProcessStateEnum
    ' so the existing state-handling vocabulary carries over unchanged.
    Public Class GreaseweazleRunner
        Implements IDisposable

        Private _cts As CancellationTokenSource
        Private _disposed As Boolean
        Private _eventContext As SynchronizationContext
        Private _state As ConsoleProcessRunner.ProcessStateEnum = ConsoleProcessRunner.ProcessStateEnum.Idle
        Private _task As Task
        Public Event LibraryDiagnostic As EventHandler(Of LibraryDiagnosticEventArgs)
        Public Event OutputDataReceived(line As String)
        Public Event OutputTextReceived(text As String)
        Public Event ProcessFailed(ex As Exception)
        Public Event ProcessStateChanged(state As ConsoleProcessRunner.ProcessStateEnum)

        Public Sub New()
            _eventContext = SynchronizationContext.Current
        End Sub

        Public Property EventContext As SynchronizationContext
            Get
                Return _eventContext
            End Get
            Set(value As SynchronizationContext)
                _eventContext = value
            End Set
        End Property

        Public ReadOnly Property IsRunning As Boolean
            Get
                Return _state = ConsoleProcessRunner.ProcessStateEnum.Running
            End Get
        End Property

        Public ReadOnly Property State As ConsoleProcessRunner.ProcessStateEnum
            Get
                Return _state
            End Get
        End Property

        Public Sub Cancel()
            Try
                _cts?.Cancel()
            Catch
            End Try
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If _disposed Then Return
            _disposed = True
            Try
                Cancel()
            Catch
            End Try
            DisposeCts()
        End Sub

        ' Forward a synthesized log line to subscribers on the captured UI context.
        ' Safe to call from any thread (typically from a typed *EventArgs handler
        ' that fires on the worker thread inside cmd.Run).
        Public Sub EmitOutputLine(line As String)
            Dim Captured = line
            RaiseOnContext(Sub() RaiseEvent OutputDataReceived(Captured))
        End Sub

        ' Forward a raw text fragment (no line discipline) to subscribers on the
        ' captured UI context. Use this for incremental, character-stream-style
        ' output where the form appends each chunk verbatim and owns any newline
        ' decisions itself. Safe to call from any thread.
        Public Sub EmitOutputText(text As String)
            Dim Captured = text
            RaiseOnContext(Sub() RaiseEvent OutputTextReceived(Captured))
        End Sub

        ' Marshal an arbitrary action to the captured UI context. Inline if already there.
        Public Sub PostToUi(action As Action)
            RaiseOnContext(action)
        End Sub

        ' Run a Sub-shaped command (e.g. Reset, or any command whose result the
        ' caller doesn't need). State transitions: Running -> {Completed, Aborted, Error}.
        Public Function RunAsync(work As Action(Of CancellationToken)) As Task
            Return RunCore(
                Sub(ct) work(ct),
                Nothing,
                Nothing)
        End Function

        ' Run a Function-shaped command. The typed result is delivered to onResult
        ' on the UI thread BEFORE the Completed state transition (both queue through
        ' the captured EventContext, so handler order is deterministic). Pass
        ' Nothing for onResult if you only care about the state transition.
        Public Function RunAsync(Of TResult)(
                work As Func(Of CancellationToken, TResult),
                Optional onResult As Action(Of TResult) = Nothing) As Task

            Return RunCore(
                Nothing,
                Function(ct) DirectCast(work(ct), Object),
                Sub(o)
                    If onResult Is Nothing Then Return
                    onResult(DirectCast(o, TResult))
                End Sub)
        End Function
        Private Sub DisposeCts()
            Try
                _cts?.Dispose()
            Catch
            End Try
            _cts = Nothing
        End Sub

        Private Sub NotifyFailure(ex As Exception)
            Dim Captured = If(ex, New Exception("Unknown error"))
            RaiseOnContext(Sub() RaiseEvent ProcessFailed(Captured))
            SetState(ConsoleProcessRunner.ProcessStateEnum.Error)
        End Sub

        Private Sub OnLibraryDiagnostic(sender As Object, e As LibraryDiagnosticEventArgs)
            If e Is Nothing OrElse String.IsNullOrEmpty(e.Message) Then
                Return
            End If

            EmitOutputLine(e.Message)

            Dim Captured = e
            RaiseOnContext(Sub() RaiseEvent LibraryDiagnostic(Me, Captured))
        End Sub

        Private Sub RaiseOnContext(action As Action)
            Dim ctx = _eventContext
            If ctx Is Nothing OrElse ctx Is SynchronizationContext.Current Then
                action()
            Else
                ctx.Post(Sub(state) action(), Nothing)
            End If
        End Sub

        Private Function RunCore(workSub As Action(Of CancellationToken), workFunc As Func(Of CancellationToken, Object), onResult As Action(Of Object)) As Task
            If IsRunning Then
                Throw New InvalidOperationException("Runner is already busy.")
            End If

            DisposeCts()
            _cts = New CancellationTokenSource()
            Dim Cts = _cts

            SetState(ConsoleProcessRunner.ProcessStateEnum.Running)

            _task = Task.Run(
                Sub()
                    Dim DiagHandler As EventHandler(Of LibraryDiagnosticEventArgs) = AddressOf OnLibraryDiagnostic

                    AddHandler LibraryDiagnostics.MessageEmitted, DiagHandler

                    Try
                        If workSub IsNot Nothing Then
                            workSub(Cts.Token)
                        Else
                            Dim r = workFunc(Cts.Token)
                            If onResult IsNot Nothing Then
                                RaiseOnContext(Sub() onResult(r))
                            End If
                        End If
                        SetState(ConsoleProcessRunner.ProcessStateEnum.Completed)
                    Catch ex As OperationCanceledException
                        SetState(ConsoleProcessRunner.ProcessStateEnum.Aborted)
                    Catch ex As CmdError
                        NotifyFailure(ex)
                    Catch ex As FatalException
                        NotifyFailure(ex)
                    Catch ex As Exception
                        NotifyFailure(ex)
                    Finally
                        RemoveHandler LibraryDiagnostics.MessageEmitted, DiagHandler
                    End Try
                End Sub)

            Return _task
        End Function

        Private Sub SetState(newState As ConsoleProcessRunner.ProcessStateEnum)
            _state = newState
            RaiseOnContext(Sub() RaiseEvent ProcessStateChanged(newState))
        End Sub
    End Class

End Namespace
