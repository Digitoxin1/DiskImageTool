Imports System.IO

Public Enum TransCopyTrackFlags As UInt16
    KeepTrackLength = 1
    CopyAcrossIndex = 2
    CopyWeakBits = 4
    VerifyWrite = 8
    NoAddressMarks = 128
End Enum

Public Enum TransCopyDiskType As Byte
    Unknown = &HFF
    MFMHighDensity = &H2
    MFMDoubleDensity360RPM = &H3
    AppleIIGCR = &H4
    FMSengleDensity = &H5
    CommodoreGCR = &H6
    MFMDoubleDensity = &H7
    CommodoreAmiga = &H8
    AtariFM = &HC
End Enum

Public Class TransCopyCylinder
    Public Sub New(Track As UShort, Side As UShort)
        _Track = Track
        _Side = Side
        _AddressMarkingTiming = New List(Of UShort)
        _Data = New Byte(-1) {}
        _DecodedData = Nothing
        _Decoded = False
    End Sub
    Public Property Track As UShort
    Public Property Side As UShort
    Public Property Skew As UInt16
    Public Property Offset As UInt32
    Public Property Length As UInt16
    Public Property Flags As TransCopyTrackFlags
    Public ReadOnly Property AddressMarkingTiming As List(Of UShort)
    Public Property Data As Byte()
    Public Property DecodedData As MFMTrack
    Public Property Decoded As Boolean

    Public Function KeepTrackLength() As Boolean
        Return (_Flags And TransCopyTrackFlags.KeepTrackLength) > 0
    End Function

    Public Function CopyAcrossIndex() As Boolean
        Return (_Flags And TransCopyTrackFlags.CopyAcrossIndex) > 0
    End Function

    Public Function CopyWeakBits() As Boolean
        Return (_Flags And TransCopyTrackFlags.CopyWeakBits) > 0
    End Function

    Public Function VerifyWrite() As Boolean
        Return (_Flags And TransCopyTrackFlags.VerifyWrite) > 0
    End Function

    Public Function NoAddressMarks() As Boolean
        Return (_Flags And TransCopyTrackFlags.NoAddressMarks) > 0
    End Function

    Public Function TrackType() As TransCopyDiskType
        Dim Value = _Flags And Not (2 ^ 15)

        Return Value >> 8
    End Function

    Public Function IsMFMTrackType() As Boolean
        Dim Type = TrackType()
        If TransCopyDiskType.MFMDoubleDensity Or Type = TransCopyDiskType.MFMDoubleDensity360RPM Or Type = TransCopyDiskType.MFMHighDensity Then
            Return True
        Else
            Return False
        End If
    End Function
End Class

