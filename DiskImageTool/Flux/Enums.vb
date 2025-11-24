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
                    Return "HxC HFE Image"
                Case ImageImportOutputTypes.MFM
                    Return "HxC MFM Image"
                Case ImageImportOutputTypes.F86
                    Return "86Box 86F Image"
                Case ImageImportOutputTypes.TC
                    Return "TransCopy Image"
                Case ImageImportOutputTypes.IMA
                    Return "Basic Sector Image"
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
