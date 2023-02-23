Imports System.ComponentModel
Imports DiskImageTool.DiskImage
Imports Hb.Windows.Forms

Public Class HexViewForm
    Public Shared ReadOnly ALT_BACK_COLOR As Color = Color.FromArgb(246, 246, 252)
    Private ReadOnly _Changes As Stack(Of HexChange)
    Private ReadOnly _HexViewSectorData As HexViewSectorData
    Private ReadOnly _RedoChanges As Stack(Of HexChange)
    Private ReadOnly _SectorNavigator As Boolean
    Private ReadOnly _ClusterNavigator As Boolean
    Private _CurrentHexViewData As HexViewData
    Private _CurrentIndex As Integer = -1
    Private _CurrentSector As UInteger = 0
    Private _Initialized As Boolean = False
    Private _Modified As Boolean = False
    Private _RegionDescriptions As Dictionary(Of UInteger, HexViewHighlightRegion)
    Private _IgnoreEvent As Boolean = False
    Private WithEvents NumericSector As ToolStripNumericUpDown
    Private WithEvents NumericCluster As ToolStripNumericUpDown
    Private WithEvents ComboTrack As ToolStripComboBox

    Public Sub New(HexViewSectorData As HexViewSectorData, Caption As String, SectorNavigator As Boolean, ClusterNavigator As Boolean)
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        _HexViewSectorData = HexViewSectorData
        _Changes = New Stack(Of HexChange)
        _RedoChanges = New Stack(Of HexChange)
        _CurrentHexViewData = Nothing
        _SectorNavigator = SectorNavigator
        _ClusterNavigator = ClusterNavigator

        HexBox1.ReadOnly = False

        If Caption = "" Then
            Me.Text = "Hex Editor"
        Else
            Me.Text = Caption
        End If

        LblGroups.Visible = Not SectorNavigator And Not ClusterNavigator
        CmbGroups.Visible = Not SectorNavigator And Not ClusterNavigator

        If ClusterNavigator Then
            InitializeTrackNavigator()
        End If

        If SectorNavigator Then
            InitializeSectorNavigator()
        End If

        If ClusterNavigator Then
            InitializeClusterNavigator()
        End If
    End Sub

    Private Sub InitializeSectorNavigator()
        NumericSector = New ToolStripNumericUpDown() With {
            .Alignment = ToolStripItemAlignment.Right
        }

        Dim LabelSector = New ToolStripLabel("Sector") With {
            .Alignment = ToolStripItemAlignment.Right,
            .Padding = New Padding(12, 0, 0, 0)
        }

        ToolStripMain.Items.Add(NumericSector)
        ToolStripMain.Items.Add(LabelSector)
    End Sub

    Private Sub InitializeClusterNavigator()
        NumericCluster = New ToolStripNumericUpDown() With {
            .Alignment = ToolStripItemAlignment.Right
        }

        Dim LabelCluster = New ToolStripLabel("Cluster") With {
            .Alignment = ToolStripItemAlignment.Right,
            .Padding = New Padding(12, 0, 0, 0)
        }

        ToolStripMain.Items.Add(NumericCluster)
        ToolStripMain.Items.Add(LabelCluster)
    End Sub

    Private Sub InitializeTrackNavigator()
        ComboTrack = New ToolStripComboBox() With {
            .Alignment = ToolStripItemAlignment.Right,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .AutoSize = False,
            .FlatStyle = FlatStyle.Popup,
            .Size = New Drawing.Size(50, 23)
        }

        Dim LabelTrack = New ToolStripLabel("Track") With {
            .Alignment = ToolStripItemAlignment.Right,
            .Padding = New Padding(12, 0, 0, 0)
        }

        ToolStripMain.Items.Add(ComboTrack)
        ToolStripMain.Items.Add(LabelTrack)
    End Sub

    Public ReadOnly Property Modified As Boolean
        Get
            Return _Modified
        End Get
    End Property

    Private Function ClipboardHasHex() As Boolean
        Dim DataObject = Clipboard.GetDataObject()

        If DataObject.GetDataPresent(GetType(String)) Then
            Dim Hex = CStr(DataObject.GetData(GetType(String)))
            Return ConvertHexToBytes(Hex) IsNot Nothing
        End If

        Return False
    End Function

    Private Sub CommitChanges()
        _HexViewSectorData.SectorData.CommitChanges()
        _Modified = True

        Me.Close()
    End Sub

    Private Sub CopyHexFormatted()
        Dim Capacity As Integer = HexBox1.SelectionLength * 2 + HexBox1.SelectionLength + HexBox1.SelectionLength \ 16
        Dim SB = New System.Text.StringBuilder(Capacity)
        For Counter = 0 To HexBox1.SelectionLength - 1
            Dim B = HexBox1.ByteProvider.ReadByte(HexBox1.SelectionStart + Counter)
            SB.Append(B.ToString("X2"))
            If (Counter + 1) Mod 16 = 0 Then
                SB.Append(vbNewLine)
            Else
                SB.Append(" ")
            End If
        Next
        Clipboard.SetText(SB.ToString)
        RefreshPasteButton()
    End Sub

    Private Sub CopyHexToClipboard()
        HexBox1.CopyHex()
        RefreshPasteButton()
    End Sub

    Private Sub CopyTextToClipboard()
        HexBox1.Copy()
        RefreshPasteButton()
    End Sub

    Private Sub DisplayBlock(SelectedIndex As Integer)
        If SelectedIndex = _CurrentIndex Then
            Exit Sub
        End If

        _CurrentIndex = SelectedIndex
        _CurrentHexViewData = CType(CmbGroups.SelectedItem, HexViewData)
        Dim HighlightedRegions As HighlightedRegions
        If _CurrentHexViewData.SectorBlock.Index < _HexViewSectorData.HighlightedRegionList.Count Then
            HighlightedRegions = _HexViewSectorData.HighlightedRegionList(_CurrentHexViewData.SectorBlock.Index)
        Else
            HighlightedRegions = New HighlightedRegions
        End If

        With HexBox1
            .ByteProvider = Nothing

            If _CurrentHexViewData.SectorBlock.Size > 0 Then
                .ByteProvider = New MyByteProvider(_HexViewSectorData.SectorData.Data, _CurrentHexViewData.SectorBlock.Offset, _CurrentHexViewData.SectorBlock.Size)
                .LineInfoOffset = SectorToBytes(_CurrentHexViewData.SectorBlock.SectorStart)

                HighlightedRegions.Sort()

                Dim TotalLength As UInteger = .ByteProvider.Length
                Dim Start As UInteger = 0
                Dim Length = BYTES_PER_SECTOR - (Start Mod BYTES_PER_SECTOR)
                Dim Size As UInteger = Math.Min(Length, TotalLength - Start)
                Dim CurrentRegion As HexViewHighlightRegion
                Dim Index As Integer = 0
                If Index < HighlightedRegions.Count Then
                    CurrentRegion = HighlightedRegions(Index)
                Else
                    CurrentRegion = Nothing
                End If
                Do While Size > 0
                    Dim HighlightForeColor As Color = Color.Black
                    Dim HighlightBackColor As Color = Color.White

                    If CurrentRegion IsNot Nothing Then
                        If CurrentRegion.Start = Start Then
                            Size = CurrentRegion.Size
                            HighlightForeColor = CurrentRegion.ForeColor
                            HighlightBackColor = CurrentRegion.BackColor
                        ElseIf CurrentRegion.Start > Start And CurrentRegion.Start < Start + Size Then
                            Size = CurrentRegion.Start - Start
                        End If
                        If Start + Size >= CurrentRegion.Start + CurrentRegion.Size Then
                            Index += 1
                            If Index < HighlightedRegions.Count Then
                                CurrentRegion = HighlightedRegions(Index)
                            Else
                                CurrentRegion = Nothing
                            End If
                        End If
                    End If

                    If Size > 0 Then
                        If HighlightBackColor = Color.White AndAlso OffsetToSector(Start) Mod 2 = 1 Then
                            HighlightBackColor = ALT_BACK_COLOR
                        End If
                        If HighlightForeColor <> Color.Black Or HighlightBackColor <> Color.White Then
                            .HighlightForeColor = HighlightForeColor
                            .HighlightBackColor = HighlightBackColor
                            .Highlight(Start, Size)
                        End If
                    End If

                    Start += Size
                    Length = BYTES_PER_SECTOR - (Start Mod BYTES_PER_SECTOR)
                    Size = Math.Min(Length, TotalLength - Start)
                Loop
            End If
        End With

        InitRegionDescriptions(HighlightedRegions)
        RefreshSelection(True)
        RefresSelectors()

        ToolStripStatusBytes.Text = Format(_CurrentHexViewData.SectorBlock.Size, "N0") & " bytes"
    End Sub

    Private Sub FillRegion(Offset As Integer, Length As Integer, Value As Byte)
        Dim b(Length - 1) As Byte
        Dim Modified As Boolean = False

        For Index = 0 To Length - 1
            b(Index) = HexBox1.ByteProvider.ReadByte(Offset + Index)
            If b(Index) <> Value Then
                HexBox1.ByteProvider.WriteByte(Offset + Index, Value)
                Modified = True
            End If
        Next

        If Modified Then
            PushChange(_CurrentIndex, Offset, b, HexBox1.SelectionStart, HexBox1.SelectionLength)
            HexBox1.Invalidate()
        End If
    End Sub

    Private Sub FillSelected(Value As Byte)
        FillRegion(HexBox1.SelectionStart, HexBox1.SelectionLength, Value)
    End Sub

    Private Function GetCRC32Selected() As String
        Dim OffsetStart As UInteger = _CurrentHexViewData.SectorBlock.Offset + HexBox1.SelectionStart
        Dim Length As UInteger = HexBox1.SelectionLength

        Dim B = _HexViewSectorData.SectorData.Data.GetBytes(OffsetStart, Length)
        Return Crc32.ComputeChecksum(B).ToString("X8")
    End Function

    Private Function GetVisibleOffset() As UInteger
        Dim Offset As UInteger

        If HexBox1.SelectionStart < HexBox1.StartByte Or HexBox1.SelectionStart > HexBox1.EndByte Then
            Offset = HexBox1.StartByte
        Else
            Offset = HexBox1.SelectionStart
        End If

        Offset = HexBox1.LineInfoOffset + Offset

        Return Offset
    End Function

    Private Sub InitRegionDescriptions(HighlightedRegions As HighlightedRegions)
        _RegionDescriptions = New Dictionary(Of UInteger, HexViewHighlightRegion)

        For Each HighlightedRegion In HighlightedRegions
            If HighlightedRegion.Description.Length > 0 Then
                For Counter = 0 To HighlightedRegion.Size - 1
                    _RegionDescriptions.Item(HighlightedRegion.Start + Counter) = HighlightedRegion
                Next
            End If
        Next
    End Sub

    Private Sub JumpToCluster(Cluster As UShort)
        Dim SectorStart = _CurrentHexViewData.SectorBlock.SectorStart
        Dim SectorEnd = SectorStart + _CurrentHexViewData.SectorBlock.SectorCount - 1
        Dim ClusterStart = _HexViewSectorData.Disk.BootSector.SectorToCluster(SectorStart)
        Dim ClusterEnd = _HexViewSectorData.Disk.BootSector.SectorToCluster(SectorEnd)
        Dim Sector = _HexViewSectorData.Disk.BootSector.ClusterToSector(Cluster)

        If Cluster > 1 And Cluster >= ClusterStart And Cluster <= clusterend Then
            Dim Offset = SectorToBytes(Sector) - HexBox1.LineInfoOffset
            Dim Line = Offset \ HexBox1.BytesPerLine
            HexBox1.PerformScrollToLine(Line)
        End If
    End Sub

    Private Sub JumpToSector(Sector As UInteger)
        Dim SectorStart = _CurrentHexViewData.SectorBlock.SectorStart
        Dim SectorEnd = SectorStart + _CurrentHexViewData.SectorBlock.SectorCount - 1

        If Sector >= SectorStart And Sector <= SectorEnd Then
            Dim Offset = SectorToBytes(Sector) - HexBox1.LineInfoOffset
            Dim Line = Offset \ HexBox1.BytesPerLine
            HexBox1.PerformScrollToLine(Line)
        End If
    End Sub

    Private Sub JumpToTrack(TrackValue As String)
        Dim TrackSet = TrackValue.Split(".")
        Dim Track As UShort = TrackSet(0)
        Dim Side As UShort = TrackSet(1)

        Dim SectorStart = _CurrentHexViewData.SectorBlock.SectorStart
        Dim SectorEnd = SectorStart + _CurrentHexViewData.SectorBlock.SectorCount - 1
        Dim TrackStart = _HexViewSectorData.Disk.BootSector.SectorToTrack(SectorStart)
        Dim TrackEnd = _HexViewSectorData.Disk.BootSector.SectorToTrack(SectorEnd)
        Dim Sector = _HexViewSectorData.Disk.BootSector.TrackToSector(Track, Side)

        If Track >= TrackStart And Track <= TrackEnd Then
            Dim Offset = SectorToBytes(Sector) - HexBox1.LineInfoOffset
            Dim Line = Offset \ HexBox1.BytesPerLine
            HexBox1.PerformScrollToLine(Line)
        End If
    End Sub

    Private Sub PasteHex()
        Dim HexBytes = ConvertHexToBytes(Clipboard.GetText)
        Dim Offset = HexBox1.SelectionStart
        Dim Length = HexBytes.Length
        Dim Modified As Boolean = False

        If Offset + Length > HexBox1.ByteProvider.Length Then
            Length = HexBox1.ByteProvider.Length - Offset
        End If

        If Length > 0 Then
            Dim b(Length - 1) As Byte
            For Index = 0 To Length - 1
                b(Index) = HexBox1.ByteProvider.ReadByte(Offset + Index)
                If b(Index) <> HexBytes(Index) Then
                    HexBox1.ByteProvider.WriteByte(Offset + Index, HexBytes(Index))
                    Modified = True
                End If
            Next

            If Modified Then
                PushChange(_CurrentIndex, Offset, b, HexBox1.SelectionStart, HexBox1.SelectionLength)
                HexBox1.SelectionLength = Length
                HexBox1.Invalidate()
            End If
        End If
    End Sub

    Private Sub PopChange(Source As Stack(Of HexChange), Destination As Stack(Of HexChange))
        If Source.Count > 0 Then
            Dim BlockOffset As UInteger
            Dim HexChange = Source.Pop()

            If HexChange.BlockIndex <> _CurrentIndex Then
                CmbGroups.SelectedIndex = HexChange.BlockIndex
            End If

            Dim b(HexChange.Data.Length - 1) As Byte
            BlockOffset = _CurrentHexViewData.SectorBlock.Offset + HexChange.Index
            _HexViewSectorData.SectorData.Data.CopyTo(BlockOffset, b, 0, HexChange.Data.Length)
            Destination.Push(New HexChange(HexChange.BlockIndex, HexChange.Index, b, HexChange.SelectionStart, HexChange.SelectionLength))

            For Counter = 0 To HexChange.Data.Length - 1
                HexBox1.ByteProvider.WriteByte(HexChange.Index + Counter, HexChange.Data(Counter))
            Next

            HexBox1.Select(HexChange.SelectionStart, HexChange.SelectionLength)

            RefreshUndoButtons()
        End If
    End Sub

    Private Sub ProcessKeyPress(e As KeyEventArgs)
        If e.Control And e.KeyCode = Keys.C Then
            If HexBox1.CanCopy Then
                If e.Shift Then
                    CopyHexFormatted()
                Else
                    CopyHexToClipboard()
                End If
            End If
            e.SuppressKeyPress = True
        ElseIf e.Control And e.KeyCode = Keys.V Then
            If ClipboardHasHex() Then
                PasteHex()
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Back Then
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Delete Then
            If HexBox1.SelectionLength > 0 Then
                FillSelected(0)
            End If
            e.SuppressKeyPress = True
        ElseIf e.KeyCode = Keys.Down Then
            Dim LineCount = Math.Ceiling(HexBox1.ByteProvider.Length / HexBox1.HorizontalByteCount)
            If HexBox1.CurrentLine >= LineCount Then
                If CmbGroups.SelectedIndex < CmbGroups.Items.Count - 1 Then
                    Dim Offset = HexBox1.SelectionStart Mod HexBox1.HorizontalByteCount
                    CmbGroups.SelectedIndex = CmbGroups.SelectedIndex + 1
                    HexBox1.Select(Offset, 0)
                    e.SuppressKeyPress = True
                End If
            End If
        ElseIf e.KeyCode = Keys.Up Then
            If HexBox1.CurrentLine <= 1 Then
                If CmbGroups.SelectedIndex > 0 Then
                    Dim Offset = HexBox1.SelectionStart Mod HexBox1.HorizontalByteCount
                    CmbGroups.SelectedIndex = CmbGroups.SelectedIndex - 1
                    HexBox1.Select(HexBox1.ByteProvider.Length - (HexBox1.HorizontalByteCount - Offset), 0)
                    e.SuppressKeyPress = True
                End If
            End If
        ElseIf e.KeyCode = Keys.End Then
            If CmbGroups.SelectedIndex < CmbGroups.Items.Count - 1 Then
                CmbGroups.SelectedIndex = CmbGroups.Items.Count - 1
                HexBox1.Select(HexBox1.ByteProvider.Length - HexBox1.HorizontalByteCount, 0)
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.Home Then
            If CmbGroups.SelectedIndex > 0 Then
                CmbGroups.SelectedIndex = 0
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = Keys.PageUp Then
            If HexBox1.StartByte = 0 Then
                If CmbGroups.SelectedIndex > 0 Then
                    Dim Offset = HexBox1.SelectionStart Mod HexBox1.HorizontalByteCount
                    CmbGroups.SelectedIndex = CmbGroups.SelectedIndex - 1
                    HexBox1.Select(HexBox1.ByteProvider.Length - (HexBox1.HorizontalByteCount - Offset), 0)
                    e.SuppressKeyPress = True
                End If
            End If
        ElseIf e.KeyCode = Keys.PageDown Then
            If HexBox1.EndByte = HexBox1.ByteProvider.Length - 1 Then
                If CmbGroups.SelectedIndex < CmbGroups.Items.Count - 1 Then
                    Dim Offset = HexBox1.SelectionStart Mod HexBox1.HorizontalByteCount
                    CmbGroups.SelectedIndex = CmbGroups.SelectedIndex + 1
                    HexBox1.Select(Offset, 0)
                    e.SuppressKeyPress = True
                End If
            End If
        End If
    End Sub

    Private Sub ProcessMousewheel(Delta As Integer)
        If Delta < 0 Then
            If HexBox1.EndByte = HexBox1.ByteProvider.Length - 1 Then
                If CmbGroups.SelectedIndex < CmbGroups.Items.Count - 1 Then
                    Dim Offset = HexBox1.SelectionStart Mod HexBox1.HorizontalByteCount
                    CmbGroups.SelectedIndex = CmbGroups.SelectedIndex + 1
                    HexBox1.Select(Offset, 0)
                End If
            End If
        ElseIf Delta > 0 Then
            If HexBox1.StartByte = 0 Then
                If CmbGroups.SelectedIndex > 0 Then
                    Dim Offset = HexBox1.SelectionStart Mod HexBox1.HorizontalByteCount
                    CmbGroups.SelectedIndex = CmbGroups.SelectedIndex - 1
                    HexBox1.Select(HexBox1.ByteProvider.Length - (HexBox1.HorizontalByteCount - Offset), 0)
                End If
            End If
        End If
    End Sub

    Private Sub PushChange(BlockIndex As Integer, Index As Long, Data As Byte, SelectionStart As Long, SelectionLength As Long)
        _Changes.Push(New HexChange(BlockIndex, Index, Data, SelectionStart, SelectionLength))
        _RedoChanges.Clear()
        RefreshUndoButtons()
    End Sub

    Private Sub PushChange(BlockIndex As Integer, Index As Long, Data() As Byte, SelectionStart As Long, SelectionLength As Long)
        _Changes.Push(New HexChange(BlockIndex, Index, Data, SelectionStart, SelectionLength))
        _RedoChanges.Clear()
        RefreshUndoButtons()
    End Sub

    Private Sub RefreshPasteButton()
        BtnPaste.Enabled = ClipboardHasHex()
        ToolStripBtnPaste.Enabled = BtnPaste.Enabled
    End Sub

    Private Sub RefreshSelection(ForceUpdate As Boolean)
        If Not _Initialized And Not ForceUpdate Then
            Exit Sub
        End If

        Dim Disk = _CurrentHexViewData.Disk
        Dim SelectionStart = HexBox1.SelectionStart
        Dim SelectionLength = HexBox1.SelectionLength
        Dim SelectionEnd = SelectionStart + SelectionLength - 1

        If SelectionStart = -1 Then
            ToolStripStatusOffset.Visible = False
            ToolStripStatusBlock.Visible = False
            ToolStripStatusLength.Visible = False
            ToolStripStatusSector.Visible = False
            ToolStripStatusCluster.Visible = False
            ToolStripStatusTrack.Visible = False
            ToolStripStatusSide.Visible = False
            ToolStripStatusFile.Visible = False
            ToolStripStatusDescription.Visible = False
            BtnSelectSector.Text = "Select &Sector"
            BtnSelectSector.Enabled = False
            ToolStripBtnSelectSector.Text = "Sector"
            ToolStripBtnSelectSector.ToolTipText = "SelectSector"
            ToolStripBtnSelectSector.Enabled = BtnSelectSector.Enabled
        Else
            Dim Offset As UInteger = HexBox1.LineInfoOffset + SelectionStart
            Dim FileName As String = ""
            Dim OutOfRange As Boolean = SelectionStart >= HexBox1.ByteProvider.Length

            Dim Sector = OffsetToSector(Offset)

            ToolStripStatusOffset.Visible = Not OutOfRange
            ToolStripStatusOffset.Text = "Offset(h): " & SelectionStart.ToString("X")

            If SelectionLength = 0 Then
                ToolStripStatusBlock.Visible = False
                ToolStripStatusBlock.Text = ""
                ToolStripStatusLength.Visible = False
                ToolStripStatusLength.Text = ""
            Else
                ToolStripStatusBlock.Visible = True
                ToolStripStatusBlock.Text = "Block(h): " & SelectionStart.ToString("X") & "-" & SelectionEnd.ToString("X")
                ToolStripStatusLength.Visible = True
                ToolStripStatusLength.Text = "Length(h): " & SelectionLength.ToString("X")
            End If

            If _CurrentSector <> Sector Or ForceUpdate Then
                Dim Cluster As UShort
                If Disk.IsValidImage Then
                    Cluster = Disk.BootSector.SectorToCluster(Sector)
                Else
                    Cluster = 0
                End If

                ToolStripStatusSector.Visible = Not OutOfRange
                ToolStripStatusSector.Text = "Sector: " & Sector
                If Cluster < 2 Then
                    ToolStripStatusCluster.Visible = False
                    ToolStripStatusCluster.Text = ""
                Else
                    ToolStripStatusCluster.Visible = Not OutOfRange
                    ToolStripStatusCluster.Text = "Cluster: " & Cluster

                    If Disk.FAT.FileAllocation.ContainsKey(Cluster) Then
                        Dim OffsetList = Disk.FAT.FileAllocation.Item(Cluster)
                        Dim DirectoryEntry = Disk.GetDirectoryEntryByOffset(OffsetList.Item(0))
                        FileName = DirectoryEntry.GetFullFileName
                    End If
                End If

                If Disk.IsValidImage Then
                    ToolStripStatusTrack.Visible = Not OutOfRange
                    ToolStripStatusTrack.Text = "Track: " & Disk.BootSector.SectorToTrack(Sector)

                    ToolStripStatusSide.Visible = Not OutOfRange
                    ToolStripStatusSide.Text = "Side: " & Disk.BootSector.SectorToSide(Sector)
                Else
                    ToolStripStatusTrack.Visible = False
                    ToolStripStatusTrack.Text = ""

                    ToolStripStatusSide.Visible = False
                    ToolStripStatusSide.Text = ""
                End If

                If FileName.Length = 0 Then
                    ToolStripStatusFile.Visible = False
                    ToolStripStatusFile.Text = ""
                Else
                    ToolStripStatusFile.Visible = Not OutOfRange
                    ToolStripStatusFile.Text = "File: " & FileName
                End If

                BtnSelectSector.Text = "Select &Sector " & Sector
                BtnSelectSector.Enabled = Not OutOfRange
                ToolStripBtnSelectSector.Text = "Sector " & Sector
                ToolStripBtnSelectSector.ToolTipText = "Select Sector " & Sector
                ToolStripBtnSelectSector.Enabled = BtnSelectSector.Enabled

                _CurrentSector = Sector
            End If

            If ToolStripStatusFile.Visible Or _RegionDescriptions.Count = 0 Then
                ToolStripStatusDescription.Visible = False
            Else
                Dim RegionStart As HexViewHighlightRegion
                Dim RegionEnd As HexViewHighlightRegion

                If _RegionDescriptions.ContainsKey(SelectionStart) Then
                    RegionStart = _RegionDescriptions.Item(SelectionStart)
                Else
                    RegionStart = Nothing
                End If

                If SelectionLength = 0 Then
                    RegionEnd = RegionStart
                ElseIf _RegionDescriptions.ContainsKey(SelectionEnd) Then
                    RegionEnd = _RegionDescriptions.Item(SelectionEnd)
                Else
                    RegionEnd = Nothing
                End If

                If RegionStart IsNot Nothing AndAlso RegionStart Is RegionEnd Then
                    ToolStripStatusDescription.Visible = Not OutOfRange
                    ToolStripStatusDescription.Text = _RegionDescriptions.Item(SelectionStart).Description
                Else
                    ToolStripStatusDescription.Visible = False
                    ToolStripStatusDescription.Text = ""
                End If
            End If
        End If

        BtnCopyText.Enabled = HexBox1.CanCopy
        ToolStripBtnCopyText.Enabled = BtnCopyText.Enabled

        BtnCopyHex.Enabled = HexBox1.CanCopy
        ToolStripBtnCopyHex.Enabled = BtnCopyHex.Enabled

        BtnCopyHexFormatted.Enabled = HexBox1.CanCopy
        ToolStripBtnCopyHexFormatted.Enabled = BtnCopyHexFormatted.Enabled

        BtnCRC32.Visible = HexBox1.CanCopy
        ToolStripSeparatorCRC32.Visible = HexBox1.CanCopy

        BtnDelete.Enabled = HexBox1.SelectionLength > 0
        ToolStripBtnDelete.Enabled = BtnDelete.Enabled

        BtnFillF6.Enabled = HexBox1.SelectionLength > 0
        ToolStripBtnFillF6.Enabled = BtnFillF6.Enabled

        RefreshPasteButton()
        RefreshUndoButtons()

        _Initialized = True
    End Sub

    Private Sub RefreshUndoButtons()
        BtnUndo.Enabled = _Changes.Count > 0
        ToolStripBtnUndo.Enabled = BtnUndo.Enabled

        BtnRedo.Enabled = _RedoChanges.Count > 0
        ToolStripBtnRedo.Enabled = BtnRedo.Enabled

        ToolStripBtnCommit.Enabled = _Changes.Count > 0
    End Sub

    Private Sub RefresSelectors()
        Dim SectorStart As UInteger = 0
        Dim SectorEnd As UInteger = 0

        _IgnoreEvent = True

        If _CurrentHexViewData.SectorBlock.Size > 0 Then
            If _SectorNavigator Or _ClusterNavigator Then
                SectorStart = _CurrentHexViewData.SectorBlock.SectorStart
                SectorEnd = SectorStart + _CurrentHexViewData.SectorBlock.SectorCount - 1
            End If

            If _SectorNavigator Then
                NumericSector.Minimum = SectorStart
                NumericSector.Maximum = SectorEnd
                NumericSector.Enabled = True
            End If

            If _ClusterNavigator Then
                NumericCluster.Minimum = _HexViewSectorData.Disk.BootSector.SectorToCluster(SectorStart)
                NumericCluster.Maximum = _HexViewSectorData.Disk.BootSector.SectorToCluster(SectorEnd)
                NumericCluster.Enabled = True

                ComboTrack.Items.Clear()
                Dim TrackStart = _HexViewSectorData.Disk.BootSector.SectorToTrack(SectorStart)
                Dim TrackEnd = _HexViewSectorData.Disk.BootSector.SectorToTrack(SectorEnd)
                For i = TrackStart To TrackEnd
                    For j = 0 To _HexViewSectorData.Disk.BootSector.NumberOfHeads - 1
                        ComboTrack.Items.Add(i & "." & j)
                    Next
                Next
                ComboTrack.SelectedIndex = 0
                ComboTrack.Enabled = True
            End If
        End If

        _IgnoreEvent = False
    End Sub

    Private Sub RefresSelectorValues()
        If _CurrentHexViewData Is Nothing Then
            Exit Sub
        End If

        Dim Offset = HexBox1.LineInfoOffset + HexBox1.StartByte
        Dim Sector = OffsetToSector(Offset)

        _IgnoreEvent = True

        If _SectorNavigator Then
            If Sector <> NumericSector.Value Then
                NumericSector.Value = Sector
            End If
        End If

        If _ClusterNavigator Then
            Dim Cluster = _CurrentHexViewData.Disk.BootSector.SectorToCluster(Sector)

            If Cluster <> NumericCluster.Value Then
                NumericCluster.Value = Cluster
            End If

            Dim Track = _CurrentHexViewData.Disk.BootSector.SectorToTrack(Sector)
            Dim Side = _CurrentHexViewData.Disk.BootSector.SectorToSide(Sector)
            Dim Value = Track.ToString & "." & Side.ToString
            If Value <> ComboTrack.Text Then
                ComboTrack.Text = Value
            End If
        End If

        _IgnoreEvent = False
    End Sub
    Private Sub SelectCurrentSector()
        Dim OutOfRange As Boolean = HexBox1.SelectionStart >= HexBox1.ByteProvider.Length
        If Not OutOfRange Then
            Dim Offset = SectorToBytes(_CurrentSector) - HexBox1.LineInfoOffset
            Dim Length = BYTES_PER_SECTOR
            HexBox1.Select(Offset, Length)
        End If
    End Sub

