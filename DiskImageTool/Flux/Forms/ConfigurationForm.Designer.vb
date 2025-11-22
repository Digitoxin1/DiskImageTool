Namespace Flux.Greaseweazle
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class ConfigurationForm
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ConfigurationForm))
            Dim PanelMain As System.Windows.Forms.Panel
            Dim TabControl1 As System.Windows.Forms.TabControl
            Dim TabPageGreaseweazle As System.Windows.Forms.TabPage
            Dim TabPageKryoflux As System.Windows.Forms.TabPage
            Me.BtnCancel = New System.Windows.Forms.Button()
            Me.BtnUpdate = New System.Windows.Forms.Button()
            Me.SettingsPanelGreaseweazle = New DiskImageTool.Flux.Greaseweazle.SettingsPanel()
            Me.SettingsPanelKryoflux = New DiskImageTool.Flux.Kryoflux.SettingsPanel()
            Me.TwoColumnToolTip1 = New DiskImageTool.TwoColumnToolTip()
            PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
            PanelMain = New System.Windows.Forms.Panel()
            TabControl1 = New System.Windows.Forms.TabControl()
            TabPageGreaseweazle = New System.Windows.Forms.TabPage()
            TabPageKryoflux = New System.Windows.Forms.TabPage()
            PanelBottom.SuspendLayout()
            PanelMain.SuspendLayout()
            TabControl1.SuspendLayout()
            TabPageGreaseweazle.SuspendLayout()
            TabPageKryoflux.SuspendLayout()
            Me.SuspendLayout()
            '
            'PanelBottom
            '
            PanelBottom.Controls.Add(Me.BtnCancel)
            PanelBottom.Controls.Add(Me.BtnUpdate)
            resources.ApplyResources(PanelBottom, "PanelBottom")
            PanelBottom.Name = "PanelBottom"
            '
            'BtnCancel
            '
            Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            resources.ApplyResources(Me.BtnCancel, "BtnCancel")
            Me.BtnCancel.Name = "BtnCancel"
            Me.BtnCancel.UseVisualStyleBackColor = True
            '
            'BtnUpdate
            '
            Me.BtnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK
            resources.ApplyResources(Me.BtnUpdate, "BtnUpdate")
            Me.BtnUpdate.Name = "BtnUpdate"
            Me.BtnUpdate.UseVisualStyleBackColor = True
            '
            'PanelMain
            '
            resources.ApplyResources(PanelMain, "PanelMain")
            PanelMain.Controls.Add(TabControl1)
            PanelMain.Name = "PanelMain"
            '
            'TabControl1
            '
            TabControl1.Controls.Add(TabPageGreaseweazle)
            TabControl1.Controls.Add(TabPageKryoflux)
            resources.ApplyResources(TabControl1, "TabControl1")
            TabControl1.Name = "TabControl1"
            TabControl1.SelectedIndex = 0
            '
            'TabPageGreaseweazle
            '
            TabPageGreaseweazle.Controls.Add(Me.SettingsPanelGreaseweazle)
            resources.ApplyResources(TabPageGreaseweazle, "TabPageGreaseweazle")
            TabPageGreaseweazle.Name = "TabPageGreaseweazle"
            TabPageGreaseweazle.UseVisualStyleBackColor = True
            '
            'SettingsPanelGreaseweazle
            '
            resources.ApplyResources(Me.SettingsPanelGreaseweazle, "SettingsPanelGreaseweazle")
            Me.SettingsPanelGreaseweazle.Name = "SettingsPanelGreaseweazle"
            '
            'TabPageKryoflux
            '
            TabPageKryoflux.Controls.Add(Me.SettingsPanelKryoflux)
            resources.ApplyResources(TabPageKryoflux, "TabPageKryoflux")
            TabPageKryoflux.Name = "TabPageKryoflux"
            TabPageKryoflux.UseVisualStyleBackColor = True
            '
            'SettingsPanelKryoflux
            '
            resources.ApplyResources(Me.SettingsPanelKryoflux, "SettingsPanelKryoflux")
            Me.SettingsPanelKryoflux.Name = "SettingsPanelKryoflux"
            '
            'TwoColumnToolTip1
            '
            Me.TwoColumnToolTip1.OwnerDraw = True
            '
            'ConfigurationForm
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(PanelMain)
            Me.Controls.Add(PanelBottom)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ConfigurationForm"
            Me.ShowInTaskbar = False
            PanelBottom.ResumeLayout(False)
            PanelMain.ResumeLayout(False)
            TabControl1.ResumeLayout(False)
            TabPageGreaseweazle.ResumeLayout(False)
            TabPageKryoflux.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents BtnUpdate As Button
        Friend WithEvents BtnCancel As Button
        Friend WithEvents TwoColumnToolTip1 As TwoColumnToolTip
        Friend WithEvents SettingsPanelGreaseweazle As SettingsPanel
        Friend WithEvents SettingsPanelKryoflux As Kryoflux.SettingsPanel
    End Class
End Namespace