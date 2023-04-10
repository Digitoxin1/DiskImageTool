<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BootSectorForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim Label1 As System.Windows.Forms.Label
        Dim Label2 As System.Windows.Forms.Label
        Dim Label3 As System.Windows.Forms.Label
        Dim Label4 As System.Windows.Forms.Label
        Dim Label5 As System.Windows.Forms.Label
        Dim Label6 As System.Windows.Forms.Label
        Dim Label7 As System.Windows.Forms.Label
        Dim Label8 As System.Windows.Forms.Label
        Dim Label9 As System.Windows.Forms.Label
        Dim Label10 As System.Windows.Forms.Label
        Dim Label11 As System.Windows.Forms.Label
        Dim Label13 As System.Windows.Forms.Label
        Dim Label14 As System.Windows.Forms.Label
        Dim Label15 As System.Windows.Forms.Label
        Dim Label17 As System.Windows.Forms.Label
        Dim Label18 As System.Windows.Forms.Label
        Dim Label12 As System.Windows.Forms.Label
        Dim Label19 As System.Windows.Forms.Label
        Dim FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
        Dim Label16 As System.Windows.Forms.Label
        Dim FlowLayoutPanel3 As System.Windows.Forms.FlowLayoutPanel
        Me.CboDiskType = New System.Windows.Forms.ComboBox()
        Me.GroupBoxMain = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanelMain = New System.Windows.Forms.TableLayoutPanel()
        Me.CboOEMName = New System.Windows.Forms.ComboBox()
        Me.HexOEMName = New DiskImageTool.HexTextBox()
        Me.CboBytesPerSector = New System.Windows.Forms.ComboBox()
        Me.CboSectorsPerCluster = New System.Windows.Forms.ComboBox()
        Me.TxtReservedSectors = New System.Windows.Forms.TextBox()
        Me.TxtNumberOfFATs = New System.Windows.Forms.TextBox()
        Me.TxtRootDirectoryEntries = New System.Windows.Forms.TextBox()
        Me.TxtSectorCountSmall = New System.Windows.Forms.TextBox()
        Me.HexMediaDescriptor = New DiskImageTool.HexTextBox()
        Me.TxtSectorsPerFAT = New System.Windows.Forms.TextBox()
        Me.TxtSectorsPerTrack = New System.Windows.Forms.TextBox()
        Me.TxtNumberOfHeads = New System.Windows.Forms.TextBox()
        Me.TxtHiddenSectors = New System.Windows.Forms.TextBox()
        Me.GroupBoxExtended = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.HexFileSystemType = New DiskImageTool.HexTextBox()
        Me.TxtSectorCountLarge = New System.Windows.Forms.TextBox()
        Me.TxtDriveNumber = New System.Windows.Forms.TextBox()
        Me.TxtVolumeLabel = New System.Windows.Forms.TextBox()
        Me.HexVolumeLabel = New DiskImageTool.HexTextBox()
        Me.TxtFileSystemType = New System.Windows.Forms.TextBox()
        Me.HexExtendedBootSignature = New DiskImageTool.HexTextBox()
        Me.HexVolumeSerialNumber = New DiskImageTool.HexTextBox()
        Me.FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Label1 = New System.Windows.Forms.Label()
        Label2 = New System.Windows.Forms.Label()
        Label3 = New System.Windows.Forms.Label()
        Label4 = New System.Windows.Forms.Label()
        Label5 = New System.Windows.Forms.Label()
        Label6 = New System.Windows.Forms.Label()
        Label7 = New System.Windows.Forms.Label()
        Label8 = New System.Windows.Forms.Label()
        Label9 = New System.Windows.Forms.Label()
        Label10 = New System.Windows.Forms.Label()
        Label11 = New System.Windows.Forms.Label()
        Label13 = New System.Windows.Forms.Label()
        Label14 = New System.Windows.Forms.Label()
        Label15 = New System.Windows.Forms.Label()
        Label17 = New System.Windows.Forms.Label()
        Label18 = New System.Windows.Forms.Label()
        Label12 = New System.Windows.Forms.Label()
        Label19 = New System.Windows.Forms.Label()
        FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Label16 = New System.Windows.Forms.Label()
        FlowLayoutPanel3 = New System.Windows.Forms.FlowLayoutPanel()
        FlowLayoutPanel1.SuspendLayout()
        FlowLayoutPanel3.SuspendLayout()
        Me.GroupBoxMain.SuspendLayout()
        Me.TableLayoutPanelMain.SuspendLayout()
        Me.GroupBoxExtended.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.FlowLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Label1.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label1.AutoSize = True
        Label1.Location = New System.Drawing.Point(3, 7)
        Label1.Name = "Label1"
        Label1.Size = New System.Drawing.Size(62, 13)
        Label1.TabIndex = 0
        Label1.Text = "OEM Name"
        '
        'Label2
        '
        Label2.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label2.AutoSize = True
        Label2.Location = New System.Drawing.Point(3, 34)
        Label2.Name = "Label2"
        Label2.Size = New System.Drawing.Size(85, 13)
        Label2.TabIndex = 3
        Label2.Text = "Bytes per Sector"
        '
        'Label3
        '
        Label3.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label3.AutoSize = True
        Label3.Location = New System.Drawing.Point(251, 34)
        Label3.Name = "Label3"
        Label3.Size = New System.Drawing.Size(97, 13)
        Label3.TabIndex = 5
        Label3.Text = "Sectors Per Cluster"
        '
        'Label4
        '
        Label4.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label4.AutoSize = True
        Label4.Location = New System.Drawing.Point(3, 60)
        Label4.Name = "Label4"
        Label4.Size = New System.Drawing.Size(92, 13)
        Label4.TabIndex = 7
        Label4.Text = "Reserved Sectors"
        '
        'Label5
        '
        Label5.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label5.AutoSize = True
        Label5.Location = New System.Drawing.Point(251, 60)
        Label5.Name = "Label5"
        Label5.Size = New System.Drawing.Size(84, 13)
        Label5.TabIndex = 9
        Label5.Text = "Number of FATs"
        '
        'Label6
        '
        Label6.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label6.AutoSize = True
        Label6.Location = New System.Drawing.Point(3, 86)
        Label6.Name = "Label6"
        Label6.Size = New System.Drawing.Size(110, 13)
        Label6.TabIndex = 11
        Label6.Text = "Root Directory Entries"
        '
        'Label7
        '
        Label7.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label7.AutoSize = True
        Label7.Location = New System.Drawing.Point(251, 86)
        Label7.Name = "Label7"
        Label7.Size = New System.Drawing.Size(96, 13)
        Label7.TabIndex = 13
        Label7.Text = "Total Sector Count"
        '
        'Label8
        '
        Label8.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label8.AutoSize = True
        Label8.Location = New System.Drawing.Point(3, 112)
        Label8.Name = "Label8"
        Label8.Size = New System.Drawing.Size(87, 13)
        Label8.TabIndex = 15
        Label8.Text = "Media Descriptor"
        '
        'Label9
        '
        Label9.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label9.AutoSize = True
        Label9.Location = New System.Drawing.Point(251, 112)
        Label9.Name = "Label9"
        Label9.Size = New System.Drawing.Size(84, 13)
        Label9.TabIndex = 17
        Label9.Text = "Sectors per FAT"
        '
        'Label10
        '
        Label10.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label10.AutoSize = True
        Label10.Location = New System.Drawing.Point(3, 138)
        Label10.Name = "Label10"
        Label10.Size = New System.Drawing.Size(92, 13)
        Label10.TabIndex = 19
        Label10.Text = "Sectors per Track"
        '
        'Label11
        '
        Label11.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label11.AutoSize = True
        Label11.Location = New System.Drawing.Point(251, 138)
        Label11.Name = "Label11"
        Label11.Size = New System.Drawing.Size(90, 13)
        Label11.TabIndex = 21
        Label11.Text = "Number of Heads"
        '
        'Label13
        '
        Label13.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label13.AutoSize = True
        Label13.Location = New System.Drawing.Point(3, 164)
        Label13.Name = "Label13"
        Label13.Size = New System.Drawing.Size(80, 13)
        Label13.TabIndex = 23
        Label13.Text = "Hidden Sectors"
        '
        'Label14
        '
        Label14.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label14.AutoSize = True
        Label14.Location = New System.Drawing.Point(3, 6)
        Label14.Name = "Label14"
        Label14.Size = New System.Drawing.Size(132, 13)
        Label14.TabIndex = 0
        Label14.Text = "Total Sector Count (Large)"
        '
        'Label15
        '
        Label15.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label15.AutoSize = True
        Label15.Location = New System.Drawing.Point(3, 32)
        Label15.Name = "Label15"
        Label15.Size = New System.Drawing.Size(72, 13)
        Label15.TabIndex = 4
        Label15.Text = "Drive Number"
        '
        'Label17
        '
        Label17.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label17.AutoSize = True
        Label17.Location = New System.Drawing.Point(251, 6)
        Label17.Name = "Label17"
        Label17.Size = New System.Drawing.Size(125, 13)
        Label17.TabIndex = 2
        Label17.Text = "Extended Boot Signature"
        '
        'Label18
        '
        Label18.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label18.AutoSize = True
        Label18.Location = New System.Drawing.Point(251, 32)
        Label18.Name = "Label18"
        Label18.Size = New System.Drawing.Size(111, 13)
        Label18.TabIndex = 6
        Label18.Text = "Volume Serial Number"
        '
        'Label12
        '
        Label12.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label12.AutoSize = True
        Label12.Location = New System.Drawing.Point(3, 58)
        Label12.Name = "Label12"
        Label12.Size = New System.Drawing.Size(71, 13)
        Label12.TabIndex = 8
        Label12.Text = "Volume Label"
        '
        'Label19
        '
        Label19.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label19.AutoSize = True
        Label19.Location = New System.Drawing.Point(3, 7)
        Label19.Name = "Label19"
        Label19.Size = New System.Drawing.Size(55, 13)
        Label19.TabIndex = 0
        Label19.Text = "Disk Type"
        '
        'FlowLayoutPanel1
        '
        FlowLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        FlowLayoutPanel1.AutoSize = True
        FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        FlowLayoutPanel1.Controls.Add(Label19)
        FlowLayoutPanel1.Controls.Add(Me.CboDiskType)
        FlowLayoutPanel1.Location = New System.Drawing.Point(317, 3)
        FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        FlowLayoutPanel1.Size = New System.Drawing.Size(188, 27)
        FlowLayoutPanel1.TabIndex = 0
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
        'Label16
        '
        Label16.Anchor = System.Windows.Forms.AnchorStyles.Left
        Label16.AutoSize = True
        Label16.Location = New System.Drawing.Point(3, 84)
        Label16.Name = "Label16"
        Label16.Size = New System.Drawing.Size(74, 13)
        Label16.TabIndex = 11
        Label16.Text = "File System ID"
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
        FlowLayoutPanel3.Size = New System.Drawing.Size(508, 444)
        FlowLayoutPanel3.TabIndex = 4
        FlowLayoutPanel3.WrapContents = False
        '
        'GroupBoxMain
        '
        Me.GroupBoxMain.AutoSize = True
        Me.GroupBoxMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupBoxMain.Controls.Add(Me.TableLayoutPanelMain)
        Me.GroupBoxMain.Location = New System.Drawing.Point(3, 36)
        Me.GroupBoxMain.Name = "GroupBoxMain"
        Me.GroupBoxMain.Size = New System.Drawing.Size(502, 222)
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
        Me.TableLayoutPanelMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 132.0!))
        Me.TableLayoutPanelMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelMain.Controls.Add(Label2, 0, 1)
        Me.TableLayoutPanelMain.Controls.Add(Label4, 0, 2)
        Me.TableLayoutPanelMain.Controls.Add(Label6, 0, 3)
        Me.TableLayoutPanelMain.Controls.Add(Label1, 0, 0)
        Me.TableLayoutPanelMain.Controls.Add(Me.CboOEMName, 1, 0)
        Me.TableLayoutPanelMain.Controls.Add(Me.HexOEMName, 2, 0)
        Me.TableLayoutPanelMain.Controls.Add(Me.CboBytesPerSector, 1, 1)
        Me.TableLayoutPanelMain.Controls.Add(Label3, 2, 1)
        Me.TableLayoutPanelMain.Controls.Add(Me.CboSectorsPerCluster, 3, 1)
        Me.TableLayoutPanelMain.Controls.Add(Label13, 0, 6)
        Me.TableLayoutPanelMain.Controls.Add(Label8, 0, 4)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtReservedSectors, 1, 2)
        Me.TableLayoutPanelMain.Controls.Add(Label5, 2, 2)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtNumberOfFATs, 3, 2)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtRootDirectoryEntries, 1, 3)
        Me.TableLayoutPanelMain.Controls.Add(Label7, 2, 3)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtSectorCountSmall, 3, 3)
        Me.TableLayoutPanelMain.Controls.Add(Me.HexMediaDescriptor, 1, 4)
        Me.TableLayoutPanelMain.Controls.Add(Label9, 2, 4)
        Me.TableLayoutPanelMain.Controls.Add(Label10, 0, 5)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtSectorsPerFAT, 3, 4)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtSectorsPerTrack, 1, 5)
        Me.TableLayoutPanelMain.Controls.Add(Label11, 2, 5)
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
        Me.TableLayoutPanelMain.Size = New System.Drawing.Size(490, 184)
        Me.TableLayoutPanelMain.TabIndex = 0
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
        'CboBytesPerSector
        '
        Me.CboBytesPerSector.FormattingEnabled = True
        Me.CboBytesPerSector.Location = New System.Drawing.Point(141, 30)
        Me.CboBytesPerSector.MaxLength = 5
        Me.CboBytesPerSector.Name = "CboBytesPerSector"
        Me.CboBytesPerSector.Size = New System.Drawing.Size(104, 21)
        Me.CboBytesPerSector.TabIndex = 4
        '
        'CboSectorsPerCluster
        '
        Me.CboSectorsPerCluster.FormattingEnabled = True
        Me.CboSectorsPerCluster.Location = New System.Drawing.Point(383, 30)
        Me.CboSectorsPerCluster.MaxLength = 3
        Me.CboSectorsPerCluster.Name = "CboSectorsPerCluster"
        Me.CboSectorsPerCluster.Size = New System.Drawing.Size(104, 21)
        Me.CboSectorsPerCluster.TabIndex = 6
        '
        'TxtReservedSectors
        '
        Me.TxtReservedSectors.Location = New System.Drawing.Point(141, 57)
        Me.TxtReservedSectors.MaxLength = 3
        Me.TxtReservedSectors.Name = "TxtReservedSectors"
        Me.TxtReservedSectors.Size = New System.Drawing.Size(104, 20)
        Me.TxtReservedSectors.TabIndex = 8
        '
        'TxtNumberOfFATs
        '
        Me.TxtNumberOfFATs.Location = New System.Drawing.Point(383, 57)
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
        'TxtSectorCountSmall
        '
        Me.TxtSectorCountSmall.Location = New System.Drawing.Point(383, 83)
        Me.TxtSectorCountSmall.MaxLength = 5
        Me.TxtSectorCountSmall.Name = "TxtSectorCountSmall"
        Me.TxtSectorCountSmall.Size = New System.Drawing.Size(104, 20)
        Me.TxtSectorCountSmall.TabIndex = 14
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
        'TxtSectorsPerFAT
        '
        Me.TxtSectorsPerFAT.Location = New System.Drawing.Point(383, 109)
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
        'TxtNumberOfHeads
        '
        Me.TxtNumberOfHeads.Location = New System.Drawing.Point(383, 135)
        Me.TxtNumberOfHeads.MaxLength = 5
        Me.TxtNumberOfHeads.Name = "TxtNumberOfHeads"
        Me.TxtNumberOfHeads.Size = New System.Drawing.Size(104, 20)
        Me.TxtNumberOfHeads.TabIndex = 22
        '
        'TxtHiddenSectors
        '
        Me.TxtHiddenSectors.Location = New System.Drawing.Point(141, 161)
        Me.TxtHiddenSectors.MaxLength = 5
        Me.TxtHiddenSectors.Name = "TxtHiddenSectors"
        Me.TxtHiddenSectors.Size = New System.Drawing.Size(104, 20)
        Me.TxtHiddenSectors.TabIndex = 24
        '
        'GroupBoxExtended
        '
        Me.GroupBoxExtended.AutoSize = True
        Me.GroupBoxExtended.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupBoxExtended.Controls.Add(Me.TableLayoutPanel1)
        Me.GroupBoxExtended.Location = New System.Drawing.Point(3, 264)
        Me.GroupBoxExtended.Name = "GroupBoxExtended"
        Me.GroupBoxExtended.Size = New System.Drawing.Size(501, 142)
        Me.GroupBoxExtended.TabIndex = 2
        Me.GroupBoxExtended.TabStop = False
        Me.GroupBoxExtended.Text = "Extended Parameters"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 4
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 138.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.HexFileSystemType, 2, 3)
        Me.TableLayoutPanel1.Controls.Add(Label14, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TxtSectorCountLarge, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Label15, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.TxtDriveNumber, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Label12, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.TxtVolumeLabel, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.HexVolumeLabel, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Label16, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.TxtFileSystemType, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Label17, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.HexExtendedBootSignature, 3, 0)
        Me.TableLayoutPanel1.Controls.Add(Label18, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.HexVolumeSerialNumber, 3, 1)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(6, 19)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(489, 104)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'HexFileSystemType
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.HexFileSystemType, 2)
        Me.HexFileSystemType.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HexFileSystemType.Location = New System.Drawing.Point(251, 81)
        Me.HexFileSystemType.MaskLength = 8
        Me.HexFileSystemType.Name = "HexFileSystemType"
        Me.HexFileSystemType.Size = New System.Drawing.Size(170, 20)
        Me.HexFileSystemType.TabIndex = 13
        '
        'TxtSectorCountLarge
        '
        Me.TxtSectorCountLarge.Location = New System.Drawing.Point(141, 3)
        Me.TxtSectorCountLarge.MaxLength = 10
        Me.TxtSectorCountLarge.Name = "TxtSectorCountLarge"
        Me.TxtSectorCountLarge.Size = New System.Drawing.Size(104, 20)
        Me.TxtSectorCountLarge.TabIndex = 1
        '
        'TxtDriveNumber
        '
        Me.TxtDriveNumber.Location = New System.Drawing.Point(141, 29)
        Me.TxtDriveNumber.MaxLength = 3
        Me.TxtDriveNumber.Name = "TxtDriveNumber"
        Me.TxtDriveNumber.Size = New System.Drawing.Size(104, 20)
        Me.TxtDriveNumber.TabIndex = 5
        '
        'TxtVolumeLabel
        '
        Me.TxtVolumeLabel.Location = New System.Drawing.Point(141, 55)
        Me.TxtVolumeLabel.MaxLength = 11
        Me.TxtVolumeLabel.Name = "TxtVolumeLabel"
        Me.TxtVolumeLabel.Size = New System.Drawing.Size(104, 20)
        Me.TxtVolumeLabel.TabIndex = 9
        '
        'HexVolumeLabel
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.HexVolumeLabel, 2)
        Me.HexVolumeLabel.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HexVolumeLabel.Location = New System.Drawing.Point(251, 55)
        Me.HexVolumeLabel.MaskLength = 11
        Me.HexVolumeLabel.Name = "HexVolumeLabel"
        Me.HexVolumeLabel.Size = New System.Drawing.Size(234, 20)
        Me.HexVolumeLabel.TabIndex = 10
        '
        'TxtFileSystemType
        '
        Me.TxtFileSystemType.Location = New System.Drawing.Point(141, 81)
        Me.TxtFileSystemType.MaxLength = 8
        Me.TxtFileSystemType.Name = "TxtFileSystemType"
        Me.TxtFileSystemType.Size = New System.Drawing.Size(104, 20)
        Me.TxtFileSystemType.TabIndex = 12
        '
        'HexExtendedBootSignature
        '
        Me.HexExtendedBootSignature.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HexExtendedBootSignature.Location = New System.Drawing.Point(382, 3)
        Me.HexExtendedBootSignature.MaskLength = 1
        Me.HexExtendedBootSignature.Name = "HexExtendedBootSignature"
        Me.HexExtendedBootSignature.Size = New System.Drawing.Size(30, 20)
        Me.HexExtendedBootSignature.TabIndex = 3
        '
        'HexVolumeSerialNumber
        '
        Me.HexVolumeSerialNumber.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HexVolumeSerialNumber.Location = New System.Drawing.Point(382, 29)
        Me.HexVolumeSerialNumber.MaskLength = 4
        Me.HexVolumeSerialNumber.Name = "HexVolumeSerialNumber"
        Me.HexVolumeSerialNumber.Size = New System.Drawing.Size(104, 20)
        Me.HexVolumeSerialNumber.TabIndex = 7
        '
        'FlowLayoutPanel2
        '
        Me.FlowLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.FlowLayoutPanel2.AutoSize = True
        Me.FlowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel2.Controls.Add(Me.BtnUpdate)
        Me.FlowLayoutPanel2.Controls.Add(Me.BtnCancel)
        Me.FlowLayoutPanel2.Location = New System.Drawing.Point(158, 412)
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
        'BootSectorForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CancelButton = Me.BtnCancel
        Me.ClientSize = New System.Drawing.Size(573, 504)
        Me.Controls.Add(FlowLayoutPanel3)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
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
End Class
