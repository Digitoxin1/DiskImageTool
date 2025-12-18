Namespace ImageFormats.TD0
    Public Class TD0Sector
        Private ReadOnly _header As TD0SectorHeader
        Private _Data As Byte() = Nothing
        Private _DataHeader As TD0SectorDataHeader = Nothing

        Public Sub New(Header As TD0SectorHeader)
            _header = Header
        End Sub

        Public ReadOnly Property Data As Byte()
            Get
                Return _Data
            End Get
        End Property

        Public ReadOnly Property DataHeader As TD0SectorDataHeader
            Get
                Return _DataHeader
            End Get
        End Property

        Public ReadOnly Property Header As TD0SectorHeader
            Get
                Return _header
            End Get
        End Property

        Public Function CrcValid() As Boolean
            Return _header.CrcValid(_Data)
        End Function

        Public Sub RefreshStoredCrc16()
            _header.RefreshStoredCrcLow(_Data)
        End Sub

        Public Sub SetData(DataHeader As TD0SectorDataHeader, Data As Byte())
            If DataHeader Is Nothing Then
                _DataHeader = New TD0SectorDataHeader(Data.Length, TD0EncodingMethod.Raw)
            Else
                _DataHeader = DataHeader
            End If

            _Data = Data
        End Sub
    End Class
End Namespace
