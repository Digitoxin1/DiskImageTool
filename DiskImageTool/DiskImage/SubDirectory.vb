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

        Public Overrides Function AddDirectory(EntryData() As Byte, Options As AddFileOptions, Filename As String, Optional Index As Integer = -1) As AddFileData Implements IDirectory.AddDirectory
            Dim Result As Boolean = True

            Dim Data = InitializeAddDirectory(Me, Options, Index, Filename)

            If Data.ClusterList Is Nothing Then
                Result = False
            End If

            If Result Then
                Dim Cluster As UShort
                If Data.RequiresExpansion Then
                    Cluster = Disk.FAT.GetNextFreeCluster(Data.ClusterList, True)

                    If Cluster = 0 Then
                        Result = False
                    End If
                End If

                If Result Then
                    Dim UseTransaction As Boolean = Disk.BeginTransaction

                    If Data.RequiresExpansion Then
                        ExpandDirectorySize(Cluster)
                    End If

                    ProcessAddDirectory(Me, Data, EntryData)

                    If UseTransaction Then
                        Disk.EndTransaction()
                    End If
                End If
            End If

            Return Data
        End Function

        Public Overrides Function AddFile(FilePath As String, Options As AddFileOptions, Optional Index As Integer = -1) As Integer Implements IDirectory.AddFile
            Return AddFile(New IO.FileInfo(FilePath), Options, Index)
        End Function

        Public Overrides Function AddFile(FileInfo As IO.FileInfo, Options As AddFileOptions, Optional Index As Integer = -1) As Integer Implements IDirectory.AddFile
            If FileInfo.Length > Disk.Image.Length Then
                Return -1
            End If

            Dim Data = InitializeAddFile(Me, Options, Index, FileInfo)

            If Data.ClusterList Is Nothing Then
                Return -1
            End If

            Dim Cluster As UShort
            If Data.RequiresExpansion Then
                Cluster = Disk.FAT.GetNextFreeCluster(Data.ClusterList, True)

                If Cluster = 0 Then
                    Return -1
                End If
            End If

            Dim UseTransaction As Boolean = Disk.BeginTransaction

            If Data.RequiresExpansion Then
                ExpandDirectorySize(Cluster)
            End If

            ProcessAddFile(Me, Data)

            If UseTransaction Then
                Disk.EndTransaction()
            End If

            Return Data.EntriesNeeded
        End Function

        Public Function ExpandDirectorySize() As Boolean
            Dim ClusterList = Disk.FAT.GetFreeClusters(CUShort(1))

            If ClusterList Is Nothing Then
                Return False
            End If

            Dim Cluster = Disk.FAT.GetNextFreeCluster(ClusterList, True)

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
                Dim NewDirectoryEntry = New DirectoryEntry(_RootDirectory, Me, Offset, DirectoryEntries.Count, True)
                DirectoryEntries.Add(NewDirectoryEntry)
            Next

            Data.AvailableEntryCount += EntryCount

            If UseTransaction Then
                Disk.EndTransaction()
            End If
        End Sub

        Public Overrides Function GetContent() As Byte() Implements IDirectory.GetContent
            Return GetDataFromChain(Disk.Image, SectorChain)
        End Function

        Public Sub Initialize()
            Dim ClusterSize = Disk.BPB.BytesPerCluster
            Dim Entry As DirectoryEntryBase

            For Each Cluster In ParentEntry.FATChain.Clusters
                Dim OffsetStart As UInteger = Disk.BPB.ClusterToOffset(Cluster)

                Dim Buffer = New Byte(ClusterSize - 1) {}
                Disk.Image.SetBytes(Buffer, OffsetStart)
            Next

            Entry = New DirectoryEntryBase With {
                .StartingCluster = ParentEntry.StartingCluster,
                .FileNameWithExtension = DirectoryEntryBase.CurrentDirectoryEntry,
                .IsDirectory = True
            }
            SetEntryDates(Entry)
            DirectoryEntries.Item(0).Data = Entry.Data

            Dim StartingCluster As UShort = 0
            If ParentEntry.ParentDirectory.ParentEntry IsNot Nothing Then
                StartingCluster = ParentEntry.ParentDirectory.ParentEntry.StartingCluster
            End If

            Entry = New DirectoryEntryBase With {
                .StartingCluster = StartingCluster,
                .FileNameWithExtension = DirectoryEntryBase.ParentDirectoryEntry,
                .IsDirectory = True
            }
            SetEntryDates(Entry)
            DirectoryEntries.Item(1).Data = Entry.Data

            UpdateEntryCounts()
        End Sub

        Public Function ReduceDirectorySize() As Boolean
            If ClusterChain.Count < 2 Then
                Return False
            End If

            Dim Cluster = ClusterChain.Last

            If Not IsClusterEmpty(Cluster) Then
                Return False
            End If

            Dim UseTransaction As Boolean = Disk.BeginTransaction

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

            Data.AvailableEntryCount -= EntryCount

            If Data.EntryCount > DirectoryEntries.Count Then
                Data.EntryCount = DirectoryEntries.Count
            End If

            If UseTransaction Then
                Disk.EndTransaction()
            End If

            Return True
        End Function

        Public Overrides Function RemoveEntry(Index As UInteger) As Boolean Implements IDirectory.RemoveEntry
            Dim UseTransaction As Boolean = Disk.BeginTransaction

            Dim Result = MyBase.RemoveEntry(Index)

            If Result Then
                ReduceDirectorySize()
            End If

            If UseTransaction Then
                Disk.EndTransaction()
            End If

            Return Result
        End Function

        Public Overrides Function RemoveLFN(Index As UInteger) As Boolean Implements IDirectory.RemoveLFN
            Dim UseTransaction As Boolean = Disk.BeginTransaction

            Dim Result = MyBase.RemoveLFN(Index)

            If Result Then
                ReduceDirectorySize()
            End If

            If UseTransaction Then
                Disk.EndTransaction()
            End If

            Return Result
        End Function

        Public Overrides Function UpdateLFN(FileName As String, Index As Integer, UseNTExtensions As Boolean) As Boolean Implements IDirectory.UpdateLFN
            Dim Data = InitializeUpdateLFN(Me, FileName, Index, UseNTExtensions)

            Dim UseTransaction As Boolean = Disk.BeginTransaction

            Dim Result As Boolean = True

            If Data.RequiresExpansion Then
                Result = ExpandDirectorySize()
            End If

            If Result Then
                ProcessUpdateLFN(Me, Data)

                If Data.EntriesNeeded < 0 Then
                    ReduceDirectorySize()
                End If
            End If

            If UseTransaction Then
                Disk.EndTransaction()
            End If

            Return Result
        End Function

        Public Sub UpdateLinkDates()
            Dim Entry As DirectoryEntryBase

            For i = 0 To 1
                Entry = DirectoryEntries.Item(i).Clone
                SetEntryDates(Entry)
                DirectoryEntries.Item(i).Data = Entry.Data
            Next i
        End Sub

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

        Private Sub SetEntryDates(Entry As DirectoryEntryBase)
            Entry.LastAccessDate = ParentEntry.LastAccessDate
            Entry.LastWriteDate = ParentEntry.LastWriteDate
            Entry.LastWriteTime = ParentEntry.LastWriteTime
            Entry.CreationDate = ParentEntry.CreationDate
            Entry.CreationTime = ParentEntry.CreationTime
            Entry.CreationMillisecond = ParentEntry.CreationMillisecond
        End Sub
    End Class
End Namespace
