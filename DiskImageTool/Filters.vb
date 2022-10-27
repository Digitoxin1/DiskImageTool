Public Module Filters
    Public Enum FilterTypes
        ModifiedFiles
        UnknownOEMID
        MismatchedOEMID
        HasCreated
        HasLastAccessed
        HasLongFileNames
        HasInvalidDirectoryEntries
        UnusedClusters
        HasInvalidImage
        HasBadSectors
        HasMismatchedFATs
        HasFATChainingErrors
    End Enum

    Public Function FilterGetCaption(ID As FilterTypes, Count As Integer) As String
        Dim Caption As String

        Select Case ID
            Case FilterTypes.UnknownOEMID
                Caption = "Unknown OEM ID"
            Case FilterTypes.MismatchedOEMID
                Caption = "Mismatched OEM ID"
            Case FilterTypes.HasCreated
                Caption = "Has Creation Date"
            Case FilterTypes.HasLastAccessed
                Caption = "Has Last Access Date"
            Case FilterTypes.HasInvalidImage
                Caption = "Unknown Image Format"
            Case FilterTypes.HasLongFileNames
                Caption = "Has Long File Names"
            Case FilterTypes.HasInvalidDirectoryEntries
                Caption = "Invalid Directory Entries"
            Case FilterTypes.ModifiedFiles
                Caption = "Modified Files"
            Case FilterTypes.UnusedClusters
                Caption = "Unused Clusters with Data"
            Case FilterTypes.HasBadSectors
                Caption = "Bad Sectors"
            Case FilterTypes.HasMismatchedFATs
                Caption = "Mismatched FATs"
            Case FilterTypes.HasFATChainingErrors
                Caption = "FAT Chaining Errors"
            Case Else
                Caption = ""
        End Select
        Caption &= "  [" & Count & "]"

        Return Caption
    End Function

    Private Function CheckFilter(FilterType As FilterTypes, AppliedFilters As Integer) As Boolean
        Return (AppliedFilters And (2 ^ FilterType)) > 0
    End Function

    Public Function IsFiltered(ImageData As LoadedImageData, AppliedFilters As FilterTypes) As Boolean
        If CheckFilter(FilterTypes.HasInvalidImage, AppliedFilters) Then
            If Not ImageData.ScanInfo.IsValidImage Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasCreated, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasCreated Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasLastAccessed, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasLastAccessed Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.MismatchedOEMID, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.OEMIDFound And Not ImageData.ScanInfo.OEMIDMatched Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.UnknownOEMID, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And Not ImageData.ScanInfo.OEMIDFound Then
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

        If CheckFilter(FilterTypes.ModifiedFiles, AppliedFilters) Then
            If ImageData.Modified Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.UnusedClusters, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasUnusedClusters Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasBadSectors, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasBadSectors Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasMismatchedFATs, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.hasmismatchedFATS Then
                Return False
            End If
        End If

        If CheckFilter(FilterTypes.HasFATChainingErrors, AppliedFilters) Then
            If ImageData.ScanInfo.IsValidImage And ImageData.ScanInfo.HasFATChainingErrors Then
                Return False
            End If
        End If

        Return True
    End Function
End Module


