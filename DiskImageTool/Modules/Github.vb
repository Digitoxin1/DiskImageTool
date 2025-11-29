Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers

Module Github
    Private ReadOnly APP_REPOSITORY As String = My.Resources.URL_AppAPI_EndPoint
    Private ReadOnly APP_CHANGELOG_URL As String = APP_REPOSITORY & "/releases?per_page=100"
    Private ReadOnly APP_UPDATE_URL As String = APP_REPOSITORY & "/releases/latest"
    Private ReadOnly DB_REPOSITORY As String = My.Resources.URL_DBAPI_EndPoint
    Private ReadOnly DB_UPDATE_URL As String = DB_REPOSITORY & "/releases/latest"
    Private ReadOnly USER_AGENT As String = My.Application.Info.ProductName

    Public Function GetAppUpdateResponse() As String
        Dim Response = GetUpdateResponse(APP_UPDATE_URL, "AppUpdateResponse.cache", App.Globals.UserState.ETags.AppUpdate)
        App.Globals.UserState.ETags.AppUpdate = Response.NewETag

        Return Response.Content
    End Function

    Public Function GetChangeLogResponse() As String
        Dim Response = GetUpdateResponse(APP_CHANGELOG_URL, "ChangeLogResponse.cache", App.Globals.UserState.ETags.ChangeLog)
        App.Globals.UserState.ETags.ChangeLog = Response.NewETag

        Return Response.Content
    End Function

    Public Function GetDatabaseUpdateResponse() As String
        Dim Response = GetUpdateResponse(DB_UPDATE_URL, "DBUpdateResponse.cache", App.Globals.UserState.ETags.DBUpdate)
        App.Globals.UserState.ETags.DBUpdate = Response.NewETag

        Return Response.Content
    End Function

    Private Function GetUpdateResponse(UpdateURL As String, CacheFile As String, ETag As String) As (Content As String, NewETag As String)
        Return GetUpdateResponseAsync(UpdateURL, CacheFile, ETag).GetAwaiter().GetResult()
    End Function

    Private Async Function GetUpdateResponseAsync(UpdateURL As String, CacheFile As String, ETag As String) As Task(Of (Content As String, NewETag As String))
        Dim TempPath = InitTempPath()
        Dim CachePath = IO.Path.Combine(TempPath, CacheFile)

        Dim ResponseText As String = ""
        Dim NotModified As Boolean = False
        Dim NewETag As String = ETag

        If Not IO.File.Exists(CachePath) Then
            ETag = ""
        End If

        Try
            Using client As New HttpClient()
                client.Timeout = TimeSpan.FromSeconds(30)
                ' GitHub requires a User-Agent header
                client.DefaultRequestHeaders.UserAgent.ParseAdd(USER_AGENT)
                client.DefaultRequestHeaders.Accept.Add(
                    New MediaTypeWithQualityHeaderValue("application/vnd.github+json"))

                Using request As New HttpRequestMessage(HttpMethod.Get, UpdateURL)

                    ' Add If-None-Match if we have an ETag
                    If Not String.IsNullOrEmpty(ETag) Then
                        request.Headers.TryAddWithoutValidation("If-None-Match", ETag)
                    End If

                    Using response As HttpResponseMessage = Await client.SendAsync(request).ConfigureAwait(False)
                        If response.StatusCode = HttpStatusCode.NotModified Then
                            NotModified = True
                        ElseIf response.IsSuccessStatusCode Then
                            ResponseText = Await response.Content.ReadAsStringAsync().ConfigureAwait(False)

                            File.WriteAllText(CachePath, ResponseText)

                            ' Capture new ETag if present
                            If response.Headers.ETag IsNot Nothing Then
                                NewETag = response.Headers.ETag.Tag
                            Else
                                Dim values As IEnumerable(Of String) = Nothing
                                If response.Headers.TryGetValues("ETag", values) Then
                                    NewETag = values.FirstOrDefault()
                                End If
                            End If
                        Else
                            ' Non-success and not 304, leave responseText as ""
                        End If
                    End Using
                End Using
            End Using

        Catch ex As HttpRequestException
            DebugException(ex)
        Catch ex As Exception
            DebugException(ex)
        End Try

        If NotModified Then
            Try
                ResponseText = File.ReadAllText(CachePath)
            Catch ex As Exception
                DebugException(ex)
            End Try
        End If

        Return (ResponseText, NewETag)
    End Function
End Module
