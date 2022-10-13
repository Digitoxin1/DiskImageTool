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
        FlowLayoutPanel1.Visible = False
        BtnClearCreated.Enabled = False
        BtnClearCreated.Visible = False
        BtnDisplayClusters.Visible = False
        BtnClearLastAccessed.Enabled = False
        BtnClearLastAccessed.Visible = False
        BtnScan.Enabled = False
        BtnFilters.Enabled = False
        BtnRevert.Visible = False
        CBCheckAll.Visible = False
        LblFilterMessage.Visible = False
        ToolStripFileCount.Visible = False
        ToolStripFileName.Visible = False
        ToolStripModified.Visible = False
    End Sub
    Private Function IsFiltered(ImageData As LoadedImageData, AppliedFilters As FilterTypes) As Boolean
        If (AppliedFilters And FilterTypes.HasInvalidImage) > 0 Then
            If Not ImageData.ScanInfo.IsValidImage Then
                Return False
            End If
        End If

        If (AppliedFilters And FilterTypes.HasCreated) > 0 Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasCreated Then
                Return False
            End If
        End If

        If (AppliedFilters And FilterTypes.HasLastAccessed) > 0 Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasLastAccessed Then
                Return False
            End If
        End If

        If (AppliedFilters And FilterTypes.MismatchedOEMID) > 0 Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.OEMIDFound And Not ImageData.ScanInfo.OEMIDMatched Then
                Return False
            End If
        End If

        If (AppliedFilters And FilterTypes.UnknownOEMID) > 0 Then
            If ImageData.ScanInfo.IsValidImage And Not ImageData.ScanInfo.OEMIDFound Then
                Return False
            End If
        End If

        If (AppliedFilters And FilterTypes.HasLongFileNames) > 0 Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasLongFileNames Then
                Return False
            End If
        End If

        If (AppliedFilters And FilterTypes.HasInvalidDirectoryEntries) > 0 Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasInvalidDirectoryEntries Then
                Return False
            End If
        End If

        If (AppliedFilters And FilterTypes.ModifiedFiles) > 0 Then
            If ImageData.Modified Then
                Return False
            End If
        End If

        If (AppliedFilters And FilterTypes.UnusedClusters) > 0 Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasUnusedClusters Then
                Return False
            End If
        End If

        Return True
    End Function

    Private Sub ApplyFilters(e As ItemCheckEventArgs)
        Dim Count As Integer = ListFilters.CheckedItems.Count
        Dim IsChecked As Boolean
        Dim Index As Integer = -1
        Dim CurrentChecked As Boolean = False

        If e IsNot Nothing Then
            If e.NewValue = CheckState.Checked And e.CurrentValue = CheckState.Unchecked Then
                Count += 1
            ElseIf e.NewValue = CheckState.Unchecked And e.CurrentValue = CheckState.Checked Then
                Count -= 1
            End If
            Index = e.Index
            CurrentChecked = (e.NewValue = CheckState.Checked)
        End If

        If Count > 0 Then
            Me.UseWaitCursor = True

            Dim SelectedItem As LoadedImageData = ComboGroups.SelectedItem
            ComboGroups.Items.Clear()
            Dim AppliedFilters As FilterTypes = 0
            For Counter = 0 To ListFilters.Items.Count - 1
                If Counter = Index Then
                    IsChecked = CurrentChecked
                Else
                    IsChecked = ListFilters.GetItemChecked(Counter)
                End If
                If IsChecked Then
                    Dim Item As ComboFileType = ListFilters.Items(Counter)
                    AppliedFilters += Item.ID
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

            BtnFilters.BackColor = Color.LightGreen

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
        BtnFilters.Enabled = False
        ListFilters.Items.Clear()
    End Sub

    Private Sub ResetFilterButton()
        _FiltersApplied = False

        BtnFilters.BackColor = SystemColors.Control
        BtnFilters.UseVisualStyleBackColor = True
        ToolStripFileCount.Text = _LoadedImageList.Count & " File" & IIf(_LoadedImageList.Count <> 1, "s", "")
        ToolStripFileCount.Visible = True
    End Sub

    Private Function ChangeOEMID() As Boolean
        Dim frmOEMID As New OEMIDForm(_Disk, _OEMIDDictionary)
        Dim Result As Boolean

        frmOEMID.ShowDialog()

        Result = frmOEMID.Result

        If Result Then
            ComboItemSetModified(False)
        End If

        Return Result
    End Function

    Private Sub ComboItemSetModified(FullRefresh As Boolean)
        Dim ImageData As LoadedImageData = ComboGroups.SelectedItem
        If Not ImageData.Modified Then
            ImageData.Modified = True
            _ModifiedCount += 1
            ToolStripModified.Text = _ModifiedCount & " File" & IIf(_ModifiedCount <> 1, "s", "") & " Modified"
            ToolStripModified.Visible = True
            RefreshModifiedFilter()
        End If
        ImageData.Modifications = _Disk.Modifications
        _SuppressEvent = True
        ComboGroups.Items(ComboGroups.SelectedIndex) = ComboGroups.Items(ComboGroups.SelectedIndex)
        _SuppressEvent = False
        If FullRefresh Then
            PopulateDetails()
        End If
        PopulateSummary()
        BtnSave.Enabled = True
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
            ComboItemSetModified(False)
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
            ComboItemSetModified(False)
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

    Private Sub RefreshButtonState()
        If _Disk.IsValidImage Then
            FlowLayoutPanel1.Visible = True
            CBCheckAll.Visible = True
        Else
            FlowLayoutPanel1.Visible = False
            CBCheckAll.Visible = False
        End If
        BtnClearCreated.Enabled = False
        BtnClearLastAccessed.Enabled = False
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
        BtnClearCreated.Visible = (Response.HasCreated)
        BtnClearLastAccessed.Visible = (Response.HasLastAccessed)
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
                    'If Strings.Right(OEMIDString, 3) <> "IHC" Then
                    'System.IO.File.AppendAllText("D:\exceptions.txt", BootstrapChecksum.ToString("X8") & "  " & OEMIDString & "  " & _BootstrapTypes.Item(BootstrapChecksum).OEMID & "  " & DiskImage.FilePath & vbCrLf)
                    'End If
                End If
                'Else
                'System.IO.File.AppendAllText("D:\missing.txt", BootstrapChecksum.ToString("X8") & "  " & OEMIDString & "  " & DiskImage.FilePath & vbCrLf)
            End If
        End With
        With ListViewHashes.Items
            .Clear()
            .Add(GetListViewSummaryItem("CRC32", Crc32.ComputeChecksum(_Disk.Data).ToString("X8")))
            .Add(GetListViewSummaryItem("MD5", MD5Hash(_Disk.Data)))
            .Add(GetListViewSummaryItem("SHA-1", SHA1Hash(_Disk.Data)))
        End With

        BtnDisplayClusters.Visible = _Disk.HasUnusedClustersWithData
        BtnRevert.Visible = _Disk.Modified
    End Sub
    Private Sub ProcessFileDrop(Files() As String)
        Dim AllowedExtensions = {".img", ".ima"}
        Dim FilePath As String
        Dim FileInfo As System.IO.FileInfo
        Dim PathName As String
        Dim FileName As String
        Dim ComboCleared As Boolean = False

        BtnSave.Enabled = False

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

        Dim ImageData As LoadedImageData = ComboGroups.SelectedItem
        If ImageData.Modified Then
            ImageData.Modified = False
            _ModifiedCount -= 1
            ToolStripModified.Text = _ModifiedCount & " File" & IIf(_ModifiedCount <> 1, "s", "") & " Modified"
            ToolStripModified.Visible = (_ModifiedCount > 0)
            RefreshModifiedFilter()
        End If
        ImageData.Modifications = Nothing
        _SuppressEvent = True
        ComboGroups.Items(ComboGroups.SelectedIndex) = ComboGroups.Items(ComboGroups.SelectedIndex)
        _SuppressEvent = False
        PopulateDetails()
        PopulateSummary()
        BtnSave.Enabled = (_ModifiedCount > 0)
    End Sub

    Private Sub SaveChanges()
        _SuppressEvent = True
        For Each ImageData In _LoadedImageList
            If ImageData.Modified Then
                Dim FilePath As String = System.IO.Path.Combine(ImageData.Path, ImageData.File)
                Dim Disk = New DiskImage.Disk(FilePath)
                Disk.ApplyModifications(ImageData.Modifications)
                Dim BackupPath As String = Disk.FilePath & ".bak"
                System.IO.File.Copy(Disk.FilePath, BackupPath, True)
                Disk.SaveFile(Disk.FilePath)
                ImageData.Modified = False
                ImageData.Modifications = Nothing
                _ModifiedCount -= 1
                ToolStripModified.Text = _ModifiedCount & " File" & IIf(_ModifiedCount <> 1, "s", "") & " Modified"
                ToolStripModified.Visible = (_ModifiedCount > 0)
                If ImageData.ComboIndex > -1 Then
                    ComboGroups.Items(ImageData.ComboIndex) = ComboGroups.Items(ImageData.ComboIndex)
                End If
            End If
        Next
        _SuppressEvent = False
        PopulateSummary()
        RefreshModifiedFilter()
        BtnSave.Enabled = False
    End Sub

    Private Sub RefreshModifiedFilter()
        Dim Item As ComboFileType
        Dim Index As Integer = -1
        Dim FilterApplied As Boolean = False

        For Counter = 0 To ListFilters.Items.Count - 1
            Item = ListFilters.Items(Counter)
            If Item.ID = FilterTypes.ModifiedFiles Then
                Index = Counter
                Exit For
            End If
        Next

        If _ModifiedCount > 0 Then
            If Index = -1 Then
                ListFilters.Items.Insert(0, New ComboFileType(FilterTypes.ModifiedFiles, _ModifiedCount))
            Else
                Item = ListFilters.Items(Index)
                Item.Count = _ModifiedCount
            End If
        Else
            If Index > -1 Then
                FilterApplied = ListFilters.GetItemChecked(Index)
                ListFilters.Items.RemoveAt(Index)
            End If
        End If

        ListFilters.Height = Math.Max(3, ListFilters.Items.Count) * 15 + 4
        BtnFilters.Enabled = (ListFilters.Items.Count > 0)

        If FilterApplied Then
            ApplyFilters(Nothing)
        End If
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

        LblFilterMessage.Visible = True
        For Counter = 0 To ComboGroups.Items.Count - 1
            Dim Percentage = Counter / ComboGroups.Items.Count * 100
            If Counter Mod 100 = 0 Then
                LblFilterMessage.Text = "Scanning... " & Int(Percentage) & "%"
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
        LblFilterMessage.Visible = False

        If _ModifiedCount > 0 Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.ModifiedFiles, _ModifiedCount))
        End If
        If UnknownOEMIDCount > 0 Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.UnknownOEMID, UnknownOEMIDCount))
        End If
        If MismatchedOEMIDCount > 0 Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.MismatchedOEMID, MismatchedOEMIDCount))
        End If
        If CreatedCount > 0 Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.HasCreated, CreatedCount))
        End If
        If LastAccessedCount > 0 Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.HasLastAccessed, LastAccessedCount))
        End If
        If LongFileNameCount > 0 Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.HasLongFileNames, LongFileNameCount))
        End If
        If InvalidDirectoryEntryCount > 0 Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.HasInvalidDirectoryEntries, InvalidDirectoryEntryCount))
        End If
        If UnusedClusterCount > 0 Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.UnusedClusters, UnusedClusterCount))
        End If
        If InvalidImageCount > 0 Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.HasInvalidImage, InvalidImageCount))
        End If
        ListFilters.Height = Math.Max(3, ListFilters.Items.Count) * 15 + 4
        BtnFilters.Enabled = (ListFilters.Items.Count > 0)
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
    Private Sub Button1_Click(sender As Object, e As EventArgs)
        For Counter = 0 To ComboGroups.Items.Count - 1
            ComboGroups.SelectedIndex = Counter
            Debug.Print(ComboGroups.Text)
            Application.DoEvents()
        Next
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

    Private Sub BtnDisplayBootSector_Click(sender As Object, e As EventArgs) Handles BtnDisplayBootSector.Click
        Dim Block As DiskImage.DataBlock

        Block.Cluster = 0
        Block.Sector = 0
        Block.Offset = 0
        Block.Data = _Disk.BootSector.Data

        Dim DataBlockList = New List(Of DiskImage.DataBlock) From {
            Block
        }

        Dim frmHexView As New HexViewForm(_Disk, DataBlockList, False)
        frmHexView.ShowDialog()
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

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        SaveChanges()
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

    Private Sub BtnScan_Click(sender As Object, e As EventArgs) Handles BtnScan.Click
        ScanImages()
    End Sub

    Private Sub BtnFilters_Click(sender As Object, e As EventArgs) Handles BtnFilters.Click
        ListFilters.Visible = Not ListFilters.Visible
        If ListFilters.Visible Then
            ListFilters.Focus()
        End If
    End Sub

    Private Sub ListFilters_LostFocus(sender As Object, e As EventArgs) Handles ListFilters.LostFocus
        ListFilters.Visible = False
    End Sub

    Private Sub ListFilters_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles ListFilters.ItemCheck
        ApplyFilters(e)
    End Sub

    Private Sub MainForm_Click(sender As Object, e As EventArgs) Handles Me.Click
        If ListFilters.Visible Then
            ListFilters.Visible = False
        End If
    End Sub

    Private Sub ContextMenuFiles_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuFiles.Opening
        If ListViewFiles.FocusedItem IsNot Nothing Then
            Dim DirectoryData = _DirectoryHashTable(ListViewFiles.FocusedItem.Group.GetHashCode)
            ItemDisplayDirectory.Text = "Display Directory (" & IIf(DirectoryData.Path = "", "Root", DirectoryData.Path) & ")"
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub ItemDisplayDirectory_Click(sender As Object, e As EventArgs) Handles ItemDisplayDirectory.Click
        If ListViewFiles.FocusedItem IsNot Nothing Then
            Dim DirectoryData = _DirectoryHashTable(ListViewFiles.FocusedItem.Group.GetHashCode)
            Dim DataBlockList = DirectoryData.Directory.GetData

            Dim frmHexView As New HexViewForm(_Disk, DataBlockList, False)
            frmHexView.ShowDialog()
        End If
    End Sub

    Private Sub FlowLayoutPanel1_Resize(sender As Object, e As EventArgs) Handles FlowLayoutPanel1.Resize
        FlowLayoutPanel1.Left = (FlowLayoutPanel1.Parent.Width - FlowLayoutPanel1.Width) / 2
    End Sub

    Private Sub BtnDisplayClusters_Click(sender As Object, e As EventArgs) Handles BtnDisplayClusters.Click
        Dim DataBlockList = _Disk.GetUnusedClustersWithData

        Dim frmHexView As New HexViewForm(_Disk, DataBlockList, True)
        frmHexView.ShowDialog()
        If frmHexView.Modified Then
            ComboItemSetModified(False)
        End If
    End Sub

    Private Sub BtnRevert_Click(sender As Object, e As EventArgs) Handles BtnRevert.Click
        RevertChanges
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


