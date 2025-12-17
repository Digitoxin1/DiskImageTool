Namespace ImageFormats.TD0
    Public Class TD0Sector
        Private ReadOnly _header As TD0SectorHeader
        Private _Data As Byte() = Nothing
        Private _DataHeader As TD0SectorDataHeader = Nothing

        Public Sub New(Header As TD0SectorHeader)
            _header = Header
        End Sub

        Public ReadOnly Property ChecksumError As Boolean
            Get
                Return _header.HasCrcError
            End Get
        End Property

        Public ReadOnly Property Cylinder As Byte
            Get
                Return _header.Cylinder
            End Get
        End Property

        Public ReadOnly Property Data As Byte()
            Get
                Return _Data
            End Get
        End Property

        Public ReadOnly Property Deleted As Boolean
            Get
                Return _header.IsDeletedDataMark
            End Get
        End Property

        Public ReadOnly Property Flags As TD0SectorFlags
            Get
                Return _header.Flags
            End Get
        End Property

        Public ReadOnly Property Head As Byte
            Get
                Return _header.Head
            End Get
        End Property

        Public ReadOnly Property SectorId As Byte
            Get
                Return _header.SectorId
            End Get
        End Property

        Public ReadOnly Property SizeCode As Byte
            Get
                Return _header.SizeCode
            End Get
        End Property

        Public ReadOnly Property Unavailable As Boolean
            Get
                Return (Not _header.HasDataBlock OrElse Data Is Nothing)
            End Get
        End Property

        Public Function CrcValid() As Boolean
            Return _header.CrcValid(_DataHeader?.GetBytes(), _Data)
        End Function

        Public Function GetSizeBytes() As Integer
            Return _header.GetSectorSizeBytes
        End Function

        Public Sub SetData(DataHeader As TD0SectorDataHeader, Data As Byte())
            _DataHeader = DataHeader
            _Data = Data
        End Sub
    End Class
End Namespace
