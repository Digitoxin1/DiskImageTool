Imports System.IO

Namespace Transcopy
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

                        Dim Offset = 4 * Track + 2 * Side

                        Cylinder.Skew = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Skews + Offset)
                        Dim OffsetValue = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Offsets + Offset)
                        Cylinder.Offset = MyBitConverter.SwapEndian(OffsetValue) * 256
                        Cylinder.Length = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Lengths + Offset)
                        Cylinder.Flags = BitConverter.ToUInt16(Buffer, TransCopyOffsets.Flags + Offset)

                        If Cylinder.Offset >= TransCopyOffsets.Data Then
                            Cylinder.BitstreamOffset = (Cylinder.Offset - TransCopyOffsets.Data) * 8
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
                                    Cylinder.DecodedData = New IBM_MFM.MFMTrack(Cylinder.Data)
                                    Cylinder.Decoded = True
                                    'DebugTranscopyCylinder(Cylinder)
                                End If
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
        Public Property Comment2 As String
        Public Property DiskType As TransCopyDiskType
        Public Property CylinderStart As Byte
        Public Property CylinderEnd As Byte
        Public Property Sides As Byte
        Public Property CylinderIncrement As Byte
        Public Property Cylinders As List(Of TransCopyCylinder)
    End Class
End Namespace
