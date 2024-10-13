Namespace DiskImage
    Public Interface IDirectory
        ReadOnly Property ClusterChain As List(Of UShort)
        ReadOnly Property Data As DirectoryData
        ReadOnly Property DirectoryEntries As List(Of DirectoryEntry)
        ReadOnly Property Disk As Disk
        ReadOnly Property IsRootDirectory As Boolean
        ReadOnly Property ParentEntry As DirectoryEntry
        ReadOnly Property SectorChain As List(Of UInteger)
        Function AddFile(FilePath As String, LFN As Boolean, Optional Index As Integer = -1) As Integer
        Function AddFile(FilePath As String, LFN As Boolean, ClusterList As SortedSet(Of UShort), Optional Index As Integer = -1) As Integer
        Function GetAvailableFileName(FileName As String) As String
        Function GetContent() As Byte()
        Function GetFile(Index As UInteger) As DirectoryEntry
        Function GetIndex(DirectoryEntry As DirectoryEntry) As Integer
        Function GetAvailableEntry() As DirectoryEntry
        Function GetFileIndex(Filename As String, IncludeDirectories As Boolean) As Integer
        Function RemoveEntry(Index As UInteger) As Boolean
        Sub UpdateEntryCounts()
    End Interface
End Namespace
