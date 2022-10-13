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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.CmbGroups = New System.Windows.Forms.ComboBox()
        Me.ListViewHex = New DiskImageTool.AdvancedListView()
        HexGutter = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HexOffset = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HexData = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HexText = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(221, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(51, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Jump To:"
        '
        'CmbGroups
        '
        Me.CmbGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbGroups.FormattingEnabled = True
        Me.CmbGroups.Location = New System.Drawing.Point(278, 9)
        Me.CmbGroups.Name = "CmbGroups"
        Me.CmbGroups.Size = New System.Drawing.Size(202, 21)
        Me.CmbGroups.TabIndex = 2
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
        Me.ListViewHex.Location = New System.Drawing.Point(12, 36)
        Me.ListViewHex.MultiSelect = False
        Me.ListViewHex.Name = "ListViewHex"
        Me.ListViewHex.ShowGroups = False
        Me.ListViewHex.Size = New System.Drawing.Size(676, 428)
        Me.ListViewHex.TabIndex = 0
        Me.ListViewHex.UseCompatibleStateImageBehavior = False
        Me.ListViewHex.View = System.Windows.Forms.View.Details
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
        'HexViewForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(700, 476)
        Me.Controls.Add(Me.CmbGroups)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ListViewHex)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(716, 1280)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(716, 480)
        Me.Name = "HexViewForm"
        Me.ShowIcon = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Hex Viewer"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ListViewHex As AdvancedListView
    Friend WithEvents Label1 As Label
    Friend WithEvents CmbGroups As ComboBox
End Class
