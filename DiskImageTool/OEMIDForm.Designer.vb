<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OEMIDForm
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtCurrentOEMID = New System.Windows.Forms.TextBox()
        Me.CboOEMID = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(33, 27)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(82, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Current OEM ID"
        '
        'txtCurrentOEMID
        '
        Me.txtCurrentOEMID.Location = New System.Drawing.Point(121, 24)
        Me.txtCurrentOEMID.MaxLength = 8
        Me.txtCurrentOEMID.Name = "txtCurrentOEMID"
        Me.txtCurrentOEMID.ReadOnly = True
        Me.txtCurrentOEMID.Size = New System.Drawing.Size(104, 20)
        Me.txtCurrentOEMID.TabIndex = 1
        '
        'CboOEMID
        '
        Me.CboOEMID.FormattingEnabled = True
        Me.CboOEMID.Location = New System.Drawing.Point(121, 50)
        Me.CboOEMID.MaxLength = 8
        Me.CboOEMID.Name = "CboOEMID"
        Me.CboOEMID.Size = New System.Drawing.Size(104, 21)
        Me.CboOEMID.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(33, 53)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(70, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "New OEM ID"
        '
        'BtnUpdate
        '
        Me.BtnUpdate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnUpdate.Location = New System.Drawing.Point(36, 82)
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.Size = New System.Drawing.Size(90, 23)
        Me.BtnUpdate.TabIndex = 4
        Me.BtnUpdate.Text = "Update"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(135, 82)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(90, 23)
        Me.BtnCancel.TabIndex = 5
        Me.BtnCancel.Text = "Cancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'OEMIDForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(259, 129)
        Me.Controls.Add(Me.BtnCancel)
        Me.Controls.Add(Me.BtnUpdate)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.CboOEMID)
        Me.Controls.Add(Me.txtCurrentOEMID)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "OEMIDForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Change OEM ID"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents txtCurrentOEMID As TextBox
    Friend WithEvents CboOEMID As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents BtnUpdate As Button
    Friend WithEvents BtnCancel As Button
End Class
