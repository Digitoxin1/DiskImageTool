Namespace FloppyDB
    Public Class TitleFindResult
        Public Property Matches As IReadOnlyList(Of FloppyData) = Nothing
        Public Property MD5 As String = ""
        Public ReadOnly Property Primary As FloppyData
            Get
                Return If(Matches IsNot Nothing AndAlso Matches.Count > 0, Matches(0), Nothing)
            End Get
        End Property

        Public Function GetIntList() As String
            Return JoinDistinctStrings(Function(d) d.Int)
        End Function

        Public Function GetLanguageList() As String
            Return JoinDistinctCommaValues(Function(d) d.Language)
        End Function

        Public Function GetNameList() As String
            Return JoinDistinctStrings(Function(d) d.Name, vbNewLine)
        End Function

        Public Function GetPublisherList() As String
            Return JoinDistinctStrings(Function(d) d.Publisher, vbNewLine)
        End Function

        Public Function GetRegionList() As String
            Return JoinDistinctCommaValues(Function(d) d.Region)
        End Function

        Public Function GetSerialList() As String
            Return JoinDistinctStrings(Function(d) d.Serial)
        End Function

        Public Function GetSeriesList() As String
            Return JoinDistinctStrings(Function(d) d.Series, vbNewLine)
        End Function

        Public Function GetVersionDisplay() As String
            If Matches Is Nothing OrElse Matches.Count = 0 Then
                Return ""
            End If

            If Matches.Count = 1 Then
                Return FormatOne(Matches(0))
            End If

            Dim firstKey = MakeKey(Matches(0))
            Dim allSame As Boolean = True
            For i = 1 To Matches.Count - 1
                If MakeKey(Matches(i)) <> firstKey Then
                    allSame = False
                    Exit For
                End If
            Next

            If allSame Then
                Return FormatOne(Matches(0))
            End If

            Dim rows = Matches.Select(Function(d) New With {
            .Ver = Normalize(d.Version),
            .Ser = Normalize(d.Serial),
            .Int = Normalize(d.Int)
        }).
        GroupBy(Function(r) r.Ver & vbNullChar & r.Ser & vbNullChar & r.Int).
        Select(Function(g) g.First()).
        ToList


            Dim groups = rows.GroupBy(Function(r) (r.Ver, r.Ser)).ToList()

            Dim parts As New List(Of String)

            For Each g In groups
                Dim seg As New List(Of String)

                If g.Key.Ver <> "" Then
                    seg.Add(g.Key.Ver)
                End If
                If g.Key.Ser <> "" Then
                    seg.Add("Serial " & g.Key.Ser)
                End If

                Dim ints = g.Select(Function(x) x.Int).Where(Function(s) s <> "").Distinct(StringComparer.OrdinalIgnoreCase).ToList()

                If ints.Count > 0 Then
                    seg.Add("Int. " & String.Join(", ", ints))
                End If

                If seg.Count > 0 Then
                    parts.Add(String.Join(", ", seg))
                End If
            Next

            Return String.Join(", ", parts)
        End Function

        Public Function GetVersionList() As String
            Return JoinDistinctStrings(Function(d) d.Version)
        End Function
        Public Function GetYearList() As String
            Return JoinDistinctStrings(Function(d) d.Year)
        End Function

        Private Shared Function Normalize(s As String) As String
            Return If(s, "").Trim()
        End Function

        Private Function DistinctCommaValues(selector As Func(Of FloppyData, String)) As List(Of String)
            Dim result As New List(Of String)
            Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each d In Matches
                Dim value = selector(d)
                If String.IsNullOrWhiteSpace(value) Then Continue For

                For Each part In value.Split(","c)
                    Dim s = part.Trim()
                    If s = "" Then Continue For

                    If seen.Add(s) Then
                        result.Add(s)
                    End If
                Next
            Next

            Return result
        End Function

        Private Function DistinctStrings(selector As Func(Of FloppyData, String)) As List(Of String)
            Dim result As New List(Of String)
            Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each d In Matches
                Dim s = selector(d)
                If String.IsNullOrWhiteSpace(s) Then
                    Continue For
                End If
                s = s.Trim()
                If seen.Add(s) Then
                    result.Add(s)
                End If
            Next

            Return result
        End Function
        Private Function FormatOne(d As FloppyData) As String
            Dim ver = Normalize(d.Version)
            Dim ser = Normalize(d.Serial)
            Dim intl = Normalize(d.Int)

            Dim seg As New List(Of String)
            If ver <> "" Then
                seg.Add(ver)
            End If
            If ser <> "" Then
                seg.Add("Serial " & ser)
            End If
            If intl <> "" Then
                seg.Add("Int. " & intl)
            End If

            Return String.Join(", ", seg)
        End Function

        Private Function JoinDistinctCommaValues(selector As Func(Of FloppyData, String), Optional Separator As String = ", ") As String
            Return String.Join(Separator, DistinctCommaValues(selector))
        End Function

        Private Function JoinDistinctStrings(selector As Func(Of FloppyData, String), Optional Separator As String = ", ") As String
            Return String.Join(Separator, DistinctStrings(selector))
        End Function
        Private Function MakeKey(d As FloppyData) As String
            Dim ver = Normalize(d.Version)
            Dim ser = Normalize(d.Serial)
            Dim intl = Normalize(d.Int)

            Return ver & vbNullChar & ser & vbNullChar & intl
        End Function
    End Class
End Namespace