Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports Greaseweazle.Actions
Imports Greaseweazle.Tools

Namespace Flux.Greaseweazle
    Public Class CleanDiskForm
        Inherits BaseFluxForm

#Region "Form Controls"
        Private WithEvents ButtonProcess As Button
        Private WithEvents ButtonReset As Button
        Private WithEvents ComboImageDrives As ComboBox
        Private _NumericCyls As NumericUpDown
        Private _NumericLinger As NumericUpDown
        Private _NumericPasses As NumericUpDown
#End Region
        Private WithEvents CleanCmd As CleanCommand
        Private ReadOnly _Initialized As Boolean = False

        Public Sub New()
            MyBase.New(Settings.LogFileName, False)
            InitializeControls()

            CleanCmd = Engine.Clean

            Me.Text = My.Resources.Label_CleanDisk

            _NumericCyls.Value = DEFAULT_CYLS
            _NumericLinger.Value = DEFAULT_LINGER
            _NumericPasses.Value = DEFAULT_PASSES

            PopulateDrives(ComboImageDrives, FloppyDriveType.DriveUnknown)
            ClearLogAndStatus()
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

        Private Sub CleanDisk()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            If String.IsNullOrEmpty(Opt?.Id) Then
                Exit Sub
            End If

            ClearLogAndStatus()

            Dim Drive As DriveSpec = Nothing
            If Not TryMakeDriveSpec(Opt.Id, Drive) Then
                Return
            End If

            Dim Cyls = CInt(_NumericCyls.Value)
            Dim Passes = CInt(_NumericPasses.Value)

            Dim Opts As New CleanOptions With {
                .Cyls = Cyls,
                .Sequences = Clean.BuildPassSequences(Cyls, Passes),
                .LingerMs = CInt(_NumericLinger.Value),
                .Live = True,
                .Device = ConfiguredDevice(),
                .Drive = Drive
            }

            Runner.RunAsync(Sub(Token) CleanCmd.Run(Opts, Token))
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

            _NumericCyls = New NumericUpDown With {
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

            _NumericPasses = New NumericUpDown With {
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

            _NumericLinger = New NumericUpDown With {
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
                .Controls.Add(_NumericCyls, 4, Row)

                .Controls.Add(PassesLabel, 5, Row)
                .Controls.Add(_NumericPasses, 6, Row)

                .Controls.Add(LingerLabel, 7, Row)
                .Controls.Add(_NumericLinger, 8, Row)

                Row = 1
                .Controls.Add(ButtonContainer, 5, Row)
                .SetColumnSpan(ButtonContainer, 4)

                .ResumeLayout()
            End With
        End Sub

        Private Sub RefreshButtonState()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            ComboImageDrives.Enabled = IsIdle
            _NumericPasses.Enabled = IsIdle
            _NumericLinger.Enabled = IsIdle
            _NumericCyls.Enabled = IsIdle

            ButtonProcess.Text = If(IsRunning, My.Resources.Label_Abort, My.Resources.Label_Clean)
            ButtonProcess.Enabled = Not String.IsNullOrEmpty(Opt?.Id)

            ButtonSaveLog.Enabled = IsIdle AndAlso TextBoxConsole.Text.Length > 0

            ButtonReset.Enabled = IsIdle
        End Sub

        Private Sub RefreshCylinderCount()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            Dim MaxTrackCount As UShort = If(Opt IsNot Nothing AndAlso Opt.Type = FloppyDriveType.Drive525DoubleDensity, 40US, 80US)

            If _NumericCyls.Maximum <> MaxTrackCount Then
                _NumericCyls.Maximum = MaxTrackCount
                _NumericCyls.Value = MaxTrackCount
            End If
        End Sub

#Region "Events"
        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If CancelIfRunning() Then
                Exit Sub
            End If

            CleanDisk()
        End Sub

        Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
            GreaseweazleReset()
        End Sub

        Private Sub CleanCmd_CylinderSeeked(sender As Object, e As CleanCylinderEventArgs) Handles CleanCmd.CylinderSeeked
            Runner.EmitOutputText(FormatCleanCylinderSeekedFragment(e))
        End Sub

        Private Sub CleanCmd_PassStarted(sender As Object, e As CleanPassStartedEventArgs) Handles CleanCmd.PassStarted
            Runner.EmitOutputText(FormatCleanPassStartedFragment(e))
        End Sub

        Private Sub ComboImageDrives_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageDrives.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            RefreshButtonState()
            RefreshCylinderCount()
        End Sub

        Private Sub Runner_ProcessStateChanged(state As ConsoleProcessRunner.ProcessStateEnum) Handles Runner.ProcessStateChanged
            RefreshButtonState()
        End Sub
#End Region
    End Class
End Namespace
