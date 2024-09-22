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

    Public Function ToBigEndianUInt16(value() As Byte) As UInt16
        Return CType(value(0), UInt16) << 8 Or value(1)
    End Function

    Public Function ToBigEndianUInt16(value As UInt16) As UInt16
        Return CUShort((value >> 8) Or (value << 8))
    End Function

    Public Function TranscopyGetImageType(tc As TransCopyImage) As DiskImage.FloppyDiskType
        Dim DiskType As DiskImage.FloppyDiskType
        Dim MaxSectors As Byte

        Dim SectorCount As Byte = 0
        For Each Cylinder In tc.Cylinders
            If Cylinder.DecodedData IsNot Nothing Then
                If Cylinder.Data.Length > 23000 Then
                    MaxSectors = 18
                ElseIf Cylinder.Data.Length > 18000 Then
                    MaxSectors = 15
                Else
                    MaxSectors = 9
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
            If SectorCount = 15 Then
                DiskType = DiskImage.FloppyDiskType.Floppy1200
            ElseIf SectorCount = 18 Then
                DiskType = DiskImage.FloppyDiskType.Floppy1440
            Else
                DiskType = DiskImage.FloppyDiskType.Floppy720
            End If
        Else
            If tc.Sides = 1 Then
                If SectorCount = 8 Then
                    DiskType = DiskImage.FloppyDiskType.Floppy160
                Else
                    DiskType = DiskImage.FloppyDiskType.Floppy180
                End If
            Else
                If SectorCount = 8 Then
                    DiskType = DiskImage.FloppyDiskType.Floppy320
                Else
                    DiskType = DiskImage.FloppyDiskType.Floppy360
                End If
            End If
        End If

        Return DiskType
    End Function

    Public Function TranscopyToSectorImage(tc As TransCopyImage, DiskType As DiskImage.FloppyDiskType) As Byte()
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

    Private Function HexFormat(Data() As Byte) As String
        Dim HexString = BitConverter.ToString(Data).Replace("-", " ")
        Dim TempString = ""
        Do While Len(HexString) > 47
            If TempString.Length > 0 Then
                TempString &= vbCrLf
            End If
            TempString &= Left(HexString, 47)
            HexString = Right(HexString, Len(HexString) - 48)
        Loop
        If Len(HexString) > 0 Then
            If TempString.Length > 0 Then
                TempString &= vbCrLf
            End If
            TempString &= HexString
        End If

        Return TempString
    End Function

    Public Sub MFMTrackDebug(MFMTrack As MFMTrack, TrackName As String)
        Dim file = My.Computer.FileSystem.OpenTextFileWriter("H:\debug.log", True)
        file.Write("Trk: " & TrackName)
        file.Write(", Gap 4A: " & MFMTrack.Gap4A.Length)
        If MFMTrack.Gap4A.Length > 0 Then
            file.Write(", IAM Mark: " & MFMTrack.IAMMark.ToString("X2") & If(MFMTrack.IAMMark <> MFMTrack.MFMAddressMarks.Index, " (Unexpected)", ""))
        End If
        file.WriteLine(", Gap 1: " & MFMTrack.Gap1.Length)
        If MFMTrack.Gap4A.Length > 0 Then
            file.WriteLine("Gap 4A:")
            file.WriteLine(HexFormat(MFMTrack.Gap4A))
            file.WriteLine("")
        End If
        If MFMTrack.Gap1.Length > 0 Then
            file.WriteLine("Gap 1:")
            file.WriteLine(HexFormat(MFMTrack.Gap1))
            file.WriteLine("")
        End If

        For Each Sector In MFMTrack.Sectors
            Dim ChecksumValid = Sector.Checksum = Sector.CalculatedChecksum
            Dim DataChecksumValid = Sector.DataChecksum = Sector.CalculatedDataChecksum

            file.Write("Trk: " & Sector.Track & "." & Sector.Side)
            file.Write(", Id: " & Sector.SectorId)
            file.Write(", Size: " & Sector.Size)
            file.Write(", Checksum: " & Sector.Checksum.ToString("X4") & If(ChecksumValid, "", "#"))
            file.Write(", Data Checksum: " & Sector.DataChecksum.ToString("X4") & If(DataChecksumValid, "", "#"))
            file.Write(", Gap2: " & Sector.Gap2.Length)
            file.Write(", Gap3: " & Sector.Gap3.Length)
            file.WriteLine(", Overlaps: " & Sector.Overlaps)
            If Sector.Gap2.Length > 0 Then
                file.WriteLine("Gap 2:")
                file.WriteLine(HexFormat(Sector.Gap2))
                file.WriteLine("")
            End If
            If Sector.Gap3.Length > 0 Then
                file.WriteLine("Gap 3:")
                file.WriteLine(HexFormat(Sector.Gap3))
                file.WriteLine("")
            End If
        Next
        file.Close()
    End Sub

End Module
