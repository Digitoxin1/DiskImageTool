Imports System.IO
Imports DiskImageTool.DiskImage

Public Class ImportFileForm
    Private ReadOnly _Directory As IDirectory
    Private _FileList As ImportDirectoryRoot
    Private _HasLFN As Boolean
    Private _HasNTExtensions As Boolean
    Private _IgnoreEvent As Boolean = False
    Private _ListViewHeader As ListViewHeader

    Public Sub New(Directory As IDirectory, FileNames() As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _IgnoreEvent = True
        _Directory = Directory
        PopulateFileList(Directory, FileNames)
        RefreshTotals()
        _IgnoreEvent = False
    End Sub

    Public ReadOnly Property FileList As ImportDirectoryRoot
        Get
            Return _FileList
        End Get
    End Property

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
        Dim SizeOnDisk As Long = Math.Ceiling(File.Length / BytesPerCluster) * BytesPerCluster

        Dim ImportFile = Parent.AddFile(File.FullName, File.Name, SizeOnDisk)

        If ImportFile.IsFileTooLarge Then
            ImportFile.IsSelected = False

            RowForeColor = Color.Gray
        Else
            If ImportFile.CanUseNTExtensions Then
                _HasNTExtensions = True
            End If

            If ImportFile.IsLongFileName Then
                _HasLFN = True
            End If

            ImportFile.IsSelected = True

            RowForeColor = SystemColors.WindowText
        End If

        Dim Item = New ListViewItem(Group) With {
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

        SubItem = Item.SubItems.Add(Format(File.Length, "N0"))
        SubItem.ForeColor = RowForeColor

        SubItem = Item.SubItems.Add(Format(ImportFile.SizeOnDisk, "N0"))
        SubItem.ForeColor = RowForeColor

        SubItem = Item.SubItems.Add(Format(File.LastWriteTime, "yyyy-MM-dd  HH:mm:ss"))
        SubItem.ForeColor = RowForeColor

        SubItem = Item.SubItems.Add(Format(File.CreationTime, "yyyy-MM-dd  HH:mm:ss"))
        SubItem.ForeColor = Color.Gray
        SubItem.Name = "CreationTime"

        SubItem = Item.SubItems.Add(Format(File.LastAccessTime, "yyyy-MM-dd"))
        SubItem.ForeColor = Color.Gray
        SubItem.Name = "LastAccessTime"

        SubItem = Item.SubItems.Add(If(ImportFile.IsSelected, 0, 1))
        SubItem.Name = "IsSelected"

        ListViewFiles.Items.Add(Item)
    End Sub

    Private Sub GridAddFolder(Parent As ImportDirectory, GroupPath As String, Folder As DirectoryInfo)
        GroupPath = GroupPath & If(GroupPath = "", "", "\") & Folder.Name

        Dim ImportDirectory = Parent.AddDirectory(Folder.FullName, Folder.Name)

        'Dim ShortName = GetAvailableFileName(ImportDirectory.FileName, False, Parent.FileNames)
        'Parent.FileNames.Add(ShortName)

        Dim Group = New ListViewGroup(GroupPath) With {
            .Tag = ImportDirectory
        }
        ListViewFiles.Groups.Add(Group)

        ProcessFolders(ImportDirectory, GroupPath, Folder.GetDirectories.ToList)
        ProcessFiles(ImportDirectory, Group, Folder.GetFiles.ToList)
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

        _FileList = New ImportDirectoryRoot(Directory.Data.AvailableEntryCount, Directory.Disk.BPB.BytesPerCluster, Directory.Disk.FAT.GetFreeSpace)

        'PopulateExistingFileNames(Directory, _FileList)

        Dim Group = New ListViewGroup(If(GroupPath = "", "(Root)", GroupPath)) With {
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

    Private Sub ProcessFiles(Parent As ImportDirectory, Group As ListViewGroup, FileList As List(Of IO.FileInfo))
        For Each File In FileList
            GridAddFile(Parent, Group, File)
        Next
    End Sub

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
            Item.SubItems.Item("CreationTime").ForeColor = ForeColor
        Next
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
            Item.SubItems.Item("LastAccessTime").ForeColor = ForeColor
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
        LblSelected.Text = Format(_FileList.SelectedFiles, "N0") & " of " & Format(ListViewFiles.Items.Count, "N0")
        LblBytesRequired.Text = Format(_FileList.TotalSpaceRequired, "N0")
        LblBytesFree.Text = Format(_FileList.FreeSpace, "N0")
        If _FileList.TotalSpaceRequired > _FileList.FreeSpace Then
            LblBytesRequired.ForeColor = Color.Red
        Else
            LblBytesRequired.ForeColor = SystemColors.WindowText
        End If

        BtnOK.Enabled = _FileList.SelectedFiles > 0 And _FileList.TotalSpaceRequired <= _FileList.FreeSpace
    End Sub

    Private Class ListViewItemComparer
        Implements IComparer

        Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
            Dim Value1 As Integer = DirectCast(x, ListViewItem).SubItems("IsSelected").Text
            Dim Value2 As Integer = DirectCast(y, ListViewItem).SubItems("IsSelected").Text

            Return Value1.CompareTo(Value2)
        End Function
    End Class

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
#End Region
End Class

#Region "Classes"
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
        Dim NewDirectory = New ImportDirectory(FilePath, FileName, Root, Me)
        _DirectoryList.Add(NewDirectory)

        Return NewDirectory
    End Function

    Public Function AddFile(FilePath As String, FileName As String, SizeOnDisk As Long) As ImportFile
        Dim NewFile = New ImportFile(FilePath, FileName, SizeOnDisk, Root, Me)
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
            Dim EntriesPerCluster = Root.BytesPerCluster \ 32
            RequiredBytes = Math.Ceiling(EntryCount / EntriesPerCluster) * Root.BytesPerCluster
        End If

        Return RequiredBytes
    End Function
End Class

Public Class ImportDirectoryRoot
    Inherits ImportDirectory

    Private ReadOnly _BytesPerCluster As UInteger
    Private ReadOnly _FreeSpace As UInteger
    Private ReadOnly _Options As AddFileOptions
    Private _TotalSpaceRequired As Long

    Public Sub New(AvailableEntries As Integer, BytesPerCluster As UInteger, FreeSpace As UInteger)
        MyBase.New(AvailableEntries)

        _BytesPerCluster = BytesPerCluster
        _FreeSpace = FreeSpace

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
                EntriesRequired += Math.Ceiling(_FileName.Length / 26)
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
#End Region