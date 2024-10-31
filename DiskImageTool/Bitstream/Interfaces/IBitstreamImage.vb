Namespace Bitstream
    Public Interface IBitstreamImage
        ReadOnly Property TrackCount As UShort
        ReadOnly Property SideCount As Byte
        ReadOnly Property TrackStep As Byte
        ReadOnly Property HasSurfaceData As Boolean
        Function Load(FilePath As String) As Boolean
        Function Load(Buffer() As Byte) As Boolean
        Function Export(FilePath As String) As Boolean
        Function GetTrack(Track As UShort, Side As Byte) As IBitstreamTrack
    End Interface
End Namespace
