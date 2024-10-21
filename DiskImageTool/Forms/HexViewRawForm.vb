Imports System.Runtime.InteropServices
Imports DiskImageTool.HexView
Imports Hb.Windows.Forms
Imports DiskImageTool.Bitstream.IBM_MFM
Imports DiskImageTool.DiskImage

Public Class HexViewRawForm
    Private WithEvents CheckBoxAllTracks As ToolStripCheckBox
    Private WithEvents ComboTrack As ToolStripComboBox
    Private WithEvents ComboSector As ToolStripComboBox
    Private WithEvents NumericBitOffset As ToolStripNumericUpDown
    Private _LabelBitOffset As ToolStripLabel
    Private _LabelSector As ToolStripLabel
    Private _CachedSelectedLength As Long = -1
    Private _CurrentRegionSector As BitstreamRegionSector = Nothing
    Private _CurrentSelectionLength As Long = -1
    Private _CurrentSelectionStart As Long = -1
    Private _ByteArray As IByteArray
    Private _Track As UShort
    Private _Side As Byte
    Private _AllTracks As Boolean
    Private _Data() As Byte
    Private _DataGridInspector As HexViewDataGridInspector
    Private _IgnoreEvent As Boolean = False
    Private _Initialized As Boolean = False
    Private _LastSearch As HexSearch
    Private _RegionData As RegionData
    Private _RegionMap() As BitstreamRegion
    Private _StoredCellValue As String

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function AddClipboardFormatListener(hwnd As IntPtr) As Boolean
    End Function

    Public Sub New(ByteArray As IByteArray, Track As UShort, Side As Byte, AllTracks As Boolean)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _ByteArray = ByteArray
        _Track = Track
        _Side = Side
        _AllTracks = AllTracks
        _Data = Nothing
        _RegionData = Nothing

        HexBox1.ReadOnly = True
        HexBox1.LineInfoOffset = 0

        Me.Text = "Raw Track Data"
    End Sub

    Private Sub HexViewRawForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        AddClipboardFormatListener(Me.Handle)

        _DataGridInspector = New HexViewDataGridInspector(DataGridDataInspector)
        _DataGridInspector.SetDataRow(DataRowEnum.File, Nothing, True, True)
        _DataGridInspector.SetDataRow(DataRowEnum.Description, Nothing, True, True)

        HexBox1.VScrollBarVisible = True

        Dim ShowAllTracks As Boolean = True
        If _ByteArray.AdditionalTracks.Count = 0 And _ByteArray.NonStandardTracks.Count = 0 Then
            _AllTracks = True
            ShowAllTracks = False
        End If

        InitializeTrackNavigator()
        If ShowAllTracks Then
            InitializeAllTracksCheckBox()
        End If
        InitializeSectorNavigator()
        InitializeBitOffsetNavigator()
        PopulateTracks(_AllTracks)
    End Sub

    Private Sub ChangeOffset(Offset As UInteger)
        Dim TrackData As TrackData = ComboTrack.SelectedItem
        If TrackData IsNot Nothing AndAlso TrackData.Offset <> Offset Then
            TrackData.Offset = Offset

            Dim StartByte = HexBox1.StartByte
            Dim StartLine = StartByte \ HexBox1.BytesPerLine
            Dim SelectionStart = HexBox1.SelectionStart
            Dim SelectionLength = HexBox1.SelectionLength
            LoadTrack(TrackData, True)
            HexBox1.PerformScrollToLine(StartLine)
            HexBox1.Select(SelectionStart, SelectionLength)
        End If
    End Sub

    Private Sub CopyHexToClipboard()
        HexBox1.CopyHex()
    End Sub

    Private Sub CopyTextToClipboard()
        HexBox1.Copy()
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
            Case Else
                Return Color.Black
        End Select
    End Function

    Private Function GetRegionDescription(RegionType As MFMRegionType) As String
        Select Case RegionType
            Case MFMRegionType.Gap1
                Return "GAP 1"
            Case MFMRegionType.Gap2
                Return "GAP 2"
            Case MFMRegionType.Gap3
                Return "GAP 3"
            Case MFMRegionType.Gap4A
                Return "GAP 4A"
            Case MFMRegionType.Gap4B
                Return "GAP 3+4B"
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
                Return "Data Area"
            Case MFMRegionType.DataChecksumValid, MFMRegionType.DataChecksumInvalid
                Return "Data Checksum"
            Case Else
                Return ""
        End Select
    End Function

    Private Sub HighlightRegions()
        For Each BitstreamRegion In _RegionData.Regions
            If BitstreamRegion.Length > 0 Then
                HexBox1.HighlightForeColor = GetRegionColor(BitstreamRegion.RegionType)
                HexBox1.HighlightBackColor = Color.White
                HexBox1.Highlight(BitstreamRegion.StartIndex, BitstreamRegion.Length)
            End If
        Next
    End Sub

    Private Sub InitRegionMap()
        _RegionMap = New BitstreamRegion(HexBox1.ByteProvider.Length - 1) {}

        For Each BitstreamRegion In _RegionData.Regions
            For Counter = 0 To BitstreamRegion.Length - 1
                _RegionMap(BitstreamRegion.StartIndex + Counter) = BitstreamRegion
            Next
        Next

        ComboSector.ComboBox.Items.Clear()

        For Each Sector In _RegionData.Sectors
            ComboSector.ComboBox.Items.Add(Sector)
        Next

        If _RegionData.Sectors.Count = 0 Then
            ComboSector.Visible = False
            _LabelSector.Visible = False
            NumericBitOffset.Visible = False
            _LabelBitOffset.Visible = False
            ToolStripBtnAdjustOffset.Visible = False
        Else
            ComboSector.Visible = True
            _LabelSector.Visible = True
            NumericBitOffset.Visible = True
            _LabelBitOffset.Visible = True
            ToolStripBtnAdjustOffset.Visible = True
        End If
    End Sub

    Private Sub InitializeBitOffsetNavigator()
        NumericBitOffset = New ToolStripNumericUpDown() With {
            .Alignment = ToolStripItemAlignment.Right,
            .AutoSize = False,
            .Size = New Drawing.Size(55, 23),
            .Minimum = 0,
            .Maximum = 15
        }

        _LabelBitOffset = New ToolStripLabel("Bit Offset") With {
            .Alignment = ToolStripItemAlignment.Right,
            .Padding = New Padding(12, 0, 0, 0)
        }

        Dim Button = ToolStripBtnAdjustOffset
        ToolStripMain.Items.Remove(Button)

        ToolStripMain.Items.Add(Button)
        ToolStripMain.Items.Add(NumericBitOffset)
        ToolStripMain.Items.Add(_LabelBitOffset)
    End Sub

    Private Sub InitializeSectorNavigator()
        ComboSector = New ToolStripComboBox() With {
            .Alignment = ToolStripItemAlignment.Right,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .AutoSize = False,
            .FlatStyle = FlatStyle.Standard,
            .Size = New Drawing.Size(50, 23)
        }
        '.Size = New Drawing.Size(65, 23)

        _LabelSector = New ToolStripLabel("Sector") With {
            .Alignment = ToolStripItemAlignment.Right,
            .Padding = New Padding(12, 0, 0, 0)
        }

        ToolStripMain.Items.Add(ComboSector)
        ToolStripMain.Items.Add(_LabelSector)
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

    Private Sub LoadData(Data() As Byte, RegionData As RegionData)
        _Initialized = False
        _Data = Data
        _RegionData = RegionData
        HexBox1.ByteProvider = New DynamicByteProvider(Data)

        ToolStripStatusBytes.Text = Format(Data.Length, "N0") & " bytes"
        _LastSearch = New HexSearch

        InitRegionMap()
        HighlightRegions()
        RefreshSelection(True)
        DataInspectorRefresh(True)
        Me.Refresh()
    End Sub

    Private Sub LoadTrack(TrackData As TrackData, ForceReload As Boolean)

        If _Initialized And TrackData.Track = _Track And TrackData.Side = _Side And Not ForceReload Then
            Exit Sub
        End If

        Me.Text = "Raw Track Data - Track " & TrackData.Track & "." & TrackData.Side

        _Track = TrackData.Track
        _Side = TrackData.Side

        Dim MFMTrack = _ByteArray.BitstreamImage.GetTrack(TrackData.Track * _ByteArray.BitstreamImage.TrackStep, TrackData.Side)
        Dim Data() As Byte
        Dim RegionData As RegionData
        If MFMTrack.Decoded Then
            If TrackData.Offset = -1 Then
                TrackData.Offset = BitstreamGetOffset(MFMTrack.Bitstream)
            End If
            'Dim AlignedBitstream = BitstreamAlign(MFMTrack.Bitstream, TrackData.Offset)
            Data = DecodeTrack(MFMTrack.Bitstream, TrackData.Offset)
            RegionData = BitstreamGetRegionList(MFMTrack.Bitstream)
            _IgnoreEvent = True
            NumericBitOffset.Value = TrackData.Offset
            _IgnoreEvent = False
        Else
            Data = FMGetBytes(MFMTrack.Bitstream)
            RegionData.Regions = New List(Of BitstreamRegion)
            RegionData.Sectors = New List(Of BitstreamRegionSector)
        End If

        RegionData.Track = TrackData.Track
        RegionData.Side = TrackData.Side

        LoadData(Data, RegionData)
    End Sub

    Private Sub ProcessKeyPress(e As KeyEventArgs)
        If e.Control And e.KeyCode = Keys.C Then
            If HexBox1.CanCopy Then
                If e.Shift Then
                    CopyHexFormatted(HexBox1)
                Else
                    CopyHexToClipboard()
                End If
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub PopulateTracks(AllTracks As Boolean)
        Dim SelectedIndex As Integer = -1
        ComboTrack.Items.Clear()
        For i = 0 To _ByteArray.TrackCount - 1
            For j = 0 To _ByteArray.HeadCount - 1
                Dim TrackIndex = i * _ByteArray.HeadCount + j
                If AllTracks OrElse (_ByteArray.NonStandardTracks.Contains(TrackIndex) Or _ByteArray.AdditionalTracks.Contains(TrackIndex)) Then
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
            Dim Description = GetRegionDescription(RegionStart.RegionType)
            If RegionStart Is RegionEnd AndAlso Description.Length > 0 Then
                _DataGridInspector.SetDataRow(DataRowEnum.Description, Description, True, True)
                BtnSelectRegion.Enabled = True
            Else
                _DataGridInspector.SetDataRow(DataRowEnum.Description, Nothing, True, True)
                BtnSelectRegion.Enabled = False
            End If

            _CurrentRegionSector = RegionStart.Sector

            Dim TrackData As TrackData = ComboTrack.SelectedItem

            If TrackData Is Nothing Then
                BtnAdjustOffset.Enabled = False
            Else
                BtnAdjustOffset.Enabled = RegionStart.BitOffset <> TrackData.Offset
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

        _IgnoreEvent = False

        _Initialized = True
    End Sub

    Private Sub RefreshSelectorValues()
        Dim Offset = HexBox1.LineInfoOffset + HexBox1.StartByte + HexBox1.BytesPerLine
        Dim RegionData = _RegionMap(Offset)

        If RegionData Is Nothing Then
            Exit Sub
        End If

        _IgnoreEvent = True

        ComboSector.SelectedItem = RegionData.Sector

        _IgnoreEvent = False
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

    Private Sub BtnCopyHex_Click(sender As Object, e As EventArgs) Handles BtnCopyHex.Click, ToolStripBtnCopyHex.Click
        If HexBox1.CanCopy Then
            CopyHexToClipboard()
        End If
    End Sub

    Private Sub BtnCopyHexFormatted_Click(sender As Object, e As EventArgs) Handles BtnCopyHexFormatted.Click, ToolStripBtnCopyHexFormatted.Click
        If HexBox1.CanCopy Then
            CopyHexFormatted(HexBox1)
        End If
    End Sub

    Private Sub BtnCopyText_Click(sender As Object, e As EventArgs) Handles BtnCopyText.Click, ToolStripBtnCopyText.Click
        If HexBox1.CanCopy Then
            CopyTextToClipboard()
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

    Private Sub ComboSector_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboSector.SelectedIndexChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        JumpToSector(ComboSector.SelectedItem)
        RefreshSelection(False)
        DataInspectorRefresh(False)
    End Sub

    Private Sub ComboTrack_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboTrack.SelectedIndexChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        LoadTrack(ComboTrack.SelectedItem, False)
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
                HexBox1.SetCaretPosition(New Point(e.X, e.Y))
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

    Private Sub ToolStripMain_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ToolStripMain.ItemClicked
        HexBox1.Focus()
    End Sub

    Private Class TrackData
        Public Property Track As UShort
        Public Property Side As Byte
        Public Property Offset As Integer
        Public Overrides Function ToString() As String
            Return Track & "." & Side
        End Function
    End Class
End Class