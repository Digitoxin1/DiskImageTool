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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SaveAllForm))
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
        resources.ApplyResources(Me.FlowLayoutPanel1, "FlowLayoutPanel1")
        Me.FlowLayoutPanel1.BackColor = System.Drawing.SystemColors.Control
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnYes)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnNo)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnCancel)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnYesToAll)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnNoToall)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        '
        'BtnYes
        '
        resources.ApplyResources(Me.BtnYes, "BtnYes")
        Me.BtnYes.Name = "BtnYes"
        Me.BtnYes.UseVisualStyleBackColor = True
        '
        'BtnNo
        '
        resources.ApplyResources(Me.BtnNo, "BtnNo")
        Me.BtnNo.Name = "BtnNo"
        Me.BtnNo.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnYesToAll
        '
        resources.ApplyResources(Me.BtnYesToAll, "BtnYesToAll")
        Me.BtnYesToAll.Name = "BtnYesToAll"
        Me.BtnYesToAll.UseVisualStyleBackColor = True
        '
        'BtnNoToall
        '
        resources.ApplyResources(Me.BtnNoToall, "BtnNoToall")
        Me.BtnNoToall.Name = "BtnNoToall"
        Me.BtnNoToall.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.Control
        Me.Panel1.Controls.Add(Me.FlowLayoutPanel1)
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Name = "Panel1"
        '
        'LblCaption
        '
        resources.ApplyResources(Me.LblCaption, "LblCaption")
        Me.LblCaption.Name = "LblCaption"
        Me.LblCaption.UseMnemonic = False
        '
        'SaveAllForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.CancelButton = Me.BtnCancel
        Me.Controls.Add(Me.LblCaption)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SaveAllForm"
        Me.ShowIcon = False
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
