Namespace Flux.Greaseweazle
    Public Class ConfigurationForm
        Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
            SettingsPanelGreaseweazle.UpdateSettings()
            SettingsPanelKryoflux.UpdateSettings()
        End Sub

        Private Sub ConfigurationForm_Load(sender As Object, e As EventArgs) Handles Me.Load
            SettingsPanelGreaseweazle.Initialize()
            SettingsPanelKryoflux.Initialize()
        End Sub
    End Class
End Namespace
