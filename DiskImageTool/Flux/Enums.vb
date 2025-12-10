Namespace Flux
    Module Enums
        Enum ActionTypeEnum
            Unknown
            Read
            Write
            [Erase]
            Import
            Complete
        End Enum

        Public Enum ImageImportOutputTypes
            IMA
            HFE
            MFM
            F86
            TC
        End Enum

        Public Enum InputFileTypeEnum
            none
            scp
            raw
            sectorImage
        End Enum

        Friend Enum TrackHeads
            head0
            head1
            both
        End Enum

        Friend Enum DeviceCapabilities
            None = 0
            Read = 1
            Write = 2
            Convert = 4
        End Enum

        Public Function ImageImportOutputTypeDescription(Value As ImageImportOutputTypes) As String
            Select Case Value
                Case ImageImportOutputTypes.HFE
                    Return My.Resources.FloppyImageType_HFEImage
                Case ImageImportOutputTypes.MFM
                    Return My.Resources.FloppyImageType_MFMImage
                Case ImageImportOutputTypes.F86
                    Return My.Resources.FloppyImageType_D86FImage
                Case ImageImportOutputTypes.TC
                    Return My.Resources.FloppyImageType_TranscopyImage
                Case ImageImportOutputTypes.IMA
                    Return My.Resources.FloppyImageType_BasicSectorImage
                Case Else
                    Return ""
            End Select
        End Function

        Public Function ImageImportOutputTypeFileExt(Value As ImageImportOutputTypes) As String
            Select Case Value
                Case ImageImportOutputTypes.HFE
                    Return ".hfe"
                Case ImageImportOutputTypes.MFM
                    Return ".mfm"
                Case ImageImportOutputTypes.F86
                    Return ".86f"
                Case ImageImportOutputTypes.TC
                    Return ".tc"
                Case ImageImportOutputTypes.IMA
                    Return ".ima"
                Case Else
                    Return ".ima"
            End Select
        End Function
    End Module
End Namespace
