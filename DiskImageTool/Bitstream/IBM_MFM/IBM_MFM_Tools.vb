Namespace Bitstream
    Namespace IBM_MFM
        Public Enum MFMSectionType
            GapBytes
            SyncNulls
            SyncBytes
            AddressMark
            IDArea
            CRCBytes
            DataArea
        End Enum
        Public Enum MFMAddressMark As Byte
            Index = &HFC
            ID = &HFE
            Data = &HFB
            DeletedData = &HF8
        End Enum

        Public Enum MFMTrackFormat As Byte
            TrackFormatUnknown = 0
            TrackFormatDD = 1
            TrackFormatHD = 2
            TrackFormatHD1200 = 3
            TrackFormatED = 4
        End Enum

        Module IBM_MFM_Tools
            Public Const MFM_BYTE_SIZE As Byte = 16
            Public Const MFM_ADDRESS_MARK_SIZE As Byte = 4
            Public Const MFM_SYNC_SIZE As Byte = 12
            Public Const MFM_SYNC_SIZE_BITS As Integer = 12 * MFM_BYTE_SIZE
            Public Const MFM_GAP_BYTE As Byte = &H4E
            Public Const MFM_GAP1_SIZE As Byte = 50
            Public Const MFM_GAP2_SIZE As Byte = 22
            Public Const MFM_GAP3_SIZE As Byte = 80
            Public Const MFM_GAP4A_SIZE As Byte = 80
            Public Const MFM_IDAM_SIZE As Byte = 6
            Public Const MFM_IDAREA_SIZE As Byte = 4
            Public Const MFM_CRC_SIZE As Byte = 2
            Public ReadOnly IAM_Sync_Bytes() As Byte = {&H52, &H24, &H52, &H24, &H52, &H24}
            Public ReadOnly IDAM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89, &H55, &H54}
            Public ReadOnly DAM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89, &H55, &H45}
            Public ReadOnly MFM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89}

            Public Function BytesToBits(bytes() As Byte) As BitArray
                Dim buffer(bytes.Length - 1) As Byte

                For i As Integer = 0 To bytes.Length - 1
                    buffer(i) = ReverseBits(bytes(i))
                Next

                Return New BitArray(buffer)
            End Function

            Public Function BytesToBits(bytes() As Byte, Offset As UInteger, Length As UInteger) As BitArray
                Dim buffer(Length - 1) As Byte

                For i As Integer = 0 To Length - 1
                    buffer(i) = ReverseBits(bytes(Offset + i))
                Next

                Return New BitArray(buffer)
            End Function

            Public Function BitsToBytes(Bitstream As BitArray, Padding As UInteger) As Byte()
                Dim Length = (Bitstream.Length + Padding) \ 8

                Dim buffer(Length - 1) As Byte

                If Padding > 0 Then
                    Dim Offset = Bitstream.Length
                    Bitstream.Length += Padding
                    For i = Offset To Offset + Padding - 1
                        Bitstream(i) = Bitstream(i - Offset)
                    Next
                End If

                Bitstream.CopyTo(buffer, 0)

                For i As Integer = 0 To buffer.Length - 1
                    buffer(i) = ReverseBits(buffer(i))
                Next

                Return buffer
            End Function

            Public Function BitstreamAlign(Bitstream As BitArray) As BitArray
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

            Public Function ReverseBits(b As Byte) As Byte
                b = ((b >> 1) And &H55) Or ((b << 1) And &HAA) ' Swap odd and even bits
                b = ((b >> 2) And &H33) Or ((b << 2) And &HCC) ' Swap consecutive pairs
                b = ((b >> 4) And &HF) Or ((b << 4) And &HF0)  ' Swap nibbles

                Return b
            End Function

            Public Function DecodeTrack(Bitstream As BitArray) As Byte()
                Return MFMGetBytes(Bitstream, 0, Bitstream.Length \ MFM_BYTE_SIZE)
            End Function

            Public Function DecodeTrack(Data() As Byte) As Byte()
                Return DecodeTrack(BytesToBits(Data))
            End Function

            Public Function BitstreamGetSectionList(Bitstream As BitArray) As List(Of BitstreamSection)
                Dim IAMPattern = BytesToBits(IAM_Sync_Bytes)
                Dim MFMPattern = BytesToBits(MFM_Sync_Bytes)
                Dim BitstreamIndex As UInteger = 0
                Dim ByteIndex As UInteger = 0
                Dim Buffer() As Byte
                Dim SyncNullCount As UInteger = 0
                Dim GapCount As UInteger = 0
                Dim Index As Integer
                Dim DataIndex As Integer
                Dim Offset As UInteger

                Dim Sections = New List(Of BitstreamSection)

                Index = FindPattern(Bitstream, IAMPattern, BitstreamIndex)
                If Index > -1 Then
                    Offset = Index Mod MFM_BYTE_SIZE
                    Buffer = GetGapBytes(Bitstream, BitstreamIndex + Offset, Index)

                    If Buffer.Length > 0 Then
                        SyncNullCount = GetByteCount(Buffer, 0, 0)
                        GapCount = Buffer.Length - SyncNullCount

                        If GapCount > 0 Then
                            Sections.Add(New BitstreamSection(MFMSectionType.GapBytes, ByteIndex, GapCount))
                        End If
                        If SyncNullCount > 0 Then
                            Sections.Add(New BitstreamSection(MFMSectionType.SyncNulls, ByteIndex + GapCount, SyncNullCount))
                        End If
                    End If
                    ByteIndex += Buffer.Length
                    BitstreamIndex = Index

                    Sections.Add(New BitstreamSection(MFMSectionType.SyncBytes, ByteIndex, IAMPattern.Length \ 16))
                    ByteIndex += IAMPattern.Length \ 16
                    BitstreamIndex += MFMPattern.Length

                    Sections.Add(New BitstreamSection(MFMSectionType.AddressMark, ByteIndex, 1))
                    ByteIndex += 1
                    BitstreamIndex += MFM_BYTE_SIZE
                End If

                Dim SectorList = GetSectorList(Bitstream)

                Dim i = 0
                For Each Index In SectorList
                    If BitstreamIndex = 0 Then
                        Offset = Index Mod MFM_BYTE_SIZE
                    Else
                        Offset = 0
                    End If
                    Buffer = GetGapBytes(Bitstream, BitstreamIndex + Offset, Index)
                    If Buffer.Length > 0 Then
                        SyncNullCount = GetByteCount(Buffer, 0, 0)
                        GapCount = Buffer.Length - SyncNullCount

                        If GapCount > 0 Then
                            Sections.Add(New BitstreamSection(MFMSectionType.GapBytes, ByteIndex, GapCount))
                        End If
                        If SyncNullCount > 0 Then
                            Sections.Add(New BitstreamSection(MFMSectionType.SyncNulls, ByteIndex + GapCount, SyncNullCount))
                        End If
                    End If
                    ByteIndex = Index \ 16
                    BitstreamIndex = Index

                    Sections.Add(New BitstreamSection(MFMSectionType.SyncBytes, ByteIndex, MFMPattern.Length \ 16))
                    ByteIndex += MFMPattern.Length \ 16
                    BitstreamIndex += MFMPattern.Length

                    Sections.Add(New BitstreamSection(MFMSectionType.AddressMark, ByteIndex, 1))
                    ByteIndex += 1
                    BitstreamIndex += MFM_BYTE_SIZE

                    Buffer = GetGapBytes(Bitstream, BitstreamIndex, BitstreamIndex + 64)
                    Dim SectorSize = GetSectorSizeBytes(Buffer(3))
                    Sections.Add(New BitstreamSection(MFMSectionType.IDArea, ByteIndex, MFM_IDAREA_SIZE))
                    ByteIndex += MFM_IDAREA_SIZE
                    BitstreamIndex += MFM_IDAREA_SIZE * 16

                    Sections.Add(New BitstreamSection(MFMSectionType.CRCBytes, ByteIndex, MFM_CRC_SIZE))
                    ByteIndex += MFM_CRC_SIZE
                    BitstreamIndex += MFM_CRC_SIZE * 16

                    Dim SectorEnd As UInteger
                    If i < SectorList.Count - 1 Then
                        SectorEnd = SectorList.Item(i + 1)
                    Else
                        SectorEnd = Bitstream.Length
                    End If

                    DataIndex = FindPattern(Bitstream, MFMPattern, BitstreamIndex)
                    If DataIndex > -1 And DataIndex < SectorEnd Then
                        Buffer = GetGapBytes(Bitstream, BitstreamIndex, DataIndex)
                        If Buffer.Length > 0 Then
                            SyncNullCount = GetByteCount(Buffer, 0, 0)
                            GapCount = Buffer.Length - SyncNullCount

                            If GapCount > 0 Then
                                Sections.Add(New BitstreamSection(MFMSectionType.GapBytes, ByteIndex, GapCount))
                            End If
                            If SyncNullCount > 0 Then
                                Sections.Add(New BitstreamSection(MFMSectionType.SyncNulls, ByteIndex + GapCount, SyncNullCount))
                            End If
                        End If
                        ByteIndex += Buffer.Length
                        BitstreamIndex = DataIndex

                        Sections.Add(New BitstreamSection(MFMSectionType.SyncBytes, ByteIndex, MFMPattern.Length \ 16))
                        ByteIndex += MFMPattern.Length \ 16
                        BitstreamIndex += MFMPattern.Length

                        Sections.Add(New BitstreamSection(MFMSectionType.AddressMark, ByteIndex, 1))
                        ByteIndex += 1
                        BitstreamIndex += MFM_BYTE_SIZE

                        Dim SectorLength = (SectorEnd - BitstreamIndex) \ 16
                        Dim Overlaps As Boolean = False
                        If SectorSize > SectorLength Then
                            SectorSize = SectorLength
                            Overlaps = True
                        End If

                        SyncNullCount = 0
                        GapCount = 0
                        If Overlaps Then
                            Buffer = MFMGetBytes(Bitstream, BitstreamIndex, SectorSize)
                            SyncNullCount = GetByteCount(Buffer, 0, 0)
                            SectorSize -= SyncNullCount
                            GapCount = GetByteCount(Buffer, MFM_GAP_BYTE, SyncNullCount)
                            SectorSize -= GapCount
                        End If

                        Sections.Add(New BitstreamSection(MFMSectionType.DataArea, ByteIndex, SectorSize))
                        ByteIndex += SectorSize
                        BitstreamIndex += SectorSize * 16

                        If Not Overlaps And SectorSize < SectorLength - 2 Then
                            Sections.Add(New BitstreamSection(MFMSectionType.CRCBytes, ByteIndex, MFM_CRC_SIZE))
                            ByteIndex += MFM_CRC_SIZE
                            BitstreamIndex += MFM_CRC_SIZE * 16
                        End If

                        If GapCount > 0 Then
                            Sections.Add(New BitstreamSection(MFMSectionType.GapBytes, ByteIndex, GapCount))
                            ByteIndex += GapCount
                            BitstreamIndex += GapCount * 16
                        End If

                        If SyncNullCount > 0 Then
                            Sections.Add(New BitstreamSection(MFMSectionType.SyncNulls, ByteIndex, SyncNullCount))
                            ByteIndex += SyncNullCount
                            BitstreamIndex += SyncNullCount * 16
                        End If
                    End If
                    i += 1
                Next

                If BitstreamIndex < Bitstream.Length Then
                    GapCount = (Bitstream.Length - BitstreamIndex) \ 16
                    Sections.Add(New BitstreamSection(MFMSectionType.GapBytes, ByteIndex, GapCount))
                End If

                Return Sections
            End Function

            Private Function GetByteCount(Buffer() As Byte, Value As Byte, OffsetEnd As UInteger) As UInteger
                Dim Result As UInteger = 0

                For i = Buffer.Length - 1 - OffsetEnd To 0 Step -1
                    If Buffer(i) <> Value Then
                        Exit For
                    End If
                    Result += 1
                Next

                Return Result
            End Function

            Public Function FindPattern(BitStream As BitArray, Pattern As BitArray, Optional Start As Integer = 0) As Integer
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

            Public Function GetGapBytes(BitStream As BitArray, OffsetStart As Integer, OffsetEnd As Integer) As Byte()
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

            Public Function GetSectorList(BitStream As BitArray) As List(Of UInteger)
                Dim IDAMPattern = BytesToBits(IDAM_Sync_Bytes)

                Dim Start As UInteger = 0
                Dim IDFieldSyncIndex As Integer
                Dim SectorList As New List(Of UInteger)
                Do
                    IDFieldSyncIndex = FindPattern(BitStream, IDAMPattern, Start)
                    If IDFieldSyncIndex > -1 Then
                        SectorList.Add(IDFieldSyncIndex)
                        Start = IDFieldSyncIndex + IDAMPattern.Length
                    End If
                Loop Until IDFieldSyncIndex = -1

                Return SectorList
            End Function

            Public Function GetSectorSizeBytes(Size As Byte) As UInteger
                If Size > 7 Then
                    Size = 7
                End If
                Return (2 ^ Size) * 128
            End Function

            Public Function GetTrackFormat(Size As UInteger) As MFMTrackFormat
                Size = Math.Round(Size / 10000) * 10000
                If Size = 100000 Then
                    Return MFMTrackFormat.TrackFormatDD
                ElseIf Size = 170000 Then
                    Return MFMTrackFormat.TrackFormatHD1200
                ElseIf Size = 200000 Then
                    Return MFMTrackFormat.TrackFormatHD
                ElseIf Size = 400000 Then
                    Return MFMTrackFormat.TrackFormatED
                Else
                    Return MFMTrackFormat.TrackFormatUnknown
                End If
            End Function

            Public Function IsStandardSector(Sector As IBM_MFM_Sector, Track As Byte, Side As Byte) As Boolean
                If Not Sector.InitialChecksumValid Then
                    Return False
                End If

                If Sector.Overlaps Then
                    Return False
                End If

                If Sector.GetSizeBytes <> 512 Then
                    Return False
                End If

                If Sector.Track <> Track Or Sector.Side <> Side Then
                    Return False
                End If

                If Sector.DAM <> MFMAddressMark.Data Then
                    Return False
                End If

                Return True
            End Function

            Public Function MFMCRC16(data As Byte()) As UShort
                Dim crc As UShort = &HFFFF

                For Each b As Byte In data
                    crc = crc Xor (CUShort(b) << 8)
                    For i As Integer = 0 To 7
                        If (crc And &H8000) <> 0 Then
                            crc = CUShort((crc << 1) Xor &H1021)
                        Else
                            crc = CUShort(crc << 1)
                        End If
                    Next
                Next

                Return MyBitConverter.SwapEndian(crc)

            End Function

            Public Function MFMEncodeBytes(Data() As Byte, SeedBit As Boolean) As BitArray
                Dim Bitstream As New BitArray(Data.Length * 16)
                Dim bitCount As Integer = 0
                Dim dateBit As Boolean
                Dim clockBit As Boolean

                Dim prevDataBit = SeedBit
                For i = 0 To Data.Length - 1
                    For j = 7 To 0 Step -1
                        dateBit = CBool((Data(i) And (1 << j)))
                        If Not dateBit And Not prevDataBit Then
                            clockBit = True
                        Else
                            clockBit = False
                        End If
                        Bitstream.Set(bitCount, clockBit)
                        Bitstream.Set(bitCount + 1, dateBit)

                        prevDataBit = dateBit
                        bitCount += 2
                    Next
                Next

                Return Bitstream
            End Function

            Public Function MFMGetByte(Bitstream As BitArray, Start As Integer) As Byte
                Return MFMGetBytes(Bitstream, Start, 1)(0)
            End Function

            Public Function MFMGetBytes(Bitstream As BitArray, Start As UInteger, NumBytes As UInteger) As Byte()
                Dim decodedBytes(NumBytes - 1) As Byte
                Dim byteBuffer As Byte
                Dim bitCount As Integer = 0
                Dim byteCount As Integer = 0
                Dim clockBit As Boolean
                Dim dataBit As Boolean

                If NumBytes > 0 Then
                    Dim Length = Bitstream.Length - 1

                    Do
                        clockBit = Bitstream(Start)
                        Start += 1
                        If Start > Length Then
                            Start = 0
                        End If
                        dataBit = Bitstream(Start)
                        Start += 1
                        If Start > Length Then
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
                    Loop Until byteCount = NumBytes
                End If

                Return decodedBytes
            End Function
        End Module

        Public Class BitstreamSection
            Public Sub New(SectionType As MFMSectionType, StartIndex As UInteger, Length As UInteger)
                _SectionType = SectionType
                _StartIndex = StartIndex
                _Length = Length
            End Sub

            Public Property SectionType As MFMSectionType
            Public Property StartIndex As UInteger
            Public Property Length As UInteger
        End Class
    End Namespace
End Namespace
