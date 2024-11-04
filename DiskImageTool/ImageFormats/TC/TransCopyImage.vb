Imports DiskImageTool.Bitstream

Namespace ImageFormats
    Namespace TC
        Public Class TransCopyImage
            Inherits BitStreamImageBase
            Implements IBitstreamImage

            Private Const EMPTY_FLAG As Byte = &H44
            Private Const EMPTY_SIZE As Byte = &H33
            Private Const EMPTY_SKU As Byte = &H11
            Private _Comment() As Byte
            Private _Comment2() As Byte
            Private _Sides As Byte
            Private _TrackEnd As Byte
            Private _TrackIncrement As Byte
            Private _Tracks() As TransCopyTrack
            Private _TrackStart As Byte

            Private Enum TransCopyOffsets
                Signature = &H0
                Comment = &H2
                Comment2 = &H22
                DiskType = &H100
                TrackStart = &H101
                TrackEnd = &H102
                Sides = &H103
                TrackIncrement = &H104
                Skews = &H105
                Offsets = &H305
                Lengths = &H505
                Flags = &H705
                Timings = &H905
                Data = &H4000
            End Enum

            Private Enum TransCopySizes
                Signature = 2
                Comment = 32
                Comment2 = 32
            End Enum

            Public Sub New()
                _Comment = New Byte(31) {}
                _Comment2 = New Byte(31) {}
                _DiskType = TransCopyDiskType.Unknown
                _TrackStart = 0
                _TrackEnd = 0
                _Sides = 0
                _TrackIncrement = 1
            End Sub

            Public Sub New(Tracks As Byte, Sides As Byte, TrackIncrement As Byte)
                _Comment = New Byte(31) {}
                _Comment2 = New Byte(31) {}
                _DiskType = TransCopyDiskType.Unknown
                _TrackStart = 0
                _TrackEnd = Tracks - 1
                _Sides = Sides
                _TrackIncrement = TrackIncrement

                _Tracks = New TransCopyTrack((_TrackEnd + 1) * Sides - 1) {}
            End Sub

            Public Property Comment As String
                Get
                    Return Text.Encoding.UTF8.GetString(_Comment).TrimEnd(vbNullChar)
                End Get
                Set(value As String)
                    value = Left(value, 32).PadRight(32, vbNullChar)
                    _Comment = System.Text.Encoding.UTF8.GetBytes(value)
                End Set
            End Property

            Public Property Comment2 As String
                Get
                    Return Text.Encoding.UTF8.GetString(_Comment2).TrimEnd(vbNullChar)
                End Get
                Set(value As String)
                    value = Left(value, 32).PadRight(32, vbNullChar)
                    _Comment2 = System.Text.Encoding.UTF8.GetBytes(value)
                End Set
            End Property

            Public Property DiskType As TransCopyDiskType

            Public Overloads ReadOnly Property SideCount As Byte Implements IBitstreamImage.SideCount
                Get
                    Return _Sides
                End Get
            End Property

            Public Overloads ReadOnly Property TrackCount As UShort Implements IBitstreamImage.TrackCount
                Get
                    Return _TrackEnd + 1
                End Get
            End Property

            Public ReadOnly Property TrackEnd As Byte
                Get
                    Return _TrackEnd
                End Get
            End Property

            Public ReadOnly Property TrackIncrement As Byte
                Get
                    Return _TrackIncrement
                End Get
            End Property

            Public ReadOnly Property TrackStart As Byte
                Get
                    Return _TrackStart
                End Get
            End Property

            Public Overrides Function Export(FilePath As String) As Boolean Implements IBitstreamImage.Export
                Dim buffer() As Byte
                Dim OffsetList As New List(Of UShort)
                Dim Index As Integer
                Dim TransCopyTrack As TransCopyTrack

                Try
                    If IO.File.Exists(FilePath) Then
                        IO.File.Delete(FilePath)
                    End If

                    Using fs As IO.FileStream = IO.File.OpenWrite(FilePath)
                        '0x0000     byte[2]     Magic Number
                        fs.WriteByte(&H5A)
                        fs.WriteByte(&HA5)

                        '0x0002     char[32]    First comment line
                        fs.Write(_Comment, 0, 32)

                        '0x0022     char[32]    Second comment line
                        fs.Write(_Comment2, 0, 32)

                        '0x042      char[190]   Unused space
                        buffer = New Byte(189) {}
                        fs.Write(buffer, 0, buffer.Length)

                        '0x100      byte        Disk Type
                        fs.WriteByte(_DiskType)

                        '0x101      byte        Starting track number
                        fs.WriteByte(_TrackStart)

                        '0x102      byte        Ending track number
                        fs.WriteByte(_TrackEnd)

                        '0x103      byte        Number of sides
                        fs.WriteByte(_Sides)

                        '0x104      byte        Track increment between tracks
                        fs.WriteByte(_TrackIncrement)

                        buffer = New Byte(511) {}

                        '0x105      word[256]   Track skew (One entry per track)                
                        FillArray(buffer, EMPTY_SKU)
                        Index = 0
                        For Track As Byte = 0 To _TrackEnd
                            For Side As Byte = 0 To 1
                                If Side < _Sides Then
                                    TransCopyTrack = GetTrack(Track, Side)
                                    If TransCopyTrack IsNot Nothing Then
                                        BitConverter.GetBytes(TransCopyTrack.Skew).CopyTo(buffer, Index)
                                    End If
                                End If
                                Index += 2
                            Next
                        Next
                        fs.Write(buffer, 0, buffer.Length)

                        '0x305      word[256]   Start of track data (One entry per track)                
                        FillArray(buffer, &H0)
                        fs.Write(buffer, 0, buffer.Length)

                        '0x505      word[256]   Size of track data (One entry per track)
                        FillArray(buffer, EMPTY_SIZE)
                        Index = 0
                        For Track As Byte = 0 To _TrackEnd
                            For Side As Byte = 0 To 1
                                If Side < _Sides Then
                                    TransCopyTrack = GetTrack(Track, Side)
                                    If TransCopyTrack IsNot Nothing Then
                                        BitConverter.GetBytes(TransCopyTrack.Length).CopyTo(buffer, Index)
                                    End If
                                End If
                                Index += 2
                            Next
                        Next
                        fs.Write(buffer, 0, buffer.Length)

                        '0x705      word[256]   Track flags (One entry per track)
                        FillArray(buffer, EMPTY_FLAG)
                        Index = 0
                        For Track As Byte = 0 To _TrackEnd
                            For Side As Byte = 0 To 1
                                If Side < _Sides Then
                                    TransCopyTrack = GetTrack(Track, Side)
                                    If TransCopyTrack IsNot Nothing Then
                                        Dim TrackType = TransCopyTrack.TrackType
                                        If TransCopyTrack.NoAddressMarks Then
                                            TrackType = TrackType Or &H80
                                        End If
                                        buffer(Index) = TransCopyTrack.Flags
                                        buffer(Index + 1) = TrackType
                                    End If
                                End If
                                Index += 2
                            Next
                        Next
                        fs.Write(buffer, 0, buffer.Length)

                        '0x905      word[4096]  Address mark timing (16 entries per track)
                        buffer = New Byte(8191) {}
                        FillArray(buffer, &H0)
                        Index = 0
                        For Track As Byte = 0 To _TrackEnd
                            For Side As Byte = 0 To 1
                                If Side < _Sides Then
                                    TransCopyTrack = GetTrack(Track, Side)
                                    If TransCopyTrack IsNot Nothing Then
                                        Dim Length = TransCopyTrack.AddressMarkingTiming.Count
                                        Dim Entry As UShort
                                        For i = 0 To 15
                                            If i < Length Then
                                                Entry = TransCopyTrack.AddressMarkingTiming(i)
                                            Else
                                                Entry = 0
                                            End If
                                            BitConverter.GetBytes(Entry).CopyTo(buffer, Index + i * 2)
                                        Next
                                    End If
                                End If
                                Index += 32
                            Next
                        Next
                        fs.Write(buffer, 0, buffer.Length)

                        '0x2905     byte[5883]  Unused space
                        buffer = New Byte(5882) {}
                        fs.Write(buffer, 0, buffer.Length)

                        '0x4000     Start of track data
                        For Track As Byte = 0 To _TrackEnd
                            For Side As Byte = 0 To 1
                                Dim TrackOffset As UShort = 0
                                If Side < _Sides Then
                                    TransCopyTrack = GetTrack(Track, Side)
                                    If TransCopyTrack IsNot Nothing Then
                                        Dim BitLength = Math.Ceiling(TransCopyTrack.Bitstream.Length / 2048) * 2048
                                        'Dim Padding = BitLength - TransCopyTrack.Bitstream.Length
                                        'buffer = IBM_MFM.BitsToBytes(TransCopyTrack.Bitstream, Padding)
                                        buffer = IBM_MFM.BitsToBytes(IBM_MFM.ResizeBitstream(TransCopyTrack.Bitstream, BitLength), 0)
                                        Dim Offset = fs.Position
                                        Dim NextBoundary = Math.Ceiling(Offset / 65536) * 65536
                                        If Offset + buffer.Length > NextBoundary Then
                                            For Counter = 0 To NextBoundary - Offset - 1
                                                fs.WriteByte(&H0)
                                            Next
                                            Offset = fs.Position
                                        End If
                                        fs.Write(buffer, 0, buffer.Length)
                                        TrackOffset = Offset \ 256
                                    End If
                                End If
                                OffsetList.Add(TrackOffset)
                            Next
                        Next

                        fs.Seek(&H305, IO.SeekOrigin.Begin)
                        For Each Offset As UShort In OffsetList
                            Offset = MyBitConverter.SwapEndian(Offset)
                            fs.Write(BitConverter.GetBytes(Offset), 0, 2)
                        Next
                    End Using
                Catch ex As Exception
                    DebugException(ex)
                    Return False
                End Try

                Return True
            End Function

            Public Function GetTrack(Track As Byte, Side As Byte) As TransCopyTrack
                If Track > _TrackEnd Or Side > _Sides - 1 Then
                    Throw New System.IndexOutOfRangeException
                End If

                Dim Index = Track * _Sides + Side

                Return _Tracks(Index)
            End Function

            Public Overrides Function Load(FilePath As String) As Boolean Implements IBitstreamImage.Load
                Return Load(IO.File.ReadAllBytes(FilePath))
            End Function

            Public Overrides Function Load(Buffer() As Byte) As Boolean Implements IBitstreamImage.Load
                Dim Result As Boolean

                If Buffer.Length < 2 Then
                    Return False
                End If

                Dim Signature = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Signature)
                Result = (Signature = &HA55A)

                If Result Then
                    Result = Buffer.Length > TransCopyOffsets.Data
                End If

                If Result Then
                    Try
                        Array.Copy(Buffer, TransCopyOffsets.Comment, _Comment, 0, TransCopySizes.Comment)
                        Array.Copy(Buffer, TransCopyOffsets.Comment2, _Comment2, 0, TransCopySizes.Comment2)

                        _DiskType = Buffer(TransCopyOffsets.DiskType)
                        _TrackStart = Buffer(TransCopyOffsets.TrackStart)
                        _TrackEnd = Buffer(TransCopyOffsets.TrackEnd)
                        _Sides = Buffer(TransCopyOffsets.Sides)
                        _TrackIncrement = Buffer(TransCopyOffsets.TrackIncrement)

                        _Tracks = New TransCopyTrack((_TrackEnd + 1) * SideCount - 1) {}

                        For Track = 0 To _TrackEnd
                            For Side = 0 To _Sides - 1
                                Dim TransCopyTrack = New TransCopyTrack(Track, Side)

                                Dim Offset = 4 * Track + 2 * Side

                                Dim OffsetValue = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Offsets + Offset)
                                If OffsetValue > 0 Then
                                    TransCopyTrack.Offset = MyBitConverter.SwapEndian(OffsetValue) * 256
                                    TransCopyTrack.Skew = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Skews + Offset)
                                    TransCopyTrack.Length = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Lengths + Offset)
                                    TransCopyTrack.Flags = Buffer(TransCopyOffsets.Flags + Offset)
                                    TransCopyTrack.TrackType = Buffer(TransCopyOffsets.Flags + Offset + 1) And &H7F

                                    For i = 0 To 15
                                        Dim TimingOffset = 64 * Track + 32 * Side + 2 * i
                                        Dim Value = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Timings + TimingOffset)
                                        TransCopyTrack.AddressMarkingTiming.Add(Value)
                                    Next

                                    If TransCopyTrack.Offset >= TransCopyOffsets.Data Then
                                        TransCopyTrack.Bitstream = IBM_MFM.BytesToBits(Buffer, TransCopyTrack.Offset, TransCopyTrack.Length * 8)

                                        If TransCopyTrack.IsMFMTrackType Then
                                            TransCopyTrack.MFMData = New IBM_MFM.IBM_MFM_Track(TransCopyTrack.Bitstream)
                                            'Bitstream.DebugTranscopyCylinder(Cylinder)
                                        End If
                                    End If
                                End If
                                SetTrack(Track, Side, TransCopyTrack)
                            Next
                        Next
                    Catch ex As Exception
                        DebugException(ex)
                        Return False
                    End Try
                End If

                Return Result
            End Function

            Public Sub SetTrack(Track As Byte, Side As Byte, Value As TransCopyTrack)
                If Track > _TrackEnd Or Side > _Sides - 1 Then
                    Throw New System.IndexOutOfRangeException
                End If

                Dim Index = Track * _Sides + Side

                _Tracks(Index) = Value
            End Sub

            Private Function IBitstreamImage_GetTrack(Track As UShort, Side As Byte) As IBitstreamTrack Implements IBitstreamImage.GetTrack
                Return GetTrack(Track, Side)
            End Function
        End Class
    End Namespace
End Namespace
