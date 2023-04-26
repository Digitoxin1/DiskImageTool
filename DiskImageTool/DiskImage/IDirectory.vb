Namespace DiskImage
    Public Interface IDirectory
        ReadOnly Property Data As DirectoryData
        ReadOnly Property SectorChain As List(Of UInteger)
        Function GetContent() As Byte()
        Function GetFile(Index As UInteger) As DirectoryEntry
        Function HasFile(Filename As String) As Boolean
        Sub RefreshData()
    End Interface
End Namespace
