Imports System.IO

Namespace ImageFormats.TD0

    Friend NotInheritable Class TD0Lzhuf

        ' LZSS parameters
        Private Const F As Integer = 60
        Private Const N As Integer = 4096
        Private Const THRESHOLD As Integer = 2

        ' Huffman parameters
        Private Const MAX_FREQ As Integer = &H8000
        Private Const N_CHAR As Integer = (256 - THRESHOLD + F)
        Private Const R As Integer = (T - 1)
        Private Const T As Integer = (N_CHAR * 2 - 1)

        Private Shared ReadOnly d_code As Byte() = {
            &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0,
            &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0,
            &H1, &H1, &H1, &H1, &H1, &H1, &H1, &H1, &H1, &H1, &H1, &H1, &H1, &H1, &H1, &H1,
            &H2, &H2, &H2, &H2, &H2, &H2, &H2, &H2, &H2, &H2, &H2, &H2, &H2, &H2, &H2, &H2,
            &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3,
            &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5,
            &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7,
            &H8, &H8, &H8, &H8, &H8, &H8, &H8, &H8, &H9, &H9, &H9, &H9, &H9, &H9, &H9, &H9,
            &HA, &HA, &HA, &HA, &HA, &HA, &HA, &HA, &HB, &HB, &HB, &HB, &HB, &HB, &HB, &HB,
            &HC, &HC, &HC, &HC, &HD, &HD, &HD, &HD, &HE, &HE, &HE, &HE, &HF, &HF, &HF, &HF,
            &H10, &H10, &H10, &H10, &H11, &H11, &H11, &H11, &H12, &H12, &H12, &H12, &H13, &H13, &H13, &H13,
            &H14, &H14, &H14, &H14, &H15, &H15, &H15, &H15, &H16, &H16, &H16, &H16, &H17, &H17, &H17, &H17,
            &H18, &H18, &H19, &H19, &H1A, &H1A, &H1B, &H1B, &H1C, &H1C, &H1D, &H1D, &H1E, &H1E, &H1F, &H1F,
            &H20, &H20, &H21, &H21, &H22, &H22, &H23, &H23, &H24, &H24, &H25, &H25, &H26, &H26, &H27, &H27,
            &H28, &H28, &H29, &H29, &H2A, &H2A, &H2B, &H2B, &H2C, &H2C, &H2D, &H2D, &H2E, &H2E, &H2F, &H2F,
            &H30, &H31, &H32, &H33, &H34, &H35, &H36, &H37, &H38, &H39, &H3A, &H3B, &H3C, &H3D, &H3E, &H3F
        }

        Private Shared ReadOnly d_len As Byte() = {
            &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3,
            &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3, &H3,
            &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4,
            &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4,
            &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4, &H4,
            &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5,
            &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5,
            &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5,
            &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5, &H5,
            &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6,
            &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6,
            &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6, &H6,
            &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7,
            &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7,
            &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7, &H7,
            &H8, &H8, &H8, &H8, &H8, &H8, &H8, &H8, &H8, &H8, &H8, &H8, &H8, &H8, &H8, &H8
        }

        Public Shared Function DecompressToBuffer(src() As Byte, maxOut As Integer) As Byte()
            If src Is Nothing OrElse src.Length = 0 Then
                Return Array.Empty(Of Byte)()
            End If

            If maxOut <= 0 Then
                maxOut = TD0_MAX_BUFSZ
            End If

            Dim st As New State With {.src = src, .srcPos = 0, .inbuf = New Byte(511) {}}
            InitDecode(st)

            Using ms As New MemoryStream(Math.Min(maxOut, TD0_MAX_BUFSZ))
                Dim tmp(4095) As Byte

                While ms.Length < maxOut
                    Dim want As Integer = Math.Min(tmp.Length, maxOut - CInt(ms.Length))
                    Dim got As Integer = DecodeChunk(st, tmp, want)
                    If got <= 0 Then
                        Exit While
                    End If
                    ms.Write(tmp, 0, got)
                End While

                Return ms.ToArray()
            End Using
        End Function

        Private Shared Function DecodeChar(st As State) As Integer
            Dim c As Integer = st.son(R)
            While c < T
                Dim b As Integer = GetBit(st)
                If b < 0 Then
                    Return -1
                End If
                c = st.son(c + b)
            End While
            c -= T
            Update(st, c)

            Return c
        End Function

        Private Shared Function DecodeChunk(st As State, outBuf() As Byte, outLen As Integer) As Integer
            Dim count As Integer = 0

            While count < outLen
                If st.bufcnt = 0 Then
                    Dim c As Integer = DecodeChar(st)
                    If c < 0 Then
                        Exit While
                    End If

                    If c < 256 Then
                        outBuf(count) = CByte(c)
                        count += 1

                        st.text_buf(st.r) = CByte(c)
                        st.r = CUShort((st.r + 1) And (N - 1))
                    Else
                        Dim pos As Integer = DecodePosition(st)
                        If pos < 0 Then
                            Exit While
                        End If
                        st.bufpos = (st.r - pos - 1) And (N - 1)
                        st.bufcnt = c - 255 + THRESHOLD
                        st.bufndx = 0
                    End If
                Else
                    While st.bufndx < st.bufcnt AndAlso count < outLen
                        Dim c As Integer = st.text_buf((st.bufpos + st.bufndx) And (N - 1))
                        outBuf(count) = CByte(c)
                        count += 1

                        st.bufndx += 1
                        st.text_buf(st.r) = CByte(c)
                        st.r = CUShort((st.r + 1) And (N - 1))
                    End While
                    If st.bufndx >= st.bufcnt Then
                        st.bufndx = 0
                        st.bufcnt = 0
                    End If
                End If
            End While

            Return count
        End Function

        Private Shared Function DecodePosition(st As State) As Integer
            Dim b As Integer = GetByteBits(st)
            If b < 0 Then
                Return -1
            End If

            Dim i As Integer = b And &HFF
            Dim c As Integer = (d_code(i) << 6)
            Dim j As Integer = d_len(i)

            j -= 2
            While j > 0
                Dim bit As Integer = GetBit(st)
                If bit < 0 Then
                    Return -1
                End If
                i = ((i << 1) And &HFF) Or bit
                j -= 1
            End While

            Return c Or (i And &H3F)
        End Function

        Private Shared Function GetBit(st As State) As Integer
            If Not NextWord(st) Then
                Return -1
            End If

            Dim i As Short = CShort(st.getbuf)
            st.getbuf = CUShort((st.getbuf << 1) And &HFFFF)
            st.getlen = CByte(st.getlen - 1)

            Return If(i < 0, 1, 0)
        End Function

        Private Shared Function GetByteBits(st As State) As Integer
            If Not NextWord(st) Then
                Return -1
            End If

            Dim i As UShort = st.getbuf
            st.getbuf = CUShort((st.getbuf << 8) And &HFFFF)
            st.getlen = CByte(st.getlen - 8)
            i = CUShort(i >> 8)

            Return i
        End Function

        Private Shared Sub InitDecode(st As State)
            st.getbuf = 0
            st.getlen = 0
            st.ibufcnt = 0
            st.ibufndx = 0
            st.bufcnt = 0
            StartHuff(st)

            For i = 0 To (N - F - 1)
                st.text_buf(i) = AscW(" "c)
            Next

            st.r = CUShort(N - F)
        End Sub

        Private Shared Function NextWord(st As State) As Boolean
            While st.getlen <= 8
                Dim b As Integer = ReadByteBuffered(st)
                If b < 0 Then
                    Return False
                End If
                st.getbuf = CUShort(st.getbuf Or (CUShort(b) << (8 - st.getlen)))
                st.getlen = CByte(st.getlen + 8)
            End While

            Return True
        End Function

        Private Shared Function ReadByteBuffered(st As State) As Integer
            If st.ibufndx >= st.ibufcnt Then
                st.ibufndx = 0

                st.ibufcnt = Math.Min(st.inbuf.Length, st.src.Length - st.srcPos)

                If st.ibufcnt <= 0 Then
                    Return -1
                End If

                Array.Copy(st.src, st.srcPos, st.inbuf, 0, st.ibufcnt)
                st.srcPos += st.ibufcnt
            End If

            Dim b As Integer = st.inbuf(st.ibufndx)
            st.ibufndx += 1

            Return b
        End Function

        Private Shared Sub Reconst(st As State)
            Dim j As Integer = 0
            For i = 0 To T - 1
                If st.son(i) >= T Then
                    st.freq(j) = CUShort((st.freq(i) + 1) \ 2)
                    st.son(j) = st.son(i)
                    j += 1
                End If
            Next

            Dim ii As Integer = 0
            Dim jj As Integer = N_CHAR
            While jj < T
                Dim k As Integer = ii + 1
                Dim f As UShort = CUShort(st.freq(ii) + st.freq(k))

                Dim pos As Integer = jj - 1
                While f < st.freq(pos)
                    pos -= 1
                End While
                pos += 1

                ' shift right by 1 from pos..jj-1
                For m = jj To pos + 1 Step -1
                    st.freq(m) = st.freq(m - 1)
                    st.son(m) = st.son(m - 1)
                Next

                st.freq(pos) = f
                st.son(pos) = CShort(ii)

                jj += 1
                ii += 2
            End While

            For i = 0 To T - 1
                Dim k As Integer = st.son(i)
                If k >= T Then
                    st.prnt(k) = CShort(i)
                Else
                    st.prnt(k) = CShort(i)
                    st.prnt(k + 1) = CShort(i)
                End If
            Next
        End Sub

        Private Shared Sub StartHuff(st As State)
            For i = 0 To N_CHAR - 1
                st.freq(i) = 1
                st.son(i) = CShort(i + T)
                st.prnt(i + T) = CShort(i)
            Next

            Dim ii As Integer = 0
            Dim j As Integer = N_CHAR
            While j <= R
                st.freq(j) = CUShort(st.freq(ii) + st.freq(ii + 1))
                st.son(j) = CShort(ii)
                st.prnt(ii) = CShort(j)
                st.prnt(ii + 1) = CShort(j)
                ii += 2
                j += 1
            End While

            st.freq(T) = &HFFFFUS
            st.prnt(R) = 0
        End Sub

        Private Shared Sub Update(st As State, c As Integer)
            If st.freq(R) = MAX_FREQ Then
                Reconst(st)
            End If

            Dim cc As Integer = st.prnt(c + T)

            While cc <> 0
                st.freq(cc) = CUShort(st.freq(cc) + 1)
                Dim k As UShort = st.freq(cc)

                Dim l As Integer = cc + 1
                If l <= R AndAlso k > st.freq(l) Then
                    While l <= R AndAlso k > st.freq(l)
                        l += 1
                    End While
                    l -= 1

                    st.freq(cc) = st.freq(l)
                    st.freq(l) = k

                    Dim i As Integer = st.son(cc)
                    st.prnt(i) = CShort(l)
                    If i < T Then
                        st.prnt(i + 1) = CShort(l)
                    End If

                    Dim j As Integer = st.son(l)
                    st.son(l) = CShort(i)

                    st.prnt(j) = CShort(cc)
                    If j < T Then
                        st.prnt(j + 1) = CShort(cc)
                    End If
                    st.son(cc) = CShort(j)

                    cc = l
                End If

                cc = st.prnt(cc)
            End While
        End Sub

        Private NotInheritable Class State
            Public ReadOnly freq(T) As UShort
            Public ReadOnly prnt(T + N_CHAR - 1) As Short
            Public ReadOnly son(T - 1) As Short

            Public ReadOnly text_buf(N + F - 2) As Byte

            Public bufcnt As Integer
            Public bufndx As Integer
            Public bufpos As Integer
            Public getbuf As UShort
            Public getlen As Byte
            Public ibufcnt As Integer
            Public ibufndx As Integer
            Public inbuf() As Byte
            Public r As UShort
            Public src() As Byte
            Public srcPos As Integer
        End Class
    End Class

End Namespace
