Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports DiskImageTool.DiskImage
Imports BootSectorOffsets = DiskImageTool.DiskImage.BootSector.BootSectorOffsets
Imports BootSectorSizes = DiskImageTool.DiskImage.BootSector.BootSectorSizes

Public Structure FileData
    Dim DirectoryEntry As DiskImage.DirectoryEntry
    Dim FilePath As String
    Dim IsLastEntry As Boolean
End Structure

Public Class MainForm
    Private WithEvents ContextMenuCopy1 As ContextMenuStrip
    Private WithEvents ContextMenuCopy2 As ContextMenuStrip
    Private WithEvents Debounce As Timer
    Public Const NULL_CHAR As Char = "�"
    Public Const SITE_URL = "https://github.com/Digitoxin1/DiskImageTool"
    Public Const UPDATE_URL = "https://api.github.com/repos/Digitoxin1/DiskImageTool/releases/latest"
    Private Const FILE_FILTER As String = "Disk Image Files (*.ima; *.img)|*.ima;*.img|All files (*.*)|*.*"
    Private _CheckAll As Boolean = False
    Private _Disk As DiskImage.Disk
    Private _FilterCounts() As Integer
    Private _FilterOEMName As Dictionary(Of String, ComboOEMNameItem)
    Private _FiltersApplied As Boolean = False
    Private _LoadedFileNames As Dictionary(Of String, LoadedImageData)
    Private _OEMNameDictionary As Dictionary(Of UInteger, BootstrapLookup)
    Private _ScanRun As Boolean = False
    Private _SuppressEvent As Boolean = False
    Private _FileVersion As String = ""

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Friend Function DiskImageLoad(ImageData As LoadedImageData) As DiskImage.Disk
        Return New DiskImage.Disk(ImageData.FilePath, ImageData.Modifications)
    End Function

    Friend Function ItemScanDirectory(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False) As ProcessDirectoryEntryResponse
        Dim Response As ProcessDirectoryEntryResponse

        If Not Remove And Disk.IsValidImage Then
            Response = ProcessDirectoryEntries(Disk.Directory, "", True)
        Else
            With Response
                .HasCreated = False
                .HasFATChainingErrors = False
                .HasInvalidDirectoryEntries = False
                .HasLastAccessed = False
                .HasLFN = False
            End With
        End If

        If Not ImageData.Scanned Or Response.HasCreated <> ImageData.ScanInfo.HasCreated Then
            ImageData.ScanInfo.HasCreated = Response.HasCreated
            If Response.HasCreated Then
                _FilterCounts(FilterTypes.HasCreated) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.HasCreated) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.HasCreated)
            End If
        End If

        If Not ImageData.Scanned Or Response.HasLastAccessed <> ImageData.ScanInfo.HasLastAccessed Then
            ImageData.ScanInfo.HasLastAccessed = Response.HasLastAccessed
            If Response.HasLastAccessed Then
                _FilterCounts(FilterTypes.HasLastAccessed) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.HasLastAccessed) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.HasLastAccessed)
            End If
        End If

        If Not ImageData.Scanned Or Response.HasLFN <> ImageData.ScanInfo.HasLongFileNames Then
            ImageData.ScanInfo.HasLongFileNames = Response.HasLFN
            If Response.HasLFN Then
                _FilterCounts(FilterTypes.HasLongFileNames) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.HasLongFileNames) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.HasLongFileNames)
            End If
        End If

        If Not ImageData.Scanned Or Response.HasInvalidDirectoryEntries <> ImageData.ScanInfo.HasInvalidDirectoryEntries Then
            ImageData.ScanInfo.HasInvalidDirectoryEntries = Response.HasInvalidDirectoryEntries
            If Response.HasInvalidDirectoryEntries Then
                _FilterCounts(FilterTypes.HasInvalidDirectoryEntries) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.HasInvalidDirectoryEntries) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.HasInvalidDirectoryEntries)
            End If
        End If

        If Not ImageData.Scanned Or Response.HasFATChainingErrors <> ImageData.ScanInfo.HasFATChainingErrors Then
            ImageData.ScanInfo.HasFATChainingErrors = Response.HasFATChainingErrors
            If Response.HasFATChainingErrors Then
                _FilterCounts(FilterTypes.HasFATChainingErrors) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.HasFATChainingErrors) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.HasFATChainingErrors)
            End If
        End If

        Return Response
    End Function

    Friend Sub ItemScanDisk(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim IsValidImage As Boolean = Disk.IsValidImage
        Dim HasBadSectors As Boolean = False
        Dim HasMismatchedFATs As Boolean = False
        Dim HasInvalidImageSize As Boolean = False

        If Not Remove And IsValidImage Then
            HasBadSectors = Disk.FAT.BadClusters.Count > 0
            HasMismatchedFATs = Not Disk.FAT.CompareTables
            HasInvalidImageSize = Disk.HasInvalidSize
        End If

        If Remove Then
            IsValidImage = True
        End If

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

        If Not ImageData.Scanned Or HasBadSectors <> ImageData.ScanInfo.HasBadSectors Then
            ImageData.ScanInfo.HasBadSectors = HasBadSectors
            If HasBadSectors Then
                _FilterCounts(FilterTypes.HasBadSectors) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.HasBadSectors) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.HasBadSectors)
            End If
        End If

        If Not ImageData.Scanned Or HasMismatchedFATs <> ImageData.ScanInfo.HasMismatchedFATs Then
            ImageData.ScanInfo.HasMismatchedFATs = HasMismatchedFATs
            If HasMismatchedFATs Then
                _FilterCounts(FilterTypes.HasMismatchedFATs) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.HasMismatchedFATs) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.HasMismatchedFATs)
            End If
        End If

        If Not ImageData.Scanned Or HasInvalidImageSize <> ImageData.ScanInfo.HasInvalidImageSize Then
            ImageData.ScanInfo.HasInvalidImageSize = HasInvalidImageSize
            If HasInvalidImageSize Then
                _FilterCounts(FilterTypes.HasInvalidImageSize) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.HasInvalidImageSize) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.HasInvalidImageSize)
            End If
        End If
    End Sub

    Friend Sub ItemScanModified(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim IsModified As Boolean = Not Remove And (Not _Disk.LoadError AndAlso Disk.Data.Modified)

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

    Friend Sub ItemScanOEMName(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim OEMNameMatched As Boolean = True
        Dim OEMNameFound As Boolean = True
        Dim OEMNameWin9x As Boolean = False
        Dim OEMNameString As String = ""

        If Not Remove And Disk.IsValidImage Then
            Dim BootstrapChecksum = Crc32.ComputeChecksum(Disk.BootSector.BootStrapCode)
            Dim OEMName = Disk.BootSector.OEMName
            OEMNameWin9x = Disk.BootSector.IsWin9xOEMName
            OEMNameString = Disk.BootSector.GetOEMNameString.TrimEnd(NULL_CHAR)

            Dim BootstrapType = OEMNameFindMatch(BootstrapChecksum)
            OEMNameFound = BootstrapType IsNot Nothing
            If OEMNameFound AndAlso Not OEMNameWin9x AndAlso Not BootstrapType.ExactMatch Then
                OEMNameMatched = False
                For Each KnownOEMName In BootstrapType.KnownOEMNames
                    If ByteArrayCompare(KnownOEMName.Name, OEMName) Then
                        OEMNameMatched = True
                        Exit For
                    End If
                Next
            Else
                OEMNameMatched = True
            End If
        End If

        If Not ImageData.Scanned Or OEMNameWin9x <> ImageData.ScanInfo.OEMNameWin9x Then
            ImageData.ScanInfo.OEMNameWin9x = OEMNameWin9x
            If OEMNameWin9x Then
                _FilterCounts(FilterTypes.Windows9xOEMName) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.Windows9xOEMName) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.Windows9xOEMName)
            End If
        End If

        If Not ImageData.Scanned Or OEMNameMatched <> ImageData.ScanInfo.OEMNameMatched Then
            ImageData.ScanInfo.OEMNameMatched = OEMNameMatched
            If Not OEMNameMatched Then
                _FilterCounts(FilterTypes.MismatchedOEMName) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.MismatchedOEMName) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.MismatchedOEMName)
            End If
        End If

        If Not ImageData.Scanned Or OEMNameFound <> ImageData.ScanInfo.OEMNameFound Then
            ImageData.ScanInfo.OEMNameFound = OEMNameFound
            If Not OEMNameFound Then
                _FilterCounts(FilterTypes.UnknownOEMName) += 1
            ElseIf ImageData.Scanned Then
                _FilterCounts(FilterTypes.UnknownOEMName) -= 1
            End If
            If UpdateFilters Then
                FilterUpdate(FilterTypes.UnknownOEMName)
            End If
        End If

        If Disk.IsValidImage And (Not ImageData.Scanned Or OEMNameString <> ImageData.ScanInfo.OEMName) Then
            If ImageData.Scanned Then
                If _FilterOEMName.ContainsKey(ImageData.ScanInfo.OEMName) Then
                    Dim Item = _FilterOEMName.Item(ImageData.ScanInfo.OEMName)
                    Item.Count -= 1
                    If Item.Count = 0 Then
                        _FilterOEMName.Remove(Item.Name)
                        If UpdateFilters Then
                            ComboOEMName.Items.Remove(Item)
                            If ComboOEMName.SelectedIndex = -1 Then
                                ComboOEMName.SelectedIndex = 0
                            End If
                        End If
                    Else
                        If UpdateFilters Then
                            Dim Index = ComboOEMName.Items.IndexOf(Item)
                            ComboOEMName.Items.Item(Index) = Item
                        End If
                    End If
                End If
            End If

            ImageData.ScanInfo.OEMName = OEMNameString

            If Not Remove AndAlso Not OEMNameWin9x Then
                If _FilterOEMName.ContainsKey(OEMNameString) Then
                    Dim Item = _FilterOEMName.Item(ImageData.ScanInfo.OEMName)
                    Item.Count += 1
                    If UpdateFilters Then
                        Dim Index = ComboOEMName.Items.IndexOf(Item)
                        ComboOEMName.Items.Item(Index) = Item
                    End If
                Else
                    Dim Item = New ComboOEMNameItem With {
                        .Name = OEMNameString,
                        .Count = 1
                    }
                    _FilterOEMName.Add(Item.Name, Item)
                    If UpdateFilters Then
                        ComboOEMName.Items.Add(Item)
                    End If
                End If
            End If
        End If
    End Sub

    Friend Sub ItemScanUnusedClusters(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim HasUnusedClusters As Boolean = False

        If Not Remove And Disk.IsValidImage Then
            HasUnusedClusters = Disk.FAT.HasUnusedSectors(True)
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

    <Runtime.InteropServices.DllImport("User32.dll")>
    Private Shared Function SetForegroundWindow(ByVal hWnd As Integer) As Integer
    End Function

    Private Sub BadSectorsDisplayHex()
        Dim HexViewSectorData As New HexViewSectorData(_Disk)
        Dim Offset As UInteger = 0
        Dim Length As UInteger = 0
        Dim LastCluster As UShort = 0

        For Each Cluster In _Disk.FAT.BadClusters
            If Cluster > LastCluster + 1 Then
                If Length > 0 Then
                    HexViewSectorData.SectorData.AddBlockByOffset(Offset, Length)
                End If
                Offset = _Disk.BootSector.ClusterToOffset(Cluster)
                Length = 0
            End If
            Length += _Disk.BootSector.BytesPerCluster
            LastCluster = Cluster
        Next

        If Length > 0 Then
            HexViewSectorData.SectorData.AddBlockByOffset(Offset, Length)
        End If

        If DisplayHexViewForm(HexViewSectorData, "Bad Sectors", False, False) Then
            ComboItemRefresh(False, True)
        End If
    End Sub

    Private Sub BootSectorDisplayHex()
        Dim HexViewSectorData As New HexViewSectorData(_Disk)
        Dim HighlightedRegions As New HighlightedRegions

        HexViewSectorData.SectorData.AddBlock(0, 1)
        HexViewSectorData.HighlightedRegionList.Add(HighlightedRegions)

        If _Disk.IsValidImage Then
            Dim BootStrapStart = _Disk.BootSector.GetBootStrapOffset
            Dim BootStrapLength = BootSectorOffsets.BootStrapSignature - BootStrapStart

            Dim ForeColor As Color
            If _Disk.BootSector.HasValidJumpInstruction Then
                ForeColor = Color.Green
            Else
                ForeColor = Color.Black
            End If

            HighlightedRegions.AddOffset(BootSectorOffsets.JmpBoot, ForeColor)
            HighlightedRegions.AddOffset(BootSectorOffsets.OEMName, Color.Red)
            HighlightedRegions.AddOffset(BootSectorOffsets.BytesPerSector, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.SectorsPerCluster, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.ReservedSectorCount, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.NumberOfFATs, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.RootEntryCount, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.SectorCountSmall, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.MediaDescriptor, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.SectorsPerFAT, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.SectorsPerTrack, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.NumberOfHeads, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.HiddenSectors, Color.Blue)

            If _Disk.BootSector.HasValidExtendedBootSignature And BootStrapStart >= BootSectorOffsets.FileSystemType + BootSectorSizes.FileSystemType Then
                HighlightedRegions.AddOffset(BootSectorOffsets.DriveNumber, Color.Purple)
                HighlightedRegions.AddOffset(BootSectorOffsets.Reserved, Color.Purple)
                HighlightedRegions.AddOffset(BootSectorOffsets.ExtendedBootSignature, Color.Purple)
                HighlightedRegions.AddOffset(BootSectorOffsets.VolumeSerialNumber, Color.Purple)
                HighlightedRegions.AddOffset(BootSectorOffsets.VolumeLabel, Color.Purple)
                HighlightedRegions.AddOffset(BootSectorOffsets.FileSystemType, Color.Purple)
            End If

            If BootStrapStart > 2 And BootStrapLength > 1 Then
                HighlightedRegions.AddItem(BootStrapStart, BootStrapLength, ForeColor, "Boot Strap Code")
            End If

            If _Disk.BootSector.HasValidBootStrapSignature Then
                ForeColor = Color.Goldenrod
            Else
                ForeColor = Color.Black
            End If

            HighlightedRegions.AddOffset(BootSectorOffsets.BootStrapSignature, ForeColor)
        End If

        If DisplayHexViewForm(HexViewSectorData, "Boot Sector", False, False) Then
            DiskImageProcess(True, True)
        End If
    End Sub

    Private Sub CheckForUpdates()
        Dim DownloadVersion As String = ""
        Dim DownloadURL As String = ""
        Dim UpdateAvailable As Boolean = False
        Dim Regex As Regex
        Dim Match As Match

        Cursor.Current = Cursors.WaitCursor
        Try
            Dim Request As HttpWebRequest = WebRequest.Create(UPDATE_URL)
            Request.UserAgent = "DiskImageTool"
            Dim Response As HttpWebResponse = Request.GetResponse
            Dim Reader As New StreamReader(Response.GetResponseStream)
            Dim ResponseText = Reader.ReadToEnd


            Regex = New Regex("""tag_name"":""(\d+\.\d+(?:\.\d+)?(?:\.\d+)?)""")
            Match = Regex.Match(ResponseText)
            If Match.Success Then
                If Match.Groups.Count > 1 Then
                    DownloadVersion = Match.Groups.Item(1).Value
                End If
            End If

            Regex = New Regex("""browser_download_url"":""([^""]+)""")
            Match = Regex.Match(ResponseText)
            If Match.Success Then
                If Match.Groups.Count > 1 Then
                    DownloadURL = Match.Groups.Item(1).Value
                End If
            End If
        Catch
            MsgBox("An error occured while checking for updates.  Please try again later.", MsgBoxStyle.Exclamation)
            Exit Sub
        End Try
        Cursor.Current = Cursors.Default

        If DownloadVersion <> "" And DownloadURL <> "" Then
            Dim CurrentVersion = FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion
            UpdateAvailable = Version.Parse(DownloadVersion) > Version.Parse(CurrentVersion)
        End If

        If UpdateAvailable Then
            Dim Msg = "A New version Of " & My.Application.Info.Title & " Is available." & vbCrLf & vbCrLf & "Do you wish to download it at this time?"
            If MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
                Dim Dialog As New SaveFileDialog
                Dialog.Filter = "Zip Archive|*.zip"
                Dialog.FileName = Path.GetFileName(DownloadURL)
                Dialog.ShowDialog()
                If Dialog.FileName <> "" Then
                    Cursor.Current = Cursors.WaitCursor
                    Try
                        Dim Client As New WebClient()
                        Client.DownloadFile(DownloadURL, Dialog.FileName)
                    Catch
                        MsgBox("An error occured while downloading the file.", MsgBoxStyle.Exclamation)
                    End Try
                    Cursor.Current = Cursors.Default
                End If
            End If
        Else
            MsgBox("You are running the latest version of " & My.Application.Info.Title & ".", MsgBoxStyle.Information)
        End If
    End Sub

    Private Sub ClearFilesPanel()
        ListViewFiles.Items.Clear()
        BtnWin9xClean.Enabled = False
        ItemSelectionChanged()
    End Sub

    Private Function CloseAll() As Boolean
        Dim BatchResult As MyMsgBoxResult = MyMsgBoxResult.Yes
        Dim Result As MyMsgBoxResult = MyMsgBoxResult.Yes

        Dim ModifyImageList = GetModifiedImageList()

        If ModifyImageList.Count > 0 Then
            Dim ShowDialog As Boolean = True

            For Each ImageData In ModifyImageList
                If ShowDialog Then
                    If ModifyImageList.Count = 1 Then
                        Result = MsgBoxSave(ImageData.FilePath)
                    Else
                        Result = MsgBoxSaveAll(ImageData.FilePath)
                    End If
                Else
                    Result = BatchResult
                End If
                If Result = MyMsgBoxResult.YesToAll Or Result = MyMsgBoxResult.NoToAll Then
                    ShowDialog = False
                    If Result = MyMsgBoxResult.NoToAll Then
                        BatchResult = MyMsgBoxResult.No
                    End If
                End If
                If Result = MyMsgBoxResult.Yes Or Result = MyMsgBoxResult.YesToAll Then
                    If Not DiskImageSave(ImageData) Then
                        Result = MyMsgBoxResult.Cancel
                        Exit For
                    End If
                ElseIf Result = MyMsgBoxResult.Cancel Then
                    Exit For
                End If
            Next
        End If

        If Result <> MyMsgBoxResult.Cancel Then
            ResetAll()
        End If
        Return (Result <> MyMsgBoxResult.Cancel)
    End Function

    Private Sub CloseCurrent()
        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem
        Dim Result As MsgBoxResult

        If CurrentImageData.Modified Then
            Result = MsgBoxSave(CurrentImageData.FilePath)
        Else
            Result = MsgBoxResult.No
        End If

        If Result = MsgBoxResult.Yes Then
            If Not DiskImageSave(CurrentImageData) Then
                Result = MsgBoxResult.Cancel
            End If
        End If

        If Result <> MsgBoxResult.Cancel Then
            FileClose(CurrentImageData)
        End If
    End Sub

    Private Sub ComboImagesRefreshCurrentItemText()
        _SuppressEvent = True

        If ComboImages.SelectedIndex > -1 Then
            ComboImages.Items(ComboImages.SelectedIndex) = ComboImages.Items(ComboImages.SelectedIndex)
        End If

        If ComboImagesFiltered.SelectedIndex > -1 Then
            ComboImagesFiltered.Items(ComboImagesFiltered.SelectedIndex) = ComboImagesFiltered.Items(ComboImagesFiltered.SelectedIndex)
        End If

        _SuppressEvent = False
    End Sub

    Private Sub ComboImagesRefreshItemText(Item As Object)
        Dim Index As Integer

        _SuppressEvent = True

        Index = ComboImages.Items.IndexOf(Item)
        If Index > -1 Then
            ComboImages.Items(Index) = ComboImages.Items(Index)
        End If

        Index = ComboImagesFiltered.Items.IndexOf(Item)
        If Index > -1 Then
            ComboImagesFiltered.Items(Index) = ComboImagesFiltered.Items(Index)
        End If

        _SuppressEvent = False
    End Sub

    Private Sub ComboImagesRefreshItemText()
        _SuppressEvent = True

        For Index = 0 To ComboImages.Items.Count - 1
            ComboImages.Items(Index) = ComboImages.Items(Index)
        Next

        For Index = 0 To ComboImagesFiltered.Items.Count - 1
            ComboImagesFiltered.Items(Index) = ComboImagesFiltered.Items(Index)
        Next

        _SuppressEvent = False
    End Sub

    Private Sub ComboImagesRefreshPaths()
        ComboImages.BeginUpdate()
        LoadedImageData.StringOffset = GetPathOffset()
        ComboImagesRefreshItemText()
        ComboImages.EndUpdate()
    End Sub

    Private Sub ComboItemRefresh(FullRefresh As Boolean, DoItemScan As Boolean)
        If DoItemScan Then
            ItemScanAll(_Disk, ComboImages.SelectedItem, True)
        End If

        ComboImagesRefreshCurrentItemText()

        PopulateSummaryPanel()

        If FullRefresh Then
            If _Disk.IsValidImage Then
                PopulateFilesPanel()
            Else
                ClearFilesPanel()
            End If
        End If

        SaveButtonsRefresh()
    End Sub

    Private Function DirectoryEntryCanExport(DirectoryEntry As DiskImage.DirectoryEntry) As Boolean
        Return DirectoryEntryIsValid(DirectoryEntry) _
            And Not DirectoryEntry.IsDeleted _
            And Not DirectoryEntry.IsDirectory _
            And Not DirectoryEntry.HasInvalidFilename _
            And Not DirectoryEntry.HasInvalidExtension
    End Function

    Private Sub DeleteFile(DirectoryEntry As DiskImage.DirectoryEntry, Clear As Boolean)
        Dim Msg As String = "Are you sure you with to delete " & DirectoryEntry.GetFullFileName & "?"
        If MsgBox(Msg, MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
            DirectoryEntry.Remove(Clear)

            ComboItemRefresh(True, True)
        End If
    End Sub

    Private Sub RemoveDeletedFile(DirectoryEntry As DiskImage.DirectoryEntry, IsLastEntry As Boolean)
        DirectoryEntry.Clear(IsLastEntry)
        ComboItemRefresh(True, True)
    End Sub

    Private Sub DirectoryEntryDisplayHex(Offset As UInteger)
        Dim HexViewSectorData As HexViewSectorData
        Dim Caption As String

        If Offset = 0 Then
            Caption = "Root Directory"
            HexViewSectorData = New HexViewSectorData(_Disk, _Disk.Directory.SectorChain)
        Else
            Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(Offset)
            If Not DirectoryEntryIsValid(DirectoryEntry) Then
                Exit Sub
            End If

            Caption = IIf(DirectoryEntry.IsDirectory, "Directory", "File") & " - " & DirectoryEntry.GetFullFileName

            If DirectoryEntry.IsDeleted Then
                Caption = "Deleted " & Caption
                Dim DataOffset = _Disk.BootSector.ClusterToOffset(DirectoryEntry.StartingCluster)
                Dim Length As UInteger
                Dim FileSize As UInteger
                If DirectoryEntry.IsDirectory Then
                    Length = _Disk.BootSector.BytesPerCluster
                    FileSize = Length
                Else
                    Length = Math.Ceiling(DirectoryEntry.FileSize / _Disk.BootSector.BytesPerCluster) * _Disk.BootSector.BytesPerCluster
                    FileSize = DirectoryEntry.FileSize
                End If

                HexViewSectorData = New HexViewSectorData(_Disk, DataOffset, Length)
                HighlightSectorData(HexViewSectorData, FileSize, True)
            Else
                HexViewSectorData = New HexViewSectorData(_Disk, DirectoryEntry.FATChain.SectorChain)
                If Not DirectoryEntry.IsDirectory Then
                    HighlightSectorData(HexViewSectorData, DirectoryEntry.FileSize, False)
                End If
            End If
        End If

        If DisplayHexViewForm(HexViewSectorData, Caption, False, False) Then
            ComboItemRefresh(True, True)
        End If
    End Sub

    Private Sub DirectoryEntryDisplayText(DirectoryEntry As DiskImage.DirectoryEntry)
        Dim frmTextView As TextViewForm

        If Not DirectoryEntryIsValid(DirectoryEntry) _
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

    Private Function DirectoryEntryGetStats(DirectoryEntry As DiskImage.DirectoryEntry) As DirectoryStats
        Dim Stats As DirectoryStats

        With Stats
            .IsValid = DirectoryEntryIsValid(DirectoryEntry)
            .IsDirectory = DirectoryEntry.IsDirectory
            .IsDeleted = DirectoryEntry.IsDeleted
            .IsModified = DirectoryEntry.IsModified
            .IsValidFile = .IsValid And Not .IsDirectory And Not .IsDeleted
            .CanExport = .IsValidFile And Not DirectoryEntry.HasInvalidFilename And Not DirectoryEntry.HasInvalidExtension
            .FileSize = DirectoryEntry.FileSize
            .FullFileName = DirectoryEntry.GetFullFileName
        End With

        Return Stats
    End Function

    Private Function DirectoryEntryIsValid(DirectoryEntry As DiskImage.DirectoryEntry) As Boolean
        With DirectoryEntry
            Return Not (.IsVolumeName Or .HasInvalidFileSize Or .HasInvalidStartingCluster Or .StartingCluster < 2)
        End With
    End Function

    Private Sub DiskImageDisplayHex()
        Dim HexViewSectorData = New HexViewSectorData(_Disk, 0, _Disk.Data.Length)
        Dim ClusterNavigator = _Disk.IsValidImage

        If DisplayHexViewForm(HexViewSectorData, "Disk", True, ClusterNavigator) Then
            ComboItemRefresh(True, True)
        End If
    End Sub


    Private Sub DiskImageLoad(DoItemScan As Boolean)
        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem
        _Disk = DiskImageLoad(CurrentImageData)
        If Not _Disk.LoadError Then
            CurrentImageData.Modifications = _Disk.Data.Changes
        End If
        Debug.Print("load")
        DiskImageProcess(DoItemScan, False)
    End Sub

    Private Sub DiskImageProcess(DoItemScan As Boolean, Reinitialize As Boolean)
        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem

        If Reinitialize Then
            _Disk.Reinitialize()
        End If

        InitButtonState()
        PopulateSummaryPanel()
        If _Disk.IsValidImage Then
            If CurrentImageData.CachedRootDir Is Nothing Then
                CurrentImageData.CachedRootDir = _Disk.Directory.GetContent
            End If
            PopulateFilesPanel()
        Else
            ClearFilesPanel()
        End If

        If DoItemScan Then
            ItemScanAll(_Disk, CurrentImageData, True)
            ComboImagesRefreshCurrentItemText()
            SaveButtonsRefresh()
        End If
    End Sub

    Private Function DiskImageSave(ImageData As LoadedImageData, Optional NewFilePath As String = "") As Boolean
        Dim Disk As DiskImage.Disk
        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem
        Dim Success As Boolean

        Do
            If ImageData Is CurrentImageData Then
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
            ItemScanModified(Disk, ImageData)
        End If

        Return Success
    End Function

    Private Sub DiskImagesScan(NewOnly As Boolean)
        Me.UseWaitCursor = True
        Dim T = Stopwatch.StartNew

        BtnScanNew.Visible = False
        BtnScan.Enabled = False
        If _FiltersApplied Then
            FiltersClear()
            ImageCountUpdate()
        End If
        Dim ItemScanForm As New ItemScanForm(Me, ComboImages.Items, NewOnly)
        ItemScanForm.ShowDialog()
        BtnScanNew.Visible = Not ItemScanForm.ScanComplete

        For Counter = 0 To [Enum].GetNames(GetType(FilterTypes)).Length - 1
            FilterUpdate(Counter)
        Next

        PopulateComboOEMName()

        BtnScan.Text = "Rescan Images"
        BtnScan.Enabled = True
        _ScanRun = True

        T.Stop()
        Debug.Print("ScanImages Time Taken: " & T.Elapsed.ToString)
        Me.UseWaitCursor = False

        SetForegroundWindow(Me.Handle)
        MainMenuFilters.ShowDropDown()
    End Sub

    Private Sub DisplayCrossLinkedFiles(DirectoryEntry As DiskImage.DirectoryEntry)
        Dim Msg As String = DirectoryEntry.GetFullFileName() & " is crosslinked with the following files:" & vbCrLf

        For Each Crosslink In DirectoryEntry.CrossLinks
            If Crosslink <> DirectoryEntry.Offset Then
                Dim CrossLinkedFile = _Disk.GetDirectoryEntryByOffset(Crosslink)
                Msg &= vbCrLf & CrossLinkedFile.GetFullFileName()
            End If
        Next
        MsgBox(Msg, MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
    End Sub

    Private Function DisplayHexViewForm(HexViewSectorData As HexViewSectorData, Caption As String, SectorNavigator As Boolean, ClusterNavigator As Boolean) As Boolean
        Dim frmHexView As New HexViewForm(HexViewSectorData, Caption, SectorNavigator, ClusterNavigator)
        frmHexView.ShowDialog()

        Return frmHexView.Modified
    End Function

    Private Sub DragDropSelectedFiles()
        If ListViewFiles.SelectedItems.Count = 0 Then
            Exit Sub
        End If

        Dim TempPath As String = IO.Path.GetTempPath() & Guid.NewGuid().ToString() & "\"

        FileExportSelected(False, TempPath)

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

    Private Function ExpandedDateToString(D As DiskImage.ExpandedDate) As String
        Return ExpandedDateToString(D, False, False, False, False)
    End Function

    Private Function ExpandedDateToString(D As DiskImage.ExpandedDate, IncludeTime As Boolean, IncludeSeconds As Boolean, IncludeMilliseconds As Boolean, Use24Hour As Boolean) As String
        Dim Response As String = Format(D.Year, "0000") & "-" & Format(D.Month, "00") & "-" & Format(D.Day, "00")
        If IncludeTime Then
            Response &= "  " & Format(IIf(Use24Hour, D.Hour, D.Hour12), "00") _
                & ":" & Format(D.Minute, "00")

            If IncludeSeconds Then
                Response &= ":" & Format(D.Second, "00")
            End If

            If IncludeMilliseconds Then
                Response &= Format(D.Milliseconds / 1000, ".000")
            End If

            If Not Use24Hour Then
                Response &= IIf(D.IsPM, " PM", " AM")
            End If
        End If

        Return Response
    End Function

    Private Sub ExportDebugScript()
        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem
        GenerateDebugPackage(_Disk, CurrentImageData)
    End Sub

    Private Sub FATDisplayHex()
        Dim HexViewSectorData = New HexViewSectorData(_Disk)
        Dim HighlightedRegions As New HighlightedRegions
        Dim OriginalData() As Byte = Nothing
        For Index = 0 To _Disk.BootSector.NumberOfFATs - 1
            Dim Length As UInteger = SectorToBytes(_Disk.BootSector.SectorsPerFAT)
            Dim Start As UInteger = SectorToBytes(_Disk.BootSector.FATRegionStart) + Length * Index
            Dim Data = _Disk.Data.GetBytes(Start, Length)

            HexViewSectorData.SectorData.AddBlockByOffset(Start, Length, "FAT " & Index + 1)
            HexViewSectorData.HighlightedRegionList.Add(HighlightedRegions)

            If Index = 0 Then
                OriginalData = Data
            Else
                Dim HighlightStart As Integer = -2
                Dim HighlightLength As Integer = 0
                For Counter = 0 To Data.Length - 1
                    If Data(Counter) <> OriginalData(Counter) Then
                        If Counter > HighlightStart + 1 Then
                            If HighlightLength > 0 Then
                                HighlightedRegions.AddItem(HighlightStart, HighlightLength, Color.Red)
                            End If
                            HighlightStart = Counter
                            HighlightLength = 1
                        Else
                            HighlightLength += 1
                        End If
                    End If
                Next
                If HighlightLength > 0 Then
                    HighlightedRegions.AddItem(HighlightStart, HighlightLength, Color.Red)
                End If
            End If
        Next

        If DisplayHexViewForm(HexViewSectorData, "File Allocation Table", False, False) Then
            DiskImageProcess(True, True)
        End If
    End Sub

    Private Sub FileClose(ImageData As LoadedImageData)
        ItemScanAll(_Disk, ImageData, True, True)
        _LoadedFileNames.Remove(ImageData.FilePath)

        Dim ActiveComboBox As ComboBox = IIf(_FiltersApplied, ComboImagesFiltered, ComboImages)

        Dim SelectedIndex = ActiveComboBox.SelectedIndex

        ComboImages.Items.Remove(ImageData)
        ComboImagesFiltered.Items.Remove(ImageData)

        If ActiveComboBox.SelectedIndex = -1 Then
            If SelectedIndex > ActiveComboBox.Items.Count - 1 Then
                SelectedIndex = ActiveComboBox.Items.Count - 1
            End If
            ActiveComboBox.SelectedIndex = SelectedIndex
        End If

        If ComboImages.Items.Count = 0 Then
            ResetAll()
        Else
            ImageCountUpdate()
        End If
    End Sub

    Private Sub FileCountUpdate()
        If ListViewFiles.SelectedItems.Count > 0 Then
            ToolStripFileCount.Text = ListViewFiles.SelectedItems.Count & " of " & ListViewFiles.Items.Count & " File" & IIf(ListViewFiles.Items.Count <> 1, "s", "") & " Selected"
        Else
            ToolStripFileCount.Text = ListViewFiles.Items.Count & " File" & IIf(ListViewFiles.Items.Count <> 1, "s", "")
        End If
        ToolStripFileCount.Visible = True
    End Sub

    Private Sub FileDropStart(e As DragEventArgs)
        If _SuppressEvent Then
            Exit Sub
        End If

        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub FileExport()
        If ListViewFiles.SelectedItems.Count = 1 Then
            FileExportCurrent()
        ElseIf ListViewFiles.SelectedItems.Count > 1 Then
            FileExportSelected(True)
        End If
    End Sub

    Private Sub FileExportCurrent()
        Dim FileData As FileData = ListViewFiles.SelectedItems(0).Tag
        Dim DirectoryEntry = FileData.DirectoryEntry

        Dim Dialog = New SaveFileDialog With {
            .FileName = DirectoryEntry.GetFullFileName
        }
        If Dialog.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If

        Dim Result = FileSave(Dialog.FileName, DirectoryEntry)

        If Not Result Then
            Dim Msg As String = "Error saving file '" & IO.Path.GetFileName(Dialog.FileName) & "'."
            MsgBox(Msg, MsgBoxStyle.Critical + MsgBoxStyle.OkOnly)
        End If
    End Sub

    Private Sub FileExportSelected(ShowDialog As Boolean, Optional Path As String = "")
        Dim ShowResults As Boolean = ShowDialog

        If Path = "" Then
            Dim Dialog = New FolderBrowserDialog

            If Dialog.ShowDialog <> DialogResult.OK Then
                Exit Sub
            End If
            Path = Dialog.SelectedPath
        End If

        Dim BatchResult As MyMsgBoxResult = MyMsgBoxResult.Yes
        Dim Result As MyMsgBoxResult = MyMsgBoxResult.Yes
        Dim FileCount As Integer = 0
        Dim TotalFiles As Integer = 0
        For Each Item As ListViewItem In ListViewFiles.SelectedItems
            Dim FileData As FileData = Item.Tag
            Dim DirectoryEntry = FileData.DirectoryEntry
            If DirectoryEntryCanExport(DirectoryEntry) Then
                TotalFiles += 1
                If Result <> MyMsgBoxResult.Cancel Then
                    Dim FilePath = IO.Path.Combine(Path, FileData.FilePath, DirectoryEntry.GetFullFileName)
                    If Not IO.Directory.Exists(IO.Path.GetDirectoryName(FilePath)) Then
                        IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(FilePath))
                    End If
                    If IO.File.Exists(FilePath) Then
                        If ShowDialog Then
                            Result = MsgBoxOverwrite(FilePath)
                        Else
                            Result = BatchResult
                        End If
                    Else
                        Result = MyMsgBoxResult.Yes
                    End If
                    If Result = MyMsgBoxResult.YesToAll Or Result = MyMsgBoxResult.NoToAll Then
                        ShowDialog = False
                        If Result = MyMsgBoxResult.NoToAll Then
                            BatchResult = MyMsgBoxResult.No
                        End If
                    End If
                    If Result = MyMsgBoxResult.Yes Or Result = MyMsgBoxResult.YesToAll Then
                        If FileSave(FilePath, DirectoryEntry) Then
                            FileCount += 1
                        End If
                    End If
                End If
            End If
        Next

        If ShowResults Then
            Dim Msg As String = FileCount & " of " & TotalFiles & " file" & IIf(TotalFiles <> 1, "s", "") & " exported successfully."
            MsgBox(Msg, MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
        End If
    End Sub

    Private Function FilePropertiesEdit() As Boolean
        Dim Result As Boolean = False

        Dim frmFileProperties As New FilePropertiesForm(_Disk, ListViewFiles.SelectedItems)
        frmFileProperties.ShowDialog()

        If frmFileProperties.DialogResult = DialogResult.OK Then
            Result = frmFileProperties.Updated

            If Result Then
                ComboItemRefresh(True, True)
            End If
        End If

        Return Result
    End Function

    Private Sub FileReplace(DirectoryEntry As DiskImage.DirectoryEntry)
        Dim Dialog = New OpenFileDialog
        If Dialog.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If

        Dim FileInfo As New IO.FileInfo(Dialog.FileName)
        Dim Msg As String

        If FileInfo.Length < DirectoryEntry.FileSize Then
            Msg = FileInfo.Name & " Is smaller than " & DirectoryEntry.GetFullFileName & " And will be padded With nulls." & vbCrLf & vbCrLf
        ElseIf FileInfo.Length > DirectoryEntry.FileSize Then
            Msg = FileInfo.Name & " Is larger than " & DirectoryEntry.GetFullFileName & " And will be truncated." & vbCrLf & vbCrLf
        Else
            Msg = ""
        End If
        Msg &= "Do you wish To replace " & DirectoryEntry.GetFullFileName & " With " & FileInfo.FullName & "?"

        If MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
            Dim ClusterSize = _Disk.BootSector.BytesPerCluster
            Dim FileSize = Math.Min(DirectoryEntry.FileSize, DirectoryEntry.FATChain.Chain.Count * ClusterSize)
            Dim BytesToFill As Integer
            Dim ClusterIndex As Integer = 0

            Using fs = FileInfo.OpenRead()
                Dim BytesToRead As Integer = Math.Min(fs.Length, FileSize)
                Dim n As Integer
                Do While BytesToRead > 0
                    Dim b(ClusterSize - 1) As Byte
                    Dim Cluster = DirectoryEntry.FATChain.Chain(ClusterIndex)
                    Dim ClusterOffset = _Disk.BootSector.ClusterToOffset(Cluster)
                    If BytesToRead < ClusterSize Then
                        b = _Disk.Data.GetBytes(ClusterOffset, ClusterSize)
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
                    _Disk.Data.SetBytes(b, ClusterOffset)

                    ClusterIndex += 1
                Loop
            End Using

            Do While FileSize > 0
                Dim b(ClusterSize - 1) As Byte
                Dim Cluster = DirectoryEntry.FATChain.Chain(ClusterIndex)
                Dim ClusterOffset = _Disk.BootSector.ClusterToOffset(Cluster)
                BytesToFill = Math.Min(FileSize, ClusterSize)
                If BytesToFill < ClusterSize Then
                    b = _Disk.Data.GetBytes(ClusterOffset, ClusterSize)
                End If
                For Counter = 0 To BytesToFill - 1
                    b(Counter) = 0
                Next
                FileSize -= BytesToFill
                _Disk.Data.SetBytes(b, ClusterOffset)
                ClusterIndex += 1
            Loop

            ComboItemRefresh(True, True)
        End If
    End Sub

    Private Function FileSave(FilePath As String, DirectoryEntry As DiskImage.DirectoryEntry) As Boolean
        Try
            IO.File.WriteAllBytes(FilePath, DirectoryEntry.GetContent)
            Dim D = DirectoryEntry.GetLastWriteDate
            If D.IsValidDate Then
                IO.File.SetLastWriteTime(FilePath, D.DateObject)
            End If
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Private Sub FilesOpen()
        Dim Dialog = New OpenFileDialog With {
            .Filter = FILE_FILTER,
            .Multiselect = True
        }
        If Dialog.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If

        ProcessFileDrop(Dialog.FileNames)
    End Sub

    Private Sub FiltersApply()
        Dim FilterCount As Integer = [Enum].GetNames(GetType(FilterTypes)).Length
        Dim TextFilter As String = TxtSearch.Text.Trim.ToLower

        Dim FiltersChecked As Boolean = False

        For Counter = 0 To FilterCount - 1
            Dim Item As ToolStripMenuItem = ContextMenuFilters.Items("key_" & Counter)
            If Item.Checked Then
                FiltersChecked = True
                Exit For
            End If
        Next

        Dim OEMNameItem As ComboOEMNameItem = ComboOEMName.SelectedItem
        Dim HasComboFilter = OEMNameItem IsNot Nothing AndAlso Not OEMNameItem.AllItems

        If FiltersChecked Or TextFilter.Length > 0 Or HasComboFilter Then
            Me.UseWaitCursor = True

            ComboImagesFiltered.BeginUpdate()
            ComboImagesFiltered.Items.Clear()

            Dim AppliedFilters As Integer = 0
            For Counter = 0 To FilterCount - 1
                Dim Item As ToolStripMenuItem = ContextMenuFilters.Items("key_" & Counter)
                If Item.Checked Then
                    AppliedFilters += Item.Tag
                End If
            Next

            For Each ImageData As LoadedImageData In ComboImages.Items
                Dim ShowItem As Boolean = True

                If TextFilter.Length > 0 Then
                    Dim FilePath = ImageData.FilePath.ToLower
                    ShowItem = FilePath.StartsWith(TextFilter) OrElse FilePath.Contains(" " & TextFilter) OrElse FilePath.Contains("\" & TextFilter)
                End If

                If ShowItem And HasComboFilter Then
                    ShowItem = ImageData.ScanInfo.IsValidImage And OEMNameItem.Name = ImageData.ScanInfo.OEMName
                End If

                If ShowItem And FiltersChecked Then
                    ShowItem = Not IsFiltered(ImageData, AppliedFilters)
                End If

                If ShowItem Then
                    Dim Index = ComboImagesFiltered.Items.Add(ImageData)
                    If ImageData Is ComboImages.SelectedItem Then
                        ComboImagesFiltered.SelectedIndex = Index
                    End If
                End If
            Next

            If ComboImagesFiltered.SelectedIndex = -1 AndAlso ComboImagesFiltered.Items.Count > 0 Then
                ComboImagesFiltered.SelectedIndex = 0
            End If

            MainMenuFilters.BackColor = Color.LightGreen

            ComboImagesFiltered.EndUpdate()
            ComboImagesFiltered.Visible = True
            ComboImagesFiltered.Enabled = ComboImagesFiltered.Items.Count > 0
            ComboImages.Visible = False

            _FiltersApplied = True
            BtnClearFilters.Enabled = True

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
        TxtSearch.Text = ""
        If ComboOEMName.Items.Count > 0 Then
            ComboOEMName.SelectedIndex = 0
        End If
        _SuppressEvent = False

        ComboImages.Visible = True
        ComboImagesFiltered.Visible = False
        ComboImagesFiltered.Items.Clear()

        MainMenuFilters.BackColor = SystemColors.Control
        _FiltersApplied = False
        BtnClearFilters.Enabled = False

        Me.UseWaitCursor = False
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
        _FilterOEMName = New Dictionary(Of String, ComboOEMNameItem)
    End Sub

    Private Sub FiltersReset()
        For Counter = 0 To _FilterCounts.Length - 1
            _FilterCounts(Counter) = 0
            FilterUpdate(Counter)
        Next
        BtnScan.Text = "Scan Images"
        TxtSearch.Text = ""
        BtnClearFilters.Enabled = False
        _FilterOEMName.Clear()
        ComboOEMName.Items.Clear()
        ComboOEMName.Visible = False
        ToolStripOEMName.Visible = False
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
            ToolStripBtnSaveAll.Enabled = BtnSaveAll.Enabled
        End If

        Return CheckstateChanged
    End Function

    Private Function GetFileDataFromDirectoryEntry(DirectoryEntry As DiskImage.DirectoryEntry, FilePath As String, IsLastEntry As Boolean) As FileData
        Dim Response As FileData
        With Response
            .FilePath = FilePath
            .DirectoryEntry = DirectoryEntry
            .IsLastEntry = IsLastEntry
        End With

        Return Response
    End Function

    Private Function GetModifiedImageList() As List(Of LoadedImageData)
        Dim ModifyImageList As New List(Of LoadedImageData)

        For Each ImageData As LoadedImageData In ComboImages.Items
            If ImageData.Modified Then
                ModifyImageList.Add(ImageData)
            End If
        Next

        Return ModifyImageList
    End Function

    Private Function GetPathOffset() As Integer
        Dim PathName As String = ""
        Dim CheckPath As Boolean = False

        For Each ImageData As LoadedImageData In ComboImages.Items
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

    Private Function GetWindowCaption() As String
        Return My.Application.Info.ProductName & " v" & _FileVersion
    End Function

    Private Sub HighlightSectorData(HexViewSectorData As HexViewSectorData, FileSize As UInteger, HighlightAssigned As Boolean)
        For Index = 0 To HexViewSectorData.SectorData.BlockCount - 1
            Dim SectorBlock = HexViewSectorData.SectorData.GetBlock(Index)
            Dim HighlightedRegions As New HighlightedRegions
            HexViewSectorData.HighlightedRegionList.Add(HighlightedRegions)

            If HighlightAssigned Then
                Dim BytesRemaining = FileSize
                For J = 0 To SectorBlock.SectorCount - 1
                    Dim Sector = SectorBlock.SectorStart + J
                    Dim BlockOffset = J * BYTES_PER_SECTOR
                    Dim BlockSize = Math.Min(BytesRemaining, BYTES_PER_SECTOR)
                    If BlockSize > 0 Then
                        Dim Cluster = _Disk.BootSector.SectorToCluster(Sector)
                        If _Disk.FAT.FileAllocation.ContainsKey(Cluster) Then
                            HighlightedRegions.AddItem(BlockOffset, BlockSize, Color.Red)
                        End If
                    End If
                    BytesRemaining -= BlockSize
                Next
            End If

            If SectorBlock.Size > FileSize Then
                Dim Start = FileSize
                Dim Size = SectorBlock.Size - FileSize
                If Size > 0 Then
                    HighlightedRegions.AddItem(Start, Size, Color.DarkGray)
                End If
            End If

            FileSize -= Math.Min(FileSize, SectorBlock.Size)
        Next
    End Sub
    Private Sub ImageCountUpdate()
        If _FiltersApplied Then
            ToolStripImageCount.Text = ComboImagesFiltered.Items.Count & " Of " & ComboImages.Items.Count & " Image" & IIf(ComboImages.Items.Count <> 1, "s", "")
        Else
            ToolStripImageCount.Text = ComboImages.Items.Count & " Image" & IIf(ComboImages.Items.Count <> 1, "s", "")
        End If
    End Sub

    Private Sub InitButtonState()
        If _Disk IsNot Nothing AndAlso Not _Disk.LoadError Then
            BtnChangeOEMName.Enabled = _Disk.IsValidImage
            BtnDisplayBootSector.Enabled = True
            BtnDisplayDisk.Enabled = True
            BtnDisplayFAT.Enabled = _Disk.IsValidImage
            BtnDisplayDirectory.Enabled = _Disk.IsValidImage
        Else
            BtnChangeOEMName.Enabled = False
            BtnDisplayBootSector.Enabled = False
            BtnDisplayDisk.Enabled = False
            BtnDisplayFAT.Enabled = False
            BtnDisplayDirectory.Enabled = False
        End If
        BtnWin9xClean.Enabled = False

        MenuDisplayDirectorySubMenuClear()

        SaveButtonsRefresh()
    End Sub

    Private Sub ItemScanAll(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        ItemScanModified(Disk, ImageData, UpdateFilters, Remove)

        If ImageData.Scanned Then
            ItemScanDisk(Disk, ImageData, UpdateFilters, Remove)
            ItemScanOEMName(Disk, ImageData, UpdateFilters, Remove)
            ItemScanUnusedClusters(Disk, ImageData, UpdateFilters, Remove)
            ItemScanDirectory(Disk, ImageData, UpdateFilters, Remove)
        End If
    End Sub

    Private Sub ItemSelectionChanged()
        RefreshFileButtons()
        FileCountUpdate()
        RefreshCheckAll()
    End Sub

    Private Function ListViewFilesGetItem(DirectoryEntry As DiskImage.DirectoryEntry, Group As ListViewGroup, LFNFileName As String, FileData As FileData) As ListViewItem
        Dim SI As ListViewItem.ListViewSubItem
        Dim ForeColor As Color
        Dim IsDeleted As Boolean = DirectoryEntry.IsDeleted
        Dim IsVolumeName As Boolean = DirectoryEntry.IsVolumeName
        Dim IsDirectory As Boolean = DirectoryEntry.IsDirectory
        Dim HasInvalidFileSize As Boolean = DirectoryEntry.HasInvalidFileSize

        Dim Attrib As String = IIf(DirectoryEntry.IsArchive, "A ", "- ") _
            & IIf(DirectoryEntry.IsReadOnly, "R ", "- ") _
            & IIf(DirectoryEntry.IsSystem, "S ", "- ") _
            & IIf(DirectoryEntry.IsHidden, "H ", "- ") _
            & IIf(IsDirectory, "D ", "- ") _
            & IIf(IsVolumeName, "V ", "- ")

        If IsDeleted Then
            ForeColor = Color.Gray
        ElseIf IsVolumeName Then
            ForeColor = Color.Green
        ElseIf IsDirectory Then
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

        SI = Item.SubItems.Add(DirectoryEntry.GetFileName)
        If Not IsDeleted And DirectoryEntry.HasInvalidFilename Then
            SI.ForeColor = Color.Red
        Else
            SI.ForeColor = ForeColor
        End If

        SI = Item.SubItems.Add(DirectoryEntry.GetFileExtension)
        If Not IsDeleted And DirectoryEntry.HasInvalidExtension Then
            SI.ForeColor = Color.Red
        Else
            SI.ForeColor = ForeColor
        End If

        If HasInvalidFileSize Then
            SI = Item.SubItems.Add("Invalid")
            If Not IsDeleted Then
                SI.ForeColor = Color.Red
            Else
                SI.ForeColor = ForeColor
            End If
        ElseIf Not IsDeleted And DirectoryEntry.HasIncorrectFileSize Then
            SI = Item.SubItems.Add(Format(DirectoryEntry.FileSize, "N0"))
            SI.ForeColor = Color.Red
        Else
            SI = Item.SubItems.Add(Format(DirectoryEntry.FileSize, "N0"))
            SI.ForeColor = ForeColor
        End If

        SI = Item.SubItems.Add(ExpandedDateToString(DirectoryEntry.GetLastWriteDate, True, True, False, True))
        If DirectoryEntry.GetLastWriteDate.IsValidDate Or IsDeleted Then
            SI.ForeColor = ForeColor
        Else
            SI.ForeColor = Color.Red
        End If

        Dim ErrorText As String = ""
        Dim SubItemForeColor As Color = ForeColor
        If DirectoryEntry.HasInvalidStartingCluster Then
            SI = Item.SubItems.Add("Invalid")
            If Not IsDeleted Then
                SubItemForeColor = Color.Red
            End If
        Else
            SI = Item.SubItems.Add(Format(DirectoryEntry.StartingCluster, "N0"))
        End If

        If Not IsDeleted And DirectoryEntry.IsCrossLinked Then
            SubItemForeColor = Color.Red
            ErrorText = "CL"
        ElseIf Not IsDeleted And DirectoryEntry.HasCircularChain Then
            SubItemForeColor = Color.Red
            ErrorText = "CC"
        End If
        SI.ForeColor = SubItemForeColor
        SI.Name = "FileStartingCluster"

        SI = Item.SubItems.Add(ErrorText)
        SI.Name = "FileClusterError"
        SI.ForeColor = Color.Red

        SI = Item.SubItems.Add(Attrib)
        If Not IsDeleted And DirectoryEntry.HasInvalidAttributes Then
            SI.ForeColor = Color.Red
        Else
            SI.ForeColor = ForeColor
        End If

        If Not IsDeleted And Not IsDirectory And Not IsVolumeName And Not HasInvalidFileSize Then
            SI = Item.SubItems.Add(Crc32.ComputeChecksum(DirectoryEntry.GetContent).ToString("X8"))
        Else
            SI = Item.SubItems.Add("")
        End If
        SI.ForeColor = ForeColor

        If DirectoryEntry.HasCreationDate Then
            SI = Item.SubItems.Add(ExpandedDateToString(DirectoryEntry.GetCreationDate, True, True, True, True))
            If DirectoryEntry.GetCreationDate.IsValidDate Or IsDeleted Then
                SI.ForeColor = ForeColor
            Else
                SI.ForeColor = Color.Red
            End If
        Else
            Item.SubItems.Add("")
        End If

        If DirectoryEntry.HasLastAccessDate Then
            SI = Item.SubItems.Add(ExpandedDateToString(DirectoryEntry.GetLastAccessDate))
            If DirectoryEntry.GetLastAccessDate.IsValidDate Or IsDeleted Then
                SI.ForeColor = ForeColor
            Else
                SI.ForeColor = Color.Red
            End If
        Else
            Item.SubItems.Add("")
        End If

        SI = Item.SubItems.Add(LFNFileName)
        SI.ForeColor = ForeColor

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

    Private Function ListViewTileGetItem(Group As ListViewGroup, Name As String, Value As String) As ListViewItem
        Dim Item = New ListViewItem(Name, Group) With {
                .UseItemStyleForSubItems = False
            }
        Item.SubItems.Add(Value)

        Return Item
    End Function

    Private Function ListViewTileGetItem(Group As ListViewGroup, Name As String, Value As String, ForeColor As Color) As ListViewItem
        Dim Item = New ListViewItem(Name, Group) With {
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
                Return MediaDescriptor.ToString("X2") & " Hex"
        End Select
    End Function

    Private Sub MenuDisplayDirectorySubMenuClear()
        For Each Item As ToolStripMenuItem In BtnDisplayDirectory.DropDownItems
            RemoveHandler Item.Click, AddressOf BtnDisplayDirectory_Click
        Next
        BtnDisplayDirectory.DropDownItems.Clear()
        BtnDisplayDirectory.Text = "Root &Directory"
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

    Private Function MsgBoxOverwrite(FilePath As String) As MyMsgBoxResult
        Dim MSg As String = IO.Path.GetFileName(FilePath) & " already exists." & vbCrLf & "Do you want to replace it?"

        Dim SaveAllForm As New SaveAllForm(MSg)
        SaveAllForm.ShowDialog()
        Return SaveAllForm.Result
    End Function

    Private Function MsgBoxSave(FilePath As String) As MsgBoxResult
        Dim Msg As String = "Save file '" & IO.Path.GetFileName(FilePath) & "'?"
        Return MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNoCancel + MsgBoxStyle.DefaultButton3, "Save")
    End Function

    Private Function MsgBoxSaveAll(FilePath As String) As MyMsgBoxResult
        Dim Msg As String = "Save file '" & IO.Path.GetFileName(FilePath) & "'?"

        Dim SaveAllForm As New SaveAllForm(Msg)
        SaveAllForm.ShowDialog()
        Return SaveAllForm.Result
    End Function

    Private Function OEMNameEdit() As Boolean
        Dim frmOEMName As New OEMNameForm(_Disk, _OEMNameDictionary)
        Dim Result As Boolean

        frmOEMName.ShowDialog()

        Result = frmOEMName.DialogResult = DialogResult.OK

        If Result Then
            ComboItemRefresh(True, True)
        End If

        Return Result
    End Function

    Private Sub FixImageSize()
        Dim Msg As String = "Any modifications you have made to the disk image will be saved at this time.  This process cannot be undone." _
            & vbCrLf & vbCrLf & "Are you sure you wish to resize the disk image?"

        If MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
            If _Disk.FixImageSize Then
                ComboItemRefresh(False, True)
                SaveCurrent(False)
            End If
        End If
    End Sub

    Private Sub Win9xClean()
        Dim Result As Boolean = False

        If _Disk.BootSector.IsWin9xOEMName Then
            Dim BootstrapChecksum = Crc32.ComputeChecksum(_Disk.BootSector.BootStrapCode)
            If _OEMNameDictionary.ContainsKey(BootstrapChecksum) Then
                Dim BootstrapType = _OEMNameDictionary.Item(BootstrapChecksum)
                If BootstrapType.KnownOEMNames.Count > 0 Then
                    _Disk.BootSector.OEMName = BootstrapType.KnownOEMNames.Item(0).Name
                    Result = True
                End If
            End If
        End If

        For Each Item As ListViewItem In ListViewFiles.Items
            Dim FileData As FileData = Item.Tag
            If FileData.DirectoryEntry.HasLastAccessDate Then
                FileData.DirectoryEntry.ClearLastAccessDate()
                Result = True
            End If
            If FileData.DirectoryEntry.HasCreationDate Then
                FileData.DirectoryEntry.ClearCreationDate()
                Result = True
            End If
        Next

        If Result Then
            ComboItemRefresh(True, True)
        End If
    End Sub

    Private Function OEMNameFindMatch(Checksum As UInteger) As BootstrapLookup
        If _OEMNameDictionary.ContainsKey(Checksum) Then
            Return _OEMNameDictionary.Item(Checksum)
        Else
            Return Nothing
        End If
    End Function

    Private Sub PopulateComboOEMName()
        Dim Item As ComboOEMNameItem

        ComboOEMName.BeginUpdate()
        ComboOEMName.Items.Clear()
        ComboOEMName.Sorted = True
        For Each Item In _FilterOEMName.Values
            ComboOEMName.Items.Add(Item)
        Next
        Item = New ComboOEMNameItem With {
            .AllItems = True
        }
        ComboOEMName.Sorted = False
        ComboOEMName.Items.Insert(0, Item)
        ComboOEMName.SelectedIndex = 0
        ComboOEMName.EndUpdate()
        ComboOEMName.Visible = True
        ToolStripOEMName.Visible = True
    End Sub

    Private Sub PopulateFilesPanel()
        ListViewFiles.BeginUpdate()

        ListViewFiles.Items.Clear()
        ListViewFiles.MultiSelect = True

        Dim Items As New List(Of ListViewItem)
        Dim Response As ProcessDirectoryEntryResponse = ProcessDirectoryEntries(_Disk.Directory, "", False)

        If BtnDisplayDirectory.DropDownItems.Count > 0 Then
            BtnDisplayDirectory.Text = "Directory"
            MenuDisplayDirectorySubMenuItemAdd("(Root)", 0, 0)
            BtnDisplayDirectory.Tag = Nothing
        Else
            BtnDisplayDirectory.Tag = 0
        End If

        If Response.HasCreated Then
            FileCreateDate.Width = 140
        Else
            FileCreateDate.Width = 0
        End If

        If Response.HasLastAccessed Then
            FileLastAccessDate.Width = 90
        Else
            FileLastAccessDate.Width = 0
        End If

        If Response.HasLFN Then
            FileLFN.Width = 200
        Else
            FileLFN.Width = 0
        End If

        If Response.HasFATChainingErrors Then
            FileClusterError.Width = 30
            RefreshClusterErrors()
        Else
            FileClusterError.Width = 0
        End If

        BtnWin9xClean.Enabled = Response.HasCreated Or Response.HasLastAccessed Or _Disk.BootSector.IsWin9xOEMName

        ListViewFiles.EndUpdate()

        ItemSelectionChanged()
    End Sub

    Private Sub PopulateHashes()
        ListViewHashes.BeginUpdate()
        With ListViewHashes.Items
            .Clear()
            If Not _Disk.LoadError Then
                .Add(ListViewTileGetItem("CRC32", Crc32.ComputeChecksum(_Disk.Data.Data).ToString("X8")))
                .Add(ListViewTileGetItem("MD5", MD5Hash(_Disk.Data.Data)))
                .Add(ListViewTileGetItem("SHA-1", SHA1Hash(_Disk.Data.Data)))
            End If
        End With
        ListViewHashes.EndUpdate()
        ListViewHashes.Refresh()
    End Sub

    Private Sub PopulateSummaryPanel()
        Dim Value As String
        Dim ForeColor As Color

        Me.Text = GetWindowCaption() & " - " & IO.Path.GetFileName(_Disk.FilePath)

        ToolStripFileName.Text = IO.Path.GetFileName(_Disk.FilePath)
        ToolStripFileName.Visible = True

        ListViewSummary.BeginUpdate()

        With ListViewSummary.Items
            .Clear()

            Dim DiskGroup = ListViewSummary.Groups.Add("Disk", "Disk")
            If Not _Disk.LoadError Then
                If _Disk.IsValidImage AndAlso _Disk.HasInvalidSize Then
                    ForeColor = Color.Red
                Else
                    ForeColor = SystemColors.WindowText
                End If
                .Add(ListViewTileGetItem(DiskGroup, "Image Size", _Disk.Data.Length.ToString("N0"), ForeColor))
            End If
            If _Disk.IsValidImage Then
                ToolStripStatusModified.Visible = _Disk.Data.Modified
                Dim BootStrapStart = _Disk.BootSector.GetBootStrapOffset
                Dim CopyProtection As String = GetCopyProtection(_Disk)
                If CopyProtection.Length > 0 Then
                    .Add(ListViewTileGetItem(DiskGroup, "Copy Protection", CopyProtection))
                End If

                Dim BroderbundCopyright = GetBroderbundCopyright(_Disk)
                If BroderbundCopyright <> "" Then
                    .Add(ListViewTileGetItem(DiskGroup, "Broderbund Copyright", BroderbundCopyright))
                End If

                Dim BootstrapChecksum = Crc32.ComputeChecksum(_Disk.BootSector.BootStrapCode)
                Dim OEMName = _Disk.BootSector.OEMName
                Dim KnownOEMNameMatch As KnownOEMName = Nothing
                Dim OEMNameWin9x = _Disk.BootSector.IsWin9xOEMName

                Dim BootstrapType = OEMNameFindMatch(BootstrapChecksum)
                If BootstrapType IsNot Nothing Then
                    For Each KnownOEMName In BootstrapType.KnownOEMNames
                        If ByteArrayCompare(KnownOEMName.Name, OEMName) Or (OEMNameWin9x And KnownOEMName.Win9xId) Then
                            KnownOEMNameMatch = KnownOEMName
                            Exit For
                        End If
                    Next
                    If KnownOEMNameMatch Is Nothing And BootstrapType.ExactMatch Then
                        BootstrapType = Nothing
                    End If
                End If

                Dim BootRecordGroup = ListViewSummary.Groups.Add("BootRecord", "Boot Record")

                If BootstrapType IsNot Nothing Then
                    If KnownOEMNameMatch Is Nothing Then
                        ForeColor = Color.Red
                    Else
                        ForeColor = Color.Green
                    End If
                Else
                    ForeColor = SystemColors.WindowText
                End If
                .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.OEMName), _Disk.BootSector.GetOEMNameString.TrimEnd(NULL_CHAR), ForeColor))
                .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.BytesPerSector), _Disk.BootSector.BytesPerSector))
                .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.SectorsPerCluster), _Disk.BootSector.SectorsPerCluster))
                .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.ReservedSectorCount), _Disk.BootSector.ReservedSectorCount))

                Value = _Disk.BootSector.NumberOfFATs
                If Not _Disk.FAT.CompareTables Then
                    Value &= " (Mismatched)"
                    ForeColor = Color.Red
                Else
                    ForeColor = SystemColors.WindowText
                End If
                .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.NumberOfFATs), Value, ForeColor))

                .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.RootEntryCount), _Disk.BootSector.RootEntryCount))
                .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.SectorCountSmall), _Disk.BootSector.SectorCount))
                .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.MediaDescriptor), MediaTypeGet(_Disk.BootSector.MediaDescriptor, _Disk.BootSector.SectorsPerTrack)))
                .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.SectorsPerFAT), _Disk.BootSector.SectorsPerFAT))
                .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.SectorsPerTrack), _Disk.BootSector.SectorsPerTrack))
                .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.NumberOfHeads), _Disk.BootSector.NumberOfHeads))
                If _Disk.BootSector.HiddenSectors > 0 Then
                    .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.HiddenSectors), _Disk.BootSector.HiddenSectors))
                End If
                If BootStrapStart >= BootSectorOffsets.BootStrapCode Then
                    If _Disk.BootSector.DriveNumber > 0 Then
                        .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.DriveNumber), _Disk.BootSector.DriveNumber))
                    End If
                    If _Disk.BootSector.HasValidExtendedBootSignature Then
                        .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.VolumeSerialNumber), _Disk.BootSector.VolumeSerialNumber.ToString("X8").Insert(4, "-")))
                        .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.VolumeLabel), _Disk.BootSector.GetVolumeLabelString.TrimEnd(NULL_CHAR)))
                        .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.FileSystemType), Encoding.UTF8.GetString(_Disk.BootSector.FileSystemType)))
                    End If
                End If
                If _Disk.BootSector.BootStrapSignature <> &HAA55 Then
                    .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.BootStrapSignature), _Disk.BootSector.BootStrapSignature.ToString("X4")))
                End If
                If Not _Disk.BootSector.HasValidJumpInstruction Then
                    ForeColor = Color.Red
                Else
                    ForeColor = SystemColors.WindowText
                End If
                If Debugger.IsAttached Then
                    .Add(ListViewTileGetItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.JmpBoot), BitConverter.ToString(_Disk.BootSector.JmpBoot), ForeColor))
                End If

                Dim FileSystemGroup = ListViewSummary.Groups.Add("FileSystem", "File System")

                Dim VolumeLabel = _Disk.GetVolumeLabel
                If VolumeLabel IsNot Nothing Then
                    .Add(ListViewTileGetItem(FileSystemGroup, "Volume Label", VolumeLabel.GetVolumeName.TrimEnd(NULL_CHAR)))
                    Dim VolumeDate = VolumeLabel.GetLastWriteDate
                    .Add(ListViewTileGetItem(FileSystemGroup, "Volume Date", ExpandedDateToString(VolumeDate, True, False, False, False)))
                End If

                .Add(ListViewTileGetItem(FileSystemGroup, "Total Space", Format(SectorToBytes(_Disk.BootSector.DataRegionSize), "N0") & " bytes"))
                .Add(ListViewTileGetItem(FileSystemGroup, "Free Space", Format(_Disk.FAT.FreeSpace, "N0") & " bytes"))

                If _Disk.FAT.BadClusters.Count > 0 Then
                    Value = Format(_Disk.FAT.BadClusters.Count * _Disk.BootSector.BytesPerCluster, "N0") & " bytes"
                    ForeColor = Color.Red
                    .Add(ListViewTileGetItem(FileSystemGroup, "Bad Sectors", Value, ForeColor))
                End If

                Dim BootStrapGroup = ListViewSummary.Groups.Add("Bootstrap", "Bootstrap")

                If Debugger.IsAttached Then
                    .Add(ListViewTileGetItem(BootStrapGroup, "Bootstrap CRC32", Crc32.ComputeChecksum(_Disk.BootSector.BootStrapCode).ToString("X8")))
                End If

                If BootstrapType IsNot Nothing Then
                    If BootstrapType.Language.Length > 0 Then
                        .Add(ListViewTileGetItem(BootStrapGroup, "Language", BootstrapType.Language))
                    End If
                    If KnownOEMNameMatch Is Nothing And BootstrapType.KnownOEMNames.Count = 1 Then
                        KnownOEMNameMatch = BootstrapType.KnownOEMNames(0)
                    End If
                    If KnownOEMNameMatch IsNot Nothing Then
                        If KnownOEMNameMatch.Company <> "" Then
                            .Add(ListViewTileGetItem(BootStrapGroup, "Company", KnownOEMNameMatch.Company))
                        End If
                        If KnownOEMNameMatch.Description <> "" Then
                            .Add(ListViewTileGetItem(BootStrapGroup, "Description", KnownOEMNameMatch.Description))
                        End If
                    End If
                    If Not BootstrapType.ExactMatch Then
                        For Each KnownOEMName In BootstrapType.KnownOEMNames
                            If KnownOEMName.Name.Length > 0 AndAlso KnownOEMName.Suggestion AndAlso Not ByteArrayCompare(KnownOEMName.Name, OEMName) Then
                                .Add(ListViewTileGetItem(BootStrapGroup, "Alternative OEM Name", KnownOEMName.GetNameAsString))
                            End If
                        Next
                    End If
                End If
            Else
                ToolStripStatusModified.Visible = False

                If _Disk.LoadError Then
                    .Add(ListViewTileGetItem(DiskGroup, "Error", "Error Loading File", Color.Red))
                Else
                    .Add(ListViewTileGetItem(DiskGroup, "Error", "Unknown Image Format", Color.Red))
                End If
            End If
        End With
        ListViewSummary.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
        ListViewSummary.EndUpdate()
        ListViewSummary.Refresh()

        PopulateHashes()

        If _Disk.IsValidImage Then
            BtnDisplayClusters.Enabled = _Disk.FAT.HasUnusedSectors(True)
            BtnDisplayBadSectors.Enabled = _Disk.FAT.BadClusters.Count > 0
            BtnFixImageSize.Enabled = _Disk.HasInvalidSize
        Else
            BtnDisplayClusters.Enabled = False
            BtnDisplayBadSectors.Enabled = False
            BtnFixImageSize.Enabled = False
        End If

        BtnRevert.Enabled = Not _Disk.LoadError AndAlso _Disk.Data.Modified

        BtnUndo.Enabled = Not _Disk.LoadError AndAlso _Disk.Data.UndoEnabled
        ToolStripBtnUndo.Enabled = BtnUndo.Enabled

        BtnRedo.Enabled = Not _Disk.LoadError AndAlso _Disk.Data.RedoEnabled
        ToolStripBtnRedo.Enabled = BtnRedo.Enabled
    End Sub

    Private Sub PositionForm()
        Dim WorkingArea = Screen.FromControl(Me).WorkingArea
        Dim Width As Integer = Me.Width
        Dim Height As Integer = Me.Height

        If My.Settings.WindowWidth > 0 Then
            Width = My.Settings.WindowWidth
        End If
        If My.Settings.WindowHeight > 0 Then
            Height = My.Settings.WindowHeight
        End If

        Width = Math.Min(Width, WorkingArea.Width)
        Height = Math.Min(Height, WorkingArea.Height)

        Me.Size = New Size(Width, Height)
        Me.Location = New Point(WorkingArea.Left + (WorkingArea.Width - Width) / 2, WorkingArea.Top + (WorkingArea.Height - Height) / 2)
    End Sub

    Private Function ProcessDirectoryEntries(Directory As DiskImage.IDirectory, Path As String, ScanOnly As Boolean) As ProcessDirectoryEntryResponse
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
            .HasFATChainingErrors = False
        End With

        If Not ScanOnly Then
            Dim GroupName As String = IIf(Path = "", "(Root)", Path)
            GroupName = GroupName & "  (" & FileCount & IIf(FileCount <> 1, " entries", " entry") & ")"
            Group = New ListViewGroup(GroupName)
            ListViewFiles.Groups.Add(Group)
        End If

        Dim DirectoryEntryCount = Directory.DirectoryEntryCount

        If DirectoryEntryCount > 0 Then
            For Counter = 0 To DirectoryEntryCount - 1
                Dim File = Directory.GetFile(Counter)
                Dim FullFileName = File.GetFullFileName
                Dim IsLastEntry = (Counter = DirectoryEntryCount - 1)
                Dim FileData = GetFileDataFromDirectoryEntry(File, Path, IsLastEntry)

                If Not File.IsLink Then
                    If File.IsLFN Then
                        LFNFileName = File.GetLFNFileName & LFNFileName
                    Else
                        If Not ScanOnly Then
                            Dim item = ListViewFilesGetItem(File, Group, LFNFileName, FileData)
                            ListViewFiles.Items.Add(item)
                        End If

                        If Not Response.HasInvalidDirectoryEntries Then
                            If Not File.IsDeleted Then
                                If File.HasInvalidFilename _
                                Or File.HasInvalidExtension _
                                Or File.HasInvalidFileSize _
                                Or File.HasIncorrectFileSize _
                                Or File.HasInvalidAttributes _
                                Or File.HasInvalidStartingCluster _
                                Or Not File.GetLastWriteDate.IsValidDate Then
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

                        If Not Response.HasFATChainingErrors Then
                            If File.HasCircularChain Or File.IsCrossLinked Then
                                Response.HasFATChainingErrors = True
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
                        If File.SubDirectory.DirectoryEntryCount > 0 Then
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
                            Response.HasFATChainingErrors = Response.HasFATChainingErrors Or SubResponse.HasFATChainingErrors
                        End If
                    End If
                End If
            Next
        End If

        Return Response
    End Function

    Private Sub ProcessFileDrop(File As String)
        Dim Files(0) As String
        Files(0) = File

        ProcessFileDrop(Files)
    End Sub

    Private Sub ProcessFileDrop(Files() As String)
        Dim AllowedExtensions = {".img", ".ima"}
        Dim FilePath As String
        Dim FileInfo As IO.FileInfo
        Dim SelectedImageData As LoadedImageData = Nothing

        Me.UseWaitCursor = True

        If _FiltersApplied Then
            FiltersClear()
        End If

        ComboImages.BeginUpdate()

        LoadedImageData.StringOffset = 0

        For Each FilePath In Files.OrderBy(Function(f) f)
            Dim FAttributes = IO.File.GetAttributes(FilePath)
            If (FAttributes And IO.FileAttributes.Directory) > 0 Then
                Dim DirectoryInfo As New IO.DirectoryInfo(FilePath)
                For Each FileInfo In DirectoryInfo.GetFiles("*.*", IO.SearchOption.AllDirectories)
                    If Not _LoadedFileNames.ContainsKey(FileInfo.FullName) Then
                        Dim ImageData As New LoadedImageData(FileInfo.FullName)
                        _LoadedFileNames.Add(FileInfo.FullName, ImageData)
                        ComboImages.Items.Add(ImageData)
                        If SelectedImageData Is Nothing Then
                            SelectedImageData = ImageData
                        End If
                    End If
                Next
            Else
                If Not _LoadedFileNames.ContainsKey(FilePath) Then
                    Dim ImageData As New LoadedImageData(FilePath)
                    _LoadedFileNames.Add(FilePath, ImageData)
                    ComboImages.Items.Add(ImageData)
                    If SelectedImageData Is Nothing Then
                        SelectedImageData = ImageData
                    End If
                End If
            End If
        Next

        LoadedImageData.StringOffset = GetPathOffset()

        If SelectedImageData IsNot Nothing Then
            ComboImages.Enabled = True
            ComboImagesRefreshItemText()
            ImageCountUpdate()

            ComboImages.SelectedItem = SelectedImageData
            If ComboImages.SelectedIndex = -1 Then
                ComboImages.SelectedIndex = 0
            End If

            SetImagesLoaded(True)
        End If

        ComboImages.EndUpdate()

        Me.UseWaitCursor = False
    End Sub

    Private Sub RefreshCheckAll()
        Dim CheckAll = (ListViewFiles.SelectedItems.Count = ListViewFiles.Items.Count And ListViewFiles.Items.Count > 0)
        If CheckAll <> _CheckAll Then
            _CheckAll = CheckAll
            ListViewFiles.Invalidate(New Rectangle(0, 0, 20, 20), True)
        End If
    End Sub

    Private Sub RefreshClusterErrors()
        For Each Item As ListViewItem In ListViewFiles.Items
            Dim FileData As FileData = Item.Tag
            If FileData.DirectoryEntry.IsCrossLinked Then
                Item.SubItems.Item("FileStartingCluster").ForeColor = Color.Red
                Item.SubItems.Item("FileClusterError").Text = "CL"
            End If
        Next
    End Sub

    Private Sub RefreshFileButtons()
        Dim Stats As DirectoryStats

        If ListViewFiles.SelectedItems.Count = 0 Then
            BtnExportFile.Text = "&Export File"
            BtnExportFile.Enabled = False

            BtnReplaceFile.Enabled = False
            BtnFileProperties.Enabled = False
            BtnFileMenuViewCrosslinked.Visible = False

            BtnFileMenuViewFileText.Visible = False
            BtnFileMenuViewFileText.Enabled = False

            BtnDisplayFile.Tag = Nothing
            BtnDisplayFile.Visible = False

            BtnFileMenuViewFile.Text = "&View File"
            BtnFileMenuViewFile.Enabled = False

            BtnFileMenuRemoveDeletedFile.Visible = False
            BtnFileMenuRemoveDeletedFile.Enabled = False

            BtnFileMenuDeleteFile.Visible = True
            BtnFileMenuDeleteFile.Enabled = False

        ElseIf ListViewFiles.SelectedItems.Count = 1 Then
            Dim FileData As FileData = ListViewFiles.SelectedItems(0).Tag
            Stats = DirectoryEntryGetStats(FileData.DirectoryEntry)

            BtnExportFile.Text = "&Export File"
            BtnExportFile.Enabled = Stats.CanExport

            BtnReplaceFile.Enabled = Stats.IsValidFile
            BtnFileProperties.Enabled = True
            BtnFileMenuViewCrosslinked.Visible = FileData.DirectoryEntry.IsCrossLinked

            BtnFileMenuViewFileText.Visible = Stats.IsValidFile
            BtnFileMenuViewFileText.Enabled = Stats.FileSize > 0

            If Stats.IsDeleted Then
                BtnFileMenuRemoveDeletedFile.Visible = True
                BtnFileMenuRemoveDeletedFile.Enabled = True
                BtnFileMenuDeleteFile.Visible = False
                BtnFileMenuDeleteFile.Enabled = False
                BtnFileMenuDeleteFileWithFill.Visible = False
                BtnFileMenuDeleteFileWithFill.Enabled = False
            Else
                BtnFileMenuRemoveDeletedFile.Visible = False
                BtnFileMenuRemoveDeletedFile.Enabled = False
                BtnFileMenuDeleteFile.Visible = True
                BtnFileMenuDeleteFile.Enabled = Stats.IsValid And Not Stats.IsDirectory
            End If

            If Stats.IsValid Then
                If Stats.IsDeleted Then
                    BtnDisplayFile.Text = "Deleted &File:  " & Stats.FullFileName
                Else
                    BtnDisplayFile.Text = "&File:  " & Stats.FullFileName
                End If
                BtnDisplayFile.Tag = FileData.DirectoryEntry.Offset
                BtnDisplayFile.Visible = Not Stats.IsDirectory And Stats.FileSize > 0

                Dim Caption As String = "&View "
                If Stats.IsDeleted Then
                    Caption &= "Deleted "
                End If
                If Stats.IsDirectory Then
                    Caption &= "Directory"
                Else
                    Caption &= "File"
                End If
                BtnFileMenuViewFile.Text = Caption
                BtnFileMenuViewFile.Enabled = Stats.IsDirectory Or Stats.FileSize > 0
            Else
                BtnDisplayFile.Tag = Nothing
                BtnDisplayFile.Visible = False

                BtnFileMenuViewFile.Text = "&View File"
                BtnFileMenuViewFile.Enabled = False
            End If
        Else
            Dim ExportEnabled As Boolean = False
            For Each Item As ListViewItem In ListViewFiles.SelectedItems
                Dim FileData As FileData = Item.Tag
                Stats = DirectoryEntryGetStats(FileData.DirectoryEntry)
                If Stats.CanExport Then
                    ExportEnabled = True
                End If
            Next
            BtnExportFile.Text = "&Export Selected Files"
            BtnExportFile.Enabled = ExportEnabled

            BtnReplaceFile.Enabled = False
            BtnFileProperties.Enabled = True
            BtnFileMenuViewCrosslinked.Visible = False

            BtnFileMenuViewFileText.Visible = True
            BtnFileMenuViewFileText.Enabled = False

            BtnDisplayFile.Tag = Nothing
            BtnDisplayFile.Visible = False

            BtnFileMenuViewFile.Text = "&View File"
            BtnFileMenuViewFile.Enabled = False

            BtnFileMenuRemoveDeletedFile.Visible = False
            BtnFileMenuRemoveDeletedFile.Enabled = False

            BtnFileMenuDeleteFile.Visible = True
            BtnFileMenuDeleteFile.Enabled = False
        End If

        BtnFileMenuExportFile.Text = BtnExportFile.Text
        BtnFileMenuExportFile.Enabled = BtnExportFile.Enabled
        ToolStripBtnExportFile.Enabled = BtnExportFile.Enabled
        BtnFileMenuReplaceFile.Enabled = BtnReplaceFile.Enabled
        BtnFileMenuFileProperties.Enabled = BtnFileProperties.Enabled
        ToolStripBtnFileProperties.Enabled = BtnFileProperties.Enabled
        ToolStripBtnViewFile.Text = BtnFileMenuViewFile.Text
        ToolStripBtnViewFile.Enabled = BtnFileMenuViewFile.Enabled
        ToolStripBtnViewFileText.Enabled = BtnFileMenuViewFileText.Enabled
        BtnFileMenuDeleteFileWithFill.Visible = BtnFileMenuDeleteFile.Enabled
        BtnFileMenuDeleteFileWithFill.Enabled = BtnFileMenuDeleteFile.Enabled
    End Sub

    Private Sub ResetAll()
        _Disk = Nothing
        Me.Text = GetWindowCaption()
        _FiltersApplied = False
        _CheckAll = False
        _LoadedFileNames.Clear()
        _ScanRun = False
        BtnDisplayClusters.Enabled = False
        BtnDisplayBadSectors.Enabled = False
        BtnRevert.Enabled = False
        BtnFixImageSize.Enabled = False

        BtnUndo.Enabled = False
        ToolStripBtnUndo.Enabled = False

        BtnRedo.Enabled = False
        ToolStripBtnRedo.Enabled = False

        ToolStripFileName.Visible = False
        ToolStripModified.Visible = False
        ToolStripStatusModified.Visible = False
        ToolStripFileCount.Visible = False

        BtnSaveAll.Enabled = False
        ToolStripBtnSaveAll.Enabled = BtnSaveAll.Enabled

        ComboImages.Enabled = False
        ComboImages.Visible = True
        ComboImagesFiltered.Visible = False
        ListViewSummary.Items.Clear()
        ListViewHashes.Items.Clear()

        ListViewFiles.BeginUpdate()
        ListViewFiles.Items.Clear()
        ListViewFiles.Groups.Clear()
        ListViewFiles.MultiSelect = False
        ListViewFiles.EndUpdate()

        ComboImages.Items.Clear()
        ComboImagesFiltered.Items.Clear()
        MainMenuFilters.BackColor = SystemColors.Control
        RefreshFileButtons()
        SetImagesLoaded(False)
        FiltersReset()
        InitButtonState()
    End Sub

    Private Sub RevertChanges()
        If _Disk.Data.Modified Then
            Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem
            CurrentImageData.Modifications.Clear()
            DiskImageLoad(True)
        End If
    End Sub

    Private Sub SaveAll()
        _SuppressEvent = True
        For Index = 0 To ComboImages.Items.Count - 1
            Dim ImageData As LoadedImageData = ComboImages.Items(Index)
            If ImageData.Modified Then
                Dim Result = DiskImageSave(ImageData)
                If Result Then
                    If ImageData Is ComboImages.SelectedItem Then
                        ComboItemRefresh(True, False)
                    Else
                        ComboImagesRefreshItemText(ImageData)
                    End If
                End If
            End If
        Next Index
        _SuppressEvent = False

        FilterUpdate(FilterTypes.ModifiedFiles)
    End Sub

    Private Sub SaveButtonsRefresh()
        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem

        If CurrentImageData Is Nothing Then
            BtnSave.Enabled = False
            BtnSaveAs.Enabled = False
            BtnExportDebug.Enabled = False
        Else
            BtnSave.Enabled = CurrentImageData.Modified
            BtnSaveAs.Enabled = CurrentImageData.Modified
            'BtnExportDebug.Enabled = (CurrentImageData.Modified Or CurrentImageData.SessionModifications.Count > 0)
            BtnExportDebug.Enabled = False
        End If
        ToolStripBtnSave.Enabled = BtnSave.Enabled
        ToolStripBtnSaveAs.Enabled = BtnSaveAs.Enabled
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

        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem

        Dim Result = DiskImageSave(CurrentImageData, FilePath)
        If Result Then
            FilterUpdate(FilterTypes.ModifiedFiles)
            If NewFileName Then
                If _LoadedFileNames.ContainsKey(FilePath) Then
                    FileClose(_LoadedFileNames.Item(FilePath))
                End If
                _LoadedFileNames.Remove(CurrentImageData.FilePath)
                _LoadedFileNames.Add(FilePath, CurrentImageData)
                CurrentImageData.FilePath = FilePath
                ComboImagesRefreshPaths()
            End If
            ComboItemRefresh(True, False)
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

    Private Sub SetImagesLoaded(Value As Boolean)
        ToolStripImageCount.Visible = Value
        LabelDropMessage.Visible = Not Value
        BtnScan.Enabled = Value
        BtnScanNew.Enabled = Value
        BtnScanNew.Visible = _ScanRun
        BtnClose.Enabled = Value
        ToolStripBtnClose.Enabled = BtnClose.Enabled
        BtnCloseAll.Enabled = Value
        ToolStripBtnCloseAll.Enabled = BtnCloseAll.Enabled
        TxtSearch.Enabled = Value
    End Sub

    Private Sub UnusedClustersDisplayHex()
        Dim HexViewSectorData = New HexViewSectorData(_Disk, _Disk.FAT.GetUnusedSectors(True))

        If DisplayHexViewForm(HexViewSectorData, "Unused Clusters", False, False) Then
            ComboItemRefresh(False, True)
        End If
    End Sub

    Private Structure DirectoryStats
        Dim CanExport As Boolean
        Dim FileSize As UInteger
        Dim FullFileName As String
        Dim IsDeleted As Boolean
        Dim IsDirectory As Boolean
        Dim IsModified As Boolean
        Dim IsValid As Boolean
        Dim IsValidFile As Boolean
    End Structure

    Friend Structure ProcessDirectoryEntryResponse
        Dim HasCreated As Boolean
        Dim HasFATChainingErrors As Boolean
        Dim HasInvalidDirectoryEntries As Boolean
        Dim HasLastAccessed As Boolean
        Dim HasLFN As Boolean
    End Structure

    Private Class ComboOEMNameItem
        Public Property AllItems As Boolean = False
        Public Property Count As Integer
        Public Property Name As String

        Public Overrides Function ToString() As String
            If AllItems Then
                Return "(ALL)"
            Else
                Return Name & "  [" & _Count & "]"
            End If
        End Function

    End Class

