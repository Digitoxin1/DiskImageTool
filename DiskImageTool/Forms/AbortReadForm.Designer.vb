<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class AbortReadForm
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
        Me.BtnKeep = New System.Windows.Forms.Button()
        Me.BtnDiscard = New System.Windows.Forms.Button()
        Me.BtnContinue = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.LblCaption = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.BackColor = System.Drawing.SystemColors.Control
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnContinue)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnKeep)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnDiscard)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Padding = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(434, 42)
        Me.FlowLayoutPanel1.TabIndex = 2
        Me.FlowLayoutPanel1.WrapContents = False
        '
        'BtnKeep
        '
        Me.BtnKeep.AutoSize = True
        Me.BtnKeep.Location = New System.Drawing.Point(4, 10)
        Me.BtnKeep.Margin = New System.Windows.Forms.Padding(4, 10, 16, 9)
        Me.BtnKeep.MinimumSize = New System.Drawing.Size(75, 23)
        Me.BtnKeep.Name = "BtnKeep"
        Me.BtnKeep.Size = New System.Drawing.Size(75, 23)
        Me.BtnKeep.TabIndex = 1
        Me.BtnKeep.Text = "{&Keep}"
        Me.BtnKeep.UseVisualStyleBackColor = True
        '
        'BtnDiscard
        '
        Me.BtnDiscard.AutoSize = True
        Me.BtnDiscard.Location = New System.Drawing.Point(87, 10)
        Me.BtnDiscard.Margin = New System.Windows.Forms.Padding(4, 10, 4, 9)
        Me.BtnDiscard.MinimumSize = New System.Drawing.Size(75, 23)
        Me.BtnDiscard.Name = "BtnDiscard"
        Me.BtnDiscard.Size = New System.Drawing.Size(75, 23)
        Me.BtnDiscard.TabIndex = 2
        Me.BtnDiscard.Text = "{&Discard}"
        Me.BtnDiscard.UseVisualStyleBackColor = True
        '
        'BtnContinue
        '
        Me.BtnContinue.AutoSize = True
        Me.BtnContinue.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnContinue.Location = New System.Drawing.Point(170, 10)
        Me.BtnContinue.Margin = New System.Windows.Forms.Padding(4, 10, 4, 9)
        Me.BtnContinue.MinimumSize = New System.Drawing.Size(75, 23)
        Me.BtnContinue.Name = "BtnContinue"
        Me.BtnContinue.Size = New System.Drawing.Size(75, 23)
        Me.BtnContinue.TabIndex = 0
        Me.BtnContinue.Text = "{&Continue}"
        Me.BtnContinue.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.Control
        Me.Panel1.Controls.Add(Me.FlowLayoutPanel1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 80)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(434, 42)
        Me.Panel1.TabIndex = 1
        '
        'LblCaption
        '
        Me.LblCaption.AutoSize = True
        Me.LblCaption.Location = New System.Drawing.Point(61, 34)
        Me.LblCaption.Margin = New System.Windows.Forms.Padding(3, 0, 3, 75)
        Me.LblCaption.MaximumSize = New System.Drawing.Size(350, 0)
        Me.LblCaption.Name = "LblCaption"
        Me.LblCaption.Size = New System.Drawing.Size(51, 13)
        Me.LblCaption.TabIndex = 0
        Me.LblCaption.Text = "{Caption}"
        Me.LblCaption.UseMnemonic = False
        '
        'AbortReadForm
        '
        Me.AcceptButton = Me.BtnContinue
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.CancelButton = Me.BtnContinue
        Me.ClientSize = New System.Drawing.Size(434, 122)
        Me.Controls.Add(Me.LblCaption)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(450, 161)
        Me.Name = "AbortReadForm"
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
    Friend WithEvents BtnKeep As Button
    Friend WithEvents BtnDiscard As Button
    Friend WithEvents BtnContinue As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents LblCaption As Label
End Class
