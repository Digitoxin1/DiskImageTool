Namespace ImageFormats.TD0
    Public Class TD0SectorDataHeader
        Public Const LENGTH As Integer = 3

        Private ReadOnly _header As Byte()

        Private Enum Offset As Integer
            BlockSize = 0
            EncodingMethod = 2
        End Enum

        Public Sub New(BlockSize As Integer, EncodingMethod As TD0EncodingMethod)
            _header = New Byte(LENGTH - 1) {}
            Me.BlockSize = BlockSize
            Me.EncodingMethod = EncodingMethod
        End Sub

        Public Sub New(imagebuf As Byte(), offset As Integer)
            _header = New Byte(LENGTH - 1) {}
            If imagebuf Is Nothing Then
                Return
            End If

            If offset < 0 OrElse (offset + LENGTH) > imagebuf.Length Then
                Return
            End If

            Array.Copy(imagebuf, offset, _header, 0, LENGTH)
        End Sub

        Public Property BlockSize As Integer
            Get
                Return ReadUInt16LE(_header, Offset.BlockSize)
            End Get
            Set(value As Integer)
                WriteUInt16LE(_header, Offset.BlockSize, value)
            End Set
        End Property

        Public Property EncodingMethod As TD0EncodingMethod
            Get
                Return CType(_header(Offset.EncodingMethod), TD0EncodingMethod)
            End Get
            Set(value As TD0EncodingMethod)
                _header(Offset.EncodingMethod) = CByte(value)
            End Set
        End Property

        Public Function GetBytes() As Byte()
            Dim b(LENGTH - 1) As Byte
            Array.Copy(_header, 0, b, 0, LENGTH)
            Return b
        End Function
    End Class
End Namespace
