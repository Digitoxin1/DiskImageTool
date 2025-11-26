Namespace Flux
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class BaseFluxForm
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
            Dim PanelBottom As System.Windows.Forms.Panel
            Dim PanelBottomInner As System.Windows.Forms.TableLayoutPanel
            Dim PanelMain As System.Windows.Forms.Panel
            Dim PanelInner As System.Windows.Forms.Panel
            Me.PanelButtonsRight = New System.Windows.Forms.FlowLayoutPanel()
            Me.ButtonCancel = New System.Windows.Forms.Button()
            Me.ButtonOk = New System.Windows.Forms.Button()
            Me.PanelButtonsLeft = New System.Windows.Forms.FlowLayoutPanel()
            Me.ButtonSaveLog = New System.Windows.Forms.Button()
            Me.TableLayoutPanelMain = New System.Windows.Forms.TableLayoutPanel()
            Me.LabelConsoleOutput = New System.Windows.Forms.Label()
            Me.TextBoxConsole = New System.Windows.Forms.TextBox()
            Me.StatusStripBottom = New System.Windows.Forms.StatusStrip()
            Me.StatusTrack = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusSide = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusType = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusBadSectors = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusUnexpected = New System.Windows.Forms.ToolStripStatusLabel()
            PanelBottom = New System.Windows.Forms.Panel()
            PanelBottomInner = New System.Windows.Forms.TableLayoutPanel()
            PanelMain = New System.Windows.Forms.Panel()
            PanelInner = New System.Windows.Forms.Panel()
            PanelBottom.SuspendLayout()
            PanelBottomInner.SuspendLayout()
            Me.PanelButtonsRight.SuspendLayout()
            Me.PanelButtonsLeft.SuspendLayout()
            PanelMain.SuspendLayout()
            PanelInner.SuspendLayout()
            Me.StatusStripBottom.SuspendLayout()
            Me.SuspendLayout()
            '
            'PanelBottom
            '
            PanelBottom.AutoSize = True
            PanelBottom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            PanelBottom.Controls.Add(PanelBottomInner)
            PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
            PanelBottom.Location = New System.Drawing.Point(0, 419)
            PanelBottom.Name = "PanelBottom"
            PanelBottom.Size = New System.Drawing.Size(767, 43)
            PanelBottom.TabIndex = 1
            '
            'PanelBottomInner
            '
            PanelBottomInner.AutoSize = True
            PanelBottomInner.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            PanelBottomInner.ColumnCount = 3
            PanelBottomInner.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            PanelBottomInner.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            PanelBottomInner.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            PanelBottomInner.Controls.Add(Me.PanelButtonsRight, 2, 0)
            PanelBottomInner.Controls.Add(Me.PanelButtonsLeft, 1, 0)
            PanelBottomInner.Dock = System.Windows.Forms.DockStyle.Fill
            PanelBottomInner.Location = New System.Drawing.Point(0, 0)
            PanelBottomInner.Name = "PanelBottomInner"
            PanelBottomInner.RowCount = 1
            PanelBottomInner.RowStyles.Add(New System.Windows.Forms.RowStyle())
            PanelBottomInner.Size = New System.Drawing.Size(767, 43)
            PanelBottomInner.TabIndex = 0
            '
            'PanelButtonsRight
            '
            Me.PanelButtonsRight.AutoSize = True
            Me.PanelButtonsRight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.PanelButtonsRight.Controls.Add(Me.ButtonCancel)
            Me.PanelButtonsRight.Controls.Add(Me.ButtonOk)
            Me.PanelButtonsRight.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
            Me.PanelButtonsRight.Location = New System.Drawing.Point(581, 0)
            Me.PanelButtonsRight.Margin = New System.Windows.Forms.Padding(0)
            Me.PanelButtonsRight.Name = "PanelButtonsRight"
            Me.PanelButtonsRight.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
            Me.PanelButtonsRight.Size = New System.Drawing.Size(186, 43)
            Me.PanelButtonsRight.TabIndex = 1
            Me.PanelButtonsRight.WrapContents = False
            '
            'ButtonCancel
            '
            Me.ButtonCancel.AutoSize = True
            Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.ButtonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.ButtonCancel.Location = New System.Drawing.Point(93, 10)
            Me.ButtonCancel.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
            Me.ButtonCancel.Name = "ButtonCancel"
            Me.ButtonCancel.Size = New System.Drawing.Size(75, 23)
            Me.ButtonCancel.TabIndex = 1
            Me.ButtonCancel.Text = "{&Cancel}"
            Me.ButtonCancel.UseVisualStyleBackColor = True
            '
            'ButtonOk
            '
            Me.ButtonOk.AutoSize = True
            Me.ButtonOk.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.ButtonOk.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.ButtonOk.Location = New System.Drawing.Point(6, 10)
            Me.ButtonOk.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
            Me.ButtonOk.Name = "ButtonOk"
            Me.ButtonOk.Size = New System.Drawing.Size(75, 23)
            Me.ButtonOk.TabIndex = 0
            Me.ButtonOk.Text = "{Ok}"
            Me.ButtonOk.UseVisualStyleBackColor = True
            '
            'PanelButtonsLeft
            '
            Me.PanelButtonsLeft.AutoSize = True
            Me.PanelButtonsLeft.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.PanelButtonsLeft.Controls.Add(Me.ButtonSaveLog)
            Me.PanelButtonsLeft.Location = New System.Drawing.Point(0, 0)
            Me.PanelButtonsLeft.Margin = New System.Windows.Forms.Padding(0)
            Me.PanelButtonsLeft.Name = "PanelButtonsLeft"
            Me.PanelButtonsLeft.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
            Me.PanelButtonsLeft.Size = New System.Drawing.Size(99, 43)
            Me.PanelButtonsLeft.TabIndex = 0
            Me.PanelButtonsLeft.WrapContents = False
            '
            'ButtonSaveLog
            '
            Me.ButtonSaveLog.AutoSize = True
            Me.ButtonSaveLog.Location = New System.Drawing.Point(12, 10)
            Me.ButtonSaveLog.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
            Me.ButtonSaveLog.MinimumSize = New System.Drawing.Size(75, 0)
            Me.ButtonSaveLog.Name = "ButtonSaveLog"
            Me.ButtonSaveLog.Size = New System.Drawing.Size(75, 23)
            Me.ButtonSaveLog.TabIndex = 0
            Me.ButtonSaveLog.Text = "{Save Log}"
            Me.ButtonSaveLog.UseVisualStyleBackColor = True
            '
            'PanelMain
            '
            PanelMain.AutoSize = True
            PanelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            PanelMain.Controls.Add(PanelInner)
            PanelMain.Controls.Add(Me.LabelConsoleOutput)
            PanelMain.Controls.Add(Me.TextBoxConsole)
            PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
            PanelMain.Location = New System.Drawing.Point(0, 0)
            PanelMain.Name = "PanelMain"
            PanelMain.Padding = New System.Windows.Forms.Padding(18, 18, 18, 6)
            PanelMain.Size = New System.Drawing.Size(767, 419)
            PanelMain.TabIndex = 0
            '
            'PanelInner
            '
            PanelInner.AutoSize = True
            PanelInner.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            PanelInner.Controls.Add(Me.TableLayoutPanelMain)
            PanelInner.Dock = System.Windows.Forms.DockStyle.Fill
            PanelInner.Location = New System.Drawing.Point(18, 18)
            PanelInner.Name = "PanelInner"
            PanelInner.Size = New System.Drawing.Size(731, 259)
            PanelInner.TabIndex = 0
            '
            'TableLayoutPanelMain
            '
            Me.TableLayoutPanelMain.Anchor = System.Windows.Forms.AnchorStyles.Top
            Me.TableLayoutPanelMain.AutoSize = True
            Me.TableLayoutPanelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.TableLayoutPanelMain.ColumnCount = 1
            Me.TableLayoutPanelMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.TableLayoutPanelMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.TableLayoutPanelMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.TableLayoutPanelMain.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize
            Me.TableLayoutPanelMain.Location = New System.Drawing.Point(355, 0)
            Me.TableLayoutPanelMain.Margin = New System.Windows.Forms.Padding(0)
            Me.TableLayoutPanelMain.Name = "TableLayoutPanelMain"
            Me.TableLayoutPanelMain.RowCount = 1
            Me.TableLayoutPanelMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.TableLayoutPanelMain.Size = New System.Drawing.Size(20, 20)
            Me.TableLayoutPanelMain.TabIndex = 0
            '
            'LabelConsoleOutput
            '
            Me.LabelConsoleOutput.AutoSize = True
            Me.LabelConsoleOutput.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.LabelConsoleOutput.Location = New System.Drawing.Point(18, 277)
            Me.LabelConsoleOutput.Margin = New System.Windows.Forms.Padding(3)
            Me.LabelConsoleOutput.Name = "LabelConsoleOutput"
            Me.LabelConsoleOutput.Padding = New System.Windows.Forms.Padding(0, 0, 0, 3)
            Me.LabelConsoleOutput.Size = New System.Drawing.Size(88, 16)
            Me.LabelConsoleOutput.TabIndex = 1
            Me.LabelConsoleOutput.Text = "{Console Output}"
            '
            'TextBoxConsole
            '
            Me.TextBoxConsole.BackColor = System.Drawing.SystemColors.Control
            Me.TextBoxConsole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.TextBoxConsole.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.TextBoxConsole.Font = New System.Drawing.Font("Consolas", 9.0!)
            Me.TextBoxConsole.Location = New System.Drawing.Point(18, 293)
            Me.TextBoxConsole.Multiline = True
            Me.TextBoxConsole.Name = "TextBoxConsole"
            Me.TextBoxConsole.ReadOnly = True
            Me.TextBoxConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.TextBoxConsole.Size = New System.Drawing.Size(731, 120)
            Me.TextBoxConsole.TabIndex = 2
            '
            'StatusStripBottom
            '
            Me.StatusStripBottom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusTrack, Me.StatusSide, Me.StatusType, Me.StatusBadSectors, Me.StatusUnexpected})
            Me.StatusStripBottom.Location = New System.Drawing.Point(0, 462)
            Me.StatusStripBottom.Name = "StatusStripBottom"
            Me.StatusStripBottom.Size = New System.Drawing.Size(767, 24)
            Me.StatusStripBottom.SizingGrip = False
            Me.StatusStripBottom.TabIndex = 2
            '
            'StatusTrack
            '
            Me.StatusTrack.Margin = New System.Windows.Forms.Padding(2, 3, 0, 2)
            Me.StatusTrack.Name = "StatusTrack"
            Me.StatusTrack.Size = New System.Drawing.Size(43, 19)
            Me.StatusTrack.Text = "{Track}"
            Me.StatusTrack.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'StatusSide
            '
            Me.StatusSide.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
            Me.StatusSide.Margin = New System.Windows.Forms.Padding(0, 3, 2, 2)
            Me.StatusSide.Name = "StatusSide"
            Me.StatusSide.Size = New System.Drawing.Size(41, 19)
            Me.StatusSide.Text = "{Side}"
            Me.StatusSide.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'StatusType
            '
            Me.StatusType.AutoSize = False
            Me.StatusType.Margin = New System.Windows.Forms.Padding(2, 3, 2, 2)
            Me.StatusType.Name = "StatusType"
            Me.StatusType.Size = New System.Drawing.Size(454, 19)
            Me.StatusType.Spring = True
            Me.StatusType.Text = "{Status}"
            Me.StatusType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'StatusBadSectors
            '
            Me.StatusBadSectors.Margin = New System.Windows.Forms.Padding(6, 3, 0, 2)
            Me.StatusBadSectors.Name = "StatusBadSectors"
            Me.StatusBadSectors.Size = New System.Drawing.Size(76, 19)
            Me.StatusBadSectors.Text = "{Bad Sectors}"
            Me.StatusBadSectors.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'StatusUnexpected
            '
            Me.StatusUnexpected.Margin = New System.Windows.Forms.Padding(6, 3, 0, 2)
            Me.StatusUnexpected.Name = "StatusUnexpected"
            Me.StatusUnexpected.Size = New System.Drawing.Size(118, 19)
            Me.StatusUnexpected.Text = "{Unexpected Sectors}"
            '
            'BaseFluxForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.AutoSize = True
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.ClientSize = New System.Drawing.Size(767, 486)
            Me.Controls.Add(PanelMain)
            Me.Controls.Add(PanelBottom)
            Me.Controls.Add(Me.StatusStripBottom)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.MinimumSize = New System.Drawing.Size(700, 39)
            Me.Name = "BaseFluxForm"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            PanelBottom.ResumeLayout(False)
            PanelBottom.PerformLayout()
            PanelBottomInner.ResumeLayout(False)
            PanelBottomInner.PerformLayout()
            Me.PanelButtonsRight.ResumeLayout(False)
            Me.PanelButtonsRight.PerformLayout()
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
        Friend WithEvents PanelButtonsRight As FlowLayoutPanel
        Friend WithEvents LabelConsoleOutput As Label
    End Class
End Namespace