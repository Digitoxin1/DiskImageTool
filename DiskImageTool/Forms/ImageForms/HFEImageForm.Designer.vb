<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class HFEImageForm
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
        Me.LblSignature = New System.Windows.Forms.Label()
        Me.TxtSignature = New System.Windows.Forms.TextBox()
        Me.LblTracks = New System.Windows.Forms.Label()
        Me.TxtTrackCount = New System.Windows.Forms.TextBox()
        Me.LblRPM = New System.Windows.Forms.Label()
        Me.TxtRPM = New System.Windows.Forms.TextBox()
        Me.CboInterfaceType = New System.Windows.Forms.ComboBox()
        Me.LblReserved = New System.Windows.Forms.Label()
        Me.TxtReserved = New System.Windows.Forms.TextBox()
        Me.LblTrackListOffset = New System.Windows.Forms.Label()
        Me.TxtTrackListOffset = New System.Windows.Forms.TextBox()
        Me.LblWriteAllowed = New System.Windows.Forms.Label()
        Me.CboWriteAllowed = New System.Windows.Forms.ComboBox()
        Me.LblSingleStep = New System.Windows.Forms.Label()
        Me.TxtSingleStep = New System.Windows.Forms.TextBox()
        Me.LblTrack0Side0Alt = New System.Windows.Forms.Label()
        Me.TxtTrack0Side0Alt = New System.Windows.Forms.TextBox()
        Me.LblTrack0Side1Alt = New System.Windows.Forms.Label()
        Me.TxtTrack0Side1Alt = New System.Windows.Forms.TextBox()
        Me.LblFormatRevision = New System.Windows.Forms.Label()
        Me.TxtFormatRevision = New System.Windows.Forms.TextBox()
        Me.LblSides = New System.Windows.Forms.Label()
        Me.TxtSides = New System.Windows.Forms.TextBox()
        Me.LblInterfaceType = New System.Windows.Forms.Label()
        Me.LblTrack0Side0Encoding = New System.Windows.Forms.Label()
        Me.TxtTrack0Side0Encoding = New System.Windows.Forms.TextBox()
        Me.LblTrack0Side1Encoding = New System.Windows.Forms.Label()
        Me.TxtTrack0Side1Encoding = New System.Windows.Forms.TextBox()
        Me.LblBitRate = New System.Windows.Forms.Label()
        Me.TxtBitRate = New System.Windows.Forms.TextBox()
        Me.LblTrackEncoding = New System.Windows.Forms.Label()
        Me.TxtTrackEncoding = New System.Windows.Forms.TextBox()
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
        PanelBottom.Location = New System.Drawing.Point(0, 558)
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
        PanelMain.Size = New System.Drawing.Size(704, 558)
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
        Me.DataGridViewTracks.Location = New System.Drawing.Point(12, 238)
        Me.DataGridViewTracks.MultiSelect = False
        Me.DataGridViewTracks.Name = "DataGridViewTracks"
        Me.DataGridViewTracks.ReadOnly = True
        Me.DataGridViewTracks.RowHeadersVisible = False
        Me.DataGridViewTracks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridViewTracks.Size = New System.Drawing.Size(680, 314)
        Me.DataGridViewTracks.TabIndex = 1
        '
        'TableLayoutPanelHeader
        '
        Me.TableLayoutPanelHeader.AutoSize = True
        Me.TableLayoutPanelHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanelHeader.ColumnCount = 4
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanelHeader.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblSignature, 0, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtSignature, 1, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblTracks, 0, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtTrackCount, 1, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblRPM, 0, 6)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtRPM, 1, 6)
        Me.TableLayoutPanelHeader.Controls.Add(Me.CboInterfaceType, 1, 7)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblTrack0Side0Alt, 0, 12)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtTrack0Side0Alt, 1, 12)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblTrack0Side1Alt, 0, 14)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtTrack0Side1Alt, 1, 14)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblFormatRevision, 2, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtFormatRevision, 3, 0)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblSides, 2, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtSides, 3, 2)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblInterfaceType, 0, 7)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblBitRate, 2, 6)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtBitRate, 3, 6)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblTrackEncoding, 2, 7)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtTrackEncoding, 3, 7)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblTrack0Side0Encoding, 2, 12)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblTrack0Side1Encoding, 2, 14)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtTrack0Side0Encoding, 3, 12)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtTrack0Side1Encoding, 3, 14)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblTrackListOffset, 0, 10)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblWriteAllowed, 0, 9)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtTrackListOffset, 1, 10)
        Me.TableLayoutPanelHeader.Controls.Add(Me.CboWriteAllowed, 1, 9)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblReserved, 2, 10)
        Me.TableLayoutPanelHeader.Controls.Add(Me.LblSingleStep, 2, 9)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtReserved, 3, 10)
        Me.TableLayoutPanelHeader.Controls.Add(Me.TxtSingleStep, 3, 9)
        Me.TableLayoutPanelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanelHeader.Location = New System.Drawing.Point(12, 12)
        Me.TableLayoutPanelHeader.Name = "TableLayoutPanelHeader"
        Me.TableLayoutPanelHeader.Padding = New System.Windows.Forms.Padding(0, 0, 0, 16)
        Me.TableLayoutPanelHeader.RowCount = 16
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanelHeader.Size = New System.Drawing.Size(680, 226)
        Me.TableLayoutPanelHeader.TabIndex = 0
        '
        'LblSignature
        '
        Me.LblSignature.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSignature.AutoSize = True
        Me.LblSignature.Location = New System.Drawing.Point(3, 6)
        Me.LblSignature.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblSignature.Name = "LblSignature"
        Me.LblSignature.Size = New System.Drawing.Size(60, 13)
        Me.LblSignature.TabIndex = 0
        Me.LblSignature.Text = "{Signature}"
        '
        'TxtSignature
        '
        Me.TxtSignature.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtSignature.Location = New System.Drawing.Point(120, 3)
        Me.TxtSignature.Name = "TxtSignature"
        Me.TxtSignature.ReadOnly = True
        Me.TxtSignature.Size = New System.Drawing.Size(120, 20)
        Me.TxtSignature.TabIndex = 1
        '
        'LblTracks
        '
        Me.LblTracks.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblTracks.AutoSize = True
        Me.LblTracks.Location = New System.Drawing.Point(3, 32)
        Me.LblTracks.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblTracks.Name = "LblTracks"
        Me.LblTracks.Size = New System.Drawing.Size(48, 13)
        Me.LblTracks.TabIndex = 4
        Me.LblTracks.Text = "{Tracks}"
        '
        'TxtTrackCount
        '
        Me.TxtTrackCount.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtTrackCount.Location = New System.Drawing.Point(120, 29)
        Me.TxtTrackCount.Name = "TxtTrackCount"
        Me.TxtTrackCount.ReadOnly = True
        Me.TxtTrackCount.Size = New System.Drawing.Size(64, 20)
        Me.TxtTrackCount.TabIndex = 5
        '
        'LblRPM
        '
        Me.LblRPM.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblRPM.AutoSize = True
        Me.LblRPM.Location = New System.Drawing.Point(3, 58)
        Me.LblRPM.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblRPM.Name = "LblRPM"
        Me.LblRPM.Size = New System.Drawing.Size(39, 13)
        Me.LblRPM.TabIndex = 12
        Me.LblRPM.Text = "{RPM}"
        '
        'TxtRPM
        '
        Me.TxtRPM.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtRPM.Location = New System.Drawing.Point(120, 55)
        Me.TxtRPM.MaxLength = 5
        Me.TxtRPM.Name = "TxtRPM"
        Me.TxtRPM.Size = New System.Drawing.Size(64, 20)
        Me.TxtRPM.TabIndex = 13
        '
        'CboInterfaceType
        '
        Me.CboInterfaceType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.CboInterfaceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CboInterfaceType.FormattingEnabled = True
        Me.CboInterfaceType.Location = New System.Drawing.Point(120, 81)
        Me.CboInterfaceType.Name = "CboInterfaceType"
        Me.CboInterfaceType.Size = New System.Drawing.Size(164, 21)
        Me.CboInterfaceType.TabIndex = 15
        '
        'LblReserved
        '
        Me.LblReserved.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblReserved.AutoSize = True
        Me.LblReserved.Location = New System.Drawing.Point(290, 138)
        Me.LblReserved.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblReserved.Name = "LblReserved"
        Me.LblReserved.Size = New System.Drawing.Size(61, 13)
        Me.LblReserved.TabIndex = 16
        Me.LblReserved.Text = "{Reserved}"
        '
        'TxtReserved
        '
        Me.TxtReserved.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtReserved.Location = New System.Drawing.Point(410, 135)
        Me.TxtReserved.Name = "TxtReserved"
        Me.TxtReserved.ReadOnly = True
        Me.TxtReserved.Size = New System.Drawing.Size(64, 20)
        Me.TxtReserved.TabIndex = 17
        '
        'LblTrackListOffset
        '
        Me.LblTrackListOffset.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblTrackListOffset.AutoSize = True
        Me.LblTrackListOffset.Location = New System.Drawing.Point(3, 138)
        Me.LblTrackListOffset.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblTrackListOffset.Name = "LblTrackListOffset"
        Me.LblTrackListOffset.Size = New System.Drawing.Size(93, 13)
        Me.LblTrackListOffset.TabIndex = 18
        Me.LblTrackListOffset.Text = "{Track List Offset}"
        '
        'TxtTrackListOffset
        '
        Me.TxtTrackListOffset.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtTrackListOffset.Location = New System.Drawing.Point(120, 135)
        Me.TxtTrackListOffset.Name = "TxtTrackListOffset"
        Me.TxtTrackListOffset.ReadOnly = True
        Me.TxtTrackListOffset.Size = New System.Drawing.Size(64, 20)
        Me.TxtTrackListOffset.TabIndex = 19
        '
        'LblWriteAllowed
        '
        Me.LblWriteAllowed.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblWriteAllowed.AutoSize = True
        Me.LblWriteAllowed.Location = New System.Drawing.Point(3, 112)
        Me.LblWriteAllowed.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblWriteAllowed.Name = "LblWriteAllowed"
        Me.LblWriteAllowed.Size = New System.Drawing.Size(80, 13)
        Me.LblWriteAllowed.TabIndex = 20
        Me.LblWriteAllowed.Text = "{Write Allowed}"
        '
        'CboWriteAllowed
        '
        Me.CboWriteAllowed.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.CboWriteAllowed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CboWriteAllowed.FormattingEnabled = True
        Me.CboWriteAllowed.Location = New System.Drawing.Point(120, 108)
        Me.CboWriteAllowed.Name = "CboWriteAllowed"
        Me.CboWriteAllowed.Size = New System.Drawing.Size(164, 21)
        Me.CboWriteAllowed.TabIndex = 21
        '
        'LblSingleStep
        '
        Me.LblSingleStep.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSingleStep.AutoSize = True
        Me.LblSingleStep.Location = New System.Drawing.Point(290, 112)
        Me.LblSingleStep.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblSingleStep.Name = "LblSingleStep"
        Me.LblSingleStep.Size = New System.Drawing.Size(37, 13)
        Me.LblSingleStep.TabIndex = 22
        Me.LblSingleStep.Text = "{Step}"
        '
        'TxtSingleStep
        '
        Me.TxtSingleStep.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtSingleStep.Location = New System.Drawing.Point(410, 108)
        Me.TxtSingleStep.Name = "TxtSingleStep"
        Me.TxtSingleStep.ReadOnly = True
        Me.TxtSingleStep.Size = New System.Drawing.Size(164, 20)
        Me.TxtSingleStep.TabIndex = 23
        '
        'LblTrack0Side0Alt
        '
        Me.LblTrack0Side0Alt.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblTrack0Side0Alt.AutoSize = True
        Me.LblTrack0Side0Alt.Location = New System.Drawing.Point(3, 164)
        Me.LblTrack0Side0Alt.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblTrack0Side0Alt.Name = "LblTrack0Side0Alt"
        Me.LblTrack0Side0Alt.Size = New System.Drawing.Size(106, 13)
        Me.LblTrack0Side0Alt.TabIndex = 24
        Me.LblTrack0Side0Alt.Text = "{Track 0.0 Alternate}"
        '
        'TxtTrack0Side0Alt
        '
        Me.TxtTrack0Side0Alt.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtTrack0Side0Alt.Location = New System.Drawing.Point(120, 161)
        Me.TxtTrack0Side0Alt.Name = "TxtTrack0Side0Alt"
        Me.TxtTrack0Side0Alt.ReadOnly = True
        Me.TxtTrack0Side0Alt.Size = New System.Drawing.Size(164, 20)
        Me.TxtTrack0Side0Alt.TabIndex = 25
        '
        'LblTrack0Side1Alt
        '
        Me.LblTrack0Side1Alt.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblTrack0Side1Alt.AutoSize = True
        Me.LblTrack0Side1Alt.Location = New System.Drawing.Point(3, 190)
        Me.LblTrack0Side1Alt.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblTrack0Side1Alt.Name = "LblTrack0Side1Alt"
        Me.LblTrack0Side1Alt.Size = New System.Drawing.Size(106, 13)
        Me.LblTrack0Side1Alt.TabIndex = 28
        Me.LblTrack0Side1Alt.Text = "{Track 0.1 Alternate}"
        '
        'TxtTrack0Side1Alt
        '
        Me.TxtTrack0Side1Alt.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtTrack0Side1Alt.Location = New System.Drawing.Point(120, 187)
        Me.TxtTrack0Side1Alt.Name = "TxtTrack0Side1Alt"
        Me.TxtTrack0Side1Alt.ReadOnly = True
        Me.TxtTrack0Side1Alt.Size = New System.Drawing.Size(164, 20)
        Me.TxtTrack0Side1Alt.TabIndex = 29
        '
        'LblFormatRevision
        '
        Me.LblFormatRevision.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblFormatRevision.AutoSize = True
        Me.LblFormatRevision.Location = New System.Drawing.Point(290, 6)
        Me.LblFormatRevision.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblFormatRevision.Name = "LblFormatRevision"
        Me.LblFormatRevision.Size = New System.Drawing.Size(91, 13)
        Me.LblFormatRevision.TabIndex = 2
        Me.LblFormatRevision.Text = "{Format Revision}"
        '
        'TxtFormatRevision
        '
        Me.TxtFormatRevision.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtFormatRevision.Location = New System.Drawing.Point(410, 3)
        Me.TxtFormatRevision.Name = "TxtFormatRevision"
        Me.TxtFormatRevision.ReadOnly = True
        Me.TxtFormatRevision.Size = New System.Drawing.Size(64, 20)
        Me.TxtFormatRevision.TabIndex = 3
        '
        'LblSides
        '
        Me.LblSides.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblSides.AutoSize = True
        Me.LblSides.Location = New System.Drawing.Point(290, 32)
        Me.LblSides.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblSides.Name = "LblSides"
        Me.LblSides.Size = New System.Drawing.Size(41, 13)
        Me.LblSides.TabIndex = 6
        Me.LblSides.Text = "{Sides}"
        '
        'TxtSides
        '
        Me.TxtSides.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtSides.Location = New System.Drawing.Point(410, 29)
        Me.TxtSides.Name = "TxtSides"
        Me.TxtSides.ReadOnly = True
        Me.TxtSides.Size = New System.Drawing.Size(64, 20)
        Me.TxtSides.TabIndex = 7
        '
        'LblInterfaceType
        '
        Me.LblInterfaceType.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblInterfaceType.AutoSize = True
        Me.LblInterfaceType.Location = New System.Drawing.Point(3, 85)
        Me.LblInterfaceType.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblInterfaceType.Name = "LblInterfaceType"
        Me.LblInterfaceType.Size = New System.Drawing.Size(84, 13)
        Me.LblInterfaceType.TabIndex = 14
        Me.LblInterfaceType.Text = "{Interface Type}"
        '
        'LblTrack0Side0Encoding
        '
        Me.LblTrack0Side0Encoding.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblTrack0Side0Encoding.AutoSize = True
        Me.LblTrack0Side0Encoding.Location = New System.Drawing.Point(290, 164)
        Me.LblTrack0Side0Encoding.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblTrack0Side0Encoding.Name = "LblTrack0Side0Encoding"
        Me.LblTrack0Side0Encoding.Size = New System.Drawing.Size(109, 13)
        Me.LblTrack0Side0Encoding.TabIndex = 26
        Me.LblTrack0Side0Encoding.Text = "{Track 0.0 Encoding}"
        '
        'TxtTrack0Side0Encoding
        '
        Me.TxtTrack0Side0Encoding.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtTrack0Side0Encoding.Location = New System.Drawing.Point(410, 161)
        Me.TxtTrack0Side0Encoding.Name = "TxtTrack0Side0Encoding"
        Me.TxtTrack0Side0Encoding.ReadOnly = True
        Me.TxtTrack0Side0Encoding.Size = New System.Drawing.Size(164, 20)
        Me.TxtTrack0Side0Encoding.TabIndex = 27
        '
        'LblTrack0Side1Encoding
        '
        Me.LblTrack0Side1Encoding.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblTrack0Side1Encoding.AutoSize = True
        Me.LblTrack0Side1Encoding.Location = New System.Drawing.Point(290, 190)
        Me.LblTrack0Side1Encoding.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblTrack0Side1Encoding.Name = "LblTrack0Side1Encoding"
        Me.LblTrack0Side1Encoding.Size = New System.Drawing.Size(109, 13)
        Me.LblTrack0Side1Encoding.TabIndex = 30
        Me.LblTrack0Side1Encoding.Text = "{Track 0.1 Encoding}"
        '
        'TxtTrack0Side1Encoding
        '
        Me.TxtTrack0Side1Encoding.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtTrack0Side1Encoding.Location = New System.Drawing.Point(410, 187)
        Me.TxtTrack0Side1Encoding.Name = "TxtTrack0Side1Encoding"
        Me.TxtTrack0Side1Encoding.ReadOnly = True
        Me.TxtTrack0Side1Encoding.Size = New System.Drawing.Size(164, 20)
        Me.TxtTrack0Side1Encoding.TabIndex = 31
        '
        'LblBitRate
        '
        Me.LblBitRate.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblBitRate.AutoSize = True
        Me.LblBitRate.Location = New System.Drawing.Point(290, 58)
        Me.LblBitRate.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblBitRate.Name = "LblBitRate"
        Me.LblBitRate.Size = New System.Drawing.Size(45, 13)
        Me.LblBitRate.TabIndex = 10
        Me.LblBitRate.Text = "{Bitrate}"
        '
        'TxtBitRate
        '
        Me.TxtBitRate.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtBitRate.Location = New System.Drawing.Point(410, 55)
        Me.TxtBitRate.MaxLength = 5
        Me.TxtBitRate.Name = "TxtBitRate"
        Me.TxtBitRate.Size = New System.Drawing.Size(64, 20)
        Me.TxtBitRate.TabIndex = 11
        '
        'LblTrackEncoding
        '
        Me.LblTrackEncoding.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.LblTrackEncoding.AutoSize = True
        Me.LblTrackEncoding.Location = New System.Drawing.Point(290, 85)
        Me.LblTrackEncoding.Margin = New System.Windows.Forms.Padding(3, 0, 8, 0)
        Me.LblTrackEncoding.Name = "LblTrackEncoding"
        Me.LblTrackEncoding.Size = New System.Drawing.Size(91, 13)
        Me.LblTrackEncoding.TabIndex = 8
        Me.LblTrackEncoding.Text = "{Track Encoding}"
        '
        'TxtTrackEncoding
        '
        Me.TxtTrackEncoding.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TxtTrackEncoding.Location = New System.Drawing.Point(410, 81)
        Me.TxtTrackEncoding.Name = "TxtTrackEncoding"
        Me.TxtTrackEncoding.ReadOnly = True
        Me.TxtTrackEncoding.Size = New System.Drawing.Size(164, 20)
        Me.TxtTrackEncoding.TabIndex = 9
        '
        'HFEImageForm
        '
        Me.AcceptButton = Me.BtnUpdate
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(704, 601)
        Me.Controls.Add(PanelMain)
        Me.Controls.Add(PanelBottom)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(720, 640)
        Me.Name = "HFEImageForm"
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
    Friend WithEvents LblSignature As Label
    Friend WithEvents TxtSignature As TextBox
    Friend WithEvents LblFormatRevision As Label
    Friend WithEvents TxtFormatRevision As TextBox
    Friend WithEvents LblTracks As Label
    Friend WithEvents TxtTrackCount As TextBox
    Friend WithEvents LblSides As Label
    Friend WithEvents TxtSides As TextBox
    Friend WithEvents LblTrackEncoding As Label
    Friend WithEvents TxtTrackEncoding As TextBox
    Friend WithEvents LblBitRate As Label
    Friend WithEvents TxtBitRate As TextBox
    Friend WithEvents LblRPM As Label
    Friend WithEvents TxtRPM As TextBox
    Friend WithEvents LblInterfaceType As Label
    Friend WithEvents CboInterfaceType As ComboBox
    Friend WithEvents LblReserved As Label
    Friend WithEvents TxtReserved As TextBox
    Friend WithEvents LblTrackListOffset As Label
    Friend WithEvents TxtTrackListOffset As TextBox
    Friend WithEvents LblWriteAllowed As Label
    Friend WithEvents CboWriteAllowed As ComboBox
    Friend WithEvents LblSingleStep As Label
    Friend WithEvents TxtSingleStep As TextBox
    Friend WithEvents LblTrack0Side0Alt As Label
    Friend WithEvents TxtTrack0Side0Alt As TextBox
    Friend WithEvents LblTrack0Side0Encoding As Label
    Friend WithEvents TxtTrack0Side0Encoding As TextBox
    Friend WithEvents LblTrack0Side1Alt As Label
    Friend WithEvents TxtTrack0Side1Alt As TextBox
    Friend WithEvents LblTrack0Side1Encoding As Label
    Friend WithEvents TxtTrack0Side1Encoding As TextBox
    Friend WithEvents DataGridViewTracks As DataGridView
End Class
