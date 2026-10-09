<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TransCopyImageForm
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
        Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
        Dim PanelMain As System.Windows.Forms.Panel
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.DataGridViewTracks = New System.Windows.Forms.DataGridView()
        Me.TableLayoutPanelHeader = New System.Windows.Forms.TableLayoutPanel()
        Me.LblComment = New System.Windows.Forms.Label()
        Me.TxtComment = New System.Windows.Forms.TextBox()
        Me.LblComment2 = New System.Windows.Forms.Label()
        Me.TxtComment2 = New System.Windows.Forms.TextBox()
        Me.LblDiskType = New System.Windows.Forms.Label()
        Me.CboDiskType = New System.Windows.Forms.ComboBox()
        Me.LblTrackStart = New System.Windows.Forms.Label()
        Me.TxtTrackStart = New System.Windows.Forms.TextBox()
        Me.LblTrackEnd = New System.Windows.Forms.Label()
        Me.TxtTrackEnd = New System.Windows.Forms.TextBox()
        Me.LblSides = New System.Windows.Forms.Label()
        Me.TxtSides = New System.Windows.Forms.TextBox()
        Me.LblTrackIncrement = New System.Windows.Forms.Label()
        Me.TxtTrackIncrement = New System.Windows.Forms.TextBox()
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
        PanelBottom.Location = New System.Drawing.Point(0, 557)
        PanelBottom.Margin = New System.Windows.Forms.Padding(0)
        PanelBottom.Name = "PanelBottom"
        PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
        PanelBottom.Size = New System.Drawing.Size(984, 43)
        PanelBottom.TabIndex = 1
        PanelBottom.WrapContents = False
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(891, 10)
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
        Me.BtnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnUpdate.Location = New System.Drawing.Point(804, 10)
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
        PanelMain.Size = New System.Drawing.Size(984, 557)
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
        Me.DataGridViewTracks.Location = New System.Drawing.Point(12, 159)
        Me.DataGridViewTracks.MultiSelect = False
        Me.DataGridViewTracks.Name = "DataGridViewTracks"
        Me.DataGridViewTracks.ReadOnly = True
        Me.DataGridViewTracks.RowHeadersVisible = False
        Me.DataGridViewTracks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridViewTracks.Size = New System.Drawing.Size(960, 392)
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
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblComment, 0, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtComment, 1, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblComment2, 0, 1)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtComment2, 1, 1)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblDiskType, 0, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.CboDiskType, 1, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblTrackStart, 0, 3)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtTrackStart, 1, 3)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblTrackIncrement, 2, 4)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtTrackIncrement, 3, 4)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblSides, 0, 4)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblTrackEnd, 2, 3)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtSides, 1, 4)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtTrackEnd, 3, 3)
        Me.TableLayoutPanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanelHeader.Location = New System.Drawing.Point(12, 12)
        Me.TableLayoutPanelHeader.Name = "TableLayoutPanelHeader"
        Me.TableLayoutPanelHeader.Padding = New System.Windows.Forms.Padding(0, 0, 0, 16)
        Me.TableLayoutPanelHeader.RowCount = 5
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.Size = New System.Drawing.Size(960, 147)
        Me.TableLayoutPanelHeader.TabIndex = 0
        '
        'LblComment
        '
        Me.LblComment.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblComment.AutoSize = True
        Me.LblComment.Location = New System.Drawing.Point(3, 6)
        Me.LblComment.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblComment.Name = "LblComment"
        Me.LblComment.Size = New System.Drawing.Size(59, 13)
        Me.LblComment.TabIndex = 0
        Me.LblComment.Text = "{Comment}"
        '
        'TxtComment
        '
        Me.TxtComment.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanelHeader.SetColumnSpan(Me.TxtComment, 4)
        Me.TxtComment.Location = New System.Drawing.Point(96, 3)
        Me.TxtComment.MaxLength = 32
        Me.TxtComment.Name = "TxtComment"
        Me.TxtComment.Size = New System.Drawing.Size(861, 20)
        Me.TxtComment.TabIndex = 1
        '
        'LblComment2
        '
        Me.LblComment2.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblComment2.AutoSize = True
        Me.LblComment2.Location = New System.Drawing.Point(3, 32)
        Me.LblComment2.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblComment2.Name = "LblComment2"
        Me.LblComment2.Size = New System.Drawing.Size(68, 13)
        Me.LblComment2.TabIndex = 2
        Me.LblComment2.Text = "{Comment 2}"
        '
        'TxtComment2
        '
        Me.TxtComment2.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanelHeader.SetColumnSpan(Me.TxtComment2, 4)
        Me.TxtComment2.Location = New System.Drawing.Point(96, 29)
        Me.TxtComment2.MaxLength = 32
        Me.TxtComment2.Name = "TxtComment2"
        Me.TxtComment2.Size = New System.Drawing.Size(861, 20)
        Me.TxtComment2.TabIndex = 3
        '
        'LblDiskType
        '
        Me.LblDiskType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblDiskType.AutoSize = True
        Me.LblDiskType.Location = New System.Drawing.Point(3, 59)
        Me.LblDiskType.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblDiskType.Name = "LblDiskType"
        Me.LblDiskType.Size = New System.Drawing.Size(63, 13)
        Me.LblDiskType.TabIndex = 4
        Me.LblDiskType.Text = "{Disk Type}"
        '
        'CboDiskType
        '
        Me.CboDiskType.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanelHeader.SetColumnSpan(Me.CboDiskType, 3)
        Me.CboDiskType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CboDiskType.FormattingEnabled = True
        Me.CboDiskType.Location = New System.Drawing.Point(96, 55)
        Me.CboDiskType.Name = "CboDiskType"
        Me.CboDiskType.Size = New System.Drawing.Size(206, 21)
        Me.CboDiskType.TabIndex = 5
        '
        'LblTrackStart
        '
        Me.LblTrackStart.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblTrackStart.AutoSize = True
        Me.LblTrackStart.Location = New System.Drawing.Point(3, 85)
        Me.LblTrackStart.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblTrackStart.Name = "LblTrackStart"
        Me.LblTrackStart.Size = New System.Drawing.Size(82, 13)
        Me.LblTrackStart.TabIndex = 6
        Me.LblTrackStart.Text = "{Starting Track}"
        '
        'TxtTrackStart
        '
        Me.TxtTrackStart.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtTrackStart.Location = New System.Drawing.Point(96, 82)
        Me.TxtTrackStart.Name = "TxtTrackStart"
        Me.TxtTrackStart.ReadOnly = True
        Me.TxtTrackStart.Size = New System.Drawing.Size(48, 20)
        Me.TxtTrackStart.TabIndex = 7
        '
        'LblTrackEnd
        '
        Me.LblTrackEnd.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblTrackEnd.AutoSize = True
        Me.LblTrackEnd.Location = New System.Drawing.Point(150, 85)
        Me.LblTrackEnd.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblTrackEnd.Name = "LblTrackEnd"
        Me.LblTrackEnd.Size = New System.Drawing.Size(79, 13)
        Me.LblTrackEnd.TabIndex = 8
        Me.LblTrackEnd.Text = "{Ending Track}"
        '
        'TxtTrackEnd
        '
        Me.TxtTrackEnd.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtTrackEnd.Location = New System.Drawing.Point(254, 82)
        Me.TxtTrackEnd.Name = "TxtTrackEnd"
        Me.TxtTrackEnd.ReadOnly = True
        Me.TxtTrackEnd.Size = New System.Drawing.Size(48, 20)
        Me.TxtTrackEnd.TabIndex = 9
        '
        'LblSides
        '
        Me.LblSides.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSides.AutoSize = True
        Me.LblSides.Location = New System.Drawing.Point(3, 111)
        Me.LblSides.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblSides.Name = "LblSides"
        Me.LblSides.Size = New System.Drawing.Size(41, 13)
        Me.LblSides.TabIndex = 10
        Me.LblSides.Text = "{Sides}"
        '
        'TxtSides
        '
        Me.TxtSides.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtSides.Location = New System.Drawing.Point(96, 108)
        Me.TxtSides.Name = "TxtSides"
        Me.TxtSides.ReadOnly = True
        Me.TxtSides.Size = New System.Drawing.Size(48, 20)
        Me.TxtSides.TabIndex = 11
        '
        'LblTrackIncrement
        '
        Me.LblTrackIncrement.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblTrackIncrement.AutoSize = True
        Me.LblTrackIncrement.Location = New System.Drawing.Point(150, 111)
        Me.LblTrackIncrement.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblTrackIncrement.Name = "LblTrackIncrement"
        Me.LblTrackIncrement.Size = New System.Drawing.Size(93, 13)
        Me.LblTrackIncrement.TabIndex = 12
        Me.LblTrackIncrement.Text = "{Track Increment}"
        '
        'TxtTrackIncrement
        '
        Me.TxtTrackIncrement.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtTrackIncrement.Location = New System.Drawing.Point(254, 108)
        Me.TxtTrackIncrement.Name = "TxtTrackIncrement"
        Me.TxtTrackIncrement.ReadOnly = True
        Me.TxtTrackIncrement.Size = New System.Drawing.Size(48, 20)
        Me.TxtTrackIncrement.TabIndex = 13
        '
        'TransCopyImageForm
        '
        Me.AcceptButton = Me.BtnUpdate
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(984, 600)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(720, 480)
        Me.Name = "TransCopyImageForm"
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
    Friend WithEvents TableLayoutPanelHeader As TableLayoutPanel
    Friend WithEvents LblComment As Label
    Friend WithEvents TxtComment As TextBox
    Friend WithEvents LblComment2 As Label
    Friend WithEvents TxtComment2 As TextBox
    Friend WithEvents LblDiskType As Label
    Friend WithEvents CboDiskType As ComboBox
    Friend WithEvents LblTrackStart As Label
    Friend WithEvents TxtTrackStart As TextBox
    Friend WithEvents LblTrackEnd As Label
    Friend WithEvents TxtTrackEnd As TextBox
    Friend WithEvents LblSides As Label
    Friend WithEvents TxtSides As TextBox
    Friend WithEvents LblTrackIncrement As Label
    Friend WithEvents TxtTrackIncrement As TextBox
    Friend WithEvents DataGridViewTracks As DataGridView
End Class
