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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HexViewForm))
        Dim ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
        Dim ToolStripStatusGap As System.Windows.Forms.ToolStripStatusLabel
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
        Me.BtnFill = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.ToolStripStatusTrackSector = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusBytes = New System.Windows.Forms.ToolStripStatusLabel()
        Me.HexBox1 = New DiskImageTool.Hb.Windows.Forms.HexBox()
        Me.DataGridDataInspector = New System.Windows.Forms.DataGridView()
        Me.ContextMenuStrip2 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnCopyValue = New System.Windows.Forms.ToolStripMenuItem()
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
        resources.ApplyResources(ToolStripSeparator1, "ToolStripSeparator1")
        '
        'ToolStripSeparator3
        '
        ToolStripSeparator3.Name = "ToolStripSeparator3"
        resources.ApplyResources(ToolStripSeparator3, "ToolStripSeparator3")
        '
        'ToolStripSeparator2
        '
        ToolStripSeparator2.Name = "ToolStripSeparator2"
        resources.ApplyResources(ToolStripSeparator2, "ToolStripSeparator2")
        '
        'ToolStripSeparator4
        '
        ToolStripSeparator4.Name = "ToolStripSeparator4"
        resources.ApplyResources(ToolStripSeparator4, "ToolStripSeparator4")
        '
        'ToolStripSeparator5
        '
        ToolStripSeparator5.Name = "ToolStripSeparator5"
        resources.ApplyResources(ToolStripSeparator5, "ToolStripSeparator5")
        '
        'ToolStripSeparator6
        '
        ToolStripSeparator6.Name = "ToolStripSeparator6"
        resources.ApplyResources(ToolStripSeparator6, "ToolStripSeparator6")
        '
        'ToolStripSeparator7
        '
        ToolStripSeparator7.Name = "ToolStripSeparator7"
        resources.ApplyResources(ToolStripSeparator7, "ToolStripSeparator7")
        '
        'ToolStripStatusGap
        '
        ToolStripStatusGap.Name = "ToolStripStatusGap"
        resources.ApplyResources(ToolStripStatusGap, "ToolStripStatusGap")
        ToolStripStatusGap.Spring = True
        '
        'ToolStripStatusLabel1
        '
        ToolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        ToolStripStatusLabel1.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        ToolStripStatusLabel1.Padding = New System.Windows.Forms.Padding(16, 0, 0, 0)
        resources.ApplyResources(ToolStripStatusLabel1, "ToolStripStatusLabel1")
        '
        'ToolStripStatusLabel2
        '
        ToolStripStatusLabel2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        ToolStripStatusLabel2.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        ToolStripStatusLabel2.Padding = New System.Windows.Forms.Padding(16, 0, 0, 0)
        resources.ApplyResources(ToolStripStatusLabel2, "ToolStripStatusLabel2")
        '
        'ToolStripMain
        '
        Me.ToolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStripMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtnCommit, ToolStripSeparator7, Me.ToolStripBtnUndo, Me.ToolStripBtnRedo, ToolStripSeparator4, Me.ToolStripBtnCopyText, Me.ToolStripBtnCopyHex, Me.ToolStripBtnCopyHexFormatted, Me.ToolStripBtnPaste, ToolStripSeparator5, Me.ToolStripBtnFind, Me.ToolStripBtnFindNext, Me.ToolStripSeparator8, Me.ToolStripBtnDelete, Me.ToolStripBtnFillF6, ToolStripSeparator6, Me.ToolStripBtnSelectAll, Me.ToolStripBtnSelectSector, Me.CmbGroups, Me.LblGroups, Me.ToolStripBtnSelectTrack})
        resources.ApplyResources(Me.ToolStripMain, "ToolStripMain")
        Me.ToolStripMain.Name = "ToolStripMain"
        '
        'ToolStripBtnCommit
        '
        resources.ApplyResources(Me.ToolStripBtnCommit, "ToolStripBtnCommit")
        Me.ToolStripBtnCommit.Name = "ToolStripBtnCommit"
        '
        'ToolStripBtnUndo
        '
        Me.ToolStripBtnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripBtnUndo, "ToolStripBtnUndo")
        Me.ToolStripBtnUndo.Name = "ToolStripBtnUndo"
        '
        'ToolStripBtnRedo
        '
        Me.ToolStripBtnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripBtnRedo, "ToolStripBtnRedo")
        Me.ToolStripBtnRedo.Name = "ToolStripBtnRedo"
        '
        'ToolStripBtnCopyText
        '
        Me.ToolStripBtnCopyText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripBtnCopyText, "ToolStripBtnCopyText")
        Me.ToolStripBtnCopyText.Name = "ToolStripBtnCopyText"
        '
        'ToolStripBtnCopyHex
        '
        Me.ToolStripBtnCopyHex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripBtnCopyHex, "ToolStripBtnCopyHex")
        Me.ToolStripBtnCopyHex.Name = "ToolStripBtnCopyHex"
        '
        'ToolStripBtnCopyHexFormatted
        '
        Me.ToolStripBtnCopyHexFormatted.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripBtnCopyHexFormatted, "ToolStripBtnCopyHexFormatted")
        Me.ToolStripBtnCopyHexFormatted.Name = "ToolStripBtnCopyHexFormatted"
        '
        'ToolStripBtnPaste
        '
        Me.ToolStripBtnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripBtnPaste, "ToolStripBtnPaste")
        Me.ToolStripBtnPaste.Name = "ToolStripBtnPaste"
        '
        'ToolStripBtnFind
        '
        Me.ToolStripBtnFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripBtnFind, "ToolStripBtnFind")
        Me.ToolStripBtnFind.Name = "ToolStripBtnFind"
        '
        'ToolStripBtnFindNext
        '
        Me.ToolStripBtnFindNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripBtnFindNext, "ToolStripBtnFindNext")
        Me.ToolStripBtnFindNext.Name = "ToolStripBtnFindNext"
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        resources.ApplyResources(Me.ToolStripSeparator8, "ToolStripSeparator8")
        '
        'ToolStripBtnDelete
        '
        Me.ToolStripBtnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripBtnDelete, "ToolStripBtnDelete")
        Me.ToolStripBtnDelete.Name = "ToolStripBtnDelete"
        '
        'ToolStripBtnFillF6
        '
        Me.ToolStripBtnFillF6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripBtnFillF6, "ToolStripBtnFillF6")
        Me.ToolStripBtnFillF6.Name = "ToolStripBtnFillF6"
        '
        'ToolStripBtnSelectAll
        '
        Me.ToolStripBtnSelectAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripBtnSelectAll, "ToolStripBtnSelectAll")
        Me.ToolStripBtnSelectAll.Name = "ToolStripBtnSelectAll"
        '
        'ToolStripBtnSelectSector
        '
        resources.ApplyResources(Me.ToolStripBtnSelectSector, "ToolStripBtnSelectSector")
        Me.ToolStripBtnSelectSector.Margin = New System.Windows.Forms.Padding(0, 1, 4, 2)
        Me.ToolStripBtnSelectSector.Name = "ToolStripBtnSelectSector"
        '
        'CmbGroups
        '
        Me.CmbGroups.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.CmbGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbGroups.DropDownWidth = 218
        resources.ApplyResources(Me.CmbGroups, "CmbGroups")
        Me.CmbGroups.Name = "CmbGroups"
        '
        'LblGroups
        '
        Me.LblGroups.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.LblGroups.Name = "LblGroups"
        resources.ApplyResources(Me.LblGroups, "LblGroups")
        '
        'ToolStripBtnSelectTrack
        '
        resources.ApplyResources(Me.ToolStripBtnSelectTrack, "ToolStripBtnSelectTrack")
        Me.ToolStripBtnSelectTrack.Margin = New System.Windows.Forms.Padding(0, 1, 4, 2)
        Me.ToolStripBtnSelectTrack.Name = "ToolStripBtnSelectTrack"
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnUndo, Me.BtnRedo, ToolStripSeparator3, Me.BtnCopyText, Me.BtnCopyHex, Me.BtnCopyHexFormatted, Me.BtnPaste, ToolStripSeparator2, Me.BtnFind, Me.BtnFindNext, Me.ToolStripMenuItem1, Me.BtnDelete, Me.BtnFillF6, Me.BtnFill, ToolStripSeparator1, Me.BtnSelectSector, Me.BtnSelectTrack, Me.BtnSelectAll})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
        '
        'BtnUndo
        '
        resources.ApplyResources(Me.BtnUndo, "BtnUndo")
        Me.BtnUndo.Name = "BtnUndo"
        '
        'BtnRedo
        '
        resources.ApplyResources(Me.BtnRedo, "BtnRedo")
        Me.BtnRedo.Name = "BtnRedo"
        '
        'BtnCopyText
        '
        resources.ApplyResources(Me.BtnCopyText, "BtnCopyText")
        Me.BtnCopyText.Name = "BtnCopyText"
        '
        'BtnCopyHex
        '
        resources.ApplyResources(Me.BtnCopyHex, "BtnCopyHex")
        Me.BtnCopyHex.Name = "BtnCopyHex"
        '
        'BtnCopyHexFormatted
        '
        resources.ApplyResources(Me.BtnCopyHexFormatted, "BtnCopyHexFormatted")
        Me.BtnCopyHexFormatted.Name = "BtnCopyHexFormatted"
        '
        'BtnPaste
        '
        resources.ApplyResources(Me.BtnPaste, "BtnPaste")
        Me.BtnPaste.Name = "BtnPaste"
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
        'BtnDelete
        '
        resources.ApplyResources(Me.BtnDelete, "BtnDelete")
        Me.BtnDelete.Name = "BtnDelete"
        '
        'BtnFillF6
        '
        resources.ApplyResources(Me.BtnFillF6, "BtnFillF6")
        Me.BtnFillF6.Name = "BtnFillF6"
        '
        'BtnFill
        '
        Me.BtnFill.Name = "BtnFill"
        resources.ApplyResources(Me.BtnFill, "BtnFill")
        '
        'BtnSelectSector
        '
        resources.ApplyResources(Me.BtnSelectSector, "BtnSelectSector")
        Me.BtnSelectSector.Name = "BtnSelectSector"
        '
        'BtnSelectTrack
        '
        resources.ApplyResources(Me.BtnSelectTrack, "BtnSelectTrack")
        Me.BtnSelectTrack.Name = "BtnSelectTrack"
        '
        'BtnSelectAll
        '
        resources.ApplyResources(Me.BtnSelectAll, "BtnSelectAll")
        Me.BtnSelectAll.Name = "BtnSelectAll"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusOffset, Me.ToolStripStatusBlock, Me.ToolStripStatusLength, ToolStripStatusGap, Me.ToolStripStatusCluster, Me.ToolStripStatusSector, ToolStripStatusLabel1, Me.ToolStripStatusTrack, Me.ToolStripStatusSide, Me.ToolStripStatusTrackSector, ToolStripStatusLabel2, Me.ToolStripStatusBytes})
        resources.ApplyResources(Me.StatusStrip1, "StatusStrip1")
        Me.StatusStrip1.Name = "StatusStrip1"
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
        'ToolStripStatusCluster
        '
        Me.ToolStripStatusCluster.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusCluster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusCluster.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusCluster.Name = "ToolStripStatusCluster"
        resources.ApplyResources(Me.ToolStripStatusCluster, "ToolStripStatusCluster")
        '
        'ToolStripStatusSector
        '
        Me.ToolStripStatusSector.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusSector.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusSector.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusSector.Name = "ToolStripStatusSector"
        resources.ApplyResources(Me.ToolStripStatusSector, "ToolStripStatusSector")
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
        'ToolStripStatusBytes
        '
        Me.ToolStripStatusBytes.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.ToolStripStatusBytes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusBytes.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusBytes.Name = "ToolStripStatusBytes"
        resources.ApplyResources(Me.ToolStripStatusBytes, "ToolStripStatusBytes")
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
        'HexViewForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.DataGridDataInspector)
        Me.Controls.Add(Me.ToolStripMain)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.HexBox1)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "HexViewForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
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
    Friend WithEvents ToolStripBtnSelectTrack As ToolStripButton
    Friend WithEvents ToolStripStatusTrackSector As ToolStripStatusLabel
    Friend WithEvents BtnFill As ToolStripMenuItem
End Class
