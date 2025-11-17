Namespace Flux
    Public Class BaseForm
        Friend WithEvents Process As ConsoleProcessRunner
        Private WithEvents TS0 As FloppyTrackGrid
        Private WithEvents TS1 As FloppyTrackGrid
        Private Const TOTAL_TRACKS As UShort = 84
        Private ReadOnly _LogFileName As String
        Private _CancelButtonClicked As Boolean = False
        Private _Sides As Byte
        Private _TotalBadSectors As UInteger = 0
        Private _TotalUnexpectedSectors As UInteger = 0
        Private _Tracks As UShort

        Public Enum ActionTypeEnum
            Read
            Write
            [Erase]
            Import
        End Enum

        Public Enum TrackStatusEnum
            Reading
            Erasing
            Writing
            Retry
            Failed
            Success
            Aborted
            [Error]
        End Enum

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
                TS0 = New FloppyTrackGrid(TOTAL_TRACKS, My.Resources.Label_Side & " 0") With {
                .Anchor = AnchorStyles.Top Or AnchorStyles.Right
            }
                TS1 = New FloppyTrackGrid(TOTAL_TRACKS, My.Resources.Label_Side & " 1") With {
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

        Public Property TotalBadSectors As UInteger
            Get
                Return _TotalBadSectors
            End Get
            Set(value As UInteger)
                _TotalBadSectors = value
            End Set
        End Property

        Public Property TotalUnexpectedSectors As UInteger
            Get
                Return _TotalUnexpectedSectors
            End Get
            Set(value As UInteger)
                _TotalUnexpectedSectors = value
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
            _TotalBadSectors = 0
            _TotalUnexpectedSectors = 0
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

        Public Function GetTrackStatusText(Status As TrackStatusEnum, Optional Retries As UShort = 0) As String
            Select Case Status
                Case TrackStatusEnum.Reading
                    Return My.Resources.Label_Reading
                Case TrackStatusEnum.Erasing
                    Return My.Resources.Label_Erasing
                Case TrackStatusEnum.Writing
                    Return My.Resources.Label_Writing
                Case TrackStatusEnum.Retry
                    Return My.Resources.Label_Retrying & IIf(Retries > 0, " " & Retries, "")
                Case TrackStatusEnum.Failed
                    Return My.Resources.Label_Failed
                Case TrackStatusEnum.Success
                    Return My.Resources.Label_Complete
                Case TrackStatusEnum.Aborted
                    Return My.Resources.Label_Aborted
                Case TrackStatusEnum.Error
                    Return My.Resources.Label_Error
                Case Else
                    Return ""
            End Select
        End Function

        Public Sub GridMarkTrack(Track As Integer, Side As Integer, Status As TrackStatusEnum, Label As String, DoubleStep As Boolean)
            Dim Table = GridGetTable(Side)

            If Table IsNot Nothing Then
                Dim Colors = GetTrackStatusColor(Status)
                If DoubleStep Then
                    Track *= 2
                End If
                Table.SetCell(Track, Text:=Label, BackColor:=Colors.BackColor, ForeColor:=Colors.ForeColor)
            End If
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

        Public Sub UpdateTrackStatusError()
            StatusType.Text = GetTrackStatusText(TrackStatusEnum.Error)
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

        Private Function GetTrackStatusColor(Status As TrackStatusEnum) As (ForeColor As Color, BackColor As Color)
            Select Case Status
                Case TrackStatusEnum.Reading
                    Return (Color.Black, Color.LightBlue)
                Case TrackStatusEnum.Erasing
                    Return (Color.Black, Color.MediumPurple)
                Case TrackStatusEnum.Writing
                    Return (Color.Black, Color.Orange)
                Case TrackStatusEnum.Retry
                    Return (Color.Black, Color.Yellow)
                Case TrackStatusEnum.Failed
                    Return (Color.Black, Color.Red)
                Case TrackStatusEnum.Success
                    Return (Color.Black, Color.LightGreen)
                Case Else
                    Return (Color.Black, Color.Gray)
            End Select
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
    End Class

End Namespace
