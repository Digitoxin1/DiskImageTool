Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports Greaseweazle.Actions
Imports Greaseweazle.Core
Imports Greaseweazle.Tools

Namespace Flux.Greaseweazle
    Public Class WriteDiskForm
        Inherits BaseFluxForm

#Region "Form Controls"
        Private WithEvents ButtonProcess As Button
        Private WithEvents ButtonReset As Button
        Private WithEvents CheckboxNoVerify As CheckBox
        Private WithEvents CheckBoxSelect As CheckBox
        Private WithEvents ComboImageDrives As ComboBox
        Private _CheckBoxContinue As CheckBox
        Private _CheckBoxEraseEmpty As CheckBox
        Private _CheckBoxPreErase As CheckBox
        Private _LabelImageFormat As Label
        Private _LabelWarning As Label
        Private _NumericRetries As NumericUpDown
#End Region
        Private WithEvents WriteCmd As WriteCommand
        Private ReadOnly _Disk As DiskImage.Disk
        Private ReadOnly _DiskParams As FloppyDiskParams
        Private ReadOnly _Initialized As Boolean = False
        Private ReadOnly _IsBitstreamImage As Boolean
        Private ReadOnly _SideCount As Byte
        Private ReadOnly _Status As TrackStatus
        Private ReadOnly _TrackCount As UShort
        Private _ContinueAfterWrite As Boolean = False
        Private _CurrentFilePath As String = ""
        Private _DoubleStep As Boolean = False

        Public Sub New(Disk As DiskImage.Disk, FileName As String)
            MyBase.New(Settings.LogFileName)
            InitializeControls()

            WriteCmd = Engine.Write
            _Status = New TrackStatus()
            TrackStatus = _Status

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

            _NumericRetries.Value = DEFAULT_RETRIES

            _LabelImageFormat.Text = _DiskParams.Description

            Dim Mode As String = If(_IsBitstreamImage, My.Resources.FileType_BitstreamImage, My.Resources.FileType_SectorImage)
            StatusStripBottom.Items.Add(New ToolStripStatusLabel(Mode))

            Dim AvailableTypes = Settings.AvailableDriveTypes
            Dim SelectedFormat = GreaseweazleFindCompatibleDriveType(_DiskParams, AvailableTypes)

            PopulateDrives(ComboImageDrives, SelectedFormat)
            ResetState()
            ResetCheckBoxSelect()
            RefreshButtonState()
            _LabelWarning.Visible = Not CheckCompatibility()

            _Initialized = True
        End Sub

        Public Shared Sub Display(Disk As DiskImage.Disk, FileName As String)
            Using dlg As New WriteDiskForm(Disk, FileName)
                dlg.ShowDialog(App.CurrentFormInstance)
            End Using
        End Sub

        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
            WriteCmd = Nothing

            MyBase.OnFormClosed(e)
        End Sub

        Private Sub ApplyProcessState(state As ConsoleProcessRunner.ProcessStateEnum)
            Select Case state
                Case ConsoleProcessRunner.ProcessStateEnum.Aborted
                    TrackStatus.UpdateTrackStatusAborted()

                Case ConsoleProcessRunner.ProcessStateEnum.Completed
                    Dim DoKeepProcessing As Boolean = CheckKeepProcessing()
                    TrackStatus.UpdateTrackStatusComplete(_DoubleStep, DoKeepProcessing)

                    If DoKeepProcessing Then
                        KeepProcessing()
                        Exit Sub
                    End If

                Case ConsoleProcessRunner.ProcessStateEnum.Error
                    TrackStatus.UpdateTrackStatusError()
            End Select

            If state <> ConsoleProcessRunner.ProcessStateEnum.Running Then
                If _CurrentFilePath <> "" Then
                    DeleteTempFileIfExists(_CurrentFilePath)
                End If

                GridSetBusy(False)
                ResetCheckBoxSelect()
                _ContinueAfterWrite = False
            End If

            RefreshButtonState()
        End Sub

        Private Function CheckCompatibility() As Boolean
            Dim Opt As DriveOption = TryCast(ComboImageDrives.SelectedItem, DriveOption)

            If Opt Is Nothing OrElse Opt.Type = FloppyDriveType.DriveUnknown Then
                Return True
            End If

            Dim FloppyType = GreaseweazleFindCompatibleDriveType(_DiskParams, Opt.Type)

            Return FloppyType = Opt.Type
        End Function

        Private Function CheckKeepProcessing() As Boolean
            Dim KeepProcessing As Boolean = _ContinueAfterWrite OrElse (RetriesAllowed() AndAlso _CheckBoxContinue.Checked AndAlso CheckBoxSelect.Checked)

            If KeepProcessing Then
                KeepProcessing = TrackStatus.CanKeepProcessing
            End If

            Return KeepProcessing
        End Function

        Private Function ConfirmIncompatibleImage() As Boolean
            Dim Msg = String.Format(My.Resources.Dialog_ImageFormatWarning, Environment.NewLine)
            Return MsgBox(Msg, MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes
        End Function

        Private Function GetEndTrackFromDriveOption(Opt As DriveOption) As UShort
            Dim TrackCount As Integer = Opt.Tracks

            If _DiskParams.DriveType = FloppyDriveType.Drive525DoubleDensity Then
                If Opt.Type <> FloppyDriveType.Drive525DoubleDensity AndAlso Opt.Type <> FloppyDriveType.Drive525HighDensity Then
                    TrackCount = GreaseweazleSettings.MAX_TRACKS_525DD
                End If
            ElseIf Opt.Type = FloppyDriveType.Drive525DoubleDensity AndAlso _TrackCount > GreaseweazleSettings.MAX_TRACKS_525DD Then
                TrackCount = GreaseweazleSettings.MIN_TRACKS_525DD
            End If

            If _DoubleStep Then
                TrackCount = CeilDiv(CUInt(TrackCount), 2)
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

            _NumericRetries = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 45,
                .Minimum = MIN_RETRIES,
                .Maximum = MAX_RETRIES
            }

            _CheckBoxPreErase = New CheckBox With {
                .Text = My.Resources.Label_PreErase,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(3, 3, 3, 3)
            }

            _CheckBoxEraseEmpty = New CheckBox With {
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

            _CheckBoxContinue = New CheckBox With {
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

            _LabelWarning = New Label With {
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

            _LabelImageFormat = New Label With {
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .UseMnemonic = False
            }

            ButtonOk.Visible = False
            ButtonCancel.Text = WithoutHotkey(My.Resources.Menu_Close)

            Dim Row As Integer

            With TableLayoutPanelMain
                .SuspendLayout()

                .Left = 0
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

                .Controls.Add(_CheckBoxPreErase, 4, Row)

                .Controls.Add(_CheckBoxEraseEmpty, 5, Row)

                .Controls.Add(RetriesLabel, 6, Row)
                .Controls.Add(_NumericRetries, 7, Row)

                Row = 1
                .Controls.Add(ImageFormatCaption, 0, Row)
                .Controls.Add(_LabelImageFormat, 1, Row)

                .Controls.Add(CheckboxNoVerify, 4, Row)

                .Controls.Add(_CheckBoxContinue, 5, Row)

                Row = 2
                ' .RowStyles(Row).SizeType = SizeType.Absolute
                ' .RowStyles(Row).Height = 20
                .Controls.Add(_LabelWarning, 0, Row)
                .SetColumnSpan(_LabelWarning, 2)

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
            Dim Response = TrackStatus.GetNextTrackRange

            _ContinueAfterWrite = Response.Continue

            WriteDisk(_CurrentFilePath, Response.Ranges, Response.Heads)
        End Sub

        Private Sub ProcessDisk()
            Dim Response = SaveTempImage()

            If Response.Result Then
                _CurrentFilePath = Response.FilePath

                ResetState(ResetSelected:=False)

                WriteDisk(_CurrentFilePath)
            End If
        End Sub

        Private Sub RefreshButtonState()
            ComboImageDrives.Enabled = IsIdle
            _CheckBoxPreErase.Enabled = IsIdle
            _CheckBoxEraseEmpty.Enabled = IsIdle
            CheckboxNoVerify.Enabled = IsIdle AndAlso VerifyAllowed()
            CheckBoxSelect.Enabled = IsIdle

            ButtonProcess.Text = If(IsRunning, My.Resources.Label_Abort, My.Resources.Label_Write)

            ButtonReset.Enabled = IsIdle

            ButtonSaveLog.Enabled = IsIdle AndAlso TextBoxConsole.Text.Length > 0

            RefreshProcessButtonState()
            RefreshVerifyButtonState()
        End Sub

        Private Sub RefreshProcessButtonState()
            Dim Opt As DriveOption = TryCast(ComboImageDrives.SelectedItem, DriveOption)
            ButtonProcess.Enabled = Not String.IsNullOrEmpty(Opt?.Id) AndAlso (Not CheckBoxSelect.Checked OrElse TableSide0.SelectedTracks.Count > 0 OrElse TableSide1.SelectedTracks.Count > 0)
        End Sub

        Private Sub RefreshVerifyButtonState()
            Dim AllowRetries = RetriesAllowed()

            _CheckBoxContinue.Enabled = IsIdle AndAlso AllowRetries
            _NumericRetries.Enabled = IsIdle AndAlso AllowRetries
        End Sub

        Private Sub ResetCheckBoxSelect()
            CheckBoxSelect.Checked = False
        End Sub

        Private Sub ResetState(Optional ResetSelected As Boolean = True)
            ResetTrackGrid(ResetSelected)
            ClearLogAndStatus()
            TrackStatus.Clear()
        End Sub

        Private Sub ResetTrackGrid(Optional ResetSelected As Boolean = True)
            Dim Opt As DriveOption = TryCast(ComboImageDrives.SelectedItem, DriveOption)

            Dim TrackCount As UShort
            If Opt Is Nothing OrElse Opt.Type = FloppyDriveType.DriveUnknown Then
                If _DiskParams.DriveType = FloppyDriveType.Drive525DoubleDensity Then
                    TrackCount = GreaseweazleSettings.MAX_TRACKS_525DD
                Else
                    TrackCount = GreaseweazleSettings.MAX_TRACKS
                End If
            Else
                TrackCount = Opt.Tracks
            End If

            TrackCount = Math.Max(TrackCount, _TrackCount)

            GridReset(TrackCount, _SideCount, Nothing, ResetSelected)
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
            Dim Heads As TrackHeads = TrackHeads.both

            If CheckBoxSelect.Checked Then
                TrackRanges = GetSelectedTrackRanges()
                Heads = GetSelectedTrackHeads()

                If TrackRanges.Count = 0 Then
                    Exit Sub
                End If

                GridSetBusy(True)
            End If

            WriteDisk(FilePath, TrackRanges, Heads)
        End Sub

        Private Sub WriteDisk(FilePath As String, TrackRanges As List(Of (StartTrack As UShort, EndTrack As UShort)), Heads As TrackHeads)
            Dim Opt As DriveOption = TryCast(ComboImageDrives.SelectedItem, DriveOption)

            If String.IsNullOrEmpty(Opt?.Id) Then
                Exit Sub
            End If

            Dim Drive As DriveSpec = Nothing
            If Not TryMakeDriveSpec(Opt.Id, Drive) Then
                ApplyProcessState(ConsoleProcessRunner.ProcessStateEnum.Error)
                Return
            End If

            _DoubleStep = (Opt.Type = FloppyDriveType.Drive525HighDensity AndAlso _DiskParams.DriveType = FloppyDriveType.Drive525DoubleDensity)

            Dim Format As String = Nothing

            Dim FileExt = IO.Path.GetExtension(FilePath).ToLower

            If FileExt = ".ima" Then
                Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(_DiskParams.Format)
                Format = GreaseweazleImageFormatCommandLine(ImageFormat)
            End If

            If TrackRanges Is Nothing Then
                TrackRanges = New List(Of (StartTrack As UShort, EndTrack As UShort)) From {
                    (0, GetEndTrackFromDriveOption(Opt))
                }
            End If

            Dim TrackSet = BuildUserSpec(TrackRanges, Heads, _DoubleStep)

            Dim Opts As New WriteOptions With {
                .FileName = FilePath,
                .Format = Format,
                .TrackSet = TrackSet,
                .PreErase = _CheckBoxPreErase.Checked,
                .EraseEmpty = _CheckBoxEraseEmpty.Checked,
                .NoVerify = VerifyAllowed() AndAlso CheckboxNoVerify.Checked,
                .Retries = If(RetriesAllowed(), CInt(_NumericRetries.Value), DEFAULT_RETRIES),
                .Live = True,
                .Device = ConfiguredDevice(),
                .Drive = Drive
            }

            _Status.OnWriteStarted(TrackRanges, Heads)

            Runner.RunAsync(Sub(Token) WriteCmd.Run(Opts, Token))
        End Sub

#Region "Events"
        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If CancelIfRunning() Then
                Exit Sub
            End If

            Dim Opt As DriveOption = TryCast(ComboImageDrives.SelectedItem, DriveOption)

            If String.IsNullOrEmpty(Opt?.Id) Then
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
            GreaseweazleReset()
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

            CheckStateChanged(CheckBoxSelect.Checked)

            RefreshButtonState()
        End Sub

        Private Sub ComboImageDrives_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageDrives.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            ResetCheckBoxSelect()
            ResetTrackGrid()
            RefreshProcessButtonState()

            _LabelWarning.Visible = Not CheckCompatibility()
        End Sub

        Private Sub Runner_ProcessFailed(ex As Exception) Handles Runner.ProcessFailed
            Dim VerifyEx = TryCast(ex, WriteVerifyFailedException)

            If VerifyEx IsNot Nothing Then
                _Status.OnWriteVerifyFailed(VerifyEx.Cyl, VerifyEx.Head, _DoubleStep)
            End If
        End Sub

        Private Sub Runner_ProcessStateChanged(state As ConsoleProcessRunner.ProcessStateEnum) Handles Runner.ProcessStateChanged
            ApplyProcessState(state)
        End Sub

        Private Sub WriteCmd_HardSectorsDetected(sender As Object, e As HardSectorsDetectedEventArgs) Handles WriteCmd.HardSectorsDetected
            Runner.EmitOutputLine(FormatWriteHardSectorsLine(e))
        End Sub

        Private Sub WriteCmd_Started(sender As Object, e As WriteStartedEventArgs) Handles WriteCmd.Started
            For Each Line In FormatWriteStartedLines(e)
                Runner.EmitOutputLine(Line)
            Next
        End Sub

        Private Sub WriteCmd_TrackErasing(sender As Object, e As WriteTrackErasingEventArgs) Handles WriteCmd.TrackErasing
            Runner.EmitOutputLine(FormatWriteTrackErasingLine(e))

            Runner.PostToUi(Sub() _Status.OnWriteTrackErasing(e, _DoubleStep))
        End Sub

        Private Sub WriteCmd_TrackOutOfRange(sender As Object, e As WriteTrackOutOfRangeEventArgs) Handles WriteCmd.TrackOutOfRange
            Runner.EmitOutputLine(FormatWriteTrackOutOfRangeLine(e))

            Runner.PostToUi(Sub() _Status.OnWriteTrackOutOfRange(e, _DoubleStep))
        End Sub

        Private Sub WriteCmd_TrackWriting(sender As Object, e As WriteTrackWritingEventArgs) Handles WriteCmd.TrackWriting
            Runner.EmitOutputLine(FormatWriteTrackWritingLine(e))

            Runner.PostToUi(Sub() _Status.OnWriteTrackWriting(e, _DoubleStep))
        End Sub

        Private Sub WriteCmd_VerifyCompleted(sender As Object, e As WriteVerifyOutcomeEventArgs) Handles WriteCmd.VerifyCompleted
            Runner.EmitOutputLine(FormatWriteVerifyOutcomeLine(e))
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
