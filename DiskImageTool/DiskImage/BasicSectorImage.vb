Namespace DiskImage
    Public Class BasicSectorImage
        Implements IFloppyImage

        Private _Data() As Byte
        Private ReadOnly _ProtectedSectors As HashSet(Of UInteger)
        Private ReadOnly _AdditionalTracks As HashSet(Of UShort)
        Private ReadOnly _NonStandardTracks As HashSet(Of UShort)
        Private ReadOnly _History As FloppyImageHistory

        Sub New(Data() As Byte)
            _Data = Data
            _History = New FloppyImageHistory(Me)
            _ProtectedSectors = New HashSet(Of UInteger)
            _AdditionalTracks = New HashSet(Of UShort)
            _NonStandardTracks = New HashSet(Of UShort)
        End Sub

        Public ReadOnly Property BitstreamImage As Bitstream.IBitstreamImage Implements IFloppyImage.BitstreamImage
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property CanResize As Boolean Implements IFloppyImage.CanResize
            Get
                Return True
            End Get
        End Property

        Public ReadOnly Property History As FloppyImageHistory Implements IFloppyImage.History
            Get
                Return _History
            End Get
        End Property

        Public ReadOnly Property ImageType As FloppyImageType Implements IFloppyImage.ImageType
            Get
                Return FloppyImageType.BasicSectorImage
            End Get
        End Property

        Public ReadOnly Property IsBitstreamImage As Boolean Implements IFloppyImage.IsBitstreamImage
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property Length As Integer Implements IFloppyImage.Length
            Get
                Return _Data.Length
            End Get
        End Property

        Public Sub Append(Data() As Byte) Implements IFloppyImage.Append
            Dim Offset As UInteger = _Data.Length
            Dim Size As UInteger = Data.Length

            ReDim Preserve _Data(Offset + Size - 1)

            Data.CopyTo(_Data, Offset)
        End Sub

        Public Sub CopyTo(SourceIndex As Integer, ByRef DestinationArray() As Byte, DestinationIndex As Integer, Length As Integer) Implements IFloppyImage.CopyTo
            Array.Copy(_Data, SourceIndex, DestinationArray, DestinationIndex, Length)
        End Sub

        Public Sub CopyTo(DestinationArray() As Byte, Index As Integer) Implements IFloppyImage.CopyTo
            _Data.CopyTo(DestinationArray, Index)
        End Sub

        Public Function GetByte(Offset As UInteger) As Byte Implements IFloppyImage.GetByte
            Return _Data(Offset)
        End Function

        Public Function GetBytes() As Byte() Implements IFloppyImage.GetBytes
            Return _Data
        End Function

        Public Function GetBytes(Offset As UInteger, Size As UInteger) As Byte() Implements IFloppyImage.GetBytes
            Dim temp(Size - 1) As Byte

            If Offset + Size > _Data.Length Then
                If _Data.Length - Offset >= 0 Then
                    Size = _Data.Length - Offset
                Else
                    Size = 0
                End If
            End If

            If Size > 0 Then
                Array.Copy(_Data, Offset, temp, 0, Size)
            End If

            Return temp
        End Function

        Public Function GetBytesInteger(Offset As UInteger) As UInteger Implements IFloppyImage.GetBytesInteger
            Return BitConverter.ToUInt32(_Data, Offset)
        End Function

        Public Function GetBytesShort(Offset As UInteger) As UShort Implements IFloppyImage.GetBytesShort
            Return BitConverter.ToUInt16(_Data, Offset)
        End Function

        Public Function GetCRC32() As String Implements IFloppyImage.GetCRC32
            Return HashFunctions.CRC32Hash(_Data)
        End Function

        Public Function GetMD5Hash() As String Implements IFloppyImage.GetMD5Hash
            Return MD5Hash(_Data)
        End Function

        Public Function GetSHA1Hash() As String Implements IFloppyImage.GetSHA1Hash
            Return SHA1Hash(_Data)
        End Function

        Public Function Resize(Length As Integer) As Boolean Implements IFloppyImage.Resize
            Dim Result As Boolean = False

            If _Data.Length <> Length Then
                Dim CurrentLength = _Data.Length
                ReDim Preserve _Data(Length - 1)
                Result = True
                If _History.Enabled Then
                    _History.AddSizeChange(CurrentLength, Length)
                End If
            End If

            Return Result
        End Function

        Public Function SetBytes(Value As Object, Offset As UInteger) As Boolean Implements IFloppyImage.SetBytes
            If TypeOf Value Is UShort Then
                Return SetBytes(DirectCast(Value, UShort), Offset)
            ElseIf TypeOf Value Is UInteger Then
                Return SetBytes(DirectCast(Value, UInteger), Offset)
            ElseIf TypeOf Value Is Byte Then
                Return SetBytes(DirectCast(Value, Byte), Offset)
            ElseIf TypeOf Value Is Byte() Then
                Return SetBytes(DirectCast(Value, Byte()), Offset)
            Else
                Return False
            End If
        End Function

        Public Function SetBytes(Value As UShort, Offset As UInteger) As Boolean Implements IFloppyImage.SetBytes
            Dim Result As Boolean = False
            Dim CurrentValue = GetBytesShort(Offset)

            If CurrentValue <> Value Then
                Array.Copy(BitConverter.GetBytes(Value), 0, _Data, Offset, 2)
                Result = True
                If _History.Enabled Then
                    _History.AddDataChange(Offset, CurrentValue, Value)
                End If
            End If

            Return Result
        End Function

        Public Function SetBytes(Value As UInteger, Offset As UInteger) As Boolean Implements IFloppyImage.SetBytes
            Dim Result As Boolean = False
            Dim CurrentValue = GetBytesInteger(Offset)

            If CurrentValue <> Value Then
                Array.Copy(BitConverter.GetBytes(Value), 0, _Data, Offset, 4)
                Result = True
                If _History.Enabled Then
                    _History.AddDataChange(Offset, CurrentValue, Value)
                End If
            End If

            Return Result
        End Function

        Public Function SetBytes(Value As Byte, Offset As UInteger) As Boolean Implements IFloppyImage.SetBytes
            Dim Result As Boolean = False
            Dim CurrentValue = GetByte(Offset)

            If CurrentValue <> Value Then
                _Data(Offset) = Value
                Result = True
                If _History.Enabled Then
                    _History.AddDataChange(Offset, CurrentValue, Value)
                End If
            End If

            Return Result
        End Function

        Public Function SetBytes(Value() As Byte, Offset As UInteger) As Boolean Implements IFloppyImage.SetBytes
            Return SetBytes(Value, Offset, Value.Length, 0)
        End Function

        Public Function SetBytes(Value() As Byte, Offset As UInteger, Size As UInteger, Padding As Byte) As Boolean Implements IFloppyImage.SetBytes
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

                If Not CurrentValue.CompareTo(Value) Then
                    Array.Copy(Value, 0, _Data, Offset, Size)
                    Result = True
                    If _History.Enabled Then
                        _History.AddDataChange(Offset, CurrentValue, Value)
                    End If
                End If
            End If

            Return Result
        End Function

        Public Function ToUInt16(StartIndex As Integer) As UShort Implements IFloppyImage.ToUInt16
            Return BitConverter.ToUInt16(_Data, StartIndex)
        End Function

        Public Function SaveToFile(FilePath As String) As Boolean Implements IFloppyImage.SaveToFile
            Try
                IO.File.WriteAllBytes(FilePath, _Data)
            Catch ex As Exception
                DebugException(ex)
                Return False
            End Try

            Return True
        End Function

        Private ReadOnly Property IFloppyImage_AdditionalTracks As HashSet(Of UShort) Implements IFloppyImage.AdditionalTracks
            Get
                Return _AdditionalTracks
            End Get
        End Property

        Private ReadOnly Property IFloppyImage_NonStandardTracks As HashSet(Of UShort) Implements IFloppyImage.NonStandardTracks
            Get
                Return _NonStandardTracks
            End Get
        End Property

        Private ReadOnly Property IFloppyImage_ProtectedSectors As HashSet(Of UInteger) Implements IFloppyImage.ProtectedSectors
            Get
                Return _ProtectedSectors
            End Get
        End Property

        Private ReadOnly Property IFloppyImage_TrackCount As UShort Implements IFloppyImage.TrackCount
            Get
                Return 0
            End Get
        End Property

        Private ReadOnly Property IFloppyImage_HeadCount As Byte Implements IFloppyImage.SideCount
            Get
                Return 0
            End Get
        End Property
    End Class
End Namespace
