Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports DiskImageTool.DiskImage

Public Enum ItemScanTypes
    None = 0
    Disk = 1
    OEMName = 2
    DiskType = 4
    FreeClusters = 8
    Directory = 16
    All = 31
End Enum

Public Structure FileSystemInfo
    Dim NewestFileDate As Date?
    Dim OldestFileDate As Date?
    Dim VolumeLabel As DirectoryEntry
End Structure

Public Structure SaveDialogFilter
    Dim Filter As String
    Dim FilterIndex As Integer
End Structure

Public Class MainForm
    Private WithEvents ContextMenuCopy1 As ContextMenuStrip
    Private WithEvents ContextMenuCopy2 As ContextMenuStrip
    Private WithEvents Debounce As Timer
    Private WithEvents ImageFilters As Filters.ImageFilters
    Public Const SITE_URL = "https://github.com/Digitoxin1/DiskImageTool"
    Private ReadOnly _lvwColumnSorter As ListViewColumnSorter
    Private _BootStrapDB As BootstrapDB
    Private _CurrentImage As CurrentImage
    Private _DriveAEnabled As Boolean = False
    Private _DriveBEnabled As Boolean = False
    Private _ExportUnknownImages As Boolean = False
    Private _FileVersion As String = ""
    Private _ListViewCheckAll As Boolean = False
    Private _ListViewClickedGroup As ListViewGroup = Nothing
    Private _ListViewHeader As ListViewHeader
    Private _ListViewWidths() As Integer
    Private _LoadedFiles As LoadedFiles
    Private _ScanRun As Boolean = False
    Private _SubFilterDiskType As ComboFilter
    Private _SubFilterOEMName As ComboFilter
    Private _SuppressEvent As Boolean = False
    Private _TitleDB As FloppyDB

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _lvwColumnSorter = New ListViewColumnSorter
        ListViewInit()
        ToolStripFATCombo.ComboBox.DrawMode = DrawMode.OwnerDrawFixed
        AddHandler ToolStripFATCombo.ComboBox.DrawItem, AddressOf DrawComboFAT

        _CurrentImage = Nothing
    End Sub

    Public Function ImageFiltersScanDirectory(Disk As DiskImage.Disk, ImageData As ImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False) As DirectoryScanResponse
        Dim Response As DirectoryScanResponse
        Dim HasLostClusters As Boolean = False

        If Not Remove AndAlso Disk.IsValidImage Then
            Response = ProcessDirectoryEntries(Disk.RootDirectory, True)
            HasLostClusters = Disk.RootDirectory.FATAllocation.LostClusters.Count > 0
        Else
            Response = New DirectoryScanResponse(Nothing)
        End If

        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FileSystem_HasCreationDate, Response.HasValidCreated)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FileSystem_HasLastAccessDate, Response.HasValidLastAccessed)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FileSystem_HasReservedBytesSet, Response.HasNTUnknownFlags Or Response.HasFAT32Cluster)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FileSystem_HasLongFileNames, Response.HasLFN)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FileSystem_DirectoryHasAdditionalData, Response.HasAdditionalData)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FileSystem_DirectoryHasBootSector, Response.HasBootSector)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FileSystem_InvalidDirectoryEntries, Response.HasInvalidDirectoryEntries)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FAT_ChainingErrors, Response.HasFATChainingErrors)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FAT_LostClusters, HasLostClusters)

        Return Response
    End Function

    Public Sub ImageFiltersScanDisk(Disk As DiskImage.Disk, ImageData As ImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim TitleFindResult As FloppyDB.TitleFindResult = Nothing
        Dim FileData As FloppyDB.FileNameData = Nothing
        Dim Disk_UnknownFormat As Boolean
        Dim FAT_BadSectors As Boolean = False
        Dim FATS_MismatchedFATs As Boolean = False
        Dim Disk_MismatchedMediaDescriptor As Boolean = False
        Dim Disk_MismatchedImageSize As Boolean = False
        Dim Disk_CustomFormat As Boolean = False
        Dim Image_InDatabase As Boolean = False
        Dim Image_NotInDatabase As Boolean = False
        Dim Image_Verified As Boolean = False
        Dim Image_Unverified As Boolean = False
        Dim Disk_NOBPB As Boolean = False
        Dim Disk_NoBootLoader As Boolean = False
        Dim DIsk_CustomBootLoader As Boolean = False
        Dim Database_MismatchedStatus As Boolean = False

        If Not Remove Then
            Disk_UnknownFormat = Not Disk.IsValidImage
            Image_NotInDatabase = True
            If Not Disk_UnknownFormat Then
                Dim MediaDescriptor = GetFloppyDiskMediaDescriptor(Disk.DiskFormat)
                FAT_BadSectors = Disk.FAT.BadClusters.Count > 0
                FATS_MismatchedFATs = Not IsDiskFormatXDF(Disk.DiskFormat) AndAlso Not Disk.FATTables.FATsMatch
                If Disk.BootSector.BPB.IsValid Then
                    If Disk.BootSector.BPB.MediaDescriptor <> MediaDescriptor Then
                        Disk_MismatchedMediaDescriptor = True
                    ElseIf Disk.DiskFormat = FloppyDiskFormat.FloppyXDF35 And Disk.FAT.MediaDescriptor = &HF9 Then
                        Disk_MismatchedMediaDescriptor = False
                    ElseIf Disk.FAT.HasMediaDescriptor AndAlso Disk.FAT.MediaDescriptor <> Disk.BootSector.BPB.MediaDescriptor Then
                        Disk_MismatchedMediaDescriptor = True
                    ElseIf Disk.FAT.HasMediaDescriptor AndAlso Disk.FAT.MediaDescriptor <> MediaDescriptor Then
                        Disk_MismatchedMediaDescriptor = True
                    End If
                End If
                Disk_MismatchedImageSize = Disk.CheckImageSize <> 0
                Disk_CustomFormat = GetFloppyDiskFormat(Disk.BPB, False) = FloppyDiskFormat.FloppyUnknown
                Disk_NOBPB = Not Disk.BootSector.BPB.IsValid
                If Disk.BootSector.BootStrapCode.Length = 0 Then
                    If Disk.BootSector.CheckJumpInstruction(False, True) Then
                        Disk_NoBootLoader = True
                    Else
                        DIsk_CustomBootLoader = True
                        Disk_NOBPB = False
                    End If
                End If
            End If

            If _TitleDB.TitleCount > 0 Then
                TitleFindResult = _TitleDB.TitleFind(Disk)
                If TitleFindResult.TitleData IsNot Nothing Then
                    Image_NotInDatabase = False
                    Image_InDatabase = True
                    If TitleFindResult.TitleData.GetStatus = FloppyDB.FloppyDBStatus.Verified Then
                        Image_Verified = True
                    Else
                        Image_Unverified = True
                    End If
                    If My.Settings.Debug Then
                        FileData = New FloppyDB.FileNameData(ImageData.FileName)
                        If TitleFindResult.TitleData.GetStatus <> FileData.Status Then
                            Database_MismatchedStatus = True
                        End If
                    End If
                End If
            End If
        Else
            Disk_UnknownFormat = False
        End If

        If _TitleDB.TitleCount > 0 Then
            ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Image_InDatabase, Image_InDatabase)
            ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Image_NotInDatabase, Image_NotInDatabase)
            ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Image_Verified, Image_Verified)
            ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Image_Unverified, Image_Unverified)
        End If

        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FAT_BadSectors, FAT_BadSectors)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FAT_MismatchedFATs, FATS_MismatchedFATs)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_UnknownFormat, Disk_UnknownFormat)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_MismatchedMediaDescriptor, Disk_MismatchedMediaDescriptor)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_MismatchedImageSize, Disk_MismatchedImageSize)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_CustomFormat, Disk_CustomFormat)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_NOBPB, Disk_NOBPB)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_NoBootLoader, Disk_NoBootLoader)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.DIsk_CustomBootLoader, DIsk_CustomBootLoader)

        If My.Settings.Debug And Disk IsNot Nothing Then
            ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Database_MismatchedStatus, Database_MismatchedStatus)

            If _ExportUnknownImages Then
                If Image_NotInDatabase Then
                    If TitleFindResult Is Nothing Then
                        TitleFindResult = _TitleDB.TitleFind(Disk)
                    End If
                    If FileData Is Nothing Then
                        FileData = New FloppyDB.FileNameData(ImageData.FileName)
                    End If
                    Dim Media = GetFloppyDiskFormatName(Disk.BPB, True)
                    _TitleDB.AddTile(FileData, Media, TitleFindResult.MD5)
                End If
            End If
        End If
    End Sub

    Public Sub ImageFiltersScanFreeClusters(Disk As DiskImage.Disk, ImageData As ImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim HasFreeClusters As Boolean = False

        If Not Remove AndAlso Disk.IsValidImage Then
            HasFreeClusters = Disk.FAT.HasFreeClusters(FAT12.FreeClusterEmum.WithData)
        End If

        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_FreeClustersWithData, HasFreeClusters)
    End Sub

    Public Sub ImageFiltersScanModified(Disk As DiskImage.Disk, ImageData As ImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim IsModified As Boolean = Not Remove And (Disk IsNot Nothing AndAlso Disk.Image.History.Modified)

        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.ModifiedFiles, IsModified, True)
    End Sub

    Public Sub ImageFiltersScanOEMName(Disk As DiskImage.Disk, ImageData As ImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim Bootstrap_Unknown = False
        Dim OEMName_Unknown = False
        Dim OEMName_Mismatched = False
        Dim OEMName_Verified = False
        Dim OEMName_Unverified = False
        Dim OEMName_Windows9x = False

        If Not Remove AndAlso Disk.IsValidImage Then
            Dim DoOEMNameCheck As Boolean = Disk.BootSector.BPB.IsValid
            Dim OEMNameResponse = _BootStrapDB.CheckOEMName(Disk.BootSector)
            Bootstrap_Unknown = Not OEMNameResponse.NoBootLoader And Not OEMNameResponse.Found
            If DoOEMNameCheck Then
                OEMName_Unknown = Not OEMNameResponse.Found And Not OEMNameResponse.NoBootLoader
                OEMName_Mismatched = OEMNameResponse.Found And Not OEMNameResponse.Matched
                OEMName_Verified = OEMNameResponse.Matched And OEMNameResponse.Verified
                OEMName_Unverified = OEMNameResponse.Matched And Not OEMNameResponse.Verified
                OEMName_Windows9x = OEMNameResponse.IsWin9x
            End If
        End If

        If My.Settings.Debug Then
            ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Bootstrap_Unknown, Bootstrap_Unknown)
        End If
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.OEMName_Windows9x, OEMName_Windows9x)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.OEMName_Mismatched, OEMName_Mismatched)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.OEMName_Unknown, OEMName_Unknown)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.OEMName_Verified, OEMName_Verified)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.OEMName_Unverified, OEMName_Unverified)
    End Sub

    Public Sub ImageSubFilterDiskTypeUpdate(Disk As DiskImage.Disk, ImageData As ImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim DiskFormat As String = ""

        If Not Remove Then
            'If Disk.IsValidImage Then
            '    DiskType = GetFloppyDiskTypeName(Disk.BPB, True)
            'Else
            '    DiskType = GetFloppyDiskTypeName(Disk.Data.Length, True)
            'End If
            Dim DiskFormatBySize = GetFloppyDiskFormat(Disk.Image.Length)

            If Disk.DiskFormat <> FloppyDiskFormat.FloppyUnknown Or DiskFormatBySize = FloppyDiskFormat.FloppyUnknown Then
                DiskFormat = GetFloppyDiskFormatName(Disk.DiskFormat)
            Else
                DiskFormat = GetFloppyDiskFormatName(DiskFormatBySize)
            End If
        End If

        If Not ImageData.Scanned Or DiskFormat <> ImageData.DiskType Then
            If ImageData.Scanned Then
                _SubFilterDiskType.Remove(ImageData.DiskType, UpdateFilters)
            End If

            ImageData.DiskType = DiskFormat

            If Not Remove Then
                _SubFilterDiskType.Add(ImageData.DiskType, UpdateFilters)
            End If
        End If
    End Sub

    Public Sub ImageSubFilterOEMNameUpdate(Disk As DiskImage.Disk, ImageData As ImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        If Disk IsNot Nothing AndAlso Disk.IsValidImage Then
            Dim OEMNameString As String = ""

            If Not Remove Then
                If Disk.BootSector.BPB.IsValid Then
                    OEMNameString = Disk.BootSector.GetOEMNameString.TrimEnd(NULL_CHAR)
                    'OEMNameString = Crc32.ComputeChecksum(Disk.BootSector.BootStrapCode).ToString("X8") & " (" & OEMNameString & ")"
                End If
            End If

            If Not ImageData.Scanned Or OEMNameString <> ImageData.OEMName Then
                If ImageData.Scanned Then
                    _SubFilterOEMName.Remove(ImageData.OEMName, UpdateFilters)
                End If

                ImageData.OEMName = OEMNameString

                If Not Remove Then
                    SubFilterOEMNameAdd(ImageData.OEMName, UpdateFilters)
                End If
            End If
        End If
    End Sub

    Public Function ImageWin9xClean(Disk As Disk, Batch As Boolean) As Boolean
        Dim Result As Boolean = False

        If Batch Then
            If _TitleDB.IsVerifiedImage(Disk) Then
                Return Result
            End If
        End If

        Dim UseTransaction = Disk.BeginTransaction

        If Disk.BootSector.IsWin9xOEMName Then
            Dim BootstrapType = _BootStrapDB.FindMatch(Disk.BootSector.BootStrapCode)
            If BootstrapType IsNot Nothing Then
                If BootstrapType.OEMNames.Count > 0 Then
                    Disk.BootSector.OEMName = BootstrapType.OEMNames.Item(0).Name
                    Result = True
                End If
            End If
        End If

        Dim FileList = Disk.RootDirectory.GetFileList()

        For Each DirectoryEntry In FileList
            If DirectoryEntry.IsValid Then
                If DirectoryEntry.HasLastAccessDate Then
                    If DirectoryEntry.GetLastAccessDate.IsValidDate Then
                        DirectoryEntry.ClearLastAccessDate()
                        Result = True
                    End If
                End If

                If DirectoryEntry.HasCreationDate Then
                    If DirectoryEntry.GetCreationDate.IsValidDate Then
                        DirectoryEntry.ClearCreationDate()
                        Result = True
                    End If
                End If
            End If
        Next

        If UseTransaction Then
            Disk.EndTransaction()
        End If

        Return Result
    End Function

    Private Shared Sub DirectoryEntryDisplayText(DirectoryEntry As DiskImage.DirectoryEntry)
        If Not DirectoryEntry.IsValidFile Then 'Or DirectoryEntry.IsDeleted Then
            Exit Sub
        End If

        Dim Caption As String = $"File - {DirectoryEntry.GetShortFileName(True)}"
        If DirectoryEntry.IsDeleted Then
            Caption = "Deleted " & Caption
        End If
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

        Dim frmTextView = New TextViewForm(Caption, Content)
        frmTextView.ShowDialog()
    End Sub

    Private Shared Function ListViewFilesGetItem(Group As ListViewGroup, FileData As FileData) As ListViewItem
        Dim SI As ListViewItem.ListViewSubItem
        Dim ForeColor As Color
        Dim IsDeleted As Boolean = FileData.DirectoryEntry.IsDeleted
        Dim HasInvalidFileSize As Boolean = FileData.DirectoryEntry.HasInvalidFileSize
        Dim IsBlank As Boolean = FileData.DirectoryEntry.IsBlank

        Dim Attrib As String = IIf(FileData.DirectoryEntry.IsArchive, "A ", "- ") _
            & IIf(FileData.DirectoryEntry.IsReadOnly, "R ", "- ") _
            & IIf(FileData.DirectoryEntry.IsSystem, "S ", "- ") _
            & IIf(FileData.DirectoryEntry.IsHidden, "H ", "- ") _
            & IIf(FileData.DirectoryEntry.IsDirectory, "D ", "- ") _
            & IIf(FileData.DirectoryEntry.IsVolumeName, "V ", "- ")

        If IsDeleted Then
            ForeColor = Color.Gray
        ElseIf FileData.DirectoryEntry.IsValidVolumeName Then
            ForeColor = Color.Green
        ElseIf FileData.DirectoryEntry.IsDirectory And Not FileData.DirectoryEntry.IsVolumeName Then
            ForeColor = Color.Blue
        End If

        Dim ModifiedString As String = IIf(FileData.DirectoryEntry.IsModified, "#", "")

        Dim Item = New ListViewItem(ModifiedString, Group) With {
            .UseItemStyleForSubItems = False,
            .Tag = FileData
        }
        If ModifiedString = "" Then
            Item.ForeColor = ForeColor
        Else
            Item.ForeColor = Color.Blue
        End If

        SI = Item.SubItems.Add(FileData.DirectoryEntry.GetFileName(True))
        SI.Name = "FileName"
        If Not IsDeleted And (FileData.DirectoryEntry.HasInvalidFilename Or FileData.DuplicateFileName) Then
            SI.ForeColor = Color.Red
        Else
            SI.ForeColor = ForeColor
        End If

        SI = Item.SubItems.Add(FileData.DirectoryEntry.GetFileExtension(True))
        SI.Name = "FileExtension"
        If Not IsDeleted And (FileData.DirectoryEntry.HasInvalidExtension Or FileData.DuplicateFileName) Then
            SI.ForeColor = Color.Red
        Else
            SI.ForeColor = ForeColor
        End If

        If IsBlank Then
            SI = Item.SubItems.Add("")
        ElseIf HasInvalidFileSize Then
            SI = Item.SubItems.Add("Invalid")
            If Not IsDeleted Then
                SI.ForeColor = Color.Red
            Else
                SI.ForeColor = ForeColor
            End If
        ElseIf Not IsDeleted And FileData.DirectoryEntry.HasIncorrectFileSize Then
            SI = Item.SubItems.Add(Format(FileData.DirectoryEntry.FileSize, "N0"))
            SI.ForeColor = Color.Red
        Else
            SI = Item.SubItems.Add(Format(FileData.DirectoryEntry.FileSize, "N0"))
            SI.ForeColor = ForeColor
        End If
        SI.Name = "FileSize"

        If IsBlank Then
            SI = Item.SubItems.Add("")
        Else
            SI = Item.SubItems.Add(FileData.DirectoryEntry.GetLastWriteDate.ToString(True, True, False, True))
            If FileData.DirectoryEntry.GetLastWriteDate.IsValidDate Or IsDeleted Then
                SI.ForeColor = ForeColor
            Else
                SI.ForeColor = Color.Red
            End If
        End If
        SI.Name = "FileLastWriteDate"

        Dim SubItemForeColor As Color = ForeColor
        If IsBlank Then
            SI = Item.SubItems.Add("")
        Else
            If FileData.DirectoryEntry.HasInvalidStartingCluster Then
                SI = Item.SubItems.Add("Invalid")
                If Not IsDeleted Then
                    SubItemForeColor = Color.Red
                End If
            Else
                SI = Item.SubItems.Add(Format(FileData.DirectoryEntry.StartingCluster, "N0"))
            End If
        End If
        SI.Name = "FileStartingCluster"

        If IsBlank Then
            SI = Item.SubItems.Add("")
        Else
            Dim ErrorText As String = ""
            If Not IsDeleted And FileData.DirectoryEntry.IsCrossLinked Then
                SubItemForeColor = Color.Red
                ErrorText = "CL"
            ElseIf Not IsDeleted And FileData.DirectoryEntry.HasCircularChain Then
                SubItemForeColor = Color.Red
                ErrorText = "CC"
            End If
            SI.ForeColor = SubItemForeColor

            SI = Item.SubItems.Add(ErrorText)
            SI.ForeColor = Color.Red
        End If
        SI.Name = "FileClusterError"

        If IsBlank Then
            Item.SubItems.Add("")
        Else
            SI = Item.SubItems.Add(Attrib)
            If Not IsDeleted And (FileData.DirectoryEntry.HasInvalidAttributes Or FileData.InvalidVolumeName) Then
                SI.ForeColor = Color.Red
            Else
                SI.ForeColor = ForeColor
            End If
        End If

        If IsBlank Then
            Item.SubItems.Add("")
        Else
            If FileData.DirectoryEntry.IsValidFile Then
                SI = Item.SubItems.Add(FileData.DirectoryEntry.GetChecksum().ToString("X8"))
            Else
                SI = Item.SubItems.Add("")
            End If
            SI.ForeColor = ForeColor
        End If

        If IsBlank Then
            SI = Item.SubItems.Add("")
        Else
            If FileData.DirectoryEntry.HasCreationDate Then
                SI = Item.SubItems.Add(FileData.DirectoryEntry.GetCreationDate.ToString(True, True, True, True))
                If FileData.DirectoryEntry.GetCreationDate.IsValidDate Or IsDeleted Then
                    SI.ForeColor = ForeColor
                Else
                    SI.ForeColor = Color.Red
                End If
            Else
                SI = Item.SubItems.Add("")
            End If
        End If
        SI.Name = "FileCreationDate"

        If IsBlank Then
            SI = Item.SubItems.Add("")
        Else
            If FileData.DirectoryEntry.HasLastAccessDate Then
                SI = Item.SubItems.Add(FileData.DirectoryEntry.GetLastAccessDate.ToString())
                If FileData.DirectoryEntry.GetLastAccessDate.IsValidDate Or IsDeleted Then
                    SI.ForeColor = ForeColor
                Else
                    SI.ForeColor = Color.Red
                End If
            Else
                SI = Item.SubItems.Add("")
            End If
        End If
        SI.Name = "FileLastAccessDate"


        If IsBlank Then
            Item.SubItems.Add("")
        Else
            Dim Reserved As String = ""
            If FileData.DirectoryEntry.ReservedForWinNT <> 0 Then
                Reserved = FileData.DirectoryEntry.ReservedForWinNT.ToString("X2")
            End If
            SI = Item.SubItems.Add(Reserved)
            If FileData.DirectoryEntry.HasNTUnknownFlags Then
                SI.ForeColor = Color.Red
            Else
                SI.ForeColor = ForeColor
            End If
        End If

        If IsBlank Then
            Item.SubItems.Add("")
        Else
            Dim FAT32Cluster As String = ""
            If FileData.DirectoryEntry.ReservedForFAT32 <> 0 Then
                FAT32Cluster = BitConverter.ToString(BitConverter.GetBytes(FileData.DirectoryEntry.ReservedForFAT32))
            End If
            SI = Item.SubItems.Add(FAT32Cluster)
            SI.ForeColor = Color.Red
        End If

        If IsBlank Then
            Item.SubItems.Add("")
        Else
            SI = Item.SubItems.Add(FileData.LFNFileName)
            SI.ForeColor = ForeColor
        End If

        Return Item
    End Function

    Private Shared Function MsgBoxNewFileName(FileName As String) As MsgBoxResult
        Dim Msg As String = $"'{FileName}' is a read-only file.  Please specify a new file name."
        Return MsgBox(Msg, MsgBoxStyle.OkCancel)
    End Function

    Private Shared Function MsgBoxOverwrite(FilePath As String) As MyMsgBoxResult
        Dim Msg As String = $"{IO.Path.GetFileName(FilePath)} already exists.{vbCrLf}Do you wish to replace it?"

        Dim SaveAllForm As New SaveAllForm(Msg)
        SaveAllForm.ShowDialog()
        Return SaveAllForm.Result
    End Function

    Private Shared Function MsgBoxSave(FileName As String) As MsgBoxResult
        Dim Msg As String = $"Save file '{FileName}'?"

        Return MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNoCancel + MsgBoxStyle.DefaultButton3, "Save")
    End Function

    Private Shared Function MsgBoxSaveAll(FileName As String) As MyMsgBoxResult
        Dim Msg As String = $"Save file '{FileName}'?"

        Dim SaveAllForm As New SaveAllForm(Msg)
        SaveAllForm.ShowDialog()
        Return SaveAllForm.Result
    End Function

    Private Sub BootSectorEdit(CurrentImage As CurrentImage)
        Dim BootSectorForm As New BootSectorForm(CurrentImage.Disk.BootSector.Data, _BootStrapDB)

        BootSectorForm.ShowDialog()

        Dim Result As Boolean = BootSectorForm.DialogResult = DialogResult.OK

        If Result Then
            If Not CurrentImage.Disk.BootSector.Data.CompareTo(BootSectorForm.Data) Then
                CurrentImage.Disk.BootSector.Data = BootSectorForm.Data
                DiskImageRefresh(CurrentImage)
            End If
        End If
    End Sub

    Private Sub CheckForUpdates()
        Dim DownloadVersion As String = ""
        Dim DownloadURL As String = ""
        Dim Body As String = ""
        Dim UpdateAvailable As Boolean = False
        Dim ErrMsg As String = "An error occurred while checking for updates.  Please try again later."

        Cursor.Current = Cursors.WaitCursor
        Dim ResponseText = GetAppUpdateResponse()
        Cursor.Current = Cursors.Default

        If ResponseText = "" Then
            MsgBox(ErrMsg, MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        Try
            Dim JSON As Dictionary(Of String, Object) = CompactJson.Serializer.Parse(Of Dictionary(Of String, Object))(ResponseText)

            If JSON.ContainsKey("tag_name") Then
                DownloadVersion = JSON.Item("tag_name").ToString
                If DownloadVersion.StartsWith("v", StringComparison.OrdinalIgnoreCase) Then
                    DownloadVersion = DownloadVersion.Remove(0, 1)
                End If
            End If

            If JSON.ContainsKey("assets") Then
                Dim assets() As Dictionary(Of String, Object) = CompactJson.Serializer.Parse(Of Dictionary(Of String, Object)())(JSON.Item("assets").ToString)
                If assets.Length > 0 Then
                    If assets(0).ContainsKey("browser_download_url") Then
                        DownloadURL = assets(0).Item("browser_download_url").ToString
                    End If
                End If
            End If

            If JSON.ContainsKey("body") Then
                Body = JSON.Item("body").ToString
            End If

            If DownloadVersion <> "" And DownloadURL <> "" Then
                Dim CurrentVersion = GetVersionString()
                UpdateAvailable = Version.Parse(DownloadVersion) > Version.Parse(CurrentVersion)
            End If

        Catch ex As Exception
            MsgBox(ErrMsg, MsgBoxStyle.Exclamation)
            DebugException(ex)
            Exit Sub
        End Try

        If UpdateAvailable Then
            Dim Msg = $"{My.Application.Info.Title} v{DownloadVersion} is available."
            If Body <> "" Then
                Msg &= $"{vbCrLf}{vbCrLf}Whats New{vbCrLf}{New String("—", 6)}{vbCrLf}{Body}{vbCrLf}"
            End If
            Msg &= $"{vbCrLf}{vbCrLf}Do you wish to download it at this time?"

            If MsgBoxQuestion(Msg) Then
                Dim Dialog As New SaveFileDialog With {
                    .Filter = FileDialogGetFilter("Zip Archive", ".zip"),
                    .FileName = IO.Path.GetFileName(DownloadURL),
                    .InitialDirectory = GetDownloadsFolder(),
                    .RestoreDirectory = True
                }
                Dialog.ShowDialog()
                If Dialog.FileName <> "" Then
                    Cursor.Current = Cursors.WaitCursor
                    Try
                        Dim Client As New Net.WebClient()
                        Client.DownloadFile(DownloadURL, Dialog.FileName)
                    Catch ex As Exception
                        MsgBox("An error occurred while downloading the file.", MsgBoxStyle.Exclamation)
                        DebugException(ex)
                    End Try
                    Cursor.Current = Cursors.Default
                End If
            End If
        Else
            MsgBox($"You are running the latest version of {My.Application.Info.Title}.", MsgBoxStyle.Information)
        End If
    End Sub

    Private Async Sub CheckForUpdatesStartup()
        Dim DownloadVersion As String = ""
        Dim DownloadURL As String = ""
        Dim UpdateAvailable As Boolean = False
        Dim ResponseText As String = ""

        Await Task.Run(Sub()
                           ResponseText = GetAppUpdateResponse()
                       End Sub)

        If ResponseText <> "" Then
            Try
                Dim JSON As Dictionary(Of String, Object) = CompactJson.Serializer.Parse(Of Dictionary(Of String, Object))(ResponseText)

                If JSON.ContainsKey("tag_name") Then
                    DownloadVersion = JSON.Item("tag_name").ToString
                    If DownloadVersion.StartsWith("v", StringComparison.OrdinalIgnoreCase) Then
                        DownloadVersion = DownloadVersion.Remove(0, 1)
                    End If
                End If

                If JSON.ContainsKey("assets") Then
                    Dim assets() As Dictionary(Of String, Object) = CompactJson.Serializer.Parse(Of Dictionary(Of String, Object)())(JSON.Item("assets").ToString)
                    If assets.Length > 0 Then
                        If assets(0).ContainsKey("browser_download_url") Then
                            DownloadURL = assets(0).Item("browser_download_url").ToString
                        End If
                    End If
                End If

                If DownloadVersion <> "" And DownloadURL <> "" Then
                    Dim CurrentVersion = GetVersionString()
                    UpdateAvailable = Version.Parse(DownloadVersion) > Version.Parse(CurrentVersion)
                End If

            Catch ex As Exception
                DebugException(ex)
            End Try
        End If

        MainMenuUpdateAvailable.Visible = UpdateAvailable
    End Sub

    Private Sub ClearFilesPanel(CurrentImage As CurrentImage)
        MenuDisplayDirectorySubMenuClear()
        ListViewFiles.ListViewItemSorter = Nothing
        ListViewFiles.Items.Clear()
        MenuToolsWin9xClean.Enabled = False
        MenuToolsClearReservedBytes.Enabled = False
        ItemSelectionChanged(CurrentImage)
    End Sub

    Private Sub ClearSort(Reset As Boolean)
        If Reset Then
            _lvwColumnSorter.Sort(0)
            ListViewFiles.Sort()
        Else
            _lvwColumnSorter.Sort(-1, SortOrder.None)
        End If
        ListViewFiles.SetSortIcon(-1, SortOrder.None)
        _lvwColumnSorter.ClearHistory()
        BtnResetSort.Enabled = False
    End Sub

    Private Function CloseAll(CurrentImage As CurrentImage) As Boolean
        Dim BatchResult As MyMsgBoxResult = MyMsgBoxResult.Yes
        Dim Result As MyMsgBoxResult = MyMsgBoxResult.Yes

        Dim ModifyImageList = GetModifiedImageList()

        If ModifyImageList.Count > 0 Then
            Dim ShowDialog As Boolean = True

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

                If Result = MyMsgBoxResult.YesToAll Or Result = MyMsgBoxResult.NoToAll Then
                    ShowDialog = False
                    If Result = MyMsgBoxResult.NoToAll Then
                        BatchResult = MyMsgBoxResult.No
                    End If
                End If

                If Result = MyMsgBoxResult.Yes Or Result = MyMsgBoxResult.YesToAll Then
                    If ImageData.ReadOnly Then
                        If Not ShowDialog Then
                            If MsgBoxNewFileName(ImageData.FileName) <> MsgBoxResult.Ok Then
                                Result = MyMsgBoxResult.Cancel
                                Exit For
                            End If
                        End If
                        If Result <> MyMsgBoxResult.No Then
                            NewFilePath = GetNewFilePath(CurrentImage, ImageData)
                            If NewFilePath = "" Then
                                Result = MyMsgBoxResult.Cancel
                                Exit For
                            End If
                        End If
                    End If
                End If

                If Result = MyMsgBoxResult.Yes Or Result = MyMsgBoxResult.YesToAll Then
                    If Not DiskImageSave(CurrentImage, ImageData, NewFilePath) Then
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

    Private Sub CloseCurrent(CurrentImage As CurrentImage)
        Dim NewFilePath As String = ""
        Dim Result As MsgBoxResult

        If CurrentImage.ImageData.Filter(Filters.FilterTypes.ModifiedFiles) Then
            Result = MsgBoxSave(CurrentImage.ImageData.FileName)
        Else
            Result = MsgBoxResult.No
        End If

        If Result = MsgBoxResult.Yes Then
            If CurrentImage.ImageData.ReadOnly Then
                NewFilePath = GetNewFilePath(CurrentImage)
                If NewFilePath = "" Then
                    Result = MsgBoxResult.Cancel
                End If
            End If
            If Result = MsgBoxResult.Yes Then
                If Not DiskImageSave(CurrentImage, NewFilePath) Then
                    Result = MsgBoxResult.Cancel
                End If
            End If
        End If

        If Result <> MsgBoxResult.Cancel Then
            FileClose(CurrentImage.ImageData)
            ComboImagesRefreshPaths()
        End If
    End Sub

    Private Sub ComboImagesClear(Combo As ComboBox)
        Combo.Items.Clear()
        RefreshSubFilterEnabled(Combo)
    End Sub

    Private Sub ComboImagesRefreshCurrentItemText()
        ComboImages.Invalidate()
        ComboImagesFiltered.Invalidate()
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
        ImageData.StringOffset = GetPathOffset()
        ComboImagesRefreshItemText()
        ComboImages.EndUpdate()
    End Sub

    Private Sub ComboImagesReset()
        ComboImagesClear(ComboImages)
        ComboImagesClear(ComboImagesFiltered)
        ComboImagesToggle(False)
    End Sub

    Private Sub ComboImagesToggle(Filtered As Boolean)
        ComboImages.Visible = Not Filtered
        ComboImagesFiltered.Visible = Filtered
    End Sub

    Private Sub CompareImages()
        Dim ImageData1 As ImageData = ComboImages.Items(0)
        Dim ImageData2 As ImageData = ComboImages.Items(1)

        Dim Content = ImageCompare.CompareImages(ImageData1, ImageData2)

        Dim frmTextView = New TextViewForm("Image Comparison", Content)
        frmTextView.ShowDialog()
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

    Private Function DirectoryEntryGetStats(DirectoryEntry As DiskImage.DirectoryEntry) As DirectoryStats
        Dim Stats As DirectoryStats

        With Stats
            .IsDirectory = DirectoryEntry.IsDirectory And Not DirectoryEntry.IsVolumeName
            .IsDeleted = DirectoryEntry.IsDeleted
            .IsModified = DirectoryEntry.IsModified
            .IsValidFile = DirectoryEntry.IsValidFile
            .IsValidDirectory = DirectoryEntry.IsValidDirectory
            .CanExport = DirectoryEntryCanExport(DirectoryEntry)
            .FileSize = DirectoryEntry.FileSize
            .FullFileName = DirectoryEntry.GetShortFileName(True)
            .CanDelete = DirectoryEntryCanDelete(DirectoryEntry, False)
            .CanUndelete = DirectoryEntryCanUndelete(DirectoryEntry)
            .CanInsert = DirectoryEntry.Index < DirectoryEntry.ParentDirectory.DirectoryEntries.Count - DirectoryEntry.ParentDirectory.Data.AvailableEntryCount
        End With

        Return Stats
    End Function

    Private Sub DiskImageProcess(CurrentImage As CurrentImage, DoItemScan As Boolean, ClearItems As Boolean)
        InitButtonState(CurrentImage)
        'PopulateSummary(CurrentImage)

        If CurrentImage IsNot Nothing AndAlso CurrentImage.Disk IsNot Nothing AndAlso CurrentImage.Disk.IsValidImage Then
            If CurrentImage.ImageData.CachedRootDir Is Nothing Then
                CurrentImage.ImageData.CachedRootDir = CurrentImage.Disk.RootDirectory.GetContent
            End If
            PopulateFilesPanel(CurrentImage, ClearItems)
        Else
            ClearFilesPanel(CurrentImage)
        End If

        PopulateSummary(CurrentImage)
        StatusStripBottom.Refresh()

        If DoItemScan Then
            ImageFiltersUpdate(CurrentImage)
            ComboImagesRefreshCurrentItemText()
            RefreshSaveButtons(CurrentImage)
        End If
    End Sub

    Private Sub DiskImageRefresh(CurrentImage As CurrentImage)
        If CurrentImage IsNot Nothing Then
            CurrentImage.ImageData.BottomIndex = ListViewFiles.GetBottomIndex
            CurrentImage.Disk?.Reinitialize()
        End If

        _SuppressEvent = True
        DiskImageProcess(CurrentImage, True, False)
        _SuppressEvent = False
    End Sub

    Private Function DiskImageSave(CurrentImage As CurrentImage, Optional NewFilePath As String = "") As Boolean
        Return DiskImageSave(CurrentImage, CurrentImage.ImageData, NewFilePath)
    End Function

    Private Function DiskImageSave(CurrentImage As CurrentImage, ImageData As ImageData, Optional NewFilePath As String = "") As Boolean
        Dim Image As CurrentImage = Nothing
        Dim Success As Boolean = False

        Do
            If ImageData Is CurrentImage.ImageData Then
                Image = CurrentImage
                Success = True
            Else
                Dim Disk = DiskImageLoad(ImageData)
                If Disk IsNot Nothing Then
                    Image = New CurrentImage(Disk, ImageData)
                    Success = True
                End If
            End If

            If Success Then
                If NewFilePath = "" Then
                    NewFilePath = Image.ImageData.GetSaveFile
                End If

                Dim Response = SaveDiskImageToFile(Image.Disk, NewFilePath)
                Success = (Response = SaveImageResponse.Success)

                If Response = SaveImageResponse.Unsupported Then
                    MsgBox("Saving to this image type is not supported.", MsgBoxStyle.Exclamation)
                    Exit Do
                ElseIf Response = SaveImageResponse.Unknown Then
                    MsgBox("Unsupported Disk Type.", MsgBoxStyle.Exclamation)
                    Exit Do
                ElseIf Response = SaveImageResponse.Cancelled Then
                    Exit Do
                End If
            End If

            If Not Success Then
                Dim Msg As String = $"Error saving file '{IO.Path.GetFileName(NewFilePath)}'."
                Dim ErrorResult = MsgBox(Msg, MsgBoxStyle.Critical + MsgBoxStyle.RetryCancel)
                If ErrorResult = MsgBoxResult.Cancel Then
                    Exit Do
                End If
            End If
        Loop Until Success

        If Success Then
            Image.ImageData.Checksum = CRC32.ComputeChecksum(Image.Disk.Image.GetBytes)
            Image.ImageData.ExternalModified = False
            ImageFiltersScanModified(Image.Disk, Image.ImageData)
        End If

        Return Success
    End Function

    Private Sub DiskImagesScan(CurrentImage As CurrentImage, NewOnly As Boolean)
        Me.UseWaitCursor = True
        Dim T = Stopwatch.StartNew

        MenuFiltersScanNew.Visible = False
        MenuFiltersScan.Enabled = False
        If ImageFilters.FiltersApplied Then
            FiltersClear(False)
            ImageCountUpdate()
        End If

        Dim ItemScanForm As New ItemScanForm(Me, ComboImages.Items, CurrentImage, NewOnly, ScanType.ScanTypeFilters)
        ItemScanForm.ShowDialog()
        MenuFiltersScanNew.Visible = ItemScanForm.ItemsRemaining > 0

        If _ExportUnknownImages Then
            _TitleDB.SaveNewXML()
        End If

        ImageFilters.UpdateAllMenuItems()

        RefreshModifiedCount()
        SubFiltersPopulate()

        ToolStripOEMNameCombo.Visible = True
        ToolStripOEMNameLabel.Visible = True
        ToolStripDiskTypeCombo.Visible = True
        ToolStripDiskTypeLabel.Visible = True

        MenuFiltersScan.Text = "Rescan Images"
        MenuFiltersScan.Enabled = True
        _ScanRun = True

        T.Stop()
        Debug.Print($"Image Scan Time Taken: {T.Elapsed}")
        Me.UseWaitCursor = False

        Dim Handle = WindowsAPI.GetForegroundWindow()
        If Handle = Me.Handle Then
            MainMenuFilters.ShowDropDown()
        Else
            WindowsAPI.FlashWindow(Me.Handle, True, True, 5, True)
        End If
    End Sub

    Private Sub DisplayChangeLog()
        Dim VersionLine As String
        Dim PublishedAt As String
        Dim Body As String
        Dim BodyArray() As String
        Dim Changelog = New StringBuilder()
        Dim ChangeLogString As String
        Dim ErrMsg As String = "An error occurred while downloading the change log.  Please try again later."

        Cursor.Current = Cursors.WaitCursor
        Dim ResponseText = GetChangeLogResponse()
        Cursor.Current = Cursors.Default

        If ResponseText = "" Then
            MsgBox(ErrMsg, MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        Try
            Dim JSON As List(Of Dictionary(Of String, Object)) = CompactJson.Serializer.Parse(Of List(Of Dictionary(Of String, Object)))(ResponseText)

            For Each Release In JSON
                If Release.ContainsKey("tag_name") Then
                    VersionLine = Release.Item("tag_name").ToString
                    If Release.ContainsKey("published_at") Then
                        PublishedAt = Release.Item("published_at").ToString
                        Dim PublishDate As Date
                        If Date.TryParse(PublishedAt, PublishDate) Then
                            VersionLine &= " (" & PublishDate.ToString & ")"
                        End If
                    End If
                    If Release.ContainsKey("body") Then
                        Body = Release.Item("body").ToString
                        If Body <> "" Then
                            Body = Replace(Body, Chr(13) & Chr(10), Chr(10))
                            BodyArray = Body.Split(Chr(10))
                            Changelog.AppendLine(VersionLine)
                            For Counter = 0 To BodyArray.Length - 1
                                Dim ChangeLine = BodyArray(Counter).Trim
                                If ChangeLine.Length > 0 Then
                                    If ChangeLine.Substring(0, 1) <> "-" Then
                                        ChangeLine = "- " & ChangeLine
                                    End If
                                    Changelog.AppendLine(ChangeLine)
                                End If
                            Next
                            Changelog.AppendLine("")
                        End If
                    End If
                End If
            Next

            ChangeLogString = Changelog.ToString

        Catch ex As Exception
            MsgBox(ErrMsg, MsgBoxStyle.Exclamation)
            DebugException(ex)
            Exit Sub
        End Try

        Dim frmTextView = New TextViewForm("Change Log", ChangeLogString)
        frmTextView.ShowDialog()
    End Sub

    Private Sub DisplayCrossLinkedFiles(Disk As Disk, DirectoryEntry As DiskImage.DirectoryEntry)
        Dim Msg As String = $"{DirectoryEntry.GetShortFileName(True)} is crosslinked with the following files:{vbCrLf}"

        For Each Crosslink In DirectoryEntry.CrossLinks
            If Crosslink IsNot DirectoryEntry Then
                Msg &= vbCrLf & Crosslink.GetShortFileName(True)
            End If
        Next
        MsgBox(Msg, MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
    End Sub

    Private Sub DisplayDirectoryContextMenu(Directory As IDirectory)
        MenuDirectoryView.Tag = Directory
        MenuDirectoryImportFiles.Tag = Directory
        MenuDirectoryNewDirectory.Tag = Directory
        ContextMenuDirectory.Show(MousePosition)
    End Sub

    Private Sub DragDropSelectedFiles()
        If ListViewFiles.SelectedItems.Count = 0 Then
            Exit Sub
        End If

        Dim TempPath As String = IO.Path.GetTempPath() & Guid.NewGuid().ToString() & "\"

        ImageFileExportToTemp(TempPath)

        If IO.Directory.Exists(TempPath) Then
            Dim FileList = IO.Directory.EnumerateDirectories(TempPath)
            For Each FilePath In IO.Directory.GetFiles(TempPath)
                FileList = FileList.Append(FilePath)
            Next
            If FileList.Count > 0 Then
                Dim Data = New DataObject(DataFormats.FileDrop, FileList.ToArray)
                ListViewFiles.DoDragDrop(Data, DragDropEffects.Move)
            End If
            IO.Directory.Delete(TempPath, True)
        End If
    End Sub

    Private Sub DrawComboFAT(ByVal sender As Object, ByVal e As DrawItemEventArgs)
        e.DrawBackground()

        If e.Index >= 0 Then
            Dim Item As String = ToolStripFATCombo.Items(e.Index)

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

    Private Sub ExportDebugScript(CurrentImage As CurrentImage)
        GenerateDebugPackage(CurrentImage)
    End Sub

    Private Sub FATEdit(CurrentImage As CurrentImage, Index As UShort)
        Dim frmFATEdit As New FATEditForm(CurrentImage.Disk, Index)

        frmFATEdit.ShowDialog()

        If frmFATEdit.Updated Then
            DiskImageRefresh(CurrentImage)
        End If
    End Sub

    Private Sub FATSubMenuRefresh(CurrentImage As CurrentImage, FATTablesMatch As Boolean)
        For Each Item As ToolStripMenuItem In MenuEditFAT.DropDownItems
            RemoveHandler Item.Click, AddressOf BtnEditFAT_Click
        Next
        MenuEditFAT.DropDownItems.Clear()
        MenuEditFAT.Tag = Nothing
        ToolStripFATCombo.Items.Clear()

        If CurrentImage IsNot Nothing AndAlso CurrentImage.Disk IsNot Nothing AndAlso CurrentImage.Disk.IsValidImage Then
            If FATTablesMatch Then
                MenuEditFAT.Tag = -1
            Else
                For Counter = 0 To CurrentImage.Disk.BPB.NumberOfFATs - 1
                    Dim Item As New ToolStripMenuItem With {
                       .Text = "FAT &" & Counter + 1,
                       .Tag = Counter
                    }
                    MenuEditFAT.DropDownItems.Add(Item)
                    AddHandler Item.Click, AddressOf BtnEditFAT_Click
                    ToolStripFATCombo.Items.Add("FAT " & Counter + 1)
                Next
                _SuppressEvent = True
                If CurrentImage.ImageData Is Nothing Then
                    ToolStripFATCombo.SelectedIndex = 0
                Else
                    If CurrentImage.ImageData.FATIndex > CurrentImage.Disk.BPB.NumberOfFATs - 1 Or FATTablesMatch Then
                        CurrentImage.ImageData.FATIndex = 0
                    End If
                    ToolStripFATCombo.SelectedIndex = CurrentImage.ImageData.FATIndex
                End If
                _SuppressEvent = False
            End If
        End If
    End Sub

    Private Sub FileClose(ImageData As ImageData)
        ItemFiltersRemove(ImageData)
        _LoadedFiles.FileNames.Remove(ImageData.DisplayPath)

        Dim ActiveComboBox As ComboBox = IIf(ImageFilters.FiltersApplied, ComboImagesFiltered, ComboImages)

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
            MenuToolsCompare.Enabled = ComboImages.Items.Count > 1
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

    Private Sub FileInfoUpdate(Disk As DiskImage.Disk)
        If ListViewFiles.SelectedItems.Count > 0 Then
            ToolStripFileCount.Text = $"{ListViewFiles.SelectedItems.Count} of {ListViewFiles.Items.Count} {"File".Pluralize(ListViewFiles.Items.Count)} Selected"
        Else
            ToolStripFileCount.Text = $"{ListViewFiles.Items.Count} {"File".Pluralize(ListViewFiles.Items.Count)}"
        End If
        ToolStripFileCount.Visible = True

        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim FileData As FileData = ListViewFiles.SelectedItems(0).Tag

            If FileData IsNot Nothing AndAlso FileData.DirectoryEntry.StartingCluster >= 2 Then

                Dim Sector = Disk.BPB.ClusterToSector(FileData.DirectoryEntry.StartingCluster)
                ToolStripFileSector.Text = $"Sector {Sector}"
                ToolStripFileSector.Visible = True

                Dim Track = Disk.BPB.SectorToTrack(Sector)
                Dim Side = Disk.BPB.SectorToSide(Sector)

                ToolStripFileTrack.Text = $"Track {Track}.{Side}"
                ToolStripFileTrack.Visible = True

                ToolStripFileTrack.GetCurrentParent.Refresh()
            Else
                ToolStripFileSector.Visible = False
                ToolStripFileTrack.Visible = False
            End If
        Else
            ToolStripFileSector.Visible = False
            ToolStripFileTrack.Visible = False
        End If
    End Sub

    Private Sub FilePropertiesEdit(CurrentImage As CurrentImage)
        Dim Result As Boolean = False

        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim Item As ListViewItem = ListViewFiles.SelectedItems.Item(0)
            Dim FileData As FileData = Item.Tag

            Dim frmFilePropertiesEdit As New FilePropertiesFormSingle(FileData.DirectoryEntry)
            frmFilePropertiesEdit.ShowDialog()
            If frmFilePropertiesEdit.DialogResult = DialogResult.OK Then
                Result = frmFilePropertiesEdit.Updated
            End If
        Else
            Dim Entries As New List(Of DirectoryEntry)
            For Each Item As ListViewItem In ListViewFiles.SelectedItems
                Dim FileData As FileData = Item.Tag
                Entries.Add(FileData.DirectoryEntry)
            Next

            Dim frmFilePropertiesEdit As New FilePropertiesFormMultiple(CurrentImage.Disk, Entries)
            frmFilePropertiesEdit.ShowDialog()
            If frmFilePropertiesEdit.DialogResult = DialogResult.OK Then
                Result = frmFilePropertiesEdit.Updated
            End If
        End If

        If Result Then
            DiskImageRefresh(CurrentImage)
        End If
    End Sub

    Private Sub FilesOpen()
        Dim FileFilter = GetLoadDialogFilters()

        Dim Dialog = New OpenFileDialog With {
            .Filter = FileFilter,
            .Multiselect = True
        }
        If Dialog.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If

        ProcessFileDrop(Dialog.FileNames, True)
    End Sub

    Private Sub FiltersApply(ResetSubFilters As Boolean)
        Dim HasFilter As Boolean = False
        Dim AppliedFilters As Integer = 0
        Dim r As RegularExpressions.Regex = Nothing

        If ResetSubFilters Then
            SubFiltersClearFilter()
        End If

        Dim FiltersChecked As Boolean = ImageFilters.AreFiltersApplied()
        If FiltersChecked Then
            AppliedFilters = ImageFilters.GetAppliedFilters(True)
        End If
        HasFilter = HasFilter Or FiltersChecked

        Dim TextFilter As String = ToolStripSearchText.Text.Trim.ToLower
        Dim HasTextFilter = TextFilter.Length > 0
        If HasTextFilter Then
            Dim Pattern As String = "(?:\W|^|_)(" & RegularExpressions.Regex.Escape(TextFilter) & ")"
            r = New RegularExpressions.Regex(Pattern, RegularExpressions.RegexOptions.IgnoreCase)
        End If
        HasFilter = HasFilter Or HasTextFilter

        Dim OEMNameItem As ComboFilterItem = ToolStripOEMNameCombo.SelectedItem
        Dim HasOEMNameFilter = OEMNameItem IsNot Nothing AndAlso Not OEMNameItem.AllItems
        HasFilter = HasFilter Or HasOEMNameFilter

        Dim DiskTypeItem As ComboFilterItem = ToolStripDiskTypeCombo.SelectedItem
        Dim HasDiskTypeFilter = DiskTypeItem IsNot Nothing AndAlso Not DiskTypeItem.AllItems
        HasFilter = HasFilter Or HasDiskTypeFilter

        If HasFilter Then
            Cursor.Current = Cursors.WaitCursor

            If ResetSubFilters Then
                SubFiltersClear()
            End If

            ComboImagesFiltered.BeginUpdate()
            ComboImagesFiltered.Items.Clear()

            For Each ImageData As ImageData In ComboImages.Items
                Dim ShowItem As Boolean = True

                If ShowItem AndAlso FiltersChecked Then
                    ShowItem = Not Filters.ImageFilters.IsFiltered(ImageData, AppliedFilters, ImageFilters.FilterCounts)
                End If

                If ShowItem AndAlso ResetSubFilters Then
                    SubFilterOEMNameAdd(ImageData.OEMName, False)
                    _SubFilterDiskType.Add(ImageData.DiskType, False)
                End If

                If ShowItem AndAlso HasTextFilter Then
                    ShowItem = r.IsMatch(ImageData.DisplayPath)
                End If

                If ShowItem AndAlso HasOEMNameFilter Then
                    Dim IsValidImage = Not ImageData.Filter(Filters.FilterTypes.Disk_UnknownFormat)
                    ShowItem = IsValidImage And OEMNameItem.Name = ImageData.OEMName
                End If

                If ShowItem AndAlso HasDiskTypeFilter Then
                    ShowItem = DiskTypeItem.Name = ImageData.DiskType
                End If

                If ShowItem Then
                    Dim Index = ComboImagesFiltered.Items.Add(ImageData)
                    If ImageData Is ComboImages.SelectedItem Then
                        ComboImagesFiltered.SelectedIndex = Index
                    End If
                End If
            Next

            ImageFilters.UpdateAllMenuItems()
            ImageFilters.FiltersApplied = True

            If ResetSubFilters Then
                SubFiltersPopulate()
            End If

            If ComboImagesFiltered.SelectedIndex = -1 AndAlso ComboImagesFiltered.Items.Count > 0 Then
                ComboImagesFiltered.SelectedIndex = 0
            End If

            ComboImagesFiltered.EndUpdate()

            ComboImagesToggle(True)
            RefreshSubFilterEnabled(ComboImagesFiltered)

            RefreshFilterButtons(True)

            Cursor.Current = Cursors.Default

        ElseIf ImageFilters.FiltersApplied Then
            FiltersClear(True)
            ImageFilters.UpdateAllMenuItems()
        End If

        ImageCountUpdate()
    End Sub

    Private Sub FiltersClear(ResetSubFilters As Boolean)
        Cursor.Current = Cursors.WaitCursor

        Dim FiltersApplied = ImageFilters.AreFiltersApplied()

        ImageFilters.Clear()
        SubFiltersClearFilter()

        If FiltersApplied Or ResetSubFilters Then
            SubFiltersPopulateUnfiltered()
        End If

        ComboImagesClear(ComboImagesFiltered)
        ComboImagesToggle(False)

        RefreshFilterButtons(False)

        Cursor.Current = Cursors.Default
    End Sub

    Private Sub FiltersReset()
        ImageFilters.Reset()
        SubFiltersReset()

        RefreshModifiedCount()

        MenuFiltersScan.Text = "Scan Images"
        RefreshFilterButtons(False)
    End Sub

    Private Sub GenerateTrackLayout()
        If _CurrentImage Is Nothing Then Exit Sub

        If Not _CurrentImage.Disk.Image.IsBitstreamImage Then
            Exit Sub
        End If

        Dim BitstreamImage = _CurrentImage.Disk.Image.BitstreamImage
        Dim Gap4A As UShort
        Dim Gap1 As UShort
        Dim Gap3List() As UShort
        Dim Gap3 As UShort
        Dim Gap3Match As Boolean
        Dim SectorCount As UShort
        Dim GapString As String
        Dim TrackString As String
        Dim PrevTrackString As String = ""
        Dim FirstTrack As UShort
        Dim Track As UShort
        Dim Side As UShort
        Dim TrackLayout As New StringBuilder

        For Track = 0 To BitstreamImage.TrackCount - 1 Step BitstreamImage.TrackStep
            TrackString = ""
            For Side = 0 To BitstreamImage.SideCount - 1
                Gap4A = 80
                Gap1 = 50
                Gap3List = New UShort(0) {80}
                Gap3Match = True
                Dim BitstreamTrack = BitstreamImage.GetTrack(Track, Side)
                Dim RegionList = Bitstream.IBM_MFM.MFMGetRegionList(BitstreamTrack.Bitstream, BitstreamTrack.TrackType)
                SectorCount = RegionList.Sectors.Count
                Gap4A = RegionList.Gap4A
                Gap1 = RegionList.Gap1
                If SectorCount > 1 Then
                    Gap3List = New UShort(SectorCount - 1) {}
                    For k = 0 To SectorCount - 2
                        Dim Sector = RegionList.Sectors.Item(k)
                        Gap3List(k) = Sector.Gap3
                        If k > 0 Then
                            If Gap3 <> Gap3List(k) Then
                                Gap3Match = False
                            End If
                        End If
                        Gap3 = Gap3List(k)
                    Next
                    Gap3List(SectorCount - 1) = Gap3List(SectorCount - 2)
                End If
                GapString = Gap4A & "," & Gap1 & ","
                If Gap3Match Then
                    GapString &= Gap3List(0)
                Else
                    GapString &= "["
                    For k = 0 To Gap3List.Length - 1
                        If k > 0 Then
                            GapString &= ","
                        End If
                        GapString &= Gap3List(k)
                    Next
                    GapString &= "]"
                End If

                If Side = 0 Then
                    TrackString = GapString
                ElseIf TrackString <> GapString Then
                    TrackString &= "." & GapString
                End If
            Next

            If Track = 0 Then
                FirstTrack = Track
            ElseIf TrackString <> PrevTrackString Then
                TrackLayout.AppendLine(FirstTrack & "-" & Track - 1 & ":" & PrevTrackString)
                FirstTrack = Track
            End If
            PrevTrackString = TrackString
        Next
        TrackLayout.AppendLine(FirstTrack & "-" & Track - 1 & ":" & PrevTrackString)

        Dim frmTextView = New TextViewForm("Tracklayout", TrackLayout.ToString)
        frmTextView.ShowDialog()
    End Sub

    Private Function GetModifiedImageList() As List(Of ImageData)
        Dim ModifyImageList As New List(Of ImageData)

        For Each ImageData As ImageData In ComboImages.Items
            If ImageData.Filter(Filters.FilterTypes.ModifiedFiles) Then
                ModifyImageList.Add(ImageData)
            End If
        Next

        Return ModifyImageList
    End Function

    Private Function GetNewFilePath(CurrentImage As CurrentImage) As String
        Return GetNewFilePath(CurrentImage, CurrentImage.ImageData)
    End Function

    Private Function GetNewFilePath(CurrentImage As CurrentImage, ImageData As ImageData) As String
        Dim Disk As DiskImage.Disk
        Dim FilePath = ImageData.GetSaveFile
        Dim NewFilePath As String = ""
        Dim DiskFormat As FloppyDiskFormat = FloppyDiskFormat.FloppyUnknown

        If ImageData Is CurrentImage.ImageData Then
            Disk = CurrentImage.Disk
        Else
            Disk = DiskImageLoad(ImageData)
        End If

        If Disk IsNot Nothing Then
            DiskFormat = Disk.DiskFormat
        End If

        Dim FileExt = IO.Path.GetExtension(FilePath)
        Dim FileFilter = GetSaveDialogFilters(DiskFormat, Disk.Image.ImageType, FileExt)

        Dim Dialog = New SaveFileDialog With {
            .InitialDirectory = IO.Path.GetDirectoryName(FilePath),
            .FileName = IO.Path.GetFileName(FilePath),
            .Filter = FileFilter.Filter,
            .FilterIndex = FileFilter.FilterIndex,
            .DefaultExt = FileExt
        }

        AddHandler Dialog.FileOk, Sub(sender As Object, e As CancelEventArgs)
                                      If Dialog.FileName <> FilePath AndAlso _LoadedFiles.FileNames.ContainsKey(Dialog.FileName) Then
                                          Dim Msg As String = IO.Path.GetFileName(Dialog.FileName) &
                                            $"{vbCrLf}This file is currently open in {Application.ProductName}." &
                                            $"Try again with a different file name."
                                          MsgBox(Msg, MsgBoxStyle.Exclamation, "Save As")
                                          e.Cancel = True
                                      End If
                                  End Sub

        If Dialog.ShowDialog = DialogResult.OK Then
            NewFilePath = Dialog.FileName
        End If

        Return NewFilePath
    End Function

    Private Function GetNormalizedDataByBadSectors(Disk As Disk) As Byte()
        Dim Data(Disk.Image.Length - 1) As Byte
        Disk.Image.CopyTo(Data, 0)
        Dim BytesPerCluster = Disk.BPB.BytesPerCluster()
        Dim Buffer(BytesPerCluster - 1) As Byte
        For Each Cluster In Disk.FATTables.FAT(0).BadClusters
            Dim Offset = Disk.BPB.ClusterToOffset(Cluster)
            If Offset + BytesPerCluster <= Data.Length Then
                Buffer.CopyTo(Data, Offset)
            End If
        Next
        Return Data
    End Function

    Private Function GetNormalizedDataByTrackList(Disk As Disk, TrackList As List(Of FloppyDB.BooterTrack)) As Byte()
        Dim BPB As BiosParameterBlock = BuildBPB(Disk.Image.Length)

        Dim Data(Disk.Image.Length - 1) As Byte
        Disk.Image.CopyTo(Data, 0)
        Dim BytesPerTrack = BPB.BytesPerSector * BPB.SectorsPerTrack
        Dim Buffer(BytesPerTrack - 1) As Byte
        For Each Track In TrackList
            Dim Offset = Disk.SectorToBytes(BPB.TrackToSector(Track.Track, Track.Side))
            If Offset + BytesPerTrack <= Data.Length Then
                Buffer.CopyTo(Data, Offset)
            End If
        Next
        Return Data
    End Function

    Private Function GetPathOffset() As Integer
        Dim PathName As String = ""
        Dim CheckPath As Boolean = False

        For Each ImageData As ImageData In ComboImages.Items
            Dim CurrentPathName As String = IO.Path.GetDirectoryName(ImageData.DisplayPath)
            If CheckPath Then
                Do While CurrentPathName.Split("\").Count > PathName.Split("\").Count
                    CurrentPathName = IO.Path.GetDirectoryName(CurrentPathName)
                Loop
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

    Private Sub HexDisplayBadSectors(CurrentImage As CurrentImage)
        Dim HexViewSectorData = HexViewBadSectors(CurrentImage.Disk)

        If DisplayHexViewForm(HexViewSectorData) Then
            DiskImageRefresh(CurrentImage)
        End If
    End Sub

    Private Sub HexDisplayBootSector(CurrentImage As CurrentImage)
        Dim HexViewSectorData = HexViewBootSector(CurrentImage.Disk)

        If DisplayHexViewForm(HexViewSectorData) Then
            DiskImageRefresh(CurrentImage)
        End If
    End Sub

    Private Sub HexDisplayDirectoryEntry(CurrentImage As CurrentImage, DirectoryEntry As DirectoryEntry)
        Dim HexViewSectorData = HexViewDirectoryEntry(CurrentImage.Disk, DirectoryEntry)

        If HexViewSectorData IsNot Nothing Then
            If DisplayHexViewForm(HexViewSectorData) Then
                DiskImageRefresh(CurrentImage)
            End If
        End If
    End Sub

    Private Sub HexDisplayDiskImage(CurrentImage As CurrentImage)
        Dim HexViewSectorData = New HexViewSectorData(CurrentImage.Disk, 0, CurrentImage.Disk.Image.Length) With {
            .Description = "Disk"
        }

        If DisplayHexViewForm(HexViewSectorData, True, True, False) Then
            DiskImageRefresh(CurrentImage)
        End If
    End Sub

    Private Sub HexDisplayFAT(CurrentImage As CurrentImage)
        Dim HexViewSectorData = HexViewFAT(CurrentImage.Disk)

        Dim SyncBlocks = CurrentImage.Disk.BPB.NumberOfFATEntries > 1 AndAlso Not IsDiskFormatXDF(CurrentImage.Disk.DiskFormat)

        If DisplayHexViewForm(HexViewSectorData, SyncBlocks) Then
            DiskImageRefresh(CurrentImage)
        End If
    End Sub

    Private Sub HexDisplayFreeClusters(CurrentImage As CurrentImage)
        Dim HexViewSectorData = New HexViewSectorData(CurrentImage.Disk, CurrentImage.Disk.FAT.GetFreeClusters(FAT12.FreeClusterEmum.WithData).ToList) With {
            .Description = "Free Clusters"
        }

        If DisplayHexViewForm(HexViewSectorData) Then
            DiskImageRefresh(CurrentImage)
        End If
    End Sub

    Private Sub HexDisplayLostClusters(CurrentImage As CurrentImage)
        Dim HexViewSectorData = HexViewLostClusters(CurrentImage.Disk)

        If DisplayHexViewForm(HexViewSectorData) Then
            DiskImageRefresh(CurrentImage)
        End If
    End Sub

    Private Sub HexDisplayOverdumpData(CurrentImage As CurrentImage)
        Dim Offset = CurrentImage.Disk.BPB.ReportedImageSize() + 1

        Dim HexViewSectorData = New HexViewSectorData(CurrentImage.Disk, Offset, CurrentImage.Disk.Image.Length - Offset) With {
            .Description = "Disk"
        }

        If DisplayHexViewForm(HexViewSectorData, True, True, False) Then
            DiskImageRefresh(CurrentImage)
        End If
    End Sub

    Private Sub HexDisplayRawTrackData(Disk As Disk, TrackIndex As Integer)
        Dim Track As UShort
        Dim Side As Byte
        Dim AllTracks As Boolean

        If TrackIndex = -1 Then
            Track = 0
            Side = 0
            AllTracks = True
        Else
            Track = TrackIndex \ Disk.Image.SideCount
            Side = TrackIndex Mod Disk.Image.SideCount
            AllTracks = False
        End If

        Dim Image = Disk.Image.BitstreamImage

        If Image IsNot Nothing Then
            Dim frmHexView As New HexViewRawForm(Disk, Track, Side, AllTracks)
            frmHexView.ShowDialog()
        End If
    End Sub

    Private Sub HexDisplayRootDirectory(CurrentImage As CurrentImage)
        Dim HexViewSectorData = HexViewRootDirectory(CurrentImage.Disk)

        If HexViewSectorData IsNot Nothing Then
            If DisplayHexViewForm(HexViewSectorData) Then
                DiskImageRefresh(CurrentImage)
            End If
        End If
    End Sub

    Private Sub ImageAddDirectory(CurrentImage As CurrentImage, ParentDirectory As IDirectory, Optional Index As Integer = -1)
        Dim Updated As Boolean

        Dim frmNewDirectory As New NewDirectoryForm()
        frmNewDirectory.ShowDialog()

        If frmNewDirectory.DialogResult = DialogResult.OK Then
            Updated = frmNewDirectory.Updated

            If Updated Then
                Dim Options As New AddFileOptions With {
                    .UseLFN = frmNewDirectory.HasLFN,
                    .UseNTExtensions = frmNewDirectory.UseNTExtensions
                }
                Dim AddDirectoryResponse = ParentDirectory.AddDirectory(frmNewDirectory.NewDirectoryData, Options, frmNewDirectory.NewFilename, Index)

                If AddDirectoryResponse.Entry IsNot Nothing Then
                    DiskImageRefresh(CurrentImage)
                Else
                    MsgBox($"Warning: Insufficient Disk Space", MsgBoxStyle.Exclamation)
                End If
            End If
        End If
    End Sub

    Private Sub ImageClearReservedBytes(CurrentImage As CurrentImage)
        If _TitleDB.IsVerifiedImage(CurrentImage.Disk) Then
            If Not MsgBoxQuestion("This is a verified image.  Are you sure you wish to clear reserved bytes from this image?") Then
                Exit Sub
            End If
        End If

        Dim Result = DiskImage.ClearReservedBytes(CurrentImage.Disk)

        If Result Then
            DiskImageRefresh(CurrentImage)
        End If
    End Sub

    Private Sub ImageCountUpdate()
        If ImageFilters.FiltersApplied Then
            ToolStripImageCount.Text = $"{ComboImagesFiltered.Items.Count} of {ComboImages.Items.Count} {"Image".Pluralize(ComboImages.Items.Count)}"
        Else
            ToolStripImageCount.Text = $"{ComboImages.Items.Count} {"Image".Pluralize(ComboImages.Items.Count)}"
        End If
    End Sub

    Private Sub ImageDeleteSelectedFiles(CurrentImage As CurrentImage, Remove As Boolean)
        Dim Msg As String
        Dim Title As String
        Dim DialogResult As DeleteFileForm.DeleteFileFormResult
        Dim Item As ListViewItem
        Dim FileData As FileData
        Dim Result As Boolean = False
        Dim CanFill As Boolean
        Dim DoRemove As Boolean
        Dim DisplayDialog As Boolean = False


        If ListViewFiles.SelectedItems.Count = 0 Then
            Exit Sub
        End If

        For Each Item In ListViewFiles.SelectedItems
            FileData = Item.Tag
            If FileData IsNot Nothing Then
                If Not FileData.DirectoryEntry.IsDeleted Then
                    DisplayDialog = True
                    Exit For
                End If
            End If
        Next

        If DisplayDialog Then
            If ListViewFiles.SelectedItems.Count = 1 Then
                Item = ListViewFiles.SelectedItems(0)
                FileData = Item.Tag
                Msg = $"Are you sure you wish to {If(Remove, "remove", "delete")} {FileData.DirectoryEntry.GetShortFileName(True)}?"
                Title = $"{If(Remove, "Remove", "Delete")} File"
                CanFill = FileData.DirectoryEntry.IsValidFile Or FileData.DirectoryEntry.IsValidDirectory
            Else
                Msg = $"Are you sure you wish to {If(Remove, "remove", "delete")} the selected files?"
                Title = $"{If(Remove, "Remove", "Delete")} {ListViewFiles.SelectedItems.Count} Files"
                CanFill = True
            End If


            Dim DeleteFileForm As New DeleteFileForm(Msg, Title, CanFill)
            DeleteFileForm.ShowDialog(Me)
            DialogResult = DeleteFileForm.Result
            DeleteFileForm.Close()

            If DialogResult.Cancelled Then
                Exit Sub
            End If
        Else
            DialogResult.Clear = False
            DialogResult.FillChar = 0
        End If

        Dim UseTransaction As Boolean = CurrentImage.Disk.BeginTransaction

        For Each Item In ListViewFiles.SelectedItems
            DoRemove = False
            FileData = Item.Tag
            If FileData IsNot Nothing Then
                If FileData.DirectoryEntry.IsDeleted Then
                    DoRemove = True
                ElseIf DirectoryEntryDelete(FileData.DirectoryEntry, DialogResult.FillChar, DialogResult.Clear) Then
                    Result = True
                    DoRemove = True
                End If
            End If

            If DoRemove And Remove Then
                Dim ParentDirectory = FileData.DirectoryEntry.ParentDirectory
                Dim Index = FileData.DirectoryEntry.Index

                If ParentDirectory.RemoveEntry(Index) Then
                    Result = True
                End If
            End If
        Next

        If UseTransaction Then
            CurrentImage.Disk.EndTransaction()
        End If

        If Result Then
            DiskImageRefresh(CurrentImage)
        End If
    End Sub

    Private Sub ImageFileExport()
        If ListViewFiles.SelectedItems.Count = 1 Then
            DirectoryEntryExport(ListViewFiles.SelectedItems(0).Tag)
        ElseIf ListViewFiles.SelectedItems.Count > 1 Then
            ImageFileExportSelected()
        End If
    End Sub

    Private Sub ImageFileExportSelected()
        Dim Dialog = New FolderBrowserDialog

        If Dialog.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If
        Dim Path = Dialog.SelectedPath

        Dim ShowDialog As Boolean = True
        Dim BatchResult As MyMsgBoxResult = MyMsgBoxResult.Yes
        Dim Result As MyMsgBoxResult = MyMsgBoxResult.Yes
        Dim FileCount As Integer = 0
        Dim TotalFiles As Integer = 0
        For Each Item As ListViewItem In ListViewFiles.SelectedItems
            Dim FileData As FileData = Item.Tag
            If FileData IsNot Nothing Then
                Dim DirectoryEntry = FileData.DirectoryEntry
                If DirectoryEntryCanExport(DirectoryEntry) Then
                    TotalFiles += 1
                    If Result <> MyMsgBoxResult.Cancel Then
                        If DirectoryEntry.IsDirectory Then
                            Dim PathName = IO.Path.Combine(Path, CleanPathName(FileData.FilePath), CleanFileName(DirectoryEntry.GetFullFileName))

                            If Not IO.Directory.Exists(PathName) Then
                                IO.Directory.CreateDirectory(PathName)
                            End If
                            FileCount += 1
                        Else
                            Dim PathName = IO.Path.Combine(Path, CleanPathName(FileData.FilePath))

                            If Not IO.Directory.Exists(PathName) Then
                                IO.Directory.CreateDirectory(PathName)
                            End If

                            Dim FileName = CleanFileName(DirectoryEntry.GetFullFileName)

                            Dim FilePath = IO.Path.Combine(PathName, FileName)

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
                                If DirectoryEntrySaveToFile(FilePath, DirectoryEntry) Then
                                    FileCount += 1
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Next

        Dim Msg As String = $"{FileCount} of {TotalFiles} {"file".Pluralize(TotalFiles)} exported successfully."
        MsgBox(Msg, MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
    End Sub

    Private Sub ImageFileExportToTemp(TempPath As String)
        For Each Item As ListViewItem In ListViewFiles.SelectedItems
            Dim FileData As FileData = Item.Tag
            If FileData IsNot Nothing Then
                Dim DirectoryEntry = FileData.DirectoryEntry

                If DirectoryEntryCanExport(DirectoryEntry) Then
                    If DirectoryEntry.IsDirectory Then
                        Dim PathName = IO.Path.Combine(TempPath, CleanPathName(FileData.FilePath), CleanFileName(DirectoryEntry.GetFullFileName))

                        If Not IO.Directory.Exists(PathName) Then
                            IO.Directory.CreateDirectory(PathName)
                        End If
                    Else
                        Dim PathName = IO.Path.Combine(TempPath, CleanPathName(FileData.FilePath))

                        If Not IO.Directory.Exists(PathName) Then
                            IO.Directory.CreateDirectory(PathName)
                        End If

                        Dim FileName = CleanFileName(DirectoryEntry.GetFullFileName)

                        Dim FilePath = IO.Path.Combine(PathName, FileName)

                        DirectoryEntrySaveToFile(FilePath, DirectoryEntry)
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub ImageFiltersScan(Disk As Disk, ImageData As ImageData, Remove As Boolean)
        ImageFiltersScanModified(Disk, ImageData, True, Remove)

        RefreshModifiedCount()

        If ImageData.Scanned Then
            ImageFiltersScanDisk(Disk, ImageData, True, Remove)
            ImageFiltersScanOEMName(Disk, ImageData, True, Remove)
            ImageSubFilterOEMNameUpdate(Disk, ImageData, True, Remove)
            ImageSubFilterDiskTypeUpdate(Disk, ImageData, True, Remove)
            ImageFiltersScanFreeClusters(Disk, ImageData, True, Remove)
            ImageFiltersScanDirectory(Disk, ImageData, True, Remove)
        End If
    End Sub

    Private Sub ImageFiltersUpdate(CurrentImage As CurrentImage)
        ImageFiltersScan(CurrentImage.Disk, CurrentImage.ImageData, False)
    End Sub

    Private Sub ImageFixFileSize(CurrentImage As CurrentImage, DirectoryEntry As DirectoryEntry)
        If Not DirectoryEntry.HasIncorrectFileSize Then
            Exit Sub
        End If

        DirectoryEntry.FileSize = DirectoryEntry.GetAllocatedSizeFromFAT
        DiskImageRefresh(CurrentImage)
    End Sub

    Private Sub ImageFixImageSize(CurrentImage As CurrentImage)
        Dim Result As Boolean = True

        If _TitleDB.IsVerifiedImage(CurrentImage.Disk) Then
            If Not MsgBoxQuestion("This is a verified image.  Are you sure you wish to adjust the image size for this image?") Then
                Exit Sub
            End If
        End If

        Dim Compare = CurrentImage.Disk.CheckImageSize

        If Compare = 0 Then
            Result = False
        ElseIf Compare < 0 Then
            Result = MsgBoxQuestion($"The image size is smaller than the detected size.{vbCrLf}{vbCrLf}Are you sure you wish to increase the image size?")
        Else
            Dim ReportedSize = CurrentImage.Disk.BPB.ReportedImageSize
            Dim Data = CurrentImage.Disk.Image.GetBytes(ReportedSize, CurrentImage.Disk.Image.Length - ReportedSize)
            Dim HasData As Boolean = False
            For Each b In Data
                If b <> 0 Then
                    HasData = True
                    Exit For
                End If
            Next
            If HasData Then
                Result = MsgBoxQuestion($"There is data in the overdumped region of the image.{vbCrLf}{vbCrLf}Are you sure you wish to truncate this image?")
            End If
        End If

        If Result Then
            If DiskImage.FixImageSize(CurrentImage.Disk) Then
                DiskImageRefresh(CurrentImage)
            End If
        End If
    End Sub

    Private Sub ImageImport(CurrentImage As CurrentImage, ParentDirectory As IDirectory, Multiselect As Boolean, Optional Index As Integer = -1)
        Dim FileNames() As String = New String(-1) {}

        If My.Settings.DragAndDrop Then
            Dim frmFileDrop As New FileDropForm()

            If frmFileDrop.ShowDialog() <> DialogResult.OK Then
                Exit Sub
            End If

            FileNames = frmFileDrop.FileNames
        Else
            Dim Dialog = New OpenFileDialog With {
               .Multiselect = Multiselect
           }
            If Dialog.ShowDialog <> DialogResult.OK Then
                Exit Sub
            End If

            FileNames = Dialog.FileNames
        End If

        Dim ImportFilesForm As New ImportFileForm(ParentDirectory, FileNames)
        Dim Result = ImportFilesForm.ShowDialog()

        If Result = DialogResult.Cancel Then
            Exit Sub
        End If

        Dim FileList = ImportFilesForm.FileList

        If FileList.SelectedFiles < 1 Then
            Exit Sub
        End If

        Dim UseTransaction = ParentDirectory.Disk.BeginTransaction

        Dim FilesAdded As UInteger = 0
        Dim FileCount As UInteger = FileList.SelectedFiles

        ImageImportFolders(FileList.DirectoryList, ParentDirectory, Index, FileList.Options, FilesAdded)
        ImageImportFiles(FileList.FileList, ParentDirectory, Index, FileList.Options, FilesAdded)

        If FilesAdded < FileCount Then
            MsgBox($"Warning: Insufficient Disk Space{vbCrLf}{vbCrLf}{FilesAdded} of {FileCount} file(s) added successfully", MsgBoxStyle.Exclamation)
        End If

        If UseTransaction Then
            ParentDirectory.Disk.EndTransaction()
        End If

        If FilesAdded > 0 Then
            DiskImageRefresh(CurrentImage)
        End If
    End Sub

    Private Sub ImageImportFiles(FileList As List(Of ImportFile), ParentDirectory As IDirectory, Index As Integer, Options As AddFileOptions, ByRef FilesAdded As UInteger)
        For Each File In FileList
            If File.IsSelected Then
                Dim FileInfo As New IO.FileInfo(File.FilePath)
                Dim EntriesAdded = ParentDirectory.AddFile(FileInfo, Options, Index)
                If EntriesAdded > -1 Then
                    FilesAdded += 1
                    If Index > -1 Then
                        Index += EntriesAdded
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub ImageImportFolders(FolderList As List(Of ImportDirectory), ParentDirectory As IDirectory, Index As Integer, Options As AddFileOptions, ByRef FilesAdded As UInteger)
        For Each Folder In FolderList
            If Folder.SelectedFiles > 0 Then
                Dim DirectoryInfo As New IO.DirectoryInfo(Folder.FilePath)

                Dim DirectoryEntry = New DirectoryEntryBase
                DirectoryEntry.SetFileInfo(DirectoryInfo, Options.UseCreatedDate, Options.UseLastAccessedDate)

                Dim AddDirectoryResponse = ParentDirectory.AddDirectory(DirectoryEntry.Data, Options, DirectoryInfo.Name, Index)
                If AddDirectoryResponse.Entry IsNot Nothing Then
                    FilesAdded += 1
                    If Index > -1 Then
                        Index += AddDirectoryResponse.EntriesNeeded
                    End If

                    ImageImportFolders(Folder.DirectoryList, AddDirectoryResponse.Entry.SubDirectory, -1, Options, FilesAdded)
                    ImageImportFiles(Folder.FileList, AddDirectoryResponse.Entry.SubDirectory, -1, Options, FilesAdded)
                End If
            End If
        Next
    End Sub
    Private Function ImageIsWin9xDisk(Disk As Disk) As Boolean
        Dim Response As Boolean = False
        Dim OEMNameWin9x = Disk.BootSector.IsWin9xOEMName
        Dim OEMName = Disk.BootSector.OEMName

        If OEMNameWin9x Then
            Dim BootstrapType = _BootStrapDB.FindMatch(Disk.BootSector.BootStrapCode)

            If BootstrapType IsNot Nothing Then
                For Each KnownOEMName In BootstrapType.OEMNames
                    If KnownOEMName.Name.CompareTo(OEMName) Or KnownOEMName.Win9xId Then
                        Return True
                        Exit For
                    End If
                Next
            End If
        End If

        Return Response
    End Function

    Private Sub ImageNew()
        Dim frmImageCreationForm As New ImageCreationForm()
        frmImageCreationForm.ShowDialog()

        Dim Data = frmImageCreationForm.Data
        Dim DiskFormat = frmImageCreationForm.DiskFormat

        If Data IsNot Nothing Then
            Dim FileName = FloppyDiskSaveFile(Data, DiskFormat, _LoadedFiles.FileNames)
            If FileName.Length > 0 Then
                ProcessFileDrop(FileName)
            End If
        End If
    End Sub

    Private Sub ImageReplaceFile(CurrentImage As CurrentImage, DirectoryEntry As DirectoryEntry)
        Dim Dialog = New OpenFileDialog
        Dim FormResult As ReplaceFileForm.ReplaceFileFormResult

        If Dialog.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If

        Dim FileInfo As New IO.FileInfo(Dialog.FileName)

        Dim AvailableSpace = DirectoryEntry.Disk.FAT.GetFreeSpace() + DirectoryEntry.GetSizeOnDisk

        Dim ReplaceFileForm As New ReplaceFileForm(AvailableSpace, DirectoryEntry.ParentDirectory)
        With ReplaceFileForm
            .SetOriginalFile(DirectoryEntry.GetShortFileName, DirectoryEntry.GetLastWriteDate.DateObject, DirectoryEntry.FileSize)
            .SetNewFile(DirectoryEntry.ParentDirectory.GetAvailableShortFileName(FileInfo.Name, False, DirectoryEntry.Index), FileInfo.LastWriteTime, FileInfo.Length)
            .RefreshText()
            .ShowDialog(Me)
            FormResult = .Result
            .Close()
        End With

        If Not FormResult.Cancelled Then
            Dim UseTransaction As Boolean = DirectoryEntry.Disk.BeginTransaction

            Dim Result As Boolean = False
            Dim FreeClusters = DirectoryEntry.Disk.FAT.GetFreeClusters(FAT12.FreeClusterEmum.WithoutData)

            Result = DirectoryEntry.UpdateFile(Dialog.FileName, FormResult.FileSize, FormResult.FillChar, FreeClusters)
            If Not Result Then
                Result = DirectoryEntry.UpdateFile(Dialog.FileName, FormResult.FileSize, FormResult.FillChar)
            End If

            If Result Then
                If FormResult.FileNameChanged Then
                    DirectoryEntry.SetFileName(FormResult.FileName)
                End If

                If FormResult.FileDateChanged Then
                    DirectoryEntry.SetLastWriteDate(FormResult.FileDate)
                End If
            End If

            If UseTransaction Then
                DirectoryEntry.Disk.EndTransaction()
            End If

            If Result Then
                DiskImageRefresh(CurrentImage)
            End If
        End If
    End Sub
    Private Sub ImageRestructure(CurrentImage As CurrentImage)
        If _TitleDB.IsVerifiedImage(CurrentImage.Disk) Then
            If Not MsgBoxQuestion("This is a verified image.  Are you sure you wish to restructure this image?") Then
                Exit Sub
            End If
        End If

        DiskImage.RestructureImage(CurrentImage.Disk)

        DiskImageRefresh(CurrentImage)
    End Sub

    Private Sub ImageUndeleteFile(CurrentImage As CurrentImage, DirectoryEntry As DirectoryEntry)
        If Not DirectoryEntryCanUndelete(DirectoryEntry) Then
            Exit Sub
        End If

        Dim UndeleteForm As New UndeleteForm(DirectoryEntry.GetShortFileName)

        UndeleteForm.ShowDialog()

        If UndeleteForm.DialogResult = DialogResult.OK Then
            Dim FirstChar = UndeleteForm.FirstChar
            If FirstChar > 0 Then
                DirectoryEntry.UnDelete(FirstChar)

                DiskImageRefresh(CurrentImage)
            End If
        End If
    End Sub

    Private Sub ImageWin9xCleanBatch(CurrentImage As CurrentImage)
        Dim Msg = $"This will restore the OEM Name and remove any Creation and Last Accessed Dates from all unverified loaded images.{vbCrLf}{vbCrLf}Do you wish to proceed??"

        If Not MsgBoxQuestion(Msg) Then
            Exit Sub
        End If

        Me.UseWaitCursor = True
        Dim T = Stopwatch.StartNew

        Dim ItemScanForm As New ItemScanForm(Me, ComboImages.Items, CurrentImage, False, ScanType.ScanTypeWin9xClean)
        ItemScanForm.ShowDialog()

        ImageFilters.UpdateAllMenuItems()

        RefreshModifiedCount()
        If _ScanRun Then
            SubFiltersPopulate()
        End If

        Dim UpdateCount As Integer = 0
        For Index = 0 To ComboImages.Items.Count - 1
            Dim ImageData As ImageData = ComboImages.Items(Index)
            If ImageData.BatchUpdated Then
                If ImageData Is CurrentImage.ImageData Then
                    DiskImageRefresh(CurrentImage)
                End If
                ImageData.BatchUpdated = False
                UpdateCount += 1
            End If
        Next Index

        T.Stop()
        Debug.Print($"Batch Process Time Taken: {T.Elapsed}")
        Me.UseWaitCursor = False

        Msg = UpdateCount & " image" & IIf(UpdateCount <> 1, "s", "") & " cleaned."
        MsgBox(Msg, MsgBoxStyle.Information)

        If UpdateCount > 0 Then
            FiltersApply(True)
        End If
    End Sub

    Private Sub ImageWin9xCleanCurrent(CurrentImage As CurrentImage)
        If _TitleDB.IsVerifiedImage(CurrentImage.Disk) Then
            If Not MsgBoxQuestion("This is a verified image.  Are you sure you wish to remove any windows modifications from this image?") Then
                Exit Sub
            End If
        End If

        Dim Result = ImageWin9xClean(CurrentImage.Disk, False)

        If Result Then
            DiskImageRefresh(CurrentImage)
        End If
    End Sub

    Private Sub InitButtonState(CurrentImage As CurrentImage)
        Dim FATTablesMatch As Boolean = True
        Dim PrevVisible = ToolStripFATCombo.Visible

        If CurrentImage IsNot Nothing AndAlso CurrentImage.Disk IsNot Nothing Then
            FATTablesMatch = IsDiskFormatXDF(CurrentImage.Disk.DiskFormat) OrElse CurrentImage.Disk.FATTables.FATsMatch
            MenuHexBootSector.Enabled = CurrentImage.Disk.CheckSize
            MenuEditBootSector.Enabled = CurrentImage.Disk.CheckSize
            MenuHexDisk.Enabled = CurrentImage.Disk.CheckSize
            MenuHexFAT.Enabled = CurrentImage.Disk.IsValidImage
            MenuEditFAT.Enabled = CurrentImage.Disk.IsValidImage
            MenuHexDirectory.Enabled = CurrentImage.Disk.IsValidImage
            ToolStripSeparatorFAT.Visible = Not FATTablesMatch
            ToolStripFATCombo.Visible = Not FATTablesMatch
            ToolStripFATCombo.Width = 60
            MenuDiskWriteFloppyA.Enabled = _DriveAEnabled
            MenuDiskWriteFloppyB.Enabled = _DriveBEnabled
            MenuFileSaveAs.Enabled = True
        Else
            MenuHexBootSector.Enabled = False
            MenuHexDisk.Enabled = False
            MenuHexFAT.Enabled = False
            MenuEditBootSector.Enabled = False
            MenuEditFAT.Enabled = False
            MenuHexDirectory.Enabled = False
            ToolStripSeparatorFAT.Visible = False
            ToolStripFATCombo.Visible = False
            MenuDiskWriteFloppyA.Enabled = False
            MenuDiskWriteFloppyB.Enabled = False
            MenuFileSaveAs.Enabled = False
        End If
        MenuToolsWin9xClean.Enabled = False
        MenuToolsClearReservedBytes.Enabled = False
        ToolStripSaveAs.Enabled = MenuFileSaveAs.Enabled

        MenuDisplayDirectorySubMenuClear()
        FATSubMenuRefresh(CurrentImage, FATTablesMatch)

        RefreshSaveButtons(CurrentImage)

        If ToolStripFATCombo.Visible <> PrevVisible Then
            ToolStripTop.Refresh()
        End If
    End Sub

    Private Sub InitDebugFeatures(Enabled As Boolean)
        If Enabled Then
            MenuOptionsSeparatorDebug.Visible = True
            MenuOptionsExportUnknown.Visible = True
            MenuOptionsExportUnknown.Checked = _ExportUnknownImages
        Else
            MenuOptionsSeparatorDebug.Visible = False
            MenuOptionsExportUnknown.Visible = False
        End If
    End Sub

    Private Sub ItemFiltersRemove(ImageData As ImageData)
        ImageFiltersScan(Nothing, ImageData, True)
    End Sub
    Private Sub ItemSelectionChanged(CurrentImage As CurrentImage)
        RefreshFileButtons(CurrentImage)
        FileInfoUpdate(CurrentImage.Disk)
        RefreshCheckAll()
    End Sub

    Private Sub ListViewAutoSize()
        For Each Column As ColumnHeader In ListViewFiles.Columns
            If Column.Width > 0 Then
                Column.Width = -2
                If Column.Width < _ListViewWidths(Column.Index) Then
                    Column.Width = _ListViewWidths(Column.Index)
                End If
            End If
        Next
    End Sub

    Private Function ListViewFilesAddEmpty(Group As ListViewGroup, ItemIndex As Integer) As ListViewItem
        Dim SI As ListViewItem.ListViewSubItem

        Dim Item = New ListViewItem("", Group) With {
            .UseItemStyleForSubItems = False,
            .Tag = Nothing
        }
        SI = Item.SubItems.Add("No Files")
        For Counter = 0 To 10
            Item.SubItems.Add("")
        Next

        If ListViewFiles.Items.Count <= ItemIndex Then
            ListViewFiles.Items.Add(Item)
        Else
            ListViewFiles.Items.Item(ItemIndex) = Item
        End If

        Return Item
    End Function

    Private Function ListViewFilesAddGroup(Directory As DiskImage.IDirectory, Path As String, GroupIndex As Integer) As ListViewGroup
        Dim FileCount As UInteger = Directory.Data.FileCount
        Dim GroupName As String = IIf(Path = "", "(Root)", Path)
        GroupName = GroupName & "  (" & FileCount & IIf(FileCount <> 1, " entries", " entry") _
            & IIf(Directory.Data.HasBootSector, ", Boot Sector", "") _
            & IIf(Directory.Data.HasAdditionalData, ", Additional Data", "") _
            & ")"

        Dim Group = New ListViewGroup(GroupName) With {
            .Tag = Directory
        }
        ListViewFiles.Groups.Add(Group)

        Return Group
    End Function

    Private Function ListViewFilesAddItem(FileData As FileData, Group As ListViewGroup, ItemIndex As Integer) As ListViewItem
        Dim Item = ListViewFilesGetItem(Group, FileData)

        If ListViewFiles.Items.Count <= ItemIndex Then
            ListViewFiles.Items.Add(Item)
        Else
            ListViewFiles.Items.Item(ItemIndex) = Item
        End If

        Return Item
    End Function

    Private Sub ListViewFilesClearModifiedFlag()
        ListViewFiles.BeginUpdate()

        For Each Item As ListViewItem In ListViewFiles.Items
            If Item.SubItems(0).Text = "#" Then
                Item.SubItems(0) = New ListViewItem.ListViewSubItem()
            End If
        Next

        ListViewFiles.EndUpdate()
    End Sub

    Private Sub ListViewFilesRemoveUnused(ItemCount As Integer)
        For Counter = ListViewFiles.Items.Count - 1 To ItemCount Step -1
            ListViewFiles.Items.RemoveAt(Counter)
        Next
        For Counter = ListViewFiles.Groups.Count - 1 To 0 Step -1
            Dim Group = ListViewFiles.Groups.Item(Counter)
            If Group.Items.Count = 0 Then
                ListViewFiles.Groups.Remove(Group)
            End If
        Next
    End Sub

    Private Sub ListViewFilesReset()
        ClearSort(False)
        ListViewFiles.BeginUpdate()
        ListViewFiles.Items.Clear()
        ListViewFiles.Groups.Clear()
        ListViewFiles.MultiSelect = False
        ListViewFiles.EndUpdate()
    End Sub

    Private Sub ListViewInit()
        ReDim _ListViewWidths(_ListViewFiles.Columns.Count - 1)
        For Each Column As ColumnHeader In ListViewFiles.Columns
            _ListViewWidths(Column.Index) = Column.Width
        Next

        FileCreationDate.Width = 0
        FileLastAccessDate.Width = 0
        FileLFN.Width = 0
        FileClusterError.Width = 0
        FileNTReserved.Width = 0
        FileFAT32Cluster.Width = 0
    End Sub

    Private Sub LoadCurrentImage(ImageData As ImageData, DoItemScan As Boolean)
        Cursor.Current = Cursors.WaitCursor

        Dim Disk = DiskImageLoad(ImageData, True)

        _CurrentImage = New CurrentImage(Disk, ImageData)

        ClearSort(False)

        If _CurrentImage IsNot Nothing Then
            If _CurrentImage.ImageData.ExternalModified Then
                _CurrentImage.ImageData.ExternalModified = False
                DoItemScan = True
                Dim Msg = "Image has been modified by another application." & vbCrLf & vbCrLf & "Changes have been discarded."
                MsgBox(Msg, MsgBoxStyle.Exclamation)
            End If

            DiskImageProcess(_CurrentImage, DoItemScan, True)
        End If

        Cursor.Current = Cursors.Default
    End Sub

    Private Sub MenuDisplayDirectorySubMenuClear()
        For Each Item As ToolStripMenuItem In MenuHexDirectory.DropDownItems
            RemoveHandler Item.Click, AddressOf BtnDisplayDirectory_Click
        Next
        MenuHexDirectory.DropDownItems.Clear()
        MenuHexDirectory.Text = "Root &Directory"
    End Sub

    Private Sub MenuDisplayDirectorySubMenuItemAdd(Path As String, Directory As IDirectory, Index As Integer)
        Dim Item As New ToolStripMenuItem With {
            .Text = Path,
            .Tag = Directory
        }
        If Index = -1 Then
            MenuHexDirectory.DropDownItems.Add(Item)
        Else
            MenuHexDirectory.DropDownItems.Insert(Index, Item)
        End If
        AddHandler Item.Click, AddressOf BtnDisplayDirectory_Click
    End Sub

    Private Sub MenuRawTrackDataSubMenuClear()
        For Each Item As ToolStripMenuItem In MenuHexRawTrackData.DropDownItems
            RemoveHandler Item.Click, AddressOf BtnRawTrackData_Click
        Next
        MenuHexRawTrackData.DropDownItems.Clear()
    End Sub

    Private Sub MenuRawTrackDataSubMenuItemAdd(Track As Integer, Text As String)
        Dim Item As New ToolStripMenuItem With {
           .Text = Text,
           .Tag = Track
       }
        MenuHexRawTrackData.DropDownItems.Add(Item)
        AddHandler Item.Click, AddressOf BtnRawTrackData_Click
    End Sub

    Private Function PopulateFilesPanel(CurrentImage As CurrentImage, ClearItems As Boolean) As DirectoryScanResponse
        MenuDisplayDirectorySubMenuClear()

        ListViewFiles.BeginUpdate()

        ListViewFiles.ListViewItemSorter = Nothing

        If ClearItems Then
            ListViewFiles.Items.Clear()
            ListViewFiles.Groups.Clear()
        End If
        ListViewFiles.MultiSelect = True

        Dim Response = ProcessDirectoryEntries(CurrentImage.Disk.RootDirectory, False)

        If MenuHexDirectory.DropDownItems.Count > 0 Then
            MenuHexDirectory.Text = "Directory"
            MenuDisplayDirectorySubMenuItemAdd("(Root)", CurrentImage.Disk.RootDirectory, 0)
            MenuHexDirectory.Tag = Nothing
        Else
            MenuHexDirectory.Tag = CurrentImage.Disk.RootDirectory
        End If

        If Not ClearItems Then
            ListViewFilesRemoveUnused(Response.ItemCount)
        End If

        ProcessDirectoryScanResponse(CurrentImage, Response)

        ListViewAutoSize()

        ListViewFiles.ListViewItemSorter = _lvwColumnSorter

        If CurrentImage.ImageData.SortHistory IsNot Nothing Then
            If CurrentImage.ImageData.SortHistory.Count > 0 Then
                For Each Sort In CurrentImage.ImageData.SortHistory
                    _lvwColumnSorter.Sort(Sort)
                    ListViewFiles.Sort()
                    ListViewFiles.SetSortIcon(_lvwColumnSorter.SortColumn, _lvwColumnSorter.Order)
                Next
                BtnResetSort.Enabled = True
            End If
        End If

        If ClearItems Then
            If CurrentImage.ImageData IsNot Nothing AndAlso CurrentImage.ImageData.BottomIndex > -1 Then
                If CurrentImage.ImageData.BottomIndex < ListViewFiles.Items.Count Then
                    ListViewFiles.EnsureVisible(CurrentImage.ImageData.BottomIndex)
                End If
            End If
        End If

        ListViewFiles.EndUpdate()
        ListViewFiles.Refresh()

        ItemSelectionChanged(CurrentImage)

        Return Response
    End Function

    Private Sub PopulateHashPanel(Disk As Disk, MD5 As String)
        With ListViewHashes
            .BeginUpdate()
            .Items.Clear()

            If Disk IsNot Nothing Then
                .AddItem("CRC32", Disk.Image.GetCRC32, False)
                .AddItem("MD5", MD5, False)
                .AddItem("SHA-1", Disk.Image.GetSHA1Hash, False)
            End If
            .EndUpdate()
            .Refresh()
        End With
    End Sub

    Private Sub PopulateSummary(CurrentImage As CurrentImage)
        Dim MD5 As String = ""

        If CurrentImage IsNot Nothing AndAlso CurrentImage.Disk IsNot Nothing Then
            MD5 = CurrentImage.Disk.Image.GetMD5Hash
        End If

        SetCurrentFileName(CurrentImage.ImageData)
        PopulateSummaryPanel(CurrentImage, MD5)
        PopulateHashPanel(CurrentImage.Disk, MD5)
        RefreshDiskButtons(CurrentImage)
    End Sub

    Private Sub PopulateSummaryPanel(CurrentImage As CurrentImage, MD5 As String)
        With ListViewSummary
            .BeginUpdate()
            .Items.Clear()
            .Groups.Clear()

            If CurrentImage.Disk IsNot Nothing Then
                PopulateSummaryPanelMain(ListViewSummary, CurrentImage.Disk, _TitleDB, _BootStrapDB, MD5)

                .HideSelection = False
                .TabStop = True
                btnRetry.Visible = False
            Else
                PopulateSummaryPanelError(ListViewSummary, CurrentImage.ImageData.InvalidImage)

                .HideSelection = True
                .TabStop = False
                btnRetry.Visible = Not CurrentImage.ImageData.InvalidImage
            End If

            .EndUpdate()
            .Refresh()
        End With
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

    Private Function ProcessDirectoryEntries(Directory As DiskImage.IDirectory, ScanOnly As Boolean) As DirectoryScanResponse
        Dim ItemIndex As Integer = 0

        Dim Response = ProcessDirectoryEntries(Directory, 0, "", ScanOnly, 0, ItemIndex)
        Response.ItemCount = ItemIndex

        Return Response
    End Function

    Private Function ProcessDirectoryEntries(Directory As DiskImage.IDirectory, Offset As UInteger, Path As String, ScanOnly As Boolean, ByRef GroupIndex As Integer, ByRef ItemIndex As Integer) As DirectoryScanResponse
        Dim Response As New DirectoryScanResponse(Directory)
        Dim Group As ListViewGroup = Nothing

        If Not ScanOnly Then
            Group = ListViewFilesAddGroup(Directory, Path, GroupIndex)
            GroupIndex += 1
        End If

        Dim DirectoryEntryCount = Directory.Data.EntryCount

        If DirectoryEntryCount > 0 Then
            Dim HasLFN As Boolean = False
            Dim LFNFileName As String = ""
            Dim LFNIndex As Byte = 0
            Dim LFNChecksum As Byte = 0

            Dim EntryCount = 0
            For Counter = 0 To DirectoryEntryCount - 1
                Dim DirectoryEntry = Directory.GetFile(Counter)

                If Not DirectoryEntry.IsLink Then
                    If DirectoryEntry.IsLFN Then
                        If LFNIndex > 0 Then
                            LFNIndex -= 1
                        End If
                        LFNIndex = DirectoryEntry.LFNGetNextSequence(LFNIndex, LFNChecksum, True)
                        If LFNIndex > 0 Then
                            LFNChecksum = DirectoryEntry.LFNChecksum
                            If (LFNIndex And &H40) > 0 Then
                                LFNIndex = LFNIndex And Not &H40
                                LFNFileName = DirectoryEntry.GetLFNFileName
                            Else
                                LFNFileName = DirectoryEntry.GetLFNFileName & LFNFileName
                            End If
                            HasLFN = True
                        Else
                            LFNFileName = ""
                            HasLFN = False
                        End If
                    Else
                        If HasLFN Then
                            If LFNIndex <> 1 Or LFNChecksum <> DirectoryEntry.CalculateLFNChecksum Then
                                LFNFileName = ""
                                HasLFN = False
                            End If
                        End If
                        If Not HasLFN Then
                            If Not DirectoryEntry.HasNTUnknownFlags And (DirectoryEntry.HasNTLowerCaseFileName Or DirectoryEntry.HasNTLowerCaseExtension) Then
                                LFNFileName = DirectoryEntry.GetNTFileName
                            End If
                        End If
                        Dim ProcessResponse = Response.ProcessDirectoryEntry(DirectoryEntry, LFNFileName, Path = "")

                        If Not ScanOnly Then
                            Dim FileData As New FileData With {
                                .Index = Counter,
                                .FilePath = Path,
                                .DirectoryEntry = DirectoryEntry,
                                .IsLastEntry = (Counter = DirectoryEntryCount - 1),
                                .LFNFileName = LFNFileName,
                                .DuplicateFileName = ProcessResponse.DuplicateFileName,
                                .InvalidVolumeName = ProcessResponse.InvalidVolumeName
                            }
                            Dim Item = ListViewFilesAddItem(FileData, Group, ItemIndex)
                            ItemIndex += 1
                        End If

                        If DirectoryEntry.IsDirectory And DirectoryEntry.SubDirectory IsNot Nothing Then
                            If DirectoryEntry.SubDirectory.Data.EntryCount > 0 Then
                                Dim NewPath As String
                                If HasLFN Then
                                    NewPath = LFNFileName
                                Else
                                    NewPath = DirectoryEntry.GetShortFileName
                                End If
                                If Path <> "" Then
                                    NewPath = Path & "\" & NewPath
                                End If

                                If Not ScanOnly Then
                                    MenuDisplayDirectorySubMenuItemAdd(NewPath, DirectoryEntry.SubDirectory, -1)
                                End If
                                Dim SubResponse = ProcessDirectoryEntries(DirectoryEntry.SubDirectory, DirectoryEntry.Offset, NewPath, ScanOnly, GroupIndex, ItemIndex)
                                Response.Combine(SubResponse)
                            End If
                        End If

                        LFNFileName = ""
                        LFNIndex = 0
                        LFNChecksum = 0
                        HasLFN = False
                    End If
                    EntryCount += 1
                End If
            Next

            If Not ScanOnly And EntryCount = 0 Then
                Dim Item = ListViewFilesAddEmpty(Group, ItemIndex)
                ItemIndex += 1
            End If

        ElseIf Not ScanOnly Then
            Dim Item = ListViewFilesAddEmpty(Group, ItemIndex)
            ItemIndex += 1
        End If

        Return Response
    End Function

    Private Sub ProcessDirectoryScanResponse(CurrentImage As CurrentImage, Response As DirectoryScanResponse)
        If Response.HasCreated Then
            FileCreationDate.Width = 140
        Else
            FileCreationDate.Width = 0
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

        If Response.HasNTReserved Then
            FileNTReserved.Width = 30
        Else
            FileNTReserved.Width = 0
        End If

        If Response.HasFAT32Cluster Then
            FileFAT32Cluster.Width = 50
        Else
            FileFAT32Cluster.Width = 0
        End If

        If Response.HasFATChainingErrors Then
            FileClusterError.Width = 30
            RefreshClusterErrors()
        Else
            FileClusterError.Width = 0
        End If

        MenuToolsWin9xClean.Enabled = Response.HasValidCreated Or Response.HasValidLastAccessed Or CurrentImage.Disk.BootSector.IsWin9xOEMName
        MenuToolsClearReservedBytes.Enabled = Response.HasNTUnknownFlags Or Response.HasFAT32Cluster
    End Sub

    Private Sub ProcessFileDrop(File As String)
        Dim Files(0) As String
        Files(0) = File

        ProcessFileDrop(Files, False)
    End Sub

    Private Sub ProcessFileDrop(Files() As String, ShowDialog As Boolean)
        Cursor.Current = Cursors.WaitCursor
        Dim T = Stopwatch.StartNew

        If ImageFilters.FiltersApplied Then
            FiltersClear(False)
            ImageFilters.UpdateAllMenuItems()
            ImageCountUpdate()
        End If

        ComboImages.BeginUpdate()

        ImageData.StringOffset = 0

        Dim ImageLoadForm As New ImageLoadForm(Me, Files, _LoadedFiles)
        If ShowDialog Then
            ImageLoadForm.ShowDialog(Me)
        Else
            ImageLoadForm.ProcessScan(Nothing)
        End If

        ImageData.StringOffset = GetPathOffset()

        If ImageLoadForm.SelectedImageData IsNot Nothing Then
            LabelDropMessage.Visible = False

            RefreshSubFilterEnabled(ComboImages)
            ComboImagesRefreshItemText()
            ImageCountUpdate()

            ComboImages.SelectedItem = ImageLoadForm.SelectedImageData
            If ComboImages.SelectedIndex = -1 Then
                ComboImages.SelectedIndex = 0
            End If

            SetImagesLoaded(True)
        End If

        ComboImages.EndUpdate()

        ImageLoadForm.Close()

        T.Stop()
        Debug.Print($"Image Load Time Taken: {T.Elapsed}")
        Cursor.Current = Cursors.Default
    End Sub

    Private Function ReadFileIntoBuffer(FileInfo As IO.FileInfo, FileSize As UInteger, FillChar As Byte) As Byte()
        Dim FileBuffer(FileSize - 1) As Byte
        Dim n As Integer
        Using fs = FileInfo.OpenRead()
            n = fs.Read(FileBuffer, 0, Math.Min(FileInfo.Length, FileBuffer.Length))
        End Using
        For Counter As Integer = n To FileBuffer.Length - 1
            FileBuffer(Counter) = FillChar
        Next

        Return FileBuffer
    End Function

    Private Sub RefreshCheckAll()
        Dim CheckAll = (ListViewFiles.SelectedItems.Count = ListViewFiles.Items.Count And ListViewFiles.Items.Count > 0)
        If CheckAll <> _ListViewCheckAll Then
            _ListViewCheckAll = CheckAll
            ListViewFiles.Invalidate(New Rectangle(0, 0, 20, 20), True)
        End If
    End Sub

    Private Sub RefreshClusterErrors()
        For Each Item As ListViewItem In ListViewFiles.Items
            Dim FileData As FileData = Item.Tag
            If FileData IsNot Nothing Then
                If FileData.DirectoryEntry.IsCrossLinked Then
                    Item.SubItems.Item("FileStartingCluster").ForeColor = Color.Red
                    Item.SubItems.Item("FileClusterError").Text = "CL"
                End If
            End If
        Next
    End Sub

    Private Sub RefreshCurrentState(CurrentImage As CurrentImage)
        ComboImagesRefreshCurrentItemText()
        RefreshDiskButtons(CurrentImage)
        RefreshSaveButtons(CurrentImage)
    End Sub

    Private Sub RefreshDiskButtons(CurrentImage As CurrentImage)
        If CurrentImage IsNot Nothing AndAlso CurrentImage.Disk IsNot Nothing AndAlso CurrentImage.Disk.IsValidImage Then
            Dim Compare = CurrentImage.Disk.CheckImageSize
            MenuToolsFixImageSize.Enabled = CurrentImage.Disk.Image.CanResize And Compare <> 0
            If CurrentImage.Disk.RootDirectory.Data.HasBootSector Then
                Dim BootSectorBytes = CurrentImage.Disk.Image.GetBytes(CurrentImage.Disk.RootDirectory.Data.BootSectorOffset, BootSector.BOOT_SECTOR_SIZE)
                MenuToolsRestoreBootSector.Enabled = Not BootSectorBytes.CompareTo(CurrentImage.Disk.BootSector.Data)
            Else
                MenuToolsRestoreBootSector.Enabled = False
            End If
            MenuToolsRemoveBootSector.Enabled = CurrentImage.Disk.RootDirectory.Data.HasBootSector
            MenuHexOverdumpData.Enabled = Compare > 0
            MenuHexFreeClusters.Enabled = CurrentImage.Disk.FAT.HasFreeClusters(FAT12.FreeClusterEmum.WithData)
            MenuHexBadSectors.Enabled = CurrentImage.Disk.FAT.BadClusters.Count > 0
            MenuHexLostClusters.Enabled = CurrentImage.Disk.RootDirectory.FATAllocation.LostClusters.Count > 0

            RefreshFixImageSubMenu(CurrentImage.Disk)
        Else
            MenuToolsFixImageSize.Enabled = False
            MenuToolsRestructureImage.Enabled = False
            MenuToolsFixImageSizeSubMenu.Enabled = False
            MenuToolsFixImageSizeSubMenu.Visible = False
            MenuToolsRestoreBootSector.Enabled = False
            MenuToolsRemoveBootSector.Enabled = False
            MenuHexOverdumpData.Enabled = False
            MenuHexRawTrackData.Enabled = False
            MenuHexFreeClusters.Enabled = False
            MenuHexBadSectors.Enabled = False
            MenuHexLostClusters.Enabled = False
        End If

        MenuToolsTruncateImage.Enabled = MenuToolsFixImageSize.Enabled

        If CurrentImage IsNot Nothing AndAlso CurrentImage.Disk IsNot Nothing Then
            MenuEditRevert.Enabled = CurrentImage.Disk.Image.History.Modified
            SetButtonStateUndo(CurrentImage.Disk.Image.History.UndoEnabled)
            SetButtonStateRedo(CurrentImage.Disk.Image.History.RedoEnabled)
            ToolStripStatusModified.Visible = CurrentImage.Disk.Image.History.Modified
            ToolStripStatusReadOnly.Visible = CurrentImage.ImageData.ReadOnly
            ToolStripStatusReadOnly.Text = IIf(CurrentImage.ImageData.Compressed, "Compressed", "Read Only")
            RefreshRawTrackSubMenu(CurrentImage.Disk)
            MenuToolsTrackLayout.Visible = My.Settings.Debug AndAlso CurrentImage.Disk.Image.IsBitstreamImage
        Else
            MenuEditRevert.Enabled = False
            SetButtonStateUndo(False)
            SetButtonStateRedo(False)
            ToolStripStatusModified.Visible = False
            ToolStripStatusReadOnly.Visible = False
            MenuRawTrackDataSubMenuClear()
            MenuToolsTrackLayout.Visible = False
        End If
    End Sub

    Private Sub RefreshFileButtons(CurrentImage As CurrentImage)
        Dim Stats As DirectoryStats
        Dim ParentDirectory As IDirectory = Nothing
        Dim Caption As String

        If ListViewFiles.SelectedItems.Count = 0 Then
            SetButtonStateExportFile(False)
            SetButtonStateReplaceFile(False)
            SetButtonStateFileProperties(False)
            SetButtonStateViewFileText(False, False)
            SetButtonStateHexFile(False)
            SetButtonStateViewFile(False)
            SetButtonStateRemoveFile(False, True)
            SetButtonStateDeleteFile(False, True)
            SetButtonStateUnDeleteFile(False, False)

            MenuFileViewCrosslinked.Visible = False
            MenuFileImportFilesHere.Enabled = False
            MenuFileNewDirectoryHere.Enabled = False

        ElseIf ListViewFiles.SelectedItems.Count = 1 Then
            Dim FileData As FileData = ListViewFiles.SelectedItems(0).Tag
            ParentDirectory = ListViewFiles.SelectedItems(0).Group.Tag
            If FileData IsNot Nothing Then
                Stats = DirectoryEntryGetStats(FileData.DirectoryEntry)

                SetButtonStateExportFile(Stats.CanExport And Not Stats.IsDirectory)
                SetButtonStateReplaceFile(Stats.IsValidFile And Not Stats.IsDeleted)
                SetButtonStateFileProperties(True)

                MenuFileViewCrosslinked.Visible = FileData.DirectoryEntry.IsCrossLinked
                MenuFileFixSize.Enabled = FileData.DirectoryEntry.HasIncorrectFileSize

                Caption = "View " & If(Stats.IsDeleted, "Deleted ", "") & "&File as Text"
                SetButtonStateViewFileText(Stats.FileSize > 0, Stats.IsValidFile, Caption)

                Caption = "&Remove " & If(Stats.IsDeleted, "Deleted ", "") & If(Stats.IsDirectory, "Directory", "File")
                If Stats.IsDeleted Then
                    SetButtonStateRemoveFile(True, True, Caption)
                Else
                    SetButtonStateRemoveFile(Stats.CanDelete, True, Caption)
                End If

                Caption = "&Delete " & If(Stats.IsDirectory, "Directory", "File")
                If Stats.IsDeleted Then
                    SetButtonStateDeleteFile(False, False)
                Else
                    SetButtonStateDeleteFile(Stats.CanDelete, True, Caption)
                End If

                Caption = "&Undelete " & If(Stats.IsDirectory, "Directory", "File")
                If Stats.IsDeleted Then
                    SetButtonStateUnDeleteFile(Stats.CanUndelete, True, Caption)
                Else
                    SetButtonStateUnDeleteFile(False, False)
                End If

                If Stats.IsValidFile Or Stats.IsValidDirectory Then
                    Caption = If(Stats.IsDeleted, "Deleted ", "") & If(Stats.IsDirectory, "&Directory", "&File") & ":  " & Stats.FullFileName
                    SetButtonStateHexFile(Stats.IsDirectory Or Stats.FileSize > 0, FileData.DirectoryEntry, Caption)

                    Caption = "&View " & If(Stats.IsDeleted, "Deleted ", "") & If(Stats.IsDirectory, "Directory", "File")
                    SetButtonStateViewFile(Stats.IsDirectory Or Stats.FileSize > 0, Caption)
                Else
                    SetButtonStateHexFile(False)
                    SetButtonStateViewFile(False)
                End If
                MenuFileImportFilesHere.Enabled = Stats.CanInsert
                MenuFileNewDirectoryHere.Enabled = Stats.CanInsert
            Else
                MenuFileImportFilesHere.Enabled = False
                MenuFileNewDirectoryHere.Enabled = False
            End If
        Else
            Dim FileData As FileData
            Dim ExportEnabled As Boolean = False
            Dim DeleteEnabled As Boolean = False
            Dim RemoveEnabled As Boolean = False
            Dim MutlipleParents As Boolean = False

            ParentDirectory = Nothing

            For Each Item As ListViewItem In ListViewFiles.SelectedItems
                FileData = Item.Tag
                If FileData IsNot Nothing Then
                    Stats = DirectoryEntryGetStats(FileData.DirectoryEntry)
                    If Stats.CanExport Then
                        ExportEnabled = True
                    End If
                    If Not Stats.IsDeleted And Stats.CanDelete Then
                        DeleteEnabled = True
                    End If
                    If Stats.IsDeleted Or Stats.CanDelete Then
                        RemoveEnabled = True
                    End If
                    If ParentDirectory Is Nothing Then
                        ParentDirectory = FileData.DirectoryEntry.ParentDirectory
                    ElseIf ParentDirectory IsNot FileData.DirectoryEntry.ParentDirectory Then
                        MutlipleParents = True
                    End If
                End If
            Next

            If MutlipleParents Then
                ParentDirectory = Nothing
            End If

            SetButtonStateExportFile(ExportEnabled, "&Export Selected Files")
            SetButtonStateReplaceFile(False)
            SetButtonStateFileProperties(True)
            SetButtonStateViewFileText(False, True)
            SetButtonStateHexFile(False)
            SetButtonStateViewFile(False)
            SetButtonStateRemoveFile(RemoveEnabled, True, "&Remove Selected Files")
            SetButtonStateDeleteFile(DeleteEnabled, True, "&Delete Selected Files")
            SetButtonStateUnDeleteFile(False, False)

            MenuFileViewCrosslinked.Visible = False
            MenuFileImportFilesHere.Enabled = False
            MenuFileNewDirectoryHere.Enabled = False
        End If

        If ParentDirectory Is Nothing Then
            SetButtonStateViewDirectory(False, False)
            SetButtonStateAddFile(False)
        Else
            If ParentDirectory Is CurrentImage.Disk.RootDirectory Then
                Caption = "View Root D&irectory"
            Else
                Caption = "View Parent D&irectory"
            End If
            SetButtonStateViewDirectory(True, True, ParentDirectory, Caption)
            SetButtonStateAddFile(True, ParentDirectory)
        End If
    End Sub

    Private Sub RefreshFilterButtons(Enabled As Boolean)
        MenuFiltersClear.Enabled = Enabled
        If Enabled Then
            MainMenuFilters.BackColor = Color.LightGreen
        Else
            MainMenuFilters.BackColor = SystemColors.Control
        End If
    End Sub

    Private Sub RefreshFixImageSubMenu(Disk As Disk)
        Dim EnableSubMenu As Boolean
        Dim EnableRestructureImage As Boolean = False

        If Disk.Image.CanResize Then
            Dim DiskFormatBySize = GetFloppyDiskFormat(Disk.Image.Length)

            If Disk.DiskFormat = FloppyDiskFormat.Floppy160 And DiskFormatBySize = FloppyDiskFormat.Floppy180 Then
                EnableRestructureImage = True
            ElseIf Disk.DiskFormat = FloppyDiskFormat.Floppy160 And DiskFormatBySize = FloppyDiskFormat.Floppy320 Then
                EnableRestructureImage = True
            ElseIf Disk.DiskFormat = FloppyDiskFormat.Floppy160 And DiskFormatBySize = FloppyDiskFormat.Floppy360 Then
                EnableRestructureImage = True
            ElseIf Disk.DiskFormat = FloppyDiskFormat.Floppy180 And DiskFormatBySize = FloppyDiskFormat.Floppy360 Then
                EnableRestructureImage = True
            ElseIf Disk.DiskFormat = FloppyDiskFormat.Floppy320 And DiskFormatBySize = FloppyDiskFormat.Floppy360 Then
                EnableRestructureImage = True
            End If
        End If

        EnableSubMenu = EnableRestructureImage

        If EnableSubMenu Then
            MenuToolsFixImageSize.Visible = False
            MenuToolsFixImageSizeSubMenu.Visible = True
            MenuToolsFixImageSizeSubMenu.Enabled = True
            MenuToolsRestructureImage.Enabled = EnableRestructureImage
            MenuToolsRestructureImage.Visible = EnableRestructureImage
        Else
            MenuToolsFixImageSize.Visible = True
            MenuToolsFixImageSizeSubMenu.Visible = False
            MenuToolsFixImageSizeSubMenu.Enabled = False
            MenuToolsRestructureImage.Enabled = False
        End If
    End Sub

    Private Sub RefreshModifiedCount()
        Dim Count As Integer = ImageFilters.FilterCounts(Filters.FilterTypes.ModifiedFiles).Total

        ToolStripModified.Text = $"{Count} {"Image".Pluralize(Count)} Modified"
        ToolStripModified.Visible = (Count > 0)
        SetButtonStateSaveAll(Count > 0)
    End Sub

    Private Sub RefreshRawTrackSubMenu(Disk As Disk)
        MenuRawTrackDataSubMenuClear()
        MenuHexRawTrackData.Enabled = False

        If Disk.Image.IsBitstreamImage Then
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

                    MenuRawTrackDataSubMenuItemAdd(-1, "All Tracks")

                    For i = 0 To TrackList.Length - 1
                        Dim Track = TrackList(i)
                        Dim TrackString = "Track " & (Track \ Disk.Image.SideCount) & "." & (Track Mod Disk.Image.SideCount)
                        MenuRawTrackDataSubMenuItemAdd(Track, TrackString)
                    Next

                    MenuHexRawTrackData.Enabled = True
                    MenuHexRawTrackData.Tag = Nothing
                Else
                    MenuHexRawTrackData.Enabled = True
                    MenuHexRawTrackData.Tag = -1
                End If
            Else
                MenuHexRawTrackData.Enabled = True
                MenuHexRawTrackData.Tag = -1
            End If
        End If
    End Sub

    Private Sub RefreshSaveButtons(CurrentImage As CurrentImage)
        If CurrentImage Is Nothing Then
            SetButtonStateSaveFile(False)
            BtnExportDebug.Enabled = False
            MenuFileReload.Enabled = False
        Else
            Dim Modified = CurrentImage.ImageData.Filter(Filters.FilterTypes.ModifiedFiles)
            SetButtonStateSaveFile(Modified And Not CurrentImage.ImageData.ReadOnly)
            MenuFileReload.Enabled = True
            'BtnExportDebug.Enabled = (CurrentImageData.Modified Or CurrentImageData.SessionModifications.Count > 0)
            BtnExportDebug.Enabled = False
        End If
    End Sub

    Private Sub RefreshSubFilterEnabled(SubFilter As ComboBox)
        Dim Enabled As Boolean = SubFilter.Items.Count > 0
        SubFilter.Enabled = Enabled
        If Enabled Then
            SubFilter.DrawMode = DrawMode.OwnerDrawFixed
        Else
            SubFilter.DrawMode = DrawMode.Normal
        End If
    End Sub

    Private Sub ReloadCurrentImage(RevertChanges As Boolean)
        _CurrentImage.ImageData.BottomIndex = ListViewFiles.GetBottomIndex
        _CurrentImage.ImageData.SortHistory = _lvwColumnSorter.SortHistory

        If RevertChanges Then
            _CurrentImage.ImageData.Modifications.Clear()
        End If

        LoadCurrentImage(_CurrentImage.ImageData, RevertChanges)
    End Sub
    Private Sub RemoveDeletedFile(CurrentImage As CurrentImage, DirectoryEntry As DirectoryEntry)
        Dim ParentDirectory = DirectoryEntry.ParentDirectory

        Dim Result = ParentDirectory.RemoveEntry(DirectoryEntry.Index)

        If Result Then
            DiskImageRefresh(CurrentImage)
        End If
    End Sub

    Private Sub ResetAll()
        'EmptyTempPath()
        _CurrentImage = Nothing
        Me.Text = GetWindowCaption()
        ImageFilters.FiltersApplied = False
        _ListViewCheckAll = False
        _LoadedFiles.FileNames.Clear()
        _ScanRun = False

        MenuOptionsCreateBackup.Checked = My.Settings.CreateBackups
        MenuOptionsCheckUpdate.Checked = My.Settings.CheckUpdateOnStartup
        MenuOptionsDragDrop.Checked = My.Settings.DragAndDrop

        RefreshDiskButtons(Nothing)

        ToolStripFileName.Visible = False
        ToolStripModified.Visible = False
        ToolStripFileCount.Visible = False
        ToolStripFileSector.Visible = False
        ToolStripFileTrack.Visible = False

        SetButtonStateSaveAll(False)
        btnRetry.Visible = False

        ListViewSummary.Items.Clear()
        ListViewHashes.Items.Clear()

        ComboImagesReset()
        ListViewFilesReset()

        RefreshFileButtons(Nothing)
        SetImagesLoaded(False)
        FiltersReset()
        InitButtonState(Nothing)
    End Sub
    Private Sub RevertChanges(CurrentImage As CurrentImage)
        If CurrentImage.Disk.Image.History.Modified Then
            ReloadCurrentImage(True)
        End If
    End Sub

    Private Sub SaveAll(CurrentImage As CurrentImage)
        Dim RefreshCurrent As Boolean = False

        _SuppressEvent = True
        For Index = 0 To ComboImages.Items.Count - 1
            Dim NewFilePath As String = ""
            Dim DoSave As Boolean = True
            Dim ImageData As ImageData = ComboImages.Items(Index)
            If ImageData.Filter(Filters.FilterTypes.ModifiedFiles) Then
                If ImageData.ReadOnly Then
                    If MsgBoxNewFileName(ImageData.FileName) = MsgBoxResult.Ok Then
                        NewFilePath = GetNewFilePath(CurrentImage, ImageData)
                        DoSave = (NewFilePath <> "")
                    Else
                        DoSave = False
                    End If
                End If
                If DoSave Then
                    Dim Result = DiskImageSave(CurrentImage, ImageData, NewFilePath)
                    If Result Then
                        If ImageData.ReadOnly Then
                            SetNewFilePath(ImageData, NewFilePath)
                        End If
                        If ImageData Is ComboImages.SelectedItem Then
                            SetCurrentFileName(ImageData)
                            ListViewFilesClearModifiedFlag()
                            RefreshCurrent = True
                        End If
                    End If
                End If
            End If
        Next Index
        _SuppressEvent = False

        ImageFilters.UpdateMenuItem(Filters.FilterTypes.ModifiedFiles)
        RefreshModifiedCount()

        If RefreshCurrent Then
            RefreshCurrentState(CurrentImage)
            ReloadCurrentImage(False)
        End If
    End Sub

    Private Sub SaveCurrent(CurrentImage As CurrentImage, NewFileName As Boolean)
        Dim NewFilePath As String = ""

        If NewFileName Then
            NewFilePath = GetNewFilePath(CurrentImage)
            If NewFilePath = "" Then
                Exit Sub
            End If
        End If


        Dim Result = DiskImageSave(CurrentImage, NewFilePath)
        If Result Then
            ImageFilters.UpdateMenuItem(Filters.FilterTypes.ModifiedFiles)
            RefreshModifiedCount()

            If NewFileName Then
                SetNewFilePath(CurrentImage.ImageData, NewFilePath)
                SetCurrentFileName(CurrentImage.ImageData)
            End If

            ListViewFilesClearModifiedFlag()
            RefreshCurrentState(CurrentImage)
            ReloadCurrentImage(False)
        End If
    End Sub

    Private Sub SetButtonStateAddFile(Enabled As Boolean, Optional Tag As Object = Nothing)
        MenuFileImportFiles.Enabled = Enabled
        MenuFileImportFiles.Tag = Tag

        MenuFileNewDirectory.Enabled = Enabled
        MenuFileNewDirectory.Tag = Tag
    End Sub

    Private Sub SetButtonStateDeleteFile(Enabled As Boolean, Visible As Boolean, Optional Text As String = "&Delete File")
        MenuFileDeleteFile.Visible = Visible
        MenuFileDeleteFile.Enabled = Enabled
        MenuFileDeleteFile.Text = Text
    End Sub

    Private Sub SetButtonStateExportFile(Enabled As Boolean, Optional Text As String = "&Export File")
        MenuEditExportFile.Enabled = Enabled
        MenuEditExportFile.Text = Text

        MenuFileExportFile.Enabled = Enabled
        MenuFileExportFile.Text = Text

        ToolStripExportFile.Enabled = Enabled
        ToolStripExportFile.Text = Text
    End Sub

    Private Sub SetButtonStateFileProperties(Enabled As Boolean)
        MenuEditFileProperties.Enabled = Enabled
        MenuFileFileProperties.Enabled = Enabled
        ToolStripFileProperties.Enabled = Enabled
    End Sub

    Private Sub SetButtonStateHexFile(Visible As Boolean, Optional Tag As Object = Nothing, Optional Text As String = "&File")
        MenuHexFile.Visible = Visible
        MenuHexFile.Text = Text
        MenuHexFile.Tag = Tag

        MenuHexSeparatorFile.Visible = Visible
    End Sub

    Private Sub SetButtonStateRedo(Enabled As Boolean)
        MenuEditRedo.Enabled = Enabled
        ToolStripRedo.Enabled = Enabled
    End Sub

    Private Sub SetButtonStateRemoveFile(Enabled As Boolean, Visible As Boolean, Optional Text As String = "&Remove File")
        MenuFileRemove.Enabled = Enabled
        MenuFileRemove.Visible = Visible
        MenuFileRemove.Text = Text
    End Sub

    Private Sub SetButtonStateReplaceFile(Enabled As Boolean)
        MenuFileReplaceFile.Enabled = Enabled
        MenuEditReplaceFile.Enabled = Enabled
    End Sub

    Private Sub SetButtonStateSaveAll(Enabled As Boolean)
        MenuFileSaveAll.Enabled = Enabled
        ToolStripSaveAll.Enabled = Enabled
    End Sub

    Private Sub SetButtonStateSaveFile(Enabled As Boolean)
        MenuFileSave.Enabled = Enabled
        ToolStripSave.Enabled = Enabled
    End Sub

    Private Sub SetButtonStateUnDeleteFile(Enabled As Boolean, Visible As Boolean, Optional Text As String = "&Undelete File")
        MenuFileUnDeleteFile.Visible = Visible
        MenuFileUnDeleteFile.Enabled = Enabled
        MenuFileUnDeleteFile.Text = Text
    End Sub

    Private Sub SetButtonStateUndo(Enabled As Boolean)
        MenuEditUndo.Enabled = Enabled
        ToolStripUndo.Enabled = Enabled
    End Sub

    Private Sub SetButtonStateViewDirectory(Enabled As Boolean, Visible As Boolean, Optional Tag As Object = Nothing, Optional Text As String = "View D&irectory")
        MenuFileViewDirectory.Enabled = Enabled
        MenuFileViewDirectory.Visible = Visible
        MenuFileViewDirectory.Tag = Tag
        MenuFileViewDirectory.Text = Text

        FileMenuSeparatorDirectory.Visible = Visible
    End Sub

    Private Sub SetButtonStateViewFile(Enabled As Boolean, Optional Text As String = "&View File")
        MenuFileViewFile.Enabled = Enabled
        MenuFileViewFile.Text = Text

        ToolStripViewFile.Enabled = Enabled
        ToolStripViewFile.Text = Text
    End Sub

    Private Sub SetButtonStateViewFileText(Enabled As Boolean, Visible As Boolean, Optional Text As String = "View File as &Text")
        MenuFileViewFileText.Visible = Visible
        MenuFileViewFileText.Enabled = Enabled
        MenuFileViewFileText.Text = Text

        ToolStripViewFileText.Enabled = Enabled
        ToolStripViewFileText.Text = Text
    End Sub
    Private Sub SetCurrentFileName(ImageData As ImageData)
        Dim FileName = ImageData.FileName

        Me.Text = $"{GetWindowCaption()} - {FileName}"

        ToolStripFileName.Text = FileName
        ToolStripFileName.Visible = True
    End Sub

    Private Sub SetImagesLoaded(Value As Boolean)
        ToolStripImageCount.Visible = Value
        LabelDropMessage.Visible = Not Value
        MenuFiltersScan.Enabled = Value
        MenuFiltersScanNew.Enabled = Value
        MenuFiltersScanNew.Visible = _ScanRun
        MenuFileClose.Enabled = Value
        ToolStripClose.Enabled = MenuFileClose.Enabled
        MenuFileCloseAll.Enabled = Value
        ToolStripCloseAll.Enabled = MenuFileCloseAll.Enabled
        ToolStripSearchText.Enabled = Value
        MenuToolsWin9xCleanBatch.Enabled = Value
        If Value Then
            MenuToolsCompare.Enabled = ComboImages.Items.Count > 1
        Else
            MenuToolsCompare.Enabled = False
        End If
    End Sub

    Private Sub SetNewFilePath(ImageData As ImageData, NewFilePath As String)
        If ImageData.SourceFile <> NewFilePath Then
            _LoadedFiles.FileNames.Remove(ImageData.DisplayPath)

            ImageData.SourceFile = NewFilePath
            ImageData.Compressed = False
            ImageData.CompressedFile = ""
            ImageData.ReadOnly = IsFileReadOnly(NewFilePath)

            If _LoadedFiles.FileNames.ContainsKey(ImageData.DisplayPath) Then
                FileClose(_LoadedFiles.FileNames.Item(ImageData.DisplayPath))
            End If

            _LoadedFiles.FileNames.Add(ImageData.DisplayPath, ImageData)

            ComboImagesRefreshPaths()
        End If
    End Sub

    Private Sub SubFilterDiskTypePopulateUnfiltered()
        _SubFilterDiskType.Clear()
        For Each ImageData As ImageData In ComboImages.Items
            _SubFilterDiskType.Add(ImageData.DiskType, False)
        Next
        _SubFilterDiskType.Populate()
    End Sub

    Private Sub SubFilterOEMNameAdd(OEMName As String, UpdateFilters As Boolean)
        If OEMName.EndsWith("IHC") Then
            Return
        End If

        _SubFilterOEMName.Add(OEMName, UpdateFilters)
    End Sub

    Private Sub SubFilterOEMNamePopulateUnfiltered()
        _SubFilterOEMName.Clear()
        For Each ImageData As ImageData In ComboImages.Items
            SubFilterOEMNameAdd(ImageData.OEMName, False)
        Next
        _SubFilterOEMName.Populate()
    End Sub

    Private Sub SubFiltersClear()
        _SuppressEvent = True

        _SubFilterOEMName.Clear()
        _SubFilterDiskType.Clear()

        _SuppressEvent = False
    End Sub

    Private Sub SubFiltersClearFilter()
        _SuppressEvent = True

        ToolStripSearchText.Text = ""
        _SubFilterOEMName.ClearFilter()
        _SubFilterDiskType.ClearFilter()

        _SuppressEvent = False
    End Sub

    Private Sub SubFiltersInitialize()
        _SubFilterOEMName = New ComboFilter(ToolStripOEMNameCombo)
        _SubFilterDiskType = New ComboFilter(ToolStripDiskTypeCombo)
    End Sub

    Private Sub SubFiltersPopulate()
        _SuppressEvent = True

        _SubFilterOEMName.Populate()
        _SubFilterDiskType.Populate()

        _SuppressEvent = False
    End Sub

    Private Sub SubFiltersPopulateUnfiltered()
        _SuppressEvent = True

        SubFilterOEMNamePopulateUnfiltered()
        SubFilterDiskTypePopulateUnfiltered()

        _SuppressEvent = False
    End Sub

    Private Sub SubFiltersReset()
        ToolStripSearchText.Text = ""
        _SubFilterOEMName.Clear()
        _SubFilterDiskType.Clear()

        ToolStripOEMNameCombo.Visible = False
        ToolStripOEMNameLabel.Visible = False

        ToolStripDiskTypeCombo.Visible = False
        ToolStripDiskTypeLabel.Visible = False
    End Sub

    Private Structure DirectoryStats
        Dim CanDelete As Boolean
        Dim CanExport As Boolean
        Dim CanInsert As Boolean
        Dim CanUndelete As Boolean
        Dim FileSize As UInteger
        Dim FullFileName As String
        Dim IsDeleted As Boolean
        Dim IsDirectory As Boolean
        Dim IsModified As Boolean
        Dim IsValidDirectory As Boolean
        Dim IsValidFile As Boolean
    End Structure

#Region "Events"
    Private Sub BtnAddDirectory_Click(sender As Object, e As EventArgs) Handles MenuFileNewDirectory.Click, MenuDirectoryNewDirectory.Click
        If sender.Tag IsNot Nothing Then
            Dim Directory As IDirectory = sender.Tag
            ImageAddDirectory(_CurrentImage, Directory)
        End If
    End Sub
    Private Sub BtnAddDirectoryHere_Click(sender As Object, e As EventArgs) Handles MenuFileNewDirectoryHere.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim FileData As FileData = ListViewFiles.SelectedItems(0).Tag
            ImageAddDirectory(_CurrentImage, FileData.DirectoryEntry.ParentDirectory, FileData.DirectoryEntry.Index)
        End If
    End Sub
    Private Sub BtnClearFilters_Click(sender As Object, e As EventArgs) Handles MenuFiltersClear.Click
        If ImageFilters.FiltersApplied Then
            FiltersClear(False)
            ImageFilters.UpdateAllMenuItems()
            ImageCountUpdate()
            ContextMenuFilters.Invalidate()
        End If
    End Sub

    Private Sub BtnClearReservedBytes_Click(sender As Object, e As EventArgs) Handles MenuToolsClearReservedBytes.Click
        ImageClearReservedBytes(_CurrentImage)
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles MenuFileClose.Click, ToolStripClose.Click
        CloseCurrent(_CurrentImage)
    End Sub

    Private Sub BtnCloseAll_Click(sender As Object, e As EventArgs) Handles MenuFileCloseAll.Click, ToolStripCloseAll.Click
        If MsgBox("Are you sure you wish to close all open files?", MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
            CloseAll(_CurrentImage)
        End If
    End Sub

    Private Sub BtnCompare_Click(sender As Object, e As EventArgs) Handles MenuToolsCompare.Click
        CompareImages()
    End Sub

    Private Sub BtnDisplayBadSectors_Click(sender As Object, e As EventArgs) Handles MenuHexBadSectors.Click
        HexDisplayBadSectors(_CurrentImage)
    End Sub

    Private Sub BtnDisplayBootSector_Click(sender As Object, e As EventArgs) Handles MenuHexBootSector.Click
        HexDisplayBootSector(_CurrentImage)
    End Sub

    Private Sub BtnDisplayClusters_Click(sender As Object, e As EventArgs) Handles MenuHexFreeClusters.Click
        HexDisplayFreeClusters(_CurrentImage)
    End Sub

    Private Sub BtnDisplayDirectory_Click(sender As Object, e As EventArgs) Handles MenuHexDirectory.Click, MenuFileViewDirectory.Click, MenuDirectoryView.Click
        If sender.Tag IsNot Nothing Then
            Dim Directory As IDirectory = sender.tag
            If Directory Is _CurrentImage.Disk.RootDirectory Then
                HexDisplayRootDirectory(_CurrentImage)
            Else
                HexDisplayDirectoryEntry(_CurrentImage, Directory.ParentEntry)
            End If
        End If
    End Sub

    Private Sub BtnDisplayDisk_Click(sender As Object, e As EventArgs) Handles MenuHexDisk.Click
        HexDisplayDiskImage(_CurrentImage)
    End Sub

    Private Sub BtnDisplayFAT_Click(sender As Object, e As EventArgs) Handles MenuHexFAT.Click
        HexDisplayFAT(_CurrentImage)
    End Sub

    Private Sub BtnDisplayFile_Click(sender As Object, e As EventArgs) Handles MenuHexFile.Click
        If sender.tag IsNot Nothing Then
            HexDisplayDirectoryEntry(_CurrentImage, sender.tag)
        End If
    End Sub

    Private Sub BtnDisplayLostClusters_Click(sender As Object, e As EventArgs) Handles MenuHexLostClusters.Click
        HexDisplayLostClusters(_CurrentImage)
    End Sub

    Private Sub BtnDisplayOverdumpData_Click(sender As Object, e As EventArgs) Handles MenuHexOverdumpData.Click
        HexDisplayOverdumpData(_CurrentImage)
    End Sub

    Private Sub BtnEditBootSector_Click(sender As Object, e As EventArgs) Handles MenuEditBootSector.Click
        BootSectorEdit(_CurrentImage)
    End Sub

    Private Sub BtnEditFAT_Click(sender As Object, e As EventArgs) Handles MenuEditFAT.Click
        If sender.tag IsNot Nothing Then
            If sender.tag = -1 Then
                FATEdit(_CurrentImage, 0)
            Else
                FATEdit(_CurrentImage, sender.tag)
            End If
        End If
    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles MenuFileExit.Click
        If CloseAll(_CurrentImage) Then
            Me.Close()
        End If
    End Sub

    Private Sub BtnExportDebug_Click(sender As Object, e As EventArgs) Handles BtnExportDebug.Click
        ExportDebugScript(_CurrentImage)
    End Sub

    Private Sub BtnExportFile_Click(sender As Object, e As EventArgs) Handles MenuEditExportFile.Click, MenuFileExportFile.Click, ToolStripExportFile.Click
        ImageFileExport()
    End Sub

    Private Sub BtnFileMenuDeleteFile_Click(sender As Object, e As EventArgs) Handles MenuFileDeleteFile.Click
        ImageDeleteSelectedFiles(_CurrentImage, False)
    End Sub

    Private Sub BtnFileMenuFixSize_Click(sender As Object, e As EventArgs) Handles MenuFileFixSize.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim FileData As FileData = ListViewFiles.SelectedItems(0).Tag
            ImageFixFileSize(_CurrentImage, FileData.DirectoryEntry)
        End If
    End Sub

    Private Sub BtnFileMenuRemove_Click(sender As Object, e As EventArgs) Handles MenuFileRemove.Click
        ImageDeleteSelectedFiles(_CurrentImage, True)
    End Sub

    Private Sub BtnFileMenuUnDeleteFile_Click(sender As Object, e As EventArgs) Handles MenuFileUnDeleteFile.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim FileData As FileData = ListViewFiles.SelectedItems(0).Tag
            ImageUndeleteFile(_CurrentImage, FileData.DirectoryEntry)
        End If
    End Sub

    Private Sub BtnFileMenuViewCrosslinked_Click(sender As Object, e As EventArgs) Handles MenuFileViewCrosslinked.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim FileData As FileData = ListViewFiles.SelectedItems(0).Tag
            DisplayCrossLinkedFiles(_CurrentImage.Disk, FileData.DirectoryEntry)
        End If
    End Sub

    Private Sub BtnFileMenuViewFile_Click(sender As Object, e As EventArgs) Handles MenuFileViewFile.Click, ToolStripViewFile.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim FileData As FileData = ListViewFiles.SelectedItems(0).Tag
            HexDisplayDirectoryEntry(_CurrentImage, FileData.DirectoryEntry)
        End If
    End Sub

    Private Sub BtnFileMenuViewFileText_Click(sender As Object, e As EventArgs) Handles MenuFileViewFileText.Click, ToolStripViewFileText.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim FileData As FileData = ListViewFiles.SelectedItems(0).Tag
            DirectoryEntryDisplayText(FileData.DirectoryEntry)
        End If
    End Sub

    Private Sub BtnFileProperties_Click(sender As Object, e As EventArgs) Handles MenuEditFileProperties.Click, MenuFileFileProperties.Click, ToolStripFileProperties.Click
        FilePropertiesEdit(_CurrentImage)
    End Sub

    Private Sub BtnFixImageSize_Click(sender As Object, e As EventArgs) Handles MenuToolsFixImageSize.Click, MenuToolsTruncateImage.Click
        ImageFixImageSize(_CurrentImage)
    End Sub

    Private Sub BtnHelpAbout_Click(sender As Object, e As EventArgs) Handles MenuHelpAbout.Click
        Dim AboutBox As New AboutBox()
        AboutBox.ShowDialog()
    End Sub

    Private Sub BtnHelpChangeLog_Click(sender As Object, e As EventArgs) Handles MenuHelpChangeLog.Click
        DisplayChangeLog()
    End Sub

    Private Sub BtnHelpProjectPage_Click(sender As Object, e As EventArgs) Handles MenuHelpProjectPage.Click
        Process.Start(SITE_URL)
    End Sub

    Private Sub BtnHelpUpdateCheck_Click(sender As Object, e As EventArgs) Handles MenuHelpUpdateCheck.Click, MainMenuUpdateAvailable.Click
        CheckForUpdates()
    End Sub

    Private Sub BtnImportFiles_Click(sender As Object, e As EventArgs) Handles MenuFileImportFiles.Click, MenuDirectoryImportFiles.Click
        If sender.Tag IsNot Nothing Then
            Dim Directory As IDirectory = sender.Tag
            ImageImport(_CurrentImage, Directory, True)
        End If
    End Sub
    Private Sub BtnImportFilesHere_Click(sender As Object, e As EventArgs) Handles MenuFileImportFilesHere.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim FileData As FileData = ListViewFiles.SelectedItems(0).Tag
            ImageImport(_CurrentImage, FileData.DirectoryEntry.ParentDirectory, True, FileData.DirectoryEntry.Index)
        End If
    End Sub

    Private Sub BtnkWriteFloppyA_Click(sender As Object, e As EventArgs) Handles MenuDiskWriteFloppyA.Click
        FloppyDiskWrite(Me, _CurrentImage.Disk, FloppyDriveEnum.FloppyDriveA)
    End Sub

    Private Sub BtnNewImage_Click(sender As Object, e As EventArgs) Handles MenuFileNewImage.Click
        ImageNew()
    End Sub

    Private Sub BtnOpen_Click(sender As Object, e As EventArgs) Handles MenuFileOpen.Click, ToolStripOpen.Click
        FilesOpen()
    End Sub

    Private Sub BtnRawTrackData_Click(sender As Object, e As EventArgs) Handles MenuHexRawTrackData.Click
        If sender.tag IsNot Nothing Then
            HexDisplayRawTrackData(_CurrentImage.Disk, sender.tag)
        End If
    End Sub

    Private Sub BtnReadFloppyA_Click(sender As Object, e As EventArgs) Handles MenuDiskReadFloppyA.Click
        Dim FileName = FloppyDiskRead(Me, FloppyDriveEnum.FloppyDriveA, _LoadedFiles.FileNames)
        If FileName.Length > 0 Then
            ProcessFileDrop(FileName)
        End If
    End Sub

    Private Sub BtnReadFloppyB_Click(sender As Object, e As EventArgs) Handles MenuDiskReadFloppyB.Click
        Dim FileName = FloppyDiskRead(Me, FloppyDriveEnum.FloppyDriveB, _LoadedFiles.FileNames)
        If FileName.Length > 0 Then
            ProcessFileDrop(FileName)
        End If
    End Sub

    Private Sub BtnRedo_Click(sender As Object, e As EventArgs) Handles MenuEditRedo.Click, ToolStripRedo.Click
        _CurrentImage.Disk.Image.History.Redo()
        DiskImageRefresh(_CurrentImage)
    End Sub

    Private Sub BtnReload_Click(sender As Object, e As EventArgs) Handles MenuFileReload.Click
        ReloadCurrentImage(False)
    End Sub

    Private Sub BtnRemoveBootSector_Click(sender As Object, e As EventArgs) Handles MenuToolsRemoveBootSector.Click
        BootSectorRemoveFromDirectory(_CurrentImage.Disk)
        DiskImageRefresh(_CurrentImage)
    End Sub

    Private Sub BtnReplaceFile_Click(sender As Object, e As EventArgs) Handles MenuFileReplaceFile.Click, MenuEditReplaceFile.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim FileData As FileData = ListViewFiles.SelectedItems(0).Tag
            ImageReplaceFile(_CurrentImage, FileData.DirectoryEntry)
        End If
    End Sub

    Private Sub BtnResetSort_Click(sender As Object, e As EventArgs) Handles BtnResetSort.Click
        ClearSort(True)
    End Sub

    Private Sub BtnRestoreBootSector_Click(sender As Object, e As EventArgs) Handles MenuToolsRestoreBootSector.Click
        BootSectorRestoreFromDirectory(_CurrentImage.Disk)
        DiskImageRefresh(_CurrentImage)
    End Sub

    Private Sub BtnRestructureImage_Click(sender As Object, e As EventArgs) Handles MenuToolsRestructureImage.Click
        ImageRestructure(_CurrentImage)
    End Sub
    Private Sub BtnRetry_Click(sender As Object, e As EventArgs) Handles btnRetry.Click
        ReloadCurrentImage(False)
    End Sub

    Private Sub BtnRevert_Click(sender As Object, e As EventArgs) Handles MenuEditRevert.Click
        RevertChanges(_CurrentImage)
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles MenuFileSave.Click, ToolStripSave.Click
        SaveCurrent(_CurrentImage, False)
    End Sub

    Private Sub BtnSaveAll_Click(sender As Object, e As EventArgs) Handles MenuFileSaveAll.Click, ToolStripSaveAll.Click
        If MsgBox("Are you sure you wish to save all modified files?", MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
            SaveAll(_CurrentImage)
        End If
    End Sub

    Private Sub BtnSaveAs_Click(sender As Object, e As EventArgs) Handles MenuFileSaveAs.Click, ToolStripSaveAs.Click
        SaveCurrent(_CurrentImage, True)
    End Sub

    Private Sub BtnScan_Click(sender As Object, e As EventArgs) Handles MenuFiltersScan.Click
        ContextMenuFilters.Close()
        DiskImagesScan(_CurrentImage, False)
    End Sub

    Private Sub BtnScanNew_Click(sender As Object, e As EventArgs) Handles MenuFiltersScanNew.Click
        ContextMenuFilters.Close()
        DiskImagesScan(_CurrentImage, True)
    End Sub

    Private Sub BtnUndo_Click(sender As Object, e As EventArgs) Handles MenuEditUndo.Click, ToolStripUndo.Click
        _CurrentImage.Disk.Image.History.Undo()
        DiskImageRefresh(_CurrentImage)
    End Sub

    Private Sub BtnWin9xClean_Click(sender As Object, e As EventArgs) Handles MenuToolsWin9xClean.Click
        ImageWin9xCleanCurrent(_CurrentImage)
    End Sub

    Private Sub BtnWin9xCleanBatch_Click(sender As Object, e As EventArgs) Handles MenuToolsWin9xCleanBatch.Click
        ImageWin9xCleanBatch(_CurrentImage)
    End Sub

    Private Sub BtnWriteFloppyB_Click(sender As Object, e As EventArgs) Handles MenuDiskWriteFloppyB.Click
        FloppyDiskWrite(Me, _CurrentImage.Disk, FloppyDriveEnum.FloppyDriveB)
    End Sub

    Private Sub ComboImages_DrawItem(sender As Object, e As DrawItemEventArgs) Handles ComboImages.DrawItem, ComboImagesFiltered.DrawItem
        If e.Index >= -1 Then
            e.DrawBackground()

            If e.Index > -1 Then
                Dim CB As ComboBox = sender
                Dim tBrush As Brush

                If e.State And DrawItemState.Selected Then
                    tBrush = SystemBrushes.HighlightText
                Else
                    Dim ImageData As ImageData = CB.Items(e.Index)
                    If ImageData IsNot Nothing AndAlso ImageData.Filter(Filters.FilterTypes.ModifiedFiles) Then
                        tBrush = Brushes.Blue
                    Else
                        tBrush = SystemBrushes.WindowText
                    End If
                End If

                Dim Format As New StringFormat With {
                    .Trimming = StringTrimming.None,
                    .FormatFlags = StringFormatFlags.NoWrap
                }
                e.Graphics.DrawString(CB.Items(e.Index).ToString, e.Font, tBrush, e.Bounds, Format)
            End If
        End If

        e.DrawFocusRectangle()
    End Sub

    Private Sub ComboImages_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImages.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        If _CurrentImage IsNot Nothing Then
            _CurrentImage.ImageData.BottomIndex = ListViewFiles.GetBottomIndex
            _CurrentImage.ImageData.SortHistory = _lvwColumnSorter.SortHistory
        End If

        LoadCurrentImage(ComboImages.SelectedItem, False)
    End Sub

    Private Sub ComboImagesFiltered_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImagesFiltered.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        ComboImages.SelectedItem = ComboImagesFiltered.SelectedItem
    End Sub

    Private Sub ContextMenuCopy_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ContextMenuCopy1.ItemClicked, ContextMenuCopy2.ItemClicked
        Dim LV As ListView = Nothing

        If sender.name = "Summary" Then
            LV = ListViewSummary
        ElseIf sender.name = "Hashes" Then
            LV = ListViewHashes
        End If

        If LV IsNot Nothing AndAlso LV.FocusedItem IsNot Nothing Then
            Dim Item = LV.FocusedItem.SubItems.Item(1)
            Dim Value As String
            If Item.Tag Is Nothing Then
                Value = Item.Text
            Else
                Value = Item.Tag
            End If
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
            Dim Item = LV.FocusedItem

            If Item.SubItems.Count > 1 Then
                Dim Text As String
                If Item.Tag Is Nothing Then
                    Text = Item.Text
                Else
                    Text = Item.Tag
                End If
                CM.Items(0).Text = "&Copy " & Text
            Else
                e.Cancel = True
            End If
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub ContextMenuFiles_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuFiles.Opening
        If ListViewFiles.SelectedItems.Count = 0 Then
            e.Cancel = True
        End If
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

    Private Sub Debounce_Tick(sender As Object, e As EventArgs) Handles Debounce.Tick
        Debounce.Stop()

        FiltersApply(False)
    End Sub

    Private Sub File_DragDrop(sender As Object, e As DragEventArgs) Handles ComboImages.DragDrop, ComboImagesFiltered.DragDrop, LabelDropMessage.DragDrop, ListViewFiles.DragDrop, ListViewHashes.DragDrop, ListViewSummary.DragDrop
        Dim Files As String() = e.Data.GetData(DataFormats.FileDrop)
        ProcessFileDrop(Files, True)
    End Sub

    Private Sub File_DragEnter(sender As Object, e As DragEventArgs) Handles ComboImages.DragEnter, ComboImagesFiltered.DragEnter, LabelDropMessage.DragEnter, ListViewFiles.DragEnter, ListViewHashes.DragEnter, ListViewSummary.DragEnter
        FileDropStart(e)
    End Sub

    Private Sub ImageFilters_FilterChanged() Handles ImageFilters.FilterChanged
        FiltersApply(True)
    End Sub

    Private Sub ListViewFiles_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles ListViewFiles.ColumnClick
        If ListViewFiles.Items.Count = 0 Then
            Exit Sub
        End If

        If e.Column = 0 Then
            _ListViewCheckAll = Not _ListViewCheckAll
            _SuppressEvent = True
            For Each Item As ListViewItem In ListViewFiles.Items
                Item.Selected = _ListViewCheckAll
            Next
            _SuppressEvent = False
            ItemSelectionChanged(_CurrentImage)
        Else
            If e.Column = _lvwColumnSorter.SortColumn Then
                _lvwColumnSorter.SwitchOrder()
            Else
                _lvwColumnSorter.Sort(e.Column)
            End If
            ListViewFiles.Sort()
            ListViewFiles.SetSortIcon(_lvwColumnSorter.SortColumn, _lvwColumnSorter.Order)
            BtnResetSort.Enabled = True
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
            Dim State = IIf(_ListViewCheckAll, VisualStyles.CheckBoxState.CheckedNormal, VisualStyles.CheckBoxState.UncheckedNormal)
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

        ItemSelectionChanged(_CurrentImage)
    End Sub

    Private Sub ListViewFiles_MouseDown(sender As Object, e As MouseEventArgs) Handles ListViewFiles.MouseDown
        _ListViewClickedGroup = Nothing

        If e.Button And MouseButtons.Right Then
            _ListViewClickedGroup = ListViewFiles.GetGroupAtPoint(e.Location)
        End If
    End Sub

    Private Sub ListViewFiles_MouseUp(sender As Object, e As MouseEventArgs) Handles ListViewFiles.MouseUp
        If _ListViewClickedGroup IsNot Nothing Then
            DisplayDirectoryContextMenu(_ListViewClickedGroup.Tag)
            _ListViewClickedGroup = Nothing
        End If
    End Sub

    Private Sub ListViewHashes_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles ListViewHashes.ItemSelectionChanged
        e.Item.Selected = False
    End Sub

    Private Sub ListViewSummary_DrawItem(sender As Object, e As DrawListViewItemEventArgs) Handles ListViewSummary.DrawItem
        e.DrawDefault = True
    End Sub

    Private Sub ListViewSummary_DrawSubItem(sender As Object, e As DrawListViewSubItemEventArgs) Handles ListViewSummary.DrawSubItem
        If e.Item.Group IsNot Nothing AndAlso e.Item.Group.Name = "Title" AndAlso e.Item.Group.Tag <> 0 Then
            Dim Offset = e.Item.Group.Tag
            e.DrawBackground()
            Dim rect As Rectangle = Rectangle.Inflate(e.Bounds, -3, -2)
            If e.ColumnIndex = 0 Then
                rect.Width -= Offset
            Else
                rect.X -= Offset
                rect.Width += Offset
            End If
            TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.SubItem.Font, rect, e.SubItem.ForeColor, TextFormatFlags.Default Or TextFormatFlags.NoPrefix)
        Else
            e.DrawDefault = True
        End If
    End Sub

    Private Sub ListViewSummary_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles ListViewSummary.ItemSelectionChanged
        e.Item.Selected = False
    End Sub

    Private Sub MainForm_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        'EmptyTempPath()
    End Sub

    Private Sub MainForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If Not CloseAll(_CurrentImage) Then
            e.Cancel = True
        End If
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        _FileVersion = GetVersionString()
        Me.Text = GetWindowCaption()

        InitAllFileExtensions()

        PositionForm()

        AddHandler MainMenuOptions.DropDown.Closing, AddressOf ContextMenuOptions_Closing

        MainMenuUpdateAvailable.Visible = False
        MenuToolsCompare.Visible = False
        ListViewFiles.DoubleBuffer
        ListViewSummary.DoubleBuffer
        _ListViewHeader = New ListViewHeader(ListViewFiles.Handle)
        ListViewSummary.AutoResizeColumnsContstrained(ColumnHeaderAutoResizeStyle.None)
        ImageFilters = New Filters.ImageFilters(ContextMenuFilters)
        SubFiltersInitialize()
        _LoadedFiles = New LoadedFiles
        _BootStrapDB = New BootstrapDB
        _TitleDB = New FloppyDB
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

        InitDebugFeatures(My.Settings.Debug)

        DetectFloppyDrives()
        ResetAll()

        Dim Args = Environment.GetCommandLineArgs.Skip(1).ToArray

        If Args.Length > 0 Then
            ProcessFileDrop(Args, True)
        End If

        If My.Settings.CheckUpdateOnStartup Then
            CheckForUpdatesStartup()
        End If
    End Sub

    Private Sub MainForm_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.WindowWidth = Me.Width
        My.Settings.WindowHeight = Me.Height
    End Sub

    Private Sub MenuOptionsCheckUpdate_CheckStateChanged(sender As Object, e As EventArgs) Handles MenuOptionsCheckUpdate.CheckStateChanged
        My.Settings.CheckUpdateOnStartup = MenuOptionsCheckUpdate.Checked
    End Sub

    Private Sub MenuOptionsCreateBackup_CheckStateChanged(sender As Object, e As EventArgs) Handles MenuOptionsCreateBackup.CheckStateChanged
        My.Settings.CreateBackups = MenuOptionsCreateBackup.Checked
    End Sub

    Private Sub MenuOptionsDragDrop_CheckStateChanged(sender As Object, e As EventArgs) Handles MenuOptionsDragDrop.CheckStateChanged
        My.Settings.DragAndDrop = MenuOptionsDragDrop.Checked
    End Sub

    Private Sub MenuOptionsExportUnknown_CheckStateChanged(sender As Object, e As EventArgs) Handles MenuOptionsExportUnknown.CheckStateChanged
        _ExportUnknownImages = MenuOptionsExportUnknown.Checked
    End Sub

    Private Sub MenuToolsTrackLayout_Click(sender As Object, e As EventArgs) Handles MenuToolsTrackLayout.Click
        GenerateTrackLayout()
    End Sub

    Private Sub ToolStripCombo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripOEMNameCombo.SelectedIndexChanged, ToolStripDiskTypeCombo.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        FiltersApply(False)
    End Sub

    Private Sub ToolStripFATCombo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripFATCombo.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        If _CurrentImage IsNot Nothing Then
            _CurrentImage.ImageData.FATIndex = ToolStripFATCombo.SelectedIndex
            ReloadCurrentImage(False)
        End If
    End Sub

    Private Sub ToolStripSearchText_TextChanged(sender As Object, e As EventArgs) Handles ToolStripSearchText.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Debounce.Stop()
        Debounce.Start()
    End Sub
#End Region
End Class
