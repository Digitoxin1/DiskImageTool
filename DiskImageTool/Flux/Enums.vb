Imports System.Runtime.CompilerServices

Namespace Flux
    Module Enums
        Private Const SCP_EXTENSION As String = ".scp"
        Private Const RAW_EXTENSION As String = ".raw"
        Private Const A2R_EXTENSION As String = ".a2r"

        Enum ActionTypeEnum
            Read
            Write
            [Erase]
            Complete
        End Enum

        Public Enum ConversionMode
            Import
            Save
        End Enum

        Public Enum FluxFileTypeEnum
            None
            SectorImage
            HFE
            MFM
            F86
            TC
            SCP
            RAW
            A2R
        End Enum

        Friend Enum DeviceCapabilities
            None = 0
            Read = 1
            Write = 2
            Convert = 4
        End Enum

        Friend Enum TrackHeads
            Head0
            Head1
            Both
        End Enum

        Public Function FluxFileTypeFromExtension(Extension As String) As FluxFileTypeEnum
            Select Case Extension.ToLower()
                Case FloppyImageFormats.GetInfo(DiskImage.FloppyImageType.HFEImage).Extension
                    Return FluxFileTypeEnum.HFE
                Case FloppyImageFormats.GetInfo(DiskImage.FloppyImageType.MFMImage).Extension
                    Return FluxFileTypeEnum.MFM
                Case FloppyImageFormats.GetInfo(DiskImage.FloppyImageType.D86FImage).Extension
                    Return FluxFileTypeEnum.F86
                Case FloppyImageFormats.GetInfo(DiskImage.FloppyImageType.TranscopyImage).Extension
                    Return FluxFileTypeEnum.TC
                Case SCP_EXTENSION
                    Return FluxFileTypeEnum.SCP
                Case RAW_EXTENSION
                    Return FluxFileTypeEnum.RAW
                Case A2R_EXTENSION
                    Return FluxFileTypeEnum.A2R
                Case Else
                    Return FluxFileTypeEnum.SectorImage
            End Select
        End Function

        <Extension()>
        Public Function GetDescription(Value As FluxFileTypeEnum) As String
            Select Case Value
                Case FluxFileTypeEnum.SectorImage
                    Return My.Resources.FloppyImageType_BasicSectorImage
                Case FluxFileTypeEnum.HFE
                    Return String.Format(My.Resources.FloppyImageType_Image, My.Resources.FloppyImageType_HFE)
                Case FluxFileTypeEnum.MFM
                    Return String.Format(My.Resources.FloppyImageType_Image, My.Resources.FloppyImageType_MFM)
                Case FluxFileTypeEnum.F86
                    Return String.Format(My.Resources.FloppyImageType_Image, My.Resources.FloppyImageType_86F)
                Case FluxFileTypeEnum.TC
                    Return String.Format(My.Resources.FloppyImageType_Image, My.Resources.FloppyImageType_TC)
                Case FluxFileTypeEnum.SCP
                    Return String.Format(My.Resources.FloppyImageType_Image, My.Resources.FloppyImageType_SCP)
                Case FluxFileTypeEnum.RAW
                    Return My.Resources.FloppyImageType_RAWImage
                Case FluxFileTypeEnum.A2R
                    Return String.Format(My.Resources.FloppyImageType_Image, My.Resources.FloppyImageType_A2R)
                Case Else
                    Return ""
            End Select
        End Function

        <Extension()>
        Public Function GetExtension(Value As FluxFileTypeEnum) As String
            Select Case Value
                Case FluxFileTypeEnum.HFE
                    Return FloppyImageFormats.GetInfo(DiskImage.FloppyImageType.HFEImage).Extension
                Case FluxFileTypeEnum.MFM
                    Return FloppyImageFormats.GetInfo(DiskImage.FloppyImageType.MFMImage).Extension
                Case FluxFileTypeEnum.F86
                    Return FloppyImageFormats.GetInfo(DiskImage.FloppyImageType.D86FImage).Extension
                Case FluxFileTypeEnum.TC
                    Return FloppyImageFormats.GetInfo(DiskImage.FloppyImageType.TranscopyImage).Extension
                Case FluxFileTypeEnum.SCP
                    Return SCP_EXTENSION
                Case FluxFileTypeEnum.RAW
                    Return RAW_EXTENSION
                Case FluxFileTypeEnum.A2R
                    Return A2R_EXTENSION
                Case Else
                    Return FloppyImageFormats.GetInfo(DiskImage.FloppyImageType.BasicSectorImage).Extension
            End Select
        End Function

        <Extension()>
        Public Function IsFlux(Value As FluxFileTypeEnum) As Boolean
            Return Value = FluxFileTypeEnum.RAW OrElse Value = FluxFileTypeEnum.SCP OrElse Value = FluxFileTypeEnum.A2R
        End Function
    End Module
End Namespace
