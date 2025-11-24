Imports CompactJson
Imports DiskImageTool.Flux

Namespace Settings
    Public Class UserStateFluxConvertDevice
        Inherits SettingsGroup

        Private _OutputType As ImageImportOutputTypes?
        Private _SaveLog As Boolean = True
        Private _ExtendedLogs As Boolean? = Nothing

        Friend Property ExtendedLogs As Boolean?
            Get
                Return _ExtendedLogs
            End Get
            Set
                If Not Nullable.Equals(_ExtendedLogs, Value) Then
                    _ExtendedLogs = Value
                    MarkDirty()
                End If
            End Set
        End Property

        Friend Property OutputType As ImageImportOutputTypes?
            Get
                Return _OutputType
            End Get
            Set
                If Not Nullable.Equals(_OutputType, Value) Then
                    _OutputType = Value
                    MarkDirty()
                End If
            End Set
        End Property

        Friend Property SaveLog As Boolean
            Get
                Return _SaveLog
            End Get
            Set
                If _SaveLog <> Value Then
                    _SaveLog = Value
                    MarkDirty()
                End If
            End Set
        End Property

        Public Sub LoadFromDictionary(dict As IReadOnlyDictionary(Of String, JsonValue))
            If dict Is Nothing Then
                MarkClean()
                Return
            End If

            _SaveLog = ReadValue(dict, "saveLog", _SaveLog)
            _OutputType = ReadValue(dict, "outputType", _OutputType)
            _ExtendedLogs = ReadValue(dict, "extendedLogs", _ExtendedLogs)

            MarkClean()
        End Sub

        Public Function ToJsonObject() As Dictionary(Of String, Object)
            Dim result As New Dictionary(Of String, Object)

            result("saveLog") = _SaveLog
            If _OutputType.HasValue Then
                result("outputType") = _OutputType.Value.ToString()
            End If
            If _ExtendedLogs.HasValue Then
                result("extendedLogs") = _ExtendedLogs.Value
            End If

            Return result
        End Function
    End Class
End Namespace
