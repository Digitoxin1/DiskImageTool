Imports System.Text
Imports DiskImageTool.DiskImage

Module ImageTools
    Public Function BootSectorEdit(Disk As Disk, BootStrapDB As BootstrapDB) As Boolean
        Dim BootSectorForm As New BootSectorForm(Disk.BootSector.Data, BootStrapDB)

        BootSectorForm.ShowDialog()

        Dim Result As Boolean = BootSectorForm.DialogResult = DialogResult.OK

        If Result Then
            If Not Disk.BootSector.Data.CompareTo(BootSectorForm.Data) Then
                Disk.BootSector.Data = BootSectorForm.Data
            Else
                Result = False
            End If
        End If

        Return Result
    End Function

    Public Sub CrossLinkedFilesDisplay(Disk As Disk, DirectoryEntry As DiskImage.DirectoryEntry)
        Dim Msg = String.Format(My.Resources.Dialog_CrossLinked, DirectoryEntry.GetShortFileName(True), Environment.NewLine)

        For Each Crosslink In DirectoryEntry.CrossLinks
            If Crosslink IsNot DirectoryEntry Then
                Msg &= Environment.NewLine & Crosslink.GetShortFileName(True)
            End If
        Next
        MsgBox(Msg, MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
    End Sub

    Public Sub DirectoryEntryDisplayText(DirectoryEntry As DiskImage.DirectoryEntry)
        If Not DirectoryEntry.IsValidFile Then 'Or DirectoryEntry.IsDeleted Then
            Exit Sub
        End If

        Dim Caption As String

        If DirectoryEntry.IsDeleted Then
            Caption = My.Resources.Caption_DeleteFile & " - " & DirectoryEntry.GetShortFileName(True)
        Else
            Caption = My.Resources.Caption_File & " - " & DirectoryEntry.GetShortFileName(True)
        End If

        Dim Bytes = DirectoryEntry.GetContent
        Dim Content As String

        Using Stream As New IO.MemoryStream
            Dim PrevByte As Byte = 0
            For Counter = 0 To Bytes.Length - 1
                Dim B = Bytes(Counter)
                If B = 0 Then
                    Stream.WriteByte(32)
                ElseIf Counter > 0 And B = 10 And PrevByte <> 13 Then
                    Stream.WriteByte(13)
                    Stream.WriteByte(10)
                Else
                    Stream.WriteByte(B)
                End If
                PrevByte = B
            Next
            Content = Encoding.UTF7.GetString(Stream.GetBuffer)
        End Using

        Dim frmTextView = New TextViewForm(Caption, Content, False, True, DirectoryEntry.GetFullFileName)
        frmTextView.ShowDialog()
    End Sub

    Public Function FATEdit(Disk As Disk, Index As UShort) As Boolean
        Dim frmFATEdit As New FATEditForm(Disk, Index)

        frmFATEdit.ShowDialog()

        Return frmFATEdit.Updated
    End Function

    Public Function FilePropertiesEdit(FilePanel As FilePanel) As Boolean
        Dim Result As Boolean = False

        If FilePanel.SelectedItems.Count = 1 Then
            Dim Item As ListViewItem = FilePanel.FirstSelectedItem
            Dim FileData As FileData = TryCast(Item.Tag, FileData)

            If FileData IsNot Nothing Then
                Dim frmFilePropertiesEdit As New FilePropertiesFormSingle(FileData.DirectoryEntry)
                frmFilePropertiesEdit.ShowDialog()
                If frmFilePropertiesEdit.DialogResult = DialogResult.OK Then
                    Result = frmFilePropertiesEdit.Updated
                End If
            End If
        Else
            Dim Entries As New List(Of DirectoryEntry)
            For Each Item As ListViewItem In FilePanel.SelectedItems
                Dim FileData As FileData = TryCast(Item.Tag, FileData)
                If FileData IsNot Nothing Then
                    Entries.Add(FileData.DirectoryEntry)
                End If
            Next

            If Entries.Count > 0 Then
                Dim frmFilePropertiesEdit As New FilePropertiesFormMultiple(FilePanel.CurrentImage.Disk, Entries)
                frmFilePropertiesEdit.ShowDialog()
                If frmFilePropertiesEdit.DialogResult = DialogResult.OK Then
                    Result = frmFilePropertiesEdit.Updated
                End If
            End If
        End If

        Return Result
    End Function

    Public Function ImageAddDirectory(ParentDirectory As IDirectory, Optional Index As Integer = -1) As Boolean
        Dim Updated As Boolean = False

        Dim frmNewDirectory As New NewDirectoryForm()
        frmNewDirectory.ShowDialog()

        If frmNewDirectory.DialogResult = DialogResult.OK Then
            Updated = frmNewDirectory.Updated

            If Updated Then
                Dim Options As New AddFileOptions With {
                    .UseLFN = frmNewDirectory.HasLFN,
                    .UseNTExtensions = frmNewDirectory.UseNTExtensions
                }
                Dim AddDirectoryResponse = ParentDirectory.AddDirectory(frmNewDirectory.NewDirectoryData, Options, frmNewDirectory.NewFilename, Index)

                If AddDirectoryResponse.Entry Is Nothing Then
                    MsgBox(My.Resources.Dialog_DiskSpaceWarning, MsgBoxStyle.Exclamation)
                    Updated = False
                End If
            End If
        End If

        Return Updated
    End Function

    Public Function ImageDeleteSelectedFiles(FilePanel As FilePanel, Remove As Boolean) As Boolean
        Dim Msg As String
        Dim Title As String
        Dim DialogResult As DeleteFileForm.DeleteFileFormResult
        Dim Item As ListViewItem
        Dim FileData As FileData
        Dim Result As Boolean = False
        Dim CanFill As Boolean
        Dim DoRemove As Boolean
        Dim DisplayDialog As Boolean = False


        If FilePanel.SelectedItems.Count = 0 Then
            Return False
        End If

        For Each Item In FilePanel.SelectedItems
            FileData = Item.Tag
            If FileData IsNot Nothing Then
                If Not FileData.DirectoryEntry.IsDeleted Then
                    DisplayDialog = True
                    Exit For
                End If
            End If
        Next

        If DisplayDialog Then
            If FilePanel.SelectedItems.Count = 1 Then
                Item = FilePanel.FirstSelectedItem
                FileData = Item.Tag
                If Remove Then
                    Msg = String.Format(My.Resources.Dialog_RemoveFile, FileData.DirectoryEntry.GetShortFileName(True))
                    Title = My.Resources.Caption_RemoveFile
                Else
                    Msg = String.Format(My.Resources.Dialog_DeleteFile, FileData.DirectoryEntry.GetShortFileName(True))
                    Title = My.Resources.Caption_DeleteFile
                End If
                CanFill = FileData.DirectoryEntry.IsValidFile Or FileData.DirectoryEntry.IsValidDirectory
            Else
                If Remove Then
                    Msg = My.Resources.Dialog_RemoveSelectedFiles
                    Title = String.Format(My.Resources.Caption_RemoveFiles, FilePanel.SelectedItems.Count)
                Else
                    Msg = My.Resources.Dialog_DeleteSelectedFiles
                    Title = String.Format(My.Resources.Caption_DeleteFiles, FilePanel.SelectedItems.Count)
                End If
                CanFill = True
            End If


            Dim DeleteFileForm As New DeleteFileForm(Msg, Title, CanFill)
            DeleteFileForm.ShowDialog()
            DialogResult = DeleteFileForm.Result
            DeleteFileForm.Close()

            If DialogResult.Cancelled Then
                Return False
            End If
        Else
            DialogResult.Clear = False
            DialogResult.FillChar = 0
        End If

        Dim UseTransaction As Boolean = FilePanel.CurrentImage.Disk.BeginTransaction

        For Each Item In FilePanel.SelectedItems
            DoRemove = False
            FileData = Item.Tag
            If FileData IsNot Nothing Then
                If FileData.DirectoryEntry.IsDeleted Then
                    DoRemove = True
                ElseIf DirectoryEntryDelete(FileData.DirectoryEntry, DialogResult.FillChar, DialogResult.Clear) Then
                    Result = True
                    DoRemove = True
                End If
            End If

            If DoRemove And Remove Then
                Dim ParentDirectory = FileData.DirectoryEntry.ParentDirectory
                Dim Index = FileData.DirectoryEntry.Index

                If ParentDirectory.RemoveEntry(Index) Then
                    Result = True
                End If
            End If
        Next

        If UseTransaction Then
            FilePanel.CurrentImage.Disk.EndTransaction()
        End If

        Return Result
    End Function

    Public Function ImageFixFileSize(DirectoryEntry As DirectoryEntry)
        If Not DirectoryEntry.HasIncorrectFileSize Then
            Return False
        End If

        DirectoryEntry.FileSize = DirectoryEntry.GetAllocatedSizeFromFAT

        Return True
    End Function

    Public Function ImageImport(ParentDirectory As IDirectory, Multiselect As Boolean, Optional Index As Integer = -1) As Boolean
        Dim FileNames() As String = New String(-1) {}

        If My.Settings.DragAndDrop Then
            Dim frmFileDrop As New FileDropForm()

            If frmFileDrop.ShowDialog() <> DialogResult.OK Then
                Return False
            End If

            FileNames = frmFileDrop.FileNames
        Else
            Dim Dialog = New OpenFileDialog With {
               .Multiselect = Multiselect
           }
            If Dialog.ShowDialog <> DialogResult.OK Then
                Return False
            End If

            FileNames = Dialog.FileNames
        End If

        Dim ImportFilesForm As New ImportFileForm(ParentDirectory, FileNames)
        Dim Result = ImportFilesForm.ShowDialog()

        If Result = DialogResult.Cancel Then
            Return False
        End If

        Dim FileList = ImportFilesForm.FileList

        If FileList.SelectedFiles < 1 Then
            Return False
        End If

        Dim UseTransaction = ParentDirectory.Disk.BeginTransaction

        Dim FilesAdded As UInteger = 0
        Dim FileCount As UInteger = FileList.SelectedFiles

        ImageImportFolders(FileList.DirectoryList, ParentDirectory, Index, FileList.Options, FilesAdded)
        ImageImportFiles(FileList.FileList, ParentDirectory, Index, FileList.Options, FilesAdded)

        If FilesAdded < FileCount Then
            MsgBox(String.Format(My.Resources.Dialog_InsufficientDiskSpaceWarning, Environment.NewLine, FilesAdded, FileCount), MsgBoxStyle.Exclamation)
        End If

        If UseTransaction Then
            ParentDirectory.Disk.EndTransaction()
        End If

        Return FilesAdded > 0
    End Function

    Public Function ImageIsWin9xDisk(Disk As Disk, BootStrapDB As BootstrapDB) As Boolean
        Dim Response As Boolean = False
        Dim OEMNameWin9x = Disk.BootSector.IsWin9xOEMName
        Dim OEMName = Disk.BootSector.OEMName

        If OEMNameWin9x Then
            Dim BootstrapType = BootStrapDB.FindMatch(Disk.BootSector.BootStrapCode)

            If BootstrapType IsNot Nothing Then
                For Each KnownOEMName In BootstrapType.OEMNames
                    If KnownOEMName.Name.CompareTo(OEMName) Or KnownOEMName.Win9xId Then
                        Return True
                        Exit For
                    End If
                Next
            End If
        End If

        Return Response
    End Function

    Public Function ImageReplaceFile(DirectoryEntry As DirectoryEntry) As Boolean
        Dim Dialog = New OpenFileDialog
        Dim FormResult As ReplaceFileForm.ReplaceFileFormResult

        If Dialog.ShowDialog <> DialogResult.OK Then
            Return False
        End If

        Dim FileInfo As New IO.FileInfo(Dialog.FileName)

        Dim AvailableSpace = DirectoryEntry.Disk.FAT.GetFreeSpace() + DirectoryEntry.GetSizeOnDisk

        Dim ReplaceFileForm As New ReplaceFileForm(AvailableSpace, DirectoryEntry.ParentDirectory)
        With ReplaceFileForm
            .SetOriginalFile(DirectoryEntry.GetShortFileName, DirectoryEntry.GetLastWriteDate.DateObject, DirectoryEntry.FileSize)
            .SetNewFile(DirectoryEntry.ParentDirectory.GetAvailableShortFileName(FileInfo.Name, False, DirectoryEntry.Index), FileInfo.LastWriteTime, FileInfo.Length)
            .RefreshText()
            .ShowDialog()
            FormResult = .Result
            .Close()
        End With

        If FormResult.Cancelled Then
            Return False
        End If

        Dim UseTransaction As Boolean = DirectoryEntry.Disk.BeginTransaction

        Dim Result As Boolean = False
        Dim FreeClusters = DirectoryEntry.Disk.FAT.GetFreeClusters(FAT12.FreeClusterEmum.WithoutData)

        Result = DirectoryEntry.UpdateFile(Dialog.FileName, FormResult.FileSize, FormResult.FillChar, FreeClusters)
        If Not Result Then
            Result = DirectoryEntry.UpdateFile(Dialog.FileName, FormResult.FileSize, FormResult.FillChar)
        End If

        If Result Then
            If FormResult.FileNameChanged Then
                DirectoryEntry.SetFileName(FormResult.FileName)
            End If

            If FormResult.FileDateChanged Then
                DirectoryEntry.SetLastWriteDate(FormResult.FileDate)
            End If
        End If

        If UseTransaction Then
            DirectoryEntry.Disk.EndTransaction()
        End If

        Return Result
    End Function

    Public Function ImageUndeleteFile(DirectoryEntry As DirectoryEntry) As Boolean
        If Not DirectoryEntryCanUndelete(DirectoryEntry) Then
            Return False
        End If

        Dim UndeleteForm As New UndeleteForm(DirectoryEntry.GetShortFileName)

        UndeleteForm.ShowDialog()

        If UndeleteForm.DialogResult = DialogResult.OK Then
            Dim FirstChar = UndeleteForm.FirstChar
            If FirstChar > 0 Then
                DirectoryEntry.UnDelete(FirstChar)

                Return True
            End If
        End If

        Return False
    End Function

    Private Sub ImageImportFiles(FileList As List(Of ImportFile), ParentDirectory As IDirectory, Index As Integer, Options As AddFileOptions, ByRef FilesAdded As UInteger)
        For Each File In FileList
            If File.IsSelected Then
                Dim FileInfo As New IO.FileInfo(File.FilePath)
                Dim EntriesAdded = ParentDirectory.AddFile(FileInfo, Options, Index)
                If EntriesAdded > -1 Then
                    FilesAdded += 1
                    If Index > -1 Then
                        Index += EntriesAdded
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub ImageImportFolders(FolderList As List(Of ImportDirectory), ParentDirectory As IDirectory, Index As Integer, Options As AddFileOptions, ByRef FilesAdded As UInteger)
        For Each Folder In FolderList
            If Folder.SelectedFiles > 0 Then
                Dim DirectoryInfo As New IO.DirectoryInfo(Folder.FilePath)

                Dim DirectoryEntry = New DirectoryEntryBase
                DirectoryEntry.SetFileInfo(DirectoryInfo, Options.UseCreatedDate, Options.UseLastAccessedDate)

                Dim AddDirectoryResponse = ParentDirectory.AddDirectory(DirectoryEntry.Data, Options, DirectoryInfo.Name, Index)
                If AddDirectoryResponse.Entry IsNot Nothing Then
                    FilesAdded += 1
                    If Index > -1 Then
                        Index += AddDirectoryResponse.EntriesNeeded
                    End If

                    ImageImportFolders(Folder.DirectoryList, AddDirectoryResponse.Entry.SubDirectory, -1, Options, FilesAdded)
                    ImageImportFiles(Folder.FileList, AddDirectoryResponse.Entry.SubDirectory, -1, Options, FilesAdded)
                End If
            End If
        Next
    End Sub
End Module
