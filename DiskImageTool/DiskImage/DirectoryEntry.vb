Imports System.Text

Namespace DiskImage
    Public Structure ExpandedDate
        Dim Year As UShort
        Dim Month As Byte
        Dim Day As Byte
        Dim Hour As Byte
        Dim Hour12 As Byte
        Dim Minute As Byte
        Dim Second As Byte
        Dim Milliseconds As UShort
        Dim IsValidDate As Boolean
        Dim DateObject As Date
        Dim IsPM As Boolean
    End Structure

    Public Enum PropertyType
        inByte = 0
        inString = 1
    End Enum

    Public Class DirectoryEntry
        Private Shared ReadOnly CP437LookupTable As Integer() = New Integer(255) {
            0, 9786, 9787, 9829, 9830, 9827, 9824, 8226, 9688, 9675, 9689, 9794, 9792, 9834, 9835, 9788,
            9658, 9668, 8597, 8252, 182, 167, 9644, 8616, 8593, 8595, 8594, 8592, 8735, 8596, 9650, 9660,
            32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
            48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63,
            64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79,
            80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95,
            96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
            112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 8962,
            199, 252, 233, 226, 228, 224, 229, 231, 234, 235, 232, 239, 238, 236, 196, 197,
            201, 230, 198, 244, 246, 242, 251, 249, 255, 214, 220, 162, 163, 165, 8359, 402,
            225, 237, 243, 250, 241, 209, 170, 186, 191, 8976, 172, 189, 188, 161, 171, 187,
            9617, 9618, 9619, 9474, 9508, 9569, 9570, 9558, 9557, 9571, 9553, 9559, 9565, 9564, 9563, 9488,
            9492, 9524, 9516, 9500, 9472, 9532, 9566, 9567, 9562, 9556, 9577, 9574, 9568, 9552, 9580, 9575,
            9576, 9572, 9573, 9561, 9560, 9554, 9555, 9579, 9578, 9496, 9484, 9608, 9604, 9612, 9616, 9600,
            945, 223, 915, 960, 931, 963, 181, 964, 934, 920, 937, 948, 8734, 966, 949, 8745,
            8801, 177, 8805, 8804, 8992, 8993, 247, 8776, 176, 8729, 183, 8730, 8319, 178, 9632, 160
        }
        Private Const DIRECTORY_ENTRY_SIZE As Byte = 32
        Private Const CHAR_SPACE As Byte = 32
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

        Private _Data() As Byte
        Private ReadOnly _Offset As UInteger
        Private ReadOnly _FatChain As FATChain
        Private WithEvents Parent As Disk
        Private _SubDirectory As Directory

        Sub New(Parent As Disk, Offset As UInteger)
            Me.Parent = Parent
            _Offset = Offset

            LoadData()

            If _Parent.FATChains.ContainsKey(Offset) Then
                _FatChain = _Parent.FATChains.Item(Offset)
            Else
                If IsDeleted() Then
                    _FatChain = _Parent.InitFATChain(Offset, 0)
                ElseIf IsDirectory() AndAlso {".", ".."}.Contains(GetFullFileName) Then
                    _FatChain = _Parent.InitFATChain(Offset, 0)
                Else
                    _FatChain = _Parent.InitFATChain(Offset, StartingCluster)
                End If
            End If

            InitSubDirectory()
        End Sub

        Public Shared Function CP437ToUnicode(b() As Byte) As String
            Dim b2(b.Length - 1) As UShort
            For counter = 0 To b.Length - 1
                b2(counter) = CP437LookupTable(b(counter))
            Next
            ReDim b(b2.Length * 2 - 1)
            Buffer.BlockCopy(b2, 0, b, 0, b.Length)
            Return Encoding.Unicode.GetString(b)
        End Function

        Public Shared Function DateToFATDate(D As Date) As UShort
            Dim FATDate As UShort = D.Year - 1980

            FATDate <<= 4
            FATDate += D.Month
            FATDate <<= 5
            FATDate += D.Day

            Return FATDate
        End Function

        Public Shared Function DateToFATTime(D As Date) As UShort
            Dim DTTime As UShort = D.Hour

            DTTime <<= 6
            DTTime += D.Minute
            DTTime <<= 5
            DTTime += D.Second / 2

            Return DTTime
        End Function

        Public Shared Function DateToFATMilliseconds(D As Date) As Byte
            Return D.Millisecond / 10
        End Function

        Public Shared Function ExpandDate(FATDate As UShort) As ExpandedDate
            Return ExpandDate(FATDate, 0, 0)
        End Function

        Public Shared Function ExpandDate(FATDate As UShort, FATTime As UShort) As ExpandedDate
            Return ExpandDate(FATDate, FATTime, 0)
        End Function

        Public Shared Function ExpandDate(FATDate As UShort, FATTime As UShort, Milliseconds As Byte) As ExpandedDate
            Dim DT As ExpandedDate

            DT.Day = FATDate Mod 32
            FATDate >>= 5
            DT.Month = FATDate Mod 16
            FATDate >>= 4
            DT.Year = 1980 + FATDate Mod 128

            DT.Second = (FATTime Mod 32) * 2
            FATTime >>= 5
            DT.Minute = FATTime Mod 64
            FATTime >>= 6
            DT.Hour = FATTime Mod 32

            If Milliseconds < 100 Then
                DT.Milliseconds = Milliseconds * 10
            Else
                DT.Milliseconds = 0
            End If

            Dim DateString As String = DT.Year & "-" & Format(DT.Month, "00") & "-" & Format(DT.Day, "00") & " " & DT.Hour & ":" & Format(DT.Minute, "00") & ":" & Format(DT.Second, "00")

            DT.IsValidDate = IsDate(DateString)

            DT.IsPM = (DT.Hour > 11)

            If DT.Hour > 12 Then
                DT.Hour12 = DT.Hour - 12
            ElseIf DT.Hour = 0 Then
                DT.Hour12 = 12
            Else
                DT.Hour12 = DT.Hour
            End If

            If DT.IsValidDate Then
                DT.DateObject = New Date(DT.Year, DT.Month, DT.Day, DT.Hour, DT.Minute, DT.Second, DT.Milliseconds)
            End If

            Return DT
        End Function

        Private Function GetByte(Offset As UInteger) As Byte
            Return _Data(Offset)
        End Function

        Private Function GetBytes(Offset As UInteger, Size As UInteger) As Byte()
            Dim temp(Size - 1) As Byte
            Array.Copy(_Data, Offset, temp, 0, Size)
            Return temp
        End Function

        Private Function GetBytesShort(Offset As UInteger) As UShort
            Return BitConverter.ToUInt16(_Data, Offset)
        End Function

        Private Function GetBytesInteger(Offset As UInteger) As UInteger
            Return BitConverter.ToUInt32(_Data, Offset)
        End Function

        Private Sub SetBytes(Value As UShort, Offset As UInteger)
            Array.Copy(BitConverter.GetBytes(Value), 0, _Data, Offset, 2)
            Parent.SetBytes(_Data, _Offset)
        End Sub

        Private Sub SetBytes(Value As UInteger, Offset As UInteger)
            Array.Copy(BitConverter.GetBytes(Value), 0, _Data, Offset, 4)
            Parent.SetBytes(_Data, _Offset)
        End Sub

        Private Sub SetBytes(Value As Byte, Offset As UInteger)
            _Data(Offset) = Value
            Parent.SetBytes(_Data, _Offset)
        End Sub

        Private Sub SetBytes(Value() As Byte, Offset As UInteger, Size As UInteger, Padding As Byte)
            If Value.Length <> Size Then
                Disk.ResizeArray(Value, Size, Padding)
            End If
            Array.Copy(Value, 0, _Data, Offset, Size)
            Parent.SetBytes(_Data, _Offset)
        End Sub

        Public Sub RemoveModification()
            If Parent.RemoveModification(_Offset) Then
                LoadData()
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

        Public Function GetCreationDate() As ExpandedDate
            Return ExpandDate(CreationDate, CreationTime, CreationMillisecond)
        End Function

        Public Function GetDataBlocks() As List(Of DataBlock)
            Return Parent.GetDataBlocksFromFATClusterList(_FatChain.Chain)
        End Function

        Public Function GetContent() As Byte()
            Dim Content = Parent.GetDataFromFATClusterList(_FatChain.Chain)

            If Content.Length <> FileSize Then
                Array.Resize(Of Byte)(Content, FileSize)
            End If

            Return Content
        End Function

        Public Function GetFileExtension() As String
            Return CP437ToUnicode(Extension).TrimEnd(" ")
        End Function

        Public Function GetFileName() As String
            Return CP437ToUnicode(FileName).TrimEnd(" ")
        End Function

        Public Function GetVolumeName() As String
            Return CP437ToUnicode(FileName) & CP437ToUnicode(Extension).TrimEnd(" ")
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

                Array.Copy(Parent.Data, _Offset + DirectoryEntryOffset.LFNFilePart1, LFN, 0, DirectoryEntrySize.LFNFilePart1)
                Array.Copy(Parent.Data, _Offset + DirectoryEntryOffset.LFNFilePart2, LFN, DirectoryEntrySize.LFNFilePart1, DirectoryEntrySize.LFNFilePart2)
                Array.Copy(Parent.Data, _Offset + DirectoryEntryOffset.LFNFilePart3, LFN, DirectoryEntrySize.LFNFilePart1 + DirectoryEntrySize.LFNFilePart2, DirectoryEntrySize.LFNFilePart3)

                Return Encoding.Unicode.GetString(LFN)
            Else
                Dim LFN(1) As Byte

                Return Encoding.Unicode.GetString(LFN)
            End If
        End Function

        Public Function HasCreationDate() As Boolean
            Return CreationDate <> 0 Or CreationTime <> 0 Or CreationMillisecond <> 0
        End Function

        Public Function HasInvalidAttributes() As Boolean
            Return (Attributes > 63)
        End Function

        Public Function HasInvalidExtension() As Boolean
            Return Not Disk.CheckValidFileName(Extension, True, IsVolumeName())
        End Function

        Public Function HasInvalidFilename() As Boolean
            Return Not Disk.CheckValidFileName(FileName, False, IsVolumeName())
        End Function

        Public Function HasInvalidFileSize() As Boolean
            Return FileSize > Parent.BootSector.ImageSize
        End Function

        Public Function HasInvalidStartingCluster() As Boolean
            Return StartingCluster = 1 Or StartingCluster > _Parent.BootSector.NumberOfFATEntries + 1
        End Function

        Public Function HasIncorrectFileSize() As Boolean
            If IsDirectory() Then
                Return False
            End If

            Dim AllocatedSize As UInteger = Math.Ceiling(FileSize / _Parent.BootSector.BytesPerCluster) * _Parent.BootSector.BytesPerCluster
            Dim AllocatedSizeFromFAT As UInteger = _FatChain.Chain.Count * _Parent.BootSector.BytesPerCluster

            Return AllocatedSize <> AllocatedSizeFromFAT
        End Function

        Public Function HasLastAccessDate() As Boolean
            Return LastAccessDate <> 0
        End Function

        Private Sub InitSubDirectory()
            If IsDirectory() And Not IsDeleted() Then
                _SubDirectory = New Directory(Parent, _FatChain.Chain, FileSize)
            Else
                _SubDirectory = Nothing
            End If
        End Sub

        Public Function IsArchive() As Boolean
            Return (Attributes And AttributeFlags.ArchiveFlag) > 0
        End Function

        Public Function IsCrossLinked() As Boolean
            Return _FatChain.CrossLinks.Count > 0
        End Function

        Public Function IsDeleted() As Boolean
            Return FileName(0) = &HE5
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

        Public Function IsModified() As Boolean
            Dim HasModification As Boolean = Parent.HasModification(_Offset)
            If Not HasModification Then
                If _FatChain.Chain.Count > 0 Then
                    Dim ClusterOffset = _Parent.ClusterToOffset(_FatChain.Chain(0))
                    HasModification = Parent.HasModification(ClusterOffset)
                End If
            End If
            Return HasModification
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

        Private Sub LoadData()
            _Data = Parent.GetBytes(Offset, DIRECTORY_ENTRY_SIZE)
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

        Private Sub Parent_ChangesReverted(sender As Object, e As EventArgs) Handles Parent.ChangesReverted
            LoadData()
        End Sub

        Public Property Attributes() As Byte
            Get
                Return GetByte(DirectoryEntryOffset.Attributes)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.Attributes)
            End Set
        End Property

        Public Property CreationDate() As UShort
            Get
                Return GetBytesShort(DirectoryEntryOffset.CreationDate)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.CreationDate)
            End Set
        End Property

        Public Property CreationTime() As UShort
            Get
                Return GetBytesShort(DirectoryEntryOffset.CreationTime)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.CreationTime)
            End Set
        End Property

        Public Property CreationMillisecond() As Byte
            Get
                Return GetByte(DirectoryEntryOffset.CreationMillisecond)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.CreationMillisecond)
            End Set
        End Property

        Public ReadOnly Property CrossLinks() As List(Of UInteger)
            Get
                Return _FatChain.CrossLinks
            End Get
        End Property

        Public Property Extension() As Byte()
            Get
                Return GetBytes(DirectoryEntryOffset.Extension, DirectoryEntrySize.Extension)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.Extension, DirectoryEntrySize.Extension, CHAR_SPACE)
            End Set
        End Property

        Public ReadOnly Property FATChain As List(Of UShort)
            Get
                Return _FatChain.Chain
            End Get
        End Property

        Public Property FileName() As Byte()
            Get
                Return GetBytes(DirectoryEntryOffset.FileName, DirectoryEntrySize.FileName)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.FileName, DirectoryEntrySize.FileName, CHAR_SPACE)
            End Set
        End Property

        Public Property FileSize() As UInteger
            Get
                Return GetBytesInteger(DirectoryEntryOffset.FileSize)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.FileSize)
            End Set
        End Property

        Public ReadOnly Property HasCircularChain As Boolean
            Get
                Return _FatChain.HasCircularChain
            End Get
        End Property

        Public Property LastAccessDate() As UShort
            Get
                Return GetBytesShort(DirectoryEntryOffset.LastAccessDate)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.LastAccessDate)
            End Set
        End Property

        Public Property LastWriteDate() As UShort
            Get
                Return GetBytesShort(DirectoryEntryOffset.LastWriteDate)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.LastWriteDate)
            End Set
        End Property

        Public Property LastWriteTime() As UShort
            Get
                Return GetBytesShort(DirectoryEntryOffset.LastWriteTime)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.LastWriteTime)
            End Set
        End Property

        Public ReadOnly Property LFNSequence As Byte
            Get
                Return GetByte(DirectoryEntryOffset.LFNSequence)
            End Get
        End Property

        Public ReadOnly Property Offset As Integer
            Get
                Return _Offset
            End Get
        End Property

        Public Property ReservedForWinNT() As Byte
            Get
                Return GetByte(DirectoryEntryOffset.ReservedForWinNT)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.ReservedForWinNT)
            End Set
        End Property

        Public Property ReservedForFAT32() As UShort
            Get
                Return GetBytesShort(DirectoryEntryOffset.ReservedForFAT32)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.ReservedForFAT32)
            End Set
        End Property

        Public Property StartingCluster() As UShort
            Get
                Return GetBytesShort(DirectoryEntryOffset.StartingCluster)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.StartingCluster)
            End Set
        End Property

        Public ReadOnly Property SubDirectory As Directory
            Get
                Return _SubDirectory
            End Get
        End Property
    End Class
End Namespace