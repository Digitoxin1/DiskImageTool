Imports System.Runtime.CompilerServices

Module StringExtensions
    <Extension()>
    Public Function WordWrap(Text As String, Width As Integer, Font As Font) As List(Of String)
        Dim Result As New List(Of String)

        If String.IsNullOrEmpty(Text) OrElse Width <= 0 Then
            Result.Add(Text)
            Return Result
        End If

        ' Normalize newlines and split into logical lines
        Dim Lines = Text.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).Split(vbLf)

        For Each LogicalLine In Lines
            ' Preserve blank lines
            If LogicalLine = "" Then
                Result.Add("")
                Continue For
            End If

            Dim Words = LogicalLine.Split(" ")
            Dim Line As String = ""

            For Each Word In Words
                Dim NewLine = If(Line = "", Word, Line & " " & Word)
                Dim Size = TextRenderer.MeasureText(NewLine, Font)

                If Size.Width <= Width Then
                    Line = NewLine
                Else
                    If Line <> "" Then
                        Result.Add(Line)
                    End If
                    Line = Word
                End If
            Next

            If Line <> "" Then
                Result.Add(Line)
            End If
        Next

        Return Result
    End Function
End Module
