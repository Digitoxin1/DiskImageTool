Imports System.ComponentModel
Imports System.Text

Public Class MainForm
    Private Const FILE_FILTER As String = "Disk Image Files (*.ima; *.img)|*.ima;*.img"
    Private ReadOnly _LoadedImageList As List(Of LoadedImageData)
    Private ReadOnly _LoadedFileNames As Dictionary(Of String, LoadedImageData)
    Private ReadOnly _OEMIDDictionary As Dictionary(Of UInteger, OEMIDList)
    Private _Disk As DiskImage.Disk
    Private _SuppressEvent As Boolean = False
    Private _FiltersApplied As Boolean = False
    Private _FilterCounts() As Integer
    Private _CheckAll As Boolean = False
    Private _ScanRun As Boolean = False

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ListViewDoubleBuffer(ListViewFiles)
        FiltersInitialize()
        _LoadedImageList = New List(Of LoadedImageData)
        _LoadedFileNames = New Dictionary(Of String, LoadedImageData)
        _OEMIDDictionary = GetOEMIDDictionary()

        ResetAll()
    End Sub

    Private Sub BootSectorDisplayHex()
        Dim Block = DiskImage.Disk.NewDataBlock(0, 512)

        Dim frmHexView As New HexViewForm(_Disk, Block, False, "Boot Sector")
        frmHexView.ShowDialog()
    End Sub

    Private Sub ResetAll()
        _Disk = Nothing
        _FiltersApplied = False
        _CheckAll = False
        _LoadedImageList.Clear()
        _LoadedFileNames.Clear()
        _ScanRun = False
        LabelDropMessage.Visible = True
        BtnOEMID.Enabled = False
        BtnDisplayBootSector.Enabled = False
        BtnDisplayDirectory.Enabled = False
        BtnDisplayClusters.Enabled = False
        BtnFileProperties.Enabled = False
        BtnReplaceFile.Enabled = False
        BtnScan.Enabled = False
        BtnScanNew.Enabled = False
        BtnScanNew.Visible = False
        BtnRevert.Enabled = False
        BtnDisplayFile.Tag = Nothing
        BtnDisplayFile.Visible = False
        ToolStripImageCount.Visible = False
        ToolStripFileName.Visible = False
        ToolStripModified.Visible = False
        BtnSaveAll.Enabled = False
        BtnClose.Enabled = False
        BtnCloseAll.Enabled = False
        BtnExportDebug.Enabled = False
        ComboGroups.Enabled = False
        ListViewSummary.Items.Clear()
        ListViewHashes.Items.Clear()
        ListViewFiles.Items.Clear()
        ListViewFiles.Groups.Clear()
        ListViewFiles.MultiSelect = False
        ListViewFiles.Refresh()
        ComboGroups.Items.Clear()
        MainMenuFilters.BackColor = SystemColors.Control
        FiltersReset()
        SaveButtonRefresh(False)
    End Sub

    Private Function CloseAll() As Boolean
        Dim ModifyImageList As New List(Of LoadedImageData)
        Dim Result As MsgBoxResult = MsgBoxResult.No

        For Each ImageData In _LoadedImageList
            If ImageData.Modified Then
                ModifyImageList.Add(ImageData)
            End If
        Next

        If ModifyImageList.Count > 0 Then
            Dim ShowDialog As Boolean = True
            Dim SaveAllForm As SaveAllForm

            For Each ImageData In ModifyImageList
                Dim Msg As String = "Save file '" & IO.Path.GetFileName(ImageData.FilePath) & "'?"
                If ShowDialog Then
                    If ModifyImageList.Count = 1 Then
                        Result = MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNoCancel + MsgBoxStyle.DefaultButton3, "Save")
                    Else
                        SaveAllForm = New SaveAllForm(Msg)
                        SaveAllForm.ShowDialog()
                        Result = SaveAllForm.Result
                        SaveAllForm = Nothing
                    End If
                Else
                    Result = vbYes
                End If
                If Result = MsgBoxResult.Yes Or Result = MsgBoxResult.Retry Then
                    If Result = MsgBoxResult.Retry Then
                        ShowDialog = False
                    End If
                    DiskImageSave(ImageData)
                ElseIf Result = MsgBoxResult.Ignore Or Result = MsgBoxResult.Cancel Then
                    Exit For
                End If
            Next
        End If

        If Result <> MsgBoxResult.Cancel Then
            ResetAll()
        End If
        Return (Result <> MsgBoxResult.Cancel)
    End Function

    Private Sub CloseCurrent()
        Dim ImageData As LoadedImageData = ComboGroups.SelectedItem
        Dim Result As MsgBoxResult

        If ImageData.Modified Then
            Dim Msg As String = "Save file '" & IO.Path.GetFileName(ImageData.FilePath) & "'?"
            Result = MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNoCancel + MsgBoxStyle.DefaultButton3, "Save")
        Else
            Result = MsgBoxResult.No
        End If

        If Result <> MsgBoxResult.Cancel Then
            If Result = MsgBoxResult.Yes Then
                DiskImageSave(ImageData)
            End If
            FileClose(ImageData)
        End If
    End Sub

    Private Sub ComboGroupRefreshItemText()
        _SuppressEvent = True
        For Counter = 0 To ComboGroups.Items.Count - 1
            ComboGroups.Items(Counter) = ComboGroups.Items(Counter)
        Next
        _SuppressEvent = False
    End Sub

    Private Sub ComboItemRefresh(FullRefresh As Boolean)
        Dim ImageData As LoadedImageData = ComboGroups.SelectedItem

        _SuppressEvent = True
        ComboGroups.Items(ComboGroups.SelectedIndex) = ImageData
        _SuppressEvent = False

        PopulateSummary()
        If FullRefresh Then
            PopulateDirectory()
        End If

        SaveButtonRefresh(ImageData.Modified)
        BtnExportDebug.Enabled = (ImageData.Modified Or ImageData.SessionModifications.Count > 0)
    End Sub

    Private Sub DirectoryEntryDisplayText(Offset As UInteger)
        Dim frmTextView As TextViewForm

        Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(Offset)

        If DirectoryEntry.IsVolumeName _
            Or DirectoryEntry.HasInvalidFileSize _
            Or DirectoryEntry.StartingCluster < 2 _
            Or DirectoryEntry.IsDirectory _
            Or DirectoryEntry.IsDeleted Then
            Exit Sub
        End If

        Dim Caption As String = "File - " & DirectoryEntry.GetFullFileName
        Dim Bytes = DirectoryEntry.GetContent
        Dim Content As String

        Using Stream As New IO.MemoryStream
            Dim PrevByte As Byte = 0
            For Counter = 0 To Bytes.Length - 1
                Dim B = Bytes(Counter)
                If B = 0 Then
                    Stream.WriteByte(32)
                ElseIf Counter > 0 And B = 10 And PrevByte <> 13 Then
                    Stream.WriteByte(13)
                    Stream.WriteByte(10)
                Else
                    Stream.WriteByte(B)
                End If
                PrevByte = B
            Next
            Content = Encoding.UTF7.GetString(Stream.GetBuffer)
        End Using

        frmTextView = New TextViewForm(Caption, Content)
        frmTextView.ShowDialog()
    End Sub

    Private Sub DirectoryEntryDisplayHex(Offset As UInteger)
        Dim frmHexView As HexViewForm

        If Offset = 0 Then
            Dim DataBlockList = _Disk.Directory.GetDataBlocks
            Dim Caption As String = "Directory - Root"
            frmHexView = New HexViewForm(_Disk, DataBlockList, False, Caption, False)
        Else
            Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(Offset)

            If DirectoryEntry.IsVolumeName Or DirectoryEntry.HasInvalidFileSize Or DirectoryEntry.StartingCluster < 2 Then
                Exit Sub
            End If

            Dim Caption As String = IIf(DirectoryEntry.IsDirectory, "Directory", "File") & " - " & DirectoryEntry.GetFullFileName

            If DirectoryEntry.IsDeleted Then
                Dim DataOffset = _Disk.ClusterToOffset(DirectoryEntry.StartingCluster)
                Dim Length = Math.Ceiling(DirectoryEntry.FileSize / _Disk.BootSector.BytesPerCluster) * _Disk.BootSector.BytesPerCluster
                Dim Block = DiskImage.Disk.NewDataBlock(DataOffset, Length)
                Caption = "Deleted " & Caption
                frmHexView = New HexViewForm(_Disk, Block, True, Caption)
            Else
                Dim DataBlocks = DirectoryEntry.GetDataBlocks
                frmHexView = New HexViewForm(_Disk, DataBlocks, False, Caption, False)
            End If
        End If
        frmHexView.ShowDialog()
    End Sub

    Private Function DiskImageLoad(ImageData As LoadedImageData) As DiskImage.Disk
        Dim Disk = New DiskImage.Disk(ImageData.FilePath, ImageData.Modifications)
        If Not Disk.LoadError Then
            ImageData.Modifications = Disk.Modifications
        End If

        Return Disk
    End Function

    Private Sub DiskImageProcess(ImageData As LoadedImageData)
        _Disk = DiskImageLoad(ImageData)

        RefreshButtonState()
        SaveButtonRefresh(_Disk.Modified)
        BtnExportDebug.Enabled = (_Disk.Modified Or ImageData.SessionModifications.Count > 0)

        PopulateSummary()
        If _Disk.IsValidImage Then
            If ImageData.CachedRootDir Is Nothing Then
                ImageData.CachedRootDir = _Disk.Directory.GetContent
            End If
            PopulateDirectory()
        Else
            ListViewFiles.Items.Clear()
        End If
    End Sub

    Private Function DiskImageSave(ImageData As LoadedImageData, Optional NewFilePath As String = "") As Boolean
        Dim Disk As DiskImage.Disk
        Dim SelectedItem As LoadedImageData = ComboGroups.SelectedItem
        Dim Success As Boolean

        Dim TempHashTable = DuplicateHashTable(ImageData.Modifications)
        Do
            If ImageData Is SelectedItem Then
                Disk = _Disk
            Else
                Disk = DiskImageLoad(ImageData)
            End If
            Success = Not Disk.LoadError
            If Success Then
                If NewFilePath = "" Then
                    NewFilePath = Disk.FilePath
                End If
                Success = SaveDiskImageToFile(Disk, NewFilePath)
            End If
            If Not Success Then
                Dim Msg As String = "Error saving file '" & IO.Path.GetFileName(ImageData.FilePath) & "'."
                Dim ErrorResult = MsgBox(Msg, MsgBoxStyle.Critical + MsgBoxStyle.RetryCancel)
                If ErrorResult = MsgBoxResult.Cancel Then
                    Exit Do
                End If
            End If
        Loop Until Success

        If Success Then
            ImageData.UpdateSessionModifications(TempHashTable)
            ItemScanModified(Disk, ImageData)
        End If

        Return Success
    End Function


    Private Sub DiskImagesScan(NewOnly As Boolean)
        Dim Disk As DiskImage.Disk
        Dim ImageData As LoadedImageData

        Me.UseWaitCursor = True
        Dim T = Stopwatch.StartNew

        BtnScanNew.Visible = False
        BtnScan.Enabled = False
        If _FiltersApplied Then
            FiltersClear()
            ImageCountUpdate()
        End If

        Dim ItemCount As Integer
        If NewOnly Then
            For Each ImageData In _LoadedImageList
                If Not ImageData.Scanned Then
                    ItemCount += 1
                End If
            Next
        Else
            ItemCount = _LoadedImageList.Count
        End If

        Dim Counter As Integer = 0
        For Each ImageData In _LoadedImageList
            Dim Percentage = Counter / ItemCount * 100
            If Counter Mod 100 = 0 Then
                BtnScan.Text = "Scanning... " & Int(Percentage) & "%"
                Application.DoEvents()
            End If
            If Not NewOnly Or Not ImageData.Scanned Then
                Disk = DiskImageLoad(ImageData)

                If Not Disk.LoadError Then
                    ItemScanModified(Disk, ImageData)
                    ItemScanValidImage(Disk, ImageData)
                    ItemScanOEMID(Disk, ImageData)
                    ItemScanUnusedClusters(Disk, ImageData)
                    ItemScanDirectory(Disk, ImageData)

                    ImageData.Scanned = True
                End If
                Counter += 1
            End If
        Next

        For Counter = 0 To [Enum].GetNames(GetType(FilterTypes)).Length - 1
            FilterUpdate(Counter)
        Next

        BtnScan.Text = "Rescan Images"
        BtnScan.Enabled = True
        _ScanRun = True

        T.Stop()
        Debug.Print("ScanImages Time Taken: " & T.Elapsed.ToString)
        Me.UseWaitCursor = False
    End Sub

    Private Sub DragDropSelectedFiles()
        If ListViewFiles.SelectedItems.Count = 0 Then
            Exit Sub
        End If

        Dim TempPath As String = IO.Path.GetTempPath() & Guid.NewGuid().ToString() & "\"

        For Each Item As ListViewItem In ListViewFiles.SelectedItems
            Dim FileData As FileData = Item.Tag
            Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(FileData.Offset)
            If Not DirectoryEntry.IsDeleted And Not DirectoryEntry.IsDirectory And Not DirectoryEntry.IsVolumeName And Not DirectoryEntry.HasInvalidFileSize And Not DirectoryEntry.HasInvalidFilename And Not DirectoryEntry.HasInvalidExtension Then
                Dim FilePath = IO.Path.Combine(TempPath, FileData.FilePath, DirectoryEntry.GetFullFileName)
                If Not IO.Directory.Exists(IO.Path.GetDirectoryName(FilePath)) Then
                    IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(FilePath))
                End If
                IO.File.WriteAllBytes(FilePath, DirectoryEntry.GetContent)
                Dim D = DirectoryEntry.GetLastWriteDate
                If D.IsValidDate Then
                    IO.File.SetLastWriteTime(FilePath, D.DateObject)
                End If
            End If
        Next

        If IO.Directory.Exists(TempPath) Then
            Dim FileList = IO.Directory.EnumerateDirectories(TempPath)
            For Each FilePath In IO.Directory.GetFiles(TempPath)
                FileList = FileList.Append(FilePath)
            Next
            If FileList.Count > 0 Then
                Dim Data = New DataObject(DataFormats.FileDrop, FileList.ToArray)
                ListViewFiles.DoDragDrop(Data, DragDropEffects.Copy)
            End If
            IO.Directory.Delete(TempPath, True)
        End If
    End Sub

    Private Function ExpandedDateToString(D As DiskImage.ExpandedDate, IncludeTime As Boolean, IncludeMilliseconds As Boolean) As String
        Dim Response As String = Format(D.Year, "0000") & "-" & Format(D.Month, "00") & "-" & Format(D.Day, "00")
        If IncludeTime Then
            Response &= "  " & Format(D.Hour, "00") _
                & ":" & Format(D.Minute, "00") _
                & ":" & Format(D.Second, "00")
        End If
        If IncludeMilliseconds Then
            Response &= Format(D.Milliseconds / 1000, ".000")
        End If

        Return Response
    End Function

    Private Sub ExportDebugScript()
        Dim ImageData As LoadedImageData = ComboGroups.SelectedItem
        GenerateDebugPackage(_Disk, ImageData)
    End Sub

    Private Sub FileClose(ImageData As LoadedImageData)
        ItemScanAll(_Disk, ImageData, True, True)
        _LoadedImageList.Remove(ImageData)
        _LoadedFileNames.Remove(ImageData.FilePath)
        Dim SelectedIndex = ComboGroups.SelectedIndex
        ComboGroups.Items.Remove(ImageData)
        If ComboGroups.Items.Count > 0 Then
            If SelectedIndex > ComboGroups.Items.Count - 1 Then
                SelectedIndex = ComboGroups.Items.Count - 1
            End If
            ComboGroups.SelectedIndex = SelectedIndex
            ImageCountUpdate()
        Else
            ResetAll()
        End If
    End Sub

    Private Sub FileDropStart(e As DragEventArgs)
        If _SuppressEvent Then
            Exit Sub
        End If
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub FileReplace(Offset As UInteger)
        Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(Offset)
        Dim ImageData As LoadedImageData = ComboGroups.SelectedItem

        Dim Dialog = New OpenFileDialog
        If Dialog.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If

        Dim FileInfo As New IO.FileInfo(Dialog.FileName)
        Dim Msg As String

        If FileInfo.Length < DirectoryEntry.FileSize Then
            Msg = FileInfo.Name & " is smaller than " & DirectoryEntry.GetFullFileName & " and will be padded with nulls." & vbCrLf & vbCrLf
        ElseIf FileInfo.Length > DirectoryEntry.FileSize Then
            Msg = FileInfo.Name & " is larger than " & DirectoryEntry.GetFullFileName & " and will be truncated." & vbCrLf & vbCrLf
        Else
            Msg = ""
        End If
        Msg &= "Do you wish to replace " & DirectoryEntry.GetFullFileName & " with " & FileInfo.FullName & "?"


        If MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
            Dim ClusterSize = _Disk.BootSector.BytesPerCluster
            Dim FileSize = Math.Min(DirectoryEntry.FileSize, DirectoryEntry.FatClusterList.Count * ClusterSize)
            Dim BytesToFill As Integer
            Dim ClusterIndex As Integer = 0
            Using fs = FileInfo.OpenRead()
                Dim BytesToRead As Integer = Math.Min(fs.Length, FileSize)
                Dim n As Integer
                Do While BytesToRead > 0
                    Dim b(ClusterSize - 1) As Byte
                    Dim Cluster = DirectoryEntry.FatClusterList(ClusterIndex)
                    Dim ClusterOffset = _Disk.ClusterToOffset(Cluster)
                    If BytesToRead < ClusterSize Then
                        b = _Disk.GetBytes(ClusterOffset, ClusterSize)
                    End If
                    n = fs.Read(b, 0, Math.Min(ClusterSize, BytesToRead))
                    BytesToRead -= n
                    FileSize -= n
                    If BytesToRead <= 0 And FileSize > 0 And n < ClusterSize Then
                        BytesToFill = Math.Min(b.Length - n, FileSize)
                        For Counter = n To n + BytesToFill - 1
                            b(Counter) = 0
                        Next
                        FileSize -= BytesToFill
                    End If
                    _Disk.SetBytes(b, ClusterOffset)

                    ClusterIndex += 1
                Loop
            End Using
            Do While FileSize > 0
                Dim b(ClusterSize - 1) As Byte
                Dim Cluster = DirectoryEntry.FatClusterList(ClusterIndex)
                Dim ClusterOffset = _Disk.ClusterToOffset(Cluster)
                BytesToFill = Math.Min(FileSize, ClusterSize)
                If BytesToFill < ClusterSize Then
                    b = _Disk.GetBytes(ClusterOffset, ClusterSize)
                End If
                For Counter = 0 To BytesToFill - 1
                    b(Counter) = 0
                Next
                FileSize -= BytesToFill
                _Disk.SetBytes(b, ClusterOffset)
                ClusterIndex += 1
            Loop
            ItemScanAll(_Disk, ComboGroups.SelectedItem, True)
            ComboItemRefresh(True)
        End If
    End Sub

    Private Sub FilesOpen()
        Dim Dialog = New OpenFileDialog With {
            .Filter = FILE_FILTER,
            .Multiselect = True
        }
        If Dialog.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If

        ProcessFileDrop(Dialog.FileNames, False)
    End Sub

    Private Sub FiltersInitialize()
        Dim FilterCount As Integer = [Enum].GetNames(GetType(FilterTypes)).Length
        ReDim _FilterCounts(FilterCount - 1)
        For Counter = 0 To FilterCount
            Dim Item = New ToolStripMenuItem With {
                .Text = FilterGetCaption(Counter, 0),
                .CheckOnClick = True,
                .Name = "key_" & Counter,
                .Visible = False,
                .Enabled = False,
                .Tag = 2 ^ Counter
            }
            AddHandler Item.CheckStateChanged, AddressOf ContextMenuFilters_CheckStateChanged
            ContextMenuFilters.Items.Add(Item)
        Next
        FilterSeparator.Visible = False
        FilterSeparator.Tag = 0
    End Sub

    Private Function FilterUpdate(ID As FilterTypes) As Boolean
        Dim Count As Integer = _FilterCounts(ID)
        Dim Item As ToolStripMenuItem = ContextMenuFilters.Items("key_" & ID)
        Dim Enabled As Boolean = (Count > 0)
        Dim CheckstateChanged As Boolean = False

        Item.Text = FilterGetCaption(ID, Count)

        If Enabled <> Item.Enabled Then
            Item.Visible = Enabled
            Item.Enabled = Enabled
            If Not Enabled And Item.CheckState = CheckState.Checked Then
                Item.CheckState = CheckState.Unchecked
                CheckstateChanged = True
            End If
            FilterSeparator.Tag = FilterSeparator.Tag + IIf(Enabled, 1, -1)
            FilterSeparator.Visible = (FilterSeparator.Tag > 0)
        End If

        If ID = FilterTypes.ModifiedFiles Then
            ToolStripModified.Text = Count & " Image" & IIf(Count <> 1, "s", "") & " Modified"
            ToolStripModified.Visible = (Count > 0)
            BtnSaveAll.Enabled = (Count > 0)
        End If

        Return CheckstateChanged
    End Function

    Private Sub FileCountUpdate()
        If ListViewFiles.SelectedItems.Count > 0 Then
            ToolStripFileCount.Text = ListViewFiles.SelectedItems.Count & " of " & ListViewFiles.Items.Count & " File" & IIf(ListViewFiles.Items.Count <> 1, "s", "") & " Selected"
        Else
            ToolStripFileCount.Text = ListViewFiles.Items.Count & " File" & IIf(ListViewFiles.Items.Count <> 1, "s", "")
        End If
    End Sub

    Private Sub ImageCountUpdate()
        If _FiltersApplied Then
            ToolStripImageCount.Text = ComboGroups.Items.Count & " of " & _LoadedImageList.Count & " Image" & IIf(_LoadedImageList.Count <> 1, "s", "")
        Else
            ToolStripImageCount.Text = _LoadedImageList.Count & " Image" & IIf(_LoadedImageList.Count <> 1, "s", "")
        End If
    End Sub


    Private Sub FiltersApply()
        Dim FilterCount As Integer = [Enum].GetNames(GetType(FilterTypes)).Length
        Dim Count As Integer = 0
        For Counter = 0 To FilterCount - 1
            Dim Item As ToolStripMenuItem = ContextMenuFilters.Items("key_" & Counter)
            If Item.CheckState = CheckState.Checked Then
                Count += 1
            End If
        Next

        If Count > 0 Then
            Me.UseWaitCursor = True

            Dim SelectedItem As LoadedImageData = ComboGroups.SelectedItem
            ComboGroups.Items.Clear()
            Dim AppliedFilters As Integer = 0
            For Counter = 0 To FilterCount - 1
                Dim Item As ToolStripMenuItem = ContextMenuFilters.Items("key_" & Counter)
                If Item.CheckState = CheckState.Checked Then
                    AppliedFilters += Item.Tag
                End If
            Next
            For Each ImageData In _LoadedImageList
                If Not IsFiltered(ImageData, AppliedFilters) Then
                    ImageData.ComboIndex = ComboGroups.Items.Add(ImageData)
                Else
                    ImageData.ComboIndex = -1
                End If
            Next
            If ComboGroups.Items.Count > 0 Then
                Dim SearchIndex = ComboGroups.Items.IndexOf(SelectedItem)
                If SearchIndex > -1 Then
                    ComboGroups.SelectedIndex = SearchIndex
                Else
                    ComboGroups.SelectedIndex = 0
                End If
            End If
            MainMenuFilters.BackColor = Color.LightGreen

            _FiltersApplied = True

            Me.UseWaitCursor = False
        Else
            If _FiltersApplied Then
                FiltersClear()
            End If
        End If
        ImageCountUpdate()
    End Sub

    Private Sub FiltersClear()
        Me.UseWaitCursor = True

        _SuppressEvent = True
        For Counter = 0 To [Enum].GetNames(GetType(FilterTypes)).Length - 1
            Dim Item As ToolStripMenuItem = ContextMenuFilters.Items("key_" & Counter)
            If Item.CheckState = CheckState.Checked Then
                Item.CheckState = CheckState.Unchecked
            End If
        Next
        _SuppressEvent = False

        Dim SelectedItem As LoadedImageData = ComboGroups.SelectedItem
        ComboGroups.Items.Clear()
        For Each ImageData In _LoadedImageList
            ImageData.ComboIndex = ComboGroups.Items.Add(ImageData)
        Next
        If ComboGroups.Items.Count > 0 Then
            Dim SearchIndex = ComboGroups.Items.IndexOf(SelectedItem)
            If SearchIndex > -1 Then
                ComboGroups.SelectedIndex = SearchIndex
            Else
                ComboGroups.SelectedIndex = 0
            End If
        End If

        MainMenuFilters.BackColor = SystemColors.Control
        _FiltersApplied = False

        Me.UseWaitCursor = False
    End Sub

    Private Sub FiltersReset()
        For Counter = 0 To _FilterCounts.Length - 1
            _FilterCounts(Counter) = 0
            FilterUpdate(Counter)
        Next
        BtnScan.Text = "Scan Images"
    End Sub

    Private Function GetFileDataFromDirectoryEntry(DirectoryEntry As DiskImage.DirectoryEntry, FilePath As String) As FileData
        Dim Response As FileData
        With Response
            .FilePath = FilePath
            .Offset = DirectoryEntry.Offset
        End With

        Return Response
    End Function

    Public Function GetPathOffset() As Integer
        Dim PathName As String = ""
        Dim CheckPath As Boolean = False
        For Each ImageData In _LoadedImageList
            Dim CurrentPathName As String = IO.Path.GetDirectoryName(ImageData.FilePath)
            If CheckPath Then
                If Len(CurrentPathName) > Len(PathName) Then
                    CurrentPathName = Strings.Left(CurrentPathName, Len(PathName))
                End If
                Do While PathName <> CurrentPathName
                    PathName = IO.Path.GetDirectoryName(PathName)
                    CurrentPathName = IO.Path.GetDirectoryName(CurrentPathName)
                Loop
            Else
                PathName = CurrentPathName
            End If
            If PathName = "" Then
                Exit For
            End If
            CheckPath = True
        Next
        PathName = PathAddBackslash(PathName)

        Return Len(PathName)
    End Function

    Private Sub InitializeOptionalColumns()
        If Not ListViewFiles.Columns.ContainsKey("FileCreateDate") Then
            ListViewAddColumn(ListViewFiles, "FileCreateDate", "Created", 0, 8)
        End If
        If Not ListViewFiles.Columns.ContainsKey("FileLastAccessDate") Then
            ListViewAddColumn(ListViewFiles, "FileLastAccessDate", "Last Accessed", 0, 9)
        End If
        If Not ListViewFiles.Columns.ContainsKey("FileLFN") Then
            ListViewAddColumn(ListViewFiles, "FileLFN", "Long File Name", 0, 10)
        End If
    End Sub

    Private Sub ItemScanAll(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        ItemScanModified(Disk, ImageData, UpdateFilters, Remove)
        If ImageData.Scanned Then
            ItemScanValidImage(Disk, ImageData, UpdateFilters, Remove)
            ItemScanOEMID(Disk, ImageData, UpdateFilters, Remove)
            ItemScanUnusedClusters(Disk, ImageData, UpdateFilters, Remove)
            ItemScanDirectory(Disk, ImageData, UpdateFilters, Remove)
        End If
    End Sub

    Private Sub ItemScanDirectory(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim HasCreated As Boolean = False
        Dim HasLastAccessed As Boolean = False
        Dim HasLongFileNames As Boolean = False
        Dim HasInvalidDirectoryEntries As Boolean = False

        If Not Remove And Disk.IsValidImage Then
            Dim Response As ProcessDirectoryEntryResponse = ProcessDirectoryEntries(Disk.Directory, "", True)
            HasCreated = Response.HasCreated
            HasLastAccessed = Response.HasLastAccessed
            HasLongFileNames = Response.HasLFN
            HasInvalidDirectoryEntries = Response.HasInvalidDirectoryEntries
        End If

        If Not ImageData.Scanned Or HasCreated <> ImageData.ScanInfo.HasCreated Then
            ImageData.ScanInfo.HasCreated = HasCreated
            If HasCreated Then
                _FilterCounts(FilterTypes.HasCreated) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.HasCreated) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.HasCreated)
            End If
        End If

        If Not ImageData.Scanned Or HasLastAccessed <> ImageData.ScanInfo.HasLastAccessed Then
            ImageData.ScanInfo.HasLastAccessed = HasLastAccessed
            If HasLastAccessed Then
                _FilterCounts(FilterTypes.HasLastAccessed) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.HasLastAccessed) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.HasLastAccessed)
            End If
        End If

        If Not ImageData.Scanned Or HasLongFileNames <> ImageData.ScanInfo.HasLongFileNames Then
            ImageData.ScanInfo.HasLongFileNames = HasLongFileNames
            If HasLongFileNames Then
                _FilterCounts(FilterTypes.HasLongFileNames) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.HasLongFileNames) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.HasLongFileNames)
            End If
        End If

        If Not ImageData.Scanned Or HasInvalidDirectoryEntries <> ImageData.ScanInfo.HasInvalidDirectoryEntries Then
            ImageData.ScanInfo.HasInvalidDirectoryEntries = HasInvalidDirectoryEntries
            If HasInvalidDirectoryEntries Then
                _FilterCounts(FilterTypes.HasInvalidDirectoryEntries) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.HasInvalidDirectoryEntries) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.HasInvalidDirectoryEntries)
            End If
        End If
    End Sub

    Private Sub ItemScanModified(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim IsModified As Boolean = Not Remove And Disk.Modified

        If IsModified <> ImageData.Modified Then
            ImageData.Modified = IsModified
            If IsModified Then
                _FilterCounts(FilterTypes.ModifiedFiles) += 1
            Else
                _FilterCounts(FilterTypes.ModifiedFiles) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.ModifiedFiles)
            End If
        End If
    End Sub

    Private Sub ItemScanOEMID(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim OEMIDMatched As Boolean = True
        Dim OEMIDFound As Boolean = True

        If Not Remove And Disk.IsValidImage Then
            Dim BootstrapChecksum = Crc32.ComputeChecksum(Disk.BootSector.BootStrapCode)
            Dim OEMIDString As String = Encoding.UTF8.GetString(Disk.BootSector.OEMID)

            Dim BootstrapType = OEMIDFindMatch(BootstrapChecksum)
            OEMIDFound = BootstrapType IsNot Nothing
            If OEMIDFound Then
                OEMIDMatched = BootstrapType.OEMIDList.Contains(OEMIDString)
            Else
                OEMIDMatched = True
            End If
        End If

        If Not ImageData.Scanned Or OEMIDMatched <> ImageData.ScanInfo.OEMIDMatched Then
            ImageData.ScanInfo.OEMIDMatched = OEMIDMatched
            If Not OEMIDMatched Then
                _FilterCounts(FilterTypes.MismatchedOEMID) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.MismatchedOEMID) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.MismatchedOEMID)
            End If
        End If

        If Not ImageData.Scanned Or OEMIDFound <> ImageData.ScanInfo.OEMIDFound Then
            ImageData.ScanInfo.OEMIDFound = OEMIDFound
            If Not OEMIDFound Then
                _FilterCounts(FilterTypes.UnknownOEMID) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.UnknownOEMID) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.UnknownOEMID)
            End If
        End If
    End Sub

    Private Sub ItemScanUnusedClusters(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim HasUnusedClusters As Boolean = False

        If Not Remove And Disk.IsValidImage Then
            HasUnusedClusters = Disk.HasUnusedClustersWithData
        End If

        If Not ImageData.Scanned Or HasUnusedClusters <> ImageData.ScanInfo.HasUnusedClusters Then
            ImageData.ScanInfo.HasUnusedClusters = HasUnusedClusters
            If HasUnusedClusters Then
                _FilterCounts(FilterTypes.UnusedClusters) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.UnusedClusters) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.UnusedClusters)
            End If
        End If
    End Sub

    Private Sub ItemScanValidImage(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim IsValidImage As Boolean = Disk.IsValidImage Or Remove


        If Not ImageData.Scanned Or IsValidImage <> ImageData.ScanInfo.IsValidImage Then
            ImageData.ScanInfo.IsValidImage = IsValidImage
            If Not IsValidImage Then
                _FilterCounts(FilterTypes.HasInvalidImage) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.HasInvalidImage) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.HasInvalidImage)
            End If
        End If
    End Sub

    Private Sub ItemSelectionChanged()
        RefreshFileButtons()
        FileCountUpdate()
        Dim CheckAll = (ListViewFiles.SelectedItems.Count = ListViewFiles.Items.Count)
        If CheckAll <> _CheckAll Then
            _CheckAll = CheckAll
            ListViewFiles.Invalidate(New Rectangle(0, 0, 20, 20), True)
        End If
    End Sub

    Private Function ListViewFilesGetItem(DirectoryEntry As DiskImage.DirectoryEntry, Group As ListViewGroup, LFNFileName As String, FileData As FileData) As ListViewItem
        Dim SI As ListViewItem.ListViewSubItem
        Dim ForeColor As Color

        Dim Attrib As String = IIf(DirectoryEntry.IsArchive, "A ", "- ") _
            & IIf(DirectoryEntry.IsReadOnly, "R ", "- ") _
            & IIf(DirectoryEntry.IsSystem, "S ", "- ") _
            & IIf(DirectoryEntry.IsHidden, "H ", "- ") _
            & IIf(DirectoryEntry.IsDirectory, "D ", "- ") _
            & IIf(DirectoryEntry.IsVolumeName, "V ", "- ")

        If DirectoryEntry.IsDeleted Then
            ForeColor = Color.Gray
        ElseIf DirectoryEntry.IsVolumeName Then
            ForeColor = Color.Green
        ElseIf DirectoryEntry.IsDirectory Then
            ForeColor = Color.Blue
        End If

        Dim ModifiedString As String = IIf(DirectoryEntry.IsModified, "#", "")

        Dim Item = New ListViewItem(ModifiedString, Group) With {
            .UseItemStyleForSubItems = False,
            .Tag = FileData
        }
        If ModifiedString = "" Then
            Item.ForeColor = ForeColor
        Else
            Item.ForeColor = Color.Blue
        End If

        SI = Item.SubItems.Add(Encoding.UTF8.GetString(DirectoryEntry.FileName).Trim)
        If Not DirectoryEntry.IsDeleted And DirectoryEntry.HasInvalidFilename Then
            SI.ForeColor = Color.Red
        Else
            SI.ForeColor = ForeColor
        End If

        SI = Item.SubItems.Add(Encoding.UTF8.GetString(DirectoryEntry.Extension).Trim)
        If Not DirectoryEntry.IsDeleted And DirectoryEntry.HasInvalidExtension Then
            SI.ForeColor = Color.Red
        Else
            SI.ForeColor = ForeColor
        End If

        If Not DirectoryEntry.IsDeleted And DirectoryEntry.HasInvalidFileSize Then
            SI = Item.SubItems.Add("Invalid")
            SI.ForeColor = Color.Red
        Else
            SI = Item.SubItems.Add(Format(DirectoryEntry.FileSize, "N0"))
            SI.ForeColor = ForeColor
        End If

        SI = Item.SubItems.Add(ExpandedDateToString(DirectoryEntry.GetLastWriteDate, True, False))
        If DirectoryEntry.GetLastWriteDate.IsValidDate Or DirectoryEntry.IsDeleted Then
            SI.ForeColor = ForeColor
        Else
            SI.ForeColor = Color.Red
        End If

        SI = Item.SubItems.Add(Format(DirectoryEntry.StartingCluster, "N0"))
        SI.ForeColor = ForeColor

        SI = Item.SubItems.Add(Attrib)
        If Not DirectoryEntry.IsDeleted And DirectoryEntry.HasInvalidAttributes Then
            SI.ForeColor = Color.Red
        Else
            SI.ForeColor = ForeColor
        End If

        If Not DirectoryEntry.IsDeleted And Not DirectoryEntry.IsDirectory And Not DirectoryEntry.IsVolumeName And Not DirectoryEntry.HasInvalidFileSize Then
            SI = Item.SubItems.Add(Crc32.ComputeChecksum(DirectoryEntry.GetContent).ToString("X8"))
        Else
            SI = Item.SubItems.Add("")
        End If
        SI.ForeColor = ForeColor

        If DirectoryEntry.HasCreationDate Then
            SI = Item.SubItems.Add(ExpandedDateToString(DirectoryEntry.GetCreationDate, True, True))
            If DirectoryEntry.GetCreationDate.IsValidDate Or DirectoryEntry.IsDeleted Then
                SI.ForeColor = ForeColor
            Else
                SI.ForeColor = Color.Red
            End If
        Else
            SI = Item.SubItems.Add("")
        End If
        SI.Name = "FileCreateDate"

        If DirectoryEntry.HasLastAccessDate Then
            SI = Item.SubItems.Add(ExpandedDateToString(DirectoryEntry.GetLastAccessDate, False, False))
            If DirectoryEntry.GetLastAccessDate.IsValidDate Or DirectoryEntry.IsDeleted Then
                SI.ForeColor = ForeColor
            Else
                SI.ForeColor = Color.Red
            End If
        Else
            SI = Item.SubItems.Add("")
        End If
        SI.Name = "FileLastAccessDate"

        SI = Item.SubItems.Add(LFNFileName)
        SI.ForeColor = ForeColor
        SI.Name = "FileLFN"

        Return Item
    End Function

    Private Function ListViewTileGetItem(Name As String, Value As String) As ListViewItem
        Dim Item = New ListViewItem(Name) With {
                .UseItemStyleForSubItems = False
            }
        Item.SubItems.Add(Value)

        Return Item
    End Function

    Private Function ListViewTileGetItem(Name As String, Value As String, ForeColor As Color) As ListViewItem
        Dim Item = New ListViewItem(Name) With {
                .UseItemStyleForSubItems = False
            }
        Dim SubItem = Item.SubItems.Add(Value)
        SubItem.ForeColor = ForeColor

        Return Item
    End Function

    Private Function MediaTypeGet(MediaDescriptor As Byte, SectorsPerTrack As UShort) As String
        Select Case MediaDescriptor
            Case &HF0
                If SectorsPerTrack = 36 Then
                    Return "2.88M Floppy"
                ElseIf SectorsPerTrack = 21 Then
                    Return "DMF Floppy"
                Else
                    Return "1.44M Floppy"
                End If
            Case &HF8
                Return "Fixed Disk"
            Case &HF9
                If SectorsPerTrack = 15 Then
                    Return "1.2M Floppy"
                Else
                    Return "720KB Floppy"
                End If
            Case &HFC
                Return "180KB Floppy"
            Case &HFD
                Return "360KB Floppy"
            Case &HFE
                Return "160KB Floppy"
            Case &HFF
                Return "320KB Floppy"
            Case Else
                Return "Unknown"
        End Select
    End Function

    Private Sub MenuDisplayDirectorySubMenuClear()
        For Each Item As ToolStripMenuItem In BtnDisplayDirectory.DropDownItems
            RemoveHandler Item.Click, AddressOf BtnDisplayDirectory_Click
        Next
        BtnDisplayDirectory.DropDownItems.Clear()
        BtnDisplayDirectory.Text = "Root Directory"
    End Sub

    Private Sub MenuDisplayDirectorySubMenuItemAdd(Path As String, Offset As UInteger, Index As Integer)
        Dim Item As New ToolStripMenuItem With {
            .Text = Path,
            .Tag = Offset
        }
        If Index = -1 Then
            BtnDisplayDirectory.DropDownItems.Add(Item)
        Else
            BtnDisplayDirectory.DropDownItems.Insert(Index, Item)
        End If
        AddHandler Item.Click, AddressOf BtnDisplayDirectory_Click
    End Sub

    Private Function FilePropertiesEdit() As Boolean
        Dim Result As Boolean = False

        Dim frmFileProperties As New FilePropertiesForm(_Disk, ListViewFiles.SelectedItems)
        frmFileProperties.ShowDialog()

        If frmFileProperties.DialogResult = DialogResult.OK Then
            Result = frmFileProperties.Updated

            If Result Then
                ItemScanAll(_Disk, ComboGroups.SelectedItem, True)
                ComboItemRefresh(True)
            End If
        End If

        Return Result
    End Function

    Private Function OEMIDEdit() As Boolean
        Dim frmOEMID As New OEMIDForm(_Disk, _OEMIDDictionary)
        Dim Result As Boolean

        frmOEMID.ShowDialog()

        Result = frmOEMID.DialogResult = DialogResult.OK

        If Result Then
            ItemScanAll(_Disk, ComboGroups.SelectedItem, True)
            ComboItemRefresh(False)
        End If

        Return Result
    End Function

    Private Function OEMIDFindMatch(Checksum As UInteger) As OEMIDList
        If _OEMIDDictionary.ContainsKey(Checksum) Then
            Return _OEMIDDictionary.Item(Checksum)
        Else
            Return Nothing
        End If
    End Function

    Private Sub PopulateDirectory()
        ListViewFiles.BeginUpdate()
        ListViewFiles.Items.Clear()
        ListViewFiles.MultiSelect = True

        InitializeOptionalColumns()

        Dim Items As New List(Of ListViewItem)
        Dim Response As ProcessDirectoryEntryResponse = ProcessDirectoryEntries(_Disk.Directory, "", False)

        If BtnDisplayDirectory.DropDownItems.Count > 0 Then
            BtnDisplayDirectory.Text = "Directory"
            MenuDisplayDirectorySubMenuItemAdd("(Root)", 0, 0)
            BtnDisplayDirectory.Tag = Nothing
        Else
            BtnDisplayDirectory.Tag = 0
        End If

        If Not Response.HasCreated Then
            ListViewFiles.Columns.RemoveByKey("FileCreateDate")
        Else
            ListViewFiles.Columns.Item("FileCreateDate").Width = 140
        End If
        If Not Response.HasLastAccessed Then
            ListViewFiles.Columns.RemoveByKey("FileLastAccessDate")
        Else
            ListViewFiles.Columns.Item("FileLastAccessDate").Width = 90
        End If
        If Not Response.HasLFN Then
            ListViewFiles.Columns.RemoveByKey("FileLFN")
        Else
            ListViewFiles.Columns.Item("FileLFN").Width = 200
        End If
        ListViewFiles.EndUpdate()

        ItemSelectionChanged()
    End Sub

    Private Sub PopulateSummary()
        Dim ForeColor As Color

        Me.Text = "Disk Image Tool - " & IO.Path.GetFileName(_Disk.FilePath)
        ToolStripFileName.Text = IO.Path.GetFileName(_Disk.FilePath)
        ToolStripFileName.Visible = True

        ListViewSummary.BeginUpdate()
        With ListViewSummary.Items
            .Clear()
            If _Disk.IsValidImage Then
                Dim BootstrapChecksum = Crc32.ComputeChecksum(_Disk.BootSector.BootStrapCode)
                Dim OEMIDString As String = Encoding.UTF8.GetString(_Disk.BootSector.OEMID)
                Dim OEMIDMatched As Boolean = False

                Dim BootstrapType = OEMIDFindMatch(BootstrapChecksum)

                If BootstrapType IsNot Nothing Then
                    OEMIDMatched = BootstrapType.OEMIDList.Contains(OEMIDString)
                End If

                .Add(ListViewTileGetItem("Modified:", IIf(_Disk.Modified, "Yes", "No"), IIf(_Disk.Modified, Color.Blue, SystemColors.WindowText)))
                If BootstrapType IsNot Nothing Then
                    If Not OEMIDMatched Then
                        ForeColor = Color.Red
                    Else
                        ForeColor = Color.Green
                    End If
                Else
                    ForeColor = SystemColors.WindowText
                End If
                .Add(ListViewTileGetItem("OEM ID:", OEMIDString, ForeColor))
                If BootstrapType IsNot Nothing Then
                    .Add(ListViewTileGetItem("Language:", _OEMIDDictionary.Item(BootstrapChecksum).Language))
                End If
                .Add(ListViewTileGetItem("Media Type:", MediaTypeGet(_Disk.BootSector.MediaDescriptor, _Disk.BootSector.SectorsPerTrack)))

                Dim VolumeLabel = _Disk.GetVolumeLabel
                If VolumeLabel <> "" Then
                    .Add(ListViewTileGetItem("Volume Label:", VolumeLabel))
                End If
                If _Disk.BootSector.ExtendedBootSignature = &H29 Then
                    .Add(ListViewTileGetItem("Volume Serial Number:", _Disk.BootSector.VolumeSerialNumber.ToString("X8").Insert(4, "-")))
                    .Add(ListViewTileGetItem("File System Type:", Encoding.UTF8.GetString(_Disk.BootSector.FileSystemType)))
                End If
                .Add(ListViewTileGetItem("Bytes Per Sector:", _Disk.BootSector.BytesPerSector))
                .Add(ListViewTileGetItem("Sectors Per Cluster:", _Disk.BootSector.SectorsPerCluster))
                .Add(ListViewTileGetItem("Sectors Per Track:", _Disk.BootSector.SectorsPerTrack))
                .Add(ListViewTileGetItem("Free Space:", Format(_Disk.FreeSpace, "N0") & " bytes"))
                If BootstrapType IsNot Nothing Then
                    If Not OEMIDMatched Then
                        For Each OEMID In BootstrapType.OEMIDList
                            .Add(ListViewTileGetItem("Detected OEM ID:", OEMID))
                        Next
                    End If
                End If
            Else
                If _Disk.LoadError Then
                    .Add(ListViewTileGetItem("Error:", "Error Loading File", Color.Red))
                Else
                    .Add(ListViewTileGetItem("Error:", "Invalid Disk Image", Color.Red))
                End If
            End If
        End With
        ListViewSummary.EndUpdate()
        ListViewSummary.Refresh()

        ListViewHashes.BeginUpdate()
        With ListViewHashes.Items
            .Clear()
            If Not _Disk.LoadError Then
                .Add(ListViewTileGetItem("CRC32", Crc32.ComputeChecksum(_Disk.Data).ToString("X8")))
                .Add(ListViewTileGetItem("MD5", MD5Hash(_Disk.Data)))
                .Add(ListViewTileGetItem("SHA-1", SHA1Hash(_Disk.Data)))
            End If
        End With
        ListViewHashes.EndUpdate()
        ListViewHashes.Refresh()

        If _Disk.IsValidImage Then
            BtnDisplayClusters.Enabled = _Disk.HasUnusedClustersWithData
        Else
            BtnDisplayClusters.Enabled = False
        End If
        BtnRevert.Enabled = _Disk.Modified
    End Sub

    Private Function ProcessDirectoryEntries(Directory As DiskImage.Directory, Path As String, ScanOnly As Boolean) As ProcessDirectoryEntryResponse
        Dim Group As ListViewGroup = Nothing
        Dim Counter As UInteger
        Dim FileCount As UInteger = Directory.FileCount
        Dim LFNFileName As String = ""

        Dim Response As ProcessDirectoryEntryResponse
        With Response
            .HasCreated = False
            .HasLFN = False
            .HasLastAccessed = False
            .HasInvalidDirectoryEntries = False
        End With

        If Not ScanOnly Then
            Dim GroupName As String = IIf(Path = "", "(Root)", Path)
            GroupName = GroupName & "  (" & FileCount & IIf(FileCount <> 1, " entries", " entry") & ")"
            Group = New ListViewGroup(GroupName)
            ListViewFiles.Groups.Add(Group)
        End If

        For Counter = 1 To Directory.DirectoryLength
            Dim File = Directory.GetFile(Counter)
            Dim FullFileName = File.GetFullFileName
            Dim FileData = GetFileDataFromDirectoryEntry(File, Path)

            If FullFileName <> "." And FullFileName <> ".." Then
                If File.IsLFN Then
                    LFNFileName = File.GetLFNFileName & LFNFileName
                Else
                    If Not ScanOnly Then
                        Dim item = ListViewFilesGetItem(File, Group, LFNFileName, FileData)
                        ListViewFiles.Items.Add(item)
                    End If

                    If Not Response.HasInvalidDirectoryEntries Then
                        If Not File.IsDeleted Then
                            If File.HasInvalidFilename Or File.HasInvalidExtension Or File.HasInvalidFileSize Or File.HasInvalidAttributes Or Not File.GetLastWriteDate.IsValidDate Then
                                Response.HasInvalidDirectoryEntries = True
                            End If
                        End If
                    End If

                    If Not Response.HasCreated Then
                        If File.HasCreationDate Then
                            Response.HasCreated = True
                            If Not Response.HasInvalidDirectoryEntries Then
                                If Not File.IsDeleted Then
                                    If Not File.GetCreationDate.IsValidDate Then
                                        Response.HasInvalidDirectoryEntries = True
                                    End If
                                End If
                            End If
                        End If
                    End If
                    If Not Response.HasLastAccessed Then
                        If File.HasLastAccessDate Then
                            Response.HasLastAccessed = True
                            If Not Response.HasInvalidDirectoryEntries Then
                                If Not File.IsDeleted Then
                                    If Not File.GetLastAccessDate.IsValidDate Then
                                        Response.HasInvalidDirectoryEntries = True
                                    End If
                                End If
                            End If
                        End If
                    End If

                    If Not Response.HasLFN Then
                        If LFNFileName <> "" Then
                            Response.HasLFN = True
                        End If
                    End If
                    LFNFileName = ""
                End If

                If File.IsDirectory And File.SubDirectory IsNot Nothing Then
                    If FullFileName <> "." And FullFileName <> ".." And File.SubDirectory.DirectoryLength > 0 Then
                        Dim NewPath = FullFileName
                        If Path <> "" Then
                            NewPath = Path & "\" & NewPath
                        End If
                        If Not ScanOnly Then
                            MenuDisplayDirectorySubMenuItemAdd(NewPath, File.Offset, -1)
                        End If
                        Dim SubResponse = ProcessDirectoryEntries(File.SubDirectory, NewPath, ScanOnly)
                        Response.HasLastAccessed = Response.HasLastAccessed Or SubResponse.HasLastAccessed
                        Response.HasCreated = Response.HasCreated Or SubResponse.HasCreated
                        Response.HasLFN = Response.HasLFN Or SubResponse.HasLFN
                        Response.HasInvalidDirectoryEntries = Response.HasInvalidDirectoryEntries Or SubResponse.HasInvalidDirectoryEntries
                    End If
                End If
            End If
        Next

        Return Response
    End Function

    Private Sub ProcessFileDrop(File As String, AllowAllExtensions As Boolean)
        Dim Files(0) As String
        Files(0) = File

        ProcessFileDrop(Files, AllowAllExtensions)
    End Sub

    Private Sub ProcessFileDrop(Files() As String, AllowAllExtensions As Boolean)
        Dim AllowedExtensions = {".img", ".ima"}
        Dim FilePath As String
        Dim FileInfo As IO.FileInfo
        Dim SelectedImageData As LoadedImageData = Nothing

        Me.UseWaitCursor = True
        ComboGroups.BeginUpdate()

        LoadedImageData.StringOffset = 0

        For Each FilePath In Files.OrderBy(Function(f) f)
            Dim FAttributes = IO.File.GetAttributes(FilePath)
            If (FAttributes And IO.FileAttributes.Directory) > 0 Then
                Dim DirectoryInfo As New IO.DirectoryInfo(FilePath)
                For Each FileInfo In DirectoryInfo.GetFiles("*.im*", IO.SearchOption.AllDirectories)
                    If AllowedExtensions.Contains(FileInfo.Extension.ToLower) Then
                        If Not _LoadedFileNames.ContainsKey(FileInfo.FullName) Then
                            Dim ImageData As New LoadedImageData(FileInfo.FullName)
                            _LoadedFileNames.Add(FileInfo.FullName, ImageData)
                            ImageData.ComboIndex = ComboGroups.Items.Add(ImageData)
                            _LoadedImageList.Add(ImageData)
                            If SelectedImageData Is Nothing Then
                                SelectedImageData = ImageData
                            End If
                        End If
                    End If
                Next
            Else
                Dim Ext As String = IO.Path.GetExtension(FilePath)
                If AllowAllExtensions Or AllowedExtensions.Contains(Ext.ToLower) Then
                    If Not _LoadedFileNames.ContainsKey(FilePath) Then
                        Dim ImageData As New LoadedImageData(FilePath)
                        _LoadedFileNames.Add(FilePath, ImageData)
                        ImageData.ComboIndex = ComboGroups.Items.Add(ImageData)
                        _LoadedImageList.Add(ImageData)
                        If SelectedImageData Is Nothing Then
                            SelectedImageData = ImageData
                        End If
                    End If
                End If
            End If
        Next

        LoadedImageData.StringOffset = GetPathOffset()

        If SelectedImageData IsNot Nothing Then
            ComboGroups.Enabled = True
            If _FiltersApplied Then
                FiltersClear()
            Else
                ComboGroupRefreshItemText()
            End If
            ImageCountUpdate()
            ComboGroups.SelectedItem = SelectedImageData
            If ComboGroups.SelectedIndex = -1 Then
                ComboGroups.SelectedIndex = 0
            End If
            ToolStripImageCount.Visible = True
            LabelDropMessage.Visible = False
            BtnScan.Enabled = True
            BtnScanNew.Enabled = True
            If _ScanRun Then
                BtnScanNew.Visible = True
            End If
            BtnClose.Enabled = True
            BtnCloseAll.Enabled = True
        End If

        ComboGroups.EndUpdate()
        Me.UseWaitCursor = False
    End Sub

    Private Sub RefreshFileButtons()
        BtnFileProperties.Enabled = (ListViewFiles.SelectedItems.Count > 0)
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim FileData = ListViewFiles.SelectedItems(0).Tag
            Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(FileData.Offset)

            If (Not DirectoryEntry.IsDirectory Or DirectoryEntry.IsDeleted) _
                And Not DirectoryEntry.IsVolumeName _
                And Not DirectoryEntry.HasInvalidFileSize _
                And DirectoryEntry.StartingCluster > 1 Then

                If DirectoryEntry.IsDeleted Then
                    BtnDisplayFile.Text = "Deleted &File:  " & DirectoryEntry.GetFullFileName
                    BtnReplaceFile.Enabled = False
                Else
                    BtnDisplayFile.Text = "&File:  " & DirectoryEntry.GetFullFileName
                    BtnReplaceFile.Enabled = True
                End If
                BtnDisplayFile.Tag = FileData.Offset
                BtnDisplayFile.Visible = True
            Else
                BtnDisplayFile.Tag = Nothing
                BtnDisplayFile.Visible = False
                BtnReplaceFile.Enabled = False
            End If
        Else
            BtnDisplayFile.Tag = Nothing
            BtnDisplayFile.Visible = False
            BtnReplaceFile.Enabled = False
        End If
    End Sub

    Private Sub RefreshButtonState()
        If _Disk.IsValidImage Then
            BtnOEMID.Enabled = True
            BtnDisplayBootSector.Enabled = True
            BtnDisplayDirectory.Enabled = True
        Else
            BtnOEMID.Enabled = False
            BtnDisplayBootSector.Enabled = False
            BtnDisplayDirectory.Enabled = False
        End If
        BtnDisplayFile.Tag = Nothing
        BtnDisplayFile.Visible = False
        MenuDisplayDirectorySubMenuClear()
    End Sub

    Private Sub RevertChanges()
        If _Disk.Modified Then
            _Disk.RevertChanges()
            ItemScanAll(_Disk, ComboGroups.SelectedItem, True)
            ComboItemRefresh(True)
        End If
    End Sub

    Private Sub SaveAll()
        _SuppressEvent = True
        For Each ImageData In _LoadedImageList
            If ImageData.Modified Then
                Dim Result = DiskImageSave(ImageData)
                If Result Then
                    If ImageData.ComboIndex > -1 Then
                        _SuppressEvent = True
                        If ImageData Is ComboGroups.SelectedItem Then
                            ComboItemRefresh(True)
                        Else
                            _SuppressEvent = True
                            ComboGroups.Items(ImageData.ComboIndex) = ComboGroups.Items(ImageData.ComboIndex)
                            _SuppressEvent = False
                        End If
                    End If
                End If
            End If
        Next
        _SuppressEvent = False

        FilterUpdate(FilterTypes.ModifiedFiles)
    End Sub

    Private Sub SaveCurrent(NewFileName As Boolean)
        Dim FilePath As String

        If NewFileName Then
            Dim Dialog = New SaveFileDialog With {
                .InitialDirectory = IO.Path.GetDirectoryName(_Disk.FilePath),
                .FileName = IO.Path.GetFileName(_Disk.FilePath),
                .Filter = FILE_FILTER
            }
            If Dialog.ShowDialog <> DialogResult.OK Then
                Exit Sub
            End If
            FilePath = Dialog.FileName
        Else
            FilePath = ""
        End If

        Dim ImageData As LoadedImageData = ComboGroups.SelectedItem

        Dim Result = DiskImageSave(ImageData, FilePath)
        If Result Then
            FilterUpdate(FilterTypes.ModifiedFiles)
            If NewFileName Then
                If _LoadedFileNames.ContainsKey(FilePath) Then
                    FileClose(_LoadedFileNames.Item(FilePath))
                End If
                _LoadedFileNames.Remove(ImageData.FilePath)
                _LoadedFileNames.Add(FilePath, ImageData)
                ImageData.FilePath = FilePath
                ComboGroups.BeginUpdate()
                LoadedImageData.StringOffset = GetPathOffset()
                ComboGroupRefreshItemText()
                ComboGroups.EndUpdate()
            End If
            ComboItemRefresh(True)
        End If
    End Sub

    Private Function SaveDiskImageToFile(Disk As DiskImage.Disk, FilePath As String) As Boolean
        Try
            If IO.File.Exists(FilePath) Then
                Dim BackupPath As String = FilePath & ".bak"
                IO.File.Copy(FilePath, BackupPath, True)
            End If
            Disk.SaveFile(FilePath)
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Private Sub SaveButtonRefresh(Enabled As Boolean)
        BtnSave.Enabled = Enabled
        BtnSaveAs.Enabled = Enabled
    End Sub

    Private Sub UnusedClustersDisplayHex()
        Dim DataBlockList = _Disk.GetUnusedClusterDataBlocks(True)

        Dim frmHexView As New HexViewForm(_Disk, DataBlockList, True, "Unused Clusters", False)
        frmHexView.ShowDialog()

        If frmHexView.Modified Then
            ItemScanAll(_Disk, ComboGroups.SelectedItem, True)
            ComboItemRefresh(False)
        End If
    End Sub

