Namespace ImageFormats.TD0
    Module TD0Rle
        Friend Function RleDecode(src As Byte(), srcOffset As Integer, decodedLength As Integer) As (Result As Boolean, Data As Byte(), BytesConsumed As Integer, BytesWritten As Integer)
            If src Is Nothing OrElse decodedLength < 0 OrElse srcOffset < 0 OrElse srcOffset > src.Length Then
                Return (False, Nothing, 0, 0)
            End If

            Dim dst(decodedLength - 1) As Byte

            Dim srcEnd As Integer = src.Length
            Dim s As Integer = srcOffset
            Dim d As Integer = 0

            While d < decodedLength
                ' Need lenCode + rep
                If s + 2 > srcEnd Then
                    Return (False, Nothing, s - srcOffset, d)
                End If

                Dim lenCode As Integer = src(s)
                Dim rep As Integer = src(s + 1)
                s += 2

                If lenCode = 0 Then
                    ' Literal run
                    If s + rep > srcEnd Then
                        Return (False, Nothing, s - srcOffset, d)
                    End If

                    Dim remaining As Integer = decodedLength - d
                    Dim copyLen As Integer = Math.Min(rep, remaining)
                    If copyLen > 0 Then
                        Array.Copy(src, s, dst, d, copyLen)
                        d += copyLen
                    End If

                    s += rep
                Else
                    ' Defensive sanity check (TD0 never needs > 128-byte patterns)
                    If lenCode > 7 Then
                        Return (False, Nothing, s - srcOffset, d)
                    End If

                    Dim blockLen As Integer = (1 << lenCode)
                    If s + blockLen > srcEnd Then
                        Return (False, Nothing, s - srcOffset, d)
                    End If

                    Dim remaining As Integer = decodedLength - d
                    Dim total As Integer = blockLen * rep
                    If total > remaining Then total = remaining

                    Dim patternOfs As Integer = s
                    s += blockLen

                    Dim wrote As Integer = 0
                    While wrote < total
                        Dim n As Integer = Math.Min(blockLen, total - wrote)
                        Array.Copy(src, patternOfs, dst, d + wrote, n)
                        wrote += n
                    End While

                    d += total
                End If
            End While

            Return (True, dst, s - srcOffset, d)
        End Function

        Friend Function RleEncode(Input As Byte()) As (Result As Boolean, Data As Byte(), BytesConsumed As Integer, BytesWritten As Integer)
            If Input Is Nothing Then
                Return (False, Nothing, 0, 0)
            End If

            If Input.Length = 0 Then
                Return (True, Array.Empty(Of Byte)(), 0, 0)
            End If

            Dim ms As New IO.MemoryStream(Input.Length \ 2)

            Dim i As Integer = 0
            Dim n As Integer = Input.Length

            Dim litStart As Integer = 0
            Dim litLen As Integer = 0

            ' Flush pending literal bytes as one or more len<=255 chunks
            Dim FlushLiteral =
            Sub()
                Dim pos As Integer = litStart
                Dim remaining As Integer = litLen

                While remaining > 0
                    Dim chunk As Integer = Math.Min(255, remaining)
                    ms.WriteByte(0)                  ' lenCode=0 => literal
                    ms.WriteByte(CByte(chunk))       ' rep=literal length
                    ms.Write(Input, pos, chunk)
                    pos += chunk
                    remaining -= chunk
                End While

                litLen = 0
            End Sub

            While i < n
                ' Find best repeating pattern at i
                Dim bestLenCode As Integer = -1
                Dim bestBlockLen As Integer = 0
                Dim bestRep As Integer = 0
                Dim bestTotal As Integer = 0
                Dim bestGain As Integer = 0

                ' lenCode 1..7 => blockLen 2..128 (TD0 never needs >128 here)
                For lenCode As Integer = 1 To 7
                    Dim blockLen As Integer = (1 << lenCode)
                    If i + blockLen > n Then Exit For

                    Dim maxRep As Integer = Math.Min(255, (n - i) \ blockLen)
                    If maxRep < 2 Then Continue For

                    Dim rep As Integer = 1
                    While rep < maxRep AndAlso BlocksEqual(Input, i, i + rep * blockLen, blockLen)
                        rep += 1
                    End While

                    If rep >= 2 Then
                        Dim total As Integer = rep * blockLen
                        ' Literal size for total bytes would be 2 + total
                        ' Pattern size is 2 + blockLen
                        ' So "gain" (bytes saved) is total - blockLen
                        Dim gain As Integer = total - blockLen

                        If gain > bestGain Then
                            bestGain = gain
                            bestLenCode = lenCode
                            bestBlockLen = blockLen
                            bestRep = rep
                            bestTotal = total
                        End If
                    End If
                Next

                If bestLenCode <> -1 Then
                    ' We found a good pattern run. Flush any pending literal first.
                    If litLen > 0 Then FlushLiteral()

                    ms.WriteByte(CByte(bestLenCode))
                    ms.WriteByte(CByte(bestRep))
                    ms.Write(Input, i, bestBlockLen) ' store pattern once
                    i += bestTotal
                    litStart = i
                Else
                    ' No pattern run found; accumulate literal
                    If litLen = 0 Then litStart = i
                    litLen += 1
                    i += 1

                    ' Literal chunks limited to 255 bytes
                    If litLen = 255 Then FlushLiteral()
                End If
            End While

            If litLen > 0 Then FlushLiteral()

            Dim outBytes = ms.ToArray()
            Return (True, outBytes, Input.Length, outBytes.Length)
        End Function

        Private Function BlocksEqual(buf As Byte(), a As Integer, b As Integer, count As Integer) As Boolean
            ' Caller ensures ranges are valid
            For k As Integer = 0 To count - 1
                If buf(a + k) <> buf(b + k) Then
                    Return False
                End If
            Next

            Return True
        End Function
    End Module
End Namespace
