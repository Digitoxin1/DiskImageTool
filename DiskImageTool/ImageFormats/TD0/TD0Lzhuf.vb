Imports System.IO

Namespace ImageFormats.TD0

    ''' <summary>
    ''' TeleDisk 2.x (LZHUF) decompressor matching 86Box's implementation:
    ''' - LZSS (N=4096, F=60, THRESHOLD=2)
    ''' - Adaptive Huffman (Yoshizaki)
    ''' - MSB-first bitstream reader with 16-bit shift register
    ''' </summary>
    Friend NotInheritable Class TD0Lzhuf

        Private Const N As Integer = 4096
        Private Const F As Integer = 60
        Private Const THRESHOLD As Integer = 2

        Private Const N_CHAR As Integer = 256 - THRESHOLD + F
        Private Const T As Integer = N_CHAR * 2 - 1
        Private Const ROOT As Integer = T - 1
        Private Const MAX_FREQ As Integer = &H8000

        ' 86Box uses freq[T+1] with freq(T)=&HFFFF sentinel.
        Private ReadOnly freq(T) As UShort
        ' 86Box uses prnt[T + N_CHAR] (size = T+N_CHAR).
        Private ReadOnly prnt(T + N_CHAR - 1) As Short
        ' 86Box uses son[T] (size = T).
        Private ReadOnly son(T - 1) As Short
        ' N + F - 1 bytes
        Private ReadOnly textBuf(N + F - 2) As Byte

        ' LZSS state
        Private r As Integer
        Private bufcnt As Integer
        Private bufndx As Integer
        Private bufpos As Integer

        ' Bitstream state
        Private getbuf As UShort
        Private getlen As Integer

        Private src() As Byte
        Private srcPos As Integer
        Private output As MemoryStream

        Public Shared Function Decompress(srcData As Byte(), maxOut As Integer) As Byte()
            Dim d As New TD0Lzhuf()
            Return d.Decode(srcData, maxOut)
        End Function

        Private Function Decode(srcData As Byte(), maxOut As Integer) As Byte()
            If srcData Is Nothing OrElse maxOut <= 0 Then Return Array.Empty(Of Byte)()

            src = srcData
            srcPos = 0

            InitDecode()

            output = New MemoryStream(maxOut)

            While output.Length < maxOut
                Dim remaining As Integer = maxOut - CInt(output.Length)
                Dim written As Integer = DecodeChunk(remaining)
                If written <= 0 Then Exit While
            End While

            Return output.ToArray()
        End Function

        Private Sub InitDecode()
            getbuf = 0
            getlen = 0

            bufcnt = 0
            bufndx = 0
            bufpos = 0

            StartHuff()

            For i As Integer = 0 To (N - F - 1)
                textBuf(i) = &H20
            Next

            r = N - F
        End Sub

        ' ================= Bitstream =================

        Private Function NextWord() As Integer
            While getlen <= 8
                If srcPos >= src.Length Then Return -1
                Dim b As Integer = src(srcPos)
                srcPos += 1
                getbuf = CUShort(getbuf Or (CUShort(b) << (8 - getlen)))
                getlen += 8
            End While
            Return 0
        End Function

        Private Function GetBit() As Integer
            If NextWord() < 0 Then Return -1
            Dim bit As Integer = If((getbuf And &H8000US) <> 0US, 1, 0)
            getbuf = CUShort((getbuf << 1) And &HFFFFUS)
            getlen -= 1
            Return bit
        End Function

        Private Function GetByte() As Integer
            If NextWord() < 0 Then Return -1
            Dim i As Integer = (getbuf >> 8) And &HFF
            getbuf = CUShort((getbuf << 8) And &HFFFFUS)
            getlen -= 8
            Return i
        End Function

        ' ================= Huffman =================

        Private Sub StartHuff()
            For i As Integer = 0 To N_CHAR - 1
                freq(i) = 1US
                son(i) = CShort(i + T)
                prnt(i + T) = CShort(i)
            Next

            Dim i2 As Integer = 0
            Dim j As Integer = N_CHAR
            While j <= ROOT
                freq(j) = CUShort(freq(i2) + freq(i2 + 1))
                son(j) = CShort(i2)
                prnt(i2) = CShort(j)
                prnt(i2 + 1) = CShort(j)
                i2 += 2
                j += 1
            End While

            freq(T) = &HFFFFUS
            prnt(ROOT) = 0
        End Sub

        Private Sub Reconst()
            Dim j As Integer = 0
            For i As Integer = 0 To T - 1
                If son(i) >= T Then
                    freq(j) = CUShort((CInt(freq(i)) + 1) \ 2)
                    son(j) = son(i)
                    j += 1
                End If
            Next

            Dim i2 As Integer = 0
            Dim jj As Integer = N_CHAR
            While jj < T
                Dim k As Integer = i2 + 1
                Dim f As UShort = CUShort(CInt(freq(i2)) + CInt(freq(k)))

                Dim kk As Integer = jj - 1
                While f < freq(kk)
                    kk -= 1
                End While
                kk += 1

                Dim count As Integer = jj - kk
                If count > 0 Then
                    Array.Copy(freq, kk, freq, kk + 1, count)
                    Array.Copy(son, kk, son, kk + 1, count)
                End If

                freq(kk) = f
                son(kk) = CShort(i2)

                i2 += 2
                jj += 1
            End While

            For i As Integer = 0 To T - 1
                Dim k As Integer = son(i)
                If k >= T Then
                    prnt(k) = CShort(i)
                Else
                    prnt(k) = CShort(i)
                    prnt(k + 1) = CShort(i)
                End If
            Next
        End Sub

        Private Sub Update(c As Integer)
            If freq(ROOT) = MAX_FREQ Then Reconst()

            c = prnt(c + T)

            Do
                Dim k As Integer = CInt(freq(c)) + 1
                freq(c) = CUShort(k)

                Dim l As Integer = c + 1
                If k > freq(l) Then
                    Do
                        l += 1
                    Loop While k > freq(l)
                    l -= 1

                    freq(c) = freq(l)
                    freq(l) = CUShort(k)

                    Dim i As Integer = son(c)
                    son(c) = son(l)
                    son(l) = CShort(i)

                    prnt(i) = CShort(l)
                    If i < T Then prnt(i + 1) = CShort(l)

                    Dim j As Integer = son(c)
                    prnt(j) = CShort(c)
                    If j < T Then prnt(j + 1) = CShort(c)

                    c = l
                End If

                c = prnt(c)
            Loop While c <> 0
        End Sub

        Private Function DecodeCharValue() As Integer
            Dim c As Integer = son(ROOT)
            While c < T
                Dim b As Integer = GetBit()
                If b < 0 Then Return -1
                c = son(c + b)
            End While

            c -= T
            Update(c)
            Return c
        End Function

        ' ================= Position =================

        Private Function DecodePosition() As Integer
            Dim bit8 As Integer = GetByte()
            If bit8 < 0 Then Return -1

            Dim i As Integer = bit8 And &HFF
            Dim c As Integer = (CInt(d_code(i)) << 6)
            Dim j As Integer = CInt(d_len(i))

            j -= 2
            While j > 0
                Dim b As Integer = GetBit()
                If b < 0 Then Return -1
                i = (i << 1) Or b
                j -= 1
            End While

            Return c Or (i And &H3F)
        End Function

        ' ================= Main decode =================

        Private Function DecodeChunk(len As Integer) As Integer
            Dim count As Integer = 0

            While count < len
                If bufcnt = 0 Then
                    Dim c As Integer = DecodeCharValue()
                    If c < 0 Then Return count

                    If c < 256 Then
                        WriteByte(CByte(c))
                        count += 1
                    Else
                        Dim pos As Integer = DecodePosition()
                        If pos < 0 Then Return count

                        bufpos = (r - pos - 1) And (N - 1)
                        bufcnt = c - 255 + THRESHOLD
                        bufndx = 0
                    End If
                Else
                    While bufndx < bufcnt AndAlso count < len
                        Dim b As Byte = textBuf((bufpos + bufndx) And (N - 1))
                        WriteByte(b)
                        bufndx += 1
                        count += 1
                    End While

                    If bufndx >= bufcnt Then
                        bufndx = 0
                        bufcnt = 0
                    End If
                End If
            End While

            Return count
        End Function

        Private Sub WriteByte(b As Byte)
            output.WriteByte(b)
            textBuf(r) = b
            r = (r + 1) And (N - 1)
        End Sub

        ' ================= Tables (FULL 256 entries, from 86Box) =================

        Private Shared ReadOnly d_code() As Byte = {
            &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0,
            &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0,
            &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0,
            &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0,
            &H1, &H1, &H1, &H1, &H1, &H1, &H1, &H1,
            &H1, &H1, &H1, &H1, &H1, &H1, &H1, &H1,
            &H2, &H2, &H2, &H2, &H2, &H2, &H2, &H2,
            &H2, &H2, &H2, &H2, &H2, &H2, &H2, &H2,
            &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3,
            &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3,
            &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4,
            &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5,
            &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6,
            &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7,
            &H8, &H8, &H8, &H8, &H8, &H8, &H8, &H8,
            &H9, &H9, &H9, &H9, &H9, &H9, &H9, &H9,
            &HA, &HA, &HA, &HA, &HA, &HA, &HA, &HA,
            &HB, &HB, &HB, &HB, &HB, &HB, &HB, &HB,
            &HC, &HC, &HC, &HC, &HD, &HD, &HD, &HD,
            &HE, &HE, &HE, &HE, &HF, &HF, &HF, &HF,
            &H10, &H10, &H10, &H10, &H11, &H11, &H11, &H11,
            &H12, &H12, &H12, &H12, &H13, &H13, &H13, &H13,
            &H14, &H14, &H14, &H14, &H15, &H15, &H15, &H15,
            &H16, &H16, &H16, &H16, &H17, &H17, &H17, &H17,
            &H18, &H18, &H19, &H19, &H1A, &H1A, &H1B, &H1B,
            &H1C, &H1C, &H1D, &H1D, &H1E, &H1E, &H1F, &H1F,
            &H20, &H20, &H21, &H21, &H22, &H22, &H23, &H23,
            &H24, &H24, &H25, &H25, &H26, &H26, &H27, &H27,
            &H28, &H28, &H29, &H29, &H2A, &H2A, &H2B, &H2B,
            &H2C, &H2C, &H2D, &H2D, &H2E, &H2E, &H2F, &H2F,
            &H30, &H31, &H32, &H33, &H34, &H35, &H36, &H37,
            &H38, &H39, &H3A, &H3B, &H3C, &H3D, &H3E, &H3F
        }

        Private Shared ReadOnly d_len() As Byte = {
            &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3,
            &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3,
            &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3,
            &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3,
            &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4,
            &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4,
            &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4,
            &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4,
            &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4,
            &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4,
            &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5,
            &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5,
            &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5,
            &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5,
            &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5,
            &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5,
            &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5,
            &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5,
            &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6,
            &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6,
            &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6,
            &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6,
            &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6,
            &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6,
            &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7,
            &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7,
            &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7,
            &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7,
            &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7,
            &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7,
            &H8, &H8, &H8, &H8, &H8, &H8, &H8, &H8,
            &H8, &H8, &H8, &H8, &H8, &H8, &H8, &H8
        }

    End Class

End Namespace
