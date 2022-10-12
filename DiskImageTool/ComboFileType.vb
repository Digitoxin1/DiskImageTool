Public Enum FilterTypes
    UnknownOEMID = 1
    MismatchedOEMID = 2
    HasCreated = 4
    HasLastAccessed = 8
    HasInvalidImage = 16
    HasLongFileNames = 32
    HasInvalidDirectoryEntries = 64
    ModifiedFiles = 128
End Enum

Public Class ComboFileType
    Private _ID As FilterTypes
    Private _Count As Integer

    Public Property Count As Integer
        Get
            Return _Count
        End Get
        Set
            _Count = Value
        End Set
    End Property

    Public Property ID As FilterTypes
        Get
            Return _ID
        End Get
        Set
            _ID = Value
        End Set
    End Property
    Public Sub New(ID As FilterTypes, Count As Integer)
        _ID = ID
        _Count = Count
    End Sub
    Public Overrides Function ToString() As String
        Return GetText() & "  [" & Count & "]"
    End Function

    Private Function GetText() As String
        Select Case ID
            Case FilterTypes.UnknownOEMID
                Return "Unknown OEM ID"
            Case FilterTypes.MismatchedOEMID
                Return "Mismatched OEM ID"
            Case FilterTypes.HasCreated
                Return "Has Creation Date"
            Case FilterTypes.HasLastAccessed
                Return "Has Last Access Date"
            Case FilterTypes.HasInvalidImage
                Return "Invalid Image"
            Case FilterTypes.HasLongFileNames
                Return "Has Long File Names"
            Case FilterTypes.HasInvalidDirectoryEntries
                Return "Has Invalid Directory Entries"
            Case FilterTypes.ModifiedFiles
                Return "Modified Files"
            Case Else
                Return ""
        End Select
    End Function
End Class


