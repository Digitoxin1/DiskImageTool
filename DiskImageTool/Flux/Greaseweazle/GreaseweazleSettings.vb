Imports CompactJson
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Public Class GreaseweazleSettings
        Implements DiskImageTool.Settings.ISettingsGroup

        Public Const MAX_TRACKS As Byte = 84
        Public Const MAX_TRACKS_525DD As Byte = 42
        Public Const MIN_TRACKS As Byte = 80
        Public Const MIN_TRACKS_525DD As Byte = 40

        Private Const MaxDrives As Integer = 3
        Private Const DEFAULT_LOG_FILE_NAME As String = "log.txt"

        Private _appPath As String = ""
        Private _comPort As String = ""
        Private _defaultRevs As Integer = 3
        Private _Drives As New List(Of DriveSettings) From {
                        New DriveSettings(),
                        New DriveSettings(),
                        New DriveSettings()
                    }

        Private _fluxRootPath As String = ""
        Private _interface As GreaseweazleInterface = GreaseweazleInterface.IBM
        Private _isDirty As Boolean
        Private _logFileName As String = DEFAULT_LOG_FILE_NAME
        Public Enum GreaseweazleInterface
            IBM
            Shugart
        End Enum

        Public Property [Interface] As GreaseweazleInterface
            Get
                Return _interface
            End Get
            Set(value As GreaseweazleInterface)
                If _interface <> value Then
                    _interface = value
                    _isDirty = True
                End If
            End Set
        End Property

        Public Property AppPath As String
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

        Public ReadOnly Property AvailableDriveTypes As FloppyMediaType
            Get
                Dim AvailableTypes As FloppyMediaType = Drives(0).Type Or Drives(1).Type
                If [Interface] = GreaseweazleInterface.Shugart Then
                    AvailableTypes = AvailableTypes Or Drives(2).Type
                End If

                Return AvailableTypes
            End Get
        End Property

        Public Property ComPort As String
            Get
                Return _comPort
            End Get
            Set(value As String)
                If _comPort <> value Then
                    _comPort = value
                    _isDirty = True
                End If
            End Set
        End Property

        Public Property DefaultRevs As Integer
            Get
                Dim value As UShort = _defaultRevs
                If value < CommandLineBuilder.MIN_REVS Then
                    value = CommandLineBuilder.MIN_REVS
                ElseIf value > CommandLineBuilder.MAX_REVS Then
                    value = CommandLineBuilder.MAX_REVS
                End If

                Return value
            End Get
            Set(value As Integer)
                If value < CommandLineBuilder.MIN_REVS Then
                    value = CommandLineBuilder.MIN_REVS
                ElseIf value > CommandLineBuilder.MAX_REVS Then
                    value = CommandLineBuilder.MAX_REVS
                End If

                If _defaultRevs <> value Then
                    _defaultRevs = value
                    _isDirty = True
                End If
            End Set
        End Property

        Public Property Drives As List(Of DriveSettings)
            Get
                Return _Drives
            End Get
            Set
                _Drives = Value
            End Set
        End Property

        Public Property FluxRootPath As String
            Get
                Return _fluxRootPath
            End Get
            Set(value As String)
                If _fluxRootPath <> value Then
                    _fluxRootPath = value
                    _isDirty = True
                End If
            End Set
        End Property
        Public Property IsDirty As Boolean Implements DiskImageTool.Settings.ISettingsGroup.IsDirty
            Get
                If _isDirty Then Return True

                If Drives IsNot Nothing Then
                    For Each d In Drives
                        If d IsNot Nothing AndAlso d.IsDirty Then
                            Return True
                        End If
                    Next
                End If

                Return False
            End Get
            Set(value As Boolean)
                _isDirty = value

                If Not value AndAlso Drives IsNot Nothing Then
                    For Each d In Drives
                        d?.MarkClean()
                    Next
                End If
            End Set
        End Property

        Public Property LogFileName As String
            Get
                Return If(String.IsNullOrEmpty(_logFileName), DEFAULT_LOG_FILE_NAME, _logFileName)
            End Get
            Set(value As String)
                If _logFileName <> value Then
                    _logFileName = value
                    _isDirty = True
                End If
            End Set
        End Property

        Public Function IsPathValid() As Boolean
            Return Flux.IsPathValid(_appPath)
        End Function

        Public Sub LoadFromDictionary(dict As Dictionary(Of String, JsonValue))
            If dict Is Nothing Then
                MarkClean()
                Return
            End If

            _appPath = ReadValue(dict, "appPath", _appPath)
            _interface = GetInterfaceFromName(ReadValue(dict, "interface", GetInterfaceName(_interface)))
            _comPort = ReadValue(dict, "comPort", _comPort)
            _defaultRevs = ReadValue(dict, "defaultRevs", _defaultRevs)
            _logFileName = ReadValue(dict, "logFileName", _logFileName)
            _fluxRootPath = ReadValue(dict, "fluxRootPath", _fluxRootPath)

            LoadDriveList(dict)

            MarkClean()
        End Sub

        Public Sub MarkClean() Implements DiskImageTool.Settings.ISettingsGroup.MarkClean
            IsDirty = False
        End Sub

        Public Function ToJsonObject() As Dictionary(Of String, Object)
            Dim driveArray As New List(Of Object)
            If Drives IsNot Nothing Then
                For Each d In Drives
                    driveArray.Add(d.ToJsonObject())
                Next
            End If

            Return New Dictionary(Of String, Object) From {
                {"appPath", _appPath},
                {"interface", GetInterfaceName(_interface)},
                {"drives", driveArray},
                {"comPort", _comPort},
                {"defaultRevs", _defaultRevs},
                {"logFileName", _logFileName},
                {"fluxRootPath", _fluxRootPath}
            }
        End Function

        Private Shared Function GetInterfaceFromName(Value As String) As GreaseweazleInterface
            Select Case Value
                Case "Shugart"
                    Return GreaseweazleInterface.Shugart
                Case Else
                    Return GreaseweazleInterface.IBM
            End Select
        End Function

        Private Shared Function GetInterfaceName(Value As GreaseweazleInterface) As String
            Select Case Value
                Case GreaseweazleInterface.Shugart
                    Return "Shugart"
                Case Else
                    Return "IBM"
            End Select
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

        Private Sub LoadDriveList(parent As Dictionary(Of String, JsonValue))

            Dim drivesValue As JsonValue = Nothing
            If Not parent.TryGetValue("drives", drivesValue) OrElse drivesValue Is Nothing Then
                Return
            End If

            Try
                ' Parse into list of dictionaries
                Dim arr = Serializer.Parse(Of List(Of Dictionary(Of String, JsonValue)))(drivesValue.ToString())

                Dim newList As New List(Of DriveSettings)(MaxDrives)

                For i = 0 To MaxDrives - 1
                    Dim d As New DriveSettings
                    If i < arr.Count Then
                        d.LoadFromDictionary(arr(i))
                    End If

                    newList.Add(d)

                Next

                _Drives = newList

            Catch
                ' Fail silently, keep existing list
            End Try
        End Sub
    End Class

End Namespace
