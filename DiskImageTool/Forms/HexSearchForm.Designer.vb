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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HexSearchForm))
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
        'FlowLayoutPanel2
        '
        resources.ApplyResources(Me.FlowLayoutPanel2, "FlowLayoutPanel2")
        Me.FlowLayoutPanel2.Controls.Add(Me.BtnOK)
        Me.FlowLayoutPanel2.Controls.Add(Me.BtnCancel)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        '
        'BtnOK
        '
        resources.ApplyResources(Me.BtnOK, "BtnOK")
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
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
        'ChkCaseSensitive
        '
        resources.ApplyResources(Me.ChkCaseSensitive, "ChkCaseSensitive")
        Me.ChkCaseSensitive.Name = "ChkCaseSensitive"
        Me.ChkCaseSensitive.UseVisualStyleBackColor = True
        '
        'HexSearchForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BtnCancel
        Me.Controls.Add(Me.ChkCaseSensitive)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TextSearch)
        Me.Controls.Add(Me.FlowLayoutPanel2)
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "HexSearchForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
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
