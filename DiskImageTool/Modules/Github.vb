Module Github
    Private Const REPOSITORY = "https://api.github.com/repos/Digitoxin1/DiskImageTool"
    Private Const CHANGELOG_URL = REPOSITORY & "/releases?per_page=100"
    Private Const UPDATE_URL = REPOSITORY & "/releases/latest"
    Private Const USER_AGENT = "DiskImageTool"

    Public Function GetAppUpdateResponse() As String
        Dim Response As Net.HttpWebResponse
        Dim ResponseText As String = ""
        Dim TempPath = InitTempPath()
        Dim CachePath = IO.Path.Combine(TempPath, "AppUpdateResponse.cache")
        Dim ETag = App.Globals.AppSettings.ETags.AppUpdate
        Dim NotModified As Boolean = False

        If Not IO.File.Exists(CachePath) Then
            ETag = ""
        End If

        Dim Request As Net.HttpWebRequest = Net.WebRequest.Create(UPDATE_URL)
        Request.UserAgent = USER_AGENT
        If ETag <> "" Then
            Request.Headers.Add("if-none-match", ETag)
        End If

        Try
            Response = Request.GetResponse
            Dim Reader As New IO.StreamReader(Response.GetResponseStream)
            ResponseText = Reader.ReadToEnd

            IO.File.WriteAllText(CachePath, ResponseText)
            App.Globals.AppSettings.ETags.AppUpdate = Response.Headers.Item("etag")

        Catch ex As Net.WebException
            If ex.Response IsNot Nothing Then
                Response = CType(ex.Response, Net.HttpWebResponse)
                If Response.StatusCode = Net.HttpStatusCode.NotModified Then
                    NotModified = True
                End If
            End If
        Catch ex As Exception
            DebugException(ex)
        End Try

        If NotModified Then
            Try
                ResponseText = IO.File.ReadAllText(CachePath)
            Catch ex As Exception
                DebugException(ex)
            End Try
        End If

        Return ResponseText
    End Function

    Public Function GetChangeLogResponse() As String
        Dim Response As Net.HttpWebResponse
        Dim ResponseText As String = ""
        Dim TempPath = InitTempPath()
        Dim CachePath = IO.Path.Combine(TempPath, "ChangeLogResponse.cache")
        Dim ETag = App.Globals.AppSettings.ETags.ChangeLog
        Dim NotModified As Boolean = False

        If Not IO.File.Exists(CachePath) Then
            ETag = ""
        End If

        Dim Request As Net.HttpWebRequest = Net.WebRequest.Create(CHANGELOG_URL)
        Request.UserAgent = USER_AGENT
        If ETag <> "" Then
            Request.Headers.Add("if-none-match", ETag)
        End If

        Try
            Response = Request.GetResponse
            Dim Reader As New IO.StreamReader(Response.GetResponseStream)
            ResponseText = Reader.ReadToEnd

            IO.File.WriteAllText(CachePath, ResponseText)
            App.Globals.AppSettings.ETags.ChangeLog = Response.Headers.Item("etag")

        Catch ex As Net.WebException
            If ex.Response IsNot Nothing Then
                Response = CType(ex.Response, Net.HttpWebResponse)
                If Response.StatusCode = Net.HttpStatusCode.NotModified Then
                    NotModified = True
                End If
            End If
        Catch ex As Exception
            DebugException(ex)
        End Try

        If NotModified Then
            Try
                ResponseText = IO.File.ReadAllText(CachePath)
            Catch ex As Exception
                DebugException(ex)
            End Try
        End If

        Return ResponseText
    End Function
End Module
