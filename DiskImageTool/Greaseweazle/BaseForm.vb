Namespace Greaseweazle
    Public Class BaseForm
        Private WithEvents TS0 As FloppyTrackGrid
        Private WithEvents TS1 As FloppyTrackGrid
        Private Const TOTAL_TRACKS As UShort = 84
        Private ReadOnly _Parser As ConsoleOutputParser
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

        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            _Parser = New ConsoleOutputParser
            TS0 = New FloppyTrackGrid(TOTAL_TRACKS, My.Resources.Label_Side & " 0") With {
                .Anchor = AnchorStyles.Top Or AnchorStyles.Right
            }
            TS1 = New FloppyTrackGrid(TOTAL_TRACKS, My.Resources.Label_Side & " 1") With {
                .Anchor = AnchorStyles.Top Or AnchorStyles.Right,
                .Margin = New Padding(32, 3, 3, 3)
            }
        End Sub

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

        Private Sub GridResetTracks(Grid As FloppyTrackGrid, Tracks As UShort, Disabled As Boolean, Optional ResetSelected As Boolean = True)
            Grid.ActiveTrackCount = Tracks
            Grid.ResetAll()

            If ResetSelected Then
                Grid.IsChecked = False
            End If
            Grid.Disabled = Disabled
        End Sub
#Region "Events"
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
