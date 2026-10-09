Namespace DiskImage
    Public Interface IImageFieldSource
        Sub SetImageField(IsTrackField As Boolean, Track As UShort, Side As Byte, Sector As UShort, FieldId As UShort, Value As Object)
    End Interface
End Namespace
