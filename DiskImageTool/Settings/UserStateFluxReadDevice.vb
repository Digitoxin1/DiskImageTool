Imports CompactJson
Imports DiskImageTool.Flux.Greaseweazle

Namespace Settings
    Public Class UserStateFluxReadDevice
        Inherits SettingsGroup

        Private _DriveId As String = ""
        Private _OutputType As ReadDiskOutputTypes?
        Private _SaveLog As Boolean = True

        Friend Property DriveId As String
            Get
                Return _DriveId
            End Get
            Set
                If _DriveId <> Value Then
                    _DriveId = Value
                    MarkDirty()
                End If
            End Set
        End Property

        Friend Property OutputType As ReadDiskOutputTypes?
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
            _DriveId = ReadValue(dict, "driveId", _DriveId)
            _OutputType = ReadValue(dict, "outputType", _OutputType)

            MarkClean()
        End Sub

        Public Function ToJsonObject() As Dictionary(Of String, Object)
            Dim result As New Dictionary(Of String, Object)

            result("saveLog") = _SaveLog
            result("driveId") = _DriveId
            If _OutputType.HasValue Then
                result("outputType") = _OutputType.Value.ToString()
            End If

            Return result
        End Function
    End Class
End Namespace
