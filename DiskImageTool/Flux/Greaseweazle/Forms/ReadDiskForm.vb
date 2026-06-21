Imports System.ComponentModel
Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports Greaseweazle.Actions
Imports Greaseweazle.Tools

Namespace Flux.Greaseweazle
    Partial Public Class ReadDiskForm
        Inherits BaseFluxForm

#Region "Form Controls"
        Private WithEvents ButtonConvert As Button
        Private WithEvents ButtonDetect As Button
        Private WithEvents ButtonDiscard As Button
        Private WithEvents ButtonImageFolderBrowse As Button
        Private WithEvents ButtonImport As SplitButton
        Private WithEvents ButtonImportAndClose As SplitButton
        Private WithEvents ButtonPreview As Button
        Private WithEvents ButtonRead As Button
        Private WithEvents ButtonRefine As Button
        Private WithEvents ButtonRootBrowse As Button
        Private WithEvents ButtonToggleSequence As Button
        Private WithEvents ButtonToggleSequence2 As Button
        Private WithEvents ButtonVerify As Button
        Private WithEvents CheckBoxSaveLog As CheckBox
        Private WithEvents ComboDrives As ComboBox
        Private WithEvents ComboExtensions As ComboBox
        Private WithEvents ComboExtensions2 As ComboBox
        Private WithEvents ComboImageFormat As ComboBox
        Private WithEvents ComboImageLocation As ComboBox
        Private WithEvents ComboOutputType As ComboBox
        Private WithEvents ComboOutputType2 As ComboBox
        Private WithEvents NumericRevs As NumericUpDown
        Private WithEvents TextBoxFileName As TextBox
        Private WithEvents TextBoxFolderName As TextBox
        Private WithEvents TextBoxImageFolder As TextBox
        Private WithEvents TextBoxPrefixName As PlaceholderTextBox
        Private WithEvents TextBoxRootFolder As TextBox
        Private _ImportButtonNoEvent As Boolean = False
        Private _LabelDrive As Label
        Private _LabelFileName As Label
        Private _LabelFolderName As Label
        Private _LabelImageFolder As Label
        Private _LabelImageFormat As Label
        Private _LabelLocation As Label
        Private _LabelOutputType As Label
        Private _LabelOutputType2 As Label
        Private _LabelPrefixName As Label
        Private _LabelRetries As Label
        Private _LabelRevs As Label
        Private _LabelRootFolder As Label
        Private _LabelSeekRetries As Label
        Private _NumericRetries As NumericUpDown
        Private _NumericSeekRetries As NumericUpDown
