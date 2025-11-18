Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Public Class EraseDiskForm
        Inherits BaseForm
        Private WithEvents ButtonProcess As Button
        Private WithEvents ButtonReset As Button
        Private WithEvents CheckBoxSelect As CheckBox
        Private WithEvents ComboImageDrives As ComboBox
        Private ReadOnly _Initialized As Boolean = False
        Private ReadOnly _TrackStatus As TrackStatus
        Private _ProcessRunning As Boolean = False
        Private _TrackRange As ConsoleParser.TrackRange = Nothing
        Private CheckBoxHFreq As CheckBox
        Private NumericRevs As NumericUpDown

        Public Sub New()
            MyBase.New(Settings.LogFileName)
            InitializeControls()

            _TrackStatus = New TrackStatus(Me)

            Me.Text = My.Resources.Label_EraseDisk

            NumericRevs.Value = CommandLineBuilder.DEFAULT_REVS

            PopulateDrives(ComboImageDrives, FloppyMediaType.MediaUnknown)
            ResetState()
            ResetCheckBoxSelect()
            RefreshButtonState()

            _Initialized = True
        End Sub

        Private Sub EraseDisk()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            If Opt.Id = "" Then
                Exit Sub
            End If

            ResetState(False)

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

            If Heads <> CommandLineBuilder.TrackHeads.both Then
                Builder.Heads = Heads
            End If

            Dim Arguments = Builder.Arguments

            ToggleProcessRunning(True)
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
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Reset,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonContainer.Controls.Add(ButtonProcess)
            ButtonContainer.Controls.Add(ButtonReset)

            ButtonOk.Visible = False
            ButtonCancel.Text = My.Resources.Label_Close

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

            If _TrackRange Is Nothing Then
                _TrackRange = _TrackStatus.ParseDiskRange(line)
            End If

            Dim TrackInfo = _TrackStatus.ParseTrackWrite(line)
            If TrackInfo IsNot Nothing Then
                Dim Action As TrackStatus.ActionTypeEnum
                If TrackInfo.Action = "Erasing" Then
                    Action = TrackStatus.ActionTypeEnum.Erase
                ElseIf TrackInfo.Action = "Writing" Then
                    Action = TrackStatus.ActionTypeEnum.Write
                Else
                    Action = TrackStatus.ActionTypeEnum.Unknown
                End If

                Dim Statusinfo = _TrackStatus.UpdateStatusInfo(TrackInfo, TrackStatus.ActionTypeEnum.Erase)
                _TrackStatus.UpdateTrackStatus(Statusinfo, Action, False)
                Return
            End If
        End Sub

        Private Sub RefreshButtonState()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            ComboImageDrives.Enabled = Not _ProcessRunning
            CheckBoxSelect.Enabled = Not _ProcessRunning AndAlso Opt.Id <> ""
            CheckBoxHFreq.Enabled = Not _ProcessRunning

            If _ProcessRunning Then
                ButtonProcess.Text = My.Resources.Label_Abort
            Else
                ButtonProcess.Text = My.Resources.Label_Erase
            End If

            ButtonSaveLog.Enabled = Not _ProcessRunning AndAlso TextBoxConsole.Text.Length > 0

            ButtonProcess.Enabled = Opt.Id <> "" AndAlso Not CheckBoxSelect.Checked OrElse TableSide0.SelectedTracks.Count > 0 OrElse TableSide1.SelectedTracks.Count > 0

            ButtonReset.Enabled = Not _ProcessRunning
            NumericRevs.Enabled = Not _ProcessRunning
        End Sub

        Private Sub ResetCheckBoxSelect()
            CheckBoxSelect.Checked = False
        End Sub

        Private Sub ResetState(Optional ResetSelected As Boolean = True)
            ResetTrackGrid(ResetSelected)
            ClearStatusBar()
            _TrackStatus.Clear()
            _TrackRange = Nothing

            TextBoxConsole.Clear()
        End Sub

        Private Sub ResetTrackGrid(Optional ResetSelected As Boolean = True)
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            Dim TrackCount As UShort
            If Opt.Type = FloppyMediaType.MediaUnknown Then
                TrackCount = GreaseweazleSettings.MAX_TRACKS
            Else
                TrackCount = Opt.Tracks
            End If

            GridReset(TrackCount, 2, ResetSelected)
        End Sub

        Private Sub ToggleProcessRunning(Value As Boolean)
            _ProcessRunning = Value

            If Not Value Then
                CheckBoxSelect.Checked = False
            End If

            RefreshButtonState()
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

        Private Sub Process_ErrorDataReceived(data As String) Handles Process.ErrorDataReceived
            ProcessOutputLine(data)
        End Sub

        Private Sub Process_ProcessExited(exitCode As Integer) Handles Process.ProcessExited
            Dim Aborted = (exitCode = -1)

            _TrackStatus.UpdateTrackStatusComplete(Aborted, False)
            ToggleProcessRunning(False)
        End Sub

        Private Sub Process_ProcessFailed(message As String, ex As Exception) Handles Process.ProcessFailed
            _TrackStatus.UpdateTrackStatusError()
            ToggleProcessRunning(False)
        End Sub
#End Region
    End Class
End Namespace
