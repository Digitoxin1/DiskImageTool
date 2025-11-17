Namespace Flux.Kryoflux
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
            Dim PanelMain As System.Windows.Forms.Panel
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ConfigurationForm))
            Dim LabelApplicationPath As System.Windows.Forms.Label
            Dim LabelLogFile As System.Windows.Forms.Label
            Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.TextBoxPath = New System.Windows.Forms.TextBox()
            Me.ButtonBrowse = New System.Windows.Forms.Button()
            Me.ButtonClear = New System.Windows.Forms.Button()
            Me.TextBoxLogFile = New System.Windows.Forms.TextBox()
            Me.BtnCancel = New System.Windows.Forms.Button()
            Me.BtnUpdate = New System.Windows.Forms.Button()
            PanelMain = New System.Windows.Forms.Panel()
            LabelApplicationPath = New System.Windows.Forms.Label()
            LabelLogFile = New System.Windows.Forms.Label()
            PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
            PanelMain.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            PanelBottom.SuspendLayout()
            Me.SuspendLayout()
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
            Me.TableLayoutPanel1.Controls.Add(Me.TextBoxPath, 1, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.ButtonBrowse, 5, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.ButtonClear, 6, 0)
            Me.TableLayoutPanel1.Controls.Add(LabelLogFile, 0, 6)
            Me.TableLayoutPanel1.Controls.Add(Me.TextBoxLogFile, 1, 6)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'LabelApplicationPath
            '
            resources.ApplyResources(LabelApplicationPath, "LabelApplicationPath")
            LabelApplicationPath.Name = "LabelApplicationPath"
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
            PanelMain.ResumeLayout(False)
            PanelMain.PerformLayout()
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            PanelBottom.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
        Friend WithEvents TextBoxPath As TextBox
        Friend WithEvents ButtonBrowse As Button
        Friend WithEvents ButtonClear As Button
        Friend WithEvents TextBoxLogFile As TextBox
        Friend WithEvents BtnCancel As Button
        Friend WithEvents BtnUpdate As Button
    End Class
End Namespace