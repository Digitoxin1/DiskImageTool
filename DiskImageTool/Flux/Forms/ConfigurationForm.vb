Namespace Flux
    Public Class ConfigurationForm
        Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
            If SettingsPanelGreaseweazle.Initialized Then
                SettingsPanelGreaseweazle.UpdateSettings()
            End If
            If SettingsPanelKryoflux.Initialized Then
                SettingsPanelKryoflux.UpdateSettings()
            End If
            If SettingsPanelPcImgCnv.Initialized Then
                SettingsPanelPcImgCnv.UpdateSettings()
            End If
        End Sub

        Private Sub ConfigurationForm_Load(sender As Object, e As EventArgs) Handles Me.Load
            SettingsPanelGreaseweazle.Initialize()
            SettingsPanelKryoflux.Initialize()

            If Not App.AppSettings.PcImgCnv.Enabled Then
                TabControl1.TabPages.Remove(TabPagePcImgCnv)
            Else
                SettingsPanelPcImgCnv.Initialize()
            End If
        End Sub
    End Class
End Namespace
