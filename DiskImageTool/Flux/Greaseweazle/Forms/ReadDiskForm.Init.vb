Namespace Flux.Greaseweazle
    Partial Public Class ReadDiskForm
        Private Sub InitializeControls()
            Dim ColWidth As Integer = 0

            InitializeFooter()

            With TableLayoutPanelMain
                .SuspendLayout()

                .Left = 0
                .RowCount = 10
                .ColumnCount = 11
                .Dock = DockStyle.Fill

                FillAutoSizeStyles()

                .ColumnStyles(0).SizeType = SizeType.Absolute

                Dim row As Integer = 0

                InitializeControlsRowDrive(row, ColWidth) : row += 1
                InitializeControlsRowFormat(row, ColWidth) : row += 1
                GridAddSpacerRow(row) : row += 1
                InitializeControlsRowFileName(row, ColWidth) : row += 1
                InitializeControlsRowPrefix(row, ColWidth) : row += 1
                InitializeControlsRowRootFolder(row, ColWidth) : row += 1
                InitializeControlsRowImageFolder(row, ColWidth) : row += 1
                InitializeControlsRowFolderName(row, ColWidth) : row += 1
                GridAddSeparatorrRow(row) : row += 1
                InitializeControlsRowGrid(row) : row += 1

                .ColumnStyles(0).Width = ColWidth

                .ResumeLayout()
                '.Left = (.Parent.ClientSize.Width - .Width) \ 2
            End With
        End Sub

        Private Sub InitializeControlsRowDrive(Row As Integer, ByRef ColWidth As Integer)
            _GridIndex.Add(GridRows.Drive, Row)

            _LabelDrive = New Label With {
                .Text = My.Resources.Label_Drive,
                .Anchor = AnchorStyles.Right,
                .TextAlign = ContentAlignment.MiddleRight,
                .AutoSize = False
            }
            ColWidth = Math.Max(ColWidth, ReadDiskHelpers.SetControlWidth(_LabelDrive))

            ComboDrives = New ComboBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Width = 180
            }

            _LabelOutputType = New Label With {
                .Text = My.Resources.Label_OutputType,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            ComboOutputType = New ComboBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ComboExtensions = New ComboBox With {
                .Anchor = AnchorStyles.Left,
                .Width = 50,
                .DropDownStyle = ComboBoxStyle.DropDownList
            }

            CheckBoxSaveLog = New CheckBox With {
                .Text = My.Resources.Label_SaveLog,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            With TableLayoutPanelMain
                .Controls.Add(_LabelDrive, 0, Row)
                .Controls.AddWithSpan(ComboDrives, 1, Row, 2)

                .Controls.Add(_LabelOutputType, 3, Row)
                .Controls.AddWithSpan(ComboOutputType, 4, Row, 4)
                .Controls.Add(ComboExtensions, 8, Row)

                .Controls.AddWithSpan(CheckBoxSaveLog, 9, Row, 2)
            End With
        End Sub

        Private Sub InitializeControlsRowFileName(Row As Integer, ByRef ColWidth As Integer)
            _GridIndex.Add(GridRows.FileName, Row)

            _LabelFileName = New Label With {
                .Text = My.Resources.Label_FileName,
                .Anchor = AnchorStyles.Right,
                .TextAlign = ContentAlignment.MiddleRight,
                .AutoSize = False
            }
            ColWidth = Math.Max(ColWidth, ReadDiskHelpers.SetControlWidth(_LabelFileName))

            TextBoxFileName = New TextBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .MaxLength = 255
            }

            ButtonToggleSequence = New Button With {
                .Margin = New Padding(15, 0, 0, 0),
                .Text = "< >",
                .Anchor = AnchorStyles.Left,
                .AutoSize = True
            }
            _ToolTip.SetToolTip(ButtonToggleSequence, My.Resources.Label_ToggleSequence & " (Alt+S)")

            With TableLayoutPanelMain
                .Controls.Add(_LabelFileName, 0, Row)
                .Controls.AddWithSpan(TextBoxFileName, 1, Row, 8)
                .Controls.AddWithSpan(ButtonToggleSequence, 9, Row, 2)
            End With
        End Sub

        Private Sub InitializeControlsRowFolderName(Row As Integer, ByRef ColWidth As Integer)
            _GridIndex.Add(GridRows.FolderName, Row)

            _LabelFolderName = New Label With {
                .Text = My.Resources.Label_FolderName,
                .Anchor = AnchorStyles.Right,
                .TextAlign = ContentAlignment.MiddleRight,
                .AutoSize = False
            }
            ColWidth = Math.Max(ColWidth, ReadDiskHelpers.SetControlWidth(_LabelFolderName))

            TextBoxFolderName = New TextBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .MaxLength = 255
            }

            ButtonToggleSequence2 = New Button With {
                .Margin = New Padding(15, 0, 0, 0),
                .Text = "< >",
                .Anchor = AnchorStyles.Left,
                .AutoSize = True
            }
            _ToolTip.SetToolTip(ButtonToggleSequence2, My.Resources.Label_ToggleSequence & " (Alt+S)")

            With TableLayoutPanelMain
                .Controls.Add(_LabelFolderName, 0, Row)
                .Controls.AddWithSpan(TextBoxFolderName, 1, Row, 8)
                .Controls.AddWithSpan(ButtonToggleSequence2, 9, Row, 2)
            End With
        End Sub

        Private Sub InitializeControlsRowFormat(Row As Integer, ByRef ColWidth As Integer)
            _GridIndex.Add(GridRows.Format, Row)

            _LabelImageFormat = New Label With {
                .Text = My.Resources.Label_Format,
                .Anchor = AnchorStyles.Right,
                .TextAlign = ContentAlignment.MiddleRight,
                .AutoSize = False
            }
            ColWidth = Math.Max(ColWidth, ReadDiskHelpers.SetControlWidth(_LabelImageFormat))

            ComboImageFormat = New ComboBox With {
                .Anchor = AnchorStyles.Left,
                .Width = 200
            }

            ButtonDetect = New Button With {
                .Width = 75,
                .Margin = New Padding(3, 3, 3, 3),
                .Text = My.Resources.Label_Detect
            }

            _LabelRevs = New Label With {
                .Text = My.Resources.Label_Revs,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            NumericRevs = New NumericUpDown With {
                .Minimum = MIN_REVS,
                .Maximum = MAX_REVS,
                .Width = 50,
                .Anchor = AnchorStyles.Left
            }

            _LabelRetries = New Label With {
                .Text = My.Resources.Label_Retries,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            _NumericRetries = New NumericUpDown With {
                .Minimum = MIN_RETRIES,
                .Maximum = MAX_RETRIES,
                .Width = 45,
                .Anchor = AnchorStyles.Left
            }

            _LabelSeekRetries = New Label With {
                .Text = My.Resources.Label_SeekRetries,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            _NumericSeekRetries = New NumericUpDown With {
                .Minimum = MIN_RETRIES,
                .Maximum = MAX_RETRIES,
                .Width = 45,
                .Anchor = AnchorStyles.Left
            }

            With TableLayoutPanelMain
                .Controls.Add(_LabelImageFormat, 0, Row)
                .Controls.AddWithSpan(ComboImageFormat, 1, Row, 2)
                .Controls.Add(ButtonDetect, 3, Row)

                .Controls.Add(_LabelRevs, 4, Row)
                .Controls.Add(NumericRevs, 5, Row)

                .Controls.AddWithSpan(_LabelRetries, 6, Row, 2)
                .Controls.Add(_NumericRetries, 8, Row)

                .Controls.Add(_LabelSeekRetries, 9, Row)
                .Controls.Add(_NumericSeekRetries, 10, Row)
            End With
        End Sub

        Private Sub InitializeControlsRowGrid(Row As Integer)
            _GridIndex.Add(GridRows.Grid, Row)

            Dim ButtonContainer As New FlowLayoutPanel With {
                .FlowDirection = FlowDirection.TopDown,
                .AutoSize = True,
                .Margin = New Padding(12, 24, 3, 3)
            }

            ButtonRefine = New Button With {
                .Margin = New Padding(3, 0, 3, 3),
                .Text = My.Resources.Label_Refine,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonPreview = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Preview,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonRead = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Read,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonConvert = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Convert,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Visible = False
            }

            ButtonDiscard = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Discard,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonContainer.Controls.Add(ButtonRefine)
            ButtonContainer.Controls.Add(ButtonPreview)
            ButtonContainer.Controls.Add(ButtonRead)
            ButtonContainer.Controls.Add(ButtonConvert)
            ButtonContainer.Controls.Add(ButtonDiscard)

            With TableLayoutPanelMain
                .Controls.AddWithSpan(TableSide0, 0, Row, 3)
                .Controls.AddWithSpan(TableSide1, 3, Row, 6)
                .Controls.AddWithSpan(ButtonContainer, 9, Row, 2)
            End With
        End Sub

        Private Sub InitializeControlsRowImageFolder(Row As Integer, ByRef ColWidth As Integer)
            _GridIndex.Add(GridRows.ImageFolder, Row)

            _LabelImageFolder = New Label With {
                .Text = My.Resources.Label_ImageFolder,
                .Anchor = AnchorStyles.Right,
                .TextAlign = ContentAlignment.MiddleRight,
                .AutoSize = False
            }
            ColWidth = Math.Max(ColWidth, ReadDiskHelpers.SetControlWidth(_LabelImageFolder))

            TextBoxImageFolder = New TextBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .MaxLength = 255,
                .[ReadOnly] = True,
                .BackColor = SystemColors.Window
            }

            ButtonImageFolderBrowse = New Button With {
                .Text = My.Resources.Label_Browse,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True
            }

            With TableLayoutPanelMain
                .Controls.Add(_LabelImageFolder, 0, Row)
                .Controls.AddWithSpan(TextBoxImageFolder, 1, Row, 6)
                .Controls.AddWithSpan(ButtonImageFolderBrowse, 7, Row, 2)
            End With
        End Sub

        Private Sub InitializeControlsRowPrefix(Row As Integer, ByRef ColWidth As Integer)
            _GridIndex.Add(GridRows.Prefix, Row)

            _LabelPrefixName = New Label With {
                .Text = My.Resources.Label_Prefix,
                .Anchor = AnchorStyles.Right,
                .TextAlign = ContentAlignment.MiddleRight,
                .AutoSize = False,
                .Visible = False
            }
            ColWidth = Math.Max(ColWidth, ReadDiskHelpers.SetControlWidth(_LabelPrefixName))

            TextBoxPrefixName = New PlaceholderTextBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .MaxLength = 255,
                .Visible = False,
                .PlaceholderText = DEFAULT_RAW_FILE_NAME,
                .ShowCueWhenFocused = True
            }

            _LabelOutputType2 = New Label With {
                .Text = My.Resources.Label_OutputType & " 2",
                .Anchor = AnchorStyles.Right,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            ComboOutputType2 = New ComboBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ComboExtensions2 = New ComboBox With {
                .Anchor = AnchorStyles.Left,
                .Width = 50,
                .DropDownStyle = ComboBoxStyle.DropDownList
            }

            _LabelLocation = New Label With {
                .Text = My.Resources.Label_ImageLocation,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            ComboImageLocation = New ComboBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Margin = New Padding(0, 3, 0, 3)
            }

            With TableLayoutPanelMain
                .Controls.Add(_LabelPrefixName, 0, Row)
                .Controls.Add(TextBoxPrefixName, 1, Row)
                .Controls.Add(_LabelOutputType2, 2, Row)
                .Controls.AddWithSpan(ComboOutputType2, 3, Row, 2)
                .Controls.Add(ComboExtensions2, 5, Row)
                .Controls.AddWithSpan(_LabelLocation, 6, Row, 3)
                .Controls.AddWithSpan(ComboImageLocation, 9, Row, 2)
            End With
        End Sub

        Private Sub InitializeControlsRowRootFolder(Row As Integer, ByRef ColWidth As Integer)
            _GridIndex.Add(GridRows.RootFolder, Row)

            _LabelRootFolder = New Label With {
                .Text = My.Resources.Label_RootFolder,
                .Anchor = AnchorStyles.Right,
                .TextAlign = ContentAlignment.MiddleRight,
                .AutoSize = False
            }
            ColWidth = Math.Max(ColWidth, ReadDiskHelpers.SetControlWidth(_LabelRootFolder))

            TextBoxRootFolder = New TextBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .MaxLength = 255,
                .[ReadOnly] = True,
                .BackColor = SystemColors.Window
            }

            ButtonRootBrowse = New Button With {
                .Text = My.Resources.Label_Browse,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True
            }

            With TableLayoutPanelMain
                .Controls.Add(_LabelRootFolder, 0, Row)
                .Controls.AddWithSpan(TextBoxRootFolder, 1, Row, 6)
                .Controls.AddWithSpan(ButtonRootBrowse, 7, Row, 2)
            End With
        End Sub

        Private Sub InitializeFooter()
            ButtonVerify = New Button With {
                .Margin = New Padding(6, 0, 18, 0),
                .Text = My.Resources.Label_Verify,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .TabIndex = 0,
                .Visible = False
            }

            ButtonImport = New SplitButton With {
                .Margin = New Padding(6, 0, 6, 0),
                .Text = My.Resources.Label_Import,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .TabIndex = 1
            }

            ButtonImportAndClose = New SplitButton With {
                .Margin = New Padding(6, 0, 6, 0),
                .Text = My.Resources.Label_ImportClose,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .TabIndex = 2
            }

            BumpTabIndexes(PanelButtonsRight, 3)
            PanelButtonsRight.Controls.Add(ButtonImportAndClose)
            PanelButtonsRight.Controls.Add(ButtonImport)
            PanelButtonsRight.Controls.Add(ButtonVerify)

            ButtonOk.Visible = False
        End Sub

        Private Sub InitializeHelp()
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Retries, _LabelRetries, _NumericRetries)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_SeekRetries, _LabelSeekRetries, _NumericSeekRetries)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Revs, _LabelRevs, NumericRevs)
            SetHelpString(My.Resources.HelpStrings.Flux_SaveLog, ButtonSaveLog)
            SetHelpString(My.Resources.HelpStrings.Flux_Detect, ButtonDetect)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Drives, _LabelDrive, ComboDrives)
            SetHelpString(My.Resources.HelpStrings.Flux_Format, _LabelImageFormat, ComboImageFormat)
            SetHelpString(My.Resources.HelpStrings.Flux_ImageType, _LabelOutputType, ComboOutputType)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_ReadFilename, _LabelFileName, TextBoxFileName)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_FileExt, ComboExtensions)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_SaveLog, CheckBoxSaveLog)
            SetHelpString(My.Resources.HelpStrings.Flux_Discard, ButtonDiscard)
            SetHelpString(My.Resources.HelpStrings.Flux_Read, ButtonRead)
            SetHelpString(My.Resources.HelpStrings.Flux_Convert, ButtonConvert)
            SetHelpString(My.Resources.HelpStrings.Flux_Preview, ButtonPreview)
            SetHelpString(My.Resources.HelpStrings.Flux_Refine, ButtonRefine)
            SetHelpString(My.Resources.HelpStrings.Flux_Verify, ButtonVerify)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_RootFolder, _LabelRootFolder, TextBoxRootFolder)
            SetHelpString(My.Resources.HelpStrings.Flux_RootBrowse, ButtonRootBrowse)
            SetHelpString(My.Resources.HelpStrings.Flux_ImageFolder, _LabelImageFolder, TextBoxImageFolder)
            SetHelpString(My.Resources.HelpStrings.Flux_ImageFolderBrowse, ButtonImageFolderBrowse)
            SetHelpString(My.Resources.HelpStrings.Flux_FolderName, _LabelFolderName, TextBoxFolderName)
            SetHelpString(My.Resources.HelpStrings.Flux_PrefixName, _LabelPrefixName, TextBoxPrefixName)
            SetHelpString(My.Resources.HelpStrings.Flux_OutputType2, _LabelOutputType2, ComboOutputType2)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_FileExt2, ComboExtensions2)
            SetHelpString(My.Resources.HelpStrings.Flux_ImageLocation, _LabelLocation, ComboImageLocation)
            SetHelpString(My.Resources.HelpStrings.Flux_ToggleSequence, ButtonToggleSequence)
            SetHelpString(My.Resources.HelpStrings.Flux_ToggleSequence, ButtonToggleSequence2)

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
    End Class
End Namespace
