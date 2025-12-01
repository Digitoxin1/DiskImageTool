Imports System.ComponentModel
Imports System.Text
Imports DiskImageTool.DiskImage

Module DiskImageLib
    Public Enum DiskImageMenuItem
        BootSectorEdit
        BootSectorRemoveFromDirectory
        BootSectorRestoreFromDirectory
        EditFAT
        HexDisplayBadSectors
        HexDisplayBootSector
        HexDisplayDirectory
        HexDisplayDirectoryEntry
        HexDisplayDisk
        HexDisplayFAT
        HexDisplayFreeClusters
        HexDisplayLostSectors
        HexDisplayOverdumpData
        ImageClearReservedBytes
        ImageFixImageSize
        ImageRedo
        ImageRestructure
        ImageUndo
        RemoveWindowsModifications
    End Enum

    Public Sub DirectoryEntryDisplay(FilePanel As FilePanel, FileData As FileData)
        If FileData.DirectoryEntry.IsValidFile And FileData.DirectoryEntry.FileSize > 0 Then
            If IsBinaryData(FileData.DirectoryEntry.GetContent) Then
                If FileData.DirectoryEntry IsNot Nothing Then
                    If HexDisplayDirectoryEntry(FilePanel.CurrentImage.Disk, FileData.DirectoryEntry) Then
                        DiskImageRefresh(FilePanel)
                    End If
                End If
            Else
                DirectoryEntryDisplayText(FileData.DirectoryEntry)
            End If
        End If
    End Sub

    Public Function DiskImageCloseCurrent(CurrentImage As DiskImageContainer, LoadedFiles As LoadedFiles) As Boolean
        Dim NewFilePath As String = ""
        Dim Result As MsgBoxResult

        If CurrentImage.ImageData.IsModified Then
            Result = MsgBoxSave(CurrentImage.ImageData.FileName)
        Else
            Result = MsgBoxResult.No
        End If

        If Result = MsgBoxResult.Yes Then
            If CurrentImage.ImageData.ReadOnly Or CurrentImage.ImageData.FileType = ImageData.FileTypeEnum.NewImage Then
                NewFilePath = GetNewFilePath(CurrentImage, LoadedFiles)
                If NewFilePath = "" Then
                    Result = MsgBoxResult.Cancel
                End If
            End If
            If Result = MsgBoxResult.Yes Then
                If Not DiskImageSave(CurrentImage, NewFilePath) Then
                    Result = MsgBoxResult.Cancel
                End If
            End If
        End If

        If Result <> MsgBoxResult.Cancel Then
            If CurrentImage.ImageData.FileType = ImageData.FileTypeEnum.NewImage Then
                DeleteTempFileIfExists(CurrentImage.ImageData.SourceFile)
            End If
        End If

        Return Result <> MsgBoxResult.Cancel
    End Function

    Public Function DiskImageIsWin9xDisk(Disk As Disk) As Boolean
        Dim Response As Boolean = False
        Dim OEMNameWin9x = Disk.BootSector.IsWin9xOEMName
        Dim OEMName = Disk.BootSector.OEMName

        If OEMNameWin9x Then
            Dim BootstrapType = App.Globals.BootstrapDB.FindMatch(Disk.BootSector.BootStrapCode)

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

    Public Function DiskImageLoadIfNeeded(CurrentImage As DiskImageContainer, ImageData As ImageData) As DiskImageContainer
        Dim Image As DiskImageContainer = Nothing

        Do
            If ImageData Is CurrentImage.ImageData Then
                Image = CurrentImage
            Else
                Dim Disk = DiskImageLoadFromImageData(ImageData)
                If Disk IsNot Nothing Then
                    Image = New DiskImageContainer(Disk, ImageData)
                End If
            End If

            If Image Is Nothing Then
                Dim Msg As String = String.Format(My.Resources.Dialog_OpenFileError, ImageData.GetSaveFile)
                Dim ErrorResult = MsgBox(Msg, MsgBoxStyle.Critical + MsgBoxStyle.RetryCancel)
                If ErrorResult = MsgBoxResult.Cancel Then
                    Exit Do
                End If
            End If
        Loop Until Image IsNot Nothing

        Return Image
    End Function

    Public Sub DiskImageLoadIntoFilePanel(FilePanel As FilePanel, ImageData As ImageData, DoItemScan As Boolean)
        If ImageData Is Nothing Then
            FilePanel.Load(Nothing, DoItemScan)
            Exit Sub
        End If

        Cursor.Current = Cursors.WaitCursor

        Dim Disk = DiskImageLoadFromImageData(ImageData, True)

        Dim Image As New DiskImageContainer(Disk, ImageData)

        If Image IsNot Nothing Then
            If Image.ImageData.ExternalModified Then
                Image.ImageData.ExternalModified = False
                DoItemScan = True
                Dim Msg = String.Format(My.Resources.Dialog_ChangesDiscarded, Environment.NewLine)
                MsgBox(Msg, MsgBoxStyle.Exclamation)
            End If

            FilePanel.Load(Image, DoItemScan)
        End If

        Cursor.Current = Cursors.Default
    End Sub

    Public Sub DiskImageProcessEvent(FilePanel As FilePanel, MenuItem As DiskImageMenuItem, Optional Data As Object = Nothing)
        Dim DoRefresh As Boolean = False

        Select Case MenuItem
            Case DiskImageMenuItem.BootSectorEdit
                DoRefresh = BootSectorEdit(FilePanel.CurrentImage.Disk)

            Case DiskImageMenuItem.BootSectorRemoveFromDirectory
                BootSectorRemoveFromDirectory(FilePanel.CurrentImage.Disk)
                DoRefresh = True

            Case DiskImageMenuItem.BootSectorRestoreFromDirectory
                BootSectorRestoreFromDirectory(FilePanel.CurrentImage.Disk)
                DoRefresh = True

            Case DiskImageMenuItem.EditFAT
                If Data IsNot Nothing Then
                    DoRefresh = FATEdit(FilePanel.CurrentImage.Disk, CUShort(Data))
                End If

            Case DiskImageMenuItem.HexDisplayBadSectors
                DoRefresh = HexDisplayBadSectors(FilePanel.CurrentImage.Disk)

            Case DiskImageMenuItem.HexDisplayBootSector
                DoRefresh = HexDisplayBootSector(FilePanel.CurrentImage.Disk)

            Case DiskImageMenuItem.HexDisplayDirectory
                Dim Directory = TryCast(Data, IDirectory)
                If Directory IsNot Nothing Then
                    DoRefresh = HexDisplayDirectory(FilePanel.CurrentImage.Disk, Directory)
                End If

            Case DiskImageMenuItem.HexDisplayDirectoryEntry
                Dim DirectoryEntry = TryCast(Data, DirectoryEntry)
                If DirectoryEntry IsNot Nothing Then
                    DoRefresh = HexDisplayDirectoryEntry(FilePanel.CurrentImage.Disk, DirectoryEntry)
                End If

            Case DiskImageMenuItem.HexDisplayDisk
                DoRefresh = HexDisplayDisk(FilePanel.CurrentImage.Disk)

            Case DiskImageMenuItem.HexDisplayFAT
                DoRefresh = HexDisplayFAT(FilePanel.CurrentImage.Disk)

            Case DiskImageMenuItem.HexDisplayFreeClusters
                DoRefresh = HexDisplayFreeClusters(FilePanel.CurrentImage.Disk)

            Case DiskImageMenuItem.HexDisplayLostSectors
                DoRefresh = HexDisplayLostSectors(FilePanel.CurrentImage.Disk)

            Case DiskImageMenuItem.HexDisplayOverdumpData
                DoRefresh = HexDisplayOverdumpData(FilePanel.CurrentImage.Disk)

            Case DiskImageMenuItem.ImageClearReservedBytes
                DoRefresh = ImageClearReservedBytes(FilePanel.CurrentImage)

            Case DiskImageMenuItem.ImageFixImageSize
                DoRefresh = ImageFixImageSize(FilePanel.CurrentImage)

            Case DiskImageMenuItem.ImageRestructure
                DoRefresh = ImageRestructure(FilePanel.CurrentImage.Disk)

            Case DiskImageMenuItem.ImageUndo
                FilePanel.CurrentImage.Disk.Image.History.Redo()
                DoRefresh = True

            Case DiskImageMenuItem.ImageRedo
                FilePanel.CurrentImage.Disk.Image.History.Undo()
                DoRefresh = True

            Case DiskImageMenuItem.RemoveWindowsModifications
                DoRefresh = ImageRemoveWindowsModifications(FilePanel.CurrentImage.Disk, False)
        End Select

        If DoRefresh Then
            DiskImageRefresh(FilePanel)
        End If
    End Sub

    Public Sub DiskImageRefresh(FilePanel As FilePanel)
        FilePanel.CurrentImage?.Disk?.Reinitialize()

        FilePanel.Load(FilePanel.CurrentImage, True)
    End Sub

    Public Sub DiskImageReloadCurrent(FilePanel As FilePanel, RevertChanges As Boolean)
        If RevertChanges Then
            FilePanel.CurrentImage.ImageData.Modifications.Clear()
        End If

        DiskImageLoadIntoFilePanel(FilePanel, FilePanel.CurrentImage.ImageData, RevertChanges)
    End Sub

    Public Sub DiskImageRevertChanges(FilePanel As FilePanel)
        If FilePanel.CurrentImage.Disk.Image.History.Modified Then
            DiskImageReloadCurrent(FilePanel, True)
        End If
    End Sub

    Public Function DiskImageSave(Image As DiskImageContainer, Optional NewFilePath As String = "") As Boolean
        Dim Result As Boolean = False

        Do
            If NewFilePath = "" Then
                NewFilePath = Image.ImageData.GetSaveFile
            End If

            Dim SaveImageResponse = SaveDiskImageToFile(Image.Disk, NewFilePath, App.Globals.AppSettings.CreateBackups)
            Result = (SaveImageResponse = SaveImageResponse.Success)

            If SaveImageResponse = SaveImageResponse.Unsupported Then
                MsgBox(My.Resources.Dialog_SaveNotSupported, MsgBoxStyle.Exclamation)
                Exit Do
            ElseIf SaveImageResponse = SaveImageResponse.Unknown Then
                MsgBox(My.Resources.Dialog_UnsupportedDiskType, MsgBoxStyle.Exclamation)
                Exit Do
            ElseIf SaveImageResponse = SaveImageResponse.Cancelled Then
                Exit Do
            End If

            If Not Result Then
                Dim Msg As String = String.Format(My.Resources.Dialog_SaveFileError, IO.Path.GetFileName(NewFilePath))
                Dim ErrorResult = MsgBox(Msg, MsgBoxStyle.Critical + MsgBoxStyle.RetryCancel)
                If ErrorResult = MsgBoxResult.Cancel Then
                    Exit Do
                End If
            End If
        Loop Until Result

        If Result Then
            Image.Disk.ClearChanges()

            If Image.ImageData.SourceFile <> NewFilePath Then
                Image.ImageData.OldDisplayPath = Image.ImageData.DisplayPath
                Image.ImageData.FileNameChanged = True

                If Image.ImageData.FileType = ImageData.FileTypeEnum.NewImage Then
                    DeleteTempFileIfExists(Image.ImageData.SourceFile)
                End If

                Image.ImageData.SourceFile = NewFilePath
                Image.ImageData.FileType = ImageData.FileTypeEnum.Standard
                Image.ImageData.CompressedFile = ""
                Image.ImageData.ReadOnly = IsFileReadOnly(NewFilePath)
            End If

            Image.ImageData.Checksum = CRC32.ComputeChecksum(Image.Disk.Image.GetBytes)
            Image.ImageData.ExternalModified = False
        End If

        Return Result
    End Function
    Public Sub FilePanelProcessEvent(FilePanel As FilePanel, MenuItem As FilePanel.FilePanelMenuItem, Optional Directory As IDirectory = Nothing)
        Dim FileData = FilePanel.SelectedFileData
        Dim DoRefresh As Boolean = False

        Select Case MenuItem
            Case FilePanel.FilePanelMenuItem.FileProperties
                DoRefresh = FilePropertiesEdit(FilePanel)

            Case FilePanel.FilePanelMenuItem.ExportFile
                ImageFileExport(FilePanel)

            Case FilePanel.FilePanelMenuItem.ReplaceFile
                If FileData IsNot Nothing Then
                    DoRefresh = ImageReplaceFile(FileData.DirectoryEntry)
                End If

            Case FilePanel.FilePanelMenuItem.ViewDirectory
                If Directory IsNot Nothing Then
                    DoRefresh = HexDisplayDirectory(FilePanel.CurrentImage.Disk, Directory)
                End If

            Case FilePanel.FilePanelMenuItem.ViewFile
                If FileData IsNot Nothing Then
                    DoRefresh = HexDisplayDirectoryEntry(FilePanel.CurrentImage.Disk, FileData.DirectoryEntry)
                End If

            Case FilePanel.FilePanelMenuItem.ViewFileText
                If FileData IsNot Nothing Then
                    DirectoryEntryDisplayText(FileData.DirectoryEntry)
                End If

            Case FilePanel.FilePanelMenuItem.ViewCrosslinked
                If FileData IsNot Nothing Then
                    CrossLinkedFilesDisplay(FilePanel.CurrentImage.Disk, FileData.DirectoryEntry)
                End If

            Case FilePanel.FilePanelMenuItem.ImportFiles
                If Directory IsNot Nothing Then
                    DoRefresh = ImageImport(Directory, True)
                End If

            Case FilePanel.FilePanelMenuItem.ImportFilesHere
                If FileData IsNot Nothing Then
                    DoRefresh = ImageImport(FileData.DirectoryEntry.ParentDirectory, True, FileData.DirectoryEntry.Index)
                End If

            Case FilePanel.FilePanelMenuItem.NewDirectory
                If Directory IsNot Nothing Then
                    DoRefresh = ImageAddDirectory(Directory)
                End If

            Case FilePanel.FilePanelMenuItem.NewDirectoryHere
                If FileData IsNot Nothing Then
                    DoRefresh = ImageAddDirectory(FileData.DirectoryEntry.ParentDirectory, FileData.DirectoryEntry.Index)
                End If

            Case FilePanel.FilePanelMenuItem.DeleteFile
                DoRefresh = ImageDeleteSelectedFiles(FilePanel, False)

            Case FilePanel.FilePanelMenuItem.UnDeleteFile
                If FileData IsNot Nothing Then
                    DoRefresh = ImageUndeleteFile(FileData.DirectoryEntry)
                End If

            Case FilePanel.FilePanelMenuItem.FileRemove
                DoRefresh = ImageDeleteSelectedFiles(FilePanel, True)

            Case FilePanel.FilePanelMenuItem.FixSize
                If FileData IsNot Nothing Then
                    DoRefresh = ImageFixFileSize(FileData.DirectoryEntry)
                End If
        End Select

        If DoRefresh Then
            DiskImageRefresh(FilePanel)
        End If
    End Sub

    Public Function GetNewFilePath(CurrentImage As DiskImageContainer, LoadedFiles As LoadedFiles) As String
        Return GetNewFilePath(CurrentImage, CurrentImage.ImageData, LoadedFiles)
    End Function

    Public Function GetNewFilePath(CurrentImage As DiskImageContainer, ImageData As ImageData, LoadedFiles As LoadedFiles) As String
        Dim Disk As DiskImage.Disk
        Dim FilePath = ImageData.GetSaveFile
        Dim NewFilePath As String = ""
        Dim DiskFormat As FloppyDiskFormat = FloppyDiskFormat.FloppyUnknown
        Dim InitialDirectory As String = ""

        If ImageData Is CurrentImage.ImageData Then
            Disk = CurrentImage.Disk
        Else
            Disk = DiskImageLoadFromImageData(ImageData)
        End If

        If Disk IsNot Nothing Then
            DiskFormat = Disk.DiskParams.Format
        End If

        Dim FileExt = IO.Path.GetExtension(FilePath)
        Dim FileFilter = GetSaveDialogFilters(DiskFormat, Disk.Image.ImageType, FileExt)

        If ImageData.FileType <> ImageData.FileTypeEnum.NewImage Then
            InitialDirectory = IO.Path.GetDirectoryName(FilePath)
        Else
            InitialDirectory = App.UserState.LastNewImagePath
        End If

        Using Dialog As New SaveFileDialog With {
                .InitialDirectory = InitialDirectory,
                .FileName = IO.Path.GetFileName(FilePath),
                .Filter = FileFilter.Filter,
                .FilterIndex = FileFilter.FilterIndex,
                .DefaultExt = FileExt
            }

            AddHandler Dialog.FileOk,
                Sub(sender As Object, e As CancelEventArgs)
                    If Dialog.FileName <> FilePath AndAlso LoadedFiles.FileNames.ContainsKey(Dialog.FileName) Then
                        Dim Msg = String.Format(My.Resources.Dialog_FileCurrentlyOpen, IO.Path.GetFileName(Dialog.FileName), Environment.NewLine, Application.ProductName)
                        MsgBox(Msg, MsgBoxStyle.Exclamation, WithoutHotkey(My.Resources.Menu_SaveAs))
                        e.Cancel = True
                    End If
                End Sub

            If Dialog.ShowDialog = DialogResult.OK Then
                NewFilePath = Dialog.FileName
                If ImageData.FileType = ImageData.FileTypeEnum.NewImage Then
                    App.UserState.LastNewImagePath = IO.Path.GetDirectoryName(NewFilePath)
                End If
            End If
        End Using

        Return NewFilePath
    End Function

    Public Function MsgBoxNewFileName(FileName As String) As MsgBoxResult
        Dim Msg As String = String.Format(My.Resources.Dialog_NewFileName, FileName)
        Return MsgBox(Msg, MsgBoxStyle.OkCancel)
    End Function

    Public Function MsgBoxSave(FileName As String) As MsgBoxResult
        Dim Msg As String = String.Format(My.Resources.Dialog_SaveFile, FileName)

        Return MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNoCancel + MsgBoxStyle.DefaultButton3, "Save")
    End Function

    Public Function MsgBoxSaveAll(FileName As String) As MyMsgBoxResult
        Dim Msg As String = String.Format(My.Resources.Dialog_SaveFile, FileName)

        Dim SaveAllForm As New SaveAllForm(Msg)
        SaveAllForm.ShowDialog()
        Return SaveAllForm.Result
    End Function
    Public Sub NewImageImport(FilePanel As FilePanel, FileName As String)
        If FilePanel.CurrentImage Is Nothing Then
            Exit Sub
        End If

        If FilePanel.CurrentImage.ImageData.SourceFile = FileName Then
            If ImageImport(FilePanel.CurrentImage.Disk.RootDirectory, True) Then
                DiskImageRefresh(FilePanel)
            End If
        End If
    End Sub

    Private Function BootSectorEdit(Disk As Disk) As Boolean
        Dim BootSectorForm As New BootSectorForm(Disk.BootSector.Data, App.Globals.BootstrapDB)

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

    Private Sub CrossLinkedFilesDisplay(Disk As Disk, DirectoryEntry As DiskImage.DirectoryEntry)
        Dim Msg = String.Format(My.Resources.Dialog_CrossLinked, DirectoryEntry.GetShortFileName(True), Environment.NewLine)

        For Each Crosslink In DirectoryEntry.CrossLinks
            If Crosslink IsNot DirectoryEntry Then
                Msg &= Environment.NewLine & Crosslink.GetShortFileName(True)
            End If
        Next
        MsgBox(Msg, MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
    End Sub

    Private Sub DirectoryEntryDisplayText(DirectoryEntry As DiskImage.DirectoryEntry)
        If Not DirectoryEntry.IsValidFile Then 'Or DirectoryEntry.IsDeleted Then
            Exit Sub
        End If

        Dim Caption As String

        If DirectoryEntry.IsDeleted Then
            Caption = My.Resources.Caption_DeleteFile & " - " & DirectoryEntry.GetShortFileName(True)
        Else
            Caption = My.Resources.Label_File & " - " & DirectoryEntry.GetShortFileName(True)
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

        Dim frmTextView As New TextViewForm(Caption, Content, False, True, DirectoryEntry.GetFullFileName)
        frmTextView.ShowDialog()
    End Sub

    Private Function FATEdit(Disk As Disk, Index As UShort) As Boolean
        Dim frmFATEdit As New FATEditForm(Disk, Index)

        frmFATEdit.ShowDialog()

        Return frmFATEdit.Updated
    End Function

    Private Function FilePropertiesEdit(FilePanel As FilePanel) As Boolean
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

    Private Function ImageAddDirectory(ParentDirectory As IDirectory, Optional Index As Integer = -1) As Boolean
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

    Private Function ImageDeleteSelectedFiles(FilePanel As FilePanel, Remove As Boolean) As Boolean
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

    Private Function ImageFixFileSize(DirectoryEntry As DirectoryEntry)
        If Not DirectoryEntry.HasIncorrectFileSize Then
            Return False
        End If

        DirectoryEntry.FileSize = DirectoryEntry.GetAllocatedSizeFromFAT

        Return True
    End Function

    Private Function ImageImport(ParentDirectory As IDirectory, Multiselect As Boolean, Optional Index As Integer = -1) As Boolean
        Dim FileNames() As String = New String(-1) {}

        If App.Globals.AppSettings.DragAndDrop Then
            Dim frmFileDrop As New FileDropForm()

            If frmFileDrop.ShowDialog() <> DialogResult.OK Then
                Return False
            End If

            FileNames = frmFileDrop.FileNames
        Else
            Using Dialog As New OpenFileDialog With {
                   .Multiselect = Multiselect
               }

                If Dialog.ShowDialog <> DialogResult.OK Then
                    Return False
                End If

                FileNames = Dialog.FileNames
            End Using
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

                Dim DirectoryEntry As New DirectoryEntryBase
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

    Private Function ImageReplaceFile(DirectoryEntry As DirectoryEntry) As Boolean
        Dim FileName As String

        Using Dialog As New OpenFileDialog
            If Dialog.ShowDialog <> DialogResult.OK Then
                Return False
            End If

            FileName = Dialog.FileName
        End Using

        Dim FormResult As ReplaceFileForm.ReplaceFileFormResult
        Dim FileInfo As New IO.FileInfo(FileName)

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

        Result = DirectoryEntry.UpdateFile(FileName, FormResult.FileSize, FormResult.FillChar, FreeClusters)
        If Not Result Then
            Result = DirectoryEntry.UpdateFile(FileName, FormResult.FileSize, FormResult.FillChar)
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

    Private Function ImageUndeleteFile(DirectoryEntry As DirectoryEntry) As Boolean
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
End Module
