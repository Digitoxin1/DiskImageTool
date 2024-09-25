Module BitstreamDebug
    Public Sub DebugExportMFMBitstream(Bitstream As BitArray, FileName As String)
        Dim DebugFile = My.Computer.FileSystem.OpenTextFileWriter(FileName, False)
        Dim byteBuffer As Byte
        Dim bitCount As Integer = 0
        Dim byteCount As Integer = 0
        Dim lastDataBit As Boolean = True
        Dim SyncBit As Boolean = False
        Dim ZeroCount As Integer = 0
        Dim LostClock As Boolean = False
        For i As UInteger = 0 To Bitstream.Length - 2 Step 2
            Dim clockBit As Boolean = Bitstream(i)
            If Not clockBit Then
                ZeroCount += 1
            Else
                ZeroCount = 0
            End If
            Dim dataBit As Boolean = Bitstream(i + 1)
            If Not dataBit Then
                ZeroCount += 1
            Else
                ZeroCount = 0
            End If
            If Not lastDataBit And Not clockBit And Not dataBit Then
                SyncBit = True
            End If
            If ZeroCount > 3 Then
                LostClock = True
            End If
            If bitCount = 0 Then
                DebugFile.Write("0x" & byteCount.ToString("X4") & ": ")
            End If
            DebugFile.Write(CInt(clockBit) And 1)
            DebugFile.Write(CInt(dataBit) And 1)
            DebugFile.Write(" ")
            byteBuffer = (byteBuffer << 1) Or (CInt(dataBit) And 1)
            bitCount += 1
            If bitCount = 8 Then
                DebugFile.Write(": " & byteBuffer.ToString("X2"))
                If LostClock Then
                    DebugFile.Write(" Lost Clock")
                End If
                If SyncBit Then
                    DebugFile.Write(" SYNC")
                End If
                DebugFile.WriteLine()
                byteBuffer = 0
                bitCount = 0
                SyncBit = False
                LostClock = False
                byteCount += 1
            End If
            lastDataBit = dataBit
        Next
        DebugFile.Close()
    End Sub

    Public Sub DebugExportMFMTrack(MFMTrack As IBM_MFM.MFMTrack, TrackName As String, FileName As String)
        Dim DebugFile = My.Computer.FileSystem.OpenTextFileWriter(FileName, False)
        DebugFile.Write("Trk: " & TrackName)
        DebugFile.Write(", Gap 4A: " & MFMTrack.Gap4A.Length)
        If MFMTrack.Gap4A.Length > 0 Then
            DebugFile.Write(", IAM Mark: " & CByte(MFMTrack.IAM).ToString("X2"))
            If MFMTrack.IAM <> IBM_MFM.MFMAddressMark.Index Then
                DebugFile.Write(" (Unexpected)")
            End If
        End If
        DebugFile.WriteLine(", Gap 1: " & MFMTrack.Gap1.Length)
        If MFMTrack.Gap4A.Length > 0 Then
            DebugFile.WriteLine("Gap 4A:")
            DebugFile.WriteLine(HexFormat(MFMTrack.Gap4A))
            DebugFile.WriteLine("")
        End If
        If MFMTrack.Gap1.Length > 0 Then
            DebugFile.WriteLine("Gap 1:")
            DebugFile.WriteLine(HexFormat(MFMTrack.Gap1))
            DebugFile.WriteLine("")
        End If

        For Each Sector In MFMTrack.Sectors
            Dim ChecksumValid = Sector.IDChecksum = Sector.CalculateIDChecksum
            Dim DataChecksumValid = Sector.DataChecksum = Sector.CalculateDataChecksum

            DebugFile.Write("Trk: " & Sector.Track & "." & Sector.Side)
            DebugFile.Write(", Id: " & Sector.SectorId)
            DebugFile.Write(", Size: " & Sector.GetSizeBytes)
            DebugFile.Write(", Checksum: " & Sector.IDChecksum.ToString("X4") & If(ChecksumValid, "", "#"))
            DebugFile.Write(", Data Checksum: " & Sector.DataChecksum.ToString("X4") & If(DataChecksumValid, "", "#"))
            DebugFile.Write(", DAM Mark: " & CByte(Sector.DAM).ToString("X2"))
            If Sector.DAM <> IBM_MFM.MFMAddressMark.Data And Sector.DAM <> IBM_MFM.MFMAddressMark.DeletedData Then
                DebugFile.Write(" (Unexpected)")
            End If
            DebugFile.Write(", Gap2: " & Sector.Gap2.Length)
            DebugFile.Write(", Gap3: " & Sector.Gap3.Length)
            DebugFile.Write(", Overlaps: " & Sector.Overlaps)
            DebugFile.WriteLine(", Offset: " & Sector.Offset \ IBM_MFM.MFM_BYTE_SIZE)
            If Sector.Gap2.Length > 0 Then
                DebugFile.WriteLine("Gap 2:")
                DebugFile.WriteLine(HexFormat(Sector.Gap2))
                DebugFile.WriteLine("")
            End If
            If Sector.Gap3.Length > 0 Then
                DebugFile.WriteLine("Gap 3:")
                DebugFile.WriteLine(HexFormat(Sector.Gap3))
                DebugFile.WriteLine("")
            End If
        Next
        DebugFile.Close()
    End Sub

    Public Sub DebugTranscopyCylinder(Cylinder As Transcopy.TransCopyCylinder)
        Dim DebugPath As String = "H:\Debug\"
        Console.Write("Track: " & Cylinder.Track)
        Console.Write(", Side: " & Cylinder.Side)
        Console.WriteLine(", TrackType: " & Cylinder.TrackType)

        Dim TrackString As String = Cylinder.Track.ToString.PadLeft(2, "0") & "." & Cylinder.Side
        DebugExportMFMTrack(Cylinder.DecodedData, TrackString, DebugPath & "track" & TrackString & ".log")

        Dim Bitstream = MyBitConverter.BytesToBits(Cylinder.Data)

        DebugExportMFMBitstream(Bitstream, DebugPath & "track" & TrackString & "_bitstream" & ".txt")
        IO.File.WriteAllBytes(DebugPath & "track" & TrackString & ".bin", IBM_MFM.DecodeTrack(Cylinder.Data))
    End Sub

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
End Module
