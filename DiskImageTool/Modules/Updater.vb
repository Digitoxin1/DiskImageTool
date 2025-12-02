Imports System.IO
Imports System.IO.Compression
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Net.NetworkInformation

Module Updater
    Public Function CheckForUpdates() As Boolean
        Dim AppResponse As UpdateResponse
        Dim DBResponse? As UpdateResponse = Nothing

        Cursor.Current = Cursors.WaitCursor
        AppResponse = ProcessUpdateResponse(GetAppUpdateResponse(), GetVersionString())

        If AppResponse.HasException Then
            MsgBox(My.Resources.Dialog_UpdateError, MsgBoxStyle.Exclamation)
            Cursor.Current = Cursors.Default
            Return False
        End If

        If Not AppResponse.UpdateAvailable Then
            DBResponse = ProcessUpdateResponse(GetDatabaseUpdateResponse(), App.TitleDB.Version)
        End If

        Cursor.Current = Cursors.Default

        If AppResponse.UpdateAvailable Then
            Return DisplayAppUpdateDialog(AppResponse)
        ElseIf DBResponse IsNot Nothing AndAlso DBResponse.Value.UpdateAvailable Then
            Return DisplayDBUpdateDialog(DBResponse.Value)
        Else
            MsgBox(String.Format(My.Resources.Dialog_LatestVersion, My.Application.Info.Title), MsgBoxStyle.Information)
        End If

        Return False
    End Function

    Public Function CheckIfAppUpdateAvailable() As Boolean
        Dim UpdateAvailable = ProcessUpdateResponse(GetAppUpdateResponse(), GetVersionString()).UpdateAvailable

        If Not UpdateAvailable Then
            UpdateAvailable = ProcessUpdateResponse(GetDatabaseUpdateResponse(), App.TitleDB.Version).UpdateAvailable
        End If

        Return UpdateAvailable
    End Function

    Private Function CompareVersions(v1 As String, v2 As String) As Integer
        Dim ver1 As Version = Nothing
        Dim ver2 As Version = Nothing

        ' Try Version.Parse on both
        Dim ok1 = Version.TryParse(v1, ver1)
        Dim ok2 = Version.TryParse(v2, ver2)

        If ok1 AndAlso ok2 Then
            Return ver1.CompareTo(ver2)
        End If

        ' Fallback: simple string compare
        Return String.Compare(v1, v2, StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function DisplayAppUpdateDialog(Response As UpdateResponse) As Boolean
        Dim Msg = String.Format(My.Resources.Dialog_UpdateAvailable, My.Application.Info.Title, Response.Version)
        If Response.Body <> "" Then
            Msg &= String.Format(My.Resources.Dialog_UpdateWhatsNew, Environment.NewLine, New String("—", 6), Response.Body)
        End If
        Msg &= String.Format(My.Resources.Dialog_UpdateDownload, Environment.NewLine)

        If Not MsgBoxQuestion(Msg) Then
            Return False
        End If

        Dim FileName As String = ""

        Using Dialog As New SaveFileDialog With {
                .Filter = FileDialogGetFilter(My.Resources.FileType_ZipArchive, ".zip"),
                .FileName = Path.GetFileName(Response.URL),
                .InitialDirectory = GetDownloadsFolder(),
                .RestoreDirectory = True
            }

            Dialog.ShowDialog()

            FileName = Dialog.FileName
        End Using

        If String.IsNullOrEmpty(FileName) Then
            Return False
        End If

        Cursor.Current = Cursors.WaitCursor
        Try
            Using client As New HttpClient()
                client.Timeout = TimeSpan.FromSeconds(30)
                client.DefaultRequestHeaders.UserAgent.ParseAdd(My.Application.Info.ProductName)

                Dim data As Byte() = client.GetByteArrayAsync(Response.URL).GetAwaiter().GetResult()
                File.WriteAllBytes(FileName, data)
            End Using

            Return True

        Catch ex As Exception
            MsgBox(My.Resources.Dialog_FileDownloadError, MsgBoxStyle.Exclamation)
            DebugException(ex)

            Return False
        Finally
            Cursor.Current = Cursors.Default
        End Try
    End Function

    Private Function DisplayDBUpdateDialog(Response As UpdateResponse) As Boolean
        Dim Msg = My.Resources.Dialog_DatabaseUpdateAvailable & String.Format(My.Resources.Dialog_UpdateDownload, Environment.NewLine)

        If Not MsgBoxQuestion(Msg) Then
            Return False
        End If

        Dim SaveSuccessful As Boolean = False
        Dim TempPath = InitTempPath()

        If String.IsNullOrEmpty(TempPath) Then
            MsgBox(My.Resources.Dialog_FileDownloadError, MsgBoxStyle.Exclamation)
            Return False
        End If

        Dim ZipFilePath As String = Path.Combine(TempPath, Path.GetFileName(Response.URL))
        Dim DBFileName As String = FloppyDB.MAIN_DB_FILE_NAME

        Cursor.Current = Cursors.WaitCursor

        Try
            Using client As New HttpClient()
                client.Timeout = TimeSpan.FromSeconds(30)
                client.DefaultRequestHeaders.UserAgent.ParseAdd(My.Application.Info.ProductName)

                Dim data As Byte() = client.GetByteArrayAsync(Response.URL).GetAwaiter().GetResult()
                File.WriteAllBytes(ZipFilePath, data)
            End Using

            Using archive As ZipArchive = ZipFile.OpenRead(ZipFilePath)
                Dim entry = archive.Entries.FirstOrDefault(Function(e) String.Equals(e.Name, DBFileName, StringComparison.OrdinalIgnoreCase))

                If entry Is Nothing Then
                    Throw New FileNotFoundException($"Could not find {DBFileName} in the downloaded archive.")
                End If

                Dim AppPath = GetAppPath()
                Dim DestPath = Path.Combine(AppPath, DBFileName)

                Try
                    entry.ExtractToFile(DestPath, overwrite:=True)
                    SaveSuccessful = True
                Catch ex As Exception
                End Try

                If Not SaveSuccessful Then
                    Dim DataPath = InitDataPath()
                    If Not String.IsNullOrEmpty(DataPath) Then
                        DestPath = Path.Combine(DataPath, DBFileName)
                        Try
                            entry.ExtractToFile(DestPath, overwrite:=True)
                            SaveSuccessful = True
                            Dim WarningMsg = String.Format(My.Resources.Dialog_DatabaseUpdateWarning, DBFileName, AppPath, DataPath, Environment.NewLine)
                            MsgBox(WarningMsg, vbExclamation)
                        Catch ex As Exception
                        End Try
                    End If
                End If
            End Using

            DeleteTempFileIfExists(ZipFilePath)

            If SaveSuccessful Then
                App.TitleDB.Load()

                MsgBox(My.Resources.Dialog_DatabaseUpdateSuccessful, MsgBoxStyle.Information)

                Return True
            Else
                MsgBox(My.Resources.Dialog_DatabaseUpdateError, MsgBoxStyle.Critical)

                Return False
            End If

        Catch ex As Exception
            MsgBox(My.Resources.Dialog_FileDownloadError, MsgBoxStyle.Critical)
            DebugException(ex)

            Return False
        Finally
            Cursor.Current = Cursors.Default
        End Try
    End Function

    Private Function ProcessUpdateResponse(ResponseText As String, CurrentVersion As String) As UpdateResponse
        Dim Response As New UpdateResponse With {
            .Version = "",
            .URL = "",
            .Body = "",
            .UpdateAvailable = False,
            .HasException = False
        }

        If ResponseText <> "" Then
            Try
                Dim JSON As Dictionary(Of String, Object) = CompactJson.Serializer.Parse(Of Dictionary(Of String, Object))(ResponseText)

                If JSON.ContainsKey("tag_name") Then
                    Response.Version = JSON.Item("tag_name").ToString
                End If

                If JSON.ContainsKey("assets") Then
                    Dim assets() As Dictionary(Of String, Object) = CompactJson.Serializer.Parse(Of Dictionary(Of String, Object)())(JSON.Item("assets").ToString)
                    If assets.Length > 0 Then
                        If assets(0).ContainsKey("browser_download_url") Then
                            Response.URL = assets(0).Item("browser_download_url").ToString
                        End If
                    End If
                End If

                If JSON.ContainsKey("body") Then
                    Response.Body = JSON.Item("body").ToString
                End If

                If Response.Version <> "" And Response.URL <> "" Then
                    Dim CheckVersion = Response.Version
                    If CheckVersion.StartsWith("v", StringComparison.OrdinalIgnoreCase) Then
                        CheckVersion = CheckVersion.Remove(0, 1)
                    End If
                    Response.UpdateAvailable = CompareVersions(CheckVersion, CurrentVersion) > 0
                End If

            Catch ex As Exception
                Response.HasException = True
                DebugException(ex)
            End Try
        Else
            Response.HasException = True
        End If

        Return Response
    End Function
    Private Structure UpdateResponse
        Public Body As String
        Public HasException As Boolean
        Public UpdateAvailable As Boolean
        Public URL As String
        Public Version As String
    End Structure
End Module
