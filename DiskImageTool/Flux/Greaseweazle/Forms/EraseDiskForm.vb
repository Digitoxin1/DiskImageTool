Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports Greaseweazle.Actions
Imports Greaseweazle.Tools

Namespace Flux.Greaseweazle
    Public Class EraseDiskForm
        Inherits BaseFluxForm

#Region "Form Controls"
        Private WithEvents ButtonProcess As Button
        Private WithEvents ButtonReset As Button
        Private WithEvents CheckBoxSelect As CheckBox
        Private WithEvents ComboImageDrives As ComboBox
        Private _CheckBoxHFreq As CheckBox
        Private _LabelDrive As Label
        Private _LabelRevs As Label
        Private _NumericRevs As NumericUpDown
#End Region
        Private WithEvents EraseCmd As EraseCommand
        Private ReadOnly _Initialized As Boolean = False
        Private ReadOnly _Status As TrackStatus

        Public Sub New()
            MyBase.New(Settings.LogFileName)
            InitializeControls()
            InitializeHelp()

            EraseCmd = Engine.Erase
            _Status = New TrackStatus()
            TrackStatus = _Status

            Me.HelpButton = True
            Me.Text = My.Resources.Label_EraseDisk

            _NumericRevs.Value = DEFAULT_REVS

            PopulateDrives(ComboImageDrives, FloppyDriveType.DriveUnknown)
            ResetState()
            ResetCheckBoxSelect()
            RefreshButtonState()

            _Initialized = True
        End Sub

        Public Shared Sub Display()
            Using dlg As New EraseDiskForm()
                dlg.ShowDialog(App.CurrentFormInstance)
            End Using
        End Sub

        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
            EraseCmd = Nothing

            MyBase.OnFormClosed(e)
        End Sub

        Private Sub ApplyProcessState(state As ConsoleProcessRunner.ProcessStateEnum)
            Select Case state
                Case ConsoleProcessRunner.ProcessStateEnum.Aborted
                    TrackStatus.UpdateTrackStatusAborted()

                Case ConsoleProcessRunner.ProcessStateEnum.Completed
                    TrackStatus.UpdateTrackStatusComplete(False)

                Case ConsoleProcessRunner.ProcessStateEnum.Error
                    TrackStatus.UpdateTrackStatusError()
            End Select

            If state <> ConsoleProcessRunner.ProcessStateEnum.Running Then
                ResetCheckBoxSelect()
            End If

            RefreshButtonState()
        End Sub

        Private Sub EraseDisk()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            If String.IsNullOrEmpty(Opt?.Id) Then
                Exit Sub
            End If

            ResetState(ResetSelected:=False)

            Dim Drive As DriveSpec = Nothing
            If Not TryMakeDriveSpec(Opt.Id, Drive) Then
                ApplyProcessState(ConsoleProcessRunner.ProcessStateEnum.Error)
                Return
            End If

            Dim TrackRanges As List(Of (StartTrack As UShort, EndTrack As UShort))
            Dim Heads As TrackHeads

            If CheckBoxSelect.Checked Then
                TrackRanges = GetSelectedTrackRanges()
                Heads = GetSelectedTrackHeads()

                If TrackRanges.Count = 0 Then
                    Exit Sub
                End If
            Else
                Dim LastTrack As UShort = CUShort(Math.Max(0, CInt(Opt.Tracks) - 1))
                TrackRanges = New List(Of (StartTrack As UShort, EndTrack As UShort)) From {(0, LastTrack)}
                Heads = TrackHeads.both
            End If

            Dim TrackSet = BuildUserSpec(TrackRanges, Heads)

            Dim Opts As New EraseOptions With {
                .TrackSet = TrackSet,
                .Revs = CInt(_NumericRevs.Value),
                .Hfreq = _CheckBoxHFreq.Checked,
                .Live = True,
                .Device = ConfiguredDevice(),
                .Drive = Drive
            }

            Runner.RunAsync(Sub(Token) EraseCmd.Run(Opts, Token))
        End Sub

        Private Sub InitializeControls()
            _LabelDrive = New Label With {
                .Text = My.Resources.Label_Drive,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            ComboImageDrives = New ComboBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Width = 180
            }

            _LabelRevs = New Label With {
                .Text = My.Resources.Label_Revs,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(32, 3, 3, 3)
            }

            _NumericRevs = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 45,
                .Minimum = MIN_REVS,
                .Maximum = MAX_REVS
            }

            CheckBoxSelect = New CheckBox With {
                .Text = My.Resources.Label_SelectTracks,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            _CheckBoxHFreq = New CheckBox With {
                .Text = My.Resources.Label_Hfreq,
                .Anchor = AnchorStyles.Right,
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
                .Text = My.Resources.Label_Erase,
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

            ButtonOk.Visible = False
            ButtonCancel.Text = WithoutHotkey(My.Resources.Menu_Close)

            Dim Row As Integer

            With TableLayoutPanelMain
                .SuspendLayout()

                .Left = 0
                .RowCount = 2
                .ColumnCount = 6
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
                .Controls.Add(_LabelDrive, 0, Row)
                .Controls.Add(ComboImageDrives, 1, Row)

                .Controls.Add(_LabelRevs, 2, Row)
                .Controls.Add(_NumericRevs, 3, Row)

                .Controls.Add(_CheckBoxHFreq, 4, Row)

                .Controls.Add(CheckBoxSelect, 5, Row)

                Row = 1
                .Controls.Add(TableSide0, 0, Row)
                .SetColumnSpan(TableSide0, 2)

                .Controls.Add(TableSide1, 2, Row)
                .SetColumnSpan(TableSide1, 3)

                .Controls.Add(ButtonContainer, 5, Row)

                .ResumeLayout()
                '.Left = (.Parent.ClientSize.Width - .Width) \ 2
            End With
        End Sub

        Private Sub InitializeHelp()
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Drives, _LabelDrive, ComboImageDrives)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Revs, _LabelRevs, _NumericRevs)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Hfreq, _CheckBoxHFreq)
            SetHelpString(My.Resources.HelpStrings.Flux_SelectTracks, CheckBoxSelect)
            SetHelpString(My.Resources.HelpStrings.Flux_Erase, ButtonProcess)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_DeviceReset, ButtonReset)
            SetHelpString(My.Resources.HelpStrings.Flux_SaveLog, ButtonSaveLog)
        End Sub

        Private Sub RefreshButtonState()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue
            Dim HasOptId = Not String.IsNullOrEmpty(Opt?.Id)

            ComboImageDrives.Enabled = IsIdle
            CheckBoxSelect.Enabled = IsIdle AndAlso HasOptId
            _CheckBoxHFreq.Enabled = IsIdle

            ButtonProcess.Text = If(IsRunning, My.Resources.Label_Abort, My.Resources.Label_Erase)
            ButtonProcess.Enabled = HasOptId AndAlso (Not CheckBoxSelect.Checked OrElse TableSide0.SelectedTracks.Count > 0 OrElse TableSide1.SelectedTracks.Count > 0)

            ButtonSaveLog.Enabled = IsIdle AndAlso TextBoxConsole.Text.Length > 0

            ButtonReset.Enabled = IsIdle
            _NumericRevs.Enabled = IsIdle
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
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            Dim TrackCount As UShort = If(Opt Is Nothing OrElse Opt.Type = FloppyDriveType.DriveUnknown, GreaseweazleSettings.MAX_TRACKS, Opt.Tracks)

            GridReset(TrackCount, 2, Nothing, ResetSelected)
        End Sub

