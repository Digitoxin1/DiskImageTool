<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class VolumeSerialNumberForm
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
        Dim TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Dim Label1 As System.Windows.Forms.Label
        Dim Label3 As System.Windows.Forms.Label
        Dim Label2 As System.Windows.Forms.Label
        Dim FlowLayoutPanel2 As System.Windows.Forms.FlowLayoutPanel
        Dim FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
        Me.DTDate = New System.Windows.Forms.DateTimePicker()
        Me.DTTime = New System.Windows.Forms.DateTimePicker()
        Me.NumMS = New System.Windows.Forms.NumericUpDown()
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Label1 = New System.Windows.Forms.Label()
        Label3 = New System.Windows.Forms.Label()
        Label2 = New System.Windows.Forms.Label()
        FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        TableLayoutPanel1.SuspendLayout()
        CType(Me.NumMS, System.ComponentModel.ISupportInitialize).BeginInit()
        FlowLayoutPanel2.SuspendLayout()
        FlowLayoutPanel1.SuspendLayout()
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
        TableLayoutPanel1.Controls.Add(Label1, 0, 0)
        TableLayoutPanel1.Controls.Add(Me.DTTime, 1, 1)
        TableLayoutPanel1.Controls.Add(Label3, 2, 0)
        TableLayoutPanel1.Controls.Add(Me.NumMS, 2, 1)
        TableLayoutPanel1.Controls.Add(Label2, 1, 0)
        TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 2
        TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        TableLayoutPanel1.Size = New System.Drawing.Size(276, 39)
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
        'Label1
        '
        Label1.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label1.AutoSize = True
        Label1.Location = New System.Drawing.Point(3, 0)
        Label1.Name = "Label1"
        Label1.Size = New System.Drawing.Size(30, 13)
        Label1.TabIndex = 0
        Label1.Text = "Date"
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
        'Label3
        '
        Label3.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label3.AutoSize = True
        Label3.Location = New System.Drawing.Point(209, 0)
        Label3.Name = "Label3"
        Label3.Size = New System.Drawing.Size(64, 13)
        Label3.TabIndex = 2
        Label3.Text = "Milliseconds"
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
        'Label2
        '
        Label2.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label2.AutoSize = True
        Label2.Location = New System.Drawing.Point(106, 0)
        Label2.Name = "Label2"
        Label2.Size = New System.Drawing.Size(30, 13)
        Label2.TabIndex = 1
        Label2.Text = "Time"
        '
        'FlowLayoutPanel2
        '
        FlowLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Top
        FlowLayoutPanel2.AutoSize = True
        FlowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        FlowLayoutPanel2.Controls.Add(Me.BtnOK)
        FlowLayoutPanel2.Controls.Add(Me.BtnCancel)
        FlowLayoutPanel2.Location = New System.Drawing.Point(45, 48)
        FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        FlowLayoutPanel2.Size = New System.Drawing.Size(192, 29)
        FlowLayoutPanel2.TabIndex = 1
        '
        'BtnOK
        '
        Me.BtnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Location = New System.Drawing.Point(3, 3)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(90, 23)
        Me.BtnOK.TabIndex = 0
        Me.BtnOK.Text = "&OK"
        Me.BtnOK.UseVisualStyleBackColor = True
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
        'FlowLayoutPanel1
        '
        FlowLayoutPanel1.AutoSize = True
        FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        FlowLayoutPanel1.Controls.Add(TableLayoutPanel1)
        FlowLayoutPanel1.Controls.Add(FlowLayoutPanel2)
        FlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        FlowLayoutPanel1.Location = New System.Drawing.Point(21, 21)
        FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        FlowLayoutPanel1.Size = New System.Drawing.Size(282, 80)
        FlowLayoutPanel1.TabIndex = 0
        '
        'VolumeSerialNumberForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ClientSize = New System.Drawing.Size(362, 145)
        Me.Controls.Add(FlowLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "VolumeSerialNumberForm"
        Me.Padding = New System.Windows.Forms.Padding(18)
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Generate Volume Serial Number"
        TableLayoutPanel1.ResumeLayout(False)
        TableLayoutPanel1.PerformLayout()
        CType(Me.NumMS, System.ComponentModel.ISupportInitialize).EndInit()
        FlowLayoutPanel2.ResumeLayout(False)
        FlowLayoutPanel1.ResumeLayout(False)
        FlowLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents NumMS As NumericUpDown
    Friend WithEvents DTDate As DateTimePicker
    Friend WithEvents DTTime As DateTimePicker
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
End Class
