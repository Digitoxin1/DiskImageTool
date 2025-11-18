Namespace Flux
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
            Dim PanelButtonsRight As System.Windows.Forms.FlowLayoutPanel
            Dim PanelBottom As System.Windows.Forms.Panel
            Dim PanelBottomInner As System.Windows.Forms.TableLayoutPanel
            Dim PanelMain As System.Windows.Forms.Panel
            Dim PanelInner As System.Windows.Forms.Panel
            Me.ButtonCancel = New System.Windows.Forms.Button()
            Me.ButtonOk = New System.Windows.Forms.Button()
            Me.PanelButtonsLeft = New System.Windows.Forms.FlowLayoutPanel()
            Me.ButtonSaveLog = New System.Windows.Forms.Button()
            Me.TableLayoutPanelMain = New System.Windows.Forms.TableLayoutPanel()
            Me.TextBoxConsole = New System.Windows.Forms.TextBox()
            Me.StatusStripBottom = New System.Windows.Forms.StatusStrip()
            Me.StatusTrack = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusSide = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusType = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusBadSectors = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusUnexpected = New System.Windows.Forms.ToolStripStatusLabel()
            Label3 = New System.Windows.Forms.Label()
            PanelButtonsRight = New System.Windows.Forms.FlowLayoutPanel()
            PanelBottom = New System.Windows.Forms.Panel()
            PanelBottomInner = New System.Windows.Forms.TableLayoutPanel()
            PanelMain = New System.Windows.Forms.Panel()
            PanelInner = New System.Windows.Forms.Panel()
            PanelButtonsRight.SuspendLayout()
            PanelBottom.SuspendLayout()
            PanelBottomInner.SuspendLayout()
            Me.PanelButtonsLeft.SuspendLayout()
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
            'PanelButtonsRight
            '
            resources.ApplyResources(PanelButtonsRight, "PanelButtonsRight")
            PanelButtonsRight.Controls.Add(Me.ButtonCancel)
            PanelButtonsRight.Controls.Add(Me.ButtonOk)
            PanelButtonsRight.Name = "PanelButtonsRight"
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
            'PanelBottom
            '
            resources.ApplyResources(PanelBottom, "PanelBottom")
            PanelBottom.Controls.Add(PanelBottomInner)
            PanelBottom.Name = "PanelBottom"
            '
            'PanelBottomInner
            '
            resources.ApplyResources(PanelBottomInner, "PanelBottomInner")
            PanelBottomInner.Controls.Add(PanelButtonsRight, 2, 0)
            PanelBottomInner.Controls.Add(Me.PanelButtonsLeft, 1, 0)
            PanelBottomInner.Name = "PanelBottomInner"
            '
            'PanelButtonsLeft
            '
            resources.ApplyResources(Me.PanelButtonsLeft, "PanelButtonsLeft")
            Me.PanelButtonsLeft.Controls.Add(Me.ButtonSaveLog)
            Me.PanelButtonsLeft.Name = "PanelButtonsLeft"
            '
            'ButtonSaveLog
            '
            resources.ApplyResources(Me.ButtonSaveLog, "ButtonSaveLog")
            Me.ButtonSaveLog.Name = "ButtonSaveLog"
            Me.ButtonSaveLog.UseVisualStyleBackColor = True
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
            Me.StatusStripBottom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusTrack, Me.StatusSide, Me.StatusType, Me.StatusBadSectors, Me.StatusUnexpected})
            resources.ApplyResources(Me.StatusStripBottom, "StatusStripBottom")
            Me.StatusStripBottom.Name = "StatusStripBottom"
            Me.StatusStripBottom.SizingGrip = False
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
            'StatusType
            '
            resources.ApplyResources(Me.StatusType, "StatusType")
            Me.StatusType.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
            Me.StatusType.Name = "StatusType"
            Me.StatusType.Spring = True
            '
            'StatusBadSectors
            '
            Me.StatusBadSectors.Margin = New System.Windows.Forms.Padding(6, 3, 0, 2)
            Me.StatusBadSectors.Name = "StatusBadSectors"
            resources.ApplyResources(Me.StatusBadSectors, "StatusBadSectors")
            '
            'StatusUnexpected
            '
            Me.StatusUnexpected.Margin = New System.Windows.Forms.Padding(6, 3, 0, 2)
            Me.StatusUnexpected.Name = "StatusUnexpected"
            resources.ApplyResources(Me.StatusUnexpected, "StatusUnexpected")
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
            PanelButtonsRight.ResumeLayout(False)
            PanelBottom.ResumeLayout(False)
            PanelBottom.PerformLayout()
            PanelBottomInner.ResumeLayout(False)
            PanelBottomInner.PerformLayout()
            Me.PanelButtonsLeft.ResumeLayout(False)
            Me.PanelButtonsLeft.PerformLayout()
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
        Friend WithEvents StatusBadSectors As ToolStripStatusLabel
        Friend WithEvents StatusUnexpected As ToolStripStatusLabel
        Friend WithEvents ButtonSaveLog As Button
        Friend WithEvents PanelButtonsLeft As FlowLayoutPanel
    End Class
End Namespace