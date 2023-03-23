<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FATEditForm
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
        Me.components = New System.ComponentModel.Container()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.DataGridViewFAT = New System.Windows.Forms.DataGridView()
        Me.GridCluster = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GridType = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GridValue = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GridError = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GridFile = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.ChkSync = New System.Windows.Forms.CheckBox()
        Me.ContextMenuGrid = New System.Windows.Forms.ContextMenuStrip(Me.components)
        CType(Me.DataGridViewFAT, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DataGridViewFAT
        '
        Me.DataGridViewFAT.AllowUserToAddRows = False
        Me.DataGridViewFAT.AllowUserToDeleteRows = False
        Me.DataGridViewFAT.AllowUserToResizeColumns = False
        Me.DataGridViewFAT.AllowUserToResizeRows = False
        Me.DataGridViewFAT.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridViewFAT.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewFAT.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.GridCluster, Me.GridType, Me.GridValue, Me.GridError, Me.GridFile})
        Me.DataGridViewFAT.ContextMenuStrip = Me.ContextMenuGrid
        Me.DataGridViewFAT.Location = New System.Drawing.Point(21, 21)
        Me.DataGridViewFAT.MultiSelect = False
        Me.DataGridViewFAT.Name = "DataGridViewFAT"
        Me.DataGridViewFAT.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DataGridViewFAT.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.DataGridViewFAT.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DataGridViewFAT.Size = New System.Drawing.Size(542, 497)
        Me.DataGridViewFAT.TabIndex = 0
        '
        'GridCluster
        '
        Me.GridCluster.DataPropertyName = "Cluster"
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black
        Me.GridCluster.DefaultCellStyle = DataGridViewCellStyle5
        Me.GridCluster.HeaderText = "Cluster"
        Me.GridCluster.Name = "GridCluster"
        Me.GridCluster.ReadOnly = True
        Me.GridCluster.Width = 65
        '
        'GridType
        '
        Me.GridType.DataPropertyName = "Type"
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black
        Me.GridType.DefaultCellStyle = DataGridViewCellStyle6
        Me.GridType.HeaderText = "Type"
        Me.GridType.Name = "GridType"
        Me.GridType.ReadOnly = True
        Me.GridType.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.GridType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.GridType.Width = 75
        '
        'GridValue
        '
        Me.GridValue.DataPropertyName = "Value"
        Me.GridValue.HeaderText = "Value"
        Me.GridValue.Name = "GridValue"
        Me.GridValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.GridValue.Width = 60
        '
        'GridError
        '
        Me.GridError.DataPropertyName = "Error"
        DataGridViewCellStyle7.ForeColor = System.Drawing.Color.Red
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Red
        Me.GridError.DefaultCellStyle = DataGridViewCellStyle7
        Me.GridError.HeaderText = "Error"
        Me.GridError.Name = "GridError"
        Me.GridError.ReadOnly = True
        Me.GridError.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.GridError.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'GridFile
        '
        Me.GridFile.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.GridFile.DataPropertyName = "File"
        DataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.White
        DataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.Black
        Me.GridFile.DefaultCellStyle = DataGridViewCellStyle8
        Me.GridFile.HeaderText = "File"
        Me.GridFile.Name = "GridFile"
        Me.GridFile.ReadOnly = True
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnUpdate)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnCancel)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(196, 547)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(192, 29)
        Me.FlowLayoutPanel1.TabIndex = 2
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
        'ChkSync
        '
        Me.ChkSync.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.ChkSync.AutoSize = True
        Me.ChkSync.Location = New System.Drawing.Point(236, 524)
        Me.ChkSync.Name = "ChkSync"
        Me.ChkSync.Size = New System.Drawing.Size(112, 17)
        Me.ChkSync.TabIndex = 1
        Me.ChkSync.Text = "Synchronize FATs"
        Me.ChkSync.UseVisualStyleBackColor = True
        '
        'ContextMenuGrid
        '
        Me.ContextMenuGrid.Name = "ContextMenuGrid"
        Me.ContextMenuGrid.ShowImageMargin = False
        Me.ContextMenuGrid.Size = New System.Drawing.Size(36, 4)
        '
        'FATEditForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 597)
        Me.Controls.Add(Me.ChkSync)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.DataGridViewFAT)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(600, 450)
        Me.Name = "FATEditForm"
        Me.Padding = New System.Windows.Forms.Padding(18)
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "File Allocation Table"
        CType(Me.DataGridViewFAT, System.ComponentModel.ISupportInitialize).EndInit()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents DataGridViewFAT As DataGridView
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents BtnUpdate As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents ChkSync As CheckBox
    Friend WithEvents GridCrossLinked As DataGridViewCheckBoxColumn
    Friend WithEvents GridCluster As DataGridViewTextBoxColumn
    Friend WithEvents GridType As DataGridViewTextBoxColumn
    Friend WithEvents GridValue As DataGridViewTextBoxColumn
    Friend WithEvents GridError As DataGridViewTextBoxColumn
    Friend WithEvents GridFile As DataGridViewTextBoxColumn
    Friend WithEvents ContextMenuGrid As ContextMenuStrip
End Class
