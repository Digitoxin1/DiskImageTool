Namespace Filters
    Public Enum FilterTypes
        ModifiedFiles
        Disk_UnknownFormat
        Disk_CustomFormat
        Disk_NOBPB
        Disk_NoBootLoader
        Disk_CustomBootLoader
        Disk_MismatchedImageSize
        Disk_MismatchedMediaDescriptor
        Disk_FreeClustersWithData
        Disk_HasWriteSplices
        Bootstrap_Unknown
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
        FAT_MismatchedFATs
        FAT_ChainingErrors
        Image_InDatabase
        Image_NotInDatabase
        Image_Verified
        Image_Unverified
        Image_InTDC
        Image_NotInTDC
        Database_MismatchedStatus
    End Enum

    Public Module Enums
        Public Function FilterGetCount() As Integer
            Return [Enum].GetNames(GetType(FilterTypes)).Length
        End Function
    End Module
End Namespace
