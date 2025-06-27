Namespace Hb.Windows.Forms
    Friend NotInheritable Class FileDataBlock
        Inherits Forms.DataBlock

        Private _fileOffset As Long
        Private _length As Long

        Public Sub New(fileOffset As Long, length As Long)
            _fileOffset = fileOffset
            _length = length
        End Sub

        Public ReadOnly Property FileOffset As Long
            Get
                Return _fileOffset
            End Get
        End Property

        Public Overrides ReadOnly Property Length As Long
            Get
                Return _length
            End Get
        End Property

        Public Overrides Sub RemoveBytes(position As Long, count As Long)
            If position > _length Then
                Throw New ArgumentOutOfRangeException("position")
            End If

            If position + count > _length Then
                Throw New ArgumentOutOfRangeException("count")
            End If

            Dim prefixLength = position
            Dim prefixFileOffset = _fileOffset

            Dim suffixLength = _length - count - prefixLength
            Dim suffixFileOffset = _fileOffset + position + count

            If prefixLength > 0 AndAlso suffixLength > 0 Then
                _fileOffset = prefixFileOffset
                _length = prefixLength
                MyBase._map.AddAfter(Me, New FileDataBlock(suffixFileOffset, suffixLength))
                Return
            End If

            If prefixLength > 0 Then
                _fileOffset = prefixFileOffset
                _length = prefixLength
            Else
                _fileOffset = suffixFileOffset
                _length = suffixLength
            End If
        End Sub

        Public Sub RemoveBytesFromEnd(count As Long)
            If count > _length Then
                Throw New ArgumentOutOfRangeException("count")
            End If

            _length -= count
        End Sub

        Public Sub RemoveBytesFromStart(count As Long)
            If count > _length Then
                Throw New ArgumentOutOfRangeException("count")
            End If

            _fileOffset += count
            _length -= count
        End Sub

        Public Sub SetFileOffset(value As Long)
            _fileOffset = value
        End Sub
    End Class
End Namespace
