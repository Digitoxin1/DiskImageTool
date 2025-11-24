Imports CompactJson

Namespace Flux.Kryoflux
    Public Class KryofluxSettings
        Inherits Settings.SettingsGroup
        Implements ISettings

        Private _appPath As String = ""
        Private _logFileName As String = "log.txt"
        Private _logStripPath As Boolean = False

        Public Property AppPath As String Implements ISettings.AppPath
            Get
                Return _appPath
            End Get
            Set(value As String)
                If _appPath <> value Then
                    _appPath = value
                    MarkDirty()
                End If
            End Set
        End Property

        Public Property LogFileName As String Implements ISettings.LogFileName
            Get
                Return _logFileName
            End Get
            Set(value As String)
                If _logFileName <> value Then
                    _logFileName = value
                    MarkDirty()
                End If
            End Set
        End Property

        Public Property LogStripPath As Boolean Implements ISettings.LogStripPath
            Get
                Return _logStripPath
            End Get
            Set(value As Boolean)
                If _logStripPath <> value Then
                    _logStripPath = value
                    MarkDirty()
                End If
            End Set
        End Property

        Public Function IsPathValid() As Boolean Implements ISettings.IsPathValid
            Return Flux.IsPathValid(_appPath)
        End Function

        Public Sub LoadFromDictionary(dict As Dictionary(Of String, JsonValue))
            If dict Is Nothing Then
                MarkClean()
                Return
            End If

            _appPath = ReadValue(dict, "appPath", _appPath)
            _logFileName = ReadValue(dict, "logFileName", _logFileName)
            _logStripPath = ReadValue(dict, "logStripPath", _logStripPath)

            MarkClean()
        End Sub

        Public Function ToJsonObject() As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {
                {"appPath", _appPath},
                {"logFileName", _logFileName},
                {"logStripPath", _logStripPath}
            }
        End Function
    End Class
End Namespace
