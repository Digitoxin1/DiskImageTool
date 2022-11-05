Imports System.Text

Public Class HexViewForm
    Public Shared ReadOnly ALT_BACK_COLOR As Color = Color.FromArgb(246, 246, 252)
    Private _AllowModifications As Boolean = False
    Private _CurrentSector As UInteger = 0
    Private _DataList As List(Of HexViewData)
    Private _Disk As DiskImage.Disk
    Private _Modified As Boolean = False
    Private _RegionDescriptions As Dictionary(Of UInteger, HexViewHighlightRegion)
    Private _Initialized As Boolean = False

    Public Sub New(Disk As DiskImage.Disk, Block As DiskImage.DataBlock, AllowModifications As Boolean, Caption As String)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim DataList = New List(Of HexViewData) From {
            New HexViewData(Block)
        }
        Initialize(Disk, DataList, AllowModifications, Caption)
    End Sub

    Public Sub New(Disk As DiskImage.Disk, Data As HexViewData, AllowModifications As Boolean, Caption As String)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim DataList = New List(Of HexViewData) From {
            Data
        }
        Initialize(Disk, DataList, AllowModifications, Caption)
    End Sub

    Public Sub New(Disk As DiskImage.Disk, DataBlockList As List(Of DiskImage.DataBlock), AllowModifications As Boolean, Caption As String)
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        Dim DataList = New List(Of HexViewData)
        For Each Block In DataBlockList
            DataList.Add(New HexViewData(Block))
        Next
        Initialize(Disk, DataList, AllowModifications, Caption)
    End Sub

    Public Sub New(Disk As DiskImage.Disk, DataList As List(Of HexViewData), AllowModifications As Boolean, Caption As String)
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        Initialize(Disk, DataList, AllowModifications, Caption)
    End Sub

    Public ReadOnly Property Modified As Boolean
        Get
            Return _Modified
        End Get
    End Property

    Private Sub ClearData()
        Dim Value = Convert.ToByte(ComboBytes.Text, 16)
        Dim SectorSize = _Disk.BootSector.BytesPerSector
        Dim Data(SectorSize - 1) As Byte
        Dim SectorOffset As UInteger = 0

        If HexBox1.SelectionLength > 0 Then
            Dim HexOffsetStart = HexBox1.SelectionStart
            Dim HexOffsetEnd = HexOffsetStart + HexBox1.SelectionLength - 1
            Dim PrevSector As UInteger = 0
            For Index = HexOffsetStart To HexOffsetEnd
                Dim Offset As UInteger = HexBox1.LineInfoOffset + Index
                Dim Sector = _Disk.OffsetToSector(Offset)
                If Sector <> PrevSector Or Index = HexOffsetStart Then
                    If Index > HexOffsetStart Then
                        _Disk.SetBytes(Data, SectorOffset)
                    End If
                    SectorOffset = _Disk.SectorToOffset(Sector)
                    Data = _Disk.GetBytes(SectorOffset, SectorSize)
                End If
                HexBox1.ByteProvider.WriteByte(Index, Value)
                Data(Offset - SectorOffset) = Value
            Next
            _Disk.SetBytes(Data, SectorOffset)
        Else
            Dim Sector = _Disk.OffsetToSector(GetVisibleOffset)
            SectorOffset = _Disk.SectorToOffset(Sector)
            Dim OffsetStart = SectorOffset - HexBox1.LineInfoOffset
            Dim OffsetEnd = OffsetStart + SectorSize - 1
            For Index = OffsetStart To OffsetEnd
                HexBox1.ByteProvider.WriteByte(Index, Value)
            Next
            For Index = 0 To Data.Length - 1
                Data(Index) = Value
            Next
            _Disk.SetBytes(Data, SectorOffset)
        End If

        HexBox1.Refresh()
        _Modified = True
    End Sub

    Private Sub DisplayBlock(Data As HexViewData)
        Dim BlockLength As UInteger

        With HexBox1
            .ByteProvider = Nothing

            If Data.DataBlock.Offset + Data.DataBlock.Length > _Disk.Data.Length Then
                BlockLength = Math.Max(0, _Disk.Data.Length - Data.DataBlock.Offset)
            Else
                BlockLength = Data.DataBlock.Length
            End If

            If BlockLength > 0 Then
                .ByteProvider = New Hb.Windows.Forms.DynamicByteProvider(_Disk.GetBytes(Data.DataBlock.Offset, BlockLength))
                .LineInfoOffset = Data.DataBlock.Offset

                Data.HighlightedRegions.Sort()

                Dim BytesPerSector As UShort
                If _Disk.IsValidImage Then
                    BytesPerSector = _Disk.BootSector.BytesPerSector
                Else
                    BytesPerSector = 512
                End If
                Dim TotalLength As UInteger = .ByteProvider.Length
                Dim Start As UInteger = 0
                Dim Length = BytesPerSector - (Start Mod BytesPerSector)
                Dim Size As UInteger = Math.Min(Length, TotalLength - Start)
                Dim CurrentRegion As HexViewHighlightRegion
                Dim Index As Integer = 0
                If Index < Data.HighlightedRegions.Count Then
                    CurrentRegion = Data.HighlightedRegions(Index)
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
                            If Index < Data.HighlightedRegions.Count Then
                                CurrentRegion = Data.HighlightedRegions(Index)
                            Else
                                CurrentRegion = Nothing
                            End If
                        End If
                    End If

                    If Size > 0 Then
                        If HighlightBackColor = Color.White AndAlso (Start \ BytesPerSector) Mod 2 = 1 Then
                            HighlightBackColor = ALT_BACK_COLOR
                        End If
                        If HighlightForeColor <> Color.Black Or HighlightBackColor <> Color.White Then
                            .HighlightForeColor = HighlightForeColor
                            .HighlightBackColor = HighlightBackColor
                            .Highlight(Start, Size)
                        End If
                    End If

                    Start += Size
                    Length = BytesPerSector - (Start Mod BytesPerSector)
                    Size = Math.Min(Length, TotalLength - Start)
                Loop
            End If
        End With

        InitRegionDescriptions(Data.HighlightedRegions)
        RefreshSelection(True)

        ToolStripStatusBytes.Text = Format(BlockLength, "N0") & " bytes"
    End Sub

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

    Private Sub Initialize(Disk As DiskImage.Disk, DataList As List(Of HexViewData), AllowModifications As Boolean, Caption As String)
        _Disk = Disk
        _DataList = DataList
        _AllowModifications = AllowModifications

        Me.Text = "Hex Viewer" & IIf(Caption <> "", " - " & Caption, "")
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

    Private Sub ProcessKeyPress(e As KeyEventArgs)
        If e.KeyCode = 40 Then 'Down Arrow
            Dim LineCount = Math.Ceiling(HexBox1.ByteProvider.Length / HexBox1.HorizontalByteCount)
            If HexBox1.CurrentLine >= LineCount Then
                If CmbGroups.SelectedIndex < CmbGroups.Items.Count - 1 Then
                    Dim Offset = HexBox1.SelectionStart Mod HexBox1.HorizontalByteCount
                    CmbGroups.SelectedIndex = CmbGroups.SelectedIndex + 1
                    HexBox1.SelectionStart = Offset
                    e.SuppressKeyPress = True
                End If
            End If
        ElseIf e.KeyCode = 38 Then 'Up Arrow
            If HexBox1.CurrentLine <= 1 Then
                If CmbGroups.SelectedIndex > 0 Then
                    Dim Offset = HexBox1.SelectionStart Mod HexBox1.HorizontalByteCount
                    CmbGroups.SelectedIndex = CmbGroups.SelectedIndex - 1
                    HexBox1.SelectionStart = HexBox1.ByteProvider.Length - (HexBox1.HorizontalByteCount - Offset)
                    e.SuppressKeyPress = True
                End If
            End If
        ElseIf e.KeyCode = 35 Then 'End
            If CmbGroups.SelectedIndex < CmbGroups.Items.Count - 1 Then
                CmbGroups.SelectedIndex = CmbGroups.Items.Count - 1
                HexBox1.SelectionStart = HexBox1.ByteProvider.Length - HexBox1.HorizontalByteCount
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = 36 Then 'Home
            If CmbGroups.SelectedIndex > 0 Then
                CmbGroups.SelectedIndex = 0
                e.SuppressKeyPress = True
            End If
        ElseIf e.KeyCode = 33 Then 'Page Up
            If HexBox1.StartByte = 0 Then
                If CmbGroups.SelectedIndex > 0 Then
                    Dim Offset = HexBox1.SelectionStart Mod HexBox1.HorizontalByteCount
                    CmbGroups.SelectedIndex = CmbGroups.SelectedIndex - 1
                    HexBox1.SelectionStart = HexBox1.ByteProvider.Length - (HexBox1.HorizontalByteCount - Offset)
                    e.SuppressKeyPress = True
                End If
            End If
        ElseIf e.KeyCode = 34 Then 'Page Down
            If HexBox1.EndByte = HexBox1.ByteProvider.Length - 1 Then
                If CmbGroups.SelectedIndex < CmbGroups.Items.Count - 1 Then
                    Dim Offset = HexBox1.SelectionStart Mod HexBox1.HorizontalByteCount
                    CmbGroups.SelectedIndex = CmbGroups.SelectedIndex + 1
                    HexBox1.SelectionStart = Offset
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
                    HexBox1.SelectionStart = Offset
                End If
            End If
        ElseIf Delta > 0 Then
            If HexBox1.StartByte = 0 Then
                If CmbGroups.SelectedIndex > 0 Then
                    Dim Offset = HexBox1.SelectionStart Mod HexBox1.HorizontalByteCount
                    CmbGroups.SelectedIndex = CmbGroups.SelectedIndex - 1
                    HexBox1.SelectionStart = HexBox1.ByteProvider.Length - (HexBox1.HorizontalByteCount - Offset)
                End If
            End If
        End If
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
            ToolStripStatusFile.Visible = False
            ToolStripStatusDescription.Visible = False
            BtnClear.Text = "Clear Sector"
            BtnClear.Enabled = False
            ComboBytes.Enabled = False
        Else
            Dim Offset As UInteger = HexBox1.LineInfoOffset + SelectionStart
            Dim Sector As UInteger = 0
            Dim ClearEnabled As Boolean = False
            Dim FileName As String = ""
            Dim OutOfRange As Boolean = SelectionStart >= HexBox1.ByteProvider.Length

            If _Disk.IsValidImage Then
                Sector = _Disk.OffsetToSector(Offset)
            End If

            ToolStripStatusOffset.Visible = Not OutOfRange
            ToolStripStatusOffset.Text = "Offset(h): " & SelectionStart.ToString("X")

            If SelectionLength = 0 Then
                ToolStripStatusBlock.Visible = False
                ToolStripStatusBlock.Text = ""
                ToolStripStatusLength.Visible = False
                ToolStripStatusLength.Text = ""

                BtnClear.Text = "Clear Sector " & _Disk.OffsetToSector(GetVisibleOffset)
            Else
                ToolStripStatusBlock.Visible = True
                ToolStripStatusBlock.Text = "Block(h): " & SelectionStart.ToString("X") & "-" & SelectionEnd.ToString("X")
                ToolStripStatusLength.Visible = True
                ToolStripStatusLength.Text = "Length(h): " & SelectionLength.ToString("X")
                BtnClear.Text = "Clear Selection"
            End If

            If _CurrentSector <> Sector Or ForceUpdate Then
                Dim Cluster As UShort = _Disk.SectorToCluster(Sector)

                ToolStripStatusSector.Visible = Not OutOfRange
                ToolStripStatusSector.Text = "Sector: " & Sector
                If Cluster < 2 Then
                    ToolStripStatusCluster.Visible = False
                    ToolStripStatusCluster.Text = ""
                Else
                    ToolStripStatusCluster.Visible = Not OutOfRange
                    ToolStripStatusCluster.Text = "Cluster: " & Cluster

                    If _Disk.FileAllocation.ContainsKey(Cluster) Then
                        Dim OffsetList = _Disk.FileAllocation.Item(Cluster)
                        Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(OffsetList.Item(0))
                        FileName = DirectoryEntry.GetFullFileName
                    Else
                        ClearEnabled = Not OutOfRange
                    End If
                End If

                If FileName.Length = 0 Then
                    ToolStripStatusFile.Visible = False
                    ToolStripStatusFile.Text = ""
                Else
                    ToolStripStatusFile.Visible = Not OutOfRange
                    ToolStripStatusFile.Text = "File: " & FileName
                End If

                BtnClear.Enabled = ClearEnabled
                ComboBytes.Enabled = ClearEnabled

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

        _Initialized = True
    End Sub

