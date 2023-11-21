Imports System.Text

Namespace DiskImage

    Public Class DirectoryEntry
        Public Const CHAR_DELETED As Byte = &HE5
        Public Const DIRECTORY_ENTRY_SIZE As Byte = 32
        Private Const CHAR_EMPTY As Byte = &H0
        Private Const CHAR_SPACE As Byte = &H20
        Private ReadOnly _BPB As BiosParameterBlock
        Private ReadOnly _FAT As FAT12
        Private ReadOnly _FatChain As FATChain
        Private ReadOnly _FileBytes As ImageByteArray
        Private ReadOnly _Offset As UInteger
        Private ReadOnly _DirectoryCache As Dictionary(Of UInteger, DirectoryCacheEntry)
        Private _SubDirectory As SubDirectory

        Public Enum AttributeFlags
            [ReadOnly] = 1
            Hidden = 2
            System = 4
            VolumeName = 8
            Directory = 16
            ArchiveFlag = 32
            LongFileName = 15
        End Enum

        Public Enum DirectoryEntryOffsets As UInteger
            FileName = 0
            Extension = 8
            Attributes = 11
            ReservedForWinNT = 12
            CreationMillisecond = 13
            CreationTime = 14
            CreationDate = 16
            LastAccessDate = 18
            ReservedForFAT32 = 20
            LastWriteTime = 22
            LastWriteDate = 24
            StartingCluster = 26
            FileSize = 28
        End Enum

        Public Enum DirectoryEntrySizes As UInteger
            FileName = 8
            Extension = 3
            Attributes = 1
            ReservedForWinNT = 1
            CreationMillisecond = 1
            CreationTime = 2
            CreationDate = 2
            LastAccessDate = 2
            ReservedForFAT32 = 2
            LastWriteTime = 2
            LastWriteDate = 2
            StartingCluster = 2
            FileSize = 4
        End Enum

        Public Enum LFNOffsets As UInteger
            Sequence = 0
            FilePart1 = 1
            Attributes = 11
            Type = 12
            Checksum = 13
            FilePart2 = 14
            StartingCluster = 26
            FilePart3 = 28
        End Enum

        Public Enum LFNSizes As UInteger
            Sequence = 1
            FilePart1 = 10
            Attributes = 1
            Type = 1
            Checksum = 1
            FilePart2 = 12
            StartingCluster = 2
            FilePart3 = 4
        End Enum

        Sub New(FileBytes As ImageByteArray, BPB As BiosParameterBlock, FAT As FAT12, DirectoryCache As Dictionary(Of UInteger, DirectoryCacheEntry), Offset As UInteger)
            _BPB = BPB
            _FAT = FAT
            _FileBytes = FileBytes
            _Offset = Offset
            _DirectoryCache = DirectoryCache

            If _FAT.FATChains.ContainsKey(Offset) Then
                _FatChain = _FAT.FATChains.Item(Offset)
            Else
                If IsDeleted() Then
                    _FatChain = _FAT.InitFATChain(Offset, 0)
                ElseIf IsDirectory() AndAlso IsLink() Then
                    _FatChain = _FAT.InitFATChain(Offset, 0)
                Else
                    _FatChain = _FAT.InitFATChain(Offset, StartingCluster)
                End If
            End If

            InitSubDirectory()
        End Sub
        Public Property Attributes() As Byte
            Get
                Return _FileBytes.GetByte(_Offset + DirectoryEntryOffsets.Attributes)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.Attributes)
            End Set
        End Property

        Public Property BatchEditMode() As Boolean
            Get
                Return _FileBytes.BatchEditMode
            End Get
            Set
                _FileBytes.BatchEditMode = Value
            End Set
        End Property

        Public Property CreationDate() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffsets.CreationDate)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.CreationDate)
            End Set
        End Property

        Public Property CreationMillisecond() As Byte
            Get
                Return _FileBytes.GetByte(_Offset + DirectoryEntryOffsets.CreationMillisecond)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.CreationMillisecond)
            End Set
        End Property

        Public Property CreationTime() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffsets.CreationTime)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.CreationTime)
            End Set
        End Property

        Public ReadOnly Property CrossLinks() As List(Of UInteger)
            Get
                Return _FatChain.CrossLinks
            End Get
        End Property

        Public ReadOnly Property Data As Byte()
            Get
                Return _FileBytes.GetBytes(_Offset, DIRECTORY_ENTRY_SIZE)
            End Get
        End Property

        Public Property Extension() As Byte()
            Get
                Return _FileBytes.GetBytes(_Offset + DirectoryEntryOffsets.Extension, DirectoryEntrySizes.Extension)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.Extension, DirectoryEntrySizes.Extension, CHAR_SPACE)
            End Set
        End Property

        Public ReadOnly Property FATChain As FATChain
            Get
                Return _FatChain
            End Get
        End Property

        Public Property FileName() As Byte()
            Get
                Return _FileBytes.GetBytes(_Offset + DirectoryEntryOffsets.FileName, DirectoryEntrySizes.FileName)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.FileName, DirectoryEntrySizes.FileName, CHAR_SPACE)
            End Set
        End Property

        Public Property FileSize() As UInteger
            Get
                Return _FileBytes.GetBytesInteger(_Offset + DirectoryEntryOffsets.FileSize)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.FileSize)
            End Set
        End Property

        Public ReadOnly Property HasCircularChain As Boolean
            Get
                Return _FatChain.HasCircularChain
            End Get
        End Property

        Public Property LastAccessDate() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffsets.LastAccessDate)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.LastAccessDate)
            End Set
        End Property

        Public Property LastWriteDate() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffsets.LastWriteDate)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.LastWriteDate)
            End Set
        End Property

        Public Property LastWriteTime() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffsets.LastWriteTime)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.LastWriteTime)
            End Set
        End Property

        Public ReadOnly Property LFNSequence As Byte
            Get
                Return _FileBytes.GetByte(_Offset + LFNOffsets.Sequence)
            End Get
        End Property

        Public ReadOnly Property Offset As UInteger
            Get
                Return _Offset
            End Get
        End Property

        Public Property ReservedForFAT32() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffsets.ReservedForFAT32)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.ReservedForFAT32)
            End Set
        End Property

        Public Property ReservedForWinNT() As Byte
            Get
                Return _FileBytes.GetByte(_Offset + DirectoryEntryOffsets.ReservedForWinNT)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.ReservedForWinNT)
            End Set
        End Property

        Public Property StartingCluster() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffsets.StartingCluster)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.StartingCluster)
            End Set
        End Property

        Public ReadOnly Property SubDirectory As SubDirectory
            Get
                Return _SubDirectory
            End Get
        End Property

        Public Sub Clear(FullClear As Boolean)
            Dim b(31) As Byte

            If FullClear Then
                For Counter = 0 To b.Length - 1
                    b(Counter) = 0
                Next
            Else
                b(0) = CHAR_DELETED
                For counter = 1 To 10
                    b(counter) = 32
                Next
                For Counter = 11 To b.Length - 1
                    b(Counter) = 0
                Next
            End If

            _FileBytes.SetBytes(b, _Offset)
        End Sub

        Public Sub ClearCreationDate()
            CreationDate = 0
            CreationTime = 0
            CreationMillisecond = 0
        End Sub

        Public Sub ClearLastAccessDate()
            LastAccessDate = 0
        End Sub

        Public Sub ClearLastWriteDate()
            LastWriteDate = 0
            LastWriteTime = 0
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
            Dim Content() As Byte

            If IsDeleted() Then
                Dim Offset As UInteger = _BPB.ClusterToOffset(StartingCluster)
                Dim Size = FileSize
                If Offset + Size > _FileBytes.Length Then
                    Size = _FileBytes.Length - Offset
                End If
                Content = _FileBytes.GetBytes(Offset, Size)
            Else
                Content = GetDataFromChain(_FileBytes, ClusterListToSectorList(_BPB, _FatChain.Clusters))

                If Content.Length <> FileSize Then
                    Array.Resize(Of Byte)(Content, FileSize)
                End If
            End If

            Return Content
        End Function

        Public Function GetCreationDate() As ExpandedDate
            Return ExpandDate(CreationDate, CreationTime, CreationMillisecond)
        End Function

        Public Function GetFileExtension() As String
            Return CodePage437ToUnicode(Extension).TrimEnd(" ")
        End Function

        Public Function GetFileName() As String
            Return CodePage437ToUnicode(FileName).TrimEnd(" ")
        End Function

        Public Function GetFullFileName() As String
            Dim File = GetFileName()
            Dim Ext = GetFileExtension()

            If Ext <> "" Then
                File = File & "." & Ext
            End If

            Return File
        End Function

        Public Function GetLastAccessDate() As ExpandedDate
            Return ExpandDate(LastAccessDate)
        End Function

        Public Function GetLastWriteDate() As ExpandedDate
            Return ExpandDate(LastWriteDate, LastWriteTime)
        End Function

        Public Function GetLFNFileName() As String
            If IsLFN() Then
                Dim Size As Integer = LFNSizes.FilePart1 + LFNSizes.FilePart2 + LFNSizes.FilePart3
                Dim LFN(Size - 1) As Byte

                _FileBytes.CopyTo(_Offset + LFNOffsets.FilePart1, LFN, 0, LFNSizes.FilePart1)
                _FileBytes.CopyTo(_Offset + LFNOffsets.FilePart2, LFN, LFNSizes.FilePart1, LFNSizes.FilePart2)
                _FileBytes.CopyTo(_Offset + LFNOffsets.FilePart3, LFN, LFNSizes.FilePart1 + LFNSizes.FilePart2, LFNSizes.FilePart3)

                Return Encoding.Unicode.GetString(LFN)
            Else
                Dim LFN(1) As Byte

                Return Encoding.Unicode.GetString(LFN)
            End If
        End Function

        Public Function GetVolumeName() As String
            Return (CodePage437ToUnicode(FileName) & CodePage437ToUnicode(Extension)).TrimEnd(" ")
        End Function

        Public Function HasCreationDate() As Boolean
            Return CreationDate <> 0 Or CreationTime <> 0 Or CreationMillisecond <> 0
        End Function

        Public Function HasIncorrectFileSize() As Boolean
            If IsDirectory() Or IsVolumeName() Then
                Return FileSize > 0
            Else
                Return GetAllocatedSize() <> GetAllocatedSizeFromFAT()
            End If
        End Function

        Public Function HasInvalidAttributes() As Boolean
            Return (Attributes > 63)
        End Function

        Public Function HasInvalidExtension() As Boolean
            Return Not CheckValidFileName(Extension, True, IsVolumeName())
        End Function

        Public Function HasInvalidFilename() As Boolean
            Return Not CheckValidFileName(FileName, False, IsVolumeName())
        End Function

        Public Function HasInvalidFileSize() As Boolean
            Return FileSize > _BPB.ImageSize
        End Function

        Public Function HasInvalidStartingCluster() As Boolean
            Return StartingCluster = 1 Or StartingCluster > _BPB.NumberOfFATEntries + 1
        End Function

        Public Function HasLastAccessDate() As Boolean
            Return LastAccessDate <> 0
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
        Public Function IsArchive() As Boolean
            Return (Attributes And AttributeFlags.ArchiveFlag) > 0
        End Function

        Public Function IsCrossLinked() As Boolean
            Return _FatChain.CrossLinks.Count > 0
        End Function

        Public Function IsCurrentLink() As Boolean
            Dim FilePart = _FileBytes.ToUInt16(_Offset)
            Return (FilePart = &H202E)
        End Function

        Public Function IsDeleted() As Boolean
            Return FileName(0) = CHAR_DELETED
        End Function

        Public Function IsDirectory() As Boolean
            Return (Attributes And AttributeFlags.Directory) > 0
        End Function

        Public Function IsEmpty() As Boolean
            Return FileName(0) = CHAR_EMPTY
        End Function

        Public Function IsHidden() As Boolean
            Return (Attributes And AttributeFlags.Hidden) > 0
        End Function

        Public Function IsLFN() As Boolean
            Return (Attributes And AttributeFlags.LongFileName) = AttributeFlags.LongFileName
        End Function

        Public Function IsLink() As Boolean
            Dim FilePart = _FileBytes.ToUInt16(_Offset)
            Return (FilePart = &H202E Or FilePart = &H2E2E)
        End Function
        Public Function IsModified() As Boolean
            If _DirectoryCache.ContainsKey(_Offset) Then
                Dim CacheEntry = _DirectoryCache.Item(_Offset)
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

        Public Function ISParentLink() As Boolean
            Dim FilePart = _FileBytes.ToUInt16(_Offset)
            Return (FilePart = &H2E2E)
        End Function
        Public Function IsReadOnly() As Boolean
            Return (Attributes And AttributeFlags.ReadOnly) > 0
        End Function

        Public Function IsSystem() As Boolean
            Return (Attributes And AttributeFlags.System) > 0
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

        Public Function IsValidValumeName() As Boolean
            Return IsVolumeName() AndAlso Not (IsHidden() OrElse IsSystem() OrElse IsDirectory() OrElse IsDeleted()) AndAlso StartingCluster = 0
        End Function
        Public Function IsVolumeName() As Boolean
            Return (Attributes And AttributeFlags.VolumeName) > 0
        End Function

        Public Sub Remove(Clear As Boolean)
            Dim UseBatchEditMode As Boolean = Not _FileBytes.BatchEditMode

            If UseBatchEditMode Then
                _FileBytes.BatchEditMode = True
            End If

            _FileBytes.SetBytes(CHAR_DELETED, _Offset)

            Dim b(_BPB.BytesPerCluster - 1) As Byte
            If Clear Then
                For i = 0 To b.Length - 1
                    b(i) = &HF6
                Next
            End If

            For Each Cluster In _FatChain.Clusters
                If Clear Then
                    Dim Offset = _BPB.ClusterToOffset(Cluster)
                    _FileBytes.SetBytes(b, Offset)
                End If

                _FAT.TableEntry(Cluster) = 0
            Next
            _FAT.UpdateFAT12(True)

            If UseBatchEditMode Then
                _FileBytes.BatchEditMode = False
            End If
        End Sub

        Public Sub SetCreationDate(Value As Date)
            CreationDate = DateToFATDate(Value)
            CreationTime = DateToFATTime(Value)
            CreationMillisecond = DateToFATMilliseconds(Value)
        End Sub

        Public Sub SetLastAccessDate(Value As Date)
            LastAccessDate = DateToFATDate(Value)
        End Sub

        Public Sub SetLastWriteDate(Value As Date)
            LastWriteDate = DateToFATDate(Value)
            LastWriteTime = DateToFATTime(Value)
        End Sub

        Private Shared Function CheckValidFileName(FileName() As Byte, IsExtension As Boolean, IsVolumeName As Boolean) As Boolean
            Dim Result As Boolean = True
            Dim C As Byte
            Dim SpaceAllowed As Boolean = True

            If FileName.Length = 0 Then
                Return IsExtension
            End If

            For Index = FileName.Length - 1 To 0 Step -1
                C = FileName(Index)
                If Not IsVolumeName And (C <> &H20 Or (Not IsExtension And Index = 0)) Then
                    SpaceAllowed = False
                End If

                If C = &H20 And SpaceAllowed Then
                    Result = True
                ElseIf IsVolumeName And (C = &H0 Or (Not IsExtension And Index = 1 And C = &H3)) Then
                    Result = True
                ElseIf C < &H21 Then
                    Result = False
                    Exit For
                ElseIf Not IsVolumeName And C > &H60 And C < &H7B Then
                    Result = False
                    Exit For
                ElseIf Not IsVolumeName And InvalidFileChars.Contains(C) Then
                    Result = False
                    Exit For
                End If
            Next

            Return Result
        End Function

        Private Shared Function DateToFATDate(D As Date) As UShort
            Dim FATDate As UShort = D.Year - 1980

            FATDate <<= 4
            FATDate += D.Month
            FATDate <<= 5
            FATDate += D.Day

            Return FATDate
        End Function

        Private Shared Function DateToFATMilliseconds(D As Date) As Byte
            Return D.Millisecond \ 10 + (D.Second Mod 2) * 100
        End Function

        Private Shared Function DateToFATTime(D As Date) As UShort
            Dim DTTime As UShort = D.Hour

            DTTime <<= 6
            DTTime += D.Minute
            DTTime <<= 5
            DTTime += D.Second \ 2

            Return DTTime
        End Function

        Private Sub InitSubDirectory()
            If IsValidDirectory() AndAlso Not IsLink() AndAlso Not IsDeleted() Then
                _SubDirectory = New SubDirectory(_FileBytes, _BPB, _FAT, _FatChain, _DirectoryCache)
            Else
                _SubDirectory = Nothing
            End If
        End Sub
    End Class

End Namespace