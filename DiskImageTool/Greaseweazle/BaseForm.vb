Namespace Greaseweazle
    Public Class BaseForm
        Private Const TOTAL_TRACKS As UShort = 83
        Private ReadOnly _Parser As ConsoleOutputParser
        Private _TableSide0 As TableLayoutPanel
        Private _TableSide0Outer As TableLayoutPanel
        Private _TableSide1 As TableLayoutPanel
        Private _TableSide1Outer As TableLayoutPanel
        Public Enum TrackStatus
            Erasing
            Writing
            Retry
            Failed
            Success
            Aborted
            [Error]
        End Enum

        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            _Parser = New ConsoleOutputParser
            _TableSide0Outer = New TableLayoutPanel With {
                .Anchor = AnchorStyles.Top Or AnchorStyles.Right
            }
            _TableSide1Outer = New TableLayoutPanel With {
                .Anchor = AnchorStyles.Top Or AnchorStyles.Right,
                .Margin = New Padding(32, 3, 3, 3)
            }
        End Sub

        Public ReadOnly Property Parser As ConsoleOutputParser
            Get
                Return _Parser
            End Get
        End Property

        Public ReadOnly Property TableSide0 As TableLayoutPanel
            Get
                Return _TableSide0
            End Get
        End Property

        Public ReadOnly Property TableSide0Outer As TableLayoutPanel
            Get
                Return _TableSide0Outer
            End Get
        End Property

        Public ReadOnly Property TableSide1 As TableLayoutPanel
            Get
                Return _TableSide1
            End Get
        End Property

        Public ReadOnly Property TableSide1Outer As TableLayoutPanel
            Get
                Return _TableSide1Outer
            End Get
        End Property

        Public Sub MarkTrack(Track As Integer, Side As Integer, Status As TrackStatus, Label As String)
            Dim Table As TableLayoutPanel

            If Side = 0 Then
                Table = TableSide0
            ElseIf Side = 1 Then
                Table = TableSide1
            Else
                Table = Nothing
            End If

            If Table IsNot Nothing Then
                Dim BackColor = GetTrackStatusColor(Status)
                FloppyGridSetLabel(Table, Track, Label, BackColor)
            End If
        End Sub

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

        Public Sub TrackGridInit(Tracks As UShort, Sides As Byte)
            If _TableSide0 IsNot Nothing Then
                TrackGridReset(Tracks, Sides)
                Exit Sub
            End If

            _TableSide0 = FloppyGridInit(_TableSide0Outer, My.Resources.Label_Side & " 0", Tracks, Math.Max(Tracks, TOTAL_TRACKS))

            Dim TrackCount As UShort = 0
            If Sides > 1 Then
                TrackCount = Tracks
            End If
            _TableSide1 = FloppyGridInit(_TableSide1Outer, My.Resources.Label_Side & " 1", TrackCount, Math.Max(Tracks, TOTAL_TRACKS))
        End Sub

        Public Sub TrackGridReset(Tracks As UShort, Sides As Byte)
            FloppyGridReset(_TableSide0, Tracks, Math.Max(Tracks, TOTAL_TRACKS))

            Dim TrackCount As UShort = 0
            If Sides > 1 Then
                TrackCount = Tracks
            End If
            FloppyGridReset(_TableSide1, TrackCount, Math.Max(Tracks, TOTAL_TRACKS))
        End Sub
    End Class
End Namespace
