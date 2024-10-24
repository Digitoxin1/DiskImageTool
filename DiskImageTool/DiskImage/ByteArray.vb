﻿Imports DiskImageTool.Bitstream

Namespace DiskImage
    Public Class ByteArray
        Implements IByteArray

        Private _Data() As Byte
        Private ReadOnly _ProtectedSectors As HashSet(Of UInteger)
        Private ReadOnly _AdditionalTracks As HashSet(Of UShort)
        Private ReadOnly _NonStandardTracks As HashSet(Of UShort)

        Public Event DataChanged As DataChangedEventHandler Implements IByteArray.DataChanged
        Public Event SizeChanged As SizeChangedEventHandler Implements IByteArray.SizeChanged

        Sub New(Data() As Byte)
            _Data = Data
            _ProtectedSectors = New HashSet(Of UInteger)
            _AdditionalTracks = New HashSet(Of UShort)
            _NonStandardTracks = New HashSet(Of UShort)
        End Sub

        Public ReadOnly Property BitstreamImage As IBitstreamImage Implements IByteArray.BitstreamImage
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property CanResize As Boolean Implements IByteArray.CanResize
            Get
                Return True
            End Get
        End Property

        Public ReadOnly Property ImageType As FloppyImageType Implements IByteArray.ImageType
            Get
                Return FloppyImageType.BasicSectorImage
            End Get
        End Property

        Public ReadOnly Property Length As Integer Implements IByteArray.Length
            Get
                Return _Data.Length
            End Get
        End Property

        Public Sub Append(Data() As Byte) Implements IByteArray.Append
            Dim Offset As UInteger = _Data.Length
            Dim Size As UInteger = Data.Length

            ReDim Preserve _Data(Offset + Size - 1)

            Data.CopyTo(_Data, Offset)
        End Sub

        Public Sub CopyTo(SourceIndex As Integer, ByRef DestinationArray() As Byte, DestinationIndex As Integer, Length As Integer) Implements IByteArray.CopyTo
            Array.Copy(_Data, SourceIndex, DestinationArray, DestinationIndex, Length)
        End Sub

        Public Sub CopyTo(DestinationArray() As Byte, Index As Integer) Implements IByteArray.CopyTo
            _Data.CopyTo(DestinationArray, Index)
        End Sub

        Public Function GetByte(Offset As UInteger) As Byte Implements IByteArray.GetByte
            Return _Data(Offset)
        End Function

        Public Function GetBytes() As Byte() Implements IByteArray.GetBytes
            Return _Data
        End Function

        Public Function GetBytes(Offset As UInteger, Size As UInteger) As Byte() Implements IByteArray.GetBytes
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

        Public Function GetBytesInteger(Offset As UInteger) As UInteger Implements IByteArray.GetBytesInteger
            Return BitConverter.ToUInt32(_Data, Offset)
        End Function

        Public Function GetBytesShort(Offset As UInteger) As UShort Implements IByteArray.GetBytesShort
            Return BitConverter.ToUInt16(_Data, Offset)
        End Function

        Public Function GetCRC32() As String Implements IByteArray.GetCRC32
            Return HashFunctions.CRC32Hash(_Data)
        End Function

        Public Function GetMD5Hash() As String Implements IByteArray.GetMD5Hash
            Return MD5Hash(_Data)
        End Function

        Public Function GetSHA1Hash() As String Implements IByteArray.GetSHA1Hash
            Return SHA1Hash(_Data)
        End Function

        Public Function Resize(Length As Integer) As Boolean Implements IByteArray.Resize
            Dim Result As Boolean = False

            If _Data.Length <> Length Then
                Dim CurrentLength = _Data.Length
                ReDim Preserve _Data(Length - 1)
                Result = True
                RaiseEvent SizeChanged(CurrentLength, Length)
            End If

            Return Result
        End Function

        Public Function SetBytes(Value As UShort, Offset As UInteger) As Boolean Implements IByteArray.SetBytes
            Dim Result As Boolean = False
            Dim CurrentValue = GetBytesShort(Offset)

            If CurrentValue <> Value Then
                Array.Copy(BitConverter.GetBytes(Value), 0, _Data, Offset, 2)
                Result = True
                RaiseEvent DataChanged(Offset, CurrentValue, Value)
            End If

            Return Result
        End Function

        Public Function SetBytes(Value As UInteger, Offset As UInteger) As Boolean Implements IByteArray.SetBytes
            Dim Result As Boolean = False
            Dim CurrentValue = GetBytesInteger(Offset)

            If CurrentValue <> Value Then
                Array.Copy(BitConverter.GetBytes(Value), 0, _Data, Offset, 4)
                Result = True
                RaiseEvent DataChanged(Offset, CurrentValue, Value)
            End If

            Return Result
        End Function

        Public Function SetBytes(Value As Byte, Offset As UInteger) As Boolean Implements IByteArray.SetBytes
            Dim Result As Boolean = False
            Dim CurrentValue = GetByte(Offset)

            If CurrentValue <> Value Then
                _Data(Offset) = Value
                Result = True
                RaiseEvent DataChanged(Offset, CurrentValue, Value)
            End If

            Return Result
        End Function

        Public Function SetBytes(Value() As Byte, Offset As UInteger) As Boolean Implements IByteArray.SetBytes
            Return SetBytes(Value, Offset, Value.Length, 0)
        End Function

        Public Function SetBytes(Value() As Byte, Offset As UInteger, Size As UInteger, Padding As Byte) As Boolean Implements IByteArray.SetBytes
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
                    RaiseEvent DataChanged(Offset, CurrentValue, Value)
                End If
            End If

            Return Result
        End Function

        Public Function ToUInt16(StartIndex As Integer) As UShort Implements IByteArray.ToUInt16
            Return BitConverter.ToUInt16(_Data, StartIndex)
        End Function

        Public Function SaveToFile(FilePath As String) As Boolean Implements IByteArray.SaveToFile
            Try
                IO.File.WriteAllBytes(FilePath, _Data)
            Catch ex As Exception
                Return False
            End Try

            Return True
        End Function

        Private ReadOnly Property IByteArray_AdditionalTracks As HashSet(Of UShort) Implements IByteArray.AdditionalTracks
            Get
                Return _AdditionalTracks
            End Get
        End Property

        Private ReadOnly Property IByteArray_NonStandardTracks As HashSet(Of UShort) Implements IByteArray.NonStandardTracks
            Get
                Return _NonStandardTracks
            End Get
        End Property

        Private ReadOnly Property IByteArray_ProtectedSectors As HashSet(Of UInteger) Implements IByteArray.ProtectedSectors
            Get
                Return _ProtectedSectors
            End Get
        End Property

        Private ReadOnly Property IByteArray_TrackCount As UShort Implements IByteArray.TrackCount
            Get
                Return 0
            End Get
        End Property

        Private ReadOnly Property IByteArray_HeadCount As Byte Implements IByteArray.HeadCount
            Get
                Return 0
            End Get
        End Property
    End Class
End Namespace
