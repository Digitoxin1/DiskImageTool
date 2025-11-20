Imports System.ComponentModel
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux
    Public Class ImportImageFormBase
        Inherits BaseForm

        Friend WithEvents ButtonClear As Button
        Friend WithEvents ButtonImport As Button
        Friend WithEvents ButtonOpen As Button
        Friend WithEvents ButtonProcess As Button
        Friend WithEvents CheckBoxDoublestep As CheckBox
        Friend WithEvents ComboExtensions As ComboBox
        Friend WithEvents ComboImageFormat As ComboBox
        Friend WithEvents ComboOutputType As ComboBox
        Friend WithEvents TextBoxFileName As TextBox

        Private ReadOnly _AllowHFE As Boolean
        Private ReadOnly _AllowSCP As Boolean
        Private _ComboExtensionsNoEvent As Boolean = False
        Private _ExtensionsDisabled As Boolean = False
        Private _Initialized As Boolean = False
        Private _InputFilePath As String = ""
        Private _OutputFilePath As String = ""
        Private _OutputTypeDisabled As Boolean = False
        Private _SideCount As Integer
        Private _TrackCount As Integer

        Public Event FileDropped As EventHandler(Of String)
        Public Event ImportRequested(File As String, NewFilename As String)

        Public Event ProcessedImageCleared As EventHandler(Of Boolean)
        Public Sub New(FilePath As String, TrackCount As Integer, SideCount As Integer, LogFileName As String, IsGreaseweazle As Boolean)
            MyBase.New(LogFileName)

            Me.AllowDrop = True

            _AllowSCP = IsGreaseweazle
            _AllowHFE = IsGreaseweazle
            _InputFilePath = FilePath
            _TrackCount = TrackCount
            _SideCount = SideCount

            InitializeControls(IsGreaseweazle)
        End Sub

        Friend ReadOnly Property DoubleStep As Boolean
            Get
                Return CheckBoxDoublestep.Enabled AndAlso CheckBoxDoublestep.Checked
            End Get
        End Property

        Friend Property Initialized As Boolean
            Get
                Return _Initialized
            End Get
            Set(value As Boolean)
                _Initialized = value
            End Set
        End Property

        Friend ReadOnly Property InputFilePath As String
            Get
                Return _InputFilePath
            End Get
        End Property

        Friend Property OutputFilePath As String
            Get
                Return _OutputFilePath
            End Get
            Set(value As String)
                _OutputFilePath = value
            End Set
        End Property

        Friend Property OutputTypeDisabled As Boolean
            Get
                Return _OutputTypeDisabled
            End Get
            Set(value As Boolean)
                _OutputTypeDisabled = value
            End Set
        End Property

        Public Function GetNewFileName() As String
            Dim Item As FileExtensionItem = ComboExtensions.SelectedItem

            Return TextBoxFileName.Text & Item.Extension
        End Function

        Friend Sub ClearOutputFile(Delete As Boolean)
            If Delete AndAlso Not String.IsNullOrEmpty(_OutputFilePath) Then
                DeleteFileIfExists(_OutputFilePath)
            End If
            _OutputFilePath = ""
        End Sub

        Friend Sub ClearProcessedImage(DeleteOutputFile As Boolean)
            TextBoxConsole.Clear()
            ClearOutputFile(DeleteOutputFile)
            ClearStatusBar()
            GridReset(_TrackCount, _SideCount)

            RaiseEvent ProcessedImageCleared(Me, DeleteOutputFile)
        End Sub

        Friend Sub InitLoadedImage()
            GridReset(_TrackCount, _SideCount)
            SetNewFileName()
            SetTiltebarText()
            ClearStatusBar()
            RefreshButtonState()
        End Sub

        Friend Function OpenFluxImage(Filename As String) As Boolean
            Dim response = AnalyzeFluxImage(Filename, _AllowSCP)
            If response.Result Then
                _TrackCount = response.TrackCount
                _SideCount = response.SideCount
                _InputFilePath = Filename

                Return True
            End If

            Return False
        End Function

        Friend Function OpenFluxImage() As Boolean
            Dim FileName As String = SharedLib.OpenFluxImage(Me, _AllowSCP)

            If FileName <> "" Then
                Return OpenFluxImage(FileName)
            End If

            Return False
        End Function

        Friend Sub PopulateFileExtensions()
            _ComboExtensionsNoEvent = True

            If _ExtensionsDisabled Then
                Dim items As New List(Of FileExtensionItem)

                If _AllowHFE Then
                    items.Add(New FileExtensionItem(".hfe", Nothing))
                End If

                With ComboExtensions
                    .DataSource = Nothing
                    .Items.Clear()
                    .DisplayMember = "Extension"
                    .DataSource = items
                    .SelectedIndex = If(items.Count > 0, 0, -1)
                    .DropDownStyle = ComboBoxStyle.DropDownList
                End With
            Else
                Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue
                SharedLib.PopulateFileExtensions(ComboExtensions, ImageParams.Format)
            End If

            ComboExtensions.Enabled = Not _ExtensionsDisabled

            _ComboExtensionsNoEvent = False
        End Sub

        Friend Sub RefreshButtonState()
            Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue
            Dim Is525DDStandard As Boolean = ImageParams.IsStandard AndAlso ImageParams.MediaType = FloppyMediaType.Media525DoubleDensity

            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)
            Dim HasInputFile As Boolean = Not String.IsNullOrEmpty(_InputFilePath)
            Dim IsRunning As Boolean = Process.IsRunning
            Dim IsIdle As Boolean = Not IsRunning

            Dim SettingsEnabled As Boolean = IsIdle AndAlso Not HasOutputfile
            Dim CanConfigure As Boolean = SettingsEnabled AndAlso HasInputFile

            ComboExtensions.Enabled = HasInputFile And Not _ExtensionsDisabled
            ComboImageFormat.Enabled = CanConfigure

            If ComboOutputType IsNot Nothing Then
                ComboOutputType.Enabled = CanConfigure And Not _OutputTypeDisabled
            End If

            ButtonCancel.Text = If(IsRunning Or HasOutputfile, My.Resources.Label_Cancel, My.Resources.Label_Close)
            ButtonClear.Enabled = IsIdle AndAlso HasOutputfile
            ButtonOpen.Enabled = SettingsEnabled

            ButtonProcess.Enabled = Not ImageParams.IsNonImage AndAlso HasInputFile AndAlso (IsRunning Or Not HasOutputfile)
            ButtonProcess.Text = If(IsRunning, My.Resources.Label_Abort, My.Resources.Label_Process)

            ButtonSaveLog.Enabled = IsIdle AndAlso Not String.IsNullOrEmpty(TextBoxConsole.Text)

            TextBoxFileName.ReadOnly = Not HasInputFile

            If Is525DDStandard Then
                CheckBoxDoublestep.Enabled = CanConfigure AndAlso _TrackCount > 42
                CheckBoxDoublestep.Checked = _TrackCount > 79
            Else
                CheckBoxDoublestep.Enabled = False
                CheckBoxDoublestep.Checked = False
            End If

            RefreshImportButtonState()
        End Sub

        Protected Overrides Sub OnAfterBaseFormClosing(e As FormClosingEventArgs)
            If e.CloseReason = CloseReason.UserClosing OrElse CancelButtonClicked Then
                ClearOutputFile(True)
            End If
        End Sub
        Private Function CanAcceptDrop(paths As IEnumerable(Of String)) As Boolean
            For Each path In paths
                If IsValidFluxImport(path, _AllowSCP).Result Then
                    Return True
                End If

                Exit For
            Next

            Return False
        End Function

        Private Sub ClearLoadedImage()
            TextBoxFileName.Text = ""
            _InputFilePath = ""
            ComboImageFormat.SelectedIndex = 0
            SetTiltebarText()
            RefreshButtonState()
        End Sub
        Private Sub InitializeControls(DisplayOutputType As Boolean)
            Dim LabelFileName As New Label With {
                .Text = My.Resources.Label_FileName,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            TextBoxFileName = New TextBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .MaxLength = 255
            }

            ComboExtensions = New ComboBox With {
                .Anchor = AnchorStyles.Left,
                .Width = 50,
                .DropDownStyle = ComboBoxStyle.DropDownList
            }

            Dim LabelImageFormat As New Label With {
                .Text = My.Resources.Label_ImageFormat,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            ComboImageFormat = New ComboBox With {
                .Anchor = AnchorStyles.Left,
                .Width = 200
            }

            Dim LabelOutputType As Label = Nothing

            If DisplayOutputType Then
                LabelOutputType = New Label With {
                    .Text = My.Resources.Label_OutputType,
                    .Anchor = AnchorStyles.Left,
                    .AutoSize = True,
                    .Margin = New Padding(12, 3, 3, 3)
                }

                ComboOutputType = New ComboBox With {
                    .Anchor = AnchorStyles.Left,
                    .Width = 175
                }
            End If

            CheckBoxDoublestep = New CheckBox With {
                .Text = My.Resources.Label_DoubleStep,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            Dim ButtonContainer As New FlowLayoutPanel With {
                .FlowDirection = FlowDirection.TopDown,
                .AutoSize = True,
                .Margin = New Padding(12, 24, 3, 3)
            }

            ButtonProcess = New Button With {
                .Margin = New Padding(3, 0, 3, 3),
                .Text = My.Resources.Label_Write,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonClear = New Button With {
                .Margin = New Padding(3, 12, 3, 3),
                .Text = My.Resources.Label_Discard,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right
            }

            ButtonImport = New Button With {
                .Margin = New Padding(6, 0, 6, 0),
                .Text = My.Resources.Label_Import,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True,
                .TabIndex = 0
            }

            ButtonOpen = New Button With {
                .Margin = New Padding(15, 3, 3, 3),
                .Text = My.Resources.Label_Open,
                .MinimumSize = New Size(75, 0),
                .AutoSize = True
            }

            BumpTabIndexes(PanelButtonsRight)
            PanelButtonsRight.Controls.Add(ButtonImport)

            ButtonContainer.Controls.Add(ButtonProcess)
            ButtonContainer.Controls.Add(ButtonClear)

            ButtonOk.Text = My.Resources.Label_ImportClose
            ButtonOk.Visible = True

            Dim Row As Integer

            With TableLayoutPanelMain
                .SuspendLayout()

                .Left = 0
                .RowCount = 3
                .ColumnCount = 6
                .Dock = DockStyle.Fill

                While .RowStyles.Count < .RowCount
                    .RowStyles.Add(New RowStyle())
                End While
                For i As Integer = 0 To .RowCount - 1
                    .RowStyles(i).SizeType = SizeType.AutoSize
                Next

                While .ColumnStyles.Count < .ColumnCount
                    .ColumnStyles.Add(New ColumnStyle())
                End While
                For j As Integer = 0 To .ColumnCount - 1
                    .ColumnStyles(j).SizeType = SizeType.AutoSize
                Next

                .ColumnStyles(0).SizeType = SizeType.Percent
                .ColumnStyles(0).Width = 100

                Row = 0
                .Controls.Add(LabelFileName, 0, Row)
                .Controls.Add(TextBoxFileName, 1, Row)
                .SetColumnSpan(TextBoxFileName, 3)
                .Controls.Add(ComboExtensions, 3, Row)
                .Controls.Add(ButtonOpen, 5, Row)

                Row = 1
                .Controls.Add(LabelImageFormat, 0, Row)
                .Controls.Add(ComboImageFormat, 1, Row)

                If DisplayOutputType Then
                    .Controls.Add(LabelOutputType, 2, Row)
                    .Controls.Add(ComboOutputType, 3, Row)
                    .SetColumnSpan(ComboOutputType, 2)
                End If

                .Controls.Add(CheckBoxDoublestep, 5, Row)

                Row = 2
                .Controls.Add(TableSide0, 0, Row)
                .SetColumnSpan(TableSide0, 2)

                .Controls.Add(TableSide1, 2, Row)
                .SetColumnSpan(TableSide1, 3)

                .Controls.Add(ButtonContainer, 5, Row)

                .ResumeLayout()
                '.Left = (.Parent.ClientSize.Width - .Width) \ 2
            End With
        End Sub

        Private Sub ProcessImport()
            RaiseEvent ImportRequested(OutputFilePath, GetNewFileName())

            ClearProcessedImage(False)
            ClearLoadedImage()
        End Sub

        Private Sub RefreshImportButtonState()
            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)
            Dim HasInputFile As Boolean = Not String.IsNullOrEmpty(_InputFilePath)

            Dim EnableImport As Boolean = Not Process.IsRunning AndAlso HasOutputfile AndAlso HasInputFile AndAlso Not String.IsNullOrEmpty(TextBoxFileName.Text)

            ButtonOk.Enabled = EnableImport
            ButtonImport.Enabled = EnableImport
        End Sub

        Private Sub RefreshPreferredExensions()
            Dim Item As FileExtensionItem = ComboExtensions.SelectedValue

            If Not Item.Format.HasValue Then
                Exit Sub
            End If

            Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            If Item.Format.Value <> ImageParams.Format Then
                App.AppSettings.RemovePreferredExtension(ImageParams.Format)
            End If

            App.AppSettings.SetPreferredExtension(Item.Format.Value, Item.Extension)
        End Sub
        Private Sub SetNewFileName()
            Dim FileExt = IO.Path.GetExtension(_InputFilePath).ToLower

            If FileExt = ".raw" Then
                Dim ParentFolder As String = IO.Path.GetFileName(IO.Directory.GetParent(_InputFilePath).FullName)
                TextBoxFileName.Text = ParentFolder
            Else
                TextBoxFileName.Text = IO.Path.GetFileNameWithoutExtension(_InputFilePath)
            End If
        End Sub

        Private Sub SetTiltebarText()
            Dim Text = My.Resources.Caption_ImportFluxImage

            If String.IsNullOrEmpty(_InputFilePath) Then
                Me.Text = Text
                Exit Sub
            End If

            Dim DisplayFileName = IO.Path.GetFileName(_InputFilePath)
            Dim FileExt = IO.Path.GetExtension(DisplayFileName).ToLower

            If FileExt = ".raw" Then
                Dim ParentFolder As String = IO.Path.GetFileName(IO.Directory.GetParent(_InputFilePath).FullName)
                DisplayFileName = IO.Path.Combine(ParentFolder, "*.raw")
            End If

            Me.Text = Text & " - " & DisplayFileName
        End Sub

#Region "Events"
        Private Sub ButtonClear_Click(sender As Object, e As EventArgs) Handles ButtonClear.Click
            ClearProcessedImage(True)
            RefreshButtonState()
        End Sub

        Private Sub ButtonImport_Click(sender As Object, e As EventArgs) Handles ButtonImport.Click
            If OutputFilePath <> "" Then
                ProcessImport()
            End If
        End Sub

        Private Sub ComboExtensions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboExtensions.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            If _ComboExtensionsNoEvent Then
                Exit Sub
            End If

            RefreshPreferredExensions()
        End Sub

        Private Sub ComboOutputType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboOutputType.SelectedIndexChanged
            If Not Initialized Then
                Exit Sub
            End If

            Dim OutputType As Greaseweazle.ImageImportOutputTypes = ComboOutputType.SelectedValue
            _ExtensionsDisabled = (OutputType = Greaseweazle.ImageImportOutputTypes.HFE)
            PopulateFileExtensions()
        End Sub

        Private Sub ImageImportForm_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)
            If Process.IsRunning Or HasOutputfile Then
                Return
            End If

            If Not e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Return
            End If

            Dim paths = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
            If paths Is Nothing OrElse paths.Length = 0 Then
                Exit Sub
            End If

            Dim firstPath = paths(0)

            Dim Response = IsValidFluxImport(firstPath, _AllowSCP)
            If Response.Result Then
                RaiseEvent FileDropped(Me, Response.File)
            End If
        End Sub

        Private Sub ImageImportForm_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)

            If Process.IsRunning Or HasOutputfile Then
                e.Effect = DragDropEffects.None
                Return
            End If

            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Dim paths = DirectCast(e.Data.GetData(DataFormats.FileDrop), String())
                If paths IsNot Nothing AndAlso CanAcceptDrop(paths) Then
                    e.Effect = DragDropEffects.Copy
                    Return
                End If
            End If

            e.Effect = DragDropEffects.None
        End Sub

        Private Sub TextBoxFileName_TextChanged(sender As Object, e As EventArgs) Handles TextBoxFileName.TextChanged
            RefreshImportButtonState()
        End Sub

        Private Sub TextBoxFileName_Validating(sender As Object, e As CancelEventArgs) Handles TextBoxFileName.Validating
            Dim tb As TextBox = DirectCast(sender, TextBox)
            tb.Text = SanitizeFileName(tb.Text)
            RefreshImportButtonState()
        End Sub
#End Region

    End Class
End Namespace
