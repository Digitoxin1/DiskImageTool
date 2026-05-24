Imports System.Text.RegularExpressions

Namespace FloppyDB
    Public Class FileNameData
        Private ReadOnly _Db As FloppyDB

        Public Sub New(FileName As String, Db As FloppyDB)
            Me._Db = Db
            Me.FileName = FileName
            ParseFileName()
        End Sub

        Public Property CopyProtected As Boolean = False
        Public Property Cracked As Boolean = False
        Public Property Disk As String = ""
        Public Property FileName As String = ""
        Public Property Languages As String = ""
        Public Property Modified As Boolean = False
        Public Property Region As String = ""
        Public Property Status As FloppyDBStatus = FloppyDBStatus.Unknown
        Public Property StatusString As String = ""
        Public Property Title As String = ""
        Public Property Verified As Boolean = False
        Public Property Version As String = ""
        Public Property Year As String = ""

        Private Sub ParseFileName()
            Me.Title = Trim(Regex.Match(FileName, "^[^\\(]+").Value)
            Me.Title = Replace(Me.Title, " - ", ": ")

            Dim Groups = Regex.Match(FileName, "\((\d{4}-*\d*-*\d*)\)").Groups
            If Groups.Count > 1 Then
                Me.Year = Groups.Item(1).Value
            End If

            Groups = Regex.Match(FileName, "\(v(.*?)\)").Groups
            If Groups.Count > 1 Then
                Me.Version = Groups.Item(1).Value
            End If

            Groups = Regex.Match(FileName, "\(.*Disk (.*?)\)|\((\w*? Disk)\)").Groups
            If Groups.Count > 1 Then
                Me.Disk = Groups.Item(1).Value
            End If

            Dim Captures = Regex.Match(FileName, "\[!\]").Captures
            If Captures.Count > 0 Then
                Me.Verified = True
            End If

            Captures = Regex.Match(FileName, "\[M.*\]").Captures
            If Captures.Count > 0 Then
                Me.Modified = True
            End If

            Captures = Regex.Match(FileName, "\[cp\]").Captures
            If Captures.Count > 0 Then
                Me.CopyProtected = True
            End If

            Captures = Regex.Match(FileName, "\[cr\]").Captures
            If Captures.Count > 0 Then
                Me.Cracked = True
            End If

            If _Db IsNot Nothing AndAlso _Db.Regions IsNot Nothing Then
                For Each kvp In _Db.Regions
                    Dim m = Regex.Match(FileName, "\(" & Regex.Escape(kvp.Value) & "\)")
                    If m.Success Then
                        Me.Region = kvp.Key
                        Exit For
                    End If
                Next
            End If

            Groups = Regex.Match(FileName, "\((\s*[A-Za-z]{2}(?:\s*,\s*[A-Za-z]{2})*\s*)\)").Groups
            If Groups.Count > 1 Then
                Me.Languages = ParseLanguageList(Groups.Item(1).Value)
            End If

            If Verified Then
                Me.StatusString = "V"
            ElseIf Modified Then
                Me.StatusString = "M"
            Else
                Me.StatusString = "U"
            End If
            Me.Status = GetFloppyDBStatus(Me.StatusString)
        End Sub

        Private Function ParseLanguageList(codesList As String) As String
            If String.IsNullOrWhiteSpace(codesList) Then
                Return ""
            End If
            If _Db Is Nothing OrElse _Db.Languages Is Nothing Then
                Return ""
            End If

            Dim result As New List(Of String)

            For Each p In codesList.Split(","c)
                Dim code = p.Trim()
                If code = "" Then Continue For

                For Each kvp In _Db.Languages
                    If String.Equals(kvp.Key, code, StringComparison.OrdinalIgnoreCase) Then
                        result.Add(kvp.Key)
                        Exit For
                    End If
                Next
            Next

            Return String.Join(",", result)
        End Function
    End Class
End Namespace