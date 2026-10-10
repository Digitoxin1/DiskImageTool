Imports DiskImageTool.DiskImage

Public Class HighlightedRegions
    Inherits List(Of HexViewHighlightRegion)

    Public Sub AddBootSectorOffset(Offset As BootSector.BootSectorOffsets, ForeColor As Color)
        Dim Name As String = [Enum].GetName(GetType(BootSector.BootSectorOffsets), Offset)
        Dim Size As BootSector.BootSectorSizes

        If Not [Enum].TryParse(Name, Size) Then
            Size = 0
        End If

        Me.Add(New HexViewHighlightRegion(Offset, Size, ForeColor, Offset.GetDescription()))
    End Sub

    Public Sub AddBootSectorOffset(Description As String, Offset As BootSector.BootSectorOffsets, Size As BootSector.BootSectorSizes, ForeColor As Color)
        Me.Add(New HexViewHighlightRegion(Offset, Size, ForeColor, Description))
    End Sub

    Public Sub AddBPBoffset(Offset As DiskImage.BiosParameterBlock.BPBOoffsets, ForeColor As Color)
        Dim Name As String = [Enum].GetName(GetType(BiosParameterBlock.BPBOoffsets), Offset)
        Dim Size As BiosParameterBlock.BPBSizes

        If Not [Enum].TryParse(Name, Size) Then
            Size = 0
        End If

        Me.Add(New HexViewHighlightRegion(Offset, Size, ForeColor, Offset.GetDescription()))
    End Sub

    Public Sub AddDirectoryEntryLFNOffset(Start As Long, Offset As DirectoryEntry.LFNOffsets, ForeColor As Color)
        Dim Name As String = [Enum].GetName(GetType(DirectoryEntry.LFNOffsets), Offset)
        Dim Size As DirectoryEntry.LFNSizes

        If Not [Enum].TryParse(Name, Size) Then
            Size = 0
        End If

        Me.Add(New HexViewHighlightRegion(Start + Offset, Size, ForeColor, Offset.GetDescription()))
    End Sub

    Public Sub AddDirectoryEntryOffset(Start As Long, Offset As DirectoryEntry.DirectoryEntryOffsets, ForeColor As Color)
        Dim Name As String = [Enum].GetName(GetType(DirectoryEntry.DirectoryEntryOffsets), Offset)
        Dim Size As DirectoryEntry.DirectoryEntrySizes

        If Not [Enum].TryParse(Name, Size) Then
            Size = 0
        End If

        Me.Add(New HexViewHighlightRegion(Start + Offset, Size, ForeColor, Offset.GetDescription()))
    End Sub

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
End Class
