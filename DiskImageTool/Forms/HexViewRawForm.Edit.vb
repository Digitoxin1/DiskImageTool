Imports DiskImageTool.Bitstream
Imports DiskImageTool.Bitstream.IBM_MFM
Imports DiskImageTool.DiskImage
Imports DiskImageTool.Hb.Windows.Forms
Imports DiskImageTool.HexView

Partial Public Class HexViewRawForm
    Private ReadOnly _Changes As New Stack(Of List(Of RawHexChange))
    Private ReadOnly _OriginalBitstreams As New Dictionary(Of Point, BitArray)
    Private ReadOnly _RedoChanges As New Stack(Of List(Of RawHexChange))
    Private _TracksUpdated As Boolean = False

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
    ''' Commits the staged edits by assigning the working-clone bitstream to the live track,
    ''' marking the track for re-sync on close, and clearing the undo/redo stacks.
    ''' </summary>
    Private Sub ApplyStagedChanges()
        Dim BitstreamImage = _FloppyImage.BitstreamImage
        Dim BT = BitstreamImage.GetTrack(CUShort(_Track * BitstreamImage.TrackStep), CByte(_Side))
        If BT IsNot Nothing Then
            Dim Key As New Point(_Track, _Side)
            If Not _OriginalBitstreams.ContainsKey(Key) Then
                _OriginalBitstreams(Key) = CType(BT.Bitstream.Clone(), BitArray)
            End If
            BT.Bitstream = _Bitstream
        End If

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

    ''' <summary>
    ''' Returns the region at the given hex byte index if it can be edited (Debug only, MFM
    ''' track, current bit offset): a gap, a valid non-overlapping ID field, or a valid
    ''' non-overlapping data area.
    ''' </summary>
    Private Function GetEditableRegion(Index As Long) As BitstreamRegion
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
        If Region Is Nothing Then
            Return Nothing
        End If

        If _CurrentTrackData Is Nothing OrElse Region.BitOffset <> _CurrentTrackData.Offset Then
            Return Nothing
        End If

        If IsGapRegion(Region.RegionType) Then
            Return Region
        End If

        If IsNullRegion(Region.RegionType) Then
            Return Region
        End If

        Dim Sector = Region.Sector
        If Sector Is Nothing OrElse Sector.Overlaps Then
            Return Nothing
        End If

        If IsIDAreaRegion(Region.RegionType) Then
            If Sector.IDAMChecksumValid Then
                Return Region
            End If
            Return Nothing
        End If

        If Region.RegionType = MFMRegionType.DataArea AndAlso Sector.DataChecksumValid Then
            Return Region
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' Hex-view index of the first ID-field byte (cylinder) for this ID-area region.
    ''' Sector.StartIndex cannot be used here: it includes IDAM nulls and is not the hex
    ''' index of the C/H/R/N field.
    ''' </summary>
    Private Function GetIDAreaStartIndex(Region As BitstreamRegion) As Long
        Select Case Region.RegionType
            Case MFMRegionType.IDAreaHead
                Return Region.StartIndex - 1
            Case MFMRegionType.IDAreaSectorId
                Return Region.StartIndex - 2
            Case MFMRegionType.IDAreaSizeId
                Return Region.StartIndex - 3
            Case Else
                Return Region.StartIndex
        End Select
    End Function

    Private Function IsGapRegion(RegionType As MFMRegionType) As Boolean
        Select Case RegionType
            Case MFMRegionType.Gap1, MFMRegionType.Gap2, MFMRegionType.Gap3, MFMRegionType.Gap4A, MFMRegionType.Gap4B
                Return True
            Case Else
                Return False
        End Select
    End Function

    Private Function IsIDAreaRegion(RegionType As MFMRegionType) As Boolean
        Select Case RegionType
            Case MFMRegionType.IDArea, MFMRegionType.IDAreaCylinder, MFMRegionType.IDAreaHead, MFMRegionType.IDAreaSectorId, MFMRegionType.IDAreaSizeId
                Return True
            Case Else
                Return False
        End Select
    End Function

    Private Function IsNullRegion(RegionType As MFMRegionType) As Boolean
        Select Case RegionType
            Case MFMRegionType.DAMNulls, MFMRegionType.IDAMNulls, MFMRegionType.IAMNulls
                Return True
            Case Else
                Return False
        End Select
    End Function

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
    ''' Pushes an edit onto the undo stack, clears the redo stack, and refreshes the buttons.
    ''' </summary>
    Private Sub PushChanges(ChangeList As List(Of RawHexChange))
        _Changes.Push(ChangeList)
        _RedoChanges.Clear()
        RefreshUndoButtons()
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
    ''' Overwrites the editable region at the caret with clipboard hex, truncated at the
    ''' region end, as a single undo step with one CRC refresh.
    ''' </summary>
    Private Sub PasteHex()
        Dim HexBytes = ConvertHexToBytes(Clipboard.GetText)
        If HexBytes Is Nothing Then
            Exit Sub
        End If

        Dim Offset = HexBox1.SelectionStart
        Dim Region = GetEditableRegion(Offset)
        If Region Is Nothing Then
            Exit Sub
        End If

        Dim MaxLength = CInt(Region.StartIndex + Region.Length - Offset)
        If MaxLength <= 0 Then
            Exit Sub
        End If

        Dim Length = HexBytes.Length
        If Length > MaxLength Then
            Length = MaxLength
        End If
        If Offset + Length > HexBox1.ByteProvider.Length Then
            Length = CInt(HexBox1.ByteProvider.Length - Offset)
        End If
        If Length <= 0 Then
            Exit Sub
        End If

        Dim Original(Length - 1) As Byte
        Dim Modified = False

        _IgnoreEvent = True
        Try
            For i = 0 To Length - 1
                Dim Index = CInt(Offset + i)
                Original(i) = _Data(Index)
                If Original(i) <> HexBytes(i) Then
                    HexBox1.ByteProvider.WriteByte(Index, HexBytes(i))
                    ReEncodeByte(Index)
                    Modified = True
                End If
            Next

            If Modified Then
                Dim ChangeList As New List(Of RawHexChange) From {
                    New RawHexChange(CInt(Offset), Original, Offset, Length)
                }
                StageRegionChecksum(ChangeList, Region)
                PushChanges(ChangeList)
            End If
        Finally
            _IgnoreEvent = False
        End Try

        If Modified Then
            HexBox1.SelectionLength = Length
            HexBox1.Invalidate()
            RefreshBits(_Bitstream, DataRowEnum.Bitstream, True)
            DataInspectorRefresh(True)
        End If
    End Sub

    ''' <summary>
    ''' Overwrites the current selection with a constant byte as a single undo step with one
    ''' CRC refresh. The selection must already lie inside one editable region.
    ''' </summary>
    Private Sub FillSelected(Value As Byte)
        If Not CanFillSelection() Then
            Exit Sub
        End If

        Dim Offset = HexBox1.SelectionStart
        Dim Length = CInt(HexBox1.SelectionLength)
        Dim Region = GetEditableRegion(Offset)
        If Region Is Nothing OrElse Length <= 0 Then
            Exit Sub
        End If

        Dim Original(Length - 1) As Byte
        Dim Modified = False

        _IgnoreEvent = True
        Try
            For i = 0 To Length - 1
                Dim Index = CInt(Offset + i)
                Original(i) = _Data(Index)
                If Original(i) <> Value Then
                    HexBox1.ByteProvider.WriteByte(Index, Value)
                    ReEncodeByte(Index)
                    Modified = True
                End If
            Next

            If Modified Then
                Dim ChangeList As New List(Of RawHexChange) From {
                    New RawHexChange(CInt(Offset), Original, Offset, Length)
                }
                StageRegionChecksum(ChangeList, Region)
                PushChanges(ChangeList)
            End If
        Finally
            _IgnoreEvent = False
        End Try

        If Modified Then
            HexBox1.Invalidate()
            RefreshBits(_Bitstream, DataRowEnum.Bitstream, True)
            DataInspectorRefresh(True)
        End If
    End Sub

    ''' <summary>
    ''' Enables/disables the undo, redo, and commit toolbar buttons based on the stack state.
    ''' </summary>
    Private Sub RefreshUndoButtons()
        ToolStripBtnUndo.Enabled = _Changes.Count > 0
        ToolStripBtnRedo.Enabled = _RedoChanges.Count > 0
        ToolStripBtnCommit.Enabled = _Changes.Count > 0
    End Sub

    Private Sub RefreshPasteButton()
        Dim Enabled = ClipboardHasHex() AndAlso GetEditableRegion(HexBox1.SelectionStart) IsNot Nothing
        BtnPaste.Enabled = Enabled
        ToolStripBtnPaste.Enabled = Enabled
    End Sub

    ''' <summary>
    ''' Recalculates ID or data CRC for the edited region and stages the checksum bytes.
    ''' </summary>
    Private Sub StageRegionChecksum(ChangeList As List(Of RawHexChange), Region As BitstreamRegion)
        Dim Offset = _CurrentTrackData.Offset
        Dim Length = _Bitstream.Length

        If IsIDAreaRegion(Region.RegionType) Then
            Dim IdStart = GetIDAreaStartIndex(Region)
            Dim IdBit = AdjustBitIndex(IdStart * 16 + Offset, Length)
            Dim SyncBit = AdjustBitIndex(IdBit - MFM_SYNC_MARK_BYTES * 16, Length)
            Dim Buffer = MFMGetBytes(_Bitstream, SyncBit, MFM_SYNC_MARK_BYTES + MFM_IDAREA_BYTES)
            Dim CsBytes = BitConverter.GetBytes(MFMCRC16(Buffer))
            Dim Cs1 = IdStart + MFM_IDAREA_BYTES
            StageChecksumBytes(ChangeList, Cs1, CsBytes, Offset, Length)
        ElseIf Region.RegionType = MFMRegionType.DataArea Then
            Dim Sector = Region.Sector
            Dim DataBit = AdjustBitIndex(Sector.DataStartIndex * 16 + Offset, Length)
            Dim SyncBit = AdjustBitIndex(DataBit - MFM_SYNC_MARK_BYTES * 16, Length)
            Dim Buffer = MFMGetBytes(_Bitstream, SyncBit, Sector.AdjustedDataLength + MFM_SYNC_MARK_BYTES)
            Dim CsBytes = BitConverter.GetBytes(MFMCRC16(Buffer))
            Dim Cs1 = Sector.DataStartIndex + Sector.AdjustedDataLength
            StageChecksumBytes(ChangeList, Cs1, CsBytes, Offset, Length)
        End If
    End Sub

    ''' <summary>
    ''' Writes two recomputed checksum bytes into the working-clone bitstream and the decoded
    ''' display, and records them on the undo list.
    ''' </summary>
    Private Sub StageChecksumBytes(ChangeList As List(Of RawHexChange), Cs1 As Long, CsBytes() As Byte, Offset As Integer, Length As Integer)
        ChangeList.Add(New RawHexChange(Cs1, {_Data(Cs1)}, HexBox1.SelectionStart, HexBox1.SelectionLength))
        ChangeList.Add(New RawHexChange(Cs1 + 1, {_Data(Cs1 + 1)}, HexBox1.SelectionStart, HexBox1.SelectionLength))
        WriteMFMByteAt(_Bitstream, AdjustBitIndex(Cs1 * 16 + Offset, Length), CsBytes(0))
        WriteMFMByteAt(_Bitstream, AdjustBitIndex((Cs1 + 1) * 16 + Offset, Length), CsBytes(1))

        Dim RestoreIgnore = _IgnoreEvent
        _IgnoreEvent = True
        HexBox1.ByteProvider.WriteByte(Cs1, CsBytes(0))
        HexBox1.ByteProvider.WriteByte(Cs1 + 1, CsBytes(1))
        _IgnoreEvent = RestoreIgnore
    End Sub

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

