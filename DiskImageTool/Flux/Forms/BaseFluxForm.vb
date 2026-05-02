Imports DiskImageTool.Flux.Greaseweazle
Imports Greaseweazle.Actions
Imports Greaseweazle.Tools

Namespace Flux
    Public MustInherit Class BaseFluxForm
        Protected WithEvents Process As ConsoleProcessRunner
        Protected WithEvents Runner As GreaseweazleRunner
        Private WithEvents TS As ITrackStatus = Nothing
        Private WithEvents TS0 As FloppyTrackGrid
        Private WithEvents TS1 As FloppyTrackGrid
        Private Const TOTAL_TRACKS As UShort = 84
        Private ReadOnly _Engine As New GreaseweazleEngine()
        Private ReadOnly _HelpProvider As New HelpProvider()
        Private _FluxHeaders As Dictionary(Of TrackSide, Dictionary(Of String, String))
        Private _LogFileName As String = ""
        Private _LogFilePath As String = ""
        Private _LogStripPath As Boolean = False
        Private _Sides As Byte
        Private _StatusData As TrackStatusData? = Nothing
        Private _Tracks As UShort

        Protected Event CheckChanged(sender As Object, Checked As Boolean, Side As Byte)
        Protected Event SelectionChanged(sender As Object, Track As UShort, Side As Byte, Enabled As Boolean)

        Public Sub New(LogFileName As String, Optional UseGrid As Boolean = True, Optional UseProcess As Boolean = False)
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            LocalizeForm()
            _LogFileName = LogFileName
            StatusDevice.Visible = False

            Runner = New GreaseweazleRunner()

            If UseProcess Then
                Process = New ConsoleProcessRunner With {
                    .EventContext = Threading.SynchronizationContext.Current
                }
            End If

            If UseGrid Then
                TS0 = New FloppyTrackGrid(TOTAL_TRACKS, 0) With {
                .Anchor = AnchorStyles.Top Or AnchorStyles.Right
            }
                TS1 = New FloppyTrackGrid(TOTAL_TRACKS, 1) With {
                    .Anchor = AnchorStyles.Top Or AnchorStyles.Right,
                    .Margin = New Padding(32, 3, 3, 3)
                }
            End If
        End Sub

        Friend Property TrackStatus As ITrackStatus
            Get
                Return TS
            End Get
            Set(value As ITrackStatus)
                TS = value
            End Set
        End Property

        Protected ReadOnly Property Engine As GreaseweazleEngine
            Get
                Return _Engine
            End Get
        End Property

        Protected ReadOnly Property HasSelectedTracks As Boolean
            Get
                Return TS0.SelectedTracks.Count > 0 OrElse TS1.SelectedTracks.Count > 0
            End Get
        End Property

        Protected ReadOnly Property IsIdle As Boolean
            Get
                Return Not IsRunning
            End Get
        End Property

        Protected ReadOnly Property IsRunning As Boolean
            Get
                Return (Runner IsNot Nothing AndAlso Runner.IsRunning) OrElse (Process IsNot Nothing AndAlso Process.IsRunning)
            End Get
        End Property

        Protected Property LogFileName As String
            Get
                Return _LogFileName
            End Get
            Set(value As String)
                _LogFileName = value
            End Set
        End Property

        Protected Property LogStripPath As Boolean
            Get
                Return _LogStripPath
            End Get
            Set
                _LogStripPath = Value
            End Set
        End Property

        Protected ReadOnly Property TableSide0 As FloppyTrackGrid
            Get
                Return TS0
            End Get
        End Property

        Protected ReadOnly Property TableSide1 As FloppyTrackGrid
            Get
                Return TS1
            End Get
        End Property

        Friend Function GetSelectedTrackHeads() As TrackHeads
            If TS0.IsChecked AndAlso TS1.IsChecked Then
                Return TrackHeads.both
            ElseIf TS0.IsChecked Then
                Return TrackHeads.head0
            Else
                Return TrackHeads.head1
            End If
        End Function

        Friend Sub GridReset(Tracks As UShort, Sides As Byte, Optional FluxHeaders As Dictionary(Of TrackSide, Dictionary(Of String, String)) = Nothing, Optional ResetSelected As Boolean = True)
            _Tracks = Tracks
            _Sides = Sides

            GridResetTracks(TS0, Tracks, False, ResetSelected)
            GridResetTracks(TS1, Tracks, Sides < 2, ResetSelected)

            If _FluxHeaders IsNot FluxHeaders Then
                _FluxHeaders = FluxHeaders
                GridUpdateFluxHeaders()
            End If
        End Sub

        Protected Function CancelIfRunning() As Boolean
            If (Process IsNot Nothing AndAlso Process.IsRunning) OrElse (Runner IsNot Nothing AndAlso Runner.IsRunning) Then
                If Not ConfirmCancel() Then
                    Return True
                End If
                If (Process IsNot Nothing AndAlso Process.IsRunning) Then
                    Process.Cancel()
                End If

                If (Runner IsNot Nothing AndAlso Runner.IsRunning) Then
                    Runner.Cancel()
                End If
                Return True
            End If

            Return False
        End Function

        Protected Sub CheckStateChanged(Checked As Boolean)
            If Checked Then
                GridReset()
            End If

            TS0.SelectEnabled = Checked
            TS1.SelectEnabled = Checked
        End Sub

        Protected Sub ClearLogAndStatus()
            ClearStatusBar()
            TextBoxConsole.Clear()
        End Sub

        Protected Sub ClearStatusBar()
            _StatusData = Nothing
            StatusDevice.Text = ""
            StatusDevice.Visible = False
            StatusType.Text = ""
            StatusTrack.Text = ""
            StatusSide.Text = ""
            StatusSide.Visible = False
            StatusBadSectors.Text = ""
            StatusBadSectors.Visible = False
            StatusUnexpected.Text = ""
            StatusUnexpected.Visible = False
            ToolStripProgress.Visible = False
        End Sub

        Protected Function ConfirmWrite(Title As String, DriveName As String) As Boolean
            Dim Msg = String.Format(My.Resources.Dialog_ConfirmWrite, Environment.NewLine, DriveName, Title)
            Return MsgBox(Msg, MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2 + MsgBoxStyle.Exclamation, Title) = MsgBoxResult.Yes
        End Function

        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing Then
                    Try
                        Runner?.Dispose()
                    Catch
                    End Try
                    Runner = Nothing

                    Try
                        Process?.Dispose()
                    Catch
                    End Try
                    Process = Nothing

                    components?.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        Protected Sub FillAutoSizeStyles()
            With TableLayoutPanelMain
                While .RowStyles.Count < .RowCount
                    .RowStyles.Add(New RowStyle())
                End While

                For i = 0 To .RowCount - 1
                    .RowStyles(i).SizeType = SizeType.AutoSize
                Next

                While .ColumnStyles.Count < .ColumnCount
                    .ColumnStyles.Add(New ColumnStyle())
                End While

                For j = 0 To .ColumnCount - 1
                    .ColumnStyles(j).SizeType = SizeType.AutoSize
                Next
            End With
        End Sub

        Protected Function GetDriveName(Drive As String) As String
            Select Case Drive
                Case "A", "B"
                    Return Drive & ":"
                Case Else
                    Return "DS" & Drive & ":"
            End Select
        End Function

        Protected Function GetSelectedTrackRanges() As List(Of (StartTrack As UShort, EndTrack As UShort))
            Dim Selected As New HashSet(Of UShort)(TS0.SelectedTracks)
            Selected.UnionWith(TS1.SelectedTracks)

            Dim Ranges = BuildRanges(Selected)

            Return Ranges
        End Function

        Protected Function GetState(Optional Doublestep As Boolean = False) As FluxFormState
            Dim State As New FluxFormState With {
                .LogFileName = _LogFileName,
                .LogStripPath = _LogStripPath,
                .Side0State = TS0.GetState(Doublestep),
                .Side1State = TS1.GetState(Doublestep),
                .StatusData = _StatusData,
                .Device = StatusDevice.Text,
                .Log = TextBoxConsole.Text
            }

            Return State
        End Function

        Protected Sub GreaseweazleReset()
            TextBoxConsole.Clear()

            Try
                Engine.Reset.Run(New ResetOptions With {
                    .Live = True,
                    .Device = ConfiguredDevice()
                })

                MsgBox(My.Resources.Dialog_GreaseweazleReset, MsgBoxStyle.Information)
            Catch ex As Exception
                HandleRunFailure(ex.Message)
            End Try
        End Sub

        Protected Sub GridHideSelection(Value As Boolean)
            TS0.HideSelection = Value
            TS0.IsBusy = Value

            TS1.HideSelection = Value
            TS1.IsBusy = Value
        End Sub

        Protected Sub GridResetSelectedCells()
            TS0.ResetSelectedCells()
            TS1.ResetSelectedCells()
        End Sub

        Protected Sub GridSelectEnabled(Value As Boolean)
            TS0.SelectEnabled = Value
            TS1.SelectEnabled = Value
        End Sub

        Protected Sub GridSetBusy(Busy As Boolean)
            If TS0 IsNot Nothing Then
                TS0.IsBusy = Busy
            End If

            If TS1 IsNot Nothing Then
                TS1.IsBusy = Busy
            End If
        End Sub

        Protected Sub HandleRunFailure(message As String)
            Dim Msg = If(String.IsNullOrEmpty(message), "Unknown error", message)

            AppendLogLine("Command Failed: " & Msg)
        End Sub

        Protected Sub InitLogFilePath(FilePath As String, Optional Append As Boolean = False)
            _LogFilePath = FilePath

            If Not Append AndAlso Not String.IsNullOrEmpty(_LogFilePath) Then
                DeleteFileIfExists(_LogFilePath)
            End If
        End Sub

        Protected Overridable Sub OnAfterBaseFormClosing(e As FormClosingEventArgs)
            ' Default: do nothing
        End Sub

        Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
            If (Process IsNot Nothing AndAlso Process.IsRunning) OrElse (Runner IsNot Nothing AndAlso Runner.IsRunning) Then
                If (e.CloseReason = CloseReason.UserClosing OrElse e.CloseReason = CloseReason.None) AndAlso (DialogResult = DialogResult.Cancel OrElse DialogResult = DialogResult.None) Then
                    If Not ConfirmCancel() Then
                        e.Cancel = True
                        Exit Sub
                    End If
                End If
                Try
                    If Process IsNot Nothing AndAlso Process.IsRunning Then
                        Process.Cancel()
                    End If
                    If Runner IsNot Nothing AndAlso Runner.IsRunning Then
                        Runner.Cancel()
                    End If
                Catch ex As Exception
                End Try
            End If

            OnAfterBaseFormClosing(e)

            If e.Cancel Then
                Exit Sub
            End If

            MyBase.OnFormClosing(e)
        End Sub

        Protected Sub RefreshSaveLogButtonState()
            ButtonSaveLog.Enabled = IsIdle AndAlso TextBoxConsole.TextLength > 0
        End Sub

        Protected Overridable Sub SaveLog(RemovePath As Boolean, Optional InitialDirectory As String = "")
            Dim FileName As String = _LogFileName
            Dim Extension = IO.Path.GetExtension(FileName).ToLower
            Dim FilterIndex As Integer = 1
            If Extension <> ".txt" Then
                FilterIndex = 2
            End If

            Using Dialog As New SaveFileDialog With {
                   .FileName = FileName,
                   .DefaultExt = "txt",
                   .Filter = My.Resources.FileType_Text & " (*.txt)|*.txt|" & My.Resources.FileType_All & " (*.*)|*.*",
                   .FilterIndex = FilterIndex,
                   .InitialDirectory = InitialDirectory,
                   .RestoreDirectory = True
                }

                If Dialog.ShowDialog = DialogResult.OK Then
                    SaveLogFile(Dialog.FileName, TextBoxConsole.Text, RemovePath)
                End If
            End Using
        End Sub

        Protected Sub SetHelpString(HelpString As String, ParamArray ControlArray() As Control)
            For Each Control In ControlArray
                _HelpProvider.SetHelpString(Control, HelpString.Replace("\t", vbTab))
                _HelpProvider.SetShowHelp(Control, True)
            Next
        End Sub

        Protected Sub SetRowVisible(row As Integer, visible As Boolean)
            If visible Then
                TableLayoutPanelMain.RowStyles(row).SizeType = SizeType.AutoSize
            Else
                TableLayoutPanelMain.RowStyles(row).SizeType = SizeType.Absolute
                TableLayoutPanelMain.RowStyles(row).Height = 0
            End If

            For Each c As Control In TableLayoutPanelMain.Controls
                If TableLayoutPanelMain.GetRow(c) = row Then
                    c.Visible = visible
                End If
            Next
        End Sub

        Protected Sub SetState(State As FluxFormState, Optional Doublestep As Boolean = False)
            _LogFileName = State.LogFileName
            _LogStripPath = State.LogStripPath

            TS0.SetState(State.Side0State, Doublestep)
            TS1.SetState(State.Side1State, Doublestep)

            If State.StatusData.HasValue Then
                UpdateStatus(State.StatusData.Value)
            Else
                ClearStatusBar()
            End If

            StatusDevice.Text = State.Device
            StatusDevice.Visible = Not String.IsNullOrEmpty(State.Device)

            TextBoxConsole.Text = State.Log
        End Sub

        Protected Function TryMakeDriveSpec(Id As String, ByRef Drive As DriveSpec) As Boolean
            Try
                Drive = MakeDriveSpec(Id)
                Return True
            Catch ex As Exception
                HandleRunFailure(ex.Message)
                Return False
            End Try
        End Function

        Private Sub AppendLogLine(line As String)
            If TextBoxConsole.Text.Length > 0 Then
                TextBoxConsole.AppendText(Environment.NewLine)
            End If
            TextBoxConsole.AppendText(line)

            If Not String.IsNullOrEmpty(_LogFilePath) Then
                Try
                    IO.File.AppendAllText(_LogFilePath, line & Environment.NewLine)
                Catch ex As Exception
                    Debug.WriteLine($"AppendLogLine failed: {ex.Message}")
                End Try
            End If
        End Sub

        Private Function ConfirmCancel() As Boolean
            Return MsgBox(My.Resources.Dialog_ConfirmCancel, MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2 + MsgBoxStyle.Exclamation) = MsgBoxResult.Yes
        End Function

        Private Function GridGetTable(Side As Byte) As FloppyTrackGrid
            If Side = 0 Then
                Return TS0
            ElseIf Side = 1 Then
                Return TS1
            Else
                Return Nothing
            End If
        End Function

        Private Sub GridMarkTrack(StatusData As TrackStatusData)
            Dim Table = GridGetTable(StatusData.Side)
            Dim Track = StatusData.Track

            Table?.SetCell(Track, Text:=StatusData.CellText, BackColor:=StatusData.BackColor, ForeColor:=StatusData.ForeColor, Tooltip:=StatusData.Tooltip)
        End Sub

        Private Sub GridReset()
            GridReset(_Tracks, _Sides, _FluxHeaders)
        End Sub
        Private Sub GridResetTracks(Grid As FloppyTrackGrid, Tracks As UShort, Disabled As Boolean, Optional ResetSelected As Boolean = True)
            Grid.ActiveTrackCount = Tracks
            Grid.ResetAll()

            If ResetSelected Then
                Grid.IsChecked = False
            End If
            Grid.Disabled = Disabled
        End Sub

        Private Sub GridUpdateFluxHeaders()
            For Track = 0 To _Tracks - 1
                For Side = 0 To _Sides - 1
                    Dim Table = GridGetTable(Side)

                    If Table IsNot Nothing Then
                        Dim Name As String = ""
                        Dim Version As String = ""
                        Dim HostDate As String = ""
                        Dim HostTime As String = ""
                        If _FluxHeaders IsNot Nothing Then
                            Dim Headers As Dictionary(Of String, String) = Nothing
                            _FluxHeaders.TryGetValue(New TrackSide(Track, Side), Headers)
                            If Headers IsNot Nothing Then
                                Headers.TryGetValue("name", Name)
                                Headers.TryGetValue("version", Version)
                                Headers.TryGetValue("host_date", HostDate)
                                Headers.TryGetValue("host_time", HostTime)
                            End If
                        End If
                        Table.SetCellFluxHeader(Track, Name, Version, HostDate, HostTime)
                    End If
                Next
            Next
        End Sub

        Private Sub GridUpdateTooltip(Track As Integer, Side As Integer, Tooltip As String)
            Dim Table = GridGetTable(Side)

            Table?.SetCellTooltip(Track, Tooltip)
        End Sub

        Private Sub LocalizeForm()
            ButtonCancel.Text = My.Resources.Menu_Cancel
            ButtonSaveLog.Text = My.Resources.Label_SaveLog
            ButtonReset.Text = My.Resources.Label_Reset
            LabelConsoleOutput.Text = My.Resources.Label_ConsoleOutput

            SetHelpString(My.Resources.HelpStrings.Greaseweazle_DeviceReset, ButtonReset)
        End Sub

        Private Sub UpdateStatus(StatusData As TrackStatusData)
            _StatusData = StatusData

            ToolStripProgress.Visible = False
            StatusType.Text = StatusData.StatusText
            StatusTrack.Text = My.Resources.Label_Track & " " & StatusData.Track
            StatusSide.Text = My.Resources.Label_Side & " " & StatusData.Side
            StatusSide.Visible = StatusData.SideVisible

            If StatusData.TotalBadSectors = 1 Then
                StatusBadSectors.Text = StatusData.TotalBadSectors & " " & My.Resources.Label_BadSector
                StatusBadSectors.Visible = True
            ElseIf StatusData.TotalBadSectors > 1 Then
                StatusBadSectors.Text = StatusData.TotalBadSectors & " " & My.Resources.Label_BadSectors
                StatusBadSectors.Visible = True
            Else
                StatusBadSectors.Visible = False
            End If

            If StatusData.TotalUnexpectedSectors = 1 Then
                StatusUnexpected.Text = StatusData.TotalUnexpectedSectors & " " & My.Resources.Label_UnexpectedSector
                StatusUnexpected.Visible = True
            ElseIf StatusData.TotalUnexpectedSectors > 1 Then
                StatusUnexpected.Text = StatusData.TotalUnexpectedSectors & " " & My.Resources.Label_UnexpectedSectors
                StatusUnexpected.Visible = True
            Else
                StatusUnexpected.Visible = False
            End If
        End Sub

        Private Sub UpdateTrackStatusType(Text As String)
            If _StatusData.HasValue Then
                Dim StatusData = _StatusData.Value
                StatusData.StatusText = Text
                _StatusData = StatusData
            End If

            StatusType.Text = Text
        End Sub
