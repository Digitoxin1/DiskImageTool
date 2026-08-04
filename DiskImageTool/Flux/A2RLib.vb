Imports System.IO

Namespace Flux
    Module A2RLib
        Public Function GetFluxSetInfoA2R(FilePath As String) As FluxSetInfo
            Dim Response As New FluxSetInfo(False, 0, 0, "")

            If String.IsNullOrWhiteSpace(FilePath) OrElse Not File.Exists(FilePath) OrElse Not FilePath.EndsWith(".a2r", StringComparison.OrdinalIgnoreCase) Then
                Return Response
            End If

            Try
                Using fs As New FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                    Using br As New BinaryReader(fs, System.Text.Encoding.ASCII, leaveOpen:=False)
                        If fs.Length < 8 Then
                            Return Response
                        End If

                        Dim header As Byte() = br.ReadBytes(8)

                        If header.Length <> 8 Then
                            Return Response
                        End If

                        Dim signature As String = Text.Encoding.ASCII.GetString(header, 0, 4)

                        ' A2R2 is a valid A2R format, but is not supported.
                        If signature = "A2R2" Then
                            Response.Result = False
                            Response.Unsupported = True

                            Return Response
                        End If

                        If signature <> "A2R3" Then
                            Return Response
                        End If

                        ' Validate the complete A2R header.
                        If header(4) <> &HFF OrElse header(5) <> &HA OrElse header(6) <> &HD OrElse header(7) <> &HA Then
                            Return Response
                        End If

                        Dim driveType As Integer = 0
                        Dim locations As New HashSet(Of Integer)

                        While fs.Position <= fs.Length - 8
                            Dim chunkIdBytes As Byte() = br.ReadBytes(4)

                            If chunkIdBytes.Length <> 4 Then
                                Return Response
                            End If

                            Dim chunkId As String = Text.Encoding.ASCII.GetString(chunkIdBytes)

                            Dim chunkSize As UInteger = br.ReadUInt32()
                            Dim chunkStart As Long = fs.Position
                            Dim chunkEnd As Long = chunkStart + CLng(chunkSize)

                            If chunkEnd < chunkStart OrElse chunkEnd > fs.Length Then
                                Return Response
                            End If

                            Select Case chunkId
                                Case "INFO"
                                    If signature = "A2R2" Then
                                        ' INFO v1:
                                        ' +0  INFO version
                                        ' +1  Creator, 32 bytes
                                        ' +33 Disk type
                                        If chunkSize >= 34UI Then
                                            fs.Position = chunkStart + 33
                                            driveType = br.ReadByte()
                                        End If
                                    Else
                                        ' INFO v1:
                                        ' +0  INFO version
                                        ' +1  Creator, 32 bytes
                                        ' +33 Drive type
                                        If chunkSize >= 34UI Then
                                            fs.Position = chunkStart + 33
                                            driveType = br.ReadByte()
                                        End If
                                    End If

                                Case "STRM"
                                    If signature = "A2R2" Then
                                        If Not ReadA2R2Locations(br, chunkStart, chunkEnd, locations) Then
                                            Return Response
                                        End If
                                    End If

                                Case "RWCP"
                                    If signature = "A2R3" Then
                                        If Not ReadA2R3RawCaptureLocations(br, chunkStart, chunkEnd, locations) Then
                                            Return Response
                                        End If
                                    End If

                                Case "SLVD"
                                    If signature = "A2R3" Then
                                        If Not ReadA2R3SolvedLocations(br, chunkStart, chunkEnd, locations) Then
                                            Return Response
                                        End If
                                    End If
                            End Select

                            fs.Position = chunkEnd
                        End While

                        If driveType = 0 OrElse locations.Count = 0 Then
                            Return Response
                        End If

                        Dim maxTrack As Integer = -1
                        Dim sides As New HashSet(Of Integer)

                        For Each location As Integer In locations
                            Dim track As Integer
                            Dim side As Integer

                            If IsA2RQuarterTrackDrive(signature, driveType) Then
                                ' Apple II 5.25-inch captures use quarter-track
                                ' locations: track 0 = 0, track 1 = 4, etc.
                                track = location \ 4
                                side = 0
                            Else
                                ' Other drive types use:
                                ' location = (track << 1) + side
                                track = location \ 2
                                side = location And 1
                            End If

                            If track > maxTrack Then
                                maxTrack = track
                            End If

                            sides.Add(side)
                        Next

                        If maxTrack < 0 OrElse sides.Count = 0 Then
                            Return Response
                        End If

                        Response.Result = True
                        Response.TrackCount = maxTrack + 1
                        Response.SideCount = sides.Count

                        Return Response
                    End Using
                End Using

            Catch ex As IOException
                Return Response
            Catch ex As UnauthorizedAccessException
                Return Response
            Catch ex As ArgumentException
                Return Response
            Catch ex As OverflowException
                Return Response
            End Try
        End Function

        Private Function IsA2RQuarterTrackDrive(signature As String, driveType As Integer) As Boolean
            If signature = "A2R2" Then
                ' A2R2 disk type 1 = 5.25-inch.
                Return driveType = 1
            End If

            ' A2R3 drive type 1 = 5.25-inch single-sided, 40-track, quarter-step.
            Return driveType = 1
        End Function

        Private Function ReadA2R2Locations(br As IO.BinaryReader, chunkStart As Long, chunkEnd As Long, locations As HashSet(Of Integer)) As Boolean
            Dim fs As IO.Stream = br.BaseStream
            fs.Position = chunkStart

            While fs.Position < chunkEnd
                Dim location As Integer = br.ReadByte()

                ' A2R2 STRM terminator.
                If location = &HFF Then
                    Return True
                End If

                ' Capture header after Location:
                ' +1 Capture Type, 1 byte
                ' +2 Data Length, 4 bytes
                ' +6 Estimated Loop Point, 4 bytes
                If chunkEnd - fs.Position < 9 Then
                    Return False
                End If

                Dim captureType As Integer = br.ReadByte()
                Dim dataLength As UInteger = br.ReadUInt32()
                Dim estimatedLoopPoint As UInteger = br.ReadUInt32()

                Dim dataEnd As Long = fs.Position + CLng(dataLength)

                If dataEnd < fs.Position OrElse dataEnd > chunkEnd Then
                    Return False
                End If

                locations.Add(location)
                fs.Position = dataEnd
            End While

            ' A valid STRM chunk should contain its 0xFF terminator.
            Return False
        End Function


        Private Function ReadA2R3RawCaptureLocations(br As IO.BinaryReader, chunkStart As Long, chunkEnd As Long, locations As HashSet(Of Integer)) As Boolean
            Dim fs As IO.Stream = br.BaseStream

            ' RWCP has a 16-byte chunk header before capture entries.
            If chunkEnd - chunkStart < 16 Then
                Return False
            End If

            fs.Position = chunkStart + 16

            While fs.Position < chunkEnd
                Dim mark As Integer = br.ReadByte()

                ' "X" marks the end of capture entries.
                If mark = AscW("X"c) Then
                    Return True
                End If

                If mark <> AscW("C"c) Then
                    Return False
                End If

                ' Capture header:
                ' +1 Capture Type, 1 byte
                ' +2 Location, 2 bytes
                ' +4 Index Count, 1 byte
                If chunkEnd - fs.Position < 4 Then
                    Return False
                End If

                Dim captureType As Integer = br.ReadByte()
                Dim location As Integer = br.ReadUInt16()
                Dim indexCount As Integer = br.ReadByte()

                Dim indexBytes As Long = CLng(indexCount) * 4L

                ' Index array followed by uint32 capture-data size.
                If indexBytes > chunkEnd - fs.Position - 4L Then
                    Return False
                End If

                fs.Position += indexBytes

                Dim dataLength As UInteger = br.ReadUInt32()
                Dim dataEnd As Long = fs.Position + CLng(dataLength)

                If dataEnd < fs.Position OrElse dataEnd > chunkEnd Then
                    Return False
                End If

                locations.Add(location)
                fs.Position = dataEnd
            End While

            Return False
        End Function
        Private Function ReadA2R3SolvedLocations(br As IO.BinaryReader, chunkStart As Long, chunkEnd As Long, locations As HashSet(Of Integer)) As Boolean
            Dim fs As IO.Stream = br.BaseStream

            ' SLVD has a 16-byte chunk header before track entries.
            If chunkEnd - chunkStart < 16 Then
                Return False
            End If

            fs.Position = chunkStart + 16

            While fs.Position < chunkEnd
                Dim mark As Integer = br.ReadByte()

                ' "X" marks the end of track entries.
                If mark = AscW("X"c) Then
                    Return True
                End If

                If mark <> AscW("T"c) Then
                    Return False
                End If

                ' Bytes remaining before the index array:
                ' Location                  2
                ' Mirror Distance Outward   1
                ' Mirror Distance Inward    1
                ' Reserved                  6
                ' Number of Index Signals   1
                If chunkEnd - fs.Position < 11 Then
                    Return False
                End If

                Dim location As Integer = br.ReadUInt16()

                ' Mirror distances.
                br.ReadByte()
                br.ReadByte()

                ' Reserved bytes.
                fs.Position += 6

                Dim indexCount As Integer = br.ReadByte()
                Dim indexBytes As Long = CLng(indexCount) * 4L

                ' Index array followed by uint32 flux-data size.
                If indexBytes > chunkEnd - fs.Position - 4L Then
                    Return False
                End If

                fs.Position += indexBytes

                Dim dataLength As UInteger = br.ReadUInt32()
                Dim dataEnd As Long = fs.Position + CLng(dataLength)

                If dataEnd < fs.Position OrElse dataEnd > chunkEnd Then
                    Return False
                End If

                locations.Add(location)
                fs.Position = dataEnd
            End While

            Return False
        End Function
    End Module
End Namespace
