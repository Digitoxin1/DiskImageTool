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

        Enum FluxDevice
            Greaseweazle
            Kryoflux
        End Enum

        Public Enum ImageImportOutputTypes
            IMA
            HFE
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
        Public Function ImageImportOutputTypeDescription(Value As ImageImportOutputTypes) As String
            Select Case Value
                Case ImageImportOutputTypes.HFE
                    Return "HxC HFE Image"
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
                Case ImageImportOutputTypes.IMA
                    Return ".ima"
                Case Else
                    Return ".ima"
            End Select
        End Function
    End Module
End Namespace
