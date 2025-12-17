Namespace ImageFormats.PRI
    Public Class PRIChunk
        Private _ChunkCRC As UInt32 = 0
        Private _ChunkData As Byte()
        Private _ChunkID As Byte()

        Public Sub New()
            _ChunkID = New Byte(3) {}
            _ChunkData = New Byte(-1) {}
        End Sub

        Public Sub New(ChunkID As String)
            _ChunkID = Text.Encoding.UTF8.GetBytes(Left(ChunkID, 4))
            _ChunkData = New Byte(-1) {}
        End Sub

        Public Sub New(ChunkID As String, ChunkData() As Byte)
            _ChunkID = Text.Encoding.UTF8.GetBytes(Left(ChunkID, 4))
            _ChunkData = ChunkData
        End Sub

        Public Property ChunkData As Byte()
            Get
                Return _ChunkData
            End Get
            Set
                _ChunkData = Value
            End Set
        End Property

        Public Property ChunkID As String
            Get
                Return Text.Encoding.UTF8.GetString(_ChunkID, 0, 4)
            End Get
            Set(value As String)
                _ChunkID = Text.Encoding.UTF8.GetBytes(Left(value, 4))
            End Set
        End Property

        Public ReadOnly Property ChunkSize As UInt32
            Get
                Return _ChunkData.Length
            End Get
        End Property

        Public Function ReadFromBuffer(Buffer() As Byte, Offset As UInt32) As ReadResponse
            Dim Response As ReadResponse

            Array.Copy(Buffer, Offset, _ChunkID, 0, 4)

            Dim ChunkSize = MyBitConverter.ToUInt32(Buffer, True, Offset + 4)

            If ChunkSize <= Buffer.Length - Offset - 12 Then
                _ChunkData = New Byte(ChunkSize - 1) {}
                Array.Copy(Buffer, Offset + 8, _ChunkData, 0, ChunkSize)
                _ChunkCRC = MyBitConverter.ToUInt32(Buffer, True, Offset + 8 + ChunkSize)
            End If

            Response.Offset = Offset + 12 + ChunkSize
            Response.ChecksumVerified = CheckCRC()

            Return Response
        End Function

        Public Function ToBytes() As Byte()
            Dim ChunkSize = BitConverter.GetBytes(MyBitConverter.SwapEndian(CUInt(_ChunkData.Length)))

            Dim Buffer(_ChunkData.Length + 12 - 1) As Byte

            Array.Copy(_ChunkID, 0, Buffer, 0, 4)
            Array.Copy(ChunkSize, 0, Buffer, 4, 4)
            If _ChunkData.Length > 0 Then
                Array.Copy(_ChunkData, 0, Buffer, 8, _ChunkData.Length)
            End If

            Dim ChunkCRC = PSI_CRC(Buffer, Buffer.Length - 4)
            Dim CRCBytes = BitConverter.GetBytes(MyBitConverter.SwapEndian(ChunkCRC))

            Array.Copy(CRCBytes, 0, Buffer, 8 + _ChunkData.Length, 4)

            Return Buffer
        End Function

        Private Function CheckCRC() As Boolean
            Dim Buffer = ToBytes()
            Dim CRC = MyBitConverter.ToUInt32(Buffer, True, Buffer.Length - 4)

            Return (_ChunkCRC = CRC)
        End Function

        Private Function PSI_CRC(buf() As Byte, cnt As Integer) As UInteger
            Dim i As Integer
            Dim j As Integer
            Dim crc As UInteger = 0

            For i = 0 To cnt - 1
                crc = crc Xor (CUInt(buf(i)) << 24)

                For j = 0 To 7
                    If (crc And &H80000000UI) <> 0 Then
                        crc = (crc << 1) Xor &H1EDC6F41UI
                    Else
                        crc <<= 1
                    End If
                Next
            Next

            Return crc And &HFFFFFFFFUI
        End Function
    End Class
End Namespace