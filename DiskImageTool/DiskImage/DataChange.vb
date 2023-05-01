Namespace DiskImage
    Public Class DataChange
        Public Sub New(Offset As UInteger, OriginalValue As Object, NewValue As Object)
            Me.Offset = Offset
            Me.OriginalValue = OriginalValue
            Me.NewValue = NewValue
        End Sub
        Public Property Offset As UInteger
        Public Property OriginalValue As Object
        Public Property NewValue As Object
    End Class
End Namespace
