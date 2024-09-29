Namespace Bitstream
    Public Interface IBitstreamTrack
        Property Bitstream As BitArray
        Property MFMData As Bitstream.IBM_MFM.IBM_MFM_Track
        Property Decoded As Boolean
    End Interface
End Namespace
