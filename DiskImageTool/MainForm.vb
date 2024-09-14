Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports DiskImageTool.DiskImage
Imports DiskImageTool.FloppyDB
Imports BootSectorOffsets = DiskImageTool.DiskImage.BootSector.BootSectorOffsets
Imports BPBOffsets = DiskImageTool.DiskImage.BiosParameterBlock.BPBOoffsets

Public Enum ItemScanTypes
    None = 0
    Disk = 1
    OEMName = 2
    DiskType = 4
    FreeClusters = 8
    Directory = 16
    All = 31
End Enum

Public Structure FileData
    Dim DirectoryEntry As DiskImage.DirectoryEntry
    Dim FilePath As String
    Dim Index As Integer
    Dim IsLastEntry As Boolean
    Dim LFNFileName As String
    Dim ParentOffset As Integer
End Structure

Public Structure FileSystemInfo
    Dim VolumeLabel As DirectoryEntry
    Dim OldestFileDate As Date?
    Dim NewestFileDate As Date?
End Structure

Public Structure SaveDialogFilter
    Dim Filter As String
    Dim FilterIndex As Integer
End Structure

Public Class MainForm
    Private WithEvents ContextMenuCopy1 As ContextMenuStrip
    Private WithEvents ContextMenuCopy2 As ContextMenuStrip
    Private WithEvents Debounce As Timer
    Private WithEvents ImageFilters As ImageFilters
    Public Const NULL_CHAR As Char = "�"
    Public Const SITE_URL = "https://github.com/Digitoxin1/DiskImageTool"
    Public Const UPDATE_URL = "https://api.github.com/repos/Digitoxin1/DiskImageTool/releases/latest"
    Private ReadOnly _ArchiveFilterExt As New List(Of String) From {".zip"}
    Private ReadOnly _lvwColumnSorter As ListViewColumnSorter
    Private ReadOnly _ValidFileExt As New List(Of String) From {".ima", ".img", ".imz", ".vfd", ".flp"}
    Private _BootStrapDB As BoootstrapDB
    Private _TitleDB As FloppyDB
    Private _CheckAll As Boolean = False
    Private _CurrentImageData As LoadedImageData = Nothing
    Private _Disk As DiskImage.Disk
    Private _FileVersion As String = ""
    Private _SubFilterDiskType As ComboFilter
    Private _SubFilterOEMName As ComboFilter
    Private _ListViewHeader As ListViewHeader
    Private _ListViewWidths() As Integer
    Private _LoadedFileNames As Dictionary(Of String, LoadedImageData)
    Private _ScanRun As Boolean = False
    Private _SuppressEvent As Boolean = False
    Private _DriveAEnabled As Boolean = False
    Private _DriveBEnabled As Boolean = False
    Private _ExportUnknownImages As Boolean = False

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _lvwColumnSorter = New ListViewColumnSorter
        ListViewInit()
        ComboFAT.ComboBox.DrawMode = DrawMode.OwnerDrawFixed
        AddHandler ComboFAT.ComboBox.DrawItem, AddressOf DrawComboFAT
    End Sub

    Friend Sub DiskTypeFilterUpdate(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim DiskType As String = ""

        If Not Remove Then
            'If Disk.IsValidImage Then
            '    DiskType = GetFloppyDiskTypeName(Disk.BPB, True)
            'Else
            '    DiskType = GetFloppyDiskTypeName(Disk.Data.Length, True)
            'End If
            Dim DiskTypeBySize = GetFloppyDiskType(Disk.Data.Length)

            If Disk.DiskType <> FloppyDiskType.FloppyUnknown Or DiskTypeBySize = FloppyDiskType.FloppyUnknown Then
                DiskType = GetFloppyDiskTypeName(Disk.DiskType)
            Else
                DiskType = GetFloppyDiskTypeName(DiskTypeBySize)
            End If
        End If

        If Not ImageData.Scanned Or DiskType <> ImageData.DiskType Then
            If ImageData.Scanned Then
                _SubFilterDiskType.Remove(ImageData.DiskType, UpdateFilters)
            End If

            ImageData.DiskType = DiskType

            If Not Remove Then
                _SubFilterDiskType.Add(ImageData.DiskType, UpdateFilters)
            End If
        End If
    End Sub

    Friend Function ItemScanDirectory(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False) As DirectoryScanResponse
        Dim Response As DirectoryScanResponse
        Dim HasLostClusters As Boolean = False

        If Not Remove And Disk.IsValidImage Then
            Response = ProcessDirectoryEntries(Disk.Directory, True)
            HasLostClusters = Disk.FAT.GetLostClusterCount > 0
        Else
            Response = New DirectoryScanResponse(Nothing)
        End If

        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.FileSystem_HasCreationDate, Response.HasValidCreated)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.FileSystem_HasLastAccessDate, Response.HasValidLastAccessed)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.FileSystem_HasReservedBytesSet, Response.HasReserved)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.FileSystem_HasLongFileNames, Response.HasLFN)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.FileSystem_DirectoryHasAdditionalData, Response.HasAdditionalData)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.FileSystem_DirectoryHasBootSector, Response.HasBootSector)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.FileSystem_InvalidDirectoryEntries, Response.HasInvalidDirectoryEntries)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.FAT_ChainingErrors, Response.HasFATChainingErrors)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.FAT_LostClusters, HasLostClusters)

        Return Response
    End Function

    Friend Sub ItemScanDisk(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim TitleFindResult As TitleFindResult = Nothing
        Dim FileData As FileNameData = Nothing
        Dim Disk_UnknownFormat As Boolean = Not Disk.IsValidImage
        Dim FAT_BadSectors As Boolean = False
        Dim FATS_MismatchedFATs As Boolean = False
        Dim Disk_MismatchedMediaDescriptor As Boolean = False
        Dim Disk_MismatchedImageSize As Boolean = False
        Dim Disk_CustomFormat As Boolean = False
        Dim Image_InDatabase As Boolean = False
        Dim Image_NotInDatabase As Boolean = True
        Dim Image_Verified As Boolean = False
        Dim Image_Unverified As Boolean = False
        Dim Disk_NOBPB As Boolean = False
        Dim Disk_NoBootLoader As Boolean = False
        Dim DIsk_CustomBootLoader As Boolean = False
        Dim Database_MismatchedStatus As Boolean = False

        If Not Remove Then
            If Not Disk_UnknownFormat Then
                Dim MediaDescriptor = GetFloppyDiskMediaDescriptor(Disk.DiskType)
                FAT_BadSectors = Disk.FAT.BadClusters.Count > 0
                FATS_MismatchedFATs = Disk.DiskType <> FloppyDiskType.FloppyXDF AndAlso Not Disk.FATTables.FATsMatch
                If Disk.BootSector.BPB.IsValid Then
                    If Disk.BootSector.BPB.MediaDescriptor <> MediaDescriptor Then
                        Disk_MismatchedMediaDescriptor = True
                    ElseIf Disk.DiskType = FloppyDiskType.FloppyXDF And Disk.FAT.MediaDescriptor = &HF9 Then
                        Disk_MismatchedMediaDescriptor = False
                    ElseIf Disk.FAT.HasMediaDescriptor AndAlso Disk.FAT.MediaDescriptor <> Disk.BootSector.BPB.MediaDescriptor Then
                        Disk_MismatchedMediaDescriptor = True
                    ElseIf Disk.FAT.HasMediaDescriptor AndAlso Disk.FAT.MediaDescriptor <> MediaDescriptor Then
                        Disk_MismatchedMediaDescriptor = True
                    End If
                End If
                Disk_MismatchedImageSize = Disk.CheckImageSize <> 0
                Disk_CustomFormat = GetFloppyDiskType(Disk.BPB, False) = FloppyDiskType.FloppyUnknown
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
                        FileData = New FileNameData(ImageData.FileName)
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
            ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.Image_InDatabase, Image_InDatabase)
            ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.Image_NotInDatabase, Image_NotInDatabase)
            ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.Image_Verified, Image_Verified)
            ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.Image_Unverified, Image_Unverified)
        End If

        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.FAT_BadSectors, FAT_BadSectors)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.FAT_MismatchedFATs, FATS_MismatchedFATs)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.Disk_UnknownFormat, Disk_UnknownFormat)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.Disk_MismatchedMediaDescriptor, Disk_MismatchedMediaDescriptor)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.Disk_MismatchedImageSize, Disk_MismatchedImageSize)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.Disk_CustomFormat, Disk_CustomFormat)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.Disk_NOBPB, Disk_NOBPB)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.Disk_NoBootLoader, Disk_NoBootLoader)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.DIsk_CustomBootLoader, DIsk_CustomBootLoader)

        If My.Settings.Debug Then
            ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.Database_MismatchedStatus, Database_MismatchedStatus)

            If _ExportUnknownImages Then
                If Image_NotInDatabase Then
                    If TitleFindResult Is Nothing Then
                        TitleFindResult = _TitleDB.TitleFind(Disk)
                    End If
                    If FileData Is Nothing Then
                        FileData = New FileNameData(ImageData.FileName)
                    End If
                    Dim Media = GetFloppyDiskTypeName(Disk.BPB, True)
                    _TitleDB.AddTile(FileData, Media, TitleFindResult.MD5, TitleFindResult.MD5_CP)
                End If
            End If
        End If
    End Sub

    Friend Sub ItemScanFreeClusters(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim HasFreeClusters As Boolean = False

        If Not Remove And Disk.IsValidImage Then
            HasFreeClusters = Disk.FAT.HasFreeClusters(FAT12.FreeClusterEmum.WithData)
        End If

        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.Disk_FreeClustersWithData, HasFreeClusters)
    End Sub

    Friend Sub ItemScanModified(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim IsModified As Boolean = Not Remove And (Disk IsNot Nothing AndAlso Disk.Data.Modified)

        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.ModifiedFiles, IsModified, True)
    End Sub

    Friend Sub ItemScanOEMName(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        Dim DoOEMNameCheck As Boolean = Disk.BootSector.BPB.IsValid

        Dim Bootstrap_Unknown = False
        Dim OEMName_Unknown = False
        Dim OEMName_Mismatched = False
        Dim OEMName_Verified = False
        Dim OEMName_Unverified = False
        Dim OEMName_Windows9x = False

        If Not Remove AndAlso Disk.IsValidImage Then
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
            ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.Bootstrap_Unknown, Bootstrap_Unknown)
        End If
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.OEMName_Windows9x, OEMName_Windows9x)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.OEMName_Mismatched, OEMName_Mismatched)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.OEMName_Unknown, OEMName_Unknown)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.OEMName_Verified, OEMName_Verified)
        ImageFilters.FilterUpdate(ImageData, UpdateFilters, FilterTypes.OEMName_Unverified, OEMName_Unverified)
    End Sub
    Friend Sub OEMNameFilterUpdate(Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        If Disk.IsValidImage Then
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

    Friend Function Win9xClean(Disk As Disk, Batch As Boolean) As Boolean
        Dim Result As Boolean = False

        If Batch Then
            If _TitleDB.IsVerifiedImage(Disk) Then
                Return Result
            End If
        End If

        Disk.Data.BatchEditMode = True

        If Disk.BootSector.IsWin9xOEMName Then
            Dim BootstrapType = _BootStrapDB.FindMatch(Disk.BootSector.BootStrapCode)
            If BootstrapType IsNot Nothing Then
                If BootstrapType.OEMNames.Count > 0 Then
                    Disk.BootSector.OEMName = BootstrapType.OEMNames.Item(0).Name
                    Result = True
                End If
            End If
        End If

        Dim FileList = Disk.GetFileList()

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

        Disk.Data.BatchEditMode = False


        Return Result
    End Function

    Private Shared Function DirectoryEntryCanDelete(DirectoryEntry As DiskImage.DirectoryEntry, Clear As Boolean) As Boolean
        If DirectoryEntry.IsDeleted Then
            Return False
        ElseIf Clear And Not DirectoryEntry.IsValidFile Then
            Return False
        ElseIf DirectoryEntry.IsValidDirectory Then
            Return DirectoryEntry.SubDirectory.Data.FileCount - DirectoryEntry.SubDirectory.Data.DeletedFileCount = 0
        Else
            Return True
        End If
    End Function

    Private Shared Function DirectoryEntryCanExport(DirectoryEntry As DiskImage.DirectoryEntry) As Boolean
        Return (DirectoryEntry.IsValidFile Or DirectoryEntry.IsValidVolumeName) _
            And Not DirectoryEntry.IsDeleted _
            And Not DirectoryEntry.HasInvalidFilename _
            And Not DirectoryEntry.HasInvalidExtension
    End Function

    Private Shared Function DirectoryEntryCanUndelete(DirectoryEntry As DiskImage.DirectoryEntry) As Boolean
        If Not DirectoryEntry.IsDeleted Then
            Return False
        End If

        Return DirectoryEntry.CanRestore
    End Function

    Private Shared Sub DirectoryEntryDisplayText(DirectoryEntry As DiskImage.DirectoryEntry)
        If Not DirectoryEntry.IsValidFile Then 'Or DirectoryEntry.IsDeleted Then
            Exit Sub
        End If

        Dim Caption As String = $"File - {DirectoryEntry.GetFullFileName}"
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

    Private Shared Function FileSave(FilePath As String, DirectoryEntry As DiskImage.DirectoryEntry) As Boolean
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

        SI = Item.SubItems.Add(FileData.DirectoryEntry.GetFileName)
        SI.Name = "FileName"
        If Not IsDeleted And FileData.DirectoryEntry.HasInvalidFilename Then
            SI.ForeColor = Color.Red
        Else
            SI.ForeColor = ForeColor
        End If

        SI = Item.SubItems.Add(FileData.DirectoryEntry.GetFileExtension)
        SI.Name = "FileExtension"
        If Not IsDeleted And FileData.DirectoryEntry.HasInvalidExtension Then
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
            SI = Item.SubItems.Add(ExpandedDateToString(FileData.DirectoryEntry.GetLastWriteDate, True, True, False, True))
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
            If Not IsDeleted And FileData.DirectoryEntry.HasInvalidAttributes Then
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
                SI = Item.SubItems.Add(ExpandedDateToString(FileData.DirectoryEntry.GetCreationDate, True, True, True, True))
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
                SI = Item.SubItems.Add(ExpandedDateToString(FileData.DirectoryEntry.GetLastAccessDate))
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
            If FileData.DirectoryEntry.ReservedForWinNT <> 0 Or FileData.DirectoryEntry.ReservedForFAT32 <> 0 Then
                Reserved = FileData.DirectoryEntry.ReservedForWinNT.ToString("X2")
                Reserved &= "-" & BitConverter.ToString(BitConverter.GetBytes(FileData.DirectoryEntry.ReservedForFAT32))
            End If
            SI = Item.SubItems.Add(Reserved)
            SI.ForeColor = ForeColor
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

    Private Shared Function SaveDiskImageToFile(Disk As DiskImage.Disk, FilePath As String) As Boolean
        Try
            If My.Settings.CreateBackups AndAlso IO.File.Exists(FilePath) Then
                Dim BackupPath As String = FilePath & ".bak"
                IO.File.Copy(FilePath, BackupPath, True)
            End If
            Disk.SaveFile(FilePath)
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Private Sub BootSectorEdit()
        Dim BootSectorForm As New BootSectorForm(_Disk, _BootStrapDB)

        BootSectorForm.ShowDialog()

        Dim Result As Boolean = BootSectorForm.DialogResult = DialogResult.OK

        If Result Then
            DiskImageRefresh()
        End If
    End Sub

    Private Sub BootSectorRemove()
        If Not _Disk.Directory.Data.HasBootSector Then
            Exit Sub
        End If

        Dim BootSectorBytes(BootSector.BOOT_SECTOR_SIZE - 1) As Byte

        For Counter = 0 To BootSectorBytes.Length - 1
            BootSectorBytes(Counter) = 0
        Next

        _Disk.Data.SetBytes(BootSectorBytes, _Disk.Directory.Data.BootSectorOffset)

        DiskImageRefresh()
    End Sub

    Private Sub BootSectorRestore()
        If Not _Disk.Directory.Data.HasBootSector Then
            Exit Sub
        End If

        Dim BootSectorBytes = _Disk.Data.GetBytes(_Disk.Directory.Data.BootSectorOffset, BootSector.BOOT_SECTOR_SIZE)
        _Disk.Data.SetBytes(BootSectorBytes, 0)

        DiskImageRefresh()
    End Sub

    Private Sub CheckForUpdates()
        Dim DownloadVersion As String = ""
        Dim DownloadURL As String = ""
        Dim Body As String = ""
        Dim UpdateAvailable As Boolean = False

        Cursor.Current = Cursors.WaitCursor

        Try
            Dim Request As HttpWebRequest = WebRequest.Create(UPDATE_URL)
            Request.UserAgent = "DiskImageTool"
            Dim Response As HttpWebResponse = Request.GetResponse
            Dim Reader As New StreamReader(Response.GetResponseStream)
            Dim ResponseText = Reader.ReadToEnd

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
        Catch ex As Exception
            MsgBox("An error occured while checking for updates.  Please try again later.", MsgBoxStyle.Exclamation)
            Exit Sub
        End Try

        Cursor.Current = Cursors.Default
        If DownloadVersion <> "" And DownloadURL <> "" Then
            Dim CurrentVersion = GetVersionString()
            UpdateAvailable = Version.Parse(DownloadVersion) > Version.Parse(CurrentVersion)
        End If

        If UpdateAvailable Then
            Dim Msg = $"{My.Application.Info.Title} v{DownloadVersion} is available."
            If Body <> "" Then
                Msg &= $"{vbCrLf}{vbCrLf}Whats New{vbCrLf}{New String("—", 6)}{vbCrLf}{Body}{vbCrLf}"
            End If
            Msg &= $"{vbCrLf}{vbCrLf}Do you wish to download it at this time?"

            If MsgBoxQuestion(Msg) Then
                Dim Dialog As New SaveFileDialog With {
                    .Filter = FileDialogGetFilter("Zip Archive", ".zip"),
                    .FileName = Path.GetFileName(DownloadURL),
                    .InitialDirectory = GetDownloadsFolder(),
                    .RestoreDirectory = True
                }
                Dialog.ShowDialog()
                If Dialog.FileName <> "" Then
                    Cursor.Current = Cursors.WaitCursor
                    Try
                        Dim Client As New WebClient()
                        Client.DownloadFile(DownloadURL, Dialog.FileName)
                    Catch ex As Exception
                        MsgBox("An error occured while downloading the file.", MsgBoxStyle.Exclamation)
                    End Try
                    Cursor.Current = Cursors.Default
                End If
            End If
        Else
            MsgBox($"You are running the latest version of {My.Application.Info.Title}.", MsgBoxStyle.Information)
        End If
    End Sub

    Private Sub ClearFilesPanel()
        MenuDisplayDirectorySubMenuClear()
        ListViewFiles.ListViewItemSorter = Nothing
        ListViewFiles.Items.Clear()
        BtnWin9xClean.Enabled = False
        BtnClearReservedBytes.Enabled = False
        ItemSelectionChanged()
    End Sub

    Private Sub ClearReservedBytes()
        Dim Result As Boolean = False

        If _TitleDB.IsVerifiedImage(_Disk) Then
            If Not MsgBoxQuestion("This is a verified image.  Are you sure you wish to clear reserved bytes from this image?") Then
                Exit Sub
            End If
        End If

        _Disk.Data.BatchEditMode = True

        Dim FileList = _Disk.GetFileList()

        For Each DirectoryEntry In FileList
            If DirectoryEntry.IsValid Then
                If DirectoryEntry.ReservedForWinNT <> 0 Then
                    DirectoryEntry.ReservedForWinNT = 0
                    Result = True
                End If
                If DirectoryEntry.ReservedForFAT32 <> 0 Then
                    DirectoryEntry.ReservedForFAT32 = 0
                    Result = True
                End If
            End If
        Next

        _Disk.Data.BatchEditMode = False

        If Result Then
            DiskImageRefresh()
        End If
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

    Private Function CloseAll() As Boolean
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
                            NewFilePath = GetNewFilePath(ImageData)
                            If NewFilePath = "" Then
                                Result = MyMsgBoxResult.Cancel
                                Exit For
                            End If
                        End If
                    End If
                End If

                If Result = MyMsgBoxResult.Yes Or Result = MyMsgBoxResult.YesToAll Then
                    If Not DiskImageSave(ImageData, NewFilePath) Then
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
        Dim NewFilePath As String = ""
        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem
        Dim Result As MsgBoxResult

        If CurrentImageData.Filter(FilterTypes.ModifiedFiles) Then
            Result = MsgBoxSave(CurrentImageData.FileName)
        Else
            Result = MsgBoxResult.No
        End If

        If Result = MsgBoxResult.Yes Then
            If CurrentImageData.ReadOnly Then
                NewFilePath = GetNewFilePath(CurrentImageData)
                If NewFilePath = "" Then
                    Result = MsgBoxResult.Cancel
                End If
            End If
            If Result = MsgBoxResult.Yes Then
                If Not DiskImageSave(CurrentImageData, NewFilePath) Then
                    Result = MsgBoxResult.Cancel
                End If
            End If
        End If

        If Result <> MsgBoxResult.Cancel Then
            FileClose(CurrentImageData)
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
        LoadedImageData.StringOffset = GetPathOffset()
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
        Dim ImageData1 As LoadedImageData = ComboImages.Items(0)
        Dim ImageData2 As LoadedImageData = ComboImages.Items(1)

        Dim Content = ImageCompare.CompareImages(ImageData1, ImageData2)


        Dim frmTextView = New TextViewForm("Image Comparison", Content)
        frmTextView.ShowDialog()
    End Sub

    Private Sub ConvertImageSectors()
        If _TitleDB.IsVerifiedImage(_Disk) Then
            If Not MsgBoxQuestion("This is a verified image.  Are you sure you wish to convert this image to 8 sectors per track?") Then
                Exit Sub
            End If
        End If

        Dim Size = GetFloppyDiskSize(_Disk.DiskType)
        Dim Data(Size - 1) As Byte

        Dim DataOffset As UInteger = 0
        For Offset As UInteger = 0 To _Disk.Data.Length - 1 Step Disk.BYTES_PER_SECTOR
            Dim Sector As UInteger = Offset \ Disk.BYTES_PER_SECTOR
            Dim TrackSector As UShort = Sector Mod 9
            If TrackSector < 8 Then
                _Disk.Data.CopyTo(Offset, Data, DataOffset, Disk.BYTES_PER_SECTOR)
                DataOffset += Disk.BYTES_PER_SECTOR
            End If
        Next

        DiskUpdateBytes(Data)
    End Sub

    Private Sub ConvertImageSingleSided()
        If _TitleDB.IsVerifiedImage(_Disk) Then
            If Not MsgBoxQuestion("This is a verified image.  Are you sure you wish to convert this image to a single sided image?") Then
                Exit Sub
            End If
        End If

        Dim Size = GetFloppyDiskSize(_Disk.DiskType)
        Dim Data(Size - 1) As Byte

        Dim Params = GetFloppyDiskParams(_Disk.DiskType)

        Dim DataOffset As UInteger = 0
        For Offset As UInteger = 0 To _Disk.Data.Length - 1 Step Disk.BYTES_PER_SECTOR
            Dim Sector As UInteger = Offset \ Disk.BYTES_PER_SECTOR
            Dim Track As UShort = Sector \ Params.SectorsPerTrack
            Dim Side As UShort = Track Mod 2
            If Side = 0 Then
                _Disk.Data.CopyTo(Offset, Data, DataOffset, Disk.BYTES_PER_SECTOR)
                DataOffset += Disk.BYTES_PER_SECTOR
            End If
        Next

        DiskUpdateBytes(Data)
    End Sub

    Private Sub DiskUpdateBytes(Data() As Byte)
        _Disk.Data.BatchEditMode = True
        _Disk.Data.SetBytes(Data, 0)
        _Disk.Data.Resize(Data.Length)
        _Disk.Data.BatchEditMode = False

        DiskImageRefresh()
    End Sub

    Private Sub DeleteSelectedFiles(Clear As Boolean)
        Dim Msg As String
        Dim Title As String
        Dim DialogResult As ClearSectorsForm.ClearSectorsFormResult
        Dim Item As ListViewItem
        Dim FileData As FileData
        Dim Result As Boolean = False

        If ListViewFiles.SelectedItems.Count = 0 Then
            Exit Sub
        ElseIf ListViewFiles.SelectedItems.Count = 1 Then
            Item = ListViewFiles.SelectedItems(0)
            FileData = Item.Tag
            Msg = $"Are you sure you wish to delete {FileData.DirectoryEntry.GetFullFileName}?"
            Title = "Delete File"
        Else
            Msg = "Are you sure you wish to delete the selected files?"
            Title = "Delete " & ListViewFiles.SelectedItems.Count & " Files"
        End If

        If Clear Then
            Dim ClearSectorsForm As New ClearSectorsForm(Msg, Title)
            ClearSectorsForm.ShowDialog(Me)
            DialogResult = ClearSectorsForm.Result
            ClearSectorsForm.Close()
        Else
            DialogResult.Cancelled = Not MsgBoxQuestion(Msg, Title)
            DialogResult.FillChar = 0
        End If

        If DialogResult.Cancelled Then
            Exit Sub
        End If

        _Disk.Data.BatchEditMode = True

        For Each Item In ListViewFiles.SelectedItems
            FileData = Item.Tag
            If DirectoryEntryCanDelete(FileData.DirectoryEntry, Clear) Then
                FileData.DirectoryEntry.Remove(Clear, DialogResult.FillChar)
                Result = True
            ElseIf Clear And DirectoryEntryCanDelete(FileData.DirectoryEntry, False) Then
                FileData.DirectoryEntry.Remove(False, DialogResult.FillChar)
                Result = True
            End If
        Next

        _Disk.Data.BatchEditMode = False

        If Result Then
            DiskImageRefresh()
        End If
    End Sub

    Private Sub DetectFloppyDrives()
        Dim AllDrives() = DriveInfo.GetDrives()
        _DriveAEnabled = False
        _DriveBEnabled = False

        For Each Drive In AllDrives
            If Drive.Name = "A:\" Then
                If Drive.DriveType = DriveType.Removable Then
                    _DriveAEnabled = True
                End If
            End If
            If Drive.Name = "B:\" Then
                If Drive.DriveType = DriveType.Removable Then
                    _DriveBEnabled = True
                End If
            End If
        Next

        BtnReadFloppyA.Enabled = _DriveAEnabled
        BtnReadFloppyB.Enabled = _DriveBEnabled
        BtnWriteFloppyA.Enabled = False
        BtnWriteFloppyB.Enabled = False
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
            .FullFileName = DirectoryEntry.GetFullFileName
            .CanDelete = DirectoryEntryCanDelete(DirectoryEntry, False)
            .CanUndelete = DirectoryEntryCanUndelete(DirectoryEntry)
            .CanDeleteWithFill = DirectoryEntryCanDelete(DirectoryEntry, True)
        End With

        Return Stats
    End Function

    Private Sub DiskImageProcess(DoItemScan As Boolean, ClearItems As Boolean)
        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem

        InitButtonState(_Disk, CurrentImageData)
        'PopulateSummary(_Disk, CurrentImageData)

        If _Disk IsNot Nothing AndAlso _Disk.IsValidImage Then
            If CurrentImageData.CachedRootDir Is Nothing Then
                CurrentImageData.CachedRootDir = _Disk.Directory.GetContent
            End If
            PopulateFilesPanel(CurrentImageData, ClearItems)
        Else
            ClearFilesPanel()
        End If

        PopulateSummary(_Disk, CurrentImageData)
        StatusStrip1.Refresh()

        If DoItemScan Then
            ItemScan(ItemScanTypes.All, _Disk, CurrentImageData, True)
            ComboImagesRefreshCurrentItemText()
            RefreshSaveButtons()
        End If
    End Sub

    Private Sub DiskImageRefresh()
        If _CurrentImageData IsNot Nothing Then
            _CurrentImageData.BottomIndex = ListViewFiles.GetBottomIndex
        End If

        _Disk?.Reinitialize()

        DiskImageProcess(True, False)
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
            Success = Disk IsNot Nothing
            If Success Then
                If NewFilePath = "" Then
                    NewFilePath = ImageData.SaveFile
                End If
                Success = SaveDiskImageToFile(Disk, NewFilePath)
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
            ItemScanModified(Disk, ImageData)
        End If

        Return Success
    End Function

    Private Sub DiskImagesScan(NewOnly As Boolean)
        Me.UseWaitCursor = True
        Dim T = Stopwatch.StartNew

        BtnScanNew.Visible = False
        BtnScan.Enabled = False
        If ImageFilters.FiltersApplied Then
            FiltersClear(False)
            ImageCountUpdate()
        End If

        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem
        Dim ItemScanForm As New ItemScanForm(Me, ComboImages.Items, CurrentImageData, _Disk, NewOnly, ScanType.ScanTypeFilters)
        ItemScanForm.ShowDialog()
        BtnScanNew.Visible = ItemScanForm.ItemsRemaining > 0

        If _ExportUnknownImages Then
            _TitleDB.SaveNewXML()
        End If

        ImageFilters.UpdateAllMenuItems()

        RefreshModifiedCount()
        SubFiltersPopulate()

        ComboOEMName.Visible = True
        ToolStripOEMName.Visible = True
        ComboDiskType.Visible = True
        ToolStripDiskType.Visible = True

        BtnScan.Text = "Rescan Images"
        BtnScan.Enabled = True
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

    Private Sub DisplayCrossLinkedFiles(DirectoryEntry As DiskImage.DirectoryEntry)
        Dim Msg As String = $"{DirectoryEntry.GetFullFileName()} is crosslinked with the following files:{vbCrLf}"

        For Each Crosslink In DirectoryEntry.CrossLinks
            If Crosslink <> DirectoryEntry.Offset Then
                Dim CrossLinkedFile = _Disk.GetDirectoryEntryByOffset(Crosslink)
                Msg &= vbCrLf & CrossLinkedFile.GetFullFileName()
            End If
        Next
        MsgBox(Msg, MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
    End Sub

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

    Private Sub DrawComboFAT(ByVal sender As Object, ByVal e As DrawItemEventArgs)
        e.DrawBackground()

        If e.Index >= 0 Then
            Dim Item As String = ComboFAT.Items(e.Index)

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

    Private Sub ExportDebugScript()
        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem
        GenerateDebugPackage(_Disk, CurrentImageData)
    End Sub

    Private Sub FATEdit(Index As UShort)
        Dim frmFATEdit As New FATEditForm(_Disk, Index)

        frmFATEdit.ShowDialog()

        If frmFATEdit.Updated Then
            DiskImageRefresh()
        End If
    End Sub

    Private Sub FATSubMenuRefresh(Disk As Disk, CurrentImageData As LoadedImageData, FATTablesMatch As Boolean)
        For Each Item As ToolStripMenuItem In BtnEditFAT.DropDownItems
            RemoveHandler Item.Click, AddressOf BtnEditFAT_Click
        Next
        BtnEditFAT.DropDownItems.Clear()
        BtnEditFAT.Tag = Nothing
        ComboFAT.Items.Clear()

        If Disk IsNot Nothing AndAlso Disk.IsValidImage Then
            If FATTablesMatch Then
                BtnEditFAT.Tag = -1
            Else
                For Counter = 0 To Disk.BPB.NumberOfFATs - 1
                    Dim Item As New ToolStripMenuItem With {
                       .Text = "FAT &" & Counter + 1,
                       .Tag = Counter
                    }
                    BtnEditFAT.DropDownItems.Add(Item)
                    AddHandler Item.Click, AddressOf BtnEditFAT_Click
                    ComboFAT.Items.Add("FAT " & Counter + 1)
                Next
                _SuppressEvent = True
                If CurrentImageData Is Nothing Then
                    ComboFAT.SelectedIndex = 0
                Else
                    If CurrentImageData.FATIndex > Disk.BPB.NumberOfFATs - 1 Or FATTablesMatch Then
                        CurrentImageData.FATIndex = 0
                    End If
                    ComboFAT.SelectedIndex = CurrentImageData.FATIndex
                End If
                _SuppressEvent = False
            End If
        End If
    End Sub

    Private Sub FileAdd(Offset As UInteger)
        Dim Dialog = New OpenFileDialog

        If Dialog.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If

        Dim FileInfo As New IO.FileInfo(Dialog.FileName)

        Dim ClusterSize = _Disk.BPB.BytesPerCluster

        'Attempt to only use empty free clusters if space is available
        Dim FreeClusters = _Disk.FAT.GetFreeClusters(FAT12.FreeClusterEmum.WithoutData)
        Dim AvailableSpace = FreeClusters.Count * ClusterSize
        If FileInfo.Length > AvailableSpace Then
            FreeClusters = _Disk.FAT.GetFreeClusters(FAT12.FreeClusterEmum.All)
            AvailableSpace = FreeClusters.Count * ClusterSize
        End If

        If FileInfo.Length > AvailableSpace Then
            MsgBox("There is not enough available space on this disk to add this file.", vbExclamation)
            Exit Sub
        End If

        Dim Directory As IDirectory
        If Offset = 0 Then
            Directory = _Disk.Directory
        Else
            Directory = _Disk.GetDirectoryEntryByOffset(Offset).SubDirectory
        End If

        If Directory.Data.EntryCount >= Directory.Data.MaxEntries Then
            MsgBox("There are no more available entries in this directory to add this file.", vbExclamation)
            Exit Sub
        End If

        _Disk.Data.BatchEditMode = True

        Dim FirstCluster As UShort = 0

        If FileInfo.Length > 0 Then
            'Load file into buffer, padding with empty space if needed
            Dim FileSize = Math.Ceiling(FileInfo.Length / ClusterSize) * ClusterSize
            Dim FileBuffer(FileSize - 1) As Byte
            Dim n As Integer
            Using fs = FileInfo.OpenRead()
                n = fs.Read(FileBuffer, 0, Math.Min(FileInfo.Length, FileBuffer.Length))
            End Using
            For Counter As Integer = n To FileBuffer.Length - 1
                FileBuffer(Counter) = 0
            Next

            Dim FreeClusterIndex As Integer = 0
            Dim LastCluster As UShort = 0

            For Counter As Integer = 0 To FileBuffer.Length - 1 Step ClusterSize
                Dim Cluster = FreeClusters(FreeClusterIndex)
                If Counter = 0 Then
                    FirstCluster = Cluster
                Else
                    _Disk.FATTables.UpdateTableEntry(LastCluster, Cluster)
                End If
                Dim ClusterOffset = _Disk.BPB.ClusterToOffset(Cluster)
                Dim Buffer = _Disk.Data.GetBytes(ClusterOffset, ClusterSize)
                Array.Copy(FileBuffer, Counter, Buffer, 0, ClusterSize)
                _Disk.Data.SetBytes(Buffer, ClusterOffset)
                LastCluster = Cluster
                FreeClusterIndex += 1
            Next

            If LastCluster > 0 Then
                _Disk.FATTables.UpdateTableEntry(LastCluster, FAT12.FAT_LAST_CLUSTER_END)
            End If

            _Disk.FATTables.UpdateFAT12()
        End If

        Dim NewEntry = New DirectoryEntryBase
        NewEntry.SetFileInfo(FileInfo, False, False)
        NewEntry.StartingCluster = FirstCluster

        Dim Entry = Directory.GetFile(Directory.Data.EntryCount)
        Entry.Data = NewEntry.Data

        _Disk.Data.BatchEditMode = False

        DiskImageRefresh()
    End Sub

    Private Sub FileClose(ImageData As LoadedImageData)
        ItemScan(ItemScanTypes.All, _Disk, ImageData, True, True)
        _LoadedFileNames.Remove(ImageData.DisplayPath)

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
            BtnCompare.Enabled = ComboImages.Items.Count > 1
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
            Dim Msg As String = $"Error saving file '{IO.Path.GetFileName(Dialog.FileName)}'."
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
                    Dim FilePath = IO.Path.Combine(Path, CleanPathName(FileData.FilePath), CleanFileName(DirectoryEntry.GetFullFileName))
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
            Dim Msg As String = $"{FileCount} of {TotalFiles} {"file".Pluralize(TotalFiles)} exported successfully."
            MsgBox(Msg, MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
        End If
    End Sub

    Private Sub FileInfoUpdate()
        If ListViewFiles.SelectedItems.Count > 0 Then
            ToolStripFileCount.Text = $"{ListViewFiles.SelectedItems.Count} of {ListViewFiles.Items.Count} {"File".Pluralize(ListViewFiles.Items.Count)} Selected"
        Else
            ToolStripFileCount.Text = $"{ListViewFiles.Items.Count} {"File".Pluralize(ListViewFiles.Items.Count)}"
        End If
        ToolStripFileCount.Visible = True

        If ListViewFiles.SelectedItems.Count = 1 Then
            Dim FileData As FileData = ListViewFiles.SelectedItems(0).Tag

            If FileData.DirectoryEntry.StartingCluster >= 2 Then

                Dim Sector = _Disk.BPB.ClusterToSector(FileData.DirectoryEntry.StartingCluster)
                ToolStripFileSector.Text = $"Sector {Sector}"
                ToolStripFileSector.Visible = True

                Dim Track = _Disk.BPB.SectorToTrack(Sector)
                Dim Side = _Disk.BPB.SectorToSide(Sector)

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

    Private Function FilePropertiesEdit() As Boolean
        Dim Result As Boolean = False

        Dim frmFileProperties As New FilePropertiesForm(_Disk, ListViewFiles.SelectedItems)
        frmFileProperties.ShowDialog()

        If frmFileProperties.DialogResult = DialogResult.OK Then
            Result = frmFileProperties.Updated

            If Result Then
                DiskImageRefresh()
            End If
        End If

        Return Result
    End Function

    Private Sub FileReplace(Item As ListViewItem)
        Dim Dialog = New OpenFileDialog
        Dim Result As ReplaceFileForm.ReplaceFileFormResult

        If Dialog.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If

        Dim FileInfo As New IO.FileInfo(Dialog.FileName)

        Dim FileData As FileData = Item.Tag

        Dim AvailableSpace = _Disk.FAT.GetFreeSpace(_Disk.BPB.BytesPerCluster) + FileData.DirectoryEntry.GetSizeOnDisk

        Dim ReplaceFileForm As New ReplaceFileForm(AvailableSpace)
        With ReplaceFileForm
            .SetOriginalFile(FileData.DirectoryEntry.GetFullFileName, FileData.DirectoryEntry.GetLastWriteDate.DateObject, FileData.DirectoryEntry.FileSize)
            .SetNewFile(CleanDOSFileName(FileInfo.Name), FileInfo.LastWriteTime, FileInfo.Length)
            .RefreshText()
            .ShowDialog(Me)
            Result = .Result
            .Close()
        End With

        If Not Result.Cancelled Then
            Dim ClusterSize = _Disk.BPB.BytesPerCluster

            'Load file into buffer, padding with empty space if needed
            Dim FileBuffer(Result.FileSize - 1) As Byte
            Dim n As Integer
            Using fs = FileInfo.OpenRead()
                n = fs.Read(FileBuffer, 0, Math.Min(FileInfo.Length, FileBuffer.Length))
            End Using
            For Counter As Integer = n To FileBuffer.Length - 1
                FileBuffer(Counter) = Result.FillChar
            Next

            'Attempt to only use empty free clusters if space is available
            Dim FreeClusters = _Disk.FAT.GetFreeClusters(FAT12.FreeClusterEmum.WithoutData)
            Dim AvailableEmptySpace = FreeClusters.Count * ClusterSize + FileData.DirectoryEntry.GetSizeOnDisk
            If Result.FileSize > AvailableEmptySpace Then
                FreeClusters = _Disk.FAT.GetFreeClusters(FAT12.FreeClusterEmum.All)
            End If

            _Disk.Data.BatchEditMode = True

            If Result.FileNameChanged Then
                FileData.DirectoryEntry.SetFileName(Result.FileName)
            End If

            If Result.FileDateChanged Then
                FileData.DirectoryEntry.SetLastWriteDate(Result.FileDate)
            End If

            FileData.DirectoryEntry.FileSize = Result.FileSize

            Dim ClusterCount = FileData.DirectoryEntry.FATChain.Clusters.Count
            Dim ClusterIndex As Integer = 0
            Dim FreeClusterIndex As Integer = 0
            Dim Cluster As UShort
            Dim LastCluster As UShort = 0
            Dim FATUpdated As Boolean = False

            'Update assigned clusters and assign new ones if new file is larger
            For Counter As Integer = 0 To FileBuffer.Length - 1 Step ClusterSize
                Dim Length = Math.Min(ClusterSize, FileBuffer.Length - Counter)

                If ClusterIndex < ClusterCount Then
                    Cluster = FileData.DirectoryEntry.FATChain.Clusters(ClusterIndex)
                Else
                    Cluster = FreeClusters(FreeClusterIndex)
                    If LastCluster > 0 Then
                        _Disk.FATTables.UpdateTableEntry(LastCluster, Cluster)
                        FATUpdated = True
                    End If
                    FreeClusterIndex += 1
                End If

                Dim ClusterOffset = _Disk.BPB.ClusterToOffset(Cluster)
                Dim Buffer = _Disk.Data.GetBytes(ClusterOffset, ClusterSize)
                Array.Copy(FileBuffer, Counter, Buffer, 0, Length)
                _Disk.Data.SetBytes(Buffer, ClusterOffset)
                ClusterIndex += 1
                LastCluster = Cluster
            Next

            If LastCluster > 0 Then
                _Disk.FATTables.UpdateTableEntry(LastCluster, FAT12.FAT_LAST_CLUSTER_END)
                FATUpdated = True
            End If

            'Free unused clusters if new file is smaller
            If ClusterIndex < ClusterCount Then
                Dim Buffer(ClusterSize - 1) As Byte
                For i = 0 To Buffer.Length - 1
                    Buffer(i) = Result.FillChar
                Next

                For Index = ClusterIndex To ClusterCount - 1
                    Cluster = FileData.DirectoryEntry.FATChain.Clusters(Index)
                    Dim ClusterOffset = _Disk.BPB.ClusterToOffset(Cluster)
                    _Disk.Data.SetBytes(Buffer, ClusterOffset)

                    _Disk.FATTables.UpdateTableEntry(Cluster, FAT12.FAT_FREE_CLUSTER)
                    FATUpdated = True
                Next
            End If

            If FATUpdated Then
                _Disk.FATTables.UpdateFAT12()
            End If

            _Disk.Data.BatchEditMode = False

            DiskImageRefresh()
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

        ProcessFileDrop(Dialog.FileNames)
    End Sub

    Private Sub FiltersApply(ResetSubFilters As Boolean)
        Dim HasFilter As Boolean = False
        Dim AppliedFilters As Integer = 0
        Dim r As Regex = Nothing

        If ResetSubFilters Then
            SubFiltersClearFilter()
        End If

        Dim FiltersChecked As Boolean = ImageFilters.AreFiltersApplied()
        If FiltersChecked Then
            AppliedFilters = ImageFilters.GetAppliedFilters(True)
        End If
        HasFilter = HasFilter Or FiltersChecked

        Dim TextFilter As String = TxtSearch.Text.Trim.ToLower
        Dim HasTextFilter = TextFilter.Length > 0
        If HasTextFilter Then
            Dim Pattern As String = "(?:\W|^|_)(" & Regex.Escape(TextFilter) & ")"
            r = New Regex(Pattern, RegexOptions.IgnoreCase)
        End If
        HasFilter = HasFilter Or HasTextFilter

        Dim OEMNameItem As ComboFilterItem = ComboOEMName.SelectedItem
        Dim HasOEMNameFilter = OEMNameItem IsNot Nothing AndAlso Not OEMNameItem.AllItems
        HasFilter = HasFilter Or HasOEMNameFilter

        Dim DiskTypeItem As ComboFilterItem = ComboDiskType.SelectedItem
        Dim HasDiskTypeFilter = DiskTypeItem IsNot Nothing AndAlso Not DiskTypeItem.AllItems
        HasFilter = HasFilter Or HasDiskTypeFilter

        If HasFilter Then
            Cursor.Current = Cursors.WaitCursor

            If ResetSubFilters Then
                SubFiltersClear()
            End If

            ComboImagesFiltered.BeginUpdate()
            ComboImagesFiltered.Items.Clear()

            For Each ImageData As LoadedImageData In ComboImages.Items
                Dim ShowItem As Boolean = True

                If ShowItem AndAlso FiltersChecked Then
                    ShowItem = Not ImageFilters.IsFiltered(ImageData, AppliedFilters, ImageFilters.FilterCounts)
                End If

                If ShowItem AndAlso ResetSubFilters Then
                    SubFilterOEMNameAdd(ImageData.OEMName, False)
                    _SubFilterDiskType.Add(ImageData.DiskType, False)
                End If

                If ShowItem AndAlso HasTextFilter Then
                    ShowItem = r.IsMatch(ImageData.DisplayPath)
                End If

                If ShowItem AndAlso HasOEMNameFilter Then
                    Dim IsValidImage = Not ImageData.Filter(FilterTypes.Disk_UnknownFormat)
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

        BtnScan.Text = "Scan Images"
        RefreshFilterButtons(False)
    End Sub

    Private Sub FixFileSize(Item As ListViewItem)
        Dim FileData As FileData = Item.Tag

        If Not FileData.DirectoryEntry.HasIncorrectFileSize Then
            Exit Sub
        End If

        FileData.DirectoryEntry.FileSize = FileData.DirectoryEntry.GetAllocatedSizeFromFAT
        DiskImageRefresh()
    End Sub

    Private Sub FixImageSize()
        Dim Result As Boolean = True

        If _TitleDB.IsVerifiedImage(_Disk) Then
            If Not MsgBoxQuestion("This is a verified image.  Are you sure you wish to adjust the image size for this image?") Then
                Exit Sub
            End If
        End If

        Dim Compare = _Disk.CheckImageSize

        If Compare = 0 Then
            Result = False
        ElseIf Compare < 0 Then
            Result = MsgBoxQuestion($"The image size is smaller than the detected size.{vbCrLf}{vbCrLf}Are you sure you wish to increase the image size?")
        Else
            Dim ReportedSize = _Disk.BPB.ReportedImageSize
            Dim Data = _Disk.Data.GetBytes(ReportedSize, _Disk.Data.Length - ReportedSize)
            Dim HasData As Boolean = False
            For Each b In Data
                If b <> 0 Then
                    HasData = True
                    Exit For
                End If
            Next
            If HasData Then
                Result = MsgBoxQuestion($"There is data in the overdumped region of the image.{vbCrLf}{vbCrLf}Are you sure you wish to truncate the image?")
            End If
        End If

        If Result Then
            If _Disk.FixImageSize Then
                DiskImageRefresh()
            End If
        End If
    End Sub

    Private Function FloppyDiskGetReadOptions(FloppyDrive As FloppyInterface) As BiosParameterBlock
        Dim DetectedType As FloppyDiskType
        Dim ReturnedType As FloppyDiskType
        Dim BootSector As BootSector = Nothing

        Dim Buffer(Disk.BYTES_PER_SECTOR - 1) As Byte
        Dim BytesRead = FloppyDrive.ReadSector(0, Buffer)
        If BytesRead = Buffer.Length Then
            BootSector = New BootSector(New ImageByteArray(Buffer))
            DetectedType = GetFloppyDiskType(BootSector.BPB, False)
        Else
            DetectedType = FloppyDiskType.FloppyUnknown
        End If

        Dim FloppyReadOptionsForm As New FloppyReadOptionsForm(DetectedType)
        FloppyReadOptionsForm.ShowDialog(Me)
        ReturnedType = FloppyReadOptionsForm.DiskType
        FloppyReadOptionsForm.Close()

        If FloppyReadOptionsForm.DialogResult = DialogResult.Cancel Then
            Return Nothing
        ElseIf ReturnedType = -1 Then
            Return Nothing
        ElseIf BootSector IsNot Nothing AndAlso DetectedType = ReturnedType Then
            Return BootSector.BPB
        Else
            Return BuildBPB(ReturnedType)
        End If
    End Function

    Private Function FloppyDiskWriteOptions(DoFormat As Boolean, DetectedType As FloppyDiskType, ImageType As FloppyDiskType) As FloppyWriteOptionsForm.FloppyWriteOptions
        Dim WriteOptions As FloppyWriteOptionsForm.FloppyWriteOptions

        Dim FloppyWriteOptioonsForm As New FloppyWriteOptionsForm(DoFormat, DetectedType, ImageType)
        FloppyWriteOptioonsForm.ShowDialog(Me)
        WriteOptions = FloppyWriteOptioonsForm.WriteOptions
        FloppyWriteOptioonsForm.Close()

        Return WriteOptions
    End Function

    Private Sub FloppyDiskRead(Drive As FloppyDriveEnum)
        Dim FloppyDrive = New FloppyInterface
        Dim DriveLetter = FloppyInterface.GetDriveLetter(Drive)
        Dim DriveName = DriveLetter & ":\"
        Dim DriveInfo = New DriveInfo(DriveName)
        Dim Result = DriveInfo.IsReady

        If Result Then
            Result = FloppyDrive.OpenRead(Drive)
        End If

        If Result Then
            Dim BPB = FloppyDiskGetReadOptions(FloppyDrive)
            If BPB IsNot Nothing Then
                Dim FloppyAccessForm As New FloppyAccessForm(FloppyDrive, BPB, FloppyAccessForm.FloppyAccessType.Read)
                FloppyAccessForm.ShowDialog(Me)
                If FloppyAccessForm.Complete Then
                    Dim DiskType = GetFloppyDiskType(BPB, False)
                    FloppyDiskSaveFile(FloppyAccessForm.DiskBuffer, DiskType)
                End If
                FloppyAccessForm.Close()
            End If
            FloppyDrive.Close()
        Else
            MsgBox($"Floppy drive {DriveLetter} is not ready.{vbCrLf}{vbCrLf}Please verify that a formatted disk is inserted into drive {DriveLetter}.", MsgBoxStyle.Exclamation)
        End If
    End Sub

    Private Sub FloppyDiskWrite(Drive As FloppyDriveEnum)
        If _Disk Is Nothing Then
            Exit Sub
        End If

        Dim FloppyDrive = New FloppyInterface
        Dim DriveLetter = FloppyInterface.GetDriveLetter(Drive)
        Dim DriveName = DriveLetter & ":\"
        Dim DriveInfo = New DriveInfo(DriveName)
        Dim IsReady = DriveInfo.IsReady
        Dim NewDiskType = GetFloppyDiskType(_Disk.BPB, False)
        Dim NewTypeName = GetFloppyDiskTypeName(NewDiskType) & " Floppy"
        Dim DetectedType As FloppyDiskType = 255
        Dim DoFormat = Not IsReady

        Dim MsgBoxResult As MsgBoxResult
        If IsReady Then
            Dim Result = FloppyDrive.OpenRead(Drive)
            If Result Then
                Dim Buffer(Disk.BYTES_PER_SECTOR - 1) As Byte
                Dim BytesRead = FloppyDrive.ReadSector(0, Buffer)
                If BytesRead = Buffer.Length Then
                    Dim BootSector = New BootSector(New ImageByteArray(Buffer))
                    DetectedType = GetFloppyDiskType(BootSector.BPB, False)
                Else
                    DetectedType = FloppyDiskType.FloppyUnknown
                End If
                FloppyDrive.Close()
            Else
                DetectedType = FloppyDiskType.FloppyUnknown
            End If
            Dim Msg As String
            If DetectedType = NewDiskType Then
                Msg = $"Warning: The disk in drive {DriveLetter} is not empty.{vbCrLf}{vbCrLf}If you continue, the disk will be overwritten.{vbCrLf}{vbCrLf}Do you wish to continue?"
            ElseIf DetectedType = -1 Then
                DoFormat = True
                Msg = $"Warning: The disk in drive {DriveLetter} is not empty, but I am unable to determine the format type.{vbCrLf}{vbCrLf}The image you are attempting to write is a {NewTypeName} image.{vbCrLf}{vbCrLf}If you continue, the disk will be overwritten and may be unreadable.{vbCrLf}{vbCrLf}Do you wish to continue?"
            Else
                DoFormat = True
                Dim DetectedTypeName = GetFloppyDiskTypeName(DetectedType) & " Floppy"
                Msg = $"Warning: The disk in drive {DriveLetter} is not empty and is formatted as a {DetectedTypeName}.{vbCrLf}{vbCrLf}The image you are attempting to write is a {NewTypeName} image.{vbCrLf}{vbCrLf}If you continue, the disk will be overwritten and may be unreadable.{vbCrLf}{vbCrLf}Do you wish to continue?"
            End If
            MsgBoxResult = MsgBox(Msg, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkCancel Or MsgBoxStyle.DefaultButton2)
        Else
            MsgBoxResult = MsgBoxResult.Ok
        End If

        If MsgBoxResult = MsgBoxResult.Ok Then
            Dim Result = FloppyDrive.OpenWrite(Drive)
            If Result Then
                Dim WriteOptions = FloppyDiskWriteOptions(DoFormat, DetectedType, NewDiskType)
                If Not WriteOptions.Cancelled Then
                    Dim FloppyAccessForm As New FloppyAccessForm(FloppyDrive, _Disk.BPB, FloppyAccessForm.FloppyAccessType.Write) With {
                        .DiskBuffer = _Disk.Data.Data,
                        .DoFormat = WriteOptions.Format,
                        .DoVerify = WriteOptions.Verify
                    }
                    FloppyAccessForm.ShowDialog(Me)
                    FloppyAccessForm.Close()
                End If
                FloppyDrive.Close()
            Else
                MsgBox($"Error: Unable to write to the drive at this time.", MsgBoxStyle.Exclamation)
            End If
        End If
    End Sub

    Private Sub FloppyDiskSaveFile(Buffer() As Byte, DiskType As FloppyDiskType)
        Dim FileExt = ".ima"
        Dim FileFilter = GetSaveDialogFilters(DiskType, FileExt)

        Dim Dialog = New SaveFileDialog With {
            .Filter = FileFilter.Filter,
            .FilterIndex = FileFilter.FilterIndex,
            .DefaultExt = FileExt
        }

        AddHandler Dialog.FileOk, Sub(sender As Object, e As CancelEventArgs)
                                      If _LoadedFileNames.ContainsKey(Dialog.FileName) Then
                                          Dim Msg As String = Path.GetFileName(Dialog.FileName) &
                                            $"{vbCrLf}This file is currently open in {Application.ProductName}." &
                                            $"Try again with a different file name."
                                          MsgBox(Msg, MsgBoxStyle.Exclamation, "Save As")
                                          e.Cancel = True
                                      End If
                                  End Sub

        If Dialog.ShowDialog = DialogResult.OK Then
            IO.File.WriteAllBytes(Dialog.FileName, Buffer)
            ProcessFileDrop(Dialog.FileName)
        End If
    End Sub

    Private Function GetFileDataFromDirectoryEntry(Index As Integer, DirectoryEntry As DiskImage.DirectoryEntry, ParentOffset As UInteger, FilePath As String, IsLastEntry As Boolean, LFNFileName As String) As FileData
        Dim Response As FileData
        With Response
            .Index = Index
            .FilePath = FilePath
            .DirectoryEntry = DirectoryEntry
            .IsLastEntry = IsLastEntry
            .ParentOffset = ParentOffset
            .LFNFileName = LFNFileName
        End With

        Return Response
    End Function

    Private Function GetFileSystemInfo(Disk As Disk) As FileSystemInfo
        Dim fsi As FileSystemInfo

        fsi.OldestFileDate = Nothing
        fsi.NewestFileDate = Nothing
        fsi.VolumeLabel = Nothing

        Dim FileList = Disk.GetFileList()

        For Each DirectoryEntry In FileList
            If DirectoryEntry.IsValid Then
                If fsi.VolumeLabel Is Nothing AndAlso DirectoryEntry.IsValidVolumeName AndAlso DirectoryEntry.IsInRoot Then
                    fsi.VolumeLabel = DirectoryEntry
                End If
                Dim LastWriteDate = DirectoryEntry.GetLastWriteDate
                If LastWriteDate.IsValidDate Then
                    If fsi.OldestFileDate Is Nothing Then
                        fsi.OldestFileDate = LastWriteDate.DateObject
                    ElseIf fsi.OldestFileDate.Value.CompareTo(LastWriteDate.DateObject) > 0 Then
                        fsi.OldestFileDate = LastWriteDate.DateObject
                    End If
                    If fsi.NewestFileDate Is Nothing Then
                        fsi.NewestFileDate = LastWriteDate.DateObject
                    ElseIf fsi.NewestFileDate.Value.CompareTo(LastWriteDate.DateObject) < 0 Then
                        fsi.NewestFileDate = LastWriteDate.DateObject
                    End If
                End If
            End If
        Next

        Return fsi
    End Function

    Private Function GetLoadDialogFilters() As String
        Dim FileFilter As String
        Dim ExtensionList As List(Of String)


        FileFilter = FileDialogGetFilter("All Floppy Disk Images", _ValidFileExt)

        ExtensionList = New List(Of String) From {".imz"}
        FileFilter = FileDialogAppendFilter(FileFilter, "Compressed Disk Image", ExtensionList)

        ExtensionList = New List(Of String) From {".vfd", ".flp"}
        FileFilter = FileDialogAppendFilter(FileFilter, "Virtual Floppy Disk", ExtensionList)

        FileFilter = FileDialogAppendFilter(FileFilter, "Zip Archive", ".zip")
        FileFilter = FileDialogAppendFilter(FileFilter, "All files", ".*")

        Return FileFilter
    End Function

    Private Function GetModifiedImageList() As List(Of LoadedImageData)
        Dim ModifyImageList As New List(Of LoadedImageData)

        For Each ImageData As LoadedImageData In ComboImages.Items
            If ImageData.Filter(FilterTypes.ModifiedFiles) Then
                ModifyImageList.Add(ImageData)
            End If
        Next

        Return ModifyImageList
    End Function

    Private Function GetNewFilePath(ImageData As LoadedImageData) As String
        Dim Disk As DiskImage.Disk
        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem
        Dim FilePath = ImageData.SaveFile
        Dim NewFilePath As String = ""
        Dim DiskType As FloppyDiskType = FloppyDiskType.FloppyUnknown

        If ImageData Is CurrentImageData Then
            Disk = _Disk
        Else
            Disk = DiskImageLoad(ImageData)
        End If

        If Disk IsNot Nothing Then
            DiskType = Disk.DiskType
        End If

        Dim FileExt = IO.Path.GetExtension(FilePath)
        Dim FileFilter = GetSaveDialogFilters(DiskType, FileExt)

        Dim Dialog = New SaveFileDialog With {
            .InitialDirectory = IO.Path.GetDirectoryName(FilePath),
            .FileName = IO.Path.GetFileName(FilePath),
            .Filter = FileFilter.Filter,
            .FilterIndex = FileFilter.FilterIndex,
            .DefaultExt = FileExt
        }

        AddHandler Dialog.FileOk, Sub(sender As Object, e As CancelEventArgs)
                                      If Dialog.FileName <> FilePath AndAlso _LoadedFileNames.ContainsKey(Dialog.FileName) Then
                                          Dim Msg As String = Path.GetFileName(Dialog.FileName) &
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

    Private Function GetPathOffset() As Integer
        Dim PathName As String = ""
        Dim CheckPath As Boolean = False

        For Each ImageData As LoadedImageData In ComboImages.Items
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

    Private Function GetSaveDialogFilters(DiskType As FloppyDiskType, FileExt As String) As SaveDialogFilter
        Dim Response As SaveDialogFilter
        Dim CurrentIndex As Integer = 1
        Dim Extension As String
        Dim ExtensionList As List(Of String)

        Response.FilterIndex = 0

        ExtensionList = New List(Of String) From {".ima", ".img"}
        For Each Extension In ExtensionList
            If FileExt.Equals(Extension, StringComparison.OrdinalIgnoreCase) Then
                Response.FilterIndex = CurrentIndex
            End If
        Next
        Response.Filter = FileDialogGetFilter("Floppy Disk Image", ExtensionList)
        CurrentIndex += 1

        ExtensionList = New List(Of String) From {".vfd", ".flp"}
        For Each Extension In ExtensionList
            If FileExt.Equals(Extension, StringComparison.OrdinalIgnoreCase) Then
                Response.FilterIndex = CurrentIndex
            End If
        Next
        Response.Filter = FileDialogAppendFilter(Response.Filter, "Virtual Floppy Disk", ExtensionList)
        CurrentIndex += 1

        Dim Items = System.Enum.GetValues(GetType(FloppyDiskType))
        For Each Item As Integer In Items
            Extension = GetFileFilterExtByType(Item)
            If Extension <> "" Then
                Dim Description = GetFileFilterDescriptionByType(Item)
                If FileExt.Equals(Extension, StringComparison.OrdinalIgnoreCase) Then
                    Response.Filter = FileDialogAppendFilter(Response.Filter, Description, Extension)
                    Response.FilterIndex = CurrentIndex
                    CurrentIndex += 1
                ElseIf Item = DiskType Then
                    Response.Filter = FileDialogAppendFilter(Response.Filter, Description, Extension)
                    CurrentIndex += 1
                End If
            End If
        Next

        Response.Filter = FileDialogAppendFilter(Response.Filter, "All files", ".*")
        If Response.FilterIndex = 0 Then
            Response.FilterIndex = CurrentIndex
        End If

        Return Response
    End Function

    Private Function GetWindowCaption() As String
        Return My.Application.Info.ProductName & " v" & _FileVersion
    End Function

    Private Sub HexDisplayBadSectors()
        Dim HexViewSectorData = HexViewBadSectors(_Disk)

        If DisplayHexViewForm(HexViewSectorData) Then
            DiskImageRefresh()
        End If
    End Sub

    Private Sub HexDisplayBootSector()
        Dim HexViewSectorData = HexViewBootSector(_Disk)

        If DisplayHexViewForm(HexViewSectorData) Then
            DiskImageRefresh()
        End If
    End Sub

    Private Sub HexDisplayDirectoryEntry(Offset As UInteger)
        Dim HexViewSectorData = HexViewDirectoryEntry(_Disk, Offset)

        If HexViewSectorData IsNot Nothing Then
            If DisplayHexViewForm(HexViewSectorData) Then
                DiskImageRefresh()
            End If
        End If
    End Sub

    Private Sub HexDisplayDiskImage()
        Dim HexViewSectorData = New HexViewSectorData(_Disk, 0, _Disk.Data.Length) With {
            .Description = "Disk"
        }

        If DisplayHexViewForm(HexViewSectorData, True, True, False) Then
            DiskImageRefresh()
        End If
    End Sub

    Private Sub HexDisplayFAT()
        Dim HexViewSectorData = HexViewFAT(_Disk)

        Dim SyncBlocks = _Disk.BPB.NumberOfFATEntries > 1 AndAlso _Disk.DiskType <> FloppyDiskType.FloppyXDF

        If DisplayHexViewForm(HexViewSectorData, SyncBlocks) Then
            DiskImageRefresh()
        End If
    End Sub

    Private Sub HexDisplayFreeClusters()
        Dim HexViewSectorData = New HexViewSectorData(_Disk, _Disk.FAT.GetFreeClusters(FAT12.FreeClusterEmum.WithData)) With {
            .Description = "Free Clusters"
        }

        If DisplayHexViewForm(HexViewSectorData) Then
            DiskImageRefresh()
        End If
    End Sub

    Private Sub HexDisplayLostClusters()
        Dim HexViewSectorData = HexViewLostClusters(_Disk)

        If DisplayHexViewForm(HexViewSectorData) Then
            DiskImageRefresh()
        End If
    End Sub
    Private Sub ImageCountUpdate()
        If ImageFilters.FiltersApplied Then
            ToolStripImageCount.Text = $"{ComboImagesFiltered.Items.Count} of {ComboImages.Items.Count} {"Image".Pluralize(ComboImages.Items.Count)}"
        Else
            ToolStripImageCount.Text = $"{ComboImages.Items.Count} {"Image".Pluralize(ComboImages.Items.Count)}"
        End If
    End Sub

    Private Sub InitButtonState(Disk As Disk, CurrentImageData As LoadedImageData)
        Dim FATTablesMatch As Boolean = True
        Dim PrevVisible = ComboFAT.Visible

        If Disk IsNot Nothing Then
            FATTablesMatch = Disk.DiskType = FloppyDiskType.FloppyXDF OrElse Disk.FATTables.FATsMatch
            BtnDisplayBootSector.Enabled = Disk.CheckSize
            BtnEditBootSector.Enabled = Disk.CheckSize
            BtnDisplayDisk.Enabled = Disk.CheckSize
            BtnDisplayFAT.Enabled = Disk.IsValidImage
            BtnEditFAT.Enabled = Disk.IsValidImage
            BtnDisplayDirectory.Enabled = Disk.IsValidImage
            ToolStripSeparatorFAT.Visible = Not FATTablesMatch
            ComboFAT.Visible = Not FATTablesMatch
            ComboFAT.Width = 60
            BtnWriteFloppyA.Enabled = _DriveAEnabled
            BtnWriteFloppyB.Enabled = _DriveBEnabled
            BtnAddFile.Enabled = Disk.IsValidImage
        Else
            BtnDisplayBootSector.Enabled = False
            BtnDisplayDisk.Enabled = False
            BtnDisplayFAT.Enabled = False
            BtnEditBootSector.Enabled = False
            BtnEditFAT.Enabled = False
            BtnDisplayDirectory.Enabled = False
            ToolStripSeparatorFAT.Visible = False
            ComboFAT.Visible = False
            BtnWriteFloppyA.Enabled = False
            BtnWriteFloppyB.Enabled = False
            BtnAddFile.Enabled = False
        End If
        BtnWin9xClean.Enabled = False
        BtnClearReservedBytes.Enabled = False
        BtnAddFile.Tag = 0

        MenuDisplayDirectorySubMenuClear()
        FATSubMenuRefresh(Disk, CurrentImageData, FATTablesMatch)

        RefreshSaveButtons()

        If ComboFAT.Visible <> PrevVisible Then
            ToolStripTop.Refresh()
        End If
    End Sub

    Private Sub InitDebugFeatures()
        Dim Separator = New ToolStripSeparator With {
            .Visible = True
        }
        ContextMenuFilters.Items.Add(Separator)

        Dim Item = New ToolStripMenuItem With {
            .Text = "Export Unknown Images on Scan",
            .CheckOnClick = True,
            .Checked = _ExportUnknownImages
        }
        AddHandler Item.CheckStateChanged, AddressOf ExportUnknownImages_CheckStateChanged
        ContextMenuFilters.Items.Add(Item)
    End Sub

    Private Sub InitValidFilters()
        ParseFileTypeFilters()
    End Sub

    Private Function IsWin9xDisk(Disk As Disk) As Boolean
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

    Private Sub ItemScan(Type As ItemScanTypes, Disk As DiskImage.Disk, ImageData As LoadedImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
        ItemScanModified(Disk, ImageData, UpdateFilters, Remove)

        If UpdateFilters Then
            RefreshModifiedCount()
        End If

        If ImageData.Scanned Then
            If Type And ItemScanTypes.Disk Then
                ItemScanDisk(Disk, ImageData, UpdateFilters, Remove)
            End If
            If Type And ItemScanTypes.OEMName Then
                ItemScanOEMName(Disk, ImageData, UpdateFilters, Remove)
                OEMNameFilterUpdate(Disk, ImageData, UpdateFilters, Remove)
            End If
            If Type And ItemScanTypes.DiskType Then
                DiskTypeFilterUpdate(Disk, ImageData, UpdateFilters, Remove)
            End If
            If Type And ItemScanTypes.FreeClusters Then
                ItemScanFreeClusters(Disk, ImageData, UpdateFilters, Remove)
            End If
            If Type And ItemScanTypes.Directory Then
                ItemScanDirectory(Disk, ImageData, UpdateFilters, Remove)
            End If
        End If
    End Sub

    Private Sub ItemSelectionChanged()
        RefreshFileButtons()
        FileInfoUpdate()
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

    Private Function ListViewFilesAddGroup(Directory As DiskImage.IDirectory, Path As String, GroupIndex As Integer) As ListViewGroup
        Dim FileCount As UInteger = Directory.Data.FileCount
        Dim GroupName As String = IIf(Path = "", "(Root)", Path)
        GroupName = GroupName & "  (" & FileCount & IIf(FileCount <> 1, " entries", " entry") _
            & IIf(Directory.Data.HasBootSector, ", Boot Sector", "") _
            & IIf(Directory.Data.HasAdditionalData, ", Additional Data", "") _
            & ")"

        Dim Group = New ListViewGroup(GroupName)
        ListViewFiles.Groups.Add(Group)

        Return Group
    End Function

    Private Function ListViewFilesAddItem(FilePath As String, DirectoryEntry As DirectoryEntry, LFNFileName As String, ParentOffset As UInteger, Index As Integer, Count As Integer, Group As ListViewGroup, ItemIndex As Integer) As ListViewItem
        Dim IsLastEntry = (Index = Count - 1)
        Dim FileData = GetFileDataFromDirectoryEntry(Index, DirectoryEntry, ParentOffset, FilePath, IsLastEntry, LFNFileName)
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
        FileReserved.Width = 0
    End Sub

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

    Private Sub ParseFileTypeFilters()
        Dim Items = System.Enum.GetValues(GetType(FloppyDiskType))
        For Each Item As Integer In Items
            Dim FileExt = GetFileFilterExtByType(Item)
            If FileExt <> "" Then
                If Not _ValidFileExt.Contains(FileExt) Then
                    _ValidFileExt.Add(FileExt)
                End If
            End If
        Next
    End Sub

    Private Function PopulateFilesPanel(ImageData As LoadedImageData, ClearItems As Boolean) As DirectoryScanResponse
        MenuDisplayDirectorySubMenuClear()

        ListViewFiles.BeginUpdate()

        ListViewFiles.ListViewItemSorter = Nothing

        If ClearItems Then
            ListViewFiles.Items.Clear()
            ListViewFiles.Groups.Clear()
        End If
        ListViewFiles.MultiSelect = True

        If BtnDisplayDirectory.DropDownItems.Count > 0 Then
            BtnDisplayDirectory.Text = "Directory"
            MenuDisplayDirectorySubMenuItemAdd("(Root)", 0, 0)
            BtnDisplayDirectory.Tag = Nothing
        Else
            BtnDisplayDirectory.Tag = 0
        End If

        Dim Response = ProcessDirectoryEntries(_Disk.Directory, False)

        If Not ClearItems Then
            ListViewFilesRemoveUnused(Response.ItemCount)
        End If

        ProcessDirectoryScanResponse(Response)

        ListViewAutoSize()

        ListViewFiles.ListViewItemSorter = _lvwColumnSorter

        If ImageData.SortHistory IsNot Nothing Then
            If ImageData.SortHistory.Count > 0 Then
                For Each Sort In ImageData.SortHistory
                    _lvwColumnSorter.Sort(Sort)
                    ListViewFiles.Sort()
                    ListViewFiles.SetSortIcon(_lvwColumnSorter.SortColumn, _lvwColumnSorter.Order)
                Next
                BtnResetSort.Enabled = True
            End If
        End If

        If ClearItems Then
            If ImageData IsNot Nothing AndAlso ImageData.BottomIndex > -1 Then
                If ImageData.BottomIndex < ListViewFiles.Items.Count Then
                    ListViewFiles.EnsureVisible(ImageData.BottomIndex)
                End If
            End If
        End If

        ListViewFiles.EndUpdate()
        ListViewFiles.Refresh()

        ItemSelectionChanged()

        Return Response
    End Function

    Private Sub PopulateHashPanel(Disk As Disk, MD5 As String)
        With ListViewHashes
            .BeginUpdate()
            .Items.Clear()

            If Disk IsNot Nothing Then
                .AddItem("CRC32", Crc32.ComputeChecksum(Disk.Data.Data).ToString("X8"), False)
                .AddItem("MD5", MD5, False)
                .AddItem("SHA-1", SHA1Hash(Disk.Data.Data), False)
            End If

            .EndUpdate()
            .Refresh()
        End With
    End Sub

    Private Function GetNormalizedDataByTrackList(Disk As Disk, TrackList As List(Of FloppyDB.BooterTrack)) As Byte()
        Dim BPB As BiosParameterBlock = BuildBPB(_Disk.Data.Data.Length)

        Dim Data(Disk.Data.Data.Length - 1) As Byte
        Disk.Data.Data.CopyTo(Data, 0)
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

    Private Function GetNormalizedDataByBadSectors(Disk As Disk) As Byte()
        Dim Data(Disk.Data.Data.Length - 1) As Byte
        Disk.Data.Data.CopyTo(Data, 0)
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

    Private Sub PopulateSummary(Disk As Disk, ImageData As LoadedImageData)
        Dim MD5 As String = ""

        If Disk IsNot Nothing Then
            MD5 = MD5Hash(Disk.Data.Data)
        End If

        SetCurrentFileName(ImageData)
        PopulateSummaryPanel(Disk, MD5)
        PopulateHashPanel(Disk, MD5)
        RefreshDiskButtons(Disk, ImageData)
    End Sub

    Private Sub PopulateTitleGroup(TitleData As FloppyDB.FloppyData)
        Dim MAxOffset As Integer = 40
        Dim Offset As Integer = 0
        Dim Value As String
        Dim ForeColor As Color
        Dim Name = TitleData.GetName
        Dim Variation = TitleData.GetVariation
        Dim Compilation = TitleData.GetCompilation
        Dim Publisher = TitleData.GetPublisher
        Dim Version = TitleData.GetVersion
        'If Variation <> "" Then
        ' Name &= " (" & Variation & ")"
        'End If

        With ListViewSummary
            Dim ColumnWidth As Integer = .Columns.Item(1).Width - 5
            Dim MaxWidth = ColumnWidth + MAxOffset

            Dim TitleGroup = .Groups.Add("Title", "Title")

            If Name <> "" Then
                Dim Status = TitleData.GetStatus
                If Status = FloppyDB.FloppyDBStatus.Verified Then
                    ForeColor = Color.Green
                ElseIf Status = FloppyDB.FloppyDBStatus.Modified Then
                    ForeColor = Color.Red
                Else
                    ForeColor = Color.Blue
                End If

                .AddItem(TitleGroup, "Title", Name, ForeColor, True, MaxWidth)
            End If
            If Variation <> "" Then
                .AddItem(TitleGroup, "Variant", Variation, False)
            End If
            If Compilation <> "" Then
                .AddItem(TitleGroup, "Compilation", Compilation, SystemColors.WindowText, True, MaxWidth)
            End If
            If Publisher <> "" Then
                .AddItem(TitleGroup, "Publisher", Publisher, SystemColors.WindowText, True, MaxWidth)
            End If
            Value = TitleData.GetYear
            If Value <> "" Then
                .AddItem(TitleGroup, "Year", Value, False)
            End If
            Value = TitleData.GetOperatingSystem
            If Value <> "" Then
                .AddItem(TitleGroup, "OS", Value, False)
            End If
            Value = TitleData.GetRegion
            If Value <> "" Then
                .AddItem(TitleGroup, "Region", Value, False)
            End If
            Value = TitleData.GetLanguage
            If Value <> "" Then
                .AddItem(TitleGroup, "Language", Value, False)
            End If
            If Version <> "" Then
                .AddItem(TitleGroup, "Version", Version, SystemColors.WindowText, True, MaxWidth)
            End If
            Value = TitleData.GetDisk
            If Value <> "" Then
                .AddItem(TitleGroup, "Disk", Value, False)
            End If

            Dim TextWidth As Integer = ColumnWidth
            For Each Item As ListViewItem In TitleGroup.Items
                Value = Item.SubItems.Item(1).Text
                TextWidth = Math.Max(TextWidth, TextRenderer.MeasureText(Value, .Font).Width)
            Next
            TitleGroup.Tag = TextWidth - ColumnWidth
        End With
    End Sub
    Private Sub PopulateSummaryPanel(Disk As Disk, MD5 As String)
        Dim Value As String
        Dim ForeColor As Color

        With ListViewSummary
            .BeginUpdate()
            .Items.Clear()
            .Groups.Clear()

            If Disk IsNot Nothing Then
                Dim CopyProtection As String = ""
                Dim TitleFound As Boolean = False

                If _TitleDB.TitleCount > 0 Then
                    Dim TitleFindResult = _TitleDB.TitleFind(Disk, MD5)
                    If TitleFindResult.TitleData IsNot Nothing Then
                        TitleFound = True
                        CopyProtection = TitleFindResult.TitleData.GetCopyProtection
                        PopulateTitleGroup(TitleFindResult.TitleData)
                    End If
                End If

                Dim DiskGroup = .Groups.Add("Disk", "Disk")

                If Disk.IsValidImage AndAlso Disk.CheckImageSize <> 0 Then
                    ForeColor = Color.Red
                Else
                    ForeColor = SystemColors.WindowText
                End If
                .AddItem(DiskGroup, "Image Size", Disk.Data.Length.ToString("N0"), ForeColor)

                If Disk.IsValidImage(False) Then
                    Dim DiskTypeString = GetFloppyDiskTypeName(Disk.DiskType)
                    Dim DiskTypeBySize = GetFloppyDiskType(Disk.Data.Length)

                    If Disk.DiskType <> FloppyDiskType.FloppyUnknown Or DiskTypeBySize = FloppyDiskType.FloppyUnknown Then
                        .AddItem(DiskGroup, "Disk Type", DiskTypeString & " Floppy")
                    Else
                        Dim DiskTypeStringBySize = GetFloppyDiskTypeName(DiskTypeBySize)
                        .AddItem(DiskGroup, "Disk Type", DiskTypeStringBySize & " Floppy (Custom Format)")
                    End If
                    If Disk.BPB.IsValid AndAlso Disk.CheckImageSize > 0 AndAlso Disk.DiskType <> FloppyDiskType.FloppyUnknown Then
                        .AddItem(DiskGroup, DiskTypeString & " CRC32", Crc32.ComputeChecksum(Disk.Data.GetBytes(0, Disk.BPB.ReportedImageSize())).ToString("X8"))
                    End If
                End If

                If Disk.IsValidImage Then
                    Dim BadSectors = GetBadSectors(Disk.BPB, Disk.FAT.BadClusters)
                    If CopyProtection.Length = 0 Then
                        CopyProtection = GetCopyProtection(Disk, BadSectors)
                    End If
                End If

                If CopyProtection.Length > 0 Then
                    .AddItem(DiskGroup, "Copy Protection", CopyProtection)
                End If

                If Not Disk.IsValidImage Then
                    .AddItem(DiskGroup, "File System", "Unknown", Color.Red)
                Else
                    Dim OEMNameResponse = _BootStrapDB.CheckOEMName(Disk.BootSector)

                    If OEMNameResponse.NoBootLoader Then
                        If Disk.BootSector.CheckJumpInstruction(False, True) Then
                            .AddItem(DiskGroup, "Bootstrap", "No Boot Loader", Color.Red)
                        Else
                            .AddItem(DiskGroup, "Bootstrap", "Custom Boot Loader", Color.Red)
                        End If
                    ElseIf Not Disk.BootSector.BPB.IsValid Then
                        .AddItem(DiskGroup, "Boot Record", "No BPB", Color.Red)
                    End If

                    If Not Disk.BootSector.BPB.IsValid Then
                        If Not Disk.FATTables.FATsMatch Then
                            .AddItem(DiskGroup, "FAT", "Mismatched", Color.Red)
                        End If
                    End If

                    If Disk.BootSector.BPB.IsValid Then
                        Dim DiskTypeBySize = GetFloppyDiskType(Disk.Data.Length)
                        Dim BPBBySize = BuildBPB(DiskTypeBySize)
                        Dim DoBPBCompare = Disk.DiskType = FloppyDiskType.FloppyUnknown And DiskTypeBySize <> FloppyDiskType.FloppyUnknown

                        Dim BootRecordGroup = .Groups.Add("BootRecord", "Boot Record")

                        If Not OEMNameResponse.Found Then
                            ForeColor = SystemColors.WindowText
                        ElseIf Not OEMNameResponse.Matched Then
                            ForeColor = Color.Red
                        ElseIf OEMNameResponse.Verified Then
                            ForeColor = Color.Green
                        Else
                            ForeColor = Color.Blue
                        End If

                        .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.OEMName), Disk.BootSector.GetOEMNameString.TrimEnd(NULL_CHAR), ForeColor)

                        If DoBPBCompare AndAlso Disk.BootSector.BPB.BytesPerSector <> BPBBySize.BytesPerSector Then
                            ForeColor = Color.Blue
                        Else
                            ForeColor = SystemColors.WindowText
                        End If

                        .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.BytesPerSector), Disk.BootSector.BPB.BytesPerSector, ForeColor)

                        If DoBPBCompare AndAlso Disk.BootSector.BPB.SectorsPerCluster <> BPBBySize.SectorsPerCluster Then
                            ForeColor = Color.Blue
                        Else
                            ForeColor = SystemColors.WindowText
                        End If

                        .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.SectorsPerCluster), Disk.BootSector.BPB.SectorsPerCluster, ForeColor)

                        If DoBPBCompare AndAlso Disk.BootSector.BPB.ReservedSectorCount <> BPBBySize.ReservedSectorCount Then
                            ForeColor = Color.Blue
                        Else
                            ForeColor = SystemColors.WindowText
                        End If

                        .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.ReservedSectorCount), Disk.BootSector.BPB.ReservedSectorCount, ForeColor)

                        If DoBPBCompare AndAlso Disk.BootSector.BPB.NumberOfFATs <> BPBBySize.NumberOfFATs Then
                            ForeColor = Color.Blue
                        Else
                            ForeColor = SystemColors.WindowText
                        End If

                        If Disk.DiskType = FloppyDiskType.FloppyXDF Then
                            Value = "1 + Compatibility Image"
                        Else
                            Value = Disk.BootSector.BPB.NumberOfFATs
                            If Not Disk.FATTables.FATsMatch Then
                                Value &= " (Mismatched)"
                                ForeColor = Color.Red
                            End If
                        End If

                        .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.NumberOfFATs), Value, ForeColor)

                        If DoBPBCompare AndAlso Disk.BootSector.BPB.RootEntryCount <> BPBBySize.RootEntryCount Then
                            ForeColor = Color.Blue
                        Else
                            ForeColor = SystemColors.WindowText
                        End If

                        .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.RootEntryCount), Disk.BootSector.BPB.RootEntryCount, ForeColor)

                        If DoBPBCompare AndAlso Disk.BootSector.BPB.SectorCount <> BPBBySize.SectorCount Then
                            ForeColor = Color.Blue
                        Else
                            ForeColor = SystemColors.WindowText
                        End If

                        .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.SectorCountSmall), Disk.BootSector.BPB.SectorCount, ForeColor)


                        If DoBPBCompare AndAlso Disk.BootSector.BPB.MediaDescriptor <> BPBBySize.MediaDescriptor Then
                            ForeColor = Color.Blue
                        Else
                            ForeColor = SystemColors.WindowText
                        End If

                        Value = Disk.BootSector.BPB.MediaDescriptor.ToString("X2") & " Hex"

                        If Disk.BootSector.BPB.IsValid Then
                            If Not Disk.BPB.HasValidMediaDescriptor Then
                                Value &= " (Invalid)"
                                ForeColor = Color.Red
                            ElseIf Disk.DiskType = FloppyDiskType.FloppyXDF AndAlso Disk.FAT.MediaDescriptor = &HF9 Then
                                'Do Nothing - This is normal for XDF
                                'ElseIf Disk.DiskType <> FloppyDiskType.FloppyUnknown AndAlso Disk.BootSector.BPB.MediaDescriptor <> GetFloppyDiskMediaDescriptor(Disk.DiskType) Then
                            ElseIf Disk.BootSector.BPB.MediaDescriptor <> GetFloppyDiskMediaDescriptor(Disk.DiskType) Then
                                Value &= " (Mismatched)"
                                ForeColor = Color.Red
                            ElseIf Disk.FAT.MediaDescriptor <> Disk.BootSector.BPB.MediaDescriptor Then
                                Value &= " (Mismatched)"
                                ForeColor = Color.Red
                            End If
                        End If

                        .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.MediaDescriptor), Value, ForeColor)

                        If DoBPBCompare AndAlso Disk.BootSector.BPB.SectorsPerFAT <> BPBBySize.SectorsPerFAT Then
                            ForeColor = Color.Blue
                        Else
                            ForeColor = SystemColors.WindowText
                        End If

                        .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.SectorsPerFAT), Disk.BootSector.BPB.SectorsPerFAT, ForeColor)

                        If DoBPBCompare AndAlso Disk.BootSector.BPB.SectorsPerTrack <> BPBBySize.SectorsPerTrack Then
                            ForeColor = Color.Blue
                        Else
                            ForeColor = SystemColors.WindowText
                        End If

                        .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.SectorsPerTrack), Disk.BootSector.BPB.SectorsPerTrack, ForeColor)

                        If DoBPBCompare AndAlso Disk.BootSector.BPB.NumberOfHeads <> BPBBySize.NumberOfHeads Then
                            ForeColor = Color.Blue
                        Else
                            ForeColor = SystemColors.WindowText
                        End If

                        .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.NumberOfHeads), Disk.BootSector.BPB.NumberOfHeads, ForeColor)

                        If Disk.BootSector.BPB.HiddenSectors > 0 Then
                            .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.HiddenSectors), Disk.BootSector.BPB.HiddenSectors)
                        End If

                        Dim BootStrapStart = Disk.BootSector.GetBootStrapOffset

                        If BootStrapStart >= BootSectorOffsets.BootStrapCode Then
                            If Disk.BootSector.DriveNumber > 0 Then
                                .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.DriveNumber), Disk.BootSector.DriveNumber)
                            End If

                            If Disk.BootSector.HasValidExtendedBootSignature Then
                                .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.VolumeSerialNumber), Disk.BootSector.VolumeSerialNumber.ToString("X8").Insert(4, "-"))
                                If Disk.BootSector.ExtendedBootSignature = BootSector.ValidExtendedBootSignature(1) Then
                                    .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.VolumeLabel), Disk.BootSector.GetVolumeLabelString.TrimEnd(NULL_CHAR))
                                    .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.FileSystemType), Disk.BootSector.GetFileSystemTypeString)
                                End If
                            End If
                        End If

                        If Not Disk.BootSector.HasValidBootStrapSignature Then
                            .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.BootStrapSignature), Disk.BootSector.BootStrapSignature.ToString("X4"))
                        End If

                        If My.Settings.Debug Then
                            If Not Disk.BootSector.CheckJumpInstruction(True, True) Then
                                ForeColor = Color.Red
                            Else
                                ForeColor = SystemColors.WindowText
                            End If
                            .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.JmpBoot), BitConverter.ToString(Disk.BootSector.JmpBoot), ForeColor)
                        End If
                    End If

                    Dim FileSystemGroup = .Groups.Add("FileSystem", "File System")

                    If _Disk.FAT.HasMediaDescriptor Then
                        Value = Disk.FAT.MediaDescriptor.ToString("X2") & " Hex"
                        ForeColor = SystemColors.WindowText
                        Dim Visible As Boolean = False
                        If Not Disk.BootSector.BPB.IsValid Then
                            Visible = True
                        ElseIf Disk.FAT.MediaDescriptor <> Disk.BootSector.BPB.MediaDescriptor Then
                            Visible = True
                        End If
                        If Not Disk.FAT.HasValidMediaDescriptor Then
                            Value &= " (Invalid)"
                            ForeColor = Color.Red
                            Visible = True
                        ElseIf Disk.DiskType = FloppyDiskType.FloppyXDF AndAlso Disk.FAT.MediaDescriptor = &HF9 Then
                            Visible = False
                        ElseIf Disk.DiskType <> FloppyDiskType.FloppyUnknown AndAlso Disk.FAT.MediaDescriptor <> GetFloppyDiskMediaDescriptor(Disk.DiskType) Then
                            Value &= " (Mismatched)"
                            ForeColor = Color.Red
                            Visible = True
                        End If

                        If Visible Then
                            .AddItem(FileSystemGroup, "Media Descriptor", Value, ForeColor)
                        End If
                    End If

                    Dim fsi = GetFileSystemInfo(Disk)

                    If fsi.VolumeLabel IsNot Nothing Then
                        .AddItem(FileSystemGroup, "Volume Label", fsi.VolumeLabel.GetVolumeName.TrimEnd(NULL_CHAR))
                        Dim VolumeDate = fsi.VolumeLabel.GetLastWriteDate
                        .AddItem(FileSystemGroup, "Volume Date", ExpandedDateToString(VolumeDate, True, False, False, False))
                    End If

                    .AddItem(FileSystemGroup, "Total Space", Format(Disk.SectorToBytes(Disk.BPB.DataRegionSize), "N0") & " bytes")
                    .AddItem(FileSystemGroup, "Free Space", Format(Disk.FAT.GetFreeSpace(Disk.BPB.BytesPerCluster), "N0") & " bytes")

                    If Disk.FAT.BadClusters.Count > 0 Then
                        Dim SectorCount = Disk.FAT.BadClusters.Count * Disk.BPB.SectorsPerCluster
                        Value = Format(Disk.FAT.BadClusters.Count * Disk.BPB.BytesPerCluster, "N0") & " bytes  (" & SectorCount & ")"
                        .AddItem(FileSystemGroup, "Bad Sectors", Value, Color.Red)
                    End If

                    Dim LostClusters = Disk.FAT.GetLostClusterCount
                    If LostClusters > 0 Then
                        Value = Format(LostClusters * Disk.BPB.BytesPerCluster, "N0") & " bytes  (" & LostClusters & ")"
                        .AddItem(FileSystemGroup, "Lost Clusters", Value, Color.Red)
                    End If

                    If Disk.FAT.ReservedClusters > 0 Then
                        Value = Format(Disk.FAT.ReservedClusters * Disk.BPB.BytesPerCluster, "N0") & " bytes  (" & Disk.FAT.ReservedClusters & ")"
                        .AddItem(FileSystemGroup, "Reserved Clusters", Value)
                    End If

                    If fsi.OldestFileDate IsNot Nothing Then
                        .AddItem(FileSystemGroup, "Oldest Date", fsi.OldestFileDate.Value.ToString("yyyy-MM-dd  hh:mm tt"))
                    End If

                    If fsi.NewestFileDate IsNot Nothing Then
                        .AddItem(FileSystemGroup, "Newest Date", fsi.NewestFileDate.Value.ToString("yyyy-MM-dd  hh:mm tt"))
                    End If

                    Dim BootStrapGroup = .Groups.Add("Bootstrap", "Bootstrap")

                    If Not OEMNameResponse.NoBootLoader Then
                        Dim BootStrapCRC32 = Crc32.ComputeChecksum(Disk.BootSector.BootStrapCode)
                        .AddItem(BootStrapGroup, "Bootstrap CRC32", BootStrapCRC32.ToString("X8"))
                    End If

                    If OEMNameResponse.Found Then
                        If OEMNameResponse.Data.Language.Length > 0 Then
                            .AddItem(BootStrapGroup, "Language", OEMNameResponse.Data.Language)
                        End If

                        Dim OEMName = OEMNameResponse.MatchedOEMName

                        If OEMName Is Nothing And OEMNameResponse.Data.OEMNames.Count = 1 Then
                            OEMName = OEMNameResponse.Data.OEMNames(0)
                        End If

                        If OEMName IsNot Nothing Then
                            If OEMName.Company <> "" Then
                                .AddItem(BootStrapGroup, "Company", OEMName.Company)
                            End If
                            If OEMName.Description <> "" Then
                                .AddItem(BootStrapGroup, "Description", OEMName.Description)
                            End If
                            If OEMName.Note <> "" Then
                                .AddItem(BootStrapGroup, "Note", OEMName.Note, Color.Blue)
                            End If
                        End If

                        If Disk.BootSector.BPB.IsValid Then
                            If Not OEMNameResponse.Data.ExactMatch Then
                                For Each OEMName In OEMNameResponse.Data.OEMNames
                                    If OEMName.Name.Length > 0 AndAlso OEMName.Suggestion AndAlso OEMName IsNot OEMNameResponse.MatchedOEMName Then
                                        If OEMName.Verified Then
                                            ForeColor = Color.Green
                                        Else
                                            ForeColor = SystemColors.WindowText
                                        End If
                                        .AddItem(BootStrapGroup, "Alternative OEM Name", OEMName.GetNameAsString, ForeColor)
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
                .HideSelection = False
                .TabStop = True
                btnRetry.Visible = False
            Else
                Dim DiskGroup = .Groups.Add("Disk", "Disk")
                Dim Item = New ListViewItem("  Error Loading File", DiskGroup) With {
                    .ForeColor = Color.Red
                }
                .Items.Add(Item)
                .HideSelection = True
                .TabStop = False
                btnRetry.Visible = True
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
            Dim LFNFileName As String = ""

            For Counter = 0 To DirectoryEntryCount - 1
                Dim DirectoryEntry = Directory.GetFile(Counter)

                If Not DirectoryEntry.IsLink Then
                    If DirectoryEntry.IsLFN Then
                        LFNFileName = DirectoryEntry.GetLFNFileName & LFNFileName
                    Else
                        If Not ScanOnly Then
                            Dim Item = ListViewFilesAddItem(Path, DirectoryEntry, LFNFileName, Offset, Counter, DirectoryEntryCount, Group, ItemIndex)
                            ItemIndex += 1
                        End If

                        Response.ProcessDirectoryEntry(DirectoryEntry, LFNFileName)

                        LFNFileName = ""
                    End If

                    If DirectoryEntry.IsDirectory And DirectoryEntry.SubDirectory IsNot Nothing Then
                        If DirectoryEntry.SubDirectory.Data.EntryCount > 0 Then
                            Dim NewPath = DirectoryEntry.GetFullFileName
                            If Path <> "" Then
                                NewPath = Path & "\" & NewPath
                            End If

                            If Not ScanOnly Then
                                MenuDisplayDirectorySubMenuItemAdd(NewPath, DirectoryEntry.Offset, -1)
                            End If

                            Dim SubResponse = ProcessDirectoryEntries(DirectoryEntry.SubDirectory, DirectoryEntry.Offset, NewPath, ScanOnly, GroupIndex, ItemIndex)
                            Response.Combine(SubResponse)
                        End If
                    End If
                End If
            Next
        End If

        Return Response
    End Function

    Private Sub ProcessDirectoryScanResponse(Response As DirectoryScanResponse)
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

        If Response.HasReserved Then
            FileReserved.Width = 60
        Else
            FileReserved.Width = 0
        End If

        If Response.HasFATChainingErrors Then
            FileClusterError.Width = 30
            RefreshClusterErrors()
        Else
            FileClusterError.Width = 0
        End If

        BtnWin9xClean.Enabled = Response.HasValidCreated Or Response.HasValidLastAccessed Or _Disk.BootSector.IsWin9xOEMName
        BtnClearReservedBytes.Enabled = Response.HasReserved
    End Sub

    Private Sub ProcessFileDrop(File As String)
        Dim Files(0) As String
        Files(0) = File

        ProcessFileDrop(Files)
    End Sub

    Private Sub ProcessFileDrop(Files() As String)
        Cursor.Current = Cursors.WaitCursor
        Dim T = Stopwatch.StartNew

        If ImageFilters.FiltersApplied Then
            FiltersClear(False)
            ImageFilters.UpdateAllMenuItems()
            ImageCountUpdate()
        End If

        ComboImages.BeginUpdate()

        LoadedImageData.StringOffset = 0

        Dim ImageLoadForm As New ImageLoadForm(Me, Files, _LoadedFileNames, _ValidFileExt, _ArchiveFilterExt)
        ImageLoadForm.ShowDialog(Me)

        LoadedImageData.StringOffset = GetPathOffset()

        If ImageLoadForm.SelectedImageData IsNot Nothing Then
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

    Private Sub RefreshCurrentState()
        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem

        ComboImagesRefreshCurrentItemText()
        RefreshDiskButtons(_Disk, CurrentImageData)
        RefreshSaveButtons()
    End Sub

    Private Sub RefreshFixImageSubMenu(Disk As Disk)
        Dim EnableSubMenu As Boolean
        Dim EnableConvertSectors As Boolean = False
        Dim EnableConvertSingleSided As Boolean = False
        Dim DiskTypeBySize = GetFloppyDiskType(Disk.Data.Length)

        If Disk.DiskType = FloppyDiskType.Floppy160 And DiskTypeBySize = FloppyDiskType.Floppy180 Then
            EnableConvertSectors = True
        ElseIf Disk.DiskType = FloppyDiskType.Floppy320 And DiskTypeBySize = FloppyDiskType.Floppy360 Then
            EnableConvertSectors = True
        End If

        If Disk.DiskType = FloppyDiskType.Floppy160 And DiskTypeBySize = FloppyDiskType.Floppy320 Then
            EnableConvertSingleSided = True
        ElseIf Disk.DiskType = FloppyDiskType.Floppy180 And DiskTypeBySize = FloppyDiskType.Floppy360 Then
            EnableConvertSingleSided = True
        End If

        EnableSubMenu = EnableConvertSectors Or EnableConvertSingleSided

        If EnableSubMenu Then
            BtnFixImageSize.Visible = False
            SubMenuFixImageSize.Visible = True
            SubMenuFixImageSize.Enabled = True
            BtnConvertSectors.Enabled = EnableConvertSectors
            BtnConvertSectors.Visible = EnableConvertSectors
            BtnConvertSingleSided.Enabled = EnableConvertSingleSided
            BtnConvertSingleSided.Visible = EnableConvertSingleSided
        Else
            BtnFixImageSize.Visible = True
            SubMenuFixImageSize.Visible = False
            SubMenuFixImageSize.Enabled = False
            BtnConvertSectors.Enabled = False
            BtnConvertSingleSided.Enabled = False
        End If
    End Sub

    Private Sub RefreshDiskButtons(Disk As Disk, ImageData As LoadedImageData)
        If Disk IsNot Nothing AndAlso Disk.IsValidImage Then
            BtnDisplayClusters.Enabled = Disk.FAT.HasFreeClusters(FAT12.FreeClusterEmum.WithData)
            BtnDisplayBadSectors.Enabled = Disk.FAT.BadClusters.Count > 0
            BtnDisplayLostClusters.Enabled = Disk.FAT.GetLostClusterCount > 0
            BtnFixImageSize.Enabled = Disk.CheckImageSize <> 0
            If Disk.Directory.Data.HasBootSector Then
                Dim BootSectorBytes = Disk.Data.GetBytes(Disk.Directory.Data.BootSectorOffset, BootSector.BOOT_SECTOR_SIZE)
                BtnRestoreBootSector.Enabled = Not BootSectorBytes.CompareTo(Disk.BootSector.Data)
            Else
                BtnRestoreBootSector.Enabled = False
            End If
            BtnRemoveBootSector.Enabled = Disk.Directory.Data.HasBootSector

            RefreshFixImageSubMenu(Disk)
        Else
            BtnDisplayClusters.Enabled = False
            BtnDisplayBadSectors.Enabled = False
            BtnDisplayLostClusters.Enabled = False
            BtnFixImageSize.Enabled = False
            BtnConvertSectors.Enabled = False
            BtnConvertSingleSided.Enabled = False
            SubMenuFixImageSize.Enabled = False
            SubMenuFixImageSize.Visible = False
            BtnRestoreBootSector.Enabled = False
            BtnRemoveBootSector.Enabled = False
        End If

        BtnResizeImage.Enabled = BtnFixImageSize.Enabled

        If Disk IsNot Nothing Then
            BtnRevert.Enabled = Disk.Data.Modified
            BtnUndo.Enabled = Disk.Data.UndoEnabled
            BtnRedo.Enabled = Disk.Data.RedoEnabled
            ToolStripStatusModified.Visible = Disk.Data.Modified
            ToolStripStatusReadOnly.Visible = ImageData.ReadOnly
            ToolStripStatusReadOnly.Text = IIf(ImageData.Compressed, "Compressed", "Read Only")
        Else
            BtnRevert.Enabled = False
            BtnUndo.Enabled = False
            BtnRedo.Enabled = False
            ToolStripStatusModified.Visible = False
            ToolStripStatusReadOnly.Visible = False
        End If

        ToolStripBtnUndo.Enabled = BtnUndo.Enabled
        ToolStripBtnRedo.Enabled = BtnRedo.Enabled
    End Sub

    Private Sub RefreshFileButtons()
        Dim Stats As DirectoryStats
        Dim DirectoryOffset As Integer = -1
        Dim Caption As String

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
            BtnFileMenuDeleteFile.Text = "&Delete File"

            BtnFileMenuUnDeleteFile.Visible = False
            BtnFileMenuUnDeleteFile.Enabled = False
            BtnFileMenuUnDeleteFile.Text = "&Undelete File"

            BtnFileMenuDeleteFileWithFill.Visible = False
            BtnFileMenuDeleteFileWithFill.Enabled = False
            BtnFileMenuDeleteFileWithFill.Text = "&Delete File and Clear Sectors"

        ElseIf ListViewFiles.SelectedItems.Count = 1 Then
            Dim FileData As FileData = ListViewFiles.SelectedItems(0).Tag
            DirectoryOffset = FileData.ParentOffset
            Stats = DirectoryEntryGetStats(FileData.DirectoryEntry)

            BtnExportFile.Text = "&Export File"
            BtnExportFile.Enabled = Stats.CanExport

            BtnReplaceFile.Enabled = Stats.IsValidFile And Not Stats.IsDeleted
            BtnFileProperties.Enabled = True
            BtnFileMenuViewCrosslinked.Visible = FileData.DirectoryEntry.IsCrossLinked
            BtnFileMenuFixSize.Enabled = FileData.DirectoryEntry.HasIncorrectFileSize

            Caption = "View "
            If Stats.IsDeleted Then
                Caption &= "Deleted "
            End If
            Caption &= "&File as Text"
            BtnFileMenuViewFileText.Text = Caption
            BtnFileMenuViewFileText.Visible = Stats.IsValidFile 'And Not Stats.IsDeleted
            BtnFileMenuViewFileText.Enabled = Stats.FileSize > 0

            If Stats.IsDeleted Then
                BtnFileMenuRemoveDeletedFile.Visible = True
                BtnFileMenuRemoveDeletedFile.Enabled = True

                BtnFileMenuDeleteFile.Visible = False
                BtnFileMenuDeleteFile.Enabled = False

                BtnFileMenuUnDeleteFile.Visible = True
                BtnFileMenuUnDeleteFile.Enabled = Stats.CanUndelete

                BtnFileMenuDeleteFileWithFill.Visible = False
                BtnFileMenuDeleteFileWithFill.Enabled = False
            Else
                BtnFileMenuRemoveDeletedFile.Visible = False
                BtnFileMenuRemoveDeletedFile.Enabled = False

                BtnFileMenuDeleteFile.Visible = True
                BtnFileMenuDeleteFile.Enabled = Stats.CanDelete
                BtnFileMenuDeleteFile.Text = "&Delete File"

                BtnFileMenuUnDeleteFile.Visible = False
                BtnFileMenuUnDeleteFile.Enabled = False

                BtnFileMenuDeleteFileWithFill.Visible = Stats.CanDeleteWithFill
                BtnFileMenuDeleteFileWithFill.Enabled = Stats.CanDeleteWithFill
                BtnFileMenuDeleteFileWithFill.Text = "&Delete File and Clear Sectors"
            End If

            If Stats.IsValidFile Or Stats.IsValidDirectory Then
                If Stats.IsDeleted Then
                    BtnDisplayFile.Text = "Deleted &File:  " & Stats.FullFileName
                Else
                    BtnDisplayFile.Text = "&File:  " & Stats.FullFileName
                End If
                BtnDisplayFile.Tag = FileData.DirectoryEntry.Offset
                BtnDisplayFile.Visible = Not Stats.IsDirectory And Stats.FileSize > 0

                Caption = "&View "
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
            Dim FileData As FileData
            Dim ExportEnabled As Boolean = False
            Dim DeleteEnabled As Boolean = False
            FileData = ListViewFiles.SelectedItems(0).Tag
            DirectoryOffset = FileData.ParentOffset
            Dim ParentOffset As UInteger = FileData.ParentOffset

            For Each Item As ListViewItem In ListViewFiles.SelectedItems
                FileData = Item.Tag
                Stats = DirectoryEntryGetStats(FileData.DirectoryEntry)
                If Stats.CanExport Then
                    ExportEnabled = True
                End If
                If Not Stats.IsDeleted And Stats.CanDelete Then
                    DeleteEnabled = True
                End If
                If FileData.ParentOffset <> ParentOffset Then
                    DirectoryOffset = -1
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
            BtnFileMenuDeleteFile.Enabled = DeleteEnabled
            BtnFileMenuDeleteFile.Text = "&Delete Selected Files"

            BtnFileMenuUnDeleteFile.Visible = False
            BtnFileMenuUnDeleteFile.Enabled = False

            BtnFileMenuDeleteFileWithFill.Visible = True
            BtnFileMenuDeleteFileWithFill.Enabled = DeleteEnabled
            BtnFileMenuDeleteFileWithFill.Text = "&Delete Selected Files and Clear Sectors"
        End If

        BtnFileMenuExportFile.Text = BtnExportFile.Text
        BtnFileMenuExportFile.Enabled = BtnExportFile.Enabled
        ToolStripBtnExportFile.Enabled = BtnExportFile.Enabled
        BtnFileMenuReplaceFile.Enabled = BtnReplaceFile.Enabled
        BtnFileMenuFileProperties.Enabled = BtnFileProperties.Enabled
        ToolStripBtnFileProperties.Enabled = BtnFileProperties.Enabled
        ToolStripBtnViewFile.Text = BtnFileMenuViewFile.Text
        ToolStripBtnViewFile.Enabled = BtnFileMenuViewFile.Enabled
        ToolStripBtnViewFileText.Text = BtnFileMenuViewFileText.Text
        ToolStripBtnViewFileText.Enabled = BtnFileMenuViewFileText.Enabled

        If DirectoryOffset = -1 Then
            BtnFileMenuViewDirectory.Visible = False
            BtnFileMenuViewDirectory.Enabled = False
            FileMenuSeparatorDirectory.Visible = False
            BtnFileMenuAddFile.Enabled = False
        Else
            If DirectoryOffset = 0 Then
                BtnFileMenuViewDirectory.Visible = True
                BtnFileMenuViewDirectory.Text = "View Root D&irectory"
                BtnFileMenuViewDirectory.Enabled = True
            Else
                BtnFileMenuViewDirectory.Visible = True
                BtnFileMenuViewDirectory.Text = "View Parent D&irectory"
                BtnFileMenuViewDirectory.Enabled = True
            End If
            BtnFileMenuViewDirectory.Tag = DirectoryOffset
            FileMenuSeparatorDirectory.Visible = True
            BtnFileMenuAddFile.Enabled = True
            BtnFileMenuAddFile.Tag = DirectoryOffset
        End If
    End Sub

    Private Sub RefreshFilterButtons(Enabled As Boolean)
        BtnClearFilters.Enabled = Enabled
        If Enabled Then
            MainMenuFilters.BackColor = Color.LightGreen
        Else
            MainMenuFilters.BackColor = SystemColors.Control
        End If
    End Sub

    Private Sub RefreshModifiedCount()
        Dim Count As Integer = ImageFilters.FilterCounts(FilterTypes.ModifiedFiles).Total

        ToolStripModified.Text = $"{Count} {"Image".Pluralize(Count)} Modified"
        ToolStripModified.Visible = (Count > 0)
        BtnSaveAll.Enabled = (Count > 0)
        ToolStripBtnSaveAll.Enabled = BtnSaveAll.Enabled
    End Sub

    Private Sub RefreshSaveButtons()
        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem

        If CurrentImageData Is Nothing Then
            BtnSave.Enabled = False
            BtnSaveAs.Enabled = False
            BtnExportDebug.Enabled = False
        Else
            Dim Modified = CurrentImageData.Filter(FilterTypes.ModifiedFiles)
            BtnSave.Enabled = Modified And Not CurrentImageData.ReadOnly
            BtnSaveAs.Enabled = Modified
            'BtnExportDebug.Enabled = (CurrentImageData.Modified Or CurrentImageData.SessionModifications.Count > 0)
            BtnExportDebug.Enabled = False
        End If
        ToolStripBtnSave.Enabled = BtnSave.Enabled
        ToolStripBtnSaveAs.Enabled = BtnSaveAs.Enabled
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

    Private Sub ReloadCurrentImage(DoItemScan As Boolean)
        If _CurrentImageData IsNot Nothing Then
            _CurrentImageData.BottomIndex = ListViewFiles.GetBottomIndex
            _CurrentImageData.SortHistory = _lvwColumnSorter.SortHistory
        End If
        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem
        _CurrentImageData = CurrentImageData
        _Disk = DiskImageLoad(CurrentImageData)
        If _Disk IsNot Nothing Then
            CurrentImageData.Modifications = _Disk.Data.Changes
        End If
        ClearSort(False)
        DiskImageProcess(DoItemScan, True)
    End Sub

    Private Sub RemoveDeletedFile(Item As ListViewItem)
        Dim FileData As FileData = Item.Tag

        If Not FileData.DirectoryEntry.IsDeleted Then
            Exit Sub
        End If

        FileData.DirectoryEntry.BatchEditMode = True

        FileData.DirectoryEntry.Clear(FileData.IsLastEntry)
        If FileData.IsLastEntry And FileData.Index > 0 Then
            Dim Offset = FileData.DirectoryEntry.Offset
            For Counter = FileData.Index - 1 To 0 Step -1
                Offset -= DirectoryEntry.DIRECTORY_ENTRY_SIZE
                Dim PrevEntry = _Disk.GetDirectoryEntryByOffset(Offset)
                If PrevEntry.IsLFN Then
                    PrevEntry.Clear(True)
                Else
                    Exit For
                End If
            Next
        End If

        FileData.DirectoryEntry.BatchEditMode = False

        'FilePropertiesRefresh(Item, False, False)
        DiskImageRefresh()
    End Sub

    Private Sub ResetAll()
        _Disk = Nothing
        Me.Text = GetWindowCaption()
        ImageFilters.FiltersApplied = False
        _CheckAll = False
        _LoadedFileNames.Clear()
        _ScanRun = False

        btnCreateBackup.Checked = My.Settings.CreateBackups

        RefreshDiskButtons(Nothing, Nothing)

        ToolStripFileName.Visible = False
        ToolStripModified.Visible = False
        ToolStripFileCount.Visible = False
        ToolStripFileSector.Visible = False
        ToolStripFileTrack.Visible = False

        BtnSaveAll.Enabled = False
        ToolStripBtnSaveAll.Enabled = BtnSaveAll.Enabled
        btnRetry.Visible = False

        ListViewSummary.Items.Clear()
        ListViewHashes.Items.Clear()

        ComboImagesReset()
        ListViewFilesReset()

        RefreshFileButtons()
        SetImagesLoaded(False)
        FiltersReset()
        InitButtonState(Nothing, Nothing)
    End Sub

    Private Sub RevertChanges()
        If _Disk.Data.Modified Then
            Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem
            CurrentImageData.Modifications.Clear()
            ReloadCurrentImage(True)
        End If
    End Sub

    Private Sub SaveAll()
        _SuppressEvent = True
        For Index = 0 To ComboImages.Items.Count - 1
            Dim NewFilePath As String = ""
            Dim DoSave As Boolean = True
            Dim ImageData As LoadedImageData = ComboImages.Items(Index)
            If ImageData.Filter(FilterTypes.ModifiedFiles) Then
                If ImageData.ReadOnly Then
                    If MsgBoxNewFileName(ImageData.FileName) = MsgBoxResult.Ok Then
                        NewFilePath = GetNewFilePath(ImageData)
                        DoSave = (NewFilePath <> "")
                    Else
                        DoSave = False
                    End If
                End If
                If DoSave Then
                    Dim Result = DiskImageSave(ImageData, NewFilePath)
                    If Result Then
                        If ImageData.ReadOnly Then
                            SetNewFilePath(ImageData, NewFilePath)
                        End If
                        If ImageData Is ComboImages.SelectedItem Then
                            SetCurrentFileName(ImageData)
                            ListViewFilesClearModifiedFlag()
                            RefreshCurrentState()
                        End If
                    End If
                End If
            End If
        Next Index
        _SuppressEvent = False

        ImageFilters.UpdateMenuItem(FilterTypes.ModifiedFiles)
        RefreshModifiedCount()
    End Sub

    Private Sub SaveCurrent(NewFileName As Boolean)
        Dim NewFilePath As String = ""

        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem

        If NewFileName Then
            NewFilePath = GetNewFilePath(CurrentImageData)
            If NewFilePath = "" Then
                Exit Sub
            End If
        End If


        Dim Result = DiskImageSave(CurrentImageData, NewFilePath)
        If Result Then
            ImageFilters.UpdateMenuItem(FilterTypes.ModifiedFiles)
            RefreshModifiedCount()

            If NewFileName Then
                SetNewFilePath(CurrentImageData, NewFilePath)
                SetCurrentFileName(CurrentImageData)
            End If

            ListViewFilesClearModifiedFlag()
            RefreshCurrentState()
        End If
    End Sub
    Private Sub SetCurrentFileName(ImageData As LoadedImageData)
        Dim FileName = ImageData.FileName

        Me.Text = $"{GetWindowCaption()} - {FileName}"

        ToolStripFileName.Text = FileName
        ToolStripFileName.Visible = True
    End Sub

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
        BtnWin9xCleanBatch.Enabled = Value
        If Value Then
            BtnCompare.Enabled = ComboImages.Items.Count > 1
        Else
            BtnCompare.Enabled = False
        End If
    End Sub

    Private Sub SetNewFilePath(CurrentImageData As LoadedImageData, NewFilePath As String)
        If CurrentImageData.SourceFile <> NewFilePath Then
            _LoadedFileNames.Remove(CurrentImageData.DisplayPath)

            CurrentImageData.SourceFile = NewFilePath
            CurrentImageData.Compressed = False
            CurrentImageData.CompressedFile = ""
            CurrentImageData.ReadOnly = IsFileReadOnly(NewFilePath)

            If _LoadedFileNames.ContainsKey(CurrentImageData.DisplayPath) Then
                FileClose(_LoadedFileNames.Item(CurrentImageData.DisplayPath))
            End If

            _LoadedFileNames.Add(CurrentImageData.DisplayPath, CurrentImageData)

            ComboImagesRefreshPaths()
        End If
    End Sub

    Private Sub SubFilterDiskTypePopulateUnfiltered()
        _SubFilterDiskType.Clear()
        For Each ImageData As LoadedImageData In ComboImages.Items
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
        For Each ImageData As LoadedImageData In ComboImages.Items
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

        TxtSearch.Text = ""
        _SubFilterOEMName.ClearFilter()
        _SubFilterDiskType.ClearFilter()

        _SuppressEvent = False
    End Sub

    Private Sub SubFiltersInitialize()
        _SubFilterOEMName = New ComboFilter(ComboOEMName)
        _SubFilterDiskType = New ComboFilter(ComboDiskType)
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
        TxtSearch.Text = ""
        _SubFilterOEMName.Clear()
        _SubFilterDiskType.Clear()

        ComboOEMName.Visible = False
        ToolStripOEMName.Visible = False

        ComboDiskType.Visible = False
        ToolStripDiskType.Visible = False
    End Sub

    Private Sub UndeleteFile(Item As ListViewItem)
        Dim FileData As FileData = Item.Tag

        If Not DirectoryEntryCanUndelete(FileData.DirectoryEntry) Then
            Exit Sub
        End If

        Dim UndeleteForm As New UndeleteForm(FileData.DirectoryEntry.GetFullFileName)

        UndeleteForm.ShowDialog()

        If UndeleteForm.DialogResult = DialogResult.OK Then
            Dim FirstChar = UndeleteForm.FirstChar
            If FirstChar > 0 Then
                FileData.DirectoryEntry.Restore(FirstChar)

                DiskImageRefresh()
            End If
        End If
    End Sub

    Private Sub Win9xCleanBatch()
        Dim Msg = $"This will restore the OEM Name and remove any Creation and Last Accessed Dates from all unverified loaded images.{vbCrLf}{vbCrLf}Do you wish to proceed??"

        If Not MsgBoxQuestion(Msg) Then
            Exit Sub
        End If

        Me.UseWaitCursor = True
        Dim T = Stopwatch.StartNew

        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem
        Dim ItemScanForm As New ItemScanForm(Me, ComboImages.Items, CurrentImageData, _Disk, False, ScanType.ScanTypeWin9xClean)
        ItemScanForm.ShowDialog()

        ImageFilters.UpdateAllMenuItems()

        RefreshModifiedCount()
        If _ScanRun Then
            SubFiltersPopulate()
        End If

        Dim UpdateCount As Integer = 0
        For Index = 0 To ComboImages.Items.Count - 1
            Dim ImageData As LoadedImageData = ComboImages.Items(Index)
            If ImageData.BatchUpdated Then
                If ImageData Is CurrentImageData Then
                    DiskImageRefresh()
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

    Private Sub Win9xCleanCurrent()
        If _TitleDB.IsVerifiedImage(_Disk) Then
            If Not MsgBoxQuestion("This is a verified image.  Are you sure you wish to remove any windows modifications from this image?") Then
                Exit Sub
            End If
        End If

        Dim Result = Win9xClean(_Disk, False)

        If Result Then
            DiskImageRefresh()
        End If
    End Sub
    Private Structure DirectoryStats
        Dim CanDelete As Boolean
        Dim CanDeleteWithFill As Boolean
        Dim CanExport As Boolean
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

    Private Sub BtnAddFile_Click(sender As Object, e As EventArgs) Handles BtnAddFile.Click, BtnFileMenuAddFile.Click
        ContextMenuEdit.Close()
        If sender.Tag IsNot Nothing Then
            FileAdd(sender.tag)
        End If
    End Sub

    Private Sub BtnClearFilters_Click(sender As Object, e As EventArgs) Handles BtnClearFilters.Click
        If ImageFilters.FiltersApplied Then
            FiltersClear(False)
            ImageFilters.UpdateAllMenuItems()
            ImageCountUpdate()
            ContextMenuFilters.Invalidate()
        End If
    End Sub

    Private Sub BtnClearReservedBytes_Click(sender As Object, e As EventArgs) Handles BtnClearReservedBytes.Click
        ClearReservedBytes()
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click, ToolStripBtnClose.Click
        CloseCurrent()
    End Sub

    Private Sub BtnCloseAll_Click(sender As Object, e As EventArgs) Handles BtnCloseAll.Click, ToolStripBtnCloseAll.Click
        If MsgBox("Are you sure you wish to close all open files?", MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
            CloseAll()
        End If
    End Sub

    Private Sub BtnCompare_Click(sender As Object, e As EventArgs) Handles BtnCompare.Click
        CompareImages()
    End Sub

    Private Sub BtnConvertSectors_Click(sender As Object, e As EventArgs) Handles BtnConvertSectors.Click
        ConvertImageSectors()
    End Sub

    Private Sub BtnConvertSingleSided_Click(sender As Object, e As EventArgs) Handles BtnConvertSingleSided.Click
        ConvertImageSingleSided()
    End Sub

    Private Sub BtnCreateBackup_Click(sender As Object, e As EventArgs) Handles btnCreateBackup.Click
        My.Settings.CreateBackups = btnCreateBackup.Checked
    End Sub

    Private Sub BtnDisplayBadSectors_Click(sender As Object, e As EventArgs) Handles BtnDisplayBadSectors.Click
        HexDisplayBadSectors()
    End Sub

    Private Sub BtnDisplayBootSector_Click(sender As Object, e As EventArgs) Handles BtnDisplayBootSector.Click
        HexDisplayBootSector()
    End Sub

    Private Sub BtnDisplayClusters_Click(sender As Object, e As EventArgs) Handles BtnDisplayClusters.Click
        HexDisplayFreeClusters()
    End Sub

    Private Sub BtnDisplayDirectory_Click(sender As Object, e As EventArgs) Handles BtnDisplayDirectory.Click, BtnFileMenuViewDirectory.Click
        If sender.Tag IsNot Nothing Then
            HexDisplayDirectoryEntry(sender.tag)
        End If
    End Sub

    Private Sub BtnDisplayDisk_Click(sender As Object, e As EventArgs) Handles BtnDisplayDisk.Click
        HexDisplayDiskImage()
    End Sub

    Private Sub BtnDisplayFAT_Click(sender As Object, e As EventArgs) Handles BtnDisplayFAT.Click
        HexDisplayFAT()
    End Sub

    Private Sub BtnDisplayFile_Click(sender As Object, e As EventArgs) Handles BtnDisplayFile.Click
        If sender.tag IsNot Nothing Then
            HexDisplayDirectoryEntry(sender.tag)
        End If
    End Sub

    Private Sub BtnDisplayLostClusters_Click(sender As Object, e As EventArgs) Handles BtnDisplayLostClusters.Click
        HexDisplayLostClusters()
    End Sub
    Private Sub BtnEditBootSector_Click(sender As Object, e As EventArgs) Handles BtnEditBootSector.Click
        ContextMenuEdit.Close()
        BootSectorEdit()
    End Sub

    Private Sub BtnEditFAT_Click(sender As Object, e As EventArgs) Handles BtnEditFAT.Click
        If sender.tag IsNot Nothing Then
            ContextMenuEdit.Close()
            If sender.tag = -1 Then
                FATEdit(0)
            Else
                FATEdit(sender.tag)
            End If
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
        ContextMenuEdit.Close()
        FileExport()
    End Sub

    Private Sub BtnFileMenuDeleteFile_Click(sender As Object, e As EventArgs) Handles BtnFileMenuDeleteFile.Click
        DeleteSelectedFiles(False)
    End Sub

    Private Sub BtnFileMenuDeleteFileWithFill_Click(sender As Object, e As EventArgs) Handles BtnFileMenuDeleteFileWithFill.Click
        DeleteSelectedFiles(True)
    End Sub

    Private Sub BtnFileMenuFixSize_Click(sender As Object, e As EventArgs) Handles BtnFileMenuFixSize.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            FixFileSize(ListViewFiles.SelectedItems(0))
        End If
    End Sub

    Private Sub BtnFileMenuRemoveDeletedFile_Click(sender As Object, e As EventArgs) Handles BtnFileMenuRemoveDeletedFile.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            RemoveDeletedFile(ListViewFiles.SelectedItems(0))
        End If
    End Sub

    Private Sub BtnFileMenuUnDeleteFile_Click(sender As Object, e As EventArgs) Handles BtnFileMenuUnDeleteFile.Click
        If ListViewFiles.SelectedItems.Count = 1 Then
            UndeleteFile(ListViewFiles.SelectedItems(0))
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
            HexDisplayDirectoryEntry(FileData.DirectoryEntry.Offset)
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
        ContextMenuEdit.Close()
        FilePropertiesEdit()
    End Sub


    Private Sub BtnFixImageSize_Click(sender As Object, e As EventArgs) Handles BtnFixImageSize.Click, BtnResizeImage.Click
        FixImageSize()
    End Sub

    Private Sub BtnHelpAbout_Click(sender As Object, e As EventArgs) Handles BtnHelpAbout.Click
        Dim AboutBox As New AboutBox()
        AboutBox.ShowDialog()
    End Sub

    Private Sub BtnHelpProjectPage_Click(sender As Object, e As EventArgs) Handles BtnHelpProjectPage.Click
        Process.Start(SITE_URL)
    End Sub

    Private Sub BtnHelpUpdateCheck_Click(sender As Object, e As EventArgs) Handles BtnHelpUpdateCheck.Click
        CheckForUpdates()
    End Sub

    Private Sub BtnOpen_Click(sender As Object, e As EventArgs) Handles BtnOpen.Click, ToolStripBtnOpen.Click
        FilesOpen()
    End Sub

    Private Sub BtnReadFloppyA_Click(sender As Object, e As EventArgs) Handles BtnReadFloppyA.Click
        FloppyDiskRead(FloppyDriveEnum.FloppyDriveA)
    End Sub

    Private Sub BtnReadFloppyB_Click(sender As Object, e As EventArgs) Handles BtnReadFloppyB.Click
        FloppyDiskRead(FloppyDriveEnum.FloppyDriveB)
    End Sub

    Private Sub BtnRedo_Click(sender As Object, e As EventArgs) Handles BtnRedo.Click, ToolStripBtnRedo.Click
        ContextMenuEdit.Close()
        _Disk.Data.Redo()
        DiskImageRefresh()
    End Sub

    Private Sub BtnRemoveBootSector_Click(sender As Object, e As EventArgs) Handles BtnRemoveBootSector.Click
        BootSectorRemove()
    End Sub

    Private Sub BtnReplaceFile_Click(sender As Object, e As EventArgs) Handles BtnReplaceFile.Click, BtnFileMenuReplaceFile.Click
        ContextMenuEdit.Close()
        If ListViewFiles.SelectedItems.Count = 1 Then
            FileReplace(ListViewFiles.SelectedItems(0))
        End If
    End Sub

    Private Sub BtnResetSort_Click(sender As Object, e As EventArgs) Handles BtnResetSort.Click
        ClearSort(True)
    End Sub

    Private Sub BtnRestoreBootSector_Click(sender As Object, e As EventArgs) Handles BtnRestoreBootSector.Click
        BootSectorRestore()
    End Sub

    Private Sub BtnRetry_Click(sender As Object, e As EventArgs) Handles btnRetry.Click
        ReloadCurrentImage(False)
    End Sub

    Private Sub BtnRevert_Click(sender As Object, e As EventArgs) Handles BtnRevert.Click
        ContextMenuEdit.Close()
        RevertChanges()
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click, ToolStripBtnSave.Click
        SaveCurrent(False)
    End Sub

    Private Sub BtnSaveAll_Click(sender As Object, e As EventArgs) Handles BtnSaveAll.Click, ToolStripBtnSaveAll.Click
        If MsgBox("Are you sure you wish to save all modified files?", MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
            SaveAll()
        End If
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
        ContextMenuEdit.Close()
        _Disk.Data.Undo()
        DiskImageRefresh()
    End Sub

    Private Sub BtnWin9xClean_Click(sender As Object, e As EventArgs) Handles BtnWin9xClean.Click
        Win9xCleanCurrent()
    End Sub
    Private Sub BtnWin9xCleanBatch_Click(sender As Object, e As EventArgs) Handles BtnWin9xCleanBatch.Click
        Win9xCleanBatch()
    End Sub

    Private Sub BtnWriteFloppyA_Click(sender As Object, e As EventArgs) Handles BtnWriteFloppyA.Click
        FloppyDiskWrite(FloppyDriveEnum.FloppyDriveA)
    End Sub

    Private Sub BtnWriteFloppyB_Click(sender As Object, e As EventArgs) Handles BtnWriteFloppyB.Click
        FloppyDiskWrite(FloppyDriveEnum.FloppyDriveB)
    End Sub

    Private Sub ComboFAT_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboFAT.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim CurrentImageData As LoadedImageData = ComboImages.SelectedItem

        If CurrentImageData IsNot Nothing Then
            CurrentImageData.FATIndex = ComboFAT.SelectedIndex
            ReloadCurrentImage(False)
        End If
    End Sub

    Private Sub ComboFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboOEMName.SelectedIndexChanged, ComboDiskType.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        FiltersApply(False)
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
                    Dim CurrentImageData As LoadedImageData = CB.Items(e.Index)
                    If CurrentImageData IsNot Nothing AndAlso CurrentImageData.Filter(FilterTypes.ModifiedFiles) Then
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

        ReloadCurrentImage(False)
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

    Private Sub ContextMenuEdit_Closing(sender As Object, e As ToolStripDropDownClosingEventArgs) Handles ContextMenuEdit.Closing
        If e.CloseReason = ToolStripDropDownCloseReason.ItemClicked Then
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

    Private Sub Debounce_Tick(sender As Object, e As EventArgs) Handles Debounce.Tick
        Debounce.Stop()

        FiltersApply(False)
    End Sub

    Private Sub ExportUnknownImages_CheckStateChanged(sender As Object, e As EventArgs)
        _ExportUnknownImages = DirectCast(sender, ToolStripMenuItem).Checked
    End Sub

    Private Sub File_DragDrop(sender As Object, e As DragEventArgs) Handles ComboImages.DragDrop, ComboImagesFiltered.DragDrop, LabelDropMessage.DragDrop, ListViewFiles.DragDrop, ListViewHashes.DragDrop, ListViewSummary.DragDrop
        Dim Files As String() = e.Data.GetData(DataFormats.FileDrop)
        ProcessFileDrop(Files)
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
            _CheckAll = Not _CheckAll
            _SuppressEvent = True
            For Each Item As ListViewItem In ListViewFiles.Items
                Item.Selected = _CheckAll
            Next
            _SuppressEvent = False
            ItemSelectionChanged()
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
        _FileVersion = GetVersionString()
        Me.Text = GetWindowCaption()

        InitValidFilters()

        PositionForm()

        BtnCompare.Visible = False
        ListViewFiles.DoubleBuffer
        ListViewSummary.DoubleBuffer
        _ListViewHeader = New ListViewHeader(ListViewFiles.Handle)
        ListViewSummary.AutoResizeColumnsContstrained(ColumnHeaderAutoResizeStyle.None)
        ImageFilters = New ImageFilters(ContextMenuFilters)
        SubFiltersInitialize()
        _LoadedFileNames = New Dictionary(Of String, LoadedImageData)
        _BootStrapDB = New BoootstrapDB
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

        If My.Settings.Debug Then
            InitDebugFeatures()
        End If

        DetectFloppyDrives()
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

#End Region

End Class