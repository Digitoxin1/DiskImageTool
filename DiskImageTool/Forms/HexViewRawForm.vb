Imports System.Runtime.InteropServices
Imports DiskImageTool.HexView
Imports Hb.Windows.Forms
Imports DiskImageTool.Bitstream.IBM_MFM
Imports DiskImageTool.DiskImage
Imports DiskImageTool.Bitstream

Public Class HexViewRawForm
    Private WithEvents CheckBoxAllTracks As ToolStripCheckBox
    Private WithEvents ComboTrack As ToolStripComboBox
    Private WithEvents NumericBitOffset As ToolStripNumericUpDown
    Private Const PADDING_COLS As Integer = 8
    Private Const PADDING_ROWS As Integer = 6
    Private Const SECTOR_HEIGHT As Integer = 16
    Private Const SECTOR_WIDTH As Integer = 24
    Private ReadOnly _ToolTip As ToolTip
    Private _AllTracks As Boolean
    Private _Bitstream As BitArray
    Private _CachedSelectedLength As Long = -1
    Private _CurrentRegionSector As BitstreamRegionSector = Nothing
    Private _CurrentSelectionLength As Long = -1
    Private _CurrentSelectionStart As Long = -1
    Private _CurrentTrackData As TrackData = Nothing
    Private _Data() As Byte
    Private _DataGridInspector As HexViewDataGridInspector
    Private _FloppyImage As IFloppyImage
    Private _IgnoreEvent As Boolean = False
    Private _Initialized As Boolean = False
    Private _LabelBitOffset As ToolStripLabel
    Private _LabelBitOffsetAligned As ToolStripLabel
    Private _LastSearch As HexSearch
    Private _RegionData As RegionData
    Private _RegionMap() As BitstreamRegion
    Private _SectorLabels As List(Of Label)
    Private _SectorsPerTrack As UShort
    Private _Side As Byte
    Private _StoredCellValue As String
    Private _SurfaceData As BitArray
    Private _TopSector As BitstreamRegionSector = Nothing
    Private _Track As UShort
    Private _TrackType As BitstreamTrackType
    Private _WeakBitRegions As List(Of HighlightRange)
    Public Sub New(Disk As Disk, Track As UShort, Side As Byte, AllTracks As Boolean)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _ToolTip = New ToolTip()
        _FloppyImage = Disk.Image
        _SectorsPerTrack = BuildBPB(Disk.DiskFormat).SectorsPerTrack
        _Track = Track
        _Side = Side
        _AllTracks = AllTracks
        _Data = Nothing
        _RegionData = Nothing
        _SectorLabels = New List(Of Label)

        HexBox1.ReadOnly = True
        HexBox1.LineInfoOffset = 0

        BtnCopyEncoded.Visible = My.Settings.Debug

        EnableDoubleBuffering(PanelSectors)

        Me.Text = "Raw Track Data"
    End Sub

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function AddClipboardFormatListener(hwnd As IntPtr) As Boolean
    End Function
    Private Sub ChangeOffset(Offset As UInteger)
        If _CurrentTrackData IsNot Nothing AndAlso _CurrentTrackData.Offset <> Offset Then
            _CurrentTrackData.Offset = Offset

            LoadTrack(_CurrentTrackData, True, True)
        End If
    End Sub

    Private Sub DataInspectorRefresh(ForceUpdate As Boolean)
        If Not _Initialized Then
            Exit Sub
        End If

        Dim SelectionStart = HexBox1.SelectionStart
        Dim SelectionLength As Long
        If _CachedSelectedLength = -1 Then
            SelectionLength = HexBox1.SelectionLength
        Else
            SelectionLength = _CachedSelectedLength
        End If

        If SelectionStart = _CurrentSelectionStart And SelectionLength = _CurrentSelectionLength And Not ForceUpdate Then
            Exit Sub
        End If

        _CurrentSelectionStart = SelectionStart
        _CurrentSelectionLength = SelectionLength

        Dim Bytes() As Byte = Nothing
        Dim SelectedBytes() As Byte = Nothing

        Dim Length As Long
        'Dim BigEndien As Boolean = RadioButtonBigEndien.Checked
        Dim BigEndien As Boolean = False

        Dim HasSelection = SelectionLength > 0
        If SelectionLength = 0 Then
            SelectionLength = 1
        End If
        Dim SelectionEnd = SelectionStart + SelectionLength - 1

        If SelectionStart > -1 Then
            Dim OutOfRange As Boolean = SelectionEnd >= HexBox1.ByteProvider.Length
            If Not OutOfRange Then
                If HasSelection Then
                    Length = Math.Min(SelectionLength, 8)
                Else
                    Length = Math.Min(HexBox1.ByteProvider.Length - SelectionStart, 8)
                End If

                SelectedBytes = New Byte(SelectionLength - 1) {}
                Bytes = New Byte(Length - 1) {}
                Array.Copy(_Data, SelectionStart, SelectedBytes, 0, SelectionLength)
                Array.Copy(_Data, SelectionStart, Bytes, 0, Length)
            End If
        End If

        _DataGridInspector.Refresh(Bytes, SelectedBytes, HexBox1.ReadOnly, BigEndien, True)

        DataInspectorRefreshButtons()
    End Sub

    Private Sub DataInspectorRefreshButtons()
        Dim Enabled As Boolean = False

        If DataGridDataInspector.SelectedRows.Count > 0 Then
            Dim Row = DataGridDataInspector.SelectedRows.Item(0)
            Dim Invalid As Boolean = Row.Cells.Item("DataGridInvalid").Value
            If Not Invalid Then
                Enabled = True
            End If
        End If

        BtnCopyValue.Enabled = Enabled
    End Sub

    Private Sub EnableDoubleBuffering(panel As Panel)
        ' Get the type of the panel
        Dim panelType As Type = panel.GetType()

        ' Get the DoubleBuffered property via reflection
        Dim propertyInfo As Reflection.PropertyInfo = panelType.GetProperty("DoubleBuffered", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)

        ' Set the property value to True
        propertyInfo?.SetValue(panel, True, Nothing)
    End Sub

    Public Sub CopyHexEncoded(HexBox As HexBox, Formatted As Boolean)
        Dim Bitstream = BitstreamAlign(_Bitstream, _CurrentTrackData.Offset)
        Dim Data = BitsToBytes(_Bitstream, 0)

        Dim StartIndex = HexBox.SelectionStart * 2
        Dim Length = HexBox.SelectionLength * 2

        If StartIndex + Length > Data.Length Then
            Length = Data.Length - StartIndex
        End If

        Dim Capacity As Integer = Length * 2 + Length
        If Formatted Then
            Capacity += Length \ 16
        End If

        Dim SB = New System.Text.StringBuilder(Capacity)
        For Counter = 0 To Length - 1
            Dim B = Data(StartIndex + Counter)
            SB.Append(B.ToString("X2"))
            If Formatted AndAlso (Counter + 1) Mod 16 = 0 Then
                SB.Append(vbNewLine)
            Else
                SB.Append(" ")
            End If
        Next
        Clipboard.SetText(SB.ToString)
    End Sub

    Private Function GetBits(BitArray As BitArray, Offset As UInteger, CheckSync As Boolean) As String
        If BitArray Is Nothing Then
            Return ""
        End If

        Dim lastBit As Boolean
        Dim clockBit As Boolean
        Dim dataBit As Boolean
        Dim clockSeparator As String
        Dim dataSeparator As String
        Dim Value As String = ""
        Dim hasSyncBit As Boolean
        Dim BitIndex = Offset * 16 + _CurrentTrackData.Offset - 1

        BitIndex = AdjustBitIndex(BitIndex, BitArray.Length)

        lastBit = BitArray(BitIndex)
        BitIndex += 1
        If BitIndex > BitArray.Length - 1 Then
            BitIndex = 0
        End If

        For i = 0 To 7
            clockSeparator = ""
            dataSeparator = ""
            If i > 0 Then
                If BitIndex = 0 Then
                    clockSeparator = " > "
                Else
                    clockSeparator = " "
                End If
            End If
            clockBit = BitArray(BitIndex)
            BitIndex += 1
            If BitIndex > BitArray.Length - 1 Then
                BitIndex = 0
            End If

            If BitIndex = 0 Then
                dataSeparator = " > "
            End If
            dataBit = BitArray(BitIndex)
            BitIndex += 1
            If BitIndex > BitArray.Length - 1 Then
                BitIndex = 0
            End If

            hasSyncBit = CheckSync And Not lastBit And Not clockBit And Not dataBit


            Value &= clockSeparator
            If hasSyncBit Then
                Value &= "["
            End If
            Value &= If(clockBit, 1, 0)
            Value &= dataSeparator
            Value &= If(dataBit, 1, 0)
            If hasSyncBit Then
                Value &= "]"
            End If
            lastBit = dataBit
        Next

        Return Value
    End Function

    Private Function GetRegionColor(RegionType As MFMRegionType) As Color
        Select Case RegionType
            Case MFMRegionType.Gap1, MFMRegionType.Gap2, MFMRegionType.Gap3, MFMRegionType.Gap4A, MFMRegionType.Gap4B
                Return Color.Green
            Case MFMRegionType.IAMNulls, MFMRegionType.IDAMNulls, MFMRegionType.DAMNulls
                Return Color.Gray
            Case MFMRegionType.IAMSync, MFMRegionType.IDAMSync, MFMRegionType.DAMSync
                Return Color.MediumBlue
            Case MFMRegionType.IAM, MFMRegionType.IDAM, MFMRegionType.DAM
                Return Color.Blue
            Case MFMRegionType.IDArea
                Return Color.DarkOrange
            Case MFMRegionType.IDAMChecksumValid, MFMRegionType.DataChecksumValid
                Return Color.Purple
            Case MFMRegionType.IDAMChecksumInvalid, MFMRegionType.DataChecksumInvalid
                Return Color.Red
            Case MFMRegionType.DataArea
                Return Color.Black
            Case MFMRegionType.Overflow
                Return Color.Gray
            Case Else
                Return Color.Black
        End Select
    End Function
    Private Function GetRegionDescription(Region As BitstreamRegion) As String
        Select Case Region.RegionType
            Case MFMRegionType.Gap1
                Return "GAP 1" & "  (" & Region.Length & ")"
            Case MFMRegionType.Gap2
                Return "GAP 2" & "  (" & Region.Length & ")"
            Case MFMRegionType.Gap3
                Return "GAP 3" & "  (" & Region.Length & ")"
            Case MFMRegionType.Gap4A
                Return "GAP 4A" & "  (" & Region.Length & ")"
            Case MFMRegionType.Gap4B
                Return "GAP 3+4B" & "  (" & Region.Length & ")"
            Case MFMRegionType.IAMNulls, MFMRegionType.IAMSync
                Return "Index Field Sync"
            Case MFMRegionType.IAM
                Return "Index Address Mark"
            Case MFMRegionType.IDAMNulls, MFMRegionType.IDAMSync
                Return "ID Field Sync"
            Case MFMRegionType.IDAM
                Return "ID Address Mark"
            Case MFMRegionType.IDArea
                Return "ID Area"
            Case MFMRegionType.IDAMChecksumValid, MFMRegionType.IDAMChecksumInvalid
                Return "ID Area Checksum"
            Case MFMRegionType.DAMNulls, MFMRegionType.DAMSync
                Return "Data Field Sync"
            Case MFMRegionType.DAM
                Return "Data Address Mark"
            Case MFMRegionType.DataArea
                Return "Data Area" & "  (" & Region.Length & ")"
            Case MFMRegionType.DataChecksumValid, MFMRegionType.DataChecksumInvalid
                Return "Data Checksum"
            Case MFMRegionType.Overflow
                Return "Overflow"
            Case Else
                Return ""
        End Select
    End Function

    Private Function GetSectorIndex(MousePos As Point) As Integer
        If MousePos.X >= PanelSectors.Width - PanelSectors.Padding.Right Then
            Return -1
        End If

        MousePos.Offset(-PanelSectors.Padding.Left, -PanelSectors.Padding.Top)
        Dim SectorWidth = SECTOR_WIDTH + PADDING_COLS
        Dim SectorHeight = SECTOR_HEIGHT + PADDING_ROWS
        Dim ColIndex = MousePos.X \ SectorWidth
        Dim RowIndex = MousePos.Y \ SectorHeight
        Dim SectorRect = New Rectangle(ColIndex * SectorWidth, RowIndex * SectorHeight, SECTOR_WIDTH, SECTOR_HEIGHT)
        If SectorRect.Contains(MousePos) Then
            Return RowIndex * PanelSectorsPerRow() + ColIndex
        End If

        Return -1
    End Function

    Private Function GetWeakBitRegions(Bitstream As BitArray, Offset As UInteger) As List(Of HighlightRange)
        If _SurfaceData Is Nothing Then
            Return Nothing
        End If

        Dim RangeList As New List(Of HighlightRange)
        Dim Range As HighlightRange = Nothing


        Dim BitValue As Boolean
        Dim NumBytes As UInteger = Bitstream.Length \ 16
        Dim HasWeakBit As Boolean

        Dim BitIndex = Offset
        For i = 0 To NumBytes - 1
            HasWeakBit = False
            For j = 0 To 15
                BitValue = _SurfaceData(BitIndex)
                If BitValue Then
                    HasWeakBit = True
                End If
                BitIndex += 1
                If BitIndex > _SurfaceData.Length - 1 Then
                    BitIndex = 0
                End If
            Next
            If HasWeakBit Then
                If Range Is Nothing Then
                    Range = New HighlightRange(i)
                    RangeList.Add(Range)
                ElseIf i > Range.EndIndex + 1 Then
                    Range = New HighlightRange(i)
                    RangeList.Add(Range)
                Else
                    Range.EndIndex = i
                End If
            End If
        Next

        Return RangeList
    End Function
    Private Sub HexBoxHighlight(start As Long, size As Long, ForeColor As Color, BackColor As Color)
        HexBox1.HighlightForeColor = ForeColor
        HexBox1.HighlightBackColor = BackColor
        HexBox1.Highlight(start, size)
    End Sub

    Private Sub HexViewRawForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        AddClipboardFormatListener(Me.Handle)

        _DataGridInspector = New HexViewDataGridInspector(DataGridDataInspector)
        _DataGridInspector.SetDataRow(DataRowEnum.File, Nothing, True, True)
        _DataGridInspector.SetDataRow(DataRowEnum.Description, Nothing, True, True)

        HexBox1.VScrollBarVisible = True

        Dim ShowAllTracks As Boolean = True
        If _FloppyImage.AdditionalTracks.Count = 0 And _FloppyImage.NonStandardTracks.Count = 0 Then
            _AllTracks = True
            ShowAllTracks = False
        End If

        InitializeTrackNavigator()
        If ShowAllTracks Then
            InitializeAllTracksCheckBox()
        End If
        InitializeBitOffsetNavigator()
        PopulateTracks(_AllTracks)
    End Sub
    Private Sub HighlightRegions()
        Dim BitOffset As Integer = 0

        If _CurrentTrackData IsNot Nothing Then
            BitOffset = _CurrentTrackData.Offset
        End If

        Dim WeakBitIndex As UInteger = 0
        Dim WeakBitStartIndex As Integer = -1
        Dim WeakBitLength As Integer = 0
        Dim Range As HighlightRange
        Dim RegionForeColor As Color
        Dim RegionBackColor As Color
        Dim Length As UInteger

        If _WeakBitRegions IsNot Nothing Then
            If WeakBitIndex < _WeakBitRegions.Count Then
                Range = _WeakBitRegions(WeakBitIndex)
                WeakBitStartIndex = Range.StartIndex
                WeakBitLength = Range.EndIndex - Range.StartIndex + 1
            End If
        End If

        For Each BitstreamRegion In _RegionData.Regions
            If BitstreamRegion.Length > 0 Then
                RegionForeColor = GetRegionColor(BitstreamRegion.RegionType)
                If BitstreamRegion.BitOffset <> BitOffset Then
                    RegionBackColor = Color.FromArgb(245, 245, 245)
                Else
                    RegionBackColor = Color.White
                End If

                Dim RegionStartIndex = BitstreamRegion.StartIndex
                Dim RegionLength = BitstreamRegion.Length

                If WeakBitStartIndex > -1 Then
                    Do While WeakBitLength > 0 And WeakBitStartIndex >= BitstreamRegion.StartIndex And WeakBitStartIndex <= BitstreamRegion.StartIndex + BitstreamRegion.Length - 1
                        If RegionStartIndex < WeakBitStartIndex Then
                            Length = WeakBitStartIndex - RegionStartIndex
                            HexBoxHighlight(RegionStartIndex, Length, RegionForeColor, RegionBackColor)
                            RegionStartIndex += Length
                            RegionLength -= Length
                        Else
                            Length = Math.Min(WeakBitLength, RegionLength)
                            HexBoxHighlight(RegionStartIndex, Length, RegionForeColor, Color.LightYellow)
                            RegionStartIndex += Length
                            RegionLength -= Length
                            WeakBitLength -= Length
                            If WeakBitLength > 0 Then
                                WeakBitStartIndex += Length
                            Else
                                WeakBitIndex += 1
                                If WeakBitIndex < _WeakBitRegions.Count Then
                                    Range = _WeakBitRegions(WeakBitIndex)
                                    WeakBitStartIndex = Range.StartIndex
                                    WeakBitLength = Range.EndIndex - Range.StartIndex + 1
                                Else
                                    WeakBitStartIndex = -1
                                End If
                            End If
                        End If
                    Loop
                End If

                If RegionLength > 0 Then
                    HexBoxHighlight(RegionStartIndex, RegionLength, RegionForeColor, RegionBackColor)
                End If
            End If
        Next
    End Sub
    Private Sub InitializeAllTracksCheckBox()
        CheckBoxAllTracks = New ToolStripCheckBox With {
            .Alignment = ToolStripItemAlignment.Right,
            .Checked = _AllTracks,
            .Margin = New Padding(12, 3, 0, 2),
            .Text = "All Tracks"
        }

        ToolStripMain.Items.Add(CheckBoxAllTracks)
    End Sub

    Private Sub InitializeBitOffsetNavigator()
        NumericBitOffset = New ToolStripNumericUpDown() With {
            .Alignment = ToolStripItemAlignment.Right,
            .AutoSize = False,
            .Size = New Drawing.Size(55, 21),
            .Minimum = 0,
            .Maximum = 15
        }

        _LabelBitOffsetAligned = New ToolStripLabel("Aligned") With {
            .Alignment = ToolStripItemAlignment.Right,
            .AutoSize = True,
            .Padding = New Padding(0, 0, 36, 0),
            .ForeColor = Color.Green,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        _LabelBitOffset = New ToolStripLabel("Bit Offset") With {
            .Alignment = ToolStripItemAlignment.Right,
            .Padding = New Padding(12, 0, 0, 0)
        }

        Dim Button = ToolStripBtnAdjustOffset
        ToolStripMain.Items.Remove(Button)

        ToolStripMain.Items.Add(Button)
        ToolStripMain.Items.Add(NumericBitOffset)
        ToolStripMain.Items.Add(_LabelBitOffsetAligned)
        ToolStripMain.Items.Add(_LabelBitOffset)
    End Sub

    Private Sub InitializeTrackNavigator()
        ComboTrack = New ToolStripComboBox() With {
            .Alignment = ToolStripItemAlignment.Right,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .AutoSize = False,
            .FlatStyle = FlatStyle.Standard,
            .Size = New Drawing.Size(50, 23)
        }

        Dim LabelTrack = New ToolStripLabel("Track") With {
            .Alignment = ToolStripItemAlignment.Right,
            .Padding = New Padding(6, 0, 0, 0)
        }

        ToolStripMain.Items.Add(ComboTrack)
        ToolStripMain.Items.Add(LabelTrack)
    End Sub

    Private Sub InitRegionMap()
        _RegionMap = New BitstreamRegion(HexBox1.ByteProvider.Length - 1) {}

        Dim BitOffset = -1
        Dim OffsetsMatch As Boolean = True

        For Each BitstreamRegion In _RegionData.Regions
            If BitOffset = -1 Then
                BitOffset = BitstreamRegion.BitOffset
            Else
                If BitstreamRegion.BitOffset <> BitOffset Then
                    OffsetsMatch = False
                End If
            End If
            For Counter = 0 To BitstreamRegion.Length - 1
                _RegionMap(BitstreamRegion.StartIndex + Counter) = BitstreamRegion
            Next
        Next

        _TopSector = Nothing

        _SectorLabels.Clear()

        If _RegionData.Sectors.Count = 0 Then
            NumericBitOffset.Visible = False
            _LabelBitOffset.Visible = False
            _LabelBitOffsetAligned.Visible = False
            ToolStripBtnAdjustOffset.Visible = False
        Else
            Dim Aligned = _RegionData.Aligned And _RegionData.NumBits Mod 16 = 0
            NumericBitOffset.Visible = Not Aligned
            NumericBitOffset.Enabled = Not OffsetsMatch
            _LabelBitOffset.Visible = True
            _LabelBitOffsetAligned.Visible = Aligned
            ToolStripBtnAdjustOffset.Visible = Not Aligned
        End If
    End Sub
    Private Sub JumpToSector(Sector As BitstreamRegionSector)
        _IgnoreEvent = True

        If Sector IsNot Nothing Then
            Dim Offset = Sector.StartIndex
            Dim MaxLineCount = Math.Ceiling(HexBox1.ByteProvider.Length / HexBox1.BytesPerLine)
            Dim Line = Offset \ HexBox1.BytesPerLine
            If MaxLineCount - Line < HexBox1.VerticalByteCount Then
                Line -= HexBox1.VerticalByteCount - (MaxLineCount - Line)
            End If
            HexBox1.PerformScrollToLine(Line)
            HexBox1.Select(Offset, 0)
        End If

        _IgnoreEvent = False
    End Sub

    Private Sub LoadData(Data() As Byte, RegionData As RegionData, KeepGridLocation As Boolean)
        _Initialized = False
        _Data = Data
        _RegionData = RegionData

        Dim StartByte = HexBox1.StartByte
        Dim StartLine = StartByte \ HexBox1.BytesPerLine
        Dim SelectionStart = HexBox1.SelectionStart
        Dim SelectionLength = HexBox1.SelectionLength

        HexBox1.ByteProvider = Nothing
        HexBox1.ByteProvider = New DynamicByteProvider(Data)

        If KeepGridLocation Then
            HexBox1.PerformScrollToLine(StartLine)
            HexBox1.Select(SelectionStart, SelectionLength)
        End If

        ToolStripStatusBits.Text = Format(RegionData.NumBits, "N0") & " bits"
        ToolStripStatusBytes.Text = Format(Math.Ceiling(RegionData.NumBits / 16), "N0") & " bytes"
        _LastSearch = New HexSearch

        RefreshSize()
        InitRegionMap()
        HighlightRegions()
        RefreshSelection(True)
        DataInspectorRefresh(True)
        RefreshSelectorValues()
        Me.Refresh()
    End Sub

    Private Sub LoadTrack(TrackData As TrackData, ForceReload As Boolean, KeepGridLocation As Boolean)

        If _Initialized And TrackData.Track = _Track And TrackData.Side = _Side And Not ForceReload Then
            Exit Sub
        End If

        Me.Text = "Raw Track Data - Track " & TrackData.Track & "." & TrackData.Side

        _CurrentTrackData = TrackData

        _Track = TrackData.Track
        _Side = TrackData.Side

        Dim MFMTrack = _FloppyImage.BitstreamImage.GetTrack(TrackData.Track * _FloppyImage.BitstreamImage.TrackStep, TrackData.Side)

        Dim Data() As Byte
        Dim RegionData As RegionData
        _TrackType = MFMTrack.TrackType
        If MFMTrack.TrackType = BitstreamTrackType.MFM Or MFMTrack.TrackType = BitstreamTrackType.FM Then
            If TrackData.Offset = -1 Then
                If MFMTrack.TrackType = BitstreamTrackType.MFM Then
                    TrackData.Offset = MFMGetOffset(MFMTrack.Bitstream)
                Else
                    TrackData.Offset = FMGetOffset(MFMTrack.Bitstream)
                End If
            End If
            Dim NumBytes = MFMTrack.Bitstream.Length \ 16
            RegionData = MFMGetRegionList(MFMTrack.Bitstream, MFMTrack.TrackType)
            Data = MFMGetBytes(MFMTrack.Bitstream, TrackData.Offset, RegionData.NumBytes)
            _IgnoreEvent = True
            NumericBitOffset.Value = TrackData.Offset
            _IgnoreEvent = False
            _Bitstream = MFMTrack.Bitstream
            _SurfaceData = MFMTrack.SurfaceData
            _WeakBitRegions = GetWeakBitRegions(MFMTrack.Bitstream, TrackData.Offset)
        Else
            Data = BitsToBytes(MFMTrack.Bitstream, 0)
            RegionData.NumBits = MFMTrack.Bitstream.Length
            RegionData.Regions = New List(Of BitstreamRegion)
            RegionData.Sectors = New List(Of BitstreamRegionSector)
            _Bitstream = Nothing
            _SurfaceData = Nothing
        End If

        RegionData.Track = TrackData.Track
        RegionData.Side = TrackData.Side

        LoadData(Data, RegionData, KeepGridLocation)
    End Sub

    Private Function PanelSectorsPerRow() As Integer
        Dim MaxWidth As Integer = PanelSectors.Width - PanelSectors.Padding.Horizontal + PADDING_COLS
        Dim SectorWidth As Integer = SECTOR_WIDTH + PADDING_COLS

        Return MaxWidth \ SectorWidth
    End Function

    Private Sub PopulateTracks(AllTracks As Boolean)
        Dim SelectedIndex As Integer = -1
        ComboTrack.Items.Clear()
        For i = 0 To _FloppyImage.TrackCount - 1
            For j = 0 To _FloppyImage.SideCount - 1
                Dim TrackIndex = i * _FloppyImage.SideCount + j
                If AllTracks OrElse (_FloppyImage.NonStandardTracks.Contains(TrackIndex) Or _FloppyImage.AdditionalTracks.Contains(TrackIndex)) Then
                    Dim TrackData As New TrackData With {
                        .Track = i,
                        .Side = j,
                        .Offset = -1
                    }
                    ComboTrack.Items.Add(TrackData)
                    If i = _Track And j = _Side Then
                        SelectedIndex = ComboTrack.Items.Count - 1
                    End If
                End If
            Next
        Next
        If ComboTrack.Items.Count > 0 And SelectedIndex = -1 Then
            SelectedIndex = 0
        End If

        ComboTrack.SelectedIndex = SelectedIndex
    End Sub

    Private Sub ProcessKeyPress(e As KeyEventArgs)
        If e.Control And e.KeyCode = Keys.C Then
            If HexBox1.CanCopy Then
                If e.Shift Then
                    CopyHex(HexBox1, True)
                Else
                    CopyHex(HexBox1, False)
                End If
            End If
            e.SuppressKeyPress = True
        End If
    End Sub
    Private Sub RefreshBits(BitArray As BitArray, DataRow As DataRowEnum, CheckSync As Boolean)
        If BitArray Is Nothing Then
            _DataGridInspector.SetDataRow(DataRow, Nothing, True, True)
        Else
            Dim SelectionStart = HexBox1.SelectionStart
            Dim SelectionLength = HexBox1.SelectionLength
            Dim OutOfRange As Boolean = SelectionStart >= HexBox1.ByteProvider.Length

            If SelectionStart > -1 And SelectionLength < 2 And Not OutOfRange Then
                Dim Bits = GetBits(BitArray, SelectionStart, CheckSync)
                _DataGridInspector.SetDataRow(DataRow, Bits, True, False)
            Else
                _DataGridInspector.SetDataRow(DataRow, Nothing, True, False)
            End If
        End If
    End Sub

    Private Sub RefreshSelection(ForceUpdate As Boolean)
        If Not _Initialized And Not ForceUpdate Then
            Exit Sub
        End If

        _IgnoreEvent = True

        _CurrentRegionSector = Nothing

        Dim SelectionStart = HexBox1.SelectionStart
        Dim SelectionLength = HexBox1.SelectionLength

        Dim SelectionEnd = SelectionStart + SelectionLength - 1

        If SelectionEnd > HexBox1.ByteProvider.Length - 1 Then
            SelectionEnd = HexBox1.ByteProvider.Length - 1
        ElseIf SelectionEnd < SelectionStart Then
            SelectionEnd = SelectionStart
        End If

        Dim RegionStart As BitstreamRegion = Nothing
        Dim RegionEnd As BitstreamRegion = Nothing

        If SelectionStart = -1 Then
            ToolStripStatusOffset.Visible = False
            ToolStripStatusBlock.Visible = False
            ToolStripStatusLength.Visible = False
        Else
            Dim OffsetStart As UInteger = HexBox1.LineInfoOffset + SelectionStart
            Dim OffsetEnd As Integer = HexBox1.LineInfoOffset + SelectionEnd
            Dim OutOfRange As Boolean = SelectionStart >= HexBox1.ByteProvider.Length

            ToolStripStatusOffset.Visible = Not OutOfRange
            ToolStripStatusOffset.Text = "Offset(h) :  " & OffsetStart.ToString("X")

            If SelectionLength = 0 Then
                ToolStripStatusBlock.Visible = False
                ToolStripStatusBlock.Text = ""
                ToolStripStatusLength.Visible = False
                ToolStripStatusLength.Text = ""
            Else
                ToolStripStatusBlock.Visible = True
                ToolStripStatusBlock.Text = "Block(h): " & OffsetStart.ToString("X") & "-" & OffsetEnd.ToString("X")
                ToolStripStatusLength.Visible = True
                ToolStripStatusLength.Text = "Length(h): " & SelectionLength.ToString("X") & "  (" & SelectionLength & ")"
            End If

            If Not OutOfRange Then
                RegionStart = _RegionMap(SelectionStart)
                RegionEnd = _RegionMap(SelectionEnd)
            End If
        End If

        If RegionStart IsNot Nothing Then
            Dim Description = GetRegionDescription(RegionStart)
            If RegionStart Is RegionEnd AndAlso Description.Length > 0 Then
                _DataGridInspector.SetDataRow(DataRowEnum.Description, Description, True, True)
                BtnSelectRegion.Enabled = True
            Else
                _DataGridInspector.SetDataRow(DataRowEnum.Description, Nothing, True, True)
                BtnSelectRegion.Enabled = False
            End If

            _CurrentRegionSector = RegionStart.Sector

            If _CurrentTrackData Is Nothing Then
                BtnAdjustOffset.Enabled = False
            Else
                BtnAdjustOffset.Enabled = RegionStart.BitOffset <> _CurrentTrackData.Offset
            End If
        Else
            _DataGridInspector.SetDataRow(DataRowEnum.Description, Nothing, True, True)
            BtnSelectRegion.Enabled = False
            BtnAdjustOffset.Enabled = False
        End If

        If _CurrentRegionSector Is Nothing Then
            ToolStripStatusTrack.Visible = True
            ToolStripStatusTrack.Text = "Track: " & _RegionData.Track
            ToolStripStatusSide.Visible = True
            ToolStripStatusSide.Text = "Side: " & _RegionData.Side
            ToolStripStatusTrackSector.Visible = False
            ToolStripStatusTrackSize.Visible = False
            ToolStripStatusChecksumText.Visible = False
            ToolStripStatusChecksum.Visible = False
            BtnSelectData.Enabled = False
            BtnSelectSector.Enabled = False
        Else
            ToolStripStatusTrack.Visible = True
            ToolStripStatusTrack.Text = "Track: " & _CurrentRegionSector.Track
            ToolStripStatusSide.Visible = True
            ToolStripStatusSide.Text = "Side: " & _CurrentRegionSector.Side
            ToolStripStatusTrackSector.Visible = True
            ToolStripStatusTrackSector.Text = "Sector Id: " & _CurrentRegionSector.SectorId
            ToolStripStatusTrackSize.Visible = True
            ToolStripStatusTrackSize.Text = "Size: " & _CurrentRegionSector.DataLength
            ToolStripStatusChecksumText.Visible = True
            ToolStripStatusChecksum.Visible = True
            If _CurrentRegionSector.DataChecksumValid Then
                ToolStripStatusChecksum.Text = "Valid"
                ToolStripStatusChecksum.ForeColor = Color.Green
            Else
                ToolStripStatusChecksum.Text = "Invalid"
                ToolStripStatusChecksum.ForeColor = Color.Red
            End If
            If RegionEnd IsNot Nothing Then
                BtnSelectData.Enabled = RegionStart.Sector Is RegionEnd.Sector And _CurrentRegionSector.DataLength > 0 And _CurrentRegionSector.DataStartIndex > 0
                BtnSelectSector.Enabled = RegionStart.Sector Is RegionEnd.Sector
            Else
                BtnSelectData.Enabled = False
                BtnSelectSector.Enabled = False
            End If
        End If

        ToolStripBtnSelectRegion.Enabled = BtnSelectRegion.Enabled
        ToolStripBtnSelectSector.Enabled = BtnSelectSector.Enabled
        ToolStripBtnSelectData.Enabled = BtnSelectData.Enabled
        ToolStripBtnAdjustOffset.Enabled = BtnAdjustOffset.Enabled

        BtnCopyText.Enabled = HexBox1.CanCopy
        ToolStripBtnCopyText.Enabled = BtnCopyText.Enabled

        BtnCopyHex.Enabled = HexBox1.CanCopy
        ToolStripBtnCopyHex.Enabled = BtnCopyHex.Enabled

        BtnCopyHexFormatted.Enabled = HexBox1.CanCopy
        ToolStripBtnCopyHexFormatted.Enabled = BtnCopyHexFormatted.Enabled

        BtnCopyEncoded.Enabled = HexBox1.CanCopy

        RefreshBits(_Bitstream, DataRowEnum.Bitstream, True)
        RefreshBits(_SurfaceData, DataRowEnum.WeakBits, False)

        _IgnoreEvent = False

        _Initialized = True
    End Sub

    Private Sub RefreshSelectorValues()
        Dim Offset = HexBox1.LineInfoOffset + HexBox1.StartByte + HexBox1.BytesPerLine

        If Offset > _RegionMap.Length - 1 Then
            Exit Sub
        End If

        Dim RegionData = _RegionMap(Offset)

        If RegionData Is Nothing Then
            Exit Sub
        End If

        _IgnoreEvent = True

        _TopSector = RegionData.Sector
        PanelSectors.Refresh()

        _IgnoreEvent = False
    End Sub
    Private Sub RefreshSize()
        Dim MaxSectors As Integer = PanelSectorsPerRow()

        If _RegionData.Sectors.Count = 0 Then
            PanelSectors.Height = 0
            PanelSectors.Visible = False
        Else
            Dim NumRows As Integer = Math.Ceiling(_RegionData.Sectors.Count / MaxSectors)
            Dim PanelHeight = SECTOR_HEIGHT * NumRows + PADDING_ROWS * (NumRows - 1) + PanelSectors.Padding.Top + PanelSectors.Padding.Bottom

            PanelSectors.Height = PanelHeight
            PanelSectors.Visible = True
        End If

        Dim TopPos = PanelSectors.Top + PanelSectors.Height + 3
        Dim Height = StatusStripBottom.Top - TopPos - 3

        If TopPos <> HexBox1.Top Or Height <> HexBox1.Height Then
            If TopPos > DataGridDataInspector.Top Then
                HexBox1.Height = Height
                HexBox1.Top = TopPos
            Else
                HexBox1.Top = TopPos
                HexBox1.Height = Height
            End If
            HexBox1.Refresh()
        End If

        If TopPos <> DataGridDataInspector.Top Or Height <> DataGridDataInspector.Height Then
            If TopPos > DataGridDataInspector.Top Then
                DataGridDataInspector.Height = Height
                DataGridDataInspector.Top = TopPos
            Else
                DataGridDataInspector.Top = TopPos
                DataGridDataInspector.Height = Height
            End If
            DataGridDataInspector.Refresh()
        End If
    End Sub

    Private Sub Search(FindNext As Boolean)
        Dim Result As Boolean = True
        Dim SelectionStart = HexBox1.SelectionStart
        Dim SelectionLength = HexBox1.SelectionLength
        Dim TopLine = HexBox1.StartByte \ HexBox1.BytesPerLine

        _IgnoreEvent = True

        If FindNext And _LastSearch.SearchString <> "" Then
            Result = SearchNext(_LastSearch)
        Else
            Dim frmHexSearchForm As New HexSearchForm(_LastSearch)
            frmHexSearchForm.ShowDialog()

            If frmHexSearchForm.DialogResult = DialogResult.OK Then
                _LastSearch = frmHexSearchForm.Search
                Result = SearchNext(_LastSearch)
            End If
        End If

        _IgnoreEvent = False

        If Not Result Then
            HexBox1.PerformScrollToLine(TopLine)
            HexBox1.SelectionStart = SelectionStart
            HexBox1.SelectionLength = SelectionLength
            MsgBox("Can't Find '" & _LastSearch.SearchString & "'", MsgBoxStyle.Information)
        Else
            RefreshSelection(False)
            DataInspectorRefresh(False)
            RefreshSelectorValues()
        End If
    End Sub

    Private Function SearchNext(Search As HexSearch) As Boolean
        Dim Result As Boolean = True
        Dim DoSearch As Boolean = True

        If Search.SearchString <> "" Then
            Dim Options As New FindOptions()
            If Search.SearchHex Then
                Options.Type = FindType.Hex
                Options.Hex = ConvertHexToBytes(Search.SearchString)
                If Options.Hex Is Nothing Then
                    DoSearch = False
                End If
            Else
                Options.Type = FindType.Text
                Options.Text = Search.SearchString
                Options.MatchCase = Search.CaseSensitive
            End If
            If DoSearch Then
                Dim Index = HexBox1.Find(Options)
                If Index = -1 Then
                    If HexBox1.SelectionStart > 0 Then
                        HexBox1.SelectionStart = 0
                        HexBox1.SelectionLength = 0
                        Result = SearchNext(Search)
                    Else
                        Result = False
                    End If
                End If
            End If
        End If

        Return Result
    End Function

    Private Sub SelectData()
        If _CurrentRegionSector IsNot Nothing Then
            Dim MaxLength = HexBox1.ByteProvider.Length
            Dim DataLength = _CurrentRegionSector.DataLength
            If _CurrentRegionSector.DataStartIndex + DataLength > MaxLength Then
                DataLength = MaxLength - _CurrentRegionSector.DataStartIndex
            End If
            HexBox1.Select(_CurrentRegionSector.DataStartIndex, DataLength)
        End If
    End Sub

    Private Sub SelectRegion()
        Dim SelectionStart = HexBox1.SelectionStart
        If SelectionStart > -1 Then
            Dim OutOfRange As Boolean = SelectionStart >= HexBox1.ByteProvider.Length
            If Not OutOfRange Then
                Dim RegionStart = _RegionMap(SelectionStart)
                If RegionStart IsNot Nothing Then
                    HexBox1.Select(RegionStart.StartIndex, RegionStart.Length)
                End If
            End If
        End If
    End Sub

    Private Sub SelectSector()
        If _CurrentRegionSector IsNot Nothing Then
            HexBox1.Select(_CurrentRegionSector.StartIndex, _CurrentRegionSector.Length)
        End If
    End Sub

    Private Sub SelectTopSector(SectorIndex As Integer)
        _TopSector = _RegionData.Sectors.Item(SectorIndex)
        JumpToSector(_TopSector)
        RefreshSelection(False)
        DataInspectorRefresh(False)
        PanelSectors.Refresh()
    End Sub

    Private Function IsStandardSector(Sector As BitstreamRegionSector) As Boolean
        If Sector.DataLength <> 512 Then
            Return False
        End If

        If Sector.SectorId < 1 Or Sector.SectorId > _SectorsPerTrack Then
            Return False
        End If

        If Sector.Track <> _Track Then
            Return False
        End If

        If Sector.Side <> _Side Then
            Return False
        End If

        If Sector.DAM <> MFMAddressMark.Data Then
            Return False
        End If

        Return True
    End Function
#Region "Events"

    Private Sub BtnAdjustOffset_Click(sender As Object, e As EventArgs) Handles BtnAdjustOffset.Click, ToolStripBtnAdjustOffset.Click
        Dim SelectionStart = HexBox1.SelectionStart
        If SelectionStart > -1 Then
            Dim OutOfRange As Boolean = SelectionStart >= HexBox1.ByteProvider.Length
            If Not OutOfRange Then
                Dim RegionStart = _RegionMap(SelectionStart)
                If RegionStart IsNot Nothing Then
                    ChangeOffset(RegionStart.BitOffset)
                End If
            End If
        End If
    End Sub

    Private Sub BtnCopyEncoded_Click(sender As Object, e As EventArgs) Handles BtnCopyEncoded.Click
        If HexBox1.CanCopy Then
            CopyHexEncoded(HexBox1, False)
        End If
    End Sub

    Private Sub BtnCopyHex_Click(sender As Object, e As EventArgs) Handles BtnCopyHex.Click, ToolStripBtnCopyHex.Click
        If HexBox1.CanCopy Then
            CopyHex(HexBox1, False)
        End If
    End Sub

    Private Sub BtnCopyHexFormatted_Click(sender As Object, e As EventArgs) Handles BtnCopyHexFormatted.Click, ToolStripBtnCopyHexFormatted.Click
        If HexBox1.CanCopy Then
            CopyHex(HexBox1, True)
        End If
    End Sub

    Private Sub BtnCopyText_Click(sender As Object, e As EventArgs) Handles BtnCopyText.Click, ToolStripBtnCopyText.Click
        If HexBox1.CanCopy Then
            HexBox1.Copy()
        End If
    End Sub

    Private Sub BtnCopyValue_Click(sender As Object, e As EventArgs) Handles BtnCopyValue.Click
        _DataGridInspector.CopyValueToClipboard()
    End Sub

    Private Sub BtnFind_Click(sender As Object, e As EventArgs) Handles BtnFind.Click, ToolStripBtnFind.Click
        Search(False)
    End Sub

    Private Sub BtnFindNext_Click(sender As Object, e As EventArgs) Handles BtnFindNext.Click, ToolStripBtnFindNext.Click
        Search(True)
    End Sub

    Private Sub BtnSelectAll_Click(sender As Object, e As EventArgs) Handles BtnSelectAll.Click, ToolStripBtnSelectAll.Click
        HexBox1.SelectAll()
    End Sub

    Private Sub BtnSelectData_Click(sender As Object, e As EventArgs) Handles BtnSelectData.Click, ToolStripBtnSelectData.Click
        SelectData()
    End Sub

    Private Sub BtnSelectRegion_Click(sender As Object, e As EventArgs) Handles BtnSelectRegion.Click, ToolStripBtnSelectRegion.Click
        SelectRegion()
    End Sub

    Private Sub BtnSelectSector_Click(sender As Object, e As EventArgs) Handles BtnSelectSector.Click, ToolStripBtnSelectSector.Click
        SelectSector()
    End Sub

    Private Sub CheckBoxAllTracks_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAllTracks.CheckedChanged
        If Not _Initialized Or _IgnoreEvent Then
            Exit Sub
        End If

        PopulateTracks(CheckBoxAllTracks.Checked)
    End Sub

    Private Sub ComboTrack_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboTrack.SelectedIndexChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        LoadTrack(ComboTrack.SelectedItem, False, False)
    End Sub

    Private Sub DataGridDataInspector_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles DataGridDataInspector.CellBeginEdit
        Dim Row = DataGridDataInspector.Rows(e.RowIndex)
        Dim Invalid As Boolean = Row.Cells.Item("DataGridInvalid").Value
        Dim Editable As Boolean = Row.Cells.Item("DataGridEditable").Value
        Dim Length As Integer = Row.Cells.Item("DataGridLength").Value

        If Not Editable Then
            e.Cancel = True
            Exit Sub
        End If

        _CachedSelectedLength = HexBox1.SelectionLength
        HexBox1.SelectionLength = Length

        _StoredCellValue = Row.Cells.Item("DataGridValue").Value
    End Sub

    Private Sub DataGridDataInspector_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridDataInspector.CellEndEdit
        Dim Row = DataGridDataInspector.Rows(e.RowIndex)
        Dim CellValue As String = Row.Cells.Item("DataGridValue").Value
        Dim DataType As DataRowEnum = Row.Cells.Item("DataGridType").Value

        If CellValue <> _StoredCellValue Then
            _CachedSelectedLength = -1
            'DataInspectorUpdateHexBox(CellValue, DataType)
        Else
            HexBox1.SelectionLength = _CachedSelectedLength
            _CachedSelectedLength = -1
        End If
    End Sub

    Private Sub DataGridDataInspector_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridDataInspector.SelectionChanged
        DataInspectorRefreshButtons()
    End Sub

    Private Sub HexBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles HexBox1.KeyDown
        ProcessKeyPress(e)
    End Sub

    Private Sub HexBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles HexBox1.MouseDown
        If e.Button = MouseButtons.Right Then
            If HexBox1.SelectionLength < 2 Then
                HexBox1.SetCaretPosition(New Drawing.Point(e.X, e.Y))
            End If
        End If
    End Sub

    Private Sub HexBox1_SelectionLengthChanged(sender As Object, e As EventArgs) Handles HexBox1.SelectionLengthChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        RefreshSelection(False)
        DataInspectorRefresh(False)
    End Sub

    Private Sub HexBox1_SelectionStartChanged(sender As Object, e As EventArgs) Handles HexBox1.SelectionStartChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        RefreshSelection(False)
        DataInspectorRefresh(False)
    End Sub

    Private Sub HexBox1_VisibilityBytesChanged(sender As Object, e As EventArgs) Handles HexBox1.VisibilityBytesChanged
        If Not _Initialized Or _IgnoreEvent Then
            Exit Sub
        End If

        RefreshSelectorValues()
    End Sub

    Private Sub NumericBitOffset_ValueChanged(sender As Object, e As EventArgs) Handles NumericBitOffset.ValueChanged
        If Not _Initialized Or _IgnoreEvent Then
            Exit Sub
        End If

        If ComboTrack.SelectedItem Is Nothing Then
            Exit Sub
        End If

        ChangeOffset(NumericBitOffset.Value)
    End Sub

    Private Sub PanelSectors_KeyDown(sender As Object, e As KeyEventArgs) Handles PanelSectors.KeyDown
        If _RegionData.Sectors Is Nothing Then
            Exit Sub
        End If

        Dim Index As Integer = -1

        If e.KeyCode = Keys.Left Then
            Index = _RegionData.Sectors.IndexOf(_TopSector)
            If Index = -1 Then
                Index = _RegionData.Sectors.Count - 1
            Else
                Index -= 1
            End If
        ElseIf e.KeyCode = Keys.Right Then
            Index = _RegionData.Sectors.IndexOf(_TopSector)
            If Index = -1 Then
                Index = 0
            Else
                Index += 1
            End If
        End If

        If Index > -1 And Index < _RegionData.Sectors.Count Then
            SelectTopSector(Index)
        End If
    End Sub

    Private Sub PanelSectors_MouseClick(sender As Object, e As MouseEventArgs) Handles PanelSectors.MouseClick
        If e.Button And MouseButtons.Left Then
            If _RegionData.Sectors Is Nothing Then
                Exit Sub
            End If

            Dim MousePos As New Point(e.X, e.Y)

            Dim SectorIndex = GetSectorIndex(MousePos)

            If SectorIndex > -1 And SectorIndex < _RegionData.Sectors.Count Then
                SelectTopSector(SectorIndex)
            End If
        End If
    End Sub
    Private Sub PanelSectors_Paint(sender As Object, e As PaintEventArgs) Handles PanelSectors.Paint
        Dim SectorFont = New Font("Microsoft Sans Serif", 7)

        Dim LeftPos As Integer
        Dim TopPos As Integer
        Dim Value As String
        Dim TextSize As SizeF
        Dim SelectedPen = New Pen(Color.Blue, 2)
        Dim SectorBrush As Brush

        e.Graphics.Clear(SystemColors.Control)

        LeftPos = PanelSectors.Padding.Left
        TopPos = PanelSectors.Padding.Top

        If _RegionData.Sectors IsNot Nothing Then
            For Each Sector In _RegionData.Sectors
                If Not Sector.HasData Then
                    SectorBrush = Brushes.LightGray
                ElseIf Not Sector.DataChecksumValid Or Not Sector.IDAMChecksumValid Then
                    SectorBrush = Brushes.LightPink
                ElseIf IsStandardSector(Sector) Then
                    SectorBrush = Brushes.LightGreen
                Else
                    SectorBrush = Brushes.LightBlue
                End If

                e.Graphics.FillRectangle(SectorBrush, LeftPos, TopPos, SECTOR_WIDTH, SECTOR_HEIGHT)
                If _TopSector Is Sector Then
                    e.Graphics.DrawRectangle(SelectedPen, LeftPos - 1, TopPos - 1, SECTOR_WIDTH + 2, SECTOR_HEIGHT + 2)
                Else
                    e.Graphics.DrawRectangle(SystemPens.WindowFrame, LeftPos, TopPos, SECTOR_WIDTH, SECTOR_HEIGHT)
                End If
                Value = Sector.SectorId
                TextSize = e.Graphics.MeasureString(Value, SectorFont)
                e.Graphics.DrawString(Value, SectorFont, SystemBrushes.WindowText, LeftPos + (SECTOR_WIDTH - TextSize.Width) / 2, TopPos + (SECTOR_HEIGHT - TextSize.Height) / 2)

                LeftPos += SECTOR_WIDTH + PADDING_COLS
                If LeftPos + SECTOR_WIDTH > PanelSectors.Width - PanelSectors.Padding.Right Then
                    LeftPos = PanelSectors.Padding.Left
                    TopPos = TopPos + SECTOR_HEIGHT + PADDING_ROWS
                End If
            Next
        End If

        SelectedPen.Dispose()
    End Sub

    Private Sub ToolStripMain_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ToolStripMain.ItemClicked
        HexBox1.Focus()
    End Sub

    Private Sub PanelSectors_MouseMove(sender As Object, e As MouseEventArgs) Handles PanelSectors.MouseMove
        If _RegionData.Sectors Is Nothing Then
            Exit Sub
        End If

        Dim TooltipText As String = ""
        Dim MousePos As New Point(e.X, e.Y)

        Dim SectorIndex = GetSectorIndex(MousePos)

        If SectorIndex > -1 And SectorIndex < _RegionData.Sectors.Count Then
            Dim Sector = _RegionData.Sectors(SectorIndex)
            TooltipText = "Sector Id: " & vbTab & vbTab & Sector.SectorId
            TooltipText &= vbCrLf & "Size: " & vbTab & vbTab & vbTab & Sector.DataLength

            If Sector.Track <> _Track Then
                TooltipText &= vbCrLf & "Track: " & vbTab & vbTab & vbTab & Sector.Track
            End If

            If Sector.Side <> _Side Then
                TooltipText &= vbCrLf & "Side: " & vbTab & vbTab & vbTab & Sector.Side
            End If

            TooltipText &= vbCrLf & "Address Checksum: " & vbTab & If(Sector.IDAMChecksumValid, "Valid", "Invalid")

            If Sector.HasData Then
                TooltipText &= vbCrLf & "Data Checksum: " & vbTab & vbTab & If(Sector.DataChecksumValid, "Valid", "Invalid")
            End If

            Dim DAMText As String
            If Not Sector.HasData Then
                DAMText = "Missing"
            ElseIf Sector.DAM = MFMAddressMark.Data Then
                DAMText = "Normal"
            ElseIf Sector.DAM = MFMAddressMark.DeletedData Then
                DAMText = "Deleted"
            Else
                DAMText = "Unknown"
            End If
            TooltipText &= vbCrLf & "Data Address Mark: " & vbTab & DAMText

            If Sector.Overlaps Then
                TooltipText &= vbCrLf & "Overlaps"
            End If
        End If

        If TooltipText <> _ToolTip.GetToolTip(PanelSectors) Then
            _ToolTip.SetToolTip(PanelSectors, TooltipText)
        End If
    End Sub
#End Region

    Private Class HighlightRange
        Public Sub New(StartIndex As UInteger)
            _StartIndex = StartIndex
            _EndIndex = StartIndex
        End Sub

        Public Property EndIndex As UInteger
        Public Property StartIndex As UInteger
    End Class

    Private Class TrackData
        Public Property Offset As Integer
        Public Property Side As Byte
        Public Property Track As UShort
        Public Overrides Function ToString() As String
            Return Track & "." & Side
        End Function
    End Class
End Class