Public Class Crc32
    Shared ReadOnly table As UInteger()

    Shared Sub New()
        Dim poly As UInteger = &HEDB88320UI
        table = New UInteger(255) {}
        Dim temp As UInteger
        For i As UInteger = 0 To table.Length - 1
            temp = i
            For j As Integer = 8 To 1 Step -1
                If (temp And 1) = 1 Then
                    temp = CUInt((temp >> 1) Xor poly)
                Else
                    temp >>= 1
                End If
            Next
            table(i) = temp
        Next
    End Sub

    Public Shared Function ComputeChecksum(bytes As Byte()) As UInteger
        Dim crc As UInteger = &HFFFFFFFFUI
        For i As Integer = 0 To bytes.Length - 1
            Dim index As Byte = CByte(((crc) And &HFF) Xor bytes(i))
            crc = CUInt((crc >> 8) Xor table(index))
        Next
        Return Not crc
    End Function

End Class