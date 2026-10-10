Public Class FileDialogFilterBuilder
    Private ReadOnly _Filters As New List(Of String)

    Public Sub Add(Description As String, ParamArray Extensions As String())
        Dim Pattern = String.Join(";", Extensions.Select(
            Function(e) If(e = "*", "*.*", "*." & e.TrimStart("."c))))

        _Filters.Add($"{Description} ({Pattern})|{Pattern}")
    End Sub

    Public Overrides Function ToString() As String
        Return String.Join("|", _Filters)
    End Function
End Class