<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ReadDiskDriveParams
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
        Me.GroupBoxParams = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanelParams = New System.Windows.Forms.TableLayoutPanel()
        Me.LabelDrive = New System.Windows.Forms.Label()
        Me.ComboDrives = New System.Windows.Forms.ComboBox()
        Me.LabelImageFormat = New System.Windows.Forms.Label()
        Me.ComboImageFormat = New System.Windows.Forms.ComboBox()
        Me.LabelRevs = New System.Windows.Forms.Label()
        Me.LabelRetries = New System.Windows.Forms.Label()
        Me.LabelSeekRetries = New System.Windows.Forms.Label()
        Me.NumericRevs = New System.Windows.Forms.NumericUpDown()
        Me.NumericRetries = New System.Windows.Forms.NumericUpDown()
        Me.NumericSeekRetries = New System.Windows.Forms.NumericUpDown()
        Me.ButtonDetect = New System.Windows.Forms.Button()
        Me.GroupBoxParams.SuspendLayout()
        Me.TableLayoutPanelParams.SuspendLayout()
        CType(Me.NumericRevs, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericRetries, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericSeekRetries, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GroupBoxParams
        '
        Me.GroupBoxParams.AutoSize = True
        Me.GroupBoxParams.Controls.Add(Me.TableLayoutPanelParams)
        Me.GroupBoxParams.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBoxParams.Location = New System.Drawing.Point(0, 0)
        Me.GroupBoxParams.Margin = New System.Windows.Forms.Padding(0)
        Me.GroupBoxParams.MinimumSize = New System.Drawing.Size(0, 153)
        Me.GroupBoxParams.Name = "GroupBoxParams"
        Me.GroupBoxParams.Padding = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.GroupBoxParams.Size = New System.Drawing.Size(248, 153)
        Me.GroupBoxParams.TabIndex = 0
        Me.GroupBoxParams.TabStop = False
        Me.GroupBoxParams.Text = "Drive && Read Parameters"
        '
        'TableLayoutPanelParams
        '
        Me.TableLayoutPanelParams.AutoSize = True
        Me.TableLayoutPanelParams.ColumnCount = 3
        Me.TableLayoutPanelParams.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelParams.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelParams.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelParams.Controls.Add(Me.LabelDrive, 0, 0)
        Me.TableLayoutPanelParams.Controls.Add(Me.ComboDrives, 0, 1)
        Me.TableLayoutPanelParams.Controls.Add(Me.LabelImageFormat, 0, 2)
        Me.TableLayoutPanelParams.Controls.Add(Me.ComboImageFormat, 0, 3)
        Me.TableLayoutPanelParams.Controls.Add(Me.LabelRevs, 0, 4)
        Me.TableLayoutPanelParams.Controls.Add(Me.LabelRetries, 1, 4)
        Me.TableLayoutPanelParams.Controls.Add(Me.LabelSeekRetries, 2, 4)
        Me.TableLayoutPanelParams.Controls.Add(Me.NumericRevs, 0, 5)
        Me.TableLayoutPanelParams.Controls.Add(Me.NumericRetries, 1, 5)
        Me.TableLayoutPanelParams.Controls.Add(Me.NumericSeekRetries, 2, 5)
        Me.TableLayoutPanelParams.Controls.Add(Me.ButtonDetect, 2, 1)
        Me.TableLayoutPanelParams.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TableLayoutPanelParams.Location = New System.Drawing.Point(6, 19)
        Me.TableLayoutPanelParams.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.TableLayoutPanelParams.MinimumSize = New System.Drawing.Size(231, 0)
        Me.TableLayoutPanelParams.Name = "TableLayoutPanelParams"
        Me.TableLayoutPanelParams.RowCount = 1
        Me.TableLayoutPanelParams.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelParams.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelParams.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelParams.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelParams.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelParams.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelParams.Size = New System.Drawing.Size(233, 121)
        Me.TableLayoutPanelParams.TabIndex = 0
        '
        'LabelDrive
        '
        Me.LabelDrive.AutoSize = True
        Me.TableLayoutPanelParams.SetColumnSpan(Me.LabelDrive, 3)
        Me.LabelDrive.Location = New System.Drawing.Point(3, 0)
        Me.LabelDrive.Name = "LabelDrive"
        Me.LabelDrive.Size = New System.Drawing.Size(32, 13)
        Me.LabelDrive.TabIndex = 0
        Me.LabelDrive.Text = "Drive"
        Me.LabelDrive.UseMnemonic = False
        '
        'ComboDrives
        '
        Me.TableLayoutPanelParams.SetColumnSpan(Me.ComboDrives, 2)
        Me.ComboDrives.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ComboDrives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboDrives.FormattingEnabled = True
        Me.ComboDrives.Location = New System.Drawing.Point(3, 16)
        Me.ComboDrives.MinimumSize = New System.Drawing.Size(136, 0)
        Me.ComboDrives.Name = "ComboDrives"
        Me.ComboDrives.Size = New System.Drawing.Size(148, 21)
        Me.ComboDrives.TabIndex = 1
        '
        'LabelImageFormat
        '
        Me.LabelImageFormat.AutoSize = True
        Me.TableLayoutPanelParams.SetColumnSpan(Me.LabelImageFormat, 3)
        Me.LabelImageFormat.Location = New System.Drawing.Point(3, 41)
        Me.LabelImageFormat.Name = "LabelImageFormat"
        Me.LabelImageFormat.Size = New System.Drawing.Size(63, 13)
        Me.LabelImageFormat.TabIndex = 3
        Me.LabelImageFormat.Text = "Disk Format"
        Me.LabelImageFormat.UseMnemonic = False
        '
        'ComboImageFormat
        '
        Me.TableLayoutPanelParams.SetColumnSpan(Me.ComboImageFormat, 3)
        Me.ComboImageFormat.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ComboImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboImageFormat.FormattingEnabled = True
        Me.ComboImageFormat.Location = New System.Drawing.Point(3, 57)
        Me.ComboImageFormat.Name = "ComboImageFormat"
        Me.ComboImageFormat.Size = New System.Drawing.Size(227, 21)
        Me.ComboImageFormat.TabIndex = 4
        '
        'LabelRevs
        '
        Me.LabelRevs.AutoSize = True
        Me.LabelRevs.Location = New System.Drawing.Point(3, 81)
        Me.LabelRevs.Name = "LabelRevs"
        Me.LabelRevs.Size = New System.Drawing.Size(32, 13)
        Me.LabelRevs.TabIndex = 5
        Me.LabelRevs.Text = "Revs"
        Me.LabelRevs.UseMnemonic = False
        '
        'LabelRetries
        '
        Me.LabelRetries.AutoSize = True
        Me.LabelRetries.Location = New System.Drawing.Point(80, 81)
        Me.LabelRetries.Name = "LabelRetries"
        Me.LabelRetries.Size = New System.Drawing.Size(40, 13)
        Me.LabelRetries.TabIndex = 6
        Me.LabelRetries.Text = "Retries"
        Me.LabelRetries.UseMnemonic = False
        '
        'LabelSeekRetries
        '
        Me.LabelSeekRetries.AutoSize = True
        Me.LabelSeekRetries.Location = New System.Drawing.Point(157, 81)
        Me.LabelSeekRetries.Name = "LabelSeekRetries"
        Me.LabelSeekRetries.Size = New System.Drawing.Size(68, 13)
        Me.LabelSeekRetries.TabIndex = 7
        Me.LabelSeekRetries.Text = "Seek Retries"
        Me.LabelSeekRetries.UseMnemonic = False
        '
        'NumericRevs
        '
        Me.NumericRevs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NumericRevs.Location = New System.Drawing.Point(3, 97)
        Me.NumericRevs.Margin = New System.Windows.Forms.Padding(3, 3, 3, 4)
        Me.NumericRevs.MinimumSize = New System.Drawing.Size(68, 0)
        Me.NumericRevs.Name = "NumericRevs"
        Me.NumericRevs.Size = New System.Drawing.Size(71, 20)
        Me.NumericRevs.TabIndex = 8
        '
        'NumericRetries
        '
        Me.NumericRetries.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NumericRetries.Location = New System.Drawing.Point(80, 97)
        Me.NumericRetries.Margin = New System.Windows.Forms.Padding(3, 3, 3, 4)
        Me.NumericRetries.MinimumSize = New System.Drawing.Size(68, 0)
        Me.NumericRetries.Name = "NumericRetries"
        Me.NumericRetries.Size = New System.Drawing.Size(71, 20)
        Me.NumericRetries.TabIndex = 9
        '
        'NumericSeekRetries
        '
        Me.NumericSeekRetries.Dock = System.Windows.Forms.DockStyle.Fill
        Me.NumericSeekRetries.Location = New System.Drawing.Point(157, 97)
        Me.NumericSeekRetries.Margin = New System.Windows.Forms.Padding(3, 3, 3, 4)
        Me.NumericSeekRetries.MinimumSize = New System.Drawing.Size(68, 0)
        Me.NumericSeekRetries.Name = "NumericSeekRetries"
        Me.NumericSeekRetries.Size = New System.Drawing.Size(73, 20)
        Me.NumericSeekRetries.TabIndex = 10
        '
        'ButtonDetect
        '
        Me.ButtonDetect.AutoSize = True
        Me.ButtonDetect.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ButtonDetect.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ButtonDetect.Location = New System.Drawing.Point(157, 15)
        Me.ButtonDetect.Margin = New System.Windows.Forms.Padding(3, 2, 3, 3)
        Me.ButtonDetect.Name = "ButtonDetect"
        Me.ButtonDetect.Size = New System.Drawing.Size(73, 23)
        Me.ButtonDetect.TabIndex = 2
        Me.ButtonDetect.Text = "Detect"
        Me.ButtonDetect.UseVisualStyleBackColor = True
        '
        'ReadDiskDriveParams
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Controls.Add(Me.GroupBoxParams)
        Me.Name = "ReadDiskDriveParams"
        Me.Size = New System.Drawing.Size(248, 153)
        Me.GroupBoxParams.ResumeLayout(False)
        Me.GroupBoxParams.PerformLayout()
        Me.TableLayoutPanelParams.ResumeLayout(False)
        Me.TableLayoutPanelParams.PerformLayout()
        CType(Me.NumericRevs, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericRetries, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericSeekRetries, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents GroupBoxParams As GroupBox
    Friend WithEvents TableLayoutPanelParams As TableLayoutPanel
    Friend WithEvents LabelDrive As Label
    Friend WithEvents ComboDrives As ComboBox
    Friend WithEvents LabelImageFormat As Label
    Friend WithEvents ComboImageFormat As ComboBox
    Friend WithEvents LabelRevs As Label
    Friend WithEvents LabelRetries As Label
    Friend WithEvents LabelSeekRetries As Label
    Friend WithEvents NumericRevs As NumericUpDown
    Friend WithEvents NumericRetries As NumericUpDown
    Friend WithEvents NumericSeekRetries As NumericUpDown
    Friend WithEvents ButtonDetect As Button
End Class
