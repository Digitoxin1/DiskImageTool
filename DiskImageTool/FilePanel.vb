Public Class FilePanel
    Private WithEvents ListView As ListView
    Private Const COL_ATTRIBUTES_WIDTH As Integer = 72
    Private Const COL_CLUSTER_ERROR_WIDTH As Integer = 30
    Private Const COL_CLUSTER_WIDTH As Integer = 57
    Private Const COL_CRC32_WIDTH As Integer = 70
    Private Const COL_CREATED_WIDTH As Integer = 140
    Private Const COL_EXTENSION_WIDTH As Integer = 50
    Private Const COL_FAT32_CLUSTER_WIDTH As Integer = 50
    Private Const COL_LAST_ACCESSED_WIDTH As Integer = 90
    Private Const COL_LASTWRITTEN_WIDTH As Integer = 120
    Private Const COL_LFN_WIDTH As Integer = 200
    Private Const COL_MODIFIED_WIDTH As Integer = 27
    Private Const COL_NAME_WIDTH As Integer = 120
    Private Const COL_NT_RESERVED_WIDTH As Integer = 30
    Private Const COL_SIZE_WIDTH As Integer = 80
    Private ReadOnly _ListViewHeader As ListViewHeader
    Private ReadOnly _lvwColumnSorter As ListViewColumnSorter
    Private _CheckAll As Boolean = False
    Private _ClickedGroup As ListViewGroup = Nothing
    Private _ColClusterError As ColumnHeader
    Private _ColCreationDate As ColumnHeader
    Private _ColFAT32Cluster As ColumnHeader
    Private _ColLastAccessDate As ColumnHeader
    Private _ColLFN As ColumnHeader
    Private _ColNTReserved As ColumnHeader
    Private _ColumnWidths() As Integer
    Private _SuppressEvent As Boolean = False

    Public Event GroupClick As EventHandler(Of ListViewGroup)
    Public Event ItemDoubleClick As EventHandler(Of ListViewItem)
    Public Event ItemSelectionChanged As EventHandler
    Public Event SortChanged As EventHandler(Of Boolean)

    Public Sub New(ListViewFiles As ListView)
        ListView = ListViewFiles

        _lvwColumnSorter = New ListViewColumnSorter
        _ListViewHeader = New ListViewHeader(ListView.Handle)

        Initialize()
        InitColumns()
    End Sub

    Public ReadOnly Property Items As ListView.ListViewItemCollection
        Get
            Return ListView.Items
        End Get
    End Property

    Public ReadOnly Property SelectedFileData As FileData
        Get
            If ListView.SelectedItems.Count = 1 Then
                Return TryCast(ListView.SelectedItems(0).Tag, FileData)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public ReadOnly Property SelectedItems As ListView.SelectedListViewItemCollection
        Get
            Return ListView.SelectedItems
        End Get
    End Property

    Public ReadOnly Property SortHistory As List(Of SortEntity)
        Get
            Return _lvwColumnSorter.SortHistory
        End Get
    End Property

    Public Function AddEmptyItem(Group As ListViewGroup, ItemIndex As Integer) As ListViewItem
        Dim Item = New ListViewItem("", Group) With {
            .UseItemStyleForSubItems = False,
            .Tag = Nothing
        }

        Dim SI = Item.SubItems.Add(My.Resources.Label_NoFiles)
        For Counter = 0 To 10
            Item.SubItems.Add("")
        Next

        If ListView.Items.Count <= ItemIndex Then
            ListView.Items.Add(Item)
        Else
            ListView.Items.Item(ItemIndex) = Item
        End If

        Return Item
    End Function

    Public Function AddGroup(Directory As DiskImage.IDirectory, Path As String, GroupIndex As Integer) As ListViewGroup
        Dim FileCount As UInteger = Directory.Data.FileCount

        Dim GroupName As String = IIf(Path = "", InParens(My.Resources.Label_Root), Path) & "  ("

        If FileCount = 1 Then
            GroupName &= String.Format(My.Resources.Label_Entry, FileCount)
        Else
            GroupName &= String.Format(My.Resources.Label_Entries, FileCount)
        End If

        If Directory.Data.HasBootSector Then
            GroupName &= ", " & My.Resources.Label_BootSector
        End If

        If Directory.Data.HasAdditionalData Then
            GroupName &= ", " & My.Resources.Label_AdditionalData
        End If

        GroupName &= ")"

        Dim Group = New ListViewGroup(GroupName) With {
            .Tag = Directory
        }
        ListView.Groups.Add(Group)

        Return Group
    End Function

    Public Function AddItem(FileData As FileData, Group As ListViewGroup, ItemIndex As Integer) As ListViewItem
        Dim Item = GetItem(Group, FileData)

        If ListView.Items.Count <= ItemIndex Then
            ListView.Items.Add(Item)
        Else
            ListView.Items.Item(ItemIndex) = Item
        End If

        Return Item
    End Function

    Public Sub ClearItems()
        EnableSorter(False)
        ListView.Items.Clear()
        SelectionChanged()
    End Sub

    Public Sub ClearModifiedFlag()
        ListView.BeginUpdate()

        For Each Item As ListViewItem In ListView.Items
            If Item.SubItems(0).Text = "#" Then
                Item.SubItems(0) = New ListViewItem.ListViewSubItem()
            End If
        Next

        ListView.EndUpdate()
    End Sub

    Public Sub ClearSort(Reset As Boolean)
        If Reset Then
            _lvwColumnSorter.Sort(0)
            ListView.Sort()
        Else
            _lvwColumnSorter.Sort(-1, SortOrder.None)
        End If
        ListView.SetSortIcon(-1, SortOrder.None)
        _lvwColumnSorter.ClearHistory()
        RaiseEvent SortChanged(Me, False)
    End Sub

    Public Function GetBottomIndex() As Integer
        Return ListView.GetBottomIndex
    End Function

    Public Function Populate(CurrentImage As CurrentImage, ClearItems As Boolean) As DirectoryScanResponse
        ListView.BeginUpdate()

        EnableSorter(False)

        If ClearItems Then
            ListView.Items.Clear()
            ListView.Groups.Clear()
        End If

        ListView.MultiSelect = True

        Dim Response = ProcessDirectoryEntries(CurrentImage.Disk.RootDirectory, Me)

        If Not ClearItems Then
            RemoveUnused(Response.ItemCount)
        End If

        AdjustWidths(Response)

        If Response.HasFATChainingErrors Then
            RefreshClusterErrors()
        End If

        AutoSize()

        EnableSorter(True)

        If CurrentImage.ImageData.SortHistory IsNot Nothing Then
            Sort(CurrentImage.ImageData.SortHistory)
        End If

        If ClearItems Then
            If CurrentImage.ImageData IsNot Nothing Then
                ScrollToIndex(CurrentImage.ImageData.BottomIndex)
            End If
        End If

        ListView.EndUpdate()
        ListView.Refresh()

        SelectionChanged()

        Return Response
    End Function

    Public Sub Reset()
        _CheckAll = False
        ClearSort(False)
        With ListView
            .BeginUpdate()
            .Items.Clear()
            .Groups.Clear()
            .MultiSelect = False
            .EndUpdate()
        End With
    End Sub

    Private Shared Function GetItem(Group As ListViewGroup, FileData As FileData) As ListViewItem
        Dim SI As ListViewItem.ListViewSubItem
        Dim ForeColor As Color
        Dim IsDeleted As Boolean = FileData.DirectoryEntry.IsDeleted
        Dim HasInvalidFileSize As Boolean = FileData.DirectoryEntry.HasInvalidFileSize
        Dim IsBlank As Boolean = FileData.DirectoryEntry.IsBlank

        Dim Attrib As String = IIf(FileData.DirectoryEntry.IsArchive, "A ", "- ") _
            & IIf(FileData.DirectoryEntry.IsReadOnly, "R ", "- ") _
            & IIf(FileData.DirectoryEntry.IsSystem, "S ", "- ") _
            & IIf(FileData.DirectoryEntry.IsHidden, "H ", "- ") _
            & IIf(FileData.DirectoryEntry.IsDirectory, "D ", "- ") _
            & IIf(FileData.DirectoryEntry.IsVolumeName, "V ", "- ")

        If IsDeleted Then
            ForeColor = Color.Gray
        ElseIf FileData.DirectoryEntry.IsValidVolumeName Then
            ForeColor = Color.Green
        ElseIf FileData.DirectoryEntry.IsDirectory And Not FileData.DirectoryEntry.IsVolumeName Then
            ForeColor = Color.Blue
        End If

        Dim ModifiedString As String = IIf(FileData.DirectoryEntry.IsModified, "#", "")

        Dim Item = New ListViewItem(ModifiedString, Group) With {
            .UseItemStyleForSubItems = False,
            .Tag = FileData
        }
        If ModifiedString = "" Then
            Item.ForeColor = ForeColor
        Else
            Item.ForeColor = Color.Blue
        End If

        SI = Item.SubItems.Add(FileData.DirectoryEntry.GetFileName(True))
        SI.Name = FilePanelColumns.FileName
        If Not IsDeleted And (FileData.DirectoryEntry.HasInvalidFilename Or FileData.DuplicateFileName) Then
            SI.ForeColor = Color.Red
        Else
            SI.ForeColor = ForeColor
        End If

        SI = Item.SubItems.Add(FileData.DirectoryEntry.GetFileExtension(True))
        SI.Name = FilePanelColumns.FileExtension
        If Not IsDeleted And (FileData.DirectoryEntry.HasInvalidExtension Or FileData.DuplicateFileName) Then
            SI.ForeColor = Color.Red
        Else
            SI.ForeColor = ForeColor
        End If

        If IsBlank Then
            SI = Item.SubItems.Add("")
        ElseIf HasInvalidFileSize Then
            SI = Item.SubItems.Add(My.Resources.Label_Invalid)
            If Not IsDeleted Then
                SI.ForeColor = Color.Red
            Else
                SI.ForeColor = ForeColor
            End If
        ElseIf Not IsDeleted And FileData.DirectoryEntry.HasIncorrectFileSize Then
            SI = Item.SubItems.Add(FormatThousands(FileData.DirectoryEntry.FileSize))
            SI.ForeColor = Color.Red
        Else
            SI = Item.SubItems.Add(FormatThousands(FileData.DirectoryEntry.FileSize))
            SI.ForeColor = ForeColor
        End If
        SI.Name = FilePanelColumns.FileSize

        If IsBlank Then
            SI = Item.SubItems.Add("")
        Else
            SI = Item.SubItems.Add(FileData.DirectoryEntry.GetLastWriteDate.ToString(True, True, False, True))
            If FileData.DirectoryEntry.GetLastWriteDate.IsValidDate Or IsDeleted Then
                SI.ForeColor = ForeColor
            Else
                SI.ForeColor = Color.Red
            End If
        End If
        SI.Name = FilePanelColumns.FileLastWriteDate

        Dim SubItemForeColor As Color = ForeColor
        If IsBlank Then
            SI = Item.SubItems.Add("")
        Else
            If FileData.DirectoryEntry.HasInvalidStartingCluster Then
                SI = Item.SubItems.Add(My.Resources.Label_Invalid)
                If Not IsDeleted Then
                    SubItemForeColor = Color.Red
                End If
            Else
                SI = Item.SubItems.Add(FormatThousands(FileData.DirectoryEntry.StartingCluster))
            End If
        End If
        SI.Name = FilePanelColumns.FileStartingCluster

        If IsBlank Then
            SI = Item.SubItems.Add("")
        Else
            Dim ErrorText As String = ""
            If Not IsDeleted And FileData.DirectoryEntry.IsCrossLinked Then
                SubItemForeColor = Color.Red
                ErrorText = "CL"
            ElseIf Not IsDeleted And FileData.DirectoryEntry.HasCircularChain Then
                SubItemForeColor = Color.Red
                ErrorText = "CC"
            End If
            SI.ForeColor = SubItemForeColor

            SI = Item.SubItems.Add(ErrorText)
            SI.ForeColor = Color.Red
        End If
        SI.Name = FilePanelColumns.FileClusterError

        If IsBlank Then
            Item.SubItems.Add("")
        Else
            SI = Item.SubItems.Add(Attrib)
            If Not IsDeleted And (FileData.DirectoryEntry.HasInvalidAttributes Or FileData.InvalidVolumeName) Then
                SI.ForeColor = Color.Red
            Else
                SI.ForeColor = ForeColor
            End If
        End If

        If IsBlank Then
            Item.SubItems.Add("")
        Else
            If FileData.DirectoryEntry.IsValidFile Then
                SI = Item.SubItems.Add(FileData.DirectoryEntry.GetChecksum().ToString("X8"))
            Else
                SI = Item.SubItems.Add("")
            End If
            SI.ForeColor = ForeColor
        End If

        If IsBlank Then
            SI = Item.SubItems.Add("")
        Else
            If FileData.DirectoryEntry.HasCreationDate Then
                SI = Item.SubItems.Add(FileData.DirectoryEntry.GetCreationDate.ToString(True, True, True, True))
                If FileData.DirectoryEntry.GetCreationDate.IsValidDate Or IsDeleted Then
                    SI.ForeColor = ForeColor
                Else
                    SI.ForeColor = Color.Red
                End If
            Else
                SI = Item.SubItems.Add("")
            End If
        End If
        SI.Name = FilePanelColumns.FileCreationDate

        If IsBlank Then
            SI = Item.SubItems.Add("")
        Else
            If FileData.DirectoryEntry.HasLastAccessDate Then
                SI = Item.SubItems.Add(FileData.DirectoryEntry.GetLastAccessDate.ToString())
                If FileData.DirectoryEntry.GetLastAccessDate.IsValidDate Or IsDeleted Then
                    SI.ForeColor = ForeColor
                Else
                    SI.ForeColor = Color.Red
                End If
            Else
                SI = Item.SubItems.Add("")
            End If
        End If
        SI.Name = FilePanelColumns.FileLastAccessDate


        If IsBlank Then
            Item.SubItems.Add("")
        Else
            Dim Reserved As String = ""
            If FileData.DirectoryEntry.ReservedForWinNT <> 0 Then
                Reserved = FileData.DirectoryEntry.ReservedForWinNT.ToString("X2")
            End If
            SI = Item.SubItems.Add(Reserved)
            If FileData.DirectoryEntry.HasNTUnknownFlags Then
                SI.ForeColor = Color.Red
            Else
                SI.ForeColor = ForeColor
            End If
        End If

        If IsBlank Then
            Item.SubItems.Add("")
        Else
            Dim FAT32Cluster As String = ""
            If FileData.DirectoryEntry.ReservedForFAT32 <> 0 Then
                FAT32Cluster = BitConverter.ToString(BitConverter.GetBytes(FileData.DirectoryEntry.ReservedForFAT32))
            End If
            SI = Item.SubItems.Add(FAT32Cluster)
            SI.ForeColor = Color.Red
        End If

        If IsBlank Then
            Item.SubItems.Add("")
        Else
            SI = Item.SubItems.Add(FileData.LFNFileName)
            SI.ForeColor = ForeColor
        End If

        Return Item
    End Function

    Private Sub AdjustWidths(Response As DirectoryScanResponse)
        If Response.HasCreated Then
            _ColCreationDate.Width = COL_CREATED_WIDTH
        Else
            _ColCreationDate.Width = 0
        End If

        If Response.HasLastAccessed Then
            _ColLastAccessDate.Width = COL_LAST_ACCESSED_WIDTH
        Else
            _ColLastAccessDate.Width = 0
        End If

        If Response.HasLFN Then
            _ColLFN.Width = COL_LFN_WIDTH
        Else
            _ColLFN.Width = 0
        End If

        If Response.HasNTReserved Then
            _ColNTReserved.Width = COL_NT_RESERVED_WIDTH
        Else
            _ColNTReserved.Width = 0
        End If

        If Response.HasFAT32Cluster Then
            _ColFAT32Cluster.Width = COL_FAT32_CLUSTER_WIDTH
        Else
            _ColFAT32Cluster.Width = 0
        End If

        If Response.HasFATChainingErrors Then
            _ColClusterError.Width = COL_CLUSTER_ERROR_WIDTH
        Else
            _ColClusterError.Width = 0
        End If
    End Sub

    Private Sub AutoSize()
        For Each Column As ColumnHeader In ListView.Columns
            If Column.Width > 0 Then
                Column.Width = -2
                If Column.Width < _ColumnWidths(Column.Index) Then
                    Column.Width = _ColumnWidths(Column.Index)
                End If
            End If
        Next
    End Sub

    Private Sub EnableSorter(Value As Boolean)
        If Value Then
            ListView.ListViewItemSorter = _lvwColumnSorter
        Else
            ListView.ListViewItemSorter = Nothing
        End If
    End Sub

    Private Sub InitColumns()
        ReDim _ColumnWidths(ListView.Columns.Count - 1)
        For Each Column As ColumnHeader In ListView.Columns
            _ColumnWidths(Column.Index) = Column.Width
        Next

        _ColCreationDate.Width = 0
        _ColLastAccessDate.Width = 0
        _ColLFN.Width = 0
        _ColClusterError.Width = 0
        _ColNTReserved.Width = 0
        _ColFAT32Cluster.Width = 0
    End Sub

    Private Sub Initialize()
        With ListView
            .FullRowSelect = True
            .View = View.Details
            .HideSelection = False
            .OwnerDraw = True
            .DoubleBuffer
            .Items.Clear()
            .Columns.Clear()
            .Columns.Add("", COL_MODIFIED_WIDTH, HorizontalAlignment.Left)
            .Columns.Add(My.Resources.FilePanel_Name, COL_NAME_WIDTH, HorizontalAlignment.Left)
            .Columns.Add(My.Resources.FilePanel_Extension, COL_EXTENSION_WIDTH, HorizontalAlignment.Left)
            .Columns.Add(My.Resources.FilePanel_Size, COL_SIZE_WIDTH, HorizontalAlignment.Right)
            .Columns.Add(My.Resources.FilePanel_LastWritten, COL_LASTWRITTEN_WIDTH, HorizontalAlignment.Left)
            .Columns.Add(My.Resources.FilePanel_Cluster, COL_CLUSTER_WIDTH, HorizontalAlignment.Right)
            _ColClusterError = .Columns.Add(My.Resources.FilePanel_ClusterError, COL_CLUSTER_ERROR_WIDTH, HorizontalAlignment.Left)
            .Columns.Add(My.Resources.FilePanel_Attributes, COL_ATTRIBUTES_WIDTH, HorizontalAlignment.Left)
            .Columns.Add(My.Resources.FilePanel_CRC32, COL_CRC32_WIDTH, HorizontalAlignment.Left)
            _ColCreationDate = .Columns.Add(My.Resources.FilePanel_Created, COL_CREATED_WIDTH, HorizontalAlignment.Left)
            _ColLastAccessDate = .Columns.Add(My.Resources.FilePanel_LastAccessed, COL_LAST_ACCESSED_WIDTH, HorizontalAlignment.Left)
            _ColNTReserved = .Columns.Add(My.Resources.FilePanel_NTReserved, COL_NT_RESERVED_WIDTH, HorizontalAlignment.Left)
            _ColFAT32Cluster = .Columns.Add(My.Resources.FilePanel_FAT32Cluster, COL_FAT32_CLUSTER_WIDTH, HorizontalAlignment.Left)
            _ColLFN = .Columns.Add(My.Resources.FilePanel_LFN, COL_LFN_WIDTH, HorizontalAlignment.Left)
        End With
    End Sub

    Private Sub RefreshCheckAll()
        Dim CheckAll = (ListView.SelectedItems.Count = ListView.Items.Count And ListView.Items.Count > 0)
        If CheckAll <> _CheckAll Then
            _CheckAll = CheckAll
            ListView.Invalidate(New Rectangle(0, 0, 20, 20), True)
        End If
    End Sub

    Private Sub RefreshClusterErrors()
        For Each Item As ListViewItem In ListView.Items
            Dim FileData As FileData = TryCast(Item.Tag, FileData)
            If FileData IsNot Nothing Then
                If FileData.DirectoryEntry.IsCrossLinked Then
                    Item.SubItems.Item(FilePanelColumns.FileStartingCluster).ForeColor = Color.Red
                    Item.SubItems.Item(FilePanelColumns.FileClusterError).Text = "CL"
                End If
            End If
        Next
    End Sub

    Private Sub RemoveUnused(ItemCount As Integer)
        For Counter = ListView.Items.Count - 1 To ItemCount Step -1
            ListView.Items.RemoveAt(Counter)
        Next
        For Counter = ListView.Groups.Count - 1 To 0 Step -1
            Dim Group = ListView.Groups.Item(Counter)
            If Group.Items.Count = 0 Then
                ListView.Groups.Remove(Group)
            End If
        Next
    End Sub
    Private Sub ScrollToIndex(Index As Integer)
        If Index > -1 AndAlso Index < ListView.Items.Count Then
            ListView.EnsureVisible(Index)
        End If
    End Sub

    Private Sub SelectionChanged()
        RefreshCheckAll()
        RaiseEvent ItemSelectionChanged(Me, EventArgs.Empty)
    End Sub

    Private Sub Sort(Item As SortEntity)
        _lvwColumnSorter.Sort(Item)
        ListView.Sort()
        ListView.SetSortIcon(_lvwColumnSorter.SortColumn, _lvwColumnSorter.Order)
    End Sub
    Private Sub Sort(SortList As List(Of SortEntity))
        If SortList.Count > 0 Then
            For Each Item In SortList
                Sort(Item)
            Next
            RaiseEvent SortChanged(Me, True)
        End If
    End Sub
