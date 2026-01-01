Imports System.Runtime.CompilerServices

Module StringExtensions
    <Extension()>
    Public Function WordWrap(Text As String, MaxWidth As Integer, TargetWidth As Integer, Font As Font) As List(Of String)
        If String.IsNullOrEmpty(Text) OrElse MaxWidth <= 0 Then
            Return New List(Of String) From {Text}
        End If

        ' Normalize newlines and split into logical lines
        Dim Lines = Text.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).Split(vbLf)

        Dim DoReduceLines As Boolean = (Lines.Count = 1)

        Dim Result As List(Of String)
        Dim Words() As String = {}
        Dim TryReduceLines As Boolean

        Do
            Result = New List(Of String)
            For Each Line In Lines
                ' Preserve blank lines
                If Line = "" Then
                    Result.Add("")
                    Continue For
                End If

                Words = Line.Split(" ")

                Result.AddRange(WrapLine(Words, TargetWidth, Font))
            Next

            TryReduceLines = DoReduceLines AndAlso Result.Count > 2 AndAlso TargetWidth < MaxWidth AndAlso Words.Length > 0

            If TryReduceLines Then
                TargetWidth += 4
                If TargetWidth > MaxWidth Then
                    TargetWidth = MaxWidth
                End If
            End If
        Loop Until Not TryReduceLines

        Return Result
    End Function

    Private Function WrapLine(Words() As String, MaxWidth As Integer, Font As Font) As List(Of String)
        Dim Lines As New List(Of String)()
        Dim CurrentLine As String = ""

        For Each Word In Words
            Dim NewLine = If(CurrentLine = "", Word, CurrentLine & " " & Word)
            Dim Size = TextRenderer.MeasureText(NewLine, Font)

            If Size.Width <= MaxWidth Then
                CurrentLine = NewLine
            Else
                If CurrentLine <> "" Then
                    Lines.Add(CurrentLine)
                End If
                CurrentLine = Word
            End If
        Next

        If CurrentLine <> "" Then
            Lines.Add(CurrentLine)
        End If

        Return Lines
    End Function
End Module
