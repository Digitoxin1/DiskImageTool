Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Public Class ImageImportForm
        Inherits ImportImageFormBase

        Private ReadOnly _TrackStatus As TrackStatus

        Public Sub New(FilePath As String, TrackCount As Integer, SideCount As Integer)
            MyBase.New(FilePath, TrackCount, SideCount, Settings.LogFileName, True)

            _TrackStatus = New TrackStatus(Me)

            InitializeImage()

            Initialized = True
        End Sub

        Private Sub InitializeImage()
            Dim ImageFormat = ReadImageFormat()
            PopulateImageFormats(ComboImageFormat, ImageFormat, ImageFormat)
            PopulateOutputTypes()
            PopulateFileExtensions()
            InitLoadedImage()
        End Sub

        Private Function IsOutputTypeDisabled() As Boolean
            Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            If Not ImageParams.IsNonImage Then
                Dim imageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(ImageParams.Format)
                Return (imageFormat = GreaseweazleImageFormat.None)
            End If

            Return False
        End Function

        Private Sub PopulateOutputTypes()
            OutputTypeDisabled = IsOutputTypeDisabled()
            Dim DriveList As New List(Of KeyValuePair(Of String, ImageImportOutputTypes))

            If IsOutputTypeDisabled() Then
                DriveList.Add(New KeyValuePair(Of String, ImageImportOutputTypes)(
                    ImageImportOutputTypeDescription(ImageImportOutputTypes.HFE), ImageImportOutputTypes.HFE)
                )
            Else
                For Each OutputType As ImageImportOutputTypes In [Enum].GetValues(GetType(ImageImportOutputTypes))
                    DriveList.Add(New KeyValuePair(Of String, ImageImportOutputTypes)(
                        ImageImportOutputTypeDescription(OutputType), OutputType)
                    )
                Next
            End If

            InitializeCombo(ComboOutputType, DriveList, Nothing)

            ComboOutputType.Enabled = Not OutputTypeDisabled
        End Sub

        Private Sub ProcessImage()
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            If DiskParams.IsNonImage Then
                Exit Sub
            End If

            Dim OutputType As ImageImportOutputTypes = ComboOutputType.SelectedValue

            Dim FilePath = GenerateOutputFile(ImageImportOutputTypeFileExt(OutputType))
            If FilePath = "" Then
                Exit Sub
            End If

            ClearProcessedImage(True)

            OutputFilePath = FilePath

            Dim Arguments = GenerateCommandLineImport(InputFilePath, OutputFilePath, DiskParams, OutputType, DoubleStep)
            Process.StartAsync(Settings.AppPath, Arguments)
        End Sub

        Private Sub ProcessOutputLine(line As String)
            If TextBoxConsole.Text.Length > 0 Then
                TextBoxConsole.AppendText(Environment.NewLine)
            End If
            TextBoxConsole.AppendText(line)

            _TrackStatus.ProcessOutputLineRead(line, TrackStatus.ActionTypeEnum.Import, DoubleStep)
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

            PopulateOutputTypes()
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
                    _TrackStatus.UpdateTrackStatusComplete(DoubleStep)

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
