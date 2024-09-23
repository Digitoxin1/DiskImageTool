Imports System.IO

Namespace Transcopy
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
            If Type = TransCopyDiskType.MFMDoubleDensity Or Type = TransCopyDiskType.MFMDoubleDensity360RPM Or Type = TransCopyDiskType.MFMHighDensity Then
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

            Dim Signature = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Signature)
            Result = (Signature = &HA55A)

            If Result Then
                Result = Buffer.Length > TransCopyOffsets.Data
            End If

            If Result Then
                _Comment = System.Text.Encoding.UTF8.GetString(Buffer, TransCopyOffsets.Comment, TransCopySizes.Comment).TrimEnd(vbNullChar)
                _Comment2 = System.Text.Encoding.UTF8.GetString(Buffer, TransCopyOffsets.Comment2, TransCopySizes.Comment2).TrimEnd(vbNullChar)

                _DiskType = Buffer(TransCopyOffsets.DiskType)
                _CylinderStart = Buffer(TransCopyOffsets.CylinderStart)
                _CylinderEnd = Buffer(TransCopyOffsets.CylinderEnd)
                _Sides = Buffer(TransCopyOffsets.Sides)
                _CylinderIncrement = Buffer(TransCopyOffsets.CylinderIncrement)

                For Track = 0 To _CylinderEnd
                    For Side = 0 To 1
                        Dim Cylinder = New TransCopyCylinder(Track, Side)
                        _Cylinders.Add(Cylinder)

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

                        If Cylinder.Offset > 0 Then
                            Dim Data(Cylinder.Length - 1) As Byte
                            Array.Copy(Buffer, Cylinder.Offset, Data, 0, Data.Length)
                            Cylinder.Data = Data
                        End If

                        If Cylinder.IsMFMTrackType Then
                            If Cylinder.Side < _Sides Then
                                Cylinder.DecodedData = New MFMTrack(Cylinder.Data)
                                Cylinder.Decoded = True
                                'DebugTranscopyCylinder(Cylinder)
                            End If
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
        Public Property Comment2 As String
        Public Property DiskType As TransCopyDiskType
        Public Property CylinderStart As Byte
        Public Property CylinderEnd As Byte
        Public Property Sides As Byte
        Public Property CylinderIncrement As Byte
        Public Property Cylinders As List(Of TransCopyCylinder)
    End Class
End Namespace
