Imports System.ComponentModel
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports System.Threading
Imports DiskImageTool.DiskImage
Imports Hb.Windows.Forms

Public Class HexViewForm
    Private WithEvents CheckBoxSync As ToolStripCheckBox
    Private WithEvents ComboTrack As ToolStripComboBox
    Private WithEvents NumericCluster As ToolStripNumericUpDown
    Private WithEvents NumericSector As ToolStripNumericUpDown
    Public Shared ReadOnly ALT_BACK_COLOR As Color = Color.FromArgb(246, 246, 252)
    Private ReadOnly _BPB As BiosParameterBlock
    Private ReadOnly _Changes As Stack(Of List(Of HexChange))
    Private ReadOnly _ClusterNavigator As Boolean
    Private ReadOnly _HexViewSectorData As HexViewSectorData
    Private ReadOnly _RedoChanges As Stack(Of List(Of HexChange))
    Private ReadOnly _SectorNavigator As Boolean
    Private ReadOnly _SyncBlocks As Boolean
    Private _CurrentHexViewData As HexViewData
    Private _CurrentIndex As Integer = -1
    Private _CurrentSector As UInteger = 0
    Private _IgnoreEvent As Boolean = False
    Private _Initialized As Boolean = False
    Private _LastSearch As HexSearch
    Private _Modified As Boolean = False
    Private _RegionDescriptions As Dictionary(Of UInteger, HexViewHighlightRegion)
    Private _StartingCluster As UShort = 0

    Public Sub New(HexViewSectorData As HexViewSectorData, SectorNavigator As Boolean, ClusterNavigator As Boolean, SyncBlocks As Boolean)
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        _HexViewSectorData = HexViewSectorData
        _BPB = _HexViewSectorData.Disk.BPB
        _Changes = New Stack(Of List(Of HexChange))
        _RedoChanges = New Stack(Of List(Of HexChange))
        _CurrentHexViewData = Nothing
        _SectorNavigator = SectorNavigator
        _ClusterNavigator = ClusterNavigator
        _SyncBlocks = SyncBlocks
        _LastSearch = New HexSearch

        If Not _BPB.IsValid Then
            _BPB = BuildBPB(_HexViewSectorData.Disk.Data.Length)
        End If

        If Not _BPB.IsValid Then
            _ClusterNavigator = False
        End If

        HexBox1.ReadOnly = False

        If HexViewSectorData.Description = "" Then
            Me.Text = "Hex Editor"
        Else
            Me.Text = HexViewSectorData.Description
        End If

        CmbGroups.Size = New Drawing.Size(IIf(_SyncBlocks, 130, 218), CmbGroups.Size.Height)
        CmbGroups.DropDownWidth = CmbGroups.Width

        LblGroups.Visible = Not _SectorNavigator And Not _ClusterNavigator
        CmbGroups.Visible = Not _SectorNavigator And Not _ClusterNavigator

        If _ClusterNavigator Then
            InitializeTrackNavigator()
        End If

        If _SectorNavigator Then
            InitializeSectorNavigator()
        End If

        If _ClusterNavigator Then
            InitializeClusterNavigator()
        End If

        If _SyncBlocks Then
            InitializeSyncCheckBox()
        End If
    End Sub

    Public ReadOnly Property Modified As Boolean
        Get
            Return _Modified
        End Get
    End Property

    Private Shared Function ClipboardHasHex() As Boolean
        Dim DataObject = Clipboard.GetDataObject()

        If DataObject.GetDataPresent(GetType(String)) Then
            Dim Hex = CStr(DataObject.GetData(GetType(String)))
            Return ConvertHexToBytes(Hex) IsNot Nothing
        End If

        Return False
    End Function

    Public Sub SetStartingCluster(Cluster As UShort)
        _StartingCluster = Cluster
    End Sub

    Private Sub CommitChanges()
        _HexViewSectorData.SectorData.CommitChanges()
        _Modified = True

        Me.Close()
    End Sub

    Private Shared Function ConvertHexToByte(Hex As String, <Out> ByRef b As Byte) As Boolean
        Return Byte.TryParse(Hex, NumberStyles.HexNumber, Thread.CurrentThread.CurrentCulture, b)
    End Function

    Private Shared Function ConvertHexToBytes(Hex As String) As Byte()
        If String.IsNullOrEmpty(Hex) Then
            Return Nothing
        End If

        Hex = Hex.Trim()
        Hex = Hex.Replace(" ", "")
        Hex = Hex.Replace(Chr(13), "")
        Hex = Hex.Replace(Chr(10), "")
        Hex = Hex.Replace(Chr(9), "")

        Dim regex = New Regex("^[0-9A-F]*$", RegexOptions.IgnoreCase)

        Dim HexArray As String()
        If regex.IsMatch(Hex) Then
            If Hex.Length Mod 2 = 1 Then
                Hex = "0" & Hex
            End If
            HexArray = New String(Hex.Length / 2 - 1) {}
            For i As Integer = 0 To Hex.Length / 2 - 1
                HexArray(i) = Hex.Substring(i * 2, 2)
            Next
        Else
            Return Nothing
        End If

        Dim ByteArray = New Byte(HexArray.Length - 1) {}
        Dim b As Byte = Nothing
        For j = 0 To HexArray.Length - 1
            Dim HexValue = HexArray(j)

            If Not ConvertHexToByte(HexValue, b) Then
                Return Nothing
            End If

            ByteArray(j) = b
        Next

        Return ByteArray
    End Function

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
                .ByteProvider = _CurrentHexViewData.ByteProvider
                .LineInfoOffset = Disk.SectorToBytes(_CurrentHexViewData.SectorBlock.SectorStart)

                HighlightedRegions.Sort()

                Dim TotalLength As UInteger = .ByteProvider.Length
                Dim Start As UInteger = 0
                Dim Length = Disk.BYTES_PER_SECTOR - (Start Mod Disk.BYTES_PER_SECTOR)
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
                        If HighlightBackColor = Color.White AndAlso Disk.OffsetToSector(Start) Mod 2 = 1 Then
                            HighlightBackColor = ALT_BACK_COLOR
                        End If
                        If HighlightForeColor <> Color.Black Or HighlightBackColor <> Color.White Then
                            .HighlightForeColor = HighlightForeColor
                            .HighlightBackColor = HighlightBackColor
                            .Highlight(Start, Size)
                        End If
                    End If

                    Start += Size
                    Length = Disk.BYTES_PER_SECTOR - (Start Mod Disk.BYTES_PER_SECTOR)
                    Size = Math.Min(Length, TotalLength - Start)
                Loop
            End If
        End With

        InitRegionDescriptions(HighlightedRegions)
        RefreshSelection(True)
        RefresSelectors()

        ToolStripStatusBytes.Text = Format(_CurrentHexViewData.SectorBlock.Size, "N0") & " bytes"
    End Sub

    Private Shared Function FillRegion(ByteProvider As IByteProvider, Offset As Integer, Length As Integer, Value As Byte) As FillRegionResult
        Dim Result As New FillRegionResult(Length)

        For Index = 0 To Length - 1
            Result.OriginalData(Index) = ByteProvider.ReadByte(Offset + Index)
            If Result.OriginalData(Index) <> Value Then
                ByteProvider.WriteByte(Offset + Index, Value)
                Result.Modified = True
            End If
        Next

        Return Result
    End Function

    Private Shared Function FillRegion(ByteProvider As IByteProvider, Offset As Integer, Length As Integer, Value() As Byte) As FillRegionResult
        Dim Result As New FillRegionResult(Length)

        For Index = 0 To Length - 1
            Result.OriginalData(Index) = ByteProvider.ReadByte(Offset + Index)
            If Result.OriginalData(Index) <> Value(Index) Then
                ByteProvider.WriteByte(Offset + Index, Value(Index))
                Result.Modified = True
            End If
        Next

        Return Result
    End Function

    Private Sub FillSelected(Value As Byte)
        Dim ChangeList As New List(Of HexChange)
        Dim DoSync As Boolean = _SyncBlocks AndAlso CheckBoxSync.Checked
        Dim StartIndex As Integer
        Dim EndIndex As Integer

        If DoSync Then
            StartIndex = 0
            EndIndex = CmbGroups.Items.Count - 1
        Else
            StartIndex = _CurrentIndex
            EndIndex = _CurrentIndex
        End If

        For Counter = StartIndex To EndIndex
            Dim IsPrimary As Boolean = (Counter = _CurrentIndex)
            Dim Data As HexViewData = CmbGroups.Items(Counter)
            Dim Result = FillRegion(Data.ByteProvider, HexBox1.SelectionStart, HexBox1.SelectionLength, Value)
            If Result.Modified Then
                ChangeList.Add(New HexChange(Counter, HexBox1.SelectionStart, Result.OriginalData, HexBox1.SelectionStart, HexBox1.SelectionLength, IsPrimary))
            End If
        Next

        If ChangeList.Count > 0 Then
            PushChanges(ChangeList)
            HexBox1.Invalidate()
        End If
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
    Private Sub InitializeSyncCheckBox()
        CheckBoxSync = New ToolStripCheckBox With {
            .Alignment = ToolStripItemAlignment.Right,
            .Checked = True,
            .Margin = New Padding(0, 3, 6, 2),
            .Text = "Sync FATs"
        }

        ToolStripMain.Items.Add(CheckBoxSync)

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
        Dim ClusterStart = _BPB.SectorToCluster(SectorStart)
        Dim ClusterEnd = _BPB.SectorToCluster(SectorEnd)
        Dim Sector = _BPB.ClusterToSector(Cluster)

        If Cluster > 1 And Cluster >= ClusterStart And Cluster <= ClusterEnd Then
            Dim Offset = Disk.SectorToBytes(Sector) - HexBox1.LineInfoOffset
            Dim Line = Offset \ HexBox1.BytesPerLine
            HexBox1.PerformScrollToLine(Line)
        End If
    End Sub

    Private Sub JumpToSector(Sector As UInteger)
        Dim SectorStart = _CurrentHexViewData.SectorBlock.SectorStart
        Dim SectorEnd = SectorStart + _CurrentHexViewData.SectorBlock.SectorCount - 1

        If Sector >= SectorStart And Sector <= SectorEnd Then
            Dim Offset = Disk.SectorToBytes(Sector) - HexBox1.LineInfoOffset
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
        Dim TrackStart = _BPB.SectorToTrack(SectorStart)
        Dim TrackEnd = _BPB.SectorToTrack(SectorEnd)
        Dim Sector = _BPB.TrackToSector(Track, Side)

        If Track >= TrackStart And Track <= TrackEnd Then
            Dim Offset = Disk.SectorToBytes(Sector) - HexBox1.LineInfoOffset
            Dim Line = Offset \ HexBox1.BytesPerLine
            HexBox1.PerformScrollToLine(Line)
        End If
    End Sub

    Private Sub PasteHex()
        Dim HexBytes = ConvertHexToBytes(Clipboard.GetText)
        Dim Offset = HexBox1.SelectionStart
        Dim Length = HexBytes.Length

        If Offset + Length > HexBox1.ByteProvider.Length Then
            Length = HexBox1.ByteProvider.Length - Offset
        End If

        If Length > 0 Then
            Dim ChangeList As New List(Of HexChange)
            Dim DoSync As Boolean = _SyncBlocks AndAlso CheckBoxSync.Checked
            Dim StartIndex As Integer
            Dim EndIndex As Integer

            If DoSync Then
                StartIndex = 0
                EndIndex = CmbGroups.Items.Count - 1
            Else
                StartIndex = _CurrentIndex
                EndIndex = _CurrentIndex
            End If

            For Counter = StartIndex To EndIndex
                Dim IsPrimary As Boolean = (Counter = _CurrentIndex)
                Dim Data As HexViewData = CmbGroups.Items(Counter)
                Dim Result = FillRegion(Data.ByteProvider, Offset, Length, HexBytes)
                If Result.Modified Then
                    ChangeList.Add(New HexChange(Counter, Offset, Result.OriginalData, HexBox1.SelectionStart, HexBox1.SelectionLength, IsPrimary))
                End If
            Next

            If ChangeList.Count > 0 Then
                PushChanges(ChangeList)
                HexBox1.SelectionLength = Length
                HexBox1.Invalidate()
            End If
        End If
    End Sub

    Private Sub PopChange(Source As Stack(Of List(Of HexChange)), Destination As Stack(Of List(Of HexChange)))
        If Source.Count > 0 Then
            Dim HexChangelist = Source.Pop()
            Dim DestinationChangeList = New List(Of HexChange)

            For Each HexChange In HexChangelist
                Dim Data As HexViewData = CmbGroups.Items(HexChange.BlockIndex)

                Dim OriginalData = ReadBytes(Data.ByteProvider, HexChange.Index, HexChange.Data.Length)
                DestinationChangeList.Add(New HexChange(HexChange.BlockIndex, HexChange.Index, OriginalData, HexChange.SelectionStart, HexChange.SelectionLength, HexChange.IsPrimary))

                WriteBytes(Data.ByteProvider, HexChange.Index, HexChange.Data)

                If HexChange.IsPrimary Then
                    If HexChange.BlockIndex <> _CurrentIndex Then
                        CmbGroups.SelectedIndex = HexChange.BlockIndex
                    End If
                    HexBox1.Select(HexChange.SelectionStart, HexChange.SelectionLength)
                End If
            Next

            Destination.Push(DestinationChangeList)

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

    Private Sub PushChanges(ChangeList As List(Of HexChange))
        _Changes.Push(ChangeList)
        _RedoChanges.Clear()
        RefreshUndoButtons()
    End Sub

    Private Shared Function ReadBytes(ByteProvider As IByteProvider, Offset As Long, Length As Integer) As Byte()
        Dim Data(Length - 1) As Byte

        For Counter = 0 To Data.Length - 1
            Data(Counter) = ByteProvider.ReadByte(Offset + Counter)
        Next

        Return Data
    End Function

    Private Sub RefreshPasteButton()
        BtnPaste.Enabled = ClipboardHasHex()
        ToolStripBtnPaste.Enabled = BtnPaste.Enabled
    End Sub

    Private Sub RefreshSelection(ForceUpdate As Boolean)
        If Not _Initialized And Not ForceUpdate Then
            Exit Sub
        End If

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
            Dim OffsetStart As UInteger = HexBox1.LineInfoOffset + SelectionStart
            Dim OffsetEnd As Integer = HexBox1.LineInfoOffset + SelectionEnd
            Dim FileName As String = ""
            Dim OutOfRange As Boolean = SelectionStart >= HexBox1.ByteProvider.Length

            Dim Sector = Disk.OffsetToSector(OffsetStart)

            ToolStripStatusOffset.Visible = Not OutOfRange
            ToolStripStatusOffset.Text = "Offset(h): " & OffsetStart.ToString("X")

            If SelectionLength = 0 Then
                ToolStripStatusBlock.Visible = False
                ToolStripStatusBlock.Text = ""
                ToolStripStatusLength.Visible = False
                ToolStripStatusLength.Text = ""
            Else
                ToolStripStatusBlock.Visible = True
                ToolStripStatusBlock.Text = "Block(h): " & OffsetStart.ToString("X") & "-" & OffsetEnd.ToString("X")
                ToolStripStatusLength.Visible = True
                ToolStripStatusLength.Text = "Length(h): " & SelectionLength.ToString("X")
            End If

            If _CurrentSector <> Sector Or ForceUpdate Then
                Dim Cluster As UShort
                If _BPB.IsValid Then
                    Cluster = _BPB.SectorToCluster(Sector)
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

                    If _BPB.IsValid Then
                        If _CurrentHexViewData.Disk.FAT.FileAllocation.ContainsKey(Cluster) Then
                            Dim OffsetList = _CurrentHexViewData.Disk.FAT.FileAllocation.Item(Cluster)
                            Dim DirectoryEntry = _CurrentHexViewData.Disk.GetDirectoryEntryByOffset(OffsetList.Item(0))
                            FileName = DirectoryEntry.GetFullFileName
                        End If
                    End If
                End If

                If _BPB.IsValid Then
                    ToolStripStatusTrack.Visible = Not OutOfRange
                    ToolStripStatusTrack.Text = "Track: " & _BPB.SectorToTrack(Sector)

                    ToolStripStatusSide.Visible = Not OutOfRange
                    ToolStripStatusSide.Text = "Side: " & _BPB.SectorToSide(Sector)
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

            If _RegionDescriptions.Count = 0 Then
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
                NumericCluster.Minimum = _BPB.SectorToCluster(SectorStart)
                NumericCluster.Maximum = _BPB.SectorToCluster(SectorEnd)
                NumericCluster.Enabled = True

                ComboTrack.Items.Clear()
                Dim TrackStart = _BPB.SectorToTrack(SectorStart)
                Dim TrackEnd = _BPB.SectorToTrack(SectorEnd)
                For i = TrackStart To TrackEnd
                    For j = 0 To _BPB.NumberOfHeads - 1
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
        Dim Sector = Disk.OffsetToSector(Offset)

        _IgnoreEvent = True

        If _SectorNavigator Then
            If Sector <> NumericSector.Value Then
                NumericSector.Value = Sector
            End If
        End If

        If _ClusterNavigator Then
            Dim Cluster = _BPB.SectorToCluster(Sector)

            If Cluster <> NumericCluster.Value Then
                NumericCluster.Value = Cluster
            End If

            Dim Track = _BPB.SectorToTrack(Sector)
            Dim Side = _BPB.SectorToSide(Sector)
            Dim Value = Track.ToString & "." & Side.ToString
            If Value <> ComboTrack.Text Then
                ComboTrack.Text = Value
            End If
        End If

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
            RefresSelectorValues()
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

    Private Sub SelectCurrentSector()
        Dim OutOfRange As Boolean = HexBox1.SelectionStart >= HexBox1.ByteProvider.Length
        If Not OutOfRange Then
            Dim Offset = Disk.SectorToBytes(_CurrentSector) - HexBox1.LineInfoOffset
            Dim Length = Disk.BYTES_PER_SECTOR
            HexBox1.Select(Offset, Length)
        End If
    End Sub
    Private Shared Sub WriteBytes(ByteProvider As IByteProvider, Offset As Long, Data() As Byte)
        For Counter = 0 To Data.Length - 1
            ByteProvider.WriteByte(Offset + Counter, Data(Counter))
        Next
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

    Private Sub BtnFind_Click(sender As Object, e As EventArgs) Handles BtnFind.Click, ToolStripBtnFind.Click
        Search(False)
    End Sub

    Private Sub BtnFindNext_Click(sender As Object, e As EventArgs) Handles BtnFindNext.Click, ToolStripBtnFindNext.Click
        Search(True)
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

    Private Sub ComboTrack_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboTrack.SelectedIndexChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        If _ClusterNavigator Then
            JumpToTrack(ComboTrack.Text)
        End If
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
        Dim ChangeList As New List(Of HexChange)
        Dim DoSync As Boolean = _SyncBlocks AndAlso CheckBoxSync.Checked

        ChangeList.Add(New HexChange(_CurrentIndex, e.Index, e.PrevValue, HexBox1.SelectionStart, HexBox1.SelectionLength, True))

        If DoSync Then
            For Counter = 0 To CmbGroups.Items.Count - 1
                If Counter <> _CurrentIndex Then
                    Dim Data As HexViewData = CmbGroups.Items(Counter)
                    Dim PrevValue As Byte = Data.ByteProvider.ReadByte(e.Index)
                    Data.ByteProvider.WriteByte(e.Index, e.Value)
                    ChangeList.Add(New HexChange(Counter, e.Index, PrevValue, HexBox1.SelectionStart, HexBox1.SelectionLength, False))
                End If
            Next
        End If

        PushChanges(ChangeList)
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
        If _IgnoreEvent Then
            Exit Sub
        End If

        RefreshSelection(False)
    End Sub

    Private Sub HexBox1_SelectionStartChanged(sender As Object, e As EventArgs) Handles HexBox1.SelectionStartChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        RefreshSelection(False)
    End Sub

    Private Sub HexBox1_VisibilityBytesChanged(sender As Object, e As EventArgs) Handles HexBox1.VisibilityBytesChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        If _SectorNavigator Or _ClusterNavigator Then
            RefresSelectorValues()
        End If
    End Sub

    Private Sub HexViewForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub HexViewForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        For Counter = 0 To _HexViewSectorData.SectorData.BlockCount - 1
            CmbGroups.Items.Add(New HexViewData(_HexViewSectorData, Counter))
        Next

        HexBox1.VScrollBarVisible = True

        If CmbGroups.Items.Count > 0 Then
            CmbGroups.SelectedIndex = 0
        End If

        If _StartingCluster <> 0 Then
            JumpToCluster(_StartingCluster)
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

    Private Sub NumericSector_KeyDown(sender As Object, e As KeyEventArgs) Handles NumericSector.KeyDown, NumericCluster.KeyDown
        If e.KeyCode = Keys.Return Then
            e.SuppressKeyPress = True
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

    Private Sub ToolStripBtnCommit_Click(sender As Object, e As EventArgs) Handles ToolStripBtnCommit.Click
        CommitChanges()
    End Sub
