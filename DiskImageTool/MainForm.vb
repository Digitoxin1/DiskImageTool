Imports System.ComponentModel
Imports System.Security.Cryptography
Imports System.Text

Public Class MainForm
    Private ReadOnly _FileHashTable As Dictionary(Of UInteger, FileData)
    Private ReadOnly _DirectoryHashTable As Dictionary(Of UInteger, DirectoryData)
    Private ReadOnly _LoadedImageList As List(Of LoadedImageData)
    Private ReadOnly _OEMIDDictionary As Dictionary(Of UInteger, OEMIDList)
    Private _Disk As DiskImage.Disk
    Private _VolumeLabel As String = ""
    Private _InternalDrop As Boolean = False
    Private _SuppressEvent As Boolean = False
    Private _FiltersApplied As Boolean = False
    Private _ModifiedCount As Integer = 0

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _FileHashTable = New Dictionary(Of UInteger, FileData)
        _DirectoryHashTable = New Dictionary(Of UInteger, DirectoryData)
        _LoadedImageList = New List(Of LoadedImageData)
        _OEMIDDictionary = GetOEMIDDictionary()
        BtnOEMID.Enabled = False
        BtnDisplayBootSector.Enabled = False
        BtnDisplayDirectory.Enabled = False
        BtnClearCreated.Enabled = False
        BtnDisplayClusters.Enabled = False
        BtnClearLastAccessed.Enabled = False
        BtnScan.Enabled = False
        BtnRevert.Enabled = False
        CBCheckAll.Visible = False
        ToolStripFileCount.Visible = False
        ToolStripFileName.Visible = False
        ToolStripModified.Visible = False
        RefreshSaveButton(False)

        ListViewDoubleBuffer(ListViewFiles)
    End Sub

    Private Sub ApplyFilters(e As ToolStripItemClickedEventArgs)
        Dim ClickedItem As ToolStripMenuItem = Nothing
        Dim CurrentChecked As Boolean = False
        Dim Checked As Boolean

        Dim Count As Integer = 0
        For Counter = 2 To ContextMenuFilters.Items.Count - 1
            Dim Item As ToolStripMenuItem = ContextMenuFilters.Items(Counter)
            If Item.CheckState = CheckState.Checked Then
                Count += 1
            End If
        Next

        If e IsNot Nothing Then
            ClickedItem = e.ClickedItem
            If ClickedItem.CheckState = CheckState.Checked Then
                Count -= 1
                CurrentChecked = False
            Else
                Count += 1
                CurrentChecked = True
            End If
        End If

        If Count > 0 Then
            Me.UseWaitCursor = True

            Dim SelectedItem As LoadedImageData = ComboGroups.SelectedItem
            ComboGroups.Items.Clear()
            Dim AppliedFilters As FilterTypes = 0
            For Counter = 2 To ContextMenuFilters.Items.Count - 1
                Dim Item As ToolStripMenuItem = ContextMenuFilters.Items(Counter)
                If Item Is ClickedItem Then
                    Checked = CurrentChecked
                Else
                    Checked = (Item.CheckState = CheckState.Checked)
                End If
                If Checked Then
                    AppliedFilters += Item.Name
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

            MenuStripMain.Items("FilterToolStripMenuItem").BackColor = Color.LightGreen

            ToolStripFileCount.Text = ComboGroups.Items.Count & " of " & _LoadedImageList.Count & " File" & IIf(_LoadedImageList.Count <> 1, "s", "")

            _FiltersApplied = True

            Me.UseWaitCursor = False
        Else
            If _FiltersApplied Then
                ClearFilters()
            End If
        End If
    End Sub

    Private Sub ClearFilters()
        Me.UseWaitCursor = True

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

        ResetFilterButton()

        Me.UseWaitCursor = False
    End Sub

    Private Sub ResetFilters()
        For Counter = ContextMenuFilters.Items.Count - 1 To 2 Step -1
            Dim Item As ToolStripMenuItem = ContextMenuFilters.Items(Counter)
            If Item.CheckOnClick Then
                ContextMenuFilters.Items.RemoveAt(Counter)
            End If
        Next
        FilterSeparator.Visible = False
    End Sub

    Private Sub ResetFilterButton()
        _FiltersApplied = False

        MenuStripMain.Items("FilterToolStripMenuItem").BackColor = SystemColors.Control
        ToolStripFileCount.Text = _LoadedImageList.Count & " File" & IIf(_LoadedImageList.Count <> 1, "s", "")
        ToolStripFileCount.Visible = True
    End Sub

    Private Function ChangeOEMID() As Boolean
        Dim frmOEMID As New OEMIDForm(_Disk, _OEMIDDictionary)
        Dim Result As Boolean

        frmOEMID.ShowDialog()

        Result = frmOEMID.Result

        If Result Then
            ComboItemSetModified(True, False)
        End If

        Return Result
    End Function

    Private Sub ComboItemSetModified(Modified As Boolean, FullRefresh As Boolean)
        Dim ImageData As LoadedImageData = ComboGroups.SelectedItem
        If ImageData.Modified <> Modified Then
            ImageData.Modified = Modified
            _ModifiedCount += IIf(Modified, 1, -1)
            ToolStripModified.Text = _ModifiedCount & " File" & IIf(_ModifiedCount <> 1, "s", "") & " Modified"
            ToolStripModified.Visible = (_ModifiedCount > 0)
            RefreshModifiedFilter()
        End If
        If Modified Then
            ImageData.Modifications = _Disk.Modifications
        Else
            ImageData.Modifications = Nothing
        End If
        _SuppressEvent = True
        ComboGroups.Items(ComboGroups.SelectedIndex) = ComboGroups.Items(ComboGroups.SelectedIndex)
        _SuppressEvent = False
        If FullRefresh Then
            PopulateDetails()
        End If
        PopulateSummary()
        RefreshSaveButton(Modified)
    End Sub

    Private Function ClearCreatedDate() As Boolean
        Dim Result As Boolean = False

        For Each Item As ListViewItem In ListViewFiles.CheckedItems
            Dim FileData = _FileHashTable(Item.GetHashCode)
            If FileData.HasCreated Then
                Dim File = _Disk.GetDirectoryEntryByOffset(FileData.Offset)
                File.ClearCreationDate()
                Item.SubItems.Item("FileCreateDate").Text = ""
                Result = True
            End If
        Next

        If Result Then
            ComboItemSetModified(True, False)
        End If

        Return Result
    End Function

    Private Function ClearLastAccessedDate() As Boolean
        Dim Result As Boolean = False

        For Each Item As ListViewItem In ListViewFiles.CheckedItems
            Dim FileData = _FileHashTable(Item.GetHashCode)
            If FileData.HasLastAccessed Then
                Dim File = _Disk.GetDirectoryEntryByOffset(FileData.Offset)
                File.ClearLastAccessDate()
                Item.SubItems.Item("FileLastAccessDate").Text = ""
                Result = True
            End If
        Next

        If Result Then
            ComboItemSetModified(True, False)
        End If

        Return Result
    End Function

    Private Sub DragDropSelectedFiles()
        If ListViewFiles.SelectedItems.Count = 0 Then
            Exit Sub
        End If

        Dim TempPath As String = System.IO.Path.GetTempPath() & Guid.NewGuid().ToString() & "\"
        For Each Item As ListViewItem In ListViewFiles.SelectedItems
            Dim FileData = _FileHashTable(Item.GetHashCode)
            Dim File = _Disk.GetDirectoryEntryByOffset(FileData.Offset)
            If Not File.IsDeleted And Not File.IsDirectory And Not File.IsVolumeName And Not File.HasInvalidFileSize And Not File.HasInvalidFilename And Not File.HasInvalidExtension Then
                Dim FilePath = System.IO.Path.Combine(TempPath, FileData.FilePath, File.GetFileName)
                If Not System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(FilePath)) Then
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(FilePath))
                End If
                System.IO.File.WriteAllBytes(FilePath, File.GetContent)
                Dim D = File.GetLastWriteDate
                If D.IsValidDate Then
                    System.IO.File.SetLastWriteTime(FilePath, D.DateObject)
                End If
            End If
        Next

        If System.IO.Directory.Exists(TempPath) Then
            Dim FileList = System.IO.Directory.EnumerateDirectories(TempPath)
            For Each FilePath In System.IO.Directory.GetFiles(TempPath)
                FileList = FileList.Append(FilePath)
            Next
            If FileList.Count > 0 Then
                Dim Data = New DataObject(DataFormats.FileDrop, FileList.ToArray)
                ListViewFiles.DoDragDrop(Data, DragDropEffects.Copy)
            End If
            System.IO.Directory.Delete(TempPath, True)
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

    Private Function GetListViewFileItem(File As DiskImage.DirectoryEntry, Group As ListViewGroup, LFNFileName As String) As ListViewItem
        Dim SI As ListViewItem.ListViewSubItem

        Dim Attrib As String = IIf(File.IsArchive, "A ", "- ") _
            & IIf(File.IsReadOnly, "R ", "- ") _
            & IIf(File.IsSystem, "S ", "- ") _
            & IIf(File.IsHidden, "H ", "- ") _
            & IIf(File.IsDirectory, "D ", "- ") _
            & IIf(File.IsVolumeName, "V ", "- ")

        Dim Item = New ListViewItem("", Group) With {
            .UseItemStyleForSubItems = False
        }

        If File.IsDeleted Then
            Item.ForeColor = Color.Gray
        ElseIf File.IsVolumeName Then
            Item.ForeColor = Color.Green
        ElseIf File.IsDirectory Then
            Item.ForeColor = Color.Blue
        End If

        SI = Item.SubItems.Add(Encoding.UTF8.GetString(File.FileName).Trim)
        If Not File.IsDeleted And File.HasInvalidFilename Then
            SI.ForeColor = Color.Red
        Else
            SI.ForeColor = Item.ForeColor
        End If

        SI = Item.SubItems.Add(Encoding.UTF8.GetString(File.Extension).Trim)
        If Not File.IsDeleted And File.HasInvalidExtension Then
            SI.ForeColor = Color.Red
        Else
            SI.ForeColor = Item.ForeColor
        End If

        If Not File.IsDeleted And File.HasInvalidFileSize Then
            SI = Item.SubItems.Add("Invalid")
            SI.ForeColor = Color.Red
        Else
            SI = Item.SubItems.Add(Format(File.FileSize, "N0"))
            SI.ForeColor = Item.ForeColor
        End If

        SI = Item.SubItems.Add(ExpandedDateToString(File.GetLastWriteDate, True, False))
        If File.GetLastWriteDate.IsValidDate Or File.IsDeleted Then
            SI.ForeColor = Item.ForeColor
        Else
            SI.ForeColor = Color.Red
        End If

        SI = Item.SubItems.Add(Format(File.StartingCluster, "N0"))
        SI.ForeColor = Item.ForeColor

        SI = Item.SubItems.Add(Attrib)
        If Not File.IsDeleted And File.HasInvalidAttributes Then
            SI.ForeColor = Color.Red
        Else
            SI.ForeColor = Item.ForeColor
        End If

        If Not File.IsDeleted And Not File.IsDirectory And Not File.IsVolumeName And Not File.HasInvalidFileSize Then
            SI = Item.SubItems.Add(Crc32.ComputeChecksum(File.GetContent).ToString("X8"))
        Else
            SI = Item.SubItems.Add("")
        End If
        SI.ForeColor = Item.ForeColor

        If File.HasCreationDate Then
            SI = Item.SubItems.Add(ExpandedDateToString(File.GetCreationDate, True, True))
            If File.GetCreationDate.IsValidDate Or File.IsDeleted Then
                SI.ForeColor = Item.ForeColor
            Else
                SI.ForeColor = Color.Red
            End If
        Else
            SI = Item.SubItems.Add("")
        End If
        SI.Name = "FileCreateDate"

        If File.HasLastAccessDate Then
            SI = Item.SubItems.Add(ExpandedDateToString(File.GetLastAccessDate, False, False))
            If File.GetLastAccessDate.IsValidDate Or File.IsDeleted Then
                SI.ForeColor = Item.ForeColor
            Else
                SI.ForeColor = Color.Red
            End If
        Else
            SI = Item.SubItems.Add("")
        End If
        SI.Name = "FileLastAccessDate"

        SI = Item.SubItems.Add(LFNFileName)
        SI.ForeColor = Item.ForeColor
        SI.Name = "FileLFN"

        Return Item
    End Function

    Private Function GetListViewSummaryItem(Name As String, Value As String) As ListViewItem
        Dim Item = New ListViewItem(Name) With {
                .UseItemStyleForSubItems = False
            }
        Item.SubItems.Add(Value)

        Return Item
    End Function

    Private Function GetListViewSummaryItem(Name As String, Value As String, ForeColor As Color) As ListViewItem
        Dim Item = New ListViewItem(Name) With {
                .UseItemStyleForSubItems = False
            }
        Dim SubItem = Item.SubItems.Add(Value)
        SubItem.ForeColor = ForeColor

        Return Item
    End Function

    Private Function GetMediaType(MediaDescriptor As Byte, SectorsPerTrack As UShort) As String
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

    Private Sub ListViewDoubleBuffer(lv As ListView)
        lv.GetType() _
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic) _
            .SetValue(lv, True, Nothing)
    End Sub

    Private Sub RefreshButtonState()
        If _Disk.IsValidImage Then
            BtnOEMID.Enabled = True
            BtnDisplayBootSector.Enabled = True
            BtnDisplayDirectory.Enabled = True
            CBCheckAll.Visible = True
        Else
            BtnOEMID.Enabled = False
            BtnDisplayBootSector.Enabled = False
            BtnDisplayDirectory.Enabled = False
            CBCheckAll.Visible = False
        End If
        BtnClearCreated.Enabled = False
        BtnClearLastAccessed.Enabled = False
        ClearDisplayDirectorySubMenu()
    End Sub

    Private Sub InitializeColumns()
        If Not ListViewFiles.Columns.ContainsKey("FileCreateDate") Then
            ListViewAddColumn("FileCreateDate", "Created", 0, 8)
        End If
        If Not ListViewFiles.Columns.ContainsKey("FileLastAccessDate") Then
            ListViewAddColumn("FileLastAccessDate", "Last Accessed", 0, 9)
        End If
        If Not ListViewFiles.Columns.ContainsKey("FileLFN") Then
            ListViewAddColumn("FileLFN", "Long File Name", 0, 10)
        End If
    End Sub
    Private Sub ListViewAddColumn(Name As String, Text As String, Width As Integer, Index As Integer)
        Dim Column As New ColumnHeader With {
            .Name = Name,
            .Width = Width,
            .Text = Text
        }
        ListViewFiles.Columns.Insert(Index, Column)
    End Sub

    Private Function MD5Hash(Data() As Byte) As String
        Using hasher As MD5 = MD5.Create()
            Dim dbytes As Byte() = hasher.ComputeHash(Data)
            Dim sBuilder As New StringBuilder()

            For n As Integer = 0 To dbytes.Length - 1
                sBuilder.Append(dbytes(n).ToString("X2"))
            Next n

            Return sBuilder.ToString
        End Using
    End Function

    Private Function SHA1Hash(Data() As Byte) As String
        Using hasher As SHA1 = SHA1.Create()
            Dim dbytes As Byte() = hasher.ComputeHash(Data)
            Dim sBuilder As New StringBuilder()

            For n As Integer = 0 To dbytes.Length - 1
                sBuilder.Append(dbytes(n).ToString("X2"))
            Next n

            Return sBuilder.ToString
        End Using
    End Function

    Private Sub PopulateDetails()
        ListViewFiles.BeginUpdate()
        ListViewFiles.Items.Clear()

        Dim Items As New List(Of ListViewItem)
        Dim Response As ProcessFilesResponse = ProcessFiles(_Disk.Directory, "", False)

        If BtnDisplayDirectory.DropDownItems.Count > 0 Then
            BtnDisplayDirectory.Text = "Directory"
            AddDisplayDirectorySubMenuItem("(Root)", BtnDisplayDirectory.Tag, 0)
            BtnDisplayDirectory.Tag = Nothing

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

        _VolumeLabel = Response.VolumeLabel
        CBCheckAll.Checked = False
    End Sub

    Private Function FindOEMMatch(Checksum As UInteger) As OEMIDList
        If _OEMIDDictionary.ContainsKey(Checksum) Then
            Return _OEMIDDictionary.Item(Checksum)
        Else
            Return Nothing
        End If
    End Function

    Private Sub PopulateSummary()
        Dim BootstrapChecksum = Crc32.ComputeChecksum(_Disk.BootSector.BootStrapCode)
        Dim ForeColor As Color
        Dim OEMIDString As String = Encoding.UTF8.GetString(_Disk.BootSector.OEMID)
        Dim OEMIDMatched As Boolean = False

        Me.Text = "Disk Image Tool - " & System.IO.Path.GetFileName(_Disk.FilePath)
        ToolStripFileName.Text = System.IO.Path.GetFileName(_Disk.FilePath)
        ToolStripFileName.Visible = True

        Dim BootstrapType = FindOEMMatch(BootstrapChecksum)
        If BootstrapType IsNot Nothing Then
            OEMIDMatched = BootstrapType.OEMIDList.Contains(OEMIDString)
        End If

        With ListViewSummary.Items
            .Clear()
            .Add(GetListViewSummaryItem("Modified:", IIf(_Disk.Modified, "Yes", "No"), IIf(_Disk.Modified, Color.Blue, SystemColors.WindowText)))
            If BootstrapType IsNot Nothing Then
                If Not OEMIDMatched Then
                    ForeColor = Color.Red
                Else
                    ForeColor = Color.Green
                End If
            Else
                ForeColor = SystemColors.WindowText
            End If
            .Add(GetListViewSummaryItem("OEM ID:", OEMIDString, ForeColor))
            If BootstrapType IsNot Nothing Then
                .Add(GetListViewSummaryItem("Language:", _OEMIDDictionary.Item(BootstrapChecksum).Language))
            End If
            .Add(GetListViewSummaryItem("Media Type:", GetMediaType(_Disk.BootSector.MediaDescriptor, _Disk.BootSector.SectorsPerTrack)))

            If _VolumeLabel <> "" Then
                .Add(GetListViewSummaryItem("Volume Label:", _VolumeLabel))
            End If
            If _Disk.BootSector.ExtendedBootSignature = &H29 Then
                .Add(GetListViewSummaryItem("Volume Serial Number:", _Disk.BootSector.VolumeSerialNumber.ToString("X8").Insert(4, "-")))
                .Add(GetListViewSummaryItem("File System Type:", Encoding.UTF8.GetString(_Disk.BootSector.FileSystemType)))
            End If
            .Add(GetListViewSummaryItem("Bytes Per Sector:", _Disk.BootSector.BytesPerSector))
            .Add(GetListViewSummaryItem("Sectors Per Cluster:", _Disk.BootSector.SectorsPerCluster))
            .Add(GetListViewSummaryItem("Sectors Per Track:", _Disk.BootSector.SectorsPerTrack))
            .Add(GetListViewSummaryItem("Free Space:", Format(_Disk.FreeSpace, "N0") & " bytes"))
            If BootstrapType IsNot Nothing Then
                If Not OEMIDMatched Then
                    For Each OEMID In BootstrapType.OEMIDList
                        .Add(GetListViewSummaryItem("Detected OEM ID:", OEMID))
                    Next
                End If
            End If
        End With
        With ListViewHashes.Items
            .Clear()
            .Add(GetListViewSummaryItem("CRC32", Crc32.ComputeChecksum(_Disk.Data).ToString("X8")))
            .Add(GetListViewSummaryItem("MD5", MD5Hash(_Disk.Data)))
            .Add(GetListViewSummaryItem("SHA-1", SHA1Hash(_Disk.Data)))
        End With

        BtnDisplayClusters.Enabled = _Disk.HasUnusedClustersWithData
        BtnRevert.Enabled = _Disk.Modified
    End Sub
    Private Sub ProcessFileDrop(Files() As String)
        Dim AllowedExtensions = {".img", ".ima"}
        Dim FilePath As String
        Dim FileInfo As System.IO.FileInfo
        Dim PathName As String
        Dim FileName As String
        Dim ComboCleared As Boolean = False

        Me.UseWaitCursor = True

        For Each FilePath In Files
            Dim FAttributes = System.IO.File.GetAttributes(FilePath)
            If (FAttributes And System.IO.FileAttributes.Directory) > 0 Then
                Dim DirectoryInfo As New System.IO.DirectoryInfo(FilePath)
                For Each FileInfo In DirectoryInfo.GetFiles("*.im*", System.IO.SearchOption.AllDirectories)
                    If AllowedExtensions.Contains(FileInfo.Extension.ToLower) Then
                        PathName = System.IO.Path.GetDirectoryName(FilePath)
                        If Not PathName.EndsWith("\") Then
                            PathName &= "\"
                        End If
                        FileName = Strings.Right(FileInfo.FullName, Len(FileInfo.FullName) - Len(PathName))
                        If Not ComboCleared Then
                            ComboGroups.Items.Clear()
                            _LoadedImageList.Clear()
                            ComboCleared = True
                        End If
                        Dim ImageData As New LoadedImageData(PathName, FileName)
                        ImageData.ComboIndex = ComboGroups.Items.Add(ImageData)
                        _LoadedImageList.Add(ImageData)
                    End If
                Next
            Else
                Dim Ext As String = UCase(System.IO.Path.GetExtension(FilePath))
                If Ext = ".IMA" Or Ext = ".IMG" Then
                    PathName = System.IO.Path.GetDirectoryName(FilePath)
                    FileName = System.IO.Path.GetFileName(FilePath)
                    If Not ComboCleared Then
                        ComboGroups.Items.Clear()
                        _LoadedImageList.Clear()
                        ComboCleared = True
                    End If
                    Dim ImageData As New LoadedImageData(PathName, FileName)
                    ImageData.ComboIndex = ComboGroups.Items.Add(ImageData)
                    _LoadedImageList.Add(ImageData)
                End If
            End If
        Next

        If ComboCleared And ComboGroups.Items.Count > 0 Then
            ComboGroups.SelectedIndex = 0
            LabelDropMessage.Visible = False
            BtnScan.Enabled = True
            ResetFilters()
            ResetFilterButton()
            ToolStripModified.Visible = False
            _ModifiedCount = 0
        End If

        RefreshSaveButton(False)

        Me.UseWaitCursor = False
    End Sub
    Private Function GetFileDataFromFile(File As DiskImage.DirectoryEntry, FilePath As String) As FileData
        Dim Response As FileData
        With Response
            .FilePath = FilePath
            .Offset = File.Offset
            .HasCreated = File.HasCreationDate
            .HasLastAccessed = File.HasLastAccessDate
        End With

        Return Response
    End Function

    Private Sub OpenFiles()
        Dim Dialog = New OpenFileDialog With {
            .Filter = "Disk Image Files (*.ima; *.img)|*.ima;*.img",
            .Multiselect = True
        }
        If Dialog.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If

        ProcessFileDrop(Dialog.FileNames)
    End Sub

    Private Sub ClearDisplayDirectorySubMenu()
        For Each Item As ToolStripMenuItem In BtnDisplayDirectory.DropDownItems
            RemoveHandler Item.Click, AddressOf BtnDisplayDirectory_Click
        Next
        BtnDisplayDirectory.DropDownItems.Clear()
        BtnDisplayDirectory.Text = "Root Directory"
    End Sub

    Private Sub AddDisplayDirectorySubMenuItem(Path As String, HashCode As Integer, Index As Integer)
        Dim Item As New ToolStripMenuItem With {
            .Text = Path,
            .Tag = HashCode
        }
        If Index = -1 Then
            BtnDisplayDirectory.DropDownItems.Add(Item)
        Else
            BtnDisplayDirectory.DropDownItems.Insert(Index, Item)
        End If
        AddHandler Item.Click, AddressOf BtnDisplayDirectory_Click
    End Sub

    Private Function ProcessFiles(Directory As DiskImage.Directory, Path As String, ScanOnly As Boolean) As ProcessFilesResponse
        Dim Group As ListViewGroup = Nothing
        Dim Counter As UInteger
        Dim FileCount As UInteger = Directory.FileCount
        Dim LFNFileName As String = ""

        Dim Response As ProcessFilesResponse
        With Response
            .VolumeLabel = ""
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

            Dim DirectoryData As DirectoryData
            DirectoryData.Path = Path
            DirectoryData.Directory = Directory
            _DirectoryHashTable.Add(Group.GetHashCode, DirectoryData)
            If Path = "" Then
                BtnDisplayDirectory.Tag = Group.GetHashCode
            Else
                AddDisplayDirectorySubMenuItem(Path, Group.GetHashCode, -1)
            End If
        End If

        For Counter = 1 To Directory.DirectoryLength
            Dim File = Directory.GetFile(Counter)
            Dim FullFileName = File.GetFileName
            Dim FileData = GetFileDataFromFile(File, Path)

            If FullFileName <> "." And FullFileName <> ".." Then
                If Path = "" And Response.VolumeLabel = "" Then
                    If Not File.IsDeleted And File.IsVolumeName Then
                        Response.VolumeLabel = Encoding.UTF8.GetString(File.FileName) & Encoding.UTF8.GetString(File.Extension)
                    End If
                End If
                If File.IsLFN Then
                    LFNFileName = File.GetLFNFileName & LFNFileName
                Else
                    If Not ScanOnly Then
                        Dim item = GetListViewFileItem(File, Group, LFNFileName)
                        _FileHashTable.Add(item.GetHashCode, FileData)
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
                        Dim SubResponse = ProcessFiles(File.SubDirectory, NewPath, ScanOnly)
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
    Private Sub ProcessImage(Item As LoadedImageData)
        _FileHashTable.Clear()
        _DirectoryHashTable.Clear()
        InitializeColumns()

        Dim FilePath As String = System.IO.Path.Combine(Item.Path, Item.File)
        _Disk = New DiskImage.Disk(FilePath)
        If Item.Modified Then
            _Disk.ApplyModifications(Item.Modifications)
        End If

        RefreshButtonState()
        RefreshSaveButton(_Disk.Modified)

        If _Disk.IsValidImage Then
            PopulateDetails()
            PopulateSummary()
            LblInvalidImage.Visible = False
        Else
            _VolumeLabel = ""
            ListViewSummary.Items.Clear()
            ListViewFiles.Items.Clear()
            LblInvalidImage.Visible = True
        End If
    End Sub
    Private Sub RefreshButtons()
        Dim CreatedEnabled As Boolean = False
        Dim LastAccessEnabled As Boolean = False

        For Each Item As ListViewItem In ListViewFiles.CheckedItems
            Dim FileData = _FileHashTable(Item.GetHashCode)
            If FileData.HasCreated Then
                CreatedEnabled = True
            End If
            If FileData.HasLastAccessed Then
                LastAccessEnabled = True
            End If
            If CreatedEnabled And LastAccessEnabled Then
                Exit For
            End If
        Next
        BtnClearCreated.Enabled = CreatedEnabled
        BtnClearLastAccessed.Enabled = LastAccessEnabled
    End Sub

    Private Sub RevertChanges()
        If _Disk.Modified Then
            _Disk.RevertChanges()
        End If

        ComboItemSetModified(False, True)
    End Sub

    Private Sub SaveDiskToFile(Disk As DiskImage.Disk, FilePath As String)
        If System.IO.File.Exists(FilePath) Then
            Dim BackupPath As String = FilePath & ".bak"
            System.IO.File.Copy(FilePath, BackupPath, True)
        End If
        Disk.SaveFile(FilePath)
    End Sub

    Private Sub SaveCurrent(NewFileName As Boolean)
        Dim FilePath As String

        If NewFileName Then
            Dim Dialog = New SaveFileDialog With {
                .InitialDirectory = System.IO.Path.GetDirectoryName(_Disk.FilePath),
                .FileName = System.IO.Path.GetFileName(_Disk.FilePath),
                .Filter = "Disk Image Files (*.ima; *.img)|*.ima;*.img"
            }
            If Dialog.ShowDialog <> DialogResult.OK Then
                Exit Sub
            End If
            FilePath = Dialog.FileName
        Else
            FilePath = _Disk.FilePath
        End If

        SaveDiskToFile(_Disk, FilePath)

        ComboItemSetModified(False, False)
    End Sub

    Private Sub SaveAll()
        _SuppressEvent = True
        For Each ImageData In _LoadedImageList
            If ImageData.Modified Then
                Dim FilePath As String = System.IO.Path.Combine(ImageData.Path, ImageData.File)
                Dim Disk = New DiskImage.Disk(FilePath)

                Disk.ApplyModifications(ImageData.Modifications)
                SaveDiskToFile(Disk, Disk.FilePath)

                ImageData.Modified = False
                ImageData.Modifications = Nothing
                If ImageData.ComboIndex > -1 Then
                    ComboGroups.Items(ImageData.ComboIndex) = ComboGroups.Items(ImageData.ComboIndex)
                End If

                _ModifiedCount -= 1
            End If
        Next
        _SuppressEvent = False

        ToolStripModified.Text = _ModifiedCount & " File" & IIf(_ModifiedCount <> 1, "s", "") & " Modified"
        ToolStripModified.Visible = (_ModifiedCount > 0)
        RefreshModifiedFilter()
        PopulateSummary()
        RefreshSaveButton(False)
    End Sub

    Private Sub RefreshModifiedFilter()
        Dim FilterModifiedFiles As ToolStripMenuItem = Nothing
        Dim FilterApplied As Boolean = False

        For Counter = 2 To ContextMenuFilters.Items.Count - 1
            Dim Item As ToolStripMenuItem = ContextMenuFilters.Items(Counter)
            If Item.Name = FilterTypes.ModifiedFiles Then
                    FilterModifiedFiles = Item
                    Exit For
                End If
            Next
            If _ModifiedCount > 0 Then
            If FilterModifiedFiles Is Nothing Then
                FilterAdd(ContextMenuFilters, FilterTypes.ModifiedFiles, _ModifiedCount, 2)
            Else
                FilterModifiedFiles.Text = FilterGetCaption(FilterTypes.ModifiedFiles, _ModifiedCount)
            End If
        Else
            If FilterModifiedFiles IsNot Nothing Then
                FilterApplied = FilterModifiedFiles.CheckState = CheckState.Checked
                ContextMenuFilters.Items.Remove(FilterModifiedFiles)
            End If
        End If

        If FilterApplied Then
            ApplyFilters(Nothing)
        End If
    End Sub

    Private Sub RefreshSaveButton(Enabled As Boolean)
        BtnSave.Enabled = Enabled
        BtnSaveAs.Enabled = Enabled
        BtnSaveAll.Enabled = (_ModifiedCount > 0)
    End Sub

    Private Sub ScanImages()
        Dim Disk As DiskImage.Disk
        Dim MismatchedOEMIDCount As Integer = 0
        Dim UnknownOEMIDCount As Integer = 0
        Dim CreatedCount As Integer = 0
        Dim LastAccessedCount As Integer = 0
        Dim InvalidImageCount As Integer = 0
        Dim LongFileNameCount As Integer = 0
        Dim InvalidDirectoryEntryCount As Integer = 0
        Dim UnusedClusterCount As Integer = 0

        Me.UseWaitCursor = True
        Dim T = Stopwatch.StartNew

        BtnScan.Enabled = False
        ResetFilters()
        If _FiltersApplied Then
            ClearFilters()
        End If

        For Counter = 0 To ComboGroups.Items.Count - 1
            Dim Percentage = Counter / ComboGroups.Items.Count * 100
            If Counter Mod 100 = 0 Then
                BtnScan.Text = "Scanning... " & Int(Percentage) & "%"
                Application.DoEvents()
            End If
            Dim Item As LoadedImageData = ComboGroups.Items(Counter)
            Dim FilePath As String = System.IO.Path.Combine(Item.Path, Item.File)
            Disk = New DiskImage.Disk(FilePath)
            If Item.Modified Then
                Disk.ApplyModifications(Item.Modifications)
            End If
            If Disk.IsValidImage Then
                Item.ScanInfo.IsValidImage = True
                Dim BootstrapChecksum = Crc32.ComputeChecksum(Disk.BootSector.BootStrapCode)
                Dim OEMIDString As String = Encoding.UTF8.GetString(Disk.BootSector.OEMID)

                Dim BootstrapType = FindOEMMatch(BootstrapChecksum)
                If BootstrapType IsNot Nothing Then
                    Item.ScanInfo.OEMIDFound = True
                    Item.ScanInfo.OEMIDMatched = BootstrapType.OEMIDList.Contains(OEMIDString)
                    If Not Item.ScanInfo.OEMIDMatched Then
                        MismatchedOEMIDCount += 1
                    End If
                Else
                    Item.ScanInfo.OEMIDFound = False
                    Item.ScanInfo.OEMIDMatched = False
                    UnknownOEMIDCount += 1
                End If

                If Disk.HasUnusedClustersWithData Then
                    Item.ScanInfo.HasUnusedClusters = True
                    UnusedClusterCount += 1
                Else
                    Item.ScanInfo.HasUnusedClusters = False
                End If

                Dim Response As ProcessFilesResponse = ProcessFiles(Disk.Directory, "", True)
                Item.ScanInfo.HasCreated = Response.HasCreated
                If Item.ScanInfo.HasCreated Then
                    CreatedCount += 1
                End If
                Item.ScanInfo.HasLastAccessed = Response.HasLastAccessed
                If Item.ScanInfo.HasLastAccessed Then
                    LastAccessedCount += 1
                End If
                Item.ScanInfo.HasLongFileNames = Response.HasLFN
                If Item.ScanInfo.HasLongFileNames Then
                    LongFileNameCount += 1
                End If
                Item.ScanInfo.HasInvalidDirectoryEntries = Response.HasInvalidDirectoryEntries
                If Item.ScanInfo.HasInvalidDirectoryEntries Then
                    InvalidDirectoryEntryCount += 1
                End If
            Else
                Item.ScanInfo.IsValidImage = False
                InvalidImageCount += 1
            End If

            Item.Scanned = True
        Next

        If _ModifiedCount > 0 Then
            FilterAdd(ContextMenuFilters, FilterTypes.ModifiedFiles, _ModifiedCount)
        End If
        If UnknownOEMIDCount > 0 Then
            FilterAdd(ContextMenuFilters, FilterTypes.UnknownOEMID, UnknownOEMIDCount)
        End If
        If MismatchedOEMIDCount > 0 Then
            FilterAdd(ContextMenuFilters, FilterTypes.MismatchedOEMID, MismatchedOEMIDCount)
        End If
        If CreatedCount > 0 Then
            FilterAdd(ContextMenuFilters, FilterTypes.HasCreated, CreatedCount)
        End If
        If LastAccessedCount > 0 Then
            FilterAdd(ContextMenuFilters, FilterTypes.HasLastAccessed, LastAccessedCount)
        End If
        If LongFileNameCount > 0 Then
            FilterAdd(ContextMenuFilters, FilterTypes.HasLongFileNames, LongFileNameCount)
        End If
        If InvalidDirectoryEntryCount > 0 Then
            FilterAdd(ContextMenuFilters, FilterTypes.HasInvalidDirectoryEntries, InvalidDirectoryEntryCount)
        End If
        If UnusedClusterCount > 0 Then
            FilterAdd(ContextMenuFilters, FilterTypes.UnusedClusters, UnusedClusterCount)
        End If
        If InvalidImageCount > 0 Then
            FilterAdd(ContextMenuFilters, FilterTypes.HasInvalidImage, InvalidImageCount)
        End If

        FilterSeparator.Visible = (ContextMenuFilters.Items.Count > 2)
        BtnScan.Text = "Scan Images"
        BtnScan.Enabled = True

        T.Stop()
        Debug.Print("ScanImages Time Taken: " & T.Elapsed.ToString)
        Me.UseWaitCursor = False
    End Sub
    Private Sub StartFileDrop(e As DragEventArgs)
        If _InternalDrop Then
            Exit Sub
        End If
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub
    Private Sub ComboGroups_DragDrop(sender As Object, e As DragEventArgs) Handles ComboGroups.DragDrop
        ProcessFileDrop(e.Data.GetData(DataFormats.FileDrop))
    End Sub
    Private Sub ComboGroups_DragEnter(sender As Object, e As DragEventArgs) Handles ComboGroups.DragEnter
        StartFileDrop(e)
    End Sub
    Private Sub ComboGroups_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboGroups.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        ProcessImage(ComboGroups.SelectedItem)
    End Sub
    Private Sub ListViewFiles_DragDrop(sender As Object, e As DragEventArgs) Handles ListViewFiles.DragDrop
        ProcessFileDrop(e.Data.GetData(DataFormats.FileDrop))
    End Sub
    Private Sub ListViewFiles_DragEnter(sender As Object, e As DragEventArgs) Handles ListViewFiles.DragEnter
        StartFileDrop(e)
    End Sub
    Private Sub ListViewSummary_DragDrop(sender As Object, e As DragEventArgs) Handles ListViewSummary.DragDrop
        ProcessFileDrop(e.Data.GetData(DataFormats.FileDrop))
    End Sub
    Private Sub ListViewSummary_DragEnter(sender As Object, e As DragEventArgs) Handles ListViewSummary.DragEnter
        StartFileDrop(e)
    End Sub

    Private Sub ListViewHashes_DragDrop(sender As Object, e As DragEventArgs) Handles ListViewHashes.DragDrop
        ProcessFileDrop(e.Data.GetData(DataFormats.FileDrop))
    End Sub

    Private Sub ListViewHashes_DragEnter(sender As Object, e As DragEventArgs) Handles ListViewHashes.DragEnter
        StartFileDrop(e)
    End Sub

    Private Sub ListViewFiles_ItemDrag(sender As Object, e As ItemDragEventArgs) Handles ListViewFiles.ItemDrag
        _InternalDrop = True
        DragDropSelectedFiles()
        _InternalDrop = False
    End Sub

    Private Sub BtnOEMID_Click(sender As Object, e As EventArgs) Handles BtnOEMID.Click
        ChangeOEMID()
    End Sub

    Private Sub LabelDropMessage_DragDrop(sender As Object, e As DragEventArgs) Handles LabelDropMessage.DragDrop
        ProcessFileDrop(e.Data.GetData(DataFormats.FileDrop))
    End Sub

    Private Sub LabelDropMessage_DragEnter(sender As Object, e As DragEventArgs) Handles LabelDropMessage.DragEnter
        StartFileDrop(e)
    End Sub

    Private Sub ListViewFiles_ColumnWidthChanging(sender As Object, e As ColumnWidthChangingEventArgs) Handles ListViewFiles.ColumnWidthChanging
        If e.ColumnIndex = 0 Then
            e.NewWidth = Me.ListViewFiles.Columns(e.ColumnIndex).Width
            e.Cancel = True
        End If
    End Sub

    Private Sub CBCheckAll_CheckedChanged(sender As Object, e As EventArgs) Handles CBCheckAll.CheckedChanged
        _SuppressEvent = True
        For Each Item As ListViewItem In ListViewFiles.Items
            Item.Checked = CBCheckAll.Checked
        Next
        _SuppressEvent = False
        RefreshButtons()
    End Sub

    Private Sub ListViewFiles_ItemChecked(sender As Object, e As ItemCheckedEventArgs) Handles ListViewFiles.ItemChecked
        If _SuppressEvent Then
            Exit Sub
        End If

        RefreshButtons()
    End Sub

    Private Sub BtnClearCreated_Click(sender As Object, e As EventArgs) Handles BtnClearCreated.Click
        Dim Msg As String = "Are you sure you wish to clear the Creation Date for all selected files?"
        If MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
            ClearCreatedDate()
            BtnClearCreated.Enabled = False
        End If
    End Sub

    Private Sub BtnClearLastAccessed_Click(sender As Object, e As EventArgs) Handles BtnClearLastAccessed.Click
        Dim Msg As String = "Are you sure you wish to clear the Last Access Date for all selected files?"
        If MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
            ClearLastAccessedDate()
            BtnClearLastAccessed.Enabled = False
        End If
    End Sub

    Private Sub BtnDisplayClusters_Click(sender As Object, e As EventArgs) Handles BtnDisplayClusters.Click
        Dim DataBlockList = _Disk.GetUnusedClustersWithData

        Dim frmHexView As New HexViewForm(_Disk, DataBlockList, True, "Unused Clusters", True)
        frmHexView.ShowDialog()
        If frmHexView.Modified Then
            ComboItemSetModified(True, False)
        End If
    End Sub

    Private Sub BtnRevert_Click(sender As Object, e As EventArgs) Handles BtnRevert.Click
        RevertChanges()
    End Sub

    Private Sub ContextMenuFilters_Closing(sender As Object, e As ToolStripDropDownClosingEventArgs) Handles ContextMenuFilters.Closing
        If e.CloseReason = ToolStripDropDownCloseReason.ItemClicked Then
            e.Cancel = True
        End If
    End Sub

    Private Sub ContextMenuFilters_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ContextMenuFilters.ItemClicked
        Dim ClickedItem As ToolStripMenuItem = e.ClickedItem
        If ClickedItem.CheckOnClick Then
            ApplyFilters(e)
        End If
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        SaveCurrent(False)
    End Sub

    Private Sub BtnSaveAs_Click(sender As Object, e As EventArgs) Handles BtnSaveAs.Click
        SaveCurrent(True)
    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles BtnExit.Click
        Me.Close()
    End Sub

    Private Sub BtnSaveAll_Click(sender As Object, e As EventArgs) Handles BtnSaveAll.Click
        SaveAll()
    End Sub

    Private Sub BtnOpen_Click(sender As Object, e As EventArgs) Handles BtnOpen.Click
        OpenFiles()
    End Sub

    Private Sub BtnScan_Click(sender As Object, e As EventArgs) Handles BtnScan.Click
        ScanImages()
    End Sub

    Private Sub BtnDisplayBootSector_Click(sender As Object, e As EventArgs) Handles BtnDisplayBootSector.Click
        Dim Block As DiskImage.DataBlock
        With Block
            .Offset = 0
            .Length = 512
        End With

        Dim frmHexView As New HexViewForm(_Disk, Block, False, "Boot Sector")
        frmHexView.ShowDialog()
    End Sub

    Private Sub BtnDisplayDirectory_Click(sender As Object, e As EventArgs) Handles BtnDisplayDirectory.Click
        If sender.Tag IsNot Nothing Then
            Dim DirectoryData = _DirectoryHashTable(sender.Tag)
            Dim DataBlockList = DirectoryData.Directory.GetData

            Dim Caption As String = "Directory - " & IIf(DirectoryData.Path = "", "Root", DirectoryData.Path)

            Dim frmHexView As New HexViewForm(_Disk, DataBlockList, False, Caption, False)
            frmHexView.ShowDialog()
        End If
    End Sub
End Class

Public Structure ProcessFilesResponse
    Dim VolumeLabel As String
    Dim HasCreated As Boolean
    Dim HasLastAccessed As Boolean
    Dim HasLFN As Boolean
    Dim HasInvalidDirectoryEntries As Boolean
End Structure

Public Structure FileData
    Dim FilePath As String
    Dim Offset As UInteger
    Dim HasCreated As Boolean
    Dim HasLastAccessed As Boolean
End Structure

Public Structure DirectoryData
    Dim Path As String
    Dim Directory As DiskImage.Directory
End Structure


