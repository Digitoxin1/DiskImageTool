Namespace Flux
    Public Class BaseForm
        Friend WithEvents Process As ConsoleProcessRunner
        Private WithEvents TS As ITrackStatus = Nothing
        Private WithEvents TS0 As FloppyTrackGrid
        Private WithEvents TS1 As FloppyTrackGrid
        Private Const TOTAL_TRACKS As UShort = 84
        Private _CancelButtonClicked As Boolean = False
        Private _LogFileName As String
        Private _Sides As Byte
        Private _Tracks As UShort
        Public Event CheckChanged(sender As Object, Checked As Boolean, Side As Byte)
        Public Event SelectionChanged(sender As Object, Track As UShort, Side As Byte, Enabled As Boolean)

        Public Sub New(LogFileName As String, Optional UseGrid As Boolean = True)
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            _LogFileName = LogFileName

            Process = New ConsoleProcessRunner With {
                .EventContext = Threading.SynchronizationContext.Current
            }

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

        Public ReadOnly Property CancelButtonClicked As Boolean
            Get
                Return _CancelButtonClicked
            End Get
        End Property

        Public Property LogFileName As String
            Get
                Return _LogFileName
            End Get
            Set(value As String)
                _LogFileName = value
            End Set
        End Property
        Public ReadOnly Property TableSide0 As FloppyTrackGrid
            Get
                Return TS0
            End Get
        End Property

        Public ReadOnly Property TableSide1 As FloppyTrackGrid
            Get
                Return TS1
            End Get
        End Property

        Friend Property TrackStatus As ITrackStatus
            Get
                Return TS
            End Get
            Set(value As ITrackStatus)
                TS = value
            End Set
        End Property

        Public Function CancelProcessIfRunning() As Boolean
            If Process.IsRunning Then
                If Not ConfirmCancel() Then
                    Return True
                End If
                Process.Cancel()
                Return True
            End If

            Return False
        End Function

        Public Sub ClearStatusBar()
            StatusType.Text = ""
            StatusTrack.Text = ""
            StatusSide.Text = ""
            StatusSide.Visible = False
            StatusBadSectors.Text = ""
            StatusBadSectors.Visible = False
            StatusUnexpected.Text = ""
            StatusUnexpected.Visible = False
        End Sub

        Public Function ConfirmWrite(Title As String, DriveName As String) As Boolean
            Dim Msg = String.Format(My.Resources.Dialog_ConfirmWrite, vbNewLine, DriveName, Title)
            Return MsgBox(Msg, MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2 + MsgBoxStyle.Exclamation, Title) = MsgBoxResult.Yes
        End Function

        Public Function GetDriveName(Drive As String) As String
            Select Case Drive
                Case "A", "B"
                    Return Drive & ":"
                Case Else
                    Return "DS" & Drive & ":"
            End Select
        End Function

        Public Sub GridMarkTrack(StatusData As TrackStatusData)
            Dim Table = GridGetTable(StatusData.Side)
            Dim Track = StatusData.Track

            Table?.SetCell(Track, Text:=StatusData.CellText, BackColor:=StatusData.BackColor, ForeColor:=StatusData.ForeColor, Tooltip:=StatusData.Tooltip)
        End Sub

        Public Sub GridReset()
            GridReset(_Tracks, _Sides)
        End Sub

        Public Sub GridReset(Tracks As UShort, Sides As Byte, Optional ResetSelected As Boolean = True)
            _Tracks = Tracks
            _Sides = Sides

            GridResetTracks(TS0, Tracks, False, ResetSelected)
            GridResetTracks(TS1, Tracks, Sides < 2, ResetSelected)
        End Sub

        Public Sub SaveLog()
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
                   .FilterIndex = FilterIndex
                }

                If Dialog.ShowDialog = DialogResult.OK Then
                    IO.File.WriteAllText(Dialog.FileName, TextBoxConsole.Text & vbNewLine)
                End If
            End Using
        End Sub

        Public Sub UpdateStatus(StatusData As TrackStatusData)
            GridMarkTrack(StatusData)

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

        Public Sub UpdateTrackStatusType(Text As String)
            StatusType.Text = Text
        End Sub
        Protected Overridable Sub OnAfterBaseFormClosing(e As FormClosingEventArgs)
            ' Default: do nothing
        End Sub

        Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
            If Process.IsRunning Then
                If e.CloseReason = CloseReason.UserClosing OrElse _CancelButtonClicked Then
                    If Not ConfirmCancel() Then
                        e.Cancel = True
                        _CancelButtonClicked = False
                        Exit Sub
                    End If
                End If
                Try
                    Process.Cancel()
                Catch ex As Exception
                End Try
            End If

            If e.Cancel Then
                _CancelButtonClicked = False
                Return
            End If

            OnAfterBaseFormClosing(e)

            _CancelButtonClicked = False

            If e.Cancel Then
                Return
            End If

            MyBase.OnFormClosing(e)
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

        Private Sub GridResetTracks(Grid As FloppyTrackGrid, Tracks As UShort, Disabled As Boolean, Optional ResetSelected As Boolean = True)
            Grid.ActiveTrackCount = Tracks
            Grid.ResetAll()

            If ResetSelected Then
                Grid.IsChecked = False
            End If
            Grid.Disabled = Disabled
        End Sub

#Region "Events"
        Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
            _CancelButtonClicked = True
        End Sub

        Private Sub ButtonSaveLog_Click(sender As Object, e As EventArgs) Handles ButtonSaveLog.Click
            SaveLog()
        End Sub

        Private Sub TS_UpdateGridTrack(StatusData As TrackStatusData) Handles TS.UpdateGridTrack
            GridMarkTrack(StatusData)
        End Sub

        Private Sub TS_UpdateStatus(StatusData As TrackStatusData) Handles TS.UpdateStatus
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
