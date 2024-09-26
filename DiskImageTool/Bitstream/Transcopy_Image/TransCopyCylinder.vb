Namespace Transcopy
    Public Class TransCopyCylinder
        Public Sub New(Track As UShort, Side As UShort)
            _Track = Track
            _Side = Side
            _AddressMarkingTiming = New List(Of UShort)
            _RawData = New Byte(-1) {}
            _MFMData = Nothing
            _Decoded = False
        End Sub
        Public Property Track As UShort
        Public Property Side As UShort
        Public Property Skew As UInt16
        Public Property Offset As UInt32
        Public Property Length As UInt16
        Public Property Flags As TransCopyTrackFlags
        Public ReadOnly Property AddressMarkingTiming As List(Of UShort)
        Public Property RawData As Byte()
        Public Property MFMData As IBM_MFM.MFMTrack
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
End Namespace
