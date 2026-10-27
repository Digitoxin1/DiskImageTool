Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports Greaseweazle.Actions
Imports Greaseweazle.Tools

Namespace Flux.Greaseweazle
    Public Class CleanDiskForm
        Inherits BaseFluxForm

#Region "Form Controls"
        Private WithEvents ButtonProcess As Button
        Private WithEvents ComboImageDrives As ComboBox
        Private _LabelCyls As Label
        Private _LabelLinger As Label
        Private _LabelPasses As Label
        Private _NumericCyls As NumericUpDown
        Private _NumericLinger As NumericUpDown
        Private _NumericPasses As NumericUpDown
#End Region
        Private WithEvents CleanCmd As CleanCommand
        Private ReadOnly _Initialized As Boolean = False

        Public Sub New()
            MyBase.New(Settings.LogFileName, False)
            InitializeControls()
            InitializeHelp()

            CleanCmd = Engine.Clean

            Me.HelpButton = True
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

            DisableProcessButton()

            Runner.RunAsync(Sub(Token) CleanCmd.Run(Opts, Token))
        End Sub

        Private Sub DisableProcessButton()
            ButtonProcess.Enabled = False
            StartDebounceTimer()
        End Sub

        Private Sub InitializeControls()
            InitializeFooter()
            InitTLP()

            With TableLayoutPanelMain
                .SuspendLayout()
                .RowCount = 1
                .ColumnCount = 3
                .Dock = DockStyle.Fill
                .Padding = New Padding(0, 0, 0, 12)

                FillAutoSizeStyles()

                InitializeControlsRowDrive(0)

                .ResumeLayout()
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

            _LabelCyls = New Label With {
                .Text = My.Resources.Label_Cylinders,
                .AutoSize = True,
                .Anchor = AnchorStyles.Left,
                .Margin = New Padding(0)
            }
            OptionsContainer.Controls.Add(_LabelCyls)

            _NumericCyls = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 45,
                .Minimum = MIN_CYLS,
                .Maximum = MAX_CYLS,
                .Margin = New Padding(6, 0, 0, 0)
            }
            OptionsContainer.Controls.Add(_NumericCyls)

            _LabelPasses = New Label With {
                .Text = My.Resources.Label_Passes,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 0, 0, 0)
            }
            OptionsContainer.Controls.Add(_LabelPasses)

            _NumericPasses = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 45,
                .Minimum = MIN_PASSES,
                .Maximum = MAX_PASSES,
                .Margin = New Padding(6, 0, 0, 0)
            }
            OptionsContainer.Controls.Add(_NumericPasses)

            _LabelLinger = New Label With {
                .Text = My.Resources.Label_Linger,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 0, 0, 0)
            }
            OptionsContainer.Controls.Add(_LabelLinger)

            _NumericLinger = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 60,
                .Minimum = MIN_LINGER,
                .Maximum = MAX_LINGER,
                .Margin = New Padding(6, 0, 0, 0)
            }
            OptionsContainer.Controls.Add(_NumericLinger)

            GroupBoxOptions.Controls.Add(OptionsContainer)

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
                .Controls.Add(ButtonProcess, 2, Row)
            End With
        End Sub

        Private Sub InitializeFooter()
            ButtonOk.Visible = False
            ButtonCancel.Text = WithoutHotkey(My.Resources.Menu_Close)
        End Sub

        Private Sub InitializeHelp()
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Drives, ComboImageDrives)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_CleanCyls, _LabelCyls, _NumericCyls)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_CleanPasses, _LabelPasses, _NumericPasses)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_CleanLinger, _LabelLinger, _NumericLinger)
            SetHelpString(My.Resources.HelpStrings.Flux_Clean, ButtonProcess)
            SetHelpString(My.Resources.HelpStrings.Flux_SaveLog, ButtonSaveLog)
        End Sub

        Private Sub RefreshButtonState()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            ComboImageDrives.Enabled = IsIdle
            _NumericPasses.Enabled = IsIdle
            _NumericLinger.Enabled = IsIdle
            _NumericCyls.Enabled = IsIdle

            ButtonProcess.Text = If(IsRunning, My.Resources.Label_Abort, My.Resources.Label_Clean)
            ButtonProcess.Enabled = Not String.IsNullOrEmpty(Opt?.Id)

            RefreshSaveLogButtonState()

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
            If CheckDebounce Then
                Exit Sub
            End If

            If CancelIfRunning() Then
                Exit Sub
            End If

            CleanDisk()
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
