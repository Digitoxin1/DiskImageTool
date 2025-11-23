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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TextViewForm))
        Dim PanelMain As System.Windows.Forms.Panel
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.BtnSave = New System.Windows.Forms.Button()
        PanelMain = New System.Windows.Forms.Panel()
        Me.PanelBottom.SuspendLayout()
        PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'TextBox1
        '
        Me.TextBox1.AcceptsReturn = True
        Me.TextBox1.AcceptsTab = True
        Me.TextBox1.BackColor = System.Drawing.SystemColors.Window
        resources.ApplyResources(Me.TextBox1, "TextBox1")
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        '
        'PanelBottom
        '
        Me.PanelBottom.BackColor = System.Drawing.SystemColors.Control
        Me.PanelBottom.Controls.Add(Me.BtnClose)
        Me.PanelBottom.Controls.Add(Me.BtnSave)
        resources.ApplyResources(Me.PanelBottom, "PanelBottom")
        Me.PanelBottom.Name = "PanelBottom"
        '
        'BtnClose
        '
        Me.BtnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        resources.ApplyResources(Me.BtnClose, "BtnClose")
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.UseVisualStyleBackColor = True
        '
        'BtnSave
        '
        resources.ApplyResources(Me.BtnSave, "BtnSave")
        Me.BtnSave.Name = "BtnSave"
        Me.BtnSave.UseVisualStyleBackColor = True
        '
        'PanelMain
        '
        PanelMain.Controls.Add(Me.TextBox1)
        resources.ApplyResources(PanelMain, "PanelMain")
        PanelMain.Name = "PanelMain"
        '
        'TextViewForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(Me.PanelBottom)
        Me.KeyPreview = True
        Me.Name = "TextViewForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.PanelBottom.ResumeLayout(False)
        PanelMain.ResumeLayout(False)
        PanelMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents BtnClose As Button
    Friend WithEvents BtnSave As Button
    Friend WithEvents PanelBottom As FlowLayoutPanel
End Class
