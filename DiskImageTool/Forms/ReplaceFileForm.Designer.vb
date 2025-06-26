<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ReplaceFileForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ReplaceFileForm))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.GroupBoxOriginal = New System.Windows.Forms.GroupBox()
        Me.ChkFileSizeOriginal = New System.Windows.Forms.CheckBox()
        Me.ChkFileDateOriginal = New System.Windows.Forms.CheckBox()
        Me.ChkFilenameOriginal = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.BtnUndo = New System.Windows.Forms.Button()
        Me.TxtFileExtNew = New System.Windows.Forms.TextBox()
        Me.TxtFilenameNew = New System.Windows.Forms.TextBox()
        Me.ChkFileSizeNew = New System.Windows.Forms.CheckBox()
        Me.ChkFileDateNew = New System.Windows.Forms.CheckBox()
        Me.ChkFilenameNew = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.LblFileNameCaption = New System.Windows.Forms.Label()
        Me.LblFileName = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        Me.LblFileDateCaption = New System.Windows.Forms.Label()
        Me.LblFileDate = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel3 = New System.Windows.Forms.FlowLayoutPanel()
        Me.LblFileSizeCaption = New System.Windows.Forms.Label()
        Me.LblFileSize = New System.Windows.Forms.Label()
        Me.LblFileSizeError = New System.Windows.Forms.Label()
        Me.FlowLayoutPad = New System.Windows.Forms.FlowLayoutPanel()
        Me.LblPadCaption = New System.Windows.Forms.Label()
        Me.RadioFillF6 = New System.Windows.Forms.RadioButton()
        Me.RadioFill00 = New System.Windows.Forms.RadioButton()
        Me.Panel1.SuspendLayout()
        Me.GroupBoxOriginal.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.FlowLayoutPanel2.SuspendLayout()
        Me.FlowLayoutPanel3.SuspendLayout()
        Me.FlowLayoutPad.SuspendLayout()
        Me.SuspendLayout()
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
        'GroupBoxOriginal
        '
        Me.GroupBoxOriginal.Controls.Add(Me.ChkFileSizeOriginal)
        Me.GroupBoxOriginal.Controls.Add(Me.ChkFileDateOriginal)
        Me.GroupBoxOriginal.Controls.Add(Me.ChkFilenameOriginal)
        resources.ApplyResources(Me.GroupBoxOriginal, "GroupBoxOriginal")
        Me.GroupBoxOriginal.Name = "GroupBoxOriginal"
        Me.GroupBoxOriginal.TabStop = False
        '
        'ChkFileSizeOriginal
        '
        resources.ApplyResources(Me.ChkFileSizeOriginal, "ChkFileSizeOriginal")
        Me.ChkFileSizeOriginal.Name = "ChkFileSizeOriginal"
        Me.ChkFileSizeOriginal.UseMnemonic = False
        Me.ChkFileSizeOriginal.UseVisualStyleBackColor = True
        '
        'ChkFileDateOriginal
        '
        resources.ApplyResources(Me.ChkFileDateOriginal, "ChkFileDateOriginal")
        Me.ChkFileDateOriginal.Name = "ChkFileDateOriginal"
        Me.ChkFileDateOriginal.UseMnemonic = False
        Me.ChkFileDateOriginal.UseVisualStyleBackColor = True
        '
        'ChkFilenameOriginal
        '
        resources.ApplyResources(Me.ChkFilenameOriginal, "ChkFilenameOriginal")
        Me.ChkFilenameOriginal.Name = "ChkFilenameOriginal"
        Me.ChkFilenameOriginal.UseMnemonic = False
        Me.ChkFilenameOriginal.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.BtnUndo)
        Me.GroupBox1.Controls.Add(Me.TxtFileExtNew)
        Me.GroupBox1.Controls.Add(Me.TxtFilenameNew)
        Me.GroupBox1.Controls.Add(Me.ChkFileSizeNew)
        Me.GroupBox1.Controls.Add(Me.ChkFileDateNew)
        Me.GroupBox1.Controls.Add(Me.ChkFilenameNew)
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'BtnUndo
        '
        Me.BtnUndo.FlatAppearance.BorderSize = 0
        resources.ApplyResources(Me.BtnUndo, "BtnUndo")
        Me.BtnUndo.Name = "BtnUndo"
        Me.BtnUndo.UseVisualStyleBackColor = True
        '
        'TxtFileExtNew
        '
        Me.TxtFileExtNew.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        resources.ApplyResources(Me.TxtFileExtNew, "TxtFileExtNew")
        Me.TxtFileExtNew.Name = "TxtFileExtNew"
        Me.TxtFileExtNew.ShortcutsEnabled = False
        '
        'TxtFilenameNew
        '
        Me.TxtFilenameNew.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        resources.ApplyResources(Me.TxtFilenameNew, "TxtFilenameNew")
        Me.TxtFilenameNew.Name = "TxtFilenameNew"
        Me.TxtFilenameNew.ShortcutsEnabled = False
        '
        'ChkFileSizeNew
        '
        resources.ApplyResources(Me.ChkFileSizeNew, "ChkFileSizeNew")
        Me.ChkFileSizeNew.Name = "ChkFileSizeNew"
        Me.ChkFileSizeNew.UseMnemonic = False
        Me.ChkFileSizeNew.UseVisualStyleBackColor = True
        '
        'ChkFileDateNew
        '
        resources.ApplyResources(Me.ChkFileDateNew, "ChkFileDateNew")
        Me.ChkFileDateNew.Name = "ChkFileDateNew"
        Me.ChkFileDateNew.UseMnemonic = False
        Me.ChkFileDateNew.UseVisualStyleBackColor = True
        '
        'ChkFilenameNew
        '
        resources.ApplyResources(Me.ChkFilenameNew, "ChkFilenameNew")
        Me.ChkFilenameNew.Name = "ChkFilenameNew"
        Me.ChkFilenameNew.UseMnemonic = False
        Me.ChkFilenameNew.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel1
        '
        resources.ApplyResources(Me.FlowLayoutPanel1, "FlowLayoutPanel1")
        Me.FlowLayoutPanel1.Controls.Add(Me.LblFileNameCaption)
        Me.FlowLayoutPanel1.Controls.Add(Me.LblFileName)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        '
        'LblFileNameCaption
        '
        resources.ApplyResources(Me.LblFileNameCaption, "LblFileNameCaption")
        Me.LblFileNameCaption.Name = "LblFileNameCaption"
        Me.LblFileNameCaption.UseMnemonic = False
        '
        'LblFileName
        '
        resources.ApplyResources(Me.LblFileName, "LblFileName")
        Me.LblFileName.ForeColor = System.Drawing.Color.Blue
        Me.LblFileName.Name = "LblFileName"
        Me.LblFileName.UseMnemonic = False
        '
        'FlowLayoutPanel2
        '
        resources.ApplyResources(Me.FlowLayoutPanel2, "FlowLayoutPanel2")
        Me.FlowLayoutPanel2.Controls.Add(Me.LblFileDateCaption)
        Me.FlowLayoutPanel2.Controls.Add(Me.LblFileDate)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        '
        'LblFileDateCaption
        '
        resources.ApplyResources(Me.LblFileDateCaption, "LblFileDateCaption")
        Me.LblFileDateCaption.Name = "LblFileDateCaption"
        Me.LblFileDateCaption.UseMnemonic = False
        '
        'LblFileDate
        '
        resources.ApplyResources(Me.LblFileDate, "LblFileDate")
        Me.LblFileDate.ForeColor = System.Drawing.Color.Blue
        Me.LblFileDate.Name = "LblFileDate"
        Me.LblFileDate.UseMnemonic = False
        '
        'FlowLayoutPanel3
        '
        resources.ApplyResources(Me.FlowLayoutPanel3, "FlowLayoutPanel3")
        Me.FlowLayoutPanel3.Controls.Add(Me.LblFileSizeCaption)
        Me.FlowLayoutPanel3.Controls.Add(Me.LblFileSize)
        Me.FlowLayoutPanel3.Controls.Add(Me.LblFileSizeError)
        Me.FlowLayoutPanel3.Name = "FlowLayoutPanel3"
        '
        'LblFileSizeCaption
        '
        resources.ApplyResources(Me.LblFileSizeCaption, "LblFileSizeCaption")
        Me.LblFileSizeCaption.Name = "LblFileSizeCaption"
        Me.LblFileSizeCaption.UseMnemonic = False
        '
        'LblFileSize
        '
        resources.ApplyResources(Me.LblFileSize, "LblFileSize")
        Me.LblFileSize.ForeColor = System.Drawing.Color.Blue
        Me.LblFileSize.Name = "LblFileSize"
        Me.LblFileSize.UseMnemonic = False
        '
        'LblFileSizeError
        '
        resources.ApplyResources(Me.LblFileSizeError, "LblFileSizeError")
        Me.LblFileSizeError.ForeColor = System.Drawing.Color.Red
        Me.LblFileSizeError.Name = "LblFileSizeError"
        Me.LblFileSizeError.UseMnemonic = False
        '
        'FlowLayoutPad
        '
        resources.ApplyResources(Me.FlowLayoutPad, "FlowLayoutPad")
        Me.FlowLayoutPad.Controls.Add(Me.LblPadCaption)
        Me.FlowLayoutPad.Controls.Add(Me.RadioFillF6)
        Me.FlowLayoutPad.Controls.Add(Me.RadioFill00)
        Me.FlowLayoutPad.Name = "FlowLayoutPad"
        '
        'LblPadCaption
        '
        resources.ApplyResources(Me.LblPadCaption, "LblPadCaption")
        Me.LblPadCaption.Name = "LblPadCaption"
        Me.LblPadCaption.UseMnemonic = False
        '
        'RadioFillF6
        '
        resources.ApplyResources(Me.RadioFillF6, "RadioFillF6")
        Me.RadioFillF6.Checked = True
        Me.RadioFillF6.Name = "RadioFillF6"
        Me.RadioFillF6.TabStop = True
        Me.RadioFillF6.UseMnemonic = False
        Me.RadioFillF6.UseVisualStyleBackColor = True
        '
        'RadioFill00
        '
        resources.ApplyResources(Me.RadioFill00, "RadioFill00")
        Me.RadioFill00.Name = "RadioFill00"
        Me.RadioFill00.TabStop = True
        Me.RadioFill00.UseMnemonic = False
        Me.RadioFill00.UseVisualStyleBackColor = True
        '
        'ReplaceFileForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.CancelButton = Me.BtnCancel
        Me.Controls.Add(Me.FlowLayoutPad)
        Me.Controls.Add(Me.FlowLayoutPanel3)
        Me.Controls.Add(Me.FlowLayoutPanel2)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.GroupBoxOriginal)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ReplaceFileForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Panel1.ResumeLayout(False)
        Me.GroupBoxOriginal.ResumeLayout(False)
        Me.GroupBoxOriginal.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.FlowLayoutPanel2.ResumeLayout(False)
        Me.FlowLayoutPanel2.PerformLayout()
        Me.FlowLayoutPanel3.ResumeLayout(False)
        Me.FlowLayoutPanel3.PerformLayout()
        Me.FlowLayoutPad.ResumeLayout(False)
        Me.FlowLayoutPad.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents GroupBoxOriginal As GroupBox
    Friend WithEvents ChkFileDateOriginal As CheckBox
    Friend WithEvents ChkFilenameOriginal As CheckBox
    Friend WithEvents ChkFileSizeOriginal As CheckBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents ChkFileSizeNew As CheckBox
    Friend WithEvents ChkFileDateNew As CheckBox
    Friend WithEvents ChkFilenameNew As CheckBox
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents LblFileNameCaption As Label
    Friend WithEvents LblFileName As Label
    Friend WithEvents FlowLayoutPanel2 As FlowLayoutPanel
    Friend WithEvents LblFileDateCaption As Label
    Friend WithEvents LblFileDate As Label
    Friend WithEvents FlowLayoutPanel3 As FlowLayoutPanel
    Friend WithEvents LblFileSizeCaption As Label
    Friend WithEvents LblFileSize As Label
    Friend WithEvents FlowLayoutPad As FlowLayoutPanel
    Friend WithEvents LblPadCaption As Label
    Friend WithEvents RadioFillF6 As RadioButton
    Friend WithEvents RadioFill00 As RadioButton
    Friend WithEvents LblFileSizeError As Label
    Friend WithEvents TxtFilenameNew As TextBox
    Friend WithEvents TxtFileExtNew As TextBox
    Friend WithEvents BtnUndo As Button
End Class
