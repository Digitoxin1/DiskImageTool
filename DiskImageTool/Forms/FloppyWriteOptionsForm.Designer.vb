<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FloppyWriteOptionsForm
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
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.CheckFormat = New System.Windows.Forms.CheckBox()
        Me.CheckVerify = New System.Windows.Forms.CheckBox()
        Me.lblImageTypeLabel = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.lblImageType = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnOK)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnCancel)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(45, 121)
        Me.FlowLayoutPanel1.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(192, 29)
        Me.FlowLayoutPanel1.TabIndex = 7
        Me.FlowLayoutPanel1.TabStop = True
        '
        'BtnOK
        '
        Me.BtnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Location = New System.Drawing.Point(3, 3)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(90, 23)
        Me.BtnOK.TabIndex = 0
        Me.BtnOK.Text = "{&Ok}"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(99, 3)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(90, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "{&Cancel}"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'CheckFormat
        '
        Me.CheckFormat.AutoSize = True
        Me.CheckFormat.Location = New System.Drawing.Point(15, 19)
        Me.CheckFormat.Name = "CheckFormat"
        Me.CheckFormat.Size = New System.Drawing.Size(90, 17)
        Me.CheckFormat.TabIndex = 0
        Me.CheckFormat.Text = "{Format Disk}"
        Me.CheckFormat.UseVisualStyleBackColor = True
        '
        'CheckVerify
        '
        Me.CheckVerify.AutoSize = True
        Me.CheckVerify.Location = New System.Drawing.Point(111, 19)
        Me.CheckVerify.Name = "CheckVerify"
        Me.CheckVerify.Size = New System.Drawing.Size(93, 17)
        Me.CheckVerify.TabIndex = 1
        Me.CheckVerify.Text = "{Verify Writes}"
        Me.CheckVerify.UseVisualStyleBackColor = True
        '
        'lblImageTypeLabel
        '
        Me.lblImageTypeLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblImageTypeLabel.AutoSize = True
        Me.lblImageTypeLabel.Location = New System.Drawing.Point(12, 16)
        Me.lblImageTypeLabel.Name = "lblImageTypeLabel"
        Me.lblImageTypeLabel.Size = New System.Drawing.Size(74, 13)
        Me.lblImageTypeLabel.TabIndex = 2
        Me.lblImageTypeLabel.Text = "{Image Type:}"
        Me.lblImageTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.CheckFormat)
        Me.GroupBox1.Controls.Add(Me.CheckVerify)
        Me.GroupBox1.Location = New System.Drawing.Point(40, 68)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(210, 47)
        Me.GroupBox1.TabIndex = 6
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "{Options}"
        '
        'lblImageType
        '
        Me.lblImageType.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblImageType.AutoSize = True
        Me.lblImageType.Location = New System.Drawing.Point(108, 16)
        Me.lblImageType.Name = "lblImageType"
        Me.lblImageType.Size = New System.Drawing.Size(68, 13)
        Me.lblImageType.TabIndex = 3
        Me.lblImageType.Text = "{ImageType}"
        Me.lblImageType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.lblImageTypeLabel)
        Me.GroupBox2.Controls.Add(Me.lblImageType)
        Me.GroupBox2.Location = New System.Drawing.Point(40, 12)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(210, 41)
        Me.GroupBox2.TabIndex = 8
        Me.GroupBox2.TabStop = False
        '
        'FloppyWriteOptionsForm
        '
        Me.AcceptButton = Me.BtnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CancelButton = Me.BtnCancel
        Me.ClientSize = New System.Drawing.Size(276, 159)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FloppyWriteOptionsForm"
        Me.Padding = New System.Windows.Forms.Padding(40, 12, 40, 6)
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents CheckFormat As CheckBox
    Friend WithEvents CheckVerify As CheckBox
    Friend WithEvents lblImageTypeLabel As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents lblImageType As Label
    Friend WithEvents GroupBox2 As GroupBox
End Class
