
Public Class HexViewForm
    Private _SuppressEvent As Boolean = False
    Private _Disk As DiskImage.Disk
    Private _Modified As Boolean = False
    Private _JumpPoints As Boolean = False
    Private _CurrentSector As UInteger = 0
    Private _AllowModifications As Boolean = False

    Public Sub New(Disk As DiskImage.Disk, Block As DiskImage.DataBlock, AllowModifications As Boolean, Caption As String)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim DataList = New List(Of HexViewData) From {
            New HexViewData(Block)
        }
        Initialize(Disk, DataList, AllowModifications, Caption, False)
    End Sub

    Public Sub New(Disk As DiskImage.Disk, Data As HexViewData, AllowModifications As Boolean, Caption As String)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim DataList = New List(Of HexViewData) From {
            Data
        }
        Initialize(Disk, DataList, AllowModifications, Caption, False)
    End Sub

    Public Sub New(Disk As DiskImage.Disk, DataBlockList As List(Of DiskImage.DataBlock), AllowModifications As Boolean, Caption As String)
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        Dim DataList = New List(Of HexViewData)
        For Each Block In DataBlockList
            DataList.Add(New HexViewData(Block))
        Next
        Initialize(Disk, DataList, AllowModifications, Caption, False)
    End Sub

    Public Sub New(Disk As DiskImage.Disk, DataList As List(Of HexViewData), AllowModifications As Boolean, Caption As String)
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        Initialize(Disk, DataList, AllowModifications, Caption, False)
    End Sub

    Private Sub Initialize(Disk As DiskImage.Disk, DataList As List(Of HexViewData), AllowModifications As Boolean, Caption As String, JumpPoints As Boolean)
        _Disk = Disk
        _JumpPoints = JumpPoints
        _AllowModifications = AllowModifications

        Me.Text = "Hex Viewer" & IIf(Caption <> "", " - " & Caption, "")

        If AllowModifications Then
            BtnClear.Visible = True
            ComboBytes.Visible = True
            LblHeaderFill.Visible = True
        Else
            BtnClear.Visible = False
            ComboBytes.Visible = False
            LblHeaderFill.Visible = False
        End If

        If JumpPoints Then
            LblHeaderCaption.Text = "Jump To:"
        Else
            LblHeaderCaption.Text = "Display:"
        End If

        If DataList.Count > 1 Or JumpPoints Or AllowModifications Then
            FlowLayoutHeader.Visible = True
            FlowLayoutHeader.Left = (Me.ClientSize.Width - FlowLayoutHeader.Width) / 2
            HexBox1.Top = FlowLayoutHeader.Bottom + 6
            For Each HexViewData In DataList
                CmbGroups.Items.Add(New ComboGroups(Disk, HexViewData))
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

        If JumpPoints Then
            Dim Sector = _Disk.OffsetToSector(DataList(0).DataBlock.Offset)
            Dim Cluster = _Disk.SectorToCluster(Sector)
            Dim Offset = _Disk.ClusterToOffset(Cluster)
            Dim OffsetEnd = Math.Min(Disk.Data.Length, Disk.BootSector.ImageSize)

            Dim HexViewData As New HexViewData(DiskImage.Disk.NewDataBlock(Offset, OffsetEnd - Offset))

            DisplayBlock(HexViewData)
        Else
            DisplayBlock(DataList(0))
        End If

        If CmbGroups.Items.Count > 0 Then
            CmbGroups.SelectedIndex = 0
        End If
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
            Dim Sector = _Disk.OffsetToSector(HexBox1.LineInfoOffset + HexBox1.StartByte)
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
        With HexBox1
            .ByteProvider = Nothing
            .ByteProvider = New Hb.Windows.Forms.DynamicByteProvider(_Disk.GetBytes(Data.DataBlock.Offset, Data.DataBlock.Length))
            .LineInfoOffset = Data.DataBlock.Offset
            For Each HighlightRegion In Data.HighlightedRegions
                .HighlightBackColor = HighlightRegion.BackColor
                .HighlightForeColor = HighlightRegion.ForeColor
                .Highlight(HighlightRegion.Start, HighlightRegion.Size)
            Next
        End With
        PopulateToolstrip(Data.DataBlock.Offset, True)
        ToolStripStatusBytes.Text = Format(Data.DataBlock.Length, "N0") & " bytes"
    End Sub

    Private Sub JumpTo(Offset As UInteger)
        Offset -= HexBox1.LineInfoOffset

        If Offset > HexBox1.StartByte Then
            Offset += (HexBox1.VerticalByteCount - 1) * HexBox1.HorizontalByteCount
        End If

        HexBox1.ScrollByteIntoView(Offset)
    End Sub

    Private Function PopulateToolstrip(Offset As UInteger, ForceUpdate As Boolean) As UInteger
        Dim Length As UInteger
        Dim BlockOffset As UInteger = 0
        Dim Cluster As UShort = 0
        Dim Sector As UInteger = 0
        Dim FileName As String = ""

        If _Disk.IsValidImage Then
            Sector = _Disk.OffsetToSector(Offset)
        End If

        If _CurrentSector <> Sector Or ForceUpdate Then
            If _Disk.IsValidImage Then
                Cluster = _Disk.SectorToCluster(Sector)
            End If

            If Cluster = 0 Then
                Length = _Disk.BootSector.BytesPerSector
                BlockOffset = _Disk.SectorToOffset(Sector)
                ToolStripStatusCluster.Visible = False
            Else
                Length = _Disk.BootSector.BytesPerCluster
                BlockOffset = _Disk.ClusterToOffset(Cluster)
                ToolStripStatusCluster.Visible = True
                ToolStripStatusCluster.Text = "Cluster " & Cluster
                If _Disk.FileAllocation.ContainsKey(Cluster) Then
                    Dim OffsetList = _Disk.FileAllocation.Item(Cluster)
                    Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(OffsetList.Item(0))
                    FileName = DirectoryEntry.GetFullFileName
                End If
            End If
            ToolStripStatusSector.Text = "Sector " & Sector
            ToolStripStatusRange.Text = BlockOffset.ToString("X8") & "-" & (BlockOffset + Length - 1).ToString("X8")

            If FileName <> "" Then
                ToolStripStatusFile.Text = FileName
                ToolStripStatusFile.Visible = True
            Else
                ToolStripStatusFile.Visible = False
            End If

            If _AllowModifications Then
                BtnClear.Enabled = (FileName = "")
                ComboBytes.Enabled = (FileName = "")
            End If

            If HexBox1.SelectionLength = 0 Then
                BtnClear.Text = "Clear Sector " & Sector
            End If
            _CurrentSector = Sector
        End If

        Return BlockOffset
    End Function

    Private Sub ProcessMousewheel(Delta As Integer)
        If _JumpPoints Then
            Exit Sub
        End If

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

    Private Sub ProcessKeyPress(e As KeyEventArgs)
        If _JumpPoints Then
            Exit Sub
        End If

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

    Private Sub SelectGroupByOffset(Offset As UInteger)
        For Each Group As ComboGroups In CmbGroups.Items
            If Offset >= Group.Data.DataBlock.Offset And Offset < Group.Data.DataBlock.Offset + Group.Data.DataBlock.Length Then
                If CmbGroups.SelectedItem IsNot Group Then
                    _SuppressEvent = True
                    CmbGroups.SelectedItem = Group
                    _SuppressEvent = False
                End If
                Exit For
            End If
        Next
    End Sub

    Private Sub BtnClear_Click(sender As Object, e As EventArgs) Handles BtnClear.Click
        ClearData()
    End Sub
    Private Sub CmbGroups_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbGroups.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim Group As ComboGroups = CmbGroups.SelectedItem
        If _JumpPoints Then
            JumpTo(Group.Data.DataBlock.Offset)
        Else
            DisplayBlock(Group.Data)
        End If
    End Sub

    Private Sub HexBox1_VisibilityBytesChanged(sender As Object, e As EventArgs) Handles HexBox1.VisibilityBytesChanged
        Dim Offset As UInteger

        If HexBox1.SelectionStart < HexBox1.StartByte Or HexBox1.SelectionStart > HexBox1.EndByte Then
            Offset = HexBox1.StartByte
        Else
            Offset = HexBox1.SelectionStart
        End If

        Offset = PopulateToolstrip(HexBox1.LineInfoOffset + Offset, _JumpPoints)

        If _JumpPoints Then
            SelectGroupByOffset(Offset)
        End If
    End Sub

    Private Sub HexBox1_SelectionLengthChanged(sender As Object, e As EventArgs) Handles HexBox1.SelectionLengthChanged
        If HexBox1.SelectionLength > 0 Then
            BtnClear.Text = "Clear Selection"
        Else
            BtnClear.Text = "Clear Sector"
        End If
    End Sub

    Private Sub HexBox1_MouseWheel(sender As Object, e As MouseEventArgs) Handles HexBox1.MouseWheel
        ProcessMousewheel(e.Delta)
    End Sub

    Private Sub HexBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles HexBox1.KeyDown
        ProcessKeyPress(e)
    End Sub

    Private Class ComboGroups
        Public Property Disk As DiskImage.Disk
        Public Property Data As HexViewData

        Public Sub New(Disk As DiskImage.Disk, Data As HexViewData)
            Me.Disk = Disk
            Me.Data = Data
        End Sub
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

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening
        BtnCopyText.Enabled = HexBox1.SelectionLength > 0
        BtnCopyHex.Enabled = HexBox1.SelectionLength > 0
        BtnCopyHexFormatted.Enabled = HexBox1.SelectionLength > 0
        BtnCRC32.Enabled = HexBox1.SelectionLength > 0
    End Sub

    Private Sub BtnSelectAll_Click(sender As Object, e As EventArgs) Handles BtnSelectAll.Click
        HexBox1.SelectAll()
    End Sub

    Private Sub BtnCopyText_Click(sender As Object, e As EventArgs) Handles BtnCopyText.Click
        HexBox1.Copy()
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

    Private Sub BtnCRC32_Click(sender As Object, e As EventArgs) Handles BtnCRC32.Click
        Dim OffsetStart As UInteger = HexBox1.SelectionStart + HexBox1.LineInfoOffset
        Dim Length As UInteger = HexBox1.SelectionLength

        Dim B = _Disk.GetBytes(OffsetStart, Length)
        Dim Checksum = Crc32.ComputeChecksum(B).ToString("X8")
        Dim Range = OffsetStart.ToString("X8") & "-" & (OffsetStart + Length - 1).ToString("X8")
        Dim Msg As String = "The CRC32 for " & Range & " is: " & Checksum
        MsgBox(Msg, MsgBoxStyle.Information)
    End Sub
End Class
Public Class HexViewHighlightRegion
    Public ReadOnly Property Start As Long
    Public ReadOnly Property Size As Long
    Public ReadOnly Property ForeColor As Color
    Public ReadOnly Property BackColor As Color
    Public Sub New(Start As Long, Size As Long, ForeColor As Color, BackColor As Color)
        _Start = Start
        _Size = Size
        _ForeColor = ForeColor
        _BackColor = BackColor
    End Sub
End Class

Public Class HexViewData
    Public Property HighlightedRegions As New List(Of HexViewHighlightRegion)
    Public ReadOnly Property DataBlock As DiskImage.DataBlock
    Public Sub New(DataBlock As DiskImage.DataBlock)
        _DataBlock = DataBlock
    End Sub
End Class