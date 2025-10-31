Public Class DiskImageContainer
    Public Sub New(Disk As DiskImage.Disk, ImageData As ImageData)
        _Disk = Disk
        _ImageData = ImageData
    End Sub

    Public ReadOnly Property Disk As DiskImage.Disk
    Public ReadOnly Property ImageData As ImageData
End Class