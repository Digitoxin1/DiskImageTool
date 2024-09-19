Imports System.ComponentModel
Imports System.Globalization
Imports DiskImageTool.DiskImage

Public Class FATEditForm
    Const ValidHexChars = "0123456789ABCDEF"
    Private ReadOnly _Disk As DiskImage.Disk
    Private ReadOnly _FATTable As DataTable
    Private ReadOnly _FATTables As FATTables
    Private ReadOnly _GridCellColors() As Color = {
        Color.White,
        Color.Red,
        Color.Gray,
        Color.Blue,
        Color.Green
    }

    Private ReadOnly _ToolTip As ToolTip
    Private _CurrentFileIndex As UInteger = 0
    Private _GridCells() As GridCellType
    Private _GridSize As Integer
    Private _IgnoreEvents As Boolean = True
    Private _OffsetLookup As Dictionary(Of UInteger, UInteger)
    Private _Updated As Boolean = False
    Private Enum GridCellType
        Free
        Bad
        Reserved
        Allocated
        Highlight
    End Enum

    Public Sub New(Disk As DiskImage.Disk, Index As UShort)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        _ToolTip = New ToolTip()
        _Disk = Disk

        _FATTables = New FATTables(_Disk.BPB, _Disk.Data, Index)
        ProcessFATChains()

        Me.Text = "File Allocation Table " & Index + 1

        Dim SyncFATS = Not IsDiskTypeXDF(_Disk.DiskType) AndAlso Disk.FATTables.FATsMatch
        Dim DisplaySync = Not IsDiskTypeXDF(_Disk.DiskType) AndAlso Not Disk.FATTables.FATsMatch

        ChkSync.Checked = SyncFATS
        ChkSync.Visible = DisplaySync
        If Not DisplaySync Then
            DataGridViewFAT.Height = ChkSync.Bottom - DataGridViewFAT.Top
        End If
        SetButtonStatus(False)

        PopulateContextMenu()
        InitializeGridColumns()

        _FATTable = GetDataTable(_FATTables.FAT)
        ReDim _GridCells(_FATTable.Rows.Count - 1)
        Dim TableLength = _FATTables.FAT.GetFATTableLength()
        _GridSize = GetGridSize(TableLength)
        LblValid.Text = "Valid Clusters: 2 - " & TableLength

        DataGridViewFAT.DataSource = _FATTable

        PopulateMediaDescriptors()
        ControlSetValue(CboMediaDescriptor, _FATTables.FAT.MediaDescriptor.ToString("X2"), Array.ConvertAll(FAT12.ValidMediaDescriptor, Function(x) x.ToString("X2")), True)

        _IgnoreEvents = False

        If DataGridViewFAT.Rows.Count > 0 Then
            DataGridViewFAT.CurrentCell = DataGridViewFAT.Rows(0).Cells(0)
        End If
    End Sub

    Public ReadOnly Property Updated As Boolean
        Get
            Return _Updated
        End Get
    End Property

    Private Sub ApplyUpdates()
        Dim SyncFATs = ChkSync.Checked
        Dim Result As Boolean

        For Each Row As DataGridViewRow In DataGridViewFAT.Rows
            Dim Cluster As UShort = Row.Cells("GridCluster").Value
            Dim Value As UShort = Row.Cells("GridValue").Value
            If _Disk.FATTables.FAT(_FATTables.FATIndex).TableEntry(Cluster) <> Value Then
                _FATTables.UpdateTableEntry(Cluster, Value, SyncFATs)
            End If
        Next

        Dim MediaDescriptor = _FATTables.FAT.MediaDescriptor
        If UShort.TryParse(CboMediaDescriptor.Text, NumberStyles.HexNumber, CultureInfo.CurrentCulture, Result) Then
            MediaDescriptor = Convert.ToByte(CboMediaDescriptor.Text, 16)
        End If

        Result = _FATTables.UpdateFAT12(SyncFATs, MediaDescriptor)

        If Result Then
            _Updated = True
        End If
    End Sub

    Private Function GetDataTable(FAT As FAT12) As DataTable
        Dim FATTable = New DataTable("FATTable")
        Dim Column As DataColumn

        Column = New DataColumn("Cluster", Type.GetType("System.UInt16")) With {
            .ReadOnly = True
        }
        FATTable.Columns.Add(Column)

        Column = New DataColumn("Type", Type.GetType("System.String"))
        FATTable.Columns.Add(Column)

        Column = New DataColumn("Value", Type.GetType("System.UInt16"))
        FATTable.Columns.Add(Column)

        Column = New DataColumn("Error", Type.GetType("System.String"))
        FATTable.Columns.Add(Column)

        Column = New DataColumn("File", Type.GetType("System.String"))
        FATTable.Columns.Add(Column)

        Column = New DataColumn("FileIndex", Type.GetType("System.UInt16"))
        FATTable.Columns.Add(Column)

        'Column = New DataColumn("StartingCluster", Type.GetType("System.Boolean"))
        'FATTable.Columns.Add(Column)

        _OffsetLookup = New Dictionary(Of UInteger, UInteger)
        Dim OffsetIndex As UInteger = 1

        For Cluster = 2 To FAT.GetFATTableLength
            Dim OffsetList = GetOffsetsFromCluster(FAT, Cluster)
            Dim Value = FAT.TableEntry(Cluster)
            Dim Row = FATTable.NewRow
            Row.Item("Cluster") = Cluster
            Row.Item("Value") = Value
            Row.Item("Type") = GetTypeFromValue(Value)
            Row.Item("File") = GetFileFromOffsetList(OffsetList)
            Row.Item("Error") = GetRowError(FAT, Cluster, Value)
            If OffsetList IsNot Nothing Then
                'Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(OffsetList(0))
                'Row.Item("StartingCluster") = (DirectoryEntry.StartingCluster = Cluster)
                If Not _OffsetLookup.ContainsKey(OffsetList(0)) Then
                    _OffsetLookup.Add(OffsetList(0), OffsetIndex)
                    Row.Item("FileIndex") = OffsetIndex
                    OffsetIndex += 1
                Else
                    Row.Item("FileIndex") = _OffsetLookup.Item(OffsetList(0))
                End If
            Else
                Row.Item("FileIndex") = 0
                'Row.Item("StartingCluster") = False
            End If
            FATTable.Rows.Add(Row)
        Next

        Return FATTable
    End Function

    Private Function GetFileFromOffsetList(OffsetList As List(Of UInteger)) As String
        Dim FileName As String = ""

        If OffsetList IsNot Nothing Then
            For Each Offset In OffsetList
                Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(Offset)
                If FileName <> "" Then
                    FileName &= ", "
                End If
                FileName &= DirectoryEntry.GetFullFileName()
            Next
        End If

        Return FileName
    End Function

    Private Function GetGridCellType(Row As DataRow) As GridCellType
        Dim Value As UShort = Row.Item("Value")
        Dim FileIndex As UInteger = Row.Item("FileIndex")
        'Dim StartingCluster As Boolean = Row.Item("StartingCluster")
        Dim HasError As Boolean = (Row.Item("Error") <> "")

        If HasError Then
            Return GridCellType.Bad
        ElseIf Value = FAT12.FAT_FREE_CLUSTER Then
            Return GridCellType.Free
        ElseIf Value = 1 Or (Value >= FAT12.FAT_RESERVED_START And Value <= FAT12.FAT_RESERVED_END) Then
            Return GridCellType.Reserved
        Else
            'Brush = BrushAllocated(FileIndex Mod _ColorArray.Length)
            If _CurrentFileIndex = FileIndex And FileIndex > 0 Then
                'If StartingCluster Then
                'Brush = BrushHighlightStart
                'Else
                Return GridCellType.Highlight
                'End If
            Else
                Return GridCellType.Allocated
            End If
        End If
    End Function

    Private Function GetGridSize(Count As Integer) As Integer
        Dim Size As Integer = 3
        Dim GridWidth As Integer = Int(PictureBoxFAT.Width / Size)
        Dim GridHeight As Integer = Int(PictureBoxFAT.Height / Size)
        Dim RequiredHeight As Integer = Math.Ceiling(Count / GridWidth)

        Do While RequiredHeight < GridHeight
            Size += 1
            GridWidth = Int(PictureBoxFAT.Width / Size)
            GridHeight = Int(PictureBoxFAT.Height / Size)
            RequiredHeight = Math.Ceiling(Count / GridWidth)
        Loop

        If RequiredHeight > GridHeight Then
            Size -= 1
        End If

        Return Size
    End Function

    Private Function GetOffsetsFromCluster(FAT As FAT12, Cluster As UShort) As List(Of UInteger)
        If FAT.FileAllocation.ContainsKey(Cluster) Then
            Return FAT.FileAllocation.Item(Cluster)
        Else
            Return Nothing
        End If
    End Function

    Private Function GetRowError(FAT As FAT12, Cluster As UShort, Value As UShort) As String
        Dim FileAllocation As List(Of UInteger) = Nothing

        If FAT.FileAllocation.ContainsKey(Cluster) Then
            FileAllocation = FAT.FileAllocation.Item(Cluster)
        End If

        If Value = FAT12.FAT_BAD_CLUSTER Then
            Return "Bad Sector"
        ElseIf Value = FAT12.FAT_FREE_CLUSTER Then
            If FileAllocation Is Nothing Then
                Return ""
            Else
                Return "Invalid Allocation"
            End If
        ElseIf Value = 1 Or (Value >= FAT12.FAT_RESERVED_START And Value <= FAT12.FAT_RESERVED_END) Then
            If FileAllocation Is Nothing Then
                Return ""
            Else
                Return "Invalid Allocation"
            End If
        ElseIf Value >= FAT12.FAT_LAST_CLUSTER_START And Value <= FAT12.FAT_LAST_CLUSTER_END Then
            If FileAllocation Is Nothing Then
                Return "Lost Cluster"
            ElseIf FileAllocation.Count > 1 Then
                Return "Cross-Linked"
            Else
                Return ""
            End If
        ElseIf Value > FAT.GetFATTableLength Then
            Return "Invalid Cluster"
        ElseIf FileAllocation Is Nothing Then
            Return "Lost Cluster"
        ElseIf FileAllocation.Count > 1 Then
            Return "Cross-Linked"
        ElseIf FAT.CircularChains.Contains(Cluster) Then
            Return "Circular Chain"
        Else
            Return ""
        End If
    End Function

    Private Function GetSelectedFileInddex() As UInteger
        Dim FileIndex As UInteger = 0

        If DataGridViewFAT.CurrentRow IsNot Nothing Then
            Dim RowIndex = DataGridViewFAT.CurrentRow.Index
            If RowIndex >= 0 Then
                FileIndex = DataGridViewFAT.Rows(RowIndex).Cells("GridFileIndex").Value
            End If
        End If

        Return FileIndex
    End Function

    Private Function GetTypeFromValue(Value As UShort) As String
        If Value = FAT12.FAT_FREE_CLUSTER Then
            Return "Free"
        ElseIf Value = FAT12.FAT_BAD_CLUSTER Then
            Return "Bad"
        ElseIf Value >= FAT12.FAT_LAST_CLUSTER_START And Value <= FAT12.FAT_LAST_CLUSTER_END Then
            Return "Last"
        ElseIf Value = 1 Or (Value >= FAT12.FAT_RESERVED_START And Value <= FAT12.FAT_RESERVED_END) Then
            Return "Reserved"
        Else
            Return "Next"
        End If
    End Function

    Private Function GetValueForeColor(RowIndex As Integer) As Color
        Dim Row = DataGridViewFAT.Rows(RowIndex)
        Dim Value As UShort = Row.Cells("GridValue").Value
        Dim ErrorString As String = Row.Cells("GridError").Value

        Dim Style = New DataGridViewCellStyle

        Dim TypeName = GetTypeFromValue(Value)
        If ErrorString <> "" Then
            Return Color.Red
        ElseIf TypeName = "Free" Then
            Return Color.Gray
        ElseIf TypeName = "Bad" Then
            Return Color.Red
        ElseIf TypeName = "Last" Then
            Return Color.Black
        ElseIf TypeName = "Reserved" Then
            Return Color.Orange
        ElseIf Value > _FATTables.FAT.GetFATTableLength Then
            Return Color.Red
        Else
            Return Color.Blue
        End If
    End Function

    Private Function GridGetIndex(e As MouseEventArgs) As Integer
        Dim Index As Integer = -1

        Dim Length = _FATTables.FAT.GetFATTableLength - 2
        Dim LeftPos As Integer = Int(e.X / _GridSize)
        Dim TopPos As Integer = Int(e.Y / _GridSize)
        Dim MaxWidth As Integer = Int(PictureBoxFAT.Width / _GridSize)
        Dim MaxHeight As Integer = Math.Ceiling(Length / MaxWidth)

        If LeftPos < MaxWidth And TopPos < MaxHeight Then
            Index = TopPos * MaxWidth + LeftPos
            If Index < 0 Or Index > Length Then
                Index = -1
            End If
        End If

        Return Index
    End Function

    Private Function GridGetRowIndexFromIndex(Index As Integer) As Integer
        Dim RowIndex As Integer = -1

        If Index > -1 Then
            For Each Row As DataGridViewRow In DataGridViewFAT.Rows
                If Row.Cells("GridCluster").Value = Index + 2 Then
                    RowIndex = Row.Index
                    Exit For
                End If
            Next
        End If

        Return RowIndex
    End Function

    Private Sub HexDisplayDiskImage(Cluster As UShort)
        Dim HexViewSectorData = New HexViewSectorData(_Disk, 0, _Disk.Data.Length) With {
            .Description = "Disk"
        }

        If DisplayHexViewForm(HexViewSectorData, True, True, False, Cluster) Then
            _Updated = True
        End If
    End Sub

    Private Sub InitializeGridColumns()
        Dim GridViewColumn As DataGridViewColumn

        DataGridViewFAT.Columns.Clear()

        GridViewColumn = New DataGridViewTextBoxColumn With {
            .Name = "GridCluster",
            .HeaderText = "Cluster",
            .ReadOnly = True,
            .DataPropertyName = "Cluster",
            .Width = 65,
            .DefaultCellStyle = New DataGridViewCellStyle()
        }
        GridViewColumn.DefaultCellStyle.SelectionForeColor = Color.Black
        GridViewColumn.DefaultCellStyle.SelectionBackColor = Color.White
        DataGridViewFAT.Columns.Add(GridViewColumn)

        GridViewColumn = New DataGridViewTextBoxColumn With {
            .Name = "GridType",
            .HeaderText = "Type",
            .ReadOnly = True,
            .Resizable = False,
            .SortMode = DataGridViewColumnSortMode.NotSortable,
            .DataPropertyName = "Type",
            .Width = 75,
            .DefaultCellStyle = New DataGridViewCellStyle()
        }
        GridViewColumn.DefaultCellStyle.SelectionForeColor = Color.Black
        GridViewColumn.DefaultCellStyle.SelectionBackColor = Color.White
        DataGridViewFAT.Columns.Add(GridViewColumn)

        GridViewColumn = New DataGridViewTextBoxColumn With {
           .Name = "GridValue",
           .HeaderText = "Value",
           .SortMode = DataGridViewColumnSortMode.NotSortable,
           .DataPropertyName = "Value",
           .Width = 60
       }
        DataGridViewFAT.Columns.Add(GridViewColumn)

        GridViewColumn = New DataGridViewTextBoxColumn With {
            .Name = "GridError",
            .HeaderText = "Error",
            .ReadOnly = True,
            .Resizable = False,
            .SortMode = DataGridViewColumnSortMode.NotSortable,
            .DataPropertyName = "Error",
            .Width = 100,
            .DefaultCellStyle = New DataGridViewCellStyle()
        }
        GridViewColumn.DefaultCellStyle.ForeColor = Color.Red
        GridViewColumn.DefaultCellStyle.SelectionForeColor = Color.Red
        GridViewColumn.DefaultCellStyle.SelectionBackColor = Color.White
        DataGridViewFAT.Columns.Add(GridViewColumn)

        GridViewColumn = New DataGridViewTextBoxColumn With {
            .Name = "GridFile",
            .HeaderText = "File",
            .ReadOnly = True,
            .DataPropertyName = "File",
            .Width = 200,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            .DefaultCellStyle = New DataGridViewCellStyle()
        }
        GridViewColumn.DefaultCellStyle.SelectionForeColor = Color.Black
        GridViewColumn.DefaultCellStyle.SelectionBackColor = Color.White
        DataGridViewFAT.Columns.Add(GridViewColumn)

        GridViewColumn = New DataGridViewTextBoxColumn With {
            .Name = "GridFileIndex",
            .HeaderText = "FileIndex",
            .Visible = False,
            .DataPropertyName = "FileIndex"
        }
        DataGridViewFAT.Columns.Add(GridViewColumn)
    End Sub

    Private Sub PopulateContextMenu()
        Dim Item As ToolStripMenuItem
        Dim SubItem As ToolStripMenuItem

        Dim Label = New ToolStripLabel("Cluster:  0") With {
            .Name = "lblCluster"
        }
        ContextMenuGrid.Items.Add(Label)
        ContextMenuGrid.Items.Add(New ToolStripSeparator)

        Item = ContextMenuGrid.Items.Add("&Free")
        Item.Tag = FAT12.FAT_FREE_CLUSTER

        Item = ContextMenuGrid.Items.Add("&Bad")
        Item.Tag = FAT12.FAT_BAD_CLUSTER

        Item = ContextMenuGrid.Items.Add("&Last")
        Item.DropDown = ContextMenuLast
        SubItem = ContextMenuLast.Items.Add(FAT12.FAT_LAST_CLUSTER_END)
        SubItem.Tag = FAT12.FAT_LAST_CLUSTER_END
        ContextMenuLast.Items.Add("-")
        For Counter = FAT12.FAT_LAST_CLUSTER_START To FAT12.FAT_LAST_CLUSTER_END - 1
            SubItem = ContextMenuLast.Items.Add(Counter)
            SubItem.Tag = Counter
        Next

        Item = ContextMenuGrid.Items.Add("&Reserved")
        Item.DropDown = ContextMenuReserved
        For Counter = FAT12.FAT_RESERVED_START To FAT12.FAT_RESERVED_END
            SubItem = ContextMenuReserved.Items.Add(Counter)
            SubItem.Tag = Counter
        Next
        SubItem = ContextMenuReserved.Items.Add(1)
        SubItem.Tag = 1

        ContextMenuGrid.Items.Add(New ToolStripSeparator)

        Item = ContextMenuGrid.Items.Add("&View in Hex Editor")
        Item.Tag = -1
    End Sub

    Private Sub PopulateMediaDescriptors()
        CboMediaDescriptor.Items.Clear()
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("FE", "160K"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("FC", "180K"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("FF", "320K"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("FD", "360K"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F9", "720K"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F9", "1.2M"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F0", "1.44M"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F0", "2.88M"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F0", "DMF"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F9", "XDF"))
    End Sub

    Private Sub ProcessFATChains()
        Dim Directory = New RootDirectory(_Disk.Data, _Disk.BPB, _FATTables, _Disk.DirectoryCache, True)
    End Sub

    Private Sub RefreshFAT()
        _FATTables.FAT.ProcessFAT12()
        ProcessFATChains()

        RefreshGrid()
        PictureBoxFAT.Refresh()
    End Sub

    Private Sub RefreshGrid()
        _IgnoreEvents = True

        _OffsetLookup.Clear()
        Dim OffsetIndex As UInteger = 1
        For Each Row As DataGridViewRow In DataGridViewFAT.Rows
            Dim Cluster As UShort = Row.Cells("GridCluster").Value
            Dim OffsetList = GetOffsetsFromCluster(_FATTables.FAT, Cluster)
            Dim Value As UShort = Row.Cells("GridValue").Value
            Row.Cells("GridFile").Value = GetFileFromOffsetList(OffsetList)
            Row.Cells("GridError").Value = GetRowError(_FATTables.FAT, Cluster, Value)
            If OffsetList IsNot Nothing Then
                If Not _OffsetLookup.ContainsKey(OffsetList(0)) Then
                    'Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(OffsetList(0))
                    'Row.Cells("GridStartingCluster").Value = (DirectoryEntry.StartingCluster = Cluster)
                    _OffsetLookup.Add(OffsetList(0), OffsetIndex)
                    Row.Cells("GridFileIndex").Value = OffsetIndex
                    OffsetIndex += 1
                Else
                    Row.Cells("GridFileIndex").Value = _OffsetLookup.Item(OffsetList(0))
                    'Row.Cells("GridStartingCluster").Value = False
                End If
            Else
                Row.Cells("GridFileIndex").Value = 0
            End If
            DataGridViewFAT.InvalidateCell(2, Row.Index)
        Next

        _IgnoreEvents = False
    End Sub

    Private Sub SetButtonStatus(Enabled As Boolean)
        For Each Control In FlowLayoutPanelTop.Controls.OfType(Of Button)
            Control.Enabled = Enabled
        Next
    End Sub

    Private Sub SetCurrentCellValue(Value As UShort)
        If DataGridViewFAT.CurrentCellAddress.Y >= 0 Then
            DataGridViewFAT.Rows(DataGridViewFAT.CurrentCellAddress.Y).Cells("GridValue").Value = Value
        End If
    End Sub

    Private Sub SetCurrentFileIndex(FileIndex As UInteger)
        If _CurrentFileIndex <> FileIndex Then
            _CurrentFileIndex = FileIndex
            PictureBoxFAT.Refresh()
        End If
    End Sub

    Private Function SetTypeFromValue(RowIndex As Integer) As Boolean
        Dim Changed As Boolean = False

        _IgnoreEvents = True

        Dim Row = DataGridViewFAT.Rows(RowIndex)
        Dim Value As UShort = Row.Cells.Item("GridValue").Value
        Dim TypeName = GetTypeFromValue(Value)

        If Row.Cells.Item("GridType").Value <> TypeName Then
            Row.Cells.Item("GridType").Value = TypeName
            Changed = True
        End If

        _IgnoreEvents = False

        Return Changed
    End Function

    Private Sub UpdateFAT(RowIndex As Integer)
        Dim Row = DataGridViewFAT.Rows(RowIndex)
        Dim Cluster As UShort = Row.Cells("GridCluster").Value
        Dim Value As UShort = Row.Cells("GridValue").Value

        _FATTables.FAT.TableEntry(Cluster) = Value
        RefreshFAT()
    End Sub

    Private Sub UpdateTag(Control As Control)
        ControlSetLastValue(Control, Control.Text)

        ControlUpdateBackColor(Control)
        ControlUpdateColor(Control)
    End Sub
#Region "Events"
    Private Sub BtnBad_Click(sender As Object, e As EventArgs) Handles BtnBad.Click
        SetCurrentCellValue(FAT12.FAT_BAD_CLUSTER)
    End Sub

    Private Sub BtnFree_Click(sender As Object, e As EventArgs) Handles BtnFree.Click
        SetCurrentCellValue(FAT12.FAT_FREE_CLUSTER)
    End Sub

    Private Sub BtnLast_Click(sender As Object, e As EventArgs) Handles BtnLast.Click
        Dim Button = DirectCast(sender, Button)
        ContextMenuLast.Show(Button, 0, Button.Height)
    End Sub

    Private Sub BtnReserved_Click(sender As Object, e As EventArgs) Handles BtnReserved.Click
        Dim Button = DirectCast(sender, Button)
        ContextMenuReserved.Show(Button, 0, Button.Height)
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        ApplyUpdates()
    End Sub

    Private Sub CboMediaDescriptor_DrawItem(sender As Object, e As DrawItemEventArgs) Handles CboMediaDescriptor.DrawItem
        Dim CB As ComboBox = sender

        e.DrawBackground()

        If e.Index >= 0 Then
            Dim Item As MediaDescriptorType = CB.Items(e.Index)

            Dim Brush As Brush
            Dim tBrush As Brush
            Dim nBrush As Brush

            If e.State And DrawItemState.Selected Then
                Brush = SystemBrushes.Highlight
                tBrush = SystemBrushes.HighlightText
                nBrush = SystemBrushes.HighlightText
            Else
                Brush = SystemBrushes.Window
                tBrush = SystemBrushes.WindowText
                nBrush = Brushes.Blue
            End If

            e.Graphics.FillRectangle(Brush, e.Bounds)
            Dim r1 As Rectangle = e.Bounds

            Dim Width = TextRenderer.MeasureText(Item.MediaDescriptor, e.Font).Width + 2
            r1.Width -= Width
            Dim r2 As Rectangle = e.Bounds
            r2.X = r2.Width - Width
            e.Graphics.DrawString(Item.MediaDescriptor, e.Font, nBrush, r2, StringFormat.GenericDefault)

            e.Graphics.DrawString(Item.Description, e.Font, tBrush, r1, StringFormat.GenericDefault)
        End If

        e.DrawFocusRectangle()
    End Sub

    Private Sub CboMediaDescriptor_KeyPress(sender As Object, e As KeyPressEventArgs) Handles CboMediaDescriptor.KeyPress
        e.KeyChar = UCase(e.KeyChar)
        If Not ValidHexChars.Contains(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub CboMediaDescriptor_LostFocus(sender As Object, e As EventArgs) Handles CboMediaDescriptor.LostFocus
        Dim CB As ComboBox = sender
        Dim Result As UShort

        If Not UShort.TryParse(CB.Text, NumberStyles.HexNumber, CultureInfo.CurrentCulture, Result) Then
            ControlRevertValue(CB)
        Else
            _IgnoreEvents = True
            If CB.Text.Length = 1 Then
                CB.Text = "0" & CB.Text
            End If
            If CB.Text <> CType(CB.Tag, FormControlData).LastValue Then
                UpdateTag(CB)
            End If
            _IgnoreEvents = False
        End If
    End Sub
    Private Sub CboMediaDescriptor_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboMediaDescriptor.SelectedIndexChanged
        If _IgnoreEvents Then
            Exit Sub
        End If

        Dim CB As ComboBox = sender
        _IgnoreEvents = True
        If CB.Text <> CType(CB.Tag, FormControlData).LastValue Then
            UpdateTag(CB)
        End If
        _IgnoreEvents = False
    End Sub

    Private Sub ContextMenuGrid_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ContextMenuGrid.ItemClicked, ContextMenuLast.ItemClicked, ContextMenuReserved.ItemClicked
        If e.ClickedItem.Tag IsNot Nothing Then
            Dim Value As Short = e.ClickedItem.Tag
            If Value = -1 Then
                Dim Cluster As UShort = DataGridViewFAT.CurrentRow.Cells("GridCluster").Value
                HexDisplayDiskImage(Cluster)
            Else
                SetCurrentCellValue(Value)
            End If
        End If
    End Sub

    Private Sub ContextMenuGrid_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuGrid.Opening
        If DataGridViewFAT.CurrentCellAddress.Y < 0 Then
            e.Cancel = True
        Else
            Dim Cluster As UShort = DataGridViewFAT.CurrentRow.Cells("GridCluster").Value
            ContextMenuGrid.Items("lblCluster").Text = "Cluster:  " & Cluster
        End If
    End Sub

    Private Sub DataGridViewFAT_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridViewFAT.CellFormatting
        If e.RowIndex >= 0 Then
            If e.ColumnIndex = 2 Then
                e.CellStyle.ForeColor = GetValueForeColor(e.RowIndex)
            End If
        End If
    End Sub

    Private Sub DataGridViewFAT_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles DataGridViewFAT.CellValidating
        If e.ColumnIndex = 2 Then
            Dim i As Integer
            If Not Integer.TryParse(e.FormattedValue, i) Then
                e.Cancel = True
                DataGridViewFAT.CancelEdit()
            ElseIf i < 0 Or i > 4095 Then
                e.Cancel = True
                DataGridViewFAT.CancelEdit()
            End If
        End If
    End Sub

    Private Sub DataGridViewFAT_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewFAT.CellValueChanged
        If _IgnoreEvents Then
            Exit Sub
        End If

        If e.RowIndex >= 0 Then
            If e.ColumnIndex = 2 Then
                SetTypeFromValue(e.RowIndex)
                UpdateFAT(e.RowIndex)
            End If
        End If
    End Sub

    Private Sub DataGridViewFAT_MouseDown(sender As Object, e As MouseEventArgs) Handles DataGridViewFAT.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim htinfo As DataGridView.HitTestInfo = DataGridViewFAT.HitTest(e.X, e.Y)
            If htinfo.Type = DataGridViewHitTestType.Cell Then
                Dim Cell = DataGridViewFAT.Item(htinfo.ColumnIndex, htinfo.RowIndex)
                DataGridViewFAT.CurrentCell = Cell
            End If
        End If
    End Sub


    Private Sub DataGridViewFAT_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewFAT.RowEnter
        Dim Enabled As Boolean
        Dim Fileindex As UInteger

        Enabled = e.RowIndex >= 0
        SetButtonStatus(Enabled)

        If Enabled Then
            Fileindex = DataGridViewFAT.Rows(e.RowIndex).Cells("GridFileIndex").Value
        Else
            Fileindex = 0
        End If
        SetCurrentFileIndex(Fileindex)
    End Sub


    Private Sub DataGridViewFAT_SortCompare(sender As Object, e As DataGridViewSortCompareEventArgs) Handles DataGridViewFAT.SortCompare
        If e.Column.Index = 4 Then
            e.SortResult = System.String.Compare(e.CellValue1.ToString, e.CellValue2.ToString)
            If e.SortResult = 0 Then
                Dim Value1 As Integer = DataGridViewFAT.Rows(e.RowIndex1).Cells(0).Value
                Dim Value2 As Integer = DataGridViewFAT.Rows(e.RowIndex2).Cells(0).Value
                e.SortResult = Value1.CompareTo(Value2)
            End If
            e.Handled = True
        End If
    End Sub

    Private Sub PictureBoxFAT_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBoxFAT.MouseDown
        Dim RowIndex = GridGetRowIndexFromIndex(GridGetIndex(e))

        If RowIndex > -1 Then
            DataGridViewFAT.FirstDisplayedScrollingRowIndex = RowIndex
            DataGridViewFAT.CurrentCell = DataGridViewFAT.Rows(RowIndex).Cells("GridValue")
        End If
    End Sub

    Private Sub PictureBoxFAT_MouseLeave(sender As Object, e As EventArgs) Handles PictureBoxFAT.MouseLeave
        SetCurrentFileIndex(GetSelectedFileInddex())
    End Sub

    Private Sub PictureBoxFAT_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBoxFAT.MouseMove
        Dim TooltipText As String = ""
        Dim FileIndex As UInteger = 0
        Dim Index = GridGetIndex(e)

        If Index > -1 Then
            Dim Row = _FATTable.Rows(Index)
            Dim Cluster As UShort = Row.Item("Cluster")
            Dim File As String = Row.Item("File")
            Dim ErrorString As String = Row.Item("Error")
            FileIndex = Row.Item("FileIndex")

            TooltipText = "Cluster: " & vbTab & Cluster
            If File <> "" Then
                TooltipText &= vbCrLf & "File: " & vbTab & File
            End If
            If ErrorString <> "" Then
                TooltipText &= vbCrLf & "Error: " & vbTab & ErrorString
            End If
        End If

        If TooltipText <> _ToolTip.GetToolTip(PictureBoxFAT) Then
            _ToolTip.SetToolTip(PictureBoxFAT, TooltipText)
        End If

        SetCurrentFileIndex(FileIndex)
    End Sub

    Private Sub PictureBoxFAT_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBoxFAT.MouseUp
        If e.Button And MouseButtons.Right Then
            Dim RowIndex = GridGetRowIndexFromIndex(GridGetIndex(e))
            If RowIndex > -1 Then
                ContextMenuGrid.Show(MousePosition)
            End If
        End If
    End Sub
    Private Sub PictureBoxFAT_Paint(sender As Object, e As PaintEventArgs) Handles PictureBoxFAT.Paint
        e.Graphics.Clear(PictureBoxFAT.BackColor)

        If _FATTable IsNot Nothing Then
            Dim Width = PictureBoxFAT.Width
            Dim Size As Integer = _GridSize

            Dim Pen As New Pen(Color.FromArgb(160, 160, 160))
            Dim Brushes(_GridCellColors.Length - 1) As SolidBrush
            For Counter = 0 To _GridCellColors.Length - 1
                Brushes(Counter) = New SolidBrush(_GridCellColors(Counter))
            Next

            Dim Brush As Brush
            Dim Left As Integer = 0
            Dim Top As Integer = 0
            Dim CellType As GridCellType
            Dim RowIndex As Integer = 0
            For Each Row As DataRow In _FATTable.Rows
                CellType = GetGridCellType(Row)
                _GridCells(RowIndex) = CellType
                Brush = Brushes(CellType)
                e.Graphics.FillRectangle(Brush, Left, Top, Size, Size)
                e.Graphics.DrawRectangle(Pen, Left, Top, Size, Size)
                Left += Size
                If Left + Size > Width Then
                    Left = 0
                    Top += Size
                End If
                RowIndex += 1
            Next
            Pen.Dispose()
            For Counter = 0 To _GridCellColors.Length - 1
                Brushes(Counter).Dispose()
            Next
        End If
    End Sub

    Private Sub PictureBoxFAT_Resize(sender As Object, e As EventArgs) Handles PictureBoxFAT.Resize
        If _FATTables IsNot Nothing Then
            _GridSize = GetGridSize(_FATTables.FAT.GetFATTableLength)
            PictureBoxFAT.Refresh()
        End If
    End Sub
#End Region
End Class