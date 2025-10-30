Imports System.Text

Module MenuHelp
    Public Sub AboutBoxDisplay()
        Dim AboutBox As New AboutBox()
        AboutBox.ShowDialog()
    End Sub

    Public Sub ChangeLogDisplay()
        Dim VersionLine As String
        Dim PublishedAt As String
        Dim Body As String
        Dim BodyArray() As String
        Dim Changelog = New StringBuilder()
        Dim ChangeLogString As String
        Dim ErrMsg As String = My.Resources.Dialog_ChangeLogDownloadError

        Cursor.Current = Cursors.WaitCursor
        Dim ResponseText = GetChangeLogResponse()
        Cursor.Current = Cursors.Default

        If ResponseText = "" Then
            MsgBox(ErrMsg, MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        Try
            Dim JSON As List(Of Dictionary(Of String, Object)) = CompactJson.Serializer.Parse(Of List(Of Dictionary(Of String, Object)))(ResponseText)

            For Each Release In JSON
                If Release.ContainsKey("tag_name") Then
                    VersionLine = Release.Item("tag_name").ToString
                    If Release.ContainsKey("published_at") Then
                        PublishedAt = Release.Item("published_at").ToString
                        Dim PublishDate As Date
                        If Date.TryParse(PublishedAt, PublishDate) Then
                            VersionLine &= " " & InParens(PublishDate.ToString)
                        End If
                    End If
                    If Release.ContainsKey("body") Then
                        Body = Release.Item("body").ToString
                        If Body <> "" Then
                            Body = Replace(Body, Chr(13) & Chr(10), Chr(10))
                            BodyArray = Body.Split(Chr(10))
                            Changelog.AppendLine(VersionLine)
                            For Counter = 0 To BodyArray.Length - 1
                                Dim ChangeLine = BodyArray(Counter).Trim
                                If ChangeLine.Length > 0 Then
                                    If ChangeLine.Substring(0, 1) <> "-" Then
                                        ChangeLine = "- " & ChangeLine
                                    End If
                                    Changelog.AppendLine(ChangeLine)
                                End If
                            Next
                            Changelog.AppendLine("")
                        End If
                    End If
                End If
            Next

            ChangeLogString = Changelog.ToString

        Catch ex As Exception
            MsgBox(ErrMsg, MsgBoxStyle.Exclamation)
            DebugException(ex)
            Exit Sub
        End Try

        Dim frmTextView = New TextViewForm(My.Resources.Caption_ChangeLog, ChangeLogString, False, True, "ChangeLog.txt")
        frmTextView.ShowDialog()
    End Sub

    Public Sub ProjectPageDisplay()
        Process.Start(My.Resources.URL_Repository)
    End Sub
End Module