#End Region
        Private WithEvents ConvertCmd As ConvertCommand
        Private WithEvents ReadCmd As ReadCommand
        Private Const DEFAULT_RAW_FILE_NAME As String = "track"
        Private Shared _CachedFileNameTemplate As String = ""
        Private Shared _CachedFolderNameTemplate As String = ""
        Private Shared _CachedPrefixNameTemplate As String = DEFAULT_RAW_FILE_NAME
        Private ReadOnly _DrivesAvailable As Boolean = False
        Private ReadOnly _GridIndex As New Dictionary(Of GridRows, Integer)
        Private ReadOnly _Initialized As Boolean = False
        Private ReadOnly _KryofluxAvailable As Boolean = False
        Private ReadOnly _Status As TrackStatus
        Private ReadOnly _ToolTip As New ToolTip()
        Private ReadOnly _UserState As Settings.UserStateFlux
        Private _CachedRevs As Byte = 0
        Private _CachedRevsFlux As Byte = 0
        Private _ComboExtensionsNoEvent As Boolean = False
        Private _ComboImageFormatNoEvent As Boolean = False
        Private _ComboImageLocationNoEvent As Boolean = False
        Private _ComboOutputTypeNoEvent As Boolean = False
        Private _FileReprocessMode As Boolean = False
        Private _LastFluxOutput As Boolean? = Nothing
        Private _LastSequenceTextBox As TextBoxBase
        Private _NewFileName As String = ""
        Private _NewFilePath As String = ""
        Private _NumericRevsNoEvent As Boolean = False
        Private _OutputDiskParams As FloppyDiskParams?
        Private _OutputDriveOption As DriveOption
        Private _OutputDoubleStep As Boolean = False
        Private _SelectedDriveOption As DriveOption
        Private _TempFilePath As String = ""
        Private _TempFilePath2 As String = ""

        Private Enum GridRows
            Drive
            Format
            FileName
            Prefix
            RootFolder
            ImageFolder
            FolderName
            Grid
        End Enum

        Public Event ImportProcess(File As String, NewFilename As String)

        Public Sub New(KryofluxAvailable As Boolean)
            MyBase.New(Settings.LogFileName, UseProcess:=KryofluxAvailable)
            InitializeControls()

            ConvertCmd = Engine.Convert
            ReadCmd = Engine.Read

            _UserState = App.UserState.Flux
            _Status = New TrackStatus()
            TrackStatus = _Status

            _KryofluxAvailable = KryofluxAvailable

            If _KryofluxAvailable Then
                _KryofluxStatus = New Flux.Kryoflux.TrackStatus()
            End If

            Me.HelpButton = True
            Me.Text = My.Resources.Label_ReadDisk

            InitializeHelp()

            _SelectedDriveOption = PopulateDrives(ComboDrives, FloppyDriveType.DriveUnknown, SelectedDeviceState.DriveId)
            _DrivesAvailable = ComboDrives.Items.OfType(Of DriveOption)().Any(Function(d) Not String.IsNullOrEmpty(d.Id))

            PopulateImageFormats()
            PopulateImageLocations(SelectedDeviceState.ImageLocation)
            InitializeImage()

            SetFilenames(True)
            SetRootFolder(SelectedDeviceState.RootFolder)
            SetImageFolder(SelectedDeviceState.ImageFolder)

            _CachedRevs = 2
            _CachedRevsFlux = Settings.DefaultRevs

            RefreshRevs()

            _NumericRetries.Value = DEFAULT_RETRIES
            _NumericSeekRetries.Value = DEFAULT_SEEK_RETRIES

            CheckBoxSaveLog.Checked = SelectedDeviceState.SaveLog

            ButtonToggleSequence.Enabled = False
            ButtonToggleSequence2.Enabled = False

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

        Private ReadOnly Property CanChangeSettings As Boolean
            Get
                Return IsIdle AndAlso Not HasOutputFile
            End Get
        End Property

        Private ReadOnly Property CanRefine As Boolean
            Get
                Return HasOutputFile AndAlso IsFluxOutput
            End Get
        End Property

        Private ReadOnly Property DisplayImageFolder As Boolean
            Get
                Return DisplayImageLocation AndAlso SelectedImageLocation = ReadDiskImageLocations.Other
            End Get
        End Property

        Private ReadOnly Property DisplayImageLocation As Boolean
            Get
                Return IsFluxOutput AndAlso SelectedOutputType2 <> ReadDiskOutputTypes.None
            End Get
        End Property

        Private Property FileNameInput As String
            Get
                Return TextBoxFileName.Text.Trim()
            End Get
            Set(value As String)
                TextBoxFileName.Text = If(value, String.Empty).Trim
            End Set
        End Property

        Private Property FolderNameInput As String
            Get
                Return TextBoxFolderName.Text.Trim()
            End Get
            Set(value As String)
                TextBoxFolderName.Text = If(value, String.Empty).Trim
            End Set
        End Property

        Private ReadOnly Property HasOptionId As Boolean
            Get
                Return Not String.IsNullOrEmpty(_SelectedDriveOption?.Id)
            End Get
        End Property

        Private ReadOnly Property HasOutputFile As Boolean
            Get
                Return Not String.IsNullOrEmpty(_TempFilePath)
            End Get
        End Property

        Private ReadOnly Property HasOutputFile2 As Boolean
            Get
                Return Not String.IsNullOrEmpty(_TempFilePath2)
            End Get
        End Property

        Private Property ImageFolderInput As String
            Get
                Return TextBoxImageFolder.Text.Trim()
            End Get
            Set(value As String)
                TextBoxImageFolder.Text = If(value, String.Empty).Trim()
            End Set
        End Property

        Private ReadOnly Property IsDoubleStepDrive As Boolean
            Get
                Return _SelectedDriveOption IsNot Nothing AndAlso _SelectedDriveOption.Type = FloppyDriveType.Drive525HighDensity
            End Get
        End Property

        Private ReadOnly Property IsFluxOutput As Boolean
            Get
                Return SelectedOutputType = ReadDiskOutputTypes.RAW
            End Get
        End Property

        Private Property RootFolderInput As String
            Get
                Return TextBoxRootFolder.Text.Trim()
            End Get
            Set(value As String)
                TextBoxRootFolder.Text = If(value, String.Empty).Trim()
            End Set
        End Property
        Private ReadOnly Property SelectedDeviceState As Settings.UserStateFluxReadDevice
            Get
                Return _UserState.Read.Device(IDevice.FluxDevice.Greaseweazle)
            End Get
        End Property

        Private ReadOnly Property SelectedDiskFormat As FloppyDiskFormat?
            Get
                If TypeOf ComboImageFormat.SelectedValue IsNot FloppyDiskParams Then
                    Return Nothing
                End If

                Return DirectCast(ComboImageFormat.SelectedValue, FloppyDiskParams).Format
            End Get
        End Property

        Private ReadOnly Property SelectedDiskParams As FloppyDiskParams?
            Get
                If TypeOf ComboImageFormat.SelectedValue IsNot FloppyDiskParams Then
                    Return Nothing
                End If

                Return DirectCast(ComboImageFormat.SelectedValue, FloppyDiskParams)
            End Get
        End Property

        Private ReadOnly Property SelectedImageLocation As ReadDiskImageLocations?
            Get
                If TypeOf ComboImageLocation.SelectedValue IsNot ReadDiskImageLocations Then
                    Return Nothing
                End If

                Return DirectCast(ComboImageLocation.SelectedValue, ReadDiskImageLocations)
            End Get
        End Property

        Private ReadOnly Property SelectedOutputType As ReadDiskOutputTypes
            Get
                If ComboOutputType.SelectedValue Is Nothing Then
                    Return ReadDiskOutputTypes.None
                End If

                Return DirectCast(ComboOutputType.SelectedValue, ReadDiskOutputTypes)
            End Get
        End Property

        Private ReadOnly Property SelectedOutputType2 As ReadDiskOutputTypes
            Get
                If ComboOutputType2.SelectedValue Is Nothing Then
                    Return ReadDiskOutputTypes.None
                End If

                Return DirectCast(ComboOutputType2.SelectedValue, ReadDiskOutputTypes)
            End Get
        End Property

        Protected Overrides Sub OnAfterBaseFormClosing(e As FormClosingEventArgs)
            If Me.DialogResult = DialogResult.Cancel OrElse Me.DialogResult = DialogResult.None Then
                ClearOutputFile(True)
                _NewFilePath = ""
                _NewFileName = ""
            End If
        End Sub

        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
            ConvertCmd = Nothing
            ReadCmd = Nothing

            MyBase.OnFormClosed(e)
        End Sub

        Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
            If keyData = (Keys.Alt Or Keys.S) Then
                If IsToggleSequenceEnabled() Then
                    DoToggleSequence()
                End If
                Return True
            End If

            Return MyBase.ProcessCmdKey(msg, keyData)
        End Function

        Protected Overrides Sub SaveLog(RemovePath As Boolean, Optional InitialDirectory As String = "")
            If String.IsNullOrEmpty(InitialDirectory) Then
                If HasOutputFile AndAlso IsFluxOutput Then
                    InitialDirectory = IO.Path.GetDirectoryName(_TempFilePath)
                End If
            End If

            MyBase.SaveLog(RemovePath, InitialDirectory)
        End Sub

        Private Sub ApplyProcessState(state As ConsoleProcessRunner.ProcessStateEnum)
            Dim Finished As Boolean = False
            Dim Completed As Boolean = False

            Select Case state
                Case ConsoleProcessRunner.ProcessStateEnum.Aborted
                    Finished = True
                    TrackStatus.UpdateTrackStatusAborted()

                Case ConsoleProcessRunner.ProcessStateEnum.Completed
                    Finished = True
                    If TrackStatus.TrackFound Then
                        Completed = True
                        TrackStatus.UpdateTrackStatusComplete(_OutputDoubleStep)
                    Else
                        TrackStatus.UpdateTrackStatusError()
                    End If

                Case ConsoleProcessRunner.ProcessStateEnum.Error
                    Finished = True
                    TrackStatus.UpdateTrackStatusError()
            End Select

            If Finished Then
                If Completed Then
                    SetTitleBarText()
                ElseIf Not _FileReprocessMode Then
                    ClearOutputFile(True)
                End If
                _FileReprocessMode = False
            End If

            RefreshFormState()
        End Sub

        Private Sub CacheFilenameTemplate()
            If IsFluxOutput Then
                Dim FolderName = FolderNameInput

                If ContainsPlaceholder(FolderName) Then
                    _CachedFolderNameTemplate = IncrementPlaceholders(FolderName)
                Else
                    _CachedFolderNameTemplate = ""
                End If

                Dim PrefixName = TextBoxPrefixName.Text.Trim
                If ContainsPlaceholder(PrefixName) Then
                    _CachedPrefixNameTemplate = IncrementPlaceholders(PrefixName)
                Else
                    _CachedPrefixNameTemplate = DEFAULT_RAW_FILE_NAME
                End If

                _CachedFileNameTemplate = ""

            Else
                Dim Filename = FileNameInput

                If ContainsPlaceholder(Filename) Then
                    _CachedFileNameTemplate = IncrementPlaceholders(Filename)
                Else
                    _CachedFileNameTemplate = ""
                End If

                _CachedFolderNameTemplate = ""
                _CachedPrefixNameTemplate = ""
            End If
        End Sub

        Private Function CheckCompatibility() As Boolean
            Dim DiskParams = SelectedDiskParams

            If _SelectedDriveOption Is Nothing OrElse _SelectedDriveOption.Type = FloppyDriveType.DriveUnknown Then
                Return True
            End If

            If Not DiskParams.HasValue OrElse DiskParams.Value.IsNonImage Then
                Return True
            End If

            Dim FloppyType = GreaseweazleFindCompatibleDriveType(DiskParams.Value, _SelectedDriveOption.Type)

            Return FloppyType = _SelectedDriveOption.Type
        End Function

        Private Function CheckRootFolder() As Boolean
            If String.IsNullOrEmpty(RootFolderInput) Then
                MsgBox(My.Resources.Dialog_SelectRootFolder, MsgBoxStyle.Exclamation)
                ButtonRootBrowse.Focus()
                Return False
            End If

            Return True
        End Function

        Private Function CheckFileNameEntered(CheckImageFile As Boolean) As Boolean
            If IsFluxOutput Then
                If Not CheckRootFolder() Then
                    Return False
                End If

                If CheckImageFile AndAlso DisplayImageFolder AndAlso String.IsNullOrEmpty(ImageFolderInput) Then
                    MsgBox(My.Resources.Dialog_SelectImageFolder, MsgBoxStyle.Exclamation)
                    TextBoxImageFolder.Focus()
                    Return False
                End If

                If String.IsNullOrEmpty(FolderNameInput) Then
                    MsgBox(My.Resources.Dialog_EnterFolderName, MsgBoxStyle.Exclamation)
                    TextBoxFolderName.Focus()
                    Return False
                End If


                If Not ConfirmFluxDestinationOverwrite(CheckImageFile) Then
                    TextBoxFolderName.Focus()
                    Return False
                End If

            Else
                If String.IsNullOrEmpty(FileNameInput) Then
                    MsgBox(My.Resources.Dialog_EnterFileName, MsgBoxStyle.Exclamation)
                    TextBoxFileName.Focus()
                    Return False
                End If
            End If

            Return True
        End Function

        Private Sub ClearOutputFile(Delete As Boolean)
            If Not HasOutputFile Then
                Exit Sub
            End If

            If Delete Then
                If IsFluxOutput Then
                    Dim PathName = IO.Path.GetDirectoryName(_TempFilePath)
                    DeleteTempFolder(PathName)
                Else
                    DeleteTempFileIfExists(_TempFilePath)
                End If
            End If

            If HasOutputFile2 Then
                DeleteTempFileIfExists(_TempFilePath2)
            End If

            _TempFilePath = ""
            _TempFilePath2 = ""
            _OutputDoubleStep = False
            _OutputDiskParams = Nothing
            _OutputDriveOption = Nothing
            _FileReprocessMode = False
        End Sub

        Private Sub ClearOutputType2()
            If Not String.IsNullOrEmpty(_TempFilePath2) Then
                DeleteFileIfExists(_TempFilePath2)
                _TempFilePath2 = ""
            End If

            _ComboOutputTypeNoEvent = True
            ComboOutputType2.SelectedValue = ReadDiskOutputTypes.None
            _ComboOutputTypeNoEvent = False

            PopulateFileExtensions(ComboExtensions2, SelectedOutputType2)
        End Sub

        Private Sub ClearProcessedImage(DeleteOutputFile As Boolean, RefreshState As Boolean)
            _FileReprocessMode = False

            ClearOutputFile(DeleteOutputFile)
            ClearLogAndStatus()
            ResetTrackGrid()
            TrackStatus.Clear()
            SetTitleBarText()

            If Not DeleteOutputFile Then
                CacheFilenameTemplate()
                SetFilenames(True)

            ElseIf RefreshState Then
                SetFilenames(False)
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
            CacheFilenameTemplate()

            _NewFilePath = NewFilePath
            _NewFileName = NewFileName
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Sub

        Private Function ConfirmDiscardSecondaryImageForReprocess() As Boolean
            If Not HasOutputFile2 Then
                Return True
            End If

            Dim msg = My.Resources.Dialog_DiscardSecondaryImageOnReprocess

            If MsgBox(msg, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkCancel Or vbDefaultButton2) <> MsgBoxResult.Ok Then
                Return False
            End If

            ClearOutputType2()

            RefreshFormState()

            Return True
        End Function

        Private Function ConfirmFluxDestinationOverwrite(CheckImageFile As Boolean) As Boolean
            Dim destinationFolder = FluxGetFolderPath()
            Dim imageFilePath = GetImageFilePath()

            Dim ImageSetExists As Boolean = False
            Dim ImageFileExists As Boolean = False

            If CheckImageFile AndAlso Not String.IsNullOrEmpty(imageFilePath) AndAlso IO.File.Exists(imageFilePath) Then
                ImageFileExists = True
            End If

            If IO.Directory.Exists(destinationFolder) Then
                If IO.Directory.EnumerateFiles(destinationFolder, FluxGetWildcardFileName(), IO.SearchOption.TopDirectoryOnly).Any() Then
                    ImageSetExists = True
                End If
            End If

            If Not ImageSetExists AndAlso Not ImageFileExists Then
                Return True
            End If

            Dim msg As String
            If ImageSetExists AndAlso ImageFileExists Then
                msg = String.Format(My.Resources.Dialog_ImageSetAndFileExist, destinationFolder, imageFilePath)
            ElseIf ImageSetExists Then
                msg = String.Format(My.Resources.Dialog_ImageSetExists, destinationFolder)
            Else
                msg = String.Format(My.Resources.Dialog_ImageFileExists, imageFilePath)
            End If

            Return MsgBox(msg, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkCancel Or vbDefaultButton2) = MsgBoxResult.Ok
        End Function
        Private Sub ConvertImage()
            Dim Response = ConvertFluxImage(_TempFilePath, True, Nothing, True, FluxGetOutputFilePath())

            If Response.Result = DialogResult.OK Then
                If Not FinalizeFluxOutput() Then
                    Exit Sub
                End If

                CloseForm(Response.OutputFile, Response.NewFileName)

            ElseIf Response.Result = DialogResult.Retry Then
                If Not FinalizeFluxOutput() Then
                    Exit Sub
                End If

                ProcessImport(Response.OutputFile, Response.NewFileName)
            End If
        End Sub

        Private Sub DoFormatDetection()
            Dim ImageFormat As FloppyDiskFormat?
            Dim Doublestep As Boolean

            If Not HasOptionId Then
                ImageFormat = Nothing
                Doublestep = False
            Else
                ImageFormat = ReadImageFormat(_SelectedDriveOption.Id)
                _SelectedDriveOption.SelectedFormat = ImageFormat
                _SelectedDriveOption.DetectedFormat = ImageFormat
                Doublestep = IsDoubleStepDrive
            End If

            SharedLib.PopulateImageFormats(ComboImageFormat, ImageFormat, ImageFormat, True, Doublestep)
        End Sub

        Private Sub DoToggleSequence()
            If _LastSequenceTextBox Is Nothing Then
                Exit Sub
            End If
            ToggleNumberTokenAtCaretOrSelection(_LastSequenceTextBox)
            UpdateSequenceButtonState()
            _LastSequenceTextBox.Focus()
        End Sub

        Private Sub FileNameChangePostProcess()
            RefreshImportButtonState()
            RefreshProcessButtonState()
            RefreshTitleBarText()
        End Sub

        Private Function FinalizeFluxOutput() As Boolean
            If Not HasOutputFile Then
                Return False
            End If

            Dim tempFolder = IO.Path.GetDirectoryName(_TempFilePath)
            Dim destFolder = FluxGetFolderPath()
            Dim prefix = FluxGetPrefix()

            If Not ReadDiskHelpers.FinalizeFluxTempFolder(tempFolder, destFolder, prefix) Then
                MsgBox(My.Resources.Message_SaveFluxSetError, MsgBoxStyle.Exclamation)
                Return False
            End If

            UpdateVerifyLogPaths(destFolder)
            _TempFilePath = FluxGetOutputFilePath()

            Return True
        End Function

        Private Sub FinalizeImageOutput()
            Dim FilePath = GetImageFilePath()

            If String.IsNullOrEmpty(FilePath) Then
                Return
            End If
            Try
                IO.File.Copy(_TempFilePath2, FilePath, True)
                DeleteFileIfExists(_TempFilePath2)
            Catch
                MsgBox(String.Format(My.Resources.Dialog_SaveFileError, FilePath), MsgBoxStyle.Exclamation)
                Return
            End Try

            Return
        End Sub

        Private Function FluxGetFirstTrackFileName() As String
            Return ReadDiskHelpers.FluxGetFirstTrackFileName(FluxGetPrefix())
        End Function

        Private Function FluxGetFolderPath() As String
            Dim Folder = ReadDiskHelpers.GetOutputFolderName(FolderNameInput)

            Return IO.Path.Combine(RootFolderInput, Folder)
        End Function

        Private Function FluxGetOutputFilePath() As String
            Return IO.Path.Combine(FluxGetFolderPath(), FluxGetFirstTrackFileName())
        End Function

        Private Function FluxGetPrefix() As String
            Dim Prefix = TextBoxPrefixName.Text.Trim()

            If String.IsNullOrEmpty(Prefix) Then
                Prefix = DEFAULT_RAW_FILE_NAME
            End If

            Return Prefix
        End Function

        Private Function FluxGetWildcardFileName() As String
            Return FluxGetPrefix() & ReadDiskHelpers.FLUX_WILDCARD
        End Function

        Private Function GetImageFilePath() As String
            If Not HasOutputFile2 Then
                Return ""
            End If

            If Not SelectedImageLocation.HasValue Then
                Return ""
            End If

            Dim ImageLocation = SelectedImageLocation.Value

            Dim Extension = IO.Path.GetExtension(_TempFilePath2)
            Dim FileName = ReadDiskHelpers.GetOutputFolderName(FolderNameInput) & Extension
            Dim PathName As String

            If ImageLocation = ReadDiskImageLocations.Root Then
                PathName = RootFolderInput
            ElseIf ImageLocation = ReadDiskImageLocations.Flux Then
                PathName = FluxGetFolderPath()
            Else
                PathName = ImageFolderInput
            End If

            Dim FilePath = IO.Path.Combine(PathName, FileName)

            Return FilePath
        End Function
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

            Dim NewFileName As String = FileNameInput

            If ContainsPlaceholder(NewFileName) Then
                NewFileName = StripAngleBrackets(NewFileName)
            End If

            NewFileName &= Extension

            Return NewFileName
        End Function

        Private Function GetOutputFilePaths() As (FilePath As String, FilePath2 As String, LogFilePath As String, IsFlux As Boolean)
            Dim Response As (FilePath As String, FilePath2 As String, LogFilePath As String, IsFlux As Boolean)

            Response.LogFilePath = ""
            Response.FilePath2 = ""
            Response.IsFlux = IsFluxOutput

            Dim DiskParams = SelectedDiskParams
            Dim NonStandard As Boolean = Not DiskParams.HasValue OrElse Not DiskParams.Value.IsStandard

            If Response.IsFlux Then
                Dim FileName = "~gwread_" & Guid.NewGuid().ToString("N")
                Dim Pathname = IO.Path.Combine(RootFolderInput, FileName)
                Dim FilePath = IO.Path.Combine(Pathname, FluxGetFirstTrackFileName())

                Try
                    IO.Directory.CreateDirectory(Pathname)
                    Response.FilePath = FilePath
                Catch ex As Exception
                    MsgBox(My.Resources.Dialog_ParentFolderCreationError, MsgBoxStyle.Critical)
                    Response.FilePath = ""
                End Try

                If CheckBoxSaveLog.Checked Then
                    Response.LogFilePath = IO.Path.Combine(Pathname, Settings.LogFileName)
                End If

                If SelectedOutputType2 <> ReadDiskOutputTypes.None AndAlso SelectedOutputType2 <> ReadDiskOutputTypes.RAW Then
                    Dim OutputType2 = If(NonStandard, ReadDiskOutputTypes.HFE, SelectedOutputType2)

                    Response.FilePath2 = GenerateOutputFile(ReadDisktOutputTypeFileExt(OutputType2))
                End If

            Else
                Dim OutputType = If(NonStandard, ReadDiskOutputTypes.HFE, SelectedOutputType)

                Response.FilePath = GenerateOutputFile(ReadDisktOutputTypeFileExt(OutputType))
            End If

            Return Response
        End Function

        Private Sub ImageFolderBrowse()
            Dim FolderName = BrowseFolderVista(ImageFolderInput, Me.Handle)
            If FolderName <> "" Then
                SetImageFolder(FolderName)
            End If
        End Sub

        Private Sub InitializeImage()
            ResetOutputTypes()
            ResetOutputTypes2()

            ResetTrackGrid()
            ClearStatusBar()
            RefreshFormState()
            SetTitleBarText()
        End Sub

        Private Function IsToggleSequenceEnabled() As Boolean
            Return (_LastSequenceTextBox IsNot Nothing AndAlso CanToggleSequenceAtCaretOrSelection(_LastSequenceTextBox))
        End Function

        Private Sub PopulateFileExtensions(Combo As ComboBox, OutputType As ReadDiskOutputTypes)
            _ComboExtensionsNoEvent = True

            Dim IsBitstreamOutput As Boolean = (OutputType = ReadDiskOutputTypes.HFE OrElse OutputType = ReadDiskOutputTypes.RAW)

            If OutputType = ReadDiskOutputTypes.None Then
                With Combo
                    .DataSource = Nothing
                    .Items.Clear()
                    .DisplayMember = "Extension"
                    .DropDownStyle = ComboBoxStyle.DropDownList
                End With

            ElseIf IsBitstreamOutput Then
                Dim items As New List(Of FileExtensionItem) From {
                    New FileExtensionItem(ReadDisktOutputTypeFileExt(OutputType), Nothing)
                }
                With Combo
                    .DataSource = Nothing
                    .Items.Clear()
                    .DisplayMember = "Extension"
                    .DataSource = items
                    .SelectedIndex = 0
                    .DropDownStyle = ComboBoxStyle.DropDownList
                End With
            Else
                SharedLib.PopulateFileExtensions(Combo, SelectedDiskFormat)
            End If

            Combo.Enabled = (Combo.Items.Count > 1)

            _ComboExtensionsNoEvent = False
        End Sub

        Private Sub PopulateImageFormats()
            _ComboImageFormatNoEvent = True
            SharedLib.PopulateImageFormats(ComboImageFormat, _SelectedDriveOption, True, IsDoubleStepDrive)
            _ComboImageFormatNoEvent = False
        End Sub

        Private Sub PopulateImageLocations(Optional CurrentValue As ReadDiskImageLocations? = Nothing)
            _ComboImageLocationNoEvent = True

            Dim ComboList As New List(Of KeyValuePair(Of String, ReadDiskImageLocations))

            For Each Location As ReadDiskImageLocations In [Enum].GetValues(GetType(ReadDiskImageLocations))
                ComboList.Add(New KeyValuePair(Of String, ReadDiskImageLocations)(
                    ReadDiskImageLocationDescription(Location), Location)
                )
            Next

            InitializeCombo(ComboImageLocation, ComboList, CurrentValue)

            FinalizeCombo(ComboImageLocation)

            _ComboImageLocationNoEvent = False
        End Sub

        Private Sub PopulateOutputTypes(Combo As ComboBox, Optional CurrentValue As ReadDiskOutputTypes = ReadDiskOutputTypes.None, Optional NoFlux As Boolean = False)
            _ComboOutputTypeNoEvent = True

            Dim DiskParams = SelectedDiskParams

            Dim DriveList As New List(Of KeyValuePair(Of String, ReadDiskOutputTypes))


            For Each OutputType As ReadDiskOutputTypes In [Enum].GetValues(GetType(ReadDiskOutputTypes))
                If OutputType = ReadDiskOutputTypes.None AndAlso Not NoFlux Then
                    Continue For
                End If

                If OutputType = ReadDiskOutputTypes.RAW AndAlso NoFlux Then
                    Continue For
                End If

                If OutputType <> ReadDiskOutputTypes.None AndAlso DiskParams.HasValue Then
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

            InitializeCombo(Combo, DriveList, CurrentValue)

            FinalizeCombo(Combo)

            _ComboOutputTypeNoEvent = False
        End Sub

        Private Sub PreviewImage()
            Dim Caption As String

            If IsFluxOutput Then
                Dim FileName = ReadDiskHelpers.GetOutputFolderName(FolderNameInput)

                If String.IsNullOrEmpty(FileName) Then
                    Caption = ""
                Else
                    Caption = IO.Path.GetFileName(FileName)
                End If
            Else
                Caption = ReadDiskHelpers.GetOutputFolderName(FileNameInput)
            End If

            If HasOutputFile2 Then
                Dim ImageData = New ImageData(_TempFilePath2)

                ImagePreview.Display(ImageData, Caption)

            ElseIf HasOutputFile AndAlso Not IsFluxOutput Then
                Dim ImageData = New ImageData(_TempFilePath)

                ImagePreview.Display(ImageData, Caption)

            Else
                Dim DiskParams = SelectedDiskParams
                If Not DiskParams.HasValue OrElse Not HasOptionId Then
                    Exit Sub
                End If

                Dim Response = ReadFirstTrack(_SelectedDriveOption.Id, True, DiskParams.Value)

                If Not Response.Result Then
                    MsgBox(My.Resources.Dialog_ImagePreviewFail, MsgBoxStyle.Exclamation)
                Else
                    ImagePreview.Display(Response.FileName, DiskParams.Value, Caption)
                End If

                DeleteTempFileIfExists(Response.FileName)
            End If
        End Sub

        Private Sub ProcessImage()
            Dim DiskParams = SelectedDiskParams

            If Not DiskParams.HasValue Then
                Exit Sub
            End If

            If Not HasOptionId Then
                Exit Sub
            End If

            Dim Response = GetOutputFilePaths()

            If Response.FilePath = "" Then
                Exit Sub
            End If

            TrackStatus = _Status
            ClearProcessedImage(True, False)

            _TempFilePath = Response.FilePath
            _TempFilePath2 = Response.FilePath2
            _OutputDiskParams = DiskParams
            _OutputDriveOption = _SelectedDriveOption
            _OutputDoubleStep = ReadDiskHelpers.UseDoubleStep(_SelectedDriveOption.Type, DiskParams.Value.Format)

            InitLogFilePath(If(Response.LogFilePath, ""))

            StartReadRun(Response.FilePath, _SelectedDriveOption, DiskParams.Value, SelectedOutputType, _OutputDoubleStep, Nothing, Nothing, Response.FilePath2, SelectedOutputType2)
        End Sub

        Private Sub ProcessImport(OutputFile As String, NewFileName As String)
            If Not String.IsNullOrEmpty(OutputFile) Then
                RaiseEvent ImportProcess(OutputFile, NewFileName)
            End If

            ClearProcessedImage(False, True)
        End Sub

        Private Function ProcessSave() As Boolean
            Dim NewFileName = GetNewFileName(_TempFilePath)
            Dim Extension = IO.Path.GetExtension(NewFileName)
            Dim ImageTypeName = GetImageTypeNameFromExtension(Extension)
            Dim FileName = IO.Path.GetFileName(NewFileName)

            Dim InitialDirectory As String
            Dim SavePath As Boolean = False

            If NewFileName = FileName Then
                InitialDirectory = App.UserState.LastNewImagePath
                SavePath = True
            Else
                InitialDirectory = IO.Path.GetDirectoryName(NewFileName)
            End If

            Dim FilePath = ShowSingleExtSaveDialog(FileName, InitialDirectory, ImageTypeName)

            If String.IsNullOrEmpty(FilePath) Then
                Return False
            End If

            If SavePath Then
                App.UserState.LastNewImagePath = IO.Path.GetDirectoryName(FilePath)
            End If

            Try
                IO.File.Copy(_TempFilePath, FilePath, True)
                DeleteFileIfExists(_TempFilePath)
            Catch
                MsgBox(String.Format(My.Resources.Dialog_SaveFileError, FilePath), MsgBoxStyle.Exclamation)
                Return False
            End Try

            ClearProcessedImage(False, True)

            Return True
        End Function

        Private Function ReadImageFormat(DriveId As String) As DiskImage.FloppyDiskFormat?
            Dim Response = ReadFirstTrack(DriveId, False)

            TextBoxConsole.Text = Response.Output

            If Not Response.Result Then
                Return Nothing
            End If

            Return DetectImageFormat(Response.FileName, True)
        End Function

        Private Sub RefineFromRaw()
            Dim SourceFile = OpenFluxImage(False)

            If String.IsNullOrEmpty(SourceFile) Then
                Exit Sub
            End If

            Dim Info = GetFluxSetInfoRaw(SourceFile, ReadHeaders:=True)

            If Not Info.Result Then
                MsgBox(My.Resources.Dialog_InvalidKryofluxFile, MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            Dim FormatResp = ConvertFirstTrack(SourceFile, False)

            Dim DetectedFormat As FloppyDiskFormat = If(FormatResp.Result, DetectImageFormat(FormatResp.FileName, True), FloppyDiskFormat.FloppyUnknown)

            Dim TempRoot = InitTempImagePath()

            If String.IsNullOrEmpty(TempRoot) Then
                MsgBox(My.Resources.Dialog_TempPathError, MsgBoxStyle.Critical)
                Exit Sub
            End If

            Dim TempFolder = IO.Path.Combine(TempRoot, Guid.NewGuid.ToString("N"))
            Dim SourceFolder = IO.Path.GetDirectoryName(SourceFile)

            Try
                IO.Directory.CreateDirectory(TempFolder)

                For Each src In IO.Directory.EnumerateFiles(SourceFolder, Info.Prefix & "*.raw", IO.SearchOption.TopDirectoryOnly)
                    IO.File.Copy(src, IO.Path.Combine(TempFolder, IO.Path.GetFileName(src)), True)
                Next
            Catch ex As Exception
                HandleRunFailure(ex.Message)
                DeleteTempFolder(TempFolder)
                Exit Sub
            End Try

            ClearProcessedImage(False, False)

            SelectDriveForRefine(DetectedFormat)

            If HasOptionId Then
                _SelectedDriveOption.SelectedFormat = DetectedFormat
                _SelectedDriveOption.DetectedFormat = DetectedFormat
            End If

            _ComboImageFormatNoEvent = True
            SharedLib.PopulateImageFormats(ComboImageFormat, DetectedFormat, DetectedFormat, True, IsDoubleStepDrive)
            _ComboImageFormatNoEvent = False

            ResetOutputTypes(ReadDiskOutputTypes.RAW)

            RootFolderInput = IO.Path.GetDirectoryName(SourceFolder)
            FolderNameInput = ReadDiskHelpers.NormalizeFluxFolder(SourceFolder, RootFolderInput)
            TextBoxPrefixName.Text = Info.Prefix

            Dim DiskParams = SelectedDiskParams

            If Not DiskParams.HasValue Then
                MsgBox(My.Resources.Dialog_DetectFormatError, MsgBoxStyle.Exclamation)
                DeleteTempFolder(TempFolder)
                Exit Sub
            End If

            _TempFilePath = IO.Path.Combine(TempFolder, ReadDiskHelpers.FluxGetFirstTrackFileName(Info.Prefix))
            _OutputDoubleStep = _SelectedDriveOption IsNot Nothing AndAlso ReadDiskHelpers.UseDoubleStep(_SelectedDriveOption.Type, DetectedFormat)

            TrackStatus = _Status
            TrackStatus.Clear()
            ResetTrackGrid()

            Dim Opts As ConvertOptions
            Try
                Opts = ReadDiskHelpers.BuildRefineConvertOptions(_TempFilePath, DiskParams.Value, _OutputDoubleStep)
            Catch ex As Exception
                HandleRunFailure(ex.Message)
                ApplyProcessState(ConsoleProcessRunner.ProcessStateEnum.Error)
                Return
            End Try

            ClearOutputType2()

            InitLogFilePath(If(CheckBoxSaveLog.Checked, IO.Path.Combine(TempFolder, Settings.LogFileName), ""))

            Runner.RunAsync(Sub(Token) ConvertCmd.Run(Opts, Token))
        End Sub

        Private Sub RefreshFormState()
            Dim DiskParams = SelectedDiskParams

            Dim HasOutputFile As Boolean = Me.HasOutputFile

            Dim CanChangeSettings = Me.CanChangeSettings
            Dim CanConvert As Boolean = IsIdle AndAlso HasOutputFile AndAlso IsFluxOutput AndAlso Not HasOutputFile2
            Dim DriveSelected As Boolean = HasOptionId
            Dim SelectMode As Boolean = IsIdle AndAlso CanRefine

            ComboImageFormat.Enabled = (CanChangeSettings OrElse SelectMode) AndAlso DriveSelected
            ComboDrives.Enabled = CanChangeSettings OrElse SelectMode

            ComboOutputType.Enabled = CanChangeSettings AndAlso ComboOutputType.Items.Count > 1
            ComboOutputType2.Enabled = CanChangeSettings AndAlso ComboOutputType2.Items.Count > 1

            ComboExtensions2.Enabled = CanChangeSettings AndAlso ComboExtensions2.Items.Count > 1

            NumericRevs.Enabled = CanChangeSettings OrElse SelectMode
            _NumericRetries.Enabled = CanChangeSettings OrElse SelectMode
            _NumericSeekRetries.Enabled = CanChangeSettings OrElse SelectMode

            RefreshSaveLogButtonState()
            ButtonReset.Enabled = IsIdle

            ButtonConvert.Enabled = CanConvert
            ButtonConvert.Visible = IsFluxOutput

            Dim NonImageFormat As Boolean = Not _OutputDiskParams.HasValue OrElse _OutputDiskParams.Value.IsNonImage
            Dim CanVerify As Boolean = IsIdle AndAlso HasOutputFile AndAlso IsFluxOutput AndAlso Not NonImageFormat AndAlso _KryofluxAvailable

            ButtonVerify.Enabled = CanVerify
            ButtonVerify.Visible = IsFluxOutput AndAlso _KryofluxAvailable

            ButtonDiscard.Enabled = IsIdle AndAlso HasOutputFile

            ButtonDetect.Enabled = (CanChangeSettings OrElse SelectMode) AndAlso DriveSelected

            ButtonCancel.Text = If(IsRunning OrElse HasOutputFile, WithoutHotkey(My.Resources.Menu_Cancel), WithoutHotkey(My.Resources.Menu_Close))

            ButtonPreview.Enabled = IsIdle AndAlso DriveSelected AndAlso DiskParams.HasValue AndAlso Not DiskParams.Value.IsNonImage

            ButtonRefine.Enabled = IsIdle AndAlso Not HasOutputFile AndAlso _DrivesAvailable

            GridSelectEnabled(CanRefine)
            GridHideSelection(Not IsIdle)

            RefreshProcessButtonState()
            RefreshImportButtonState()
            ToggleRootFolderControls()
            ToggleImageLocationVisible()
        End Sub

        Private Sub RefreshImportButtonItems(IsFluxOutput As Boolean)
            _ImportButtonNoEvent = True

            PanelButtonsRight.SuspendLayout()

            ButtonImport.ClearItems()
            ButtonImportAndClose.ClearItems()

            If IsFluxOutput Then
                ButtonImport.AddItem(WithoutHotkey(My.Resources.Menu_Save), ConversionMode.Save)
                ButtonImportAndClose.AddItem(My.Resources.Label_SaveAndClose, ConversionMode.Save)
                ButtonImport.SelectedValue = ConversionMode.Save
                ButtonImportAndClose.SelectedValue = ConversionMode.Save
            Else
                ButtonImport.AddItem(WithoutHotkey(My.Resources.Label_Import), ConversionMode.Import)
                ButtonImport.AddItem(My.Resources.Label_Save, ConversionMode.Save)
                ButtonImportAndClose.AddItem(My.Resources.Label_ImportClose, ConversionMode.Import)
                ButtonImportAndClose.AddItem(My.Resources.Label_SaveAndClose, ConversionMode.Save)

                Dim Mode = App.UserState.Flux.Convert.ConversionMode
                ButtonImport.SelectedValue = Mode
                ButtonImportAndClose.SelectedValue = Mode
            End If

            PanelButtonsRight.ResumeLayout(True)

            _ImportButtonNoEvent = False
        End Sub
        Private Sub RefreshImportButtonState()

            Dim EnableImport As Boolean = IsIdle AndAlso HasOutputFile

            ButtonImportAndClose.Enabled = EnableImport
            ButtonImport.Enabled = EnableImport

            InitializeHelpImportButtons(IsFluxOutput)
        End Sub

        Private Sub RefreshPreferredExensions()
            Dim Item As FileExtensionItem = ComboExtensions.SelectedValue

            If Not Item.Format.HasValue Then
                Exit Sub
            End If

            Dim Format = SelectedDiskFormat

            If Not Format.HasValue Then
                Exit Sub
            End If

            If Item.Format.Value <> Format.Value Then
                App.UserState.RemovePreferredExtension(Format.Value)
            End If

            App.UserState.SetPreferredExtension(Item.Format.Value, Item.Extension)
        End Sub

        Private Sub RefreshProcessButtonState()
            If IsRunning Then
                ButtonRead.Text = My.Resources.Label_Abort
                ButtonRead.Enabled = True
            Else
                Dim DiskParams = SelectedDiskParams
                Dim TracksSelected As Boolean = CanRefine AndAlso HasSelectedTracks

                ButtonRead.Text = My.Resources.Label_Read
                ButtonRead.Enabled = HasOptionId AndAlso DiskParams.HasValue AndAlso (Not HasOutputFile OrElse TracksSelected)
            End If
        End Sub

        Private Sub RefreshRevs()
            _NumericRevsNoEvent = True
            If IsFluxOutput Then
                NumericRevs.Value = _CachedRevsFlux
            Else
                NumericRevs.Value = _CachedRevs
            End If
            _NumericRevsNoEvent = False
        End Sub

        Private Sub RefreshTitleBarText()
            If Not Runner.State = ConsoleProcessRunner.ProcessStateEnum.Completed Then
                Exit Sub
            End If

            SetTitleBarText()
        End Sub

        Private Sub RefreshTrackState(PrevOption As DriveOption, CurrentOption As DriveOption)
            Dim DiskParams = SelectedDiskParams

            If Not DiskParams.HasValue Then
                Exit Sub
            End If

            Dim PrevDoubleStep = ReadDiskHelpers.UseDoubleStep(PrevOption.Type, DiskParams.Value.Format)
            Dim Doublestep = ReadDiskHelpers.UseDoubleStep(CurrentOption.Type, DiskParams.Value.Format)

            If PrevDoubleStep <> Doublestep Then
                Dim State = GetState(PrevDoubleStep)
                SetState(State, Doublestep)
            End If
        End Sub

        Private Sub RefreshWarningLabel()
            _LabelImageFormat.ForeColor = If(CheckCompatibility(), SystemColors.ControlText, Color.Red)
        End Sub

        Private Sub ReprocessImage()
            Dim DiskParams = SelectedDiskParams

            If Not DiskParams.HasValue Then
                Exit Sub
            End If

            If Not HasOptionId Then
                Exit Sub
            End If

            TrackStatus = _Status
            TrackStatus.Clear()
            _FileReprocessMode = True

            Dim TrackRanges As List(Of (StartTrack As UShort, EndTrack As UShort)) = Nothing
            Dim Heads As TrackHeads? = Nothing
            Dim AppendLog As Boolean = False

            If HasSelectedTracks Then
                TrackRanges = GetSelectedTrackRanges()
                Heads = GetSelectedTrackHeads()

                GridResetSelectedCells()

                AppendLog = True
            End If

            _OutputDoubleStep = ReadDiskHelpers.UseDoubleStep(_SelectedDriveOption.Type, DiskParams.Value.Format)

            InitLogFilePath(IO.Path.Combine(IO.Path.GetDirectoryName(_TempFilePath), Settings.LogFileName), Append:=AppendLog)

            StartReadRun(_TempFilePath, _SelectedDriveOption, DiskParams.Value, SelectedOutputType, _OutputDoubleStep, TrackRanges, Heads, Nothing, ReadDiskOutputTypes.None)
        End Sub

        Private Sub ResetOutputTypes(Optional OutputType As ReadDiskOutputTypes? = Nothing)
            If Not OutputType.HasValue Then
                OutputType = SelectedDeviceState.OutputType
            End If

            PopulateOutputTypes(ComboOutputType, OutputType)
            PopulateFileExtensions(ComboExtensions, SelectedOutputType)
            RefreshImportButtonItems(IsFluxOutput)
        End Sub

        Private Sub ResetOutputTypes2()
            PopulateOutputTypes(ComboOutputType2, SelectedDeviceState.OutputType2, NoFlux:=True)
            PopulateFileExtensions(ComboExtensions2, SelectedOutputType2)
        End Sub

        Private Sub ResetTrackGrid(Optional ResetSelected As Boolean = True)
            Dim DiskParams = SelectedDiskParams

            Dim SideCount As Byte
            Dim FormatDriveType As FloppyDriveType

            If Not DiskParams.HasValue OrElse DiskParams.Value.IsNonImage Then
                SideCount = 2
                FormatDriveType = FloppyDriveType.DriveUnknown
            Else
                SideCount = DiskParams.Value.BPBParams.NumberOfHeads
                FormatDriveType = DiskParams.Value.DriveType
            End If
            Dim TrackCount As UShort

            If _SelectedDriveOption Is Nothing OrElse _SelectedDriveOption.Type = FloppyDriveType.DriveUnknown Then
                TrackCount = If(FormatDriveType = FloppyDriveType.Drive525DoubleDensity, GreaseweazleSettings.MAX_TRACKS_525DD, GreaseweazleSettings.MAX_TRACKS)
            Else
                TrackCount = _SelectedDriveOption.Tracks
            End If

            If _SelectedDriveOption IsNot Nothing Then
                TrackCount = Math.Max(TrackCount, _SelectedDriveOption.Tracks)
            End If

            GridReset(TrackCount, SideCount, Nothing, ResetSelected)
        End Sub

        Private Sub RootFolderBrowse()
            Dim FolderName = BrowseFolderVista(RootFolderInput, Me.Handle)
            If FolderName <> "" Then
                SetRootFolder(FolderName)
            End If
        End Sub

        Private Sub SaveAndClose(DialogResult As DialogResult)
            If ProcessSave() Then
                Me.DialogResult = DialogResult
                Me.Close()
            End If
        End Sub
        Private Sub SelectDriveForRefine(detectedFormat As FloppyDiskFormat)
            Dim TargetType As FloppyDriveType = FloppyDriveType.DriveUnknown

            If detectedFormat <> FloppyDiskFormat.FloppyUnknown Then
                Dim DiskParams = FloppyDiskFormatGetParams(detectedFormat)
                TargetType = GreaseweazleFindCompatibleDriveType(DiskParams, Settings.AvailableDriveTypes)
            End If

            If TargetType <> FloppyDriveType.DriveUnknown AndAlso HasOptionId AndAlso _SelectedDriveOption.Type = TargetType Then
                Return
            End If

            Dim TargetOpt As DriveOption = Nothing
            Dim FirstOpt As DriveOption = Nothing

            For Each Item In ComboDrives.Items
                Dim Opt = TryCast(Item, DriveOption)
                If Opt Is Nothing OrElse String.IsNullOrEmpty(Opt.Id) Then
                    Continue For
                End If

                If FirstOpt Is Nothing Then
                    FirstOpt = Opt
                End If
                If TargetOpt Is Nothing AndAlso TargetType <> FloppyDriveType.DriveUnknown AndAlso Opt.Type = TargetType Then
                    TargetOpt = Opt
                End If
            Next

            Dim NewSelection As DriveOption = Nothing
            If TargetOpt IsNot Nothing Then
                NewSelection = TargetOpt
            ElseIf Not HasOptionId Then
                NewSelection = FirstOpt
            End If

            If NewSelection IsNot Nothing AndAlso NewSelection IsNot _SelectedDriveOption Then
                ComboDrives.SelectedItem = NewSelection
            End If
        End Sub

        Private Sub SetFilenames(UseCache As Boolean)
            If UseCache Then
                FileNameInput = _CachedFileNameTemplate
                TextBoxPrefixName.Text = _CachedPrefixNameTemplate
                FolderNameInput = _CachedFolderNameTemplate
            Else
                FileNameInput = ""
                TextBoxPrefixName.Text = DEFAULT_RAW_FILE_NAME
                FolderNameInput = ""
            End If
        End Sub

        Private Sub SetImageFolder(Path As String)
            If Path <> "" AndAlso Not IO.Directory.Exists(Path) Then
                Path = ""
            End If

            ImageFolderInput = Path
            SelectedDeviceState.ImageFolder = Path
        End Sub

        Private Sub SetRootFolder(Path As String)
            If Path <> "" AndAlso Not IO.Directory.Exists(Path) Then
                Path = ""
            End If

            RootFolderInput = Path
            SelectedDeviceState.RootFolder = Path
        End Sub
        Private Sub SetTitleBarText()
            Dim Text = My.Resources.Label_ReadDisk

            If Not HasOutputFile Then
                Me.Text = Text
                Exit Sub
            End If

            If Not IsFluxOutput AndAlso String.IsNullOrEmpty(FileNameInput) Then
                Me.Text = Text
                Exit Sub

            ElseIf IsFluxOutput AndAlso String.IsNullOrEmpty(FolderNameInput) Then
                Me.Text = Text
                Exit Sub
            End If

            Dim DisplayFileName As String

            If IsFluxOutput Then
                Dim ParentFolder As String = ReadDiskHelpers.GetOutputFolderName(FolderNameInput)
                DisplayFileName = IO.Path.Combine(ParentFolder, ReadDiskHelpers.FLUX_WILDCARD)
            Else
                DisplayFileName = GetNewFileName(_TempFilePath)
            End If

            Me.Text = Text & " - " & DisplayFileName
        End Sub

        Private Sub StartReadRun(filePath As String,
                                 opt As DriveOption,
                                 diskParams As FloppyDiskParams,
                                 outputType As ReadDiskOutputTypes,
                                 doubleStep As Boolean,
                                 trackRanges As List(Of (StartTrack As UShort, EndTrack As UShort)),
                                 heads As TrackHeads?,
                                 filePath2 As String,
                                 outputType2 As ReadDiskOutputTypes)

            Dim Opts As ReadOptions
            Try
                Opts = ReadDiskHelpers.BuildReadOptions(filePath,
                                        opt,
                                        diskParams,
                                        outputType,
                                        doubleStep,
                                        trackRanges,
                                        heads,
                                        CInt(_NumericRetries.Value),
                                        CInt(_NumericSeekRetries.Value),
                                        CInt(NumericRevs.Value),
                                        filePath2,
                                        outputType2)
            Catch ex As Exception
                HandleRunFailure(ex.Message)
                ApplyProcessState(ConsoleProcessRunner.ProcessStateEnum.Error)
                Return
            End Try

            Runner.RunAsync(Sub(Token) ReadCmd.Run(Opts, Token))
        End Sub

        Private Sub ToggleImageLocationControls()
            TableLayoutPanelMain.SuspendLayout()
            SetRowVisible(_GridIndex(GridRows.ImageFolder), DisplayImageFolder)
            TableLayoutPanelMain.ResumeLayout(True)
        End Sub

        Private Sub ToggleImageLocationVisible()
            Dim Visible = DisplayImageLocation

            _LabelLocation.Visible = Visible
            ComboImageLocation.Visible = Visible
            ComboExtensions2.Visible = Visible

            ToggleImageLocationControls()
        End Sub

        Private Sub ToggleRootFolderControls()
            Dim HasOutputFile As Boolean = Me.HasOutputFile

            If Not _LastFluxOutput.HasValue OrElse _LastFluxOutput.Value <> IsFluxOutput Then
                _LastFluxOutput = IsFluxOutput
                TableLayoutPanelMain.SuspendLayout()
                SetRowVisible(_GridIndex(GridRows.FileName), Not IsFluxOutput)
                SetRowVisible(_GridIndex(GridRows.Prefix), IsFluxOutput)
                SetRowVisible(_GridIndex(GridRows.RootFolder), IsFluxOutput)
                SetRowVisible(_GridIndex(GridRows.ImageFolder), DisplayImageFolder)
                SetRowVisible(_GridIndex(GridRows.FolderName), IsFluxOutput)
                TableLayoutPanelMain.ResumeLayout(True)

                UpdateSequenceButtonState()
            End If

            Dim CanChangeSettings = Me.CanChangeSettings

            TextBoxRootFolder.Enabled = CanChangeSettings
            TextBoxPrefixName.Enabled = CanChangeSettings
            'TextBoxImageFolder.Enabled = CanChangeSettings

            ButtonRootBrowse.Enabled = IsIdle AndAlso IsFluxOutput AndAlso Not HasOutputFile
            'ButtonImageFolderBrowse.Enabled = IsIdle AndAlso IsFluxOutput AndAlso Not HasOutputFile

            CheckBoxSaveLog.Enabled = IsIdle AndAlso IsFluxOutput AndAlso Not HasOutputFile
            CheckBoxSaveLog.Visible = IsFluxOutput
        End Sub

        Private Sub UpdateSequenceButtonState()
            Dim Enabled = IsToggleSequenceEnabled()

            ButtonToggleSequence.Enabled = Enabled
            ButtonToggleSequence2.Enabled = Enabled
        End Sub

#Region "Events"
        Private Sub ButtonConvert_Click(sender As Object, e As EventArgs) Handles ButtonConvert.Click
            If Not CheckFileNameEntered(CheckImageFile:=False) Then
                Exit Sub
            End If

            ConvertImage()
        End Sub

        Private Sub ButtonDetect_Click(sender As Object, e As EventArgs) Handles ButtonDetect.Click
            DoFormatDetection()
        End Sub

        Private Sub ButtonDiscard_Click(sender As Object, e As EventArgs) Handles ButtonDiscard.Click
            ClearProcessedImage(True, True)
        End Sub

        Private Sub ButtonImageFolderBrowse_Click(sender As Object, e As EventArgs) Handles ButtonImageFolderBrowse.Click
            ImageFolderBrowse()
        End Sub

        Private Sub ButtonImport_Click(sender As Object, e As EventArgs) Handles ButtonImport.Click
            If Not CheckFileNameEntered(CheckImageFile:=True) Then
                Exit Sub
            End If

            If IsFluxOutput Then
                If Not FinalizeFluxOutput() Then
                    Exit Sub
                End If

                If HasOutputFile2 Then
                    FinalizeImageOutput()
                    _TempFilePath2 = ""
                End If


                ClearProcessedImage(False, True)
                Exit Sub
            End If

            If Not HasOutputFile Then
                Exit Sub
            End If

            If CType(ButtonImport.SelectedValue, ConversionMode) = ConversionMode.Save Then
                ProcessSave()
            Else
                ProcessImport(_TempFilePath, GetNewFileName(_TempFilePath))
            End If
        End Sub

        Private Sub ButtonImport_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ButtonImport.SelectedIndexChanged, ButtonImportAndClose.SelectedIndexChanged
            If _ImportButtonNoEvent Then
                Exit Sub
            End If

            _ImportButtonNoEvent = True

            Dim Mode = CType(DirectCast(sender, SplitButton).SelectedValue, ConversionMode)

            ButtonImport.SelectedValue = Mode
            ButtonImportAndClose.SelectedValue = Mode
            App.UserState.Flux.Convert.ConversionMode = Mode

            _ImportButtonNoEvent = False

            InitializeHelpImportButtons(IsFluxOutput)
        End Sub

        Private Sub ButtonImportAndClose_Click(sender As Object, e As EventArgs) Handles ButtonImportAndClose.Click
            If Not CheckFileNameEntered(CheckImageFile:=True) Then
                Exit Sub
            End If

            If IsFluxOutput Then
                If Not FinalizeFluxOutput() Then
                    Exit Sub
                End If

                If HasOutputFile2 Then
                    FinalizeImageOutput()
                    _TempFilePath2 = ""
                End If

                CloseForm("", "")
                Exit Sub
            End If

            If CType(ButtonImportAndClose.SelectedValue, ConversionMode) = ConversionMode.Save Then
                SaveAndClose(DialogResult.OK)
            Else
                CloseForm(_TempFilePath, GetNewFileName(_TempFilePath))
            End If
        End Sub

        Private Sub ButtonPreview_Click(sender As Object, e As EventArgs) Handles ButtonPreview.Click
            PreviewImage()
        End Sub

        Private Sub ButtonRead_Click(sender As Object, e As EventArgs) Handles ButtonRead.Click
            If CancelIfRunning() Then
                Exit Sub
            End If

            If IsFluxOutput AndAlso Not CheckRootFolder() Then
                Exit Sub
            End If

            If CanRefine Then
                If Not ConfirmDiscardSecondaryImageForReprocess() Then
                    Exit Sub
                End If

                ReprocessImage()
            Else
                ProcessImage()
            End If
        End Sub

        Private Sub ButtonRefine_Click(sender As Object, e As EventArgs) Handles ButtonRefine.Click
            If IsFluxOutput AndAlso Not CheckRootFolder() Then
                Exit Sub
            End If

            RefineFromRaw()
        End Sub

        Private Sub ButtonRootBrowse_Click(sender As Object, e As EventArgs) Handles ButtonRootBrowse.Click
            RootFolderBrowse()
        End Sub

        Private Sub ButtonToggleSequence_Click(sender As Object, e As EventArgs) Handles ButtonToggleSequence.Click, ButtonToggleSequence2.Click
            DoToggleSequence()
        End Sub

        Private Sub ButtonVerify_Click(sender As Object, e As EventArgs) Handles ButtonVerify.Click
            If CancelIfRunning() Then
                Exit Sub
            End If

            VerifyImage()
        End Sub

        Private Sub CheckBoxSaveLog_CheckStateChanged(sender As Object, e As EventArgs) Handles CheckBoxSaveLog.CheckStateChanged
            If Not _Initialized Then
                Exit Sub
            End If

            SelectedDeviceState.SaveLog = CheckBoxSaveLog.Checked
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

            Dim PrevOption = _SelectedDriveOption
            _SelectedDriveOption = ComboDrives.SelectedValue

            If Not HasOutputFile Then
                PopulateImageFormats()
                ResetTrackGrid()
            Else
                RefreshTrackState(PrevOption, _SelectedDriveOption)
            End If

            RefreshFormState()
            RefreshWarningLabel()

            SelectedDeviceState.DriveId = _SelectedDriveOption.Id
        End Sub

        Private Sub ComboImageFormat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageFormat.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboImageFormatNoEvent Then
                Exit Sub
            End If

            If Not String.IsNullOrEmpty(_SelectedDriveOption?.Id) Then
                _SelectedDriveOption.SelectedFormat = SelectedDiskFormat
            End If

            If Not HasOutputFile OrElse Not IsFluxOutput Then
                ResetOutputTypes()
            End If

            ResetOutputTypes2()

            If Not HasOutputFile Then
                ResetTrackGrid()
            Else
                RefreshTrackState(_SelectedDriveOption, _SelectedDriveOption)
            End If

            RefreshFormState()
            RefreshWarningLabel()
        End Sub

        Private Sub ComboImageLocation_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageLocation.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboImageLocationNoEvent Then
                Exit Sub
            End If

            SelectedDeviceState.ImageLocation = SelectedImageLocation

            ToggleImageLocationControls()
        End Sub

        Private Sub ComboOutputType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboOutputType.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboOutputTypeNoEvent Then
                Exit Sub
            End If

            PopulateFileExtensions(ComboExtensions, SelectedOutputType)
            RefreshFormState()
            RefreshRevs()
            RefreshImportButtonItems(IsFluxOutput)

            SelectedDeviceState.OutputType = SelectedOutputType
        End Sub

        Private Sub ComboOutputType2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboOutputType2.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboOutputTypeNoEvent Then
                Exit Sub
            End If

            PopulateFileExtensions(ComboExtensions2, SelectedOutputType2)

            SelectedDeviceState.OutputType2 = SelectedOutputType2

            ToggleImageLocationVisible()
        End Sub

        Private Sub ConvertCmd_HardSectorsApplied(sender As Object, e As ConvertHardSectorsEventArgs) Handles ConvertCmd.HardSectorsApplied
            Runner.EmitOutputLine(FormatConvertHardSectorsAppliedLine(e))
        End Sub

        Private Sub ConvertCmd_Started(sender As Object, e As ConvertStartedEventArgs) Handles ConvertCmd.Started
            For Each Line In FormatConvertStartedLines(e)
                Runner.EmitOutputLine(Line)
            Next
        End Sub

        Private Sub ConvertCmd_SummaryReady(sender As Object, e As SectorSummaryReadyEventArgs) Handles ConvertCmd.SummaryReady
            For Each Line In FormatSectorSummaryLines(e.Grid)
                Runner.EmitOutputLine(Line)
            Next

            Runner.PostToUi(Sub() _Status.OnSummary(e.Grid))
        End Sub

        Private Sub ConvertCmd_TrackProcessed(sender As Object, e As TrackProcessedEventArgs) Handles ConvertCmd.TrackProcessed
            Runner.EmitOutputLine(FormatConvertTrackProcessedLine(e))

            Runner.PostToUi(Sub() _Status.OnConvertTrackProcessed(e, _OutputDoubleStep))
        End Sub

        Private Sub ConvertCmd_UnexpectedSectorIgnored(sender As Object, e As UnexpectedSectorEventArgs) Handles ConvertCmd.UnexpectedSectorIgnored
            Runner.EmitOutputLine(FormatConvertUnexpectedSectorLine(e))

            Runner.PostToUi(Sub() _Status.OnConvertUnexpectedSector(e, _OutputDoubleStep))
        End Sub

        Private Sub NumericRevs_ValueChanged(sender As Object, e As EventArgs) Handles NumericRevs.ValueChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _NumericRevsNoEvent Then
                Exit Sub
            End If

            If IsFluxOutput Then
                _CachedRevsFlux = NumericRevs.Value
            Else
                _CachedRevs = NumericRevs.Value
            End If
        End Sub

        Private Sub ReadCmd_HardSectorsDetected(sender As Object, e As HardSectorsDetectedEventArgs) Handles ReadCmd.HardSectorsDetected
            Runner.EmitOutputLine(FormatReadHardSectorsLine(e))
        End Sub

        Private Sub ReadCmd_Started(sender As Object, e As ReadStartedEventArgs) Handles ReadCmd.Started
            For Each Line In FormatReadStartedLines(e)
                Runner.EmitOutputLine(Line)
            Next
        End Sub
        Private Sub ReadCmd_SummaryReady(sender As Object, e As SectorSummaryReadyEventArgs) Handles ReadCmd.SummaryReady
            For Each Line In FormatSectorSummaryLines(e.Grid)
                Runner.EmitOutputLine(Line)
            Next

            Runner.PostToUi(Sub() _Status.OnSummary(e.Grid))
        End Sub

        Private Sub ReadCmd_TrackGaveUp(sender As Object, e As ReadTrackGaveUpEventArgs) Handles ReadCmd.TrackGaveUp
            Runner.EmitOutputLine(FormatReadTrackGaveUpLine(e))

            Runner.PostToUi(Sub() _Status.OnReadTrackGaveUp(e, _OutputDoubleStep))
        End Sub

        Private Sub ReadCmd_TrackProcessed(sender As Object, e As TrackProcessedEventArgs) Handles ReadCmd.TrackProcessed
            Runner.EmitOutputLine(FormatReadTrackProcessedLine(e))

            Runner.PostToUi(Sub() _Status.OnReadTrackProcessed(e, _OutputDoubleStep))
        End Sub

        Private Sub ReadCmd_UnexpectedSectorIgnored(sender As Object, e As UnexpectedSectorEventArgs) Handles ReadCmd.UnexpectedSectorIgnored
            Runner.EmitOutputLine(FormatReadUnexpectedSectorLine(e))

            Runner.PostToUi(Sub() _Status.OnReadUnexpectedSector(e, _OutputDoubleStep))
        End Sub

        Private Sub ReadDiskForm_CheckChanged(sender As Object, Checked As Boolean, Side As Byte) Handles Me.CheckChanged
            RefreshFormState()
        End Sub

        Private Sub ReadDiskForm_SelectionChanged(sender As Object, Track As UShort, Side As Byte, Enabled As Boolean) Handles Me.SelectionChanged
            RefreshFormState()
        End Sub

        Private Sub Runner_ProcessStateChanged(state As ConsoleProcessRunner.ProcessStateEnum) Handles Runner.ProcessStateChanged
            ApplyProcessState(state)
        End Sub

        Private Sub TextBox_Enter(sender As Object, e As EventArgs) Handles TextBoxFileName.Enter, TextBoxPrefixName.Enter, TextBoxFolderName.Enter
            _LastSequenceTextBox = DirectCast(sender, TextBoxBase)

            UpdateSequenceButtonState()
        End Sub

        Private Sub TextBox_KeyUp(sender As Object, e As KeyEventArgs) Handles TextBoxFileName.KeyUp, TextBoxPrefixName.KeyUp, TextBoxFolderName.KeyUp
            UpdateSequenceButtonState()
        End Sub

        Private Sub TextBox_Leave(sender As Object, e As EventArgs) Handles TextBoxPrefixName.Leave, TextBoxFolderName.Leave, TextBoxFileName.Leave
            If Me.ActiveControl Is ButtonToggleSequence OrElse Me.ActiveControl Is ButtonToggleSequence2 Then
                Return
            End If

            _LastSequenceTextBox = Nothing

            UpdateSequenceButtonState()
        End Sub

        Private Sub TextBox_MouseUp(sender As Object, e As MouseEventArgs) Handles TextBoxFileName.MouseUp, TextBoxPrefixName.MouseUp, TextBoxFolderName.MouseUp
            UpdateSequenceButtonState()
        End Sub

        Private Sub TextBoxFileName_TextChanged(sender As Object, e As EventArgs) Handles TextBoxFileName.TextChanged, TextBoxPrefixName.TextChanged, TextBoxFolderName.TextChanged
            UpdateSequenceButtonState()
            FileNameChangePostProcess()
        End Sub

        Private Sub TextBoxFileName_Validating(sender As Object, e As CancelEventArgs) Handles TextBoxFileName.Validating, TextBoxPrefixName.Validating
            Dim tb As TextBox = DirectCast(sender, TextBox)

            Dim errorMessage As String = ValidateFileNameWithPlaceholders(tb.Text)

            If errorMessage <> "" Then
                MessageBox.Show(errorMessage, My.Resources.Label_InvalidFilename, MessageBoxButtons.OK, MessageBoxIcon.Error)
                e.Cancel = True
                tb.SelectAll()
                Return
            End If

            tb.Text = tb.Text.Trim()

            FileNameChangePostProcess()
        End Sub

        Private Sub TextBoxFolderName_Validating(sender As Object, e As CancelEventArgs) Handles TextBoxFolderName.Validating
            Dim tb As TextBox = DirectCast(sender, TextBox)

            Dim errorMessage As String = ValidatePathNameWithPlaceholders(tb.Text)

            If errorMessage <> "" Then
                MessageBox.Show(errorMessage, My.Resources.Label_InvalidFilename, MessageBoxButtons.OK, MessageBoxIcon.Error)
                e.Cancel = True
                tb.SelectAll()
                Return
            End If

            tb.Text = tb.Text.Trim().Replace("/"c, "\"c)

            FileNameChangePostProcess()
        End Sub

        Private Sub TextBoxImageFolder_Click(sender As Object, e As EventArgs) Handles TextBoxImageFolder.Click
            If TextBoxImageFolder.Enabled AndAlso String.IsNullOrEmpty(ImageFolderInput) Then
                ImageFolderBrowse()
            End If
        End Sub

        Private Sub TextBoxRootFolder_Click(sender As Object, e As EventArgs) Handles TextBoxRootFolder.Click
            If TextBoxRootFolder.Enabled AndAlso String.IsNullOrEmpty(RootFolderInput) Then
                RootFolderBrowse()
            End If
        End Sub
#End Region
    End Class
End Namespace
