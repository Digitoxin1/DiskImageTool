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
        Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
        Dim PanelMain As System.Windows.Forms.Panel
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.PictureBoxFAT = New System.Windows.Forms.PictureBox()
        Me.DataGridViewFAT = New System.Windows.Forms.DataGridView()
        Me.ContextMenuGrid = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ChkSync = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanelTop = New System.Windows.Forms.FlowLayoutPanel()
        Me.BtnFree = New System.Windows.Forms.Button()
        Me.BtnBad = New System.Windows.Forms.Button()
        Me.BtnLast = New System.Windows.Forms.Button()
        Me.BtnReserved = New System.Windows.Forms.Button()
        Me.LblValid = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.TxtMediaDescriptor = New System.Windows.Forms.Label()
        Me.CboMediaDescriptor = New System.Windows.Forms.ComboBox()
        Me.ContextMenuLast = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ContextMenuReserved = New System.Windows.Forms.ContextMenuStrip(Me.components)
        PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        PanelMain = New System.Windows.Forms.Panel()
        PanelBottom.SuspendLayout()
        PanelMain.SuspendLayout()
        CType(Me.PictureBoxFAT, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridViewFAT, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.FlowLayoutPanelTop.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelBottom
        '
        PanelBottom.Controls.Add(Me.BtnCancel)
        PanelBottom.Controls.Add(Me.BtnUpdate)
        PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        PanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        PanelBottom.Location = New System.Drawing.Point(0, 552)
        PanelBottom.Margin = New System.Windows.Forms.Padding(0)
        PanelBottom.Name = "PanelBottom"
        PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
        PanelBottom.Size = New System.Drawing.Size(634, 43)
        PanelBottom.TabIndex = 1
        PanelBottom.WrapContents = False
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(541, 10)
        Me.BtnCancel.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "{&Cancel}"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnUpdate
        '
        Me.BtnUpdate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnUpdate.Location = New System.Drawing.Point(454, 10)
        Me.BtnUpdate.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.Size = New System.Drawing.Size(75, 23)
        Me.BtnUpdate.TabIndex = 0
        Me.BtnUpdate.Text = "{&Update}"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'PanelMain
        '
        PanelMain.Controls.Add(Me.PictureBoxFAT)
        PanelMain.Controls.Add(Me.DataGridViewFAT)
        PanelMain.Controls.Add(Me.ChkSync)
        PanelMain.Controls.Add(Me.FlowLayoutPanelTop)
        PanelMain.Controls.Add(Me.FlowLayoutPanel1)
        PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        PanelMain.Location = New System.Drawing.Point(0, 0)
        PanelMain.Name = "PanelMain"
        PanelMain.Padding = New System.Windows.Forms.Padding(18, 18, 18, 6)
        PanelMain.Size = New System.Drawing.Size(634, 552)
        PanelMain.TabIndex = 0
        '
        'PictureBoxFAT
        '
        Me.PictureBoxFAT.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBoxFAT.Location = New System.Drawing.Point(18, 18)
        Me.PictureBoxFAT.Name = "PictureBoxFAT"
        Me.PictureBoxFAT.Size = New System.Drawing.Size(598, 138)
        Me.PictureBoxFAT.TabIndex = 4
        Me.PictureBoxFAT.TabStop = False
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
        Me.DataGridViewFAT.Location = New System.Drawing.Point(18, 200)
        Me.DataGridViewFAT.MultiSelect = False
        Me.DataGridViewFAT.Name = "DataGridViewFAT"
        Me.DataGridViewFAT.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DataGridViewFAT.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.DataGridViewFAT.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DataGridViewFAT.Size = New System.Drawing.Size(598, 328)
        Me.DataGridViewFAT.TabIndex = 2
        '
        'ContextMenuGrid
        '
        Me.ContextMenuGrid.Name = "ContextMenuGrid"
        Me.ContextMenuGrid.ShowImageMargin = False
        Me.ContextMenuGrid.Size = New System.Drawing.Size(36, 4)
        '
        'ChkSync
        '
        Me.ChkSync.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.ChkSync.AutoSize = True
        Me.ChkSync.Location = New System.Drawing.Point(261, 534)
        Me.ChkSync.Name = "ChkSync"
        Me.ChkSync.Size = New System.Drawing.Size(120, 17)
        Me.ChkSync.TabIndex = 3
        Me.ChkSync.Text = "{Synchronize FATs}"
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
        Me.FlowLayoutPanelTop.Location = New System.Drawing.Point(18, 162)
        Me.FlowLayoutPanelTop.Name = "FlowLayoutPanelTop"
        Me.FlowLayoutPanelTop.Size = New System.Drawing.Size(335, 29)
        Me.FlowLayoutPanelTop.TabIndex = 0
        '
        'BtnFree
        '
        Me.BtnFree.AutoSize = True
        Me.BtnFree.Location = New System.Drawing.Point(3, 3)
        Me.BtnFree.Name = "BtnFree"
        Me.BtnFree.Size = New System.Drawing.Size(50, 23)
        Me.BtnFree.TabIndex = 0
        Me.BtnFree.Text = "{Free}"
        Me.BtnFree.UseVisualStyleBackColor = True
        '
        'BtnBad
        '
        Me.BtnBad.AutoSize = True
        Me.BtnBad.Location = New System.Drawing.Point(59, 3)
        Me.BtnBad.Name = "BtnBad"
        Me.BtnBad.Size = New System.Drawing.Size(50, 23)
        Me.BtnBad.TabIndex = 1
        Me.BtnBad.Text = "{Bad}"
        Me.BtnBad.UseVisualStyleBackColor = True
        '
        'BtnLast
        '
        Me.BtnLast.AutoSize = True
        Me.BtnLast.Location = New System.Drawing.Point(115, 3)
        Me.BtnLast.Name = "BtnLast"
        Me.BtnLast.Size = New System.Drawing.Size(50, 23)
        Me.BtnLast.TabIndex = 2
        Me.BtnLast.Text = "{Last}"
        Me.BtnLast.UseVisualStyleBackColor = True
        '
        'BtnReserved
        '
        Me.BtnReserved.AutoSize = True
        Me.BtnReserved.Location = New System.Drawing.Point(171, 3)
        Me.BtnReserved.Name = "BtnReserved"
        Me.BtnReserved.Size = New System.Drawing.Size(75, 23)
        Me.BtnReserved.TabIndex = 3
        Me.BtnReserved.Text = "{Reserved}"
        Me.BtnReserved.UseVisualStyleBackColor = True
        '
        'LblValid
        '
        Me.LblValid.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblValid.AutoSize = True
        Me.LblValid.Location = New System.Drawing.Point(257, 8)
        Me.LblValid.Margin = New System.Windows.Forms.Padding(8, 0, 3, 0)
        Me.LblValid.Name = "LblValid"
        Me.LblValid.Size = New System.Drawing.Size(75, 13)
        Me.LblValid.TabIndex = 4
        Me.LblValid.Text = "{ValidClusters}"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel1.Controls.Add(Me.TxtMediaDescriptor)
        Me.FlowLayoutPanel1.Controls.Add(Me.CboMediaDescriptor)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(444, 163)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(172, 27)
        Me.FlowLayoutPanel1.TabIndex = 1
        '
        'TxtMediaDescriptor
        '
        Me.TxtMediaDescriptor.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtMediaDescriptor.AutoSize = True
        Me.TxtMediaDescriptor.Location = New System.Drawing.Point(3, 7)
        Me.TxtMediaDescriptor.Name = "TxtMediaDescriptor"
        Me.TxtMediaDescriptor.Size = New System.Drawing.Size(95, 13)
        Me.TxtMediaDescriptor.TabIndex = 20
        Me.TxtMediaDescriptor.Text = "{Media Descriptor}"
        '
        'CboMediaDescriptor
        '
        Me.CboMediaDescriptor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.CboMediaDescriptor.FormattingEnabled = True
        Me.CboMediaDescriptor.Location = New System.Drawing.Point(104, 3)
        Me.CboMediaDescriptor.MaxLength = 2
        Me.CboMediaDescriptor.Name = "CboMediaDescriptor"
        Me.CboMediaDescriptor.Size = New System.Drawing.Size(65, 21)
        Me.CboMediaDescriptor.TabIndex = 22
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
        'FATEditForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(634, 595)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(650, 450)
        Me.Name = "FATEditForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        PanelBottom.ResumeLayout(False)
        PanelMain.ResumeLayout(False)
        PanelMain.PerformLayout()
        CType(Me.PictureBoxFAT, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridViewFAT, System.ComponentModel.ISupportInitialize).EndInit()
        Me.FlowLayoutPanelTop.ResumeLayout(False)
        Me.FlowLayoutPanelTop.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents DataGridViewFAT As DataGridView
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
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents TxtMediaDescriptor As Label
    Friend WithEvents CboMediaDescriptor As ComboBox
End Class
