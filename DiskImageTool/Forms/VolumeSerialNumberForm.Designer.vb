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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(VolumeSerialNumberForm))
        Dim LabelDate As System.Windows.Forms.Label
        Dim LabelMilliseconds As System.Windows.Forms.Label
        Dim LabelTime As System.Windows.Forms.Label
        Dim FlowLayoutPanelButtons As System.Windows.Forms.FlowLayoutPanel
        Dim FlowLayoutPanelMain As System.Windows.Forms.FlowLayoutPanel
        Me.DTDate = New System.Windows.Forms.DateTimePicker()
        Me.DTTime = New System.Windows.Forms.DateTimePicker()
        Me.NumMS = New System.Windows.Forms.NumericUpDown()
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        LabelDate = New System.Windows.Forms.Label()
        LabelMilliseconds = New System.Windows.Forms.Label()
        LabelTime = New System.Windows.Forms.Label()
        FlowLayoutPanelButtons = New System.Windows.Forms.FlowLayoutPanel()
        FlowLayoutPanelMain = New System.Windows.Forms.FlowLayoutPanel()
        TableLayoutPanel1.SuspendLayout()
        CType(Me.NumMS, System.ComponentModel.ISupportInitialize).BeginInit()
        FlowLayoutPanelButtons.SuspendLayout()
        FlowLayoutPanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(TableLayoutPanel1, "TableLayoutPanel1")
        TableLayoutPanel1.Controls.Add(Me.DTDate, 0, 1)
        TableLayoutPanel1.Controls.Add(LabelDate, 0, 0)
        TableLayoutPanel1.Controls.Add(Me.DTTime, 1, 1)
        TableLayoutPanel1.Controls.Add(LabelMilliseconds, 2, 0)
        TableLayoutPanel1.Controls.Add(Me.NumMS, 2, 1)
        TableLayoutPanel1.Controls.Add(LabelTime, 1, 0)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'DTDate
        '
        Me.DTDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        resources.ApplyResources(Me.DTDate, "DTDate")
        Me.DTDate.MaxDate = New Date(2107, 12, 31, 0, 0, 0, 0)
        Me.DTDate.MinDate = New Date(1980, 1, 1, 0, 0, 0, 0)
        Me.DTDate.Name = "DTDate"
        '
        'LabelDate
        '
        resources.ApplyResources(LabelDate, "LabelDate")
        LabelDate.Name = "LabelDate"
        '
        'DTTime
        '
        Me.DTTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        resources.ApplyResources(Me.DTTime, "DTTime")
        Me.DTTime.MaxDate = New Date(2107, 12, 31, 0, 0, 0, 0)
        Me.DTTime.MinDate = New Date(1980, 1, 1, 0, 0, 0, 0)
        Me.DTTime.Name = "DTTime"
        Me.DTTime.ShowUpDown = True
        '
        'LabelMilliseconds
        '
        resources.ApplyResources(LabelMilliseconds, "LabelMilliseconds")
        LabelMilliseconds.Name = "LabelMilliseconds"
        '
        'NumMS
        '
        Me.NumMS.Increment = New Decimal(New Integer() {10, 0, 0, 0})
        resources.ApplyResources(Me.NumMS, "NumMS")
        Me.NumMS.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        Me.NumMS.Name = "NumMS"
        '
        'LabelTime
        '
        resources.ApplyResources(LabelTime, "LabelTime")
        LabelTime.Name = "LabelTime"
        '
        'FlowLayoutPanelButtons
        '
        resources.ApplyResources(FlowLayoutPanelButtons, "FlowLayoutPanelButtons")
        FlowLayoutPanelButtons.Controls.Add(Me.BtnOK)
        FlowLayoutPanelButtons.Controls.Add(Me.BtnCancel)
        FlowLayoutPanelButtons.Name = "FlowLayoutPanelButtons"
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
        'FlowLayoutPanelMain
        '
        resources.ApplyResources(FlowLayoutPanelMain, "FlowLayoutPanelMain")
        FlowLayoutPanelMain.Controls.Add(TableLayoutPanel1)
        FlowLayoutPanelMain.Controls.Add(FlowLayoutPanelButtons)
        FlowLayoutPanelMain.Name = "FlowLayoutPanelMain"
        '
        'VolumeSerialNumberForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(FlowLayoutPanelMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "VolumeSerialNumberForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        TableLayoutPanel1.ResumeLayout(False)
        TableLayoutPanel1.PerformLayout()
        CType(Me.NumMS, System.ComponentModel.ISupportInitialize).EndInit()
        FlowLayoutPanelButtons.ResumeLayout(False)
        FlowLayoutPanelMain.ResumeLayout(False)
        FlowLayoutPanelMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents NumMS As NumericUpDown
    Friend WithEvents DTDate As DateTimePicker
    Friend WithEvents DTTime As DateTimePicker
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
End Class