#Region "Events"
    Private Sub BtnOEMID_Click(sender As Object, e As EventArgs) Handles BtnOEMID.Click
        OEMIDEdit()
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        CloseCurrent()
    End Sub

    Private Sub BtnCloseAll_Click(sender As Object, e As EventArgs) Handles BtnCloseAll.Click
        CloseAll()
    End Sub

    Private Sub BtnExportDebug_Click(sender As Object, e As EventArgs) Handles BtnExportDebug.Click
        ExportDebugScript()
    End Sub

    Private Sub BtnDisplayBootSector_Click(sender As Object, e As EventArgs) Handles BtnDisplayBootSector.Click
        BootSectorDisplayHex()
    End Sub

    Private Sub BtnDisplayClusters_Click(sender As Object, e As EventArgs) Handles BtnDisplayClusters.Click
        UnusedClustersDisplayHex()
    End Sub

    Private Sub BtnDisplayDirectory_Click(sender As Object, e As EventArgs) Handles BtnDisplayDirectory.Click
        If sender.Tag IsNot Nothing Then
            DirectoryEntryDisplayHex(sender.tag)
        End If
    End Sub

    Private Sub BtnDisplayFile_Click(sender As Object, e As EventArgs) Handles BtnDisplayFile.Click
        If sender.tag IsNot Nothing Then
            DirectoryEntryDisplayHex(sender.tag)
        End If
    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles BtnExit.Click
        If CloseAll() Then
            Me.Close()
        End If
    End Sub

    Private Sub BtnOpen_Click(sender As Object, e As EventArgs) Handles BtnOpen.Click
        FilesOpen()
    End Sub

    Private Sub BtnRevert_Click(sender As Object, e As EventArgs) Handles BtnRevert.Click
        RevertChanges()
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        SaveCurrent(False)
    End Sub

    Private Sub BtnSaveAll_Click(sender As Object, e As EventArgs) Handles BtnSaveAll.Click
        SaveAll()
    End Sub

    Private Sub BtnSaveAs_Click(sender As Object, e As EventArgs) Handles BtnSaveAs.Click
        SaveCurrent(True)
    End Sub

    Private Sub BtnScan_Click(sender As Object, e As EventArgs) Handles BtnScan.Click
        DiskImagesScan(False)
    End Sub

    Private Sub BtnScanNew_Click(sender As Object, e As EventArgs) Handles BtnScanNew.Click
        DiskImagesScan(True)
    End Sub

    Private Sub ComboGroups_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboGroups.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        DiskImageProcess(ComboGroups.SelectedItem)
    End Sub

    Private Sub ContextMenuFilters_CheckStateChanged(sender As Object, e As EventArgs)
        If _SuppressEvent Then
            Exit Sub
        End If

        FiltersApply()
    End Sub

    Private Sub ContextMenuFilters_Closing(sender As Object, e As ToolStripDropDownClosingEventArgs) Handles ContextMenuFilters.Closing
        If e.CloseReason = ToolStripDropDownCloseReason.ItemClicked Then
            e.Cancel = True
        End If
    End Sub

    Private Sub File_DragDrop(sender As Object, e As DragEventArgs) Handles ComboGroups.DragDrop, LabelDropMessage.DragDrop, ListViewFiles.DragDrop, ListViewHashes.DragDrop, ListViewSummary.DragDrop
        Dim Files As String() = e.Data.GetData(DataFormats.FileDrop)
        ProcessFileDrop(Files, False)
    End Sub

    Private Sub File_DragEnter(sender As Object, e As DragEventArgs) Handles ComboGroups.DragEnter, LabelDropMessage.DragEnter, ListViewFiles.DragEnter, ListViewHashes.DragEnter, ListViewSummary.DragEnter
        FileDropStart(e)
    End Sub

    Private Sub ListViewFiles_ColumnWidthChanging(sender As Object, e As ColumnWidthChangingEventArgs) Handles ListViewFiles.ColumnWidthChanging
        e.NewWidth = Me.ListViewFiles.Columns(e.ColumnIndex).Width
        e.Cancel = True
    End Sub

    Private Sub ListViewFiles_DrawItem(sender As Object, e As DrawListViewItemEventArgs) Handles ListViewFiles.DrawItem
        e.DrawDefault = True
    End Sub

    Private Sub ListViewFiles_DrawColumnHeader(sender As Object, e As DrawListViewColumnHeaderEventArgs) Handles ListViewFiles.DrawColumnHeader
        If e.ColumnIndex = 0 Then
            'Dim Offset As Integer
            'If (e.State And ListViewItemStates.Selected) > 0 Then
            ' Offset = 1
            ' Else
            '    Offset = 0
            'End If
            Dim State = IIf(_CheckAll, VisualStyles.CheckBoxState.CheckedNormal, VisualStyles.CheckBoxState.UncheckedNormal)
            Dim Size = CheckBoxRenderer.GetGlyphSize(e.Graphics, State)
            CheckBoxRenderer.DrawCheckBox(e.Graphics, New Point(4, (e.Bounds.Height - Size.Height) \ 2), State)
            'e.Graphics.DrawString(e.Header.Text, e.Font, New SolidBrush(Color.Black), New Point(20 + Offset, (e.Bounds.Height - Size.Height) \ 2 + Offset))
            'e.Graphics.DrawLine(New Pen(Color.FromArgb(229, 229, 229), 1), New Point(e.Bounds.Width - 1, 0), New Point(e.Bounds.Width - 1, e.Bounds.Height))
        Else
            e.DrawDefault = True
        End If
    End Sub

    Private Sub ListViewFiles_DrawSubItem(sender As Object, e As DrawListViewSubItemEventArgs) Handles ListViewFiles.DrawSubItem
        e.DrawDefault = True
    End Sub

    Private Sub ListViewFiles_ItemDrag(sender As Object, e As ItemDragEventArgs) Handles ListViewFiles.ItemDrag
        _SuppressEvent = True
        DragDropSelectedFiles()
        _SuppressEvent = False
    End Sub

    Private Sub ListViewFiles_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles ListViewFiles.ColumnClick
        If e.Column = 0 Then
            If ListViewFiles.Items.Count > 0 Then
                _CheckAll = Not _CheckAll
                _SuppressEvent = True
                For Each Item As ListViewItem In ListViewFiles.Items
                    Item.Selected = _CheckAll
                Next
                _SuppressEvent = False
                ItemSelectionChanged()
            End If
        End If
    End Sub

    Private Sub MainForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If Not CloseAll() Then
            e.Cancel = True
        End If
    End Sub

    Private Sub ListViewFiles_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles ListViewFiles.ItemSelectionChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        ItemSelectionChanged()
    End Sub

    Private Sub BtnFileProperties_Click(sender As Object, e As EventArgs) Handles BtnFileProperties.Click, BtnFileMenuFileProperties.Click
        FilePropertiesEdit()
    End Sub

    Private Sub ContextMenuFiles_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuFiles.Opening
        If ListViewFiles.SelectedItems.Count = 0 Then
            e.Cancel = True
        Else
            Dim Caption As String = "&View "
            If ListViewFiles.SelectedItems.Count = 1 Then
                Dim Item As ListViewItem = ListViewFiles.SelectedItems(0)
                Dim FileData As FileData = Item.Tag
                Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(FileData.Offset)
                If DirectoryEntry.IsVolumeName Or DirectoryEntry.HasInvalidFileSize Or DirectoryEntry.StartingCluster < 2 Then
                    BtnFileMenuViewFile.Enabled = False
                    BtnFileMenuViewFileText.Visible = False
                    BtnFileMenuReplaceFile.Enabled = False
                    Caption &= "File"
                Else
                    BtnFileMenuViewFile.Enabled = True
                    If DirectoryEntry.IsDeleted Then
                        Caption &= "Deleted "
                    End If
                    If DirectoryEntry.IsDirectory Then
                        Caption &= "Directory"
                        BtnFileMenuViewFileText.Visible = False
                        BtnFileMenuReplaceFile.Enabled = False
                    Else
                        Caption &= "File"
                        BtnFileMenuViewFileText.Visible = Not DirectoryEntry.IsDeleted
                        BtnFileMenuViewFileText.Enabled = True
                        BtnFileMenuReplaceFile.Enabled = Not DirectoryEntry.IsDeleted
                    End If
                End If
                BtnFileMenuUndo.Enabled = DirectoryEntry.IsModified
            Else
                BtnFileMenuViewFile.Enabled = False
                BtnFileMenuViewFileText.Visible = True
                BtnFileMenuViewFileText.Enabled = False
                Caption &= "File"
                BtnFileMenuUndo.Enabled = False
                For Each Item As ListViewItem In ListViewFiles.SelectedItems
                    Dim FileData As FileData = Item.Tag
                    Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(FileData.Offset)
                    If DirectoryEntry.IsModified Then
                        BtnFileMenuUndo.Enabled = True
                        Exit For
                    End If
                Next
                BtnFileMenuReplaceFile.Enabled = False
            End If
            BtnFileMenuViewFile.Text = Caption
        End If
    End Sub

    Private Sub BtnFileMenuViewFile_Click(sender As Object, e As EventArgs) Handles BtnFileMenuViewFile.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim Item As ListViewItem = ListViewFiles.SelectedItems(0)
            Dim FileData As FileData = Item.Tag
            DirectoryEntryDisplayHex(FileData.Offset)
        End If
    End Sub

    Private Sub BtnFileMenuViewFileText_Click(sender As Object, e As EventArgs) Handles BtnFileMenuViewFileText.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim Item As ListViewItem = ListViewFiles.SelectedItems(0)
            Dim FileData As FileData = Item.Tag
            DirectoryEntryDisplayText(FileData.Offset)
        End If
    End Sub

    Private Sub BtnFileMenuUndo_Click(sender As Object, e As EventArgs) Handles BtnFileMenuUndo.Click
        Dim Removed As Boolean = False
        For Each Item As ListViewItem In ListViewFiles.SelectedItems
            Dim FileData As FileData = Item.Tag
            Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(FileData.Offset)
            If DirectoryEntry.IsModified Then
                DirectoryEntry.RemoveModification()
                Removed = True
            End If
        Next
        If Removed Then
            ItemScanAll(_Disk, ComboGroups.SelectedItem, True)
            ComboItemRefresh(True)
        End If
    End Sub

    Private Sub BtnReplaceFile_Click(sender As Object, e As EventArgs) Handles BtnReplaceFile.Click, BtnFileMenuReplaceFile.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim Item As ListViewItem = ListViewFiles.SelectedItems(0)
            Dim FileData As FileData = Item.Tag
            FileReplace(FileData.Offset)
        End If
    End Sub

#End Region

End Class

Public Structure ProcessDirectoryEntryResponse
    Dim HasCreated As Boolean
    Dim HasLastAccessed As Boolean
    Dim HasLFN As Boolean
    Dim HasInvalidDirectoryEntries As Boolean
End Structure

Public Structure FileData
    Dim FilePath As String
    Dim Offset As UInteger
End Structure

