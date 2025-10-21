Namespace Bitstream
    Namespace IBM_MFM
        Public Class IBM_MFM_Sector
            Private ReadOnly _Bitstream As BitArray
            Private ReadOnly _IDArea() As Byte
            Private ReadOnly _IDChecksum As UShort
            Private ReadOnly _IDChecksumValid As Boolean
            Private ReadOnly _InitialDataChecksumValid As Boolean
            Private ReadOnly _Offset As UInteger
            Private ReadOnly _OffsetEnd As UInteger
            Private _DAMFound As Boolean = False
            Private _Data As Byte()
            Private _DataChecksum As UShort
            Private _DataFieldSync() As Byte
            Private _DataOffset As UInteger
            Private _InitialDataChecksum As UShort
            Private _Overlaps As Boolean

            Public Sub New(Bitstream As BitArray, Offset As UInteger)
                Dim NumBytes As UInteger

                _Bitstream = Bitstream
                _Overlaps = False
                _Offset = Offset

                NumBytes = MFM_SYNC_MARK_BYTES + MFM_IDAREA_BYTES
                _IDArea = MFMGetBytes(Bitstream, Offset, NumBytes)
                Offset += MFMBytesToBits(NumBytes)

                _IDChecksum = GetChecksum(Bitstream, Offset)
                Offset += MFM_CRC_BITS

                _OffsetEnd = ProcessDataField(Bitstream, Offset)

                _InitialDataChecksumValid = (_DataChecksum = CalculateDataChecksum())
                _IDChecksumValid = (_IDChecksum = CalculateIDChecksum())
            End Sub

            Public ReadOnly Property DAM As MFMAddressMark
                Get
                    Return _DataFieldSync(3)
                End Get
            End Property

            Public ReadOnly Property DAMFound As Boolean
                Get
                    Return _DAMFound
                End Get
            End Property

            Public ReadOnly Property Data As Byte()
                Get
                    Return _Data
                End Get
            End Property

            Public Property DataChecksum As UShort
                Get
                    Return _DataChecksum
                End Get
                Set(value As UShort)
                    _DataChecksum = value
                End Set
            End Property

            Public ReadOnly Property DataOffset As UInteger
                Get
                    Return _DataOffset
                End Get
            End Property

            Public ReadOnly Property IDChecksum As UShort
                Get
                    Return _IDChecksum
                End Get
            End Property

            Public ReadOnly Property IDChecksumValid As Boolean
                Get
                    Return _IDChecksumValid
                End Get
            End Property

            Public ReadOnly Property InitialDataChecksumValid As Boolean
                Get
                    Return _InitialDataChecksumValid
                End Get
            End Property

            Public ReadOnly Property Offset As UInteger
                Get
                    Return _Offset
                End Get
            End Property

            Public ReadOnly Property OffsetEnd As UInteger
                Get
                    Return _OffsetEnd
                End Get
            End Property

            Public Property Overlaps As Boolean
                Get
                    Return _Overlaps
                End Get
                Set
                    _Overlaps = Value
                End Set
            End Property

            Public ReadOnly Property SectorId As Byte
                Get
                    Return _IDArea(6)
                End Get
            End Property

            Public ReadOnly Property Side As Byte
                Get
                    Return _IDArea(5)
                End Get
            End Property

            Public ReadOnly Property Size As Byte
                Get
                    Return _IDArea(7)
                End Get
            End Property

            Public ReadOnly Property Track As Byte
                Get
                    Return _IDArea(4)
                End Get
            End Property
            Public Function CalculateDataChecksum() As UShort
                If _DAMFound Then
                    Dim DataSize = GetSizeBytes()
                    Dim Buffer(_DataFieldSync.Length + DataSize - 1) As Byte
                    Array.Copy(_DataFieldSync, 0, Buffer, 0, _DataFieldSync.Length)
                    Array.Copy(_Data, 0, Buffer, _DataFieldSync.Length, DataSize)

                    Return MFMCRC16(Buffer)
                Else
                    Return 0
                End If
            End Function

            Public Function CalculateIDChecksum() As UShort
                Return MFMCRC16(_IDArea)
            End Function

            Public Function GetDataBitstream(SeedBit As Boolean) As BitArray
                Dim DataSize = GetSizeBytes()
                Dim Buffer(DataSize + MFM_CRC_BYTES - 1) As Byte

                Dim Offset As UInteger = 0
                Array.Copy(_Data, 0, Buffer, Offset, DataSize)
                Offset += DataSize
                Array.Copy(BitConverter.GetBytes(_DataChecksum), 0, Buffer, Offset, MFM_CRC_BYTES)

                Return MFMEncodeBytes(Buffer, SeedBit)
            End Function

            Public Function GetSizeBytes() As UInteger
                Return GetSectorSizeBytes(Size)
            End Function

            Public Function IsModified() As Boolean
                Return _InitialDataChecksum <> _DataChecksum
            End Function

            Public Sub UpdateBitstream()
                UpdateChecksum()

                Dim SeedBit = GetSeedBit(_DataOffset)

                Dim Bitstream = GetDataBitstream(SeedBit)

                For i = 0 To Bitstream.Length - 1
                    _Bitstream(_DataOffset + i) = Bitstream(i)
                Next

                Dim Offset = _DataOffset + Bitstream.Length - 1
                If Offset < _Bitstream.Length - 2 Then
                    Dim PrevDataBit = _Bitstream(Offset)
                    Dim DataBit = _Bitstream(Offset + 2)
                    Dim ClockBit = (Not DataBit And Not PrevDataBit)
                    _Bitstream(Offset + 1) = ClockBit
                End If

            End Sub

            Public Sub UpdateChecksum()
                _DataChecksum = CalculateDataChecksum()
            End Sub

            Private Function GetChecksum(Bitstream As BitArray, Start As Integer) As UShort
                Dim Checksum = MFMGetBytes(Bitstream, Start, MFM_CRC_BYTES)

                Return BitConverter.ToUInt16(Checksum, 0)
            End Function

            Private Function GetSeedBit(Offset As UInteger) As Boolean
                If Offset > 0 Then
                    Return _Bitstream(Offset - 1)
                Else
                    Return True
                End If
            End Function
            Private Function ProcessDataField(BitStream As BitArray, Start As UInteger) As Integer
                Dim MFMPattern = BytesToBits(MFM_SYNC_PATTERN_BYTES)

                _DAMFound = False
                Dim DataFieldSyncIndex = FindPattern(BitStream, MFMPattern, Start)
                If DataFieldSyncIndex > -1 Then
                    Dim DAM As MFMAddressMark = MFMGetByte(BitStream, DataFieldSyncIndex + MFM_SYNC_BITS)
                    If DAM = MFMAddressMark.Data Or DAM = MFMAddressMark.DeletedData Then
                        _DAMFound = True
                    End If
                End If

                If _DAMFound Then
                    _DataFieldSync = MFMGetBytes(BitStream, DataFieldSyncIndex, MFM_SYNC_MARK_BYTES)
                    Dim Size = GetSizeBytes()
                    _DataOffset = DataFieldSyncIndex + MFMPattern.Length + MFM_BYTE_BITS
                    _Data = MFMGetBytes(BitStream, _DataOffset, Size)
                    Start = _DataOffset + MFMBytesToBits(Size)
                    Start = Start Mod BitStream.Length
                    _DataChecksum = GetChecksum(BitStream, Start)
                    _InitialDataChecksum = _DataChecksum
                    Start += MFM_CRC_BITS
                Else
                    _DataFieldSync = New Byte(MFM_SYNC_MARK_BYTES - 1) {}
                    _DataOffset = 0
                    _Data = New Byte(-1) {}
                    _DataChecksum = 0
                End If

                Return Start
            End Function
        End Class
    End Namespace
End Namespace
