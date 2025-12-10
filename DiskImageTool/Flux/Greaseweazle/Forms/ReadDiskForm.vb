Imports System.ComponentModel
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Public Class ReadDiskForm
        Inherits BaseFluxForm

        Private WithEvents ButtonBrowse As Button
        Private WithEvents ButtonConvert As Button
        Private WithEvents ButtonDetect As Button
        Private WithEvents ButtonDiscard As Button
        Private WithEvents ButtonImport As Button
        Private WithEvents ButtonPreview As Button
        Private WithEvents ButtonProcess As Button
        Private WithEvents ButtonReset As Button
        Private WithEvents CheckBoxSaveLog As CheckBox
        Private WithEvents CheckBoxSelect As CheckBox
        Private WithEvents ComboDrives As ComboBox
        Private WithEvents ComboExtensions As ComboBox
        Private WithEvents ComboImageFormat As ComboBox
        Private WithEvents ComboOutputType As ComboBox
        Private WithEvents TextBoxFileName As TextBox
        Private WithEvents TextBoxFluxFolder As TextBox
        Private Const DEFAULT_RAW_FILE_NAME As String = "track"
        Private Shared _CachedFileNameTemplate As String = ""
        Private ReadOnly _HelpProvider1 As HelpProvider
        Private ReadOnly _Initialized As Boolean = False
        Private ReadOnly _UserState As Settings.UserStateFlux
        Private _ComboExtensionsNoEvent As Boolean = False
        Private _ComboImageFormatNoEvent As Boolean = False
        Private _ComboOutputTypeNoEvent As Boolean = False
        Private _FileOverwriteMode As Boolean = False
        Private _NewFileName As String = ""
        Private _NewFilePath As String = ""
        Private _NumericRetries As NumericUpDown
        Private _NumericRevs As NumericUpDown
        Private _NumericSeekRetries As NumericUpDown
        Private _OutputDoubleStep As Boolean = False
        Private _OutputFilePath As String = ""
        Private _SelectedOption As DriveOption
        Private LabelDrive As Label
        Private LabelFileName As Label
        Private LabelFluxFolder As Label
        Private LabelImageFormat As Label
        Private LabelOutputType As Label
        Private LabelRetries As Label
        Private LabelRevs As Label
        Private LabelSeekRetries As Label
        Private LabelWarning As Label
        Public Event ImportProcess(File As String, NewFilename As String)

        Public Sub New()
            MyBase.New(Settings.LogFileName)
            InitializeControls()

            _UserState = App.UserState.Flux
            _HelpProvider1 = New HelpProvider
            TrackStatus = New TrackStatus()

            ButtonOk.DialogResult = DialogResult.None
            Me.HelpButton = True
            Me.Text = My.Resources.Label_ReadDisk

            IntitializeHelp()

            _SelectedOption = PopulateDrives(ComboDrives, FloppyDriveType.DriveUnknown, GetSelectedDeviceState.DriveId)
            PopulateImageFormats()
            InitializeImage()

            TextBoxFileName.Text = _CachedFileNameTemplate
            SetFluxFolder(App.AppSettings.Greaseweazle.FluxRootPath)

            _NumericRevs.Value = Settings.DefaultRevs
            _NumericRetries.Value = CommandLineBuilder.DEFAULT_RETRIES
            _NumericSeekRetries.Value = CommandLineBuilder.DEFAULT_SEEK_RETRIES

            CheckBoxSaveLog.Checked = GetSelectedDeviceState.SaveLog

            _Initialized = True
        End Sub

        Public ReadOnly Property NewFileName As String
            Get
                Return _NewFileName
            End Get
        End Property

        Public ReadOnly Property NewFilePath As String
            Get
                Return _NewFilePath
            End Get
        End Property

        Public Overrides Sub SaveLog(RemovePath As Boolean, Optional InitialDirectory As String = "")
            If String.IsNullOrEmpty(InitialDirectory) Then
                If Not String.IsNullOrEmpty(_OutputFilePath) AndAlso CheckIsFluxOutput() Then
                    InitialDirectory = IO.Path.GetDirectoryName(_OutputFilePath)
                End If
            End If

            MyBase.SaveLog(RemovePath, InitialDirectory)
        End Sub

        Public Sub SetFluxFolder(Path As String)
            If Path <> "" AndAlso Not IO.Directory.Exists(Path) Then
                Path = ""
            End If

            TextBoxFluxFolder.Text = Path
            App.AppSettings.Greaseweazle.FluxRootPath = Path
        End Sub

        Protected Overrides Sub OnAfterBaseFormClosing(e As FormClosingEventArgs)
            If Me.DialogResult = DialogResult.Cancel OrElse Me.DialogResult = DialogResult.None Then
                ClearOutputFile(True)
                _NewFilePath = ""
                _NewFileName = ""
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
            Dim Opt As DriveOption = _SelectedOption
            Dim DiskParams = SelectedDiskParams()

            If Opt.Type = FloppyDriveType.DriveUnknown Then
                Return True
            End If

            If Not DiskParams.HasValue OrElse DiskParams.Value.IsNonImage Then
                Return True
            End If


            Dim FloppyType = GreaseweazleFindCompatibleDriveType(DiskParams.Value, Opt.Type)

            Return FloppyType = Opt.Type
        End Function

        Private Function CheckFileNameEntered() As Boolean
            Dim IsFluxOutput = CheckIsFluxOutput()

            If IsFluxOutput AndAlso String.IsNullOrEmpty(TextBoxFluxFolder.Text.Trim) Then
                MsgBox(My.Resources.Dialog_SelectRootFolder, MsgBoxStyle.Exclamation)
                ButtonBrowse.Focus()
                Return False
            End If


            If String.IsNullOrEmpty(TextBoxFileName.Text.Trim) Then
                Dim Msg As String
                If IsFluxOutput Then
                    Msg = My.Resources.Dialog_EnterFolderName
                Else
                    Msg = My.Resources.Dialog_EnterFileName
                End If
                MsgBox(Msg, MsgBoxStyle.Exclamation)
                TextBoxFileName.Focus()
                Return False
            Else
                Return True
            End If
        End Function

        Private Function CheckIsFluxOutput() As Boolean
            Dim OutputType As ReadDiskOutputTypes = ComboOutputType.SelectedValue
            Return (OutputType = ReadDiskOutputTypes.RAW)
        End Function

        Private Function CheckRawFolderExists(FilePath As String) As (Result As Boolean, Overwritemode As Boolean)
            Dim FolderName = IO.Path.GetDirectoryName(FilePath)

            If GetFirstRawInFolder(FolderName) IsNot Nothing Then
                Dim Msg = String.Format(My.Resources.Dialog_ImageSetExists, FolderName)
                If MsgBox(Msg, MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel + vbDefaultButton2) <> MsgBoxResult.Ok Then
                    Return (False, False)
                End If
                Return (True, True)
            End If

            Return (True, False)
        End Function

        Private Sub ClearOutputFile(Delete As Boolean)
            If String.IsNullOrEmpty(_OutputFilePath) Then
                Exit Sub
            End If

            If Delete AndAlso Not _FileOverwriteMode Then
                If CheckIsFluxOutput() Then
                    Dim PathName = IO.Path.GetDirectoryName(_OutputFilePath)
                    DeleteFilesAndFolderIfEmpty(PathName, "*.raw", Settings.LogFileName)
                Else
                    DeleteTempFileIfExists(_OutputFilePath)
                End If
            End If

            _OutputFilePath = ""
            _OutputDoubleStep = False
            _FileOverwriteMode = False

            HideSelection(False)
        End Sub

        Private Sub ClearProcessedImage(DeleteOutputFile As Boolean, RefreshState As Boolean)
            _FileOverwriteMode = False

            TextBoxConsole.Clear()
            ClearOutputFile(DeleteOutputFile)
            ClearStatusBar()
            ResetTrackGrid()
            TrackStatus.Clear()
            SetTiltebarText()
            CheckBoxSelect.Checked = False

            If Not DeleteOutputFile Then
                CashFilenameTemplate()
                TextBoxFileName.Text = _CachedFileNameTemplate
            ElseIf RefreshState Then
                TextBoxFileName.Text = ""
            End If

            If RefreshState Then
                For Each Opt As DriveOption In ComboDrives.Items
                    Opt.ResetFormats()
                Next
                PopulateImageFormats()
                RefreshFormState()
            End If
        End Sub

        Private Sub CloseForm(NewFilePath As String, NewFileName As String)
            CashFilenameTemplate()

            _NewFilePath = NewFilePath
            _NewFileName = NewFileName
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub

        Private Sub ConvertImage()
            Dim Response = ConvertFluxImage(Me, _OutputFilePath, True, Nothing, True)

            If Response.Result = DialogResult.OK Then
                CloseForm(Response.OutputFile, Response.NewFileName)

            ElseIf Response.Result = DialogResult.Retry Then
                ProcessImport(Response.OutputFile, Response.NewFileName)
            End If
        End Sub

        Private Sub DoFormatDetection()
            Dim Opt As DriveOption = _SelectedOption

            Dim ImageFormat As FloppyDiskFormat?

            If Opt.Id = "" Then
                ImageFormat = Nothing
            Else
                ImageFormat = ReadImageFormat(Opt.Id)
                Opt.SelectedFormat = ImageFormat
                Opt.DetectedFormat = ImageFormat
            End If

            SharedLib.PopulateImageFormats(ComboImageFormat, ImageFormat, ImageFormat, True)
        End Sub

        Private Sub FileNameChangePostProcess()
            RefreshImportButtonState()
            RefreshProcessButtonState()
            RefreshTitleBarText()
        End Sub

        Private Sub FluxFolderBrowse()
            Dim FolderName = BrowseFolder(TextBoxFluxFolder.Text)
            If FolderName <> "" Then
                SetFluxFolder(FolderName)
            End If
        End Sub

        Private Function GetNewFileName(FilePath As String) As String
            If String.IsNullOrEmpty(FilePath) Then
                Return ""
            End If

            Dim Extension As String

            If ComboExtensions.Enabled AndAlso ComboExtensions.SelectedIndex > -1 Then
                Dim Item As FileExtensionItem = ComboExtensions.SelectedValue
                Extension = Item.Extension
            Else
                Extension = IO.Path.GetExtension(FilePath)
            End If

            Dim NewFileName As String = TextBoxFileName.Text

            If ContainsPlaceholder(NewFileName) Then
                NewFileName = StripAngleBrackets(NewFileName)
            End If

            NewFileName &= Extension

            Return NewFileName
        End Function

        Private Function GetOutputFilePaths() As (FilePath As String, LogFilePath As String, IsFlux As Boolean)
            Dim Response As (FilePath As String, LogFilePath As String, IsFlux As Boolean)
            Response.LogFilePath = ""

            Dim OutputType As ReadDiskOutputTypes = ComboOutputType.SelectedValue
            Dim IsFluxOutput = (OutputType = ReadDiskOutputTypes.RAW)

            If IsFluxOutput Then
                Dim FileName = TextBoxFileName.Text.Trim
                If ContainsPlaceholder(FileName) Then
                    FileName = StripAngleBrackets(FileName)
                End If
                Dim Pathname = IO.Path.Combine(TextBoxFluxFolder.Text, FileName)
                Dim FilePath = IO.Path.Combine(Pathname, DEFAULT_RAW_FILE_NAME & "00.0.raw")
                Try
                    IO.Directory.CreateDirectory(Pathname)
                    Response.FilePath = FilePath
                Catch ex As Exception
                    MsgBox(My.Resources.Dialog_ParentFolderCreationError, MsgBoxStyle.Critical)
                    Response.FilePath = ""
                End Try

                Response.IsFlux = True
                If CheckBoxSaveLog.Checked Then
                    Response.LogFilePath = IO.Path.Combine(Pathname, Settings.LogFileName)
                End If

            Else
                Dim DiskParams = SelectedDiskParams()

                If Not DiskParams.HasValue OrElse Not DiskParams.Value.IsStandard Then
                    OutputType = ReadDiskOutputTypes.HFE
                End If

                Response.FilePath = GenerateOutputFile(ReadDisktOutputTypeFileExt(OutputType))
                Response.IsFlux = False
            End If

            Return Response
        End Function

        Private Function GetSelectedDeviceState() As Settings.UserStateFluxReadDevice
            Return _UserState.Read.Device(IDevice.FluxDevice.Greaseweazle)
        End Function

        Private Sub HideSelection(Value As Boolean)
            TableSide0.HideSelection = Value
            TableSide1.HideSelection = Value
        End Sub

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

            LabelDrive = New Label With {
                .Text = My.Resources.Label_Drive,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            ComboDrives = New ComboBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Width = 180
            }

            LabelFileName = New Label With {
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

            LabelImageFormat = New Label With {
                .Text = My.Resources.Label_Format,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            ComboImageFormat = New ComboBox With {
                .Anchor = AnchorStyles.Left,
                .Width = 200
            }

            LabelOutputType = New Label With {
                .Text = My.Resources.Label_OutputType,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            ComboOutputType = New ComboBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Width = 175
            }

            CheckBoxSaveLog = New CheckBox With {
                .Text = My.Resources.Label_SaveLog,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            CheckBoxSelect = New CheckBox With {
                .Text = My.Resources.Label_SelectTracks,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            LabelSeekRetries = New Label With {
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

            LabelRetries = New Label With {
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

            LabelRevs = New Label With {
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

            ButtonPreview = New Button With {
                .Margin = New Padding(3, 0, 3, 3),
                .Text = My.Resources.Label_Preview,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonProcess = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Write,
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

            BumpTabIndexes(PanelButtonsRight, 1)
            PanelButtonsRight.Controls.Add(ButtonImport)

            ButtonDetect = New Button With {
                .Width = 75,
                .Margin = New Padding(3, 3, 3, 3),
                .Text = My.Resources.Label_Detect
            }

            ButtonContainer.Controls.Add(ButtonPreview)
            ButtonContainer.Controls.Add(ButtonProcess)
            ButtonContainer.Controls.Add(ButtonConvert)
            ButtonContainer.Controls.Add(ButtonDiscard)


            ButtonOk.Text = My.Resources.Label_ImportClose
            ButtonOk.Visible = True

            Dim PanelLabel As New Panel With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Height = 20
            }

            LabelWarning = New Label With {
                .Text = "",
                .AutoSize = True,
                .TextAlign = ContentAlignment.TopLeft,
                .Dock = DockStyle.Left,
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
                .Controls.Add(LabelFileName, 0, Row)
                .Controls.Add(TextBoxFileName, 1, Row)
                .SetColumnSpan(TextBoxFileName, 6)

                .Controls.Add(ComboExtensions, 7, Row)

                Row = 1
                .Controls.Add(LabelFluxFolder, 0, Row)
                .Controls.Add(TextBoxFluxFolder, 1, Row)
                .SetColumnSpan(TextBoxFluxFolder, 5)
                .Controls.Add(ButtonBrowse, 6, Row)
                .SetColumnSpan(ButtonBrowse, 2)

                .Controls.Add(CheckBoxSaveLog, 8, Row)

                Row = 2
                .Controls.Add(LabelDrive, 0, Row)
                .Controls.Add(ComboDrives, 1, Row)

                .Controls.Add(LabelOutputType, 2, Row)
                .Controls.Add(ComboOutputType, 3, Row)
                .SetColumnSpan(ComboOutputType, 5)

                .Controls.Add(CheckBoxSelect, 8, Row)

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
                .Controls.Add(PanelLabel, 1, Row)
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
            PopulateOutputTypes(GetSelectedDeviceState.OutputType)
            PopulateFileExtensions()
            ResetTrackGrid()
            ClearStatusBar()
            RefreshFormState()
            SetTiltebarText()
        End Sub

        Private Sub IntitializeHelp()
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Retries, LabelRetries, _NumericRetries)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_SeekRetries, LabelSeekRetries, _NumericSeekRetries)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Revs, LabelRevs, _NumericRevs)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_DeviceReset, ButtonReset)
            SetHelpString(My.Resources.HelpStrings.Flux_SaveLog, ButtonSaveLog)
            SetHelpString(My.Resources.HelpStrings.Flux_Detect, ButtonDetect)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_Drives, LabelDrive, ComboDrives)
            SetHelpString(My.Resources.HelpStrings.Flux_Format, LabelImageFormat, ComboImageFormat)
            SetHelpString(My.Resources.HelpStrings.Flux_ImageType, LabelOutputType, ComboOutputType)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_ReadFilename, LabelFileName, TextBoxFileName)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_FileExt, ComboExtensions)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_SaveLog, CheckBoxSaveLog)
            SetHelpString(My.Resources.HelpStrings.Flux_Discard, ButtonDiscard)
            SetHelpString(My.Resources.HelpStrings.Flux_Read, ButtonProcess)
            SetHelpString(My.Resources.HelpStrings.Flux_Convert, ButtonConvert)
            SetHelpString(My.Resources.HelpStrings.Greaseweazle_RootFolder, LabelFluxFolder, TextBoxFluxFolder)

            IntitializeHelpImportButtons(False)
        End Sub

        Private Sub IntitializeHelpImportButtons(IsFluxOutput As Boolean)
            If IsFluxOutput Then
                SetHelpString(My.Resources.HelpStrings.Flux_Save, ButtonImport)
                SetHelpString(My.Resources.HelpStrings.Flux_SaveClose, ButtonOk)
            Else
                SetHelpString(My.Resources.HelpStrings.Flux_Import, ButtonImport)
                SetHelpString(My.Resources.HelpStrings.Flux_ImportClose, ButtonOk)
            End If
        End Sub

        Private Sub PopulateFileExtensions()
            _ComboExtensionsNoEvent = True

            Dim OutputType As ReadDiskOutputTypes = ComboOutputType.SelectedValue
            Dim IsBitstreamOutput As Boolean = (OutputType = ReadDiskOutputTypes.HFE Or OutputType = ReadDiskOutputTypes.RAW)

            If IsBitstreamOutput Then
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
                End With
            Else
                SharedLib.PopulateFileExtensions(ComboExtensions, SelectedDiskFormat())
            End If

            ComboExtensions.Enabled = (ComboExtensions.Items.Count > 1)

            _ComboExtensionsNoEvent = False
        End Sub

        Private Sub PopulateImageFormats()
            _ComboImageFormatNoEvent = True
            SharedLib.PopulateImageFormats(ComboImageFormat, _SelectedOption, True)
            _ComboImageFormatNoEvent = False
        End Sub

        Private Sub PopulateOutputTypes(Optional CurrentValue As ReadDiskOutputTypes? = Nothing)
            _ComboOutputTypeNoEvent = True

            Dim DiskParams = SelectedDiskParams()

            Dim DriveList As New List(Of KeyValuePair(Of String, ReadDiskOutputTypes))
            For Each OutputType As ReadDiskOutputTypes In [Enum].GetValues(GetType(ReadDiskOutputTypes))
                If DiskParams.HasValue Then
                    If DiskParams.Value.IsNonImage AndAlso OutputType <> ReadDiskOutputTypes.RAW Then
                        Continue For
                    ElseIf Not DiskParams.Value.IsStandard AndAlso OutputType = ReadDiskOutputTypes.IMA Then
                        Continue For
                    End If
                End If
                DriveList.Add(New KeyValuePair(Of String, ReadDiskOutputTypes)(
                    ReadDiskOutputTypeDescription(OutputType), OutputType)
                )
            Next

            InitializeCombo(ComboOutputType, DriveList, CurrentValue)

            If ComboOutputType.Items.Count > 0 AndAlso ComboOutputType.SelectedIndex = -1 Then
                ComboOutputType.SelectedIndex = 0
            End If

            ComboOutputType.Enabled = (ComboOutputType.Items.Count > 1)

            _ComboOutputTypeNoEvent = False
        End Sub

        Private Sub PreviewImage()
            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)
            Dim IsFluxOutput = CheckIsFluxOutput()

            If HasOutputfile AndAlso Not IsFluxOutput Then
                Dim ImageData = New ImageData(_OutputFilePath)

                Dim Caption As String = TextBoxFileName.Text

                If Not ImagePreview.Display(ImageData, Caption, Me) Then
                    MsgBox(My.Resources.Dialog_ImagePreviewFail, MsgBoxStyle.Exclamation)
                End If
            Else
                Dim DiskParams = SelectedDiskParams()
                Dim Opt As DriveOption = _SelectedOption

                If Not DiskParams.HasValue OrElse Opt.Id = "" Then
                    Exit Sub
                End If

                Dim Response = ReadFirstTrack(Opt.Id, True, DiskParams.Value)

                If Not Response.Result Then
                    MsgBox(My.Resources.Dialog_ImagePreviewFail, MsgBoxStyle.Exclamation)
                End If

                Dim Caption As String = TextBoxFileName.Text

                If Not ImagePreview.Display(Response.FileName, DiskParams.Value, Caption, Me) Then
                    MsgBox(My.Resources.Dialog_ImagePreviewFail, MsgBoxStyle.Exclamation)
                End If

                DeleteTempFileIfExists(Response.FileName)
            End If
        End Sub

        Private Sub ProcessImage()
            Dim DiskParams = SelectedDiskParams()

            If Not DiskParams.HasValue Then
                Exit Sub
            End If

            Dim Opt As DriveOption = _SelectedOption

            If Opt.Id = "" Then
                Exit Sub
            End If

            Dim OverwriteMode As Boolean = False

            Dim Response = GetOutputFilePaths()
            If Response.FilePath = "" Then
                Exit Sub
            ElseIf Response.IsFlux Then
                If Not _FileOverwriteMode Then
                    Dim CheckResponse = CheckRawFolderExists(Response.FilePath)
                    If Not CheckResponse.Result Then
                        Exit Sub
                    End If
                    OverwriteMode = CheckResponse.Overwritemode
                End If
            End If

            ClearProcessedImage(True, False)

            Dim OutputType As ReadDiskOutputTypes = ComboOutputType.SelectedValue

            _OutputFilePath = Response.FilePath
            _OutputDoubleStep = UseDoubleStep(Opt.Type, DiskParams.Value.Format)
            _FileOverwriteMode = OverwriteMode

            Dim Arguments = GenerateCommandLineRead(Response.FilePath,
                                                    Opt,
                                                    DiskParams.Value,
                                                    OutputType,
                                                    _OutputDoubleStep,
                                                    _NumericRetries.Value,
                                                    _NumericSeekRetries.Value,
                                                    _NumericRevs.Value)

            If Not String.IsNullOrEmpty(Response.LogFilePath) Then
                DeleteFileIfExists(Response.LogFilePath)
            End If

            Process.StartAsync(Settings.AppPath, Arguments, logFile:=Response.LogFilePath)
        End Sub

        Private Sub ProcessImport(OutputFile As String, NewFileName As String)
            If Not String.IsNullOrEmpty(OutputFile) Then
                RaiseEvent ImportProcess(OutputFile, NewFileName)
            End If

            ClearProcessedImage(False, True)
        End Sub

        Private Sub ProcessOutputLine(line As String)
            If TextBoxConsole.Text.Length > 0 Then
                TextBoxConsole.AppendText(Environment.NewLine)
            End If
            TextBoxConsole.AppendText(line)

            TrackStatus.ProcessOutputLineRead(line, ActionTypeEnum.Read, _OutputDoubleStep)

            If TrackStatus.Failed Then
                Process.Cancel()
            End If
        End Sub

        Private Function ReadImageFormat(DriveId As String) As DiskImage.FloppyDiskFormat?
            Dim Response = ReadFirstTrack(DriveId, False)

            TextBoxConsole.Text = Response.Output

            If Not Response.Result Then
                Return Nothing
            End If

            Return DetectImageFormat(Response.FileName, True)
        End Function

        Private Sub RefreshFormState()
            Dim DiskParams = SelectedDiskParams()
            Dim IsFluxOutput = CheckIsFluxOutput()
            Dim Opt As DriveOption = _SelectedOption

            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)
            Dim IsRunning As Boolean = Process.IsRunning
            Dim IsIdle As Boolean = Not IsRunning

            Dim CanChangeSettings As Boolean = IsIdle AndAlso Not HasOutputfile
            Dim CanConvert As Boolean = IsIdle AndAlso HasOutputfile AndAlso IsFluxOutput
            Dim DriveSelected As Boolean = Not String.IsNullOrEmpty(Opt.Id)
            Dim SelectMode As Boolean = CanConvert AndAlso CheckBoxSelect.Checked

            ComboImageFormat.Enabled = CanChangeSettings AndAlso DriveSelected
            ComboDrives.Enabled = CanChangeSettings OrElse SelectMode
            ComboOutputType.Enabled = CanChangeSettings AndAlso ComboOutputType.Items.Count > 1

            _NumericRevs.Enabled = CanChangeSettings OrElse SelectMode
            _NumericRetries.Enabled = CanChangeSettings OrElse SelectMode
            _NumericSeekRetries.Enabled = CanChangeSettings OrElse SelectMode

            ButtonSaveLog.Enabled = IsIdle AndAlso TextBoxConsole.TextLength > 0
            ButtonReset.Enabled = IsIdle

            ButtonConvert.Enabled = CanConvert
            ButtonConvert.Visible = IsFluxOutput

            ButtonDiscard.Enabled = IsIdle AndAlso HasOutputfile

            CheckBoxSelect.Enabled = CanConvert

            ButtonDetect.Enabled = CanChangeSettings AndAlso DriveSelected

            ButtonCancel.Text = If(IsRunning OrElse HasOutputfile, WithoutHotkey(My.Resources.Menu_Cancel), WithoutHotkey(My.Resources.Menu_Close))

            ButtonPreview.Enabled = IsIdle AndAlso DriveSelected AndAlso DiskParams.HasValue AndAlso Not DiskParams.Value.IsNonImage

            RefreshProcessButtonState()
            RefreshImportButtonState()
            ToggleRootFolderControls()
        End Sub

        Private Sub RefreshImportButtonState()
            Dim IsFluxOutput = CheckIsFluxOutput()

            Dim EnableImport As Boolean = Not Process.IsRunning AndAlso Not String.IsNullOrEmpty(_OutputFilePath)

            ButtonOk.Enabled = EnableImport
            ButtonOk.Text = If(IsFluxOutput, My.Resources.Label_SaveAndClose, My.Resources.Label_ImportClose)

            ButtonImport.Enabled = EnableImport
            ButtonImport.Text = If(IsFluxOutput, WithoutHotkey(My.Resources.Menu_Save), My.Resources.Label_Import)

            IntitializeHelpImportButtons(IsFluxOutput)
        End Sub

        Private Sub RefreshPreferredExensions()
            Dim Item As FileExtensionItem = ComboExtensions.SelectedValue

            If Not Item.Format.HasValue Then
                Exit Sub
            End If

            Dim Format = SelectedDiskFormat()

            If Not Format.Value Then
                Exit Sub
            End If

            If Item.Format.Value <> Format.Value Then
                App.UserState.RemovePreferredExtension(Format.Value)
            End If

            App.UserState.SetPreferredExtension(Item.Format.Value, Item.Extension)
        End Sub

        Private Sub RefreshProcessButtonState()
            If Process.IsRunning Then
                ButtonProcess.Text = My.Resources.Label_Abort
                ButtonProcess.Enabled = True
            Else
                Dim DiskParams = SelectedDiskParams()
                Dim HasOutputFile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)
                Dim TracksSelected As Boolean = CheckBoxSelect.Checked AndAlso (TableSide0.SelectedTracks.Count + TableSide1.SelectedTracks.Count > 0)

                ButtonProcess.Text = My.Resources.Label_Read
                ButtonProcess.Enabled = DiskParams.HasValue AndAlso (Not HasOutputFile OrElse TracksSelected)
            End If
        End Sub

        Private Sub RefreshTitleBarText()
            If Not Process.State = ConsoleProcessRunner.ProcessStateEnum.Completed Then
                Exit Sub
            End If

            SetTiltebarText()
        End Sub

        Private Sub RefreshTrackState(PrevOption As DriveOption, CurrentOption As DriveOption)
            Dim DiskParams = SelectedDiskParams()
            Dim PrevDoubleStep = UseDoubleStep(PrevOption.Type, DiskParams.Value.Format)
            Dim Doublestep = UseDoubleStep(CurrentOption.Type, DiskParams.Value.Format)
            If PrevDoubleStep <> Doublestep Then
                Dim State = GetState(PrevDoubleStep)
                SetState(State, Doublestep)
            End If
        End Sub

        Private Sub RefreshWarningLabel()
            Dim Format = SelectedDiskFormat()

            If Not CheckCompatibility() Then
                LabelWarning.Text = My.Resources.Message_ImageFormatWarning
                LabelWarning.ForeColor = Color.Red
                LabelWarning.Visible = True

            ElseIf Format.HasValue AndAlso UseDoubleStep(_SelectedOption.Type, Format.Value) Then
                LabelWarning.Text = My.Resources.Label_DoubleStep
                LabelWarning.ForeColor = Color.Blue
                LabelWarning.Visible = True

            Else
                LabelWarning.Visible = False
                LabelWarning.Text = ""
            End If
        End Sub

        Private Sub ReprocessImage()
            Dim DiskParams = SelectedDiskParams()

            If Not DiskParams.HasValue Then
                Exit Sub
            End If

            Dim Opt As DriveOption = _SelectedOption

            If Opt.Id = "" Then
                Exit Sub
            End If

            Dim OutputType As ReadDiskOutputTypes = ComboOutputType.SelectedValue

            TrackStatus.Clear()
            _FileOverwriteMode = True

            Dim TrackRanges As List(Of (StartTrack As UShort, EndTrack As UShort)) = Nothing
            Dim Heads As TrackHeads? = Nothing

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

                TableSide0.ResetSelectedSells()
                TableSide1.ResetSelectedSells()
                HideSelection(True)
            End If

            _OutputDoubleStep = UseDoubleStep(Opt.Type, DiskParams.Value.Format)

            Dim Arguments = GenerateCommandLineRead(_OutputFilePath,
                                                    Opt,
                                                    DiskParams.Value,
                                                    OutputType,
                                                    _OutputDoubleStep,
                                                    _NumericRetries.Value,
                                                    _NumericSeekRetries.Value,
                                                    _NumericRevs.Value,
                                                    TrackRanges,
                                                    Heads)

            Dim LogFilePath = IO.Path.Combine(IO.Path.GetDirectoryName(_OutputFilePath), Settings.LogFileName)

            Process.StartAsync(Settings.AppPath, Arguments, logFile:=LogFilePath)
        End Sub

        Private Sub ResetTrackGrid(Optional ResetSelected As Boolean = True)
            Dim DiskParams = SelectedDiskParams()

            Dim SideCount As Byte
            Dim FormatDriveType As FloppyDriveType

            If Not DiskParams.HasValue OrElse DiskParams.Value.IsNonImage Then
                SideCount = 2
                FormatDriveType = FloppyDriveType.DriveUnknown
            Else
                SideCount = DiskParams.Value.BPBParams.NumberOfHeads
                FormatDriveType = DiskParams.Value.DriveType
            End If

            Dim Opt As DriveOption = _SelectedOption
            Dim TrackCount As UShort

            If Opt.Type = FloppyDriveType.DriveUnknown Then
                TrackCount = If(FormatDriveType = FloppyDriveType.Drive525DoubleDensity, GreaseweazleSettings.MAX_TRACKS_525DD, GreaseweazleSettings.MAX_TRACKS)
            Else
                TrackCount = Opt.Tracks
            End If

            TrackCount = Math.Max(TrackCount, Opt.Tracks)

            GridReset(TrackCount, SideCount, Nothing, ResetSelected)
        End Sub

        Private Function SelectedDiskFormat() As FloppyDiskFormat?
            If TypeOf ComboImageFormat.SelectedValue IsNot FloppyDiskParams Then
                Return Nothing
            End If

            Return DirectCast(ComboImageFormat.SelectedValue, FloppyDiskParams).Format
        End Function

        Private Function SelectedDiskParams() As FloppyDiskParams?
            If TypeOf ComboImageFormat.SelectedValue IsNot FloppyDiskParams Then
                Return Nothing
            End If

            Return DirectCast(ComboImageFormat.SelectedValue, FloppyDiskParams)
        End Function

        Private Sub SetHelpString(HelpString As String, ParamArray ControlArray() As Control)
            For Each Control In ControlArray
                _HelpProvider1.SetHelpString(Control, HelpString.Replace("\t", vbTab))
                _HelpProvider1.SetShowHelp(Control, True)
            Next
        End Sub

        Private Sub SetTiltebarText()
            Dim Text = My.Resources.Label_ReadDisk

            If String.IsNullOrEmpty(_OutputFilePath) Then
                Me.Text = Text
                Exit Sub
            End If

            If String.IsNullOrEmpty(TextBoxFileName.Text.Trim) Then
                Me.Text = Text
                Exit Sub
            End If

            Dim DisplayFileName = ""

            If CheckIsFluxOutput() Then
                Dim ParentFolder As String = IO.Path.GetFileName(IO.Directory.GetParent(_OutputFilePath).FullName)
                DisplayFileName = IO.Path.Combine(ParentFolder, "*.raw")
            Else
                DisplayFileName = GetNewFileName(_OutputFilePath)
            End If

            Me.Text = Text & " - " & DisplayFileName
        End Sub

        Private Sub ToggleRootFolderControls()
            Dim HasOutputFile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)
            Dim IsFluxOutput = CheckIsFluxOutput()
            Dim IsRunning As Boolean = Process.IsRunning

            LabelFileName.Text = If(IsFluxOutput, My.Resources.Label_FolderName, My.Resources.Label_FileName)
            TextBoxFileName.Enabled = Not IsFluxOutput OrElse (Not IsRunning AndAlso Not HasOutputFile)

            TextBoxFluxFolder.Enabled = Not IsRunning AndAlso IsFluxOutput AndAlso Not HasOutputFile
            ButtonBrowse.Enabled = Not IsRunning AndAlso IsFluxOutput AndAlso Not HasOutputFile

            CheckBoxSaveLog.Enabled = Not IsRunning AndAlso IsFluxOutput AndAlso Not HasOutputFile
        End Sub

        Private Function UseDoubleStep(DriveType As FloppyDriveType, Format As FloppyDiskFormat) As Boolean
            Dim ImageParams As FloppyDiskParams = FloppyDiskFormatGetParams(Format)

            Return ImageParams.IsStandard AndAlso ImageParams.DriveType = FloppyDriveType.Drive525DoubleDensity AndAlso DriveType = FloppyDriveType.Drive525HighDensity
        End Function
