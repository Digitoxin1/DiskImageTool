Public Structure SortEntity
    Dim Column As Integer
    Dim Order As SortOrder
End Structure

Public Class ListViewColumnSorter
    Implements IComparer

    Private ReadOnly _SortHistory As List(Of SortEntity)
    Private ReadOnly ObjectCompare As CaseInsensitiveComparer
    Private ColumnToSort As Integer
    Private OrderOfSort As SortOrder
    Public Sub New()
        ColumnToSort = -1
        OrderOfSort = SortOrder.None
        ObjectCompare = New CaseInsensitiveComparer()
        _SortHistory = New List(Of SortEntity)
    End Sub

    Public Property Order As SortOrder
        Set(ByVal value As SortOrder)
            If OrderOfSort <> value Then
                OrderOfSort = value
                If ColumnToSort > -1 Then
                    AddColumnToHistory(ColumnToSort, value)
                End If
            End If
        End Set
        Get
            Return OrderOfSort
        End Get
    End Property

    Public ReadOnly Property SortColumn As Integer
        Get
            Return ColumnToSort
        End Get
    End Property

    Public ReadOnly Property SortHistory As List(Of SortEntity)
        Get
            Dim NewSortHistory As New List(Of SortEntity)
            For Each Entity In _SortHistory
                NewSortHistory.Add(Entity)
            Next
            Return NewSortHistory
        End Get
    End Property

    Public Sub ClearHistory()
        _SortHistory.Clear()
    End Sub

    Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
        Dim Response As Integer = 0

        If OrderOfSort <> SortOrder.None And ColumnToSort > -1 Then
            Dim ListViewX = CType(x, ListViewItem)
            Dim ListViewY = CType(y, ListViewItem)
            Dim FileDataX As FileData = ListViewX.Tag
            Dim FileDataY As FileData = ListViewY.Tag
            Dim ObjX As Object
            Dim ObjY As Object

            If FileDataX Is Nothing Then
                Return -1
            ElseIf FileDataY Is Nothing Then
                Return 1
            End If

            Select Case ColumnToSort
                Case 0
                    ObjX = FileDataX.Index
                    ObjY = FileDataY.Index
                Case ListViewX.SubItems.IndexOfKey("FileName")
                    ObjX = FileDataX.DirectoryEntry.GetFileName
                    ObjY = FileDataY.DirectoryEntry.GetFileName
                Case ListViewX.SubItems.IndexOfKey("FileExtension")
                    ObjX = FileDataX.DirectoryEntry.GetFileExtension
                    ObjY = FileDataY.DirectoryEntry.GetFileExtension
                Case ListViewX.SubItems.IndexOfKey("FileSize")
                    ObjX = FileDataX.DirectoryEntry.FileSize
                    ObjY = FileDataY.DirectoryEntry.FileSize
                Case ListViewX.SubItems.IndexOfKey("FileLastWriteDate")
                    ObjX = FileDataX.DirectoryEntry.GetLastWriteDate.DateObject
                    ObjY = FileDataY.DirectoryEntry.GetLastWriteDate.DateObject
                Case ListViewX.SubItems.IndexOfKey("FileStartingCluster")
                    ObjX = FileDataX.DirectoryEntry.StartingCluster
                    ObjY = FileDataY.DirectoryEntry.StartingCluster
                Case ListViewX.SubItems.IndexOfKey("FileCreationDate")
                    ObjX = FileDataX.DirectoryEntry.GetCreationDate.DateObject
                    ObjY = FileDataY.DirectoryEntry.GetCreationDate.DateObject
                Case ListViewX.SubItems.IndexOfKey("FileLastAccessDate")
                    ObjX = FileDataX.DirectoryEntry.GetLastAccessDate.DateObject
                    ObjY = FileDataY.DirectoryEntry.GetLastAccessDate.DateObject
                Case Else
                    ObjX = ListViewX.SubItems(ColumnToSort).Text
                    ObjY = ListViewY.SubItems(ColumnToSort).Text
            End Select

            Dim CompareResult = ObjectCompare.Compare(ObjX, ObjY)

            If OrderOfSort = SortOrder.Ascending Then
                Response = CompareResult
            ElseIf OrderOfSort = SortOrder.Descending Then
                Response = -CompareResult
            End If
        End If

        Return Response
    End Function
    Public Sub Sort(Column As Integer, Optional Order As SortOrder = SortOrder.Ascending)
        If Column <> ColumnToSort Then
            ColumnToSort = Column
        End If
        If Order <> OrderOfSort Then
            OrderOfSort = Order
        End If
        If Column > -1 Then
            AddColumnToHistory(Column, Order)
        End If
    End Sub

    Public Sub Sort(Entity As SortEntity)
        Sort(Entity.Column, Entity.Order)
    End Sub

    Public Sub SwitchOrder()
        If OrderOfSort = SortOrder.Ascending Then
            OrderOfSort = SortOrder.Descending
        Else
            OrderOfSort = SortOrder.Ascending
        End If
        If ColumnToSort > -1 Then
            AddColumnToHistory(ColumnToSort, OrderOfSort)
        End If
    End Sub

    Private Sub AddColumnToHistory(Column As Integer, Order As SortOrder)
        RemoveColumnFromHistory(Column)

        Dim Entity As SortEntity
        Entity.Column = Column
        Entity.Order = Order
        _SortHistory.Add(Entity)
    End Sub

    Private Sub RemoveColumnFromHistory(Column As Integer)
        For Each Item In _SortHistory
            If Item.Column = Column Then
                _SortHistory.Remove(Item)
                Exit For
            End If
        Next
    End Sub
End Class
