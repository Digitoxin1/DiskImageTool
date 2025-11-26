<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HexSearchForm
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
        Dim PanelMain As System.Windows.Forms.Panel
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.RadBtnText = New System.Windows.Forms.RadioButton()
        Me.RadBtnHex = New System.Windows.Forms.RadioButton()
        Me.ChkCaseSensitive = New System.Windows.Forms.CheckBox()
        Me.TextSearch = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        PanelMain = New System.Windows.Forms.Panel()
        PanelBottom.SuspendLayout()
        PanelMain.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelBottom
        '
        PanelBottom.Controls.Add(Me.BtnCancel)
        PanelBottom.Controls.Add(Me.BtnOK)
        PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        PanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        PanelBottom.Location = New System.Drawing.Point(0, 190)
        PanelBottom.Margin = New System.Windows.Forms.Padding(0)
        PanelBottom.Name = "PanelBottom"
        PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
        PanelBottom.Size = New System.Drawing.Size(496, 43)
        PanelBottom.TabIndex = 1
        PanelBottom.WrapContents = False
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(403, 10)
        Me.BtnCancel.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "{&Cancel}"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnOK
        '
        Me.BtnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Location = New System.Drawing.Point(316, 10)
        Me.BtnOK.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(75, 23)
        Me.BtnOK.TabIndex = 0
        Me.BtnOK.Text = "{&Ok}"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'PanelMain
        '
        PanelMain.AutoSize = True
        PanelMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        PanelMain.BackColor = System.Drawing.SystemColors.Window
        PanelMain.Controls.Add(Me.FlowLayoutPanel1)
        PanelMain.Controls.Add(Me.ChkCaseSensitive)
        PanelMain.Controls.Add(Me.TextSearch)
        PanelMain.Controls.Add(Me.Label1)
        PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        PanelMain.Location = New System.Drawing.Point(0, 0)
        PanelMain.Name = "PanelMain"
        PanelMain.Padding = New System.Windows.Forms.Padding(18, 18, 18, 6)
        PanelMain.Size = New System.Drawing.Size(496, 190)
        PanelMain.TabIndex = 0
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel1.Controls.Add(Me.RadBtnText)
        Me.FlowLayoutPanel1.Controls.Add(Me.RadBtnHex)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(18, 18)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(118, 23)
        Me.FlowLayoutPanel1.TabIndex = 0
        '
        'RadBtnText
        '
        Me.RadBtnText.AutoSize = True
        Me.RadBtnText.Location = New System.Drawing.Point(3, 3)
        Me.RadBtnText.Name = "RadBtnText"
        Me.RadBtnText.Size = New System.Drawing.Size(54, 17)
        Me.RadBtnText.TabIndex = 0
        Me.RadBtnText.TabStop = True
        Me.RadBtnText.Text = "{Text}"
        Me.RadBtnText.UseVisualStyleBackColor = True
        '
        'RadBtnHex
        '
        Me.RadBtnHex.AutoSize = True
        Me.RadBtnHex.Location = New System.Drawing.Point(63, 3)
        Me.RadBtnHex.Name = "RadBtnHex"
        Me.RadBtnHex.Size = New System.Drawing.Size(52, 17)
        Me.RadBtnHex.TabIndex = 1
        Me.RadBtnHex.TabStop = True
        Me.RadBtnHex.Text = "{Hex}"
        Me.RadBtnHex.UseVisualStyleBackColor = True
        '
        'ChkCaseSensitive
        '
        Me.ChkCaseSensitive.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ChkCaseSensitive.AutoSize = True
        Me.ChkCaseSensitive.Location = New System.Drawing.Point(374, 51)
        Me.ChkCaseSensitive.Name = "ChkCaseSensitive"
        Me.ChkCaseSensitive.Size = New System.Drawing.Size(104, 17)
        Me.ChkCaseSensitive.TabIndex = 2
        Me.ChkCaseSensitive.Text = "{Case Sensitive}"
        Me.ChkCaseSensitive.UseVisualStyleBackColor = True
        '
        'TextSearch
        '
        Me.TextSearch.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextSearch.Location = New System.Drawing.Point(18, 70)
        Me.TextSearch.Multiline = True
        Me.TextSearch.Name = "TextSearch"
        Me.TextSearch.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextSearch.Size = New System.Drawing.Size(460, 111)
        Me.TextSearch.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(18, 51)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(67, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "{Search For}"
        '
        'HexSearchForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BtnCancel
        Me.ClientSize = New System.Drawing.Size(496, 233)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "HexSearchForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        PanelBottom.ResumeLayout(False)
        PanelMain.ResumeLayout(False)
        PanelMain.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents RadBtnText As RadioButton
    Friend WithEvents RadBtnHex As RadioButton
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents TextSearch As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents ChkCaseSensitive As CheckBox
End Class
