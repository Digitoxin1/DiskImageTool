Imports System.Text

Namespace DiskImage

    Public Class DirectoryEntry
        Private Const CHAR_SPACE As Byte = 32
        Private Const CHAR_DELETED As Byte = &HE5
        Private ReadOnly _FatChain As FATChain
        Private ReadOnly _FAT As FAT12
        Private ReadOnly _Offset As UInteger
        Private ReadOnly _BootSector As BootSector
        Private ReadOnly _FileBytes As ImageByteArray
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

        Private Enum DirectoryEntryOffset As UInteger
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
            LFNSequence = 0
            LFNFilePart1 = 1
            LFNFilePart2 = 14
            LFNFilePart3 = 28
        End Enum

        Private Enum DirectoryEntrySize As UInteger
            FileName = 8
            Extension = 3
            LFNFilePart1 = 10
            LFNFilePart2 = 12
            LFNFilePart3 = 4
        End Enum

        Sub New(FileBytes As ImageByteArray, BootSector As BootSector, FAT As FAT12, Offset As UInteger)
            _BootSector = BootSector
            _FAT = FAT
            _FileBytes = FileBytes
            _Offset = Offset

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
                Return _FileBytes.GetByte(_Offset + DirectoryEntryOffset.Attributes)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffset.Attributes)
            End Set
        End Property

        Public Property CreationDate() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffset.CreationDate)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffset.CreationDate)
            End Set
        End Property

        Public Property CreationMillisecond() As Byte
            Get
                Return _FileBytes.GetByte(_Offset + DirectoryEntryOffset.CreationMillisecond)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffset.CreationMillisecond)
            End Set
        End Property

        Public Property CreationTime() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffset.CreationTime)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffset.CreationTime)
            End Set
        End Property

        Public ReadOnly Property CrossLinks() As List(Of UInteger)
            Get
                Return _FatChain.CrossLinks
            End Get
        End Property

        Public ReadOnly Property Data As Byte()
            Get
                Return _FileBytes.GetBytes(_Offset, 32)
            End Get
        End Property

        Public Property Extension() As Byte()
            Get
                Return _FileBytes.GetBytes(_Offset + DirectoryEntryOffset.Extension, DirectoryEntrySize.Extension)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffset.Extension, DirectoryEntrySize.Extension, CHAR_SPACE)
            End Set
        End Property

        Public ReadOnly Property FATChain As FATChain
            Get
                Return _FatChain
            End Get
        End Property

        Public Property FileName() As Byte()
            Get
                Return _FileBytes.GetBytes(_Offset + DirectoryEntryOffset.FileName, DirectoryEntrySize.FileName)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffset.FileName, DirectoryEntrySize.FileName, CHAR_SPACE)
            End Set
        End Property

        Public Property FileSize() As UInteger
            Get
                Return _FileBytes.GetBytesInteger(_Offset + DirectoryEntryOffset.FileSize)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffset.FileSize)
            End Set
        End Property

        Public ReadOnly Property HasCircularChain As Boolean
            Get
                Return _FatChain.HasCircularChain
            End Get
        End Property

        Public Property LastAccessDate() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffset.LastAccessDate)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffset.LastAccessDate)
            End Set
        End Property

        Public Property LastWriteDate() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffset.LastWriteDate)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffset.LastWriteDate)
            End Set
        End Property

        Public Property LastWriteTime() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffset.LastWriteTime)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffset.LastWriteTime)
            End Set
        End Property

        Public ReadOnly Property LFNSequence As Byte
            Get
                Return _FileBytes.GetByte(_Offset + DirectoryEntryOffset.LFNSequence)
            End Get
        End Property

        Public ReadOnly Property Offset As Integer
            Get
                Return _Offset
            End Get
        End Property

        Public Property ReservedForFAT32() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffset.ReservedForFAT32)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffset.ReservedForFAT32)
            End Set
        End Property

        Public Property ReservedForWinNT() As Byte
            Get
                Return _FileBytes.GetByte(_Offset + DirectoryEntryOffset.ReservedForWinNT)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffset.ReservedForWinNT)
            End Set
        End Property

        Public Property StartingCluster() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffset.StartingCluster)
            End Get
            Set
                _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffset.StartingCluster)
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
                For Counter = 1 To b.Length - 1
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

        Public Function GetContent() As Byte()
            Dim Content = GetDataFromChain(_FileBytes, _FatChain.Sectors)

            If Content.Length <> FileSize Then
                Array.Resize(Of Byte)(Content, FileSize)
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
                Dim Size As Integer = DirectoryEntrySize.LFNFilePart1 + DirectoryEntrySize.LFNFilePart2 + DirectoryEntrySize.LFNFilePart3
                Dim LFN(Size - 1) As Byte

                _FileBytes.CopyTo(_Offset + DirectoryEntryOffset.LFNFilePart1, LFN, 0, DirectoryEntrySize.LFNFilePart1)
                _FileBytes.CopyTo(_Offset + DirectoryEntryOffset.LFNFilePart2, LFN, DirectoryEntrySize.LFNFilePart1, DirectoryEntrySize.LFNFilePart2)
                _FileBytes.CopyTo(_Offset + DirectoryEntryOffset.LFNFilePart3, LFN, DirectoryEntrySize.LFNFilePart1 + DirectoryEntrySize.LFNFilePart2, DirectoryEntrySize.LFNFilePart3)

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
            If IsDirectory() Then
                Return False
            End If

            Dim AllocatedSize As UInteger = Math.Ceiling(FileSize / _BootSector.BytesPerCluster) * _BootSector.BytesPerCluster
            Dim AllocatedSizeFromFAT As UInteger = _FatChain.Clusters.Count * _BootSector.BytesPerCluster

            Return AllocatedSize <> AllocatedSizeFromFAT
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
            Return FileSize > _BootSector.ImageSize
        End Function

        Public Function HasInvalidStartingCluster() As Boolean
            Return StartingCluster = 1 Or StartingCluster > _BootSector.NumberOfFATEntries + 1
        End Function

        Public Function HasLastAccessDate() As Boolean
            Return LastAccessDate <> 0
        End Function

        Public Function IsArchive() As Boolean
            Return (Attributes And AttributeFlags.ArchiveFlag) > 0
        End Function

        Public Function IsCrossLinked() As Boolean
            Return _FatChain.CrossLinks.Count > 0
        End Function

        Public Function IsDeleted() As Boolean
            Return FileName(0) = CHAR_DELETED
        End Function

        Public Function IsDirectory() As Boolean
            Return (Attributes And AttributeFlags.Directory) > 0
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

        Public Function ISParentLink() As Boolean
            Dim FilePart = _FileBytes.ToUInt16(_Offset)
            Return (FilePart = &H2E2E)
        End Function

        Public Function IsModified() As Boolean
            Return _FileBytes.DirectoryCache.ContainsKey(_Offset) AndAlso Not ByteArrayCompare(Data, _FileBytes.DirectoryCache.Item(_Offset))
        End Function

        Public Function IsReadOnly() As Boolean
            Return (Attributes And AttributeFlags.ReadOnly) > 0
        End Function

        Public Function IsSystem() As Boolean
            Return (Attributes And AttributeFlags.System) > 0
        End Function

        Public Function IsVolumeName() As Boolean
            Return (Attributes And AttributeFlags.VolumeName) > 0
        End Function

        Public Sub Remove(Clear As Boolean)
            _FileBytes.BatchEditMode = True
            _FileBytes.SetBytes(CHAR_DELETED, _Offset)

            Dim b(_BootSector.BytesPerCluster - 1) As Byte
            If Clear Then
                For i = 0 To b.Length - 1
                    b(i) = &HF6
                Next
            End If

            For Each Cluster In _FatChain.Clusters
                If Clear Then
                    Dim Offset = _BootSector.ClusterToOffset(Cluster)
                    _FileBytes.SetBytes(b, Offset)
                End If

                _FAT.TableEntry(Cluster) = 0
            Next
            _FAT.UpdateFAT12(True)

            _FileBytes.BatchEditMode = False
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

        Private Sub InitSubDirectory()
            If IsDirectory() And Not IsDeleted() Then
                _SubDirectory = New SubDirectory(_FileBytes, _BootSector, _FAT, _FatChain, FileSize)
            Else
                _SubDirectory = Nothing
            End If
        End Sub
    End Class

End Namespace