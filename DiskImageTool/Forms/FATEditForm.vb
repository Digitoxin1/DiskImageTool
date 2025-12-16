Imports System.ComponentModel
Imports System.Globalization
Imports DiskImageTool.DiskImage

Public Class FATEditForm
    Const ValidHexChars = "0123456789ABCDEF"

    Private Const GRID_COLUMN_CLUSTER As String = "GridCluster"
    Private Const GRID_COLUMN_TYPE As String = "GridType"
    Private Const GRID_COLUMN_VALUE As String = "GridValue"
    Private Const GRID_COLUMN_ERROR As String = "GridError"
    Private Const GRID_COLUMN_FILE As String = "GridFile"
    Private Const GRID_COLUMN_FILE_INDEX As String = "GridFileIndex"
    Private Const LABEL_CLUSTER As String = "lblCluster"

    Private ReadOnly _Disk As DiskImage.Disk
    Private ReadOnly _FATTable As DataTable
    Private ReadOnly _FATTables As FATTables
    Private ReadOnly _Directory As RootDirectory
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
    Private _OffsetLookup As Dictionary(Of DirectoryEntry, UInteger)
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
        LocalizeForm()

        _ToolTip = New ToolTip()
        _Disk = Disk

        _FATTables = New FATTables(_Disk.BPB, _Disk.Image, Index)
        _Directory = New RootDirectory(_Disk, _FATTables.FAT)

        Me.Text = My.Resources.Label_FAT & " " & Index + 1

        Dim SyncFATS = Not _Disk.DiskParams.IsXDF AndAlso Disk.FATTables.FATsMatch
        Dim DisplaySync = Not _Disk.DiskParams.IsXDF AndAlso Not Disk.FATTables.FATsMatch

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
        Dim TableLength = _FATTables.FAT.TableLength
        _GridSize = GetGridSize(TableLength)
        LblValid.Text = My.Resources.Caption_ValidClusters & ": 2 - " & TableLength

        DataGridViewFAT.DataSource = _FATTable

        PopulateMediaDescriptors()
        ControlSetValue(CboMediaDescriptor, _FATTables.FAT.MediaDescriptor.ToString("X2"), Array.ConvertAll(FAT12.ValidMediaDescriptor, Function(x) x.ToString("X2")), True)

        _IgnoreEvents = False

        If DataGridViewFAT.Rows.Count > 0 Then
            DataGridViewFAT.CurrentCell = DataGridViewFAT.Rows(0).Cells(0)
        End If
    End Sub

    Private Sub LocalizeForm()
        BtnCancel.Text = My.Resources.Menu_Cancel
        BtnUpdate.Text = My.Resources.Menu_Update
        BtnReserved.Text = My.Resources.Label_Reserved
        TxtMediaDescriptor.Text = My.Resources.Label_MediaDescriptor
        BtnLast.Text = My.Resources.Label_Last
        BtnFree.Text = My.Resources.Label_Free
        BtnBad.Text = My.Resources.Label_Bad
        ChkSync.Text = My.Resources.Label_SynchronizeFATs
    End Sub

    Public ReadOnly Property Updated As Boolean
        Get
            Return _Updated
        End Get
    End Property

    Public Shared Function Display(Disk As DiskImage.Disk, Index As UShort) As Boolean
        Using Form As New FATEditForm(Disk, Index)
            Form.ShowDialog()

            Return Form.Updated
        End Using
    End Function

    Private Sub ApplyUpdates()
        Dim SyncFATs = ChkSync.Checked
        Dim Result As Boolean

        For Each Row As DataGridViewRow In DataGridViewFAT.Rows
            Dim Cluster As UShort = Row.Cells(GRID_COLUMN_CLUSTER).Value
            Dim Value As UShort = Row.Cells(GRID_COLUMN_VALUE).Value
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
        Dim FATTable As New DataTable("FATTable")
        Dim Column As DataColumn

        Column = New DataColumn(GRID_COLUMN_CLUSTER, Type.GetType("System.UInt16")) With {
            .ReadOnly = True
        }
        FATTable.Columns.Add(Column)

        Column = New DataColumn(GRID_COLUMN_TYPE, Type.GetType("System.String"))
        FATTable.Columns.Add(Column)

        Column = New DataColumn(GRID_COLUMN_VALUE, Type.GetType("System.UInt16"))
        FATTable.Columns.Add(Column)

        Column = New DataColumn(GRID_COLUMN_ERROR, Type.GetType("System.String"))
        FATTable.Columns.Add(Column)

        Column = New DataColumn(GRID_COLUMN_FILE, Type.GetType("System.String"))
        FATTable.Columns.Add(Column)

        Column = New DataColumn(GRID_COLUMN_FILE_INDEX, Type.GetType("System.UInt16"))
        FATTable.Columns.Add(Column)

        'Column = New DataColumn("StartingCluster", Type.GetType("System.Boolean"))
        'FATTable.Columns.Add(Column)

        _OffsetLookup = New Dictionary(Of DirectoryEntry, UInteger)
        Dim OffsetIndex As UInteger = 1

        For Cluster = 2 To FAT.TableLength
            Dim EntreyList = GetEntriesFromCluster(FAT, Cluster)
            Dim Value = FAT.TableEntry(Cluster)
            Dim Row = FATTable.NewRow
            Row.Item(GRID_COLUMN_CLUSTER) = Cluster
            Row.Item(GRID_COLUMN_VALUE) = Value
            Row.Item(GRID_COLUMN_TYPE) = GetTypeNameFromValue(Value)
            Row.Item(GRID_COLUMN_FILE) = GetFileFromOffsetList(EntreyList)
            Row.Item(GRID_COLUMN_ERROR) = GetRowError(FAT, Cluster, Value)
            If EntreyList IsNot Nothing Then
                'Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(OffsetList(0))
                'Row.Item("StartingCluster") = (DirectoryEntry.StartingCluster = Cluster)
                If Not _OffsetLookup.ContainsKey(EntreyList(0)) Then
                    _OffsetLookup.Add(EntreyList(0), OffsetIndex)
                    Row.Item(GRID_COLUMN_FILE_INDEX) = OffsetIndex
                    OffsetIndex += 1
                Else
                    Row.Item(GRID_COLUMN_FILE_INDEX) = _OffsetLookup.Item(EntreyList(0))
                End If
            Else
                Row.Item(GRID_COLUMN_FILE_INDEX) = 0
                'Row.Item("StartingCluster") = False
            End If
            FATTable.Rows.Add(Row)
        Next

        Return FATTable
    End Function

    Private Function GetFileFromOffsetList(OffsetList As List(Of DirectoryEntry)) As String
        Dim FileName As String = ""

        If OffsetList IsNot Nothing Then
            For Each DirectoryEntry In OffsetList
                If FileName <> "" Then
                    FileName &= ", "
                End If
                FileName &= DirectoryEntry.GetShortFileName(True)
            Next
        End If

        Return FileName
    End Function

    Private Function GetGridCellType(Row As DataRow) As GridCellType
        Dim Value As UShort = Row.Item(GRID_COLUMN_VALUE)
        Dim FileIndex As UInteger = Row.Item(GRID_COLUMN_FILE_INDEX)
        'Dim StartingCluster As Boolean = Row.Item("StartingCluster")
        Dim HasError As Boolean = (Row.Item(GRID_COLUMN_ERROR) <> "")

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
        Dim RequiredHeight As Integer = CeilDiv(CUInt(Count), CUInt(GridWidth))

        Do While RequiredHeight < GridHeight
            Size += 1
            GridWidth = Int(PictureBoxFAT.Width / Size)
            GridHeight = Int(PictureBoxFAT.Height / Size)
            RequiredHeight = CeilDiv(CUInt(Count), CUInt(GridWidth))
        Loop

        If RequiredHeight > GridHeight Then
            Size -= 1
        End If

        Return Size
    End Function

    Private Function GetEntriesFromCluster(FAT As FAT12, Cluster As UShort) As List(Of DirectoryEntry)
        If _Directory.FATAllocation.FileAllocation.ContainsKey(Cluster) Then
            Return _Directory.FATAllocation.FileAllocation.Item(Cluster)
        Else
            Return Nothing
        End If
    End Function

    Private Function GetRowError(FAT As FAT12, Cluster As UShort, Value As UShort) As String
        Dim FileAllocation As List(Of DirectoryEntry) = Nothing

        If _Directory.FATAllocation.FileAllocation.ContainsKey(Cluster) Then
            FileAllocation = _Directory.FATAllocation.FileAllocation.Item(Cluster)
        End If

        If Value = FAT12.FAT_BAD_CLUSTER Then
            Return My.Resources.Label_BadSector
        ElseIf Value = FAT12.FAT_FREE_CLUSTER Then
            If FileAllocation Is Nothing Then
                Return ""
            Else
                Return My.Resources.Label_InvalidAllocation
            End If
        ElseIf Value = 1 Or (Value >= FAT12.FAT_RESERVED_START And Value <= FAT12.FAT_RESERVED_END) Then
            If FileAllocation Is Nothing Then
                Return ""
            Else
                Return My.Resources.Label_InvalidAllocation
            End If
        ElseIf Value >= FAT12.FAT_LAST_CLUSTER_START And Value <= FAT12.FAT_LAST_CLUSTER_END Then
            If FileAllocation Is Nothing Then
                Return My.Resources.Label_LostCluster
            ElseIf FileAllocation.Count > 1 Then
                Return My.Resources.Label_CrossLinked
            Else
                Return ""
            End If
        ElseIf Value > FAT.TableLength Then
            Return My.Resources.Label_InvalidCluster
        ElseIf FileAllocation Is Nothing Then
            Return My.Resources.Label_LostCluster
        ElseIf FileAllocation.Count > 1 Then
            Return My.Resources.Label_CrossLinked
        ElseIf _Directory.FATAllocation.CircularChains.Contains(Cluster) Then
            Return My.Resources.Label_CircularChain
        Else
            Return ""
        End If
    End Function

    Private Function GetSelectedFileInddex() As UInteger
        Dim FileIndex As UInteger = 0

        If DataGridViewFAT.CurrentRow IsNot Nothing Then
            Dim RowIndex = DataGridViewFAT.CurrentRow.Index
            If RowIndex >= 0 Then
                FileIndex = DataGridViewFAT.Rows(RowIndex).Cells(GRID_COLUMN_FILE_INDEX).Value
            End If
        End If

        Return FileIndex
    End Function

    Private Function GetTypeNameFromValue(Value As UShort) As String
        If Value = FAT12.FAT_FREE_CLUSTER Then
            Return My.Resources.Label_Free
        ElseIf Value = FAT12.FAT_BAD_CLUSTER Then
            Return My.Resources.Label_Bad
        ElseIf Value >= FAT12.FAT_LAST_CLUSTER_START And Value <= FAT12.FAT_LAST_CLUSTER_END Then
            Return My.Resources.Label_Last
        ElseIf Value = 1 Or (Value >= FAT12.FAT_RESERVED_START And Value <= FAT12.FAT_RESERVED_END) Then
            Return My.Resources.Label_Reserved
        Else
            Return My.Resources.Label_FATType_Next
        End If
    End Function

    Private Function GetValueForeColor(RowIndex As Integer) As Color
        Dim Row = DataGridViewFAT.Rows(RowIndex)
        Dim Value As UShort = Row.Cells(GRID_COLUMN_VALUE).Value
        Dim ErrorString As String = Row.Cells(GRID_COLUMN_ERROR).Value

        Dim Style As New DataGridViewCellStyle

        Dim TypeName = GetTypeNameFromValue(Value)
        If ErrorString <> "" Then
            Return Color.Red
        ElseIf TypeName = My.Resources.Label_Free Then
            Return Color.Gray
        ElseIf TypeName = My.Resources.Label_Bad Then
            Return Color.Red
        ElseIf TypeName = My.Resources.Label_Last Then
            Return Color.Black
        ElseIf TypeName = My.Resources.Label_Reserved Then
            Return Color.Orange
        ElseIf Value > _FATTables.FAT.TableLength Then
            Return Color.Red
        Else
            Return Color.Blue
        End If
    End Function

    Private Function GridGetIndex(e As MouseEventArgs) As Integer
        Dim Index As Integer = -1

        Dim Length = _FATTables.FAT.TableLength - 2
        Dim LeftPos As Integer = Int(e.X / _GridSize)
        Dim TopPos As Integer = Int(e.Y / _GridSize)
        Dim MaxWidth As Integer = Int(PictureBoxFAT.Width / _GridSize)
        Dim MaxHeight As Integer = CeilDiv(CUInt(Length), CUInt(MaxWidth))

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
                If Row.Cells(GRID_COLUMN_CLUSTER).Value = Index + 2 Then
                    RowIndex = Row.Index
                    Exit For
                End If
            Next
        End If

        Return RowIndex
    End Function

    Private Sub HexDisplayDiskImage(Cluster As UShort)
        Dim Offset = _Disk.BPB.ClusterToOffset(2)

        Dim HexViewSectorData As New HexViewSectorData(_Disk, Offset, _Disk.Image.Length - Offset) With {
            .Description = My.Resources.Label_Disk
        }

        If DisplayHexViewForm(HexViewSectorData, True, True, False, Cluster) Then
            _Updated = True
        End If
    End Sub

    Private Sub InitializeGridColumns()
        Dim GridViewColumn As DataGridViewColumn

        DataGridViewFAT.Columns.Clear()

        GridViewColumn = New DataGridViewTextBoxColumn With {
            .Name = GRID_COLUMN_CLUSTER,
            .HeaderText = My.Resources.Label_Cluster,
            .ReadOnly = True,
            .DataPropertyName = GRID_COLUMN_CLUSTER,
            .Width = 65,
            .DefaultCellStyle = New DataGridViewCellStyle()
        }
        GridViewColumn.DefaultCellStyle.SelectionForeColor = Color.Black
        GridViewColumn.DefaultCellStyle.SelectionBackColor = Color.White
        DataGridViewFAT.Columns.Add(GridViewColumn)

        GridViewColumn = New DataGridViewTextBoxColumn With {
            .Name = GRID_COLUMN_TYPE,
            .HeaderText = My.Resources.Label_Type,
            .ReadOnly = True,
            .Resizable = False,
            .SortMode = DataGridViewColumnSortMode.NotSortable,
            .DataPropertyName = GRID_COLUMN_TYPE,
            .Width = 75,
            .DefaultCellStyle = New DataGridViewCellStyle()
        }
        GridViewColumn.DefaultCellStyle.SelectionForeColor = Color.Black
        GridViewColumn.DefaultCellStyle.SelectionBackColor = Color.White
        DataGridViewFAT.Columns.Add(GridViewColumn)

        GridViewColumn = New DataGridViewTextBoxColumn With {
           .Name = GRID_COLUMN_VALUE,
           .HeaderText = My.Resources.Label_Value,
           .SortMode = DataGridViewColumnSortMode.NotSortable,
           .DataPropertyName = GRID_COLUMN_VALUE,
           .Width = 60
       }
        DataGridViewFAT.Columns.Add(GridViewColumn)

        GridViewColumn = New DataGridViewTextBoxColumn With {
            .Name = GRID_COLUMN_ERROR,
            .HeaderText = My.Resources.Label_Error,
            .ReadOnly = True,
            .Resizable = False,
            .SortMode = DataGridViewColumnSortMode.NotSortable,
            .DataPropertyName = GRID_COLUMN_ERROR,
            .Width = 100,
            .DefaultCellStyle = New DataGridViewCellStyle()
        }
        GridViewColumn.DefaultCellStyle.ForeColor = Color.Red
        GridViewColumn.DefaultCellStyle.SelectionForeColor = Color.Red
        GridViewColumn.DefaultCellStyle.SelectionBackColor = Color.White
        DataGridViewFAT.Columns.Add(GridViewColumn)

        GridViewColumn = New DataGridViewTextBoxColumn With {
            .Name = GRID_COLUMN_FILE,
            .HeaderText = My.Resources.Label_File,
            .ReadOnly = True,
            .DataPropertyName = GRID_COLUMN_FILE,
            .Width = 200,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            .DefaultCellStyle = New DataGridViewCellStyle()
        }
        GridViewColumn.DefaultCellStyle.SelectionForeColor = Color.Black
        GridViewColumn.DefaultCellStyle.SelectionBackColor = Color.White
        DataGridViewFAT.Columns.Add(GridViewColumn)

        GridViewColumn = New DataGridViewTextBoxColumn With {
            .Name = GRID_COLUMN_FILE_INDEX,
            .Visible = False,
            .DataPropertyName = GRID_COLUMN_FILE_INDEX
        }
        DataGridViewFAT.Columns.Add(GridViewColumn)
    End Sub

    Private Sub PopulateContextMenu()
        Dim Item As ToolStripMenuItem
        Dim SubItem As ToolStripMenuItem

        Dim Label As New ToolStripLabel(FormatLabelPair(My.Resources.Label_Cluster, 0)) With {
            .Name = LABEL_CLUSTER
        }
        ContextMenuGrid.Items.Add(Label)
        ContextMenuGrid.Items.Add(New ToolStripSeparator)

        Item = ContextMenuGrid.Items.Add(My.Resources.Menu_FATEdit_Free)
        Item.Tag = FAT12.FAT_FREE_CLUSTER

        Item = ContextMenuGrid.Items.Add(My.Resources.Menu_FATEdit_Bad)
        Item.Tag = FAT12.FAT_BAD_CLUSTER

        Item = ContextMenuGrid.Items.Add(My.Resources.Menu_FATEdit_Last)
        Item.DropDown = ContextMenuLast
        SubItem = ContextMenuLast.Items.Add(FAT12.FAT_LAST_CLUSTER_END)
        SubItem.Tag = FAT12.FAT_LAST_CLUSTER_END
        ContextMenuLast.Items.Add("-")
        For Counter = FAT12.FAT_LAST_CLUSTER_START To FAT12.FAT_LAST_CLUSTER_END - 1
            SubItem = ContextMenuLast.Items.Add(Counter)
            SubItem.Tag = Counter
        Next

        Item = ContextMenuGrid.Items.Add(My.Resources.Menu_FATEdit_Reserved)
        Item.DropDown = ContextMenuReserved
        For Counter = FAT12.FAT_RESERVED_START To FAT12.FAT_RESERVED_END
            SubItem = ContextMenuReserved.Items.Add(Counter)
            SubItem.Tag = Counter
        Next
        SubItem = ContextMenuReserved.Items.Add(1)
        SubItem.Tag = 1

        ContextMenuGrid.Items.Add(New ToolStripSeparator)

        Item = ContextMenuGrid.Items.Add(My.Resources.Menu_FATEdit_ViewHexEditor)
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

    Private Sub RefreshFAT()
        _Directory.RefreshData()

        RefreshGrid()
        PictureBoxFAT.Refresh()
    End Sub

    Private Sub RefreshGrid()
        _IgnoreEvents = True

        _OffsetLookup.Clear()
        Dim OffsetIndex As UInteger = 1
        For Each Row As DataGridViewRow In DataGridViewFAT.Rows
            Dim Cluster As UShort = Row.Cells(GRID_COLUMN_CLUSTER).Value
            Dim OffsetList = GetEntriesFromCluster(_FATTables.FAT, Cluster)
            Dim Value As UShort = Row.Cells(GRID_COLUMN_VALUE).Value
            Row.Cells(GRID_COLUMN_FILE).Value = GetFileFromOffsetList(OffsetList)
            Row.Cells(GRID_COLUMN_ERROR).Value = GetRowError(_FATTables.FAT, Cluster, Value)
            If OffsetList IsNot Nothing Then
                If Not _OffsetLookup.ContainsKey(OffsetList(0)) Then
                    'Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(OffsetList(0))
                    'Row.Cells("GridStartingCluster").Value = (DirectoryEntry.StartingCluster = Cluster)
                    _OffsetLookup.Add(OffsetList(0), OffsetIndex)
                    Row.Cells(GRID_COLUMN_FILE_INDEX).Value = OffsetIndex
                    OffsetIndex += 1
                Else
                    Row.Cells(GRID_COLUMN_FILE_INDEX).Value = _OffsetLookup.Item(OffsetList(0))
                    'Row.Cells("GridStartingCluster").Value = False
                End If
            Else
                Row.Cells(GRID_COLUMN_FILE_INDEX).Value = 0
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
            DataGridViewFAT.Rows(DataGridViewFAT.CurrentCellAddress.Y).Cells(GRID_COLUMN_VALUE).Value = Value
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
        Dim Value As UShort = Row.Cells.Item(GRID_COLUMN_VALUE).Value
        Dim TypeName = GetTypeNameFromValue(Value)

        If Row.Cells.Item(GRID_COLUMN_TYPE).Value <> TypeName Then
            Row.Cells.Item(GRID_COLUMN_TYPE).Value = TypeName
            Changed = True
        End If

        _IgnoreEvents = False

        Return Changed
    End Function

    Private Sub UpdateFAT(RowIndex As Integer)
        Dim Row = DataGridViewFAT.Rows(RowIndex)
        Dim Cluster As UShort = Row.Cells(GRID_COLUMN_CLUSTER).Value
        Dim Value As UShort = Row.Cells(GRID_COLUMN_VALUE).Value

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
                Dim Cluster As UShort = DataGridViewFAT.CurrentRow.Cells(GRID_COLUMN_CLUSTER).Value
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
            Dim Cluster As UShort = DataGridViewFAT.CurrentRow.Cells(GRID_COLUMN_CLUSTER).Value
            ContextMenuGrid.Items(LABEL_CLUSTER).Text = FormatLabelPair(My.Resources.Label_Cluster, Cluster)
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
            Fileindex = DataGridViewFAT.Rows(e.RowIndex).Cells(GRID_COLUMN_FILE_INDEX).Value
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
            DataGridViewFAT.CurrentCell = DataGridViewFAT.Rows(RowIndex).Cells(GRID_COLUMN_VALUE)
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
            Dim Cluster As UShort = Row.Item(GRID_COLUMN_CLUSTER)
            Dim File As String = Row.Item(GRID_COLUMN_FILE)
            Dim ErrorString As String = Row.Item(GRID_COLUMN_ERROR)
            FileIndex = Row.Item(GRID_COLUMN_FILE_INDEX)

            TooltipText = FormatLabelPair(My.Resources.Label_Cluster, vbTab & Cluster)
            If File <> "" Then
                TooltipText &= Environment.NewLine & FormatLabelPair(My.Resources.Label_File, vbTab & File)
            End If
            If ErrorString <> "" Then
                TooltipText &= Environment.NewLine & FormatLabelPair(My.Resources.Label_Error, vbTab & ErrorString)
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
            Dim Height = PictureBoxFAT.Height
            Dim Size As Integer = _GridSize

            Dim Pen As New Pen(Color.FromArgb(160, 160, 160))
            Dim Brushes(_GridCellColors.Length - 1) As SolidBrush
            For Counter = 0 To _GridCellColors.Length - 1
                Brushes(Counter) = New SolidBrush(_GridCellColors(Counter))
            Next

            Dim Brush As Brush
            Dim Left As Integer = 0
            Dim Top As Integer = 0
            Dim CellWidth As Integer
            Dim CellHeight As Integer
            Dim CellType As GridCellType
            Dim RowIndex As Integer = 0
            For Each Row As DataRow In _FATTable.Rows
                CellType = GetGridCellType(Row)
                CellWidth = Size
                If Left + CellWidth >= Width Then
                    CellWidth = Width - Left - 1
                End If
                CellHeight = Size
                If Top + CellHeight >= Height Then
                    CellHeight = Height - Top - 1
                End If
                _GridCells(RowIndex) = CellType
                Brush = Brushes(CellType)
                e.Graphics.FillRectangle(Brush, Left, Top, CellWidth, CellHeight)
                e.Graphics.DrawRectangle(Pen, Left, Top, CellWidth, CellHeight)
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
            _GridSize = GetGridSize(_FATTables.FAT.TableLength)
            PictureBoxFAT.Refresh()
        End If
    End Sub
#End Region
End Class