<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ReadDiskOutput
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.GroupBoxOutput = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanelOutput = New System.Windows.Forms.TableLayoutPanel()
        Me.LabelOutputType = New System.Windows.Forms.Label()
        Me.LabelOutputType2 = New System.Windows.Forms.Label()
        Me.LabelLocation = New System.Windows.Forms.Label()
        Me.ComboBoxPrimaryOutput = New System.Windows.Forms.ComboBox()
        Me.ComboBoxPrimaryExtension = New System.Windows.Forms.ComboBox()
        Me.ComboBoxSecondaryOutput = New System.Windows.Forms.ComboBox()
        Me.ComboBoxSecondaryExtension = New System.Windows.Forms.ComboBox()
        Me.ComboBoxSecondaryLocation = New System.Windows.Forms.ComboBox()
        Me.GroupBoxOutput.SuspendLayout()
        Me.TableLayoutPanelOutput.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBoxOutput
        '
        Me.GroupBoxOutput.AutoSize = True
        Me.GroupBoxOutput.Controls.Add(Me.TableLayoutPanelOutput)
        Me.GroupBoxOutput.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBoxOutput.Location = New System.Drawing.Point(0, 0)
        Me.GroupBoxOutput.Margin = New System.Windows.Forms.Padding(0)
        Me.GroupBoxOutput.MinimumSize = New System.Drawing.Size(0, 153)
        Me.GroupBoxOutput.Name = "GroupBoxOutput"
        Me.GroupBoxOutput.Padding = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.GroupBoxOutput.Size = New System.Drawing.Size(194, 153)
        Me.GroupBoxOutput.TabIndex = 0
        Me.GroupBoxOutput.TabStop = False
        Me.GroupBoxOutput.Text = "Output"
        '
        'TableLayoutPanelOutput
        '
        Me.TableLayoutPanelOutput.AutoSize = True
        Me.TableLayoutPanelOutput.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanelOutput.ColumnCount = 2
        Me.TableLayoutPanelOutput.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelOutput.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 54.0!))
        Me.TableLayoutPanelOutput.Controls.Add(Me.LabelOutputType, 0, 0)
        Me.TableLayoutPanelOutput.Controls.Add(Me.LabelOutputType2, 0, 2)
        Me.TableLayoutPanelOutput.Controls.Add(Me.LabelLocation, 0, 4)
        Me.TableLayoutPanelOutput.Controls.Add(Me.ComboBoxPrimaryOutput, 0, 1)
        Me.TableLayoutPanelOutput.Controls.Add(Me.ComboBoxPrimaryExtension, 1, 1)
        Me.TableLayoutPanelOutput.Controls.Add(Me.ComboBoxSecondaryOutput, 0, 3)
        Me.TableLayoutPanelOutput.Controls.Add(Me.ComboBoxSecondaryExtension, 1, 3)
        Me.TableLayoutPanelOutput.Controls.Add(Me.ComboBoxSecondaryLocation, 0, 5)
        Me.TableLayoutPanelOutput.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TableLayoutPanelOutput.Location = New System.Drawing.Point(6, 19)
        Me.TableLayoutPanelOutput.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.TableLayoutPanelOutput.Name = "TableLayoutPanelOutput"
        Me.TableLayoutPanelOutput.RowCount = 6
        Me.TableLayoutPanelOutput.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelOutput.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelOutput.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelOutput.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelOutput.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelOutput.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelOutput.Size = New System.Drawing.Size(179, 121)
        Me.TableLayoutPanelOutput.TabIndex = 0
        '
        'LabelOutputType
        '
        Me.LabelOutputType.AutoSize = True
        Me.TableLayoutPanelOutput.SetColumnSpan(Me.LabelOutputType, 2)
        Me.LabelOutputType.Location = New System.Drawing.Point(3, 0)
        Me.LabelOutputType.Name = "LabelOutputType"
        Me.LabelOutputType.Size = New System.Drawing.Size(76, 13)
        Me.LabelOutputType.TabIndex = 0
        Me.LabelOutputType.Text = "Primary Output"
        Me.LabelOutputType.UseMnemonic = False
        '
        'LabelOutputType2
        '
        Me.LabelOutputType2.AutoSize = True
        Me.TableLayoutPanelOutput.SetColumnSpan(Me.LabelOutputType2, 2)
        Me.LabelOutputType2.Location = New System.Drawing.Point(3, 41)
        Me.LabelOutputType2.Name = "LabelOutputType2"
        Me.LabelOutputType2.Size = New System.Drawing.Size(93, 13)
        Me.LabelOutputType2.TabIndex = 3
        Me.LabelOutputType2.Text = "Secondary Output"
        Me.LabelOutputType2.UseMnemonic = False
        '
        'LabelLocation
        '
        Me.LabelLocation.AutoSize = True
        Me.TableLayoutPanelOutput.SetColumnSpan(Me.LabelLocation, 2)
        Me.LabelLocation.Location = New System.Drawing.Point(3, 81)
        Me.LabelLocation.Name = "LabelLocation"
        Me.LabelLocation.Size = New System.Drawing.Size(134, 13)
        Me.LabelLocation.TabIndex = 6
        Me.LabelLocation.Text = "Secondary Image Location"
        Me.LabelLocation.UseMnemonic = False
        '
        'ComboBoxPrimaryOutput
        '
        Me.ComboBoxPrimaryOutput.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ComboBoxPrimaryOutput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxPrimaryOutput.FormattingEnabled = True
        Me.ComboBoxPrimaryOutput.Location = New System.Drawing.Point(3, 16)
        Me.ComboBoxPrimaryOutput.Margin = New System.Windows.Forms.Padding(3, 3, 3, 4)
        Me.ComboBoxPrimaryOutput.Name = "ComboBoxPrimaryOutput"
        Me.ComboBoxPrimaryOutput.Size = New System.Drawing.Size(119, 21)
        Me.ComboBoxPrimaryOutput.TabIndex = 1
        '
        'ComboBoxPrimaryExtension
        '
        Me.ComboBoxPrimaryExtension.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ComboBoxPrimaryExtension.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxPrimaryExtension.FormattingEnabled = True
        Me.ComboBoxPrimaryExtension.Location = New System.Drawing.Point(128, 16)
        Me.ComboBoxPrimaryExtension.Margin = New System.Windows.Forms.Padding(3, 3, 3, 4)
        Me.ComboBoxPrimaryExtension.Name = "ComboBoxPrimaryExtension"
        Me.ComboBoxPrimaryExtension.Size = New System.Drawing.Size(48, 21)
        Me.ComboBoxPrimaryExtension.TabIndex = 2
        '
        'ComboBoxSecondaryOutput
        '
        Me.ComboBoxSecondaryOutput.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ComboBoxSecondaryOutput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxSecondaryOutput.FormattingEnabled = True
        Me.ComboBoxSecondaryOutput.Location = New System.Drawing.Point(3, 57)
        Me.ComboBoxSecondaryOutput.Name = "ComboBoxSecondaryOutput"
        Me.ComboBoxSecondaryOutput.Size = New System.Drawing.Size(119, 21)
        Me.ComboBoxSecondaryOutput.TabIndex = 4
        '
        'ComboBoxSecondaryExtension
        '
        Me.ComboBoxSecondaryExtension.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ComboBoxSecondaryExtension.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxSecondaryExtension.FormattingEnabled = True
        Me.ComboBoxSecondaryExtension.Location = New System.Drawing.Point(128, 57)
        Me.ComboBoxSecondaryExtension.Name = "ComboBoxSecondaryExtension"
        Me.ComboBoxSecondaryExtension.Size = New System.Drawing.Size(48, 21)
        Me.ComboBoxSecondaryExtension.TabIndex = 5
        '
        'ComboBoxSecondaryLocation
        '
        Me.TableLayoutPanelOutput.SetColumnSpan(Me.ComboBoxSecondaryLocation, 2)
        Me.ComboBoxSecondaryLocation.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ComboBoxSecondaryLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxSecondaryLocation.FormattingEnabled = True
        Me.ComboBoxSecondaryLocation.Location = New System.Drawing.Point(3, 97)
        Me.ComboBoxSecondaryLocation.Name = "ComboBoxSecondaryLocation"
        Me.ComboBoxSecondaryLocation.Size = New System.Drawing.Size(173, 21)
        Me.ComboBoxSecondaryLocation.TabIndex = 7
        '
        'ReadDiskOutput
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.GroupBoxOutput)
        Me.Name = "ReadDiskOutput"
        Me.Size = New System.Drawing.Size(194, 153)
        Me.GroupBoxOutput.ResumeLayout(False)
        Me.GroupBoxOutput.PerformLayout()
        Me.TableLayoutPanelOutput.ResumeLayout(False)
        Me.TableLayoutPanelOutput.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents GroupBoxOutput As GroupBox
    Friend WithEvents TableLayoutPanelOutput As TableLayoutPanel
    Friend WithEvents LabelOutputType As Label
    Friend WithEvents LabelOutputType2 As Label
    Friend WithEvents LabelLocation As Label
    Friend WithEvents ComboBoxPrimaryOutput As ComboBox
    Friend WithEvents ComboBoxPrimaryExtension As ComboBox
    Friend WithEvents ComboBoxSecondaryOutput As ComboBox
    Friend WithEvents ComboBoxSecondaryExtension As ComboBox
    Friend WithEvents ComboBoxSecondaryLocation As ComboBox
End Class
