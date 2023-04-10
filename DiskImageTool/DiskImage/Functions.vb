Imports System.Text

Namespace DiskImage
    Module Functions
        Public Const BYTES_PER_SECTOR As UShort = 512
        Public ReadOnly InvalidFileChars() As Byte = {&H22, &H2A, &H2B, &H2C, &H2E, &H2F, &H3A, &H3B, &H3C, &H3D, &H3E, &H3F, &H5B, &H5C, &H5D, &H7C}
        Private ReadOnly CodePage437LookupTable As UShort() = New UShort(255) {
            65533, 9786, 9787, 9829, 9830, 9827, 9824, 8226, 9688, 9675, 9689, 9794, 9792, 9834, 9835, 9788,
            9658, 9668, 8597, 8252, 182, 167, 9644, 8616, 8593, 8595, 8594, 8592, 8735, 8596, 9650, 9660,
            32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
            48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63,
            64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79,
            80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95,
            96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
            112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 8962,
            199, 252, 233, 226, 228, 224, 229, 231, 234, 235, 232, 239, 238, 236, 196, 197,
            201, 230, 198, 244, 246, 242, 251, 249, 255, 214, 220, 162, 163, 165, 8359, 402,
            225, 237, 243, 250, 241, 209, 170, 186, 191, 8976, 172, 189, 188, 161, 171, 187,
            9617, 9618, 9619, 9474, 9508, 9569, 9570, 9558, 9557, 9571, 9553, 9559, 9565, 9564, 9563, 9488,
            9492, 9524, 9516, 9500, 9472, 9532, 9566, 9567, 9562, 9556, 9577, 9574, 9568, 9552, 9580, 9575,
            9576, 9572, 9573, 9561, 9560, 9554, 9555, 9579, 9578, 9496, 9484, 9608, 9604, 9612, 9616, 9600,
            945, 223, 915, 960, 931, 963, 181, 964, 934, 920, 937, 948, 8734, 966, 949, 8745,
            8801, 177, 8805, 8804, 8992, 8993, 247, 8776, 176, 8729, 183, 8730, 8319, 178, 9632, 160
        }

        Private CodePage437ReverseLookupTable As Dictionary(Of UShort, Byte)

        Public Function BootSectorDescription(Offset As DiskImage.BootSector.BootSectorOffsets) As String
            Select Case Offset
                Case DiskImage.BootSector.BootSectorOffsets.JmpBoot
                    Return "Bootstrap Jump"
                Case DiskImage.BootSector.BootSectorOffsets.OEMName
                    Return "OEM Name"
                Case DiskImage.BootSector.BootSectorOffsets.BytesPerSector
                    Return "Bytes per Sector"
                Case DiskImage.BootSector.BootSectorOffsets.SectorsPerCluster
                    Return "Sectors per Cluster"
                Case DiskImage.BootSector.BootSectorOffsets.ReservedSectorCount
                    Return "Reserved Sectors"
                Case DiskImage.BootSector.BootSectorOffsets.NumberOfFATs
                    Return "Number of FATs"
                Case DiskImage.BootSector.BootSectorOffsets.RootEntryCount
                    Return "Root Directory Entries"
                Case DiskImage.BootSector.BootSectorOffsets.SectorCountSmall
                    Return "Total Sector Count"
                Case DiskImage.BootSector.BootSectorOffsets.MediaDescriptor
                    Return "Media Descriptor"
                Case DiskImage.BootSector.BootSectorOffsets.SectorsPerFAT
                    Return "Sectors per FAT"
                Case DiskImage.BootSector.BootSectorOffsets.SectorsPerTrack
                    Return "Sectors per Track"
                Case DiskImage.BootSector.BootSectorOffsets.NumberOfHeads
                    Return "Number of Heads"
                Case DiskImage.BootSector.BootSectorOffsets.HiddenSectors
                    Return "Hidden Sectors"
                Case DiskImage.BootSector.BootSectorOffsets.DriveNumber
                    Return "Drive Number"
                Case DiskImage.BootSector.BootSectorOffsets.Reserved
                    Return "Reserved"
                Case DiskImage.BootSector.BootSectorOffsets.ExtendedBootSignature
                    Return "Extended Boot Signature"
                Case DiskImage.BootSector.BootSectorOffsets.VolumeSerialNumber
                    Return "Volume Serial Number"
                Case DiskImage.BootSector.BootSectorOffsets.VolumeLabel
                    Return "Volume Label"
                Case DiskImage.BootSector.BootSectorOffsets.FileSystemType
                    Return "File System ID"
                Case DiskImage.BootSector.BootSectorOffsets.BootStrapSignature
                    Return "Boot Sector Signature"
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function BuildBootSectorFromFileSize(Size As Integer) As BootSector
            Dim Data(511) As Byte
            Dim FileBytes As New ImageByteArray(Data)
            Dim BootSector = New BootSector(FileBytes)

            Select Case Size
                Case 163840
                    BootSector.BytesPerSector = 512
                    BootSector.HiddenSectors = 0
                    BootSector.MediaDescriptor = &HFE
                    BootSector.NumberOfFATs = 2
                    BootSector.NumberOfHeads = 1
                    BootSector.ReservedSectorCount = 1
                    BootSector.RootEntryCount = 64
                    BootSector.SectorCountSmall = 320
                    BootSector.SectorsPerCluster = 1
                    BootSector.SectorsPerFAT = 1
                    BootSector.SectorsPerTrack = 8
                Case 184320
                    BootSector.BytesPerSector = 512
                    BootSector.HiddenSectors = 0
                    BootSector.MediaDescriptor = &HFC
                    BootSector.NumberOfFATs = 2
                    BootSector.NumberOfHeads = 1
                    BootSector.ReservedSectorCount = 1
                    BootSector.RootEntryCount = 64
                    BootSector.SectorCountSmall = 360
                    BootSector.SectorsPerCluster = 1
                    BootSector.SectorsPerFAT = 1
                    BootSector.SectorsPerTrack = 9
                Case 327680
                    BootSector.BytesPerSector = 512
                    BootSector.HiddenSectors = 0
                    BootSector.MediaDescriptor = &HFF
                    BootSector.NumberOfFATs = 2
                    BootSector.NumberOfHeads = 2
                    BootSector.ReservedSectorCount = 1
                    BootSector.RootEntryCount = 112
                    BootSector.SectorCountSmall = 640
                    BootSector.SectorsPerCluster = 2
                    BootSector.SectorsPerFAT = 1
                    BootSector.SectorsPerTrack = 8
                Case 368640
                    BootSector.BytesPerSector = 512
                    BootSector.HiddenSectors = 0
                    BootSector.MediaDescriptor = &HFD
                    BootSector.NumberOfFATs = 2
                    BootSector.NumberOfHeads = 2
                    BootSector.ReservedSectorCount = 1
                    BootSector.RootEntryCount = 112
                    BootSector.SectorCountSmall = 720
                    BootSector.SectorsPerCluster = 2
                    BootSector.SectorsPerFAT = 2
                    BootSector.SectorsPerTrack = 9
                Case 737280
                    BootSector.BytesPerSector = 512
                    BootSector.HiddenSectors = 0
                    BootSector.MediaDescriptor = &HF9
                    BootSector.NumberOfFATs = 2
                    BootSector.NumberOfHeads = 2
                    BootSector.ReservedSectorCount = 1
                    BootSector.RootEntryCount = 112
                    BootSector.SectorCountSmall = 1440
                    BootSector.SectorsPerCluster = 2
                    BootSector.SectorsPerFAT = 3
                    BootSector.SectorsPerTrack = 9
                Case 1228800
                    BootSector.BytesPerSector = 512
                    BootSector.HiddenSectors = 0
                    BootSector.MediaDescriptor = &HF9
                    BootSector.NumberOfFATs = 2
                    BootSector.NumberOfHeads = 2
                    BootSector.ReservedSectorCount = 1
                    BootSector.RootEntryCount = 224
                    BootSector.SectorCountSmall = 2400
                    BootSector.SectorsPerCluster = 1
                    BootSector.SectorsPerFAT = 7
                    BootSector.SectorsPerTrack = 15
                Case 1474560
                    BootSector.BytesPerSector = 512
                    BootSector.HiddenSectors = 0
                    BootSector.MediaDescriptor = &HF0
                    BootSector.NumberOfFATs = 2
                    BootSector.NumberOfHeads = 2
                    BootSector.ReservedSectorCount = 1
                    BootSector.RootEntryCount = 224
                    BootSector.SectorCountSmall = 2880
                    BootSector.SectorsPerCluster = 1
                    BootSector.SectorsPerFAT = 9
                    BootSector.SectorsPerTrack = 18
            End Select

            Return BootSector
        End Function

        Public Function BytesToSector(Bytes As UInteger) As UInteger
            Return Math.Ceiling(Bytes / BYTES_PER_SECTOR)
        End Function

        Public Function CheckValidFileName(FileName() As Byte, IsExtension As Boolean, IsVolumeName As Boolean) As Boolean
            Dim Result As Boolean = True
            Dim C As Byte
            Dim SpaceAllowed As Boolean = True

            If FileName.Length = 0 Then
                Return IsExtension
            End If

            For Index = FileName.Length - 1 To 0 Step -1
                C = FileName(Index)
                If Not IsVolumeName And (C <> &H20 Or (Not IsExtension And Index = 0)) Then
                    SpaceAllowed = False
                End If

                If C = &H20 And SpaceAllowed Then
                    Result = True
                ElseIf IsVolumeName And (C = &H0 Or (Not IsExtension And Index = 1 And C = &H3)) Then
                    Result = True
                ElseIf C < &H21 Then
                    Result = False
                    Exit For
                ElseIf Not IsVolumeName And C > &H60 And C < &H7B Then
                    Result = False
                    Exit For
                ElseIf Not IsVolumeName And InvalidFileChars.Contains(C) Then
                    Result = False
                    Exit For
                End If
            Next

            Return Result
        End Function

        Public Function CodePage437ToUnicode(b() As Byte) As String
            Dim b2(b.Length - 1) As UShort
            For counter = 0 To b.Length - 1
                b2(counter) = CodePage437LookupTable(b(counter))
            Next
            ReDim b(b2.Length * 2 - 1)
            Buffer.BlockCopy(b2, 0, b, 0, b.Length)

            Return Encoding.Unicode.GetString(b)
        End Function

        Public Function DateToFATDate(D As Date) As UShort
            Dim FATDate As UShort = D.Year - 1980

            FATDate <<= 4
            FATDate += D.Month
            FATDate <<= 5
            FATDate += D.Day

            Return FATDate
        End Function

        Public Function DateToFATMilliseconds(D As Date) As Byte
            Return D.Millisecond \ 10 + (D.Second Mod 2) * 100
        End Function

        Public Function DateToFATTime(D As Date) As UShort
            Dim DTTime As UShort = D.Hour

            DTTime <<= 6
            DTTime += D.Minute
            DTTime <<= 5
            DTTime += D.Second \ 2

            Return DTTime
        End Function

        Public Function ExpandDate(FATDate As UShort) As ExpandedDate
            Return ExpandDate(FATDate, 0, 0)
        End Function

        Public Function ExpandDate(FATDate As UShort, FATTime As UShort) As ExpandedDate
            Return ExpandDate(FATDate, FATTime, 0)
        End Function

        Public Function ExpandDate(FATDate As UShort, FATTime As UShort, Milliseconds As Byte) As ExpandedDate
            Dim DT As ExpandedDate

            DT.Day = FATDate Mod 32
            FATDate >>= 5
            DT.Month = FATDate Mod 16
            FATDate >>= 4
            DT.Year = 1980 + FATDate Mod 128

            DT.Second = (FATTime Mod 32) * 2
            FATTime >>= 5
            DT.Minute = FATTime Mod 64
            FATTime >>= 6
            DT.Hour = FATTime Mod 32

            If Milliseconds < 200 Then
                DT.Second += (Milliseconds \ 100)
                DT.Milliseconds = (Milliseconds Mod 100) * 10
            Else
                DT.Milliseconds = 0
            End If

            Dim DateString As String = DT.Year & "-" & Format(DT.Month, "00") & "-" & Format(DT.Day, "00") & " " & DT.Hour & ":" & Format(DT.Minute, "00") & ":" & Format(DT.Second, "00") & "." & DT.Milliseconds.ToString.PadLeft(3, "0")

            DT.IsValidDate = IsDate(DateString)

            DT.IsPM = (DT.Hour > 11)

            If DT.Hour > 12 Then
                DT.Hour12 = DT.Hour - 12
            ElseIf DT.Hour = 0 Then
                DT.Hour12 = 12
            Else
                DT.Hour12 = DT.Hour
            End If

            If DT.IsValidDate Then
                DT.DateObject = New Date(DT.Year, DT.Month, DT.Day, DT.Hour, DT.Minute, DT.Second, DT.Milliseconds)
            End If

            Return DT
        End Function

        Public Function GetDataFromChain(FileBytes As ByteArray, SectorChain As List(Of UInteger)) As Byte()
            Dim SectorSize As UInteger = BYTES_PER_SECTOR
            Dim Content(SectorChain.Count * SectorSize - 1) As Byte
            Dim ContentOffset As UInteger = 0

            For Each Sector In SectorChain
                Dim Offset As UInteger = SectorToBytes(Sector)
                If FileBytes.Length < Offset + SectorSize Then
                    SectorSize = Math.Max(FileBytes.Length - Offset, 0)
                End If
                If SectorSize > 0 Then
                    FileBytes.CopyTo(Offset, Content, ContentOffset, SectorSize)
                    ContentOffset += SectorSize
                Else
                    Exit For
                End If
            Next

            Return Content
        End Function

        Public Function GetDirectoryEntryCount(FileBytes As ByteArray, OffsetStart As UInteger, OffsetEnd As UInteger, FileCountOnly As Boolean) As UInteger
            Dim Count As UInteger = 0

            Do While FileBytes.GetByte(OffsetStart) > 0
                If Not FileCountOnly Then
                    Count += 1
                ElseIf FileBytes.GetByte(OffsetStart + 11) <> &HF Then 'Exclude LFN entries
                    Dim FilePart = FileBytes.ToUInt16(OffsetStart)
                    If FilePart <> &H202E And FilePart <> &H2E2E Then 'Exclude '.' and '..' entries
                        Count += 1
                    End If
                End If
                OffsetStart += 32
                If OffsetEnd > 0 And OffsetStart >= OffsetEnd Then
                    Exit Do
                End If
            Loop

            Return Count
        End Function

        Public Function GetObjectSize(o As Object) As Integer
            Dim t = o.GetType()
            If t.Equals(GetType(Byte)) Then
                Return 1
            ElseIf t.Equals(GetType(UShort)) Then
                Return 2
            ElseIf t.Equals(GetType(UInteger)) Then
                Return 4
            ElseIf t.Equals(GetType(Byte())) Then
                Return CType(o, Byte()).Length
            Else
                Return 0
            End If
        End Function

        Public Function IsDataBlockEmpty(Data() As Byte) As Boolean
            Dim EmptyByte As Byte = Data(0)
            If EmptyByte <> &HF6 And EmptyByte <> &H0 Then
                Return False
            Else
                For Each B In Data
                    If B <> EmptyByte Then
                        Return False
                        Exit For
                    End If
                Next
            End If

            Return True
        End Function

        Public Function OffsetToSector(Offset As UInteger) As UInteger
            Return Offset \ BYTES_PER_SECTOR
        End Function

        Public Sub ResizeArray(ByRef b() As Byte, Length As UInteger, Padding As Byte)
            Dim Size = b.Length - 1
            If Size <> Length - 1 Then
                ReDim Preserve b(Length - 1)
                For Counter As UInteger = Size + 1 To Length - 1
                    b(Counter) = Padding
                Next
            End If
        End Sub

        Public Function SectorToBytes(Sector As UInteger) As UInteger
            Return Sector * BYTES_PER_SECTOR
        End Function

        Public Function UnicodeToCodePage437(Value As String) As Byte()
            If CodePage437ReverseLookupTable Is Nothing Then
                CodePage437ReverseLookupTable = New Dictionary(Of UShort, Byte)
                For Counter = 0 To CodePage437LookupTable.Length - 1
                    CodePage437ReverseLookupTable.Add(CodePage437LookupTable(Counter), Counter)
                Next
            End If

            Dim b = Encoding.Unicode.GetBytes(Value)
            Dim b2(b.Length / 2 - 1) As Byte
            For Counter = 0 To b.Length - 1 Step 2
                Dim c = BitConverter.ToUInt16(b, Counter)
                If CodePage437ReverseLookupTable.ContainsKey(c) Then
                    c = CodePage437ReverseLookupTable.Item(c)
                Else
                    c = 32
                End If
                b2(Counter / 2) = c
            Next

            Return b2
        End Function
    End Module
End Namespace
