Namespace Greaseweazle
    Module Enums
        Public Enum GreaseweazleFloppyType
            None = 0
            F525_DD_360K = 1
            F525_HD_12M = 2
            F35_DD_720K = 4
            F35_HD_144M = 8
            F35_ED_288M = 16
        End Enum

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

        Public Enum GreaseweazleInterface
            IBM
            Shugart
        End Enum

        Public Enum GreaseweazleOutputType
            IMA
            HFE
        End Enum

        Public Function GetMaxTracks(Value As GreaseweazleImageFormat) As Integer
            Select Case Value
                Case GreaseweazleImageFormat.IBM_160, GreaseweazleImageFormat.IBM_180, GreaseweazleImageFormat.IBM_320, GreaseweazleImageFormat.IBM_360
                    Return 40
                Case Else
                    Return 80
            End Select
        End Function

        Public Function GreaseweazleFindCompatibleFloppyType(Format As DiskImage.FloppyDiskFormat, AvailableTypes As GreaseweazleFloppyType) As GreaseweazleFloppyType
            Dim Result As GreaseweazleFloppyType = GreaseweazleFloppyType.None

            Select Case Format
                Case DiskImage.FloppyDiskFormat.Floppy160, DiskImage.FloppyDiskFormat.Floppy180, DiskImage.FloppyDiskFormat.Floppy320, DiskImage.FloppyDiskFormat.Floppy360
                    If (AvailableTypes And GreaseweazleFloppyType.F525_DD_360K) > 0 Then
                        Result = GreaseweazleFloppyType.F525_DD_360K
                    ElseIf (AvailableTypes And GreaseweazleFloppyType.F525_HD_12M) > 0 Then
                        Result = GreaseweazleFloppyType.F525_HD_12M
                    End If

                Case DiskImage.FloppyDiskFormat.Floppy720
                    If (AvailableTypes And GreaseweazleFloppyType.F35_DD_720K) > 0 Then
                        Result = GreaseweazleFloppyType.F35_DD_720K
                    ElseIf (AvailableTypes And GreaseweazleFloppyType.F35_HD_144M) > 0 Then
                        Result = GreaseweazleFloppyType.F35_HD_144M
                    ElseIf (AvailableTypes And GreaseweazleFloppyType.F35_ED_288M) > 0 Then
                        Result = GreaseweazleFloppyType.F35_ED_288M
                    End If

                Case DiskImage.FloppyDiskFormat.Floppy1200, DiskImage.FloppyDiskFormat.FloppyTandy2000, DiskImage.FloppyDiskFormat.FloppyXDF525
                    If (AvailableTypes And GreaseweazleFloppyType.F525_HD_12M > 0) Then
                        Result = GreaseweazleFloppyType.F525_HD_12M
                    End If

                Case DiskImage.FloppyDiskFormat.Floppy1440, DiskImage.FloppyDiskFormat.Floppy2HD, DiskImage.FloppyDiskFormat.FloppyDMF1024, DiskImage.FloppyDiskFormat.FloppyDMF2048, DiskImage.FloppyDiskFormat.FloppyProCopy
                    If (AvailableTypes And GreaseweazleFloppyType.F35_HD_144M > 0) Then
                        Result = GreaseweazleFloppyType.F35_HD_144M
                    ElseIf (AvailableTypes And GreaseweazleFloppyType.F35_ED_288M > 0) Then
                        Result = GreaseweazleFloppyType.F35_ED_288M
                    End If

                Case DiskImage.FloppyDiskFormat.Floppy2880
                    If (AvailableTypes And GreaseweazleFloppyType.F35_ED_288M > 0) Then
                        Result = GreaseweazleFloppyType.F35_ED_288M
                    End If
            End Select

            Return Result
        End Function

        Public Function GreaseweazleFloppyTypeDescription(Value As GreaseweazleFloppyType) As String
            Select Case Value
                Case GreaseweazleFloppyType.F525_DD_360K
                    Return "5.25"" 360 KB (DD)"
                Case GreaseweazleFloppyType.F35_DD_720K
                    Return "3.5"" 720 KB (DD)"
                Case GreaseweazleFloppyType.F525_HD_12M
                    Return "5.25"" 1.2 MB (HD)"
                Case GreaseweazleFloppyType.F35_HD_144M
                    Return "3.5"" 1.44 MB (HD)"
                Case GreaseweazleFloppyType.F35_ED_288M
                    Return "3.5"" 2.88 MB (ED)"
                Case Else
                    Return "None"
            End Select
        End Function

        Public Function GreaseweazleFloppyTypeFromName(Value As String) As GreaseweazleFloppyType
            Select Case Value
                Case "360"
                    Return GreaseweazleFloppyType.F525_DD_360K
                Case "720"
                    Return GreaseweazleFloppyType.F35_DD_720K
                Case "1200"
                    Return GreaseweazleFloppyType.F525_HD_12M
                Case "1440"
                    Return GreaseweazleFloppyType.F35_HD_144M
                Case "2880"
                    Return GreaseweazleFloppyType.F35_ED_288M
                Case Else
                    Return GreaseweazleFloppyType.None
            End Select
        End Function

        Public Function GreaseweazleFloppyTypeName(Value As GreaseweazleFloppyType) As String
            Select Case Value
                Case GreaseweazleFloppyType.F525_DD_360K
                    Return "360"
                Case GreaseweazleFloppyType.F35_DD_720K
                    Return "720"
                Case GreaseweazleFloppyType.F525_HD_12M
                    Return "1200"
                Case GreaseweazleFloppyType.F35_HD_144M
                    Return "1440"
                Case GreaseweazleFloppyType.F35_ED_288M
                    Return "2880"
                Case Else
                    Return ""
            End Select
        End Function
        Public Function GreaseweazleImageFormatBitrate(Value As GreaseweazleImageFormat) As Integer
            Select Case Value
                Case GreaseweazleImageFormat.IBM_1200, GreaseweazleImageFormat.IBM_1440, GreaseweazleImageFormat.IBM_DMF
                    Return 500
                Case GreaseweazleImageFormat.IBM_2880
                    Return 1000
                Case Else
                    Return 250
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

        Public Function GreaseweazleImageFormatDescription(Value As GreaseweazleImageFormat) As String
            Select Case Value
                Case GreaseweazleImageFormat.None
                    Return My.Resources.Label_PleaseSelect
                Case GreaseweazleImageFormat.IBM_160
                    Return "5.25"" 160 KB (SS, DD)"
                Case GreaseweazleImageFormat.IBM_180
                    Return "5.25"" 180 KB (SS, DD)"
                Case GreaseweazleImageFormat.IBM_320
                    Return "5.25"" 320 KB (DS, DD)"
                Case GreaseweazleImageFormat.IBM_360
                    Return "5.25"" 360 KB (DS, DD)"
                Case GreaseweazleImageFormat.IBM_1200
                    Return "5.25"" 1.2 MB (DS, HD)"
                Case GreaseweazleImageFormat.IBM_720
                    Return "3.5"" 720 KB (DS, DD)"
                Case GreaseweazleImageFormat.IBM_800
                    Return "3.5"" 800 KB (DS, DD)"
                Case GreaseweazleImageFormat.IBM_1440
                    Return "3.5"" 1.44 MB (DS, HD)"
                Case GreaseweazleImageFormat.IBM_2880
                    Return "3.5"" 2.88 MB (DS, ED)"
                Case GreaseweazleImageFormat.IBM_DMF
                    Return "3.5"" 1.68 MB (DS, HD, DMF)"
                Case Else
                    Return ""
            End Select
        End Function
        Public Function GreaseweazleImageFormatFromFloppyDiskFormat(Format As DiskImage.FloppyDiskFormat) As GreaseweazleImageFormat
            Select Case Format
                Case DiskImage.FloppyDiskFormat.Floppy160
                    Return GreaseweazleImageFormat.IBM_160
                Case DiskImage.FloppyDiskFormat.Floppy180
                    Return GreaseweazleImageFormat.IBM_180
                Case DiskImage.FloppyDiskFormat.Floppy320
                    Return GreaseweazleImageFormat.IBM_320
                Case DiskImage.FloppyDiskFormat.Floppy360
                    Return GreaseweazleImageFormat.IBM_360
                Case DiskImage.FloppyDiskFormat.Floppy720
                    Return GreaseweazleImageFormat.IBM_720
                Case DiskImage.FloppyDiskFormat.Floppy1200
                    Return GreaseweazleImageFormat.IBM_1200
                Case DiskImage.FloppyDiskFormat.Floppy1440
                    Return GreaseweazleImageFormat.IBM_1440
                Case DiskImage.FloppyDiskFormat.Floppy2880
                    Return GreaseweazleImageFormat.IBM_2880
                Case DiskImage.FloppyDiskFormat.FloppyDMF1024, DiskImage.FloppyDiskFormat.FloppyDMF2048
                    Return GreaseweazleImageFormat.IBM_DMF
                Case Else
                    Return GreaseweazleImageFormat.None
            End Select
        End Function

        Public Function GreaseweazleImageFormatRPM(Value As GreaseweazleImageFormat) As Integer
            Select Case Value
                Case GreaseweazleImageFormat.IBM_1200
                    Return 360
                Case Else
                    Return 300
            End Select
        End Function

        Public Function GreaseweazleImageFormatSideCount(Value As GreaseweazleImageFormat) As Integer
            Select Case Value
                Case GreaseweazleImageFormat.IBM_160, GreaseweazleImageFormat.IBM_180
                    Return 1
                Case Else
                    Return 2
            End Select
        End Function

        Public Function GreaseweazleImageFormatTrackCount(Value As GreaseweazleImageFormat) As Integer
            Select Case Value
                Case GreaseweazleImageFormat.IBM_160, GreaseweazleImageFormat.IBM_180, GreaseweazleImageFormat.IBM_320, GreaseweazleImageFormat.IBM_360
                    Return 40
                Case Else
                    Return 80
            End Select
        End Function

        Public Function GreaseweazleInterfaceFromName(Value As String) As GreaseweazleInterface
            Select Case Value
                Case "Shugart"
                    Return GreaseweazleInterface.Shugart
                Case Else
                    Return GreaseweazleInterface.IBM
            End Select
        End Function

        Public Function GreaseweazleInterfaceName(Value As GreaseweazleInterface) As String
            Select Case Value
                Case GreaseweazleInterface.Shugart
                    Return "Shugart"
                Case Else
                    Return "IBM"
            End Select
        End Function

        Public Function GreaseweazleOutputTypeDescription(Value As GreaseweazleOutputType) As String
            Select Case Value
                Case GreaseweazleOutputType.HFE
                    Return "HxC HFE Image (.hfe)"
                Case GreaseweazleOutputType.IMA
                    Return "Basic Sector Image (.ima)"
                Case Else
                    Return ""
            End Select
        End Function

        Public Function GreaseweazleOutputTypeFileExt(Value As GreaseweazleOutputType) As String
            Select Case Value
                Case GreaseweazleOutputType.HFE
                    Return ".hfe"
                Case GreaseweazleOutputType.IMA
                    Return ".ima"
                Case Else
                    Return ".ima"
            End Select
        End Function
    End Module
End Namespace
