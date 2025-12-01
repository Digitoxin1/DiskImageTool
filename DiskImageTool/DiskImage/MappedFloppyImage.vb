Imports DiskImageTool.Bitstream

Namespace DiskImage
    Public MustInherit Class MappedFloppyImage
        Implements IFloppyImage

        Friend Const SECTOR_COUNT As Byte = 36
        Private ReadOnly _AdditionalTracks As HashSet(Of UShort)
        Private ReadOnly _BytesPerSector As UInteger
        Private ReadOnly _Image As IBitstreamImage
        Private ReadOnly _NonStandardTracks As HashSet(Of UShort)
        Private ReadOnly _ProtectedSectors As HashSet(Of UInteger)
        Private ReadOnly _TranslatedSectors As HashSet(Of UInteger)
        Private ReadOnly _History As ImageHistory
        Private _BPB As BiosParameterBlock
        Private _EmptySector() As Byte
        Private _ImageSize As Integer
        Private _Sectors() As BitstreamSector
        Private _SideCount As Byte
        Private _TrackCount As UShort
        Private _Tracks() As TrackData

        Public Sub New(Image As IBitstreamImage, BytesPerSector As UInteger)
            _Image = Image
            _History = New ImageHistory(Me)
            _ProtectedSectors = New HashSet(Of UInteger)
            _TranslatedSectors = New HashSet(Of UInteger)
            _AdditionalTracks = New HashSet(Of UShort)
            _NonStandardTracks = New HashSet(Of UShort)
            _BytesPerSector = BytesPerSector

            If _Image IsNot Nothing Then
                BuildSectorMap()
            End If
        End Sub

        Public ReadOnly Property AdditionalTracks As HashSet(Of UShort) Implements IFloppyImage.AdditionalTracks
            Get
                Return _AdditionalTracks
            End Get
        End Property

        Public ReadOnly Property BitStreamImage As IBitstreamImage Implements IFloppyImage.BitstreamImage
            Get
                Return _Image
            End Get
        End Property

        Public ReadOnly Property BytesPerSector As UInteger Implements IFloppyImage.BytesPerSector
            Get
                Return _BytesPerSector
            End Get
        End Property

        Public ReadOnly Property CanResize As Boolean Implements IFloppyImage.CanResize
            Get
                Return False
            End Get
        End Property

        Public Overridable ReadOnly Property HasWeakBits As Boolean Implements IFloppyImage.HasWeakBits
            Get
                Return False
            End Get
        End Property

        Public Overridable ReadOnly Property HasWeakBitsSupport As Boolean Implements IFloppyImage.HasWeakBitsSupport
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property History As ImageHistory Implements IFloppyImage.History
            Get
                Return _History
            End Get
        End Property

        Public MustOverride ReadOnly Property ImageType As FloppyImageType Implements IFloppyImage.ImageType

        Public ReadOnly Property IsBitstreamImage As Boolean Implements IFloppyImage.IsBitstreamImage
            Get
                Return _Image IsNot Nothing
            End Get
        End Property

        Public ReadOnly Property Length As Integer Implements IFloppyImage.Length
            Get
                Return _ImageSize
            End Get
        End Property

        Public ReadOnly Property NonStandardTracks As HashSet(Of UShort) Implements IFloppyImage.NonStandardTracks
            Get
                Return _NonStandardTracks
            End Get
        End Property

        Public ReadOnly Property Sectors As BitstreamSector()
            Get
                Return _Sectors
            End Get
        End Property

        Public ReadOnly Property SideCount As Byte Implements IFloppyImage.SideCount
            Get
                Return _SideCount
            End Get
        End Property

        Public ReadOnly Property TrackCount As UShort Implements IFloppyImage.TrackCount
            Get
                Return _TrackCount
            End Get
        End Property

        Public Sub Append(Data() As Byte) Implements IFloppyImage.Append
            ' Unsupported - Do Nothing
        End Sub

        Public Sub CopyTo(SourceIndex As Integer, ByRef DestinationArray() As Byte, DestinationIndex As Integer, Length As Integer) Implements IFloppyImage.CopyTo

            Do While Length > 0
                Dim MappedData = GetMappedData(SourceIndex)
                Dim ActualSize = Math.Min(Length, MappedData.Size)
                Array.Copy(MappedData.Data, MappedData.Offset, DestinationArray, DestinationIndex, ActualSize)
                SourceIndex += ActualSize
                Length -= ActualSize
                DestinationIndex += ActualSize
            Loop
        End Sub

        Public Sub CopyTo(DestinationArray() As Byte, Index As Integer) Implements IFloppyImage.CopyTo
            CopyTo(0, DestinationArray, 0, _ImageSize)
        End Sub

        Public Function GetByte(Offset As UInteger) As Byte Implements IFloppyImage.GetByte
            Dim MappedData = GetMappedData(Offset)

            Return MappedData.Data(MappedData.Offset)
        End Function

        Public Function GetBytes() As Byte() Implements IFloppyImage.GetBytes
            Return GetBytes(0, _ImageSize)
        End Function

        Public Function GetBytes(Offset As UInteger, Size As UInteger) As Byte() Implements IFloppyImage.GetBytes
            Dim temp(Size - 1) As Byte
            Dim Index As Long = 0
            Do While Size > 0
                Dim MappedData = GetMappedData(Offset)
                Dim ActualSize = Math.Min(Size, MappedData.Size)
                Array.Copy(MappedData.Data, MappedData.Offset, temp, Index, ActualSize)
                Offset += ActualSize
                Size -= ActualSize
                Index += ActualSize
            Loop

            Return temp
        End Function

        Public Function GetBytesInteger(Offset As UInteger) As UInteger Implements IFloppyImage.GetBytesInteger
            Dim MappedData = GetMappedData(Offset)

            Return BitConverter.ToUInt32(MappedData.Data, MappedData.Offset)
        End Function

        Public Function GetBytesShort(Offset As UInteger) As UShort Implements IFloppyImage.GetBytesShort
            Dim MappedData = GetMappedData(Offset)

            Return BitConverter.ToUInt16(MappedData.Data, MappedData.Offset)
        End Function

        Public MustOverride Function GetCRC32() As String Implements IFloppyImage.GetCRC32

        Public MustOverride Function GetMD5Hash() As String Implements IFloppyImage.GetMD5Hash

        Public Function GetSector(Track As UShort, Head As Byte, SectorId As Byte) As BitstreamSector
            If Track > _TrackCount - 1 Or Head > _SideCount - 1 Or SectorId > SECTOR_COUNT Then
                Throw New System.IndexOutOfRangeException
            End If

            Dim Index = Track * _SideCount * SECTOR_COUNT + SECTOR_COUNT * Head + (SectorId - 1)

            Return _Sectors(Index)
        End Function

        Public Function GetSectorFromParams(Track As UShort, Side As Byte, SectorId As UShort) As UInteger
            Dim Offset As UInteger = Track * _BPB.NumberOfHeads * _BPB.SectorsPerTrack + _BPB.SectorsPerTrack * Side + (SectorId - 1)

            Return Offset
        End Function

        Public MustOverride Function GetSHA1Hash() As String Implements IFloppyImage.GetSHA1Hash

        Public Function GetTrack(Track As UShort, Head As Byte) As TrackData
            If Track > _TrackCount - 1 Or Head > _SideCount - 1 Then
                Return Nothing
            End If

            Dim Index = Track * _SideCount + Head

            Return _Tracks(Index)
        End Function

        Public Sub InitDiskFormat(DiskFormat As FloppyDiskFormat)
            InferFloppyDiskFormat(DiskFormat)
            InitProtectedSectors()
        End Sub

        Public Function IsProtectedSector(Sector As UInteger) As Boolean Implements IFloppyImage.IsProtectedSector
            Return _ProtectedSectors.Contains(Sector)
        End Function

        Public Function IsTranslatedSector(Sector As UInteger) As Boolean Implements IFloppyImage.IsTranslatedSector
            Return _TranslatedSectors.Contains(Sector)
        End Function

        Public Function Resize(Length As Integer) As Boolean Implements IFloppyImage.Resize
            Return False
        End Function

        Public Overridable Function SaveToFile(FilePath As String) As Boolean Implements IFloppyImage.SaveToFile
            Return _Image.Export(FilePath)
        End Function

        Public Function SetBytes(Value As Object, Offset As UInteger) As Boolean Implements IFloppyImage.SetBytes
            If TypeOf Value Is UShort Then
                Return SetBytes(DirectCast(Value, UShort), Offset)
            ElseIf TypeOf Value Is UInteger Then
                Return SetBytes(DirectCast(Value, UInteger), Offset)
            ElseIf TypeOf Value Is Byte Then
                Return SetBytes(DirectCast(Value, Byte), Offset)
            ElseIf TypeOf Value Is Byte() Then
                Return SetBytes(DirectCast(Value, Byte()), Offset)
            Else
                Return False
            End If
        End Function

        Public Function SetBytes(Value As UShort, Offset As UInteger) As Boolean Implements IFloppyImage.SetBytes
            Dim MappedData = GetMappedData(Offset)

            If Not MappedData.CanWrite Then
                Return False
            End If

            Dim Result As Boolean = False
            Dim CurrentValue = BitConverter.ToUInt16(MappedData.Data, MappedData.Offset)

            If CurrentValue <> Value Then
                If MappedData.CanWrite Then
                    Array.Copy(BitConverter.GetBytes(Value), 0, MappedData.Data, MappedData.Offset, 2)
                    MappedData.MFMSector?.UpdateBitstream()
                    Result = True
                    If _History.Enabled Then
                        _History.AddDataChange(Offset, CurrentValue, Value)
                    End If
                End If
            End If

            Return Result
        End Function

        Public Function SetBytes(Value As UInteger, Offset As UInteger) As Boolean Implements IFloppyImage.SetBytes
            Dim MappedData = GetMappedData(Offset)

            If Not MappedData.CanWrite Then
                Return False
            End If

            Dim Result As Boolean = False
            Dim CurrentValue = BitConverter.ToUInt32(MappedData.Data, MappedData.Offset)

            If CurrentValue <> Value Then
                If MappedData.CanWrite Then
                    Array.Copy(BitConverter.GetBytes(Value), 0, MappedData.Data, MappedData.Offset, 4)
                    MappedData.MFMSector?.UpdateBitstream()
                    Result = True
                    If _History.Enabled Then
                        _History.AddDataChange(Offset, CurrentValue, Value)
                    End If
                End If
            End If

            Return Result
        End Function

        Public Function SetBytes(Value As Byte, Offset As UInteger) As Boolean Implements IFloppyImage.SetBytes
            Dim MappedData = GetMappedData(Offset)

            If Not MappedData.CanWrite Then
                Return False
            End If

            Dim Result As Boolean = False
            Dim CurrentValue = MappedData.Data(MappedData.Offset)

            If CurrentValue <> Value Then
                If MappedData.CanWrite Then
                    MappedData.Data(MappedData.Offset) = Value
                    MappedData.MFMSector?.UpdateBitstream()
                    Result = True
                    If _History.Enabled Then
                        _History.AddDataChange(Offset, CurrentValue, Value)
                    End If
                End If
            End If

            Return Result
        End Function

        Public Function SetBytes(Value() As Byte, Offset As UInteger) As Boolean Implements IFloppyImage.SetBytes
            Return SetBytes(Value, Offset, Value.Length, 0)
        End Function

        Public Function SetBytes(Value() As Byte, Offset As UInteger, Size As UInteger, Padding As Byte) As Boolean Implements IFloppyImage.SetBytes
            Dim Result As Boolean = False

            If Offset + Size > _ImageSize Then
                If _ImageSize - Offset >= 0 Then
                    Size = _ImageSize - Offset
                Else
                    Size = 0
                End If
            End If

            If Size > 0 Then
                If Value.Length <> Size Then
                    ResizeArray(Value, Size, Padding)
                End If

                Dim CurrentValue = GetBytes(Offset, Size)

                If Not CurrentValue.CompareTo(Value) Then
                    Dim Index As Long = 0
                    Dim NewOffset = Offset
                    Do While Size > 0
                        Dim MappedData = GetMappedData(NewOffset)
                        Dim ActualSize = Math.Min(Size, MappedData.Size)
                        If MappedData.CanWrite Then
                            Array.Copy(Value, Index, MappedData.Data, MappedData.Offset, ActualSize)
                            MappedData.MFMSector?.UpdateBitstream()
                            Result = True
                        End If
                        NewOffset += ActualSize
                        Size -= ActualSize
                        Index += ActualSize
                    Loop

                    If Result Then
                        If _History.Enabled Then
                            _History.AddDataChange(Offset, CurrentValue, Value)
                        End If
                    End If
                End If
            End If

            Return Result
        End Function

        Public Sub SetSector(Track As UShort, Head As Byte, SectorId As Byte, Value As BitstreamSector)
            If Track > _TrackCount - 1 Or Head > _SideCount - 1 Or SectorId > SECTOR_COUNT Then
                Throw New System.IndexOutOfRangeException
            End If

            Dim Index = Track * _SideCount * SECTOR_COUNT + SECTOR_COUNT * Head + (SectorId - 1)

            _Sectors(Index) = Value
        End Sub

        Public Function SetTrack(Track As UShort, Head As Byte) As TrackData
            If Track > _TrackCount - 1 Or Head > _SideCount - 1 Then
                Throw New System.IndexOutOfRangeException
            End If

            Dim Index = Track * _SideCount + Head

            Dim TrackData As New TrackData

            _Tracks(Index) = TrackData

            Return TrackData
        End Function

        Public Sub SetTracks(TrackCount As UShort, HeadCount As Byte)
            _TrackCount = TrackCount
            _SideCount = HeadCount

            _Sectors = New BitstreamSector(_TrackCount * _SideCount * SECTOR_COUNT - 1) {}
            _Tracks = New TrackData(_TrackCount * _SideCount - 1) {}
        End Sub

        Public Function ToUInt16(StartIndex As Integer) As UShort Implements IFloppyImage.ToUInt16
            Dim MappedData = GetMappedData(StartIndex)

            Return BitConverter.ToUInt16(MappedData.Data, MappedData.Offset)
        End Function

        Private Sub BuildSectorMap()
            Dim BitstreamTrack As IBitstreamTrack

            Dim TrackCount = Math.Ceiling(_Image.TrackCount / _Image.TrackStep)

            SetTracks(TrackCount, _Image.SideCount)

            For Track As UShort = 0 To _Image.TrackCount - 1 Step _Image.TrackStep
                For Side As Byte = 0 To _Image.SideCount - 1
                    BitstreamTrack = _Image.GetTrack(Track, Side)

                    If BitstreamTrack IsNot Nothing Then
                        Dim MappedTrack = Track \ _Image.TrackStep

                        SetTrack(MappedTrack, Side, BitstreamTrack.MFMData, BitstreamTrack.TrackType)

                        If BitstreamTrack.MFMData IsNot Nothing Then
                            If _BytesPerSector = 512 And BitstreamTrack.MFMData.FirstSectorId = 1 And BitstreamTrack.MFMData.LastSectorId = 4 And BitstreamTrack.MFMData.SectorSize = 1024 Then
                                ProcessMFMSectors1024(Track, Side, BitstreamTrack.MFMData)
                            Else
                                ProcessMFMSectors(MappedTrack, Side, BitstreamTrack.MFMData)
                            End If
                        End If
                    End If
                Next
            Next
        End Sub

        Private Function GetMappedData(Offset As UInteger) As MappedData
            Dim Sector = OffsetToSector(Offset)
            Dim Track = SectorToTrack(Sector)
            Dim Head = SectorToHead(Sector)
            Dim SectorId = SectorToTrackSector(Sector) + 1

            Dim MappedData As New MappedData With {
                .Sector = Sector,
                .Offset = SectorToOffset(Offset),
                .Size = _BPB.BytesPerSector,
                .CanWrite = False
            }

            If Track > _TrackCount - 1 Or Head > _SideCount - 1 Or SectorId > SECTOR_COUNT Then
                MappedData.Data = _EmptySector
            Else
                Dim BitstreamSector = GetSector(Track, Head, SectorId)
                If BitstreamSector Is Nothing Then
                    MappedData.Data = _EmptySector
                Else
                    If BitstreamSector.Size < MappedData.Size Then
                        MappedData.Data = New Byte(MappedData.Size - 1) {}
                        BitstreamSector.Data.CopyTo(MappedData.Data, 0)
                    Else
                        MappedData.Data = BitstreamSector.Data
                        MappedData.MFMSector = BitstreamSector.MFMSector
                        MappedData.CanWrite = BitstreamSector.IsStandard
                    End If
                End If
            End If

            Return MappedData
        End Function

        Private Function GetSectorData(Track As UShort, Head As Byte, SectorId As Byte) As Byte()
            Dim BitstreamSector = GetSector(Track, Head, SectorId)
            If BitstreamSector Is Nothing Then
                Return _EmptySector
            ElseIf BitstreamSector.Size < _BPB.BytesPerSector Then
                Dim Data = New Byte(_BPB.BytesPerSector - 1) {}
                BitstreamSector.Data.CopyTo(Data, 0)
                Return Data
            Else
                Return BitstreamSector.Data
            End If
        End Function

        Private Function GetTrackCount() As UInteger
            Return _BPB.SectorCountSmall \ _BPB.SectorsPerTrack \ _BPB.NumberOfHeads
        End Function

        Private Sub InferFloppyDiskFormat(DiskFormat As FloppyDiskFormat)
            Dim BPBParams = FloppyDiskFormatGetParams(DiskFormat).BPBParams
            _BPB = BPBParams.GetBPB
            _ImageSize = BPBParams.SizeInBytes
            _EmptySector = New Byte(_BPB.BytesPerSector - 1) {}

            Dim Data = GetSectorData(0, 0, 1)
            Dim BPB As New BiosParameterBlock(Data)

            If BPB.IsValid Then
                _BPB = BPB
                _ImageSize = _BPB.SectorCount * _BPB.BytesPerSector
            Else
                Data = GetSectorData(0, 0, 2)
                Dim MediaDescriptor As Byte = 0
                If Data(1) = &HFF And Data(2) = &HFF Then
                    MediaDescriptor = Data(0)
                End If
                Dim FATDiskFormat = FloppyDiskFormatGet(MediaDescriptor)
                If FATDiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                    If _SideCount = 1 Then
                        If FATDiskFormat = FloppyDiskFormat.Floppy360 Then
                            FATDiskFormat = FloppyDiskFormat.Floppy180
                        ElseIf FATDiskFormat = FloppyDiskFormat.Floppy320 Then
                            FATDiskFormat = FloppyDiskFormat.Floppy160
                        End If
                    End If
                    _BPB = BuildBPB(FATDiskFormat)
                    _ImageSize = FloppyDiskFormatGetParams(FATDiskFormat).BPBParams.SizeInBytes
                End If
            End If
        End Sub

        Private Sub InitProtectedSectors()
            _ProtectedSectors.Clear()
            _TranslatedSectors.Clear()
            _NonStandardTracks.Clear()
            _AdditionalTracks.Clear()

            Dim TrackCount = GetTrackCount()

            For Cylinder = 0 To Math.Max(_TrackCount, TrackCount) - 1
                For Side = 0 To _SideCount - 1
                    If Cylinder < TrackCount Then
                        Dim TrackData = GetTrack(Cylinder, Side)
                        Dim TrackIsStandard As Boolean = False
                        If TrackData IsNot Nothing Then
                            If TrackData.Encoding = BitstreamTrackType.MFM Then
                                If TrackData.FirstSectorId > -1 Then
                                    TrackIsStandard = TrackData.FirstSectorId >= 1 And TrackData.LastSectorId <= Math.Max(_BPB.SectorsPerTrack, 9)
                                End If
                            End If

                            For SectorId = 1 To _BPB.SectorsPerTrack
                                Dim Sector = GetSectorFromParams(Cylinder, Side, SectorId)
                                Dim BitstreamSector As BitstreamSector = Nothing
                                If Cylinder < _TrackCount And Side < _SideCount Then
                                    BitstreamSector = GetSector(Cylinder, Side, SectorId)
                                End If
                                If BitstreamSector Is Nothing Then
                                    _ProtectedSectors.Add(Sector)
                                    TrackIsStandard = False
                                Else
                                    If Not BitstreamSector.IsStandard Then
                                        _ProtectedSectors.Add(Sector)
                                        TrackIsStandard = False
                                    End If
                                    If BitstreamSector.IsTranslated Then
                                        _TranslatedSectors.Add(Sector)
                                    End If
                                End If
                            Next
                        Else
                            For SectorId = 1 To _BPB.SectorsPerTrack
                                Dim Sector = GetSectorFromParams(Cylinder, Side, SectorId)
                                _ProtectedSectors.Add(Sector)
                            Next
                        End If

                        If Not TrackIsStandard Then
                            _NonStandardTracks.Add(Cylinder * _SideCount + Side)
                        End If
                    Else
                        _AdditionalTracks.Add(Cylinder * _SideCount + Side)
                    End If
                Next
            Next
        End Sub

        Private Function OffsetToSector(Offset As UInteger) As UInteger
            Return Offset \ _BPB.BytesPerSector
        End Function

        Private Sub ProcessMFMSectors(Track As UShort, Side As Byte, MFMData As IBM_MFM.IBM_MFM_Track)
            Dim BitstreamSector As BitstreamSector
            Dim Sector As BitstreamSector
            Dim SectorSize As UInteger
            Dim IsStandard As Boolean
            Dim Buffer() As Byte

            For Each MFMSector In MFMData.Sectors
                IsStandard = IBM_MFM.IsStandardSector(MFMSector, Track, Side, SECTOR_COUNT)
                If IsStandard Then
                    Sector = GetSector(Track, Side, MFMSector.SectorId)
                    If Sector Is Nothing Then
                        SectorSize = MFMSector.GetSizeBytes
                        If SectorSize > 0 And SectorSize < _BytesPerSector Then
                            Buffer = New Byte(_BytesPerSector - 1) {}
                            Array.Copy(MFMSector.Data, 0, Buffer, 0, SectorSize)
                            BitstreamSector = New BitstreamSector(Buffer, Buffer.Length, False)
                            SetSector(Track, Side, MFMSector.SectorId, BitstreamSector)
                        ElseIf SectorSize = _BytesPerSector Then
                            BitstreamSector = New BitstreamSector(MFMSector, IsStandard)
                            SetSector(Track, Side, MFMSector.SectorId, BitstreamSector)
                        End If
                    Else
                        Sector.IsStandard = False
                    End If
                End If
            Next
        End Sub

        Private Sub ProcessMFMSectors1024(Track As UShort, Side As Byte, MFMData As IBM_MFM.IBM_MFM_Track)
            Dim BitstreamSector As BitstreamSector
            Dim Sector As BitstreamSector
            Dim NewSectorId As Integer
            Dim IsStandard As Boolean
            Dim Buffer() As Byte

            For Each MFMSector In MFMData.Sectors
                IsStandard = IBM_MFM.IsStandardSector(MFMSector, Track, Side, 4)
                If IsStandard And MFMSector.GetSizeBytes = 1024 Then
                    For i = 0 To 1
                        NewSectorId = (MFMSector.SectorId - 1) * 2 + 1 + i
                        Sector = GetSector(Track, Side, NewSectorId)
                        If Sector Is Nothing Then
                            Buffer = New Byte(511) {}
                            Array.Copy(MFMSector.Data, Buffer.Length * i, Buffer, 0, Buffer.Length)
                            BitstreamSector = New BitstreamSector(Buffer, Buffer.Length, False) With {
                                .IsTranslated = True
                            }
                            SetSector(Track, Side, NewSectorId, BitstreamSector)
                        Else
                            Sector.IsStandard = False
                        End If
                    Next
                End If
            Next
        End Sub

        Private Function SectorToHead(Sector As UInteger) As Byte
            Return Int(Sector / _BPB.SectorsPerTrack) Mod _BPB.NumberOfHeads
        End Function

        Private Function SectorToOffset(Offset As UInteger) As UInteger
            Return Offset Mod _BPB.BytesPerSector
        End Function

        Private Function SectorToTrack(Sector As UInteger) As UShort
            Return Sector \ _BPB.SectorsPerTrack \ _BPB.NumberOfHeads
        End Function

        Private Function SectorToTrackSector(Sector As UInteger) As UShort
            Return Sector Mod _BPB.SectorsPerTrack
        End Function

        Private Sub SetTrack(Track As UShort, Head As Byte, MFMData As IBM_MFM.IBM_MFM_Track, Encoding As BitstreamTrackType)
            If Track > _TrackCount - 1 Or Head > _SideCount - 1 Then
                Throw New System.IndexOutOfRangeException
            End If

            Dim Index = Track * _SideCount + Head

            Dim TrackData As New TrackData With {
                .Encoding = Encoding
            }
            If MFMData IsNot Nothing Then
                TrackData.FirstSectorId = MFMData.FirstSectorId
                TrackData.LastSectorId = MFMData.LastSectorId
                TrackData.SectorSize = MFMData.SectorSize
                TrackData.DuplicateSectors = MFMData.DuplicateSectors
                TrackData.OverlappingSectors = MFMData.OverlappingSectors
                TrackData.SectorCount = MFMData.Sectors.Count
            End If

            _Tracks(Index) = TrackData
        End Sub

        Public Class TrackData
            Public Sub New()
                _DuplicateSectors = False
                _FirstSectorId = -1
                _LastSectorId = -1
                _OverlappingSectors = False
                _Encoding = BitstreamTrackType.Other
                _SectorCount = 0
            End Sub

            Public Property DuplicateSectors As Boolean
            Public Property Encoding As BitstreamTrackType
            Public Property FirstSectorId As Short
            Public Property LastSectorId As Short
            Public Property OverlappingSectors As Boolean
            Public Property SectorCount As Integer
            Public Property SectorSize As Integer
        End Class

        Private Class MappedData
            Public Property CanWrite As Boolean
            Public Property Data As Byte()
            Public Property MFMSector As IBM_MFM.IBM_MFM_Sector = Nothing
            Public Property Offset As UInteger
            Public Property Sector As UInteger
            Public Property Size As UInteger
        End Class
    End Class

End Namespace
