Public Class UndeleteForm
    Private Const VALID_CHARS As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"
    Public Sub New(Filename As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Filename = "?" & Filename.Substring(1)
        Me.LabelFileName.Text = Filename
    End Sub

    Public ReadOnly Property FirstChar As Byte
        Get
            If TextBox1.Text = "" Then
                Return 0
            Else
                Return Asc(TextBox1.Text)
            End If
        End Get
    End Property

    Private Sub TextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress
        Dim Value = Asc(e.KeyChar)

        If Value > 96 And Value < 123 Then
            Value -= 32
            e.KeyChar = Chr(Value)
        ElseIf Value > 255 Then
            e.KeyChar = Chr(0)
        ElseIf Value < 33 And Value <> 8 Then
            e.KeyChar = Chr(0)
        ElseIf DiskImage.InvalidFileChars.Contains(Value) Then
            e.KeyChar = Chr(0)
        End If
    End Sub
End Class