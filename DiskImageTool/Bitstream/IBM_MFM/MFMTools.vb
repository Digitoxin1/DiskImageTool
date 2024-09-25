Namespace IBM_MFM
    Public Enum MFMAddressMark As Byte
        Index = &HFC
        ID = &HFE
        Data = &HFB
        DeletedData = &HF8
    End Enum

    Module MFMTools
        Public Const MFM_BYTE_SIZE As Byte = 16
        Public Const SYNC_NULL_LENGTH As Integer = 12 * MFM_BYTE_SIZE
        Public ReadOnly IAM_Sync_Bytes() As Byte = {&H52, &H24, &H52, &H24, &H52, &H24}
        Public ReadOnly IDAM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89, &H55, &H54}
        Public ReadOnly DAM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89, &H55, &H45}
        Public ReadOnly MFM_Sync_Bytes() As Byte = {&H44, &H89, &H44, &H89, &H44, &H89}

        Public Function DecodeTrack(Bitstream As BitArray) As Byte()
            Return MFMGetBytes(Bitstream, 0, Bitstream.Length \ MFM_BYTE_SIZE)
        End Function

        Public Function DecodeTrack(Data() As Byte) As Byte()
            Return DecodeTrack(MyBitConverter.BytesToBits(Data))
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
    End Module
End Namespace
