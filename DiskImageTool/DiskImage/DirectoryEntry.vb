Namespace DiskImage
    Public Class DirectoryEntry
        Inherits DirectoryEntryBase

        Private ReadOnly _BPB As BiosParameterBlock
        Private ReadOnly _Disk As Disk
        Private ReadOnly _Image As ImageByteArray
        Private ReadOnly _ParentDirectory As IDirectory
        Private ReadOnly _RootDirectory As RootDirectory
        Private _FatChain As FATChain
        Private _Index As UInteger
        Private _SubDirectory As SubDirectory

        Sub New(RootDirectory As RootDirectory, ParentDirectory As IDirectory, Offset As UInteger, Index As UInteger, EmptyEntry As Boolean)
            MyBase.New(RootDirectory.Disk.Image.Data, Offset)

            _RootDirectory = RootDirectory
            _ParentDirectory = ParentDirectory
            _Disk = _RootDirectory.Disk
            _BPB = _RootDirectory.Disk.BPB
            _Image = _RootDirectory.Disk.Image
            _Index = Index

            If Not EmptyEntry Then
                InitFatChain()
                InitSubDirectory()
            End If
        End Sub

        Public ReadOnly Property CrossLinks() As List(Of DirectoryEntry)
            Get
                Return _FatChain.CrossLinks
            End Get
        End Property

        Public ReadOnly Property Disk As Disk
            Get
                Return _Disk
            End Get
        End Property

        Public ReadOnly Property FATChain As FATChain
            Get
                Return _FatChain
            End Get
        End Property

        Public Property Index As UInteger
            Get
                Return _Index
            End Get
            Set(value As UInteger)
                _Index = value
            End Set
        End Property

        Public ReadOnly Property HasCircularChain As Boolean
            Get
                Return _FatChain.HasCircularChain
            End Get
        End Property

        Public ReadOnly Property ParentDirectory As IDirectory
            Get
                Return _ParentDirectory
            End Get
        End Property

        Public ReadOnly Property RootDirectory As RootDirectory
            Get
                Return _RootDirectory
            End Get
        End Property


        Public ReadOnly Property SubDirectory As SubDirectory
            Get
                Return _SubDirectory
            End Get
        End Property

        Public Function AddFile(FilePath As String, ShortFileName As String, WindowsAdditions As Boolean, Optional StartingCluster As UShort = 2) As Boolean
            Return AddFile(FilePath, ShortFileName, WindowsAdditions, Disk.FAT.FreeClusters, StartingCluster)
        End Function

        Public Function AddFile(FilePath As String, ShortFileName As String, WindowsAdditions As Boolean, ClusterList As SortedSet(Of UShort), Optional StartingCluster As UShort = 2)
            Dim FileInfo = New IO.FileInfo(FilePath)
            Dim ClusterSize = Disk.BPB.BytesPerCluster

            If FileInfo.Length > ClusterList.Count * ClusterSize Then
                Return False
            End If

            Dim UseTransaction As Boolean = _Disk.BeginTransaction

            Dim FirstCluster As UShort = 0

            If FileInfo.Length > 0 Then
                'Load file into buffer, padding with empty space if needed            
                Dim FileSize = Math.Ceiling(FileInfo.Length / ClusterSize) * ClusterSize
                Dim FileBuffer = ReadFileIntoBuffer(FileInfo, FileSize, 0)

                Dim LastCluster As UShort = 0

                For Counter As Integer = 0 To FileBuffer.Length - 1 Step ClusterSize
                    Dim Cluster = Disk.FAT.GetNextFreeCluster(ClusterList, True, StartingCluster)
                    If Cluster > 0 Then
                        If Counter = 0 Then
                            FirstCluster = Cluster
                        Else
                            Disk.FATTables.UpdateTableEntry(LastCluster, Cluster)
                        End If
                        Dim ClusterOffset = Disk.BPB.ClusterToOffset(Cluster)
                        Dim Buffer = Disk.Image.GetBytes(ClusterOffset, ClusterSize)
                        Array.Copy(FileBuffer, Counter, Buffer, 0, ClusterSize)
                        Disk.Image.SetBytes(Buffer, ClusterOffset)
                        LastCluster = Cluster
                        StartingCluster = Cluster + 1
                    End If
                Next

                If LastCluster > 0 Then
                    Disk.FATTables.UpdateTableEntry(LastCluster, FAT12.FAT_LAST_CLUSTER_END)
                End If
            End If

            Dim NewEntry = New DirectoryEntryBase
            NewEntry.SetFileInfo(FileInfo, ShortFileName, WindowsAdditions, WindowsAdditions)
            NewEntry.StartingCluster = FirstCluster

            Data = NewEntry.Data

            InitFatChain()

            If UseTransaction Then
                _Disk.EndTransaction()
            End If

            Return True
        End Function

        Public Function CanUnDelete() As Boolean
            If FileSize = 0 Or HasInvalidFileSize() Or HasInvalidStartingCluster() Then
                Return False
            End If

            Dim Size = _BPB.NumberOfFATEntries + 2

            Dim ClusterCount As UShort

            If IsDirectory() Then
                ClusterCount = 1
            Else
                ClusterCount = Math.Ceiling(FileSize / _BPB.BytesPerCluster)
            End If

            If StartingCluster + ClusterCount - 1 > Size Then
                Return False
            End If

            If ClusterCount > 0 Then
                For Index As UShort = 0 To ClusterCount - 1
                    Dim Cluster = StartingCluster + Index
                    If _Disk.FATTables.FAT.TableEntry(Cluster) <> FAT12.FAT_FREE_CLUSTER Then
                        Return False
                    End If
                Next
            End If

            Return True
        End Function

        Public Sub Delete(Clear As Boolean, FillChar As Byte)
            Dim UseTransaction As Boolean = _Disk.BeginTransaction

            _Image.SetBytes(CHAR_DELETED, Offset)

            Dim b(_BPB.BytesPerCluster - 1) As Byte
            If Clear Then
                For i = 0 To b.Length - 1
                    b(i) = FillChar
                Next
            End If

            For Each Cluster In _FatChain.Clusters
                If Clear Then
                    Dim Offset = _BPB.ClusterToOffset(Cluster)
                    _Image.SetBytes(b, Offset)
                End If

                _Disk.FATTables.UpdateTableEntry(Cluster, FAT12.FAT_FREE_CLUSTER)
            Next
            InitFatChain()

            If UseTransaction Then
                _Disk.EndTransaction()
            End If
        End Sub

        Public Function GetAllocatedSize() As UInteger
            Return Math.Ceiling(FileSize / _BPB.BytesPerCluster) * _BPB.BytesPerCluster
        End Function

        Public Function GetAllocatedSizeFromFAT() As UInteger
            Return _FatChain.Clusters.Count * _BPB.BytesPerCluster
        End Function

        Public Function GetChecksum() As UInteger
            Return Crc32.ComputeChecksum(GetContent)
        End Function

        Public Function GetContent() As Byte()
            Dim Size = FileSize
            Dim Content() As Byte

            If IsDeleted() Then
                ReDim Content(Size - 1)
                Dim Offset As UInteger = _BPB.ClusterToOffset(StartingCluster)
                If Offset + Size > _Image.Length Then
                    Size = Math.Max(_Image.Length - Offset, 0)
                End If
                If Size > 0 Then
                    _Image.CopyTo(Offset, Content, 0, Size)
                End If
            Else
                Content = GetDataFromChain(_Image.Data, ClusterListToSectorList(_BPB, _FatChain.Clusters))

                If Content.Length <> Size Then
                    Array.Resize(Of Byte)(Content, Size)
                End If
            End If

            Return Content
        End Function

        Public Function GetSizeOnDisk() As UInteger
            Return _FatChain.Clusters.Count * _BPB.BytesPerCluster
        End Function

        Public Function HasIncorrectFileSize() As Boolean
            If IsDirectory() Or IsVolumeName() Then
                Return FileSize > 0
            Else
                Return GetAllocatedSize() <> GetAllocatedSizeFromFAT()
            End If
        End Function

        Public Function HasInvalidFileSize() As Boolean
            Return FileSize > _BPB.ImageSize
        End Function

        Public Function HasInvalidStartingCluster() As Boolean
            Return StartingCluster = 1 Or StartingCluster > _BPB.NumberOfFATEntries + 1
        End Function

        Public Function HasLFN() As Boolean
            If _Index > 0 Then
                If _ParentDirectory.DirectoryEntries(_Index - 1).IsLFN Then
                    Return True
                End If
            End If

            Return False
        End Function

        Public Sub InitFatChain()
            _FatChain = _RootDirectory.FATAllocation.InitFATChain(Me)
        End Sub

        Public Function IsCrossLinked() As Boolean
            Return _FatChain.CrossLinks.Count > 0
        End Function

        Public Function IsModified() As Boolean
            If _Disk.RootDirectory.DirectoryCache.ContainsKey(Offset) Then
                Dim CacheEntry = _Disk.RootDirectory.DirectoryCache.Item(Offset)
                If Not Data.CompareTo(CacheEntry.Data) Then
                    Return True
                Else
                    Dim Checksum As UInteger = 0
                    If IsValidFile() Then
                        Checksum = GetChecksum()
                    End If
                    If Checksum <> CacheEntry.Checksum Then
                        Return True
                    End If
                End If
            Else
                Return True
            End If

            Return False
        End Function

        Public Function IsValid() As Boolean
            Return Not (HasInvalidFileSize() OrElse HasInvalidAttributes() OrElse HasInvalidStartingCluster())
        End Function

        Public Function IsValidDirectory() As Boolean
            Return IsDirectory() AndAlso Not (IsVolumeName() OrElse HasInvalidStartingCluster()) AndAlso StartingCluster > 1
        End Function

        Public Function IsValidFile() As Boolean
            Return Not (IsDirectory() OrElse IsVolumeName() OrElse HasInvalidStartingCluster() OrElse HasInvalidFileSize()) AndAlso StartingCluster > 1
        End Function

        Public Function IsValidVolumeName() As Boolean
            Return IsVolumeName() AndAlso Not (IsHidden() OrElse IsSystem() OrElse IsDirectory() OrElse IsDeleted()) AndAlso StartingCluster = 0
        End Function
        Public Sub UnDelete(FirstChar As Byte)
            Dim UseTransaction As Boolean = _Disk.BeginTransaction

            _Image.SetBytes(FirstChar, Offset)

            Dim ClusterCount As UShort

            If IsDirectory() Then
                ClusterCount = 1
            Else
                ClusterCount = Math.Ceiling(FileSize / _BPB.BytesPerCluster)
            End If

            If ClusterCount > 0 Then
                Dim TableEntry As UShort
                For Index As UShort = 0 To ClusterCount - 1
                    Dim Cluster = StartingCluster + Index
                    If Index < ClusterCount - 1 Then
                        TableEntry = Cluster + 1
                    Else
                        TableEntry = FAT12.FAT_LAST_CLUSTER_END
                    End If
                    _Disk.FATTables.UpdateTableEntry(Cluster, TableEntry)
                Next
                InitFatChain()
            End If

            If UseTransaction Then
                _Disk.EndTransaction()
            End If
        End Sub
        Public Function UpdateFile(FilePath As String, FileSize As UInteger, FillChar As Byte) As Boolean
            Return UpdateFile(FilePath, FileSize, FillChar, Disk.FAT.FreeClusters)
        End Function

        Public Function UpdateFile(FilePath As String, FileSize As UInteger, FillChar As Byte, ClusterList As SortedSet(Of UShort))
            Dim FileInfo = New IO.FileInfo(FilePath)
            Dim ClusterSize = Disk.BPB.BytesPerCluster

            Dim FileLength As Long

            If GetSizeOnDisk() > FileSize Then
                FileLength = 0
            Else
                FileLength = FileSize - GetSizeOnDisk()
            End If

            If FileLength > ClusterList.Count * ClusterSize Then
                Return False
            End If

            Dim UseTransaction As Boolean = _Disk.BeginTransaction

            'Load file into buffer, padding with empty space if needed            
            Dim FileBuffer = ReadFileIntoBuffer(FileInfo, FileSize, FillChar)

            Me.FileSize = FileSize

            Dim ClusterCount = FATChain.Clusters.Count
            Dim ClusterIndex As Integer = 0
            Dim Cluster As UShort
            Dim LastCluster As UShort = 0
            Dim FATUpdated As Boolean = False

            'Update assigned clusters and assign new ones if new file is larger
            For Counter As Integer = 0 To FileBuffer.Length - 1 Step ClusterSize
                If ClusterIndex < ClusterCount Then
                    Cluster = FATChain.Clusters(ClusterIndex)
                Else
                    Cluster = Disk.FAT.GetNextFreeCluster(ClusterList, True)
                    If Cluster = 0 Then
                        Exit For
                    End If
                    If LastCluster > 0 Then
                        Disk.FATTables.UpdateTableEntry(LastCluster, Cluster)
                        FATUpdated = True
                    End If
                End If

                Dim ClusterOffset = Disk.BPB.ClusterToOffset(Cluster)
                Dim Length = Math.Min(ClusterSize, FileBuffer.Length - Counter)
                Dim Buffer = Disk.Image.GetBytes(ClusterOffset, ClusterSize)
                Array.Copy(FileBuffer, Counter, Buffer, 0, Length)
                Disk.Image.SetBytes(Buffer, ClusterOffset)
                ClusterIndex += 1
                LastCluster = Cluster
            Next

            If LastCluster > 0 Then
                Disk.FATTables.UpdateTableEntry(LastCluster, FAT12.FAT_LAST_CLUSTER_END)
                FATUpdated = True
            End If

            'Free unused clusters if new file is smaller
            If ClusterIndex < ClusterCount Then
                Dim Buffer(ClusterSize - 1) As Byte
                For i = 0 To Buffer.Length - 1
                    Buffer(i) = FillChar
                Next

                For Index As UInteger = ClusterIndex To ClusterCount - 1
                    Cluster = FATChain.Clusters(Index)
                    Dim ClusterOffset = Disk.BPB.ClusterToOffset(Cluster)
                    Disk.Image.SetBytes(Buffer, ClusterOffset)

                    Disk.FATTables.UpdateTableEntry(Cluster, FAT12.FAT_FREE_CLUSTER)
                    FATUpdated = True
                Next
            End If

            If FATUpdated Then
                InitFatChain()
            End If

            If UseTransaction Then
                _Disk.EndTransaction()
            End If

            Return True
        End Function

        Private Sub InitSubDirectory()
            If IsValidDirectory() AndAlso Not IsLink() AndAlso Not IsDeleted() Then
                _SubDirectory = New SubDirectory(_RootDirectory, Me)
            Else
                _SubDirectory = Nothing
            End If
        End Sub
    End Class

End Namespace