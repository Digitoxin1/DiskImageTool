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
        Dim MenuFileSeparator1 As System.Windows.Forms.ToolStripSeparator
        Dim MenuFileSeparator2 As System.Windows.Forms.ToolStripSeparator
        Dim MenuFileSeparator3 As System.Windows.Forms.ToolStripSeparator
        Dim MainMenuEdit As System.Windows.Forms.ToolStripMenuItem
        Dim MenuEditSeparator1 As System.Windows.Forms.ToolStripSeparator
        Dim MenuEditSeparator2 As System.Windows.Forms.ToolStripSeparator
        Dim MainMenuView As System.Windows.Forms.ToolStripMenuItem
        Dim MainMenuExperimental As System.Windows.Forms.ToolStripMenuItem
        Dim FileMenuSeparator1 As System.Windows.Forms.ToolStripSeparator
        Dim FileCRC32 As System.Windows.Forms.ColumnHeader
        Dim ToolStripSearchLabel As System.Windows.Forms.ToolStripLabel
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
        Me.MainMenuOptions = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuOptionsCreateBackup = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuOptionsDragDrop = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuOptionsWindowsExtensions = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuOptionsSeparatorDebug = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuOptionsExportUnknown = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripTop = New System.Windows.Forms.ToolStrip()
        Me.ToolStripSearchText = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripOEMNameCombo = New System.Windows.Forms.ToolStripComboBox()
        Me.ToolStripOEMNameLabel = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripDiskTypeCombo = New System.Windows.Forms.ToolStripComboBox()
        Me.ToolStripDiskTypeLabel = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripOpen = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSave = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSaveAs = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSaveAll = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripClose = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripCloseAll = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripFileProperties = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripExportFile = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripUndo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripRedo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripViewFileText = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripViewFile = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparatorFAT = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripFATCombo = New System.Windows.Forms.ToolStripComboBox()
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
        Me.ListViewHashes = New System.Windows.Forms.ListView()
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
        MainMenuExperimental = New System.Windows.Forms.ToolStripMenuItem()
        FileMenuSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        FileCRC32 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        ToolStripSearchLabel = New System.Windows.Forms.ToolStripLabel()
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
        MainMenuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuFileOpen, Me.MenuFileReload, Me.MenuFileNewImage, MenuFileSeparator1, Me.MenuFileSave, Me.MenuFileSaveAs, Me.MenuFileSaveAll, MenuFileSeparator2, Me.MenuFileClose, Me.MenuFileCloseAll, MenuFileSeparator3, Me.MenuFileExit})
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
        'MenuFileSeparator1
        '
        MenuFileSeparator1.Name = "MenuFileSeparator1"
        MenuFileSeparator1.Size = New System.Drawing.Size(209, 6)
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
        'MenuFileSeparator2
        '
        MenuFileSeparator2.Name = "MenuFileSeparator2"
        MenuFileSeparator2.Size = New System.Drawing.Size(209, 6)
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
        'MenuFileSeparator3
        '
        MenuFileSeparator3.Name = "MenuFileSeparator3"
        MenuFileSeparator3.Size = New System.Drawing.Size(209, 6)
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
        MainMenuEdit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuEditBootSector, Me.MenuEditFAT, MenuEditSeparator1, Me.MenuEditFileProperties, Me.MenuEditExportFile, Me.MenuEditReplaceFile, MenuEditSeparator2, Me.MenuEditUndo, Me.MenuEditRedo, Me.MenuEditRevert})
        MainMenuEdit.Name = "MainMenuEdit"
        MainMenuEdit.Size = New System.Drawing.Size(39, 20)
        MainMenuEdit.Text = "&Edit"
        '
        'MenuEditBootSector
        '
        Me.MenuEditBootSector.Name = "MenuEditBootSector"
        Me.MenuEditBootSector.Size = New System.Drawing.Size(179, 22)
        Me.MenuEditBootSector.Text = "&Boot Sector"
        '
        'MenuEditFAT
        '
        Me.MenuEditFAT.Name = "MenuEditFAT"
        Me.MenuEditFAT.Size = New System.Drawing.Size(179, 22)
        Me.MenuEditFAT.Text = "File &Allocation Table"
        '
        'MenuEditSeparator1
        '
        MenuEditSeparator1.Name = "MenuEditSeparator1"
        MenuEditSeparator1.Size = New System.Drawing.Size(176, 6)
        '
        'MenuEditFileProperties
        '
        Me.MenuEditFileProperties.Image = CType(resources.GetObject("MenuEditFileProperties.Image"), System.Drawing.Image)
        Me.MenuEditFileProperties.Name = "MenuEditFileProperties"
        Me.MenuEditFileProperties.Size = New System.Drawing.Size(179, 22)
        Me.MenuEditFileProperties.Text = "File &Properties"
        '
        'MenuEditExportFile
        '
        Me.MenuEditExportFile.Image = CType(resources.GetObject("MenuEditExportFile.Image"), System.Drawing.Image)
        Me.MenuEditExportFile.Name = "MenuEditExportFile"
        Me.MenuEditExportFile.Size = New System.Drawing.Size(179, 22)
        Me.MenuEditExportFile.Text = "&Export File"
        '
        'MenuEditReplaceFile
        '
        Me.MenuEditReplaceFile.Name = "MenuEditReplaceFile"
        Me.MenuEditReplaceFile.Size = New System.Drawing.Size(179, 22)
        Me.MenuEditReplaceFile.Text = "&Replace File"
        '
        'MenuEditSeparator2
        '
        MenuEditSeparator2.Name = "MenuEditSeparator2"
        MenuEditSeparator2.Size = New System.Drawing.Size(176, 6)
        '
        'MenuEditUndo
        '
        Me.MenuEditUndo.Image = CType(resources.GetObject("MenuEditUndo.Image"), System.Drawing.Image)
        Me.MenuEditUndo.Name = "MenuEditUndo"
        Me.MenuEditUndo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.MenuEditUndo.Size = New System.Drawing.Size(179, 22)
        Me.MenuEditUndo.Text = "&Undo"
        '
        'MenuEditRedo
        '
        Me.MenuEditRedo.Image = CType(resources.GetObject("MenuEditRedo.Image"), System.Drawing.Image)
        Me.MenuEditRedo.Name = "MenuEditRedo"
        Me.MenuEditRedo.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.MenuEditRedo.Size = New System.Drawing.Size(179, 22)
        Me.MenuEditRedo.Text = "&Redo"
        '
        'MenuEditRevert
        '
        Me.MenuEditRevert.Name = "MenuEditRevert"
        Me.MenuEditRevert.Size = New System.Drawing.Size(179, 22)
        Me.MenuEditRevert.Text = "&Revert Changes"
        Me.MenuEditRevert.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MainMenuView
        '
        MainMenuView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuHexBootSector, Me.MenuHexFAT, Me.MenuHexDirectory, Me.MenuHexFreeClusters, Me.MenuHexBadSectors, Me.MenuHexLostClusters, Me.MenuHexOverdumpData, Me.MenuHexRawTrackData, Me.MenuHexDisk, Me.MenuHexSeparatorFile, Me.MenuHexFile})
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
        'MenuHexSeparatorFile
        '
        Me.MenuHexSeparatorFile.Name = "MenuHexSeparatorFile"
        Me.MenuHexSeparatorFile.Size = New System.Drawing.Size(191, 6)
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
        'FileMenuSeparator1
        '
        FileMenuSeparator1.Name = "FileMenuSeparator1"
        FileMenuSeparator1.Size = New System.Drawing.Size(186, 6)
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
        MainMenuTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuToolsCompare, Me.MenuToolsWin9xClean, Me.MenuToolsClearReservedBytes, Me.MenuToolsFixImageSize, Me.MenuToolsFixImageSizeSubMenu, Me.MenuToolsRestoreBootSector, Me.MenuToolsRemoveBootSector, MenuToolsSeparator, Me.MenuToolsWin9xCleanBatch, Me.MenuToolsTrackLayout})
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
        Me.MenuToolsTruncateImage.Size = New System.Drawing.Size(170, 22)
        Me.MenuToolsTruncateImage.Text = "&Truncate Image"
        '
        'MenuToolsRestructureImage
        '
        Me.MenuToolsRestructureImage.Name = "MenuToolsRestructureImage"
        Me.MenuToolsRestructureImage.Size = New System.Drawing.Size(170, 22)
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
        'MenuToolsSeparator
        '
        MenuToolsSeparator.Name = "MenuToolsSeparator"
        MenuToolsSeparator.Size = New System.Drawing.Size(286, 6)
        '
        'MenuToolsWin9xCleanBatch
        '
        Me.MenuToolsWin9xCleanBatch.Name = "MenuToolsWin9xCleanBatch"
        Me.MenuToolsWin9xCleanBatch.Size = New System.Drawing.Size(289, 22)
        Me.MenuToolsWin9xCleanBatch.Text = "Batch Remove Windows Modifications"
        '
        'MenuToolsTrackLayout
        '
        Me.MenuToolsTrackLayout.Name = "MenuToolsTrackLayout"
        Me.MenuToolsTrackLayout.Size = New System.Drawing.Size(289, 22)
        Me.MenuToolsTrackLayout.Text = "Generate tracklayout.txt"
        '
        'MainMenuHelp
        '
        MainMenuHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuHelpProjectPage, Me.MenuHelpUpdateCheck, Me.MenuHelpChangeLog, MenuHelpSeparator, Me.MenuHelpAbout})
        MainMenuHelp.Name = "MainMenuHelp"
        MainMenuHelp.Size = New System.Drawing.Size(24, 20)
        MainMenuHelp.Text = "?"
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
        'MenuHelpSeparator
        '
        MenuHelpSeparator.Name = "MenuHelpSeparator"
        MenuHelpSeparator.Size = New System.Drawing.Size(209, 6)
        '
        'MenuHelpAbout
        '
        Me.MenuHelpAbout.Name = "MenuHelpAbout"
        Me.MenuHelpAbout.ShortcutKeys = System.Windows.Forms.Keys.F1
        Me.MenuHelpAbout.Size = New System.Drawing.Size(212, 22)
        Me.MenuHelpAbout.Text = "About Disk Image Tool"
        '
        'FileMenuSeparator2
        '
        FileMenuSeparator2.Name = "FileMenuSeparator2"
        FileMenuSeparator2.Size = New System.Drawing.Size(186, 6)
        '
        'FileMenuSeparator3
        '
        FileMenuSeparator3.Name = "FileMenuSeparator3"
        FileMenuSeparator3.Size = New System.Drawing.Size(186, 6)
        FileMenuSeparator3.Visible = False
        '
        'FileMenuSeparator4
        '
        FileMenuSeparator4.Name = "FileMenuSeparator4"
        FileMenuSeparator4.Size = New System.Drawing.Size(186, 6)
        '
        'FileMenuSeparator5
        '
        FileMenuSeparator5.Name = "FileMenuSeparator5"
        FileMenuSeparator5.Size = New System.Drawing.Size(186, 6)
        '
        'ToolStripSeparator6
        '
        ToolStripSeparator6.Name = "ToolStripSeparator6"
        ToolStripSeparator6.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripSeparator7
        '
        ToolStripSeparator7.Name = "ToolStripSeparator7"
        ToolStripSeparator7.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripSeparator8
        '
        ToolStripSeparator8.Name = "ToolStripSeparator8"
        ToolStripSeparator8.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripSeparator10
        '
        ToolStripSeparator10.Name = "ToolStripSeparator10"
        ToolStripSeparator10.Size = New System.Drawing.Size(6, 25)
        '
        'MenuDiskSeparator
        '
        MenuDiskSeparator.Name = "MenuDiskSeparator"
        MenuDiskSeparator.Size = New System.Drawing.Size(178, 6)
        '
        'ToolStripSeparator9
        '
        ToolStripSeparator9.Name = "ToolStripSeparator9"
        ToolStripSeparator9.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripSeparator11
        '
        ToolStripSeparator11.Name = "ToolStripSeparator11"
        ToolStripSeparator11.Size = New System.Drawing.Size(147, 6)
        '
        'MainMenuDisk
        '
        MainMenuDisk.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuDiskReadFloppyA, Me.MenuDiskReadFloppyB, MenuDiskSeparator, Me.MenuDiskWriteFloppyA, Me.MenuDiskWriteFloppyB})
        MainMenuDisk.Name = "MainMenuDisk"
        MainMenuDisk.Size = New System.Drawing.Size(41, 20)
        MainMenuDisk.Text = "&Disk"
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
        'MenuStripTop
        '
        MenuStripTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {MainMenuFile, MainMenuEdit, Me.MainMenuFilters, MainMenuView, MainMenuTools, MainMenuDisk, Me.MainMenuOptions, MainMenuHelp, MainMenuExperimental})
        MenuStripTop.Location = New System.Drawing.Point(0, 0)
        MenuStripTop.Name = "MenuStripTop"
        MenuStripTop.Padding = New System.Windows.Forms.Padding(6, 2, 12, 2)
        MenuStripTop.Size = New System.Drawing.Size(1004, 24)
        MenuStripTop.TabIndex = 0
        MenuStripTop.Text = "MenuStrip1"
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
        Me.ContextMenuFilters.OwnerItem = Me.MainMenuFilters
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
        'MainMenuOptions
        '
        Me.MainMenuOptions.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuOptionsCreateBackup, Me.MenuOptionsDragDrop, Me.MenuOptionsWindowsExtensions, Me.MenuOptionsSeparatorDebug, Me.MenuOptionsExportUnknown})
        Me.MainMenuOptions.Name = "MainMenuOptions"
        Me.MainMenuOptions.Size = New System.Drawing.Size(61, 20)
        Me.MainMenuOptions.Text = "&Options"
        '
        'MenuOptionsCreateBackup
        '
        Me.MenuOptionsCreateBackup.CheckOnClick = True
        Me.MenuOptionsCreateBackup.Name = "MenuOptionsCreateBackup"
        Me.MenuOptionsCreateBackup.Size = New System.Drawing.Size(248, 22)
        Me.MenuOptionsCreateBackup.Text = "Create Backup on Save"
        '
        'MenuOptionsDragDrop
        '
        Me.MenuOptionsDragDrop.CheckOnClick = True
        Me.MenuOptionsDragDrop.Name = "MenuOptionsDragDrop"
        Me.MenuOptionsDragDrop.Size = New System.Drawing.Size(248, 22)
        Me.MenuOptionsDragDrop.Text = "Import using Drag and Drop"
        '
        'MenuOptionsWindowsExtensions
        '
        Me.MenuOptionsWindowsExtensions.CheckOnClick = True
        Me.MenuOptionsWindowsExtensions.Name = "MenuOptionsWindowsExtensions"
        Me.MenuOptionsWindowsExtensions.Size = New System.Drawing.Size(248, 22)
        Me.MenuOptionsWindowsExtensions.Text = "Enable Windows Extensions"
        Me.MenuOptionsWindowsExtensions.ToolTipText = "Enabled Created and Last Access dates and Long File Name support when adding file" &
    "s"
        '
        'MenuOptionsSeparatorDebug
        '
        Me.MenuOptionsSeparatorDebug.Name = "MenuOptionsSeparatorDebug"
        Me.MenuOptionsSeparatorDebug.Size = New System.Drawing.Size(245, 6)
        '
        'MenuOptionsExportUnknown
        '
        Me.MenuOptionsExportUnknown.CheckOnClick = True
        Me.MenuOptionsExportUnknown.Name = "MenuOptionsExportUnknown"
        Me.MenuOptionsExportUnknown.Size = New System.Drawing.Size(248, 22)
        Me.MenuOptionsExportUnknown.Text = "Export Unknown Images on Scan"
        '
        'ToolStripTop
        '
        Me.ToolStripTop.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStripTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripSearchText, ToolStripSearchLabel, Me.ToolStripOEMNameCombo, Me.ToolStripOEMNameLabel, Me.ToolStripDiskTypeCombo, Me.ToolStripDiskTypeLabel, Me.ToolStripOpen, ToolStripSeparator6, Me.ToolStripSave, Me.ToolStripSaveAs, Me.ToolStripSaveAll, ToolStripSeparator7, Me.ToolStripClose, Me.ToolStripCloseAll, ToolStripSeparator8, Me.ToolStripFileProperties, Me.ToolStripExportFile, ToolStripSeparator9, Me.ToolStripUndo, Me.ToolStripRedo, ToolStripSeparator10, Me.ToolStripViewFileText, Me.ToolStripViewFile, Me.ToolStripSeparatorFAT, Me.ToolStripFATCombo})
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
        'StatusStripBottom
        '
        Me.StatusStripBottom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusReadOnly, Me.ToolStripStatusModified, Me.ToolStripFileName, Me.ToolStripFileCount, Me.ToolStripFileSector, Me.ToolStripFileTrack, Me.ToolStripImageCount, Me.ToolStripModified})
        Me.StatusStripBottom.Location = New System.Drawing.Point(0, 552)
        Me.StatusStripBottom.Name = "StatusStripBottom"
        Me.StatusStripBottom.ShowItemToolTips = True
        Me.StatusStripBottom.Size = New System.Drawing.Size(1004, 24)
        Me.StatusStripBottom.TabIndex = 9
        Me.StatusStripBottom.Text = "StatusStripMain"
        '
        'ToolStripStatusReadOnly
        '
        Me.ToolStripStatusReadOnly.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.ToolStripStatusReadOnly.ForeColor = System.Drawing.Color.Red
        Me.ToolStripStatusReadOnly.Name = "ToolStripStatusReadOnly"
        Me.ToolStripStatusReadOnly.Size = New System.Drawing.Size(61, 19)
        Me.ToolStripStatusReadOnly.Text = "Read Only"
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
        Me.ToolStripFileName.Size = New System.Drawing.Size(499, 19)
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
        Me.ListViewFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {FileModified, FileName, FileExt, FileSize, FileLastWriteDate, FileStartingCluster, Me.FileClusterError, FileAttrib, FileCRC32, Me.FileCreationDate, Me.FileLastAccessDate, Me.FileNTReserved, Me.FileFAT32Cluster, Me.FileLFN})
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
        'FileNTReserved
        '
        Me.FileNTReserved.Text = "NT"
        Me.FileNTReserved.Width = 30
        '
        'FileFAT32Cluster
        '
        Me.FileFAT32Cluster.Text = "FAT32"
        Me.FileFAT32Cluster.Width = 50
        '
        'FileLFN
        '
        Me.FileLFN.Text = "Long File Name"
        Me.FileLFN.Width = 200
        '
        'ContextMenuFiles
        '
        Me.ContextMenuFiles.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuFileFileProperties, Me.MenuFileExportFile, Me.MenuFileReplaceFile, FileMenuSeparator1, Me.MenuFileViewDirectory, Me.FileMenuSeparatorDirectory, Me.MenuFileViewFile, Me.MenuFileViewFileText, Me.MenuFileViewCrosslinked, FileMenuSeparator2, Me.MenuFileImportFiles, Me.MenuFileImportFilesHere, FileMenuSeparator5, Me.MenuFileNewDirectory, Me.MenuFileNewDirectoryHere, FileMenuSeparator4, Me.MenuFileDeleteFile, Me.MenuFileUnDeleteFile, Me.MenuFileRemove, FileMenuSeparator3, Me.MenuFileFixSize})
        Me.ContextMenuFiles.Name = "ContextMenuFiles"
        Me.ContextMenuFiles.Size = New System.Drawing.Size(190, 370)
        '
        'MenuFileFileProperties
        '
        Me.MenuFileFileProperties.Image = CType(resources.GetObject("MenuFileFileProperties.Image"), System.Drawing.Image)
        Me.MenuFileFileProperties.Name = "MenuFileFileProperties"
        Me.MenuFileFileProperties.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileFileProperties.Text = "Edit File &Properties"
        '
        'MenuFileExportFile
        '
        Me.MenuFileExportFile.Image = CType(resources.GetObject("MenuFileExportFile.Image"), System.Drawing.Image)
        Me.MenuFileExportFile.Name = "MenuFileExportFile"
        Me.MenuFileExportFile.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileExportFile.Text = "&Export File"
        '
        'MenuFileReplaceFile
        '
        Me.MenuFileReplaceFile.Name = "MenuFileReplaceFile"
        Me.MenuFileReplaceFile.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileReplaceFile.Text = "&Replace File"
        '
        'MenuFileViewDirectory
        '
        Me.MenuFileViewDirectory.Name = "MenuFileViewDirectory"
        Me.MenuFileViewDirectory.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileViewDirectory.Text = "View Parent D&irectory"
        '
        'FileMenuSeparatorDirectory
        '
        Me.FileMenuSeparatorDirectory.Name = "FileMenuSeparatorDirectory"
        Me.FileMenuSeparatorDirectory.Size = New System.Drawing.Size(186, 6)
        '
        'MenuFileViewFile
        '
        Me.MenuFileViewFile.Image = CType(resources.GetObject("MenuFileViewFile.Image"), System.Drawing.Image)
        Me.MenuFileViewFile.Name = "MenuFileViewFile"
        Me.MenuFileViewFile.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileViewFile.Text = "&View File"
        '
        'MenuFileViewFileText
        '
        Me.MenuFileViewFileText.Image = CType(resources.GetObject("MenuFileViewFileText.Image"), System.Drawing.Image)
        Me.MenuFileViewFileText.Name = "MenuFileViewFileText"
        Me.MenuFileViewFileText.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileViewFileText.Text = "View File as &Text"
        '
        'MenuFileViewCrosslinked
        '
        Me.MenuFileViewCrosslinked.Name = "MenuFileViewCrosslinked"
        Me.MenuFileViewCrosslinked.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileViewCrosslinked.Text = "View &Crosslinked Files"
        '
        'MenuFileImportFiles
        '
        Me.MenuFileImportFiles.Name = "MenuFileImportFiles"
        Me.MenuFileImportFiles.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileImportFiles.Text = "&Import Files"
        '
        'MenuFileImportFilesHere
        '
        Me.MenuFileImportFilesHere.Name = "MenuFileImportFilesHere"
        Me.MenuFileImportFilesHere.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileImportFilesHere.Text = "&Import Files Here"
        '
        'MenuFileNewDirectory
        '
        Me.MenuFileNewDirectory.Name = "MenuFileNewDirectory"
        Me.MenuFileNewDirectory.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileNewDirectory.Text = "&New Directory"
        '
        'MenuFileNewDirectoryHere
        '
        Me.MenuFileNewDirectoryHere.Name = "MenuFileNewDirectoryHere"
        Me.MenuFileNewDirectoryHere.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileNewDirectoryHere.Text = "&New Directory Here"
        '
        'MenuFileDeleteFile
        '
        Me.MenuFileDeleteFile.Name = "MenuFileDeleteFile"
        Me.MenuFileDeleteFile.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileDeleteFile.Text = "&Delete File"
        '
        'MenuFileUnDeleteFile
        '
        Me.MenuFileUnDeleteFile.Name = "MenuFileUnDeleteFile"
        Me.MenuFileUnDeleteFile.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileUnDeleteFile.Text = "&Undelete File"
        '
        'MenuFileRemove
        '
        Me.MenuFileRemove.Name = "MenuFileRemove"
        Me.MenuFileRemove.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileRemove.Text = "Remove &Deleted File"
        '
        'MenuFileFixSize
        '
        Me.MenuFileFixSize.Name = "MenuFileFixSize"
        Me.MenuFileFixSize.Size = New System.Drawing.Size(189, 22)
        Me.MenuFileFixSize.Text = "Fix File &Size"
        Me.MenuFileFixSize.Visible = False
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
        'ContextMenuDirectory
        '
        Me.ContextMenuDirectory.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuDirectoryView, ToolStripSeparator11, Me.MenuDirectoryImportFiles, Me.MenuDirectoryNewDirectory})
        Me.ContextMenuDirectory.Name = "ContextMenuDirectory"
        Me.ContextMenuDirectory.Size = New System.Drawing.Size(151, 76)
        '
        'MenuDirectoryView
        '
        Me.MenuDirectoryView.Name = "MenuDirectoryView"
        Me.MenuDirectoryView.Size = New System.Drawing.Size(150, 22)
        Me.MenuDirectoryView.Text = "View D&irectory"
        '
        'MenuDirectoryImportFiles
        '
        Me.MenuDirectoryImportFiles.Name = "MenuDirectoryImportFiles"
        Me.MenuDirectoryImportFiles.Size = New System.Drawing.Size(150, 22)
        Me.MenuDirectoryImportFiles.Text = "&Import Files"
        '
        'MenuDirectoryNewDirectory
        '
        Me.MenuDirectoryNewDirectory.Name = "MenuDirectoryNewDirectory"
        Me.MenuDirectoryNewDirectory.Size = New System.Drawing.Size(150, 22)
        Me.MenuDirectoryNewDirectory.Text = "&New Directory"
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
        Me.Controls.Add(Me.StatusStripBottom)
        Me.Controls.Add(MenuStripTop)
        Me.Controls.Add(Me.LabelDropMessage)
        Me.Controls.Add(Me.ListViewHashes)
        Me.Controls.Add(Me.ComboImages)
        Me.Controls.Add(Me.ListViewSummary)
        Me.Controls.Add(Me.ListViewFiles)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = MenuStripTop
        Me.MinimumSize = New System.Drawing.Size(1020, 600)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Disk Image Tool"
        MenuStripTop.ResumeLayout(False)
        MenuStripTop.PerformLayout()
        Me.ContextMenuFilters.ResumeLayout(False)
        Me.ToolStripTop.ResumeLayout(False)
        Me.ToolStripTop.PerformLayout()
        Me.StatusStripBottom.ResumeLayout(False)
        Me.StatusStripBottom.PerformLayout()
        Me.ContextMenuFiles.ResumeLayout(False)
        Me.ContextMenuDirectory.ResumeLayout(False)
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
    Friend WithEvents BtnExportDebug As ToolStripMenuItem
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
    Friend WithEvents ToolStripDiskTypeCombo As ToolStripComboBox
    Friend WithEvents ToolStripDiskTypeLabel As ToolStripLabel
    Friend WithEvents MenuToolsWin9xCleanBatch As ToolStripMenuItem
    Friend WithEvents MenuToolsCompare As ToolStripMenuItem
    Friend WithEvents MenuFileUnDeleteFile As ToolStripMenuItem
    Friend WithEvents FileNTReserved As ColumnHeader
    Friend WithEvents MenuToolsClearReservedBytes As ToolStripMenuItem
    Friend WithEvents MenuHexLostClusters As ToolStripMenuItem
    Friend WithEvents ToolStripSeparatorFAT As ToolStripSeparator
    Friend WithEvents ToolStripFATCombo As ToolStripComboBox
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
    Friend WithEvents MenuOptionsWindowsExtensions As ToolStripMenuItem
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
    Friend WithEvents MenuOptionsSeparatorDebug As ToolStripSeparator
    Friend WithEvents MenuOptionsExportUnknown As ToolStripMenuItem
    Friend WithEvents StatusStripBottom As StatusStrip
    Friend WithEvents ToolStripTop As ToolStrip
    Friend WithEvents MenuOptionsDragDrop As ToolStripMenuItem
    Friend WithEvents FileFAT32Cluster As ColumnHeader
    Friend WithEvents MenuToolsTrackLayout As ToolStripMenuItem
End Class
