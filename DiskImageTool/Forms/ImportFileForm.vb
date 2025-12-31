Imports System.IO
Imports DiskImageTool.DiskImage

Public Class ImportFileForm
    Private Const COLUMN_CREATION_TIME As String = "CreationTime"
    Private Const COLUMN_IS_SELECTED As String = "IsSelected"
    Private Const COLUMN_LAST_ACCESS_TIME As String = "LastAccessTime"
    Private ReadOnly _FileNames() As String
    Private ReadOnly _SelectionByPath As New Dictionary(Of String, Boolean)
    Private _FileList As ImportDirectoryRoot
    Private _HasLFN As Boolean
    Private _HasNTExtensions As Boolean
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

    Public Function GetAvailableFileName(FileName As String, UseNTExtensions As Boolean, ExistingFiles As HashSet(Of String)) As String
        Dim FileParts = SplitFilename(FileName)

        Dim CleanFileName = DOSCleanFileName(FileParts.Name)
        Dim CleanExtension = DOSCleanFileName(FileParts.Extension, 3)

        Dim Checksum As String = ""

        If UseNTExtensions Then
            Checksum = GetShortFileChecksumString(FileName)
        End If

        Dim Index As UInteger = 1
        Dim NewFileName As String

        If CleanFileName.Length > 8 Or CleanFileName.Length <> FileParts.Name.Length Then
            NewFileName = TruncateFileName(CleanFileName, CleanExtension, Checksum, Index, UseNTExtensions)
            Index += 1
        Else
            NewFileName = CombineFileParts(CleanFileName, CleanExtension)
        End If

        Do While ExistingFiles.Contains(NewFileName)
            NewFileName = TruncateFileName(CleanFileName, CleanExtension, Checksum, Index, UseNTExtensions)
            Index += 1
        Loop

        Return NewFileName
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

    Private Sub ComboDirectoryList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboDirectoryList.SelectedIndexChanged
        If _IgnoreEvent Then Exit Sub

        If _LastSelectedIndex = ComboDirectoryList.SelectedIndex Then
            Exit Sub
        End If

        _LastSelectedIndex = ComboDirectoryList.SelectedIndex

        RefreshFileList()
    End Sub

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

    Private Sub GridAddFile(Parent As ImportDirectory, Group As ListViewGroup, File As IO.FileInfo)
        Dim SubItem As ListViewItem.ListViewSubItem
        Dim BytesPerCluster = Parent.Root.BytesPerCluster
        Dim RowForeColor As Color
        Dim SizeOnDisk As Long = AlignUp(CULng(File.Length), BytesPerCluster)
        Dim DefaultSelected As Boolean

        Dim ImportFile = Parent.AddFile(File.FullName, File.Name, SizeOnDisk)

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

            Dim PreviousSelected As Boolean = False
            Dim HasPrevious As Boolean = _SelectionByPath.TryGetValue(File.FullName, PreviousSelected)

            ImportFile.IsSelected = Not HasPrevious OrElse PreviousSelected
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

        'Dim ShortName As String = ""
        'If ImportFile.IsSelected Then
        '    ShortName = GetAvailableFileName(ImportFile.FileName, False, Parent.FileNames)
        '    Parent.FileNames.Add(ShortName)
        'End If

        'SubItem = Item.SubItems.Add(ShortName)
        'SubItem.ForeColor = RowForeColor
        'SubItem.Name = "ShortName"

        SubItem = Item.SubItems.Add(FormatThousands(File.Length))
        SubItem.ForeColor = RowForeColor

        SubItem = Item.SubItems.Add(FormatThousands(ImportFile.SizeOnDisk))
        SubItem.ForeColor = RowForeColor

        SubItem = Item.SubItems.Add(Format(File.LastWriteTime, My.Resources.IsoDateTimeFormat))
        SubItem.ForeColor = RowForeColor

        SubItem = Item.SubItems.Add(Format(File.CreationTime, My.Resources.IsoDateTimeFormat))
        SubItem.ForeColor = Color.Gray
        SubItem.Name = COLUMN_CREATION_TIME

        SubItem = Item.SubItems.Add(Format(File.LastAccessTime, My.Resources.IsoDateFormat))
        SubItem.ForeColor = Color.Gray
        SubItem.Name = COLUMN_LAST_ACCESS_TIME

        SubItem = Item.SubItems.Add(If(DefaultSelected, 0, 1))
        SubItem.Name = COLUMN_IS_SELECTED

        ListViewFiles.Items.Add(Item)
    End Sub

    Private Sub GridAddFolder(Parent As ImportDirectory, GroupPath As String, Folder As DirectoryInfo)
        GroupPath = GroupPath & If(GroupPath = "", "", "\") & Folder.Name

        Dim ImportDirectory = Parent.AddDirectory(Folder.FullName, Folder.Name)

        'Dim ShortName = GetAvailableFileName(ImportDirectory.FileName, False, Parent.FileNames)
        'Parent.FileNames.Add(ShortName)

        Dim Group As New ListViewGroup(GroupPath) With {
            .Tag = ImportDirectory
        }
        ListViewFiles.Groups.Add(Group)

        ProcessFolders(ImportDirectory, GroupPath, Folder.GetDirectories.ToList)
        ProcessFiles(ImportDirectory, Group, Folder.GetFiles.ToList)
    End Sub

    Private Sub ImportFileForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        ListViewFiles.DoubleBuffer
        _ListViewHeader = New ListViewHeader(ListViewFiles.Handle)
    End Sub

    Private Sub ListViewFiles_ColumnWidthChanging(sender As Object, e As ColumnWidthChangingEventArgs) Handles ListViewFiles.ColumnWidthChanging
        e.NewWidth = Me.ListViewFiles.Columns(e.ColumnIndex).Width
        e.Cancel = True
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
            FileData.IsSelected = e.NewValue
        End If

        RefreshTotals()
    End Sub

    Private Sub LocalizeForm()
        BtnCancel.Text = WithoutHotkey(My.Resources.Menu_Cancel)
        BtnOK.Text = WithoutHotkey(My.Resources.Label_Import)
        ChkCreated.Text = My.Resources.Label_CreatedDate
        ChkLastAccessed.Text = My.Resources.Label_LastAccessedDate
        ChkLFN.Text = My.Resources.Label_LongFileNames
        ChkNTExtensions.Text = My.Resources.Label_NTExtensions
        FileCreationDate.Text = My.Resources.Label_Created
        FileDisabled.Text = My.Resources.Label_Disabled
        FileLastAccessDate.Text = My.Resources.Label_LastAccessed
        FileLastWriteDate.Text = My.Resources.Label_LastWritten
        FileName.Text = My.Resources.Label_FileName
        FileSize.Text = My.Resources.Label_Size
        FileSizeOnDisk.Text = My.Resources.Label_SizeOnDisk
        LabelBytesFree.Text = My.Resources.Label_BytesFree & ":"
        LabelBytesRequired.Text = My.Resources.Label_BytesRequired & ":"
        LabelDirectoryList.Text = My.Resources.Label_ImportIntoDirectory
        LabelFilesSelected.Text = My.Resources.Label_FilesSelected & ":"
        LabelOptions.Text = My.Resources.Label_Options
        Me.Text = My.Resources.Label_ImportFiles
    End Sub

    Private Sub PopulateDirectoryList(RootDirectory As IDirectory, CurrentDirectory As IDirectory)
        ComboDirectoryList.Items.Clear()

        ProcessDirectoryEntries(RootDirectory, "", CurrentDirectory)

        If ComboDirectoryList.SelectedIndex = -1 AndAlso ComboDirectoryList.Items.Count > 0 Then
            ComboDirectoryList.SelectedIndex = 0
        End If

        _LastSelectedIndex = ComboDirectoryList.SelectedIndex

        AutoSizeComboWidth(ComboDirectoryList)
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
        ListViewFiles.BeginUpdate()
        ListViewFiles.Items.Clear()
        ListViewFiles.Groups.Clear()

        _FileList = New ImportDirectoryRoot(Directory)

        'PopulateExistingFileNames(Directory, _FileList)

        Dim Group As New ListViewGroup(If(GroupPath = "", InParens(My.Resources.Label_Root), GroupPath)) With {
            .Tag = _FileList
        }

        ListViewFiles.Groups.Add(Group)

        ProcessFiles(_FileList, Group, FileList)
        ProcessFolders(_FileList, GroupPath, FolderList)
        RefreshOptions()

        If ListViewFiles.Items.Count > 0 Then
            ListViewFiles.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent)
            If ListViewFiles.Columns.Item(0).Width < 120 Then
                ListViewFiles.Columns.Item(0).Width = 120
            End If
            ListViewFiles.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent)
            If ListViewFiles.Columns.Item(1).Width < 70 Then
                ListViewFiles.Columns.Item(1).Width = 70
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

    Private Sub ProcessFiles(Parent As ImportDirectory, Group As ListViewGroup, FileList As List(Of IO.FileInfo))
        For Each File In FileList
            GridAddFile(Parent, Group, File)
        Next
    End Sub

    'Private Sub PopulateExistingFileNames(Directory As IDirectory, ImportDirectory As ImportDirectory)
    '    If Directory.Data.EntryCount > 0 Then
    '        For Counter As UInteger = 0 To Directory.Data.EntryCount - 1
    '            Dim File = Directory.DirectoryEntries.Item(Counter)
    '            If Not File.IsDeleted And Not File.IsVolumeName Then
    '                ImportDirectory.FileNames.Add(File.GetShortFileName)
    '            End If
    '        Next
    '    End If
    'End Sub
    Private Sub ProcessFolders(Parent As ImportDirectory, GroupPath As String, FolderList As List(Of DirectoryInfo))
        For Each Folder In FolderList
            GridAddFolder(Parent, GroupPath, Folder)
        Next
    End Sub

    Private Sub RefreshCreated()
        Dim ForeColor As Color

        For Each Item As ListViewItem In ListViewFiles.Items
            Dim FileData As ImportFile = Item.Tag

            If FileData.IsFileTooLarge Then
                ForeColor = Color.Gray
            ElseIf Not ChkCreated.Checked Then
                ForeColor = Color.Gray
            Else
                ForeColor = SystemColors.WindowText
            End If
            Item.SubItems.Item(COLUMN_CREATION_TIME).ForeColor = ForeColor
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

    Private Sub RefreshLastAccessed()
        Dim ForeColor As Color

        For Each Item As ListViewItem In ListViewFiles.Items
            Dim FileData As ImportFile = Item.Tag

            If FileData.IsFileTooLarge Then
                ForeColor = Color.Gray
            ElseIf Not ChkLastAccessed.Checked Then
                ForeColor = Color.Gray
            Else
                ForeColor = SystemColors.WindowText
            End If
            Item.SubItems.Item(COLUMN_LAST_ACCESS_TIME).ForeColor = ForeColor
        Next
    End Sub

    Private Sub RefreshOptions()
        If _HasLFN Then
            ChkLFN.Enabled = True
            ChkLFN.Checked = True
        Else
            ChkLFN.Enabled = True
            ChkLFN.Checked = False
        End If

        If _HasLFN Then
            ChkNTExtensions.Enabled = True
            ChkNTExtensions.Checked = False
        Else
            ChkNTExtensions.Enabled = False
            ChkNTExtensions.Checked = False
        End If

        ChkCreated.Checked = False
        ChkLastAccessed.Checked = False

        _FileList.Options.UseLFN = ChkLFN.Checked
        _FileList.Options.UseNTExtensions = ChkNTExtensions.Checked
        _FileList.Options.UseCreatedDate = ChkCreated.Checked
        _FileList.Options.UseLastAccessedDate = ChkLastAccessed.Checked
    End Sub

    Private Sub RefreshTotals()
        LblSelected.Text = FormatThousands(_FileList.SelectedFiles) & " of " & FormatThousands(ListViewFiles.Items.Count)
        LblBytesRequired.Text = FormatThousands(_FileList.TotalSpaceRequired)
        LblBytesFree.Text = FormatThousands(_FileList.FreeSpace)
        If _FileList.TotalSpaceRequired > _FileList.FreeSpace Then
            LblBytesRequired.ForeColor = Color.Red
        Else
            LblBytesRequired.ForeColor = SystemColors.WindowText
        End If

        BtnOK.Enabled = _FileList.SelectedFiles > 0 And _FileList.TotalSpaceRequired <= _FileList.FreeSpace
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
            _SelectionByPath(f.FilePath) = f.IsSelected
        Next

        ' Recurse into subdirectories
        For Each subDir In dir.DirectoryList
            SaveSelectionsFromDirectory(subDir)
        Next
    End Sub
    Public Class ImportDirectory
        Inherits ImportFileBase

        Private ReadOnly _AvailableEntries As Integer
        Private ReadOnly _DirectoryList As List(Of ImportDirectory)
        Private ReadOnly _FileList As List(Of ImportFile)
        'Private ReadOnly _FileNames As HashSet(Of String)
        Private ReadOnly _Root As ImportDirectoryRoot
        Private _EntriesRequired As Integer
        Private _SelectedFiles As Integer

        Public Sub New(FilePath As String, FileName As String, Root As ImportDirectoryRoot, Parent As ImportDirectory)
            MyBase.New(FilePath, FileName, Parent)

            _Root = Root
            _FileList = New List(Of ImportFile)
            _DirectoryList = New List(Of ImportDirectory)
            '_FileNames = New HashSet(Of String)
            _SelectedFiles = 0
            _EntriesRequired = 0
            _AvailableEntries = 0
        End Sub

        Public Sub New(AvailableEntries As Integer)
            MyBase.New("", "", Nothing)
            _Root = Nothing
            _FileList = New List(Of ImportFile)
            _DirectoryList = New List(Of ImportDirectory)
            '_FileNames = New HashSet(Of String)
            _SelectedFiles = 0
            _EntriesRequired = 0
            _AvailableEntries = AvailableEntries
        End Sub

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

        Public ReadOnly Property FileList As List(Of ImportFile)
            Get
                Return _FileList
            End Get
        End Property

        'Public ReadOnly Property FileNames As HashSet(Of String)
        '    Get
        '        Return _FileNames
        '    End Get
        'End Property

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

                    If Parent IsNot Nothing Then
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
            If _SelectedFiles > 0 Then
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
        Private Function GetRequiredBytes() As Integer
            Dim RequiredBytes As Integer = 0
            Dim EntryCount As Integer = _EntriesRequired - _AvailableEntries

            If EntryCount > 0 Then
                Dim EntriesPerCluster As UInteger = Root.BytesPerCluster \ 32
                RequiredBytes = CeilDiv(CUInt(EntryCount), EntriesPerCluster) * Root.BytesPerCluster
            End If

            Return RequiredBytes
        End Function
    End Class

    Public Class ImportDirectoryRoot
        Inherits ImportDirectory

        Private ReadOnly _BytesPerCluster As UInteger
        Private ReadOnly _CurrentDirectory As IDirectory
        Private ReadOnly _FreeSpace As UInteger
        Private ReadOnly _Options As AddFileOptions
        Private _TotalSpaceRequired As Long
        Public Sub New(CurrentDirectory As IDirectory)
            MyBase.New(CurrentDirectory.Data.AvailableEntryCount)

            _CurrentDirectory = CurrentDirectory
            _BytesPerCluster = CurrentDirectory.Disk.BPB.BytesPerCluster
            _FreeSpace = CurrentDirectory.Disk.FAT.GetFreeSpace

            _Options = New AddFileOptions With {
            .UseLFN = True,
            .UseNTExtensions = False
        }

            _TotalSpaceRequired = 0
        End Sub

        Public ReadOnly Property BytesPerCluster As UInteger
            Get
                Return _BytesPerCluster
            End Get
        End Property

        Public ReadOnly Property CurrentDirectory As IDirectory
            Get
                Return _CurrentDirectory
            End Get
        End Property
        Public ReadOnly Property FreeSpace As UInteger
            Get
                Return _FreeSpace
            End Get
        End Property

        Public ReadOnly Property Options As AddFileOptions
            Get
                Return _Options
            End Get
        End Property

        Public Property TotalSpaceRequired As Long
            Get
                Return _TotalSpaceRequired
            End Get
            Set(value As Long)
                _TotalSpaceRequired = value
            End Set
        End Property

        Public Sub SetOptions(LFN As Boolean, NTExtensions As Boolean)

            RefreshDirectoryList(DirectoryList, LFN, NTExtensions)
            RefreshFileList(FileList, LFN, NTExtensions)

            _Options.UseLFN = LFN
            _Options.UseNTExtensions = NTExtensions
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
                    File.RefreshRequiredEntries(LFN, NTExtensions)
                End If
            Next
        End Sub
    End Class

    Public Class ImportFile
        Inherits ImportFileBase

        Private ReadOnly _Root As ImportDirectoryRoot
        Private ReadOnly _SizeOnDisk As Long
        Private _IsSelected As Boolean

        Public Sub New(FilePath As String, FileName As String, SizeOnDisk As Long, Root As ImportDirectoryRoot, Parent As ImportDirectory)
            MyBase.New(FilePath, FileName, Parent)

            _Root = Root
            _IsSelected = False
            _SizeOnDisk = SizeOnDisk
        End Sub

        Public Property IsSelected As Boolean
            Get
                Return _IsSelected
            End Get
            Set(value As Boolean)
                If _IsSelected <> value Then
                    _IsSelected = value
                    Dim Diff = If(value, 1, -1)
                    Dim RequiredEntries = GetRequiredEntries(Root.Options.UseLFN, Root.Options.UseNTExtensions)
                    Dim Directory = Parent
                    If Directory IsNot Nothing Then
                        Directory.EntriesRequired += (RequiredEntries * Diff)
                        Do
                            Directory.SelectedFiles += Diff
                            Directory = Directory.Parent
                        Loop Until Directory Is Nothing
                    End If
                    If _Root IsNot Nothing Then
                        _Root.TotalSpaceRequired += (_SizeOnDisk * Diff)
                    End If
                End If
            End Set
        End Property

        Public ReadOnly Property Root As ImportDirectoryRoot
            Get
                Return _Root
            End Get
        End Property

        Public ReadOnly Property SizeOnDisk As Long
            Get
                Return _SizeOnDisk
            End Get
        End Property

        Public Function IsFileTooLarge() As Boolean
            Return _SizeOnDisk > _Root.FreeSpace
        End Function

        Public Sub RefreshRequiredEntries(UseLFN As Boolean, UseNTExtensions As Boolean)
            If _IsSelected Then
                Dim RequiredEntries = GetRequiredEntries(Root.Options.UseLFN, Root.Options.UseNTExtensions)
                Dim NewRequiredEntries = GetRequiredEntries(UseLFN, UseNTExtensions)
                Dim Diff = NewRequiredEntries - RequiredEntries
                If Diff <> 0 Then
                    Parent.EntriesRequired += Diff
                End If
            End If
        End Sub
    End Class

    Public MustInherit Class ImportFileBase
        Private ReadOnly _CanUseNTExtensions As Boolean
        Private ReadOnly _FileName As String
        Private ReadOnly _FilePath As String
        Private ReadOnly _IsLongFileName As Boolean
        Private ReadOnly _Parent As ImportDirectory

        Public Sub New(FilePath As String, FileName As String, Parent As ImportDirectory)
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

        Public ReadOnly Property CanUseNTExtensions As Boolean
            Get
                Return _CanUseNTExtensions
            End Get
        End Property

        Public ReadOnly Property FileName As String
            Get
                Return _FileName
            End Get
        End Property

        Public ReadOnly Property FilePath As String
            Get
                Return _FilePath
            End Get
        End Property

        Public ReadOnly Property IsLongFileName As Boolean
            Get
                Return _IsLongFileName
            End Get
        End Property

        Public ReadOnly Property Parent As ImportDirectory
            Get
                Return _Parent
            End Get
        End Property

        Public Function GetRequiredEntries(UseLFN As Boolean, UseNTExtensions As Boolean) As Integer
            If _FileName.Length = 0 Then
                Return 0
            End If

            Dim EntriesRequired As Integer = 1

            If Not UseNTExtensions Or Not _CanUseNTExtensions Then
                If UseLFN And _IsLongFileName Then
                    EntriesRequired += CeilDiv(CUInt(_FileName.Length), 26)
                End If
            End If

            Return EntriesRequired
        End Function

        Private Function CalcCanUseNTExtensions() As Boolean
            Dim FileParts = SplitFilename(_FileName)

            If FileParts.Name.Length > 8 Or FileParts.Extension.Length > 3 Then
                Return False
            Else
                Dim CleanFileName = DOSCleanFileName(FileParts.Name)
                Dim CleanExtension = DOSCleanFileName(FileParts.Extension)

                If CleanFileName = FileParts.Name.ToUpper And CleanExtension = FileParts.Extension.ToUpper Then
                    If (CleanFileName = FileParts.Name Or FileParts.Name.ToLower = FileParts.Name) And (CleanExtension = FileParts.Extension Or FileParts.Extension.ToLower = FileParts.Extension) Then
                        Return True
                    End If
                End If
            End If

            Return False
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