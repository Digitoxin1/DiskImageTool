Module Updater
    Public Sub CheckForUpdates()
        Dim DownloadVersion As String = ""
        Dim DownloadURL As String = ""
        Dim Body As String = ""
        Dim UpdateAvailable As Boolean = False
        Dim ErrMsg As String = My.Resources.Dialog_UpdateError

        Cursor.Current = Cursors.WaitCursor
        Dim ResponseText = GetAppUpdateResponse()
        Cursor.Current = Cursors.Default

        If ResponseText = "" Then
            MsgBox(ErrMsg, MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        Try
            Dim JSON As Dictionary(Of String, Object) = CompactJson.Serializer.Parse(Of Dictionary(Of String, Object))(ResponseText)

            If JSON.ContainsKey("tag_name") Then
                DownloadVersion = JSON.Item("tag_name").ToString
                If DownloadVersion.StartsWith("v", StringComparison.OrdinalIgnoreCase) Then
                    DownloadVersion = DownloadVersion.Remove(0, 1)
                End If
            End If

            If JSON.ContainsKey("assets") Then
                Dim assets() As Dictionary(Of String, Object) = CompactJson.Serializer.Parse(Of Dictionary(Of String, Object)())(JSON.Item("assets").ToString)
                If assets.Length > 0 Then
                    If assets(0).ContainsKey("browser_download_url") Then
                        DownloadURL = assets(0).Item("browser_download_url").ToString
                    End If
                End If
            End If

            If JSON.ContainsKey("body") Then
                Body = JSON.Item("body").ToString
            End If

            If DownloadVersion <> "" And DownloadURL <> "" Then
                Dim CurrentVersion = GetVersionString()
                UpdateAvailable = Version.Parse(DownloadVersion) > Version.Parse(CurrentVersion)
            End If

        Catch ex As Exception
            MsgBox(ErrMsg, MsgBoxStyle.Exclamation)
            DebugException(ex)
            Exit Sub
        End Try

        If UpdateAvailable Then
            Dim Msg = String.Format(My.Resources.Dialog_UpdateAvailable, My.Application.Info.Title, DownloadVersion)
            If Body <> "" Then
                Msg &= String.Format(My.Resources.Dialog_UpdateWhatsNew, Environment.NewLine, New String("—", 6), Body)
            End If
            Msg &= String.Format(My.Resources.Dialog_UpdateDownload, Environment.NewLine)

            If MsgBoxQuestion(Msg) Then
                Dim Dialog As New SaveFileDialog With {
                    .Filter = FileDialogGetFilter(My.Resources.FileType_ZipArchive, ".zip"),
                    .FileName = IO.Path.GetFileName(DownloadURL),
                    .InitialDirectory = GetDownloadsFolder(),
                    .RestoreDirectory = True
                }
                Dialog.ShowDialog()
                If Dialog.FileName <> "" Then
                    Cursor.Current = Cursors.WaitCursor
                    Try
                        Dim Client As New Net.WebClient()
                        Client.DownloadFile(DownloadURL, Dialog.FileName)
                    Catch ex As Exception
                        MsgBox(My.Resources.Dialog_FileDownloadError, MsgBoxStyle.Exclamation)
                        DebugException(ex)
                    End Try
                    Cursor.Current = Cursors.Default
                End If
            End If
        Else
            MsgBox(String.Format(My.Resources.Dialog_LatestVersion, My.Application.Info.Title), MsgBoxStyle.Information)
        End If
    End Sub

    Public Function CheckIfUpdatesExist() As Boolean
        Dim DownloadVersion As String = ""
        Dim DownloadURL As String = ""
        Dim UpdateAvailable As Boolean = False

        Dim ResponseText = GetAppUpdateResponse()

        If ResponseText <> "" Then
            Try
                Dim JSON As Dictionary(Of String, Object) = CompactJson.Serializer.Parse(Of Dictionary(Of String, Object))(ResponseText)

                If JSON.ContainsKey("tag_name") Then
                    DownloadVersion = JSON.Item("tag_name").ToString
                    If DownloadVersion.StartsWith("v", StringComparison.OrdinalIgnoreCase) Then
                        DownloadVersion = DownloadVersion.Remove(0, 1)
                    End If
                End If

                If JSON.ContainsKey("assets") Then
                    Dim assets() As Dictionary(Of String, Object) = CompactJson.Serializer.Parse(Of Dictionary(Of String, Object)())(JSON.Item("assets").ToString)
                    If assets.Length > 0 Then
                        If assets(0).ContainsKey("browser_download_url") Then
                            DownloadURL = assets(0).Item("browser_download_url").ToString
                        End If
                    End If
                End If

                If DownloadVersion <> "" And DownloadURL <> "" Then
                    Dim CurrentVersion = GetVersionString()
                    UpdateAvailable = Version.Parse(DownloadVersion) > Version.Parse(CurrentVersion)
                End If

            Catch ex As Exception
                DebugException(ex)
            End Try
        End If

        Return UpdateAvailable
    End Function
End Module
