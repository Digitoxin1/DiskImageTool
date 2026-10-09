<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class D86FImageForm
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
        Me.LblSignature = New System.Windows.Forms.Label()
        Me.TxtSignature = New System.Windows.Forms.TextBox()
        Me.LblSides = New System.Windows.Forms.Label()
        Me.TxtSides = New System.Windows.Forms.TextBox()
        Me.LblDiskType = New System.Windows.Forms.Label()
        Me.TxtDiskType = New System.Windows.Forms.TextBox()
        Me.LblVersion = New System.Windows.Forms.Label()
        Me.TxtVersion = New System.Windows.Forms.TextBox()
        Me.LblZoneType = New System.Windows.Forms.Label()
        Me.TxtZoneType = New System.Windows.Forms.TextBox()
        Me.LblBitCellMode = New System.Windows.Forms.Label()
        Me.TxtBitCellMode = New System.Windows.Forms.TextBox()
        Me.LblAlternateBitCell = New System.Windows.Forms.Label()
        Me.TxtAlternateBitCell = New System.Windows.Forms.TextBox()
        Me.LblHole = New System.Windows.Forms.Label()
        Me.CboHole = New System.Windows.Forms.ComboBox()
        Me.LblRPMSlowdown = New System.Windows.Forms.Label()
        Me.TxtRPMSlowdown = New System.Windows.Forms.TextBox()
        Me.TxtReverseEndian = New System.Windows.Forms.TextBox()
        Me.LblReverseEndian = New System.Windows.Forms.Label()
        Me.LblSurfaceData = New System.Windows.Forms.Label()
        Me.TxtSurfaceData = New System.Windows.Forms.TextBox()
        Me.LblWriteProtect = New System.Windows.Forms.Label()
        Me.CboWriteProtect = New System.Windows.Forms.ComboBox()
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
        PanelBottom.Location = New System.Drawing.Point(0, 558)
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
        PanelMain.Size = New System.Drawing.Size(704, 558)
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
        Me.DataGridViewTracks.Location = New System.Drawing.Point(12, 186)
        Me.DataGridViewTracks.MultiSelect = False
        Me.DataGridViewTracks.Name = "DataGridViewTracks"
        Me.DataGridViewTracks.ReadOnly = True
        Me.DataGridViewTracks.RowHeadersVisible = False
        Me.DataGridViewTracks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridViewTracks.Size = New System.Drawing.Size(680, 366)
        Me.DataGridViewTracks.TabIndex = 1
        '
        'TableLayoutPanelHeader
        '
        Me.TableLayoutPanelHeader.AutoSize = True
        Me.TableLayoutPanelHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanelHeader.ColumnCount = 4
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblSignature, 0, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtSignature, 1, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblSides, 0, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtSides, 1, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblDiskType, 0, 10)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtDiskType, 1, 10)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblVersion, 2, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtVersion, 3, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblZoneType, 2, 10)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtZoneType, 3, 10)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblBitCellMode, 0, 5)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtBitCellMode, 1, 5)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblAlternateBitCell, 2, 5)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtAlternateBitCell, 3, 5)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblHole, 2, 6)
        Me.TableLayoutPanelHeader.Controls.Add(Me.CboHole, 3, 6)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblRPMSlowdown, 0, 6)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtRPMSlowdown, 1, 6)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtReverseEndian, 3, 8)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblReverseEndian, 2, 8)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblSurfaceData, 0, 8)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtSurfaceData, 1, 8)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblWriteProtect, 2, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.CboWriteProtect, 3, 2)
        Me.TableLayoutPanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanelHeader.Location = New System.Drawing.Point(12, 12)
        Me.TableLayoutPanelHeader.Name = "TableLayoutPanelHeader"
        Me.TableLayoutPanelHeader.Padding = New System.Windows.Forms.Padding(0, 0, 0, 16)
        Me.TableLayoutPanelHeader.RowCount = 12
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.Size = New System.Drawing.Size(680, 174)
        Me.TableLayoutPanelHeader.TabIndex = 0
        '
        'LblSignature
        '
        Me.LblSignature.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSignature.AutoSize = True
        Me.LblSignature.Location = New System.Drawing.Point(3, 6)
        Me.LblSignature.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblSignature.Name = "LblSignature"
        Me.LblSignature.Size = New System.Drawing.Size(60, 13)
        Me.LblSignature.TabIndex = 0
        Me.LblSignature.Text = "{Signature}"
        '
        'TxtSignature
        '
        Me.TxtSignature.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtSignature.Location = New System.Drawing.Point(105, 3)
        Me.TxtSignature.Name = "TxtSignature"
        Me.TxtSignature.ReadOnly = True
        Me.TxtSignature.Size = New System.Drawing.Size(95, 20)
        Me.TxtSignature.TabIndex = 1
        '
        'LblSides
        '
        Me.LblSides.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSides.AutoSize = True
        Me.LblSides.Location = New System.Drawing.Point(3, 33)
        Me.LblSides.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblSides.Name = "LblSides"
        Me.LblSides.Size = New System.Drawing.Size(41, 13)
        Me.LblSides.TabIndex = 4
        Me.LblSides.Text = "{Sides}"
        '
        'TxtSides
        '
        Me.TxtSides.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtSides.Location = New System.Drawing.Point(105, 29)
        Me.TxtSides.Name = "TxtSides"
        Me.TxtSides.ReadOnly = True
        Me.TxtSides.Size = New System.Drawing.Size(95, 20)
        Me.TxtSides.TabIndex = 5
        '
        'LblDiskType
        '
        Me.LblDiskType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblDiskType.AutoSize = True
        Me.LblDiskType.Location = New System.Drawing.Point(3, 138)
        Me.LblDiskType.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblDiskType.Name = "LblDiskType"
        Me.LblDiskType.Size = New System.Drawing.Size(63, 13)
        Me.LblDiskType.TabIndex = 20
        Me.LblDiskType.Text = "{Disk Type}"
        '
        'TxtDiskType
        '
        Me.TxtDiskType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtDiskType.Location = New System.Drawing.Point(105, 135)
        Me.TxtDiskType.Name = "TxtDiskType"
        Me.TxtDiskType.ReadOnly = True
        Me.TxtDiskType.Size = New System.Drawing.Size(95, 20)
        Me.TxtDiskType.TabIndex = 21
        '
        'LblVersion
        '
        Me.LblVersion.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblVersion.AutoSize = True
        Me.LblVersion.Location = New System.Drawing.Point(206, 6)
        Me.LblVersion.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblVersion.Name = "LblVersion"
        Me.LblVersion.Size = New System.Drawing.Size(50, 13)
        Me.LblVersion.TabIndex = 2
        Me.LblVersion.Text = "{Version}"
        '
        'TxtVersion
        '
        Me.TxtVersion.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtVersion.Location = New System.Drawing.Point(329, 3)
        Me.TxtVersion.Name = "TxtVersion"
        Me.TxtVersion.ReadOnly = True
        Me.TxtVersion.Size = New System.Drawing.Size(95, 20)
        Me.TxtVersion.TabIndex = 3
        '
        'LblZoneType
        '
        Me.LblZoneType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblZoneType.AutoSize = True
        Me.LblZoneType.Location = New System.Drawing.Point(206, 138)
        Me.LblZoneType.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblZoneType.Name = "LblZoneType"
        Me.LblZoneType.Size = New System.Drawing.Size(67, 13)
        Me.LblZoneType.TabIndex = 22
        Me.LblZoneType.Text = "{Zone Type}"
        '
        'TxtZoneType
        '
        Me.TxtZoneType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtZoneType.Location = New System.Drawing.Point(329, 135)
        Me.TxtZoneType.Name = "TxtZoneType"
        Me.TxtZoneType.ReadOnly = True
        Me.TxtZoneType.Size = New System.Drawing.Size(95, 20)
        Me.TxtZoneType.TabIndex = 23
        '
        'LblBitCellMode
        '
        Me.LblBitCellMode.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblBitCellMode.AutoSize = True
        Me.LblBitCellMode.Location = New System.Drawing.Point(3, 59)
        Me.LblBitCellMode.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblBitCellMode.Name = "LblBitCellMode"
        Me.LblBitCellMode.Size = New System.Drawing.Size(77, 13)
        Me.LblBitCellMode.TabIndex = 8
        Me.LblBitCellMode.Text = "{Bit Cell Mode}"
        '
        'TxtBitCellMode
        '
        Me.TxtBitCellMode.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtBitCellMode.Location = New System.Drawing.Point(105, 56)
        Me.TxtBitCellMode.Name = "TxtBitCellMode"
        Me.TxtBitCellMode.ReadOnly = True
        Me.TxtBitCellMode.Size = New System.Drawing.Size(95, 20)
        Me.TxtBitCellMode.TabIndex = 9
        '
        'LblAlternateBitCell
        '
        Me.LblAlternateBitCell.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblAlternateBitCell.AutoSize = True
        Me.LblAlternateBitCell.Location = New System.Drawing.Point(206, 59)
        Me.LblAlternateBitCell.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblAlternateBitCell.Name = "LblAlternateBitCell"
        Me.LblAlternateBitCell.Size = New System.Drawing.Size(112, 13)
        Me.LblAlternateBitCell.TabIndex = 10
        Me.LblAlternateBitCell.Text = "{Alternate Calculation}"
        '
        'TxtAlternateBitCell
        '
        Me.TxtAlternateBitCell.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtAlternateBitCell.Location = New System.Drawing.Point(329, 56)
        Me.TxtAlternateBitCell.Name = "TxtAlternateBitCell"
        Me.TxtAlternateBitCell.ReadOnly = True
        Me.TxtAlternateBitCell.Size = New System.Drawing.Size(95, 20)
        Me.TxtAlternateBitCell.TabIndex = 11
        '
        'LblHole
        '
        Me.LblHole.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblHole.AutoSize = True
        Me.LblHole.Location = New System.Drawing.Point(206, 86)
        Me.LblHole.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblHole.Name = "LblHole"
        Me.LblHole.Size = New System.Drawing.Size(37, 13)
        Me.LblHole.TabIndex = 14
        Me.LblHole.Text = "{Hole}"
        '
        'CboHole
        '
        Me.CboHole.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.CboHole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CboHole.FormattingEnabled = True
        Me.CboHole.Location = New System.Drawing.Point(329, 82)
        Me.CboHole.Name = "CboHole"
        Me.CboHole.Size = New System.Drawing.Size(95, 21)
        Me.CboHole.TabIndex = 15
        '
        'LblRPMSlowdown
        '
        Me.LblRPMSlowdown.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblRPMSlowdown.AutoSize = True
        Me.LblRPMSlowdown.Location = New System.Drawing.Point(3, 86)
        Me.LblRPMSlowdown.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblRPMSlowdown.Name = "LblRPMSlowdown"
        Me.LblRPMSlowdown.Size = New System.Drawing.Size(91, 13)
        Me.LblRPMSlowdown.TabIndex = 12
        Me.LblRPMSlowdown.Text = "{RPM Slowdown}"
        '
        'TxtRPMSlowdown
        '
        Me.TxtRPMSlowdown.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtRPMSlowdown.Location = New System.Drawing.Point(105, 82)
        Me.TxtRPMSlowdown.Name = "TxtRPMSlowdown"
        Me.TxtRPMSlowdown.ReadOnly = True
        Me.TxtRPMSlowdown.Size = New System.Drawing.Size(95, 20)
        Me.TxtRPMSlowdown.TabIndex = 13
        '
        'TxtReverseEndian
        '
        Me.TxtReverseEndian.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtReverseEndian.Location = New System.Drawing.Point(329, 109)
        Me.TxtReverseEndian.Name = "TxtReverseEndian"
        Me.TxtReverseEndian.ReadOnly = True
        Me.TxtReverseEndian.Size = New System.Drawing.Size(95, 20)
        Me.TxtReverseEndian.TabIndex = 19
        '
        'LblReverseEndian
        '
        Me.LblReverseEndian.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblReverseEndian.AutoSize = True
        Me.LblReverseEndian.Location = New System.Drawing.Point(206, 112)
        Me.LblReverseEndian.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblReverseEndian.Name = "LblReverseEndian"
        Me.LblReverseEndian.Size = New System.Drawing.Size(91, 13)
        Me.LblReverseEndian.TabIndex = 18
        Me.LblReverseEndian.Text = "{Reverse Endian}"
        '
        'LblSurfaceData
        '
        Me.LblSurfaceData.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSurfaceData.AutoSize = True
        Me.LblSurfaceData.Location = New System.Drawing.Point(3, 112)
        Me.LblSurfaceData.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblSurfaceData.Name = "LblSurfaceData"
        Me.LblSurfaceData.Size = New System.Drawing.Size(78, 13)
        Me.LblSurfaceData.TabIndex = 16
        Me.LblSurfaceData.Text = "{Surface Data}"
        '
        'TxtSurfaceData
        '
        Me.TxtSurfaceData.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtSurfaceData.Location = New System.Drawing.Point(105, 109)
        Me.TxtSurfaceData.Name = "TxtSurfaceData"
        Me.TxtSurfaceData.ReadOnly = True
        Me.TxtSurfaceData.Size = New System.Drawing.Size(95, 20)
        Me.TxtSurfaceData.TabIndex = 17
        '
        'LblWriteProtect
        '
        Me.LblWriteProtect.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblWriteProtect.AutoSize = True
        Me.LblWriteProtect.Location = New System.Drawing.Point(206, 33)
        Me.LblWriteProtect.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblWriteProtect.Name = "LblWriteProtect"
        Me.LblWriteProtect.Size = New System.Drawing.Size(77, 13)
        Me.LblWriteProtect.TabIndex = 6
        Me.LblWriteProtect.Text = "{Write Protect}"
        '
        'CboWriteProtect
        '
        Me.CboWriteProtect.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.CboWriteProtect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CboWriteProtect.FormattingEnabled = True
        Me.CboWriteProtect.Location = New System.Drawing.Point(329, 29)
        Me.CboWriteProtect.Name = "CboWriteProtect"
        Me.CboWriteProtect.Size = New System.Drawing.Size(95, 21)
        Me.CboWriteProtect.TabIndex = 7
        '
        'D86FImageForm
        '
        Me.AcceptButton = Me.BtnUpdate
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(704, 601)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(720, 640)
        Me.Name = "D86FImageForm"
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
    Friend WithEvents LblSignature As Label
    Friend WithEvents TxtSignature As TextBox
    Friend WithEvents LblVersion As Label
    Friend WithEvents TxtVersion As TextBox
    Friend WithEvents LblSides As Label
    Friend WithEvents TxtSides As TextBox
    Friend WithEvents LblHole As Label
    Friend WithEvents CboHole As ComboBox
    Friend WithEvents LblWriteProtect As Label
    Friend WithEvents CboWriteProtect As ComboBox
    Friend WithEvents LblRPMSlowdown As Label
    Friend WithEvents TxtRPMSlowdown As TextBox
    Friend WithEvents LblBitCellMode As Label
    Friend WithEvents TxtBitCellMode As TextBox
    Friend WithEvents LblAlternateBitCell As Label
    Friend WithEvents TxtAlternateBitCell As TextBox
    Friend WithEvents LblReverseEndian As Label
    Friend WithEvents TxtReverseEndian As TextBox
    Friend WithEvents LblSurfaceData As Label
    Friend WithEvents TxtSurfaceData As TextBox
    Friend WithEvents LblDiskType As Label
    Friend WithEvents TxtDiskType As TextBox
    Friend WithEvents LblZoneType As Label
    Friend WithEvents TxtZoneType As TextBox
End Class
