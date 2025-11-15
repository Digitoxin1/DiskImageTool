Imports System.ComponentModel
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Greaseweazle
    Public Class ImageImportForm
        Inherits BaseForm

        Private WithEvents ButtonProcess As Button
        Private WithEvents CheckBoxDoublestep As CheckBox
        Private WithEvents ComboImageFormat As ComboBox
        Private WithEvents ComboOutputType As ComboBox
        Private WithEvents TextBoxFileName As TextBox
        Private ReadOnly _InputFilePath As String
        Private ReadOnly _SideCount As Integer
        Private ReadOnly _TrackCount As Integer
        Private ReadOnly _TrackStatus As Dictionary(Of String, TrackStatusInfo)
        Private _CachedOutputTypeValue As GreaseweazleOutputType = GreaseweazleOutputType.IMA
        Private _DoubleStep As Boolean = False
        Private _OutputFilePath As String = ""
        Private _ProcessRunning As Boolean = False
        Private _StatusBadSectors As ToolStripStatusLabel
        Private _StatusUnexpected As ToolStripStatusLabel
        Private _TotalBadSectors As UInteger = 0
        Private _TotalUnexpectedSectors As UInteger = 0
        Public Sub New(FilePath As String, TrackCount As Integer, SideCount As Integer)
            MyBase.New()

            _InputFilePath = FilePath
            _TrackStatus = New Dictionary(Of String, TrackStatusInfo)
            _TrackCount = TrackCount
            _SideCount = SideCount
            GridReset(_TrackCount, _SideCount)
            InitializeControls()
            Dim ImageFormat = DetectImageFormat()
            PopulateOutputTypes()
            PopulateImageFormats(ImageFormat)

            SetNewFileName()
            SetTilebarText()
            ResetStatusBar()
        End Sub

        Public ReadOnly Property OutputFilePath As String
            Get
                Return _OutputFilePath
            End Get
        End Property

        Public Function GetNewFileName() As String
            Dim OutputType As GreaseweazleOutputType = ComboOutputType.SelectedValue

            Return TextBoxFileName.Text & GreaseweazleOutputTypeFileExt(OutputType)
        End Function

        Private Sub ClearOutputFile()
            If Not String.IsNullOrEmpty(_OutputFilePath) Then
                DeleteFileIfExists(_OutputFilePath)
            End If
            _OutputFilePath = ""
        End Sub

        Private Function DetectImageFormat() As DiskImage.FloppyDiskFormat
            Dim Response = ConvertFirstTrack(_InputFilePath)

            If Not Response.Result Then
                Return GreaseweazleImageFormat.None
            End If

            Dim Buffer As Byte()
            Dim SecondOffset As Long = 4096
            Dim Length As Integer = 513
            Dim DetectedFormat As FloppyDiskFormat

            Using fs As New IO.FileStream(Response.FileName, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
                Using br As New IO.BinaryReader(fs, System.Text.Encoding.ASCII, leaveOpen:=False)
                    Buffer = br.ReadBytes(Length)
                    DetectedFormat = FloppyDiskFormatGet(Buffer)
                    If DetectedFormat = FloppyDiskFormat.FloppyXDFMicro Then
                        If fs.Length >= SecondOffset + Length Then
                            fs.Seek(SecondOffset, IO.SeekOrigin.Begin)
                            Buffer = br.ReadBytes(513)
                            DetectedFormat = FloppyDiskFormatGet(Buffer)
                        End If
                    End If
                End Using
            End Using

            DeleteFileIfExists(Response.FileName)

            Return DetectedFormat
        End Function

        Private Function GenerateCommandLine(DiskParams As FloppyDiskParams, OutputType As GreaseweazleOutputType, DoubleStep As Boolean) As String
            Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.convert) With {
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
            Dim FileNameLabel = New Label With {
                .Text = My.Resources.Label_FileName,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            TextBoxFileName = New TextBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .MaxLength = 255
            }

            Dim ImageFormatLabel = New Label With {
                .Text = My.Resources.Label_ImageFormat,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            ComboImageFormat = New ComboBox With {
                .Anchor = AnchorStyles.Left,
                .Width = 200
            }

            Dim OutputTypeLabel = New Label With {
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

            _StatusBadSectors = New ToolStripStatusLabel With {
                .TextAlign = ContentAlignment.MiddleRight
            }
            StatusStripBottom.Items.Add(_StatusBadSectors)

            _StatusUnexpected = New ToolStripStatusLabel With {
                .TextAlign = ContentAlignment.MiddleRight,
                .Margin = New Padding(6, 3, 0, 2)
            }
            StatusStripBottom.Items.Add(_StatusUnexpected)

            ButtonProcess = New Button With {
                .Width = 75,
                .Margin = New Padding(12, 24, 3, 3)
            }

            ButtonOk.Text = My.Resources.Label_Import
            ButtonOk.Visible = True

            Dim Row As Integer

            With TableLayoutPanelMain
                .SuspendLayout()

                .RowCount = 3
                .ColumnCount = 5

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

                Row = 0
                .Controls.Add(FileNameLabel, 0, Row)
                .Controls.Add(TextBoxFileName, 1, Row)
                .SetColumnSpan(TextBoxFileName, 3)

                Row = 1
                .Controls.Add(ImageFormatLabel, 0, Row)
                .Controls.Add(ComboImageFormat, 1, Row)

                .Controls.Add(OutputTypeLabel, 2, Row)
                .Controls.Add(ComboOutputType, 3, Row)

                .Controls.Add(CheckBoxDoublestep, 4, Row)

                Row = 2
                .Controls.Add(TableSide0, 0, Row)
                .SetColumnSpan(TableSide0, 2)

                .Controls.Add(TableSide1, 2, Row)
                .SetColumnSpan(TableSide1, 2)

                .Controls.Add(ButtonProcess, 4, Row)

                .ResumeLayout()
                .Left = (.Parent.ClientSize.Width - .Width) \ 2
            End With
        End Sub

        Private Sub PopulateImageFormats(SelectedFormat As FloppyDiskFormat?)
            Dim list = FloppyDiskFormatGetComboList()

            ComboImageFormat.DisplayMember = "Description"
            ComboImageFormat.ValueMember = Nothing
            ComboImageFormat.DataSource = list
            ComboImageFormat.DropDownStyle = ComboBoxStyle.DropDownList

            If SelectedFormat.HasValue Then
                Dim match = list.FirstOrDefault(Function(p) p.Format = SelectedFormat.Value)
                If match IsNot Nothing Then
                    match.Detected = True
                    ComboImageFormat.SelectedItem = match
                End If
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

            ClearOutputFile()
            ResetStatusBar()

            Dim OutputType As GreaseweazleOutputType = ComboOutputType.SelectedValue

            Dim TempPath = InitTempImagePath()
            Dim FileName = "New Image" & GreaseweazleOutputTypeFileExt(OutputType)

            If TempPath = "" Then
                MsgBox(My.Resources.Dialog_TempPathError, MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            TextBoxConsole.Clear()
            _OutputFilePath = GenerateUniqueFileName(TempPath, FileName)

            _TrackStatus.Clear()
            GridReset(_TrackCount, _SideCount)

            Dim DoubleStep As Boolean = DiskParams.IsStandard AndAlso CheckBoxDoublestep.Enabled AndAlso CheckBoxDoublestep.Checked
            _DoubleStep = DoubleStep

            ToggleProcessRunning(True)

            Dim Arguments = GenerateCommandLine(DiskParams, OutputType, DoubleStep)
            Process.StartAsync(GreaseweazleSettings.AppPath, Arguments)
        End Sub

        Private Sub ProcessOutputLine(line As String)
            If TextBoxConsole.Text.Length > 0 Then
                TextBoxConsole.AppendText(Environment.NewLine)
            End If
            TextBoxConsole.AppendText(line)

            Dim TrackInfo = Parser.ParseTrackInfo(line)
            If TrackInfo IsNot Nothing Then
                Dim Statusinfo = UpdateStatusInfo(TrackInfo)
                ProcessTrackStatus(Statusinfo)
                Return
            End If

            Dim TrackInfoUnexpected = Parser.ParseUnexpectedSector(line)
            If TrackInfoUnexpected IsNot Nothing Then
                Dim StatusInfo = UpdateStatusInfo(TrackInfoUnexpected)
                ProcessTrackStatus(StatusInfo)
                Return
            End If
        End Sub

        Private Sub ProcessTrackStatus(StatusInfo As TrackStatusInfo)
            Dim Table = GridGetTable(StatusInfo.Side)

            If Table IsNot Nothing Then
                Dim Track = StatusInfo.Track
                If _DoubleStep Then
                    Track *= 2
                End If

                Dim BackColor As Color
                Dim Text As String = ""
                If StatusInfo.BadSectors > 0 Then
                    BackColor = Color.Red
                    Text = StatusInfo.BadSectors
                ElseIf StatusInfo.UnexpectedSectors.Count > 0 Then
                    BackColor = Color.Yellow
                    Text = StatusInfo.UnexpectedSectors.Count
                Else
                    BackColor = Color.LightGreen
                End If
                Table.SetCell(Track, Text:=Text, BackColor:=BackColor)
                'FloppyGridSetLabel(Table, Track, Text, BackColor)

                StatusTrack.Text = My.Resources.Label_Track & " " & Track
                StatusSide.Text = My.Resources.Label_Side & " " & StatusInfo.Side
                StatusSide.Visible = True

                If _TotalBadSectors = 1 Then
                    _StatusBadSectors.Text = _TotalBadSectors & " " & My.Resources.Label_BadSector
                ElseIf _TotalBadSectors > 1 Then
                    _StatusBadSectors.Text = _TotalBadSectors & " " & My.Resources.Label_BadSectors
                End If

                If _TotalUnexpectedSectors = 1 Then
                    _StatusUnexpected.Text = _TotalUnexpectedSectors & " " & My.Resources.Label_UnexpectedSector
                ElseIf _TotalUnexpectedSectors > 1 Then
                    _StatusUnexpected.Text = _TotalUnexpectedSectors & " " & My.Resources.Label_UnexpectedSectors
                End If
            End If
        End Sub

        Private Sub RefreshButtonState(CheckImageFormat As Boolean)
            Dim ImageParams As FloppyDiskParams = ComboImageFormat.SelectedValue
            Dim OutputTypeDisabled As Boolean = False

            If CheckImageFormat Then
                If ImageParams.IsNonImage Then
                    OutputTypeDisabled = False
                Else
                    Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(ImageParams.Format)
                    OutputTypeDisabled = (ImageFormat = GreaseweazleImageFormat.None)
                End If

                If ComboOutputType.Enabled AndAlso OutputTypeDisabled Then
                    _CachedOutputTypeValue = ComboOutputType.SelectedValue
                    ComboOutputType.SelectedValue = GreaseweazleOutputType.HFE
                ElseIf Not ComboOutputType.Enabled And Not OutputTypeDisabled Then
                    ComboOutputType.SelectedValue = _CachedOutputTypeValue
                End If
            End If

            ComboImageFormat.Enabled = Not _ProcessRunning
            ComboOutputType.Enabled = Not _ProcessRunning And Not OutputTypeDisabled

            ButtonProcess.Enabled = ImageParams.Format <> FloppyDiskFormat.FloppyUnknown
            If _ProcessRunning Then
                ButtonProcess.Text = My.Resources.Label_Abort
            Else
                ButtonProcess.Text = My.Resources.Label_Process
            End If

            If ImageParams.IsStandard AndAlso ImageParams.MediaType = FloppyMediaType.Media525DoubleDensity Then
                CheckBoxDoublestep.Enabled = Not _ProcessRunning AndAlso _TrackCount > 42
                CheckBoxDoublestep.Checked = _TrackCount > 79
            Else
                CheckBoxDoublestep.Enabled = False
                CheckBoxDoublestep.Checked = False
            End If

            RefreshImportButtonState()
        End Sub

        Private Sub RefreshImportButtonState()
            ButtonOk.Enabled = Not _ProcessRunning AndAlso Not String.IsNullOrEmpty(_OutputFilePath) AndAlso Not String.IsNullOrEmpty(TextBoxFileName.Text)
        End Sub

        Private Sub ResetStatusBar()
            ClearStatusBar()
            _StatusBadSectors.Text = ""
            _StatusUnexpected.Text = ""
            _TotalBadSectors = 0
            _TotalUnexpectedSectors = 0
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

        Private Sub SetTilebarText()
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

        Private Function UpdateStatusInfo(TrackInfo As ConsoleOutputParser.TrackInfo) As TrackStatusInfo
            Dim Key = TrackInfo.SourceTrack & "." & TrackInfo.SourceSide
            Dim StatusInfo As TrackStatusInfo

            If _TrackStatus.ContainsKey(Key) Then
                StatusInfo = _TrackStatus.Item(Key)
            Else
                StatusInfo = New TrackStatusInfo With {
                    .Track = TrackInfo.SourceTrack,
                    .Side = TrackInfo.SourceSide
                }
                _TrackStatus.Add(Key, StatusInfo)
            End If
            StatusInfo.BadSectors += TrackInfo.BadSectors
            _TotalBadSectors += TrackInfo.BadSectors

            Return StatusInfo
        End Function

        Private Function UpdateStatusInfo(TrackInfo As ConsoleOutputParser.UnexpectedSector) As TrackStatusInfo
            Dim Key = TrackInfo.SourceTrack & "." & TrackInfo.SourceSide
            Dim StatusInfo As TrackStatusInfo

            If _TrackStatus.ContainsKey(Key) Then
                StatusInfo = _TrackStatus.Item(Key)
            Else
                StatusInfo = New TrackStatusInfo With {
                    .Track = TrackInfo.SourceTrack,
                    .Side = TrackInfo.SourceSide
                }
                _TrackStatus.Add(Key, StatusInfo)
            End If
            If Not StatusInfo.UnexpectedSectors.ContainsKey(TrackInfo.Key) Then
                StatusInfo.UnexpectedSectors.Add(TrackInfo.Key, TrackInfo)
                _TotalUnexpectedSectors += 1
            End If

            Return StatusInfo
        End Function

        Protected Overrides Sub OnAfterBaseFormClosing(e As FormClosingEventArgs)
            If e.CloseReason = CloseReason.UserClosing OrElse CancelButtonClicked Then
                ClearOutputFile()
            End If
        End Sub

#Region "Events"
        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If CancelProcessIfRunning() Then
                Exit Sub
            End If

            ProcessImage()
        End Sub

        Private Sub ComboImageFormat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageFormat.SelectedIndexChanged
            RefreshButtonState(True)
        End Sub

        Private Sub Process_ErrorDataReceived(data As String) Handles Process.ErrorDataReceived
            ProcessOutputLine(data)
        End Sub

        Private Sub Process_ProcessExited(exitCode As Integer) Handles Process.ProcessExited
            If exitCode = -1 Then
                ClearOutputFile()
                StatusType.Text = My.Resources.Label_Aborted
            Else
                StatusType.Text = My.Resources.Label_Complete
            End If
            ToggleProcessRunning(False)
        End Sub

        Private Sub Process_ProcessFailed(message As String, ex As Exception) Handles Process.ProcessFailed
            StatusType.Text = My.Resources.Label_Failed
            ToggleProcessRunning(False)
        End Sub

        Private Sub Process_ProcessStarted(exePath As String, arguments As String) Handles Process.ProcessStarted
            StatusType.Text = My.Resources.Label_Processing
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
        Private Class TrackStatusInfo
            Public Sub New()
                _UnexpectedSectors = New Dictionary(Of String, ConsoleOutputParser.UnexpectedSector)
            End Sub

            Public Property BadSectors As UShort = 0
            Public Property Side As Integer
            Public Property Track As Integer
            Public Property UnexpectedSectors As Dictionary(Of String, ConsoleOutputParser.UnexpectedSector)
        End Class
    End Class
End Namespace
