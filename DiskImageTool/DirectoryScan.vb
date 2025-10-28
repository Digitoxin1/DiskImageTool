Module DirectoryScan
    Public Function ProcessDirectoryEntries(Directory As DiskImage.IDirectory, FilePanel As FilePanel) As DirectoryScanResponse
        Dim ItemIndex As Integer = 0

        Dim Response = ProcessDirectoryEntries(Directory, 0, "", FilePanel, 0, ItemIndex)
        Response.ItemCount = ItemIndex

        Return Response
    End Function

    Public Function ProcessDirectoryEntries(Directory As DiskImage.IDirectory, Offset As UInteger, Path As String, FilePanel As FilePanel, ByRef GroupIndex As Integer, ByRef ItemIndex As Integer) As DirectoryScanResponse
        Dim Response As New DirectoryScanResponse(Directory)
        Dim Group As ListViewGroup = Nothing

        If FilePanel IsNot Nothing Then
            Group = FilePanel.AddGroup(Directory, Path, GroupIndex)
            GroupIndex += 1
        End If

        Dim DirectoryEntryCount = Directory.Data.EntryCount

        If DirectoryEntryCount > 0 Then
            Dim HasLFN As Boolean = False
            Dim LFNFileName As String = ""
            Dim LFNIndex As Byte = 0
            Dim LFNChecksum As Byte = 0

            Dim EntryCount = 0
            For Counter = 0 To DirectoryEntryCount - 1
                Dim DirectoryEntry = Directory.GetFile(Counter)

                If Not DirectoryEntry.IsLink Then
                    If DirectoryEntry.IsLFN Then
                        If LFNIndex > 0 Then
                            LFNIndex -= 1
                        End If
                        LFNIndex = DirectoryEntry.LFNGetNextSequence(LFNIndex, LFNChecksum, True)
                        If LFNIndex > 0 Then
                            LFNChecksum = DirectoryEntry.LFNChecksum
                            If (LFNIndex And &H40) > 0 Then
                                LFNIndex = LFNIndex And Not &H40
                                LFNFileName = DirectoryEntry.GetLFNFileName
                            Else
                                LFNFileName = DirectoryEntry.GetLFNFileName & LFNFileName
                            End If
                            HasLFN = True
                        Else
                            LFNFileName = ""
                            HasLFN = False
                        End If
                    Else
                        If HasLFN Then
                            If LFNIndex <> 1 Or LFNChecksum <> DirectoryEntry.CalculateLFNChecksum Then
                                LFNFileName = ""
                                HasLFN = False
                            End If
                        End If
                        If Not HasLFN Then
                            If Not DirectoryEntry.HasNTUnknownFlags And (DirectoryEntry.HasNTLowerCaseFileName Or DirectoryEntry.HasNTLowerCaseExtension) Then
                                LFNFileName = DirectoryEntry.GetNTFileName
                            End If
                        End If
                        Dim ProcessResponse = Response.ProcessDirectoryEntry(DirectoryEntry, LFNFileName, Path = "")

                        If FilePanel IsNot Nothing Then
                            Dim FileData As New FileData With {
                                .Index = Counter,
                                .FilePath = Path,
                                .DirectoryEntry = DirectoryEntry,
                                .IsLastEntry = (Counter = DirectoryEntryCount - 1),
                                .LFNFileName = LFNFileName,
                                .DuplicateFileName = ProcessResponse.DuplicateFileName,
                                .InvalidVolumeName = ProcessResponse.InvalidVolumeName
                            }
                            Dim Item = FilePanel.AddItem(FileData, Group, ItemIndex)
                            ItemIndex += 1
                        End If

                        If DirectoryEntry.IsDirectory And DirectoryEntry.SubDirectory IsNot Nothing Then
                            If DirectoryEntry.SubDirectory.Data.EntryCount > 0 Then
                                Dim NewPath As String
                                If HasLFN Then
                                    NewPath = LFNFileName
                                Else
                                    NewPath = DirectoryEntry.GetShortFileName
                                End If
                                If Path <> "" Then
                                    NewPath = Path & "\" & NewPath
                                End If

                                If FilePanel IsNot Nothing Then
                                    Response.AddDirectory(NewPath, DirectoryEntry.SubDirectory)
                                End If
                                Dim SubResponse = ProcessDirectoryEntries(DirectoryEntry.SubDirectory, DirectoryEntry.Offset, NewPath, FilePanel, GroupIndex, ItemIndex)
                                Response.Combine(SubResponse)
                            End If
                        End If

                        LFNFileName = ""
                        LFNIndex = 0
                        LFNChecksum = 0
                        HasLFN = False
                    End If
                    EntryCount += 1
                End If
            Next

            If FilePanel IsNot Nothing And EntryCount = 0 Then
                Dim Item = FilePanel.AddEmptyItem(Group, ItemIndex)
                ItemIndex += 1
            End If

        ElseIf FilePanel IsNot Nothing Then
            Dim Item = FilePanel.AddEmptyItem(Group, ItemIndex)
            ItemIndex += 1
        End If

        Return Response
    End Function
End Module
