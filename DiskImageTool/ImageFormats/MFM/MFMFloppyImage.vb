Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats.MFM
    Public Class MFMFloppyImage
        Inherits MappedFloppyImage
        Implements IFloppyImage
        Implements IImageFieldSource

        Private Enum MFMImageField As UShort
            RPM = 1
            BitRate = 2
            InterfaceType = 3
        End Enum

        Private Const ADVANCED_TRACK_LIST As Byte = &H80
        Private Const INTERFACE_MODE_DISABLED As Byte = &HFE

        Private ReadOnly _Image As MFMImage

        Public Sub New(Image As MFMImage, DiskFormat As FloppyDiskFormat, BytesPerSector As UInteger)
            MyBase.New(Image, BytesPerSector)

            _Image = Image

            InitDiskFormat(DiskFormat)
        End Sub

        Public ReadOnly Property Image As MFMImage
            Get
                Return _Image
            End Get
        End Property

        Public Overrides ReadOnly Property ImageType As FloppyImageType Implements IFloppyImage.ImageType
            Get
                Return FloppyImageType.MFMImage
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

        Public Sub SetImageField(IsTrackField As Boolean, Track As UShort, Side As Byte, Sector As UShort, FieldId As UShort, Value As Object) Implements IImageFieldSource.SetImageField
            If IsTrackField Then
                Exit Sub
            End If

            Select Case CType(FieldId, MFMImageField)
                Case MFMImageField.RPM
                    _Image.RPM = CUShort(Value)
                Case MFMImageField.BitRate
                    _Image.BitRate = CUShort(Value)
                Case MFMImageField.InterfaceType
                    _Image.IFType = CByte(Value)
            End Select
        End Sub

        Public Function UpdateHeader(RPM As UShort, BitRate As UShort, InterfaceMode As Byte) As Boolean
            Dim Changes As New List(Of ImageFieldChange)
            Dim InterfaceType = CombineInterfaceType(InterfaceMode)

            If _Image.RPM <> RPM Then
                Changes.Add(New ImageFieldChange(False, 0, 0, 0, MFMImageField.RPM, _Image.RPM, RPM))
            End If

            If _Image.BitRate <> BitRate Then
                Changes.Add(New ImageFieldChange(False, 0, 0, 0, MFMImageField.BitRate, _Image.BitRate, BitRate))
            End If

            If _Image.IFType <> InterfaceType Then
                Changes.Add(New ImageFieldChange(False, 0, 0, 0, MFMImageField.InterfaceType, _Image.IFType, InterfaceType))
            End If

            Return History.CommitImageFields(Changes)
        End Function

        Private Function CombineInterfaceType(InterfaceMode As Byte) As Byte
            If InterfaceMode = INTERFACE_MODE_DISABLED Then
                Return INTERFACE_MODE_DISABLED
            End If

            Return CByte((InterfaceMode And &H7F) Or (_Image.IFType And ADVANCED_TRACK_LIST))
        End Function
    End Class
End Namespace
