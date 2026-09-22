<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class InsertBytesForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
        Dim PanelMain As System.Windows.Forms.Panel
        Me.RadBtn4E = New System.Windows.Forms.RadioButton()
        Me.RadBtn00 = New System.Windows.Forms.RadioButton()
        Me.RadBtnOther = New System.Windows.Forms.RadioButton()
        Me.TextOther = New System.Windows.Forms.TextBox()
        Me.LabelCount = New System.Windows.Forms.Label()
        Me.NumCount = New System.Windows.Forms.NumericUpDown()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        PanelMain = New System.Windows.Forms.Panel()
        TableLayoutPanel1.SuspendLayout()
        CType(Me.NumCount, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.FlowLayoutPanel1.SuspendLayout()
        PanelBottom.SuspendLayout()
        PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        TableLayoutPanel1.AutoSize = True
        TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        TableLayoutPanel1.ColumnCount = 2
        TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        TableLayoutPanel1.Controls.Add(Me.FlowLayoutPanel1, 0, 0)
        TableLayoutPanel1.Controls.Add(Me.LabelCount, 0, 1)
        TableLayoutPanel1.Controls.Add(Me.NumCount, 1, 1)
        TableLayoutPanel1.Location = New System.Drawing.Point(18, 18)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 2
        TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        TableLayoutPanel1.Size = New System.Drawing.Size(250, 53)
        TableLayoutPanel1.TabIndex = 0
        TableLayoutPanel1.SetColumnSpan(Me.FlowLayoutPanel1, 2)
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel1.Controls.Add(Me.RadBtn4E)
        Me.FlowLayoutPanel1.Controls.Add(Me.RadBtn00)
        Me.FlowLayoutPanel1.Controls.Add(Me.RadBtnOther)
        Me.FlowLayoutPanel1.Controls.Add(Me.TextOther)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.FlowLayoutPanel1.Margin = New System.Windows.Forms.Padding(0, 0, 0, 6)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(234, 26)
        Me.FlowLayoutPanel1.TabIndex = 0
        Me.FlowLayoutPanel1.WrapContents = False
        '
        'RadBtn4E
        '
        Me.RadBtn4E.AutoSize = True
        Me.RadBtn4E.Location = New System.Drawing.Point(3, 3)
        Me.RadBtn4E.Name = "RadBtn4E"
        Me.RadBtn4E.Size = New System.Drawing.Size(40, 17)
        Me.RadBtn4E.TabIndex = 0
        Me.RadBtn4E.TabStop = True
        Me.RadBtn4E.Text = "4E"
        Me.RadBtn4E.UseMnemonic = False
        Me.RadBtn4E.UseVisualStyleBackColor = True
        '
        'RadBtn00
        '
        Me.RadBtn00.AutoSize = True
        Me.RadBtn00.Location = New System.Drawing.Point(49, 3)
        Me.RadBtn00.Name = "RadBtn00"
        Me.RadBtn00.Size = New System.Drawing.Size(40, 17)
        Me.RadBtn00.TabIndex = 1
        Me.RadBtn00.TabStop = True
        Me.RadBtn00.Text = "00"
        Me.RadBtn00.UseMnemonic = False
        Me.RadBtn00.UseVisualStyleBackColor = True
        '
        'RadBtnOther
        '
        Me.RadBtnOther.AutoSize = True
        Me.RadBtnOther.Location = New System.Drawing.Point(95, 3)
        Me.RadBtnOther.Name = "RadBtnOther"
        Me.RadBtnOther.Size = New System.Drawing.Size(54, 17)
        Me.RadBtnOther.TabIndex = 2
        Me.RadBtnOther.TabStop = True
        Me.RadBtnOther.Text = "{Other}"
        Me.RadBtnOther.UseVisualStyleBackColor = True
        '
        'TextOther
        '
        Me.TextOther.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.TextOther.Enabled = False
        Me.TextOther.Location = New System.Drawing.Point(155, 3)
        Me.TextOther.MaxLength = 2
        Me.TextOther.Name = "TextOther"
        Me.TextOther.Size = New System.Drawing.Size(32, 20)
        Me.TextOther.TabIndex = 3
        '
        'LabelCount
        '
        Me.LabelCount.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LabelCount.AutoSize = True
        Me.LabelCount.Location = New System.Drawing.Point(3, 33)
        Me.LabelCount.Name = "LabelCount"
        Me.LabelCount.Size = New System.Drawing.Size(37, 13)
        Me.LabelCount.TabIndex = 1
        Me.LabelCount.Text = "{Bytes}"
        '
        'NumCount
        '
        Me.NumCount.Location = New System.Drawing.Point(46, 32)
        Me.NumCount.Maximum = New Decimal(New Integer() {65535, 0, 0, 0})
        Me.NumCount.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NumCount.Name = "NumCount"
        Me.NumCount.Size = New System.Drawing.Size(80, 20)
        Me.NumCount.TabIndex = 2
        Me.NumCount.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'PanelBottom
        '
        PanelBottom.Controls.Add(Me.BtnCancel)
        PanelBottom.Controls.Add(Me.BtnOK)
        PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        PanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        PanelBottom.Location = New System.Drawing.Point(0, 80)
        PanelBottom.Name = "PanelBottom"
        PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
        PanelBottom.Size = New System.Drawing.Size(314, 43)
        PanelBottom.TabIndex = 1
        PanelBottom.WrapContents = False
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(221, 10)
        Me.BtnCancel.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "{&Cancel}"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnOK
        '
        Me.BtnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Location = New System.Drawing.Point(134, 10)
        Me.BtnOK.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(75, 23)
        Me.BtnOK.TabIndex = 0
        Me.BtnOK.Text = "{&Ok}"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'PanelMain
        '
        PanelMain.AutoSize = True
        PanelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        PanelMain.BackColor = System.Drawing.SystemColors.Window
        PanelMain.Controls.Add(TableLayoutPanel1)
        PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        PanelMain.Location = New System.Drawing.Point(0, 0)
        PanelMain.Name = "PanelMain"
        PanelMain.Padding = New System.Windows.Forms.Padding(18, 18, 18, 6)
        PanelMain.Size = New System.Drawing.Size(314, 80)
        PanelMain.TabIndex = 0
        '
        'InsertBytesForm
        '
        Me.AcceptButton = Me.BtnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BtnCancel
        Me.ClientSize = New System.Drawing.Size(314, 123)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "InsertBytesForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        TableLayoutPanel1.ResumeLayout(False)
        TableLayoutPanel1.PerformLayout()
        CType(Me.NumCount, System.ComponentModel.ISupportInitialize).EndInit()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        PanelBottom.ResumeLayout(False)
        PanelMain.ResumeLayout(False)
        PanelMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents RadBtn4E As RadioButton
    Friend WithEvents RadBtn00 As RadioButton
    Friend WithEvents RadBtnOther As RadioButton
    Friend WithEvents TextOther As TextBox
    Friend WithEvents LabelCount As Label
    Friend WithEvents NumCount As NumericUpDown
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
End Class
