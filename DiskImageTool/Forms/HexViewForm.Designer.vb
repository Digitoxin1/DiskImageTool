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
        Dim ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Me.FlowLayoutHeader = New System.Windows.Forms.FlowLayoutPanel()
        Me.LblHeaderCaption = New System.Windows.Forms.Label()
        Me.CmbGroups = New System.Windows.Forms.ComboBox()
        Me.BtnClear = New System.Windows.Forms.Button()
        Me.LblHeaderFill = New System.Windows.Forms.Label()
        Me.ComboBytes = New System.Windows.Forms.ComboBox()
        Me.HexBox1 = New Hb.Windows.Forms.HexBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.BtnCopyText = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnCopyHex = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnCopyHexFormatted = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnSelectAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnCRC32 = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusBlock = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusCluster = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSector = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusGap = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusBytes = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusOffset = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLength = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusFile = New System.Windows.Forms.ToolStripStatusLabel()
        ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.FlowLayoutHeader.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStripSeparator1
        '
        ToolStripSeparator1.Name = "ToolStripSeparator1"
        ToolStripSeparator1.Size = New System.Drawing.Size(181, 6)
        '
        'ToolStripSeparator2
        '
        ToolStripSeparator2.Name = "ToolStripSeparator2"
        ToolStripSeparator2.Size = New System.Drawing.Size(181, 6)
        '
        'FlowLayoutHeader
        '
        Me.FlowLayoutHeader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutHeader.AutoSize = True
        Me.FlowLayoutHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutHeader.Controls.Add(Me.LblHeaderCaption)
        Me.FlowLayoutHeader.Controls.Add(Me.CmbGroups)
        Me.FlowLayoutHeader.Controls.Add(Me.BtnClear)
        Me.FlowLayoutHeader.Controls.Add(Me.LblHeaderFill)
        Me.FlowLayoutHeader.Controls.Add(Me.ComboBytes)
        Me.FlowLayoutHeader.Location = New System.Drawing.Point(98, 12)
        Me.FlowLayoutHeader.Name = "FlowLayoutHeader"
        Me.FlowLayoutHeader.Size = New System.Drawing.Size(472, 29)
        Me.FlowLayoutHeader.TabIndex = 0
        Me.FlowLayoutHeader.WrapContents = False
        '
        'LblHeaderCaption
        '
        Me.LblHeaderCaption.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LblHeaderCaption.AutoSize = True
        Me.LblHeaderCaption.Location = New System.Drawing.Point(3, 0)
        Me.LblHeaderCaption.Name = "LblHeaderCaption"
        Me.LblHeaderCaption.Size = New System.Drawing.Size(44, 29)
        Me.LblHeaderCaption.TabIndex = 0
        Me.LblHeaderCaption.Text = "Display:"
        Me.LblHeaderCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbGroups
        '
        Me.CmbGroups.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CmbGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbGroups.FormattingEnabled = True
        Me.CmbGroups.Location = New System.Drawing.Point(53, 4)
        Me.CmbGroups.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.CmbGroups.Name = "CmbGroups"
        Me.CmbGroups.Size = New System.Drawing.Size(218, 21)
        Me.CmbGroups.TabIndex = 1
        '
        'BtnClear
        '
        Me.BtnClear.AutoSize = True
        Me.BtnClear.Location = New System.Drawing.Point(282, 3)
        Me.BtnClear.Margin = New System.Windows.Forms.Padding(8, 3, 3, 3)
        Me.BtnClear.Name = "BtnClear"
        Me.BtnClear.Size = New System.Drawing.Size(87, 23)
        Me.BtnClear.TabIndex = 2
        Me.BtnClear.Text = "Clear Sector"
        Me.BtnClear.UseVisualStyleBackColor = True
        Me.BtnClear.Visible = False
        '
        'LblHeaderFill
        '
        Me.LblHeaderFill.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LblHeaderFill.AutoSize = True
        Me.LblHeaderFill.Location = New System.Drawing.Point(375, 0)
        Me.LblHeaderFill.Name = "LblHeaderFill"
        Me.LblHeaderFill.Size = New System.Drawing.Size(47, 29)
        Me.LblHeaderFill.TabIndex = 5
        Me.LblHeaderFill.Text = "Fill With:"
        Me.LblHeaderFill.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ComboBytes
        '
        Me.ComboBytes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBytes.FormattingEnabled = True
        Me.ComboBytes.Items.AddRange(New Object() {"F6", "00"})
        Me.ComboBytes.Location = New System.Drawing.Point(428, 4)
        Me.ComboBytes.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.ComboBytes.Name = "ComboBytes"
        Me.ComboBytes.Size = New System.Drawing.Size(41, 21)
        Me.ComboBytes.TabIndex = 4
        Me.ComboBytes.Visible = False
        '
        'HexBox1
        '
        Me.HexBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
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
        Me.HexBox1.Location = New System.Drawing.Point(12, 47)
        Me.HexBox1.Name = "HexBox1"
        Me.HexBox1.ReadOnly = True
        Me.HexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(60, Byte), Integer), CType(CType(188, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.HexBox1.Size = New System.Drawing.Size(645, 519)
        Me.HexBox1.StringViewVisible = True
        Me.HexBox1.TabIndex = 1
        Me.HexBox1.UseFixedBytesPerLine = True
        Me.HexBox1.VScrollBarVisible = False
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BtnCopyText, Me.BtnCopyHex, Me.BtnCopyHexFormatted, ToolStripSeparator1, Me.BtnSelectAll, ToolStripSeparator2, Me.BtnCRC32})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(185, 126)
        '
        'BtnCopyText
        '
        Me.BtnCopyText.Name = "BtnCopyText"
        Me.BtnCopyText.Size = New System.Drawing.Size(184, 22)
        Me.BtnCopyText.Text = "Copy &Text"
        '
        'BtnCopyHex
        '
        Me.BtnCopyHex.Name = "BtnCopyHex"
        Me.BtnCopyHex.Size = New System.Drawing.Size(184, 22)
        Me.BtnCopyHex.Text = "Copy &Hex"
        '
        'BtnCopyHexFormatted
        '
        Me.BtnCopyHexFormatted.Name = "BtnCopyHexFormatted"
        Me.BtnCopyHexFormatted.Size = New System.Drawing.Size(184, 22)
        Me.BtnCopyHexFormatted.Text = "Copy Hex &Formatted"
        '
        'BtnSelectAll
        '
        Me.BtnSelectAll.Name = "BtnSelectAll"
        Me.BtnSelectAll.Size = New System.Drawing.Size(184, 22)
        Me.BtnSelectAll.Text = "Select &All"
        '
        'BtnCRC32
        '
        Me.BtnCRC32.Name = "BtnCRC32"
        Me.BtnCRC32.Size = New System.Drawing.Size(184, 22)
        Me.BtnCRC32.Text = "Calculate CRC32"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusFile, Me.ToolStripStatusOffset, Me.ToolStripStatusBlock, Me.ToolStripStatusLength, Me.ToolStripStatusGap, Me.ToolStripStatusCluster, Me.ToolStripStatusSector, Me.ToolStripStatusBytes})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 569)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(669, 24)
        Me.StatusStrip1.TabIndex = 2
        Me.StatusStrip1.Text = "StatusStrip1"
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
        'ToolStripStatusGap
        '
        Me.ToolStripStatusGap.Name = "ToolStripStatusGap"
        Me.ToolStripStatusGap.Size = New System.Drawing.Size(213, 19)
        Me.ToolStripStatusGap.Spring = True
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
        'ToolStripStatusFile
        '
        Me.ToolStripStatusFile.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
        Me.ToolStripStatusFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusFile.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusFile.Name = "ToolStripStatusFile"
        Me.ToolStripStatusFile.Size = New System.Drawing.Size(32, 19)
        Me.ToolStripStatusFile.Text = "File:"
        Me.ToolStripStatusFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'HexViewForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(669, 593)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.HexBox1)
        Me.Controls.Add(Me.FlowLayoutHeader)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(685, 1280)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(685, 480)
        Me.Name = "HexViewForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Hex Viewer"
        Me.FlowLayoutHeader.ResumeLayout(False)
        Me.FlowLayoutHeader.PerformLayout()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents FlowLayoutHeader As FlowLayoutPanel
    Friend WithEvents LblHeaderCaption As Label
    Friend WithEvents CmbGroups As ComboBox
    Friend WithEvents BtnClear As Button
    Friend WithEvents ComboBytes As ComboBox
    Friend WithEvents HexBox1 As Hb.Windows.Forms.HexBox
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusCluster As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusSector As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusBlock As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusBytes As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusGap As ToolStripStatusLabel
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents BtnCopyText As ToolStripMenuItem
    Friend WithEvents BtnCopyHex As ToolStripMenuItem
    Friend WithEvents BtnSelectAll As ToolStripMenuItem
    Friend WithEvents BtnCopyHexFormatted As ToolStripMenuItem
    Friend WithEvents LblHeaderFill As Label
    Friend WithEvents BtnCRC32 As ToolStripMenuItem
    Friend WithEvents ToolStripStatusOffset As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLength As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusFile As ToolStripStatusLabel
End Class
