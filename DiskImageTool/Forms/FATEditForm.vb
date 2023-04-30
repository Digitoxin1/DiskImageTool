Imports System.ComponentModel
Imports DiskImageTool.DiskImage

Public Class FATEditForm
    Private ReadOnly _Disk As DiskImage.Disk
    Private ReadOnly _FAT As FAT12
    Private ReadOnly _FATTable As DataTable
    Private ReadOnly _ToolTip As ToolTip
    Private _IgnoreEvents As Boolean = True
    Private _Updated As Boolean = False
    Private _GridSize As Integer
    Private _OffsetLookup As Dictionary(Of UInteger, UInteger)
    Private _CurrentFileIndex As UInteger = 0
    'Private ReadOnly _ColorArray() As Color = {
    '    Color.LightBlue,
    '    Color.LightGreen,
    '    Color.Orange,
    '    Color.Pink,
    '    Color.Yellow,
    '    Color.Magenta
    '}

    Public Sub New(Disk As DiskImage.Disk, Index As UShort, DisplaySync As Boolean)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Me.Text = "File Allocation Table " & Index + 1
        ChkSync.Checked = True
        ChkSync.Visible = DisplaySync
        If Not DisplaySync Then
            DataGridViewFAT.Height = ChkSync.Bottom - DataGridViewFAT.Top
        End If
        SetButtonStatus(False)

        PopulateContextMenu()
        InitializeGridColumns()

        _ToolTip = New ToolTip()
        _Disk = Disk

        _FAT = New FAT12(_Disk.Data, _Disk.BootSector, Index)
        ProcessFATChains()

        _FATTable = GetDataTable(_FAT)

        _GridSize = GetGridSize(_FAT.TableLength - 2)
        LblValid.Text = "Valid Clusters: 2 - " & _FAT.TableLength - 1

        DataGridViewFAT.DataSource = _FATTable

        _IgnoreEvents = False
    End Sub

    Public ReadOnly Property Updated As Boolean
        Get
            Return _Updated
        End Get
    End Property

    Private Sub ApplyUpdates()
        Dim SyncAll = ChkSync.Checked

        For Each Row As DataGridViewRow In DataGridViewFAT.Rows
            Dim Cluster As UShort = Row.Cells("GridCluster").Value
            Dim Value As UShort = Row.Cells("GridValue").Value
            _FAT.TableEntry(Cluster) = Value
        Next

        _Updated = _FAT.UpdateFAT12(SyncAll)
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

        For Cluster = 2 To FAT.TableLength - 1
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

    Private Function GetOffsetsFromCluster(FAT As FAT12, Cluster As UShort) As List(Of UInteger)
        If FAT.FileAllocation.ContainsKey(Cluster) Then
            Return FAT.FileAllocation.Item(Cluster)
        Else
            Return Nothing
        End If
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

    Private Function GetSelectedFileInddex() As UInteger
        Dim FileIndex As UInteger = 0

        If DataGridViewFAT.SelectedCells.Count > 0 Then
            Dim RowIndex = DataGridViewFAT.SelectedCells(0).RowIndex
            If RowIndex >= 0 And DataGridViewFAT.SelectedCells(0).ColumnIndex = 2 Then
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
        ElseIf Value > _FAT.TableLength - 1 Then
            Return Color.Red
        Else
            Return Color.Blue
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
        ElseIf Value > _FAT.TableLength - 1 Then
            Return "Invalid Cluster"
        ElseIf FileAllocation Is Nothing Then
            Return "Lost Cluster"
        ElseIf FileAllocation.Count > 1 Then
            Return "Cross-Linked"
        ElseIf _FAT.CircularChains.Contains(Cluster) Then
            Return "Circular Chain"
        Else
            Return ""
        End If
    End Function

    Private Sub PopulateContextMenu()
        Dim Item As ToolStripMenuItem
        Dim SubItem As ToolStripMenuItem

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
    End Sub

    Private Sub RefreshGrid()
        _IgnoreEvents = True

        _OffsetLookup.Clear()
        Dim OffsetIndex As UInteger = 1
        For Each Row As DataGridViewRow In DataGridViewFAT.Rows
            Dim Cluster As UShort = Row.Cells("GridCluster").Value
            Dim OffsetList = GetOffsetsFromCluster(_FAT, Cluster)
            Dim Value As UShort = Row.Cells("GridValue").Value
            Row.Cells("GridFile").Value = GetFileFromOffsetList(OffsetList)
            Row.Cells("GridError").Value = GetRowError(_FAT, Cluster, Value)
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

    Private Function GridGetIndex(e As MouseEventArgs) As Integer
        Dim Index As Integer = -1

        Dim Length = _FAT.TableLength - 2
        Dim LeftPos As Integer = Int(e.X / _GridSize)
        Dim TopPos As Integer = Int(e.Y / _GridSize)
        Dim MaxWidth As Integer = Int(PictureBoxFAT.Width / _GridSize)
        Dim MaxHeight As Integer = Math.Ceiling(Length / MaxWidth)

        If LeftPos < MaxWidth And TopPos < MaxHeight Then
            Index = TopPos * MaxWidth + LeftPos
            If Index < 0 Or Index >= Length Then
                Index = -1
            End If
        End If

        Return Index
    End Function

    Private Sub ProcessFATChains()
        Dim Directory = New RootDirectory(_Disk.Data, _Disk.BootSector, _FAT, True)
    End Sub

    Private Sub SetButtonStatus(Enabled As Boolean)
        For Each Control In FlowLayoutPanelTop.Controls.OfType(Of Button)
            Control.Enabled = Enabled
        Next
    End Sub

    Private Sub SetCurrentCellValue(Value As UShort)
        If DataGridViewFAT.CurrentCellAddress.Y >= 0 And DataGridViewFAT.CurrentCellAddress.X = 2 Then
            DataGridViewFAT.CurrentCell.Value = Value
        End If
    End Sub

    Private Sub SetCurrentFileIndex(FileIndex As UInteger)
        If _CurrentFileIndex <> FileIndex Then
            _CurrentFileIndex = FileIndex
            PictureBoxFAT.Invalidate()
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

        _FAT.TableEntry(Cluster) = Value
        _FAT.ProcessFAT12()

        RefreshGrid()
        PictureBoxFAT.Invalidate()
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

    Private Sub DataGridViewFAT_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridViewFAT.CellFormatting
        If e.RowIndex >= 0 Then
            If e.ColumnIndex = 2 Then
                e.CellStyle.ForeColor = GetValueForeColor(e.RowIndex)
            End If
        End If
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        ApplyUpdates()
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

    Private Sub ContextMenuGrid_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuGrid.Opening
        If DataGridViewFAT.CurrentCellAddress.Y < 0 Or DataGridViewFAT.CurrentCellAddress.X <> 2 Then
            e.Cancel = True
        End If
    End Sub

    Private Sub ContextMenuGrid_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ContextMenuGrid.ItemClicked, ContextMenuLast.ItemClicked, ContextMenuReserved.ItemClicked
        If e.ClickedItem.Tag IsNot Nothing Then
            Dim Value As UShort = e.ClickedItem.Tag
            SetCurrentCellValue(Value)
        End If
    End Sub

    Private Sub DataGridViewFAT_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridViewFAT.SelectionChanged
        Dim Enabled As Boolean

        If DataGridViewFAT.SelectedCells.Count > 0 Then
            Enabled = (DataGridViewFAT.SelectedCells(0).RowIndex >= 0 And DataGridViewFAT.SelectedCells(0).ColumnIndex = 2)
        Else
            Enabled = False
        End If
        SetButtonStatus(Enabled)

        SetCurrentFileIndex(GetSelectedFileInddex())
    End Sub

    Private Sub BtnFree_Click(sender As Object, e As EventArgs) Handles BtnFree.Click
        SetCurrentCellValue(FAT12.FAT_FREE_CLUSTER)
    End Sub

    Private Sub BtnBad_Click(sender As Object, e As EventArgs) Handles BtnBad.Click
        SetCurrentCellValue(FAT12.FAT_BAD_CLUSTER)
    End Sub

    Private Sub BtnLast_Click(sender As Object, e As EventArgs) Handles BtnLast.Click
        Dim Button = DirectCast(sender, Button)
        ContextMenuLast.Show(Button, 0, Button.Height)
    End Sub

    Private Sub BtnReserved_Click(sender As Object, e As EventArgs) Handles BtnReserved.Click
        Dim Button = DirectCast(sender, Button)
        ContextMenuReserved.Show(Button, 0, Button.Height)
    End Sub

    Private Sub PictureBoxFAT_Paint(sender As Object, e As PaintEventArgs) Handles PictureBoxFAT.Paint
        e.Graphics.Clear(PictureBoxFAT.BackColor)

        If _FATTable IsNot Nothing Then
            Dim Width = PictureBoxFAT.Width
            Dim Size As Integer = _GridSize

            'Dim BrushAllocated(_ColorArray.Length - 1) As Brush
            'For Counter = 0 To _ColorArray.Length - 1
            '    BrushAllocated(Counter) = New SolidBrush(_ColorArray(Counter))
            'Next

            Dim Pen As New Pen(Color.FromArgb(160, 160, 160))
            Dim BrushFree As New SolidBrush(Color.White)
            Dim BrushBad As New SolidBrush(Color.Red)
            Dim BrushReserved As New SolidBrush(Color.Gray)
            Dim BrushAllocated As New SolidBrush(Color.Blue)
            Dim BrushHighlight As New SolidBrush(Color.Green)
            'Dim BrushHighlightStart As New SolidBrush(Color.LightGreen)
            Dim Brush As Brush

            Dim Left As Integer = 0
            Dim Top As Integer = 0
            For Each Row In _FATTable.Rows
                Dim Value As UShort = Row.Item("Value")
                Dim FileIndex As UInteger = Row.Item("FileIndex")
                'Dim StartingCluster As Boolean = Row.Item("StartingCluster")
                Dim HasError As Boolean = (Row.Item("Error") <> "")
                If HasError Then
                    Brush = BrushBad
                ElseIf Value = FAT12.FAT_FREE_CLUSTER Then
                    Brush = BrushFree
                ElseIf Value = 1 Or (Value >= FAT12.FAT_RESERVED_START And Value <= FAT12.FAT_RESERVED_END) Then
                    Brush = BrushReserved
                Else
                    'Brush = BrushAllocated(FileIndex Mod _ColorArray.Length)
                    If _CurrentFileIndex = FileIndex And FileIndex > 0 Then
                        'If StartingCluster Then
                        'Brush = BrushHighlightStart
                        'Else
                        Brush = BrushHighlight
                        'End If
                    Else
                        Brush = BrushAllocated
                    End If
                End If
                e.Graphics.FillRectangle(Brush, Left, Top, Size, Size)
                e.Graphics.DrawRectangle(Pen, Left, Top, Size, Size)
                Left += Size
                If Left + Size > Width Then
                    Left = 0
                    Top += Size
                End If
            Next
            Pen.Dispose()
            BrushFree.Dispose()
            BrushBad.Dispose()
            BrushReserved.Dispose()
            BrushAllocated.Dispose()
            BrushHighlight.Dispose()
            'BrushHighlightStart.Dispose()
            'For Counter = 0 To _ColorArray.Length - 1
            '    BrushAllocated(Counter).Dispose()
            'Next
        End If
    End Sub

    Private Sub PictureBoxFAT_Resize(sender As Object, e As EventArgs) Handles PictureBoxFAT.Resize
        If _FAT IsNot Nothing Then
            _GridSize = GetGridSize(_FAT.TableLength - 2)
            PictureBoxFAT.Invalidate()
        End If
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

    Private Sub PictureBoxFAT_MouseClick(sender As Object, e As MouseEventArgs) Handles PictureBoxFAT.MouseClick
        Dim Index = GridGetIndex(e)

        If Index > -1 Then
            Dim RowIndex As Integer = -1
            For Each Row As DataGridViewRow In DataGridViewFAT.Rows
                If Row.Cells("GridCluster").Value = Index + 2 Then
                    RowIndex = Row.Index
                    Exit For
                End If
            Next
            If RowIndex > -1 Then
                DataGridViewFAT.FirstDisplayedScrollingRowIndex = RowIndex
                DataGridViewFAT.CurrentCell = DataGridViewFAT.Rows(RowIndex).Cells(2)
            End If
        End If
    End Sub

    Private Sub PictureBoxFAT_MouseLeave(sender As Object, e As EventArgs) Handles PictureBoxFAT.MouseLeave
        SetCurrentFileIndex(GetSelectedFileInddex())
    End Sub
End Class