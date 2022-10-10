Namespace DiskImage
    Public Class Disk
        Private ReadOnly _BootSector As DiskImage.BootSector
        Private ReadOnly _Directory As DiskImage.Directory
        Private _FAT12() As UShort
        Private ReadOnly _FileBytes() As Byte
        Private ReadOnly _FilePath As String
        Private _Modified As Boolean

        Sub New(FilePath As String)
            _Modified = False
            _FilePath = FilePath
            _FileBytes = System.IO.File.ReadAllBytes(FilePath)
            _BootSector = New DiskImage.BootSector(Me)
            If IsValidImage() Then
                PopulateFAT12()
                _Directory = New DiskImage.Directory(Me)
            End If
        End Sub

        Public ReadOnly Property BootSector As DiskImage.BootSector
            Get
                Return _BootSector
            End Get
        End Property

        Public ReadOnly Property Data As Byte()
            Get
                Return _FileBytes
            End Get
        End Property

        Public ReadOnly Property Directory As DiskImage.Directory
            Get
                Return _Directory
            End Get
        End Property

        Public ReadOnly Property FAT12 As UShort()
            Get
                Return _FAT12
            End Get
        End Property

        Public ReadOnly Property FilePath As String
            Get
                Return _FilePath
            End Get
        End Property

        Public ReadOnly Property Modified As Boolean
            Get
                Return _Modified
            End Get
        End Property

        Public Function GetByte(Offset As UInteger) As Byte
            Return _FileBytes(Offset)
        End Function

        Public Function GetBytes(Offset As UInteger, Size As UInteger) As Byte()
            Dim temp(Size - 1) As Byte
            Array.Copy(_FileBytes, Offset, temp, 0, Size)
            Return temp
        End Function

        Public Function GetBytesInteger(Offset As UInteger) As UInteger
            Return BitConverter.ToUInt32(_FileBytes, Offset)
        End Function

        Public Function GetBytesShort(Offset As UInteger) As UShort
            Return BitConverter.ToUInt16(_FileBytes, Offset)
        End Function

        Public Sub SetBytes(Value As UShort, Offset As UInteger)
            Array.Copy(BitConverter.GetBytes(Value), 0, _FileBytes, Offset, 2)
            _Modified = True
        End Sub

        Public Sub SetBytes(Value As UInteger, Offset As UInteger)
            Array.Copy(BitConverter.GetBytes(Value), 0, _FileBytes, Offset, 4)
            _Modified = True
        End Sub

        Public Sub SetBytes(Value As Byte, Offset As UInteger)
            _FileBytes(Offset) = Value
            _Modified = True
        End Sub

        Public Sub SetBytes(Value() As Byte, Offset As UInteger, Size As UInteger, Padding As Byte)
            Disk.ResizeArray(Value, Size, Padding)
            Array.Copy(Value, 0, _FileBytes, Offset, Size)
            _Modified = True
        End Sub

        Public Shared Sub ResizeArray(ByRef b() As Byte, length As UInteger, Padding As Byte)
            Dim Size = UBound(b)
            If Size < length - 1 Then
                ReDim Preserve b(length - 1)
                For counter As UInteger = Size + 1 To length - 1
                    b(counter) = Padding
                Next
            End If
        End Sub

        Public Function GetDirectoryEntryByOffset(Offset As UInteger) As DiskImage.DirectoryEntry
            Return New DiskImage.DirectoryEntry(Me, Offset)
        End Function

        Public Function GetDirectoryLength(OffsetStart As UInteger, OffsetEnd As UInteger, FileCountOnly As Boolean) As Integer
            Dim Count As UInteger = 0

            Do While _FileBytes(OffsetStart) > 0
                If Not FileCountOnly Then
                    Count += 1
                ElseIf _FileBytes(OffsetStart + 11) <> &HF Then 'Exclude LFN entries
                    Dim FileBytes = BitConverter.ToUInt16(_FileBytes, OffsetStart)
                    If FileBytes <> &H202E And FileBytes <> &H2E2E Then 'Exclude '.' and '..' entries
                        Count += 1
                    End If
                End If
                OffsetStart += 32
                If OffsetEnd > 0 And OffsetStart >= OffsetEnd Then
                    Exit Do
                End If
            Loop

            Return Count
        End Function

        Public Function IsValidImage() As Boolean
            Dim Result As Boolean = True

            Result = Result And (_BootSector.BytesPerSector = 512 Or _BootSector.BytesPerSector = 1024 Or _BootSector.BytesPerSector = 2048 Or _BootSector.BytesPerSector = 4096)
            Result = Result And (_BootSector.SectorsPerCluster = 1 Or _BootSector.SectorsPerCluster = 2 Or _BootSector.SectorsPerCluster = 4 Or _BootSector.SectorsPerCluster = 8 Or _BootSector.SectorsPerCluster = 16 Or _BootSector.SectorsPerCluster = 32 Or _BootSector.SectorsPerCluster = 128)
            Result = Result And (_BootSector.MediaDescriptor = 240 Or (_BootSector.MediaDescriptor >= 248 And _BootSector.MediaDescriptor <= 255))
            Return Result
        End Function

        Private Sub PopulateFAT12()
            Dim Start As UInteger = _BootSector.FatRegionStart * _BootSector.BytesPerSector
            Dim Length As UInteger = _BootSector.FatRegionSize / 2 * _BootSector.BytesPerSector

            Dim FAT12Bytes(Length - 1) As Byte
            ReDim _FAT12(_BootSector.NumberOfFATEntries + 1)

            Array.Copy(_FileBytes, Start, FAT12Bytes, 0, Length)

            Dim b As UInteger
            Start = 0
            Dim Cluster As UInteger = 0
            Do While Cluster <= _BootSector.NumberOfFATEntries + 1
                b = BitConverter.ToUInt32(FAT12Bytes, Start)
                _FAT12(Cluster) = b Mod 4096
                Cluster += 1
                If Cluster <= _BootSector.NumberOfFATEntries + 1 Then
                    b >>= 12
                    _FAT12(Cluster) = b Mod 4096
                    Cluster += 1
                End If
                Start += 3
            Loop
        End Sub

        Public Sub SaveFile(FilePath As String)
            System.IO.File.WriteAllBytes(FilePath, _FileBytes)
            _Modified = False
        End Sub
    End Class
End Namespace
