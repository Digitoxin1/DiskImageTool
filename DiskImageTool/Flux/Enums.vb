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

        Public Function FluxFileTypeDescription(Value As FluxFileTypeEnum) As String
            Select Case Value
                Case FluxFileTypeEnum.SectorImage
                    Return My.Resources.FloppyImageType_BasicSectorImage
                Case FluxFileTypeEnum.HFE
                    Return My.Resources.FloppyImageType_HFEImage
                Case FluxFileTypeEnum.MFM
                    Return My.Resources.FloppyImageType_MFMImage
                Case FluxFileTypeEnum.F86
                    Return My.Resources.FloppyImageType_D86FImage
                Case FluxFileTypeEnum.TC
                    Return My.Resources.FloppyImageType_TranscopyImage
                Case FluxFileTypeEnum.SCP
                    Return My.Resources.FloppyImageType_SCPImage
                Case FluxFileTypeEnum.RAW
                    Return My.Resources.FloppyImageType_RAWImage
                Case FluxFileTypeEnum.A2R
                    Return My.Resources.FloppyImageType_A2RImage
                Case Else
                    Return ""
            End Select
        End Function

        Public Function FluxFileTypeExtension(Value As FluxFileTypeEnum) As String
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

        Public Function FileTypeIsFlux(Value As FluxFileTypeEnum) As Boolean
            Return Value = FluxFileTypeEnum.RAW OrElse Value = FluxFileTypeEnum.SCP OrElse Value = FluxFileTypeEnum.A2R
        End Function
    End Module
End Namespace
