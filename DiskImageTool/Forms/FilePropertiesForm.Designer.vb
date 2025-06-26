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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FilePropertiesForm))
        Dim GroupAttributes As System.Windows.Forms.GroupBox
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
        Me.TableLayoutPanelFile = New System.Windows.Forms.TableLayoutPanel()
        Me.MskExtensionHex = New DiskImageTool.HexTextBox()
        Me.MskFileHex = New DiskImageTool.HexTextBox()
        Me.TxtExtension = New System.Windows.Forms.TextBox()
        Me.TxtFile = New System.Windows.Forms.MaskedTextBox()
        Me.GroupFileName = New System.Windows.Forms.GroupBox()
        Me.LblMultipleFiles = New System.Windows.Forms.Label()
        Me.TxtLFN = New System.Windows.Forms.TextBox()
        Me.FlowLayoutFileNameType = New System.Windows.Forms.FlowLayoutPanel()
        Me.RadioFileShort = New System.Windows.Forms.RadioButton()
        Me.RadioFileLong = New System.Windows.Forms.RadioButton()
        Me.ChkNTExtensions = New System.Windows.Forms.CheckBox()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        GroupFileDates = New System.Windows.Forms.GroupBox()
        GroupAttributes = New System.Windows.Forms.GroupBox()
        GroupFileDates.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.PanelCreatedMS.SuspendLayout()
        CType(Me.NumCreatedMS, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelCreatedTime.SuspendLayout()
        GroupAttributes.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.TableLayoutPanelFile.SuspendLayout()
        Me.GroupFileName.SuspendLayout()
        Me.FlowLayoutFileNameType.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.FlowLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupFileDates
        '
        resources.ApplyResources(GroupFileDates, "GroupFileDates")
        GroupFileDates.Controls.Add(Me.TableLayoutPanel1)
        GroupFileDates.Name = "GroupFileDates"
        GroupFileDates.TabStop = False
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
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
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'PanelCreatedMS
        '
        Me.PanelCreatedMS.BackColor = System.Drawing.SystemColors.Window
        Me.PanelCreatedMS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelCreatedMS.Controls.Add(Me.NumCreatedMS)
        resources.ApplyResources(Me.PanelCreatedMS, "PanelCreatedMS")
        Me.PanelCreatedMS.Name = "PanelCreatedMS"
        '
        'NumCreatedMS
        '
        Me.NumCreatedMS.BackColor = System.Drawing.SystemColors.Window
        Me.NumCreatedMS.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.NumCreatedMS.Increment = New Decimal(New Integer() {10, 0, 0, 0})
        resources.ApplyResources(Me.NumCreatedMS, "NumCreatedMS")
        Me.NumCreatedMS.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        Me.NumCreatedMS.Name = "NumCreatedMS"
        '
        'BtnLastAccessed
        '
        resources.ApplyResources(Me.BtnLastAccessed, "BtnLastAccessed")
        Me.BtnLastAccessed.Name = "BtnLastAccessed"
        Me.BtnLastAccessed.UseVisualStyleBackColor = True
        '
        'LblLastWritten
        '
        resources.ApplyResources(Me.LblLastWritten, "LblLastWritten")
        Me.LblLastWritten.Name = "LblLastWritten"
        '
        'BtnCreated
        '
        resources.ApplyResources(Me.BtnCreated, "BtnCreated")
        Me.BtnCreated.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCreated.Name = "BtnCreated"
        Me.BtnCreated.UseVisualStyleBackColor = True
        '
        'LblCreated
        '
        resources.ApplyResources(Me.LblCreated, "LblCreated")
        Me.LblCreated.Name = "LblCreated"
        '
        'LblLastAccessed
        '
        resources.ApplyResources(Me.LblLastAccessed, "LblLastAccessed")
        Me.LblLastAccessed.Name = "LblLastAccessed"
        '
        'DTLastAccessed
        '
        resources.ApplyResources(Me.DTLastAccessed, "DTLastAccessed")
        Me.DTLastAccessed.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTLastAccessed.MaxDate = New Date(2107, 12, 31, 0, 0, 0, 0)
        Me.DTLastAccessed.MinDate = New Date(1980, 1, 1, 0, 0, 0, 0)
        Me.DTLastAccessed.Name = "DTLastAccessed"
        Me.DTLastAccessed.ShowCheckBox = True
        '
        'DTCreated
        '
        resources.ApplyResources(Me.DTCreated, "DTCreated")
        Me.DTCreated.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTCreated.MaxDate = New Date(2107, 12, 31, 0, 0, 0, 0)
        Me.DTCreated.MinDate = New Date(1980, 1, 1, 0, 0, 0, 0)
        Me.DTCreated.Name = "DTCreated"
        Me.DTCreated.ShowCheckBox = True
        '
        'DTLastWritten
        '
        resources.ApplyResources(Me.DTLastWritten, "DTLastWritten")
        Me.DTLastWritten.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DTLastWritten.MaxDate = New Date(2107, 12, 31, 0, 0, 0, 0)
        Me.DTLastWritten.MinDate = New Date(1980, 1, 1, 0, 0, 0, 0)
        Me.DTLastWritten.Name = "DTLastWritten"
        '
        'BtnLastWritten
        '
        resources.ApplyResources(Me.BtnLastWritten, "BtnLastWritten")
        Me.BtnLastWritten.Name = "BtnLastWritten"
        Me.BtnLastWritten.UseVisualStyleBackColor = True
        '
        'DTLastWrittenTime
        '
        Me.DTLastWrittenTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        resources.ApplyResources(Me.DTLastWrittenTime, "DTLastWrittenTime")
        Me.DTLastWrittenTime.Name = "DTLastWrittenTime"
        Me.DTLastWrittenTime.ShowUpDown = True
        '
        'PanelCreatedTime
        '
        Me.PanelCreatedTime.BackColor = System.Drawing.SystemColors.Window
        Me.PanelCreatedTime.Controls.Add(Me.DTCreatedTime)
        resources.ApplyResources(Me.PanelCreatedTime, "PanelCreatedTime")
        Me.PanelCreatedTime.Name = "PanelCreatedTime"
        '
        'DTCreatedTime
        '
        Me.DTCreatedTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        resources.ApplyResources(Me.DTCreatedTime, "DTCreatedTime")
        Me.DTCreatedTime.Name = "DTCreatedTime"
        Me.DTCreatedTime.ShowUpDown = True
        '
        'GroupAttributes
        '
        resources.ApplyResources(GroupAttributes, "GroupAttributes")
        GroupAttributes.Controls.Add(Me.TableLayoutPanel2)
        GroupAttributes.Name = "GroupAttributes"
        GroupAttributes.TabStop = False
        '
        'TableLayoutPanel2
        '
        resources.ApplyResources(Me.TableLayoutPanel2, "TableLayoutPanel2")
        Me.TableLayoutPanel2.Controls.Add(Me.ChkArchive, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.ChkSystem, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.BtnArchive, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.BtnSystem, 1, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.BtnReadOnly, 3, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.BtnHidden, 3, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.ChkReadOnly, 2, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.ChkHidden, 2, 1)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        '
        'ChkArchive
        '
        resources.ApplyResources(Me.ChkArchive, "ChkArchive")
        Me.ChkArchive.Name = "ChkArchive"
        Me.ChkArchive.UseVisualStyleBackColor = True
        '
        'ChkSystem
        '
        resources.ApplyResources(Me.ChkSystem, "ChkSystem")
        Me.ChkSystem.Name = "ChkSystem"
        Me.ChkSystem.UseVisualStyleBackColor = True
        '
        'BtnArchive
        '
        resources.ApplyResources(Me.BtnArchive, "BtnArchive")
        Me.BtnArchive.Name = "BtnArchive"
        Me.BtnArchive.UseVisualStyleBackColor = True
        '
        'BtnSystem
        '
        resources.ApplyResources(Me.BtnSystem, "BtnSystem")
        Me.BtnSystem.Name = "BtnSystem"
        Me.BtnSystem.UseVisualStyleBackColor = True
        '
        'BtnReadOnly
        '
        resources.ApplyResources(Me.BtnReadOnly, "BtnReadOnly")
        Me.BtnReadOnly.Name = "BtnReadOnly"
        Me.BtnReadOnly.UseVisualStyleBackColor = True
        '
        'BtnHidden
        '
        resources.ApplyResources(Me.BtnHidden, "BtnHidden")
        Me.BtnHidden.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnHidden.Name = "BtnHidden"
        Me.BtnHidden.UseVisualStyleBackColor = True
        '
        'ChkReadOnly
        '
        resources.ApplyResources(Me.ChkReadOnly, "ChkReadOnly")
        Me.ChkReadOnly.Name = "ChkReadOnly"
        Me.ChkReadOnly.UseVisualStyleBackColor = True
        '
        'ChkHidden
        '
        resources.ApplyResources(Me.ChkHidden, "ChkHidden")
        Me.ChkHidden.Name = "ChkHidden"
        Me.ChkHidden.UseVisualStyleBackColor = True
        '
        'TableLayoutPanelFile
        '
        resources.ApplyResources(Me.TableLayoutPanelFile, "TableLayoutPanelFile")
        Me.TableLayoutPanelFile.Controls.Add(Me.MskExtensionHex, 1, 1)
        Me.TableLayoutPanelFile.Controls.Add(Me.MskFileHex, 0, 1)
        Me.TableLayoutPanelFile.Controls.Add(Me.TxtExtension, 1, 0)
        Me.TableLayoutPanelFile.Controls.Add(Me.TxtFile, 0, 0)
        Me.TableLayoutPanelFile.Name = "TableLayoutPanelFile"
        '
        'MskExtensionHex
        '
        resources.ApplyResources(Me.MskExtensionHex, "MskExtensionHex")
        Me.MskExtensionHex.MaskLength = 3
        Me.MskExtensionHex.Name = "MskExtensionHex"
        '
        'MskFileHex
        '
        resources.ApplyResources(Me.MskFileHex, "MskFileHex")
        Me.MskFileHex.MaskLength = 8
        Me.MskFileHex.Name = "MskFileHex"
        '
        'TxtExtension
        '
        resources.ApplyResources(Me.TxtExtension, "TxtExtension")
        Me.TxtExtension.Name = "TxtExtension"
        '
        'TxtFile
        '
        resources.ApplyResources(Me.TxtFile, "TxtFile")
        Me.TxtFile.Name = "TxtFile"
        '
        'GroupFileName
        '
        resources.ApplyResources(Me.GroupFileName, "GroupFileName")
        Me.GroupFileName.Controls.Add(Me.LblMultipleFiles)
        Me.GroupFileName.Controls.Add(Me.TxtLFN)
        Me.GroupFileName.Controls.Add(Me.FlowLayoutFileNameType)
        Me.GroupFileName.Controls.Add(Me.TableLayoutPanelFile)
        Me.GroupFileName.Name = "GroupFileName"
        Me.GroupFileName.TabStop = False
        '
        'LblMultipleFiles
        '
        resources.ApplyResources(Me.LblMultipleFiles, "LblMultipleFiles")
        Me.LblMultipleFiles.Name = "LblMultipleFiles"
        '
        'TxtLFN
        '
        resources.ApplyResources(Me.TxtLFN, "TxtLFN")
        Me.TxtLFN.Name = "TxtLFN"
        '
        'FlowLayoutFileNameType
        '
        resources.ApplyResources(Me.FlowLayoutFileNameType, "FlowLayoutFileNameType")
        Me.FlowLayoutFileNameType.Controls.Add(Me.RadioFileShort)
        Me.FlowLayoutFileNameType.Controls.Add(Me.RadioFileLong)
        Me.FlowLayoutFileNameType.Controls.Add(Me.ChkNTExtensions)
        Me.FlowLayoutFileNameType.Name = "FlowLayoutFileNameType"
        '
        'RadioFileShort
        '
        resources.ApplyResources(Me.RadioFileShort, "RadioFileShort")
        Me.RadioFileShort.Name = "RadioFileShort"
        Me.RadioFileShort.TabStop = True
        Me.RadioFileShort.UseVisualStyleBackColor = True
        '
        'RadioFileLong
        '
        resources.ApplyResources(Me.RadioFileLong, "RadioFileLong")
        Me.RadioFileLong.Name = "RadioFileLong"
        Me.RadioFileLong.TabStop = True
        Me.RadioFileLong.UseVisualStyleBackColor = True
        '
        'ChkNTExtensions
        '
        resources.ApplyResources(Me.ChkNTExtensions, "ChkNTExtensions")
        Me.ChkNTExtensions.Name = "ChkNTExtensions"
        Me.ChkNTExtensions.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.CausesValidation = False
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnUpdate
        '
        resources.ApplyResources(Me.BtnUpdate, "BtnUpdate")
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel1
        '
        resources.ApplyResources(Me.FlowLayoutPanel1, "FlowLayoutPanel1")
        Me.FlowLayoutPanel1.CausesValidation = False
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnUpdate)
        Me.FlowLayoutPanel1.Controls.Add(Me.BtnCancel)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        '
        'FlowLayoutPanel2
        '
        resources.ApplyResources(Me.FlowLayoutPanel2, "FlowLayoutPanel2")
        Me.FlowLayoutPanel2.Controls.Add(Me.GroupFileName)
        Me.FlowLayoutPanel2.Controls.Add(GroupFileDates)
        Me.FlowLayoutPanel2.Controls.Add(GroupAttributes)
        Me.FlowLayoutPanel2.Controls.Add(Me.FlowLayoutPanel1)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        '
        'FilePropertiesForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.FlowLayoutPanel2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FilePropertiesForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
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
        Me.TableLayoutPanelFile.ResumeLayout(False)
        Me.TableLayoutPanelFile.PerformLayout()
        Me.GroupFileName.ResumeLayout(False)
        Me.GroupFileName.PerformLayout()
        Me.FlowLayoutFileNameType.ResumeLayout(False)
        Me.FlowLayoutFileNameType.PerformLayout()
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
    Friend WithEvents FlowLayoutFileNameType As FlowLayoutPanel
    Friend WithEvents RadioFileShort As RadioButton
    Friend WithEvents RadioFileLong As RadioButton
    Friend WithEvents TableLayoutPanelFile As TableLayoutPanel
    Friend WithEvents TxtLFN As TextBox
    Friend WithEvents LblMultipleFiles As Label
    Friend WithEvents ChkNTExtensions As CheckBox
End Class
