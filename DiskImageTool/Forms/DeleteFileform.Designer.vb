<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DeleteFileForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.LblCaption = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.RadioFillKeep = New System.Windows.Forms.RadioButton()
        Me.RadioFill00 = New System.Windows.Forms.RadioButton()
        Me.RadioFillF6 = New System.Windows.Forms.RadioButton()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'LblCaption
        '
        Me.LblCaption.AutoSize = True
        Me.LblCaption.Location = New System.Drawing.Point(61, 34)
        Me.LblCaption.Name = "LblCaption"
        Me.LblCaption.Size = New System.Drawing.Size(43, 13)
        Me.LblCaption.TabIndex = 0
        Me.LblCaption.Text = "Caption"
        Me.LblCaption.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.LblCaption.UseMnemonic = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.GroupBox1.Controls.Add(Me.RadioFillKeep)
        Me.GroupBox1.Controls.Add(Me.RadioFill00)
        Me.GroupBox1.Controls.Add(Me.RadioFillF6)
        Me.GroupBox1.Location = New System.Drawing.Point(63, 80)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(216, 47)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Fill Sectors  With"
        '
        'RadioFillKeep
        '
        Me.RadioFillKeep.AutoSize = True
        Me.RadioFillKeep.Checked = True
        Me.RadioFillKeep.Location = New System.Drawing.Point(15, 19)
        Me.RadioFillKeep.Name = "RadioFillKeep"
        Me.RadioFillKeep.Size = New System.Drawing.Size(76, 17)
        Me.RadioFillKeep.TabIndex = 0
        Me.RadioFillKeep.TabStop = True
        Me.RadioFillKeep.Text = "Keep Data"
        Me.RadioFillKeep.UseVisualStyleBackColor = True
        '
        'RadioFill00
        '
        Me.RadioFill00.AutoSize = True
        Me.RadioFill00.Location = New System.Drawing.Point(158, 19)
        Me.RadioFill00.Name = "RadioFill00"
        Me.RadioFill00.Size = New System.Drawing.Size(48, 17)
        Me.RadioFill00.TabIndex = 2
        Me.RadioFill00.Text = "0x00"
        Me.RadioFill00.UseMnemonic = False
        Me.RadioFill00.UseVisualStyleBackColor = True
        '
        'RadioFillF6
        '
        Me.RadioFillF6.AutoSize = True
        Me.RadioFillF6.Location = New System.Drawing.Point(98, 19)
        Me.RadioFillF6.Name = "RadioFillF6"
        Me.RadioFillF6.Size = New System.Drawing.Size(48, 17)
        Me.RadioFillF6.TabIndex = 1
        Me.RadioFillF6.Text = "0xF6"
        Me.RadioFillF6.UseMnemonic = False
        Me.RadioFillF6.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.Control
        Me.Panel1.Controls.Add(Me.BtnOK)
        Me.Panel1.Controls.Add(Me.BtnCancel)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 149)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(3, 3, 50, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(332, 42)
        Me.Panel1.TabIndex = 2
        '
        'BtnOK
        '
        Me.BtnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Location = New System.Drawing.Point(158, 10)
        Me.BtnOK.Margin = New System.Windows.Forms.Padding(4, 10, 4, 9)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(75, 23)
        Me.BtnOK.TabIndex = 0
        Me.BtnOK.Text = "&Yes"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        Me.BtnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(241, 10)
        Me.BtnCancel.Margin = New System.Windows.Forms.Padding(4, 10, 4, 9)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "&No"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'DeleteFileForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.CancelButton = Me.BtnCancel
        Me.ClientSize = New System.Drawing.Size(332, 191)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.LblCaption)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DeleteFileForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Delete File"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents LblCaption As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents RadioFill00 As RadioButton
    Friend WithEvents RadioFillF6 As RadioButton
    Friend WithEvents Panel1 As Panel
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents RadioFillKeep As RadioButton
End Class
