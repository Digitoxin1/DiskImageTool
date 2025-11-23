Namespace Flux.PcImgCnv
    Module PcImgCnvLib
        Public Function Settings() As PcImgCnvSettings
            Return App.Globals.AppSettings.PcImgCnv
        End Function

        Public Function GenerateCommandLineImport(InputFilePath As String, OutputFilePath As String) As String

            Dim Builder As New CommandLineBuilder() With {
                .InFile = InputFilePath,
                .OutFile = OutputFilePath
            }

            Return Builder.Arguments
        End Function
    End Module
End Namespace
