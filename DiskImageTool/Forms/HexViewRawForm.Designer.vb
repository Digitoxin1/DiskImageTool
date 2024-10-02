<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HexViewRawForm
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
        Dim ToolStripStatusGap As System.Windows.Forms.ToolStripStatusLabel
        Dim ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HexViewRawForm))
        Dim ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
        Me.DataGridDataInspector = New System.Windows.Forms.DataGridView()
        Me.DataGridName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridValue = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridLength = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridInvalid = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridEditable = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridType = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ContextMenuStrip2 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnCopyValue = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusOffset = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusBlock = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLength = New System.Windows.Forms.ToolStripStatusLabel()
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
        Me.ToolStripBtnCopyText = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnCopyHex = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnCopyHexFormatted = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnFind = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtnFindNext = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripBtnSelectAll = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripMain = New System.Windows.Forms.ToolStrip()
        ToolStripStatusGap = New System.Windows.Forms.ToolStripStatusLabel()
        ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        CType(Me.DataGridDataInspector, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip2.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.ToolStripMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStripStatusGap
        '
        ToolStripStatusGap.Name = "ToolStripStatusGap"
        ToolStripStatusGap.Size = New System.Drawing.Size(642, 19)
        ToolStripStatusGap.Spring = True
        '
        'ToolStripSeparator2
        '
        ToolStripSeparator2.Name = "ToolStripSeparator2"
        ToolStripSeparator2.Size = New System.Drawing.Size(255, 6)
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
        Me.DataGridDataInspector.Location = New System.Drawing.Point(668, 28)
        Me.DataGridDataInspector.MultiSelect = False
        Me.DataGridDataInspector.Name = "DataGridDataInspector"
        Me.DataGridDataInspector.RowHeadersVisible = False
        Me.DataGridDataInspector.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
        Me.DataGridDataInspector.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.DataGridDataInspector.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridDataInspector.Size = New System.Drawing.Size(264, 518)
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
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusOffset, Me.ToolStripStatusBlock, Me.ToolStripStatusLength, ToolStripStatusGap, Me.ToolStripStatusBytes})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 549)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(944, 24)
        Me.StatusStrip1.TabIndex = 3
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
        Me.HexBox1.Size = New System.Drawing.Size(650, 518)
        Me.HexBox1.StringViewVisible = True
        Me.HexBox1.TabIndex = 1
        Me.HexBox1.UseFixedBytesPerLine = True
        Me.HexBox1.VScrollBarVisible = False
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnCopyText, Me.BtnCopyHex, Me.BtnCopyHexFormatted, ToolStripSeparator2, Me.BtnFind, Me.BtnFindNext, Me.ToolStripMenuItem1, Me.BtnSelectAll})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(259, 148)
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
        'ToolStripSeparator5
        '
        ToolStripSeparator5.Name = "ToolStripSeparator5"
        ToolStripSeparator5.Size = New System.Drawing.Size(6, 25)
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
        Me.ToolStripMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtnCopyText, Me.ToolStripBtnCopyHex, Me.ToolStripBtnCopyHexFormatted, ToolStripSeparator5, Me.ToolStripBtnFind, Me.ToolStripBtnFindNext, Me.ToolStripSeparator8, Me.ToolStripBtnSelectAll})
        Me.ToolStripMain.Location = New System.Drawing.Point(0, 0)
        Me.ToolStripMain.Name = "ToolStripMain"
        Me.ToolStripMain.Padding = New System.Windows.Forms.Padding(12, 0, 12, 0)
        Me.ToolStripMain.Size = New System.Drawing.Size(944, 25)
        Me.ToolStripMain.TabIndex = 0
        Me.ToolStripMain.Text = "ToolStrip1"
        '
        'HexViewRawForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(944, 573)
        Me.Controls.Add(Me.ToolStripMain)
        Me.Controls.Add(Me.DataGridDataInspector)
        Me.Controls.Add(Me.StatusStrip1)
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
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
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
    Friend WithEvents StatusStrip1 As StatusStrip
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
End Class
