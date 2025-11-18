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
    End Module
End Namespace
