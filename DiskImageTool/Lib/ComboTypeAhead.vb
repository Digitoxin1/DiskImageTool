Public Class ComboTypeAhead
    Private ReadOnly _cb As ComboBox
    Private ReadOnly _timer As New Timer() With {.Interval = 750}
    Private _buffer As String = ""
    Private _maxLength As Integer = 2

    Public Sub New(cb As ComboBox)
        _cb = cb
        _cb.AutoCompleteMode = AutoCompleteMode.None
        AddHandler _cb.KeyPress, AddressOf OnKeyPress
        AddHandler _timer.Tick, Sub()
                                    _buffer = ""
                                    _maxLength = 2
                                    _timer.Stop()
                                End Sub
    End Sub

    Private Sub OnKeyPress(sender As Object, e As KeyPressEventArgs)
        If Char.IsDigit(e.KeyChar) Then
            If _buffer.Length >= _maxLength Then
                _buffer = ""
                _maxLength = 2
            End If
            _buffer &= e.KeyChar
            e.Handled = True

        ElseIf e.KeyChar = "."c Then
            If _buffer.Length = 0 Then
                e.Handled = True
                Return
            End If
            _buffer &= e.KeyChar
            _maxLength = _buffer.Length + 1
            e.Handled = True

        Else
            e.Handled = True
            Return
        End If

        _timer.Stop()
        _timer.Start()

        Dim idx = FindIndex(_buffer)
        If idx >= 0 Then
            _cb.SelectedIndex = idx
        End If
    End Sub

    Private Function FindIndex(prefix As String) As Integer
        For i = 0 To _cb.Items.Count - 1
            Dim s = _cb.GetItemText(_cb.Items(i))
            If s.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then
                Return i
            End If
        Next

        Return -1
    End Function
End Class
