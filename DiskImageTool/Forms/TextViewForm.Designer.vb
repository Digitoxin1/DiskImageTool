<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TextViewForm
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
        Dim PanelMain As System.Windows.Forms.Panel
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.BtnSave = New System.Windows.Forms.Button()
        PanelMain = New System.Windows.Forms.Panel()
        PanelMain.SuspendLayout()
        Me.PanelBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelMain
        '
        PanelMain.Controls.Add(Me.TextBox1)
        PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        PanelMain.Location = New System.Drawing.Point(0, 0)
        PanelMain.Name = "PanelMain"
        PanelMain.Size = New System.Drawing.Size(769, 518)
        PanelMain.TabIndex = 0
        '
        'TextBox1
        '
        Me.TextBox1.AcceptsReturn = True
        Me.TextBox1.AcceptsTab = True
        Me.TextBox1.BackColor = System.Drawing.SystemColors.Window
        Me.TextBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBox1.Font = New System.Drawing.Font("Consolas", 11.25!)
        Me.TextBox1.Location = New System.Drawing.Point(0, 0)
        Me.TextBox1.MaxLength = 2949120
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox1.Size = New System.Drawing.Size(769, 518)
        Me.TextBox1.TabIndex = 0
        '
        'PanelBottom
        '
        Me.PanelBottom.BackColor = System.Drawing.SystemColors.Control
        Me.PanelBottom.Controls.Add(Me.BtnClose)
        Me.PanelBottom.Controls.Add(Me.BtnSave)
        Me.PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.PanelBottom.Location = New System.Drawing.Point(0, 518)
        Me.PanelBottom.Margin = New System.Windows.Forms.Padding(0)
        Me.PanelBottom.Name = "PanelBottom"
        Me.PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
        Me.PanelBottom.Size = New System.Drawing.Size(769, 43)
        Me.PanelBottom.TabIndex = 1
        '
        'BtnClose
        '
        Me.BtnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnClose.Location = New System.Drawing.Point(676, 10)
        Me.BtnClose.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(75, 23)
        Me.BtnClose.TabIndex = 1
        Me.BtnClose.Text = "{Close}"
        Me.BtnClose.UseVisualStyleBackColor = True
        '
        'BtnSave
        '
        Me.BtnSave.Location = New System.Drawing.Point(589, 10)
        Me.BtnSave.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnSave.Name = "BtnSave"
        Me.BtnSave.Size = New System.Drawing.Size(75, 23)
        Me.BtnSave.TabIndex = 0
        Me.BtnSave.Text = "{Save}"
        Me.BtnSave.UseVisualStyleBackColor = True
        '
        'TextViewForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(769, 561)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(Me.PanelBottom)
        Me.KeyPreview = True
        Me.Name = "TextViewForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        PanelMain.ResumeLayout(False)
        PanelMain.PerformLayout()
        Me.PanelBottom.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents BtnClose As Button
    Friend WithEvents BtnSave As Button
    Friend WithEvents PanelBottom As FlowLayoutPanel
End Class
