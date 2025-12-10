Namespace Flux
    Public Class ConfigurationForm
        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            LocalizeForm()

        End Sub

        Public Shared Sub Display(owner As IWin32Window)
            Using Form As New Flux.ConfigurationForm
                Form.ShowDialog(owner)
            End Using
        End Sub

        Private Sub LocalizeForm()
            Me.Text = My.Resources.Caption_FluxConfiguration
            BtnCancel.Text = My.Resources.Menu_Cancel
            BtnUpdate.Text = My.Resources.Menu_Update
        End Sub

        Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
            If SettingsPanelGeneral.Initialized Then
                SettingsPanelGeneral.UpdateSettings()
            End If

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
            SettingsPanelGeneral.Initialize()
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
