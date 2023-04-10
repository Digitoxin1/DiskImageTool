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
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.RadBtnText = New System.Windows.Forms.RadioButton()
        Me.RadBtnHex = New System.Windows.Forms.RadioButton()
        Me.FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.TextSearch = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ChkCaseSensitive = New System.Windows.Forms.CheckBox()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.FlowLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.AutoSize = True
        Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel1.Controls.Add(Me.RadBtnText)
        Me.FlowLayoutPanel1.Controls.Add(Me.RadBtnHex)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(24, 21)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(102, 23)
        Me.FlowLayoutPanel1.TabIndex = 2
        '
        'RadBtnText
        '
        Me.RadBtnText.AutoSize = True
        Me.RadBtnText.Location = New System.Drawing.Point(3, 3)
        Me.RadBtnText.Name = "RadBtnText"
        Me.RadBtnText.Size = New System.Drawing.Size(46, 17)
        Me.RadBtnText.TabIndex = 0
        Me.RadBtnText.TabStop = True
        Me.RadBtnText.Text = "Text"
        Me.RadBtnText.UseVisualStyleBackColor = True
        '
        'RadBtnHex
        '
        Me.RadBtnHex.AutoSize = True
        Me.RadBtnHex.Location = New System.Drawing.Point(55, 3)
        Me.RadBtnHex.Name = "RadBtnHex"
        Me.RadBtnHex.Size = New System.Drawing.Size(44, 17)
        Me.RadBtnHex.TabIndex = 1
        Me.RadBtnHex.TabStop = True
        Me.RadBtnHex.Text = "Hex"
        Me.RadBtnHex.UseVisualStyleBackColor = True
        '
        'FlowLayoutPanel2
        '
        Me.FlowLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.FlowLayoutPanel2.AutoSize = True
        Me.FlowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel2.Controls.Add(Me.BtnOK)
        Me.FlowLayoutPanel2.Controls.Add(Me.BtnCancel)
        Me.FlowLayoutPanel2.Location = New System.Drawing.Point(102, 190)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        Me.FlowLayoutPanel2.Size = New System.Drawing.Size(192, 29)
        Me.FlowLayoutPanel2.TabIndex = 1
        '
        'BtnOK
        '
        Me.BtnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Location = New System.Drawing.Point(3, 3)
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.Size = New System.Drawing.Size(90, 23)
        Me.BtnOK.TabIndex = 0
        Me.BtnOK.Text = "&OK"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(99, 3)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(90, 23)
        Me.BtnCancel.TabIndex = 1
        Me.BtnCancel.Text = "&Cancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'TextSearch
        '
        Me.TextSearch.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextSearch.Location = New System.Drawing.Point(21, 73)
        Me.TextSearch.Multiline = True
        Me.TextSearch.Name = "TextSearch"
        Me.TextSearch.Size = New System.Drawing.Size(354, 111)
        Me.TextSearch.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(21, 54)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(59, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Search For"
        '
        'ChkCaseSensitive
        '
        Me.ChkCaseSensitive.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ChkCaseSensitive.AutoSize = True
        Me.ChkCaseSensitive.Location = New System.Drawing.Point(279, 50)
        Me.ChkCaseSensitive.Name = "ChkCaseSensitive"
        Me.ChkCaseSensitive.Size = New System.Drawing.Size(96, 17)
        Me.ChkCaseSensitive.TabIndex = 4
        Me.ChkCaseSensitive.Text = "Case Sensitive"
        Me.ChkCaseSensitive.UseVisualStyleBackColor = True
        '
        'HexSearchForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.CancelButton = Me.BtnCancel
        Me.ClientSize = New System.Drawing.Size(396, 236)
        Me.Controls.Add(Me.ChkCaseSensitive)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TextSearch)
        Me.Controls.Add(Me.FlowLayoutPanel2)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "HexSearchForm"
        Me.Padding = New System.Windows.Forms.Padding(18)
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Find"
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.FlowLayoutPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents RadBtnText As RadioButton
    Friend WithEvents RadBtnHex As RadioButton
    Friend WithEvents FlowLayoutPanel2 As FlowLayoutPanel
    Friend WithEvents BtnOK As Button
    Friend WithEvents BtnCancel As Button
    Friend WithEvents TextSearch As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents ChkCaseSensitive As CheckBox
End Class
