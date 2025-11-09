Imports System.Text

Namespace Greaseweazle
    Module GreaseweazleLib
        Public Enum GreaseweazleFloppyType
            None
            F525_DD_360K
            F525_HD_12M
            F35_DD_720K
            F35_HD_144M
            F35_ED_288M
        End Enum

        Public Enum GreaseweazleInterface
            IBM
            Shugart
        End Enum

        Public Sub DisplayGreaseweazleInfo()
            Dim AppPath As String = My.Settings.GW_Path

            If Not IsValidGreaseweazlePath(AppPath) Then
                DisplayInvalidApplicationPathMsg()
                Exit Sub
            End If

            Dim Content = GetGreaseweazleInfo(AppPath)

            Dim frmTextView = New TextViewForm("Greaseweazle - Info", Content, False, True, "GreaseweazleInfo.txt")
            frmTextView.ShowDialog()
        End Sub

        Public Sub DisplayInvalidApplicationPathMsg()
            MessageBox.Show("Application path is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Public Function GetGreaseweazleFloppyTypeDescription(Value As GreaseweazleFloppyType) As String
            Select Case Value
                Case GreaseweazleFloppyType.F525_DD_360K
                    Return "5.25"" 360 KB (Double Density)"
                Case GreaseweazleFloppyType.F35_DD_720K
                    Return "3.5"" 720 KB (Double Density)"
                Case GreaseweazleFloppyType.F525_HD_12M
                    Return "5.25"" 1.2 MB (High Density)"
                Case GreaseweazleFloppyType.F35_HD_144M
                    Return "3.5"" 1.44 MB (High Density)"
                Case GreaseweazleFloppyType.F35_ED_288M
                    Return "3.5"" 2.88 MB (Extra Density)"
                Case Else
                    Return "None"
            End Select
        End Function

        Public Function GetGreaseweazleFloppyTypeName(Value As GreaseweazleFloppyType) As String
            Select Case Value
                Case GreaseweazleFloppyType.F525_DD_360K
                    Return "360"
                Case GreaseweazleFloppyType.F35_DD_720K
                    Return "720"
                Case GreaseweazleFloppyType.F525_HD_12M
                    Return "1200"
                Case GreaseweazleFloppyType.F35_HD_144M
                    Return "1440"
                Case GreaseweazleFloppyType.F35_ED_288M
                    Return "2880"
                Case Else
                    Return ""
            End Select
        End Function

        Public Function GetGreaseweazleFoppyTypeFromName(Value As String) As GreaseweazleFloppyType
            Select Case Value
                Case "360"
                    Return GreaseweazleFloppyType.F525_DD_360K
                Case "720"
                    Return GreaseweazleFloppyType.F35_DD_720K
                Case "1200"
                    Return GreaseweazleFloppyType.F525_HD_12M
                Case "1440"
                    Return GreaseweazleFloppyType.F35_HD_144M
                Case "2880"
                    Return GreaseweazleFloppyType.F35_ED_288M
                Case Else
                    Return GreaseweazleFloppyType.None
            End Select
        End Function

        Public Function GetGreaseweazleInfo(AppPath As String) As String
            Dim psi As New ProcessStartInfo() With {
                .FileName = AppPath,
                .Arguments = "info",
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .UseShellExecute = False,
                .CreateNoWindow = True
            }

            Dim outputBuilder As New StringBuilder()

            Using proc As New Process()
                proc.StartInfo = psi
                AddHandler proc.OutputDataReceived, Sub(sender, e)
                                                        If e.Data IsNot Nothing Then
                                                            SyncLock outputBuilder
                                                                outputBuilder.AppendLine(e.Data)
                                                            End SyncLock
                                                        End If
                                                    End Sub
                AddHandler proc.ErrorDataReceived, Sub(sender, e)
                                                       If e.Data IsNot Nothing Then
                                                           SyncLock outputBuilder
                                                               outputBuilder.AppendLine(e.Data)
                                                           End SyncLock
                                                       End If
                                                   End Sub

                proc.Start()
                proc.BeginOutputReadLine()
                proc.BeginErrorReadLine()
                proc.WaitForExit()
            End Using

            Return outputBuilder.ToString
        End Function
        Public Function GetGreaseweazleInterfaceName(Value As GreaseweazleInterface) As String
            Select Case Value
                Case GreaseweazleInterface.Shugart
                    Return "Shugart"
                Case Else
                    Return "IBM"
            End Select
        End Function

        Public Function GetGreaseweazleInterfacFromName(Value As String) As GreaseweazleInterface
            Select Case Value
                Case "Shugart"
                    Return GreaseweazleInterface.Shugart
                Case Else
                    Return GreaseweazleInterface.IBM
            End Select
        End Function

        Public Function IsExecutable(path As String) As Boolean
            Return Not String.IsNullOrWhiteSpace(path) AndAlso
           IO.File.Exists(path) AndAlso
           IO.Path.GetExtension(path).Equals(".exe", StringComparison.OrdinalIgnoreCase)
        End Function

        Public Function IsValidGreaseweazlePath(Path As String) As Boolean
            Dim IsValid As Boolean

            If My.Settings.GW_Path = "" Then
                IsValid = False
            ElseIf Not IsExecutable(My.Settings.GW_Path) Then
                IsValid = False
            ElseIf Not IO.File.Exists(My.Settings.GW_Path) Then
                IsValid = False
            Else
                IsValid = True
            End If

            Return IsValid
        End Function
    End Module
End Namespace