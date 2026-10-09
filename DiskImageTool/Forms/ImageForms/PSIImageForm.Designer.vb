<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PSIImageForm
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.PanelBottom = New System.Windows.Forms.Panel()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.PanelMain = New System.Windows.Forms.Panel()
        Me.TableLayoutPanelGrids = New System.Windows.Forms.TableLayoutPanel()
        Me.LblTracks = New System.Windows.Forms.Label()
        Me.LblSectors = New System.Windows.Forms.Label()
        Me.DataGridViewTracks = New System.Windows.Forms.DataGridView()
        Me.DataGridViewSectors = New System.Windows.Forms.DataGridView()
        Me.TableLayoutPanelHeader = New System.Windows.Forms.TableLayoutPanel()
        Me.LblVersion = New System.Windows.Forms.Label()
        Me.TxtVersion = New System.Windows.Forms.TextBox()
        Me.LblFormat = New System.Windows.Forms.Label()
        Me.TxtFormat = New System.Windows.Forms.TextBox()
        Me.LblComment = New System.Windows.Forms.Label()
        Me.TxtComment = New System.Windows.Forms.TextBox()
        Me.PanelBottom.SuspendLayout()
        Me.PanelMain.SuspendLayout()
        Me.TableLayoutPanelGrids.SuspendLayout()
        CType(Me.DataGridViewTracks, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridViewSectors, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanelHeader.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelBottom
        '
        Me.PanelBottom.Controls.Add(Me.BtnCancel)
        Me.PanelBottom.Controls.Add(Me.BtnUpdate)
        Me.PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelBottom.Location = New System.Drawing.Point(0, 554)
        Me.PanelBottom.Name = "PanelBottom"
        Me.PanelBottom.Padding = New System.Windows.Forms.Padding(12, 6, 12, 12)
        Me.PanelBottom.Size = New System.Drawing.Size(704, 47)
        Me.PanelBottom.TabIndex = 1
        '
        'BtnCancel
        '
        Me.BtnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(617, 12)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "{Cancel}"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnUpdate
        '
        Me.BtnUpdate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnUpdate.Location = New System.Drawing.Point(536, 12)
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.Size = New System.Drawing.Size(75, 23)
        Me.BtnUpdate.TabIndex = 0
        Me.BtnUpdate.Text = "{Update}"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'PanelMain
        '
        Me.PanelMain.Controls.Add(Me.TableLayoutPanelGrids)
        Me.PanelMain.Controls.Add(Me.TableLayoutPanelHeader)
        Me.PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelMain.Location = New System.Drawing.Point(0, 0)
        Me.PanelMain.Name = "PanelMain"
        Me.PanelMain.Padding = New System.Windows.Forms.Padding(12, 12, 12, 6)
        Me.PanelMain.Size = New System.Drawing.Size(704, 554)
        Me.PanelMain.TabIndex = 0
        '
        'TableLayoutPanelGrids
        '
        Me.TableLayoutPanelGrids.ColumnCount = 2
        Me.TableLayoutPanelGrids.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelGrids.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanelGrids.Controls.Add(Me.LblTracks, 0, 0)
        Me.TableLayoutPanelGrids.Controls.Add(Me.LblSectors, 1, 0)
        Me.TableLayoutPanelGrids.Controls.Add(Me.DataGridViewTracks, 0, 1)
        Me.TableLayoutPanelGrids.Controls.Add(Me.DataGridViewSectors, 1, 1)
        Me.TableLayoutPanelGrids.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanelGrids.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize
        Me.TableLayoutPanelGrids.Location = New System.Drawing.Point(12, 132)
        Me.TableLayoutPanelGrids.Name = "TableLayoutPanelGrids"
        Me.TableLayoutPanelGrids.RowCount = 2
        Me.TableLayoutPanelGrids.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelGrids.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanelGrids.Size = New System.Drawing.Size(680, 416)
        Me.TableLayoutPanelGrids.TabIndex = 1
        '
        'LblTracks
        '
        Me.LblTracks.AutoSize = True
        Me.LblTracks.Location = New System.Drawing.Point(3, 0)
        Me.LblTracks.Margin = New System.Windows.Forms.Padding(3, 0, 8, 4)
        Me.LblTracks.Name = "LblTracks"
        Me.LblTracks.Size = New System.Drawing.Size(48, 13)
        Me.LblTracks.TabIndex = 0
        Me.LblTracks.Text = "{Tracks}"
        '
        'LblSectors
        '
        Me.LblSectors.AutoSize = True
        Me.LblSectors.Location = New System.Drawing.Point(320, 0)
        Me.LblSectors.Margin = New System.Windows.Forms.Padding(12, 0, 8, 4)
        Me.LblSectors.Name = "LblSectors"
        Me.LblSectors.Size = New System.Drawing.Size(51, 13)
        Me.LblSectors.TabIndex = 1
        Me.LblSectors.Text = "{Sectors}"
        '
        'DataGridViewTracks
        '
        Me.DataGridViewTracks.AllowUserToAddRows = False
        Me.DataGridViewTracks.AllowUserToDeleteRows = False
        Me.DataGridViewTracks.AllowUserToResizeRows = False
        Me.DataGridViewTracks.BackgroundColor = System.Drawing.SystemColors.Window
        Me.DataGridViewTracks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewTracks.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridViewTracks.Location = New System.Drawing.Point(0, 17)
        Me.DataGridViewTracks.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.DataGridViewTracks.MultiSelect = False
        Me.DataGridViewTracks.Name = "DataGridViewTracks"
        Me.DataGridViewTracks.ReadOnly = True
        Me.DataGridViewTracks.RowHeadersVisible = False
        Me.DataGridViewTracks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridViewTracks.Size = New System.Drawing.Size(300, 399)
        Me.DataGridViewTracks.TabIndex = 2
        '
        'DataGridViewSectors
        '
        Me.DataGridViewSectors.AllowUserToAddRows = False
        Me.DataGridViewSectors.AllowUserToDeleteRows = False
        Me.DataGridViewSectors.AllowUserToResizeRows = False
        Me.DataGridViewSectors.BackgroundColor = System.Drawing.SystemColors.Window
        Me.DataGridViewSectors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewSectors.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridViewSectors.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.DataGridViewSectors.Location = New System.Drawing.Point(316, 17)
        Me.DataGridViewSectors.Margin = New System.Windows.Forms.Padding(8, 0, 0, 0)
        Me.DataGridViewSectors.MultiSelect = False
        Me.DataGridViewSectors.Name = "DataGridViewSectors"
        Me.DataGridViewSectors.RowHeadersVisible = False
        Me.DataGridViewSectors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridViewSectors.Size = New System.Drawing.Size(364, 399)
        Me.DataGridViewSectors.TabIndex = 3
        '
        'TableLayoutPanelHeader
        '
        Me.TableLayoutPanelHeader.AutoSize = True
        Me.TableLayoutPanelHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanelHeader.ColumnCount = 4
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblVersion, 0, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtVersion, 1, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblFormat, 2, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtFormat, 3, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblComment, 0, 1)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtComment, 1, 1)
        Me.TableLayoutPanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanelHeader.Location = New System.Drawing.Point(12, 12)
        Me.TableLayoutPanelHeader.Name = "TableLayoutPanelHeader"
        Me.TableLayoutPanelHeader.RowCount = 2
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.Size = New System.Drawing.Size(680, 120)
        Me.TableLayoutPanelHeader.TabIndex = 0
        '
        'LblVersion
        '
        Me.LblVersion.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblVersion.AutoSize = True
        Me.LblVersion.Location = New System.Drawing.Point(3, 6)
        Me.LblVersion.Name = "LblVersion"
        Me.LblVersion.Size = New System.Drawing.Size(50, 13)
        Me.LblVersion.TabIndex = 0
        Me.LblVersion.Text = "{Version}"
        '
        'TxtVersion
        '
        Me.TxtVersion.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtVersion.Location = New System.Drawing.Point(68, 3)
        Me.TxtVersion.Name = "TxtVersion"
        Me.TxtVersion.ReadOnly = True
        Me.TxtVersion.Size = New System.Drawing.Size(100, 20)
        Me.TxtVersion.TabIndex = 1
        '
        'LblFormat
        '
        Me.LblFormat.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblFormat.AutoSize = True
        Me.LblFormat.Location = New System.Drawing.Point(187, 6)
        Me.LblFormat.Margin = New System.Windows.Forms.Padding(16, 0, 3, 0)
        Me.LblFormat.Name = "LblFormat"
        Me.LblFormat.Size = New System.Drawing.Size(47, 13)
        Me.LblFormat.TabIndex = 2
        Me.LblFormat.Text = "{Format}"
        '
        'TxtFormat
        '
        Me.TxtFormat.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtFormat.Location = New System.Drawing.Point(240, 3)
        Me.TxtFormat.Name = "TxtFormat"
        Me.TxtFormat.ReadOnly = True
        Me.TxtFormat.Size = New System.Drawing.Size(248, 20)
        Me.TxtFormat.TabIndex = 3
        '
        'LblComment
        '
        Me.LblComment.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblComment.AutoSize = True
        Me.LblComment.Location = New System.Drawing.Point(3, 66)
        Me.LblComment.Name = "LblComment"
        Me.LblComment.Size = New System.Drawing.Size(59, 13)
        Me.LblComment.TabIndex = 4
        Me.LblComment.Text = "{Comment}"
        '
        'TxtComment
        '
        Me.TxtComment.AcceptsReturn = True
        Me.TxtComment.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TableLayoutPanelHeader.SetColumnSpan(Me.TxtComment, 3)
        Me.TxtComment.Location = New System.Drawing.Point(68, 29)
        Me.TxtComment.Multiline = True
        Me.TxtComment.Name = "TxtComment"
        Me.TxtComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TxtComment.Size = New System.Drawing.Size(609, 88)
        Me.TxtComment.TabIndex = 5
        '
        'PSIImageForm
        '
        Me.AcceptButton = Me.BtnUpdate
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(704, 601)
        Me.Controls.Add(Me.PanelMain)
        Me.Controls.Add(Me.PanelBottom)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(720, 640)
        Me.Name = "PSIImageForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "{Image Properties}"
        Me.PanelBottom.ResumeLayout(False)
        Me.PanelMain.ResumeLayout(False)
        Me.PanelMain.PerformLayout()
        Me.TableLayoutPanelGrids.ResumeLayout(False)
        Me.TableLayoutPanelGrids.PerformLayout()
        CType(Me.DataGridViewTracks, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridViewSectors, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanelHeader.ResumeLayout(False)
        Me.TableLayoutPanelHeader.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents BtnCancel As Button
    Friend WithEvents BtnUpdate As Button
    Friend WithEvents TableLayoutPanelGrids As TableLayoutPanel
    Friend WithEvents LblTracks As Label
    Friend WithEvents LblSectors As Label
    Friend WithEvents DataGridViewTracks As DataGridView
    Friend WithEvents DataGridViewSectors As DataGridView
    Friend WithEvents TableLayoutPanelHeader As TableLayoutPanel
    Friend WithEvents LblVersion As Label
    Friend WithEvents TxtVersion As TextBox
    Friend WithEvents LblFormat As Label
    Friend WithEvents TxtFormat As TextBox
    Friend WithEvents LblComment As Label
    Friend WithEvents TxtComment As TextBox
    Friend WithEvents PanelBottom As Panel
    Friend WithEvents PanelMain As Panel
End Class
