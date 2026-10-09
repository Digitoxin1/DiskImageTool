Namespace DiskImage
    Public Enum DataChangeType
        Data
        Size
        Bitstream
        ImageField
    End Enum

    Public Class ImageFieldChange
        Public Sub New(IsTrackField As Boolean, Track As UShort, Side As Byte, FieldId As UShort, OriginalValue As Object, NewValue As Object)
            Me.IsTrackField = IsTrackField
            Me.Track = Track
            Me.Side = Side
            Me.FieldId = FieldId
            Me.OriginalValue = OriginalValue
            Me.NewValue = NewValue
        End Sub

        Public ReadOnly Property FieldId As UShort
        Public ReadOnly Property IsTrackField As Boolean
        Public ReadOnly Property NewValue As Object
        Public ReadOnly Property OriginalValue As Object
        Public ReadOnly Property Side As Byte
        Public ReadOnly Property Track As UShort
    End Class

    Public Class DataChange
        Public Sub New(Type As DataChangeType, Offset As UInteger, OriginalValue As Object, NewValue As Object)
            Me.Type = Type
            Me.Offset = Offset
            Me.OriginalValue = OriginalValue
            Me.NewValue = NewValue
        End Sub

        Public Sub New(Field As ImageFieldChange)
            Me.Type = DataChangeType.ImageField
            Me.ImageField = Field
        End Sub

        Public Property ImageField As ImageFieldChange
        Public Property NewValue As Object
        Public Property Offset As UInteger
        Public Property OriginalValue As Object
        Public Property Type As DataChangeType
    End Class
End Namespace
