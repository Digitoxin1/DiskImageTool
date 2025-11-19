Imports System.ComponentModel
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Public Class ReadDiskForm
        Inherits BaseForm

        Private WithEvents ButtonBrowse As Button
        Private WithEvents ButtonDetect As Button
        Private WithEvents ButtonDiscard As Button
        Private WithEvents ButtonImport As Button
        Private WithEvents ButtonKeep As Button
        Private WithEvents ButtonProcess As Button
        Private WithEvents ButtonReset As Button
        Private WithEvents CheckBoxDoublestep As CheckBox
        Private WithEvents ComboExtensions As ComboBox
        Private WithEvents ComboImageDrives As ComboBox
        Private WithEvents ComboImageFormat As ComboBox
        Private WithEvents ComboOutputType As ComboBox
        Private WithEvents TextBoxFileName As TextBox
        Private WithEvents TextBoxFluxFolder As TextBox
        Private Const DEFAULT_RAW_FILE_NAME As String = "track"
        Private Shared _CachedDriveId As String = ""
        Private Shared _CachedFileNameTemplate As String = ""
        Private ReadOnly _Initialized As Boolean = False
        Private ReadOnly _TrackStatus As TrackStatus
        Private _ComboExtensionsNoEvent As Boolean = False
        Private _ComboImageFormatNoEvent As Boolean = False
        Private _DoubleStep As Boolean = False
        Private _NumericRetries As NumericUpDown
        Private _NumericRevs As NumericUpDown
        Private _NumericSeekRetries As NumericUpDown
        Private _OutputFilePath As String = ""
        Private _ProcessingFilePath As String = ""
        Private _ProcessingAction As TrackStatus.ActionTypeEnum = TrackStatus.ActionTypeEnum.Read
        Private _StagingFileIsFlux As Boolean = False
        Private _StagingFilePath As String = ""
        Private LabelFluxFolder As Label
        Private LabelWarning As Label
        Private LaelFileName As Label
        Public Event ImportRequested(File As String, NewFilename As String)

        Public Sub New()
            MyBase.New(Settings.LogFileName)
            InitializeControls()

            _TrackStatus = New TrackStatus(Me)

            Me.Text = My.Resources.Label_ReadDisk

            PopulateDrives(ComboImageDrives, FloppyMediaType.MediaUnknown, _CachedDriveId)
            PopulateImageFormats(ComboImageFormat, ComboImageDrives.SelectedValue)
            InitializeImage()

            TextBoxFileName.Text = _CachedFileNameTemplate
            SetFluxFolder(App.AppSettings.Greaseweazle.FluxRootPath)

            _NumericRevs.Value = Settings.DefaultRevs
            _NumericRetries.Value = CommandLineBuilder.DEFAULT_RETRIES
            _NumericSeekRetries.Value = CommandLineBuilder.DEFAULT_SEEK_RETRIES

            _Initialized = True
        End Sub

        Public ReadOnly Property OutputFilePath As String
            Get
                Return _OutputFilePath
            End Get
        End Property

        Public Function GetNewFileName() As String
            Dim Item As FileExtensionItem = ComboExtensions.SelectedItem

            Dim FileName As String = TextBoxFileName.Text

            If ContainsPlaceholder(FileName) Then
                FileName = StripAngleBrackets(FileName)
            End If

            Return FileName & Item.Extension
        End Function

        Public Sub SetFluxFolder(Path As String)
            If Path <> "" AndAlso Not IO.Directory.Exists(Path) Then
                Path = ""
            End If

            TextBoxFluxFolder.Text = Path
            App.AppSettings.Greaseweazle.FluxRootPath = Path
        End Sub

        Protected Overrides Sub OnAfterBaseFormClosing(e As FormClosingEventArgs)
            If e.CloseReason = CloseReason.UserClosing OrElse CancelButtonClicked Then
                ClearStagingFile()
            End If
        End Sub

        Private Sub CashFilenameTemplate()
            Dim Filename = TextBoxFileName.Text

            If ContainsPlaceholder(Filename) Then
                _CachedFileNameTemplate = IncrementPlaceholders(Filename)
            Else
                _CachedFileNameTemplate = ""
            End If
        End Sub

        Private Function CheckCompatibility() As Boolean
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            If Opt.Type = FloppyMediaType.MediaUnknown Then
                Return True
            End If

            If DiskParams.IsNonImage Then
                Return True
            End If

            Dim FloppyType = GreaseweazleFindCompatibleFloppyType(DiskParams, Opt.Type)

            Return FloppyType = Opt.Type
        End Function

        Private Sub ClearProcessedFile()
            If String.IsNullOrEmpty(_ProcessingFilePath) Then
                Exit Sub
            End If

            Dim OutputType As ReadDiskOutputTypes = ComboOutputType.SelectedValue

            If OutputType = ReadDiskOutputTypes.RAW Then
                Dim PathName = IO.Path.GetDirectoryName(_ProcessingFilePath)
                DeleteFilesAndFolderIfEmpty(PathName, "*.raw")
            Else
                DeleteFileIfExists(_ProcessingFilePath)
            End If

            _ProcessingFilePath = ""
        End Sub

        Public Sub ResetInterface()
            TextBoxConsole.Clear()
            ClearStatusBar()
            _TrackStatus.Clear()
            ResetTrackGrid()
            SetTiltebarText()
        End Sub

        Private Sub ClearStagedImage(KeepFile As Boolean)
            Dim IsFlux As Boolean = _StagingFileIsFlux

            If KeepFile Then
                _StagingFilePath = ""
                _StagingFileIsFlux = False
            Else
                ClearStagingFile()
            End If

            ResetInterface()

            Dim OutputType As ReadDiskOutputTypes = ComboOutputType.SelectedValue
            If IsFlux Then
                OutputType = ReadDiskOutputTypes.RAW
            End If
            PopulateOutputTypes(OutputType)
        End Sub

        Private Sub ClearStagingFile()
            If String.IsNullOrEmpty(_StagingFilePath) Then
                Exit Sub
            End If

            If _StagingFileIsFlux Then
                Dim PathName = IO.Path.GetDirectoryName(_StagingFilePath)
                DeleteFilesAndFolderIfEmpty(PathName, "*.raw")
            Else
                DeleteFileIfExists(_StagingFilePath)
            End If

            _StagingFilePath = ""
            _StagingFileIsFlux = False
        End Sub

        Private Sub DoFormatDetection()
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            Dim ImageFormat As FloppyDiskFormat

            If Opt.Id = "" Then
                ImageFormat = GreaseweazleImageFormat.None
            Else
                ImageFormat = ReadImageFormat(Opt.Id)
                Opt.SelectedFormat = ImageFormat
                Opt.DetectedFormat = ImageFormat
            End If

            PopulateImageFormats(ComboImageFormat, ImageFormat, ImageFormat)
        End Sub

        Private Sub FluxFolderBrowse()
            Dim FolderName = BrowseFolder(TextBoxFluxFolder.Text)
            If FolderName <> "" Then
                SetFluxFolder(FolderName)
            End If
        End Sub

        Private Function GetOutputFilePath() As String
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue
            Dim FilePath As String
            Dim OutputType As ReadDiskOutputTypes = ComboOutputType.SelectedValue

            If OutputType = ReadDiskOutputTypes.RAW Then
                Dim FileName = TextBoxFileName.Text.Trim
                If ContainsPlaceholder(FileName) Then
                    FileName = StripAngleBrackets(FileName)
                End If
                FilePath = IO.Path.Combine(TextBoxFluxFolder.Text, FileName, DEFAULT_RAW_FILE_NAME & "00.0.raw")
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(FilePath))
            Else
                If Not DiskParams.IsStandard Then
                    OutputType = ReadDiskOutputTypes.HFE
                End If

                FilePath = GenerateOutputFile(ReadDisktOutputTypeFileExt(OutputType))
            End If

            Return FilePath
        End Function

        Private Sub InitializeControls()
            LabelFluxFolder = New Label With {
                .Text = My.Resources.Label_RootFolder,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            TextBoxFluxFolder = New TextBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .MaxLength = 255,
                .[ReadOnly] = True,
                .BackColor = SystemColors.Window
            }

            ButtonBrowse = New Button With {
                .Text = My.Resources.Label_Browse,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True
            }

            Dim LabelDrive As New Label With {
                .Text = My.Resources.Label_Drive,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            ComboImageDrives = New ComboBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Width = 180
            }

            LaelFileName = New Label With {
                .Text = My.Resources.Label_FileName,
                .TextAlign = ContentAlignment.MiddleRight,
                .Anchor = AnchorStyles.Right,
                .AutoSize = False,
                .Width = TextRenderer.MeasureText(My.Resources.Label_FolderName, DefaultFont).Width
            }

            TextBoxFileName = New TextBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .MaxLength = 255
            }

            ComboExtensions = New ComboBox With {
                .Anchor = AnchorStyles.Left,
                .Width = 50,
                .DropDownStyle = ComboBoxStyle.DropDownList
            }

            Dim LabelImageFormat As New Label With {
                .Text = My.Resources.Label_Format,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            ComboImageFormat = New ComboBox With {
                .Anchor = AnchorStyles.Left,
                .Width = 200
            }

            Dim LabelOutputType As New Label With {
                .Text = My.Resources.Label_OutputType,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            ComboOutputType = New ComboBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Width = 175
            }

            CheckBoxDoublestep = New CheckBox With {
                .Text = My.Resources.Label_DoubleStep,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            Dim LabelSeekRetries As New Label With {
                .Text = My.Resources.Label_SeekRetries,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            _NumericSeekRetries = New NumericUpDown With {
                .Minimum = CommandLineBuilder.MIN_RETRIES,
                .Maximum = CommandLineBuilder.MAX_RETRIES,
                .Width = 45,
                .Anchor = AnchorStyles.Left
            }

            Dim LabelRetries As New Label With {
                .Text = My.Resources.Label_Retries,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            _NumericRetries = New NumericUpDown With {
                .Minimum = CommandLineBuilder.MIN_RETRIES,
                .Maximum = CommandLineBuilder.MAX_RETRIES,
                .Width = 45,
                .Anchor = AnchorStyles.Left
            }

            Dim LabelRevs As New Label With {
                .Text = My.Resources.Label_Revs,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True,
                 .Margin = New Padding(12, 3, 3, 3)
            }

            _NumericRevs = New NumericUpDown With {
                .Minimum = CommandLineBuilder.MIN_REVS,
                .Maximum = CommandLineBuilder.MAX_REVS,
                .Width = 40,
                .Anchor = AnchorStyles.Left
            }

            Dim ButtonContainer As New FlowLayoutPanel With {
                .FlowDirection = FlowDirection.TopDown,
                .AutoSize = True,
                .Margin = New Padding(12, 24, 3, 3)
            }

            ButtonProcess = New Button With {
                .Margin = New Padding(3, 0, 3, 3),
                .Text = My.Resources.Label_Write,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonKeep = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Keep,
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

            ButtonReset = New Button With {
                .Margin = New Padding(6, 0, 6, 0),
                .Text = My.Resources.Label_Reset,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .TabIndex = 0
            }

            ButtonImport = New Button With {
                .Margin = New Padding(6, 0, 6, 0),
                .Text = My.Resources.Label_Import,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .TabIndex = 0
            }

            PanelButtonsLeft.Controls.Add(ButtonReset)
            ButtonReset.BringToFront()

            BumpTabIndexes(PanelButtonsRight)
            PanelButtonsRight.Controls.Add(ButtonImport)

            ButtonDetect = New Button With {
                .Width = 75,
                .Margin = New Padding(3, 3, 3, 3),
                .Text = My.Resources.Label_Detect
            }

            ButtonContainer.Controls.Add(ButtonProcess)
            ButtonContainer.Controls.Add(ButtonKeep)
            ButtonContainer.Controls.Add(ButtonDiscard)


            ButtonOk.Text = "Import and Close"
            ButtonOk.Visible = True

            Dim PanelLabel As New Panel With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Height = 20
            }

            LabelWarning = New Label With {
                .Text = My.Resources.Message_ImageFormatWarning,
                .ForeColor = Color.Red,
                .AutoSize = True,
                .TextAlign = ContentAlignment.TopRight,
                .Dock = DockStyle.Right,
                .Visible = False
            }

            PanelLabel.Controls.Add(LabelWarning)

            Dim Row As Integer

            With TableLayoutPanelMain
                .SuspendLayout()

                .Left = 0
                .RowCount = 6
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

                .ColumnStyles(0).SizeType = SizeType.Percent
                .ColumnStyles(0).Width = 100

                Row = 0
                .Controls.Add(LaelFileName, 0, Row)
                .Controls.Add(TextBoxFileName, 1, Row)
                .SetColumnSpan(TextBoxFileName, 6)

                .Controls.Add(ComboExtensions, 7, Row)

                Row = 1
                .Controls.Add(LabelFluxFolder, 0, Row)
                .Controls.Add(TextBoxFluxFolder, 1, Row)
                .SetColumnSpan(TextBoxFluxFolder, 5)
                .Controls.Add(ButtonBrowse, 6, Row)
                .SetColumnSpan(ButtonBrowse, 2)

                Row = 2
                .Controls.Add(LabelDrive, 0, Row)
                .Controls.Add(ComboImageDrives, 1, Row)

                .Controls.Add(LabelOutputType, 2, Row)
                .Controls.Add(ComboOutputType, 3, Row)
                .SetColumnSpan(ComboOutputType, 5)

                .Controls.Add(CheckBoxDoublestep, 8, Row)

                Row = 3
                .Controls.Add(LabelImageFormat, 0, Row)
                .Controls.Add(ComboImageFormat, 1, Row)
                .Controls.Add(ButtonDetect, 2, Row)

                .Controls.Add(LabelRevs, 3, Row)
                .Controls.Add(_NumericRevs, 4, Row)

                .Controls.Add(LabelRetries, 5, Row)
                .SetColumnSpan(LabelRetries, 2)
                .Controls.Add(_NumericRetries, 7, Row)


                Row = 4
                .Controls.Add(PanelLabel, 0, Row)
                .SetColumnSpan(PanelLabel, 2)

                .Controls.Add(LabelSeekRetries, 4, Row)
                .SetColumnSpan(LabelSeekRetries, 3)
                .Controls.Add(_NumericSeekRetries, 7, Row)

                Row = 5
                .Controls.Add(TableSide0, 0, Row)
                .SetColumnSpan(TableSide0, 2)

                .Controls.Add(TableSide1, 2, Row)
                .SetColumnSpan(TableSide1, 6)

                .Controls.Add(ButtonContainer, 8, Row)

                .ResumeLayout()
                '.Left = (.Parent.ClientSize.Width - .Width) \ 2
            End With
        End Sub

        Private Sub InitializeImage()
            PopulateOutputTypes()
            PopulateFileExtensions()
            ResetTrackGrid()
            ClearStatusBar()
            RefreshButtonState(True)
            SetTiltebarText()
        End Sub

        Private Sub PopulateFileExtensions()
            _ComboExtensionsNoEvent = True

            Dim OutputType As ReadDiskOutputTypes = ComboOutputType.SelectedValue

            If OutputType = ReadDiskOutputTypes.HFE Or OutputType = ReadDiskOutputTypes.RAW Then
                Dim items As New List(Of FileExtensionItem) From {
                    New FileExtensionItem(ReadDisktOutputTypeFileExt(OutputType), Nothing)
                }
                With ComboExtensions
                    .DataSource = Nothing
                    .Items.Clear()
                    .DisplayMember = "Extension"
                    .DataSource = items
                    .SelectedIndex = 0
                    .DropDownStyle = ComboBoxStyle.DropDownList
                    .Enabled = False
                End With
            Else
                Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue
                ComboExtensions.Enabled = True
                SharedLib.PopulateFileExtensions(ComboExtensions, ImageParams.Format)
            End If

            _ComboExtensionsNoEvent = False
        End Sub

        Private Sub PopulateOutputTypes(Optional CurrentValue As ReadDiskOutputTypes? = Nothing)

            Dim DriveList As New List(Of KeyValuePair(Of String, ReadDiskOutputTypes))
            For Each OutputType As ReadDiskOutputTypes In [Enum].GetValues(GetType(ReadDiskOutputTypes))
                If _StagingFileIsFlux AndAlso OutputType = ReadDiskOutputTypes.RAW Then
                    Continue For
                End If
                DriveList.Add(New KeyValuePair(Of String, ReadDiskOutputTypes)(
                    ReadDiskOutputTypeDescription(OutputType), OutputType)
                )
            Next

            InitializeCombo(ComboOutputType, DriveList, CurrentValue)

            If ComboOutputType.Items.Count > 0 AndAlso ComboOutputType.SelectedIndex = -1 Then
                ComboOutputType.SelectedIndex = 0
            End If
        End Sub

        Private Sub ProcessRawImage()
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            If DiskParams.IsNonImage Then
                Exit Sub
            End If

            Dim OutputType As ImageImportOutputTypes = ComboOutputType.SelectedValue

            Dim FilePath = GenerateOutputFile(ImageImportOutputTypeFileExt(OutputType))
            If FilePath = "" Then
                Exit Sub
            End If

            ResetInterface()

            _ProcessingFilePath = FilePath
            _ProcessingAction = TrackStatus.ActionTypeEnum.Import

            _DoubleStep = DiskParams.IsStandard AndAlso CheckBoxDoublestep.Enabled AndAlso CheckBoxDoublestep.Checked

            Dim Arguments = GenerateCommandLineImport(_StagingFilePath, _ProcessingFilePath, DiskParams, OutputType, _DoubleStep)
            Process.StartAsync(Settings.AppPath, Arguments)
        End Sub

        Private Sub ProcessImage()
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            If DiskParams.IsNonImage Then
                Exit Sub
            End If

            If Opt.Id = "" Then
                Exit Sub
            End If

            Dim FilePath As String = GetOutputFilePath()

            ClearStagedImage(False)

            _ProcessingFilePath = FilePath
            _ProcessingAction = TrackStatus.ActionTypeEnum.Read

            Dim OutputType As ReadDiskOutputTypes = ComboOutputType.SelectedValue
            _DoubleStep = DiskParams.IsStandard AndAlso CheckBoxDoublestep.Enabled AndAlso CheckBoxDoublestep.Checked

            Dim Arguments = GenerateCommandLineRead(FilePath, Opt, DiskParams, OutputType, _DoubleStep, _NumericRetries.Value, _NumericSeekRetries.Value, _NumericRevs.Value)

            Process.StartAsync(Settings.AppPath, Arguments)
        End Sub

        Private Sub ProcessImport()
            If String.IsNullOrEmpty(_StagingFilePath) Then
                Exit Sub
            End If

            If _StagingFileIsFlux Then
                ProcessImportStage()
                Exit Sub
            End If

            RaiseEvent ImportRequested(_StagingFilePath, GetNewFileName())
            CashFilenameTemplate()
            ClearStagedImage(True)
            RefreshButtonState(True)
            TextBoxFileName.Text = _CachedFileNameTemplate
        End Sub

        Private Sub ProcessImportStage()
            If String.IsNullOrEmpty(_StagingFilePath) Then
                Exit Sub
            End If

            ProcessRawImage()
        End Sub
        Private Sub ProcessOutputLine(line As String, Action As TrackStatus.ActionTypeEnum)
            If TextBoxConsole.Text.Length > 0 Then
                TextBoxConsole.AppendText(Environment.NewLine)
            End If
            TextBoxConsole.AppendText(line)

            _TrackStatus.ProcessOutputLineRead(line, Action, _DoubleStep)
        End Sub

        Private Function ReadImageFormat(DriveId As String) As DiskImage.FloppyDiskFormat
            Dim Response = ReadFirstTrack(DriveId)

            TextBoxConsole.Text = Response.Output

            If Not Response.Result Then
                Return GreaseweazleImageFormat.None
            End If

            Return DetectImageFormat(Response.FileName, True)
        End Function

        Private Sub RefreshButtonState(CheckImageFormat As Boolean)
            Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue
            Dim OutputType As ReadDiskOutputTypes = ComboOutputType.SelectedValue
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            Dim HasStagingFile As Boolean = Not String.IsNullOrEmpty(_StagingFilePath)
            Dim IsRunning As Boolean = Process.IsRunning
            Dim IsIdle As Boolean = Not IsRunning
            Dim IsFlux As Boolean = _StagingFileIsFlux

            Dim CanChangeSettings As Boolean = IsIdle AndAlso Not HasStagingFile
            Dim CanChangeOutputFile As Boolean = IsIdle AndAlso (Not HasStagingFile Or IsFlux)
            Dim DriveSelected As Boolean = Not String.IsNullOrEmpty(Opt.Id)

            Dim OutputTypeDisabled As Boolean = False
            If CheckImageFormat AndAlso Not ImageParams.IsNonImage Then
                Dim imageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(ImageParams.Format)
                OutputTypeDisabled = (imageFormat = GreaseweazleImageFormat.None)
            End If

            ComboImageFormat.Enabled = CanChangeSettings AndAlso DriveSelected
            ComboImageDrives.Enabled = CanChangeSettings
            ComboOutputType.Enabled = CanChangeOutputFile AndAlso Not OutputTypeDisabled

            _NumericRevs.Enabled = CanChangeSettings
            _NumericRetries.Enabled = CanChangeSettings
            _NumericSeekRetries.Enabled = CanChangeSettings

            ButtonSaveLog.Enabled = IsIdle AndAlso TextBoxConsole.TextLength > 0
            ButtonReset.Enabled = IsIdle

            ButtonKeep.Enabled = IsIdle AndAlso HasStagingFile AndAlso IsFlux
            ButtonKeep.Visible = IsFlux

            ButtonDiscard.Enabled = IsIdle AndAlso HasStagingFile

            Dim Is525DDStandard As Boolean = ImageParams.IsStandard AndAlso ImageParams.MediaType = FloppyMediaType.Media525DoubleDensity

            If Is525DDStandard Then
                CheckBoxDoublestep.Enabled = CanChangeSettings AndAlso Opt.Tracks > 42
                CheckBoxDoublestep.Checked = Opt.Tracks > 79
            Else
                CheckBoxDoublestep.Enabled = False
                CheckBoxDoublestep.Checked = False
            End If

            ButtonDetect.Enabled = CanChangeSettings AndAlso DriveSelected

            ButtonCancel.Text = If(IsRunning OrElse HasStagingFile, My.Resources.Label_Cancel, My.Resources.Label_Close)

            ButtonProcess.Text = If(IsRunning, My.Resources.Label_Abort, My.Resources.Label_Read)


            RefreshProcessButtonState()
            RefreshImportButtonState()
            ToggleRootFolderControls()
        End Sub

        Private Sub RefreshImportButtonState()
            Dim EnableImport As Boolean = Not Process.IsRunning AndAlso Not String.IsNullOrEmpty(_StagingFilePath) AndAlso Not String.IsNullOrEmpty(TextBoxFileName.Text)

            ButtonOk.Enabled = EnableImport
            ButtonImport.Enabled = EnableImport
        End Sub

        Private Sub RefreshPreferredExensions()
            Dim Item As FileExtensionItem = ComboExtensions.SelectedValue

            If Not Item.Format.HasValue Then
                Exit Sub
            End If

            Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            If Item.Format.Value <> ImageParams.Format Then
                App.AppSettings.RemovePreferredExtension(ImageParams.Format)
            End If

            App.AppSettings.SetPreferredExtension(Item.Format.Value, Item.Extension)
        End Sub

        Private Sub RefreshProcessButtonState()
            Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue
            Dim OutputType As ReadDiskOutputTypes = ComboOutputType.SelectedValue
            Dim IsFluxOutput = (OutputType = ReadDiskOutputTypes.RAW)
            Dim HasStagingFile As Boolean = Not String.IsNullOrEmpty(_StagingFilePath)
            Dim HasInputFile As Boolean = Not String.IsNullOrEmpty(TextBoxFileName.Text)

            ButtonProcess.Enabled = ImageParams.Format <> FloppyDiskFormat.FloppyUnknown AndAlso
                    (Process.IsRunning Or Not HasStagingFile) AndAlso
                    (Not IsFluxOutput Or HasInputFile)

        End Sub

        Private Sub RefreshTitleBarText()
            If Not Process.State = ConsoleProcessRunner.ProcessStateEnum.Completed Then
                Exit Sub
            End If

            If _StagingFileIsFlux Then
                Exit Sub
            End If

            SetTiltebarText()
        End Sub

        Private Sub ResetTrackGrid(Optional ResetSelected As Boolean = True)
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            Dim SideCount As Byte
            Dim FormatMediaType As FloppyMediaType

            If DiskParams.IsNonImage Then
                SideCount = 2
                FormatMediaType = FloppyMediaType.MediaUnknown
            Else
                SideCount = DiskParams.BPBParams.NumberOfHeads
                FormatMediaType = DiskParams.MediaType
            End If

            Dim TrackCount As UShort
            If Opt.Type = FloppyMediaType.MediaUnknown Then
                TrackCount = If(FormatMediaType = FloppyMediaType.Media525DoubleDensity, GreaseweazleSettings.MAX_TRACKS_525DD, GreaseweazleSettings.MAX_TRACKS)
            Else
                TrackCount = Opt.Tracks
            End If

            TrackCount = Math.Max(TrackCount, Opt.Tracks)

            GridReset(TrackCount, SideCount, ResetSelected)
        End Sub

        Private Sub SetTiltebarText()
            Dim Text = My.Resources.Label_ReadDisk

            If String.IsNullOrEmpty(_StagingFilePath) Then
                Me.Text = Text
                Exit Sub
            End If

            If String.IsNullOrEmpty(TextBoxFileName.Text.Trim) Then
                Me.Text = Text
                Exit Sub
            End If

            Dim DisplayFileName = ""

            If _StagingFileIsFlux Then
                Dim ParentFolder As String = IO.Path.GetFileName(IO.Directory.GetParent(_StagingFilePath).FullName)
                DisplayFileName = IO.Path.Combine(ParentFolder, "*.raw")
            Else
                DisplayFileName = GetNewFileName()
            End If

            Me.Text = Text & " - " & DisplayFileName
        End Sub

        Private Sub StageProcessedFile()
            Dim OutputType As ReadDiskOutputTypes = ComboOutputType.SelectedValue

            _StagingFilePath = _ProcessingFilePath
            _StagingFileIsFlux = (OutputType = ReadDiskOutputTypes.RAW)
            _ProcessingFilePath = ""
            SetTiltebarText()
            PopulateOutputTypes(ComboOutputType.SelectedValue)
        End Sub
        Private Sub ToggleRootFolderControls()
            Dim HasStagingFile As Boolean = Not String.IsNullOrEmpty(_StagingFilePath)
            Dim OutputType As ReadDiskOutputTypes = ComboOutputType.SelectedValue
            Dim IsFluxOutput = (OutputType = ReadDiskOutputTypes.RAW)
            Dim IsRunning As Boolean = Process.IsRunning

            TextBoxFluxFolder.Enabled = Not IsRunning AndAlso IsFluxOutput AndAlso Not HasStagingFile
            TextBoxFileName.Enabled = (Not IsRunning OrElse (IsRunning AndAlso Not IsFluxOutput))

            ButtonBrowse.Enabled = Not IsRunning AndAlso IsFluxOutput AndAlso Not HasStagingFile

            LaelFileName.Text = If(Not HasStagingFile And IsFluxOutput, My.Resources.Label_FolderName, My.Resources.Label_FileName)
        End Sub
#Region "Events"
        Private Sub ButtonBrowse_Click(sender As Object, e As EventArgs) Handles ButtonBrowse.Click
            FluxFolderBrowse()
        End Sub

        Private Sub ButtonDetect_Click(sender As Object, e As EventArgs) Handles ButtonDetect.Click
            DoFormatDetection()
        End Sub

        Private Sub ButtonDiscard_Click(sender As Object, e As EventArgs) Handles ButtonDiscard.Click
            ClearStagedImage(False)
            RefreshButtonState(True)
        End Sub
        Private Sub ButtonImport_Click(sender As Object, e As EventArgs) Handles ButtonImport.Click
            ProcessImport()
        End Sub

        Private Sub ButtonKeep_Click(sender As Object, e As EventArgs) Handles ButtonKeep.Click
            CashFilenameTemplate()
            ClearStagedImage(True)
            RefreshButtonState(True)
            TextBoxFileName.Text = _CachedFileNameTemplate
        End Sub

        Private Sub ButtonOk_Click(sender As Object, e As EventArgs) Handles ButtonOk.Click
            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            _CachedDriveId = Opt.Id
            CashFilenameTemplate()
            _OutputFilePath = _StagingFilePath
        End Sub

        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If CancelProcessIfRunning() Then
                Exit Sub
            End If

            ProcessImage()
        End Sub

        Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
            Reset(TextBoxConsole)
        End Sub

        Private Sub ComboExtensions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboExtensions.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboExtensionsNoEvent Then
                Exit Sub
            End If

            RefreshPreferredExensions()
            RefreshTitleBarText()
        End Sub

        Private Sub ComboImageDrives_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageDrives.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            Dim Opt As DriveOption = ComboImageDrives.SelectedValue

            _ComboImageFormatNoEvent = True
            PopulateImageFormats(ComboImageFormat, Opt)
            _ComboImageFormatNoEvent = False

            ResetTrackGrid()
            RefreshButtonState(False)
            LabelWarning.Visible = Not CheckCompatibility()
        End Sub

        Private Sub ComboImageFormat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageFormat.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboImageFormatNoEvent Then
                Exit Sub
            End If

            Dim Opt As DriveOption = ComboImageDrives.SelectedValue
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            If Opt.Id <> "" Then
                Opt.SelectedFormat = DiskParams.Format
            End If

            PopulateFileExtensions()
            ResetTrackGrid()
            RefreshButtonState(True)
            LabelWarning.Visible = Not CheckCompatibility()
        End Sub
        Private Sub ComboOutputType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboOutputType.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            PopulateFileExtensions()
            RefreshButtonState(False)
        End Sub

        Private Sub Process_DataReceived(data As String) Handles Process.ErrorDataReceived, Process.OutputDataReceived
            ProcessOutputLine(data, _ProcessingAction)
        End Sub

        Private Sub Process_ProcessStateChanged(State As ConsoleProcessRunner.ProcessStateEnum) Handles Process.ProcessStateChanged
            Select Case State
                Case ConsoleProcessRunner.ProcessStateEnum.Aborted
                    ClearProcessedFile()
                    _TrackStatus.UpdateTrackStatusAborted()

                Case ConsoleProcessRunner.ProcessStateEnum.Completed
                    StageProcessedFile()
                    _TrackStatus.UpdateTrackStatusComplete(_DoubleStep)

                Case ConsoleProcessRunner.ProcessStateEnum.Error
                    ClearProcessedFile()
                    _TrackStatus.UpdateTrackStatusError()
            End Select

            RefreshButtonState(False)
        End Sub

        Private Sub TextBoxFileName_TextChanged(sender As Object, e As EventArgs) Handles TextBoxFileName.TextChanged
            RefreshImportButtonState()
            RefreshProcessButtonState()
            RefreshTitleBarText()
        End Sub

        Private Sub TextBoxFileName_Validating(sender As Object, e As CancelEventArgs) Handles TextBoxFileName.Validating
            Dim tb As TextBox = DirectCast(sender, TextBox)
            tb.Text = SanitizeFileNamePreservePlaceholders(tb.Text)

            RefreshImportButtonState()
            RefreshProcessButtonState()
            RefreshTitleBarText()
        End Sub

        Private Sub TextBoxFluxFolder_Click(sender As Object, e As EventArgs) Handles TextBoxFluxFolder.Click
            If TextBoxFluxFolder.Enabled AndAlso String.IsNullOrEmpty(TextBoxFluxFolder.Text) Then
                FluxFolderBrowse()
            End If
        End Sub
#End Region
    End Class
End Namespace
