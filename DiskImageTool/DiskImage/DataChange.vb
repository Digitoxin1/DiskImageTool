Namespace DiskImage

    Public Enum DataChangeType
        Data
        Size
    End Enum
    Public Class DataChange
        Public Sub New(Type As DataChangeType, Offset As UInteger, OriginalValue As Object, NewValue As Object)
            Me.Type = Type
            Me.Offset = Offset
            Me.OriginalValue = OriginalValue
            Me.NewValue = NewValue
        End Sub
        Public Property Type As DataChangeType
        Public Property Offset As UInteger
        Public Property OriginalValue As Object
        Public Property NewValue As Object
    End Class
End Namespace
