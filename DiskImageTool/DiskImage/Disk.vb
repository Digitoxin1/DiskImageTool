Namespace DiskImage
    Public Structure DataBlock
        Dim Cluster As UInteger
        Dim Sector As UInteger
        Dim Offset As UInteger
        Dim Data() As Byte
    End Structure

    Public Class Disk
        Private ReadOnly _ValidBytesPerSector() As UShort = {512, 1024, 2048, 4096}
        Private ReadOnly _ValidSectorsPerSector() As Byte = {1, 2, 4, 8, 16, 32, 128}
        Private ReadOnly _BootSector As DiskImage.BootSector
        Private ReadOnly _Directory As DiskImage.Directory
        Private _FAT12() As UShort
        Private _FreeSpace As UInteger = 0
        Private _FreeSpaceClusterStart As UInteger = 0
        Private ReadOnly _FileBytes() As Byte
        Private ReadOnly _FilePath As String
        Private _Modified As Boolean
        Private ReadOnly _Modifications As Hashtable

        Sub New(FilePath As String)
            _Modifications = New Hashtable()
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

        Public ReadOnly Property FreeSpace As UInteger
            Get
                Return _FreeSpace
            End Get
        End Property

        Public ReadOnly Property FreeSpaceClusterStart As UInteger
            Get
                Return _FreeSpaceClusterStart
            End Get
        End Property

        Public ReadOnly Property Modified As Boolean
            Get
                Return _Modified
            End Get
        End Property

        Public ReadOnly Property Modifications As Hashtable
            Get
                Return _Modifications
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
            _Modifications.Item(Offset) = Value
        End Sub

        Public Sub SetBytes(Value As UInteger, Offset As UInteger)
            Array.Copy(BitConverter.GetBytes(Value), 0, _FileBytes, Offset, 4)
            _Modified = True
            _Modifications.Item(Offset) = Value
        End Sub

        Public Sub SetBytes(Value As Byte, Offset As UInteger)
            _FileBytes(Offset) = Value
            _Modified = True
            _Modifications.Item(Offset) = Value
        End Sub

        Public Sub SetBytes(Value() As Byte, Offset As UInteger)
            Array.Copy(Value, 0, _FileBytes, Offset, Value.Length)
            _Modified = True
            _Modifications.Item(Offset) = Value
        End Sub

        Public Sub SetBytes(Value() As Byte, Offset As UInteger, Size As UInteger, Padding As Byte)
            Disk.ResizeArray(Value, Size, Padding)
            Array.Copy(Value, 0, _FileBytes, Offset, Size)
            _Modified = True
            _Modifications.Item(Offset) = Value
        End Sub

        Public Sub ApplyModifications(Modifications As Hashtable)
            For Each Offset As UInteger In Modifications.Keys
                Dim Value = Modifications.Item(Offset)
                SetBytes(Value, Offset)
            Next
        End Sub

        Public Shared Sub ResizeArray(ByRef b() As Byte, Length As UInteger, Padding As Byte)
            Dim Size = UBound(b)
            If Size < Length - 1 Then
                ReDim Preserve b(Length - 1)
                For counter As UInteger = Size + 1 To Length - 1
                    b(counter) = Padding
                Next
            End If
        End Sub

        Public Function ClusterToSector(Cluster As UInteger) As UInteger
            Return (_BootSector.DataRegionStart + ((Cluster - 2) * _BootSector.SectorsPerCluster))
        End Function

        Public Function ClusterToOffset(Cluster As UInteger) As UInteger
            Return (_BootSector.DataRegionStart + ((Cluster - 2) * _BootSector.SectorsPerCluster)) * _BootSector.BytesPerSector
        End Function

        Public Function SectorToCluster(Sector As UInteger) As UInteger
            Return Int((Sector - _BootSector.DataRegionStart) / _BootSector.SectorsPerCluster + 2)
        End Function

        Public Function SectorToOffset(Sector As UInteger) As UInteger
            Return Sector * _BootSector.BytesPerSector
        End Function

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

        Public Function HasUnusedClustersWithData() As Boolean
            If _FreeSpaceClusterStart > 0 Then
                Dim ClusterCount As UInteger = _BootSector.NumberOfFATEntries + 1

                For Counter = _FreeSpaceClusterStart To ClusterCount
                    Dim Data = GetBytes(ClusterToOffset(Counter), _BootSector.BytesPerCluster)
                    For Each B In Data
                        If B <> &HF6 And B <> &H0 Then
                            Return True
                        End If
                    Next
                Next
            End If

            Return False
        End Function

        Public Function GetUnusedClustersWithData() As List(Of DataBlock)
            Dim Result As New List(Of DataBlock)
            Dim Found As Boolean

            If _FreeSpaceClusterStart > 0 Then
                Dim SectorStart As UInteger = ClusterToSector(_FreeSpaceClusterStart)
                Dim SectorCount As UInteger = _BootSector.SectorCount - 1

                For Counter = SectorStart To SectorCount
                    Dim Block As DataBlock
                    With Block
                        .Cluster = SectorToCluster(Counter)
                        .Sector = Counter
                        .Offset = SectorToOffset(Counter)
                        .Data = GetBytes(.Offset, _BootSector.BytesPerSector)
                    End With
                    Found = False
                    For Each B In Block.Data
                        If B <> &HF6 And B <> &H0 Then
                            Found = True
                            Exit For
                        End If
                    Next
                    If Found Then
                        Result.Add(Block)
                    End If
                Next
            End If

            Return Result
        End Function

        Public Function IsValidImage() As Boolean
            Dim Result As Boolean = True

            Result = Result And _ValidBytesPerSector.Contains(_BootSector.BytesPerSector)
            Result = Result And _ValidSectorsPerSector.Contains(_BootSector.SectorsPerCluster)
            Result = Result And (_BootSector.MediaDescriptor = 240 Or (_BootSector.MediaDescriptor >= 248 And _BootSector.MediaDescriptor <= 255))
            Return Result
        End Function

        Private Sub PopulateFAT12()
            Dim Start As UInteger = SectorToOffset(_BootSector.FatRegionStart)
            Dim Length As UInteger = SectorToOffset(_BootSector.FatRegionSize / 2)

            Dim FAT12Bytes(Length - 1) As Byte
            ReDim _FAT12(_BootSector.NumberOfFATEntries + 1)
            _FreeSpace = 0
            _FreeSpaceClusterStart = 0

            Array.Copy(_FileBytes, Start, FAT12Bytes, 0, Length)

            Dim b As UInteger
            Start = 0
            Dim Cluster As UInteger = 0
            Do While Cluster <= _BootSector.NumberOfFATEntries + 1
                b = BitConverter.ToUInt32(FAT12Bytes, Start)
                _FAT12(Cluster) = b Mod 4096
                If _FAT12(Cluster) = 0 Then
                    _FreeSpace += _BootSector.BytesPerCluster
                    If _FreeSpaceClusterStart = 0 Then
                        _FreeSpaceClusterStart = Cluster
                    End If
                Else
                    _FreeSpaceClusterStart = 0
                End If
                Cluster += 1
                If Cluster <= _BootSector.NumberOfFATEntries + 1 Then
                    b >>= 12
                    _FAT12(Cluster) = b Mod 4096
                    If _FAT12(Cluster) = 0 Then
                        _FreeSpace += _BootSector.BytesPerCluster
                        If _FreeSpaceClusterStart = 0 Then
                            _FreeSpaceClusterStart = Cluster
                        End If
                    Else
                        _FreeSpaceClusterStart = 0
                    End If
                    Cluster += 1
                End If
                Start += 3
            Loop
        End Sub

        Public Sub SaveFile(FilePath As String)
            System.IO.File.WriteAllBytes(FilePath, _FileBytes)
            _Modified = False
            _Modifications.Clear()
        End Sub
    End Class
End Namespace
