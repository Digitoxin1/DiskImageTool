<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ImagePreview
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
        Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
        Dim SplitContainer1 As System.Windows.Forms.SplitContainer
        Dim PanelMain As System.Windows.Forms.Panel
        Me.OKButton = New System.Windows.Forms.Button()
        Me.ListViewSummary = New System.Windows.Forms.ListView()
        Me.PanelSpacer = New System.Windows.Forms.Panel()
        Me.HashPanel1 = New DiskImageTool.HashPanel()
        Me.ListViewFiles = New DiskImageTool.ListViewEx()
        PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        SplitContainer1 = New System.Windows.Forms.SplitContainer()
        PanelMain = New System.Windows.Forms.Panel()
        PanelBottom.SuspendLayout()
        CType(SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer1.Panel1.SuspendLayout()
        SplitContainer1.Panel2.SuspendLayout()
        SplitContainer1.SuspendLayout()
        PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelBottom
        '
        PanelBottom.Controls.Add(Me.OKButton)
        PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        PanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        PanelBottom.Location = New System.Drawing.Point(0, 468)
        PanelBottom.Name = "PanelBottom"
        PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
        PanelBottom.Size = New System.Drawing.Size(924, 43)
        PanelBottom.TabIndex = 1
        PanelBottom.WrapContents = False
        '
        'OKButton
        '
        Me.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.OKButton.Location = New System.Drawing.Point(831, 10)
        Me.OKButton.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.OKButton.Name = "OKButton"
        Me.OKButton.Size = New System.Drawing.Size(75, 23)
        Me.OKButton.TabIndex = 0
        Me.OKButton.Text = "&Ok"
        '
        'SplitContainer1
        '
        SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        SplitContainer1.IsSplitterFixed = True
        SplitContainer1.Location = New System.Drawing.Point(16, 16)
        SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        SplitContainer1.Panel1.Controls.Add(Me.ListViewSummary)
        SplitContainer1.Panel1.Controls.Add(Me.PanelSpacer)
        SplitContainer1.Panel1.Controls.Add(Me.HashPanel1)
        '
        'SplitContainer1.Panel2
        '
        SplitContainer1.Panel2.Controls.Add(Me.ListViewFiles)
        SplitContainer1.Size = New System.Drawing.Size(892, 452)
        SplitContainer1.SplitterDistance = 310
        SplitContainer1.SplitterWidth = 6
        SplitContainer1.TabIndex = 0
        '
        'ListViewSummary
        '
        Me.ListViewSummary.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListViewSummary.HideSelection = False
        Me.ListViewSummary.Location = New System.Drawing.Point(0, 0)
        Me.ListViewSummary.Name = "ListViewSummary"
        Me.ListViewSummary.Size = New System.Drawing.Size(310, 345)
        Me.ListViewSummary.TabIndex = 0
        Me.ListViewSummary.UseCompatibleStateImageBehavior = False
        '
        'PanelSpacer
        '
        Me.PanelSpacer.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelSpacer.Location = New System.Drawing.Point(0, 345)
        Me.PanelSpacer.Name = "PanelSpacer"
        Me.PanelSpacer.Size = New System.Drawing.Size(310, 6)
        Me.PanelSpacer.TabIndex = 1
        '
        'PanelMain
        '
        PanelMain.Controls.Add(SplitContainer1)
        PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        PanelMain.Location = New System.Drawing.Point(0, 0)
        PanelMain.Name = "PanelMain"
        PanelMain.Padding = New System.Windows.Forms.Padding(16, 16, 16, 0)
        PanelMain.Size = New System.Drawing.Size(924, 468)
        PanelMain.TabIndex = 0
        '
        'HashPanel1
        '
        Me.HashPanel1.BackColor = System.Drawing.SystemColors.Window
        Me.HashPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.HashPanel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.HashPanel1.Location = New System.Drawing.Point(0, 351)
        Me.HashPanel1.Name = "HashPanel1"
        Me.HashPanel1.Padding = New System.Windows.Forms.Padding(3)
        Me.HashPanel1.Size = New System.Drawing.Size(310, 101)
        Me.HashPanel1.TabIndex = 2
        '
        'ListViewFiles
        '
        Me.ListViewFiles.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListViewFiles.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.ListViewFiles.HideSelection = False
        Me.ListViewFiles.Location = New System.Drawing.Point(0, 0)
        Me.ListViewFiles.Name = "ListViewFiles"
        Me.ListViewFiles.Size = New System.Drawing.Size(576, 452)
        Me.ListViewFiles.TabIndex = 0
        Me.ListViewFiles.UseCompatibleStateImageBehavior = False
        '
        'ImagePreview
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(924, 511)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(870, 550)
        Me.Name = "ImagePreview"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        PanelBottom.ResumeLayout(False)
        SplitContainer1.Panel1.ResumeLayout(False)
        SplitContainer1.Panel2.ResumeLayout(False)
        CType(SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        SplitContainer1.ResumeLayout(False)
        PanelMain.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents OKButton As Button
    Friend WithEvents ListViewSummary As ListView
    Friend WithEvents ListViewFiles As ListViewEx
    Friend WithEvents HashPanel1 As HashPanel
    Friend WithEvents PanelSpacer As Panel
End Class
