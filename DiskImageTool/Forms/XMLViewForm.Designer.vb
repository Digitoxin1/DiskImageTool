<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class XMLViewForm
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
        Me.TreeView1 = New System.Windows.Forms.TreeView()
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
        PanelMain.Controls.Add(Me.TreeView1)
        PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        PanelMain.Location = New System.Drawing.Point(0, 0)
        PanelMain.Name = "PanelMain"
        PanelMain.Size = New System.Drawing.Size(769, 518)
        PanelMain.TabIndex = 3
        '
        'TreeView1
        '
        Me.TreeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TreeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText
        Me.TreeView1.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TreeView1.FullRowSelect = True
        Me.TreeView1.HideSelection = False
        Me.TreeView1.Location = New System.Drawing.Point(0, 0)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.ShowNodeToolTips = True
        Me.TreeView1.Size = New System.Drawing.Size(769, 518)
        Me.TreeView1.TabIndex = 0
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
        Me.PanelBottom.TabIndex = 2
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
        'XMLViewForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(769, 561)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(Me.PanelBottom)
        Me.KeyPreview = True
        Me.Name = "XMLViewForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        PanelMain.ResumeLayout(False)
        Me.PanelBottom.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelBottom As FlowLayoutPanel
    Friend WithEvents BtnClose As Button
    Friend WithEvents BtnSave As Button
    Friend WithEvents TreeView1 As TreeView
End Class
