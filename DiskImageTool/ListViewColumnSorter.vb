Public Class ListViewColumnSorter
    Implements IComparer

    Private ColumnToSort As Integer
    Private OrderOfSort As SortOrder
    Private ReadOnly ObjectCompare As CaseInsensitiveComparer

    Public Sub New()
        ColumnToSort = -1
        OrderOfSort = SortOrder.None
        ObjectCompare = New CaseInsensitiveComparer()
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

    Public Property SortColumn As Integer
        Set(ByVal value As Integer)
            ColumnToSort = value
        End Set
        Get
            Return ColumnToSort
        End Get
    End Property

    Public Property Order As SortOrder
        Set(ByVal value As SortOrder)
            OrderOfSort = value
        End Set
        Get
            Return OrderOfSort
        End Get
    End Property
End Class
