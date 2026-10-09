<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MFMImageForm
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim PanelBottom As System.Windows.Forms.FlowLayoutPanel
        Dim PanelMain As System.Windows.Forms.Panel
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.BtnUpdate = New System.Windows.Forms.Button()
        Me.DataGridViewTracks = New System.Windows.Forms.DataGridView()
        Me.TableLayoutPanelHeader = New System.Windows.Forms.TableLayoutPanel()
        Me.LblTracks = New System.Windows.Forms.Label()
        Me.TxtTrackCount = New System.Windows.Forms.TextBox()
        Me.LblSides = New System.Windows.Forms.Label()
        Me.TxtSides = New System.Windows.Forms.TextBox()
        Me.LblRPM = New System.Windows.Forms.Label()
        Me.TxtRPM = New System.Windows.Forms.TextBox()
        Me.LblInterfaceType = New System.Windows.Forms.Label()
        Me.CboInterfaceType = New System.Windows.Forms.ComboBox()
        Me.ChkPerTrackRates = New System.Windows.Forms.CheckBox()
        Me.LblBitRate = New System.Windows.Forms.Label()
        Me.TxtBitRate = New System.Windows.Forms.TextBox()
        PanelBottom = New System.Windows.Forms.FlowLayoutPanel()
        PanelMain = New System.Windows.Forms.Panel()
        PanelBottom.SuspendLayout()
        PanelMain.SuspendLayout()
        CType(Me.DataGridViewTracks, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanelHeader.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelBottom
        '
        PanelBottom.Controls.Add(Me.BtnCancel)
        PanelBottom.Controls.Add(Me.BtnUpdate)
        PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        PanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        PanelBottom.Location = New System.Drawing.Point(0, 518)
        PanelBottom.Margin = New System.Windows.Forms.Padding(0)
        PanelBottom.Name = "PanelBottom"
        PanelBottom.Padding = New System.Windows.Forms.Padding(6, 10, 6, 10)
        PanelBottom.Size = New System.Drawing.Size(704, 43)
        PanelBottom.TabIndex = 1
        PanelBottom.WrapContents = False
        '
        'BtnCancel
        '
        Me.BtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Location = New System.Drawing.Point(611, 10)
        Me.BtnCancel.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(75, 23)
        Me.BtnCancel.TabIndex = 0
        Me.BtnCancel.Text = "{&Cancel}"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'BtnUpdate
        '
        Me.BtnUpdate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BtnUpdate.Location = New System.Drawing.Point(524, 10)
        Me.BtnUpdate.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.BtnUpdate.Name = "BtnUpdate"
        Me.BtnUpdate.Size = New System.Drawing.Size(75, 23)
        Me.BtnUpdate.TabIndex = 1
        Me.BtnUpdate.Text = "{&Update}"
        Me.BtnUpdate.UseVisualStyleBackColor = True
        '
        'PanelMain
        '
        PanelMain.Controls.Add(Me.DataGridViewTracks)
        PanelMain.Controls.Add(Me.TableLayoutPanelHeader)
        PanelMain.Dock = System.Windows.Forms.DockStyle.Fill
        PanelMain.Location = New System.Drawing.Point(0, 0)
        PanelMain.Name = "PanelMain"
        PanelMain.Padding = New System.Windows.Forms.Padding(12, 12, 12, 6)
        PanelMain.Size = New System.Drawing.Size(704, 518)
        PanelMain.TabIndex = 0
        '
        'DataGridViewTracks
        '
        Me.DataGridViewTracks.AllowUserToAddRows = False
        Me.DataGridViewTracks.AllowUserToDeleteRows = False
        Me.DataGridViewTracks.AllowUserToResizeRows = False
        Me.DataGridViewTracks.BackgroundColor = System.Drawing.SystemColors.Window
        Me.DataGridViewTracks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewTracks.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridViewTracks.Location = New System.Drawing.Point(12, 133)
        Me.DataGridViewTracks.MultiSelect = False
        Me.DataGridViewTracks.Name = "DataGridViewTracks"
        Me.DataGridViewTracks.ReadOnly = True
        Me.DataGridViewTracks.RowHeadersVisible = False
        Me.DataGridViewTracks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridViewTracks.Size = New System.Drawing.Size(680, 379)
        Me.DataGridViewTracks.TabIndex = 1
        '
        'TableLayoutPanelHeader
        '
        Me.TableLayoutPanelHeader.AutoSize = True
        Me.TableLayoutPanelHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanelHeader.ColumnCount = 5
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblTracks, 0, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtTrackCount, 1, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblSides, 2, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtSides, 3, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblRPM, 0, 1)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtRPM, 1, 1)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblInterfaceType, 0, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.CboInterfaceType, 1, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.ChkPerTrackRates, 1, 3)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblBitRate, 2, 1)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtBitRate, 3, 1)
        Me.TableLayoutPanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanelHeader.Location = New System.Drawing.Point(12, 12)
        Me.TableLayoutPanelHeader.Name = "TableLayoutPanelHeader"
        Me.TableLayoutPanelHeader.Padding = New System.Windows.Forms.Padding(0, 0, 0, 16)
        Me.TableLayoutPanelHeader.RowCount = 4
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.Size = New System.Drawing.Size(680, 121)
        Me.TableLayoutPanelHeader.TabIndex = 0
        '
        'LblTracks
        '
        Me.LblTracks.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblTracks.AutoSize = True
        Me.LblTracks.Location = New System.Drawing.Point(3, 6)
        Me.LblTracks.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblTracks.Name = "LblTracks"
        Me.LblTracks.Size = New System.Drawing.Size(48, 13)
        Me.LblTracks.TabIndex = 0
        Me.LblTracks.Text = "{Tracks}"
        '
        'TxtTrackCount
        '
        Me.TxtTrackCount.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtTrackCount.Location = New System.Drawing.Point(98, 3)
        Me.TxtTrackCount.Name = "TxtTrackCount"
        Me.TxtTrackCount.ReadOnly = True
        Me.TxtTrackCount.Size = New System.Drawing.Size(64, 20)
        Me.TxtTrackCount.TabIndex = 1
        '
        'LblSides
        '
        Me.LblSides.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSides.AutoSize = True
        Me.LblSides.Location = New System.Drawing.Point(168, 6)
        Me.LblSides.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblSides.Name = "LblSides"
        Me.LblSides.Size = New System.Drawing.Size(41, 13)
        Me.LblSides.TabIndex = 2
        Me.LblSides.Text = "{Sides}"
        '
        'TxtSides
        '
        Me.TxtSides.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtSides.Location = New System.Drawing.Point(224, 3)
        Me.TxtSides.Name = "TxtSides"
        Me.TxtSides.ReadOnly = True
        Me.TxtSides.Size = New System.Drawing.Size(64, 20)
        Me.TxtSides.TabIndex = 3
        '
        'LblRPM
        '
        Me.LblRPM.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblRPM.AutoSize = True
        Me.LblRPM.Location = New System.Drawing.Point(3, 32)
        Me.LblRPM.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblRPM.Name = "LblRPM"
        Me.LblRPM.Size = New System.Drawing.Size(39, 13)
        Me.LblRPM.TabIndex = 4
        Me.LblRPM.Text = "{RPM}"
        '
        'TxtRPM
        '
        Me.TxtRPM.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtRPM.Location = New System.Drawing.Point(98, 29)
        Me.TxtRPM.MaxLength = 5
        Me.TxtRPM.Name = "TxtRPM"
        Me.TxtRPM.Size = New System.Drawing.Size(64, 20)
        Me.TxtRPM.TabIndex = 5
        '
        'LblInterfaceType
        '
        Me.LblInterfaceType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblInterfaceType.AutoSize = True
        Me.LblInterfaceType.Location = New System.Drawing.Point(3, 59)
        Me.LblInterfaceType.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblInterfaceType.Name = "LblInterfaceType"
        Me.LblInterfaceType.Size = New System.Drawing.Size(84, 13)
        Me.LblInterfaceType.TabIndex = 8
        Me.LblInterfaceType.Text = "{Interface Type}"
        '
        'CboInterfaceType
        '
        Me.CboInterfaceType.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanelHeader.SetColumnSpan(Me.CboInterfaceType, 3)
        Me.CboInterfaceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CboInterfaceType.FormattingEnabled = True
        Me.CboInterfaceType.Location = New System.Drawing.Point(98, 55)
        Me.CboInterfaceType.Name = "CboInterfaceType"
        Me.CboInterfaceType.Size = New System.Drawing.Size(190, 21)
        Me.CboInterfaceType.TabIndex = 9
        '
        'ChkPerTrackRates
        '
        Me.ChkPerTrackRates.AutoCheck = False
        Me.ChkPerTrackRates.AutoSize = True
        Me.TableLayoutPanelHeader.SetColumnSpan(Me.ChkPerTrackRates, 3)
        Me.ChkPerTrackRates.Location = New System.Drawing.Point(98, 85)
        Me.ChkPerTrackRates.Margin = New System.Windows.Forms.Padding(3, 6, 3, 3)
        Me.ChkPerTrackRates.Name = "ChkPerTrackRates"
        Me.ChkPerTrackRates.Size = New System.Drawing.Size(160, 17)
        Me.ChkPerTrackRates.TabIndex = 10
        Me.ChkPerTrackRates.Text = "{Per-track RPM and bit rate}"
        Me.ChkPerTrackRates.UseVisualStyleBackColor = True
        '
        'LblBitRate
        '
        Me.LblBitRate.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblBitRate.AutoSize = True
        Me.LblBitRate.Location = New System.Drawing.Point(168, 32)
        Me.LblBitRate.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblBitRate.Name = "LblBitRate"
        Me.LblBitRate.Size = New System.Drawing.Size(45, 13)
        Me.LblBitRate.TabIndex = 6
        Me.LblBitRate.Text = "{Bitrate}"
        '
        'TxtBitRate
        '
        Me.TxtBitRate.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtBitRate.Location = New System.Drawing.Point(224, 29)
        Me.TxtBitRate.MaxLength = 5
        Me.TxtBitRate.Name = "TxtBitRate"
        Me.TxtBitRate.Size = New System.Drawing.Size(64, 20)
        Me.TxtBitRate.TabIndex = 7
        '
        'MFMImageForm
        '
        Me.AcceptButton = Me.BtnUpdate
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(704, 561)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(720, 480)
        Me.Name = "MFMImageForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "{Image Properties}"
        PanelBottom.ResumeLayout(False)
        PanelMain.ResumeLayout(False)
        PanelMain.PerformLayout()
        CType(Me.DataGridViewTracks, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanelHeader.ResumeLayout(False)
        Me.TableLayoutPanelHeader.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents BtnCancel As Button
    Friend WithEvents BtnUpdate As Button
    Friend WithEvents TableLayoutPanelHeader As TableLayoutPanel
    Friend WithEvents LblTracks As Label
    Friend WithEvents TxtTrackCount As TextBox
    Friend WithEvents LblSides As Label
    Friend WithEvents TxtSides As TextBox
    Friend WithEvents LblInterfaceType As Label
    Friend WithEvents CboInterfaceType As ComboBox
    Friend WithEvents DataGridViewTracks As DataGridView
    Friend WithEvents LblRPM As Label
    Friend WithEvents TxtRPM As TextBox
    Friend WithEvents ChkPerTrackRates As CheckBox
    Friend WithEvents LblBitRate As Label
    Friend WithEvents TxtBitRate As TextBox
End Class
