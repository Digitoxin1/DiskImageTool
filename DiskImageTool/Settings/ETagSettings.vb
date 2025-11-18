Imports CompactJson

Namespace Settings
    Public Class ETagSettings
        Implements ISettingsGroup

        Private _appUpdate As String = ""
        Private _changeLog As String = ""
        Private _isDirty As Boolean

        Public Property AppUpdate As String
            Get
                Return _appUpdate
            End Get
            Set(value As String)
                If _appUpdate <> value Then
                    _appUpdate = value
                    _isDirty = True
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
                    _isDirty = True
                End If
            End Set
        End Property

        Public Property IsDirty As Boolean Implements ISettingsGroup.IsDirty
            Get
                Return _isDirty
            End Get
            Set(value As Boolean)
                _isDirty = value
            End Set
        End Property

        Public Sub LoadFromDictionary(dict As Dictionary(Of String, JsonValue))
            If dict Is Nothing Then
                MarkClean()
                Return
            End If

            _changeLog = ReadValue(dict, "changeLog", _changeLog)
            _appUpdate = ReadValue(dict, "appUpdate", _appUpdate)

            MarkClean()
        End Sub

        Public Sub MarkClean() Implements ISettingsGroup.MarkClean
            _isDirty = False
        End Sub

        Public Function ToJsonObject() As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {
                {"changeLog", _changeLog},
                {"appUpdate", _appUpdate}
            }
        End Function
    End Class

End Namespace
