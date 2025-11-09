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
        Me.TableSide1 = New System.Windows.Forms.TableLayoutPanel()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.StatusType = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusTrack = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusSide = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusBadSectors = New System.Windows.Forms.ToolStripStatusLabel()
        Me.btnAbort = New System.Windows.Forms.Button()
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
        Me.TableSide0.ColumnCount = 1
        Me.TableSide0.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableSide0.Location = New System.Drawing.Point(24, 24)
        Me.TableSide0.Name = "TableSide0"
        Me.TableSide0.RowCount = 1
        Me.TableSide0.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableSide0.Size = New System.Drawing.Size(219, 197)
        Me.TableSide0.TabIndex = 3
        '
        'TableSide1
        '
        Me.TableSide1.ColumnCount = 1
        Me.TableSide1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableSide1.Location = New System.Drawing.Point(266, 24)
        Me.TableSide1.Name = "TableSide1"
        Me.TableSide1.RowCount = 1
        Me.TableSide1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableSide1.Size = New System.Drawing.Size(219, 197)
        Me.TableSide1.TabIndex = 4
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
        Me.StatusType.Text = "{Status}"
        Me.StatusType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'StatusTrack
        '
        Me.StatusTrack.Name = "StatusTrack"
        Me.StatusTrack.Size = New System.Drawing.Size(43, 17)
        Me.StatusTrack.Text = "{Track}"
        Me.StatusTrack.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'StatusSide
        '
        Me.StatusSide.Name = "StatusSide"
        Me.StatusSide.Size = New System.Drawing.Size(314, 17)
        Me.StatusSide.Spring = True
        Me.StatusSide.Text = "{Side}"
        Me.StatusSide.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'StatusBadSectors
        '
        Me.StatusBadSectors.Name = "StatusBadSectors"
        Me.StatusBadSectors.Size = New System.Drawing.Size(76, 17)
        Me.StatusBadSectors.Text = "{Bad Sectors}"
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
        Me.ClientSize = New System.Drawing.Size(514, 305)
        Me.Controls.Add(Me.btnAbort)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.TableSide1)
        Me.Controls.Add(Me.TableSide0)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FloppyAccessForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "{Caption}"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents TableSide0 As TableLayoutPanel
    Friend WithEvents TableSide1 As TableLayoutPanel
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents StatusType As ToolStripStatusLabel
    Friend WithEvents StatusTrack As ToolStripStatusLabel
    Friend WithEvents StatusSide As ToolStripStatusLabel
    Friend WithEvents StatusBadSectors As ToolStripStatusLabel
    Friend WithEvents btnAbort As Button
End Class
