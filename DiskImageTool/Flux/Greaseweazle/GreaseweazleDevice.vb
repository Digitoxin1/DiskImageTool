Namespace Flux.Greaseweazle
    Public Class GreaseweazleDevice
        Implements IDevice

        Private ReadOnly _TrackStatus As TrackStatus

        Private ReadOnly ValidInputTypes As New List(Of InputFileTypeEnum) From {
                InputFileTypeEnum.scp,
                InputFileTypeEnum.raw
            }

        Private ReadOnly ValidOutputTypes As New List(Of ImageImportOutputTypes) From {
                ImageImportOutputTypes.IMA,
                ImageImportOutputTypes.HFE
            }
        Public Sub New()
            _TrackStatus = New TrackStatus()
        End Sub

        Friend Shared ReadOnly Property Settings As ISettings
            Get
                Return App.Globals.AppSettings.Greaseweazle
            End Get
        End Property

        Friend Shared ReadOnly Property Capabilities As DeviceCapabilities = DeviceCapabilities.Read Or DeviceCapabilities.Write Or DeviceCapabilities.Convert

        Friend ReadOnly Property Device As IDevice.FluxDevice Implements IDevice.Device
            Get
                Return IDevice.FluxDevice.Greaseweazle
            End Get
        End Property

        Friend ReadOnly Property Name As String Implements IDevice.Name
            Get
                Return "Greaseweazle"
            End Get
        End Property

        Friend ReadOnly Property RequiresImageFormat As Boolean Implements IDevice.RequiresImageFormat
            Get
                Return True
            End Get
        End Property

        Friend ReadOnly Property SupportsPreview As Boolean Implements IDevice.SupportsPreview
            Get
                Return True
            End Get
        End Property

        Friend ReadOnly Property TrackStatus As ITrackStatus Implements IDevice.TrackStatus
            Get
                Return _TrackStatus
            End Get
        End Property

        Private ReadOnly Property IDevice_Capabilities As DeviceCapabilities Implements IDevice.Capabilities
            Get
                Return Capabilities
            End Get
        End Property

        Private ReadOnly Property IDevice_Settings As ISettings Implements IDevice.Settings
            Get
                Return Settings
            End Get
        End Property
        Friend Function ConvertFirstTrack(InputFilePath As String, BothSides As Boolean, Optional ImageParams As DiskImage.FloppyDiskParams? = Nothing) As (Result As Boolean, Filename As String) Implements IDevice.ConvertFirstTrack
            Return Greaseweazle.ConvertFirstTrack(InputFilePath, BothSides, ImageParams)
        End Function

        Friend Function InputTypeSupported(fileType As InputFileTypeEnum) As Boolean Implements IDevice.InputTypeSupported
            Return ValidInputTypes.Contains(fileType)
        End Function

        Friend Function OutputTypeSupported(fileType As ImageImportOutputTypes) As Boolean Implements IDevice.OutputTypeSupported
            Return ValidOutputTypes.Contains(fileType)
        End Function
    End Class
End Namespace
