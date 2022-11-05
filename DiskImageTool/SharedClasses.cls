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
    Public Property HighlightedRegions As New HighlightedRegions
End Class

Public Class HighlightedRegions
    Inherits List(Of HexViewHighlightRegion)

    Public Function AddItem(Start As Long, Size As Long, ForeColor As Color) As HexViewHighlightRegion
        Return AddItem(Start, Size, ForeColor, Color.White, "")
    End Function

    Public Function AddItem(Start As Long, Size As Long, ForeColor As Color, Description As String) As HexViewHighlightRegion
        Return AddItem(Start, Size, ForeColor, Color.White, Description)
    End Function

    Public Function AddItem(Start As Long, Size As Long, ForeColor As Color, BackColor As Color) As HexViewHighlightRegion
        Return AddItem(Start, Size, ForeColor, BackColor, "")
    End Function

    Public Function AddItem(Start As Long, Size As Long, ForeColor As Color, BackColor As Color, Description As String) As HexViewHighlightRegion
        Dim HexViewHighlightRegion As New HexViewHighlightRegion(Start, Size, ForeColor, BackColor, Description)
        Me.Add(HexViewHighlightRegion)

        Return HexViewHighlightRegion
    End Function

    Public Sub AddOffset(Offset As DiskImage.BootSector.BootSectorOffset, ForeColor As Color)
        Dim Name As String = [Enum].GetName(GetType(DiskImage.BootSector.BootSectorOffset), Offset)
        Dim Size As DiskImage.BootSector.BootSectorSize

        If Not [Enum].TryParse(Of DiskImage.BootSector.BootSectorSize)(Name, Size) Then
            Size = 0
        End If

        Me.Add(New HexViewHighlightRegion(Offset, Size, ForeColor, BootSectorDescription(Offset)))
    End Sub
End Class

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