Module BitstreamUtil
    Private Const crcPoly As UShort = &H1021
    Private Const crcIV As UShort = &HFFFF

    Public Function BytesToBits(bytes() As Byte) As BitArray
        Dim buffer(bytes.Length - 1) As Byte

        For i As Integer = 0 To bytes.Length - 1
            buffer(i) = ReverseBits(bytes(i))
        Next

        Return New BitArray(buffer)
    End Function

    'Public Function BytesToBits(bytes() As Byte) As BitArray
    '    Dim BitStream As New BitArray(bytes.Length * 8)

    '    Dim i As Integer = 0
    '    For Each b As Byte In bytes
    '        For j As Integer = 7 To 0 Step -1
    '            Dim value As Boolean = (b And (1 << j)) <> 0
    '            BitStream.Set(i, value)
    '            i += 1
    '        Next j
    '    Next

    '    Return BitStream
    'End Function

    Private Function CheckDoubleSizeSectors(Sectors As List(Of MFMSector)) As Boolean
        Dim Result As Boolean = True
        Dim SectorCount As UShort

        SectorCount = 0
        For Each MFMSector In Sectors
            If MFMSector.Data IsNot Nothing Then
                If MFMSector.SectorId >= 1 And MFMSector.SectorId <= 8 Then
                    If MFMSector.SectorId > 4 Then
                        Result = False
                        Exit For
                    ElseIf MFMSector.Size <> 1024 Then
                        Result = False
                        Exit For
                    Else
                        SectorCount += 1
                    End If
                End If
            End If
        Next
        If SectorCount <> 4 Then
            Result = False
        End If

        Return Result
    End Function

    Public Function Crc16(data As Byte()) As UShort
        Dim crc As UShort = crcIV

        For Each b As Byte In data
            crc = crc Xor (CUShort(b) << 8)
            For i As Integer = 0 To 7
                If (crc And &H8000) <> 0 Then
                    crc = CUShort((crc << 1) Xor crcPoly)
                Else
                    crc = CUShort(crc << 1)
                End If
            Next
        Next

        Return crc
    End Function

    Public Function Crc16BigEndian(data As Byte()) As UShort
        Return ToBigEndianUInt16(Crc16(data))
    End Function

    Private Function GetImageOffset(ImageParams As DiskImage.FloppyDiskParams, Track As UShort, Side As Byte, Sector As UShort) As UInteger
        Dim Offset = Track * ImageParams.NumberOfHeads * ImageParams.SectorsPerTrack + ImageParams.SectorsPerTrack * Side + Sector

        Return Offset * 512
    End Function

    Public Function ReverseBits(b As Byte) As Byte
        b = ((b >> 1) And &H55) Or ((b << 1) And &HAA) ' Swap odd and even bits
        b = ((b >> 2) And &H33) Or ((b << 2) And &HCC) ' Swap consecutive pairs
        b = ((b >> 4) And &HF) Or ((b << 4) And &HF0)  ' Swap nibbles

        Return b
    End Function

    Public Function ToBigEndianUInt16(value() As Byte, Optional StartIndex As Integer = 0) As UInt16
        Return CType(value(StartIndex), UInt16) << 8 Or value(StartIndex + 1)
    End Function

    Public Function ToBigEndianUInt16(value As UInt16) As UInt16
        Return CUShort((value >> 8) Or (value << 8))
    End Function

    Public Function ToBigEndianUInt32(value() As Byte, Optional StartIndex As Integer = 0) As UInt32
        Return ToBigEndianUInt32(BitConverter.ToUInt32(value, StartIndex))
    End Function

    Public Function ToBigEndianUInt32(value As UInt32) As UInt32
        ' Manually reorder the bytes to convert to Big Endian
        Dim byte1 As UInt32 = (value And &HFF) << 24        ' Get the lowest byte and shift it to the highest byte position
        Dim byte2 As UInt32 = (value And &HFF00) << 8       ' Get the second lowest byte and shift it
        Dim byte3 As UInt32 = (value And &HFF0000) >> 8     ' Get the second highest byte and shift it
        Dim byte4 As UInt32 = (value And &HFF000000UI) >> 24 ' Get the highest byte and shift it to the lowest byte position

        ' Combine the bytes into the final Big Endian UInt32 value
        Return byte1 Or byte2 Or byte3 Or byte4
    End Function

    Public Function PSIGetImageType(PSI As PSIImage.PSISectorImage) As DiskImage.FloppyDiskType
        Dim DiskType As DiskImage.FloppyDiskType
        Dim MaxSectors As Byte

        If PSI.DefaultSectorFormat = PSIImage.DefaultSectorFormat.IBM_MFM_DD Then
            MaxSectors = 9
        ElseIf PSI.DefaultSectorFormat = PSIImage.DefaultSectorFormat.IBM_MFM_HD Then
            MaxSectors = 18
        ElseIf PSI.DefaultSectorFormat = PSIImage.DefaultSectorFormat.IBM_MFM_ED Then
            MaxSectors = 36
        End If

        Dim SectorCount As Byte = 0
        Dim CylinderCount As UShort = 0
        Dim HeadCount As UShort = 0

        For Each Sector In PSI.Sectors
            If Sector.Sector >= 1 And Sector.Sector <= MaxSectors Then
                If Sector.Sector > SectorCount Then
                    SectorCount = Sector.Sector
                End If
            End If
            If Sector.Cylinder + 1 > CylinderCount Then
                CylinderCount = Sector.Cylinder + 1
            End If
            If Sector.Head + 1 > HeadCount Then
                HeadCount = Sector.Head + 1
            End If
        Next

        If PSI.DefaultSectorFormat = PSIImage.DefaultSectorFormat.IBM_MFM_DD Then
            If CylinderCount >= 79 Then
                DiskType = DiskImage.FloppyDiskType.Floppy720
            Else
                If HeadCount = 1 Then
                    If SectorCount < 9 Then
                        DiskType = DiskImage.FloppyDiskType.Floppy160
                    Else
                        DiskType = DiskImage.FloppyDiskType.Floppy180
                    End If
                Else
                    If SectorCount < 9 Then
                        DiskType = DiskImage.FloppyDiskType.Floppy320
                    Else
                        DiskType = DiskImage.FloppyDiskType.Floppy360
                    End If
                End If
            End If
        ElseIf PSI.DefaultSectorFormat = PSIImage.DefaultSectorFormat.IBM_MFM_HD Then
            If SectorCount > 15 Then
                DiskType = DiskImage.FloppyDiskType.Floppy1440
            Else
                DiskType = DiskImage.FloppyDiskType.Floppy1200
            End If
        Else
            DiskType = DiskImage.FloppyDiskType.Floppy2880

        End If

        Return DiskType
    End Function

    Public Function TranscopyGetImageType(tc As Transcopy.TransCopyImage) As DiskImage.FloppyDiskType
        Dim DiskType As DiskImage.FloppyDiskType
        Dim MaxSectors As Byte

        Dim SectorCount As Byte = 0
        For Each Cylinder In tc.Cylinders
            If Cylinder.DecodedData IsNot Nothing Then
                If Cylinder.TrackType = Transcopy.TransCopyDiskType.MFMDoubleDensity Or Cylinder.TrackType = Transcopy.TransCopyDiskType.MFMDoubleDensity360RPM Then
                    MaxSectors = 9
                Else
                    MaxSectors = 18
                End If
                For Each Sector In Cylinder.DecodedData.Sectors
                    If Sector.SectorId >= 1 And Sector.SectorId <= MaxSectors Then
                        If Sector.SectorId > SectorCount Then
                            SectorCount = Sector.SectorId
                        End If
                    End If
                Next
            End If
        Next

        If tc.CylinderEnd < 45 And SectorCount > 9 Then
            SectorCount = 9
        End If

        If tc.CylinderEnd >= 79 Then
            If SectorCount > 15 Then
                DiskType = DiskImage.FloppyDiskType.Floppy1440
            ElseIf SectorCount > 9 Then
                DiskType = DiskImage.FloppyDiskType.Floppy1200
            Else
                DiskType = DiskImage.FloppyDiskType.Floppy720
            End If
        Else
            If tc.Sides = 1 Then
                If SectorCount < 9 Then
                    DiskType = DiskImage.FloppyDiskType.Floppy160
                Else
                    DiskType = DiskImage.FloppyDiskType.Floppy180
                End If
            Else
                If SectorCount < 9 Then
                    DiskType = DiskImage.FloppyDiskType.Floppy320
                Else
                    DiskType = DiskImage.FloppyDiskType.Floppy360
                End If
            End If
        End If

        Return DiskType
    End Function

    Public Function PSIToSectorImage(PSI As PSIImage.PSISectorImage, DiskType As DiskImage.FloppyDiskType) As Byte()
        Dim ImageSize = DiskImage.GetFloppyDiskSize(DiskType)
        Dim ImageParams = DiskImage.GetFloppyDiskParams(DiskType)

        Dim SectorImage(ImageSize - 1) As Byte
        Dim TrackCount = Int(ImageParams.SectorCountSmall / ImageParams.SectorsPerTrack / ImageParams.NumberOfHeads)

        For Each PSISector In PSI.Sectors
            Dim MaxSize As UInteger = ImageParams.BytesPerSector
            Dim MaxSectors As UShort = ImageParams.SectorsPerTrack
            Dim SectorStep As Byte = 1

            If Not PSISector.IsAlternateSector Then
                If PSISector.Cylinder < TrackCount And PSISector.Head < ImageParams.NumberOfHeads Then
                    If PSISector.Sector >= 1 And PSISector.Sector <= MaxSectors And PSISector.Size <= MaxSize Then
                        Dim Sector = PSISector.Sector * SectorStep - SectorStep
                        Dim Offset = GetImageOffset(ImageParams, PSISector.Cylinder, PSISector.Head, Sector)
                        Dim Size As UShort
                        Dim Data() As Byte
                        If PSISector.IsCompressed Then
                            Data = New Byte(PSISector.Size - 1) {}
                            For i = 0 To Data.Length - 1
                                Data(i) = PSISector.CkmpressedSectorData
                            Next
                            Size = PSISector.Size
                        Else
                            Data = PSISector.Data
                            Size = Math.Min(PSISector.Size, PSISector.Data.Length)
                        End If
                        Array.Copy(Data, 0, SectorImage, Offset, Size)
                    End If
                End If
            End If
        Next

        Return SectorImage
    End Function

    Public Function TranscopyToSectorImage(tc As Transcopy.TransCopyImage, DiskType As DiskImage.FloppyDiskType) As Byte()
        Dim ImageSize = DiskImage.GetFloppyDiskSize(DiskType)
        Dim ImageParams = DiskImage.GetFloppyDiskParams(DiskType)

        Dim SectorImage(ImageSize - 1) As Byte
        Dim TrackCount = Int(ImageParams.SectorCountSmall / ImageParams.SectorsPerTrack / ImageParams.NumberOfHeads)

        For Each Cylinder In tc.Cylinders
            Dim MaxSize As UInteger = ImageParams.BytesPerSector
            Dim MaxSectors As UShort = ImageParams.SectorsPerTrack
            Dim SectorStep As Byte = 1
            If Cylinder.DecodedData IsNot Nothing Then
                If Cylinder.Track < TrackCount And Cylinder.Side < ImageParams.NumberOfHeads Then
                    If ImageParams.SectorsPerTrack = 8 AndAlso CheckDoubleSizeSectors(Cylinder.DecodedData.Sectors) Then
                        MaxSize *= 2
                        MaxSectors /= 2
                        SectorStep = 2
                    End If
                    For Each MFMSector In Cylinder.DecodedData.Sectors
                        If MFMSector.Data IsNot Nothing Then
                            If MFMSector.SectorId >= 1 And MFMSector.SectorId <= MaxSectors And Not MFMSector.Overlaps And MFMSector.Size <= MaxSize Then
                                Dim Sector = MFMSector.SectorId * SectorStep - SectorStep
                                Dim Offset = GetImageOffset(ImageParams, Cylinder.Track, Cylinder.Side, Sector)
                                Array.Copy(MFMSector.Data, 0, SectorImage, Offset, MFMSector.Size)
                            End If
                        End If
                    Next
                End If
            End If
        Next

        Return SectorImage
    End Function
End Module
