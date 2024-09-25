Namespace DiskImage
    Public Class ImageByteArray
        Private WithEvents ImageData As IByteArray
        Private ReadOnly _Changes As Stack(Of DataChange())
        Private ReadOnly _RedoChanges As Stack(Of DataChange())
        Private _BatchEditMode As Boolean = False
        Private _IgnoreChange As Boolean = False
        Private _PendingChanges As List(Of DataChange)

        Sub New(Data As IByteArray)
            ImageData = Data

            _Changes = New Stack(Of DataChange())
            _RedoChanges = New Stack(Of DataChange())
        End Sub

        Public Property BatchEditMode As Boolean
            Get
                Return _BatchEditMode
            End Get
            Set(value As Boolean)
                If _BatchEditMode <> value Then
                    _BatchEditMode = value
                    If value Then
                        _PendingChanges = New List(Of DataChange)
                    Else
                        If _PendingChanges.Count > 0 Then
                            _Changes.Push(_PendingChanges.ToArray)
                            _RedoChanges.Clear()
                        End If
                        _PendingChanges = Nothing
                    End If
                End If
            End Set
        End Property

        Public ReadOnly Property Changes As Stack(Of DataChange())
            Get
                Return _Changes
            End Get
        End Property

        Public ReadOnly Property Data As IByteArray
            Get
                Return ImageData
            End Get
        End Property

        Public ReadOnly Property Length As Integer
            Get
                Return ImageData.Length
            End Get
        End Property

        Public ReadOnly Property Modified As Boolean
            Get
                Return _Changes.Count > 0
            End Get
        End Property

        Public ReadOnly Property RedoEnabled As Boolean
            Get
                Return _RedoChanges.Count > 0
            End Get
        End Property

        Public ReadOnly Property UndoEnabled As Boolean
            Get
                Return _Changes.Count > 0
            End Get
        End Property

        Public Sub Append(Data() As Byte)
            ImageData.Append(Data)
        End Sub

        Public Sub ApplyModifications(Modifications As Stack(Of DataChange()))
            For Each DataChange In Modifications.Reverse
                BatchEditMode = DataChange.Length > 1
                For Each Item In DataChange
                    If Item.Type = DataChangeType.Data Then
                        SetBytes(Item.NewValue, Item.Offset)
                    ElseIf Item.Type = DataChangeType.Size Then
                        ImageData.Resize(Item.NewValue)
                    End If
                Next
                If BatchEditMode Then
                    BatchEditMode = False
                End If
            Next
        End Sub

        Public Sub ClearChanges()
            _Changes.Clear()
            _RedoChanges.Clear()
            _PendingChanges = Nothing
        End Sub

        Public Sub CopyTo(SourceIndex As Integer, ByRef DestinationArray() As Byte, DestinationIndex As Integer, Length As Integer)
            ImageData.CopyTo(SourceIndex, DestinationArray, DestinationIndex, Length)
        End Sub

        Public Sub CopyTo(DestinationArray() As Byte, Index As Integer)
            ImageData.CopyTo(DestinationArray, Index)
        End Sub

        Public Function GetByte(Offset As UInteger) As Byte
            Return ImageData.GetByte(Offset)
        End Function

        Public Function GetBytes() As Byte()
            Return ImageData.GetBytes
        End Function

        Public Function GetBytes(Offset As UInteger, Size As UInteger) As Byte()
            Return ImageData.GetBytes(Offset, Size)
        End Function

        Public Function GetBytesInteger(Offset As UInteger) As UInteger
            Return ImageData.GetBytesInteger(Offset)
        End Function

        Public Function GetBytesShort(Offset As UInteger) As UShort
            Return ImageData.GetBytesShort(Offset)
        End Function

        Public Function GetSector(Sector As UInteger) As Byte()
            Dim Offset = Disk.SectorToBytes(Sector)

            Return GetSectors(Sector, 1)
        End Function

        Public Function GetSectors(SectorStart As UInteger, Count As UShort) As Byte()
            Dim Offset = Disk.SectorToBytes(SectorStart)
            Dim Size = Disk.SectorToBytes(Count)

            If Size + Offset > ImageData.Length Then
                Size = ImageData.Length - Offset
            End If

            Return ImageData.GetBytes(Offset, Size)
        End Function

        Public Sub Redo()
            Dim DataChange = _RedoChanges.Pop
            _IgnoreChange = True
            For Each Item In DataChange
                If Item.Type = DataChangeType.Data Then
                    SetBytes(Item.NewValue, Item.Offset)
                ElseIf Item.Type = DataChangeType.Size Then
                    ImageData.Resize(Item.NewValue)
                End If
            Next
            _IgnoreChange = False
            _Changes.Push(DataChange)
        End Sub

        Public Function Resize(Length As Integer) As Boolean
            Return ImageData.Resize(Length)
        End Function

        Public Function SetBytes(Value As UShort, Offset As UInteger) As Boolean
            Return ImageData.SetBytes(Value, Offset)
        End Function
        Public Function SetBytes(Value As UInteger, Offset As UInteger) As Boolean
            Return ImageData.SetBytes(Value, Offset)
        End Function
        Public Function SetBytes(Value As Byte, Offset As UInteger) As Boolean
            Return ImageData.SetBytes(Value, Offset)
        End Function
        Public Function SetBytes(Value() As Byte, Offset As UInteger) As Boolean
            Return ImageData.SetBytes(Value, Offset)
        End Function
        Public Function SetBytes(Value() As Byte, Offset As UInteger, Size As UInteger, Padding As Byte) As Boolean
            Return ImageData.SetBytes(Value, Offset, Size, Padding)
        End Function

        Public Function SetSector(Value() As Byte, Sector As UInteger) As Boolean
            Dim Offset = Disk.SectorToBytes(Sector)

            Return ImageData.SetBytes(Value, Offset, Disk.BYTES_PER_SECTOR, 0)
        End Function

        Public Function ToUInt16(StartIndex As Integer) As UShort
            Return ImageData.ToUInt16(StartIndex)
        End Function

        Public Sub Undo()
            Dim DataChange = _Changes.Pop
            _IgnoreChange = True
            For Each Item In DataChange.Reverse
                If Item.Type = DataChangeType.Data Then
                    SetBytes(Item.OriginalValue, Item.Offset)
                ElseIf Item.Type = DataChangeType.Size Then
                    ImageData.Resize(Item.OriginalValue)
                End If
            Next
            _IgnoreChange = False
            _RedoChanges.Push(DataChange)
        End Sub

        Private Sub ImageData_DataChanged(Offset As UInteger, OriginalValue As Object, NewValue As Object) Handles ImageData.DataChanged
            If _IgnoreChange Then Exit Sub

            Dim DataChange = New DataChange(DataChangeType.Data, Offset, OriginalValue, NewValue)

            If _BatchEditMode Then
                _PendingChanges.Add(DataChange)
            Else
                _Changes.Push({DataChange})
                _RedoChanges.Clear()
            End If
        End Sub

        Private Sub ImageData_SizeChanged(OriginalLength As Integer, NewLength As Integer) Handles ImageData.SizeChanged
            If _IgnoreChange Then Exit Sub

            Dim DataChange = New DataChange(DataChangeType.Size, 0, OriginalLength, NewLength)

            If _BatchEditMode Then
                _PendingChanges.Add(DataChange)
            Else
                _Changes.Push({DataChange})
                _RedoChanges.Clear()
            End If
        End Sub
    End Class
End Namespace
