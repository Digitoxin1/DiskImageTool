﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DeleteFileForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DeleteFileForm))
        Me.LblCaption = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.RadioFillKeep = New System.Windows.Forms.RadioButton()
        Me.RadioFill00 = New System.Windows.Forms.RadioButton()
        Me.RadioFillF6 = New System.Windows.Forms.RadioButton()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'LblCaption
        '
        resources.ApplyResources(Me.LblCaption, "LblCaption")
        Me.LblCaption.Name = "LblCaption"
        Me.LblCaption.UseMnemonic = False
        '
        'GroupBox1
        '
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Controls.Add(Me.RadioFillKeep)
        Me.GroupBox1.Controls.Add(Me.RadioFill00)
        Me.GroupBox1.Controls.Add(Me.RadioFillF6)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'RadioFillKeep
        '
        resources.ApplyResources(Me.RadioFillKeep, "RadioFillKeep")
        Me.RadioFillKeep.Checked = True
        Me.RadioFillKeep.Name = "RadioFillKeep"
        Me.RadioFillKeep.TabStop = True
        Me.RadioFillKeep.UseVisualStyleBackColor = True
        '
        'RadioFill00
        '
        resources.ApplyResources(Me.RadioFill00, "RadioFill00")
        Me.RadioFill00.Name = "RadioFill00"
        Me.RadioFill00.UseMnemonic = False
        Me.RadioFill00.UseVisualStyleBackColor = True
        '
        'RadioFillF6
        '
        resources.ApplyResources(Me.RadioFillF6, "RadioFillF6")
        Me.RadioFillF6.Name = "RadioFillF6"
        Me.RadioFillF6.UseMnemonic = False
        Me.RadioFillF6.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.Control
        Me.Panel1.Controls.Add(Me.BtnOK)
        Me.Panel1.Controls.Add(Me.BtnCancel)
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Name = "Panel1"
        '
        'BtnOK
        '
        resources.ApplyResources(Me.BtnOK, "BtnOK")
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'DeleteFileForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.CancelButton = Me.BtnCancel
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.LblCaption)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DeleteFileForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents LblCaption As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents RadioFill00 As RadioButton
    Friend WithEvents RadioFillF6 As RadioButton
    Friend WithEvents Panel1 As Panel
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents RadioFillKeep As RadioButton
End Class
