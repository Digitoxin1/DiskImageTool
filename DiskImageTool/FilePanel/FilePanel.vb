Imports System.ComponentModel
Imports DiskImageTool.DiskImage

Public Class FilePanel
    Private WithEvents ContextMenuFiles As ContextMenuStrip
    Private WithEvents ContextMenuDirectory As ContextMenuStrip
    Private WithEvents ListViewFiles As ListViewEx
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
    Private ReadOnly _lvwColumnSorter As FilePanelColumnSorter
    Private _CheckAll As Boolean = False
    Private _ClickedGroup As ListViewGroup = Nothing
    Private _ColClusterError As ColumnHeader
    Private _ColCreationDate As ColumnHeader
    Private _ColFAT32Cluster As ColumnHeader
    Private _ColLastAccessDate As ColumnHeader
    Private _ColLFN As ColumnHeader
    Private _ColNTReserved As ColumnHeader
    Private _ColumnWidths() As Integer
    Private _CurrentImage As DiskImageContainer = Nothing
    Private _DirectoryScan As DirectoryScanResponse = Nothing
    Private _MenuState As FileMenuState
    Private _SuppressEvent As Boolean = False

    Public Enum FilePanelMenuItem
        FileProperties
        ExportFile
        ReplaceFile
        ViewDirectory
        ViewDirectorySeparator
        ViewFile
        ViewFileText
        ViewCrosslinked
        ImportFiles
        ImportFilesHere
        NewDirectory
        NewDirectoryHere
        DeleteFile
        UnDeleteFile
        FileRemove
        FixSize
    End Enum

    Public Event CurrentImageChangeStart As EventHandler(Of Boolean)
    Public Event CurrentImageChangeEnd As EventHandler(Of Boolean)
    Public Event ItemDoubleClick As EventHandler(Of ListViewItem)
    Public Event ItemDrag As EventHandler(Of ItemDragEventArgs)
    Public Event ItemSelectionChanged As EventHandler
    Public Event MenuItemClicked As EventHandler(Of MenuItemClickedEventArgs)
    Public Event SortChanged As EventHandler(Of Boolean)

    Public Sub New(ListViewFiles As ListViewEx)
        Me.ListViewFiles = ListViewFiles
        ContextMenuFiles = New ContextMenuStrip
        ContextMenuDirectory = New ContextMenuStrip
        Me.ListViewFiles.ContextMenuStrip = ContextMenuFiles

        _lvwColumnSorter = New FilePanelColumnSorter
        _ListViewHeader = New ListViewHeader(Me.ListViewFiles.Handle)

        Initialize()
        InitColumns()
        InitContextMenuDirectory()
        InitContextMenuFiles()

        _MenuState = GetFileMenuState(Me)
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        ListViewFiles = Nothing
        ContextMenuFiles = Nothing
        ContextMenuDirectory = Nothing
    End Sub

    Public ReadOnly Property CurrentImage As DiskImageContainer
        Get
            Return _CurrentImage
        End Get
    End Property

    Public ReadOnly Property DirectoryScan As DirectoryScanResponse
        Get
            Return _DirectoryScan
        End Get
    End Property

    Public ReadOnly Property FirstSelectedItem As ListViewItem
        Get
            If ListViewFiles.SelectedItems.Count > 0 Then
                Return ListViewFiles.SelectedItems(0)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public ReadOnly Property Items As ListView.ListViewItemCollection
        Get
            Return ListViewFiles.Items
        End Get
    End Property

    Public ReadOnly Property MenuState As FileMenuState
        Get
            Return _MenuState
        End Get
    End Property

    Public ReadOnly Property SelectedFileData As FileData
        Get
            If ListViewFiles.SelectedItems.Count = 1 Then
                Return TryCast(ListViewFiles.SelectedItems(0).Tag, FileData)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public ReadOnly Property SelectedItems As ListView.SelectedListViewItemCollection
        Get
            Return ListViewFiles.SelectedItems
        End Get
    End Property

    Public Function AddEmptyItem(Group As ListViewGroup, ItemIndex As Integer) As ListViewItem
        Dim Item As New ListViewItem("", Group) With {
            .UseItemStyleForSubItems = False,
            .Tag = Nothing
        }

        Dim SI = Item.SubItems.Add(My.Resources.Label_NoFiles)
        For Counter = 0 To 10
            Item.SubItems.Add("")
        Next

        If ListViewFiles.Items.Count <= ItemIndex Then
            ListViewFiles.Items.Add(Item)
        Else
            ListViewFiles.Items.Item(ItemIndex) = Item
        End If

        Return Item
    End Function

    Public Function AddGroup(Directory As IDirectory, Path As String, GroupIndex As Integer) As ListViewGroup
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

        Dim Group As New ListViewGroup(GroupName) With {
            .Tag = Directory
        }
        ListViewFiles.Groups.Add(Group)

        Return Group
    End Function

    Public Function AddItem(FileData As FileData, Group As ListViewGroup, ItemIndex As Integer) As ListViewItem
        Dim Item = GetItem(Group, FileData)

        If ListViewFiles.Items.Count <= ItemIndex Then
            ListViewFiles.Items.Add(Item)
        Else
            ListViewFiles.Items.Item(ItemIndex) = Item
        End If

        Return Item
    End Function

    Public Sub ClearModifiedFlag()
        ListViewFiles.BeginUpdate()

        For Each Item As ListViewItem In ListViewFiles.Items
            If Item.SubItems(0).Text = "#" Then
                Item.SubItems(0) = New ListViewItem.ListViewSubItem()
            End If
        Next

        ListViewFiles.EndUpdate()
    End Sub

    Public Sub ClearSort(Reset As Boolean)
        If Reset Then
            _lvwColumnSorter.Sort(0)
            ListViewFiles.Sort()
        Else
            _lvwColumnSorter.Sort(-1, SortOrder.None)
        End If
        ListViewFiles.SetSortIcon(-1, SortOrder.None)
        _lvwColumnSorter.ClearHistory()

        If Reset Then
            UpdateSortHistory()
        End If

        RaiseEvent SortChanged(Me, False)
    End Sub

    Public Function DoDragDrop(data As Object, allowedEffects As DragDropEffects) As DragDropEffects
        Return ListViewFiles.DoDragDrop(data, allowedEffects)
    End Function

    Private Sub UpdateSortHistory()
        If _CurrentImage IsNot Nothing Then
            _CurrentImage.ImageData.SortHistory = _lvwColumnSorter.SortHistory
        End If
    End Sub

    Public Sub Load(CurrentImage As DiskImageContainer, DoItemScan As Boolean)
        Dim ClearItems As Boolean = CurrentImage IsNot _CurrentImage

        If _CurrentImage IsNot Nothing AndAlso ClearItems Then
            _CurrentImage.ImageData.BottomIndex = ListViewFiles.GetBottomIndex
        End If

        _CurrentImage = CurrentImage

        RaiseEvent CurrentImageChangeStart(Me, DoItemScan)

        Dim Response As DirectoryScanResponse = Nothing
        Dim IsValidImage = CurrentImage IsNot Nothing AndAlso CurrentImage.Disk IsNot Nothing AndAlso CurrentImage.Disk.IsValidImage


        ListViewFiles.BeginUpdate()

        ClearSort(False)
        EnableSorter(False)

        If ClearItems Then
            ListViewFiles.Items.Clear()
            ListViewFiles.Groups.Clear()
        End If

        If IsValidImage Then
            ListViewFiles.MultiSelect = True

            Response = ProcessDirectoryEntries(CurrentImage.Disk.RootDirectory, Me)

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
        End If

        ListViewFiles.EndUpdate()
        ListViewFiles.Refresh()

        _DirectoryScan = Response

        SelectionChanged()

        RaiseEvent CurrentImageChangeEnd(Me, DoItemScan)
    End Sub

    Public Sub Reset()
        If _CurrentImage IsNot Nothing Then
            _CurrentImage.ImageData.BottomIndex = ListViewFiles.GetBottomIndex
        End If

        _CheckAll = False
        ClearSort(False)
        With ListViewFiles
            .BeginUpdate()
            .Items.Clear()
            .Groups.Clear()
            .MultiSelect = False
            .EndUpdate()
        End With
        _CurrentImage = Nothing
        _MenuState = GetFileMenuState(Me)
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

        Dim Item As New ListViewItem(ModifiedString, Group) With {
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

    Private Function GetMenuItem(MenuStrip As ContextMenuStrip, name As FilePanelMenuItem) As ToolStripItem
        Dim key As String = name

        Return MenuStrip.Items.Item(key)
    End Function

    Private Function AddMenuItem(MenuStrip As ContextMenuStrip, name As FilePanelMenuItem, text As String) As ToolStripMenuItem
        Dim Item As New ToolStripMenuItem(text) With {
            .Name = name
        }
        MenuStrip.Items.Add(Item)

        Return Item
    End Function

    Private Function AddMenuItem(MenuStrip As ContextMenuStrip, name As FilePanelMenuItem, text As String, image As Image) As ToolStripMenuItem
        Dim Item As New ToolStripMenuItem(text, image) With {
            .Name = name
        }
        MenuStrip.Items.Add(Item)

        Return Item
    End Function

    Private Function AddMenuSeparator(MenuStrip As ContextMenuStrip) As ToolStripItem
        Dim Item As New ToolStripSeparator()
        MenuStrip.Items.Add(Item)

        Return Item
    End Function

    Private Function AddMenuSeparator(MenuStrip As ContextMenuStrip, name As FilePanelMenuItem) As ToolStripItem
        Dim Item As New ToolStripSeparator() With {
            .Name = name
        }
        MenuStrip.Items.Add(Item)

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
        For Each Column As ColumnHeader In ListViewFiles.Columns
            If Column.Width > 0 Then
                Column.Width = -2
                If Column.Width < _ColumnWidths(Column.Index) Then
                    Column.Width = _ColumnWidths(Column.Index)
                End If
            End If
        Next
    End Sub

    Private Sub DisplayDirectoryContextMenu(Directory As IDirectory, Location As Point)
        ContextMenuDirectory.Tag = Directory
        ContextMenuDirectory.Show(Location)
    End Sub

    Private Sub EnableSorter(Value As Boolean)
        If Value Then
            ListViewFiles.ListViewItemSorter = _lvwColumnSorter
        Else
            ListViewFiles.ListViewItemSorter = Nothing
        End If
    End Sub

    Private Sub InitColumns()
        ReDim _ColumnWidths(ListViewFiles.Columns.Count - 1)
        For Each Column As ColumnHeader In ListViewFiles.Columns
            _ColumnWidths(Column.Index) = Column.Width
        Next

        _ColCreationDate.Width = 0
        _ColLastAccessDate.Width = 0
        _ColLFN.Width = 0
        _ColClusterError.Width = 0
        _ColNTReserved.Width = 0
        _ColFAT32Cluster.Width = 0
    End Sub

    Private Sub InitContextMenuDirectory()
        AddMenuItem(ContextMenuDirectory, FilePanelMenuItem.ViewDirectory, My.Resources.Menu_ViewDirectoryAlt, My.Resources.FolderHex)
        AddMenuSeparator(ContextMenuDirectory)
        AddMenuItem(ContextMenuDirectory, FilePanelMenuItem.ImportFiles, My.Resources.Menu_ImportFiles, My.Resources.Import)
        AddMenuItem(ContextMenuDirectory, FilePanelMenuItem.NewDirectory, My.Resources.Menu_NewDirectory, My.Resources.NewFolder)
    End Sub

    Private Sub InitContextMenuFiles()
        Dim Item As ToolStripItem

        AddMenuItem(ContextMenuFiles, FilePanelMenuItem.FileProperties, My.Resources.Menu_EditFileProperties, My.Resources.PropertiesFolderClosed)
        AddMenuItem(ContextMenuFiles, FilePanelMenuItem.ExportFile, My.Resources.Menu_ExportFile, My.Resources.Export)
        AddMenuItem(ContextMenuFiles, FilePanelMenuItem.ReplaceFile, My.Resources.Menu_ReplaceFile, My.Resources.SwitchFolders)
        AddMenuSeparator(ContextMenuFiles)
        AddMenuItem(ContextMenuFiles, FilePanelMenuItem.ViewDirectory, My.Resources.Menu_ViewDirectoryAlt, My.Resources.FolderHex)
        AddMenuSeparator(ContextMenuFiles, FilePanelMenuItem.ViewDirectorySeparator)
        AddMenuItem(ContextMenuFiles, FilePanelMenuItem.ViewFile, My.Resources.Menu_ViewFile, My.Resources.BinaryFile)
        AddMenuItem(ContextMenuFiles, FilePanelMenuItem.ViewFileText, My.Resources.Menu_ViewFileAsText, My.Resources.TextFile)
        AddMenuItem(ContextMenuFiles, FilePanelMenuItem.ViewCrosslinked, My.Resources.Menu_ViewCrosslinked)
        AddMenuSeparator(ContextMenuFiles)
        AddMenuItem(ContextMenuFiles, FilePanelMenuItem.ImportFiles, My.Resources.Menu_ImportFiles, My.Resources.Import)
        AddMenuItem(ContextMenuFiles, FilePanelMenuItem.ImportFilesHere, My.Resources.Menu_ImportFilesHere)
        AddMenuSeparator(ContextMenuFiles)
        AddMenuItem(ContextMenuFiles, FilePanelMenuItem.NewDirectory, My.Resources.Menu_NewDirectory, My.Resources.NewFolder)
        AddMenuItem(ContextMenuFiles, FilePanelMenuItem.NewDirectoryHere, My.Resources.Menu_NewDirectoryHere)
        AddMenuSeparator(ContextMenuFiles)
        AddMenuItem(ContextMenuFiles, FilePanelMenuItem.DeleteFile, My.Resources.Menu_DeleteFile, My.Resources.DeleteDocument)
        AddMenuItem(ContextMenuFiles, FilePanelMenuItem.UnDeleteFile, My.Resources.Menu_UndeleteFile)
        AddMenuItem(ContextMenuFiles, FilePanelMenuItem.FileRemove, My.Resources.Menu_RemoveFile)

        Item = AddMenuSeparator(ContextMenuFiles)
        Item.Visible = False

        Item = AddMenuItem(ContextMenuFiles, FilePanelMenuItem.FixSize, My.Resources.Menu_FixFlieSize)
        Item.Visible = False
    End Sub

    Private Sub Initialize()
        With ListViewFiles
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
        Dim CheckAll = (ListViewFiles.SelectedItems.Count = ListViewFiles.Items.Count And ListViewFiles.Items.Count > 0)
        If CheckAll <> _CheckAll Then
            _CheckAll = CheckAll
            ListViewFiles.Invalidate(New Rectangle(0, 0, 20, 20), True)
        End If
    End Sub

    Private Sub RefreshClusterErrors()
        For Each Item As ListViewItem In ListViewFiles.Items
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
        For Counter = ListViewFiles.Items.Count - 1 To ItemCount Step -1
            ListViewFiles.Items.RemoveAt(Counter)
        Next
        For Counter = ListViewFiles.Groups.Count - 1 To 0 Step -1
            Dim Group = ListViewFiles.Groups.Item(Counter)
            If Group.Items.Count = 0 Then
                ListViewFiles.Groups.Remove(Group)
            End If
        Next
    End Sub
    Private Sub ScrollToIndex(Index As Integer)
        If Index > -1 AndAlso Index < ListViewFiles.Items.Count Then
            ListViewFiles.EnsureVisible(Index)
        End If
    End Sub

    Private Sub SelectionChanged()
        RefreshCheckAll()
        _MenuState = GetFileMenuState(Me)
        RaiseEvent ItemSelectionChanged(Me, EventArgs.Empty)
    End Sub

    Private Sub Sort(Item As SortEntity)
        _lvwColumnSorter.Sort(Item)
        ListViewFiles.Sort()
        ListViewFiles.SetSortIcon(_lvwColumnSorter.SortColumn, _lvwColumnSorter.Order)
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
    Private Sub ContextMenuDirectory_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ContextMenuDirectory.ItemClicked
        ContextMenuDirectory.Close(ToolStripDropDownCloseReason.ItemClicked)

        Dim Item As Integer
        If Integer.TryParse(e.ClickedItem.Name, Item) Then
            Dim ParentDirectory = TryCast(ContextMenuDirectory.Tag, IDirectory)
            If ParentDirectory IsNot Nothing Then
                RaiseEvent MenuItemClicked(Me, New MenuItemClickedEventArgs(Item, ParentDirectory))
            End If
        End If
    End Sub
    Private Sub ContextMenuFiles_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ContextMenuFiles.ItemClicked
        ContextMenuFiles.Close(ToolStripDropDownCloseReason.ItemClicked)

        Dim Item As Integer
        If Integer.TryParse(e.ClickedItem.Name, Item) Then
            RaiseEvent MenuItemClicked(Me, New MenuItemClickedEventArgs(Item, _MenuState.ParentDirectory))
        End If
    End Sub

    Private Sub ContextMenuFiles_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuFiles.Opening
        If SelectedItems.Count = 0 Then
            e.Cancel = True
        End If

        SetMenuItemStateEnabled(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.FileProperties), _MenuState.FilePropertiesEnabled)
        SetMenuItemState(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.ExportFile), _MenuState.ExportFile)
        SetMenuItemStateEnabled(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.ReplaceFile), _MenuState.ReplaceFileEnabled)
        SetMenuItemState(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.ViewDirectory), _MenuState.ViewDirectory, _MenuState.ParentDirectory)
        SetMenuItemStateVisible(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.ViewDirectorySeparator), _MenuState.ViewDirectory.Visible)
        SetMenuItemState(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.ViewFile), _MenuState.ViewFile)
        SetMenuItemState(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.ViewFileText), _MenuState.ViewFileText)
        SetMenuItemStateVisible(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.ViewCrosslinked), _MenuState.ViewCrosslinkedVisible)
        SetMenuItemStateEnabled(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.ImportFilesHere), _MenuState.ImportFilesHereEnabled)
        SetMenuItemStateEnabled(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.NewDirectoryHere), _MenuState.NewDirectoryHereEnabled)
        SetMenuItemState(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.DeleteFile), _MenuState.DeleteFile)
        SetMenuItemState(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.UnDeleteFile), _MenuState.UndeleteFile)
        SetMenuItemState(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.FileRemove), _MenuState.RemoveFile)
        SetMenuItemStateEnabled(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.FixSize), _MenuState.FixSizeEnabled)
        SetMenuItemStateEnabled(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.ImportFiles), _MenuState.AddFileEnabled, _MenuState.ParentDirectory)
        SetMenuItemStateEnabled(GetMenuItem(ContextMenuFiles, FilePanelMenuItem.NewDirectory), _MenuState.AddFileEnabled, _MenuState.ParentDirectory)
    End Sub

    Private Sub ListView_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles ListViewFiles.ColumnClick
        If ListViewFiles.Items.Count = 0 Then
            Exit Sub
        End If

        If e.Column = 0 Then
            _CheckAll = Not _CheckAll
            _SuppressEvent = True
            For Each Item As ListViewItem In ListViewFiles.Items
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
            ListViewFiles.Sort()
            ListViewFiles.SetSortIcon(_lvwColumnSorter.SortColumn, _lvwColumnSorter.Order)
            UpdateSortHistory()
            RaiseEvent SortChanged(Me, True)
        End If
    End Sub
    Private Sub ListView_ColumnWidthChanging(sender As Object, e As ColumnWidthChangingEventArgs) Handles ListViewFiles.ColumnWidthChanging
        e.NewWidth = ListViewFiles.Columns(e.ColumnIndex).Width
        e.Cancel = True
    End Sub

    Private Sub ListView_DrawColumnHeader(sender As Object, e As DrawListViewColumnHeaderEventArgs) Handles ListViewFiles.DrawColumnHeader
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

    Private Sub ListView_DrawItem(sender As Object, e As DrawListViewItemEventArgs) Handles ListViewFiles.DrawItem
        e.DrawDefault = True
    End Sub

    Private Sub ListView_DrawSubItem(sender As Object, e As DrawListViewSubItemEventArgs) Handles ListViewFiles.DrawSubItem
        e.DrawDefault = True
    End Sub

    'Private Sub ListView_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles ListViewFiles.ItemSelectionChanged
    '    If _SuppressEvent Then
    '        Exit Sub
    '    End If

    '    SelectionChanged()
    'End Sub

    Private Sub ListView_ItemDrag(sender As Object, e As ItemDragEventArgs) Handles ListViewFiles.ItemDrag
        RaiseEvent ItemDrag(Me, e)
    End Sub

    Private Sub ListView_ItemSelectionEnd(sender As Object, e As EventArgs) Handles ListViewFiles.ItemSelectionEnd
        If _SuppressEvent Then
            Exit Sub
        End If

        Debug.Print("FilePanel.ListView_ItemSelectionEnd fired")

        SelectionChanged()
    End Sub

    Private Sub ListView_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListViewFiles.MouseDoubleClick
        If e.Button = MouseButtons.Left Then
            If ListViewFiles.SelectedItems.Count = 1 Then
                RaiseEvent ItemDoubleClick(Me, ListViewFiles.SelectedItems(0))
            End If
        End If
    End Sub

    Private Sub ListView_MouseDown(sender As Object, e As MouseEventArgs) Handles ListViewFiles.MouseDown
        _ClickedGroup = Nothing

        If e.Button And MouseButtons.Right Then
            _ClickedGroup = ListViewFiles.GetGroupAtPoint(e.Location)
        End If
    End Sub

    Private Sub ListView_MouseUp(sender As Object, e As MouseEventArgs) Handles ListViewFiles.MouseUp
        If _ClickedGroup IsNot Nothing Then
            Dim Directory = TryCast(_ClickedGroup.Tag, IDirectory)
            If Directory IsNot Nothing Then
                _ClickedGroup = Nothing
                DisplayDirectoryContextMenu(Directory, ListView.MousePosition)
            End If
        End If
    End Sub
#End Region
End Class
