Public Class UndeleteForm
    Public Sub New(Filename As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()

        Filename = "?" & Filename.Substring(1)
        Me.LabelFileName.Text = Filename
    End Sub

    Private Sub LocalizeForm()
        BtnCancel.Text = My.Resources.Menu_Cancel
        BtnUpdate.Text = My.Resources.Menu_Update
    End Sub

    Public ReadOnly Property FirstChar As Byte
        Get
            If TextBoxChar.Text = "" Then
                Return 0
            Else
                Return Asc(TextBoxChar.Text)
            End If
        End Get
    End Property

    Private Sub TextBoxChar_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBoxChar.KeyPress
        Dim Value = Asc(e.KeyChar)

        If Value > 96 And Value < 123 Then
            Value -= 32
            e.KeyChar = Chr(Value)
        ElseIf Value > 255 Then
            e.KeyChar = Chr(0)
        ElseIf Value < 33 And Value <> 8 Then
            e.KeyChar = Chr(0)
        ElseIf DiskImage.DirectoryEntry.InvalidFileChars.Contains(Value) Then
            e.KeyChar = Chr(0)
        End If
    End Sub
End Class