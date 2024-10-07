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
        Me.BtnUndo = New System.Windows.Forms.Button()
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
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 256)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(3, 3, 50, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(512, 42)
        Me.Panel1.TabIndex = 16
        '
        'BtnOK
        '
        Me.BtnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnOK.Location = New System.Drawing.Point(338, 10)
        Me.BtnOK.Margin = New System.Windows.Forms.Padding(4, 10, 4, 9)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(75, 23)
        Me.BtnOK.TabIndex = 17
        Me.BtnOK.Text = "Replace"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        Me.BtnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(421, 10)
        Me.BtnCancel.Margin = New System.Windows.Forms.Padding(4, 10, 4, 9)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
        Me.BtnCancel.TabIndex = 18
        Me.BtnCancel.Text = "Cancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'GroupBoxOriginal
        '
        Me.GroupBoxOriginal.Controls.Add(Me.ChkFileSizeOriginal)
        Me.GroupBoxOriginal.Controls.Add(Me.ChkFileDateOriginal)
        Me.GroupBoxOriginal.Controls.Add(Me.ChkFilenameOriginal)
        Me.GroupBoxOriginal.Location = New System.Drawing.Point(27, 27)
        Me.GroupBoxOriginal.Margin = New System.Windows.Forms.Padding(18)
        Me.GroupBoxOriginal.Name = "GroupBoxOriginal"
        Me.GroupBoxOriginal.Size = New System.Drawing.Size(212, 108)
        Me.GroupBoxOriginal.TabIndex = 0
        Me.GroupBoxOriginal.TabStop = False
        Me.GroupBoxOriginal.Text = "Original File"
        '
        'ChkFileSizeOriginal
        '
        Me.ChkFileSizeOriginal.AutoSize = True
        Me.ChkFileSizeOriginal.Location = New System.Drawing.Point(10, 75)
        Me.ChkFileSizeOriginal.Name = "ChkFileSizeOriginal"
        Me.ChkFileSizeOriginal.Size = New System.Drawing.Size(60, 17)
        Me.ChkFileSizeOriginal.TabIndex = 2
        Me.ChkFileSizeOriginal.Text = "0 bytes"
        Me.ChkFileSizeOriginal.UseMnemonic = False
        Me.ChkFileSizeOriginal.UseVisualStyleBackColor = True
        '
        'ChkFileDateOriginal
        '
        Me.ChkFileDateOriginal.AutoSize = True
        Me.ChkFileDateOriginal.Location = New System.Drawing.Point(10, 51)
        Me.ChkFileDateOriginal.Name = "ChkFileDateOriginal"
        Me.ChkFileDateOriginal.Size = New System.Drawing.Size(121, 17)
        Me.ChkFileDateOriginal.TabIndex = 1
        Me.ChkFileDateOriginal.Text = "1/1/1980 12:00 AM"
        Me.ChkFileDateOriginal.UseMnemonic = False
        Me.ChkFileDateOriginal.UseVisualStyleBackColor = True
        '
        'ChkFilenameOriginal
        '
        Me.ChkFilenameOriginal.AutoSize = True
        Me.ChkFilenameOriginal.Location = New System.Drawing.Point(10, 28)
        Me.ChkFilenameOriginal.Name = "ChkFilenameOriginal"
        Me.ChkFilenameOriginal.Size = New System.Drawing.Size(79, 17)
        Me.ChkFilenameOriginal.TabIndex = 0
        Me.ChkFilenameOriginal.Text = "FILENAME"
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
        Me.GroupBox1.Location = New System.Drawing.Point(275, 27)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(18)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(212, 108)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "New File"
        '
        'TxtFileExtNew
        '
        Me.TxtFileExtNew.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.TxtFileExtNew.Location = New System.Drawing.Point(136, 25)
        Me.TxtFileExtNew.MaxLength = 3
        Me.TxtFileExtNew.Name = "TxtFileExtNew"
        Me.TxtFileExtNew.ShortcutsEnabled = False
        Me.TxtFileExtNew.Size = New System.Drawing.Size(44, 20)
        Me.TxtFileExtNew.TabIndex = 2
        '
        'TxtFilenameNew
        '
        Me.TxtFilenameNew.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.TxtFilenameNew.Location = New System.Drawing.Point(30, 25)
        Me.TxtFilenameNew.MaxLength = 8
        Me.TxtFilenameNew.Name = "TxtFilenameNew"
        Me.TxtFilenameNew.ShortcutsEnabled = False
        Me.TxtFilenameNew.Size = New System.Drawing.Size(100, 20)
        Me.TxtFilenameNew.TabIndex = 1
        '
        'ChkFileSizeNew
        '
        Me.ChkFileSizeNew.AutoSize = True
        Me.ChkFileSizeNew.Location = New System.Drawing.Point(10, 75)
        Me.ChkFileSizeNew.Name = "ChkFileSizeNew"
        Me.ChkFileSizeNew.Size = New System.Drawing.Size(60, 17)
        Me.ChkFileSizeNew.TabIndex = 5
        Me.ChkFileSizeNew.Text = "0 bytes"
        Me.ChkFileSizeNew.UseMnemonic = False
        Me.ChkFileSizeNew.UseVisualStyleBackColor = True
        '
        'ChkFileDateNew
        '
        Me.ChkFileDateNew.AutoSize = True
        Me.ChkFileDateNew.Location = New System.Drawing.Point(10, 51)
        Me.ChkFileDateNew.Name = "ChkFileDateNew"
        Me.ChkFileDateNew.Size = New System.Drawing.Size(121, 17)
        Me.ChkFileDateNew.TabIndex = 4
        Me.ChkFileDateNew.Text = "1/1/1980 12:00 AM"
        Me.ChkFileDateNew.UseMnemonic = False
        Me.ChkFileDateNew.UseVisualStyleBackColor = True
        '
        'ChkFilenameNew
        '
        Me.ChkFilenameNew.AutoSize = True
        Me.ChkFilenameNew.Location = New System.Drawing.Point(10, 28)
        Me.ChkFilenameNew.Name = "ChkFilenameNew"
        Me.ChkFilenameNew.Size = New System.Drawing.Size(15, 14)
        Me.ChkFilenameNew.TabIndex = 0
        Me.ChkFilenameNew.UseMnemonic = False
        Me.ChkFilenameNew.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel1.Controls.Add(Me.LblFileNameCaption)
        Me.FlowLayoutPanel1.Controls.Add(Me.LblFileName)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(27, 156)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(179, 13)
        Me.FlowLayoutPanel1.TabIndex = 3
        '
        'LblFileNameCaption
        '
        Me.LblFileNameCaption.AutoSize = True
        Me.LblFileNameCaption.Location = New System.Drawing.Point(0, 0)
        Me.LblFileNameCaption.Margin = New System.Windows.Forms.Padding(0)
        Me.LblFileNameCaption.Name = "LblFileNameCaption"
        Me.LblFileNameCaption.Size = New System.Drawing.Size(116, 13)
        Me.LblFileNameCaption.TabIndex = 4
        Me.LblFileNameCaption.Text = "File name will be set to:"
        Me.LblFileNameCaption.UseMnemonic = False
        '
        'LblFileName
        '
        Me.LblFileName.AutoSize = True
        Me.LblFileName.ForeColor = System.Drawing.Color.Blue
        Me.LblFileName.Location = New System.Drawing.Point(116, 0)
        Me.LblFileName.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.LblFileName.Name = "LblFileName"
        Me.LblFileName.Size = New System.Drawing.Size(60, 13)
        Me.LblFileName.TabIndex = 5
        Me.LblFileName.Text = "FILENAME"
        Me.LblFileName.UseMnemonic = False
        '
        'FlowLayoutPanel2
        '
        Me.FlowLayoutPanel2.AutoSize = True
        Me.FlowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel2.Controls.Add(Me.LblFileDateCaption)
        Me.FlowLayoutPanel2.Controls.Add(Me.LblFileDate)
        Me.FlowLayoutPanel2.Location = New System.Drawing.Point(27, 184)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        Me.FlowLayoutPanel2.Size = New System.Drawing.Size(254, 13)
        Me.FlowLayoutPanel2.TabIndex = 6
        '
        'LblFileDateCaption
        '
        Me.LblFileDateCaption.AutoSize = True
        Me.LblFileDateCaption.Location = New System.Drawing.Point(0, 0)
        Me.LblFileDateCaption.Margin = New System.Windows.Forms.Padding(0)
        Me.LblFileDateCaption.Name = "LblFileDateCaption"
        Me.LblFileDateCaption.Size = New System.Drawing.Size(149, 13)
        Me.LblFileDateCaption.TabIndex = 7
        Me.LblFileDateCaption.Text = "Last written date will be set to:"
        Me.LblFileDateCaption.UseMnemonic = False
        '
        'LblFileDate
        '
        Me.LblFileDate.AutoSize = True
        Me.LblFileDate.ForeColor = System.Drawing.Color.Blue
        Me.LblFileDate.Location = New System.Drawing.Point(149, 0)
        Me.LblFileDate.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.LblFileDate.Name = "LblFileDate"
        Me.LblFileDate.Size = New System.Drawing.Size(102, 13)
        Me.LblFileDate.TabIndex = 8
        Me.LblFileDate.Text = "1/1/1980 12:00 AM"
        Me.LblFileDate.UseMnemonic = False
        '
        'FlowLayoutPanel3
        '
        Me.FlowLayoutPanel3.AutoSize = True
        Me.FlowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel3.Controls.Add(Me.LblFileSizeCaption)
        Me.FlowLayoutPanel3.Controls.Add(Me.LblFileSize)
        Me.FlowLayoutPanel3.Controls.Add(Me.LblFileSizeError)
        Me.FlowLayoutPanel3.Location = New System.Drawing.Point(27, 212)
        Me.FlowLayoutPanel3.Name = "FlowLayoutPanel3"
        Me.FlowLayoutPanel3.Size = New System.Drawing.Size(193, 13)
        Me.FlowLayoutPanel3.TabIndex = 9
        '
        'LblFileSizeCaption
        '
        Me.LblFileSizeCaption.AutoSize = True
        Me.LblFileSizeCaption.Location = New System.Drawing.Point(0, 0)
        Me.LblFileSizeCaption.Margin = New System.Windows.Forms.Padding(0)
        Me.LblFileSizeCaption.Name = "LblFileSizeCaption"
        Me.LblFileSizeCaption.Size = New System.Drawing.Size(108, 13)
        Me.LblFileSizeCaption.TabIndex = 10
        Me.LblFileSizeCaption.Text = "File size will be set to:"
        Me.LblFileSizeCaption.UseMnemonic = False
        '
        'LblFileSize
        '
        Me.LblFileSize.AutoSize = True
        Me.LblFileSize.ForeColor = System.Drawing.Color.Blue
        Me.LblFileSize.Location = New System.Drawing.Point(108, 0)
        Me.LblFileSize.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.LblFileSize.Name = "LblFileSize"
        Me.LblFileSize.Size = New System.Drawing.Size(41, 13)
        Me.LblFileSize.TabIndex = 11
        Me.LblFileSize.Text = "0 bytes"
        Me.LblFileSize.UseMnemonic = False
        '
        'LblFileSizeError
        '
        Me.LblFileSizeError.AutoSize = True
        Me.LblFileSizeError.ForeColor = System.Drawing.Color.Red
        Me.LblFileSizeError.Location = New System.Drawing.Point(155, 0)
        Me.LblFileSizeError.Name = "LblFileSizeError"
        Me.LblFileSizeError.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.LblFileSizeError.Size = New System.Drawing.Size(35, 13)
        Me.LblFileSizeError.TabIndex = 12
        Me.LblFileSizeError.Text = "(Error)"
        Me.LblFileSizeError.UseMnemonic = False
        '
        'FlowLayoutPad
        '
        Me.FlowLayoutPad.AutoSize = True
        Me.FlowLayoutPad.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPad.Controls.Add(Me.LblPadCaption)
        Me.FlowLayoutPad.Controls.Add(Me.RadioFillF6)
        Me.FlowLayoutPad.Controls.Add(Me.RadioFill00)
        Me.FlowLayoutPad.Location = New System.Drawing.Point(275, 207)
        Me.FlowLayoutPad.Name = "FlowLayoutPad"
        Me.FlowLayoutPad.Size = New System.Drawing.Size(181, 23)
        Me.FlowLayoutPad.TabIndex = 12
        '
        'LblPadCaption
        '
        Me.LblPadCaption.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblPadCaption.AutoSize = True
        Me.LblPadCaption.Location = New System.Drawing.Point(3, 5)
        Me.LblPadCaption.Name = "LblPadCaption"
        Me.LblPadCaption.Size = New System.Drawing.Size(67, 13)
        Me.LblPadCaption.TabIndex = 13
        Me.LblPadCaption.Text = "Pad file with:"
        Me.LblPadCaption.UseMnemonic = False
        '
        'RadioFillF6
        '
        Me.RadioFillF6.AutoSize = True
        Me.RadioFillF6.Checked = True
        Me.RadioFillF6.Location = New System.Drawing.Point(76, 3)
        Me.RadioFillF6.Name = "RadioFillF6"
        Me.RadioFillF6.Size = New System.Drawing.Size(48, 17)
        Me.RadioFillF6.TabIndex = 14
        Me.RadioFillF6.TabStop = True
        Me.RadioFillF6.Text = "0xF6"
        Me.RadioFillF6.UseMnemonic = False
        Me.RadioFillF6.UseVisualStyleBackColor = True
        '
        'RadioFill00
        '
        Me.RadioFill00.AutoSize = True
        Me.RadioFill00.Location = New System.Drawing.Point(130, 3)
        Me.RadioFill00.Name = "RadioFill00"
        Me.RadioFill00.Size = New System.Drawing.Size(48, 17)
        Me.RadioFill00.TabIndex = 15
        Me.RadioFill00.TabStop = True
        Me.RadioFill00.Text = "0x00"
        Me.RadioFill00.UseMnemonic = False
        Me.RadioFill00.UseVisualStyleBackColor = True
        '
        'BtnUndo
        '
        Me.BtnUndo.FlatAppearance.BorderSize = 0
        Me.BtnUndo.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnUndo.Image = CType(resources.GetObject("BtnUndo.Image"), System.Drawing.Image)
        Me.BtnUndo.Location = New System.Drawing.Point(184, 24)
        Me.BtnUndo.Name = "BtnUndo"
        Me.BtnUndo.Size = New System.Drawing.Size(22, 22)
        Me.BtnUndo.TabIndex = 3
        Me.BtnUndo.UseVisualStyleBackColor = True
        '
        'ReplaceFileForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.CancelButton = Me.BtnCancel
        Me.ClientSize = New System.Drawing.Size(512, 298)
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
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Replace File"
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