#End Region

    Private Class FillRegionResult
        Public Sub New(Length As Integer)
            ReDim _OriginalData(Length - 1)
        End Sub
        Public Property Modified As Boolean = False
        Public Property OriginalData As Byte()
    End Class

    Private Class HexChange
        Public Sub New(BlockIndex As UShort, Index As UInteger, Data As Byte(), SelectionStart As UInteger, SelectionLength As UInteger, IsPrimary As Boolean)
            Me.BlockIndex = BlockIndex
            Me.Index = Index
            Me.IsPrimary = IsPrimary
            Me.SelectionLength = SelectionLength
            Me.SelectionStart = SelectionStart
            Me.Data = Data
        End Sub

        Public Sub New(BlockIndex As UShort, Index As UInteger, Data As Byte, SelectionStart As UInteger, SelectionLength As UInteger, IsPrimary As Boolean)
            Me.BlockIndex = BlockIndex
            Me.Index = Index
            Me.IsPrimary = IsPrimary
            Me.SelectionLength = SelectionLength
            Me.SelectionStart = SelectionStart
            Me.Data = {Data}
        End Sub

        Public Property BlockIndex As UShort
        Public Property Data As Byte()
        Public Property Index As UInteger
        Public Property IsPrimary As Boolean
        Public Property SelectionLength As UInteger
        Public Property SelectionStart As UInteger
    End Class
End Class
