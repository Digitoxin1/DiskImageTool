Imports System.Threading

Public Class FilterScanner
    Implements IImageScanner

    Private ReadOnly _currentImage As DiskImageContainer
    Private ReadOnly _filterScan As Action(Of DiskImage.Disk, ImageData)
    Private ReadOnly _images As IList(Of ImageData)
    Private ReadOnly _newOnly As Boolean
    Private _itemsRemaining As UInteger

    Public Event ProgressChanged(Percent As Double) Implements IImageScanner.ProgressChanged
    Public Event ScanCompleted(Cancelled As Boolean, [Error] As Exception) Implements IImageScanner.ScanCompleted

    Public Sub New(Images As IList(Of ImageData), CurrentImage As DiskImageContainer, NewOnly As Boolean, FilterScan As Action(Of DiskImage.Disk, ImageData))
        _images = Images
        _currentImage = CurrentImage
        _newOnly = NewOnly
        _filterScan = FilterScan

        _itemsRemaining = CUInt(_images.Count)

        If _newOnly Then
            For Each img In _images
                If img.Scanned Then
                    _itemsRemaining -= 1
                End If
            Next
        End If
    End Sub

    Public ReadOnly Property ItemsRemaining As UInteger Implements IImageScanner.ItemsRemaining
        Get
            Return _itemsRemaining
        End Get
    End Property

    Public Sub Scan(ct As CancellationToken) Implements IImageScanner.Scan
        Dim ErrorEx As Exception = Nothing
        Dim Cancelled As Boolean = False

        Try
            Dim Total As Integer = CInt(_itemsRemaining)
            If Total <= 0 Then
                RaiseEvent ProgressChanged(100)
                Exit Try
            End If

            Dim Processed As Integer = 0
            Dim lastTenth As Integer = -1

            For Each img In _images
                If ct.IsCancellationRequested Then
                    Cancelled = True
                    Exit For
                End If

                If _newOnly AndAlso img.Scanned Then
                    Continue For
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

                _filterScan?.Invoke(disk, img)
                img.Scanned = True

                Processed += 1
                If _itemsRemaining > 0 Then
                    _itemsRemaining -= 1
                End If

                Dim tenth As Integer = CInt(Math.Round((Processed * 1000.0R) / Total, MidpointRounding.AwayFromZero))
                If tenth <> lastTenth Then
                    lastTenth = tenth
                    RaiseEvent ProgressChanged(tenth / 10.0R)
                End If
            Next

            If Not Cancelled AndAlso lastTenth < 1000 AndAlso _itemsRemaining = 0 Then
                RaiseEvent ProgressChanged(100)
            End If

        Catch ex As Exception
            ErrorEx = ex
        Finally
            RaiseEvent ScanCompleted(Cancelled, ErrorEx)
        End Try
    End Sub
End Class
