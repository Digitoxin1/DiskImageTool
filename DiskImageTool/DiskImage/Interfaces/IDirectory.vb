Namespace DiskImage
    Public Interface IDirectory
        ReadOnly Property Data As DirectoryData
        ReadOnly Property ClusterChain As List(Of UShort)
        ReadOnly Property SectorChain As List(Of UInteger)
        ReadOnly Property IsRootDirectory As Boolean
        Function GetContent() As Byte()
        Function GetFile(Index As UInteger) As DirectoryEntry
        Function GetNextAvailableEntry() As DirectoryEntry
        Function HasFile(Filename As String) As Integer
        Sub RefreshData()
    End Interface
End Namespace
