Imports DiskImageTool.Bitstream

Namespace ImageFormats.D86F
    Public Class D86FTrack
        Implements IBitstreamTrack

        Private _Bitstream As BitArray
        Private _SurfaceData As BitArray

        Public Sub New(Track As UShort, Side As Byte, Offset As UInt32)
            _Flags = 0
            _Track = Track
            _Side = Side
            _Offset = Offset
            _BitCellCount = 0
            _IndexHolePos = 0
            _Bitstream = New BitArray(0)
            _SurfaceData = Nothing
            _MFMData = Nothing
        End Sub

        Public Property BitCellCount As UInt32

        Public Property BitRate As BitRate
            Get
                Return _Flags And &H7
            End Get
            Set(value As BitRate)
                value = value And &H7
                _Flags = (_Flags And Not (&H7)) Or value
            End Set
        End Property

        Public Property Bitstream As BitArray Implements IBitstreamTrack.Bitstream
            Get
                Return _Bitstream
            End Get
            Set
                _Bitstream = Value
            End Set
        End Property

        Public ReadOnly Property BitstreamTrackType As BitstreamTrackType Implements IBitstreamTrack.TrackType
            Get
                Select Case Encoding
                    Case Encoding.MFM
                        If MFMData IsNot Nothing AndAlso _MFMData.Sectors.Count > 0 Then
                            Return BitstreamTrackType.MFM
                        Else
                            Return BitstreamTrackType.Other
                        End If
                    Case Encoding.FM
                        If _Bitstream.Length > 0 Then
                            Return BitstreamTrackType.FM
                        Else
                            Return BitstreamTrackType.Other
                        End If
                    Case Else
                        Return BitstreamTrackType.Other
                End Select
            End Get
        End Property

        Public ReadOnly Property Decoded As Boolean Implements IBitstreamTrack.Decoded
            Get
                Return _MFMData IsNot Nothing AndAlso _MFMData.Sectors.Count > 0
            End Get
        End Property

        Public Property Encoding As Encoding
            Get
                Return (_Flags >> 3) And &H3
            End Get
            Set(value As Encoding)
                _Flags = (_Flags And Not (&H18)) Or (value << 3)
            End Set
        End Property

        Public Property Flags As UShort

        Public Property IndexHolePos As UInt32

        Public Property MFMData As IBM_MFM.IBM_MFM_Track Implements IBitstreamTrack.MFMData

        Public Property Offset As UInt32

        Public Property RPM As RPM
            Get
                Return (_Flags >> 5) And &H7
            End Get
            Set(value As RPM)
                value = value And &H7
                _Flags = (_Flags And Not (&HE0)) Or (value << 5)
            End Set
        End Property

        Public Property Side As Byte Implements IBitstreamTrack.Side

        Public Property SurfaceData As BitArray Implements IBitstreamTrack.SurfaceData
            Get
                Return _SurfaceData
            End Get
            Set
                _SurfaceData = Value
            End Set
        End Property

        Public Property Track As UShort Implements IBitstreamTrack.Track

        Private ReadOnly Property IBitstreamTrack_BitRate As UShort Implements IBitstreamTrack.BitRate
            Get
                Return GetBitRate(BitRate, Decoded)
            End Get
        End Property

        Private ReadOnly Property IBitstreamTrack_RPM As UShort Implements IBitstreamTrack.RPM
            Get
                Return GetRPM(RPM)
            End Get
        End Property
    End Class
End Namespace