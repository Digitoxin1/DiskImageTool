Public Class BootstrapLookup
    Public Property ExactMatch As Boolean = False
    Public Property KnownOEMNames As New List(Of KnownOEMName)
    Public Property Language As String = "English"
End Class

Public Class HexViewData

    Public Sub New(DataBlock As DiskImage.DataBlock)
        _DataBlock = DataBlock
    End Sub

    Public ReadOnly Property DataBlock As DiskImage.DataBlock
    Public Property HighlightedRegions As New List(Of HexViewHighlightRegion)
End Class

Public Class HexViewHighlightRegion
    Implements IEquatable(Of HexViewHighlightRegion)
    Implements IComparable(Of HexViewHighlightRegion)
    Public Sub New(Start As Long, Size As Long, ForeColor As Color, BackColor As Color)
        _Start = Start
        _Size = Size
        _ForeColor = ForeColor
        _BackColor = BackColor
    End Sub

    Public ReadOnly Property BackColor As Color
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

Public Class KnownOEMName
    Public Property Company As String = ""
    Public Property Description As String = ""
    Public Property Name As Byte()
    Public Property Suggestion As Boolean = True
    Public Property Win9xId As Boolean = False

    Public Function GetNameAsString() As String
        Return CodePage437ToUnicode(_Name)
    End Function

    Public Overrides Function ToString() As String
        Return GetNameAsString()
    End Function

End Class