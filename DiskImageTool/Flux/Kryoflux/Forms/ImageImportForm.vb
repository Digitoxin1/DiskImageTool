Imports System.ComponentModel
Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports DiskImageTool.Flux.Kryoflux.CommandLineBuilder

Namespace Flux.Kryoflux
    Public Class ImageImportForm
        Inherits BaseForm

        Private WithEvents ButtonProcess As Button
        Private WithEvents CheckBoxDoublestep As CheckBox
        Private WithEvents ComboImageFormat As ComboBox
        Private WithEvents TextBoxFileName As TextBox
        Private ReadOnly _Initialized As Boolean = False
        Private ReadOnly _InputFilePath As String
        Private ReadOnly _SideCount As Integer
        Private ReadOnly _TrackCount As Integer
        Private ReadOnly _TrackStatus As TrackStatus
        Private _DoubleStep As Boolean = False
        Private _OutputFilePath As String = ""
        Private _ProcessRunning As Boolean = False

        Public Sub New(FilePath As String, TrackCount As Integer, SideCount As Integer)
            MyBase.New(KryofluxSettings.LogFileName)

            _TrackStatus = New TrackStatus(Me)

            _InputFilePath = FilePath
            _TrackCount = TrackCount
            _SideCount = SideCount
            GridReset(_TrackCount, _SideCount)
            InitializeControls()
            Dim ImageFormat = ReadImageFormat()
            PopulateImageFormats(ComboImageFormat, ImageFormat, ImageFormat)

            SetNewFileName()
            SetTiltebarText()
            ClearStatusBar()
            RefreshButtonState()

            _Initialized = True
        End Sub

        Public ReadOnly Property OutputFilePath As String
            Get
                Return _OutputFilePath
            End Get
        End Property

        Public Function GetNewFileName() As String
            Return TextBoxFileName.Text & ".ima"
        End Function

        Protected Overrides Sub OnAfterBaseFormClosing(e As FormClosingEventArgs)
            If e.CloseReason = CloseReason.UserClosing OrElse CancelButtonClicked Then
                ClearOutputFile()
            End If
        End Sub

        Private Sub ClearOutputFile()
            If Not String.IsNullOrEmpty(_OutputFilePath) Then
                DeleteFileIfExists(_OutputFilePath)
            End If
            _OutputFilePath = ""
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

            Dim Builder = New CommandLineBuilder() With {
                .InFile = _InputFilePath,
                .OutFile = _OutputFilePath,
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
                _OutputFilePath = GetSide0FileName(_OutputFilePath)
            End If

            Return Builder.Arguments
        End Function

        Private Sub InitializeControls()
            Dim FileNameLabel = New Label With {
                .Text = My.Resources.Label_FileName,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            TextBoxFileName = New TextBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .MaxLength = 255
            }

            Dim ImageFormatLabel = New Label With {
                .Text = My.Resources.Label_ImageFormat,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            ComboImageFormat = New ComboBox With {
                .Anchor = AnchorStyles.Left,
                .Width = 200
            }

            CheckBoxDoublestep = New CheckBox With {
                .Text = My.Resources.Label_DoubleStep,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            ButtonProcess = New Button With {
                .Width = 75,
                .Margin = New Padding(12, 24, 3, 3)
            }

            ButtonOk.Text = My.Resources.Label_Import
            ButtonOk.Visible = True

            Dim Row As Integer

            With TableLayoutPanelMain
                .SuspendLayout()

                .Left = 0
                .RowCount = 3
                .ColumnCount = 5
                .Dock = DockStyle.Fill

                While .RowStyles.Count < .RowCount
                    .RowStyles.Add(New RowStyle())
                End While
                For i As Integer = 0 To .RowCount - 1
                    .RowStyles(i).SizeType = SizeType.AutoSize
                Next

                While .ColumnStyles.Count < .ColumnCount
                    .ColumnStyles.Add(New ColumnStyle())
                End While
                For j As Integer = 0 To .ColumnCount - 1
                    .ColumnStyles(j).SizeType = SizeType.AutoSize
                Next

                .ColumnStyles(0).SizeType = SizeType.Percent
                .ColumnStyles(0).Width = 100

                Row = 0
                .Controls.Add(FileNameLabel, 0, Row)
                .Controls.Add(TextBoxFileName, 1, Row)
                .SetColumnSpan(TextBoxFileName, 3)

                Row = 1
                .Controls.Add(ImageFormatLabel, 0, Row)
                .Controls.Add(ComboImageFormat, 1, Row)

                .Controls.Add(CheckBoxDoublestep, 4, Row)

                Row = 2
                .Controls.Add(TableSide0, 0, Row)
                .SetColumnSpan(TableSide0, 2)

                .Controls.Add(TableSide1, 2, Row)
                .SetColumnSpan(TableSide1, 2)

                .Controls.Add(ButtonProcess, 4, Row)

                .ResumeLayout()
                '.Left = (.Parent.ClientSize.Width - .Width) \ 2
            End With
        End Sub

        Private Sub ProcessImage()
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            If DiskParams.IsNonImage Then
                Exit Sub
            End If

            ClearOutputFile()
            ClearStatusBar()

            Dim TempPath = InitTempImagePath()
            Dim FileName = "New Image.ima"

            If TempPath = "" Then
                MsgBox(My.Resources.Dialog_TempPathError, MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            TextBoxConsole.Clear()
            _OutputFilePath = GenerateUniqueFileName(TempPath, FileName)

            _TrackStatus.Clear()
            GridReset(_TrackCount, _SideCount)

            Dim DoubleStep As Boolean = DiskParams.IsStandard AndAlso CheckBoxDoublestep.Enabled AndAlso CheckBoxDoublestep.Checked
            _DoubleStep = DoubleStep

            ToggleProcessRunning(True)

            Dim Arguments = GenerateCommandLine(DiskParams, DoubleStep)
            Process.StartAsync(KryofluxSettings.AppPath, Arguments)
        End Sub

        Private Sub ProcessOutputLine(line As String)
            If TextBoxConsole.Text.Length > 0 Then
                TextBoxConsole.AppendText(Environment.NewLine)
            End If
            TextBoxConsole.AppendText(line)

            Dim TrackSummary = _TrackStatus.ParseTrackSummary(line)
            If TrackSummary IsNot Nothing Then

                Dim TrackInfo = _TrackStatus.ParseTrackInfo(TrackSummary.Details)
                If TrackInfo IsNot Nothing Then
                    Dim StatusInfo = _TrackStatus.UpdateStatusInfo(TrackInfo, TrackSummary.Track, TrackSummary.Side, TrackStatus.ActionTypeEnum.Import)
                    _TrackStatus.UpdateTrackStatus(StatusInfo, TrackStatus.ActionTypeEnum.Read)
                Else
                    Dim StatusInfo = _TrackStatus.UpdateStatusInfo(TrackSummary, TrackStatus.ActionTypeEnum.Import)
                    _TrackStatus.UpdateTrackStatus(StatusInfo, TrackStatus.ActionTypeEnum.Read)
                End If

            End If
        End Sub

        Private Function ReadImageFormat() As DiskImage.FloppyDiskFormat
            Dim Response = ConvertFirstTrack(_InputFilePath)

            If Not Response.Result Then
                Return FloppyDiskFormat.FloppyUnknown
            End If

            Return DetectImageFormat(Response.FileName, True)
        End Function

        Private Sub RefreshButtonState()
            Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            ComboImageFormat.Enabled = Not _ProcessRunning

            ButtonProcess.Enabled = ImageParams.Format <> FloppyDiskFormat.FloppyUnknown
            If _ProcessRunning Then
                ButtonProcess.Text = My.Resources.Label_Abort
            Else
                ButtonProcess.Text = My.Resources.Label_Process
            End If

            ButtonSaveLog.Enabled = Not _ProcessRunning AndAlso TextBoxConsole.Text.Length > 0

            If ImageParams.IsStandard AndAlso ImageParams.MediaType = FloppyMediaType.Media525DoubleDensity Then
                CheckBoxDoublestep.Enabled = Not _ProcessRunning AndAlso _TrackCount > 42
                CheckBoxDoublestep.Checked = _TrackCount > 79
            Else
                CheckBoxDoublestep.Enabled = False
                CheckBoxDoublestep.Checked = False
            End If

            RefreshImportButtonState()
        End Sub

        Private Sub RefreshImportButtonState()
            ButtonOk.Enabled = Not _ProcessRunning AndAlso Not String.IsNullOrEmpty(_OutputFilePath) AndAlso Not String.IsNullOrEmpty(TextBoxFileName.Text)
        End Sub

        Private Sub SetNewFileName()
            Dim FileExt = IO.Path.GetExtension(_InputFilePath).ToLower
            If FileExt = ".raw" Then
                Dim ParentFolder As String = IO.Path.GetFileName(IO.Directory.GetParent(_InputFilePath).FullName)
                TextBoxFileName.Text = ParentFolder
            Else
                TextBoxFileName.Text = IO.Path.GetFileNameWithoutExtension(_InputFilePath)
            End If
        End Sub

        Private Sub SetTiltebarText()
            Dim DisplayFileName = IO.Path.GetFileName(_InputFilePath)
            Dim FileExt = IO.Path.GetExtension(DisplayFileName).ToLower
            If FileExt = ".raw" Then
                Dim ParentFolder As String = IO.Path.GetFileName(IO.Directory.GetParent(_InputFilePath).FullName)
                DisplayFileName = IO.Path.Combine(ParentFolder, "*.raw")
            End If

            Me.Text = My.Resources.Caption_ImportFluxImage & " - " & DisplayFileName
        End Sub
        Private Sub ToggleProcessRunning(Value As Boolean)
            _ProcessRunning = Value
            RefreshButtonState()
        End Sub

#Region "Events"
        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If CancelProcessIfRunning() Then
                Exit Sub
            End If

            ProcessImage()
        End Sub

        Private Sub ComboImageFormat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageFormat.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            RefreshButtonState()
        End Sub

        Private Sub Process_ErrorDataReceived(data As String) Handles Process.ErrorDataReceived, Process.OutputDataReceived
            ProcessOutputLine(data)
        End Sub

        Private Sub Process_ProcessExited(exitCode As Integer) Handles Process.ProcessExited
            Dim Aborted = (exitCode = -1)

            If Aborted Then
                ClearOutputFile()
            End If

            _TrackStatus.UpdateTrackStatusComplete(Aborted)
            ToggleProcessRunning(False)
        End Sub

        Private Sub Process_ProcessFailed(message As String, ex As Exception) Handles Process.ProcessFailed
            _TrackStatus.UpdateTrackStatusError()
            ToggleProcessRunning(False)
        End Sub

        Private Sub TextBoxFileName_TextChanged(sender As Object, e As EventArgs) Handles TextBoxFileName.TextChanged
            RefreshImportButtonState()
        End Sub

        Private Sub TextBoxFileName_Validating(sender As Object, e As CancelEventArgs) Handles TextBoxFileName.Validating
            Dim tb As TextBox = DirectCast(sender, TextBox)
            tb.Text = SanitizeFileName(tb.Text)
            RefreshImportButtonState()
        End Sub
#End Region
    End Class
End Namespace
