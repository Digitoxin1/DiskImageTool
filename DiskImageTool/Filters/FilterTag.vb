Namespace Filters
    Public Class FilterTag
        Public Sub New(Value As Long)
            _Value = Value
            _Visible = False
        End Sub
        Public ReadOnly Property Value As Long
        Public Property Visible As Boolean
    End Class
End Namespace
