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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BootSectorForm))
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
        resources.ApplyResources(FlowLayoutPanel1, "FlowLayoutPanel1")
        Me.TableLayoutPanelMain.SetColumnSpan(FlowLayoutPanel1, 2)
        FlowLayoutPanel1.Controls.Add(Me.LblDiskType)
        FlowLayoutPanel1.Controls.Add(Me.CboDiskType)
        FlowLayoutPanel1.Controls.Add(Me.btnReset)
        FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        '
        'LblDiskType
        '
        resources.ApplyResources(Me.LblDiskType, "LblDiskType")
        Me.LblDiskType.Name = "LblDiskType"
        '
        'CboDiskType
        '
        Me.CboDiskType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CboDiskType.FormattingEnabled = True
        resources.ApplyResources(Me.CboDiskType, "CboDiskType")
        Me.CboDiskType.Name = "CboDiskType"
        '
        'btnReset
        '
        Me.btnReset.Image = Global.DiskImageTool.My.Resources.Resources.Undo
        resources.ApplyResources(Me.btnReset, "btnReset")
        Me.btnReset.Name = "btnReset"
        Me.btnReset.UseVisualStyleBackColor = True
        '
        'PanelInner
        '
        resources.ApplyResources(PanelInner, "PanelInner")
        PanelInner.Controls.Add(Me.GroupBoxMain)
        PanelInner.Controls.Add(Me.GroupBoxExtended)
        PanelInner.Controls.Add(Me.GroupBoxAdditionalData)
        PanelInner.Controls.Add(Me.FlowLayoutPanel2)
        PanelInner.Name = "PanelInner"
        '
        'GroupBoxMain
        '
        resources.ApplyResources(Me.GroupBoxMain, "GroupBoxMain")
        Me.GroupBoxMain.Controls.Add(Me.TableLayoutPanelMain)
        Me.GroupBoxMain.Name = "GroupBoxMain"
        Me.GroupBoxMain.TabStop = False
        '
        'TableLayoutPanelMain
        '
        resources.ApplyResources(Me.TableLayoutPanelMain, "TableLayoutPanelMain")
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
        Me.TableLayoutPanelMain.Name = "TableLayoutPanelMain"
        '
        'CboMediaDescriptor
        '
        Me.CboMediaDescriptor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.CboMediaDescriptor.FormattingEnabled = True
        resources.ApplyResources(Me.CboMediaDescriptor, "CboMediaDescriptor")
        Me.CboMediaDescriptor.Name = "CboMediaDescriptor"
        '
        'HexJumpInstruction
        '
        resources.ApplyResources(Me.HexJumpInstruction, "HexJumpInstruction")
        Me.HexJumpInstruction.MaskLength = 3
        Me.HexJumpInstruction.Name = "HexJumpInstruction"
        '
        'LblJumpInstruction
        '
        resources.ApplyResources(Me.LblJumpInstruction, "LblJumpInstruction")
        Me.LblJumpInstruction.Name = "LblJumpInstruction"
        '
        'LblBytesPerSector
        '
        resources.ApplyResources(Me.LblBytesPerSector, "LblBytesPerSector")
        Me.LblBytesPerSector.Name = "LblBytesPerSector"
        '
        'LblReservedSectors
        '
        resources.ApplyResources(Me.LblReservedSectors, "LblReservedSectors")
        Me.LblReservedSectors.Name = "LblReservedSectors"
        '
        'LblRootDirectoryEntries
        '
        resources.ApplyResources(Me.LblRootDirectoryEntries, "LblRootDirectoryEntries")
        Me.LblRootDirectoryEntries.Name = "LblRootDirectoryEntries"
        '
        'LblOEMName
        '
        resources.ApplyResources(Me.LblOEMName, "LblOEMName")
        Me.LblOEMName.Name = "LblOEMName"
        '
        'CboOEMName
        '
        resources.ApplyResources(Me.CboOEMName, "CboOEMName")
        Me.CboOEMName.FormattingEnabled = True
        Me.CboOEMName.Name = "CboOEMName"
        '
        'HexOEMName
        '
        Me.TableLayoutPanelMain.SetColumnSpan(Me.HexOEMName, 2)
        resources.ApplyResources(Me.HexOEMName, "HexOEMName")
        Me.HexOEMName.MaskLength = 8
        Me.HexOEMName.Name = "HexOEMName"
        '
        'CboBytesPerSector
        '
        Me.CboBytesPerSector.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.CboBytesPerSector.FormattingEnabled = True
        resources.ApplyResources(Me.CboBytesPerSector, "CboBytesPerSector")
        Me.CboBytesPerSector.Name = "CboBytesPerSector"
        '
        'LblSectorsPerCluster
        '
        resources.ApplyResources(Me.LblSectorsPerCluster, "LblSectorsPerCluster")
        Me.LblSectorsPerCluster.Name = "LblSectorsPerCluster"
        '
        'CboSectorsPerCluster
        '
        Me.CboSectorsPerCluster.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.CboSectorsPerCluster.FormattingEnabled = True
        resources.ApplyResources(Me.CboSectorsPerCluster, "CboSectorsPerCluster")
        Me.CboSectorsPerCluster.Name = "CboSectorsPerCluster"
        '
        'LblHiddenSectors
        '
        resources.ApplyResources(Me.LblHiddenSectors, "LblHiddenSectors")
        Me.LblHiddenSectors.Name = "LblHiddenSectors"
        '
        'TxtMediaDescriptor
        '
        resources.ApplyResources(Me.TxtMediaDescriptor, "TxtMediaDescriptor")
        Me.TxtMediaDescriptor.Name = "TxtMediaDescriptor"
        '
        'TxtReservedSectors
        '
        resources.ApplyResources(Me.TxtReservedSectors, "TxtReservedSectors")
        Me.TxtReservedSectors.Name = "TxtReservedSectors"
        '
        'LblNumberOfFATS
        '
        resources.ApplyResources(Me.LblNumberOfFATS, "LblNumberOfFATS")
        Me.LblNumberOfFATS.Name = "LblNumberOfFATS"
        '
        'TxtNumberOfFATs
        '
        resources.ApplyResources(Me.TxtNumberOfFATs, "TxtNumberOfFATs")
        Me.TxtNumberOfFATs.Name = "TxtNumberOfFATs"
        '
        'TxtRootDirectoryEntries
        '
        resources.ApplyResources(Me.TxtRootDirectoryEntries, "TxtRootDirectoryEntries")
        Me.TxtRootDirectoryEntries.Name = "TxtRootDirectoryEntries"
        '
        'LblSectorCountSmall
        '
        resources.ApplyResources(Me.LblSectorCountSmall, "LblSectorCountSmall")
        Me.LblSectorCountSmall.Name = "LblSectorCountSmall"
        '
        'TxtSectorCountSmall
        '
        resources.ApplyResources(Me.TxtSectorCountSmall, "TxtSectorCountSmall")
        Me.TxtSectorCountSmall.Name = "TxtSectorCountSmall"
        '
        'LblSectorsPerFAT
        '
        resources.ApplyResources(Me.LblSectorsPerFAT, "LblSectorsPerFAT")
        Me.LblSectorsPerFAT.Name = "LblSectorsPerFAT"
        '
        'LblSectorsPerTrack
        '
        resources.ApplyResources(Me.LblSectorsPerTrack, "LblSectorsPerTrack")
        Me.LblSectorsPerTrack.Name = "LblSectorsPerTrack"
        '
        'TxtSectorsPerFAT
        '
        resources.ApplyResources(Me.TxtSectorsPerFAT, "TxtSectorsPerFAT")
        Me.TxtSectorsPerFAT.Name = "TxtSectorsPerFAT"
        '
        'TxtSectorsPerTrack
        '
        resources.ApplyResources(Me.TxtSectorsPerTrack, "TxtSectorsPerTrack")
        Me.TxtSectorsPerTrack.Name = "TxtSectorsPerTrack"
        '
        'LblNumberOfHeads
        '
        resources.ApplyResources(Me.LblNumberOfHeads, "LblNumberOfHeads")
        Me.LblNumberOfHeads.Name = "LblNumberOfHeads"
        '
        'TxtNumberOfHeads
        '
        resources.ApplyResources(Me.TxtNumberOfHeads, "TxtNumberOfHeads")
        Me.TxtNumberOfHeads.Name = "TxtNumberOfHeads"
        '
        'TxtHiddenSectors
        '
        resources.ApplyResources(Me.TxtHiddenSectors, "TxtHiddenSectors")
        Me.TxtHiddenSectors.Name = "TxtHiddenSectors"
        '
        'LblBootSectorSignature
        '
        resources.ApplyResources(Me.LblBootSectorSignature, "LblBootSectorSignature")
        Me.LblBootSectorSignature.Name = "LblBootSectorSignature"
        '
        'HexBootSectorSignature
        '
        resources.ApplyResources(Me.HexBootSectorSignature, "HexBootSectorSignature")
        Me.HexBootSectorSignature.MaskLength = 2
        Me.HexBootSectorSignature.Name = "HexBootSectorSignature"
        '
        'GroupBoxExtended
        '
        resources.ApplyResources(Me.GroupBoxExtended, "GroupBoxExtended")
        Me.GroupBoxExtended.Controls.Add(FlowLayoutPanel4)
        Me.GroupBoxExtended.Name = "GroupBoxExtended"
        Me.GroupBoxExtended.TabStop = False
        '
        'FlowLayoutPanel4
        '
        resources.ApplyResources(FlowLayoutPanel4, "FlowLayoutPanel4")
        FlowLayoutPanel4.Controls.Add(Me.LblExtendedMsg)
        FlowLayoutPanel4.Controls.Add(Me.TableLayoutPanel1)
        FlowLayoutPanel4.Name = "FlowLayoutPanel4"
        '
        'LblExtendedMsg
        '
        Me.LblExtendedMsg.ForeColor = System.Drawing.Color.Blue
        resources.ApplyResources(Me.LblExtendedMsg, "LblExtendedMsg")
        Me.LblExtendedMsg.Name = "LblExtendedMsg"
        Me.LblExtendedMsg.UseMnemonic = False
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
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
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'BtnVolumeSerialNumber
        '
        resources.ApplyResources(Me.BtnVolumeSerialNumber, "BtnVolumeSerialNumber")
        Me.BtnVolumeSerialNumber.FlatAppearance.BorderSize = 0
        Me.BtnVolumeSerialNumber.Image = Global.DiskImageTool.My.Resources.Resources.VolumeSerialNumber
        Me.BtnVolumeSerialNumber.Name = "BtnVolumeSerialNumber"
        Me.BtnVolumeSerialNumber.UseVisualStyleBackColor = True
        '
        'HexFileSystemType
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.HexFileSystemType, 3)
        resources.ApplyResources(Me.HexFileSystemType, "HexFileSystemType")
        Me.HexFileSystemType.MaskLength = 8
        Me.HexFileSystemType.Name = "HexFileSystemType"
        '
        'TxtDriveNumber
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.TxtDriveNumber, 2)
        resources.ApplyResources(Me.TxtDriveNumber, "TxtDriveNumber")
        Me.TxtDriveNumber.Name = "TxtDriveNumber"
        '
        'LblDriveNumber
        '
        resources.ApplyResources(Me.LblDriveNumber, "LblDriveNumber")
        Me.LblDriveNumber.Name = "LblDriveNumber"
        '
        'LblSectorCountLarge
        '
        resources.ApplyResources(Me.LblSectorCountLarge, "LblSectorCountLarge")
        Me.LblSectorCountLarge.Name = "LblSectorCountLarge"
        '
        'TxtSectorCountLarge
        '
        resources.ApplyResources(Me.TxtSectorCountLarge, "TxtSectorCountLarge")
        Me.TxtSectorCountLarge.Name = "TxtSectorCountLarge"
        '
        'LblVolumeLabel
        '
        resources.ApplyResources(Me.LblVolumeLabel, "LblVolumeLabel")
        Me.LblVolumeLabel.Name = "LblVolumeLabel"
        '
        'TxtVolumeLabel
        '
        resources.ApplyResources(Me.TxtVolumeLabel, "TxtVolumeLabel")
        Me.TxtVolumeLabel.Name = "TxtVolumeLabel"
        '
        'HexVolumeLabel
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.HexVolumeLabel, 3)
        resources.ApplyResources(Me.HexVolumeLabel, "HexVolumeLabel")
        Me.HexVolumeLabel.MaskLength = 11
        Me.HexVolumeLabel.Name = "HexVolumeLabel"
        '
        'LblFileSystemType
        '
        resources.ApplyResources(Me.LblFileSystemType, "LblFileSystemType")
        Me.LblFileSystemType.Name = "LblFileSystemType"
        '
        'TxtFileSystemType
        '
        resources.ApplyResources(Me.TxtFileSystemType, "TxtFileSystemType")
        Me.TxtFileSystemType.Name = "TxtFileSystemType"
        '
        'LblVolumeSerialNumber
        '
        resources.ApplyResources(Me.LblVolumeSerialNumber, "LblVolumeSerialNumber")
        Me.LblVolumeSerialNumber.Name = "LblVolumeSerialNumber"
        '
        'HexVolumeSerialNumber
        '
        resources.ApplyResources(Me.HexVolumeSerialNumber, "HexVolumeSerialNumber")
        Me.HexVolumeSerialNumber.MaskLength = 4
        Me.HexVolumeSerialNumber.Name = "HexVolumeSerialNumber"
        '
        'lblExtendedBootSignature
        '
        resources.ApplyResources(Me.lblExtendedBootSignature, "lblExtendedBootSignature")
        Me.lblExtendedBootSignature.Name = "lblExtendedBootSignature"
        '
        'HexExtendedBootSignature
        '
        resources.ApplyResources(Me.HexExtendedBootSignature, "HexExtendedBootSignature")
        Me.HexExtendedBootSignature.MaskLength = 1
        Me.HexExtendedBootSignature.Name = "HexExtendedBootSignature"
        '
        'GroupBoxAdditionalData
        '
        resources.ApplyResources(Me.GroupBoxAdditionalData, "GroupBoxAdditionalData")
        Me.GroupBoxAdditionalData.Controls.Add(Me.HexBox1)
        Me.GroupBoxAdditionalData.Name = "GroupBoxAdditionalData"
        Me.GroupBoxAdditionalData.TabStop = False
        '
        'HexBox1
        '
        '
        '
        '
        Me.HexBox1.BuiltInContextMenu.CopyMenuItemImage = Nothing
        Me.HexBox1.BuiltInContextMenu.CopyMenuItemText = resources.GetString("HexBox1.BuiltInContextMenu.CopyMenuItemText")
        Me.HexBox1.BuiltInContextMenu.CutMenuItemImage = Nothing
        Me.HexBox1.BuiltInContextMenu.CutMenuItemText = resources.GetString("HexBox1.BuiltInContextMenu.CutMenuItemText")
        Me.HexBox1.BuiltInContextMenu.PasteMenuItemImage = Nothing
        Me.HexBox1.BuiltInContextMenu.PasteMenuItemText = resources.GetString("HexBox1.BuiltInContextMenu.PasteMenuItemText")
        Me.HexBox1.BuiltInContextMenu.SelectAllMenuItemImage = Nothing
        Me.HexBox1.BuiltInContextMenu.SelectAllMenuItemText = resources.GetString("HexBox1.BuiltInContextMenu.SelectAllMenuItemText")
        resources.ApplyResources(Me.HexBox1, "HexBox1")
        Me.HexBox1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.HexBox1.HexViewTextColor = System.Drawing.SystemColors.ControlText
        Me.HexBox1.Name = "HexBox1"
        Me.HexBox1.ReadOnly = True
        Me.HexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(188, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.HexBox1.StringViewVisible = True
        Me.HexBox1.UseFixedBytesPerLine = True
        Me.HexBox1.VScrollBarVisible = False
        '
        'FlowLayoutPanel2
        '
        resources.ApplyResources(Me.FlowLayoutPanel2, "FlowLayoutPanel2")
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        '
        'PanelBottom
        '
        PanelBottom.Controls.Add(Me.BtnCancel)
        PanelBottom.Controls.Add(Me.BtnUpdate)
        resources.ApplyResources(PanelBottom, "PanelBottom")
        PanelBottom.Name = "PanelBottom"
        '
        'BtnCancel
        '
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnUpdate
        '
        Me.BtnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK
        resources.ApplyResources(Me.BtnUpdate, "BtnUpdate")
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'PanelMain
        '
        resources.ApplyResources(PanelMain, "PanelMain")
        PanelMain.Controls.Add(PanelInner)
        PanelMain.Name = "PanelMain"
        '
        'BootSectorForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.HelpButton = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "BootSectorForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
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
