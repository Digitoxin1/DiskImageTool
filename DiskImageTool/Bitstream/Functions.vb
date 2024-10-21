Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Bitstream
    Module Functions
        Public Function BitstreamGetImageFormat(Image As IBitstreamImage) As FloppyDiskFormat
            Dim DiskFormat As FloppyDiskFormat
            Dim Track As IBitstreamTrack
            Dim SectorCount As Byte = 0

            If Image.TrackCount > 0 Then
                Track = Image.GetTrack(0, 0)
                If Track IsNot Nothing AndAlso Track.MFMData IsNot Nothing Then
                    Dim TotalSize As UShort = 0
                    For Each Sector In Track.MFMData.Sectors
                        If Not Sector.Overlaps And Sector.InitialChecksumValid Then
                            TotalSize += Sector.GetSizeBytes
                        End If
                    Next
                    SectorCount = TotalSize \ 512
                End If
            End If

            If Image.TrackCount \ Image.TrackStep >= 79 Then
                If SectorCount > 15 Then
                    DiskFormat = FloppyDiskFormat.Floppy1440
                ElseIf SectorCount > 9 Then
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

            If BitCount <= 100000 Then
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

            If BitCount <= 100000 Then
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

        Public Function RoundBitRate(BitRate As UShort) As UShort
            Return Math.Round(BitRate / 50) * 50
        End Function

        Public Function RoundRPM(RPM As UShort) As UShort
            Return Math.Round(RPM / 60) * 60
        End Function
    End Module
End Namespace
