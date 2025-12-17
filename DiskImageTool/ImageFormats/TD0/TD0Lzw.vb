Imports System.IO

Namespace ImageFormats.TD0

    ''' <summary>
    ''' TeleDisk 1.x LZW decompressor matching the 86Box-integrated Dipperstein LZW decoder:
    ''' - Fixed 12-bit codes packed in a nibble-addressed stream (bufPos += 3 nibbles per code)
    ''' - Blocked stream: [UInt16LE bufLenNibbles] + payload bytes (ceil(bufLenNibbles/2))
    ''' - Blocks are up to 0x3000 nibbles (0x1800 bytes). Stop after a block < 0x1800 bytes.
    ''' </summary>
    Friend NotInheritable Class TD0Lzw

        Private Const FIRST_CODE As Integer = 256
        Private Const MAX_CODES As Integer = 4096                 ' 12-bit
        Private Const MAX_BLOCK_BYTES As Integer = &H1800         ' 0x1800 bytes payload
        Private Const MAX_BLOCK_NIBBLES As Integer = &H3000       ' 0x3000 nibbles

        Private Structure DecodeEntry
            Public Suffix As Byte
            Public Prefix As UShort
        End Structure

        Public Shared Function Decompress(srcData As Byte(), maxOut As Integer) As Byte()
            If srcData Is Nothing OrElse maxOut <= 0 Then Return Array.Empty(Of Byte)()

            Dim out As New MemoryStream(Math.Min(maxOut, 64 * 1024))

            Dim pos As Integer = 0
            Dim srcLen As Integer = srcData.Length

            While True
                ' Need 2 bytes for block length
                If pos + 2 > srcLen Then Exit While

                Dim bufLenNibbles As Integer = ReadUInt16LE(srcData, pos)
                pos += 2

                ' bufLenNibbles is the "bufLen" used by GetCodeWord (measured in nibbles)
                Dim payloadBytes As Integer = bufLenNibbles \ 2
                If (bufLenNibbles And 1) <> 0 Then payloadBytes += 1

                If payloadBytes > MAX_BLOCK_BYTES Then
                    ' matches C: if (size > 0x1800) return -1;
                    Exit While
                End If

                If pos + payloadBytes > srcLen Then Exit While

                ' Decode one block
                DecodeBlock(srcData, pos, bufLenNibbles, out, maxOut)

                pos += payloadBytes

                ' 86Box rule: last block is the first block smaller than 0x1800 bytes
                ' (or if we reached src_len)
                If payloadBytes < MAX_BLOCK_BYTES OrElse pos >= srcLen Then Exit While

                If out.Length >= maxOut Then Exit While
            End While

            Return out.ToArray()
        End Function

        Private Shared Sub DecodeBlock(src As Byte(), blockStart As Integer, bufLenNibbles As Integer, out As MemoryStream, maxOut As Integer)
            Dim dict(MAX_CODES - FIRST_CODE - 1) As DecodeEntry

            Dim bufPosNibbles As Integer = 0
            Dim bufOutPosBefore As Long = out.Length

            ' First code must be a literal (0..255)
            Dim lastCode As Integer = GetCodeWord(src, blockStart, bufLenNibbles, bufPosNibbles)
            If lastCode < 0 OrElse lastCode > 255 Then Exit Sub

            Dim c As Byte = CByte(lastCode)
            WriteByte(out, c, maxOut)

            Dim nextCode As Integer = FIRST_CODE

            While True
                Dim code As Integer = GetCodeWord(src, blockStart, bufLenNibbles, bufPosNibbles)
                If code < 0 Then Exit While

                If code < nextCode Then
                    ' known code
                    c = DecodeString(code, dict, out, maxOut)
                Else
                    ' unknown code special case
                    Dim tmp As Byte = c
                    c = DecodeString(lastCode, dict, out, maxOut)
                    WriteByte(out, tmp, maxOut)
                End If

                ' add new dictionary entry if room
                If nextCode < MAX_CODES Then
                    dict(nextCode - FIRST_CODE).Prefix = CUShort(lastCode)
                    dict(nextCode - FIRST_CODE).Suffix = c
                    nextCode += 1
                End If

                lastCode = code

                If out.Length >= maxOut Then Exit While
            End While
        End Sub

        ''' <summary>
        ''' Decodes a code into bytes written to out, and returns the first character of the decoded string.
        ''' Iterative equivalent of DecodeRecursive() to avoid deep recursion.
        ''' </summary>
        Private Shared Function DecodeString(code As Integer, dict() As DecodeEntry, out As MemoryStream, maxOut As Integer) As Byte
            ' Walk prefixes pushing suffix bytes
            Dim stack As New List(Of Byte)(64)

            Dim cur As Integer = code
            While cur >= FIRST_CODE
                Dim e As DecodeEntry = dict(cur - FIRST_CODE)
                stack.Add(e.Suffix)
                cur = e.Prefix
            End While

            ' cur is now a literal byte, and is the "firstChar"
            Dim firstChar As Byte = CByte(cur And &HFF)

            ' Output in correct order: base char, then suffixes in reverse push order
            WriteByte(out, firstChar, maxOut)
            For i As Integer = stack.Count - 1 To 0 Step -1
                WriteByte(out, stack(i), maxOut)
            Next

            Return firstChar
        End Function

        ''' <summary>
        ''' Matches GetCodeWord() in your C:
        ''' - bufPos is in nibbles (4-bit units)
        ''' - realPos = bufPos >> 1 is a byte index into the block payload
        ''' - even bufPos: code = b0 | ((b1 & 0x0F) << 8)
        ''' - odd  bufPos: code = (highNibble(b0)) | (b1 << 4)
        ''' - bufPos += 3 nibbles each code (12 bits)
        ''' Returns -1 for EOF.
        ''' </summary>
        Private Shared Function GetCodeWord(src As Byte(), blockStart As Integer, bufLenNibbles As Integer, ByRef bufPosNibbles As Integer) As Integer
            If bufPosNibbles >= bufLenNibbles Then Return -1

            Dim realPos As Integer = bufPosNibbles >> 1
            Dim b0 As Integer = src(blockStart + realPos) And &HFF
            Dim b1 As Integer = src(blockStart + realPos + 1) And &HFF

            Dim code As Integer
            If (bufPosNibbles And 1) <> 0 Then
                ' odd position
                code = ((b0 And &HF0) >> 4) Or (b1 << 4)
            Else
                ' even position
                code = b0 Or ((b1 And &HF) << 8)
            End If

            bufPosNibbles += 3
            Return code And &HFFF
        End Function

        Private Shared Sub WriteByte(out As MemoryStream, b As Byte, maxOut As Integer)
            If out.Length >= maxOut Then Exit Sub
            out.WriteByte(b)
        End Sub

        Private Shared Function ReadUInt16LE(b() As Byte, offset As Integer) As Integer
            Return (b(offset) And &HFF) Or ((b(offset + 1) And &HFF) << 8)
        End Function

    End Class

End Namespace
