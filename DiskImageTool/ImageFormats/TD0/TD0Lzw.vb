Imports System.IO

Namespace ImageFormats.TD0

    Friend NotInheritable Class TD0Lzw

        Private Const CLEAR As Integer = 256
        Private Const EOI As Integer = 257
        Private Const FIRST As Integer = 258
        Private Const MAX_BITS As Integer = 12
        Private Const MAX_CODES As Integer = (1 << MAX_BITS)

        Public Shared Function DecompressToBuffer(src() As Byte, maxOut As Integer) As Byte()
            If src Is Nothing OrElse src.Length = 0 Then
                Return Array.Empty(Of Byte)()
            End If

            If maxOut <= 0 Then
                maxOut = 1024 * 1024 * 4
            End If

            Dim prefix(MAX_CODES - 1) As Short
            Dim suffix(MAX_CODES - 1) As Byte
            Dim stack(MAX_CODES - 1) As Byte

            For i = 0 To 255
                prefix(i) = -1
                suffix(i) = CByte(i)
            Next

            Dim bitPos As Integer = 0
            Dim codeSize As Integer = 9
            Dim nextCode As Integer = FIRST
            Dim oldCode As Integer = -1
            Dim firstChar As Byte = 0

            Using ms As New MemoryStream(Math.Min(maxOut, 1024 * 1024))
                While ms.Length < maxOut
                    Dim code As Integer = ReadCode(src, bitPos, codeSize)
                    If code < 0 Then
                        Exit While
                    End If

                    If code = CLEAR Then
                        codeSize = 9
                        nextCode = FIRST
                        oldCode = -1
                        Continue While
                    ElseIf code = EOI Then
                        Exit While
                    End If

                    Dim inCode As Integer = code
                    Dim sp As Integer = 0

                    If oldCode = -1 Then
                        ' first symbol
                        Dim ch As Byte = ResolveFirstChar(prefix, suffix, code)
                        ms.WriteByte(ch)
                        firstChar = ch
                        oldCode = code
                        Continue While
                    End If

                    If code >= nextCode Then
                        ' KwKwK case
                        stack(sp) = firstChar : sp += 1
                        code = oldCode
                    End If

                    ' decode string for code into stack
                    While code >= 256
                        stack(sp) = suffix(code) : sp += 1
                        code = prefix(code)
                        If sp >= stack.Length Then
                            Exit While
                        End If
                    End While

                    firstChar = suffix(code)
                    stack(sp) = firstChar : sp += 1

                    ' output reversed
                    For i = sp - 1 To 0 Step -1
                        If ms.Length >= maxOut Then
                            Exit For
                        End If
                        ms.WriteByte(stack(i))
                    Next

                    ' add new entry
                    If nextCode < MAX_CODES Then
                        prefix(nextCode) = CShort(oldCode)
                        suffix(nextCode) = firstChar
                        nextCode += 1

                        If nextCode = (1 << codeSize) AndAlso codeSize < MAX_BITS Then
                            codeSize += 1
                        End If
                    End If

                    oldCode = inCode
                End While

                Return ms.ToArray()
            End Using
        End Function

        Private Shared Function ReadCode(src() As Byte, ByRef bitPos As Integer, codeSize As Integer) As Integer
            Dim totalBits As Integer = src.Length * 8
            If bitPos + codeSize > totalBits Then
                Return -1
            End If

            Dim val As Integer = 0
            For i = 0 To codeSize - 1
                Dim byteIndex As Integer = (bitPos + i) >> 3
                Dim bitIndex As Integer = (bitPos + i) And 7
                Dim bit As Integer = (src(byteIndex) >> bitIndex) And 1
                val = val Or (bit << i) ' LSB-first bit packing
            Next

            bitPos += codeSize

            Return val
        End Function

        Private Shared Function ResolveFirstChar(prefix() As Short, suffix() As Byte, code As Integer) As Byte
            Dim c As Integer = code

            While c >= 256 AndAlso prefix(c) >= 0
                c = prefix(c)
            End While

            Return suffix(c)
        End Function
    End Class

End Namespace
