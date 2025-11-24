Namespace Flux
    Module DeviceLib
        Public Function FluxDeviceGetCapabilities(Device As IDevice.FluxDevice) As DeviceCapabilities
            Select Case Device
                Case IDevice.FluxDevice.Greaseweazle
                    Return Greaseweazle.GreaseweazleDevice.Capabilities

                Case IDevice.FluxDevice.Kryoflux
                    Return Kryoflux.KryofluxDevice.Capabilities

                Case IDevice.FluxDevice.PcImgCnv
                    Return PcImgCnv.PcImgCnvDevice.Capabilities

                Case Else
                    Return DeviceCapabilities.None
            End Select
        End Function

        Public Function FluxDeviceGetList(FileType As InputFileTypeEnum) As List(Of IDevice)
            Dim list As New List(Of IDevice)

            For Each dev As IDevice.FluxDevice In [Enum].GetValues(GetType(IDevice.FluxDevice))
                ' Skip if not available
                If Not FluxDeviceIsAvailable(dev) Then
                    Continue For
                End If

                ' Get FluxDeviceInfo
                Dim Device = FluxDeviceGet(dev)

                If Device Is Nothing Then
                    Continue For
                End If

                If (Device.Capabilities And DeviceCapabilities.Convert) = 0 Then
                    Continue For
                End If

                If Not Device.InputTypeSupported(FileType) Then
                    Continue For
                End If

                list.Add(Device)
            Next

            Return list
        End Function

        Private Function FluxDeviceGet(Device As IDevice.FluxDevice) As IDevice
            Select Case Device
                Case IDevice.FluxDevice.Greaseweazle
                    Return New Greaseweazle.GreaseweazleDevice

                Case IDevice.FluxDevice.Kryoflux
                    Return New Kryoflux.KryofluxDevice

                Case IDevice.FluxDevice.PcImgCnv
                    Return New PcImgCnv.PcImgCnvDevice

                Case Else
                    Return Nothing
            End Select
        End Function
        Private Function FluxDeviceIsAvailable(Device As IDevice.FluxDevice) As Boolean
            Select Case Device
                Case IDevice.FluxDevice.Greaseweazle
                    Return App.AppSettings.Greaseweazle.IsPathValid
                Case IDevice.FluxDevice.Kryoflux
                    Return App.AppSettings.Kryoflux.IsPathValid
                Case IDevice.FluxDevice.PcImgCnv
                    Return App.AppSettings.PcImgCnv.IsPathValid
                Case Else
                    Return False
            End Select
        End Function
    End Module
End Namespace
