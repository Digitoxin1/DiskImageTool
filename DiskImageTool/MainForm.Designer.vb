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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.ListViewSummary = New System.Windows.Forms.ListView()
        Me.ComboGroups = New System.Windows.Forms.ComboBox()
        Me.ListViewHashes = New System.Windows.Forms.ListView()
        Me.LabelDropMessage = New System.Windows.Forms.Label()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripFileName = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripFileCount = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripModified = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ListViewFiles = New System.Windows.Forms.ListView()
        Me.MenuStripMain = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnOpen = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnSave = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnSaveAs = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnSaveAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnClose = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnCloseAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnExit = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnOEMID = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnClearCreated = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnClearLastAccessed = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnRevert = New System.Windows.Forms.ToolStripMenuItem()
        Me.FilterToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuFilters = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnScanNew = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnScan = New System.Windows.Forms.ToolStripMenuItem()
        Me.FilterSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.HexViewerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayDirectory = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayClusters = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnExportDebug = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.StatusStrip1.SuspendLayout()
        Me.MenuStripMain.SuspendLayout()
        Me.ContextMenuFilters.SuspendLayout()
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
        'ListViewSummary
        '
        Me.ListViewSummary.AllowDrop = True
        Me.ListViewSummary.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListViewSummary.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {SummaryName, SummaryValue})
        Me.ListViewSummary.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.ListViewSummary.HideSelection = False
        Me.ListViewSummary.Location = New System.Drawing.Point(12, 28)
        Me.ListViewSummary.MultiSelect = False
        Me.ListViewSummary.Name = "ListViewSummary"
        Me.ListViewSummary.ShowGroups = False
        Me.ListViewSummary.Size = New System.Drawing.Size(302, 414)
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
        Me.ComboGroups.Location = New System.Drawing.Point(320, 29)
        Me.ComboGroups.Name = "ComboGroups"
        Me.ComboGroups.Size = New System.Drawing.Size(642, 21)
        Me.ComboGroups.Sorted = True
        Me.ComboGroups.TabIndex = 3
        '
        'ListViewHashes
        '
        Me.ListViewHashes.AllowDrop = True
        Me.ListViewHashes.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListViewHashes.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {HashName, HashValue})
        Me.ListViewHashes.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListViewHashes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.ListViewHashes.HideSelection = False
        Me.ListViewHashes.Location = New System.Drawing.Point(12, 448)
        Me.ListViewHashes.MultiSelect = False
        Me.ListViewHashes.Name = "ListViewHashes"
        Me.ListViewHashes.Scrollable = False
        Me.ListViewHashes.Size = New System.Drawing.Size(302, 101)
        Me.ListViewHashes.TabIndex = 7
        Me.ListViewHashes.TileSize = New System.Drawing.Size(295, 30)
        Me.ListViewHashes.UseCompatibleStateImageBehavior = False
        Me.ListViewHashes.View = System.Windows.Forms.View.Tile
        '
        'LabelDropMessage
        '
        Me.LabelDropMessage.AllowDrop = True
        Me.LabelDropMessage.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.LabelDropMessage.AutoSize = True
        Me.LabelDropMessage.BackColor = System.Drawing.SystemColors.Window
        Me.LabelDropMessage.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelDropMessage.Location = New System.Drawing.Point(504, 288)
        Me.LabelDropMessage.Name = "LabelDropMessage"
        Me.LabelDropMessage.Size = New System.Drawing.Size(274, 16)
        Me.LabelDropMessage.TabIndex = 11
        Me.LabelDropMessage.Text = "Drag && Drop Floppy Disk Images Here"
        Me.LabelDropMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripFileName, Me.ToolStripFileCount, Me.ToolStripModified})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 552)
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
        Me.ListViewFiles.FullRowSelect = True
        Me.ListViewFiles.HideSelection = False
        Me.ListViewFiles.Location = New System.Drawing.Point(320, 56)
        Me.ListViewFiles.Name = "ListViewFiles"
        Me.ListViewFiles.OwnerDraw = True
        Me.ListViewFiles.Size = New System.Drawing.Size(642, 493)
        Me.ListViewFiles.TabIndex = 10
        Me.ListViewFiles.UseCompatibleStateImageBehavior = False
        Me.ListViewFiles.View = System.Windows.Forms.View.Details
        '
        'MenuStripMain
        '
        Me.MenuStripMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.EditToolStripMenuItem, Me.FilterToolStripMenuItem, Me.HexViewerToolStripMenuItem, Me.ToolsToolStripMenuItem})
        Me.MenuStripMain.Location = New System.Drawing.Point(0, 0)
        Me.MenuStripMain.Name = "MenuStripMain"
        Me.MenuStripMain.Size = New System.Drawing.Size(974, 24)
        Me.MenuStripMain.TabIndex = 14
        Me.MenuStripMain.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnOpen, Me.toolStripSeparator, Me.BtnSave, Me.BtnSaveAs, Me.BtnSaveAll, Me.ToolStripSeparator3, Me.BtnClose, Me.BtnCloseAll, Me.toolStripSeparator1, Me.BtnExit})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'BtnOpen
        '
        Me.BtnOpen.Image = CType(resources.GetObject("BtnOpen.Image"), System.Drawing.Image)
        Me.BtnOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.BtnOpen.Name = "BtnOpen"
        Me.BtnOpen.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.BtnOpen.Size = New System.Drawing.Size(146, 22)
        Me.BtnOpen.Text = "&Open"
        '
        'toolStripSeparator
        '
        Me.toolStripSeparator.Name = "toolStripSeparator"
        Me.toolStripSeparator.Size = New System.Drawing.Size(143, 6)
        '
        'BtnSave
        '
        Me.BtnSave.Image = CType(resources.GetObject("BtnSave.Image"), System.Drawing.Image)
        Me.BtnSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.BtnSave.Name = "BtnSave"
        Me.BtnSave.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.BtnSave.Size = New System.Drawing.Size(146, 22)
        Me.BtnSave.Text = "&Save"
        '
        'BtnSaveAs
        '
        Me.BtnSaveAs.Name = "BtnSaveAs"
        Me.BtnSaveAs.Size = New System.Drawing.Size(146, 22)
        Me.BtnSaveAs.Text = "Save &As"
        '
        'BtnSaveAll
        '
        Me.BtnSaveAll.Name = "BtnSaveAll"
        Me.BtnSaveAll.Size = New System.Drawing.Size(146, 22)
        Me.BtnSaveAll.Text = "Save All"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(143, 6)
        '
        'BtnClose
        '
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(146, 22)
        Me.BtnClose.Text = "&Close"
        '
        'BtnCloseAll
        '
        Me.BtnCloseAll.Name = "BtnCloseAll"
        Me.BtnCloseAll.Size = New System.Drawing.Size(146, 22)
        Me.BtnCloseAll.Text = "Close All"
        '
        'toolStripSeparator1
        '
        Me.toolStripSeparator1.Name = "toolStripSeparator1"
        Me.toolStripSeparator1.Size = New System.Drawing.Size(143, 6)
        '
        'BtnExit
        '
        Me.BtnExit.Name = "BtnExit"
        Me.BtnExit.Size = New System.Drawing.Size(146, 22)
        Me.BtnExit.Text = "E&xit"
        '
        'EditToolStripMenuItem
        '
        Me.EditToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnOEMID, Me.BtnClearCreated, Me.BtnClearLastAccessed, Me.ToolStripSeparator2, Me.BtnRevert})
        Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
        Me.EditToolStripMenuItem.Size = New System.Drawing.Size(39, 20)
        Me.EditToolStripMenuItem.Text = "&Edit"
        '
        'BtnOEMID
        '
        Me.BtnOEMID.Name = "BtnOEMID"
        Me.BtnOEMID.Size = New System.Drawing.Size(196, 22)
        Me.BtnOEMID.Text = "Change &OEM ID"
        '
        'BtnClearCreated
        '
        Me.BtnClearCreated.Name = "BtnClearCreated"
        Me.BtnClearCreated.Size = New System.Drawing.Size(196, 22)
        Me.BtnClearCreated.Text = "Clear &Creation Dates"
        '
        'BtnClearLastAccessed
        '
        Me.BtnClearLastAccessed.Name = "BtnClearLastAccessed"
        Me.BtnClearLastAccessed.Size = New System.Drawing.Size(196, 22)
        Me.BtnClearLastAccessed.Text = "Clear &Last Access Dates"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(193, 6)
        '
        'BtnRevert
        '
        Me.BtnRevert.Name = "BtnRevert"
        Me.BtnRevert.Size = New System.Drawing.Size(196, 22)
        Me.BtnRevert.Text = "&Revert Changes"
        Me.BtnRevert.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FilterToolStripMenuItem
        '
        Me.FilterToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.FilterToolStripMenuItem.DropDown = Me.ContextMenuFilters
        Me.FilterToolStripMenuItem.Name = "FilterToolStripMenuItem"
        Me.FilterToolStripMenuItem.Size = New System.Drawing.Size(50, 20)
        Me.FilterToolStripMenuItem.Text = "F&ilters"
        '
        'ContextMenuFilters
        '
        Me.ContextMenuFilters.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnScanNew, Me.BtnScan, Me.FilterSeparator})
        Me.ContextMenuFilters.Name = "ContextMenuStrip1"
        Me.ContextMenuFilters.OwnerItem = Me.FilterToolStripMenuItem
        Me.ContextMenuFilters.Size = New System.Drawing.Size(168, 54)
        '
        'BtnScanNew
        '
        Me.BtnScanNew.Name = "BtnScanNew"
        Me.BtnScanNew.Size = New System.Drawing.Size(167, 22)
        Me.BtnScanNew.Text = "Scan &New Images"
        '
        'BtnScan
        '
        Me.BtnScan.Name = "BtnScan"
        Me.BtnScan.Size = New System.Drawing.Size(167, 22)
        Me.BtnScan.Text = "&Scan Images"
        '
        'FilterSeparator
        '
        Me.FilterSeparator.Name = "FilterSeparator"
        Me.FilterSeparator.Size = New System.Drawing.Size(164, 6)
        Me.FilterSeparator.Visible = False
        '
        'HexViewerToolStripMenuItem
        '
        Me.HexViewerToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnDisplayBootSector, Me.BtnDisplayDirectory, Me.BtnDisplayClusters, Me.BtnDisplayFile})
        Me.HexViewerToolStripMenuItem.Name = "HexViewerToolStripMenuItem"
        Me.HexViewerToolStripMenuItem.Size = New System.Drawing.Size(78, 20)
        Me.HexViewerToolStripMenuItem.Text = "&Hex Viewer"
        '
        'BtnDisplayBootSector
        '
        Me.BtnDisplayBootSector.Name = "BtnDisplayBootSector"
        Me.BtnDisplayBootSector.Size = New System.Drawing.Size(159, 22)
        Me.BtnDisplayBootSector.Text = "&Boot Sector"
        '
        'BtnDisplayDirectory
        '
        Me.BtnDisplayDirectory.Name = "BtnDisplayDirectory"
        Me.BtnDisplayDirectory.Size = New System.Drawing.Size(159, 22)
        Me.BtnDisplayDirectory.Text = "&Root Directory"
        '
        'BtnDisplayClusters
        '
        Me.BtnDisplayClusters.Name = "BtnDisplayClusters"
        Me.BtnDisplayClusters.Size = New System.Drawing.Size(159, 22)
        Me.BtnDisplayClusters.Text = "&Unused Clusters"
        '
        'BtnDisplayFile
        '
        Me.BtnDisplayFile.Name = "BtnDisplayFile"
        Me.BtnDisplayFile.Size = New System.Drawing.Size(159, 22)
        Me.BtnDisplayFile.Text = "&File"
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnExportDebug})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(88, 20)
        Me.ToolsToolStripMenuItem.Text = "Experimental"
        '
        'BtnExportDebug
        '
        Me.BtnExportDebug.Name = "BtnExportDebug"
        Me.BtnExportDebug.Size = New System.Drawing.Size(225, 22)
        Me.BtnExportDebug.Text = "Generate Physical Disk Patch"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(974, 576)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStripMain)
        Me.Controls.Add(Me.LabelDropMessage)
        Me.Controls.Add(Me.ListViewHashes)
        Me.Controls.Add(Me.ComboGroups)
        Me.Controls.Add(Me.ListViewSummary)
        Me.Controls.Add(Me.ListViewFiles)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStripMain
        Me.MinimumSize = New System.Drawing.Size(640, 480)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Disk Image Tool"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.MenuStripMain.ResumeLayout(False)
        Me.MenuStripMain.PerformLayout()
        Me.ContextMenuFilters.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ListViewSummary As ListView
    Friend WithEvents ComboGroups As ComboBox
    Friend WithEvents ListViewHashes As ListView
    Friend WithEvents LabelDropMessage As Label
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripFileCount As ToolStripStatusLabel
    Friend WithEvents ToolStripFileName As ToolStripStatusLabel
    Friend WithEvents ToolStripModified As ToolStripStatusLabel
    Friend WithEvents ListViewFiles As ListView
    Friend WithEvents MenuStripMain As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BtnOpen As ToolStripMenuItem
    Friend WithEvents toolStripSeparator As ToolStripSeparator
    Friend WithEvents BtnSave As ToolStripMenuItem
    Friend WithEvents BtnSaveAs As ToolStripMenuItem
    Friend WithEvents toolStripSeparator1 As ToolStripSeparator
    Friend WithEvents BtnExit As ToolStripMenuItem
    Friend WithEvents FilterToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ContextMenuFilters As ContextMenuStrip
    Friend WithEvents BtnSaveAll As ToolStripMenuItem
    Friend WithEvents BtnScan As ToolStripMenuItem
    Friend WithEvents FilterSeparator As ToolStripSeparator
    Friend WithEvents HexViewerToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BtnDisplayBootSector As ToolStripMenuItem
    Friend WithEvents BtnDisplayDirectory As ToolStripMenuItem
    Friend WithEvents BtnDisplayClusters As ToolStripMenuItem
    Friend WithEvents EditToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BtnOEMID As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents BtnRevert As ToolStripMenuItem
    Friend WithEvents BtnClearCreated As ToolStripMenuItem
    Friend WithEvents BtnClearLastAccessed As ToolStripMenuItem
    Friend WithEvents BtnDisplayFile As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents BtnClose As ToolStripMenuItem
    Friend WithEvents BtnCloseAll As ToolStripMenuItem
    Friend WithEvents BtnScanNew As ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BtnExportDebug As ToolStripMenuItem
End Class
