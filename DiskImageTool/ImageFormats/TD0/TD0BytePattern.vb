Namespace ImageFormats.TD0
    Module TD0BytePattern
        Friend Function BytePatternCanEncode(data As Byte()) As Boolean
            If data Is Nothing OrElse data.Length < 2 Then
                Return False
            End If

            Dim p0 As Byte = data(0)
            Dim p1 As Byte = data(1)

            Dim i As Integer = 0
            Dim n As Integer = data.Length

            ' Check full 2-byte pairs
            While i + 1 < n
                If data(i) <> p0 OrElse data(i + 1) <> p1 Then
                    Return False
                End If
                i += 2
            End While

            ' Optional odd trailing byte (rare for TD0, but allowed)
            If i < n Then
                If data(i) <> p0 Then Return False
            End If

            Return True
        End Function

        Friend Function BytePatternDecode(src As Byte(),
                                                  srcOffset As Integer,
                                          decodedLength As Integer,
                                          payloadBytes As Integer) As (Result As Boolean, Data As Byte(), BytesConsumed As Integer, BytesWritten As Integer)

            If src Is Nothing OrElse decodedLength < 0 OrElse payloadBytes < 0 OrElse srcOffset < 0 OrElse srcOffset > src.Length Then
                Return (False, Nothing, 0, 0)
            End If

            Dim dst(decodedLength - 1) As Byte

            Dim srcEnd As Integer = Math.Min(src.Length, srcOffset + payloadBytes)
            Dim s As Integer = srcOffset
            Dim d As Integer = 0

            While d < decodedLength AndAlso (s + 4) <= srcEnd
                Dim countWords As Integer = ReadUInt16LE(src, s)
                Dim p0 As Byte = src(s + 2)
                Dim p1 As Byte = src(s + 3)
                s += 4

                If countWords <= 0 Then Exit While

                Dim bytesToWrite As Integer = countWords * 2
                Dim remaining As Integer = decodedLength - d
                If bytesToWrite > remaining Then bytesToWrite = remaining

                Dim pairs As Integer = bytesToWrite \ 2
                For i As Integer = 0 To pairs - 1
                    dst(d) = p0 : d += 1
                    dst(d) = p1 : d += 1
                Next

                ' odd tail (kept to match your original safety behavior)
                If (bytesToWrite And 1) <> 0 Then
                    dst(d) = p0
                    d += 1
                End If
            End While

            Return (True, dst, s - srcOffset, d)
        End Function
        Friend Function BytePatternEncode(data As Byte()) As Byte()
            Dim countWords As Integer = data.Length \ 2

            Dim out(3) As Byte
            WriteUInt16LE(out, 0, countWords)
            out(2) = data(0)
            out(3) = data(1)

            Return out
        End Function

    End Module
End Namespace