#Region "Events"
    Private Sub HexBox1_ByteChanged(source As Object, e As HexBox.ByteChangedArgs) Handles HexBox1.ByteChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        Dim Region = GetEditableRegion(e.Index)
        If Region Is Nothing Then
            Exit Sub
        End If

        Dim Offset = _CurrentTrackData.Offset
        Dim Length = _Bitstream.Length

        ' Record the original bytes affected by this edit so it can be undone.
        Dim ChangeList As New List(Of RawHexChange) From {
            New RawHexChange(e.Index, {e.PrevValue}, HexBox1.SelectionStart, HexBox1.SelectionLength)
        }

        ' Re-encode the edited byte into the working-clone bitstream
        ' (the decoded value is already in _Data via the shared provider).
        Dim ByteBit = AdjustBitIndex(e.Index * 16 + Offset, Length)
        WriteMFMByteAt(_Bitstream, ByteBit, e.Value)

        StageRegionChecksum(ChangeList, Region)

        PushChanges(ChangeList)

        ' Bit-inspector now reflects the staged clone.
        RefreshBits(_Bitstream, DataRowEnum.Bitstream, True)
    End Sub

    Private Sub HexBox1_InsertActiveChanged(sender As Object, e As EventArgs) Handles HexBox1.InsertActiveChanged
        HexBox1.InsertActive = False
    End Sub

    ''' <summary>
    ''' On close, re-decode every edited track from its (already updated) bitstream and rebuild
    ''' the image's decoded sector map so edits are reflected in the rest of the application.
    ''' </summary>
    Private Sub HexViewRawForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        If _OriginalBitstreams.Count = 0 Then
            Exit Sub
        End If

        Dim BitstreamImage = _FloppyImage.BitstreamImage

        For Each KVP In _OriginalBitstreams
            Dim BT = BitstreamImage.GetTrack(CUShort(KVP.Key.X * BitstreamImage.TrackStep), CByte(KVP.Key.Y))
            If BT IsNot Nothing Then
                BT.MFMData = New IBM_MFM_Track(BT.Bitstream)
            End If
        Next

        If TypeOf _FloppyImage Is MappedFloppyImage Then
            CType(_FloppyImage, MappedFloppyImage).RebuildSectorMap()
        End If

        _FloppyImage.History.BatchEditMode = True
        For Each KVP In _OriginalBitstreams
            Dim BT = BitstreamImage.GetTrack(CUShort(KVP.Key.X * BitstreamImage.TrackStep), CByte(KVP.Key.Y))
            If BT IsNot Nothing Then
                _FloppyImage.History.AddBitstreamChange(CUShort(KVP.Key.X), CByte(KVP.Key.Y), KVP.Value, CType(BT.Bitstream.Clone(), BitArray))
            End If
        Next
        _FloppyImage.History.BatchEditMode = False

        _TracksUpdated = True
    End Sub

    Private Sub HexViewRawForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If _Changes.Count > 0 Then
            Dim Msg = String.Format(My.Resources.Dialog_CommitChanges, Environment.NewLine)
            Dim Response = MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNoCancel + MsgBoxStyle.DefaultButton3)

            If Response = MsgBoxResult.Cancel Then
                e.Cancel = True
                Exit Sub
            ElseIf Response = MsgBoxResult.Yes Then
                ApplyStagedChanges()
            End If
        End If

        RemoveClipboardFormatListener(Me.Handle)
    End Sub

    Private Sub InitEditingButtons() Handles Me.Load
        RefreshUndoButtons()
        RefreshPasteButton()
    End Sub

    Private Sub BtnPaste_Click(sender As Object, e As EventArgs) Handles BtnPaste.Click, ToolStripBtnPaste.Click
        If ClipboardHasHex() Then
            PasteHex()
        End If
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs) Handles BtnDelete.Click, ToolStripBtnDelete.Click
        FillSelected(0)
    End Sub

    Private Sub BtnFill4E_Click(sender As Object, e As EventArgs) Handles BtnFill4E.Click, ToolStripBtnFill4E.Click
        FillSelected(&H4E)
    End Sub

    Private Sub BtnFill_Click(sender As Object, e As EventArgs)
        FillSelected(CByte(sender.Tag))
    End Sub

    Private Sub ToolStripBtnCommit_Click(sender As Object, e As EventArgs) Handles ToolStripBtnCommit.Click
        CommitChanges(False)
    End Sub

    Private Sub ToolStripBtnRedo_Click(sender As Object, e As EventArgs) Handles ToolStripBtnRedo.Click
        PopChange(_RedoChanges, _Changes)
    End Sub

    Private Sub ToolStripBtnUndo_Click(sender As Object, e As EventArgs) Handles ToolStripBtnUndo.Click
        PopChange(_Changes, _RedoChanges)
    End Sub
#End Region

#Region "Helpers"
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

        Public Property Data As Byte()
        Public Property Index As Integer
        Public Property SelectionLength As Long
        Public Property SelectionStart As Long
    End Class

    ''' <summary>
    ''' Overwrite-only byte provider that wraps a byte array by reference, so edits stay in
    ''' sync with the form's decoded data buffer. Insert/delete are unsupported, enforcing
    ''' overwrite-only editing in the raw hex view.
    ''' </summary>
    Private Class SharedByteProvider
        Implements IByteProvider

        Private ReadOnly _Bytes() As Byte
        Public Event Changed As EventHandler Implements IByteProvider.Changed
        Public Event LengthChanged As EventHandler Implements IByteProvider.LengthChanged

        Public Sub New(Bytes() As Byte)
            _Bytes = Bytes
        End Sub

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
#End Region

End Class
