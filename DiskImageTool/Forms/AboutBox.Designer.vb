<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class AboutBox
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
    Friend WithEvents LabelProductName As System.Windows.Forms.Label
    Friend WithEvents TextBoxDescription As System.Windows.Forms.TextBox
    Friend WithEvents OKButton As System.Windows.Forms.Button

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
        Dim PanelMain As System.Windows.Forms.Panel
        Dim TableLayoutPanel As System.Windows.Forms.TableLayoutPanel
        Dim LogoPictureBox As System.Windows.Forms.PictureBox
        Dim FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
        Dim FlowLayoutPanel2 As System.Windows.Forms.FlowLayoutPanel
        Me.OKButton = New System.Windows.Forms.Button()
        Me.TextBoxDescription = New System.Windows.Forms.TextBox()
        Me.LabelProductName = New System.Windows.Forms.Label()
        Me.LabelVersion = New System.Windows.Forms.Label()
        Me.LabelURL = New System.Windows.Forms.LinkLabel()
        Me.LabelDB = New System.Windows.Forms.Label()
        Me.LabelVersionCaption = New System.Windows.Forms.Label()
        Me.LabelDBVersion = New System.Windows.Forms.LinkLabel()
        PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        PanelMain = New System.Windows.Forms.Panel()
        TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        LogoPictureBox = New System.Windows.Forms.PictureBox()
        FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        PanelBottom.SuspendLayout()
        PanelMain.SuspendLayout()
        TableLayoutPanel.SuspendLayout()
        CType(LogoPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        FlowLayoutPanel1.SuspendLayout()
        FlowLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelBottom
        '
        PanelBottom.Controls.Add(Me.OKButton)
        PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        PanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        PanelBottom.Location = New System.Drawing.Point(0, 270)
        PanelBottom.Name = "PanelBottom"
        PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
        PanelBottom.Size = New System.Drawing.Size(426, 43)
        PanelBottom.TabIndex = 1
        PanelBottom.WrapContents = False
        '
        'OKButton
        '
        Me.OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.OKButton.Location = New System.Drawing.Point(333, 10)
        Me.OKButton.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.OKButton.Name = "OKButton"
        Me.OKButton.Size = New System.Drawing.Size(75, 23)
        Me.OKButton.TabIndex = 0
        Me.OKButton.Text = "&Ok"
        '
        'PanelMain
        '
        PanelMain.Controls.Add(TableLayoutPanel)
        PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        PanelMain.Location = New System.Drawing.Point(0, 0)
        PanelMain.Name = "PanelMain"
        PanelMain.Padding = New System.Windows.Forms.Padding(16, 16, 16, 6)
        PanelMain.Size = New System.Drawing.Size(426, 270)
        PanelMain.TabIndex = 0
        '
        'TableLayoutPanel
        '
        TableLayoutPanel.ColumnCount = 2
        TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        TableLayoutPanel.Controls.Add(LogoPictureBox, 0, 0)
        TableLayoutPanel.Controls.Add(Me.TextBoxDescription, 0, 4)
        TableLayoutPanel.Controls.Add(Me.LabelProductName, 1, 0)
        TableLayoutPanel.Controls.Add(Me.LabelURL, 1, 3)
        TableLayoutPanel.Controls.Add(FlowLayoutPanel1, 1, 2)
        TableLayoutPanel.Controls.Add(FlowLayoutPanel2, 1, 1)
        TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        TableLayoutPanel.Location = New System.Drawing.Point(16, 16)
        TableLayoutPanel.Name = "TableLayoutPanel"
        TableLayoutPanel.RowCount = 5
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60.0!))
        TableLayoutPanel.Size = New System.Drawing.Size(394, 248)
        TableLayoutPanel.TabIndex = 0
        '
        'LogoPictureBox
        '
        LogoPictureBox.ErrorImage = Nothing
        LogoPictureBox.Image = Global.DiskImageTool.My.Resources.Resources.AppIconLarge
        LogoPictureBox.InitialImage = Nothing
        LogoPictureBox.Location = New System.Drawing.Point(3, 3)
        LogoPictureBox.Margin = New System.Windows.Forms.Padding(3, 3, 12, 3)
        LogoPictureBox.Name = "LogoPictureBox"
        TableLayoutPanel.SetRowSpan(LogoPictureBox, 4)
        LogoPictureBox.Size = New System.Drawing.Size(64, 64)
        LogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        LogoPictureBox.TabIndex = 0
        LogoPictureBox.TabStop = False
        '
        'TextBoxDescription
        '
        TableLayoutPanel.SetColumnSpan(Me.TextBoxDescription, 2)
        Me.TextBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBoxDescription.Location = New System.Drawing.Point(6, 99)
        Me.TextBoxDescription.Margin = New System.Windows.Forms.Padding(6, 3, 3, 3)
        Me.TextBoxDescription.Multiline = True
        Me.TextBoxDescription.Name = "TextBoxDescription"
        Me.TextBoxDescription.ReadOnly = True
        Me.TextBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TextBoxDescription.Size = New System.Drawing.Size(385, 146)
        Me.TextBoxDescription.TabIndex = 4
        Me.TextBoxDescription.TabStop = False
        '
        'LabelProductName
        '
        Me.LabelProductName.AutoSize = True
        Me.LabelProductName.Location = New System.Drawing.Point(85, 0)
        Me.LabelProductName.Margin = New System.Windows.Forms.Padding(6, 0, 3, 0)
        Me.LabelProductName.Name = "LabelProductName"
        Me.LabelProductName.Size = New System.Drawing.Size(80, 13)
        Me.LabelProductName.TabIndex = 0
        Me.LabelProductName.Text = "{ProductName}"
        Me.LabelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LabelVersion
        '
        Me.LabelVersion.AutoSize = True
        Me.LabelVersion.Location = New System.Drawing.Point(60, 0)
        Me.LabelVersion.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.LabelVersion.Name = "LabelVersion"
        Me.LabelVersion.Size = New System.Drawing.Size(50, 13)
        Me.LabelVersion.TabIndex = 1
        Me.LabelVersion.Text = "{Version}"
        Me.LabelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LabelURL
        '
        Me.LabelURL.AutoSize = True
        Me.LabelURL.Location = New System.Drawing.Point(85, 72)
        Me.LabelURL.Margin = New System.Windows.Forms.Padding(6, 0, 3, 0)
        Me.LabelURL.Name = "LabelURL"
        Me.LabelURL.Size = New System.Drawing.Size(73, 13)
        Me.LabelURL.TabIndex = 3
        Me.LabelURL.TabStop = True
        Me.LabelURL.Text = "{ProjectPage}"
        Me.LabelURL.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'FlowLayoutPanel1
        '
        FlowLayoutPanel1.Controls.Add(Me.LabelDB)
        FlowLayoutPanel1.Controls.Add(Me.LabelDBVersion)
        FlowLayoutPanel1.Location = New System.Drawing.Point(79, 48)
        FlowLayoutPanel1.Margin = New System.Windows.Forms.Padding(0)
        FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        FlowLayoutPanel1.Size = New System.Drawing.Size(128, 13)
        FlowLayoutPanel1.TabIndex = 2
        FlowLayoutPanel1.WrapContents = False
        '
        'LabelDB
        '
        Me.LabelDB.AutoSize = True
        Me.LabelDB.Location = New System.Drawing.Point(6, 0)
        Me.LabelDB.Margin = New System.Windows.Forms.Padding(6, 0, 0, 0)
        Me.LabelDB.Name = "LabelDB"
        Me.LabelDB.Size = New System.Drawing.Size(54, 13)
        Me.LabelDB.TabIndex = 0
        Me.LabelDB.Text = "{Caption:}"
        Me.LabelDB.UseMnemonic = False
        '
        'FlowLayoutPanel2
        '
        FlowLayoutPanel2.AutoSize = True
        FlowLayoutPanel2.Controls.Add(Me.LabelVersionCaption)
        FlowLayoutPanel2.Controls.Add(Me.LabelVersion)
        FlowLayoutPanel2.Location = New System.Drawing.Point(79, 24)
        FlowLayoutPanel2.Margin = New System.Windows.Forms.Padding(0)
        FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        FlowLayoutPanel2.Size = New System.Drawing.Size(113, 13)
        FlowLayoutPanel2.TabIndex = 1
        FlowLayoutPanel2.WrapContents = False
        '
        'LabelVersionCaption
        '
        Me.LabelVersionCaption.AutoSize = True
        Me.LabelVersionCaption.Location = New System.Drawing.Point(6, 0)
        Me.LabelVersionCaption.Margin = New System.Windows.Forms.Padding(6, 0, 0, 0)
        Me.LabelVersionCaption.Name = "LabelVersionCaption"
        Me.LabelVersionCaption.Size = New System.Drawing.Size(54, 13)
        Me.LabelVersionCaption.TabIndex = 0
        Me.LabelVersionCaption.Text = "{Caption:}"
        '
        'LabelDBVersion
        '
        Me.LabelDBVersion.AutoSize = True
        Me.LabelDBVersion.Location = New System.Drawing.Point(60, 0)
        Me.LabelDBVersion.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.LabelDBVersion.Name = "LabelDBVersion"
        Me.LabelDBVersion.Size = New System.Drawing.Size(61, 13)
        Me.LabelDBVersion.TabIndex = 1
        Me.LabelDBVersion.TabStop = True
        Me.LabelDBVersion.Text = "{Database}"
        Me.LabelDBVersion.UseMnemonic = False
        '
        'AboutBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.OKButton
        Me.ClientSize = New System.Drawing.Size(426, 313)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AboutBox"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        PanelBottom.ResumeLayout(False)
        PanelMain.ResumeLayout(False)
        TableLayoutPanel.ResumeLayout(False)
        TableLayoutPanel.PerformLayout()
        CType(LogoPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        FlowLayoutPanel1.ResumeLayout(False)
        FlowLayoutPanel1.PerformLayout()
        FlowLayoutPanel2.ResumeLayout(False)
        FlowLayoutPanel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LabelVersion As Label
    Friend WithEvents LabelURL As LinkLabel
    Friend WithEvents LabelDB As Label
    Friend WithEvents LabelVersionCaption As Label
    Friend WithEvents LabelDBVersion As LinkLabel
End Class
