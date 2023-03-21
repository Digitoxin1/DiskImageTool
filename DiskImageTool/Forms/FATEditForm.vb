Imports System.Text
Imports DiskImageTool.DiskImage

Public Class FATEditForm
    Private ReadOnly _Disk As DiskImage.Disk
    Private _IgnoreEvents As Boolean = True

    Public Sub New(Disk As DiskImage.Disk)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Disk = Disk

        PopulateGrid()

        _IgnoreEvents = False
    End Sub

    Private Sub PopulateGrid()
        DataGridViewFAT.Rows.Clear()
        For Counter = 2 To _Disk.FAT.TableLength - 1
            Dim Value = _Disk.FAT.TableEntry(Counter)
            Dim TypeName = GetTypeFromValue(Value)
            Dim Filename = GetFileFromCluster(Counter)
            Dim Row As New DataGridViewRow
            Row.CreateCells(DataGridViewFAT, Counter, TypeName, Value, Filename)
            Dim Index = DataGridViewFAT.Rows.Add(Row)
            SetRowStyle(Index)
        Next
    End Sub

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

    Private Function GetFileFromCluster(Cluster As UInteger) As String
        Dim FileName As String = ""

        If _Disk.FAT.FileAllocation.ContainsKey(Cluster) Then
            Dim OffsetList = _Disk.FAT.FileAllocation.Item(Cluster)
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

        SetRowStyle(RowIndex)

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

        SetRowStyle(RowIndex)

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

    Private Sub SetRowStyle(RowIndex As Integer)
        Dim Style = New DataGridViewCellStyle

        Dim Row = DataGridViewFAT.Rows(RowIndex)
        Dim Cell = Row.Cells(2)
        Dim TypeName = GetTypeFromValue(Cell.Value)
        If TypeName = "Free" Then
            Style.ForeColor = Color.Gray
        ElseIf TypeName = "Bad" Then
            Style.ForeColor = Color.Red
        ElseIf TypeName = "Last" Then
            Style.ForeColor = Color.Black
        ElseIf TypeName = "Reserved" Then
            Style.ForeColor = Color.Orange
        ElseIf Cell.Value > _Disk.FAT.TableLength - 1 Then
            Style.ForeColor = Color.Red
        Else
            Style.ForeColor = Color.Blue
        End If
        Cell.Style = Style
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
End Class