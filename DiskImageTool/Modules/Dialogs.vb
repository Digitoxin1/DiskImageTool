Imports System.IO

Module Dialogs
    Public Function ShowSingleExtSaveDialog(owner As IWin32Window, SourceFilePath As String, InitialDirectory As String, FilterDescription As String, Optional Title As String = "Save As") As String
        Dim ext As String = Path.GetExtension(SourceFilePath)

        If String.IsNullOrEmpty(ext) Then
            ext = ".bin"
        End If

        Dim baseName As String = Path.GetFileName(SourceFilePath)
        Dim filterPattern As String = "*" & ext

        Using dlg As New SaveFileDialog() With {
                .Title = Title,
                .FileName = baseName,
                .Filter = $"{FilterDescription}|{filterPattern}",
                .DefaultExt = ext.Trim("."),
                .AddExtension = True,
                .OverwritePrompt = True,
                .ValidateNames = True,
                .InitialDirectory = InitialDirectory,
                .RestoreDirectory = True
            }

            If dlg.ShowDialog(owner) = DialogResult.OK Then
                Dim result As String = dlg.FileName

                If Not result.EndsWith(ext, StringComparison.OrdinalIgnoreCase) Then
                    result = Path.ChangeExtension(result, ext)
                End If

                Return result
            End If
        End Using

        Return String.Empty
    End Function
End Module
