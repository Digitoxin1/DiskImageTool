<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FATEditForm
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
        Me.DataGridViewFAT = New System.Windows.Forms.DataGridView()
        Me.GridCluster = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GridType = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.GridValue = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GridFile = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.DataGridViewFAT, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.DataGridViewFAT.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.GridCluster, Me.GridType, Me.GridValue, Me.GridFile})
        Me.DataGridViewFAT.Location = New System.Drawing.Point(12, 12)
        Me.DataGridViewFAT.MultiSelect = False
        Me.DataGridViewFAT.Name = "DataGridViewFAT"
        Me.DataGridViewFAT.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DataGridViewFAT.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.DataGridViewFAT.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DataGridViewFAT.Size = New System.Drawing.Size(466, 573)
        Me.DataGridViewFAT.TabIndex = 0
        '
        'GridCluster
        '
        Me.GridCluster.DataPropertyName = "Cluster"
        Me.GridCluster.HeaderText = "Cluster"
        Me.GridCluster.Name = "GridCluster"
        Me.GridCluster.ReadOnly = True
        Me.GridCluster.Width = 60
        '
        'GridType
        '
        Me.GridType.DataPropertyName = "Type"
        Me.GridType.HeaderText = "Type"
        Me.GridType.Items.AddRange(New Object() {"Free", "Next", "Bad", "Last", "Reserved"})
        Me.GridType.Name = "GridType"
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
        'GridFile
        '
        Me.GridFile.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.GridFile.DataPropertyName = "File"
        Me.GridFile.HeaderText = "File"
        Me.GridFile.Name = "GridFile"
        Me.GridFile.ReadOnly = True
        '
        'FATEditForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(490, 597)
        Me.Controls.Add(Me.DataGridViewFAT)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FATEditForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "File Allocation Table"
        CType(Me.DataGridViewFAT, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents DataGridViewFAT As DataGridView
    Friend WithEvents GridCluster As DataGridViewTextBoxColumn
    Friend WithEvents GridType As DataGridViewComboBoxColumn
    Friend WithEvents GridValue As DataGridViewTextBoxColumn
    Friend WithEvents GridFile As DataGridViewTextBoxColumn
End Class
