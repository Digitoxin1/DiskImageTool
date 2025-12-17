Namespace ImageFormats.TD0
    Public Structure TD0SectorDataHeader
        Public Const LENGTH As Integer = 3

        Private ReadOnly _header As Byte()

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

        Public ReadOnly Property BlockSize As Integer
            Get
                Return ReadUInt16LE(_header, 0)
            End Get
        End Property

        Public ReadOnly Property EncodingMethod As TD0EncodingMethod
            Get
                Return CType(_header(2), TD0EncodingMethod)
            End Get
        End Property

        Public Function GetBytes() As Byte()
            Dim b(LENGTH - 1) As Byte
            Array.Copy(_header, 0, b, 0, LENGTH)
            Return b
        End Function
    End Structure
End Namespace
