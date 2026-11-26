Namespace Flux.Greaseweazle
    Partial Public Class ReadDiskForm
        Private ReadOnly _KryofluxStatus As Kryoflux.TrackStatus
        Private _VerifyPauseTrack As Integer = -1
        Private _VerifyPauseMaxTrack As Integer = -1

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

            Dim Arguments As String
            Dim PartialVerify = HasSelectedTracks

            Try
                If PartialVerify Then
                    Dim Ranges = GetSelectedTrackRanges(DtcDoubleStep)
                    If Ranges.Count = 0 Then
                        Exit Sub
                    End If

                    Dim StartTrack = Ranges.Min(Function(Range) CInt(Range.StartTrack))
                    Dim EndTrack = Ranges.Max(Function(Range) CInt(Range.EndTrack))
                    Dim Heads = GetSelectedTrackHeads()
                    Dim Args = Flux.Kryoflux.GenerateCommandLineImport(_TempFilePath, "", DiskParams.Value, TrackCount, DtcDoubleStep, Flux.Kryoflux.CommandLineBuilder.LogMask.Format, StartTrack, EndTrack, Heads)
                    Arguments = Args.Arguments
                Else
                    Dim IncompleteEnd As Integer = -1

                    If Not _DumpComplete Then
                        Dim LastTrack = LastCompletedDisplayTrack()
                        If LastTrack < 0 Then
                            Exit Sub
                        End If
                        IncompleteEnd = DisplayTrackCylinder(LastTrack)
                    End If

                    If IncompleteEnd >= 0 Then
                        Dim Args = Flux.Kryoflux.GenerateCommandLineImport(_TempFilePath, "", DiskParams.Value, TrackCount, DtcDoubleStep, Flux.Kryoflux.CommandLineBuilder.LogMask.Format, 0, IncompleteEnd)
                        Arguments = Args.Arguments
                    Else
                        Dim Args = Flux.Kryoflux.GenerateCommandLineImport(_TempFilePath, "", DiskParams.Value, TrackCount, DtcDoubleStep, Flux.Kryoflux.CommandLineBuilder.LogMask.Format)
                        Arguments = Args.Arguments
                    End If
                End If
            Catch ex As Exception
                HandleRunFailure(ex.Message)
                Exit Sub
            End Try

            If String.IsNullOrEmpty(Arguments) Then
                Exit Sub
            End If

            ClearLogAndStatus()
            TrackStatus = _KryofluxStatus
            TrackStatus.Clear()
            _KryofluxStatus.ReportLogicalTracks = DtcDoubleStep

            If PartialVerify Then
                GridResetSelectedCells()
                InitLogFilePath("")
            Else
                _VerifyPauseTrack = PauseTrack
                _VerifyPauseMaxTrack = PauseMaxTrack
                ResetTrackGrid(ResetSelected:=False)
                InitLogFilePath(GetVerifyLogPath())
            End If
            StartVerifyCommand(Arguments)
        End Sub

        Private Sub StartVerifyCommand(Arguments As String)
            Try
                Process.StartAsync(Flux.Kryoflux.Settings().AppPath, Arguments)
            Catch ex As Exception
                HandleRunFailure(ex.Message)
            End Try
        End Sub

#Region "Events"
        Private Sub Process_DataReceived(Data As String) Handles Process.OutputDataReceived, Process.ErrorDataReceived
            AppendLogLine(Data)
            _KryofluxStatus.ProcessOutputLineRead(Data, _OutputLayout.DisplayDoubleStep)
        End Sub

        Private Sub Process_ProcessStateChanged(state As ConsoleProcessRunner.ProcessStateEnum) Handles Process.ProcessStateChanged
            Select Case state
                Case ConsoleProcessRunner.ProcessStateEnum.Aborted
                    _KryofluxStatus.UpdateTrackStatusAborted()

                Case ConsoleProcessRunner.ProcessStateEnum.Completed
                    If _KryofluxStatus.TrackFound Then
                        _KryofluxStatus.UpdateTrackStatusComplete(_OutputLayout.DisplayDoubleStep)
                    Else
                        _KryofluxStatus.UpdateTrackStatusError()
                    End If

                Case ConsoleProcessRunner.ProcessStateEnum.Error
                    _KryofluxStatus.UpdateTrackStatusError()
            End Select

            Select Case state
                Case ConsoleProcessRunner.ProcessStateEnum.Completed,
                     ConsoleProcessRunner.ProcessStateEnum.Error,
                     ConsoleProcessRunner.ProcessStateEnum.Aborted
                    RestoreVerifyPauseMark()
            End Select

            RefreshFormState()
        End Sub

        Private Sub RestoreVerifyPauseMark()
            If _VerifyPauseTrack < 0 Then
                Return
            End If

            Dim Track = _VerifyPauseTrack
            Dim MaxTrack = _VerifyPauseMaxTrack
            _VerifyPauseTrack = -1
            _VerifyPauseMaxTrack = -1
            PlacePauseMark(Track, MaxTrack)
        End Sub
#End Region
    End Class
End Namespace
