Namespace Hb.Windows.Forms
    Friend NotInheritable Class MemoryDataBlock
        Inherits Forms.DataBlock

        Private _data As Byte()

        Public Sub New(data As Byte)
            _data = New Byte() {data}
        End Sub

        Public Sub New(data As Byte())
            If data Is Nothing Then
                Throw New ArgumentNullException("data")
            End If

            _data = CType(data.Clone(), Byte())
        End Sub

        Public ReadOnly Property Data As Byte()
            Get
                Return _data
            End Get
        End Property

        Public Overrides ReadOnly Property Length As Long
            Get
                Return _data.LongLength
            End Get
        End Property

        Public Sub AddByteToEnd(value As Byte)
            Dim newData = New Byte(_data.LongLength + 1 - 1) {}
            _data.CopyTo(newData, 0)
            newData(newData.LongLength - 1) = value
            _data = newData
        End Sub

        Public Sub AddByteToStart(value As Byte)
            Dim newData = New Byte(_data.LongLength + 1 - 1) {}
            newData(0) = value
            _data.CopyTo(newData, 1)
            _data = newData
        End Sub

        Public Sub InsertBytes(position As Long, data As Byte())
            Dim newData = New Byte(_data.LongLength + data.LongLength - 1) {}

            If position > 0 Then
                Array.Copy(_data, 0, newData, 0, position)
            End If

            Array.Copy(data, 0, newData, position, data.LongLength)

            If position < _data.LongLength Then
                Array.Copy(_data, position, newData, position + data.LongLength, _data.LongLength - position)
            End If

            _data = newData
        End Sub

        Public Overrides Sub RemoveBytes(position As Long, count As Long)
            Dim newData = New Byte(_data.LongLength - count - 1) {}

            If position > 0 Then
                Array.Copy(_data, 0, newData, 0, position)
            End If

            If position + count < _data.LongLength Then
                Array.Copy(_data, position + count, newData, position, newData.LongLength - position)
            End If

            _data = newData
        End Sub
    End Class
End Namespace
