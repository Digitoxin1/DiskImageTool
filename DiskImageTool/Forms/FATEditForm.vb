Imports System.IO
Imports DiskImageTool.DiskImage

Public Class FATEditForm
    Private ReadOnly _Disk As DiskImage.Disk
    Private ReadOnly _FAT() As FAT12
    Private ReadOnly _FATTable() As DataTable
    Private _IgnoreEvents As Boolean = True

    Public Sub New(Disk As DiskImage.Disk)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Disk = Disk

        ReDim _FAT(_Disk.BootSector.NumberOfFATs - 1)
        ReDim _FATTable(_Disk.BootSector.NumberOfFATs - 1)
        For Counter = 0 To _Disk.BootSector.NumberOfFATs - 1
            _FAT(Counter) = New FAT12(_Disk.Data, _Disk.BootSector, Counter, True)
            _FATTable(Counter) = GetDataTable(_FAT(Counter))
        Next

        DataGridViewFAT.DataSource = _FATTable(0)

        _IgnoreEvents = False
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
        Column = New DataColumn("File", Type.GetType("System.String")) With {
            .ReadOnly = True
        }
        FATTable.Columns.Add(Column)

        For Counter = 2 To FAT.TableLength - 1
            Dim Value = FAT.TableEntry(Counter)
            Dim Row = FATTable.NewRow
            Row.Item("Cluster") = Counter
            Row.Item("Value") = Value
            Row.Item("Type") = GetTypeFromValue(Value)
            Row.Item("File") = GetFileFromCluster(FAT, Counter)
            FATTable.Rows.Add(Row)
        Next

        Return FATTable
    End Function

    Private Sub DataGridViewFAT_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewFAT.CellValueChanged
        If _IgnoreEvents Then
            Exit Sub
        End If

        If e.ColumnIndex = 1 Then
            SetValueFromType(e.RowIndex)
        ElseIf e.ColumnIndex = 2 Then
            SetTypeFromValue(e.RowIndex)
        End If
    End Sub

    Private Sub DataGridViewFAT_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles DataGridViewFAT.CurrentCellDirtyStateChanged
        If DataGridViewFAT.CurrentCell.ColumnIndex = 1 Then
            If DataGridViewFAT.IsCurrentCellDirty Then
                DataGridViewFAT.CommitEdit(DataGridViewDataErrorContexts.Commit)
            End If
        End If
    End Sub

    Private Function GetFileFromCluster(FAT As FAT12, Cluster As UInteger) As String
        Dim FileName As String = ""

        If FAT.FileAllocation.ContainsKey(Cluster) Then
            Dim OffsetList = FAT.FileAllocation.Item(Cluster)
            Dim DirectoryEntry = _Disk.GetDirectoryEntryByOffset(OffsetList.Item(0))
            FileName = DirectoryEntry.GetFullFileName
        End If

        Return FileName
    End Function

    Private Function GetTypeFromValue(Value As UShort) As String
        If Value = 0 Then
            Return "Free"
        ElseIf Value = 4087 Then
            Return "Bad"
        ElseIf Value >= 4088 And Value <= 4095 Then
            Return "Last"
        ElseIf Value = 1 Or (Value >= 4080 And Value <= 4086) Then
            Return "Reserved"
        Else
            Return "Next"
        End If
    End Function

    Private Sub SetTypeFromValue(RowIndex As Integer)
        _IgnoreEvents = True
        Dim Row = DataGridViewFAT.Rows(RowIndex)
        Dim Cell = Row.Cells(2)

        If Cell.Value Is Nothing Then
            Cell.Value = 0
        End If

        Dim Value As UShort = Cell.Value
        Dim TypeName = GetTypeFromValue(Value)

        Row.Cells(1).Value = TypeName

        _IgnoreEvents = False
    End Sub

    Private Sub SetValueFromType(RowIndex As Integer)
        _IgnoreEvents = True
        Dim Row = DataGridViewFAT.Rows(RowIndex)
        Dim TypeName As String = Row.Cells(1).Value
        Dim Value As UShort = 0
        Dim SetValue As Boolean = False
        If TypeName = "Free" Then
            Value = 0
            SetValue = True
        ElseIf TypeName = "Bad" Then
            Value = 4087
            SetValue = True
        ElseIf TypeName = "Last" Then
            Value = 4095
            SetValue = True
        ElseIf TypeName = "Reserved" Then
            Value = 4086
            SetValue = True
        End If

        If SetValue Then
            Row.Cells(2).Value = Value
        End If

        _IgnoreEvents = False

        If Not SetValue Then
            DataGridViewFAT.CurrentCell = Row.Cells(2)
            DataGridViewFAT.BeginEdit(True)
        End If
    End Sub

    Private Sub DataGridViewFAT_CellLeave(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewFAT.CellLeave
        If _IgnoreEvents Then
            Exit Sub
        End If

        If e.ColumnIndex = 2 Then
            SetTypeFromValue(e.RowIndex)
        End If
    End Sub

    Private Function GetValueForeColor(Value As UShort) As Color
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
        ElseIf Value > _Disk.FAT.TableLength - 1 Then
            Return Color.Red
        Else
            Return Color.Blue
        End If
    End Function

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
        If e.Column.Index = 3 Then
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
        If e.RowIndex >= 0 And e.ColumnIndex = 2 Then
            e.CellStyle.ForeColor = GetValueForeColor(e.Value)
        End If
    End Sub
End Class