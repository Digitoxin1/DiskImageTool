Imports System.Text.RegularExpressions
Imports DiskImageTool.Bitstream
Imports DiskImageTool.Bitstream.IBM_MFM
Imports DiskImageTool.DiskImage
Imports DiskImageTool.Hb.Windows.Forms
Imports DiskImageTool.HexView

Partial Public Class HexViewRawForm
    Private ReadOnly _Changes As New Stack(Of List(Of RawHexChange))
    Private ReadOnly _RedoChanges As New Stack(Of List(Of RawHexChange))
    Private ReadOnly _UpdatedTracks As New HashSet(Of Point)
    Private _TracksUpdated As Boolean = False

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

#Region "Data Area Editing"

    ''' <summary>
    ''' True when one or more tracks were edited in this session and re-synced on close, so
    ''' the caller can refresh the decoded views (tree/summary/hex).
    ''' </summary>
    Public ReadOnly Property TracksUpdated As Boolean
        Get
            Return _TracksUpdated
        End Get
    End Property

    ''' <summary>
    ''' On close, re-decode every edited track from its (already updated) bitstream and rebuild
    ''' the image's decoded sector map so edits are reflected in the rest of the application.
    ''' </summary>
    Private Sub HexViewRawForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        If _UpdatedTracks.Count = 0 Then
            Exit Sub
        End If

        Dim BitstreamImage = _FloppyImage.BitstreamImage

        For Each P In _UpdatedTracks
            Dim BT = BitstreamImage.GetTrack(CUShort(P.X * BitstreamImage.TrackStep), CByte(P.Y))
            If BT IsNot Nothing Then
                BT.MFMData = New IBM_MFM_Track(BT.Bitstream)
            End If
        Next

        If TypeOf _FloppyImage Is MappedFloppyImage Then
            CType(_FloppyImage, MappedFloppyImage).RebuildSectorMap()
        End If

        _TracksUpdated = True
    End Sub

    ''' <summary>
    ''' Returns the sector whose data area contains the given hex byte index and can be
    ''' edited (Debug only, MFM track, valid non-overlapping data checksum), otherwise Nothing.
    ''' </summary>
    Private Function GetEditableSector(Index As Long) As BitstreamRegionSector
        If Not App.AppSettings.Debug Then
            Return Nothing
        End If

        If _TrackType <> BitstreamTrackType.MFM Then
            Return Nothing
        End If

        If _RegionMap Is Nothing Then
            Return Nothing
        End If

        If Index < 0 OrElse Index > _RegionMap.Length - 1 Then
            Return Nothing
        End If

        Dim Region = _RegionMap(Index)
        If Region Is Nothing OrElse Region.RegionType <> MFMRegionType.DataArea Then
            Return Nothing
        End If

        Dim Sector = Region.Sector
        If Sector Is Nothing OrElse Not Sector.DataChecksumValid OrElse Sector.Overlaps Then
            Return Nothing
        End If

        Return Sector
    End Function

    ''' <summary>
    ''' Encodes a single byte into the bitstream at the given bit index using standard MFM
    ''' rules. The first clock bit is derived from the previous byte's last data bit (read
    ''' from the stream), and the following byte's first clock bit is recomputed so it stays
    ''' consistent with this byte's last data bit.
    ''' </summary>
    Private Sub WriteMFMByteAt(Bitstream As BitArray, BitIndex As Integer, Value As Byte)
        Dim Length = Bitstream.Length

        BitIndex = AdjustBitIndex(BitIndex, Length)

        Dim SeedBit = Bitstream(AdjustBitIndex(BitIndex - 1, Length))
        Dim Encoded = MFMEncodeBytes({Value}, SeedBit)

        For k = 0 To 15
            Bitstream(AdjustBitIndex(BitIndex + k, Length)) = Encoded(k)
        Next

        ' The following byte's first clock bit depends on this byte's last data bit.
        Dim NextClock = AdjustBitIndex(BitIndex + 16, Length)
        Dim NextData = AdjustBitIndex(BitIndex + 17, Length)
        Bitstream(NextClock) = (Not Bitstream(NextData)) And (Not Encoded(15))
    End Sub

    Private Sub HexBox1_ByteChanged(source As Object, e As HexBox.ByteChangedArgs) Handles HexBox1.ByteChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        Dim Sector = GetEditableSector(e.Index)
        If Sector Is Nothing Then
            Exit Sub
        End If

        Dim Offset = _CurrentTrackData.Offset
        Dim Length = _Bitstream.Length

        ' Record the original bytes affected by this edit so it can be undone.
        Dim ChangeList As New List(Of RawHexChange) From {
            New RawHexChange(e.Index, {e.PrevValue}, HexBox1.SelectionStart, HexBox1.SelectionLength)
        }

        ' Re-encode the edited data byte into the working-clone bitstream
        ' (the decoded value is already in _Data via the shared provider).
        Dim ByteBit = AdjustBitIndex(e.Index * 16 + Offset, Length)
        WriteMFMByteAt(_Bitstream, ByteBit, e.Value)

        ' Recompute the data CRC over the decoded [A1 A1 A1, FB, data...] bytes.
        Dim DataBit = AdjustBitIndex(Sector.DataStartIndex * 16 + Offset, Length)
        Dim SyncBit = AdjustBitIndex(DataBit - MFM_SYNC_MARK_BYTES * 16, Length)
        Dim Buffer = MFMGetBytes(_Bitstream, SyncBit, Sector.AdjustedDataLength + MFM_SYNC_MARK_BYTES)
        Dim CsBytes = BitConverter.GetBytes(MFMCRC16(Buffer))

        ' Overwrite the two checksum bytes so the sector stays checksum-valid.
        Dim Cs1 = Sector.DataStartIndex + Sector.AdjustedDataLength
        ChangeList.Add(New RawHexChange(Cs1, {_Data(Cs1)}, HexBox1.SelectionStart, HexBox1.SelectionLength))
        ChangeList.Add(New RawHexChange(Cs1 + 1, {_Data(Cs1 + 1)}, HexBox1.SelectionStart, HexBox1.SelectionLength))
        WriteMFMByteAt(_Bitstream, AdjustBitIndex(Cs1 * 16 + Offset, Length), CsBytes(0))
        WriteMFMByteAt(_Bitstream, AdjustBitIndex((Cs1 + 1) * 16 + Offset, Length), CsBytes(1))

        ' Reflect the recomputed checksum bytes in the display (and _Data via the shared provider).
        _IgnoreEvent = True
        HexBox1.ByteProvider.WriteByte(Cs1, CsBytes(0))
        HexBox1.ByteProvider.WriteByte(Cs1 + 1, CsBytes(1))
        _IgnoreEvent = False

        PushChanges(ChangeList)

        ' Bit-inspector now reflects the staged clone.
        RefreshBits(_Bitstream, DataRowEnum.Bitstream, True)
    End Sub

    Private Sub HexBox1_InsertActiveChanged(sender As Object, e As EventArgs) Handles HexBox1.InsertActiveChanged
        HexBox1.InsertActive = False
    End Sub

    ''' <summary>
    ''' Re-encodes the byte at the given _Data index into the working-clone bitstream so the
    ''' clone (and the bit-inspector) stays consistent with the decoded data buffer.
    ''' </summary>
    Private Sub ReEncodeByte(Index As Integer)
        Dim Offset = _CurrentTrackData.Offset
        Dim Length = _Bitstream.Length

        WriteMFMByteAt(_Bitstream, AdjustBitIndex(Index * 16 + Offset, Length), _Data(Index))
    End Sub

    ''' <summary>
    ''' Pushes an edit onto the undo stack, clears the redo stack, and refreshes the buttons.
    ''' </summary>
    Private Sub PushChanges(ChangeList As List(Of RawHexChange))
        _Changes.Push(ChangeList)
        _RedoChanges.Clear()
        RefreshUndoButtons()
    End Sub

    ''' <summary>
    ''' Applies one undo/redo step: restores the stored bytes into _Data, re-encodes them into
    ''' the working-clone bitstream, and records the inverse on the destination stack.
    ''' </summary>
    Private Sub PopChange(Source As Stack(Of List(Of RawHexChange)), Destination As Stack(Of List(Of RawHexChange)))
        If Source.Count = 0 Then
            Exit Sub
        End If

        Dim ChangeList = Source.Pop()
        Dim DestinationList As New List(Of RawHexChange)

        _IgnoreEvent = True
        For Each Change In ChangeList
            ' Capture the current value so the operation can be reversed.
            DestinationList.Add(New RawHexChange(Change.Index, {_Data(Change.Index)}, Change.SelectionStart, Change.SelectionLength))

            ' Restore the stored bytes into the decoded display and the working-clone bitstream.
            For Counter = 0 To Change.Data.Length - 1
                HexBox1.ByteProvider.WriteByte(Change.Index + Counter, Change.Data(Counter))
                ReEncodeByte(Change.Index + Counter)
            Next
        Next
        _IgnoreEvent = False

        Destination.Push(DestinationList)

        ' Restore the selection recorded with the first change in the list.
        HexBox1.Select(ChangeList(0).SelectionStart, ChangeList(0).SelectionLength)

        RefreshUndoButtons()
        RefreshBits(_Bitstream, DataRowEnum.Bitstream, True)
        DataInspectorRefresh(True)
    End Sub

    ''' <summary>
    ''' Enables/disables the undo, redo, and commit toolbar buttons based on the stack state.
    ''' </summary>
    Private Sub RefreshUndoButtons()
        ToolStripBtnUndo.Enabled = _Changes.Count > 0
        ToolStripBtnRedo.Enabled = _RedoChanges.Count > 0
        ToolStripBtnCommit.Enabled = _Changes.Count > 0
    End Sub

    ''' <summary>
    ''' Commits the staged edits by assigning the working-clone bitstream to the live track,
    ''' marking the track for re-sync on close, and clearing the undo/redo stacks.
    ''' </summary>
    Private Sub ApplyStagedChanges()
        Dim BitstreamImage = _FloppyImage.BitstreamImage
        Dim BT = BitstreamImage.GetTrack(CUShort(_Track * BitstreamImage.TrackStep), CByte(_Side))
        If BT IsNot Nothing Then
            BT.Bitstream = _Bitstream
        End If

        _UpdatedTracks.Add(New Point(_Track, _Side))

        _Changes.Clear()
        _RedoChanges.Clear()
        RefreshUndoButtons()
    End Sub

    ''' <summary>
    ''' Commits staged edits to the in-memory track and reloads so all derived displays reflect
    ''' the committed bitstream. Optionally closes the form afterwards.
    ''' </summary>
    Private Sub CommitChanges(CloseAfterCommit As Boolean)
        If _Changes.Count > 0 Then
            ApplyStagedChanges()
            LoadTrack(_CurrentTrackData, True, True)
        End If

        If CloseAfterCommit Then
            Me.Close()
        End If
    End Sub

    ''' <summary>
    ''' When uncommitted edits exist, prompts to commit, discard, or cancel. Returns False only
    ''' when the user cancels (so the caller can abort navigation/close).
    ''' </summary>
    Private Function ConfirmCommitOrDiscard() As Boolean
        If _Changes.Count = 0 Then
            Return True
        End If

        Dim Msg = String.Format(My.Resources.Dialog_CommitChangesTrack, Environment.NewLine)
        Dim Response = MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNoCancel + MsgBoxStyle.DefaultButton3)

        If Response = MsgBoxResult.Cancel Then
            Return False
        ElseIf Response = MsgBoxResult.Yes Then
            ApplyStagedChanges()
        Else
            ' Discard: drop the staged edits (the working clone is abandoned on the next LoadTrack).
            _Changes.Clear()
            _RedoChanges.Clear()
            RefreshUndoButtons()
        End If

        Return True
    End Function

    Private Sub InitEditingButtons() Handles Me.Load
        RefreshUndoButtons()
    End Sub

    Private Sub ToolStripBtnCommit_Click(sender As Object, e As EventArgs) Handles ToolStripBtnCommit.Click
        CommitChanges(False)
    End Sub

    Private Sub ToolStripBtnUndo_Click(sender As Object, e As EventArgs) Handles ToolStripBtnUndo.Click
        PopChange(_Changes, _RedoChanges)
    End Sub

    Private Sub ToolStripBtnRedo_Click(sender As Object, e As EventArgs) Handles ToolStripBtnRedo.Click
        PopChange(_RedoChanges, _Changes)
    End Sub

    Private Sub HexViewRawForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If _Changes.Count = 0 Then
            Exit Sub
        End If

        Dim Msg = String.Format(My.Resources.Dialog_CommitChanges, Environment.NewLine)
        Dim Response = MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNoCancel + MsgBoxStyle.DefaultButton3)

        If Response = MsgBoxResult.Cancel Then
            e.Cancel = True
        ElseIf Response = MsgBoxResult.Yes Then
            ApplyStagedChanges()
        End If
    End Sub

    ''' <summary>
    ''' Overwrite-only byte provider that wraps a byte array by reference, so edits stay in
    ''' sync with the form's decoded data buffer. Insert/delete are unsupported, enforcing
    ''' overwrite-only editing in the raw hex view.
    ''' </summary>
    Private Class SharedByteProvider
        Implements IByteProvider

        Private ReadOnly _Bytes() As Byte

        Public Sub New(Bytes() As Byte)
            _Bytes = Bytes
        End Sub

        Public Event Changed As EventHandler Implements IByteProvider.Changed
        Public Event LengthChanged As EventHandler Implements IByteProvider.LengthChanged

        Public ReadOnly Property Length As Long Implements IByteProvider.Length
            Get
                Return _Bytes.Length
            End Get
        End Property

        Public Sub ApplyChanges() Implements IByteProvider.ApplyChanges
        End Sub

        Public Sub DeleteBytes(index As Long, length As Long) Implements IByteProvider.DeleteBytes
            'Not supported - overwrite only
        End Sub

        Public Function HasChanges() As Boolean Implements IByteProvider.HasChanges
            Return False
        End Function

        Public Sub InsertBytes(index As Long, bs() As Byte) Implements IByteProvider.InsertBytes
            'Not supported - overwrite only
        End Sub

        Public Function ReadByte(index As Long) As Byte Implements IByteProvider.ReadByte
            Return _Bytes(index)
        End Function

        Public Function SupportsDeleteBytes() As Boolean Implements IByteProvider.SupportsDeleteBytes
            Return False
        End Function

        Public Function SupportsInsertBytes() As Boolean Implements IByteProvider.SupportsInsertBytes
            Return False
        End Function

        Public Function SupportsWriteByte() As Boolean Implements IByteProvider.SupportsWriteByte
            Return True
        End Function

        Public Sub WriteByte(index As Long, value As Byte) Implements IByteProvider.WriteByte
            _Bytes(index) = value
            RaiseEvent Changed(Me, EventArgs.Empty)
        End Sub
    End Class

    ''' <summary>
    ''' A single staged edit for undo/redo: the original bytes at a given _Data index plus the
    ''' selection to restore when the change is applied.
    ''' </summary>
    Private Class RawHexChange
        Public Sub New(Index As Integer, Data() As Byte, SelectionStart As Long, SelectionLength As Long)
            Me.Index = Index
            Me.Data = Data
            Me.SelectionStart = SelectionStart
            Me.SelectionLength = SelectionLength
        End Sub

        Public Property Index As Integer
        Public Property Data As Byte()
        Public Property SelectionStart As Long
        Public Property SelectionLength As Long
    End Class

#End Region
End Class
