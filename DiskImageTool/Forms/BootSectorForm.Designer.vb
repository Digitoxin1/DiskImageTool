<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class BootSectorForm
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
        Dim FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
        Dim FlowLayoutPanel3 As System.Windows.Forms.FlowLayoutPanel
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BootSectorForm))
        Dim FlowLayoutPanel4 As System.Windows.Forms.FlowLayoutPanel
        Me.LblDiskType = New System.Windows.Forms.Label()
        Me.CboDiskType = New System.Windows.Forms.ComboBox()
        Me.GroupBoxMain = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanelMain = New System.Windows.Forms.TableLayoutPanel()
        Me.LblBytesPerSector = New System.Windows.Forms.Label()
        Me.LblReservedSectors = New System.Windows.Forms.Label()
        Me.LblRootDirectoryEntries = New System.Windows.Forms.Label()
        Me.LblOEMName = New System.Windows.Forms.Label()
        Me.CboOEMName = New System.Windows.Forms.ComboBox()
        Me.CboBytesPerSector = New System.Windows.Forms.ComboBox()
        Me.LblSectorsPerCluster = New System.Windows.Forms.Label()
        Me.CboSectorsPerCluster = New System.Windows.Forms.ComboBox()
        Me.LblHiddenSectors = New System.Windows.Forms.Label()
        Me.TxtMediaDescriptor = New System.Windows.Forms.Label()
        Me.TxtReservedSectors = New System.Windows.Forms.TextBox()
        Me.LblNumberOfFATS = New System.Windows.Forms.Label()
        Me.TxtNumberOfFATs = New System.Windows.Forms.TextBox()
        Me.TxtRootDirectoryEntries = New System.Windows.Forms.TextBox()
        Me.LblSectorCountSmall = New System.Windows.Forms.Label()
        Me.TxtSectorCountSmall = New System.Windows.Forms.TextBox()
        Me.LblSectorsPerFAT = New System.Windows.Forms.Label()
        Me.LblSectorsPerTrack = New System.Windows.Forms.Label()
        Me.TxtSectorsPerFAT = New System.Windows.Forms.TextBox()
        Me.TxtSectorsPerTrack = New System.Windows.Forms.TextBox()
        Me.LblNumberOfHeads = New System.Windows.Forms.Label()
        Me.TxtNumberOfHeads = New System.Windows.Forms.TextBox()
        Me.TxtHiddenSectors = New System.Windows.Forms.TextBox()
        Me.GroupBoxExtended = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.BtnVolumeSerialNumber = New System.Windows.Forms.Button()
        Me.TxtDriveNumber = New System.Windows.Forms.TextBox()
        Me.LblDriveNumber = New System.Windows.Forms.Label()
        Me.LblSectorCountLarge = New System.Windows.Forms.Label()
        Me.TxtSectorCountLarge = New System.Windows.Forms.TextBox()
        Me.LblVolumeLabel = New System.Windows.Forms.Label()
        Me.TxtVolumeLabel = New System.Windows.Forms.TextBox()
        Me.LblFileSystemType = New System.Windows.Forms.Label()
        Me.TxtFileSystemType = New System.Windows.Forms.TextBox()
        Me.LblVolumeSerialNumber = New System.Windows.Forms.Label()
        Me.lblExtendedBootSignature = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.LblExtendedMsg = New System.Windows.Forms.Label()
        Me.HexOEMName = New DiskImageTool.HexTextBox()
        Me.HexMediaDescriptor = New DiskImageTool.HexTextBox()
        Me.HexFileSystemType = New DiskImageTool.HexTextBox()
        Me.HexVolumeLabel = New DiskImageTool.HexTextBox()
        Me.HexVolumeSerialNumber = New DiskImageTool.HexTextBox()
        Me.HexExtendedBootSignature = New DiskImageTool.HexTextBox()
        FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        FlowLayoutPanel3 = New System.Windows.Forms.FlowLayoutPanel()
        FlowLayoutPanel4 = New System.Windows.Forms.FlowLayoutPanel()
        FlowLayoutPanel1.SuspendLayout()
        FlowLayoutPanel3.SuspendLayout()
        Me.GroupBoxMain.SuspendLayout()
        Me.TableLayoutPanelMain.SuspendLayout()
        Me.GroupBoxExtended.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.FlowLayoutPanel2.SuspendLayout()
        FlowLayoutPanel4.SuspendLayout()
        Me.SuspendLayout()
        '
        'FlowLayoutPanel1
        '
        FlowLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        FlowLayoutPanel1.AutoSize = True
        FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        FlowLayoutPanel1.Controls.Add(Me.LblDiskType)
        FlowLayoutPanel1.Controls.Add(Me.CboDiskType)
        FlowLayoutPanel1.Location = New System.Drawing.Point(315, 3)
        FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        FlowLayoutPanel1.Size = New System.Drawing.Size(188, 27)
        FlowLayoutPanel1.TabIndex = 0
        '
        'LblDiskType
        '
        Me.LblDiskType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblDiskType.AutoSize = True
        Me.LblDiskType.Location = New System.Drawing.Point(3, 7)
        Me.LblDiskType.Name = "LblDiskType"
        Me.LblDiskType.Size = New System.Drawing.Size(55, 13)
        Me.LblDiskType.TabIndex = 0
        Me.LblDiskType.Text = "Disk Type"
        '
        'CboDiskType
        '
        Me.CboDiskType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CboDiskType.FormattingEnabled = True
        Me.CboDiskType.Location = New System.Drawing.Point(64, 3)
        Me.CboDiskType.Name = "CboDiskType"
        Me.CboDiskType.Size = New System.Drawing.Size(121, 21)
        Me.CboDiskType.TabIndex = 1
        '
        'FlowLayoutPanel3
        '
        FlowLayoutPanel3.AutoSize = True
        FlowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        FlowLayoutPanel3.Controls.Add(FlowLayoutPanel1)
        FlowLayoutPanel3.Controls.Add(Me.GroupBoxMain)
        FlowLayoutPanel3.Controls.Add(Me.GroupBoxExtended)
        FlowLayoutPanel3.Controls.Add(Me.FlowLayoutPanel2)
        FlowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        FlowLayoutPanel3.Location = New System.Drawing.Point(21, 21)
        FlowLayoutPanel3.Name = "FlowLayoutPanel3"
        FlowLayoutPanel3.Size = New System.Drawing.Size(506, 467)
        FlowLayoutPanel3.TabIndex = 0
        FlowLayoutPanel3.WrapContents = False
        '
        'GroupBoxMain
        '
        Me.GroupBoxMain.AutoSize = True
        Me.GroupBoxMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupBoxMain.Controls.Add(Me.TableLayoutPanelMain)
        Me.GroupBoxMain.Location = New System.Drawing.Point(3, 36)
        Me.GroupBoxMain.Name = "GroupBoxMain"
        Me.GroupBoxMain.Size = New System.Drawing.Size(500, 222)
        Me.GroupBoxMain.TabIndex = 1
        Me.GroupBoxMain.TabStop = False
        Me.GroupBoxMain.Text = "Boot Record"
        '
        'TableLayoutPanelMain
        '
        Me.TableLayoutPanelMain.AutoSize = True
        Me.TableLayoutPanelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanelMain.ColumnCount = 4
        Me.TableLayoutPanelMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 138.0!))
        Me.TableLayoutPanelMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130.0!))
        Me.TableLayoutPanelMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelMain.Controls.Add(Me.LblBytesPerSector, 0, 1)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblReservedSectors, 0, 2)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblRootDirectoryEntries, 0, 3)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblOEMName, 0, 0)
        Me.TableLayoutPanelMain.Controls.Add(Me.CboOEMName, 1, 0)
        Me.TableLayoutPanelMain.Controls.Add(Me.HexOEMName, 2, 0)
        Me.TableLayoutPanelMain.Controls.Add(Me.CboBytesPerSector, 1, 1)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblSectorsPerCluster, 2, 1)
        Me.TableLayoutPanelMain.Controls.Add(Me.CboSectorsPerCluster, 3, 1)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblHiddenSectors, 0, 6)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtMediaDescriptor, 0, 4)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtReservedSectors, 1, 2)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblNumberOfFATS, 2, 2)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtNumberOfFATs, 3, 2)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtRootDirectoryEntries, 1, 3)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblSectorCountSmall, 2, 3)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtSectorCountSmall, 3, 3)
        Me.TableLayoutPanelMain.Controls.Add(Me.HexMediaDescriptor, 1, 4)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblSectorsPerFAT, 2, 4)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblSectorsPerTrack, 0, 5)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtSectorsPerFAT, 3, 4)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtSectorsPerTrack, 1, 5)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblNumberOfHeads, 2, 5)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtNumberOfHeads, 3, 5)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtHiddenSectors, 1, 6)
        Me.TableLayoutPanelMain.Location = New System.Drawing.Point(6, 19)
        Me.TableLayoutPanelMain.Name = "TableLayoutPanelMain"
        Me.TableLayoutPanelMain.RowCount = 7
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.Size = New System.Drawing.Size(488, 184)
        Me.TableLayoutPanelMain.TabIndex = 0
        '
        'LblBytesPerSector
        '
        Me.LblBytesPerSector.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblBytesPerSector.AutoSize = True
        Me.LblBytesPerSector.Location = New System.Drawing.Point(3, 34)
        Me.LblBytesPerSector.Name = "LblBytesPerSector"
        Me.LblBytesPerSector.Size = New System.Drawing.Size(85, 13)
        Me.LblBytesPerSector.TabIndex = 3
        Me.LblBytesPerSector.Text = "Bytes per Sector"
        '
        'LblReservedSectors
        '
        Me.LblReservedSectors.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblReservedSectors.AutoSize = True
        Me.LblReservedSectors.Location = New System.Drawing.Point(3, 60)
        Me.LblReservedSectors.Name = "LblReservedSectors"
        Me.LblReservedSectors.Size = New System.Drawing.Size(92, 13)
        Me.LblReservedSectors.TabIndex = 7
        Me.LblReservedSectors.Text = "Reserved Sectors"
        '
        'LblRootDirectoryEntries
        '
        Me.LblRootDirectoryEntries.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblRootDirectoryEntries.AutoSize = True
        Me.LblRootDirectoryEntries.Location = New System.Drawing.Point(3, 86)
        Me.LblRootDirectoryEntries.Name = "LblRootDirectoryEntries"
        Me.LblRootDirectoryEntries.Size = New System.Drawing.Size(110, 13)
        Me.LblRootDirectoryEntries.TabIndex = 11
        Me.LblRootDirectoryEntries.Text = "Root Directory Entries"
        '
        'LblOEMName
        '
        Me.LblOEMName.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblOEMName.AutoSize = True
        Me.LblOEMName.Location = New System.Drawing.Point(3, 7)
        Me.LblOEMName.Name = "LblOEMName"
        Me.LblOEMName.Size = New System.Drawing.Size(62, 13)
        Me.LblOEMName.TabIndex = 0
        Me.LblOEMName.Text = "OEM Name"
        '
        'CboOEMName
        '
        Me.CboOEMName.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.CboOEMName.FormattingEnabled = True
        Me.CboOEMName.Location = New System.Drawing.Point(141, 3)
        Me.CboOEMName.MaxLength = 8
        Me.CboOEMName.Name = "CboOEMName"
        Me.CboOEMName.Size = New System.Drawing.Size(104, 21)
        Me.CboOEMName.TabIndex = 1
        '
        'CboBytesPerSector
        '
        Me.CboBytesPerSector.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.CboBytesPerSector.FormattingEnabled = True
        Me.CboBytesPerSector.Location = New System.Drawing.Point(141, 30)
        Me.CboBytesPerSector.MaxLength = 5
        Me.CboBytesPerSector.Name = "CboBytesPerSector"
        Me.CboBytesPerSector.Size = New System.Drawing.Size(104, 21)
        Me.CboBytesPerSector.TabIndex = 4
        '
        'LblSectorsPerCluster
        '
        Me.LblSectorsPerCluster.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSectorsPerCluster.AutoSize = True
        Me.LblSectorsPerCluster.Location = New System.Drawing.Point(251, 34)
        Me.LblSectorsPerCluster.Name = "LblSectorsPerCluster"
        Me.LblSectorsPerCluster.Size = New System.Drawing.Size(97, 13)
        Me.LblSectorsPerCluster.TabIndex = 5
        Me.LblSectorsPerCluster.Text = "Sectors Per Cluster"
        '
        'CboSectorsPerCluster
        '
        Me.CboSectorsPerCluster.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.CboSectorsPerCluster.FormattingEnabled = True
        Me.CboSectorsPerCluster.Location = New System.Drawing.Point(381, 30)
        Me.CboSectorsPerCluster.MaxLength = 3
        Me.CboSectorsPerCluster.Name = "CboSectorsPerCluster"
        Me.CboSectorsPerCluster.Size = New System.Drawing.Size(104, 21)
        Me.CboSectorsPerCluster.TabIndex = 6
        '
        'LblHiddenSectors
        '
        Me.LblHiddenSectors.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblHiddenSectors.AutoSize = True
        Me.LblHiddenSectors.Location = New System.Drawing.Point(3, 164)
        Me.LblHiddenSectors.Name = "LblHiddenSectors"
        Me.LblHiddenSectors.Size = New System.Drawing.Size(80, 13)
        Me.LblHiddenSectors.TabIndex = 23
        Me.LblHiddenSectors.Text = "Hidden Sectors"
        '
        'TxtMediaDescriptor
        '
        Me.TxtMediaDescriptor.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtMediaDescriptor.AutoSize = True
        Me.TxtMediaDescriptor.Location = New System.Drawing.Point(3, 112)
        Me.TxtMediaDescriptor.Name = "TxtMediaDescriptor"
        Me.TxtMediaDescriptor.Size = New System.Drawing.Size(87, 13)
        Me.TxtMediaDescriptor.TabIndex = 15
        Me.TxtMediaDescriptor.Text = "Media Descriptor"
        '
        'TxtReservedSectors
        '
        Me.TxtReservedSectors.Location = New System.Drawing.Point(141, 57)
        Me.TxtReservedSectors.MaxLength = 3
        Me.TxtReservedSectors.Name = "TxtReservedSectors"
        Me.TxtReservedSectors.Size = New System.Drawing.Size(104, 20)
        Me.TxtReservedSectors.TabIndex = 8
        '
        'LblNumberOfFATS
        '
        Me.LblNumberOfFATS.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblNumberOfFATS.AutoSize = True
        Me.LblNumberOfFATS.Location = New System.Drawing.Point(251, 60)
        Me.LblNumberOfFATS.Name = "LblNumberOfFATS"
        Me.LblNumberOfFATS.Size = New System.Drawing.Size(84, 13)
        Me.LblNumberOfFATS.TabIndex = 9
        Me.LblNumberOfFATS.Text = "Number of FATs"
        '
        'TxtNumberOfFATs
        '
        Me.TxtNumberOfFATs.Location = New System.Drawing.Point(381, 57)
        Me.TxtNumberOfFATs.MaxLength = 3
        Me.TxtNumberOfFATs.Name = "TxtNumberOfFATs"
        Me.TxtNumberOfFATs.Size = New System.Drawing.Size(104, 20)
        Me.TxtNumberOfFATs.TabIndex = 10
        '
        'TxtRootDirectoryEntries
        '
        Me.TxtRootDirectoryEntries.Location = New System.Drawing.Point(141, 83)
        Me.TxtRootDirectoryEntries.MaxLength = 5
        Me.TxtRootDirectoryEntries.Name = "TxtRootDirectoryEntries"
        Me.TxtRootDirectoryEntries.Size = New System.Drawing.Size(104, 20)
        Me.TxtRootDirectoryEntries.TabIndex = 12
        '
        'LblSectorCountSmall
        '
        Me.LblSectorCountSmall.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSectorCountSmall.AutoSize = True
        Me.LblSectorCountSmall.Location = New System.Drawing.Point(251, 86)
        Me.LblSectorCountSmall.Name = "LblSectorCountSmall"
        Me.LblSectorCountSmall.Size = New System.Drawing.Size(96, 13)
        Me.LblSectorCountSmall.TabIndex = 13
        Me.LblSectorCountSmall.Text = "Total Sector Count"
        '
        'TxtSectorCountSmall
        '
        Me.TxtSectorCountSmall.Location = New System.Drawing.Point(381, 83)
        Me.TxtSectorCountSmall.MaxLength = 5
        Me.TxtSectorCountSmall.Name = "TxtSectorCountSmall"
        Me.TxtSectorCountSmall.Size = New System.Drawing.Size(104, 20)
        Me.TxtSectorCountSmall.TabIndex = 14
        '
        'LblSectorsPerFAT
        '
        Me.LblSectorsPerFAT.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSectorsPerFAT.AutoSize = True
        Me.LblSectorsPerFAT.Location = New System.Drawing.Point(251, 112)
        Me.LblSectorsPerFAT.Name = "LblSectorsPerFAT"
        Me.LblSectorsPerFAT.Size = New System.Drawing.Size(84, 13)
        Me.LblSectorsPerFAT.TabIndex = 17
        Me.LblSectorsPerFAT.Text = "Sectors per FAT"
        '
        'LblSectorsPerTrack
        '
        Me.LblSectorsPerTrack.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSectorsPerTrack.AutoSize = True
        Me.LblSectorsPerTrack.Location = New System.Drawing.Point(3, 138)
        Me.LblSectorsPerTrack.Name = "LblSectorsPerTrack"
        Me.LblSectorsPerTrack.Size = New System.Drawing.Size(92, 13)
        Me.LblSectorsPerTrack.TabIndex = 19
        Me.LblSectorsPerTrack.Text = "Sectors per Track"
        '
        'TxtSectorsPerFAT
        '
        Me.TxtSectorsPerFAT.Location = New System.Drawing.Point(381, 109)
        Me.TxtSectorsPerFAT.MaxLength = 5
        Me.TxtSectorsPerFAT.Name = "TxtSectorsPerFAT"
        Me.TxtSectorsPerFAT.Size = New System.Drawing.Size(104, 20)
        Me.TxtSectorsPerFAT.TabIndex = 18
        '
        'TxtSectorsPerTrack
        '
        Me.TxtSectorsPerTrack.Location = New System.Drawing.Point(141, 135)
        Me.TxtSectorsPerTrack.MaxLength = 5
        Me.TxtSectorsPerTrack.Name = "TxtSectorsPerTrack"
        Me.TxtSectorsPerTrack.Size = New System.Drawing.Size(104, 20)
        Me.TxtSectorsPerTrack.TabIndex = 20
        '
        'LblNumberOfHeads
        '
        Me.LblNumberOfHeads.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblNumberOfHeads.AutoSize = True
        Me.LblNumberOfHeads.Location = New System.Drawing.Point(251, 138)
        Me.LblNumberOfHeads.Name = "LblNumberOfHeads"
        Me.LblNumberOfHeads.Size = New System.Drawing.Size(90, 13)
        Me.LblNumberOfHeads.TabIndex = 21
        Me.LblNumberOfHeads.Text = "Number of Heads"
        '
        'TxtNumberOfHeads
        '
        Me.TxtNumberOfHeads.Location = New System.Drawing.Point(381, 135)
        Me.TxtNumberOfHeads.MaxLength = 5
        Me.TxtNumberOfHeads.Name = "TxtNumberOfHeads"
        Me.TxtNumberOfHeads.Size = New System.Drawing.Size(104, 20)
        Me.TxtNumberOfHeads.TabIndex = 22
        '
        'TxtHiddenSectors
        '
        Me.TxtHiddenSectors.Location = New System.Drawing.Point(141, 161)
        Me.TxtHiddenSectors.MaxLength = 10
        Me.TxtHiddenSectors.Name = "TxtHiddenSectors"
        Me.TxtHiddenSectors.Size = New System.Drawing.Size(104, 20)
        Me.TxtHiddenSectors.TabIndex = 24
        '
        'GroupBoxExtended
        '
        Me.GroupBoxExtended.AutoSize = True
        Me.GroupBoxExtended.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupBoxExtended.Controls.Add(FlowLayoutPanel4)
        Me.GroupBoxExtended.Location = New System.Drawing.Point(3, 264)
        Me.GroupBoxExtended.Name = "GroupBoxExtended"
        Me.GroupBoxExtended.Size = New System.Drawing.Size(500, 165)
        Me.GroupBoxExtended.TabIndex = 2
        Me.GroupBoxExtended.TabStop = False
        Me.GroupBoxExtended.Text = "Extended Parameters"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 5
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 138.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.BtnVolumeSerialNumber, 4, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.HexFileSystemType, 2, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.TxtDriveNumber, 3, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.LblDriveNumber, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.LblSectorCountLarge, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TxtSectorCountLarge, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.LblVolumeLabel, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.TxtVolumeLabel, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.HexVolumeLabel, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.LblFileSystemType, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.TxtFileSystemType, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.LblVolumeSerialNumber, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.HexVolumeSerialNumber, 3, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.lblExtendedBootSignature, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.HexExtendedBootSignature, 1, 1)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 23)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(488, 104)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'BtnVolumeSerialNumber
        '
        Me.BtnVolumeSerialNumber.AutoSize = True
        Me.BtnVolumeSerialNumber.FlatAppearance.BorderSize = 0
        Me.BtnVolumeSerialNumber.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnVolumeSerialNumber.Image = CType(resources.GetObject("BtnVolumeSerialNumber.Image"), System.Drawing.Image)
        Me.BtnVolumeSerialNumber.Location = New System.Drawing.Point(461, 26)
        Me.BtnVolumeSerialNumber.Margin = New System.Windows.Forms.Padding(0)
        Me.BtnVolumeSerialNumber.Name = "BtnVolumeSerialNumber"
        Me.BtnVolumeSerialNumber.Size = New System.Drawing.Size(24, 24)
        Me.BtnVolumeSerialNumber.TabIndex = 8
        Me.BtnVolumeSerialNumber.UseVisualStyleBackColor = True
        '
        'TxtDriveNumber
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.TxtDriveNumber, 2)
        Me.TxtDriveNumber.Location = New System.Drawing.Point(371, 3)
        Me.TxtDriveNumber.MaxLength = 3
        Me.TxtDriveNumber.Name = "TxtDriveNumber"
        Me.TxtDriveNumber.Size = New System.Drawing.Size(114, 20)
        Me.TxtDriveNumber.TabIndex = 3
        '
        'LblDriveNumber
        '
        Me.LblDriveNumber.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblDriveNumber.AutoSize = True
        Me.LblDriveNumber.Location = New System.Drawing.Point(251, 6)
        Me.LblDriveNumber.Name = "LblDriveNumber"
        Me.LblDriveNumber.Size = New System.Drawing.Size(72, 13)
        Me.LblDriveNumber.TabIndex = 2
        Me.LblDriveNumber.Text = "Drive Number"
        '
        'LblSectorCountLarge
        '
        Me.LblSectorCountLarge.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSectorCountLarge.AutoSize = True
        Me.LblSectorCountLarge.Location = New System.Drawing.Point(3, 6)
        Me.LblSectorCountLarge.Name = "LblSectorCountLarge"
        Me.LblSectorCountLarge.Size = New System.Drawing.Size(132, 13)
        Me.LblSectorCountLarge.TabIndex = 0
        Me.LblSectorCountLarge.Text = "Total Sector Count (Large)"
        '
        'TxtSectorCountLarge
        '
        Me.TxtSectorCountLarge.Location = New System.Drawing.Point(141, 3)
        Me.TxtSectorCountLarge.MaxLength = 10
        Me.TxtSectorCountLarge.Name = "TxtSectorCountLarge"
        Me.TxtSectorCountLarge.Size = New System.Drawing.Size(104, 20)
        Me.TxtSectorCountLarge.TabIndex = 1
        '
        'LblVolumeLabel
        '
        Me.LblVolumeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblVolumeLabel.AutoSize = True
        Me.LblVolumeLabel.Location = New System.Drawing.Point(3, 58)
        Me.LblVolumeLabel.Name = "LblVolumeLabel"
        Me.LblVolumeLabel.Size = New System.Drawing.Size(71, 13)
        Me.LblVolumeLabel.TabIndex = 9
        Me.LblVolumeLabel.Text = "Volume Label"
        '
        'TxtVolumeLabel
        '
        Me.TxtVolumeLabel.Location = New System.Drawing.Point(141, 55)
        Me.TxtVolumeLabel.MaxLength = 11
        Me.TxtVolumeLabel.Name = "TxtVolumeLabel"
        Me.TxtVolumeLabel.Size = New System.Drawing.Size(104, 20)
        Me.TxtVolumeLabel.TabIndex = 10
        '
        'LblFileSystemType
        '
        Me.LblFileSystemType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblFileSystemType.AutoSize = True
        Me.LblFileSystemType.Location = New System.Drawing.Point(3, 84)
        Me.LblFileSystemType.Name = "LblFileSystemType"
        Me.LblFileSystemType.Size = New System.Drawing.Size(74, 13)
        Me.LblFileSystemType.TabIndex = 12
        Me.LblFileSystemType.Text = "File System ID"
        '
        'TxtFileSystemType
        '
        Me.TxtFileSystemType.Location = New System.Drawing.Point(141, 81)
        Me.TxtFileSystemType.MaxLength = 8
        Me.TxtFileSystemType.Name = "TxtFileSystemType"
        Me.TxtFileSystemType.Size = New System.Drawing.Size(104, 20)
        Me.TxtFileSystemType.TabIndex = 13
        '
        'LblVolumeSerialNumber
        '
        Me.LblVolumeSerialNumber.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblVolumeSerialNumber.AutoSize = True
        Me.LblVolumeSerialNumber.Location = New System.Drawing.Point(251, 32)
        Me.LblVolumeSerialNumber.Name = "LblVolumeSerialNumber"
        Me.LblVolumeSerialNumber.Size = New System.Drawing.Size(111, 13)
        Me.LblVolumeSerialNumber.TabIndex = 6
        Me.LblVolumeSerialNumber.Text = "Volume Serial Number"
        '
        'lblExtendedBootSignature
        '
        Me.lblExtendedBootSignature.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblExtendedBootSignature.AutoSize = True
        Me.lblExtendedBootSignature.Location = New System.Drawing.Point(3, 32)
        Me.lblExtendedBootSignature.Name = "lblExtendedBootSignature"
        Me.lblExtendedBootSignature.Size = New System.Drawing.Size(125, 13)
        Me.lblExtendedBootSignature.TabIndex = 4
        Me.lblExtendedBootSignature.Text = "Extended Boot Signature"
        '
        'FlowLayoutPanel2
        '
        Me.FlowLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.FlowLayoutPanel2.AutoSize = True
        Me.FlowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel2.Controls.Add(Me.BtnUpdate)
        Me.FlowLayoutPanel2.Controls.Add(Me.BtnCancel)
        Me.FlowLayoutPanel2.Location = New System.Drawing.Point(157, 435)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        Me.FlowLayoutPanel2.Size = New System.Drawing.Size(192, 29)
        Me.FlowLayoutPanel2.TabIndex = 3
        '
        'BtnUpdate
        '
        Me.BtnUpdate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnUpdate.Location = New System.Drawing.Point(3, 3)
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.Size = New System.Drawing.Size(90, 23)
        Me.BtnUpdate.TabIndex = 0
        Me.BtnUpdate.Text = "&Update"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(99, 3)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(90, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "&Cancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel4
        '
        FlowLayoutPanel4.AutoSize = True
        FlowLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        FlowLayoutPanel4.Controls.Add(Me.LblExtendedMsg)
        FlowLayoutPanel4.Controls.Add(Me.TableLayoutPanel1)
        FlowLayoutPanel4.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        FlowLayoutPanel4.Location = New System.Drawing.Point(6, 19)
        FlowLayoutPanel4.Name = "FlowLayoutPanel4"
        FlowLayoutPanel4.Size = New System.Drawing.Size(488, 127)
        FlowLayoutPanel4.TabIndex = 1
        '
        'LblExtendedMsg
        '
        Me.LblExtendedMsg.AutoSize = True
        Me.LblExtendedMsg.ForeColor = System.Drawing.Color.Blue
        Me.LblExtendedMsg.Location = New System.Drawing.Point(3, 0)
        Me.LblExtendedMsg.Margin = New System.Windows.Forms.Padding(3, 0, 3, 10)
        Me.LblExtendedMsg.Name = "LblExtendedMsg"
        Me.LblExtendedMsg.Size = New System.Drawing.Size(417, 13)
        Me.LblExtendedMsg.TabIndex = 0
        Me.LblExtendedMsg.Text = "There appears to be data stored in the Extended Parameters block of this Boot Sec" &
    "tor. "
        '
        'HexOEMName
        '
        Me.TableLayoutPanelMain.SetColumnSpan(Me.HexOEMName, 2)
        Me.HexOEMName.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HexOEMName.Location = New System.Drawing.Point(251, 3)
        Me.HexOEMName.MaskLength = 8
        Me.HexOEMName.Name = "HexOEMName"
        Me.HexOEMName.Size = New System.Drawing.Size(170, 20)
        Me.HexOEMName.TabIndex = 2
        '
        'HexMediaDescriptor
        '
        Me.HexMediaDescriptor.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HexMediaDescriptor.Location = New System.Drawing.Point(141, 109)
        Me.HexMediaDescriptor.MaskLength = 1
        Me.HexMediaDescriptor.Name = "HexMediaDescriptor"
        Me.HexMediaDescriptor.Size = New System.Drawing.Size(30, 20)
        Me.HexMediaDescriptor.TabIndex = 16
        '
        'HexFileSystemType
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.HexFileSystemType, 3)
        Me.HexFileSystemType.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HexFileSystemType.Location = New System.Drawing.Point(251, 81)
        Me.HexFileSystemType.MaskLength = 8
        Me.HexFileSystemType.Name = "HexFileSystemType"
        Me.HexFileSystemType.Size = New System.Drawing.Size(170, 20)
        Me.HexFileSystemType.TabIndex = 14
        '
        'HexVolumeLabel
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.HexVolumeLabel, 3)
        Me.HexVolumeLabel.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HexVolumeLabel.Location = New System.Drawing.Point(251, 55)
        Me.HexVolumeLabel.MaskLength = 11
        Me.HexVolumeLabel.Name = "HexVolumeLabel"
        Me.HexVolumeLabel.Size = New System.Drawing.Size(234, 20)
        Me.HexVolumeLabel.TabIndex = 11
        '
        'HexVolumeSerialNumber
        '
        Me.HexVolumeSerialNumber.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HexVolumeSerialNumber.Location = New System.Drawing.Point(371, 29)
        Me.HexVolumeSerialNumber.MaskLength = 4
        Me.HexVolumeSerialNumber.Name = "HexVolumeSerialNumber"
        Me.HexVolumeSerialNumber.Size = New System.Drawing.Size(87, 20)
        Me.HexVolumeSerialNumber.TabIndex = 7
        '
        'HexExtendedBootSignature
        '
        Me.HexExtendedBootSignature.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HexExtendedBootSignature.Location = New System.Drawing.Point(141, 29)
        Me.HexExtendedBootSignature.MaskLength = 1
        Me.HexExtendedBootSignature.Name = "HexExtendedBootSignature"
        Me.HexExtendedBootSignature.Size = New System.Drawing.Size(30, 20)
        Me.HexExtendedBootSignature.TabIndex = 5
        '
        'BootSectorForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CancelButton = Me.BtnCancel
        Me.ClientSize = New System.Drawing.Size(560, 529)
        Me.Controls.Add(FlowLayoutPanel3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.HelpButton = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "BootSectorForm"
        Me.Padding = New System.Windows.Forms.Padding(18)
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Boot Sector"
        FlowLayoutPanel1.ResumeLayout(False)
        FlowLayoutPanel1.PerformLayout()
        FlowLayoutPanel3.ResumeLayout(False)
        FlowLayoutPanel3.PerformLayout()
        Me.GroupBoxMain.ResumeLayout(False)
        Me.GroupBoxMain.PerformLayout()
        Me.TableLayoutPanelMain.ResumeLayout(False)
        Me.TableLayoutPanelMain.PerformLayout()
        Me.GroupBoxExtended.ResumeLayout(False)
        Me.GroupBoxExtended.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.FlowLayoutPanel2.ResumeLayout(False)
        FlowLayoutPanel4.ResumeLayout(False)
        FlowLayoutPanel4.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents TableLayoutPanelMain As TableLayoutPanel
    Friend WithEvents HexOEMName As HexTextBox
    Friend WithEvents CboOEMName As ComboBox
    Friend WithEvents HexVolumeSerialNumber As HexTextBox
    Friend WithEvents HexMediaDescriptor As HexTextBox
    Friend WithEvents HexExtendedBootSignature As HexTextBox
    Friend WithEvents HexVolumeLabel As HexTextBox
    Friend WithEvents TxtVolumeLabel As TextBox
    Friend WithEvents TxtReservedSectors As TextBox
    Friend WithEvents TxtNumberOfFATs As TextBox
    Friend WithEvents TxtRootDirectoryEntries As TextBox
    Friend WithEvents TxtSectorCountSmall As TextBox
    Friend WithEvents TxtSectorsPerFAT As TextBox
    Friend WithEvents TxtSectorsPerTrack As TextBox
    Friend WithEvents TxtNumberOfHeads As TextBox
    Friend WithEvents TxtHiddenSectors As TextBox
    Friend WithEvents TxtSectorCountLarge As TextBox
    Friend WithEvents TxtDriveNumber As TextBox
    Friend WithEvents CboBytesPerSector As ComboBox
    Friend WithEvents CboSectorsPerCluster As ComboBox
    Friend WithEvents CboDiskType As ComboBox
    Friend WithEvents GroupBoxMain As GroupBox
    Friend WithEvents GroupBoxExtended As GroupBox
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents HexFileSystemType As HexTextBox
    Friend WithEvents TxtFileSystemType As TextBox
    Friend WithEvents FlowLayoutPanel2 As FlowLayoutPanel
    Friend WithEvents BtnUpdate As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents LblOEMName As Label
    Friend WithEvents LblBytesPerSector As Label
    Friend WithEvents LblSectorsPerCluster As Label
    Friend WithEvents LblReservedSectors As Label
    Friend WithEvents LblNumberOfFATS As Label
    Friend WithEvents LblRootDirectoryEntries As Label
    Friend WithEvents LblSectorCountSmall As Label
    Friend WithEvents TxtMediaDescriptor As Label
    Friend WithEvents LblSectorsPerFAT As Label
    Friend WithEvents LblSectorsPerTrack As Label
    Friend WithEvents LblNumberOfHeads As Label
    Friend WithEvents LblHiddenSectors As Label
    Friend WithEvents LblSectorCountLarge As Label
    Friend WithEvents LblDriveNumber As Label
    Friend WithEvents lblExtendedBootSignature As Label
    Friend WithEvents LblVolumeSerialNumber As Label
    Friend WithEvents LblVolumeLabel As Label
    Friend WithEvents LblFileSystemType As Label
    Friend WithEvents LblDiskType As Label
    Friend WithEvents BtnVolumeSerialNumber As Button
    Friend WithEvents LblExtendedMsg As Label
End Class
