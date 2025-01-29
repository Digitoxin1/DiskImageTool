﻿Namespace Bitstream
    Namespace IBM_MFM
        Public Enum MFMRegionType
            Gap1
            Gap2
            Gap3
            Gap4A
            Gap4B
            IAMNulls
            IAMSync
            IAM
            IDAMNulls
            IDAMSync
            IDAM
            IDArea
            IDAMChecksumValid
            IDAMChecksumInvalid
            DAMNulls
            DAMSync
            DAM
            DataArea
            DataChecksumValid
            DataChecksumInvalid
            Overflow
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

        Public Structure RegionData
            Dim Regions As List(Of BitstreamRegion)
            Dim Sectors As List(Of BitstreamRegionSector)
            Dim Track As UShort
            Dim Side As Byte
            Dim NumBytes As UInteger
            Dim NumBits As UInteger
            Dim Aligned As Boolean
            Dim Gap4A As UShort
            Dim Gap1 As UShort
        End Structure

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
            Public ReadOnly MFM_IAM_Sync_Bytes() As Byte = {&H52, &H24, &H52, &H24, &H52, &H24}
            Public ReadOnly MFM_IDAM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89, &H55, &H54}
            Public ReadOnly MFM_DAM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89, &H55, &H45}
            Public ReadOnly MFM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89}
            Public ReadOnly FM_IAM_Sync_Bytes() As Byte = {&HF5, &H7A}
            Public ReadOnly FM_IDAM_Sync_Bytes() As Byte = {&HF5, &H7E}
            Public ReadOnly FM_DAM_Sync_Bytes() As Byte = {&HF5, &H6F}

            Public Function AdjustBitIndex(Index As Integer, Length As UInteger) As Integer
                If Index < 0 Then
                    Index = Index Mod Length
                    Index = Length + Index
                ElseIf Index > Length - 1 Then
                    Index = Index Mod Length
                End If

                Return Index
            End Function

            Public Function BytesToBits(bytes() As Byte, Optional Reverse As Boolean = True) As BitArray
                Dim buffer(bytes.Length - 1) As Byte

                If Reverse Then
                    For i As Integer = 0 To bytes.Length - 1
                        buffer(i) = ReverseBits(bytes(i))
                    Next
                Else
                    buffer = bytes
                End If

                Return New BitArray(buffer)
            End Function

            Public Function BytesToBits(bytes() As Byte, Offset As UInteger, BitLength As UInteger, Optional Reverse As Boolean = True) As BitArray
                Dim bufferSize = Math.Ceiling(BitLength / 8)
                Dim buffer(bufferSize - 1) As Byte

                For i As Integer = 0 To bufferSize - 1
                    If Reverse Then
                        buffer(i) = ReverseBits(bytes(Offset + i))
                    Else
                        buffer(i) = bytes(Offset + i)
                    End If
                Next

                Dim bitArray = New BitArray(buffer) With {
                    .Length = BitLength
                }

                Return bitArray
            End Function

            Public Function BitsToBytes(Bitstream As BitArray, Padding As UInteger, Optional Reverse As Boolean = True) As Byte()
                Dim Length = Math.Ceiling((Bitstream.Length + Padding) / 8)

                Dim buffer(Length - 1) As Byte

                If Padding > 0 Then
                    Dim Offset = Bitstream.Length
                    Bitstream.Length += Padding
                    For i = Offset To Offset + Padding - 1
                        Bitstream(i) = Bitstream(i - Offset)
                    Next
                End If

                Bitstream.CopyTo(buffer, 0)

                If Reverse Then
                    For i As Integer = 0 To buffer.Length - 1
                        buffer(i) = ReverseBits(buffer(i))
                    Next
                End If

                Return buffer
            End Function

            Public Function BitstreamAlign(Bitstream As BitArray, Offset As UInteger) As BitArray
                Dim NewBitstream = Bitstream

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

                Return NewBitstream
            End Function

            Public Function FMGetOffset(Bitstream As BitArray) As UInteger
                Dim FMPattern = BytesToBits(FM_IDAM_Sync_Bytes)
                Dim Pos = FindPattern(Bitstream, FMPattern, 0)
                If Pos > -1 Then
                    Return Pos Mod MFM_BYTE_SIZE
                End If

                Return 0
            End Function

            Public Function MFMGetOffset(Bitstream As BitArray) As UInteger
                Dim MFMPattern = BytesToBits(MFM_Sync_Bytes)
                Dim Pos = FindPattern(Bitstream, MFMPattern, 0)
                If Pos > -1 Then
                    Return Pos Mod MFM_BYTE_SIZE
                End If

                Return 0
            End Function

            Public Function ReverseBits(b As Byte) As Byte
                b = ((b >> 1) And &H55) Or ((b << 1) And &HAA) ' Swap odd and even bits
                b = ((b >> 2) And &H33) Or ((b << 2) And &HCC) ' Swap consecutive pairs
                b = ((b >> 4) And &HF) Or ((b << 4) And &HF0)  ' Swap nibbles

                Return b
            End Function

            Public Function MFMGetRegionList(Bitstream As BitArray, TrackType As BitstreamTrackType) As RegionData
                Dim IAMPattern As BitArray
                Dim IDAMPattern As BitArray
                Dim DAMPattern As BitArray
                Dim MaxSyncNulls As UInteger
                Dim IDAMByteLength As Byte

                If TrackType = BitstreamTrackType.MFM Then
                    IAMPattern = BytesToBits(MFM_IAM_Sync_Bytes)
                    IDAMPattern = BytesToBits(MFM_Sync_Bytes)
                    DAMPattern = BytesToBits(MFM_Sync_Bytes)
                    MaxSyncNulls = 12
                    IDAMByteLength = 8
                Else
                    IAMPattern = BytesToBits(FM_IAM_Sync_Bytes)
                    IDAMPattern = BytesToBits(FM_IDAM_Sync_Bytes)
                    DAMPattern = BytesToBits(FM_DAM_Sync_Bytes)
                    MaxSyncNulls = 6
                    IDAMByteLength = 5
                End If

                Dim BitstreamIndex As UInteger = 0
                Dim ByteIndex As UInteger = 0
                Dim PrevByteIndex As UInteger
                Dim Buffer() As Byte
                Dim CalculatedChecksum As UShort
                Dim Checksum As UShort
                Dim RegionType As MFMRegionType
                Dim SyncNullCount As UInteger
                Dim GapCount As UInteger
                Dim Index As Integer
                Dim DataIndex As Integer
                Dim DataLength As UInteger
                Dim BitOffset As UInteger = 0
                Dim PrevBitOffset As UInteger
                Dim RegionSector As BitstreamRegionSector = Nothing
                Dim PrevRegionSector As BitstreamRegionSector

                Dim RegionData As RegionData
                RegionData.Regions = New List(Of BitstreamRegion)
                RegionData.Sectors = New List(Of BitstreamRegionSector)
                RegionData.NumBytes = Math.Ceiling(Bitstream.Length / MFM_BYTE_SIZE)
                RegionData.NumBits = Bitstream.Length
                RegionData.Aligned = True
                RegionData.Gap4A = 0
                RegionData.Gap1 = 0

                Dim Pattern As BitArray
                If TrackType = BitstreamTrackType.MFM Then
                    Pattern = BytesToBits(MFM_IDAM_Sync_Bytes)
                Else
                    Pattern = BytesToBits(FM_IDAM_Sync_Bytes)
                End If

                Dim SectorList = MFMGetSectorList(Bitstream, Pattern)

                Dim EndIndex = Bitstream.Length
                If SectorList.Count > 0 Then
                    EndIndex = SectorList.Item(0)
                End If

                Index = FindPattern(Bitstream, IAMPattern, BitstreamIndex)
                If Index > -1 And Index < EndIndex Then
                    PrevByteIndex = ByteIndex
                    ByteIndex = Index \ MFM_BYTE_SIZE
                    BitstreamIndex = Index Mod MFM_BYTE_SIZE
                    BitOffset = BitstreamIndex
                    If BitOffset > 0 Then
                        RegionData.Aligned = False
                    End If

                    If Index >= BitstreamIndex + MFM_BYTE_SIZE Then
                        SyncNullCount = GetSyncNullCount(Bitstream, BitstreamIndex, Index, MaxSyncNulls)

                        Buffer = GetGapBytes(Bitstream, BitstreamIndex, Index - SyncNullCount * MFM_BYTE_SIZE)
                        RegionData.Gap4A = Buffer.Length

                        If Buffer.Length > 0 Then
                            RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.Gap4A, PrevByteIndex, Buffer.Length, Nothing, BitOffset))
                        End If

                        If SyncNullCount > 0 Then
                            RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.IAMNulls, ByteIndex - SyncNullCount, SyncNullCount, Nothing, BitOffset))
                        End If
                    End If
                    BitstreamIndex = Index

                    If TrackType = BitstreamTrackType.MFM Then
                        RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.IAMSync, ByteIndex, IAMPattern.Length \ MFM_BYTE_SIZE, Nothing, BitOffset))
                        ByteIndex += IAMPattern.Length \ MFM_BYTE_SIZE
                        BitstreamIndex += IDAMPattern.Length
                    End If

                    RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.IAM, ByteIndex, 1, Nothing, BitOffset))
                    ByteIndex += 1
                    BitstreamIndex += MFM_BYTE_SIZE
                End If

                Dim SectorIndex = 0
                For Each Index In SectorList
                    PrevByteIndex = ByteIndex
                    PrevBitOffset = BitOffset
                    ByteIndex = Index \ MFM_BYTE_SIZE
                    BitOffset = Index Mod MFM_BYTE_SIZE
                    If BitOffset > 0 Then
                        RegionData.Aligned = False
                    End If

                    PrevRegionSector = RegionSector
                    If PrevRegionSector IsNot Nothing Then
                        PrevRegionSector.Length = Index \ MFM_BYTE_SIZE - PrevRegionSector.StartIndex
                    End If

                    RegionSector = New BitstreamRegionSector With {
                        .StartIndex = Index \ MFM_BYTE_SIZE,
                        .SectorIndex = SectorIndex,
                        .Gap3 = 0
                    }
                    RegionData.Sectors.Add(RegionSector)

                    If BitstreamIndex = 0 Then
                        BitstreamIndex = BitOffset
                    End If

                    If Index >= BitstreamIndex + MFM_BYTE_SIZE Then
                        SyncNullCount = GetSyncNullCount(Bitstream, BitstreamIndex, Index, MaxSyncNulls)

                        Buffer = GetGapBytes(Bitstream, BitstreamIndex, Index - SyncNullCount * MFM_BYTE_SIZE)

                        If Buffer.Length > 0 Then
                            If SectorIndex = 0 Then
                                RegionType = MFMRegionType.Gap1
                                RegionData.Gap1 = Buffer.Length
                            Else
                                RegionType = MFMRegionType.Gap3
                                RegionData.Sectors.Item(SectorIndex - 1).Gap3 = Buffer.Length
                            End If
                            RegionData.Regions.Add(New BitstreamRegion(RegionType, PrevByteIndex, Buffer.Length, PrevRegionSector, BitOffset))
                        End If

                        If SyncNullCount > 0 Then
                            RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.IDAMNulls, ByteIndex - SyncNullCount, SyncNullCount, RegionSector, BitOffset))
                        End If

                        RegionSector.StartIndex -= SyncNullCount

                        If PrevRegionSector IsNot Nothing Then
                            PrevRegionSector.Length -= SyncNullCount
                        End If
                    End If
                    BitstreamIndex = Index

                    Buffer = MFMGetBytes(Bitstream, BitstreamIndex, IDAMByteLength)
                    CalculatedChecksum = MFMCRC16(Buffer)

                    If TrackType = BitstreamTrackType.MFM Then
                        RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.IDAMSync, ByteIndex, IDAMPattern.Length \ MFM_BYTE_SIZE, RegionSector, BitOffset))
                        ByteIndex += IDAMPattern.Length \ MFM_BYTE_SIZE
                        BitstreamIndex += IDAMPattern.Length
                    End If

                    RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.IDAM, ByteIndex, 1, RegionSector, BitOffset))
                    ByteIndex += 1
                    BitstreamIndex += MFM_BYTE_SIZE

                    Buffer = MFMGetBytes(Bitstream, BitstreamIndex, 4)
                    RegionSector.Track = Buffer(0)
                    RegionSector.Side = Buffer(1)
                    RegionSector.SectorId = Buffer(2)
                    DataLength = GetSectorSizeBytes(Buffer(3))
                    RegionSector.DataLength = DataLength
                    RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.IDArea, ByteIndex, MFM_IDAREA_SIZE, RegionSector, BitOffset))
                    ByteIndex += MFM_IDAREA_SIZE
                    BitstreamIndex += MFM_IDAREA_SIZE * MFM_BYTE_SIZE

                    Buffer = MFMGetBytes(Bitstream, BitstreamIndex, 2)
                    Checksum = BitConverter.ToUInt16(Buffer, 0)
                    RegionSector.IDAMChecksumValid = (Checksum = CalculatedChecksum)

                    If RegionSector.IDAMChecksumValid Then
                        RegionType = MFMRegionType.IDAMChecksumValid
                    Else
                        RegionType = MFMRegionType.IDAMChecksumInvalid
                    End If

                    RegionData.Regions.Add(New BitstreamRegion(RegionType, ByteIndex, MFM_CRC_SIZE, RegionSector, BitOffset))
                    ByteIndex += MFM_CRC_SIZE
                    BitstreamIndex += MFM_CRC_SIZE * MFM_BYTE_SIZE

                    Dim SectorEnd As UInteger
                    If SectorIndex < SectorList.Count - 1 Then
                        SectorEnd = SectorList.Item(SectorIndex + 1)
                        'Detect invalid data length - Formaster Copylock
                        If DataLength = 256 Then
                            If BitstreamIndex + 526 * MFM_BYTE_SIZE < SectorEnd Then
                                DataLength = 512
                            End If
                        End If
                    Else
                        SectorEnd = Bitstream.Length
                    End If

                    DataIndex = FindPattern(Bitstream, DAMPattern, BitstreamIndex)
                    If DataIndex > -1 And DataIndex < SectorEnd Then
                        PrevByteIndex = ByteIndex
                        PrevBitOffset = BitOffset
                        ByteIndex = DataIndex \ MFM_BYTE_SIZE
                        BitOffset = DataIndex Mod MFM_BYTE_SIZE
                        If BitOffset > 0 Then
                            RegionData.Aligned = False
                        End If

                        If DataIndex >= BitstreamIndex + MFM_BYTE_SIZE Then
                            SyncNullCount = GetSyncNullCount(Bitstream, BitstreamIndex, DataIndex, MaxSyncNulls)

                            Buffer = GetGapBytes(Bitstream, BitstreamIndex, DataIndex - SyncNullCount * MFM_BYTE_SIZE)

                            If Buffer.Length > 0 Then
                                RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.Gap2, PrevByteIndex, Buffer.Length, RegionSector, PrevBitOffset))
                            End If

                            If SyncNullCount > 0 Then
                                RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.DAMNulls, ByteIndex - SyncNullCount, SyncNullCount, RegionSector, BitOffset))
                            End If
                        End If

                        BitstreamIndex = DataIndex

                        Buffer = MFMGetBytes(Bitstream, BitstreamIndex, DataLength + 4)
                        CalculatedChecksum = MFMCRC16(Buffer)

                        If TrackType = BitstreamTrackType.MFM Then
                            RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.DAMSync, ByteIndex, DAMPattern.Length \ MFM_BYTE_SIZE, RegionSector, BitOffset))
                            ByteIndex += DAMPattern.Length \ MFM_BYTE_SIZE
                            BitstreamIndex += DAMPattern.Length
                        End If

                        RegionSector.DAM = MFMGetByte(Bitstream, BitstreamIndex)
                        RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.DAM, ByteIndex, 1, RegionSector, BitOffset))
                        ByteIndex += 1
                        BitstreamIndex += MFM_BYTE_SIZE

                        Dim SectorSize = DataLength
                        Dim SectorLength = (SectorEnd - BitstreamIndex) \ MFM_BYTE_SIZE

                        If SectorSize > SectorLength Then
                            SectorSize = SectorLength
                            RegionSector.Overlaps = True
                        Else
                            RegionSector.Overlaps = False
                        End If

                        SyncNullCount = 0
                        GapCount = 0
                        If RegionSector.Overlaps Then
                            Buffer = MFMGetBytes(Bitstream, BitstreamIndex, SectorSize)
                            SyncNullCount = GetByteCount(Buffer, 0, 0)
                            SectorSize -= SyncNullCount
                            GapCount = GetByteCount(Buffer, MFM_GAP_BYTE, SyncNullCount, 6)
                            SectorSize -= GapCount
                        End If

                        RegionSector.DataStartIndex = ByteIndex
                        RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.DataArea, ByteIndex, SectorSize, RegionSector, BitOffset))
                        If ByteIndex + DataLength > RegionData.NumBytes Then
                            RegionData.NumBytes = ByteIndex + DataLength
                        End If

                        Buffer = MFMGetBytes(Bitstream, BitstreamIndex + DataLength * MFM_BYTE_SIZE, 2)
                        Checksum = BitConverter.ToUInt16(Buffer, 0)
                        RegionSector.DataChecksumValid = (Checksum = CalculatedChecksum)

                        If RegionSector.DataChecksumValid Then
                            RegionType = MFMRegionType.DataChecksumValid
                        Else
                            RegionType = MFMRegionType.DataChecksumInvalid
                        End If

                        ByteIndex += SectorSize
                        BitstreamIndex += SectorSize * MFM_BYTE_SIZE

                        If Not RegionSector.Overlaps And SectorSize < SectorLength - 2 Then
                            RegionData.Regions.Add(New BitstreamRegion(RegionType, ByteIndex, MFM_CRC_SIZE, RegionSector, BitOffset))
                            ByteIndex += MFM_CRC_SIZE
                            BitstreamIndex += MFM_CRC_SIZE * MFM_BYTE_SIZE
                        End If

                        RegionSector.HasData = True
                    Else
                        RegionSector.HasData = False
                    End If
                    SectorIndex += 1
                Next

                If BitstreamIndex > 0 And BitstreamIndex < Bitstream.Length Then
                    GapCount = Math.Ceiling((Bitstream.Length - BitstreamIndex) / MFM_BYTE_SIZE)
                    RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.Gap4B, ByteIndex, GapCount, RegionSector, BitOffset))
                    If RegionSector IsNot Nothing Then
                        RegionSector.Length = Math.Ceiling(Bitstream.Length / MFM_BYTE_SIZE) - RegionSector.StartIndex
                    End If
                End If

                ByteIndex = Math.Ceiling(Bitstream.Length / MFM_BYTE_SIZE)
                If RegionData.NumBytes > ByteIndex Then
                    Dim Length = RegionData.NumBytes - ByteIndex
                    RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.Overflow, ByteIndex, Length, Nothing, BitOffset))
                End If

                Return RegionData
            End Function

            Private Function GetByteCount(Buffer() As Byte, Value As Byte, OffsetEnd As UInteger, Optional Tolerance As UInteger = 0) As UInteger
                Dim Result As UInteger = 0
                Dim Count As UInteger = 1
                Dim Matches As UInteger = 0

                For i = Buffer.Length - 1 - OffsetEnd To 0 Step -1
                    If Buffer(i) <> Value Then
                        If Matches > 0 Then
                            Exit For
                        End If
                        Matches = 0
                        If Count < Tolerance + 1 Then
                            Count += 1
                        Else
                            Exit For
                        End If
                    ElseIf Count > 1 And Matches < Tolerance Then
                        Matches += 1
                        Count += 1
                    Else
                        Matches = 0
                        Result += Count
                        Count = 1
                    End If
                Next

                Return Result
            End Function

            Private Function GetGapBytes(BitStream As BitArray, OffsetStart As UInteger, OffsetEnd As UInteger) As Byte()
                Dim Diff = Math.Truncate((OffsetEnd - OffsetStart) / MFM_BYTE_SIZE) * MFM_BYTE_SIZE

                OffsetEnd = OffsetStart + Diff

                Return MFMGetBytesByRange(BitStream, OffsetStart, OffsetEnd)
            End Function

            Private Function GetSyncNullCount(BitStream As BitArray, OffsetStart As UInteger, OffsetEnd As UInteger, MaxCount As UInteger) As UInteger
                Dim Diff = Math.Truncate((OffsetEnd - OffsetStart) / MFM_BYTE_SIZE) * MFM_BYTE_SIZE
                OffsetStart = OffsetEnd - Diff

                Dim Buffer = MFMGetBytesByRange(BitStream, OffsetStart, OffsetEnd)

                Dim Count = GetByteCount(Buffer, 0, 0)

                If Count > MaxCount Then
                    Count = MaxCount
                End If

                Return Count
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

            Public Function MFMGetSectorList(BitStream As BitArray, IDAMPattern As BitArray) As List(Of UInteger)
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

            Public Function GetTrackFormat(BitCount As UInteger) As MFMTrackFormat
                BitCount = Math.Round(BitCount / 5000) * 5000

                If BitCount <= 135000 Then
                    Return MFMTrackFormat.TrackFormatDD
                ElseIf BitCount >= 165000 And BitCount <= 175000 Then
                    Return MFMTrackFormat.TrackFormatHD1200
                ElseIf BitCount >= 195000 And BitCount <= 205000 Then
                    Return MFMTrackFormat.TrackFormatHD
                ElseIf BitCount >= 395000 And BitCount <= 405000 Then
                    Return MFMTrackFormat.TrackFormatED
                Else
                    Return MFMTrackFormat.TrackFormatUnknown
                End If
            End Function

            Public Function IsStandardSector(Sector As IBM_MFM_Sector, Track As Byte, Side As Byte, MaxSectors As Byte) As Boolean
                If Sector.SectorId < 1 Or Sector.SectorId > MaxSectors Then
                    Return False
                End If

                If Not Sector.DAMFound Then
                    Return False
                End If

                If Not Sector.InitialDataChecksumValid Or Not Sector.IDChecksumValid Then
                    Return False
                End If

                If Sector.Overlaps Then
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
                Dim dataBit As Boolean
                Dim clockBit As Boolean

                Dim prevDataBit = SeedBit
                For i = 0 To Data.Length - 1
                    For j = 7 To 0 Step -1
                        dataBit = CBool((Data(i) And (1 << j)))
                        clockBit = (Not dataBit And Not prevDataBit)
                        Bitstream.Set(bitCount, clockBit)
                        Bitstream.Set(bitCount + 1, dataBit)

                        prevDataBit = dataBit
                        bitCount += 2
                    Next
                Next

                Return Bitstream
            End Function

            Public Function MFMGetByte(Bitstream As BitArray, Start As Integer) As Byte
                Return MFMGetBytes(Bitstream, Start, 1)(0)
            End Function

            Public Function MFMGetBytes(Bitstream As BitArray) As Byte()
                Dim NumBytes As UInteger = Bitstream.Length \ 16

                Return MFMGetBytes(Bitstream, 0, NumBytes)
            End Function

            Public Function MFMGetBytes(Bitstream As BitArray, Start As Integer) As Byte()
                Dim NumBytes As UInteger = Bitstream.Length \ 16

                Return MFMGetBytes(Bitstream, Start, NumBytes)
            End Function

            Public Function MFMGetBytes(Bitstream As BitArray, Start As Integer, NumBytes As UInteger) As Byte()
                Dim decodedBytes(NumBytes - 1) As Byte
                Dim byteBuffer As Byte
                Dim bitCount As Integer = 0
                Dim byteCount As Integer = 0
                Dim clockBit As Boolean
                Dim dataBit As Boolean

                If NumBytes > 0 Then
                    Dim Length = Bitstream.Length

                    Start = AdjustBitIndex(Start, Length)

                    Do
                        clockBit = Bitstream(Start)
                        Start += 1
                        If Start > Length - 1 Then
                            Start = 0
                        End If
                        dataBit = Bitstream(Start)
                        Start += 1
                        If Start > Length - 1 Then
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

            Public Function MFMGetBytesByRange(BitStream As BitArray, OffsetStart As Integer, OffsetEnd As Integer) As Byte()
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

            Public Function ResizeBitstream(Bitstream As BitArray, Length As UInteger) As BitArray
                Dim NewBitstream = CType(Bitstream.Clone, BitArray)
                NewBitstream.Length = Length

                Return NewBitstream
            End Function
        End Module
    End Namespace
End Namespace
