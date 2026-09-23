Imports DiskImageTool.Bitstream
Imports DiskImageTool.Bitstream.IBM_MFM
Imports DiskImageTool.DiskImage
Imports DiskImageTool.Hb.Windows.Forms
Imports DiskImageTool.HexView

Partial Public Class HexViewRawForm
    Private ReadOnly _Changes As New Stack(Of RawUndoStep)
    Private ReadOnly _OriginalBitstreams As New Dictionary(Of Point, BitArray)
    Private ReadOnly _RedoChanges As New Stack(Of RawUndoStep)
    Private ReadOnly _TrackCache As New Dictionary(Of Point, CachedTrack)
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
    ''' Commits staged edits by writing a clone of each undo-stack track's cached bitstream
    ''' to the live track, then clearing the undo/redo stacks.
    ''' </summary>
    Private Sub ApplyStagedChanges()
        Dim BitstreamImage = _FloppyImage.BitstreamImage
        Dim EditedTracks As New HashSet(Of Point)

        For Each UndoStep In _Changes
            EditedTracks.Add(New Point(UndoStep.Track, UndoStep.Side))
        Next

        For Each Key In EditedTracks
            Dim Cached As CachedTrack = Nothing
            If Not _TrackCache.TryGetValue(Key, Cached) OrElse Cached.Bitstream Is Nothing Then
                Continue For
            End If

            Dim BT = BitstreamImage.GetTrack(CUShort(Key.X * BitstreamImage.TrackStep), CByte(Key.Y))
            If BT Is Nothing Then
                Continue For
            End If

            If Not _OriginalBitstreams.ContainsKey(Key) Then
                _OriginalBitstreams(Key) = CType(BT.Bitstream.Clone(), BitArray)
            End If
            BT.Bitstream = CType(Cached.Bitstream.Clone(), BitArray)
        Next

        _Changes.Clear()
        _RedoChanges.Clear()
        RefreshUndoButtons()
    End Sub

    ''' <summary>
    ''' Commits staged edits to the live tracks and closes the form.
    ''' </summary>
    Private Sub CommitChanges()
        If _Changes.Count > 0 Then
            ApplyStagedChanges()
        End If

        Me.Close()
    End Sub

    ''' <summary>
    ''' Returns the region at the given hex byte index if it can be edited (Debug only, MFM
    ''' track, current bit offset): a gap, an ID field with a valid checksum, or a data area
    ''' (checksum may be invalid; sector may overlap).
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
        If Sector Is Nothing Then
            Return Nothing
        End If

        If IsIDAreaRegion(Region.RegionType) Then
            If Sector.IDAMChecksumValid Then
                Return Region
            End If
            Return Nothing
        End If

        If Region.RegionType = MFMRegionType.DataArea Then
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
    ''' Applies one undo/redo step. Overwrite restores decoded bytes in place. Insert/remove
    ''' splices bits in the working clone and rebuilds the hex view from that clone.
    ''' </summary>
    Private Sub PopChange(Source As Stack(Of RawUndoStep), Destination As Stack(Of RawUndoStep))
        If Source.Count = 0 Then
            Exit Sub
        End If

        Dim UndoStep = Source.Pop()

        If UndoStep.Kind = RawUndoKind.NormalizeAll Then
            ApplyNormalizeAllUndo(UndoStep, Destination)
            Exit Sub
        End If

        SwitchToWorkingTrack(UndoStep.Track, UndoStep.Side)

        If UndoStep.Kind = RawUndoKind.Overwrite Then
            Dim DestinationList As New List(Of RawHexChange)

            _IgnoreEvent = True
            For Each Change In UndoStep.Overwrites
                Dim Inverse(Change.Data.Length - 1) As Byte
                For Counter = 0 To Change.Data.Length - 1
                    Inverse(Counter) = _Data(Change.Index + Counter)
                Next
                DestinationList.Add(New RawHexChange(Change.Index, Inverse, Change.SelectionStart, Change.SelectionLength))

                For Counter = 0 To Change.Data.Length - 1
                    HexBox1.ByteProvider.WriteByte(Change.Index + Counter, Change.Data(Counter))
                    ReEncodeByte(Change.Index + Counter)
                Next
            Next
            _IgnoreEvent = False

            Destination.Push(New RawUndoStep(DestinationList, UndoStep.Track, UndoStep.Side))
            HexBox1.Select(UndoStep.Overwrites(0).SelectionStart, UndoStep.Overwrites(0).SelectionLength)

            RefreshUndoButtons()
            RefreshBits(_Bitstream, DataRowEnum.Bitstream, True)
            DataInspectorRefresh(True)
            Exit Sub
        End If

        If UndoStep.Kind = RawUndoKind.RotateTrack Then
            Dim Inverse = _Bitstream.Length - UndoStep.BitIndex
            Dim CurrentStart = HexBox1.SelectionStart
            Dim CurrentLength = HexBox1.SelectionLength

            SetWorkingBitstream(BitstreamAlign(_Bitstream, CUInt(Inverse)))
            Destination.Push(New RawUndoStep(RawUndoKind.RotateTrack, Inverse, Nothing, CurrentStart, CurrentLength, UndoStep.Track, UndoStep.Side))
            ReloadFromWorkingBitstream()
            HexBox1.Select(UndoStep.SelectionStart, UndoStep.SelectionLength)
            RefreshUndoButtons()
            RefreshBits(_Bitstream, DataRowEnum.Bitstream, True)
            DataInspectorRefresh(True)
            Exit Sub
        End If

        If UndoStep.Kind = RawUndoKind.ReplaceTail Then
            Dim CurrentTail = CopyBits(_Bitstream, UndoStep.BitIndex, _Bitstream.Length - UndoStep.BitIndex)
            SetWorkingBitstream(RemoveBits(_Bitstream, UndoStep.BitIndex, _Bitstream.Length - UndoStep.BitIndex))
            If UndoStep.Bits IsNot Nothing AndAlso UndoStep.Bits.Length > 0 Then
                SetWorkingBitstream(InsertBits(_Bitstream, UndoStep.BitIndex, UndoStep.Bits))
            End If
            ApplyMFMSpliceClocks(UndoStep.BitIndex, If(UndoStep.Bits Is Nothing, 0, UndoStep.Bits.Length))
            Destination.Push(New RawUndoStep(RawUndoKind.ReplaceTail, UndoStep.BitIndex, CurrentTail, UndoStep.SelectionStart, UndoStep.SelectionLength, UndoStep.Track, UndoStep.Side))
            ReloadFromWorkingBitstream()
            HexBox1.Select(UndoStep.SelectionStart, UndoStep.SelectionLength)
            RefreshUndoButtons()
            RefreshBits(_Bitstream, DataRowEnum.Bitstream, True)
            DataInspectorRefresh(True)
            Exit Sub
        End If

        If UndoStep.Kind = RawUndoKind.InsertBits Then
            SetWorkingBitstream(RemoveBits(_Bitstream, UndoStep.BitIndex, UndoStep.Bits.Length))
            Destination.Push(New RawUndoStep(RawUndoKind.RemoveBits, UndoStep.BitIndex, UndoStep.Bits, UndoStep.SelectionStart, UndoStep.SelectionLength, UndoStep.Track, UndoStep.Side))
            ApplyMFMSpliceClocks(UndoStep.BitIndex, 0)
        Else
            SetWorkingBitstream(InsertBits(_Bitstream, UndoStep.BitIndex, UndoStep.Bits))
            Destination.Push(New RawUndoStep(RawUndoKind.InsertBits, UndoStep.BitIndex, UndoStep.Bits, UndoStep.SelectionStart, UndoStep.SelectionLength, UndoStep.Track, UndoStep.Side))
            ApplyMFMSpliceClocks(UndoStep.BitIndex, UndoStep.Bits.Length)
        End If

        ReloadFromWorkingBitstream()
        HexBox1.Select(UndoStep.SelectionStart, UndoStep.SelectionLength)

        RefreshUndoButtons()
        RefreshBits(_Bitstream, DataRowEnum.Bitstream, True)
        DataInspectorRefresh(True)
    End Sub

    ''' <summary>
    ''' Pushes an overwrite edit onto the undo stack, clears the redo stack, and refreshes the buttons.
    ''' </summary>
    Private Sub PushChanges(ChangeList As List(Of RawHexChange))
        _Changes.Push(New RawUndoStep(ChangeList, _Track, _Side))
        _RedoChanges.Clear()
        RefreshUndoButtons()
    End Sub

    ''' <summary>
    ''' Pushes an insert or remove of a bit splice onto the undo stack for a later length-changing edit.
    ''' </summary>
    Private Sub PushSplice(Kind As RawUndoKind, BitIndex As Integer, Bits As BitArray, SelectionStart As Long, SelectionLength As Long)
        _Changes.Push(New RawUndoStep(Kind, BitIndex, CType(Bits.Clone(), BitArray), SelectionStart, SelectionLength, _Track, _Side))
        _RedoChanges.Clear()
        RefreshUndoButtons()
    End Sub

    Private Function InsertBits(source As BitArray, index As Integer, bitsToInsert As BitArray) As BitArray
        If index < 0 OrElse index > source.Length Then
            Throw New ArgumentOutOfRangeException(NameOf(index))
        End If

        Dim insertCount = bitsToInsert.Length
        Dim result As New BitArray(source.Length + insertCount)

        Dim destPos As Integer = 0

        For i = 0 To index - 1
            result(destPos) = source(i)
            destPos += 1
        Next

        For i = 0 To insertCount - 1
            result(destPos) = bitsToInsert(i)
            destPos += 1
        Next

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

        For i = 0 To index - 1
            result(destPos) = source(i)
            destPos += 1
        Next

        For i = index + count To source.Length - 1
            result(destPos) = source(i)
            destPos += 1
        Next

        Return result
    End Function

    Private Function GetPreviousDataBit(Bits As BitArray, BitIndex As Integer) As Boolean
        Return Bits(AdjustBitIndex(BitIndex - 1, Bits.Length))
    End Function

    ''' <summary>
    ''' Last data bit of the MFM byte that ends immediately before BitIndex (the first clock
    ''' of the byte at BitIndex). The stream is treated as circular.
    ''' </summary>
    Private Function GetPreviousDataBit(BitIndex As Integer) As Boolean
        Return GetPreviousDataBit(_Bitstream, BitIndex)
    End Function

    Private Sub FixClockAt(Bits As BitArray, BitIndex As Integer)
        If Bits Is Nothing OrElse Bits.Length < 2 Then
            Exit Sub
        End If

        Dim Length = Bits.Length
        Dim ClockIndex = AdjustBitIndex(BitIndex, Length)
        Dim DataIndex = AdjustBitIndex(BitIndex + 1, Length)
        Dim PrevData = GetPreviousDataBit(Bits, BitIndex)

        Bits(ClockIndex) = (Not Bits(DataIndex)) AndAlso (Not PrevData)
    End Sub

    ''' <summary>
    ''' Recomputes the MFM clock bit at BitIndex: clock is 1 only when both the previous data
    ''' bit and this byte's first data bit are 0.
    ''' </summary>
    Private Sub FixClockAt(BitIndex As Integer)
        FixClockAt(_Bitstream, BitIndex)
    End Sub

    Private Sub ApplyMFMSpliceClocks(Bits As BitArray, StartBit As Integer, InsertedBitCount As Integer)
        FixClockAt(Bits, StartBit)

        If InsertedBitCount > 0 Then
            FixClockAt(Bits, StartBit + InsertedBitCount)
        End If
    End Sub

    ''' <summary>
    ''' After a splice, fix clocks at both sides of the joined bytes. InsertedBitCount 0 is a
    ''' delete: only the byte that now follows the previous content needs its first clock.
    ''' </summary>
    Private Sub ApplyMFMSpliceClocks(StartBit As Integer, InsertedBitCount As Integer)
        FixClockAt(StartBit)

        If InsertedBitCount > 0 Then
            FixClockAt(StartBit + InsertedBitCount)
        End If
    End Sub

    Private Function CopyBits(Source As BitArray, Index As Integer, Count As Integer) As BitArray
        Dim Result As New BitArray(Count)

        For i = 0 To Count - 1
            Result(i) = Source(Index + i)
        Next

        Return Result
    End Function

    Private Function GetGapOrNullRegion(Index As Long) As BitstreamRegion
        Dim Region = GetEditableRegion(Index)

        If Region Is Nothing Then
            Return Nothing
        End If

        If IsGapRegion(Region.RegionType) OrElse IsNullRegion(Region.RegionType) Then
            Return Region
        End If

        Return Nothing
    End Function

    Private Function SelectionSpansMultipleRegions() As Boolean
        If HexBox1.SelectionLength <= 1 Then
            Return False
        End If

        If _RegionMap Is Nothing Then
            Return True
        End If

        Dim SelectionStart = HexBox1.SelectionStart
        Dim SelectionEnd = SelectionStart + HexBox1.SelectionLength - 1

        If SelectionStart < 0 OrElse SelectionEnd > _RegionMap.Length - 1 Then
            Return True
        End If

        Dim RegionStart = _RegionMap(SelectionStart)
        Dim RegionEnd = _RegionMap(SelectionEnd)

        Return RegionStart Is Nothing OrElse RegionEnd Is Nothing OrElse RegionStart IsNot RegionEnd
    End Function

    Private Sub RefreshGapMenuItems()
        Dim Region = GetGapOrNullRegion(HexBox1.SelectionStart)

        Dim Visible = Region IsNot Nothing AndAlso Not SelectionSpansMultipleRegions()

        BtnInsertBytes.Enabled = Visible
        ToolStripToolsInsertBytes.Enabled = Visible

        BtnDeleteBytes.Enabled = Visible AndAlso HexBox1.SelectionLength > 0
        ToolStripToolsDeleteBytes.Enabled = Visible AndAlso HexBox1.SelectionLength > 0
    End Sub

    Private Function TryGetRemoveSplice(ByRef BitIndex As Integer, ByRef BitCount As Integer) As Boolean
        BitIndex = -1
        BitCount = 0

        If _TrackType <> BitstreamTrackType.MFM Then
            Return False
        End If

        If _CurrentTrackData Is Nothing OrElse _RegionMap Is Nothing OrElse _RegionData Is Nothing OrElse _Bitstream Is Nothing Then
            Return False
        End If

        Dim SelectionStart = HexBox1.SelectionStart
        If SelectionStart < 0 OrElse SelectionStart > _RegionMap.Length - 1 Then
            Return False
        End If

        Dim Region = _RegionMap(SelectionStart)
        If Region Is Nothing Then
            Return False
        End If

        Dim ByteOffset = SelectionStart - Region.StartIndex
        If ByteOffset < 0 OrElse ByteOffset > 1 Then
            Return False
        End If

        If CULng(SelectionStart) >= CULng(Region.StartIndex) + Region.Length Then
            Return False
        End If

        Dim TrackOffset = CUInt(_CurrentTrackData.Offset)
        If Region.BitOffset = TrackOffset Then
            Return False
        End If

        Dim Previous As BitstreamRegion = Nothing
        Dim Found = False
        For Each Item In _RegionData.Regions
            If Item Is Region Then
                Found = True
                Exit For
            End If
            Previous = Item
        Next

        If Not Found OrElse Previous Is Nothing Then
            Return False
        End If

        If Previous.BitOffset <> TrackOffset Then
            Return False
        End If

        BitCount = (CInt(Region.BitOffset) - CInt(Previous.BitOffset) + 16) Mod 16
        If BitCount < 1 OrElse BitCount > 15 Then
            Return False
        End If

        BitIndex = CInt(Region.StartIndex * 16 + _CurrentTrackData.Offset)
        If BitIndex < 0 OrElse BitIndex + BitCount > _Bitstream.Length Then
            Return False
        End If

        Return True
    End Function

    Private Sub RefreshRemoveSpliceMenuItem()
        Dim BitIndex As Integer
        Dim BitCount As Integer

        Dim Enabled = TryGetRemoveSplice(BitIndex, BitCount)

        BtnRemoveSplice.Enabled = Enabled
        ToolStripToolsRemoveSplice.Enabled = Enabled
    End Sub

    Private Function TryGetRotateTrack(ByRef Offset As Integer) As Boolean
        Offset = 0

        If _Bitstream Is Nothing OrElse _CurrentTrackData Is Nothing OrElse _RegionMap Is Nothing Then
            Return False
        End If

        Dim SelectionStart = HexBox1.SelectionStart
        If SelectionStart <= 0 OrElse SelectionStart > _RegionMap.Length - 1 Then
            Return False
        End If

        Dim Region = _RegionMap(SelectionStart)
        If Region Is Nothing Then
            Return False
        End If

        If Region.BitOffset <> CUInt(_CurrentTrackData.Offset) Then
            Return False
        End If

        Offset = CInt(SelectionStart * 16)
        If Offset <= 0 OrElse Offset >= _Bitstream.Length Then
            Return False
        End If

        Return True
    End Function

    Private Sub RefreshRotateTrackMenuItem()
        Dim Offset As Integer

        Dim Enabled = TryGetRotateTrack(Offset)

        BtnRotateTrack.Enabled = Enabled
        ToolStripToolsRotateTrack.Enabled = Enabled
    End Sub

    Private Function GetCaretBitIndex() As Integer
        If _Bitstream Is Nothing OrElse _CurrentTrackData Is Nothing Then
            Return -1
        End If

        Dim BitIndex = CInt(HexBox1.SelectionStart * 16 + _CurrentTrackData.Offset)

        If BitIndex < 0 OrElse BitIndex > _Bitstream.Length Then
            Return -1
        End If

        Return BitIndex
    End Function

    ''' <summary>
    ''' Inserts N MFM-encoded fill bytes at the caret into the working clone.
    ''' </summary>
    Private Sub InsertGapOrNullBytes()
        Dim Region = GetGapOrNullRegion(HexBox1.SelectionStart)

        If Region Is Nothing OrElse SelectionSpansMultipleRegions() Then
            Exit Sub
        End If

        Dim DefaultFill As Byte = If(IsGapRegion(Region.RegionType), CByte(&H4E), CByte(0))
        Dim Result = InsertBytesForm.Display(DefaultFill)

        If Not Result.Result OrElse Result.Count < 1 Then
            Exit Sub
        End If

        Dim Count = Result.Count
        Dim FillByte = Result.FillByte

        Dim BitIndex = GetCaretBitIndex()

        If BitIndex < 0 Then
            Exit Sub
        End If

        Dim Fill(Count - 1) As Byte

        For i = 0 To Count - 1
            Fill(i) = FillByte
        Next

        Dim SelectionStart = HexBox1.SelectionStart
        Dim SelectionLength = HexBox1.SelectionLength
        Dim Encoded = MFMEncodeBytes(Fill, GetPreviousDataBit(BitIndex))

        SetWorkingBitstream(InsertBits(_Bitstream, BitIndex, Encoded))
        ApplyMFMSpliceClocks(BitIndex, Encoded.Length)
        PushSplice(RawUndoKind.InsertBits, BitIndex, Encoded, SelectionStart, SelectionLength)
        ReloadFromWorkingBitstream()
        HexBox1.Select(SelectionStart, Count)
    End Sub

    ''' <summary>
    ''' Removes the selected decoded bytes from the current gap/null region.
    ''' </summary>
    Private Sub DeleteGapOrNullBytes()
        If HexBox1.SelectionLength < 1 OrElse SelectionSpansMultipleRegions() Then
            Exit Sub
        End If

        Dim Region = GetGapOrNullRegion(HexBox1.SelectionStart)

        If Region Is Nothing Then
            Exit Sub
        End If

        Dim Count = CInt(HexBox1.SelectionLength)

        Dim BitIndex = GetCaretBitIndex()

        If BitIndex < 0 Then
            Exit Sub
        End If

        Dim MaxBytes = CInt(Region.StartIndex + Region.Length - HexBox1.SelectionStart)
        Dim MaxBits = (_Bitstream.Length - BitIndex) \ 16

        If MaxBits < MaxBytes Then
            MaxBytes = MaxBits
        End If

        If MaxBytes < 1 Then
            Exit Sub
        End If

        If Count > MaxBytes Then
            Count = MaxBytes
        End If

        RemoveBitsFromWorkingClone(BitIndex, Count * 16)
    End Sub

    ''' <summary>
    ''' Removes 1–15 extra splice bits at the start of the current unaligned region so its
    ''' bit offset matches the previous aligned region.
    ''' </summary>
    Private Sub RemoveSplice()
        Dim BitIndex As Integer
        Dim BitCount As Integer

        If Not TryGetRemoveSplice(BitIndex, BitCount) Then
            Exit Sub
        End If

        RemoveBitsFromWorkingClone(BitIndex, BitCount)
    End Sub

    ''' <summary>
    ''' Removes BitCount bits at BitIndex from the working clone, records a RemoveBits undo
    ''' step, and reloads the hex view.
    ''' </summary>
    Private Sub RemoveBitsFromWorkingClone(BitIndex As Integer, BitCount As Integer)
        Dim SelectionStart = HexBox1.SelectionStart
        Dim SelectionLength = HexBox1.SelectionLength
        Dim Removed = CopyBits(_Bitstream, BitIndex, BitCount)

        SetWorkingBitstream(RemoveBits(_Bitstream, BitIndex, BitCount))
        ApplyMFMSpliceClocks(BitIndex, 0)
        PushSplice(RawUndoKind.RemoveBits, BitIndex, Removed, SelectionStart, SelectionLength)
        ReloadFromWorkingBitstream()
        HexBox1.Select(SelectionStart, 0)
    End Sub

    ''' <summary>
    ''' Rotates the working bitstream so the aligned byte under the caret becomes the first
    ''' decoded byte of the track.
    ''' </summary>
    Private Sub RotateTrack()
        Dim Offset As Integer

        If Not TryGetRotateTrack(Offset) Then
            Exit Sub
        End If

        Dim SelectionStart = HexBox1.SelectionStart
        Dim SelectionLength = HexBox1.SelectionLength

        SetWorkingBitstream(BitstreamAlign(_Bitstream, CUInt(Offset)))
        _Changes.Push(New RawUndoStep(RawUndoKind.RotateTrack, Offset, Nothing, SelectionStart, SelectionLength, _Track, _Side))
        _RedoChanges.Clear()
        RefreshUndoButtons()
        ReloadFromWorkingBitstream()
        HexBox1.Select(0, 0)
    End Sub

    Private Function GetTargetTrackBitCount(Track As UShort, Side As Byte, Bits As BitArray) As Integer
        If Bits Is Nothing OrElse _FloppyImage Is Nothing OrElse _FloppyImage.BitstreamImage Is Nothing Then
            Return 0
        End If

        Dim Image = _FloppyImage.BitstreamImage
        Dim BT = Image.GetTrack(CUShort(Track * Image.TrackStep), CByte(Side))
        If BT IsNot Nothing AndAlso BT.RPM <> 0 AndAlso BT.BitRate <> 0 Then
            Return CInt(MFMGetSize(BT.RPM, BT.BitRate))
        End If

        Return CInt(InferBitCount(CUInt(Bits.Length)))
    End Function

    Private Function GetTargetTrackBitCount() As Integer
        Return GetTargetTrackBitCount(_Track, _Side, _Bitstream)
    End Function

    Private Function TryGetNormalizeTrackSize(Bits As BitArray, Track As UShort, Side As Byte, ByRef Target As Integer) As Boolean
        Target = 0

        If Bits Is Nothing Then
            Return False
        End If

        Target = GetTargetTrackBitCount(Track, Side, Bits)
        If Target <= 0 OrElse Target = Bits.Length Then
            Return False
        End If

        Return True
    End Function

    Private Function TryGetNormalizeTrackSize(ByRef Target As Integer) As Boolean
        Return TryGetNormalizeTrackSize(_Bitstream, _Track, _Side, Target)
    End Function

    Private Function AnyTrackNeedsNormalize() As Boolean
        If _FloppyImage Is Nothing OrElse _FloppyImage.BitstreamImage Is Nothing Then
            Return False
        End If

        Dim Image = _FloppyImage.BitstreamImage
        For t = 0 To _FloppyImage.TrackCount - 1
            For s = 0 To _FloppyImage.SideCount - 1
                Dim BT = Image.GetTrack(CUShort(t * Image.TrackStep), CByte(s))
                If BT Is Nothing Then
                    Continue For
                End If
                If BT.TrackType <> BitstreamTrackType.MFM AndAlso BT.TrackType <> BitstreamTrackType.FM Then
                    Continue For
                End If

                Dim Bits As BitArray = Nothing
                Dim Cached As CachedTrack = Nothing
                If _TrackCache.TryGetValue(New Point(t, s), Cached) AndAlso Cached.Bitstream IsNot Nothing Then
                    Bits = Cached.Bitstream
                Else
                    Bits = BT.Bitstream
                End If

                Dim Target As Integer
                If TryGetNormalizeTrackSize(Bits, CUShort(t), CByte(s), Target) Then
                    Return True
                End If
            Next
        Next

        Return False
    End Function

    Private Sub RefreshNormalizeTrackSizeMenuItem()
        Dim Target As Integer
        Dim Enabled = TryGetNormalizeTrackSize(Target)

        BtnNormalizeTrackSize.Enabled = Enabled
        ToolStripToolsNormalizeTrackSize.Enabled = Enabled
        ToolStripToolsNormalizeAllTrackSizes.Enabled = AnyTrackNeedsNormalize()
    End Sub

    Private Function TryBuildNormalizeTail(Bits As BitArray, Track As UShort, Side As Byte, ByRef BitIndex As Integer, ByRef NewTail As BitArray) As Boolean
        BitIndex = 0
        NewTail = Nothing

        Dim Target As Integer
        If Not TryGetNormalizeTrackSize(Bits, Track, Side, Target) Then
            Return False
        End If

        Dim Length = Bits.Length

        If Length > Target Then
            BitIndex = Target
            NewTail = New BitArray(0)
            Return True
        End If

        BitIndex = (Length \ 16) * 16
        Dim PadBytes = (Target - BitIndex) \ 16
        If PadBytes < 1 Then
            Return False
        End If

        Dim Fill(PadBytes - 1) As Byte
        For i = 0 To PadBytes - 1
            Fill(i) = CByte(MFM_GAP_BYTE)
        Next

        NewTail = MFMEncodeBytes(Fill, GetPreviousDataBit(Bits, BitIndex))
        Return True
    End Function

    Private Function ReplaceTailOnBits(Bits As BitArray, BitIndex As Integer, NewTail As BitArray, ByRef OldTail As BitArray) As BitArray
        Dim TailLength = Bits.Length - BitIndex
        If TailLength < 0 Then
            TailLength = 0
        End If
        OldTail = CopyBits(Bits, BitIndex, TailLength)

        Dim Result = RemoveBits(Bits, BitIndex, TailLength)
        If NewTail IsNot Nothing AndAlso NewTail.Length > 0 Then
            Result = InsertBits(Result, BitIndex, NewTail)
        End If
        ApplyMFMSpliceClocks(Result, BitIndex, If(NewTail Is Nothing, 0, NewTail.Length))
        Return Result
    End Function

    Private Sub AssignCachedBitstream(Track As UShort, Side As Byte, Bits As BitArray)
        Dim Cached As CachedTrack = Nothing
        If _TrackCache.TryGetValue(New Point(Track, Side), Cached) Then
            Cached.Bitstream = Bits
        End If

        If Track = _Track AndAlso Side = _Side Then
            SetWorkingBitstream(Bits)
        End If
    End Sub

    ''' <summary>
    ''' Resizes the working bitstream to the image-type target: chop the tail if too long,
    ''' or 16-align and append MFM 0x4E if too short.
    ''' </summary>
    Private Sub NormalizeTrackSize()
        Dim BitIndex As Integer
        Dim NewTail As BitArray = Nothing
        If Not TryBuildNormalizeTail(_Bitstream, _Track, _Side, BitIndex, NewTail) Then
            Exit Sub
        End If

        ReplaceTail(BitIndex, NewTail)
    End Sub

    ''' <summary>
    ''' Normalizes every MFM/FM track that is not already at its target size and records
    ''' the batch as a single undo step.
    ''' </summary>
    Private Sub NormalizeAllTrackSizes()
        If _FloppyImage Is Nothing OrElse _FloppyImage.BitstreamImage Is Nothing Then
            Exit Sub
        End If

        Dim Image = _FloppyImage.BitstreamImage
        Dim Tails As New List(Of RawTrackTail)
        Dim CurrentChanged = False

        For t = 0 To _FloppyImage.TrackCount - 1
            For s = 0 To _FloppyImage.SideCount - 1
                Dim BT = Image.GetTrack(CUShort(t * Image.TrackStep), CByte(s))
                If BT Is Nothing Then
                    Continue For
                End If
                If BT.TrackType <> BitstreamTrackType.MFM AndAlso BT.TrackType <> BitstreamTrackType.FM Then
                    Continue For
                End If

                Dim Track = CUShort(t)
                Dim Side = CByte(s)
                Dim Cached = GetOrCreateCachedTrack(Track, Side, BT)
                Dim BitIndex As Integer
                Dim NewTail As BitArray = Nothing
                If Not TryBuildNormalizeTail(Cached.Bitstream, Track, Side, BitIndex, NewTail) Then
                    Continue For
                End If

                Dim OldTail As BitArray = Nothing
                Dim Updated = ReplaceTailOnBits(Cached.Bitstream, BitIndex, NewTail, OldTail)
                AssignCachedBitstream(Track, Side, Updated)
                If Track = _Track AndAlso Side = _Side Then
                    CurrentChanged = True
                End If
                Tails.Add(New RawTrackTail(Track, Side, BitIndex, OldTail))
            Next
        Next

        If Tails.Count = 0 Then
            Exit Sub
        End If

        _Changes.Push(New RawUndoStep(Tails))
        _RedoChanges.Clear()
        RefreshUndoButtons()

        If CurrentChanged Then
            ReloadFromWorkingBitstream()
            If HexBox1.ByteProvider Is Nothing OrElse HexBox1.SelectionStart >= HexBox1.ByteProvider.Length Then
                HexBox1.Select(0, 0)
            End If
            RefreshBits(_Bitstream, DataRowEnum.Bitstream, True)
            DataInspectorRefresh(True)
        End If

        RefreshNormalizeTrackSizeMenuItem()
        MsgBox(String.Format(My.Resources.Dialog_NormalizeAllTrackSizes, Tails.Count), MsgBoxStyle.Information)
    End Sub

    Private Sub ApplyNormalizeAllUndo(UndoStep As RawUndoStep, Destination As Stack(Of RawUndoStep))
        Dim Inverse As New List(Of RawTrackTail)
        Dim CurrentChanged = False
        Dim Image = _FloppyImage.BitstreamImage

        For Each Tail In UndoStep.TrackTails
            Dim BT = Image.GetTrack(CUShort(Tail.Track * Image.TrackStep), CByte(Tail.Side))
            If BT Is Nothing Then
                Continue For
            End If

            Dim Cached = GetOrCreateCachedTrack(Tail.Track, Tail.Side, BT)
            Dim CurrentTail As BitArray = Nothing
            Dim Updated = ReplaceTailOnBits(Cached.Bitstream, Tail.BitIndex, Tail.Bits, CurrentTail)
            AssignCachedBitstream(Tail.Track, Tail.Side, Updated)
            Inverse.Add(New RawTrackTail(Tail.Track, Tail.Side, Tail.BitIndex, CurrentTail))
            If Tail.Track = _Track AndAlso Tail.Side = _Side Then
                CurrentChanged = True
            End If
        Next

        Destination.Push(New RawUndoStep(Inverse))
        RefreshUndoButtons()

        If CurrentChanged Then
            ReloadFromWorkingBitstream()
            If HexBox1.ByteProvider Is Nothing OrElse HexBox1.SelectionStart >= HexBox1.ByteProvider.Length Then
                HexBox1.Select(0, 0)
            End If
            RefreshBits(_Bitstream, DataRowEnum.Bitstream, True)
            DataInspectorRefresh(True)
        End If

        RefreshNormalizeTrackSizeMenuItem()
    End Sub

    ''' <summary>
    ''' Replaces bits from BitIndex through the end of the working clone with NewTail and
    ''' records a single ReplaceTail undo step.
    ''' </summary>
    Private Sub ReplaceTail(BitIndex As Integer, NewTail As BitArray)
        Dim SelectionStart = HexBox1.SelectionStart
        Dim SelectionLength = HexBox1.SelectionLength
        Dim OldTail As BitArray = Nothing
        Dim Updated = ReplaceTailOnBits(_Bitstream, BitIndex, NewTail, OldTail)

        SetWorkingBitstream(Updated)
        PushSplice(RawUndoKind.ReplaceTail, BitIndex, OldTail, SelectionStart, SelectionLength)
        ReloadFromWorkingBitstream()

        If HexBox1.ByteProvider Is Nothing OrElse SelectionStart >= HexBox1.ByteProvider.Length Then
            HexBox1.Select(0, 0)
        Else
            HexBox1.Select(SelectionStart, 0)
        End If
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
    ''' Data CRC is only rewritten when the existing checksum is valid.
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
            If Sector Is Nothing OrElse Not Sector.DataChecksumValid Then
                Exit Sub
            End If

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
        Bitstream(NextClock) = (Not Bitstream(NextData)) AndAlso (Not Encoded(15))
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
    ''' On close, re-decode every committed track from its updated bitstream and rebuild
    ''' the image's decoded sector map so edits are reflected in the rest of the application.
    ''' Uncommitted work that was fully undone never reaches this path because originals are
    ''' only snapshotted when the undo stack is applied.
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
            Else
                _OriginalBitstreams.Clear()
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

    Private Sub BtnInsertBytes_Click(sender As Object, e As EventArgs) Handles BtnInsertBytes.Click, ToolStripToolsInsertBytes.Click
        InsertGapOrNullBytes()
    End Sub

    Private Sub BtnDeleteBytes_Click(sender As Object, e As EventArgs) Handles BtnDeleteBytes.Click, ToolStripToolsDeleteBytes.Click
        DeleteGapOrNullBytes()
    End Sub

    Private Sub BtnRemoveSplice_Click(sender As Object, e As EventArgs) Handles BtnRemoveSplice.Click, ToolStripToolsRemoveSplice.Click
        RemoveSplice()
    End Sub

    Private Sub BtnRotateTrack_Click(sender As Object, e As EventArgs) Handles BtnRotateTrack.Click, ToolStripToolsRotateTrack.Click
        RotateTrack()
    End Sub

    Private Sub BtnNormalizeTrackSize_Click(sender As Object, e As EventArgs) Handles BtnNormalizeTrackSize.Click, ToolStripToolsNormalizeTrackSize.Click
        NormalizeTrackSize()
    End Sub

    Private Sub ToolStripToolsNormalizeAllTrackSizes_Click(sender As Object, e As EventArgs) Handles ToolStripToolsNormalizeAllTrackSizes.Click
        NormalizeAllTrackSizes()
    End Sub

    Private Sub ToolStripBtnCommit_Click(sender As Object, e As EventArgs) Handles ToolStripBtnCommit.Click
        CommitChanges()
    End Sub

    Private Sub ToolStripBtnRedo_Click(sender As Object, e As EventArgs) Handles ToolStripBtnRedo.Click
        PopChange(_RedoChanges, _Changes)
    End Sub

    Private Sub ToolStripBtnUndo_Click(sender As Object, e As EventArgs) Handles ToolStripBtnUndo.Click
        PopChange(_Changes, _RedoChanges)
    End Sub
