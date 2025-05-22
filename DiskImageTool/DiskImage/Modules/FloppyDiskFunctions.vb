
Namespace DiskImage
    Public Module FloppyDiskFunctions
        Public Enum FloppyDiskFormat As Byte
            FloppyUnknown = 0
            Floppy160 = 1
            Floppy180 = 2
            Floppy320 = 3
            Floppy360 = 4
            Floppy720 = 5
            Floppy1200 = 6
            Floppy1440 = 7
            Floppy2880 = 8
            FloppyDMF1024 = 9
            FloppyDMF2048 = 10
            FloppyProCopy = 11
            FloppyXDF35 = 12
            FloppyXDF525 = 13
            FloppyXDFMicro = 14
            FloppyTandy2000 = 15
            FloppyNoBPB = 16
            FloppyJapanese = 17
        End Enum

        Public Function BPBCompare(BPB As BiosParameterBlock, Params As FloppyDiskParams, CheckMediaDescriptor As Boolean) As Boolean
            Return BPB.BytesPerSector = Params.BytesPerSector _
                AndAlso BPB.NumberOfFATs = Params.NumberOfFATs _
                AndAlso BPB.NumberOfHeads = Params.NumberOfHeads _
                AndAlso BPB.ReservedSectorCount = Params.ReservedSectorCount _
                AndAlso BPB.RootEntryCount = Params.RootEntryCount _
                AndAlso BPB.SectorCountSmall = Params.SectorCountSmall _
                AndAlso BPB.SectorsPerCluster = Params.SectorsPerCluster _
                AndAlso BPB.SectorsPerFAT = Params.SectorsPerFAT _
                AndAlso BPB.SectorsPerTrack = Params.SectorsPerTrack _
                AndAlso (Not CheckMediaDescriptor Or BPB.MediaDescriptor = Params.MediaDescriptor)
        End Function

        Public Function BuildBPB(Params As FloppyDiskParams) As BiosParameterBlock
            Dim BPB = New BiosParameterBlock()

            With Params
                BPB.BytesPerSector = .BytesPerSector
                BPB.MediaDescriptor = .MediaDescriptor
                BPB.NumberOfFATs = .NumberOfFATs
                BPB.NumberOfHeads = .NumberOfHeads
                BPB.ReservedSectorCount = .ReservedSectorCount
                BPB.RootEntryCount = .RootEntryCount
                BPB.SectorCountSmall = .SectorCountSmall
                BPB.SectorsPerCluster = .SectorsPerCluster
                BPB.SectorsPerFAT = .SectorsPerFAT
                BPB.SectorsPerTrack = .SectorsPerTrack
            End With

            Return BPB
        End Function

        Public Function BuildBPB(DiskFormat As FloppyDiskFormat) As BiosParameterBlock
            Return BuildBPB(GetFloppyDiskParams(DiskFormat))
        End Function

        Public Function BuildBPB(Size As Integer) As BiosParameterBlock
            Return BuildBPB(GetFloppyDiskParams(Size))
        End Function

        Public Function BuildBPB(MediaDescriptor As Byte) As BiosParameterBlock
            Return BuildBPB(GetFloppyDiskParams(GetFloppyDiskFomat(MediaDescriptor)))
        End Function

        Public Function GetFloppyDiskMediaDescriptor(Size As Integer) As Byte
            Return GetFloppyDiskMediaDescriptor(GetFloppyDiskFormat(Size))
        End Function

        Public Function GetFloppyDiskMediaDescriptor(DiskFormat As FloppyDiskFormat) As Byte
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy160
                    Return &HFE
                Case FloppyDiskFormat.Floppy180
                    Return &HFC
                Case FloppyDiskFormat.Floppy320
                    Return &HFF
                Case FloppyDiskFormat.Floppy360
                    Return &HFD
                Case FloppyDiskFormat.Floppy720
                    Return &HF9
                Case FloppyDiskFormat.Floppy1200
                    Return &HF9
                Case FloppyDiskFormat.Floppy1440
                    Return &HF0
                Case FloppyDiskFormat.FloppyDMF1024
                    Return &HF0
                Case FloppyDiskFormat.FloppyDMF2048
                    Return &HF0
                Case FloppyDiskFormat.Floppy2880
                    Return &HF0
                Case FloppyDiskFormat.FloppyProCopy
                    Return &HF0
                Case FloppyDiskFormat.FloppyXDF35
                    Return &HF0
                Case FloppyDiskFormat.FloppyXDF525
                    Return &HF9
                Case FloppyDiskFormat.FloppyXDFMicro
                    Return &HF9
                Case FloppyDiskFormat.FloppyTandy2000
                    Return &HED
                Case FloppyDiskFormat.FloppyJapanese
                    Return &HFE
                Case Else
                    Return &HF0
            End Select
        End Function

        Public Function GetFloppyDiskParams(DiskFormat As FloppyDiskFormat) As FloppyDiskParams
            Dim Params As FloppyDiskParams

            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy160
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HFE
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 1
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 64
                    Params.SectorCountSmall = 320
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 1
                    Params.SectorsPerTrack = 8

                Case FloppyDiskFormat.Floppy180
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HFC
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 1
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 64
                    Params.SectorCountSmall = 360
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 2
                    Params.SectorsPerTrack = 9

                Case FloppyDiskFormat.Floppy320
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HFF
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 112
                    Params.SectorCountSmall = 640
                    Params.SectorsPerCluster = 2
                    Params.SectorsPerFAT = 1
                    Params.SectorsPerTrack = 8

                Case FloppyDiskFormat.Floppy360
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HFD
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 112
                    Params.SectorCountSmall = 720
                    Params.SectorsPerCluster = 2
                    Params.SectorsPerFAT = 2
                    Params.SectorsPerTrack = 9

                Case FloppyDiskFormat.Floppy720
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF9
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 112
                    Params.SectorCountSmall = 1440
                    Params.SectorsPerCluster = 2
                    Params.SectorsPerFAT = 3
                    Params.SectorsPerTrack = 9

                Case FloppyDiskFormat.Floppy1200
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF9
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 224
                    Params.SectorCountSmall = 2400
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 7
                    Params.SectorsPerTrack = 15

                Case FloppyDiskFormat.Floppy1440
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF0
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 224
                    Params.SectorCountSmall = 2880
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 9
                    Params.SectorsPerTrack = 18

                Case FloppyDiskFormat.FloppyDMF1024
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF0
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 16
                    Params.SectorCountSmall = 3360
                    Params.SectorsPerCluster = 2
                    Params.SectorsPerFAT = 5
                    Params.SectorsPerTrack = 21

                Case FloppyDiskFormat.FloppyDMF2048
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF0
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 16
                    Params.SectorCountSmall = 3360
                    Params.SectorsPerCluster = 4
                    Params.SectorsPerFAT = 3
                    Params.SectorsPerTrack = 21

                Case FloppyDiskFormat.Floppy2880
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF0
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 240
                    Params.SectorCountSmall = 5760
                    Params.SectorsPerCluster = 2
                    Params.SectorsPerFAT = 9
                    Params.SectorsPerTrack = 36

                Case FloppyDiskFormat.FloppyProCopy
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF0
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 16
                    Params.SectorCountSmall = 2880
                    Params.SectorsPerCluster = 2
                    Params.SectorsPerFAT = 5
                    Params.SectorsPerTrack = 18

                Case FloppyDiskFormat.FloppyXDF35
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF0
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 224
                    Params.SectorCountSmall = 3680
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 11
                    Params.SectorsPerTrack = 23

                Case FloppyDiskFormat.FloppyXDF525
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF9
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 224
                    Params.SectorCountSmall = 3040
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 9
                    Params.SectorsPerTrack = 19

                Case FloppyDiskFormat.FloppyXDFMicro
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF9
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 1
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 16
                    Params.SectorCountSmall = 8
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 1
                    Params.SectorsPerTrack = 8

                Case FloppyDiskFormat.FloppyTandy2000
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HED
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 112
                    Params.SectorCountSmall = 1440
                    Params.SectorsPerCluster = 4
                    Params.SectorsPerFAT = 2
                    Params.SectorsPerTrack = 9

                Case FloppyDiskFormat.FloppyJapanese
                    Params.BytesPerSector = 1024
                    Params.MediaDescriptor = &HFE
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 192
                    Params.SectorCountSmall = 1232
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 2
                    Params.SectorsPerTrack = 8

                Case FloppyDiskFormat.FloppyNoBPB
                    Params.BytesPerSector = 0
                    Params.MediaDescriptor = 0
                    Params.NumberOfFATs = 0
                    Params.NumberOfHeads = 0
                    Params.ReservedSectorCount = 0
                    Params.RootEntryCount = 0
                    Params.SectorCountSmall = 0
                    Params.SectorsPerCluster = 0
                    Params.SectorsPerFAT = 0
                    Params.SectorsPerTrack = 0
            End Select

            Return Params
        End Function

        Public Function GetFloppyDiskParams(Size As Integer) As FloppyDiskParams
            Return GetFloppyDiskParams(GetFloppyDiskFormat(Size))
        End Function

        Public Function GetFloppyDiskFomat(MediaDescriptor As Byte) As FloppyDiskFormat
            Select Case MediaDescriptor
                Case &HFE
                    Return FloppyDiskFormat.Floppy160
                Case &HFC
                    Return FloppyDiskFormat.Floppy180
                Case &HFF
                    Return FloppyDiskFormat.Floppy320
                Case &HFD
                    Return FloppyDiskFormat.Floppy360
                Case &HED
                    Return FloppyDiskFormat.FloppyTandy2000
                Case Else
                    Return FloppyDiskFormat.FloppyUnknown
            End Select
        End Function

        Public Function GetFloppyDiskFormat(BPB As BiosParameterBlock, CheckMediaDescriptor As Boolean) As FloppyDiskFormat
            Return GetFloppyDiskFormat(BPB, CheckMediaDescriptor, True)
        End Function

        Public Function GetFloppyDiskFormat(BPB As BiosParameterBlock, CheckMediaDescriptor As Boolean, IgnoreNoBPB As Boolean) As FloppyDiskFormat
            Dim Items = System.Enum.GetValues(GetType(FloppyDiskFormat))

            For Each DiskFormat As FloppyDiskFormat In Items
                If DiskFormat <> FloppyDiskFormat.FloppyUnknown Or (IgnoreNoBPB And DiskFormat <> FloppyDiskFormat.FloppyNoBPB) Then
                    Dim Params = GetFloppyDiskParams(DiskFormat)
                    If BPBCompare(BPB, Params, CheckMediaDescriptor) Then
                        Return DiskFormat
                    End If
                End If
            Next

            Return FloppyDiskFormat.FloppyUnknown
        End Function

        Public Function GetFloppyDiskSize(DiskFormat As FloppyDiskFormat) As Integer
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy160
                    Return 163840
                Case FloppyDiskFormat.Floppy180
                    Return 184320
                Case FloppyDiskFormat.Floppy320
                    Return 327680
                Case FloppyDiskFormat.Floppy360
                    Return 368640
                Case FloppyDiskFormat.Floppy720
                    Return 737280
                Case FloppyDiskFormat.Floppy1200
                    Return 1228800
                Case FloppyDiskFormat.Floppy1440
                    Return 1474560
                Case FloppyDiskFormat.FloppyDMF1024
                    Return 1720320
                Case FloppyDiskFormat.FloppyDMF2048
                    Return 1720320
                Case FloppyDiskFormat.FloppyProCopy
                    Return 1474560
                Case FloppyDiskFormat.FloppyXDF525
                    Return 1556480
                Case FloppyDiskFormat.FloppyXDF35
                    Return 1884160
                Case FloppyDiskFormat.FloppyXDFMicro
                    Return 4096
                Case FloppyDiskFormat.Floppy2880
                    Return 2949120
                Case FloppyDiskFormat.FloppyTandy2000
                    Return 737280
                Case FloppyDiskFormat.FloppyJapanese
                    Return 1261568
                Case Else
                    Return 0
            End Select
        End Function

        Public Function GetFloppyDiskFormat(Size As Integer) As FloppyDiskFormat
            Select Case Size
                Case 163840
                    Return FloppyDiskFormat.Floppy160
                Case 184320
                    Return FloppyDiskFormat.Floppy180
                Case 327680
                    Return FloppyDiskFormat.Floppy320
                Case 368640
                    Return FloppyDiskFormat.Floppy360
                Case 737280
                    Return FloppyDiskFormat.Floppy720
                Case 1228800
                    Return FloppyDiskFormat.Floppy1200
                Case 1474560
                    Return FloppyDiskFormat.Floppy1440
                Case 1720320
                    Return FloppyDiskFormat.FloppyDMF2048
                Case 1556480
                    Return FloppyDiskFormat.FloppyXDF525
                Case 1884160
                    Return FloppyDiskFormat.FloppyXDF35
                Case 4096
                    Return FloppyDiskFormat.FloppyXDFMicro
                Case 2949120
                    Return FloppyDiskFormat.Floppy2880
                Case 1261568
                    Return FloppyDiskFormat.FloppyJapanese
                Case Else
                    Return FloppyDiskFormat.FloppyUnknown
            End Select
        End Function

        Public Function GetFloppyDiskFormat(Name As String) As FloppyDiskFormat
            Select Case Name
                Case "160K"
                    Return FloppyDiskFormat.Floppy160
                Case "180K"
                    Return FloppyDiskFormat.Floppy180
                Case "320K"
                    Return FloppyDiskFormat.Floppy320
                Case "360K"
                    Return FloppyDiskFormat.Floppy360
                Case "720K"
                    Return FloppyDiskFormat.Floppy720
                Case "1.2M"
                    Return FloppyDiskFormat.Floppy1200
                Case "1.44M"
                    Return FloppyDiskFormat.Floppy1440
                Case "DMF (1024)"
                    Return FloppyDiskFormat.FloppyDMF1024
                Case "DMF (2048)"
                    Return FloppyDiskFormat.FloppyDMF2048
                Case "2.88M"
                    Return FloppyDiskFormat.Floppy2880
                Case "ProCopy"
                    Return FloppyDiskFormat.FloppyProCopy
                Case "XDF 5.25"""
                    Return FloppyDiskFormat.FloppyXDF525
                Case "XDF 3.5"""
                    Return FloppyDiskFormat.FloppyXDF35
                Case "XDF Micro"
                    Return FloppyDiskFormat.FloppyXDFMicro
                Case "Tandy 2000"
                    Return FloppyDiskFormat.FloppyTandy2000
                Case "2HD (1.23M)"
                    Return FloppyDiskFormat.FloppyJapanese
                Case "NO BPB"
                    Return FloppyDiskFormat.FloppyNoBPB
                Case Else
                    Return FloppyDiskFormat.FloppyUnknown
            End Select
        End Function

        Public Function GetFloppyDiskFormatName(DiskFormat As FloppyDiskFormat, Optional Extended As Boolean = False) As String
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy160
                    Return "160K"
                Case FloppyDiskFormat.Floppy180
                    Return "180K"
                Case FloppyDiskFormat.Floppy320
                    Return "320K"
                Case FloppyDiskFormat.Floppy360
                    Return "360K"
                Case FloppyDiskFormat.Floppy720
                    Return "720K"
                Case FloppyDiskFormat.Floppy1200
                    Return "1.2M"
                Case FloppyDiskFormat.Floppy1440
                    Return "1.44M"
                Case FloppyDiskFormat.FloppyDMF1024
                    If Extended Then
                        Return "DMF (1024)"
                    Else
                        Return "DMF"
                    End If
                Case FloppyDiskFormat.FloppyDMF2048
                    If Extended Then
                        Return "DMF (2048)"
                    Else
                        Return "DMF"
                    End If
                Case FloppyDiskFormat.Floppy2880
                    Return "2.88M"
                Case FloppyDiskFormat.FloppyProCopy
                    Return "ProCopy"
                Case FloppyDiskFormat.FloppyXDF525
                    Return "XDF 5.25"""
                Case FloppyDiskFormat.FloppyXDF35
                    Return "XDF 3.5"""
                Case FloppyDiskFormat.FloppyXDFMicro
                    Return "XDF Micro"
                Case FloppyDiskFormat.FloppyTandy2000
                    Return "Tandy 2000"
                Case FloppyDiskFormat.FloppyJapanese
                    Return "2HD (1.23M)"
                Case FloppyDiskFormat.FloppyNoBPB
                    Return "No BPB"
                Case Else
                    Return "Custom"
            End Select
        End Function

        Public Function GetImageFileExtensionByFormat(DiskFormat As FloppyDiskFormat) As String
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy160
                    Return ".160"
                Case FloppyDiskFormat.Floppy180
                    Return ".180"
                Case FloppyDiskFormat.Floppy320
                    Return ".320"
                Case FloppyDiskFormat.Floppy360
                    Return ".360"
                Case FloppyDiskFormat.Floppy720
                    Return ".720"
                Case FloppyDiskFormat.Floppy1200
                    Return ".120"
                Case FloppyDiskFormat.Floppy1440
                    Return ".144"
                Case FloppyDiskFormat.FloppyDMF1024
                    Return ".dmf"
                Case FloppyDiskFormat.FloppyDMF2048
                    Return ".dmf"
                Case FloppyDiskFormat.Floppy2880
                    Return ".288"
                Case FloppyDiskFormat.FloppyProCopy
                    Return ""
                Case FloppyDiskFormat.FloppyXDF525
                    Return ".xdf"
                Case FloppyDiskFormat.FloppyXDF35
                    Return ".xdf"
                Case FloppyDiskFormat.FloppyXDFMicro
                    Return ".xdf"
                Case FloppyDiskFormat.FloppyJapanese
                    Return ".hdm"
                Case Else
                    Return ""
            End Select
        End Function

        Public Function GetFileFilterDescriptionByFormat(DiskFormat As FloppyDiskFormat) As String
            Dim Description As String = GetFloppyDiskFormatName(DiskFormat)
            Select Case DiskFormat
                Case FloppyDiskFormat.FloppyProCopy
                    Description = ""
                Case FloppyDiskFormat.FloppyNoBPB
                    Description = ""
                Case FloppyDiskFormat.FloppyUnknown
                    Description = ""
            End Select

            If Description <> "" Then
                Description &= " Floppy Image"
            End If

            Return Description
        End Function

        Public Function GetFloppyDiskFormatName(BPB As BiosParameterBlock, CheckMediaDescriptor As Boolean, Optional Extended As Boolean = False) As String
            Return GetFloppyDiskFormatName(GetFloppyDiskFormat(BPB, CheckMediaDescriptor), Extended)
        End Function

        Public Function GetFloppyDiskFormatName(Size As Integer, Optional Extended As Boolean = False) As String
            Return GetFloppyDiskFormatName(GetFloppyDiskFormat(Size), Extended)
        End Function

        Public Function GetFloppyDiskFormatName(MediaDescriptor As Byte, Optional Extended As Boolean = False) As String
            Return GetFloppyDiskFormatName(GetFloppyDiskFomat(MediaDescriptor), Extended)
        End Function

        Public Function IsDiskFormatXDF(DiskFormat As FloppyDiskFormat) As Boolean
            Return (DiskFormat = FloppyDiskFormat.FloppyXDF525 Or DiskFormat = FloppyDiskFormat.FloppyXDF35)
        End Function

        Public Function IsDiskFormatValidForRead(DiskFormat As FloppyDiskFormat) As Boolean
            If DiskFormat = FloppyDiskFormat.FloppyDMF1024 _
                Or DiskFormat = FloppyDiskFormat.FloppyDMF2048 _
                Or DiskFormat = FloppyDiskFormat.FloppyNoBPB _
                Or DiskFormat = FloppyDiskFormat.FloppyProCopy _
                Or DiskFormat = FloppyDiskFormat.FloppyXDF35 _
                Or DiskFormat = FloppyDiskFormat.FloppyXDF525 _
                Or DiskFormat = FloppyDiskFormat.FloppyJapanese _
                Or DiskFormat = FloppyDiskFormat.FloppyXDFMicro Then
                Return False
            End If

            Return True
        End Function

        Public Structure FloppyDiskParams
            Dim BytesPerSector As UShort
            Dim MediaDescriptor As Byte
            Dim NumberOfFATs As Byte
            Dim NumberOfHeads As UShort
            Dim ReservedSectorCount As UShort
            Dim RootEntryCount As UShort
            Dim SectorCountSmall As UShort
            Dim SectorsPerCluster As Byte
            Dim SectorsPerFAT As UShort
            Dim SectorsPerTrack As UShort
        End Structure
    End Module
End Namespace
