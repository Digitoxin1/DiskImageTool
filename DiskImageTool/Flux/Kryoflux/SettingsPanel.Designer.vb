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
            Dim LabelApplicationPath As System.Windows.Forms.Label
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SettingsPanel))
            Dim LabelLogFile As System.Windows.Forms.Label
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.TextBoxPath = New System.Windows.Forms.TextBox()
            Me.ButtonBrowse = New System.Windows.Forms.Button()
            Me.ButtonClear = New System.Windows.Forms.Button()
            Me.TextBoxLogFile = New System.Windows.Forms.TextBox()
            Me.CheckBoxStripPath = New System.Windows.Forms.CheckBox()
            LabelApplicationPath = New System.Windows.Forms.Label()
            LabelLogFile = New System.Windows.Forms.Label()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'LabelApplicationPath
            '
            resources.ApplyResources(LabelApplicationPath, "LabelApplicationPath")
            LabelApplicationPath.Name = "LabelApplicationPath"
            '
            'LabelLogFile
            '
            resources.ApplyResources(LabelLogFile, "LabelLogFile")
            LabelLogFile.Name = "LabelLogFile"
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(LabelApplicationPath, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBoxPath, 1, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.ButtonBrowse, 5, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.ButtonClear, 6, 0)
            Me.TableLayoutPanel1.Controls.Add(LabelLogFile, 0, 6)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBoxLogFile, 1, 6)
            Me.TableLayoutPanel1.Controls.Add(Me.CheckBoxStripPath, 2, 6)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
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
            'TextBoxLogFile
            '
            resources.ApplyResources(Me.TextBoxLogFile, "TextBoxLogFile")
            Me.TextBoxLogFile.Name = "TextBoxLogFile"
            '
            'CheckBoxStripPath
            '
            resources.ApplyResources(Me.CheckBoxStripPath, "CheckBoxStripPath")
            Me.TableLayoutPanel1.SetColumnSpan(Me.CheckBoxStripPath, 3)
            Me.CheckBoxStripPath.Name = "CheckBoxStripPath"
            Me.CheckBoxStripPath.UseVisualStyleBackColor = True
            '
            'SettingsPanel
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Name = "SettingsPanel"
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
    End Class
End Namespace
