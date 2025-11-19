Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Public Class CleanDiskForm
        Inherits BaseForm
        Private WithEvents ButtonProcess As Button
        Private WithEvents ButtonReset As Button
        Private WithEvents ComboImageDrives As ComboBox
        Private ReadOnly _Initialized As Boolean = False
        Private NumericCyls As NumericUpDown
        Private NumericLinger As NumericUpDown
        Private NumericPasses As NumericUpDown

        Public Sub New()
            MyBase.New(Settings.LogFileName, False)
            InitializeControls()

            Me.Text = My.Resources.Label_CleanDisk

            NumericCyls.Value = CommandLineBuilder.DEFAULT_CYLS
            NumericLinger.Value = CommandLineBuilder.DEFAULT_LINGER
            NumericPasses.Value = CommandLineBuilder.DEFAULT_PASSES

            PopulateDrives(ComboImageDrives, FloppyMediaType.MediaUnknown)
            ResetState()
            RefreshButtonState()
            RefreshCylinderCount()

            _Initialized = True
        End Sub

        Private Sub CleanDisk()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            If Opt.Id = "" Then
                Exit Sub
            End If

            ResetState()

            Dim Builder As New CommandLineBuilder(CommandLineBuilder.CommandAction.clean) With {
                   .Device = Settings.ComPort,
                   .Drive = Opt.Id,
                   .Cyls = NumericCyls.Value,
                   .Linger = NumericLinger.Value,
                   .Passes = NumericPasses.Value
               }

            Dim Arguments = Builder.Arguments

            Process.StartAsyncRaw(Settings.AppPath, Arguments)
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
                .Minimum = CommandLineBuilder.MIN_CYLS,
                .Maximum = CommandLineBuilder.MAX_CYLS
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
                .Minimum = CommandLineBuilder.MIN_PASSES,
                .Maximum = CommandLineBuilder.MAX_PASSES
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
                .Minimum = CommandLineBuilder.MIN_LINGER,
                .Maximum = CommandLineBuilder.MAX_LINGER
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
            ButtonCancel.Text = My.Resources.Label_Close

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
            Dim IsRunning As Boolean = Process.IsRunning
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

            Dim MaxTrackCount As UShort = IIf(Opt.Type = FloppyMediaType.Media525DoubleDensity, 40, 80)

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
            If CancelProcessIfRunning() Then
                Exit Sub
            End If

            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            If Opt.Id = "" Then
                Exit Sub
            End If

            CleanDisk()
        End Sub

        Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
            Reset(TextBoxConsole)
        End Sub

        Private Sub ComboImageDrives_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageDrives.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            RefreshButtonState()
            RefreshCylinderCount()
        End Sub

        Private Sub Process_DataReceived(data As String) Handles Process.ErrorDataReceived, Process.OutputDataReceived
            TextBoxConsole.AppendText(data)
        End Sub

        Private Sub Process_ProcessStateChanged(State As ConsoleProcessRunner.ProcessStateEnum) Handles Process.ProcessStateChanged
            RefreshButtonState()
        End Sub
#End Region
    End Class
End Namespace
