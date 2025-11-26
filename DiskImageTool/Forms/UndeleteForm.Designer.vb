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
        Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
        Dim PanelMain As System.Windows.Forms.Panel
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.FlowLayoutPanelMain = New System.Windows.Forms.FlowLayoutPanel()
        Me.LabelCaption = New System.Windows.Forms.Label()
        Me.LabelFileName = New System.Windows.Forms.Label()
        Me.TextBoxChar = New System.Windows.Forms.TextBox()
        PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        PanelMain = New System.Windows.Forms.Panel()
        PanelBottom.SuspendLayout()
        PanelMain.SuspendLayout()
        Me.FlowLayoutPanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelBottom
        '
        PanelBottom.BackColor = System.Drawing.SystemColors.Control
        PanelBottom.Controls.Add(Me.BtnCancel)
        PanelBottom.Controls.Add(Me.BtnUpdate)
        PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        PanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        PanelBottom.Location = New System.Drawing.Point(0, 47)
        PanelBottom.Name = "PanelBottom"
        PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
        PanelBottom.Size = New System.Drawing.Size(350, 43)
        PanelBottom.TabIndex = 1
        PanelBottom.WrapContents = False
        '
        'BtnCancel
        '
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(257, 10)
        Me.BtnCancel.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "{&Cancel}"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnUpdate
        '
        Me.BtnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnUpdate.Location = New System.Drawing.Point(170, 10)
        Me.BtnUpdate.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.Size = New System.Drawing.Size(75, 23)
        Me.BtnUpdate.TabIndex = 0
        Me.BtnUpdate.Text = "{&Update}"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'PanelMain
        '
        PanelMain.AutoSize = True
        PanelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        PanelMain.BackColor = System.Drawing.SystemColors.Window
        PanelMain.Controls.Add(Me.FlowLayoutPanelMain)
        PanelMain.Controls.Add(Me.TextBoxChar)
        PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        PanelMain.Location = New System.Drawing.Point(0, 0)
        PanelMain.Name = "PanelMain"
        PanelMain.Padding = New System.Windows.Forms.Padding(18, 18, 18, 6)
        PanelMain.Size = New System.Drawing.Size(350, 47)
        PanelMain.TabIndex = 0
        '
        'FlowLayoutPanelMain
        '
        Me.FlowLayoutPanelMain.Controls.Add(Me.LabelCaption)
        Me.FlowLayoutPanelMain.Controls.Add(Me.LabelFileName)
        Me.FlowLayoutPanelMain.Location = New System.Drawing.Point(18, 18)
        Me.FlowLayoutPanelMain.Name = "FlowLayoutPanelMain"
        Me.FlowLayoutPanelMain.Size = New System.Drawing.Size(262, 20)
        Me.FlowLayoutPanelMain.TabIndex = 0
        Me.FlowLayoutPanelMain.WrapContents = False
        '
        'LabelCaption
        '
        Me.LabelCaption.AutoSize = True
        Me.LabelCaption.Location = New System.Drawing.Point(0, 3)
        Me.LabelCaption.Margin = New System.Windows.Forms.Padding(0, 3, 3, 0)
        Me.LabelCaption.Name = "LabelCaption"
        Me.LabelCaption.Size = New System.Drawing.Size(51, 13)
        Me.LabelCaption.TabIndex = 0
        Me.LabelCaption.Text = "{Caption}"
        Me.LabelCaption.UseMnemonic = False
        '
        'LabelFileName
        '
        Me.LabelFileName.AutoSize = True
        Me.LabelFileName.ForeColor = System.Drawing.Color.Blue
        Me.LabelFileName.Location = New System.Drawing.Point(54, 3)
        Me.LabelFileName.Margin = New System.Windows.Forms.Padding(0, 3, 3, 0)
        Me.LabelFileName.Name = "LabelFileName"
        Me.LabelFileName.Size = New System.Drawing.Size(59, 13)
        Me.LabelFileName.TabIndex = 1
        Me.LabelFileName.Text = "{FileName}"
        Me.LabelFileName.UseMnemonic = False
        '
        'TextBoxChar
        '
        Me.TextBoxChar.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.TextBoxChar.Location = New System.Drawing.Point(286, 18)
        Me.TextBoxChar.MaxLength = 1
        Me.TextBoxChar.Name = "TextBoxChar"
        Me.TextBoxChar.ShortcutsEnabled = False
        Me.TextBoxChar.Size = New System.Drawing.Size(43, 20)
        Me.TextBoxChar.TabIndex = 1
        '
        'UndeleteForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.CancelButton = Me.BtnCancel
        Me.ClientSize = New System.Drawing.Size(350, 90)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UndeleteForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        PanelBottom.ResumeLayout(False)
        PanelMain.ResumeLayout(False)
        PanelMain.PerformLayout()
        Me.FlowLayoutPanelMain.ResumeLayout(False)
        Me.FlowLayoutPanelMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BtnUpdate As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents TextBoxChar As TextBox
    Friend WithEvents FlowLayoutPanelMain As FlowLayoutPanel
    Friend WithEvents LabelCaption As Label
    Friend WithEvents LabelFileName As Label
End Class
