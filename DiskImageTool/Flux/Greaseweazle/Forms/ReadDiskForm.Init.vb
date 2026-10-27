Namespace Flux.Greaseweazle
    Partial Public Class ReadDiskForm
        Private _DotPattern As Bitmap
        Private _DotPatternColor As Color = Color.Empty

        Private Sub InitializeControls()
            Dim RowGrid As FlowLayoutPanel

            InitializeFooter()

            InitTLP()

            With TableLayoutPanelMain
                .SuspendLayout()
                .Left = 0
                .RowCount = 2
                .ColumnCount = 3
                .Dock = DockStyle.Fill

                FillAutoSizeStyles()

                With .ColumnStyles(TableLayoutPanelMain.ColumnCount - 1)
                    .SizeType = SizeType.Percent
                    .Width = 100.0F
                End With

                _PanelDriveParams = New ReadDiskDriveParams
                _PanelOutput = New ReadDiskOutput
                _PanelDestination = New ReadDiskDestination With {
                    .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
                }

                .Controls.Add(_PanelDriveParams, 0, 0)
                .Controls.Add(_PanelOutput, 1, 0)
                .Controls.Add(_PanelDestination, 2, 0)

                RowGrid = InitializeControlsRowGrid(1)

                .ResumeLayout()
            End With

            InitializePanels()
            InitializeEvents()

            AddHandler TableLayoutPanelMain.CellPaint, AddressOf PaintGridBackground
        End Sub

        Private Sub InitializePanels()
            With _PanelDriveParams
                .LabelDrive.Text = My.Resources.Label_Drive
                .ButtonDetect.Text = My.Resources.Label_Detect
                .LabelImageFormat.Text = My.Resources.Label_DiskFormat

                .LabelRevs.Text = My.Resources.Label_Revs
                .NumericRevs.Minimum = MIN_REVS
                .NumericRevs.Maximum = MAX_REVS

                .LabelRetries.Text = My.Resources.Label_Retries
                .NumericRetries.Minimum = MIN_RETRIES
                .NumericRetries.Maximum = MAX_RETRIES

                .LabelSeekRetries.Text = My.Resources.Label_SeekRetries
                .NumericSeekRetries.Minimum = MIN_RETRIES
                .NumericSeekRetries.Maximum = MAX_RETRIES
            End With

            With _PanelOutput
                .LabelOutputType.Text = My.Resources.Label_PrimaryOutput
                .LabelOutputType2.Text = My.Resources.Label_SecondaryOutput
                .LabelLocation.Text = My.Resources.Label_ImageLocation
            End With

            With _PanelDestination
                _ToolTip.SetToolTip(.ButtonToggleSequence, My.Resources.Label_ToggleSequence & " (Alt+S)")

                .LabelRootFolder.Text = My.Resources.Label_RootFolder
                .LabelFolderName.Text = My.Resources.Label_FolderName

                .TextBoxRootFolder.MaxLength = 255
                .TextBoxRootFolder.ReadOnly = True
                .TextBoxRootFolder.BackColor = SystemColors.Window

                .LabelPrefix.Text = My.Resources.Label_Prefix

                .TextBoxPrefix.PlaceholderText = DEFAULT_RAW_FILE_NAME
                .TextBoxPrefix.ShowCueWhenFocused = True
                .TextBoxPrefix.MaxLength = 255

                .LabelSecondaryImageFolder.Text = My.Resources.Label_ImageFolder

                .TextBoxImageFolder.MaxLength = 255
                .TextBoxImageFolder.ReadOnly = True
                .TextBoxImageFolder.BackColor = SystemColors.Window

                .LabelFileName.Text = My.Resources.Label_FileName
                .TextBoxFileName.MaxLength = 255

                .ToggleView(True)
            End With
        End Sub

        Private Sub InitializeEvents()
            With _PanelDriveParams
                AddHandler .ComboDrives.SelectedIndexChanged, AddressOf ComboImageDrives_SelectedIndexChanged
                AddHandler .ButtonDetect.Click, AddressOf ButtonDetect_Click
                AddHandler .ComboImageFormat.SelectedIndexChanged, AddressOf ComboImageFormat_SelectedIndexChanged
                AddHandler .NumericRevs.ValueChanged, AddressOf NumericRevs_ValueChanged
                AddHandler .NumericRetries.ValueChanged, AddressOf NumericRetries_ValueChanged
                AddHandler .NumericSeekRetries.ValueChanged, AddressOf NumericRetries_ValueChanged
            End With

            With _PanelOutput
                AddHandler .ComboBoxPrimaryOutput.SelectedIndexChanged, AddressOf ComboOutputType_SelectedIndexChanged
                AddHandler .ComboBoxPrimaryExtension.SelectedIndexChanged, AddressOf ComboExtensions_SelectedIndexChanged
                AddHandler .ComboBoxSecondaryOutput.SelectedIndexChanged, AddressOf ComboOutputType2_SelectedIndexChanged
                AddHandler .ComboBoxSecondaryExtension.SelectedIndexChanged, AddressOf ComboExtensions_SelectedIndexChanged
                AddHandler .ComboBoxSecondaryLocation.SelectedIndexChanged, AddressOf ComboImageLocation_SelectedIndexChanged
            End With

            With _PanelDestination
                AddHandler .ButtonToggleSequence.Click, AddressOf ButtonToggleSequence_Click

                AddHandler .TextBoxRootFolder.Click, AddressOf TextBoxRootFolder_Click
                AddHandler .ButtonBrowse.Click, AddressOf ButtonRootBrowse_Click

                AddHandler .TextBoxFolderName.Enter, AddressOf TextBox_Enter
                AddHandler .TextBoxFolderName.KeyUp, AddressOf TextBox_KeyUp
                AddHandler .TextBoxFolderName.Leave, AddressOf TextBox_Leave
                AddHandler .TextBoxFolderName.MouseUp, AddressOf TextBox_MouseUp
                AddHandler .TextBoxFolderName.TextChanged, AddressOf TextBoxFileName_TextChanged
                AddHandler .TextBoxFolderName.Validating, AddressOf TextBoxFolderName_Validating

                AddHandler .TextBoxPrefix.Enter, AddressOf TextBox_Enter
                AddHandler .TextBoxPrefix.KeyUp, AddressOf TextBox_KeyUp
                AddHandler .TextBoxPrefix.Leave, AddressOf TextBox_Leave
                AddHandler .TextBoxPrefix.MouseUp, AddressOf TextBox_MouseUp
                AddHandler .TextBoxPrefix.TextChanged, AddressOf TextBoxFileName_TextChanged
                AddHandler .TextBoxPrefix.Validating, AddressOf TextBoxFileName_Validating

                AddHandler .TextBoxImageFolder.Click, AddressOf TextBoxImageFolder_Click
                AddHandler .ButtonImageFolderBrowse.Click, AddressOf ButtonImageFolderBrowse_Click

                AddHandler .TextBoxFileName.Enter, AddressOf TextBox_Enter
                AddHandler .TextBoxFileName.KeyUp, AddressOf TextBox_KeyUp
                AddHandler .TextBoxFileName.Leave, AddressOf TextBox_Leave
                AddHandler .TextBoxFileName.MouseUp, AddressOf TextBox_MouseUp
                AddHandler .TextBoxFileName.TextChanged, AddressOf TextBoxFileName_TextChanged
                AddHandler .TextBoxFileName.Validating, AddressOf TextBoxFileName_Validating
            End With
        End Sub

        Private Function InitializeControlsRowGrid(Row As Integer) As FlowLayoutPanel
            Dim TableContainer = InitializeTables()

            With TableLayoutPanelMain
                .Controls.AddWithSpan(TableContainer, 0, Row, 3)
            End With

            Return TableContainer
        End Function

        Private Sub PaintGridBackground(sender As Object, e As TableLayoutCellPaintEventArgs)
            If e.Row <> 1 Then
                Return
            End If

            Dim TLP = DirectCast(sender, TableLayoutPanel)

            ' Create or update the pattern only when necessary
            If _DotPattern Is Nothing OrElse _DotPatternColor <> TLP.ForeColor Then
                If _DotPattern IsNot Nothing Then _DotPattern.Dispose()

                _DotPattern = New Bitmap(14, 14)
                _DotPatternColor = TLP.ForeColor

                Dim DotColor = Color.FromArgb(18, TLP.ForeColor)

                Using G As Graphics = Graphics.FromImage(_DotPattern)
                    Using B As New SolidBrush(DotColor)
                        G.FillRectangle(B, 8, 8, 2, 2)
                    End Using
                End Using
            End If

            ' Repeat the pattern across the entire cell
            Using B As New TextureBrush(_DotPattern)
                e.Graphics.FillRectangle(B, e.CellBounds)
            End Using
        End Sub

        Private Function InitializeTables() As FlowLayoutPanel
            Dim ButtonContainer = InitializeButtons()

            Dim TableContainer As New FlowLayoutPanel With {
                .FlowDirection = FlowDirection.LeftToRight,
                .Anchor = AnchorStyles.Top,
                .AutoSize = True,
                .AutoSizeMode = AutoSizeMode.GrowAndShrink,
                .WrapContents = False
            }

            TableContainer.Controls.Add(TableSide0)
            TableContainer.Controls.Add(TableSide1)
            TableContainer.Controls.Add(ButtonContainer)

            TableSide0.Anchor = AnchorStyles.Left Or AnchorStyles.Top
            TableSide0.Dock = DockStyle.None
            TableSide0.Margin = New Padding(0, 0, 20, 0)

            TableSide1.Anchor = AnchorStyles.Left Or AnchorStyles.Top
            TableSide1.Dock = DockStyle.None
            TableSide1.Margin = New Padding(20, 0, 20, 0)

            Return TableContainer
        End Function

        Private Function InitializeButtons() As FlowLayoutPanel
            Dim ButtonContainer As New FlowLayoutPanel With {
                .FlowDirection = FlowDirection.TopDown,
                .AutoSize = True,
                .Margin = New Padding(20, 24, 3, 3),
                .Anchor = AnchorStyles.Top Or AnchorStyles.Right
            }

            ButtonRefine = New Button With {
                .Margin = New Padding(3, 0, 3, 3),
                .Text = My.Resources.Label_Refine,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }
            ButtonContainer.Controls.Add(ButtonRefine)

            ButtonPreview = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Preview,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }
            ButtonContainer.Controls.Add(ButtonPreview)

            ButtonRead = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Read,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }
            ButtonContainer.Controls.Add(ButtonRead)

            ButtonConvert = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Convert,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Visible = False
            }
            ButtonContainer.Controls.Add(ButtonConvert)

            ButtonDiscard = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Discard,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }
            ButtonContainer.Controls.Add(ButtonDiscard)

            Return ButtonContainer
        End Function

        Private Sub InitializeFooter()
            BumpTabIndexes(PanelButtonsRight, 3)

            ButtonImportAndClose = New SplitButton With {
                .Margin = New Padding(6, 0, 6, 0),
                .Text = My.Resources.Label_ImportClose,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .TabIndex = 2
            }
            PanelButtonsRight.Controls.Add(ButtonImportAndClose)

            ButtonImport = New SplitButton With {
                .Margin = New Padding(6, 0, 6, 0),
                .Text = My.Resources.Label_Import,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .TabIndex = 1
            }
            PanelButtonsRight.Controls.Add(ButtonImport)

            ButtonVerify = New Button With {
                .Margin = New Padding(6, 0, 18, 0),
                .Text = My.Resources.Label_Verify,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .TabIndex = 0,
                .Visible = False
            }
            PanelButtonsRight.Controls.Add(ButtonVerify)

            ButtonOk.Visible = False
        End Sub

        Private Sub InitializeHelp()
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Retries, _PanelDriveParams.LabelRetries, _PanelDriveParams.NumericRetries)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_SeekRetries, _PanelDriveParams.LabelSeekRetries, _PanelDriveParams.NumericSeekRetries)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Revs, _PanelDriveParams.LabelRevs, _PanelDriveParams.NumericRevs)
            SetHelpString(My.Resources.HelpStrings.Flux_SaveLog, ButtonSaveLog)
            SetHelpString(My.Resources.HelpStrings.Flux_Detect, _PanelDriveParams.ButtonDetect)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Drives, _PanelDriveParams.LabelDrive, _PanelDriveParams.ComboDrives)
            SetHelpString(My.Resources.HelpStrings.Flux_Format, _PanelDriveParams.LabelImageFormat, _PanelDriveParams.ComboImageFormat)
            SetHelpString(My.Resources.HelpStrings.Flux_ImageType, _PanelOutput.LabelOutputType, _PanelOutput.ComboBoxPrimaryOutput)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_ReadFilename, _PanelDestination.LabelFileName, _PanelDestination.TextBoxFileName)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_FileExt, _PanelOutput.ComboBoxPrimaryExtension)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_SaveLog, CheckSaveLog)
            SetHelpString(My.Resources.HelpStrings.Flux_Discard, ButtonDiscard)
            SetHelpString(My.Resources.HelpStrings.Flux_Read, ButtonRead)
            SetHelpString(My.Resources.HelpStrings.Flux_Convert, ButtonConvert)
            SetHelpString(My.Resources.HelpStrings.Flux_Preview, ButtonPreview)
            SetHelpString(My.Resources.HelpStrings.Flux_Refine, ButtonRefine)
            SetHelpString(My.Resources.HelpStrings.Flux_Verify, ButtonVerify)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_RootFolder, _PanelDestination.LabelRootFolder, _PanelDestination.TextBoxRootFolder)
            SetHelpString(My.Resources.HelpStrings.Flux_RootBrowse, _PanelDestination.ButtonBrowse)
            SetHelpString(My.Resources.HelpStrings.Flux_ImageFolder, _PanelDestination.LabelSecondaryImageFolder, _PanelDestination.TextBoxImageFolder)
            SetHelpString(My.Resources.HelpStrings.Flux_ImageFolderBrowse, _PanelDestination.ButtonImageFolderBrowse)
            SetHelpString(My.Resources.HelpStrings.Flux_FolderName, _PanelDestination.LabelFolderName, _PanelDestination.TextBoxFolderName)
            SetHelpString(My.Resources.HelpStrings.Flux_PrefixName, _PanelDestination.LabelPrefix, _PanelDestination.TextBoxPrefix)
            SetHelpString(My.Resources.HelpStrings.Flux_OutputType2, _PanelOutput.LabelOutputType2, _PanelOutput.ComboBoxSecondaryOutput)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_FileExt2, _PanelOutput.ComboBoxSecondaryExtension)
            SetHelpString(My.Resources.HelpStrings.Flux_ImageLocation, _PanelOutput.LabelLocation, _PanelOutput.ComboBoxSecondaryLocation)
            SetHelpString(My.Resources.HelpStrings.Flux_ToggleSequence, _PanelDestination.ButtonToggleSequence, _PanelDestination.ButtonToggleSequence)

            InitializeHelpImportButtons(False)
        End Sub

        Private Sub InitializeHelpImportButtons(IsFluxOutput As Boolean)
            If IsFluxOutput Then
                SetHelpString(My.Resources.HelpStrings.Flux_Save, ButtonImport)
                SetHelpString(My.Resources.HelpStrings.Flux_SaveClose, ButtonImportAndClose)
            Else
                SetHelpString(My.Resources.HelpStrings.Flux_Import, ButtonImport)
                SetHelpString(My.Resources.HelpStrings.Flux_ImportClose, ButtonImportAndClose)
            End If
        End Sub

        Private Sub ReadDiskForm_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
            If _DotPattern IsNot Nothing Then
                _DotPattern.Dispose()
                _DotPattern = Nothing
            End If
        End Sub
    End Class
End Namespace
