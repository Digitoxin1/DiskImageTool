Imports DiskImageTool.DiskImage
Imports Hb.Windows.Forms

Public Class MyByteProvider
    Implements IByteProvider

    Private WithEvents DataBytes As IByteArray
    Private ReadOnly _Offset As UInteger
    Private ReadOnly _Size As UInteger
    Private _hasChanges As Boolean
    Public Event Changed As EventHandler Implements IByteProvider.Changed
    Public Event LengthChanged As EventHandler Implements IByteProvider.LengthChanged

    Public Sub New(DataBytes As IByteArray, Offset As UInteger, Size As UInteger)
        Me.DataBytes = DataBytes
        _Offset = Offset
        _Size = Size
    End Sub

    Public ReadOnly Property Length As Long Implements IByteProvider.Length
        Get
            Return _Size
        End Get
    End Property

    Public Sub ApplyChanges() Implements IByteProvider.ApplyChanges
        _hasChanges = False
    End Sub

    Public Sub DeleteBytes(index As Long, length As Long) Implements IByteProvider.DeleteBytes
        'Not Implmented
    End Sub

    Public Function HasChanges() As Boolean Implements IByteProvider.HasChanges
        Return _hasChanges
    End Function

    Public Sub InsertBytes(index As Long, bs As Byte()) Implements IByteProvider.InsertBytes
        'Not Implmented
    End Sub

    Public Function ReadByte(index As Long) As Byte Implements IByteProvider.ReadByte
        Return DataBytes.GetByte(_Offset + index)
    End Function

    Public Function SupportsDeleteBytes() As Boolean Implements IByteProvider.SupportsDeleteBytes
        Return False
    End Function

    Public Function SupportsInsertBytes() As Boolean Implements IByteProvider.SupportsInsertBytes
        Return False
    End Function

    Public Function SupportsWriteByte() As Boolean Implements IByteProvider.SupportsWriteByte
        Return True
    End Function

    Public Sub WriteByte(index As Long, value As Byte) Implements IByteProvider.WriteByte
        Dim Offset As UInteger = _Offset + index
        DataBytes.SetBytes(value, Offset)
    End Sub

    Private Sub DataBytes_DataChanged(Offset As UInteger, OriginalValue As Object, NewValue As Object) Handles DataBytes.DataChanged
        _hasChanges = True
        RaiseEvent Changed(Me, EventArgs.Empty)
    End Sub
End Class

