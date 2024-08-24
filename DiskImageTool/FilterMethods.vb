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
        FAT_MismatchedFATs
        FAT_ChainingErrors
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
            Case FilterTypes.FAT_MismatchedFATs
                Caption = "FAT - Mismatched FATs"
            Case FilterTypes.FAT_ChainingErrors
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
        If Count > 0 Then
            Caption &= "  [" & Count & "]"
        End If

        Return Caption
    End Function

    Public Function IsFiltered(ImageData As LoadedImageData, AppliedFilters As Integer, ByRef FilterCounts() As FilterCounts) As Boolean
        Dim Result = False
        ImageData.AppliedFilters = 0

        Dim FilterCount As Integer = [Enum].GetNames(GetType(FilterTypes)).Length
        For FilterType As FilterTypes = 0 To FilterCount - 1
            If Not ImageData.Filter(FilterType) Then
                ImageData.AppliedFilters += (2 ^ FilterType)
                If CheckFilter(FilterType, AppliedFilters) Then
                    Result = True
                End If
            End If
        Next

        If Not Result Then
            For FilterType As FilterTypes = 0 To FilterCount - 1
                If Not CheckFilter(FilterType, ImageData.AppliedFilters) Then
                    FilterCounts(FilterType).Available += 1
                End If
            Next
        End If

        Return Result
    End Function

    Private Function CheckFilter(FilterType As FilterTypes, AppliedFilters As Integer) As Boolean
        Return (AppliedFilters And (2 ^ FilterType)) > 0
    End Function

    Public Class FilterCounts
        Public Sub New()
            _Total = 0
            _Available = 0
        End Sub
        Public Property Total As Integer
        Public Property Available As Integer
    End Class

    Public Class FilterTag
        Public Sub New(Value As Integer)
            _Value = Value
            _Visible = False
        End Sub
        Public ReadOnly Property Value As Integer
        Public Property Visible As Boolean
    End Class
End Module