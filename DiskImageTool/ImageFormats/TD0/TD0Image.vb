Imports DiskImageTool.Bitstream

Namespace ImageFormats.TD0
    Public Class TD0Image
        Inherits BitStreamImageBase
        Implements IBitstreamImage

        Private _Header As TD0Header
        Private _Comment As TD0Comment = Nothing
        Private _Tracks As List(Of TD0Track)

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

        Public ReadOnly Property Tracks As List(Of TD0Track)
            Get
                Return _Tracks
            End Get
        End Property

        Public Overrides Function Export(FilePath As String) As Boolean Implements IBitstreamImage.Export
            Try
                DeleteFileIfExists(FilePath)

                _Header.IsCompressed = False
                _Header.DosAllocationFlag = 0
                _Header.RefreshStoredCrc16()

                Using fs As New IO.FileStream(FilePath, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None)
                    fs.Write(_Header.GetBytes(), 0, TD0Header.LENGTH)

                    If _Comment IsNot Nothing Then
                        fs.Write(_Comment.GetBytes(), 0, _Comment.TotalLength)
                    End If

                    Dim TrackHeaderBytes = New Byte() {&HFF, 0, 0, 0}

                    For Each Track In _Tracks
                        TrackHeaderBytes = Track.GetHeaderBytes
                        fs.Write(TrackHeaderBytes, 0, TD0TrackHeader.LENGTH)

                        For Each Sector In Track.Sectors
                            Dim HasData = (Not Sector.Header.NoData AndAlso Sector.Header.GetSectorSizeBytes > 0)

                            If HasData Then
                                If Sector.Header.DosSkipped Then
                                    Sector.Header.DosSkipped = False
                                End If
                                Sector.RefreshStoredCrc16()
                            End If

                            fs.Write(Sector.Header.GetBytes, 0, TD0SectorHeader.LENGTH)

                            If HasData Then
                                Dim DataOut() As Byte

                                If BytePatternCanEncode(Sector.Data) Then
                                    Sector.DataHeader.EncodingMethod = TD0EncodingMethod.Repeat2BytePattern
                                    DataOut = BytePatternEncode(Sector.Data)
                                Else
                                    Dim r = RleEncode(Sector.Data)
                                    Dim UseRle As Boolean = r.Result AndAlso r.Data.Length <= (Sector.Data.Length - 2)

                                    If UseRle Then
                                        Sector.DataHeader.EncodingMethod = TD0EncodingMethod.Rle
                                        DataOut = r.Data
                                    Else
                                        Sector.DataHeader.EncodingMethod = TD0EncodingMethod.Raw
                                        DataOut = Sector.Data
                                    End If
                                End If

                                fs.Write(Sector.DataHeader.GetBytes(), 0, TD0SectorDataHeader.LENGTH)
                                fs.Write(DataOut, 0, DataOut.Length)
                            End If
                        Next
                    Next
                    TrackHeaderBytes(0) = &HFF ' End of tracks  
                    fs.Write(TrackHeaderBytes, 0, TD0TrackHeader.LENGTH)
                End Using
            Catch ex As Exception
                DebugException(ex)
                Return False
            End Try

            Return True
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

                Debug.Print($"TD0: Version {_Header.VersionMajor}.{_Header.VersionMinor}, Compressed={_Header.IsCompressed}" &
                            $", Sides={_Header.Sides}, IsSingleDensity={_Header.IsSingleDensity}, HasCommentBlock={_Header.HasCommentBlock}" &
                            $", Stepping={_Header.Stepping}, DataRate={_Header.DataRate}, DriveType={_Header.DriveType}" &
                            $", DosAllocFlag={_Header.DosAllocationFlag}, Sequence={_Header.Sequence}, CheckSequence={_Header.CheckSequence}")

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
            _Tracks = New List(Of TD0Track)
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

                'Debug.Print($"TD0: Track Header: Sector Count {TrackHeader.SectorCount}, Cylinder {TrackHeader.PhysicalCylinder}, HeadAndFlags {TrackHeader.HeadAndFlags}, CrcLow {TrackHeader.StoredCrcLow}")

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

                Dim trk As New TD0Track(TrackHeader, _Header.IsSingleDensity)

                For i = 0 To TrackHeader.SectorCount - 1
                    If ofs + TD0SectorHeader.LENGTH > imagebuf.Length Then
                        Return False
                    End If

                    Dim SectorHeader = New TD0SectorHeader(imagebuf, ofs)

                    'Debug.Print($"TD0: Sector Header: Cylinder {SectorHeader.Cylinder}, Head {SectorHeader.Head}, SectorId {SectorHeader.SectorId}" &
                    '$", SizeCode {SectorHeader.SizeCode}, Flags {SectorHeader.Flags}")

                    ofs += TD0SectorHeader.LENGTH

                    Dim sec As New TD0Sector(SectorHeader)

                    Dim size As Integer = sec.Header.GetSectorSizeBytes
                    If size <= 0 Then
                        trk.AddSector(sec)
                        Continue For
                    End If

                    If SectorHeader.IsDosSkipped OrElse SectorHeader.NoData Then
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

                    'Debug.Print($"TD0: Sector Data Header: EncodingMethod {SectorDataHeader.EncodingMethod}, BlockSize {SectorDataHeader.BlockSize}")

                    ofs += TD0SectorDataHeader.LENGTH

                    If SectorDataHeader.BlockSize <= 0 Then
                        Return False
                    End If

                    Dim Data = New Byte(size - 1) {}

                    Select Case SectorDataHeader.EncodingMethod
                        Case TD0EncodingMethod.Raw
                            If ofs + size > imagebuf.Length Then
                                Return False
                            End If
                            Array.Copy(imagebuf, ofs, Data, 0, size)
                            ofs += size

                        Case TD0EncodingMethod.Repeat2BytePattern
                            Dim payloadBytes As Integer = SectorDataHeader.BlockSize - 1
                            Dim r = BytePatternDecode(imagebuf, ofs, size, payloadBytes)

                            If Not r.Result OrElse r.BytesWritten <> size Then
                                Return False
                            End If

                            Data = r.Data
                            ofs += r.BytesConsumed

                        Case TD0EncodingMethod.Rle
                            Dim r = RleDecode(imagebuf, ofs, size)

                            If Not r.Result OrElse r.BytesWritten <> size Then
                                Return False
                            End If

                            Data = r.Data
                            ofs += r.BytesConsumed

                        Case Else
                            Return False
                    End Select

                    sec.SetData(SectorDataHeader, Data)

                    If Not sec.CrcValid Then
                        Debug.Print($"TD0: Sector header CRC invalid: Cylinder {sec.Header.Cylinder}, Head {sec.Header.Head}, Sector Id {sec.Header.SectorId}")
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

            TrackCount = _TrackCount
            SideCount = If(_Header.Sides = 1, CByte(1), CByte(Math.Max(1, _SideCount)))

            Return _Tracks.Count > 0
        End Function
    End Class
End Namespace