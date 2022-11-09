Namespace DiskImage
    Public Interface IDirectory
        ReadOnly Property SectorChain As List(Of UInteger)
        Function DirectoryEntryCount() As UInteger
        Function FileCount() As UInteger
        Function GetContent() As Byte()
        Function GetFile(Index As UInteger) As DirectoryEntry
        Function HasFile(Filename As String) As Boolean
    End Interface
End Namespace
