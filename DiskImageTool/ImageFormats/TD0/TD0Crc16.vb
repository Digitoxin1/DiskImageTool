Namespace ImageFormats.TD0
    Friend Module TD0Crc16
        Public Const POLY As UShort = &HA097US

        Public Function Compute(buf() As Byte, offset As Integer, count As Integer) As UShort
            Dim crc As UShort = 0US
            For i = 0 To count - 1
                crc = Update(crc, buf(offset + i))
            Next
            Return crc
        End Function

        Public Function LowByte(crc As UShort) As Byte
            Return CByte(crc And &HFFUS)
        End Function

        Public Function Update(crc As UShort, b As Byte) As UShort
            crc = CUShort(crc Xor (CUShort(b) << 8))
            For i = 0 To 7
                If (crc And &H8000US) <> 0US Then
                    crc = CUShort(((crc << 1) And &HFFFFUS) Xor POLY)
                Else
                    crc = CUShort((crc << 1) And &HFFFFUS)
                End If
            Next
            Return crc
        End Function
    End Module
End Namespace
