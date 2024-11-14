Namespace DiskImage
    Public Structure AddDirectoryData
        Dim Index As Integer
        Dim EntriesNeeded As Integer
        Dim ShortFileName As String
        Dim ClusterList As SortedSet(Of UShort)
        Dim Options As AddFileOptions
        Dim LFNEntries As List(Of Byte())
        Dim RequiresExpansion As Boolean
        Dim Entry As DirectoryEntry
    End Structure

    Public Structure AddFileData
        Dim FilePath As String
        Dim Options As AddFileOptions
        Dim Index As Integer
        Dim EntriesNeeded As Integer
        Dim ShortFileName As String
        Dim ClusterList As SortedSet(Of UShort)
        Dim LFNEntries As List(Of Byte())
        Dim RequiresExpansion As Boolean
    End Structure

    Public Structure DirectoryCacheEntry
        Dim Checksum As UInteger
        Dim Data() As Byte
    End Structure

    Public Structure UpdateLFNData
        Dim EntriesNeeded As Integer
        Dim ShortFileName As String
        Dim CurrentLFNIndex As Integer
        Dim LFNEntries As List(Of Byte())
        Dim RequiresExpansion As Boolean
        Dim DirectoryEntry As DirectoryEntry
        Dim NTLowerCaseFileName As Boolean
        Dim NTLowerCaseExtension As Boolean
    End Structure
End Namespace
