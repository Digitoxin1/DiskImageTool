Imports System.IO
Imports CompactJson
Imports DiskImageTool.Settings.SettingsGroup

Namespace Settings
    Public Class AppSettings
        Implements IDisposable
        Public Const MAX_RECENT_FILES As Integer = 10
        Public Const MAX_RECENT_FOLDERS As Integer = 5
        Private Const DEFAULT_CONFIG_FILE As String = "settings.json"

        Private ReadOnly _filePath As String

        Private _checkUpdateOnStartup As Boolean = True
        Private _createBackups As Boolean = True
        Private _databasePath As String = ""
        Private _debug As Boolean = False
        Private _displayTitles As Boolean = True
        Private _dragAndDrop As Boolean = True
        Private _imageConvertStartPathMode As ImageConvertPathMode = ImageConvertPathMode.LastSavedImage
        Private _language As String = ""
        Private _pendingDirty As Boolean = False
        Private _recentFiles As New List(Of String)
        Private _recentFolders As New List(Of String)
        Private _suspendUpdates As Integer = 0
        Private _windowHeight As Integer = 0
        Private _windowWidth As Integer = 0

        Public Enum ImageConvertPathMode
            SameAsFlux = 0
            ParentOfFlux = 1
            LastSavedImage = 2
        End Enum

        Private Sub New(filePath As String)
            _filePath = filePath

            Greaseweazle = New Flux.Greaseweazle.GreaseweazleSettings()
            Kryoflux = New Flux.Kryoflux.KryofluxSettings()
            PcImgCnv = New Flux.PcImgCnv.PcImgCnvSettings()
            Expert = New SettingsExpert()
        End Sub

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

        Public Property DatabasePath As String
            Get
                Return _databasePath
            End Get
            Set(value As String)
                If _databasePath <> value Then
                    _databasePath = value
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

        Public Property Expert As SettingsExpert

        Public Property Greaseweazle As Flux.Greaseweazle.GreaseweazleSettings

        Public Property ImageConvertStartPathMode As ImageConvertPathMode
            Get
                Return _imageConvertStartPathMode
            End Get
            Set(value As ImageConvertPathMode)
                If _imageConvertStartPathMode <> value Then
                    _imageConvertStartPathMode = value
                    IsDirty = True
                End If
            End Set
        End Property

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

        Public Property PcImgCnv As Flux.PcImgCnv.PcImgCnvSettings

        Public ReadOnly Property RecentFiles As IReadOnlyList(Of String)
            Get
                Return _recentFiles
            End Get
        End Property

        Public ReadOnly Property RecentFolders As IReadOnlyList(Of String)
            Get
                Return _recentFolders
            End Get
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
                settings._databasePath = ReadValue(root, "databasePath", settings._databasePath)
                settings._imageConvertStartPathMode = ReadValue(root, "imageConvertStartPathMode", settings._imageConvertStartPathMode)

                Dim files = ReadValue(Of List(Of String))(root, "recentFiles", Nothing)
                If files IsNot Nothing Then
                    settings._recentFiles = files _
                        .Where(Function(p) Not String.IsNullOrWhiteSpace(p)) _
                        .Take(MAX_RECENT_FILES) _
                        .ToList()
                End If

                Dim folders = ReadValue(Of List(Of String))(root, "recentFolders", Nothing)
                If folders IsNot Nothing Then
                    settings._recentFolders = folders _
                        .Where(Function(p) Not String.IsNullOrWhiteSpace(p)) _
                        .Take(MAX_RECENT_FOLDERS) _
                        .ToList()
                End If

                settings._Greaseweazle.LoadFromDictionary(ReadSection(root, "greaseweazle"))
                settings._Kryoflux.LoadFromDictionary(ReadSection(root, "kryoflux"))
                settings._PcImgCnv.LoadFromDictionary(ReadSection(root, "pcImgCnv"))
                settings._Expert.LoadFromDictionary(ReadSection(root, "expert"))

                settings.IsDirty = False

            Catch ex As Exception
                settings.IsDirty = True
            End Try

            Return settings
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            Save()
            GC.SuppressFinalize(Me)
        End Sub

        Public Sub RecentClearAll()
            If _recentFiles.Count = 0 AndAlso _recentFolders.Count = 0 Then
                Return
            End If
            _recentFiles.Clear()
            _recentFolders.Clear()
            SetDirty()
        End Sub

        Public Sub RecentFilesAdd(filePath As String)
            AddToRecentList(_recentFiles, filePath, MAX_RECENT_FILES)
        End Sub

        Public Sub RecentFilesBeginUpdate()
            _suspendUpdates += 1
        End Sub

        Public Sub RecentFilesClear()
            If _recentFiles.Count = 0 Then
                Return
            End If
            _recentFiles.Clear()
            SetDirty()
        End Sub

        Public Sub RecentFilesEndUpdate()
            If _suspendUpdates > 0 Then
                _suspendUpdates -= 1
            End If
            If _suspendUpdates = 0 AndAlso _pendingDirty Then
                _pendingDirty = False
                IsDirty = True
            End If
        End Sub

        Public Sub RecentFilesRemove(filePath As String)
            RemoveFromRecentList(_recentFiles, filePath)
        End Sub

        Public Sub RecentFoldersAdd(folderPath As String)
            AddToRecentList(_recentFolders, folderPath, MAX_RECENT_FOLDERS)
        End Sub

        Public Sub RecentFoldersClear()
            If _recentFolders.Count = 0 Then
                Return
            End If
            _recentFolders.Clear()
            SetDirty()
        End Sub

        Public Sub RecentFoldersRemove(folderPath As String)
            RemoveFromRecentList(_recentFolders, folderPath)
        End Sub

        Public Sub Save(Optional Force As Boolean = False)
            Dim sectionsDirty As Boolean = Greaseweazle.IsDirty OrElse Kryoflux.IsDirty OrElse PcImgCnv.IsDirty OrElse Expert.IsDirty

            If Not (IsDirty OrElse sectionsDirty OrElse Force) Then
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
                {"language", _language},
                {"imageConvertStartPathMode", _imageConvertStartPathMode}
            }

            If Not String.IsNullOrEmpty(_databasePath) Then
                root("databasePath") = _databasePath
            End If

            If _recentFiles.Count > 0 Then
                root("recentFiles") = _recentFiles
            End If
            If _recentFolders.Count > 0 Then
                root("recentFolders") = _recentFolders
            End If

            root("greaseweazle") = Greaseweazle.ToJsonObject()
            root("kryoflux") = Kryoflux.ToJsonObject()
            root("pcImgCnv") = PcImgCnv.ToJsonObject()
            root("expert") = Expert.ToJsonObject()

            Try
                Dim json = Serializer.ToString(root, True)
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath))
                File.WriteAllText(_filePath, json)

                IsDirty = False

                Greaseweazle.MarkClean()
                Kryoflux.MarkClean()
                PcImgCnv.MarkClean()
                Expert.MarkClean()
            Catch ex As Exception
                Diagnostics.Debug.Print("Error: Unable to save AppSettings")
            End Try
        End Sub

        Private Shared Function GetDefaultConfigPath() As String
            Dim DataPath = GetAppDataPath()
            Return Path.Combine(DataPath, DEFAULT_CONFIG_FILE)
        End Function

        Private Sub AddToRecentList(list As List(Of String), filePath As String, maxCount As Integer)
            If String.IsNullOrWhiteSpace(filePath) Then
                Return
            End If

            Dim normalized As String
            Try
                normalized = IO.Path.GetFullPath(filePath)
            Catch
                Return
            End Try

            Dim existing = list.FindIndex(Function(p) String.Equals(p, normalized, StringComparison.OrdinalIgnoreCase))

            If existing >= 0 Then
                list.RemoveAt(existing)
            End If

            list.Insert(0, normalized)

            If list.Count > maxCount Then
                list.RemoveRange(maxCount, list.Count - maxCount)
            End If

            SetDirty()
        End Sub

        Private Sub RemoveFromRecentList(list As List(Of String), filePath As String)
            If String.IsNullOrWhiteSpace(filePath) Then Return

            Dim normalized As String
            Try
                normalized = IO.Path.GetFullPath(filePath)
            Catch
                normalized = filePath
            End Try

            Dim removed = list.RemoveAll(Function(p) String.Equals(p, normalized, StringComparison.OrdinalIgnoreCase))

            If removed > 0 Then
                SetDirty()
            End If
        End Sub

        Private Sub SetDirty()
            If _suspendUpdates > 0 Then
                _pendingDirty = True
            Else
                IsDirty = True
            End If
        End Sub
    End Class
End Namespace
