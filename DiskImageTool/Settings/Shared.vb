Imports CompactJson

Namespace Settings
    Module Utility
        Public Function ReadValue(Of T)(
            dict As Dictionary(Of String, JsonValue),
            key As String,
            defaultValue As T) As T

            Dim jv As JsonValue = Nothing
            If dict Is Nothing OrElse Not dict.TryGetValue(key, jv) OrElse jv Is Nothing Then
                Return defaultValue
            End If

            Try
                Dim raw = jv.ToString()
                Return Serializer.Parse(Of T)(raw)
            Catch
                Return defaultValue
            End Try
        End Function

        Public Function CloneDictionary(Of TKey, TValue)(source As IDictionary(Of TKey, TValue)) As Dictionary(Of TKey, TValue)
            If source Is Nothing Then
                Return New Dictionary(Of TKey, TValue)()
            End If

            Return New Dictionary(Of TKey, TValue)(source)
        End Function

        Public Function DictionariesEqual(Of TKey, TValue)(a As IDictionary(Of TKey, TValue), b As IDictionary(Of TKey, TValue)) As Boolean
            If ReferenceEquals(a, b) Then
                Return True
            End If
            If a Is Nothing AndAlso b Is Nothing Then
                Return True
            End If
            If a Is Nothing OrElse b Is Nothing Then
                Return False
            End If
            If a.Count <> b.Count Then
                Return False
            End If

            Dim cmp = EqualityComparer(Of TValue).Default

            For Each kvp In a
                Dim otherValue As TValue = Nothing
                If Not b.TryGetValue(kvp.Key, otherValue) Then
                    Return False
                End If
                If Not cmp.Equals(kvp.Value, otherValue) Then
                    Return False
                End If
            Next

            Return True
        End Function
    End Module
End Namespace
