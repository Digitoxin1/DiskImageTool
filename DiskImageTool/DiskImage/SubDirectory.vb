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

        Public Overrides Function AddFile(FilePath As String, LFN As Boolean) As Boolean Implements IDirectory.AddFile
            Return AddFile(FilePath, LFN, Disk.FAT.FreeClusters)
        End Function

        Public Overrides Function AddFile(FilePath As String, WindowsAdditions As Boolean, ClusterList As SortedSet(Of UShort)) As Boolean Implements IDirectory.AddFile
            Dim ClusterSize = Disk.BPB.BytesPerCluster
            Dim FileInfo = New IO.FileInfo(FilePath)
            Dim LFNEntries As List(Of Byte()) = Nothing
            Dim EntryCount As Integer = 1

            Dim ShortFileName = GetAvailableFileName(FileInfo.Name)
            If WindowsAdditions Then
                LFNEntries = GetLFNDirectoryEntries(FileInfo.Name, ShortFileName)
                EntryCount += LFNEntries.Count
            End If

            Dim Length = FileInfo.Length

            Dim Entries = GetAvailableEntries(EntryCount)

            If Entries Is Nothing Then
                Length += ClusterSize
            End If

            If Length > ClusterList.Count * ClusterSize Then
                Return False
            End If

            If Entries Is Nothing Then
                Dim Cluster = Disk.FAT.GetNextFreeCluster(ClusterList, True)

                If Cluster = 0 Then
                    Return False
                End If

                ExpandDirectorySize(Cluster)

                Entries = GetAvailableEntries(EntryCount)
            End If

            Dim DirectoryEntry = Entries(Entries.Count - 1)
            Dim Result = DirectoryEntry.AddFile(FilePath, ShortFileName, WindowsAdditions, ClusterList)

            If Result Then
                Dim Checksum = DirectoryEntry.GetLFNChecksum
                If WindowsAdditions Then
                    For Counter = 0 To LFNEntries.Count - 1
                        Dim Buffer = LFNEntries(Counter)
                        Buffer(13) = Checksum
                        DirectoryEntry = Entries(Counter)
                        DirectoryEntry.Data = Buffer
                    Next
                End If

                Data.EntryCount += EntryCount
                Data.AvailableEntries -= EntryCount
            End If

            Return Result
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
        End Sub

        Public Overrides Function GetContent() As Byte() Implements IDirectory.GetContent
            Return GetDataFromChain(Disk.Image.Data, SectorChain)
        End Function

        Public Function ReduceDirectorySize() As Boolean
            If ClusterChain.Count < 2 Then
                Return False
            End If

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

            Return True
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
    End Class
End Namespace
