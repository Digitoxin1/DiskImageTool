Namespace Bitstream
    Module Functions
        Public Function CalculatetBitCount(BitRate As UShort, RPM As UShort) As UInteger
            Dim MicrosecondsPerRev As Single = 1 / RPM * 60 * 1000000
            Dim MicrossecondsPerBit As Single = BitRate / 1000

            Return (MicrosecondsPerRev * MicrossecondsPerBit \ 8) * 16
        End Function

        Public Function CalculatetRPM(BitRate As UShort, BitCount As UInteger) As UShort
            Dim MicrossecondsPerBit As Single = BitRate / 1000
            Dim MicrosecondsPerRev As Single = BitCount / (2 * MicrossecondsPerBit)

            Return RoundRPM((60 * 1000000) / MicrosecondsPerRev)
        End Function

        Public Function InferRPM(BitCount As UInteger) As UShort
            BitCount = Math.Round(BitCount / 10000) * 10000

            If BitCount = 100000 Then
                Return 300
            ElseIf BitCount = 170000 Then
                Return 360
            ElseIf BitCount = 200000 Then
                Return 300
            ElseIf BitCount = 400000 Then
                Return 300
            Else
                Return 0
            End If
        End Function

        Public Function InferBitRate(BitCount As UInteger) As UShort
            BitCount = Math.Round(BitCount / 10000) * 10000

            If BitCount = 100000 Then
                Return 250
            ElseIf BitCount = 170000 Then
                Return 500
            ElseIf BitCount = 200000 Then
                Return 500
            ElseIf BitCount = 400000 Then
                Return 1000
            ElseIf BitCount = 800000 Then
                Return 2000
            Else
                Return 0
            End If
        End Function

        Public Function RoundBitRate(BitRate As UShort) As UShort
            Return Math.Round(BitRate / 50) * 50
        End Function

        Public Function RoundRPM(RPM As UShort) As UShort
            Return Math.Round(RPM / 60) * 60
        End Function
    End Module
End Namespace
