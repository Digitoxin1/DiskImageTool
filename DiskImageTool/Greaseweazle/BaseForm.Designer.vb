Namespace Greaseweazle
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class BaseForm
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
            Dim Label3 As System.Windows.Forms.Label
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BaseForm))
            Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
            Dim PanelMain As System.Windows.Forms.Panel
            Dim PanelInner As System.Windows.Forms.Panel
            Me.ButtonCancel = New System.Windows.Forms.Button()
            Me.ButtonOk = New System.Windows.Forms.Button()
            Me.TableLayoutPanelMain = New System.Windows.Forms.TableLayoutPanel()
            Me.TextBoxConsole = New System.Windows.Forms.TextBox()
            Me.StatusStripBottom = New System.Windows.Forms.StatusStrip()
            Me.StatusType = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusTrack = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusSide = New System.Windows.Forms.ToolStripStatusLabel()
            Label3 = New System.Windows.Forms.Label()
            PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
            PanelMain = New System.Windows.Forms.Panel()
            PanelInner = New System.Windows.Forms.Panel()
            PanelBottom.SuspendLayout()
            PanelMain.SuspendLayout()
            PanelInner.SuspendLayout()
            Me.StatusStripBottom.SuspendLayout()
            Me.SuspendLayout()
            '
            'Label3
            '
            resources.ApplyResources(Label3, "Label3")
            Label3.Name = "Label3"
            '
            'PanelBottom
            '
            PanelBottom.Controls.Add(Me.ButtonCancel)
            PanelBottom.Controls.Add(Me.ButtonOk)
            resources.ApplyResources(PanelBottom, "PanelBottom")
            PanelBottom.Name = "PanelBottom"
            '
            'ButtonCancel
            '
            Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            resources.ApplyResources(Me.ButtonCancel, "ButtonCancel")
            Me.ButtonCancel.Name = "ButtonCancel"
            Me.ButtonCancel.UseVisualStyleBackColor = True
            '
            'ButtonOk
            '
            Me.ButtonOk.DialogResult = System.Windows.Forms.DialogResult.OK
            resources.ApplyResources(Me.ButtonOk, "ButtonOk")
            Me.ButtonOk.Name = "ButtonOk"
            Me.ButtonOk.UseVisualStyleBackColor = True
            '
            'PanelMain
            '
            resources.ApplyResources(PanelMain, "PanelMain")
            PanelMain.Controls.Add(PanelInner)
            PanelMain.Controls.Add(Label3)
            PanelMain.Controls.Add(Me.TextBoxConsole)
            PanelMain.Name = "PanelMain"
            '
            'PanelInner
            '
            resources.ApplyResources(PanelInner, "PanelInner")
            PanelInner.Controls.Add(Me.TableLayoutPanelMain)
            PanelInner.Name = "PanelInner"
            '
            'TableLayoutPanelMain
            '
            resources.ApplyResources(Me.TableLayoutPanelMain, "TableLayoutPanelMain")
            Me.TableLayoutPanelMain.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize
            Me.TableLayoutPanelMain.Name = "TableLayoutPanelMain"
            '
            'TextBoxConsole
            '
            Me.TextBoxConsole.BackColor = System.Drawing.SystemColors.Control
            Me.TextBoxConsole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            resources.ApplyResources(Me.TextBoxConsole, "TextBoxConsole")
            Me.TextBoxConsole.Name = "TextBoxConsole"
            Me.TextBoxConsole.ReadOnly = True
            '
            'StatusStripBottom
            '
            Me.StatusStripBottom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusTrack, Me.StatusSide, Me.StatusType})
            resources.ApplyResources(Me.StatusStripBottom, "StatusStripBottom")
            Me.StatusStripBottom.Name = "StatusStripBottom"
            Me.StatusStripBottom.SizingGrip = False
            '
            'StatusType
            '
            resources.ApplyResources(Me.StatusType, "StatusType")
            Me.StatusType.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
            Me.StatusType.Name = "StatusType"
            Me.StatusType.Spring = True
            '
            'StatusTrack
            '
            Me.StatusTrack.Margin = New System.Windows.Forms.Padding(2, 3, 0, 2)
            Me.StatusTrack.Name = "StatusTrack"
            resources.ApplyResources(Me.StatusTrack, "StatusTrack")
            '
            'StatusSide
            '
            Me.StatusSide.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
            Me.StatusSide.Margin = New System.Windows.Forms.Padding(0, 3, 2, 2)
            Me.StatusSide.Name = "StatusSide"
            resources.ApplyResources(Me.StatusSide, "StatusSide")
            '
            'BaseForm
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(PanelMain)
            Me.Controls.Add(PanelBottom)
            Me.Controls.Add(Me.StatusStripBottom)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "BaseForm"
            Me.ShowInTaskbar = False
            PanelBottom.ResumeLayout(False)
            PanelMain.ResumeLayout(False)
            PanelMain.PerformLayout()
            PanelInner.ResumeLayout(False)
            PanelInner.PerformLayout()
            Me.StatusStripBottom.ResumeLayout(False)
            Me.StatusStripBottom.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents StatusStripBottom As StatusStrip
        Friend WithEvents TableLayoutPanelMain As TableLayoutPanel
        Friend WithEvents TextBoxConsole As TextBox
        Friend WithEvents ButtonOk As Button
        Friend WithEvents ButtonCancel As Button
        Friend WithEvents StatusType As ToolStripStatusLabel
        Friend WithEvents StatusTrack As ToolStripStatusLabel
        Friend WithEvents StatusSide As ToolStripStatusLabel
    End Class
End Namespace