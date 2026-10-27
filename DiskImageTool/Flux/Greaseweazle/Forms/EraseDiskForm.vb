Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports Greaseweazle.Actions
Imports Greaseweazle.Tools

Namespace Flux.Greaseweazle
    Public Class EraseDiskForm
        Inherits BaseFluxForm

#Region "Form Controls"
        Private WithEvents ButtonProcess As Button
        Private WithEvents CheckBoxSelect As CheckBox
        Private WithEvents ComboImageDrives As ComboBox
        Private _CheckBoxHFreq As CheckBox
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
                Heads = TrackHeads.Both
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
            InitializeFooter()
            InitTLP()

            With TableLayoutPanelMain
                .SuspendLayout()

                .Left = 0
                .RowCount = 2
                .ColumnCount = 4
                .Dock = DockStyle.Fill

                FillAutoSizeStyles()

                '.ColumnStyles(0).SizeType = SizeType.Percent
                '.ColumnStyles(0).Width = 100

                Dim Row As Integer = 0

                InitializeControlsRowDrive(Row) : Row += 1
                InitGridRow(Row, .ColumnCount, Nothing, True) : Row += 1

                .ResumeLayout()
                '.Left = (.Parent.ClientSize.Width - .Width) \ 2
            End With
        End Sub

        Private Sub InitializeControlsRowDrive(Row As Integer)
            Dim GroupBoxDrive As New GroupBox With {
                .Text = My.Resources.Label_Drive,
                .AutoSize = True,
                .AutoSizeMode = AutoSizeMode.GrowAndShrink,
                .MinimumSize = New Size(0, 60)
            }

            ComboImageDrives = New ComboBox With {
                .Width = 180,
                .Location = New Point(9, 19)
            }

            GroupBoxDrive.Controls.Add(ComboImageDrives)

            Dim GroupBoxOptions As New GroupBox With {
                .Text = My.Resources.Label_Options,
                .AutoSize = True,
                .AutoSizeMode = AutoSizeMode.GrowAndShrink,
                .MinimumSize = New Size(0, 60)
            }

            Dim OptionsContainer As New FlowLayoutPanel With {
                .FlowDirection = FlowDirection.LeftToRight,
                .AutoSize = True,
                .AutoSizeMode = AutoSizeMode.GrowAndShrink,
                .Location = New Point(9, 19),
                .WrapContents = False
            }

            _LabelRevs = New Label With {
                .Text = My.Resources.Label_Revs,
                .AutoSize = True,
                .Anchor = AnchorStyles.Left,
                .Margin = New Padding(0)
            }
            OptionsContainer.Controls.Add(_LabelRevs)

            _NumericRevs = New NumericUpDown With {
                .Width = 45,
                .Anchor = AnchorStyles.Left,
                .Minimum = MIN_REVS,
                .Maximum = MAX_REVS,
                .Margin = New Padding(6, 0, 0, 0)
            }
            OptionsContainer.Controls.Add(_NumericRevs)

            _CheckBoxHFreq = New CheckBox With {
                .Text = My.Resources.Label_Hfreq,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 0, 0, 0)
            }
            OptionsContainer.Controls.Add(_CheckBoxHFreq)

            GroupBoxOptions.Controls.Add(OptionsContainer)

            CheckBoxSelect = New CheckBox With {
                .Text = My.Resources.Label_SelectTracks,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 0, 0, 0)
            }

            ButtonProcess = New Button With {
                .Margin = New Padding(12, 0, 0, 0),
                .Text = My.Resources.Label_Erase,
                .Anchor = AnchorStyles.Right,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True
            }

            With TableLayoutPanelMain
                .Controls.Add(GroupBoxDrive, 0, Row)
                .Controls.Add(GroupBoxOptions, 1, Row)
                .Controls.Add(CheckBoxSelect, 2, Row)
                .Controls.Add(ButtonProcess, 3, Row)
            End With
        End Sub

        Private Sub InitializeFooter()
            ButtonOk.Visible = False
            ButtonCancel.Text = WithoutHotkey(My.Resources.Menu_Close)
        End Sub
        Private Sub InitializeHelp()
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Drives, ComboImageDrives)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Revs, _LabelRevs, _NumericRevs)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Hfreq, _CheckBoxHFreq)
            SetHelpString(My.Resources.HelpStrings.Flux_SelectTracks, CheckBoxSelect)
            SetHelpString(My.Resources.HelpStrings.Flux_Erase, ButtonProcess)
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

            RefreshSaveLogButtonState()

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
