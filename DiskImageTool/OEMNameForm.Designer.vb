<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OEMNameForm
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
        Me.TxtCurrentOEMName = New System.Windows.Forms.TextBox()
        Me.CboOEMName = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TxtCurrentOEMHex = New System.Windows.Forms.TextBox()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.MskOEMNameHex = New DiskImageTool.HexTextBox()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 6)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(99, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Current OEM Name"
        '
        'TxtCurrentOEMName
        '
        Me.TxtCurrentOEMName.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtCurrentOEMName.Location = New System.Drawing.Point(108, 3)
        Me.TxtCurrentOEMName.MaxLength = 8
        Me.TxtCurrentOEMName.Name = "TxtCurrentOEMName"
        Me.TxtCurrentOEMName.ReadOnly = True
        Me.TxtCurrentOEMName.Size = New System.Drawing.Size(104, 20)
        Me.TxtCurrentOEMName.TabIndex = 1
        '
        'CboOEMName
        '
        Me.CboOEMName.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.CboOEMName.FormattingEnabled = True
        Me.CboOEMName.Location = New System.Drawing.Point(108, 29)
        Me.CboOEMName.MaxLength = 8
        Me.CboOEMName.Name = "CboOEMName"
        Me.CboOEMName.Size = New System.Drawing.Size(104, 21)
        Me.CboOEMName.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(3, 33)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(87, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "New OEM Name"
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
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.MskOEMNameHex, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.TxtCurrentOEMHex, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label2, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.TxtCurrentOEMName, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.CboOEMName, 1, 1)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(21, 21)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(391, 53)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'TxtCurrentOEMHex
        '
        Me.TxtCurrentOEMHex.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtCurrentOEMHex.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCurrentOEMHex.Location = New System.Drawing.Point(218, 3)
        Me.TxtCurrentOEMHex.MaxLength = 8
        Me.TxtCurrentOEMHex.Name = "TxtCurrentOEMHex"
        Me.TxtCurrentOEMHex.ReadOnly = True
        Me.TxtCurrentOEMHex.Size = New System.Drawing.Size(170, 20)
        Me.TxtCurrentOEMHex.TabIndex = 2
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnUpdate)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnCancel)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(125, 80)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(192, 29)
        Me.FlowLayoutPanel1.TabIndex = 1
        '
        'MskOEMNameHex
        '
        Me.MskOEMNameHex.CutCopyMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals
        Me.MskOEMNameHex.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MskOEMNameHex.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite
        Me.MskOEMNameHex.Location = New System.Drawing.Point(218, 29)
        Me.MskOEMNameHex.Mask = "AA AA AA AA AA AA AA AA"
        Me.MskOEMNameHex.MaskLength = 8
        Me.MskOEMNameHex.Name = "MskOEMNameHex"
        Me.MskOEMNameHex.PromptChar = Global.Microsoft.VisualBasic.ChrW(45)
        Me.MskOEMNameHex.ShortcutsEnabled = False
        Me.MskOEMNameHex.Size = New System.Drawing.Size(170, 20)
        Me.MskOEMNameHex.TabIndex = 5
        Me.MskOEMNameHex.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals
        '
        'OEMNameForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ClientSize = New System.Drawing.Size(440, 151)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "OEMNameForm"
        Me.Padding = New System.Windows.Forms.Padding(18)
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Change OEM Name"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents TxtCurrentOEMName As TextBox
    Friend WithEvents CboOEMName As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents BtnUpdate As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents TxtCurrentOEMHex As TextBox
    Friend WithEvents MskOEMNameHex As HexTextBox
End Class
