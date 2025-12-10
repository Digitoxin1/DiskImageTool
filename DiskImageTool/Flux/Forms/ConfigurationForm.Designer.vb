Namespace Flux
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
            Dim PanelMain As System.Windows.Forms.Panel
            Dim TabPageGeneral As System.Windows.Forms.TabPage
            Dim TabPageGreaseweazle As System.Windows.Forms.TabPage
            Dim TabPageKryoflux As System.Windows.Forms.TabPage
            Me.BtnCancel = New System.Windows.Forms.Button()
            Me.BtnUpdate = New System.Windows.Forms.Button()
            Me.TabControl1 = New System.Windows.Forms.TabControl()
            Me.SettingsPanelGeneral = New DiskImageTool.Flux.GeneralSettingsPanel()
            Me.SettingsPanelGreaseweazle = New DiskImageTool.Flux.Greaseweazle.SettingsPanel()
            Me.SettingsPanelKryoflux = New DiskImageTool.Flux.Kryoflux.SettingsPanel()
            Me.TabPagePcImgCnv = New System.Windows.Forms.TabPage()
            Me.SettingsPanelPcImgCnv = New DiskImageTool.Flux.PcImgCnv.SettingsPanel()
            PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
            PanelMain = New System.Windows.Forms.Panel()
            TabPageGeneral = New System.Windows.Forms.TabPage()
            TabPageGreaseweazle = New System.Windows.Forms.TabPage()
            TabPageKryoflux = New System.Windows.Forms.TabPage()
            PanelBottom.SuspendLayout()
            PanelMain.SuspendLayout()
            Me.TabControl1.SuspendLayout()
            TabPageGeneral.SuspendLayout()
            TabPageGreaseweazle.SuspendLayout()
            TabPageKryoflux.SuspendLayout()
            Me.TabPagePcImgCnv.SuspendLayout()
            Me.SuspendLayout()
            '
            'PanelBottom
            '
            PanelBottom.Controls.Add(Me.BtnCancel)
            PanelBottom.Controls.Add(Me.BtnUpdate)
            PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
            PanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
            PanelBottom.Location = New System.Drawing.Point(0, 263)
            PanelBottom.Name = "PanelBottom"
            PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
            PanelBottom.Size = New System.Drawing.Size(724, 43)
            PanelBottom.TabIndex = 1
            PanelBottom.WrapContents = False
            '
            'BtnCancel
            '
            Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.BtnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.BtnCancel.Location = New System.Drawing.Point(631, 10)
            Me.BtnCancel.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
            Me.BtnCancel.Name = "BtnCancel"
            Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
            Me.BtnCancel.TabIndex = 1
            Me.BtnCancel.Text = "{&Cancel}"
            Me.BtnCancel.UseVisualStyleBackColor = True
            '
            'BtnUpdate
            '
            Me.BtnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.BtnUpdate.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.BtnUpdate.Location = New System.Drawing.Point(544, 10)
            Me.BtnUpdate.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
            Me.BtnUpdate.Name = "BtnUpdate"
            Me.BtnUpdate.Size = New System.Drawing.Size(75, 23)
            Me.BtnUpdate.TabIndex = 0
            Me.BtnUpdate.Text = "{&Update}"
            Me.BtnUpdate.UseVisualStyleBackColor = True
            '
            'PanelMain
            '
            PanelMain.AutoSize = True
            PanelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            PanelMain.Controls.Add(Me.TabControl1)
            PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
            PanelMain.Location = New System.Drawing.Point(0, 0)
            PanelMain.Name = "PanelMain"
            PanelMain.Padding = New System.Windows.Forms.Padding(6, 6, 6, 0)
            PanelMain.Size = New System.Drawing.Size(724, 263)
            PanelMain.TabIndex = 0
            '
            'TabControl1
            '
            Me.TabControl1.Controls.Add(TabPageGeneral)
            Me.TabControl1.Controls.Add(TabPageGreaseweazle)
            Me.TabControl1.Controls.Add(TabPageKryoflux)
            Me.TabControl1.Controls.Add(Me.TabPagePcImgCnv)
            Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.TabControl1.Location = New System.Drawing.Point(6, 6)
            Me.TabControl1.Name = "TabControl1"
            Me.TabControl1.Padding = New System.Drawing.Point(12, 3)
            Me.TabControl1.SelectedIndex = 0
            Me.TabControl1.Size = New System.Drawing.Size(712, 257)
            Me.TabControl1.TabIndex = 1
            '
            'TabPageGeneral
            '
            TabPageGeneral.Controls.Add(Me.SettingsPanelGeneral)
            TabPageGeneral.Location = New System.Drawing.Point(4, 22)
            TabPageGeneral.Name = "TabPageGeneral"
            TabPageGeneral.Size = New System.Drawing.Size(704, 231)
            TabPageGeneral.TabIndex = 3
            TabPageGeneral.Text = "General"
            TabPageGeneral.UseVisualStyleBackColor = True
            '
            'SettingsPanelGeneral
            '
            Me.SettingsPanelGeneral.Dock = System.Windows.Forms.DockStyle.Fill
            Me.SettingsPanelGeneral.Location = New System.Drawing.Point(0, 0)
            Me.SettingsPanelGeneral.Name = "SettingsPanelGeneral"
            Me.SettingsPanelGeneral.Padding = New System.Windows.Forms.Padding(12)
            Me.SettingsPanelGeneral.Size = New System.Drawing.Size(704, 231)
            Me.SettingsPanelGeneral.TabIndex = 0
            '
            'TabPageGreaseweazle
            '
            TabPageGreaseweazle.Controls.Add(Me.SettingsPanelGreaseweazle)
            TabPageGreaseweazle.Location = New System.Drawing.Point(4, 22)
            TabPageGreaseweazle.Name = "TabPageGreaseweazle"
            TabPageGreaseweazle.Padding = New System.Windows.Forms.Padding(3)
            TabPageGreaseweazle.Size = New System.Drawing.Size(704, 231)
            TabPageGreaseweazle.TabIndex = 0
            TabPageGreaseweazle.Text = "Greaseweazle"
            TabPageGreaseweazle.UseVisualStyleBackColor = True
            '
            'SettingsPanelGreaseweazle
            '
            Me.SettingsPanelGreaseweazle.Dock = System.Windows.Forms.DockStyle.Fill
            Me.SettingsPanelGreaseweazle.Location = New System.Drawing.Point(3, 3)
            Me.SettingsPanelGreaseweazle.Name = "SettingsPanelGreaseweazle"
            Me.SettingsPanelGreaseweazle.Padding = New System.Windows.Forms.Padding(6)
            Me.SettingsPanelGreaseweazle.Size = New System.Drawing.Size(698, 225)
            Me.SettingsPanelGreaseweazle.TabIndex = 0
            '
            'TabPageKryoflux
            '
            TabPageKryoflux.Controls.Add(Me.SettingsPanelKryoflux)
            TabPageKryoflux.Location = New System.Drawing.Point(4, 22)
            TabPageKryoflux.Name = "TabPageKryoflux"
            TabPageKryoflux.Padding = New System.Windows.Forms.Padding(3)
            TabPageKryoflux.Size = New System.Drawing.Size(704, 231)
            TabPageKryoflux.TabIndex = 1
            TabPageKryoflux.Text = "Kryoflux"
            TabPageKryoflux.UseVisualStyleBackColor = True
            '
            'SettingsPanelKryoflux
            '
            Me.SettingsPanelKryoflux.Dock = System.Windows.Forms.DockStyle.Fill
            Me.SettingsPanelKryoflux.Location = New System.Drawing.Point(3, 3)
            Me.SettingsPanelKryoflux.Name = "SettingsPanelKryoflux"
            Me.SettingsPanelKryoflux.Padding = New System.Windows.Forms.Padding(6)
            Me.SettingsPanelKryoflux.Size = New System.Drawing.Size(698, 225)
            Me.SettingsPanelKryoflux.TabIndex = 0
            '
            'TabPagePcImgCnv
            '
            Me.TabPagePcImgCnv.Controls.Add(Me.SettingsPanelPcImgCnv)
            Me.TabPagePcImgCnv.Location = New System.Drawing.Point(4, 22)
            Me.TabPagePcImgCnv.Name = "TabPagePcImgCnv"
            Me.TabPagePcImgCnv.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPagePcImgCnv.Size = New System.Drawing.Size(704, 231)
            Me.TabPagePcImgCnv.TabIndex = 2
            Me.TabPagePcImgCnv.Text = "PcImgCnv"
            Me.TabPagePcImgCnv.UseVisualStyleBackColor = True
            '
            'SettingsPanelPcImgCnv
            '
            Me.SettingsPanelPcImgCnv.Dock = System.Windows.Forms.DockStyle.Fill
            Me.SettingsPanelPcImgCnv.Location = New System.Drawing.Point(3, 3)
            Me.SettingsPanelPcImgCnv.Name = "SettingsPanelPcImgCnv"
            Me.SettingsPanelPcImgCnv.Padding = New System.Windows.Forms.Padding(6)
            Me.SettingsPanelPcImgCnv.Size = New System.Drawing.Size(698, 225)
            Me.SettingsPanelPcImgCnv.TabIndex = 0
            '
            'ConfigurationForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(724, 306)
            Me.Controls.Add(PanelMain)
            Me.Controls.Add(PanelBottom)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ConfigurationForm"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            PanelBottom.ResumeLayout(False)
            PanelMain.ResumeLayout(False)
            Me.TabControl1.ResumeLayout(False)
            TabPageGeneral.ResumeLayout(False)
            TabPageGreaseweazle.ResumeLayout(False)
            TabPageKryoflux.ResumeLayout(False)
            Me.TabPagePcImgCnv.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents BtnUpdate As Button
        Friend WithEvents BtnCancel As Button
        Friend WithEvents SettingsPanelGreaseweazle As Greaseweazle.SettingsPanel
        Friend WithEvents SettingsPanelKryoflux As Kryoflux.SettingsPanel
        Friend WithEvents SettingsPanelPcImgCnv As PcImgCnv.SettingsPanel
        Friend WithEvents TabControl1 As TabControl
        Friend WithEvents TabPagePcImgCnv As TabPage
        Friend WithEvents SettingsPanelGeneral As DiskImageTool.Flux.GeneralSettingsPanel
    End Class
End Namespace