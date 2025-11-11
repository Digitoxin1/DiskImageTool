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
            Me.LabelDriveType0 = New System.Windows.Forms.Label()
            Me.LabelDriveType1 = New System.Windows.Forms.Label()
            Me.LabelDriveType2 = New System.Windows.Forms.Label()
            Me.FlowLayoutPanelBottom = New System.Windows.Forms.FlowLayoutPanel()
            Me.BtnUpdate = New System.Windows.Forms.Button()
            Me.BtnCancel = New System.Windows.Forms.Button()
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.ComboDriveType2 = New System.Windows.Forms.ComboBox()
            Me.ComboDriveType1 = New System.Windows.Forms.ComboBox()
            Me.ComboDriveType0 = New System.Windows.Forms.ComboBox()
            Me.ComboInterface = New System.Windows.Forms.ComboBox()
            Me.TextBoxPath = New System.Windows.Forms.TextBox()
            Me.ButtonBrowse = New System.Windows.Forms.Button()
            Me.ButtonClear = New System.Windows.Forms.Button()
            Me.TextBoxInfo = New System.Windows.Forms.TextBox()
            Me.ButtonInfo = New System.Windows.Forms.Button()
            LabelApplicationPath = New System.Windows.Forms.Label()
            LabelDriveInterface = New System.Windows.Forms.Label()
            Me.FlowLayoutPanelBottom.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
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
            'FlowLayoutPanelBottom
            '
            resources.ApplyResources(Me.FlowLayoutPanelBottom, "FlowLayoutPanelBottom")
            Me.FlowLayoutPanelBottom.Controls.Add(Me.BtnUpdate)
            Me.FlowLayoutPanelBottom.Controls.Add(Me.BtnCancel)
            Me.FlowLayoutPanelBottom.Name = "FlowLayoutPanelBottom"
            '
            'BtnUpdate
            '
            resources.ApplyResources(Me.BtnUpdate, "BtnUpdate")
            Me.BtnUpdate.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.BtnUpdate.Name = "BtnUpdate"
            Me.BtnUpdate.UseVisualStyleBackColor = True
            '
            'BtnCancel
            '
            resources.ApplyResources(Me.BtnCancel, "BtnCancel")
            Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.BtnCancel.Name = "BtnCancel"
            Me.BtnCancel.UseVisualStyleBackColor = True
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
            Me.TableLayoutPanel1.Controls.Add(Me.ButtonBrowse, 3, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.ButtonClear, 5, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBoxInfo, 2, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.ButtonInfo, 2, 1)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
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
            Me.TableLayoutPanel1.SetColumnSpan(Me.TextBoxPath, 3)
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
            Me.TableLayoutPanel1.SetColumnSpan(Me.TextBoxInfo, 4)
            Me.TextBoxInfo.Name = "TextBoxInfo"
            Me.TextBoxInfo.ReadOnly = True
            Me.TableLayoutPanel1.SetRowSpan(Me.TextBoxInfo, 4)
            Me.TextBoxInfo.ShortcutsEnabled = False
            '
            'ButtonInfo
            '
            resources.ApplyResources(Me.ButtonInfo, "ButtonInfo")
            Me.ButtonInfo.Name = "ButtonInfo"
            Me.ButtonInfo.UseVisualStyleBackColor = True
            '
            'GreaseweazleConfigurationForm
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Controls.Add(Me.FlowLayoutPanelBottom)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "GreaseweazleConfigurationForm"
            Me.ShowInTaskbar = False
            Me.FlowLayoutPanelBottom.ResumeLayout(False)
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents FlowLayoutPanelBottom As FlowLayoutPanel
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
        Friend WithEvents ButtonClear As Button
        Friend WithEvents ButtonInfo As Button
        Friend WithEvents TextBoxInfo As TextBox
    End Class
End Namespace