﻿Imports DiskImageTool.Bitstream

Namespace ImageFormats
    Namespace MFM
        Public Class MFMTrack
            Implements IBitstreamTrack

            Private _Bitstream As BitArray

            Public Sub New(Track As UShort, Side As Byte)
                _Track = Track
                _Side = Side
                _Offset = 0
                _Bitstream = New BitArray(0)
                _MFMData = Nothing
                _RPM = 0
                _BitRate = 0
            End Sub

            Public Property Track As UShort
            Public Property Side As Byte
            Public Property Offset As UInt32
            Public Property Length As UInt16
            Public Property RPM As UInt16 Implements IBitstreamTrack.RPM
            Public Property BitRate As UInt16 Implements IBitstreamTrack.BitRate
            Public Property Bitstream As BitArray Implements IBitstreamTrack.Bitstream
                Get
                    Return _Bitstream
                End Get
                Set
                    _Bitstream = Value
                    _Length = _Bitstream.Length \ 8
                End Set
            End Property
            Public Property MFMData As IBM_MFM.IBM_MFM_Track Implements IBitstreamTrack.MFMData
            Public Property Decoded As Boolean Implements IBitstreamTrack.Decoded
                Get
                    Return True
                End Get
                Set
                    'Do Nothing
                End Set
            End Property

            Private ReadOnly Property IBitstreamTrack_TrackType As BitstreamTrackType Implements IBitstreamTrack.TrackType
                Get
                    Return BitstreamTrackType.MFM
                End Get
            End Property
        End Class
    End Namespace
End Namespace