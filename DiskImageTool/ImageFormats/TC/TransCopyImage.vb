Imports DiskImageTool.Bitstream

Namespace ImageFormats
    Namespace TC
        Public Class TransCopyImage
            Implements IBitstreamImage

            Private Const EMPTY_FLAG As Byte = &H44
            Private Const EMPTY_SIZE As Byte = &H33
            Private Const EMPTY_SKU As Byte = &H11
            Private _Comment() As Byte
            Private _Comment2() As Byte
            Private _CylinderEnd As Byte
            Private _CylinderIncrement As Byte
            Private _Cylinders() As TransCopyCylinder
            Private _CylinderStart As Byte
            Private _Sides As Byte
            Private Enum TransCopyOffsets
                Signature = &H0
                Comment = &H2
                Comment2 = &H22
                DiskType = &H100
                CylinderStart = &H101
                CylinderEnd = &H102
                Sides = &H103
                CylinderIncrement = &H104
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
                _CylinderStart = 0
                _CylinderEnd = 0
                _Sides = 0
                _CylinderIncrement = 1
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

            Public ReadOnly Property CylinderCount As UShort Implements IBitstreamImage.TrackCount
                Get
                    Return _CylinderEnd + 1
                End Get
            End Property

            Public ReadOnly Property CylinderEnd As Byte
                Get
                    Return _CylinderEnd
                End Get
            End Property

            Public ReadOnly Property CylinderIncrement As Byte
                Get
                    Return _CylinderIncrement
                End Get
            End Property

            Public ReadOnly Property CylinderStart As Byte
                Get
                    Return _CylinderStart
                End Get
            End Property

            Public Property DiskType As TransCopyDiskType

            Public ReadOnly Property Sides As Byte Implements IBitstreamImage.SideCount
                Get
                    Return _Sides
                End Get
            End Property

            Private ReadOnly Property IBitstreamImage_TrackStep As Byte Implements IBitstreamImage.TrackStep
                Get
                    Return 1
                End Get
            End Property

            Public Function Export(FilePath As String) As Boolean
                Dim buffer() As Byte
                Dim OffsetList As New List(Of UShort)
                Dim Index As Integer
                Dim Cylinder As TransCopyCylinder

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

                        '0x101      byte        Starting cylinder number
                        fs.WriteByte(_CylinderStart)

                        '0x102      byte        Ending cylinder number
                        fs.WriteByte(_CylinderEnd)

                        '0x103      byte        Number of sides
                        fs.WriteByte(_Sides)

                        '0x104      byte        Cylinder increment between tracks
                        fs.WriteByte(_CylinderIncrement)

                        buffer = New Byte(511) {}

                        '0x105      word[256]   Track skew (One entry per track)                
                        FillBuffer(buffer, EMPTY_SKU)
                        Index = 0
                        For Track = 0 To _CylinderEnd
                            For Side = 0 To 1
                                If Side < _Sides Then
                                    Cylinder = GetCylinder(Track, Side)
                                    If Cylinder IsNot Nothing Then
                                        BitConverter.GetBytes(Cylinder.Skew).CopyTo(buffer, Index)
                                    End If
                                End If
                                Index += 2
                            Next
                        Next
                        fs.Write(buffer, 0, buffer.Length)

                        '0x305      word[256]   Start of track data (One entry per track)                
                        FillBuffer(buffer, &H0)
                        fs.Write(buffer, 0, buffer.Length)

                        '0x505      word[256]   Size of track data (One entry per track)
                        FillBuffer(buffer, EMPTY_SIZE)
                        Index = 0
                        For Track = 0 To _CylinderEnd
                            For Side = 0 To 1
                                If Side < _Sides Then
                                    Cylinder = GetCylinder(Track, Side)
                                    If Cylinder IsNot Nothing Then
                                        BitConverter.GetBytes(Cylinder.Length).CopyTo(buffer, Index)
                                    End If
                                End If
                                Index += 2
                            Next
                        Next
                        fs.Write(buffer, 0, buffer.Length)

                        '0x705      word[256]   Track flags (One entry per track)
                        FillBuffer(buffer, EMPTY_FLAG)
                        Index = 0
                        For Track = 0 To _CylinderEnd
                            For Side = 0 To 1
                                If Side < _Sides Then
                                    Cylinder = GetCylinder(Track, Side)
                                    If Cylinder IsNot Nothing Then
                                        Dim TrackType = Cylinder.TrackType
                                        If Cylinder.NoAddressMarks Then
                                            TrackType = TrackType Or &H80
                                        End If
                                        buffer(Index) = Cylinder.Flags
                                        buffer(Index + 1) = TrackType
                                    End If
                                End If
                                Index += 2
                            Next
                        Next
                        fs.Write(buffer, 0, buffer.Length)

                        '0x905      word[4096]  Address mark timing (16 entries per track)
                        buffer = New Byte(8191) {}
                        FillBuffer(buffer, &H0)
                        Index = 0
                        For Track = 0 To _CylinderEnd
                            For Side = 0 To 1
                                If Side < _Sides Then
                                    Cylinder = GetCylinder(Track, Side)
                                    If Cylinder IsNot Nothing Then
                                        Dim Length = Cylinder.AddressMarkingTiming.Count
                                        Dim Entry As UShort
                                        For i = 0 To 15
                                            If i < Length Then
                                                Entry = Cylinder.AddressMarkingTiming(i)
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
                        For Track = 0 To _CylinderEnd
                            For Side = 0 To 1
                                Dim TrackOffset As UShort = 0
                                If Side < _Sides Then
                                    Cylinder = GetCylinder(Track, Side)
                                    If Cylinder IsNot Nothing Then
                                        Dim BitLength = Math.Ceiling(Cylinder.Bitstream.Length / 2048) * 2048
                                        'Dim Padding = BitLength - Cylinder.Bitstream.Length
                                        'buffer = IBM_MFM.BitsToBytes(Cylinder.Bitstream, Padding)
                                        buffer = IBM_MFM.BitsToBytes(IBM_MFM.ResizeBitstream(Cylinder.Bitstream, BitLength), 0)
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
                    Return False
                End Try

                Return True
            End Function

            Public Function GetCylinder(Cylinder As Byte, Side As Byte) As TransCopyCylinder
                If Cylinder > _CylinderEnd Or Side > _Sides - 1 Then
                    Throw New System.IndexOutOfRangeException
                End If

                Dim Index = Cylinder * _Sides + Side

                Return _Cylinders(Index)
            End Function

            Public Sub Initialize(Cylinders As Byte, Sides As Byte, CylinderIncrement As Byte)
                _CylinderStart = 0
                _CylinderEnd = Cylinders - 1
                _Sides = Sides
                _CylinderIncrement = CylinderIncrement

                _Cylinders = New TransCopyCylinder((_CylinderEnd + 1) * Sides - 1) {}
            End Sub

            Public Function Initialize(Buffer() As Byte) As Boolean
                Dim Result As Boolean

                Dim Signature = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Signature)
                Result = (Signature = &HA55A)

                If Result Then
                    Result = Buffer.Length > TransCopyOffsets.Data
                End If

                If Result Then
                    Array.Copy(Buffer, TransCopyOffsets.Comment, _Comment, 0, TransCopySizes.Comment)
                    Array.Copy(Buffer, TransCopyOffsets.Comment2, _Comment2, 0, TransCopySizes.Comment2)

                    _DiskType = Buffer(TransCopyOffsets.DiskType)
                    _CylinderStart = Buffer(TransCopyOffsets.CylinderStart)
                    _CylinderEnd = Buffer(TransCopyOffsets.CylinderEnd)
                    _Sides = Buffer(TransCopyOffsets.Sides)
                    _CylinderIncrement = Buffer(TransCopyOffsets.CylinderIncrement)

                    _Cylinders = New TransCopyCylinder((_CylinderEnd + 1) * Sides - 1) {}

                    For Track = 0 To _CylinderEnd
                        For Side = 0 To _Sides - 1
                            Dim Cylinder = New TransCopyCylinder(Track, Side)

                            Dim Offset = 4 * Track + 2 * Side

                            Dim OffsetValue = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Offsets + Offset)
                            If OffsetValue > 0 Then
                                Cylinder.Offset = MyBitConverter.SwapEndian(OffsetValue) * 256
                                Cylinder.Skew = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Skews + Offset)
                                Cylinder.Length = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Lengths + Offset)
                                Cylinder.Flags = Buffer(TransCopyOffsets.Flags + Offset)
                                Cylinder.TrackType = Buffer(TransCopyOffsets.Flags + Offset + 1) And &H7F

                                For i = 0 To 15
                                    Dim TimingOffset = 64 * Track + 32 * Side + 2 * i
                                    Dim Value = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Timings + TimingOffset)
                                    Cylinder.AddressMarkingTiming.Add(Value)
                                Next

                                If Cylinder.Offset >= TransCopyOffsets.Data Then
                                    Cylinder.Bitstream = IBM_MFM.BytesToBits(Buffer, Cylinder.Offset, Cylinder.Length * 8)

                                    If Cylinder.IsMFMTrackType Then
                                        Cylinder.MFMData = New IBM_MFM.IBM_MFM_Track(Cylinder.Bitstream)
                                        Cylinder.Decoded = True
                                        'Bitstream.DebugTranscopyCylinder(Cylinder)
                                    End If
                                End If
                            End If
                            SetCylinder(Track, Side, Cylinder)
                        Next
                    Next
                End If

                Return Result
            End Function

            Public Function Load(FilePath As String) As Boolean
                Return Initialize(IO.File.ReadAllBytes(FilePath))
            End Function
            Public Sub SetCylinder(Cylinder As Byte, Side As Byte, Value As TransCopyCylinder)
                If Cylinder > _CylinderEnd Or Side > _Sides - 1 Then
                    Throw New System.IndexOutOfRangeException
                End If

                Dim Index = Cylinder * _Sides + Side

                _Cylinders(Index) = Value
            End Sub

            Private Sub FillBuffer(buffer() As Byte, b As Byte)
                For i = 0 To buffer.Length - 1
                    buffer(i) = b
                Next
            End Sub

            Private Function IBitstreamImage_GetTrack(Track As UShort, Side As Byte) As IBitstreamTrack Implements IBitstreamImage.GetTrack
                Return GetCylinder(Track, Side)
            End Function

            Private ReadOnly Property IBitstreamImage_HasSurfaceData As Boolean Implements IBitstreamImage.HasSurfaceData
                Get
                    Return False
                End Get
            End Property
        End Class
    End Namespace
End Namespace
