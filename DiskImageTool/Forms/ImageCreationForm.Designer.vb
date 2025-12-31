<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ImageCreationForm
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
        Dim RadioFormat2880 As System.Windows.Forms.RadioButton
        Dim RadioFormat1440 As System.Windows.Forms.RadioButton
        Dim RadioFormat1200 As System.Windows.Forms.RadioButton
        Dim RadioFormat720 As System.Windows.Forms.RadioButton
        Dim RadioFormat360 As System.Windows.Forms.RadioButton
        Dim RadioFormat320 As System.Windows.Forms.RadioButton
        Dim RadioFormat180 As System.Windows.Forms.RadioButton
        Dim RadioFormat160 As System.Windows.Forms.RadioButton
        Dim RadioFormatTandy2000 As System.Windows.Forms.RadioButton
        Dim RadioFormatProCopy As System.Windows.Forms.RadioButton
        Dim RadioFormatDMF2048 As System.Windows.Forms.RadioButton
        Dim RadioFormatDMF1024 As System.Windows.Forms.RadioButton
        Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
        Dim PanelMain As System.Windows.Forms.Panel
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.PanelFormats = New System.Windows.Forms.Panel()
        Me.GroupBoxSpecial = New System.Windows.Forms.GroupBox()
        Me.GroupBoxStandard = New System.Windows.Forms.GroupBox()
        Me.LabelBootSector = New System.Windows.Forms.Label()
        Me.ComboBootSector = New System.Windows.Forms.ComboBox()
        RadioFormat2880 = New System.Windows.Forms.RadioButton()
        RadioFormat1440 = New System.Windows.Forms.RadioButton()
        RadioFormat1200 = New System.Windows.Forms.RadioButton()
        RadioFormat720 = New System.Windows.Forms.RadioButton()
        RadioFormat360 = New System.Windows.Forms.RadioButton()
        RadioFormat320 = New System.Windows.Forms.RadioButton()
        RadioFormat180 = New System.Windows.Forms.RadioButton()
        RadioFormat160 = New System.Windows.Forms.RadioButton()
        RadioFormatTandy2000 = New System.Windows.Forms.RadioButton()
        RadioFormatProCopy = New System.Windows.Forms.RadioButton()
        RadioFormatDMF2048 = New System.Windows.Forms.RadioButton()
        RadioFormatDMF1024 = New System.Windows.Forms.RadioButton()
        PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        PanelMain = New System.Windows.Forms.Panel()
        PanelBottom.SuspendLayout()
        PanelMain.SuspendLayout()
        Me.PanelFormats.SuspendLayout()
        Me.SuspendLayout()
        '
        'RadioFormat2880
        '
        RadioFormat2880.AutoSize = True
        RadioFormat2880.Location = New System.Drawing.Point(91, 88)
        RadioFormat2880.Name = "RadioFormat2880"
        RadioFormat2880.Size = New System.Drawing.Size(55, 17)
        RadioFormat2880.TabIndex = 9
        RadioFormat2880.Tag = "8"
        RadioFormat2880.Text = "2.88M"
        RadioFormat2880.UseVisualStyleBackColor = True
        '
        'RadioFormat1440
        '
        RadioFormat1440.AutoSize = True
        RadioFormat1440.Checked = True
        RadioFormat1440.Location = New System.Drawing.Point(91, 66)
        RadioFormat1440.Name = "RadioFormat1440"
        RadioFormat1440.Size = New System.Drawing.Size(55, 17)
        RadioFormat1440.TabIndex = 8
        RadioFormat1440.TabStop = True
        RadioFormat1440.Tag = "7"
        RadioFormat1440.Text = "1.44M"
        RadioFormat1440.UseVisualStyleBackColor = True
        '
        'RadioFormat1200
        '
        RadioFormat1200.AutoSize = True
        RadioFormat1200.Location = New System.Drawing.Point(91, 44)
        RadioFormat1200.Name = "RadioFormat1200"
        RadioFormat1200.Size = New System.Drawing.Size(49, 17)
        RadioFormat1200.TabIndex = 7
        RadioFormat1200.Tag = "6"
        RadioFormat1200.Text = "1.2M"
        RadioFormat1200.UseVisualStyleBackColor = True
        '
        'RadioFormat720
        '
        RadioFormat720.AutoSize = True
        RadioFormat720.Location = New System.Drawing.Point(91, 22)
        RadioFormat720.Name = "RadioFormat720"
        RadioFormat720.Size = New System.Drawing.Size(50, 17)
        RadioFormat720.TabIndex = 6
        RadioFormat720.Tag = "5"
        RadioFormat720.Text = "720K"
        RadioFormat720.UseVisualStyleBackColor = True
        '
        'RadioFormat360
        '
        RadioFormat360.AutoSize = True
        RadioFormat360.Location = New System.Drawing.Point(8, 88)
        RadioFormat360.Name = "RadioFormat360"
        RadioFormat360.Size = New System.Drawing.Size(50, 17)
        RadioFormat360.TabIndex = 5
        RadioFormat360.Tag = "4"
        RadioFormat360.Text = "360K"
        RadioFormat360.UseVisualStyleBackColor = True
        '
        'RadioFormat320
        '
        RadioFormat320.AutoSize = True
        RadioFormat320.Location = New System.Drawing.Point(8, 66)
        RadioFormat320.Name = "RadioFormat320"
        RadioFormat320.Size = New System.Drawing.Size(50, 17)
        RadioFormat320.TabIndex = 4
        RadioFormat320.Tag = "3"
        RadioFormat320.Text = "320K"
        RadioFormat320.UseVisualStyleBackColor = True
        '
        'RadioFormat180
        '
        RadioFormat180.AutoSize = True
        RadioFormat180.Location = New System.Drawing.Point(8, 44)
        RadioFormat180.Name = "RadioFormat180"
        RadioFormat180.Size = New System.Drawing.Size(50, 17)
        RadioFormat180.TabIndex = 3
        RadioFormat180.Tag = "2"
        RadioFormat180.Text = "180K"
        RadioFormat180.UseVisualStyleBackColor = True
        '
        'RadioFormat160
        '
        RadioFormat160.AutoSize = True
        RadioFormat160.Location = New System.Drawing.Point(8, 22)
        RadioFormat160.Name = "RadioFormat160"
        RadioFormat160.Size = New System.Drawing.Size(50, 17)
        RadioFormat160.TabIndex = 2
        RadioFormat160.Tag = "1"
        RadioFormat160.Text = "160K"
        RadioFormat160.UseVisualStyleBackColor = True
        '
        'RadioFormatTandy2000
        '
        RadioFormatTandy2000.AutoSize = True
        RadioFormatTandy2000.Location = New System.Drawing.Point(190, 88)
        RadioFormatTandy2000.Name = "RadioFormatTandy2000"
        RadioFormatTandy2000.Size = New System.Drawing.Size(82, 17)
        RadioFormatTandy2000.TabIndex = 13
        RadioFormatTandy2000.Tag = "15"
        RadioFormatTandy2000.Text = "Tandy 2000"
        RadioFormatTandy2000.UseVisualStyleBackColor = True
        '
        'RadioFormatProCopy
        '
        RadioFormatProCopy.AutoSize = True
        RadioFormatProCopy.Location = New System.Drawing.Point(190, 66)
        RadioFormatProCopy.Name = "RadioFormatProCopy"
        RadioFormatProCopy.Size = New System.Drawing.Size(65, 17)
        RadioFormatProCopy.TabIndex = 12
        RadioFormatProCopy.Tag = "11"
        RadioFormatProCopy.Text = "ProCopy"
        RadioFormatProCopy.UseVisualStyleBackColor = True
        '
        'RadioFormatDMF2048
        '
        RadioFormatDMF2048.AutoSize = True
        RadioFormatDMF2048.Location = New System.Drawing.Point(190, 44)
        RadioFormatDMF2048.Name = "RadioFormatDMF2048"
        RadioFormatDMF2048.Size = New System.Drawing.Size(121, 17)
        RadioFormatDMF2048.TabIndex = 11
        RadioFormatDMF2048.Tag = "10"
        RadioFormatDMF2048.Text = "DMF (2048 Clusters)"
        RadioFormatDMF2048.UseVisualStyleBackColor = True
        '
        'RadioFormatDMF1024
        '
        RadioFormatDMF1024.AutoSize = True
        RadioFormatDMF1024.Location = New System.Drawing.Point(190, 22)
        RadioFormatDMF1024.Name = "RadioFormatDMF1024"
        RadioFormatDMF1024.Size = New System.Drawing.Size(121, 17)
        RadioFormatDMF1024.TabIndex = 10
        RadioFormatDMF1024.Tag = "9"
        RadioFormatDMF1024.Text = "DMF (1024 Clusters)"
        RadioFormatDMF1024.UseVisualStyleBackColor = True
        '
        'PanelBottom
        '
        PanelBottom.BackColor = System.Drawing.SystemColors.Control
        PanelBottom.Controls.Add(Me.BtnCancel)
        PanelBottom.Controls.Add(Me.BtnOK)
        PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        PanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        PanelBottom.Location = New System.Drawing.Point(0, 210)
        PanelBottom.Name = "PanelBottom"
        PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
        PanelBottom.Size = New System.Drawing.Size(383, 43)
        PanelBottom.TabIndex = 1
        PanelBottom.WrapContents = False
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(290, 10)
        Me.BtnCancel.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "{Cancel}"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnOK
        '
        Me.BtnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Location = New System.Drawing.Point(203, 10)
        Me.BtnOK.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(75, 23)
        Me.BtnOK.TabIndex = 0
        Me.BtnOK.Text = "{Ok}"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'PanelMain
        '
        PanelMain.AutoSize = True
        PanelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        PanelMain.BackColor = System.Drawing.SystemColors.Window
        PanelMain.Controls.Add(Me.PanelFormats)
        PanelMain.Controls.Add(Me.LabelBootSector)
        PanelMain.Controls.Add(Me.ComboBootSector)
        PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        PanelMain.Location = New System.Drawing.Point(0, 0)
        PanelMain.Name = "PanelMain"
        PanelMain.Padding = New System.Windows.Forms.Padding(18, 18, 18, 6)
        PanelMain.Size = New System.Drawing.Size(383, 210)
        PanelMain.TabIndex = 0
        '
        'PanelFormats
        '
        Me.PanelFormats.Controls.Add(RadioFormatTandy2000)
        Me.PanelFormats.Controls.Add(RadioFormatProCopy)
        Me.PanelFormats.Controls.Add(RadioFormatDMF2048)
        Me.PanelFormats.Controls.Add(RadioFormatDMF1024)
        Me.PanelFormats.Controls.Add(Me.GroupBoxSpecial)
        Me.PanelFormats.Controls.Add(RadioFormat2880)
        Me.PanelFormats.Controls.Add(RadioFormat1440)
        Me.PanelFormats.Controls.Add(RadioFormat1200)
        Me.PanelFormats.Controls.Add(RadioFormat720)
        Me.PanelFormats.Controls.Add(RadioFormat360)
        Me.PanelFormats.Controls.Add(RadioFormat320)
        Me.PanelFormats.Controls.Add(RadioFormat180)
        Me.PanelFormats.Controls.Add(RadioFormat160)
        Me.PanelFormats.Controls.Add(Me.GroupBoxStandard)
        Me.PanelFormats.Location = New System.Drawing.Point(18, 18)
        Me.PanelFormats.Margin = New System.Windows.Forms.Padding(0)
        Me.PanelFormats.Name = "PanelFormats"
        Me.PanelFormats.Size = New System.Drawing.Size(344, 122)
        Me.PanelFormats.TabIndex = 0
        '
        'GroupBoxSpecial
        '
        Me.GroupBoxSpecial.Location = New System.Drawing.Point(181, 0)
        Me.GroupBoxSpecial.Name = "GroupBoxSpecial"
        Me.GroupBoxSpecial.Size = New System.Drawing.Size(161, 16)
        Me.GroupBoxSpecial.TabIndex = 1
        Me.GroupBoxSpecial.TabStop = False
        Me.GroupBoxSpecial.Text = "{Special Formats}"
        '
        'GroupBoxStandard
        '
        Me.GroupBoxStandard.Location = New System.Drawing.Point(0, 0)
        Me.GroupBoxStandard.Name = "GroupBoxStandard"
        Me.GroupBoxStandard.Size = New System.Drawing.Size(161, 16)
        Me.GroupBoxStandard.TabIndex = 0
        Me.GroupBoxStandard.TabStop = False
        Me.GroupBoxStandard.Text = "{Standard Formats}"
        '
        'LabelBootSector
        '
        Me.LabelBootSector.AutoSize = True
        Me.LabelBootSector.Location = New System.Drawing.Point(18, 153)
        Me.LabelBootSector.Name = "LabelBootSector"
        Me.LabelBootSector.Size = New System.Drawing.Size(71, 13)
        Me.LabelBootSector.TabIndex = 1
        Me.LabelBootSector.Text = "{Boot Sector}"
        '
        'ComboBootSector
        '
        Me.ComboBootSector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBootSector.FormattingEnabled = True
        Me.ComboBootSector.Location = New System.Drawing.Point(18, 169)
        Me.ComboBootSector.Name = "ComboBootSector"
        Me.ComboBootSector.Size = New System.Drawing.Size(344, 21)
        Me.ComboBootSector.TabIndex = 2
        '
        'ImageCreationForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(383, 253)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ImageCreationForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        PanelBottom.ResumeLayout(False)
        PanelMain.ResumeLayout(False)
        PanelMain.PerformLayout()
        Me.PanelFormats.ResumeLayout(False)
        Me.PanelFormats.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents PanelFormats As Panel
    Friend WithEvents GroupBoxStandard As GroupBox
    Friend WithEvents GroupBoxSpecial As GroupBox
    Friend WithEvents LabelBootSector As Label
    Friend WithEvents ComboBootSector As ComboBox
End Class
