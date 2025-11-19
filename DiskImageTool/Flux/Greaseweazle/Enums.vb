Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Module Enums
        Public Enum GreaseweazleImageFormat
            None
            IBM_160
            IBM_180
            IBM_320
            IBM_360
            IBM_1200
            IBM_720
            IBM_800
            IBM_1440
            IBM_DMF
            IBM_2880
        End Enum

        Public Enum ImageImportOutputTypes
            IMA
            HFE
        End Enum

        Public Enum ReadDiskOutputTypes
            IMA
            HFE
            RAW
        End Enum

        Public Function GreaseweazleFindCompatibleFloppyType(DiskParams As FloppyDiskParams, AvailableTypes As FloppyMediaType) As FloppyMediaType
            Dim Result As FloppyMediaType = FloppyMediaType.MediaUnknown

            Dim MediaType = DiskParams.MediaType

            If DiskParams.Format = FloppyDiskFormat.FloppyTandy2000 Then
                MediaType = FloppyMediaType.Media525HighDensity
            End If

            Select Case MediaType
                Case FloppyMediaType.Media525DoubleDensity
                    If (AvailableTypes And FloppyMediaType.Media525DoubleDensity) > 0 Then
                        Result = FloppyMediaType.Media525DoubleDensity
                    ElseIf (AvailableTypes And FloppyMediaType.Media525HighDensity) > 0 Then
                        Result = FloppyMediaType.Media525HighDensity
                    End If

                Case FloppyMediaType.Media35DoubleDensity
                    If (AvailableTypes And FloppyMediaType.Media35DoubleDensity) > 0 Then
                        Result = FloppyMediaType.Media35DoubleDensity
                    ElseIf (AvailableTypes And FloppyMediaType.Media35HighDensity) > 0 Then
                        Result = FloppyMediaType.Media35HighDensity
                    ElseIf (AvailableTypes And FloppyMediaType.Media35ExtraHighDensity) > 0 Then
                        Result = FloppyMediaType.Media35ExtraHighDensity
                    End If

                Case FloppyMediaType.Media525HighDensity
                    If (AvailableTypes And FloppyMediaType.Media525HighDensity > 0) Then
                        Result = FloppyMediaType.Media525HighDensity
                    End If

                Case FloppyMediaType.Media35HighDensity
                    If (AvailableTypes And FloppyMediaType.Media35HighDensity > 0) Then
                        Result = FloppyMediaType.Media35HighDensity
                    ElseIf (AvailableTypes And FloppyMediaType.Media35ExtraHighDensity > 0) Then
                        Result = FloppyMediaType.Media35ExtraHighDensity
                    End If

                Case FloppyMediaType.Media35ExtraHighDensity
                    If (AvailableTypes And FloppyMediaType.Media35ExtraHighDensity > 0) Then
                        Result = FloppyMediaType.Media35ExtraHighDensity
                    End If
            End Select

            Return Result
        End Function

        Public Function GreaseweazleFloppyTypeDescription(Value As FloppyMediaType) As String
            Select Case Value
                Case FloppyMediaType.Media525DoubleDensity
                    Return "5.25"" 360 KB (DD)"
                Case FloppyMediaType.Media35DoubleDensity
                    Return "3.5"" 720 KB (DD)"
                Case FloppyMediaType.Media525HighDensity
                    Return "5.25"" 1.2 MB (HD)"
                Case FloppyMediaType.Media35HighDensity
                    Return "3.5"" 1.44 MB (HD)"
                Case FloppyMediaType.Media35ExtraHighDensity
                    Return "3.5"" 2.88 MB (ED)"
                Case Else
                    Return "None"
            End Select
        End Function

        Public Function GreaseweazleImageFormatCommandLine(Value As GreaseweazleImageFormat) As String
            Select Case Value
                Case GreaseweazleImageFormat.None
                    Return "ibm.scan"
                Case GreaseweazleImageFormat.IBM_160
                    Return "ibm.160"
                Case GreaseweazleImageFormat.IBM_180
                    Return "ibm.180"
                Case GreaseweazleImageFormat.IBM_320
                    Return "ibm.320"
                Case GreaseweazleImageFormat.IBM_360
                    Return "ibm.360"
                Case GreaseweazleImageFormat.IBM_1200
                    Return "ibm.1200"
                Case GreaseweazleImageFormat.IBM_720
                    Return "ibm.720"
                Case GreaseweazleImageFormat.IBM_800
                    Return "ibm.800"
                Case GreaseweazleImageFormat.IBM_1440
                    Return "ibm.1440"
                Case GreaseweazleImageFormat.IBM_2880
                    Return "ibm.2880"
                Case GreaseweazleImageFormat.IBM_DMF
                    Return "ibm.dmf"
                Case Else
                    Return "ibm.scan"
            End Select
        End Function

        Public Function GreaseweazleImageFormatFromFloppyDiskFormat(Format As FloppyDiskFormat) As GreaseweazleImageFormat
            Select Case Format
                Case FloppyDiskFormat.Floppy160
                    Return GreaseweazleImageFormat.IBM_160
                Case FloppyDiskFormat.Floppy180
                    Return GreaseweazleImageFormat.IBM_180
                Case FloppyDiskFormat.Floppy320
                    Return GreaseweazleImageFormat.IBM_320
                Case FloppyDiskFormat.Floppy360
                    Return GreaseweazleImageFormat.IBM_360
                Case FloppyDiskFormat.Floppy720
                    Return GreaseweazleImageFormat.IBM_720
                Case FloppyDiskFormat.Floppy1200
                    Return GreaseweazleImageFormat.IBM_1200
                Case FloppyDiskFormat.Floppy1440, FloppyDiskFormat.FloppyProCopy
                    Return GreaseweazleImageFormat.IBM_1440
                Case FloppyDiskFormat.Floppy2880
                    Return GreaseweazleImageFormat.IBM_2880
                Case FloppyDiskFormat.FloppyDMF1024, FloppyDiskFormat.FloppyDMF2048
                    Return GreaseweazleImageFormat.IBM_DMF
                Case Else
                    Return GreaseweazleImageFormat.None
            End Select
        End Function

        Public Function GreaseweazleInterfaceName(Value As GreaseweazleSettings.GreaseweazleInterface) As String
            Select Case Value
                Case GreaseweazleSettings.GreaseweazleInterface.Shugart
                    Return "Shugart"
                Case Else
                    Return "IBM"
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

        Public Function ReadDisktOutputTypeFileExt(Value As ReadDiskOutputTypes) As String
            Select Case Value
                Case ImageImportOutputTypes.HFE
                    Return ".hfe"
                Case ImageImportOutputTypes.IMA
                    Return ".ima"
                Case ReadDiskOutputTypes.RAW
                    Return ".raw"
                Case Else
                    Return ".ima"
            End Select
        End Function

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

        Public Function ReadDiskOutputTypeDescription(Value As ReadDiskOutputTypes) As String
            Select Case Value
                Case ReadDiskOutputTypes.HFE
                    Return "HxC HFE Image"
                Case ReadDiskOutputTypes.IMA
                    Return "Basic Sector Image"
                Case ReadDiskOutputTypes.RAW
                    Return "Raw Flux Set"
                Case Else
                    Return ""
            End Select
        End Function
    End Module
End Namespace
