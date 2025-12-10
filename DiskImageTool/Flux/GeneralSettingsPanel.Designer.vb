Namespace Flux
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class GeneralSettingsPanel
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
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
            Me.GroupBoxImageSaveLocation = New System.Windows.Forms.GroupBox()
            Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
            Me.RadioImageConvertLastUsed = New System.Windows.Forms.RadioButton()
            Me.RadioImageConvertSameFolder = New System.Windows.Forms.RadioButton()
            Me.RadioImageConvertParentFolder = New System.Windows.Forms.RadioButton()
            Me.GroupBoxImageSaveLocation.SuspendLayout()
            Me.FlowLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'GroupBoxImageSaveLocation
            '
            Me.GroupBoxImageSaveLocation.AutoSize = True
            Me.GroupBoxImageSaveLocation.Controls.Add(Me.FlowLayoutPanel1)
            Me.GroupBoxImageSaveLocation.Location = New System.Drawing.Point(3, 3)
            Me.GroupBoxImageSaveLocation.Name = "GroupBoxImageSaveLocation"
            Me.GroupBoxImageSaveLocation.Padding = New System.Windows.Forms.Padding(3, 6, 3, 6)
            Me.GroupBoxImageSaveLocation.Size = New System.Drawing.Size(295, 113)
            Me.GroupBoxImageSaveLocation.TabIndex = 0
            Me.GroupBoxImageSaveLocation.TabStop = False
            Me.GroupBoxImageSaveLocation.Text = "Default save location for converted images"
            '
            'FlowLayoutPanel1
            '
            Me.FlowLayoutPanel1.AutoSize = True
            Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.FlowLayoutPanel1.Controls.Add(Me.RadioImageConvertLastUsed)
            Me.FlowLayoutPanel1.Controls.Add(Me.RadioImageConvertSameFolder)
            Me.FlowLayoutPanel1.Controls.Add(Me.RadioImageConvertParentFolder)
            Me.FlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
            Me.FlowLayoutPanel1.Location = New System.Drawing.Point(6, 22)
            Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
            Me.FlowLayoutPanel1.Size = New System.Drawing.Size(139, 69)
            Me.FlowLayoutPanel1.TabIndex = 0
            Me.FlowLayoutPanel1.WrapContents = False
            '
            'RadioImageConvertLastUsed
            '
            Me.RadioImageConvertLastUsed.AutoSize = True
            Me.RadioImageConvertLastUsed.Location = New System.Drawing.Point(3, 3)
            Me.RadioImageConvertLastUsed.Name = "RadioImageConvertLastUsed"
            Me.RadioImageConvertLastUsed.Size = New System.Drawing.Size(133, 17)
            Me.RadioImageConvertLastUsed.TabIndex = 0
            Me.RadioImageConvertLastUsed.TabStop = True
            Me.RadioImageConvertLastUsed.Text = "Last used output folder"
            Me.RadioImageConvertLastUsed.UseVisualStyleBackColor = True
            '
            'RadioImageConvertSameFolder
            '
            Me.RadioImageConvertSameFolder.AutoSize = True
            Me.RadioImageConvertSameFolder.Location = New System.Drawing.Point(3, 26)
            Me.RadioImageConvertSameFolder.Name = "RadioImageConvertSameFolder"
            Me.RadioImageConvertSameFolder.Size = New System.Drawing.Size(131, 17)
            Me.RadioImageConvertSameFolder.TabIndex = 1
            Me.RadioImageConvertSameFolder.TabStop = True
            Me.RadioImageConvertSameFolder.Text = "Same folder as flux set"
            Me.RadioImageConvertSameFolder.UseVisualStyleBackColor = True
            '
            'RadioImageConvertParentFolder
            '
            Me.RadioImageConvertParentFolder.AutoSize = True
            Me.RadioImageConvertParentFolder.Location = New System.Drawing.Point(3, 49)
            Me.RadioImageConvertParentFolder.Name = "RadioImageConvertParentFolder"
            Me.RadioImageConvertParentFolder.Size = New System.Drawing.Size(133, 17)
            Me.RadioImageConvertParentFolder.TabIndex = 2
            Me.RadioImageConvertParentFolder.TabStop = True
            Me.RadioImageConvertParentFolder.Text = "Parent folder of flux set"
            Me.RadioImageConvertParentFolder.UseVisualStyleBackColor = True
            '
            'GeneralSettingsPanel
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.GroupBoxImageSaveLocation)
            Me.Name = "GeneralSettingsPanel"
            Me.Size = New System.Drawing.Size(544, 291)
            Me.GroupBoxImageSaveLocation.ResumeLayout(False)
            Me.GroupBoxImageSaveLocation.PerformLayout()
            Me.FlowLayoutPanel1.ResumeLayout(False)
            Me.FlowLayoutPanel1.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents GroupBoxImageSaveLocation As GroupBox
        Friend WithEvents RadioImageConvertParentFolder As RadioButton
        Friend WithEvents RadioImageConvertSameFolder As RadioButton
        Friend WithEvents RadioImageConvertLastUsed As RadioButton
        Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    End Class
End Namespace