#Region "Events"
        Private Sub ButtonBrowse_Click(sender As Object, e As EventArgs) Handles ButtonBrowse.Click
            FluxFolderBrowse()
        End Sub

        Private Sub ButtonConvert_Click(sender As Object, e As EventArgs) Handles ButtonConvert.Click
            ConvertImage()
        End Sub

        Private Sub ButtonDetect_Click(sender As Object, e As EventArgs) Handles ButtonDetect.Click
            DoFormatDetection()
        End Sub

        Private Sub ButtonDiscard_Click(sender As Object, e As EventArgs) Handles ButtonDiscard.Click
            ClearProcessedImage(True, True)
        End Sub

        Private Sub ButtonImport_Click(sender As Object, e As EventArgs) Handles ButtonImport.Click
            If Not CheckFileNameEntered() Then
                Exit Sub
            End If

            If CheckIsFluxOutput() Then
                ClearProcessedImage(False, True)
            Else
                If String.IsNullOrEmpty(_OutputFilePath) Then
                    Exit Sub
                End If

                ProcessImport(_OutputFilePath, GetNewFileName(_OutputFilePath))
            End If
        End Sub

        Private Sub ButtonImportAndClose_Click(sender As Object, e As EventArgs) Handles ButtonOk.Click
            If Not CheckFileNameEntered() Then
                Exit Sub
            End If

            If CheckIsFluxOutput() Then
                CloseForm("", "")
            Else
                CloseForm(_OutputFilePath, GetNewFileName(_OutputFilePath))
            End If
        End Sub
        Private Sub ButtonPreview_Click(sender As Object, e As EventArgs) Handles ButtonPreview.Click
            PreviewImage()
        End Sub

        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If CancelProcessIfRunning() Then
                Exit Sub
            End If

            Dim IsFluxOutput = CheckIsFluxOutput()

            If IsFluxOutput AndAlso Not CheckFileNameEntered() Then
                Exit Sub
            End If

            Dim HasOutputFile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)

            If HasOutputFile AndAlso IsFluxOutput Then
                ReprocessImage()
            Else
                ProcessImage()
            End If
        End Sub

        Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
            Reset(TextBoxConsole)
        End Sub

        Private Sub CheckBoxSaveLog_CheckStateChanged(sender As Object, e As EventArgs) Handles CheckBoxSaveLog.CheckStateChanged
            If Not _Initialized Then
                Exit Sub
            End If

            GetSelectedDeviceState.SaveLog = CheckBoxSaveLog.Checked
        End Sub

        Private Sub CheckBoxSelect_CheckStateChanged(sender As Object, e As EventArgs) Handles CheckBoxSelect.CheckStateChanged
            If Not _Initialized Then
                Exit Sub
            End If

            TableSide0.SelectEnabled = CheckBoxSelect.Checked
            TableSide1.SelectEnabled = CheckBoxSelect.Checked

            RefreshFormState()
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

        Private Sub ComboImageDrives_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboDrives.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            Dim PrevOption = _SelectedOption
            _SelectedOption = ComboDrives.SelectedValue
            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)

            If Not HasOutputfile Then
                PopulateImageFormats()
                ResetTrackGrid()
            Else
                RefreshTrackState(PrevOption, _SelectedOption)
            End If

            RefreshFormState()
            RefreshWarningLabel()

            GetSelectedDeviceState.DriveId = _SelectedOption.Id
        End Sub

        Private Sub ComboImageFormat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageFormat.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboImageFormatNoEvent Then
                Exit Sub
            End If

            Dim Opt As DriveOption = _SelectedOption

            If Opt.Id <> "" Then
                Opt.SelectedFormat = SelectedDiskFormat()
            End If

            PopulateOutputTypes(GetSelectedDeviceState.OutputType)
            PopulateFileExtensions()
            ResetTrackGrid()
            RefreshFormState()
            RefreshWarningLabel()
        End Sub

        Private Sub ComboOutputType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboOutputType.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboOutputTypeNoEvent Then
                Exit Sub
            End If

            PopulateFileExtensions()
            RefreshFormState()

            GetSelectedDeviceState.OutputType = ComboOutputType.SelectedValue
        End Sub

        Private Sub Process_DataReceived(data As String) Handles Process.ErrorDataReceived, Process.OutputDataReceived
            ProcessOutputLine(data)
        End Sub

        Private Sub Process_ProcessStateChanged(State As ConsoleProcessRunner.ProcessStateEnum) Handles Process.ProcessStateChanged
            Select Case State
                Case ConsoleProcessRunner.ProcessStateEnum.Aborted
                    ClearOutputFile(True)
                    If Not TrackStatus.Failed Then
                        TrackStatus.UpdateTrackStatusAborted()
                    End If

                Case ConsoleProcessRunner.ProcessStateEnum.Completed
                    If TrackStatus.TrackFound Then
                        TrackStatus.UpdateTrackStatusComplete(_OutputDoubleStep)
                        SetTiltebarText()
                        HideSelection(False)
                    Else
                        ClearOutputFile(True)
                        TrackStatus.UpdateTrackStatusError()
                    End If

                Case ConsoleProcessRunner.ProcessStateEnum.Error
                    ClearOutputFile(True)
                    TrackStatus.UpdateTrackStatusError()
            End Select

            RefreshFormState()
        End Sub

        Private Sub ReadDiskForm_CheckChanged(sender As Object, Checked As Boolean, Side As Byte) Handles Me.CheckChanged
            RefreshFormState()
        End Sub

        Private Sub ReadDiskForm_SelectionChanged(sender As Object, Track As UShort, Side As Byte, Enabled As Boolean) Handles Me.SelectionChanged
            RefreshFormState()
        End Sub

        Private Sub TextBoxFileName_TextChanged(sender As Object, e As EventArgs) Handles TextBoxFileName.TextChanged
            FileNameChangePostProcess()
        End Sub

        Private Sub TextBoxFileName_Validating(sender As Object, e As CancelEventArgs) Handles TextBoxFileName.Validating
            Dim tb As TextBox = DirectCast(sender, TextBox)
            tb.Text = SanitizeFileNamePreservePlaceholders(tb.Text)

            FileNameChangePostProcess()
        End Sub

        Private Sub TextBoxFluxFolder_Click(sender As Object, e As EventArgs) Handles TextBoxFluxFolder.Click
            If TextBoxFluxFolder.Enabled AndAlso String.IsNullOrEmpty(TextBoxFluxFolder.Text) Then
                FluxFolderBrowse()
            End If
        End Sub
#End Region
    End Class
End Namespace
