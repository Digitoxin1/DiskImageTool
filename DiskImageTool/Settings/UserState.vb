Imports System.IO
Imports CompactJson
Imports DiskImageTool.Settings.SettingsGroup

Namespace Settings
    Public Class UserState
        Implements IDisposable
        Public Const MAX_RECENT_FILES As Integer = 10
        Public Const MAX_RECENT_FOLDERS As Integer = 5
        Const DEFAULT_CONFIG_FILE As String = "userState.json"

        Private ReadOnly _filePath As String

        Private _lastExportFilePath As String = ""
        Private _lastNewImagePath As String = ""
        Private _pendingDirty As Boolean = False
        Private _preferredFileExtensions As New Dictionary(Of DiskImage.FloppyDiskFormat, String)
        Private _preferredFileExtensionsOriginal As Dictionary(Of DiskImage.FloppyDiskFormat, String)
        Private _recentFiles As New List(Of String)
        Private _recentFolders As New List(Of String)
        Private _suspendUpdates As Integer = 0
        Private _windowHeight As Integer = 0
        Private _windowWidth As Integer = 0

        Private Sub New(filePath As String)
            _filePath = filePath

            ETags = New UserStateETags()
            Flux = New UserStateFlux
        End Sub

        Public Property ETags As UserStateETags
        Public Property Flux As UserStateFlux
        Public Property IsDirty As Boolean

        Public Property LastExportFilePath As String
            Get
                Return _lastExportFilePath
            End Get
            Set(value As String)
                If _lastExportFilePath <> value Then
                    _lastExportFilePath = value
                    IsDirty = True
                End If
            End Set
        End Property

        Public Property LastNewImagePath As String
            Get
                Return _lastNewImagePath
            End Get
            Set(value As String)
                If _lastNewImagePath <> value Then
                    _lastNewImagePath = value
                    IsDirty = True
                End If
            End Set
        End Property

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

        Public Shared Function Load(Optional configPath As String = Nothing) As UserState
            Dim path = If(configPath, GetDefaultConfigPath())
            Dim userState As New UserState(path)

            If Not File.Exists(path) Then
                userState.Save(True)
                Return userState
            End If

            Try
                Dim json = File.ReadAllText(path)

                If String.IsNullOrWhiteSpace(json) Then
                    userState.Save(True)
                    Return userState
                End If

                Dim root = Serializer.Parse(Of Dictionary(Of String, JsonValue))(json)

                ' top-level simple values (use backing fields to avoid IsDirty during load)
                userState._lastNewImagePath = ReadValue(root, "lastNewImagePath", userState._lastNewImagePath)
                userState._lastExportFilePath = ReadValue(root, "lastExportFilePath", userState._lastExportFilePath)
                userState._windowWidth = ReadValue(root, "windowWidth", userState._windowWidth)
                userState._windowHeight = ReadValue(root, "windowHeight", userState._windowHeight)
                userState._ETags.LoadFromDictionary(ReadSection(root, "eTags"))
                userState._Flux.LoadFromDictionary(ReadSection(root, "flux"))

                Dim files = ReadValue(Of List(Of String))(root, "recentFiles", Nothing)
                If files IsNot Nothing Then
                    userState._recentFiles = files _
                        .Where(Function(p) Not String.IsNullOrWhiteSpace(p)) _
                        .Take(MAX_RECENT_FILES) _
                        .ToList()
                End If

                Dim folders = ReadValue(Of List(Of String))(root, "recentFolders", Nothing)
                If folders IsNot Nothing Then
                    userState._recentFolders = folders _
                        .Where(Function(p) Not String.IsNullOrWhiteSpace(p)) _
                        .Take(MAX_RECENT_FOLDERS) _
                        .ToList()
                End If

                userState._preferredFileExtensions = LoadPreferredExtensions(root)
                userState._preferredFileExtensionsOriginal = CloneDictionary(userState._preferredFileExtensions)

                userState.IsDirty = False

            Catch ex As Exception
                Console.WriteLine(ex.ToString())
                userState.IsDirty = True
            End Try

            Return userState
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            Save()
            GC.SuppressFinalize(Me)
        End Sub

        Public Function GetPreferredExtension(Format As DiskImage.FloppyDiskFormat) As String
            Dim Extension As String = ""
            _preferredFileExtensions.TryGetValue(Format, Extension)

            Return Extension
        End Function

        Public Sub RemovePreferredExtension(Format As DiskImage.FloppyDiskFormat)
            If _preferredFileExtensions.ContainsKey(Format) Then
                _preferredFileExtensions.Remove(Format)
                IsDirty = True
            End If
        End Sub

        Public Sub Save(Optional Force As Boolean = False)
            Dim prefsDirty As Boolean = Not DictionariesEqual(_preferredFileExtensions, _preferredFileExtensionsOriginal)
            Dim sectionsDirty As Boolean = ETags.IsDirty OrElse Flux.IsDirty

            If Not (IsDirty OrElse sectionsDirty OrElse prefsDirty OrElse Force) Then
                Return
            End If

            Dim root As New Dictionary(Of String, Object) From {
                 {"lastNewImagePath", _lastNewImagePath},
                 {"lastExportFilePath", _lastExportFilePath},
                 {"windowWidth", _windowWidth},
                 {"windowHeight", _windowHeight}
            }

            If _recentFiles.Count > 0 Then
                root("recentFiles") = _recentFiles
            End If
            If _recentFolders.Count > 0 Then
                root("recentFolders") = _recentFolders
            End If

            root("eTags") = ETags.ToJsonObject()
            root("flux") = Flux.ToJsonObject()

            Dim prefArr As New List(Of Object)

            For Each kvp In _preferredFileExtensions
                prefArr.Add(New Dictionary(Of String, Object) From {
                    {"format", kvp.Key.ToString()},
                    {"extension", kvp.Value}
                })
            Next

            root("preferredFileExtensions") = prefArr

            Try
                Dim json = Serializer.ToString(root, True)
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath))
                File.WriteAllText(_filePath, json)

                IsDirty = False
                ETags.MarkClean()
                Flux.MarkClean()
            Catch ex As Exception
                Diagnostics.Debug.Print("Error: Unable to save AppSettings")
            End Try
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

        Public Sub SetPreferredExtension(Format As DiskImage.FloppyDiskFormat, Extension As String)
            If _preferredFileExtensions.ContainsKey(Format) Then
                _preferredFileExtensions.Item(Format) = Extension
            Else
                _preferredFileExtensions.Add(Format, Extension)
            End If
        End Sub

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

        Private Shared Function GetDefaultConfigPath() As String
            Dim DataPath = GetAppDataPath()
            Return Path.Combine(DataPath, DEFAULT_CONFIG_FILE)
        End Function

        Private Shared Function LoadPreferredExtensions(root As Dictionary(Of String, JsonValue)) As Dictionary(Of DiskImage.FloppyDiskFormat, String)
            Dim peValue As JsonValue = Nothing

            Dim newDict As New Dictionary(Of DiskImage.FloppyDiskFormat, String)

            If root.TryGetValue("preferredFileExtensions", peValue) AndAlso peValue IsNot Nothing Then
                Try
                    Dim raw = peValue.ToString()
                    Dim arr = Serializer.Parse(Of List(Of Dictionary(Of String, JsonValue)))(raw)

                    For Each item In arr
                        Try
                            Dim fmtStr = ReadValue(item, "format", "")
                            Dim ext = ReadValue(item, "extension", "")

                            If fmtStr <> "" Then
                                Dim fmt As DiskImage.FloppyDiskFormat
                                If [Enum].TryParse(fmtStr, fmt) Then
                                    newDict(fmt) = ext
                                End If
                            End If

                        Catch
                            ' ignore malformed entries (per-item fallback)
                        End Try
                    Next
                Catch
                    ' bad array → keep defaults
                End Try
            End If

            Return newDict
        End Function
    End Class
End Namespace
