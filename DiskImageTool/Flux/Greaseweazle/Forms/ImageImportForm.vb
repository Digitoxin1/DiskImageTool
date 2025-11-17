Imports System.ComponentModel
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Public Class ImageImportForm
        Inherits BaseForm

        Private WithEvents ButtonProcess As Button
        Private WithEvents CheckBoxDoublestep As CheckBox
        Private WithEvents ComboImageFormat As ComboBox
        Private WithEvents ComboOutputType As ComboBox
        Private WithEvents TextBoxFileName As TextBox
        Private ReadOnly _Initialized As Boolean = False
        Private ReadOnly _InputFilePath As String
        Private ReadOnly _SideCount As Integer
        Private ReadOnly _TrackCount As Integer
        Private ReadOnly _TrackStatus As TrackStatus
        Private _CachedOutputTypeValue As GreaseweazleOutputType = GreaseweazleOutputType.IMA
        Private _DoubleStep As Boolean = False
        Private _OutputFilePath As String = ""
        Private _ProcessRunning As Boolean = False

        Public Sub New(FilePath As String, TrackCount As Integer, SideCount As Integer)
            MyBase.New(GreaseweazleSettings.LogFileName)

            _TrackStatus = New TrackStatus(Me)

            _InputFilePath = FilePath
            _TrackCount = TrackCount
            _SideCount = SideCount
            GridReset(_TrackCount, _SideCount)
            InitializeControls()
            Dim ImageFormat = ReadImageFormat()
            PopulateOutputTypes()
            PopulateImageFormats(ComboImageFormat, ImageFormat, ImageFormat)

            SetNewFileName()
            SetTiltebarText()
            ClearStatusBar()
            RefreshButtonState(True)

            _Initialized = True
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

        Protected Overrides Sub OnAfterBaseFormClosing(e As FormClosingEventArgs)
            If e.CloseReason = CloseReason.UserClosing OrElse CancelButtonClicked Then
                ClearOutputFile()
            End If
        End Sub

        Private Sub ClearOutputFile()
            If Not String.IsNullOrEmpty(_OutputFilePath) Then
                DeleteFileIfExists(_OutputFilePath)
            End If
            _OutputFilePath = ""
        End Sub

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

            ButtonProcess = New Button With {
                .Width = 75,
                .Margin = New Padding(12, 24, 3, 3)
            }

            ButtonOk.Text = My.Resources.Label_Import
            ButtonOk.Visible = True

            Dim Row As Integer

            With TableLayoutPanelMain
                .SuspendLayout()

                .Left = 0
                .RowCount = 3
                .ColumnCount = 5
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
                '.Left = (.Parent.ClientSize.Width - .Width) \ 2
            End With
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
            ClearStatusBar()

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

            Dim TrackInfo = _TrackStatus.ParseTrackInfoRead(line)
            If TrackInfo IsNot Nothing Then
                Dim Statusinfo = _TrackStatus.UpdateStatusInfo(TrackInfo, True, TrackStatus.ActionTypeEnum.Import)
                _TrackStatus.UpdateTrackStatus(Statusinfo, TrackStatus.ActionTypeEnum.Read, _DoubleStep)
                Return
            End If

            Dim TrackInfoUnexpected = _TrackStatus.ParseUnexpectedSector(line)
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

            ButtonSaveLog.Enabled = Not _ProcessRunning AndAlso TextBoxConsole.Text.Length > 0

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
        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If CancelProcessIfRunning() Then
                Exit Sub
            End If

            ProcessImage()
        End Sub

        Private Sub ComboImageFormat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageFormat.SelectedIndexChanged
            If Not _Initialized Then
                Exit Sub
            End If

            RefreshButtonState(True)
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
