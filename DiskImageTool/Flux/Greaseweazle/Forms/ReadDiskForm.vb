Imports System.ComponentModel
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Public Class ReadDiskForm
        Inherits BaseForm

        Private WithEvents ButtonDetect As Button
        Private WithEvents ButtonProcess As Button
        Private WithEvents ButtonReset As Button
        Private WithEvents CheckBoxDoublestep As CheckBox
        Private WithEvents ComboImageDrives As ComboBox
        Private WithEvents ComboImageFormat As ComboBox
        Private WithEvents ComboOutputType As ComboBox
        Private WithEvents TextBoxFileName As TextBox
        Private ReadOnly _Initialized As Boolean = False
        Private ReadOnly _InputFilePath As String
        Private ReadOnly _TrackStatus As TrackStatus
        Private _CachedOutputTypeValue As GreaseweazleOutputType = GreaseweazleOutputType.IMA
        Private _ComboImageFormatNoEvent As Boolean = False
        Private _DoubleStep As Boolean = False
        Private _OutputFilePath As String = ""
        Private _ProcessRunning As Boolean = False
        Private _TrackRange As ConsoleParser.TrackRange = Nothing
        Private LabelWarning As Label
        Private _NumericRevs As NumericUpDown
        Private _NumericRetries As NumericUpDown
        Private _NumericSeekRetries As NumericUpDown

        Public Sub New()
            MyBase.New(GreaseweazleSettings.LogFileName)
            InitializeControls()

            _TrackStatus = New TrackStatus(Me)

            Me.Text = My.Resources.Label_ReadDisk

            PopulateDrives(ComboImageDrives, FloppyMediaType.MediaUnknown)
            PopulateImageFormats(ComboImageFormat, ComboImageDrives.SelectedValue)
            PopulateOutputTypes()
            ResetTrackGrid()
            ClearStatusBar()
            RefreshButtonState(True)

            _NumericRevs.Value = GreaseweazleSettings.DefaultRevs
            _NumericRetries.Value = CommandLineBuilder.DEFAULT_RETRIES
            _NumericSeekRetries.Value = CommandLineBuilder.DEFAULT_SEEK_RETRIES

            _Initialized = True
        End Sub

        Public ReadOnly Property OutputFilePath As String
            Get
                Return _OutputFilePath
            End Get
        End Property

        Public Function GetNewFileName() As String
            Dim OutputType As GreaseweazleOutputType = ComboOutputType.SelectedValue

            Return TextBoxFileName.Text & GreaseweazleOutputTypeFileExt(OutputType)
        End Function

        Protected Overrides Sub OnAfterBaseFormClosing(e As FormClosingEventArgs)
            If e.CloseReason = CloseReason.UserClosing OrElse CancelButtonClicked Then
                ClearOutputFile()
            End If
        End Sub

        Private Function CheckCompatibility() As Boolean
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            If Opt.Type = FloppyMediaType.MediaUnknown Then
                Return True
            End If

            If DiskParams.IsNonImage Then
                Return True
            End If

            Dim FloppyType = GreaseweazleFindCompatibleFloppyType(DiskParams, Opt.Type)

            Return FloppyType = Opt.Type
        End Function

        Private Sub ClearOutputFile()
            If Not String.IsNullOrEmpty(_OutputFilePath) Then
                DeleteFileIfExists(_OutputFilePath)
            End If
            _OutputFilePath = ""
        End Sub

        Private Sub DoFormatDetection()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            Dim ImageFormat As FloppyDiskFormat

            If Opt.Id = "" Then
                ImageFormat = GreaseweazleImageFormat.None
            Else
                ImageFormat = ReadImageFormat(Opt.Id)
                Opt.SelectedFormat = ImageFormat
                Opt.DetectedFormat = ImageFormat
            End If

            PopulateImageFormats(ComboImageFormat, ImageFormat, ImageFormat)
        End Sub

        Private Sub InitializeControls()
            Dim DriveLabel = New Label With {
                .Text = My.Resources.Label_Drive,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            ComboImageDrives = New ComboBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Width = 180
            }

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
                .Text = My.Resources.Label_Format,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            ComboImageFormat = New ComboBox With {
                .Anchor = AnchorStyles.Left,
                .Width = 200
            }

            Dim OutputTypeLabel = New Label With {
                .Text = My.Resources.Label_OutputType,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            ComboOutputType = New ComboBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Width = 175
            }

            CheckBoxDoublestep = New CheckBox With {
                .Text = My.Resources.Label_DoubleStep,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            Dim SeekRetriesLabel = New Label With {
                .Text = My.Resources.Label_SeekRetries,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            _NumericSeekRetries = New NumericUpDown With {
                .Minimum = CommandLineBuilder.MIN_RETRIES,
                .Maximum = CommandLineBuilder.MAX_RETRIES,
                .Width = 45,
                .Anchor = AnchorStyles.Left
            }

            Dim RetriesLabel = New Label With {
                .Text = My.Resources.Label_Retries,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            _NumericRetries = New NumericUpDown With {
                .Minimum = CommandLineBuilder.MIN_RETRIES,
                .Maximum = CommandLineBuilder.MAX_RETRIES,
                .Width = 45,
                .Anchor = AnchorStyles.Left
            }

            Dim RevsLabel = New Label With {
                .Text = My.Resources.Label_Revs,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True,
                 .Margin = New Padding(12, 3, 3, 3)
            }

            _NumericRevs = New NumericUpDown With {
                .Minimum = CommandLineBuilder.MIN_REVS,
                .Maximum = CommandLineBuilder.MAX_REVS,
                .Width = 40,
                .Anchor = AnchorStyles.Left
            }

            Dim ButtonContainer = New FlowLayoutPanel With {
                .FlowDirection = FlowDirection.TopDown,
                .AutoSize = True,
                .Margin = New Padding(12, 24, 3, 3)
            }

            ButtonProcess = New Button With {
                .Margin = New Padding(3, 0, 3, 3),
                .Text = My.Resources.Label_Write,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonReset = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Reset,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonDetect = New Button With {
                .Width = 75,
                .Margin = New Padding(3, 3, 3, 3),
                .Text = My.Resources.Label_Detect
            }

            ButtonContainer.Controls.Add(ButtonProcess)
            ButtonContainer.Controls.Add(ButtonReset)


            ButtonOk.Text = My.Resources.Label_Import
            ButtonOk.Visible = True

            LabelWarning = New Label With {
                .Text = My.Resources.Message_ImageFormatWarning,
                .ForeColor = Color.Red,
                .AutoSize = True,
                .Anchor = AnchorStyles.Right,
                .Visible = False
            }

            Dim Row As Integer

            With TableLayoutPanelMain
                .SuspendLayout()

                .Left = 0
                .RowCount = 5
                .ColumnCount = 8
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
                .SetColumnSpan(TextBoxFileName, 6)

                Row = 1
                .Controls.Add(DriveLabel, 0, Row)
                .Controls.Add(ComboImageDrives, 1, Row)

                .Controls.Add(OutputTypeLabel, 2, Row)
                .Controls.Add(ComboOutputType, 3, Row)
                .SetColumnSpan(ComboOutputType, 4)

                .Controls.Add(CheckBoxDoublestep, 7, Row)

                Row = 2
                .Controls.Add(ImageFormatLabel, 0, Row)
                .Controls.Add(ComboImageFormat, 1, Row)
                .Controls.Add(ButtonDetect, 2, Row)

                .Controls.Add(RevsLabel, 3, Row)
                .Controls.Add(_NumericRevs, 4, Row)

                .Controls.Add(RetriesLabel, 5, Row)
                .Controls.Add(_NumericRetries, 6, Row)

                Row = 3
                .Controls.Add(LabelWarning, 0, Row)
                .SetColumnSpan(LabelWarning, 2)

                .Controls.Add(SeekRetriesLabel, 4, Row)
                .SetColumnSpan(SeekRetriesLabel, 2)
                .Controls.Add(_NumericSeekRetries, 6, Row)

                Row = 4
                .Controls.Add(TableSide0, 0, Row)
                .SetColumnSpan(TableSide0, 2)

                .Controls.Add(TableSide1, 2, Row)
                .SetColumnSpan(TableSide1, 5)

                .Controls.Add(ButtonContainer, 4, Row)

                .ResumeLayout()
                '.Left = (.Parent.ClientSize.Width - .Width) \ 2
            End With
        End Sub

        Private Sub PopulateOutputTypes()
            Dim DriveList As New List(Of KeyValuePair(Of String, GreaseweazleOutputType))
            For Each OutputType As GreaseweazleOutputType In [Enum].GetValues(GetType(GreaseweazleOutputType))
                DriveList.Add(New KeyValuePair(Of String, GreaseweazleOutputType)(
                    GreaseweazleOutputTypeDescription(OutputType), OutputType)
                )
            Next

            InitializeCombo(ComboOutputType, DriveList, Nothing)
        End Sub

        Private Sub ProcessImage()
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            If DiskParams.IsNonImage Then
                Exit Sub
            End If

            If Opt.Id = "" Then
                Exit Sub
            End If

            ClearOutputFile()
            ClearStatusBar()

            Dim OutputType As GreaseweazleOutputType = ComboOutputType.SelectedValue

            Dim TempPath = InitTempImagePath()
            Dim FileName = "New Image" & GreaseweazleOutputTypeFileExt(OutputType)

            If TempPath = "" Then
                MsgBox(My.Resources.Dialog_TempPathError, MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            TextBoxConsole.Clear()
            _OutputFilePath = GenerateUniqueFileName(TempPath, FileName)

            _TrackStatus.Clear()
            ResetTrackGrid()

            Dim DoubleStep As Boolean = DiskParams.IsStandard AndAlso CheckBoxDoublestep.Enabled AndAlso CheckBoxDoublestep.Checked
            _DoubleStep = DoubleStep

            Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.read) With {
                .Drive = Opt.Id,
                .File = _OutputFilePath,
                .Retries = _NumericRetries.Value,
                .SeekRetries = _NumericSeekRetries.Value,
                .Revs = _NumericRevs.Value
            }

            If Not DiskParams.IsStandard Then
                OutputType = GreaseweazleOutputType.HFE
            End If

            Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(DiskParams.Format)

            If OutputType <> GreaseweazleOutputType.HFE OrElse ImageFormat <> GreaseweazleImageFormat.None Then
                Builder.Format = GreaseweazleImageFormatCommandLine(ImageFormat)
            End If

            If OutputType = GreaseweazleOutputType.HFE Then
                Builder.BitRate = DiskParams.BitRateKbps
                Builder.AdjustSpeed = DiskParams.RPM & "rpm"
                Builder.Raw = True
            End If

            If DoubleStep Then
                Builder.HeadStep = 2
            End If

            Dim Arguments = Builder.Arguments

            ToggleProcessRunning(True)
            Process.StartAsync(GreaseweazleSettings.AppPath, Arguments)
        End Sub

        Private Sub ProcessOutputLine(line As String)
            If TextBoxConsole.Text.Length > 0 Then
                TextBoxConsole.AppendText(Environment.NewLine)
            End If
            TextBoxConsole.AppendText(line)

            If _TrackRange Is Nothing Then
                _TrackRange = _TrackStatus.ParseDiskRange(line)
            End If

            Dim Summary = _TrackStatus.ParseTrackReadSummary(line)
            If Summary IsNot Nothing Then
                Dim Details = _TrackStatus.ParseTrackReadDetails(Summary.Details)
                If Details IsNot Nothing Then
                    Dim Statusinfo = _TrackStatus.UpdateStatusInfo(Summary, Details, False, TrackStatus.ActionTypeEnum.Read)
                    _TrackStatus.UpdateTrackStatus(Statusinfo, TrackStatus.ActionTypeEnum.Read, _DoubleStep)
                    Return
                End If

                Dim FailedSectors = _TrackStatus.ParseTrackReadFailed(Summary.Details)
                If FailedSectors.HasValue Then
                    Dim Statusinfo = _TrackStatus.UpdateStatusInfo(Summary, FailedSectors.Value, TrackStatus.ActionTypeEnum.Read)
                    _TrackStatus.UpdateTrackStatus(Statusinfo, TrackStatus.ActionTypeEnum.Read, _DoubleStep)
                    Return
                End If
            End If

            Dim TrackInfoUnexpected = _TrackStatus.ParseTrackUnexpected(line)
            If TrackInfoUnexpected IsNot Nothing Then
                Dim StatusInfo = _TrackStatus.UpdateStatusInfo(TrackInfoUnexpected, TrackStatus.ActionTypeEnum.Read)
                _TrackStatus.UpdateTrackStatus(StatusInfo, TrackStatus.ActionTypeEnum.Read, _DoubleStep)
                Return
            End If
        End Sub

        Private Function ReadImageFormat(DriveId As String) As DiskImage.FloppyDiskFormat
            Dim Response = ReadFirstTrack(DriveId)

            TextBoxConsole.Text = Response.Output

            If Not Response.Result Then
                Return GreaseweazleImageFormat.None
            End If

            Return DetectImageFormat(Response.FileName, True)
        End Function

        Private Sub RefreshButtonState(CheckImageFormat As Boolean)
            Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            Dim OutputTypeDisabled As Boolean = False

            If CheckImageFormat Then
                If ImageParams.IsNonImage Then
                    OutputTypeDisabled = False
                Else
                    Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(ImageParams.Format)
                    OutputTypeDisabled = (ImageFormat = GreaseweazleImageFormat.None)
                End If

                If ComboOutputType.Enabled AndAlso OutputTypeDisabled Then
                    _CachedOutputTypeValue = ComboOutputType.SelectedValue
                    ComboOutputType.SelectedValue = GreaseweazleOutputType.HFE
                ElseIf Not ComboOutputType.Enabled And Not OutputTypeDisabled Then
                    ComboOutputType.SelectedValue = _CachedOutputTypeValue
                End If
            End If

            ComboImageFormat.Enabled = Not _ProcessRunning AndAlso Opt.Id <> ""
            ComboImageDrives.Enabled = Not _ProcessRunning
            ComboOutputType.Enabled = Not _ProcessRunning And Not OutputTypeDisabled

            _NumericRevs.Enabled = Not _ProcessRunning
            _NumericRetries.Enabled = Not _ProcessRunning
            _NumericSeekRetries.Enabled = Not _ProcessRunning

            ButtonProcess.Enabled = ImageParams.Format <> FloppyDiskFormat.FloppyUnknown
            If _ProcessRunning Then
                ButtonProcess.Text = My.Resources.Label_Abort
            Else
                ButtonProcess.Text = My.Resources.Label_Read
            End If

            ButtonSaveLog.Enabled = Not _ProcessRunning AndAlso TextBoxConsole.Text.Length > 0

            If ImageParams.IsStandard AndAlso ImageParams.MediaType = FloppyMediaType.Media525DoubleDensity Then
                CheckBoxDoublestep.Enabled = Not _ProcessRunning AndAlso Opt.Tracks > 42
                CheckBoxDoublestep.Checked = Opt.Tracks > 79
            Else
                CheckBoxDoublestep.Enabled = False
                CheckBoxDoublestep.Checked = False
            End If

            ButtonDetect.Enabled = Not _ProcessRunning AndAlso Opt.Id <> ""

            RefreshImportButtonState()
        End Sub

        Private Sub RefreshImportButtonState()
            ButtonOk.Enabled = Not _ProcessRunning AndAlso Not String.IsNullOrEmpty(_OutputFilePath) AndAlso Not String.IsNullOrEmpty(TextBoxFileName.Text)
        End Sub

        Private Sub ResetTrackGrid(Optional ResetSelected As Boolean = True)
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            Dim SideCount As Byte
            Dim FormatMediaType As FloppyMediaType

            If DiskParams.IsNonImage Then
                SideCount = 2
                FormatMediaType = FloppyMediaType.MediaUnknown
            Else
                SideCount = DiskParams.BPBParams.NumberOfHeads
                FormatMediaType = DiskParams.MediaType
            End If

            Dim TrackCount As UShort
            If Opt.Type = FloppyMediaType.MediaUnknown Then
                If FormatMediaType = FloppyMediaType.Media525DoubleDensity Then
                    TrackCount = Settings.MAX_TRACKS_525DD
                Else
                    TrackCount = Settings.MAX_TRACKS
                End If
            Else
                TrackCount = Opt.Tracks
            End If

            TrackCount = Math.Max(TrackCount, Opt.Tracks)

            GridReset(TrackCount, SideCount, ResetSelected)
        End Sub

        Private Sub ToggleProcessRunning(Value As Boolean)
            _ProcessRunning = Value
            RefreshButtonState(False)
        End Sub

#Region "Events"
        Private Sub ButtonDetect_Click(sender As Object, e As EventArgs) Handles ButtonDetect.Click
            DoFormatDetection()
        End Sub

        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If CancelProcessIfRunning() Then
                Exit Sub
            End If

            ProcessImage()
        End Sub

        Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
            Reset(TextBoxConsole)
        End Sub

        Private Sub ComboImageDrives_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageDrives.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            _ComboImageFormatNoEvent = True
            PopulateImageFormats(ComboImageFormat, Opt)
            _ComboImageFormatNoEvent = False

            ResetTrackGrid()
            RefreshButtonState(False)
            LabelWarning.Visible = Not CheckCompatibility()
        End Sub

        Private Sub ComboImageFormat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageFormat.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboImageFormatNoEvent Then
                Exit Sub
            End If

            Dim Opt As DriveOption = ComboImageDrives.SelectedValue
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            If Opt.Id <> "" Then
                Opt.SelectedFormat = DiskParams.Format
            End If


            ResetTrackGrid()
            RefreshButtonState(True)
            LabelWarning.Visible = Not CheckCompatibility()
        End Sub
        Private Sub Process_ErrorDataReceived(data As String) Handles Process.ErrorDataReceived
            ProcessOutputLine(data)
        End Sub

        Private Sub Process_ProcessExited(exitCode As Integer) Handles Process.ProcessExited
            Dim Aborted = (exitCode = -1)

            If Aborted Then
                ClearOutputFile()
            End If

            _TrackStatus.UpdateTrackStatusComplete(Aborted, _DoubleStep)
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
