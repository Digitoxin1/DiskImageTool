Imports CompactJson

Namespace Settings
    Public Class UserStateETags
        Inherits SettingsGroup

        Private _appUpdate As String = ""
        Private _dbUpdate As String = ""
        Private _changeLog As String = ""

        Public Property AppUpdate As String
            Get
                Return _appUpdate
            End Get
            Set(value As String)
                If _appUpdate <> value Then
                    _appUpdate = value
                    MarkDirty()
                End If
            End Set
        End Property

        Public Property ChangeLog As String
            Get
                Return _changeLog
            End Get
            Set(value As String)
                If _changeLog <> value Then
                    _changeLog = value
                    MarkDirty()
                End If
            End Set
        End Property

        Public Property DBUpdate As String
            Get
                Return _dbUpdate
            End Get
            Set(value As String)
                If _dbUpdate <> value Then
                    _dbUpdate = value
                    MarkDirty()
                End If
            End Set
        End Property

        Public Sub LoadFromDictionary(dict As Dictionary(Of String, JsonValue))
            If dict Is Nothing Then
                MarkClean()
                Return
            End If

            _changeLog = ReadValue(dict, "changeLog", _changeLog)
            _appUpdate = ReadValue(dict, "appUpdate", _appUpdate)
            _dbUpdate = ReadValue(dict, "dbUpdate", _dbUpdate)

            MarkClean()
        End Sub

        Public Function ToJsonObject() As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {
                {"changeLog", _changeLog},
                {"appUpdate", _appUpdate},
                {"dbUpdate", _dbUpdate}
            }
        End Function
    End Class

End Namespace
