Public Module Filters
    Public Enum FilterTypes
        UnknownOEMID = 1
        MismatchedOEMID = 2
        HasCreated = 4
        HasLastAccessed = 8
        HasInvalidImage = 16
        HasLongFileNames = 32
        HasInvalidDirectoryEntries = 64
        ModifiedFiles = 128
        UnusedClusters = 256
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
                Caption = "Invalid Image"
            Case FilterTypes.HasLongFileNames
                Caption = "Has Long File Names"
            Case FilterTypes.HasInvalidDirectoryEntries
                Caption = "Has Invalid Directory Entries"
            Case FilterTypes.ModifiedFiles
                Caption = "Modified Files"
            Case FilterTypes.UnusedClusters
                Caption = "Unused Clusters with Data"
            Case Else
                Caption = ""
        End Select
        Caption &= "  [" & Count & "]"

        Return Caption
    End Function

    Public Function IsFiltered(ImageData As LoadedImageData, AppliedFilters As FilterTypes) As Boolean
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
End Module


