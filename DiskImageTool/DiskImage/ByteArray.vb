Namespace DiskImage
    Public Delegate Sub DataChangedEventHandler(Offset As UInteger, OriginalValue As Object, NewValue As Object)

    Public Class ByteArray
        Private _Data() As Byte

        Public Event DataChanged As DataChangedEventHandler

        Sub New(Data() As Byte)
            _Data = Data
        End Sub

        Public ReadOnly Property Data As Byte()
            Get
                Return _Data
            End Get
        End Property

        Public ReadOnly Property Length As Integer
            Get
                Return _Data.Length
            End Get
        End Property

        Public Sub Append(Data() As Byte)
            Dim Offset As UInteger = _Data.Length
            Dim Size As UInteger = Data.Length

            ReDim Preserve _Data(Offset + Size - 1)

            Data.CopyTo(_Data, Offset)
        End Sub

        Public Sub CopyTo(SourceIndex As Integer, ByRef DestinationArray() As Byte, DestinationIndex As Integer, Length As Integer)
            Array.Copy(_Data, SourceIndex, DestinationArray, DestinationIndex, Length)
        End Sub

        Public Function GetByte(Offset As UInteger) As Byte
            Return _Data(Offset)
        End Function

        Public Function GetBytes(Offset As UInteger, Size As UInteger) As Byte()
            Dim temp(Size - 1) As Byte
            Array.Copy(_Data, Offset, temp, 0, Size)
            Return temp
        End Function

        Public Function GetBytesInteger(Offset As UInteger) As UInteger
            Return BitConverter.ToUInt32(_Data, Offset)
        End Function

        Public Function GetBytesShort(Offset As UInteger) As UShort
            Return BitConverter.ToUInt16(_Data, Offset)
        End Function

        Public Function Resize(Length As Integer) As Boolean
            Dim Result As Boolean = False

            If _Data.Length <> Length Then
                ReDim Preserve _Data(Length - 1)
                Result = True
            End If

            Return Result
        End Function

        Public Function SetBytes(Value As UShort, Offset As UInteger) As Boolean
            Dim Result As Boolean = False
            Dim CurrentValue = GetBytesShort(Offset)

            If CurrentValue <> Value Then
                Array.Copy(BitConverter.GetBytes(Value), 0, _Data, Offset, 2)
                Result = True
                RaiseEvent DataChanged(Offset, CurrentValue, Value)
            End If

            Return Result
        End Function

        Public Function SetBytes(Value As UInteger, Offset As UInteger) As Boolean
            Dim Result As Boolean = False
            Dim CurrentValue = GetBytesInteger(Offset)

            If CurrentValue <> Value Then
                Array.Copy(BitConverter.GetBytes(Value), 0, _Data, Offset, 4)
                Result = True
                RaiseEvent DataChanged(Offset, CurrentValue, Value)
            End If

            Return Result
        End Function

        Public Function SetBytes(Value As Byte, Offset As UInteger) As Boolean
            Dim Result As Boolean = False
            Dim CurrentValue = GetByte(Offset)

            If CurrentValue <> Value Then
                _Data(Offset) = Value
                Result = True
                RaiseEvent DataChanged(Offset, CurrentValue, Value)
            End If

            Return Result
        End Function

        Public Function SetBytes(Value() As Byte, Offset As UInteger) As Boolean
            Return SetBytes(Value, Offset, Value.Length, 0)
        End Function

        Public Function SetBytes(Value() As Byte, Offset As UInteger, Size As UInteger, Padding As Byte) As Boolean
            Dim Result As Boolean = False

            If Offset + Size > _Data.Length Then
                If _Data.Length - Offset >= 0 Then
                    Size = _Data.Length - Offset
                Else
                    Size = 0
                End If
            End If

            If Size > 0 Then
                If Value.Length <> Size Then
                    ResizeArray(Value, Size, Padding)
                End If

                Dim CurrentValue = GetBytes(Offset, Size)

                If Not ByteArrayCompare(CurrentValue, Value) Then
                    Array.Copy(Value, 0, _Data, Offset, Size)
                    Result = True
                    RaiseEvent DataChanged(Offset, CurrentValue, Value)
                End If
            End If

            Return Result
        End Function

        Public Function ToUInt16(StartIndex As Integer) As UShort
            Return BitConverter.ToUInt16(_Data, StartIndex)
        End Function
    End Class
End Namespace
