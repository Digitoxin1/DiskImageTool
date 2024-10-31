Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Bitstream
    Module Functions
        Private ReadOnly ValidSectorCount() As Byte = {8, 9, 15, 18, 36}
        Public Function BitstreamGetImageFormat(Image As IBitstreamImage) As FloppyDiskFormat
            Dim DiskFormat As FloppyDiskFormat

            Dim SectorCount = InferSectorCount(Image)

            If Image.TrackCount \ Image.TrackStep >= 79 Then
                If SectorCount >= 36 Then
                    DiskFormat = FloppyDiskFormat.Floppy2880
                ElseIf SectorCount >= 18 Then
                    DiskFormat = FloppyDiskFormat.Floppy1440
                ElseIf SectorCount >= 15 Then
                    DiskFormat = FloppyDiskFormat.Floppy1200
                Else
                    DiskFormat = FloppyDiskFormat.Floppy720
                End If
            Else
                If Image.SideCount = 1 Then
                    If SectorCount < 9 Then
                        DiskFormat = FloppyDiskFormat.Floppy160
                    Else
                        DiskFormat = FloppyDiskFormat.Floppy180
                    End If
                Else
                    If SectorCount < 9 Then
                        DiskFormat = FloppyDiskFormat.Floppy320
                    Else
                        DiskFormat = FloppyDiskFormat.Floppy360
                    End If
                End If
            End If

            Return DiskFormat
        End Function
        Public Function CalculatetBitCount(BitRate As UShort, RPM As UShort) As UInteger
            Dim MicrosecondsPerRev As Single = 1 / RPM * 60 * 1000000
            Dim MicrossecondsPerBit As Single = BitRate / 1000

            Return (MicrosecondsPerRev * MicrossecondsPerBit \ 8) * 16
        End Function

        Public Function CalculatetRPM(BitRate As UShort, BitCount As UInteger) As UShort
            Dim MicrossecondsPerBit As Single = BitRate / 1000
            Dim MicrosecondsPerRev As Single = BitCount / (2 * MicrossecondsPerBit)

            Return RoundRPM((60 * 1000000) / MicrosecondsPerRev)
        End Function

        Public Function InferRPM(BitCount As UInteger) As UShort
            BitCount = Math.Round(BitCount / 10000) * 10000

            If BitCount <= 130000 Then
                Return 300
            ElseIf BitCount = 170000 Then
                Return 360
            ElseIf BitCount = 200000 Then
                Return 300
            ElseIf BitCount = 400000 Then
                Return 300
            Else
                Return 300
            End If
        End Function

        Public Function InferBitRate(BitCount As UInteger) As UShort
            BitCount = Math.Round(BitCount / 10000) * 10000

            If BitCount <= 130000 Then
                Return 250
            ElseIf BitCount = 170000 Then
                Return 500
            ElseIf BitCount = 200000 Then
                Return 500
            ElseIf BitCount = 400000 Then
                Return 1000
            ElseIf BitCount = 800000 Then
                Return 2000
            Else
                Return 0
            End If
        End Function

        Public Function InferSectorCount(BitCount As UInteger) As UShort
            BitCount = Math.Round(BitCount / 10000) * 10000

            If BitCount <= 130000 Then
                Return 9
            ElseIf BitCount = 170000 Then
                Return 15
            ElseIf BitCount = 200000 Then
                Return 18
            ElseIf BitCount = 400000 Then
                Return 36
            Else
                Return 0
            End If
        End Function

        Public Function InferSectorCount(Image As IBitstreamImage) As UShort
            Dim Track As IBitstreamTrack
            Dim SectorCount As Byte = 0
            Dim TotalSize As UShort
            Dim CountFound As Boolean = False

            For i = 0 To Image.TrackCount - 1 Step Image.TrackStep
                For j = 0 To Image.SideCount - 1
                    Track = Image.GetTrack(i, j)
                    If Track IsNot Nothing AndAlso Track.MFMData IsNot Nothing Then
                        TotalSize = GetSectorSize(Track.MFMData.Sectors)
                        SectorCount = TotalSize \ 512
                        If ValidSectorCount.Contains(SectorCount) Then
                            CountFound = True
                            Exit For
                        End If
                    End If
                Next
                If CountFound Then
                    Exit For
                End If
            Next

            If SectorCount < 8 Then
                SectorCount = 8
            ElseIf SectorCount < 11 Then
                SectorCount = 9
            End If

            Return SectorCount
        End Function

        Public Function RoundBitRate(BitRate As UShort) As UShort
            Return Math.Round(BitRate / 50) * 50
        End Function

        Public Function RoundRPM(RPM As UShort) As UShort
            Return Math.Round(RPM / 60) * 60
        End Function

        Private Function GetSectorSize(Sectors As List(Of IBM_MFM.IBM_MFM_Sector)) As UShort
            Dim TotalSize As UShort = 0
            For Each Sector In Sectors
                If Not Sector.Overlaps Then
                    TotalSize += Sector.GetSizeBytes
                End If
            Next
            Return TotalSize
        End Function
    End Module
End Namespace
