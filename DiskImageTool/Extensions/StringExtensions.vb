Imports System.Runtime.CompilerServices

Module StringExtensions
    <Extension()>
    Public Function WordWrap(Text As String, Width As Integer, Font As Font) As List(Of String)
        Dim StringList = New List(Of String)
        Dim Size = TextRenderer.MeasureText(Text, Font)

        If String.IsNullOrEmpty(Text) OrElse Width = 0 OrElse Width >= Size.Width Then
            StringList.Add(Text)
            Return StringList
        End If

        Dim Words = Text.Split(" ")
        Dim Line As String = ""
        For Each Word In Words
            Dim NewLine = Line
            If NewLine <> "" Then
                NewLine &= " "
            End If
            NewLine &= Word
            Size = TextRenderer.MeasureText(NewLine, Font)

            If Size.Width <= Width Then
                Line = NewLine
            Else
                StringList.Add(Line)
                Line = Word
            End If
        Next
        If Line <> "" Then
            StringList.Add(Line)
        End If

        Return StringList
    End Function
End Module
