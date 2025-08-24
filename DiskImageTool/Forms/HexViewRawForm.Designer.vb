<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class HexViewRawForm
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
        Dim ToolStripStatusGap As System.Windows.Forms.ToolStripStatusLabel
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HexViewRawForm))
        Dim ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripStatusLabel2 As System.Windows.Forms.ToolStripStatusLabel
        Me.DataGridDataInspector = New System.Windows.Forms.DataGridView()
        Me.ContextMenuStrip2 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnCopyValue = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStripBottom = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusOffset = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusBlock = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLength = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusTrack = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSide = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusTrackSector = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusTrackSize = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusChecksumText = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusChecksum = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusBits = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusBytes = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnCopyHex = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnCopyText = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnCopyHexFormatted = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnCopyEncoded = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFind = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFindNext = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnSelectAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnSelectRegion = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnSelectSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnSelectData = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnAdjustOffset = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripBtnCopyText = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnCopyHex = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnCopyHexFormatted = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnFind = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnFindNext = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripBtnSelectAll = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripMain = New System.Windows.Forms.ToolStrip()
        Me.ToolStripBtnSelectRegion = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnSelectSector = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnSelectData = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnAdjustOffset = New System.Windows.Forms.ToolStripButton()
        Me.PanelSectors = New DiskImageTool.SelectablePanel()
        Me.HexBox1 = New DiskImageTool.Hb.Windows.Forms.HexBox()
        ToolStripStatusGap = New System.Windows.Forms.ToolStripStatusLabel()
        ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        CType(Me.DataGridDataInspector, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip2.SuspendLayout()
        Me.StatusStripBottom.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.ToolStripMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStripStatusGap
        '
        ToolStripStatusGap.Name = "ToolStripStatusGap"
        resources.ApplyResources(ToolStripStatusGap, "ToolStripStatusGap")
        ToolStripStatusGap.Spring = True
        '
        'ToolStripSeparator2
        '
        ToolStripSeparator2.Name = "ToolStripSeparator2"
        resources.ApplyResources(ToolStripSeparator2, "ToolStripSeparator2")
        '
        'ToolStripSeparator5
        '
        ToolStripSeparator5.Name = "ToolStripSeparator5"
        resources.ApplyResources(ToolStripSeparator5, "ToolStripSeparator5")
        '
        'ToolStripStatusLabel2
        '
        ToolStripStatusLabel2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        ToolStripStatusLabel2.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        ToolStripStatusLabel2.Padding = New System.Windows.Forms.Padding(16, 0, 0, 0)
        resources.ApplyResources(ToolStripStatusLabel2, "ToolStripStatusLabel2")
        '
        'DataGridDataInspector
        '
        resources.ApplyResources(Me.DataGridDataInspector, "DataGridDataInspector")
        Me.DataGridDataInspector.BackgroundColor = System.Drawing.SystemColors.Window
        Me.DataGridDataInspector.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DataGridDataInspector.ContextMenuStrip = Me.ContextMenuStrip2
        Me.DataGridDataInspector.GridColor = System.Drawing.SystemColors.ControlLight
        Me.DataGridDataInspector.Name = "DataGridDataInspector"
        '
        'ContextMenuStrip2
        '
        Me.ContextMenuStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnCopyValue})
        Me.ContextMenuStrip2.Name = "ContextMenuStrip2"
        resources.ApplyResources(Me.ContextMenuStrip2, "ContextMenuStrip2")
        '
        'BtnCopyValue
        '
        Me.BtnCopyValue.Name = "BtnCopyValue"
        resources.ApplyResources(Me.BtnCopyValue, "BtnCopyValue")
        '
        'StatusStripBottom
        '
        Me.StatusStripBottom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusOffset, Me.ToolStripStatusBlock, Me.ToolStripStatusLength, ToolStripStatusGap, Me.ToolStripStatusTrack, Me.ToolStripStatusSide, Me.ToolStripStatusTrackSector, Me.ToolStripStatusTrackSize, Me.ToolStripStatusChecksumText, Me.ToolStripStatusChecksum, ToolStripStatusLabel2, Me.ToolStripStatusBits, Me.ToolStripStatusBytes})
        resources.ApplyResources(Me.StatusStripBottom, "StatusStripBottom")
        Me.StatusStripBottom.Name = "StatusStripBottom"
        '
        'ToolStripStatusOffset
        '
        Me.ToolStripStatusOffset.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
        Me.ToolStripStatusOffset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusOffset.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusOffset.Name = "ToolStripStatusOffset"
        resources.ApplyResources(Me.ToolStripStatusOffset, "ToolStripStatusOffset")
        '
        'ToolStripStatusBlock
        '
        Me.ToolStripStatusBlock.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
        Me.ToolStripStatusBlock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusBlock.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusBlock.Name = "ToolStripStatusBlock"
        resources.ApplyResources(Me.ToolStripStatusBlock, "ToolStripStatusBlock")
        '
        'ToolStripStatusLength
        '
        Me.ToolStripStatusLength.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
        Me.ToolStripStatusLength.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusLength.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusLength.Name = "ToolStripStatusLength"
        resources.ApplyResources(Me.ToolStripStatusLength, "ToolStripStatusLength")
        '
        'ToolStripStatusTrack
        '
        Me.ToolStripStatusTrack.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusTrack.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusTrack.Name = "ToolStripStatusTrack"
        resources.ApplyResources(Me.ToolStripStatusTrack, "ToolStripStatusTrack")
        '
        'ToolStripStatusSide
        '
        Me.ToolStripStatusSide.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusSide.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusSide.Name = "ToolStripStatusSide"
        resources.ApplyResources(Me.ToolStripStatusSide, "ToolStripStatusSide")
        '
        'ToolStripStatusTrackSector
        '
        Me.ToolStripStatusTrackSector.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusTrackSector.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusTrackSector.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusTrackSector.Name = "ToolStripStatusTrackSector"
        resources.ApplyResources(Me.ToolStripStatusTrackSector, "ToolStripStatusTrackSector")
        '
        'ToolStripStatusTrackSize
        '
        Me.ToolStripStatusTrackSize.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusTrackSize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusTrackSize.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusTrackSize.Name = "ToolStripStatusTrackSize"
        resources.ApplyResources(Me.ToolStripStatusTrackSize, "ToolStripStatusTrackSize")
        '
        'ToolStripStatusChecksumText
        '
        Me.ToolStripStatusChecksumText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusChecksumText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusChecksumText.Margin = New System.Windows.Forms.Padding(2, 3, 0, 2)
        Me.ToolStripStatusChecksumText.Name = "ToolStripStatusChecksumText"
        resources.ApplyResources(Me.ToolStripStatusChecksumText, "ToolStripStatusChecksumText")
        '
        'ToolStripStatusChecksum
        '
        Me.ToolStripStatusChecksum.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusChecksum.Margin = New System.Windows.Forms.Padding(0, 3, 2, 2)
        Me.ToolStripStatusChecksum.Name = "ToolStripStatusChecksum"
        resources.ApplyResources(Me.ToolStripStatusChecksum, "ToolStripStatusChecksum")
        '
        'ToolStripStatusBits
        '
        Me.ToolStripStatusBits.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusBits.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusBits.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusBits.Name = "ToolStripStatusBits"
        resources.ApplyResources(Me.ToolStripStatusBits, "ToolStripStatusBits")
        '
        'ToolStripStatusBytes
        '
        Me.ToolStripStatusBytes.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusBytes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusBytes.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusBytes.Name = "ToolStripStatusBytes"
        resources.ApplyResources(Me.ToolStripStatusBytes, "ToolStripStatusBytes")
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnCopyHex, Me.BtnCopyText, Me.BtnCopyHexFormatted, Me.BtnCopyEncoded, ToolStripSeparator2, Me.BtnFind, Me.BtnFindNext, Me.ToolStripMenuItem1, Me.BtnSelectAll, Me.BtnSelectRegion, Me.BtnSelectSector, Me.BtnSelectData, Me.ToolStripSeparator1, Me.BtnAdjustOffset})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
        '
        'BtnCopyHex
        '
        Me.BtnCopyHex.Image = Global.DiskImageTool.My.Resources.Resources.Copy
        Me.BtnCopyHex.Name = "BtnCopyHex"
        resources.ApplyResources(Me.BtnCopyHex, "BtnCopyHex")
        '
        'BtnCopyText
        '
        Me.BtnCopyText.Image = Global.DiskImageTool.My.Resources.Resources.TextBlock
        Me.BtnCopyText.Name = "BtnCopyText"
        resources.ApplyResources(Me.BtnCopyText, "BtnCopyText")
        '
        'BtnCopyHexFormatted
        '
        Me.BtnCopyHexFormatted.Image = Global.DiskImageTool.My.Resources.Resources.CopyHexFormatted
        Me.BtnCopyHexFormatted.Name = "BtnCopyHexFormatted"
        resources.ApplyResources(Me.BtnCopyHexFormatted, "BtnCopyHexFormatted")
        '
        'BtnCopyEncoded
        '
        Me.BtnCopyEncoded.Name = "BtnCopyEncoded"
        resources.ApplyResources(Me.BtnCopyEncoded, "BtnCopyEncoded")
        '
        'BtnFind
        '
        Me.BtnFind.Name = "BtnFind"
        resources.ApplyResources(Me.BtnFind, "BtnFind")
        '
        'BtnFindNext
        '
        Me.BtnFindNext.Name = "BtnFindNext"
        resources.ApplyResources(Me.BtnFindNext, "BtnFindNext")
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        resources.ApplyResources(Me.ToolStripMenuItem1, "ToolStripMenuItem1")
        '
        'BtnSelectAll
        '
        Me.BtnSelectAll.Image = Global.DiskImageTool.My.Resources.Resources.SelectAll
        Me.BtnSelectAll.Name = "BtnSelectAll"
        resources.ApplyResources(Me.BtnSelectAll, "BtnSelectAll")
        '
        'BtnSelectRegion
        '
        Me.BtnSelectRegion.Image = Global.DiskImageTool.My.Resources.Resources.SelectSector
        Me.BtnSelectRegion.Name = "BtnSelectRegion"
        resources.ApplyResources(Me.BtnSelectRegion, "BtnSelectRegion")
        '
        'BtnSelectSector
        '
        Me.BtnSelectSector.Image = Global.DiskImageTool.My.Resources.Resources.SelectSector
        Me.BtnSelectSector.Name = "BtnSelectSector"
        resources.ApplyResources(Me.BtnSelectSector, "BtnSelectSector")
        '
        'BtnSelectData
        '
        Me.BtnSelectData.Image = Global.DiskImageTool.My.Resources.Resources.SelectSector
        Me.BtnSelectData.Name = "BtnSelectData"
        resources.ApplyResources(Me.BtnSelectData, "BtnSelectData")
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'BtnAdjustOffset
        '
        Me.BtnAdjustOffset.Name = "BtnAdjustOffset"
        resources.ApplyResources(Me.BtnAdjustOffset, "BtnAdjustOffset")
        '
        'ToolStripBtnCopyText
        '
        Me.ToolStripBtnCopyText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnCopyText.Image = Global.DiskImageTool.My.Resources.Resources.TextBlock
        resources.ApplyResources(Me.ToolStripBtnCopyText, "ToolStripBtnCopyText")
        Me.ToolStripBtnCopyText.Name = "ToolStripBtnCopyText"
        '
        'ToolStripBtnCopyHex
        '
        Me.ToolStripBtnCopyHex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnCopyHex.Image = Global.DiskImageTool.My.Resources.Resources.Copy
        resources.ApplyResources(Me.ToolStripBtnCopyHex, "ToolStripBtnCopyHex")
        Me.ToolStripBtnCopyHex.Name = "ToolStripBtnCopyHex"
        '
        'ToolStripBtnCopyHexFormatted
        '
        Me.ToolStripBtnCopyHexFormatted.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnCopyHexFormatted.Image = Global.DiskImageTool.My.Resources.Resources.CopyHexFormatted
        resources.ApplyResources(Me.ToolStripBtnCopyHexFormatted, "ToolStripBtnCopyHexFormatted")
        Me.ToolStripBtnCopyHexFormatted.Name = "ToolStripBtnCopyHexFormatted"
        '
        'ToolStripBtnFind
        '
        Me.ToolStripBtnFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnFind.Image = Global.DiskImageTool.My.Resources.Resources.Search
        resources.ApplyResources(Me.ToolStripBtnFind, "ToolStripBtnFind")
        Me.ToolStripBtnFind.Name = "ToolStripBtnFind"
        '
        'ToolStripBtnFindNext
        '
        Me.ToolStripBtnFindNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnFindNext.Image = Global.DiskImageTool.My.Resources.Resources.FindNext
        resources.ApplyResources(Me.ToolStripBtnFindNext, "ToolStripBtnFindNext")
        Me.ToolStripBtnFindNext.Name = "ToolStripBtnFindNext"
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        resources.ApplyResources(Me.ToolStripSeparator8, "ToolStripSeparator8")
        '
        'ToolStripBtnSelectAll
        '
        Me.ToolStripBtnSelectAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnSelectAll.Image = Global.DiskImageTool.My.Resources.Resources.SelectAll
        resources.ApplyResources(Me.ToolStripBtnSelectAll, "ToolStripBtnSelectAll")
        Me.ToolStripBtnSelectAll.Name = "ToolStripBtnSelectAll"
        '
        'ToolStripMain
        '
        Me.ToolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStripMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtnCopyText, Me.ToolStripBtnCopyHex, Me.ToolStripBtnCopyHexFormatted, ToolStripSeparator5, Me.ToolStripBtnFind, Me.ToolStripBtnFindNext, Me.ToolStripSeparator8, Me.ToolStripBtnSelectAll, Me.ToolStripBtnSelectRegion, Me.ToolStripBtnSelectSector, Me.ToolStripBtnSelectData, Me.ToolStripBtnAdjustOffset})
        resources.ApplyResources(Me.ToolStripMain, "ToolStripMain")
        Me.ToolStripMain.Name = "ToolStripMain"
        '
        'ToolStripBtnSelectRegion
        '
        Me.ToolStripBtnSelectRegion.Image = Global.DiskImageTool.My.Resources.Resources.SelectSector
        resources.ApplyResources(Me.ToolStripBtnSelectRegion, "ToolStripBtnSelectRegion")
        Me.ToolStripBtnSelectRegion.Name = "ToolStripBtnSelectRegion"
        '
        'ToolStripBtnSelectSector
        '
        Me.ToolStripBtnSelectSector.Image = Global.DiskImageTool.My.Resources.Resources.SelectSector
        resources.ApplyResources(Me.ToolStripBtnSelectSector, "ToolStripBtnSelectSector")
        Me.ToolStripBtnSelectSector.Name = "ToolStripBtnSelectSector"
        '
        'ToolStripBtnSelectData
        '
        Me.ToolStripBtnSelectData.Image = Global.DiskImageTool.My.Resources.Resources.SelectSector
        resources.ApplyResources(Me.ToolStripBtnSelectData, "ToolStripBtnSelectData")
        Me.ToolStripBtnSelectData.Name = "ToolStripBtnSelectData"
        '
        'ToolStripBtnAdjustOffset
        '
        Me.ToolStripBtnAdjustOffset.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripBtnAdjustOffset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnAdjustOffset.Image = Global.DiskImageTool.My.Resources.Resources.AdjustOffset
        resources.ApplyResources(Me.ToolStripBtnAdjustOffset, "ToolStripBtnAdjustOffset")
        Me.ToolStripBtnAdjustOffset.Margin = New System.Windows.Forms.Padding(6, 1, 0, 2)
        Me.ToolStripBtnAdjustOffset.Name = "ToolStripBtnAdjustOffset"
        '
        'PanelSectors
        '
        resources.ApplyResources(Me.PanelSectors, "PanelSectors")
        Me.PanelSectors.Name = "PanelSectors"
        Me.PanelSectors.TabStop = True
        '
        'HexBox1
        '
        resources.ApplyResources(Me.HexBox1, "HexBox1")
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
        Me.HexBox1.ColumnInfoVisible = True
        Me.HexBox1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.HexBox1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.HexBox1.HexViewTextColor = System.Drawing.SystemColors.ControlText
        Me.HexBox1.LineInfoVisible = True
        Me.HexBox1.Name = "HexBox1"
        Me.HexBox1.ReadOnly = True
        Me.HexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(188, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.HexBox1.StringViewVisible = True
        Me.HexBox1.UseFixedBytesPerLine = True
        Me.HexBox1.VScrollBarVisible = False
        '
        'HexViewRawForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.PanelSectors)
        Me.Controls.Add(Me.ToolStripMain)
        Me.Controls.Add(Me.DataGridDataInspector)
        Me.Controls.Add(Me.StatusStripBottom)
        Me.Controls.Add(Me.HexBox1)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "HexViewRawForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.DataGridDataInspector, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip2.ResumeLayout(False)
        Me.StatusStripBottom.ResumeLayout(False)
        Me.StatusStripBottom.PerformLayout()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ToolStripMain.ResumeLayout(False)
        Me.ToolStripMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents DataGridDataInspector As DataGridView
    Friend WithEvents StatusStripBottom As StatusStrip
    Friend WithEvents ToolStripStatusOffset As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusBlock As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLength As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusBytes As ToolStripStatusLabel
    Friend WithEvents HexBox1 As Hb.Windows.Forms.HexBox
    Friend WithEvents ContextMenuStrip2 As ContextMenuStrip
    Friend WithEvents BtnCopyValue As ToolStripMenuItem
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents BtnCopyText As ToolStripMenuItem
    Friend WithEvents BtnCopyHex As ToolStripMenuItem
    Friend WithEvents BtnCopyHexFormatted As ToolStripMenuItem
    Friend WithEvents BtnFind As ToolStripMenuItem
    Friend WithEvents BtnFindNext As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents BtnSelectAll As ToolStripMenuItem
    Friend WithEvents ToolStripBtnCopyText As ToolStripButton
    Friend WithEvents ToolStripBtnCopyHex As ToolStripButton
    Friend WithEvents ToolStripBtnCopyHexFormatted As ToolStripButton
    Friend WithEvents ToolStripBtnFind As ToolStripButton
    Friend WithEvents ToolStripBtnFindNext As ToolStripButton
    Friend WithEvents ToolStripSeparator8 As ToolStripSeparator
    Friend WithEvents ToolStripBtnSelectAll As ToolStripButton
    Friend WithEvents ToolStripMain As ToolStrip
    Friend WithEvents ToolStripBtnSelectRegion As ToolStripButton
    Friend WithEvents BtnSelectRegion As ToolStripMenuItem
    Friend WithEvents BtnSelectSector As ToolStripMenuItem
    Friend WithEvents ToolStripBtnSelectSector As ToolStripButton
    Friend WithEvents ToolStripStatusTrack As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusSide As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusTrackSector As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusTrackSize As ToolStripStatusLabel
    Friend WithEvents BtnSelectData As ToolStripMenuItem
    Friend WithEvents ToolStripStatusChecksumText As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusChecksum As ToolStripStatusLabel
    Friend WithEvents ToolStripBtnSelectData As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents BtnAdjustOffset As ToolStripMenuItem
    Friend WithEvents ToolStripBtnAdjustOffset As ToolStripButton
    Friend WithEvents ToolStripStatusBits As ToolStripStatusLabel
    Friend WithEvents PanelSectors As DiskImageTool.SelectablePanel
    Friend WithEvents BtnCopyEncoded As ToolStripMenuItem
End Class
