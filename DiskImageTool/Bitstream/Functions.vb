Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports System.Security.Cryptography

Namespace Bitstream
    Module Functions
        Private ReadOnly ValidSectorCount() As Byte = {8, 9, 15, 18, 36}

        Public Function BitstreamCalculateHash(Image As IBitstreamImage, HashAlgorithm As HashAlgorithm, Optional AllTracks As Boolean = True) As String
            Dim BitstreamTrack As IBitstreamTrack
            Dim TrackCount As UShort

            If AllTracks Then
                TrackCount = Image.TrackCount
            Else
                TrackCount = StandardTrackCount(Image.TrackCount, Image.TrackStep)
            End If

            Dim NewTrackCount = Image.TrackCount \ Image.TrackStep

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                Dim NewTrack = Track \ Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)
                    If BitstreamTrack IsNot Nothing AndAlso BitstreamTrack.MFMData IsNot Nothing Then
                        Dim Sectors = BitstreamTrack.MFMData.Sectors
                        Dim Count = Sectors.Count
                        Dim IsMetadata As Boolean = False

                        ' Skip metadata tracks
                        If ((NewTrackCount = 41 And NewTrack = 40) Or (NewTrackCount = 81 And NewTrack = 80)) Then
                            If Count = 1 AndAlso Sectors(0).SectorId = 1 Then
                                IsMetadata = True
                            End If
                        End If

                        If Not IsMetadata Then
                            Dim SectorIndex As Integer = BitstreamTrack.MFMData.SectorStartIndex
                            For Counter = 0 To Count - 1
                                Dim MFMSector = Sectors(SectorIndex)
                                If MFMSector.DAMFound Then
                                    If MFMSector.InitialDataChecksumValid Then
                                        HashAlgorithm.TransformBlock(MFMSector.Data, 0, MFMSector.Data.Length, Nothing, 0)
                                    End If
                                End If
                                SectorIndex += 1
                                If SectorIndex = Count Then
                                    SectorIndex = 0
                                End If
                            Next
                        End If
                    End If
                Next
            Next
            HashAlgorithm.TransformFinalBlock(New Byte(0) {}, 0, 0)

            Return HashBytesToString(HashAlgorithm.Hash)
        End Function

        Public Function BitstreamGetImageFormat(Image As IBitstreamImage, BytesPerSector As UInteger) As FloppyDiskFormat
            Dim DiskFormat As FloppyDiskFormat

            Dim SectorCount = InferSectorCount(Image, BytesPerSector)

            If BytesPerSector = 1024 And SectorCount = 8 Then
                DiskFormat = FloppyDiskFormat.Floppy2HD

            ElseIf Image.TrackCount \ Image.TrackStep >= 79 Then
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

        Public Function GetBytesPerSector(Image As IBitstreamImage) As UInteger
            Dim BytesPerSector As UInteger = 512

            Dim Track = Image.GetTrack(0, 0)
            If Track IsNot Nothing Then
                Dim MFMData = Track.MFMData
                If MFMData IsNot Nothing Then
                    For Each Sector In MFMData.Sectors
                        If Sector.SectorId = 1 Then
                            BytesPerSector = Sector.GetSizeBytes
                            Exit For
                        End If
                    Next
                End If
            End If

            Return BytesPerSector
        End Function

        Public Function InferBitCount(BitCount As UInteger) As UInteger
            Dim BitRate = InferBitRate(BitCount)
            Dim RPM = InferRPM(BitCount)

            Return IBM_MFM.MFMGetSize(RPM, BitRate)
        End Function

        Public Function InferBitRate(BitCount As UInteger) As UShort
            BitCount = Math.Round(BitCount / 5000) * 5000

            If BitCount <= 135000 Then
                Return 250
            ElseIf BitCount >= 165000 And BitCount <= 175000 Then
                Return 500
            ElseIf BitCount >= 195000 And BitCount <= 205000 Then
                Return 500
            ElseIf BitCount >= 395000 And BitCount <= 405000 Then
                Return 1000
            ElseIf BitCount >= 795000 And BitCount <= 805000 Then
                Return 2000
            Else
                Return 0
            End If
        End Function

        Public Function InferRPM(BitCount As UInteger) As UShort
            BitCount = Math.Round(BitCount / 5000) * 5000

            If BitCount <= 135000 Then
                Return 300
            ElseIf BitCount >= 165000 And BitCount <= 175000 Then
                Return 360
            ElseIf BitCount >= 195000 And BitCount <= 205000 Then
                Return 300
            ElseIf BitCount >= 395000 And BitCount <= 405000 Then
                Return 300
            Else
                Return 300
            End If
        End Function
        Public Function InferSectorCount(BitCount As UInteger) As UShort
            BitCount = Math.Round(BitCount / 5000) * 5000

            If BitCount <= 135000 Then
                Return 9
            ElseIf BitCount >= 165000 And BitCount <= 175000 Then
                Return 15
            ElseIf BitCount >= 195000 And BitCount <= 205000 Then
                Return 18
            ElseIf BitCount >= 395000 And BitCount <= 405000 Then
                Return 36
            Else
                Return 0
            End If
        End Function

        Public Function InferSectorCount(Image As IBitstreamImage, BytesPerSector As UInteger) As UShort
            Dim Track As IBitstreamTrack
            Dim SectorCount As Byte = 0
            Dim TotalSize As UShort
            Dim CountFound As Boolean = False

            For i = 0 To Image.TrackCount - 1 Step Image.TrackStep
                For j = 0 To Image.SideCount - 1
                    Track = Image.GetTrack(i, j)
                    If Track IsNot Nothing AndAlso Track.MFMData IsNot Nothing Then
                        TotalSize = GetSectorSize(Track.MFMData.Sectors)
                        SectorCount = TotalSize \ BytesPerSector
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
            ElseIf SectorCount > 9 And SectorCount < 11 Then
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

        Private Function StandardTrackCount(TrackCount As UShort, TrackStep As Byte) As UShort
            Dim NewTrackCount = TrackCount \ TrackStep

            If NewTrackCount > 40 And NewTrackCount < 43 Then
                NewTrackCount = 40
            ElseIf NewTrackCount > 80 Then
                NewTrackCount = 80
            End If

            NewTrackCount = NewTrackCount * TrackStep

            Return NewTrackCount
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
