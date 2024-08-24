Namespace DiskImage
    Public Module FloppyDiskFunctions
        Public Enum FloppyDiskType As Byte
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
            FloppyXDF = 12
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

        Public Function BuildBPB(Type As FloppyDiskType) As BiosParameterBlock
            Return BuildBPB(GetFloppyDiskParams(Type))
        End Function

        Public Function BuildBPB(Size As Integer) As BiosParameterBlock
            Return BuildBPB(GetFloppyDiskParams(Size))
        End Function

        Public Function BuildBPB(MediaDescriptor As Byte) As BiosParameterBlock
            Return BuildBPB(GetFloppyDiskParams(GetFloppyDiskType(MediaDescriptor)))
        End Function

        Public Function GetFloppyDiskMediaDescriptor(Size As Integer) As Byte
            Return GetFloppyDiskMediaDescriptor(GetFloppyDiskType(Size))
        End Function

        Public Function GetFloppyDiskMediaDescriptor(Type As FloppyDiskType) As Byte
            Select Case Type
                Case FloppyDiskType.Floppy160
                    Return &HFE
                Case FloppyDiskType.Floppy180
                    Return &HFC
                Case FloppyDiskType.Floppy320
                    Return &HFF
                Case FloppyDiskType.Floppy360
                    Return &HFD
                Case FloppyDiskType.Floppy720
                    Return &HF9
                Case FloppyDiskType.Floppy1200
                    Return &HF9
                Case FloppyDiskType.Floppy1440
                    Return &HF0
                Case FloppyDiskType.FloppyDMF1024
                    Return &HF0
                Case FloppyDiskType.FloppyDMF2048
                    Return &HF0
                Case FloppyDiskType.Floppy2880
                    Return &HF0
                Case FloppyDiskType.FloppyProCopy
                    Return &HF0
                Case FloppyDiskType.FloppyXDF
                    Return &HF0
                Case Else
                    Return &HF0
            End Select
        End Function

        Public Function GetFloppyDiskParams(Type As FloppyDiskType) As FloppyDiskParams
            Dim Params As FloppyDiskParams

            Select Case Type
                Case FloppyDiskType.Floppy160
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

                Case FloppyDiskType.Floppy180
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

                Case FloppyDiskType.Floppy320
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

                Case FloppyDiskType.Floppy360
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

                Case FloppyDiskType.Floppy720
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

                Case FloppyDiskType.Floppy1200
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

                Case FloppyDiskType.Floppy1440
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

                Case FloppyDiskType.FloppyDMF1024
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

                Case FloppyDiskType.FloppyDMF2048
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

                Case FloppyDiskType.Floppy2880
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

                Case FloppyDiskType.FloppyProCopy
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

                Case FloppyDiskType.FloppyXDF
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
            End Select

            Return Params
        End Function

        Public Function GetFloppyDiskParams(Size As Integer) As FloppyDiskParams
            Return GetFloppyDiskParams(GetFloppyDiskType(Size))
        End Function

        Public Function GetFloppyDiskType(MediaDescriptor As Byte) As FloppyDiskType
            Select Case MediaDescriptor
                Case &HFE
                    Return FloppyDiskType.Floppy160
                Case &HFC
                    Return FloppyDiskType.Floppy180
                Case &HFF
                    Return FloppyDiskType.Floppy320
                Case &HFD
                    Return FloppyDiskType.Floppy360
                Case Else
                    Return FloppyDiskType.FloppyUnknown
            End Select
        End Function

        Public Function GetFloppyDiskType(BPB As BiosParameterBlock, CheckMediaDescriptor As Boolean) As FloppyDiskType
            Dim Items = System.Enum.GetValues(GetType(FloppyDiskType))

            For Each Type As FloppyDiskType In Items
                If Type <> FloppyDiskType.FloppyUnknown Then
                    Dim Params = GetFloppyDiskParams(Type)
                    If BPBCompare(BPB, Params, CheckMediaDescriptor) Then
                        Return Type
                    End If
                End If
            Next

            Return FloppyDiskType.FloppyUnknown
        End Function

        Public Function GetFloppyDiskSize(Type As FloppyDiskType) As Integer
            Select Case Type
                Case FloppyDiskType.Floppy160
                    Return 163840
                Case FloppyDiskType.Floppy180
                    Return 184320
                Case FloppyDiskType.Floppy320
                    Return 327680
                Case FloppyDiskType.Floppy360
                    Return 368640
                Case FloppyDiskType.Floppy720
                    Return 737280
                Case FloppyDiskType.Floppy1200
                    Return 1228800
                Case FloppyDiskType.Floppy1440
                    Return 1474560
                Case FloppyDiskType.FloppyDMF2048
                    Return 1720320
                Case FloppyDiskType.Floppy2880
                    Return 1884160
                Case FloppyDiskType.FloppyXDF
                    Return 2949120
                Case Else
                    Return 0
            End Select
        End Function

        Public Function GetFloppyDiskType(Size As Integer) As FloppyDiskType
            Select Case Size
                Case 163840
                    Return FloppyDiskType.Floppy160
                Case 184320
                    Return FloppyDiskType.Floppy180
                Case 327680
                    Return FloppyDiskType.Floppy320
                Case 368640
                    Return FloppyDiskType.Floppy360
                Case 737280
                    Return FloppyDiskType.Floppy720
                Case 1228800
                    Return FloppyDiskType.Floppy1200
                Case 1474560
                    Return FloppyDiskType.Floppy1440
                Case 1720320
                    Return FloppyDiskType.FloppyDMF2048
                Case 1884160
                    Return FloppyDiskType.FloppyXDF
                Case 2949120
                    Return FloppyDiskType.Floppy2880
                Case Else
                    Return FloppyDiskType.FloppyUnknown
            End Select
        End Function

        Public Function GetFloppyDiskType(Name As String) As FloppyDiskType
            Select Case Name
                Case "160K"
                    Return FloppyDiskType.Floppy160
                Case "180K"
                    Return FloppyDiskType.Floppy180
                Case "320K"
                    Return FloppyDiskType.Floppy320
                Case "360K"
                    Return FloppyDiskType.Floppy360
                Case "720K"
                    Return FloppyDiskType.Floppy720
                Case "1.2M"
                    Return FloppyDiskType.Floppy1200
                Case "1.44M"
                    Return FloppyDiskType.Floppy1440
                Case "DMF (1024)"
                    Return FloppyDiskType.FloppyDMF1024
                Case "DMF (2048)"
                    Return FloppyDiskType.FloppyDMF2048
                Case "2.88M"
                    Return FloppyDiskType.Floppy2880
                Case "ProCopy"
                    Return FloppyDiskType.FloppyProCopy
                Case "XDF"
                    Return FloppyDiskType.FloppyXDF
                Case Else
                    Return FloppyDiskType.FloppyUnknown
            End Select
        End Function

        Public Function GetFloppyDiskTypeName(Type As FloppyDiskType, Optional Extended As Boolean = False) As String
            Select Case Type
                Case FloppyDiskType.Floppy160
                    Return "160K"
                Case FloppyDiskType.Floppy180
                    Return "180K"
                Case FloppyDiskType.Floppy320
                    Return "320K"
                Case FloppyDiskType.Floppy360
                    Return "360K"
                Case FloppyDiskType.Floppy720
                    Return "720K"
                Case FloppyDiskType.Floppy1200
                    Return "1.2M"
                Case FloppyDiskType.Floppy1440
                    Return "1.44M"
                Case FloppyDiskType.FloppyDMF1024
                    If Extended Then
                        Return "DMF (1024)"
                    Else
                        Return "DMF"
                    End If
                Case FloppyDiskType.FloppyDMF2048
                    If Extended Then
                        Return "DMF (2048)"
                    Else
                        Return "DMF"
                    End If
                Case FloppyDiskType.Floppy2880
                    Return "2.88M"
                Case FloppyDiskType.FloppyProCopy
                    Return "ProCopy"
                Case FloppyDiskType.FloppyXDF
                    Return "XDF"
                Case Else
                    Return "Custom"
            End Select
        End Function

        Public Function GetFileFilterExtByType(Type As FloppyDiskType) As String
            Select Case Type
                Case FloppyDiskType.Floppy160
                    Return ".160"
                Case FloppyDiskType.Floppy180
                    Return ".180"
                Case FloppyDiskType.Floppy320
                    Return ".320"
                Case FloppyDiskType.Floppy360
                    Return ".360"
                Case FloppyDiskType.Floppy720
                    Return ".720"
                Case FloppyDiskType.Floppy1200
                    Return ".120"
                Case FloppyDiskType.Floppy1440
                    Return ".144"
                Case FloppyDiskType.FloppyDMF1024
                    Return ".dmf"
                Case FloppyDiskType.FloppyDMF2048
                    Return ".dmf"
                Case FloppyDiskType.Floppy2880
                    Return ".288"
                Case FloppyDiskType.FloppyProCopy
                    Return ""
                Case FloppyDiskType.FloppyXDF
                    Return ".xdf"
                Case Else
                    Return ""
            End Select
        End Function

        Public Function GetFileFilterDescriptionByType(Type As FloppyDiskType) As String
            Dim Description As String = GetFloppyDiskTypeName(Type)
            Select Case Type
                Case FloppyDiskType.FloppyProCopy
                    Description = ""
                Case FloppyDiskType.FloppyUnknown
                    Description = ""
            End Select

            If Description <> "" Then
                Description &= " Floppy Image"
            End If

            Return Description
        End Function

        Public Function GetFloppyDiskTypeName(BPB As BiosParameterBlock, CheckMediaDescriptor As Boolean, Optional Extended As Boolean = False) As String
            Return GetFloppyDiskTypeName(GetFloppyDiskType(BPB, CheckMediaDescriptor), Extended)
        End Function

        Public Function GetFloppyDiskTypeName(Size As Integer, Optional Extended As Boolean = False) As String
            Return GetFloppyDiskTypeName(GetFloppyDiskType(Size), Extended)
        End Function

        Public Function GetFloppyDiskTypeName(MediaDescriptor As Byte, Optional Extended As Boolean = False) As String
            Return GetFloppyDiskTypeName(GetFloppyDiskType(MediaDescriptor), Extended)
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
