Imports System.Text.RegularExpressions

Module FilenameTemplateHelper
    Private ReadOnly PlaceholderPattern As New Regex("\<(\d+)\>", RegexOptions.Compiled)

    Public Function ContainsPlaceholder(template As String) As Boolean
        If String.IsNullOrEmpty(template) Then
            Return False
        End If

        Return PlaceholderPattern.IsMatch(template)
    End Function

    Public Function IncrementPlaceholders(template As String) As String
        If String.IsNullOrEmpty(template) Then
            Return template
        End If

        Return PlaceholderPattern.Replace(template, AddressOf IncrementMatchEvaluator)
    End Function

    Public Function SanitizeFileNamePreservePlaceholders(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then
            Return ""
        End If


        Dim placeholders As New List(Of String)
        Dim tokenized As String = placeholderPattern.Replace(input,
        Function(m)
            placeholders.Add(m.Value)
            Return $"__PLACEHOLDER_{placeholders.Count - 1}__"
        End Function)

        tokenized = SanitizeFileName(tokenized)

        For i As Integer = 0 To placeholders.Count - 1
            tokenized = tokenized.Replace($"__PLACEHOLDER_{i}__", placeholders(i))
        Next

        Return tokenized
    End Function

    Public Function StripAngleBrackets(template As String) As String
        If String.IsNullOrEmpty(template) Then
            Return template
        End If

        Return PlaceholderPattern.Replace(template, AddressOf StripMatchEvaluator)
    End Function
    Private Function IncrementMatchEvaluator(m As Match) As String
        Dim digits As String = m.Groups(1).Value

        Dim n As Integer
        If Not Integer.TryParse(digits, n) Then
            Return m.Value
        End If

        n += 1

        Dim width As Integer = digits.Length
        Return "<" & n.ToString(New String("0"c, width)) & ">"
    End Function

    Private Function StripMatchEvaluator(m As Match) As String
        Return m.Groups(1).Value
    End Function
End Module
