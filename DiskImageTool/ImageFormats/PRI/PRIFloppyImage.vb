﻿Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace PRI
        Public Class PRIFloppyImage
            Inherits MappedFloppyImage
            Implements IFloppyImage

            Private ReadOnly _Image As PRIImage

            Public Sub New(Image As PRIImage, DiskFormat As FloppyDiskFormat, BytesPerSector As UInteger)
                MyBase.New(Image, BytesPerSector)

                _Image = Image

                InitDiskFormat(DiskFormat)
            End Sub

            Public Overrides ReadOnly Property HasWeakBits As Boolean Implements IFloppyImage.HasWeakBits
                Get
                    Return _Image.HasWeakBits
                End Get
            End Property

            Public Overrides ReadOnly Property HasWeakBitsSupport As Boolean Implements IFloppyImage.HasWeakBitsSupport
                Get
                    Return True
                End Get
            End Property

            Public ReadOnly Property Image As PRIImage
                Get
                    Return _Image
                End Get
            End Property

            Public Overrides ReadOnly Property ImageType As FloppyImageType Implements IFloppyImage.ImageType
                Get
                    Return FloppyImageType.PRIImage
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
