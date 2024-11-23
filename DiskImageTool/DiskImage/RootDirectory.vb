Namespace DiskImage
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

        Public Overrides Function AddDirectory(EntryData() As Byte, Options As AddFileOptions, Filename As String, Optional Index As Integer = -1) As AddFileData Implements IDirectory.AddDirectory
            Dim Data = InitializeAddDirectory(Me, Options, Index, Filename)

            If Data.ClusterList IsNot Nothing Then
                Dim UseTransaction As Boolean = Disk.BeginTransaction

                ProcessAddDirectory(Me, Data, EntryData)

                If UseTransaction Then
                    Disk.EndTransaction()
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

            Dim UseTransaction As Boolean = Disk.BeginTransaction

            ProcessAddFile(Me, Data)

            If UseTransaction Then
                Disk.EndTransaction()
            End If

            Return Data.EntriesNeeded
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

        Public Overrides Function UpdateLFN(FileName As String, Index As Integer, UseNTExtensions As Boolean) As Boolean Implements IDirectory.UpdateLFN
            Dim Data = InitializeUpdateLFN(Me, FileName, Index, UseNTExtensions)

            If Data.RequiresExpansion Then
                Return False
            End If

            Dim UseTransaction As Boolean = Disk.BeginTransaction

            ProcessUpdateLFN(Me, Data)

            If UseTransaction Then
                Disk.EndTransaction()
            End If

            Return True
        End Function

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
