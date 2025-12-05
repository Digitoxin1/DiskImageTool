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
        ReadOnly Property SupportsPreview As Boolean
        ReadOnly Property TrackStatus As ITrackStatus
        Function ConvertFirstTrack(InputFilePath As String, BothSides As Boolean, Optional ImageParams As DiskImage.FloppyDiskParams? = Nothing) As (Result As Boolean, Filename As String)
        Function InputTypeSupported(fileType As InputFileTypeEnum) As Boolean
        Function OutputTypeSupported(fileType As ImageImportOutputTypes) As Boolean
    End Interface
End Namespace
