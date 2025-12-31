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
        Dim PanelTop As System.Windows.Forms.FlowLayoutPanel
        Dim FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
        Me.ChkLFN = New System.Windows.Forms.CheckBox()
        Me.ChkNTExtensions = New System.Windows.Forms.CheckBox()
        Me.ChkCreated = New System.Windows.Forms.CheckBox()
        Me.ChkLastAccessed = New System.Windows.Forms.CheckBox()
        Me.LabelDirectoryList = New System.Windows.Forms.Label()
        Me.ComboDirectoryList = New System.Windows.Forms.ComboBox()
        Me.FileName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileSizeOnDisk = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileDisabled = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.LabelBytesRequired = New System.Windows.Forms.Label()
        Me.LabelBytesFree = New System.Windows.Forms.Label()
        Me.LabelFilesSelected = New System.Windows.Forms.Label()
        Me.FileLastAccessDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileLastWriteDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.LabelOptions = New System.Windows.Forms.Label()
        Me.FileSize = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.FileCreationDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ListViewFiles = New System.Windows.Forms.ListView()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.FlowLayoutTotals = New System.Windows.Forms.FlowLayoutPanel()
        Me.LblSelected = New System.Windows.Forms.Label()
        Me.LblBytesFree = New System.Windows.Forms.Label()
        Me.LblBytesRequired = New System.Windows.Forms.Label()
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        PanelTop = New System.Windows.Forms.FlowLayoutPanel()
        FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        PanelTop.SuspendLayout()
        FlowLayoutPanel1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.FlowLayoutTotals.SuspendLayout()
        Me.SuspendLayout()
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
        Me.ChkLFN.Size = New System.Drawing.Size(113, 17)
        Me.ChkLFN.TabIndex = 0
        Me.ChkLFN.Text = "{Long File Names}"
        Me.ChkLFN.UseVisualStyleBackColor = True
        '
        'ChkNTExtensions
        '
        Me.ChkNTExtensions.AutoSize = True
        Me.ChkNTExtensions.Location = New System.Drawing.Point(128, 15)
        Me.ChkNTExtensions.Name = "ChkNTExtensions"
        Me.ChkNTExtensions.Size = New System.Drawing.Size(103, 17)
        Me.ChkNTExtensions.TabIndex = 1
        Me.ChkNTExtensions.Text = "{NT Extensions}"
        Me.ChkNTExtensions.UseVisualStyleBackColor = True
        '
        'ChkCreated
        '
        Me.ChkCreated.AutoSize = True
        Me.ChkCreated.Location = New System.Drawing.Point(237, 15)
        Me.ChkCreated.Name = "ChkCreated"
        Me.ChkCreated.Size = New System.Drawing.Size(97, 17)
        Me.ChkCreated.TabIndex = 2
        Me.ChkCreated.Text = "{Created Date}"
        Me.ChkCreated.UseVisualStyleBackColor = True
        '
        'ChkLastAccessed
        '
        Me.ChkLastAccessed.AutoSize = True
        Me.ChkLastAccessed.Location = New System.Drawing.Point(340, 15)
        Me.ChkLastAccessed.Name = "ChkLastAccessed"
        Me.ChkLastAccessed.Size = New System.Drawing.Size(130, 17)
        Me.ChkLastAccessed.TabIndex = 3
        Me.ChkLastAccessed.Text = "{Last Accessed Date}"
        Me.ChkLastAccessed.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel1
        '
        FlowLayoutPanel1.AutoSize = True
        FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        FlowLayoutPanel1.Controls.Add(Me.LabelDirectoryList)
        FlowLayoutPanel1.Controls.Add(Me.ComboDirectoryList)
        FlowLayoutPanel1.Location = New System.Drawing.Point(12, 65)
        FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        FlowLayoutPanel1.Size = New System.Drawing.Size(246, 27)
        FlowLayoutPanel1.TabIndex = 2
        '
        'LabelDirectoryList
        '
        Me.LabelDirectoryList.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LabelDirectoryList.AutoSize = True
        Me.LabelDirectoryList.Location = New System.Drawing.Point(3, 7)
        Me.LabelDirectoryList.Name = "LabelDirectoryList"
        Me.LabelDirectoryList.Size = New System.Drawing.Size(44, 13)
        Me.LabelDirectoryList.TabIndex = 0
        Me.LabelDirectoryList.Text = "{Import}"
        '
        'ComboDirectoryList
        '
        Me.ComboDirectoryList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboDirectoryList.FormattingEnabled = True
        Me.ComboDirectoryList.Location = New System.Drawing.Point(53, 3)
        Me.ComboDirectoryList.Name = "ComboDirectoryList"
        Me.ComboDirectoryList.Size = New System.Drawing.Size(190, 21)
        Me.ComboDirectoryList.TabIndex = 1
        '
        'FileName
        '
        Me.FileName.Text = "{Filename}"
        Me.FileName.Width = 120
        '
        'FileSizeOnDisk
        '
        Me.FileSizeOnDisk.Text = "{Size on Disk}"
        Me.FileSizeOnDisk.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.FileSizeOnDisk.Width = 90
        '
        'FileDisabled
        '
        Me.FileDisabled.Text = "{Disabled}"
        Me.FileDisabled.Width = 0
        '
        'LabelBytesRequired
        '
        Me.LabelBytesRequired.AutoSize = True
        Me.LabelBytesRequired.Location = New System.Drawing.Point(334, 0)
        Me.LabelBytesRequired.Margin = New System.Windows.Forms.Padding(9, 0, 0, 0)
        Me.LabelBytesRequired.Name = "LabelBytesRequired"
        Me.LabelBytesRequired.Size = New System.Drawing.Size(90, 13)
        Me.LabelBytesRequired.TabIndex = 4
        Me.LabelBytesRequired.Text = "{Bytes Required:}"
        Me.LabelBytesRequired.UseMnemonic = False
        '
        'LabelBytesFree
        '
        Me.LabelBytesFree.AutoSize = True
        Me.LabelBytesFree.Location = New System.Drawing.Point(178, 0)
        Me.LabelBytesFree.Margin = New System.Windows.Forms.Padding(9, 0, 0, 0)
        Me.LabelBytesFree.Name = "LabelBytesFree"
        Me.LabelBytesFree.Size = New System.Drawing.Size(68, 13)
        Me.LabelBytesFree.TabIndex = 2
        Me.LabelBytesFree.Text = "{Bytes Free:}"
        Me.LabelBytesFree.UseMnemonic = False
        '
        'LabelFilesSelected
        '
        Me.LabelFilesSelected.AutoSize = True
        Me.LabelFilesSelected.Location = New System.Drawing.Point(3, 0)
        Me.LabelFilesSelected.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.LabelFilesSelected.Name = "LabelFilesSelected"
        Me.LabelFilesSelected.Size = New System.Drawing.Size(84, 13)
        Me.LabelFilesSelected.TabIndex = 0
        Me.LabelFilesSelected.Text = "{Files Selected:}"
        Me.LabelFilesSelected.UseMnemonic = False
        '
        'FileLastAccessDate
        '
        Me.FileLastAccessDate.Text = "{Last Accessed}"
        Me.FileLastAccessDate.Width = 90
        '
        'FileLastWriteDate
        '
        Me.FileLastWriteDate.Text = "{Last Written}"
        Me.FileLastWriteDate.Width = 120
        '
        'LabelOptions
        '
        Me.LabelOptions.AutoSize = True
        Me.LabelOptions.Location = New System.Drawing.Point(18, 5)
        Me.LabelOptions.Name = "LabelOptions"
        Me.LabelOptions.Size = New System.Drawing.Size(51, 13)
        Me.LabelOptions.TabIndex = 1
        Me.LabelOptions.Text = "{Options}"
        Me.LabelOptions.UseMnemonic = False
        '
        'FileSize
        '
        Me.FileSize.Text = "{Size}"
        Me.FileSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.FileSize.Width = 90
        '
        'FileCreationDate
        '
        Me.FileCreationDate.Text = "{Created}"
        Me.FileCreationDate.Width = 120
        '
        'ListViewFiles
        '
        Me.ListViewFiles.AllowDrop = True
        Me.ListViewFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListViewFiles.CheckBoxes = True
        Me.ListViewFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.FileName, Me.FileSize, Me.FileSizeOnDisk, Me.FileLastWriteDate, Me.FileCreationDate, Me.FileLastAccessDate, Me.FileDisabled})
        Me.ListViewFiles.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.ListViewFiles.FullRowSelect = True
        Me.ListViewFiles.HideSelection = False
        Me.ListViewFiles.Location = New System.Drawing.Point(12, 98)
        Me.ListViewFiles.Name = "ListViewFiles"
        Me.ListViewFiles.Size = New System.Drawing.Size(760, 418)
        Me.ListViewFiles.TabIndex = 3
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
        Me.Panel1.TabIndex = 4
        '
        'FlowLayoutTotals
        '
        Me.FlowLayoutTotals.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutTotals.Controls.Add(Me.LabelFilesSelected)
        Me.FlowLayoutTotals.Controls.Add(Me.LblSelected)
        Me.FlowLayoutTotals.Controls.Add(Me.LabelBytesFree)
        Me.FlowLayoutTotals.Controls.Add(Me.LblBytesFree)
        Me.FlowLayoutTotals.Controls.Add(Me.LabelBytesRequired)
        Me.FlowLayoutTotals.Controls.Add(Me.LblBytesRequired)
        Me.FlowLayoutTotals.Location = New System.Drawing.Point(12, 17)
        Me.FlowLayoutTotals.Name = "FlowLayoutTotals"
        Me.FlowLayoutTotals.Size = New System.Drawing.Size(591, 13)
        Me.FlowLayoutTotals.TabIndex = 0
        '
        'LblSelected
        '
        Me.LblSelected.Location = New System.Drawing.Point(87, 0)
        Me.LblSelected.Margin = New System.Windows.Forms.Padding(0)
        Me.LblSelected.Name = "LblSelected"
        Me.LblSelected.Size = New System.Drawing.Size(82, 13)
        Me.LblSelected.TabIndex = 1
        Me.LblSelected.Text = "{FilesSelected}"
        Me.LblSelected.UseMnemonic = False
        '
        'LblBytesFree
        '
        Me.LblBytesFree.Location = New System.Drawing.Point(246, 0)
        Me.LblBytesFree.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.LblBytesFree.Name = "LblBytesFree"
        Me.LblBytesFree.Size = New System.Drawing.Size(76, 13)
        Me.LblBytesFree.TabIndex = 3
        Me.LblBytesFree.Text = "{BytesFree}"
        Me.LblBytesFree.UseMnemonic = False
        '
        'LblBytesRequired
        '
        Me.LblBytesRequired.Location = New System.Drawing.Point(424, 0)
        Me.LblBytesRequired.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.LblBytesRequired.Name = "LblBytesRequired"
        Me.LblBytesRequired.Size = New System.Drawing.Size(82, 13)
        Me.LblBytesRequired.TabIndex = 5
        Me.LblBytesRequired.Text = "{BytesRequired}"
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
        Me.BtnOK.TabIndex = 1
        Me.BtnOK.Text = "{Import}"
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
        Me.BtnCancel.TabIndex = 2
        Me.BtnCancel.Text = "{Cancel}"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'ImportFileForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 561)
        Me.Controls.Add(FlowLayoutPanel1)
        Me.Controls.Add(Me.LabelOptions)
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
        PanelTop.ResumeLayout(False)
        PanelTop.PerformLayout()
        FlowLayoutPanel1.ResumeLayout(False)
        FlowLayoutPanel1.PerformLayout()
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
    Friend WithEvents FileCreationDate As ColumnHeader
    Friend WithEvents FileSize As ColumnHeader
    Friend WithEvents LabelOptions As Label
    Friend WithEvents FileLastWriteDate As ColumnHeader
    Friend WithEvents FileLastAccessDate As ColumnHeader
    Friend WithEvents LabelFilesSelected As Label
    Friend WithEvents LabelBytesFree As Label
    Friend WithEvents LabelBytesRequired As Label
    Friend WithEvents FileDisabled As ColumnHeader
    Friend WithEvents FileSizeOnDisk As ColumnHeader
    Friend WithEvents FileName As ColumnHeader
    Friend WithEvents LabelDirectoryList As Label
    Friend WithEvents ComboDirectoryList As ComboBox
End Class
