Imports System.ComponentModel
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux
    Public Class ConvertImageForm
        Inherits BaseFluxForm

        Private WithEvents ButtonClear As Button
        Private WithEvents ButtonImport As Button
        Private WithEvents ButtonOpen As Button
        Private WithEvents ButtonProcess As Button
        Private WithEvents CheckBoxDoublestep As CheckBox
        Private WithEvents CheckBoxExtendedLogging As CheckBox
        Private WithEvents ComboDevices As ComboBox
        Private WithEvents ComboExtensions As ComboBox
        Private WithEvents ComboImageFormat As ComboBox
        Private WithEvents ComboOutputType As ComboBox
        Private WithEvents TextBoxFileName As TextBox

        Private Shared _CachedDevice? As FluxDeviceInfo = Nothing
        Private Shared _CachedExtendedLogging As Boolean = False
        Private ReadOnly _Initialized As Boolean = False
        Private ReadOnly _LaunchedFromDialog As Boolean = False
        Private _ComboDevicesNoEvent As Boolean = False
        Private _ComboExtensionsNoEvent As Boolean = False
        Private _ComboOutputTypeNoEvent As Boolean = False
        Private _DoubleStep As Boolean = False
        Private _InputFilePath As String = ""
        Private _OutputFilePath As String = ""
        Private _SelectedDevice As FluxDeviceInfo = Nothing
        Private _SideCount As Integer
        Private _TrackCount As Integer
        Private LabelOutputType As Label

        Public Event ImportRequested(File As String, NewFilename As String)

        Public Sub New(FilePath As String, TrackCount As Integer, SideCount As Integer, LaunchedFromDialog As Boolean)
            MyBase.New("")

            Me.AllowDrop = True

            _LaunchedFromDialog = LaunchedFromDialog
            _InputFilePath = FilePath
            _TrackCount = TrackCount
            _SideCount = SideCount

            InitializeControls()

            If LaunchedFromDialog Then
                ButtonImport.DialogResult = DialogResult.Retry
            End If

            CheckBoxExtendedLogging.Checked = _CachedExtendedLogging

            InitializeDevice(True)

            _Initialized = True
        End Sub

        Public ReadOnly Property OutputFilePath As String
            Get
                Return _OutputFilePath
            End Get
        End Property

        Public Function GetNewFileName() As String
            Dim Item As FileExtensionItem = ComboExtensions.SelectedItem

            Return TextBoxFileName.Text & Item.Extension
        End Function

        Protected Overrides Sub OnAfterBaseFormClosing(e As FormClosingEventArgs)
            If Me.DialogResult = DialogResult.Cancel OrElse Me.DialogResult = DialogResult.None Then
                ClearOutputFile(True)
            End If
        End Sub

        Private Function AllowSectorImage() As Boolean
            Dim SelectedDevice As FluxDeviceInfo = CType(ComboDevices.SelectedItem, FluxDeviceInfo)
            Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            Select Case SelectedDevice.Device
                Case FluxDevice.Greaseweazle
                    Dim imageFormat = Greaseweazle.GreaseweazleImageFormatFromFloppyDiskFormat(ImageParams.Format)
                    Return (imageFormat <> Greaseweazle.GreaseweazleImageFormat.None)

                Case Else
                    Return True
            End Select
        End Function

        Private Function CanAcceptDrop(paths As IEnumerable(Of String)) As Boolean
            Dim SelectedDevice As FluxDeviceInfo = CType(ComboDevices.SelectedItem, FluxDeviceInfo)

            For Each path In paths
                If IsValidFluxImport(path, SelectedDevice.AllowSCP).Result Then
                    Return True
                End If

                Exit For
            Next

            Return False
        End Function

        Private Sub ClearLoadedImage()
            TextBoxFileName.Text = ""
            _InputFilePath = ""
            ComboImageFormat.SelectedIndex = 0
            SetTiltebarText()
            RefreshButtonState()
        End Sub

        Private Sub ClearOutputFile(Delete As Boolean)
            If Delete AndAlso Not String.IsNullOrEmpty(_OutputFilePath) Then
                DeleteFileIfExists(_OutputFilePath)
            End If
            _OutputFilePath = ""
        End Sub

        Private Sub ClearProcessedImage(DeleteOutputFile As Boolean)
            TextBoxConsole.Clear()
            ClearOutputFile(DeleteOutputFile)
            ClearStatusBar()
            GridReset(_TrackCount, _SideCount)

            TrackStatus.Clear()
        End Sub

        Private Function ConvertFirstTrack() As (Result As Boolean, Filename As String)
            Dim SelectedDevice As FluxDeviceInfo = CType(ComboDevices.SelectedItem, FluxDeviceInfo)

            Select Case SelectedDevice.Device
                Case FluxDevice.Greaseweazle
                    Return Greaseweazle.ConvertFirstTrack(_InputFilePath)

                Case FluxDevice.Kryoflux
                    Return Kryoflux.ConvertFirstTrack(_InputFilePath)
            End Select

            Return (False, "")
        End Function

        Private Function GenerateCommandLine(FilePath As String)
            Dim OutputType As ImageImportOutputTypes = ComboOutputType.SelectedValue

            Select Case _SelectedDevice.Device
                Case FluxDevice.Greaseweazle
                    _OutputFilePath = FilePath
                    Return Greaseweazle.GenerateCommandLineImport(_InputFilePath, FilePath, ComboImageFormat.SelectedValue, OutputType, _DoubleStep)

                Case FluxDevice.Kryoflux
                    Dim LogLevel As Kryoflux.CommandLineBuilder.LogMask = 0

                    If CheckBoxExtendedLogging.Checked Then
                        LogLevel = Kryoflux.CommandLineBuilder.LogMask.Read Or Kryoflux.CommandLineBuilder.LogMask.Cell
                    End If

                    Dim Response = Kryoflux.GenerateCommandLineImport(_InputFilePath, FilePath, ComboImageFormat.SelectedValue, _DoubleStep, LogLevel)
                    _OutputFilePath = Response.OutputFilePath
                    Return Response.Arguments
            End Select

            Return ""
        End Function

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

            CheckBoxExtendedLogging = New CheckBox With {
                .Text = "Extended Log Output",
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
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

            ButtonProcess = New Button With {
                .Margin = New Padding(3, 0, 3, 3),
                .Text = My.Resources.Label_Write,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonClear = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Discard,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonImport = New Button With {
                .Margin = New Padding(6, 0, 6, 0),
                .Text = My.Resources.Label_Import,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .TabIndex = 0
            }

            ButtonOpen = New Button With {
                .Margin = New Padding(15, 3, 3, 3),
                .Text = My.Resources.Label_Open,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True
            }

            BumpTabIndexes(PanelButtonsRight)
            PanelButtonsRight.Controls.Add(ButtonImport)

            ButtonContainer.Controls.Add(ButtonProcess)
            ButtonContainer.Controls.Add(ButtonClear)

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

            _SelectedDevice = CType(ComboDevices.SelectedItem, FluxDeviceInfo)
            _CachedDevice = _SelectedDevice

            LogFileName = _SelectedDevice.Settings.LogFileName
            LogStripPath = _SelectedDevice.Settings.LogStripPath

            Select Case _SelectedDevice.Device
                Case FluxDevice.Greaseweazle
                    TrackStatus = New Greaseweazle.TrackStatus()

                Case FluxDevice.Kryoflux
                    TrackStatus = New Kryoflux.TrackStatus()
            End Select

            InitializeImage()
        End Sub

        Private Sub InitializeImage()
            RefreshDeviceState()
            GridReset(_TrackCount, _SideCount)
            SetNewFileName()
            SetTiltebarText()
            ClearStatusBar()
            Dim ImageFormat = ReadImageFormat()
            PopulateImageFormats(ComboImageFormat, ImageFormat, ImageFormat)
            PopulateOutputTypes()
            PopulateFileExtensions()
            RefreshButtonState()
            Me.Refresh()
        End Sub

        Private Function OpenFluxImage(Filename As String) As Boolean
            Dim SelectedDevice As FluxDeviceInfo = CType(ComboDevices.SelectedItem, FluxDeviceInfo)

            Dim response = AnalyzeFluxImage(Filename, SelectedDevice.AllowSCP)
            If response.Result Then
                _TrackCount = response.TrackCount
                _SideCount = response.SideCount
                _InputFilePath = Filename

                Return True
            End If

            Return False
        End Function

        Private Function OpenFluxImage() As Boolean
            Dim SelectedDevice As FluxDeviceInfo = CType(ComboDevices.SelectedItem, FluxDeviceInfo)
            Dim FileName As String = SharedLib.OpenFluxImage(Me, SelectedDevice.AllowSCP)

            If FileName <> "" Then
                Return OpenFluxImage(FileName)
            End If

            Return False
        End Function

        Private Sub PopulateDeviceCombo()
            _ComboDevicesNoEvent = True

            Dim FileType = GetInputFileType()
            Dim Items = FluxDeviceGetList(FileType)

            With ComboDevices
                .DataSource = Items
                .DisplayMember = "Name"
                .ValueMember = "Device"

                .DropDownStyle = ComboBoxStyle.DropDownList
            End With

            If _CachedDevice.HasValue Then
                Dim Device As FluxDevice = _CachedDevice.Value.Device
                ComboDevices.SelectedValue = Device
            End If

            If ComboDevices.Items.Count > 0 And ComboDevices.SelectedIndex < 0 Then
                ComboDevices.SelectedIndex = 0
            End If

            If ComboDevices.SelectedItem IsNot Nothing Then
                _SelectedDevice = CType(ComboDevices.SelectedItem, FluxDeviceInfo)
                _CachedDevice = _SelectedDevice
            End If

            ComboDevices.Enabled = (ComboDevices.Items.Count > 1)

            _ComboDevicesNoEvent = False
        End Sub

        Private Sub PopulateFileExtensions()
            _ComboExtensionsNoEvent = True

            Dim OutputType As ImageImportOutputTypes = ComboOutputType.SelectedValue
            Dim hfeOnly = OutputType = ImageImportOutputTypes.HFE AndAlso _SelectedDevice.AllowHFE

            If hfeOnly Then
                Dim items As New List(Of FileExtensionItem) From {
                    New FileExtensionItem(".hfe", Nothing)
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
                Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue
                SharedLib.PopulateFileExtensions(ComboExtensions, ImageParams.Format)
            End If

            If ComboExtensions.SelectedIndex = -1 AndAlso ComboExtensions.Items.Count > 0 Then
                ComboExtensions.SelectedIndex = 0
            End If

            ComboExtensions.Enabled = (ComboExtensions.Items.Count > 1)

            _ComboExtensionsNoEvent = False
        End Sub

        Private Sub PopulateOutputTypes()
            _ComboOutputTypeNoEvent = True

            Dim DriveList As New List(Of KeyValuePair(Of String, ImageImportOutputTypes))
            Dim UseSectorImage As Boolean = AllowSectorImage()

            For Each OutputType As ImageImportOutputTypes In [Enum].GetValues(GetType(ImageImportOutputTypes))
                If OutputType = ImageImportOutputTypes.IMA AndAlso Not UseSectorImage AndAlso _SelectedDevice.AllowHFE Then
                    Continue For
                End If

                If OutputType = ImageImportOutputTypes.HFE AndAlso Not _SelectedDevice.AllowHFE Then
                    Continue For
                End If

                DriveList.Add(New KeyValuePair(Of String, ImageImportOutputTypes)(
                    ImageImportOutputTypeDescription(OutputType), OutputType)
                )
            Next

            InitializeCombo(ComboOutputType, DriveList, Nothing)

            If ComboOutputType.SelectedIndex = -1 AndAlso ComboOutputType.Items.Count > 0 Then
                ComboOutputType.SelectedIndex = 0
            End If

            ComboOutputType.Enabled = (ComboDevices.Items.Count > 1)

            _ComboOutputTypeNoEvent = False
        End Sub

        Private Sub ProcessImage()
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            If DiskParams.IsNonImage Then
                Exit Sub
            End If

            Dim OutputType As ImageImportOutputTypes = ComboOutputType.SelectedValue

            Dim FilePath = GenerateOutputFile(ImageImportOutputTypeFileExt(OutputType))
            If FilePath = "" Then
                Exit Sub
            End If

            ClearProcessedImage(True)

            _DoubleStep = CheckBoxDoublestep.Enabled AndAlso CheckBoxDoublestep.Checked

            Dim Arguments = GenerateCommandLine(FilePath)
            Process.StartAsync(_SelectedDevice.Settings.AppPath, Arguments)
        End Sub

        Private Sub ProcessImport()
            RaiseEvent ImportRequested(_OutputFilePath, GetNewFileName())

            ClearProcessedImage(False)
            ClearLoadedImage()
        End Sub

        Private Sub ProcessOutputLine(line As String)
            If TextBoxConsole.Text.Length > 0 Then
                TextBoxConsole.AppendText(Environment.NewLine)
            End If
            TextBoxConsole.AppendText(line)

            TrackStatus.ProcessOutputLineRead(line, ActionTypeEnum.Import, _DoubleStep)

            If TrackStatus.Failed Then
                Process.Cancel()
            End If
        End Sub

        Private Function ReadImageFormat() As DiskImage.FloppyDiskFormat
            Dim Response = ConvertFirstTrack()

            If Not Response.Result Then
                Return FloppyDiskFormat.FloppyUnknown
            End If

            Return DetectImageFormat(Response.Filename, True)
        End Function

        Private Sub RefreshButtonState()
            Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue
            Dim Is525DDStandard As Boolean = ImageParams.IsStandard AndAlso ImageParams.MediaType = FloppyMediaType.Media525DoubleDensity

            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)
            Dim HasInputFile As Boolean = Not String.IsNullOrEmpty(_InputFilePath)
            Dim IsRunning As Boolean = Process.IsRunning
            Dim IsIdle As Boolean = Not IsRunning

            Dim SettingsEnabled As Boolean = IsIdle AndAlso Not HasOutputfile
            Dim CanConfigure As Boolean = SettingsEnabled AndAlso HasInputFile


            ComboExtensions.Enabled = HasInputFile And ComboExtensions.Items.Count > 1
            ComboImageFormat.Enabled = CanConfigure

            If ComboOutputType IsNot Nothing Then
                ComboOutputType.Enabled = CanConfigure And ComboOutputType.Items.Count > 1
            End If

            ComboDevices.Enabled = IsIdle AndAlso Not HasOutputfile AndAlso ComboDevices.Items.Count > 1

            ButtonCancel.Text = If(IsRunning Or HasOutputfile, My.Resources.Label_Cancel, My.Resources.Label_Close)
            ButtonClear.Enabled = IsIdle AndAlso HasOutputfile
            ButtonOpen.Enabled = SettingsEnabled

            ButtonProcess.Enabled = Not ImageParams.IsNonImage AndAlso HasInputFile AndAlso (IsRunning Or Not HasOutputfile)
            ButtonProcess.Text = If(IsRunning, My.Resources.Label_Abort, My.Resources.Label_Process)

            ButtonSaveLog.Enabled = IsIdle AndAlso Not String.IsNullOrEmpty(TextBoxConsole.Text)

            TextBoxFileName.ReadOnly = Not HasInputFile

            If Is525DDStandard Then
                CheckBoxDoublestep.Enabled = CanConfigure AndAlso _TrackCount > 42
                CheckBoxDoublestep.Checked = _TrackCount > 79
            Else
                CheckBoxDoublestep.Enabled = False
                CheckBoxDoublestep.Checked = False
            End If

            CheckBoxExtendedLogging.Enabled = IsIdle AndAlso Not HasOutputfile

            RefreshImportButtonState()
        End Sub

        Private Sub RefreshDeviceState()
            CheckBoxExtendedLogging.Visible = _SelectedDevice.Device = FluxDevice.Kryoflux
        End Sub
        Private Sub RefreshImportButtonState()
            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)
            Dim HasInputFile As Boolean = Not String.IsNullOrEmpty(_InputFilePath)

            Dim EnableImport As Boolean = Not Process.IsRunning AndAlso HasOutputfile AndAlso HasInputFile AndAlso Not String.IsNullOrEmpty(TextBoxFileName.Text)

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

        Private Sub SetNewFileName()
            Dim FileExt = IO.Path.GetExtension(_InputFilePath).ToLower

            If FileExt = ".raw" Then
                Dim ParentFolder As String = IO.Path.GetFileName(IO.Directory.GetParent(_InputFilePath).FullName)
                TextBoxFileName.Text = ParentFolder
            Else
                TextBoxFileName.Text = IO.Path.GetFileNameWithoutExtension(_InputFilePath)
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

#Region "Events"
        Private Sub ButtonClear_Click(sender As Object, e As EventArgs) Handles ButtonClear.Click
            ClearProcessedImage(True)
            RefreshButtonState()
        End Sub

        Private Sub ButtonImport_Click(sender As Object, e As EventArgs) Handles ButtonImport.Click
            If _LaunchedFromDialog Then
                Exit Sub
            End If

            If _OutputFilePath <> "" Then
                ProcessImport()
            End If
        End Sub

        Private Sub ButtonOpen_Click(sender As Object, e As EventArgs) Handles ButtonOpen.Click
            If OpenFluxImage() Then
                InitializeDevice(True)
            End If
        End Sub

        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If CancelProcessIfRunning() Then
                Exit Sub
            End If

            ProcessImage()
        End Sub

        Private Sub CheckBoxExtendedLogging_CheckStateChanged(sender As Object, e As EventArgs) Handles CheckBoxExtendedLogging.CheckStateChanged
            If Not _Initialized Then
                Exit Sub
            End If

            _CachedExtendedLogging = CheckBoxExtendedLogging.Checked
        End Sub

        Private Sub ComboDevices_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboDevices.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboDevicesNoEvent Then
                Exit Sub
            End If

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

        Private Sub ComboOutputType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboOutputType.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboOutputTypeNoEvent Then
                Exit Sub
            End If

            PopulateFileExtensions()
        End Sub

        Private Sub ImageImportForm_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
            Dim SelectedDevice As FluxDeviceInfo = CType(ComboDevices.SelectedItem, FluxDeviceInfo)
            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)
            If Process.IsRunning Or HasOutputfile Then
                Return
            End If

            If Not e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Return
            End If

            Dim paths = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
            If paths Is Nothing OrElse paths.Length = 0 Then
                Exit Sub
            End If

            Dim firstPath = paths(0)

            Dim Response = IsValidFluxImport(firstPath, SelectedDevice.AllowSCP)
            If Response.Result Then
                If OpenFluxImage(Response.File) Then
                    InitializeDevice(True)
                End If
            End If
        End Sub

        Private Sub ImageImportForm_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)

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
                        TrackStatus.UpdateTrackStatusComplete(_DoubleStep)
                    Else
                        ClearOutputFile(True)
                        TrackStatus.UpdateTrackStatusError()
                    End If

                Case ConsoleProcessRunner.ProcessStateEnum.Error
                    ClearOutputFile(True)
                    TrackStatus.UpdateTrackStatusError()
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