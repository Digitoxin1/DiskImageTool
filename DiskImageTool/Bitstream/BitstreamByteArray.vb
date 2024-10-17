Imports DiskImageTool.DiskImage

Namespace Bitstream
    Public MustInherit Class BitstreamByteArray
        Implements IByteArray

        Friend Const SECTOR_COUNT As Byte = 36
        Private ReadOnly _ProtectedSectors As HashSet(Of UInteger)
        Private ReadOnly _NonStandardTracks As HashSet(Of UShort)
        Private ReadOnly _EmptySector() As Byte
        Private _DiskFormat As FloppyDiskFormat
        Private _BPB As BiosParameterBlock
        Private _ImageSize As Integer
        Private _TrackCount As UShort
        Private _HeadCount As Byte
        Private _Sectors() As BitstreamSector
        Private _Tracks() As TrackData
        Public Event DataChanged As DataChangedEventHandler Implements IByteArray.DataChanged
        Public Event SizeChanged As SizeChangedEventHandler Implements IByteArray.SizeChanged

        Private Class TrackData
            Public Property FirstSector As Integer
            Public Property LastSector As Integer
        End Class

        Private Class MappedData
            Public Property Sector As UInteger
            Public Property Offset As UInteger
            Public Property Data As Byte()
            Public Property CanWrite As Boolean
            Public Property Size As UInteger
        End Class

        Public Sub New()
            _ProtectedSectors = New HashSet(Of UInteger)
            _NonStandardTracks = New HashSet(Of UShort)
            _EmptySector = New Byte(Disk.BYTES_PER_SECTOR - 1) {}
        End Sub

        Public ReadOnly Property CanResize As Boolean Implements IByteArray.CanResize
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property DiskFormat As FloppyDiskFormat
            Get
                Return _DiskFormat
            End Get
        End Property

        Public ReadOnly Property HeadCount As Byte Implements IByteArray.HeadCount
            Get
                Return _HeadCount
            End Get
        End Property

        Public ReadOnly Property NonStandardTracks As HashSet(Of UShort) Implements IByteArray.NonStandardTracks
            Get
                Return _NonStandardTracks
            End Get
        End Property

        Public ReadOnly Property ProtectedSectors As HashSet(Of UInteger) Implements IByteArray.ProtectedSectors
            Get
                Return _ProtectedSectors
            End Get
        End Property

        Public ReadOnly Property Sectors As BitstreamSector()
            Get
                Return _Sectors
            End Get
        End Property

        Public ReadOnly Property TrackCount As UShort Implements IByteArray.TrackCount
            Get
                Return _TrackCount
            End Get
        End Property

        Friend Sub InitDiskFormat(DiskFormat As FloppyDiskFormat)
            InferFloppyDiskFormat(DiskFormat)

            If _DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                InitProtectedSectors()
            End If
        End Sub

        Friend Function GetSector(Track As UShort, Head As Byte, SectorId As Byte) As BitstreamSector
            If Track > _TrackCount - 1 Or Head > _HeadCount - 1 Or SectorId > SECTOR_COUNT Then
                Throw New System.IndexOutOfRangeException
            End If

            Dim Index = Track * _HeadCount * SECTOR_COUNT + SECTOR_COUNT * Head + (SectorId - 1)

            Return _Sectors(Index)
        End Function

        Private Function GetTrack(Track As UShort, Head As Byte) As TrackData
            If Track > _TrackCount - 1 Or Head > _HeadCount - 1 Then
                Return Nothing
            End If

            Dim Index = Track * _HeadCount + Head

            Return _Tracks(Index)
        End Function

        Friend Function GetSectorFromParams(Track As UShort, Side As Byte, SectorId As UShort) As UInteger
            Dim Offset As UInteger = Track * _BPB.NumberOfHeads * _BPB.SectorsPerTrack + _BPB.SectorsPerTrack * Side + (SectorId - 1)

            Return Offset
        End Function

        Friend Sub SetSector(Track As UShort, Head As Byte, SectorId As Byte, Value As BitstreamSector)
            If Track > _TrackCount - 1 Or Head > _HeadCount - 1 Or SectorId > SECTOR_COUNT Then
                Throw New System.IndexOutOfRangeException
            End If

            Dim Index = Track * _HeadCount * SECTOR_COUNT + SECTOR_COUNT * Head + (SectorId - 1)

            _Sectors(Index) = Value
        End Sub

        Friend Sub SetTrack(Track As UShort, Head As Byte, FirstSector As Integer, LastSector As Integer)
            If Track > _TrackCount - 1 Or Head > _HeadCount - 1 Then
                Throw New System.IndexOutOfRangeException
            End If

            Dim Index = Track * _HeadCount + Head

            Dim TrackData As New TrackData With {
                .FirstSector = FirstSector,
                .LastSector = LastSector
            }

            _Tracks(Index) = TrackData
        End Sub

        Friend Sub SetTracks(TrackCount As UShort, HeadCount As Byte)
            _TrackCount = TrackCount
            _HeadCount = HeadCount

            _Sectors = New BitstreamSector(_TrackCount * _HeadCount * SECTOR_COUNT - 1) {}
            _Tracks = New TrackData(_TrackCount * _HeadCount - 1) {}
        End Sub

        Private Function GetMappedData(Offset As UInteger) As MappedData
            Dim Sector = OffsetToSector(Offset)
            Dim Track = SectorToTrack(Sector)
            Dim Head = SectorToHead(Sector)
            Dim SectorId = SectorToTrackSector(Sector) + 1

            Dim MappedData As New MappedData With {
                .Sector = Sector,
                .Offset = SectorToOffset(Offset),
                .Size = Disk.BYTES_PER_SECTOR
            }

            If Track > _TrackCount - 1 Or Head > _HeadCount - 1 Or SectorId > SECTOR_COUNT Then
                MappedData.Data = _EmptySector
                MappedData.CanWrite = False
            Else
                Dim BitstreamSector = GetSector(Track, Head, SectorId)
                If BitstreamSector Is Nothing Then
                    MappedData.Data = _EmptySector
                    MappedData.CanWrite = False
                Else
                    If BitstreamSector.Size < MappedData.Size Then
                        MappedData.Data = New Byte(MappedData.Size - 1) {}
                        BitstreamSector.Data.CopyTo(MappedData.Data, 0)
                    Else
                        MappedData.Data = BitstreamSector.Data
                    End If
                    MappedData.CanWrite = BitstreamSector.IsStandard
                End If
            End If

            Return MappedData
        End Function

        Private Function GetSectorData(Track As UShort, Head As Byte, SectorId As Byte) As Byte()
            Dim BitstreamSector = GetSector(Track, Head, SectorId)
            If BitstreamSector Is Nothing Then
                Return _EmptySector
            ElseIf BitstreamSector.Size < Disk.BYTES_PER_SECTOR Then
                Dim Data = New Byte(Disk.BYTES_PER_SECTOR - 1) {}
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
            _BPB = BuildBPB(DiskFormat)
            _DiskFormat = DiskFormat
            _ImageSize = GetFloppyDiskSize(DiskFormat)

            Dim Data = GetSectorData(0, 0, 1)
            Dim BPB = New BiosParameterBlock(Data)

            If BPB.IsValid Then
                _BPB = BPB
                _DiskFormat = GetFloppyDiskFormat(BPB, False)
                _ImageSize = _BPB.SectorCount * Disk.BYTES_PER_SECTOR
            Else
                Data = GetSectorData(0, 0, 2)
                Dim MediaDescriptor = Data(0)
                Dim FATDiskFormat = GetFloppyDiskFomat(MediaDescriptor)
                If FATDiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                    If _HeadCount = 1 Then
                        If FATDiskFormat = FloppyDiskFormat.Floppy360 Then
                            FATDiskFormat = FloppyDiskFormat.Floppy180
                        ElseIf FATDiskFormat = FloppyDiskFormat.Floppy320 Then
                            FATDiskFormat = FloppyDiskFormat.Floppy160
                        End If
                    End If
                    _BPB = BuildBPB(FATDiskFormat)
                    _DiskFormat = FATDiskFormat
                    _ImageSize = GetFloppyDiskSize(_DiskFormat)
                End If
            End If
        End Sub

        Private Sub InitProtectedSectors()
            _ProtectedSectors.Clear()
            _NonStandardTracks.Clear()

            Dim TrackCount = GetTrackCount()

            For Cylinder = 0 To TrackCount - 1
                For Side = 0 To _BPB.NumberOfHeads - 1
                    Dim TrackData = GetTrack(Cylinder, Side)
                    Dim TrackIsStandard As Boolean = True
                    If TrackData IsNot Nothing Then
                        TrackIsStandard = TrackData.FirstSector >= 1 And TrackData.LastSector <= Math.Max(_BPB.SectorsPerTrack, 9)
                    End If
                    For SectorId = 1 To _BPB.SectorsPerTrack
                        Dim BitstreamSector As BitstreamSector = Nothing
                        If Cylinder < _TrackCount And Side < _HeadCount Then
                            BitstreamSector = GetSector(Cylinder, Side, SectorId)
                        End If
                        If BitstreamSector Is Nothing OrElse Not BitstreamSector.IsStandard Then
                            Dim Sector = GetSectorFromParams(Cylinder, Side, SectorId)
                            ProtectedSectors.Add(Sector)
                            TrackIsStandard = False
                        End If
                    Next
                    If Not TrackIsStandard Then
                        _NonStandardTracks.Add(Cylinder * _BPB.NumberOfHeads + Side)
                    End If
                Next
            Next
        End Sub

        Private Function OffsetToSector(Offset As UInteger) As UInteger
            Return Offset \ Disk.BYTES_PER_SECTOR
        End Function

        Private Function SectorToHead(Sector As UInteger) As Byte
            Return Int(Sector / _BPB.SectorsPerTrack) Mod _BPB.NumberOfHeads
        End Function

        Private Function SectorToOffset(Offset As UInteger) As UInteger
            Return Offset Mod Disk.BYTES_PER_SECTOR
        End Function

        Private Function SectorToTrack(Sector As UInteger) As UShort
            Return Sector \ _BPB.SectorsPerTrack \ _BPB.NumberOfHeads
        End Function

        Private Function SectorToTrackSector(Sector As UInteger) As UShort
            Return Sector Mod _BPB.SectorsPerTrack
        End Function

        Public MustOverride ReadOnly Property ImageType As FloppyImageType Implements IByteArray.ImageType

        Public ReadOnly Property Length As Integer Implements IByteArray.Length
            Get
                Return _ImageSize
            End Get
        End Property

        Public Sub Append(Data() As Byte) Implements IByteArray.Append
            ' Unsupported - Do Nothing
        End Sub

        Public Sub CopyTo(SourceIndex As Integer, ByRef DestinationArray() As Byte, DestinationIndex As Integer, Length As Integer) Implements IByteArray.CopyTo

            Do While Length > 0
                Dim MappedData = GetMappedData(SourceIndex)
                Dim ActualSize = Math.Min(Length, MappedData.Size)
                Array.Copy(MappedData.Data, MappedData.Offset, DestinationArray, DestinationIndex, ActualSize)
                SourceIndex += ActualSize
                Length -= ActualSize
                DestinationIndex += ActualSize
            Loop
        End Sub

        Public Sub CopyTo(DestinationArray() As Byte, Index As Integer) Implements IByteArray.CopyTo
            CopyTo(0, DestinationArray, 0, _ImageSize)
        End Sub

        Public Function GetByte(Offset As UInteger) As Byte Implements IByteArray.GetByte
            Dim MappedData = GetMappedData(Offset)

            Return MappedData.Data(MappedData.Offset)
        End Function

        Public Function GetBytes() As Byte() Implements IByteArray.GetBytes
            Return GetBytes(0, _ImageSize)
        End Function

        Public Function GetBytes(Offset As UInteger, Size As UInteger) As Byte() Implements IByteArray.GetBytes

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

        Public Function GetBytesInteger(Offset As UInteger) As UInteger Implements IByteArray.GetBytesInteger
            Dim MappedData = GetMappedData(Offset)

            Return BitConverter.ToUInt32(MappedData.Data, MappedData.Offset)
        End Function

        Public Function GetBytesShort(Offset As UInteger) As UShort Implements IByteArray.GetBytesShort
            Dim MappedData = GetMappedData(Offset)

            Return BitConverter.ToUInt16(MappedData.Data, MappedData.Offset)
        End Function

        Public Function Resize(Length As Integer) As Boolean Implements IByteArray.Resize
            Return False
        End Function

        Public Function SetBytes(Value As UShort, Offset As UInteger) As Boolean Implements IByteArray.SetBytes
            Dim MappedData = GetMappedData(Offset)

            If Not MappedData.CanWrite Then
                Return False
            End If

            Dim Result As Boolean = False
            Dim CurrentValue = BitConverter.ToUInt16(MappedData.Data, MappedData.Offset)

            If CurrentValue <> Value Then
                Array.Copy(BitConverter.GetBytes(Value), 0, MappedData.Data, MappedData.Offset, 2)
                Result = True
                RaiseEvent DataChanged(Offset, CurrentValue, Value)
            End If

            Return Result
        End Function

        Public Function SetBytes(Value As UInteger, Offset As UInteger) As Boolean Implements IByteArray.SetBytes
            Dim MappedData = GetMappedData(Offset)

            If Not MappedData.CanWrite Then
                Return False
            End If

            Dim Result As Boolean = False
            Dim CurrentValue = BitConverter.ToUInt32(MappedData.Data, MappedData.Offset)

            If CurrentValue <> Value Then
                Array.Copy(BitConverter.GetBytes(Value), 0, MappedData.Data, MappedData.Offset, 4)
                Result = True
                RaiseEvent DataChanged(Offset, CurrentValue, Value)
            End If

            Return Result
        End Function

        Public Function SetBytes(Value As Byte, Offset As UInteger) As Boolean Implements IByteArray.SetBytes
            Dim MappedData = GetMappedData(Offset)

            If Not MappedData.CanWrite Then
                Return False
            End If

            Dim Result As Boolean = False
            Dim CurrentValue = MappedData.Data(MappedData.Offset)

            If CurrentValue <> Value Then
                MappedData.Data(MappedData.Offset) = Value
                Result = True
                RaiseEvent DataChanged(Offset, CurrentValue, Value)
            End If

            Return Result
        End Function

        Public Function SetBytes(Value() As Byte, Offset As UInteger) As Boolean Implements IByteArray.SetBytes
            Return SetBytes(Value, Offset, Value.Length, 0)
        End Function

        Public Function SetBytes(Value() As Byte, Offset As UInteger, Size As UInteger, Padding As Byte) As Boolean Implements IByteArray.SetBytes
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
                            Result = True
                        End If
                        NewOffset += ActualSize
                        Size -= ActualSize
                        Index += ActualSize
                    Loop

                    If Result Then
                        RaiseEvent DataChanged(Offset, CurrentValue, Value)
                    End If
                End If
            End If

            Return Result
        End Function

        Public Function ToUInt16(StartIndex As Integer) As UShort Implements IByteArray.ToUInt16
            Dim MappedData = GetMappedData(StartIndex)

            Return BitConverter.ToUInt16(MappedData.Data, MappedData.Offset)
        End Function

        Public MustOverride Function GetCRC32() As String Implements IByteArray.GetCRC32

        Public MustOverride Function GetMD5Hash() As String Implements IByteArray.GetMD5Hash

        Public MustOverride Function GetSHA1Hash() As String Implements IByteArray.GetSHA1Hash

        Public MustOverride Function SaveToFile(FilePath As String) As Boolean Implements IByteArray.SaveToFile
    End Class

End Namespace
