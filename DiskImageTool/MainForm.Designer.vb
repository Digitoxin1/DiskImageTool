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
        Dim HashName As System.Windows.Forms.ColumnHeader
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Dim SplitContainer1 As System.Windows.Forms.SplitContainer
        Dim PanelSpacer3 As System.Windows.Forms.Panel
        Dim PanelSpacer2 As System.Windows.Forms.Panel
        Dim PanelCombo As System.Windows.Forms.Panel
        Dim PanelSpacer1 As System.Windows.Forms.Panel
        Dim HashValue As System.Windows.Forms.ColumnHeader
        Dim MenuFileSeparator1 As System.Windows.Forms.ToolStripSeparator
        Dim MenuFileSeparator2 As System.Windows.Forms.ToolStripSeparator
        Dim MenuFileSeparator3 As System.Windows.Forms.ToolStripSeparator
        Dim MenuEditSeparator1 As System.Windows.Forms.ToolStripSeparator
        Dim MenuEditSeparator2 As System.Windows.Forms.ToolStripSeparator
        Dim MainMenuView As System.Windows.Forms.ToolStripMenuItem
        Dim MainMenuTools As System.Windows.Forms.ToolStripMenuItem
        Dim MenuToolsSeparator As System.Windows.Forms.ToolStripSeparator
        Dim MenuHelpSeparator As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator8 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator10 As System.Windows.Forms.ToolStripSeparator
        Dim MenuDiskSeparator As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator9 As System.Windows.Forms.ToolStripSeparator
        Dim MainMenuDisk As System.Windows.Forms.ToolStripMenuItem
        Dim MenuStripTop As System.Windows.Forms.MenuStrip
        Me.btnRetry = New System.Windows.Forms.Button()
        Me.ListViewSummary = New System.Windows.Forms.ListView()
        Me.HashPanel1 = New DiskImageTool.HashPanel()
        Me.PanelFiles = New System.Windows.Forms.Panel()
        Me.LabelDropMessage = New System.Windows.Forms.Label()
        Me.ListViewFiles = New DiskImageTool.ListViewEx()
        Me.PanelOverlay = New System.Windows.Forms.TableLayoutPanel()
        Me.PanelOverlayTopZone = New System.Windows.Forms.Panel()
        Me.LabelOpenImages = New System.Windows.Forms.Label()
        Me.PanelOverlayBottomZone = New System.Windows.Forms.Panel()
        Me.LabelImportFiles = New System.Windows.Forms.Label()
        Me.ComboImages = New System.Windows.Forms.ComboBox()
        Me.BtnResetSort = New System.Windows.Forms.Button()
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
        Me.MenuToolsWin9xClean = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsClearReservedBytes = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsFixImageSize = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsFixImageSizeSubMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsTruncateImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsRestructureImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsRestoreBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsRemoveBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsWin9xCleanBatch = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskReadFloppyA = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskReadFloppyB = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskWriteFloppyA = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskWriteFloppyB = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenuFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileOpen = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileReload = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileNewImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileSave = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileSaveAs = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileSaveAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileClose = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileCloseAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileExit = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenuEdit = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.MainMenuFilters = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuFilters = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.MenuFiltersScanNew = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFiltersScan = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFiltersClear = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenuFlux = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuGreaseweazleRead = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuGreaseweazleWrite = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuFluxConvert = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparatorDevices = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuGreaseweazle = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuGreaseweazleErase = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuGreaseweazleClean = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator12 = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuGreaseweazleInfo = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuGreaseweazleBandwidth = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuPcImgCnv = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuPcImgCnvTrackLayout = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenuReports = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuReportsModifications = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuReportsImageAnalysis = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuReportsBatchImageAnalysis = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenuOptions = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuOptionsCreateBackup = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuOptionsCheckUpdate = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuOptionsDragDrop = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuOptionsDisplayTitles = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuOptionsDisplayLanguage = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.MenuOptionsFlux = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenuHelp = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHelpProjectPage = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHelpDocs = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHelpUpdateCheck = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHelpChangeLog = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuHelpAbout = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenuUpdateAvailable = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenuNewInstance = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.StatusBarStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusBarModified = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusBarFileName = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusBarFileCount = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusBarFileSector = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusBarFileTrack = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusBarImageCount = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusBarImagesModified = New System.Windows.Forms.ToolStripStatusLabel()
        HashName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        SplitContainer1 = New System.Windows.Forms.SplitContainer()
        PanelSpacer3 = New System.Windows.Forms.Panel()
        PanelSpacer2 = New System.Windows.Forms.Panel()
        PanelCombo = New System.Windows.Forms.Panel()
        PanelSpacer1 = New System.Windows.Forms.Panel()
        HashValue = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        MenuFileSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        MenuFileSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        MenuFileSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        MenuEditSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        MenuEditSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        MainMenuView = New System.Windows.Forms.ToolStripMenuItem()
        MainMenuTools = New System.Windows.Forms.ToolStripMenuItem()
        MenuToolsSeparator = New System.Windows.Forms.ToolStripSeparator()
        MenuHelpSeparator = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator10 = New System.Windows.Forms.ToolStripSeparator()
        MenuDiskSeparator = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator()
        MainMenuDisk = New System.Windows.Forms.ToolStripMenuItem()
        MenuStripTop = New System.Windows.Forms.MenuStrip()
        CType(SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer1.Panel1.SuspendLayout()
        SplitContainer1.Panel2.SuspendLayout()
        SplitContainer1.SuspendLayout()
        Me.PanelFiles.SuspendLayout()
        Me.PanelOverlay.SuspendLayout()
        Me.PanelOverlayTopZone.SuspendLayout()
        Me.PanelOverlayBottomZone.SuspendLayout()
        PanelCombo.SuspendLayout()
        MenuStripTop.SuspendLayout()
        Me.ContextMenuFilters.SuspendLayout()
        Me.ToolStripTop.SuspendLayout()
        Me.StatusStripBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'HashName
        '
        resources.ApplyResources(HashName, "HashName")
        '
        'SplitContainer1
        '
        resources.ApplyResources(SplitContainer1, "SplitContainer1")
        SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        SplitContainer1.Panel1.Controls.Add(Me.btnRetry)
        SplitContainer1.Panel1.Controls.Add(Me.ListViewSummary)
        SplitContainer1.Panel1.Controls.Add(PanelSpacer3)
        SplitContainer1.Panel1.Controls.Add(Me.HashPanel1)
        resources.ApplyResources(SplitContainer1.Panel1, "SplitContainer1.Panel1")
        '
        'SplitContainer1.Panel2
        '
        SplitContainer1.Panel2.Controls.Add(Me.PanelFiles)
        SplitContainer1.Panel2.Controls.Add(PanelSpacer2)
        SplitContainer1.Panel2.Controls.Add(PanelCombo)
        resources.ApplyResources(SplitContainer1.Panel2, "SplitContainer1.Panel2")
        '
        'btnRetry
        '
        resources.ApplyResources(Me.btnRetry, "btnRetry")
        Me.btnRetry.Name = "btnRetry"
        Me.btnRetry.UseVisualStyleBackColor = True
        '
        'ListViewSummary
        '
        Me.ListViewSummary.AllowDrop = True
        resources.ApplyResources(Me.ListViewSummary, "ListViewSummary")
        Me.ListViewSummary.HideSelection = False
        Me.ListViewSummary.Name = "ListViewSummary"
        Me.ListViewSummary.UseCompatibleStateImageBehavior = False
        '
        'PanelSpacer3
        '
        resources.ApplyResources(PanelSpacer3, "PanelSpacer3")
        PanelSpacer3.Name = "PanelSpacer3"
        '
        'HashPanel1
        '
        Me.HashPanel1.AllowDrop = True
        Me.HashPanel1.BackColor = System.Drawing.SystemColors.Window
        Me.HashPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.HashPanel1, "HashPanel1")
        Me.HashPanel1.Name = "HashPanel1"
        '
        'PanelFiles
        '
        Me.PanelFiles.AllowDrop = True
        Me.PanelFiles.Controls.Add(Me.LabelDropMessage)
        Me.PanelFiles.Controls.Add(Me.ListViewFiles)
        Me.PanelFiles.Controls.Add(Me.PanelOverlay)
        resources.ApplyResources(Me.PanelFiles, "PanelFiles")
        Me.PanelFiles.Name = "PanelFiles"
        '
        'LabelDropMessage
        '
        resources.ApplyResources(Me.LabelDropMessage, "LabelDropMessage")
        Me.LabelDropMessage.BackColor = System.Drawing.SystemColors.Window
        Me.LabelDropMessage.Name = "LabelDropMessage"
        '
        'ListViewFiles
        '
        resources.ApplyResources(Me.ListViewFiles, "ListViewFiles")
        Me.ListViewFiles.HideSelection = False
        Me.ListViewFiles.Name = "ListViewFiles"
        Me.ListViewFiles.UseCompatibleStateImageBehavior = False
        '
        'PanelOverlay
        '
        Me.PanelOverlay.BackColor = System.Drawing.SystemColors.Window
        resources.ApplyResources(Me.PanelOverlay, "PanelOverlay")
        Me.PanelOverlay.Controls.Add(Me.PanelOverlayTopZone, 0, 0)
        Me.PanelOverlay.Controls.Add(Me.PanelOverlayBottomZone, 0, 1)
        Me.PanelOverlay.Name = "PanelOverlay"
        '
        'PanelOverlayTopZone
        '
        Me.PanelOverlayTopZone.Controls.Add(Me.LabelOpenImages)
        resources.ApplyResources(Me.PanelOverlayTopZone, "PanelOverlayTopZone")
        Me.PanelOverlayTopZone.Name = "PanelOverlayTopZone"
        '
        'LabelOpenImages
        '
        resources.ApplyResources(Me.LabelOpenImages, "LabelOpenImages")
        Me.LabelOpenImages.Name = "LabelOpenImages"
        '
        'PanelOverlayBottomZone
        '
        Me.PanelOverlayBottomZone.Controls.Add(Me.LabelImportFiles)
        resources.ApplyResources(Me.PanelOverlayBottomZone, "PanelOverlayBottomZone")
        Me.PanelOverlayBottomZone.Name = "PanelOverlayBottomZone"
        '
        'LabelImportFiles
        '
        resources.ApplyResources(Me.LabelImportFiles, "LabelImportFiles")
        Me.LabelImportFiles.Name = "LabelImportFiles"
        '
        'PanelSpacer2
        '
        resources.ApplyResources(PanelSpacer2, "PanelSpacer2")
        PanelSpacer2.Name = "PanelSpacer2"
        '
        'PanelCombo
        '
        PanelCombo.Controls.Add(Me.ComboImages)
        PanelCombo.Controls.Add(PanelSpacer1)
        PanelCombo.Controls.Add(Me.BtnResetSort)
        resources.ApplyResources(PanelCombo, "PanelCombo")
        PanelCombo.Name = "PanelCombo"
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
        'PanelSpacer1
        '
        resources.ApplyResources(PanelSpacer1, "PanelSpacer1")
        PanelSpacer1.Name = "PanelSpacer1"
        '
        'BtnResetSort
        '
        resources.ApplyResources(Me.BtnResetSort, "BtnResetSort")
        Me.BtnResetSort.Name = "BtnResetSort"
        Me.BtnResetSort.UseVisualStyleBackColor = True
        '
        'HashValue
        '
        resources.ApplyResources(HashValue, "HashValue")
        '
        'MenuFileSeparator1
        '
        MenuFileSeparator1.Name = "MenuFileSeparator1"
        resources.ApplyResources(MenuFileSeparator1, "MenuFileSeparator1")
        '
        'MenuFileSeparator2
        '
        MenuFileSeparator2.Name = "MenuFileSeparator2"
        resources.ApplyResources(MenuFileSeparator2, "MenuFileSeparator2")
        '
        'MenuFileSeparator3
        '
        MenuFileSeparator3.Name = "MenuFileSeparator3"
        resources.ApplyResources(MenuFileSeparator3, "MenuFileSeparator3")
        '
        'MenuEditSeparator1
        '
        MenuEditSeparator1.Name = "MenuEditSeparator1"
        resources.ApplyResources(MenuEditSeparator1, "MenuEditSeparator1")
        '
        'MenuEditSeparator2
        '
        MenuEditSeparator2.Name = "MenuEditSeparator2"
        resources.ApplyResources(MenuEditSeparator2, "MenuEditSeparator2")
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
        'MainMenuTools
        '
        MainMenuTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuToolsWin9xClean, Me.MenuToolsClearReservedBytes, Me.MenuToolsFixImageSize, Me.MenuToolsFixImageSizeSubMenu, Me.MenuToolsRestoreBootSector, Me.MenuToolsRemoveBootSector, MenuToolsSeparator, Me.MenuToolsWin9xCleanBatch})
        MainMenuTools.Name = "MainMenuTools"
        resources.ApplyResources(MainMenuTools, "MainMenuTools")
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
        'MenuHelpSeparator
        '
        MenuHelpSeparator.Name = "MenuHelpSeparator"
        resources.ApplyResources(MenuHelpSeparator, "MenuHelpSeparator")
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
        MenuStripTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MainMenuFile, Me.MainMenuEdit, Me.MainMenuFilters, MainMenuView, MainMenuTools, MainMenuDisk, Me.MainMenuFlux, Me.MainMenuReports, Me.MainMenuOptions, Me.MainMenuHelp, Me.MainMenuUpdateAvailable, Me.MainMenuNewInstance})
        resources.ApplyResources(MenuStripTop, "MenuStripTop")
        MenuStripTop.Name = "MenuStripTop"
        MenuStripTop.ShowItemToolTips = True
        '
        'MainMenuFile
        '
        Me.MainMenuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuFileOpen, Me.MenuFileReload, Me.MenuFileNewImage, MenuFileSeparator1, Me.MenuFileSave, Me.MenuFileSaveAs, Me.MenuFileSaveAll, MenuFileSeparator2, Me.MenuFileClose, Me.MenuFileCloseAll, MenuFileSeparator3, Me.MenuFileExit})
        Me.MainMenuFile.Name = "MainMenuFile"
        resources.ApplyResources(Me.MainMenuFile, "MainMenuFile")
        '
        'MenuFileOpen
        '
        Me.MenuFileOpen.Image = Global.DiskImageTool.My.Resources.Resources.OpenfileDialog
        resources.ApplyResources(Me.MenuFileOpen, "MenuFileOpen")
        Me.MenuFileOpen.Name = "MenuFileOpen"
        '
        'MenuFileReload
        '
        Me.MenuFileReload.Image = Global.DiskImageTool.My.Resources.Resources.Refresh
        Me.MenuFileReload.Name = "MenuFileReload"
        resources.ApplyResources(Me.MenuFileReload, "MenuFileReload")
        '
        'MenuFileNewImage
        '
        Me.MenuFileNewImage.Image = Global.DiskImageTool.My.Resources.Resources.NewDocument
        Me.MenuFileNewImage.Name = "MenuFileNewImage"
        resources.ApplyResources(Me.MenuFileNewImage, "MenuFileNewImage")
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
        'MenuFileExit
        '
        Me.MenuFileExit.Image = Global.DiskImageTool.My.Resources.Resources._Exit
        Me.MenuFileExit.Name = "MenuFileExit"
        resources.ApplyResources(Me.MenuFileExit, "MenuFileExit")
        '
        'MainMenuEdit
        '
        Me.MainMenuEdit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuEditBootSector, Me.MenuEditFAT, MenuEditSeparator1, Me.MenuEditFileProperties, Me.MenuEditExportFile, Me.MenuEditReplaceFile, MenuEditSeparator2, Me.MenuEditImportFiles, Me.ToolStripSeparator1, Me.MenuEditUndo, Me.MenuEditRedo, Me.MenuEditRevert})
        Me.MainMenuEdit.Name = "MainMenuEdit"
        resources.ApplyResources(Me.MainMenuEdit, "MainMenuEdit")
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
        Me.MenuEditReplaceFile.Image = Global.DiskImageTool.My.Resources.Resources.SwitchFolders
        Me.MenuEditReplaceFile.Name = "MenuEditReplaceFile"
        resources.ApplyResources(Me.MenuEditReplaceFile, "MenuEditReplaceFile")
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
        'MainMenuFlux
        '
        Me.MainMenuFlux.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuGreaseweazleRead, Me.MenuGreaseweazleWrite, Me.ToolStripSeparator5, Me.MenuFluxConvert, Me.ToolStripSeparatorDevices, Me.MenuGreaseweazle, Me.MenuPcImgCnv})
        Me.MainMenuFlux.Name = "MainMenuFlux"
        resources.ApplyResources(Me.MainMenuFlux, "MainMenuFlux")
        '
        'MenuGreaseweazleRead
        '
        Me.MenuGreaseweazleRead.Name = "MenuGreaseweazleRead"
        resources.ApplyResources(Me.MenuGreaseweazleRead, "MenuGreaseweazleRead")
        '
        'MenuGreaseweazleWrite
        '
        Me.MenuGreaseweazleWrite.Name = "MenuGreaseweazleWrite"
        resources.ApplyResources(Me.MenuGreaseweazleWrite, "MenuGreaseweazleWrite")
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        resources.ApplyResources(Me.ToolStripSeparator5, "ToolStripSeparator5")
        '
        'MenuFluxConvert
        '
        Me.MenuFluxConvert.Name = "MenuFluxConvert"
        resources.ApplyResources(Me.MenuFluxConvert, "MenuFluxConvert")
        '
        'ToolStripSeparatorDevices
        '
        Me.ToolStripSeparatorDevices.Name = "ToolStripSeparatorDevices"
        resources.ApplyResources(Me.ToolStripSeparatorDevices, "ToolStripSeparatorDevices")
        '
        'MenuGreaseweazle
        '
        Me.MenuGreaseweazle.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuGreaseweazleErase, Me.MenuGreaseweazleClean, Me.ToolStripSeparator12, Me.MenuGreaseweazleInfo, Me.MenuGreaseweazleBandwidth})
        Me.MenuGreaseweazle.Name = "MenuGreaseweazle"
        resources.ApplyResources(Me.MenuGreaseweazle, "MenuGreaseweazle")
        '
        'MenuGreaseweazleErase
        '
        Me.MenuGreaseweazleErase.Name = "MenuGreaseweazleErase"
        resources.ApplyResources(Me.MenuGreaseweazleErase, "MenuGreaseweazleErase")
        '
        'MenuGreaseweazleClean
        '
        Me.MenuGreaseweazleClean.Name = "MenuGreaseweazleClean"
        resources.ApplyResources(Me.MenuGreaseweazleClean, "MenuGreaseweazleClean")
        '
        'ToolStripSeparator12
        '
        Me.ToolStripSeparator12.Name = "ToolStripSeparator12"
        resources.ApplyResources(Me.ToolStripSeparator12, "ToolStripSeparator12")
        '
        'MenuGreaseweazleInfo
        '
        Me.MenuGreaseweazleInfo.Name = "MenuGreaseweazleInfo"
        resources.ApplyResources(Me.MenuGreaseweazleInfo, "MenuGreaseweazleInfo")
        '
        'MenuGreaseweazleBandwidth
        '
        Me.MenuGreaseweazleBandwidth.Name = "MenuGreaseweazleBandwidth"
        resources.ApplyResources(Me.MenuGreaseweazleBandwidth, "MenuGreaseweazleBandwidth")
        '
        'MenuPcImgCnv
        '
        Me.MenuPcImgCnv.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuPcImgCnvTrackLayout})
        Me.MenuPcImgCnv.Name = "MenuPcImgCnv"
        resources.ApplyResources(Me.MenuPcImgCnv, "MenuPcImgCnv")
        '
        'MenuPcImgCnvTrackLayout
        '
        Me.MenuPcImgCnvTrackLayout.Name = "MenuPcImgCnvTrackLayout"
        resources.ApplyResources(Me.MenuPcImgCnvTrackLayout, "MenuPcImgCnvTrackLayout")
        '
        'MainMenuReports
        '
        Me.MainMenuReports.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuReportsModifications, Me.MenuReportsImageAnalysis, Me.MenuReportsBatchImageAnalysis})
        Me.MainMenuReports.Name = "MainMenuReports"
        resources.ApplyResources(Me.MainMenuReports, "MainMenuReports")
        '
        'MenuReportsModifications
        '
        Me.MenuReportsModifications.Name = "MenuReportsModifications"
        resources.ApplyResources(Me.MenuReportsModifications, "MenuReportsModifications")
        '
        'MenuReportsImageAnalysis
        '
        Me.MenuReportsImageAnalysis.Name = "MenuReportsImageAnalysis"
        resources.ApplyResources(Me.MenuReportsImageAnalysis, "MenuReportsImageAnalysis")
        '
        'MenuReportsBatchImageAnalysis
        '
        Me.MenuReportsBatchImageAnalysis.Name = "MenuReportsBatchImageAnalysis"
        resources.ApplyResources(Me.MenuReportsBatchImageAnalysis, "MenuReportsBatchImageAnalysis")
        '
        'MainMenuOptions
        '
        Me.MainMenuOptions.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuOptionsCreateBackup, Me.MenuOptionsCheckUpdate, Me.MenuOptionsDragDrop, Me.MenuOptionsDisplayTitles, Me.MenuOptionsDisplayLanguage, Me.ToolStripSeparator2, Me.MenuOptionsFlux})
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
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
        '
        'MenuOptionsFlux
        '
        Me.MenuOptionsFlux.Name = "MenuOptionsFlux"
        resources.ApplyResources(Me.MenuOptionsFlux, "MenuOptionsFlux")
        '
        'MainMenuHelp
        '
        Me.MainMenuHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuHelpProjectPage, Me.MenuHelpDocs, Me.MenuHelpUpdateCheck, Me.MenuHelpChangeLog, MenuHelpSeparator, Me.MenuHelpAbout})
        Me.MainMenuHelp.Name = "MainMenuHelp"
        resources.ApplyResources(Me.MainMenuHelp, "MainMenuHelp")
        '
        'MenuHelpProjectPage
        '
        Me.MenuHelpProjectPage.Image = Global.DiskImageTool.My.Resources.Resources.Web
        Me.MenuHelpProjectPage.Name = "MenuHelpProjectPage"
        resources.ApplyResources(Me.MenuHelpProjectPage, "MenuHelpProjectPage")
        '
        'MenuHelpDocs
        '
        Me.MenuHelpDocs.Image = Global.DiskImageTool.My.Resources.Resources.HelpTableOfContents
        Me.MenuHelpDocs.Name = "MenuHelpDocs"
        resources.ApplyResources(Me.MenuHelpDocs, "MenuHelpDocs")
        '
        'MenuHelpUpdateCheck
        '
        Me.MenuHelpUpdateCheck.Image = Global.DiskImageTool.My.Resources.Resources.Refresh
        Me.MenuHelpUpdateCheck.Name = "MenuHelpUpdateCheck"
        resources.ApplyResources(Me.MenuHelpUpdateCheck, "MenuHelpUpdateCheck")
        '
        'MenuHelpChangeLog
        '
        Me.MenuHelpChangeLog.Image = Global.DiskImageTool.My.Resources.Resources.History
        Me.MenuHelpChangeLog.Name = "MenuHelpChangeLog"
        resources.ApplyResources(Me.MenuHelpChangeLog, "MenuHelpChangeLog")
        '
        'MenuHelpAbout
        '
        Me.MenuHelpAbout.Image = Global.DiskImageTool.My.Resources.Resources.AboutBox
        Me.MenuHelpAbout.Name = "MenuHelpAbout"
        resources.ApplyResources(Me.MenuHelpAbout, "MenuHelpAbout")
        '
        'MainMenuUpdateAvailable
        '
        Me.MainMenuUpdateAvailable.ForeColor = System.Drawing.Color.Blue
        Me.MainMenuUpdateAvailable.Margin = New System.Windows.Forms.Padding(12, 0, 0, 0)
        Me.MainMenuUpdateAvailable.Name = "MainMenuUpdateAvailable"
        resources.ApplyResources(Me.MainMenuUpdateAvailable, "MainMenuUpdateAvailable")
        '
        'MainMenuNewInstance
        '
        Me.MainMenuNewInstance.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.MainMenuNewInstance.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.MainMenuNewInstance.Image = Global.DiskImageTool.My.Resources.Resources.Instance
        Me.MainMenuNewInstance.Name = "MainMenuNewInstance"
        resources.ApplyResources(Me.MainMenuNewInstance, "MainMenuNewInstance")
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
        Me.ToolStripViewFile.Image = Global.DiskImageTool.My.Resources.Resources.BinaryFile
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
        Me.StatusStripBottom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusBarStatus, Me.StatusBarModified, Me.StatusBarFileName, Me.StatusBarFileCount, Me.StatusBarFileSector, Me.StatusBarFileTrack, Me.StatusBarImageCount, Me.StatusBarImagesModified})
        resources.ApplyResources(Me.StatusStripBottom, "StatusStripBottom")
        Me.StatusStripBottom.Name = "StatusStripBottom"
        Me.StatusStripBottom.ShowItemToolTips = True
        '
        'StatusBarStatus
        '
        resources.ApplyResources(Me.StatusBarStatus, "StatusBarStatus")
        Me.StatusBarStatus.ForeColor = System.Drawing.Color.Red
        Me.StatusBarStatus.Name = "StatusBarStatus"
        '
        'StatusBarModified
        '
        Me.StatusBarModified.ForeColor = System.Drawing.Color.Blue
        Me.StatusBarModified.Name = "StatusBarModified"
        resources.ApplyResources(Me.StatusBarModified, "StatusBarModified")
        '
        'StatusBarFileName
        '
        Me.StatusBarFileName.AutoToolTip = True
        Me.StatusBarFileName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.StatusBarFileName.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.StatusBarFileName.Name = "StatusBarFileName"
        resources.ApplyResources(Me.StatusBarFileName, "StatusBarFileName")
        Me.StatusBarFileName.Spring = True
        '
        'StatusBarFileCount
        '
        Me.StatusBarFileCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.StatusBarFileCount.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.StatusBarFileCount.Name = "StatusBarFileCount"
        resources.ApplyResources(Me.StatusBarFileCount, "StatusBarFileCount")
        '
        'StatusBarFileSector
        '
        Me.StatusBarFileSector.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.StatusBarFileSector.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.StatusBarFileSector.Name = "StatusBarFileSector"
        resources.ApplyResources(Me.StatusBarFileSector, "StatusBarFileSector")
        '
        'StatusBarFileTrack
        '
        Me.StatusBarFileTrack.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.StatusBarFileTrack.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.StatusBarFileTrack.Name = "StatusBarFileTrack"
        resources.ApplyResources(Me.StatusBarFileTrack, "StatusBarFileTrack")
        '
        'StatusBarImageCount
        '
        Me.StatusBarImageCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.StatusBarImageCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.StatusBarImageCount.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.StatusBarImageCount.Name = "StatusBarImageCount"
        resources.ApplyResources(Me.StatusBarImageCount, "StatusBarImageCount")
        '
        'StatusBarImagesModified
        '
        Me.StatusBarImagesModified.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.StatusBarImagesModified.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.StatusBarImagesModified.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.StatusBarImagesModified.Name = "StatusBarImagesModified"
        resources.ApplyResources(Me.StatusBarImagesModified, "StatusBarImagesModified")
        '
        'MainForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(SplitContainer1)
        Me.Controls.Add(Me.ToolStripTop)
        Me.Controls.Add(Me.StatusStripBottom)
        Me.Controls.Add(MenuStripTop)
        Me.MainMenuStrip = MenuStripTop
        Me.Name = "MainForm"
        SplitContainer1.Panel1.ResumeLayout(False)
        SplitContainer1.Panel2.ResumeLayout(False)
        CType(SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        SplitContainer1.ResumeLayout(False)
        Me.PanelFiles.ResumeLayout(False)
        Me.PanelFiles.PerformLayout()
        Me.PanelOverlay.ResumeLayout(False)
        Me.PanelOverlayTopZone.ResumeLayout(False)
        Me.PanelOverlayTopZone.PerformLayout()
        Me.PanelOverlayBottomZone.ResumeLayout(False)
        Me.PanelOverlayBottomZone.PerformLayout()
        PanelCombo.ResumeLayout(False)
        PanelCombo.PerformLayout()
        MenuStripTop.ResumeLayout(False)
        MenuStripTop.PerformLayout()
        Me.ContextMenuFilters.ResumeLayout(False)
        Me.ToolStripTop.ResumeLayout(False)
        Me.ToolStripTop.PerformLayout()
        Me.StatusStripBottom.ResumeLayout(False)
        Me.StatusStripBottom.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ListViewSummary As ListView
    Friend WithEvents ComboImages As ComboBox
    Friend WithEvents StatusBarImageCount As ToolStripStatusLabel
    Friend WithEvents StatusBarFileName As ToolStripStatusLabel
    Friend WithEvents StatusBarImagesModified As ToolStripStatusLabel
    Friend WithEvents ListViewFiles As ListViewEx
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
    Friend WithEvents StatusBarFileCount As ToolStripStatusLabel
    Friend WithEvents MenuEditFileProperties As ToolStripMenuItem
    Friend WithEvents MainMenuFilters As ToolStripMenuItem
    Friend WithEvents MenuHexFAT As ToolStripMenuItem
    Friend WithEvents MenuEditExportFile As ToolStripMenuItem
    Friend WithEvents MenuFiltersClear As ToolStripMenuItem
    Friend WithEvents StatusBarModified As ToolStripStatusLabel
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
    Friend WithEvents MenuToolsWin9xClean As ToolStripMenuItem
    Friend WithEvents MenuToolsFixImageSize As ToolStripMenuItem
    Friend WithEvents MenuHelpAbout As ToolStripMenuItem
    Friend WithEvents MenuHelpProjectPage As ToolStripMenuItem
    Friend WithEvents MenuHelpUpdateCheck As ToolStripMenuItem
    Friend WithEvents MenuEditFAT As ToolStripMenuItem
    Friend WithEvents StatusBarFileSector As ToolStripStatusLabel
    Friend WithEvents StatusBarFileTrack As ToolStripStatusLabel
    Friend WithEvents BtnResetSort As Button
    Friend WithEvents MenuEditBootSector As ToolStripMenuItem
    Friend WithEvents StatusBarStatus As ToolStripStatusLabel
    Friend WithEvents MenuToolsRestoreBootSector As ToolStripMenuItem
    Friend WithEvents MenuToolsRemoveBootSector As ToolStripMenuItem
    Friend WithEvents MenuToolsWin9xCleanBatch As ToolStripMenuItem
    Friend WithEvents MenuToolsClearReservedBytes As ToolStripMenuItem
    Friend WithEvents MenuHexLostClusters As ToolStripMenuItem
    Friend WithEvents ToolStripSeparatorFAT As ToolStripSeparator
    Friend WithEvents MenuDiskReadFloppyA As ToolStripMenuItem
    Friend WithEvents MenuDiskReadFloppyB As ToolStripMenuItem
    Friend WithEvents MenuDiskWriteFloppyA As ToolStripMenuItem
    Friend WithEvents MenuDiskWriteFloppyB As ToolStripMenuItem
    Friend WithEvents MenuOptionsCreateBackup As ToolStripMenuItem
    Friend WithEvents btnRetry As Button
    Friend WithEvents MenuToolsFixImageSizeSubMenu As ToolStripMenuItem
    Friend WithEvents MenuToolsTruncateImage As ToolStripMenuItem
    Friend WithEvents MenuToolsRestructureImage As ToolStripMenuItem
    Friend WithEvents MenuHexOverdumpData As ToolStripMenuItem
    Friend WithEvents MenuHelpChangeLog As ToolStripMenuItem
    Friend WithEvents MenuFileReload As ToolStripMenuItem
    Friend WithEvents MenuHexRawTrackData As ToolStripMenuItem
    Friend WithEvents MenuFileNewImage As ToolStripMenuItem
    Friend WithEvents MenuHexSeparatorFile As ToolStripSeparator
    Friend WithEvents MenuEditReplaceFile As ToolStripMenuItem
    Friend WithEvents MainMenuOptions As ToolStripMenuItem
    Friend WithEvents StatusStripBottom As StatusStrip
    Friend WithEvents ToolStripTop As ToolStrip
    Friend WithEvents MenuOptionsDragDrop As ToolStripMenuItem
    Friend WithEvents MenuOptionsCheckUpdate As ToolStripMenuItem
    Friend WithEvents MainMenuUpdateAvailable As ToolStripMenuItem
    Friend WithEvents MenuOptionsDisplayTitles As ToolStripMenuItem
    Friend WithEvents MenuOptionsDisplayLanguage As ToolStripMenuItem
    Friend WithEvents HashPanel1 As HashPanel
    Friend WithEvents ToolStripImportFiles As ToolStripButton
    Friend WithEvents MenuEditImportFiles As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents MainMenuReports As ToolStripMenuItem
    Friend WithEvents MenuReportsModifications As ToolStripMenuItem
    Friend WithEvents MainMenuNewInstance As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents MenuOptionsFlux As ToolStripMenuItem
    Friend WithEvents MainMenuFlux As ToolStripMenuItem
    Friend WithEvents MenuGreaseweazleRead As ToolStripMenuItem
    Friend WithEvents MenuGreaseweazleWrite As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As ToolStripSeparator
    Friend WithEvents MenuFluxConvert As ToolStripMenuItem
    Friend WithEvents MenuGreaseweazle As ToolStripMenuItem
    Friend WithEvents MenuGreaseweazleErase As ToolStripMenuItem
    Friend WithEvents MenuGreaseweazleClean As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator12 As ToolStripSeparator
    Friend WithEvents MenuGreaseweazleInfo As ToolStripMenuItem
    Friend WithEvents MenuGreaseweazleBandwidth As ToolStripMenuItem
    Friend WithEvents ToolStripSeparatorDevices As ToolStripSeparator
    Friend WithEvents MenuPcImgCnv As ToolStripMenuItem
    Friend WithEvents MenuPcImgCnvTrackLayout As ToolStripMenuItem
    Friend WithEvents MainMenuFile As ToolStripMenuItem
    Friend WithEvents MainMenuEdit As ToolStripMenuItem
    Friend WithEvents MainMenuHelp As ToolStripMenuItem
    Friend WithEvents MenuHelpDocs As ToolStripMenuItem
    Friend WithEvents PanelFiles As Panel
    Friend WithEvents PanelOverlay As TableLayoutPanel
    Friend WithEvents PanelOverlayTopZone As Panel
    Friend WithEvents PanelOverlayBottomZone As Panel
    Friend WithEvents LabelOpenImages As Label
    Friend WithEvents LabelImportFiles As Label
    Friend WithEvents MenuReportsImageAnalysis As ToolStripMenuItem
    Friend WithEvents MenuReportsBatchImageAnalysis As ToolStripMenuItem
End Class
