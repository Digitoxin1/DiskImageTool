<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ImportFileForm
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
        Dim FileSize As System.Windows.Forms.ColumnHeader
        Dim FileLastWriteDate As System.Windows.Forms.ColumnHeader
        Dim FileSizeOnDisk As System.Windows.Forms.ColumnHeader
        Dim Label3 As System.Windows.Forms.Label
        Dim Label2 As System.Windows.Forms.Label
        Dim Label1 As System.Windows.Forms.Label
        Dim PanelTop As System.Windows.Forms.FlowLayoutPanel
        Dim Label4 As System.Windows.Forms.Label
        Dim FileCreationDate As System.Windows.Forms.ColumnHeader
        Dim FileLastAccessDate As System.Windows.Forms.ColumnHeader
        Dim FileDisabled As System.Windows.Forms.ColumnHeader
        Me.ChkLFN = New System.Windows.Forms.CheckBox()
        Me.ChkNTExtensions = New System.Windows.Forms.CheckBox()
        Me.ChkCreated = New System.Windows.Forms.CheckBox()
        Me.ChkLastAccessed = New System.Windows.Forms.CheckBox()
        Me.ListViewFiles = New System.Windows.Forms.ListView()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.FlowLayoutTotals = New System.Windows.Forms.FlowLayoutPanel()
        Me.LblSelected = New System.Windows.Forms.Label()
        Me.LblBytesFree = New System.Windows.Forms.Label()
        Me.LblBytesRequired = New System.Windows.Forms.Label()
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        FileName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileSize = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileLastWriteDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileSizeOnDisk = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Label3 = New System.Windows.Forms.Label()
        Label2 = New System.Windows.Forms.Label()
        Label1 = New System.Windows.Forms.Label()
        PanelTop = New System.Windows.Forms.FlowLayoutPanel()
        Label4 = New System.Windows.Forms.Label()
        FileCreationDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileLastAccessDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileDisabled = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        PanelTop.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.FlowLayoutTotals.SuspendLayout()
        Me.SuspendLayout()
        '
        'FileName
        '
        FileName.Text = "Filename"
        FileName.Width = 120
        '
        'FileSize
        '
        FileSize.Text = "Size"
        FileSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        FileSize.Width = 90
        '
        'FileLastWriteDate
        '
        FileLastWriteDate.Text = "Last Written"
        FileLastWriteDate.Width = 120
        '
        'FileSizeOnDisk
        '
        FileSizeOnDisk.Text = "Size on Disk"
        FileSizeOnDisk.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        FileSizeOnDisk.Width = 90
        '
        'Label3
        '
        Label3.AutoSize = True
        Label3.Location = New System.Drawing.Point(3, 0)
        Label3.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Label3.Name = "Label3"
        Label3.Size = New System.Drawing.Size(76, 13)
        Label3.TabIndex = 8
        Label3.Text = "Files Selected:"
        Label3.UseMnemonic = False
        '
        'Label2
        '
        Label2.AutoSize = True
        Label2.Location = New System.Drawing.Point(170, 0)
        Label2.Margin = New System.Windows.Forms.Padding(9, 0, 0, 0)
        Label2.Name = "Label2"
        Label2.Size = New System.Drawing.Size(60, 13)
        Label2.TabIndex = 7
        Label2.Text = "Bytes Free:"
        Label2.UseMnemonic = False
        '
        'Label1
        '
        Label1.AutoSize = True
        Label1.Location = New System.Drawing.Point(318, 0)
        Label1.Margin = New System.Windows.Forms.Padding(9, 0, 0, 0)
        Label1.Name = "Label1"
        Label1.Size = New System.Drawing.Size(82, 13)
        Label1.TabIndex = 6
        Label1.Text = "Bytes Required:"
        Label1.UseMnemonic = False
        '
        'PanelTop
        '
        PanelTop.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        PanelTop.BackColor = System.Drawing.SystemColors.Control
        PanelTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        PanelTop.Controls.Add(Me.ChkLFN)
        PanelTop.Controls.Add(Me.ChkNTExtensions)
        PanelTop.Controls.Add(Me.ChkCreated)
        PanelTop.Controls.Add(Me.ChkLastAccessed)
        PanelTop.Location = New System.Drawing.Point(12, 12)
        PanelTop.Name = "PanelTop"
        PanelTop.Padding = New System.Windows.Forms.Padding(6, 12, 0, 0)
        PanelTop.Size = New System.Drawing.Size(760, 46)
        PanelTop.TabIndex = 0
        '
        'ChkLFN
        '
        Me.ChkLFN.AutoSize = True
        Me.ChkLFN.Location = New System.Drawing.Point(9, 15)
        Me.ChkLFN.Name = "ChkLFN"
        Me.ChkLFN.Size = New System.Drawing.Size(105, 17)
        Me.ChkLFN.TabIndex = 0
        Me.ChkLFN.Text = "Long File Names"
        Me.ChkLFN.UseVisualStyleBackColor = True
        '
        'ChkNTExtensions
        '
        Me.ChkNTExtensions.AutoSize = True
        Me.ChkNTExtensions.Location = New System.Drawing.Point(120, 15)
        Me.ChkNTExtensions.Name = "ChkNTExtensions"
        Me.ChkNTExtensions.Size = New System.Drawing.Size(95, 17)
        Me.ChkNTExtensions.TabIndex = 1
        Me.ChkNTExtensions.Text = "NT Extensions"
        Me.ChkNTExtensions.UseVisualStyleBackColor = True
        '
        'ChkCreated
        '
        Me.ChkCreated.AutoSize = True
        Me.ChkCreated.Location = New System.Drawing.Point(221, 15)
        Me.ChkCreated.Name = "ChkCreated"
        Me.ChkCreated.Size = New System.Drawing.Size(89, 17)
        Me.ChkCreated.TabIndex = 2
        Me.ChkCreated.Text = "Created Date"
        Me.ChkCreated.UseVisualStyleBackColor = True
        '
        'ChkLastAccessed
        '
        Me.ChkLastAccessed.AutoSize = True
        Me.ChkLastAccessed.Location = New System.Drawing.Point(316, 15)
        Me.ChkLastAccessed.Name = "ChkLastAccessed"
        Me.ChkLastAccessed.Size = New System.Drawing.Size(122, 17)
        Me.ChkLastAccessed.TabIndex = 3
        Me.ChkLastAccessed.Text = "Last Accessed Date"
        Me.ChkLastAccessed.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Label4.AutoSize = True
        Label4.Location = New System.Drawing.Point(18, 5)
        Label4.Name = "Label4"
        Label4.Size = New System.Drawing.Size(43, 13)
        Label4.TabIndex = 3
        Label4.Text = "Options"
        Label4.UseMnemonic = False
        '
        'FileCreationDate
        '
        FileCreationDate.Text = "Created"
        FileCreationDate.Width = 120
        '
        'FileLastAccessDate
        '
        FileLastAccessDate.Text = "Last Accessed"
        FileLastAccessDate.Width = 90
        '
        'FileDisabled
        '
        FileDisabled.Text = "Disabled"
        FileDisabled.Width = 0
        '
        'ListViewFiles
        '
        Me.ListViewFiles.AllowDrop = True
        Me.ListViewFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListViewFiles.CheckBoxes = True
        Me.ListViewFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {FileName, FileSize, FileSizeOnDisk, FileLastWriteDate, FileCreationDate, FileLastAccessDate, FileDisabled})
        Me.ListViewFiles.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ListViewFiles.FullRowSelect = True
        Me.ListViewFiles.HideSelection = False
        Me.ListViewFiles.Location = New System.Drawing.Point(12, 64)
        Me.ListViewFiles.Name = "ListViewFiles"
        Me.ListViewFiles.Size = New System.Drawing.Size(760, 452)
        Me.ListViewFiles.TabIndex = 1
        Me.ListViewFiles.UseCompatibleStateImageBehavior = False
        Me.ListViewFiles.View = System.Windows.Forms.View.Details
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.Control
        Me.Panel1.Controls.Add(Me.FlowLayoutTotals)
        Me.Panel1.Controls.Add(Me.BtnOK)
        Me.Panel1.Controls.Add(Me.BtnCancel)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 519)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(3, 3, 50, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(784, 42)
        Me.Panel1.TabIndex = 2
        '
        'FlowLayoutTotals
        '
        Me.FlowLayoutTotals.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutTotals.Controls.Add(Label3)
        Me.FlowLayoutTotals.Controls.Add(Me.LblSelected)
        Me.FlowLayoutTotals.Controls.Add(Label2)
        Me.FlowLayoutTotals.Controls.Add(Me.LblBytesFree)
        Me.FlowLayoutTotals.Controls.Add(Label1)
        Me.FlowLayoutTotals.Controls.Add(Me.LblBytesRequired)
        Me.FlowLayoutTotals.Location = New System.Drawing.Point(12, 17)
        Me.FlowLayoutTotals.Name = "FlowLayoutTotals"
        Me.FlowLayoutTotals.Size = New System.Drawing.Size(591, 13)
        Me.FlowLayoutTotals.TabIndex = 6
        '
        'LblSelected
        '
        Me.LblSelected.Location = New System.Drawing.Point(79, 0)
        Me.LblSelected.Margin = New System.Windows.Forms.Padding(0)
        Me.LblSelected.Name = "LblSelected"
        Me.LblSelected.Size = New System.Drawing.Size(82, 13)
        Me.LblSelected.TabIndex = 3
        Me.LblSelected.Text = "00000 of 00000"
        Me.LblSelected.UseMnemonic = False
        '
        'LblBytesFree
        '
        Me.LblBytesFree.Location = New System.Drawing.Point(230, 0)
        Me.LblBytesFree.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.LblBytesFree.Name = "LblBytesFree"
        Me.LblBytesFree.Size = New System.Drawing.Size(76, 13)
        Me.LblBytesFree.TabIndex = 4
        Me.LblBytesFree.Text = "9,999,999"
        Me.LblBytesFree.UseMnemonic = False
        '
        'LblBytesRequired
        '
        Me.LblBytesRequired.Location = New System.Drawing.Point(400, 0)
        Me.LblBytesRequired.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.LblBytesRequired.Name = "LblBytesRequired"
        Me.LblBytesRequired.Size = New System.Drawing.Size(82, 13)
        Me.LblBytesRequired.TabIndex = 5
        Me.LblBytesRequired.Text = "9,999,999,999"
        Me.LblBytesRequired.UseMnemonic = False
        '
        'BtnOK
        '
        Me.BtnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Location = New System.Drawing.Point(610, 10)
        Me.BtnOK.Margin = New System.Windows.Forms.Padding(4, 10, 4, 9)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(75, 23)
        Me.BtnOK.TabIndex = 0
        Me.BtnOK.Text = "Import"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        Me.BtnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(693, 10)
        Me.BtnCancel.Margin = New System.Windows.Forms.Padding(4, 10, 4, 9)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "Cancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'ImportFileForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 561)
        Me.Controls.Add(Label4)
        Me.Controls.Add(PanelTop)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.ListViewFiles)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 480)
        Me.Name = "ImportFileForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Import Files"
        PanelTop.ResumeLayout(False)
        PanelTop.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.FlowLayoutTotals.ResumeLayout(False)
        Me.FlowLayoutTotals.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ListViewFiles As ListView
    Friend WithEvents Panel1 As Panel
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents FlowLayoutTotals As FlowLayoutPanel
    Friend WithEvents LblSelected As Label
    Friend WithEvents LblBytesFree As Label
    Friend WithEvents LblBytesRequired As Label
    Friend WithEvents ChkLastAccessed As CheckBox
    Friend WithEvents ChkCreated As CheckBox
    Friend WithEvents ChkLFN As CheckBox
    Friend WithEvents ChkNTExtensions As CheckBox
End Class
