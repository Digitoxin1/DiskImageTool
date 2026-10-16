Imports System.Text
Imports Greaseweazle.Actions

Namespace Flux.Greaseweazle

    ' In-memory Greaseweazle read log. Track lines stay buffered until that
    ' track finishes, so a cancelled track keeps the lines that match the
    ' .raw file still on disk. A later read replaces only the tracks it
    ' touches, and merges those cylinders into the sector map.
    Friend Class GreaseweazleReadLog

        Private ReadOnly _Preamble As New List(Of String)
        Private ReadOnly _Order As New List(Of (Cyl As Integer, Head As Integer))
        Private ReadOnly _Tracks As New Dictionary(Of (Cyl As Integer, Head As Integer), List(Of String))
        Private _Cyls As List(Of Integer)
        Private _HasOpen As Boolean
        Private _OpenCyl As Integer
        Private _OpenHead As Integer
        Private _OpenLines As List(Of String)
        Private _Reread As Boolean
        Private _Rows As List(Of SummaryRow)

        Private Class SummaryRow
            Public Property Head As Integer
            Public Property Sector As Integer
            Public Property Cells As List(Of SectorSummaryCell)
        End Class

        Public ReadOnly Property HasContent As Boolean
            Get
                If _Preamble.Count > 0 OrElse _Tracks.Count > 0 Then
                    Return True
                End If

                Return _Rows IsNot Nothing AndAlso _Rows.Count > 0
            End Get
        End Property

        Public Sub AddPreambleLine(line As String)
            If _Reread OrElse String.IsNullOrEmpty(line) Then
                Return
            End If

            _Preamble.Add(line)
        End Sub

        Public Sub AddTrackLine(cyl As Integer, head As Integer, line As String)
            If String.IsNullOrEmpty(line) Then
                Return
            End If

            If Not _HasOpen OrElse _OpenCyl <> cyl OrElse _OpenHead <> head Then
                CommitOpenTrack()
                _HasOpen = True
                _OpenCyl = cyl
                _OpenHead = head
                _OpenLines = New List(Of String)
            End If

            _OpenLines.Add(line)
        End Sub

        Public Sub BeginReread()
            CommitOpenTrack()
            _Reread = True
        End Sub

        Public Sub CommitOpenTrack()
            If Not _HasOpen Then
                Return
            End If

            Dim Key = (Cyl:=_OpenCyl, Head:=_OpenHead)

            If Not _Tracks.ContainsKey(Key) Then
                _Order.Add(Key)
            End If

            _Tracks(Key) = _OpenLines
            _HasOpen = False
            _OpenLines = Nothing
        End Sub

        Public Sub DiscardOpenTrack()
            _HasOpen = False
            _OpenLines = Nothing
        End Sub

        Public Sub MergeSummary(grid As SectorSummaryGrid)
            CommitOpenTrack()

            If grid Is Nothing OrElse Not grid.HasContent OrElse grid.Cyls Is Nothing OrElse grid.Rows Is Nothing Then
                Return
            End If

            If _Cyls Is Nothing Then
                CopySummary(grid)
                Return
            End If

            EnsureColumns(grid.Cyls)

            Dim Updated = New HashSet(Of Integer)(grid.Cyls)
            Dim MaxSectorByHead = MaxSectorsByHead(grid)

            For Each Row In grid.Rows
                Dim Stored = FindOrAddRow(Row.Head, Row.Sector)

                For i = 0 To grid.Cyls.Count - 1
                    If i >= Row.Cells.Count Then
                        Exit For
                    End If

                    Dim Index = _Cyls.IndexOf(grid.Cyls(i))

                    If Index >= 0 AndAlso Index < Stored.Cells.Count Then
                        Stored.Cells(Index) = Row.Cells(i)
                    End If
                Next
            Next

            For Each Stored In _Rows
                Dim MaxSector As Integer

                If Not MaxSectorByHead.TryGetValue(Stored.Head, MaxSector) Then
                    Continue For
                End If

                If Stored.Sector <= MaxSector Then
                    Continue For
                End If

                For Each Cyl In Updated
                    Dim Index = _Cyls.IndexOf(Cyl)

                    If Index >= 0 AndAlso Index < Stored.Cells.Count Then
                        Stored.Cells(Index) = SectorSummaryCell.Empty
                    End If
                Next
            Next
        End Sub

        Public Function Render() As String
            Dim sb As New StringBuilder()

            For Each Line In _Preamble
                sb.AppendLine(Line)
            Next

            For Each Key In _Order
                Dim Lines As List(Of String) = Nothing

                If _Tracks.TryGetValue(Key, Lines) Then
                    For Each Line In Lines
                        sb.AppendLine(Line)
                    Next
                End If
            Next

            For Each Line In FormatSectorSummaryLines(BuildGrid())
                sb.AppendLine(Line)
            Next

            Return sb.ToString()
        End Function

        Private Function BuildGrid() As SectorSummaryGrid
            If _Cyls Is Nothing OrElse _Rows Is Nothing OrElse _Rows.Count = 0 Then
                Return Nothing
            End If

            Dim Heads As New List(Of Integer)
            Dim Built As New List(Of SectorSummaryRow)
            Dim Total = 0
            Dim Good = 0

            For Each Row In _Rows
                If Not Heads.Contains(Row.Head) Then
                    Heads.Add(Row.Head)
                End If

                For Each Cell In Row.Cells
                    If Cell <> SectorSummaryCell.Empty Then
                        Total += 1
                    End If

                    If Cell = SectorSummaryCell.Good Then
                        Good += 1
                    End If
                Next

                Built.Add(New SectorSummaryRow(Row.Head, Row.Sector, Row.Cells))
            Next

            Return New SectorSummaryGrid(_Cyls, Heads, Built, Total, Good)
        End Function

        Private Sub CopySummary(grid As SectorSummaryGrid)
            _Cyls = New List(Of Integer)(grid.Cyls)
            _Rows = New List(Of SummaryRow)

            For Each Row In grid.Rows
                _Rows.Add(New SummaryRow With {
                    .Head = Row.Head,
                    .Sector = Row.Sector,
                    .Cells = New List(Of SectorSummaryCell)(Row.Cells)
                })
            Next
        End Sub

        Private Sub EnsureColumns(cyls As IReadOnlyList(Of Integer))
            For Each Cyl In cyls
                If _Cyls.Contains(Cyl) Then
                    Continue For
                End If

                Dim InsertAt = 0

                While InsertAt < _Cyls.Count AndAlso _Cyls(InsertAt) < Cyl
                    InsertAt += 1
                End While

                _Cyls.Insert(InsertAt, Cyl)

                If _Rows Is Nothing Then
                    _Rows = New List(Of SummaryRow)
                End If

                For Each Row In _Rows
                    Row.Cells.Insert(InsertAt, SectorSummaryCell.Empty)
                Next
            Next
        End Sub

        Private Function FindOrAddRow(head As Integer, sector As Integer) As SummaryRow
            If _Rows Is Nothing Then
                _Rows = New List(Of SummaryRow)
            End If

            For Each Row In _Rows
                If Row.Head = head AndAlso Row.Sector = sector Then
                    Return Row
                End If
            Next

            Dim InsertAt = _Rows.Count

            For i = 0 To _Rows.Count - 1
                Dim Row = _Rows(i)

                If Row.Head > head OrElse (Row.Head = head AndAlso Row.Sector > sector) Then
                    InsertAt = i
                    Exit For
                End If
            Next

            Dim Cells As New List(Of SectorSummaryCell)(_Cyls.Count)

            For n = 1 To _Cyls.Count
                Cells.Add(SectorSummaryCell.Empty)
            Next

            Dim Created As New SummaryRow With {
                .Head = head,
                .Sector = sector,
                .Cells = Cells
            }

            _Rows.Insert(InsertAt, Created)
            Return Created
        End Function

        Private Shared Function MaxSectorsByHead(grid As SectorSummaryGrid) As Dictionary(Of Integer, Integer)
            Dim MaxSectorByHead As New Dictionary(Of Integer, Integer)

            If grid.Heads IsNot Nothing Then
                For Each Head In grid.Heads
                    MaxSectorByHead(Head) = -1
                Next
            End If

            For Each Row In grid.Rows
                Dim Current = -1

                If MaxSectorByHead.ContainsKey(Row.Head) Then
                    Current = MaxSectorByHead(Row.Head)
                End If

                If Row.Sector > Current Then
                    MaxSectorByHead(Row.Head) = Row.Sector
                End If
            Next

            Return MaxSectorByHead
        End Function

    End Class

End Namespace
