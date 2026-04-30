Namespace Flux
    Friend Interface IDevice
        Enum FluxDevice
            Greaseweazle
            Kryoflux
            PcImgCnv
        End Enum
        ReadOnly Property Capabilities As DeviceCapabilities
        ReadOnly Property Device As FluxDevice
        ReadOnly Property Name As String
        ReadOnly Property RequiresImageFormat As Boolean
        ReadOnly Property Settings As ISettings
        ReadOnly Property TrackStatus As ITrackStatus
        Function InputTypeSupported(fileType As InputFileTypeEnum) As Boolean
        Function OutputTypeSupported(fileType As ImageImportOutputTypes) As Boolean
    End Interface
End Namespace
