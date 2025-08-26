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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ImageCreationForm))
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
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.PanelFormats = New System.Windows.Forms.Panel()
        Me.GroupBoxSpecial = New System.Windows.Forms.GroupBox()
        Me.GroupBoxStandard = New System.Windows.Forms.GroupBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ComboBootSector = New System.Windows.Forms.ComboBox()
        Me.CheckImportFiles = New System.Windows.Forms.CheckBox()
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
        Me.Panel1.SuspendLayout()
        Me.PanelFormats.SuspendLayout()
        Me.SuspendLayout()
        '
        'RadioFormat2880
        '
        resources.ApplyResources(RadioFormat2880, "RadioFormat2880")
        RadioFormat2880.Name = "RadioFormat2880"
        RadioFormat2880.Tag = "8"
        RadioFormat2880.UseVisualStyleBackColor = True
        '
        'RadioFormat1440
        '
        resources.ApplyResources(RadioFormat1440, "RadioFormat1440")
        RadioFormat1440.Checked = True
        RadioFormat1440.Name = "RadioFormat1440"
        RadioFormat1440.TabStop = True
        RadioFormat1440.Tag = "7"
        RadioFormat1440.UseVisualStyleBackColor = True
        '
        'RadioFormat1200
        '
        resources.ApplyResources(RadioFormat1200, "RadioFormat1200")
        RadioFormat1200.Name = "RadioFormat1200"
        RadioFormat1200.Tag = "6"
        RadioFormat1200.UseVisualStyleBackColor = True
        '
        'RadioFormat720
        '
        resources.ApplyResources(RadioFormat720, "RadioFormat720")
        RadioFormat720.Name = "RadioFormat720"
        RadioFormat720.Tag = "5"
        RadioFormat720.UseVisualStyleBackColor = True
        '
        'RadioFormat360
        '
        resources.ApplyResources(RadioFormat360, "RadioFormat360")
        RadioFormat360.Name = "RadioFormat360"
        RadioFormat360.Tag = "4"
        RadioFormat360.UseVisualStyleBackColor = True
        '
        'RadioFormat320
        '
        resources.ApplyResources(RadioFormat320, "RadioFormat320")
        RadioFormat320.Name = "RadioFormat320"
        RadioFormat320.Tag = "3"
        RadioFormat320.UseVisualStyleBackColor = True
        '
        'RadioFormat180
        '
        resources.ApplyResources(RadioFormat180, "RadioFormat180")
        RadioFormat180.Name = "RadioFormat180"
        RadioFormat180.Tag = "2"
        RadioFormat180.UseVisualStyleBackColor = True
        '
        'RadioFormat160
        '
        resources.ApplyResources(RadioFormat160, "RadioFormat160")
        RadioFormat160.Name = "RadioFormat160"
        RadioFormat160.Tag = "1"
        RadioFormat160.UseVisualStyleBackColor = True
        '
        'RadioFormatTandy2000
        '
        resources.ApplyResources(RadioFormatTandy2000, "RadioFormatTandy2000")
        RadioFormatTandy2000.Name = "RadioFormatTandy2000"
        RadioFormatTandy2000.Tag = "15"
        RadioFormatTandy2000.UseVisualStyleBackColor = True
        '
        'RadioFormatProCopy
        '
        resources.ApplyResources(RadioFormatProCopy, "RadioFormatProCopy")
        RadioFormatProCopy.Name = "RadioFormatProCopy"
        RadioFormatProCopy.Tag = "11"
        RadioFormatProCopy.UseVisualStyleBackColor = True
        '
        'RadioFormatDMF2048
        '
        resources.ApplyResources(RadioFormatDMF2048, "RadioFormatDMF2048")
        RadioFormatDMF2048.Name = "RadioFormatDMF2048"
        RadioFormatDMF2048.Tag = "10"
        RadioFormatDMF2048.UseVisualStyleBackColor = True
        '
        'RadioFormatDMF1024
        '
        resources.ApplyResources(RadioFormatDMF1024, "RadioFormatDMF1024")
        RadioFormatDMF1024.Name = "RadioFormatDMF1024"
        RadioFormatDMF1024.Tag = "9"
        RadioFormatDMF1024.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.Control
        Me.Panel1.Controls.Add(Me.BtnOK)
        Me.Panel1.Controls.Add(Me.BtnCancel)
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Name = "Panel1"
        '
        'BtnOK
        '
        resources.ApplyResources(Me.BtnOK, "BtnOK")
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
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
        resources.ApplyResources(Me.PanelFormats, "PanelFormats")
        Me.PanelFormats.Name = "PanelFormats"
        '
        'GroupBoxSpecial
        '
        resources.ApplyResources(Me.GroupBoxSpecial, "GroupBoxSpecial")
        Me.GroupBoxSpecial.Name = "GroupBoxSpecial"
        Me.GroupBoxSpecial.TabStop = False
        '
        'GroupBoxStandard
        '
        resources.ApplyResources(Me.GroupBoxStandard, "GroupBoxStandard")
        Me.GroupBoxStandard.Name = "GroupBoxStandard"
        Me.GroupBoxStandard.TabStop = False
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'ComboBootSector
        '
        Me.ComboBootSector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBootSector.FormattingEnabled = True
        resources.ApplyResources(Me.ComboBootSector, "ComboBootSector")
        Me.ComboBootSector.Name = "ComboBootSector"
        '
        'CheckImportFiles
        '
        resources.ApplyResources(Me.CheckImportFiles, "CheckImportFiles")
        Me.CheckImportFiles.Name = "CheckImportFiles"
        Me.CheckImportFiles.UseVisualStyleBackColor = True
        '
        'ImageCreationForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.Controls.Add(Me.CheckImportFiles)
        Me.Controls.Add(Me.ComboBootSector)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.PanelFormats)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ImageCreationForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Panel1.ResumeLayout(False)
        Me.PanelFormats.ResumeLayout(False)
        Me.PanelFormats.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Panel1 As Panel
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents PanelFormats As Panel
    Friend WithEvents GroupBoxStandard As GroupBox
    Friend WithEvents GroupBoxSpecial As GroupBox
    Friend WithEvents Label1 As Label
    Friend WithEvents ComboBootSector As ComboBox
    Friend WithEvents CheckImportFiles As CheckBox
End Class
