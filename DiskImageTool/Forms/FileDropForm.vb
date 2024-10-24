Public Class FileDropForm

    Private _FileNames() As String

    Public ReadOnly Property FileNames As String()
        Get
            Return _FileNames
        End Get
    End Property

    Private Sub FileDropForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End If
    End Sub

    Private Sub FileDropForm_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter, LabelDropMessage.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub LabelDropMessage_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop, LabelDropMessage.DragDrop
        _FileNames = e.Data.GetData(DataFormats.FileDrop)
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub
End Class