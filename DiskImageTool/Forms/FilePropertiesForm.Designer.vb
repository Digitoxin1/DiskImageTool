<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FilePropertiesForm
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
        Dim GroupFileDates As System.Windows.Forms.GroupBox
        Dim GroupAttributes As System.Windows.Forms.GroupBox
        Dim TableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.PanelCreatedMS = New System.Windows.Forms.Panel()
        Me.NumCreatedMS = New System.Windows.Forms.NumericUpDown()
        Me.BtnLastAccessed = New System.Windows.Forms.Button()
        Me.LblLastWritten = New System.Windows.Forms.Label()
        Me.BtnCreated = New System.Windows.Forms.Button()
        Me.LblCreated = New System.Windows.Forms.Label()
        Me.LblLastAccessed = New System.Windows.Forms.Label()
        Me.DTLastAccessed = New System.Windows.Forms.DateTimePicker()
        Me.DTCreated = New System.Windows.Forms.DateTimePicker()
        Me.DTLastWritten = New System.Windows.Forms.DateTimePicker()
        Me.BtnLastWritten = New System.Windows.Forms.Button()
        Me.DTLastWrittenTime = New System.Windows.Forms.DateTimePicker()
        Me.PanelCreatedTime = New System.Windows.Forms.Panel()
        Me.DTCreatedTime = New System.Windows.Forms.DateTimePicker()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.ChkArchive = New System.Windows.Forms.CheckBox()
        Me.ChkSystem = New System.Windows.Forms.CheckBox()
        Me.BtnArchive = New System.Windows.Forms.Button()
        Me.BtnSystem = New System.Windows.Forms.Button()
        Me.BtnReadOnly = New System.Windows.Forms.Button()
        Me.BtnHidden = New System.Windows.Forms.Button()
        Me.ChkReadOnly = New System.Windows.Forms.CheckBox()
        Me.ChkHidden = New System.Windows.Forms.CheckBox()
        Me.LblMultipleFiles = New System.Windows.Forms.Label()
        Me.TxtExtension = New System.Windows.Forms.TextBox()
        Me.TxtFile = New System.Windows.Forms.MaskedTextBox()
        Me.GroupFileName = New System.Windows.Forms.GroupBox()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        Me.MskExtensionHex = New DiskImageTool.HexTextBox()
        Me.MskFileHex = New DiskImageTool.HexTextBox()
        GroupFileDates = New System.Windows.Forms.GroupBox()
        GroupAttributes = New System.Windows.Forms.GroupBox()
        TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        GroupFileDates.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.PanelCreatedMS.SuspendLayout()
        CType(Me.NumCreatedMS, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelCreatedTime.SuspendLayout()
        GroupAttributes.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        TableLayoutPanel3.SuspendLayout()
        Me.GroupFileName.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.FlowLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupFileDates
        '
        GroupFileDates.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        GroupFileDates.AutoSize = True
        GroupFileDates.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        GroupFileDates.Controls.Add(Me.TableLayoutPanel1)
        GroupFileDates.Location = New System.Drawing.Point(3, 99)
        GroupFileDates.Name = "GroupFileDates"
        GroupFileDates.Padding = New System.Windows.Forms.Padding(3, 3, 3, 0)
        GroupFileDates.Size = New System.Drawing.Size(438, 113)
        GroupFileDates.TabIndex = 1
        GroupFileDates.TabStop = False
        GroupFileDates.Text = "File Dates"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel1.ColumnCount = 5
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.Controls.Add(Me.PanelCreatedMS, 3, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.BtnLastAccessed, 4, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.LblLastWritten, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.BtnCreated, 4, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.LblCreated, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.LblLastAccessed, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.DTLastAccessed, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.DTCreated, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.DTLastWritten, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.BtnLastWritten, 4, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.DTLastWrittenTime, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.PanelCreatedTime, 2, 1)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(9, 19)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(420, 78)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'PanelCreatedMS
        '
        Me.PanelCreatedMS.BackColor = System.Drawing.SystemColors.Window
        Me.PanelCreatedMS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelCreatedMS.Controls.Add(Me.NumCreatedMS)
        Me.PanelCreatedMS.Location = New System.Drawing.Point(305, 29)
        Me.PanelCreatedMS.Name = "PanelCreatedMS"
        Me.PanelCreatedMS.Padding = New System.Windows.Forms.Padding(1, 1, 0, 0)
        Me.PanelCreatedMS.Size = New System.Drawing.Size(44, 20)
        Me.PanelCreatedMS.TabIndex = 7
        '
        'NumCreatedMS
        '
        Me.NumCreatedMS.BackColor = System.Drawing.SystemColors.Window
        Me.NumCreatedMS.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.NumCreatedMS.Increment = New Decimal(New Integer() {10, 0, 0, 0})
        Me.NumCreatedMS.Location = New System.Drawing.Point(1, 1)
        Me.NumCreatedMS.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        Me.NumCreatedMS.Name = "NumCreatedMS"
        Me.NumCreatedMS.Size = New System.Drawing.Size(42, 16)
        Me.NumCreatedMS.TabIndex = 0
        '
        'BtnLastAccessed
        '
        Me.BtnLastAccessed.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BtnLastAccessed.Location = New System.Drawing.Point(355, 55)
        Me.BtnLastAccessed.Name = "BtnLastAccessed"
        Me.BtnLastAccessed.Size = New System.Drawing.Size(62, 20)
        Me.BtnLastAccessed.TabIndex = 11
        Me.BtnLastAccessed.Text = "Edit"
        Me.BtnLastAccessed.UseVisualStyleBackColor = True
        '
        'LblLastWritten
        '
        Me.LblLastWritten.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblLastWritten.AutoSize = True
        Me.LblLastWritten.Location = New System.Drawing.Point(3, 6)
        Me.LblLastWritten.Margin = New System.Windows.Forms.Padding(3)
        Me.LblLastWritten.Name = "LblLastWritten"
        Me.LblLastWritten.Size = New System.Drawing.Size(64, 13)
        Me.LblLastWritten.TabIndex = 0
        Me.LblLastWritten.Text = "Last Written"
        '
        'BtnCreated
        '
        Me.BtnCreated.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BtnCreated.Location = New System.Drawing.Point(355, 29)
        Me.BtnCreated.Name = "BtnCreated"
        Me.BtnCreated.Size = New System.Drawing.Size(62, 20)
        Me.BtnCreated.TabIndex = 8
        Me.BtnCreated.Text = "Edit"
        Me.BtnCreated.UseVisualStyleBackColor = True
        '
        'LblCreated
        '
        Me.LblCreated.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblCreated.AutoSize = True
        Me.LblCreated.Location = New System.Drawing.Point(3, 32)
        Me.LblCreated.Margin = New System.Windows.Forms.Padding(3)
        Me.LblCreated.Name = "LblCreated"
        Me.LblCreated.Size = New System.Drawing.Size(44, 13)
        Me.LblCreated.TabIndex = 4
        Me.LblCreated.Text = "Created"
        '
        'LblLastAccessed
        '
        Me.LblLastAccessed.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblLastAccessed.AutoSize = True
        Me.LblLastAccessed.Location = New System.Drawing.Point(3, 58)
        Me.LblLastAccessed.Margin = New System.Windows.Forms.Padding(3)
        Me.LblLastAccessed.Name = "LblLastAccessed"
        Me.LblLastAccessed.Size = New System.Drawing.Size(77, 13)
        Me.LblLastAccessed.TabIndex = 9
        Me.LblLastAccessed.Text = "Last Accessed"
        '
        'DTLastAccessed
        '
        Me.DTLastAccessed.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.DTLastAccessed.CustomFormat = ""
        Me.DTLastAccessed.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTLastAccessed.Location = New System.Drawing.Point(86, 55)
        Me.DTLastAccessed.MaxDate = New Date(2107, 12, 31, 0, 0, 0, 0)
        Me.DTLastAccessed.MinDate = New Date(1980, 1, 1, 0, 0, 0, 0)
        Me.DTLastAccessed.Name = "DTLastAccessed"
        Me.DTLastAccessed.ShowCheckBox = True
        Me.DTLastAccessed.Size = New System.Drawing.Size(116, 20)
        Me.DTLastAccessed.TabIndex = 10
        '
        'DTCreated
        '
        Me.DTCreated.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.DTCreated.CustomFormat = ""
        Me.DTCreated.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTCreated.Location = New System.Drawing.Point(86, 29)
        Me.DTCreated.MaxDate = New Date(2107, 12, 31, 0, 0, 0, 0)
        Me.DTCreated.MinDate = New Date(1980, 1, 1, 0, 0, 0, 0)
        Me.DTCreated.Name = "DTCreated"
        Me.DTCreated.ShowCheckBox = True
        Me.DTCreated.Size = New System.Drawing.Size(116, 20)
        Me.DTCreated.TabIndex = 5
        '
        'DTLastWritten
        '
        Me.DTLastWritten.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.DTLastWritten.CustomFormat = ""
        Me.DTLastWritten.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTLastWritten.Location = New System.Drawing.Point(86, 3)
        Me.DTLastWritten.MaxDate = New Date(2107, 12, 31, 0, 0, 0, 0)
        Me.DTLastWritten.MinDate = New Date(1980, 1, 1, 0, 0, 0, 0)
        Me.DTLastWritten.Name = "DTLastWritten"
        Me.DTLastWritten.Size = New System.Drawing.Size(116, 20)
        Me.DTLastWritten.TabIndex = 1
        '
        'BtnLastWritten
        '
        Me.BtnLastWritten.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BtnLastWritten.Location = New System.Drawing.Point(355, 3)
        Me.BtnLastWritten.Name = "BtnLastWritten"
        Me.BtnLastWritten.Size = New System.Drawing.Size(62, 20)
        Me.BtnLastWritten.TabIndex = 3
        Me.BtnLastWritten.Text = "Edit"
        Me.BtnLastWritten.UseVisualStyleBackColor = True
        '
        'DTLastWrittenTime
        '
        Me.DTLastWrittenTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.DTLastWrittenTime.Location = New System.Drawing.Point(208, 3)
        Me.DTLastWrittenTime.Name = "DTLastWrittenTime"
        Me.DTLastWrittenTime.ShowUpDown = True
        Me.DTLastWrittenTime.Size = New System.Drawing.Size(91, 20)
        Me.DTLastWrittenTime.TabIndex = 2
        '
        'PanelCreatedTime
        '
        Me.PanelCreatedTime.BackColor = System.Drawing.SystemColors.Window
        Me.PanelCreatedTime.Controls.Add(Me.DTCreatedTime)
        Me.PanelCreatedTime.Location = New System.Drawing.Point(208, 29)
        Me.PanelCreatedTime.Name = "PanelCreatedTime"
        Me.PanelCreatedTime.Size = New System.Drawing.Size(91, 20)
        Me.PanelCreatedTime.TabIndex = 6
        '
        'DTCreatedTime
        '
        Me.DTCreatedTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.DTCreatedTime.Location = New System.Drawing.Point(0, 0)
        Me.DTCreatedTime.Name = "DTCreatedTime"
        Me.DTCreatedTime.ShowUpDown = True
        Me.DTCreatedTime.Size = New System.Drawing.Size(91, 20)
        Me.DTCreatedTime.TabIndex = 0
        '
        'GroupAttributes
        '
        GroupAttributes.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        GroupAttributes.AutoSize = True
        GroupAttributes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        GroupAttributes.Controls.Add(Me.TableLayoutPanel2)
        GroupAttributes.Location = New System.Drawing.Point(3, 218)
        GroupAttributes.Name = "GroupAttributes"
        GroupAttributes.Padding = New System.Windows.Forms.Padding(3, 3, 3, 0)
        GroupAttributes.Size = New System.Drawing.Size(438, 90)
        GroupAttributes.TabIndex = 2
        GroupAttributes.TabStop = False
        GroupAttributes.Text = "Attributes"
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.AutoSize = True
        Me.TableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel2.ColumnCount = 4
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 142.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 142.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel2.Controls.Add(Me.ChkArchive, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.ChkSystem, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.BtnArchive, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.BtnSystem, 1, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.BtnReadOnly, 3, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.BtnHidden, 3, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.ChkReadOnly, 2, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.ChkHidden, 2, 1)
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(9, 22)
        Me.TableLayoutPanel2.Margin = New System.Windows.Forms.Padding(6, 3, 6, 3)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 2
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(420, 52)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'ChkArchive
        '
        Me.ChkArchive.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.ChkArchive.Location = New System.Drawing.Point(3, 4)
        Me.ChkArchive.Name = "ChkArchive"
        Me.ChkArchive.Padding = New System.Windows.Forms.Padding(0, 1, 0, 0)
        Me.ChkArchive.Size = New System.Drawing.Size(62, 17)
        Me.ChkArchive.TabIndex = 0
        Me.ChkArchive.Text = "Archive"
        Me.ChkArchive.UseVisualStyleBackColor = True
        '
        'ChkSystem
        '
        Me.ChkSystem.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.ChkSystem.Location = New System.Drawing.Point(3, 30)
        Me.ChkSystem.Name = "ChkSystem"
        Me.ChkSystem.Padding = New System.Windows.Forms.Padding(0, 1, 0, 0)
        Me.ChkSystem.Size = New System.Drawing.Size(60, 17)
        Me.ChkSystem.TabIndex = 4
        Me.ChkSystem.Text = "System"
        Me.ChkSystem.UseVisualStyleBackColor = True
        '
        'BtnArchive
        '
        Me.BtnArchive.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BtnArchive.Location = New System.Drawing.Point(145, 3)
        Me.BtnArchive.Name = "BtnArchive"
        Me.BtnArchive.Size = New System.Drawing.Size(62, 20)
        Me.BtnArchive.TabIndex = 1
        Me.BtnArchive.Text = "Edit"
        Me.BtnArchive.UseVisualStyleBackColor = True
        '
        'BtnSystem
        '
        Me.BtnSystem.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BtnSystem.Location = New System.Drawing.Point(145, 29)
        Me.BtnSystem.Name = "BtnSystem"
        Me.BtnSystem.Size = New System.Drawing.Size(62, 20)
        Me.BtnSystem.TabIndex = 5
        Me.BtnSystem.Text = "Edit"
        Me.BtnSystem.UseVisualStyleBackColor = True
        '
        'BtnReadOnly
        '
        Me.BtnReadOnly.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BtnReadOnly.Location = New System.Drawing.Point(355, 3)
        Me.BtnReadOnly.Name = "BtnReadOnly"
        Me.BtnReadOnly.Size = New System.Drawing.Size(62, 20)
        Me.BtnReadOnly.TabIndex = 3
        Me.BtnReadOnly.Text = "Edit"
        Me.BtnReadOnly.UseVisualStyleBackColor = True
        '
        'BtnHidden
        '
        Me.BtnHidden.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.BtnHidden.Location = New System.Drawing.Point(355, 29)
        Me.BtnHidden.Name = "BtnHidden"
        Me.BtnHidden.Size = New System.Drawing.Size(62, 20)
        Me.BtnHidden.TabIndex = 7
        Me.BtnHidden.Text = "Edit"
        Me.BtnHidden.UseVisualStyleBackColor = True
        '
        'ChkReadOnly
        '
        Me.ChkReadOnly.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.ChkReadOnly.Location = New System.Drawing.Point(213, 4)
        Me.ChkReadOnly.Name = "ChkReadOnly"
        Me.ChkReadOnly.Padding = New System.Windows.Forms.Padding(6, 1, 0, 0)
        Me.ChkReadOnly.Size = New System.Drawing.Size(87, 17)
        Me.ChkReadOnly.TabIndex = 2
        Me.ChkReadOnly.Text = "Read Only"
        Me.ChkReadOnly.UseVisualStyleBackColor = True
        '
        'ChkHidden
        '
        Me.ChkHidden.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.ChkHidden.Location = New System.Drawing.Point(213, 30)
        Me.ChkHidden.Name = "ChkHidden"
        Me.ChkHidden.Padding = New System.Windows.Forms.Padding(6, 1, 0, 0)
        Me.ChkHidden.Size = New System.Drawing.Size(76, 17)
        Me.ChkHidden.TabIndex = 6
        Me.ChkHidden.Text = "Hidden"
        Me.ChkHidden.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel3
        '
        TableLayoutPanel3.AutoSize = True
        TableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        TableLayoutPanel3.ColumnCount = 3
        TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        TableLayoutPanel3.Controls.Add(Me.MskExtensionHex, 1, 1)
        TableLayoutPanel3.Controls.Add(Me.MskFileHex, 0, 1)
        TableLayoutPanel3.Controls.Add(Me.LblMultipleFiles, 2, 0)
        TableLayoutPanel3.Controls.Add(Me.TxtExtension, 1, 0)
        TableLayoutPanel3.Controls.Add(Me.TxtFile, 0, 0)
        TableLayoutPanel3.Location = New System.Drawing.Point(9, 22)
        TableLayoutPanel3.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        TableLayoutPanel3.Name = "TableLayoutPanel3"
        TableLayoutPanel3.RowCount = 2
        TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        TableLayoutPanel3.Size = New System.Drawing.Size(370, 52)
        TableLayoutPanel3.TabIndex = 1
        '
        'LblMultipleFiles
        '
        Me.LblMultipleFiles.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblMultipleFiles.AutoSize = True
        Me.LblMultipleFiles.Location = New System.Drawing.Point(249, 6)
        Me.LblMultipleFiles.Name = "LblMultipleFiles"
        Me.LblMultipleFiles.Size = New System.Drawing.Size(118, 13)
        Me.LblMultipleFiles.TabIndex = 2
        Me.LblMultipleFiles.Text = "(Multiple Files Selected)"
        '
        'TxtExtension
        '
        Me.TxtExtension.Location = New System.Drawing.Point(179, 3)
        Me.TxtExtension.MaxLength = 3
        Me.TxtExtension.Name = "TxtExtension"
        Me.TxtExtension.Size = New System.Drawing.Size(64, 20)
        Me.TxtExtension.TabIndex = 1
        '
        'TxtFile
        '
        Me.TxtFile.Location = New System.Drawing.Point(3, 3)
        Me.TxtFile.Name = "TxtFile"
        Me.TxtFile.Size = New System.Drawing.Size(170, 20)
        Me.TxtFile.TabIndex = 0
        '
        'GroupFileName
        '
        Me.GroupFileName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupFileName.AutoSize = True
        Me.GroupFileName.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GroupFileName.Controls.Add(TableLayoutPanel3)
        Me.GroupFileName.Location = New System.Drawing.Point(3, 3)
        Me.GroupFileName.Name = "GroupFileName"
        Me.GroupFileName.Size = New System.Drawing.Size(438, 90)
        Me.GroupFileName.TabIndex = 0
        Me.GroupFileName.TabStop = False
        Me.GroupFileName.Text = "File Name"
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.CausesValidation = False
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(99, 3)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(90, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "&Cancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnUpdate
        '
        Me.BtnUpdate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnUpdate.Location = New System.Drawing.Point(3, 3)
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.Size = New System.Drawing.Size(90, 23)
        Me.BtnUpdate.TabIndex = 0
        Me.BtnUpdate.Text = "&Update"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel1.CausesValidation = False
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnUpdate)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnCancel)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(126, 314)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(192, 29)
        Me.FlowLayoutPanel1.TabIndex = 3
        '
        'FlowLayoutPanel2
        '
        Me.FlowLayoutPanel2.AutoSize = True
        Me.FlowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel2.Controls.Add(Me.GroupFileName)
        Me.FlowLayoutPanel2.Controls.Add(GroupFileDates)
        Me.FlowLayoutPanel2.Controls.Add(GroupAttributes)
        Me.FlowLayoutPanel2.Controls.Add(Me.FlowLayoutPanel1)
        Me.FlowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.FlowLayoutPanel2.Location = New System.Drawing.Point(21, 21)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        Me.FlowLayoutPanel2.Size = New System.Drawing.Size(444, 346)
        Me.FlowLayoutPanel2.TabIndex = 4
        '
        'MskExtensionHex
        '
        Me.MskExtensionHex.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MskExtensionHex.Location = New System.Drawing.Point(179, 29)
        Me.MskExtensionHex.MaskLength = 3
        Me.MskExtensionHex.Name = "MskExtensionHex"
        Me.MskExtensionHex.Size = New System.Drawing.Size(64, 20)
        Me.MskExtensionHex.TabIndex = 4
        '
        'MskFileHex
        '
        Me.MskFileHex.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MskFileHex.Location = New System.Drawing.Point(3, 29)
        Me.MskFileHex.MaskLength = 8
        Me.MskFileHex.Name = "MskFileHex"
        Me.MskFileHex.Size = New System.Drawing.Size(170, 20)
        Me.MskFileHex.TabIndex = 3
        '
        'FilePropertiesForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CancelButton = Me.BtnCancel
        Me.ClientSize = New System.Drawing.Size(484, 396)
        Me.Controls.Add(Me.FlowLayoutPanel2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FilePropertiesForm"
        Me.Padding = New System.Windows.Forms.Padding(18)
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "File Properties"
        GroupFileDates.ResumeLayout(False)
        GroupFileDates.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.PanelCreatedMS.ResumeLayout(False)
        CType(Me.NumCreatedMS, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelCreatedTime.ResumeLayout(False)
        GroupAttributes.ResumeLayout(False)
        GroupAttributes.PerformLayout()
        Me.TableLayoutPanel2.ResumeLayout(False)
        TableLayoutPanel3.ResumeLayout(False)
        TableLayoutPanel3.PerformLayout()
        Me.GroupFileName.ResumeLayout(False)
        Me.GroupFileName.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel2.ResumeLayout(False)
        Me.FlowLayoutPanel2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents DTLastWritten As DateTimePicker
    Friend WithEvents DTCreated As DateTimePicker
    Friend WithEvents DTLastAccessed As DateTimePicker
    Friend WithEvents BtnCancel As Button
    Friend WithEvents BtnUpdate As Button
    Friend WithEvents BtnLastAccessed As Button
    Friend WithEvents BtnCreated As Button
    Friend WithEvents BtnLastWritten As Button
    Friend WithEvents ChkSystem As CheckBox
    Friend WithEvents ChkHidden As CheckBox
    Friend WithEvents ChkReadOnly As CheckBox
    Friend WithEvents ChkArchive As CheckBox
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
    Friend WithEvents BtnArchive As Button
    Friend WithEvents BtnSystem As Button
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents TxtFile As MaskedTextBox
    Friend WithEvents TxtExtension As TextBox
    Friend WithEvents LblLastWritten As Label
    Friend WithEvents LblCreated As Label
    Friend WithEvents LblLastAccessed As Label
    Friend WithEvents LblMultipleFiles As Label
    Friend WithEvents BtnReadOnly As Button
    Friend WithEvents BtnHidden As Button
    Friend WithEvents GroupFileName As GroupBox
    Friend WithEvents MskFileHex As HexTextBox
    Friend WithEvents MskExtensionHex As HexTextBox
    Friend WithEvents NumCreatedMS As NumericUpDown
    Friend WithEvents FlowLayoutPanel2 As FlowLayoutPanel
    Friend WithEvents DTLastWrittenTime As DateTimePicker
    Friend WithEvents DTCreatedTime As DateTimePicker
    Friend WithEvents PanelCreatedMS As Panel
    Friend WithEvents PanelCreatedTime As Panel
End Class
