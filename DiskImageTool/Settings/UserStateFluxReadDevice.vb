Imports CompactJson
Imports DiskImageTool.Flux.Greaseweazle

Namespace Settings
    Public Class UserStateFluxReadDevice
        Inherits SettingsGroup

        Private _DriveId As String = ""
        Private _OutputType As ReadDiskOutputTypes = ReadDiskOutputTypes.None
        Private _OutputType2 As ReadDiskOutputTypes = ReadDiskOutputTypes.None
        Private _ImageLocation As ReadDiskImageLocations?
        Private _ImageFolder As String
        Private _RootFolder As String
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

        Friend Property ImageLocation As ReadDiskImageLocations?
            Get
                Return _ImageLocation
            End Get
            Set
                If Not Nullable.Equals(_ImageLocation, Value) Then
                    _ImageLocation = Value
                    MarkDirty()
                End If
            End Set
        End Property

        Friend Property ImageFolder As String
            Get
                Return _ImageFolder
            End Get
            Set
                If _ImageFolder <> Value Then
                    _ImageFolder = Value
                    MarkDirty()
                End If
            End Set
        End Property

        Friend Property RootFolder As String
            Get
                Return _RootFolder
            End Get
            Set
                If _RootFolder <> Value Then
                    _RootFolder = Value
                    MarkDirty()
                End If
            End Set
        End Property

        Friend Property OutputType As ReadDiskOutputTypes
            Get
                Return _OutputType
            End Get
            Set
                If _OutputType <> Value Then
                    _OutputType = Value
                    MarkDirty()
                End If
            End Set
        End Property

        Friend Property OutputType2 As ReadDiskOutputTypes
            Get
                Return _OutputType2
            End Get
            Set
                If OutputType2 <> Value Then
                    _OutputType2 = Value
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
            _OutputType2 = ReadValue(dict, "outputType2", _OutputType2)
            _ImageLocation = ReadValue(dict, "imageLocation", _ImageLocation)
            _ImageFolder = ReadValue(dict, "imageFolder", _ImageFolder)
            _RootFolder = ReadValue(dict, "rootFolder", _RootFolder)

            MarkClean()
        End Sub

        Public Function ToJsonObject() As Dictionary(Of String, Object)
            Dim result As New Dictionary(Of String, Object)

            result("saveLog") = _SaveLog
            result("driveId") = _DriveId
            If _OutputType <> ReadDiskOutputTypes.None Then
                result("outputType") = _OutputType.ToString()
            End If
            If _OutputType2 <> ReadDiskOutputTypes.None Then
                result("outputType2") = _OutputType2.ToString()
            End If
            If _ImageLocation.HasValue Then
                result("imageLocation") = _ImageLocation.Value.ToString()
            End If
            If Not String.IsNullOrEmpty(_ImageFolder) Then
                result("imageFolder") = _ImageFolder
            End If
            If Not String.IsNullOrEmpty(_RootFolder) Then
                result("rootFolder") = _RootFolder
            End If

            Return result
        End Function
    End Class
End Namespace
