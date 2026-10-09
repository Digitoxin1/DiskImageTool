<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PRIImageForm
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
        Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
        Dim PanelMain As System.Windows.Forms.Panel
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.DataGridViewTracks = New System.Windows.Forms.DataGridView()
        Me.TableLayoutPanelHeader = New System.Windows.Forms.TableLayoutPanel()
        Me.LblVersion = New System.Windows.Forms.Label()
        Me.TxtVersion = New System.Windows.Forms.TextBox()
        Me.LblReserved = New System.Windows.Forms.Label()
        Me.TxtReserved = New System.Windows.Forms.TextBox()
        Me.LblComment = New System.Windows.Forms.Label()
        Me.TxtComment = New System.Windows.Forms.TextBox()
        PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        PanelMain = New System.Windows.Forms.Panel()
        PanelBottom.SuspendLayout()
        PanelMain.SuspendLayout()
        CType(Me.DataGridViewTracks, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanelHeader.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelBottom
        '
        PanelBottom.Controls.Add(Me.BtnCancel)
        PanelBottom.Controls.Add(Me.BtnUpdate)
        PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        PanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        PanelBottom.Location = New System.Drawing.Point(0, 518)
        PanelBottom.Margin = New System.Windows.Forms.Padding(0)
        PanelBottom.Name = "PanelBottom"
        PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
        PanelBottom.Size = New System.Drawing.Size(704, 43)
        PanelBottom.TabIndex = 1
        PanelBottom.WrapContents = False
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(611, 10)
        Me.BtnCancel.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
        Me.BtnCancel.TabIndex = 0
        Me.BtnCancel.Text = "{&Cancel}"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnUpdate
        '
        Me.BtnUpdate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnUpdate.Location = New System.Drawing.Point(524, 10)
        Me.BtnUpdate.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.Size = New System.Drawing.Size(75, 23)
        Me.BtnUpdate.TabIndex = 1
        Me.BtnUpdate.Text = "{&Update}"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'PanelMain
        '
        PanelMain.Controls.Add(Me.DataGridViewTracks)
        PanelMain.Controls.Add(Me.TableLayoutPanelHeader)
        PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        PanelMain.Location = New System.Drawing.Point(0, 0)
        PanelMain.Name = "PanelMain"
        PanelMain.Padding = New System.Windows.Forms.Padding(12, 12, 12, 6)
        PanelMain.Size = New System.Drawing.Size(704, 518)
        PanelMain.TabIndex = 0
        '
        'DataGridViewTracks
        '
        Me.DataGridViewTracks.AllowUserToAddRows = False
        Me.DataGridViewTracks.AllowUserToDeleteRows = False
        Me.DataGridViewTracks.AllowUserToResizeRows = False
        Me.DataGridViewTracks.BackgroundColor = System.Drawing.SystemColors.Window
        Me.DataGridViewTracks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewTracks.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridViewTracks.Location = New System.Drawing.Point(12, 156)
        Me.DataGridViewTracks.MultiSelect = False
        Me.DataGridViewTracks.Name = "DataGridViewTracks"
        Me.DataGridViewTracks.ReadOnly = True
        Me.DataGridViewTracks.RowHeadersVisible = False
        Me.DataGridViewTracks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridViewTracks.Size = New System.Drawing.Size(680, 356)
        Me.DataGridViewTracks.TabIndex = 1
        '
        'TableLayoutPanelHeader
        '
        Me.TableLayoutPanelHeader.AutoSize = True
        Me.TableLayoutPanelHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanelHeader.ColumnCount = 5
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblVersion, 0, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtVersion, 1, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblReserved, 2, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtReserved, 3, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblComment, 0, 1)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtComment, 1, 1)
        Me.TableLayoutPanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanelHeader.Location = New System.Drawing.Point(12, 12)
        Me.TableLayoutPanelHeader.Name = "TableLayoutPanelHeader"
        Me.TableLayoutPanelHeader.Padding = New System.Windows.Forms.Padding(0, 0, 0, 16)
        Me.TableLayoutPanelHeader.RowCount = 2
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.Size = New System.Drawing.Size(680, 144)
        Me.TableLayoutPanelHeader.TabIndex = 0
        '
        'LblVersion
        '
        Me.LblVersion.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblVersion.AutoSize = True
        Me.LblVersion.Location = New System.Drawing.Point(3, 6)
        Me.LblVersion.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblVersion.Name = "LblVersion"
        Me.LblVersion.Size = New System.Drawing.Size(42, 13)
        Me.LblVersion.TabIndex = 0
        Me.LblVersion.Text = "{Version}"
        '
        'TxtVersion
        '
        Me.TxtVersion.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtVersion.Location = New System.Drawing.Point(68, 3)
        Me.TxtVersion.Name = "TxtVersion"
        Me.TxtVersion.ReadOnly = True
        Me.TxtVersion.Size = New System.Drawing.Size(64, 20)
        Me.TxtVersion.TabIndex = 1
        '
        'LblReserved
        '
        Me.LblReserved.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblReserved.AutoSize = True
        Me.LblReserved.Location = New System.Drawing.Point(190, 6)
        Me.LblReserved.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblReserved.Name = "LblReserved"
        Me.LblReserved.Size = New System.Drawing.Size(53, 13)
        Me.LblReserved.TabIndex = 2
        Me.LblReserved.Text = "{Reserved}"
        '
        'TxtReserved
        '
        Me.TxtReserved.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtReserved.Location = New System.Drawing.Point(249, 3)
        Me.TxtReserved.Name = "TxtReserved"
        Me.TxtReserved.ReadOnly = True
        Me.TxtReserved.Size = New System.Drawing.Size(64, 20)
        Me.TxtReserved.TabIndex = 3
        '
        'LblComment
        '
        Me.LblComment.AutoSize = True
        Me.LblComment.Location = New System.Drawing.Point(3, 32)
        Me.LblComment.Margin = New System.Windows.Forms.Padding(3, 6, 8, 0)
        Me.LblComment.Name = "LblComment"
        Me.LblComment.Size = New System.Drawing.Size(59, 13)
        Me.LblComment.TabIndex = 4
        Me.LblComment.Text = "{Comment}"
        '
        'TxtComment
        '
        Me.TableLayoutPanelHeader.SetColumnSpan(Me.TxtComment, 4)
        Me.TxtComment.AcceptsReturn = True
        Me.TxtComment.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtComment.Location = New System.Drawing.Point(70, 29)
        Me.TxtComment.Multiline = True
        Me.TxtComment.Name = "TxtComment"
        Me.TxtComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TxtComment.Size = New System.Drawing.Size(607, 72)
        Me.TxtComment.TabIndex = 5
        '
        'PRIImageForm
        '
        Me.AcceptButton = Me.BtnUpdate
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(704, 561)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(720, 480)
        Me.Name = "PRIImageForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "{Image Properties}"
        PanelBottom.ResumeLayout(False)
        PanelMain.ResumeLayout(False)
        PanelMain.PerformLayout()
        CType(Me.DataGridViewTracks, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanelHeader.ResumeLayout(False)
        Me.TableLayoutPanelHeader.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents BtnCancel As Button
    Friend WithEvents BtnUpdate As Button
    Friend WithEvents DataGridViewTracks As DataGridView
    Friend WithEvents TableLayoutPanelHeader As TableLayoutPanel
    Friend WithEvents LblVersion As Label
    Friend WithEvents TxtVersion As TextBox
    Friend WithEvents LblReserved As Label
    Friend WithEvents TxtReserved As TextBox
    Friend WithEvents LblComment As Label
    Friend WithEvents TxtComment As TextBox
End Class
