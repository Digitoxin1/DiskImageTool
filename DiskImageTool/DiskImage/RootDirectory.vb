Namespace DiskImage
    Public Structure DirectoryCacheEntry
        Dim Checksum As UInteger
        Dim Data() As Byte
    End Structure

    Public Class RootDirectory
        Inherits DirectoryBase
        Implements IDirectory

        Private ReadOnly _DirectoryCache As Dictionary(Of UInteger, DirectoryCacheEntry)
        Private ReadOnly _FATAllocation As FATAllocation

        Sub New(Disk As Disk, FAT As FAT12)
            MyBase.New(Disk, Nothing)

            _DirectoryCache = New Dictionary(Of UInteger, DirectoryCacheEntry)
            _FATAllocation = New FATAllocation(FAT)

            If Disk.BPB.IsValid Then
                InitializeDirectoryData()
                CacheDirectoryEntries(Me)
            End If
        End Sub

        Public Overrides ReadOnly Property ClusterChain As List(Of UShort) Implements IDirectory.ClusterChain
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property DirectoryCache As Dictionary(Of UInteger, DirectoryCacheEntry)
            Get
                Return _DirectoryCache
            End Get
        End Property

        Public ReadOnly Property FATAllocation As FATAllocation
            Get
                Return _FATAllocation
            End Get
        End Property
        Public Overrides ReadOnly Property SectorChain As List(Of UInteger) Implements IDirectory.SectorChain
            Get
                Dim Chain = New List(Of UInteger)

                For Sector = Disk.BPB.RootDirectoryRegionStart To Disk.BPB.DataRegionStart - 1
                    Chain.Add(Sector)
                Next

                Return Chain
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

            Dim ShortFileName = GetAvailableFileName(FileInfo.Name)
            If WindowsAdditions Then
                LFNEntries = GetLFNDirectoryEntries(FileInfo.Name, ShortFileName)
                EntryCount += LFNEntries.Count
            End If

            If FileInfo.Length > ClusterList.Count * ClusterSize Then
                Return False
            End If

            If Index = -1 Then
                Entries = GetAvailableEntries(EntryCount)

                If Entries Is Nothing Then
                    Return False
                End If
            Else
                If DirectoryEntries.Count - Data.EntryCount < EntryCount Then
                    Return False
                End If

                Index = AdjustIndexForLFN(Index)
                Entries = GetEntries(Index, EntryCount)
            End If

            Dim UseTransaction As Boolean = Disk.BeginTransaction

            If Index > -1 Then
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

        Public Overrides Function GetContent() As Byte() Implements IDirectory.GetContent
            Dim SectorStart = Disk.BPB.RootDirectoryRegionStart
            Dim SectorEnd = Disk.BPB.DataRegionStart
            Dim Length = Disk.SectorToBytes(SectorEnd - SectorStart)
            Dim Offset = Disk.SectorToBytes(SectorStart)

            Return Disk.Image.GetBytes(Offset, Length)
        End Function

        Public Function GetFileList() As List(Of DirectoryEntry)
            Dim DirectoryList As New List(Of DirectoryEntry)

            EnumerateDirectoryEntries(Me, DirectoryList)

            Return DirectoryList
        End Function

        Public Function GetVolumeLabel() As DirectoryEntry
            Dim VolumeLabel As DirectoryEntry = Nothing

            If Data.EntryCount > 0 Then
                For Counter As UInteger = 0 To Data.EntryCount - 1
                    Dim File = DirectoryEntries.Item(Counter)
                    If File.IsValidVolumeName Then
                        VolumeLabel = File
                        Exit For
                    End If
                Next
            End If

            Return VolumeLabel
        End Function

        Public Sub RefreshCache()
            _DirectoryCache.Clear()

            If Disk.BPB.IsValid Then
                CacheDirectoryEntries(Me)
            End If
        End Sub

        Public Sub RefreshData()
            MyBase.Data.Clear()
            DirectoryEntries.Clear()
            _FATAllocation.Clear()

            If Disk.BPB.IsValid Then
                InitializeDirectoryData()
            End If
        End Sub

        Private Sub CacheDirectoryEntries(Directory As DiskImage.IDirectory)
            If Directory.Data.EntryCount > 0 Then
                For Counter = 0 To Directory.Data.EntryCount - 1
                    Dim DirectoryEntry = Directory.GetFile(Counter)

                    If Not DirectoryEntry.IsLink Then
                        _DirectoryCache.Item(DirectoryEntry.Offset) = GetDirectoryCacheEntry(DirectoryEntry)
                        If DirectoryEntry.IsDirectory And DirectoryEntry.SubDirectory IsNot Nothing Then
                            If DirectoryEntry.SubDirectory.Data.EntryCount > 0 Then
                                CacheDirectoryEntries(DirectoryEntry.SubDirectory)
                            End If
                        End If
                    End If
                Next
            End If
        End Sub

        Private Sub EnumerateDirectoryEntries(Directory As DiskImage.IDirectory, DirectoryList As List(Of DirectoryEntry))
            If Directory.Data.EntryCount > 0 Then
                For Counter = 0 To Directory.Data.EntryCount - 1
                    Dim DirectoryEntry = Directory.GetFile(Counter)

                    If Not DirectoryEntry.IsLink Then
                        DirectoryList.Add(DirectoryEntry)
                        If DirectoryEntry.IsDirectory And DirectoryEntry.SubDirectory IsNot Nothing Then
                            If DirectoryEntry.SubDirectory.Data.EntryCount > 0 Then
                                EnumerateDirectoryEntries(DirectoryEntry.SubDirectory, DirectoryList)
                            End If
                        End If
                    End If
                Next
            End If
        End Sub

        Private Function GetDirectoryCacheEntry(DirectoryEntry As DirectoryEntry) As DirectoryCacheEntry
            Dim CacheEntry As DirectoryCacheEntry

            CacheEntry.Data = DirectoryEntry.Data
            If DirectoryEntry.IsValidFile Then
                CacheEntry.Checksum = DirectoryEntry.GetChecksum()
            Else
                CacheEntry.Checksum = 0
            End If

            Return CacheEntry
        End Function

        Private Sub InitializeDirectoryData()
            Dim OffsetStart As UInteger = Disk.SectorToBytes(Disk.BPB.RootDirectoryRegionStart)
            Dim OffsetEnd As UInteger = Disk.SectorToBytes(Disk.BPB.DataRegionStart)

            DirectoryBase.GetDirectoryData(Me, Me, OffsetStart, OffsetEnd, True)
        End Sub
    End Class
End Namespace
