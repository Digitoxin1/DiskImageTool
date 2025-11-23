Namespace Flux
    Public Class ConfigurationForm
        Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
            SettingsPanelGreaseweazle.UpdateSettings()
            SettingsPanelKryoflux.UpdateSettings()
            SettingsPanelPcImgCnv.UpdateSettings()
        End Sub

        Private Sub ConfigurationForm_Load(sender As Object, e As EventArgs) Handles Me.Load
            SettingsPanelGreaseweazle.Initialize()
            SettingsPanelKryoflux.Initialize()
            SettingsPanelPcImgCnv.Initialize()
        End Sub
    End Class
End Namespace
