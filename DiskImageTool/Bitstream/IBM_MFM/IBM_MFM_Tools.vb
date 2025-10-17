Namespace Bitstream
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

        Public Structure SyncCountResponse
            Dim Count As UInteger
            Dim WriteSplice As Boolean
        End Structure

        Module IBM_MFM_Tools
            Private Const MFM_NULL_WORD As UShort = &H5555  'Binary 0101010101010101
            Private Const MFM_GAP_WORD As UShort = &H2A49   'Binary 0010101001001001
            Public Const FM_FIELD_BYTES As UInteger = 1
            Public Const MFM_BYTE_BYTES As UInteger = 16
            Public Const MFM_SYNC_NULL_BYTES As UInteger = 12
            Public Const MFM_SYNC_NULL_BITS As UInteger = MFM_SYNC_NULL_BYTES * MFM_BYTE_BYTES
            Public Const MFM_SYNC_BYTES As UInteger = 3
            Public Const MFM_MARK_BYTES As UInteger = 1
            Public Const MFM_SYNC_MARK_BYTES As UInteger = MFM_SYNC_BYTES + MFM_MARK_BYTES
            Public Const MFM_PREAMBLE_BYTES As UInteger = MFM_SYNC_NULL_BYTES + MFM_SYNC_BYTES + MFM_MARK_BYTES

            Public Const MFM_GAP_BYTE As UInteger = &H4E
            Public Const MFM_IDAREA_BYTES As UInteger = 4
            Public Const MFM_CRC_BYTES As UInteger = 2

            Public ReadOnly MFM_IAM_SYNC_PATTERN_BYTES() As Byte = {&H52, &H24, &H52, &H24, &H52, &H24}
            Public ReadOnly MFM_IDAM_SYNC_PATTERN_BYTES() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89, &H55, &H54}
            Public ReadOnly MFM_DAM_SYNC_PATTERN_BYTES() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89, &H55, &H45}
            Public ReadOnly MFM_SYNC_PATTERN_BYTES() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89}

            Public ReadOnly FM_IAM_SYNC_PATTERN_BYTES() As Byte = {&HF5, &H7A}
            Public ReadOnly FM_IDAM_SYNC_PATTERN_BYTES() As Byte = {&HF5, &H7E}
            Public ReadOnly FM_DAM_SYNC_PATTERN_BYTES() As Byte = {&HF5, &H6F}

            Private Class MFMRegionInfo
                ReadOnly Property ByteIndex As UInteger
                ReadOnly Property BitstreamIndex As UInteger
                ReadOnly Property BitOffset As UInteger

                Public Sub New(Index As UInteger)
                    _ByteIndex = MFMBitsToBytes(Index)
                    _BitOffset = Index Mod MFM_BYTE_BYTES
                    _BitstreamIndex = Index
                End Sub

                Public Sub AddBytes(Value As UInteger)
                    _ByteIndex += Value
                    _BitstreamIndex += MFMBytesToBits(Value)
                End Sub

                Public Sub AddBits(Value As UInteger)
                    _BitstreamIndex += Value
                    _ByteIndex += MFMBitsToBytes(Value)
                End Sub

                Public Sub SetByteIndex(Value As UInteger)
                    _ByteIndex = Value
                    _BitstreamIndex = MFMBytesToBits(Value)
                End Sub
            End Class

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
                Dim FMPattern = BytesToBits(FM_IDAM_SYNC_PATTERN_BYTES)
                Dim Pos = FindPattern(Bitstream, FMPattern, 0)
                If Pos > -1 Then
                    Return Pos Mod MFM_BYTE_BYTES
                End If

                Return 0
            End Function

            Public Function MFMGetOffset(Bitstream As BitArray) As UInteger
                Dim MFMPattern = BytesToBits(MFM_SYNC_PATTERN_BYTES)
                Dim Pos = FindPattern(Bitstream, MFMPattern, 0)
                If Pos > -1 Then
                    Return Pos Mod MFM_BYTE_BYTES
                End If

                Return 0
            End Function

            Public Function ReverseBits(b As Byte) As Byte
                b = ((b >> 1) And &H55) Or ((b << 1) And &HAA) ' Swap odd and even bits
                b = ((b >> 2) And &H33) Or ((b << 2) And &HCC) ' Swap consecutive pairs
                b = ((b >> 4) And &HF) Or ((b << 4) And &HF0)  ' Swap nibbles

                Return b
            End Function

            Public Function MFMBitsToBytes(Value As UInteger) As UInteger
                Return Value \ MFM_BYTE_BYTES
            End Function

            Public Function MFMBytesToBits(Value As UInteger) As UInteger
                Return Value * MFM_BYTE_BYTES
            End Function

            Private Function MFMProcessDAMRegion(Bitstream As BitArray, TrackType As BitstreamTrackType, RegionData As BitstreamRegionData, RegionSector As BitstreamRegionSector, PrevRegion As MFMRegionInfo, EndIndex As UInteger) As MFMRegionInfo
                Dim DAMPattern As BitArray

                If TrackType = BitstreamTrackType.MFM Then
                    DAMPattern = BytesToBits(MFM_SYNC_PATTERN_BYTES)
                Else
                    DAMPattern = BytesToBits(FM_DAM_SYNC_PATTERN_BYTES)
                End If

                Dim Index = FindPattern(Bitstream, DAMPattern, PrevRegion.BitstreamIndex)

                If Index < 0 Or Index >= EndIndex Then
                    RegionSector.HasData = False
                    Return PrevRegion
                End If

                Dim Buffer As Byte()
                Dim SyncNullCount As UInteger

                Dim RegionInfo As New MFMRegionInfo(Index)

                If RegionInfo.BitOffset > 0 Then
                    RegionData.Aligned = False
                End If

                If Index >= PrevRegion.BitstreamIndex + MFM_BYTE_BYTES Then
                    SyncNullCount = GetSyncNullCount(Bitstream, PrevRegion.BitstreamIndex, Index)

                    Dim OffsetEnd As UInteger = Index - MFMBytesToBits(SyncNullCount)

                    'Write splice occurs after ID field but before data field
                    'This is the write splice we look for to determine if the sector data has been written by a PC as opposed to a track level duplicator
                    'If TrackType = BitstreamTrackType.MFM Then
                    RegionSector.WriteSplice = HasWriteSpliceStart(Bitstream, PrevRegion.BitstreamIndex, OffsetEnd)
                    'End If

                    Buffer = GetGapBytes(Bitstream, PrevRegion.BitstreamIndex, OffsetEnd)

                    If Buffer.Length > 0 Then
                        RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.Gap2, PrevRegion.ByteIndex, Buffer.Length, RegionSector, PrevRegion.BitOffset))
                    End If

                    If SyncNullCount > 0 Then
                        RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.DAMNulls, RegionInfo.ByteIndex - SyncNullCount, SyncNullCount, RegionSector, RegionInfo.BitOffset))
                    End If
                End If

                Buffer = MFMGetBytes(Bitstream, RegionInfo.BitstreamIndex, RegionSector.DataLength + MFM_SYNC_MARK_BYTES)
                Dim CalculatedChecksum = MFMCRC16(Buffer)

                Dim RegionSize As UInteger

                If TrackType = BitstreamTrackType.MFM Then
                    RegionSize = DAMPattern.Length
                    RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.DAMSync, RegionInfo.ByteIndex, MFMBitsToBytes(RegionSize), RegionSector, RegionInfo.BitOffset))
                    RegionInfo.AddBits(RegionSize)
                End If

                RegionSector.DAM = MFMGetByte(Bitstream, RegionInfo.BitstreamIndex)
                RegionSize = MFM_BYTE_BYTES
                RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.DAM, RegionInfo.ByteIndex, MFMBitsToBytes(RegionSize), RegionSector, RegionInfo.BitOffset))
                RegionInfo.AddBits(RegionSize)

                Dim SectorSize = RegionSector.DataLength
                Dim SectorLength = MFMBitsToBytes(EndIndex - RegionInfo.BitstreamIndex)

                If SectorSize > SectorLength Then
                    SectorSize = SectorLength
                    RegionSector.Overlaps = True
                Else
                    RegionSector.Overlaps = False
                End If

                If RegionSector.Overlaps Then
                    Buffer = MFMGetBytes(Bitstream, RegionInfo.BitstreamIndex, SectorSize)
                    SyncNullCount = GetByteCount(Buffer, 0, 0)
                    SectorSize -= SyncNullCount
                    Dim GapCount = GetByteCount(Buffer, MFM_GAP_BYTE, SyncNullCount, 6)
                    SectorSize -= GapCount
                End If

                RegionSector.DataStartIndex = RegionInfo.ByteIndex
                RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.DataArea, RegionInfo.ByteIndex, SectorSize, RegionSector, RegionInfo.BitOffset))
                If RegionInfo.ByteIndex + RegionSector.DataLength > RegionData.NumBytes Then
                    RegionData.NumBytes = RegionInfo.ByteIndex + RegionSector.DataLength
                End If

                Buffer = MFMGetBytes(Bitstream, RegionInfo.BitstreamIndex + MFMBytesToBits(RegionSector.DataLength), MFM_CRC_BYTES)
                Dim Checksum = BitConverter.ToUInt16(Buffer, 0)
                RegionSector.DataChecksumValid = (Checksum = CalculatedChecksum)

                RegionInfo.AddBytes(SectorSize)

                If Not RegionSector.Overlaps And SectorSize < SectorLength - MFM_CRC_BYTES Then
                    Dim RegionType As MFMRegionType

                    If RegionSector.DataChecksumValid Then
                        RegionType = MFMRegionType.DataChecksumValid
                    Else
                        RegionType = MFMRegionType.DataChecksumInvalid
                    End If

                    RegionSize = MFM_CRC_BYTES
                    RegionData.Regions.Add(New BitstreamRegion(RegionType, RegionInfo.ByteIndex, RegionSize, RegionSector, RegionInfo.BitOffset))
                    RegionInfo.AddBytes(RegionSize)
                End If

                RegionSector.HasData = True

                Return RegionInfo
            End Function

            Private Function MFMProcessIAMRegion(Bitstream As BitArray, TrackType As BitstreamTrackType, RegionData As BitstreamRegionData, PrevRegion As MFMRegionInfo, EndIndex As UInteger) As MFMRegionInfo
                Dim IAMPattern As BitArray

                If TrackType = BitstreamTrackType.MFM Then
                    IAMPattern = BytesToBits(MFM_IAM_SYNC_PATTERN_BYTES)
                Else
                    IAMPattern = BytesToBits(FM_IAM_SYNC_PATTERN_BYTES)
                End If

                Dim Index = FindPattern(Bitstream, IAMPattern, PrevRegion.BitstreamIndex)

                If Index < 0 Or Index >= EndIndex Then
                    Return PrevRegion
                End If

                Dim RegionInfo As New MFMRegionInfo(Index)

                If RegionInfo.BitOffset > 0 Then
                    RegionData.Aligned = False
                End If

                If Index >= PrevRegion.BitstreamIndex + MFM_BYTE_BYTES Then
                    Dim SyncNullCount = GetSyncNullCount(Bitstream, PrevRegion.BitstreamIndex, Index)

                    Dim Buffer = GetGapBytes(Bitstream, PrevRegion.BitstreamIndex, Index - MFMBytesToBits(SyncNullCount))
                    RegionData.Gap4A = Buffer.Length

                    If Buffer.Length > 0 Then
                        RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.Gap4A, PrevRegion.ByteIndex, Buffer.Length, RegionInfo.BitOffset))
                    End If

                    If SyncNullCount > 0 Then
                        RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.IAMNulls, RegionInfo.ByteIndex - SyncNullCount, SyncNullCount, RegionInfo.BitOffset))
                    End If
                End If

                Dim RegionSize As UInteger

                If TrackType = BitstreamTrackType.MFM Then
                    RegionSize = IAMPattern.Length
                    RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.IAMSync, RegionInfo.ByteIndex, MFMBitsToBytes(RegionSize), RegionInfo.BitOffset))
                    RegionInfo.AddBits(RegionSize)
                End If

                RegionSize = MFM_BYTE_BYTES
                RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.IAM, RegionInfo.ByteIndex, MFMBitsToBytes(RegionSize), RegionInfo.BitOffset))
                RegionInfo.AddBits(RegionSize)

                Return RegionInfo
            End Function

            Private Function MFMProcessRegionSector(Bitstream As BitArray, TrackType As BitstreamTrackType, RegionData As BitstreamRegionData, PrevRegion As MFMRegionInfo, RegionSector As BitstreamRegionSector, PrevRegionSector As BitstreamRegionSector) As MFMRegionInfo
                Dim IDAMPattern As BitArray
                Dim IDAMByteLength As Byte

                If TrackType = BitstreamTrackType.MFM Then
                    IDAMPattern = BytesToBits(MFM_SYNC_PATTERN_BYTES)
                    IDAMByteLength = MFM_SYNC_MARK_BYTES + MFM_IDAREA_BYTES
                Else
                    IDAMPattern = BytesToBits(FM_IDAM_SYNC_PATTERN_BYTES)
                    IDAMByteLength = FM_FIELD_BYTES + MFM_IDAREA_BYTES
                End If

                Dim Buffer As Byte()
                Dim RegionType As MFMRegionType

                Dim RegionInfo As New MFMRegionInfo(RegionSector.StartIndexBits)

                If RegionInfo.BitOffset > 0 Then
                    RegionData.Aligned = False
                End If

                If RegionSector.StartIndexBits >= PrevRegion.BitstreamIndex + MFM_BYTE_BYTES Then
                    Dim SyncNullCount = GetSyncNullCount(Bitstream, PrevRegion.BitstreamIndex, RegionSector.StartIndexBits)

                    Buffer = GetGapBytes(Bitstream, PrevRegion.BitstreamIndex, RegionSector.StartIndexBits - MFMBytesToBits(SyncNullCount))

                    If Buffer.Length > 0 Then
                        If RegionSector.SectorIndex = 0 Then
                            RegionType = MFMRegionType.Gap1
                            RegionData.Gap1 = Buffer.Length
                        Else
                            RegionType = MFMRegionType.Gap3
                            RegionData.Sectors.Item(RegionSector.SectorIndex - 1).Gap3 = Buffer.Length
                        End If
                        RegionData.Regions.Add(New BitstreamRegion(RegionType, PrevRegion.ByteIndex, Buffer.Length, PrevRegionSector, RegionInfo.BitOffset))
                    End If

                    If SyncNullCount > 0 Then
                        RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.IDAMNulls, RegionInfo.ByteIndex - SyncNullCount, SyncNullCount, RegionSector, RegionInfo.BitOffset))
                    End If

                    RegionSector.StartIndex -= SyncNullCount

                    If PrevRegionSector IsNot Nothing Then
                        PrevRegionSector.Length -= SyncNullCount
                    End If
                End If

                Buffer = MFMGetBytes(Bitstream, RegionInfo.BitstreamIndex, IDAMByteLength)
                Dim CalculatedChecksum = MFMCRC16(Buffer)

                Dim RegionSize As UInteger

                If TrackType = BitstreamTrackType.MFM Then
                    RegionSize = IDAMPattern.Length
                    RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.IDAMSync, RegionInfo.ByteIndex, MFMBitsToBytes(RegionSize), RegionSector, RegionInfo.BitOffset))
                    RegionInfo.AddBits(RegionSize)
                End If

                RegionSize = MFM_BYTE_BYTES
                RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.IDAM, RegionInfo.ByteIndex, MFMBitsToBytes(RegionSize), RegionSector, RegionInfo.BitOffset))
                RegionInfo.AddBits(RegionSize)

                Buffer = MFMGetBytes(Bitstream, RegionInfo.BitstreamIndex, MFM_IDAREA_BYTES)
                RegionSector.Track = Buffer(0)
                RegionSector.Side = Buffer(1)
                RegionSector.SectorId = Buffer(2)
                RegionSector.DataLength = GetSectorSizeBytes(Buffer(3))

                RegionSize = MFM_IDAREA_BYTES
                RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.IDArea, RegionInfo.ByteIndex, RegionSize, RegionSector, RegionInfo.BitOffset))
                RegionInfo.AddBytes(RegionSize)

                Buffer = MFMGetBytes(Bitstream, RegionInfo.BitstreamIndex, MFM_CRC_BYTES)
                Dim Checksum = BitConverter.ToUInt16(Buffer, 0)
                RegionSector.IDAMChecksumValid = (Checksum = CalculatedChecksum)

                If RegionSector.IDAMChecksumValid Then
                    RegionType = MFMRegionType.IDAMChecksumValid
                Else
                    RegionType = MFMRegionType.IDAMChecksumInvalid
                End If

                RegionSize = MFM_CRC_BYTES
                RegionData.Regions.Add(New BitstreamRegion(RegionType, RegionInfo.ByteIndex, RegionSize, RegionSector, RegionInfo.BitOffset))
                RegionInfo.AddBytes(RegionSize)

                Return RegionInfo
            End Function

            Public Function MFMGetRegionList(Bitstream As BitArray, TrackType As BitstreamTrackType) As BitstreamRegionData
                Dim RegionData As New BitstreamRegionData With {
                    .NumBytes = Math.Ceiling(Bitstream.Length / MFM_BYTE_BYTES),
                    .NumBits = Bitstream.Length
                }

                If TrackType = BitstreamTrackType.MFM Then
                    RegionData.Encoding = "MFM"
                ElseIf TrackType = BitstreamTrackType.FM Then
                    RegionData.Encoding = "FM"
                Else
                    RegionData.Encoding = ""
                End If

                Dim Pattern As BitArray
                If TrackType = BitstreamTrackType.MFM Then
                    Pattern = BytesToBits(MFM_IDAM_SYNC_PATTERN_BYTES)
                Else
                    Pattern = BytesToBits(FM_IDAM_SYNC_PATTERN_BYTES)
                End If

                Dim SectorList = MFMGetSectorList(Bitstream, Pattern)

                Dim EndIndex As UInteger = Bitstream.Length
                If SectorList.Count > 0 Then
                    EndIndex = SectorList.Item(0)
                End If

                Dim RegionInfo As New MFMRegionInfo(0)

                RegionInfo = MFMProcessIAMRegion(Bitstream, TrackType, RegionData, RegionInfo, EndIndex)

                Dim RegionSector As BitstreamRegionSector = Nothing
                Dim SectorIndex As UShort = 0

                For Each Index In SectorList
                    Dim PrevRegionSector = RegionSector
                    If PrevRegionSector IsNot Nothing Then
                        PrevRegionSector.Length = MFMBitsToBytes(Index) - PrevRegionSector.StartIndex
                    End If

                    RegionSector = New BitstreamRegionSector With {
                        .StartIndexBits = Index,
                        .StartIndex = MFMBitsToBytes(Index),
                        .SectorIndex = SectorIndex,
                        .Gap3 = 0,
                        .HasWeakBits = False,
                        .WriteSplice = False
                    }
                    RegionData.Sectors.Add(RegionSector)

                    RegionInfo = MFMProcessRegionSector(Bitstream, TrackType, RegionData, RegionInfo, RegionSector, PrevRegionSector)

                    Dim SectorEnd As UInteger

                    If RegionSector.SectorIndex < SectorList.Count - 1 Then
                        SectorEnd = SectorList.Item(RegionSector.SectorIndex + 1)
                        'Detect invalid data length - Formaster Copylock
                        If RegionSector.DataLength = 256 Then
                            'Not sure why I chose 526 - Need to research
                            If RegionInfo.BitstreamIndex + MFMBytesToBits(526) < SectorEnd Then
                                RegionSector.DataLength = 512
                            End If
                        End If
                    Else
                        SectorEnd = Bitstream.Length
                    End If

                    RegionInfo = MFMProcessDAMRegion(Bitstream, TrackType, RegionData, RegionSector, RegionInfo, SectorEnd)

                    SectorIndex += 1
                Next

                If RegionInfo.BitstreamIndex > 0 And RegionInfo.BitstreamIndex < Bitstream.Length Then
                    Dim GapCount As UInteger = Math.Ceiling((Bitstream.Length - RegionInfo.BitstreamIndex) / MFM_BYTE_BYTES)
                    RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.Gap4B, RegionInfo.ByteIndex, GapCount, RegionSector, RegionInfo.BitOffset))
                    If RegionSector IsNot Nothing Then
                        RegionSector.Length = Math.Ceiling(Bitstream.Length / MFM_BYTE_BYTES) - RegionSector.StartIndex
                    End If
                End If

                RegionInfo.SetByteIndex(Math.Ceiling(Bitstream.Length / MFM_BYTE_BYTES))
                If RegionData.NumBytes > RegionInfo.ByteIndex Then
                    Dim Length = RegionData.NumBytes - RegionInfo.ByteIndex
                    RegionData.Regions.Add(New BitstreamRegion(MFMRegionType.Overflow, RegionInfo.ByteIndex, Length, RegionInfo.BitOffset))
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
                Dim Diff = Math.Truncate((OffsetEnd - OffsetStart) / MFM_BYTE_BYTES) * MFM_BYTE_BYTES

                OffsetEnd = OffsetStart + Diff

                Return MFMGetBytesByRange(BitStream, OffsetStart, OffsetEnd)
            End Function

            Private Function GetGapValues(BitStream As BitArray, OffsetStart As UInteger, OffsetEnd As UInteger) As HashSet(Of UShort)
                Const TopN As Integer = 3
                Const MinFrequency As Integer = 2

                If OffsetEnd < OffsetStart + MFM_BYTE_BYTES Then
                    Return New HashSet(Of UShort)
                End If

                Dim GapValues As New Dictionary(Of UShort, Integer)

                For Offset As UInteger = OffsetStart To OffsetEnd - MFM_BYTE_BYTES Step MFM_BYTE_BYTES
                    Dim Value = GetWordFromBitArray(BitStream, Offset)
                    If GapValues.ContainsKey(Value) Then
                        GapValues(Value) += 1
                    Else
                        GapValues(Value) = 1
                    End If
                Next

                Dim Frequent = GapValues.
                    Where(Function(kv) kv.Value >= MinFrequency).
                    OrderByDescending(Function(kv) kv.Value).
                    Take(TopN)

                Return Frequent.Select(Function(kv) kv.Key).ToHashSet
            End Function

            Private Function GetSyncNullCount(BitStream As BitArray, OffsetStart As UInteger, OffsetEnd As UInteger) As UInteger
                Dim Count As UInteger = 0

                If OffsetEnd < OffsetStart + MFM_BYTE_BYTES Then
                    Return Count
                End If

                Do
                    OffsetEnd -= MFM_BYTE_BYTES
                    Dim Value = GetWordFromBitArray(BitStream, OffsetEnd)
                    If Value <> MFM_NULL_WORD Then
                        Exit Do
                    End If
                    Count += 1
                Loop While OffsetEnd >= OffsetStart + MFM_BYTE_BYTES

                Return Count
            End Function

            Private Function HasWriteSpliceStart(BitStream As BitArray, OffsetStart As UInteger, OffsetEnd As UInteger) As Boolean
                Const MaxWordsToCheck As Integer = 2
                Const MaxAllowedBadGaps As Integer = 1
                Dim Result As Boolean = False
                Dim WordCount As UInteger = 0
                Dim BadGapCount As UInteger = 0

                If OffsetEnd < OffsetStart + MFM_BYTE_BYTES Then
                    Return Result
                End If

                'Get possible gap values
                Dim Allowed = GetGapValues(BitStream, OffsetStart, OffsetEnd)

                'If there is more than one bad gap word then we have a write splice
                Dim Offset As Integer = OffsetEnd - MFM_BYTE_BYTES

                Do While Offset >= OffsetStart AndAlso WordCount < MaxWordsToCheck
                    Dim Value = GetWordFromBitArray(BitStream, Offset)
                    If Not Allowed.Contains(Value) Then
                        BadGapCount += 1
                    End If
                    WordCount += 1
                    Offset -= MFM_BYTE_BYTES
                Loop

                Result = BadGapCount > MaxAllowedBadGaps

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

            Private Function GetWordFromBitArray(BitStream As BitArray, Offset As Integer) As UShort
                Dim Value As Integer = 0
                Dim BitCount As Integer = Math.Min(MFM_BYTE_BYTES, BitStream.Length - Offset)

                For i As Integer = 0 To BitCount - 1
                    If BitStream(Offset + i) Then
                        Value = Value Or (1 << i)
                    End If
                Next

                Return CUShort(Value)
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

                NumBytes \= MFM_BYTE_BYTES

                If NumBytes > 0 Then
                    Return MFMGetBytes(BitStream, OffsetStart, NumBytes)
                End If

                Return New Byte(-1) {}
            End Function

            Public Function MFMGetSize(RPM As UShort, DataRate As UShort) As UInteger
                Dim MicrosecondsPerRev As Single = 1 / RPM * 60 * 1000000
                Dim MicrossecondsPerBit As Single = DataRate / 1000

                Return Math.Ceiling(MicrosecondsPerRev * MicrossecondsPerBit / 8) * 16
            End Function

            Public Function ResizeBitstream(Bitstream As BitArray, Length As UInteger) As BitArray
                Dim NewBitstream = CType(Bitstream.Clone, BitArray)
                NewBitstream.Length = Length

                Return NewBitstream
            End Function
        End Module
    End Namespace
End Namespace
