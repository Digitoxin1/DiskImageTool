Imports System.ComponentModel
Imports DiskImageTool.DiskImage

Public Class FATEditForm
    Private ReadOnly _Disk As DiskImage.Disk
    Private ReadOnly _FAT As FAT12
    Private ReadOnly _FATTable As DataTable
    Private _IgnoreEvents As Boolean = True
    Private _Updated As Boolean = False

    Public Sub New(Disk As DiskImage.Disk, Index As UShort)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Me.Text = "File Allocation Table " & Index + 1
        ChkSync.Checked = True
        SetButtonStatus(False)

        PopulateContextMenu()

        _Disk = Disk

        _FAT = New FAT12(_Disk.Data, _Disk.BootSector, Index, True)
        _FATTable = GetDataTable(_FAT)

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

        _Disk.Data.BatchEditMode = True

        _Updated = _FAT.UpdateFAT12(SyncAll)

        _Disk.Data.BatchEditMode = False
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

        For Counter = 2 To FAT.TableLength - 1
            Dim Value = FAT.TableEntry(Counter)
            Dim Row = FATTable.NewRow
            Row.Item("Cluster") = Counter
            Row.Item("Value") = Value
            Row.Item("Type") = GetTypeFromValue(Value)
            Row.Item("File") = GetFileFromCluster(FAT, Counter)
            Row.Item("Error") = GetRowError(FAT, Counter, Value)
            FATTable.Rows.Add(Row)
        Next

        Return FATTable
    End Function

    Private Function GetFileFromCluster(FAT As FAT12, Cluster As UShort) As String
        Dim FileName As String = ""

        If FAT.FileAllocation.ContainsKey(Cluster) Then
            Dim OffsetList = FAT.FileAllocation.Item(Cluster)
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

    Private Function GetTypeFromValue(Value As UShort) As String
        If Value = FAT12.FAT_FREE_CLUSTER Then
            Return "Free"
        ElseIf Value = FAT12.FAT_BAD_CLUSTER Then
            Return "Bad"
        ElseIf Value >= FAT12.FAT_LAST_CLUSTER_START And Value <= FAT12.FAT_LAST_CLUSTER_END Then
            Return "Last"
        ElseIf Value = 1 Or (Value >= FAT12.FAT_LAST_RESERVED_START And Value <= FAT12.FAT_LAST_RESERVED_END) Then
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
        If TypeName = "Free" Then
            Return Color.Gray
        ElseIf TypeName = "Bad" Then
            Return Color.Red
        ElseIf TypeName = "Last" Then
            Return Color.Black
        ElseIf TypeName = "Reserved" Then
            Return Color.Orange
        ElseIf Value > _FAT.TableLength - 1 Then
            Return Color.Red
        ElseIf ErrorString <> "" Then
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
        ElseIf Value = 1 Or (Value >= FAT12.FAT_LAST_RESERVED_START And Value <= FAT12.FAT_LAST_RESERVED_END) Then
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
        For Counter = FAT12.FAT_LAST_RESERVED_START To FAT12.FAT_LAST_RESERVED_END
            SubItem = ContextMenuReserved.Items.Add(Counter)
            SubItem.Tag = Counter
        Next
        SubItem = ContextMenuReserved.Items.Add(1)
        SubItem.Tag = 1
    End Sub

    Private Sub RefreshGrid()
        _IgnoreEvents = True

        For Each Row As DataGridViewRow In DataGridViewFAT.Rows
            Dim Cluster As UShort = Row.Cells("GridCluster").Value
            Dim Value As UShort = Row.Cells("GridValue").Value
            Row.Cells("GridFile").Value = GetFileFromCluster(_FAT, Cluster)
            Row.Cells("GridError").Value = GetRowError(_FAT, Cluster, Value)
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
        If DataGridViewFAT.CurrentCellAddress.Y >= 0 And DataGridViewFAT.CurrentCellAddress.X = 2 Then
            DataGridViewFAT.CurrentCell.Value = Value
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
        _FAT.ProcessFAT12(True)

        RefreshGrid()
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
        Dim Enabled = (DataGridViewFAT.CurrentCellAddress.Y >= 0 And DataGridViewFAT.CurrentCellAddress.X = 2)
        SetButtonStatus(Enabled)
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
End Class