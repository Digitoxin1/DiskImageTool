Namespace Greaseweazle
    Public Class BaseForm
        Friend WithEvents Process As ConsoleProcessRunner
        Private WithEvents TS0 As FloppyTrackGrid
        Private WithEvents TS1 As FloppyTrackGrid
        Private Const TOTAL_TRACKS As UShort = 84
        Private ReadOnly _Parser As ConsoleOutputParser
        Private _CancelButtonClicked As Boolean = False
        Private _Sides As Byte
        Private _Tracks As UShort

        Public Enum TrackStatus
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

        Public Sub New(Optional UseGrid As Boolean = True)
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Process = New ConsoleProcessRunner With {
                .EventContext = Threading.SynchronizationContext.Current
            }

            _Parser = New ConsoleOutputParser

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

        Public ReadOnly Property Parser As ConsoleOutputParser
            Get
                Return _Parser
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
        End Sub

        Public Function GetDriveName(Drive As String) As String
            Select Case Drive
                Case "A", "B"
                    Return Drive & ":"
                Case Else
                    Return "DS" & Drive & ":"
            End Select
        End Function

        Public Function GetTrackStatusColor(Status As TrackStatus) As Color
            Select Case Status
                Case TrackStatus.Erasing
                    Return Color.LightBlue
                Case TrackStatus.Writing
                    Return Color.Orange
                Case TrackStatus.Retry
                    Return Color.Yellow
                Case TrackStatus.Failed
                    Return Color.Red
                Case TrackStatus.Success
                    Return Color.LightGreen
                Case Else
                    Return Color.Gray
            End Select
        End Function

        Public Function GetTrackStatusText(Status As TrackStatus, Optional Retries As UShort = 0) As String
            Select Case Status
                Case TrackStatus.Erasing
                    Return My.Resources.Label_Erasing
                Case TrackStatus.Writing
                    Return My.Resources.Label_Writing
                Case TrackStatus.Retry
                    Return My.Resources.Label_Retrying & IIf(Retries > 0, " " & Retries, "")
                Case TrackStatus.Failed
                    Return My.Resources.Label_Failed
                Case TrackStatus.Success
                    Return My.Resources.Label_Complete
                Case TrackStatus.Aborted
                    Return My.Resources.Label_Aborted
                Case TrackStatus.Error
                    Return My.Resources.Label_Error
                Case Else
                    Return ""
            End Select
        End Function

        Public Function GridGetTable(Side As Byte) As FloppyTrackGrid
            If Side = 0 Then
                Return TS0
            ElseIf Side = 1 Then
                Return TS1
            Else
                Return Nothing
            End If
        End Function

        Public Sub GridMarkTrack(Track As Integer, Side As Integer, Status As TrackStatus, Label As String, DoubleStep As Boolean)
            Dim Table = GridGetTable(Side)

            If Table IsNot Nothing Then
                Dim BackColor = GetTrackStatusColor(Status)
                If DoubleStep Then
                    Track *= 2
                End If
                Table.SetCell(Track, Text:=Label, BackColor:=BackColor)
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

        Public Function ProcessTrackStatusWrite(Statusinfo As TrackStatusInfoWrite, Action As String) As (Status As TrackStatus, Label As String)
            Dim Status As TrackStatus
            Dim Label As String

            If Action = "Erasing" Then
                Status = TrackStatus.Erasing
                Label = "E"
            ElseIf Statusinfo.Failed Then
                Status = TrackStatus.Failed
                Label = Statusinfo.Retries
            ElseIf Statusinfo.Retries > 0 Then
                Status = TrackStatus.Retry
                Label = Statusinfo.Retries
            ElseIf Action = "Complete" Then
                If Statusinfo.Retries > 0 Then
                    Status = TrackStatus.Retry
                    Label = Statusinfo.Retries
                Else
                    Status = TrackStatus.Success
                    Label = ""
                End If
            Else
                Status = TrackStatus.Writing
                Label = "W"
            End If

            Return (Status, Label)
        End Function

        Public Function UpdateStatusInfoWrite(TrackStatus As Dictionary(Of String, TrackStatusInfoWrite), TrackInfo As ConsoleOutputParser.WriteTrackInfo) As TrackStatusInfoWrite
            Dim Key = TrackInfo.SourceTrack & "." & TrackInfo.SourceSide
            Dim StatusInfo As TrackStatusInfoWrite

            If TrackStatus.ContainsKey(Key) Then
                StatusInfo = TrackStatus.Item(Key)
            Else
                StatusInfo = New TrackStatusInfoWrite With {
                    .Track = TrackInfo.SourceTrack,
                    .Side = TrackInfo.SourceSide
                }
                TrackStatus.Add(Key, StatusInfo)
            End If

            StatusInfo.Retries = Math.Max(StatusInfo.Retries, TrackInfo.Retry)
            If TrackInfo.Failed Then
                StatusInfo.Failed = TrackInfo.Failed
            End If

            Return StatusInfo
        End Function

        Friend Function GetTrackHeads(StartHead As Integer, Optional EndHead As Integer = -1) As CommandLineBuilder.TrackHeads
            If EndHead = -1 Then
                EndHead = StartHead
            End If

            If StartHead = 0 And EndHead = 0 Then
                Return CommandLineBuilder.TrackHeads.head0
            ElseIf StartHead = 1 And EndHead = 1 Then
                Return CommandLineBuilder.TrackHeads.head1
            Else
                Return CommandLineBuilder.TrackHeads.both
            End If
        End Function

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
            Return MsgBox(My.Resources.Dialog_ConfirmCancel, MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes
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
        Public Class TrackStatusInfoWrite
            Public Property Failed As Boolean
            Public Property Retries As UShort = 0
            Public Property Side As Integer
            Public Property Track As Integer
        End Class
    End Class
End Namespace
