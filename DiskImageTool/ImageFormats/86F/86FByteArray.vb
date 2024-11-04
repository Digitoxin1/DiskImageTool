Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace _86F
        Public Class _86FByteArray
            Inherits Bitstream.BitstreamByteArray
            Implements IByteArray

            Private ReadOnly _Image As _86FImage

            Public Sub New(Image As _86FImage, DiskFormat As FloppyDiskFormat)
                MyBase.New(Image)

                _Image = Image

                InitDiskFormat(DiskFormat)
            End Sub

            Public ReadOnly Property Image As _86FImage
                Get
                    Return _Image
                End Get
            End Property

            Public Overrides ReadOnly Property ImageType As FloppyImageType Implements IByteArray.ImageType
                Get
                    Return FloppyImageType._86FImage
                End Get
            End Property

            Public Overrides Function GetCRC32() As String Implements IByteArray.GetCRC32
                Using Hasher As CRC32Hash = CRC32Hash.Create()
                    Return BitstreamCalculateHash(_Image, Hasher)
                End Using
            End Function

            Public Overrides Function GetMD5Hash() As String Implements IByteArray.GetMD5Hash
                Using Hasher As MD5 = MD5.Create()
                    Return BitstreamCalculateHash(_Image, Hasher)
                End Using
            End Function

            Public Overrides Function GetSHA1Hash() As String Implements IByteArray.GetSHA1Hash
                Using Hasher As SHA1 = SHA1.Create()
                    Return BitstreamCalculateHash(_Image, Hasher)
                End Using
            End Function
        End Class
    End Namespace
End Namespace
