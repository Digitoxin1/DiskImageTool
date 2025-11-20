Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Public Class WriteDiskForm
        Inherits BaseForm
        Private WithEvents ButtonProcess As Button
        Private WithEvents ButtonReset As Button
        Private WithEvents CheckboxNoVerify As CheckBox
        Private WithEvents CheckBoxSelect As CheckBox
        Private WithEvents ComboImageDrives As ComboBox
        Private ReadOnly _Disk As DiskImage.Disk
        Private ReadOnly _DiskParams As FloppyDiskParams
        Private ReadOnly _Initialized As Boolean = False
        Private ReadOnly _IsBitstreamImage As Boolean
        Private ReadOnly _SideCount As Byte
        Private ReadOnly _TrackCount As UShort
        Private ReadOnly _TrackStatus As TrackStatus
        Private _ContinueAfterWrite As Boolean = False
        Private _CurrentFilePath As String = ""
        Private _DoubleStep As Boolean = False
        Private _LastSelected As (Track As UShort, Side As Byte, Selected As Boolean)? = Nothing
        Private CheckBoxContinue As CheckBox
        Private CheckBoxEraseEmpty As CheckBox
        Private CheckBoxPreErase As CheckBox
        Private LabelImageFormat As Label
        Private LabelWarning As Label
        Private NumericRetries As NumericUpDown

        Public Sub New(Disk As DiskImage.Disk, FileName As String)
            MyBase.New(Settings.LogFileName)
            InitializeControls()

            _TrackStatus = New TrackStatus(Me)

            _Disk = Disk
            _DiskParams = Disk.DiskParams

            Me.Text = My.Resources.Label_WriteDisk & " - " & FileName

            If Disk.Image.IsBitstreamImage Then
                _TrackCount = Disk.Image.TrackCount
                _SideCount = Disk.Image.SideCount
                _IsBitstreamImage = True
            Else
                If _DiskParams.Format = FloppyDiskFormat.FloppyUnknown Then
                    Dim DiskFormat = FloppyDiskFormatGet(Disk.Image.Length)
                    _DiskParams = FloppyDiskFormatGetParams(DiskFormat)
                End If
                _TrackCount = _DiskParams.BPBParams.TrackCount
                _SideCount = _DiskParams.BPBParams.NumberOfHeads
                _IsBitstreamImage = False
            End If

            NumericRetries.Value = CommandLineBuilder.DEFAULT_RETRIES

            LabelImageFormat.Text = _DiskParams.Description

            Dim Mode As String = If(_IsBitstreamImage, My.Resources.FileType_BitstreamImage, My.Resources.FileType_SectorImage)
            StatusStripBottom.Items.Add(New ToolStripStatusLabel(Mode))

            Dim AvailableTypes = Settings.AvailableDriveTypes
            Dim SelectedFormat = GreaseweazleFindCompatibleFloppyType(_DiskParams, AvailableTypes)

            PopulateDrives(ComboImageDrives, SelectedFormat)
            ResetState()
            ResetCheckBoxSelect()
            RefreshButtonState()
            LabelWarning.Visible = Not CheckCompatibility()

            _Initialized = True
        End Sub

        Private Function CheckCompatibility() As Boolean
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            If Opt.Type = FloppyMediaType.MediaUnknown Then
                Return True
            End If

            Dim FloppyType = GreaseweazleFindCompatibleFloppyType(_DiskParams, Opt.Type)

            Return FloppyType = Opt.Type
        End Function

        Private Function CheckKeepProcessing() As Boolean
            Dim KeepProcessing As Boolean = _ContinueAfterWrite OrElse (RetriesAllowed() AndAlso CheckBoxContinue.Checked AndAlso CheckBoxSelect.Checked)

            If KeepProcessing Then
                KeepProcessing = _TrackStatus.CanKeepProcessing
            End If

            Return KeepProcessing
        End Function

        Private Function ConfirmIncompatibleImage() As Boolean
            Dim Msg = String.Format(My.Resources.Dialog_ImageFormatWarning, vbNewLine)
            Return MsgBox(Msg, MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes
        End Function

        Private Function GetEndTrackFromDriveOption(Opt As DriveOption) As UShort
            Dim TrackCount As Integer = Opt.Tracks

            If _DiskParams.MediaType = FloppyMediaType.Media525DoubleDensity Then
                If Opt.Type <> FloppyMediaType.Media525DoubleDensity AndAlso Opt.Type <> FloppyMediaType.Media525HighDensity Then
                    TrackCount = GreaseweazleSettings.MAX_TRACKS_525DD
                End If
            ElseIf Opt.Type = FloppyMediaType.Media525DoubleDensity AndAlso _TrackCount > GreaseweazleSettings.MAX_TRACKS_525DD Then
                TrackCount = GreaseweazleSettings.MIN_TRACKS_525DD
            End If

            If _DoubleStep Then
                TrackCount = Math.Ceiling(TrackCount / 2)
            End If

            Return TrackCount - 1
        End Function

        Private Sub InitializeControls()
            Dim DriveLabel As New Label With {
                .Text = My.Resources.Label_Drive,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            ComboImageDrives = New ComboBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Width = 180
            }

            Dim RetriesLabel As New Label With {
                .Text = My.Resources.Label_Retries,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True
            }

            NumericRetries = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 45,
                .Minimum = CommandLineBuilder.MIN_RETRIES,
                .Maximum = CommandLineBuilder.MAX_RETRIES
            }

            CheckBoxPreErase = New CheckBox With {
                .Text = My.Resources.Label_PreErase,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(3, 3, 3, 3)
            }

            CheckBoxEraseEmpty = New CheckBox With {
                .Text = My.Resources.Label_EraseEmpty,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(3, 3, 3, 3)
            }

            CheckboxNoVerify = New CheckBox With {
                .Text = My.Resources.Label_NoVerify,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(3, 3, 3, 3)
            }

            CheckBoxContinue = New CheckBox With {
                .Text = My.Resources.Label_ContinueAfterFailure,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(3, 3, 3, 3)
            }

            CheckBoxSelect = New CheckBox With {
                .Text = My.Resources.Label_SelectTracks,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(3, 3, 3, 3)
            }

            Dim ButtonContainer As New FlowLayoutPanel With {
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
                .Margin = New Padding(6, 0, 6, 0),
                .Text = My.Resources.Label_Reset,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .TabIndex = 0
            }

            PanelButtonsLeft.Controls.Add(ButtonReset)
            ButtonReset.BringToFront()

            ButtonContainer.Controls.Add(ButtonProcess)

            LabelWarning = New Label With {
                .Text = My.Resources.Message_ImageFormatWarning,
                .ForeColor = Color.Red,
                .AutoSize = True,
                .Anchor = AnchorStyles.Right,
                .Visible = False
            }

            Dim ImageFormatCaption As New Label With {
                .Text = My.Resources.Label_Format,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            LabelImageFormat = New Label With {
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .UseMnemonic = False
            }


            ButtonOk.Visible = False
            ButtonCancel.Text = My.Resources.Label_Close


            Dim Row As Integer

            With TableLayoutPanelMain
                .SuspendLayout()

                Left = 0
                .RowCount = 4
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
                .Controls.Add(DriveLabel, 0, Row)
                .Controls.Add(ComboImageDrives, 1, Row)

                .Controls.Add(CheckBoxPreErase, 4, Row)

                .Controls.Add(CheckBoxEraseEmpty, 5, Row)

                .Controls.Add(RetriesLabel, 6, Row)
                .Controls.Add(NumericRetries, 7, Row)

                Row = 1
                .Controls.Add(ImageFormatCaption, 0, Row)
                .Controls.Add(LabelImageFormat, 1, Row)

                .Controls.Add(CheckboxNoVerify, 4, Row)

                .Controls.Add(CheckBoxContinue, 5, Row)

                Row = 2
                ' .RowStyles(Row).SizeType = SizeType.Absolute
                ' .RowStyles(Row).Height = 20
                .Controls.Add(LabelWarning, 0, Row)
                .SetColumnSpan(LabelWarning, 2)

                .Controls.Add(CheckBoxSelect, 4, Row)
                .SetColumnSpan(CheckBoxSelect, 2)

                Row = 3
                .Controls.Add(TableSide0, 0, Row)
                .SetColumnSpan(TableSide0, 2)

                .Controls.Add(TableSide1, 2, Row)
                .SetColumnSpan(TableSide1, 4)

                .Controls.Add(ButtonContainer, 6, Row)
                .SetColumnSpan(ButtonContainer, 2)

                .ResumeLayout()
                '.Left = (.Parent.ClientSize.Width - .Width) \ 2
            End With
        End Sub

        Private Sub KeepProcessing()
            Dim Response = _TrackStatus.GetNextTrackRange

            If Response.Continue Then
                _ContinueAfterWrite = True
            End If

            WriteDisk(_CurrentFilePath, Response.Ranges, Response.Heads)
        End Sub

        Private Sub ProcessDisk()
            Dim Response = SaveTempImage()

            If Response.Result Then
                _CurrentFilePath = Response.FilePath

                ResetState(False)

                WriteDisk(_CurrentFilePath)
            End If
        End Sub

        Private Sub ProcessOutputLine(line As String)
            If TextBoxConsole.Text.Length > 0 Then
                TextBoxConsole.AppendText(Environment.NewLine)
            End If
            TextBoxConsole.AppendText(line)

            _TrackStatus.ProcessOutputLineWrite(line, TrackStatus.ActionTypeEnum.Write, _DoubleStep)

            If _TrackStatus.Failed Then
                Process.Cancel()
            End If
        End Sub

        Private Sub RefreshButtonState()
            Dim IsRunning As Boolean = Process.IsRunning
            Dim IsIdle As Boolean = Not IsRunning

            ComboImageDrives.Enabled = IsIdle
            CheckBoxPreErase.Enabled = IsIdle
            CheckBoxEraseEmpty.Enabled = IsIdle
            CheckboxNoVerify.Enabled = IsIdle AndAlso VerifyAllowed()
            CheckBoxSelect.Enabled = IsIdle

            ButtonProcess.Text = If(IsRunning, My.Resources.Label_Abort, My.Resources.Label_Write)

            ButtonReset.Enabled = IsIdle

            ButtonSaveLog.Enabled = IsIdle AndAlso TextBoxConsole.Text.Length > 0

            RefreshProcessButtonState()
            RefreshVerifyButtonState()
        End Sub

        Private Sub RefreshProcessButtonState()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue
            ButtonProcess.Enabled = Opt.Id <> "" AndAlso Not CheckBoxSelect.Checked OrElse TableSide0.SelectedTracks.Count > 0 OrElse TableSide1.SelectedTracks.Count > 0
        End Sub

        Private Sub RefreshVerifyButtonState()
            Dim IsIdle As Boolean = Not Process.IsRunning
            Dim AllowRetries = RetriesAllowed()

            CheckBoxContinue.Enabled = IsIdle AndAlso AllowRetries
            NumericRetries.Enabled = IsIdle AndAlso AllowRetries
        End Sub

        Private Sub ResetCheckBoxSelect()
            CheckBoxSelect.Checked = False
        End Sub

        Private Sub ResetState(Optional ResetSelected As Boolean = True)
            ResetTrackGrid(ResetSelected)
            ClearStatusBar()
            _TrackStatus.Clear()
            TextBoxConsole.Clear()
        End Sub

        Private Sub ResetTrackGrid(Optional ResetSelected As Boolean = True)
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            Dim TrackCount As UShort
            If Opt.Type = FloppyMediaType.MediaUnknown Then
                If _DiskParams.MediaType = FloppyMediaType.Media525DoubleDensity Then
                    TrackCount = GreaseweazleSettings.MAX_TRACKS_525DD
                Else
                    TrackCount = GreaseweazleSettings.MAX_TRACKS
                End If
            Else
                TrackCount = Opt.Tracks
            End If

            TrackCount = Math.Max(TrackCount, _TrackCount)

            GridReset(TrackCount, _SideCount, ResetSelected)
        End Sub

        Private Function RetriesAllowed() As Boolean
            Return Not CheckboxNoVerify.Checked AndAlso Not _IsBitstreamImage AndAlso Not CheckBoxSelect.Checked
        End Function

        Private Function SaveTempImage() As (Result As Boolean, FilePath As String)
            Dim FileExt As String = If(_IsBitstreamImage, ".hfe", ".ima")

            Dim TempPath = InitTempImagePath()
            Dim FileName = Guid.NewGuid.ToString & FileExt

            If TempPath = "" Then
                MsgBox(My.Resources.Dialog_TempPathError, MsgBoxStyle.Exclamation)
                Return (False, "")
            End If

            Dim FilePath = GenerateUniqueFileName(TempPath, FileName)

            Dim Response = ImageIO.SaveDiskImageToFile(_Disk, FilePath, False)
            Dim Result = (Response = SaveImageResponse.Success)

            Return (Result, FilePath)
        End Function

        Private Function VerifyAllowed() As Boolean
            Return Not _IsBitstreamImage
        End Function
        Private Sub WriteDisk(FilePath As String)
            Dim TrackRanges As List(Of (StartTrack As UShort, EndTrack As UShort)) = Nothing
            Dim Heads As CommandLineBuilder.TrackHeads = CommandLineBuilder.TrackHeads.both

            If CheckBoxSelect.Checked Then
                Dim SelectedTracks As New HashSet(Of UShort)(TableSide0.SelectedTracks)
                SelectedTracks.UnionWith(TableSide1.SelectedTracks)

                TrackRanges = BuildRanges(SelectedTracks)

                If TableSide0.IsChecked AndAlso TableSide1.IsChecked Then
                    Heads = CommandLineBuilder.TrackHeads.both
                ElseIf TableSide0.IsChecked Then
                    Heads = CommandLineBuilder.TrackHeads.head0
                Else
                    Heads = CommandLineBuilder.TrackHeads.head1
                End If

                If TrackRanges.Count = 0 Then
                    Exit Sub
                End If
            End If

            WriteDisk(FilePath, TrackRanges, Heads)
        End Sub

        Private Sub WriteDisk(FilePath As String, TrackRanges As List(Of (StartTrack As UShort, EndTrack As UShort)), Heads As CommandLineBuilder.TrackHeads)
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            If Opt.Id = "" Then
                Exit Sub
            End If

            Dim Builder As New CommandLineBuilder(CommandLineBuilder.CommandAction.write) With {
                   .Device = Settings.ComPort,
                   .InFile = FilePath,
                   .Drive = Opt.Id,
                   .PreErase = CheckBoxPreErase.Checked,
                   .EraseEmpty = CheckBoxEraseEmpty.Checked
               }

            If VerifyAllowed() Then
                Builder.NoVerify = CheckboxNoVerify.Checked
            End If

            If RetriesAllowed() Then
                Builder.Retries = NumericRetries.Value
            End If

            _DoubleStep = (Opt.Type = FloppyMediaType.Media525HighDensity And _DiskParams.MediaType = FloppyMediaType.Media525DoubleDensity)

            If _DoubleStep Then
                Builder.HeadStep = 2
            End If

            Dim FileExt = IO.Path.GetExtension(FilePath).ToLower

            If FileExt = ".ima" Then
                Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(_DiskParams.Format)
                Builder.Format = GreaseweazleImageFormatCommandLine(ImageFormat)
            End If

            If TrackRanges Is Nothing Then
                TrackRanges = New List(Of (StartTrack As UShort, EndTrack As UShort)) From {
                    (0, GetEndTrackFromDriveOption(Opt))
                }
            End If

            For Each Range In TrackRanges
                Builder.AddCylinder(Range.StartTrack, Range.EndTrack)
            Next

            If Heads <> CommandLineBuilder.TrackHeads.both Then
                Builder.Heads = Heads
            End If

            Dim Arguments = Builder.Arguments

            'If TextBoxConsole.Text.Length > 0 Then
            '    TextBoxConsole.AppendText(Environment.NewLine)
            'End If
            'TextBoxConsole.AppendText(Arguments)

            Process.StartAsync(Settings.AppPath, Arguments)
        End Sub
#Region "Events"
        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If CancelProcessIfRunning() Then
                Exit Sub
            End If

            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            If Opt.Id = "" Then
                Exit Sub
            End If

            If Not CheckCompatibility() Then
                If Not ConfirmIncompatibleImage() Then
                    Exit Sub
                End If
            End If

            Dim DriveName = GetDriveName(Opt.Id)
            Dim Title = My.Resources.Label_WriteDisk & " - " & DriveName

            If Not ConfirmWrite(Title, DriveName) Then
                Exit Sub
            End If

            ProcessDisk()
        End Sub

        Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
            Reset(TextBoxConsole)
        End Sub

        Private Sub CheckboxNoVerify_CheckStateChanged(sender As Object, e As EventArgs) Handles CheckboxNoVerify.CheckStateChanged
            If Not _Initialized Then
                Exit Sub
            End If

            RefreshVerifyButtonState()
        End Sub

        Private Sub CheckBoxSelect_CheckStateChanged(sender As Object, e As EventArgs) Handles CheckBoxSelect.CheckStateChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If CheckBoxSelect.Checked Then
                GridReset()
            End If

            TableSide0.SelectEnabled = CheckBoxSelect.Checked
            TableSide1.SelectEnabled = CheckBoxSelect.Checked

            RefreshButtonState()
        End Sub

        Private Sub ComboImageDrives_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageDrives.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            ResetCheckBoxSelect()
            ResetTrackGrid()
            RefreshProcessButtonState()
            LabelWarning.Visible = Not CheckCompatibility()
        End Sub

        Private Sub Process_DataReceived(data As String) Handles Process.ErrorDataReceived, Process.OutputDataReceived
            ProcessOutputLine(data)
        End Sub

        Private Sub Process_ProcessStateChanged(State As ConsoleProcessRunner.ProcessStateEnum) Handles Process.ProcessStateChanged
            Select Case State
                Case ConsoleProcessRunner.ProcessStateEnum.Aborted
                    If Not _TrackStatus.Failed Then
                        _TrackStatus.UpdateTrackStatusAborted()
                    End If

                Case ConsoleProcessRunner.ProcessStateEnum.Completed
                    Dim DoKeepProcessing As Boolean = CheckKeepProcessing()
                    _TrackStatus.UpdateTrackStatusComplete(_DoubleStep, DoKeepProcessing)

                    If DoKeepProcessing Then
                        KeepProcessing()
                        Exit Sub
                    End If

                Case ConsoleProcessRunner.ProcessStateEnum.Error
                    _TrackStatus.UpdateTrackStatusError()
            End Select

            If State <> ConsoleProcessRunner.ProcessStateEnum.Running Then
                If _CurrentFilePath <> "" Then
                    DeleteFileIfExists(_CurrentFilePath)
                End If
                ResetCheckBoxSelect()
            End If

            RefreshButtonState()
        End Sub

        Private Sub WriteDiskForm_CheckChanged(sender As Object, Checked As Boolean, Side As Byte) Handles Me.CheckChanged
            RefreshProcessButtonState()
        End Sub

        Private Sub WriteDiskForm_SelectionChanged(sender As Object, Track As UShort, Side As Byte, Enabled As Boolean) Handles Me.SelectionChanged
            RefreshProcessButtonState()
        End Sub
#End Region
    End Class
End Namespace
