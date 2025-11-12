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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UndeleteForm))
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
        resources.ApplyResources(PanelBottom, "PanelBottom")
        PanelBottom.Name = "PanelBottom"
        '
        'BtnCancel
        '
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnUpdate
        '
        Me.BtnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK
        resources.ApplyResources(Me.BtnUpdate, "BtnUpdate")
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'PanelMain
        '
        resources.ApplyResources(PanelMain, "PanelMain")
        PanelMain.BackColor = System.Drawing.SystemColors.Window
        PanelMain.Controls.Add(Me.FlowLayoutPanelMain)
        PanelMain.Controls.Add(Me.TextBoxChar)
        PanelMain.Name = "PanelMain"
        '
        'FlowLayoutPanelMain
        '
        Me.FlowLayoutPanelMain.Controls.Add(Me.LabelCaption)
        Me.FlowLayoutPanelMain.Controls.Add(Me.LabelFileName)
        resources.ApplyResources(Me.FlowLayoutPanelMain, "FlowLayoutPanelMain")
        Me.FlowLayoutPanelMain.Name = "FlowLayoutPanelMain"
        '
        'LabelCaption
        '
        resources.ApplyResources(Me.LabelCaption, "LabelCaption")
        Me.LabelCaption.Name = "LabelCaption"
        Me.LabelCaption.UseMnemonic = False
        '
        'LabelFileName
        '
        resources.ApplyResources(Me.LabelFileName, "LabelFileName")
        Me.LabelFileName.ForeColor = System.Drawing.Color.Blue
        Me.LabelFileName.Name = "LabelFileName"
        Me.LabelFileName.UseMnemonic = False
        '
        'TextBoxChar
        '
        Me.TextBoxChar.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        resources.ApplyResources(Me.TextBoxChar, "TextBoxChar")
        Me.TextBoxChar.Name = "TextBoxChar"
        Me.TextBoxChar.ShortcutsEnabled = False
        '
        'UndeleteForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.CancelButton = Me.BtnCancel
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UndeleteForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
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
