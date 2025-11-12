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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HexSearchForm))
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
        resources.ApplyResources(PanelBottom, "PanelBottom")
        PanelBottom.Name = "PanelBottom"
        '
        'BtnCancel
        '
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnOK
        '
        resources.ApplyResources(Me.BtnOK, "BtnOK")
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'PanelMain
        '
        resources.ApplyResources(PanelMain, "PanelMain")
        PanelMain.BackColor = System.Drawing.SystemColors.Window
        PanelMain.Controls.Add(Me.FlowLayoutPanel1)
        PanelMain.Controls.Add(Me.ChkCaseSensitive)
        PanelMain.Controls.Add(Me.TextSearch)
        PanelMain.Controls.Add(Me.Label1)
        PanelMain.Name = "PanelMain"
        '
        'FlowLayoutPanel1
        '
        resources.ApplyResources(Me.FlowLayoutPanel1, "FlowLayoutPanel1")
        Me.FlowLayoutPanel1.Controls.Add(Me.RadBtnText)
        Me.FlowLayoutPanel1.Controls.Add(Me.RadBtnHex)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        '
        'RadBtnText
        '
        resources.ApplyResources(Me.RadBtnText, "RadBtnText")
        Me.RadBtnText.Name = "RadBtnText"
        Me.RadBtnText.TabStop = True
        Me.RadBtnText.UseVisualStyleBackColor = True
        '
        'RadBtnHex
        '
        resources.ApplyResources(Me.RadBtnHex, "RadBtnHex")
        Me.RadBtnHex.Name = "RadBtnHex"
        Me.RadBtnHex.TabStop = True
        Me.RadBtnHex.UseVisualStyleBackColor = True
        '
        'ChkCaseSensitive
        '
        resources.ApplyResources(Me.ChkCaseSensitive, "ChkCaseSensitive")
        Me.ChkCaseSensitive.Name = "ChkCaseSensitive"
        Me.ChkCaseSensitive.UseVisualStyleBackColor = True
        '
        'TextSearch
        '
        resources.ApplyResources(Me.TextSearch, "TextSearch")
        Me.TextSearch.Name = "TextSearch"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'HexSearchForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BtnCancel
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "HexSearchForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
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
