Public Module MFM
    Private Const IAM As Byte = &HFC
    Private Const SyncBytesSize As Byte = 12
    Private ReadOnly IAM_Sync_Bytes() As Byte = {&H52, &H24, &H52, &H24, &H52, &H24}
    Private ReadOnly IDAM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89, &H55, &H54}
    Private ReadOnly DAM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89, &H55, &H45}
    Private ReadOnly MFM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89}

    Private Class FieldSyncResponse
        Public Sub New(Offset As UInteger)
            _OffsetStart = Offset
            _OffsetEnd = Offset
            _Found = False
            _Mark = 0
        End Sub

        Public Property Found As Boolean
        Public Property Mark As Byte
        Public Property OffsetStart As UInteger
        Public Property OffsetEnd As UInteger
    End Class

    Private Function BitstreamAlign(Bitstream As BitArray) As BitArray
        Dim MFMPattern = BytesToBits(MFM_Sync_Bytes)
        Dim NewBitstream = Bitstream

        Dim Pos = FindPattern(Bitstream, MFMPattern, 0)
        If Pos > -1 Then
            Dim Offset = Pos Mod 16
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

    Private Function FindPattern(BitStream As BitArray, Pattern As BitArray, Optional Start As Integer = 0) As Integer
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

    Private Function GetGapBytes(BitStream As BitArray, OffsetStart As Integer, OffsetEnd As Integer) As Byte()
        Dim NumBytes As Integer

        If OffsetEnd < OffsetStart Then
            NumBytes = OffsetEnd - (OffsetStart - BitStream.Length)
        Else
            NumBytes = OffsetEnd - OffsetStart
        End If

        NumBytes = NumBytes \ 16 - SyncBytesSize

        If NumBytes > 0 Then
            Return MFMGetBytes(BitStream, OffsetStart, NumBytes)
        End If

        Return New Byte(-1) {}
    End Function

    Private Function GetSectorList(BitStream As BitArray) As List(Of UInteger)
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

    Public Function MFMDecode(Data() As Byte, TrackNumber As UShort, Side As Byte) As MFMTrack
        Dim IAMPattern = BytesToBits(IAM_Sync_Bytes)
        Dim Track = New MFMTrack
        Dim Start As UInteger = 0

        Dim Bitstream = BytesToBits(Data)

        Bitstream = BitstreamAlign(Bitstream)

        'Dim buffer = MFMGetBytes(Bitstream, 0, Bitstream.Length / 16)
        'File.WriteAllBytes("H:\test\track" & TrackNumber.ToString.PadLeft(2, "0") & "." & Side & ".bin", buffer)

        'Index Field Sync
        Dim Pos = FindPattern(Bitstream, IAMPattern, Start)
        If Pos > -1 Then
            Track.Gap4A = GetGapBytes(Bitstream, Start, Pos)
            Start = Pos + IAMPattern.Length
            Track.IAMMark = MFMGetByte(Bitstream, Start)
            Start += 16
        End If

        Dim SectorList = GetSectorList(Bitstream)

        If SectorList.Count > 0 Then
            Track.Gap1 = GetGapBytes(Bitstream, Start, SectorList.Item(0))
            ProcessSectorList(Track, Bitstream, SectorList)
        End If

        'TrackDebug(Track)

        Return Track
    End Function

    Private Function MFMGetByte(Bitstream As BitArray, Start As Integer) As Byte
        Return MFMGetBytes(Bitstream, Start, 1)(0)
    End Function

    Private Function MFMGetBytes(Bitstream As BitArray, Start As UInteger, NumBytes As UInteger) As Byte()
        Dim decodedBytes As New List(Of Byte)
        Dim byteBuffer As Byte
        Dim bitCount As Integer = 0

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

                byteBuffer = (byteBuffer << 1) Or If(dataBit, 1, 0)
                bitCount += 1

                If bitCount = 8 Then
                    decodedBytes.Add(byteBuffer)
                    byteBuffer = 0
                    bitCount = 0
                End If
            Next
        End If

        Return decodedBytes.ToArray
    End Function

    Private Sub ProcessSectorList(Track As MFMTrack, BitStream As BitArray, SectorList As List(Of UInteger))
        Dim DAMPattern = BytesToBits(DAM_Sync_Bytes)
        Dim Sector As MFMSector = Nothing
        Dim ChecksumData() As Byte
        Dim Checksum() As Byte
        Dim Pos As Integer
        Dim DataOffset As UInteger = 0
        Dim SectorIndex As UShort = 0
        Dim NextSectorOffset As UInteger
        Dim DataEnd As UInteger

        For Each SectorOffset In SectorList
            If Sector IsNot Nothing Then
                If SectorOffset > DataOffset Then
                    Sector.Gap3 = GetGapBytes(BitStream, DataOffset, SectorOffset)
                End If
            End If
            Sector = New MFMSector
            ChecksumData = MFMGetBytes(BitStream, SectorOffset, 8)
            Sector.Track = MFMGetByte(BitStream, SectorOffset + 64)
            Sector.Side = MFMGetByte(BitStream, SectorOffset + 80)
            Sector.SectorId = MFMGetByte(BitStream, SectorOffset + 96)
            Sector.Size = SectorGetSize(MFMGetByte(BitStream, SectorOffset + 112))
            Checksum = MFMGetBytes(BitStream, SectorOffset + 128, 2)
            Sector.Checksum = BitConverter.ToUInt16(Checksum, 0)
            Sector.CalculatedChecksum = ToBigEndianUInt16(Crc16(ChecksumData))
            Sector.Overlaps = False

            DataOffset = SectorOffset + 160
            DataEnd = DataOffset + Sector.Size * 16

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
                Sector.Gap2 = GetGapBytes(BitStream, DataOffset, Pos)
                ChecksumData = MFMGetBytes(BitStream, Pos, Sector.Size + DAMPattern.Length \ 16)
                DataOffset = Pos + DAMPattern.Length
                Sector.Data = MFMGetBytes(BitStream, DataOffset, Sector.Size)
                DataOffset += Sector.Size * 16
                DataOffset = DataOffset Mod BitStream.Length
                Checksum = MFMGetBytes(BitStream, DataOffset, 2)
                Sector.DataChecksum = BitConverter.ToUInt16(Checksum, 0)
                Sector.CalculatedDataChecksum = ToBigEndianUInt16(Crc16(ChecksumData))
                DataOffset += 32
            End If

            'If SectorIndex = SectorList.Count - 1 Then
            '    Sector.Gap3 = GetGapBytes(BitStream, DataOffset, BitStream.Length - 1)
            'End If

            Track.Sectors.Add(Sector)

            SectorIndex += 1
        Next
    End Sub

    Private Function SectorGetSize(Size As Byte) As UInteger
        If Size > 7 Then
            Size = 7
        End If
        Return (2 ^ Size) * 128
    End Function

    Public Sub TrackDebug(Track As MFMTrack)
        Console.Write("Gap 4A: " & Track.Gap4A.Length)
        If Track.Gap4A.Length > 0 Then
            Console.Write(", IAM Mark: " & Track.IAMMark.ToString("X2") & If(Track.IAMMark <> IAM, " (Unexpected)", ""))
        End If
        Console.WriteLine(", Gap 1: " & Track.Gap1.Length)

        For Each Sector In Track.Sectors
            Dim ChecksumValid = Sector.Checksum = Sector.CalculatedChecksum
            Dim DataChecksumValid = Sector.DataChecksum = Sector.CalculatedDataChecksum

            Console.Write("Trk: " & Sector.Track & "." & Sector.Side)
            Console.Write(", Id: " & Sector.SectorId)
            Console.Write(", Size: " & Sector.Size)
            Console.Write(", Checksum: " & Sector.Checksum.ToString("X4") & If(ChecksumValid, "", "#"))
            Console.Write(", Data Checksum: " & Sector.DataChecksum.ToString("X4") & If(DataChecksumValid, "", "#"))
            Console.Write(", Gap2: " & Sector.Gap2.Length)
            Console.Write(", Gap3: " & Sector.Gap3.Length)
            Console.WriteLine(", Overlaps: " & Sector.Overlaps)
        Next
    End Sub
End Module

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
End Class

Public Class MFMTrack
    Public Sub New()
        _Gap4A = New Byte(-1) {}
        _Gap1 = New Byte(-1) {}
        _IAMMark = 0
        _Sectors = New List(Of MFMSector)
    End Sub

    Public Property Gap1 As Byte()
    Public Property Gap4A As Byte()
    Public Property IAMMark As Byte
    Public Property Sectors As List(Of MFMSector)
End Class
