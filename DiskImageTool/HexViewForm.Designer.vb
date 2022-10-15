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
        Me.FlowLayoutHeader = New System.Windows.Forms.FlowLayoutPanel()
        Me.LblHeaderCaption = New System.Windows.Forms.Label()
        Me.CmbGroups = New System.Windows.Forms.ComboBox()
        Me.BtnClear = New System.Windows.Forms.Button()
        Me.ComboBytes = New System.Windows.Forms.ComboBox()
        Me.HexBox1 = New Hb.Windows.Forms.HexBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusRange = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusCluster = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusSector = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusBytes = New System.Windows.Forms.ToolStripStatusLabel()
        Me.FlowLayoutHeader.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
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
        Me.FlowLayoutHeader.Controls.Add(Me.ComboBytes)
        Me.FlowLayoutHeader.Location = New System.Drawing.Point(133, 12)
        Me.FlowLayoutHeader.Name = "FlowLayoutHeader"
        Me.FlowLayoutHeader.Size = New System.Drawing.Size(403, 29)
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
        Me.CmbGroups.Size = New System.Drawing.Size(202, 21)
        Me.CmbGroups.TabIndex = 1
        '
        'BtnClear
        '
        Me.BtnClear.Location = New System.Drawing.Point(266, 3)
        Me.BtnClear.Margin = New System.Windows.Forms.Padding(8, 3, 3, 3)
        Me.BtnClear.Name = "BtnClear"
        Me.BtnClear.Size = New System.Drawing.Size(87, 23)
        Me.BtnClear.TabIndex = 2
        Me.BtnClear.Text = "Clear Sector"
        Me.BtnClear.UseVisualStyleBackColor = True
        Me.BtnClear.Visible = False
        '
        'ComboBytes
        '
        Me.ComboBytes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBytes.FormattingEnabled = True
        Me.ComboBytes.Items.AddRange(New Object() {"F6", "00"})
        Me.ComboBytes.Location = New System.Drawing.Point(359, 4)
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
        Me.HexBox1.ColumnInfoVisible = True
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
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusRange, Me.ToolStripStatusCluster, Me.ToolStripStatusSector, Me.ToolStripStatusBytes})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 569)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(669, 22)
        Me.StatusStrip1.TabIndex = 2
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusRange
        '
        Me.ToolStripStatusRange.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusRange.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusRange.Name = "ToolStripStatusRange"
        Me.ToolStripStatusRange.Size = New System.Drawing.Size(114, 17)
        Me.ToolStripStatusRange.Text = "00000000 - 00000000"
        '
        'ToolStripStatusCluster
        '
        Me.ToolStripStatusCluster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusCluster.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusCluster.Name = "ToolStripStatusCluster"
        Me.ToolStripStatusCluster.Size = New System.Drawing.Size(53, 17)
        Me.ToolStripStatusCluster.Text = "Cluster 0"
        Me.ToolStripStatusCluster.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToolStripStatusSector
        '
        Me.ToolStripStatusSector.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusSector.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusSector.Name = "ToolStripStatusSector"
        Me.ToolStripStatusSector.Size = New System.Drawing.Size(49, 17)
        Me.ToolStripStatusSector.Text = "Sector 0"
        Me.ToolStripStatusSector.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ToolStripStatusBytes
        '
        Me.ToolStripStatusBytes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripStatusBytes.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
        Me.ToolStripStatusBytes.Name = "ToolStripStatusBytes"
        Me.ToolStripStatusBytes.Size = New System.Drawing.Size(422, 17)
        Me.ToolStripStatusBytes.Spring = True
        Me.ToolStripStatusBytes.Text = "0 Bytes"
        Me.ToolStripStatusBytes.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'HexViewForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(669, 591)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.HexBox1)
        Me.Controls.Add(Me.FlowLayoutHeader)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(685, 1280)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(685, 480)
        Me.Name = "HexViewForm"
        Me.ShowIcon = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Hex Viewer"
        Me.FlowLayoutHeader.ResumeLayout(False)
        Me.FlowLayoutHeader.PerformLayout()
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
    Friend WithEvents ToolStripStatusRange As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusBytes As ToolStripStatusLabel
End Class
