Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports DiskImageTool.Flux.Kryoflux.CommandLineBuilder

Namespace Flux.Kryoflux
    Public Class ImageImportForm
        Inherits ImportImageFormBase

        Private ReadOnly _TrackStatus As TrackStatus

        Public Sub New(FilePath As String, TrackCount As Integer, SideCount As Integer)
            MyBase.New(FilePath, TrackCount, SideCount, Settings.LogFileName, False)

            _TrackStatus = New TrackStatus(Me)

            InitializeImage()

            Initialized = True
        End Sub

        Private Function GenerateCommandLine(DiskParams As FloppyDiskParams, DoubleStep As Boolean) As String
            Dim SingleSidedMode As SingleSidedModeEnum = SingleSidedModeEnum.off

            If DiskParams.Format = FloppyDiskFormat.Floppy160 Or DiskParams.Format = FloppyDiskFormat.Floppy180 Then
                SingleSidedMode = SingleSidedModeEnum.side0
            End If

            Dim TrackCount = DiskParams.BPBParams.TrackCount
            Dim TrackDistance As TrackDistanceEnum = TrackDistanceEnum.Tracks80
            If DoubleStep Then
                TrackCount *= 2
                TrackDistance = TrackDistanceEnum.Tracks40
            End If

            Dim Builder As New CommandLineBuilder() With {
                .InFile = InputFilePath,
                .OutFile = OutputFilePath,
                .DeviceMode = DeviceModeEnum.ImageFile,
                .InImageType = ImageTypeEnum.KryoFlux_Stream,
                .OutImageType = ImageTypeEnum.MFM_SectorImage,
                .LogLevel = LogMask.Format,
                .TrackStart = 0,
                .TrackEnd = TrackCount - 1,
                .OutputTrackStart = 0,
                .OutputTrackEnd = TrackCount - 1,
                .SingleSidedMode = SingleSidedMode,
                .RPM = DiskParams.RPM,
                .TrackDistance = TrackDistance,
                .SectorCount = DiskParams.BPBParams.SectorsPerTrack,
                .MissingSectorsAsBad = False
            }

            If SingleSidedMode = SingleSidedModeEnum.side0 Then
                OutputFilePath = GetSide0FileName(OutputFilePath)
            End If

            Return Builder.Arguments
        End Function

        Private Sub InitializeImage()
            Dim ImageFormat = ReadImageFormat()
            PopulateImageFormats(ComboImageFormat, ImageFormat, ImageFormat)
            PopulateFileExtensions()
            InitLoadedImage()
        End Sub

        Private Sub ProcessImage()
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            If DiskParams.IsNonImage Then
                Exit Sub
            End If

            Dim FilePath = GenerateOutputFile(".ima")
            If FilePath = "" Then
                Exit Sub
            End If

            ClearProcessedImage(True)

            OutputFilePath = FilePath

            Dim Arguments = GenerateCommandLine(DiskParams, DoubleStep)
            Process.StartAsync(Settings.AppPath, Arguments)
        End Sub

        Private Sub ProcessOutputLine(line As String)
            If TextBoxConsole.Text.Length > 0 Then
                TextBoxConsole.AppendText(Environment.NewLine)
            End If
            TextBoxConsole.AppendText(line)

            _TrackStatus.ProcessOutputLineRead(line)
        End Sub

        Private Function ReadImageFormat() As DiskImage.FloppyDiskFormat
            Dim Response = ConvertFirstTrack(InputFilePath)

            If Not Response.Result Then
                Return FloppyDiskFormat.FloppyUnknown
            End If

            Return DetectImageFormat(Response.FileName, True)
        End Function


#Region "Events"
        Private Sub ButtonOpen_Click(sender As Object, e As EventArgs) Handles ButtonOpen.Click
            If OpenFluxImage() Then
                InitializeImage()
            End If
        End Sub

        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If CancelProcessIfRunning() Then
                Exit Sub
            End If

            ProcessImage()
        End Sub

        Private Sub ComboImageFormat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageFormat.SelectedIndexChanged
            If Not Initialized Then
                Exit Sub
            End If

            PopulateFileExtensions()
            RefreshButtonState()
        End Sub

        Private Sub Parent_FileDropped(sender As Object, message As String) Handles Me.FileDropped
            If OpenFluxImage(message) Then
                InitializeImage()
            End If
        End Sub

        Private Sub Parent_ProcessedImageCleared(sender As Object, DeleteOutputFile As Boolean) Handles Me.ProcessedImageCleared
            _TrackStatus.Clear()
        End Sub

        Private Sub Process_DataReceived(Data As String) Handles Process.ErrorDataReceived, Process.OutputDataReceived
            ProcessOutputLine(Data)
        End Sub

        Private Sub Process_ProcessStateChanged(State As ConsoleProcessRunner.ProcessStateEnum) Handles Process.ProcessStateChanged
            Select Case State
                Case ConsoleProcessRunner.ProcessStateEnum.Aborted
                    ClearOutputFile(True)
                    _TrackStatus.UpdateTrackStatusAborted()

                Case ConsoleProcessRunner.ProcessStateEnum.Completed
                    If _TrackStatus.TrackFound Then
                        _TrackStatus.UpdateTrackStatusComplete()
                    Else
                        ClearOutputFile(True)
                        _TrackStatus.UpdateTrackStatusError()
                    End If

                Case ConsoleProcessRunner.ProcessStateEnum.Error
                    ClearOutputFile(True)
                    _TrackStatus.UpdateTrackStatusError()
            End Select

            RefreshButtonState()
        End Sub

        Private Sub TextBoxFileName_Click(sender As Object, e As EventArgs) Handles TextBoxFileName.Click
            If TextBoxFileName.ReadOnly Then
                If OpenFluxImage() Then
                    InitializeImage()
                End If
            End If
        End Sub
#End Region
    End Class
End Namespace
