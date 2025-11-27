Imports System.Threading

Public Class Win9xCleanScanner
    Implements IImageScanner

    Private ReadOnly _currentImage As DiskImageContainer
    Private ReadOnly _images As IList(Of ImageData)
    Private ReadOnly _win9xClean As Action(Of DiskImage.Disk, ImageData)

    Private _itemsRemaining As UInteger

    Public Event ProgressChanged(percent As Integer) Implements IImageScanner.ProgressChanged
    Public Event ScanCompleted(cancelled As Boolean, [error] As Exception) Implements IImageScanner.ScanCompleted

    Public Sub New(Images As IList(Of ImageData), CurrentImage As DiskImageContainer, Win9xClean As Action(Of DiskImage.Disk, ImageData))

        _images = Images
        _currentImage = CurrentImage
        _win9xClean = Win9xClean

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

            Dim processed = 0
            Dim lastPercent = -1

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

                _win9xClean(disk, img)

                processed += 1
                If _itemsRemaining > 0 Then
                    _itemsRemaining -= 1
                End If

                Dim percent = CInt(processed * 100 / total)
                If percent <> lastPercent Then
                    lastPercent = percent
                    RaiseEvent ProgressChanged(percent)
                End If
            Next

            If Not cancelled AndAlso lastPercent < 100 AndAlso _itemsRemaining = 0 Then
                RaiseEvent ProgressChanged(100)
            End If

        Catch ex As Exception
            errorEx = ex
        Finally
            RaiseEvent ScanCompleted(cancelled, errorEx)
        End Try
    End Sub
End Class
