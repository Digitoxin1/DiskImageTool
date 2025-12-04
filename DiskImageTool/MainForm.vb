Imports System.ComponentModel
Imports DiskImageTool.DiskImage

Public Class MainForm
    Private WithEvents FilePanelMain As FilePanel
    Private WithEvents HashPanelContextMenu As ContextMenuStrip
    Private WithEvents ImageCombo As LoadedImageList
    Private WithEvents ImageFilters As Filters.ImageFilters
    Private Const CONTEXT_MENU_HASH_KEY As String = "Hashes"
    Private _DriveAEnabled As Boolean = False
    Private _DriveBEnabled As Boolean = False
    Private _FileVersion As String = ""
    Private _LoadedFiles As LoadedFiles
    Private _SummaryPanel As SummaryPanel
    Private _Suppress_File_DragEnterEvent As Boolean = False
    Private _Suppress_ToolStripFATCombo_SelectedIndexChangedEvent As Boolean = False
    Private _ToolStripDiskTypeCombo As ToolStripComboBox
    Private _ToolStripDiskTypeLabel As ToolStripLabel
    Private _ToolStripFatCombo As ToolStripComboBox
    Private _ToolStripOEMNameCombo As ToolStripComboBox
    Private _ToolStripOEMNameLabel As ToolStripLabel
    Private _ToolStripSearchText As ToolStripSpringTextBox

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()

        InitToolStripTop()
    End Sub

    Private Async Sub CheckForUpdatesStartup()
        Dim Result = Await Task.Run(Function()
                                        Return CheckIfAppUpdateAvailable()
                                    End Function)

        MainMenuUpdateAvailable.Visible = Result
    End Sub

    Private Sub ClearCurentFileName()
        Me.Text = GetWindowCaption()
        StatusBarFileName.Visible = False
    End Sub

    Private Sub ClearFiltersIfApplied(ResetSubFilters As Boolean, UpdateMenuItems As Boolean)
        If ImageFilters.FiltersApplied Then
            FiltersClear(ResetSubFilters)
            If UpdateMenuItems Then
                ImageFilters.UpdateAllMenuItems()
                ContextMenuFilters.Invalidate()
            End If
            StatusBarImageCountUpdate()
        End If
    End Sub

    Private Sub DetectFloppyDrives()
        Dim AllDrives() = IO.DriveInfo.GetDrives()
        _DriveAEnabled = False
        _DriveBEnabled = False

        For Each Drive In AllDrives
            If Drive.Name = "A:\" Then
                If Drive.DriveType = IO.DriveType.Removable Then
                    _DriveAEnabled = True
                End If
            End If
            If Drive.Name = "B:\" Then
                If Drive.DriveType = IO.DriveType.Removable Then
                    _DriveBEnabled = True
                End If
            End If
        Next

        MenuDiskReadFloppyA.Enabled = _DriveAEnabled
        MenuDiskReadFloppyB.Enabled = _DriveBEnabled
        MenuDiskWriteFloppyA.Enabled = False
        MenuDiskWriteFloppyB.Enabled = False
    End Sub

    Private Function DiskImageCloseAll(CurrentImage As DiskImageContainer) As Boolean
        Dim ModifyImageList = ImageCombo.GetModifiedImageList()

        If ModifyImageList.Count = 0 Then
            ResetAll()
            Return True
        End If

        Dim ShowDialog As Boolean = True
        Dim BatchResult As MyMsgBoxResult = MyMsgBoxResult.Yes
        Dim Result As MyMsgBoxResult

        For Each ImageData In ModifyImageList
            Dim NewFilePath As String = ""

            If ShowDialog Then
                If ModifyImageList.Count = 1 Then
                    Result = MsgBoxSave(ImageData.FileName)
                Else
                    Result = MsgBoxSaveAll(ImageData.FileName)
                End If
            Else
                Result = BatchResult
            End If

            If Result = MyMsgBoxResult.YesToAll OrElse Result = MyMsgBoxResult.NoToAll Then
                ShowDialog = False
                If Result = MyMsgBoxResult.NoToAll Then
                    BatchResult = MyMsgBoxResult.No
                Else
                    BatchResult = MyMsgBoxResult.Yes
                End If
                Result = BatchResult
            End If

            Select Case Result
                Case MyMsgBoxResult.No
                    Continue For

                Case MyMsgBoxResult.Cancel
                    Return False

                Case MyMsgBoxResult.Yes
                    If ImageData.ReadOnly OrElse ImageData.FileType = ImageData.FileTypeEnum.NewImage Then
                        If Not ShowDialog Then
                            If MsgBoxNewFileName(ImageData.FileName) <> MsgBoxResult.Ok Then
                                Return False
                            End If
                        End If

                        NewFilePath = GetNewFilePath(CurrentImage, ImageData, _LoadedFiles)
                        If String.IsNullOrEmpty(NewFilePath) Then
                            Return False
                        End If
                    End If

                    Dim image = DiskImageLoadIfNeeded(CurrentImage, ImageData)
                    If image Is Nothing Then
                        Return False
                    End If

                    If Not DiskImageSave(image, NewFilePath) Then
                        Return False
                    End If

                    ImageFilters.ScanModified(image.Disk, image.ImageData)
            End Select
        Next

        ResetAll()
        Return True
    End Function

    Private Sub DiskImageSaveAll(FilePanel As FilePanel)
        Dim RefreshCurrent As Boolean = False

        For Index = 0 To ImageCombo.Main.Count - 1
            Dim NewFilePath As String = ""
            Dim DoSave As Boolean = True
            Dim ImageData As ImageData = ImageCombo.Main(Index)

            If Not ImageData.IsModified Then Continue For

            If ImageData.ReadOnly Then
                If MsgBoxNewFileName(ImageData.FileName) = MsgBoxResult.Ok Then
                    NewFilePath = GetNewFilePath(FilePanel.CurrentImage, ImageData, _LoadedFiles)
                    DoSave = (NewFilePath <> "")
                Else
                    DoSave = False
                End If

            ElseIf ImageData.FileType = ImageData.FileTypeEnum.NewImage Then
                NewFilePath = GetNewFilePath(FilePanel.CurrentImage, ImageData, _LoadedFiles)
                DoSave = (NewFilePath <> "")
            End If

            If DoSave Then
                Dim Image = DiskImageLoadIfNeeded(FilePanel.CurrentImage, ImageData)

                If Image Is Nothing Then Continue For

                If DiskImageSave(Image, NewFilePath) Then
                    ImageFilters.ScanModified(Image.Disk, Image.ImageData)

                    RefreshDisplayPath(ImageData, False)

                    If ImageData Is ImageCombo.SelectedImage Then
                        SetCurrentFileName(ImageData)
                        FilePanel.ClearModifiedFlag()
                        RefreshCurrent = True
                    End If
                End If
            End If
        Next Index

        ImageFilters.UpdateMenuItem(Filters.FilterTypes.ModifiedFiles)
        RefreshModifiedCount()

        If RefreshCurrent Then
            RefreshCurrentState(FilePanel)
        End If
    End Sub

    Private Sub DiskImageSaveCurrent(FilePanel As FilePanel, NewFileName As Boolean)
        Dim NewFilePath As String = ""

        If NewFileName Then
            NewFilePath = GetNewFilePath(FilePanel.CurrentImage, _LoadedFiles)
            If NewFilePath = "" Then
                Exit Sub
            End If
        End If

        If DiskImageSave(FilePanel.CurrentImage, NewFilePath) Then
            ImageFilters.ScanModified(FilePanel.CurrentImage.Disk, FilePanel.CurrentImage.ImageData)
            ImageFilters.UpdateMenuItem(Filters.FilterTypes.ModifiedFiles)
            RefreshModifiedCount()

            RefreshDisplayPath(FilePanel.CurrentImage.ImageData, True)

            FilePanel.ClearModifiedFlag()
            RefreshCurrentState(FilePanel)
        End If
    End Sub

    Private Sub DiskImagesScan(CurrentImage As DiskImageContainer, NewOnly As Boolean)
        Me.UseWaitCursor = True
        Dim T = Stopwatch.StartNew

        MenuFiltersScanNew.Visible = False
        MenuFiltersScan.Enabled = False

        ClearFiltersIfApplied(False, False)

        Dim images As New List(Of ImageData)
        For Each img As ImageData In ImageCombo.Main
            images.Add(img)
        Next

        Dim scanner As New FilterScanner(images, CurrentImage, NewOnly, AddressOf Me.ImageFiltersScanAll)

        Dim cts As New Threading.CancellationTokenSource()

        Dim scanTask = Task.Run(Sub() scanner.Scan(cts.Token))

        ItemScanForm.Display(scanner, My.Resources.Caption_ScanImages, My.Resources.Label_Scanning, cts, Me)

        MenuFiltersScanNew.Visible = scanner.ItemsRemaining > 0

        If ImageFilters.ExportUnknownImages Then
            App.Globals.TitleDB.SaveNewXML()
        End If
        'App.Globals.TitleDB.SaveXML()

        ImageFilters.UpdateAllMenuItems()
        ImageFilters.SubFiltersPopulate()
        ImageFilters.ScanRun = True

        RefreshModifiedCount()

        SubFiltersToggle(True)

        MenuFiltersScan.Text = My.Resources.Caption_RescanImages
        MenuFiltersScan.Enabled = True

        T.Stop()
        Debug.Print(String.Format(My.Resources.Debug_ScanTimeTaken, T.Elapsed))
        Me.UseWaitCursor = False

        ImageFiltersAlertUser()
    End Sub

    Private Sub DrawComboFAT(ByVal sender As Object, ByVal e As DrawItemEventArgs)
        e.DrawBackground()

        If e.Index >= 0 Then
            Dim Item As String = _ToolStripFatCombo.Items(e.Index)

            Dim Brush As Brush
            Dim tBrush As Brush

            If e.State And DrawItemState.Selected Then
                Brush = SystemBrushes.Highlight
                tBrush = SystemBrushes.HighlightText
            Else
                Brush = SystemBrushes.Window
                tBrush = SystemBrushes.WindowText
            End If

            e.Graphics.FillRectangle(Brush, e.Bounds)
            e.Graphics.DrawString(Item, e.Font, tBrush, e.Bounds, StringFormat.GenericDefault)
        End If

        e.DrawFocusRectangle()
    End Sub

    Private Function FATComboAdd() As ToolStripComboBox
        Dim Combo As New ToolStripComboBox With {
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.System,
            .AutoSize = False,
            .DropDownWidth = 25,
            .Size = New Drawing.Size(25, 23)
        }

        Combo.ComboBox.DrawMode = DrawMode.OwnerDrawFixed
        AddHandler Combo.ComboBox.DrawItem, AddressOf DrawComboFAT
        AddHandler Combo.SelectedIndexChanged, AddressOf ToolStripFATCombo_SelectedIndexChanged

        ToolStripTop.Items.Add(Combo)

        Return Combo
    End Function

    Private Sub FATSubMenuRefresh(CurrentImage As DiskImageContainer, FATTablesMatch As Boolean)
        For Each Item As ToolStripMenuItem In MenuEditFAT.DropDownItems
            RemoveHandler Item.Click, AddressOf MenuEditFAT_Click
        Next

        MenuEditFAT.DropDownItems.Clear()
        MenuEditFAT.Tag = Nothing
        _ToolStripFatCombo.Items.Clear()

        If CurrentImage IsNot Nothing AndAlso CurrentImage.Disk IsNot Nothing AndAlso CurrentImage.Disk.IsValidImage Then
            If FATTablesMatch Then
                MenuEditFAT.Tag = 0
            Else
                For Counter = 0 To CurrentImage.Disk.BPB.NumberOfFATs - 1
                    Dim Item As New ToolStripMenuItem With {
                       .Text = "FAT &" & Counter + 1,
                       .Tag = Counter
                    }
                    MenuEditFAT.DropDownItems.Add(Item)
                    AddHandler Item.Click, AddressOf MenuEditFAT_Click
                    _ToolStripFatCombo.Items.Add("FAT " & Counter + 1)
                Next
                _Suppress_ToolStripFATCombo_SelectedIndexChangedEvent = True
                If CurrentImage.ImageData Is Nothing Then
                    _ToolStripFatCombo.SelectedIndex = 0
                Else
                    If CurrentImage.ImageData.FATIndex > CurrentImage.Disk.BPB.NumberOfFATs - 1 Or FATTablesMatch Then
                        CurrentImage.ImageData.FATIndex = 0
                    End If
                    _ToolStripFatCombo.SelectedIndex = CurrentImage.ImageData.FATIndex
                End If
                _Suppress_ToolStripFATCombo_SelectedIndexChangedEvent = False
            End If
        End If
    End Sub


    Private Function FileCloseCurrent(CurrentImage As DiskImageContainer)
        If DiskImageCloseCurrent(CurrentImage, _LoadedFiles) Then
            FileRemove(CurrentImage.ImageData)
            ImageCombo.RefreshPaths()

            Return True
        End If

        Return False
    End Function

    Private Sub FileDropPostProcess()
        LabelDropMessage.Visible = False
        StatusBarImageCountUpdate()
        SetImagesLoaded(True)
    End Sub

    Private Sub FileRemove(ImageData As ImageData)
        _LoadedFiles.FileNames.Remove(ImageData.DisplayPath)

        ImageCombo.RemoveImage(ImageData)

        If ImageCombo.Main.Count = 0 Then
            ResetAll()
        Else
            ImageFilters.ScanRemove(ImageData)
            RefreshModifiedCount()
            StatusBarImageCountUpdate()
        End If
    End Sub

    Private Sub FilesOpen()
        Dim FileFilter = GetLoadDialogFilters()
        Dim FileNames As String() = Nothing

        Using Dialog As New OpenFileDialog With {
                .Filter = FileFilter,
                .Multiselect = True
            }

            If Dialog.ShowDialog <> DialogResult.OK Then
                Exit Sub
            End If

            FileNames = Dialog.FileNames
        End Using

        ProcessFileDrop(FileNames, True)
    End Sub

    Private Function FilterComboAdd(Width As Integer, Sorted As Boolean) As ToolStripComboBox
        Dim Combo As New ToolStripComboBox With {
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.System,
            .AutoSize = False,
            .DropDownWidth = Width,
            .Sorted = Sorted,
            .Alignment = ToolStripItemAlignment.Right,
            .Overflow = ToolStripItemOverflow.Never,
            .Size = New Drawing.Size(Width, 25)
        }
        ToolStripTop.Items.Add(Combo)

        Return Combo
    End Function

    Private Function FilterLabelAdd(Text As String) As ToolStripLabel
        Dim Label As New ToolStripLabel With {
            .Text = Text,
            .Alignment = ToolStripItemAlignment.Right,
            .Overflow = ToolStripItemOverflow.Never,
            .Margin = New Padding(8, 1, 0, 2),
            .Size = New Drawing.Size(0, 22)
        }
        ToolStripTop.Items.Add(Label)

        Return Label
    End Function

    Private Sub FiltersApply(ResetSubFilters As Boolean)
        Dim HasFilter As Boolean = False
        Dim AppliedFilters As Long = 0

        If ResetSubFilters Then
            SubFiltersClearFilter()
        End If

        Dim FiltersChecked As Boolean = ImageFilters.AreFiltersApplied()
        If FiltersChecked Then
            AppliedFilters = ImageFilters.GetAppliedFilters(True)
        End If
        HasFilter = HasFilter Or FiltersChecked

        Dim TextFilterRegex = _ImageFilters.TextFilterGetRegex()
        HasFilter = HasFilter Or (TextFilterRegex IsNot Nothing)

        Dim OEMNameItem As ComboFilterItem = _ToolStripOEMNameCombo.SelectedItem
        Dim HasOEMNameFilter = OEMNameItem IsNot Nothing AndAlso Not OEMNameItem.AllItems
        HasFilter = HasFilter Or HasOEMNameFilter

        Dim DiskTypeItem As ComboFilterItem = _ToolStripDiskTypeCombo.SelectedItem
        Dim HasDiskTypeFilter = DiskTypeItem IsNot Nothing AndAlso Not DiskTypeItem.AllItems
        HasFilter = HasFilter Or HasDiskTypeFilter

        If HasFilter Then
            Cursor.Current = Cursors.WaitCursor

            If ResetSubFilters Then
                ImageFilters.SubFiltersClear(False)
            End If

            ImageCombo.Filtered.Clear()

            For Each ImageData As ImageData In ImageCombo.Main
                Dim ShowItem As Boolean = True

                If ShowItem AndAlso FiltersChecked Then
                    ShowItem = Not Filters.ImageFilters.IsFiltered(ImageData, AppliedFilters, ImageFilters.FilterCounts)
                End If

                If ShowItem AndAlso ResetSubFilters Then
                    ImageFilters.SubFilterAdd(ImageData)
                End If

                If ShowItem AndAlso TextFilterRegex IsNot Nothing Then
                    ShowItem = TextFilterRegex.IsMatch(ImageData.DisplayPath)
                End If

                If ShowItem AndAlso HasOEMNameFilter Then
                    Dim IsValidImage = Not ImageData.Filter(Filters.FilterTypes.Disk_UnknownFormat)
                    ShowItem = IsValidImage And OEMNameItem.Name = ImageData.OEMName
                End If

                If ShowItem AndAlso HasDiskTypeFilter Then
                    ShowItem = DiskTypeItem.Name = ImageData.DiskType
                End If

                If ShowItem Then
                    ImageCombo.Filtered.Add(ImageData)
                End If
            Next

            ImageFilters.UpdateAllMenuItems()
            ImageFilters.FiltersApplied = True

            If ResetSubFilters Then
                ImageFilters.SubFiltersPopulate()
            End If

            ImageCombo.Refresh(True)

            RefreshFilterButtons(True)

            Cursor.Current = Cursors.Default

            StatusBarImageCountUpdate()
        Else
            ClearFiltersIfApplied(True, True)
        End If
    End Sub

    Private Sub FiltersClear(ResetSubFilters As Boolean)
        Cursor.Current = Cursors.WaitCursor

        Dim FiltersApplied = ImageFilters.AreFiltersApplied()

        ImageFilters.Clear()
        SubFiltersClearFilter()

        If FiltersApplied Or ResetSubFilters Then
            ImageFilters.SubFiltersPopulateUnfiltered(ImageCombo.Main)
        End If

        ImageCombo.Refresh(False)

        RefreshFilterButtons(False)

        Cursor.Current = Cursors.Default
    End Sub

    Private Sub FiltersReset()
        ImageFilters.Reset()
        SubFiltersReset()

        RefreshModifiedCount()

        MenuFiltersScan.Text = My.Resources.Caption_ScanImages
        RefreshFilterButtons(False)
    End Sub

    Private Function FilterTextBoxAdd() As ToolStripSpringTextBox
        Dim TextBox As New ToolStripSpringTextBox With {
            .Alignment = ToolStripItemAlignment.Right,
            .BorderStyle = BorderStyle.FixedSingle,
            .Font = New Font("Segoe UI", 9),
            .Margin = New Padding(1, 0, 0, 0),
            .MaxLength = 255,
            .MaxWidth = 195,
            .Overflow = ToolStripItemOverflow.Never,
            .Size = New Drawing.Size(195, 25)
        }

        ToolStripTop.Items.Add(TextBox)

        Return TextBox
    End Function

    Private Sub FluxConvertImage()
        Dim FileName As String = Flux.OpenFluxImage(Me, True)

        FluxConvertImage(FileName)
    End Sub

    Private Sub FluxConvertImage(FileName As String)
        If FileName <> "" Then
            Dim Response = Flux.ConvertFluxImage(Me, FileName, True, AddressOf ProcessImportedImage, False)
            If Response.Result = DialogResult.OK Then
                ProcessImportedImage(Response.OutputFile, Response.NewFileName)
            End If
        End If
    End Sub

    Private Function GetWindowCaption() As String
        Return My.Application.Info.ProductName & " v" & _FileVersion
    End Function

    Private Sub GreaseweazleReadImage()
        Dim Response = Flux.Greaseweazle.ReadFluxImage(Me, AddressOf ProcessImportedImage)
        If Response.Result Then
            ProcessImportedImage(Response.OutputFile, Response.NewFileName)
        End If
    End Sub

    Private Sub HandleDragDrop(Files() As String)
        If Files.Length = 1 Then
            Dim CanProcessFlux = Flux.Greaseweazle.Settings.IsPathValid Or Flux.Kryoflux.Settings.IsPathValid
            If CanProcessFlux Then
                Dim Result = ProcessFileDropFlux(Files(0))
                If Result Then
                    Exit Sub
                End If
            End If
        End If

        ProcessFileDrop(Files, True)
    End Sub

    Private Function HandleImageDiscovered(Args As ImageDiscoveredEventArgs) As ImageData
        Dim Image = _LoadedFiles.Add(Args.Key, Args.FileName, Args.FileType, Args.CompressedFile, Args.NewFileName)

        If Image Is Nothing Then
            Return Nothing
        End If

        ImageCombo.Main.Add(Image)

        If Image.FileType = ImageData.FileTypeEnum.NewImage Then
            ImageFilters.FilterUpdate(Image, True, Filters.FilterTypes.ModifiedFiles, True, True)
            ImageFiltersSetModified(Image)
        End If

        Return Image
    End Function

    Private Sub HashPanelInitContextMenu()
        HashPanelContextMenu = New ContextMenuStrip With {
            .Name = CONTEXT_MENU_HASH_KEY
        }
        HashPanelContextMenu.Items.Add(My.Resources.Menu_CopyValue)
        For Each Control As Control In FlowLayoutPanelHashes.Controls
            Control.ContextMenuStrip = HashPanelContextMenu
        Next
    End Sub

    Private Sub HashPanelPopulate(Disk As Disk, MD5 As String)
        If Disk IsNot Nothing Then
            LabelCRC32.Text = Disk.Image.GetCRC32
            LabelMD5.Text = MD5
            LabelSHA1.Text = Disk.Image.GetSHA1Hash
            HashPanelSetVisible(True)
        Else
            LabelCRC32.Text = ""
            LabelMD5.Text = ""
            LabelSHA1.Text = ""
            HashPanelSetVisible(False)
        End If
        FlowLayoutPanelHashes.Refresh()
    End Sub

    Private Sub HashPanelSetVisible(Visible As Boolean)
        For Each Control As Control In FlowLayoutPanelHashes.Controls
            Control.Visible = Visible
        Next
    End Sub

    Private Sub ImageFiltersAlertUser()
        Dim Handle = WindowsAPI.GetForegroundWindow()
        If Handle = Me.Handle Then
            MainMenuFilters.ShowDropDown()
        Else
            WindowsAPI.FlashWindow(Me.Handle, True, True, 5, True)
        End If
    End Sub

    Private Sub ImageFiltersScanAll(Disk As Disk, ImageData As ImageData)
        ImageFilters.ScanAll(Disk, ImageData)
    End Sub

    Private Sub ImageFiltersSetModified(ImageData As ImageData)
        ImageFilters.FilterUpdate(ImageData, True, Filters.FilterTypes.ModifiedFiles, True, True)
    End Sub

    Private Sub ImageFiltersUpdate(Image As DiskImageContainer)
        ImageFilters.ScanUpdate(Image)
        RefreshModifiedCount()
        ImageCombo.RefreshCurrentItemText()
    End Sub

    Private Sub ImageNew()
        Dim Response = ImageCreationForm.Display(Me)

        If Response.Data Is Nothing Then
            Exit Sub
        End If

        Dim FileName = FloppyDiskNewImage(Response.Data, Response.DiskFormat, _LoadedFiles.FileNames)
        If FileName.Length > 0 Then
            ProcessFileDropNew(FileName)
            If Response.ImportFiles Then
                NewImageImport(FilePanelMain, FileName)
            End If
        End If
    End Sub

    Private Sub ImageRemoveWindowsModificationsBatch(FilePanel As FilePanel)
        Dim Msg = String.Format(My.Resources.Dialog_Win9xCleanBatch, Environment.NewLine)

        If Not MsgBoxQuestion(Msg) Then
            Exit Sub
        End If

        Me.UseWaitCursor = True
        Dim T = Stopwatch.StartNew

        Dim images As New List(Of ImageData)
        For Each img As ImageData In ImageCombo.Main
            images.Add(img)
        Next

        Dim scanner As New Win9xCleanScanner(images, FilePanel.CurrentImage, AddressOf Me.ImageWin9xCleanBatch)

        Dim cts As New Threading.CancellationTokenSource()

        Dim scanTask = Task.Run(Sub() scanner.Scan(cts.Token))

        ItemScanForm.Display(scanner, My.Resources.Caption_CleanImages, My.Resources.Label_Processing, cts, Me)

        ImageFilters.UpdateAllMenuItems()
        If ImageFilters.ScanRun Then
            ImageFilters.SubFiltersPopulate()
        End If

        RefreshModifiedCount()

        Dim UpdateCount As Integer = 0
        For Index = 0 To ImageCombo.Main.Count - 1
            Dim ImageData As ImageData = ImageCombo.Main(Index)
            If ImageData.BatchUpdated Then
                If ImageData Is FilePanel.CurrentImage.ImageData Then
                    DiskImageRefresh(FilePanel)
                End If
                ImageData.BatchUpdated = False
                UpdateCount += 1
            End If
        Next Index

        T.Stop()
        Debug.Print(String.Format(My.Resources.Debug_BatchProcessTimeTaken, T.Elapsed))
        Me.UseWaitCursor = False

        If UpdateCount = 1 Then
            Msg = String.Format(My.Resources.Dialog_SuccessfulCleanCountSingular, UpdateCount)
        Else
            Msg = String.Format(My.Resources.Dialog_SuccessfulCleanCountPlural, UpdateCount)
        End If

        MsgBox(Msg, MsgBoxStyle.Information)

        If UpdateCount > 0 Then
            FiltersApply(True)
        End If
    End Sub

    Private Sub ImageWin9xCleanBatch(Disk As Disk, ImageData As ImageData)
        Dim Result = ImageRemoveWindowsModifications(Disk, True)
        ImageData.BatchUpdated = Result
        If Result Then
            ImageFilters.ScanWin9xClean(Disk, ImageData)
        End If
    End Sub
    Private Sub InitButtonState(CurrentImage As DiskImageContainer)
        Dim Disk As Disk = Nothing
        Dim FATTablesMatch As Boolean = True

        If CurrentImage IsNot Nothing Then
            Disk = CurrentImage.Disk
        End If

        Dim PrevVisible = _ToolStripFatCombo.Visible

        If Disk IsNot Nothing Then
            Dim IsValidImage = Disk.IsValidImage
            Dim CheckSize = Disk.CheckSize
            FATTablesMatch = Disk.DiskParams.IsXDF OrElse Disk.FATTables.FATsMatch

            MenuHexBootSector.Enabled = CheckSize
            MenuEditBootSector.Enabled = CheckSize
            MenuHexDisk.Enabled = CheckSize
            MenuHexFAT.Enabled = IsValidImage
            MenuEditFAT.Enabled = IsValidImage
            MenuHexDirectory.Enabled = IsValidImage
            ToolStripSeparatorFAT.Visible = Not FATTablesMatch
            _ToolStripFatCombo.Visible = Not FATTablesMatch
            _ToolStripFatCombo.Width = 60
            MenuDiskWriteFloppyA.Enabled = CheckSize AndAlso _DriveAEnabled
            MenuDiskWriteFloppyB.Enabled = CheckSize AndAlso _DriveBEnabled
            MenuReportsModifications.Enabled = Disk.Image.IsBitstreamImage
            SetButtonStateSaveAs(True)
            MenuGreaseweazleWrite.Enabled = CheckSize AndAlso App.AppSettings.Greaseweazle.IsPathValid
        Else
            MenuHexBootSector.Enabled = False
            MenuHexDisk.Enabled = False
            MenuHexFAT.Enabled = False
            MenuEditBootSector.Enabled = False
            MenuEditFAT.Enabled = False
            MenuHexDirectory.Enabled = False
            ToolStripSeparatorFAT.Visible = False
            _ToolStripFatCombo.Visible = False
            MenuDiskWriteFloppyA.Enabled = False
            MenuDiskWriteFloppyB.Enabled = False
            MenuReportsModifications.Enabled = False
            SetButtonStateSaveAs(False)
            MenuGreaseweazleWrite.Enabled = False
        End If

        SetButtonStateClose(CurrentImage IsNot Nothing)
        RefreshDirectoryMenu(Nothing, Nothing)
        FATSubMenuRefresh(CurrentImage, FATTablesMatch)

        If _ToolStripFatCombo.Visible <> PrevVisible Then
            ToolStripTop.Refresh()
        End If
    End Sub

    Private Sub InitDebugFeatures(Enabled As Boolean)
        If Enabled Then
            MainMenuOptions.DropDownItems.Add(New ToolStripSeparator)

            Dim Item As New ToolStripMenuItem With {
                .Name = "MenuOptionsExportUnknown",
                .Text = My.Resources.Menu_ExportUnknown,
                .CheckOnClick = True,
                .Checked = ImageFilters.ExportUnknownImages
            }

            AddHandler Item.CheckStateChanged, AddressOf MenuOptionsExportUnknown_CheckStateChanged

            MainMenuOptions.DropDownItems.Add(Item)

            Item = New ToolStripMenuItem With {
                .Name = "MenuOptionsEnableWriteSpliceFilter",
                .Text = My.Resources.Menu_EnableWriteSpliceFilter,
                .CheckOnClick = True,
                .Checked = ImageFilters.EnableWriteSpliceFilter
            }

            AddHandler Item.CheckStateChanged, AddressOf MenuOptionsEnableWriteSpliceFilter_CheckStateChanged

            MainMenuOptions.DropDownItems.Add(Item)
        End If
    End Sub

    Private Sub InitOptionsMenu()
        MenuOptionsCreateBackup.Checked = App.Globals.AppSettings.CreateBackups
        MenuOptionsCheckUpdate.Checked = App.Globals.AppSettings.CheckUpdateOnStartup
        MenuOptionsDragDrop.Checked = App.Globals.AppSettings.DragAndDrop
        MenuOptionsDisplayTitles.Checked = App.Globals.AppSettings.DisplayTitles

        PopulateLanguages()
    End Sub

    Private Sub InitToolStripTop()
        _ToolStripFatCombo = FATComboAdd()

        _ToolStripSearchText = FilterTextBoxAdd()
        FilterLabelAdd(My.Resources.Label_Search)

        _ToolStripOEMNameCombo = FilterComboAdd(125, True)
        _ToolStripOEMNameLabel = FilterLabelAdd(My.Resources.Label_OEMName)

        _ToolStripDiskTypeCombo = FilterComboAdd(95, False)
        _ToolStripDiskTypeLabel = FilterLabelAdd(My.Resources.Label_DiskFormat)
    End Sub

    Private Sub InitUpdateCheck()
        MainMenuUpdateAvailable.Visible = False

        If App.Globals.AppSettings.CheckUpdateOnStartup Then
            CheckForUpdatesStartup()
        End If
    End Sub

    Private Sub LaunchNewInstance(FilePath As String)
        Process.Start(Application.ExecutablePath, """" & FilePath & """")
    End Sub

    Private Sub LocalizeForm()
        MainMenuFile.Text = My.Resources.Menu_File
        MenuDiskReadFloppyA.Text = String.Format(My.Resources.Menu_ReadDiskInDrive, "A")
        MenuDiskReadFloppyB.Text = String.Format(My.Resources.Menu_ReadDiskInDrive, "B")
        MenuDiskWriteFloppyA.Text = String.Format(My.Resources.Menu_WriteDiskInDrive, "A")
        MenuDiskWriteFloppyB.Text = String.Format(My.Resources.Menu_WriteDiskInDrive, "B")
        MenuEditBootSector.Text = My.Resources.Menu_BootSector
        MenuEditFAT.Text = My.Resources.Menu_FAT
        MenuEditFileProperties.Text = My.Resources.Menu_FileProperties
        MenuEditImportFiles.Text = My.Resources.Menu_ImportFiles
        MenuEditRedo.Text = My.Resources.Menu_Redo
        MenuEditReplaceFile.Text = My.Resources.Menu_ReplaceFile
        MenuEditUndo.Text = My.Resources.Menu_Undo
        MenuFileClose.Text = My.Resources.Menu_Close
        MenuFileCloseAll.Text = My.Resources.Menu_CloseAll
        MenuFileOpen.Text = My.Resources.Menu_Open
        MenuFileSave.Text = My.Resources.Menu_Save
        MenuFileSaveAll.Text = My.Resources.Menu_SaveAll
        MenuFileSaveAs.Text = My.Resources.Menu_SaveAs
        MenuHexBootSector.Text = My.Resources.Menu_BootSector
        MenuHexFAT.Text = My.Resources.Menu_FAT
        MenuOptionsDisplayLanguage.Text = My.Resources.Label_Language
        MenuReportsModifications.Text = My.Resources.Label_Modifications
        MenuToolsTruncateImage.Text = My.Resources.Menu_TruncateImage
        StatusBarModified.Text = My.Resources.Label_Modified
        ToolStripClose.Text = WithoutHotkey(My.Resources.Menu_Close)
        ToolStripCloseAll.Text = WithoutHotkey(My.Resources.Menu_CloseAll)
        ToolStripFileProperties.Text = WithoutHotkey(My.Resources.Menu_FileProperties)
        ToolStripOpen.Text = WithoutHotkey(My.Resources.Menu_Open)
        ToolStripRedo.Text = WithoutHotkey(My.Resources.Menu_Redo)
        ToolStripSave.Text = WithoutHotkey(My.Resources.Menu_Save)
        ToolStripSaveAll.Text = WithoutHotkey(My.Resources.Menu_SaveAll)
        ToolStripSaveAs.Text = WithoutHotkey(My.Resources.Menu_SaveAs)
        ToolStripUndo.Text = WithoutHotkey(My.Resources.Menu_Undo)
        ToolStripImportFiles.ToolTipText = My.Resources.Label_ImportFiles
        MainMenuEdit.Text = My.Resources.Menu_Edit
        MenuHelpAbout.Text = String.Format(My.Resources.Label_AboutApp, My.Application.Info.Title)
    End Sub

    Private Sub MenuHexDirectorySubMenuClear()
        For Each Item As ToolStripMenuItem In MenuHexDirectory.DropDownItems
            RemoveHandler Item.Click, AddressOf MenuHexDirectory_Click
        Next
        MenuHexDirectory.DropDownItems.Clear()
        MenuHexDirectory.Text = My.Resources.Menu_RootDirectory
    End Sub

    Private Sub MenuHexDirectorySubMenuItemAdd(Path As String, Directory As IDirectory)
        Dim Item As New ToolStripMenuItem With {
            .Text = Path,
            .Tag = Directory
        }

        MenuHexDirectory.DropDownItems.Add(Item)

        AddHandler Item.Click, AddressOf MenuHexDirectory_Click
    End Sub

    Private Sub MenuHexDirectorySubMenuPopulate(Disk As Disk, Response As DirectoryScanResponse)
        MenuHexDirectorySubMenuClear()

        If Response.DirectoryList.Count > 0 Then
            MenuHexDirectory.Text = My.Resources.Label_Directory
            MenuHexDirectorySubMenuItemAdd(InParens(My.Resources.Label_Root), Disk.RootDirectory)
            MenuHexDirectory.Tag = Nothing
        Else
            MenuHexDirectory.Tag = Disk.RootDirectory
        End If

        For Each Item In Response.DirectoryList
            MenuHexDirectorySubMenuItemAdd(Item.Path, Item.Directory)
        Next
    End Sub

    Private Sub MenuOptionsAddLanguage(Text As String, Tag As String)
        Dim Item As New ToolStripMenuItem With {
            .Text = Text,
            .Tag = Tag
        }

        If App.Globals.AppSettings.Language = Item.Tag Then
            Item.Checked = True
        End If

        AddHandler Item.Click, AddressOf BtnLanguage_Click
        MenuOptionsDisplayLanguage.DropDownItems.Add(Item)
    End Sub

    Private Sub MenuRawTrackDataSubMenuClear()
        For Each Item As ToolStripMenuItem In MenuHexRawTrackData.DropDownItems
            RemoveHandler Item.Click, AddressOf MenuHexRawTrackData_Click
        Next
        MenuHexRawTrackData.DropDownItems.Clear()
    End Sub

    Private Sub MenuRawTrackDataSubMenuItemAdd(Track As Integer, Text As String)
        Dim Item As New ToolStripMenuItem With {
           .Text = Text,
           .Tag = Track
       }
        MenuHexRawTrackData.DropDownItems.Add(Item)
        AddHandler Item.Click, AddressOf MenuHexRawTrackData_Click
    End Sub

    Private Sub OpenImageInNewInstance(CurrentImage As DiskImageContainer)
        If FileCloseCurrent(CurrentImage) Then
            If CurrentImage.ImageData.FileType <> ImageData.FileTypeEnum.NewImage Then
                Dim LaunchPath = CurrentImage.ImageData.SourceFile
                If CurrentImage.ImageData.FileType = ImageData.FileTypeEnum.Compressed Then
                    LaunchPath &= "|" & CurrentImage.ImageData.CompressedFile
                End If
                LaunchNewInstance(LaunchPath)
            End If
        End If
    End Sub

    Private Sub PopulateLanguages()
        MenuOptionsDisplayLanguage.DropDownItems.Clear()

        MenuOptionsAddLanguage(My.Resources.Menu_SystemDefault, "")
        MenuOptionsAddLanguage(My.Resources.Menu_English, "en")

        Dim Languages = GetAvailableLanguages()

        For Each Language In Languages
            If Language.Name <> "en" Then
                MenuOptionsAddLanguage(Language.TextInfo.ToTitleCase(Language.NativeName), Language.Name)
            End If
        Next
    End Sub

    Private Sub PositionControls()
        btnRetry.Left = btnRetry.Parent.Width - btnRetry.Width - 20

        LabelDropMessage.Left = ListViewFiles.Left + (ListViewFiles.Width - LabelDropMessage.Width) \ 2
        LabelDropMessage.Top = ListViewFiles.Top + (ListViewFiles.Height - LabelDropMessage.Height) \ 2
    End Sub

    Private Sub PositionForm()
        Dim WorkingArea = Screen.FromControl(Me).WorkingArea
        Dim Width As Integer = Me.Width
        Dim Height As Integer = Me.Height

        If App.Globals.AppSettings.WindowWidth > 0 Then
            Width = App.Globals.AppSettings.WindowWidth
        End If
        If App.Globals.AppSettings.WindowHeight > 0 Then
            Height = App.Globals.AppSettings.WindowHeight
        End If

        Width = Math.Min(Width, WorkingArea.Width)
        Height = Math.Min(Height, WorkingArea.Height)

        Me.Size = New Size(Width, Height)
        Me.Location = New Point(WorkingArea.Left + (WorkingArea.Width - Width) / 2, WorkingArea.Top + (WorkingArea.Height - Height) / 2)
    End Sub

    Private Function ProcessFileDrop(File As String) As ImageData
        Return ProcessFileDrop({File}, False, False)
    End Function

    Private Function ProcessFileDrop(Files() As String, ShowDialog As Boolean) As ImageData
        Return ProcessFileDrop(Files, ShowDialog, False)
    End Function

    Private Function ProcessFileDrop(Files() As String, ShowDialog As Boolean, NewImage As Boolean, Optional NewFileName As String = "") As ImageData
        Cursor.Current = Cursors.WaitCursor
        Dim T = Stopwatch.StartNew

        ClearFiltersIfApplied(False, True)

        ImageData.StringOffset = 0

        Dim scanner As New ImageScanner()
        Dim firstImage As ImageData = Nothing

        AddHandler scanner.ImageDiscovered,
            Sub(sender, args)
                Dim run As Action = Sub()
                                        Dim img = HandleImageDiscovered(args)
                                        If firstImage Is Nothing AndAlso img IsNot Nothing Then
                                            firstImage = img
                                        End If
                                    End Sub
                ' Ensure we’re on the UI thread (important if Scan runs on a Task)
                If Me.InvokeRequired Then
                    Me.Invoke(run)
                Else
                    run()
                End If
            End Sub

        If ShowDialog Then
            Dim cts As New Threading.CancellationTokenSource()

            Dim scanTask = Task.Run(Sub() scanner.Scan(Files, NewImage, NewFileName, cts.Token))

            ImageLoadForm.Display(scanner, cts, Me)
        Else
            scanner.Scan(Files, NewImage, NewFileName)
        End If

        ImageData.StringOffset = ImageCombo.GetPathOffset()

        ImageCombo.Refresh(False, firstImage)

        If firstImage IsNot Nothing Then
            FileDropPostProcess()
        End If

        T.Stop()
        Debug.Print(String.Format(My.Resources.Debug_LoadTimeTaken, T.Elapsed))
        Cursor.Current = Cursors.Default

        Return firstImage
    End Function

    Private Function ProcessFileDropFlux(FilePath As String) As Boolean
        Dim IsFluxIamge As Boolean = False

        If IO.File.Exists(FilePath) Then
            If IO.Path.GetExtension(FilePath).ToLower = ".scp" Then
                If App.AppSettings.Greaseweazle.IsPathValid Then
                    IsFluxIamge = True
                End If
            ElseIf IO.Path.GetExtension(FilePath).ToLower = ".raw" Then
                Dim Response = Flux.GetTrackCountRaw(FilePath)
                If Response.Result Then
                    IsFluxIamge = True
                End If
            End If
        ElseIf IO.Directory.Exists(FilePath) Then
            Dim RawFile = Flux.Greaseweazle.GetFirstRawFile(FilePath)
            If Not String.IsNullOrEmpty(RawFile) Then
                FilePath = RawFile
                IsFluxIamge = True
            End If
        End If

        If IsFluxIamge Then
            FluxConvertImage(FilePath)
        End If

        Return IsFluxIamge
    End Function

    Private Function ProcessFileDropNew(File As String, Optional NewFileName As String = "") As ImageData
        Dim Img = ProcessFileDrop({File}, False, True, NewFileName)

        RefreshModifiedCount()

        Return Img
    End Function

    Private Function ProcessImportedImage(File As String, NewFileName As String) As ImageData
        Return ProcessFileDropNew(File, NewFileName)
    End Function

    Private Sub RefreshCurrentState(FilePanel As FilePanel)
        ImageCombo.RefreshCurrentItemText()
        RefreshDiskState(FilePanel.CurrentImage)
        DiskImageReloadCurrent(FilePanel, False)
    End Sub

    Private Sub RefreshDirectoryMenu(CurrentImage As DiskImageContainer, DirectoryScan As DirectoryScanResponse)
        If CurrentImage Is Nothing OrElse CurrentImage.Disk Is Nothing OrElse DirectoryScan Is Nothing Then
            MenuHexDirectorySubMenuClear()

            MenuToolsWin9xClean.Enabled = False
            MenuToolsClearReservedBytes.Enabled = False
        Else
            MenuHexDirectorySubMenuPopulate(CurrentImage.Disk, DirectoryScan)

            MenuToolsWin9xClean.Enabled = DirectoryScan.HasValidCreated Or DirectoryScan.HasValidLastAccessed Or CurrentImage.Disk.BootSector.IsWin9xOEMName
            MenuToolsClearReservedBytes.Enabled = DirectoryScan.HasNTUnknownFlags Or DirectoryScan.HasFAT32Cluster
        End If
    End Sub

    Private Sub RefreshDiskState(CurrentImage As DiskImageContainer)
        Dim Compare As Integer = 0
        Dim IsValidImage As Boolean = False
        Dim NewInstanceVisible As Boolean = False
        Dim Disk As Disk = Nothing

        If CurrentImage IsNot Nothing Then
            Disk = CurrentImage.Disk
            If Disk IsNot Nothing Then
                IsValidImage = CurrentImage.Disk.IsValidImage
                Compare = CurrentImage.Disk.CheckImageSize
            End If
            NewInstanceVisible = CurrentImage.ImageData.FileType <> ImageData.FileTypeEnum.NewImage AndAlso ImageCombo.Main.Count > 1
        End If

        RefreshHexMenu(Disk, IsValidImage, Compare)
        RefreshToolsMenu(Disk, IsValidImage, Compare)
        RefreshHistoryButtons(Disk)
        RefreshSaveButtons(CurrentImage)
        StatusBarImageInfoUpdate(CurrentImage)

        MainMenuNewInstance.Visible = NewInstanceVisible
    End Sub

    Private Sub RefreshDisplayPath(ImageData As ImageData, UpdateCurrent As Boolean)
        If Not ImageData.FileNameChanged Then
            Exit Sub
        End If

        _LoadedFiles.FileNames.Remove(ImageData.OldDisplayPath)

        If _LoadedFiles.FileNames.ContainsKey(ImageData.DisplayPath) Then
            FileRemove(_LoadedFiles.FileNames.Item(ImageData.DisplayPath))
        End If

        _LoadedFiles.FileNames.Add(ImageData.DisplayPath, ImageData)

        ImageCombo.RefreshPaths()

        If UpdateCurrent Then
            SetCurrentFileName(ImageData)
        End If

        ImageData.OldDisplayPath = ""
        ImageData.FileNameChanged = False
    End Sub

    Private Sub RefreshFilterButtons(Enabled As Boolean)
        MenuFiltersClear.Enabled = Enabled
        If Enabled Then
            MainMenuFilters.BackColor = Color.LightGreen
        Else
            MainMenuFilters.BackColor = SystemColors.Control
        End If
    End Sub

    Private Sub RefreshFluxMenu()
        Dim GreaseweazleEnabled As Boolean = Flux.Greaseweazle.Settings.IsPathValid
        Dim PcImgCnvEnabled As Boolean = Flux.PcImgCnv.Settings.IsPathValid()
        Dim KryofluxEnabled As Boolean = Flux.Kryoflux.Settings.IsPathValid()
        Dim Visible As Boolean = GreaseweazleEnabled Or KryofluxEnabled

        MainMenuFlux.Visible = Visible
        MenuGreaseweazleRead.Enabled = GreaseweazleEnabled
        ToolStripSeparatorDevices.Visible = GreaseweazleEnabled Or PcImgCnvEnabled
        MenuGreaseweazle.Visible = GreaseweazleEnabled
        MenuPcImgCnv.Visible = PcImgCnvEnabled
    End Sub

    Private Sub RefreshHexMenu(Disk As Disk, IsValidImage As Boolean, Compare As Integer)
        RefreshRawTrackSubMenu(Disk)

        If Disk IsNot Nothing AndAlso IsValidImage Then
            MenuHexOverdumpData.Enabled = Compare > 0
            MenuHexFreeClusters.Enabled = Disk.FAT.HasFreeClusters(FAT12.FreeClusterEmum.WithData)
            MenuHexBadSectors.Enabled = Disk.FAT.BadClusters.Count > 0
            MenuHexLostClusters.Enabled = Disk.RootDirectory.FATAllocation.LostClusters.Count > 0
        Else
            MenuHexOverdumpData.Enabled = False
            MenuHexFreeClusters.Enabled = False
            MenuHexBadSectors.Enabled = False
            MenuHexLostClusters.Enabled = False
        End If
    End Sub

    Private Sub RefreshHistoryButtons(Disk As Disk)
        If Disk IsNot Nothing Then
            MenuEditRevert.Enabled = Disk.Image.History.Modified
            SetButtonStateUndo(Disk.Image.History.UndoEnabled)
            SetButtonStateRedo(Disk.Image.History.RedoEnabled)
        Else
            MenuEditRevert.Enabled = False
            SetButtonStateUndo(False)
            SetButtonStateRedo(False)
        End If
    End Sub

    Private Sub RefreshModifiedCount()
        Dim Count = StatusBarModifiedCountUpdate()
        SetButtonStateSaveAll(Count > 0)
    End Sub

    Private Sub RefreshRawTrackSubMenu(Disk As Disk)
        Dim RawTrackDataEnabled As Boolean = False
        Dim RawTrackDataTag As Object = -1

        MenuRawTrackDataSubMenuClear()

        If Disk IsNot Nothing AndAlso Disk.Image.IsBitstreamImage Then
            Dim TrackCount = Disk.Image.NonStandardTracks.Count + Disk.Image.AdditionalTracks.Count
            If TrackCount > 0 Then
                Dim TrackList(TrackCount - 1) As UShort

                Dim i As UShort = 0
                For Each Track In Disk.Image.NonStandardTracks
                    TrackList(i) = Track
                    i += 1
                Next

                For Each Track In Disk.Image.AdditionalTracks
                    TrackList(i) = Track
                    i += 1
                Next

                If TrackList.Length <= 30 Then
                    Array.Sort(TrackList)

                    MenuRawTrackDataSubMenuItemAdd(-1, My.Resources.Label_AllTracks)

                    For i = 0 To TrackList.Length - 1
                        Dim Track = TrackList(i)
                        Dim TrackString = FormatTrackSide(My.Resources.Label_Track, Track \ Disk.Image.SideCount, Track Mod Disk.Image.SideCount)
                        MenuRawTrackDataSubMenuItemAdd(Track, TrackString)
                    Next

                    RawTrackDataTag = Nothing
                End If
            End If

            RawTrackDataEnabled = True
        End If

        MenuHexRawTrackData.Enabled = RawTrackDataEnabled
        MenuHexRawTrackData.Tag = RawTrackDataTag
    End Sub

    Private Sub RefreshSaveButtons(CurrentImage As DiskImageContainer)
        If CurrentImage Is Nothing Then
            SetButtonStateSaveFile(False)
            MenuFileReload.Enabled = False
        Else
            Dim Modified = CurrentImage.ImageData.IsModified
            Dim Disabled = CurrentImage.ImageData.ReadOnly
            SetButtonStateSaveFile(Modified And Not Disabled)
            MenuFileReload.Enabled = True
        End If
    End Sub

    Private Sub RefreshToolsMenu(Disk As Disk, IsValidImage As Boolean, Compare As Integer)
        Dim CanRestructureImage As Boolean = False
        Dim CanResize As Boolean = False
        Dim CanRestoreBootSector As Boolean = False
        Dim HasBootSector As Boolean = False
        Dim TrackLayoutEnabled As Boolean
        Dim FixImageSizeText As String = My.Resources.Menu_TruncateImage

        If Disk IsNot Nothing AndAlso IsValidImage Then
            TrackLayoutEnabled = Disk.Image.IsBitstreamImage

            CanResize = Disk.Image.CanResize

            If CanResize Then
                Dim DiskFormatBySize = FloppyDiskFormatGet(Disk.Image.Length)
                Dim Format = Disk.DiskParams.Format

                If Format = FloppyDiskFormat.Floppy160 And DiskFormatBySize = FloppyDiskFormat.Floppy180 Then
                    CanRestructureImage = True
                ElseIf Format = FloppyDiskFormat.Floppy160 And DiskFormatBySize = FloppyDiskFormat.Floppy320 Then
                    CanRestructureImage = True
                ElseIf Format = FloppyDiskFormat.Floppy160 And DiskFormatBySize = FloppyDiskFormat.Floppy360 Then
                    CanRestructureImage = True
                ElseIf Format = FloppyDiskFormat.Floppy180 And DiskFormatBySize = FloppyDiskFormat.Floppy360 Then
                    CanRestructureImage = True
                ElseIf Format = FloppyDiskFormat.Floppy320 And DiskFormatBySize = FloppyDiskFormat.Floppy360 Then
                    CanRestructureImage = True
                ElseIf Format = FloppyDiskFormat.Floppy720 And DiskFormatBySize = FloppyDiskFormat.Floppy1440 Then
                    CanRestructureImage = True
                End If

                FixImageSizeText = If(Compare < 0, My.Resources.Menu_PadImageSize, My.Resources.Menu_TruncateImage)
            End If

            HasBootSector = Disk.RootDirectory.Data.HasBootSector

            If HasBootSector Then
                Dim BootSectorBytes = Disk.Image.GetBytes(Disk.RootDirectory.Data.BootSectorOffset, BootSector.BOOT_SECTOR_SIZE)
                CanRestoreBootSector = Not BootSectorBytes.CompareTo(Disk.BootSector.Data)
            End If
        End If

        MenuToolsFixImageSize.Visible = Not CanRestructureImage
        MenuToolsFixImageSize.Enabled = CanRestructureImage Or (CanResize AndAlso Compare <> 0)
        MenuToolsFixImageSize.Text = FixImageSizeText

        MenuToolsTruncateImage.Enabled = MenuToolsFixImageSize.Enabled

        MenuToolsFixImageSizeSubMenu.Visible = CanRestructureImage
        MenuToolsFixImageSizeSubMenu.Enabled = CanRestructureImage

        MenuToolsRestructureImage.Enabled = CanRestructureImage
        MenuToolsRestructureImage.Visible = CanRestructureImage

        MenuToolsRemoveBootSector.Enabled = HasBootSector
        MenuToolsRestoreBootSector.Enabled = CanRestoreBootSector

        MenuPcImgCnvTrackLayout.Enabled = TrackLayoutEnabled
    End Sub

    Private Sub ResetAll()
        'EmptyTempImagePath()
        ImageFilters.FiltersApplied = False
        ImageFilters.ScanRun = False
        _LoadedFiles.FileNames.Clear()

        RefreshDiskState(Nothing)

        SummaryClear()

        ImageCombo.ClearAll()
        MainMenuNewInstance.Visible = False

        FilePanelMain.Reset()
        TopMenuFileButtonsRefresh(FilePanelMain.MenuState)
        ToolbarFileButtonsUpdate(FilePanelMain.MenuState)
        StatusBarFileInfoClear()

        SetImagesLoaded(False)
        FiltersReset()
        InitButtonState(Nothing)
    End Sub

    Private Sub SetButtonStateClose(Enabled As Boolean)
        MenuFileClose.Enabled = Enabled
        ToolStripClose.Enabled = Enabled
    End Sub

    Private Sub SetButtonStateRedo(Enabled As Boolean)
        MenuEditRedo.Enabled = Enabled
        ToolStripRedo.Enabled = Enabled
    End Sub

    Private Sub SetButtonStateSaveAll(Enabled As Boolean)
        MenuFileSaveAll.Enabled = Enabled
        ToolStripSaveAll.Enabled = Enabled
    End Sub

    Private Sub SetButtonStateSaveAs(Enabled As Boolean)
        MenuFileSaveAs.Enabled = Enabled
        ToolStripSaveAs.Enabled = Enabled
    End Sub

    Private Sub SetButtonStateSaveFile(Enabled As Boolean)
        MenuFileSave.Enabled = Enabled
        ToolStripSave.Enabled = Enabled
    End Sub

    Private Sub SetButtonStateUndo(Enabled As Boolean)
        MenuEditUndo.Enabled = Enabled
        ToolStripUndo.Enabled = Enabled
    End Sub

    Private Sub SetCurrentFileName(ImageData As ImageData)
        Dim FileName = ImageData.FileName

        Me.Text = $"{GetWindowCaption()} - {FileName}"

        StatusBarFileName.Text = FileName
        StatusBarFileName.Visible = True
    End Sub

    Private Sub SetImagesLoaded(Value As Boolean)
        StatusBarImageCount.Visible = Value
        LabelDropMessage.Visible = Not Value
        MenuFiltersScan.Enabled = Value
        MenuFiltersScanNew.Enabled = Value
        MenuFiltersScanNew.Visible = ImageFilters.ScanRun
        MenuFileCloseAll.Enabled = Value
        ToolStripCloseAll.Enabled = MenuFileCloseAll.Enabled
        _ToolStripSearchText.Enabled = Value
        MenuToolsWin9xCleanBatch.Enabled = Value
    End Sub
    Private Sub StatusBarFileInfoClear()
        StatusBarFileCount.Visible = False
        StatusBarFileSector.Visible = False
        StatusBarFileTrack.Visible = False
    End Sub

    Private Sub StatusBarFileInfoUpdate(FilePanel As FilePanel)
        Dim Text As String
        Dim Total As Integer = FilePanel.Items.Count
        Dim Selected As Integer = FilePanel.SelectedItems.Count

        If Selected > 0 Then
            If Total = 1 Then
                Text = String.Format(My.Resources.Label_SelectedFileCountSingular, Selected, Total)
            Else
                Text = String.Format(My.Resources.Label_SelectedFileCountPlural, Selected, Total)
            End If
        Else
            If Total = 1 Then
                Text = String.Format(My.Resources.Label_FileCountSingular, Total)
            Else
                Text = String.Format(My.Resources.Label_FileCountPlural, Total)
            End If
        End If
        StatusBarFileCount.Text = Text
        StatusBarFileCount.Visible = True

        If Selected = 1 Then
            Dim Disk = FilePanel.CurrentImage.Disk
            Dim FileData As FileData = FilePanel.FirstSelectedItem.Tag

            If FileData IsNot Nothing AndAlso FileData.DirectoryEntry.StartingCluster >= 2 Then

                Dim Sector = Disk.BPB.ClusterToSector(FileData.DirectoryEntry.StartingCluster)
                StatusBarFileSector.Text = My.Resources.Label_Sector & " " & Sector
                StatusBarFileSector.Visible = True

                Dim Track = Disk.BPB.SectorToTrack(Sector)
                Dim Side = Disk.BPB.SectorToSide(Sector)

                StatusBarFileTrack.Text = FormatTrackSide(My.Resources.Label_Track, Track, Side)
                StatusBarFileTrack.Visible = True

                StatusBarFileTrack.GetCurrentParent.Refresh()
            Else
                StatusBarFileSector.Visible = False
                StatusBarFileTrack.Visible = False
            End If
        Else
            StatusBarFileSector.Visible = False
            StatusBarFileTrack.Visible = False
        End If
    End Sub

    Private Sub StatusBarImageCountUpdate()
        Dim Text As String
        Dim Total As Integer = ImageCombo.Main.Count

        If ImageFilters.FiltersApplied Then
            Dim Filtered As Integer = ImageCombo.Filtered.Count

            If Total = 1 Then
                Text = String.Format(My.Resources.Label_FilteredImageCountSingular, Filtered, Total)
            Else
                Text = String.Format(My.Resources.Label_FilteredImageCountPlural, Filtered, Total)
            End If
        Else
            If Total = 1 Then
                Text = String.Format(My.Resources.Label_ImageCountSingular, Total)
            Else
                Text = String.Format(My.Resources.Label_ImageCountPlural, Total)
            End If
        End If

        StatusBarImageCount.Text = Text
    End Sub
    Private Sub StatusBarImageInfoUpdate(CurrentImage As DiskImageContainer)
        If CurrentImage IsNot Nothing AndAlso CurrentImage.Disk IsNot Nothing Then
            Dim StatusText = ""
            If CurrentImage.ImageData.FileType = ImageData.FileTypeEnum.Compressed Then
                StatusText = My.Resources.Label_Compressed
            ElseIf CurrentImage.ImageData.FileType = ImageData.FileTypeEnum.NewImage Then
                StatusText = My.Resources.Label_New
            ElseIf CurrentImage.ImageData.ReadOnly Then
                StatusText = My.Resources.Label_ReadOnly
            End If

            StatusBarStatus.Visible = StatusText <> ""
            StatusBarStatus.Text = StatusText

            StatusBarModified.Visible = CurrentImage.Disk.Image.History.Modified
        Else
            StatusBarStatus.Visible = False
            StatusBarModified.Visible = False
        End If
    End Sub

    Private Function StatusBarModifiedCountUpdate() As Integer
        Dim Count As Integer = ImageFilters.FilterCounts(Filters.FilterTypes.ModifiedFiles).Total

        If Count = 1 Then
            StatusBarImagesModified.Text = String.Format(My.Resources.Label_ModifiedImageCountSingular, Count)
        Else
            StatusBarImagesModified.Text = String.Format(My.Resources.Label_ModifiedImageCountPlural, Count)
        End If

        StatusBarImagesModified.Visible = (Count > 0)

        Return Count
    End Function

    Private Sub SubFiltersClearFilter()
        ImageFilters.SubFiltersClearFilter()
    End Sub

    Private Sub SubFiltersReset()
        ImageFilters.SubFiltersClear(True)

        SubFiltersToggle(False)
    End Sub

    Private Sub SubFiltersToggle(Value As Boolean)
        _ToolStripOEMNameCombo.Visible = Value
        _ToolStripOEMNameLabel.Visible = Value

        _ToolStripDiskTypeCombo.Visible = Value
        _ToolStripDiskTypeLabel.Visible = Value
    End Sub

    Private Sub SummaryClear()
        btnRetry.Visible = False
        ClearCurentFileName()
        _SummaryPanel.Clear()
        HashPanelPopulate(Nothing, "")
    End Sub

    Private Sub SummaryPopulate(CurrentImage As DiskImageContainer)
        Dim MD5 As String = ""

        If CurrentImage IsNot Nothing Then
            If CurrentImage.Disk IsNot Nothing Then
                MD5 = CurrentImage.Disk.Image.GetMD5Hash
                btnRetry.Visible = False
            Else
                btnRetry.Visible = Not CurrentImage.ImageData.InvalidImage
            End If
            SetCurrentFileName(CurrentImage.ImageData)
            _SummaryPanel.Populate(CurrentImage, App.Globals.BootstrapDB, App.Globals.TitleDB, MD5)
            HashPanelPopulate(CurrentImage.Disk, MD5)
        Else
            SummaryClear()
        End If
    End Sub
    Private Sub ToolbarFileButtonsUpdate(MenuState As FileMenuState)
        SetMenuItemStateEnabled(ToolStripFileProperties, MenuState.FilePropertiesEnabled)

        SetMenuItemState(ToolStripExportFile, MenuState.ExportFile)

        SetMenuItemStateEnabled(ToolStripImportFiles, MenuState.AddFileEnabled, MenuState.RootDirectory)

        ToolStripViewFileText.Enabled = MenuState.ViewFileText.Enabled
        ToolStripViewFileText.Text = MenuState.ViewFileText.Caption

        SetMenuItemState(ToolStripViewFile, MenuState.ViewFile)
    End Sub

    Private Sub TopMenuFileButtonsRefresh(MenuState As FileMenuState)
        SetMenuItemStateEnabled(MenuEditFileProperties, MenuState.FilePropertiesEnabled)

        SetMenuItemState(MenuEditExportFile, MenuState.ExportFile)

        SetMenuItemStateEnabled(MenuEditReplaceFile, MenuState.ReplaceFileEnabled)

        SetMenuItemStateEnabled(MenuEditImportFiles, MenuState.AddFileEnabled, MenuState.RootDirectory)

        SetMenuItemState(MenuHexFile, MenuState.ViewHexFile, MenuState.DirectoryEntry)
        MenuHexSeparatorFile.Visible = MenuState.ViewHexFile.Visible
    End Sub
#Region "Events"
    Private Sub BMenuHelpUpdateCheck_Click(sender As Object, e As EventArgs) Handles MenuHelpUpdateCheck.Click, MainMenuUpdateAvailable.Click
        Dim Result = CheckForUpdates()
        If Result Then
            MainMenuUpdateAvailable.Visible = False
        End If
    End Sub

    Private Sub BtnLanguage_Click(sender As Object, e As EventArgs)
        Dim ClickedItem = CType(sender, ToolStripMenuItem)

        If ClickedItem.Checked Then
            Exit Sub
        End If

        For Each item As ToolStripItem In ClickedItem.GetCurrentParent().Items
            If TypeOf item Is ToolStripMenuItem Then
                CType(item, ToolStripMenuItem).Checked = False
            End If
        Next

        ClickedItem.Checked = True

        App.Globals.AppSettings.Language = ClickedItem.Tag.ToString()

        MsgBox(My.Resources.Dialog_LanguageSettings, MsgBoxStyle.Information)
    End Sub

    Private Sub BtnResetSort_Click(sender As Object, e As EventArgs) Handles BtnResetSort.Click
        FilePanelMain.ClearSort(True)
    End Sub

    Private Sub BtnRetry_Click(sender As Object, e As EventArgs) Handles btnRetry.Click
        DiskImageReloadCurrent(FilePanelMain, False)
    End Sub

    Private Sub ContextMenuFilters_Closing(sender As Object, e As ToolStripDropDownClosingEventArgs) Handles ContextMenuFilters.Closing
        If e.CloseReason = ToolStripDropDownCloseReason.ItemClicked Then
            e.Cancel = True
        End If
    End Sub

    Private Sub ContextMenuOptions_Closing(sender As Object, e As ToolStripDropDownClosingEventArgs)
        If e.CloseReason = ToolStripDropDownCloseReason.ItemClicked Then
            e.Cancel = True
        End If
    End Sub

    Private Sub File_DragDrop(sender As Object, e As DragEventArgs) Handles ComboImages.DragDrop, LabelDropMessage.DragDrop, ListViewFiles.DragDrop, ListViewSummary.DragDrop, FlowLayoutPanelHashes.DragDrop
        Dim Files As String() = e.Data.GetData(DataFormats.FileDrop)

        Me.BeginInvoke(Sub()
                           Me.Cursor = Cursors.Default
                           HandleDragDrop(Files)
                       End Sub)
    End Sub

    Private Sub File_DragEnter(sender As Object, e As DragEventArgs) Handles ComboImages.DragEnter, LabelDropMessage.DragEnter, ListViewFiles.DragEnter, ListViewSummary.DragEnter, FlowLayoutPanelHashes.DragEnter
        If _Suppress_File_DragEnterEvent Then
            Exit Sub
        End If

        Debug.Print("MainForm.File_DragEnter fired")

        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub FilePanel_CurrentImageChangeEnd(sender As Object, e As Boolean) Handles FilePanelMain.CurrentImageChangeEnd
        RefreshDirectoryMenu(FilePanelMain.CurrentImage, FilePanelMain.DirectoryScan)
        SummaryPopulate(FilePanelMain.CurrentImage)
        StatusStripBottom.Refresh()
        If e Then
            ImageFiltersUpdate(FilePanelMain.CurrentImage)
        End If
        RefreshDiskState(FilePanelMain.CurrentImage)
    End Sub

    Private Sub FilePanel_CurrentImageChangeStart(sender As Object, e As Boolean) Handles FilePanelMain.CurrentImageChangeStart
        InitButtonState(FilePanelMain.CurrentImage)
    End Sub

    Private Sub FilePanel_ItemDoubleClick(sender As Object, e As ListViewItem) Handles FilePanelMain.ItemDoubleClick
        Dim FilePanel = DirectCast(sender, FilePanel)
        Dim FileData = TryCast(e.Tag, FileData)

        If FileData IsNot Nothing Then
            DirectoryEntryDisplay(FilePanel, FileData)
        End If
    End Sub

    Private Sub FilePanel_ItemDrag(sender As Object, e As ItemDragEventArgs) Handles FilePanelMain.ItemDrag
        Dim FilePanel = DirectCast(sender, FilePanel)

        _Suppress_File_DragEnterEvent = True
        DragDropExportSelectedFiles(FilePanel)
        _Suppress_File_DragEnterEvent = False
    End Sub

    Private Sub FilePanel_ItemSelectionChanged(sender As Object, e As EventArgs) Handles FilePanelMain.ItemSelectionChanged
        Dim FilePanel = DirectCast(sender, FilePanel)

        TopMenuFileButtonsRefresh(FilePanel.MenuState)
        ToolbarFileButtonsUpdate(FilePanel.MenuState)
        StatusBarFileInfoUpdate(FilePanel)
    End Sub

    Private Sub FilePanel_MenuItemClicked(sender As Object, e As MenuItemClickedEventArgs) Handles FilePanelMain.MenuItemClicked
        Dim FilePanel = DirectCast(sender, FilePanel)

        FilePanelProcessEvent(FilePanel, e.MenuItem, e.Directory)
    End Sub

    Private Sub FilePanel_SortChanged(sender As Object, e As Boolean) Handles FilePanelMain.SortChanged
        BtnResetSort.Enabled = e
    End Sub

    Private Sub HashPanelContextMenu_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles HashPanelContextMenu.ItemClicked
        Dim Label As Label = CType(sender.SourceControl, Label)
        Dim Value As String

        If Label Is LabelCRC32 Or Label Is LabelCRC32Caption Then
            Value = LabelCRC32.Text
        ElseIf Label Is LabelMD5 Or Label Is LabelMD5Caption Then
            Value = LabelMD5.Text
        ElseIf Label Is LabelSHA1 Or Label Is LabelSHA1Caption Then
            Value = LabelSHA1.Text
        Else
            Exit Sub
        End If

        Clipboard.SetText(Value)
    End Sub

    Private Sub HashPanelContextMenu_Opening(sender As Object, e As CancelEventArgs) Handles HashPanelContextMenu.Opening
        Dim CM As ContextMenuStrip = sender
        Dim Label As Label = CType(sender.SourceControl, Label)
        Dim Text As String

        If Label Is LabelCRC32 Or Label Is LabelCRC32Caption Then
            Text = LabelCRC32Caption.Text
        ElseIf Label Is LabelMD5 Or Label Is LabelMD5Caption Then
            Text = LabelMD5Caption.Text
        ElseIf Label Is LabelSHA1 Or Label Is LabelSHA1Caption Then
            Text = LabelSHA1Caption.Text
        Else
            e.Cancel = True
            Exit Sub
        End If

        CM.Items(0).Text = String.Format(My.Resources.Menu_CopyValueByName, Text)
    End Sub

    Private Sub ImageFilters_FilterChanged(ResetSubFilters As Boolean) Handles ImageFilters.FilterChanged
        FiltersApply(ResetSubFilters)
    End Sub

    Private Sub ImageList_SelectedImageChanged() Handles ImageCombo.SelectedImageChanged
        DiskImageLoadIntoFilePanel(FilePanelMain, ImageCombo.SelectedImage, False)
    End Sub

    Private Sub MainForm_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        EmptyTempImagePath()
        App.Globals.AppSettings.Save()
        App.Globals.UserState.Save()
    End Sub

    Private Sub MainForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If Not DiskImageCloseAll(FilePanelMain.CurrentImage) Then
            RefreshModifiedCount()
            e.Cancel = True
        End If
    End Sub

    Private Sub MainForm_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        ImageFilters.Dispose()
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        MainMenuNewInstance.ToolTipText = MainMenuNewInstance.Text
        _FileVersion = GetVersionString()

        PositionControls()

        InitAllFileExtensions()

        PositionForm()

        AddHandler MainMenuOptions.DropDown.Closing, AddressOf ContextMenuOptions_Closing

        _LoadedFiles = New LoadedFiles

        HashPanelInitContextMenu()

        ImageFilters = New Filters.ImageFilters(ContextMenuFilters, _ToolStripOEMNameCombo, _ToolStripDiskTypeCombo, _ToolStripSearchText, App.Globals.BootstrapDB, App.Globals.TitleDB)
        _SummaryPanel = New SummaryPanel(ListViewSummary)
        ImageCombo = New LoadedImageList(ComboImages)
        FilePanelMain = New FilePanel(ListViewFiles, False)

        DetectFloppyDrives()
        InitOptionsMenu()
        InitDebugFeatures(App.Globals.AppSettings.Debug)
        RefreshFluxMenu()
        ResetAll()

        ProcessCommandLineArgs()

        InitUpdateCheck()
    End Sub

    Private Sub MainForm_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        App.Globals.AppSettings.WindowWidth = Me.Width
        App.Globals.AppSettings.WindowHeight = Me.Height
    End Sub

    Private Sub MainMenuNewInstance_Click(sender As Object, e As EventArgs) Handles MainMenuNewInstance.Click
        OpenImageInNewInstance(FilePanelMain.CurrentImage)
    End Sub

    Private Sub MenuDiskReadFloppyA_Click(sender As Object, e As EventArgs) Handles MenuDiskReadFloppyA.Click
        Dim FileName = FloppyDiskRead(Me, FloppyDriveEnum.FloppyDriveA, _LoadedFiles.FileNames)
        If FileName.Length > 0 Then
            ProcessFileDrop(FileName)
        End If
    End Sub

    Private Sub MenuDiskReadFloppyB_Click(sender As Object, e As EventArgs) Handles MenuDiskReadFloppyB.Click
        Dim FileName = FloppyDiskRead(Me, FloppyDriveEnum.FloppyDriveB, _LoadedFiles.FileNames)
        If FileName.Length > 0 Then
            ProcessFileDrop(FileName)
        End If
    End Sub

    Private Sub MenuDiskWriteFloppyA_Click(sender As Object, e As EventArgs) Handles MenuDiskWriteFloppyA.Click
        FloppyDiskWrite(Me, FilePanelMain.CurrentImage.Disk, FloppyDriveEnum.FloppyDriveA)
    End Sub

    Private Sub MenuDiskWriteFloppyB_Click(sender As Object, e As EventArgs) Handles MenuDiskWriteFloppyB.Click
        FloppyDiskWrite(Me, FilePanelMain.CurrentImage.Disk, FloppyDriveEnum.FloppyDriveB)
    End Sub

    Private Sub MenuEditBootSector_Click(sender As Object, e As EventArgs) Handles MenuEditBootSector.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.BootSectorEdit)
    End Sub

    Private Sub MenuEditExportFile_Click(sender As Object, e As EventArgs) Handles MenuEditExportFile.Click, ToolStripExportFile.Click
        ImageFileExport(FilePanelMain)
    End Sub

    Private Sub MenuEditFAT_Click(sender As Object, e As EventArgs) Handles MenuEditFAT.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.EditFAT, sender.tag)
    End Sub

    Private Sub MenuEditFileProperties_Click(sender As Object, e As EventArgs) Handles MenuEditFileProperties.Click, ToolStripFileProperties.Click
        FilePanelProcessEvent(FilePanelMain, FilePanel.FilePanelMenuItem.FileProperties)
    End Sub

    Private Sub MenuEditImportFiles_Click(sender As Object, e As EventArgs) Handles ToolStripImportFiles.Click, MenuEditImportFiles.Click
        Dim Directory = TryCast(sender.Tag, IDirectory)

        FilePanelProcessEvent(FilePanelMain, FilePanel.FilePanelMenuItem.ImportFiles, Directory)
    End Sub

    Private Sub MenuEditRedo_Click(sender As Object, e As EventArgs) Handles MenuEditRedo.Click, ToolStripRedo.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.ImageUndo)
    End Sub

    Private Sub MenuEditReplaceFile_Click(sender As Object, e As EventArgs) Handles MenuEditReplaceFile.Click
        FilePanelProcessEvent(FilePanelMain, FilePanel.FilePanelMenuItem.ReplaceFile)
    End Sub

    Private Sub MenuEditRevert_Click(sender As Object, e As EventArgs) Handles MenuEditRevert.Click
        DiskImageRevertChanges(FilePanelMain)
    End Sub

    Private Sub MenuEditUndo_Click(sender As Object, e As EventArgs) Handles MenuEditUndo.Click, ToolStripUndo.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.ImageRedo)
    End Sub

    Private Sub MenuFileClose_Click(sender As Object, e As EventArgs) Handles MenuFileClose.Click, ToolStripClose.Click
        FileCloseCurrent(FilePanelMain.CurrentImage)
    End Sub

    Private Sub MenuFileCloseAll_Click(sender As Object, e As EventArgs) Handles MenuFileCloseAll.Click, ToolStripCloseAll.Click
        If MsgBox(My.Resources.Dialog_CloseAll, MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
            If Not DiskImageCloseAll(FilePanelMain.CurrentImage) Then
                RefreshModifiedCount()
            End If
        End If
    End Sub

    Private Sub MenuFileExit_Click(sender As Object, e As EventArgs) Handles MenuFileExit.Click
        If DiskImageCloseAll(FilePanelMain.CurrentImage) Then
            Me.Close()
        Else
            RefreshModifiedCount()
        End If
    End Sub

    Private Sub MenuFileNewImage_Click(sender As Object, e As EventArgs) Handles MenuFileNewImage.Click
        ImageNew()
    End Sub

    Private Sub MenuFileOpen_Click(sender As Object, e As EventArgs) Handles MenuFileOpen.Click, ToolStripOpen.Click
        FilesOpen()
    End Sub

    Private Sub MenuFileReload_Click(sender As Object, e As EventArgs) Handles MenuFileReload.Click
        DiskImageReloadCurrent(FilePanelMain, False)
    End Sub

    Private Sub MenuFileSave_Click(sender As Object, e As EventArgs) Handles MenuFileSave.Click, ToolStripSave.Click
        Dim NewFileName = FilePanelMain.CurrentImage.ImageData.FileType = ImageData.FileTypeEnum.NewImage
        DiskImageSaveCurrent(FilePanelMain, NewFileName)
    End Sub

    Private Sub MenuFileSaveAll_Click(sender As Object, e As EventArgs) Handles MenuFileSaveAll.Click, ToolStripSaveAll.Click
        If MsgBox(My.Resources.Dialog_SaveAll, MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
            DiskImageSaveAll(FilePanelMain)
        End If
    End Sub

    Private Sub MenuFileSaveAs_Click(sender As Object, e As EventArgs) Handles MenuFileSaveAs.Click, ToolStripSaveAs.Click
        DiskImageSaveCurrent(FilePanelMain, True)
    End Sub

    Private Sub MenuFiltersClear_Click(sender As Object, e As EventArgs) Handles MenuFiltersClear.Click
        ClearFiltersIfApplied(False, True)
    End Sub

    Private Sub MenuFiltersScan_Click(sender As Object, e As EventArgs) Handles MenuFiltersScan.Click
        ContextMenuFilters.Close()
        DiskImagesScan(FilePanelMain.CurrentImage, False)
    End Sub

    Private Sub MenuFiltersScanNew_Click(sender As Object, e As EventArgs) Handles MenuFiltersScanNew.Click
        ContextMenuFilters.Close()
        DiskImagesScan(FilePanelMain.CurrentImage, True)
    End Sub

    Private Sub MenuFluxConvert_Click(sender As Object, e As EventArgs) Handles MenuFluxConvert.Click
        FluxConvertImage()
    End Sub

    Private Sub MenuGreaseweazleBandwidth_Click(sender As Object, e As EventArgs) Handles MenuGreaseweazleBandwidth.Click
        Flux.Greaseweazle.BandwidthDisplay(Me)
    End Sub

    Private Sub MenuGreaseweazleClean_Click(sender As Object, e As EventArgs) Handles MenuGreaseweazleClean.Click
        Flux.Greaseweazle.CleanDiskForm.Display(Me)
    End Sub

    Private Sub MenuGreaseweazleErase_Click(sender As Object, e As EventArgs) Handles MenuGreaseweazleErase.Click
        Flux.Greaseweazle.EraseDiskForm.Display(Me)
    End Sub

    Private Sub MenuGreaseweazleInfo_Click(sender As Object, e As EventArgs) Handles MenuGreaseweazleInfo.Click
        Flux.Greaseweazle.InfoDisplay(Me)
    End Sub

    Private Sub MenuGreaseweazleRead_Click(sender As Object, e As EventArgs) Handles MenuGreaseweazleRead.Click
        GreaseweazleReadImage()
    End Sub

    Private Sub MenuGreaseweazleWrite_Click(sender As Object, e As EventArgs) Handles MenuGreaseweazleWrite.Click
        If FilePanelMain.CurrentImage IsNot Nothing AndAlso FilePanelMain.CurrentImage.Disk IsNot Nothing Then
            Flux.Greaseweazle.WriteDiskForm.Display(FilePanelMain.CurrentImage.Disk, FilePanelMain.CurrentImage.ImageData.FileName, Me)
        End If
    End Sub

    Private Sub MenuHelpAbout_Click(sender As Object, e As EventArgs) Handles MenuHelpAbout.Click
        AboutBox.Display()
    End Sub

    Private Sub MenuHelpChangeLog_Click(sender As Object, e As EventArgs) Handles MenuHelpChangeLog.Click
        ChangeLogDisplay()
    End Sub

    Private Sub MenuHelpProjectPage_Click(sender As Object, e As EventArgs) Handles MenuHelpProjectPage.Click
        ProjectPageDisplay()
    End Sub

    Private Sub MenuHexBadSectors_Click(sender As Object, e As EventArgs) Handles MenuHexBadSectors.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.HexDisplayBadSectors)
    End Sub

    Private Sub MenuHexBootSector_Click(sender As Object, e As EventArgs) Handles MenuHexBootSector.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.HexDisplayBootSector)
    End Sub

    Private Sub MenuHexDirectory_Click(sender As Object, e As EventArgs) Handles MenuHexDirectory.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.HexDisplayDirectory, sender.tag)
    End Sub

    Private Sub MenuHexDisk_Click(sender As Object, e As EventArgs) Handles MenuHexDisk.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.HexDisplayDisk)
    End Sub

    Private Sub MenuHexFAT_Click(sender As Object, e As EventArgs) Handles MenuHexFAT.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.HexDisplayFAT)
    End Sub

    Private Sub MenuHexFile_Click(sender As Object, e As EventArgs) Handles MenuHexFile.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.HexDisplayDirectoryEntry, sender.tag)
    End Sub

    Private Sub MenuHexFreeClusters_Click(sender As Object, e As EventArgs) Handles MenuHexFreeClusters.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.HexDisplayFreeClusters)
    End Sub

    Private Sub MenuHexLostClusters_Click(sender As Object, e As EventArgs) Handles MenuHexLostClusters.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.HexDisplayLostSectors)
    End Sub

    Private Sub MenuHexOverdumpData_Click(sender As Object, e As EventArgs) Handles MenuHexOverdumpData.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.HexDisplayOverdumpData)
    End Sub

    Private Sub MenuHexRawTrackData_Click(sender As Object, e As EventArgs) Handles MenuHexRawTrackData.Click
        If sender.tag IsNot Nothing Then
            HexDisplayRawTrackData(FilePanelMain.CurrentImage.Disk, CInt(sender.Tag))
        End If
    End Sub

    Private Sub MenuOptionsCheckUpdate_CheckStateChanged(sender As Object, e As EventArgs) Handles MenuOptionsCheckUpdate.CheckStateChanged
        App.Globals.AppSettings.CheckUpdateOnStartup = MenuOptionsCheckUpdate.Checked
    End Sub

    Private Sub MenuOptionsCreateBackup_CheckStateChanged(sender As Object, e As EventArgs) Handles MenuOptionsCreateBackup.CheckStateChanged
        App.Globals.AppSettings.CreateBackups = MenuOptionsCreateBackup.Checked
    End Sub

    Private Sub MenuOptionsDisplayTitles_CheckStateChanged(sender As Object, e As EventArgs) Handles MenuOptionsDisplayTitles.CheckStateChanged
        App.Globals.AppSettings.DisplayTitles = MenuOptionsDisplayTitles.Checked

        If FilePanelMain.CurrentImage IsNot Nothing Then
            SummaryPopulate(FilePanelMain.CurrentImage)
            RefreshDiskState(FilePanelMain.CurrentImage)
        End If
    End Sub

    Private Sub MenuOptionsDragDrop_CheckStateChanged(sender As Object, e As EventArgs) Handles MenuOptionsDragDrop.CheckStateChanged
        App.Globals.AppSettings.DragAndDrop = MenuOptionsDragDrop.Checked
    End Sub

    Private Sub MenuOptionsEnableWriteSpliceFilter_CheckStateChanged(sender As Object, e As EventArgs)
        ImageFilters.EnableWriteSpliceFilter = DirectCast(sender, ToolStripMenuItem).Checked
    End Sub

    Private Sub MenuOptionsExportUnknown_CheckStateChanged(sender As Object, e As EventArgs)
        ImageFilters.ExportUnknownImages = DirectCast(sender, ToolStripMenuItem).Checked
    End Sub

    Private Sub MenuOptionsGreaseweazle_Click(sender As Object, e As EventArgs) Handles MenuOptionsFlux.Click
        MainMenuOptions.DropDown.Close()

        Flux.ConfigurationForm.Display(Me)

        RefreshFluxMenu()
    End Sub

    Private Sub MenuPcImgCnvTrackLayout_Click(sender As Object, e As EventArgs) Handles MenuPcImgCnvTrackLayout.Click
        If FilePanelMain.CurrentImage Is Nothing Then
            Exit Sub
        End If

        GenerateTrackLayout(FilePanelMain.CurrentImage.Disk, FilePanelMain.CurrentImage.ImageData.FileName)
    End Sub

    Private Sub MenuReportsModifications_Click(sender As Object, e As EventArgs) Handles MenuReportsModifications.Click
        DisplayReportModifications(FilePanelMain.CurrentImage.Disk, FilePanelMain.CurrentImage.ImageData.FileName)
    End Sub

    Private Sub MenuToolsClearReservedBytes_Click(sender As Object, e As EventArgs) Handles MenuToolsClearReservedBytes.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.ImageClearReservedBytes)
    End Sub

    Private Sub MenuToolsFixImageSize_Click(sender As Object, e As EventArgs) Handles MenuToolsFixImageSize.Click, MenuToolsTruncateImage.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.ImageFixImageSize)
    End Sub

    Private Sub MenuToolsRemoveBootSector_Click(sender As Object, e As EventArgs) Handles MenuToolsRemoveBootSector.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.BootSectorRemoveFromDirectory)
    End Sub

    Private Sub MenuToolsRestoreBootSector_Click(sender As Object, e As EventArgs) Handles MenuToolsRestoreBootSector.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.BootSectorRestoreFromDirectory)
    End Sub

    Private Sub MenuToolsRestructureImage_Click(sender As Object, e As EventArgs) Handles MenuToolsRestructureImage.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.ImageRestructure)
    End Sub

    Private Sub MenuToolsWin9xClean_Click(sender As Object, e As EventArgs) Handles MenuToolsWin9xClean.Click
        DiskImageProcessEvent(FilePanelMain, DiskImageMenuItem.RemoveWindowsModifications)
    End Sub

    Private Sub MenuToolsWin9xCleanBatch_Click(sender As Object, e As EventArgs) Handles MenuToolsWin9xCleanBatch.Click
        ImageRemoveWindowsModificationsBatch(FilePanelMain)
    End Sub

    Private Sub ProcessCommandLineArgs()
        Dim Args = Environment.GetCommandLineArgs.Skip(1).ToArray

        If Args.Length > 0 Then
            Dim ShowDialog As Boolean = False
            If Args.Length > 1 Then
                ShowDialog = True
            Else
                If IO.Directory.Exists(Args(0)) Then
                    ShowDialog = True
                End If
            End If
            ProcessFileDrop(Args, ShowDialog)
        End If
    End Sub
    Private Sub ToolStripFATCombo_SelectedIndexChanged(sender As Object, e As EventArgs)
        If _Suppress_ToolStripFATCombo_SelectedIndexChangedEvent Then
            Exit Sub
        End If

        Debug.Print("MainForm.ToolStripFATCombo_SelectedIndexChanged fired")

        If FilePanelMain.CurrentImage IsNot Nothing Then
            FilePanelMain.CurrentImage.ImageData.FATIndex = _ToolStripFatCombo.SelectedIndex
            DiskImageReloadCurrent(FilePanelMain, False)
        End If
    End Sub

    Private Sub ToolStripViewFile_Click(sender As Object, e As EventArgs) Handles ToolStripViewFile.Click
        FilePanelProcessEvent(FilePanelMain, FilePanel.FilePanelMenuItem.ViewFile)
    End Sub

    Private Sub ToolStripViewFileText_Click(sender As Object, e As EventArgs) Handles ToolStripViewFileText.Click
        FilePanelProcessEvent(FilePanelMain, FilePanel.FilePanelMenuItem.ViewFileText)
    End Sub
#End Region
End Class