#Region "Events"

    Private Sub BtnCopyHex_Click(sender As Object, e As EventArgs) Handles BtnCopyHex.Click, ToolStripBtnCopyHex.Click
        If HexBox1.CanCopy Then
            CopyHexToClipboard()
        End If
    End Sub

    Private Sub BtnCopyHexFormatted_Click(sender As Object, e As EventArgs) Handles BtnCopyHexFormatted.Click, ToolStripBtnCopyHexFormatted.Click
        If HexBox1.CanCopy Then
            CopyHexFormatted()
        End If
    End Sub

    Private Sub BtnCopyText_Click(sender As Object, e As EventArgs) Handles BtnCopyText.Click, ToolStripBtnCopyText.Click
        If HexBox1.CanCopy Then
            CopyTextToClipboard()
        End If
    End Sub

    Private Sub BtnCRC32_Click(sender As Object, e As EventArgs) Handles BtnCRC32.Click
        If BtnCRC32.Tag <> "" Then
            Clipboard.SetText(BtnCRC32.Tag)
        End If
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs) Handles BtnDelete.Click, ToolStripBtnDelete.Click
        If HexBox1.SelectionLength > 0 Then
            FillSelected(0)
        End If
    End Sub

    Private Sub BtnFillF6_Click(sender As Object, e As EventArgs) Handles BtnFillF6.Click, ToolStripBtnFillF6.Click
        If HexBox1.SelectionLength > 0 Then
            FillSelected(&HF6)
        End If
    End Sub

    Private Sub BtnPaste_Click(sender As Object, e As EventArgs) Handles BtnPaste.Click, ToolStripBtnPaste.Click
        If ClipboardHasHex() Then
            PasteHex()
        End If
    End Sub

    Private Sub BtnRedo_Click(sender As Object, e As EventArgs) Handles BtnRedo.Click, ToolStripBtnRedo.Click
        PopChange(_RedoChanges, _Changes)
    End Sub

    Private Sub BtnSelectAll_Click(sender As Object, e As EventArgs) Handles BtnSelectAll.Click, ToolStripBtnSelectAll.Click
        HexBox1.SelectAll()
    End Sub

    Private Sub BtnSelectSector_Click(sender As Object, e As EventArgs) Handles BtnSelectSector.Click, ToolStripBtnSelectSector.Click
        SelectCurrentSector()
    End Sub

    Private Sub BtnUndo_Click(sender As Object, e As EventArgs) Handles BtnUndo.Click, ToolStripBtnUndo.Click
        PopChange(_Changes, _RedoChanges)
    End Sub

    Private Sub CmbGroups_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbGroups.SelectedIndexChanged
        DisplayBlock(CmbGroups.SelectedIndex)
    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStrip1.Opening
        If HexBox1.CanCopy Then
            Dim CRC32 = GetCRC32Selected()
            BtnCRC32.Text = "CRC32: " & CRC32
            BtnCRC32.Tag = CRC32
        Else
            BtnCRC32.Text = "CRC32"
            BtnCRC32.Tag = ""
        End If
    End Sub

    Private Sub HexBox1_ByteChanged(source As Object, e As HexBox.ByteChangedArgs) Handles HexBox1.ByteChanged
        PushChange(_CurrentIndex, e.Index, e.PrevValue, HexBox1.SelectionStart, HexBox1.SelectionLength)
    End Sub

    Private Sub HexBox1_InsertActiveChanged(sender As Object, e As EventArgs) Handles HexBox1.InsertActiveChanged
        HexBox1.InsertActive = False
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

    Private Sub HexBox1_MouseWheel(sender As Object, e As MouseEventArgs) Handles HexBox1.MouseWheel
        ProcessMousewheel(e.Delta)
    End Sub

    Private Sub HexBox1_SelectionLengthChanged(sender As Object, e As EventArgs) Handles HexBox1.SelectionLengthChanged
        RefreshSelection(False)
    End Sub

    Private Sub HexBox1_SelectionStartChanged(sender As Object, e As EventArgs) Handles HexBox1.SelectionStartChanged
        RefreshSelection(False)
    End Sub

    Private Sub HexViewForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        For Counter = 0 To _HexViewSectorData.SectorData.BlockCount - 1
            CmbGroups.Items.Add(New HexViewData(_HexViewSectorData.Disk, _HexViewSectorData.SectorData.GetBlock(Counter)))
        Next

        HexBox1.VScrollBarVisible = True

        If CmbGroups.Items.Count > 0 Then
            CmbGroups.SelectedIndex = 0
        End If
    End Sub

    Private Sub ToolStripBtnCommit_Click(sender As Object, e As EventArgs) Handles ToolStripBtnCommit.Click
        CommitChanges()
    End Sub

    Private Sub HexBox1_VisibilityBytesChanged(sender As Object, e As EventArgs) Handles HexBox1.VisibilityBytesChanged
        If _SectorNavigator Or _ClusterNavigator Then
            RefresSelectorValues()
        End If
    End Sub

    Private Sub NumericCluster_ValueChanged(sender As Object, e As EventArgs) Handles NumericCluster.ValueChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        If _ClusterNavigator Then
            JumpToCluster(NumericCluster.Value)
        End If
    End Sub

    Private Sub NumericSector_ValueChanged(sender As Object, e As EventArgs) Handles NumericSector.ValueChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        If _SectorNavigator Then
            JumpToSector(NumericSector.Value)
        End If
    End Sub

    Private Sub NumericSector_KeyDown(sender As Object, e As KeyEventArgs) Handles NumericSector.KeyDown, NumericCluster.KeyDown
        If e.KeyCode = Keys.Return Then
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub ComboTrack_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboTrack.SelectedIndexChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        If _ClusterNavigator Then
            JumpToTrack(ComboTrack.Text)
        End If
    End Sub

#End Region

    Private Class HexChange
        Public Sub New(BlockIndex As Integer, Index As Long, Data As Byte(), SelectionStart As Long, SelectionLength As Long)
            Me.BlockIndex = BlockIndex
            Me.Index = Index
            Me.SelectionLength = SelectionLength
            Me.SelectionStart = SelectionStart
            Me.Data = Data
        End Sub

        Public Sub New(BlockIndex As Integer, Index As Long, Data As Byte, SelectionStart As Long, SelectionLength As Long)
            Me.BlockIndex = BlockIndex
            Me.Index = Index
            Me.SelectionLength = SelectionLength
            Me.SelectionStart = SelectionStart
            ReDim Me.Data(0)
            Me.Data(0) = Data
        End Sub

        Public Property BlockIndex As Integer
        Public Property Data As Byte()
        Public Property Index As Long
        Public Property SelectionLength As Long
        Public Property SelectionStart As Long
    End Class
End Class
