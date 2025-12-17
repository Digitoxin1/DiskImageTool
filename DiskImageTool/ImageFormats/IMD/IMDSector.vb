
Namespace ImageFormats.IMD
    Public Class IMDSector
        Private _Data As Byte()

        Public Sub New(Size As UShort)
            _Data = New Byte(Size - 1) {}
            _Deleted = False
            _ChecksumError = False
            _Compressed = False
            _Unavailable = False
            _CompressedValue = 0
        End Sub

        Public Sub New(Size As UShort, Format As DataFormat)
            _Data = New Byte(Size - 1) {}
            _Unavailable = (Format = DataFormat.Unavailable)
            _Compressed = (Format = DataFormat.DeletedCompressed Or Format = DataFormat.DeletedErrorCompressed Or Format = DataFormat.NormalCompressed Or Format = DataFormat.NormalErrorCompressed)
            _Deleted = (Format = DataFormat.Deleted Or Format = DataFormat.DeletedCompressed Or Format = DataFormat.DeletedError Or Format = DataFormat.DeletedErrorCompressed)
            _ChecksumError = (Format = DataFormat.DeletedError Or Format = DataFormat.DeletedErrorCompressed Or Format = DataFormat.NormalError Or Format = DataFormat.NormalErrorCompressed)
            _CompressedValue = 0
        End Sub

        Public Property ChecksumError As Boolean

        Public Property Compressed As Boolean

        Public Property CompressedValue As Byte

        Public Property Data As Byte()
            Get
                Return _Data
            End Get
            Set
                _Data = Value
                UpdateDataProperties()
            End Set
        End Property

        Public Property Deleted As Boolean
        Public Property SectorId As Byte
        Public Property Side As Byte
        Public Property Track As Byte
        Public Property Unavailable As Boolean

        Public Function GetFormat() As DataFormat
            If _Unavailable Then
                Return DataFormat.Unavailable
            ElseIf _Compressed And _Deleted And _ChecksumError Then
                Return DataFormat.DeletedErrorCompressed
            ElseIf _Compressed And _Deleted Then
                Return DataFormat.DeletedCompressed
            ElseIf _Compressed And _ChecksumError Then
                Return DataFormat.NormalErrorCompressed
            ElseIf _Compressed Then
                Return DataFormat.NormalCompressed
            ElseIf _Deleted And _ChecksumError Then
                Return DataFormat.DeletedError
            ElseIf _Deleted Then
                Return DataFormat.Deleted
            ElseIf _ChecksumError Then
                Return DataFormat.NormalError
            Else
                Return DataFormat.Normal
            End If
        End Function

        Private Sub UpdateDataProperties()
            Dim CompressedValue As Byte = 0
            Dim Compressed As Boolean = True

            For i = 0 To _Data.Length - 1
                If i = 0 Then
                    CompressedValue = _Data(i)
                ElseIf CompressedValue <> _Data(i) Then
                    Compressed = False
                    CompressedValue = 0
                    Exit For
                End If
            Next

            Me.Compressed = Compressed
            Me.CompressedValue = CompressedValue
        End Sub
    End Class
End Namespace