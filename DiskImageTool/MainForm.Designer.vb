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
        Dim FileMenuSeparatoor1 As System.Windows.Forms.ToolStripSeparator
        Dim FileCRC32 As System.Windows.Forms.ColumnHeader
        Dim ToolStripSearch As System.Windows.Forms.ToolStripLabel
        Dim MainMenuTools As System.Windows.Forms.ToolStripMenuItem
        Dim ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
        Dim MainHelp As System.Windows.Forms.ToolStripMenuItem
        Dim FileMenuSeparatoor2 As System.Windows.Forms.ToolStripSeparator
        Dim FileMenuSeparatoor3 As System.Windows.Forms.ToolStripSeparator
        Me.BtnOpen = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnReload = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnSave = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnSaveAs = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnSaveAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnClose = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnCloseAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnExit = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuEdit = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnEditBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnEditFAT = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnFileProperties = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnExportFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnReplaceFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnAddFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnUndo = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnRedo = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnRevert = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.btnCreateBackup = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayFAT = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayDirectory = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayClusters = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayBadSectors = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayLostClusters = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayOverdumpData = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnDisplayDisk = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnExportDebug = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnCompare = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnWin9xClean = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnClearReservedBytes = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFixImageSize = New System.Windows.Forms.ToolStripMenuItem()
        Me.SubMenuFixImageSize = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnTruncateImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnRestructureImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnRestoreBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnRemoveBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnWin9xCleanBatch = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnHelpProjectPage = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnHelpUpdateCheck = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnHelpChangeLog = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator12 = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnHelpAbout = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripTop = New System.Windows.Forms.ToolStrip()
        Me.TxtSearch = New System.Windows.Forms.ToolStripTextBox()
        Me.ComboOEMName = New System.Windows.Forms.ToolStripComboBox()
        Me.ToolStripOEMName = New System.Windows.Forms.ToolStripLabel()
        Me.ComboDiskType = New System.Windows.Forms.ToolStripComboBox()
        Me.ToolStripDiskType = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripBtnOpen = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripBtnSave = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnSaveAs = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnSaveAll = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripBtnClose = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnCloseAll = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripBtnFileProperties = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnExportFile = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripBtnUndo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnRedo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator10 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripBtnViewFileText = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnViewFile = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparatorFAT = New System.Windows.Forms.ToolStripSeparator()
        Me.ComboFAT = New System.Windows.Forms.ToolStripComboBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusReadOnly = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusCached = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusModified = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripFileName = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripFileCount = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripFileSector = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripFileTrack = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripImageCount = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripModified = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ListViewSummary = New System.Windows.Forms.ListView()
        Me.ComboImages = New System.Windows.Forms.ComboBox()
        Me.ListViewHashes = New System.Windows.Forms.ListView()
        Me.LabelDropMessage = New System.Windows.Forms.Label()
        Me.ListViewFiles = New System.Windows.Forms.ListView()
        Me.FileClusterError = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileCreationDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileLastAccessDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileReserved = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileLFN = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ContextMenuFiles = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnFileMenuFileProperties = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuExportFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuReplaceFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuAddFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuViewDirectory = New System.Windows.Forms.ToolStripMenuItem()
        Me.FileMenuSeparatorDirectory = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnFileMenuViewFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuViewFileText = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuViewCrosslinked = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuDeleteFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuUnDeleteFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuRemoveDeletedFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuDeleteFileWithFill = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFileMenuFixSize = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStripMain = New System.Windows.Forms.MenuStrip()
        Me.MainMenuFilters = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuFilters = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnScanNew = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnScan = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnClearFilters = New System.Windows.Forms.ToolStripMenuItem()
        Me.DiskToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnReadFloppyA = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnReadFloppyB = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnWriteFloppyA = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnWriteFloppyB = New System.Windows.Forms.ToolStripMenuItem()
        Me.ComboImagesFiltered = New System.Windows.Forms.ComboBox()
        Me.BtnResetSort = New System.Windows.Forms.Button()
        Me.btnRetry = New System.Windows.Forms.Button()
        Me.BtnRawTrackData = New System.Windows.Forms.ToolStripMenuItem()
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
        FileMenuSeparatoor1 = New System.Windows.Forms.ToolStripSeparator()
        FileCRC32 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        ToolStripSearch = New System.Windows.Forms.ToolStripLabel()
        MainMenuTools = New System.Windows.Forms.ToolStripMenuItem()
        ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        MainHelp = New System.Windows.Forms.ToolStripMenuItem()
        FileMenuSeparatoor2 = New System.Windows.Forms.ToolStripSeparator()
        FileMenuSeparatoor3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ContextMenuEdit.SuspendLayout()
        Me.ToolStripTop.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.ContextMenuFiles.SuspendLayout()
        Me.MenuStripMain.SuspendLayout()
        Me.ContextMenuFilters.SuspendLayout()
        Me.SuspendLayout()
        '
        'SummaryName
        '
        SummaryName.Text = "Name"
        SummaryName.Width = 118
        '
        'SummaryValue
        '
        SummaryValue.Text = "Value"
        SummaryValue.Width = 183
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
        FileModified.Width = 27
        '
        'MainMenuFile
        '
        MainMenuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnOpen, Me.BtnReload, Me.toolStripSeparator, Me.BtnSave, Me.BtnSaveAs, Me.BtnSaveAll, Me.ToolStripSeparator3, Me.BtnClose, Me.BtnCloseAll, Me.toolStripSeparator1, Me.BtnExit})
        MainMenuFile.Name = "MainMenuFile"
        MainMenuFile.ShortcutKeyDisplayString = ""
        MainMenuFile.Size = New System.Drawing.Size(37, 20)
        MainMenuFile.Text = "&File"
        '
        'BtnOpen
        '
        Me.BtnOpen.Image = CType(resources.GetObject("BtnOpen.Image"), System.Drawing.Image)
        Me.BtnOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.BtnOpen.Name = "BtnOpen"
        Me.BtnOpen.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.BtnOpen.Size = New System.Drawing.Size(212, 22)
        Me.BtnOpen.Text = "&Open"
        '
        'BtnReload
        '
        Me.BtnReload.Name = "BtnReload"
        Me.BtnReload.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
        Me.BtnReload.Size = New System.Drawing.Size(212, 22)
        Me.BtnReload.Text = "&Reload from Disk"
        '
        'toolStripSeparator
        '
        Me.toolStripSeparator.Name = "toolStripSeparator"
        Me.toolStripSeparator.Size = New System.Drawing.Size(209, 6)
        '
        'BtnSave
        '
        Me.BtnSave.Image = CType(resources.GetObject("BtnSave.Image"), System.Drawing.Image)
        Me.BtnSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.BtnSave.Name = "BtnSave"
        Me.BtnSave.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.BtnSave.Size = New System.Drawing.Size(212, 22)
        Me.BtnSave.Text = "&Save"
        '
        'BtnSaveAs
        '
        Me.BtnSaveAs.Image = CType(resources.GetObject("BtnSaveAs.Image"), System.Drawing.Image)
        Me.BtnSaveAs.Name = "BtnSaveAs"
        Me.BtnSaveAs.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Alt) _
            Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.BtnSaveAs.Size = New System.Drawing.Size(212, 22)
        Me.BtnSaveAs.Text = "Save &As"
        '
        'BtnSaveAll
        '
        Me.BtnSaveAll.Image = CType(resources.GetObject("BtnSaveAll.Image"), System.Drawing.Image)
        Me.BtnSaveAll.Name = "BtnSaveAll"
        Me.BtnSaveAll.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.BtnSaveAll.Size = New System.Drawing.Size(212, 22)
        Me.BtnSaveAll.Text = "Save All"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(209, 6)
        '
        'BtnClose
        '
        Me.BtnClose.Image = CType(resources.GetObject("BtnClose.Image"), System.Drawing.Image)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.W), System.Windows.Forms.Keys)
        Me.BtnClose.Size = New System.Drawing.Size(212, 22)
        Me.BtnClose.Text = "&Close"
        '
        'BtnCloseAll
        '
        Me.BtnCloseAll.Image = CType(resources.GetObject("BtnCloseAll.Image"), System.Drawing.Image)
        Me.BtnCloseAll.Name = "BtnCloseAll"
        Me.BtnCloseAll.ShortcutKeyDisplayString = "     Ctrl+Shift+W"
        Me.BtnCloseAll.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.W), System.Windows.Forms.Keys)
        Me.BtnCloseAll.Size = New System.Drawing.Size(212, 22)
        Me.BtnCloseAll.Text = "Close All"
        '
        'toolStripSeparator1
        '
        Me.toolStripSeparator1.Name = "toolStripSeparator1"
        Me.toolStripSeparator1.Size = New System.Drawing.Size(209, 6)
        '
        'BtnExit
        '
        Me.BtnExit.Image = CType(resources.GetObject("BtnExit.Image"), System.Drawing.Image)
        Me.BtnExit.Name = "BtnExit"
        Me.BtnExit.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.F4), System.Windows.Forms.Keys)
        Me.BtnExit.Size = New System.Drawing.Size(212, 22)
        Me.BtnExit.Text = "E&xit"
        '
        'MainMenuEdit
        '
        MainMenuEdit.DropDown = Me.ContextMenuEdit
        MainMenuEdit.Name = "MainMenuEdit"
        MainMenuEdit.Size = New System.Drawing.Size(39, 20)
        MainMenuEdit.Text = "&Edit"
        '
        'ContextMenuEdit
        '
        Me.ContextMenuEdit.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnEditBootSector, Me.BtnEditFAT, Me.ToolStripSeparator4, Me.BtnFileProperties, Me.BtnExportFile, Me.BtnReplaceFile, Me.BtnAddFile, Me.ToolStripSeparator2, Me.BtnUndo, Me.BtnRedo, Me.BtnRevert, Me.ToolStripSeparator5, Me.btnCreateBackup})
        Me.ContextMenuEdit.Name = "ContextMenuEdit"
        Me.ContextMenuEdit.OwnerItem = MainMenuEdit
        Me.ContextMenuEdit.Size = New System.Drawing.Size(195, 242)
        '
        'BtnEditBootSector
        '
        Me.BtnEditBootSector.Name = "BtnEditBootSector"
        Me.BtnEditBootSector.Size = New System.Drawing.Size(194, 22)
        Me.BtnEditBootSector.Text = "&Boot Sector"
        '
        'BtnEditFAT
        '
        Me.BtnEditFAT.Name = "BtnEditFAT"
        Me.BtnEditFAT.Size = New System.Drawing.Size(194, 22)
        Me.BtnEditFAT.Text = "File &Allocation Table"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(191, 6)
        '
        'BtnFileProperties
        '
        Me.BtnFileProperties.Image = CType(resources.GetObject("BtnFileProperties.Image"), System.Drawing.Image)
        Me.BtnFileProperties.Name = "BtnFileProperties"
        Me.BtnFileProperties.Size = New System.Drawing.Size(194, 22)
        Me.BtnFileProperties.Text = "File &Properties"
        '
        'BtnExportFile
        '
        Me.BtnExportFile.Image = CType(resources.GetObject("BtnExportFile.Image"), System.Drawing.Image)
        Me.BtnExportFile.Name = "BtnExportFile"
        Me.BtnExportFile.Size = New System.Drawing.Size(194, 22)
        Me.BtnExportFile.Text = "&Export File"
        '
        'BtnReplaceFile
        '
        Me.BtnReplaceFile.Name = "BtnReplaceFile"
        Me.BtnReplaceFile.Size = New System.Drawing.Size(194, 22)
        Me.BtnReplaceFile.Text = "&Replace File"
        '
        'BtnAddFile
        '
        Me.BtnAddFile.Name = "BtnAddFile"
        Me.BtnAddFile.Size = New System.Drawing.Size(194, 22)
        Me.BtnAddFile.Text = "&Add File"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(191, 6)
        '
        'BtnUndo
        '
        Me.BtnUndo.Image = CType(resources.GetObject("BtnUndo.Image"), System.Drawing.Image)
        Me.BtnUndo.Name = "BtnUndo"
        Me.BtnUndo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.BtnUndo.Size = New System.Drawing.Size(194, 22)
        Me.BtnUndo.Text = "&Undo"
        '
        'BtnRedo
        '
        Me.BtnRedo.Image = CType(resources.GetObject("BtnRedo.Image"), System.Drawing.Image)
        Me.BtnRedo.Name = "BtnRedo"
        Me.BtnRedo.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.BtnRedo.Size = New System.Drawing.Size(194, 22)
        Me.BtnRedo.Text = "&Redo"
        '
        'BtnRevert
        '
        Me.BtnRevert.Name = "BtnRevert"
        Me.BtnRevert.Size = New System.Drawing.Size(194, 22)
        Me.BtnRevert.Text = "&Revert Changes"
        Me.BtnRevert.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(191, 6)
        '
        'btnCreateBackup
        '
        Me.btnCreateBackup.CheckOnClick = True
        Me.btnCreateBackup.Name = "btnCreateBackup"
        Me.btnCreateBackup.Size = New System.Drawing.Size(194, 22)
        Me.btnCreateBackup.Text = "Create Backup on Save"
        '
        'MainMenuView
        '
        MainMenuView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnDisplayBootSector, Me.BtnDisplayFAT, Me.BtnDisplayDirectory, Me.BtnDisplayClusters, Me.BtnDisplayFile, Me.BtnDisplayBadSectors, Me.BtnDisplayLostClusters, Me.BtnDisplayOverdumpData, Me.BtnRawTrackData, Me.BtnDisplayDisk})
        MainMenuView.Name = "MainMenuView"
        MainMenuView.Size = New System.Drawing.Size(40, 20)
        MainMenuView.Text = "&Hex"
        '
        'BtnDisplayBootSector
        '
        Me.BtnDisplayBootSector.Name = "BtnDisplayBootSector"
        Me.BtnDisplayBootSector.Size = New System.Drawing.Size(194, 22)
        Me.BtnDisplayBootSector.Text = "&Boot Sector"
        '
        'BtnDisplayFAT
        '
        Me.BtnDisplayFAT.Name = "BtnDisplayFAT"
        Me.BtnDisplayFAT.Size = New System.Drawing.Size(194, 22)
        Me.BtnDisplayFAT.Text = "File &Allocation Table"
        '
        'BtnDisplayDirectory
        '
        Me.BtnDisplayDirectory.Name = "BtnDisplayDirectory"
        Me.BtnDisplayDirectory.Size = New System.Drawing.Size(194, 22)
        Me.BtnDisplayDirectory.Text = "Root &Directory"
        '
        'BtnDisplayClusters
        '
        Me.BtnDisplayClusters.Name = "BtnDisplayClusters"
        Me.BtnDisplayClusters.Size = New System.Drawing.Size(194, 22)
        Me.BtnDisplayClusters.Text = "Free &Clusters with Data"
        '
        'BtnDisplayFile
        '
        Me.BtnDisplayFile.Name = "BtnDisplayFile"
        Me.BtnDisplayFile.Size = New System.Drawing.Size(194, 22)
        Me.BtnDisplayFile.Text = "&File"
        '
        'BtnDisplayBadSectors
        '
        Me.BtnDisplayBadSectors.Name = "BtnDisplayBadSectors"
        Me.BtnDisplayBadSectors.Size = New System.Drawing.Size(194, 22)
        Me.BtnDisplayBadSectors.Text = "Bad &Sectors"
        '
        'BtnDisplayLostClusters
        '
        Me.BtnDisplayLostClusters.Name = "BtnDisplayLostClusters"
        Me.BtnDisplayLostClusters.Size = New System.Drawing.Size(194, 22)
        Me.BtnDisplayLostClusters.Text = "&Lost Clusters"
        '
        'BtnDisplayOverdumpData
        '
        Me.BtnDisplayOverdumpData.Name = "BtnDisplayOverdumpData"
        Me.BtnDisplayOverdumpData.Size = New System.Drawing.Size(194, 22)
        Me.BtnDisplayOverdumpData.Text = "&Overdump Data"
        '
        'BtnDisplayDisk
        '
        Me.BtnDisplayDisk.Name = "BtnDisplayDisk"
        Me.BtnDisplayDisk.Size = New System.Drawing.Size(194, 22)
        Me.BtnDisplayDisk.Text = "Entire &Disk"
        '
        'MainMenuExperimental
        '
        MainMenuExperimental.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnExportDebug})
        MainMenuExperimental.Name = "MainMenuExperimental"
        MainMenuExperimental.Size = New System.Drawing.Size(88, 20)
        MainMenuExperimental.Text = "Experimental"
        MainMenuExperimental.Visible = False
        '
        'BtnExportDebug
        '
        Me.BtnExportDebug.Name = "BtnExportDebug"
        Me.BtnExportDebug.Size = New System.Drawing.Size(225, 22)
        Me.BtnExportDebug.Text = "Generate Physical Disk Patch"
        '
        'FileMenuSeparatoor1
        '
        FileMenuSeparatoor1.Name = "FileMenuSeparatoor1"
        FileMenuSeparatoor1.Size = New System.Drawing.Size(219, 6)
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
        ToolStripSearch.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        ToolStripSearch.Size = New System.Drawing.Size(42, 22)
        ToolStripSearch.Text = "Search"
        '
        'MainMenuTools
        '
        MainMenuTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnCompare, Me.BtnWin9xClean, Me.BtnClearReservedBytes, Me.BtnFixImageSize, Me.SubMenuFixImageSize, Me.BtnRestoreBootSector, Me.BtnRemoveBootSector, ToolStripMenuItem1, Me.BtnWin9xCleanBatch})
        MainMenuTools.Name = "MainMenuTools"
        MainMenuTools.Size = New System.Drawing.Size(46, 20)
        MainMenuTools.Text = "&Tools"
        '
        'BtnCompare
        '
        Me.BtnCompare.Name = "BtnCompare"
        Me.BtnCompare.Size = New System.Drawing.Size(289, 22)
        Me.BtnCompare.Text = "&Compare Images"
        '
        'BtnWin9xClean
        '
        Me.BtnWin9xClean.Name = "BtnWin9xClean"
        Me.BtnWin9xClean.Size = New System.Drawing.Size(289, 22)
        Me.BtnWin9xClean.Text = "Remove &Windows Modifications"
        '
        'BtnClearReservedBytes
        '
        Me.BtnClearReservedBytes.Name = "BtnClearReservedBytes"
        Me.BtnClearReservedBytes.Size = New System.Drawing.Size(289, 22)
        Me.BtnClearReservedBytes.Text = "Clear &Reserved Bytes"
        '
        'BtnFixImageSize
        '
        Me.BtnFixImageSize.Name = "BtnFixImageSize"
        Me.BtnFixImageSize.Size = New System.Drawing.Size(289, 22)
        Me.BtnFixImageSize.Text = "Fix Image &Size"
        '
        'SubMenuFixImageSize
        '
        Me.SubMenuFixImageSize.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnTruncateImage, Me.BtnRestructureImage})
        Me.SubMenuFixImageSize.Name = "SubMenuFixImageSize"
        Me.SubMenuFixImageSize.Size = New System.Drawing.Size(289, 22)
        Me.SubMenuFixImageSize.Text = "Fix Image &Size"
        '
        'BtnTruncateImage
        '
        Me.BtnTruncateImage.Name = "BtnTruncateImage"
        Me.BtnTruncateImage.Size = New System.Drawing.Size(170, 22)
        Me.BtnTruncateImage.Text = "&Truncate Image"
        '
        'BtnRestructureImage
        '
        Me.BtnRestructureImage.Name = "BtnRestructureImage"
        Me.BtnRestructureImage.Size = New System.Drawing.Size(170, 22)
        Me.BtnRestructureImage.Text = "&Restructure Image"
        '
        'BtnRestoreBootSector
        '
        Me.BtnRestoreBootSector.Name = "BtnRestoreBootSector"
        Me.BtnRestoreBootSector.Size = New System.Drawing.Size(289, 22)
        Me.BtnRestoreBootSector.Text = "Restore &Boot Sector from Root Directory"
        '
        'BtnRemoveBootSector
        '
        Me.BtnRemoveBootSector.Name = "BtnRemoveBootSector"
        Me.BtnRemoveBootSector.Size = New System.Drawing.Size(289, 22)
        Me.BtnRemoveBootSector.Text = "Remove &Boot Sector from Root Directory"
        '
        'ToolStripMenuItem1
        '
        ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        ToolStripMenuItem1.Size = New System.Drawing.Size(286, 6)
        '
        'BtnWin9xCleanBatch
        '
        Me.BtnWin9xCleanBatch.Name = "BtnWin9xCleanBatch"
        Me.BtnWin9xCleanBatch.Size = New System.Drawing.Size(289, 22)
        Me.BtnWin9xCleanBatch.Text = "Batch Remove Windows Modifications"
        '
        'MainHelp
        '
        MainHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnHelpProjectPage, Me.BtnHelpUpdateCheck, Me.BtnHelpChangeLog, Me.ToolStripSeparator12, Me.BtnHelpAbout})
        MainHelp.Name = "MainHelp"
        MainHelp.Size = New System.Drawing.Size(24, 20)
        MainHelp.Text = "?"
        '
        'BtnHelpProjectPage
        '
        Me.BtnHelpProjectPage.Name = "BtnHelpProjectPage"
        Me.BtnHelpProjectPage.Size = New System.Drawing.Size(212, 22)
        Me.BtnHelpProjectPage.Text = "&Project Page"
        '
        'BtnHelpUpdateCheck
        '
        Me.BtnHelpUpdateCheck.Name = "BtnHelpUpdateCheck"
        Me.BtnHelpUpdateCheck.Size = New System.Drawing.Size(212, 22)
        Me.BtnHelpUpdateCheck.Text = "Check for &Updates"
        '
        'BtnHelpChangeLog
        '
        Me.BtnHelpChangeLog.Name = "BtnHelpChangeLog"
        Me.BtnHelpChangeLog.Size = New System.Drawing.Size(212, 22)
        Me.BtnHelpChangeLog.Text = "&Change Log"
        '
        'ToolStripSeparator12
        '
        Me.ToolStripSeparator12.Name = "ToolStripSeparator12"
        Me.ToolStripSeparator12.Size = New System.Drawing.Size(209, 6)
        '
        'BtnHelpAbout
        '
        Me.BtnHelpAbout.Name = "BtnHelpAbout"
        Me.BtnHelpAbout.ShortcutKeys = System.Windows.Forms.Keys.F1
        Me.BtnHelpAbout.Size = New System.Drawing.Size(212, 22)
        Me.BtnHelpAbout.Text = "About Disk Image Tool"
        '
        'FileMenuSeparatoor2
        '
        FileMenuSeparatoor2.Name = "FileMenuSeparatoor2"
        FileMenuSeparatoor2.Size = New System.Drawing.Size(219, 6)
        '
        'FileMenuSeparatoor3
        '
        FileMenuSeparatoor3.Name = "FileMenuSeparatoor3"
        FileMenuSeparatoor3.Size = New System.Drawing.Size(219, 6)
        FileMenuSeparatoor3.Visible = False
        '
        'ToolStripTop
        '
        Me.ToolStripTop.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStripTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TxtSearch, ToolStripSearch, Me.ComboOEMName, Me.ToolStripOEMName, Me.ComboDiskType, Me.ToolStripDiskType, Me.ToolStripBtnOpen, Me.ToolStripSeparator6, Me.ToolStripBtnSave, Me.ToolStripBtnSaveAs, Me.ToolStripBtnSaveAll, Me.ToolStripSeparator7, Me.ToolStripBtnClose, Me.ToolStripBtnCloseAll, Me.ToolStripSeparator8, Me.ToolStripBtnFileProperties, Me.ToolStripBtnExportFile, Me.ToolStripSeparator9, Me.ToolStripBtnUndo, Me.ToolStripBtnRedo, Me.ToolStripSeparator10, Me.ToolStripBtnViewFileText, Me.ToolStripBtnViewFile, Me.ToolStripSeparatorFAT, Me.ComboFAT})
        Me.ToolStripTop.Location = New System.Drawing.Point(0, 24)
        Me.ToolStripTop.Name = "ToolStripTop"
        Me.ToolStripTop.Padding = New System.Windows.Forms.Padding(12, 0, 12, 0)
        Me.ToolStripTop.Size = New System.Drawing.Size(1004, 25)
        Me.ToolStripTop.TabIndex = 1
        Me.ToolStripTop.Text = "ToolStrip1"
        '
        'TxtSearch
        '
        Me.TxtSearch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.TxtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtSearch.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.TxtSearch.Margin = New System.Windows.Forms.Padding(1, 0, 0, 0)
        Me.TxtSearch.MaxLength = 255
        Me.TxtSearch.Name = "TxtSearch"
        Me.TxtSearch.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.TxtSearch.Size = New System.Drawing.Size(195, 25)
        '
        'ComboOEMName
        '
        Me.ComboOEMName.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ComboOEMName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboOEMName.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.ComboOEMName.Name = "ComboOEMName"
        Me.ComboOEMName.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ComboOEMName.Size = New System.Drawing.Size(125, 25)
        Me.ComboOEMName.Sorted = True
        '
        'ToolStripOEMName
        '
        Me.ToolStripOEMName.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripOEMName.Margin = New System.Windows.Forms.Padding(8, 1, 0, 2)
        Me.ToolStripOEMName.Name = "ToolStripOEMName"
        Me.ToolStripOEMName.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripOEMName.Size = New System.Drawing.Size(68, 22)
        Me.ToolStripOEMName.Text = "OEM Name"
        '
        'ComboDiskType
        '
        Me.ComboDiskType.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ComboDiskType.AutoSize = False
        Me.ComboDiskType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboDiskType.DropDownWidth = 95
        Me.ComboDiskType.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.ComboDiskType.Name = "ComboDiskType"
        Me.ComboDiskType.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ComboDiskType.Size = New System.Drawing.Size(95, 23)
        '
        'ToolStripDiskType
        '
        Me.ToolStripDiskType.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripDiskType.Name = "ToolStripDiskType"
        Me.ToolStripDiskType.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripDiskType.Size = New System.Drawing.Size(56, 22)
        Me.ToolStripDiskType.Text = "Disk Type"
        '
        'ToolStripBtnOpen
        '
        Me.ToolStripBtnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnOpen.Image = CType(resources.GetObject("ToolStripBtnOpen.Image"), System.Drawing.Image)
        Me.ToolStripBtnOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnOpen.Name = "ToolStripBtnOpen"
        Me.ToolStripBtnOpen.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnOpen.Text = "Open"
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripBtnSave
        '
        Me.ToolStripBtnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnSave.Image = CType(resources.GetObject("ToolStripBtnSave.Image"), System.Drawing.Image)
        Me.ToolStripBtnSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnSave.Name = "ToolStripBtnSave"
        Me.ToolStripBtnSave.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnSave.Text = "Save"
        '
        'ToolStripBtnSaveAs
        '
        Me.ToolStripBtnSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnSaveAs.Image = CType(resources.GetObject("ToolStripBtnSaveAs.Image"), System.Drawing.Image)
        Me.ToolStripBtnSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnSaveAs.Name = "ToolStripBtnSaveAs"
        Me.ToolStripBtnSaveAs.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnSaveAs.Text = "Save As"
        '
        'ToolStripBtnSaveAll
        '
        Me.ToolStripBtnSaveAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnSaveAll.Image = CType(resources.GetObject("ToolStripBtnSaveAll.Image"), System.Drawing.Image)
        Me.ToolStripBtnSaveAll.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnSaveAll.Name = "ToolStripBtnSaveAll"
        Me.ToolStripBtnSaveAll.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnSaveAll.Text = "Save All"
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        Me.ToolStripSeparator7.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripBtnClose
        '
        Me.ToolStripBtnClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnClose.Image = CType(resources.GetObject("ToolStripBtnClose.Image"), System.Drawing.Image)
        Me.ToolStripBtnClose.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnClose.Name = "ToolStripBtnClose"
        Me.ToolStripBtnClose.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnClose.Text = "Close"
        '
        'ToolStripBtnCloseAll
        '
        Me.ToolStripBtnCloseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnCloseAll.Image = CType(resources.GetObject("ToolStripBtnCloseAll.Image"), System.Drawing.Image)
        Me.ToolStripBtnCloseAll.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnCloseAll.Name = "ToolStripBtnCloseAll"
        Me.ToolStripBtnCloseAll.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnCloseAll.Text = "Close All"
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        Me.ToolStripSeparator8.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripBtnFileProperties
        '
        Me.ToolStripBtnFileProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnFileProperties.Image = CType(resources.GetObject("ToolStripBtnFileProperties.Image"), System.Drawing.Image)
        Me.ToolStripBtnFileProperties.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnFileProperties.Name = "ToolStripBtnFileProperties"
        Me.ToolStripBtnFileProperties.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnFileProperties.Text = "File Properties"
        '
        'ToolStripBtnExportFile
        '
        Me.ToolStripBtnExportFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnExportFile.Image = CType(resources.GetObject("ToolStripBtnExportFile.Image"), System.Drawing.Image)
        Me.ToolStripBtnExportFile.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnExportFile.Name = "ToolStripBtnExportFile"
        Me.ToolStripBtnExportFile.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnExportFile.Text = "Export File"
        '
        'ToolStripSeparator9
        '
        Me.ToolStripSeparator9.Name = "ToolStripSeparator9"
        Me.ToolStripSeparator9.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripBtnUndo
        '
        Me.ToolStripBtnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnUndo.Image = CType(resources.GetObject("ToolStripBtnUndo.Image"), System.Drawing.Image)
        Me.ToolStripBtnUndo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnUndo.Name = "ToolStripBtnUndo"
        Me.ToolStripBtnUndo.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnUndo.Text = "Undo"
        '
        'ToolStripBtnRedo
        '
        Me.ToolStripBtnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnRedo.Image = CType(resources.GetObject("ToolStripBtnRedo.Image"), System.Drawing.Image)
        Me.ToolStripBtnRedo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnRedo.Name = "ToolStripBtnRedo"
        Me.ToolStripBtnRedo.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnRedo.Text = "Redo"
        '
        'ToolStripSeparator10
        '
        Me.ToolStripSeparator10.Name = "ToolStripSeparator10"
        Me.ToolStripSeparator10.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripBtnViewFileText
        '
        Me.ToolStripBtnViewFileText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnViewFileText.Image = CType(resources.GetObject("ToolStripBtnViewFileText.Image"), System.Drawing.Image)
        Me.ToolStripBtnViewFileText.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnViewFileText.Name = "ToolStripBtnViewFileText"
        Me.ToolStripBtnViewFileText.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnViewFileText.Text = "View File as Text"
        '
        'ToolStripBtnViewFile
        '
        Me.ToolStripBtnViewFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnViewFile.Image = CType(resources.GetObject("ToolStripBtnViewFile.Image"), System.Drawing.Image)
        Me.ToolStripBtnViewFile.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnViewFile.Name = "ToolStripBtnViewFile"
        Me.ToolStripBtnViewFile.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnViewFile.Text = "View File"
        '
        'ToolStripSeparatorFAT
        '
        Me.ToolStripSeparatorFAT.Name = "ToolStripSeparatorFAT"
        Me.ToolStripSeparatorFAT.Size = New System.Drawing.Size(6, 25)
        '
        'ComboFAT
        '
        Me.ComboFAT.AutoSize = False
        Me.ComboFAT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboFAT.DropDownWidth = 25
        Me.ComboFAT.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.ComboFAT.Name = "ComboFAT"
        Me.ComboFAT.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ComboFAT.Size = New System.Drawing.Size(25, 23)
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusReadOnly, Me.ToolStripStatusCached, Me.ToolStripStatusModified, Me.ToolStripFileName, Me.ToolStripFileCount, Me.ToolStripFileSector, Me.ToolStripFileTrack, Me.ToolStripImageCount, Me.ToolStripModified})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 552)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.ShowItemToolTips = True
        Me.StatusStrip1.Size = New System.Drawing.Size(1004, 24)
        Me.StatusStrip1.TabIndex = 9
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusReadOnly
        '
        Me.ToolStripStatusReadOnly.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ToolStripStatusReadOnly.ForeColor = System.Drawing.Color.Red
        Me.ToolStripStatusReadOnly.Name = "ToolStripStatusReadOnly"
        Me.ToolStripStatusReadOnly.Size = New System.Drawing.Size(61, 19)
        Me.ToolStripStatusReadOnly.Text = "Read Only"
        '
        'ToolStripStatusCached
        '
        Me.ToolStripStatusCached.ActiveLinkColor = System.Drawing.Color.Red
        Me.ToolStripStatusCached.ForeColor = System.Drawing.Color.Green
        Me.ToolStripStatusCached.Name = "ToolStripStatusCached"
        Me.ToolStripStatusCached.Size = New System.Drawing.Size(47, 19)
        Me.ToolStripStatusCached.Text = "Cached"
        '
        'ToolStripStatusModified
        '
        Me.ToolStripStatusModified.ForeColor = System.Drawing.Color.Blue
        Me.ToolStripStatusModified.Name = "ToolStripStatusModified"
        Me.ToolStripStatusModified.Size = New System.Drawing.Size(55, 19)
        Me.ToolStripStatusModified.Text = "Modified"
        '
        'ToolStripFileName
        '
        Me.ToolStripFileName.AutoToolTip = True
        Me.ToolStripFileName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripFileName.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripFileName.Name = "ToolStripFileName"
        Me.ToolStripFileName.Size = New System.Drawing.Size(483, 19)
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
        'ToolStripFileSector
        '
        Me.ToolStripFileSector.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripFileSector.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripFileSector.Name = "ToolStripFileSector"
        Me.ToolStripFileSector.Size = New System.Drawing.Size(53, 19)
        Me.ToolStripFileSector.Text = "Sector 0"
        '
        'ToolStripFileTrack
        '
        Me.ToolStripFileTrack.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripFileTrack.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripFileTrack.Name = "ToolStripFileTrack"
        Me.ToolStripFileTrack.Size = New System.Drawing.Size(56, 19)
        Me.ToolStripFileTrack.Text = "Track 0.0"
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
        'ListViewSummary
        '
        Me.ListViewSummary.AllowDrop = True
        Me.ListViewSummary.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListViewSummary.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {SummaryName, SummaryValue})
        Me.ListViewSummary.FullRowSelect = True
        Me.ListViewSummary.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.ListViewSummary.HideSelection = False
        Me.ListViewSummary.Location = New System.Drawing.Point(12, 55)
        Me.ListViewSummary.MultiSelect = False
        Me.ListViewSummary.Name = "ListViewSummary"
        Me.ListViewSummary.OwnerDraw = True
        Me.ListViewSummary.Size = New System.Drawing.Size(305, 387)
        Me.ListViewSummary.TabIndex = 2
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
        Me.ComboImages.Location = New System.Drawing.Point(323, 55)
        Me.ComboImages.Name = "ComboImages"
        Me.ComboImages.Size = New System.Drawing.Size(598, 21)
        Me.ComboImages.Sorted = True
        Me.ComboImages.TabIndex = 3
        Me.ComboImages.Visible = False
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
        Me.ListViewHashes.Size = New System.Drawing.Size(305, 101)
        Me.ListViewHashes.TabIndex = 4
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
        Me.LabelDropMessage.Location = New System.Drawing.Point(522, 307)
        Me.LabelDropMessage.Name = "LabelDropMessage"
        Me.LabelDropMessage.Size = New System.Drawing.Size(273, 16)
        Me.LabelDropMessage.TabIndex = 8
        Me.LabelDropMessage.Text = "Drag && Drop Floppy Disk Images Here"
        Me.LabelDropMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ListViewFiles
        '
        Me.ListViewFiles.AllowDrop = True
        Me.ListViewFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListViewFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {FileModified, FileName, FileExt, FileSize, FileLastWriteDate, FileStartingCluster, Me.FileClusterError, FileAttrib, FileCRC32, Me.FileCreationDate, Me.FileLastAccessDate, Me.FileReserved, Me.FileLFN})
        Me.ListViewFiles.ContextMenuStrip = Me.ContextMenuFiles
        Me.ListViewFiles.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListViewFiles.FullRowSelect = True
        Me.ListViewFiles.HideSelection = False
        Me.ListViewFiles.Location = New System.Drawing.Point(323, 82)
        Me.ListViewFiles.Name = "ListViewFiles"
        Me.ListViewFiles.OwnerDraw = True
        Me.ListViewFiles.Size = New System.Drawing.Size(669, 467)
        Me.ListViewFiles.TabIndex = 7
        Me.ListViewFiles.UseCompatibleStateImageBehavior = False
        Me.ListViewFiles.View = System.Windows.Forms.View.Details
        '
        'FileClusterError
        '
        Me.FileClusterError.Text = "Err"
        Me.FileClusterError.Width = 30
        '
        'FileCreationDate
        '
        Me.FileCreationDate.Text = "Created"
        Me.FileCreationDate.Width = 140
        '
        'FileLastAccessDate
        '
        Me.FileLastAccessDate.Text = "Last Accessed"
        Me.FileLastAccessDate.Width = 90
        '
        'FileReserved
        '
        Me.FileReserved.Text = "Reserved"
        '
        'FileLFN
        '
        Me.FileLFN.Text = "Long File Name"
        Me.FileLFN.Width = 200
        '
        'ContextMenuFiles
        '
        Me.ContextMenuFiles.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnFileMenuFileProperties, Me.BtnFileMenuExportFile, Me.BtnFileMenuReplaceFile, Me.BtnFileMenuAddFile, FileMenuSeparatoor1, Me.BtnFileMenuViewDirectory, Me.FileMenuSeparatorDirectory, Me.BtnFileMenuViewFile, Me.BtnFileMenuViewFileText, Me.BtnFileMenuViewCrosslinked, FileMenuSeparatoor2, Me.BtnFileMenuDeleteFile, Me.BtnFileMenuUnDeleteFile, Me.BtnFileMenuRemoveDeletedFile, Me.BtnFileMenuDeleteFileWithFill, FileMenuSeparatoor3, Me.BtnFileMenuFixSize})
        Me.ContextMenuFiles.Name = "ContextMenuFiles"
        Me.ContextMenuFiles.Size = New System.Drawing.Size(223, 314)
        '
        'BtnFileMenuFileProperties
        '
        Me.BtnFileMenuFileProperties.Image = CType(resources.GetObject("BtnFileMenuFileProperties.Image"), System.Drawing.Image)
        Me.BtnFileMenuFileProperties.Name = "BtnFileMenuFileProperties"
        Me.BtnFileMenuFileProperties.Size = New System.Drawing.Size(222, 22)
        Me.BtnFileMenuFileProperties.Text = "Edit File &Properties"
        '
        'BtnFileMenuExportFile
        '
        Me.BtnFileMenuExportFile.Image = CType(resources.GetObject("BtnFileMenuExportFile.Image"), System.Drawing.Image)
        Me.BtnFileMenuExportFile.Name = "BtnFileMenuExportFile"
        Me.BtnFileMenuExportFile.Size = New System.Drawing.Size(222, 22)
        Me.BtnFileMenuExportFile.Text = "&Export File"
        '
        'BtnFileMenuReplaceFile
        '
        Me.BtnFileMenuReplaceFile.Name = "BtnFileMenuReplaceFile"
        Me.BtnFileMenuReplaceFile.Size = New System.Drawing.Size(222, 22)
        Me.BtnFileMenuReplaceFile.Text = "&Replace File"
        '
        'BtnFileMenuAddFile
        '
        Me.BtnFileMenuAddFile.Name = "BtnFileMenuAddFile"
        Me.BtnFileMenuAddFile.Size = New System.Drawing.Size(222, 22)
        Me.BtnFileMenuAddFile.Text = "&Add File"
        '
        'BtnFileMenuViewDirectory
        '
        Me.BtnFileMenuViewDirectory.Name = "BtnFileMenuViewDirectory"
        Me.BtnFileMenuViewDirectory.Size = New System.Drawing.Size(222, 22)
        Me.BtnFileMenuViewDirectory.Text = "View Parent D&irectory"
        '
        'FileMenuSeparatorDirectory
        '
        Me.FileMenuSeparatorDirectory.Name = "FileMenuSeparatorDirectory"
        Me.FileMenuSeparatorDirectory.Size = New System.Drawing.Size(219, 6)
        '
        'BtnFileMenuViewFile
        '
        Me.BtnFileMenuViewFile.Image = CType(resources.GetObject("BtnFileMenuViewFile.Image"), System.Drawing.Image)
        Me.BtnFileMenuViewFile.Name = "BtnFileMenuViewFile"
        Me.BtnFileMenuViewFile.Size = New System.Drawing.Size(222, 22)
        Me.BtnFileMenuViewFile.Text = "&View File"
        '
        'BtnFileMenuViewFileText
        '
        Me.BtnFileMenuViewFileText.Image = CType(resources.GetObject("BtnFileMenuViewFileText.Image"), System.Drawing.Image)
        Me.BtnFileMenuViewFileText.Name = "BtnFileMenuViewFileText"
        Me.BtnFileMenuViewFileText.Size = New System.Drawing.Size(222, 22)
        Me.BtnFileMenuViewFileText.Text = "View File as &Text"
        '
        'BtnFileMenuViewCrosslinked
        '
        Me.BtnFileMenuViewCrosslinked.Name = "BtnFileMenuViewCrosslinked"
        Me.BtnFileMenuViewCrosslinked.Size = New System.Drawing.Size(222, 22)
        Me.BtnFileMenuViewCrosslinked.Text = "View &Crosslinked Files"
        '
        'BtnFileMenuDeleteFile
        '
        Me.BtnFileMenuDeleteFile.Name = "BtnFileMenuDeleteFile"
        Me.BtnFileMenuDeleteFile.Size = New System.Drawing.Size(222, 22)
        Me.BtnFileMenuDeleteFile.Text = "&Delete File"
        '
        'BtnFileMenuUnDeleteFile
        '
        Me.BtnFileMenuUnDeleteFile.Name = "BtnFileMenuUnDeleteFile"
        Me.BtnFileMenuUnDeleteFile.Size = New System.Drawing.Size(222, 22)
        Me.BtnFileMenuUnDeleteFile.Text = "&Undelete File"
        '
        'BtnFileMenuRemoveDeletedFile
        '
        Me.BtnFileMenuRemoveDeletedFile.Name = "BtnFileMenuRemoveDeletedFile"
        Me.BtnFileMenuRemoveDeletedFile.Size = New System.Drawing.Size(222, 22)
        Me.BtnFileMenuRemoveDeletedFile.Text = "Remove &Deleted File"
        '
        'BtnFileMenuDeleteFileWithFill
        '
        Me.BtnFileMenuDeleteFileWithFill.Name = "BtnFileMenuDeleteFileWithFill"
        Me.BtnFileMenuDeleteFileWithFill.Size = New System.Drawing.Size(222, 22)
        Me.BtnFileMenuDeleteFileWithFill.Text = "&Delete File and Clear Sectors"
        '
        'BtnFileMenuFixSize
        '
        Me.BtnFileMenuFixSize.Name = "BtnFileMenuFixSize"
        Me.BtnFileMenuFixSize.Size = New System.Drawing.Size(222, 22)
        Me.BtnFileMenuFixSize.Text = "Fix File &Size"
        Me.BtnFileMenuFixSize.Visible = False
        '
        'MenuStripMain
        '
        Me.MenuStripMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {MainMenuFile, MainMenuEdit, Me.MainMenuFilters, MainMenuView, MainMenuTools, Me.DiskToolStripMenuItem, MainHelp, MainMenuExperimental})
        Me.MenuStripMain.Location = New System.Drawing.Point(0, 0)
        Me.MenuStripMain.Name = "MenuStripMain"
        Me.MenuStripMain.Padding = New System.Windows.Forms.Padding(6, 2, 12, 2)
        Me.MenuStripMain.Size = New System.Drawing.Size(1004, 24)
        Me.MenuStripMain.TabIndex = 0
        Me.MenuStripMain.Text = "MenuStrip1"
        '
        'MainMenuFilters
        '
        Me.MainMenuFilters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.MainMenuFilters.DropDown = Me.ContextMenuFilters
        Me.MainMenuFilters.Name = "MainMenuFilters"
        Me.MainMenuFilters.Size = New System.Drawing.Size(50, 20)
        Me.MainMenuFilters.Text = "F&ilters"
        '
        'ContextMenuFilters
        '
        Me.ContextMenuFilters.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnScanNew, Me.BtnScan, Me.BtnClearFilters})
        Me.ContextMenuFilters.Name = "ContextMenuStrip1"
        Me.ContextMenuFilters.OwnerItem = Me.MainMenuFilters
        Me.ContextMenuFilters.Size = New System.Drawing.Size(168, 70)
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
        'DiskToolStripMenuItem
        '
        Me.DiskToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnReadFloppyA, Me.BtnReadFloppyB, Me.ToolStripMenuItem2, Me.BtnWriteFloppyA, Me.BtnWriteFloppyB})
        Me.DiskToolStripMenuItem.Name = "DiskToolStripMenuItem"
        Me.DiskToolStripMenuItem.Size = New System.Drawing.Size(41, 20)
        Me.DiskToolStripMenuItem.Text = "Disk"
        '
        'BtnReadFloppyA
        '
        Me.BtnReadFloppyA.Name = "BtnReadFloppyA"
        Me.BtnReadFloppyA.Size = New System.Drawing.Size(181, 22)
        Me.BtnReadFloppyA.Text = "&Read Disk in Drive A"
        '
        'BtnReadFloppyB
        '
        Me.BtnReadFloppyB.Name = "BtnReadFloppyB"
        Me.BtnReadFloppyB.Size = New System.Drawing.Size(181, 22)
        Me.BtnReadFloppyB.Text = "&Read Disk in Drive B"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(178, 6)
        '
        'BtnWriteFloppyA
        '
        Me.BtnWriteFloppyA.Name = "BtnWriteFloppyA"
        Me.BtnWriteFloppyA.Size = New System.Drawing.Size(181, 22)
        Me.BtnWriteFloppyA.Text = "&Write Disk in Drive A"
        '
        'BtnWriteFloppyB
        '
        Me.BtnWriteFloppyB.Name = "BtnWriteFloppyB"
        Me.BtnWriteFloppyB.Size = New System.Drawing.Size(181, 22)
        Me.BtnWriteFloppyB.Text = "&Write Disk in Drive B"
        '
        'ComboImagesFiltered
        '
        Me.ComboImagesFiltered.AllowDrop = True
        Me.ComboImagesFiltered.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboImagesFiltered.BackColor = System.Drawing.SystemColors.Window
        Me.ComboImagesFiltered.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboImagesFiltered.DropDownWidth = 523
        Me.ComboImagesFiltered.Location = New System.Drawing.Point(323, 55)
        Me.ComboImagesFiltered.Name = "ComboImagesFiltered"
        Me.ComboImagesFiltered.Size = New System.Drawing.Size(598, 21)
        Me.ComboImagesFiltered.Sorted = True
        Me.ComboImagesFiltered.TabIndex = 5
        '
        'BtnResetSort
        '
        Me.BtnResetSort.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnResetSort.Location = New System.Drawing.Point(927, 54)
        Me.BtnResetSort.Name = "BtnResetSort"
        Me.BtnResetSort.Size = New System.Drawing.Size(65, 23)
        Me.BtnResetSort.TabIndex = 6
        Me.BtnResetSort.Text = "Reset Sort"
        Me.BtnResetSort.UseVisualStyleBackColor = True
        '
        'btnRetry
        '
        Me.btnRetry.Location = New System.Drawing.Point(223, 75)
        Me.btnRetry.Name = "btnRetry"
        Me.btnRetry.Size = New System.Drawing.Size(75, 23)
        Me.btnRetry.TabIndex = 3
        Me.btnRetry.Text = "Retry"
        Me.btnRetry.UseVisualStyleBackColor = True
        '
        'BtnRawTrackData
        '
        Me.BtnRawTrackData.Name = "BtnRawTrackData"
        Me.BtnRawTrackData.Size = New System.Drawing.Size(194, 22)
        Me.BtnRawTrackData.Text = "&Raw Track Data"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1004, 576)
        Me.Controls.Add(Me.btnRetry)
        Me.Controls.Add(Me.BtnResetSort)
        Me.Controls.Add(Me.ToolStripTop)
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
        Me.MinimumSize = New System.Drawing.Size(1020, 600)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Disk Image Tool"
        Me.ContextMenuEdit.ResumeLayout(False)
        Me.ToolStripTop.ResumeLayout(False)
        Me.ToolStripTop.PerformLayout()
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
    Friend WithEvents BtnDisplayBootSector As ToolStripMenuItem
    Friend WithEvents BtnDisplayDirectory As ToolStripMenuItem
    Friend WithEvents BtnDisplayClusters As ToolStripMenuItem
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
    Friend WithEvents BtnReplaceFile As ToolStripMenuItem
    Friend WithEvents BtnFileMenuReplaceFile As ToolStripMenuItem
    Friend WithEvents FileCreationDate As ColumnHeader
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
    Friend WithEvents ComboOEMName As ToolStripComboBox
    Friend WithEvents ToolStripOEMName As ToolStripLabel
    Friend WithEvents ToolStripStatusModified As ToolStripStatusLabel
    Friend WithEvents BtnDisplayBadSectors As ToolStripMenuItem
    Friend WithEvents BtnUndo As ToolStripMenuItem
    Friend WithEvents BtnRedo As ToolStripMenuItem
    Friend WithEvents ToolStripBtnOpen As ToolStripButton
    Friend WithEvents ToolStripSeparator6 As ToolStripSeparator
    Friend WithEvents LabelDropMessage As Label
    Friend WithEvents ToolStripBtnSave As ToolStripButton
    Friend WithEvents ToolStripBtnSaveAs As ToolStripButton
    Friend WithEvents ToolStripBtnSaveAll As ToolStripButton
    Friend WithEvents ToolStripSeparator7 As ToolStripSeparator
    Friend WithEvents ToolStripBtnClose As ToolStripButton
    Friend WithEvents ToolStripBtnCloseAll As ToolStripButton
    Friend WithEvents ToolStripSeparator8 As ToolStripSeparator
    Friend WithEvents ToolStripBtnUndo As ToolStripButton
    Friend WithEvents ToolStripBtnRedo As ToolStripButton
    Friend WithEvents ToolStripBtnFileProperties As ToolStripButton
    Friend WithEvents ToolStripBtnExportFile As ToolStripButton
    Friend WithEvents ToolStripSeparator9 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator10 As ToolStripSeparator
    Friend WithEvents ToolStripBtnViewFile As ToolStripButton
    Friend WithEvents ToolStripBtnViewFileText As ToolStripButton
    Friend WithEvents BtnDisplayDisk As ToolStripMenuItem
    Friend WithEvents BtnFileMenuRemoveDeletedFile As ToolStripMenuItem
    Friend WithEvents BtnFileMenuDeleteFile As ToolStripMenuItem
    Friend WithEvents BtnFileMenuDeleteFileWithFill As ToolStripMenuItem
    Friend WithEvents BtnWin9xClean As ToolStripMenuItem
    Friend WithEvents BtnFixImageSize As ToolStripMenuItem
    Friend WithEvents BtnHelpAbout As ToolStripMenuItem
    Friend WithEvents BtnHelpProjectPage As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator12 As ToolStripSeparator
    Friend WithEvents BtnHelpUpdateCheck As ToolStripMenuItem
    Friend WithEvents BtnEditFAT As ToolStripMenuItem
    Friend WithEvents ToolStripFileSector As ToolStripStatusLabel
    Friend WithEvents ToolStripFileTrack As ToolStripStatusLabel
    Friend WithEvents BtnFileMenuFixSize As ToolStripMenuItem
    Friend WithEvents BtnResetSort As Button
    Friend WithEvents BtnEditBootSector As ToolStripMenuItem
    Friend WithEvents ToolStripStatusReadOnly As ToolStripStatusLabel
    Friend WithEvents BtnRestoreBootSector As ToolStripMenuItem
    Friend WithEvents BtnRemoveBootSector As ToolStripMenuItem
    Friend WithEvents BtnFileMenuViewDirectory As ToolStripMenuItem
    Friend WithEvents FileMenuSeparatorDirectory As ToolStripSeparator
    Friend WithEvents ComboDiskType As ToolStripComboBox
    Friend WithEvents ToolStripDiskType As ToolStripLabel
    Friend WithEvents BtnWin9xCleanBatch As ToolStripMenuItem
    Friend WithEvents BtnCompare As ToolStripMenuItem
    Friend WithEvents BtnFileMenuUnDeleteFile As ToolStripMenuItem
    Friend WithEvents FileReserved As ColumnHeader
    Friend WithEvents BtnClearReservedBytes As ToolStripMenuItem
    Friend WithEvents BtnDisplayLostClusters As ToolStripMenuItem
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripSeparatorFAT As ToolStripSeparator
    Friend WithEvents ComboFAT As ToolStripComboBox
    Friend WithEvents ToolStripTop As ToolStrip
    Friend WithEvents DiskToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BtnReadFloppyA As ToolStripMenuItem
    Friend WithEvents BtnReadFloppyB As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents BtnWriteFloppyA As ToolStripMenuItem
    Friend WithEvents BtnWriteFloppyB As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As ToolStripSeparator
    Friend WithEvents btnCreateBackup As ToolStripMenuItem
    Friend WithEvents ContextMenuEdit As ContextMenuStrip
    Friend WithEvents btnRetry As Button
    Friend WithEvents BtnAddFile As ToolStripMenuItem
    Friend WithEvents BtnFileMenuAddFile As ToolStripMenuItem
    Friend WithEvents SubMenuFixImageSize As ToolStripMenuItem
    Friend WithEvents BtnTruncateImage As ToolStripMenuItem
    Friend WithEvents BtnRestructureImage As ToolStripMenuItem
    Friend WithEvents BtnDisplayOverdumpData As ToolStripMenuItem
    Friend WithEvents BtnHelpChangeLog As ToolStripMenuItem
    Friend WithEvents ToolStripStatusCached As ToolStripStatusLabel
    Friend WithEvents BtnReload As ToolStripMenuItem
    Friend WithEvents BtnRawTrackData As ToolStripMenuItem
End Class
