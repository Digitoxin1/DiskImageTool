<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class VolumeSerialNumberForm
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
        Me.DTDate = New System.Windows.Forms.DateTimePicker()
        Me.LabelDate = New System.Windows.Forms.Label()
        Me.DTTime = New System.Windows.Forms.DateTimePicker()
        Me.LabelMilliseconds = New System.Windows.Forms.Label()
        Me.NumMS = New System.Windows.Forms.NumericUpDown()
        Me.LabelTime = New System.Windows.Forms.Label()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnOK = New System.Windows.Forms.Button()
        TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        PanelMain = New System.Windows.Forms.Panel()
        TableLayoutPanel1.SuspendLayout()
        CType(Me.NumMS, System.ComponentModel.ISupportInitialize).BeginInit()
        PanelBottom.SuspendLayout()
        PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        TableLayoutPanel1.AutoSize = True
        TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        TableLayoutPanel1.ColumnCount = 3
        TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        TableLayoutPanel1.Controls.Add(Me.DTDate, 0, 1)
        TableLayoutPanel1.Controls.Add(Me.LabelDate, 0, 0)
        TableLayoutPanel1.Controls.Add(Me.DTTime, 1, 1)
        TableLayoutPanel1.Controls.Add(Me.LabelMilliseconds, 2, 0)
        TableLayoutPanel1.Controls.Add(Me.NumMS, 2, 1)
        TableLayoutPanel1.Controls.Add(Me.LabelTime, 1, 0)
        TableLayoutPanel1.Location = New System.Drawing.Point(18, 18)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 2
        TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        TableLayoutPanel1.Size = New System.Drawing.Size(284, 39)
        TableLayoutPanel1.TabIndex = 0
        '
        'DTDate
        '
        Me.DTDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTDate.Location = New System.Drawing.Point(3, 16)
        Me.DTDate.MaxDate = New Date(2107, 12, 31, 0, 0, 0, 0)
        Me.DTDate.MinDate = New Date(1980, 1, 1, 0, 0, 0, 0)
        Me.DTDate.Name = "DTDate"
        Me.DTDate.Size = New System.Drawing.Size(97, 20)
        Me.DTDate.TabIndex = 3
        '
        'LabelDate
        '
        Me.LabelDate.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LabelDate.AutoSize = True
        Me.LabelDate.Location = New System.Drawing.Point(3, 0)
        Me.LabelDate.Name = "LabelDate"
        Me.LabelDate.Size = New System.Drawing.Size(38, 13)
        Me.LabelDate.TabIndex = 0
        Me.LabelDate.Text = "{Date}"
        '
        'DTTime
        '
        Me.DTTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.DTTime.Location = New System.Drawing.Point(106, 16)
        Me.DTTime.MaxDate = New Date(2107, 12, 31, 0, 0, 0, 0)
        Me.DTTime.MinDate = New Date(1980, 1, 1, 0, 0, 0, 0)
        Me.DTTime.Name = "DTTime"
        Me.DTTime.ShowUpDown = True
        Me.DTTime.Size = New System.Drawing.Size(97, 20)
        Me.DTTime.TabIndex = 4
        '
        'LabelMilliseconds
        '
        Me.LabelMilliseconds.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LabelMilliseconds.AutoSize = True
        Me.LabelMilliseconds.Location = New System.Drawing.Point(209, 0)
        Me.LabelMilliseconds.Name = "LabelMilliseconds"
        Me.LabelMilliseconds.Size = New System.Drawing.Size(72, 13)
        Me.LabelMilliseconds.TabIndex = 2
        Me.LabelMilliseconds.Text = "{Milliseconds}"
        '
        'NumMS
        '
        Me.NumMS.Increment = New Decimal(New Integer() {10, 0, 0, 0})
        Me.NumMS.Location = New System.Drawing.Point(209, 16)
        Me.NumMS.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        Me.NumMS.Name = "NumMS"
        Me.NumMS.Size = New System.Drawing.Size(64, 20)
        Me.NumMS.TabIndex = 5
        '
        'LabelTime
        '
        Me.LabelTime.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LabelTime.AutoSize = True
        Me.LabelTime.Location = New System.Drawing.Point(106, 0)
        Me.LabelTime.Name = "LabelTime"
        Me.LabelTime.Size = New System.Drawing.Size(38, 13)
        Me.LabelTime.TabIndex = 1
        Me.LabelTime.Text = "{Time}"
        '
        'PanelBottom
        '
        PanelBottom.Controls.Add(Me.BtnCancel)
        PanelBottom.Controls.Add(Me.BtnOK)
        PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        PanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        PanelBottom.Location = New System.Drawing.Point(0, 66)
        PanelBottom.Name = "PanelBottom"
        PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
        PanelBottom.Size = New System.Drawing.Size(315, 43)
        PanelBottom.TabIndex = 1
        PanelBottom.WrapContents = False
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(222, 10)
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
        Me.BtnOK.Location = New System.Drawing.Point(135, 10)
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
        PanelMain.Size = New System.Drawing.Size(315, 66)
        PanelMain.TabIndex = 0
        '
        'VolumeSerialNumberForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(315, 109)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "VolumeSerialNumberForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        TableLayoutPanel1.ResumeLayout(False)
        TableLayoutPanel1.PerformLayout()
        CType(Me.NumMS, System.ComponentModel.ISupportInitialize).EndInit()
        PanelBottom.ResumeLayout(False)
        PanelMain.ResumeLayout(False)
        PanelMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents NumMS As NumericUpDown
    Friend WithEvents DTDate As DateTimePicker
    Friend WithEvents DTTime As DateTimePicker
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents LabelDate As Label
    Friend WithEvents LabelMilliseconds As Label
    Friend WithEvents LabelTime As Label
End Class
