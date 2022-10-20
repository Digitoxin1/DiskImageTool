Imports System
Imports System.Drawing
Imports System.Drawing.Imaging

Namespace ToggleSwitch
    Public Module ImageHelper
        Private ReadOnly _colorMatrixElements As Single()() = {New Single() {0.299, 0.299, 0.299, 0, 0}, New Single() {0.587, 0.587, 0.587, 0, 0}, New Single() {0.114, 0.114, 0.114, 0, 0}, New Single() {0, 0, 0, 1, 0}, New Single() {0, 0, 0, 0, 1}}

        Private ReadOnly _grayscaleColorMatrix = New ColorMatrix(_colorMatrixElements)

        Public Function GetGrayscaleAttributes() As ImageAttributes
            Dim attr = New ImageAttributes()
            attr.SetColorMatrix(_grayscaleColorMatrix, ColorMatrixFlag.[Default], ColorAdjustType.Bitmap)
            Return attr
        End Function

        Public Function RescaleImageToFit(ByVal imageSize As Size, ByVal canvasSize As Size) As Size
            'Code "borrowed" from http://stackoverflow.com/questions/1940581/c-sharp-image-resizing-to-different-size-while-preserving-aspect-ratio
            ' and the Math.Min improvement from http://stackoverflow.com/questions/6501797/resize-image-proportionally-with-maxheight-and-maxwidth-constraints

            ' Figure out the ratio
            Dim ratioX = canvasSize.Width / imageSize.Width
            Dim ratioY = canvasSize.Height / imageSize.Height

            ' use whichever multiplier is smaller
            Dim ratio = Math.Min(ratioX, ratioY)

            ' now we can get the new height and width
            Dim newHeight = Convert.ToInt32(imageSize.Height * ratio)
            Dim newWidth = Convert.ToInt32(imageSize.Width * ratio)

            Dim resizedSize = New Size(newWidth, newHeight)

            If resizedSize.Width > canvasSize.Width OrElse resizedSize.Height > canvasSize.Height Then
                Throw New Exception("ImageHelper.RescaleImageToFit - Resize failed!")
            End If

            Return resizedSize
        End Function
    End Module
End Namespace
