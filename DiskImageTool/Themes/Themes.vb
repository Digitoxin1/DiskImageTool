Module Themes

    Public ReadOnly Light As New AppTheme(
        FormBackColor:=SystemColors.Control,
        FormForeColor:=SystemColors.ControlText
    )

    Public ReadOnly Dark As New AppTheme(
        FormBackColor:=Color.FromArgb(32, 32, 32),
        FormForeColor:=Color.FromArgb(240, 240, 240)
    )
End Module
