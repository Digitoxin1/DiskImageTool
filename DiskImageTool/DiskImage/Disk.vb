Namespace DiskImage
    Public Structure DataBlock
        Dim Offset As UInteger
        Dim Length As UInteger
        Dim Caption As String
    End Structure

    Public Class FATChain
        Public Property Chain As New List(Of UShort)
        Public Property HasCircularChain As Boolean = False
        Public Property CrossLinks As New List(Of UInteger)
    End Class

    Public Class Disk
        Private Shared ReadOnly CP437LookupTable As Integer() = New Integer(255) {
            65533, 9786, 9787, 9829, 9830, 9827, 9824, 8226, 9688, 9675, 9689, 9794, 9792, 9834, 9835, 9788,
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
        Public Const BootSectorOffset As UInteger = 0
        Public Const BootSectorSize As UInteger = 512
        Public Shared ReadOnly InvalidFileChars() As Byte = {&H22, &H2A, &H2B, &H2C, &H2E, &H2F, &H3A, &H3B, &H3C, &H3D, &H3E, &H3F, &H5B, &H5C, &H5D, &H7C, &H7F}
        Private ReadOnly _ValidBytesPerSector() As UShort = {512, 1024, 2048, 4096}
        Private ReadOnly _ValidSectorsPerSector() As Byte = {1, 2, 4, 8, 16, 32, 128}
        Private ReadOnly _BootSector As BootSector
        Private ReadOnly _Directory As Directory
        Private _FAT() As UShort
        Private _FATChains As Dictionary(Of UInteger, FATChain)
        Private _FileAllocation As Dictionary(Of UInteger, List(Of UInteger))
        Private _FreeSpace As UInteger = 0
        Private _BadClusters As List(Of UShort)
        Private ReadOnly _FileBytes() As Byte
        Private ReadOnly _FilePath As String
        Private ReadOnly _Modifications As Hashtable
        Private ReadOnly _OriginalData As Hashtable
        Private ReadOnly _LoadError As Boolean = False

        Public Event ChangesReverted As EventHandler

        Sub New(FilePath As String, Optional Modifications As Hashtable = Nothing)
            _Modifications = New Hashtable()
            _OriginalData = New Hashtable()
            _LoadError = False
            _FilePath = FilePath
            Try
                _FileBytes = IO.File.ReadAllBytes(FilePath)
            Catch ex As Exception
                _LoadError = True
            End Try
            If Not _LoadError Then
                If Modifications IsNot Nothing Then
                    ApplyModifications(Modifications)
                End If
            End If
            _BootSector = New BootSector(Me)
            If IsValidImage() Then
                PopulateFAT12(0)
                _Directory = New Directory(Me)
            End If
        End Sub

        Public ReadOnly Property BadClusters As List(Of UShort)
            Get
                Return _BadClusters
            End Get
        End Property

        Public ReadOnly Property BootSector As BootSector
            Get
                Return _BootSector
            End Get
        End Property

        Public ReadOnly Property Data As Byte()
            Get
                Return _FileBytes
            End Get
        End Property

        Public ReadOnly Property Directory As Directory
            Get
                Return _Directory
            End Get
        End Property

        Public ReadOnly Property FAT As UShort()
            Get
                Return _FAT
            End Get
        End Property

        Public ReadOnly Property FATChains As Dictionary(Of UInteger, FATChain)
            Get
                Return _FATChains
            End Get
        End Property

        Public ReadOnly Property FileAllocation As Dictionary(Of UInteger, List(Of UInteger))
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

        Public ReadOnly Property LoadError As Boolean
            Get
                Return _LoadError
            End Get
        End Property

        Public ReadOnly Property Modified As Boolean
            Get
                Return _Modifications.Keys.Count > 0
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

            _Modifications.Item(Offset) = Value
        End Sub

        Public Sub SetBytes(Value As UInteger, Offset As UInteger)
            If Not _OriginalData.ContainsKey(Offset) Then
                _OriginalData.Item(Offset) = GetBytesInteger(Offset)
            End If

            Array.Copy(BitConverter.GetBytes(Value), 0, _FileBytes, Offset, 4)

            _Modifications.Item(Offset) = Value
        End Sub

        Public Sub SetBytes(Value As Byte, Offset As UInteger)
            If Not _OriginalData.ContainsKey(Offset) Then
                _OriginalData.Item(Offset) = GetByte(Offset)
            End If

            _FileBytes(Offset) = Value

            _Modifications.Item(Offset) = Value
        End Sub

        Public Sub SetBytes(Value() As Byte, Offset As UInteger)
            If Not _OriginalData.ContainsKey(Offset) Then
                _OriginalData.Item(Offset) = GetBytes(Offset, Value.Length)
            End If

            Array.Copy(Value, 0, _FileBytes, Offset, Value.Length)

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

            _Modifications.Item(Offset) = Value
        End Sub
        Friend Function RemoveModification(Offset As UInteger) As Boolean
            Dim Result As Boolean = False

            If _OriginalData.ContainsKey(Offset) Then
                Dim Value = _OriginalData.Item(Offset)
                _OriginalData.Remove(Offset)
                _Modifications.Remove(Offset)
                Array.Copy(Value, 0, _FileBytes, Offset, Value.Length)

                Result = True
            End If

            Return Result
        End Function

        Friend Function HasModification(Offset As UInteger) As Boolean
            Return _Modifications.ContainsKey(Offset)
        End Function

        Public Shared Function CheckValidFileName(FileName() As Byte, IsExtension As Boolean, IsVolumeName As Boolean) As Boolean
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

        Public Shared Function CP437ToUnicode(b() As Byte) As String
            Dim b2(b.Length - 1) As UShort
            For counter = 0 To b.Length - 1
                b2(counter) = CP437LookupTable(b(counter))
            Next
            ReDim b(b2.Length * 2 - 1)
            Buffer.BlockCopy(b2, 0, b, 0, b.Length)
            Return System.Text.Encoding.Unicode.GetString(b)
        End Function

        Public Shared Sub ResizeArray(ByRef b() As Byte, Length As UInteger, Padding As Byte)
            Dim Size = b.Length - 1
            If Size <> Length - 1 Then
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

        Public Function OffsetToCluster(Offset As UInteger) As UShort
            Return SectorToCluster(OffsetToSector(Offset))
        End Function

        Public Function OffsetToSector(Offset As UInteger) As UInteger
            Return Offset \ _BootSector.BytesPerSector
        End Function

        Private Sub ApplyModifications(Modifications As Hashtable)
            For Each Offset As UInteger In Modifications.Keys
                Dim Value() As Byte = Modifications.Item(Offset)
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

        Public Function GetDirectoryEntryByOffset(Offset As UInteger) As DirectoryEntry
            Return New DirectoryEntry(Me, Offset)
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

        Public Function CompareFATTables() As Boolean
            If _BootSector.FatCopies < 2 Then
                Return True
            End If

            For Counter As UShort = 1 To _BootSector.FatCopies - 1
                Dim FatCopy1 = GetFAT(Counter - 1)
                Dim FatCopy2 = GetFAT(Counter)

                For Index = 0 To FatCopy1.Length - 1
                    If FatCopy1(Index) <> FatCopy2(Index) Then
                        Return False
                    End If
                Next
            Next

            Return True
        End Function

        Private Function GetFAT(Index As UShort) As Byte()
            Dim Length As UInteger = _BootSector.SectorsPerFAT * _BootSector.BytesPerSector
            Dim Start As UInteger = SectorToOffset(_BootSector.FatRegionStart) + Length * Index

            Dim FATBytes(Length - 1) As Byte

            Array.Copy(_FileBytes, Start, FATBytes, 0, Length)

            Return FATBytes
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

        Public Function GetOEMIDString() As String
            Return CP437ToUnicode(_BootSector.OEMID)
        End Function

        Public Function GetUnusedClusterDataBlocks(WithData As Boolean) As List(Of DataBlock)
            Dim Result As New List(Of DataBlock)
            Dim CurrentOffset As UInteger = 0
            Dim ClusterSize As UInteger = BootSector.BytesPerCluster
            Dim Length As UInteger = 0
            Dim AddCluster As Boolean

            Dim LastCluster As UInteger = 0
            For Cluster As UShort = 1 To _FAT.Count - 1
                If _FAT(Cluster) = 0 Then
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

        Public Function GetVolumeLabel() As DirectoryEntry
            Dim VolumeLabel As DirectoryEntry = Nothing

            For Counter As UInteger = 1 To _Directory.DirectoryLength
                Dim File = _Directory.GetFile(Counter)
                If Not File.IsDeleted And File.IsVolumeName Then
                    VolumeLabel = File
                    Exit For
                End If
            Next

            Return VolumeLabel
        End Function

        Public Function HasUnusedClustersWithData() As Boolean
            Dim ClusterSize As UInteger = _BootSector.BytesPerCluster

            For Cluster As UShort = 1 To _FAT.Count - 1
                If _FAT(Cluster) = 0 Then
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

        Friend Function InitFATChain(Offset As UInteger, ClusterStart As UShort) As FATChain
            Dim FatChain As New FATChain
            Dim Cluster As UShort = ClusterStart
            Dim AssignedClusters As New HashSet(Of UShort)
            Dim OffsetList As List(Of UInteger)

            Do
                If Cluster > 1 And Cluster < _FAT.Length Then
                    If AssignedClusters.Contains(Cluster) Then
                        FatChain.HasCircularChain = True
                        Exit Do
                    End If
                    AssignedClusters.Add(Cluster)
                    FatChain.Chain.Add(Cluster)
                    If Not _FileAllocation.ContainsKey(Cluster) Then
                        OffsetList = New List(Of UInteger) From {
                            Offset
                        }
                        _FileAllocation.Add(Cluster, OffsetList)
                    Else
                        OffsetList = _FileAllocation.Item(Cluster)
                        For Each OffsetItem In OffsetList
                            If Not FatChain.CrossLinks.Contains(OffsetItem) Then
                                FatChain.CrossLinks.Add(OffsetItem)
                            End If
                            If _FATChains.ContainsKey(OffsetItem) Then
                                Dim CrossLinks = FATChains.Item(OffsetItem).CrossLinks
                                If Not CrossLinks.Contains(Offset) Then
                                    CrossLinks.Add(Offset)
                                End If
                            End If
                        Next
                        OffsetList.Add(Offset)
                    End If
                    Cluster = _FAT(Cluster)
                Else
                    Cluster = 0
                End If
            Loop Until Cluster < &H2 Or Cluster > &HFEF

            _FATChains.Item(Offset) = FatChain

            Return FatChain
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
            Dim Result As Boolean = Not _LoadError

            If Result Then
                Result = _ValidBytesPerSector.Contains(_BootSector.BytesPerSector)
            End If
            If Result Then
                Result = _ValidSectorsPerSector.Contains(_BootSector.SectorsPerCluster)
            End If
            If Result Then
                Result = (_BootSector.MediaDescriptor = 240 Or (_BootSector.MediaDescriptor >= 248 And _BootSector.MediaDescriptor <= 255))
            End If

            Return Result
        End Function

        Public Shared Function NewDataBlock(Offset As UInteger, Length As UInteger) As DataBlock
            NewDataBlock.Offset = Offset
            NewDataBlock.Length = Length
            NewDataBlock.Caption = ""
        End Function

        Public Shared Function NewDataBlock(Offset As UInteger, Length As UInteger, Caption As String) As DataBlock
            NewDataBlock.Offset = Offset
            NewDataBlock.Length = Length
            NewDataBlock.Caption = Caption
        End Function

        Private Sub PopulateFAT12(Index As UShort)
            Dim Size As UShort = _BootSector.NumberOfFATEntries + 1
            Dim ClusterSize As UInteger = _BootSector.BytesPerCluster
            Dim FATBytes = GetFAT(Index)

            ReDim _FAT(Size)
            _FATChains = New Dictionary(Of UInteger, FATChain)
            _FileAllocation = New Dictionary(Of UInteger, List(Of UInteger))
            _FreeSpace = 0
            _BadClusters = New List(Of UShort)

            Dim b As UInteger
            Dim Start As Integer = 0
            Dim Cluster As UShort = 0
            Do While Cluster <= Size
                b = BitConverter.ToUInt32(FATBytes, Start)
                _FAT(Cluster) = b Mod 4096
                If _FAT(Cluster) = 0 Then
                    _FreeSpace += ClusterSize
                ElseIf _FAT(Cluster) = &HFF7 Then
                    _BadClusters.Add(Cluster)
                End If
                Cluster += 1
                If Cluster <= Size Then
                    b >>= 12
                    _FAT(Cluster) = b Mod 4096
                    If _FAT(Cluster) = 0 Then
                        _FreeSpace += ClusterSize
                    ElseIf _FAT(Cluster) = &HFF7 Then
                        _BadClusters.Add(Cluster)
                    End If
                    Cluster += 1
                End If
                Start += 3
            Loop
        End Sub

        Public Sub SaveFile(FilePath As String)
            IO.File.WriteAllBytes(FilePath, _FileBytes)
            _OriginalData.Clear()
            _Modifications.Clear()
        End Sub

        Public Function RevertChanges() As Boolean
            Dim Result As Boolean = False

            If _OriginalData.Count > 0 Then
                ApplyModifications(_OriginalData)
                _OriginalData.Clear()
                _Modifications.Clear()
                RaiseEvent ChangesReverted(Me, EventArgs.Empty)
            End If

            Return Result
        End Function
    End Class
End Namespace
