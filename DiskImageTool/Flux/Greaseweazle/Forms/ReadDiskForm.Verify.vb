Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Partial Public Class ReadDiskForm
        Private ReadOnly _KryofluxStatus As Kryoflux.TrackStatus

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
            If Not CheckBoxSaveLog.Checked Then
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

            Dim DiskParams = SelectedDiskParams

            If Not DiskParams.HasValue OrElse DiskParams.Value.IsNonImage Then
                Exit Sub
            End If

            If Not _KryofluxAvailable Then
                Exit Sub
            End If

            Dim TrackCount As Integer

            If _SelectedDriveOption Is Nothing OrElse _SelectedDriveOption.Type = FloppyDriveType.DriveUnknown Then
                TrackCount = If(DiskParams.Value.DriveType = FloppyDriveType.Drive525DoubleDensity, GreaseweazleSettings.MAX_TRACKS_525DD, GreaseweazleSettings.MAX_TRACKS)
            Else
                TrackCount = _SelectedDriveOption.Tracks
            End If

            Dim Args As (Arguments As String, SingleSide As Boolean)
            Try
                Args = Flux.Kryoflux.GenerateCommandLineImport(_TempFilePath, "", DiskParams.Value, TrackCount, _OutputDoubleStep, Flux.Kryoflux.CommandLineBuilder.LogMask.Format)
            Catch ex As Exception
                HandleRunFailure(ex.Message)
                Exit Sub
            End Try

            ClearLogAndStatus()
            TrackStatus = _KryofluxStatus
            TrackStatus.Clear()
            ResetTrackGrid(ResetSelected:=False)

            InitLogFilePath(GetVerifyLogPath())

            Try
                Process.StartAsync(Flux.Kryoflux.Settings().AppPath, Args.Arguments)
            Catch ex As Exception
                HandleRunFailure(ex.Message)
            End Try
        End Sub

#Region "Events"
        Private Sub Process_DataReceived(Data As String) Handles Process.OutputDataReceived, Process.ErrorDataReceived
            AppendLogLine(Data)
            _KryofluxStatus.ProcessOutputLineRead(Data, _OutputDoubleStep)
        End Sub

        Private Sub Process_ProcessStateChanged(state As ConsoleProcessRunner.ProcessStateEnum) Handles Process.ProcessStateChanged
            Select Case state
                Case ConsoleProcessRunner.ProcessStateEnum.Aborted
                    _KryofluxStatus.UpdateTrackStatusAborted()

                Case ConsoleProcessRunner.ProcessStateEnum.Completed
                    If _KryofluxStatus.TrackFound Then
                        _KryofluxStatus.UpdateTrackStatusComplete(_OutputDoubleStep)
                    Else
                        _KryofluxStatus.UpdateTrackStatusError()
                    End If

                Case ConsoleProcessRunner.ProcessStateEnum.Error
                    _KryofluxStatus.UpdateTrackStatusError()
            End Select

            RefreshFormState()
        End Sub
#End Region
    End Class
End Namespace
