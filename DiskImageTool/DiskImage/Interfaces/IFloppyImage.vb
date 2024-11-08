Imports DiskImageTool.Bitstream

Namespace DiskImage
    Public Enum FloppyImageType
        BasicSectorImage
        TranscopyImage
        PRIImage
        PSIImage
        MFMImage
        D86FImage
        HFEImage
        IMDImage
    End Enum

    Public Interface IFloppyImage
        ReadOnly Property AdditionalTracks As HashSet(Of UShort)
        ReadOnly Property BitstreamImage As IBitstreamImage
        ReadOnly Property CanResize As Boolean
        ReadOnly Property History As ImageHistory
        ReadOnly Property ImageType As FloppyImageType
        ReadOnly Property IsBitstreamImage As Boolean
        ReadOnly Property Length As Integer
        ReadOnly Property NonStandardTracks As HashSet(Of UShort)
        ReadOnly Property ProtectedSectors As HashSet(Of UInteger)
        ReadOnly Property SideCount As Byte
        ReadOnly Property TrackCount As UShort
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
        Function SaveToFile(FilePath As String) As Boolean
        Function SetBytes(Value As Object, Offset As UInteger) As Boolean
        Function SetBytes(Value As UShort, Offset As UInteger) As Boolean
        Function SetBytes(Value As UInteger, Offset As UInteger) As Boolean
        Function SetBytes(Value As Byte, Offset As UInteger) As Boolean
        Function SetBytes(Value() As Byte, Offset As UInteger) As Boolean
        Function SetBytes(Value() As Byte, Offset As UInteger, Size As UInteger, Padding As Byte) As Boolean
        Function ToUInt16(StartIndex As Integer) As UShort
    End Interface
End Namespace