#End Region

#Region "Helpers"
    Private Enum RawUndoKind
        Overwrite
        InsertBits
        RemoveBits
        RotateTrack
        ReplaceTail
        NormalizeAll
    End Enum

    ''' <summary>
    ''' Assigns the current working bitstream and keeps the visited-track cache in sync when
    ''' insert/remove/rotate replace the BitArray instance.
    ''' </summary>
    Private Sub SetWorkingBitstream(Bits As BitArray)
        _Bitstream = Bits

        Dim Cached As CachedTrack = Nothing
        If _TrackCache.TryGetValue(New Point(_Track, _Side), Cached) Then
            Cached.Bitstream = Bits
        End If
    End Sub

    Private Function GetCachedOffset(Track As UShort, Side As Byte) As Integer
        Dim Cached As CachedTrack = Nothing
        If _TrackCache.TryGetValue(New Point(Track, Side), Cached) Then
            Return Cached.Offset
        End If

        Return -1
    End Function

    Private Sub PersistCurrentTrackOffset()
        If _CurrentTrackData Is Nothing Then
            Exit Sub
        End If

        Dim Cached As CachedTrack = Nothing
        If _TrackCache.TryGetValue(New Point(_CurrentTrackData.Track, _CurrentTrackData.Side), Cached) Then
            Cached.Offset = _CurrentTrackData.Offset
        End If
    End Sub

    Private Function GetOrCreateCachedTrack(Track As UShort, Side As Byte, LiveTrack As IBitstreamTrack) As CachedTrack
        Dim Key As New Point(Track, Side)
        Dim Cached As CachedTrack = Nothing
        If _TrackCache.TryGetValue(Key, Cached) Then
            Return Cached
        End If

        Cached = New CachedTrack With {
            .Bitstream = CType(LiveTrack.Bitstream.Clone(), BitArray),
            .Offset = -1
        }
        _TrackCache(Key) = Cached
        Return Cached
    End Function

    Private Function FindComboTrackData(Track As UShort, Side As Byte) As TrackData
        For Each Item As TrackData In ComboTrack.Items
            If Item.Track = Track AndAlso Item.Side = Side Then
                Return Item
            End If
        Next

        Return Nothing
    End Function

    ''' <summary>
    ''' Loads the given track from the visited-track cache, updating the combo if needed,
    ''' without prompting to commit.
    ''' </summary>
    Private Sub SwitchToWorkingTrack(Track As UShort, Side As Byte)
        If _Track = Track AndAlso _Side = Side Then
            Exit Sub
        End If

        Dim TrackData As TrackData = Nothing

        _IgnoreEvent = True
        Try
            If Not IsTrackListed(Track, Side, CheckBoxAllTracks.Checked) Then
                CheckBoxAllTracks.Checked = True
                PopulateTracks(True)
            End If

            TrackData = FindComboTrackData(Track, Side)

            If TrackData Is Nothing Then
                TrackData = New TrackData With {
                    .Track = Track,
                    .Side = Side,
                    .Offset = -1
                }
            End If

            ComboTrack.SelectedItem = TrackData
        Finally
            _IgnoreEvent = False
        End Try

        LoadTrack(TrackData, False, False)
    End Sub

    ''' <summary>
    ''' One undo/redo step: either a list of overwritten hex bytes, or a compact bitstream splice.
    ''' </summary>
    Private Class RawUndoStep
        Public Sub New(Overwrites As List(Of RawHexChange), Track As UShort, Side As Byte)
            Kind = RawUndoKind.Overwrite
            Me.Overwrites = Overwrites
            Me.Track = Track
            Me.Side = Side
        End Sub

        Public Sub New(Kind As RawUndoKind, BitIndex As Integer, Bits As BitArray, SelectionStart As Long, SelectionLength As Long, Track As UShort, Side As Byte)
            Me.Kind = Kind
            Me.BitIndex = BitIndex
            Me.Bits = Bits
            Me.SelectionStart = SelectionStart
            Me.SelectionLength = SelectionLength
            Me.Track = Track
            Me.Side = Side
        End Sub

        Public Sub New(Tails As List(Of RawTrackTail))
            Kind = RawUndoKind.NormalizeAll
            Me.TrackTails = Tails
        End Sub

        Public Property BitIndex As Integer
        Public Property Bits As BitArray
        Public Property Kind As RawUndoKind
        Public Property Overwrites As List(Of RawHexChange)
        Public Property SelectionLength As Long
        Public Property SelectionStart As Long
        Public Property Side As Byte
        Public Property Track As UShort
        Public Property TrackTails As List(Of RawTrackTail)
    End Class

    Private Class RawTrackTail
        Public Sub New(Track As UShort, Side As Byte, BitIndex As Integer, Bits As BitArray)
            Me.Track = Track
            Me.Side = Side
            Me.BitIndex = BitIndex
            Me.Bits = Bits
        End Sub

        Public Property BitIndex As Integer
        Public Property Bits As BitArray
        Public Property Side As Byte
        Public Property Track As UShort
    End Class

    ''' <summary>
    ''' A single staged overwrite for undo/redo: the original bytes at a given _Data index plus the
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

    ''' <summary>
    ''' Working clone and last bit offset for a visited track.
    ''' </summary>
    Private Class CachedTrack
        Public Property Bitstream As BitArray
        Public Property Offset As Integer
    End Class
#End Region

End Class
