Imports System.Text.RegularExpressions
Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports DiskImageTool.Flux.Kryoflux.CommandLineBuilder

Namespace Flux.Kryoflux
    Module KryofluxLib
        Public Function ConvertFirstTrack(FilePath As String) As (Result As Boolean, FileName As String)
            Dim TempPath = InitTempImagePath()

            If TempPath = "" Then
                Return (False, "")
            End If

            Dim FileName = GenerateUniqueFileName(TempPath, "temp.ima")

            Dim Builder As New CommandLineBuilder() With {
                .InFile = FilePath,
                .OutFile = FileName,
                .DeviceMode = DeviceModeEnum.ImageFile,
                .InImageType = ImageTypeEnum.KryoFlux_Stream,
                .OutImageType = ImageTypeEnum.MFM_SectorImage,
                .LogLevel = LogMask.Format,
                .TrackStart = 0,
                .TrackEnd = 0,
                .OutputTrackStart = 0,
                .OutputTrackEnd = 0,
                .SingleSidedMode = SingleSidedModeEnum.side0
            }

            Dim result = ConsoleProcessRunner.RunProcess(Settings.AppPath, Builder.Arguments, captureOutput:=True, captureError:=False)

            Dim RegExTrackInfo As New Regex(ConsoleParser.REGEX_TRACK, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Multiline)

            Dim Match = RegExTrackInfo.Match(result.StdOut)

            If Match.Success Then
                Dim Status = Match.Groups("status").Value.Trim
                If Status = "unformatted" Then
                    Builder.RPM = 360
                    ConsoleProcessRunner.RunProcess(Settings.AppPath, Builder.Arguments, captureOutput:=False, captureError:=False)
                End If
            End If


            FileName = GetSide0FileName(FileName)

            Return (IO.File.Exists(FileName), FileName)
        End Function

        Public Function GenerateCommandLineImport(InputFilePath As String, OutputFilePath As String, DiskParams As FloppyDiskParams, DoubleStep As Boolean) As (Arguments As String, OutputFilePath As String)
            Dim SingleSidedMode As SingleSidedModeEnum = SingleSidedModeEnum.off

            If DiskParams.Format = FloppyDiskFormat.Floppy160 Or DiskParams.Format = FloppyDiskFormat.Floppy180 Then
                SingleSidedMode = SingleSidedModeEnum.side0
            End If

            Dim TrackCount = DiskParams.BPBParams.TrackCount
            Dim TrackDistance As TrackDistanceEnum = TrackDistanceEnum.Tracks80
            If DoubleStep Then
                TrackCount *= 2
                TrackDistance = TrackDistanceEnum.Tracks40
            End If

            Dim Builder As New CommandLineBuilder() With {
                .InFile = InputFilePath,
                .OutFile = OutputFilePath,
                .DeviceMode = DeviceModeEnum.ImageFile,
                .InImageType = ImageTypeEnum.KryoFlux_Stream,
                .OutImageType = ImageTypeEnum.MFM_SectorImage,
                .LogLevel = LogMask.Format,
                .TrackStart = 0,
                .TrackEnd = TrackCount - 1,
                .OutputTrackStart = 0,
                .OutputTrackEnd = TrackCount - 1,
                .SingleSidedMode = SingleSidedMode,
                .RPM = DiskParams.RPM,
                .TrackDistance = TrackDistance,
                .SectorCount = DiskParams.BPBParams.SectorsPerTrack,
                .MissingSectorsAsBad = False
            }

            If SingleSidedMode = SingleSidedModeEnum.side0 Then
                OutputFilePath = GetSide0FileName(OutputFilePath)
            End If

            Return (Builder.Arguments, OutputFilePath)
        End Function

        Public Function GetSide0FileName(Filename As String) As String
            Dim PathName = IO.Path.GetDirectoryName(Filename)
            Dim BaseName = IO.Path.GetFileNameWithoutExtension(Filename)
            Dim Ext = IO.Path.GetExtension(Filename)

            Return IO.Path.Combine(PathName, BaseName & "_s0" & Ext)
        End Function

        Public Function Settings() As KryofluxSettings
            Return App.Globals.AppSettings.Kryoflux
        End Function
    End Module
End Namespace
