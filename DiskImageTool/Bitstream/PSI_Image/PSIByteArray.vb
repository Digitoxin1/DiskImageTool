Imports DiskImageTool.DiskImage
Imports DiskImageTool.PSI_Image

Public Class PSIByteArray
    Implements IByteArray

    Private ReadOnly _Image As PSISectorImage
    Private ReadOnly _DiskFormat As FloppyDiskFormat
    Private ReadOnly _ImageParams As FloppyDiskParams
    Private ReadOnly _ImageSize As Integer
    Private ReadOnly _SectorMap() As PSISector
    Private ReadOnly _ProtectedSectors As HashSet(Of UInteger)
    Private ReadOnly _EmptySector() As Byte

    Public Event DataChanged As DataChangedEventHandler Implements IByteArray.DataChanged
    Public Event SizeChanged As SizeChangedEventHandler Implements IByteArray.SizeChanged

    Private Class MappedData
        Public Property Sector As UInteger
        Public Property Offset As UInteger
        Public Property Data As Byte()
        Public Property CanWrite As Boolean
        Public Property Size As UInteger
    End Class

    Public Sub New(Image As PSISectorImage, DiskFormat As FloppyDiskFormat)
        _Image = Image
        _DiskFormat = DiskFormat
        _ImageParams = GetFloppyDiskParams(_DiskFormat)
        _ImageSize = GetFloppyDiskSize(_DiskFormat)
        _SectorMap = New PSISector(_ImageSize \ _ImageParams.BytesPerSector - 1) {}
        _ProtectedSectors = New HashSet(Of UInteger)
        _EmptySector = New Byte(_ImageParams.BytesPerSector - 1) {}

        BuildSectorMap()
    End Sub

    Public ReadOnly Property CanResize As Boolean Implements IByteArray.CanResize
        Get
            Return False
        End Get
    End Property

    Public ReadOnly Property ProtectedSectors As HashSet(Of UInteger) Implements IByteArray.ProtectedSectors
        Get
            Return _ProtectedSectors
        End Get
    End Property

    Private Function IsProtectedSector(PSISector As PSISector) As Boolean
        Return PSISector.Size <> _ImageParams.BytesPerSector Or PSISector.HasDataCRCError
    End Function

    Private Sub BuildSectorMap()
        Dim TrackCount = Int(_ImageParams.SectorCountSmall / _ImageParams.SectorsPerTrack / _ImageParams.NumberOfHeads)

        For Each PSISector In _Image.Sectors
            Dim MaxSize As UInteger = _ImageParams.BytesPerSector
            Dim MaxSectors As UShort = _ImageParams.SectorsPerTrack

            If Not PSISector.IsAlternateSector Then
                If PSISector.Cylinder < TrackCount And PSISector.Head < _ImageParams.NumberOfHeads Then
                    If PSISector.Sector >= 1 And PSISector.Sector <= MaxSectors Then
                        Dim Offset = GetOffset(PSISector.Cylinder, PSISector.Head, PSISector.Sector)
                        Dim Sector = GetSector(Offset)
                        If PSISector.Size <= MaxSize Then
                            If IsProtectedSector(PSISector) Then
                                If Not _ProtectedSectors.Contains(Sector) Then
                                    _ProtectedSectors.Add(Sector)
                                End If
                            End If
                        Else
                            If Not _ProtectedSectors.Contains(Sector) Then
                                _ProtectedSectors.Add(Sector)
                            End If
                        End If
                        _SectorMap(Sector) = PSISector
                    End If
                End If
            End If
        Next
    End Sub

    Private Function GetMappedData(Offset As UInteger) As MappedData
        Dim MappedData As New MappedData With {
            .Sector = GetSector(Offset),
            .Offset = GetSectorOffset(Offset),
            .Size = _ImageParams.BytesPerSector
        }

        If _SectorMap(MappedData.Sector) Is Nothing Then
            MappedData.Data = _EmptySector
            MappedData.CanWrite = False
        Else
            Dim PSISector = _SectorMap(MappedData.Sector)
            If PSISector.Size < MappedData.Size Then
                MappedData.Data = New Byte(MappedData.Size - 1) {}
                PSISector.Data.CopyTo(MappedData.Data, 0)
            Else
                MappedData.Data = PSISector.Data
            End If

            MappedData.CanWrite = Not IsProtectedSector(PSISector)
        End If

        Return MappedData
    End Function

    Private Function GetOffset(Track As UShort, Side As Byte, SectorId As UShort) As UInteger
        Dim Offset As UInteger = Track * _ImageParams.NumberOfHeads * _ImageParams.SectorsPerTrack + _ImageParams.SectorsPerTrack * Side + (SectorId - 1)

        Return Offset * _ImageParams.BytesPerSector
    End Function

    Private Function GetSector(Offset As UInteger) As UInteger
        Return Offset \ _ImageParams.BytesPerSector
    End Function

    Private Function GetSectorOffset(Offset As UInteger) As UInteger
        Return Offset Mod _ImageParams.BytesPerSector
    End Function

    Public ReadOnly Property ImageType As FloppyImageType Implements IByteArray.ImageType
        Get
            Return FloppyImageType.PSIImage
        End Get
    End Property

    Public ReadOnly Property Length As Integer Implements IByteArray.Length
        Get
            Return _ImageSize
        End Get
    End Property

    Public Sub Append(Data() As Byte) Implements IByteArray.Append
        ' Unsupported - Do Nothing
    End Sub

    Public Sub CopyTo(SourceIndex As Integer, ByRef DestinationArray() As Byte, DestinationIndex As Integer, Length As Integer) Implements IByteArray.CopyTo

        Do While Length > 0
            Dim MappedData = GetMappedData(SourceIndex)
            Dim ActualSize = Math.Min(Length, MappedData.Size)
            Array.Copy(MappedData.Data, MappedData.Offset, DestinationArray, DestinationIndex, ActualSize)
            SourceIndex += ActualSize
            Length -= ActualSize
            DestinationIndex += ActualSize
        Loop
    End Sub

    Public Sub CopyTo(DestinationArray() As Byte, Index As Integer) Implements IByteArray.CopyTo
        CopyTo(0, DestinationArray, 0, _ImageSize)
    End Sub

    Public Function GetByte(Offset As UInteger) As Byte Implements IByteArray.GetByte
        Dim MappedData = GetMappedData(Offset)

        Return MappedData.Data(MappedData.Offset)
    End Function

    Public Function GetBytes() As Byte() Implements IByteArray.GetBytes
        Return GetBytes(0, _ImageSize)
    End Function

    Public Function GetBytes(Offset As UInteger, Size As UInteger) As Byte() Implements IByteArray.GetBytes

        Dim temp(Size - 1) As Byte
        Dim Index As Long = 0
        Do While Size > 0
            Dim MappedData = GetMappedData(Offset)
            Dim ActualSize = Math.Min(Size, MappedData.Size)
            Array.Copy(MappedData.Data, MappedData.Offset, temp, Index, ActualSize)
            Offset += ActualSize
            Size -= ActualSize
            Index += ActualSize
        Loop

        Return temp
    End Function

    Public Function GetBytesInteger(Offset As UInteger) As UInteger Implements IByteArray.GetBytesInteger
        Dim MappedData = GetMappedData(Offset)

        Return BitConverter.ToUInt32(MappedData.Data, MappedData.Offset)
    End Function

    Public Function GetBytesShort(Offset As UInteger) As UShort Implements IByteArray.GetBytesShort
        Dim MappedData = GetMappedData(Offset)

        Return BitConverter.ToUInt16(MappedData.Data, MappedData.Offset)
    End Function

    Public Function Resize(Length As Integer) As Boolean Implements IByteArray.Resize
        Return False
    End Function

    Public Function SetBytes(Value As UShort, Offset As UInteger) As Boolean Implements IByteArray.SetBytes
        Dim MappedData = GetMappedData(Offset)

        If Not MappedData.CanWrite Then
            Return False
        End If

        Dim Result As Boolean = False
        Dim CurrentValue = GetBytesShort(MappedData.Offset)

        If CurrentValue <> Value Then
            Array.Copy(BitConverter.GetBytes(Value), 0, MappedData.Data, MappedData.Offset, 2)
            Result = True
            RaiseEvent DataChanged(Offset, CurrentValue, Value)
        End If

        Return Result
    End Function

    Public Function SetBytes(Value As UInteger, Offset As UInteger) As Boolean Implements IByteArray.SetBytes
        Dim MappedData = GetMappedData(Offset)

        If Not MappedData.CanWrite Then
            Return False
        End If

        Dim Result As Boolean = False
        Dim CurrentValue = GetBytesInteger(MappedData.Offset)

        If CurrentValue <> Value Then
            Array.Copy(BitConverter.GetBytes(Value), 0, MappedData.Data, MappedData.Offset, 4)
            Result = True
            RaiseEvent DataChanged(Offset, CurrentValue, Value)
        End If

        Return Result
    End Function

    Public Function SetBytes(Value As Byte, Offset As UInteger) As Boolean Implements IByteArray.SetBytes
        Dim MappedData = GetMappedData(Offset)

        If Not MappedData.CanWrite Then
            Return False
        End If

        Dim Result As Boolean = False
        Dim CurrentValue = GetByte(MappedData.Offset)

        If CurrentValue <> Value Then
            MappedData.Data(MappedData.Offset) = Value
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

        If Offset + Size > _ImageSize Then
            If _ImageSize - Offset >= 0 Then
                Size = _ImageSize - Offset
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
                Dim Index As Long = 0
                Dim NewOffset = Offset
                Do While Size > 0
                    Dim MappedData = GetMappedData(NewOffset)
                    Dim ActualSize = Math.Min(Size, MappedData.Size)
                    If MappedData.CanWrite Then
                        Array.Copy(Value, Index, MappedData.Data, MappedData.Offset, ActualSize)
                        Result = True
                    End If
                    NewOffset += ActualSize
                    Size -= ActualSize
                    Index += ActualSize
                Loop

                If Result Then
                    RaiseEvent DataChanged(Offset, CurrentValue, Value)
                End If
            End If
        End If

        Return Result
    End Function

    Public Function ToUInt16(StartIndex As Integer) As UShort Implements IByteArray.ToUInt16
        Dim MappedData = GetMappedData(StartIndex)

        Return BitConverter.ToUInt16(MappedData.Data, MappedData.Offset)
    End Function

    Public Function SaveToFile(FilePath As String) As Boolean Implements IByteArray.SaveToFile
        Return _Image.Export(FilePath)
    End Function
End Class
