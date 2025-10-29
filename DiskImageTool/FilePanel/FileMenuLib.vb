Imports DiskImageTool.DiskImage

Public Structure FileMenuStateProperties
    Dim Enabled As Boolean
    Dim Visible As Boolean
    Dim Caption As String
End Structure

Public Structure FileMenuState
    Dim FilePropertiesEnabled As Boolean
    Dim ExportFile As FileMenuStateProperties
    Dim ReplaceFileEnabled As Boolean
    Dim ViewCrosslinkedVisible As Boolean
    Dim FixSizeEnabled As Boolean
    Dim ViewFileText As FileMenuStateProperties
    Dim RemoveFile As FileMenuStateProperties
    Dim DeleteFile As FileMenuStateProperties
    Dim UndeleteFile As FileMenuStateProperties
    Dim ViewFile As FileMenuStateProperties
    Dim ViewHexFile As FileMenuStateProperties
    Dim ImportFilesHereEnabled As Boolean
    Dim NewDirectoryHereEnabled As Boolean
    Dim ViewDirectory As FileMenuStateProperties
    Dim AddFileEnabled As Boolean
    Dim TopMenuAddFileEnabled As Boolean
    Dim DirectoryEntry As DirectoryEntry
    Dim ParentDirectory As IDirectory
    Dim RootDirectory As IDirectory
End Structure

Public Class MenuItemClickedEventArgs
    Inherits EventArgs
    Public Property MenuItem As FilePanel.FilePanelMenuItem
    Public Property Directory As IDirectory

    Public Sub New(MenuItem As FilePanel.FilePanelMenuItem, Directory As IDirectory)
        Me.MenuItem = MenuItem
        Me.Directory = Directory
    End Sub
End Class

