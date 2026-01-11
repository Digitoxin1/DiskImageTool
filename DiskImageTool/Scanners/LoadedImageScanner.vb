Imports System.Threading

Public Class LoadedImageScanner
    Implements IImageScanner

    Private ReadOnly _currentImage As DiskImageContainer
    Private ReadOnly _images As IList(Of ImageData)
    Private ReadOnly _callback As Action(Of DiskImage.Disk, ImageData)

    Private _itemsRemaining As UInteger

    Public Event ProgressChanged(percent As Double) Implements IImageScanner.ProgressChanged
    Public Event ScanCompleted(cancelled As Boolean, [error] As Exception) Implements IImageScanner.ScanCompleted

    Public Sub New(Images As IList(Of ImageData), CurrentImage As DiskImageContainer, Callback As Action(Of DiskImage.Disk, ImageData))

        _images = Images
        _currentImage = CurrentImage
        _callback = Callback

        _itemsRemaining = CUInt(_images.Count)
    End Sub

    Public ReadOnly Property ItemsRemaining As UInteger Implements IImageScanner.ItemsRemaining
        Get
            Return _itemsRemaining
        End Get
    End Property

    Public Sub Scan(ct As CancellationToken) Implements IImageScanner.Scan

        Dim errorEx As Exception = Nothing
        Dim cancelled = False

        Try
            Dim total As Integer = CInt(_itemsRemaining)

            If total <= 0 Then
                RaiseEvent ProgressChanged(100)
                Exit Try
            End If

            Dim processed As Integer = 0
            Dim LastTenth As Integer = -1

            For Each img In _images
                If ct.IsCancellationRequested Then
                    cancelled = True
                    Exit For
                End If

                Dim disk As DiskImage.Disk
                If img Is _currentImage.ImageData Then
                    disk = _currentImage.Disk
                Else
                    disk = DiskImageLoadFromImageData(img, True)
                End If

                If disk Is Nothing Then
                    Continue For
                End If

                _callback(disk, img)

                processed += 1
                If _itemsRemaining > 0 Then
                    _itemsRemaining -= 1
                End If

                Dim tenth As Integer = CInt(Math.Round((processed * 1000.0R) / total, MidpointRounding.AwayFromZero))

                If tenth <> LastTenth Then
                    LastTenth = tenth
                    RaiseEvent ProgressChanged(tenth / 10.0R)
                End If
            Next

            If Not cancelled AndAlso LastTenth < 1000 AndAlso _itemsRemaining = 0 Then
                RaiseEvent ProgressChanged(100)
            End If

        Catch ex As Exception
            errorEx = ex
        Finally
            RaiseEvent ScanCompleted(cancelled, errorEx)
        End Try
    End Sub
End Class
