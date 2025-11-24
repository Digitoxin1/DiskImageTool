Imports System.IO
Imports CompactJson
Imports DiskImageTool.Settings.SettingsGroup

Namespace Settings
    Public Class UserState
        Implements IDisposable
        Const DEFAULT_CONFIG_FILE As String = "userState.json"

        Private ReadOnly _filePath As String

        Private _preferredFileExtensions As New Dictionary(Of DiskImage.FloppyDiskFormat, String)
        Private _preferredFileExtensionsOriginal As Dictionary(Of DiskImage.FloppyDiskFormat, String)

        Private Sub New(filePath As String)
            _filePath = filePath

            Flux = New UserStateFlux
        End Sub

        Public Property Flux As UserStateFlux
        Public Property IsDirty As Boolean

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
                userState._Flux.LoadFromDictionary(ReadSection(root, "flux"))

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
            Dim sectionsDirty As Boolean = Flux.IsDirty

            If Not (IsDirty OrElse sectionsDirty OrElse prefsDirty OrElse Force) Then
                Return
            End If

            Dim root As New Dictionary(Of String, Object)

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
                Flux.MarkClean()
            Catch ex As Exception
                Diagnostics.Debug.Print("Error: Unable to save AppSettings")
            End Try
        End Sub

        Public Sub SetPreferredExtension(Format As DiskImage.FloppyDiskFormat, Extension As String)
            If _preferredFileExtensions.ContainsKey(Format) Then
                _preferredFileExtensions.Item(Format) = Extension
            Else
                _preferredFileExtensions.Add(Format, Extension)
            End If
        End Sub

        Private Shared Function GetDefaultConfigPath() As String
            Dim baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            Dim appName = My.Application.Info.AssemblyName
            Dim appFolder = Path.Combine(baseFolder, appName)
            Directory.CreateDirectory(appFolder)
            Return Path.Combine(appFolder, DEFAULT_CONFIG_FILE)
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
