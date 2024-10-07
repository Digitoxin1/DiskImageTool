Namespace DiskImage
    Public Interface IDirectory
        ReadOnly Property ClusterChain As List(Of UShort)
        ReadOnly Property Data As DirectoryData
        ReadOnly Property DirectoryEntries As List(Of DirectoryEntry)
        ReadOnly Property Disk As Disk
        ReadOnly Property IsRootDirectory As Boolean
        ReadOnly Property ParentEntry As DirectoryEntry
        ReadOnly Property SectorChain As List(Of UInteger)
        Function AddFile(FilePath As String, LFN As Boolean) As Boolean
        Function AddFile(FilePath As String, LFN As Boolean, ClusterList As SortedSet(Of UShort)) As Boolean
        Function GetAvailableFileName(FileName As String) As String
        Function GetContent() As Byte()
        Function GetFile(Index As UInteger) As DirectoryEntry
        Function GetIndex(DirectoryEntry As DirectoryEntry) As Integer
        Function GetAvailableEntry() As DirectoryEntry
        Function GetAvailableEntries(Count As UInteger) As List(Of DirectoryEntry)
        Function HasFile(Filename As String, IncludeDirectories As Boolean) As Integer
    End Interface
End Namespace
