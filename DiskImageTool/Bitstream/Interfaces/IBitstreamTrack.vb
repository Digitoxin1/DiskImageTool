Namespace Bitstream
    Public Interface IBitstreamTrack
        Property Bitstream As BitArray
        Property MFMData As IBM_MFM.IBM_MFM_Track
        Property Decoded As Boolean
        ReadOnly Property BitRate As UInt16
        ReadOnly Property RPM As UInt16
        ReadOnly Property TrackType As BitstreamTrackType
    End Interface

    Public Enum BitstreamTrackType
        MFM = 1
        FM = 2
        Other = 3
    End Enum
End Namespace
