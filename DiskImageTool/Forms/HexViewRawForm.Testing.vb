Imports System.Text.RegularExpressions
Imports DiskImageTool.Bitstream.IBM_MFM

Partial Public Class HexViewRawForm
    Private Sub AddContextMenuBitEditItems()
        ContextMenuStrip1.Items.Add(New ToolStripSeparator())

        Dim Item = New ToolStripMenuItem("Edit Bits")
        AddHandler Item.Click, AddressOf ContextMenuEditBits_Click
        ContextMenuStrip1.Items.Add(Item)
    End Sub

    Private Sub ContextMenuEditBits_Click()
        Dim SelectionStart = HexBox1.SelectionStart

        Dim Bits = GetBits(_Bitstream, SelectionStart, False)

        Dim Value = InputBox("Edit bits: ", "Edit Bits", Bits)
        Value = Value.Replace(" ", "")

        If Not Regex.IsMatch(Value, "^(0|1){16}$") Then
            Exit Sub
        End If

        Dim BitIndex = SelectionStart * 16 + _CurrentTrackData.Offset
        BitIndex = AdjustBitIndex(BitIndex, _Bitstream.Length)

        For counter = 0 To Value.Length - 1
            _Bitstream.Set(BitIndex + counter, Value.Substring(counter, 1) = 1)
        Next

        Dim MFMTrack = _FloppyImage.BitstreamImage.GetTrack(_CurrentTrackData.Track * _FloppyImage.BitstreamImage.TrackStep, _CurrentTrackData.Side)
        MFMTrack.Bitstream = _Bitstream

        LoadTrack(_CurrentTrackData, True, True)
    End Sub
End Class
