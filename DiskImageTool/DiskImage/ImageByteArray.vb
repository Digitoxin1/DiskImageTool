Namespace DiskImage
    Public Class ImageByteArray
        Inherits ByteArray

        Private ReadOnly _Changes As Stack(Of DataChange())
        Private ReadOnly _DirectoryCache As Dictionary(Of UInteger, Byte())
        Private ReadOnly _RedoChanges As Stack(Of DataChange())
        Private _BatchEditMode As Boolean = False
        Private _IgnoreChange As Boolean = False
        Private _PendingChanges As List(Of DataChange)

        Sub New(Data() As Byte)
            MyBase.New(Data)

            _Changes = New Stack(Of DataChange())
            _DirectoryCache = New Dictionary(Of UInteger, Byte())
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

        Public ReadOnly Property DirectoryCache As Dictionary(Of UInteger, Byte())
            Get
                Return _DirectoryCache
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

        Public Sub ClearChanges()
            _Changes.Clear()
            _RedoChanges.Clear()
            _PendingChanges = Nothing
        End Sub

        Public Function GetSector(Sector As UInteger) As Byte()
            Dim Offset = SectorToBytes(Sector)

            Return GetSectors(Sector, 1)
        End Function

        Public Function GetSectors(SectorStart As UInteger, Count As UShort) As Byte()
            Dim Offset = SectorToBytes(SectorStart)
            Dim Size = SectorToBytes(Count)

            Return MyBase.GetBytes(Offset, Size)
        End Function

        Public Sub Redo()
            Dim DataChange = _RedoChanges.Pop
            _IgnoreChange = True
            For Each Item In DataChange
                MyBase.SetBytes(Item.NewValue, Item.Offset)
            Next
            _IgnoreChange = False
            _Changes.Push(DataChange)
        End Sub

        Public Function SetSector(Value() As Byte, Sector As UInteger) As Boolean
            Dim Offset = SectorToBytes(Sector)

            Return MyBase.SetBytes(Value, Offset, BYTES_PER_SECTOR, 0)
        End Function

        Public Sub Undo()
            Dim DataChange = _Changes.Pop
            _IgnoreChange = True
            For Each Item In DataChange
                MyBase.SetBytes(Item.OriginalValue, Item.Offset)
            Next
            _IgnoreChange = False
            _RedoChanges.Push(DataChange)
        End Sub

        Public Sub ApplyModifications(Modifications As Stack(Of DataChange()))
            For Each DataChange In Modifications.Reverse
                BatchEditMode = DataChange.Length > 1
                For Each Item In DataChange
                    MyBase.SetBytes(Item.NewValue, Item.Offset)
                Next
                If BatchEditMode Then
                    BatchEditMode = False
                End If
            Next
        End Sub

        Private Sub ImageByteArray_DataChanged(Offset As UInteger, OriginalValue As Object, NewValue As Object) Handles Me.DataChanged
            If _IgnoreChange Then Exit Sub

            Dim DataChange = New DataChange(Offset, OriginalValue, NewValue)

            If _BatchEditMode Then
                _PendingChanges.Add(DataChange)
            Else
                _Changes.Push({DataChange})
                _RedoChanges.Clear()
            End If
        End Sub
    End Class
End Namespace
