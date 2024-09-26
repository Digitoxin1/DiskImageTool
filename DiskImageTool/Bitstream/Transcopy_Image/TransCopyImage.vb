Imports System.IO

Namespace Transcopy
    Public Class TransCopyImage
        Private _Comment() As Byte
        Private _Comment2() As Byte

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
            _Cylinders = New List(Of TransCopyCylinder)
            _FilePath = ""
            _Comment = New Byte(31) {}
            _Comment2 = New Byte(31) {}
            _DiskType = TransCopyDiskType.Unknown
            _CylinderStart = 0
            _CylinderEnd = 0
            _Sides = 0
            _CylinderIncrement = 1
        End Sub

        Public Function Load(Buffer() As Byte) As Boolean
            _FilePath = ""
            _Cylinders.Clear()
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

                For Track = 0 To _CylinderEnd
                    For Side = 0 To 1
                        Dim Cylinder = New TransCopyCylinder(Track, Side)

                        Dim Offset = 4 * Track + 2 * Side

                        Cylinder.Skew = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Skews + Offset)
                        Dim OffsetValue = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Offsets + Offset)
                        Cylinder.Offset = MyBitConverter.SwapEndian(OffsetValue) * 256
                        Cylinder.Length = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Lengths + Offset)
                        Cylinder.Flags = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Flags + Offset)
                        For i = 0 To 15
                            Dim TimingOffset = 64 * Track + 32 * Side + 2 * i
                            Dim Value = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Timings + TimingOffset)
                            Cylinder.AddressMarkingTiming.Add(Value)
                        Next

                        If Cylinder.Offset >= TransCopyOffsets.Data Then
                            If Cylinder.IsMFMTrackType Then
                                If Cylinder.Side < _Sides Then
                                    Dim Bitstream = MyBitConverter.BytesToBits(Buffer, Cylinder.Offset, Cylinder.Length)
                                    Cylinder.MFMData = New IBM_MFM.MFMTrack(Bitstream)
                                    Cylinder.Decoded = True
                                    'DebugTranscopyCylinder(Cylinder)
                                End If
                            End If

                            If Not Cylinder.Decoded Then
                                Dim Data(Cylinder.Length - 1) As Byte
                                Array.Copy(Buffer, Cylinder.Offset, Data, 0, Data.Length)
                                Cylinder.RawData = Data
                            End If

                            _Cylinders.Add(Cylinder)
                        End If
                    Next
                Next
            End If


            Return Result
        End Function

        Public Function Load(FilePath As String) As Boolean
            Return Load(File.ReadAllBytes(FilePath))
        End Function

        Public Property FilePath As String
        Public Property Comment As String
            Get
                Return System.Text.Encoding.UTF8.GetString(_Comment).TrimEnd(vbNullChar)
            End Get
            Set(value As String)
                value = value.Substring(0, 32).PadRight(32, vbNullChar)
                _Comment = System.Text.Encoding.UTF8.GetBytes(value)
            End Set
        End Property

        Public Property Comment2 As String
            Get
                Return System.Text.Encoding.UTF8.GetString(_Comment2).TrimEnd(vbNullChar)
            End Get
            Set(value As String)
                value = value.Substring(0, 32).PadRight(32, vbNullChar)
                _Comment2 = System.Text.Encoding.UTF8.GetBytes(value)
            End Set
        End Property

        Public Property DiskType As TransCopyDiskType
        Public Property CylinderStart As Byte
        Public Property CylinderEnd As Byte
        Public Property Sides As Byte
        Public Property CylinderIncrement As Byte
        Public Property Cylinders As List(Of TransCopyCylinder)

        Private Sub FillBuffer(buffer() As Byte, b As Byte)
            For i = 0 To buffer.Length - 1
                buffer(i) = b
            Next
        End Sub

        Public Function Export(FilePath As String) As Boolean
            Dim buffer() As Byte
            Dim OffsetList As New List(Of UShort)
            Dim Index As Integer

            Dim Result = UpdateBitstream()

            Try
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
                    FillBuffer(buffer, &H11)
                    Index = 0
                    For Each Cylinder In _Cylinders
                        BitConverter.GetBytes(Cylinder.Skew).CopyTo(buffer, Index)
                        Index += 2
                    Next
                    fs.Write(buffer, 0, buffer.Length)

                    '0x305      word[256]   Start of track data (One entry per track)                
                    FillBuffer(buffer, &H0)
                    fs.Write(buffer, 0, buffer.Length)

                    '0x505      word[256]   Size of track data (One entry per track)
                    FillBuffer(buffer, &H33)
                    Index = 0
                    For Each Cylinder In _Cylinders
                        BitConverter.GetBytes(Cylinder.Length).CopyTo(buffer, Index)
                        Index += 2
                    Next
                    fs.Write(buffer, 0, buffer.Length)

                    '0x705      word[256]   Track flags (One entry per track)
                    FillBuffer(buffer, &H44)
                    Index = 0
                    For Each Cylinder In _Cylinders
                        BitConverter.GetBytes(Cylinder.Flags).CopyTo(buffer, Index)
                        Index += 2
                    Next
                    fs.Write(buffer, 0, buffer.Length)

                    '0x905      word[4096]  Address mark timing (16 entries per track)
                    buffer = New Byte(8191) {}
                    FillBuffer(buffer, &H0)
                    Index = 0
                    For Each Cylinder In _Cylinders
                        Dim Length = Cylinder.AddressMarkingTiming.Count
                        Dim Entry As UShort
                        For i = 0 To 15
                            If i < Length Then
                                Entry = Cylinder.AddressMarkingTiming(i)
                            Else
                                Entry = 0
                            End If
                            BitConverter.GetBytes(Entry).CopyTo(buffer, Index)
                            Index += 2
                        Next
                    Next
                    fs.Write(buffer, 0, buffer.Length)

                    '0x2905     byte[5883]  Unused space
                    buffer = New Byte(5882) {}
                    fs.Write(buffer, 0, buffer.Length)

                    '0x4000     Start of track data
                    For Each Cylinder In _Cylinders
                        If Cylinder.Decoded Then
                            buffer = MyBitConverter.BitsToBytes(Cylinder.MFMData.Bitstream)
                        Else
                            buffer = Cylinder.RawData
                        End If
                        Dim AllocatedSpace As UInteger = Math.Ceiling(buffer.Length / 256) * 256
                        Dim Offset = fs.Position
                        Dim NextBoundary = Math.Ceiling(Offset / 65536) * 65536
                        If Offset + buffer.Length > NextBoundary Then
                            For Counter = 0 To NextBoundary - Offset - 1
                                fs.WriteByte(&H0)
                            Next
                            Offset = fs.Position
                        End If
                        fs.Write(buffer, 0, buffer.Length)
                        For counter = 0 To AllocatedSpace - buffer.Length - 1
                            fs.WriteByte(&H0)
                        Next
                        OffsetList.Add(Offset \ 256)
                    Next

                    fs.Seek(&H305, SeekOrigin.Begin)
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

        Private Function UpdateBitstream() As Boolean
            Dim Updated As Boolean = False

            For Each Cylinder In _Cylinders
                If Cylinder.Decoded Then
                    Updated = Updated Or Cylinder.MFMData.UpdateBitstream()
                End If
            Next

            Return Updated
        End Function
    End Class
End Namespace
