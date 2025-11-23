Namespace Flux
    Module DeviceLib
        Public Function FluxDeviceGetList(FileType As InputFileTypeEnum) As List(Of IDevice)
            Dim list As New List(Of IDevice)

            For Each dev As IDevice.FluxDevice In [Enum].GetValues(GetType(IDevice.FluxDevice))

                ' Skip if not available
                If Not FluxDeviceIsAvailable(dev) Then
                    Continue For
                End If

                ' Get FluxDeviceInfo
                Dim Device = FluxDeviceGet(dev)
                If Device IsNot Nothing Then
                    If Not Device.InputTypeSupported(FileType) Then
                        Continue For
                    End If
                    list.Add(Device)
                End If
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
