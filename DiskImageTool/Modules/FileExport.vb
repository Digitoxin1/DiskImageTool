Module FileExport
    Public Sub DragDropExportSelectedFiles(FilePanel As FilePanel)
        If FilePanel.SelectedItems.Count = 0 Then
            Exit Sub
        End If

        Dim TempPath As String = IO.Path.GetTempPath() & Guid.NewGuid().ToString() & "\"

        ImageFileExportToTemp(FilePanel, TempPath)

        If IO.Directory.Exists(TempPath) Then
            Dim FileList = IO.Directory.EnumerateDirectories(TempPath)
            For Each FilePath In IO.Directory.GetFiles(TempPath)
                FileList = FileList.Append(FilePath)
            Next
            If FileList.Count > 0 Then
                Dim Data = New DataObject(DataFormats.FileDrop, FileList.ToArray)
                FilePanel.DoDragDrop(Data, DragDropEffects.Move)
            End If
            IO.Directory.Delete(TempPath, True)
        End If
    End Sub

    Public Sub ImageFileExport(FilePanel As FilePanel)
        If FilePanel.SelectedItems.Count = 1 Then
            Dim FileData = TryCast(FilePanel.FirstSelectedItem.Tag, FileData)
            If FileData IsNot Nothing Then
                DirectoryEntryExport(FileData)
            End If
        ElseIf FilePanel.SelectedItems.Count > 1 Then
            ImageFileExportSelected(FilePanel.SelectedItems)
        End If
    End Sub

    Private Sub ImageFileExportSelected(SelectedItems As ListView.SelectedListViewItemCollection)
        Dim Dialog = New FolderBrowserDialog

        If Dialog.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If
        Dim Path = Dialog.SelectedPath

        Dim ShowDialog As Boolean = True
        Dim BatchResult As MyMsgBoxResult = MyMsgBoxResult.Yes
        Dim Result As MyMsgBoxResult = MyMsgBoxResult.Yes
        Dim FileCount As Integer = 0
        Dim TotalFiles As Integer = 0
        For Each Item As ListViewItem In SelectedItems
            Dim FileData = TryCast(Item.Tag, FileData)
            If FileData IsNot Nothing Then
                Dim DirectoryEntry = FileData.DirectoryEntry
                If DiskImage.DirectoryEntryCanExport(DirectoryEntry) Then
                    TotalFiles += 1
                    If Result <> MyMsgBoxResult.Cancel Then
                        If DirectoryEntry.IsDirectory Then
                            Dim PathName = IO.Path.Combine(Path, CleanPathName(FileData.FilePath), CleanFileName(DirectoryEntry.GetFullFileName))

                            If Not IO.Directory.Exists(PathName) Then
                                IO.Directory.CreateDirectory(PathName)
                            End If
                            FileCount += 1
                        Else
                            Dim PathName = IO.Path.Combine(Path, CleanPathName(FileData.FilePath))

                            If Not IO.Directory.Exists(PathName) Then
                                IO.Directory.CreateDirectory(PathName)
                            End If

                            Dim FileName = CleanFileName(DirectoryEntry.GetFullFileName)

                            Dim FilePath = IO.Path.Combine(PathName, FileName)

                            If IO.File.Exists(FilePath) Then
                                If ShowDialog Then
                                    Result = MsgBoxOverwrite(FilePath)
                                Else
                                    Result = BatchResult
                                End If
                            Else
                                Result = MyMsgBoxResult.Yes
                            End If

                            If Result = MyMsgBoxResult.YesToAll Or Result = MyMsgBoxResult.NoToAll Then
                                ShowDialog = False
                                If Result = MyMsgBoxResult.NoToAll Then
                                    BatchResult = MyMsgBoxResult.No
                                End If
                            End If

                            If Result = MyMsgBoxResult.Yes Or Result = MyMsgBoxResult.YesToAll Then
                                If DirectoryEntrySaveToFile(FilePath, DirectoryEntry) Then
                                    FileCount += 1
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Next

        Dim Msg As String
        If TotalFiles = 1 Then
            Msg = String.Format(My.Resources.Dialog_SuccessfulExportSingular, FileCount, TotalFiles)
        Else
            Msg = String.Format(My.Resources.Dialog_SuccessfulExportPlural, FileCount, TotalFiles)
        End If

        MsgBox(Msg, MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
    End Sub

    Private Sub ImageFileExportToTemp(FilePanel As FilePanel, TempPath As String)
        For Each Item As ListViewItem In FilePanel.SelectedItems
            Dim FileData = TryCast(Item.Tag, FileData)
            If FileData IsNot Nothing Then
                Dim DirectoryEntry = FileData.DirectoryEntry

                If DiskImage.DirectoryEntryCanExport(DirectoryEntry) Then
                    If DirectoryEntry.IsDirectory Then
                        Dim PathName = IO.Path.Combine(TempPath, CleanPathName(FileData.FilePath), CleanFileName(DirectoryEntry.GetFullFileName))

                        If Not IO.Directory.Exists(PathName) Then
                            IO.Directory.CreateDirectory(PathName)
                        End If
                    Else
                        Dim PathName = IO.Path.Combine(TempPath, CleanPathName(FileData.FilePath))

                        If Not IO.Directory.Exists(PathName) Then
                            IO.Directory.CreateDirectory(PathName)
                        End If

                        Dim FileName = CleanFileName(DirectoryEntry.GetFullFileName)

                        Dim FilePath = IO.Path.Combine(PathName, FileName)

                        DirectoryEntrySaveToFile(FilePath, DirectoryEntry)
                    End If
                End If
            End If
        Next
    End Sub

    Private Function MsgBoxOverwrite(FilePath As String) As MyMsgBoxResult
        Dim msg As String = String.Format(My.Resources.Dialog_ReplaceFile, IO.Path.GetFileName(FilePath), Environment.NewLine)

        Dim SaveAllForm As New SaveAllForm(msg)
        SaveAllForm.ShowDialog()
        Return SaveAllForm.Result
    End Function
End Module
