Imports System.Text.RegularExpressions
Imports DiskImageTool.Bitstream
Imports DiskImageTool.Bitstream.IBM_MFM

Partial Public Class HexViewRawForm
    Private Sub AddContextMenuBitEditItems()
        ContextMenuStrip1.Items.Add(New ToolStripSeparator())

        Dim Item As New ToolStripMenuItem("Rotate All Tracks")
        AddHandler Item.Click, AddressOf ContextMenuRotateAllTracks_Click
        ContextMenuStrip1.Items.Add(Item)

        Item = New ToolStripMenuItem("Normalize First Gap")
        AddHandler Item.Click, AddressOf ContextMenuNormalizeFirstGap_Click
        ContextMenuStrip1.Items.Add(Item)

        Item = New ToolStripMenuItem("Insert Gap")
        AddHandler Item.Click, AddressOf ContextMenuInsertGap_Click
        ContextMenuStrip1.Items.Add(Item)

        Item = New ToolStripMenuItem("Insert Bits")
        AddHandler Item.Click, AddressOf ContextMenuInsertBits_Click
        ContextMenuStrip1.Items.Add(Item)

        Item = New ToolStripMenuItem("Remove Bits")
        AddHandler Item.Click, AddressOf ContextMenuRemoveBits_Click
        ContextMenuStrip1.Items.Add(Item)

        Item = New ToolStripMenuItem("Edit Bits")
        AddHandler Item.Click, AddressOf ContextMenuEditBits_Click
        ContextMenuStrip1.Items.Add(Item)

        Item = New ToolStripMenuItem("Pad All Tracks")
        AddHandler Item.Click, AddressOf ContextMenuPadAllTracks_Click
        ContextMenuStrip1.Items.Add(Item)

        Item = New ToolStripMenuItem("Gap to End")
        AddHandler Item.Click, AddressOf ContextMenuGapToEnd_Click
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

    Private Sub ContextMenuGapToEnd_Click()
        Const GapBits As String = "1001001001010100"

        Dim selectionStart = HexBox1.SelectionStart

        ' Bit index where we insert
        Dim bitIndex = selectionStart * 16 + _CurrentTrackData.Offset
        bitIndex = AdjustBitIndex(bitIndex, _Bitstream.Length)

        Dim MFMTrack = _FloppyImage.BitstreamImage.GetTrack(_CurrentTrackData.Track * _FloppyImage.BitstreamImage.TrackStep, _CurrentTrackData.Side)

        For i = bitIndex To MFMTrack.Bitstream.Length - 1
            Dim b As Boolean = GapBits((i - bitIndex) Mod 16) = "1"
            MFMTrack.Bitstream.Set(i, b)
        Next

        LoadTrack(_CurrentTrackData, True, True)
    End Sub

    Private Sub ContextMenuInsertBits_Click()
        Dim selectionStart = HexBox1.SelectionStart

        ' Empty default text
        Dim value = InputBox("Insert bits: ", "Insert Bits", "")
        value = value.Replace(" ", "")

        ' Must be at least 1 bit, only 0 or 1
        If String.IsNullOrEmpty(value) OrElse Not Regex.IsMatch(value, "^[01]+$") Then
            Exit Sub
        End If

        ' Bit index where we insert
        Dim bitIndex = selectionStart * 16 + _CurrentTrackData.Offset
        bitIndex = AdjustBitIndex(bitIndex, _Bitstream.Length)

        ' Build a BitArray from the entered bits
        Dim bitsToInsert As New BitArray(value.Length)
        For i = 0 To value.Length - 1
            bitsToInsert(i) = (value(i) = "1")
        Next

        ' Insert into the bitstream
        _Bitstream = InsertBits(_Bitstream, bitIndex, bitsToInsert)

        ' Push updated bitstream back into the track and refresh UI
        Dim MFMTrack = _FloppyImage.BitstreamImage.GetTrack(_CurrentTrackData.Track * _FloppyImage.BitstreamImage.TrackStep, _CurrentTrackData.Side)
        MFMTrack.Bitstream = _Bitstream

        LoadTrack(_CurrentTrackData, True, True)
    End Sub

    Private Sub ContextMenuInsertGap_Click()
        Const GapBits As String = "1001001001010100"

        Dim selectionStart = HexBox1.SelectionStart

        Dim value = InputBox("Insert gap: ", "Gap Count", "")

        ' Must be an integer
        If String.IsNullOrEmpty(value) OrElse Not IsNumeric(value) OrElse value <> Int(value) Then
            Exit Sub
        End If

        ' Bit index where we insert
        Dim bitIndex = selectionStart * 16 + _CurrentTrackData.Offset
        bitIndex = AdjustBitIndex(bitIndex, _Bitstream.Length)

        ' Build a BitArray from the entered bits
        Dim bitsToInsert As New BitArray(GapBits.Length * CInt(value))
        For i As Integer = 0 To value - 1
            For j = 0 To GapBits.Length - 1
                Dim idx = i * GapBits.Length + j
                bitsToInsert(idx) = (GapBits(j) = "1")
            Next
        Next

        ' Insert into the bitstream
        _Bitstream = InsertBits(_Bitstream, bitIndex, bitsToInsert)

        ' Push updated bitstream back into the track and refresh UI
        Dim MFMTrack = _FloppyImage.BitstreamImage.GetTrack(_CurrentTrackData.Track * _FloppyImage.BitstreamImage.TrackStep, _CurrentTrackData.Side)
        MFMTrack.Bitstream = _Bitstream

        LoadTrack(_CurrentTrackData, True, True)
    End Sub

    Private Sub ContextMenuNormalizeFirstGap_Click()
        Dim GapBits = New BitArray({True, False, False, True, False, False, True, False, False, True, False, True, False, True, False, False})

        Dim Value = InputBox("Normalize First Gap: ", "Gap Size")

        ' Must be an integer
        If String.IsNullOrEmpty(Value) OrElse Not IsNumeric(Value) OrElse Value <> Int(Value) OrElse Int(Value) < 0 Then
            Exit Sub
        End If

        For i = 0 To _FloppyImage.TrackCount - 1
            For j = 0 To _FloppyImage.SideCount - 1
                Dim MFMTrack = _FloppyImage.BitstreamImage.GetTrack(i * _FloppyImage.BitstreamImage.TrackStep, j)
                If MFMTrack.TrackType = BitstreamTrackType.MFM Then
                    Dim RegionData = MFMGetRegionList(MFMTrack.Bitstream, MFMTrack.TrackType)
                    Dim GapSize As UShort = RegionData.Gap4A
                    If GapSize = 0 Then
                        GapSize = RegionData.Gap1
                    End If
                    Dim Diff = CInt(Value) - GapSize
                    If Diff <> 0 Then
                        Dim NewGapBits = RepeatBitArray(GapBits, Math.Abs(Diff))
                        If Diff > 0 Then
                            MFMTrack.Bitstream = InsertBits(MFMTrack.Bitstream, 0, NewGapBits)
                            MFMTrack.Bitstream.Length = MFMTrack.Bitstream.Length - NewGapBits.Length
                        Else
                            MFMTrack.Bitstream = RemoveBits(MFMTrack.Bitstream, 0, NewGapBits.Length)
                            MFMTrack.Bitstream = InsertBits(MFMTrack.Bitstream, MFMTrack.Bitstream.Length, NewGapBits)
                        End If
                    End If
                End If
            Next
        Next

        LoadTrack(_CurrentTrackData, True, True)
    End Sub

    Private Sub ContextMenuPadAllTracks_Click()
        Const GapBits As String = "1001001001010100"

        Dim selectionStart = HexBox1.SelectionStart

        Dim value = InputBox("Pad All Tracks: ", "Track Size (Bits)", "")

        ' Must be an integer
        If String.IsNullOrEmpty(value) OrElse Not IsNumeric(value) OrElse value <> Int(value) Then
            Exit Sub
        End If

        For i = 0 To _FloppyImage.TrackCount - 1
            For j = 0 To _FloppyImage.SideCount - 1
                Dim MFMTrack = _FloppyImage.BitstreamImage.GetTrack(i * _FloppyImage.BitstreamImage.TrackStep, j)
                Dim PrevLength = MFMTrack.Bitstream.Length
                PrevLength = Math.Ceiling(PrevLength / 16) * 16
                MFMTrack.Bitstream.Length = CInt(value)
                Dim FillLength = MFMTrack.Bitstream.Length - PrevLength
                For k = 0 To FillLength - 1
                    Dim b As Boolean = GapBits(k Mod 16) = "1"
                    MFMTrack.Bitstream.Set(PrevLength + k, b)
                Next
            Next
        Next

        LoadTrack(_CurrentTrackData, True, True)
    End Sub

    Private Sub ContextMenuRemoveBits_Click()
        Dim SelectionStart = HexBox1.SelectionStart

        Dim BitIndex = SelectionStart * 16 + _CurrentTrackData.Offset
        BitIndex = AdjustBitIndex(BitIndex, _Bitstream.Length)

        Dim RegionStart = _RegionMap(SelectionStart)

        Dim Value = InputBox("Number of bits to remove: ", "Remove Bits", RegionStart.BitOffset.ToString)
        Dim Offset As UInteger
        If Not UInteger.TryParse(Value, Offset) Then
            Exit Sub
        End If

        _Bitstream = RemoveBits(_Bitstream, BitIndex, Offset)

        Dim MFMTrack = _FloppyImage.BitstreamImage.GetTrack(_CurrentTrackData.Track * _FloppyImage.BitstreamImage.TrackStep, _CurrentTrackData.Side)
        MFMTrack.Bitstream = _Bitstream

        LoadTrack(_CurrentTrackData, True, True)
    End Sub

    Private Sub ContextMenuRotateAllTracks_Click()
        For i = 0 To _FloppyImage.TrackCount - 1
            For j = 0 To _FloppyImage.SideCount - 1
                Dim MFMTrack = _FloppyImage.BitstreamImage.GetTrack(i * _FloppyImage.BitstreamImage.TrackStep, j)
                If MFMTrack.TrackType = BitstreamTrackType.MFM Then
                    Dim Offset = MFMGetOffset(MFMTrack.Bitstream)
                    If Offset > 0 Then
                        MFMTrack.Bitstream = BitstreamAlign(MFMTrack.Bitstream, Offset)
                    End If
                End If
            Next
        Next

        For Each Track As TrackData In ComboTrack.Items
            Track.Offset = -1
        Next

        LoadTrack(_CurrentTrackData, True, True)
    End Sub

    Private Function InsertBits(source As BitArray, index As Integer, bitsToInsert As BitArray) As BitArray
        If index < 0 OrElse index > source.Length Then
            Throw New ArgumentOutOfRangeException(NameOf(index))
        End If

        Dim insertCount = bitsToInsert.Length
        Dim result As New BitArray(source.Length + insertCount)

        Dim destPos As Integer = 0

        ' Copy bits before insertion point
        For i = 0 To index - 1
            result(destPos) = source(i)
            destPos += 1
        Next

        ' Copy inserted bits
        For i = 0 To insertCount - 1
            result(destPos) = bitsToInsert(i)
            destPos += 1
        Next

        ' Copy bits after insertion point
        For i = index To source.Length - 1
            result(destPos) = source(i)
            destPos += 1
        Next

        Return result
    End Function

    Private Function RemoveBits(source As BitArray, index As Integer, count As Integer) As BitArray
        Dim newLength = source.Length - count
        Dim result As New BitArray(newLength)

        Dim destPos As Integer = 0

        ' Copy before the removed slice
        For i = 0 To index - 1
            result(destPos) = source(i)
            destPos += 1
        Next

        ' Skip the removed bits and copy the rest
        For i = index + count To source.Length - 1
            result(destPos) = source(i)
            destPos += 1
        Next

        Return result
    End Function

    Private Function RepeatBitArray(source As BitArray, count As UInteger) As BitArray
        If count = 0 OrElse source.Length = 0 Then
            Return New BitArray(0)
        End If

        Dim result As New BitArray(source.Length * count)

        For repeatIndex As Integer = 0 To count - 1
            Dim offset As Integer = repeatIndex * source.Length

            For bitIndex As Integer = 0 To source.Length - 1
                result(offset + bitIndex) = source(bitIndex)
            Next
        Next

        Return result
    End Function
End Class
