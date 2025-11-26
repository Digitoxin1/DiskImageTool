Namespace Flux.Greaseweazle
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class SettingsPanel
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
            Dim TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
            Me.LabelApplicationPath = New System.Windows.Forms.Label()
            Me.LabelDriveInterface = New System.Windows.Forms.Label()
            Me.LabelDriveType0 = New System.Windows.Forms.Label()
            Me.LabelDriveType1 = New System.Windows.Forms.Label()
            Me.LabelDriveType2 = New System.Windows.Forms.Label()
            Me.ComboDriveType2 = New System.Windows.Forms.ComboBox()
            Me.ComboDriveType1 = New System.Windows.Forms.ComboBox()
            Me.ComboDriveType0 = New System.Windows.Forms.ComboBox()
            Me.ComboInterface = New System.Windows.Forms.ComboBox()
            Me.TextBoxPath = New System.Windows.Forms.TextBox()
            Me.ButtonBrowse = New System.Windows.Forms.Button()
            Me.ButtonClear = New System.Windows.Forms.Button()
            Me.TextBoxInfo = New System.Windows.Forms.TextBox()
            Me.LabelTracks0 = New System.Windows.Forms.Label()
            Me.LabelTracks1 = New System.Windows.Forms.Label()
            Me.LabelTracks2 = New System.Windows.Forms.Label()
            Me.NumericTracks1 = New System.Windows.Forms.NumericUpDown()
            Me.NumericTracks2 = New System.Windows.Forms.NumericUpDown()
            Me.NumericTracks0 = New System.Windows.Forms.NumericUpDown()
            Me.ButtonInfo = New System.Windows.Forms.Button()
            Me.LabelPort = New System.Windows.Forms.Label()
            Me.ComboPorts = New System.Windows.Forms.ComboBox()
            Me.LabelDefaultRevs = New System.Windows.Forms.Label()
            Me.NumericDefaultRevs = New System.Windows.Forms.NumericUpDown()
            Me.LabelLogFile = New System.Windows.Forms.Label()
            Me.TextBoxLogFile = New System.Windows.Forms.TextBox()
            TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            TableLayoutPanel1.SuspendLayout()
            CType(Me.NumericTracks1, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.NumericTracks2, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.NumericTracks0, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.NumericDefaultRevs, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'TableLayoutPanel1
            '
            TableLayoutPanel1.AutoSize = True
            TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            TableLayoutPanel1.ColumnCount = 7
            TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            TableLayoutPanel1.Controls.Add(Me.LabelApplicationPath, 0, 0)
            TableLayoutPanel1.Controls.Add(Me.LabelDriveInterface, 0, 1)
            TableLayoutPanel1.Controls.Add(Me.LabelDriveType0, 0, 2)
            TableLayoutPanel1.Controls.Add(Me.LabelDriveType1, 0, 3)
            TableLayoutPanel1.Controls.Add(Me.LabelDriveType2, 0, 4)
            TableLayoutPanel1.Controls.Add(Me.ComboDriveType2, 1, 4)
            TableLayoutPanel1.Controls.Add(Me.ComboDriveType1, 1, 3)
            TableLayoutPanel1.Controls.Add(Me.ComboDriveType0, 1, 2)
            TableLayoutPanel1.Controls.Add(Me.ComboInterface, 1, 1)
            TableLayoutPanel1.Controls.Add(Me.TextBoxPath, 1, 0)
            TableLayoutPanel1.Controls.Add(Me.ButtonBrowse, 5, 0)
            TableLayoutPanel1.Controls.Add(Me.ButtonClear, 6, 0)
            TableLayoutPanel1.Controls.Add(Me.TextBoxInfo, 4, 2)
            TableLayoutPanel1.Controls.Add(Me.LabelTracks0, 2, 2)
            TableLayoutPanel1.Controls.Add(Me.LabelTracks1, 2, 3)
            TableLayoutPanel1.Controls.Add(Me.LabelTracks2, 2, 4)
            TableLayoutPanel1.Controls.Add(Me.NumericTracks1, 3, 3)
            TableLayoutPanel1.Controls.Add(Me.NumericTracks2, 3, 4)
            TableLayoutPanel1.Controls.Add(Me.NumericTracks0, 3, 2)
            TableLayoutPanel1.Controls.Add(Me.ButtonInfo, 4, 1)
            TableLayoutPanel1.Controls.Add(Me.LabelPort, 2, 1)
            TableLayoutPanel1.Controls.Add(Me.ComboPorts, 3, 1)
            TableLayoutPanel1.Controls.Add(Me.LabelDefaultRevs, 0, 5)
            TableLayoutPanel1.Controls.Add(Me.NumericDefaultRevs, 1, 5)
            TableLayoutPanel1.Controls.Add(Me.LabelLogFile, 0, 6)
            TableLayoutPanel1.Controls.Add(Me.TextBoxLogFile, 1, 6)
            TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
            TableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize
            TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
            TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
            TableLayoutPanel1.Name = "TableLayoutPanel1"
            TableLayoutPanel1.RowCount = 8
            TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            TableLayoutPanel1.Size = New System.Drawing.Size(680, 216)
            TableLayoutPanel1.TabIndex = 0
            '
            'LabelApplicationPath
            '
            Me.LabelApplicationPath.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.LabelApplicationPath.AutoSize = True
            Me.LabelApplicationPath.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.LabelApplicationPath.Location = New System.Drawing.Point(3, 8)
            Me.LabelApplicationPath.Name = "LabelApplicationPath"
            Me.LabelApplicationPath.Size = New System.Drawing.Size(92, 13)
            Me.LabelApplicationPath.TabIndex = 0
            Me.LabelApplicationPath.Text = "{Application Path}"
            '
            'LabelDriveInterface
            '
            Me.LabelDriveInterface.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.LabelDriveInterface.AutoSize = True
            Me.LabelDriveInterface.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.LabelDriveInterface.Location = New System.Drawing.Point(3, 37)
            Me.LabelDriveInterface.Name = "LabelDriveInterface"
            Me.LabelDriveInterface.Size = New System.Drawing.Size(85, 13)
            Me.LabelDriveInterface.TabIndex = 4
            Me.LabelDriveInterface.Text = "{Drive Interface}"
            '
            'LabelDriveType0
            '
            Me.LabelDriveType0.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.LabelDriveType0.AutoSize = True
            Me.LabelDriveType0.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.LabelDriveType0.Location = New System.Drawing.Point(3, 65)
            Me.LabelDriveType0.Name = "LabelDriveType0"
            Me.LabelDriveType0.Size = New System.Drawing.Size(76, 13)
            Me.LabelDriveType0.TabIndex = 8
            Me.LabelDriveType0.Text = "{Drive 0 Type}"
            '
            'LabelDriveType1
            '
            Me.LabelDriveType1.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.LabelDriveType1.AutoSize = True
            Me.LabelDriveType1.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.LabelDriveType1.Location = New System.Drawing.Point(3, 92)
            Me.LabelDriveType1.Name = "LabelDriveType1"
            Me.LabelDriveType1.Size = New System.Drawing.Size(76, 13)
            Me.LabelDriveType1.TabIndex = 12
            Me.LabelDriveType1.Text = "{Drive 1 Type}"
            '
            'LabelDriveType2
            '
            Me.LabelDriveType2.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.LabelDriveType2.AutoSize = True
            Me.LabelDriveType2.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.LabelDriveType2.Location = New System.Drawing.Point(3, 119)
            Me.LabelDriveType2.Name = "LabelDriveType2"
            Me.LabelDriveType2.Size = New System.Drawing.Size(76, 13)
            Me.LabelDriveType2.TabIndex = 16
            Me.LabelDriveType2.Text = "{Drive 2 Type}"
            '
            'ComboDriveType2
            '
            Me.ComboDriveType2.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.ComboDriveType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboDriveType2.FormattingEnabled = True
            Me.ComboDriveType2.Location = New System.Drawing.Point(101, 115)
            Me.ComboDriveType2.Name = "ComboDriveType2"
            Me.ComboDriveType2.Size = New System.Drawing.Size(125, 21)
            Me.ComboDriveType2.TabIndex = 17
            '
            'ComboDriveType1
            '
            Me.ComboDriveType1.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.ComboDriveType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboDriveType1.FormattingEnabled = True
            Me.ComboDriveType1.Location = New System.Drawing.Point(101, 88)
            Me.ComboDriveType1.Name = "ComboDriveType1"
            Me.ComboDriveType1.Size = New System.Drawing.Size(125, 21)
            Me.ComboDriveType1.TabIndex = 13
            '
            'ComboDriveType0
            '
            Me.ComboDriveType0.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.ComboDriveType0.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboDriveType0.FormattingEnabled = True
            Me.ComboDriveType0.Location = New System.Drawing.Point(101, 61)
            Me.ComboDriveType0.Name = "ComboDriveType0"
            Me.ComboDriveType0.Size = New System.Drawing.Size(125, 21)
            Me.ComboDriveType0.TabIndex = 9
            '
            'ComboInterface
            '
            Me.ComboInterface.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.ComboInterface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboInterface.FormattingEnabled = True
            Me.ComboInterface.Location = New System.Drawing.Point(101, 33)
            Me.ComboInterface.Name = "ComboInterface"
            Me.ComboInterface.Size = New System.Drawing.Size(100, 21)
            Me.ComboInterface.TabIndex = 5
            '
            'TextBoxPath
            '
            Me.TextBoxPath.AllowDrop = True
            Me.TextBoxPath.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TextBoxPath.BackColor = System.Drawing.SystemColors.Window
            TableLayoutPanel1.SetColumnSpan(Me.TextBoxPath, 4)
            Me.TextBoxPath.Location = New System.Drawing.Point(101, 4)
            Me.TextBoxPath.Name = "TextBoxPath"
            Me.TextBoxPath.ReadOnly = True
            Me.TextBoxPath.Size = New System.Drawing.Size(414, 20)
            Me.TextBoxPath.TabIndex = 1
            '
            'ButtonBrowse
            '
            Me.ButtonBrowse.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.ButtonBrowse.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.ButtonBrowse.Location = New System.Drawing.Point(521, 3)
            Me.ButtonBrowse.Name = "ButtonBrowse"
            Me.ButtonBrowse.Size = New System.Drawing.Size(75, 23)
            Me.ButtonBrowse.TabIndex = 2
            Me.ButtonBrowse.Text = "{Browse}"
            Me.ButtonBrowse.UseVisualStyleBackColor = True
            '
            'ButtonClear
            '
            Me.ButtonClear.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.ButtonClear.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.ButtonClear.Location = New System.Drawing.Point(602, 3)
            Me.ButtonClear.Name = "ButtonClear"
            Me.ButtonClear.Size = New System.Drawing.Size(75, 23)
            Me.ButtonClear.TabIndex = 3
            Me.ButtonClear.Text = "{Clear}"
            Me.ButtonClear.UseVisualStyleBackColor = True
            '
            'TextBoxInfo
            '
            Me.TextBoxInfo.BackColor = System.Drawing.SystemColors.Window
            TableLayoutPanel1.SetColumnSpan(Me.TextBoxInfo, 3)
            Me.TextBoxInfo.Dock = System.Windows.Forms.DockStyle.Fill
            Me.TextBoxInfo.Font = New System.Drawing.Font("Consolas", 9.0!)
            Me.TextBoxInfo.Location = New System.Drawing.Point(360, 61)
            Me.TextBoxInfo.Margin = New System.Windows.Forms.Padding(9, 3, 3, 3)
            Me.TextBoxInfo.Multiline = True
            Me.TextBoxInfo.Name = "TextBoxInfo"
            Me.TextBoxInfo.ReadOnly = True
            TableLayoutPanel1.SetRowSpan(Me.TextBoxInfo, 6)
            Me.TextBoxInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.TextBoxInfo.ShortcutsEnabled = False
            Me.TextBoxInfo.Size = New System.Drawing.Size(317, 152)
            Me.TextBoxInfo.TabIndex = 25
            '
            'LabelTracks0
            '
            Me.LabelTracks0.Anchor = System.Windows.Forms.AnchorStyles.Right
            Me.LabelTracks0.AutoSize = True
            Me.LabelTracks0.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.LabelTracks0.Location = New System.Drawing.Point(232, 65)
            Me.LabelTracks0.Name = "LabelTracks0"
            Me.LabelTracks0.Size = New System.Drawing.Size(48, 13)
            Me.LabelTracks0.TabIndex = 10
            Me.LabelTracks0.Text = "{Tracks}"
            '
            'LabelTracks1
            '
            Me.LabelTracks1.Anchor = System.Windows.Forms.AnchorStyles.Right
            Me.LabelTracks1.AutoSize = True
            Me.LabelTracks1.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.LabelTracks1.Location = New System.Drawing.Point(232, 92)
            Me.LabelTracks1.Name = "LabelTracks1"
            Me.LabelTracks1.Size = New System.Drawing.Size(48, 13)
            Me.LabelTracks1.TabIndex = 14
            Me.LabelTracks1.Text = "{Tracks}"
            '
            'LabelTracks2
            '
            Me.LabelTracks2.Anchor = System.Windows.Forms.AnchorStyles.Right
            Me.LabelTracks2.AutoSize = True
            Me.LabelTracks2.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.LabelTracks2.Location = New System.Drawing.Point(232, 119)
            Me.LabelTracks2.Name = "LabelTracks2"
            Me.LabelTracks2.Size = New System.Drawing.Size(48, 13)
            Me.LabelTracks2.TabIndex = 18
            Me.LabelTracks2.Text = "{Tracks}"
            '
            'NumericTracks1
            '
            Me.NumericTracks1.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.NumericTracks1.Location = New System.Drawing.Point(286, 88)
            Me.NumericTracks1.Name = "NumericTracks1"
            Me.NumericTracks1.Size = New System.Drawing.Size(62, 20)
            Me.NumericTracks1.TabIndex = 15
            '
            'NumericTracks2
            '
            Me.NumericTracks2.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.NumericTracks2.Location = New System.Drawing.Point(286, 115)
            Me.NumericTracks2.Name = "NumericTracks2"
            Me.NumericTracks2.Size = New System.Drawing.Size(62, 20)
            Me.NumericTracks2.TabIndex = 19
            '
            'NumericTracks0
            '
            Me.NumericTracks0.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.NumericTracks0.Location = New System.Drawing.Point(286, 61)
            Me.NumericTracks0.Name = "NumericTracks0"
            Me.NumericTracks0.Size = New System.Drawing.Size(62, 20)
            Me.NumericTracks0.TabIndex = 11
            '
            'ButtonInfo
            '
            Me.ButtonInfo.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.ButtonInfo.Location = New System.Drawing.Point(360, 32)
            Me.ButtonInfo.Margin = New System.Windows.Forms.Padding(9, 3, 3, 3)
            Me.ButtonInfo.Name = "ButtonInfo"
            Me.ButtonInfo.Size = New System.Drawing.Size(75, 23)
            Me.ButtonInfo.TabIndex = 24
            Me.ButtonInfo.Text = "{Info}"
            Me.ButtonInfo.UseVisualStyleBackColor = True
            '
            'LabelPort
            '
            Me.LabelPort.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.LabelPort.AutoSize = True
            Me.LabelPort.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.LabelPort.Location = New System.Drawing.Point(232, 37)
            Me.LabelPort.Name = "LabelPort"
            Me.LabelPort.Size = New System.Drawing.Size(34, 13)
            Me.LabelPort.TabIndex = 6
            Me.LabelPort.Text = "{Port}"
            '
            'ComboPorts
            '
            Me.ComboPorts.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.ComboPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboPorts.FormattingEnabled = True
            Me.ComboPorts.Location = New System.Drawing.Point(286, 33)
            Me.ComboPorts.Name = "ComboPorts"
            Me.ComboPorts.Size = New System.Drawing.Size(62, 21)
            Me.ComboPorts.TabIndex = 7
            '
            'LabelDefaultRevs
            '
            Me.LabelDefaultRevs.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.LabelDefaultRevs.AutoSize = True
            Me.LabelDefaultRevs.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.LabelDefaultRevs.Location = New System.Drawing.Point(3, 145)
            Me.LabelDefaultRevs.Name = "LabelDefaultRevs"
            Me.LabelDefaultRevs.Size = New System.Drawing.Size(77, 13)
            Me.LabelDefaultRevs.TabIndex = 20
            Me.LabelDefaultRevs.Text = "{Default Revs}"
            '
            'NumericDefaultRevs
            '
            Me.NumericDefaultRevs.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.NumericDefaultRevs.Location = New System.Drawing.Point(101, 142)
            Me.NumericDefaultRevs.Name = "NumericDefaultRevs"
            Me.NumericDefaultRevs.Size = New System.Drawing.Size(46, 20)
            Me.NumericDefaultRevs.TabIndex = 21
            '
            'LabelLogFile
            '
            Me.LabelLogFile.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.LabelLogFile.AutoSize = True
            Me.LabelLogFile.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.LabelLogFile.Location = New System.Drawing.Point(3, 171)
            Me.LabelLogFile.Name = "LabelLogFile"
            Me.LabelLogFile.Size = New System.Drawing.Size(78, 13)
            Me.LabelLogFile.TabIndex = 22
            Me.LabelLogFile.Text = "{Log Filename}"
            '
            'TextBoxLogFile
            '
            Me.TextBoxLogFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TextBoxLogFile.Location = New System.Drawing.Point(101, 168)
            Me.TextBoxLogFile.MaxLength = 50
            Me.TextBoxLogFile.Name = "TextBoxLogFile"
            Me.TextBoxLogFile.Size = New System.Drawing.Size(125, 20)
            Me.TextBoxLogFile.TabIndex = 23
            '
            'SettingsPanel
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(TableLayoutPanel1)
            Me.Name = "SettingsPanel"
            Me.Size = New System.Drawing.Size(680, 216)
            TableLayoutPanel1.ResumeLayout(False)
            TableLayoutPanel1.PerformLayout()
            CType(Me.NumericTracks1, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.NumericTracks2, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.NumericTracks0, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.NumericDefaultRevs, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents LabelDriveType0 As Label
        Friend WithEvents LabelDriveType1 As Label
        Friend WithEvents LabelDriveType2 As Label
        Friend WithEvents ComboDriveType2 As ComboBox
        Friend WithEvents ComboDriveType1 As ComboBox
        Friend WithEvents ComboDriveType0 As ComboBox
        Friend WithEvents ComboInterface As ComboBox
        Friend WithEvents TextBoxPath As TextBox
        Friend WithEvents ButtonBrowse As Button
        Friend WithEvents ButtonClear As Button
        Friend WithEvents TextBoxInfo As TextBox
        Friend WithEvents LabelTracks2 As Label
        Friend WithEvents NumericTracks1 As NumericUpDown
        Friend WithEvents NumericTracks2 As NumericUpDown
        Friend WithEvents NumericTracks0 As NumericUpDown
        Friend WithEvents ButtonInfo As Button
        Friend WithEvents ComboPorts As ComboBox
        Friend WithEvents NumericDefaultRevs As NumericUpDown
        Friend WithEvents TextBoxLogFile As TextBox
        Friend WithEvents LabelApplicationPath As Label
        Friend WithEvents LabelTracks0 As Label
        Friend WithEvents LabelTracks1 As Label
        Friend WithEvents LabelLogFile As Label
        Friend WithEvents LabelDriveInterface As Label
        Friend WithEvents LabelPort As Label
        Friend WithEvents LabelDefaultRevs As Label
    End Class
End Namespace