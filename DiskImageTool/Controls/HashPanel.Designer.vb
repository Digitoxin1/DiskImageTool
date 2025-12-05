<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class HashPanel
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.FlowLayoutPanelHashes = New System.Windows.Forms.FlowLayoutPanel()
        Me.LabelCRC32Caption = New System.Windows.Forms.Label()
        Me.LabelCRC32 = New System.Windows.Forms.Label()
        Me.LabelMD5Caption = New System.Windows.Forms.Label()
        Me.LabelMD5 = New System.Windows.Forms.Label()
        Me.LabelSHA1Caption = New System.Windows.Forms.Label()
        Me.LabelSHA1 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanelHashes.SuspendLayout()
        Me.SuspendLayout()
        '
        'FlowLayoutPanelHashes
        '
        Me.FlowLayoutPanelHashes.AllowDrop = True
        Me.FlowLayoutPanelHashes.BackColor = System.Drawing.SystemColors.Window
        Me.FlowLayoutPanelHashes.Controls.Add(Me.LabelCRC32Caption)
        Me.FlowLayoutPanelHashes.Controls.Add(Me.LabelCRC32)
        Me.FlowLayoutPanelHashes.Controls.Add(Me.LabelMD5Caption)
        Me.FlowLayoutPanelHashes.Controls.Add(Me.LabelMD5)
        Me.FlowLayoutPanelHashes.Controls.Add(Me.LabelSHA1Caption)
        Me.FlowLayoutPanelHashes.Controls.Add(Me.LabelSHA1)
        Me.FlowLayoutPanelHashes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanelHashes.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.FlowLayoutPanelHashes.Location = New System.Drawing.Point(0, 0)
        Me.FlowLayoutPanelHashes.Name = "FlowLayoutPanelHashes"
        Me.FlowLayoutPanelHashes.Size = New System.Drawing.Size(275, 170)
        Me.FlowLayoutPanelHashes.TabIndex = 0
        Me.FlowLayoutPanelHashes.WrapContents = False
        '
        'LabelCRC32Caption
        '
        Me.LabelCRC32Caption.AutoSize = True
        Me.LabelCRC32Caption.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelCRC32Caption.Location = New System.Drawing.Point(3, 0)
        Me.LabelCRC32Caption.Name = "LabelCRC32Caption"
        Me.LabelCRC32Caption.Size = New System.Drawing.Size(41, 13)
        Me.LabelCRC32Caption.TabIndex = 0
        Me.LabelCRC32Caption.Text = "CRC32"
        Me.LabelCRC32Caption.UseMnemonic = False
        '
        'LabelCRC32
        '
        Me.LabelCRC32.AutoSize = True
        Me.LabelCRC32.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelCRC32.Location = New System.Drawing.Point(3, 13)
        Me.LabelCRC32.Name = "LabelCRC32"
        Me.LabelCRC32.Size = New System.Drawing.Size(49, 13)
        Me.LabelCRC32.TabIndex = 1
        Me.LabelCRC32.Text = "{CRC32}"
        Me.LabelCRC32.UseMnemonic = False
        '
        'LabelMD5Caption
        '
        Me.LabelMD5Caption.AutoSize = True
        Me.LabelMD5Caption.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelMD5Caption.Location = New System.Drawing.Point(3, 26)
        Me.LabelMD5Caption.Name = "LabelMD5Caption"
        Me.LabelMD5Caption.Padding = New System.Windows.Forms.Padding(0, 4, 0, 0)
        Me.LabelMD5Caption.Size = New System.Drawing.Size(30, 17)
        Me.LabelMD5Caption.TabIndex = 2
        Me.LabelMD5Caption.Text = "MD5"
        Me.LabelMD5Caption.UseMnemonic = False
        '
        'LabelMD5
        '
        Me.LabelMD5.AutoSize = True
        Me.LabelMD5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelMD5.Location = New System.Drawing.Point(3, 43)
        Me.LabelMD5.Name = "LabelMD5"
        Me.LabelMD5.Size = New System.Drawing.Size(38, 13)
        Me.LabelMD5.TabIndex = 3
        Me.LabelMD5.Text = "{MD5}"
        Me.LabelMD5.UseMnemonic = False
        '
        'LabelSHA1Caption
        '
        Me.LabelSHA1Caption.AutoSize = True
        Me.LabelSHA1Caption.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelSHA1Caption.Location = New System.Drawing.Point(3, 56)
        Me.LabelSHA1Caption.Name = "LabelSHA1Caption"
        Me.LabelSHA1Caption.Padding = New System.Windows.Forms.Padding(0, 4, 0, 0)
        Me.LabelSHA1Caption.Size = New System.Drawing.Size(38, 17)
        Me.LabelSHA1Caption.TabIndex = 4
        Me.LabelSHA1Caption.Text = "SHA-1"
        Me.LabelSHA1Caption.UseMnemonic = False
        '
        'LabelSHA1
        '
        Me.LabelSHA1.AutoSize = True
        Me.LabelSHA1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelSHA1.Location = New System.Drawing.Point(3, 73)
        Me.LabelSHA1.Name = "LabelSHA1"
        Me.LabelSHA1.Size = New System.Drawing.Size(43, 13)
        Me.LabelSHA1.TabIndex = 5
        Me.LabelSHA1.Text = "{SHA1}"
        Me.LabelSHA1.UseMnemonic = False
        '
        'HashPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.FlowLayoutPanelHashes)
        Me.Name = "HashPanel"
        Me.Size = New System.Drawing.Size(275, 170)
        Me.FlowLayoutPanelHashes.ResumeLayout(False)
        Me.FlowLayoutPanelHashes.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents FlowLayoutPanelHashes As FlowLayoutPanel
    Friend WithEvents LabelCRC32Caption As Label
    Friend WithEvents LabelCRC32 As Label
    Friend WithEvents LabelMD5Caption As Label
    Friend WithEvents LabelMD5 As Label
    Friend WithEvents LabelSHA1Caption As Label
    Friend WithEvents LabelSHA1 As Label
End Class
