Public Class HighlightedRegions
    Inherits List(Of HexViewHighlightRegion)

    Public Sub AddBootSectorOffset(Offset As DiskImage.BootSector.BootSectorOffsets, ForeColor As Color)
        Dim Name As String = [Enum].GetName(GetType(DiskImage.BootSector.BootSectorOffsets), Offset)
        Dim Size As DiskImage.BootSector.BootSectorSizes

        If Not [Enum].TryParse(Name, Size) Then
            Size = 0
        End If

        Me.Add(New HexViewHighlightRegion(Offset, Size, ForeColor, DiskImage.BootSectorDescription(Offset)))
    End Sub

    Public Sub AddBPBoffset(Offset As DiskImage.BiosParameterBlock.BPBOoffsets, ForeColor As Color)
        Dim Name As String = [Enum].GetName(GetType(DiskImage.BiosParameterBlock.BPBOoffsets), Offset)
        Dim Size As DiskImage.BiosParameterBlock.BPBSizes

        If Not [Enum].TryParse(Name, Size) Then
            Size = 0
        End If

        Me.Add(New HexViewHighlightRegion(Offset, Size, ForeColor, DiskImage.BPBDescription(Offset)))
    End Sub

    Public Sub AddDirectoryEntryLFNOffset(Start As Long, Offset As DiskImage.DirectoryEntry.LFNOffsets, ForeColor As Color)
        Dim Name As String = [Enum].GetName(GetType(DiskImage.DirectoryEntry.LFNOffsets), Offset)
        Dim Size As DiskImage.DirectoryEntry.LFNSizes

        If Not [Enum].TryParse(Name, Size) Then
            Size = 0
        End If

        Me.Add(New HexViewHighlightRegion(Start + Offset, Size, ForeColor, DiskImage.DirectorytEntryLFNDescription(Offset)))
    End Sub

    Public Sub AddDirectoryEntryOffset(Start As Long, Offset As DiskImage.DirectoryEntry.DirectoryEntryOffsets, ForeColor As Color)
        Dim Name As String = [Enum].GetName(GetType(DiskImage.DirectoryEntry.DirectoryEntryOffsets), Offset)
        Dim Size As DiskImage.DirectoryEntry.DirectoryEntrySizes

        If Not [Enum].TryParse(Name, Size) Then
            Size = 0
        End If

        Me.Add(New HexViewHighlightRegion(Start + Offset, Size, ForeColor, DiskImage.DirectorytEntryDescription(Offset)))
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
