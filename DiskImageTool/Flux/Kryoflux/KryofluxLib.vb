Imports System.Text.RegularExpressions
Imports DiskImageTool.Flux.Kryoflux.CommandLineBuilder

Namespace Flux.Kryoflux
    Module KryofluxLib
        Public KryofluxSettings As New Settings

        Public Function ConvertFirstTrack(FilePath As String) As (Result As Boolean, FileName As String)
            Dim TempPath = InitTempImagePath()

            If TempPath = "" Then
                Return (False, "")
            End If

            Dim FileName = GenerateUniqueFileName(TempPath, "temp.ima")

            Dim Builder = New CommandLineBuilder() With {
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

            Dim result = ConsoleProcessRunner.RunProcess(KryofluxSettings.AppPath, Builder.Arguments, captureOutput:=True, captureError:=False)

            Dim RegExTrackInfo = New Regex(ConsoleParser.REGEX_TRACK_STATUS, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Multiline)

            Dim Match = RegExTrackInfo.Match(result.StdOut)

            If Match.Success Then
                Dim Status = Match.Groups("status").Value.Trim
                If Status = "unformatted" Then
                    Builder.RPM = 360
                    ConsoleProcessRunner.RunProcess(KryofluxSettings.AppPath, Builder.Arguments, captureOutput:=False, captureError:=False)
                End If
            End If


            FileName = GetSide0FileName(FileName)

            Return (IO.File.Exists(FileName), FileName)
        End Function

        Public Function GetSide0FileName(Filename As String) As String
            Dim PathName = IO.Path.GetDirectoryName(Filename)
            Dim BaseName = IO.Path.GetFileNameWithoutExtension(Filename)
            Dim Ext = IO.Path.GetExtension(Filename)

            Return IO.Path.Combine(PathName, BaseName & "_s0" & Ext)
        End Function

        Public Function ImportFluxImage(FilePath As String, ParentForm As Form) As (Result As Boolean, OutputFile As String, NewFileName As String)
            Dim FileExt = IO.Path.GetExtension(FilePath).ToLower
            Dim TrackCount As Integer = 0
            Dim SideCount As Integer = 0

            If FileExt = ".raw" Then
                Dim Response = GetTrackCountRaw(FilePath)
                If Not Response.Result Then
                    MsgBox(My.Resources.Dialog_InvalidKryofluxFile, MsgBoxStyle.Exclamation)
                    Return (False, "", "")
                Else
                    TrackCount = Response.Tracks
                    SideCount = Response.Sides
                End If
            Else
                MsgBox(My.Resources.Dialog_InvalidFileType, MsgBoxStyle.Exclamation)
                Return (False, "", "")
            End If

            If SideCount > 2 Then
                SideCount = 2
            End If

            If TrackCount > 42 And TrackCount < 80 Then
                TrackCount = 80
            End If

            Dim Form As New ImageImportForm(FilePath, TrackCount, SideCount)
            If Form.ShowDialog(ParentForm) = DialogResult.OK Then
                If Not String.IsNullOrEmpty(Form.OutputFilePath) Then
                    Return (True, Form.OutputFilePath, Form.GetNewFileName)
                End If
            End If

            Return (False, "", "")
        End Function

        Public Function OpenFluxImage(ParentForm As Form) As String
            Using Dialog As New OpenFileDialog With {
                .Title = "Open Flux Image",
                .Filter = "KryoFlux RAW (*.raw)|*.raw",
                .FilterIndex = 1,
                .CheckFileExists = True,
                .AddExtension = True,
                .Multiselect = False
            }

                If Dialog.ShowDialog(ParentForm) = DialogResult.OK Then
                    Return Dialog.FileName
                End If
            End Using

            Return Nothing
        End Function
    End Module
End Namespace
