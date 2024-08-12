Public Module FilterMethods

    Public Enum FilterTypes
        ModifiedFiles
        UnknownOEMName
        MismatchedOEMName
        Windows9xOEMName
        HasCreated
        HasLastAccessed
        HasReservedBytesSet
        HasLongFileNames
        HasInvalidDirectoryEntries
        DirectoryHasAdditionalData
        DirectoryHasBootSector
        FreeClustersWithData
        HasInvalidImage
        CustomDiskFormat
        HasBadSectors
        HasLostClusters
        HasInvalidImageSize
        HasMismatchedFATs
        HasMismatchedMediaDescriptor
        HasFATChainingErrors
        KnownImage
        UnknownImage
    End Enum

    Public Function FilterGetCaption(ID As FilterTypes, Count As Integer) As String
        Dim Caption As String

        Select Case ID
            Case FilterTypes.UnknownOEMName
                Caption = "Unknown OEM Name"
            Case FilterTypes.Windows9xOEMName
                Caption = "Windows 9x OEM Name"
            Case FilterTypes.MismatchedOEMName
                Caption = "Mismatched OEM Name"
            Case FilterTypes.HasCreated
                Caption = "Has Creation Date"
            Case FilterTypes.HasLastAccessed
                Caption = "Has Last Access Date"
            Case FilterTypes.HasInvalidImage
                Caption = "Unknown Image Format"
            Case FilterTypes.CustomDiskFormat
                Caption = "Custom Disk Format"
            Case FilterTypes.HasLongFileNames
                Caption = "Has Long File Names"
            Case FilterTypes.HasInvalidDirectoryEntries
                Caption = "Invalid Directory Entries"
            Case FilterTypes.DirectoryHasAdditionalData
                Caption = "Directory has Additional Data"
            Case FilterTypes.DirectoryHasBootSector
                Caption = "Directory has Boot Sector"
            Case FilterTypes.ModifiedFiles
                Caption = "Modified Files"
            Case FilterTypes.FreeClustersWithData
                Caption = "Free Clusters with Data"
            Case FilterTypes.HasBadSectors
                Caption = "Bad Sectors"
            Case FilterTypes.HasLostClusters
                Caption = "Lost Clusters"
            Case FilterTypes.HasMismatchedFATs
                Caption = "Mismatched FATs"
            Case FilterTypes.HasFATChainingErrors
                Caption = "FAT Chaining Errors"
            Case FilterTypes.HasInvalidImageSize
                Caption = "Mismatched Image Size"
            Case FilterTypes.HasMismatchedMediaDescriptor
                Caption = "Mismatched Media Descriptor"
            Case FilterTypes.HasReservedBytesSet
                Caption = "Has Reserved Bytes Set"
            Case FilterTypes.KnownImage
                Caption = "Image in Database"
            Case FilterTypes.UnknownImage
                Caption = "Image Not in Database"
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

        If CheckFilter(FilterTypes.HasInvalidImage, AppliedFilters) Then
            If Not ImageData.ScanInfo.IsValidImage Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.KnownImage, AppliedFilters) Then
            If ImageData.ScanInfo.IsKnownImage Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.UnknownImage, AppliedFilters) Then
            If ImageData.ScanInfo.IsUnknownImage Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasCreated, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasValidCreated Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasLastAccessed, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasValidLastAccessed Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasReservedBytesSet, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasReservedBytesSet Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.MismatchedOEMName, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.OEMNameFound And Not ImageData.ScanInfo.OEMNameMatched Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.Windows9xOEMName, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.OEMNameWin9x Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.UnknownOEMName, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And Not ImageData.ScanInfo.OEMNameFound Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasLongFileNames, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasLongFileNames Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasInvalidDirectoryEntries, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasInvalidDirectoryEntries Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.DirectoryHasAdditionalData, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.DirectoryHasAdditionalData Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.DirectoryHasBootSector, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.DirectoryHasBootSector Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.FreeClustersWithData, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasFreeClustersWithData Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasBadSectors, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasBadSectors Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasLostClusters, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasLostClusters Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasMismatchedFATs, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasMismatchedFATs Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasMismatchedMediaDescriptor, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasMismatchedMediaDescriptor Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasFATChainingErrors, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasFATChainingErrors Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasInvalidImageSize, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasInvalidImageSize Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.CustomDiskFormat, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.CustomDiskFormat Then
                Return False
            End If
        End If

        Return True
    End Function

    Private Function CheckFilter(FilterType As FilterTypes, AppliedFilters As Integer) As Boolean
        Return (AppliedFilters And (2 ^ FilterType)) > 0
    End Function

End Module