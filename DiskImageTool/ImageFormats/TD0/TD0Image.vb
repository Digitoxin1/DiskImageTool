Imports DiskImageTool.Bitstream

Namespace ImageFormats.TD0
    Public Class TD0Image
        Inherits BitStreamImageBase
        Implements IBitstreamImage

        Private _Header As TD0Header
        Private _Comment As TD0Comment = Nothing

        Public Sub New()
            _Header = New TD0Header(Nothing)
            _Tracks = New List(Of TD0Track)()
            _TrackCount = 0
            _SideCount = 0
        End Sub

        Public ReadOnly Property Comment As TD0Comment
            Get
                Return _Comment
            End Get
        End Property

        Public ReadOnly Property Header As TD0Header
            Get
                Return _Header
            End Get
        End Property

        Public Overloads Property SideCount As Byte Implements IBitstreamImage.SideCount
        Public Overloads Property TrackCount As UShort Implements IBitstreamImage.TrackCount
        Public Property Tracks As List(Of TD0Track)

        Public Overrides Function Export(FilePath As String) As Boolean Implements IBitstreamImage.Export
            Return False ' not implemented
        End Function

        Public Overrides Function Load(FilePath As String) As Boolean Implements IBitstreamImage.Load
            Return Load(IO.File.ReadAllBytes(FilePath))
        End Function

        Public Overrides Function Load(fileBytes() As Byte) As Boolean Implements IBitstreamImage.Load
            Try
                If fileBytes Is Nothing OrElse fileBytes.Length < TD0Header.LENGTH Then
                    Return False
                End If

                _Header = New TD0Header(fileBytes)

                If Not _Header.IsValidSignature Then
                    Return False
                End If

                If Not _Header.CrcValid Then
                    Debug.Print("TD0: Header CRC invalid")
                End If

                Debug.Print($"TD0: Version {_Header.VersionMajor}.{_Header.VersionMinor}, Compressed={_Header.IsCompressed}, SidesField={_Header.SidesField}, SingleDensityGlobal={_Header.IsSingleDensityGlobal}, CommentBlock={_Header.HasCommentBlock}")

                Dim imageLen As Integer = fileBytes.Length - TD0Header.LENGTH

                If imageLen <= 0 Then
                    Return False
                End If

                Dim imagebuf = New Byte(imageLen - 1) {}
                Array.Copy(fileBytes, TD0Header.LENGTH, imagebuf, 0, imageLen)

                If _Header.IsCompressed Then
                    ' Compressed: decode the remainder as one block
                    imagebuf = DecodeCompressed(imagebuf)
                    If imagebuf Is Nothing OrElse imagebuf.Length = 0 Then
                        Return False
                    End If
                End If

                Dim ofs As Integer = 0

                If _Header.HasCommentBlock Then
                    If imagebuf.Length < TD0Comment.HEADER_LENGTH Then
                        Return False
                    End If

                    _Comment = New TD0Comment(imagebuf, 0)

                    If Not _Comment.CrcValid Then
                        Debug.Print("TD0: Comment CRC invalid")
                    End If

                    ofs = _Comment.TotalLength
                End If

                Return ParseImagebuf(imagebuf, ofs)

            Catch ex As Exception
                DebugException(ex)
                Return False
            End Try
        End Function

        Private Function DecodeCompressed(imagebuf() As Byte) As Byte()
            If _Header.VersionMajor >= 2 Then
                ' Teledisk 2.x => LZHUF
                Return TD0Lzhuf.Decompress(imagebuf, TD0_MAX_BUFSZ)
            Else
                ' Teledisk 1.x => LZW
                Return TD0Lzw.Decompress(imagebuf, TD0_MAX_BUFSZ)
            End If
        End Function

        Private Function ParseImagebuf(imagebuf() As Byte, ofs As Integer) As Boolean
            _Tracks = New List(Of TD0Track)()
            _TrackCount = 0
            _SideCount = 0

            While True
                If ofs >= imagebuf.Length Then
                    Exit While
                End If

                If ofs + TD0TrackHeader.LENGTH > imagebuf.Length Then
                    Return False
                End If

                Dim TrackHeader = New TD0TrackHeader(imagebuf, ofs)

                If Not TrackHeader.CrcValid Then
                    Debug.Print($"TD0: Track header CRC invalid: Cylinder {TrackHeader.PhysicalCylinder}, Head {TrackHeader.PhysicalHead}")
                End If

                ofs += TD0TrackHeader.LENGTH

                If TrackHeader.IsEndOfTracks Then
                    Exit While
                End If

                If TrackHeader.SectorCount <= 0 OrElse TrackHeader.SectorCount > 255 Then
                    Exit While
                End If

                Dim trk As New TD0Track(TrackHeader, _Header.IsSingleDensityGlobal)

                For i = 0 To TrackHeader.SectorCount - 1
                    If ofs + TD0SectorHeader.LENGTH > imagebuf.Length Then
                        Return False
                    End If

                    Dim SectorHeader = New TD0SectorHeader(imagebuf, ofs)
                    ofs += TD0SectorHeader.LENGTH

                    Dim sec As New TD0Sector(SectorHeader)

                    Dim size As Integer = sec.GetSizeBytes()
                    If size <= 0 Then
                        trk.AddSector(sec)
                        Continue For
                    End If

                    If Not SectorHeader.HasDataBlock Then
                        Dim FillData = New Byte(size - 1) {}
                        Dim fill As Byte = If(SectorHeader.IsDosSkipped, CByte(&HF6), CByte(&H0))
                        For b = 0 To FillData.Length - 1
                            FillData(b) = fill
                        Next
                        sec.SetData(Nothing, FillData)
                        trk.AddSector(sec)
                        Continue For
                    End If

                    If ofs + TD0SectorDataHeader.LENGTH > imagebuf.Length Then
                        Return False
                    End If

                    Dim SectorDataHeader = New TD0SectorDataHeader(imagebuf, ofs)
                    ofs += TD0SectorDataHeader.LENGTH

                    If SectorDataHeader.BlockSize <= 0 Then
                        Return False
                    End If

                    Dim Data = New Byte(size - 1) {}

                    Select Case SectorDataHeader.EncodingMethod
                        Case 0
                            If ofs + size > imagebuf.Length Then
                                Return False
                            End If
                            Array.Copy(imagebuf, ofs, Data, 0, size)
                            ofs += size

                        Case 1
                            ' Encoding 1: repeated 2-byte pattern, multiple entries possible.
                            ' Count is u16 BIG-endian and represents number of 16-bit WORDS.

                            Dim payloadBytes As Integer = SectorDataHeader.BlockSize - 1 ' blockSize includes the enc byte
                            Dim endOfPayload As Integer = ofs + payloadBytes
                            Dim pos As Integer = 0

                            While pos < size AndAlso (ofs + 4) <= endOfPayload AndAlso (ofs + 4) <= imagebuf.Length
                                ' u16 BE
                                Dim countWords As Integer = ReadUInt16LE(imagebuf, ofs)
                                Dim p0 As Byte = imagebuf(ofs + 2)
                                Dim p1 As Byte = imagebuf(ofs + 3)
                                ofs += 4

                                If countWords <= 0 Then
                                    Exit While
                                End If

                                Dim bytesToWrite As Integer = countWords * 2
                                If bytesToWrite > (size - pos) Then
                                    bytesToWrite = (size - pos)
                                End If

                                Dim j As Integer = 0
                                While j + 1 < bytesToWrite
                                    Data(pos) = p0
                                    Data(pos + 1) = p1
                                    pos += 2
                                    j += 2
                                End While

                                ' odd tail (shouldn't happen for normal sector sizes, but safe)
                                If j < bytesToWrite Then
                                    Data(pos) = p0
                                    pos += 1
                                End If
                            End While

                        Case 2
                            ' RLE: lenCode -> blockLen = 1 << lenCode  (86Box)
                            Dim k As Integer = 0
                            While k < size
                                If ofs + 2 > imagebuf.Length Then
                                    Return False
                                End If
                                Dim lenCode As Integer = imagebuf(ofs)
                                Dim rep As Integer = imagebuf(ofs + 1)
                                ofs += 2

                                If lenCode = 0 Then
                                    If ofs + rep > imagebuf.Length Then
                                        Return False
                                    End If
                                    Dim copyLen As Integer = Math.Min(rep, size - k)
                                    Array.Copy(imagebuf, ofs, Data, k, copyLen)
                                    ofs += rep
                                    k += copyLen
                                Else
                                    Dim blockLen As Integer = (1 << lenCode)
                                    If ofs + blockLen > imagebuf.Length Then
                                        Return False
                                    End If

                                    Dim total As Integer = blockLen * rep
                                    If total > (size - k) Then
                                        total = (size - k)
                                    End If

                                    Dim pattern(blockLen - 1) As Byte
                                    Array.Copy(imagebuf, ofs, pattern, 0, blockLen)
                                    ofs += blockLen

                                    Dim wrote As Integer = 0
                                    While wrote < total
                                        Dim n As Integer = Math.Min(blockLen, total - wrote)
                                        Array.Copy(pattern, 0, Data, k + wrote, n)
                                        wrote += n
                                    End While

                                    k += total
                                End If
                            End While

                        Case Else
                            Return False
                    End Select

                    sec.SetData(SectorDataHeader, Data)

                    If Not sec.CrcValid Then
                        Debug.Print($"TD0: Sector header CRC invalid: Cylinder {sec.Cylinder}, Head {sec.Head}, Sector Id {sec.SectorId}")
                    End If

                    trk.AddSector(sec)
                Next

                _Tracks.Add(trk)

                If trk.Cylinder + 1 > _TrackCount Then
                    _TrackCount = CUShort(trk.Cylinder + 1)
                End If

                If trk.Head + 1 > _SideCount Then
                    _SideCount = CByte(trk.Head + 1)
                End If
            End While

            Tracks = DirectCast(_Tracks, List(Of TD0Track))
            TrackCount = _TrackCount
            SideCount = If(_Header.SidesField = 1, CByte(1), CByte(Math.Max(1, _SideCount)))

            Return Tracks.Count > 0
        End Function
    End Class
End Namespace