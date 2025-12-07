Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Public Class EraseDiskForm
        Inherits BaseFluxForm
        Private WithEvents ButtonProcess As Button
        Private WithEvents ButtonReset As Button
        Private WithEvents CheckBoxSelect As CheckBox
        Private WithEvents ComboImageDrives As ComboBox
        Private ReadOnly _Initialized As Boolean = False
        Private CheckBoxHFreq As CheckBox
        Private NumericRevs As NumericUpDown

        Public Sub New()
            MyBase.New(Settings.LogFileName)
            InitializeControls()

            TrackStatus = New TrackStatus()

            Me.Text = My.Resources.Label_EraseDisk

            NumericRevs.Value = CommandLineBuilder.DEFAULT_REVS

            PopulateDrives(ComboImageDrives, FloppyDriveType.DriveUnknown)
            ResetState()
            ResetCheckBoxSelect()
            RefreshButtonState()

            _Initialized = True
        End Sub

        Public Shared Sub Display(owner As IWin32Window)
            Using Form As New EraseDiskForm()
                Form.ShowDialog(owner)
            End Using
        End Sub

        Private Sub EraseDisk()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            If Opt.Id = "" Then
                Exit Sub
            End If

            ResetState(False)

            Dim TrackRanges As List(Of (StartTrack As UShort, EndTrack As UShort)) = Nothing
            Dim Heads As TrackHeads = TrackHeads.both

            If CheckBoxSelect.Checked Then
                Dim SelectedTracks As New HashSet(Of UShort)(TableSide0.SelectedTracks)
                SelectedTracks.UnionWith(TableSide1.SelectedTracks)

                TrackRanges = BuildRanges(SelectedTracks)

                If TableSide0.IsChecked AndAlso TableSide1.IsChecked Then
                    Heads = TrackHeads.both
                ElseIf TableSide0.IsChecked Then
                    Heads = TrackHeads.head0
                Else
                    Heads = TrackHeads.head1
                End If

                If TrackRanges.Count = 0 Then
                    Exit Sub
                End If
            End If

            Dim Builder As New CommandLineBuilder(CommandLineBuilder.CommandAction.erase) With {
                   .Device = Settings.ComPort,
                   .Drive = Opt.Id,
                   .Revs = NumericRevs.Value,
                   .Hfreq = CheckBoxHFreq.Checked
               }

            If TrackRanges Is Nothing Then
                TrackRanges = New List(Of (StartTrack As UShort, EndTrack As UShort)) From {
                    (0, Opt.Tracks - 1)
                }
            End If

            For Each Range In TrackRanges
                Builder.AddCylinder(Range.StartTrack, Range.EndTrack)
            Next

            If Heads <> TrackHeads.both Then
                Builder.Heads = Heads
            End If

            Dim Arguments = Builder.Arguments

            Process.StartAsync(Settings.AppPath, Arguments)
        End Sub

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

            Dim RevsLabel As New Label With {
                .Text = My.Resources.Label_Revs,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(32, 3, 3, 3)
            }

            NumericRevs = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 45,
                .Minimum = CommandLineBuilder.MIN_REVS,
                .Maximum = CommandLineBuilder.MAX_REVS
            }

            CheckBoxSelect = New CheckBox With {
                .Text = My.Resources.Label_SelectTracks,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            CheckBoxHFreq = New CheckBox With {
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
                .Controls.Add(DriveLabel, 0, Row)
                .Controls.Add(ComboImageDrives, 1, Row)

                .Controls.Add(RevsLabel, 2, Row)
                .Controls.Add(NumericRevs, 3, Row)

                .Controls.Add(CheckBoxHFreq, 4, Row)

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

        Private Sub ProcessOutputLine(line As String)
            If TextBoxConsole.Text.Length > 0 Then
                TextBoxConsole.AppendText(Environment.NewLine)
            End If
            TextBoxConsole.AppendText(line)

            TrackStatus.ProcessOutputLineWrite(line, ActionTypeEnum.Erase, False)

            If TrackStatus.Failed Then
                Process.Cancel()
            End If
        End Sub

        Private Sub RefreshButtonState()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue
            Dim IsRunning As Boolean = Process.IsRunning
            Dim IsIdle As Boolean = Not IsRunning

            ComboImageDrives.Enabled = IsIdle
            CheckBoxSelect.Enabled = IsIdle AndAlso Opt.Id <> ""
            CheckBoxHFreq.Enabled = IsIdle

            ButtonProcess.Text = If(IsRunning, My.Resources.Label_Abort, My.Resources.Label_Erase)
            ButtonProcess.Enabled = Opt.Id <> "" AndAlso Not CheckBoxSelect.Checked OrElse TableSide0.SelectedTracks.Count > 0 OrElse TableSide1.SelectedTracks.Count > 0

            ButtonSaveLog.Enabled = IsIdle AndAlso TextBoxConsole.Text.Length > 0

            ButtonReset.Enabled = IsIdle
            NumericRevs.Enabled = IsIdle
        End Sub

        Private Sub ResetCheckBoxSelect()
            CheckBoxSelect.Checked = False
        End Sub

        Private Sub ResetState(Optional ResetSelected As Boolean = True)
            ResetTrackGrid(ResetSelected)
            ClearStatusBar()
            TrackStatus.Clear()

            TextBoxConsole.Clear()
        End Sub

        Private Sub ResetTrackGrid(Optional ResetSelected As Boolean = True)
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            Dim TrackCount As UShort = If(Opt.Type = FloppyDriveType.DriveUnknown, GreaseweazleSettings.MAX_TRACKS, Opt.Tracks)

            GridReset(TrackCount, 2, ResetSelected)
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

            Dim DriveName = GetDriveName(Opt.Id)
            Dim Title = My.Resources.Label_EraseDisk & " - " & DriveName

            If Not ConfirmWrite(Title, DriveName) Then
                Exit Sub
            End If

            EraseDisk()
        End Sub

        Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
            Reset(TextBoxConsole)
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
            RefreshButtonState()
        End Sub

        Private Sub EraseDiskForm_CheckChanged(sender As Object, Checked As Boolean, Side As Byte) Handles Me.CheckChanged
            RefreshButtonState()
        End Sub

        Private Sub EraseDiskForm_SelectionChanged(sender As Object, Track As UShort, Side As Byte, Enabled As Boolean) Handles Me.SelectionChanged
            RefreshButtonState()
        End Sub

        Private Sub Process_DataReceived(data As String) Handles Process.ErrorDataReceived, Process.OutputDataReceived
            ProcessOutputLine(data)
        End Sub

        Private Sub Process_ProcessStateChanged(State As ConsoleProcessRunner.ProcessStateEnum) Handles Process.ProcessStateChanged
            Select Case State
                Case ConsoleProcessRunner.ProcessStateEnum.Aborted
                    If Not TrackStatus.Failed Then
                        TrackStatus.UpdateTrackStatusAborted()
                    End If

                Case ConsoleProcessRunner.ProcessStateEnum.Completed
                    TrackStatus.UpdateTrackStatusComplete(False)

                Case ConsoleProcessRunner.ProcessStateEnum.Error
                    TrackStatus.UpdateTrackStatusError()
            End Select

            If State <> ConsoleProcessRunner.ProcessStateEnum.Running Then
                ResetCheckBoxSelect()
            End If

            RefreshButtonState()
        End Sub
#End Region
    End Class
End Namespace
