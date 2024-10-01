
Namespace Bitstream
    Namespace IBM_MFM
        Public Class IBM_MFM_Sector
            Private ReadOnly _Offset As UInteger
            Private ReadOnly _IDArea() As Byte
            Private ReadOnly _IDChecksum As UShort
            Private ReadOnly _IsValid As Boolean
            Private _Gap2 As Byte()
            Private _DataFieldSync() As Byte
            Private _DataOffset As UInteger
            Private _Data As Byte()
            Private _DataChecksum As UShort
            Private _InitialDataChecksum As UShort
            Private _Gap3 As Byte()
            Private _DAMFound As Boolean = False

            Public Sub New(Bitstream As BitArray, Offset As UInteger)
                _Offset = Offset
                _IDArea = MFMGetBytes(Bitstream, Offset, 8)
                _IDChecksum = GetChecksum(Bitstream, Offset + 128)
                _Overlaps = False

                Dim DataOffset = ProcessDataField(Bitstream, Offset + 160)

                ProcessGap3(Bitstream, DataOffset)

                _IsValid = (_DataChecksum = CalculateDataChecksum())
            End Sub

            Public ReadOnly Property Gap2 As Byte()
                Get
                    Return _Gap2
                End Get
            End Property

            Public ReadOnly Property Gap3 As Byte()
                Get
                    Return _Gap3
                End Get
            End Property

            Public ReadOnly Property Offset As UInteger
                Get
                    Return _Offset
                End Get
            End Property

            Public ReadOnly Property Track As Byte
                Get
                    Return _IDArea(4)
                End Get
            End Property

            Public ReadOnly Property Side As Byte
                Get
                    Return _IDArea(5)
                End Get
            End Property

            Public ReadOnly Property SectorId As Byte
                Get
                    Return _IDArea(6)
                End Get
            End Property

            Public ReadOnly Property Size As Byte
                Get
                    Return _IDArea(7)
                End Get
            End Property

            Public ReadOnly Property IDChecksum As UShort
                Get
                    Return _IDChecksum
                End Get
            End Property

            Public ReadOnly Property DAMFound As Boolean
                Get
                    Return _DAMFound
                End Get
            End Property

            Public ReadOnly Property DAM As MFMAddressMark
                Get
                    Return _DataFieldSync(3)
                End Get
            End Property

            Public ReadOnly Property Data As Byte()
                Get
                    Return _Data
                End Get
            End Property

            Public ReadOnly Property DataOffset As UInteger
                Get
                    Return _DataOffset
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

            Public Property Overlaps As Boolean

            Public ReadOnly Property IsValid As Boolean
                Get
                    Return _IsValid
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

            Public Function GetDataBitstream() As BitArray
                Dim DataSize = GetSizeBytes()
                Dim Buffer(DataSize + 2 - 1) As Byte

                Dim Offset As UInteger = 0
                Array.Copy(_Data, 0, Buffer, Offset, DataSize)
                Offset += DataSize
                Array.Copy(BitConverter.GetBytes(_DataChecksum), 0, Buffer, Offset, 2)

                Return MFMEncodeBytes(Buffer, 1)
            End Function

            Public Function GetSizeBytes() As UInteger
                Dim Size = Me.Size

                If Size > 7 Then
                    Size = 7
                End If
                Return (2 ^ Size) * 128
            End Function

            Public Function IsModified() As Boolean
                Return _InitialDataChecksum <> _DataChecksum
            End Function

            Public Sub UpdateChecksum()
                _DataChecksum = CalculateDataChecksum()
            End Sub

            Private Function GetChecksum(Bitstream As BitArray, Start As Integer) As UShort
                Dim Checksum = MFMGetBytes(Bitstream, Start, 2)

                Return BitConverter.ToUInt16(Checksum, 0)
            End Function

            Private Function ProcessDataField(BitStream As BitArray, Start As UInteger) As Integer
                Dim MFMPattern = BytesToBits(MFM_Sync_Bytes)

                _DAMFound = False
                Dim DataFieldSyncIndex = FindPattern(BitStream, MFMPattern, Start)
                If DataFieldSyncIndex > -1 Then
                    Dim DAM As MFMAddressMark = MFMGetByte(BitStream, DataFieldSyncIndex + 3 * MFM_BYTE_SIZE)
                    If DAM = MFMAddressMark.Data Or DAM = MFMAddressMark.DeletedData Then
                        _DAMFound = True
                    End If
                End If

                If _DAMFound Then
                    _DataFieldSync = MFMGetBytes(BitStream, DataFieldSyncIndex, 4)
                    Dim Size = GetSizeBytes()
                    _Gap2 = GetGapBytes(BitStream, Start, DataFieldSyncIndex - MFM_SYNC_SIZE_BITS)
                    _DataOffset = DataFieldSyncIndex + MFMPattern.Length + MFM_BYTE_SIZE
                    _Data = MFMGetBytes(BitStream, _DataOffset, Size)
                    Start = _DataOffset + Size * MFM_BYTE_SIZE
                    Start = Start Mod BitStream.Length
                    _DataChecksum = GetChecksum(BitStream, Start)
                    _InitialDataChecksum = _DataChecksum
                    Start += 2 * MFM_BYTE_SIZE
                Else
                    _DataFieldSync = New Byte(3) {}
                    _Gap2 = New Byte(-1) {}
                    _DataOffset = 0
                    _Data = New Byte(-1) {}
                    _DataChecksum = 0
                End If

                Return Start
            End Function

            Private Sub ProcessGap3(Bitstream As BitArray, Offset As UInteger)
                Dim IDAMPattern = BytesToBits(IDAM_Sync_Bytes)

                Dim IDAMFieldSyncIndex = FindPattern(Bitstream, IDAMPattern, Offset)
                If IDAMFieldSyncIndex = -1 Then
                    IDAMFieldSyncIndex = Bitstream.Length
                Else
                    IDAMFieldSyncIndex -= MFM_SYNC_SIZE_BITS
                End If

                _Gap3 = GetGapBytes(Bitstream, Offset, IDAMFieldSyncIndex)
            End Sub
        End Class
    End Namespace
End Namespace
