Imports System.Text.RegularExpressions
Imports DiskImageTool.Flux.Kryoflux.CommandLineBuilder

Namespace Flux.Kryoflux
    Module KryofluxLib
        Public Function Settings() As KryofluxSettings
            Return App.Globals.AppSettings.Kryoflux
        End Function

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

        Public Function GetSide0FileName(Filename As String) As String
            Dim PathName = IO.Path.GetDirectoryName(Filename)
            Dim BaseName = IO.Path.GetFileNameWithoutExtension(Filename)
            Dim Ext = IO.Path.GetExtension(Filename)

            Return IO.Path.Combine(PathName, BaseName & "_s0" & Ext)
        End Function

        Public Function ImportFluxImage(FilePath As String, ParentForm As MainForm) As (Result As Boolean, OutputFile As String, NewFileName As String)
            Dim AnalyzeResponse = AnalyzeFluxImage(FilePath, True)

            If Not AnalyzeResponse.Result Then
                Return (False, "", "")
            End If

            Using form As New ImageImportForm(FilePath, AnalyzeResponse.TrackCount, AnalyzeResponse.SideCount)

                Dim handler As ImageImportForm.ImportRequestedEventHandler =
                    Sub(File, NewName)
                        ParentForm.ProcessImportedImage(File, NewName)
                    End Sub

                AddHandler form.ImportRequested, handler

                Dim result As DialogResult = DialogResult.Cancel
                Try
                    result = form.ShowDialog(ParentForm)
                Finally
                    RemoveHandler form.ImportRequested, handler
                End Try

                If result Then
                    If Not String.IsNullOrEmpty(form.OutputFilePath) Then
                        Return (True, form.OutputFilePath, form.GetNewFileName)
                    End If
                End If

                Return (False, "", "")
            End Using
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
