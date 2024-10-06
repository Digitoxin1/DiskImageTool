Namespace DiskImage
    Public Interface IDirectory
        ReadOnly Property ClusterChain As List(Of UShort)
        ReadOnly Property Data As DirectoryData
        ReadOnly Property DirectoryEntries As List(Of DirectoryEntry)
        ReadOnly Property Disk As Disk
        ReadOnly Property IsRootDirectory As Boolean
        ReadOnly Property ParentEntry As DirectoryEntry
        ReadOnly Property SectorChain As List(Of UInteger)
        Function GetContent() As Byte()
        Function GetFile(Index As UInteger) As DirectoryEntry
        Function GetNextAvailableEntry() As DirectoryEntry
        Function HasFile(Filename As String) As Integer
    End Interface
End Namespace
