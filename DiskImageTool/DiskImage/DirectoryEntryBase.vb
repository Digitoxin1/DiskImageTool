﻿Imports System.IO
Imports System.Text

Namespace DiskImage
    Public Class DirectoryEntryBase
        Public Const CHAR_DELETED As Byte = &HE5
        Public Const DIRECTORY_ENTRY_SIZE As Byte = 32
        Public Shared ReadOnly EmptyDirectoryEntry() As Byte = {&HE5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        Public Shared ReadOnly CurrentDirectoryEntry() As Byte = {&H2E, &H20, &H20, &H20, &H20, &H20, &H20, &H20, &H20, &H20, &H20}
        Public Shared ReadOnly ParentDirectoryEntry() As Byte = {&H2E, &H2E, &H20, &H20, &H20, &H20, &H20, &H20, &H20, &H20, &H20}

        Private Const CHAR_EMPTY As Byte = &H0
        Private Const CHAR_SPACE As Byte = &H20

        Private ReadOnly _FileBytes As IByteArray

        Private _CreationDateCache As ExpandedDate
        Private _CreationDateIsCached As Boolean = False
        Private _FileExtensionCache As String
        Private _FileExtensionIsCached As Boolean = False
        Private _FileNameCache As String
        Private _FileNameIsCached As Boolean = False
        Private _HasInvalidExtensionCache As Boolean? = Nothing
        Private _HasInvalidFileNameCache As Boolean? = Nothing
        Private _IsBlankCache As Boolean? = Nothing
        Private _LastAccessDateCache As ExpandedDate
        Private _LastAccessDateIsCached As Boolean = False
        Private _LastWriteDateCache As ExpandedDate
        Private _LastWriteDateIsCached As Boolean = False
        Private _Offset As UInteger
        Private _VolumeNameCache As String
        Private _VolumeNameIsCached As Boolean = False
        Public Enum AttributeFlags As Byte
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
            FileNameAndExtension = 11
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

        Sub New()
            Dim Data = FillArray(DIRECTORY_ENTRY_SIZE, 0)
            _FileBytes = New ByteArray(Data)
            _Offset = 0
        End Sub

        Sub New(Data() As Byte)
            _FileBytes = New ByteArray(Data)
            _Offset = 0
        End Sub

        Sub New(FileBytes As IByteArray, Offset As UInteger)
            _FileBytes = FileBytes
            _Offset = Offset
        End Sub

        Public Property Attributes() As Byte
            Get
                Return _FileBytes.GetByte(_Offset + DirectoryEntryOffsets.Attributes)
            End Get
            Set
                If _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.Attributes) Then
                    _IsBlankCache = Nothing
                End If
            End Set
        End Property

        Public Property CreationDate() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffsets.CreationDate)
            End Get
            Set
                If _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.CreationDate) Then
                    _IsBlankCache = Nothing
                    _CreationDateIsCached = False
                End If
            End Set
        End Property

        Public Property CreationMillisecond() As Byte
            Get
                Return _FileBytes.GetByte(_Offset + DirectoryEntryOffsets.CreationMillisecond)
            End Get
            Set
                If _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.CreationMillisecond) Then
                    _IsBlankCache = Nothing
                    _CreationDateIsCached = False
                End If
            End Set
        End Property

        Public Property CreationTime() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffsets.CreationTime)
            End Get
            Set
                If _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.CreationTime) Then
                    _IsBlankCache = Nothing
                    _CreationDateIsCached = False
                End If
            End Set
        End Property

        Public Property Data As Byte()
            Get
                Return _FileBytes.GetBytes(_Offset, DIRECTORY_ENTRY_SIZE)
            End Get
            Set(value As Byte())
                If _FileBytes.SetBytes(value, _Offset, DIRECTORY_ENTRY_SIZE, 0) Then
                    ClearCache()
                End If
            End Set
        End Property

        Public Property Extension() As Byte()
            Get
                Return _FileBytes.GetBytes(_Offset + DirectoryEntryOffsets.Extension, DirectoryEntrySizes.Extension)
            End Get
            Set
                If _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.Extension, DirectoryEntrySizes.Extension, CHAR_SPACE) Then
                    _IsBlankCache = Nothing
                    _VolumeNameIsCached = False
                    _FileExtensionIsCached = False
                    _HasInvalidExtensionCache = Nothing
                End If
            End Set
        End Property

        Public Property FileName() As Byte()
            Get
                Return _FileBytes.GetBytes(_Offset + DirectoryEntryOffsets.FileName, DirectoryEntrySizes.FileName)
            End Get
            Set
                If _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.FileName, DirectoryEntrySizes.FileName, CHAR_SPACE) Then
                    _IsBlankCache = Nothing
                    _VolumeNameIsCached = False
                    _FileNameIsCached = False
                    _HasInvalidFileNameCache = Nothing
                End If
            End Set
        End Property

        Public Property FileNameWithExtension() As Byte()
            Get
                Return _FileBytes.GetBytes(_Offset + DirectoryEntryOffsets.FileName, DirectoryEntrySizes.FileName + DirectoryEntrySizes.Extension)
            End Get
            Set
                If _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.FileName, DirectoryEntrySizes.FileName + DirectoryEntrySizes.Extension, CHAR_SPACE) Then
                    _IsBlankCache = Nothing
                    _VolumeNameIsCached = False
                    _FileNameIsCached = False
                    _HasInvalidFileNameCache = Nothing
                End If
            End Set
        End Property

        Public Property FileSize() As UInteger
            Get
                Return _FileBytes.GetBytesInteger(_Offset + DirectoryEntryOffsets.FileSize)
            End Get
            Set
                If _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.FileSize) Then
                    _IsBlankCache = Nothing
                End If
            End Set
        End Property

        Public Property LastAccessDate() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffsets.LastAccessDate)
            End Get
            Set
                If _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.LastAccessDate) Then
                    _IsBlankCache = Nothing
                    _LastAccessDateIsCached = False
                End If
            End Set
        End Property

        Public Property LastWriteDate() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffsets.LastWriteDate)
            End Get
            Set
                If _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.LastWriteDate) Then
                    _IsBlankCache = Nothing
                    _LastWriteDateIsCached = False
                End If
            End Set
        End Property

        Public Property LastWriteTime() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffsets.LastWriteTime)
            End Get
            Set
                If _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.LastWriteTime) Then
                    _IsBlankCache = Nothing
                    _LastWriteDateIsCached = False
                End If
            End Set
        End Property

        Public ReadOnly Property LFNSequence As Byte
            Get
                Return _FileBytes.GetByte(_Offset + LFNOffsets.Sequence)
            End Get
        End Property

        Public Property Offset As UInteger
            Get
                Return _Offset
            End Get
            Set(value As UInteger)
                If _Offset <> value Then
                    Dim CurrentData = Data
                    _Offset = value
                    Data = CurrentData
                End If
            End Set
        End Property

        Public Property ReservedForFAT32() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffsets.ReservedForFAT32)
            End Get
            Set
                If _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.ReservedForFAT32) Then
                    _IsBlankCache = Nothing
                End If
            End Set
        End Property

        Public Property ReservedForWinNT() As Byte
            Get
                Return _FileBytes.GetByte(_Offset + DirectoryEntryOffsets.ReservedForWinNT)
            End Get
            Set
                If _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.ReservedForWinNT) Then
                    _IsBlankCache = Nothing
                End If
            End Set
        End Property

        Public Property StartingCluster() As UShort
            Get
                Return _FileBytes.GetBytesShort(_Offset + DirectoryEntryOffsets.StartingCluster)
            End Get
            Set
                If _FileBytes.SetBytes(Value, _Offset + DirectoryEntryOffsets.StartingCluster) Then
                    _IsBlankCache = Nothing
                End If
            End Set
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

            If _FileBytes.SetBytes(b, _Offset) Then
                ClearCache()
            End If
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

        Public Function Clone() As DirectoryEntryBase
            Return New DirectoryEntryBase(Data)
        End Function

        Public Function GetCreationDate() As ExpandedDate
            If Not _CreationDateIsCached Then
                _CreationDateCache = ExpandDate(CreationDate, CreationTime, CreationMillisecond)
                _CreationDateIsCached = True
            End If

            Return _CreationDateCache
        End Function

        Public Function GetFileExtension() As String
            If Not _FileExtensionIsCached Then
                _FileExtensionCache = CodePage437ToUnicode(Extension).TrimEnd(" ")
                _FileExtensionIsCached = True
            End If

            Return _FileExtensionCache
        End Function

        Public Function GetFileName() As String
            If Not _FileNameIsCached Then
                _FileNameCache = CodePage437ToUnicode(FileName).TrimEnd(" ")
                _FileNameIsCached = True
            End If

            Return _FileNameCache
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
            If Not _LastAccessDateIsCached Then
                _LastAccessDateCache = ExpandDate(LastAccessDate)
                _LastAccessDateIsCached = True
            End If

            Return _LastAccessDateCache
        End Function

        Public Function GetLastWriteDate() As ExpandedDate
            If Not _LastWriteDateIsCached Then
                _LastWriteDateCache = ExpandDate(LastWriteDate, LastWriteTime)
                _LastWriteDateIsCached = True
            End If

            Return _LastWriteDateCache
        End Function

        Public Function GetLFNChecksum() As Byte
            Dim pFcbName = _FileBytes.GetBytes(_Offset + DirectoryEntryOffsets.FileName, DirectoryEntrySizes.FileNameAndExtension)

            Dim Sum As Byte = 0

            For Index = 0 To 10
                Sum = ((If((Sum And 1) <> 0, &H80, 0)) + (Sum >> 1) + pFcbName(Index)) And &HFF
            Next

            Return Sum
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
            If Not _VolumeNameIsCached Then
                _VolumeNameCache = CodePage437ToUnicode(FileNameWithExtension).TrimEnd(" ")
                _VolumeNameIsCached = True
            End If

            Return _VolumeNameCache
        End Function

        Public Function HasCreationDate() As Boolean
            Return CreationDate <> 0 Or CreationTime <> 0 Or CreationMillisecond <> 0
        End Function

        Public Function HasInvalidAttributes() As Boolean
            Return (Attributes > 63)
        End Function

        Public Function HasInvalidExtension() As Boolean
            If Not _HasInvalidExtensionCache.HasValue Then
                _HasInvalidExtensionCache = Not CheckValidFileName(Extension, True, IsVolumeName())
            End If

            Return _HasInvalidExtensionCache
        End Function

        Public Function HasInvalidFilename() As Boolean
            If Not _HasInvalidFileNameCache.HasValue Then
                _HasInvalidFileNameCache = Not CheckValidFileName(FileName, False, IsVolumeName())
            End If

            Return _HasInvalidFileNameCache
        End Function

        Public Function HasLastAccessDate() As Boolean
            Return LastAccessDate <> 0
        End Function

        Public Function IsArchive() As Boolean
            Return (Attributes And AttributeFlags.ArchiveFlag) > 0
        End Function

        Public Function IsBlank() As Boolean
            If _IsBlankCache.HasValue Then
                Return _IsBlankCache.Value
            End If

            Dim Data = Me.Data
            Dim CheckByte = Data(1)
            If Data(0) = CHAR_DELETED And (CheckByte = &H0 Or CheckByte = &HF6) Then
                For Counter = 2 To Data.Length - 1
                    If Data(Counter) <> CheckByte Then
                        _IsBlankCache = False
                        Return False
                    End If
                Next
                _IsBlankCache = True
                Return True
            End If

            _IsBlankCache = False
            Return False
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

        Public Function IsValidVolumeName() As Boolean
            Return IsVolumeName() AndAlso Not (IsHidden() OrElse IsSystem() OrElse IsDirectory() OrElse IsDeleted()) AndAlso StartingCluster = 0
        End Function

        Public Function IsVolumeName() As Boolean
            Return (Attributes And AttributeFlags.VolumeName) > 0
        End Function

        Public Sub SetCreationDate(Value As Date)
            CreationDate = DateToFATDate(Value)
            CreationTime = DateToFATTime(Value)
            CreationMillisecond = DateToFATMilliseconds(Value)
        End Sub

        Public Sub SetFileInfo(FileInfo As FileInfo, UseCreationDate As Boolean, UseLastAccessDate As Boolean)
            Dim NewFileName = DOSTruncateFileName(FileInfo.Name)

            SetFileInfo(FileInfo, NewFileName, UseCreationDate, UseLastAccessDate)
        End Sub

        Public Sub SetFileInfo(FileInfo As FileInfo, NewFileName As String, UseCreationDate As Boolean, UseLastAccessDate As Boolean)
            SetFileName(NewFileName)

            FileSize = FileInfo.Length

            SetLastWriteDate(FileInfo.LastWriteTime)

            If UseCreationDate Then
                SetCreationDate(FileInfo.CreationTime)
            End If

            If UseLastAccessDate Then
                SetLastAccessDate(FileInfo.LastAccessTime)
            End If

            Dim Attrib As AttributeFlags = 0
            If (FileInfo.Attributes And FileAttributes.Archive) > 0 Then
                Attrib = Attrib Or AttributeFlags.ArchiveFlag
            End If
            If (FileInfo.Attributes And FileAttributes.ReadOnly) > 0 Then
                Attrib = Attrib Or AttributeFlags.ReadOnly
            End If
            If (FileInfo.Attributes And FileAttributes.System) > 0 Then
                Attrib = Attrib Or AttributeFlags.System
            End If
            If (FileInfo.Attributes And FileAttributes.Hidden) > 0 Then
                Attrib = Attrib Or AttributeFlags.Hidden
            End If
            If (FileInfo.Attributes And FileAttributes.Directory) > 0 Then
                Attrib = Attrib Or AttributeFlags.Directory
            End If

            Attributes = Attrib
        End Sub

        Public Sub SetFileInfo(FolderInfo As DirectoryInfo, NewFileName As String, UseCreationDate As Boolean, UseLastAccessDate As Boolean)
            SetFileName(NewFileName)

            FileSize = 0

            SetLastWriteDate(FolderInfo.LastWriteTime)

            If UseCreationDate Then
                SetCreationDate(FolderInfo.CreationTime)
            End If

            If UseLastAccessDate Then
                SetLastAccessDate(FolderInfo.LastAccessTime)
            End If

            Dim Attrib As AttributeFlags = 0
            If (FolderInfo.Attributes And FileAttributes.Archive) > 0 Then
                Attrib = Attrib Or AttributeFlags.ArchiveFlag
            End If
            If (FolderInfo.Attributes And FileAttributes.ReadOnly) > 0 Then
                Attrib = Attrib Or AttributeFlags.ReadOnly
            End If
            If (FolderInfo.Attributes And FileAttributes.System) > 0 Then
                Attrib = Attrib Or AttributeFlags.System
            End If
            If (FolderInfo.Attributes And FileAttributes.Hidden) > 0 Then
                Attrib = Attrib Or AttributeFlags.Hidden
            End If
            If (FolderInfo.Attributes And FileAttributes.Directory) > 0 Then
                Attrib = Attrib Or AttributeFlags.Directory
            End If

            Attributes = Attrib
        End Sub

        Public Sub SetFileName(Value As String)
            Dim FileName = Path.GetFileNameWithoutExtension(Value)
            Dim FileExt = Path.GetExtension(Value)

            If FileExt.Length > 0 Then
                FileExt = FileExt.Remove(0, 1)
            End If

            Me.FileName = UnicodeToCodePage437(FileName)
            Me.Extension = UnicodeToCodePage437(FileExt)
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

        Private Sub ClearCache()
            _IsBlankCache = Nothing
            _CreationDateIsCached = False
            _LastAccessDateIsCached = False
            _LastWriteDateIsCached = False
            _VolumeNameIsCached = False
            _FileExtensionIsCached = False
            _FileNameIsCached = False
            _HasInvalidExtensionCache = Nothing
            _HasInvalidFileNameCache = Nothing
        End Sub
    End Class
End Namespace
