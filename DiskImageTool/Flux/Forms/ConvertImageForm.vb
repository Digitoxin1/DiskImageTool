Imports System.Collections.Concurrent
Imports System.ComponentModel
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux
    Public Class ConvertImageForm
        Inherits BaseFluxForm

        Private WithEvents ButtonDiscard As Button
        Private WithEvents ButtonImport As Button
        Private WithEvents ButtonModifications As Button
        Private WithEvents ButtonOpen As Button
        Private WithEvents ButtonPreview As Button
        Private WithEvents ButtonProcess As Button
        Private WithEvents ButtonTrackLayout As Button
        Private WithEvents CheckBoxDoublestep As CheckBox
        Private WithEvents CheckBoxExtendedLogging As CheckBox
        Private WithEvents CheckBoxSaveLog As CheckBox
        Private WithEvents ComboDevices As ComboBox
        Private WithEvents ComboExtensions As ComboBox
        Private WithEvents ComboImageFormat As ComboBox
        Private WithEvents ComboOutputSource As ComboBox
        Private WithEvents ComboOutputType As ComboBox
        Private WithEvents ConsoleTimer As Timer
        Private WithEvents TextBoxFileName As TextBox
        Private ReadOnly _ConsoleQueue As New ConcurrentQueue(Of String)
        Private ReadOnly _Initialized As Boolean = False
        Private ReadOnly _LaunchedFromDialog As Boolean = False
        Private ReadOnly _OutputImages As OutputImages
        Private ReadOnly _UserState As Settings.UserStateFlux
        Private _ComboDevicesNoEvent As Boolean = False
        Private _ComboExtensionsNoEvent As Boolean = False
        Private _ComboOutputSourceNoEvent As Boolean = False
        Private _ComboOutputTypeNoEvent As Boolean = False
        Private _DoubleStep As Boolean = False
        Private _InputFilePath As String = ""
        Private _OutputFilePath As String = ""
        Private _SelectedDevice As IDevice = Nothing
        Private _SelectedSource As IDevice = Nothing
        Private _SideCount As Integer
        Private _TrackCount As Integer
        Private _FluxHeaders As Dictionary(Of TrackSide, Dictionary(Of String, String))
        Private _TrackLayoutExists As Boolean = False
        Private CheckBox86FSurfaceData As CheckBox
        Private CheckBoxRemaster As CheckBox
        Private LabelOutputSource As Label
        Private LabelOutputType As Label

        Public Event ImportProcess(File As String, NewFilename As String)

        Friend Sub New(TempPath As String, FilePath As String, FluxSetinfo As FluxSetInfo, LaunchedFromDialog As Boolean)
            MyBase.New("")

            Me.AllowDrop = True

            ConsoleTimer = New Timer With {
                .Interval = 10
            }

            _UserState = App.UserState.Flux
            _LaunchedFromDialog = LaunchedFromDialog
            _InputFilePath = FilePath
            _OutputImages = New OutputImages(TempPath)
            _TrackCount = FluxSetinfo.TrackCount
            _SideCount = FluxSetinfo.SideCount
            _FluxHeaders = FluxSetinfo.Headers

            InitializeControls()

            If LaunchedFromDialog Then
                ButtonImport.DialogResult = DialogResult.Retry
            End If

            RefreshRemasterState()
            InitializeDevice(True)

            _Initialized = True
        End Sub

        Public ReadOnly Property OutputFilePath As String
            Get
                Return _OutputFilePath
            End Get
        End Property

        Public Function GetNewFileName() As String
            If String.IsNullOrEmpty(_OutputFilePath) Then
                Return ""
            End If

            Dim Extension = IO.Path.GetExtension(_OutputFilePath)

            Return TextBoxFileName.Text & Extension
        End Function

        Protected Overrides Sub OnAfterBaseFormClosing(e As FormClosingEventArgs)
            If Me.DialogResult = DialogResult.OK OrElse Me.DialogResult = DialogResult.Retry Then
                MoveLogs()
                SetOutputFilePath()
            End If
            _OutputImages.ClearImages()
        End Sub

        Private Function AllowSectorImage() As Boolean
            If ComboDevices.SelectedIndex = -1 Then
                Return False
            End If

            Dim DiskParams = SelectedDiskParams()

            If Not DiskParams.HasValue Then
                Return False
            End If

            Dim SelectedDevice As IDevice = CType(ComboDevices.SelectedItem, IDevice)

            Select Case SelectedDevice.Device
                Case IDevice.FluxDevice.Greaseweazle
                    Dim imageFormat = Greaseweazle.GreaseweazleImageFormatFromFloppyDiskFormat(DiskParams.Value.Format)
                    Return (imageFormat <> Greaseweazle.GreaseweazleImageFormat.None)

                Case Else
                    Return True
            End Select
        End Function

        Private Sub AutoSaveLogFile()
            If String.IsNullOrEmpty(_InputFilePath) Then
                Exit Sub
            End If

            If String.IsNullOrEmpty(LogFileName) Then
                Exit Sub
            End If

            If _SelectedDevice Is Nothing Then
                Exit Sub
            End If

            _OutputImages.StoreLogFile(_SelectedDevice.Device, LogFileName, TextBoxConsole.Text, LogStripPath)
        End Sub

        Private Function CanAcceptDrop(paths As IEnumerable(Of String)) As Boolean
            Dim SelectedDevice As IDevice = CType(ComboDevices.SelectedItem, IDevice)
            Dim AllowSCP As Boolean = SelectedDevice.InputTypeSupported(InputFileTypeEnum.scp)

            For Each path In paths
                If IsValidFluxImport(path, AllowSCP).Result Then
                    Return True
                End If

                Exit For
            Next

            Return False
        End Function

        Private Sub ClearLoadedImage()
            TextBoxFileName.Text = ""
            _InputFilePath = ""
            _FluxHeaders = Nothing
            ComboImageFormat.SelectedIndex = 0
            CheckBoxRemaster.Checked = False
            _TrackLayoutExists = False
            SetTiltebarText()
            RefreshButtonState()
        End Sub

        Private Sub ClearProcessedImage()
            TextBoxConsole.Clear()
            ClearStatusBar()
            GridReset(_TrackCount, _SideCount, _FluxHeaders)

            TrackStatus.Clear()
        End Sub

        Private Sub DisplayModifications()
            If String.IsNullOrEmpty(_InputFilePath) Then
                Exit Sub
            End If

            If Not _OutputImages.HasPcImgCnvImage Then
                Exit Sub
            End If

            Dim ImageInfo = _OutputImages.Images(IDevice.FluxDevice.PcImgCnv)
            Dim ImageData = New ImageData(ImageInfo.FilePath)

            If ImageData.InvalidImage Then
                Exit Sub
            End If

            Dim Disk = DiskImageLoadFromImageData(ImageData)

            If Not Disk.Image.IsBitstreamImage Then
                Exit Sub
            End If

            Dim FilePath = IO.Path.GetDirectoryName(_InputFilePath)
            Dim FileExt = IO.Path.GetExtension(ImageInfo.FilePath)

            Dim FileName As String = TextBoxFileName.Text & FileExt

            DisplayReportModifications(Disk, FileName, FilePath)
        End Sub

        Private Function GenerateCommandLine(InputFilePath As String,
                                             OutputFilePath As String,
                                             Device As IDevice.FluxDevice,
                                             OutputType As ImageImportOutputTypes,
                                             DiskParams As FloppyDiskParams?) As (Arguments As String, Suffix As String)

            Dim Response As (Arguments As String, Suffix As String)
            Response.Arguments = ""
            Response.Suffix = ""

            Select Case Device
                Case IDevice.FluxDevice.Greaseweazle
                    Response.Arguments = Greaseweazle.GenerateCommandLineImport(InputFilePath, OutputFilePath, DiskParams.Value, OutputType, _DoubleStep)

                Case IDevice.FluxDevice.Kryoflux
                    Dim LogLevel As Kryoflux.CommandLineBuilder.LogMask = 0

                    If CheckBoxExtendedLogging.Checked Then
                        LogLevel = Kryoflux.CommandLineBuilder.LogMask.Read Or Kryoflux.CommandLineBuilder.LogMask.Cell
                    End If

                    Dim KryofluxResponse = Kryoflux.GenerateCommandLineImport(InputFilePath, OutputFilePath, DiskParams.Value, _TrackCount, _DoubleStep, LogLevel)
                    If KryofluxResponse.SingleSide Then
                        Response.Suffix = "s0"
                    End If
                    Response.Arguments = KryofluxResponse.Arguments

                Case IDevice.FluxDevice.PcImgCnv
                    Response.Arguments = PcImgCnv.GenerateCommandLineImport(InputFilePath, OutputFilePath, CheckBox86FSurfaceData.Checked, CheckBoxRemaster.Enabled AndAlso CheckBoxRemaster.Checked)
            End Select

            Return Response
        End Function

        Private Sub GenerateTrackData()
            If String.IsNullOrEmpty(_InputFilePath) Then
                Exit Sub
            End If

            If Not _OutputImages.HasPcImgCnvImage Then
                Exit Sub
            End If

            Dim ImageInfo = _OutputImages.Images(IDevice.FluxDevice.PcImgCnv)
            Dim ImageData = New ImageData(ImageInfo.FilePath)

            If ImageData.InvalidImage Then
                Exit Sub
            End If

            Dim Disk = DiskImageLoadFromImageData(ImageData)

            If Not Disk.Image.IsBitstreamImage Then
                Exit Sub
            End If

            Dim FilePath = IO.Path.GetDirectoryName(_InputFilePath)
            Dim FileExt = IO.Path.GetExtension(ImageInfo.FilePath)

            Dim FileName As String = TextBoxFileName.Text & FileExt

            GenerateTrackLayout(Disk, FileName, FilePath)
            RefreshRemasterState()
            RefreshButtonState()
        End Sub

        Private Function GetInputFileType() As InputFileTypeEnum
            If String.IsNullOrEmpty(_InputFilePath) Then
                Return InputFileTypeEnum.none
            End If

            Dim Ext = IO.Path.GetExtension(_InputFilePath).ToLower()

            Select Case Ext
                Case ".scp"
                    Return InputFileTypeEnum.scp
                Case ".raw"
                    Return InputFileTypeEnum.raw
                Case Else
                    Return InputFileTypeEnum.sectorImage
            End Select
        End Function

        Private Function GetSelectedDeviceState() As Settings.UserStateFluxConvertDevice
            If _SelectedDevice Is Nothing Then
                Return Nothing
            End If

            Return _UserState.Convert.Device(_SelectedDevice.Device)
        End Function

        Private Sub ImagePostProcessCompleted()
            If Not TrackStatus.TrackFound Then
                Exit Sub
            End If

            TrackStatus.UpdateTrackStatusComplete(_DoubleStep)

            If CheckBoxSaveLog.Checked Then
                AutoSaveLogFile()
            End If
            _OutputImages.StorePendingImage(GetState)
            PopulateOutputSourceCombo()
            RefreshButtonState()
        End Sub

        Private Sub InitializeControls()
            Dim LabelDevice As New Label With {
                .Text = My.Resources.Label_Device,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            ComboDevices = New ComboBox With {
                .Anchor = AnchorStyles.Left,
                .Width = 100
            }

            CheckBoxSaveLog = New CheckBox With {
                .Text = My.Resources.Label_AutoSaveLog,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            CheckBox86FSurfaceData = New CheckBox With {
                .Text = My.Resources.Label_86FSurfaceData,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3),
                .Visible = False
            }

            CheckBoxRemaster = New CheckBox With {
                .Text = My.Resources.Label_Remaster,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3),
                .Visible = False
            }

            CheckBoxExtendedLogging = New CheckBox With {
                .Text = My.Resources.Label_ExtendedLogOutput,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3),
                .Visible = False
            }

            Dim LabelFileName As New Label With {
                .Text = My.Resources.Label_FileName,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
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
                .Text = My.Resources.Label_ImageFormat,
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
                    .Anchor = AnchorStyles.Left,
                    .Width = 175
                }

            CheckBoxDoublestep = New CheckBox With {
                .Text = My.Resources.Label_DoubleStep,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
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
                .Text = My.Resources.Label_ReProcess,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonDiscard = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Discard,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonImport = New Button With {
                .Anchor = AnchorStyles.Right,
                .Margin = New Padding(6, 0, 6, 0),
                .Text = My.Resources.Label_Import,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .TabIndex = 2
            }

            ButtonOpen = New Button With {
                .Margin = New Padding(15, 3, 3, 3),
                .Text = WithoutHotkey(My.Resources.Menu_Open),
                .MinimumSize = New Size(75, 0),
                .AutoSize = True
            }

            ButtonTrackLayout = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Tracklayout,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Visible = False
            }

            ButtonModifications = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Modifications,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Visible = False
            }

            LabelOutputSource = New Label With {
                .Text = "Source",
                .Anchor = AnchorStyles.Right,
                .AutoSize = True,
                .TabIndex = 0
            }

            ComboOutputSource = New ComboBox With {
                .Anchor = AnchorStyles.Right,
                .Width = 100,
                .Margin = New Padding(3, 0, 12, 0),
                .TabIndex = 1
            }

            BumpTabIndexes(PanelButtonsRight, 3)
            PanelButtonsRight.Controls.Add(ButtonImport)
            PanelButtonsRight.Controls.Add(ComboOutputSource)
            PanelButtonsRight.Controls.Add(LabelOutputSource)

            ButtonContainer.Controls.Add(ButtonPreview)
            ButtonContainer.Controls.Add(ButtonProcess)
            ButtonContainer.Controls.Add(ButtonDiscard)
            ButtonContainer.Controls.Add(ButtonTrackLayout)
            ButtonContainer.Controls.Add(ButtonModifications)

            ButtonOk.Text = My.Resources.Label_ImportClose
            ButtonOk.Visible = True

            Dim Row As Integer

            With TableLayoutPanelMain
                .SuspendLayout()

                .Left = 0
                .RowCount = 4
                .ColumnCount = 7
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
                .Controls.Add(LabelDevice, 0, Row)
                .Controls.Add(ComboDevices, 1, Row)

                .Controls.Add(CheckBoxExtendedLogging, 2, Row)
                .SetColumnSpan(CheckBoxExtendedLogging, 2)

                .Controls.Add(CheckBoxRemaster, 2, Row)

                .Controls.Add(CheckBox86FSurfaceData, 3, Row)
                .SetColumnSpan(CheckBox86FSurfaceData, 2)

                .Controls.Add(CheckBoxSaveLog, 5, Row)
                .SetColumnSpan(CheckBoxSaveLog, 2)

                Row = 1
                .Controls.Add(LabelFileName, 0, Row)
                .Controls.Add(TextBoxFileName, 1, Row)
                .SetColumnSpan(TextBoxFileName, 4)
                .Controls.Add(ComboExtensions, 4, Row)
                .Controls.Add(ButtonOpen, 6, Row)

                Row = 2
                .Controls.Add(LabelImageFormat, 0, Row)
                .Controls.Add(ComboImageFormat, 1, Row)
                .SetColumnSpan(ComboImageFormat, 2)

                .Controls.Add(LabelOutputType, 3, Row)
                .Controls.Add(ComboOutputType, 4, Row)
                .SetColumnSpan(ComboOutputType, 2)

                .Controls.Add(CheckBoxDoublestep, 6, Row)

                Row = 3
                .Controls.Add(TableSide0, 0, Row)
                .SetColumnSpan(TableSide0, 3)

                .Controls.Add(TableSide1, 3, Row)
                .SetColumnSpan(TableSide1, 3)

                .Controls.Add(ButtonContainer, 6, Row)

                .ResumeLayout()
            End With
        End Sub

        Private Sub InitializeDevice(Repopulate As Boolean)
            If Repopulate Then
                PopulateDeviceCombo()
            End If

            ButtonProcess.Enabled = False

            _SelectedDevice = CType(ComboDevices.SelectedItem, IDevice)

            LogFileName = _SelectedDevice.Settings.LogFileName
            LogStripPath = _SelectedDevice.Settings.LogStripPath

            TrackStatus = _SelectedDevice.TrackStatus

            CheckBoxSaveLog.Checked = GetSelectedDeviceState.SaveLog
            CheckBoxExtendedLogging.Checked = GetSelectedDeviceState.ExtendedLogs.GetValueOrDefault(False)

            InitializeImage(Repopulate)
        End Sub

        Private Sub InitializeImage(Clear As Boolean)
            RefreshDeviceState()
            If Clear Then
                SetNewFileName()
                SetTiltebarText()
                GridReset(_TrackCount, _SideCount, _FluxHeaders)
                ClearStatusBar()
            End If
            Dim ImageFormat = ReadImageFormat()
            PopulateImageFormats(ComboImageFormat, ImageFormat, ImageFormat, False)
            PopulateOutputTypes()
            PopulateFileExtensions()
            RefreshButtonState()
            Me.Refresh()
        End Sub

        Private Sub MoveLogs()
            If Not String.IsNullOrEmpty(_InputFilePath) Then
                Dim LogFilePath As String = IO.Path.GetDirectoryName(_InputFilePath)
                _OutputImages.MoveLogs(LogFilePath)
            End If
        End Sub

        Private Function OpenFluxImage(Filename As String) As Boolean
            If ComboDevices.SelectedIndex = -1 Then
                Return False
            End If

            Dim SelectedDevice As IDevice = CType(ComboDevices.SelectedItem, IDevice)
            Dim AllowSCP As Boolean = SelectedDevice.InputTypeSupported(InputFileTypeEnum.scp)

            Dim Response = AnalyzeFluxImage(Filename, AllowSCP, True)
            If Response.Result Then
                _TrackCount = Response.TrackCount
                _SideCount = Response.SideCount
                _FluxHeaders = Response.Headers
                _InputFilePath = Filename

                RefreshRemasterState()

                Return True
            End If

            Return False
        End Function

        Private Function OpenFluxImage() As Boolean
            If ComboDevices.SelectedIndex = -1 Then
                Return False
            End If

            Dim SelectedDevice As IDevice = CType(ComboDevices.SelectedItem, IDevice)
            Dim AllowSCP As Boolean = SelectedDevice.InputTypeSupported(InputFileTypeEnum.scp)

            Dim FileName As String = SharedLib.OpenFluxImage(Me, AllowSCP)

            If FileName <> "" Then
                Return OpenFluxImage(FileName)
            End If

            Return False
        End Function

        Private Sub PopulateDeviceCombo()
            _ComboDevicesNoEvent = True

            Dim FileType = GetInputFileType()
            Dim Devices = FluxDeviceGetList(FileType)

            With ComboDevices
                .DataSource = Devices
                .DisplayMember = "Name"
                .ValueMember = "Device"

                .DropDownStyle = ComboBoxStyle.DropDownList
            End With

            If _UserState.Convert.LastDevice.HasValue Then
                ComboDevices.SelectedValue = _UserState.Convert.LastDevice.Value
            End If

            If ComboDevices.Items.Count > 0 And ComboDevices.SelectedIndex < 0 Then
                ComboDevices.SelectedIndex = 0
            End If

            If ComboDevices.SelectedItem IsNot Nothing Then
                _SelectedDevice = CType(ComboDevices.SelectedItem, IDevice)
            End If

            ComboDevices.Enabled = (ComboDevices.Items.Count > 1)

            _ComboDevicesNoEvent = False
        End Sub

        Private Sub PopulateFileExtensions()
            _ComboExtensionsNoEvent = True

            Dim OutputType As ImageImportOutputTypes = ComboOutputType.SelectedValue

            If OutputType <> ImageImportOutputTypes.IMA Then
                Dim Extension = ImageImportOutputTypeFileExt(OutputType)

                Dim items As New List(Of FileExtensionItem) From {
                    New FileExtensionItem(Extension, Nothing)
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

            If ComboExtensions.SelectedIndex = -1 AndAlso ComboExtensions.Items.Count > 0 Then
                ComboExtensions.SelectedIndex = 0
            End If

            ComboExtensions.Enabled = (ComboExtensions.Items.Count > 1)

            _ComboExtensionsNoEvent = False
        End Sub

        Private Sub PopulateOutputSourceCombo()
            _ComboOutputSourceNoEvent = True

            Dim FileType = GetInputFileType()
            Dim Devices = FluxDeviceGetList(FileType)

            Devices.RemoveAll(Function(d) Not _OutputImages.Images.ContainsKey(d.Device))

            With ComboOutputSource
                .DataSource = Devices
                .DisplayMember = "Name"
                .ValueMember = "Device"

                .DropDownStyle = ComboBoxStyle.DropDownList
            End With

            If _UserState.Convert.LastSource.HasValue Then
                ComboOutputSource.SelectedValue = _UserState.Convert.LastSource.Value
            End If

            If ComboOutputSource.Items.Count > 0 And ComboOutputSource.SelectedIndex < 0 Then
                ComboOutputSource.SelectedIndex = 0
            End If

            ComboOutputSource.Enabled = (ComboOutputSource.Items.Count > 1)

            _ComboOutputSourceNoEvent = False

            _SelectedSource = CType(ComboOutputSource.SelectedItem, IDevice)
        End Sub

        Private Sub PopulateOutputTypes()
            _ComboOutputTypeNoEvent = True

            Dim OutputTypes As New List(Of KeyValuePair(Of String, ImageImportOutputTypes))

            For Each OutputType As ImageImportOutputTypes In [Enum].GetValues(GetType(ImageImportOutputTypes))
                If _SelectedDevice IsNot Nothing AndAlso Not _SelectedDevice.OutputTypeSupported(OutputType) Then
                    Continue For
                End If

                OutputTypes.Add(New KeyValuePair(Of String, ImageImportOutputTypes)(
                    ImageImportOutputTypeDescription(OutputType), OutputType)
                )
            Next

            InitializeCombo(ComboOutputType, OutputTypes, Nothing)

            If _SelectedDevice IsNot Nothing Then
                Dim CachedOutputType = GetSelectedDeviceState.OutputType
                If CachedOutputType.HasValue Then
                    ComboOutputType.SelectedValue = CachedOutputType.Value
                End If
            End If

            If ComboOutputType.SelectedIndex = -1 AndAlso ComboOutputType.Items.Count > 0 Then
                ComboOutputType.SelectedIndex = 0
            End If

            ComboOutputType.Enabled = (ComboDevices.Items.Count > 1)

            _ComboOutputTypeNoEvent = False
        End Sub

        Private Sub PreviewImage()
            If _SelectedDevice Is Nothing Then
                Exit Sub
            End If

            Dim HasSelectedOutputFile As Boolean = _OutputImages.Images.ContainsKey(_SelectedDevice.Device)

            If HasSelectedOutputFile Then
                Dim ImageInfo = _OutputImages.Images(_SelectedDevice.Device)
                Dim ImageData = New ImageData(ImageInfo.FilePath)

                Dim Caption As String = TextBoxFileName.Text

                If Not ImagePreview.Display(ImageData, Caption, Me) Then
                    MsgBox(My.Resources.Dialog_ImagePreviewFail, MsgBoxStyle.Exclamation)
                End If
            Else
                If Not _SelectedDevice.SupportsPreview Then
                    Exit Sub
                End If

                Dim DiskParams = SelectedDiskParams()

                If Not DiskParams.HasValue Then
                    Exit Sub
                End If

                Dim Response = _SelectedDevice.ConvertFirstTrack(_InputFilePath, True, DiskParams.Value)

                If Not Response.Result Then
                    MsgBox(My.Resources.Dialog_ImagePreviewFail, MsgBoxStyle.Exclamation)
                End If

                Dim Caption As String = TextBoxFileName.Text

                If Not ImagePreview.Display(Response.Filename, DiskParams.Value, Caption, Me) Then
                    MsgBox(My.Resources.Dialog_ImagePreviewFail, MsgBoxStyle.Exclamation)
                End If

                DeleteTempFileIfExists(Response.Filename)
            End If
        End Sub

        Private Sub ProcessImage()
            If _SelectedDevice Is Nothing Then
                Exit Sub
            End If

            If ComboOutputType.SelectedIndex = -1 Then
                Exit Sub
            End If

            Dim UseImageFormat As Boolean = _SelectedDevice.RequiresImageFormat

            Dim DiskParams = SelectedDiskParams()

            If Not DiskParams.HasValue And UseImageFormat Then
                Exit Sub
            End If

            Dim OutputType As ImageImportOutputTypes = ComboOutputType.SelectedValue

            Dim FileExt = ImageImportOutputTypeFileExt(OutputType)

            ClearProcessedImage()

            Dim FilePath = _OutputImages.SetPendingImage(_SelectedDevice.Device, FileExt)

            _DoubleStep = CheckBoxDoublestep.Enabled AndAlso CheckBoxDoublestep.Checked

            Dim Response = GenerateCommandLine(_InputFilePath, FilePath, _SelectedDevice.Device, ComboOutputType.SelectedValue, DiskParams)
            If Not String.IsNullOrEmpty(Response.Suffix) Then
                _OutputImages.PendingImage.SetSuffix(Response.Suffix)
            End If

            Process.StartAsync(_SelectedDevice.Settings.AppPath, Response.Arguments)
        End Sub

        Private Sub ProcessImport()
            MoveLogs()
            SetOutputFilePath()

            If Not String.IsNullOrEmpty(_OutputFilePath) Then
                RaiseEvent ImportProcess(_OutputFilePath, GetNewFileName())
            End If

            _OutputImages.ClearImages()
            ClearLoadedImage()
            ClearProcessedImage()
        End Sub

        Private Sub ProcessOutputLine(line As String)
            _ConsoleQueue.Enqueue(line)

            TrackStatus.ProcessOutputLineRead(line, ActionTypeEnum.Import, _DoubleStep)

            If TrackStatus.Failed Then
                Process.Cancel()
            End If
        End Sub

        Private Function ReadImageFormat() As DiskImage.FloppyDiskFormat
            If ComboDevices.SelectedIndex = -1 Then
                Return FloppyDiskFormat.FloppyUnknown
            End If

            Dim SelectedDevice As IDevice = CType(ComboDevices.SelectedItem, IDevice)

            Dim Response = SelectedDevice.ConvertFirstTrack(_InputFilePath, False)

            If Not Response.Result Then
                Return FloppyDiskFormat.FloppyUnknown
            End If

            Return DetectImageFormat(Response.Filename, True)
        End Function

        Private Sub RefreshButtonState()
            Dim OutputType As ImageImportOutputTypes? = Nothing

            Dim Is525DDStandard As Boolean = False
            Dim IsNonImage As Boolean = True

            Dim DiskParams = SelectedDiskParams()

            If DiskParams.HasValue Then
                Is525DDStandard = DiskParams.Value.IsStandard AndAlso DiskParams.Value.DriveType = FloppyDriveType.Drive525DoubleDensity
                IsNonImage = DiskParams.Value.IsNonImage
            End If

            Dim HasOutputfile As Boolean = _OutputImages.HasImage
            Dim HasSelectedOutputFile As Boolean = _OutputImages.Images.ContainsKey(_SelectedDevice.Device)
            Dim HasInputFile As Boolean = Not String.IsNullOrEmpty(_InputFilePath)
            Dim IsRunning As Boolean = Process.IsRunning
            Dim IsIdle As Boolean = Not IsRunning

            Dim SettingsEnabled As Boolean = IsIdle
            Dim CanConfigure As Boolean = SettingsEnabled AndAlso HasInputFile
            Dim UseImageFormat As Boolean = _SelectedDevice IsNot Nothing AndAlso _SelectedDevice.RequiresImageFormat
            Dim SupportsPreview As Boolean = _SelectedDevice IsNot Nothing AndAlso _SelectedDevice.SupportsPreview

            Dim SourceIsPcImgCnv As Boolean = HasOutputfile AndAlso _OutputImages.HasPcImgCnvImage

            ComboExtensions.Enabled = HasInputFile And ComboExtensions.Items.Count > 1
            ComboImageFormat.Enabled = CanConfigure AndAlso UseImageFormat

            If ComboOutputType IsNot Nothing Then
                If ComboOutputType.SelectedIndex > -1 Then
                    OutputType = ComboOutputType.SelectedValue
                End If
                ComboOutputType.Enabled = CanConfigure And ComboOutputType.Items.Count > 1
            End If

            ComboDevices.Enabled = SettingsEnabled AndAlso ComboDevices.Items.Count > 1

            ButtonCancel.Text = If(IsRunning Or HasOutputfile, WithoutHotkey(My.Resources.Menu_Cancel), WithoutHotkey(My.Resources.Menu_Close))
            ButtonDiscard.Enabled = IsIdle AndAlso HasOutputfile
            ButtonOpen.Enabled = SettingsEnabled AndAlso Not HasOutputfile

            ButtonProcess.Enabled = HasInputFile AndAlso (Not IsNonImage OrElse Not UseImageFormat OrElse IsRunning)

            If IsRunning Then
                ButtonProcess.Text = My.Resources.Label_Abort
            ElseIf _SelectedDevice IsNot Nothing AndAlso HasSelectedOutputFile Then
                ButtonProcess.Text = My.Resources.Label_ReProcess
            Else
                ButtonProcess.Text = My.Resources.Label_Process
            End If

            ButtonSaveLog.Enabled = IsIdle AndAlso Not String.IsNullOrEmpty(TextBoxConsole.Text)

            ButtonTrackLayout.Enabled = IsIdle AndAlso HasOutputfile AndAlso SourceIsPcImgCnv
            ButtonModifications.Enabled = IsIdle AndAlso HasOutputfile AndAlso SourceIsPcImgCnv

            ButtonPreview.Enabled = HasInputFile AndAlso IsIdle AndAlso ((Not IsNonImage AndAlso SupportsPreview) OrElse HasSelectedOutputFile)

            TextBoxFileName.ReadOnly = Not HasInputFile

            If Is525DDStandard Then
                CheckBoxDoublestep.Enabled = CanConfigure AndAlso _TrackCount > 42
                CheckBoxDoublestep.Checked = _TrackCount > 79
            Else
                CheckBoxDoublestep.Enabled = False
                CheckBoxDoublestep.Checked = False
            End If

            CheckBoxExtendedLogging.Enabled = SettingsEnabled
            CheckBox86FSurfaceData.Enabled = SettingsEnabled AndAlso OutputType.HasValue AndAlso OutputType.Value = ImageImportOutputTypes.F86

            If _SelectedDevice IsNot Nothing AndAlso _SelectedDevice.Device = IDevice.FluxDevice.PcImgCnv Then
                CheckBoxRemaster.Enabled = SettingsEnabled AndAlso _TrackLayoutExists
            Else
                CheckBoxRemaster.Enabled = False
            End If

            CheckBoxSaveLog.Enabled = SettingsEnabled

            LabelOutputSource.Visible = _OutputImages.Images.Count > 1
            ComboOutputSource.Visible = _OutputImages.Images.Count > 1

            RefreshImportButtonState()
        End Sub

        Private Sub RefreshDeviceState()
            If _SelectedDevice Is Nothing Then
                Exit Sub
            End If

            Dim IsPcImgCnv As Boolean = False

            Select Case _SelectedDevice.Device
                Case IDevice.FluxDevice.Kryoflux
                    CheckBox86FSurfaceData.Visible = False
                    CheckBoxExtendedLogging.Visible = True

                Case IDevice.FluxDevice.PcImgCnv
                    CheckBoxExtendedLogging.Visible = False
                    CheckBox86FSurfaceData.Visible = True
                    IsPcImgCnv = True

                Case Else
                    CheckBox86FSurfaceData.Visible = False
                    CheckBoxExtendedLogging.Visible = False
            End Select

            CheckBoxRemaster.Visible = IsPcImgCnv
            ButtonTrackLayout.Visible = IsPcImgCnv OrElse _OutputImages.HasPcImgCnvImage
            ButtonModifications.Visible = IsPcImgCnv OrElse _OutputImages.HasPcImgCnvImage
        End Sub

        Private Sub RefreshGridState()
            If ComboDevices.SelectedIndex = -1 Then
                Exit Sub
            End If

            Dim SelectedDevice As IDevice = CType(ComboDevices.SelectedItem, IDevice)

            Dim State = _OutputImages.GetState(SelectedDevice.Device)

            If State.HasValue Then
                SetState(State.Value)
            Else
                ClearProcessedImage()
            End If
            MyBase.Refresh()
        End Sub

        Private Sub RefreshImportButtonState()
            Dim HasOutputfile As Boolean = _OutputImages.HasImage
            Dim HasInputFile As Boolean = Not String.IsNullOrEmpty(_InputFilePath)

            Dim EnableImport As Boolean = Not Process.IsRunning AndAlso HasOutputfile AndAlso HasInputFile AndAlso Not String.IsNullOrEmpty(TextBoxFileName.Text)

            ButtonOk.Enabled = EnableImport
            ButtonImport.Enabled = EnableImport
        End Sub

        Private Sub RefreshPreferredExensions()
            If ComboExtensions.SelectedIndex = -1 Then
                Exit Sub
            End If

            Dim Item As FileExtensionItem = ComboExtensions.SelectedValue

            If Not Item.Format.HasValue Then
                Exit Sub
            End If

            Dim DiskParams = SelectedDiskParams()

            If Not DiskParams.HasValue Then
                Exit Sub
            End If

            If Item.Format.Value <> DiskParams.Value.Format Then
                App.UserState.RemovePreferredExtension(DiskParams.Value.Format)
            End If

            App.UserState.SetPreferredExtension(Item.Format.Value, Item.Extension)
        End Sub

        Private Sub RefreshRemasterState()
            If Not App.AppSettings.PcImgCnv.IsPathValid Then
                Exit Sub
            End If

            Dim HasInputFile As Boolean = Not String.IsNullOrEmpty(_InputFilePath)

            _TrackLayoutExists = TrackLayoutExists()
            CheckBoxRemaster.Checked = _TrackLayoutExists AndAlso HasInputFile
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
        Private Sub SetNewFileName()
            Dim FileExt = IO.Path.GetExtension(_InputFilePath).ToLower

            If FileExt = ".raw" Then
                Dim ParentFolder As String = IO.Path.GetFileName(IO.Directory.GetParent(_InputFilePath).FullName)
                TextBoxFileName.Text = ParentFolder
            Else
                TextBoxFileName.Text = IO.Path.GetFileNameWithoutExtension(_InputFilePath)
            End If
        End Sub

        Private Sub SetOutputFilePath()
            If _SelectedSource IsNot Nothing Then
                Dim Image = _OutputImages.Images(_SelectedSource.Device)
                Image.Keep = True
                _OutputFilePath = Image.FilePath
            Else
                _OutputFilePath = ""
            End If
        End Sub

        Private Sub SetTiltebarText()
            Dim Text = My.Resources.Caption_ConvertFluxImage

            If String.IsNullOrEmpty(_InputFilePath) Then
                Me.Text = Text
                Exit Sub
            End If

            Dim DisplayFileName = IO.Path.GetFileName(_InputFilePath)
            Dim FileExt = IO.Path.GetExtension(DisplayFileName).ToLower

            If FileExt = ".raw" Then
                Dim ParentFolder As String = IO.Path.GetFileName(IO.Directory.GetParent(_InputFilePath).FullName)
                DisplayFileName = IO.Path.Combine(ParentFolder, "*.raw")
            End If

            Me.Text = Text & " - " & DisplayFileName
        End Sub

        Private Function TrackLayoutExists() As Boolean
            If String.IsNullOrEmpty(_InputFilePath) Then
                Return False
            End If

            Dim FileExt = IO.Path.GetExtension(_InputFilePath).ToLower
            If FileExt <> ".raw" Then
                Return False
            End If

            Dim ParentFolder As String = IO.Path.GetDirectoryName(_InputFilePath)

            Return IO.File.Exists(IO.Path.Combine(ParentFolder, "tracklayout.txt"))
        End Function
        Private Class OutputImageInfo
            Private ReadOnly _FileSource As IDevice.FluxDevice
            Private _FilePath As String
            Private _Keep As Boolean = False
            Public Sub New(FilePath As String, FileSource As IDevice.FluxDevice)
                _FilePath = FilePath
                _FileSource = FileSource
            End Sub

            Public ReadOnly Property FilePath As String
                Get
                    Return _FilePath
                End Get
            End Property

            Public ReadOnly Property FileSource As IDevice.FluxDevice
                Get
                    Return _FileSource
                End Get
            End Property

            Public Property Keep As Boolean
                Get
                    Return _Keep
                End Get
                Set
                    _Keep = Value
                End Set
            End Property
            Public Sub SetSuffix(Suffix As String)
                If String.IsNullOrEmpty(Suffix) Then
                    Exit Sub
                End If

                Dim PathName = IO.Path.GetDirectoryName(_FilePath)
                Dim BaseName = IO.Path.GetFileNameWithoutExtension(_FilePath)
                Dim Ext = IO.Path.GetExtension(_FilePath)

                _FilePath = IO.Path.Combine(PathName, BaseName & "_" & Suffix & Ext)
            End Sub
        End Class

        Private Class OutputImages
            Private ReadOnly _Images As Dictionary(Of IDevice.FluxDevice, OutputImageInfo)
            Private ReadOnly _LogFiles As Dictionary(Of IDevice.FluxDevice, OutputLogInfo)
            Private ReadOnly _State As Dictionary(Of IDevice.FluxDevice, FluxFormState)
            Private ReadOnly _TempPath As String
            Private _PendingImage As OutputImageInfo = Nothing

            Public Sub New(TempPath As String)
                _TempPath = TempPath
                _Images = New Dictionary(Of IDevice.FluxDevice, OutputImageInfo)
                _LogFiles = New Dictionary(Of IDevice.FluxDevice, OutputLogInfo)
                _State = New Dictionary(Of IDevice.FluxDevice, FluxFormState)
            End Sub

            Public ReadOnly Property Images As Dictionary(Of IDevice.FluxDevice, OutputImageInfo)
                Get
                    Return _Images
                End Get
            End Property

            Public ReadOnly Property PendingImage As OutputImageInfo
                Get
                    Return _PendingImage
                End Get
            End Property

            Public Sub ClearImages()
                For Each Image In _Images.Values
                    If Not Image.Keep Then
                        DeleteTempFileIfExists(Image.FilePath)
                    End If
                Next
                _Images.Clear()

                For Each LogFile In _LogFiles.Values
                    DeleteTempFileIfExists(LogFile.FilePath)
                Next
                _LogFiles.Clear()

                _State.Clear()
            End Sub

            Public Sub ClearPendingImage()
                If _PendingImage Is Nothing Then
                    Exit Sub
                End If

                DeleteTempFileIfExists(_PendingImage.FilePath)
                _PendingImage = Nothing
            End Sub

            Public Function GetState(FileSource As IDevice.FluxDevice) As FluxFormState?
                If _State.ContainsKey(FileSource) Then
                    Return _State.Item(FileSource)
                End If
                Return Nothing
            End Function

            Public Function HasImage() As Boolean
                Return _Images.Count > 0
            End Function

            Public Function HasPcImgCnvImage() As Boolean
                Return _Images.ContainsKey(IDevice.FluxDevice.PcImgCnv)
            End Function

            Public Sub MoveLogs(DestinationPath As String)
                For Each LogFile In _LogFiles.Values
                    If Not String.IsNullOrEmpty(LogFile.FilePath) And Not String.IsNullOrEmpty(LogFile.LogFileName) Then
                        Dim NewFilePath As String = IO.Path.Combine(DestinationPath, LogFile.LogFileName)
                        Try
                            IO.File.Delete(NewFilePath)
                            IO.File.Move(LogFile.FilePath, NewFilePath)
                        Catch ex As Exception
                            ' Ignore errors
                        End Try

                    End If
                Next
                _LogFiles.Clear()
            End Sub

            Public Function SetPendingImage(FileSource As IDevice.FluxDevice, Extension As String) As String
                ClearPendingImage()
                Dim FileName As String = Guid.NewGuid.ToString & Extension

                Dim FilePath = IO.Path.Combine(_TempPath, FileName)

                _PendingImage = New OutputImageInfo(FilePath, FileSource)

                Return FilePath
            End Function

            Public Sub SetState(FileSource As IDevice.FluxDevice, State As FluxFormState)
                _State.Item(FileSource) = State
            End Sub

            Public Sub StoreLogFile(FileSource As IDevice.FluxDevice, LogFileName As String, Content As String, StripPathFromLog As Boolean)
                Dim FileName As String = Guid.NewGuid.ToString & ".txt"

                Dim FilePath = IO.Path.Combine(_TempPath, FileName)

                SaveLogFile(FilePath, Content, StripPathFromLog)

                If _LogFiles.ContainsKey(FileSource) Then
                    DeleteTempFileIfExists(_LogFiles.Item(FileSource).FilePath)
                    _LogFiles.Remove(FileSource)
                End If

                _LogFiles.Item(FileSource) = New OutputLogInfo(FilePath, FileSource, LogFileName)
            End Sub

            Public Sub StorePendingImage(State As FluxFormState)
                If _PendingImage Is Nothing Then
                    Exit Sub
                End If

                If _Images.ContainsKey(_PendingImage.FileSource) Then
                    DeleteTempFileIfExists(_Images.Item(_PendingImage.FileSource).FilePath)
                    _Images.Remove(_PendingImage.FileSource)
                End If

                _Images.Item(_PendingImage.FileSource) = _PendingImage
                SetState(_PendingImage.FileSource, State)
                _PendingImage = Nothing
            End Sub
        End Class

        Private Class OutputLogInfo
            Private ReadOnly _FilePath As String
            Private ReadOnly _FileSource As IDevice.FluxDevice
            Private ReadOnly _LogFileName As String

            Public Sub New(FilePath As String, FileSource As IDevice.FluxDevice, LogFileName As String)
                _FilePath = FilePath
                _FileSource = FileSource
                _LogFileName = LogFileName
            End Sub

            Public ReadOnly Property FilePath As String
                Get
                    Return _FilePath
                End Get
            End Property

            Public ReadOnly Property FileSource As IDevice.FluxDevice
                Get
                    Return _FileSource
                End Get
            End Property

            Public ReadOnly Property LogFileName As String
                Get
                    Return _LogFileName
                End Get
            End Property
        End Class
#Region "Events"
        Private Sub ButtonDiscard_Click(sender As Object, e As EventArgs) Handles ButtonDiscard.Click
            _OutputImages.ClearImages()
            ClearLoadedImage()
            ClearProcessedImage()
        End Sub

        Private Sub ButtonImport_Click(sender As Object, e As EventArgs) Handles ButtonImport.Click
            If _LaunchedFromDialog Then
                Exit Sub
            End If

            If _OutputImages.HasImage Then
                ProcessImport()
            End If
        End Sub

        Private Sub ButtonOpen_Click(sender As Object, e As EventArgs) Handles ButtonOpen.Click
            If OpenFluxImage() Then
                InitializeDevice(True)
            End If
        End Sub

        Private Sub ButtonPreview_Click(sender As Object, e As EventArgs) Handles ButtonPreview.Click
            PreviewImage()
        End Sub

        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If CancelProcessIfRunning() Then
                Exit Sub
            End If

            ProcessImage()
        End Sub

        Private Sub ButtonTrackData_Click(sender As Object, e As EventArgs) Handles ButtonTrackLayout.Click
            GenerateTrackData()
        End Sub

        Private Sub ButtonWriteSplices_Click(sender As Object, e As EventArgs) Handles ButtonModifications.Click
            DisplayModifications()
        End Sub

        Private Sub CheckBoxExtendedLogging_CheckStateChanged(sender As Object, e As EventArgs) Handles CheckBoxExtendedLogging.CheckStateChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _SelectedDevice Is Nothing Then
                Exit Sub
            End If

            GetSelectedDeviceState.ExtendedLogs = CheckBoxExtendedLogging.Checked
        End Sub

        Private Sub CheckBoxSaveLog_CheckStateChanged(sender As Object, e As EventArgs) Handles CheckBoxSaveLog.CheckStateChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _SelectedDevice Is Nothing Then
                Exit Sub
            End If

            GetSelectedDeviceState.SaveLog = CheckBoxSaveLog.Checked
        End Sub

        Private Sub ComboDevices_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboDevices.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboDevicesNoEvent Then
                Exit Sub
            End If

            If ComboDevices.SelectedIndex > -1 Then
                _UserState.Convert.LastDevice = CType(ComboDevices.SelectedItem, IDevice).Device
            End If

            RefreshGridState()
            InitializeDevice(False)
        End Sub

        Private Sub ComboExtensions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboExtensions.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboExtensionsNoEvent Then
                Exit Sub
            End If

            RefreshPreferredExensions()
        End Sub

        Private Sub ComboImageFormat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageFormat.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            PopulateOutputTypes()
            PopulateFileExtensions()
            RefreshButtonState()
        End Sub

        Private Sub ComboOutputSource_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboOutputSource.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboOutputSourceNoEvent Then
                Exit Sub
            End If

            If ComboOutputSource.SelectedIndex > -1 Then
                _UserState.Convert.LastSource = CType(ComboOutputSource.SelectedItem, IDevice).Device
            End If

            _SelectedSource = CType(ComboOutputSource.SelectedItem, IDevice)
        End Sub
        Private Sub ComboOutputType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboOutputType.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboOutputTypeNoEvent Then
                Exit Sub
            End If

            If ComboOutputType.SelectedIndex > -1 Then
                Dim OutputType As ImageImportOutputTypes = ComboOutputType.SelectedValue
                GetSelectedDeviceState.OutputType = OutputType
            End If

            PopulateFileExtensions()
            RefreshButtonState()
        End Sub

        Private Sub ConsoleTimer_Tick(sender As Object, e As EventArgs) Handles ConsoleTimer.Tick
            If TextBoxConsole.IsDisposed Then
                ConsoleTimer.Stop()
                Return
            End If

            If _ConsoleQueue.IsEmpty Then
                If Not Process.IsRunning Then
                    ConsoleTimer.Stop()
                    If Process.State = ConsoleProcessRunner.ProcessStateEnum.Completed Then
                        ImagePostProcessCompleted()
                    End If
                End If
                Return
            End If

            Dim sb As New Text.StringBuilder()
            Dim line As String = ""

            While _ConsoleQueue.TryDequeue(line)
                sb.AppendLine(line)
            End While

            If sb.Length > 0 Then
                TextBoxConsole.AppendText(sb.ToString())
                TextBoxConsole.Refresh()
            End If

            If _ConsoleQueue.IsEmpty AndAlso Not Process.IsRunning Then
                ConsoleTimer.Stop()
                If Process.State = ConsoleProcessRunner.ProcessStateEnum.Completed Then
                    ImagePostProcessCompleted()
                End If
            End If
        End Sub

        Private Sub ImageImportForm_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
            If ComboDevices.SelectedIndex = -1 Then
                Return
            End If

            Dim SelectedDevice As IDevice = CType(ComboDevices.SelectedItem, IDevice)
            Dim AllowSCP As Boolean = SelectedDevice.InputTypeSupported(InputFileTypeEnum.scp)
            Dim HasOutputfile As Boolean = _OutputImages.HasImage

            If Process.IsRunning Or HasOutputfile Then
                Return
            End If

            If Not e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Return
            End If

            Dim paths = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
            If paths Is Nothing OrElse paths.Length = 0 Then
                Return
            End If

            Dim firstPath = paths(0)

            Dim Response = IsValidFluxImport(firstPath, AllowSCP)
            If Response.Result Then
                If OpenFluxImage(Response.File) Then
                    InitializeDevice(True)
                End If
            End If
        End Sub

        Private Sub ImageImportForm_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
            Dim HasOutputfile As Boolean = _OutputImages.HasImage

            If Process.IsRunning Or HasOutputfile Then
                e.Effect = DragDropEffects.None
                Return
            End If

            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Dim paths = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
                If paths IsNot Nothing AndAlso CanAcceptDrop(paths) Then
                    e.Effect = DragDropEffects.Copy
                    Return
                End If
            End If

            e.Effect = DragDropEffects.None
        End Sub
        Private Sub Process_DataReceived(Data As String) Handles Process.ErrorDataReceived, Process.OutputDataReceived
            ProcessOutputLine(Data)

            If Not ConsoleTimer.Enabled Then
                ConsoleTimer.Start()
            End If
        End Sub

        Private Sub Process_ProcessStarted(exePath As String, arguments As String) Handles Process.ProcessStarted
            StatusDevice.Text = _SelectedDevice.Name
            StatusDevice.Visible = True
            If _SelectedDevice.Device = IDevice.FluxDevice.PcImgCnv AndAlso CheckBoxRemaster.Checked Then
                ToolStripProgress.Visible = True
            End If
        End Sub

        Private Sub Process_ProcessStateChanged(State As ConsoleProcessRunner.ProcessStateEnum) Handles Process.ProcessStateChanged
            Select Case State
                Case ConsoleProcessRunner.ProcessStateEnum.Aborted
                    _OutputImages.ClearPendingImage()
                    If Not TrackStatus.Failed Then
                        TrackStatus.UpdateTrackStatusAborted()
                    End If
                    ToolStripProgress.Visible = False

                Case ConsoleProcessRunner.ProcessStateEnum.Completed
                    If TrackStatus.TrackFound Then
                        'ImagePostProcessCompleted()
                        Exit Sub
                    Else
                        _OutputImages.ClearPendingImage()
                        TrackStatus.UpdateTrackStatusError()
                        ToolStripProgress.Visible = False
                    End If

                Case ConsoleProcessRunner.ProcessStateEnum.Error
                    _OutputImages.ClearPendingImage()
                    TrackStatus.UpdateTrackStatusError()
                    ToolStripProgress.Visible = False

            End Select

            RefreshButtonState()
        End Sub

        Private Sub TextBoxFileName_Click(sender As Object, e As EventArgs) Handles TextBoxFileName.Click
            If TextBoxFileName.ReadOnly Then
                If OpenFluxImage() Then
                    InitializeDevice(True)
                End If
            End If
        End Sub

        Private Sub TextBoxFileName_TextChanged(sender As Object, e As EventArgs) Handles TextBoxFileName.TextChanged
            RefreshImportButtonState()
        End Sub

        Private Sub TextBoxFileName_Validating(sender As Object, e As CancelEventArgs) Handles TextBoxFileName.Validating
            Dim tb As TextBox = DirectCast(sender, TextBox)
            tb.Text = SanitizeFileName(tb.Text)
            RefreshImportButtonState()
        End Sub
#End Region

    End Class
End Namespace