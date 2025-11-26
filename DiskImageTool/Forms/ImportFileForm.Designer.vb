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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ImportFileForm))
        Dim FileSizeOnDisk As System.Windows.Forms.ColumnHeader
        Dim Label2 As System.Windows.Forms.Label
        Dim Label1 As System.Windows.Forms.Label
        Dim PanelTop As System.Windows.Forms.FlowLayoutPanel
        Dim FileDisabled As System.Windows.Forms.ColumnHeader
        Me.LabelFilesSelected = New System.Windows.Forms.Label()
        Me.ChkLFN = New System.Windows.Forms.CheckBox()
        Me.ChkNTExtensions = New System.Windows.Forms.CheckBox()
        Me.ChkCreated = New System.Windows.Forms.CheckBox()
        Me.ChkLastAccessed = New System.Windows.Forms.CheckBox()
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
        FileName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        FileSizeOnDisk = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Label2 = New System.Windows.Forms.Label()
        Label1 = New System.Windows.Forms.Label()
        PanelTop = New System.Windows.Forms.FlowLayoutPanel()
        FileDisabled = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        PanelTop.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.FlowLayoutTotals.SuspendLayout()
        Me.SuspendLayout()
        '
        'FileName
        '
        resources.ApplyResources(FileName, "FileName")
        '
        'FileSizeOnDisk
        '
        resources.ApplyResources(FileSizeOnDisk, "FileSizeOnDisk")
        '
        'LabelFilesSelected
        '
        resources.ApplyResources(Me.LabelFilesSelected, "LabelFilesSelected")
        Me.LabelFilesSelected.Name = "LabelFilesSelected"
        Me.LabelFilesSelected.UseMnemonic = False
        '
        'Label2
        '
        resources.ApplyResources(Label2, "Label2")
        Label2.Name = "Label2"
        Label2.UseMnemonic = False
        '
        'Label1
        '
        resources.ApplyResources(Label1, "Label1")
        Label1.Name = "Label1"
        Label1.UseMnemonic = False
        '
        'PanelTop
        '
        resources.ApplyResources(PanelTop, "PanelTop")
        PanelTop.BackColor = System.Drawing.SystemColors.Control
        PanelTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        PanelTop.Controls.Add(Me.ChkLFN)
        PanelTop.Controls.Add(Me.ChkNTExtensions)
        PanelTop.Controls.Add(Me.ChkCreated)
        PanelTop.Controls.Add(Me.ChkLastAccessed)
        PanelTop.Name = "PanelTop"
        '
        'ChkLFN
        '
        resources.ApplyResources(Me.ChkLFN, "ChkLFN")
        Me.ChkLFN.Name = "ChkLFN"
        Me.ChkLFN.UseVisualStyleBackColor = True
        '
        'ChkNTExtensions
        '
        resources.ApplyResources(Me.ChkNTExtensions, "ChkNTExtensions")
        Me.ChkNTExtensions.Name = "ChkNTExtensions"
        Me.ChkNTExtensions.UseVisualStyleBackColor = True
        '
        'ChkCreated
        '
        resources.ApplyResources(Me.ChkCreated, "ChkCreated")
        Me.ChkCreated.Name = "ChkCreated"
        Me.ChkCreated.UseVisualStyleBackColor = True
        '
        'ChkLastAccessed
        '
        resources.ApplyResources(Me.ChkLastAccessed, "ChkLastAccessed")
        Me.ChkLastAccessed.Name = "ChkLastAccessed"
        Me.ChkLastAccessed.UseVisualStyleBackColor = True
        '
        'FileDisabled
        '
        resources.ApplyResources(FileDisabled, "FileDisabled")
        '
        'FileLastAccessDate
        '
        resources.ApplyResources(Me.FileLastAccessDate, "FileLastAccessDate")
        '
        'FileLastWriteDate
        '
        resources.ApplyResources(Me.FileLastWriteDate, "FileLastWriteDate")
        '
        'LabelOptions
        '
        resources.ApplyResources(Me.LabelOptions, "LabelOptions")
        Me.LabelOptions.Name = "LabelOptions"
        Me.LabelOptions.UseMnemonic = False
        '
        'FileSize
        '
        resources.ApplyResources(Me.FileSize, "FileSize")
        '
        'FileCreationDate
        '
        resources.ApplyResources(Me.FileCreationDate, "FileCreationDate")
        '
        'ListViewFiles
        '
        Me.ListViewFiles.AllowDrop = True
        resources.ApplyResources(Me.ListViewFiles, "ListViewFiles")
        Me.ListViewFiles.CheckBoxes = True
        Me.ListViewFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {FileName, Me.FileSize, FileSizeOnDisk, Me.FileLastWriteDate, Me.FileCreationDate, Me.FileLastAccessDate, FileDisabled})
        Me.ListViewFiles.FullRowSelect = True
        Me.ListViewFiles.HideSelection = False
        Me.ListViewFiles.Name = "ListViewFiles"
        Me.ListViewFiles.UseCompatibleStateImageBehavior = False
        Me.ListViewFiles.View = System.Windows.Forms.View.Details
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.Control
        Me.Panel1.Controls.Add(Me.FlowLayoutTotals)
        Me.Panel1.Controls.Add(Me.BtnOK)
        Me.Panel1.Controls.Add(Me.BtnCancel)
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Name = "Panel1"
        '
        'FlowLayoutTotals
        '
        resources.ApplyResources(Me.FlowLayoutTotals, "FlowLayoutTotals")
        Me.FlowLayoutTotals.Controls.Add(Me.LabelFilesSelected)
        Me.FlowLayoutTotals.Controls.Add(Me.LblSelected)
        Me.FlowLayoutTotals.Controls.Add(Label2)
        Me.FlowLayoutTotals.Controls.Add(Me.LblBytesFree)
        Me.FlowLayoutTotals.Controls.Add(Label1)
        Me.FlowLayoutTotals.Controls.Add(Me.LblBytesRequired)
        Me.FlowLayoutTotals.Name = "FlowLayoutTotals"
        '
        'LblSelected
        '
        resources.ApplyResources(Me.LblSelected, "LblSelected")
        Me.LblSelected.Name = "LblSelected"
        Me.LblSelected.UseMnemonic = False
        '
        'LblBytesFree
        '
        resources.ApplyResources(Me.LblBytesFree, "LblBytesFree")
        Me.LblBytesFree.Name = "LblBytesFree"
        Me.LblBytesFree.UseMnemonic = False
        '
        'LblBytesRequired
        '
        resources.ApplyResources(Me.LblBytesRequired, "LblBytesRequired")
        Me.LblBytesRequired.Name = "LblBytesRequired"
        Me.LblBytesRequired.UseMnemonic = False
        '
        'BtnOK
        '
        resources.ApplyResources(Me.BtnOK, "BtnOK")
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'ImportFileForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.LabelOptions)
        Me.Controls.Add(PanelTop)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.ListViewFiles)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ImportFileForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
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
    Friend WithEvents FileCreationDate As ColumnHeader
    Friend WithEvents FileSize As ColumnHeader
    Friend WithEvents LabelOptions As Label
    Friend WithEvents FileLastWriteDate As ColumnHeader
    Friend WithEvents FileLastAccessDate As ColumnHeader
    Friend WithEvents LabelFilesSelected As Label
End Class
