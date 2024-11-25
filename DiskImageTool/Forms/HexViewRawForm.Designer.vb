﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
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
        Dim ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripStatusLabel2 As System.Windows.Forms.ToolStripStatusLabel
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HexViewRawForm))
        Me.DataGridDataInspector = New System.Windows.Forms.DataGridView()
        Me.DataGridName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridValue = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridLength = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridInvalid = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridEditable = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridType = New System.Windows.Forms.DataGridViewTextBoxColumn()
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
        Me.HexBox1 = New Hb.Windows.Forms.HexBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnCopyText = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnCopyHex = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnCopyHexFormatted = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.PanelSectors = New DiskImageTool.SelectablePanel
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
        ToolStripStatusGap.Size = New System.Drawing.Size(258, 19)
        ToolStripStatusGap.Spring = True
        '
        'ToolStripSeparator2
        '
        ToolStripSeparator2.Name = "ToolStripSeparator2"
        ToolStripSeparator2.Size = New System.Drawing.Size(255, 6)
        '
        'ToolStripSeparator5
        '
        ToolStripSeparator5.Name = "ToolStripSeparator5"
        ToolStripSeparator5.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripStatusLabel2
        '
        ToolStripStatusLabel2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        ToolStripStatusLabel2.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        ToolStripStatusLabel2.Padding = New System.Windows.Forms.Padding(16, 0, 0, 0)
        ToolStripStatusLabel2.Size = New System.Drawing.Size(20, 19)
        '
        'DataGridDataInspector
        '
        Me.DataGridDataInspector.AllowUserToAddRows = False
        Me.DataGridDataInspector.AllowUserToDeleteRows = False
        Me.DataGridDataInspector.AllowUserToResizeColumns = False
        Me.DataGridDataInspector.AllowUserToResizeRows = False
        Me.DataGridDataInspector.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridDataInspector.BackgroundColor = System.Drawing.SystemColors.Window
        Me.DataGridDataInspector.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DataGridDataInspector.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable
        Me.DataGridDataInspector.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.DataGridDataInspector.ColumnHeadersVisible = False
        Me.DataGridDataInspector.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridName, Me.DataGridValue, Me.DataGridLength, Me.DataGridInvalid, Me.DataGridEditable, Me.DataGridType})
        Me.DataGridDataInspector.ContextMenuStrip = Me.ContextMenuStrip2
        Me.DataGridDataInspector.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2
        Me.DataGridDataInspector.GridColor = System.Drawing.SystemColors.ControlLight
        Me.DataGridDataInspector.Location = New System.Drawing.Point(668, 53)
        Me.DataGridDataInspector.MultiSelect = False
        Me.DataGridDataInspector.Name = "DataGridDataInspector"
        Me.DataGridDataInspector.RowHeadersVisible = False
        Me.DataGridDataInspector.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DataGridDataInspector.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.DataGridDataInspector.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridDataInspector.Size = New System.Drawing.Size(264, 493)
        Me.DataGridDataInspector.StandardTab = True
        Me.DataGridDataInspector.TabIndex = 3
        '
        'DataGridName
        '
        Me.DataGridName.HeaderText = "Name"
        Me.DataGridName.Name = "DataGridName"
        Me.DataGridName.ReadOnly = True
        Me.DataGridName.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'DataGridValue
        '
        Me.DataGridValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.DataGridValue.HeaderText = "Value"
        Me.DataGridValue.Name = "DataGridValue"
        Me.DataGridValue.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'DataGridLength
        '
        Me.DataGridLength.HeaderText = "Length"
        Me.DataGridLength.Name = "DataGridLength"
        Me.DataGridLength.ReadOnly = True
        Me.DataGridLength.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridLength.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.DataGridLength.Visible = False
        '
        'DataGridInvalid
        '
        Me.DataGridInvalid.HeaderText = "Invalid"
        Me.DataGridInvalid.Name = "DataGridInvalid"
        Me.DataGridInvalid.ReadOnly = True
        Me.DataGridInvalid.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridInvalid.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.DataGridInvalid.Visible = False
        '
        'DataGridEditable
        '
        Me.DataGridEditable.HeaderText = "Editable"
        Me.DataGridEditable.Name = "DataGridEditable"
        Me.DataGridEditable.ReadOnly = True
        Me.DataGridEditable.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridEditable.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.DataGridEditable.Visible = False
        '
        'DataGridType
        '
        Me.DataGridType.HeaderText = "Type"
        Me.DataGridType.Name = "DataGridType"
        Me.DataGridType.ReadOnly = True
        Me.DataGridType.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.DataGridType.Visible = False
        '
        'ContextMenuStrip2
        '
        Me.ContextMenuStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnCopyValue})
        Me.ContextMenuStrip2.Name = "ContextMenuStrip2"
        Me.ContextMenuStrip2.Size = New System.Drawing.Size(176, 26)
        '
        'BtnCopyValue
        '
        Me.BtnCopyValue.Name = "BtnCopyValue"
        Me.BtnCopyValue.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.BtnCopyValue.Size = New System.Drawing.Size(175, 22)
        Me.BtnCopyValue.Text = "&Copy Value"
        '
        'StatusStripBottom
        '
        Me.StatusStripBottom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusOffset, Me.ToolStripStatusBlock, Me.ToolStripStatusLength, ToolStripStatusGap, Me.ToolStripStatusTrack, Me.ToolStripStatusSide, Me.ToolStripStatusTrackSector, Me.ToolStripStatusTrackSize, Me.ToolStripStatusChecksumText, Me.ToolStripStatusChecksum, ToolStripStatusLabel2, Me.ToolStripStatusBits, Me.ToolStripStatusBytes})
        Me.StatusStripBottom.Location = New System.Drawing.Point(0, 549)
        Me.StatusStripBottom.Name = "StatusStripBottom"
        Me.StatusStripBottom.Size = New System.Drawing.Size(944, 24)
        Me.StatusStripBottom.TabIndex = 4
        Me.StatusStripBottom.Text = "StatusStrip1"
        '
        'ToolStripStatusOffset
        '
        Me.ToolStripStatusOffset.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
        Me.ToolStripStatusOffset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusOffset.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusOffset.Name = "ToolStripStatusOffset"
        Me.ToolStripStatusOffset.Size = New System.Drawing.Size(70, 19)
        Me.ToolStripStatusOffset.Text = "Offset(h): 0"
        Me.ToolStripStatusOffset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToolStripStatusBlock
        '
        Me.ToolStripStatusBlock.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
        Me.ToolStripStatusBlock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusBlock.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusBlock.Name = "ToolStripStatusBlock"
        Me.ToolStripStatusBlock.Size = New System.Drawing.Size(78, 19)
        Me.ToolStripStatusBlock.Text = "Block(h): 0-0"
        Me.ToolStripStatusBlock.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToolStripStatusLength
        '
        Me.ToolStripStatusLength.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
        Me.ToolStripStatusLength.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusLength.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusLength.Name = "ToolStripStatusLength"
        Me.ToolStripStatusLength.Size = New System.Drawing.Size(75, 19)
        Me.ToolStripStatusLength.Text = "Length(h): 0"
        Me.ToolStripStatusLength.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToolStripStatusTrack
        '
        Me.ToolStripStatusTrack.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusTrack.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusTrack.Name = "ToolStripStatusTrack"
        Me.ToolStripStatusTrack.Size = New System.Drawing.Size(47, 19)
        Me.ToolStripStatusTrack.Text = "Track 0"
        Me.ToolStripStatusTrack.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ToolStripStatusSide
        '
        Me.ToolStripStatusSide.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusSide.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusSide.Name = "ToolStripStatusSide"
        Me.ToolStripStatusSide.Size = New System.Drawing.Size(42, 19)
        Me.ToolStripStatusSide.Text = "Side 0"
        Me.ToolStripStatusSide.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ToolStripStatusTrackSector
        '
        Me.ToolStripStatusTrackSector.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusTrackSector.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusTrackSector.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusTrackSector.Name = "ToolStripStatusTrackSector"
        Me.ToolStripStatusTrackSector.Size = New System.Drawing.Size(66, 19)
        Me.ToolStripStatusTrackSector.Text = "Sector Id 0"
        Me.ToolStripStatusTrackSector.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ToolStripStatusTrackSize
        '
        Me.ToolStripStatusTrackSize.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusTrackSize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusTrackSize.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusTrackSize.Name = "ToolStripStatusTrackSize"
        Me.ToolStripStatusTrackSize.Size = New System.Drawing.Size(40, 19)
        Me.ToolStripStatusTrackSize.Text = "Size 0"
        Me.ToolStripStatusTrackSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ToolStripStatusChecksumText
        '
        Me.ToolStripStatusChecksumText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusChecksumText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusChecksumText.Margin = New System.Windows.Forms.Padding(2, 3, 0, 2)
        Me.ToolStripStatusChecksumText.Name = "ToolStripStatusChecksumText"
        Me.ToolStripStatusChecksumText.Size = New System.Drawing.Size(70, 19)
        Me.ToolStripStatusChecksumText.Text = "Checksum:"
        Me.ToolStripStatusChecksumText.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ToolStripStatusChecksum
        '
        Me.ToolStripStatusChecksum.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusChecksum.Margin = New System.Windows.Forms.Padding(0, 3, 2, 2)
        Me.ToolStripStatusChecksum.Name = "ToolStripStatusChecksum"
        Me.ToolStripStatusChecksum.Size = New System.Drawing.Size(32, 19)
        Me.ToolStripStatusChecksum.Text = "Valid"
        Me.ToolStripStatusChecksum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToolStripStatusBits
        '
        Me.ToolStripStatusBits.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusBits.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusBits.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusBits.Name = "ToolStripStatusBits"
        Me.ToolStripStatusBits.Size = New System.Drawing.Size(39, 19)
        Me.ToolStripStatusBits.Text = "0 Bits"
        Me.ToolStripStatusBits.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ToolStripStatusBytes
        '
        Me.ToolStripStatusBytes.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusBytes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusBytes.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusBytes.Name = "ToolStripStatusBytes"
        Me.ToolStripStatusBytes.Size = New System.Drawing.Size(48, 19)
        Me.ToolStripStatusBytes.Text = "0 Bytes"
        Me.ToolStripStatusBytes.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'HexBox1
        '
        Me.HexBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        '
        '
        '
        Me.HexBox1.BuiltInContextMenu.CopyMenuItemText = "Copy Text"
        Me.HexBox1.BuiltInContextMenu.SelectAllMenuItemText = "Select All"
        Me.HexBox1.ColumnInfoVisible = True
        Me.HexBox1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.HexBox1.Font = New System.Drawing.Font("Courier New", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HexBox1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.HexBox1.HexViewTextColor = System.Drawing.SystemColors.ControlText
        Me.HexBox1.LineInfoVisible = True
        Me.HexBox1.Location = New System.Drawing.Point(12, 53)
        Me.HexBox1.Name = "HexBox1"
        Me.HexBox1.ReadOnly = True
        Me.HexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(188, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.HexBox1.Size = New System.Drawing.Size(650, 493)
        Me.HexBox1.StringViewVisible = True
        Me.HexBox1.TabIndex = 2
        Me.HexBox1.UseFixedBytesPerLine = True
        Me.HexBox1.VScrollBarVisible = False
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnCopyText, Me.BtnCopyHex, Me.BtnCopyHexFormatted, ToolStripSeparator2, Me.BtnFind, Me.BtnFindNext, Me.ToolStripMenuItem1, Me.BtnSelectAll, Me.BtnSelectRegion, Me.BtnSelectSector, Me.BtnSelectData, Me.ToolStripSeparator1, Me.BtnAdjustOffset})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(259, 242)
        '
        'BtnCopyText
        '
        Me.BtnCopyText.Image = CType(resources.GetObject("BtnCopyText.Image"), System.Drawing.Image)
        Me.BtnCopyText.Name = "BtnCopyText"
        Me.BtnCopyText.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.T), System.Windows.Forms.Keys)
        Me.BtnCopyText.Size = New System.Drawing.Size(258, 22)
        Me.BtnCopyText.Text = "Copy &Text"
        '
        'BtnCopyHex
        '
        Me.BtnCopyHex.Image = CType(resources.GetObject("BtnCopyHex.Image"), System.Drawing.Image)
        Me.BtnCopyHex.Name = "BtnCopyHex"
        Me.BtnCopyHex.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.BtnCopyHex.Size = New System.Drawing.Size(258, 22)
        Me.BtnCopyHex.Text = "Copy &Hex"
        '
        'BtnCopyHexFormatted
        '
        Me.BtnCopyHexFormatted.Image = CType(resources.GetObject("BtnCopyHexFormatted.Image"), System.Drawing.Image)
        Me.BtnCopyHexFormatted.Name = "BtnCopyHexFormatted"
        Me.BtnCopyHexFormatted.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.BtnCopyHexFormatted.Size = New System.Drawing.Size(258, 22)
        Me.BtnCopyHexFormatted.Text = "Copy Hex &Formatted"
        '
        'BtnFind
        '
        Me.BtnFind.Name = "BtnFind"
        Me.BtnFind.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.F), System.Windows.Forms.Keys)
        Me.BtnFind.Size = New System.Drawing.Size(258, 22)
        Me.BtnFind.Text = "Find"
        '
        'BtnFindNext
        '
        Me.BtnFindNext.Name = "BtnFindNext"
        Me.BtnFindNext.ShortcutKeys = System.Windows.Forms.Keys.F3
        Me.BtnFindNext.Size = New System.Drawing.Size(258, 22)
        Me.BtnFindNext.Text = "Find Next"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(255, 6)
        '
        'BtnSelectAll
        '
        Me.BtnSelectAll.Image = CType(resources.GetObject("BtnSelectAll.Image"), System.Drawing.Image)
        Me.BtnSelectAll.Name = "BtnSelectAll"
        Me.BtnSelectAll.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.A), System.Windows.Forms.Keys)
        Me.BtnSelectAll.Size = New System.Drawing.Size(258, 22)
        Me.BtnSelectAll.Text = "Select &All"
        '
        'BtnSelectRegion
        '
        Me.BtnSelectRegion.Image = CType(resources.GetObject("BtnSelectRegion.Image"), System.Drawing.Image)
        Me.BtnSelectRegion.Name = "BtnSelectRegion"
        Me.BtnSelectRegion.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
        Me.BtnSelectRegion.Size = New System.Drawing.Size(258, 22)
        Me.BtnSelectRegion.Text = "Select &Region"
        '
        'BtnSelectSector
        '
        Me.BtnSelectSector.Image = CType(resources.GetObject("BtnSelectSector.Image"), System.Drawing.Image)
        Me.BtnSelectSector.Name = "BtnSelectSector"
        Me.BtnSelectSector.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.BtnSelectSector.Size = New System.Drawing.Size(258, 22)
        Me.BtnSelectSector.Text = "Select &Sector"
        '
        'BtnSelectData
        '
        Me.BtnSelectData.Image = CType(resources.GetObject("BtnSelectData.Image"), System.Drawing.Image)
        Me.BtnSelectData.Name = "BtnSelectData"
        Me.BtnSelectData.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D), System.Windows.Forms.Keys)
        Me.BtnSelectData.Size = New System.Drawing.Size(258, 22)
        Me.BtnSelectData.Text = "Select &Data"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(255, 6)
        '
        'BtnAdjustOffset
        '
        Me.BtnAdjustOffset.Name = "BtnAdjustOffset"
        Me.BtnAdjustOffset.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.BtnAdjustOffset.Size = New System.Drawing.Size(258, 22)
        Me.BtnAdjustOffset.Text = "Adjust Bit &Offset"
        '
        'ToolStripBtnCopyText
        '
        Me.ToolStripBtnCopyText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnCopyText.Image = CType(resources.GetObject("ToolStripBtnCopyText.Image"), System.Drawing.Image)
        Me.ToolStripBtnCopyText.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnCopyText.Name = "ToolStripBtnCopyText"
        Me.ToolStripBtnCopyText.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnCopyText.Text = "Copy Text"
        '
        'ToolStripBtnCopyHex
        '
        Me.ToolStripBtnCopyHex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnCopyHex.Image = CType(resources.GetObject("ToolStripBtnCopyHex.Image"), System.Drawing.Image)
        Me.ToolStripBtnCopyHex.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnCopyHex.Name = "ToolStripBtnCopyHex"
        Me.ToolStripBtnCopyHex.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnCopyHex.Text = "Copy Hex"
        '
        'ToolStripBtnCopyHexFormatted
        '
        Me.ToolStripBtnCopyHexFormatted.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnCopyHexFormatted.Image = CType(resources.GetObject("ToolStripBtnCopyHexFormatted.Image"), System.Drawing.Image)
        Me.ToolStripBtnCopyHexFormatted.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnCopyHexFormatted.Name = "ToolStripBtnCopyHexFormatted"
        Me.ToolStripBtnCopyHexFormatted.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnCopyHexFormatted.Text = "Copy Hex Formatted"
        '
        'ToolStripBtnFind
        '
        Me.ToolStripBtnFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnFind.Image = CType(resources.GetObject("ToolStripBtnFind.Image"), System.Drawing.Image)
        Me.ToolStripBtnFind.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnFind.Name = "ToolStripBtnFind"
        Me.ToolStripBtnFind.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnFind.Text = "Find"
        '
        'ToolStripBtnFindNext
        '
        Me.ToolStripBtnFindNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnFindNext.Image = CType(resources.GetObject("ToolStripBtnFindNext.Image"), System.Drawing.Image)
        Me.ToolStripBtnFindNext.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnFindNext.Name = "ToolStripBtnFindNext"
        Me.ToolStripBtnFindNext.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnFindNext.Text = "Find Next"
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        Me.ToolStripSeparator8.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripBtnSelectAll
        '
        Me.ToolStripBtnSelectAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnSelectAll.Image = CType(resources.GetObject("ToolStripBtnSelectAll.Image"), System.Drawing.Image)
        Me.ToolStripBtnSelectAll.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnSelectAll.Name = "ToolStripBtnSelectAll"
        Me.ToolStripBtnSelectAll.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnSelectAll.Text = "Select All"
        '
        'ToolStripMain
        '
        Me.ToolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStripMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtnCopyText, Me.ToolStripBtnCopyHex, Me.ToolStripBtnCopyHexFormatted, ToolStripSeparator5, Me.ToolStripBtnFind, Me.ToolStripBtnFindNext, Me.ToolStripSeparator8, Me.ToolStripBtnSelectAll, Me.ToolStripBtnSelectRegion, Me.ToolStripBtnSelectSector, Me.ToolStripBtnSelectData, Me.ToolStripBtnAdjustOffset})
        Me.ToolStripMain.Location = New System.Drawing.Point(0, 0)
        Me.ToolStripMain.Name = "ToolStripMain"
        Me.ToolStripMain.Padding = New System.Windows.Forms.Padding(12, 0, 12, 0)
        Me.ToolStripMain.Size = New System.Drawing.Size(944, 25)
        Me.ToolStripMain.TabIndex = 0
        Me.ToolStripMain.Text = "ToolStrip1"
        '
        'ToolStripBtnSelectRegion
        '
        Me.ToolStripBtnSelectRegion.Image = CType(resources.GetObject("ToolStripBtnSelectRegion.Image"), System.Drawing.Image)
        Me.ToolStripBtnSelectRegion.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnSelectRegion.Name = "ToolStripBtnSelectRegion"
        Me.ToolStripBtnSelectRegion.Size = New System.Drawing.Size(64, 22)
        Me.ToolStripBtnSelectRegion.Text = "Region"
        Me.ToolStripBtnSelectRegion.ToolTipText = "Select Region"
        '
        'ToolStripBtnSelectSector
        '
        Me.ToolStripBtnSelectSector.Image = CType(resources.GetObject("ToolStripBtnSelectSector.Image"), System.Drawing.Image)
        Me.ToolStripBtnSelectSector.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnSelectSector.Name = "ToolStripBtnSelectSector"
        Me.ToolStripBtnSelectSector.Size = New System.Drawing.Size(60, 22)
        Me.ToolStripBtnSelectSector.Text = "Sector"
        Me.ToolStripBtnSelectSector.ToolTipText = "Select Sector"
        '
        'ToolStripBtnSelectData
        '
        Me.ToolStripBtnSelectData.Image = CType(resources.GetObject("ToolStripBtnSelectData.Image"), System.Drawing.Image)
        Me.ToolStripBtnSelectData.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnSelectData.Name = "ToolStripBtnSelectData"
        Me.ToolStripBtnSelectData.Size = New System.Drawing.Size(51, 22)
        Me.ToolStripBtnSelectData.Text = "Data"
        Me.ToolStripBtnSelectData.ToolTipText = "Select Data"
        '
        'ToolStripBtnAdjustOffset
        '
        Me.ToolStripBtnAdjustOffset.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripBtnAdjustOffset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnAdjustOffset.Image = CType(resources.GetObject("ToolStripBtnAdjustOffset.Image"), System.Drawing.Image)
        Me.ToolStripBtnAdjustOffset.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnAdjustOffset.Margin = New System.Windows.Forms.Padding(6, 1, 0, 2)
        Me.ToolStripBtnAdjustOffset.Name = "ToolStripBtnAdjustOffset"
        Me.ToolStripBtnAdjustOffset.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnAdjustOffset.Text = "Adjust Bit Offset"
        '
        'PanelSectors
        '
        Me.PanelSectors.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelSectors.Location = New System.Drawing.Point(0, 25)
        Me.PanelSectors.Name = "PanelSectors"
        Me.PanelSectors.Padding = New System.Windows.Forms.Padding(12, 4, 12, 4)
        Me.PanelSectors.Size = New System.Drawing.Size(944, 25)
        Me.PanelSectors.TabIndex = 1
        Me.PanelSectors.TabStop = True
        '
        'HexViewRawForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(944, 573)
        Me.Controls.Add(Me.PanelSectors)
        Me.Controls.Add(Me.ToolStripMain)
        Me.Controls.Add(Me.DataGridDataInspector)
        Me.Controls.Add(Me.StatusStripBottom)
        Me.Controls.Add(Me.HexBox1)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(960, 1280)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(960, 480)
        Me.Name = "HexViewRawForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Hex Viewer"
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
    Friend WithEvents DataGridName As DataGridViewTextBoxColumn
    Friend WithEvents DataGridValue As DataGridViewTextBoxColumn
    Friend WithEvents DataGridLength As DataGridViewTextBoxColumn
    Friend WithEvents DataGridInvalid As DataGridViewTextBoxColumn
    Friend WithEvents DataGridEditable As DataGridViewTextBoxColumn
    Friend WithEvents DataGridType As DataGridViewTextBoxColumn
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
End Class