#Region "Events"
        Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
            GreaseweazleReset()
        End Sub

        Private Sub ButtonSaveLog_Click(sender As Object, e As EventArgs) Handles ButtonSaveLog.Click
            SaveLog(_LogStripPath)
        End Sub

        Private Sub Runner_OutputDataReceived(line As String) Handles Runner.OutputDataReceived
            AppendLogLine(line)
        End Sub

        Private Sub Runner_OutputTextReceived(text As String) Handles Runner.OutputTextReceived
            TextBoxConsole.AppendText(text)
        End Sub

        Private Sub Runner_ProcessFailed(ex As Exception) Handles Runner.ProcessFailed
            HandleRunFailure(ex.Message)
        End Sub

        Private Sub TS_UpdateGridTooltip(Track As Integer, Side As Integer, Tooltip As String) Handles TS.UpdateGridTooltip
            GridUpdateTooltip(Track, Side, Tooltip)
        End Sub

        Private Sub TS_UpdateGridTrack(StatusData As TrackStatusData) Handles TS.UpdateGridTrack
            GridMarkTrack(StatusData)
        End Sub

        Private Sub TS_UpdateStatus(StatusData As TrackStatusData) Handles TS.UpdateStatus
            GridMarkTrack(StatusData)
            UpdateStatus(StatusData)
        End Sub

        Private Sub TS_UpdateStatusType(StatusText As String) Handles TS.UpdateStatusType
            UpdateTrackStatusType(StatusText)
        End Sub

        Private Sub TS0_CheckChanged(sender As Object, Checked As Boolean) Handles TS0.CheckChanged
            If Checked Then
                TS0.SetCellsSelected(TS1.SelectedTracks, Checked)
            End If

            RaiseEvent CheckChanged(Me, Checked, 0)
        End Sub

        Private Sub TS0_SelectionChanged(sender As Object, Track As Integer, Selected As Boolean) Handles TS0.SelectionChanged
            If TS1.SelectEnabled Then
                If TS1.IsChecked Then
                    TS1.SetCellSelected(Track, Selected, True)
                End If
            End If

            RaiseEvent SelectionChanged(Me, Track, 0, Selected)
        End Sub

        Private Sub TS1_CheckChanged(sender As Object, Checked As Boolean) Handles TS1.CheckChanged
            If Checked Then
                TS1.SetCellsSelected(TS0.SelectedTracks, Checked)
            End If

            RaiseEvent CheckChanged(Me, Checked, 1)
        End Sub

        Private Sub TS1_SelectionChanged(sender As Object, Track As Integer, Selected As Boolean) Handles TS1.SelectionChanged
            If TS0.SelectEnabled Then
                If TS0.IsChecked Then
                    TS0.SetCellSelected(Track, Selected, True)
                End If
            End If

            RaiseEvent SelectionChanged(Me, Track, 1, Selected)
        End Sub
#End Region
        Public Structure FluxFormState
            Public Property Device As String
            Public Property Log As String
            Public Property LogFileName As String
            Public Property LogStripPath As Boolean
            Public Property Side0State As FloppyTrackGrid.TrackState
            Public Property Side1State As FloppyTrackGrid.TrackState
            Public Property StatusData As TrackStatusData?
        End Structure

        Public Structure TrackStatusData
            Public BackColor As Color
            Public CellText As String
            Public ForeColor As Color
            Public Side As Integer
            Public SideVisible As Boolean
            Public StatusText As String
            Public Tooltip As String
            Public TotalBadSectors As UInteger
            Public TotalUnexpectedSectors As UInteger
            Public Track As Integer
        End Structure
    End Class

End Namespace