#Region "Events"

    Private Sub BtnClearFilters_Click(sender As Object, e As EventArgs) Handles BtnClearFilters.Click
        If _FiltersApplied Then
            FiltersClear()
            ImageCountUpdate()
            ContextMenuFilters.Invalidate()
        End If
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click, ToolStripBtnClose.Click
        CloseCurrent()
    End Sub

    Private Sub BtnCloseAll_Click(sender As Object, e As EventArgs) Handles BtnCloseAll.Click, ToolStripBtnCloseAll.Click
        CloseAll()
    End Sub

    Private Sub BtnDisplayBadSectors_Click(sender As Object, e As EventArgs) Handles BtnDisplayBadSectors.Click
        BadSectorsDisplayHex()
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

    Private Sub BtnDisplayDisk_Click(sender As Object, e As EventArgs) Handles BtnDisplayDisk.Click
        DiskImageDisplayHex()
    End Sub

    Private Sub BtnDisplayFAT_Click(sender As Object, e As EventArgs) Handles BtnDisplayFAT.Click
        FATDisplayHex()
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

    Private Sub BtnExportDebug_Click(sender As Object, e As EventArgs) Handles BtnExportDebug.Click
        ExportDebugScript()
    End Sub

    Private Sub BtnExportFile_Click(sender As Object, e As EventArgs) Handles BtnExportFile.Click, BtnFileMenuExportFile.Click, ToolStripBtnExportFile.Click
        FileExport()
    End Sub

    Private Sub BtnFileMenuDeleteFile_Click(sender As Object, e As EventArgs) Handles BtnFileMenuDeleteFile.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim Item As ListViewItem = ListViewFiles.SelectedItems(0)
            Dim FileData As FileData = Item.Tag

            If Not FileData.DirectoryEntry.IsDeleted And Not FileData.DirectoryEntry.IsDirectory Then
                DeleteFile(FileData.DirectoryEntry, False)
            End If
        End If
    End Sub

    Private Sub BtnFileMenuDeleteFileWithFill_Click(sender As Object, e As EventArgs) Handles BtnFileMenuDeleteFileWithFill.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim Item As ListViewItem = ListViewFiles.SelectedItems(0)
            Dim FileData As FileData = Item.Tag

            If Not FileData.DirectoryEntry.IsDeleted And Not FileData.DirectoryEntry.IsDirectory Then
                DeleteFile(FileData.DirectoryEntry, True)
            End If
        End If
    End Sub

    Private Sub BtnFileMenuRemoveDeletedFile_Click(sender As Object, e As EventArgs) Handles BtnFileMenuRemoveDeletedFile.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim Item As ListViewItem = ListViewFiles.SelectedItems(0)
            Dim FileData As FileData = Item.Tag

            If FileData.DirectoryEntry.IsDeleted Then
                RemoveDeletedFile(FileData.DirectoryEntry, FileData.IsLastEntry)
            End If
        End If
    End Sub

    Private Sub BtnFileMenuViewCrosslinked_Click(sender As Object, e As EventArgs) Handles BtnFileMenuViewCrosslinked.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim Item As ListViewItem = ListViewFiles.SelectedItems(0)
            Dim FileData As FileData = Item.Tag
            DisplayCrossLinkedFiles(FileData.DirectoryEntry)
        End If
    End Sub

    Private Sub BtnFileMenuViewFile_Click(sender As Object, e As EventArgs) Handles BtnFileMenuViewFile.Click, ToolStripBtnViewFile.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim Item As ListViewItem = ListViewFiles.SelectedItems(0)
            Dim FileData As FileData = Item.Tag
            DirectoryEntryDisplayHex(FileData.DirectoryEntry.Offset)
        End If
    End Sub

    Private Sub BtnFileMenuViewFileText_Click(sender As Object, e As EventArgs) Handles BtnFileMenuViewFileText.Click, ToolStripBtnViewFileText.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim Item As ListViewItem = ListViewFiles.SelectedItems(0)
            Dim FileData As FileData = Item.Tag
            DirectoryEntryDisplayText(FileData.DirectoryEntry)
        End If
    End Sub

    Private Sub BtnFileProperties_Click(sender As Object, e As EventArgs) Handles BtnFileProperties.Click, BtnFileMenuFileProperties.Click, ToolStripBtnFileProperties.Click
        FilePropertiesEdit()
    End Sub

    Private Sub BtnOOEMName_Click(sender As Object, e As EventArgs) Handles BtnChangeOEMName.Click
        OEMNameEdit()
    End Sub

    Private Sub BtnFixImageSize_Click(sender As Object, e As EventArgs) Handles BtnFixImageSize.Click
        FixImageSize()
    End Sub

    Private Sub BtnWin9xClean_Click(sender As Object, e As EventArgs) Handles BtnWin9xClean.Click
        Win9xClean()
    End Sub

    Private Sub BtnOpen_Click(sender As Object, e As EventArgs) Handles BtnOpen.Click, ToolStripBtnOpen.Click
        FilesOpen()
    End Sub

    Private Sub BtnRedo_Click(sender As Object, e As EventArgs) Handles BtnRedo.Click, ToolStripBtnRedo.Click
        _Disk.Data.Redo()
        If _Disk.ReinitializeRequired Then
            _Disk.Reinitialize()
        End If
        ComboItemRefresh(True, True)
    End Sub

    Private Sub BtnReplaceFile_Click(sender As Object, e As EventArgs) Handles BtnReplaceFile.Click, BtnFileMenuReplaceFile.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim Item As ListViewItem = ListViewFiles.SelectedItems(0)
            Dim FileData As FileData = Item.Tag
            FileReplace(FileData.DirectoryEntry)
        End If
    End Sub

    Private Sub BtnRevert_Click(sender As Object, e As EventArgs) Handles BtnRevert.Click
        RevertChanges()
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click, ToolStripBtnSave.Click
        SaveCurrent(False)
    End Sub

    Private Sub BtnSaveAll_Click(sender As Object, e As EventArgs) Handles BtnSaveAll.Click, ToolStripBtnSaveAll.Click
        SaveAll()
    End Sub

    Private Sub BtnSaveAs_Click(sender As Object, e As EventArgs) Handles BtnSaveAs.Click, ToolStripBtnSaveAs.Click
        SaveCurrent(True)
    End Sub

    Private Sub BtnScan_Click(sender As Object, e As EventArgs) Handles BtnScan.Click
        ContextMenuFilters.Close()
        DiskImagesScan(False)
    End Sub

    Private Sub BtnScanNew_Click(sender As Object, e As EventArgs) Handles BtnScanNew.Click
        ContextMenuFilters.Close()
        DiskImagesScan(True)
    End Sub

    Private Sub BtnUndo_Click(sender As Object, e As EventArgs) Handles BtnUndo.Click, ToolStripBtnUndo.Click
        _Disk.Data.Undo()
        If _Disk.ReinitializeRequired Then
            _Disk.Reinitialize()
        End If
        ComboItemRefresh(True, True)
    End Sub

    Private Sub ComboImages_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImages.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        DiskImageLoad(False)
    End Sub

    Private Sub ComboImagesFiltered_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImagesFiltered.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        ComboImages.SelectedItem = ComboImagesFiltered.SelectedItem
    End Sub

    Private Sub ComboOEMName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboOEMName.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        FiltersApply()
    End Sub

    Private Sub ContextMenuCopy_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ContextMenuCopy1.ItemClicked, ContextMenuCopy2.ItemClicked
        Dim LV As ListView = Nothing

        If sender.name = "Summary" Then
            LV = ListViewSummary
        ElseIf sender.name = "Hashes" Then
            LV = ListViewHashes
        End If

        If LV IsNot Nothing AndAlso LV.FocusedItem IsNot Nothing Then
            Dim Value As String = LV.FocusedItem.SubItems.Item(1).Text
            Clipboard.SetText(Value)
        End If
    End Sub

    Private Sub ContextMenuCopy_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuCopy1.Opening, ContextMenuCopy2.Opening
        Dim LV As ListView = Nothing
        Dim CM As ContextMenuStrip = sender

        If CM.Name = "Summary" Then
            LV = ListViewSummary
        ElseIf CM.Name = "Hashes" Then
            LV = ListViewHashes
        End If

        If LV IsNot Nothing AndAlso LV.FocusedItem IsNot Nothing Then
            CM.Items(0).Text = "&Copy " & LV.FocusedItem.Text
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub ContextMenuFiles_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuFiles.Opening
        If ListViewFiles.SelectedItems.Count = 0 Then
            e.Cancel = True
        End If
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

    Private Sub Debounce_Tick(sender As Object, e As EventArgs) Handles Debounce.Tick
        Debounce.Stop()
        FiltersApply()
    End Sub

    Private Sub File_DragDrop(sender As Object, e As DragEventArgs) Handles ComboImages.DragDrop, ComboImagesFiltered.DragDrop, LabelDropMessage.DragDrop, ListViewFiles.DragDrop, ListViewHashes.DragDrop, ListViewSummary.DragDrop
        Dim Files As String() = e.Data.GetData(DataFormats.FileDrop)
        ProcessFileDrop(Files)
    End Sub

    Private Sub File_DragEnter(sender As Object, e As DragEventArgs) Handles ComboImages.DragEnter, ComboImagesFiltered.DragEnter, LabelDropMessage.DragEnter, ListViewFiles.DragEnter, ListViewHashes.DragEnter, ListViewSummary.DragEnter
        FileDropStart(e)
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

    Private Sub ListViewFiles_ColumnWidthChanging(sender As Object, e As ColumnWidthChangingEventArgs) Handles ListViewFiles.ColumnWidthChanging
        e.NewWidth = Me.ListViewFiles.Columns(e.ColumnIndex).Width
        e.Cancel = True
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

    Private Sub ListViewFiles_DrawItem(sender As Object, e As DrawListViewItemEventArgs) Handles ListViewFiles.DrawItem
        e.DrawDefault = True
    End Sub

    Private Sub ListViewFiles_DrawSubItem(sender As Object, e As DrawListViewSubItemEventArgs) Handles ListViewFiles.DrawSubItem
        e.DrawDefault = True
    End Sub

    Private Sub ListViewFiles_ItemDrag(sender As Object, e As ItemDragEventArgs) Handles ListViewFiles.ItemDrag
        _SuppressEvent = True
        DragDropSelectedFiles()
        _SuppressEvent = False
    End Sub

    Private Sub ListViewFiles_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles ListViewFiles.ItemSelectionChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        ItemSelectionChanged()
    End Sub

    Private Sub ListViewHashes_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles ListViewHashes.ItemSelectionChanged
        e.Item.Selected = False
    End Sub

    Private Sub ListViewSummary_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles ListViewSummary.ItemSelectionChanged
        e.Item.Selected = False
    End Sub

    Private Sub MainForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If Not CloseAll() Then
            e.Cancel = True
        End If
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        _FileVersion = FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion
        Me.Text = GetWindowCaption()

        PositionForm()

        ListViewDoubleBuffer(ListViewFiles)
        FiltersInitialize()
        _LoadedFileNames = New Dictionary(Of String, LoadedImageData)
        _OEMNameDictionary = GetOEMNameDictionary()
        Debounce = New Timer With {
            .Interval = 750
        }
        ContextMenuCopy1 = New ContextMenuStrip With {
            .Name = "Summary"
        }
        ContextMenuCopy1.Items.Add("&Copy Value")
        ListViewSummary.ContextMenuStrip = ContextMenuCopy1

        ContextMenuCopy2 = New ContextMenuStrip With {
            .Name = "Hashes"
        }
        ContextMenuCopy2.Items.Add("&Copy Value")
        ListViewHashes.ContextMenuStrip = ContextMenuCopy2

        ResetAll()

        Dim Args = Environment.GetCommandLineArgs.Skip(1).ToArray

        If Args.Length > 0 Then
            ProcessFileDrop(Args)
        End If
    End Sub

    Private Sub MainForm_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.WindowWidth = Me.Width
        My.Settings.WindowHeight = Me.Height
    End Sub

    Private Sub TxtSearch_TextChanged(sender As Object, e As EventArgs) Handles TxtSearch.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Debounce.Stop()
        Debounce.Start()
    End Sub

    Private Sub BtnHelpProjectPage_Click(sender As Object, e As EventArgs) Handles BtnHelpProjectPage.Click
        Process.Start(SITE_URL)
    End Sub

    Private Sub BtnHelpAbout_Click(sender As Object, e As EventArgs) Handles BtnHelpAbout.Click
        Dim AboutBox As New AboutBox()
        AboutBox.ShowDialog()
    End Sub

    Private Sub BtnHelpUpdateCheck_Click(sender As Object, e As EventArgs) Handles BtnHelpUpdateCheck.Click
        CheckForUpdates
    End Sub
#End Region

End Class