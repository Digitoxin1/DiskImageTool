<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SaveAllForm
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
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.BtnYes = New System.Windows.Forms.Button()
        Me.BtnNo = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnYesToAll = New System.Windows.Forms.Button()
        Me.BtnNoToall = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.LblCaption = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.BackColor = System.Drawing.SystemColors.Control
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnYes)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnNo)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnCancel)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnYesToAll)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnNoToall)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(28, 0)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(417, 42)
        Me.FlowLayoutPanel1.TabIndex = 2
        '
        'BtnYes
        '
        Me.BtnYes.AutoSize = True
        Me.BtnYes.Location = New System.Drawing.Point(4, 10)
        Me.BtnYes.Margin = New System.Windows.Forms.Padding(4, 10, 4, 9)
        Me.BtnYes.Name = "BtnYes"
        Me.BtnYes.Size = New System.Drawing.Size(75, 23)
        Me.BtnYes.TabIndex = 3
        Me.BtnYes.Text = "{&Yes}"
        Me.BtnYes.UseVisualStyleBackColor = True
        '
        'BtnNo
        '
        Me.BtnNo.AutoSize = True
        Me.BtnNo.Location = New System.Drawing.Point(87, 10)
        Me.BtnNo.Margin = New System.Windows.Forms.Padding(4, 10, 4, 9)
        Me.BtnNo.Name = "BtnNo"
        Me.BtnNo.Size = New System.Drawing.Size(75, 23)
        Me.BtnNo.TabIndex = 4
        Me.BtnNo.Text = "{&No}"
        Me.BtnNo.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSize = True
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(170, 10)
        Me.BtnCancel.Margin = New System.Windows.Forms.Padding(4, 10, 4, 9)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
        Me.BtnCancel.TabIndex = 0
        Me.BtnCancel.Text = "{&Cancel}"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnYesToAll
        '
        Me.BtnYesToAll.AutoSize = True
        Me.BtnYesToAll.Location = New System.Drawing.Point(253, 10)
        Me.BtnYesToAll.Margin = New System.Windows.Forms.Padding(4, 10, 4, 9)
        Me.BtnYesToAll.Name = "BtnYesToAll"
        Me.BtnYesToAll.Size = New System.Drawing.Size(75, 23)
        Me.BtnYesToAll.TabIndex = 1
        Me.BtnYesToAll.Text = "{Yes to &all}"
        Me.BtnYesToAll.UseVisualStyleBackColor = True
        '
        'BtnNoToall
        '
        Me.BtnNoToall.AutoSize = True
        Me.BtnNoToall.Location = New System.Drawing.Point(336, 10)
        Me.BtnNoToall.Margin = New System.Windows.Forms.Padding(4, 10, 4, 9)
        Me.BtnNoToall.Name = "BtnNoToall"
        Me.BtnNoToall.Size = New System.Drawing.Size(75, 23)
        Me.BtnNoToall.TabIndex = 2
        Me.BtnNoToall.Text = "{N&o to all}"
        Me.BtnNoToall.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.Control
        Me.Panel1.Controls.Add(Me.FlowLayoutPanel1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 80)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(461, 42)
        Me.Panel1.TabIndex = 1
        '
        'LblCaption
        '
        Me.LblCaption.AutoSize = True
        Me.LblCaption.Location = New System.Drawing.Point(61, 34)
        Me.LblCaption.Margin = New System.Windows.Forms.Padding(3, 0, 3, 75)
        Me.LblCaption.MaximumSize = New System.Drawing.Size(378, 0)
        Me.LblCaption.Name = "LblCaption"
        Me.LblCaption.Size = New System.Drawing.Size(51, 13)
        Me.LblCaption.TabIndex = 0
        Me.LblCaption.Text = "{Caption}"
        Me.LblCaption.UseMnemonic = False
        '
        'SaveAllForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.CancelButton = Me.BtnCancel
        Me.ClientSize = New System.Drawing.Size(461, 122)
        Me.Controls.Add(Me.LblCaption)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(477, 161)
        Me.Name = "SaveAllForm"
        Me.ShowIcon = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents BtnYes As Button
    Friend WithEvents BtnNo As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents BtnYesToAll As Button
    Friend WithEvents BtnNoToall As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents LblCaption As Label
End Class
