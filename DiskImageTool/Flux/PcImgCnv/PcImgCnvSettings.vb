Imports CompactJson

Namespace Flux.PcImgCnv
    Public Class PcImgCnvSettings
        Implements Settings.ISettingsGroup
        Implements ISettings

        Private _appPath As String = ""
        Private _isDirty As Boolean
        Private _logFileName As String = "log.txt"

        Public Property AppPath As String Implements Flux.ISettings.AppPath
            Get
                Return _appPath
            End Get
            Set(value As String)
                If _appPath <> value Then
                    _appPath = value
                    _isDirty = True
                End If
            End Set
        End Property

        Public Property IsDirty As Boolean Implements Settings.ISettingsGroup.IsDirty
            Get
                Return _isDirty
            End Get
            Set(value As Boolean)
                _isDirty = value
            End Set
        End Property

        Public Property LogFileName As String Implements ISettings.LogFileName
            Get
                Return _logFileName
            End Get
            Set(value As String)
                If _logFileName <> value Then
                    _logFileName = value
                    _isDirty = True
                End If
            End Set
        End Property

        Public Property LogStripPath As Boolean Implements ISettings.LogStripPath
            Get
                Return False
            End Get
            Set(value As Boolean)
                ' Not supported
            End Set
        End Property

        Public Function IsPathValid() As Boolean Implements Flux.ISettings.IsPathValid
            Return Flux.IsPathValid(_appPath)
        End Function

        Public Sub LoadFromDictionary(dict As Dictionary(Of String, JsonValue))
            If dict Is Nothing Then
                MarkClean()
                Return
            End If

            _appPath = ReadValue(dict, "appPath", _appPath)
            _logFileName = ReadValue(dict, "logFileName", _logFileName)

            MarkClean()
        End Sub

        Public Sub MarkClean() Implements Settings.ISettingsGroup.MarkClean
            _isDirty = False
        End Sub

        Public Function ToJsonObject() As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {
            {"appPath", _appPath},
            {"logFileName", _logFileName}
        }
        End Function

        Private Shared Function ReadValue(Of T)(
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
    End Class
End Namespace
