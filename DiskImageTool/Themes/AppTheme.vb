Public NotInheritable Class AppTheme
    Public ReadOnly Property FormBackColor As Color
    Public ReadOnly Property FormForeColor As Color

    Public Sub New(
            FormBackColor As Color,
            FormForeColor As Color
        )

        Me.FormBackColor = FormBackColor
        Me.FormForeColor = FormForeColor
    End Sub
End Class
