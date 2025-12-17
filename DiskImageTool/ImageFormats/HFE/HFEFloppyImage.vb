Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats.HFE
    Public Class HFEFloppyImage
        Inherits MappedFloppyImage
        Implements IFloppyImage

        Private ReadOnly _Image As HFEImage

        Public Sub New(Image As HFEImage, DiskFormat As FloppyDiskFormat, BytesPerSector As UInteger)
            MyBase.New(Image, BytesPerSector)

            _Image = Image

            InitDiskFormat(DiskFormat)
        End Sub

        Public ReadOnly Property Image As HFEImage
            Get
                Return _Image
            End Get
        End Property

        Public Overrides ReadOnly Property ImageType As FloppyImageType Implements IFloppyImage.ImageType
            Get
                Return FloppyImageType.HFEImage
            End Get
        End Property

        Public Overrides Function GetCRC32() As String Implements IFloppyImage.GetCRC32
            Using Hasher As CRC32Hash = CRC32Hash.Create()
                Return BitstreamCalculateHash(_Image, Hasher)
            End Using
        End Function

        Public Overrides Function GetMD5Hash() As String Implements IFloppyImage.GetMD5Hash
            Using Hasher As MD5 = MD5.Create()
                Return BitstreamCalculateHash(_Image, Hasher)
            End Using
        End Function

        Public Overrides Function GetSHA1Hash() As String Implements IFloppyImage.GetSHA1Hash
            Using Hasher As SHA1 = SHA1.Create()
                Return BitstreamCalculateHash(_Image, Hasher)
            End Using
        End Function
    End Class
End Namespace


