Namespace DiskImage
    Module FloppyDiskFunctions
        Public Enum FloppyDiskType
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
        End Enum

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

        Public Function BuildBootSector(Params As FloppyDiskParams) As BootSector
            Dim Data(DiskImage.BootSector.BOOT_SECTOR_SIZE - 1) As Byte
            Dim FileBytes As New ImageByteArray(Data)
            Dim BootSector = New BootSector(FileBytes)

            With Params
                BootSector.BytesPerSector = .BytesPerSector
                BootSector.MediaDescriptor = .MediaDescriptor
                BootSector.NumberOfFATs = .NumberOfFATs
                BootSector.NumberOfHeads = .NumberOfHeads
                BootSector.ReservedSectorCount = .ReservedSectorCount
                BootSector.RootEntryCount = .RootEntryCount
                BootSector.SectorCountSmall = .SectorCountSmall
                BootSector.SectorsPerCluster = .SectorsPerCluster
                BootSector.SectorsPerFAT = .SectorsPerFAT
                BootSector.SectorsPerTrack = .SectorsPerTrack
            End With

            Return BootSector
        End Function

        Public Function BuildBootSector(Type As FloppyDiskType) As BootSector
            Return BuildBootSector(GetFloppyDiskParams(Type))
        End Function

        Public Function BuildBootSector(Size As Integer) As BootSector
            Return BuildBootSector(GetFloppyDiskParams(Size))
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
            End Select

            Return Params
        End Function

        Public Function GetFloppyDiskParams(Size As Integer) As FloppyDiskParams
            Return GetFloppyDiskParams(GetFloppyDiskType(Size))
        End Function

        Public Function BootSectorCompare(BootSector As BootSector, Params As FloppyDiskParams) As Boolean
            Return BootSector.BytesPerSector = Params.BytesPerSector _
                AndAlso BootSector.MediaDescriptor = Params.MediaDescriptor _
                AndAlso BootSector.NumberOfFATs = Params.NumberOfFATs _
                AndAlso BootSector.NumberOfHeads = Params.NumberOfHeads _
                AndAlso BootSector.ReservedSectorCount = Params.ReservedSectorCount _
                AndAlso BootSector.RootEntryCount = Params.RootEntryCount _
                AndAlso BootSector.SectorCountSmall = Params.SectorCountSmall _
                AndAlso BootSector.SectorsPerCluster = Params.SectorsPerCluster _
                AndAlso BootSector.SectorsPerFAT = Params.SectorsPerFAT _
                AndAlso BootSector.SectorsPerTrack = Params.SectorsPerTrack
        End Function

        Public Function GetFloppyDiskType(BootSector As BootSector) As FloppyDiskType
            Dim Items = System.Enum.GetValues(GetType(FloppyDiskType))

            For Each Type As FloppyDiskType In Items
                If Type <> FloppyDiskType.FloppyUnknown Then
                    Dim Params = GetFloppyDiskParams(Type)
                    If BootSectorCompare(BootSector, Params) Then
                        Return Type
                    End If
                End If
            Next

            Return FloppyDiskType.FloppyUnknown
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
                Case 2949120
                    Return FloppyDiskType.Floppy2880
                Case Else
                    Return FloppyDiskType.FloppyUnknown
            End Select
        End Function

        Public Function GetFloppyDiskTypeName(Type As FloppyDiskType) As String
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
                    Return "DMF (1024)"
                Case FloppyDiskType.FloppyDMF2048
                    Return "DMF (2048)"
                Case FloppyDiskType.Floppy2880
                    Return "2.88M"
                Case FloppyDiskType.FloppyProCopy
                    Return "ProCopy"
                Case Else
                    Return "Custom"
            End Select
        End Function

        Public Function GetFloppyDiskTypeName(BootSector As BootSector) As String
            Return GetFloppyDiskTypeName(GetFloppyDiskType(BootSector))
        End Function
    End Module
End Namespace
