Namespace Flux.Kryoflux
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
            Me.LabelLogFile = New System.Windows.Forms.Label()
            Me.LabelApplicationPath = New System.Windows.Forms.Label()
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.TextBoxPath = New System.Windows.Forms.TextBox()
            Me.ButtonBrowse = New System.Windows.Forms.Button()
            Me.ButtonClear = New System.Windows.Forms.Button()
            Me.TextBoxLogFile = New System.Windows.Forms.TextBox()
            Me.CheckBoxStripPath = New System.Windows.Forms.CheckBox()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'LabelLogFile
            '
            Me.LabelLogFile.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.LabelLogFile.AutoSize = True
            Me.LabelLogFile.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.LabelLogFile.Location = New System.Drawing.Point(3, 35)
            Me.LabelLogFile.Name = "LabelLogFile"
            Me.LabelLogFile.Size = New System.Drawing.Size(78, 13)
            Me.LabelLogFile.TabIndex = 22
            Me.LabelLogFile.Text = "{Log Filename}"
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
            'TableLayoutPanel1
            '
            Me.TableLayoutPanel1.AutoSize = True
            Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.TableLayoutPanel1.ColumnCount = 7
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.TableLayoutPanel1.Controls.Add(Me.LabelApplicationPath, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBoxPath, 1, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.ButtonBrowse, 5, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.ButtonClear, 6, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.LabelLogFile, 0, 6)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBoxLogFile, 1, 6)
            Me.TableLayoutPanel1.Controls.Add(Me.CheckBoxStripPath, 2, 6)
            Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
            Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            Me.TableLayoutPanel1.RowCount = 8
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.TableLayoutPanel1.Size = New System.Drawing.Size(584, 205)
            Me.TableLayoutPanel1.TabIndex = 1
            '
            'TextBoxPath
            '
            Me.TextBoxPath.AllowDrop = True
            Me.TextBoxPath.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TextBoxPath.BackColor = System.Drawing.SystemColors.Window
            Me.TableLayoutPanel1.SetColumnSpan(Me.TextBoxPath, 4)
            Me.TextBoxPath.Location = New System.Drawing.Point(101, 4)
            Me.TextBoxPath.Name = "TextBoxPath"
            Me.TextBoxPath.ReadOnly = True
            Me.TextBoxPath.Size = New System.Drawing.Size(318, 20)
            Me.TextBoxPath.TabIndex = 1
            '
            'ButtonBrowse
            '
            Me.ButtonBrowse.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.ButtonBrowse.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.ButtonBrowse.Location = New System.Drawing.Point(425, 3)
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
            Me.ButtonClear.Location = New System.Drawing.Point(506, 3)
            Me.ButtonClear.Name = "ButtonClear"
            Me.ButtonClear.Size = New System.Drawing.Size(75, 23)
            Me.ButtonClear.TabIndex = 3
            Me.ButtonClear.Text = "{Clear}"
            Me.ButtonClear.UseVisualStyleBackColor = True
            '
            'TextBoxLogFile
            '
            Me.TextBoxLogFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TextBoxLogFile.Location = New System.Drawing.Point(101, 32)
            Me.TextBoxLogFile.MaxLength = 50
            Me.TextBoxLogFile.Name = "TextBoxLogFile"
            Me.TextBoxLogFile.Size = New System.Drawing.Size(100, 20)
            Me.TextBoxLogFile.TabIndex = 23
            '
            'CheckBoxStripPath
            '
            Me.CheckBoxStripPath.Anchor = System.Windows.Forms.AnchorStyles.Left
            Me.CheckBoxStripPath.AutoSize = True
            Me.TableLayoutPanel1.SetColumnSpan(Me.CheckBoxStripPath, 3)
            Me.CheckBoxStripPath.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.CheckBoxStripPath.Location = New System.Drawing.Point(216, 33)
            Me.CheckBoxStripPath.Margin = New System.Windows.Forms.Padding(12, 3, 3, 3)
            Me.CheckBoxStripPath.Name = "CheckBoxStripPath"
            Me.CheckBoxStripPath.Size = New System.Drawing.Size(129, 17)
            Me.CheckBoxStripPath.TabIndex = 24
            Me.CheckBoxStripPath.Text = "{Strip Path from Logs}"
            Me.CheckBoxStripPath.UseVisualStyleBackColor = True
            '
            'SettingsPanel
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Name = "SettingsPanel"
            Me.Size = New System.Drawing.Size(584, 205)
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
        Friend WithEvents TextBoxPath As TextBox
        Friend WithEvents ButtonBrowse As Button
        Friend WithEvents ButtonClear As Button
        Friend WithEvents TextBoxLogFile As TextBox
        Friend WithEvents CheckBoxStripPath As CheckBox
        Friend WithEvents LabelApplicationPath As Label
        Friend WithEvents LabelLogFile As Label
    End Class
End Namespace
