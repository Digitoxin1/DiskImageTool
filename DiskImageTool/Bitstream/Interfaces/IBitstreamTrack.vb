Namespace Bitstream
    Public Enum BitstreamTrackType
        MFM = 1
        FM = 2
        Other = 3
    End Enum

    Public Interface IBitstreamTrack
        ReadOnly Property BitRate As UInt16
        Property Bitstream As BitArray
        ReadOnly Property Decoded As Boolean
        Property MFMData As IBM_MFM.IBM_MFM_Track
        ReadOnly Property RPM As UInt16
        ReadOnly Property Side As Byte
        ReadOnly Property SurfaceData As BitArray
        ReadOnly Property Track As UShort
        ReadOnly Property TrackType As BitstreamTrackType
    End Interface
End Namespace
