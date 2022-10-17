Namespace DiskImage
    Public Structure DataBlock
        Dim Offset As UInteger
        Dim Length As UInteger
    End Structure

    Public Class Disk
        Private ReadOnly _ValidBytesPerSector() As UShort = {512, 1024, 2048, 4096}
        Private ReadOnly _ValidSectorsPerSector() As Byte = {1, 2, 4, 8, 16, 32, 128}
        Private ReadOnly _BootSector As DiskImage.BootSector
        Private ReadOnly _Directory As DiskImage.Directory
        Private _FAT12() As UShort
        Private _FileAllocation As Dictionary(Of UInteger, UInteger)
        Private _FreeSpace As UInteger = 0
        Private ReadOnly _FileBytes() As Byte
        Private ReadOnly _FilePath As String
        Private _Modified As Boolean
        Private ReadOnly _Modifications As Hashtable
        Private ReadOnly _OriginalData As Hashtable

        Sub New(FilePath As String)
            _Modifications = New Hashtable()
            _OriginalData = New Hashtable()
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

        Public ReadOnly Property FileAllocation As Dictionary(Of UInteger, UInteger)
            Get
                Return _FileAllocation
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
            If Not _OriginalData.ContainsKey(Offset) Then
                _OriginalData.Item(Offset) = GetBytesShort(Offset)
            End If

            Array.Copy(BitConverter.GetBytes(Value), 0, _FileBytes, Offset, 2)

            _Modified = True
            _Modifications.Item(Offset) = Value
        End Sub

        Public Sub SetBytes(Value As UInteger, Offset As UInteger)
            If Not _OriginalData.ContainsKey(Offset) Then
                _OriginalData.Item(Offset) = GetBytesInteger(Offset)
            End If

            Array.Copy(BitConverter.GetBytes(Value), 0, _FileBytes, Offset, 4)

            _Modified = True
            _Modifications.Item(Offset) = Value
        End Sub

        Public Sub SetBytes(Value As Byte, Offset As UInteger)
            If Not _OriginalData.ContainsKey(Offset) Then
                _OriginalData.Item(Offset) = GetByte(Offset)
            End If

            _FileBytes(Offset) = Value

            _Modified = True
            _Modifications.Item(Offset) = Value
        End Sub

        Public Sub SetBytes(Value() As Byte, Offset As UInteger)
            If Not _OriginalData.ContainsKey(Offset) Then
                _OriginalData.Item(Offset) = GetBytes(Offset, Value.Length)
            End If

            Array.Copy(Value, 0, _FileBytes, Offset, Value.Length)

            _Modified = True
            _Modifications.Item(Offset) = Value
        End Sub

        Public Sub SetBytes(Value() As Byte, Offset As UInteger, Size As UInteger, Padding As Byte)
            If Not _OriginalData.ContainsKey(Offset) Then
                _OriginalData.Item(Offset) = GetBytes(Offset, Size)
            End If

            If Value.Length <> Size Then
                Disk.ResizeArray(Value, Size, Padding)
            End If
            Array.Copy(Value, 0, _FileBytes, Offset, Size)

            _Modified = True
            _Modifications.Item(Offset) = Value
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

        Public Function ClusterToSector(Cluster As UShort) As UInteger
            Return (_BootSector.DataRegionStart + ((Cluster - 2) * _BootSector.SectorsPerCluster))
        End Function

        Public Function ClusterToOffset(Cluster As UShort) As UInteger
            Return ClusterToSector(Cluster) * _BootSector.BytesPerSector
        End Function

        Public Function SectorToCluster(Sector As UInteger) As UShort
            If Sector < _BootSector.DataRegionStart Then
                Return 0
            End If
            Dim Result = (Sector - _BootSector.DataRegionStart) \ _BootSector.SectorsPerCluster + 2
            If Result < 2 Then
                Return 0
            Else
                Return Result
            End If
        End Function

        Public Function SectorToOffset(Sector As UInteger) As UInteger
            Return Sector * _BootSector.BytesPerSector
        End Function

        Public Function OffsetToSector(Offset As UInteger) As UInteger
            Return Offset \ _BootSector.BytesPerSector
        End Function

        Public Sub ApplyModifications(Modifications As Hashtable)
            For Each Offset As UInteger In Modifications.Keys
                Dim Value = Modifications.Item(Offset)
                SetBytes(Value, Offset)
            Next
        End Sub

        Public Function GetDataBlocksFromFATClusterList(FATClusterList As List(Of UShort)) As List(Of DataBlock)
            Dim Result As New List(Of DataBlock)
            Dim CurrentOffset As UInteger = 0
            Dim ClusterSize As UInteger = _BootSector.BytesPerCluster
            Dim Length As UInteger = 0

            Dim LastCluster As UInteger = 0
            For Each Cluster In FATClusterList
                Dim Offset As UInteger = ClusterToOffset(Cluster)
                If _FileBytes.Length < Offset + ClusterSize Then
                    ClusterSize = Math.Max(_FileBytes.Length - Offset, 0)
                End If
                If ClusterSize > 0 Then
                    If LastCluster = 0 Or Cluster <> LastCluster + 1 Then
                        If LastCluster > 0 Then
                            Result.Add(NewDataBlock(CurrentOffset, Length))
                            Length = 0
                        End If
                        CurrentOffset = Offset
                    End If
                    Length += ClusterSize
                    LastCluster = Cluster
                Else
                    Exit For
                End If
            Next

            If Length > 0 Then
                Result.Add(NewDataBlock(CurrentOffset, Length))
            End If

            Return Result
        End Function

        Public Function GetDataFromFATClusterList(FATClusterList As List(Of UShort)) As Byte()
            Dim ClusterSize As UInteger = _BootSector.BytesPerCluster
            Dim Content(FATClusterList.Count * ClusterSize - 1) As Byte
            Dim ContentOffset As UInteger = 0

            For Each Cluster In FATClusterList
                Dim Offset As UInteger = ClusterToOffset(Cluster)
                If _FileBytes.Length < Offset + ClusterSize Then
                    ClusterSize = Math.Max(_FileBytes.Length - Offset, 0)
                End If
                If ClusterSize > 0 Then
                    Array.Copy(_FileBytes, ClusterToOffset(Cluster), Content, ContentOffset, ClusterSize)
                    ContentOffset += ClusterSize
                Else
                    Exit For
                End If
            Next

            Return Content
        End Function

        Public Function GetDirectoryEntryByOffset(Offset As UInteger) As DiskImage.DirectoryEntry
            Return New DiskImage.DirectoryEntry(Me, Offset)
        End Function

        Public Function GetDirectoryLength(OffsetStart As UInteger, OffsetEnd As UInteger, FileCountOnly As Boolean) As UInteger
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

        Public Function GetFillCharacter() As Byte
            Dim Offset As UInteger = Math.Min(_FileBytes.Length, _BootSector.ImageSize)
            Dim FillChar = _FileBytes(Offset - 1)
            If FillChar = &H0 Then
                For Counter As Integer = Offset - 1 To Offset - _BootSector.BytesPerSector Step -1
                    If _FileBytes(Counter) <> &H0 Then
                        FillChar = &HFF
                        Exit For
                    End If
                Next
            ElseIf FillChar <> &HF6 Then
                FillChar = &HFF
            End If

            Return FillChar
        End Function

        Public Function GetUnusedClusterDataBlocks(WithData As Boolean) As List(Of DataBlock)
            Dim Result As New List(Of DataBlock)
            Dim CurrentOffset As UInteger = 0
            Dim ClusterSize As UInteger = BootSector.BytesPerCluster
            Dim Length As UInteger = 0
            Dim AddCluster As Boolean

            Dim LastCluster As UInteger = 0
            For Cluster As UShort = 1 To _FAT12.Count - 1
                If _FAT12(Cluster) = 0 Then
                    Dim Offset = ClusterToOffset(Cluster)
                    If _FileBytes.Length < Offset + ClusterSize Then
                        ClusterSize = _FileBytes.Length - Offset
                    End If
                    If ClusterSize > 0 Then
                        If WithData Then
                            AddCluster = Not IsDataBlockEmpty(GetBytes(Offset, ClusterSize))
                        Else
                            AddCluster = True
                        End If
                        If AddCluster Then
                            If LastCluster = 0 Or Cluster <> LastCluster + 1 Then
                                If LastCluster > 0 Then
                                    Result.Add(NewDataBlock(CurrentOffset, Length))
                                    Length = 0
                                End If
                                CurrentOffset = Offset
                            End If
                            Length += ClusterSize
                            LastCluster = Cluster
                        End If
                    Else
                        Exit For
                    End If
                End If
            Next

            If Length > 0 Then
                Result.Add(NewDataBlock(CurrentOffset, Length))
            End If

            Return Result
        End Function

        Public Function HasUnusedClustersWithData() As Boolean
            Dim ClusterSize As UInteger = _BootSector.BytesPerCluster

            For Cluster As UShort = 1 To _FAT12.Count - 1
                If _FAT12(Cluster) = 0 Then
                    Dim Offset = ClusterToOffset(Cluster)
                    If _FileBytes.Length < Offset + ClusterSize Then
                        ClusterSize = Math.Max(_FileBytes.Length - Offset, 0)
                    End If
                    If ClusterSize > 0 Then
                        If Not IsDataBlockEmpty(GetBytes(Offset, ClusterSize)) Then
                            Return True
                        End If
                    Else
                        Exit For
                    End If
                End If
            Next

            Return False
        End Function

        Private Function IsDataBlockEmpty(Data() As Byte) As Boolean
            Dim EmptyByte As Byte = Data(0)
            If EmptyByte <> &HF6 And EmptyByte <> &H0 Then
                Return False
            Else
                For Each B In Data
                    If B <> EmptyByte Then
                        Return False
                        Exit For
                    End If
                Next
            End If

            Return True
        End Function

        Public Function IsValidImage() As Boolean
            Dim Result As Boolean = True

            Result = Result And _ValidBytesPerSector.Contains(_BootSector.BytesPerSector)
            Result = Result And _ValidSectorsPerSector.Contains(_BootSector.SectorsPerCluster)
            Result = Result And (_BootSector.MediaDescriptor = 240 Or (_BootSector.MediaDescriptor >= 248 And _BootSector.MediaDescriptor <= 255))
            Return Result
        End Function

        Public Function NewDataBlock(Offset As UInteger, Length As UInteger) As DataBlock
            NewDataBlock.Offset = Offset
            NewDataBlock.Length = Length
        End Function

        Private Sub PopulateFAT12()
            Dim Start As UInteger = SectorToOffset(_BootSector.FatRegionStart)
            Dim Length As UInteger = SectorToOffset(_BootSector.FatRegionSize / 2)
            Dim Size As UShort = _BootSector.NumberOfFATEntries + 1
            Dim ClusterSize As UInteger = _BootSector.BytesPerCluster
            Dim FAT12Bytes(Length - 1) As Byte

            ReDim _FAT12(Size)
            _FileAllocation = New Dictionary(Of UInteger, UInteger)
            _FreeSpace = 0

            Array.Copy(_FileBytes, Start, FAT12Bytes, 0, Length)

            Dim b As UInteger
            Start = 0
            Dim Cluster As UShort = 0
            Do While Cluster <= Size
                b = BitConverter.ToUInt32(FAT12Bytes, Start)
                _FAT12(Cluster) = b Mod 4096
                If _FAT12(Cluster) = 0 Then
                    _FreeSpace += ClusterSize
                End If
                Cluster += 1
                If Cluster <= Size Then
                    b >>= 12
                    _FAT12(Cluster) = b Mod 4096
                    If _FAT12(Cluster) = 0 Then
                        _FreeSpace += ClusterSize
                    End If
                    Cluster += 1
                End If
                Start += 3
            Loop
        End Sub

        Public Sub SaveFile(FilePath As String)
            System.IO.File.WriteAllBytes(FilePath, _FileBytes)
            _OriginalData.Clear()
            _Modifications.Clear()
            _Modified = False
        End Sub

        Public Function RevertChanges() As Boolean
            Dim Result As Boolean = False

            If _OriginalData.Count > 0 Then
                ApplyModifications(_OriginalData)
                _OriginalData.Clear()
                _Modifications.Clear()
                _Modified = False
            End If

            Return Result
        End Function
    End Class
End Namespace
