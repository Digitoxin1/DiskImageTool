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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
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
        Dim MenuFileSeparator1 As System.Windows.Forms.ToolStripSeparator
        Dim MenuFileSeparator2 As System.Windows.Forms.ToolStripSeparator
        Dim MenuFileSeparator3 As System.Windows.Forms.ToolStripSeparator
        Dim MainMenuEdit As System.Windows.Forms.ToolStripMenuItem
        Dim MenuEditSeparator1 As System.Windows.Forms.ToolStripSeparator
        Dim MenuEditSeparator2 As System.Windows.Forms.ToolStripSeparator
        Dim MainMenuView As System.Windows.Forms.ToolStripMenuItem
        Dim FileMenuSeparator1 As System.Windows.Forms.ToolStripSeparator
        Dim FileCRC32 As System.Windows.Forms.ColumnHeader
        Dim MainMenuTools As System.Windows.Forms.ToolStripMenuItem
        Dim MenuToolsSeparator As System.Windows.Forms.ToolStripSeparator
        Dim MainMenuHelp As System.Windows.Forms.ToolStripMenuItem
        Dim MenuHelpSeparator As System.Windows.Forms.ToolStripSeparator
        Dim FileMenuSeparator2 As System.Windows.Forms.ToolStripSeparator
        Dim FileMenuSeparator3 As System.Windows.Forms.ToolStripSeparator
        Dim FileMenuSeparator4 As System.Windows.Forms.ToolStripSeparator
        Dim FileMenuSeparator5 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator8 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator10 As System.Windows.Forms.ToolStripSeparator
        Dim MenuDiskSeparator As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator9 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator11 As System.Windows.Forms.ToolStripSeparator
        Dim MainMenuDisk As System.Windows.Forms.ToolStripMenuItem
        Dim MenuStripTop As System.Windows.Forms.MenuStrip
        Me.MenuFileOpen = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileReload = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileNewImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileSave = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileSaveAs = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileSaveAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileClose = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileCloseAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileExit = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditFAT = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditFileProperties = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditExportFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditReplaceFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditImportFiles = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuEditUndo = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditRedo = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuEditRevert = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexFAT = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexDirectory = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexFreeClusters = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexBadSectors = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexLostClusters = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexOverdumpData = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexRawTrackData = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexDisk = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHexSeparatorFile = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuHexFile = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.MenuToolsTrackLayout = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHelpProjectPage = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHelpUpdateCheck = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHelpChangeLog = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHelpAbout = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskReadFloppyA = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskReadFloppyB = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskWriteFloppyA = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskWriteFloppyB = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenuFilters = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuFilters = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.MenuFiltersScanNew = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFiltersScan = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFiltersClear = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenuReports = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenuOptions = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuOptionsCreateBackup = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuOptionsCheckUpdate = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuOptionsDragDrop = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuOptionsDisplayTitles = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuOptionsDisplayLanguage = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenuUpdateAvailable = New System.Windows.Forms.ToolStripMenuItem()
        Me.LabelCRC32Caption = New System.Windows.Forms.Label()
        Me.LabelMD5Caption = New System.Windows.Forms.Label()
        Me.LabelSHA1Caption = New System.Windows.Forms.Label()
        Me.ToolStripTop = New System.Windows.Forms.ToolStrip()
        Me.ToolStripOpen = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSave = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSaveAs = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSaveAll = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripClose = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripCloseAll = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripFileProperties = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripExportFile = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripImportFiles = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripUndo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripRedo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripViewFileText = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripViewFile = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparatorFAT = New System.Windows.Forms.ToolStripSeparator()
        Me.StatusStripBottom = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusReadOnly = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusModified = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripFileName = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripFileCount = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripFileSector = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripFileTrack = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripImageCount = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripModified = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ListViewSummary = New System.Windows.Forms.ListView()
        Me.ComboImages = New System.Windows.Forms.ComboBox()
        Me.LabelDropMessage = New System.Windows.Forms.Label()
        Me.ListViewFiles = New System.Windows.Forms.ListView()
        Me.FileClusterError = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileCreationDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileLastAccessDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileNTReserved = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileFAT32Cluster = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileLFN = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ContextMenuFiles = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.MenuFileFileProperties = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileExportFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileReplaceFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileViewDirectory = New System.Windows.Forms.ToolStripMenuItem()
        Me.FileMenuSeparatorDirectory = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuFileViewFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileViewFileText = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileViewCrosslinked = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileImportFiles = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileImportFilesHere = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileNewDirectory = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileNewDirectoryHere = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileDeleteFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileUnDeleteFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileRemove = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileFixSize = New System.Windows.Forms.ToolStripMenuItem()
        Me.ComboImagesFiltered = New System.Windows.Forms.ComboBox()
        Me.BtnResetSort = New System.Windows.Forms.Button()
        Me.btnRetry = New System.Windows.Forms.Button()
        Me.ContextMenuDirectory = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.MenuDirectoryView = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDirectoryImportFiles = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDirectoryNewDirectory = New System.Windows.Forms.ToolStripMenuItem()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.FlowLayoutPanelHashes = New System.Windows.Forms.FlowLayoutPanel()
        Me.LabelCRC32 = New System.Windows.Forms.Label()
        Me.LabelMD5 = New System.Windows.Forms.Label()
        Me.LabelSHA1 = New System.Windows.Forms.Label()
        Me.MenuReportsWriteSplices = New System.Windows.Forms.ToolStripMenuItem()
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
        MenuFileSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        MenuFileSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        MenuFileSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        MainMenuEdit = New System.Windows.Forms.ToolStripMenuItem()
        MenuEditSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        MenuEditSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        MainMenuView = New System.Windows.Forms.ToolStripMenuItem()
        FileMenuSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        FileCRC32 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        MainMenuTools = New System.Windows.Forms.ToolStripMenuItem()
        MenuToolsSeparator = New System.Windows.Forms.ToolStripSeparator()
        MainMenuHelp = New System.Windows.Forms.ToolStripMenuItem()
        MenuHelpSeparator = New System.Windows.Forms.ToolStripSeparator()
        FileMenuSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        FileMenuSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        FileMenuSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        FileMenuSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator10 = New System.Windows.Forms.ToolStripSeparator()
        MenuDiskSeparator = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator11 = New System.Windows.Forms.ToolStripSeparator()
        MainMenuDisk = New System.Windows.Forms.ToolStripMenuItem()
        MenuStripTop = New System.Windows.Forms.MenuStrip()
        MenuStripTop.SuspendLayout()
        Me.ContextMenuFilters.SuspendLayout()
        Me.ToolStripTop.SuspendLayout()
        Me.StatusStripBottom.SuspendLayout()
        Me.ContextMenuFiles.SuspendLayout()
        Me.ContextMenuDirectory.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.FlowLayoutPanelHashes.SuspendLayout()
        Me.SuspendLayout()
        '
        'SummaryName
        '
        resources.ApplyResources(SummaryName, "SummaryName")
        '
        'SummaryValue
        '
        resources.ApplyResources(SummaryValue, "SummaryValue")
        '
        'HashName
        '
        resources.ApplyResources(HashName, "HashName")
        '
        'HashValue
        '
        resources.ApplyResources(HashValue, "HashValue")
        '
        'FileName
        '
        resources.ApplyResources(FileName, "FileName")
        '
        'FileExt
        '
        resources.ApplyResources(FileExt, "FileExt")
        '
        'FileSize
        '
        resources.ApplyResources(FileSize, "FileSize")
        '
        'FileLastWriteDate
        '
        resources.ApplyResources(FileLastWriteDate, "FileLastWriteDate")
        '
        'FileStartingCluster
        '
        resources.ApplyResources(FileStartingCluster, "FileStartingCluster")
        '
        'FileAttrib
        '
        resources.ApplyResources(FileAttrib, "FileAttrib")
        '
        'FileModified
        '
        resources.ApplyResources(FileModified, "FileModified")
        '
        'MainMenuFile
        '
        MainMenuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuFileOpen, Me.MenuFileReload, Me.MenuFileNewImage, MenuFileSeparator1, Me.MenuFileSave, Me.MenuFileSaveAs, Me.MenuFileSaveAll, MenuFileSeparator2, Me.MenuFileClose, Me.MenuFileCloseAll, MenuFileSeparator3, Me.MenuFileExit})
        MainMenuFile.Name = "MainMenuFile"
        resources.ApplyResources(MainMenuFile, "MainMenuFile")
        '
        'MenuFileOpen
        '
        Me.MenuFileOpen.Image = Global.DiskImageTool.My.Resources.Resources.OpenfileDialog
        resources.ApplyResources(Me.MenuFileOpen, "MenuFileOpen")
        Me.MenuFileOpen.Name = "MenuFileOpen"
        '
        'MenuFileReload
        '
        Me.MenuFileReload.Name = "MenuFileReload"
        resources.ApplyResources(Me.MenuFileReload, "MenuFileReload")
        '
        'MenuFileNewImage
        '
        Me.MenuFileNewImage.Name = "MenuFileNewImage"
        resources.ApplyResources(Me.MenuFileNewImage, "MenuFileNewImage")
        '
        'MenuFileSeparator1
        '
        MenuFileSeparator1.Name = "MenuFileSeparator1"
        resources.ApplyResources(MenuFileSeparator1, "MenuFileSeparator1")
        '
        'MenuFileSave
        '
        Me.MenuFileSave.Image = Global.DiskImageTool.My.Resources.Resources.Save
        resources.ApplyResources(Me.MenuFileSave, "MenuFileSave")
        Me.MenuFileSave.Name = "MenuFileSave"
        '
        'MenuFileSaveAs
        '
        Me.MenuFileSaveAs.Image = Global.DiskImageTool.My.Resources.Resources.SaveAs
        Me.MenuFileSaveAs.Name = "MenuFileSaveAs"
        resources.ApplyResources(Me.MenuFileSaveAs, "MenuFileSaveAs")
        '
        'MenuFileSaveAll
        '
        Me.MenuFileSaveAll.Image = Global.DiskImageTool.My.Resources.Resources.SaveAll
        Me.MenuFileSaveAll.Name = "MenuFileSaveAll"
        resources.ApplyResources(Me.MenuFileSaveAll, "MenuFileSaveAll")
        '
        'MenuFileSeparator2
        '
        MenuFileSeparator2.Name = "MenuFileSeparator2"
        resources.ApplyResources(MenuFileSeparator2, "MenuFileSeparator2")
        '
        'MenuFileClose
        '
        Me.MenuFileClose.Image = Global.DiskImageTool.My.Resources.Resources.Close
        Me.MenuFileClose.Name = "MenuFileClose"
        resources.ApplyResources(Me.MenuFileClose, "MenuFileClose")
        '
        'MenuFileCloseAll
        '
        Me.MenuFileCloseAll.Image = Global.DiskImageTool.My.Resources.Resources.CloseAll
        Me.MenuFileCloseAll.Name = "MenuFileCloseAll"
        resources.ApplyResources(Me.MenuFileCloseAll, "MenuFileCloseAll")
        '
        'MenuFileSeparator3
        '
        MenuFileSeparator3.Name = "MenuFileSeparator3"
        resources.ApplyResources(MenuFileSeparator3, "MenuFileSeparator3")
        '
        'MenuFileExit
        '
        Me.MenuFileExit.Image = Global.DiskImageTool.My.Resources.Resources._Exit
        Me.MenuFileExit.Name = "MenuFileExit"
        resources.ApplyResources(Me.MenuFileExit, "MenuFileExit")
        '
        'MainMenuEdit
        '
        MainMenuEdit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuEditBootSector, Me.MenuEditFAT, MenuEditSeparator1, Me.MenuEditFileProperties, Me.MenuEditExportFile, Me.MenuEditReplaceFile, MenuEditSeparator2, Me.MenuEditImportFiles, Me.ToolStripSeparator1, Me.MenuEditUndo, Me.MenuEditRedo, Me.MenuEditRevert})
        MainMenuEdit.Name = "MainMenuEdit"
        resources.ApplyResources(MainMenuEdit, "MainMenuEdit")
        '
        'MenuEditBootSector
        '
        Me.MenuEditBootSector.Name = "MenuEditBootSector"
        resources.ApplyResources(Me.MenuEditBootSector, "MenuEditBootSector")
        '
        'MenuEditFAT
        '
        Me.MenuEditFAT.Name = "MenuEditFAT"
        resources.ApplyResources(Me.MenuEditFAT, "MenuEditFAT")
        '
        'MenuEditSeparator1
        '
        MenuEditSeparator1.Name = "MenuEditSeparator1"
        resources.ApplyResources(MenuEditSeparator1, "MenuEditSeparator1")
        '
        'MenuEditFileProperties
        '
        Me.MenuEditFileProperties.Image = Global.DiskImageTool.My.Resources.Resources.PropertiesFolderClosed
        Me.MenuEditFileProperties.Name = "MenuEditFileProperties"
        resources.ApplyResources(Me.MenuEditFileProperties, "MenuEditFileProperties")
        '
        'MenuEditExportFile
        '
        Me.MenuEditExportFile.Image = Global.DiskImageTool.My.Resources.Resources.Export
        Me.MenuEditExportFile.Name = "MenuEditExportFile"
        resources.ApplyResources(Me.MenuEditExportFile, "MenuEditExportFile")
        '
        'MenuEditReplaceFile
        '
        Me.MenuEditReplaceFile.Name = "MenuEditReplaceFile"
        resources.ApplyResources(Me.MenuEditReplaceFile, "MenuEditReplaceFile")
        '
        'MenuEditSeparator2
        '
        MenuEditSeparator2.Name = "MenuEditSeparator2"
        resources.ApplyResources(MenuEditSeparator2, "MenuEditSeparator2")
        '
        'MenuEditImportFiles
        '
        Me.MenuEditImportFiles.Image = Global.DiskImageTool.My.Resources.Resources.Import
        Me.MenuEditImportFiles.Name = "MenuEditImportFiles"
        resources.ApplyResources(Me.MenuEditImportFiles, "MenuEditImportFiles")
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'MenuEditUndo
        '
        Me.MenuEditUndo.Image = Global.DiskImageTool.My.Resources.Resources.Undo
        Me.MenuEditUndo.Name = "MenuEditUndo"
        resources.ApplyResources(Me.MenuEditUndo, "MenuEditUndo")
        '
        'MenuEditRedo
        '
        Me.MenuEditRedo.Image = Global.DiskImageTool.My.Resources.Resources.Redo
        Me.MenuEditRedo.Name = "MenuEditRedo"
        resources.ApplyResources(Me.MenuEditRedo, "MenuEditRedo")
        '
        'MenuEditRevert
        '
        Me.MenuEditRevert.Name = "MenuEditRevert"
        resources.ApplyResources(Me.MenuEditRevert, "MenuEditRevert")
        '
        'MainMenuView
        '
        MainMenuView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuHexBootSector, Me.MenuHexFAT, Me.MenuHexDirectory, Me.MenuHexFreeClusters, Me.MenuHexBadSectors, Me.MenuHexLostClusters, Me.MenuHexOverdumpData, Me.MenuHexRawTrackData, Me.MenuHexDisk, Me.MenuHexSeparatorFile, Me.MenuHexFile})
        MainMenuView.Name = "MainMenuView"
        resources.ApplyResources(MainMenuView, "MainMenuView")
        '
        'MenuHexBootSector
        '
        Me.MenuHexBootSector.Name = "MenuHexBootSector"
        resources.ApplyResources(Me.MenuHexBootSector, "MenuHexBootSector")
        '
        'MenuHexFAT
        '
        Me.MenuHexFAT.Name = "MenuHexFAT"
        resources.ApplyResources(Me.MenuHexFAT, "MenuHexFAT")
        '
        'MenuHexDirectory
        '
        Me.MenuHexDirectory.Name = "MenuHexDirectory"
        resources.ApplyResources(Me.MenuHexDirectory, "MenuHexDirectory")
        '
        'MenuHexFreeClusters
        '
        Me.MenuHexFreeClusters.Name = "MenuHexFreeClusters"
        resources.ApplyResources(Me.MenuHexFreeClusters, "MenuHexFreeClusters")
        '
        'MenuHexBadSectors
        '
        Me.MenuHexBadSectors.Name = "MenuHexBadSectors"
        resources.ApplyResources(Me.MenuHexBadSectors, "MenuHexBadSectors")
        '
        'MenuHexLostClusters
        '
        Me.MenuHexLostClusters.Name = "MenuHexLostClusters"
        resources.ApplyResources(Me.MenuHexLostClusters, "MenuHexLostClusters")
        '
        'MenuHexOverdumpData
        '
        Me.MenuHexOverdumpData.Name = "MenuHexOverdumpData"
        resources.ApplyResources(Me.MenuHexOverdumpData, "MenuHexOverdumpData")
        '
        'MenuHexRawTrackData
        '
        Me.MenuHexRawTrackData.Name = "MenuHexRawTrackData"
        resources.ApplyResources(Me.MenuHexRawTrackData, "MenuHexRawTrackData")
        '
        'MenuHexDisk
        '
        Me.MenuHexDisk.Name = "MenuHexDisk"
        resources.ApplyResources(Me.MenuHexDisk, "MenuHexDisk")
        '
        'MenuHexSeparatorFile
        '
        Me.MenuHexSeparatorFile.Name = "MenuHexSeparatorFile"
        resources.ApplyResources(Me.MenuHexSeparatorFile, "MenuHexSeparatorFile")
        '
        'MenuHexFile
        '
        Me.MenuHexFile.Name = "MenuHexFile"
        resources.ApplyResources(Me.MenuHexFile, "MenuHexFile")
        '
        'FileMenuSeparator1
        '
        FileMenuSeparator1.Name = "FileMenuSeparator1"
        resources.ApplyResources(FileMenuSeparator1, "FileMenuSeparator1")
        '
        'FileCRC32
        '
        resources.ApplyResources(FileCRC32, "FileCRC32")
        '
        'MainMenuTools
        '
        MainMenuTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuToolsCompare, Me.MenuToolsWin9xClean, Me.MenuToolsClearReservedBytes, Me.MenuToolsFixImageSize, Me.MenuToolsFixImageSizeSubMenu, Me.MenuToolsRestoreBootSector, Me.MenuToolsRemoveBootSector, MenuToolsSeparator, Me.MenuToolsWin9xCleanBatch, Me.MenuToolsTrackLayout})
        MainMenuTools.Name = "MainMenuTools"
        resources.ApplyResources(MainMenuTools, "MainMenuTools")
        '
        'MenuToolsCompare
        '
        Me.MenuToolsCompare.Name = "MenuToolsCompare"
        resources.ApplyResources(Me.MenuToolsCompare, "MenuToolsCompare")
        '
        'MenuToolsWin9xClean
        '
        Me.MenuToolsWin9xClean.Name = "MenuToolsWin9xClean"
        resources.ApplyResources(Me.MenuToolsWin9xClean, "MenuToolsWin9xClean")
        '
        'MenuToolsClearReservedBytes
        '
        Me.MenuToolsClearReservedBytes.Name = "MenuToolsClearReservedBytes"
        resources.ApplyResources(Me.MenuToolsClearReservedBytes, "MenuToolsClearReservedBytes")
        '
        'MenuToolsFixImageSize
        '
        Me.MenuToolsFixImageSize.Name = "MenuToolsFixImageSize"
        resources.ApplyResources(Me.MenuToolsFixImageSize, "MenuToolsFixImageSize")
        '
        'MenuToolsFixImageSizeSubMenu
        '
        Me.MenuToolsFixImageSizeSubMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuToolsTruncateImage, Me.MenuToolsRestructureImage})
        Me.MenuToolsFixImageSizeSubMenu.Name = "MenuToolsFixImageSizeSubMenu"
        resources.ApplyResources(Me.MenuToolsFixImageSizeSubMenu, "MenuToolsFixImageSizeSubMenu")
        '
        'MenuToolsTruncateImage
        '
        Me.MenuToolsTruncateImage.Name = "MenuToolsTruncateImage"
        resources.ApplyResources(Me.MenuToolsTruncateImage, "MenuToolsTruncateImage")
        '
        'MenuToolsRestructureImage
        '
        Me.MenuToolsRestructureImage.Name = "MenuToolsRestructureImage"
        resources.ApplyResources(Me.MenuToolsRestructureImage, "MenuToolsRestructureImage")
        '
        'MenuToolsRestoreBootSector
        '
        Me.MenuToolsRestoreBootSector.Name = "MenuToolsRestoreBootSector"
        resources.ApplyResources(Me.MenuToolsRestoreBootSector, "MenuToolsRestoreBootSector")
        '
        'MenuToolsRemoveBootSector
        '
        Me.MenuToolsRemoveBootSector.Name = "MenuToolsRemoveBootSector"
        resources.ApplyResources(Me.MenuToolsRemoveBootSector, "MenuToolsRemoveBootSector")
        '
        'MenuToolsSeparator
        '
        MenuToolsSeparator.Name = "MenuToolsSeparator"
        resources.ApplyResources(MenuToolsSeparator, "MenuToolsSeparator")
        '
        'MenuToolsWin9xCleanBatch
        '
        Me.MenuToolsWin9xCleanBatch.Name = "MenuToolsWin9xCleanBatch"
        resources.ApplyResources(Me.MenuToolsWin9xCleanBatch, "MenuToolsWin9xCleanBatch")
        '
        'MenuToolsTrackLayout
        '
        Me.MenuToolsTrackLayout.Name = "MenuToolsTrackLayout"
        resources.ApplyResources(Me.MenuToolsTrackLayout, "MenuToolsTrackLayout")
        '
        'MainMenuHelp
        '
        MainMenuHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuHelpProjectPage, Me.MenuHelpUpdateCheck, Me.MenuHelpChangeLog, MenuHelpSeparator, Me.MenuHelpAbout})
        MainMenuHelp.Name = "MainMenuHelp"
        resources.ApplyResources(MainMenuHelp, "MainMenuHelp")
        '
        'MenuHelpProjectPage
        '
        Me.MenuHelpProjectPage.Name = "MenuHelpProjectPage"
        resources.ApplyResources(Me.MenuHelpProjectPage, "MenuHelpProjectPage")
        '
        'MenuHelpUpdateCheck
        '
        Me.MenuHelpUpdateCheck.Name = "MenuHelpUpdateCheck"
        resources.ApplyResources(Me.MenuHelpUpdateCheck, "MenuHelpUpdateCheck")
        '
        'MenuHelpChangeLog
        '
        Me.MenuHelpChangeLog.Name = "MenuHelpChangeLog"
        resources.ApplyResources(Me.MenuHelpChangeLog, "MenuHelpChangeLog")
        '
        'MenuHelpSeparator
        '
        MenuHelpSeparator.Name = "MenuHelpSeparator"
        resources.ApplyResources(MenuHelpSeparator, "MenuHelpSeparator")
        '
        'MenuHelpAbout
        '
        Me.MenuHelpAbout.Name = "MenuHelpAbout"
        resources.ApplyResources(Me.MenuHelpAbout, "MenuHelpAbout")
        '
        'FileMenuSeparator2
        '
        FileMenuSeparator2.Name = "FileMenuSeparator2"
        resources.ApplyResources(FileMenuSeparator2, "FileMenuSeparator2")
        '
        'FileMenuSeparator3
        '
        FileMenuSeparator3.Name = "FileMenuSeparator3"
        resources.ApplyResources(FileMenuSeparator3, "FileMenuSeparator3")
        '
        'FileMenuSeparator4
        '
        FileMenuSeparator4.Name = "FileMenuSeparator4"
        resources.ApplyResources(FileMenuSeparator4, "FileMenuSeparator4")
        '
        'FileMenuSeparator5
        '
        FileMenuSeparator5.Name = "FileMenuSeparator5"
        resources.ApplyResources(FileMenuSeparator5, "FileMenuSeparator5")
        '
        'ToolStripSeparator6
        '
        ToolStripSeparator6.Name = "ToolStripSeparator6"
        ToolStripSeparator6.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        resources.ApplyResources(ToolStripSeparator6, "ToolStripSeparator6")
        '
        'ToolStripSeparator7
        '
        ToolStripSeparator7.Name = "ToolStripSeparator7"
        ToolStripSeparator7.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        resources.ApplyResources(ToolStripSeparator7, "ToolStripSeparator7")
        '
        'ToolStripSeparator8
        '
        ToolStripSeparator8.Name = "ToolStripSeparator8"
        ToolStripSeparator8.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        resources.ApplyResources(ToolStripSeparator8, "ToolStripSeparator8")
        '
        'ToolStripSeparator10
        '
        ToolStripSeparator10.Name = "ToolStripSeparator10"
        resources.ApplyResources(ToolStripSeparator10, "ToolStripSeparator10")
        '
        'MenuDiskSeparator
        '
        MenuDiskSeparator.Name = "MenuDiskSeparator"
        resources.ApplyResources(MenuDiskSeparator, "MenuDiskSeparator")
        '
        'ToolStripSeparator9
        '
        ToolStripSeparator9.Name = "ToolStripSeparator9"
        ToolStripSeparator9.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        resources.ApplyResources(ToolStripSeparator9, "ToolStripSeparator9")
        '
        'ToolStripSeparator11
        '
        ToolStripSeparator11.Name = "ToolStripSeparator11"
        resources.ApplyResources(ToolStripSeparator11, "ToolStripSeparator11")
        '
        'MainMenuDisk
        '
        MainMenuDisk.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuDiskReadFloppyA, Me.MenuDiskReadFloppyB, MenuDiskSeparator, Me.MenuDiskWriteFloppyA, Me.MenuDiskWriteFloppyB})
        MainMenuDisk.Name = "MainMenuDisk"
        resources.ApplyResources(MainMenuDisk, "MainMenuDisk")
        '
        'MenuDiskReadFloppyA
        '
        Me.MenuDiskReadFloppyA.Name = "MenuDiskReadFloppyA"
        resources.ApplyResources(Me.MenuDiskReadFloppyA, "MenuDiskReadFloppyA")
        '
        'MenuDiskReadFloppyB
        '
        Me.MenuDiskReadFloppyB.Name = "MenuDiskReadFloppyB"
        resources.ApplyResources(Me.MenuDiskReadFloppyB, "MenuDiskReadFloppyB")
        '
        'MenuDiskWriteFloppyA
        '
        Me.MenuDiskWriteFloppyA.Name = "MenuDiskWriteFloppyA"
        resources.ApplyResources(Me.MenuDiskWriteFloppyA, "MenuDiskWriteFloppyA")
        '
        'MenuDiskWriteFloppyB
        '
        Me.MenuDiskWriteFloppyB.Name = "MenuDiskWriteFloppyB"
        resources.ApplyResources(Me.MenuDiskWriteFloppyB, "MenuDiskWriteFloppyB")
        '
        'MenuStripTop
        '
        MenuStripTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {MainMenuFile, MainMenuEdit, Me.MainMenuFilters, MainMenuView, MainMenuTools, MainMenuDisk, Me.MainMenuReports, Me.MainMenuOptions, MainMenuHelp, Me.MainMenuUpdateAvailable})
        resources.ApplyResources(MenuStripTop, "MenuStripTop")
        MenuStripTop.Name = "MenuStripTop"
        '
        'MainMenuFilters
        '
        Me.MainMenuFilters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.MainMenuFilters.DropDown = Me.ContextMenuFilters
        Me.MainMenuFilters.Name = "MainMenuFilters"
        resources.ApplyResources(Me.MainMenuFilters, "MainMenuFilters")
        '
        'ContextMenuFilters
        '
        Me.ContextMenuFilters.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuFiltersScanNew, Me.MenuFiltersScan, Me.MenuFiltersClear})
        Me.ContextMenuFilters.Name = "ContextMenuStrip1"
        Me.ContextMenuFilters.OwnerItem = Me.MainMenuFilters
        resources.ApplyResources(Me.ContextMenuFilters, "ContextMenuFilters")
        '
        'MenuFiltersScanNew
        '
        Me.MenuFiltersScanNew.Name = "MenuFiltersScanNew"
        resources.ApplyResources(Me.MenuFiltersScanNew, "MenuFiltersScanNew")
        '
        'MenuFiltersScan
        '
        Me.MenuFiltersScan.Name = "MenuFiltersScan"
        resources.ApplyResources(Me.MenuFiltersScan, "MenuFiltersScan")
        '
        'MenuFiltersClear
        '
        Me.MenuFiltersClear.Name = "MenuFiltersClear"
        resources.ApplyResources(Me.MenuFiltersClear, "MenuFiltersClear")
        '
        'MainMenuReports
        '
        Me.MainMenuReports.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuReportsWriteSplices})
        Me.MainMenuReports.Name = "MainMenuReports"
        resources.ApplyResources(Me.MainMenuReports, "MainMenuReports")
        '
        'MainMenuOptions
        '
        Me.MainMenuOptions.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuOptionsCreateBackup, Me.MenuOptionsCheckUpdate, Me.MenuOptionsDragDrop, Me.MenuOptionsDisplayTitles, Me.MenuOptionsDisplayLanguage})
        Me.MainMenuOptions.Name = "MainMenuOptions"
        resources.ApplyResources(Me.MainMenuOptions, "MainMenuOptions")
        '
        'MenuOptionsCreateBackup
        '
        Me.MenuOptionsCreateBackup.CheckOnClick = True
        Me.MenuOptionsCreateBackup.Name = "MenuOptionsCreateBackup"
        resources.ApplyResources(Me.MenuOptionsCreateBackup, "MenuOptionsCreateBackup")
        '
        'MenuOptionsCheckUpdate
        '
        Me.MenuOptionsCheckUpdate.CheckOnClick = True
        Me.MenuOptionsCheckUpdate.Name = "MenuOptionsCheckUpdate"
        resources.ApplyResources(Me.MenuOptionsCheckUpdate, "MenuOptionsCheckUpdate")
        '
        'MenuOptionsDragDrop
        '
        Me.MenuOptionsDragDrop.CheckOnClick = True
        Me.MenuOptionsDragDrop.Name = "MenuOptionsDragDrop"
        resources.ApplyResources(Me.MenuOptionsDragDrop, "MenuOptionsDragDrop")
        '
        'MenuOptionsDisplayTitles
        '
        Me.MenuOptionsDisplayTitles.CheckOnClick = True
        Me.MenuOptionsDisplayTitles.Name = "MenuOptionsDisplayTitles"
        resources.ApplyResources(Me.MenuOptionsDisplayTitles, "MenuOptionsDisplayTitles")
        '
        'MenuOptionsDisplayLanguage
        '
        Me.MenuOptionsDisplayLanguage.Name = "MenuOptionsDisplayLanguage"
        resources.ApplyResources(Me.MenuOptionsDisplayLanguage, "MenuOptionsDisplayLanguage")
        '
        'MainMenuUpdateAvailable
        '
        Me.MainMenuUpdateAvailable.ForeColor = System.Drawing.Color.Blue
        Me.MainMenuUpdateAvailable.Margin = New System.Windows.Forms.Padding(12, 0, 0, 0)
        Me.MainMenuUpdateAvailable.Name = "MainMenuUpdateAvailable"
        resources.ApplyResources(Me.MainMenuUpdateAvailable, "MainMenuUpdateAvailable")
        '
        'LabelCRC32Caption
        '
        resources.ApplyResources(Me.LabelCRC32Caption, "LabelCRC32Caption")
        Me.LabelCRC32Caption.Name = "LabelCRC32Caption"
        Me.LabelCRC32Caption.UseMnemonic = False
        '
        'LabelMD5Caption
        '
        resources.ApplyResources(Me.LabelMD5Caption, "LabelMD5Caption")
        Me.LabelMD5Caption.Name = "LabelMD5Caption"
        Me.LabelMD5Caption.UseMnemonic = False
        '
        'LabelSHA1Caption
        '
        resources.ApplyResources(Me.LabelSHA1Caption, "LabelSHA1Caption")
        Me.LabelSHA1Caption.Name = "LabelSHA1Caption"
        Me.LabelSHA1Caption.UseMnemonic = False
        '
        'ToolStripTop
        '
        Me.ToolStripTop.CanOverflow = False
        Me.ToolStripTop.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStripTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripOpen, ToolStripSeparator6, Me.ToolStripSave, Me.ToolStripSaveAs, Me.ToolStripSaveAll, ToolStripSeparator7, Me.ToolStripClose, Me.ToolStripCloseAll, ToolStripSeparator8, Me.ToolStripFileProperties, Me.ToolStripExportFile, Me.ToolStripImportFiles, ToolStripSeparator9, Me.ToolStripUndo, Me.ToolStripRedo, ToolStripSeparator10, Me.ToolStripViewFileText, Me.ToolStripViewFile, Me.ToolStripSeparatorFAT})
        resources.ApplyResources(Me.ToolStripTop, "ToolStripTop")
        Me.ToolStripTop.Name = "ToolStripTop"
        '
        'ToolStripOpen
        '
        Me.ToolStripOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripOpen.Image = Global.DiskImageTool.My.Resources.Resources.OpenfileDialog
        resources.ApplyResources(Me.ToolStripOpen, "ToolStripOpen")
        Me.ToolStripOpen.Name = "ToolStripOpen"
        Me.ToolStripOpen.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        '
        'ToolStripSave
        '
        Me.ToolStripSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripSave.Image = Global.DiskImageTool.My.Resources.Resources.Save
        resources.ApplyResources(Me.ToolStripSave, "ToolStripSave")
        Me.ToolStripSave.Name = "ToolStripSave"
        Me.ToolStripSave.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        '
        'ToolStripSaveAs
        '
        Me.ToolStripSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripSaveAs.Image = Global.DiskImageTool.My.Resources.Resources.SaveAs
        resources.ApplyResources(Me.ToolStripSaveAs, "ToolStripSaveAs")
        Me.ToolStripSaveAs.Name = "ToolStripSaveAs"
        Me.ToolStripSaveAs.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        '
        'ToolStripSaveAll
        '
        Me.ToolStripSaveAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripSaveAll.Image = Global.DiskImageTool.My.Resources.Resources.SaveAll
        resources.ApplyResources(Me.ToolStripSaveAll, "ToolStripSaveAll")
        Me.ToolStripSaveAll.Name = "ToolStripSaveAll"
        Me.ToolStripSaveAll.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        '
        'ToolStripClose
        '
        Me.ToolStripClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripClose.Image = Global.DiskImageTool.My.Resources.Resources.Close
        resources.ApplyResources(Me.ToolStripClose, "ToolStripClose")
        Me.ToolStripClose.Name = "ToolStripClose"
        Me.ToolStripClose.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        '
        'ToolStripCloseAll
        '
        Me.ToolStripCloseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripCloseAll.Image = Global.DiskImageTool.My.Resources.Resources.CloseAll
        resources.ApplyResources(Me.ToolStripCloseAll, "ToolStripCloseAll")
        Me.ToolStripCloseAll.Name = "ToolStripCloseAll"
        Me.ToolStripCloseAll.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        '
        'ToolStripFileProperties
        '
        Me.ToolStripFileProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripFileProperties.Image = Global.DiskImageTool.My.Resources.Resources.PropertiesFolderClosed
        resources.ApplyResources(Me.ToolStripFileProperties, "ToolStripFileProperties")
        Me.ToolStripFileProperties.Name = "ToolStripFileProperties"
        Me.ToolStripFileProperties.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        '
        'ToolStripExportFile
        '
        Me.ToolStripExportFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripExportFile.Image = Global.DiskImageTool.My.Resources.Resources.Export
        resources.ApplyResources(Me.ToolStripExportFile, "ToolStripExportFile")
        Me.ToolStripExportFile.Name = "ToolStripExportFile"
        Me.ToolStripExportFile.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        '
        'ToolStripImportFiles
        '
        Me.ToolStripImportFiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripImportFiles.Image = Global.DiskImageTool.My.Resources.Resources.Import
        resources.ApplyResources(Me.ToolStripImportFiles, "ToolStripImportFiles")
        Me.ToolStripImportFiles.Name = "ToolStripImportFiles"
        '
        'ToolStripUndo
        '
        Me.ToolStripUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripUndo.Image = Global.DiskImageTool.My.Resources.Resources.Undo
        resources.ApplyResources(Me.ToolStripUndo, "ToolStripUndo")
        Me.ToolStripUndo.Name = "ToolStripUndo"
        Me.ToolStripUndo.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        '
        'ToolStripRedo
        '
        Me.ToolStripRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripRedo.Image = Global.DiskImageTool.My.Resources.Resources.Redo
        resources.ApplyResources(Me.ToolStripRedo, "ToolStripRedo")
        Me.ToolStripRedo.Name = "ToolStripRedo"
        Me.ToolStripRedo.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        '
        'ToolStripViewFileText
        '
        Me.ToolStripViewFileText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripViewFileText.Image = Global.DiskImageTool.My.Resources.Resources.TextFile
        resources.ApplyResources(Me.ToolStripViewFileText, "ToolStripViewFileText")
        Me.ToolStripViewFileText.Name = "ToolStripViewFileText"
        Me.ToolStripViewFileText.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        '
        'ToolStripViewFile
        '
        Me.ToolStripViewFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripViewFile.Image = Global.DiskImageTool.My.Resources.Resources.TextArea
        resources.ApplyResources(Me.ToolStripViewFile, "ToolStripViewFile")
        Me.ToolStripViewFile.Name = "ToolStripViewFile"
        Me.ToolStripViewFile.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        '
        'ToolStripSeparatorFAT
        '
        Me.ToolStripSeparatorFAT.Name = "ToolStripSeparatorFAT"
        resources.ApplyResources(Me.ToolStripSeparatorFAT, "ToolStripSeparatorFAT")
        '
        'StatusStripBottom
        '
        Me.StatusStripBottom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusReadOnly, Me.ToolStripStatusModified, Me.ToolStripFileName, Me.ToolStripFileCount, Me.ToolStripFileSector, Me.ToolStripFileTrack, Me.ToolStripImageCount, Me.ToolStripModified})
        resources.ApplyResources(Me.StatusStripBottom, "StatusStripBottom")
        Me.StatusStripBottom.Name = "StatusStripBottom"
        Me.StatusStripBottom.ShowItemToolTips = True
        '
        'ToolStripStatusReadOnly
        '
        resources.ApplyResources(Me.ToolStripStatusReadOnly, "ToolStripStatusReadOnly")
        Me.ToolStripStatusReadOnly.ForeColor = System.Drawing.Color.Red
        Me.ToolStripStatusReadOnly.Name = "ToolStripStatusReadOnly"
        '
        'ToolStripStatusModified
        '
        Me.ToolStripStatusModified.ForeColor = System.Drawing.Color.Blue
        Me.ToolStripStatusModified.Name = "ToolStripStatusModified"
        resources.ApplyResources(Me.ToolStripStatusModified, "ToolStripStatusModified")
        '
        'ToolStripFileName
        '
        Me.ToolStripFileName.AutoToolTip = True
        Me.ToolStripFileName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripFileName.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripFileName.Name = "ToolStripFileName"
        resources.ApplyResources(Me.ToolStripFileName, "ToolStripFileName")
        Me.ToolStripFileName.Spring = True
        '
        'ToolStripFileCount
        '
        Me.ToolStripFileCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripFileCount.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripFileCount.Name = "ToolStripFileCount"
        resources.ApplyResources(Me.ToolStripFileCount, "ToolStripFileCount")
        '
        'ToolStripFileSector
        '
        Me.ToolStripFileSector.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripFileSector.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripFileSector.Name = "ToolStripFileSector"
        resources.ApplyResources(Me.ToolStripFileSector, "ToolStripFileSector")
        '
        'ToolStripFileTrack
        '
        Me.ToolStripFileTrack.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripFileTrack.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripFileTrack.Name = "ToolStripFileTrack"
        resources.ApplyResources(Me.ToolStripFileTrack, "ToolStripFileTrack")
        '
        'ToolStripImageCount
        '
        Me.ToolStripImageCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripImageCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripImageCount.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripImageCount.Name = "ToolStripImageCount"
        resources.ApplyResources(Me.ToolStripImageCount, "ToolStripImageCount")
        '
        'ToolStripModified
        '
        Me.ToolStripModified.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripModified.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripModified.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripModified.Name = "ToolStripModified"
        resources.ApplyResources(Me.ToolStripModified, "ToolStripModified")
        '
        'ListViewSummary
        '
        Me.ListViewSummary.AllowDrop = True
        resources.ApplyResources(Me.ListViewSummary, "ListViewSummary")
        Me.ListViewSummary.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {SummaryName, SummaryValue})
        Me.ListViewSummary.FullRowSelect = True
        Me.ListViewSummary.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.ListViewSummary.HideSelection = False
        Me.ListViewSummary.MultiSelect = False
        Me.ListViewSummary.Name = "ListViewSummary"
        Me.ListViewSummary.OwnerDraw = True
        Me.ListViewSummary.UseCompatibleStateImageBehavior = False
        Me.ListViewSummary.View = System.Windows.Forms.View.Details
        '
        'ComboImages
        '
        Me.ComboImages.AllowDrop = True
        resources.ApplyResources(Me.ComboImages, "ComboImages")
        Me.ComboImages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboImages.DropDownWidth = 523
        Me.ComboImages.Name = "ComboImages"
        Me.ComboImages.Sorted = True
        '
        'LabelDropMessage
        '
        Me.LabelDropMessage.AllowDrop = True
        resources.ApplyResources(Me.LabelDropMessage, "LabelDropMessage")
        Me.LabelDropMessage.BackColor = System.Drawing.SystemColors.Window
        Me.LabelDropMessage.Name = "LabelDropMessage"
        '
        'ListViewFiles
        '
        Me.ListViewFiles.AllowDrop = True
        resources.ApplyResources(Me.ListViewFiles, "ListViewFiles")
        Me.ListViewFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {FileModified, FileName, FileExt, FileSize, FileLastWriteDate, FileStartingCluster, Me.FileClusterError, FileAttrib, FileCRC32, Me.FileCreationDate, Me.FileLastAccessDate, Me.FileNTReserved, Me.FileFAT32Cluster, Me.FileLFN})
        Me.ListViewFiles.ContextMenuStrip = Me.ContextMenuFiles
        Me.ListViewFiles.FullRowSelect = True
        Me.ListViewFiles.HideSelection = False
        Me.ListViewFiles.Name = "ListViewFiles"
        Me.ListViewFiles.OwnerDraw = True
        Me.ListViewFiles.UseCompatibleStateImageBehavior = False
        Me.ListViewFiles.View = System.Windows.Forms.View.Details
        '
        'FileClusterError
        '
        resources.ApplyResources(Me.FileClusterError, "FileClusterError")
        '
        'FileCreationDate
        '
        resources.ApplyResources(Me.FileCreationDate, "FileCreationDate")
        '
        'FileLastAccessDate
        '
        resources.ApplyResources(Me.FileLastAccessDate, "FileLastAccessDate")
        '
        'FileNTReserved
        '
        resources.ApplyResources(Me.FileNTReserved, "FileNTReserved")
        '
        'FileFAT32Cluster
        '
        resources.ApplyResources(Me.FileFAT32Cluster, "FileFAT32Cluster")
        '
        'FileLFN
        '
        resources.ApplyResources(Me.FileLFN, "FileLFN")
        '
        'ContextMenuFiles
        '
        Me.ContextMenuFiles.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuFileFileProperties, Me.MenuFileExportFile, Me.MenuFileReplaceFile, FileMenuSeparator1, Me.MenuFileViewDirectory, Me.FileMenuSeparatorDirectory, Me.MenuFileViewFile, Me.MenuFileViewFileText, Me.MenuFileViewCrosslinked, FileMenuSeparator2, Me.MenuFileImportFiles, Me.MenuFileImportFilesHere, FileMenuSeparator5, Me.MenuFileNewDirectory, Me.MenuFileNewDirectoryHere, FileMenuSeparator4, Me.MenuFileDeleteFile, Me.MenuFileUnDeleteFile, Me.MenuFileRemove, FileMenuSeparator3, Me.MenuFileFixSize})
        Me.ContextMenuFiles.Name = "ContextMenuFiles"
        resources.ApplyResources(Me.ContextMenuFiles, "ContextMenuFiles")
        '
        'MenuFileFileProperties
        '
        Me.MenuFileFileProperties.Image = Global.DiskImageTool.My.Resources.Resources.PropertiesFolderClosed
        Me.MenuFileFileProperties.Name = "MenuFileFileProperties"
        resources.ApplyResources(Me.MenuFileFileProperties, "MenuFileFileProperties")
        '
        'MenuFileExportFile
        '
        Me.MenuFileExportFile.Image = Global.DiskImageTool.My.Resources.Resources.Export
        Me.MenuFileExportFile.Name = "MenuFileExportFile"
        resources.ApplyResources(Me.MenuFileExportFile, "MenuFileExportFile")
        '
        'MenuFileReplaceFile
        '
        Me.MenuFileReplaceFile.Name = "MenuFileReplaceFile"
        resources.ApplyResources(Me.MenuFileReplaceFile, "MenuFileReplaceFile")
        '
        'MenuFileViewDirectory
        '
        Me.MenuFileViewDirectory.Name = "MenuFileViewDirectory"
        resources.ApplyResources(Me.MenuFileViewDirectory, "MenuFileViewDirectory")
        '
        'FileMenuSeparatorDirectory
        '
        Me.FileMenuSeparatorDirectory.Name = "FileMenuSeparatorDirectory"
        resources.ApplyResources(Me.FileMenuSeparatorDirectory, "FileMenuSeparatorDirectory")
        '
        'MenuFileViewFile
        '
        Me.MenuFileViewFile.Image = Global.DiskImageTool.My.Resources.Resources.TextArea
        Me.MenuFileViewFile.Name = "MenuFileViewFile"
        resources.ApplyResources(Me.MenuFileViewFile, "MenuFileViewFile")
        '
        'MenuFileViewFileText
        '
        Me.MenuFileViewFileText.Image = Global.DiskImageTool.My.Resources.Resources.TextFile
        Me.MenuFileViewFileText.Name = "MenuFileViewFileText"
        resources.ApplyResources(Me.MenuFileViewFileText, "MenuFileViewFileText")
        '
        'MenuFileViewCrosslinked
        '
        Me.MenuFileViewCrosslinked.Name = "MenuFileViewCrosslinked"
        resources.ApplyResources(Me.MenuFileViewCrosslinked, "MenuFileViewCrosslinked")
        '
        'MenuFileImportFiles
        '
        Me.MenuFileImportFiles.Image = Global.DiskImageTool.My.Resources.Resources.Import
        Me.MenuFileImportFiles.Name = "MenuFileImportFiles"
        resources.ApplyResources(Me.MenuFileImportFiles, "MenuFileImportFiles")
        '
        'MenuFileImportFilesHere
        '
        Me.MenuFileImportFilesHere.Name = "MenuFileImportFilesHere"
        resources.ApplyResources(Me.MenuFileImportFilesHere, "MenuFileImportFilesHere")
        '
        'MenuFileNewDirectory
        '
        Me.MenuFileNewDirectory.Name = "MenuFileNewDirectory"
        resources.ApplyResources(Me.MenuFileNewDirectory, "MenuFileNewDirectory")
        '
        'MenuFileNewDirectoryHere
        '
        Me.MenuFileNewDirectoryHere.Name = "MenuFileNewDirectoryHere"
        resources.ApplyResources(Me.MenuFileNewDirectoryHere, "MenuFileNewDirectoryHere")
        '
        'MenuFileDeleteFile
        '
        Me.MenuFileDeleteFile.Name = "MenuFileDeleteFile"
        resources.ApplyResources(Me.MenuFileDeleteFile, "MenuFileDeleteFile")
        '
        'MenuFileUnDeleteFile
        '
        Me.MenuFileUnDeleteFile.Name = "MenuFileUnDeleteFile"
        resources.ApplyResources(Me.MenuFileUnDeleteFile, "MenuFileUnDeleteFile")
        '
        'MenuFileRemove
        '
        Me.MenuFileRemove.Name = "MenuFileRemove"
        resources.ApplyResources(Me.MenuFileRemove, "MenuFileRemove")
        '
        'MenuFileFixSize
        '
        Me.MenuFileFixSize.Name = "MenuFileFixSize"
        resources.ApplyResources(Me.MenuFileFixSize, "MenuFileFixSize")
        '
        'ComboImagesFiltered
        '
        Me.ComboImagesFiltered.AllowDrop = True
        resources.ApplyResources(Me.ComboImagesFiltered, "ComboImagesFiltered")
        Me.ComboImagesFiltered.BackColor = System.Drawing.SystemColors.Window
        Me.ComboImagesFiltered.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboImagesFiltered.DropDownWidth = 523
        Me.ComboImagesFiltered.Name = "ComboImagesFiltered"
        Me.ComboImagesFiltered.Sorted = True
        '
        'BtnResetSort
        '
        resources.ApplyResources(Me.BtnResetSort, "BtnResetSort")
        Me.BtnResetSort.Name = "BtnResetSort"
        Me.BtnResetSort.UseVisualStyleBackColor = True
        '
        'btnRetry
        '
        resources.ApplyResources(Me.btnRetry, "btnRetry")
        Me.btnRetry.Name = "btnRetry"
        Me.btnRetry.UseVisualStyleBackColor = True
        '
        'ContextMenuDirectory
        '
        Me.ContextMenuDirectory.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuDirectoryView, ToolStripSeparator11, Me.MenuDirectoryImportFiles, Me.MenuDirectoryNewDirectory})
        Me.ContextMenuDirectory.Name = "ContextMenuDirectory"
        resources.ApplyResources(Me.ContextMenuDirectory, "ContextMenuDirectory")
        '
        'MenuDirectoryView
        '
        Me.MenuDirectoryView.Name = "MenuDirectoryView"
        resources.ApplyResources(Me.MenuDirectoryView, "MenuDirectoryView")
        '
        'MenuDirectoryImportFiles
        '
        Me.MenuDirectoryImportFiles.Image = Global.DiskImageTool.My.Resources.Resources.Import
        Me.MenuDirectoryImportFiles.Name = "MenuDirectoryImportFiles"
        resources.ApplyResources(Me.MenuDirectoryImportFiles, "MenuDirectoryImportFiles")
        '
        'MenuDirectoryNewDirectory
        '
        Me.MenuDirectoryNewDirectory.Name = "MenuDirectoryNewDirectory"
        resources.ApplyResources(Me.MenuDirectoryNewDirectory, "MenuDirectoryNewDirectory")
        '
        'SplitContainer1
        '
        resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnRetry)
        Me.SplitContainer1.Panel1.Controls.Add(Me.FlowLayoutPanelHashes)
        Me.SplitContainer1.Panel1.Controls.Add(Me.ListViewSummary)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.BtnResetSort)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ComboImagesFiltered)
        Me.SplitContainer1.Panel2.Controls.Add(Me.LabelDropMessage)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ComboImages)
        Me.SplitContainer1.Panel2.Controls.Add(Me.ListViewFiles)
        '
        'FlowLayoutPanelHashes
        '
        Me.FlowLayoutPanelHashes.AllowDrop = True
        resources.ApplyResources(Me.FlowLayoutPanelHashes, "FlowLayoutPanelHashes")
        Me.FlowLayoutPanelHashes.BackColor = System.Drawing.SystemColors.Window
        Me.FlowLayoutPanelHashes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.FlowLayoutPanelHashes.Controls.Add(Me.LabelCRC32Caption)
        Me.FlowLayoutPanelHashes.Controls.Add(Me.LabelCRC32)
        Me.FlowLayoutPanelHashes.Controls.Add(Me.LabelMD5Caption)
        Me.FlowLayoutPanelHashes.Controls.Add(Me.LabelMD5)
        Me.FlowLayoutPanelHashes.Controls.Add(Me.LabelSHA1Caption)
        Me.FlowLayoutPanelHashes.Controls.Add(Me.LabelSHA1)
        Me.FlowLayoutPanelHashes.Name = "FlowLayoutPanelHashes"
        '
        'LabelCRC32
        '
        resources.ApplyResources(Me.LabelCRC32, "LabelCRC32")
        Me.LabelCRC32.Name = "LabelCRC32"
        Me.LabelCRC32.UseMnemonic = False
        '
        'LabelMD5
        '
        resources.ApplyResources(Me.LabelMD5, "LabelMD5")
        Me.LabelMD5.Name = "LabelMD5"
        Me.LabelMD5.UseMnemonic = False
        '
        'LabelSHA1
        '
        resources.ApplyResources(Me.LabelSHA1, "LabelSHA1")
        Me.LabelSHA1.Name = "LabelSHA1"
        Me.LabelSHA1.UseMnemonic = False
        '
        'MenuReportsWriteSplices
        '
        Me.MenuReportsWriteSplices.Name = "MenuReportsWriteSplices"
        resources.ApplyResources(Me.MenuReportsWriteSplices, "MenuReportsWriteSplices")
        '
        'MainForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.ToolStripTop)
        Me.Controls.Add(Me.StatusStripBottom)
        Me.Controls.Add(MenuStripTop)
        Me.MainMenuStrip = MenuStripTop
        Me.Name = "MainForm"
        MenuStripTop.ResumeLayout(False)
        MenuStripTop.PerformLayout()
        Me.ContextMenuFilters.ResumeLayout(False)
        Me.ToolStripTop.ResumeLayout(False)
        Me.ToolStripTop.PerformLayout()
        Me.StatusStripBottom.ResumeLayout(False)
        Me.StatusStripBottom.PerformLayout()
        Me.ContextMenuFiles.ResumeLayout(False)
        Me.ContextMenuDirectory.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.FlowLayoutPanelHashes.ResumeLayout(False)
        Me.FlowLayoutPanelHashes.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ListViewSummary As ListView
    Friend WithEvents ComboImages As ComboBox
    Friend WithEvents ToolStripImageCount As ToolStripStatusLabel
    Friend WithEvents ToolStripFileName As ToolStripStatusLabel
    Friend WithEvents ToolStripModified As ToolStripStatusLabel
    Friend WithEvents ListViewFiles As ListView
    Friend WithEvents MenuFileOpen As ToolStripMenuItem
    Friend WithEvents MenuFileSave As ToolStripMenuItem
    Friend WithEvents MenuFileSaveAs As ToolStripMenuItem
    Friend WithEvents MenuFileExit As ToolStripMenuItem
    Friend WithEvents ContextMenuFilters As ContextMenuStrip
    Friend WithEvents MenuFileSaveAll As ToolStripMenuItem
    Friend WithEvents MenuFiltersScan As ToolStripMenuItem
    Friend WithEvents MenuHexBootSector As ToolStripMenuItem
    Friend WithEvents MenuHexDirectory As ToolStripMenuItem
    Friend WithEvents MenuHexFreeClusters As ToolStripMenuItem
    Friend WithEvents MenuEditRevert As ToolStripMenuItem
    Friend WithEvents MenuHexFile As ToolStripMenuItem
    Friend WithEvents MenuFileClose As ToolStripMenuItem
    Friend WithEvents MenuFileCloseAll As ToolStripMenuItem
    Friend WithEvents MenuFiltersScanNew As ToolStripMenuItem
    Friend WithEvents ToolStripFileCount As ToolStripStatusLabel
    Friend WithEvents MenuEditFileProperties As ToolStripMenuItem
    Friend WithEvents ContextMenuFiles As ContextMenuStrip
    Friend WithEvents MenuFileFileProperties As ToolStripMenuItem
    Friend WithEvents MenuFileViewFile As ToolStripMenuItem
    Friend WithEvents MenuFileViewFileText As ToolStripMenuItem
    Friend WithEvents MainMenuFilters As ToolStripMenuItem
    Friend WithEvents MenuFileReplaceFile As ToolStripMenuItem
    Friend WithEvents FileCreationDate As ColumnHeader
    Friend WithEvents FileLastAccessDate As ColumnHeader
    Friend WithEvents FileLFN As ColumnHeader
    Friend WithEvents MenuHexFAT As ToolStripMenuItem
    Friend WithEvents FileClusterError As ColumnHeader
    Friend WithEvents MenuFileViewCrosslinked As ToolStripMenuItem
    Friend WithEvents MenuEditExportFile As ToolStripMenuItem
    Friend WithEvents MenuFileExportFile As ToolStripMenuItem
    Friend WithEvents ComboImagesFiltered As ComboBox
    Friend WithEvents MenuFiltersClear As ToolStripMenuItem
    Friend WithEvents ToolStripStatusModified As ToolStripStatusLabel
    Friend WithEvents MenuHexBadSectors As ToolStripMenuItem
    Friend WithEvents MenuEditUndo As ToolStripMenuItem
    Friend WithEvents MenuEditRedo As ToolStripMenuItem
    Friend WithEvents ToolStripOpen As ToolStripButton
    Friend WithEvents LabelDropMessage As Label
    Friend WithEvents ToolStripSave As ToolStripButton
    Friend WithEvents ToolStripSaveAs As ToolStripButton
    Friend WithEvents ToolStripSaveAll As ToolStripButton
    Friend WithEvents ToolStripClose As ToolStripButton
    Friend WithEvents ToolStripCloseAll As ToolStripButton
    Friend WithEvents ToolStripUndo As ToolStripButton
    Friend WithEvents ToolStripRedo As ToolStripButton
    Friend WithEvents ToolStripFileProperties As ToolStripButton
    Friend WithEvents ToolStripExportFile As ToolStripButton
    Friend WithEvents ToolStripViewFile As ToolStripButton
    Friend WithEvents ToolStripViewFileText As ToolStripButton
    Friend WithEvents MenuHexDisk As ToolStripMenuItem
    Friend WithEvents MenuFileRemove As ToolStripMenuItem
    Friend WithEvents MenuFileDeleteFile As ToolStripMenuItem
    Friend WithEvents MenuToolsWin9xClean As ToolStripMenuItem
    Friend WithEvents MenuToolsFixImageSize As ToolStripMenuItem
    Friend WithEvents MenuHelpAbout As ToolStripMenuItem
    Friend WithEvents MenuHelpProjectPage As ToolStripMenuItem
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
    Friend WithEvents MenuToolsWin9xCleanBatch As ToolStripMenuItem
    Friend WithEvents MenuToolsCompare As ToolStripMenuItem
    Friend WithEvents MenuFileUnDeleteFile As ToolStripMenuItem
    Friend WithEvents FileNTReserved As ColumnHeader
    Friend WithEvents MenuToolsClearReservedBytes As ToolStripMenuItem
    Friend WithEvents MenuHexLostClusters As ToolStripMenuItem
    Friend WithEvents ToolStripSeparatorFAT As ToolStripSeparator
    Friend WithEvents MenuDiskReadFloppyA As ToolStripMenuItem
    Friend WithEvents MenuDiskReadFloppyB As ToolStripMenuItem
    Friend WithEvents MenuDiskWriteFloppyA As ToolStripMenuItem
    Friend WithEvents MenuDiskWriteFloppyB As ToolStripMenuItem
    Friend WithEvents MenuOptionsCreateBackup As ToolStripMenuItem
    Friend WithEvents btnRetry As Button
    Friend WithEvents MenuFileImportFiles As ToolStripMenuItem
    Friend WithEvents MenuToolsFixImageSizeSubMenu As ToolStripMenuItem
    Friend WithEvents MenuToolsTruncateImage As ToolStripMenuItem
    Friend WithEvents MenuToolsRestructureImage As ToolStripMenuItem
    Friend WithEvents MenuHexOverdumpData As ToolStripMenuItem
    Friend WithEvents MenuHelpChangeLog As ToolStripMenuItem
    Friend WithEvents MenuFileReload As ToolStripMenuItem
    Friend WithEvents MenuHexRawTrackData As ToolStripMenuItem
    Friend WithEvents MenuFileNewImage As ToolStripMenuItem
    Friend WithEvents MenuFileImportFilesHere As ToolStripMenuItem
    Friend WithEvents MenuHexSeparatorFile As ToolStripSeparator
    Friend WithEvents MenuEditReplaceFile As ToolStripMenuItem
    Friend WithEvents MenuFileNewDirectory As ToolStripMenuItem
    Friend WithEvents MenuFileNewDirectoryHere As ToolStripMenuItem
    Friend WithEvents ContextMenuDirectory As ContextMenuStrip
    Friend WithEvents MenuDirectoryView As ToolStripMenuItem
    Friend WithEvents MenuDirectoryImportFiles As ToolStripMenuItem
    Friend WithEvents MenuDirectoryNewDirectory As ToolStripMenuItem
    Friend WithEvents MainMenuOptions As ToolStripMenuItem
    Friend WithEvents StatusStripBottom As StatusStrip
    Friend WithEvents ToolStripTop As ToolStrip
    Friend WithEvents MenuOptionsDragDrop As ToolStripMenuItem
    Friend WithEvents FileFAT32Cluster As ColumnHeader
    Friend WithEvents MenuToolsTrackLayout As ToolStripMenuItem
    Friend WithEvents MenuOptionsCheckUpdate As ToolStripMenuItem
    Friend WithEvents MainMenuUpdateAvailable As ToolStripMenuItem
    Friend WithEvents MenuOptionsDisplayTitles As ToolStripMenuItem
    Friend WithEvents MenuOptionsDisplayLanguage As ToolStripMenuItem
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents FlowLayoutPanelHashes As FlowLayoutPanel
    Friend WithEvents LabelCRC32 As Label
    Friend WithEvents LabelMD5 As Label
    Friend WithEvents LabelSHA1 As Label
    Friend WithEvents LabelCRC32Caption As Label
    Friend WithEvents LabelMD5Caption As Label
    Friend WithEvents LabelSHA1Caption As Label
    Friend WithEvents ToolStripImportFiles As ToolStripButton
    Friend WithEvents MenuEditImportFiles As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents MainMenuReports As ToolStripMenuItem
    Friend WithEvents MenuReportsWriteSplices As ToolStripMenuItem
End Class
