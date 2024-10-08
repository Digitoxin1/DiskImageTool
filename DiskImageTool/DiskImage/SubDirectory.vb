Imports System.IO

Namespace DiskImage
    Public Class SubDirectory
        Inherits DirectoryBase
        Implements IDirectory

        Private ReadOnly _RootDirectory As RootDirectory

        Sub New(RootDirectory As RootDirectory, ParentEntry As DirectoryEntry)
            MyBase.New(RootDirectory.Disk, ParentEntry)

            _RootDirectory = RootDirectory

            InitializeDirectoryData()
        End Sub

        Public Overrides ReadOnly Property ClusterChain As List(Of UShort) Implements IDirectory.ClusterChain
            Get
                Return ParentEntry.FATChain.Clusters
            End Get
        End Property

        Public Overrides ReadOnly Property SectorChain As List(Of UInteger) Implements IDirectory.SectorChain
            Get
                Return ClusterListToSectorList(Disk.BPB, ParentEntry.FATChain.Clusters)
            End Get
        End Property

        Public Overrides Function AddFile(FilePath As String, WindowsAdditions As Boolean, Optional Index As Integer = -1) As Boolean Implements IDirectory.AddFile
            Return AddFile(FilePath, WindowsAdditions, Disk.FAT.FreeClusters)
        End Function

        Public Overrides Function AddFile(FilePath As String, WindowsAdditions As Boolean, ClusterList As SortedSet(Of UShort), Optional Index As Integer = -1) As Boolean Implements IDirectory.AddFile
            Dim ClusterSize = Disk.BPB.BytesPerCluster
            Dim FileInfo = New IO.FileInfo(FilePath)
            Dim LFNEntries As List(Of Byte()) = Nothing
            Dim EntryCount As Integer = 1
            Dim Entries As List(Of DirectoryEntry)
            Dim Cluster As UShort

            Dim ShortFileName = GetAvailableFileName(FileInfo.Name)
            If WindowsAdditions Then
                LFNEntries = GetLFNDirectoryEntries(FileInfo.Name, ShortFileName)
                EntryCount += LFNEntries.Count
            End If

            Dim Length = FileInfo.Length

            Dim Result As Boolean
            If Index = -1 Then
                Result = Data.AvailableEntries >= EntryCount
            Else
                Result = DirectoryEntries.Count - Data.EntryCount >= EntryCount
            End If

            If Not Result Then
                Length += ClusterSize
            End If

            If Length > ClusterList.Count * ClusterSize Then
                Return False
            End If

            If Not Result Then
                Cluster = Disk.FAT.GetNextFreeCluster(ClusterList, True)

                If Cluster = 0 Then
                    Return False
                End If
            End If

            Dim UseTransaction As Boolean = Disk.BeginTransaction

            If Not Result Then
                ExpandDirectorySize(Cluster)
            End If

            If Index = -1 Then
                Entries = GetAvailableEntries(EntryCount)
            Else
                Index = AdjustIndexForLFN(Index)
                Entries = GetEntries(Index, EntryCount)
                ShiftEntries(Index, EntryCount)
            End If

            Dim DirectoryEntry = Entries(Entries.Count - 1)
            DirectoryEntry.AddFile(FilePath, ShortFileName, WindowsAdditions, ClusterList)

            Dim Checksum = DirectoryEntry.GetLFNChecksum
            If WindowsAdditions Then
                For Counter = 0 To LFNEntries.Count - 1
                    Dim Buffer = LFNEntries(Counter)
                    Buffer(13) = Checksum
                    Entries(Counter).Data = Buffer
                Next
            End If

            Data.EntryCount += EntryCount
            Data.AvailableEntries -= EntryCount

            If UseTransaction Then
                Disk.EndTransaction()
            End If

            Return True
        End Function

        Public Function ExpandDirectorySize() As Boolean
            Dim Cluster = Disk.FAT.GetNextFreeCluster(True)

            If Cluster = 0 Then
                Return False
            End If

            ExpandDirectorySize(Cluster)

            Return True
        End Function

        Public Sub ExpandDirectorySize(Cluster As UShort)
            Dim ClusterSize = Disk.BPB.BytesPerCluster
            Dim EntryCount = ClusterSize \ DirectoryEntry.DIRECTORY_ENTRY_SIZE
            Dim ClusterOffset As UInteger = Disk.BPB.ClusterToOffset(Cluster)

            Dim UseTransaction As Boolean = Disk.BeginTransaction

            Dim LastCluster = ClusterChain.Last
            Disk.FATTables.UpdateTableEntry(LastCluster, Cluster)
            Disk.FATTables.UpdateTableEntry(Cluster, FAT12.FAT_LAST_CLUSTER_END)

            ClusterChain.Add(Cluster)

            Dim Buffer = New Byte(ClusterSize - 1) {}
            Disk.Image.SetBytes(Buffer, ClusterOffset)

            For Index As UInteger = 0 To EntryCount - 1
                Dim Offset = ClusterOffset + (Index * DirectoryEntry.DIRECTORY_ENTRY_SIZE)
                Dim NewDirectoryEntry = New DirectoryEntry(_RootDirectory, Me, Offset, True)
                DirectoryEntries.Add(NewDirectoryEntry)
            Next

            Data.AvailableEntries += EntryCount

            If UseTransaction Then
                Disk.EndTransaction()
            End If
        End Sub

        Public Overrides Function GetContent() As Byte() Implements IDirectory.GetContent
            Return GetDataFromChain(Disk.Image.Data, SectorChain)
        End Function

        Public Function ReduceDirectorySize() As Boolean
            If ClusterChain.Count < 2 Then
                Return False
            End If

            Dim UseTransaction As Boolean = Disk.BeginTransaction

            Dim Cluster = ClusterChain.Last

            Dim ClusterSize = Disk.BPB.BytesPerCluster
            Dim EntryCount = ClusterSize \ DirectoryEntry.DIRECTORY_ENTRY_SIZE
            Dim ClusterOffset As UInteger = Disk.BPB.ClusterToOffset(Cluster)

            ClusterChain.RemoveAt(ClusterChain.Count - 1)

            Dim LastCluster = ClusterChain.Last
            Disk.FATTables.UpdateTableEntry(LastCluster, FAT12.FAT_LAST_CLUSTER_END)
            Disk.FATTables.UpdateTableEntry(Cluster, FAT12.FAT_FREE_CLUSTER)

            For Entry As Integer = DirectoryEntries.Count - 1 To DirectoryEntries.Count - EntryCount Step -1
                DirectoryEntries.RemoveAt(Entry)
            Next

            Data.AvailableEntries -= EntryCount

            If Data.EntryCount > DirectoryEntries.Count Then
                Data.EntryCount = DirectoryEntries.Count
            End If

            If UseTransaction Then
                Disk.EndTransaction()
            End If

            Return True
        End Function

        Public Overrides Function RemoveEntry(Index As UInteger) As Boolean Implements IDirectory.RemoveEntry
            Dim Result = MyBase.RemoveEntry(Index)

            If Result Then
                Dim Cluster = ClusterChain.Last
                If IsClusterEmpty(Cluster) Then
                    ReduceDirectorySize()
                End If
            End If

            Return Result
        End Function

        Private Sub InitializeDirectoryData()
            DirectoryEntries.Clear()

            If Data.Level <= 32 Then
                For Each Cluster In ParentEntry.FATChain.Clusters
                    Dim OffsetStart As UInteger = Disk.BPB.ClusterToOffset(Cluster)
                    Dim OffsetLength As UInteger = Disk.BPB.BytesPerCluster

                    DirectoryBase.GetDirectoryData(_RootDirectory, Me, OffsetStart, OffsetStart + OffsetLength, False)
                Next
            End If
        End Sub

        Private Function IsClusterEmpty(Cluster As UShort) As Boolean
            Dim Offset = Disk.BPB.ClusterToOffset(Cluster)
            Dim ClusterSize As UInteger = Disk.BPB.BytesPerCluster

            Dim Data = Disk.Image.GetBytes(Offset, ClusterSize)
            For Each B In Data
                If B <> &H0 Then
                    Return False
                End If
            Next

            Return True
        End Function
    End Class
End Namespace
