Imports System.ComponentModel
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Public Class ImageImportForm
        Inherits BaseForm

        Private WithEvents ButtonClear As Button
        Private WithEvents ButtonImport As Button
        Private WithEvents ButtonOpen As Button
        Private WithEvents ButtonProcess As Button
        Private WithEvents CheckBoxDoublestep As CheckBox
        Private WithEvents ComboExtensions As ComboBox
        Private WithEvents ComboImageFormat As ComboBox
        Private WithEvents ComboOutputType As ComboBox
        Private WithEvents TextBoxFileName As TextBox
        Private ReadOnly _Initialized As Boolean = False
        Private ReadOnly _TrackStatus As TrackStatus
        Private _ComboExtensionsNoEvent As Boolean = False
        Private _DoubleStep As Boolean = False
        Private _InputFilePath As String
        Private _OutputFilePath As String = ""
        Private _ProcessRunning As Boolean = False
        Private _SideCount As Integer
        Private _TrackCount As Integer

        Public Event ImportRequested(File As String, NewFilename As String)

        Public Sub New(FilePath As String, TrackCount As Integer, SideCount As Integer)
            MyBase.New(Settings.LogFileName)

            Me.AllowDrop = True

            _TrackStatus = New TrackStatus(Me)

            _InputFilePath = FilePath
            _TrackCount = TrackCount
            _SideCount = SideCount
            InitializeControls()
            InitializeImage()

            _Initialized = True
        End Sub

        Public ReadOnly Property OutputFilePath As String
            Get
                Return _OutputFilePath
            End Get
        End Property

        Public Function GetNewFileName() As String
            Dim Item As FileExtensionItem = ComboExtensions.SelectedItem

            Return TextBoxFileName.Text & Item.Extension
        End Function

        Public Sub InitializeImage()
            Dim ImageFormat = ReadImageFormat()
            GridReset(_TrackCount, _SideCount)
            PopulateOutputTypes()
            PopulateImageFormats(ComboImageFormat, ImageFormat, ImageFormat)
            PopulateFileExtensions()
            SetNewFileName()
            SetTiltebarText()
            ClearStatusBar()
            RefreshButtonState(True)
        End Sub

        Protected Overrides Sub OnAfterBaseFormClosing(e As FormClosingEventArgs)
            If e.CloseReason = CloseReason.UserClosing OrElse CancelButtonClicked Then
                ClearOutputFile()
            End If
        End Sub

        Private Function CanAcceptDrop(paths As IEnumerable(Of String)) As Boolean
            For Each path In paths
                If IsValidFluxImport(path, True).Result Then
                    Return True
                End If

                Exit For
            Next

            Return False
        End Function

        Private Sub ClearLoadedImage()
            SetTiltebarText()
            TextBoxFileName.Text = ""
            _InputFilePath = ""
            ComboImageFormat.SelectedIndex = 0
            RefreshButtonState(True)
        End Sub

        Private Sub ClearOutputFile()
            If Not String.IsNullOrEmpty(_OutputFilePath) Then
                DeleteFileIfExists(_OutputFilePath)
            End If
            _OutputFilePath = ""
        End Sub

        Private Sub ClearProcessedImage(KeepOutputFile As Boolean)
            TextBoxConsole.Clear()
            If KeepOutputFile Then
                _OutputFilePath = ""
            Else
                ClearOutputFile()
            End If
            ClearStatusBar()
            _TrackStatus.Clear()
            GridReset(_TrackCount, _SideCount)
        End Sub

        Private Function GenerateCommandLine(DiskParams As FloppyDiskParams, OutputType As GreaseweazleOutputType, DoubleStep As Boolean) As String
            Dim Builder As New CommandLineBuilder(CommandLineBuilder.CommandAction.convert) With {
                .InFile = _InputFilePath,
                .OutFile = _OutputFilePath
            }

            If Not DiskParams.IsStandard Then
                OutputType = GreaseweazleOutputType.HFE
            End If

            If OutputType <> GreaseweazleOutputType.HFE Then
                Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(DiskParams.Format)
                Builder.Format = GreaseweazleImageFormatCommandLine(ImageFormat)
            Else
                Builder.BitRate = DiskParams.BitRateKbps
                Builder.AdjustSpeed = DiskParams.RPM & "rpm"
            End If

            If DoubleStep Then
                Builder.HeadStep = 2
            End If

            Return Builder.Arguments
        End Function

        Private Sub InitializeControls()
            Dim FileNameLabel As New Label With {
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

            Dim ImageFormatLabel As New Label With {
                .Text = My.Resources.Label_ImageFormat,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            ComboImageFormat = New ComboBox With {
                .Anchor = AnchorStyles.Left,
                .Width = 200
            }

            Dim OutputTypeLabel As New Label With {
                .Text = My.Resources.Label_OutputType,
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            ComboOutputType = New ComboBox With {
                .Anchor = AnchorStyles.Left,
                .Width = 175
            }

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
                .Text = My.Resources.Label_Clear,
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
                .Controls.Add(FileNameLabel, 0, Row)
                .Controls.Add(TextBoxFileName, 1, Row)
                .SetColumnSpan(TextBoxFileName, 3)
                .Controls.Add(ComboExtensions, 3, Row)
                .Controls.Add(ButtonOpen, 5, Row)

                Row = 1
                .Controls.Add(ImageFormatLabel, 0, Row)
                .Controls.Add(ComboImageFormat, 1, Row)

                .Controls.Add(OutputTypeLabel, 2, Row)
                .Controls.Add(ComboOutputType, 3, Row)
                .SetColumnSpan(ComboOutputType, 2)

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

        Private Sub OpenFluxImage(Filename As String)
            Dim response = AnalyzeFluxImage(Filename, True)
            If response.Result Then
                _TrackCount = response.TrackCount
                _SideCount = response.SideCount
                _InputFilePath = Filename
                InitializeImage()
            End If
        End Sub

        Private Sub OpenFluxImage()
            Dim FileName As String = Greaseweazle.OpenFluxImage(Me)
            If FileName <> "" Then
                OpenFluxImage(FileName)
            End If
        End Sub
        Private Sub PopulateFileExtensions()
            Dim OutputType As GreaseweazleOutputType = ComboOutputType.SelectedValue

            If OutputType = GreaseweazleOutputType.HFE Then
                Dim items As New List(Of FileExtensionItem) From {
                    New FileExtensionItem(".hfe", Nothing)
                }
                With ComboExtensions
                    .DataSource = Nothing
                    .Items.Clear()
                    .DisplayMember = "Extension"
                    .DataSource = items
                    .SelectedIndex = 0
                    .DropDownStyle = ComboBoxStyle.DropDownList
                    .Enabled = False
                End With
            Else
                Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue
                ComboExtensions.Enabled = True
                SharedLib.PopulateFileExtensions(ComboExtensions, ImageParams.Format)
            End If
        End Sub

        Private Sub PopulateOutputTypes()
            Dim DriveList As New List(Of KeyValuePair(Of String, GreaseweazleOutputType))
            For Each OutputType As GreaseweazleOutputType In [Enum].GetValues(GetType(GreaseweazleOutputType))
                DriveList.Add(New KeyValuePair(Of String, GreaseweazleOutputType)(
                    GreaseweazleOutputTypeDescription(OutputType), OutputType)
                )
            Next

            InitializeCombo(ComboOutputType, DriveList, Nothing)
        End Sub

        Private Sub ProcessImage()
            Dim DiskParams As FloppyDiskParams = ComboImageFormat.SelectedValue

            If DiskParams.IsNonImage Then
                Exit Sub
            End If

            ClearProcessedImage(False)

            Dim OutputType As GreaseweazleOutputType = ComboOutputType.SelectedValue

            Dim TempPath = InitTempImagePath()
            Dim FileName = "New Image" & GreaseweazleOutputTypeFileExt(OutputType)

            If TempPath = "" Then
                MsgBox(My.Resources.Dialog_TempPathError, MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            _OutputFilePath = GenerateUniqueFileName(TempPath, FileName)

            Dim DoubleStep As Boolean = DiskParams.IsStandard AndAlso CheckBoxDoublestep.Enabled AndAlso CheckBoxDoublestep.Checked
            _DoubleStep = DoubleStep

            ToggleProcessRunning(True)

            Dim Arguments = GenerateCommandLine(DiskParams, OutputType, DoubleStep)
            Process.StartAsync(Settings.AppPath, Arguments)
        End Sub
        Private Sub ProcessImport()
            RaiseEvent ImportRequested(_OutputFilePath, GetNewFileName())
            ClearProcessedImage(True)
            ClearLoadedImage()
        End Sub
        Private Sub ProcessOutputLine(line As String)
            If TextBoxConsole.Text.Length > 0 Then
                TextBoxConsole.AppendText(Environment.NewLine)
            End If
            TextBoxConsole.AppendText(line)

            Dim Summary = _TrackStatus.ParseTrackReadSummary(line)
            If Summary IsNot Nothing Then
                Dim Details = _TrackStatus.ParseTrackReadDetails(Summary.Details)
                If Details IsNot Nothing Then
                    Dim Statusinfo = _TrackStatus.UpdateStatusInfo(Summary, Details, True, TrackStatus.ActionTypeEnum.Import)
                    _TrackStatus.UpdateTrackStatus(Statusinfo, TrackStatus.ActionTypeEnum.Read, _DoubleStep)
                    Return
                End If
            End If

            Dim TrackInfoUnexpected = _TrackStatus.ParseTrackUnexpected(line)
            If TrackInfoUnexpected IsNot Nothing Then
                Dim StatusInfo = _TrackStatus.UpdateStatusInfo(TrackInfoUnexpected, TrackStatus.ActionTypeEnum.Import)
                _TrackStatus.UpdateTrackStatus(StatusInfo, TrackStatus.ActionTypeEnum.Read, _DoubleStep)
                Return
            End If
        End Sub

        Private Function ReadImageFormat() As DiskImage.FloppyDiskFormat
            Dim Response = ConvertFirstTrack(_InputFilePath)

            If Not Response.Result Then
                Return FloppyDiskFormat.FloppyUnknown
            End If

            Return DetectImageFormat(Response.FileName, True)
        End Function

        Private Sub RefreshButtonState(CheckImageFormat As Boolean)
            Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue
            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)
            Dim HasInputFile As Boolean = Not String.IsNullOrEmpty(_InputFilePath)

            Dim OutputTypeDisabled As Boolean = False

            If CheckImageFormat Then
                If ImageParams.IsNonImage Then
                    OutputTypeDisabled = False
                Else
                    Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(ImageParams.Format)
                    OutputTypeDisabled = (ImageFormat = GreaseweazleImageFormat.None)
                End If
            End If

            ComboImageFormat.Enabled = Not _ProcessRunning AndAlso Not HasOutputfile AndAlso HasInputFile
            ComboOutputType.Enabled = Not _ProcessRunning AndAlso Not HasOutputfile AndAlso HasInputFile And Not OutputTypeDisabled

            ButtonProcess.Enabled = ImageParams.Format <> FloppyDiskFormat.FloppyUnknown AndAlso HasInputFile AndAlso (_ProcessRunning Or Not HasOutputfile)
            If _ProcessRunning Then
                ButtonProcess.Text = My.Resources.Label_Abort
            Else
                ButtonProcess.Text = My.Resources.Label_Process
            End If

            ButtonSaveLog.Enabled = Not _ProcessRunning AndAlso TextBoxConsole.Text.Length > 0
            ButtonClear.Enabled = Not _ProcessRunning AndAlso HasOutputfile

            If ImageParams.IsStandard AndAlso ImageParams.MediaType = FloppyMediaType.Media525DoubleDensity Then
                CheckBoxDoublestep.Enabled = Not _ProcessRunning AndAlso HasInputFile AndAlso Not HasOutputfile AndAlso _TrackCount > 42
                CheckBoxDoublestep.Checked = _TrackCount > 79
            Else
                CheckBoxDoublestep.Enabled = False
                CheckBoxDoublestep.Checked = False
            End If

            If _ProcessRunning Or HasOutputfile Then
                ButtonCancel.Text = My.Resources.Label_Cancel
            Else
                ButtonCancel.Text = My.Resources.Label_Close
            End If

            ButtonOpen.Enabled = Not _ProcessRunning AndAlso Not HasOutputfile

            TextBoxFileName.ReadOnly = Not HasInputFile
            ComboExtensions.Enabled = HasInputFile

            RefreshImportButtonState()
        End Sub

        Private Sub RefreshImportButtonState()
            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)
            Dim HasInputFile As Boolean = Not String.IsNullOrEmpty(_InputFilePath)

            Dim EnableImport As Boolean = Not _ProcessRunning AndAlso HasOutputfile AndAlso HasInputFile AndAlso Not String.IsNullOrEmpty(TextBoxFileName.Text)

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
            Dim DisplayFileName = IO.Path.GetFileName(_InputFilePath)
            Dim FileExt = IO.Path.GetExtension(DisplayFileName).ToLower
            If FileExt = ".raw" Then
                Dim ParentFolder As String = IO.Path.GetFileName(IO.Directory.GetParent(_InputFilePath).FullName)
                DisplayFileName = IO.Path.Combine(ParentFolder, "*.raw")
            End If

            Me.Text = My.Resources.Caption_ImportFluxImage & " - " & DisplayFileName
        End Sub
        Private Sub ToggleProcessRunning(Value As Boolean)
            _ProcessRunning = Value
            RefreshButtonState(False)
        End Sub

#Region "Events"
        Private Sub ButtonClear_Click(sender As Object, e As EventArgs) Handles ButtonClear.Click
            ClearProcessedImage(False)
            RefreshButtonState(True)
        End Sub

        Private Sub ButtonImport_Click(sender As Object, e As EventArgs) Handles ButtonImport.Click
            If _OutputFilePath <> "" Then
                ProcessImport()
            End If
        End Sub

        Private Sub ButtonOpen_Click(sender As Object, e As EventArgs) Handles ButtonOpen.Click
            OpenFluxImage()
        End Sub

        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If CancelProcessIfRunning() Then
                Exit Sub
            End If

            ProcessImage()
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

        Private Sub ComboImageFormat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageFormat.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            _ComboExtensionsNoEvent = True
            PopulateFileExtensions()
            _ComboExtensionsNoEvent = False

            RefreshButtonState(True)
        End Sub

        Private Sub ComboOutputType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboOutputType.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            _ComboExtensionsNoEvent = True
            PopulateFileExtensions()
            _ComboExtensionsNoEvent = False
        End Sub

        Private Sub ImageImportForm_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)
            If _ProcessRunning Or HasOutputfile Then
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

            Dim Response = IsValidFluxImport(firstPath, True)
            If Response.Result Then
                OpenFluxImage(Response.File)
            End If
        End Sub

        Private Sub ImageImportForm_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
            Dim HasOutputfile As Boolean = Not String.IsNullOrEmpty(_OutputFilePath)
            If _ProcessRunning Or HasOutputfile Then
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

        Private Sub Process_ErrorDataReceived(data As String) Handles Process.ErrorDataReceived
            ProcessOutputLine(data)
        End Sub

        Private Sub Process_ProcessExited(exitCode As Integer) Handles Process.ProcessExited
            Dim Aborted = (exitCode = -1)

            If Aborted Then
                ClearOutputFile()
            End If

            _TrackStatus.UpdateTrackStatusComplete(Aborted, _DoubleStep)
            ToggleProcessRunning(False)
        End Sub

        Private Sub Process_ProcessFailed(message As String, ex As Exception) Handles Process.ProcessFailed
            _TrackStatus.UpdateTrackStatusError()
            ToggleProcessRunning(False)
        End Sub

        Private Sub TextBoxFileName_Click(sender As Object, e As EventArgs) Handles TextBoxFileName.Click
            If TextBoxFileName.ReadOnly Then
                OpenFluxImage()
            End If
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
