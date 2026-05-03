Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports Greaseweazle.Shared
Imports Greaseweazle.Tools

Namespace Flux.Greaseweazle
    Friend Class ReadDiskHelpers
        Public Const FLUX_WILDCARD As String = "*.raw"

        Friend Shared Function BuildReadOptions(filePath As String,
                                          opt As DriveOption,
                                          diskParams As FloppyDiskParams,
                                          outputType As ReadDiskOutputTypes,
                                          doubleStep As Boolean,
                                          trackRanges As List(Of (StartTrack As UShort, EndTrack As UShort)),
                                          heads As TrackHeads?,
                                          retries As Integer,
                                          seekRetries As Integer,
                                          revs As Integer,
                                          filePath2 As String,
                                          outputType2 As ReadDiskOutputTypes) As ReadOptions

            Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(diskParams.Format)
            Dim Format As String = Nothing
            Dim Raw As Boolean = False
            Dim AdjustSpeed As Double? = Nothing

            If outputType = ReadDiskOutputTypes.IMA OrElse ImageFormat <> GreaseweazleImageFormat.None Then
                Format = GreaseweazleImageFormatString(ImageFormat)
            End If

            Dim FileNameWithOpts As String = filePath
            Dim FileName2WithOpts As String = ""

            If outputType = ReadDiskOutputTypes.HFE Then
                AdjustSpeed = 60.0 / diskParams.RPM
                Raw = True
                FileNameWithOpts &= "::bitrate=" & CInt(diskParams.BitRateKbps)

            ElseIf outputType = ReadDiskOutputTypes.RAW Then
                If diskParams.Format <> FloppyDiskFormat.FloppyUnknown Then
                    AdjustSpeed = 60.0 / diskParams.RPM
                End If
                Raw = True

                If Not String.IsNullOrEmpty(filePath2) AndAlso outputType2 <> ReadDiskOutputTypes.None Then
                    FileName2WithOpts = filePath2

                    If outputType2 = ReadDiskOutputTypes.HFE Then
                        FileName2WithOpts &= "::bitrate=" & CInt(diskParams.BitRateKbps)
                    End If
                End If
            End If

            If trackRanges Is Nothing Then
                trackRanges = New List(Of (StartTrack As UShort, EndTrack As UShort)) From {
                    (0, CUShort(Math.Max(0, opt.Tracks - 1)))
                }
            End If

            If Not heads.HasValue Then
                If diskParams.Format = FloppyDiskFormat.FloppyUnknown Then
                    heads = TrackHeads.both
                Else
                    heads = If(diskParams.BPBParams.NumberOfHeads > 1, TrackHeads.both, TrackHeads.head0)
                End If
            End If

            Dim TrackSet = BuildUserSpec(trackRanges, heads, doubleStep, True)

            Dim Drive = MakeDriveSpec(opt.Id)

            Dim Opts = New ReadOptions With {
                .FileName = FileNameWithOpts,
                .Format = Format,
                .TrackSet = TrackSet,
                .Revs = revs,
                .Raw = Raw,
                .AdjustSpeed = AdjustSpeed,
                .Retries = retries,
                .SeekRetries = seekRetries,
                .Live = True,
                .Device = ConfiguredDevice(),
                .Drive = Drive
            }

            If Not String.IsNullOrEmpty(FileName2WithOpts) Then
                Opts.AdditionalFiles = New List(Of String) From {FileName2WithOpts}
            End If

            Return Opts
        End Function

        Friend Shared Function BuildRefineConvertOptions(inputFilePath As String, diskParams As FloppyDiskParams, doubleStep As Boolean) As ConvertOptions
            Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(diskParams.Format)
            Dim Format = GreaseweazleImageFormatString(ImageFormat)

            Dim TrackSet As New TrackSetSpec

            If doubleStep Then
                TrackSet.Step = 2
            End If

            Return New ConvertOptions With {.InputFile = inputFilePath, .Format = Format, .TrackSet = TrackSet}
        End Function

        Friend Shared Function CheckRawFolderExists(FilePath As String) As (Result As Boolean, Overwritemode As Boolean)
            Dim FolderName = IO.Path.GetDirectoryName(FilePath)

            If GetFirstRawInFolder(FolderName) IsNot Nothing Then
                Dim Msg = String.Format(My.Resources.Dialog_ImageSetExists, FolderName)
                If MsgBox(Msg, MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel + vbDefaultButton2) <> MsgBoxResult.Ok Then
                    Return (False, False)
                End If
                Return (True, True)
            End If

            Return (True, False)
        End Function

        Friend Shared Function FinalizeFluxTempFolder(tempFolderPath As String, destinationFolderPath As String, Optional prefix As String = "") As Boolean
            If String.IsNullOrWhiteSpace(tempFolderPath) OrElse String.IsNullOrWhiteSpace(destinationFolderPath) Then
                Return False
            End If

            Dim tempFull = IO.Path.GetFullPath(tempFolderPath.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar))
            Dim destFull = IO.Path.GetFullPath(destinationFolderPath.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar))

            If Not IO.Directory.Exists(tempFull) Then
                Return False
            End If

            ' No-op if same folder
            If String.Equals(tempFull, destFull, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If

            Try
                ' Fast path: destination does not exist -> rename temp folder to destination
                If Not IO.Directory.Exists(destFull) Then
                    IO.Directory.Move(tempFull, destFull)
                    Return True
                End If

                ' If prefix was not provided, try to infer it from temp set
                If String.IsNullOrWhiteSpace(prefix) Then
                    Dim tempRaw = GetFirstRawInFolder(tempFull)
                    If Not String.IsNullOrWhiteSpace(tempRaw) Then
                        Dim info = GetFluxSetInfoRaw(tempRaw)
                        If info.Result Then
                            prefix = info.Prefix
                        End If
                    End If
                End If

                ' Destination exists -> delete existing raw files with same prefix
                If Not String.IsNullOrWhiteSpace(prefix) Then
                    For Each dstRaw In IO.Directory.GetFiles(destFull, prefix & FLUX_WILDCARD, IO.SearchOption.TopDirectoryOnly)
                        IO.File.Delete(dstRaw)
                    Next
                End If

                ' Move all files from temp to destination (overwrite)
                For Each srcFile In IO.Directory.GetFiles(tempFull, "*", IO.SearchOption.TopDirectoryOnly)
                    Dim dstFile = IO.Path.Combine(destFull, IO.Path.GetFileName(srcFile))

                    Dim created = IO.File.GetCreationTime(srcFile)
                    Dim modified = IO.File.GetLastWriteTime(srcFile)

                    If IO.File.Exists(dstFile) Then
                        IO.File.Delete(dstFile)
                    End If

                    IO.File.Move(srcFile, dstFile)

                    IO.File.SetCreationTime(dstFile, created)
                    IO.File.SetLastWriteTime(dstFile, modified)
                Next

                ' Delete temp folder after move
                If IO.Directory.Exists(tempFull) Then
                    IO.Directory.Delete(tempFull, True)
                End If

                Return True
            Catch ex As Exception
                Debug.WriteLine($"FinalizeFluxTempFolder failed: {ex.Message}")
                Return False
            End Try
        End Function

        Friend Shared Function FluxGetFirstTrackFileName(Prefix As String) As String
            Return FluxGetTrackFileName(Prefix, 0, 0)
        End Function

        Friend Shared Function FluxGetTrackFileName(Prefix As String, Track As Integer, Side As Integer) As String
            Return Prefix & Track.ToString("00") & "." & Side.ToString() & ".raw"
        End Function

        Friend Shared Function GetOutputFolderName(FolderName As String) As String
            If ContainsPlaceholder(FolderName) Then
                FolderName = StripAngleBrackets(FolderName)
            End If

            Return FolderName
        End Function

        Friend Shared Function NormalizeFluxFolder(selectedPath As String, rootPath As String) As String
            If String.IsNullOrWhiteSpace(selectedPath) OrElse String.IsNullOrWhiteSpace(rootPath) Then
                Return ""
            End If

            ' Normalize both paths
            Dim rootFull = IO.Path.GetFullPath(rootPath.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar))
            Dim selFull = IO.Path.GetFullPath(selectedPath.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar))

            ' Same as root → empty
            If String.Equals(rootFull, selFull, StringComparison.OrdinalIgnoreCase) Then
                Return ""
            End If

            ' Subfolder of root → relative path
            If selFull.StartsWith(rootFull & IO.Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) Then
                Return selFull.Substring(rootFull.Length + 1)
            End If

            ' Otherwise → absolute path
            Return selFull
        End Function

        Friend Shared Function SetControlWidth(Control As Control, Optional ExtraPadding As Integer = 2) As Integer
            Dim TextWidth = TextRenderer.MeasureText(Control.Text, Control.Font).Width

            Control.Width = TextWidth

            Return TextWidth + Control.Padding.Horizontal + Control.Margin.Horizontal + ExtraPadding
        End Function

        Friend Shared Function UseDoubleStep(DriveType As FloppyDriveType, Format As FloppyDiskFormat) As Boolean
            Dim ImageParams As FloppyDiskParams = FloppyDiskFormatGetParams(Format)

            Return ImageParams.IsStandard AndAlso ImageParams.DriveType = FloppyDriveType.Drive525DoubleDensity AndAlso DriveType = FloppyDriveType.Drive525HighDensity
        End Function
    End Class
End Namespace
