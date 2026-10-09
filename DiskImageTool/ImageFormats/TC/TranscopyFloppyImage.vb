Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats.TC
    Public Class TranscopyFloppyImage
        Inherits MappedFloppyImage
        Implements IFloppyImage
        Implements IImageFieldSource

        Private Enum TransCopyImageField As UShort
            Comment = 1
            Comment2 = 2
            DiskType = 3
        End Enum

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

        Public Sub SetImageField(IsTrackField As Boolean, Track As UShort, Side As Byte, Sector As UShort, FieldId As UShort, Value As Object) Implements IImageFieldSource.SetImageField
            If IsTrackField Then
                Exit Sub
            End If

            Select Case CType(FieldId, TransCopyImageField)
                Case TransCopyImageField.Comment
                    _Image.Comment = If(Value Is Nothing, "", CStr(Value))
                Case TransCopyImageField.Comment2
                    _Image.Comment2 = If(Value Is Nothing, "", CStr(Value))
                Case TransCopyImageField.DiskType
                    _Image.DiskType = CType(Value, TransCopyDiskType)
            End Select
        End Sub

        Public Function UpdateHeader(Comment As String, Comment2 As String, DiskType As TransCopyDiskType) As Boolean
            Dim Changes As New List(Of ImageFieldChange)

            If _Image.Comment <> Comment Then
                Changes.Add(New ImageFieldChange(False, 0, 0, 0, TransCopyImageField.Comment, _Image.Comment, Comment))
            End If

            If _Image.Comment2 <> Comment2 Then
                Changes.Add(New ImageFieldChange(False, 0, 0, 0, TransCopyImageField.Comment2, _Image.Comment2, Comment2))
            End If

            If _Image.DiskType <> DiskType Then
                Changes.Add(New ImageFieldChange(False, 0, 0, 0, TransCopyImageField.DiskType, _Image.DiskType, DiskType))
            End If

            Return History.CommitImageFields(Changes)
        End Function
    End Class
End Namespace