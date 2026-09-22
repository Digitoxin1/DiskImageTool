Imports System.Collections

Namespace DiskImage
    Public Class ImageHistory
        Private ReadOnly _Changes As Stack(Of DataChange())
        Private ReadOnly _FloppyImage As IFloppyImage
        Private ReadOnly _RedoChanges As Stack(Of DataChange())
        Private _BatchEditMode As Boolean
        Private _Enabled As Boolean
        Private _IgnoreChange As Boolean
        Private _PendingChanges As List(Of DataChange)

        Public Event DataChanged As EventHandler

        Public Sub New(FloppyImage As IFloppyImage)
            _FloppyImage = FloppyImage
            _BatchEditMode = False
            _Changes = New Stack(Of DataChange())
            _RedoChanges = New Stack(Of DataChange())
            _IgnoreChange = False
            _PendingChanges = Nothing
            _Enabled = False
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

        Public Property Enabled As Boolean
            Get
                Return _Enabled
            End Get
            Set(value As Boolean)
                _Enabled = value
            End Set
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

        Public Sub AddDataChange(Offset As UInteger, OriginalValue As Object, NewValue As Object)
            If Not _Enabled Then Exit Sub
            If _IgnoreChange Then Exit Sub

            Dim DataChange As New DataChange(DataChangeType.Data, Offset, OriginalValue, NewValue)

            If _BatchEditMode Then
                _PendingChanges.Add(DataChange)
            Else
                _Changes.Push({DataChange})
                _RedoChanges.Clear()
            End If

            RaiseEvent DataChanged(Me, EventArgs.Empty)
        End Sub

        Public Sub AddSizeChange(OriginalLength As Integer, NewLength As Integer)
            If Not _Enabled Then Exit Sub
            If _IgnoreChange Then Exit Sub

            Dim DataChange As New DataChange(DataChangeType.Size, 0, OriginalLength, NewLength)

            If _BatchEditMode Then
                _PendingChanges.Add(DataChange)
            Else
                _Changes.Push({DataChange})
                _RedoChanges.Clear()
            End If
        End Sub

        Public Sub AddBitstreamChange(Track As UShort, Side As Byte, OriginalBits As BitArray, NewBits As BitArray)
            If Not _Enabled Then Exit Sub
            If _IgnoreChange Then Exit Sub

            Dim Offset = (CUInt(Track) << 8) Or Side
            Dim DataChange As New DataChange(DataChangeType.Bitstream, Offset, CType(OriginalBits.Clone(), BitArray), CType(NewBits.Clone(), BitArray))

            If _BatchEditMode Then
                _PendingChanges.Add(DataChange)
            Else
                _Changes.Push({DataChange})
                _RedoChanges.Clear()
            End If

            RaiseEvent DataChanged(Me, EventArgs.Empty)
        End Sub

        Public Sub ApplyModifications(Modifications As Stack(Of DataChange()))
            If Not _Enabled Then Exit Sub

            For Each DataChange In Modifications.Reverse
                BatchEditMode = DataChange.Length > 1
                Dim HasBitstreamChange As Boolean = False
                For Each Item In DataChange
                    If Item.Type = DataChangeType.Data Then
                        _FloppyImage.SetBytes(Item.NewValue, Item.Offset)
                    ElseIf Item.Type = DataChangeType.Size Then
                        _FloppyImage.Resize(Item.NewValue)
                    ElseIf Item.Type = DataChangeType.Bitstream Then
                        ApplyBitstreamChange(Item.Offset, Item.NewValue)
                        AddBitstreamChange(TrackFromOffset(Item.Offset), SideFromOffset(Item.Offset), Item.OriginalValue, Item.NewValue)
                        HasBitstreamChange = True
                    End If
                Next
                RebuildMappedSectorsIfNeeded(HasBitstreamChange)
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

        Public Sub Redo()
            Dim DataChange = _RedoChanges.Pop
            Dim HasBitstreamChange As Boolean = False
            _IgnoreChange = True
            For Each Item In DataChange
                If Item.Type = DataChangeType.Data Then
                    _FloppyImage.SetBytes(Item.NewValue, Item.Offset)
                ElseIf Item.Type = DataChangeType.Size Then
                    _FloppyImage.Resize(Item.NewValue)
                ElseIf Item.Type = DataChangeType.Bitstream Then
                    ApplyBitstreamChange(Item.Offset, Item.NewValue)
                    HasBitstreamChange = True
                End If
            Next
            _IgnoreChange = False
            RebuildMappedSectorsIfNeeded(HasBitstreamChange)
            _Changes.Push(DataChange)
        End Sub

        Public Sub Undo()
            Dim DataChange = _Changes.Pop
            Dim HasBitstreamChange As Boolean = False
            _IgnoreChange = True
            For Each Item In DataChange.Reverse
                If Item.Type = DataChangeType.Data Then
                    _FloppyImage.SetBytes(Item.OriginalValue, Item.Offset)
                ElseIf Item.Type = DataChangeType.Size Then
                    _FloppyImage.Resize(Item.OriginalValue)
                ElseIf Item.Type = DataChangeType.Bitstream Then
                    ApplyBitstreamChange(Item.Offset, Item.OriginalValue)
                    HasBitstreamChange = True
                End If
            Next
            _IgnoreChange = False
            RebuildMappedSectorsIfNeeded(HasBitstreamChange)
            _RedoChanges.Push(DataChange)
        End Sub

        Private Sub ApplyBitstreamChange(Offset As UInteger, Bits As Object)
            _FloppyImage.SetTrackBitstream(TrackFromOffset(Offset), SideFromOffset(Offset), DirectCast(Bits, BitArray))
        End Sub

        Private Shared Function TrackFromOffset(Offset As UInteger) As UShort
            Return CUShort(Offset >> 8)
        End Function

        Private Shared Function SideFromOffset(Offset As UInteger) As Byte
            Return CByte(Offset And &HFF)
        End Function

        Private Sub RebuildMappedSectorsIfNeeded(HasBitstreamChange As Boolean)
            If Not HasBitstreamChange Then
                Exit Sub
            End If

            If TypeOf _FloppyImage Is MappedFloppyImage Then
                CType(_FloppyImage, MappedFloppyImage).RebuildSectorMap()
            End If
        End Sub
    End Class
End Namespace
