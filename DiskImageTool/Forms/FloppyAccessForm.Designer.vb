<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FloppyAccessForm
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
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.TableSide0 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableSide0Outer = New System.Windows.Forms.TableLayoutPanel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TableSide1Outer = New System.Windows.Forms.TableLayoutPanel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TableSide1 = New System.Windows.Forms.TableLayoutPanel()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.StatusType = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusTrack = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusSide = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusBadSectors = New System.Windows.Forms.ToolStripStatusLabel()
        Me.btnAbort = New System.Windows.Forms.Button()
        Me.TableSide0Outer.SuspendLayout()
        Me.TableSide1Outer.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'BackgroundWorker1
        '
        Me.BackgroundWorker1.WorkerReportsProgress = True
        Me.BackgroundWorker1.WorkerSupportsCancellation = True
        '
        'TableSide0
        '
        Me.TableSide0.AutoSize = True
        Me.TableSide0.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableSide0.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.[Single]
        Me.TableSide0.ColumnCount = 10
        Me.TableSide0Outer.SetColumnSpan(Me.TableSide0, 11)
        Me.TableSide0.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide0.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide0.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide0.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide0.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide0.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide0.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide0.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide0.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide0.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide0.Location = New System.Drawing.Point(17, 21)
        Me.TableSide0.Margin = New System.Windows.Forms.Padding(0)
        Me.TableSide0.Name = "TableSide0"
        Me.TableSide0.RowCount = 8
        Me.TableSide0Outer.SetRowSpan(Me.TableSide0, 9)
        Me.TableSide0.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide0.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide0.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide0.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide0.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide0.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide0.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide0.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide0.Size = New System.Drawing.Size(202, 177)
        Me.TableSide0.TabIndex = 1
        '
        'TableSide0Outer
        '
        Me.TableSide0Outer.AutoSize = True
        Me.TableSide0Outer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableSide0Outer.ColumnCount = 12
        Me.TableSide0Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 17.0!))
        Me.TableSide0Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide0Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide0Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide0Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide0Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide0Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide0Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide0Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide0Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide0Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide0Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableSide0Outer.Controls.Add(Me.TableSide0, 1, 1)
        Me.TableSide0Outer.Controls.Add(Me.Label2, 1, 10)
        Me.TableSide0Outer.Location = New System.Drawing.Point(24, 24)
        Me.TableSide0Outer.Name = "TableSide0Outer"
        Me.TableSide0Outer.RowCount = 11
        Me.TableSide0Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide0Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide0Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide0Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide0Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide0Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide0Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide0Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide0Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide0Outer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableSide0Outer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableSide0Outer.Size = New System.Drawing.Size(219, 221)
        Me.TableSide0Outer.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.TableSide0Outer.SetColumnSpan(Me.Label2, 11)
        Me.Label2.Location = New System.Drawing.Point(20, 208)
        Me.Label2.Margin = New System.Windows.Forms.Padding(3, 10, 3, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(196, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Side 0"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TableSide1Outer
        '
        Me.TableSide1Outer.AutoSize = True
        Me.TableSide1Outer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableSide1Outer.ColumnCount = 12
        Me.TableSide1Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 17.0!))
        Me.TableSide1Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide1Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide1Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide1Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide1Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide1Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide1Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide1Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide1Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide1Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide1Outer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableSide1Outer.Controls.Add(Me.Label3, 1, 10)
        Me.TableSide1Outer.Controls.Add(Me.TableSide1, 1, 1)
        Me.TableSide1Outer.Location = New System.Drawing.Point(266, 24)
        Me.TableSide1Outer.Name = "TableSide1Outer"
        Me.TableSide1Outer.RowCount = 11
        Me.TableSide1Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide1Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide1Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide1Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide1Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide1Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide1Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide1Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide1Outer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22.0!))
        Me.TableSide1Outer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableSide1Outer.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableSide1Outer.Size = New System.Drawing.Size(219, 221)
        Me.TableSide1Outer.TabIndex = 4
        '
        'Label3
        '
        Me.Label3.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.AutoSize = True
        Me.TableSide1Outer.SetColumnSpan(Me.Label3, 11)
        Me.Label3.Location = New System.Drawing.Point(20, 208)
        Me.Label3.Margin = New System.Windows.Forms.Padding(3, 10, 3, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(196, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Side 1"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TableSide1
        '
        Me.TableSide1.AutoSize = True
        Me.TableSide1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableSide1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.[Single]
        Me.TableSide1.ColumnCount = 10
        Me.TableSide1Outer.SetColumnSpan(Me.TableSide1, 11)
        Me.TableSide1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19.0!))
        Me.TableSide1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableSide1.Location = New System.Drawing.Point(17, 21)
        Me.TableSide1.Margin = New System.Windows.Forms.Padding(0)
        Me.TableSide1.Name = "TableSide1"
        Me.TableSide1.RowCount = 8
        Me.TableSide1Outer.SetRowSpan(Me.TableSide1, 9)
        Me.TableSide1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21.0!))
        Me.TableSide1.Size = New System.Drawing.Size(202, 177)
        Me.TableSide1.TabIndex = 1
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusType, Me.StatusTrack, Me.StatusSide, Me.StatusBadSectors})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 283)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(514, 22)
        Me.StatusStrip1.SizingGrip = False
        Me.StatusStrip1.TabIndex = 5
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'StatusType
        '
        Me.StatusType.AutoSize = False
        Me.StatusType.Name = "StatusType"
        Me.StatusType.Size = New System.Drawing.Size(66, 17)
        Me.StatusType.Text = "Reading"
        Me.StatusType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'StatusTrack
        '
        Me.StatusTrack.Name = "StatusTrack"
        Me.StatusTrack.Size = New System.Drawing.Size(43, 17)
        Me.StatusTrack.Text = "Track 0"
        Me.StatusTrack.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'StatusSide
        '
        Me.StatusSide.Name = "StatusSide"
        Me.StatusSide.Size = New System.Drawing.Size(288, 17)
        Me.StatusSide.Spring = True
        Me.StatusSide.Text = "Side 0"
        Me.StatusSide.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'StatusBadSectors
        '
        Me.StatusBadSectors.Name = "StatusBadSectors"
        Me.StatusBadSectors.Size = New System.Drawing.Size(77, 17)
        Me.StatusBadSectors.Text = "0 Bad Sectors"
        Me.StatusBadSectors.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnAbort
        '
        Me.btnAbort.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnAbort.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnAbort.Location = New System.Drawing.Point(225, 251)
        Me.btnAbort.Name = "btnAbort"
        Me.btnAbort.Size = New System.Drawing.Size(75, 23)
        Me.btnAbort.TabIndex = 6
        Me.btnAbort.Text = "Abort"
        Me.btnAbort.UseVisualStyleBackColor = True
        '
        'FloppyAccessForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnAbort
        Me.ClientSize = New System.Drawing.Size(514, 305)
        Me.Controls.Add(Me.btnAbort)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.TableSide1Outer)
        Me.Controls.Add(Me.TableSide0Outer)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FloppyAccessForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "FloppyAccessForm"
        Me.TableSide0Outer.ResumeLayout(False)
        Me.TableSide0Outer.PerformLayout()
        Me.TableSide1Outer.ResumeLayout(False)
        Me.TableSide1Outer.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents TableSide0 As TableLayoutPanel
    Friend WithEvents TableSide0Outer As TableLayoutPanel
    Friend WithEvents TableSide1Outer As TableLayoutPanel
    Friend WithEvents TableSide1 As TableLayoutPanel
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents StatusType As ToolStripStatusLabel
    Friend WithEvents StatusTrack As ToolStripStatusLabel
    Friend WithEvents StatusSide As ToolStripStatusLabel
    Friend WithEvents StatusBadSectors As ToolStripStatusLabel
    Friend WithEvents btnAbort As Button
End Class
