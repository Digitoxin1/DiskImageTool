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
        Dim HexGutter As System.Windows.Forms.ColumnHeader
        Dim HexOffset As System.Windows.Forms.ColumnHeader
        Dim HexData As System.Windows.Forms.ColumnHeader
        Dim HexText As System.Windows.Forms.ColumnHeader
        Me.FlowLayoutHeader = New System.Windows.Forms.FlowLayoutPanel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.CmbGroups = New System.Windows.Forms.ComboBox()
        Me.BtnClear = New System.Windows.Forms.Button()
        Me.ComboBytes = New System.Windows.Forms.ComboBox()
        Me.ListViewHex = New DiskImageTool.AdvancedListView()
        HexGutter = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HexOffset = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HexData = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HexText = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FlowLayoutHeader.SuspendLayout()
        Me.SuspendLayout()
        '
        'HexGutter
        '
        HexGutter.Text = ""
        HexGutter.Width = 0
        '
        'HexOffset
        '
        HexOffset.Text = "Offset (h)"
        HexOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        HexOffset.Width = 97
        '
        'HexData
        '
        HexData.Text = "00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F"
        HexData.Width = 395
        '
        'HexText
        '
        HexText.Text = "Decoded text"
        HexText.Width = 150
        '
        'FlowLayoutHeader
        '
        Me.FlowLayoutHeader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutHeader.AutoSize = True
        Me.FlowLayoutHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutHeader.Controls.Add(Me.Label1)
        Me.FlowLayoutHeader.Controls.Add(Me.CmbGroups)
        Me.FlowLayoutHeader.Controls.Add(Me.BtnClear)
        Me.FlowLayoutHeader.Controls.Add(Me.ComboBytes)
        Me.FlowLayoutHeader.Location = New System.Drawing.Point(151, 12)
        Me.FlowLayoutHeader.Name = "FlowLayoutHeader"
        Me.FlowLayoutHeader.Size = New System.Drawing.Size(398, 29)
        Me.FlowLayoutHeader.TabIndex = 3
        Me.FlowLayoutHeader.WrapContents = False
        '
        'Label1
        '
        Me.Label1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(51, 29)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Jump To:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CmbGroups
        '
        Me.CmbGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbGroups.FormattingEnabled = True
        Me.CmbGroups.Location = New System.Drawing.Point(60, 4)
        Me.CmbGroups.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.CmbGroups.Name = "CmbGroups"
        Me.CmbGroups.Size = New System.Drawing.Size(202, 21)
        Me.CmbGroups.TabIndex = 1
        '
        'BtnClear
        '
        Me.BtnClear.Location = New System.Drawing.Point(273, 3)
        Me.BtnClear.Margin = New System.Windows.Forms.Padding(8, 3, 3, 3)
        Me.BtnClear.Name = "BtnClear"
        Me.BtnClear.Size = New System.Drawing.Size(75, 23)
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
        Me.ComboBytes.Location = New System.Drawing.Point(354, 4)
        Me.ComboBytes.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.ComboBytes.Name = "ComboBytes"
        Me.ComboBytes.Size = New System.Drawing.Size(41, 21)
        Me.ComboBytes.TabIndex = 4
        Me.ComboBytes.Visible = False
        '
        'ListViewHex
        '
        Me.ListViewHex.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListViewHex.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {HexGutter, HexOffset, HexData, HexText})
        Me.ListViewHex.Font = New System.Drawing.Font("Courier New", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListViewHex.ForeColor = System.Drawing.SystemColors.WindowText
        Me.ListViewHex.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.ListViewHex.HideSelection = False
        Me.ListViewHex.Location = New System.Drawing.Point(12, 45)
        Me.ListViewHex.MultiSelect = False
        Me.ListViewHex.Name = "ListViewHex"
        Me.ListViewHex.ShowGroups = False
        Me.ListViewHex.Size = New System.Drawing.Size(676, 431)
        Me.ListViewHex.TabIndex = 0
        Me.ListViewHex.UseCompatibleStateImageBehavior = False
        Me.ListViewHex.View = System.Windows.Forms.View.Details
        '
        'HexViewForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(700, 488)
        Me.Controls.Add(Me.FlowLayoutHeader)
        Me.Controls.Add(Me.ListViewHex)
        Me.DoubleBuffered = True
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(716, 1280)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(716, 480)
        Me.Name = "HexViewForm"
        Me.ShowIcon = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Hex Viewer"
        Me.FlowLayoutHeader.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ListViewHex As AdvancedListView
    Friend WithEvents FlowLayoutHeader As FlowLayoutPanel
    Friend WithEvents Label1 As Label
    Friend WithEvents CmbGroups As ComboBox
    Friend WithEvents BtnClear As Button
    Friend WithEvents ComboBytes As ComboBox
End Class