#Region "Events"
    Private Sub ListView_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles ListView.ColumnClick
        If ListView.Items.Count = 0 Then
            Exit Sub
        End If

        If e.Column = 0 Then
            _CheckAll = Not _CheckAll
            _SuppressEvent = True
            For Each Item As ListViewItem In ListView.Items
                Item.Selected = _CheckAll
            Next
            _SuppressEvent = False
            SelectionChanged()
        Else
            If e.Column = _lvwColumnSorter.SortColumn Then
                _lvwColumnSorter.SwitchOrder()
            Else
                _lvwColumnSorter.Sort(e.Column)
            End If
            ListView.Sort()
            ListView.SetSortIcon(_lvwColumnSorter.SortColumn, _lvwColumnSorter.Order)
            RaiseEvent SortChanged(Me, True)
        End If
    End Sub
    Private Sub ListView_ColumnWidthChanging(sender As Object, e As ColumnWidthChangingEventArgs) Handles ListView.ColumnWidthChanging
        e.NewWidth = ListView.Columns(e.ColumnIndex).Width
        e.Cancel = True
    End Sub

    Private Sub ListView_DrawColumnHeader(sender As Object, e As DrawListViewColumnHeaderEventArgs) Handles ListView.DrawColumnHeader
        If e.ColumnIndex = 0 Then
            'Dim Offset As Integer
            'If (e.State And ListViewItemStates.Selected) > 0 Then
            ' Offset = 1
            ' Else
            '    Offset = 0
            'End If
            Dim State = IIf(_CheckAll, VisualStyles.CheckBoxState.CheckedNormal, VisualStyles.CheckBoxState.UncheckedNormal)
            Dim Size = CheckBoxRenderer.GetGlyphSize(e.Graphics, State)
            CheckBoxRenderer.DrawCheckBox(e.Graphics, New Point(4, (e.Bounds.Height - Size.Height) \ 2), State)
            'e.Graphics.DrawString(e.Header.Text, e.Font, New SolidBrush(Color.Black), New Point(20 + Offset, (e.Bounds.Height - Size.Height) \ 2 + Offset))
            'e.Graphics.DrawLine(New Pen(Color.FromArgb(229, 229, 229), 1), New Point(e.Bounds.Width - 1, 0), New Point(e.Bounds.Width - 1, e.Bounds.Height))
        Else
            e.DrawDefault = True
        End If
    End Sub

    Private Sub ListView_DrawItem(sender As Object, e As DrawListViewItemEventArgs) Handles ListView.DrawItem
        e.DrawDefault = True
    End Sub

    Private Sub ListView_DrawSubItem(sender As Object, e As DrawListViewSubItemEventArgs) Handles ListView.DrawSubItem
        e.DrawDefault = True
    End Sub

    Private Sub ListView_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles ListView.ItemSelectionChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        SelectionChanged()
    End Sub

    Private Sub ListView_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListView.MouseDoubleClick
        If e.Button = MouseButtons.Left Then
            If ListView.SelectedItems.Count = 1 Then
                RaiseEvent ItemDoubleClick(Me, ListView.SelectedItems(0))
            End If
        End If
    End Sub

    Private Sub ListView_MouseDown(sender As Object, e As MouseEventArgs) Handles ListView.MouseDown
        _ClickedGroup = Nothing

        If e.Button And MouseButtons.Right Then
            _ClickedGroup = ListView.GetGroupAtPoint(e.Location)
        End If
    End Sub

    Private Sub ListView_MouseUp(sender As Object, e As MouseEventArgs) Handles ListView.MouseUp
        If _ClickedGroup IsNot Nothing Then
            RaiseEvent GroupClick(Me, _ClickedGroup)
            _ClickedGroup = Nothing
        End If
    End Sub
#End Region
End Class
