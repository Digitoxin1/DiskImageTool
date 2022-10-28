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
        Dim FileName As System.Windows.Forms.ColumnHeader
        Dim FileExt As System.Windows.Forms.ColumnHeader
        Dim FileSize As System.Windows.Forms.ColumnHeader
        Dim FileLastWriteDate As System.Windows.Forms.ColumnHeader
        Dim FileStartingCluster As System.Windows.Forms.ColumnHeader
        Dim FileAttrib As System.Windows.Forms.ColumnHeader
        Dim FileModified As System.Windows.Forms.ColumnHeader
        Dim MainMenuFile As System.Windows.Forms.ToolStripMenuItem
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Dim MainMenuEdit As System.Windows.Forms.ToolStripMenuItem
        Dim MainMenuView As System.Windows.Forms.ToolStripMenuItem
        Dim MainMenuExperimental As System.Windows.Forms.ToolStripMenuItem
        Dim ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
        Dim FileCRC32 As System.Windows.Forms.ColumnHeader
        Dim ToolStripSearch As System.Windows.Forms.ToolStripLabel
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
        Me.BtnOEMID = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnFileProperties = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnExportFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnReplaceFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnRevert = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayFAT = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayDirectory = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayClusters = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnExportDebug = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripOEMID = New System.Windows.Forms.ToolStripLabel()
        Me.ListViewSummary = New System.Windows.Forms.ListView()
        Me.ComboImages = New System.Windows.Forms.ComboBox()
        Me.ListViewHashes = New System.Windows.Forms.ListView()
        Me.LabelDropMessage = New System.Windows.Forms.Label()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripFileName = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripFileCount = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripImageCount = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripModified = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ListViewFiles = New System.Windows.Forms.ListView()
        Me.FileClusterError = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileCreateDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileLastAccessDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileLFN = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ContextMenuFiles = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnFileMenuFileProperties = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuExportFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuReplaceFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuViewFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuViewFileText = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuViewCrosslinked = New System.Windows.Forms.ToolStripMenuItem()
        Me.FileMenuSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnFileMenuUndo = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStripMain = New System.Windows.Forms.MenuStrip()
        Me.MainMenuFilters = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuFilters = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnScanNew = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnScan = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnClearFilters = New System.Windows.Forms.ToolStripMenuItem()
        Me.FilterSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.TxtSearch = New System.Windows.Forms.ToolStripTextBox()
        Me.ComboOEMID = New System.Windows.Forms.ToolStripComboBox()
        Me.ComboImagesFiltered = New System.Windows.Forms.ComboBox()
        SummaryName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        SummaryValue = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HashName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HashValue = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileExt = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileSize = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileLastWriteDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileStartingCluster = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileAttrib = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileModified = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        MainMenuFile = New System.Windows.Forms.ToolStripMenuItem()
        MainMenuEdit = New System.Windows.Forms.ToolStripMenuItem()
        MainMenuView = New System.Windows.Forms.ToolStripMenuItem()
        MainMenuExperimental = New System.Windows.Forms.ToolStripMenuItem()
        ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        FileCRC32 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        ToolStripSearch = New System.Windows.Forms.ToolStripLabel()
        Me.StatusStrip1.SuspendLayout()
        Me.ContextMenuFiles.SuspendLayout()
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
        'FileModified
        '
        FileModified.Text = ""
        FileModified.Width = 20
        '
        'MainMenuFile
        '
        MainMenuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnOpen, Me.toolStripSeparator, Me.BtnSave, Me.BtnSaveAs, Me.BtnSaveAll, Me.ToolStripSeparator3, Me.BtnClose, Me.BtnCloseAll, Me.toolStripSeparator1, Me.BtnExit})
        MainMenuFile.Name = "MainMenuFile"
        MainMenuFile.Size = New System.Drawing.Size(37, 23)
        MainMenuFile.Text = "&File"
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
        'MainMenuEdit
        '
        MainMenuEdit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnOEMID, Me.ToolStripSeparator4, Me.BtnFileProperties, Me.BtnExportFile, Me.BtnReplaceFile, Me.ToolStripSeparator2, Me.BtnRevert})
        MainMenuEdit.Name = "MainMenuEdit"
        MainMenuEdit.Size = New System.Drawing.Size(39, 23)
        MainMenuEdit.Text = "&Edit"
        '
        'BtnOEMID
        '
        Me.BtnOEMID.Name = "BtnOEMID"
        Me.BtnOEMID.Size = New System.Drawing.Size(158, 22)
        Me.BtnOEMID.Text = "Change &OEM ID"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(155, 6)
        '
        'BtnFileProperties
        '
        Me.BtnFileProperties.Name = "BtnFileProperties"
        Me.BtnFileProperties.Size = New System.Drawing.Size(158, 22)
        Me.BtnFileProperties.Text = "File &Properties"
        '
        'BtnExportFile
        '
        Me.BtnExportFile.Name = "BtnExportFile"
        Me.BtnExportFile.Size = New System.Drawing.Size(158, 22)
        Me.BtnExportFile.Text = "&Export File"
        '
        'BtnReplaceFile
        '
        Me.BtnReplaceFile.Name = "BtnReplaceFile"
        Me.BtnReplaceFile.Size = New System.Drawing.Size(158, 22)
        Me.BtnReplaceFile.Text = "&Replace File"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(155, 6)
        '
        'BtnRevert
        '
        Me.BtnRevert.Name = "BtnRevert"
        Me.BtnRevert.Size = New System.Drawing.Size(158, 22)
        Me.BtnRevert.Text = "&Revert Changes"
        Me.BtnRevert.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MainMenuView
        '
        MainMenuView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnDisplayBootSector, Me.BtnDisplayFAT, Me.BtnDisplayDirectory, Me.BtnDisplayClusters, Me.BtnDisplayFile})
        MainMenuView.Name = "MainMenuView"
        MainMenuView.Size = New System.Drawing.Size(44, 23)
        MainMenuView.Text = "&View"
        '
        'BtnDisplayBootSector
        '
        Me.BtnDisplayBootSector.Name = "BtnDisplayBootSector"
        Me.BtnDisplayBootSector.Size = New System.Drawing.Size(179, 22)
        Me.BtnDisplayBootSector.Text = "&Boot Sector"
        '
        'BtnDisplayFAT
        '
        Me.BtnDisplayFAT.Name = "BtnDisplayFAT"
        Me.BtnDisplayFAT.Size = New System.Drawing.Size(179, 22)
        Me.BtnDisplayFAT.Text = "File &Allocation Table"
        '
        'BtnDisplayDirectory
        '
        Me.BtnDisplayDirectory.Name = "BtnDisplayDirectory"
        Me.BtnDisplayDirectory.Size = New System.Drawing.Size(179, 22)
        Me.BtnDisplayDirectory.Text = "&Root Directory"
        '
        'BtnDisplayClusters
        '
        Me.BtnDisplayClusters.Name = "BtnDisplayClusters"
        Me.BtnDisplayClusters.Size = New System.Drawing.Size(179, 22)
        Me.BtnDisplayClusters.Text = "&Unused Clusters"
        '
        'BtnDisplayFile
        '
        Me.BtnDisplayFile.Name = "BtnDisplayFile"
        Me.BtnDisplayFile.Size = New System.Drawing.Size(179, 22)
        Me.BtnDisplayFile.Text = "&File"
        '
        'MainMenuExperimental
        '
        MainMenuExperimental.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnExportDebug})
        MainMenuExperimental.Name = "MainMenuExperimental"
        MainMenuExperimental.Size = New System.Drawing.Size(88, 23)
        MainMenuExperimental.Text = "Experimental"
        '
        'BtnExportDebug
        '
        Me.BtnExportDebug.Name = "BtnExportDebug"
        Me.BtnExportDebug.Size = New System.Drawing.Size(225, 22)
        Me.BtnExportDebug.Text = "Generate Physical Disk Patch"
        '
        'ToolStripSeparator5
        '
        ToolStripSeparator5.Name = "ToolStripSeparator5"
        ToolStripSeparator5.Size = New System.Drawing.Size(186, 6)
        '
        'FileCRC32
        '
        FileCRC32.Text = "CRC32"
        FileCRC32.Width = 70
        '
        'ToolStripSearch
        '
        ToolStripSearch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        ToolStripSearch.Margin = New System.Windows.Forms.Padding(8, 1, 0, 2)
        ToolStripSearch.Name = "ToolStripSearch"
        ToolStripSearch.Size = New System.Drawing.Size(42, 20)
        ToolStripSearch.Text = "Search"
        '
        'ToolStripOEMID
        '
        Me.ToolStripOEMID.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripOEMID.Name = "ToolStripOEMID"
        Me.ToolStripOEMID.Size = New System.Drawing.Size(47, 20)
        Me.ToolStripOEMID.Text = "OEM ID"
        '
        'ListViewSummary
        '
        Me.ListViewSummary.AllowDrop = True
        Me.ListViewSummary.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListViewSummary.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {SummaryName, SummaryValue})
        Me.ListViewSummary.FullRowSelect = True
        Me.ListViewSummary.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.ListViewSummary.HideSelection = False
        Me.ListViewSummary.Location = New System.Drawing.Point(12, 32)
        Me.ListViewSummary.MultiSelect = False
        Me.ListViewSummary.Name = "ListViewSummary"
        Me.ListViewSummary.ShowGroups = False
        Me.ListViewSummary.Size = New System.Drawing.Size(302, 410)
        Me.ListViewSummary.TabIndex = 1
        Me.ListViewSummary.UseCompatibleStateImageBehavior = False
        Me.ListViewSummary.View = System.Windows.Forms.View.Details
        '
        'ComboImages
        '
        Me.ComboImages.AllowDrop = True
        Me.ComboImages.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboImages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboImages.DropDownWidth = 523
        Me.ComboImages.Location = New System.Drawing.Point(320, 32)
        Me.ComboImages.Name = "ComboImages"
        Me.ComboImages.Size = New System.Drawing.Size(642, 21)
        Me.ComboImages.Sorted = True
        Me.ComboImages.TabIndex = 3
        '
        'ListViewHashes
        '
        Me.ListViewHashes.AllowDrop = True
        Me.ListViewHashes.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListViewHashes.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {HashName, HashValue})
        Me.ListViewHashes.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListViewHashes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.ListViewHashes.HideSelection = False
        Me.ListViewHashes.Location = New System.Drawing.Point(12, 448)
        Me.ListViewHashes.MultiSelect = False
        Me.ListViewHashes.Name = "ListViewHashes"
        Me.ListViewHashes.Scrollable = False
        Me.ListViewHashes.Size = New System.Drawing.Size(302, 101)
        Me.ListViewHashes.TabIndex = 2
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
        Me.LabelDropMessage.Location = New System.Drawing.Point(504, 290)
        Me.LabelDropMessage.Name = "LabelDropMessage"
        Me.LabelDropMessage.Size = New System.Drawing.Size(274, 16)
        Me.LabelDropMessage.TabIndex = 5
        Me.LabelDropMessage.Text = "Drag && Drop Floppy Disk Images Here"
        Me.LabelDropMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripFileName, Me.ToolStripFileCount, Me.ToolStripImageCount, Me.ToolStripModified})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 552)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(974, 24)
        Me.StatusStrip1.TabIndex = 6
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripFileName
        '
        Me.ToolStripFileName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripFileName.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripFileName.Name = "ToolStripFileName"
        Me.ToolStripFileName.Size = New System.Drawing.Size(733, 19)
        Me.ToolStripFileName.Spring = True
        Me.ToolStripFileName.Text = "File Name"
        Me.ToolStripFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToolStripFileCount
        '
        Me.ToolStripFileCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripFileCount.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripFileCount.Name = "ToolStripFileCount"
        Me.ToolStripFileCount.Size = New System.Drawing.Size(43, 19)
        Me.ToolStripFileCount.Text = "0 Files"
        '
        'ToolStripImageCount
        '
        Me.ToolStripImageCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripImageCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripImageCount.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripImageCount.Name = "ToolStripImageCount"
        Me.ToolStripImageCount.Size = New System.Drawing.Size(58, 19)
        Me.ToolStripImageCount.Text = "0 Images"
        Me.ToolStripImageCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ToolStripModified
        '
        Me.ToolStripModified.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripModified.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripModified.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripModified.Name = "ToolStripModified"
        Me.ToolStripModified.Size = New System.Drawing.Size(109, 19)
        Me.ToolStripModified.Text = "0 Images Modified"
        '
        'ListViewFiles
        '
        Me.ListViewFiles.AllowDrop = True
        Me.ListViewFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListViewFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {FileModified, FileName, FileExt, FileSize, FileLastWriteDate, FileStartingCluster, Me.FileClusterError, FileAttrib, FileCRC32, Me.FileCreateDate, Me.FileLastAccessDate, Me.FileLFN})
        Me.ListViewFiles.ContextMenuStrip = Me.ContextMenuFiles
        Me.ListViewFiles.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListViewFiles.FullRowSelect = True
        Me.ListViewFiles.HideSelection = False
        Me.ListViewFiles.Location = New System.Drawing.Point(320, 59)
        Me.ListViewFiles.Name = "ListViewFiles"
        Me.ListViewFiles.OwnerDraw = True
        Me.ListViewFiles.Size = New System.Drawing.Size(642, 490)
        Me.ListViewFiles.TabIndex = 4
        Me.ListViewFiles.UseCompatibleStateImageBehavior = False
        Me.ListViewFiles.View = System.Windows.Forms.View.Details
        '
        'FileClusterError
        '
        Me.FileClusterError.Text = "Err"
        Me.FileClusterError.Width = 0
        '
        'FileCreateDate
        '
        Me.FileCreateDate.Text = "Created"
        Me.FileCreateDate.Width = 0
        '
        'FileLastAccessDate
        '
        Me.FileLastAccessDate.Text = "Last Accessed"
        Me.FileLastAccessDate.Width = 0
        '
        'FileLFN
        '
        Me.FileLFN.Text = "Long File Name"
        Me.FileLFN.Width = 0
        '
        'ContextMenuFiles
        '
        Me.ContextMenuFiles.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnFileMenuFileProperties, Me.BtnFileMenuExportFile, Me.BtnFileMenuReplaceFile, ToolStripSeparator5, Me.BtnFileMenuViewFile, Me.BtnFileMenuViewFileText, Me.BtnFileMenuViewCrosslinked, Me.FileMenuSeparator, Me.BtnFileMenuUndo})
        Me.ContextMenuFiles.Name = "ContextMenuFiles"
        Me.ContextMenuFiles.Size = New System.Drawing.Size(190, 170)
        '
        'BtnFileMenuFileProperties
        '
        Me.BtnFileMenuFileProperties.Name = "BtnFileMenuFileProperties"
        Me.BtnFileMenuFileProperties.Size = New System.Drawing.Size(189, 22)
        Me.BtnFileMenuFileProperties.Text = "Edit File &Properties"
        '
        'BtnFileMenuExportFile
        '
        Me.BtnFileMenuExportFile.Name = "BtnFileMenuExportFile"
        Me.BtnFileMenuExportFile.Size = New System.Drawing.Size(189, 22)
        Me.BtnFileMenuExportFile.Text = "&Export File"
        '
        'BtnFileMenuReplaceFile
        '
        Me.BtnFileMenuReplaceFile.Name = "BtnFileMenuReplaceFile"
        Me.BtnFileMenuReplaceFile.Size = New System.Drawing.Size(189, 22)
        Me.BtnFileMenuReplaceFile.Text = "&Replace File"
        '
        'BtnFileMenuViewFile
        '
        Me.BtnFileMenuViewFile.Name = "BtnFileMenuViewFile"
        Me.BtnFileMenuViewFile.Size = New System.Drawing.Size(189, 22)
        Me.BtnFileMenuViewFile.Text = "&View File"
        '
        'BtnFileMenuViewFileText
        '
        Me.BtnFileMenuViewFileText.Name = "BtnFileMenuViewFileText"
        Me.BtnFileMenuViewFileText.Size = New System.Drawing.Size(189, 22)
        Me.BtnFileMenuViewFileText.Text = "View File as &Text"
        '
        'BtnFileMenuViewCrosslinked
        '
        Me.BtnFileMenuViewCrosslinked.Name = "BtnFileMenuViewCrosslinked"
        Me.BtnFileMenuViewCrosslinked.Size = New System.Drawing.Size(189, 22)
        Me.BtnFileMenuViewCrosslinked.Text = "View &Crosslinked Files"
        '
        'FileMenuSeparator
        '
        Me.FileMenuSeparator.Name = "FileMenuSeparator"
        Me.FileMenuSeparator.Size = New System.Drawing.Size(186, 6)
        '
        'BtnFileMenuUndo
        '
        Me.BtnFileMenuUndo.Name = "BtnFileMenuUndo"
        Me.BtnFileMenuUndo.Size = New System.Drawing.Size(189, 22)
        Me.BtnFileMenuUndo.Text = "&Undo Changes"
        '
        'MenuStripMain
        '
        Me.MenuStripMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {MainMenuFile, MainMenuEdit, Me.MainMenuFilters, MainMenuView, MainMenuExperimental, Me.TxtSearch, ToolStripSearch, Me.ComboOEMID, Me.ToolStripOEMID})
        Me.MenuStripMain.Location = New System.Drawing.Point(0, 0)
        Me.MenuStripMain.Name = "MenuStripMain"
        Me.MenuStripMain.Size = New System.Drawing.Size(974, 27)
        Me.MenuStripMain.TabIndex = 0
        Me.MenuStripMain.Text = "MenuStrip1"
        '
        'MainMenuFilters
        '
        Me.MainMenuFilters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.MainMenuFilters.DropDown = Me.ContextMenuFilters
        Me.MainMenuFilters.Name = "MainMenuFilters"
        Me.MainMenuFilters.Size = New System.Drawing.Size(50, 23)
        Me.MainMenuFilters.Text = "F&ilters"
        '
        'ContextMenuFilters
        '
        Me.ContextMenuFilters.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnScanNew, Me.BtnScan, Me.BtnClearFilters, Me.FilterSeparator})
        Me.ContextMenuFilters.Name = "ContextMenuStrip1"
        Me.ContextMenuFilters.OwnerItem = Me.MainMenuFilters
        Me.ContextMenuFilters.Size = New System.Drawing.Size(168, 76)
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
        'BtnClearFilters
        '
        Me.BtnClearFilters.Name = "BtnClearFilters"
        Me.BtnClearFilters.Size = New System.Drawing.Size(167, 22)
        Me.BtnClearFilters.Text = "Clear Filters"
        '
        'FilterSeparator
        '
        Me.FilterSeparator.Name = "FilterSeparator"
        Me.FilterSeparator.Size = New System.Drawing.Size(164, 6)
        Me.FilterSeparator.Visible = False
        '
        'TxtSearch
        '
        Me.TxtSearch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.TxtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtSearch.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TxtSearch.Margin = New System.Windows.Forms.Padding(1, 0, 12, 0)
        Me.TxtSearch.MaxLength = 255
        Me.TxtSearch.Name = "TxtSearch"
        Me.TxtSearch.Size = New System.Drawing.Size(200, 23)
        '
        'ComboOEMID
        '
        Me.ComboOEMID.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ComboOEMID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboOEMID.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.ComboOEMID.Name = "ComboOEMID"
        Me.ComboOEMID.Size = New System.Drawing.Size(175, 23)
        Me.ComboOEMID.Sorted = True
        '
        'ComboImagesFiltered
        '
        Me.ComboImagesFiltered.AllowDrop = True
        Me.ComboImagesFiltered.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboImagesFiltered.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboImagesFiltered.DropDownWidth = 523
        Me.ComboImagesFiltered.Location = New System.Drawing.Point(320, 32)
        Me.ComboImagesFiltered.Name = "ComboImagesFiltered"
        Me.ComboImagesFiltered.Size = New System.Drawing.Size(642, 21)
        Me.ComboImagesFiltered.Sorted = True
        Me.ComboImagesFiltered.TabIndex = 3
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(974, 576)
        Me.Controls.Add(Me.ComboImagesFiltered)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStripMain)
        Me.Controls.Add(Me.LabelDropMessage)
        Me.Controls.Add(Me.ListViewHashes)
        Me.Controls.Add(Me.ComboImages)
        Me.Controls.Add(Me.ListViewSummary)
        Me.Controls.Add(Me.ListViewFiles)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStripMain
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Disk Image Tool"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ContextMenuFiles.ResumeLayout(False)
        Me.MenuStripMain.ResumeLayout(False)
        Me.MenuStripMain.PerformLayout()
        Me.ContextMenuFilters.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ListViewSummary As ListView
    Friend WithEvents ComboImages As ComboBox
    Friend WithEvents ListViewHashes As ListView
    Friend WithEvents LabelDropMessage As Label
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripImageCount As ToolStripStatusLabel
    Friend WithEvents ToolStripFileName As ToolStripStatusLabel
    Friend WithEvents ToolStripModified As ToolStripStatusLabel
    Friend WithEvents ListViewFiles As ListView
    Friend WithEvents MenuStripMain As MenuStrip
    Friend WithEvents BtnOpen As ToolStripMenuItem
    Friend WithEvents toolStripSeparator As ToolStripSeparator
    Friend WithEvents BtnSave As ToolStripMenuItem
    Friend WithEvents BtnSaveAs As ToolStripMenuItem
    Friend WithEvents toolStripSeparator1 As ToolStripSeparator
    Friend WithEvents BtnExit As ToolStripMenuItem
    Friend WithEvents ContextMenuFilters As ContextMenuStrip
    Friend WithEvents BtnSaveAll As ToolStripMenuItem
    Friend WithEvents BtnScan As ToolStripMenuItem
    Friend WithEvents FilterSeparator As ToolStripSeparator
    Friend WithEvents BtnDisplayBootSector As ToolStripMenuItem
    Friend WithEvents BtnDisplayDirectory As ToolStripMenuItem
    Friend WithEvents BtnDisplayClusters As ToolStripMenuItem
    Friend WithEvents BtnOEMID As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents BtnRevert As ToolStripMenuItem
    Friend WithEvents BtnDisplayFile As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents BtnClose As ToolStripMenuItem
    Friend WithEvents BtnCloseAll As ToolStripMenuItem
    Friend WithEvents BtnScanNew As ToolStripMenuItem
    Friend WithEvents BtnExportDebug As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents ToolStripFileCount As ToolStripStatusLabel
    Friend WithEvents BtnFileProperties As ToolStripMenuItem
    Friend WithEvents ContextMenuFiles As ContextMenuStrip
    Friend WithEvents BtnFileMenuFileProperties As ToolStripMenuItem
    Friend WithEvents BtnFileMenuViewFile As ToolStripMenuItem
    Friend WithEvents BtnFileMenuViewFileText As ToolStripMenuItem
    Friend WithEvents MainMenuFilters As ToolStripMenuItem
    Friend WithEvents FileMenuSeparator As ToolStripSeparator
    Friend WithEvents BtnFileMenuUndo As ToolStripMenuItem
    Friend WithEvents BtnReplaceFile As ToolStripMenuItem
    Friend WithEvents BtnFileMenuReplaceFile As ToolStripMenuItem
    Friend WithEvents FileCreateDate As ColumnHeader
    Friend WithEvents FileLastAccessDate As ColumnHeader
    Friend WithEvents FileLFN As ColumnHeader
    Friend WithEvents BtnDisplayFAT As ToolStripMenuItem
    Friend WithEvents FileClusterError As ColumnHeader
    Friend WithEvents BtnFileMenuViewCrosslinked As ToolStripMenuItem
    Friend WithEvents BtnExportFile As ToolStripMenuItem
    Friend WithEvents BtnFileMenuExportFile As ToolStripMenuItem
    Friend WithEvents TxtSearch As ToolStripTextBox
    Friend WithEvents ComboImagesFiltered As ComboBox
    Friend WithEvents BtnClearFilters As ToolStripMenuItem
    Friend WithEvents ComboOEMID As ToolStripComboBox
    Friend WithEvents ToolStripOEMID As ToolStripLabel
End Class
