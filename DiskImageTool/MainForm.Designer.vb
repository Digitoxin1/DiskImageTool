<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
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
        Dim FileName As System.Windows.Forms.ColumnHeader
        Dim FileExt As System.Windows.Forms.ColumnHeader
        Dim FileSize As System.Windows.Forms.ColumnHeader
        Dim FileLastWriteDate As System.Windows.Forms.ColumnHeader
        Dim FileStartingCluster As System.Windows.Forms.ColumnHeader
        Dim FileAttrib As System.Windows.Forms.ColumnHeader
        Dim FileCRC32 As System.Windows.Forms.ColumnHeader
        Dim SummaryName As System.Windows.Forms.ColumnHeader
        Dim SummaryValue As System.Windows.Forms.ColumnHeader
        Dim HashName As System.Windows.Forms.ColumnHeader
        Dim HashValue As System.Windows.Forms.ColumnHeader
        Me.ListViewFiles = New System.Windows.Forms.ListView()
        Me.ListViewSummary = New System.Windows.Forms.ListView()
        Me.ComboGroups = New System.Windows.Forms.ComboBox()
        Me.LblInvalidImage = New System.Windows.Forms.Label()
        Me.ListViewHashes = New System.Windows.Forms.ListView()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ButtonOEMID = New System.Windows.Forms.Button()
        Me.ButtonDisplayBootSector = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.BtnSave = New System.Windows.Forms.Button()
        FileName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileExt = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileSize = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileLastWriteDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileStartingCluster = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileAttrib = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileCRC32 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        SummaryName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        SummaryValue = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HashName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        HashValue = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'FileName
        '
        FileName.Text = "Name"
        FileName.Width = 120
        '
        'FileExt
        '
        FileExt.Text = "Ext"
        FileExt.Width = 50
        '
        'FileSize
        '
        FileSize.Text = "Size"
        FileSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        FileSize.Width = 80
        '
        'FileLastWriteDate
        '
        FileLastWriteDate.Text = "Last Written"
        FileLastWriteDate.Width = 140
        '
        'FileStartingCluster
        '
        FileStartingCluster.Text = "Cluster"
        FileStartingCluster.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FileAttrib
        '
        FileAttrib.Text = "Attrib"
        FileAttrib.Width = 70
        '
        'FileCRC32
        '
        FileCRC32.Text = "CRC32"
        FileCRC32.Width = 70
        '
        'SummaryName
        '
        SummaryName.Text = "Name"
        SummaryName.Width = 130
        '
        'SummaryValue
        '
        SummaryValue.Text = "Value"
        SummaryValue.Width = 140
        '
        'HashName
        '
        HashName.Width = -1
        '
        'HashValue
        '
        HashValue.Width = -1
        '
        'ListViewFiles
        '
        Me.ListViewFiles.AllowDrop = True
        Me.ListViewFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListViewFiles.CheckBoxes = True
        Me.ListViewFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {FileName, FileExt, FileSize, FileLastWriteDate, FileStartingCluster, FileAttrib, FileCRC32})
        Me.ListViewFiles.FullRowSelect = True
        Me.ListViewFiles.HideSelection = False
        Me.ListViewFiles.Location = New System.Drawing.Point(320, 40)
        Me.ListViewFiles.Name = "ListViewFiles"
        Me.ListViewFiles.Size = New System.Drawing.Size(615, 471)
        Me.ListViewFiles.TabIndex = 2
        Me.ListViewFiles.UseCompatibleStateImageBehavior = False
        Me.ListViewFiles.View = System.Windows.Forms.View.Details
        '
        'ListViewSummary
        '
        Me.ListViewSummary.AllowDrop = True
        Me.ListViewSummary.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListViewSummary.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {SummaryName, SummaryValue})
        Me.ListViewSummary.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.ListViewSummary.HideSelection = False
        Me.ListViewSummary.Location = New System.Drawing.Point(12, 12)
        Me.ListViewSummary.MultiSelect = False
        Me.ListViewSummary.Name = "ListViewSummary"
        Me.ListViewSummary.ShowGroups = False
        Me.ListViewSummary.Size = New System.Drawing.Size(302, 208)
        Me.ListViewSummary.TabIndex = 0
        Me.ListViewSummary.UseCompatibleStateImageBehavior = False
        Me.ListViewSummary.View = System.Windows.Forms.View.Details
        '
        'ComboGroups
        '
        Me.ComboGroups.AllowDrop = True
        Me.ComboGroups.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboGroups.FormattingEnabled = True
        Me.ComboGroups.Location = New System.Drawing.Point(320, 13)
        Me.ComboGroups.Name = "ComboGroups"
        Me.ComboGroups.Size = New System.Drawing.Size(523, 21)
        Me.ComboGroups.Sorted = True
        Me.ComboGroups.TabIndex = 1
        '
        'LblInvalidImage
        '
        Me.LblInvalidImage.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.LblInvalidImage.AutoSize = True
        Me.LblInvalidImage.BackColor = System.Drawing.SystemColors.Window
        Me.LblInvalidImage.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblInvalidImage.ForeColor = System.Drawing.Color.Red
        Me.LblInvalidImage.Location = New System.Drawing.Point(550, 261)
        Me.LblInvalidImage.Name = "LblInvalidImage"
        Me.LblInvalidImage.Size = New System.Drawing.Size(155, 29)
        Me.LblInvalidImage.TabIndex = 3
        Me.LblInvalidImage.Text = "Invalid Image"
        Me.LblInvalidImage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.LblInvalidImage.Visible = False
        '
        'ListViewHashes
        '
        Me.ListViewHashes.AllowDrop = True
        Me.ListViewHashes.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListViewHashes.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {HashName, HashValue})
        Me.ListViewHashes.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListViewHashes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.ListViewHashes.HideSelection = False
        Me.ListViewHashes.Location = New System.Drawing.Point(12, 226)
        Me.ListViewHashes.MultiSelect = False
        Me.ListViewHashes.Name = "ListViewHashes"
        Me.ListViewHashes.Scrollable = False
        Me.ListViewHashes.Size = New System.Drawing.Size(302, 101)
        Me.ListViewHashes.TabIndex = 1
        Me.ListViewHashes.TileSize = New System.Drawing.Size(295, 30)
        Me.ListViewHashes.UseCompatibleStateImageBehavior = False
        Me.ListViewHashes.View = System.Windows.Forms.View.Tile
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Panel1.BackColor = System.Drawing.SystemColors.Window
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.ButtonOEMID)
        Me.Panel1.Controls.Add(Me.ButtonDisplayBootSector)
        Me.Panel1.Controls.Add(Me.Button1)
        Me.Panel1.Location = New System.Drawing.Point(12, 333)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(302, 178)
        Me.Panel1.TabIndex = 2
        '
        'ButtonOEMID
        '
        Me.ButtonOEMID.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ButtonOEMID.Location = New System.Drawing.Point(3, 32)
        Me.ButtonOEMID.Name = "ButtonOEMID"
        Me.ButtonOEMID.Size = New System.Drawing.Size(110, 23)
        Me.ButtonOEMID.TabIndex = 1
        Me.ButtonOEMID.Text = "Change OEM ID"
        Me.ButtonOEMID.UseVisualStyleBackColor = True
        Me.ButtonOEMID.Visible = False
        '
        'ButtonDisplayBootSector
        '
        Me.ButtonDisplayBootSector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ButtonDisplayBootSector.Location = New System.Drawing.Point(3, 3)
        Me.ButtonDisplayBootSector.Name = "ButtonDisplayBootSector"
        Me.ButtonDisplayBootSector.Size = New System.Drawing.Size(110, 23)
        Me.ButtonDisplayBootSector.TabIndex = 0
        Me.ButtonDisplayBootSector.Text = "Display Boot Sector"
        Me.ButtonDisplayBootSector.UseVisualStyleBackColor = True
        Me.ButtonDisplayBootSector.Visible = False
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(25, 100)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(88, 40)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'BtnSave
        '
        Me.BtnSave.Enabled = False
        Me.BtnSave.Location = New System.Drawing.Point(849, 12)
        Me.BtnSave.Name = "BtnSave"
        Me.BtnSave.Size = New System.Drawing.Size(86, 23)
        Me.BtnSave.TabIndex = 2
        Me.BtnSave.Text = "Save Changes"
        Me.BtnSave.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(947, 523)
        Me.Controls.Add(Me.BtnSave)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.ListViewHashes)
        Me.Controls.Add(Me.LblInvalidImage)
        Me.Controls.Add(Me.ComboGroups)
        Me.Controls.Add(Me.ListViewSummary)
        Me.Controls.Add(Me.ListViewFiles)
        Me.MinimumSize = New System.Drawing.Size(640, 480)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Disk Image Tool"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ListViewFiles As ListView
    Friend WithEvents ListViewSummary As ListView
    Friend WithEvents ComboGroups As ComboBox
    Friend WithEvents LblInvalidImage As Label
    Friend WithEvents ListViewHashes As ListView
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Button1 As Button
    Friend WithEvents ButtonDisplayBootSector As Button
    Friend WithEvents ButtonOEMID As Button
    Friend WithEvents BtnSave As Button
End Class
