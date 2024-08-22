Public Module FilterMethods

    Public Enum FilterTypes
        ModifiedFiles
        Disk_UnknownFormat
        Disk_CustomFormat
        Disk_NOBPB
        DIsk_CustomBootLoader
        Disk_MismatchedImageSize
        Disk_MismatchedMediaDescriptor
        Disk_FreeClustersWithData
        OEMName_Unknown
        OEMName_Mismatched
        OEMName_Windows9x
        OEMName_Verified
        OEMName_Unverified
        FileSystem_HasCreationDate
        FileSystem_HasLastAccessDate
        FileSystem_HasReservedBytesSet
        FileSystem_HasLongFileNames
        FileSystem_InvalidDirectoryEntries
        FileSystem_DirectoryHasAdditionalData
        FileSystem_DirectoryHasBootSector
        FAT_BadSectors
        FAT_LostClusters
        FATS_MismatchedFATs
        FATS_ChainingErrors
        Image_InDatabase
        Image_NotInDatabase
        Image_Verified
        Image_Unverified
    End Enum

    Public Function FilterGetCaption(ID As FilterTypes, Count As Integer) As String
        Dim Caption As String

        Select Case ID
            Case FilterTypes.ModifiedFiles
                Caption = "Modified Files"
            Case FilterTypes.Disk_UnknownFormat
                Caption = "Disk - Unknown Format"
            Case FilterTypes.Disk_CustomFormat
                Caption = "Disk - Custom Format"
            Case FilterTypes.Disk_NOBPB
                Caption = "Disk - No BPB"
            Case FilterTypes.DIsk_CustomBootLoader
                Caption = "Disk - Custom Boot Loader"
            Case FilterTypes.Disk_MismatchedImageSize
                Caption = "Disk - Mismatched Image Size"
            Case FilterTypes.Disk_MismatchedMediaDescriptor
                Caption = "Disk - Mismatched Media Descriptor"
            Case FilterTypes.Disk_FreeClustersWithData
                Caption = "Disk - Free Clusters with Data"
            Case FilterTypes.OEMName_Unknown
                Caption = "OEM Name - Unknown"
            Case FilterTypes.OEMName_Mismatched
                Caption = "OEM Name - Mismatched"
            Case FilterTypes.OEMName_Windows9x
                Caption = "OEM Name - Windows 9x"
            Case FilterTypes.OEMName_Verified
                Caption = "OEM Name - Verified"
            Case FilterTypes.OEMName_Unverified
                Caption = "OEM Name - Unverified"
            Case FilterTypes.FileSystem_HasCreationDate
                Caption = "File System - Has Creation Date"
            Case FilterTypes.FileSystem_HasLastAccessDate
                Caption = "File System - Has Last Access Date"
            Case FilterTypes.FileSystem_HasReservedBytesSet
                Caption = "File System - Has Reserved Bytes Set"
            Case FilterTypes.FileSystem_HasLongFileNames
                Caption = "File System - Has Long File Names"
            Case FilterTypes.FileSystem_InvalidDirectoryEntries
                Caption = "File System - Invalid Directory Entries"
            Case FilterTypes.FileSystem_DirectoryHasAdditionalData
                Caption = "File System - Directory has Additional Data"
            Case FilterTypes.FileSystem_DirectoryHasBootSector
                Caption = "File System - Directory has Boot Sector"
            Case FilterTypes.FAT_BadSectors
                Caption = "FAT - Bad Sectors"
            Case FilterTypes.FAT_LostClusters
                Caption = "FAT - Lost Clusters"
            Case FilterTypes.FATS_MismatchedFATs
                Caption = "FAT - Mismatched FATs"
            Case FilterTypes.FATS_ChainingErrors
                Caption = "FAT - Chaining Errors"
            Case FilterTypes.Image_InDatabase
                Caption = "Image - In Database"
            Case FilterTypes.Image_NotInDatabase
                Caption = "Image - Not in Database"
            Case FilterTypes.Image_Verified
                Caption = "Image - Verified"
            Case FilterTypes.Image_Unverified
                Caption = "Image - Unverified"
            Case Else
                Caption = ""
        End Select
        Caption &= "  [" & Count & "]"

        Return Caption
    End Function

    Public Function IsFiltered(ImageData As LoadedImageData, AppliedFilters As FilterTypes) As Boolean
        If CheckFilter(FilterTypes.ModifiedFiles, AppliedFilters) Then
            If ImageData.Modified Then
                Return False
            End If
        End If

        If Not ImageData.Scanned Then
            Return True
        End If

        If CheckFilter(FilterTypes.Disk_UnknownFormat, AppliedFilters) Then
            If Not ImageData.ScanInfo.IsValidImage Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.Image_InDatabase, AppliedFilters) Then
            If ImageData.ScanInfo.ImageKnown Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.Image_NotInDatabase, AppliedFilters) Then
            If ImageData.ScanInfo.ImageUnknown Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.Image_Verified, AppliedFilters) Then
            If ImageData.ScanInfo.ImageVerified Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.Image_Unverified, AppliedFilters) Then
            If ImageData.ScanInfo.ImageUnverified Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.FileSystem_HasCreationDate, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasValidCreated Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.FileSystem_HasLastAccessDate, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasValidLastAccessed Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.FileSystem_HasReservedBytesSet, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasReservedBytesSet Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.OEMName_Mismatched, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.OEMNameFound And Not ImageData.ScanInfo.OEMNameMatched Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.OEMName_Windows9x, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.OEMNameWin9x Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.OEMName_Unknown, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And Not ImageData.ScanInfo.OEMNameFound Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.OEMName_Verified, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.OEMNameVerified Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.OEMName_Unverified, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.OEMNameUnverified Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.FileSystem_HasLongFileNames, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasLongFileNames Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.FileSystem_InvalidDirectoryEntries, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasInvalidDirectoryEntries Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.FileSystem_DirectoryHasAdditionalData, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.DirectoryHasAdditionalData Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.FileSystem_DirectoryHasBootSector, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.DirectoryHasBootSector Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.Disk_FreeClustersWithData, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasFreeClustersWithData Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.FAT_BadSectors, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasBadSectors Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.FAT_LostClusters, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasLostClusters Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.FATS_MismatchedFATs, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasMismatchedFATs Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.Disk_MismatchedMediaDescriptor, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasMismatchedMediaDescriptor Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.FATS_ChainingErrors, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasFATChainingErrors Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.Disk_MismatchedImageSize, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasInvalidImageSize Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.Disk_CustomFormat, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.CustomDiskFormat Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.Disk_NOBPB, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.NoBPB Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.DIsk_CustomBootLoader, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.CustomBootLoader Then
                Return False
            End If
        End If

        Return True
    End Function

    Private Function CheckFilter(FilterType As FilterTypes, AppliedFilters As Integer) As Boolean
        Return (AppliedFilters And (2 ^ FilterType)) > 0
    End Function

End Module