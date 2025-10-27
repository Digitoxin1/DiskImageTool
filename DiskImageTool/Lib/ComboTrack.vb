Public Class ComboTrack
    Inherits ToolStripComboBox

    Public Sub New()
        Me.Alignment = ToolStripItemAlignment.Right
        Me.DropDownStyle = ComboBoxStyle.DropDown
        Me.AutoSize = False
        Me.FlatStyle = FlatStyle.Standard
        Me.Size = New Drawing.Size(50, 23)
        Me.MaxLength = 4
        Me.AutoCompleteMode = AutoCompleteMode.None

        AddHandler Me.KeyDown, AddressOf KeyDownEvent
        AddHandler Me.KeyPress, AddressOf KeyPressEvent
    End Sub

    Private Sub KeyDownEvent(sender As Object, e As KeyEventArgs)
        Dim cb = DirectCast(sender, ToolStripComboBox)

        If e.KeyCode = Keys.Enter Then
            e.Handled = True
            e.SuppressKeyPress = True

            Dim text = cb.Text

            If String.IsNullOrEmpty(text) Then
                Return
            End If

            Dim Value As Decimal

            If Decimal.TryParse(text, Globalization.NumberStyles.AllowDecimalPoint, Globalization.CultureInfo.InvariantCulture, Value) Then
                text = Value.ToString("0.#", Globalization.CultureInfo.InvariantCulture)
            End If

            For i = 0 To cb.Items.Count - 1
                Dim itemText = cb.ComboBox.GetItemText(cb.Items(i))
                If itemText.StartsWith(text, StringComparison.OrdinalIgnoreCase) Then
                    cb.SelectedIndex = i
                    cb.SelectAll()
                    Return
                End If
            Next
        End If
    End Sub

    Private Sub KeyPressEvent(sender As Object, e As KeyPressEventArgs)
        Dim cb = DirectCast(sender, ToolStripComboBox)

        If Char.IsControl(e.KeyChar) Then
            e.Handled = False
            Return
        End If

        Dim text = cb.Text
        Dim selStart = cb.SelectionStart
        Dim selLength = cb.SelectionLength

        If selLength > 0 AndAlso selStart + selLength <= text.Length Then
            text = text.Remove(selStart, selLength)
        End If
        text = text.Insert(selStart, e.KeyChar.ToString())

        If System.Text.RegularExpressions.Regex.IsMatch(text, "^(?:\d{1,2}|\d{1,2}\.?|(?:\d{1,2}\.[01]))$") Then
            e.Handled = False
        Else
            e.Handled = True
            System.Media.SystemSounds.Beep.Play()
        End If
    End Sub
End Class
