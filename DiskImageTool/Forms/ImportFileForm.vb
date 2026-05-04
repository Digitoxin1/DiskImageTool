Imports System.IO
Imports DiskImageTool.DiskImage

Public Class ImportFileForm
    Private Const COLUMN_ACTION As String = "Action"
    Private Const COLUMN_CREATION_TIME As String = "CreationTime"
    Private Const COLUMN_IS_SELECTED As String = "IsSelected"
    Private Const COLUMN_LAST_ACCESS_TIME As String = "LastAccessTime"
    Private ReadOnly _ExistingPathMap As Dictionary(Of String, DirectoryEntry)
    Private ReadOnly _SelectionByPath As New Dictionary(Of String, Boolean)
    Private _FileList As ImportDirectoryRoot
    Private _FileNames() As String
    Private _HasLFN As Boolean
    Private _HasNTExtensions As Boolean
    Private _HasOverwriteCandidates As Boolean
    Private _IgnoreEvent As Boolean = False
    Private _LastSelectedIndex As Integer = -1
    Private _ListViewHeader As ListViewHeader

    Public Sub New(CurrentDirectory As IDirectory, FileNames() As String)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()
        _FileNames = FileNames

        _IgnoreEvent = True

        ChkCreated.Checked = False
        ChkLastAccessed.Checked = False
        ChkLFN.Checked = True
        ChkNTExtensions.Checked = False
        chkOverwrite.Checked = True

        _ExistingPathMap = BuildExistingPathMap(CurrentDirectory.Disk.RootDirectory)

        PopulateDirectoryList(CurrentDirectory.Disk.RootDirectory, CurrentDirectory)
        _IgnoreEvent = False

        RefreshFileList()
    End Sub

    Public ReadOnly Property FileList As ImportDirectoryRoot
        Get
            Return _FileList
        End Get
    End Property

    Public Shared Function Display(CurrentDirectory As IDirectory, FileNames() As String) As (Result As Boolean, FileList As ImportDirectoryRoot)
        Using Dlg As New ImportFileForm(CurrentDirectory, FileNames)
            Dim Result = Dlg.ShowDialog(App.CurrentFormInstance)

            Return (Result = DialogResult.OK, Dlg.FileList)
        End Using
    End Function

    Private Shared Function AppendDistinct(baseArray As String(), appendArray As String()) As String()
        Dim setSeen As New HashSet(Of String)(baseArray, StringComparer.OrdinalIgnoreCase)
        Dim result As New List(Of String)(baseArray)

        For Each s In appendArray
            If setSeen.Add(s) Then
                result.Add(s)
            End If
        Next

        Return result.ToArray()
    End Function

    Private Sub AutoSizeComboWidth(cbo As ComboBox, Optional extraPadding As Integer = 6)
        Dim maxWidth As Integer = 0

        Using g = cbo.CreateGraphics()
            For Each item In cbo.Items
                Dim w = g.MeasureString(item.ToString(), cbo.Font).Width
                maxWidth = Math.Max(maxWidth, CInt(Math.Ceiling(w)))
            Next
        End Using

        maxWidth += SystemInformation.VerticalScrollBarWidth
        maxWidth += SystemInformation.BorderSize.Width * 2
        maxWidth += extraPadding

        cbo.Width = maxWidth
    End Sub

    Private Function BuildExistingPathMap(RootDirectory As IDirectory) As Dictionary(Of String, DirectoryEntry)
        Dim Map As New Dictionary(Of String, DirectoryEntry)(StringComparer.OrdinalIgnoreCase)

        PopulateExistingPathMap(RootDirectory, "", Map)

        Return Map
    End Function

    Private Function BuildOptionsFromUI() As AddFileOptions
        Return New AddFileOptions With {
            .UseLFN = ChkLFN.Checked,
            .UseNTExtensions = ChkLFN.Checked AndAlso ChkNTExtensions.Checked,
            .UseCreatedDate = ChkCreated.Checked,
            .UseLastAccessedDate = ChkLastAccessed.Checked,
            .OverwriteExisting = chkOverwrite.Checked
        }
    End Function

    Private Function GetActionLabel(ImportFile As ImportFile) As String
        If ImportFile.IsFileTooLarge OrElse Not ImportFile.IsSelected Then
            Return My.Resources.Action_Skip
        End If

        If Not ImportFile.HasExistingMatch Then
            Return My.Resources.Action_Add
        End If

        If ImportFile.ExistingIsTypeMismatch Then
            Return My.Resources.Action_Rename
        End If

        If _FileList.Options.OverwriteExisting Then
            Return My.Resources.Label_Replace
        End If

        Return My.Resources.Action_Rename
    End Function

    Private Function GetForeColor(IsFileTooLarge As Boolean, IsEnabled As Boolean) As Color
        If IsFileTooLarge Then
            Return Color.Gray
        ElseIf Not IsEnabled Then
            Return Color.Gray
        Else
            Return SystemColors.WindowText
        End If
    End Function

    Private Function GetPathString(Directory As IDirectory) As String
        Dim PathString As String = ""
        Dim Parent As IDirectory
        Dim FileName As String

        If Not Directory.IsRootDirectory Then
            FileName = Directory.ParentEntry.GetFullFileName
            PathString = FileName
            Parent = Directory.ParentEntry.ParentDirectory

            Do While Not Parent.IsRootDirectory
                FileName = Parent.ParentEntry.GetFullFileName
                PathString = FileName & "\" & PathString
                Parent = Parent.ParentEntry.ParentDirectory
            Loop
        End If

        Return PathString
    End Function

    Private Sub GridAddFile(Parent As ImportDirectory, Group As ListViewGroup, GroupPath As String, File As IO.FileInfo)
        Dim SubItem As ListViewItem.ListViewSubItem
        Dim BytesPerCluster = Parent.Root.BytesPerCluster
        Dim RowForeColor As Color
        Dim SizeOnDisk As Long = AlignUp(CULng(File.Length), BytesPerCluster)
        Dim DefaultSelected As Boolean

        Dim ImportFile = Parent.AddFile(File.FullName, File.Name, SizeOnDisk)

        Dim FullPath = If(GroupPath.Length = 0, File.Name, GroupPath & "\" & File.Name)
        Dim Existing = LookupExistingEntry(FullPath)

        If Existing IsNot Nothing Then
            If Existing.IsDirectory OrElse Existing.IsVolumeName Then
                ImportFile.SetExistingMatch(0, 0, True)
            Else
                Dim StartIndex = Existing.ParentDirectory.AdjustIndexForLFN(Existing.Index)
                Dim EntryCount = Existing.Index - StartIndex + 1
                ImportFile.SetExistingMatch(Existing.GetSizeOnDisk, EntryCount, False)
                _HasOverwriteCandidates = True
            End If
        End If

        Dim PreviousIntent As Boolean = False
        Dim HasPrevious As Boolean = _SelectionByPath.TryGetValue(File.FullName, PreviousIntent)
        Dim UserIntent As Boolean = Not HasPrevious OrElse PreviousIntent

        ImportFile.SetUserIntent(UserIntent)

        If ImportFile.IsFileTooLarge Then
            ImportFile.IsSelected = False
            DefaultSelected = False

            RowForeColor = Color.Gray
        Else
            If ImportFile.CanUseNTExtensions Then
                _HasNTExtensions = True
            End If

            If ImportFile.IsLongFileName Then
                _HasLFN = True
            End If

            ImportFile.IsSelected = UserIntent
            DefaultSelected = True

            RowForeColor = SystemColors.WindowText
        End If

        Dim Item As New ListViewItem(Group) With {
            .Text = File.Name,
            .Tag = ImportFile,
            .Checked = ImportFile.IsSelected,
            .ForeColor = RowForeColor,
            .UseItemStyleForSubItems = False
        }

        SubItem = Item.SubItems.Add(GetActionLabel(ImportFile))
        SubItem.ForeColor = RowForeColor
        SubItem.Name = COLUMN_ACTION

        SubItem = Item.SubItems.Add(FormatThousands(File.Length))
        SubItem.ForeColor = RowForeColor

        SubItem = Item.SubItems.Add(FormatThousands(ImportFile.SizeOnDisk))
        SubItem.ForeColor = RowForeColor

        SubItem = Item.SubItems.Add(Format(File.LastWriteTime, My.Resources.IsoDateTimeFormat))
        SubItem.ForeColor = RowForeColor

        SubItem = Item.SubItems.Add(Format(File.CreationTime, My.Resources.IsoDateTimeFormat))
        SubItem.ForeColor = GetForeColor(ImportFile.IsFileTooLarge, ChkCreated.Checked)
        SubItem.Name = COLUMN_CREATION_TIME

        SubItem = Item.SubItems.Add(Format(File.LastAccessTime, My.Resources.IsoDateFormat))
        SubItem.ForeColor = GetForeColor(ImportFile.IsFileTooLarge, ChkLastAccessed.Checked)
        SubItem.Name = COLUMN_LAST_ACCESS_TIME

        SubItem = Item.SubItems.Add(If(DefaultSelected, 0, 1))
        SubItem.Name = COLUMN_IS_SELECTED

        ListViewFiles.Items.Add(Item)
    End Sub

    Private Sub GridAddFolder(Parent As ImportDirectory, GroupPath As String, Folder As DirectoryInfo)
        GroupPath = GroupPath & If(GroupPath = "", "", "\") & Folder.Name

        Dim ImportDirectory = Parent.AddDirectory(Folder.FullName, Folder.Name)

        Dim Existing = LookupExistingEntry(GroupPath)
        If Existing IsNot Nothing Then
            If Existing.IsDirectory Then
                Dim AvailableEntries As Integer = 0
                If Existing.SubDirectory IsNot Nothing Then
                    AvailableEntries = CInt(Existing.SubDirectory.Data.AvailableEntryCount)
                End If
                ImportDirectory.SetExistingMatch(False, AvailableEntries)
            Else
                ImportDirectory.SetExistingMatch(True)
            End If
        End If

        Dim Group As New ListViewGroup(GroupPath) With {
            .Tag = ImportDirectory,
            .Name = GroupPath
        }
        ListViewFiles.Groups.Add(Group)

        ProcessFolders(ImportDirectory, GroupPath, Folder.GetDirectories.ToList)
        ProcessFiles(ImportDirectory, Group, GroupPath, Folder.GetFiles.ToList)
    End Sub

    Private Sub LocalizeForm()
        BtnCancel.Text = WithoutHotkey(My.Resources.Menu_Cancel)
        BtnOK.Text = WithoutHotkey(My.Resources.Label_Import)
        ChkCreated.Text = My.Resources.Label_CreatedDate
        ChkLastAccessed.Text = My.Resources.Label_LastAccessedDate
        ChkLFN.Text = My.Resources.Label_LongFileNames
        ChkNTExtensions.Text = My.Resources.Label_NTExtensions
        chkOverwrite.Text = My.Resources.Label_OverwriteExisting
        FileCreationDate.Text = My.Resources.Label_Created
        FileDisabled.Text = My.Resources.Label_Disabled
        FileLastAccessDate.Text = My.Resources.Label_LastAccessed
        FileLastWriteDate.Text = My.Resources.Label_LastWritten
        FileName.Text = My.Resources.Label_FileName
        Action.Text = My.Resources.Label_Action
        FileSize.Text = My.Resources.Label_Size
        FileSizeOnDisk.Text = My.Resources.Label_SizeOnDisk
        LabelBytesFree.Text = My.Resources.Label_BytesFree & ":"
        LabelBytesRequired.Text = My.Resources.Label_BytesRequired & ":"
        LabelDirectoryList.Text = My.Resources.Label_ImportIntoDirectory
        LabelFilesSelected.Text = My.Resources.Label_FilesSelected & ":"
        LabelOptions.Text = My.Resources.Label_Options
        LabelEntriesFree.Text = My.Resources.Label_EntriesFree & ":"
        LabelEntriesRequired.Text = My.Resources.Label_EntriesRequired & ":"

        Me.Text = My.Resources.Label_ImportFiles
    End Sub

    Private Function LookupExistingEntry(FullPath As String) As DirectoryEntry
        Dim Entry As DirectoryEntry = Nothing
        _ExistingPathMap.TryGetValue(FullPath, Entry)

        Return Entry
    End Function

    Private Sub PopulateDirectoryList(RootDirectory As IDirectory, CurrentDirectory As IDirectory)
        ComboDirectoryList.Items.Clear()

        ProcessDirectoryEntries(RootDirectory, "", CurrentDirectory)

        If ComboDirectoryList.SelectedIndex = -1 AndAlso ComboDirectoryList.Items.Count > 0 Then
            ComboDirectoryList.SelectedIndex = 0
        End If

        _LastSelectedIndex = ComboDirectoryList.SelectedIndex

        AutoSizeComboWidth(ComboDirectoryList)
    End Sub

    Private Sub PopulateExistingPathMap(Directory As IDirectory, Prefix As String, Map As Dictionary(Of String, DirectoryEntry))
        If Directory.Data.EntryCount = 0 Then
            Exit Sub
        End If

        For Counter As UInteger = 0 To Directory.Data.EntryCount - 1
            Dim Entry = Directory.GetFile(Counter)

            If Entry.IsDeleted OrElse Entry.IsLink OrElse Entry.IsLFN Then
                Continue For
            End If

            Dim Name = Entry.GetFullFileName

            If String.IsNullOrEmpty(Name) Then
                Continue For
            End If

            Dim Path = If(Prefix.Length = 0, Name, Prefix & "\" & Name)
            Map(Path) = Entry

            If Entry.IsDirectory AndAlso Entry.SubDirectory IsNot Nothing Then
                PopulateExistingPathMap(Entry.SubDirectory, Path, Map)
            End If
        Next
    End Sub

    Private Sub PopulateFileList(Directory As IDirectory, FileNames() As String)
        Dim GroupPath = GetPathString(Directory)

        Dim FolderList As New List(Of DirectoryInfo)
        Dim FileList As New List(Of IO.FileInfo)

        For Each FilePath In FileNames
            Try
                Dim IsDirectory = File.GetAttributes(FilePath).HasFlag(FileAttributes.Directory)
                If IsDirectory Then
                    FolderList.Add(New DirectoryInfo(FilePath))
                Else
                    FileList.Add(New IO.FileInfo(FilePath))
                End If
            Catch ex As Exception
            End Try
        Next

        FolderList = FolderList.OrderBy(Function(dir) dir.FullName).ToList()
        FileList = FileList.OrderBy(Function(file) file.FullName).ToList()

        _HasNTExtensions = False
        _HasLFN = False
        _HasOverwriteCandidates = False

        ListViewFiles.BeginUpdate()
        ListViewFiles.Items.Clear()
        ListViewFiles.Groups.Clear()

        _FileList = New ImportDirectoryRoot(Directory, BuildOptionsFromUI())

        Dim Group As New ListViewGroup(If(GroupPath = "", InParens(My.Resources.Label_Root), GroupPath)) With {
            .Tag = _FileList
        }

        ListViewFiles.Groups.Add(Group)

        ProcessFiles(_FileList, Group, GroupPath, FileList)
        ProcessFolders(_FileList, GroupPath, FolderList)
        RefreshOptions()

        If ListViewFiles.Items.Count > 0 Then
            ListViewFiles.AutoResizeColumn(FileName.Index, ColumnHeaderAutoResizeStyle.ColumnContent)
            If FileName.Width < 120 Then
                FileName.Width = 120
            End If

            ResizeActionColumn()

            ListViewFiles.AutoResizeColumn(FileSize.Index, ColumnHeaderAutoResizeStyle.ColumnContent)
            If FileSize.Width < 70 Then
                FileSize.Width = 70
            End If
        End If

        ListViewFiles.ListViewItemSorter = New ListViewItemComparer()

        ListViewFiles.EndUpdate()
    End Sub

    Private Sub ProcessDirectoryEntries(Directory As DiskImage.IDirectory, Path As String, CurrentDirectory As IDirectory)
        Dim DirectoryName As String = If(Path = "", InParens(My.Resources.Label_Root), Path)

        Dim Item As New DirectoryComboItem With {
            .Text = DirectoryName,
            .Directory = Directory
        }

        Dim Index As Integer = ComboDirectoryList.Items.Add(Item)

        If Directory Is CurrentDirectory Then
            ComboDirectoryList.SelectedIndex = Index
        End If

        For Counter = 0 To Directory.Data.EntryCount - 1
            Dim DirectoryEntry = Directory.GetFile(Counter)
            If Not DirectoryEntry.IsLink Then
                If DirectoryEntry.IsDirectory AndAlso DirectoryEntry.SubDirectory IsNot Nothing Then
                    Dim NewPath = DirectoryEntry.GetFullFileName

                    If Path <> "" Then
                        NewPath = Path & "\" & NewPath
                    End If

                    ProcessDirectoryEntries(DirectoryEntry.SubDirectory, NewPath, CurrentDirectory)
                End If
            End If
        Next
    End Sub

    Private Sub ProcessFiles(Parent As ImportDirectory, Group As ListViewGroup, GroupPath As String, FileList As List(Of IO.FileInfo))
        For Each File In FileList
            GridAddFile(Parent, Group, GroupPath, File)
        Next
    End Sub

    Private Sub ProcessFolders(Parent As ImportDirectory, GroupPath As String, FolderList As List(Of DirectoryInfo))
        For Each Folder In FolderList
            GridAddFolder(Parent, GroupPath, Folder)
        Next
    End Sub

    Private Sub RefreshCreated()
        For Each Item As ListViewItem In ListViewFiles.Items
            Dim FileData As ImportFile = Item.Tag

            Item.SubItems.Item(COLUMN_CREATION_TIME).ForeColor = GetForeColor(FileData.IsFileTooLarge, ChkCreated.Checked)
        Next
    End Sub

    Private Sub RefreshFileList()
        SaveSelections()

        Dim Item = TryCast(ComboDirectoryList.SelectedItem, DirectoryComboItem)

        If Item Is Nothing Then
            Exit Sub
        End If

        _IgnoreEvent = True
        PopulateFileList(Item.Directory, _FileNames)
        RefreshTotals()
        _IgnoreEvent = False
    End Sub

    Private Sub RefreshItemStates()
        Dim WasIgnoring = _IgnoreEvent
        _IgnoreEvent = True

        Try
            For Each Item As ListViewItem In ListViewFiles.Items
                Dim FileData As ImportFile = Item.Tag
                Dim IsTooLarge As Boolean = FileData.IsFileTooLarge
                Dim DesiredSelected As Boolean = Not IsTooLarge AndAlso FileData.UserIntendsSelected

                If FileData.IsSelected <> DesiredSelected Then
                    FileData.IsSelected = DesiredSelected
                End If

                If Item.Checked <> DesiredSelected Then
                    Item.Checked = DesiredSelected
                End If

                Dim RowForeColor As Color = If(IsTooLarge, Color.Gray, SystemColors.WindowText)

                If Item.ForeColor <> RowForeColor Then
                    Item.ForeColor = RowForeColor
                End If

                For i = 1 To Item.SubItems.Count - 1
                    Dim SubItem = Item.SubItems(i)
                    Dim TargetColor As Color

                    If SubItem.Name = COLUMN_CREATION_TIME Then
                        TargetColor = GetForeColor(IsTooLarge, ChkCreated.Checked)
                    ElseIf SubItem.Name = COLUMN_LAST_ACCESS_TIME Then
                        TargetColor = GetForeColor(IsTooLarge, ChkLastAccessed.Checked)
                    Else
                        TargetColor = RowForeColor
                    End If

                    If SubItem.ForeColor <> TargetColor Then
                        SubItem.ForeColor = TargetColor
                    End If
                Next

                Dim ActionSubItem = Item.SubItems.Item(COLUMN_ACTION)
                Dim NewLabel = GetActionLabel(FileData)

                If ActionSubItem.Text <> NewLabel Then
                    ActionSubItem.Text = NewLabel
                End If
            Next
        Finally
            _IgnoreEvent = WasIgnoring
        End Try
    End Sub

    Private Sub RefreshLastAccessed()
        For Each Item As ListViewItem In ListViewFiles.Items
            Dim FileData As ImportFile = Item.Tag

            Item.SubItems.Item(COLUMN_LAST_ACCESS_TIME).ForeColor = GetForeColor(FileData.IsFileTooLarge, ChkLastAccessed.Checked)
        Next
    End Sub

    Private Sub RefreshOptions()
        Dim Forced As Boolean = False

        ChkLFN.Enabled = True
        If Not _HasLFN AndAlso ChkLFN.Checked Then
            ChkLFN.Checked = False
            Forced = True
        End If

        ChkNTExtensions.Enabled = _HasLFN AndAlso ChkLFN.Checked
        If Not _HasLFN AndAlso ChkNTExtensions.Checked Then
            ChkNTExtensions.Checked = False
            Forced = True
        End If

        chkOverwrite.Enabled = _HasOverwriteCandidates

        If Forced Then
            SyncOptionsFromUI(_FileList.Options)
        End If
    End Sub

    Private Sub RefreshTotals()
        Dim BytesRequired = _FileList.TotalSpaceRequired
        Dim EntriesRequired = _FileList.EntriesRequired

        LblSelected.Text = FormatThousands(_FileList.SelectedFiles) & " of " & FormatThousands(ListViewFiles.Items.Count)
        LblBytesRequired.Text = FormatThousands(BytesRequired)
        LblBytesFree.Text = FormatThousands(_FileList.FreeSpace)

        Dim FitsBytes = BytesRequired <= _FileList.FreeSpace
        Dim FitsEntries = Not _FileList.IsEntryOverflow

        LblBytesRequired.ForeColor = If(FitsBytes, SystemColors.WindowText, Color.Red)

        Dim ShowEntries = Not _FileList.CanExpand

        LabelEntriesRequired.Visible = ShowEntries
        LabelEntriesFree.Visible = ShowEntries
        lblEntriesRequired.Visible = ShowEntries
        lblEntriesFree.Visible = ShowEntries

        If ShowEntries Then
            lblEntriesRequired.Text = FormatThousands(EntriesRequired)
            lblEntriesFree.Text = FormatThousands(_FileList.AvailableEntries)
            lblEntriesRequired.ForeColor = If(FitsEntries, SystemColors.WindowText, Color.Red)
        End If

        BtnOK.Enabled = _FileList.SelectedFiles > 0 AndAlso FitsBytes AndAlso FitsEntries
    End Sub

    Private Sub ResizeActionColumn()
        Dim Candidates = New String() {
            Action.Text,
            My.Resources.Action_Add,
            My.Resources.Action_Rename,
            My.Resources.Action_Skip,
            My.Resources.Label_Replace
        }
        Dim MaxWidth As Integer = 0

        For Each Value In Candidates
            Dim w = TextRenderer.MeasureText(Value, ListViewFiles.Font).Width

            If w > MaxWidth Then
                MaxWidth = w
            End If
        Next

        Action.Width = MaxWidth + 16
    End Sub

    Private Sub SaveSelections()
        If _FileList Is Nothing Then
            Exit Sub
        End If

        _SelectionByPath.Clear()
        SaveSelectionsFromDirectory(_FileList)
    End Sub

    Private Sub SaveSelectionsFromDirectory(dir As ImportDirectory)
        ' Files in this directory
        For Each f In dir.FileList
            _SelectionByPath(f.FilePath) = f.UserIntendsSelected
        Next

        ' Recurse into subdirectories
        For Each subDir In dir.DirectoryList
            SaveSelectionsFromDirectory(subDir)
        Next
    End Sub

    Private Sub SyncOptionsFromUI(Options As AddFileOptions)
        Options.UseLFN = ChkLFN.Checked
        Options.UseNTExtensions = ChkLFN.Checked AndAlso ChkNTExtensions.Checked
        Options.UseCreatedDate = ChkCreated.Checked
        Options.UseLastAccessedDate = ChkLastAccessed.Checked
        Options.OverwriteExisting = chkOverwrite.Checked
    End Sub
#Region "Events"
    Private Sub ChkCreated_CheckedChanged(sender As Object, e As EventArgs) Handles ChkCreated.CheckedChanged
        If _IgnoreEvent Then Exit Sub

        _FileList.Options.UseCreatedDate = ChkCreated.Checked
        RefreshCreated()
    End Sub

    Private Sub ChkLastAccessed_CheckedChanged(sender As Object, e As EventArgs) Handles ChkLastAccessed.CheckedChanged
        If _IgnoreEvent Then Exit Sub

        _FileList.Options.UseLastAccessedDate = ChkLastAccessed.Checked

        RefreshLastAccessed()
    End Sub

    Private Sub ChkLFN_CheckedChanged(sender As Object, e As EventArgs) Handles ChkLFN.CheckedChanged
        If _IgnoreEvent Then Exit Sub

        ChkNTExtensions.Enabled = ChkLFN.Checked
        _FileList.SetOptions(ChkLFN.Checked, ChkLFN.Checked And ChkNTExtensions.Checked)

        RefreshTotals()
    End Sub

    Private Sub ChkNTExtensions_CheckedChanged(sender As Object, e As EventArgs) Handles ChkNTExtensions.CheckedChanged
        If _IgnoreEvent Then Exit Sub

        _FileList.SetOptions(ChkLFN.Checked, ChkLFN.Checked And ChkNTExtensions.Checked)

        RefreshTotals()
    End Sub

    Private Sub ChkOverwrite_CheckedChanged(sender As Object, e As EventArgs) Handles chkOverwrite.CheckedChanged
        If _IgnoreEvent Then Exit Sub

        _FileList.SetOverwriteExisting(chkOverwrite.Checked)

        _IgnoreEvent = True
        ListViewFiles.BeginUpdate()
        Try
            RefreshItemStates()
            RefreshTotals()
        Finally
            ListViewFiles.EndUpdate()
            _IgnoreEvent = False
        End Try
    End Sub

    Private Sub ComboDirectoryList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboDirectoryList.SelectedIndexChanged
        If _IgnoreEvent Then Exit Sub

        If _LastSelectedIndex = ComboDirectoryList.SelectedIndex Then
            Exit Sub
        End If

        _LastSelectedIndex = ComboDirectoryList.SelectedIndex

        RefreshFileList()
    End Sub

    Private Sub ImportFileForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        ListViewFiles.DoubleBuffer
        _ListViewHeader = New ListViewHeader(ListViewFiles.Handle)
    End Sub

    Private Sub ListViewFiles_ColumnWidthChanging(sender As Object, e As ColumnWidthChangingEventArgs) Handles ListViewFiles.ColumnWidthChanging
        e.NewWidth = Me.ListViewFiles.Columns(e.ColumnIndex).Width
        e.Cancel = True
    End Sub

    Private Sub ListViewFiles_DragDrop(sender As Object, e As DragEventArgs) Handles ListViewFiles.DragDrop
        If Not e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Exit Sub
        End If

        Dim Files As String() = TryCast(e.Data.GetData(DataFormats.FileDrop), String())

        If Files Is Nothing OrElse Files.Length = 0 Then
            Exit Sub
        End If

        _FileNames = AppendDistinct(_FileNames, Files)

        RefreshFileList()
    End Sub

    Private Sub ListViewFiles_DragEnter(sender As Object, e As DragEventArgs) Handles ListViewFiles.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub ListViewFiles_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles ListViewFiles.ItemCheck
        If _IgnoreEvent Then Exit Sub

        If e.NewValue = e.CurrentValue Then
            Exit Sub
        End If

        Dim Item As ListViewItem = ListViewFiles.Items(e.Index)

        Dim FileData As ImportFile = Item.Tag

        If FileData.IsFileTooLarge Then
            If e.NewValue Then
                e.NewValue = False
            End If
        Else
            FileData.SetUserIntent(e.NewValue = CheckState.Checked)
            FileData.IsSelected = e.NewValue
        End If

        Item.SubItems.Item(COLUMN_ACTION).Text = GetActionLabel(FileData)

        RefreshTotals()
    End Sub
#End Region

    Public Class ImportDirectory
        Inherits ImportFileBase

        Private ReadOnly _AvailableEntries As Integer
        Private ReadOnly _DirectoryList As List(Of ImportDirectory)
        Private ReadOnly _FileList As List(Of ImportFile)
        Private ReadOnly _Root As ImportDirectoryRoot
        Private _EntriesRequired As Integer
        Private _ExistingAvailableEntries As Integer
        Private _ExistingIsFile As Boolean
        Private _HasExistingMatch As Boolean
        Private _SelectedFiles As Integer

        Public Sub New(FilePath As String, FileName As String, Root As ImportDirectoryRoot, Parent As ImportDirectory)
            MyBase.New(FilePath, FileName, Parent)

            _Root = Root
            _FileList = New List(Of ImportFile)
            _DirectoryList = New List(Of ImportDirectory)
            _SelectedFiles = 0
            _EntriesRequired = 0
            _AvailableEntries = 0
        End Sub

        Public Sub New(AvailableEntries As Integer)
            MyBase.New("", "", Nothing)

            _Root = Nothing
            _FileList = New List(Of ImportFile)
            _DirectoryList = New List(Of ImportDirectory)
            _SelectedFiles = 0
            _EntriesRequired = 0
            _AvailableEntries = AvailableEntries
        End Sub

        Public ReadOnly Property AvailableEntries As Integer
            Get
                Return _AvailableEntries
            End Get
        End Property

        Public ReadOnly Property DirectoryList As List(Of ImportDirectory)
            Get
                Return _DirectoryList
            End Get
        End Property

        Public Property EntriesRequired As Integer
            Get
                Return _EntriesRequired
            End Get
            Set(value As Integer)
                If _EntriesRequired <> value Then
                    Dim PrevRequiredBytes = GetRequiredBytes()

                    _EntriesRequired = value

                    Dim Diff = GetRequiredBytes() - PrevRequiredBytes

                    If Diff <> 0 Then
                        If Root IsNot Nothing Then
                            Root.TotalSpaceRequired += Diff
                        End If
                    End If
                End If
            End Set
        End Property

        Public ReadOnly Property ExistsOnDisk As Boolean
            Get
                Return _HasExistingMatch AndAlso Not _ExistingIsFile
            End Get
        End Property

        Public ReadOnly Property FileList As List(Of ImportFile)
            Get
                Return _FileList
            End Get
        End Property

        Public ReadOnly Property Root As ImportDirectoryRoot
            Get
                If _Root Is Nothing Then
                    Return Me
                Else
                    Return _Root
                End If
            End Get
        End Property

        Public Property SelectedFiles As Integer
            Get
                Return _SelectedFiles
            End Get
            Set(value As Integer)
                If _SelectedFiles <> value Then
                    Dim PrevRequiredEntries As Integer = 0
                    Dim RequiredEntries As Integer = 0

                    If _SelectedFiles > 0 Then
                        PrevRequiredEntries = GetRequiredEntries(Root.Options.UseLFN, Root.Options.UseNTExtensions)
                    End If

                    _SelectedFiles = value

                    If _SelectedFiles > 0 Then
                        RequiredEntries = GetRequiredEntries(Root.Options.UseLFN, Root.Options.UseNTExtensions)
                    End If

                    If Parent IsNot Nothing AndAlso Not ExistsOnDisk Then
                        Dim EntriesRequired = RequiredEntries - PrevRequiredEntries
                        If EntriesRequired <> 0 Then
                            Parent.EntriesRequired += EntriesRequired
                        End If
                    End If
                End If
            End Set
        End Property

        Public Function AddDirectory(FilePath As String, FileName As String) As ImportDirectory
            Dim NewDirectory As New ImportDirectory(FilePath, FileName, Root, Me)
            _DirectoryList.Add(NewDirectory)

            Return NewDirectory
        End Function

        Public Function AddFile(FilePath As String, FileName As String, SizeOnDisk As Long) As ImportFile
            Dim NewFile As New ImportFile(FilePath, FileName, SizeOnDisk, Root, Me)
            _FileList.Add(NewFile)

            Return NewFile
        End Function

        Public Sub RefreshRequiredEntries(UseLFN As Boolean, UseNTExtensions As Boolean)
            If _SelectedFiles > 0 AndAlso Not ExistsOnDisk Then
                Dim RequiredEntries = GetRequiredEntries(Root.Options.UseLFN, Root.Options.UseNTExtensions)
                Dim NewRequiredEntries = GetRequiredEntries(UseLFN, UseNTExtensions)
                Dim Diff = NewRequiredEntries - RequiredEntries

                If Diff <> 0 Then
                    If Parent IsNot Nothing Then
                        Parent.EntriesRequired += Diff
                    End If
                End If
            End If
        End Sub

        Friend Sub SetExistingMatch(ExistingIsFile As Boolean, Optional ExistingAvailableEntries As Integer = 0)
            _ExistingIsFile = ExistingIsFile
            _ExistingAvailableEntries = ExistingAvailableEntries
            _HasExistingMatch = True
        End Sub

        Private Function GetRequiredBytes() As Integer
            Dim RequiredBytes As Integer = 0
            Dim AvailableEntries As Integer
            Dim Overhead As Integer = 0

            If _Root Is Nothing Then
                AvailableEntries = _AvailableEntries
            ElseIf ExistsOnDisk Then
                AvailableEntries = _ExistingAvailableEntries
            Else
                AvailableEntries = 0

                If _EntriesRequired > 0 Then
                    Overhead = 2
                End If
            End If

            Dim EntryCount As Integer = _EntriesRequired - AvailableEntries + Overhead

            If EntryCount > 0 Then
                If _Root Is Nothing AndAlso Not Root.CanExpand Then
                    Return 0
                End If

                Dim EntriesPerCluster As UInteger = Root.BytesPerCluster \ 32
                RequiredBytes = CeilDiv(CUInt(EntryCount), EntriesPerCluster) * Root.BytesPerCluster
            End If

            Return RequiredBytes
        End Function
    End Class

    Public Class ImportDirectoryRoot
        Inherits ImportDirectory

        Private ReadOnly _BytesPerCluster As UInteger
        Private ReadOnly _CanExpand As Boolean
        Private ReadOnly _CurrentDirectory As IDirectory
        Private ReadOnly _FreeSpace As UInteger
        Private ReadOnly _Options As AddFileOptions
        Private _TotalSpaceRequired As Long

        Public Sub New(CurrentDirectory As IDirectory, Options As AddFileOptions)
            MyBase.New(CurrentDirectory.Data.AvailableEntryCount)

            _CurrentDirectory = CurrentDirectory
            _BytesPerCluster = CurrentDirectory.Disk.BPB.BytesPerCluster
            _FreeSpace = CurrentDirectory.Disk.FAT.GetFreeSpace
            _CanExpand = Not CurrentDirectory.IsRootDirectory

            _Options = Options

            _TotalSpaceRequired = 0
        End Sub

        Friend ReadOnly Property BytesPerCluster As UInteger
            Get
                Return _BytesPerCluster
            End Get
        End Property

        Friend ReadOnly Property CanExpand As Boolean
            Get
                Return _CanExpand
            End Get
        End Property

        Friend ReadOnly Property CurrentDirectory As IDirectory
            Get
                Return _CurrentDirectory
            End Get
        End Property

        Friend ReadOnly Property FreeSpace As UInteger
            Get
                Return _FreeSpace
            End Get
        End Property

        Friend ReadOnly Property IsEntryOverflow As Boolean
            Get
                If _CanExpand Then
                    Return False
                End If

                Return EntriesRequired > AvailableEntries
            End Get
        End Property

        Friend ReadOnly Property Options As AddFileOptions
            Get
                Return _Options
            End Get
        End Property

        Friend Property TotalSpaceRequired As Long
            Get
                Return _TotalSpaceRequired
            End Get
            Set(value As Long)
                _TotalSpaceRequired = value
            End Set
        End Property

        Friend Sub SetOptions(LFN As Boolean, NTExtensions As Boolean)

            RefreshDirectoryList(DirectoryList, LFN, NTExtensions)
            RefreshFileList(FileList, LFN, NTExtensions)

            _Options.UseLFN = LFN
            _Options.UseNTExtensions = NTExtensions
        End Sub

        Friend Sub SetOverwriteExisting(NewValue As Boolean)
            If _Options.OverwriteExisting = NewValue Then Return

            ApplyOverwriteChange(Me, NewValue)

            _Options.OverwriteExisting = NewValue
        End Sub

        Private Sub ApplyOverwriteChange(Dir As ImportDirectory, NewOverwrite As Boolean)
            For Each File In Dir.FileList
                File.HandleOverwriteChange(NewOverwrite)
            Next
            For Each SubDir In Dir.DirectoryList
                ApplyOverwriteChange(SubDir, NewOverwrite)
            Next
        End Sub

        Private Sub RefreshDirectoryList(DirectoryList As List(Of ImportDirectory), LFN As Boolean, NTExtensions As Boolean)
            For Each Directory In DirectoryList
                If Directory.SelectedFiles > 0 Then
                    Directory.RefreshRequiredEntries(LFN, NTExtensions)
                End If

                RefreshDirectoryList(Directory.DirectoryList, LFN, NTExtensions)
                RefreshFileList(Directory.FileList, LFN, NTExtensions)
            Next
        End Sub

        Private Sub RefreshFileList(FileList As List(Of ImportFile), LFN As Boolean, NTExtensions As Boolean)
            For Each File In FileList
                If File.IsSelected Then
                    File.HandleLFNChange(LFN, NTExtensions)
                End If
            Next
        End Sub
    End Class

    Public Class ImportFile
        Inherits ImportFileBase

        Private ReadOnly _Root As ImportDirectoryRoot
        Private ReadOnly _SizeOnDisk As Long
        Private _ExistingEntryCount As Integer = 0
        Private _ExistingIsDirectory As Boolean = False
        Private _ExistingSizeOnDisk As Long = 0
        Private _HasExistingMatch As Boolean = False
        Private _IsSelected As Boolean
        Private _Skip As Boolean = False
        Private _UserIntendsSelected As Boolean = True

        Public Sub New(FilePath As String, FileName As String, SizeOnDisk As Long, Root As ImportDirectoryRoot, Parent As ImportDirectory)
            MyBase.New(FilePath, FileName, Parent)

            _Root = Root
            _IsSelected = False
            _SizeOnDisk = SizeOnDisk
        End Sub

        Friend ReadOnly Property ExistingEntryCount As Integer
            Get
                Return _ExistingEntryCount
            End Get
        End Property

        Friend ReadOnly Property ExistingIsDirectory As Boolean
            Get
                Return _ExistingIsDirectory
            End Get
        End Property

        Friend ReadOnly Property ExistingIsTypeMismatch As Boolean
            Get
                Return _HasExistingMatch AndAlso _ExistingIsDirectory
            End Get
        End Property

        Friend ReadOnly Property HasExistingMatch As Boolean
            Get
                Return _HasExistingMatch
            End Get
        End Property

        Friend ReadOnly Property IsFileTooLarge As Boolean
            Get
                Return EffectiveSizeOnDisk(_Root.Options.OverwriteExisting) > _Root.FreeSpace
            End Get
        End Property

        Friend Property IsSelected As Boolean
            Get
                Return _IsSelected
            End Get
            Set(value As Boolean)
                If _IsSelected <> value Then
                    _IsSelected = value

                    HandleSelectedChange(value)
                End If
            End Set
        End Property

        Friend ReadOnly Property Root As ImportDirectoryRoot
            Get
                Return _Root
            End Get
        End Property

        Friend ReadOnly Property SizeOnDisk As Long
            Get
                Return _SizeOnDisk
            End Get
        End Property

        Friend Property Skip As Boolean
            Get
                Return _Skip
            End Get
            Set(value As Boolean)
                _Skip = value
            End Set
        End Property

        Friend ReadOnly Property UserIntendsSelected As Boolean
            Get
                Return _UserIntendsSelected
            End Get
        End Property

        Friend Sub HandleLFNChange(UseLFN As Boolean, UseNTExtensions As Boolean)
            If Not _IsSelected Then
                Return
            End If

            If UseLFN = Root.Options.UseLFN AndAlso UseNTExtensions = Root.Options.UseNTExtensions Then
                Return
            End If

            Dim Overwrite = _Root.Options.OverwriteExisting
            Dim PrevEntries = EffectiveRequiredEntries(Overwrite, Root.Options.UseLFN, Root.Options.UseNTExtensions)
            Dim NewEntries = EffectiveRequiredEntries(Overwrite, UseLFN, UseNTExtensions)
            Dim Diff = NewEntries - PrevEntries

            If Diff <> 0 Then
                Parent.EntriesRequired += Diff
            End If
        End Sub

        Friend Sub HandleOverwriteChange(NewOverwrite As Boolean)
            If Not _IsSelected Then
                Return
            End If

            If Not _HasExistingMatch Then
                Return
            End If

            If _ExistingIsDirectory Then
                Return
            End If

            Dim OldOverwrite = _Root.Options.OverwriteExisting

            If OldOverwrite = NewOverwrite Then
                Return
            End If

            Dim SizeDiff = EffectiveSizeOnDisk(NewOverwrite) - EffectiveSizeOnDisk(OldOverwrite)
            Dim PrevEntries = EffectiveRequiredEntries(OldOverwrite, _Root.Options.UseLFN, _Root.Options.UseNTExtensions)
            Dim NewEntries = EffectiveRequiredEntries(NewOverwrite, _Root.Options.UseLFN, _Root.Options.UseNTExtensions)
            Dim Diff = NewEntries - PrevEntries


            If Diff <> 0 AndAlso Parent IsNot Nothing Then
                Parent.EntriesRequired += Diff
            End If

            If SizeDiff <> 0 Then
                _Root.TotalSpaceRequired += SizeDiff
            End If
        End Sub

        Friend Sub SetExistingMatch(SizeOnDisk As Long, EntryCount As Integer, ExistingIsDirectory As Boolean)
            _ExistingSizeOnDisk = SizeOnDisk
            _ExistingEntryCount = EntryCount
            _ExistingIsDirectory = ExistingIsDirectory
            _HasExistingMatch = True
        End Sub

        Friend Sub SetUserIntent(value As Boolean)
            _UserIntendsSelected = value
        End Sub

        Private Function EffectiveRequiredEntries(Overwrite As Boolean, UseLFN As Boolean, UseNTExtensions As Boolean) As Integer
            Dim Required = GetRequiredEntries(UseLFN, UseNTExtensions)

            If Overwrite AndAlso _HasExistingMatch AndAlso Not _ExistingIsDirectory Then
                Return Required - _ExistingEntryCount
            End If

            Return Required
        End Function

        Private Function EffectiveSizeOnDisk(Overwrite As Boolean) As Long
            If Overwrite AndAlso _HasExistingMatch AndAlso Not _ExistingIsDirectory Then
                Return _SizeOnDisk - _ExistingSizeOnDisk
            End If

            Return _SizeOnDisk
        End Function

        Private Sub HandleSelectedChange(IsSelected As Boolean)
            Dim Diff = If(IsSelected, 1, -1)

            Dim Overwrite = _Root.Options.OverwriteExisting
            Dim EffEntries = EffectiveRequiredEntries(Overwrite, Root.Options.UseLFN, Root.Options.UseNTExtensions)
            Dim EffSize = EffectiveSizeOnDisk(Overwrite)

            Dim Directory = Parent

            If Directory IsNot Nothing Then
                Directory.EntriesRequired += (EffEntries * Diff)

                Do
                    Directory.SelectedFiles += Diff
                    Directory = Directory.Parent
                Loop Until Directory Is Nothing
            End If

            If _Root IsNot Nothing Then
                _Root.TotalSpaceRequired += (EffSize * Diff)
            End If

        End Sub
    End Class

    Public MustInherit Class ImportFileBase
        Private ReadOnly _CanUseNTExtensions As Boolean
        Private ReadOnly _FileName As String
        Private ReadOnly _FilePath As String
        Private ReadOnly _IsLongFileName As Boolean
        Private ReadOnly _Parent As ImportDirectory

        Friend Sub New(FilePath As String, FileName As String, Parent As ImportDirectory)
            _Parent = Parent
            _FilePath = FilePath
            _FileName = FileName
            _IsLongFileName = CalcIsLongFileName()
            If _IsLongFileName Then
                _CanUseNTExtensions = CalcCanUseNTExtensions()
            Else
                _CanUseNTExtensions = False
            End If
        End Sub

        Friend ReadOnly Property CanUseNTExtensions As Boolean
            Get
                Return _CanUseNTExtensions
            End Get
        End Property

        Friend ReadOnly Property FileName As String
            Get
                Return _FileName
            End Get
        End Property

        Friend ReadOnly Property FilePath As String
            Get
                Return _FilePath
            End Get
        End Property

        Friend ReadOnly Property IsLongFileName As Boolean
            Get
                Return _IsLongFileName
            End Get
        End Property

        Friend ReadOnly Property Parent As ImportDirectory
            Get
                Return _Parent
            End Get
        End Property

        Friend Function GetRequiredEntries(UseLFN As Boolean, UseNTExtensions As Boolean) As Integer
            If _FileName.Length = 0 Then
                Return 0
            End If

            Dim EntriesRequired As Integer = 1

            If Not UseNTExtensions Or Not _CanUseNTExtensions Then
                If UseLFN And _IsLongFileName Then
                    EntriesRequired += CeilDiv(CUInt(_FileName.Length * 2), 26)
                End If
            End If

            Return EntriesRequired
        End Function

        Private Function CalcCanUseNTExtensions() As Boolean
            Dim FileParts = SplitFilename(_FileName)

            If FileParts.Name.Length > 8 Or FileParts.Extension.Length > 3 Then
                Return False
            End If

            Dim CleanFileName = DOSCleanFileName(FileParts.Name)
            Dim CleanExtension = DOSCleanFileName(FileParts.Extension)

            If CleanFileName <> FileParts.Name.ToUpper OrElse CleanExtension <> FileParts.Extension.ToUpper Then
                Return False
            End If

            Dim NameQualifies = FileParts.Name.Length > 0 AndAlso FileParts.Name = FileParts.Name.ToLower
            Dim ExtQualifies = FileParts.Extension.Length > 0 AndAlso FileParts.Extension = FileParts.Extension.ToLower

            Return NameQualifies OrElse ExtQualifies
        End Function

        Private Function CalcIsLongFileName() As Boolean
            Dim FileParts = SplitFilename(_FileName)

            If FileParts.Name.Length > 8 Or FileParts.Extension.Length > 3 Then
                Return True
            Else
                Dim CleanFileName = DOSCleanFileName(FileParts.Name)
                Dim CleanExtension = DOSCleanFileName(FileParts.Extension)

                If CleanFileName <> FileParts.Name Or CleanExtension <> FileParts.Extension Then
                    Return True
                End If
            End If

            Return False
        End Function
    End Class

    Private Class DirectoryComboItem
        Public Property Directory As IDirectory
        Public Property Text As String
        Public Overrides Function ToString() As String
            Return Text
        End Function
    End Class

    Private Class ListViewItemComparer
        Implements IComparer

        Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
            Dim Value1 As Integer = DirectCast(x, ListViewItem).SubItems(COLUMN_IS_SELECTED).Text
            Dim Value2 As Integer = DirectCast(y, ListViewItem).SubItems(COLUMN_IS_SELECTED).Text

            Return Value1.CompareTo(Value2)
        End Function
    End Class
End Class