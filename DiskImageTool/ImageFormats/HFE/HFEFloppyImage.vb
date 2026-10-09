Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats.HFE
    Public Class HFEFloppyImage
        Inherits MappedFloppyImage
        Implements IFloppyImage
        Implements IImageFieldSource

        Private Enum HFEImageField As UShort
            RPM = 1
            BitRate = 2
            InterfaceType = 3
            WriteAllowed = 4
        End Enum

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

        Public Sub SetImageField(IsTrackField As Boolean, Track As UShort, Side As Byte, Sector As UShort, FieldId As UShort, Value As Object) Implements IImageFieldSource.SetImageField
            If IsTrackField Then
                Exit Sub
            End If

            Select Case CType(FieldId, HFEImageField)
                Case HFEImageField.RPM
                    _Image.RPM = CUShort(Value)
                Case HFEImageField.BitRate
                    _Image.BitRate = CUShort(Value)
                Case HFEImageField.InterfaceType
                    _Image.FloppyInterfaceMode = CType(Value, HFEFloppyinterfaceMode)
                Case HFEImageField.WriteAllowed
                    _Image.WriteAllowed = CByte(Value)
            End Select
        End Sub

        Public Function UpdateHeader(RPM As UShort, BitRate As UShort, InterfaceMode As Byte, WriteAllowed As Byte) As Boolean
            Dim Changes As New List(Of ImageFieldChange)
            Dim InterfaceType = CType(InterfaceMode, HFEFloppyinterfaceMode)

            If _Image.RPM <> RPM Then
                Changes.Add(New ImageFieldChange(False, 0, 0, 0, HFEImageField.RPM, _Image.RPM, RPM))
            End If

            If _Image.BitRate <> BitRate Then
                Changes.Add(New ImageFieldChange(False, 0, 0, 0, HFEImageField.BitRate, _Image.BitRate, BitRate))
            End If

            If CByte(_Image.FloppyInterfaceMode) <> InterfaceMode Then
                Changes.Add(New ImageFieldChange(False, 0, 0, 0, HFEImageField.InterfaceType, _Image.FloppyInterfaceMode, InterfaceType))
            End If

            If _Image.WriteAllowed <> WriteAllowed Then
                Changes.Add(New ImageFieldChange(False, 0, 0, 0, HFEImageField.WriteAllowed, _Image.WriteAllowed, WriteAllowed))
            End If

            Return History.CommitImageFields(Changes)
        End Function
    End Class
End Namespace


