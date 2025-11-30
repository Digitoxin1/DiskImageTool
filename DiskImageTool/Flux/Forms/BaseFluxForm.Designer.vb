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
            Me.PanelBottom = New System.Windows.Forms.Panel()
            Me.PanelBottomInner = New System.Windows.Forms.TableLayoutPanel()
            Me.PanelButtonsRight = New System.Windows.Forms.FlowLayoutPanel()
            Me.ButtonCancel = New System.Windows.Forms.Button()
            Me.ButtonOk = New System.Windows.Forms.Button()
            Me.PanelButtonsLeft = New System.Windows.Forms.FlowLayoutPanel()
            Me.ButtonSaveLog = New System.Windows.Forms.Button()
            Me.PanelMain = New System.Windows.Forms.Panel()
            Me.PanelInner = New System.Windows.Forms.Panel()
            Me.TableLayoutPanelMain = New System.Windows.Forms.TableLayoutPanel()
            Me.LabelConsoleOutput = New System.Windows.Forms.Label()
            Me.TextBoxConsole = New System.Windows.Forms.TextBox()
            Me.StatusStripBottom = New System.Windows.Forms.StatusStrip()
            Me.StatusDevice = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusTrack = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusSide = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusType = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusBadSectors = New System.Windows.Forms.ToolStripStatusLabel()
            Me.StatusUnexpected = New System.Windows.Forms.ToolStripStatusLabel()
            Me.PanelBottom.SuspendLayout()
            Me.PanelBottomInner.SuspendLayout()
            Me.PanelButtonsRight.SuspendLayout()
            Me.PanelButtonsLeft.SuspendLayout()
            Me.PanelMain.SuspendLayout()
            Me.PanelInner.SuspendLayout()
            Me.StatusStripBottom.SuspendLayout()
            Me.SuspendLayout()
            '
            'PanelBottom
            '
            Me.PanelBottom.AutoSize = True
            Me.PanelBottom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.PanelBottom.Controls.Add(Me.PanelBottomInner)
            Me.PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.PanelBottom.Location = New System.Drawing.Point(0, 419)
            Me.PanelBottom.Name = "PanelBottom"
            Me.PanelBottom.Size = New System.Drawing.Size(767, 43)
            Me.PanelBottom.TabIndex = 1
            '
            'PanelBottomInner
            '
            Me.PanelBottomInner.AutoSize = True
            Me.PanelBottomInner.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.PanelBottomInner.ColumnCount = 3
            Me.PanelBottomInner.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.PanelBottomInner.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.PanelBottomInner.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.PanelBottomInner.Controls.Add(Me.PanelButtonsRight, 2, 0)
            Me.PanelBottomInner.Controls.Add(Me.PanelButtonsLeft, 1, 0)
            Me.PanelBottomInner.Dock = System.Windows.Forms.DockStyle.Fill
            Me.PanelBottomInner.Location = New System.Drawing.Point(0, 0)
            Me.PanelBottomInner.Name = "PanelBottomInner"
            Me.PanelBottomInner.RowCount = 1
            Me.PanelBottomInner.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.PanelBottomInner.Size = New System.Drawing.Size(767, 43)
            Me.PanelBottomInner.TabIndex = 0
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
            Me.ButtonCancel.Anchor = System.Windows.Forms.AnchorStyles.Left
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
            Me.ButtonOk.Anchor = System.Windows.Forms.AnchorStyles.Left
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
            Me.ButtonSaveLog.Anchor = System.Windows.Forms.AnchorStyles.Left
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
            Me.PanelMain.AutoSize = True
            Me.PanelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.PanelMain.Controls.Add(Me.PanelInner)
            Me.PanelMain.Controls.Add(Me.LabelConsoleOutput)
            Me.PanelMain.Controls.Add(Me.TextBoxConsole)
            Me.PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
            Me.PanelMain.Location = New System.Drawing.Point(0, 0)
            Me.PanelMain.Name = "PanelMain"
            Me.PanelMain.Padding = New System.Windows.Forms.Padding(18, 18, 18, 6)
            Me.PanelMain.Size = New System.Drawing.Size(767, 419)
            Me.PanelMain.TabIndex = 0
            '
            'PanelInner
            '
            Me.PanelInner.AutoSize = True
            Me.PanelInner.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.PanelInner.Controls.Add(Me.TableLayoutPanelMain)
            Me.PanelInner.Dock = System.Windows.Forms.DockStyle.Fill
            Me.PanelInner.Location = New System.Drawing.Point(18, 18)
            Me.PanelInner.Name = "PanelInner"
            Me.PanelInner.Size = New System.Drawing.Size(731, 259)
            Me.PanelInner.TabIndex = 0
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
            Me.StatusStripBottom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusDevice, Me.StatusTrack, Me.StatusSide, Me.StatusType, Me.StatusBadSectors, Me.StatusUnexpected})
            Me.StatusStripBottom.Location = New System.Drawing.Point(0, 462)
            Me.StatusStripBottom.Name = "StatusStripBottom"
            Me.StatusStripBottom.Size = New System.Drawing.Size(767, 24)
            Me.StatusStripBottom.SizingGrip = False
            Me.StatusStripBottom.TabIndex = 2
            '
            'StatusDevice
            '
            Me.StatusDevice.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
            Me.StatusDevice.Margin = New System.Windows.Forms.Padding(2, 3, 0, 2)
            Me.StatusDevice.Name = "StatusDevice"
            Me.StatusDevice.Size = New System.Drawing.Size(54, 19)
            Me.StatusDevice.Text = "{Device}"
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
            Me.StatusType.Size = New System.Drawing.Size(398, 19)
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
            Me.Controls.Add(Me.PanelMain)
            Me.Controls.Add(Me.PanelBottom)
            Me.Controls.Add(Me.StatusStripBottom)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.MinimumSize = New System.Drawing.Size(700, 39)
            Me.Name = "BaseFluxForm"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.PanelBottom.ResumeLayout(False)
            Me.PanelBottom.PerformLayout()
            Me.PanelBottomInner.ResumeLayout(False)
            Me.PanelBottomInner.PerformLayout()
            Me.PanelButtonsRight.ResumeLayout(False)
            Me.PanelButtonsRight.PerformLayout()
            Me.PanelButtonsLeft.ResumeLayout(False)
            Me.PanelButtonsLeft.PerformLayout()
            Me.PanelMain.ResumeLayout(False)
            Me.PanelMain.PerformLayout()
            Me.PanelInner.ResumeLayout(False)
            Me.PanelInner.PerformLayout()
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
        Friend WithEvents PanelBottom As Panel
        Friend WithEvents PanelBottomInner As TableLayoutPanel
        Friend WithEvents PanelMain As Panel
        Friend WithEvents PanelInner As Panel
        Friend WithEvents StatusDevice As ToolStripStatusLabel
    End Class
End Namespace