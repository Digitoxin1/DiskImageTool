Public Class ThemeForm
    Inherits Form

    Public Sub New()
        AddHandler ThemeManager.ThemeChanged, AddressOf ThemeChanged
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        ApplyTheme()
    End Sub

    Private Sub ThemeChanged(sender As Object, e As EventArgs)
        If Not Me.IsDisposed AndAlso Not Me.Disposing Then
            ApplyTheme()
        End If
    End Sub

    Protected Overridable Sub ApplyTheme()
        Dim theme = ThemeManager.CurrentTheme

        Me.BackColor = theme.FormBackColor
        Me.ForeColor = theme.FormForeColor
    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        RemoveHandler ThemeManager.ThemeChanged, AddressOf ThemeChanged
        MyBase.OnFormClosed(e)
    End Sub
End Class
