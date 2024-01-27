<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class HexViewForm
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
        Dim ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripStatusGap As System.Windows.Forms.ToolStripStatusLabel
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HexViewForm))
        Dim ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
        Dim ToolStripStatusLabel2 As System.Windows.Forms.ToolStripStatusLabel
        Me.ToolStripMain = New System.Windows.Forms.ToolStrip()
        Me.ToolStripBtnCommit = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnUndo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnRedo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnCopyText = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnCopyHex = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnCopyHexFormatted = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnPaste = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnFind = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnFindNext = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripBtnDelete = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnFillF6 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnSelectAll = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnSelectSector = New System.Windows.Forms.ToolStripButton()
        Me.CmbGroups = New System.Windows.Forms.ToolStripComboBox()
        Me.LblGroups = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripBtnSelectTrack = New System.Windows.Forms.ToolStripButton()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnUndo = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnRedo = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnCopyText = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnCopyHex = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnCopyHexFormatted = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnPaste = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFind = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFindNext = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.BtnDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnFillF6 = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnSelectSector = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnSelectTrack = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnSelectAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusOffset = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusBlock = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLength = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusCluster = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSector = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusTrack = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSide = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusBytes = New System.Windows.Forms.ToolStripStatusLabel()
        Me.HexBox1 = New Hb.Windows.Forms.HexBox()
        Me.DataGridDataInspector = New System.Windows.Forms.DataGridView()
        Me.DataGridName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridValue = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridLength = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridInvalid = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridEditable = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridType = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ContextMenuStrip2 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnCopyValue = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripStatusTrackSector = New System.Windows.Forms.ToolStripStatusLabel()
        ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripStatusGap = New System.Windows.Forms.ToolStripStatusLabel()
        ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripMain.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        CType(Me.DataGridDataInspector, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip2.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStripSeparator1
        '
        ToolStripSeparator1.Name = "ToolStripSeparator1"
        ToolStripSeparator1.Size = New System.Drawing.Size(255, 6)
        '
        'ToolStripSeparator3
        '
        ToolStripSeparator3.Name = "ToolStripSeparator3"
        ToolStripSeparator3.Size = New System.Drawing.Size(255, 6)
        '
        'ToolStripSeparator2
        '
        ToolStripSeparator2.Name = "ToolStripSeparator2"
        ToolStripSeparator2.Size = New System.Drawing.Size(255, 6)
        '
        'ToolStripSeparator4
        '
        ToolStripSeparator4.Name = "ToolStripSeparator4"
        ToolStripSeparator4.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripSeparator5
        '
        ToolStripSeparator5.Name = "ToolStripSeparator5"
        ToolStripSeparator5.Size = New System.Drawing.Size(6, 25)
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
        'ToolStripStatusGap
        '
        ToolStripStatusGap.Name = "ToolStripStatusGap"
        ToolStripStatusGap.Size = New System.Drawing.Size(281, 19)
        ToolStripStatusGap.Spring = True
        '
        'ToolStripMain
        '
        Me.ToolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStripMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtnCommit, ToolStripSeparator7, Me.ToolStripBtnUndo, Me.ToolStripBtnRedo, ToolStripSeparator4, Me.ToolStripBtnCopyText, Me.ToolStripBtnCopyHex, Me.ToolStripBtnCopyHexFormatted, Me.ToolStripBtnPaste, ToolStripSeparator5, Me.ToolStripBtnFind, Me.ToolStripBtnFindNext, Me.ToolStripSeparator8, Me.ToolStripBtnDelete, Me.ToolStripBtnFillF6, ToolStripSeparator6, Me.ToolStripBtnSelectAll, Me.ToolStripBtnSelectSector, Me.CmbGroups, Me.LblGroups, Me.ToolStripBtnSelectTrack})
        Me.ToolStripMain.Location = New System.Drawing.Point(0, 0)
        Me.ToolStripMain.Name = "ToolStripMain"
        Me.ToolStripMain.Padding = New System.Windows.Forms.Padding(12, 0, 12, 0)
        Me.ToolStripMain.Size = New System.Drawing.Size(934, 25)
        Me.ToolStripMain.TabIndex = 0
        Me.ToolStripMain.Text = "ToolStrip1"
        '
        'ToolStripBtnCommit
        '
        Me.ToolStripBtnCommit.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnCommit.Name = "ToolStripBtnCommit"
        Me.ToolStripBtnCommit.Size = New System.Drawing.Size(55, 22)
        Me.ToolStripBtnCommit.Text = "Commit"
        Me.ToolStripBtnCommit.ToolTipText = "Commit Changes"
        '
        'ToolStripBtnUndo
        '
        Me.ToolStripBtnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnUndo.Image = CType(resources.GetObject("ToolStripBtnUndo.Image"), System.Drawing.Image)
        Me.ToolStripBtnUndo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnUndo.Name = "ToolStripBtnUndo"
        Me.ToolStripBtnUndo.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnUndo.Text = "Undo"
        '
        'ToolStripBtnRedo
        '
        Me.ToolStripBtnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnRedo.Image = CType(resources.GetObject("ToolStripBtnRedo.Image"), System.Drawing.Image)
        Me.ToolStripBtnRedo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnRedo.Name = "ToolStripBtnRedo"
        Me.ToolStripBtnRedo.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnRedo.Text = "Redo"
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
        'ToolStripBtnPaste
        '
        Me.ToolStripBtnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnPaste.Image = CType(resources.GetObject("ToolStripBtnPaste.Image"), System.Drawing.Image)
        Me.ToolStripBtnPaste.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnPaste.Name = "ToolStripBtnPaste"
        Me.ToolStripBtnPaste.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnPaste.Text = "Paste Over"
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
        'ToolStripBtnDelete
        '
        Me.ToolStripBtnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnDelete.Image = CType(resources.GetObject("ToolStripBtnDelete.Image"), System.Drawing.Image)
        Me.ToolStripBtnDelete.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnDelete.Name = "ToolStripBtnDelete"
        Me.ToolStripBtnDelete.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnDelete.Text = "Fill Selection with 0x00"
        '
        'ToolStripBtnFillF6
        '
        Me.ToolStripBtnFillF6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtnFillF6.Image = CType(resources.GetObject("ToolStripBtnFillF6.Image"), System.Drawing.Image)
        Me.ToolStripBtnFillF6.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnFillF6.Name = "ToolStripBtnFillF6"
        Me.ToolStripBtnFillF6.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtnFillF6.Text = "Fill Selection with 0xF6"
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
        'ToolStripBtnSelectSector
        '
        Me.ToolStripBtnSelectSector.Image = CType(resources.GetObject("ToolStripBtnSelectSector.Image"), System.Drawing.Image)
        Me.ToolStripBtnSelectSector.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnSelectSector.Margin = New System.Windows.Forms.Padding(0, 1, 4, 2)
        Me.ToolStripBtnSelectSector.Name = "ToolStripBtnSelectSector"
        Me.ToolStripBtnSelectSector.Size = New System.Drawing.Size(60, 22)
        Me.ToolStripBtnSelectSector.Text = "Sector"
        '
        'CmbGroups
        '
        Me.CmbGroups.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.CmbGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbGroups.DropDownWidth = 218
        Me.CmbGroups.FlatStyle = System.Windows.Forms.FlatStyle.Standard
        Me.CmbGroups.Name = "CmbGroups"
        Me.CmbGroups.Size = New System.Drawing.Size(100, 25)
        '
        'LblGroups
        '
        Me.LblGroups.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.LblGroups.Name = "LblGroups"
        Me.LblGroups.Size = New System.Drawing.Size(45, 22)
        Me.LblGroups.Text = "Display"
        '
        'ToolStripBtnSelectTrack
        '
        Me.ToolStripBtnSelectTrack.Image = CType(resources.GetObject("ToolStripBtnSelectTrack.Image"), System.Drawing.Image)
        Me.ToolStripBtnSelectTrack.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtnSelectTrack.Margin = New System.Windows.Forms.Padding(0, 1, 4, 2)
        Me.ToolStripBtnSelectTrack.Name = "ToolStripBtnSelectTrack"
        Me.ToolStripBtnSelectTrack.Size = New System.Drawing.Size(54, 22)
        Me.ToolStripBtnSelectTrack.Text = "Track"
        Me.ToolStripBtnSelectTrack.ToolTipText = "Track"
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnUndo, Me.BtnRedo, ToolStripSeparator3, Me.BtnCopyText, Me.BtnCopyHex, Me.BtnCopyHexFormatted, Me.BtnPaste, ToolStripSeparator2, Me.BtnFind, Me.BtnFindNext, Me.ToolStripMenuItem1, Me.BtnDelete, Me.BtnFillF6, ToolStripSeparator1, Me.BtnSelectSector, Me.BtnSelectTrack, Me.BtnSelectAll})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(259, 314)
        '
        'BtnUndo
        '
        Me.BtnUndo.Image = CType(resources.GetObject("BtnUndo.Image"), System.Drawing.Image)
        Me.BtnUndo.Name = "BtnUndo"
        Me.BtnUndo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.BtnUndo.Size = New System.Drawing.Size(258, 22)
        Me.BtnUndo.Text = "&Undo"
        '
        'BtnRedo
        '
        Me.BtnRedo.Image = CType(resources.GetObject("BtnRedo.Image"), System.Drawing.Image)
        Me.BtnRedo.Name = "BtnRedo"
        Me.BtnRedo.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.BtnRedo.Size = New System.Drawing.Size(258, 22)
        Me.BtnRedo.Text = "&Redo"
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
        'BtnPaste
        '
        Me.BtnPaste.Image = CType(resources.GetObject("BtnPaste.Image"), System.Drawing.Image)
        Me.BtnPaste.Name = "BtnPaste"
        Me.BtnPaste.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.V), System.Windows.Forms.Keys)
        Me.BtnPaste.Size = New System.Drawing.Size(258, 22)
        Me.BtnPaste.Text = "&Paste Over"
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
        'BtnDelete
        '
        Me.BtnDelete.Image = CType(resources.GetObject("BtnDelete.Image"), System.Drawing.Image)
        Me.BtnDelete.Name = "BtnDelete"
        Me.BtnDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete
        Me.BtnDelete.Size = New System.Drawing.Size(258, 22)
        Me.BtnDelete.Text = "Fill Selection with 0x00"
        '
        'BtnFillF6
        '
        Me.BtnFillF6.Image = CType(resources.GetObject("BtnFillF6.Image"), System.Drawing.Image)
        Me.BtnFillF6.Name = "BtnFillF6"
        Me.BtnFillF6.ShortcutKeys = CType((System.Windows.Forms.Keys.Shift Or System.Windows.Forms.Keys.Delete), System.Windows.Forms.Keys)
        Me.BtnFillF6.Size = New System.Drawing.Size(258, 22)
        Me.BtnFillF6.Text = "Fill Selection with 0xF6"
        '
        'BtnSelectSector
        '
        Me.BtnSelectSector.Image = CType(resources.GetObject("BtnSelectSector.Image"), System.Drawing.Image)
        Me.BtnSelectSector.Name = "BtnSelectSector"
        Me.BtnSelectSector.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.E), System.Windows.Forms.Keys)
        Me.BtnSelectSector.Size = New System.Drawing.Size(258, 22)
        Me.BtnSelectSector.Text = "Select &Sector"
        '
        'BtnSelectTrack
        '
        Me.BtnSelectTrack.Image = CType(resources.GetObject("BtnSelectTrack.Image"), System.Drawing.Image)
        Me.BtnSelectTrack.Name = "BtnSelectTrack"
        Me.BtnSelectTrack.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
        Me.BtnSelectTrack.Size = New System.Drawing.Size(258, 22)
        Me.BtnSelectTrack.Text = "Select T&rack"
        '
        'BtnSelectAll
        '
        Me.BtnSelectAll.Image = CType(resources.GetObject("BtnSelectAll.Image"), System.Drawing.Image)
        Me.BtnSelectAll.Name = "BtnSelectAll"
        Me.BtnSelectAll.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.A), System.Windows.Forms.Keys)
        Me.BtnSelectAll.Size = New System.Drawing.Size(258, 22)
        Me.BtnSelectAll.Text = "Select &All"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusOffset, Me.ToolStripStatusBlock, Me.ToolStripStatusLength, ToolStripStatusGap, Me.ToolStripStatusCluster, Me.ToolStripStatusSector, ToolStripStatusLabel1, Me.ToolStripStatusTrack, Me.ToolStripStatusSide, Me.ToolStripStatusTrackSector, ToolStripStatusLabel2, Me.ToolStripStatusBytes})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 549)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(934, 24)
        Me.StatusStrip1.TabIndex = 5
        Me.StatusStrip1.Text = "StatusStrip1"
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
        'ToolStripStatusCluster
        '
        Me.ToolStripStatusCluster.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusCluster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusCluster.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusCluster.Name = "ToolStripStatusCluster"
        Me.ToolStripStatusCluster.Size = New System.Drawing.Size(57, 19)
        Me.ToolStripStatusCluster.Text = "Cluster 0"
        Me.ToolStripStatusCluster.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ToolStripStatusSector
        '
        Me.ToolStripStatusSector.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusSector.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusSector.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusSector.Name = "ToolStripStatusSector"
        Me.ToolStripStatusSector.Size = New System.Drawing.Size(53, 19)
        Me.ToolStripStatusSector.Text = "Sector 0"
        Me.ToolStripStatusSector.TextAlign = System.Drawing.ContentAlignment.MiddleRight
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
        Me.HexBox1.Location = New System.Drawing.Point(12, 28)
        Me.HexBox1.Name = "HexBox1"
        Me.HexBox1.ReadOnly = True
        Me.HexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(188, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.HexBox1.Size = New System.Drawing.Size(640, 517)
        Me.HexBox1.StringViewVisible = True
        Me.HexBox1.TabIndex = 1
        Me.HexBox1.UseFixedBytesPerLine = True
        Me.HexBox1.VScrollBarVisible = False
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
        Me.DataGridDataInspector.Location = New System.Drawing.Point(658, 28)
        Me.DataGridDataInspector.MultiSelect = False
        Me.DataGridDataInspector.Name = "DataGridDataInspector"
        Me.DataGridDataInspector.RowHeadersVisible = False
        Me.DataGridDataInspector.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DataGridDataInspector.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.DataGridDataInspector.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridDataInspector.Size = New System.Drawing.Size(264, 517)
        Me.DataGridDataInspector.StandardTab = True
        Me.DataGridDataInspector.TabIndex = 2
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
        'ToolStripStatusLabel1
        '
        ToolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        ToolStripStatusLabel1.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        ToolStripStatusLabel1.Padding = New System.Windows.Forms.Padding(16, 0, 0, 0)
        ToolStripStatusLabel1.Size = New System.Drawing.Size(20, 19)
        '
        'ToolStripStatusLabel2
        '
        ToolStripStatusLabel2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        ToolStripStatusLabel2.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        ToolStripStatusLabel2.Padding = New System.Windows.Forms.Padding(16, 0, 0, 0)
        ToolStripStatusLabel2.Size = New System.Drawing.Size(20, 19)
        '
        'ToolStripStatusTrackSector
        '
        Me.ToolStripStatusTrackSector.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusTrackSector.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusTrackSector.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusTrackSector.Name = "ToolStripStatusTrackSector"
        Me.ToolStripStatusTrackSector.Size = New System.Drawing.Size(53, 19)
        Me.ToolStripStatusTrackSector.Text = "Sector 0"
        Me.ToolStripStatusTrackSector.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'HexViewForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(934, 573)
        Me.Controls.Add(Me.DataGridDataInspector)
        Me.Controls.Add(Me.ToolStripMain)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.HexBox1)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(950, 1280)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(950, 480)
        Me.Name = "HexViewForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Hex Viewer"
        Me.ToolStripMain.ResumeLayout(False)
        Me.ToolStripMain.PerformLayout()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        CType(Me.DataGridDataInspector, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents HexBox1 As Hb.Windows.Forms.HexBox
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusCluster As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusSector As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusBytes As ToolStripStatusLabel
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents BtnCopyText As ToolStripMenuItem
    Friend WithEvents BtnCopyHex As ToolStripMenuItem
    Friend WithEvents BtnSelectAll As ToolStripMenuItem
    Friend WithEvents BtnCopyHexFormatted As ToolStripMenuItem
    Friend WithEvents ToolStripStatusOffset As ToolStripStatusLabel
    Friend WithEvents BtnUndo As ToolStripMenuItem
    Friend WithEvents BtnDelete As ToolStripMenuItem
    Friend WithEvents BtnFillF6 As ToolStripMenuItem
    Friend WithEvents BtnPaste As ToolStripMenuItem
    Friend WithEvents BtnSelectSector As ToolStripMenuItem
    Friend WithEvents ToolStripBtnUndo As ToolStripButton
    Friend WithEvents ToolStripBtnCopyText As ToolStripButton
    Friend WithEvents ToolStripBtnCopyHex As ToolStripButton
    Friend WithEvents ToolStripBtnCopyHexFormatted As ToolStripButton
    Friend WithEvents ToolStripBtnPaste As ToolStripButton
    Friend WithEvents ToolStripBtnDelete As ToolStripButton
    Friend WithEvents ToolStripBtnFillF6 As ToolStripButton
    Friend WithEvents ToolStripBtnSelectAll As ToolStripButton
    Friend WithEvents ToolStripBtnSelectSector As ToolStripButton
    Friend WithEvents CmbGroups As ToolStripComboBox
    Friend WithEvents BtnRedo As ToolStripMenuItem
    Friend WithEvents ToolStripBtnRedo As ToolStripButton
    Friend WithEvents ToolStripBtnCommit As ToolStripButton
    Friend WithEvents LblGroups As ToolStripLabel
    Friend WithEvents ToolStripMain As ToolStrip
    Friend WithEvents ToolStripStatusTrack As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusSide As ToolStripStatusLabel
    Friend WithEvents ToolStripBtnFind As ToolStripButton
    Friend WithEvents ToolStripBtnFindNext As ToolStripButton
    Friend WithEvents ToolStripSeparator8 As ToolStripSeparator
    Friend WithEvents BtnFind As ToolStripMenuItem
    Friend WithEvents BtnFindNext As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents BtnSelectTrack As ToolStripMenuItem
    Friend WithEvents ToolStripStatusBlock As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLength As ToolStripStatusLabel
    Friend WithEvents DataGridDataInspector As DataGridView
    'Friend WithEvents GroupBoxByteOrder As GroupBox
    'Friend WithEvents RadioButtonBigEndien As RadioButton
    'Friend WithEvents RadioButtonLittleEndien As RadioButton
    Friend WithEvents ContextMenuStrip2 As ContextMenuStrip
    Friend WithEvents BtnCopyValue As ToolStripMenuItem
    Friend WithEvents DataGridName As DataGridViewTextBoxColumn
    Friend WithEvents DataGridValue As DataGridViewTextBoxColumn
    Friend WithEvents DataGridLength As DataGridViewTextBoxColumn
    Friend WithEvents DataGridInvalid As DataGridViewTextBoxColumn
    Friend WithEvents DataGridEditable As DataGridViewTextBoxColumn
    Friend WithEvents DataGridType As DataGridViewTextBoxColumn
    Friend WithEvents ToolStripBtnSelectTrack As ToolStripButton
    Friend WithEvents ToolStripStatusTrackSector As ToolStripStatusLabel
End Class
