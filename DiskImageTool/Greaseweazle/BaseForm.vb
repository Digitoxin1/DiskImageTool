Namespace Greaseweazle
    Public Class BaseForm
        Private Const TOTAL_TRACKS As UShort = 83
        Private ReadOnly _Parser As ConsoleOutputParser
        Private _TableSide0Outer As TableLayoutPanel
        Private _TableSide1Outer As TableLayoutPanel
        Private _TableSide0 As TableLayoutPanel
        Private _TableSide1 As TableLayoutPanel

        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            _Parser = New ConsoleOutputParser
            _TableSide0Outer = New TableLayoutPanel With {
                .Anchor = AnchorStyles.Top Or AnchorStyles.Right
            }
            _TableSide1Outer = New TableLayoutPanel With {
                .Anchor = AnchorStyles.Top Or AnchorStyles.Right
            }
        End Sub

        Public ReadOnly Property Parser As ConsoleOutputParser
            Get
                Return _Parser
            End Get
        End Property

        Public ReadOnly Property TableSide0Outer As TableLayoutPanel
            Get
                Return _TableSide0Outer
            End Get
        End Property

        Public ReadOnly Property TableSide1Outer As TableLayoutPanel
            Get
                Return _TableSide1Outer
            End Get
        End Property

        Public ReadOnly Property TableSide0 As TableLayoutPanel
            Get
                Return _TableSide0
            End Get
        End Property

        Public ReadOnly Property TableSide1 As TableLayoutPanel
            Get
                Return _TableSide1
            End Get
        End Property

        Public Sub TrackGridInit(Tracks As UShort, Sides As Byte)
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
