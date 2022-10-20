Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace JCS
    Public Module GraphicsExtensionMethods
        <Extension()>
        Public Function ToGrayScale(ByVal originalColor As Color) As Color
            If originalColor.Equals(Color.Transparent) Then Return originalColor

            Dim grayScale As Integer = originalColor.R * 0.299 + originalColor.G * 0.587 + originalColor.B * 0.114
            Return Color.FromArgb(grayScale, grayScale, grayScale)
        End Function
    End Module
End Namespace