#Region "Events"
        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If CancelIfRunning() Then
                Exit Sub
            End If

            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            If String.IsNullOrEmpty(Opt?.Id) Then
                Exit Sub
            End If

            Dim DriveName = GetDriveName(Opt.Id)
            Dim Title = My.Resources.Label_EraseDisk & " - " & DriveName

            If Not ConfirmWrite(Title, DriveName) Then
                Exit Sub
            End If

            EraseDisk()
        End Sub

        Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
            GreaseweazleReset()
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
            RefreshButtonState()
        End Sub

        Private Sub EraseCmd_Started(sender As Object, e As EraseStartedEventArgs) Handles EraseCmd.Started
            Runner.EmitOutputLine(FormatEraseStartedLine(e))
        End Sub

        Private Sub EraseCmd_TrackStarted(sender As Object, e As EraseTrackEventArgs) Handles EraseCmd.TrackStarted
            Runner.EmitOutputLine(FormatEraseTrackStartedLine(e))

            Runner.PostToUi(Sub() _Status.OnEraseTrack(e))
        End Sub

        Private Sub EraseDiskForm_CheckChanged(sender As Object, Checked As Boolean, Side As Byte) Handles Me.CheckChanged
            RefreshButtonState()
        End Sub

        Private Sub EraseDiskForm_SelectionChanged(sender As Object, Track As UShort, Side As Byte, Enabled As Boolean) Handles Me.SelectionChanged
            RefreshButtonState()
        End Sub

        Private Sub Runner_ProcessStateChanged(state As ConsoleProcessRunner.ProcessStateEnum) Handles Runner.ProcessStateChanged
            ApplyProcessState(state)
        End Sub
#End Region
    End Class
End Namespace
