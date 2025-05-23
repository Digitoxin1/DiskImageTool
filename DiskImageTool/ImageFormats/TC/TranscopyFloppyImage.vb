Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace TC
        Public Class TranscopyFloppyImage
            Inherits MappedFloppyImage
            Implements IFloppyImage

            Private ReadOnly _Image As TransCopyImage

            Public Sub New(Image As TransCopyImage, DiskFormat As FloppyDiskFormat, BytesPerSector As UInteger)
                MyBase.New(Image, BytesPerSector)

                _Image = Image

                InitDiskFormat(DiskFormat)
            End Sub

            Public ReadOnly Property Image As TransCopyImage
                Get
                    Return _Image
                End Get
            End Property

            Public Overrides ReadOnly Property ImageType As FloppyImageType Implements IFloppyImage.ImageType
                Get
                    Return FloppyImageType.TranscopyImage
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
End Namespace