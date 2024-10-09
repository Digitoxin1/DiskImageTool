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
        Dim ToolStripSearchLabel As System.Windows.Forms.ToolStripLabel
        Dim MainMenuTools As System.Windows.Forms.ToolStripMenuItem
        Dim ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
        Dim MainHelp As System.Windows.Forms.ToolStripMenuItem
        Dim FileMenuSeparatoor2 As System.Windows.Forms.ToolStripSeparator
        Dim FileMenuSeparatoor3 As System.Windows.Forms.ToolStripSeparator
        Me.MenuFileOpen = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileReload = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileNewImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuFileSave = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileSaveAs = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileSaveAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuFileClose = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileCloseAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuFileExit = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuEdit = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.MenuEditBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditFAT = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuEditFileProperties = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditExportFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditReplaceFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditAddFiles = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuEditUndo = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditRedo = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditRevert = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuEditCreateBackup = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditWindowsExtensions = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexFAT = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexDirectory = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexFreeClusters = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexBadSectors = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexLostClusters = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexOverdumpData = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexRawTrackData = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexDisk = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparatorFile = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuHexFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnExportDebug = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsCompare = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsWin9xClean = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsClearReservedBytes = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsFixImageSize = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsFixImageSizeSubMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsTruncateImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsRestructureImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsRestoreBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsRemoveBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsWin9xCleanBatch = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHelpProjectPage = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHelpUpdateCheck = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHelpChangeLog = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator12 = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuHelpAbout = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripTop = New System.Windows.Forms.ToolStrip()
        Me.ToolStripSearchText = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripOEMNameCombo = New System.Windows.Forms.ToolStripComboBox()
        Me.ToolStripOEMNameLabel = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripDiskTypeCombo = New System.Windows.Forms.ToolStripComboBox()
        Me.ToolStripDiskTypeLabel = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripOpen = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSave = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSaveAs = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSaveAll = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripClose = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripCloseAll = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripFileProperties = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripExportFile = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripUndo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripRedo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator10 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripViewFileText = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripViewFile = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparatorFAT = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripFATCombo = New System.Windows.Forms.ToolStripComboBox()
        Me.StatusStripMain = New System.Windows.Forms.StatusStrip()
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
        Me.MenuFileFileProperties = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileExportFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileReplaceFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileAddFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileInsertFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileViewDirectory = New System.Windows.Forms.ToolStripMenuItem()
        Me.FileMenuSeparatorDirectory = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuFileViewFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileViewFileText = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileViewCrosslinked = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileDeleteFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileUnDeleteFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileRemove = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileDeleteFileWithFill = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileFixSize = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStripMain = New System.Windows.Forms.MenuStrip()
        Me.MainMenuFilters = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuFilters = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.MenuFiltersScanNew = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFiltersScan = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFiltersClear = New System.Windows.Forms.ToolStripMenuItem()
        Me.DiskToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskReadFloppyA = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskReadFloppyB = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuDiskWriteFloppyA = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskWriteFloppyB = New System.Windows.Forms.ToolStripMenuItem()
        Me.ComboImagesFiltered = New System.Windows.Forms.ComboBox()
        Me.BtnResetSort = New System.Windows.Forms.Button()
        Me.btnRetry = New System.Windows.Forms.Button()
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
        ToolStripSearchLabel = New System.Windows.Forms.ToolStripLabel()
        MainMenuTools = New System.Windows.Forms.ToolStripMenuItem()
        ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        MainHelp = New System.Windows.Forms.ToolStripMenuItem()
        FileMenuSeparatoor2 = New System.Windows.Forms.ToolStripSeparator()
        FileMenuSeparatoor3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ContextMenuEdit.SuspendLayout()
        Me.ToolStripTop.SuspendLayout()
        Me.StatusStripMain.SuspendLayout()
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
        MainMenuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuFileOpen, Me.MenuFileReload, Me.MenuFileNewImage, Me.toolStripSeparator, Me.MenuFileSave, Me.MenuFileSaveAs, Me.MenuFileSaveAll, Me.ToolStripSeparator3, Me.MenuFileClose, Me.MenuFileCloseAll, Me.toolStripSeparator1, Me.MenuFileExit})
        MainMenuFile.Name = "MainMenuFile"
        MainMenuFile.ShortcutKeyDisplayString = ""
        MainMenuFile.Size = New System.Drawing.Size(37, 20)
        MainMenuFile.Text = "&File"
        '
        'MenuFileOpen
        '
        Me.MenuFileOpen.Image = CType(resources.GetObject("MenuFileOpen.Image"), System.Drawing.Image)
        Me.MenuFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.MenuFileOpen.Name = "MenuFileOpen"
        Me.MenuFileOpen.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.MenuFileOpen.Size = New System.Drawing.Size(212, 22)
        Me.MenuFileOpen.Text = "&Open"
        '
        'MenuFileReload
        '
        Me.MenuFileReload.Name = "MenuFileReload"
        Me.MenuFileReload.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
        Me.MenuFileReload.Size = New System.Drawing.Size(212, 22)
        Me.MenuFileReload.Text = "&Reload from Disk"
        '
        'MenuFileNewImage
        '
        Me.MenuFileNewImage.Name = "MenuFileNewImage"
        Me.MenuFileNewImage.Size = New System.Drawing.Size(212, 22)
        Me.MenuFileNewImage.Text = "&New Image"
        '
        'toolStripSeparator
        '
        Me.toolStripSeparator.Name = "toolStripSeparator"
        Me.toolStripSeparator.Size = New System.Drawing.Size(209, 6)
        '
        'MenuFileSave
        '
        Me.MenuFileSave.Image = CType(resources.GetObject("MenuFileSave.Image"), System.Drawing.Image)
        Me.MenuFileSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.MenuFileSave.Name = "MenuFileSave"
        Me.MenuFileSave.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.MenuFileSave.Size = New System.Drawing.Size(212, 22)
        Me.MenuFileSave.Text = "&Save"
        '
        'MenuFileSaveAs
        '
        Me.MenuFileSaveAs.Image = CType(resources.GetObject("MenuFileSaveAs.Image"), System.Drawing.Image)
        Me.MenuFileSaveAs.Name = "MenuFileSaveAs"
        Me.MenuFileSaveAs.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Alt) _
            Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.MenuFileSaveAs.Size = New System.Drawing.Size(212, 22)
        Me.MenuFileSaveAs.Text = "Save &As"
        '
        'MenuFileSaveAll
        '
        Me.MenuFileSaveAll.Image = CType(resources.GetObject("MenuFileSaveAll.Image"), System.Drawing.Image)
        Me.MenuFileSaveAll.Name = "MenuFileSaveAll"
        Me.MenuFileSaveAll.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.MenuFileSaveAll.Size = New System.Drawing.Size(212, 22)
        Me.MenuFileSaveAll.Text = "Save All"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(209, 6)
        '
        'MenuFileClose
        '
        Me.MenuFileClose.Image = CType(resources.GetObject("MenuFileClose.Image"), System.Drawing.Image)
        Me.MenuFileClose.Name = "MenuFileClose"
        Me.MenuFileClose.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.W), System.Windows.Forms.Keys)
        Me.MenuFileClose.Size = New System.Drawing.Size(212, 22)
        Me.MenuFileClose.Text = "&Close"
        '
        'MenuFileCloseAll
        '
        Me.MenuFileCloseAll.Image = CType(resources.GetObject("MenuFileCloseAll.Image"), System.Drawing.Image)
        Me.MenuFileCloseAll.Name = "MenuFileCloseAll"
        Me.MenuFileCloseAll.ShortcutKeyDisplayString = "     Ctrl+Shift+W"
        Me.MenuFileCloseAll.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.W), System.Windows.Forms.Keys)
        Me.MenuFileCloseAll.Size = New System.Drawing.Size(212, 22)
        Me.MenuFileCloseAll.Text = "Close All"
        '
        'toolStripSeparator1
        '
        Me.toolStripSeparator1.Name = "toolStripSeparator1"
        Me.toolStripSeparator1.Size = New System.Drawing.Size(209, 6)
        '
        'MenuFileExit
        '
        Me.MenuFileExit.Image = CType(resources.GetObject("MenuFileExit.Image"), System.Drawing.Image)
        Me.MenuFileExit.Name = "MenuFileExit"
        Me.MenuFileExit.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.F4), System.Windows.Forms.Keys)
        Me.MenuFileExit.Size = New System.Drawing.Size(212, 22)
        Me.MenuFileExit.Text = "E&xit"
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
        Me.ContextMenuEdit.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuEditBootSector, Me.MenuEditFAT, Me.ToolStripSeparator4, Me.MenuEditFileProperties, Me.MenuEditExportFile, Me.MenuEditReplaceFile, Me.MenuEditAddFiles, Me.ToolStripSeparator2, Me.MenuEditUndo, Me.MenuEditRedo, Me.MenuEditRevert, Me.ToolStripSeparator5, Me.MenuEditCreateBackup, Me.MenuEditWindowsExtensions})
        Me.ContextMenuEdit.Name = "ContextMenuEdit"
        Me.ContextMenuEdit.Size = New System.Drawing.Size(195, 264)
        '
        'MenuEditBootSector
        '
        Me.MenuEditBootSector.Name = "MenuEditBootSector"
        Me.MenuEditBootSector.Size = New System.Drawing.Size(194, 22)
        Me.MenuEditBootSector.Text = "&Boot Sector"
        '
        'MenuEditFAT
        '
        Me.MenuEditFAT.Name = "MenuEditFAT"
        Me.MenuEditFAT.Size = New System.Drawing.Size(194, 22)
        Me.MenuEditFAT.Text = "File &Allocation Table"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(191, 6)
        '
        'MenuEditFileProperties
        '
        Me.MenuEditFileProperties.Image = CType(resources.GetObject("MenuEditFileProperties.Image"), System.Drawing.Image)
        Me.MenuEditFileProperties.Name = "MenuEditFileProperties"
        Me.MenuEditFileProperties.Size = New System.Drawing.Size(194, 22)
        Me.MenuEditFileProperties.Text = "File &Properties"
        '
        'MenuEditExportFile
        '
        Me.MenuEditExportFile.Image = CType(resources.GetObject("MenuEditExportFile.Image"), System.Drawing.Image)
        Me.MenuEditExportFile.Name = "MenuEditExportFile"
        Me.MenuEditExportFile.Size = New System.Drawing.Size(194, 22)
        Me.MenuEditExportFile.Text = "&Export File"
        '
        'MenuEditReplaceFile
        '
        Me.MenuEditReplaceFile.Name = "MenuEditReplaceFile"
        Me.MenuEditReplaceFile.Size = New System.Drawing.Size(194, 22)
        Me.MenuEditReplaceFile.Text = "&Replace File"
        '
        'MenuEditAddFiles
        '
        Me.MenuEditAddFiles.Name = "MenuEditAddFiles"
        Me.MenuEditAddFiles.Size = New System.Drawing.Size(194, 22)
        Me.MenuEditAddFiles.Text = "&Add Files"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(191, 6)
        '
        'MenuEditUndo
        '
        Me.MenuEditUndo.Image = CType(resources.GetObject("MenuEditUndo.Image"), System.Drawing.Image)
        Me.MenuEditUndo.Name = "MenuEditUndo"
        Me.MenuEditUndo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.MenuEditUndo.Size = New System.Drawing.Size(194, 22)
        Me.MenuEditUndo.Text = "&Undo"
        '
        'MenuEditRedo
        '
        Me.MenuEditRedo.Image = CType(resources.GetObject("MenuEditRedo.Image"), System.Drawing.Image)
        Me.MenuEditRedo.Name = "MenuEditRedo"
        Me.MenuEditRedo.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.MenuEditRedo.Size = New System.Drawing.Size(194, 22)
        Me.MenuEditRedo.Text = "&Redo"
        '
        'MenuEditRevert
        '
        Me.MenuEditRevert.Name = "MenuEditRevert"
        Me.MenuEditRevert.Size = New System.Drawing.Size(194, 22)
        Me.MenuEditRevert.Text = "&Revert Changes"
        Me.MenuEditRevert.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(191, 6)
        '
        'MenuEditCreateBackup
        '
        Me.MenuEditCreateBackup.CheckOnClick = True
        Me.MenuEditCreateBackup.Name = "MenuEditCreateBackup"
        Me.MenuEditCreateBackup.Size = New System.Drawing.Size(194, 22)
        Me.MenuEditCreateBackup.Text = "Create Backup on Save"
        '
        'MenuEditWindowsExtensions
        '
        Me.MenuEditWindowsExtensions.CheckOnClick = True
        Me.MenuEditWindowsExtensions.Name = "MenuEditWindowsExtensions"
        Me.MenuEditWindowsExtensions.Size = New System.Drawing.Size(194, 22)
        Me.MenuEditWindowsExtensions.Text = "Windows Extensions"
        Me.MenuEditWindowsExtensions.ToolTipText = "Enabled Created and Last Access dates and Long File Name support when adding file" &
    "s"
        '
        'MainMenuView
        '
        MainMenuView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuHexBootSector, Me.MenuHexFAT, Me.MenuHexDirectory, Me.MenuHexFreeClusters, Me.MenuHexBadSectors, Me.MenuHexLostClusters, Me.MenuHexOverdumpData, Me.MenuHexRawTrackData, Me.MenuHexDisk, Me.ToolStripSeparatorFile, Me.MenuHexFile})
        MainMenuView.Name = "MainMenuView"
        MainMenuView.Size = New System.Drawing.Size(40, 20)
        MainMenuView.Text = "&Hex"
        '
        'MenuHexBootSector
        '
        Me.MenuHexBootSector.Name = "MenuHexBootSector"
        Me.MenuHexBootSector.Size = New System.Drawing.Size(194, 22)
        Me.MenuHexBootSector.Text = "&Boot Sector"
        '
        'MenuHexFAT
        '
        Me.MenuHexFAT.Name = "MenuHexFAT"
        Me.MenuHexFAT.Size = New System.Drawing.Size(194, 22)
        Me.MenuHexFAT.Text = "File &Allocation Table"
        '
        'MenuHexDirectory
        '
        Me.MenuHexDirectory.Name = "MenuHexDirectory"
        Me.MenuHexDirectory.Size = New System.Drawing.Size(194, 22)
        Me.MenuHexDirectory.Text = "Root &Directory"
        '
        'MenuHexFreeClusters
        '
        Me.MenuHexFreeClusters.Name = "MenuHexFreeClusters"
        Me.MenuHexFreeClusters.Size = New System.Drawing.Size(194, 22)
        Me.MenuHexFreeClusters.Text = "Free &Clusters with Data"
        '
        'MenuHexBadSectors
        '
        Me.MenuHexBadSectors.Name = "MenuHexBadSectors"
        Me.MenuHexBadSectors.Size = New System.Drawing.Size(194, 22)
        Me.MenuHexBadSectors.Text = "Bad &Sectors"
        '
        'MenuHexLostClusters
        '
        Me.MenuHexLostClusters.Name = "MenuHexLostClusters"
        Me.MenuHexLostClusters.Size = New System.Drawing.Size(194, 22)
        Me.MenuHexLostClusters.Text = "&Lost Clusters"
        '
        'MenuHexOverdumpData
        '
        Me.MenuHexOverdumpData.Name = "MenuHexOverdumpData"
        Me.MenuHexOverdumpData.Size = New System.Drawing.Size(194, 22)
        Me.MenuHexOverdumpData.Text = "&Overdump Data"
        '
        'MenuHexRawTrackData
        '
        Me.MenuHexRawTrackData.Name = "MenuHexRawTrackData"
        Me.MenuHexRawTrackData.Size = New System.Drawing.Size(194, 22)
        Me.MenuHexRawTrackData.Text = "&Raw Track Data"
        '
        'MenuHexDisk
        '
        Me.MenuHexDisk.Name = "MenuHexDisk"
        Me.MenuHexDisk.Size = New System.Drawing.Size(194, 22)
        Me.MenuHexDisk.Text = "Entire &Disk"
        '
        'ToolStripSeparatorFile
        '
        Me.ToolStripSeparatorFile.Name = "ToolStripSeparatorFile"
        Me.ToolStripSeparatorFile.Size = New System.Drawing.Size(191, 6)
        '
        'MenuHexFile
        '
        Me.MenuHexFile.Name = "MenuHexFile"
        Me.MenuHexFile.Size = New System.Drawing.Size(194, 22)
        Me.MenuHexFile.Text = "&File"
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
        'ToolStripSearchLabel
        '
        ToolStripSearchLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        ToolStripSearchLabel.Margin = New System.Windows.Forms.Padding(8, 1, 0, 2)
        ToolStripSearchLabel.Name = "ToolStripSearchLabel"
        ToolStripSearchLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        ToolStripSearchLabel.Size = New System.Drawing.Size(42, 22)
        ToolStripSearchLabel.Text = "Search"
        '
        'MainMenuTools
        '
        MainMenuTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuToolsCompare, Me.MenuToolsWin9xClean, Me.MenuToolsClearReservedBytes, Me.MenuToolsFixImageSize, Me.MenuToolsFixImageSizeSubMenu, Me.MenuToolsRestoreBootSector, Me.MenuToolsRemoveBootSector, ToolStripMenuItem1, Me.MenuToolsWin9xCleanBatch})
        MainMenuTools.Name = "MainMenuTools"
        MainMenuTools.Size = New System.Drawing.Size(46, 20)
        MainMenuTools.Text = "&Tools"
        '
        'MenuToolsCompare
        '
        Me.MenuToolsCompare.Name = "MenuToolsCompare"
        Me.MenuToolsCompare.Size = New System.Drawing.Size(289, 22)
        Me.MenuToolsCompare.Text = "&Compare Images"
        '
        'MenuToolsWin9xClean
        '
        Me.MenuToolsWin9xClean.Name = "MenuToolsWin9xClean"
        Me.MenuToolsWin9xClean.Size = New System.Drawing.Size(289, 22)
        Me.MenuToolsWin9xClean.Text = "Remove &Windows Modifications"
        '
        'MenuToolsClearReservedBytes
        '
        Me.MenuToolsClearReservedBytes.Name = "MenuToolsClearReservedBytes"
        Me.MenuToolsClearReservedBytes.Size = New System.Drawing.Size(289, 22)
        Me.MenuToolsClearReservedBytes.Text = "Clear &Reserved Bytes"
        '
        'MenuToolsFixImageSize
        '
        Me.MenuToolsFixImageSize.Name = "MenuToolsFixImageSize"
        Me.MenuToolsFixImageSize.Size = New System.Drawing.Size(289, 22)
        Me.MenuToolsFixImageSize.Text = "Fix Image &Size"
        '
        'MenuToolsFixImageSizeSubMenu
        '
        Me.MenuToolsFixImageSizeSubMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuToolsTruncateImage, Me.MenuToolsRestructureImage})
        Me.MenuToolsFixImageSizeSubMenu.Name = "MenuToolsFixImageSizeSubMenu"
        Me.MenuToolsFixImageSizeSubMenu.Size = New System.Drawing.Size(289, 22)
        Me.MenuToolsFixImageSizeSubMenu.Text = "Fix Image &Size"
        '
        'MenuToolsTruncateImage
        '
        Me.MenuToolsTruncateImage.Name = "MenuToolsTruncateImage"
        Me.MenuToolsTruncateImage.Size = New System.Drawing.Size(180, 22)
        Me.MenuToolsTruncateImage.Text = "&Truncate Image"
        '
        'MenuToolsRestructureImage
        '
        Me.MenuToolsRestructureImage.Name = "MenuToolsRestructureImage"
        Me.MenuToolsRestructureImage.Size = New System.Drawing.Size(180, 22)
        Me.MenuToolsRestructureImage.Text = "&Restructure Image"
        '
        'MenuToolsRestoreBootSector
        '
        Me.MenuToolsRestoreBootSector.Name = "MenuToolsRestoreBootSector"
        Me.MenuToolsRestoreBootSector.Size = New System.Drawing.Size(289, 22)
        Me.MenuToolsRestoreBootSector.Text = "Restore &Boot Sector from Root Directory"
        '
        'MenuToolsRemoveBootSector
        '
        Me.MenuToolsRemoveBootSector.Name = "MenuToolsRemoveBootSector"
        Me.MenuToolsRemoveBootSector.Size = New System.Drawing.Size(289, 22)
        Me.MenuToolsRemoveBootSector.Text = "Remove &Boot Sector from Root Directory"
        '
        'ToolStripMenuItem1
        '
        ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        ToolStripMenuItem1.Size = New System.Drawing.Size(286, 6)
        '
        'MenuToolsWin9xCleanBatch
        '
        Me.MenuToolsWin9xCleanBatch.Name = "MenuToolsWin9xCleanBatch"
        Me.MenuToolsWin9xCleanBatch.Size = New System.Drawing.Size(289, 22)
        Me.MenuToolsWin9xCleanBatch.Text = "Batch Remove Windows Modifications"
        '
        'MainHelp
        '
        MainHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuHelpProjectPage, Me.MenuHelpUpdateCheck, Me.MenuHelpChangeLog, Me.ToolStripSeparator12, Me.MenuHelpAbout})
        MainHelp.Name = "MainHelp"
        MainHelp.Size = New System.Drawing.Size(24, 20)
        MainHelp.Text = "?"
        '
        'MenuHelpProjectPage
        '
        Me.MenuHelpProjectPage.Name = "MenuHelpProjectPage"
        Me.MenuHelpProjectPage.Size = New System.Drawing.Size(212, 22)
        Me.MenuHelpProjectPage.Text = "&Project Page"
        '
        'MenuHelpUpdateCheck
        '
        Me.MenuHelpUpdateCheck.Name = "MenuHelpUpdateCheck"
        Me.MenuHelpUpdateCheck.Size = New System.Drawing.Size(212, 22)
        Me.MenuHelpUpdateCheck.Text = "Check for &Updates"
        '
        'MenuHelpChangeLog
        '
        Me.MenuHelpChangeLog.Name = "MenuHelpChangeLog"
        Me.MenuHelpChangeLog.Size = New System.Drawing.Size(212, 22)
        Me.MenuHelpChangeLog.Text = "&Change Log"
        '
        'ToolStripSeparator12
        '
        Me.ToolStripSeparator12.Name = "ToolStripSeparator12"
        Me.ToolStripSeparator12.Size = New System.Drawing.Size(209, 6)
        '
        'MenuHelpAbout
        '
        Me.MenuHelpAbout.Name = "MenuHelpAbout"
        Me.MenuHelpAbout.ShortcutKeys = System.Windows.Forms.Keys.F1
        Me.MenuHelpAbout.Size = New System.Drawing.Size(212, 22)
        Me.MenuHelpAbout.Text = "About Disk Image Tool"
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
        Me.ToolStripTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripSearchText, ToolStripSearchLabel, Me.ToolStripOEMNameCombo, Me.ToolStripOEMNameLabel, Me.ToolStripDiskTypeCombo, Me.ToolStripDiskTypeLabel, Me.ToolStripOpen, Me.ToolStripSeparator6, Me.ToolStripSave, Me.ToolStripSaveAs, Me.ToolStripSaveAll, Me.ToolStripSeparator7, Me.ToolStripClose, Me.ToolStripCloseAll, Me.ToolStripSeparator8, Me.ToolStripFileProperties, Me.ToolStripExportFile, Me.ToolStripSeparator9, Me.ToolStripUndo, Me.ToolStripRedo, Me.ToolStripSeparator10, Me.ToolStripViewFileText, Me.ToolStripViewFile, Me.ToolStripSeparatorFAT, Me.ToolStripFATCombo})
        Me.ToolStripTop.Location = New System.Drawing.Point(0, 24)
        Me.ToolStripTop.Name = "ToolStripTop"
        Me.ToolStripTop.Padding = New System.Windows.Forms.Padding(12, 0, 12, 0)
        Me.ToolStripTop.Size = New System.Drawing.Size(1004, 25)
        Me.ToolStripTop.TabIndex = 1
        Me.ToolStripTop.Text = "ToolStrip1"
        '
        'ToolStripSearchText
        '
        Me.ToolStripSearchText.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripSearchText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ToolStripSearchText.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ToolStripSearchText.Margin = New System.Windows.Forms.Padding(1, 0, 0, 0)
        Me.ToolStripSearchText.MaxLength = 255
        Me.ToolStripSearchText.Name = "ToolStripSearchText"
        Me.ToolStripSearchText.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripSearchText.Size = New System.Drawing.Size(195, 25)
        '
        'ToolStripOEMNameCombo
        '
        Me.ToolStripOEMNameCombo.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripOEMNameCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ToolStripOEMNameCombo.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.ToolStripOEMNameCombo.Name = "ToolStripOEMNameCombo"
        Me.ToolStripOEMNameCombo.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripOEMNameCombo.Size = New System.Drawing.Size(125, 25)
        Me.ToolStripOEMNameCombo.Sorted = True
        '
        'ToolStripOEMNameLabel
        '
        Me.ToolStripOEMNameLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripOEMNameLabel.Margin = New System.Windows.Forms.Padding(8, 1, 0, 2)
        Me.ToolStripOEMNameLabel.Name = "ToolStripOEMNameLabel"
        Me.ToolStripOEMNameLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripOEMNameLabel.Size = New System.Drawing.Size(68, 22)
        Me.ToolStripOEMNameLabel.Text = "OEM Name"
        '
        'ToolStripDiskTypeCombo
        '
        Me.ToolStripDiskTypeCombo.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripDiskTypeCombo.AutoSize = False
        Me.ToolStripDiskTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ToolStripDiskTypeCombo.DropDownWidth = 95
        Me.ToolStripDiskTypeCombo.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.ToolStripDiskTypeCombo.Name = "ToolStripDiskTypeCombo"
        Me.ToolStripDiskTypeCombo.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripDiskTypeCombo.Size = New System.Drawing.Size(95, 23)
        '
        'ToolStripDiskTypeLabel
        '
        Me.ToolStripDiskTypeLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripDiskTypeLabel.Name = "ToolStripDiskTypeLabel"
        Me.ToolStripDiskTypeLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripDiskTypeLabel.Size = New System.Drawing.Size(56, 22)
        Me.ToolStripDiskTypeLabel.Text = "Disk Type"
        '
        'ToolStripOpen
        '
        Me.ToolStripOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripOpen.Image = CType(resources.GetObject("ToolStripOpen.Image"), System.Drawing.Image)
        Me.ToolStripOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripOpen.Name = "ToolStripOpen"
        Me.ToolStripOpen.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripOpen.Text = "Open"
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripSave
        '
        Me.ToolStripSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripSave.Image = CType(resources.GetObject("ToolStripSave.Image"), System.Drawing.Image)
        Me.ToolStripSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSave.Name = "ToolStripSave"
        Me.ToolStripSave.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripSave.Text = "Save"
        '
        'ToolStripSaveAs
        '
        Me.ToolStripSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripSaveAs.Image = CType(resources.GetObject("ToolStripSaveAs.Image"), System.Drawing.Image)
        Me.ToolStripSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSaveAs.Name = "ToolStripSaveAs"
        Me.ToolStripSaveAs.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripSaveAs.Text = "Save As"
        '
        'ToolStripSaveAll
        '
        Me.ToolStripSaveAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripSaveAll.Image = CType(resources.GetObject("ToolStripSaveAll.Image"), System.Drawing.Image)
        Me.ToolStripSaveAll.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSaveAll.Name = "ToolStripSaveAll"
        Me.ToolStripSaveAll.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripSaveAll.Text = "Save All"
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        Me.ToolStripSeparator7.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripClose
        '
        Me.ToolStripClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripClose.Image = CType(resources.GetObject("ToolStripClose.Image"), System.Drawing.Image)
        Me.ToolStripClose.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripClose.Name = "ToolStripClose"
        Me.ToolStripClose.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripClose.Text = "Close"
        '
        'ToolStripCloseAll
        '
        Me.ToolStripCloseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripCloseAll.Image = CType(resources.GetObject("ToolStripCloseAll.Image"), System.Drawing.Image)
        Me.ToolStripCloseAll.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripCloseAll.Name = "ToolStripCloseAll"
        Me.ToolStripCloseAll.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripCloseAll.Text = "Close All"
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        Me.ToolStripSeparator8.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripFileProperties
        '
        Me.ToolStripFileProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripFileProperties.Image = CType(resources.GetObject("ToolStripFileProperties.Image"), System.Drawing.Image)
        Me.ToolStripFileProperties.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripFileProperties.Name = "ToolStripFileProperties"
        Me.ToolStripFileProperties.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripFileProperties.Text = "File Properties"
        '
        'ToolStripExportFile
        '
        Me.ToolStripExportFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripExportFile.Image = CType(resources.GetObject("ToolStripExportFile.Image"), System.Drawing.Image)
        Me.ToolStripExportFile.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripExportFile.Name = "ToolStripExportFile"
        Me.ToolStripExportFile.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripExportFile.Text = "Export File"
        '
        'ToolStripSeparator9
        '
        Me.ToolStripSeparator9.Name = "ToolStripSeparator9"
        Me.ToolStripSeparator9.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripUndo
        '
        Me.ToolStripUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripUndo.Image = CType(resources.GetObject("ToolStripUndo.Image"), System.Drawing.Image)
        Me.ToolStripUndo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripUndo.Name = "ToolStripUndo"
        Me.ToolStripUndo.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripUndo.Text = "Undo"
        '
        'ToolStripRedo
        '
        Me.ToolStripRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripRedo.Image = CType(resources.GetObject("ToolStripRedo.Image"), System.Drawing.Image)
        Me.ToolStripRedo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripRedo.Name = "ToolStripRedo"
        Me.ToolStripRedo.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripRedo.Text = "Redo"
        '
        'ToolStripSeparator10
        '
        Me.ToolStripSeparator10.Name = "ToolStripSeparator10"
        Me.ToolStripSeparator10.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripViewFileText
        '
        Me.ToolStripViewFileText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripViewFileText.Image = CType(resources.GetObject("ToolStripViewFileText.Image"), System.Drawing.Image)
        Me.ToolStripViewFileText.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripViewFileText.Name = "ToolStripViewFileText"
        Me.ToolStripViewFileText.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripViewFileText.Text = "View File as Text"
        '
        'ToolStripViewFile
        '
        Me.ToolStripViewFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripViewFile.Image = CType(resources.GetObject("ToolStripViewFile.Image"), System.Drawing.Image)
        Me.ToolStripViewFile.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripViewFile.Name = "ToolStripViewFile"
        Me.ToolStripViewFile.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripViewFile.Text = "View File"
        '
        'ToolStripSeparatorFAT
        '
        Me.ToolStripSeparatorFAT.Name = "ToolStripSeparatorFAT"
        Me.ToolStripSeparatorFAT.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripFATCombo
        '
        Me.ToolStripFATCombo.AutoSize = False
        Me.ToolStripFATCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ToolStripFATCombo.DropDownWidth = 25
        Me.ToolStripFATCombo.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.ToolStripFATCombo.Name = "ToolStripFATCombo"
        Me.ToolStripFATCombo.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripFATCombo.Size = New System.Drawing.Size(25, 23)
        '
        'StatusStripMain
        '
        Me.StatusStripMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusReadOnly, Me.ToolStripStatusCached, Me.ToolStripStatusModified, Me.ToolStripFileName, Me.ToolStripFileCount, Me.ToolStripFileSector, Me.ToolStripFileTrack, Me.ToolStripImageCount, Me.ToolStripModified})
        Me.StatusStripMain.Location = New System.Drawing.Point(0, 552)
        Me.StatusStripMain.Name = "StatusStripMain"
        Me.StatusStripMain.ShowItemToolTips = True
        Me.StatusStripMain.Size = New System.Drawing.Size(1004, 24)
        Me.StatusStripMain.TabIndex = 9
        Me.StatusStripMain.Text = "StatusStripMain"
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
        Me.ContextMenuFiles.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuFileFileProperties, Me.MenuFileExportFile, Me.MenuFileReplaceFile, Me.MenuFileAddFile, Me.MenuFileInsertFile, FileMenuSeparatoor1, Me.MenuFileViewDirectory, Me.FileMenuSeparatorDirectory, Me.MenuFileViewFile, Me.MenuFileViewFileText, Me.MenuFileViewCrosslinked, FileMenuSeparatoor2, Me.MenuFileDeleteFile, Me.MenuFileUnDeleteFile, Me.MenuFileRemove, Me.MenuFileDeleteFileWithFill, FileMenuSeparatoor3, Me.MenuFileFixSize})
        Me.ContextMenuFiles.Name = "ContextMenuFiles"
        Me.ContextMenuFiles.Size = New System.Drawing.Size(223, 336)
        '
        'MenuFileFileProperties
        '
        Me.MenuFileFileProperties.Image = CType(resources.GetObject("MenuFileFileProperties.Image"), System.Drawing.Image)
        Me.MenuFileFileProperties.Name = "MenuFileFileProperties"
        Me.MenuFileFileProperties.Size = New System.Drawing.Size(222, 22)
        Me.MenuFileFileProperties.Text = "Edit File &Properties"
        '
        'MenuFileExportFile
        '
        Me.MenuFileExportFile.Image = CType(resources.GetObject("MenuFileExportFile.Image"), System.Drawing.Image)
        Me.MenuFileExportFile.Name = "MenuFileExportFile"
        Me.MenuFileExportFile.Size = New System.Drawing.Size(222, 22)
        Me.MenuFileExportFile.Text = "&Export File"
        '
        'MenuFileReplaceFile
        '
        Me.MenuFileReplaceFile.Name = "MenuFileReplaceFile"
        Me.MenuFileReplaceFile.Size = New System.Drawing.Size(222, 22)
        Me.MenuFileReplaceFile.Text = "&Replace File"
        '
        'MenuFileAddFile
        '
        Me.MenuFileAddFile.Name = "MenuFileAddFile"
        Me.MenuFileAddFile.Size = New System.Drawing.Size(222, 22)
        Me.MenuFileAddFile.Text = "&Add Files"
        '
        'MenuFileInsertFile
        '
        Me.MenuFileInsertFile.Name = "MenuFileInsertFile"
        Me.MenuFileInsertFile.Size = New System.Drawing.Size(222, 22)
        Me.MenuFileInsertFile.Text = "&Insert File"
        '
        'MenuFileViewDirectory
        '
        Me.MenuFileViewDirectory.Name = "MenuFileViewDirectory"
        Me.MenuFileViewDirectory.Size = New System.Drawing.Size(222, 22)
        Me.MenuFileViewDirectory.Text = "View Parent D&irectory"
        '
        'FileMenuSeparatorDirectory
        '
        Me.FileMenuSeparatorDirectory.Name = "FileMenuSeparatorDirectory"
        Me.FileMenuSeparatorDirectory.Size = New System.Drawing.Size(219, 6)
        '
        'MenuFileViewFile
        '
        Me.MenuFileViewFile.Image = CType(resources.GetObject("MenuFileViewFile.Image"), System.Drawing.Image)
        Me.MenuFileViewFile.Name = "MenuFileViewFile"
        Me.MenuFileViewFile.Size = New System.Drawing.Size(222, 22)
        Me.MenuFileViewFile.Text = "&View File"
        '
        'MenuFileViewFileText
        '
        Me.MenuFileViewFileText.Image = CType(resources.GetObject("MenuFileViewFileText.Image"), System.Drawing.Image)
        Me.MenuFileViewFileText.Name = "MenuFileViewFileText"
        Me.MenuFileViewFileText.Size = New System.Drawing.Size(222, 22)
        Me.MenuFileViewFileText.Text = "View File as &Text"
        '
        'MenuFileViewCrosslinked
        '
        Me.MenuFileViewCrosslinked.Name = "MenuFileViewCrosslinked"
        Me.MenuFileViewCrosslinked.Size = New System.Drawing.Size(222, 22)
        Me.MenuFileViewCrosslinked.Text = "View &Crosslinked Files"
        '
        'MenuFileDeleteFile
        '
        Me.MenuFileDeleteFile.Name = "MenuFileDeleteFile"
        Me.MenuFileDeleteFile.Size = New System.Drawing.Size(222, 22)
        Me.MenuFileDeleteFile.Text = "&Delete File"
        '
        'MenuFileUnDeleteFile
        '
        Me.MenuFileUnDeleteFile.Name = "MenuFileUnDeleteFile"
        Me.MenuFileUnDeleteFile.Size = New System.Drawing.Size(222, 22)
        Me.MenuFileUnDeleteFile.Text = "&Undelete File"
        '
        'MenuFileRemove
        '
        Me.MenuFileRemove.Name = "MenuFileRemove"
        Me.MenuFileRemove.Size = New System.Drawing.Size(222, 22)
        Me.MenuFileRemove.Text = "Remove &Deleted File"
        '
        'MenuFileDeleteFileWithFill
        '
        Me.MenuFileDeleteFileWithFill.Name = "MenuFileDeleteFileWithFill"
        Me.MenuFileDeleteFileWithFill.Size = New System.Drawing.Size(222, 22)
        Me.MenuFileDeleteFileWithFill.Text = "&Delete File and Clear Sectors"
        '
        'MenuFileFixSize
        '
        Me.MenuFileFixSize.Name = "MenuFileFixSize"
        Me.MenuFileFixSize.Size = New System.Drawing.Size(222, 22)
        Me.MenuFileFixSize.Text = "Fix File &Size"
        Me.MenuFileFixSize.Visible = False
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
        Me.ContextMenuFilters.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuFiltersScanNew, Me.MenuFiltersScan, Me.MenuFiltersClear})
        Me.ContextMenuFilters.Name = "ContextMenuStrip1"
        Me.ContextMenuFilters.Size = New System.Drawing.Size(168, 70)
        '
        'MenuFiltersScanNew
        '
        Me.MenuFiltersScanNew.Name = "MenuFiltersScanNew"
        Me.MenuFiltersScanNew.Size = New System.Drawing.Size(167, 22)
        Me.MenuFiltersScanNew.Text = "Scan &New Images"
        '
        'MenuFiltersScan
        '
        Me.MenuFiltersScan.Name = "MenuFiltersScan"
        Me.MenuFiltersScan.Size = New System.Drawing.Size(167, 22)
        Me.MenuFiltersScan.Text = "&Scan Images"
        '
        'MenuFiltersClear
        '
        Me.MenuFiltersClear.Name = "MenuFiltersClear"
        Me.MenuFiltersClear.Size = New System.Drawing.Size(167, 22)
        Me.MenuFiltersClear.Text = "Clear Filters"
        '
        'DiskToolStripMenuItem
        '
        Me.DiskToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuDiskReadFloppyA, Me.MenuDiskReadFloppyB, Me.ToolStripMenuItem2, Me.MenuDiskWriteFloppyA, Me.MenuDiskWriteFloppyB})
        Me.DiskToolStripMenuItem.Name = "DiskToolStripMenuItem"
        Me.DiskToolStripMenuItem.Size = New System.Drawing.Size(41, 20)
        Me.DiskToolStripMenuItem.Text = "Disk"
        '
        'MenuDiskReadFloppyA
        '
        Me.MenuDiskReadFloppyA.Name = "MenuDiskReadFloppyA"
        Me.MenuDiskReadFloppyA.Size = New System.Drawing.Size(181, 22)
        Me.MenuDiskReadFloppyA.Text = "&Read Disk in Drive A"
        '
        'MenuDiskReadFloppyB
        '
        Me.MenuDiskReadFloppyB.Name = "MenuDiskReadFloppyB"
        Me.MenuDiskReadFloppyB.Size = New System.Drawing.Size(181, 22)
        Me.MenuDiskReadFloppyB.Text = "&Read Disk in Drive B"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(178, 6)
        '
        'MenuDiskWriteFloppyA
        '
        Me.MenuDiskWriteFloppyA.Name = "MenuDiskWriteFloppyA"
        Me.MenuDiskWriteFloppyA.Size = New System.Drawing.Size(181, 22)
        Me.MenuDiskWriteFloppyA.Text = "&Write Disk in Drive A"
        '
        'MenuDiskWriteFloppyB
        '
        Me.MenuDiskWriteFloppyB.Name = "MenuDiskWriteFloppyB"
        Me.MenuDiskWriteFloppyB.Size = New System.Drawing.Size(181, 22)
        Me.MenuDiskWriteFloppyB.Text = "&Write Disk in Drive B"
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
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1004, 576)
        Me.Controls.Add(Me.btnRetry)
        Me.Controls.Add(Me.BtnResetSort)
        Me.Controls.Add(Me.ToolStripTop)
        Me.Controls.Add(Me.ComboImagesFiltered)
        Me.Controls.Add(Me.StatusStripMain)
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
        Me.StatusStripMain.ResumeLayout(False)
        Me.StatusStripMain.PerformLayout()
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
    Friend WithEvents MenuFileOpen As ToolStripMenuItem
    Friend WithEvents toolStripSeparator As ToolStripSeparator
    Friend WithEvents MenuFileSave As ToolStripMenuItem
    Friend WithEvents MenuFileSaveAs As ToolStripMenuItem
    Friend WithEvents toolStripSeparator1 As ToolStripSeparator
    Friend WithEvents MenuFileExit As ToolStripMenuItem
    Friend WithEvents ContextMenuFilters As ContextMenuStrip
    Friend WithEvents MenuFileSaveAll As ToolStripMenuItem
    Friend WithEvents MenuFiltersScan As ToolStripMenuItem
    Friend WithEvents MenuHexBootSector As ToolStripMenuItem
    Friend WithEvents MenuHexDirectory As ToolStripMenuItem
    Friend WithEvents MenuHexFreeClusters As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents MenuEditRevert As ToolStripMenuItem
    Friend WithEvents MenuHexFile As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents MenuFileClose As ToolStripMenuItem
    Friend WithEvents MenuFileCloseAll As ToolStripMenuItem
    Friend WithEvents MenuFiltersScanNew As ToolStripMenuItem
    Friend WithEvents BtnExportDebug As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents ToolStripFileCount As ToolStripStatusLabel
    Friend WithEvents MenuEditFileProperties As ToolStripMenuItem
    Friend WithEvents ContextMenuFiles As ContextMenuStrip
    Friend WithEvents MenuFileFileProperties As ToolStripMenuItem
    Friend WithEvents MenuFileViewFile As ToolStripMenuItem
    Friend WithEvents MenuFileViewFileText As ToolStripMenuItem
    Friend WithEvents MainMenuFilters As ToolStripMenuItem
    Friend WithEvents MenuEditReplaceFile As ToolStripMenuItem
    Friend WithEvents MenuFileReplaceFile As ToolStripMenuItem
    Friend WithEvents FileCreationDate As ColumnHeader
    Friend WithEvents FileLastAccessDate As ColumnHeader
    Friend WithEvents FileLFN As ColumnHeader
    Friend WithEvents MenuHexFAT As ToolStripMenuItem
    Friend WithEvents FileClusterError As ColumnHeader
    Friend WithEvents MenuFileViewCrosslinked As ToolStripMenuItem
    Friend WithEvents MenuEditExportFile As ToolStripMenuItem
    Friend WithEvents MenuFileExportFile As ToolStripMenuItem
    Friend WithEvents ToolStripSearchText As ToolStripTextBox
    Friend WithEvents ComboImagesFiltered As ComboBox
    Friend WithEvents MenuFiltersClear As ToolStripMenuItem
    Friend WithEvents ToolStripOEMNameCombo As ToolStripComboBox
    Friend WithEvents ToolStripOEMNameLabel As ToolStripLabel
    Friend WithEvents ToolStripStatusModified As ToolStripStatusLabel
    Friend WithEvents MenuHexBadSectors As ToolStripMenuItem
    Friend WithEvents MenuEditUndo As ToolStripMenuItem
    Friend WithEvents MenuEditRedo As ToolStripMenuItem
    Friend WithEvents ToolStripOpen As ToolStripButton
    Friend WithEvents ToolStripSeparator6 As ToolStripSeparator
    Friend WithEvents LabelDropMessage As Label
    Friend WithEvents ToolStripSave As ToolStripButton
    Friend WithEvents ToolStripSaveAs As ToolStripButton
    Friend WithEvents ToolStripSaveAll As ToolStripButton
    Friend WithEvents ToolStripSeparator7 As ToolStripSeparator
    Friend WithEvents ToolStripClose As ToolStripButton
    Friend WithEvents ToolStripCloseAll As ToolStripButton
    Friend WithEvents ToolStripSeparator8 As ToolStripSeparator
    Friend WithEvents ToolStripUndo As ToolStripButton
    Friend WithEvents ToolStripRedo As ToolStripButton
    Friend WithEvents ToolStripFileProperties As ToolStripButton
    Friend WithEvents ToolStripExportFile As ToolStripButton
    Friend WithEvents ToolStripSeparator9 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator10 As ToolStripSeparator
    Friend WithEvents ToolStripViewFile As ToolStripButton
    Friend WithEvents ToolStripViewFileText As ToolStripButton
    Friend WithEvents MenuHexDisk As ToolStripMenuItem
    Friend WithEvents MenuFileRemove As ToolStripMenuItem
    Friend WithEvents MenuFileDeleteFile As ToolStripMenuItem
    Friend WithEvents MenuFileDeleteFileWithFill As ToolStripMenuItem
    Friend WithEvents MenuToolsWin9xClean As ToolStripMenuItem
    Friend WithEvents MenuToolsFixImageSize As ToolStripMenuItem
    Friend WithEvents MenuHelpAbout As ToolStripMenuItem
    Friend WithEvents MenuHelpProjectPage As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator12 As ToolStripSeparator
    Friend WithEvents MenuHelpUpdateCheck As ToolStripMenuItem
    Friend WithEvents MenuEditFAT As ToolStripMenuItem
    Friend WithEvents ToolStripFileSector As ToolStripStatusLabel
    Friend WithEvents ToolStripFileTrack As ToolStripStatusLabel
    Friend WithEvents MenuFileFixSize As ToolStripMenuItem
    Friend WithEvents BtnResetSort As Button
    Friend WithEvents MenuEditBootSector As ToolStripMenuItem
    Friend WithEvents ToolStripStatusReadOnly As ToolStripStatusLabel
    Friend WithEvents MenuToolsRestoreBootSector As ToolStripMenuItem
    Friend WithEvents MenuToolsRemoveBootSector As ToolStripMenuItem
    Friend WithEvents MenuFileViewDirectory As ToolStripMenuItem
    Friend WithEvents FileMenuSeparatorDirectory As ToolStripSeparator
    Friend WithEvents ToolStripDiskTypeCombo As ToolStripComboBox
    Friend WithEvents ToolStripDiskTypeLabel As ToolStripLabel
    Friend WithEvents MenuToolsWin9xCleanBatch As ToolStripMenuItem
    Friend WithEvents MenuToolsCompare As ToolStripMenuItem
    Friend WithEvents MenuFileUnDeleteFile As ToolStripMenuItem
    Friend WithEvents FileReserved As ColumnHeader
    Friend WithEvents MenuToolsClearReservedBytes As ToolStripMenuItem
    Friend WithEvents MenuHexLostClusters As ToolStripMenuItem
    Friend WithEvents StatusStripMain As StatusStrip
    Friend WithEvents ToolStripSeparatorFAT As ToolStripSeparator
    Friend WithEvents ToolStripFATCombo As ToolStripComboBox
    Friend WithEvents ToolStripTop As ToolStrip
    Friend WithEvents DiskToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MenuDiskReadFloppyA As ToolStripMenuItem
    Friend WithEvents MenuDiskReadFloppyB As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents MenuDiskWriteFloppyA As ToolStripMenuItem
    Friend WithEvents MenuDiskWriteFloppyB As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As ToolStripSeparator
    Friend WithEvents MenuEditCreateBackup As ToolStripMenuItem
    Friend WithEvents ContextMenuEdit As ContextMenuStrip
    Friend WithEvents btnRetry As Button
    Friend WithEvents MenuEditAddFiles As ToolStripMenuItem
    Friend WithEvents MenuFileAddFile As ToolStripMenuItem
    Friend WithEvents MenuToolsFixImageSizeSubMenu As ToolStripMenuItem
    Friend WithEvents MenuToolsTruncateImage As ToolStripMenuItem
    Friend WithEvents MenuToolsRestructureImage As ToolStripMenuItem
    Friend WithEvents MenuHexOverdumpData As ToolStripMenuItem
    Friend WithEvents MenuHelpChangeLog As ToolStripMenuItem
    Friend WithEvents ToolStripStatusCached As ToolStripStatusLabel
    Friend WithEvents MenuFileReload As ToolStripMenuItem
    Friend WithEvents MenuHexRawTrackData As ToolStripMenuItem
    Friend WithEvents MenuFileNewImage As ToolStripMenuItem
    Friend WithEvents MenuEditWindowsExtensions As ToolStripMenuItem
    Friend WithEvents MenuFileInsertFile As ToolStripMenuItem
    Friend WithEvents ToolStripSeparatorFile As ToolStripSeparator
End Class
