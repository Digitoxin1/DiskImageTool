<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TD0ImageForm
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
        Me.TableLayoutPanelGrids = New System.Windows.Forms.TableLayoutPanel()
        Me.LblTracks = New System.Windows.Forms.Label()
        Me.LblSectors = New System.Windows.Forms.Label()
        Me.DataGridViewTracks = New System.Windows.Forms.DataGridView()
        Me.DataGridViewSectors = New System.Windows.Forms.DataGridView()
        Me.TableLayoutPanelHeader = New System.Windows.Forms.TableLayoutPanel()
        Me.LblDataRate = New System.Windows.Forms.Label()
        Me.TxtDataRate = New System.Windows.Forms.TextBox()
        Me.LblSingleDensity = New System.Windows.Forms.Label()
        Me.TxtSingleDensity = New System.Windows.Forms.TextBox()
        Me.LblComment = New System.Windows.Forms.Label()
        Me.TxtComment = New System.Windows.Forms.TextBox()
        Me.LblTimestamp = New System.Windows.Forms.Label()
        Me.DtpTimestamp = New System.Windows.Forms.DateTimePicker()
        Me.LblVersion = New System.Windows.Forms.Label()
        Me.LblCompression = New System.Windows.Forms.Label()
        Me.TxtVersion = New System.Windows.Forms.TextBox()
        Me.TxtCompression = New System.Windows.Forms.TextBox()
        Me.LblDriveType = New System.Windows.Forms.Label()
        Me.TxtDriveType = New System.Windows.Forms.TextBox()
        Me.LblCheckSequence = New System.Windows.Forms.Label()
        Me.LblSides = New System.Windows.Forms.Label()
        Me.TxtCheckSequence = New System.Windows.Forms.TextBox()
        Me.TxtSides = New System.Windows.Forms.TextBox()
        Me.LblStepping = New System.Windows.Forms.Label()
        Me.TxtStepping = New System.Windows.Forms.TextBox()
        Me.LblDosAllocation = New System.Windows.Forms.Label()
        Me.LblSequence = New System.Windows.Forms.Label()
        Me.TxtDosAllocation = New System.Windows.Forms.TextBox()
        Me.TxtSequence = New System.Windows.Forms.TextBox()
        PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        PanelMain = New System.Windows.Forms.Panel()
        PanelBottom.SuspendLayout()
        PanelMain.SuspendLayout()
        Me.TableLayoutPanelGrids.SuspendLayout()
        CType(Me.DataGridViewTracks, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridViewSectors, System.ComponentModel.ISupportInitialize).BeginInit()
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
        PanelBottom.Size = New System.Drawing.Size(809, 43)
        PanelBottom.TabIndex = 1
        PanelBottom.WrapContents = False
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(716, 10)
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
        Me.BtnUpdate.Location = New System.Drawing.Point(629, 10)
        Me.BtnUpdate.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.Size = New System.Drawing.Size(75, 23)
        Me.BtnUpdate.TabIndex = 1
        Me.BtnUpdate.Text = "{&Update}"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'PanelMain
        '
        PanelMain.Controls.Add(Me.TableLayoutPanelGrids)
        PanelMain.Controls.Add(Me.TableLayoutPanelHeader)
        PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        PanelMain.Location = New System.Drawing.Point(0, 0)
        PanelMain.Name = "PanelMain"
        PanelMain.Padding = New System.Windows.Forms.Padding(12, 12, 12, 6)
        PanelMain.Size = New System.Drawing.Size(809, 518)
        PanelMain.TabIndex = 0
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
        Me.TableLayoutPanelGrids.Location = New System.Drawing.Point(12, 262)
        Me.TableLayoutPanelGrids.Name = "TableLayoutPanelGrids"
        Me.TableLayoutPanelGrids.RowCount = 2
        Me.TableLayoutPanelGrids.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelGrids.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanelGrids.Size = New System.Drawing.Size(785, 250)
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
        Me.LblSectors.Location = New System.Drawing.Point(312, 0)
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
        Me.DataGridViewTracks.Size = New System.Drawing.Size(292, 233)
        Me.DataGridViewTracks.TabIndex = 1
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
        Me.DataGridViewSectors.Location = New System.Drawing.Point(308, 17)
        Me.DataGridViewSectors.Margin = New System.Windows.Forms.Padding(8, 0, 0, 0)
        Me.DataGridViewSectors.MultiSelect = False
        Me.DataGridViewSectors.Name = "DataGridViewSectors"
        Me.DataGridViewSectors.RowHeadersVisible = False
        Me.DataGridViewSectors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridViewSectors.Size = New System.Drawing.Size(477, 233)
        Me.DataGridViewSectors.TabIndex = 2
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
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblDataRate, 0, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtDataRate, 1, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblSingleDensity, 2, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtSingleDensity, 3, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblComment, 0, 5)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtComment, 1, 5)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblTimestamp, 0, 6)
        Me.TableLayoutPanelHeader.Controls.Add(Me.DtpTimestamp, 1, 6)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblVersion, 0, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblCompression, 2, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtVersion, 1, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtCompression, 3, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblDriveType, 0, 1)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtDriveType, 1, 1)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblCheckSequence, 2, 4)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblSides, 2, 1)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtCheckSequence, 3, 4)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtSides, 3, 1)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblStepping, 0, 3)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtStepping, 1, 3)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblDosAllocation, 2, 3)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblSequence, 0, 4)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtDosAllocation, 3, 3)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtSequence, 1, 4)
        Me.TableLayoutPanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanelHeader.Location = New System.Drawing.Point(12, 12)
        Me.TableLayoutPanelHeader.Name = "TableLayoutPanelHeader"
        Me.TableLayoutPanelHeader.Padding = New System.Windows.Forms.Padding(0, 0, 0, 16)
        Me.TableLayoutPanelHeader.RowCount = 7
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.Size = New System.Drawing.Size(785, 250)
        Me.TableLayoutPanelHeader.TabIndex = 0
        '
        'LblDataRate
        '
        Me.LblDataRate.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblDataRate.AutoSize = True
        Me.LblDataRate.Location = New System.Drawing.Point(3, 58)
        Me.LblDataRate.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblDataRate.Name = "LblDataRate"
        Me.LblDataRate.Size = New System.Drawing.Size(64, 13)
        Me.LblDataRate.TabIndex = 8
        Me.LblDataRate.Text = "{Data Rate}"
        '
        'TxtDataRate
        '
        Me.TxtDataRate.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtDataRate.Location = New System.Drawing.Point(81, 55)
        Me.TxtDataRate.Name = "TxtDataRate"
        Me.TxtDataRate.ReadOnly = True
        Me.TxtDataRate.Size = New System.Drawing.Size(100, 20)
        Me.TxtDataRate.TabIndex = 9
        '
        'LblSingleDensity
        '
        Me.LblSingleDensity.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSingleDensity.AutoSize = True
        Me.LblSingleDensity.Location = New System.Drawing.Point(247, 58)
        Me.LblSingleDensity.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblSingleDensity.Name = "LblSingleDensity"
        Me.LblSingleDensity.Size = New System.Drawing.Size(82, 13)
        Me.LblSingleDensity.TabIndex = 10
        Me.LblSingleDensity.Text = "{Single Density}"
        '
        'TxtSingleDensity
        '
        Me.TxtSingleDensity.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtSingleDensity.Location = New System.Drawing.Point(356, 55)
        Me.TxtSingleDensity.Name = "TxtSingleDensity"
        Me.TxtSingleDensity.ReadOnly = True
        Me.TxtSingleDensity.Size = New System.Drawing.Size(100, 20)
        Me.TxtSingleDensity.TabIndex = 11
        '
        'LblComment
        '
        Me.LblComment.AutoSize = True
        Me.LblComment.Location = New System.Drawing.Point(3, 136)
        Me.LblComment.Margin = New System.Windows.Forms.Padding(3, 6, 8, 0)
        Me.LblComment.Name = "LblComment"
        Me.LblComment.Size = New System.Drawing.Size(59, 13)
        Me.LblComment.TabIndex = 20
        Me.LblComment.Text = "{Comment}"
        '
        'TxtComment
        '
        Me.TxtComment.AcceptsReturn = True
        Me.TxtComment.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanelHeader.SetColumnSpan(Me.TxtComment, 3)
        Me.TxtComment.Location = New System.Drawing.Point(81, 133)
        Me.TxtComment.Multiline = True
        Me.TxtComment.Name = "TxtComment"
        Me.TxtComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TxtComment.Size = New System.Drawing.Size(701, 72)
        Me.TxtComment.TabIndex = 21
        '
        'LblTimestamp
        '
        Me.LblTimestamp.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblTimestamp.AutoSize = True
        Me.LblTimestamp.Location = New System.Drawing.Point(3, 214)
        Me.LblTimestamp.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblTimestamp.Name = "LblTimestamp"
        Me.LblTimestamp.Size = New System.Drawing.Size(66, 13)
        Me.LblTimestamp.TabIndex = 22
        Me.LblTimestamp.Text = "{Timestamp}"
        '
        'DtpTimestamp
        '
        Me.DtpTimestamp.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TableLayoutPanelHeader.SetColumnSpan(Me.DtpTimestamp, 3)
        Me.DtpTimestamp.CustomFormat = "yyyy-MM-dd HH:mm:ss"
        Me.DtpTimestamp.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DtpTimestamp.Location = New System.Drawing.Point(81, 211)
        Me.DtpTimestamp.Name = "DtpTimestamp"
        Me.DtpTimestamp.ShowCheckBox = True
        Me.DtpTimestamp.ShowUpDown = True
        Me.DtpTimestamp.Size = New System.Drawing.Size(180, 20)
        Me.DtpTimestamp.TabIndex = 23
        '
        'LblVersion
        '
        Me.LblVersion.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblVersion.AutoSize = True
        Me.LblVersion.Location = New System.Drawing.Point(3, 6)
        Me.LblVersion.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblVersion.Name = "LblVersion"
        Me.LblVersion.Size = New System.Drawing.Size(50, 13)
        Me.LblVersion.TabIndex = 0
        Me.LblVersion.Text = "{Version}"
        '
        'LblCompression
        '
        Me.LblCompression.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblCompression.AutoSize = True
        Me.LblCompression.Location = New System.Drawing.Point(247, 6)
        Me.LblCompression.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblCompression.Name = "LblCompression"
        Me.LblCompression.Size = New System.Drawing.Size(75, 13)
        Me.LblCompression.TabIndex = 2
        Me.LblCompression.Text = "{Compression}"
        '
        'TxtVersion
        '
        Me.TxtVersion.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtVersion.Location = New System.Drawing.Point(81, 3)
        Me.TxtVersion.Name = "TxtVersion"
        Me.TxtVersion.ReadOnly = True
        Me.TxtVersion.Size = New System.Drawing.Size(100, 20)
        Me.TxtVersion.TabIndex = 1
        '
        'TxtCompression
        '
        Me.TxtCompression.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtCompression.Location = New System.Drawing.Point(356, 3)
        Me.TxtCompression.Name = "TxtCompression"
        Me.TxtCompression.ReadOnly = True
        Me.TxtCompression.Size = New System.Drawing.Size(160, 20)
        Me.TxtCompression.TabIndex = 3
        '
        'LblDriveType
        '
        Me.LblDriveType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblDriveType.AutoSize = True
        Me.LblDriveType.Location = New System.Drawing.Point(3, 32)
        Me.LblDriveType.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblDriveType.Name = "LblDriveType"
        Me.LblDriveType.Size = New System.Drawing.Size(67, 13)
        Me.LblDriveType.TabIndex = 4
        Me.LblDriveType.Text = "{Drive Type}"
        '
        'TxtDriveType
        '
        Me.TxtDriveType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtDriveType.Location = New System.Drawing.Point(81, 29)
        Me.TxtDriveType.Name = "TxtDriveType"
        Me.TxtDriveType.ReadOnly = True
        Me.TxtDriveType.Size = New System.Drawing.Size(160, 20)
        Me.TxtDriveType.TabIndex = 5
        '
        'LblCheckSequence
        '
        Me.LblCheckSequence.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblCheckSequence.AutoSize = True
        Me.LblCheckSequence.Location = New System.Drawing.Point(247, 110)
        Me.LblCheckSequence.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblCheckSequence.Name = "LblCheckSequence"
        Me.LblCheckSequence.Size = New System.Drawing.Size(98, 13)
        Me.LblCheckSequence.TabIndex = 18
        Me.LblCheckSequence.Text = "{Check Sequence}"
        '
        'LblSides
        '
        Me.LblSides.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSides.AutoSize = True
        Me.LblSides.Location = New System.Drawing.Point(247, 32)
        Me.LblSides.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblSides.Name = "LblSides"
        Me.LblSides.Size = New System.Drawing.Size(41, 13)
        Me.LblSides.TabIndex = 6
        Me.LblSides.Text = "{Sides}"
        '
        'TxtCheckSequence
        '
        Me.TxtCheckSequence.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtCheckSequence.Location = New System.Drawing.Point(356, 107)
        Me.TxtCheckSequence.Name = "TxtCheckSequence"
        Me.TxtCheckSequence.ReadOnly = True
        Me.TxtCheckSequence.Size = New System.Drawing.Size(100, 20)
        Me.TxtCheckSequence.TabIndex = 19
        '
        'TxtSides
        '
        Me.TxtSides.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtSides.Location = New System.Drawing.Point(356, 29)
        Me.TxtSides.Name = "TxtSides"
        Me.TxtSides.ReadOnly = True
        Me.TxtSides.Size = New System.Drawing.Size(100, 20)
        Me.TxtSides.TabIndex = 7
        '
        'LblStepping
        '
        Me.LblStepping.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblStepping.AutoSize = True
        Me.LblStepping.Location = New System.Drawing.Point(3, 84)
        Me.LblStepping.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblStepping.Name = "LblStepping"
        Me.LblStepping.Size = New System.Drawing.Size(57, 13)
        Me.LblStepping.TabIndex = 12
        Me.LblStepping.Text = "{Stepping}"
        '
        'TxtStepping
        '
        Me.TxtStepping.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtStepping.Location = New System.Drawing.Point(81, 81)
        Me.TxtStepping.Name = "TxtStepping"
        Me.TxtStepping.ReadOnly = True
        Me.TxtStepping.Size = New System.Drawing.Size(100, 20)
        Me.TxtStepping.TabIndex = 13
        '
        'LblDosAllocation
        '
        Me.LblDosAllocation.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblDosAllocation.AutoSize = True
        Me.LblDosAllocation.Location = New System.Drawing.Point(247, 84)
        Me.LblDosAllocation.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblDosAllocation.Name = "LblDosAllocation"
        Me.LblDosAllocation.Size = New System.Drawing.Size(87, 13)
        Me.LblDosAllocation.TabIndex = 14
        Me.LblDosAllocation.Text = "{DOS Allocation}"
        '
        'LblSequence
        '
        Me.LblSequence.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSequence.AutoSize = True
        Me.LblSequence.Location = New System.Drawing.Point(3, 110)
        Me.LblSequence.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblSequence.Name = "LblSequence"
        Me.LblSequence.Size = New System.Drawing.Size(64, 13)
        Me.LblSequence.TabIndex = 16
        Me.LblSequence.Text = "{Sequence}"
        '
        'TxtDosAllocation
        '
        Me.TxtDosAllocation.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtDosAllocation.Location = New System.Drawing.Point(356, 81)
        Me.TxtDosAllocation.Name = "TxtDosAllocation"
        Me.TxtDosAllocation.ReadOnly = True
        Me.TxtDosAllocation.Size = New System.Drawing.Size(100, 20)
        Me.TxtDosAllocation.TabIndex = 15
        '
        'TxtSequence
        '
        Me.TxtSequence.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtSequence.Location = New System.Drawing.Point(81, 107)
        Me.TxtSequence.Name = "TxtSequence"
        Me.TxtSequence.ReadOnly = True
        Me.TxtSequence.Size = New System.Drawing.Size(100, 20)
        Me.TxtSequence.TabIndex = 17
        '
        'TD0ImageForm
        '
        Me.AcceptButton = Me.BtnUpdate
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(809, 561)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(720, 480)
        Me.Name = "TD0ImageForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "{Image Properties}"
        PanelBottom.ResumeLayout(False)
        PanelMain.ResumeLayout(False)
        PanelMain.PerformLayout()
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
    Friend WithEvents DataGridViewTracks As DataGridView
    Friend WithEvents TableLayoutPanelGrids As TableLayoutPanel
    Friend WithEvents LblTracks As Label
    Friend WithEvents LblSectors As Label
    Friend WithEvents DataGridViewSectors As DataGridView
    Friend WithEvents TableLayoutPanelHeader As TableLayoutPanel
    Friend WithEvents LblCompression As Label
    Friend WithEvents TxtCompression As TextBox
    Friend WithEvents LblVersion As Label
    Friend WithEvents TxtVersion As TextBox
    Friend WithEvents LblSequence As Label
    Friend WithEvents TxtSequence As TextBox
    Friend WithEvents LblCheckSequence As Label
    Friend WithEvents TxtCheckSequence As TextBox
    Friend WithEvents LblDataRate As Label
    Friend WithEvents TxtDataRate As TextBox
    Friend WithEvents LblSingleDensity As Label
    Friend WithEvents TxtSingleDensity As TextBox
    Friend WithEvents LblDriveType As Label
    Friend WithEvents TxtDriveType As TextBox
    Friend WithEvents LblStepping As Label
    Friend WithEvents TxtStepping As TextBox
    Friend WithEvents LblDosAllocation As Label
    Friend WithEvents TxtDosAllocation As TextBox
    Friend WithEvents LblSides As Label
    Friend WithEvents TxtSides As TextBox
    Friend WithEvents LblComment As Label
    Friend WithEvents TxtComment As TextBox
    Friend WithEvents LblTimestamp As Label
    Friend WithEvents DtpTimestamp As DateTimePicker
End Class
