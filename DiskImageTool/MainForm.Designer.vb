<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
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
        Dim SummaryName As System.Windows.Forms.ColumnHeader
        Dim SummaryValue As System.Windows.Forms.ColumnHeader
        Dim HashName As System.Windows.Forms.ColumnHeader
        Dim HashValue As System.Windows.Forms.ColumnHeader
        Dim ColumnHeader1 As System.Windows.Forms.ColumnHeader
        Dim FileName As System.Windows.Forms.ColumnHeader
        Dim FileExt As System.Windows.Forms.ColumnHeader
        Dim FileSize As System.Windows.Forms.ColumnHeader
        Dim FileLastWriteDate As System.Windows.Forms.ColumnHeader
        Dim FileStartingCluster As System.Windows.Forms.ColumnHeader
        Dim FileAttrib As System.Windows.Forms.ColumnHeader
        Dim FileCRC32 As System.Windows.Forms.ColumnHeader
        Me.ContextMenuFiles = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ItemDisplayDirectory = New System.Windows.Forms.ToolStripMenuItem()
        Me.ListViewSummary = New System.Windows.Forms.ListView()
        Me.ComboGroups = New System.Windows.Forms.ComboBox()
        Me.LblInvalidImage = New System.Windows.Forms.Label()
        Me.ListViewHashes = New System.Windows.Forms.ListView()
        Me.PanelButtons = New System.Windows.Forms.Panel()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.ButtonDisplayBootSector = New System.Windows.Forms.Button()
        Me.ButtonDisplayClusters = New System.Windows.Forms.Button()
        Me.ButtonOEMID = New System.Windows.Forms.Button()
        Me.BtnClearCreated = New System.Windows.Forms.Button()
        Me.BtnClearLastAccessed = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.BtnSave = New System.Windows.Forms.Button()
        Me.LabelDropMessage = New System.Windows.Forms.Label()
        Me.CBCheckAll = New System.Windows.Forms.CheckBox()
        Me.BtnScan = New System.Windows.Forms.Button()
        Me.BtnFilters = New System.Windows.Forms.Button()
        Me.ListFilters = New System.Windows.Forms.CheckedListBox()
        Me.LblFilterMessage = New System.Windows.Forms.Label()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripFileName = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripFileCount = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripModified = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ListViewFiles = New System.Windows.Forms.ListView()
        SummaryName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        SummaryValue = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HashName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HashValue = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileExt = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileSize = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileLastWriteDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileStartingCluster = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileAttrib = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileCRC32 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ContextMenuFiles.SuspendLayout()
        Me.PanelButtons.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'SummaryName
        '
        SummaryName.Text = "Name"
        SummaryName.Width = 130
        '
        'SummaryValue
        '
        SummaryValue.Text = "Value"
        SummaryValue.Width = 140
        '
        'HashName
        '
        HashName.Width = -1
        '
        'HashValue
        '
        HashValue.Width = -1
        '
        'ColumnHeader1
        '
        ColumnHeader1.Text = ""
        ColumnHeader1.Width = 23
        '
        'FileName
        '
        FileName.Text = "Name"
        FileName.Width = 120
        '
        'FileExt
        '
        FileExt.Text = "Ext"
        FileExt.Width = 50
        '
        'FileSize
        '
        FileSize.Text = "Size"
        FileSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        FileSize.Width = 80
        '
        'FileLastWriteDate
        '
        FileLastWriteDate.Text = "Last Written"
        FileLastWriteDate.Width = 120
        '
        'FileStartingCluster
        '
        FileStartingCluster.Text = "Cluster"
        FileStartingCluster.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FileAttrib
        '
        FileAttrib.Text = "Attrib"
        FileAttrib.Width = 75
        '
        'FileCRC32
        '
        FileCRC32.Text = "CRC32"
        FileCRC32.Width = 70
        '
        'ContextMenuFiles
        '
        Me.ContextMenuFiles.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ItemDisplayDirectory})
        Me.ContextMenuFiles.Name = "ContextMenuFiles"
        Me.ContextMenuFiles.Size = New System.Drawing.Size(164, 26)
        '
        'ItemDisplayDirectory
        '
        Me.ItemDisplayDirectory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ItemDisplayDirectory.Name = "ItemDisplayDirectory"
        Me.ItemDisplayDirectory.Size = New System.Drawing.Size(163, 22)
        Me.ItemDisplayDirectory.Text = "Display Directory"
        '
        'ListViewSummary
        '
        Me.ListViewSummary.AllowDrop = True
        Me.ListViewSummary.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListViewSummary.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {SummaryName, SummaryValue})
        Me.ListViewSummary.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.ListViewSummary.HideSelection = False
        Me.ListViewSummary.Location = New System.Drawing.Point(12, 41)
        Me.ListViewSummary.MultiSelect = False
        Me.ListViewSummary.Name = "ListViewSummary"
        Me.ListViewSummary.ShowGroups = False
        Me.ListViewSummary.Size = New System.Drawing.Size(302, 211)
        Me.ListViewSummary.TabIndex = 5
        Me.ListViewSummary.UseCompatibleStateImageBehavior = False
        Me.ListViewSummary.View = System.Windows.Forms.View.Details
        '
        'ComboGroups
        '
        Me.ComboGroups.AllowDrop = True
        Me.ComboGroups.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboGroups.DropDownWidth = 523
        Me.ComboGroups.FormattingEnabled = True
        Me.ComboGroups.Location = New System.Drawing.Point(320, 12)
        Me.ComboGroups.Name = "ComboGroups"
        Me.ComboGroups.Size = New System.Drawing.Size(550, 21)
        Me.ComboGroups.Sorted = True
        Me.ComboGroups.TabIndex = 3
        '
        'LblInvalidImage
        '
        Me.LblInvalidImage.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.LblInvalidImage.AutoSize = True
        Me.LblInvalidImage.BackColor = System.Drawing.SystemColors.Window
        Me.LblInvalidImage.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblInvalidImage.ForeColor = System.Drawing.Color.Red
        Me.LblInvalidImage.Location = New System.Drawing.Point(86, 132)
        Me.LblInvalidImage.Name = "LblInvalidImage"
        Me.LblInvalidImage.Size = New System.Drawing.Size(155, 29)
        Me.LblInvalidImage.TabIndex = 6
        Me.LblInvalidImage.Text = "Invalid Image"
        Me.LblInvalidImage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.LblInvalidImage.Visible = False
        '
        'ListViewHashes
        '
        Me.ListViewHashes.AllowDrop = True
        Me.ListViewHashes.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListViewHashes.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {HashName, HashValue})
        Me.ListViewHashes.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListViewHashes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.ListViewHashes.HideSelection = False
        Me.ListViewHashes.Location = New System.Drawing.Point(12, 258)
        Me.ListViewHashes.MultiSelect = False
        Me.ListViewHashes.Name = "ListViewHashes"
        Me.ListViewHashes.Scrollable = False
        Me.ListViewHashes.Size = New System.Drawing.Size(302, 101)
        Me.ListViewHashes.TabIndex = 7
        Me.ListViewHashes.TileSize = New System.Drawing.Size(295, 30)
        Me.ListViewHashes.UseCompatibleStateImageBehavior = False
        Me.ListViewHashes.View = System.Windows.Forms.View.Tile
        '
        'PanelButtons
        '
        Me.PanelButtons.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PanelButtons.BackColor = System.Drawing.SystemColors.Window
        Me.PanelButtons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelButtons.Controls.Add(Me.FlowLayoutPanel1)
        Me.PanelButtons.Location = New System.Drawing.Point(12, 365)
        Me.PanelButtons.Name = "PanelButtons"
        Me.PanelButtons.Size = New System.Drawing.Size(302, 157)
        Me.PanelButtons.TabIndex = 8
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel1.Controls.Add(Me.ButtonDisplayBootSector)
        Me.FlowLayoutPanel1.Controls.Add(Me.ButtonDisplayClusters)
        Me.FlowLayoutPanel1.Controls.Add(Me.ButtonOEMID)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnClearCreated)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnClearLastAccessed)
        Me.FlowLayoutPanel1.Controls.Add(Me.Button1)
        Me.FlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(280, 145)
        Me.FlowLayoutPanel1.TabIndex = 0
        '
        'ButtonDisplayBootSector
        '
        Me.ButtonDisplayBootSector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ButtonDisplayBootSector.Location = New System.Drawing.Point(3, 3)
        Me.ButtonDisplayBootSector.Name = "ButtonDisplayBootSector"
        Me.ButtonDisplayBootSector.Size = New System.Drawing.Size(134, 23)
        Me.ButtonDisplayBootSector.TabIndex = 0
        Me.ButtonDisplayBootSector.Text = "Display Boot Sector"
        Me.ButtonDisplayBootSector.UseVisualStyleBackColor = True
        '
        'ButtonDisplayClusters
        '
        Me.ButtonDisplayClusters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ButtonDisplayClusters.Location = New System.Drawing.Point(3, 32)
        Me.ButtonDisplayClusters.Name = "ButtonDisplayClusters"
        Me.ButtonDisplayClusters.Size = New System.Drawing.Size(134, 23)
        Me.ButtonDisplayClusters.TabIndex = 1
        Me.ButtonDisplayClusters.Text = "Display Unused Clusters"
        Me.ButtonDisplayClusters.UseVisualStyleBackColor = True
        '
        'ButtonOEMID
        '
        Me.ButtonOEMID.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ButtonOEMID.Location = New System.Drawing.Point(3, 61)
        Me.ButtonOEMID.Name = "ButtonOEMID"
        Me.ButtonOEMID.Size = New System.Drawing.Size(134, 23)
        Me.ButtonOEMID.TabIndex = 2
        Me.ButtonOEMID.Text = "Change OEM ID"
        Me.ButtonOEMID.UseVisualStyleBackColor = True
        '
        'BtnClearCreated
        '
        Me.BtnClearCreated.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnClearCreated.Location = New System.Drawing.Point(3, 90)
        Me.BtnClearCreated.Name = "BtnClearCreated"
        Me.BtnClearCreated.Size = New System.Drawing.Size(134, 23)
        Me.BtnClearCreated.TabIndex = 3
        Me.BtnClearCreated.Text = "Clear Creation Date"
        Me.BtnClearCreated.UseVisualStyleBackColor = True
        '
        'BtnClearLastAccessed
        '
        Me.BtnClearLastAccessed.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnClearLastAccessed.Location = New System.Drawing.Point(3, 119)
        Me.BtnClearLastAccessed.Name = "BtnClearLastAccessed"
        Me.BtnClearLastAccessed.Size = New System.Drawing.Size(134, 23)
        Me.BtnClearLastAccessed.TabIndex = 4
        Me.BtnClearLastAccessed.Text = "Clear Last Access Date"
        Me.BtnClearLastAccessed.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(143, 3)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(134, 23)
        Me.Button1.TabIndex = 5
        Me.Button1.Text = "Load All"
        Me.Button1.UseVisualStyleBackColor = True
        Me.Button1.Visible = False
        '
        'BtnSave
        '
        Me.BtnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnSave.Enabled = False
        Me.BtnSave.Location = New System.Drawing.Point(876, 11)
        Me.BtnSave.Name = "BtnSave"
        Me.BtnSave.Size = New System.Drawing.Size(86, 23)
        Me.BtnSave.TabIndex = 4
        Me.BtnSave.Text = "Save Changes"
        Me.BtnSave.UseVisualStyleBackColor = True
        '
        'LabelDropMessage
        '
        Me.LabelDropMessage.AllowDrop = True
        Me.LabelDropMessage.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.LabelDropMessage.AutoSize = True
        Me.LabelDropMessage.BackColor = System.Drawing.SystemColors.Window
        Me.LabelDropMessage.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelDropMessage.Location = New System.Drawing.Point(504, 273)
        Me.LabelDropMessage.Name = "LabelDropMessage"
        Me.LabelDropMessage.Size = New System.Drawing.Size(274, 16)
        Me.LabelDropMessage.TabIndex = 11
        Me.LabelDropMessage.Text = "Drag && Drop Floppy Disk Images Here"
        Me.LabelDropMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'CBCheckAll
        '
        Me.CBCheckAll.AutoSize = True
        Me.CBCheckAll.Location = New System.Drawing.Point(326, 48)
        Me.CBCheckAll.Name = "CBCheckAll"
        Me.CBCheckAll.Size = New System.Drawing.Size(15, 14)
        Me.CBCheckAll.TabIndex = 9
        Me.CBCheckAll.UseVisualStyleBackColor = True
        '
        'BtnScan
        '
        Me.BtnScan.Location = New System.Drawing.Point(12, 11)
        Me.BtnScan.Name = "BtnScan"
        Me.BtnScan.Size = New System.Drawing.Size(86, 23)
        Me.BtnScan.TabIndex = 0
        Me.BtnScan.Text = "Scan Images"
        Me.BtnScan.UseVisualStyleBackColor = True
        '
        'BtnFilters
        '
        Me.BtnFilters.Location = New System.Drawing.Point(104, 11)
        Me.BtnFilters.Name = "BtnFilters"
        Me.BtnFilters.Size = New System.Drawing.Size(86, 23)
        Me.BtnFilters.TabIndex = 1
        Me.BtnFilters.Text = "Filters"
        Me.BtnFilters.UseVisualStyleBackColor = True
        '
        'ListFilters
        '
        Me.ListFilters.CheckOnClick = True
        Me.ListFilters.FormattingEnabled = True
        Me.ListFilters.Location = New System.Drawing.Point(104, 34)
        Me.ListFilters.Name = "ListFilters"
        Me.ListFilters.Size = New System.Drawing.Size(197, 79)
        Me.ListFilters.TabIndex = 2
        Me.ListFilters.Visible = False
        '
        'LblFilterMessage
        '
        Me.LblFilterMessage.AutoSize = True
        Me.LblFilterMessage.Location = New System.Drawing.Point(196, 16)
        Me.LblFilterMessage.Name = "LblFilterMessage"
        Me.LblFilterMessage.Size = New System.Drawing.Size(90, 13)
        Me.LblFilterMessage.TabIndex = 12
        Me.LblFilterMessage.Text = "Scanning... 100%"
        Me.LblFilterMessage.UseMnemonic = False
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripFileName, Me.ToolStripFileCount, Me.ToolStripModified})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 523)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(974, 24)
        Me.StatusStrip1.TabIndex = 13
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripFileName
        '
        Me.ToolStripFileName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripFileName.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripFileName.Name = "ToolStripFileName"
        Me.ToolStripFileName.Size = New System.Drawing.Size(810, 19)
        Me.ToolStripFileName.Spring = True
        Me.ToolStripFileName.Text = "File Name"
        Me.ToolStripFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToolStripFileCount
        '
        Me.ToolStripFileCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripFileCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripFileCount.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripFileCount.Name = "ToolStripFileCount"
        Me.ToolStripFileCount.Size = New System.Drawing.Size(43, 19)
        Me.ToolStripFileCount.Text = "0 Files"
        Me.ToolStripFileCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ToolStripModified
        '
        Me.ToolStripModified.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripModified.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripModified.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripModified.Name = "ToolStripModified"
        Me.ToolStripModified.Size = New System.Drawing.Size(94, 19)
        Me.ToolStripModified.Text = "0 Files Modified"
        '
        'ListViewFiles
        '
        Me.ListViewFiles.AllowDrop = True
        Me.ListViewFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListViewFiles.CheckBoxes = True
        Me.ListViewFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {ColumnHeader1, FileName, FileExt, FileSize, FileLastWriteDate, FileStartingCluster, FileAttrib, FileCRC32})
        Me.ListViewFiles.ContextMenuStrip = Me.ContextMenuFiles
        Me.ListViewFiles.FullRowSelect = True
        Me.ListViewFiles.HideSelection = False
        Me.ListViewFiles.Location = New System.Drawing.Point(320, 41)
        Me.ListViewFiles.Name = "ListViewFiles"
        Me.ListViewFiles.Size = New System.Drawing.Size(642, 481)
        Me.ListViewFiles.TabIndex = 10
        Me.ListViewFiles.UseCompatibleStateImageBehavior = False
        Me.ListViewFiles.View = System.Windows.Forms.View.Details
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(974, 547)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.LblFilterMessage)
        Me.Controls.Add(Me.ListFilters)
        Me.Controls.Add(Me.BtnFilters)
        Me.Controls.Add(Me.BtnScan)
        Me.Controls.Add(Me.CBCheckAll)
        Me.Controls.Add(Me.LabelDropMessage)
        Me.Controls.Add(Me.BtnSave)
        Me.Controls.Add(Me.PanelButtons)
        Me.Controls.Add(Me.ListViewHashes)
        Me.Controls.Add(Me.LblInvalidImage)
        Me.Controls.Add(Me.ComboGroups)
        Me.Controls.Add(Me.ListViewSummary)
        Me.Controls.Add(Me.ListViewFiles)
        Me.MinimumSize = New System.Drawing.Size(640, 480)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Disk Image Tool"
        Me.ContextMenuFiles.ResumeLayout(False)
        Me.PanelButtons.ResumeLayout(False)
        Me.PanelButtons.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ListViewSummary As ListView
    Friend WithEvents ComboGroups As ComboBox
    Friend WithEvents LblInvalidImage As Label
    Friend WithEvents ListViewHashes As ListView
    Friend WithEvents PanelButtons As Panel
    Friend WithEvents BtnSave As Button
    Friend WithEvents LabelDropMessage As Label
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents ButtonOEMID As Button
    Friend WithEvents ButtonDisplayBootSector As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents BtnClearCreated As Button
    Friend WithEvents CBCheckAll As CheckBox
    Friend WithEvents BtnClearLastAccessed As Button
    Friend WithEvents BtnScan As Button
    Friend WithEvents BtnFilters As Button
    Friend WithEvents ListFilters As CheckedListBox
    Friend WithEvents LblFilterMessage As Label
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripFileCount As ToolStripStatusLabel
    Friend WithEvents ToolStripFileName As ToolStripStatusLabel
    Friend WithEvents ToolStripModified As ToolStripStatusLabel
    Friend WithEvents ContextMenuFiles As ContextMenuStrip
    Friend WithEvents ItemDisplayDirectory As ToolStripMenuItem
    Friend WithEvents ButtonDisplayClusters As Button
    Friend WithEvents ListViewFiles As ListView
End Class
