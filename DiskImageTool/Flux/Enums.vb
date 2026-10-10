Imports System.Runtime.CompilerServices

Namespace Flux
    Module Enums
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
                Case ".hfe"
                    Return FluxFileTypeEnum.HFE
                Case ".mfm"
                    Return FluxFileTypeEnum.MFM
                Case ".86f"
                    Return FluxFileTypeEnum.F86
                Case ".tc"
                    Return FluxFileTypeEnum.TC
                Case ".scp"
                    Return FluxFileTypeEnum.SCP
                Case ".raw"
                    Return FluxFileTypeEnum.RAW
                Case ".a2r"
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
                    Return ".hfe"
                Case FluxFileTypeEnum.MFM
                    Return ".mfm"
                Case FluxFileTypeEnum.F86
                    Return ".86f"
                Case FluxFileTypeEnum.TC
                    Return ".tc"
                Case FluxFileTypeEnum.SCP
                    Return ".scp"
                Case FluxFileTypeEnum.RAW
                    Return ".raw"
                Case FluxFileTypeEnum.A2R
                    Return ".a2r"
                Case Else
                    Return ".ima"
            End Select
        End Function

        <Extension()>
        Public Function IsFlux(Value As FluxFileTypeEnum) As Boolean
            Return Value = FluxFileTypeEnum.RAW OrElse Value = FluxFileTypeEnum.SCP OrElse Value = FluxFileTypeEnum.A2R
        End Function
    End Module
End Namespace
