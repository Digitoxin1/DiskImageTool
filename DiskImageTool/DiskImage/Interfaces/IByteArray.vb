Namespace DiskImage
    Public Delegate Sub DataChangedEventHandler(Offset As UInteger, OriginalValue As Object, NewValue As Object)
    Public Delegate Sub SizeChangedEventHandler(OriginalLength As Integer, NewLength As Integer)

    Public Enum FloppyImageType
        BasicSectorImage
        TranscopyImage
        PSIImage
        MFMImage
        _86FImage
    End Enum

    Public Interface IByteArray
        Event DataChanged As DataChangedEventHandler
        Event SizeChanged As SizeChangedEventHandler

        ReadOnly Property CanResize As Boolean
        ReadOnly Property ImageType As FloppyImageType
        ReadOnly Property Length As Integer
        ReadOnly Property ProtectedSectors As HashSet(Of UInteger)
        ReadOnly Property NonStandardTracks As HashSet(Of UShort)
        ReadOnly Property TrackCount As UShort
        ReadOnly Property HeadCount As Byte
        Sub Append(Data() As Byte)
        Sub CopyTo(SourceIndex As Integer, ByRef DestinationArray() As Byte, DestinationIndex As Integer, Length As Integer)
        Sub CopyTo(DestinationArray() As Byte, Index As Integer)
        Function GetByte(Offset As UInteger) As Byte
        Function GetBytes() As Byte()
        Function GetBytes(Offset As UInteger, Size As UInteger) As Byte()
        Function GetBytesInteger(Offset As UInteger) As UInteger
        Function GetBytesShort(Offset As UInteger) As UShort
        Function GetCRC32() As String
        Function GetMD5Hash() As String
        Function GetSHA1Hash() As String
        Function Resize(Length As Integer) As Boolean
        Function SetBytes(Value As UShort, Offset As UInteger) As Boolean
        Function SetBytes(Value As UInteger, Offset As UInteger) As Boolean
        Function SetBytes(Value As Byte, Offset As UInteger) As Boolean
        Function SetBytes(Value() As Byte, Offset As UInteger) As Boolean
        Function SetBytes(Value() As Byte, Offset As UInteger, Size As UInteger, Padding As Byte) As Boolean
        Function ToUInt16(StartIndex As Integer) As UShort
        Function SaveToFile(FilePath As String) As Boolean

    End Interface
End Namespace
