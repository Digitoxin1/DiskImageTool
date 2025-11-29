Imports CompactJson

Namespace Settings
    Public Class SettingsGroup
        Private _isDirty As Boolean = False
        Public Overridable ReadOnly Property IsDirty As Boolean
            Get
                If _isDirty Then
                End If
                Return _isDirty
            End Get
        End Property

        Public Shared Function CloneDictionary(Of TKey, TValue)(source As IDictionary(Of TKey, TValue)) As Dictionary(Of TKey, TValue)
            If source Is Nothing Then
                Return New Dictionary(Of TKey, TValue)()
            End If

            Return New Dictionary(Of TKey, TValue)(source)
        End Function

        Public Shared Function DictionariesEqual(Of TKey, TValue)(a As IDictionary(Of TKey, TValue), b As IDictionary(Of TKey, TValue)) As Boolean
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

        Public Shared Function ReadSection(root As Dictionary(Of String, JsonValue), key As String) As Dictionary(Of String, JsonValue)
            Dim jv As JsonValue = Nothing
            If root Is Nothing OrElse Not root.TryGetValue(key, jv) OrElse jv Is Nothing Then
                Return Nothing
            End If

            Try
                Dim raw = jv.ToString()
                Return Serializer.Parse(Of Dictionary(Of String, JsonValue))(raw)
            Catch
                Return Nothing
            End Try
        End Function

        Public Shared Function ReadValue(Of T)(dict As IReadOnlyDictionary(Of String, JsonValue), key As String, defaultValue As T) As T
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
        Public Overridable Sub MarkClean()
            _isDirty = False
        End Sub

        Public Overridable Sub MarkDirty()
            _isDirty = True
        End Sub
    End Class
End Namespace
