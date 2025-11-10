<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ImageImportForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim Label1 As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ImageImportForm))
        Dim Label2 As System.Windows.Forms.Label
        Dim Label3 As System.Windows.Forms.Label
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.ButtonProcess = New System.Windows.Forms.Button()
        Me.TableSide1 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableSide0 = New System.Windows.Forms.TableLayoutPanel()
        Me.ComboImageFormat = New System.Windows.Forms.ComboBox()
        Me.ComboOutputType = New System.Windows.Forms.ComboBox()
        Me.CheckBoxDoublestep = New System.Windows.Forms.CheckBox()
        Me.TextBoxConsole = New System.Windows.Forms.TextBox()
        Me.FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        Me.ButtonImport = New System.Windows.Forms.Button()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Label1 = New System.Windows.Forms.Label()
        Label2 = New System.Windows.Forms.Label()
        Label3 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.FlowLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        resources.ApplyResources(Label1, "Label1")
        Label1.Name = "Label1"
        '
        'Label2
        '
        resources.ApplyResources(Label2, "Label2")
        Label2.Name = "Label2"
        '
        'Label3
        '
        resources.ApplyResources(Label3, "Label3")
        Label3.Name = "Label3"
        '
        'StatusStrip1
        '
        resources.ApplyResources(Me.StatusStrip1, "StatusStrip1")
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.SizingGrip = False
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonProcess, 4, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.TableSide1, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.TableSide0, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Label1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ComboImageFormat, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Label2, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.ComboOutputType, 3, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.CheckBoxDoublestep, 4, 0)
        Me.TableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'ButtonProcess
        '
        resources.ApplyResources(Me.ButtonProcess, "ButtonProcess")
        Me.ButtonProcess.Name = "ButtonProcess"
        Me.ButtonProcess.UseVisualStyleBackColor = True
        '
        'TableSide1
        '
        resources.ApplyResources(Me.TableSide1, "TableSide1")
        Me.TableLayoutPanel1.SetColumnSpan(Me.TableSide1, 2)
        Me.TableSide1.Name = "TableSide1"
        '
        'TableSide0
        '
        resources.ApplyResources(Me.TableSide0, "TableSide0")
        Me.TableLayoutPanel1.SetColumnSpan(Me.TableSide0, 2)
        Me.TableSide0.Name = "TableSide0"
        '
        'ComboImageFormat
        '
        resources.ApplyResources(Me.ComboImageFormat, "ComboImageFormat")
        Me.ComboImageFormat.FormattingEnabled = True
        Me.ComboImageFormat.Name = "ComboImageFormat"
        '
        'ComboOutputType
        '
        resources.ApplyResources(Me.ComboOutputType, "ComboOutputType")
        Me.ComboOutputType.FormattingEnabled = True
        Me.ComboOutputType.Name = "ComboOutputType"
        '
        'CheckBoxDoublestep
        '
        resources.ApplyResources(Me.CheckBoxDoublestep, "CheckBoxDoublestep")
        Me.CheckBoxDoublestep.Name = "CheckBoxDoublestep"
        Me.CheckBoxDoublestep.UseVisualStyleBackColor = True
        '
        'TextBoxConsole
        '
        resources.ApplyResources(Me.TextBoxConsole, "TextBoxConsole")
        Me.TextBoxConsole.BackColor = System.Drawing.SystemColors.Control
        Me.TextBoxConsole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxConsole.Name = "TextBoxConsole"
        Me.TextBoxConsole.ReadOnly = True
        '
        'FlowLayoutPanel2
        '
        resources.ApplyResources(Me.FlowLayoutPanel2, "FlowLayoutPanel2")
        Me.FlowLayoutPanel2.Controls.Add(Me.ButtonImport)
        Me.FlowLayoutPanel2.Controls.Add(Me.ButtonCancel)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        '
        'ButtonImport
        '
        resources.ApplyResources(Me.ButtonImport, "ButtonImport")
        Me.ButtonImport.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.ButtonImport.Name = "ButtonImport"
        Me.ButtonImport.UseVisualStyleBackColor = True
        '
        'ButtonCancel
        '
        resources.ApplyResources(Me.ButtonCancel, "ButtonCancel")
        Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'ImageImportForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.FlowLayoutPanel2)
        Me.Controls.Add(Label3)
        Me.Controls.Add(Me.TextBoxConsole)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ImageImportForm"
        Me.ShowInTaskbar = False
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.FlowLayoutPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents ComboImageFormat As ComboBox
    Friend WithEvents ComboOutputType As ComboBox
    Friend WithEvents TextBoxConsole As TextBox
    Friend WithEvents CheckBoxDoublestep As CheckBox
    Friend WithEvents FlowLayoutPanel2 As FlowLayoutPanel
    Friend WithEvents ButtonImport As Button
    Friend WithEvents ButtonCancel As Button
    Friend WithEvents ButtonProcess As Button
    Friend WithEvents TableSide1 As TableLayoutPanel
    Friend WithEvents TableSide0 As TableLayoutPanel
End Class
