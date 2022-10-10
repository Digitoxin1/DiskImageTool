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
        Dim HexOffset As System.Windows.Forms.ColumnHeader
        Dim HexData As System.Windows.Forms.ColumnHeader
        Dim HexText As System.Windows.Forms.ColumnHeader
        Dim HexGutter As System.Windows.Forms.ColumnHeader
        Me.ListViewHex = New System.Windows.Forms.ListView()
        HexOffset = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HexData = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HexText = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HexGutter = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.SuspendLayout()
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
        'HexGutter
        '
        HexGutter.Text = ""
        HexGutter.Width = 0
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
        Me.ListViewHex.Location = New System.Drawing.Point(12, 12)
        Me.ListViewHex.MultiSelect = False
        Me.ListViewHex.Name = "ListViewHex"
        Me.ListViewHex.ShowGroups = False
        Me.ListViewHex.Size = New System.Drawing.Size(676, 422)
        Me.ListViewHex.TabIndex = 0
        Me.ListViewHex.UseCompatibleStateImageBehavior = False
        Me.ListViewHex.View = System.Windows.Forms.View.Details
        '
        'HexViewForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(700, 446)
        Me.Controls.Add(Me.ListViewHex)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(716, 1280)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(716, 480)
        Me.Name = "HexViewForm"
        Me.ShowIcon = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Hex Viewer"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ListViewHex As ListView
End Class
