Public Class MFMTrack
    Private Const MFM_BYTE_SIZE As Byte = 16
    Private Const SYNC_NULL_LENGTH As Integer = 12 * MFM_BYTE_SIZE
    Private Shared ReadOnly IAM_Sync_Bytes() As Byte = {&H52, &H24, &H52, &H24, &H52, &H24}
    Private Shared ReadOnly IDAM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89, &H55, &H54}
    Private Shared ReadOnly DAM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89, &H55, &H45}
    Private Shared ReadOnly MFM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89}

    Public Enum MFMAddressMarks
        Index = &HFC
        ID = &HFE
        Data = &HFB
    End Enum

    Private Enum MFMIDFieldOffsets
        Track = 4 * MFM_BYTE_SIZE
        Side = 5 * MFM_BYTE_SIZE
        SectorId = 6 * MFM_BYTE_SIZE
        Size = 7 * MFM_BYTE_SIZE
        Checksum = 8 * MFM_BYTE_SIZE
        Data = 10 * MFM_BYTE_SIZE
    End Enum

    Public Sub New(Data() As Byte)
        MFMDecode(BytesToBits(Data))
    End Sub

    Public Sub New(Bitstream As BitArray)
        MFMDecode(Bitstream)
    End Sub

    Public Property Gap1 As Byte()
    Public Property Gap4A As Byte()
    Public Property IAMMark As Byte
    Public Property Sectors As List(Of MFMSector)

    Private Sub MFMDecode(Bitstream As BitArray)
        Dim IAMPattern = BytesToBits(IAM_Sync_Bytes)
        Dim Start As UInteger = 0

        Bitstream = BitstreamAlign(Bitstream)

        'Index Field Sync
        Dim Pos = FindPattern(Bitstream, IAMPattern, Start)
        If Pos > -1 Then
            _Gap4A = GetGapBytes(Bitstream, Start, Pos - SYNC_NULL_LENGTH)
            Start = Pos + IAMPattern.Length
            _IAMMark = MFMGetByte(Bitstream, Start)
            Start += MFM_BYTE_SIZE
        Else
            _Gap4A = New Byte(-1) {}
            _IAMMark = 0
        End If

        Dim SectorList = GetSectorList(Bitstream)

        _Sectors = New List(Of MFMSector)

        If SectorList.Count > 0 Then
            _Gap1 = GetGapBytes(Bitstream, Start, SectorList.Item(0) - SYNC_NULL_LENGTH)
            ProcessSectorList(Bitstream, SectorList)
        Else
            _Gap1 = New Byte(-1) {}
        End If
    End Sub

    Private Sub ProcessSectorList(BitStream As BitArray, SectorList As List(Of UInteger))
        Dim DAMPattern = BytesToBits(DAM_Sync_Bytes)
        Dim IDAMPattern = BytesToBits(IDAM_Sync_Bytes)
        Dim ChecksumData() As Byte
        Dim Pos As Integer
        Dim DataOffset As UInteger
        Dim SectorIndex As UShort = 0
        Dim NextSectorOffset As UInteger
        Dim DataEnd As UInteger

        For Each SectorOffset In SectorList
            ChecksumData = MFMGetBytes(BitStream, SectorOffset, 8)
            Dim Sector = New MFMSector With {
                .Track = MFMGetByte(BitStream, SectorOffset + MFMIDFieldOffsets.Track),
                .Side = MFMGetByte(BitStream, SectorOffset + MFMIDFieldOffsets.Side),
                .SectorId = MFMGetByte(BitStream, SectorOffset + MFMIDFieldOffsets.SectorId),
                .Size = SectorGetSize(MFMGetByte(BitStream, SectorOffset + MFMIDFieldOffsets.Size)),
                .Checksum = MFMGetChecksum(BitStream, SectorOffset + MFMIDFieldOffsets.Checksum),
                .CalculatedChecksum = Crc16BigEndian(ChecksumData),
                .Overlaps = False,
                .Offset = SectorOffset \ MFM_BYTE_SIZE
            }

            DataOffset = SectorOffset + MFMIDFieldOffsets.Data
            DataEnd = DataOffset + Sector.Size * MFM_BYTE_SIZE

            If SectorIndex < SectorList.Count - 1 Then
                NextSectorOffset = SectorList.Item(SectorIndex + 1)
                If DataEnd >= NextSectorOffset Then
                    Sector.Overlaps = True
                End If
            ElseIf DataEnd > BitStream.Length Then
                Sector.Overlaps = True
            End If

            'Data Field Sync 
            Pos = FindPattern(BitStream, DAMPattern, DataOffset)
            If Pos > -1 Then
                Sector.Gap2 = GetGapBytes(BitStream, DataOffset, Pos - SYNC_NULL_LENGTH)
                ChecksumData = MFMGetBytes(BitStream, Pos, Sector.Size + DAMPattern.Length \ MFM_BYTE_SIZE)
                DataOffset = Pos + DAMPattern.Length
                Sector.Data = MFMGetBytes(BitStream, DataOffset, Sector.Size)
                DataOffset += Sector.Size * MFM_BYTE_SIZE
                DataOffset = DataOffset Mod BitStream.Length
                Sector.DataChecksum = MFMGetChecksum(BitStream, DataOffset)
                Sector.CalculatedDataChecksum = Crc16BigEndian(ChecksumData)
                DataOffset += 2 * MFM_BYTE_SIZE
            End If

            'Gap 3
            Pos = FindPattern(BitStream, IDAMPattern, DataOffset)
            If Pos = -1 Then
                Pos = BitStream.Length
            Else
                Pos -= SYNC_NULL_LENGTH
            End If
            Sector.Gap3 = GetGapBytes(BitStream, DataOffset, Pos)

            _Sectors.Add(Sector)

            SectorIndex += 1
        Next
    End Sub

    Public Shared Function BitstreamAlign(Bitstream As BitArray) As BitArray
        Dim MFMPattern = BytesToBits(MFM_Sync_Bytes)
        Dim NewBitstream = Bitstream

        Dim Pos = FindPattern(Bitstream, MFMPattern, 0)
        If Pos > -1 Then
            Dim Offset = Pos Mod MFM_BYTE_SIZE
            If Offset > 0 Then
                NewBitstream = New BitArray(Bitstream.Length)
                For i = 0 To Bitstream.Length - 1
                    Dim NewOffset = i - Offset
                    If NewOffset < 0 Then
                        NewOffset = Bitstream.Length + NewOffset
                    End If
                    NewBitstream(NewOffset) = Bitstream(i)
                Next
            End If
        End If

        Return NewBitstream
    End Function

    Public Shared Function DecodeTrack(Bitstream As BitArray) As Byte()
        Bitstream = BitstreamAlign(Bitstream)

        Return MFMGetBytes(Bitstream, 0, Bitstream.Length \ MFM_BYTE_SIZE)
    End Function

    Public Shared Function DecodeTrack(Data() As Byte) As Byte()
        Return DecodeTrack(BytesToBits(Data))
    End Function

    Private Shared Function FindPattern(BitStream As BitArray, Pattern As BitArray, Optional Start As Integer = 0) As Integer
        Dim Found As Boolean = False
        Dim i As Integer

        If BitStream.Length - Pattern.Length - Start > 0 Then
            Found = True
            For i = Start To BitStream.Length - Pattern.Length
                Found = True
                For j = 0 To Pattern.Length - 1
                    If Pattern(j) <> BitStream(i + j) Then
                        Found = False
                        Exit For
                    End If
                Next
                If Found Then
                    Exit For
                End If
            Next
        End If

        If Found Then
            Return i
        Else
            Return -1
        End If
    End Function

    Private Shared Function GetGapBytes(BitStream As BitArray, OffsetStart As Integer, OffsetEnd As Integer) As Byte()
        Dim NumBytes As Integer

        If OffsetEnd < OffsetStart Then
            NumBytes = OffsetEnd - (OffsetStart - BitStream.Length)
        Else
            NumBytes = OffsetEnd - OffsetStart
        End If

        NumBytes \= MFM_BYTE_SIZE

        If NumBytes > 0 Then
            Return MFMGetBytes(BitStream, OffsetStart, NumBytes)
        End If

        Return New Byte(-1) {}
    End Function

    Private Shared Function GetSectorList(BitStream As BitArray) As List(Of UInteger)
        Dim IDAMPattern = BytesToBits(IDAM_Sync_Bytes)

        Dim Start As UInteger = 0
        Dim Pos As Integer
        Dim SectorList As New List(Of UInteger)
        Do
            Pos = FindPattern(BitStream, IDAMPattern, Start)
            If Pos > -1 Then
                SectorList.Add(Pos)
                Start = Pos + IDAMPattern.Length
            End If
        Loop Until Pos = -1

        Return SectorList
    End Function

    Private Shared Function MFMGetByte(Bitstream As BitArray, Start As Integer) As Byte
        Return MFMGetBytes(Bitstream, Start, 1)(0)
    End Function

    Private Shared Function MFMGetBytes(Bitstream As BitArray, Start As UInteger, NumBytes As UInteger) As Byte()
        Dim decodedBytes(NumBytes - 1) As Byte
        Dim byteBuffer As Byte
        Dim bitCount As Integer = 0
        Dim byteCount As Integer = 0

        If NumBytes > 0 Then
            Dim Length = Bitstream.Length - 1

            For i As UInteger = 0 To NumBytes * 8 - 1
                Dim clockBit As Boolean = Bitstream(Start)
                Start += 1
                If Start >= Length Then
                    Start = 0
                End If
                Dim dataBit As Boolean = Bitstream(Start)
                Start += 1
                If Start >= Length Then
                    Start = 0
                End If

                byteBuffer = (byteBuffer << 1) Or (CInt(dataBit) And 1)
                bitCount += 1

                If bitCount = 8 Then
                    decodedBytes(byteCount) = byteBuffer
                    byteBuffer = 0
                    bitCount = 0
                    byteCount += 1
                End If
            Next
        End If

        Return decodedBytes
    End Function

    Private Shared Function MFMGetChecksum(Bitstream As BitArray, Start As Integer) As UShort
        Dim Checksum = MFMGetBytes(Bitstream, Start, 2)

        Return BitConverter.ToUInt16(Checksum, 0)
    End Function

    Private Shared Function SectorGetSize(Size As Byte) As UInteger
        If Size > 7 Then
            Size = 7
        End If
        Return (2 ^ Size) * 128
    End Function
End Class

Public Class MFMSector
    Public Sub New()
        _Gap2 = New Byte(-1) {}
        _Gap3 = New Byte(-1) {}
        _Overlaps = False
    End Sub

    Public Property Gap2 As Byte()
    Public Property Gap3 As Byte()
    Public Property Track As Byte
    Public Property Side As Byte
    Public Property SectorId As Byte
    Public Property Size As UInteger
    Public Property Checksum As UShort
    Public Property CalculatedChecksum As UShort
    Public Property Data As Byte()
    Public Property DataChecksum As UShort
    Public Property CalculatedDataChecksum As UShort
    Public Property Overlaps As Boolean
    Public Property Offset As UInteger
End Class


