Namespace Flux.Greaseweazle
    Partial Public Class ReadDiskForm
        Private ReadOnly _KryofluxStatus As Kryoflux.TrackStatus
        Private ReadOnly _VerifyQueue As New Queue(Of String)

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
            Try
                Process.StartAsync(Flux.Kryoflux.Settings().AppPath, Arguments)
            Catch ex As Exception
                _VerifyQueue.Clear()
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
                    _VerifyQueue.Clear()
                    _KryofluxStatus.UpdateTrackStatusAborted()

                Case ConsoleProcessRunner.ProcessStateEnum.Completed
                    If _VerifyQueue.Count > 0 AndAlso _KryofluxStatus.TrackFound Then
                        _KryofluxStatus.ResetTrackFound()
                        Dim NextArguments = _VerifyQueue.Dequeue()
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
