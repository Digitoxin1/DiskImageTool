<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UndeleteForm
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
        Me.FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.LabelFileName = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'FlowLayoutPanel2
        '
        Me.FlowLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.FlowLayoutPanel2.AutoSize = True
        Me.FlowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel2.Controls.Add(Me.BtnUpdate)
        Me.FlowLayoutPanel2.Controls.Add(Me.BtnCancel)
        Me.FlowLayoutPanel2.Location = New System.Drawing.Point(74, 70)
        Me.FlowLayoutPanel2.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        Me.FlowLayoutPanel2.Size = New System.Drawing.Size(192, 29)
        Me.FlowLayoutPanel2.TabIndex = 2
        '
        'BtnUpdate
        '
        Me.BtnUpdate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnUpdate.Location = New System.Drawing.Point(3, 3)
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.Size = New System.Drawing.Size(90, 23)
        Me.BtnUpdate.TabIndex = 0
        Me.BtnUpdate.Text = "&Update"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(99, 3)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(90, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "&Cancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 28)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(132, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Enter the first character for"
        Me.Label1.UseMnemonic = False
        '
        'TextBox1
        '
        Me.TextBox1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.TextBox1.Location = New System.Drawing.Point(280, 25)
        Me.TextBox1.MaxLength = 1
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ShortcutsEnabled = False
        Me.TextBox1.Size = New System.Drawing.Size(43, 20)
        Me.TextBox1.TabIndex = 1
        '
        'LabelFileName
        '
        Me.LabelFileName.AutoSize = True
        Me.LabelFileName.ForeColor = System.Drawing.Color.Blue
        Me.LabelFileName.Location = New System.Drawing.Point(147, 28)
        Me.LabelFileName.Name = "LabelFileName"
        Me.LabelFileName.Size = New System.Drawing.Size(60, 13)
        Me.LabelFileName.TabIndex = 3
        Me.LabelFileName.Text = "FILENAME"
        Me.LabelFileName.UseMnemonic = False
        '
        'UndeleteForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BtnCancel
        Me.ClientSize = New System.Drawing.Size(340, 108)
        Me.Controls.Add(Me.LabelFileName)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.FlowLayoutPanel2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UndeleteForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Undelete File"
        Me.FlowLayoutPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents FlowLayoutPanel2 As FlowLayoutPanel
    Friend WithEvents BtnUpdate As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents LabelFileName As Label
End Class
