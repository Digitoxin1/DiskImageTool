﻿Imports DiskImageTool.DiskImage

Namespace Filters
    Public Class ImageFilters
        Inherits ImageFiltersBase

        Private Const NULL_CHAR As Char = "�"
        Private ReadOnly _SubFilterDiskType As ComboFilter
        Private ReadOnly _SubFilterOEMName As ComboFilter
        Private _SuppressEvent As Boolean = False

        Public Property SuppressEvent As Boolean
            Get
                Return _SuppressEvent
            End Get
            Set
                _SuppressEvent = Value
            End Set
        End Property

        Public Sub New(ContextMenuFilters As ContextMenuStrip, ToolStripOEMNameCombo As ToolStripComboBox, ToolStripDiskTypeCombo As ToolStripComboBox)
            MyBase.New(ContextMenuFilters)
            _SubFilterDiskType = New ComboFilter(ToolStripDiskTypeCombo)
            _SubFilterOEMName = New ComboFilter(ToolStripOEMNameCombo)
        End Sub

        Public Sub DiskTypeUpdate(Disk As DiskImage.Disk, ImageData As ImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
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

        Public Sub OEMNameUpdate(Disk As DiskImage.Disk, ImageData As ImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
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
                        OEMNameAdd(ImageData.OEMName, UpdateFilters)
                    End If
                End If
            End If
        End Sub

        Public Sub ScanDirectory(Disk As DiskImage.Disk, ImageData As ImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
            Dim Response As DirectoryScanResponse
            Dim HasLostClusters As Boolean = False

            If Not Remove AndAlso Disk.IsValidImage Then
                Response = ProcessDirectoryEntries(Disk.RootDirectory, Nothing)
                HasLostClusters = Disk.RootDirectory.FATAllocation.LostClusters.Count > 0
            Else
                Response = New DirectoryScanResponse(Nothing)
            End If

            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FileSystem_HasCreationDate, Response.HasValidCreated)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FileSystem_HasLastAccessDate, Response.HasValidLastAccessed)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FileSystem_HasReservedBytesSet, Response.HasNTUnknownFlags Or Response.HasFAT32Cluster)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FileSystem_HasLongFileNames, Response.HasLFN)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FileSystem_DirectoryHasAdditionalData, Response.HasAdditionalData)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FileSystem_DirectoryHasBootSector, Response.HasBootSector)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FileSystem_InvalidDirectoryEntries, Response.HasInvalidDirectoryEntries)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FAT_ChainingErrors, Response.HasFATChainingErrors)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FAT_LostClusters, HasLostClusters)
        End Sub

        Public Sub ScanDisk(Disk As DiskImage.Disk, ImageData As ImageData, TitleDB As FloppyDB, EnableWriteSpliceFilter As Boolean, ExportUnknownImages As Boolean, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
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
            Dim Disk_CustomBootLoader As Boolean = False
            Dim Disk_HasWriteSplices As Boolean = False
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
                            Disk_CustomBootLoader = True
                            Disk_NOBPB = False
                        End If
                    End If
                End If

                If TitleDB.TitleCount > 0 Then
                    TitleFindResult = TitleDB.TitleFind(Disk)
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

                If EnableWriteSpliceFilter Then
                    Disk_HasWriteSplices = DiskHasWriteSplices(Disk)
                End If
            Else
                Disk_UnknownFormat = False
            End If

            If TitleDB.TitleCount > 0 Then
                FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Image_InDatabase, Image_InDatabase)
                FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Image_NotInDatabase, Image_NotInDatabase)
                FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Image_Verified, Image_Verified)
                FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Image_Unverified, Image_Unverified)
            End If

            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FAT_BadSectors, FAT_BadSectors)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.FAT_MismatchedFATs, FATS_MismatchedFATs)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_UnknownFormat, Disk_UnknownFormat)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_MismatchedMediaDescriptor, Disk_MismatchedMediaDescriptor)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_MismatchedImageSize, Disk_MismatchedImageSize)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_CustomFormat, Disk_CustomFormat)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_NOBPB, Disk_NOBPB)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_NoBootLoader, Disk_NoBootLoader)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_CustomBootLoader, Disk_CustomBootLoader)

            If EnableWriteSpliceFilter Then
                FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_HasWriteSplices, Disk_HasWriteSplices)
            End If

            If My.Settings.Debug And Disk IsNot Nothing Then
                FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Database_MismatchedStatus, Database_MismatchedStatus)

                If ExportUnknownImages Then
                    If Image_NotInDatabase Then
                        If TitleFindResult Is Nothing Then
                            TitleFindResult = TitleDB.TitleFind(Disk)
                        End If
                        If FileData Is Nothing Then
                            FileData = New FloppyDB.FileNameData(ImageData.FileName)
                        End If
                        Dim Media = GetFloppyDiskFormatName(Disk.BPB, True)
                        TitleDB.AddTile(FileData, Media, TitleFindResult.MD5)
                    End If
                End If
            End If
        End Sub

        Public Sub ScanFreeClusters(Disk As DiskImage.Disk, ImageData As ImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
            Dim HasFreeClusters As Boolean = False

            If Not Remove AndAlso Disk.IsValidImage Then
                HasFreeClusters = Disk.FAT.HasFreeClusters(FAT12.FreeClusterEmum.WithData)
            End If

            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Disk_FreeClustersWithData, HasFreeClusters)
        End Sub

        Public Sub ScanModified(Disk As DiskImage.Disk, ImageData As ImageData, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
            Dim IsModified As Boolean = Not Remove And (Disk IsNot Nothing AndAlso (Disk.Image.History.Modified Or ImageData.FileType = ImageData.FileTypeEnum.NewImage))

            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.ModifiedFiles, IsModified, True)
        End Sub

        Public Sub ScanOEMName(Disk As DiskImage.Disk, ImageData As ImageData, BootStrapDB As BootstrapDB, Optional UpdateFilters As Boolean = False, Optional Remove As Boolean = False)
            Dim Bootstrap_Unknown = False
            Dim OEMName_Unknown = False
            Dim OEMName_Mismatched = False
            Dim OEMName_Verified = False
            Dim OEMName_Unverified = False
            Dim OEMName_Windows9x = False

            If Not Remove AndAlso Disk.IsValidImage Then
                Dim DoOEMNameCheck As Boolean = Disk.BootSector.BPB.IsValid
                Dim OEMNameResponse = BootStrapDB.CheckOEMName(Disk.BootSector)
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
                FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.Bootstrap_Unknown, Bootstrap_Unknown)
            End If
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.OEMName_Windows9x, OEMName_Windows9x)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.OEMName_Mismatched, OEMName_Mismatched)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.OEMName_Unknown, OEMName_Unknown)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.OEMName_Verified, OEMName_Verified)
            FilterUpdate(ImageData, UpdateFilters, Filters.FilterTypes.OEMName_Unverified, OEMName_Unverified)
        End Sub

        Public Sub SubFilterAdd(ImageData As ImageData)
            OEMNameAdd(ImageData.OEMName, False)
            _SubFilterDiskType.Add(ImageData.DiskType, False)
        End Sub

        Public Sub SubFiltersClear()
            _SuppressEvent = True

            _SubFilterOEMName.Clear()
            _SubFilterDiskType.Clear()

            _SuppressEvent = False
        End Sub

        Public Sub SubFiltersClearFilter()
            _SuppressEvent = True

            _SubFilterOEMName.ClearFilter()
            _SubFilterDiskType.ClearFilter()

            _SuppressEvent = False
        End Sub

        Public Sub SubFiltersPopulate()
            _SuppressEvent = True

            _SubFilterOEMName.Populate()
            _SubFilterDiskType.Populate()

            _SuppressEvent = False
        End Sub

        Public Sub SubFiltersPopulateUnfiltered(ComboBox As ComboBox)
            _SuppressEvent = True

            OEMNamePopulateUnfiltered(ComboBox)
            DiskTypePopulateUnfiltered(ComboBox)

            _SuppressEvent = False
        End Sub

        Private Sub DiskTypePopulateUnfiltered(ComboBox As ComboBox)
            _SubFilterDiskType.Clear()
            For Each ImageData As ImageData In ComboBox.Items
                _SubFilterDiskType.Add(ImageData.DiskType, False)
            Next
            _SubFilterDiskType.Populate()
        End Sub

        Private Sub OEMNameAdd(OEMName As String, UpdateFilters As Boolean)
            If OEMName.EndsWith("IHC") Then
                Return
            End If

            _SubFilterOEMName.Add(OEMName, UpdateFilters)
        End Sub

        Private Sub OEMNamePopulateUnfiltered(ComboBox As ComboBox)
            _SubFilterOEMName.Clear()
            For Each ImageData As ImageData In ComboBox.Items
                OEMNameAdd(ImageData.OEMName, False)
            Next
            _SubFilterOEMName.Populate()
        End Sub
    End Class
End Namespace
