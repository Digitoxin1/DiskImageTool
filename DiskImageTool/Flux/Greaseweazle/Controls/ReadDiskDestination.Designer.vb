<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ReadDiskDestination
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.GroupBoxDestination = New System.Windows.Forms.GroupBox()
        Me.ButtonToggleSequence = New System.Windows.Forms.Button()
        Me.TableLayoutPanelDestinationFlux = New System.Windows.Forms.TableLayoutPanel()
        Me.ButtonImageFolderBrowse = New System.Windows.Forms.Button()
        Me.LabelRootFolder = New System.Windows.Forms.Label()
        Me.TextBoxRootFolder = New System.Windows.Forms.TextBox()
        Me.LabelFolderName = New System.Windows.Forms.Label()
        Me.TextBoxFolderName = New System.Windows.Forms.TextBox()
        Me.LabelPrefix = New System.Windows.Forms.Label()
        Me.TextBoxPrefix = New DiskImageTool.PlaceholderTextBox()
        Me.ButtonBrowse = New System.Windows.Forms.Button()
        Me.LabelSecondaryImageFolder = New System.Windows.Forms.Label()
        Me.TextBoxImageFolder = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanelDestinationImage = New System.Windows.Forms.TableLayoutPanel()
        Me.LabelFileName = New System.Windows.Forms.Label()
        Me.TextBoxFileName = New System.Windows.Forms.TextBox()
        Me.GroupBoxDestination.SuspendLayout()
        Me.TableLayoutPanelDestinationFlux.SuspendLayout()
        Me.TableLayoutPanelDestinationImage.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBoxDestination
        '
        Me.GroupBoxDestination.Controls.Add(Me.ButtonToggleSequence)
        Me.GroupBoxDestination.Controls.Add(Me.TableLayoutPanelDestinationFlux)
        Me.GroupBoxDestination.Controls.Add(Me.TableLayoutPanelDestinationImage)
        Me.GroupBoxDestination.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBoxDestination.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBoxDestination.Location = New System.Drawing.Point(0, 0)
        Me.GroupBoxDestination.Margin = New System.Windows.Forms.Padding(0)
        Me.GroupBoxDestination.MinimumSize = New System.Drawing.Size(0, 153)
        Me.GroupBoxDestination.Name = "GroupBoxDestination"
        Me.GroupBoxDestination.Padding = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.GroupBoxDestination.Size = New System.Drawing.Size(426, 153)
        Me.GroupBoxDestination.TabIndex = 0
        Me.GroupBoxDestination.TabStop = False
        Me.GroupBoxDestination.Text = "Destination"
        '
        'ButtonToggleSequence
        '
        Me.ButtonToggleSequence.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonToggleSequence.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonToggleSequence.Location = New System.Drawing.Point(372, 9)
        Me.ButtonToggleSequence.Margin = New System.Windows.Forms.Padding(3, 2, 3, 3)
        Me.ButtonToggleSequence.Name = "ButtonToggleSequence"
        Me.ButtonToggleSequence.Size = New System.Drawing.Size(42, 23)
        Me.ButtonToggleSequence.TabIndex = 0
        Me.ButtonToggleSequence.Text = "< >"
        Me.ButtonToggleSequence.UseVisualStyleBackColor = True
        '
        'TableLayoutPanelDestinationFlux
        '
        Me.TableLayoutPanelDestinationFlux.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanelDestinationFlux.ColumnCount = 3
        Me.TableLayoutPanelDestinationFlux.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100.0!))
        Me.TableLayoutPanelDestinationFlux.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanelDestinationFlux.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 34.0!))
        Me.TableLayoutPanelDestinationFlux.Controls.Add(Me.ButtonImageFolderBrowse, 3, 5)
        Me.TableLayoutPanelDestinationFlux.Controls.Add(Me.LabelRootFolder, 0, 0)
        Me.TableLayoutPanelDestinationFlux.Controls.Add(Me.TextBoxRootFolder, 0, 1)
        Me.TableLayoutPanelDestinationFlux.Controls.Add(Me.LabelFolderName, 0, 2)
        Me.TableLayoutPanelDestinationFlux.Controls.Add(Me.TextBoxFolderName, 0, 3)
        Me.TableLayoutPanelDestinationFlux.Controls.Add(Me.LabelPrefix, 0, 4)
        Me.TableLayoutPanelDestinationFlux.Controls.Add(Me.TextBoxPrefix, 0, 5)
        Me.TableLayoutPanelDestinationFlux.Controls.Add(Me.ButtonBrowse, 2, 1)
        Me.TableLayoutPanelDestinationFlux.Controls.Add(Me.LabelSecondaryImageFolder, 1, 4)
        Me.TableLayoutPanelDestinationFlux.Controls.Add(Me.TextBoxImageFolder, 1, 5)
        Me.TableLayoutPanelDestinationFlux.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TableLayoutPanelDestinationFlux.Location = New System.Drawing.Point(9, 19)
        Me.TableLayoutPanelDestinationFlux.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.TableLayoutPanelDestinationFlux.Name = "TableLayoutPanelDestinationFlux"
        Me.TableLayoutPanelDestinationFlux.RowCount = 6
        Me.TableLayoutPanelDestinationFlux.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelDestinationFlux.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelDestinationFlux.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelDestinationFlux.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelDestinationFlux.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelDestinationFlux.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelDestinationFlux.Size = New System.Drawing.Size(408, 123)
        Me.TableLayoutPanelDestinationFlux.TabIndex = 0
        '
        'ButtonImageFolderBrowse
        '
        Me.ButtonImageFolderBrowse.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonImageFolderBrowse.Image = Global.DiskImageTool.My.Resources.Resources.FolderOpened
        Me.ButtonImageFolderBrowse.Location = New System.Drawing.Point(377, 97)
        Me.ButtonImageFolderBrowse.Name = "ButtonImageFolderBrowse"
        Me.ButtonImageFolderBrowse.Size = New System.Drawing.Size(28, 23)
        Me.ButtonImageFolderBrowse.TabIndex = 11
        Me.ButtonImageFolderBrowse.UseVisualStyleBackColor = True
        '
        'LabelRootFolder
        '
        Me.LabelRootFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.LabelRootFolder.AutoSize = True
        Me.TableLayoutPanelDestinationFlux.SetColumnSpan(Me.LabelRootFolder, 2)
        Me.LabelRootFolder.Location = New System.Drawing.Point(3, 0)
        Me.LabelRootFolder.Name = "LabelRootFolder"
        Me.LabelRootFolder.Size = New System.Drawing.Size(62, 13)
        Me.LabelRootFolder.TabIndex = 2
        Me.LabelRootFolder.Text = "Root Folder"
        '
        'TextBoxRootFolder
        '
        Me.TableLayoutPanelDestinationFlux.SetColumnSpan(Me.TextBoxRootFolder, 2)
        Me.TextBoxRootFolder.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBoxRootFolder.Location = New System.Drawing.Point(3, 17)
        Me.TextBoxRootFolder.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.TextBoxRootFolder.MaxLength = 255
        Me.TextBoxRootFolder.Name = "TextBoxRootFolder"
        Me.TextBoxRootFolder.Size = New System.Drawing.Size(368, 20)
        Me.TextBoxRootFolder.TabIndex = 3
        '
        'LabelFolderName
        '
        Me.LabelFolderName.AutoSize = True
        Me.TableLayoutPanelDestinationFlux.SetColumnSpan(Me.LabelFolderName, 2)
        Me.LabelFolderName.Location = New System.Drawing.Point(3, 42)
        Me.LabelFolderName.Name = "LabelFolderName"
        Me.LabelFolderName.Size = New System.Drawing.Size(67, 13)
        Me.LabelFolderName.TabIndex = 5
        Me.LabelFolderName.Text = "Folder Name"
        '
        'TextBoxFolderName
        '
        Me.TableLayoutPanelDestinationFlux.SetColumnSpan(Me.TextBoxFolderName, 3)
        Me.TextBoxFolderName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBoxFolderName.Location = New System.Drawing.Point(3, 58)
        Me.TextBoxFolderName.Name = "TextBoxFolderName"
        Me.TextBoxFolderName.Size = New System.Drawing.Size(402, 20)
        Me.TextBoxFolderName.TabIndex = 6
        '
        'LabelPrefix
        '
        Me.LabelPrefix.AutoSize = True
        Me.LabelPrefix.Location = New System.Drawing.Point(3, 81)
        Me.LabelPrefix.Name = "LabelPrefix"
        Me.LabelPrefix.Size = New System.Drawing.Size(33, 13)
        Me.LabelPrefix.TabIndex = 7
        Me.LabelPrefix.Text = "Prefix"
        '
        'TextBoxPrefix
        '
        Me.TextBoxPrefix.Location = New System.Drawing.Point(3, 98)
        Me.TextBoxPrefix.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.TextBoxPrefix.Name = "TextBoxPrefix"
        Me.TextBoxPrefix.PlaceholderText = ""
        Me.TextBoxPrefix.Size = New System.Drawing.Size(94, 20)
        Me.TextBoxPrefix.TabIndex = 9
        '
        'ButtonBrowse
        '
        Me.ButtonBrowse.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonBrowse.Image = Global.DiskImageTool.My.Resources.Resources.FolderOpened
        Me.ButtonBrowse.Location = New System.Drawing.Point(377, 16)
        Me.ButtonBrowse.Name = "ButtonBrowse"
        Me.ButtonBrowse.Size = New System.Drawing.Size(28, 23)
        Me.ButtonBrowse.TabIndex = 4
        Me.ButtonBrowse.UseVisualStyleBackColor = True
        '
        'LabelSecondaryImageFolder
        '
        Me.LabelSecondaryImageFolder.AutoSize = True
        Me.LabelSecondaryImageFolder.Location = New System.Drawing.Point(103, 81)
        Me.LabelSecondaryImageFolder.Name = "LabelSecondaryImageFolder"
        Me.LabelSecondaryImageFolder.Size = New System.Drawing.Size(122, 13)
        Me.LabelSecondaryImageFolder.TabIndex = 8
        Me.LabelSecondaryImageFolder.Text = "Secondary Image Folder"
        '
        'TextBoxImageFolder
        '
        Me.TextBoxImageFolder.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBoxImageFolder.Location = New System.Drawing.Point(103, 98)
        Me.TextBoxImageFolder.Margin = New System.Windows.Forms.Padding(3, 4, 3, 3)
        Me.TextBoxImageFolder.Name = "TextBoxImageFolder"
        Me.TextBoxImageFolder.Size = New System.Drawing.Size(268, 20)
        Me.TextBoxImageFolder.TabIndex = 10
        '
        'TableLayoutPanelDestinationImage
        '
        Me.TableLayoutPanelDestinationImage.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanelDestinationImage.ColumnCount = 3
        Me.TableLayoutPanelDestinationImage.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100.0!))
        Me.TableLayoutPanelDestinationImage.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanelDestinationImage.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 34.0!))
        Me.TableLayoutPanelDestinationImage.Controls.Add(Me.LabelFileName, 0, 0)
        Me.TableLayoutPanelDestinationImage.Controls.Add(Me.TextBoxFileName, 0, 1)
        Me.TableLayoutPanelDestinationImage.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TableLayoutPanelDestinationImage.Location = New System.Drawing.Point(9, 19)
        Me.TableLayoutPanelDestinationImage.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.TableLayoutPanelDestinationImage.Name = "TableLayoutPanelDestinationImage"
        Me.TableLayoutPanelDestinationImage.RowCount = 2
        Me.TableLayoutPanelDestinationImage.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelDestinationImage.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelDestinationImage.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanelDestinationImage.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanelDestinationImage.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanelDestinationImage.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanelDestinationImage.Size = New System.Drawing.Size(408, 123)
        Me.TableLayoutPanelDestinationImage.TabIndex = 1
        '
        'LabelFileName
        '
        Me.LabelFileName.AutoSize = True
        Me.TableLayoutPanelDestinationImage.SetColumnSpan(Me.LabelFileName, 2)
        Me.LabelFileName.Location = New System.Drawing.Point(3, 0)
        Me.LabelFileName.Name = "LabelFileName"
        Me.LabelFileName.Size = New System.Drawing.Size(54, 13)
        Me.LabelFileName.TabIndex = 5
        Me.LabelFileName.Text = "File Name"
        '
        'TextBoxFileName
        '
        Me.TableLayoutPanelDestinationImage.SetColumnSpan(Me.TextBoxFileName, 3)
        Me.TextBoxFileName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBoxFileName.Location = New System.Drawing.Point(3, 16)
        Me.TextBoxFileName.Name = "TextBoxFileName"
        Me.TextBoxFileName.Size = New System.Drawing.Size(402, 20)
        Me.TextBoxFileName.TabIndex = 6
        '
        'ReadDiskDestination
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBoxDestination)
        Me.Name = "ReadDiskDestination"
        Me.Size = New System.Drawing.Size(426, 153)
        Me.GroupBoxDestination.ResumeLayout(False)
        Me.TableLayoutPanelDestinationFlux.ResumeLayout(False)
        Me.TableLayoutPanelDestinationFlux.PerformLayout()
        Me.TableLayoutPanelDestinationImage.ResumeLayout(False)
        Me.TableLayoutPanelDestinationImage.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBoxDestination As GroupBox
    Friend WithEvents TableLayoutPanelDestinationFlux As TableLayoutPanel
    Friend WithEvents LabelRootFolder As Label
    Friend WithEvents TextBoxRootFolder As TextBox
    Friend WithEvents LabelFolderName As Label
    Friend WithEvents TextBoxFolderName As TextBox
    Friend WithEvents LabelPrefix As Label
    Friend WithEvents TextBoxPrefix As PlaceholderTextBox
    Friend WithEvents ButtonBrowse As Button
    Friend WithEvents ButtonToggleSequence As Button
    Friend WithEvents LabelSecondaryImageFolder As Label
    Friend WithEvents ButtonImageFolderBrowse As Button
    Friend WithEvents TextBoxImageFolder As TextBox
    Friend WithEvents TableLayoutPanelDestinationImage As TableLayoutPanel
    Friend WithEvents LabelFileName As Label
    Friend WithEvents TextBoxFileName As TextBox
End Class
