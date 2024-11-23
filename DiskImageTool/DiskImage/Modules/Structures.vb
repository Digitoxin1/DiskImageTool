Namespace DiskImage
    Public Class AddFileData
        Public Sub New()
            _ClusterList = Nothing
            _Entry = Nothing
            _FileInfo = Nothing
        End Sub

        Public Property ClusterList As SortedSet(Of UShort)
        Public Property EntriesNeeded As Integer
        Public Property Entry As DirectoryEntry
        Public Property FileInfo As IO.FileInfo
        Public Property HasNTLowerCaseExtension As Boolean
        Public Property HasNTLowerCaseFileName As Boolean
        Public Property Index As Integer
        Public Property LFNEntries As List(Of Byte())
        Public Property Options As AddFileOptions
        Public Property RequiresExpansion As Boolean
        Public Property ShortFileName As String
    End Class

    Public Structure DirectoryCacheEntry
        Dim Checksum As UInteger
        Dim Data() As Byte
    End Structure
End Namespace
