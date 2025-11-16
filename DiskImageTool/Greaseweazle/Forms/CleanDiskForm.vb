Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Greaseweazle
    Public Class CleanDiskForm
        Inherits BaseForm
        Private WithEvents ButtonProcess As Button
        Private WithEvents ButtonReset As Button
        Private WithEvents ComboImageDrives As ComboBox
        Private _Initialized As Boolean = False
        Private _ProcessRunning As Boolean = False
        Private NumericCyls As NumericUpDown
        Private NumericLinger As NumericUpDown
        Private NumericPasses As NumericUpDown

        Public Sub New()
            MyBase.New(False)
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

            Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.clean) With {
                   .Device = GreaseweazleSettings.COMPort,
                   .Drive = Opt.Id,
                   .Cyls = NumericCyls.Value,
                   .Linger = NumericLinger.Value,
                   .Passes = NumericPasses.Value
               }

            Dim Arguments = Builder.Arguments

            ToggleProcessRunning(True)
            Process.StartAsyncRaw(GreaseweazleSettings.AppPath, Arguments)
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

            Dim CylsLabel = New Label With {
                .Text = My.Resources.Label_Cylinders,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(6, 3, 3, 3)
            }

            NumericCyls = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 45,
                .Minimum = 1,
                .Maximum = 80
            }

            Dim PassesLabel = New Label With {
                .Text = My.Resources.Label_Passes,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(6, 3, 3, 3)
            }

            NumericPasses = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 45,
                .Minimum = 1,
                .Maximum = 9
            }

            Dim LingerLabel = New Label With {
                .Text = My.Resources.Label_Linger,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(6, 3, 3, 3)
            }

            NumericLinger = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 60,
                .Minimum = 1,
                .Maximum = 1000
            }

            Dim ButtonContainer = New FlowLayoutPanel With {
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
                .Margin = New Padding(3, 3, 0, 3),
                .Text = My.Resources.Label_Reset,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True
            }

            ButtonContainer.Controls.Add(ButtonReset)
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

            ComboImageDrives.Enabled = Not _ProcessRunning
            NumericPasses.Enabled = Not _ProcessRunning
            NumericLinger.Enabled = Not _ProcessRunning
            NumericCyls.Enabled = Not _ProcessRunning

            If _ProcessRunning Then
                ButtonProcess.Text = My.Resources.Label_Abort
            Else
                ButtonProcess.Text = My.Resources.Label_Clean
            End If

            ButtonProcess.Enabled = Opt.Id <> ""

            ButtonReset.Enabled = Not _ProcessRunning
        End Sub

        Private Sub RefreshCylinderCount()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            Dim MaxTrackCount As UShort

            Select Case Opt.Type
                Case FloppyMediaType.Media525DoubleDensity
                    MaxTrackCount = 40
                Case Else
                    MaxTrackCount = 80
            End Select

            If NumericCyls.Maximum <> MaxTrackCount Then
                NumericCyls.Maximum = MaxTrackCount
                NumericCyls.Value = MaxTrackCount
            End If
        End Sub
        Private Sub ResetState()
            ClearStatusBar()
            TextBoxConsole.Clear()
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

        Private Sub Process_ErrorDataReceived(data As String) Handles Process.ErrorDataReceived
            TextBoxConsole.AppendText(data)
        End Sub

        Private Sub Process_ProcessExited(exitCode As Integer) Handles Process.ProcessExited
            ToggleProcessRunning(False)
        End Sub

        Private Sub Process_ProcessFailed(message As String, ex As Exception) Handles Process.ProcessFailed
            ToggleProcessRunning(False)
        End Sub
#End Region
    End Class
End Namespace
