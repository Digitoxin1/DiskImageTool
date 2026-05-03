Imports System.ComponentModel
Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports Greaseweazle.Actions
Imports Greaseweazle.Shared
Imports Greaseweazle.Tools

Namespace Flux.Greaseweazle
    Public Class ReadDiskForm
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
        Private Const FLUX_WILDCARD As String = "*.raw"
        Private Shared _CachedFileNameTemplate As String = ""
        Private Shared _CachedFolderNameTemplate As String = ""
        Private Shared _CachedPrefixNameTemplate As String = DEFAULT_RAW_FILE_NAME
        Private ReadOnly _DrivesAvailable As Boolean = False
        Private ReadOnly _GridIndex As New Dictionary(Of GridRows, Integer)
        Private ReadOnly _Initialized As Boolean = False
        Private ReadOnly _KryofluxAvailable As Boolean = False
        Private ReadOnly _KryofluxStatus As Flux.Kryoflux.TrackStatus
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
        Private _OutputDoubleStep As Boolean = False
        Private _TempFilePath As String = ""
        Private _TempFilePath2 As String = ""
        Private _SelectedDriveOption As DriveOption

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
            SetRootFolder(App.AppSettings.Greaseweazle.FluxRootPath)
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

        Private Function BuildReadOptions(filePath As String,
                                          opt As DriveOption,
                                          diskParams As FloppyDiskParams,
                                          outputType As ReadDiskOutputTypes,
                                          doubleStep As Boolean,
                                          trackRanges As List(Of (StartTrack As UShort, EndTrack As UShort)),
                                          heads As TrackHeads?,
                                          retries As Integer,
                                          seekRetries As Integer,
                                          revs As Integer,
                                          filePath2 As String,
                                          outputType2 As ReadDiskOutputTypes) As ReadOptions

            Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(diskParams.Format)
            Dim Format As String = Nothing
            Dim Raw As Boolean = False
            Dim AdjustSpeed As Double? = Nothing

            If outputType = ReadDiskOutputTypes.IMA OrElse ImageFormat <> GreaseweazleImageFormat.None Then
                Format = GreaseweazleImageFormatString(ImageFormat)
            End If

            Dim FileNameWithOpts As String = filePath
            Dim FileName2WithOpts As String = ""

            If outputType = ReadDiskOutputTypes.HFE Then
                AdjustSpeed = 60.0 / diskParams.RPM
                Raw = True
                FileNameWithOpts &= "::bitrate=" & CInt(diskParams.BitRateKbps)

            ElseIf outputType = ReadDiskOutputTypes.RAW Then
                If diskParams.Format <> FloppyDiskFormat.FloppyUnknown Then
                    AdjustSpeed = 60.0 / diskParams.RPM
                End If
                Raw = True

                If Not String.IsNullOrEmpty(filePath2) AndAlso outputType2 <> ReadDiskOutputTypes.None Then
                    FileName2WithOpts = filePath2

                    If outputType2 = ReadDiskOutputTypes.HFE Then
                        FileName2WithOpts &= "::bitrate=" & CInt(diskParams.BitRateKbps)
                    End If
                End If
            End If

            If trackRanges Is Nothing Then
                trackRanges = New List(Of (StartTrack As UShort, EndTrack As UShort)) From {
                    (0, CUShort(Math.Max(0, opt.Tracks - 1)))
                }
            End If

            If Not heads.HasValue Then
                If diskParams.Format = FloppyDiskFormat.FloppyUnknown Then
                    heads = TrackHeads.both
                Else
                    heads = If(diskParams.BPBParams.NumberOfHeads > 1, TrackHeads.both, TrackHeads.head0)
                End If
            End If

            Dim TrackSet = BuildUserSpec(trackRanges, heads, doubleStep, True)

            Dim Drive = MakeDriveSpec(opt.Id)

            Dim Opts = New ReadOptions With {
                .FileName = FileNameWithOpts,
                .Format = Format,
                .TrackSet = TrackSet,
                .Revs = revs,
                .Raw = Raw,
                .AdjustSpeed = AdjustSpeed,
                .Retries = retries,
                .SeekRetries = seekRetries,
                .Live = True,
                .Device = ConfiguredDevice(),
                .Drive = Drive
            }

            If Not String.IsNullOrEmpty(FileName2WithOpts) Then
                Opts.AdditionalFiles = New List(Of String) From {FileName2WithOpts}
            End If

            Return Opts
        End Function

        Private Function BuildRefineConvertOptions(inputFilePath As String, diskParams As FloppyDiskParams, doubleStep As Boolean) As ConvertOptions
            Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(diskParams.Format)
            Dim Format = GreaseweazleImageFormatString(ImageFormat)

            Dim TrackSet As New TrackSetSpec

            If doubleStep Then
                TrackSet.Step = 2
            End If

            Return New ConvertOptions With {.InputFile = inputFilePath, .Format = Format, .TrackSet = TrackSet}
        End Function

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

        Private Function CheckFileNameEntered(CheckImageFile As Boolean) As Boolean
            If IsFluxOutput Then
                If String.IsNullOrEmpty(RootFolderInput) Then
                    MsgBox(My.Resources.Dialog_SelectRootFolder, MsgBoxStyle.Exclamation)
                    ButtonRootBrowse.Focus()
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

        Private Sub VerifyImage()
            If Not HasOutputFile OrElse Not IsFluxOutput Then
                Exit Sub
            End If

            Dim DiskParams = SelectedDiskParams

            If Not DiskParams.HasValue OrElse DiskParams.Value.IsNonImage Then
                Exit Sub
            End If

            If Not _KryofluxAvailable Then
                Exit Sub
            End If

            Dim TrackCount As Integer

            If _SelectedDriveOption Is Nothing OrElse _SelectedDriveOption.Type = FloppyDriveType.DriveUnknown Then
                TrackCount = If(DiskParams.Value.DriveType = FloppyDriveType.Drive525DoubleDensity, GreaseweazleSettings.MAX_TRACKS_525DD, GreaseweazleSettings.MAX_TRACKS)
            Else
                TrackCount = _SelectedDriveOption.Tracks
            End If

            Dim Args As (Arguments As String, SingleSide As Boolean)
            Try
                Args = Flux.Kryoflux.GenerateCommandLineImport(_TempFilePath, "", DiskParams.Value, TrackCount, _OutputDoubleStep, Flux.Kryoflux.CommandLineBuilder.LogMask.Format)
            Catch ex As Exception
                HandleRunFailure(ex.Message)
                Exit Sub
            End Try

            ClearLogAndStatus()
            TrackStatus = _KryofluxStatus
            TrackStatus.Clear()
            ResetTrackGrid(ResetSelected:=False)

            InitLogFilePath(GetVerifyLogPath())

            Try
                Process.StartAsync(Flux.Kryoflux.Settings().AppPath, Args.Arguments)
            Catch ex As Exception
                HandleRunFailure(ex.Message)
            End Try
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

            If Not FinalizeFluxTempFolder(tempFolder, destFolder, prefix) Then
                MsgBox(My.Resources.Message_SaveFluxSetError, MsgBoxStyle.Exclamation)
                Return False
            End If

            UpdateVerifyLogPaths(destFolder)
            _TempFilePath = FluxGetOutputFilePath()

            Return True
        End Function

        Private Function FinalizeFluxTempFolder(tempFolderPath As String, destinationFolderPath As String, Optional prefix As String = "") As Boolean
            If String.IsNullOrWhiteSpace(tempFolderPath) OrElse String.IsNullOrWhiteSpace(destinationFolderPath) Then
                Return False
            End If

            Dim tempFull = IO.Path.GetFullPath(tempFolderPath.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar))
            Dim destFull = IO.Path.GetFullPath(destinationFolderPath.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar))

            If Not IO.Directory.Exists(tempFull) Then
                Return False
            End If

            ' No-op if same folder
            If String.Equals(tempFull, destFull, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If

            Try
                ' Fast path: destination does not exist -> rename temp folder to destination
                If Not IO.Directory.Exists(destFull) Then
                    IO.Directory.Move(tempFull, destFull)
                    Return True
                End If

                ' If prefix was not provided, try to infer it from temp set
                If String.IsNullOrWhiteSpace(prefix) Then
                    Dim tempRaw = GetFirstRawInFolder(tempFull)
                    If Not String.IsNullOrWhiteSpace(tempRaw) Then
                        Dim info = GetFluxSetInfoRaw(tempRaw)
                        If info.Result Then
                            prefix = info.Prefix
                        End If
                    End If
                End If

                ' Destination exists -> delete existing raw files with same prefix
                If Not String.IsNullOrWhiteSpace(prefix) Then
                    For Each dstRaw In IO.Directory.GetFiles(destFull, prefix & FLUX_WILDCARD, IO.SearchOption.TopDirectoryOnly)
                        IO.File.Delete(dstRaw)
                    Next
                End If

                ' Move all files from temp to destination (overwrite)
                For Each srcFile In IO.Directory.GetFiles(tempFull, "*", IO.SearchOption.TopDirectoryOnly)
                    Dim dstFile = IO.Path.Combine(destFull, IO.Path.GetFileName(srcFile))

                    Dim created = IO.File.GetCreationTime(srcFile)
                    Dim modified = IO.File.GetLastWriteTime(srcFile)

                    If IO.File.Exists(dstFile) Then
                        IO.File.Delete(dstFile)
                    End If

                    IO.File.Move(srcFile, dstFile)

                    IO.File.SetCreationTime(dstFile, created)
                    IO.File.SetLastWriteTime(dstFile, modified)
                Next

                ' Delete temp folder after move
                If IO.Directory.Exists(tempFull) Then
                    IO.Directory.Delete(tempFull, True)
                End If

                Return True
            Catch ex As Exception
                Debug.WriteLine($"FinalizeFluxTempFolder failed: {ex.Message}")
                Return False
            End Try
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

        Private Function FluxGetFirstTrackFileName(Prefix As String) As String
            Return FluxGetTrackFileName(Prefix, 0, 0)
        End Function

        Private Function FluxGetFirstTrackFileName() As String
            Return FluxGetFirstTrackFileName(FluxGetPrefix())
        End Function

        Private Function FluxGetFolderPath() As String
            Dim Folder = GetOutputFolderName(FolderNameInput)

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

        Private Function FluxGetTrackFileName(Prefix As String, Track As Integer, Side As Integer) As String
            Return Prefix & Track.ToString("00") & "." & Side.ToString() & ".raw"
        End Function

        Private Function FluxGetWildcardFileName() As String
            Return FluxGetPrefix() & FLUX_WILDCARD
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
            Dim FileName = GetOutputFolderName(FolderNameInput) & Extension
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

        Private Function GetOutputFolderName(FolderName As String) As String
            If ContainsPlaceholder(FolderName) Then
                FolderName = StripAngleBrackets(FolderName)
            End If

            Return FolderName
        End Function

        Private Function GetVerifyLogFileName() As String
            Dim KfLogName = Flux.Kryoflux.Settings().LogFileName

            If String.IsNullOrEmpty(KfLogName) Then
                Return ""
            End If

            Dim GwLogName = Settings.LogFileName
            If String.Equals(KfLogName, GwLogName, StringComparison.OrdinalIgnoreCase) Then
                Dim Base = IO.Path.GetFileNameWithoutExtension(KfLogName)
                Dim Ext = IO.Path.GetExtension(KfLogName)
                KfLogName = Base & ".verify" & Ext
            End If

            Return KfLogName
        End Function

        Private Function GetVerifyLogPath() As String
            If Not CheckBoxSaveLog.Checked Then
                Return ""
            End If

            Dim Name = GetVerifyLogFileName()

            If String.IsNullOrEmpty(Name) Then
                Return ""
            End If

            Return IO.Path.Combine(IO.Path.GetDirectoryName(_TempFilePath), Name)
        End Function

        Private Sub UpdateVerifyLogPaths(destFolder As String)
            Dim LogName = GetVerifyLogFileName()

            If String.IsNullOrEmpty(LogName) Then
                Exit Sub
            End If

            Dim LogPath = IO.Path.Combine(destFolder, LogName)
            If Not IO.File.Exists(LogPath) Then
                Exit Sub
            End If

            Try
                Dim Content = IO.File.ReadAllText(LogPath)
                Content = RemovePathFromLog(Content, IO.Path.GetFileName(destFolder))
                IO.File.WriteAllText(LogPath, Content)
            Catch ex As Exception
                Debug.WriteLine($"UpdateVerifyLogPaths failed: {ex.Message}")
            End Try
        End Sub

        Private Sub ImageFolderBrowse()
            Dim FolderName = BrowseFolderVista(ImageFolderInput, Me.Handle)
            If FolderName <> "" Then
                SetImageFolder(FolderName)
            End If
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

        Private Function NormalizeFluxFolder(selectedPath As String, rootPath As String) As String
            If String.IsNullOrWhiteSpace(selectedPath) OrElse String.IsNullOrWhiteSpace(rootPath) Then
                Return ""
            End If

            ' Normalize both paths
            Dim rootFull = IO.Path.GetFullPath(rootPath.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar))
            Dim selFull = IO.Path.GetFullPath(selectedPath.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar))

            ' Same as root → empty
            If String.Equals(rootFull, selFull, StringComparison.OrdinalIgnoreCase) Then
                Return ""
            End If

            ' Subfolder of root → relative path
            If selFull.StartsWith(rootFull & IO.Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) Then
                Return selFull.Substring(rootFull.Length + 1)
            End If

            ' Otherwise → absolute path
            Return selFull
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
                Dim FileName = GetOutputFolderName(FolderNameInput)

                If String.IsNullOrEmpty(FileName) Then
                    Caption = ""
                Else
                    Caption = IO.Path.GetFileName(FileName)
                End If
            Else
                Caption = GetOutputFolderName(FileNameInput)
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
            _OutputDoubleStep = UseDoubleStep(_SelectedDriveOption.Type, DiskParams.Value.Format)

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
            FolderNameInput = NormalizeFluxFolder(SourceFolder, RootFolderInput)
            TextBoxPrefixName.Text = Info.Prefix

            Dim DiskParams = SelectedDiskParams

            If Not DiskParams.HasValue Then
                MsgBox(My.Resources.Dialog_DetectFormatError, MsgBoxStyle.Exclamation)
                DeleteTempFolder(TempFolder)
                Exit Sub
            End If

            _TempFilePath = IO.Path.Combine(TempFolder, FluxGetFirstTrackFileName(Info.Prefix))
            _OutputDoubleStep = _SelectedDriveOption IsNot Nothing AndAlso UseDoubleStep(_SelectedDriveOption.Type, DetectedFormat)

            TrackStatus = _Status
            TrackStatus.Clear()
            ResetTrackGrid()

            Dim Opts As ConvertOptions
            Try
                Opts = BuildRefineConvertOptions(_TempFilePath, DiskParams.Value, _OutputDoubleStep)
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

            Dim NonImageFormat As Boolean = Not DiskParams.HasValue OrElse DiskParams.Value.IsNonImage
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

            Dim PrevDoubleStep = UseDoubleStep(PrevOption.Type, DiskParams.Value.Format)
            Dim Doublestep = UseDoubleStep(CurrentOption.Type, DiskParams.Value.Format)

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

            _OutputDoubleStep = UseDoubleStep(_SelectedDriveOption.Type, DiskParams.Value.Format)

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

        Private Function SetControlWidth(Control As Control, Optional ExtraPadding As Integer = 2) As Integer
            Dim TextWidth = TextRenderer.MeasureText(Control.Text, Control.Font).Width

            Control.Width = TextWidth

            Return TextWidth + Control.Padding.Horizontal + Control.Margin.Horizontal + ExtraPadding
        End Function

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
            App.AppSettings.Greaseweazle.FluxRootPath = Path
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
                Dim ParentFolder As String = GetOutputFolderName(FolderNameInput)
                DisplayFileName = IO.Path.Combine(ParentFolder, FLUX_WILDCARD)
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
                Opts = BuildReadOptions(filePath,
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

        Private Function UseDoubleStep(DriveType As FloppyDriveType, Format As FloppyDiskFormat) As Boolean
            Dim ImageParams As FloppyDiskParams = FloppyDiskFormatGetParams(Format)

            Return ImageParams.IsStandard AndAlso ImageParams.DriveType = FloppyDriveType.Drive525DoubleDensity AndAlso DriveType = FloppyDriveType.Drive525HighDensity
        End Function

#Region "Form Init"
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
            ColWidth = Math.Max(ColWidth, SetControlWidth(_LabelDrive))

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
            ColWidth = Math.Max(ColWidth, SetControlWidth(_LabelFileName))

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
            ColWidth = Math.Max(ColWidth, SetControlWidth(_LabelFolderName))

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
            ColWidth = Math.Max(ColWidth, SetControlWidth(_LabelImageFormat))

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
            ColWidth = Math.Max(ColWidth, SetControlWidth(_LabelImageFolder))

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
            ColWidth = Math.Max(ColWidth, SetControlWidth(_LabelPrefixName))

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
            ColWidth = Math.Max(ColWidth, SetControlWidth(_LabelRootFolder))

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
                .Margin = New Padding(18, 0, 6, 0),
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
#End Region

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

        Private Sub Process_DataReceived(Data As String) Handles Process.OutputDataReceived, Process.ErrorDataReceived
            AppendLogLine(Data)
            _KryofluxStatus.ProcessOutputLineRead(Data, _OutputDoubleStep)
        End Sub

        Private Sub Process_ProcessStateChanged(state As ConsoleProcessRunner.ProcessStateEnum) Handles Process.ProcessStateChanged
            Select Case state
                Case ConsoleProcessRunner.ProcessStateEnum.Aborted
                    _KryofluxStatus.UpdateTrackStatusAborted()

                Case ConsoleProcessRunner.ProcessStateEnum.Completed
                    If _KryofluxStatus.TrackFound Then
                        _KryofluxStatus.UpdateTrackStatusComplete(_OutputDoubleStep)
                    Else
                        _KryofluxStatus.UpdateTrackStatusError()
                    End If

                Case ConsoleProcessRunner.ProcessStateEnum.Error
                    _KryofluxStatus.UpdateTrackStatusError()
            End Select

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
