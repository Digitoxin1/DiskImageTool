Imports System.Text.RegularExpressions

Namespace Flux.Greaseweazle
    Partial Public Class ReadDiskForm
        Private ReadOnly _KryofluxStatus As Kryoflux.TrackStatus
        Private ReadOnly _VerifyQueue As New Queue(Of String)
        Private ReadOnly _VerifyTrackLine As New Regex("^\d+\.\d+\s+:", RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private _VerifyRunCount As Integer
        Private _VerifyRunIndex As Integer
        Private _VerifySeenTrack As Boolean

        Private Shared Function GetVerifyLogFileName() As String
            Dim KfLogName = Flux.Kryoflux.Settings().LogFileName

            If String.IsNullOrEmpty(KfLogName) Then
                Return ""
            End If

            Dim GwLogName = Settings.LogFileName
            If String.Equals(KfLogName, GwLogName, StringComparison.OrdinalIgnoreCase) Then
                Dim Base = IO.Path.GetFileNameWithoutExtension(KfLogName)
                Dim Ext = IO.Path.GetExtension(KfLogName)
                KfLogName = Base & ".verify" & Ext
            End If

            Return KfLogName
        End Function

        Private Shared Sub UpdateVerifyLogPaths(destFolder As String)
            Dim LogName = GetVerifyLogFileName()

            If String.IsNullOrEmpty(LogName) Then
                Exit Sub
            End If

            Dim LogPath = IO.Path.Combine(destFolder, LogName)
            If Not IO.File.Exists(LogPath) Then
                Exit Sub
            End If

            Try
                Dim Content = IO.File.ReadAllText(LogPath)
                Content = RemovePathFromLog(Content, IO.Path.GetFileName(destFolder))
                IO.File.WriteAllText(LogPath, Content)
            Catch ex As Exception
                Debug.WriteLine($"UpdateVerifyLogPaths failed: {ex.Message}")
            End Try
        End Sub

        Private Function GetVerifyLogPath() As String
            If Not CheckSaveLog.Checked Then
                Return ""
            End If

            Dim Name = GetVerifyLogFileName()

            If String.IsNullOrEmpty(Name) Then
                Return ""
            End If

            Return IO.Path.Combine(IO.Path.GetDirectoryName(_TempFilePath), Name)
        End Function

        Private Sub VerifyImage()
            If Not HasOutputFile OrElse Not IsFluxOutput Then
                Exit Sub
            End If

            Dim DiskParams = _OutputDiskParams

            If Not DiskParams.HasValue OrElse DiskParams.Value.IsNonImage Then
                Exit Sub
            End If

            If Not _KryofluxAvailable Then
                Exit Sub
            End If

            Dim TrackCount As Integer = GetGridTrackCount()
            Dim DtcDoubleStep = _OutputLayout.PhysicalEvenFiles

            If DtcDoubleStep Then
                TrackCount *= 2
            End If

            Dim Commands As New List(Of String)
            Dim PartialVerify = HasSelectedTracks

            Try
                If PartialVerify Then
                    Dim Ranges = GetSelectedTrackRanges(DtcDoubleStep)
                    Dim Heads = GetSelectedTrackHeads()

                    For Each Range In Ranges
                        Dim Args = Flux.Kryoflux.GenerateCommandLineImport(_TempFilePath, "", DiskParams.Value, TrackCount, DtcDoubleStep, Flux.Kryoflux.CommandLineBuilder.LogMask.Format, Range.StartTrack, Range.EndTrack, Heads)
                        Commands.Add(Args.Arguments)
                    Next
                Else
                    Dim Args = Flux.Kryoflux.GenerateCommandLineImport(_TempFilePath, "", DiskParams.Value, TrackCount, DtcDoubleStep, Flux.Kryoflux.CommandLineBuilder.LogMask.Format)
                    Commands.Add(Args.Arguments)
                End If
            Catch ex As Exception
                HandleRunFailure(ex.Message)
                Exit Sub
            End Try

            If Commands.Count = 0 Then
                Exit Sub
            End If

            _VerifyQueue.Clear()
            For i = 1 To Commands.Count - 1
                _VerifyQueue.Enqueue(Commands(i))
            Next

            _VerifyRunCount = Commands.Count
            _VerifyRunIndex = 0
            _VerifySeenTrack = False

            ClearLogAndStatus()
            TrackStatus = _KryofluxStatus
            TrackStatus.Clear()
            _KryofluxStatus.ReportLogicalTracks = DtcDoubleStep

            If PartialVerify Then
                GridResetSelectedCells()
            Else
                ResetTrackGrid(ResetSelected:=False)
            End If

            InitLogFilePath(GetVerifyLogPath())
            StartVerifyCommand(Commands(0))
        End Sub

        Private Sub StartVerifyCommand(Arguments As String)
            _VerifySeenTrack = False

            Try
                Process.StartAsync(Flux.Kryoflux.Settings().AppPath, Arguments)
            Catch ex As Exception
                _VerifyQueue.Clear()
                HandleRunFailure(ex.Message)
            End Try
        End Sub

        Private Function ShouldLogVerifyLine(Data As String) As Boolean
            If _VerifyRunCount <= 1 Then
                Return True
            End If

            Dim Line = If(Data, "").Trim()
            If _VerifyTrackLine.IsMatch(Line) Then
                _VerifySeenTrack = True
                Return True
            End If

            If IsVerifyHeaderLine(Line) OrElse (Line.Length = 0 AndAlso Not _VerifySeenTrack) Then
                Return _VerifyRunIndex = 0
            End If

            If IsVerifyFooterLine(Line) OrElse (Line.Length = 0 AndAlso _VerifySeenTrack) Then
                Return _VerifyRunIndex = _VerifyRunCount - 1
            End If

            Return True
        End Function

        Private Shared Function IsVerifyHeaderLine(Line As String) As Boolean
            Return Line.StartsWith("KryoFlux DiskTool Console", StringComparison.OrdinalIgnoreCase) OrElse
                Line.StartsWith("(c) ", StringComparison.Ordinal) OrElse
                Line.StartsWith("Developed by ", StringComparison.Ordinal) OrElse
                Line.StartsWith("Licensed for ", StringComparison.Ordinal) OrElse
                Line.StartsWith("Stream file:", StringComparison.OrdinalIgnoreCase)
        End Function

        Private Shared Function IsVerifyFooterLine(Line As String) As Boolean
            Return Line.StartsWith("Enjoy your shiny new disk image!", StringComparison.Ordinal) OrElse
                Line.StartsWith("Please consider helping us", StringComparison.Ordinal) OrElse
                Line.StartsWith("www.softpres.org/donate", StringComparison.OrdinalIgnoreCase)
        End Function

#Region "Events"
        Private Sub Process_DataReceived(Data As String) Handles Process.OutputDataReceived, Process.ErrorDataReceived
            If ShouldLogVerifyLine(Data) Then
                AppendLogLine(Data)
            End If
            _KryofluxStatus.ProcessOutputLineRead(Data, _OutputLayout.DisplayDoubleStep)
        End Sub

        Private Sub Process_ProcessStateChanged(state As ConsoleProcessRunner.ProcessStateEnum) Handles Process.ProcessStateChanged
            Select Case state
                Case ConsoleProcessRunner.ProcessStateEnum.Aborted
                    _VerifyQueue.Clear()
                    _KryofluxStatus.UpdateTrackStatusAborted()

                Case ConsoleProcessRunner.ProcessStateEnum.Completed
                    If _VerifyQueue.Count > 0 AndAlso _KryofluxStatus.TrackFound Then
                        _KryofluxStatus.ResetTrackFound()
                        Dim NextArguments = _VerifyQueue.Dequeue()
                        _VerifyRunIndex += 1
                        StartVerifyCommand(NextArguments)
                    ElseIf _KryofluxStatus.TrackFound Then
                        _VerifyQueue.Clear()
                        _KryofluxStatus.UpdateTrackStatusComplete(_OutputLayout.DisplayDoubleStep)
                    Else
                        _VerifyQueue.Clear()
                        _KryofluxStatus.UpdateTrackStatusError()
                    End If

                Case ConsoleProcessRunner.ProcessStateEnum.Error
                    _VerifyQueue.Clear()
                    _KryofluxStatus.UpdateTrackStatusError()
            End Select

            RefreshFormState()
        End Sub
#End Region
    End Class
End Namespace