Public Class TransCopyImage
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
        _Comment = ""
        _Comment2 = ""
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

        Dim Signature = BitConverter.ToUInt16(buffer, TransCopyOffsets.Signature)
        Result = (Signature = &HA55A)

        If Result Then
            _Comment = System.Text.Encoding.UTF8.GetString(buffer, TransCopyOffsets.Comment, TransCopySizes.Comment).TrimEnd(vbNullChar)
            _Comment2 = System.Text.Encoding.UTF8.GetString(buffer, TransCopyOffsets.Comment2, TransCopySizes.Comment2).TrimEnd(vbNullChar)

            _DiskType = buffer(TransCopyOffsets.DiskType)
            _CylinderStart = buffer(TransCopyOffsets.CylinderStart)
            _CylinderEnd = buffer(TransCopyOffsets.CylinderEnd)
            _Sides = buffer(TransCopyOffsets.Sides)
            _CylinderIncrement = buffer(TransCopyOffsets.CylinderIncrement)

            For Track = 0 To _CylinderEnd
                For Side = 0 To 1
                    Dim Cylinder = New TransCopyCylinder(Track, Side)
                    _Cylinders.Add(Cylinder)

                    Dim Offset = 4 * Track + 2 * Side

                    Cylinder.Skew = BitConverter.ToUInt16(buffer, TransCopyOffsets.Skews + Offset)
                    Dim OffsetValue = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Offsets + Offset)
                    Cylinder.Offset = ToBigEndianUInt16(OffsetValue) * 256
                    Cylinder.Length = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Lengths + Offset)
                    Cylinder.Flags = BitConverter.ToUInt16(buffer, TransCopyOffsets.Flags + Offset)

                    For i = 0 To 15
                        Dim TimingOffset = 64 * Track + 32 * Side + 2 * i
                        Dim Value = BitConverter.ToUInt16(buffer, TransCopyOffsets.Timings + TimingOffset)
                        Cylinder.AddressMarkingTiming.Add(Value)
                    Next

                    If Cylinder.Offset > 0 Then
                        Dim Data(Cylinder.Length - 1) As Byte
                        Array.Copy(Buffer, Cylinder.Offset, Data, 0, Data.Length)
                        Cylinder.Data = Data
                    End If

                    If Cylinder.IsMFMTrackType Then
                        If Cylinder.Side < _Sides Then
                            Cylinder.DecodedData = New MFMTrack(Cylinder.Data)
                            'MFMTrackDebug(Cylinder.DecodedData, Track.ToString.PadLeft(2, "0") & "." & Side)
                            Cylinder.Decoded = True
                        End If
                    End If
                Next
            Next
        End If


        Return Result
    End Function

    Public Function Load(FilePath As String) As Boolean
        _FilePath = FilePath
        _Cylinders.Clear()
        Dim Result As Boolean = False

        Using fs As FileStream = File.OpenRead(FilePath)
            Dim Size As Integer
            Dim buffer(31) As Byte

            fs.Seek(TransCopyOffsets.Signature, SeekOrigin.Begin)
            fs.Read(buffer, 0, TransCopySizes.Signature)
            Dim Signature = BitConverter.ToUInt16(buffer, TransCopyOffsets.Signature)
            Result = (Signature = &HA55A)

            If Result Then
                fs.Seek(TransCopyOffsets.Comment, SeekOrigin.Begin)
                Size = fs.Read(buffer, 0, TransCopySizes.Comment)
                _Comment = System.Text.Encoding.UTF8.GetString(buffer, 0, Size).TrimEnd(vbNullChar)

                fs.Seek(TransCopyOffsets.Comment2, SeekOrigin.Begin)
                Size = fs.Read(buffer, 0, TransCopySizes.Comment2)
                _Comment2 = System.Text.Encoding.UTF8.GetString(buffer, 0, Size).TrimEnd(vbNullChar)

                fs.Seek(TransCopyOffsets.DiskType, SeekOrigin.Begin)
                _DiskType = fs.ReadByte

                fs.Seek(TransCopyOffsets.CylinderStart, SeekOrigin.Begin)
                _CylinderStart = fs.ReadByte

                fs.Seek(TransCopyOffsets.CylinderEnd, SeekOrigin.Begin)
                _CylinderEnd = fs.ReadByte

                fs.Seek(TransCopyOffsets.Sides, SeekOrigin.Begin)
                _Sides = fs.ReadByte

                fs.Seek(TransCopyOffsets.CylinderIncrement, SeekOrigin.Begin)
                _CylinderIncrement = fs.ReadByte

                For Track = 0 To _CylinderEnd
                    For Side = 0 To 1
                        Dim Cylinder = New TransCopyCylinder(Track, Side)
                        _Cylinders.Add(Cylinder)

                        Dim Offset = 4 * Track + 2 * Side

                        fs.Seek(TransCopyOffsets.Skews + Offset, SeekOrigin.Begin)
                        fs.Read(buffer, 0, 2)
                        Cylinder.Skew = BitConverter.ToUInt16(buffer, 0)

                        fs.Seek(TransCopyOffsets.Offsets + Offset, SeekOrigin.Begin)
                        fs.Read(buffer, 0, 2)
                        Cylinder.Offset = ToBigEndianUInt16(buffer) * 256

                        fs.Seek(TransCopyOffsets.Lengths + Offset, SeekOrigin.Begin)
                        fs.Read(buffer, 0, 2)
                        Cylinder.Length = BitConverter.ToUInt16(buffer, 0)

                        fs.Seek(TransCopyOffsets.Flags + Offset, SeekOrigin.Begin)
                        fs.Read(buffer, 0, 2)
                        Cylinder.Flags = BitConverter.ToUInt16(buffer, 0)

                        For i = 0 To 15
                            Dim TimingOffset = 64 * Track + 32 * Side + 2 * i
                            fs.Seek(TransCopyOffsets.Timings + TimingOffset, SeekOrigin.Begin)
                            fs.Read(buffer, 0, 2)
                            Dim Value = BitConverter.ToUInt16(buffer, 0)
                            Cylinder.AddressMarkingTiming.Add(Value)
                        Next

                        If Cylinder.Offset > 0 Then
                            Dim Data(Cylinder.Length - 1) As Byte
                            fs.Seek(Cylinder.Offset, SeekOrigin.Begin)
                            fs.Read(Data, 0, Data.Length)
                            Cylinder.Data = Data
                        End If

                        If Cylinder.IsMFMTrackType Then
                            If Cylinder.Side < _Sides Then
                                Cylinder.DecodedData = New MFMTrack(Cylinder.Data)
                                Cylinder.Decoded = True
                            End If
                        End If
                    Next
                Next
            End If
        End Using

        Return Result
    End Function

    Public Property FilePath As String
    Public Property Comment As String
    Public Property Comment2 As String
    Public Property DiskType As TransCopyDiskType
    Public Property CylinderStart As Byte
    Public Property CylinderEnd As Byte
    Public Property Sides As Byte
    Public Property CylinderIncrement As Byte
    Public Property Cylinders As List(Of TransCopyCylinder)
End Class
