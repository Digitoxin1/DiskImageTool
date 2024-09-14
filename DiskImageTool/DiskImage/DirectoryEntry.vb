Namespace DiskImage

    Public Class DirectoryEntry
        Inherits DirectoryEntryBase

        Private ReadOnly _FileBytes As ImageByteArray
        Private ReadOnly _BPB As BiosParameterBlock
        Private ReadOnly _FATTables As FATTables
        Private ReadOnly _DirectoryCache As Dictionary(Of UInteger, DirectoryCacheEntry)
        Private _FatChain As FATChain
        Private _SubDirectory As SubDirectory

        Sub New(FileBytes As ImageByteArray, BPB As BiosParameterBlock, FATTables As FATTables, DirectoryCache As Dictionary(Of UInteger, DirectoryCacheEntry), Offset As UInteger)
            MyBase.New(FileBytes, Offset)

            _FileBytes = FileBytes
            _BPB = BPB
            _FATTables = FATTables
            _DirectoryCache = DirectoryCache

            InitFatChain()
            InitSubDirectory()
        End Sub

        Public Property BatchEditMode() As Boolean
            Get
                Return _FileBytes.BatchEditMode
            End Get
            Set
                _FileBytes.BatchEditMode = Value
            End Set
        End Property

        Public ReadOnly Property CrossLinks() As List(Of UInteger)
            Get
                Return _FatChain.CrossLinks
            End Get
        End Property

        Public ReadOnly Property FATChain As FATChain
            Get
                Return _FatChain
            End Get
        End Property

        Public ReadOnly Property HasCircularChain As Boolean
            Get
                Return _FatChain.HasCircularChain
            End Get
        End Property

        Public ReadOnly Property SubDirectory As SubDirectory
            Get
                Return _SubDirectory
            End Get
        End Property

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
                If Offset + Size > _FileBytes.Length Then
                    Size = Math.Max(_FileBytes.Length - Offset, 0)
                End If
                If Size > 0 Then
                    _FileBytes.CopyTo(Offset, Content, 0, Size)
                End If
            Else
                Content = GetDataFromChain(_FileBytes, ClusterListToSectorList(_BPB, _FatChain.Clusters))

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

        Public Function HasVendorExceptions() As Boolean

            If Attributes = &HF7 AndAlso ReservedForWinNT = &H7F Then       'ORIGIN Systems, Inc.
                Return True
            ElseIf Attributes = &HDB AndAlso ReservedForWinNT = &H6D Then   'Psygnosis
                Return True
            ElseIf Attributes = &HB6 AndAlso ReservedForWinNT = &HDB Then   'Psygnosis
                Return True
            ElseIf Attributes = &H6D AndAlso ReservedForWinNT = &HB6 Then   'Psygnosis
                Return True
            End If

            Return False
        End Function

        Public Function IsCrossLinked() As Boolean
            Return _FatChain.CrossLinks.Count > 0
        End Function

        Public Function IsInRoot() As Boolean
            Return _BPB.OffsetToCluster(Offset) = 0
        End Function


        Public Function IsModified() As Boolean
            If _DirectoryCache.ContainsKey(Offset) Then
                Dim CacheEntry = _DirectoryCache.Item(Offset)
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

        Public Sub Remove(Clear As Boolean, FillChar As Byte)
            Dim UseBatchEditMode As Boolean = Not _FileBytes.BatchEditMode

            If UseBatchEditMode Then
                _FileBytes.BatchEditMode = True
            End If

            _FileBytes.SetBytes(CHAR_DELETED, Offset)

            Dim b(_BPB.BytesPerCluster - 1) As Byte
            If Clear Then
                For i = 0 To b.Length - 1
                    b(i) = FillChar
                Next
            End If

            For Each Cluster In _FatChain.Clusters
                If Clear Then
                    Dim Offset = _BPB.ClusterToOffset(Cluster)
                    _FileBytes.SetBytes(b, Offset)
                End If

                _FATTables.UpdateTableEntry(Cluster, FAT12.FAT_FREE_CLUSTER)
            Next
            _FATTables.UpdateFAT12()
            InitFatChain()

            If UseBatchEditMode Then
                _FileBytes.BatchEditMode = False
            End If
        End Sub

        Public Function CanRestore() As Boolean
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
                    If _FATTables.FAT.TableEntry(Cluster) <> FAT12.FAT_FREE_CLUSTER Then
                        Return False
                    End If
                Next
            End If

            Return True
        End Function

        Public Sub Restore(FirstChar As Byte)
            Dim UseBatchEditMode As Boolean = Not _FileBytes.BatchEditMode

            If UseBatchEditMode Then
                _FileBytes.BatchEditMode = True
            End If

            _FileBytes.SetBytes(FirstChar, Offset)

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
                    _FATTables.UpdateTableEntry(Cluster, TableEntry)
                Next
                _FATTables.UpdateFAT12()
                InitFatChain()
            End If

            If UseBatchEditMode Then
                _FileBytes.BatchEditMode = False
            End If
        End Sub

        Private Sub InitFatChain()
            Dim FAT = _FATTables.FAT

            If FAT.FATChains.ContainsKey(Offset) Then
                _FatChain = FAT.FATChains.Item(Offset)
            Else
                If IsDeleted() Then
                    _FatChain = FAT.InitFATChain(Offset, 0)
                ElseIf IsDirectory() AndAlso IsLink() Then
                    _FatChain = FAT.InitFATChain(Offset, 0)
                Else
                    _FatChain = FAT.InitFATChain(Offset, StartingCluster)
                End If
            End If
        End Sub

        Private Sub InitSubDirectory()
            If IsValidDirectory() AndAlso Not IsLink() AndAlso Not IsDeleted() Then
                _SubDirectory = New SubDirectory(_FileBytes, _BPB, _FATTables, _FatChain, _DirectoryCache)
            Else
                _SubDirectory = Nothing
            End If
        End Sub
    End Class

End Namespace