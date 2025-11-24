Namespace Flux.PcImgCnv
    Module PcImgCnvLib
        Public Function GenerateCommandLineImport(InputFilePath As String, OutputFilePath As String, F86SurfaceData As Boolean, Remaster As Boolean) As String

            Dim Builder As New CommandLineBuilder() With {
                .InFile = InputFilePath,
                .OutFile = OutputFilePath,
                .F86SurfaceData = F86SurfaceData,
                .Remaster = Remaster
            }

            Return Builder.Arguments
        End Function

        Public Function Settings() As PcImgCnvSettings
            Return App.Globals.AppSettings.PcImgCnv
        End Function
    End Module
End Namespace
