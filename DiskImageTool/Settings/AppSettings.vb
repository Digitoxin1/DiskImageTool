Imports System.IO
Imports CompactJson

Namespace Settings
    Public Class AppSettings
        Implements IDisposable

        Private ReadOnly _filePath As String

        Private _checkUpdateOnStartup As Boolean = True
        Private _createBackups As Boolean = True
        Private _debug As Boolean = False
        Private _displayTitles As Boolean = True
        Private _dragAndDrop As Boolean = True
        Private _language As String = ""
        Private _windowHeight As Integer = 0
        Private _windowWidth As Integer = 0

        Private Sub New(filePath As String)
            _filePath = filePath

            ETags = New ETagSettings()
            Greaseweazle = New Flux.Greaseweazle.GreaseweazleSettings()
            Kryoflux = New Flux.Kryoflux.KryofluxSettings()
        End Sub

        Public Property DragAndDrop As Boolean
            Get
                Return _dragAndDrop
            End Get
            Set(value As Boolean)
                If _dragAndDrop <> value Then
                    _dragAndDrop = value
                    IsDirty = True
                End If
            End Set
        End Property

        Public Property CheckUpdateOnStartup As Boolean
            Get
                Return _checkUpdateOnStartup
            End Get
            Set(value As Boolean)
                If _checkUpdateOnStartup <> value Then
                    _checkUpdateOnStartup = value
                    IsDirty = True
                End If
            End Set
        End Property

        Public Property CreateBackups As Boolean
            Get
                Return _createBackups
            End Get
            Set(value As Boolean)
                If _createBackups <> value Then
                    _createBackups = value
                    IsDirty = True
                End If
            End Set
        End Property

        Public Property Debug As Boolean
            Get
                Return _debug
            End Get
            Set(value As Boolean)
                If _debug <> value Then
                    _debug = value
                    IsDirty = True
                End If
            End Set
        End Property

        Public Property DisplayTitles As Boolean
            Get
                Return _displayTitles
            End Get
            Set(value As Boolean)
                If _displayTitles <> value Then
                    _displayTitles = value
                    IsDirty = True
                End If
            End Set
        End Property
        Public Property ETags As ETagSettings

        Public Property Greaseweazle As Flux.Greaseweazle.GreaseweazleSettings

        Public Property IsDirty As Boolean

        Public Property Kryoflux As Flux.Kryoflux.KryofluxSettings

        Public Property Language As String
            Get
                Return _language
            End Get
            Set(value As String)
                If _language <> value Then
                    _language = value
                    IsDirty = True
                End If
            End Set
        End Property
        Public Property WindowHeight As Integer
            Get
                Return _windowHeight
            End Get
            Set(value As Integer)
                If _windowHeight <> value Then
                    _windowHeight = value
                    IsDirty = True
                End If
            End Set
        End Property

        Public Property WindowWidth As Integer
            Get
                Return _windowWidth
            End Get
            Set(value As Integer)
                If _windowWidth <> value Then
                    _windowWidth = value
                    IsDirty = True
                End If
            End Set
        End Property

        Public Shared Function Load(Optional configPath As String = Nothing) As AppSettings
            Dim path = If(configPath, GetDefaultConfigPath())
            Dim settings As New AppSettings(path)

            If Not File.Exists(path) Then
                settings.Save(True)
                Return settings
            End If

            Try
                Dim json = File.ReadAllText(path)
                If String.IsNullOrWhiteSpace(json) Then
                    settings.Save(True)
                    Return settings
                End If

                Dim root = Serializer.Parse(Of Dictionary(Of String, JsonValue))(json)

                ' top-level simple values (use backing fields to avoid IsDirty during load)
                settings._windowWidth = ReadValue(root, "windowWidth", settings._windowWidth)
                settings._windowHeight = ReadValue(root, "windowHeight", settings._windowHeight)
                settings._createBackups = ReadValue(root, "createBackups", settings._createBackups)
                settings._debug = ReadValue(root, "debug", settings._debug)
                settings._dragAndDrop = ReadValue(root, "dragAndDrop", settings._dragAndDrop)
                settings._checkUpdateOnStartup = ReadValue(root, "checkUpdateOnStartup", settings._checkUpdateOnStartup)
                settings._displayTitles = ReadValue(root, "displayTitles", settings._displayTitles)
                settings._language = ReadValue(root, "language", settings._language)

                Dim tagsDict = ReadSection(root, "eTags")
                settings.ETags.LoadFromDictionary(tagsDict)

                Dim gwDict = ReadSection(root, "greaseweazle")
                settings.Greaseweazle.LoadFromDictionary(gwDict)

                Dim kfDict = ReadSection(root, "kryoflux")
                settings.Kryoflux.LoadFromDictionary(kfDict)

                settings.IsDirty = False

            Catch
                settings.IsDirty = True
            End Try

            Return settings
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            Save()
            GC.SuppressFinalize(Me)
        End Sub

        Public Sub Save(Optional Force As Boolean = False)
            If Not (IsDirty OrElse ETags.IsDirty OrElse Greaseweazle.IsDirty OrElse Kryoflux.IsDirty OrElse Force) Then
                Return
            End If

            Dim root As New Dictionary(Of String, Object) From {
                {"windowWidth", _windowWidth},
                {"windowHeight", _windowHeight},
                {"createBackups", _createBackups},
                {"debug", _debug},
                {"dragAndDrop", _dragAndDrop},
                {"checkUpdateOnStartup", _checkUpdateOnStartup},
                {"displayTitles", _displayTitles},
                {"language", _language}
            }

            root("eTags") = ETags.ToJsonObject()
            root("greaseweazle") = Greaseweazle.ToJsonObject()
            root("kryoflux") = Kryoflux.ToJsonObject()

            Try
                Dim json = Serializer.ToString(root, True)
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath))
                File.WriteAllText(_filePath, json)

                IsDirty = False
                ETags.MarkClean()
                Greaseweazle.MarkClean()
                Kryoflux.MarkClean()
            Catch ex As Exception
                Diagnostics.Debug.Print("Error: Unable to save AppSettings")
            End Try
        End Sub

        Private Shared Function GetDefaultConfigPath() As String
            Dim baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            Dim appName = My.Application.Info.AssemblyName
            Dim appFolder = Path.Combine(baseFolder, appName)
            Directory.CreateDirectory(appFolder)
            Return Path.Combine(appFolder, "settings.json")
        End Function

        Private Shared Function ReadSection(
            root As Dictionary(Of String, JsonValue),
            key As String) As Dictionary(Of String, JsonValue)

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
    End Class
End Namespace