Module FileMenuLib
    Public Structure DirectoryStats
        Dim CanDelete As Boolean
        Dim CanExport As Boolean
        Dim CanInsert As Boolean
        Dim CanUndelete As Boolean
        Dim FileSize As UInteger
        Dim FullFileName As String
        Dim IsDeleted As Boolean
        Dim IsDirectory As Boolean
        Dim IsModified As Boolean
        Dim IsValidDirectory As Boolean
        Dim IsValidFile As Boolean
    End Structure

    Public Function GetFileMenuState(FilePanel As FilePanel) As FileMenuState
        Dim State As FileMenuState
        State.FilePropertiesEnabled = False
        State.ExportFile = GetProps(False, True, My.Resources.Menu_ExportFile)
        State.ReplaceFileEnabled = False
        State.ViewCrosslinkedVisible = False
        State.FixSizeEnabled = False
        State.ViewFileText = GetProps(False, False, My.Resources.Menu_ViewFileAsText)
        State.RemoveFile = GetProps(False, True, My.Resources.Menu_RemoveFile)
        State.DeleteFile = GetProps(False, True, My.Resources.Menu_DeleteFile)
        State.UndeleteFile = GetProps(False, False, My.Resources.Menu_UndeleteFile)
        State.ViewFile = GetProps(False, True, My.Resources.Menu_ViewFile)
        State.ViewHexFile = GetProps(True, False, My.Resources.Menu_ViewFile)
        State.ImportFilesHereEnabled = False
        State.NewDirectoryHereEnabled = False
        State.ViewDirectory = GetProps(False, False, My.Resources.Menu_ViewDirectoryAlt)
        State.AddFileEnabled = False
        State.TopMenuAddFileEnabled = False
        State.DirectoryEntry = Nothing
        State.ParentDirectory = Nothing
        State.RootDirectory = Nothing

        Dim Stats As DirectoryStats

        If FilePanel.SelectedItems.Count = 1 Then
            State.FilePropertiesEnabled = True

            Dim FileData As FileData = TryCast(FilePanel.FirstSelectedItem.Tag, FileData)
            State.ParentDirectory = TryCast(FilePanel.FirstSelectedItem.Group.Tag, IDirectory)
            If FileData IsNot Nothing Then
                Stats = DirectoryEntryGetStats(FileData.DirectoryEntry)

                'ExportFile
                State.ExportFile.Enabled = Stats.CanExport And Not Stats.IsDirectory

                'ReplaceFile
                State.ReplaceFileEnabled = Stats.IsValidFile And Not Stats.IsDeleted

                'ViewCrosslinked
                State.ViewCrosslinkedVisible = FileData.DirectoryEntry.IsCrossLinked

                'FixSize
                State.FixSizeEnabled = FileData.DirectoryEntry.HasIncorrectFileSize

                'ViewFileText
                State.ViewFileText.Enabled = Stats.FileSize > 0
                State.ViewFileText.Visible = Stats.IsValidFile
                If Stats.IsDeleted Then
                    State.ViewFileText.Caption = My.Resources.Menu_ViewDeletedFileAsText
                Else
                    State.ViewFileText.Caption = My.Resources.Menu_ViewFileAsText
                End If

                'RemoveFile
                If Stats.IsDeleted Then
                    State.RemoveFile.Enabled = True
                Else
                    State.RemoveFile.Enabled = Stats.CanDelete
                End If

                If Stats.IsDirectory Then
                    If Stats.IsDeleted Then
                        State.RemoveFile.Caption = My.Resources.Menu_RemoveDeletedDirectory
                    Else
                        State.RemoveFile.Caption = My.Resources.Menu_RemoveDirectory
                    End If
                Else
                    If Stats.IsDeleted Then
                        State.RemoveFile.Caption = My.Resources.Menu_RemoveDeletedFile
                    Else
                        State.RemoveFile.Caption = My.Resources.Menu_RemoveFile
                    End If
                End If

                'DeleteFile
                If Stats.IsDeleted Then
                    State.DeleteFile.Visible = False
                Else
                    State.DeleteFile.Enabled = Stats.CanDelete
                    If Stats.IsDirectory Then
                        State.DeleteFile.Caption = My.Resources.Menu_DeleteDirectory
                    Else
                        State.DeleteFile.Caption = My.Resources.Menu_DeleteFile
                    End If
                End If

                'UndeleteFile
                If Stats.IsDeleted Then
                    State.UndeleteFile.Enabled = Stats.CanUndelete
                    State.UndeleteFile.Visible = True
                    If Stats.IsDirectory Then
                        State.UndeleteFile.Caption = My.Resources.Menu_UndeleteDirectory
                    Else
                        State.UndeleteFile.Caption = My.Resources.Menu_UndeleteFile
                    End If
                End If

                If Stats.IsValidFile Or Stats.IsValidDirectory Then
                    'ViewHexFile
                    Dim Caption As String

                    If Stats.IsDirectory Then
                        If Stats.IsDeleted Then
                            Caption = My.Resources.Menu_DeletedDirectory
                        Else
                            Caption = My.Resources.Menu_Directory2
                        End If
                    Else
                        If Stats.IsDeleted Then
                            Caption = My.Resources.Menu_DeletedFile
                        Else
                            Caption = My.Resources.Menu_File
                        End If
                    End If

                    State.ViewHexFile.Caption = Caption & ":   " & Stats.FullFileName

                    State.ViewHexFile.Visible = Stats.IsDirectory Or Stats.FileSize > 0
                    State.DirectoryEntry = FileData.DirectoryEntry

                    'ViewFile
                    If Stats.IsDirectory Then
                        If Stats.IsDeleted Then
                            State.ViewFile.Caption = My.Resources.Menu_ViewDeletedDirectory
                        Else
                            State.ViewFile.Caption = My.Resources.Menu_ViewDirectory
                        End If
                    Else
                        If Stats.IsDeleted Then
                            State.ViewFile.Caption = My.Resources.Menu_ViewDeletedFile
                        Else
                            State.ViewFile.Caption = My.Resources.Menu_ViewFile
                        End If
                    End If

                    State.ViewFile.Enabled = Stats.IsDirectory Or Stats.FileSize > 0
                End If

                'NewDirectoryHere
                State.NewDirectoryHereEnabled = Stats.CanInsert

                'ImportFilesHere
                State.ImportFilesHereEnabled = Stats.CanInsert
            End If

        ElseIf FilePanel.SelectedItems.Count > 1 Then
            Dim FileData As FileData
            Dim ExportEnabled As Boolean = False
            Dim DeleteEnabled As Boolean = False
            Dim RemoveEnabled As Boolean = False
            Dim MutlipleParents As Boolean = False

            State.ParentDirectory = Nothing

            For Each Item As ListViewItem In FilePanel.SelectedItems
                FileData = TryCast(Item.Tag, FileData)
                If FileData IsNot Nothing Then
                    Stats = DirectoryEntryGetStats(FileData.DirectoryEntry)
                    If Stats.CanExport Then
                        ExportEnabled = True
                    End If
                    If Not Stats.IsDeleted And Stats.CanDelete Then
                        DeleteEnabled = True
                    End If
                    If Stats.IsDeleted Or Stats.CanDelete Then
                        RemoveEnabled = True
                    End If
                    If State.ParentDirectory Is Nothing Then
                        State.ParentDirectory = FileData.DirectoryEntry.ParentDirectory
                    ElseIf State.ParentDirectory IsNot FileData.DirectoryEntry.ParentDirectory Then
                        MutlipleParents = True
                    End If
                End If
            Next

            If MutlipleParents Then
                State.ParentDirectory = Nothing
            End If

            'FileProperties
            State.FilePropertiesEnabled = True

            'ExportFile
            State.ExportFile.Enabled = ExportEnabled
            State.ExportFile.Caption = My.Resources.Menu_ExportSelected

            'ReplaceFile
            State.ReplaceFileEnabled = False

            'ViewFileText
            State.ViewFileText.Visible = True

            'RemoveFile
            State.RemoveFile.Enabled = RemoveEnabled
            State.RemoveFile.Caption = My.Resources.Menu_RemoveSelected

            'DeleteFile
            State.DeleteFile.Enabled = DeleteEnabled
            State.DeleteFile.Caption = My.Resources.Menu_DeleteSelected
        End If

        If State.ParentDirectory Is Nothing Then
            If FilePanel.CurrentImage Is Nothing OrElse FilePanel.CurrentImage.Disk Is Nothing OrElse Not FilePanel.CurrentImage.Disk.IsValidImage Then
                State.TopMenuAddFileEnabled = False
            Else
                State.TopMenuAddFileEnabled = True
                State.RootDirectory = FilePanel.CurrentImage.Disk.RootDirectory
            End If
        Else
            If State.ParentDirectory Is FilePanel.CurrentImage.Disk.RootDirectory Then
                State.ViewDirectory.Caption = My.Resources.Menu_ViewRootDirectory
            Else
                State.ViewDirectory.Caption = My.Resources.Menu_ViewParentDirectory
            End If
            State.ViewDirectory.Enabled = True
            State.ViewDirectory.Visible = True
            State.AddFileEnabled = True
            State.TopMenuAddFileEnabled = True
            State.RootDirectory = FilePanel.CurrentImage.Disk.RootDirectory
        End If

        Return State
    End Function

    Public Sub SetMenuItemStateEnabled(Item As ToolStripItem, Enabled As Boolean)
        Item.Enabled = Enabled
    End Sub

    Public Sub SetMenuItemStateEnabled(Item As ToolStripItem, Enabled As Boolean, Tag As Object)
        Item.Enabled = Enabled
        Item.Tag = Tag
    End Sub

    Public Sub SetMenuItemStateVisible(Item As ToolStripItem, Visible As Boolean)
        Item.Visible = Visible
    End Sub

    Public Sub SetMenuItemState(Item As ToolStripItem, Props As FileMenuStateProperties)
        Item.Enabled = Props.Enabled
        Item.Visible = Props.Visible
        Item.Text = Props.Caption
    End Sub

    Public Sub SetMenuItemState(Item As ToolStripItem, Props As FileMenuStateProperties, Tag As Object)
        Item.Enabled = Props.Enabled
        Item.Visible = Props.Visible
        Item.Text = Props.Caption
        Item.Tag = Tag
    End Sub

    Private Function GetProps(Enabled As Boolean, Visible As Boolean, Caption As String) As FileMenuStateProperties
        Dim Props As FileMenuStateProperties

        Props.Enabled = Enabled
        Props.Visible = Visible
        Props.Caption = Caption

        Return Props
    End Function

    Private Function DirectoryEntryGetStats(DirectoryEntry As DiskImage.DirectoryEntry) As DirectoryStats
        Dim Stats As DirectoryStats

        With Stats
            .IsDirectory = DirectoryEntry.IsDirectory And Not DirectoryEntry.IsVolumeName
            .IsDeleted = DirectoryEntry.IsDeleted
            .IsModified = DirectoryEntry.IsModified
            .IsValidFile = DirectoryEntry.IsValidFile
            .IsValidDirectory = DirectoryEntry.IsValidDirectory
            .CanExport = DirectoryEntryCanExport(DirectoryEntry)
            .FileSize = DirectoryEntry.FileSize
            .FullFileName = DirectoryEntry.GetShortFileName(True)
            .CanDelete = DirectoryEntryCanDelete(DirectoryEntry, False)
            .CanUndelete = DirectoryEntryCanUndelete(DirectoryEntry)
            .CanInsert = DirectoryEntry.Index < DirectoryEntry.ParentDirectory.DirectoryEntries.Count - DirectoryEntry.ParentDirectory.Data.AvailableEntryCount
        End With

        Return Stats
    End Function
End Module