#Region "Events"

    Private Sub BtnClear_Click(sender As Object, e As EventArgs) Handles BtnClear.Click
        ClearData()
    End Sub

    Private Sub BtnCopyHex_Click(sender As Object, e As EventArgs) Handles BtnCopyHex.Click
        HexBox1.CopyHex()
    End Sub

    Private Sub BtnCopyHexFormatted_Click(sender As Object, e As EventArgs) Handles BtnCopyHexFormatted.Click
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
    End Sub

    Private Sub BtnCopyText_Click(sender As Object, e As EventArgs) Handles BtnCopyText.Click
        HexBox1.Copy()
    End Sub

    Private Sub BtnCRC32_Click(sender As Object, e As EventArgs) Handles BtnCRC32.Click
        Dim OffsetStart As UInteger = HexBox1.SelectionStart + HexBox1.LineInfoOffset
        Dim Length As UInteger = HexBox1.SelectionLength

        Dim B = _Disk.GetBytes(OffsetStart, Length)
        Dim Checksum = Crc32.ComputeChecksum(B).ToString("X8")
        Dim Range = OffsetStart.ToString("X8") & "-" & (OffsetStart + Length - 1).ToString("X8")
        Dim Msg As String = "The CRC32 for " & Range & " is: " & Checksum
        MsgBox(Msg, MsgBoxStyle.Information)
    End Sub

    Private Sub BtnSelectAll_Click(sender As Object, e As EventArgs) Handles BtnSelectAll.Click
        HexBox1.SelectAll()
    End Sub

    Private Sub CmbGroups_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbGroups.SelectedIndexChanged
        Dim Group As ComboGroups = CmbGroups.SelectedItem
        DisplayBlock(Group.Data)
    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening
        BtnCopyText.Enabled = HexBox1.SelectionLength > 0
        BtnCopyHex.Enabled = HexBox1.SelectionLength > 0
        BtnCopyHexFormatted.Enabled = HexBox1.SelectionLength > 0
        BtnCRC32.Enabled = HexBox1.SelectionLength > 0
    End Sub

    Private Sub HexBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles HexBox1.KeyDown
        ProcessKeyPress(e)
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

    Private Sub HexBox1_VisibilityBytesChanged(sender As Object, e As EventArgs) Handles HexBox1.VisibilityBytesChanged
        If HexBox1.SelectionLength = 0 Then
            BtnClear.Text = "Clear Sector " & _Disk.OffsetToSector(GetVisibleOffset)
        Else
            BtnClear.Text = "Clear Selection"
        End If
    End Sub
    Private Sub HexViewForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        If _AllowModifications Then
            BtnClear.Visible = True
            ComboBytes.Visible = True
            LblHeaderFill.Visible = True
        Else
            BtnClear.Visible = False
            ComboBytes.Visible = False
            LblHeaderFill.Visible = False
        End If

        LblHeaderCaption.Text = "Display:"

        If _DataList.Count > 1 Or _AllowModifications Then
            FlowLayoutHeader.Visible = True
            FlowLayoutHeader.Left = (Me.ClientSize.Width - FlowLayoutHeader.Width) / 2
            HexBox1.Top = FlowLayoutHeader.Bottom + 6
            For Each HexViewData In _DataList
                CmbGroups.Items.Add(New ComboGroups(_Disk, HexViewData))
            Next
            ComboBytes.SelectedIndex = 0
        Else
            FlowLayoutHeader.Visible = False
            HexBox1.Top = 12
        End If

        HexBox1.Anchor = AnchorStyles.Left + AnchorStyles.Right + AnchorStyles.Top
        Me.ClientSize = New Size(Me.ClientSize.Width, HexBox1.Bottom + StatusStrip1.Height + 3)
        HexBox1.Anchor = AnchorStyles.Left + AnchorStyles.Right + AnchorStyles.Top + AnchorStyles.Bottom

        HexBox1.VScrollBarVisible = True

        DisplayBlock(_DataList(0))

        If CmbGroups.Items.Count > 0 Then
            CmbGroups.SelectedIndex = 0
        End If
    End Sub

#End Region

    Private Class ComboGroups
        Public Sub New(Disk As DiskImage.Disk, Data As HexViewData)
            Me.Disk = Disk
            Me.Data = Data
        End Sub

        Public Property Data As HexViewData
        Public Property Disk As DiskImage.Disk
        Public Overrides Function ToString() As String
            Dim Sector = Disk.OffsetToSector(Data.DataBlock.Offset)
            Dim SectorEnd = Disk.OffsetToSector(Data.DataBlock.Offset + Data.DataBlock.Length - 1)
            Dim Cluster = Disk.SectorToCluster(Sector)
            Dim ClusterEnd = Disk.SectorToCluster(SectorEnd)

            Dim Header As String = "Sector " & Sector & IIf(SectorEnd > Sector, " - " & SectorEnd, "")
            If Cluster > 0 Then
                Header = "Cluster " & Cluster & IIf(ClusterEnd > Cluster, " - " & ClusterEnd, "") & "; " & Header
            End If
            If Data.DataBlock.Caption <> "" Then
                Header = Data.DataBlock.Caption & "  (" & Header & ")"
            End If

            Return Header
        End Function

    End Class

End Class
