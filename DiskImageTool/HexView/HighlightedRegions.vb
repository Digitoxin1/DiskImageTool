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

    Public Sub AddOffset(Offset As DiskImage.BootSector.BootSectorOffsets, ForeColor As Color)
        Dim Name As String = [Enum].GetName(GetType(DiskImage.BootSector.BootSectorOffsets), Offset)
        Dim Size As DiskImage.BootSector.BootSectorSizes

        If Not [Enum].TryParse(Of DiskImage.BootSector.BootSectorSizes)(Name, Size) Then
            Size = 0
        End If

        Me.Add(New HexViewHighlightRegion(Offset, Size, ForeColor, DiskImage.BootSectorDescription(Offset)))
    End Sub
End Class
