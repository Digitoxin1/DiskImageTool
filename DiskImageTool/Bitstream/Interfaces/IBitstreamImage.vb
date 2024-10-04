Namespace Bitstream
    Public Interface IBitstreamImage
        ReadOnly Property TrackCount As UShort
        ReadOnly Property SideCount As Byte
        ReadOnly Property TrackStep As Byte


        Function GetTrack(Track As UShort, Side As Byte) As IBitstreamTrack
        Function UpdateBitstream() As Boolean
    End Interface
End Namespace
