Namespace DiskImage
    Public Interface IByteArray
        Event DataChanged As DataChangedEventHandler
        Event SizeChanged As SizeChangedEventHandler

        ReadOnly Property Length As Integer
        Sub Append(Data() As Byte)
        Sub CopyTo(SourceIndex As Integer, ByRef DestinationArray() As Byte, DestinationIndex As Integer, Length As Integer)
        Sub CopyTo(DestinationArray() As Byte, Index As Integer)
        Function GetByte(Offset As UInteger) As Byte
        Function GetBytes() As Byte()
        Function GetBytes(Offset As UInteger, Size As UInteger) As Byte()
        Function GetBytesInteger(Offset As UInteger) As UInteger
        Function GetBytesShort(Offset As UInteger) As UShort
        Function Resize(Length As Integer) As Boolean
        Function SetBytes(Value As UShort, Offset As UInteger) As Boolean
        Function SetBytes(Value As UInteger, Offset As UInteger) As Boolean
        Function SetBytes(Value As Byte, Offset As UInteger) As Boolean
        Function SetBytes(Value() As Byte, Offset As UInteger) As Boolean
        Function SetBytes(Value() As Byte, Offset As UInteger, Size As UInteger, Padding As Byte) As Boolean
        Function ToUInt16(StartIndex As Integer) As UShort

    End Interface
End Namespace
