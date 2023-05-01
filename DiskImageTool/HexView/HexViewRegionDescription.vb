Public Class HexViewRegionDescription
    Public Sub New(Start As Long, Size As Long, Text As String)
        _Start = Start
        _Size = Size
        _Text = Text
    End Sub
    Public ReadOnly Property Size As Long
    Public ReadOnly Property Start As Long
    Public ReadOnly Property Text As String
End Class
