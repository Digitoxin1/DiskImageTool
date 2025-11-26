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
        Dim FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
        Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
        Me.RadioFillKeep = New System.Windows.Forms.RadioButton()
        Me.RadioFillF6 = New System.Windows.Forms.RadioButton()
        Me.RadioFill00 = New System.Windows.Forms.RadioButton()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.LblCaption = New System.Windows.Forms.Label()
        FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        FlowLayoutPanel1.SuspendLayout()
        PanelBottom.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'FlowLayoutPanel1
        '
        FlowLayoutPanel1.AutoSize = True
        FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        FlowLayoutPanel1.Controls.Add(Me.RadioFillKeep)
        FlowLayoutPanel1.Controls.Add(Me.RadioFillF6)
        FlowLayoutPanel1.Controls.Add(Me.RadioFill00)
        FlowLayoutPanel1.Location = New System.Drawing.Point(12, 19)
        FlowLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
        FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        FlowLayoutPanel1.Size = New System.Drawing.Size(198, 23)
        FlowLayoutPanel1.TabIndex = 0
        FlowLayoutPanel1.WrapContents = False
        '
        'RadioFillKeep
        '
        Me.RadioFillKeep.AutoSize = True
        Me.RadioFillKeep.Checked = True
        Me.RadioFillKeep.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.RadioFillKeep.Location = New System.Drawing.Point(3, 3)
        Me.RadioFillKeep.Name = "RadioFillKeep"
        Me.RadioFillKeep.Size = New System.Drawing.Size(84, 17)
        Me.RadioFillKeep.TabIndex = 0
        Me.RadioFillKeep.TabStop = True
        Me.RadioFillKeep.Text = "{Keep Data}"
        Me.RadioFillKeep.UseVisualStyleBackColor = True
        '
        'RadioFillF6
        '
        Me.RadioFillF6.AutoSize = True
        Me.RadioFillF6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.RadioFillF6.Location = New System.Drawing.Point(93, 3)
        Me.RadioFillF6.Name = "RadioFillF6"
        Me.RadioFillF6.Size = New System.Drawing.Size(48, 17)
        Me.RadioFillF6.TabIndex = 1
        Me.RadioFillF6.Text = "0xF6"
        Me.RadioFillF6.UseMnemonic = False
        Me.RadioFillF6.UseVisualStyleBackColor = True
        '
        'RadioFill00
        '
        Me.RadioFill00.AutoSize = True
        Me.RadioFill00.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.RadioFill00.Location = New System.Drawing.Point(147, 3)
        Me.RadioFill00.Name = "RadioFill00"
        Me.RadioFill00.Size = New System.Drawing.Size(48, 17)
        Me.RadioFill00.TabIndex = 2
        Me.RadioFill00.Text = "0x00"
        Me.RadioFill00.UseMnemonic = False
        Me.RadioFill00.UseVisualStyleBackColor = True
        '
        'PanelBottom
        '
        PanelBottom.BackColor = System.Drawing.SystemColors.Control
        PanelBottom.Controls.Add(Me.BtnCancel)
        PanelBottom.Controls.Add(Me.BtnOK)
        PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        PanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        PanelBottom.Location = New System.Drawing.Point(0, 148)
        PanelBottom.Name = "PanelBottom"
        PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
        PanelBottom.Size = New System.Drawing.Size(332, 43)
        PanelBottom.TabIndex = 2
        PanelBottom.WrapContents = False
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(239, 10)
        Me.BtnCancel.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "{&No}"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnOK
        '
        Me.BtnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Location = New System.Drawing.Point(152, 10)
        Me.BtnOK.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(75, 23)
        Me.BtnOK.TabIndex = 0
        Me.BtnOK.Text = "{&Yes}"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.AutoSize = True
        Me.GroupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupBox1.Controls.Add(FlowLayoutPanel1)
        Me.GroupBox1.Location = New System.Drawing.Point(63, 72)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(3, 3, 12, 0)
        Me.GroupBox1.Size = New System.Drawing.Size(222, 55)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "{Fill Sectors With}"
        '
        'LblCaption
        '
        Me.LblCaption.AutoSize = True
        Me.LblCaption.Location = New System.Drawing.Point(61, 34)
        Me.LblCaption.Name = "LblCaption"
        Me.LblCaption.Size = New System.Drawing.Size(51, 13)
        Me.LblCaption.TabIndex = 0
        Me.LblCaption.Text = "{Caption}"
        Me.LblCaption.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.LblCaption.UseMnemonic = False
        '
        'DeleteFileForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.CancelButton = Me.BtnCancel
        Me.ClientSize = New System.Drawing.Size(332, 191)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(PanelBottom)
        Me.Controls.Add(Me.LblCaption)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DeleteFileForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        FlowLayoutPanel1.ResumeLayout(False)
        FlowLayoutPanel1.PerformLayout()
        PanelBottom.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents LblCaption As Label
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents RadioFillKeep As RadioButton
    Friend WithEvents RadioFillF6 As RadioButton
    Friend WithEvents RadioFill00 As RadioButton
    Friend WithEvents GroupBox1 As GroupBox
End Class
