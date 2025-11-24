Imports DiskImageTool.Flux
Imports CompactJson

Namespace Settings
    Public Class UserStateFluxConvert
        Inherits SettingsGroup

        Private ReadOnly _Devices As Dictionary(Of IDevice.FluxDevice, UserStateFluxConvertDevice)
        Private _LastDevice As IDevice.FluxDevice?

        Public Sub New()
            _Devices = New Dictionary(Of IDevice.FluxDevice, UserStateFluxConvertDevice)
            For Each dev In [Enum].GetValues(GetType(IDevice.FluxDevice))
                If (FluxDeviceGetCapabilities(CType(dev, IDevice.FluxDevice)) And DeviceCapabilities.Convert) = 0 Then
                    Continue For
                End If
                _Devices(CType(dev, IDevice.FluxDevice)) = New UserStateFluxConvertDevice()
            Next
        End Sub

        Public Overrides ReadOnly Property IsDirty As Boolean
            Get
                If MyBase.IsDirty Then Return True

                For Each item In _Devices.Values
                    If item.IsDirty Then
                        Return True
                    End If
                Next

                Return False
            End Get
        End Property

        Friend Property LastDevice As IDevice.FluxDevice?
            Get
                Return _LastDevice
            End Get
            Set(value As IDevice.FluxDevice?)
                If Not Nullable.Equals(_LastDevice, value) Then
                    _LastDevice = value
                    MarkDirty()
                End If
            End Set
        End Property

        Friend ReadOnly Property Device(DeviceId As IDevice.FluxDevice) As UserStateFluxConvertDevice
            Get
                Return _Devices.Item(DeviceId)
            End Get
        End Property

        Public Sub LoadFromDictionary(dict As Dictionary(Of String, JsonValue))
            If dict Is Nothing Then
                MarkClean()
                Return
            End If

            _LastDevice = ReadValue(dict, "lastDevice", _LastDevice)

            LoadDevices(dict)

            MarkClean()
        End Sub

        Public Overrides Sub MarkClean()
            MyBase.MarkClean()

            For Each item In _Devices.Values
                item.MarkClean()
            Next
        End Sub

        Public Function ToJsonObject() As Dictionary(Of String, Object)
            Dim result As New Dictionary(Of String, Object)

            Dim DeviceDict As New Dictionary(Of String, Object)
            For Each Item In _Devices
                DeviceDict(Item.Key.ToString()) = Item.Value.ToJsonObject
            Next

            If _LastDevice.HasValue Then
                result("lastDevice") = _LastDevice.Value.ToString()
            End If
            result("devices") = DeviceDict

            Return result
        End Function

        Private Sub LoadDevices(dict As Dictionary(Of String, JsonValue))
            Dim DeviceSection = ReadSection(dict, "devices")

            If DeviceSection IsNot Nothing Then
                For Each kvp In DeviceSection
                    Dim DeviceName As String = kvp.Key
                    Dim DeviceValue As JsonValue = kvp.Value

                    Dim DeviceEnum As IDevice.FluxDevice
                    If [Enum].TryParse(DeviceName, ignoreCase:=True, result:=DeviceEnum) Then
                        Dim DeviceSettings As UserStateFluxConvertDevice = Nothing
                        If _Devices.TryGetValue(DeviceEnum, DeviceSettings) Then
                            Dim DeviceObj = TryCast(DeviceValue, JsonObject)
                            If DeviceObj IsNot Nothing Then
                                DeviceSettings.LoadFromDictionary(DeviceObj)
                            End If
                        End If
                    End If
                Next
            End If
        End Sub
    End Class
End Namespace