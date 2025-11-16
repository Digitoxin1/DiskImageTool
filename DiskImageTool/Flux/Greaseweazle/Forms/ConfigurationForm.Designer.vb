Namespace Greaseweazle
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class ConfigurationForm
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
            Dim LabelApplicationPath As System.Windows.Forms.Label
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ConfigurationForm))
            Dim LabelDriveInterface As System.Windows.Forms.Label
            Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
            Dim PanelMain As System.Windows.Forms.Panel
            Dim LabelTracks0 As System.Windows.Forms.Label
            Dim LabelTracks1 As System.Windows.Forms.Label
            Dim LabelPort As System.Windows.Forms.Label
            Dim LabelDefaultRevs As System.Windows.Forms.Label
            Dim LabelLogFile As System.Windows.Forms.Label
            Me.BtnCancel = New System.Windows.Forms.Button()
            Me.BtnUpdate = New System.Windows.Forms.Button()
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
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
            Me.LabelTracks2 = New System.Windows.Forms.Label()
            Me.NumericTracks1 = New System.Windows.Forms.NumericUpDown()
            Me.NumericTracks2 = New System.Windows.Forms.NumericUpDown()
            Me.NumericTracks0 = New System.Windows.Forms.NumericUpDown()
            Me.ButtonInfo = New System.Windows.Forms.Button()
            Me.ComboPorts = New System.Windows.Forms.ComboBox()
            Me.NumericDefaultRevs = New System.Windows.Forms.NumericUpDown()
            Me.TextBoxLogFile = New System.Windows.Forms.TextBox()
            LabelApplicationPath = New System.Windows.Forms.Label()
            LabelDriveInterface = New System.Windows.Forms.Label()
            PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
            PanelMain = New System.Windows.Forms.Panel()
            LabelTracks0 = New System.Windows.Forms.Label()
            LabelTracks1 = New System.Windows.Forms.Label()
            LabelPort = New System.Windows.Forms.Label()
            LabelDefaultRevs = New System.Windows.Forms.Label()
            LabelLogFile = New System.Windows.Forms.Label()
            PanelBottom.SuspendLayout()
            PanelMain.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            CType(Me.NumericTracks1, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.NumericTracks2, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.NumericTracks0, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.NumericDefaultRevs, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'LabelApplicationPath
            '
            resources.ApplyResources(LabelApplicationPath, "LabelApplicationPath")
            LabelApplicationPath.Name = "LabelApplicationPath"
            '
            'LabelDriveInterface
            '
            resources.ApplyResources(LabelDriveInterface, "LabelDriveInterface")
            LabelDriveInterface.Name = "LabelDriveInterface"
            '
            'PanelBottom
            '
            PanelBottom.Controls.Add(Me.BtnCancel)
            PanelBottom.Controls.Add(Me.BtnUpdate)
            resources.ApplyResources(PanelBottom, "PanelBottom")
            PanelBottom.Name = "PanelBottom"
            '
            'BtnCancel
            '
            Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            resources.ApplyResources(Me.BtnCancel, "BtnCancel")
            Me.BtnCancel.Name = "BtnCancel"
            Me.BtnCancel.UseVisualStyleBackColor = True
            '
            'BtnUpdate
            '
            Me.BtnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK
            resources.ApplyResources(Me.BtnUpdate, "BtnUpdate")
            Me.BtnUpdate.Name = "BtnUpdate"
            Me.BtnUpdate.UseVisualStyleBackColor = True
            '
            'PanelMain
            '
            resources.ApplyResources(PanelMain, "PanelMain")
            PanelMain.Controls.Add(Me.TableLayoutPanel1)
            PanelMain.Name = "PanelMain"
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(LabelApplicationPath, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(LabelDriveInterface, 0, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.LabelDriveType0, 0, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.LabelDriveType1, 0, 3)
            Me.TableLayoutPanel1.Controls.Add(Me.LabelDriveType2, 0, 4)
            Me.TableLayoutPanel1.Controls.Add(Me.ComboDriveType2, 1, 4)
            Me.TableLayoutPanel1.Controls.Add(Me.ComboDriveType1, 1, 3)
            Me.TableLayoutPanel1.Controls.Add(Me.ComboDriveType0, 1, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.ComboInterface, 1, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBoxPath, 1, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.ButtonBrowse, 5, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.ButtonClear, 6, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBoxInfo, 4, 2)
            Me.TableLayoutPanel1.Controls.Add(LabelTracks0, 2, 2)
            Me.TableLayoutPanel1.Controls.Add(LabelTracks1, 2, 3)
            Me.TableLayoutPanel1.Controls.Add(Me.LabelTracks2, 2, 4)
            Me.TableLayoutPanel1.Controls.Add(Me.NumericTracks1, 3, 3)
            Me.TableLayoutPanel1.Controls.Add(Me.NumericTracks2, 3, 4)
            Me.TableLayoutPanel1.Controls.Add(Me.NumericTracks0, 3, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.ButtonInfo, 4, 1)
            Me.TableLayoutPanel1.Controls.Add(LabelPort, 2, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.ComboPorts, 3, 1)
            Me.TableLayoutPanel1.Controls.Add(LabelDefaultRevs, 0, 5)
            Me.TableLayoutPanel1.Controls.Add(Me.NumericDefaultRevs, 1, 5)
            Me.TableLayoutPanel1.Controls.Add(LabelLogFile, 0, 6)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBoxLogFile, 1, 6)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'LabelDriveType0
            '
            resources.ApplyResources(Me.LabelDriveType0, "LabelDriveType0")
            Me.LabelDriveType0.Name = "LabelDriveType0"
            '
            'LabelDriveType1
            '
            resources.ApplyResources(Me.LabelDriveType1, "LabelDriveType1")
            Me.LabelDriveType1.Name = "LabelDriveType1"
            '
            'LabelDriveType2
            '
            resources.ApplyResources(Me.LabelDriveType2, "LabelDriveType2")
            Me.LabelDriveType2.Name = "LabelDriveType2"
            '
            'ComboDriveType2
            '
            resources.ApplyResources(Me.ComboDriveType2, "ComboDriveType2")
            Me.ComboDriveType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboDriveType2.FormattingEnabled = True
            Me.ComboDriveType2.Name = "ComboDriveType2"
            '
            'ComboDriveType1
            '
            resources.ApplyResources(Me.ComboDriveType1, "ComboDriveType1")
            Me.ComboDriveType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboDriveType1.FormattingEnabled = True
            Me.ComboDriveType1.Name = "ComboDriveType1"
            '
            'ComboDriveType0
            '
            resources.ApplyResources(Me.ComboDriveType0, "ComboDriveType0")
            Me.ComboDriveType0.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboDriveType0.FormattingEnabled = True
            Me.ComboDriveType0.Name = "ComboDriveType0"
            '
            'ComboInterface
            '
            resources.ApplyResources(Me.ComboInterface, "ComboInterface")
            Me.ComboInterface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboInterface.FormattingEnabled = True
            Me.ComboInterface.Name = "ComboInterface"
            '
            'TextBoxPath
            '
            Me.TextBoxPath.AllowDrop = True
            resources.ApplyResources(Me.TextBoxPath, "TextBoxPath")
            Me.TextBoxPath.BackColor = System.Drawing.SystemColors.Window
            Me.TableLayoutPanel1.SetColumnSpan(Me.TextBoxPath, 4)
            Me.TextBoxPath.Name = "TextBoxPath"
            Me.TextBoxPath.ReadOnly = True
            '
            'ButtonBrowse
            '
            resources.ApplyResources(Me.ButtonBrowse, "ButtonBrowse")
            Me.ButtonBrowse.Name = "ButtonBrowse"
            Me.ButtonBrowse.UseVisualStyleBackColor = True
            '
            'ButtonClear
            '
            resources.ApplyResources(Me.ButtonClear, "ButtonClear")
            Me.ButtonClear.Name = "ButtonClear"
            Me.ButtonClear.UseVisualStyleBackColor = True
            '
            'TextBoxInfo
            '
            resources.ApplyResources(Me.TextBoxInfo, "TextBoxInfo")
            Me.TextBoxInfo.BackColor = System.Drawing.SystemColors.Window
            Me.TableLayoutPanel1.SetColumnSpan(Me.TextBoxInfo, 3)
            Me.TextBoxInfo.Name = "TextBoxInfo"
            Me.TextBoxInfo.ReadOnly = True
            Me.TableLayoutPanel1.SetRowSpan(Me.TextBoxInfo, 6)
            Me.TextBoxInfo.ShortcutsEnabled = False
            '
            'LabelTracks0
            '
            resources.ApplyResources(LabelTracks0, "LabelTracks0")
            LabelTracks0.Name = "LabelTracks0"
            '
            'LabelTracks1
            '
            resources.ApplyResources(LabelTracks1, "LabelTracks1")
            LabelTracks1.Name = "LabelTracks1"
            '
            'LabelTracks2
            '
            resources.ApplyResources(Me.LabelTracks2, "LabelTracks2")
            Me.LabelTracks2.Name = "LabelTracks2"
            '
            'NumericTracks1
            '
            resources.ApplyResources(Me.NumericTracks1, "NumericTracks1")
            Me.NumericTracks1.Name = "NumericTracks1"
            '
            'NumericTracks2
            '
            resources.ApplyResources(Me.NumericTracks2, "NumericTracks2")
            Me.NumericTracks2.Name = "NumericTracks2"
            '
            'NumericTracks0
            '
            resources.ApplyResources(Me.NumericTracks0, "NumericTracks0")
            Me.NumericTracks0.Name = "NumericTracks0"
            '
            'ButtonInfo
            '
            resources.ApplyResources(Me.ButtonInfo, "ButtonInfo")
            Me.ButtonInfo.Name = "ButtonInfo"
            Me.ButtonInfo.UseVisualStyleBackColor = True
            '
            'LabelPort
            '
            resources.ApplyResources(LabelPort, "LabelPort")
            LabelPort.Name = "LabelPort"
            '
            'ComboPorts
            '
            resources.ApplyResources(Me.ComboPorts, "ComboPorts")
            Me.ComboPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ComboPorts.FormattingEnabled = True
            Me.ComboPorts.Name = "ComboPorts"
            '
            'LabelDefaultRevs
            '
            resources.ApplyResources(LabelDefaultRevs, "LabelDefaultRevs")
            LabelDefaultRevs.Name = "LabelDefaultRevs"
            '
            'NumericDefaultRevs
            '
            resources.ApplyResources(Me.NumericDefaultRevs, "NumericDefaultRevs")
            Me.NumericDefaultRevs.Name = "NumericDefaultRevs"
            '
            'LabelLogFile
            '
            resources.ApplyResources(LabelLogFile, "LabelLogFile")
            LabelLogFile.Name = "LabelLogFile"
            '
            'TextBoxLogFile
            '
            resources.ApplyResources(Me.TextBoxLogFile, "TextBoxLogFile")
            Me.TextBoxLogFile.Name = "TextBoxLogFile"
            '
            'ConfigurationForm
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(PanelMain)
            Me.Controls.Add(PanelBottom)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ConfigurationForm"
            Me.ShowInTaskbar = False
            PanelBottom.ResumeLayout(False)
            PanelMain.ResumeLayout(False)
            PanelMain.PerformLayout()
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            CType(Me.NumericTracks1, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.NumericTracks2, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.NumericTracks0, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.NumericDefaultRevs, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents BtnUpdate As Button
        Friend WithEvents BtnCancel As Button
        Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
        Friend WithEvents ComboDriveType1 As ComboBox
        Friend WithEvents ComboDriveType0 As ComboBox
        Friend WithEvents ComboInterface As ComboBox
        Friend WithEvents TextBoxPath As TextBox
        Friend WithEvents ButtonBrowse As Button
        Friend WithEvents ComboDriveType2 As ComboBox
        Friend WithEvents LabelDriveType2 As Label
        Friend WithEvents LabelDriveType0 As Label
        Friend WithEvents LabelDriveType1 As Label
        Friend WithEvents ButtonInfo As Button
        Friend WithEvents TextBoxInfo As TextBox
        Friend WithEvents ButtonClear As Button
        Friend WithEvents NumericTracks1 As NumericUpDown
        Friend WithEvents NumericTracks2 As NumericUpDown
        Friend WithEvents NumericTracks0 As NumericUpDown
        Friend WithEvents LabelTracks2 As Label
        Friend WithEvents ComboPorts As ComboBox
        Friend WithEvents NumericDefaultRevs As NumericUpDown
        Friend WithEvents TextBoxLogFile As TextBox
    End Class
End Namespace