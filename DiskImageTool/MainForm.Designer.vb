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
        Dim MenuToolsSeparator As System.Windows.Forms.ToolStripSeparator
        Dim MenuHelpSeparator As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator8 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator10 As System.Windows.Forms.ToolStripSeparator
        Dim MenuDiskSeparator As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator9 As System.Windows.Forms.ToolStripSeparator
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
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
        Me.MenuEditSeparatorImage = New System.Windows.Forms.ToolStripSeparator()
        Me.MainMenuView = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.MainMenuTools = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsWin9xClean = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsClearReservedBytes = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsFixImageSize = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsFixImageSizeSubMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsTruncateImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsRestructureImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsRestoreBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsRemoveBootSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuToolsWin9xCleanBatch = New System.Windows.Forms.ToolStripMenuItem()
        Me.MainMenuDisk = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskReadFloppyA = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskReadFloppyB = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskWriteFloppyA = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuDiskWriteFloppyB = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStripTop = New System.Windows.Forms.MenuStrip()
        Me.MainMenuFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileOpen = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuFileRecent = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.MenuEditImageProperties = New System.Windows.Forms.ToolStripMenuItem()
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
        MenuToolsSeparator = New System.Windows.Forms.ToolStripSeparator()
        MenuHelpSeparator = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator10 = New System.Windows.Forms.ToolStripSeparator()
        MenuDiskSeparator = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator()
        CType(SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer1.Panel1.SuspendLayout()
        SplitContainer1.Panel2.SuspendLayout()
        SplitContainer1.SuspendLayout()
        Me.PanelFiles.SuspendLayout()
        Me.PanelOverlay.SuspendLayout()
        Me.PanelOverlayTopZone.SuspendLayout()
        Me.PanelOverlayBottomZone.SuspendLayout()
        PanelCombo.SuspendLayout()
        Me.MenuStripTop.SuspendLayout()
        Me.ContextMenuFilters.SuspendLayout()
        Me.ToolStripTop.SuspendLayout()
        Me.StatusStripBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'HashName
        '
        HashName.Width = -1
        '
        'SplitContainer1
        '
        SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        SplitContainer1.IsSplitterFixed = True
        SplitContainer1.Location = New System.Drawing.Point(0, 49)
        SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        SplitContainer1.Panel1.Controls.Add(Me.btnRetry)
        SplitContainer1.Panel1.Controls.Add(Me.ListViewSummary)
        SplitContainer1.Panel1.Controls.Add(PanelSpacer3)
        SplitContainer1.Panel1.Controls.Add(Me.HashPanel1)
        SplitContainer1.Panel1.Padding = New System.Windows.Forms.Padding(12, 6, 0, 6)
        '
        'SplitContainer1.Panel2
        '
        SplitContainer1.Panel2.Controls.Add(Me.PanelFiles)
        SplitContainer1.Panel2.Controls.Add(PanelSpacer2)
        SplitContainer1.Panel2.Controls.Add(PanelCombo)
        SplitContainer1.Panel2.Padding = New System.Windows.Forms.Padding(0, 6, 12, 6)
        SplitContainer1.Size = New System.Drawing.Size(1004, 503)
        SplitContainer1.SplitterDistance = 308
        SplitContainer1.SplitterWidth = 6
        SplitContainer1.TabIndex = 2
        '
        'btnRetry
        '
        Me.btnRetry.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRetry.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnRetry.Location = New System.Drawing.Point(216, 26)
        Me.btnRetry.Name = "btnRetry"
        Me.btnRetry.Size = New System.Drawing.Size(73, 23)
        Me.btnRetry.TabIndex = 0
        Me.btnRetry.Text = "{Retry}"
        Me.btnRetry.UseVisualStyleBackColor = True
        '
        'ListViewSummary
        '
        Me.ListViewSummary.AllowDrop = True
        Me.ListViewSummary.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListViewSummary.HideSelection = False
        Me.ListViewSummary.Location = New System.Drawing.Point(12, 6)
        Me.ListViewSummary.Name = "ListViewSummary"
        Me.ListViewSummary.Size = New System.Drawing.Size(296, 384)
        Me.ListViewSummary.TabIndex = 1
        Me.ListViewSummary.UseCompatibleStateImageBehavior = False
        '
        'PanelSpacer3
        '
        PanelSpacer3.Dock = System.Windows.Forms.DockStyle.Bottom
        PanelSpacer3.Location = New System.Drawing.Point(12, 390)
        PanelSpacer3.Name = "PanelSpacer3"
        PanelSpacer3.Size = New System.Drawing.Size(296, 6)
        PanelSpacer3.TabIndex = 2
        '
        'HashPanel1
        '
        Me.HashPanel1.AllowDrop = True
        Me.HashPanel1.BackColor = System.Drawing.SystemColors.Window
        Me.HashPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.HashPanel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.HashPanel1.Location = New System.Drawing.Point(12, 396)
        Me.HashPanel1.Name = "HashPanel1"
        Me.HashPanel1.Padding = New System.Windows.Forms.Padding(3)
        Me.HashPanel1.Size = New System.Drawing.Size(296, 101)
        Me.HashPanel1.TabIndex = 3
        '
        'PanelFiles
        '
        Me.PanelFiles.AllowDrop = True
        Me.PanelFiles.Controls.Add(Me.LabelDropMessage)
        Me.PanelFiles.Controls.Add(Me.ListViewFiles)
        Me.PanelFiles.Controls.Add(Me.PanelOverlay)
        Me.PanelFiles.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelFiles.Location = New System.Drawing.Point(0, 33)
        Me.PanelFiles.Name = "PanelFiles"
        Me.PanelFiles.Size = New System.Drawing.Size(678, 464)
        Me.PanelFiles.TabIndex = 3
        '
        'LabelDropMessage
        '
        Me.LabelDropMessage.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.LabelDropMessage.AutoSize = True
        Me.LabelDropMessage.BackColor = System.Drawing.SystemColors.Window
        Me.LabelDropMessage.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.LabelDropMessage.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelDropMessage.Location = New System.Drawing.Point(210, 224)
        Me.LabelDropMessage.Name = "LabelDropMessage"
        Me.LabelDropMessage.Size = New System.Drawing.Size(121, 16)
        Me.LabelDropMessage.TabIndex = 0
        Me.LabelDropMessage.Text = "{Drop Message}"
        Me.LabelDropMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ListViewFiles
        '
        Me.ListViewFiles.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListViewFiles.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.ListViewFiles.HideSelection = False
        Me.ListViewFiles.Location = New System.Drawing.Point(0, 0)
        Me.ListViewFiles.Name = "ListViewFiles"
        Me.ListViewFiles.Size = New System.Drawing.Size(678, 464)
        Me.ListViewFiles.TabIndex = 1
        Me.ListViewFiles.UseCompatibleStateImageBehavior = False
        '
        'PanelOverlay
        '
        Me.PanelOverlay.BackColor = System.Drawing.SystemColors.Window
        Me.PanelOverlay.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.[Single]
        Me.PanelOverlay.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.PanelOverlay.Controls.Add(Me.PanelOverlayTopZone, 0, 0)
        Me.PanelOverlay.Controls.Add(Me.PanelOverlayBottomZone, 0, 1)
        Me.PanelOverlay.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelOverlay.Location = New System.Drawing.Point(0, 0)
        Me.PanelOverlay.Name = "PanelOverlay"
        Me.PanelOverlay.RowCount = 2
        Me.PanelOverlay.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.PanelOverlay.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.PanelOverlay.Size = New System.Drawing.Size(678, 464)
        Me.PanelOverlay.TabIndex = 2
        Me.PanelOverlay.Visible = False
        '
        'PanelOverlayTopZone
        '
        Me.PanelOverlayTopZone.Controls.Add(Me.LabelOpenImages)
        Me.PanelOverlayTopZone.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelOverlayTopZone.Location = New System.Drawing.Point(1, 1)
        Me.PanelOverlayTopZone.Margin = New System.Windows.Forms.Padding(0)
        Me.PanelOverlayTopZone.Name = "PanelOverlayTopZone"
        Me.PanelOverlayTopZone.Size = New System.Drawing.Size(676, 230)
        Me.PanelOverlayTopZone.TabIndex = 0
        '
        'LabelOpenImages
        '
        Me.LabelOpenImages.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.LabelOpenImages.AutoSize = True
        Me.LabelOpenImages.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold)
        Me.LabelOpenImages.Location = New System.Drawing.Point(273, 102)
        Me.LabelOpenImages.Name = "LabelOpenImages"
        Me.LabelOpenImages.Size = New System.Drawing.Size(194, 24)
        Me.LabelOpenImages.TabIndex = 0
        Me.LabelOpenImages.Text = "{Open Disk Images}"
        '
        'PanelOverlayBottomZone
        '
        Me.PanelOverlayBottomZone.Controls.Add(Me.LabelImportFiles)
        Me.PanelOverlayBottomZone.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelOverlayBottomZone.Location = New System.Drawing.Point(1, 232)
        Me.PanelOverlayBottomZone.Margin = New System.Windows.Forms.Padding(0)
        Me.PanelOverlayBottomZone.Name = "PanelOverlayBottomZone"
        Me.PanelOverlayBottomZone.Size = New System.Drawing.Size(676, 231)
        Me.PanelOverlayBottomZone.TabIndex = 1
        '
        'LabelImportFiles
        '
        Me.LabelImportFiles.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.LabelImportFiles.AutoSize = True
        Me.LabelImportFiles.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold)
        Me.LabelImportFiles.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LabelImportFiles.Location = New System.Drawing.Point(296, 109)
        Me.LabelImportFiles.Name = "LabelImportFiles"
        Me.LabelImportFiles.Size = New System.Drawing.Size(133, 24)
        Me.LabelImportFiles.TabIndex = 1
        Me.LabelImportFiles.Text = "{Import Files}"
        '
        'PanelSpacer2
        '
        PanelSpacer2.Dock = System.Windows.Forms.DockStyle.Top
        PanelSpacer2.Location = New System.Drawing.Point(0, 27)
        PanelSpacer2.Name = "PanelSpacer2"
        PanelSpacer2.Size = New System.Drawing.Size(678, 6)
        PanelSpacer2.TabIndex = 2
        '
        'PanelCombo
        '
        PanelCombo.Controls.Add(Me.ComboImages)
        PanelCombo.Controls.Add(PanelSpacer1)
        PanelCombo.Controls.Add(Me.BtnResetSort)
        PanelCombo.Dock = System.Windows.Forms.DockStyle.Top
        PanelCombo.Location = New System.Drawing.Point(0, 6)
        PanelCombo.Name = "PanelCombo"
        PanelCombo.Size = New System.Drawing.Size(678, 21)
        PanelCombo.TabIndex = 1
        '
        'ComboImages
        '
        Me.ComboImages.AllowDrop = True
        Me.ComboImages.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ComboImages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboImages.DropDownWidth = 523
        Me.ComboImages.Location = New System.Drawing.Point(0, 0)
        Me.ComboImages.Name = "ComboImages"
        Me.ComboImages.Size = New System.Drawing.Size(597, 21)
        Me.ComboImages.Sorted = True
        Me.ComboImages.TabIndex = 0
        Me.ComboImages.Visible = False
        '
        'PanelSpacer1
        '
        PanelSpacer1.Dock = System.Windows.Forms.DockStyle.Right
        PanelSpacer1.Location = New System.Drawing.Point(597, 0)
        PanelSpacer1.Name = "PanelSpacer1"
        PanelSpacer1.Size = New System.Drawing.Size(6, 21)
        PanelSpacer1.TabIndex = 1
        '
        'BtnResetSort
        '
        Me.BtnResetSort.AutoSize = True
        Me.BtnResetSort.Dock = System.Windows.Forms.DockStyle.Right
        Me.BtnResetSort.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.BtnResetSort.Location = New System.Drawing.Point(603, 0)
        Me.BtnResetSort.MaximumSize = New System.Drawing.Size(0, 21)
        Me.BtnResetSort.Name = "BtnResetSort"
        Me.BtnResetSort.Size = New System.Drawing.Size(75, 21)
        Me.BtnResetSort.TabIndex = 2
        Me.BtnResetSort.Text = "{Reset Sort}"
        Me.BtnResetSort.UseVisualStyleBackColor = True
        '
        'HashValue
        '
        HashValue.Width = -1
        '
        'MenuFileSeparator1
        '
        MenuFileSeparator1.Name = "MenuFileSeparator1"
        MenuFileSeparator1.Size = New System.Drawing.Size(210, 6)
        '
        'MenuFileSeparator2
        '
        MenuFileSeparator2.Name = "MenuFileSeparator2"
        MenuFileSeparator2.Size = New System.Drawing.Size(210, 6)
        '
        'MenuFileSeparator3
        '
        MenuFileSeparator3.Name = "MenuFileSeparator3"
        MenuFileSeparator3.Size = New System.Drawing.Size(210, 6)
        '
        'MenuEditSeparator1
        '
        MenuEditSeparator1.Name = "MenuEditSeparator1"
        MenuEditSeparator1.Size = New System.Drawing.Size(185, 6)
        '
        'MenuEditSeparatorImage
        '
        Me.MenuEditSeparatorImage.Name = "MenuEditSeparatorImage"
        Me.MenuEditSeparatorImage.Size = New System.Drawing.Size(185, 6)
        '
        'MenuEditSeparator2
        '
        MenuEditSeparator2.Name = "MenuEditSeparator2"
        MenuEditSeparator2.Size = New System.Drawing.Size(185, 6)
        '
        'MenuToolsSeparator
        '
        MenuToolsSeparator.Name = "MenuToolsSeparator"
        MenuToolsSeparator.Size = New System.Drawing.Size(294, 6)
        '
        'MenuHelpSeparator
        '
        MenuHelpSeparator.Name = "MenuHelpSeparator"
        MenuHelpSeparator.Size = New System.Drawing.Size(171, 6)
        '
        'ToolStripSeparator6
        '
        ToolStripSeparator6.Name = "ToolStripSeparator6"
        ToolStripSeparator6.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        ToolStripSeparator6.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripSeparator7
        '
        ToolStripSeparator7.Name = "ToolStripSeparator7"
        ToolStripSeparator7.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        ToolStripSeparator7.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripSeparator8
        '
        ToolStripSeparator8.Name = "ToolStripSeparator8"
        ToolStripSeparator8.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
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
        MenuDiskSeparator.Size = New System.Drawing.Size(186, 6)
        '
        'ToolStripSeparator9
        '
        ToolStripSeparator9.Name = "ToolStripSeparator9"
        ToolStripSeparator9.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        ToolStripSeparator9.Size = New System.Drawing.Size(6, 25)
        '
        'MainMenuView
        '
        Me.MainMenuView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuHexBootSector, Me.MenuHexFAT, Me.MenuHexDirectory, Me.MenuHexFreeClusters, Me.MenuHexBadSectors, Me.MenuHexLostClusters, Me.MenuHexOverdumpData, Me.MenuHexRawTrackData, Me.MenuHexDisk, Me.MenuHexSeparatorFile, Me.MenuHexFile})
        Me.MainMenuView.Name = "MainMenuView"
        Me.MainMenuView.Size = New System.Drawing.Size(47, 20)
        Me.MainMenuView.Text = "{&Hex}"
        '
        'MenuHexBootSector
        '
        Me.MenuHexBootSector.Name = "MenuHexBootSector"
        Me.MenuHexBootSector.Size = New System.Drawing.Size(202, 22)
        Me.MenuHexBootSector.Text = "{&Boot Sector}"
        '
        'MenuHexFAT
        '
        Me.MenuHexFAT.Name = "MenuHexFAT"
        Me.MenuHexFAT.Size = New System.Drawing.Size(202, 22)
        Me.MenuHexFAT.Text = "{File &Allocation Table}"
        '
        'MenuHexDirectory
        '
        Me.MenuHexDirectory.Name = "MenuHexDirectory"
        Me.MenuHexDirectory.Size = New System.Drawing.Size(202, 22)
        Me.MenuHexDirectory.Text = "{RootDirectory}"
        '
        'MenuHexFreeClusters
        '
        Me.MenuHexFreeClusters.Name = "MenuHexFreeClusters"
        Me.MenuHexFreeClusters.Size = New System.Drawing.Size(202, 22)
        Me.MenuHexFreeClusters.Text = "{Free &Clusters with Data}"
        '
        'MenuHexBadSectors
        '
        Me.MenuHexBadSectors.Name = "MenuHexBadSectors"
        Me.MenuHexBadSectors.Size = New System.Drawing.Size(202, 22)
        Me.MenuHexBadSectors.Text = "{Bad &Sectors}"
        '
        'MenuHexLostClusters
        '
        Me.MenuHexLostClusters.Name = "MenuHexLostClusters"
        Me.MenuHexLostClusters.Size = New System.Drawing.Size(202, 22)
        Me.MenuHexLostClusters.Text = "{&Lost Clusters}"
        '
        'MenuHexOverdumpData
        '
        Me.MenuHexOverdumpData.Name = "MenuHexOverdumpData"
        Me.MenuHexOverdumpData.Size = New System.Drawing.Size(202, 22)
        Me.MenuHexOverdumpData.Text = "{&Overdump Data}"
        '
        'MenuHexRawTrackData
        '
        Me.MenuHexRawTrackData.Name = "MenuHexRawTrackData"
        Me.MenuHexRawTrackData.Size = New System.Drawing.Size(202, 22)
        Me.MenuHexRawTrackData.Text = "{&Raw Track Data}"
        '
        'MenuHexDisk
        '
        Me.MenuHexDisk.Name = "MenuHexDisk"
        Me.MenuHexDisk.Size = New System.Drawing.Size(202, 22)
        Me.MenuHexDisk.Text = "{Entire &Disk}"
        '
        'MenuHexSeparatorFile
        '
        Me.MenuHexSeparatorFile.Name = "MenuHexSeparatorFile"
        Me.MenuHexSeparatorFile.Size = New System.Drawing.Size(199, 6)
        '
        'MenuHexFile
        '
        Me.MenuHexFile.Name = "MenuHexFile"
        Me.MenuHexFile.Size = New System.Drawing.Size(202, 22)
        Me.MenuHexFile.Text = "{File}"
        '
        'MainMenuTools
        '
        Me.MainMenuTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuToolsWin9xClean, Me.MenuToolsClearReservedBytes, Me.MenuToolsFixImageSize, Me.MenuToolsFixImageSizeSubMenu, Me.MenuToolsRestoreBootSector, Me.MenuToolsRemoveBootSector, MenuToolsSeparator, Me.MenuToolsWin9xCleanBatch})
        Me.MainMenuTools.Name = "MainMenuTools"
        Me.MainMenuTools.Size = New System.Drawing.Size(55, 20)
        Me.MainMenuTools.Text = "{&Tools}"
        '
        'MenuToolsWin9xClean
        '
        Me.MenuToolsWin9xClean.Name = "MenuToolsWin9xClean"
        Me.MenuToolsWin9xClean.Size = New System.Drawing.Size(297, 22)
        Me.MenuToolsWin9xClean.Text = "{Remove &Windows Modifications}"
        '
        'MenuToolsClearReservedBytes
        '
        Me.MenuToolsClearReservedBytes.Name = "MenuToolsClearReservedBytes"
        Me.MenuToolsClearReservedBytes.Size = New System.Drawing.Size(297, 22)
        Me.MenuToolsClearReservedBytes.Text = "{Clear &Reserved Bytes}"
        '
        'MenuToolsFixImageSize
        '
        Me.MenuToolsFixImageSize.Name = "MenuToolsFixImageSize"
        Me.MenuToolsFixImageSize.Size = New System.Drawing.Size(297, 22)
        Me.MenuToolsFixImageSize.Text = "{TruncateImage}"
        '
        'MenuToolsFixImageSizeSubMenu
        '
        Me.MenuToolsFixImageSizeSubMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuToolsTruncateImage, Me.MenuToolsRestructureImage})
        Me.MenuToolsFixImageSizeSubMenu.Name = "MenuToolsFixImageSizeSubMenu"
        Me.MenuToolsFixImageSizeSubMenu.Size = New System.Drawing.Size(297, 22)
        Me.MenuToolsFixImageSizeSubMenu.Text = "{Fix Image &Size}"
        '
        'MenuToolsTruncateImage
        '
        Me.MenuToolsTruncateImage.Name = "MenuToolsTruncateImage"
        Me.MenuToolsTruncateImage.Size = New System.Drawing.Size(178, 22)
        Me.MenuToolsTruncateImage.Text = "{&Truncate Image}"
        '
        'MenuToolsRestructureImage
        '
        Me.MenuToolsRestructureImage.Name = "MenuToolsRestructureImage"
        Me.MenuToolsRestructureImage.Size = New System.Drawing.Size(178, 22)
        Me.MenuToolsRestructureImage.Text = "{&Restructure Image}"
        '
        'MenuToolsRestoreBootSector
        '
        Me.MenuToolsRestoreBootSector.Name = "MenuToolsRestoreBootSector"
        Me.MenuToolsRestoreBootSector.Size = New System.Drawing.Size(297, 22)
        Me.MenuToolsRestoreBootSector.Text = "{Restore &Boot Sector from Root Directory}"
        '
        'MenuToolsRemoveBootSector
        '
        Me.MenuToolsRemoveBootSector.Name = "MenuToolsRemoveBootSector"
        Me.MenuToolsRemoveBootSector.Size = New System.Drawing.Size(297, 22)
        Me.MenuToolsRemoveBootSector.Text = "{Remove &Boot Sector from Root Directory}"
        '
        'MenuToolsWin9xCleanBatch
        '
        Me.MenuToolsWin9xCleanBatch.Name = "MenuToolsWin9xCleanBatch"
        Me.MenuToolsWin9xCleanBatch.Size = New System.Drawing.Size(297, 22)
        Me.MenuToolsWin9xCleanBatch.Text = "{Batch Remove Windows Modifications}"
        '
        'MainMenuDisk
        '
        Me.MainMenuDisk.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuDiskReadFloppyA, Me.MenuDiskReadFloppyB, MenuDiskSeparator, Me.MenuDiskWriteFloppyA, Me.MenuDiskWriteFloppyB})
        Me.MainMenuDisk.Name = "MainMenuDisk"
        Me.MainMenuDisk.Size = New System.Drawing.Size(49, 20)
        Me.MainMenuDisk.Text = "{&Disk}"
        '
        'MenuDiskReadFloppyA
        '
        Me.MenuDiskReadFloppyA.Name = "MenuDiskReadFloppyA"
        Me.MenuDiskReadFloppyA.Size = New System.Drawing.Size(189, 22)
        Me.MenuDiskReadFloppyA.Text = "{&Read Disk in Drive A}"
        '
        'MenuDiskReadFloppyB
        '
        Me.MenuDiskReadFloppyB.Name = "MenuDiskReadFloppyB"
        Me.MenuDiskReadFloppyB.Size = New System.Drawing.Size(189, 22)
        Me.MenuDiskReadFloppyB.Text = "{&Read Disk in Drive B}"
        '
        'MenuDiskWriteFloppyA
        '
        Me.MenuDiskWriteFloppyA.Name = "MenuDiskWriteFloppyA"
        Me.MenuDiskWriteFloppyA.Size = New System.Drawing.Size(189, 22)
        Me.MenuDiskWriteFloppyA.Text = "{&Write Disk in Drive A}"
        '
        'MenuDiskWriteFloppyB
        '
        Me.MenuDiskWriteFloppyB.Name = "MenuDiskWriteFloppyB"
        Me.MenuDiskWriteFloppyB.Size = New System.Drawing.Size(189, 22)
        Me.MenuDiskWriteFloppyB.Text = "{&Write Disk in Drive B}"
        '
        'MenuStripTop
        '
        Me.MenuStripTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MainMenuFile, Me.MainMenuEdit, Me.MainMenuFilters, Me.MainMenuView, Me.MainMenuTools, Me.MainMenuDisk, Me.MainMenuFlux, Me.MainMenuReports, Me.MainMenuOptions, Me.MainMenuHelp, Me.MainMenuUpdateAvailable, Me.MainMenuNewInstance})
        Me.MenuStripTop.Location = New System.Drawing.Point(0, 0)
        Me.MenuStripTop.Name = "MenuStripTop"
        Me.MenuStripTop.Padding = New System.Windows.Forms.Padding(6, 2, 12, 2)
        Me.MenuStripTop.ShowItemToolTips = True
        Me.MenuStripTop.Size = New System.Drawing.Size(1004, 24)
        Me.MenuStripTop.TabIndex = 0
        '
        'MainMenuFile
        '
        Me.MainMenuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuFileOpen, Me.MenuFileRecent, Me.MenuFileReload, Me.MenuFileNewImage, MenuFileSeparator1, Me.MenuFileSave, Me.MenuFileSaveAs, Me.MenuFileSaveAll, MenuFileSeparator2, Me.MenuFileClose, Me.MenuFileCloseAll, MenuFileSeparator3, Me.MenuFileExit})
        Me.MainMenuFile.Name = "MainMenuFile"
        Me.MainMenuFile.Size = New System.Drawing.Size(45, 20)
        Me.MainMenuFile.Text = "{&File}"
        '
        'MenuFileOpen
        '
        Me.MenuFileOpen.Image = Global.DiskImageTool.My.Resources.Resources.OpenfileDialog
        Me.MenuFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.MenuFileOpen.Name = "MenuFileOpen"
        Me.MenuFileOpen.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.MenuFileOpen.Size = New System.Drawing.Size(213, 22)
        Me.MenuFileOpen.Text = "{&Open}"
        '
        'MenuFileRecent
        '
        Me.MenuFileRecent.Name = "MenuFileRecent"
        Me.MenuFileRecent.Size = New System.Drawing.Size(213, 22)
        Me.MenuFileRecent.Text = "{Recent}"
        '
        'MenuFileReload
        '
        Me.MenuFileReload.Image = Global.DiskImageTool.My.Resources.Resources.Refresh
        Me.MenuFileReload.Name = "MenuFileReload"
        Me.MenuFileReload.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
        Me.MenuFileReload.Size = New System.Drawing.Size(213, 22)
        Me.MenuFileReload.Text = "{&Reload from Disk}"
        '
        'MenuFileNewImage
        '
        Me.MenuFileNewImage.Image = Global.DiskImageTool.My.Resources.Resources.NewDocument
        Me.MenuFileNewImage.Name = "MenuFileNewImage"
        Me.MenuFileNewImage.Size = New System.Drawing.Size(213, 22)
        Me.MenuFileNewImage.Text = "{&New Image}"
        '
        'MenuFileSave
        '
        Me.MenuFileSave.Image = Global.DiskImageTool.My.Resources.Resources.Save
        Me.MenuFileSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.MenuFileSave.Name = "MenuFileSave"
        Me.MenuFileSave.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.MenuFileSave.Size = New System.Drawing.Size(213, 22)
        Me.MenuFileSave.Text = "{&Save}"
        '
        'MenuFileSaveAs
        '
        Me.MenuFileSaveAs.Image = Global.DiskImageTool.My.Resources.Resources.SaveAs
        Me.MenuFileSaveAs.Name = "MenuFileSaveAs"
        Me.MenuFileSaveAs.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Alt) _
            Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.MenuFileSaveAs.Size = New System.Drawing.Size(213, 22)
        Me.MenuFileSaveAs.Text = "{Save &As}"
        '
        'MenuFileSaveAll
        '
        Me.MenuFileSaveAll.Image = Global.DiskImageTool.My.Resources.Resources.SaveAll
        Me.MenuFileSaveAll.Name = "MenuFileSaveAll"
        Me.MenuFileSaveAll.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.MenuFileSaveAll.Size = New System.Drawing.Size(213, 22)
        Me.MenuFileSaveAll.Text = "{Save All}"
        '
        'MenuFileClose
        '
        Me.MenuFileClose.Image = Global.DiskImageTool.My.Resources.Resources.Close
        Me.MenuFileClose.Name = "MenuFileClose"
        Me.MenuFileClose.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.W), System.Windows.Forms.Keys)
        Me.MenuFileClose.Size = New System.Drawing.Size(213, 22)
        Me.MenuFileClose.Text = "{&Close}"
        '
        'MenuFileCloseAll
        '
        Me.MenuFileCloseAll.Image = Global.DiskImageTool.My.Resources.Resources.CloseAll
        Me.MenuFileCloseAll.Name = "MenuFileCloseAll"
        Me.MenuFileCloseAll.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.W), System.Windows.Forms.Keys)
        Me.MenuFileCloseAll.Size = New System.Drawing.Size(213, 22)
        Me.MenuFileCloseAll.Text = "{Close All}"
        '
        'MenuFileExit
        '
        Me.MenuFileExit.Image = Global.DiskImageTool.My.Resources.Resources._Exit
        Me.MenuFileExit.Name = "MenuFileExit"
        Me.MenuFileExit.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.F4), System.Windows.Forms.Keys)
        Me.MenuFileExit.Size = New System.Drawing.Size(213, 22)
        Me.MenuFileExit.Text = "{E&xit}"
        '
        'MainMenuEdit
        '
        Me.MainMenuEdit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuEditBootSector, Me.MenuEditFAT, MenuEditSeparator1, Me.MenuEditImageProperties, Me.MenuEditSeparatorImage, Me.MenuEditFileProperties, Me.MenuEditExportFile, Me.MenuEditReplaceFile, MenuEditSeparator2, Me.MenuEditImportFiles, Me.ToolStripSeparator1, Me.MenuEditUndo, Me.MenuEditRedo, Me.MenuEditRevert})
        Me.MainMenuEdit.Name = "MainMenuEdit"
        Me.MainMenuEdit.Size = New System.Drawing.Size(47, 20)
        Me.MainMenuEdit.Text = "{&Edit}"
        '
        'MenuEditBootSector
        '
        Me.MenuEditBootSector.Name = "MenuEditBootSector"
        Me.MenuEditBootSector.Size = New System.Drawing.Size(188, 22)
        Me.MenuEditBootSector.Text = "{&Boot Sector}"
        '
        'MenuEditFAT
        '
        Me.MenuEditFAT.Name = "MenuEditFAT"
        Me.MenuEditFAT.Size = New System.Drawing.Size(188, 22)
        Me.MenuEditFAT.Text = "{File &Allocation Table}"
        '
        'MenuEditImageProperties
        '
        Me.MenuEditImageProperties.Name = "MenuEditImageProperties"
        Me.MenuEditImageProperties.Size = New System.Drawing.Size(188, 22)
        Me.MenuEditImageProperties.Text = "{I&mage Properties}"
        '
        'MenuEditFileProperties
        '
        Me.MenuEditFileProperties.Image = Global.DiskImageTool.My.Resources.Resources.PropertiesFolderClosed
        Me.MenuEditFileProperties.Name = "MenuEditFileProperties"
        Me.MenuEditFileProperties.Size = New System.Drawing.Size(188, 22)
        Me.MenuEditFileProperties.Text = "{File &Properties}"
        '
        'MenuEditExportFile
        '
        Me.MenuEditExportFile.Image = Global.DiskImageTool.My.Resources.Resources.Export
        Me.MenuEditExportFile.Name = "MenuEditExportFile"
        Me.MenuEditExportFile.Size = New System.Drawing.Size(188, 22)
        Me.MenuEditExportFile.Text = "{ExportFile}"
        '
        'MenuEditReplaceFile
        '
        Me.MenuEditReplaceFile.Image = Global.DiskImageTool.My.Resources.Resources.SwitchFolders
        Me.MenuEditReplaceFile.Name = "MenuEditReplaceFile"
        Me.MenuEditReplaceFile.Size = New System.Drawing.Size(188, 22)
        Me.MenuEditReplaceFile.Text = "{&Replace File}"
        '
        'MenuEditImportFiles
        '
        Me.MenuEditImportFiles.Image = Global.DiskImageTool.My.Resources.Resources.Import
        Me.MenuEditImportFiles.Name = "MenuEditImportFiles"
        Me.MenuEditImportFiles.Size = New System.Drawing.Size(188, 22)
        Me.MenuEditImportFiles.Text = "{&Import Files}"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(185, 6)
        '
        'MenuEditUndo
        '
        Me.MenuEditUndo.Image = Global.DiskImageTool.My.Resources.Resources.Undo
        Me.MenuEditUndo.Name = "MenuEditUndo"
        Me.MenuEditUndo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.MenuEditUndo.Size = New System.Drawing.Size(188, 22)
        Me.MenuEditUndo.Text = "{&Undo}"
        '
        'MenuEditRedo
        '
        Me.MenuEditRedo.Image = Global.DiskImageTool.My.Resources.Resources.Redo
        Me.MenuEditRedo.Name = "MenuEditRedo"
        Me.MenuEditRedo.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.MenuEditRedo.Size = New System.Drawing.Size(188, 22)
        Me.MenuEditRedo.Text = "{&Redo}"
        '
        'MenuEditRevert
        '
        Me.MenuEditRevert.Name = "MenuEditRevert"
        Me.MenuEditRevert.Size = New System.Drawing.Size(188, 22)
        Me.MenuEditRevert.Text = "{&Revert}"
        Me.MenuEditRevert.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MainMenuFilters
        '
        Me.MainMenuFilters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.MainMenuFilters.DropDown = Me.ContextMenuFilters
        Me.MainMenuFilters.Name = "MainMenuFilters"
        Me.MainMenuFilters.Size = New System.Drawing.Size(58, 20)
        Me.MainMenuFilters.Text = "{F&ilters}"
        '
        'ContextMenuFilters
        '
        Me.ContextMenuFilters.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuFiltersScanNew, Me.MenuFiltersScan, Me.MenuFiltersClear})
        Me.ContextMenuFilters.Name = "ContextMenuStrip1"
        Me.ContextMenuFilters.Size = New System.Drawing.Size(176, 70)
        '
        'MenuFiltersScanNew
        '
        Me.MenuFiltersScanNew.Name = "MenuFiltersScanNew"
        Me.MenuFiltersScanNew.Size = New System.Drawing.Size(175, 22)
        Me.MenuFiltersScanNew.Text = "{Scan &New Images}"
        '
        'MenuFiltersScan
        '
        Me.MenuFiltersScan.Name = "MenuFiltersScan"
        Me.MenuFiltersScan.Size = New System.Drawing.Size(175, 22)
        Me.MenuFiltersScan.Text = "{ScanImages}"
        '
        'MenuFiltersClear
        '
        Me.MenuFiltersClear.Name = "MenuFiltersClear"
        Me.MenuFiltersClear.Size = New System.Drawing.Size(175, 22)
        Me.MenuFiltersClear.Text = "{Clear Filters}"
        '
        'MainMenuFlux
        '
        Me.MainMenuFlux.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuGreaseweazleRead, Me.MenuGreaseweazleWrite, Me.ToolStripSeparator5, Me.MenuFluxConvert, Me.ToolStripSeparatorDevices, Me.MenuGreaseweazle, Me.MenuPcImgCnv})
        Me.MainMenuFlux.Name = "MainMenuFlux"
        Me.MainMenuFlux.Size = New System.Drawing.Size(40, 20)
        Me.MainMenuFlux.Text = "Flu&x"
        '
        'MenuGreaseweazleRead
        '
        Me.MenuGreaseweazleRead.Name = "MenuGreaseweazleRead"
        Me.MenuGreaseweazleRead.Size = New System.Drawing.Size(184, 22)
        Me.MenuGreaseweazleRead.Text = "{&Read Disk}"
        '
        'MenuGreaseweazleWrite
        '
        Me.MenuGreaseweazleWrite.Name = "MenuGreaseweazleWrite"
        Me.MenuGreaseweazleWrite.Size = New System.Drawing.Size(184, 22)
        Me.MenuGreaseweazleWrite.Text = "{&Write Disk}"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(181, 6)
        '
        'MenuFluxConvert
        '
        Me.MenuFluxConvert.Name = "MenuFluxConvert"
        Me.MenuFluxConvert.Size = New System.Drawing.Size(184, 22)
        Me.MenuFluxConvert.Text = "{&Convert Flux Image}"
        '
        'ToolStripSeparatorDevices
        '
        Me.ToolStripSeparatorDevices.Name = "ToolStripSeparatorDevices"
        Me.ToolStripSeparatorDevices.Size = New System.Drawing.Size(181, 6)
        '
        'MenuGreaseweazle
        '
        Me.MenuGreaseweazle.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuGreaseweazleErase, Me.MenuGreaseweazleClean, Me.ToolStripSeparator12, Me.MenuGreaseweazleInfo, Me.MenuGreaseweazleBandwidth})
        Me.MenuGreaseweazle.Name = "MenuGreaseweazle"
        Me.MenuGreaseweazle.Size = New System.Drawing.Size(184, 22)
        Me.MenuGreaseweazle.Text = "&Greaseweazle"
        '
        'MenuGreaseweazleErase
        '
        Me.MenuGreaseweazleErase.Name = "MenuGreaseweazleErase"
        Me.MenuGreaseweazleErase.Size = New System.Drawing.Size(177, 22)
        Me.MenuGreaseweazleErase.Text = "{&Erase Disk}"
        '
        'MenuGreaseweazleClean
        '
        Me.MenuGreaseweazleClean.Name = "MenuGreaseweazleClean"
        Me.MenuGreaseweazleClean.Size = New System.Drawing.Size(177, 22)
        Me.MenuGreaseweazleClean.Text = "{&Clean Drive}"
        '
        'ToolStripSeparator12
        '
        Me.ToolStripSeparator12.Name = "ToolStripSeparator12"
        Me.ToolStripSeparator12.Size = New System.Drawing.Size(174, 6)
        '
        'MenuGreaseweazleInfo
        '
        Me.MenuGreaseweazleInfo.Name = "MenuGreaseweazleInfo"
        Me.MenuGreaseweazleInfo.Size = New System.Drawing.Size(177, 22)
        Me.MenuGreaseweazleInfo.Text = "{&Device Info}"
        '
        'MenuGreaseweazleBandwidth
        '
        Me.MenuGreaseweazleBandwidth.Name = "MenuGreaseweazleBandwidth"
        Me.MenuGreaseweazleBandwidth.Size = New System.Drawing.Size(177, 22)
        Me.MenuGreaseweazleBandwidth.Text = "{Report &Bandwidth}"
        '
        'MenuPcImgCnv
        '
        Me.MenuPcImgCnv.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuPcImgCnvTrackLayout})
        Me.MenuPcImgCnv.Name = "MenuPcImgCnv"
        Me.MenuPcImgCnv.Size = New System.Drawing.Size(184, 22)
        Me.MenuPcImgCnv.Text = "PcImgCnv"
        '
        'MenuPcImgCnvTrackLayout
        '
        Me.MenuPcImgCnvTrackLayout.Name = "MenuPcImgCnvTrackLayout"
        Me.MenuPcImgCnvTrackLayout.Size = New System.Drawing.Size(207, 22)
        Me.MenuPcImgCnvTrackLayout.Text = "{Generate tracklayout.txt}"
        '
        'MainMenuReports
        '
        Me.MainMenuReports.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuReportsModifications, Me.MenuReportsImageAnalysis, Me.MenuReportsBatchImageAnalysis})
        Me.MainMenuReports.Name = "MainMenuReports"
        Me.MainMenuReports.Size = New System.Drawing.Size(67, 20)
        Me.MainMenuReports.Text = "{&Reports}"
        '
        'MenuReportsModifications
        '
        Me.MenuReportsModifications.Name = "MenuReportsModifications"
        Me.MenuReportsModifications.Size = New System.Drawing.Size(194, 22)
        Me.MenuReportsModifications.Text = "{Modifications}"
        '
        'MenuReportsImageAnalysis
        '
        Me.MenuReportsImageAnalysis.Name = "MenuReportsImageAnalysis"
        Me.MenuReportsImageAnalysis.Size = New System.Drawing.Size(194, 22)
        Me.MenuReportsImageAnalysis.Text = "{Image Analysis}"
        '
        'MenuReportsBatchImageAnalysis
        '
        Me.MenuReportsBatchImageAnalysis.Name = "MenuReportsBatchImageAnalysis"
        Me.MenuReportsBatchImageAnalysis.Size = New System.Drawing.Size(194, 22)
        Me.MenuReportsBatchImageAnalysis.Text = "{Batch Image Analysis}"
        '
        'MainMenuOptions
        '
        Me.MainMenuOptions.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuOptionsCreateBackup, Me.MenuOptionsCheckUpdate, Me.MenuOptionsDragDrop, Me.MenuOptionsDisplayTitles, Me.MenuOptionsDisplayLanguage, Me.ToolStripSeparator2, Me.MenuOptionsFlux})
        Me.MainMenuOptions.Name = "MainMenuOptions"
        Me.MainMenuOptions.Size = New System.Drawing.Size(69, 20)
        Me.MainMenuOptions.Text = "{&Options}"
        '
        'MenuOptionsCreateBackup
        '
        Me.MenuOptionsCreateBackup.CheckOnClick = True
        Me.MenuOptionsCreateBackup.Name = "MenuOptionsCreateBackup"
        Me.MenuOptionsCreateBackup.Size = New System.Drawing.Size(232, 22)
        Me.MenuOptionsCreateBackup.Text = "{Create Backup on Save}"
        '
        'MenuOptionsCheckUpdate
        '
        Me.MenuOptionsCheckUpdate.CheckOnClick = True
        Me.MenuOptionsCheckUpdate.Name = "MenuOptionsCheckUpdate"
        Me.MenuOptionsCheckUpdate.Size = New System.Drawing.Size(232, 22)
        Me.MenuOptionsCheckUpdate.Text = "{Check for Update on Startup}"
        '
        'MenuOptionsDragDrop
        '
        Me.MenuOptionsDragDrop.CheckOnClick = True
        Me.MenuOptionsDragDrop.Name = "MenuOptionsDragDrop"
        Me.MenuOptionsDragDrop.Size = New System.Drawing.Size(232, 22)
        Me.MenuOptionsDragDrop.Text = "{Import using Drag and Drop}"
        '
        'MenuOptionsDisplayTitles
        '
        Me.MenuOptionsDisplayTitles.CheckOnClick = True
        Me.MenuOptionsDisplayTitles.Name = "MenuOptionsDisplayTitles"
        Me.MenuOptionsDisplayTitles.Size = New System.Drawing.Size(232, 22)
        Me.MenuOptionsDisplayTitles.Text = "{Display Titles}"
        '
        'MenuOptionsDisplayLanguage
        '
        Me.MenuOptionsDisplayLanguage.Name = "MenuOptionsDisplayLanguage"
        Me.MenuOptionsDisplayLanguage.Size = New System.Drawing.Size(232, 22)
        Me.MenuOptionsDisplayLanguage.Text = "{Language}"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(229, 6)
        '
        'MenuOptionsFlux
        '
        Me.MenuOptionsFlux.Name = "MenuOptionsFlux"
        Me.MenuOptionsFlux.Size = New System.Drawing.Size(232, 22)
        Me.MenuOptionsFlux.Text = "{&Flux Configuration}"
        '
        'MainMenuHelp
        '
        Me.MainMenuHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MenuHelpProjectPage, Me.MenuHelpDocs, Me.MenuHelpUpdateCheck, Me.MenuHelpChangeLog, MenuHelpSeparator, Me.MenuHelpAbout})
        Me.MainMenuHelp.Name = "MainMenuHelp"
        Me.MainMenuHelp.Size = New System.Drawing.Size(24, 20)
        Me.MainMenuHelp.Text = "?"
        '
        'MenuHelpProjectPage
        '
        Me.MenuHelpProjectPage.Image = Global.DiskImageTool.My.Resources.Resources.Web
        Me.MenuHelpProjectPage.Name = "MenuHelpProjectPage"
        Me.MenuHelpProjectPage.Size = New System.Drawing.Size(174, 22)
        Me.MenuHelpProjectPage.Text = "{&Project Page}"
        '
        'MenuHelpDocs
        '
        Me.MenuHelpDocs.Image = Global.DiskImageTool.My.Resources.Resources.HelpTableOfContents
        Me.MenuHelpDocs.Name = "MenuHelpDocs"
        Me.MenuHelpDocs.Size = New System.Drawing.Size(174, 22)
        Me.MenuHelpDocs.Text = "{&Documentation}"
        '
        'MenuHelpUpdateCheck
        '
        Me.MenuHelpUpdateCheck.Image = Global.DiskImageTool.My.Resources.Resources.Refresh
        Me.MenuHelpUpdateCheck.Name = "MenuHelpUpdateCheck"
        Me.MenuHelpUpdateCheck.Size = New System.Drawing.Size(174, 22)
        Me.MenuHelpUpdateCheck.Text = "{Check for &Update}"
        '
        'MenuHelpChangeLog
        '
        Me.MenuHelpChangeLog.Image = Global.DiskImageTool.My.Resources.Resources.History
        Me.MenuHelpChangeLog.Name = "MenuHelpChangeLog"
        Me.MenuHelpChangeLog.Size = New System.Drawing.Size(174, 22)
        Me.MenuHelpChangeLog.Text = "{&Change Log}"
        '
        'MenuHelpAbout
        '
        Me.MenuHelpAbout.Image = Global.DiskImageTool.My.Resources.Resources.AboutBox
        Me.MenuHelpAbout.Name = "MenuHelpAbout"
        Me.MenuHelpAbout.ShortcutKeys = System.Windows.Forms.Keys.F1
        Me.MenuHelpAbout.Size = New System.Drawing.Size(174, 22)
        Me.MenuHelpAbout.Text = "{About}"
        '
        'MainMenuUpdateAvailable
        '
        Me.MainMenuUpdateAvailable.ForeColor = System.Drawing.Color.Blue
        Me.MainMenuUpdateAvailable.Margin = New System.Windows.Forms.Padding(12, 0, 0, 0)
        Me.MainMenuUpdateAvailable.Name = "MainMenuUpdateAvailable"
        Me.MainMenuUpdateAvailable.Size = New System.Drawing.Size(116, 20)
        Me.MainMenuUpdateAvailable.Text = "{Update Available}"
        '
        'MainMenuNewInstance
        '
        Me.MainMenuNewInstance.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.MainMenuNewInstance.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.MainMenuNewInstance.Image = Global.DiskImageTool.My.Resources.Resources.Instance
        Me.MainMenuNewInstance.Name = "MainMenuNewInstance"
        Me.MainMenuNewInstance.Size = New System.Drawing.Size(28, 20)
        Me.MainMenuNewInstance.Text = "{New Instance}"
        '
        'ToolStripTop
        '
        Me.ToolStripTop.CanOverflow = False
        Me.ToolStripTop.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStripTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripOpen, ToolStripSeparator6, Me.ToolStripSave, Me.ToolStripSaveAs, Me.ToolStripSaveAll, ToolStripSeparator7, Me.ToolStripClose, Me.ToolStripCloseAll, ToolStripSeparator8, Me.ToolStripFileProperties, Me.ToolStripExportFile, Me.ToolStripImportFiles, ToolStripSeparator9, Me.ToolStripUndo, Me.ToolStripRedo, ToolStripSeparator10, Me.ToolStripViewFileText, Me.ToolStripViewFile, Me.ToolStripSeparatorFAT})
        Me.ToolStripTop.Location = New System.Drawing.Point(0, 24)
        Me.ToolStripTop.Name = "ToolStripTop"
        Me.ToolStripTop.Padding = New System.Windows.Forms.Padding(12, 0, 12, 0)
        Me.ToolStripTop.Size = New System.Drawing.Size(1004, 25)
        Me.ToolStripTop.TabIndex = 1
        '
        'ToolStripOpen
        '
        Me.ToolStripOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripOpen.Image = Global.DiskImageTool.My.Resources.Resources.OpenfileDialog
        Me.ToolStripOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripOpen.Name = "ToolStripOpen"
        Me.ToolStripOpen.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripOpen.Size = New System.Drawing.Size(23, 22)
        '
        'ToolStripSave
        '
        Me.ToolStripSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripSave.Image = Global.DiskImageTool.My.Resources.Resources.Save
        Me.ToolStripSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSave.Name = "ToolStripSave"
        Me.ToolStripSave.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripSave.Size = New System.Drawing.Size(23, 22)
        '
        'ToolStripSaveAs
        '
        Me.ToolStripSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripSaveAs.Image = Global.DiskImageTool.My.Resources.Resources.SaveAs
        Me.ToolStripSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSaveAs.Name = "ToolStripSaveAs"
        Me.ToolStripSaveAs.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripSaveAs.Size = New System.Drawing.Size(23, 22)
        '
        'ToolStripSaveAll
        '
        Me.ToolStripSaveAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripSaveAll.Image = Global.DiskImageTool.My.Resources.Resources.SaveAll
        Me.ToolStripSaveAll.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSaveAll.Name = "ToolStripSaveAll"
        Me.ToolStripSaveAll.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripSaveAll.Size = New System.Drawing.Size(23, 22)
        '
        'ToolStripClose
        '
        Me.ToolStripClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripClose.Image = Global.DiskImageTool.My.Resources.Resources.Close
        Me.ToolStripClose.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripClose.Name = "ToolStripClose"
        Me.ToolStripClose.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripClose.Size = New System.Drawing.Size(23, 22)
        '
        'ToolStripCloseAll
        '
        Me.ToolStripCloseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripCloseAll.Image = Global.DiskImageTool.My.Resources.Resources.CloseAll
        Me.ToolStripCloseAll.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripCloseAll.Name = "ToolStripCloseAll"
        Me.ToolStripCloseAll.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripCloseAll.Size = New System.Drawing.Size(23, 22)
        '
        'ToolStripFileProperties
        '
        Me.ToolStripFileProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripFileProperties.Image = Global.DiskImageTool.My.Resources.Resources.PropertiesFolderClosed
        Me.ToolStripFileProperties.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripFileProperties.Name = "ToolStripFileProperties"
        Me.ToolStripFileProperties.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripFileProperties.Size = New System.Drawing.Size(23, 22)
        '
        'ToolStripExportFile
        '
        Me.ToolStripExportFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripExportFile.Image = Global.DiskImageTool.My.Resources.Resources.Export
        Me.ToolStripExportFile.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripExportFile.Name = "ToolStripExportFile"
        Me.ToolStripExportFile.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripExportFile.Size = New System.Drawing.Size(23, 22)
        '
        'ToolStripImportFiles
        '
        Me.ToolStripImportFiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripImportFiles.Image = Global.DiskImageTool.My.Resources.Resources.Import
        Me.ToolStripImportFiles.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripImportFiles.Name = "ToolStripImportFiles"
        Me.ToolStripImportFiles.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripImportFiles.ToolTipText = "{Import Files}"
        '
        'ToolStripUndo
        '
        Me.ToolStripUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripUndo.Image = Global.DiskImageTool.My.Resources.Resources.Undo
        Me.ToolStripUndo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripUndo.Name = "ToolStripUndo"
        Me.ToolStripUndo.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripUndo.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripUndo.Text = "{Undo}"
        '
        'ToolStripRedo
        '
        Me.ToolStripRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripRedo.Image = Global.DiskImageTool.My.Resources.Resources.Redo
        Me.ToolStripRedo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripRedo.Name = "ToolStripRedo"
        Me.ToolStripRedo.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripRedo.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripRedo.Text = "{Redo}"
        '
        'ToolStripViewFileText
        '
        Me.ToolStripViewFileText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripViewFileText.Image = Global.DiskImageTool.My.Resources.Resources.TextFile
        Me.ToolStripViewFileText.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripViewFileText.Name = "ToolStripViewFileText"
        Me.ToolStripViewFileText.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripViewFileText.Size = New System.Drawing.Size(23, 22)
        '
        'ToolStripViewFile
        '
        Me.ToolStripViewFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripViewFile.Image = Global.DiskImageTool.My.Resources.Resources.BinaryFile
        Me.ToolStripViewFile.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripViewFile.Name = "ToolStripViewFile"
        Me.ToolStripViewFile.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.ToolStripViewFile.Size = New System.Drawing.Size(23, 22)
        '
        'ToolStripSeparatorFAT
        '
        Me.ToolStripSeparatorFAT.Name = "ToolStripSeparatorFAT"
        Me.ToolStripSeparatorFAT.Size = New System.Drawing.Size(6, 25)
        '
        'StatusStripBottom
        '
        Me.StatusStripBottom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusBarStatus, Me.StatusBarModified, Me.StatusBarFileName, Me.StatusBarFileCount, Me.StatusBarFileSector, Me.StatusBarFileTrack, Me.StatusBarImageCount, Me.StatusBarImagesModified})
        Me.StatusStripBottom.Location = New System.Drawing.Point(0, 552)
        Me.StatusStripBottom.Name = "StatusStripBottom"
        Me.StatusStripBottom.ShowItemToolTips = True
        Me.StatusStripBottom.Size = New System.Drawing.Size(1004, 24)
        Me.StatusStripBottom.TabIndex = 3
        '
        'StatusBarStatus
        '
        Me.StatusBarStatus.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.StatusBarStatus.ForeColor = System.Drawing.Color.Red
        Me.StatusBarStatus.Name = "StatusBarStatus"
        Me.StatusBarStatus.Size = New System.Drawing.Size(47, 19)
        Me.StatusBarStatus.Text = "{Status}"
        '
        'StatusBarModified
        '
        Me.StatusBarModified.ForeColor = System.Drawing.Color.Blue
        Me.StatusBarModified.Name = "StatusBarModified"
        Me.StatusBarModified.Size = New System.Drawing.Size(63, 19)
        Me.StatusBarModified.Text = "{Modified}"
        '
        'StatusBarFileName
        '
        Me.StatusBarFileName.AutoToolTip = True
        Me.StatusBarFileName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.StatusBarFileName.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.StatusBarFileName.Name = "StatusBarFileName"
        Me.StatusBarFileName.Size = New System.Drawing.Size(549, 19)
        Me.StatusBarFileName.Spring = True
        Me.StatusBarFileName.Text = "{FileName}"
        Me.StatusBarFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'StatusBarFileCount
        '
        Me.StatusBarFileCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.StatusBarFileCount.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.StatusBarFileCount.Name = "StatusBarFileCount"
        Me.StatusBarFileCount.Size = New System.Drawing.Size(42, 19)
        Me.StatusBarFileCount.Text = "{Files}"
        '
        'StatusBarFileSector
        '
        Me.StatusBarFileSector.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.StatusBarFileSector.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.StatusBarFileSector.Name = "StatusBarFileSector"
        Me.StatusBarFileSector.Size = New System.Drawing.Size(52, 19)
        Me.StatusBarFileSector.Text = "{Sector}"
        '
        'StatusBarFileTrack
        '
        Me.StatusBarFileTrack.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.StatusBarFileTrack.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.StatusBarFileTrack.Name = "StatusBarFileTrack"
        Me.StatusBarFileTrack.Size = New System.Drawing.Size(47, 19)
        Me.StatusBarFileTrack.Text = "{Track}"
        '
        'StatusBarImageCount
        '
        Me.StatusBarImageCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.StatusBarImageCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.StatusBarImageCount.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.StatusBarImageCount.Name = "StatusBarImageCount"
        Me.StatusBarImageCount.Size = New System.Drawing.Size(57, 19)
        Me.StatusBarImageCount.Text = "{Images}"
        Me.StatusBarImageCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'StatusBarImagesModified
        '
        Me.StatusBarImagesModified.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.StatusBarImagesModified.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.StatusBarImagesModified.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.StatusBarImagesModified.Name = "StatusBarImagesModified"
        Me.StatusBarImagesModified.Size = New System.Drawing.Size(108, 19)
        Me.StatusBarImagesModified.Text = "{Images Modified}"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1004, 576)
        Me.Controls.Add(SplitContainer1)
        Me.Controls.Add(Me.ToolStripTop)
        Me.Controls.Add(Me.StatusStripBottom)
        Me.Controls.Add(Me.MenuStripTop)
        Me.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStripTop
        Me.MinimumSize = New System.Drawing.Size(960, 600)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
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
        Me.MenuStripTop.ResumeLayout(False)
        Me.MenuStripTop.PerformLayout()
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
    Friend WithEvents MenuEditImageProperties As ToolStripMenuItem
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
    Friend WithEvents MenuFileRecent As ToolStripMenuItem
    Friend WithEvents MenuStripTop As MenuStrip
    Friend WithEvents MainMenuDisk As ToolStripMenuItem
    Friend WithEvents MainMenuTools As ToolStripMenuItem
    Friend WithEvents MainMenuView As ToolStripMenuItem
    Friend WithEvents MenuEditSeparatorImage As ToolStripSeparator
End Class
