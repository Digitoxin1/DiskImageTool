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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FATEditForm))
        Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
        Dim PanelMain As System.Windows.Forms.Panel
        Me.DataGridViewFAT = New System.Windows.Forms.DataGridView()
        Me.ContextMenuGrid = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnUpdate = New System.Windows.Forms.Button()
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
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.TxtMediaDescriptor = New System.Windows.Forms.Label()
        Me.CboMediaDescriptor = New System.Windows.Forms.ComboBox()
        PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        PanelMain = New System.Windows.Forms.Panel()
        CType(Me.DataGridViewFAT, System.ComponentModel.ISupportInitialize).BeginInit()
        PanelBottom.SuspendLayout()
        Me.FlowLayoutPanelTop.SuspendLayout()
        CType(Me.PictureBoxFAT, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.FlowLayoutPanel1.SuspendLayout()
        PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'DataGridViewFAT
        '
        Me.DataGridViewFAT.AllowUserToAddRows = False
        Me.DataGridViewFAT.AllowUserToDeleteRows = False
        Me.DataGridViewFAT.AllowUserToResizeColumns = False
        Me.DataGridViewFAT.AllowUserToResizeRows = False
        resources.ApplyResources(Me.DataGridViewFAT, "DataGridViewFAT")
        Me.DataGridViewFAT.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewFAT.ContextMenuStrip = Me.ContextMenuGrid
        Me.DataGridViewFAT.MultiSelect = False
        Me.DataGridViewFAT.Name = "DataGridViewFAT"
        Me.DataGridViewFAT.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DataGridViewFAT.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        '
        'ContextMenuGrid
        '
        Me.ContextMenuGrid.Name = "ContextMenuGrid"
        Me.ContextMenuGrid.ShowImageMargin = False
        resources.ApplyResources(Me.ContextMenuGrid, "ContextMenuGrid")
        '
        'PanelBottom
        '
        PanelBottom.Controls.Add(Me.BtnCancel)
        PanelBottom.Controls.Add(Me.BtnUpdate)
        resources.ApplyResources(PanelBottom, "PanelBottom")
        PanelBottom.Name = "PanelBottom"
        '
        'BtnCancel
        '
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnUpdate
        '
        resources.ApplyResources(Me.BtnUpdate, "BtnUpdate")
        Me.BtnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'ChkSync
        '
        resources.ApplyResources(Me.ChkSync, "ChkSync")
        Me.ChkSync.Name = "ChkSync"
        Me.ChkSync.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanelTop
        '
        resources.ApplyResources(Me.FlowLayoutPanelTop, "FlowLayoutPanelTop")
        Me.FlowLayoutPanelTop.Controls.Add(Me.BtnFree)
        Me.FlowLayoutPanelTop.Controls.Add(Me.BtnBad)
        Me.FlowLayoutPanelTop.Controls.Add(Me.BtnLast)
        Me.FlowLayoutPanelTop.Controls.Add(Me.BtnReserved)
        Me.FlowLayoutPanelTop.Controls.Add(Me.LblValid)
        Me.FlowLayoutPanelTop.Name = "FlowLayoutPanelTop"
        '
        'BtnFree
        '
        resources.ApplyResources(Me.BtnFree, "BtnFree")
        Me.BtnFree.Name = "BtnFree"
        Me.BtnFree.UseVisualStyleBackColor = True
        '
        'BtnBad
        '
        resources.ApplyResources(Me.BtnBad, "BtnBad")
        Me.BtnBad.Name = "BtnBad"
        Me.BtnBad.UseVisualStyleBackColor = True
        '
        'BtnLast
        '
        resources.ApplyResources(Me.BtnLast, "BtnLast")
        Me.BtnLast.Name = "BtnLast"
        Me.BtnLast.UseVisualStyleBackColor = True
        '
        'BtnReserved
        '
        resources.ApplyResources(Me.BtnReserved, "BtnReserved")
        Me.BtnReserved.Name = "BtnReserved"
        Me.BtnReserved.UseVisualStyleBackColor = True
        '
        'LblValid
        '
        resources.ApplyResources(Me.LblValid, "LblValid")
        Me.LblValid.Name = "LblValid"
        '
        'ContextMenuLast
        '
        Me.ContextMenuLast.Name = "ContextMenuLast"
        Me.ContextMenuLast.ShowImageMargin = False
        resources.ApplyResources(Me.ContextMenuLast, "ContextMenuLast")
        '
        'ContextMenuReserved
        '
        Me.ContextMenuReserved.Name = "ContextMenuReserved"
        Me.ContextMenuReserved.ShowImageMargin = False
        resources.ApplyResources(Me.ContextMenuReserved, "ContextMenuReserved")
        '
        'PictureBoxFAT
        '
        resources.ApplyResources(Me.PictureBoxFAT, "PictureBoxFAT")
        Me.PictureBoxFAT.Name = "PictureBoxFAT"
        Me.PictureBoxFAT.TabStop = False
        '
        'FlowLayoutPanel1
        '
        resources.ApplyResources(Me.FlowLayoutPanel1, "FlowLayoutPanel1")
        Me.FlowLayoutPanel1.Controls.Add(Me.TxtMediaDescriptor)
        Me.FlowLayoutPanel1.Controls.Add(Me.CboMediaDescriptor)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        '
        'TxtMediaDescriptor
        '
        resources.ApplyResources(Me.TxtMediaDescriptor, "TxtMediaDescriptor")
        Me.TxtMediaDescriptor.Name = "TxtMediaDescriptor"
        '
        'CboMediaDescriptor
        '
        Me.CboMediaDescriptor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.CboMediaDescriptor.FormattingEnabled = True
        resources.ApplyResources(Me.CboMediaDescriptor, "CboMediaDescriptor")
        Me.CboMediaDescriptor.Name = "CboMediaDescriptor"
        '
        'PanelMain
        '
        PanelMain.Controls.Add(Me.PictureBoxFAT)
        PanelMain.Controls.Add(Me.DataGridViewFAT)
        PanelMain.Controls.Add(Me.ChkSync)
        PanelMain.Controls.Add(Me.FlowLayoutPanelTop)
        PanelMain.Controls.Add(Me.FlowLayoutPanel1)
        resources.ApplyResources(PanelMain, "PanelMain")
        PanelMain.Name = "PanelMain"
        '
        'FATEditForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FATEditForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.DataGridViewFAT, System.ComponentModel.ISupportInitialize).EndInit()
        PanelBottom.ResumeLayout(False)
        Me.FlowLayoutPanelTop.ResumeLayout(False)
        Me.FlowLayoutPanelTop.PerformLayout()
        CType(Me.PictureBoxFAT, System.ComponentModel.ISupportInitialize).EndInit()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        PanelMain.ResumeLayout(False)
        PanelMain.PerformLayout()
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
