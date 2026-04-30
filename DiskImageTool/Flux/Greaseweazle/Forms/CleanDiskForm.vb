Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports Greaseweazle.Actions
Imports Greaseweazle.Tools

Namespace Flux.Greaseweazle
    Public Class CleanDiskForm
        Inherits BaseFluxForm

        Private WithEvents ButtonProcess As Button
        Private WithEvents ButtonReset As Button
        Private WithEvents CleanCmd As CleanCommand
        Private WithEvents ComboImageDrives As ComboBox
        Private WithEvents Runner As GreaseweazleRunner
        Private ReadOnly _Engine As New GreaseweazleEngine()
        Private ReadOnly _Initialized As Boolean = False
        Private NumericCyls As NumericUpDown
        Private NumericLinger As NumericUpDown
        Private NumericPasses As NumericUpDown

        Public Sub New()
            MyBase.New(Settings.LogFileName, False)
            InitializeControls()

            Runner = New GreaseweazleRunner()
            CleanCmd = _Engine.Clean

            Me.Text = My.Resources.Label_CleanDisk

            NumericCyls.Value = DEFAULT_CYLS
            NumericLinger.Value = DEFAULT_LINGER
            NumericPasses.Value = DEFAULT_PASSES

            PopulateDrives(ComboImageDrives, FloppyDriveType.DriveUnknown)
            ResetState()
            RefreshButtonState()
            RefreshCylinderCount()

            _Initialized = True
        End Sub

        Public Shared Sub Display()
            Using dlg As New CleanDiskForm()
                dlg.ShowDialog(App.CurrentFormInstance)
            End Using
        End Sub

        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
            CleanCmd = Nothing

            MyBase.OnFormClosed(e)
        End Sub

        Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
            If Runner.IsRunning Then
                If (e.CloseReason = CloseReason.UserClosing OrElse e.CloseReason = CloseReason.None) AndAlso (DialogResult = DialogResult.Cancel OrElse DialogResult = DialogResult.None) Then
                    If Not ConfirmCancelRun() Then
                        e.Cancel = True
                        Return
                    End If
                End If
                Runner.Cancel()
            End If

            MyBase.OnFormClosing(e)
        End Sub

        Private Sub CleanDisk()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            If Opt Is Nothing OrElse Opt.Id = "" Then
                Exit Sub
            End If

            ResetState()

            Dim Drive As DriveSpec
            Try
                Drive = MakeDriveSpec(Opt.Id)
            Catch ex As Exception
                HandleRunFailure(ex.Message)
                Return
            End Try

            Dim Cyls = CInt(NumericCyls.Value)
            Dim Passes = CInt(NumericPasses.Value)

            Dim Opts As New CleanOptions With {
                .Cyls = Cyls,
                .Sequences = Clean.BuildPassSequences(Cyls, Passes),
                .LingerMs = CInt(NumericLinger.Value),
                .Live = True,
                .Device = ConfiguredDevice(),
                .Drive = Drive
            }

            Runner.RunAsync(Sub(Token) CleanCmd.Run(Opts, Token))
        End Sub

        Private Function ConfirmCancelRun() As Boolean
            Return MsgBox(My.Resources.Dialog_ConfirmCancel, MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2 + MsgBoxStyle.Exclamation) = MsgBoxResult.Yes
        End Function

        Private Sub HandleRunFailure(message As String)
            Dim Msg = If(String.IsNullOrEmpty(message), "Unknown error", message)

            AppendLogLine("Command Failed: " & Msg)
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

            Dim CylsLabel As New Label With {
                .Text = My.Resources.Label_Cylinders,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(6, 3, 3, 3)
            }

            NumericCyls = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 45,
                .Minimum = MIN_CYLS,
                .Maximum = MAX_CYLS
            }

            Dim PassesLabel As New Label With {
                .Text = My.Resources.Label_Passes,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(6, 3, 3, 3)
            }

            NumericPasses = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 45,
                .Minimum = MIN_PASSES,
                .Maximum = MAX_PASSES
            }

            Dim LingerLabel As New Label With {
                .Text = My.Resources.Label_Linger,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(6, 3, 3, 3)
            }

            NumericLinger = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 60,
                .Minimum = MIN_LINGER,
                .Maximum = MAX_LINGER
            }

            Dim ButtonContainer As New FlowLayoutPanel With {
                .FlowDirection = FlowDirection.RightToLeft,
                .AutoSize = True,
                .Anchor = AnchorStyles.Right,
                .Margin = New Padding(3, 3, 0, 3)
            }

            ButtonProcess = New Button With {
                .Width = 75,
                .Margin = New Padding(3, 3, 3, 3),
                .Text = My.Resources.Label_Erase
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
                .RowCount = 2
                .ColumnCount = 9
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

                .ColumnStyles(2).SizeType = SizeType.Percent
                .ColumnStyles(2).Width = 100

                Row = 0
                .Controls.Add(DriveLabel, 0, Row)
                .Controls.Add(ComboImageDrives, 1, Row)

                .Controls.Add(CylsLabel, 3, Row)
                .Controls.Add(NumericCyls, 4, Row)

                .Controls.Add(PassesLabel, 5, Row)
                .Controls.Add(NumericPasses, 6, Row)

                .Controls.Add(LingerLabel, 7, Row)
                .Controls.Add(NumericLinger, 8, Row)

                Row = 1
                .Controls.Add(ButtonContainer, 5, Row)
                .SetColumnSpan(ButtonContainer, 4)

                .ResumeLayout()
            End With
        End Sub

        Private Sub RefreshButtonState()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue
            Dim IsRunning As Boolean = Runner.IsRunning
            Dim IsIdle As Boolean = Not IsRunning

            ComboImageDrives.Enabled = IsIdle
            NumericPasses.Enabled = IsIdle
            NumericLinger.Enabled = IsIdle
            NumericCyls.Enabled = IsIdle

            ButtonProcess.Text = If(IsRunning, My.Resources.Label_Abort, My.Resources.Label_Clean)
            ButtonProcess.Enabled = Opt.Id <> ""

            ButtonSaveLog.Enabled = IsIdle AndAlso TextBoxConsole.Text.Length > 0

            ButtonReset.Enabled = IsIdle
        End Sub

        Private Sub RefreshCylinderCount()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            Dim MaxTrackCount As UShort = IIf(Opt.Type = FloppyDriveType.Drive525DoubleDensity, 40, 80)

            If NumericCyls.Maximum <> MaxTrackCount Then
                NumericCyls.Maximum = MaxTrackCount
                NumericCyls.Value = MaxTrackCount
            End If
        End Sub

        Private Sub ResetState()
            ClearStatusBar()
            TextBoxConsole.Clear()
        End Sub

#Region "Events"
        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If Runner.IsRunning Then
                If ConfirmCancelRun() Then
                    Runner.Cancel()
                End If
                Exit Sub
            End If

            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            If Opt.Id = "" Then
                Exit Sub
            End If

            CleanDisk()
        End Sub

        Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
            TextBoxConsole.Clear()

            Try
                _Engine.Reset.Run(New ResetOptions With {
                    .Live = True,
                    .Device = ConfiguredDevice()
                })
            Catch ex As Exception
                AppendLogLine("Command Failed: " & ex.Message)
            End Try

            MsgBox(My.Resources.Dialog_GreaseweazleReset, MsgBoxStyle.Information)
        End Sub

        Private Sub ComboImageDrives_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageDrives.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            RefreshButtonState()
            RefreshCylinderCount()
        End Sub

        Private Sub OnCleanCylinderSeeked(sender As Object, e As CleanCylinderEventArgs) Handles CleanCmd.CylinderSeeked
            Runner.EmitOutputText(FormatCleanCylinderSeekedFragment(e))
        End Sub

        Private Sub OnCleanPassStarted(sender As Object, e As CleanPassStartedEventArgs) Handles CleanCmd.PassStarted
            Runner.EmitOutputText(FormatCleanPassStartedFragment(e))
        End Sub
        Private Sub Runner_OutputDataReceived(line As String) Handles Runner.OutputDataReceived
            AppendLogLine(line)
        End Sub

        Private Sub Runner_OutputTextReceived(text As String) Handles Runner.OutputTextReceived
            TextBoxConsole.AppendText(text)
        End Sub

        Private Sub Runner_ProcessFailed(ex As Exception) Handles Runner.ProcessFailed
            AppendLogLine("Command Failed: " & ex.Message)
        End Sub

        Private Sub Runner_ProcessStateChanged(state As ConsoleProcessRunner.ProcessStateEnum) Handles Runner.ProcessStateChanged
            RefreshButtonState()
        End Sub
#End Region
    End Class
End Namespace
