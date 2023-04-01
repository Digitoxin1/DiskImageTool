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
        Me.DataGridViewFAT = New System.Windows.Forms.DataGridView()
        Me.ContextMenuGrid = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.FlowLayoutPanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.ChkSync = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanelTop = New System.Windows.Forms.FlowLayoutPanel()
        Me.BtnFree = New System.Windows.Forms.Button()
        Me.BtnBad = New System.Windows.Forms.Button()
        Me.BtnLast = New System.Windows.Forms.Button()
        Me.BtnReserved = New System.Windows.Forms.Button()
        Me.LblValid = New System.Windows.Forms.Label()
        Me.ContextMenuLast = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ContextMenuReserved = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.PictureBoxFAT = New System.Windows.Forms.PictureBox()
        CType(Me.DataGridViewFAT, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.FlowLayoutPanelBottom.SuspendLayout()
        Me.FlowLayoutPanelTop.SuspendLayout()
        CType(Me.PictureBoxFAT, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.DataGridViewFAT.ContextMenuStrip = Me.ContextMenuGrid
        Me.DataGridViewFAT.Location = New System.Drawing.Point(21, 200)
        Me.DataGridViewFAT.MultiSelect = False
        Me.DataGridViewFAT.Name = "DataGridViewFAT"
        Me.DataGridViewFAT.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DataGridViewFAT.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.DataGridViewFAT.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DataGridViewFAT.Size = New System.Drawing.Size(542, 318)
        Me.DataGridViewFAT.TabIndex = 1
        '
        'ContextMenuGrid
        '
        Me.ContextMenuGrid.Name = "ContextMenuGrid"
        Me.ContextMenuGrid.ShowImageMargin = False
        Me.ContextMenuGrid.Size = New System.Drawing.Size(36, 4)
        '
        'FlowLayoutPanelBottom
        '
        Me.FlowLayoutPanelBottom.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.FlowLayoutPanelBottom.AutoSize = True
        Me.FlowLayoutPanelBottom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanelBottom.Controls.Add(Me.BtnUpdate)
        Me.FlowLayoutPanelBottom.Controls.Add(Me.BtnCancel)
        Me.FlowLayoutPanelBottom.Location = New System.Drawing.Point(196, 547)
        Me.FlowLayoutPanelBottom.Name = "FlowLayoutPanelBottom"
        Me.FlowLayoutPanelBottom.Size = New System.Drawing.Size(192, 29)
        Me.FlowLayoutPanelBottom.TabIndex = 3
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
        Me.ChkSync.TabIndex = 2
        Me.ChkSync.Text = "Synchronize FATs"
        Me.ChkSync.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanelTop
        '
        Me.FlowLayoutPanelTop.AutoSize = True
        Me.FlowLayoutPanelTop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanelTop.Controls.Add(Me.BtnFree)
        Me.FlowLayoutPanelTop.Controls.Add(Me.BtnBad)
        Me.FlowLayoutPanelTop.Controls.Add(Me.BtnLast)
        Me.FlowLayoutPanelTop.Controls.Add(Me.BtnReserved)
        Me.FlowLayoutPanelTop.Controls.Add(Me.LblValid)
        Me.FlowLayoutPanelTop.Location = New System.Drawing.Point(21, 165)
        Me.FlowLayoutPanelTop.Name = "FlowLayoutPanelTop"
        Me.FlowLayoutPanelTop.Size = New System.Drawing.Size(423, 29)
        Me.FlowLayoutPanelTop.TabIndex = 0
        '
        'BtnFree
        '
        Me.BtnFree.Location = New System.Drawing.Point(3, 3)
        Me.BtnFree.Name = "BtnFree"
        Me.BtnFree.Size = New System.Drawing.Size(75, 23)
        Me.BtnFree.TabIndex = 0
        Me.BtnFree.Text = "Free"
        Me.BtnFree.UseVisualStyleBackColor = True
        '
        'BtnBad
        '
        Me.BtnBad.Location = New System.Drawing.Point(84, 3)
        Me.BtnBad.Name = "BtnBad"
        Me.BtnBad.Size = New System.Drawing.Size(75, 23)
        Me.BtnBad.TabIndex = 1
        Me.BtnBad.Text = "Bad"
        Me.BtnBad.UseVisualStyleBackColor = True
        '
        'BtnLast
        '
        Me.BtnLast.Location = New System.Drawing.Point(165, 3)
        Me.BtnLast.Name = "BtnLast"
        Me.BtnLast.Size = New System.Drawing.Size(75, 23)
        Me.BtnLast.TabIndex = 2
        Me.BtnLast.Text = "Last"
        Me.BtnLast.UseVisualStyleBackColor = True
        '
        'BtnReserved
        '
        Me.BtnReserved.Location = New System.Drawing.Point(246, 3)
        Me.BtnReserved.Name = "BtnReserved"
        Me.BtnReserved.Size = New System.Drawing.Size(75, 23)
        Me.BtnReserved.TabIndex = 3
        Me.BtnReserved.Text = "Reserved"
        Me.BtnReserved.UseVisualStyleBackColor = True
        '
        'LblValid
        '
        Me.LblValid.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblValid.AutoSize = True
        Me.LblValid.Location = New System.Drawing.Point(347, 8)
        Me.LblValid.Margin = New System.Windows.Forms.Padding(23, 0, 3, 0)
        Me.LblValid.Name = "LblValid"
        Me.LblValid.Size = New System.Drawing.Size(73, 13)
        Me.LblValid.TabIndex = 4
        Me.LblValid.Text = "Valid Clusters:"
        '
        'ContextMenuLast
        '
        Me.ContextMenuLast.Name = "ContextMenuLast"
        Me.ContextMenuLast.ShowImageMargin = False
        Me.ContextMenuLast.Size = New System.Drawing.Size(36, 4)
        '
        'ContextMenuReserved
        '
        Me.ContextMenuReserved.Name = "ContextMenuReserved"
        Me.ContextMenuReserved.ShowImageMargin = False
        Me.ContextMenuReserved.Size = New System.Drawing.Size(36, 4)
        '
        'PictureBoxFAT
        '
        Me.PictureBoxFAT.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBoxFAT.Location = New System.Drawing.Point(24, 21)
        Me.PictureBoxFAT.Name = "PictureBoxFAT"
        Me.PictureBoxFAT.Size = New System.Drawing.Size(539, 138)
        Me.PictureBoxFAT.TabIndex = 4
        Me.PictureBoxFAT.TabStop = False
        '
        'FATEditForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 597)
        Me.Controls.Add(Me.PictureBoxFAT)
        Me.Controls.Add(Me.FlowLayoutPanelTop)
        Me.Controls.Add(Me.ChkSync)
        Me.Controls.Add(Me.FlowLayoutPanelBottom)
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
        Me.FlowLayoutPanelBottom.ResumeLayout(False)
        Me.FlowLayoutPanelTop.ResumeLayout(False)
        Me.FlowLayoutPanelTop.PerformLayout()
        CType(Me.PictureBoxFAT, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents DataGridViewFAT As DataGridView
    Friend WithEvents FlowLayoutPanelBottom As FlowLayoutPanel
    Friend WithEvents BtnUpdate As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents ChkSync As CheckBox
    Friend WithEvents GridCrossLinked As DataGridViewCheckBoxColumn
    Friend WithEvents ContextMenuGrid As ContextMenuStrip
    Friend WithEvents FlowLayoutPanelTop As FlowLayoutPanel
    Friend WithEvents BtnFree As Button
    Friend WithEvents BtnBad As Button
    Friend WithEvents BtnLast As Button
    Friend WithEvents BtnReserved As Button
    Friend WithEvents ContextMenuLast As ContextMenuStrip
    Friend WithEvents ContextMenuReserved As ContextMenuStrip
    Friend WithEvents LblValid As Label
    Friend WithEvents PictureBoxFAT As PictureBox
End Class
