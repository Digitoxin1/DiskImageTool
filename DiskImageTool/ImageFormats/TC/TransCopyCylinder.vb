Imports DiskImageTool.Bitstream

Namespace ImageFormats
    Namespace TC
        Public Class TransCopyCylinder
            Implements IBitstreamTrack

            Private Const ROTATION_LENGTH As UShort = 40000
            Private _Bitstream As BitArray

            Public Sub New(Track As UShort, Side As UShort)
                _Track = Track
                _Side = Side
                _Offset = 0
                _Skew = 0
                _Length = 0
                _Flags = 0
                _TrackType = 0
                _AddressMarkingTiming = New List(Of UShort)
                _Bitstream = New BitArray(0)
                _MFMData = Nothing
                _Decoded = False
            End Sub
            Public Property Track As UShort
            Public Property Side As UShort
            Public Property Skew As UInt16
            Public Property Offset As UInt32
            Public Property Length As UInt16
            Public Property Flags As TransCopyTrackFlags = 0
            Public Property TrackType As TransCopyDiskType
            Public ReadOnly Property AddressMarkingTiming As List(Of UShort)
            Public Property Bitstream As BitArray Implements IBitstreamTrack.Bitstream
                Get
                    Return _Bitstream
                End Get
                Set
                    _Bitstream = Value
                    _Length = _Bitstream.Length \ 8
                End Set
            End Property

            Public Property MFMData As Bitstream.IBM_MFM.IBM_MFM_Track Implements IBitstreamTrack.MFMData
            Public Property Decoded As Boolean Implements IBitstreamTrack.Decoded

            Public Property KeepTrackLength() As Boolean
                Get
                    Return (_Flags And TransCopyTrackFlags.KeepTrackLength) > 0
                End Get
                Set(value As Boolean)
                    _Flags = ToggleBit(_Flags, TransCopyTrackFlags.KeepTrackLength, value)
                End Set
            End Property

            Public Property CopyAcrossIndex() As Boolean
                Get
                    Return (_Flags And TransCopyTrackFlags.CopyAcrossIndex) > 0
                End Get
                Set(value As Boolean)
                    _Flags = ToggleBit(_Flags, TransCopyTrackFlags.CopyAcrossIndex, value)
                End Set
            End Property

            Public Property CopyWeakBits() As Boolean
                Get
                    Return (_Flags And TransCopyTrackFlags.CopyWeakBits) > 0
                End Get
                Set(value As Boolean)
                    _Flags = ToggleBit(_Flags, TransCopyTrackFlags.CopyWeakBits, value)
                End Set
            End Property

            Public Property VerifyWrite() As Boolean
                Get
                    Return (_Flags And TransCopyTrackFlags.VerifyWrite) > 0
                End Get
                Set(value As Boolean)
                    _Flags = ToggleBit(_Flags, TransCopyTrackFlags.VerifyWrite, value)
                End Set
            End Property

            Public Property NoAddressMarks() As Boolean
                Get
                    Return (_Flags And TransCopyTrackFlags.NoAddressMarks) > 0
                End Get
                Set(value As Boolean)
                    _Flags = ToggleBit(_Flags, TransCopyTrackFlags.NoAddressMarks, value)
                End Set
            End Property

            Public Function IsMFMTrackType() As Boolean
                If _TrackType = TransCopyDiskType.MFMDoubleDensity Or _TrackType = TransCopyDiskType.MFMDoubleDensity360RPM Or _TrackType = TransCopyDiskType.MFMHighDensity Then
                    Return True
                Else
                    Return False
                End If
            End Function

            Public Sub SetTimings(AddressMarkIndexes As List(Of UInteger))
                _AddressMarkingTiming.Clear()
                If _Bitstream.Length > 0 Then
                    Dim Multiplier = ROTATION_LENGTH / _Bitstream.Length
                    Dim Start As UInteger = 0
                    Dim TimingIndex As UShort
                    For Each AddressMarkIndex In AddressMarkIndexes
                        TimingIndex = Math.Round((AddressMarkIndex - Start) * Multiplier)
                        _AddressMarkingTiming.Add(TimingIndex)
                        Start = AddressMarkIndex
                    Next
                    TimingIndex = Math.Round((_Bitstream.Length - Start) * Multiplier)
                    _AddressMarkingTiming.Add(TimingIndex)
                End If
            End Sub

            Private Function ToggleBit(Data As Byte, Bit As Byte, Value As Boolean) As Byte
                If Value Then
                    Return Data Or Bit
                Else
                    Return Data And Not Bit
                End If
            End Function
        End Class
    End Namespace
End Namespace
