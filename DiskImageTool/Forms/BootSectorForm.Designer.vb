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
        Dim PanelInner As System.Windows.Forms.FlowLayoutPanel
        Dim FlowLayoutPanel4 As System.Windows.Forms.FlowLayoutPanel
        Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
        Dim PanelMain As System.Windows.Forms.Panel
        Me.LblDiskType = New System.Windows.Forms.Label()
        Me.CboDiskType = New System.Windows.Forms.ComboBox()
        Me.btnReset = New System.Windows.Forms.Button()
        Me.GroupBoxMain = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanelMain = New System.Windows.Forms.TableLayoutPanel()
        Me.CboMediaDescriptor = New System.Windows.Forms.ComboBox()
        Me.HexJumpInstruction = New DiskImageTool.HexTextBox()
        Me.LblJumpInstruction = New System.Windows.Forms.Label()
        Me.LblBytesPerSector = New System.Windows.Forms.Label()
        Me.LblReservedSectors = New System.Windows.Forms.Label()
        Me.LblRootDirectoryEntries = New System.Windows.Forms.Label()
        Me.LblOEMName = New System.Windows.Forms.Label()
        Me.CboOEMName = New System.Windows.Forms.ComboBox()
        Me.HexOEMName = New DiskImageTool.HexTextBox()
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
        Me.LblBootSectorSignature = New System.Windows.Forms.Label()
        Me.HexBootSectorSignature = New DiskImageTool.HexTextBox()
        Me.GroupBoxExtended = New System.Windows.Forms.GroupBox()
        Me.LblExtendedMsg = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.BtnVolumeSerialNumber = New System.Windows.Forms.Button()
        Me.HexFileSystemType = New DiskImageTool.HexTextBox()
        Me.TxtDriveNumber = New System.Windows.Forms.TextBox()
        Me.LblDriveNumber = New System.Windows.Forms.Label()
        Me.LblSectorCountLarge = New System.Windows.Forms.Label()
        Me.TxtSectorCountLarge = New System.Windows.Forms.TextBox()
        Me.LblVolumeLabel = New System.Windows.Forms.Label()
        Me.TxtVolumeLabel = New System.Windows.Forms.TextBox()
        Me.HexVolumeLabel = New DiskImageTool.HexTextBox()
        Me.LblFileSystemType = New System.Windows.Forms.Label()
        Me.TxtFileSystemType = New System.Windows.Forms.TextBox()
        Me.LblVolumeSerialNumber = New System.Windows.Forms.Label()
        Me.HexVolumeSerialNumber = New DiskImageTool.HexTextBox()
        Me.lblExtendedBootSignature = New System.Windows.Forms.Label()
        Me.HexExtendedBootSignature = New DiskImageTool.HexTextBox()
        Me.GroupBoxAdditionalData = New System.Windows.Forms.GroupBox()
        Me.HexBox1 = New DiskImageTool.Hb.Windows.Forms.HexBox()
        Me.FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        PanelInner = New System.Windows.Forms.FlowLayoutPanel()
        FlowLayoutPanel4 = New System.Windows.Forms.FlowLayoutPanel()
        PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        PanelMain = New System.Windows.Forms.Panel()
        FlowLayoutPanel1.SuspendLayout()
        PanelInner.SuspendLayout()
        Me.GroupBoxMain.SuspendLayout()
        Me.TableLayoutPanelMain.SuspendLayout()
        Me.GroupBoxExtended.SuspendLayout()
        FlowLayoutPanel4.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupBoxAdditionalData.SuspendLayout()
        PanelBottom.SuspendLayout()
        PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'FlowLayoutPanel1
        '
        FlowLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        FlowLayoutPanel1.AutoSize = True
        FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanelMain.SetColumnSpan(FlowLayoutPanel1, 2)
        FlowLayoutPanel1.Controls.Add(Me.LblDiskType)
        FlowLayoutPanel1.Controls.Add(Me.CboDiskType)
        FlowLayoutPanel1.Controls.Add(Me.btnReset)
        FlowLayoutPanel1.Location = New System.Drawing.Point(248, 0)
        FlowLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
        FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        FlowLayoutPanel1.Size = New System.Drawing.Size(242, 28)
        FlowLayoutPanel1.TabIndex = 2
        '
        'LblDiskType
        '
        Me.LblDiskType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblDiskType.AutoSize = True
        Me.LblDiskType.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblDiskType.Location = New System.Drawing.Point(3, 7)
        Me.LblDiskType.Name = "LblDiskType"
        Me.LblDiskType.Size = New System.Drawing.Size(71, 13)
        Me.LblDiskType.TabIndex = 0
        Me.LblDiskType.Text = "{Disk Format}"
        '
        'CboDiskType
        '
        Me.CboDiskType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CboDiskType.FormattingEnabled = True
        Me.CboDiskType.Location = New System.Drawing.Point(80, 3)
        Me.CboDiskType.Name = "CboDiskType"
        Me.CboDiskType.Size = New System.Drawing.Size(130, 21)
        Me.CboDiskType.TabIndex = 1
        '
        'btnReset
        '
        Me.btnReset.Image = Global.DiskImageTool.My.Resources.Resources.Undo
        Me.btnReset.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnReset.Location = New System.Drawing.Point(216, 2)
        Me.btnReset.Margin = New System.Windows.Forms.Padding(3, 2, 3, 3)
        Me.btnReset.Name = "btnReset"
        Me.btnReset.Size = New System.Drawing.Size(23, 23)
        Me.btnReset.TabIndex = 2
        Me.btnReset.UseVisualStyleBackColor = True
        '
        'PanelInner
        '
        PanelInner.AutoSize = True
        PanelInner.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        PanelInner.Controls.Add(Me.GroupBoxMain)
        PanelInner.Controls.Add(Me.GroupBoxExtended)
        PanelInner.Controls.Add(Me.GroupBoxAdditionalData)
        PanelInner.Controls.Add(Me.FlowLayoutPanel2)
        PanelInner.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        PanelInner.Location = New System.Drawing.Point(18, 18)
        PanelInner.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        PanelInner.Name = "PanelInner"
        PanelInner.Size = New System.Drawing.Size(502, 503)
        PanelInner.TabIndex = 0
        PanelInner.WrapContents = False
        '
        'GroupBoxMain
        '
        Me.GroupBoxMain.AutoSize = True
        Me.GroupBoxMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupBoxMain.Controls.Add(Me.TableLayoutPanelMain)
        Me.GroupBoxMain.Location = New System.Drawing.Point(0, 3)
        Me.GroupBoxMain.Margin = New System.Windows.Forms.Padding(0, 3, 0, 3)
        Me.GroupBoxMain.Name = "GroupBoxMain"
        Me.GroupBoxMain.Padding = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.GroupBoxMain.Size = New System.Drawing.Size(502, 248)
        Me.GroupBoxMain.TabIndex = 1
        Me.GroupBoxMain.TabStop = False
        Me.GroupBoxMain.Text = "{Boot Record}"
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
        Me.TableLayoutPanelMain.Controls.Add(Me.CboMediaDescriptor, 1, 5)
        Me.TableLayoutPanelMain.Controls.Add(FlowLayoutPanel1, 2, 0)
        Me.TableLayoutPanelMain.Controls.Add(Me.HexJumpInstruction, 1, 0)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblJumpInstruction, 0, 0)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblBytesPerSector, 0, 2)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblReservedSectors, 0, 3)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblRootDirectoryEntries, 0, 4)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblOEMName, 0, 1)
        Me.TableLayoutPanelMain.Controls.Add(Me.CboOEMName, 1, 1)
        Me.TableLayoutPanelMain.Controls.Add(Me.HexOEMName, 2, 1)
        Me.TableLayoutPanelMain.Controls.Add(Me.CboBytesPerSector, 1, 2)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblSectorsPerCluster, 2, 2)
        Me.TableLayoutPanelMain.Controls.Add(Me.CboSectorsPerCluster, 3, 2)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblHiddenSectors, 0, 7)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtMediaDescriptor, 0, 5)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtReservedSectors, 1, 3)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblNumberOfFATS, 2, 3)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtNumberOfFATs, 3, 3)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtRootDirectoryEntries, 1, 4)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblSectorCountSmall, 2, 4)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtSectorCountSmall, 3, 4)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblSectorsPerFAT, 2, 5)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblSectorsPerTrack, 0, 6)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtSectorsPerFAT, 3, 5)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtSectorsPerTrack, 1, 6)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblNumberOfHeads, 2, 6)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtNumberOfHeads, 3, 6)
        Me.TableLayoutPanelMain.Controls.Add(Me.TxtHiddenSectors, 1, 7)
        Me.TableLayoutPanelMain.Controls.Add(Me.LblBootSectorSignature, 2, 7)
        Me.TableLayoutPanelMain.Controls.Add(Me.HexBootSectorSignature, 3, 7)
        Me.TableLayoutPanelMain.Location = New System.Drawing.Point(6, 19)
        Me.TableLayoutPanelMain.Name = "TableLayoutPanelMain"
        Me.TableLayoutPanelMain.RowCount = 8
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelMain.Size = New System.Drawing.Size(490, 213)
        Me.TableLayoutPanelMain.TabIndex = 0
        '
        'CboMediaDescriptor
        '
        Me.CboMediaDescriptor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.CboMediaDescriptor.FormattingEnabled = True
        Me.CboMediaDescriptor.Location = New System.Drawing.Point(141, 137)
        Me.CboMediaDescriptor.MaxLength = 2
        Me.CboMediaDescriptor.Name = "CboMediaDescriptor"
        Me.CboMediaDescriptor.Size = New System.Drawing.Size(104, 21)
        Me.CboMediaDescriptor.TabIndex = 19
        '
        'HexJumpInstruction
        '
        Me.HexJumpInstruction.Font = New System.Drawing.Font("Courier New", 8.25!)
        Me.HexJumpInstruction.Location = New System.Drawing.Point(141, 3)
        Me.HexJumpInstruction.MaskLength = 3
        Me.HexJumpInstruction.Name = "HexJumpInstruction"
        Me.HexJumpInstruction.Size = New System.Drawing.Size(69, 20)
        Me.HexJumpInstruction.TabIndex = 1
        '
        'LblJumpInstruction
        '
        Me.LblJumpInstruction.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblJumpInstruction.AutoSize = True
        Me.LblJumpInstruction.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblJumpInstruction.Location = New System.Drawing.Point(3, 7)
        Me.LblJumpInstruction.Name = "LblJumpInstruction"
        Me.LblJumpInstruction.Size = New System.Drawing.Size(92, 13)
        Me.LblJumpInstruction.TabIndex = 0
        Me.LblJumpInstruction.Text = "{Jump Instruction}"
        '
        'LblBytesPerSector
        '
        Me.LblBytesPerSector.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblBytesPerSector.AutoSize = True
        Me.LblBytesPerSector.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblBytesPerSector.Location = New System.Drawing.Point(3, 62)
        Me.LblBytesPerSector.Name = "LblBytesPerSector"
        Me.LblBytesPerSector.Size = New System.Drawing.Size(93, 13)
        Me.LblBytesPerSector.TabIndex = 6
        Me.LblBytesPerSector.Text = "{Bytes per Sector}"
        '
        'LblReservedSectors
        '
        Me.LblReservedSectors.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblReservedSectors.AutoSize = True
        Me.LblReservedSectors.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblReservedSectors.Location = New System.Drawing.Point(3, 88)
        Me.LblReservedSectors.Name = "LblReservedSectors"
        Me.LblReservedSectors.Size = New System.Drawing.Size(100, 13)
        Me.LblReservedSectors.TabIndex = 10
        Me.LblReservedSectors.Text = "{Reserved Sectors}"
        '
        'LblRootDirectoryEntries
        '
        Me.LblRootDirectoryEntries.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblRootDirectoryEntries.AutoSize = True
        Me.LblRootDirectoryEntries.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblRootDirectoryEntries.Location = New System.Drawing.Point(3, 114)
        Me.LblRootDirectoryEntries.Name = "LblRootDirectoryEntries"
        Me.LblRootDirectoryEntries.Size = New System.Drawing.Size(118, 13)
        Me.LblRootDirectoryEntries.TabIndex = 14
        Me.LblRootDirectoryEntries.Text = "{Root Directory Entries}"
        '
        'LblOEMName
        '
        Me.LblOEMName.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblOEMName.AutoSize = True
        Me.LblOEMName.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblOEMName.Location = New System.Drawing.Point(3, 35)
        Me.LblOEMName.Name = "LblOEMName"
        Me.LblOEMName.Size = New System.Drawing.Size(70, 13)
        Me.LblOEMName.TabIndex = 3
        Me.LblOEMName.Text = "{OEM Name}"
        '
        'CboOEMName
        '
        Me.CboOEMName.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.CboOEMName.FormattingEnabled = True
        Me.CboOEMName.Location = New System.Drawing.Point(141, 31)
        Me.CboOEMName.MaxLength = 8
        Me.CboOEMName.Name = "CboOEMName"
        Me.CboOEMName.Size = New System.Drawing.Size(104, 21)
        Me.CboOEMName.TabIndex = 4
        '
        'HexOEMName
        '
        Me.TableLayoutPanelMain.SetColumnSpan(Me.HexOEMName, 2)
        Me.HexOEMName.Font = New System.Drawing.Font("Courier New", 8.25!)
        Me.HexOEMName.Location = New System.Drawing.Point(251, 31)
        Me.HexOEMName.MaskLength = 8
        Me.HexOEMName.Name = "HexOEMName"
        Me.HexOEMName.Size = New System.Drawing.Size(174, 20)
        Me.HexOEMName.TabIndex = 5
        '
        'CboBytesPerSector
        '
        Me.CboBytesPerSector.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.CboBytesPerSector.FormattingEnabled = True
        Me.CboBytesPerSector.Location = New System.Drawing.Point(141, 58)
        Me.CboBytesPerSector.MaxLength = 5
        Me.CboBytesPerSector.Name = "CboBytesPerSector"
        Me.CboBytesPerSector.Size = New System.Drawing.Size(104, 21)
        Me.CboBytesPerSector.TabIndex = 7
        '
        'LblSectorsPerCluster
        '
        Me.LblSectorsPerCluster.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSectorsPerCluster.AutoSize = True
        Me.LblSectorsPerCluster.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblSectorsPerCluster.Location = New System.Drawing.Point(251, 62)
        Me.LblSectorsPerCluster.Name = "LblSectorsPerCluster"
        Me.LblSectorsPerCluster.Size = New System.Drawing.Size(104, 13)
        Me.LblSectorsPerCluster.TabIndex = 8
        Me.LblSectorsPerCluster.Text = "{Sectors per Cluster}"
        '
        'CboSectorsPerCluster
        '
        Me.CboSectorsPerCluster.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.CboSectorsPerCluster.FormattingEnabled = True
        Me.CboSectorsPerCluster.Location = New System.Drawing.Point(383, 58)
        Me.CboSectorsPerCluster.MaxLength = 3
        Me.CboSectorsPerCluster.Name = "CboSectorsPerCluster"
        Me.CboSectorsPerCluster.Size = New System.Drawing.Size(104, 21)
        Me.CboSectorsPerCluster.TabIndex = 9
        '
        'LblHiddenSectors
        '
        Me.LblHiddenSectors.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblHiddenSectors.AutoSize = True
        Me.LblHiddenSectors.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblHiddenSectors.Location = New System.Drawing.Point(3, 193)
        Me.LblHiddenSectors.Name = "LblHiddenSectors"
        Me.LblHiddenSectors.Size = New System.Drawing.Size(88, 13)
        Me.LblHiddenSectors.TabIndex = 26
        Me.LblHiddenSectors.Text = "{Hidden Sectors}"
        '
        'TxtMediaDescriptor
        '
        Me.TxtMediaDescriptor.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtMediaDescriptor.AutoSize = True
        Me.TxtMediaDescriptor.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.TxtMediaDescriptor.Location = New System.Drawing.Point(3, 141)
        Me.TxtMediaDescriptor.Name = "TxtMediaDescriptor"
        Me.TxtMediaDescriptor.Size = New System.Drawing.Size(95, 13)
        Me.TxtMediaDescriptor.TabIndex = 18
        Me.TxtMediaDescriptor.Text = "{Media Descriptor}"
        '
        'TxtReservedSectors
        '
        Me.TxtReservedSectors.Location = New System.Drawing.Point(141, 85)
        Me.TxtReservedSectors.MaxLength = 3
        Me.TxtReservedSectors.Name = "TxtReservedSectors"
        Me.TxtReservedSectors.Size = New System.Drawing.Size(104, 20)
        Me.TxtReservedSectors.TabIndex = 11
        '
        'LblNumberOfFATS
        '
        Me.LblNumberOfFATS.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblNumberOfFATS.AutoSize = True
        Me.LblNumberOfFATS.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblNumberOfFATS.Location = New System.Drawing.Point(251, 88)
        Me.LblNumberOfFATS.Name = "LblNumberOfFATS"
        Me.LblNumberOfFATS.Size = New System.Drawing.Size(92, 13)
        Me.LblNumberOfFATS.TabIndex = 12
        Me.LblNumberOfFATS.Text = "{Number of FATs}"
        '
        'TxtNumberOfFATs
        '
        Me.TxtNumberOfFATs.Location = New System.Drawing.Point(383, 85)
        Me.TxtNumberOfFATs.MaxLength = 3
        Me.TxtNumberOfFATs.Name = "TxtNumberOfFATs"
        Me.TxtNumberOfFATs.Size = New System.Drawing.Size(104, 20)
        Me.TxtNumberOfFATs.TabIndex = 13
        '
        'TxtRootDirectoryEntries
        '
        Me.TxtRootDirectoryEntries.Location = New System.Drawing.Point(141, 111)
        Me.TxtRootDirectoryEntries.MaxLength = 5
        Me.TxtRootDirectoryEntries.Name = "TxtRootDirectoryEntries"
        Me.TxtRootDirectoryEntries.Size = New System.Drawing.Size(104, 20)
        Me.TxtRootDirectoryEntries.TabIndex = 15
        '
        'LblSectorCountSmall
        '
        Me.LblSectorCountSmall.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSectorCountSmall.AutoSize = True
        Me.LblSectorCountSmall.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblSectorCountSmall.Location = New System.Drawing.Point(251, 114)
        Me.LblSectorCountSmall.Name = "LblSectorCountSmall"
        Me.LblSectorCountSmall.Size = New System.Drawing.Size(104, 13)
        Me.LblSectorCountSmall.TabIndex = 16
        Me.LblSectorCountSmall.Text = "{Total Sector Count}"
        '
        'TxtSectorCountSmall
        '
        Me.TxtSectorCountSmall.Location = New System.Drawing.Point(383, 111)
        Me.TxtSectorCountSmall.MaxLength = 5
        Me.TxtSectorCountSmall.Name = "TxtSectorCountSmall"
        Me.TxtSectorCountSmall.Size = New System.Drawing.Size(104, 20)
        Me.TxtSectorCountSmall.TabIndex = 17
        '
        'LblSectorsPerFAT
        '
        Me.LblSectorsPerFAT.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSectorsPerFAT.AutoSize = True
        Me.LblSectorsPerFAT.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblSectorsPerFAT.Location = New System.Drawing.Point(251, 141)
        Me.LblSectorsPerFAT.Name = "LblSectorsPerFAT"
        Me.LblSectorsPerFAT.Size = New System.Drawing.Size(92, 13)
        Me.LblSectorsPerFAT.TabIndex = 20
        Me.LblSectorsPerFAT.Text = "{Sectors per FAT}"
        '
        'LblSectorsPerTrack
        '
        Me.LblSectorsPerTrack.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSectorsPerTrack.AutoSize = True
        Me.LblSectorsPerTrack.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblSectorsPerTrack.Location = New System.Drawing.Point(3, 167)
        Me.LblSectorsPerTrack.Name = "LblSectorsPerTrack"
        Me.LblSectorsPerTrack.Size = New System.Drawing.Size(100, 13)
        Me.LblSectorsPerTrack.TabIndex = 22
        Me.LblSectorsPerTrack.Text = "{Sectors per Track}"
        '
        'TxtSectorsPerFAT
        '
        Me.TxtSectorsPerFAT.Location = New System.Drawing.Point(383, 137)
        Me.TxtSectorsPerFAT.MaxLength = 5
        Me.TxtSectorsPerFAT.Name = "TxtSectorsPerFAT"
        Me.TxtSectorsPerFAT.Size = New System.Drawing.Size(104, 20)
        Me.TxtSectorsPerFAT.TabIndex = 21
        '
        'TxtSectorsPerTrack
        '
        Me.TxtSectorsPerTrack.Location = New System.Drawing.Point(141, 164)
        Me.TxtSectorsPerTrack.MaxLength = 5
        Me.TxtSectorsPerTrack.Name = "TxtSectorsPerTrack"
        Me.TxtSectorsPerTrack.Size = New System.Drawing.Size(104, 20)
        Me.TxtSectorsPerTrack.TabIndex = 23
        '
        'LblNumberOfHeads
        '
        Me.LblNumberOfHeads.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblNumberOfHeads.AutoSize = True
        Me.LblNumberOfHeads.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblNumberOfHeads.Location = New System.Drawing.Point(251, 167)
        Me.LblNumberOfHeads.Name = "LblNumberOfHeads"
        Me.LblNumberOfHeads.Size = New System.Drawing.Size(98, 13)
        Me.LblNumberOfHeads.TabIndex = 24
        Me.LblNumberOfHeads.Text = "{Number of Heads}"
        '
        'TxtNumberOfHeads
        '
        Me.TxtNumberOfHeads.Location = New System.Drawing.Point(383, 164)
        Me.TxtNumberOfHeads.MaxLength = 5
        Me.TxtNumberOfHeads.Name = "TxtNumberOfHeads"
        Me.TxtNumberOfHeads.Size = New System.Drawing.Size(104, 20)
        Me.TxtNumberOfHeads.TabIndex = 25
        '
        'TxtHiddenSectors
        '
        Me.TxtHiddenSectors.Location = New System.Drawing.Point(141, 190)
        Me.TxtHiddenSectors.MaxLength = 5
        Me.TxtHiddenSectors.Name = "TxtHiddenSectors"
        Me.TxtHiddenSectors.Size = New System.Drawing.Size(104, 20)
        Me.TxtHiddenSectors.TabIndex = 27
        '
        'LblBootSectorSignature
        '
        Me.LblBootSectorSignature.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblBootSectorSignature.AutoSize = True
        Me.LblBootSectorSignature.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblBootSectorSignature.Location = New System.Drawing.Point(251, 193)
        Me.LblBootSectorSignature.Name = "LblBootSectorSignature"
        Me.LblBootSectorSignature.Size = New System.Drawing.Size(119, 13)
        Me.LblBootSectorSignature.TabIndex = 28
        Me.LblBootSectorSignature.Text = "{Boot Sector Signature}"
        '
        'HexBootSectorSignature
        '
        Me.HexBootSectorSignature.Font = New System.Drawing.Font("Courier New", 8.25!)
        Me.HexBootSectorSignature.Location = New System.Drawing.Point(383, 190)
        Me.HexBootSectorSignature.MaskLength = 2
        Me.HexBootSectorSignature.Name = "HexBootSectorSignature"
        Me.HexBootSectorSignature.Size = New System.Drawing.Size(49, 20)
        Me.HexBootSectorSignature.TabIndex = 29
        '
        'GroupBoxExtended
        '
        Me.GroupBoxExtended.AutoSize = True
        Me.GroupBoxExtended.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupBoxExtended.Controls.Add(FlowLayoutPanel4)
        Me.GroupBoxExtended.Location = New System.Drawing.Point(0, 257)
        Me.GroupBoxExtended.Margin = New System.Windows.Forms.Padding(0, 3, 0, 3)
        Me.GroupBoxExtended.Name = "GroupBoxExtended"
        Me.GroupBoxExtended.Padding = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.GroupBoxExtended.Size = New System.Drawing.Size(502, 175)
        Me.GroupBoxExtended.TabIndex = 2
        Me.GroupBoxExtended.TabStop = False
        Me.GroupBoxExtended.Text = "{Extended Parameter Block}"
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
        FlowLayoutPanel4.Size = New System.Drawing.Size(490, 140)
        FlowLayoutPanel4.TabIndex = 1
        '
        'LblExtendedMsg
        '
        Me.LblExtendedMsg.ForeColor = System.Drawing.Color.Blue
        Me.LblExtendedMsg.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblExtendedMsg.Location = New System.Drawing.Point(3, 0)
        Me.LblExtendedMsg.Margin = New System.Windows.Forms.Padding(3, 0, 3, 10)
        Me.LblExtendedMsg.Name = "LblExtendedMsg"
        Me.LblExtendedMsg.Size = New System.Drawing.Size(484, 26)
        Me.LblExtendedMsg.TabIndex = 0
        Me.LblExtendedMsg.Text = "{Message}"
        Me.LblExtendedMsg.UseMnemonic = False
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 5
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 138.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 122.0!))
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
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 36)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(490, 104)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'BtnVolumeSerialNumber
        '
        Me.BtnVolumeSerialNumber.AutoSize = True
        Me.BtnVolumeSerialNumber.FlatAppearance.BorderSize = 0
        Me.BtnVolumeSerialNumber.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnVolumeSerialNumber.Image = Global.DiskImageTool.My.Resources.Resources.VolumeSerialNumber
        Me.BtnVolumeSerialNumber.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.BtnVolumeSerialNumber.Location = New System.Drawing.Point(465, 26)
        Me.BtnVolumeSerialNumber.Margin = New System.Windows.Forms.Padding(0)
        Me.BtnVolumeSerialNumber.Name = "BtnVolumeSerialNumber"
        Me.BtnVolumeSerialNumber.Size = New System.Drawing.Size(24, 24)
        Me.BtnVolumeSerialNumber.TabIndex = 8
        Me.BtnVolumeSerialNumber.UseVisualStyleBackColor = True
        '
        'HexFileSystemType
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.HexFileSystemType, 3)
        Me.HexFileSystemType.Font = New System.Drawing.Font("Courier New", 8.25!)
        Me.HexFileSystemType.Location = New System.Drawing.Point(251, 81)
        Me.HexFileSystemType.MaskLength = 8
        Me.HexFileSystemType.Name = "HexFileSystemType"
        Me.HexFileSystemType.Size = New System.Drawing.Size(174, 20)
        Me.HexFileSystemType.TabIndex = 14
        '
        'TxtDriveNumber
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.TxtDriveNumber, 2)
        Me.TxtDriveNumber.Location = New System.Drawing.Point(373, 3)
        Me.TxtDriveNumber.MaxLength = 3
        Me.TxtDriveNumber.Name = "TxtDriveNumber"
        Me.TxtDriveNumber.Size = New System.Drawing.Size(114, 20)
        Me.TxtDriveNumber.TabIndex = 3
        '
        'LblDriveNumber
        '
        Me.LblDriveNumber.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblDriveNumber.AutoSize = True
        Me.LblDriveNumber.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblDriveNumber.Location = New System.Drawing.Point(251, 6)
        Me.LblDriveNumber.Name = "LblDriveNumber"
        Me.LblDriveNumber.Size = New System.Drawing.Size(80, 13)
        Me.LblDriveNumber.TabIndex = 2
        Me.LblDriveNumber.Text = "{Drive Number}"
        '
        'LblSectorCountLarge
        '
        Me.LblSectorCountLarge.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSectorCountLarge.AutoSize = True
        Me.LblSectorCountLarge.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblSectorCountLarge.Location = New System.Drawing.Point(3, 0)
        Me.LblSectorCountLarge.Name = "LblSectorCountLarge"
        Me.LblSectorCountLarge.Size = New System.Drawing.Size(103, 26)
        Me.LblSectorCountLarge.TabIndex = 0
        Me.LblSectorCountLarge.Text = "{Total Sector Count (Large)}"
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
        Me.LblVolumeLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblVolumeLabel.Location = New System.Drawing.Point(3, 58)
        Me.LblVolumeLabel.Name = "LblVolumeLabel"
        Me.LblVolumeLabel.Size = New System.Drawing.Size(79, 13)
        Me.LblVolumeLabel.TabIndex = 9
        Me.LblVolumeLabel.Text = "{Volume Label}"
        '
        'TxtVolumeLabel
        '
        Me.TxtVolumeLabel.Location = New System.Drawing.Point(141, 55)
        Me.TxtVolumeLabel.MaxLength = 11
        Me.TxtVolumeLabel.Name = "TxtVolumeLabel"
        Me.TxtVolumeLabel.Size = New System.Drawing.Size(104, 20)
        Me.TxtVolumeLabel.TabIndex = 10
        '
        'HexVolumeLabel
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.HexVolumeLabel, 3)
        Me.HexVolumeLabel.Font = New System.Drawing.Font("Courier New", 8.25!)
        Me.HexVolumeLabel.Location = New System.Drawing.Point(251, 55)
        Me.HexVolumeLabel.MaskLength = 11
        Me.HexVolumeLabel.Name = "HexVolumeLabel"
        Me.HexVolumeLabel.Size = New System.Drawing.Size(236, 20)
        Me.HexVolumeLabel.TabIndex = 11
        '
        'LblFileSystemType
        '
        Me.LblFileSystemType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblFileSystemType.AutoSize = True
        Me.LblFileSystemType.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblFileSystemType.Location = New System.Drawing.Point(3, 84)
        Me.LblFileSystemType.Name = "LblFileSystemType"
        Me.LblFileSystemType.Size = New System.Drawing.Size(82, 13)
        Me.LblFileSystemType.TabIndex = 12
        Me.LblFileSystemType.Text = "{File System ID}"
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
        Me.LblVolumeSerialNumber.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.LblVolumeSerialNumber.Location = New System.Drawing.Point(251, 26)
        Me.LblVolumeSerialNumber.Name = "LblVolumeSerialNumber"
        Me.LblVolumeSerialNumber.Size = New System.Drawing.Size(78, 26)
        Me.LblVolumeSerialNumber.TabIndex = 6
        Me.LblVolumeSerialNumber.Text = "{Volume Serial Number}"
        '
        'HexVolumeSerialNumber
        '
        Me.HexVolumeSerialNumber.Font = New System.Drawing.Font("Courier New", 8.25!)
        Me.HexVolumeSerialNumber.Location = New System.Drawing.Point(373, 29)
        Me.HexVolumeSerialNumber.MaskLength = 4
        Me.HexVolumeSerialNumber.Name = "HexVolumeSerialNumber"
        Me.HexVolumeSerialNumber.Size = New System.Drawing.Size(89, 20)
        Me.HexVolumeSerialNumber.TabIndex = 7
        '
        'lblExtendedBootSignature
        '
        Me.lblExtendedBootSignature.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lblExtendedBootSignature.AutoSize = True
        Me.lblExtendedBootSignature.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblExtendedBootSignature.Location = New System.Drawing.Point(3, 26)
        Me.lblExtendedBootSignature.Name = "lblExtendedBootSignature"
        Me.lblExtendedBootSignature.Size = New System.Drawing.Size(84, 26)
        Me.lblExtendedBootSignature.TabIndex = 4
        Me.lblExtendedBootSignature.Text = "{Extended Boot Signature}"
        '
        'HexExtendedBootSignature
        '
        Me.HexExtendedBootSignature.Font = New System.Drawing.Font("Courier New", 8.25!)
        Me.HexExtendedBootSignature.Location = New System.Drawing.Point(141, 29)
        Me.HexExtendedBootSignature.MaskLength = 1
        Me.HexExtendedBootSignature.Name = "HexExtendedBootSignature"
        Me.HexExtendedBootSignature.Size = New System.Drawing.Size(30, 20)
        Me.HexExtendedBootSignature.TabIndex = 5
        '
        'GroupBoxAdditionalData
        '
        Me.GroupBoxAdditionalData.AutoSize = True
        Me.GroupBoxAdditionalData.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupBoxAdditionalData.Controls.Add(Me.HexBox1)
        Me.GroupBoxAdditionalData.Location = New System.Drawing.Point(0, 438)
        Me.GroupBoxAdditionalData.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
        Me.GroupBoxAdditionalData.Name = "GroupBoxAdditionalData"
        Me.GroupBoxAdditionalData.Padding = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.GroupBoxAdditionalData.Size = New System.Drawing.Size(502, 56)
        Me.GroupBoxAdditionalData.TabIndex = 3
        Me.GroupBoxAdditionalData.TabStop = False
        Me.GroupBoxAdditionalData.Text = "{Additional Data}"
        '
        'HexBox1
        '
        '
        '
        '
        Me.HexBox1.BuiltInContextMenu.CopyMenuItemImage = Nothing
        Me.HexBox1.BuiltInContextMenu.CopyMenuItemText = "{Copy Text}"
        Me.HexBox1.BuiltInContextMenu.CutMenuItemImage = Nothing
        Me.HexBox1.BuiltInContextMenu.CutMenuItemText = Nothing
        Me.HexBox1.BuiltInContextMenu.PasteMenuItemImage = Nothing
        Me.HexBox1.BuiltInContextMenu.PasteMenuItemText = Nothing
        Me.HexBox1.BuiltInContextMenu.SelectAllMenuItemImage = Nothing
        Me.HexBox1.BuiltInContextMenu.SelectAllMenuItemText = "{Select All}"
        Me.HexBox1.Font = New System.Drawing.Font("Courier New", 8.25!)
        Me.HexBox1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.HexBox1.HexViewTextColor = System.Drawing.SystemColors.ControlText
        Me.HexBox1.Location = New System.Drawing.Point(6, 19)
        Me.HexBox1.Name = "HexBox1"
        Me.HexBox1.ReadOnly = True
        Me.HexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(188, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.HexBox1.Size = New System.Drawing.Size(490, 21)
        Me.HexBox1.StringViewVisible = True
        Me.HexBox1.TabIndex = 0
        Me.HexBox1.UseFixedBytesPerLine = True
        Me.HexBox1.VScrollBarVisible = False
        '
        'FlowLayoutPanel2
        '
        Me.FlowLayoutPanel2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel2.AutoSize = True
        Me.FlowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel2.Location = New System.Drawing.Point(502, 503)
        Me.FlowLayoutPanel2.Margin = New System.Windows.Forms.Padding(0, 9, 0, 0)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        Me.FlowLayoutPanel2.Size = New System.Drawing.Size(0, 0)
        Me.FlowLayoutPanel2.TabIndex = 4
        '
        'PanelBottom
        '
        PanelBottom.Controls.Add(Me.BtnCancel)
        PanelBottom.Controls.Add(Me.BtnUpdate)
        PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        PanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        PanelBottom.Location = New System.Drawing.Point(0, 562)
        PanelBottom.Name = "PanelBottom"
        PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
        PanelBottom.Size = New System.Drawing.Size(555, 43)
        PanelBottom.TabIndex = 1
        PanelBottom.WrapContents = False
        '
        'BtnCancel
        '
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.BtnCancel.Location = New System.Drawing.Point(462, 10)
        Me.BtnCancel.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "{&Cancel}"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnUpdate
        '
        Me.BtnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnUpdate.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.BtnUpdate.Location = New System.Drawing.Point(375, 10)
        Me.BtnUpdate.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.Size = New System.Drawing.Size(75, 23)
        Me.BtnUpdate.TabIndex = 0
        Me.BtnUpdate.Text = "{&Update}"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'PanelMain
        '
        PanelMain.AutoSize = True
        PanelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        PanelMain.Controls.Add(PanelInner)
        PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        PanelMain.Location = New System.Drawing.Point(0, 0)
        PanelMain.Name = "PanelMain"
        PanelMain.Padding = New System.Windows.Forms.Padding(18, 18, 18, 0)
        PanelMain.Size = New System.Drawing.Size(555, 562)
        PanelMain.TabIndex = 0
        '
        'BootSectorForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ClientSize = New System.Drawing.Size(555, 605)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.HelpButton = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "BootSectorForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        FlowLayoutPanel1.ResumeLayout(False)
        FlowLayoutPanel1.PerformLayout()
        PanelInner.ResumeLayout(False)
        PanelInner.PerformLayout()
        Me.GroupBoxMain.ResumeLayout(False)
        Me.GroupBoxMain.PerformLayout()
        Me.TableLayoutPanelMain.ResumeLayout(False)
        Me.TableLayoutPanelMain.PerformLayout()
        Me.GroupBoxExtended.ResumeLayout(False)
        Me.GroupBoxExtended.PerformLayout()
        FlowLayoutPanel4.ResumeLayout(False)
        FlowLayoutPanel4.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.GroupBoxAdditionalData.ResumeLayout(False)
        PanelBottom.ResumeLayout(False)
        PanelMain.ResumeLayout(False)
        PanelMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents TableLayoutPanelMain As TableLayoutPanel
    Friend WithEvents HexOEMName As HexTextBox
    Friend WithEvents CboOEMName As ComboBox
    Friend WithEvents HexVolumeSerialNumber As HexTextBox
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
    Friend WithEvents HexBox1 As Hb.Windows.Forms.HexBox
    Friend WithEvents GroupBoxAdditionalData As GroupBox
    Friend WithEvents HexJumpInstruction As HexTextBox
    Friend WithEvents LblJumpInstruction As Label
    Friend WithEvents LblBootSectorSignature As Label
    Friend WithEvents HexBootSectorSignature As HexTextBox
    Friend WithEvents btnReset As Button
    Friend WithEvents CboMediaDescriptor As ComboBox
End Class
