Public Class HexViewHighlightRegion
    Implements IEquatable(Of HexViewHighlightRegion)
    Implements IComparable(Of HexViewHighlightRegion)

    Public Sub New(Start As Long, Size As Long, ForeColor As Color)
        _Start = Start
        _Size = Size
        _ForeColor = ForeColor
        _BackColor = Color.White
        _Description = ""
    End Sub

    Public Sub New(Start As Long, Size As Long, ForeColor As Color, Description As String)
        _Start = Start
        _Size = Size
        _ForeColor = ForeColor
        _BackColor = Color.White
        _Description = Description
    End Sub

    Public Sub New(Start As Long, Size As Long, ForeColor As Color, BackColor As Color)
        _Start = Start
        _Size = Size
        _ForeColor = ForeColor
        _BackColor = BackColor
        _Description = ""
    End Sub

    Public Sub New(Start As Long, Size As Long, ForeColor As Color, BackColor As Color, Description As String)
        _Start = Start
        _Size = Size
        _ForeColor = ForeColor
        _BackColor = BackColor
        _Description = Description
    End Sub

    Public ReadOnly Property BackColor As Color
    Public ReadOnly Property Description As String
    Public ReadOnly Property ForeColor As Color
    Public ReadOnly Property Size As Long
    Public ReadOnly Property Start As Long

    Public Function CompareTo(comparePart As HexViewHighlightRegion) As Integer Implements IComparable(Of HexViewHighlightRegion).CompareTo
        If comparePart Is Nothing Then
            Return 1
        Else

            Return Me.Start.CompareTo(comparePart.Start)
        End If
    End Function

    Public Overloads Function Equals(other As HexViewHighlightRegion) As Boolean Implements IEquatable(Of HexViewHighlightRegion).Equals
        If other Is Nothing Then
            Return False
        End If
        Return (Me.Start.Equals(other.Start))
    End Function
End Class