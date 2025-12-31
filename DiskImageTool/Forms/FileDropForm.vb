Public Class FileDropForm

    Private _FileNames() As String

    Public ReadOnly Property FileNames As String()
        Get
            Return _FileNames
        End Get
    End Property

    Public Shared Function Display() As (Result As Boolean, FileNames As String())
        Using dlg As New FileDropForm()
            Dim Response = dlg.ShowDialog(App.CurrentFormInstance)

            Return (Response = DialogResult.OK, dlg.FileNames)
        End Using
    End Function

    Private Sub FileDropForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End If
    End Sub

    Private Sub FileDropForm_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub LabelDropMessage_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        _FileNames = e.Data.GetData(DataFormats.FileDrop)
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub FileDropForm_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Dim text As String = My.Resources.Label_FileDrop

        Dim textSize As Size = TextRenderer.MeasureText(text, Me.Font, New Size(Integer.MaxValue, Integer.MaxValue), TextFormatFlags.SingleLine Or TextFormatFlags.NoPrefix)

        Dim x As Single = (Me.ClientSize.Width - textSize.Width) / 2
        Dim y As Single = (Me.ClientSize.Height - textSize.Height) / 2

        TextRenderer.DrawText(e.Graphics, text, Me.Font, New Point(x, y), Color.Black, TextFormatFlags.SingleLine Or TextFormatFlags.NoPrefix)
    End Sub
End Class