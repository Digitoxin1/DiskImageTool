Imports DiskImageTool.Bitstream

Namespace ImageFormats
    Namespace _86F
        Public Class _86FImage
            Implements IBitstreamImage

            Private Const FILE_SIGNATURE = "86BF"
            Private _BitRate As BitRate = 255
            Private _DiskFlags As DiskFlags
            Private _MajorVersion As Byte
            Private _MinorVersion As Byte
            Private _RPM As RPM = 255
            Private _TrackCount As UShort
            Private _Tracks() As _86FTrack
            Public Sub New()
                _MajorVersion = 2
                _MinorVersion = 12
                _DiskFlags = 0
                _TrackCount = 0
            End Sub

            Public Sub New(TrackCount As UShort, SideCount As Byte)
                _MajorVersion = 2
                _MinorVersion = 12
                _DiskFlags = 0
                _TrackCount = TrackCount
                Sides = SideCount

                _Tracks = New _86FTrack(TrackCount * SideCount - 1) {}
            End Sub

            Public Property AlternateBitcellCalculation As Boolean
                Get
                    Return (_DiskFlags And DiskFlags.AlternateBitcellCalculation) > 0
                End Get
                Set(value As Boolean)
                    _DiskFlags = MyBitConverter.ToggleBit(_DiskFlags, DiskFlags.AlternateBitcellCalculation, value)
                End Set
            End Property

            Public Property BitcellMode As Boolean
                Get
                    Return (_DiskFlags And DiskFlags.BitcellMode) > 0
                End Get
                Set(value As Boolean)
                    _DiskFlags = MyBitConverter.ToggleBit(_DiskFlags, DiskFlags.BitcellMode, value)
                End Set
            End Property

            Public ReadOnly Property BitRate As BitRate
                Get
                    Return _BitRate
                End Get
            End Property

            Public Property DiskFlags As DiskFlags
                Get
                    Return _DiskFlags
                End Get
                Set(value As DiskFlags)
                    _DiskFlags = value
                End Set
            End Property

            Public Property DiskType As DiskType
                Get
                    Dim value = (_DiskFlags And DiskFlags.DiskType) > 0
                    Return If(value, DiskType.Zoned, DiskType.FixedRPM)
                End Get
                Set(value As DiskType)
                    _DiskFlags = MyBitConverter.ToggleBit(_DiskFlags, DiskFlags.DiskType, (value = DiskType.Zoned))
                End Set
            End Property

            Public Property HasSurfaceData As Boolean Implements IBitstreamImage.HasSurfaceData
                Get
                    Return (_DiskFlags And DiskFlags.HasSurfaceData) > 0
                End Get
                Set(value As Boolean)
                    _DiskFlags = MyBitConverter.ToggleBit(_DiskFlags, DiskFlags.HasSurfaceData, value)
                End Set
            End Property

            Public Property Hole As DiskHole
                Get
                    Return (_DiskFlags >> 1) And &H3

                End Get
                Set(value As DiskHole)
                    value = value And &H3
                    _DiskFlags = (_DiskFlags And Not (&H6)) Or (value << 1)
                End Set
            End Property

            Public Property MajorVersion As Byte
                Get
                    Return _MajorVersion
                End Get
                Set(value As Byte)
                    _MajorVersion = value
                End Set
            End Property

            Public Property MinorVersion As Byte
                Get
                    Return _MinorVersion
                End Get
                Set(value As Byte)
                    _MinorVersion = value
                End Set
            End Property

            Public Property ReverseEndian As Boolean
                Get
                    Return (_DiskFlags And DiskFlags.ReverseEndian) > 0
                End Get
                Set(value As Boolean)
                    _DiskFlags = MyBitConverter.ToggleBit(_DiskFlags, DiskFlags.ReverseEndian, value)
                End Set
            End Property

            Public ReadOnly Property RPM As RPM
                Get
                    Return _RPM
                End Get
            End Property

            Public Property RPMSlowDown As Single
                Get
                    Dim b As Byte = (_DiskFlags >> 5) And &H3
                    If b = 3 Then
                        Return 0.02
                    ElseIf b = 2 Then
                        Return 0.015
                    ElseIf b = 1 Then
                        Return 0.01
                    Else
                        Return 0
                    End If
                End Get
                Set(value As Single)
                    Dim b As Byte
                    If value >= CSng(0.02) Then
                        b = 3
                    ElseIf value > CSng(0.01) Then
                        b = 2
                    ElseIf value > 0 Then
                        b = 1
                    Else
                        b = 0
                    End If
                    b = b And &H3
                    _DiskFlags = (_DiskFlags And Not (&H30)) Or (b << 5)
                End Set
            End Property

            Public Property Sides As Byte Implements IBitstreamImage.SideCount
                Get
                    If (_DiskFlags And DiskFlags.Sides) > 0 Then
                        Return 2
                    Else
                        Return 1
                    End If
                End Get
                Set(value As Byte)
                    _DiskFlags = MyBitConverter.ToggleBit(_DiskFlags, DiskFlags.Sides, value > 1)
                End Set
            End Property

            Public ReadOnly Property TrackCount As UShort Implements IBitstreamImage.TrackCount
                Get
                    Return _TrackCount
                End Get
            End Property

            Public Property WriteProtect As Boolean
                Get
                    Return (_DiskFlags And DiskFlags.WriteProtect) > 0
                End Get
                Set(value As Boolean)
                    _DiskFlags = MyBitConverter.ToggleBit(_DiskFlags, DiskFlags.WriteProtect, value)
                End Set
            End Property

            Public Property ZoneType As ZoneType
                Get
                    Return (_DiskFlags >> 9) And &H3

                End Get
                Set(value As ZoneType)
                    value = value And &H3
                    _DiskFlags = (_DiskFlags And Not (&H600)) Or (CUShort(value) << 9)
                End Set
            End Property

            Private ReadOnly Property IBitstreamImage_TrackStep As Byte Implements IBitstreamImage.TrackStep
                Get
                    Return 1
                End Get
            End Property
            Public Function Export(FilePath As String) As Boolean
                Dim Buffer() As Byte
                Dim Pos As UInteger
                Dim DataPosition As UInteger
                Dim Track As _86FTrack
                Dim BitCellCount As UInteger
                Dim AllocatedLength As UInteger

                Dim TrackArray = GetTrackArray()

                Try
                    If IO.File.Exists(FilePath) Then
                        IO.File.Delete(FilePath)
                    End If

                    Using fs As IO.FileStream = IO.File.OpenWrite(FilePath)
                        Buffer = Text.Encoding.UTF8.GetBytes(FILE_SIGNATURE)
                        fs.Write(Buffer, 0, Buffer.Length)

                        fs.WriteByte(_MinorVersion)
                        fs.WriteByte(_MajorVersion)

                        Buffer = BitConverter.GetBytes(_DiskFlags)
                        fs.Write(Buffer, 0, Buffer.Length)
                        Pos = fs.Position
                        DataPosition = 256 * Sides * 4 + Pos

                        For Each Track In TrackArray
                            fs.Position = Pos

                            If Track Is Nothing Then
                                Buffer = New Byte(3) {}
                            ElseIf Track.Bitstream.Length = 0 Then
                                Buffer = New Byte(3) {}
                            Else
                                Buffer = BitConverter.GetBytes(DataPosition)
                            End If

                            fs.Write(Buffer, 0, Buffer.Length)

                            Pos += 4

                            If Track IsNot Nothing AndAlso Track.Bitstream.Length > 0 Then
                                fs.Position = DataPosition

                                Buffer = BitConverter.GetBytes(Track.Flags)
                                fs.Write(Buffer, 0, Buffer.Length)

                                If BitcellMode Then
                                    Buffer = BitConverter.GetBytes(Track.BitCellCount)
                                    fs.Write(Buffer, 0, Buffer.Length)
                                End If

                                Buffer = BitConverter.GetBytes(Track.IndexHolePos)
                                fs.Write(Buffer, 0, Buffer.Length)

                                If BitcellMode And AlternateBitcellCalculation Then
                                    AllocatedLength = Math.Ceiling(Track.Bitstream.Length / 8)
                                    BitCellCount = AllocatedLength * 8
                                Else
                                    Dim IsMFM = Track.Encoding = Encoding.MFM
                                    BitCellCount = GetCalculatedBitCellCount(Track.BitRate, Track.RPM, IsMFM, RPMSlowDown, AlternateBitcellCalculation, Track.BitCellCount)
                                    AllocatedLength = GetAllocatedLength(Hole, RPMSlowDown, AlternateBitcellCalculation, Track.BitCellCount)
                                End If

                                If BitCellCount > 0 Then
                                    Buffer = IBM_MFM.BitsToBytes(IBM_MFM.ResizeBitstream(Track.Bitstream, BitCellCount), 0)
                                    fs.Write(Buffer, 0, Buffer.Length)

                                    If AllocatedLength > Buffer.Length Then
                                        Buffer = New Byte(AllocatedLength - Buffer.Length - 1) {}
                                        fs.Write(Buffer, 0, Buffer.Length)
                                    End If

                                    If HasSurfaceData Then
                                        Buffer = IBM_MFM.BitsToBytes(IBM_MFM.ResizeBitstream(Track.SurfaceData, BitCellCount), 0)
                                        fs.Write(Buffer, 0, Buffer.Length)

                                        If AllocatedLength > Buffer.Length Then
                                            Buffer = New Byte(AllocatedLength - Buffer.Length - 1) {}
                                            fs.Write(Buffer, 0, Buffer.Length)
                                        End If
                                    End If
                                End If

                                DataPosition = fs.Position
                            End If
                        Next
                    End Using
                Catch ex As Exception
                    Return False
                End Try

                Return True
            End Function

            Public Function GetTrack(Track As UShort, Side As Byte) As _86FTrack
                If Track > _TrackCount - 1 Or Side > Sides - 1 Then
                    Throw New System.IndexOutOfRangeException
                End If

                Dim Index = Track * Sides + Side

                Return _Tracks(Index)
            End Function

            Public Function Load(FilePath As String) As Boolean
                Return Load(IO.File.ReadAllBytes(FilePath))
            End Function

            Public Function Load(Buffer() As Byte) As Boolean
                Dim TrackList As New List(Of _86FTrack)
                Dim Result As Boolean
                Dim DoubleStep As Boolean = False

                Dim Signature = Text.Encoding.UTF8.GetString(Buffer, 0, 4)
                Result = (Signature = FILE_SIGNATURE)

                If Result Then
                    _MinorVersion = Buffer(4)
                    _MajorVersion = Buffer(5)
                    _DiskFlags = BitConverter.ToUInt16(Buffer, 6)
                    _TrackCount = 0

                    Dim Data() As Byte
                    Dim Checksum As UInteger
                    Dim BitCellCount As UInteger
                    Dim AllocatedLength As UInteger
                    Dim Offset As UInteger

                    Dim Pos = 8
                    For i = 0 To 255
                        For j = 0 To Sides - 1
                            Offset = BitConverter.ToUInt32(Buffer, Pos)
                            Dim F86Track = New _86FTrack(i, j, Offset)
                            If Offset > 0 Then
                                F86Track.Flags = BitConverter.ToUInt16(Buffer, Offset)
                                If BitcellMode Then
                                    F86Track.BitCellCount = BitConverter.ToUInt32(Buffer, Offset + 2)
                                    If AlternateBitcellCalculation Then
                                        BitCellCount = F86Track.BitCellCount
                                        AllocatedLength = Math.Ceiling(F86Track.BitCellCount / 8)
                                    End If
                                    Offset += 4
                                End If
                                F86Track.IndexHolePos = BitConverter.ToUInt32(Buffer, Offset + 2)

                                If Not BitcellMode Or Not AlternateBitcellCalculation Then
                                    Dim IsMFM = F86Track.Encoding = Encoding.MFM
                                    BitCellCount = GetCalculatedBitCellCount(F86Track.BitRate, F86Track.RPM, IsMFM, RPMSlowDown, AlternateBitcellCalculation, F86Track.BitCellCount)
                                    AllocatedLength = GetAllocatedLength(Hole, RPMSlowDown, AlternateBitcellCalculation, F86Track.BitCellCount)
                                End If
                                F86Track.Bitstream = IBM_MFM.BytesToBits(Buffer, Offset + 6, BitCellCount)

                                If HasSurfaceData Then
                                    F86Track.SurfaceData = IBM_MFM.BytesToBits(Buffer, Offset + 6 + AllocatedLength, BitCellCount)
                                End If

                                'Check for thick tracks
                                If j = 0 Then
                                    If i < 2 Then
                                        Data = New Byte(BitCellCount \ 8 - 1) {}
                                        Array.Copy(Buffer, Offset + 6, Data, 0, Data.Length)
                                        If i = 0 Then
                                            Checksum = CRC32.ComputeChecksum(Data)
                                        Else
                                            DoubleStep = (Checksum = CRC32.ComputeChecksum(Data))
                                        End If
                                    End If
                                End If

                                _TrackCount = i + 1
                            End If
                            TrackList.Add(F86Track)
                            Pos += 4
                        Next
                    Next

                    If DoubleStep Then
                        _TrackCount \= 2
                    End If

                    _Tracks = New _86FTrack((_TrackCount) * Sides - 1) {}

                    Dim Index As UShort = 0
                    Dim Track As UShort
                    For Each F86Track In TrackList
                        If Not DoubleStep OrElse (Index \ Sides) Mod 2 = 0 Then
                            Track = F86Track.Track
                            If DoubleStep Then
                                Track \= 2
                            End If

                            If Track < _TrackCount Then
                                If F86Track.Encoding = Encoding.MFM Then
                                    F86Track.MFMData = New IBM_MFM.IBM_MFM_Track(F86Track.Bitstream)
                                    F86Track.Decoded = True
                                    If Index = 0 Then
                                        _BitRate = F86Track.BitRate
                                        _RPM = F86Track.RPM
                                    Else
                                        If _BitRate <> 255 AndAlso _BitRate <> F86Track.BitRate Then
                                            _BitRate = 255
                                        End If
                                        If _RPM <> 255 AndAlso _RPM <> F86Track.RPM Then
                                            _RPM = 255
                                        End If
                                    End If
                                End If
                                SetTrack(Track, F86Track.Side, F86Track)
                                'Dim FileName = "H:\debug\track" & F86Track.Track & "." & F86Track.Side & ".log"
                                'DebugExportMFMBitstream(F86Track.Bitstream, FileName)
                            End If
                        End If
                        Index += 1
                    Next
                End If

                Return Result
            End Function
            Public Sub SetTrack(Track As UShort, Side As Byte, Value As _86FTrack)
                If Track > _TrackCount - 1 Or Side > Sides - 1 Then
                    Throw New System.IndexOutOfRangeException
                End If

                Dim Index = Track * Sides + Side

                _Tracks(Index) = Value
            End Sub

            Private Function AdjustRPM(Length As UInteger, RPMSlowDown As Single, SpeedUp As Boolean) As UInteger
                If SpeedUp Then
                    Return Math.Truncate(Length / (1 + RPMSlowDown))
                Else
                    Return Math.Truncate(Length * (1 + RPMSlowDown))
                End If
            End Function

            Private Function GetAllocatedLength(Hole As DiskHole, RPMSlowDown As Single, SpeedUp As Boolean, ExtraBitCellCount As UInteger) As UInteger
                Dim Length As UInteger

                Select Case Hole
                    Case DiskHole.DD, DiskHole.HD
                        Length = 12500
                    Case DiskHole.ED
                        Length = 25000
                    Case DiskHole.ED2000
                        Length = 50000
                    Case Else
                        Length = 12500
                End Select

                Length = AdjustRPM(Length, RPMSlowDown, SpeedUp)

                Length <<= 4

                Length += MyBitConverter.ToInt32(ExtraBitCellCount)

                Return Length \ 8
            End Function

            Private Function GetTrackArray() As _86FTrack()
                Dim TrackArray() As _86FTrack
                Dim TrackCount = _TrackCount
                Dim TrackStep As UShort = 1

                If TrackCount < 45 Then
                    TrackStep = 2
                    TrackCount *= 2
                End If

                TrackArray = New _86FTrack(TrackCount * Sides - 1) {}

                Dim Index As UShort = 0
                For i = 0 To _TrackCount - 1
                    For j = 1 To TrackStep
                        For k = 0 To Sides - 1
                            TrackArray(Index) = GetTrack(i, k)
                            Index += 1
                        Next
                    Next
                Next

                Return TrackArray
            End Function

            Private Function GetCalculatedBitCellCount(BitRate As BitRate, RPM As RPM, IsMFM As Boolean, RPMSlowDown As Single, SpeedUp As Boolean, ExtraBitCellCount As UInteger) As UInteger
                Dim Length As Single = CalculatetBitCount(GetBitRate(BitRate, IsMFM), GetRPM(RPM))

                Length = AdjustRPM(Length, RPMSlowDown, SpeedUp)

                Length = (Length \ 16) * 16

                Length += MyBitConverter.ToInt32(ExtraBitCellCount)

                Return Length
            End Function

            Private Function IBitstreamImage_GetTrack(Track As UShort, Side As Byte) As IBitstreamTrack Implements IBitstreamImage.GetTrack
                Return GetTrack(Track, Side)
            End Function
        End Class
    End Namespace
End Namespace

