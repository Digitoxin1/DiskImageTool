Namespace Bitstream
    Public Interface IBitstreamImage
        ReadOnly Property BitRate As UShort
        ReadOnly Property HasSurfaceData As Boolean
        ReadOnly Property RPM As UShort
        ReadOnly Property SideCount As Byte
        ReadOnly Property TrackCount As UShort
        ReadOnly Property TrackStep As Byte
        ReadOnly Property VariableBitRate As Boolean
        ReadOnly Property VariableRPM As Boolean
        Function Export(FilePath As String) As Boolean
        Function GetTrack(Track As UShort, Side As Byte) As IBitstreamTrack
        Function Load(FilePath As String) As Boolean
        Function Load(Buffer() As Byte) As Boolean
    End Interface
End Namespace
