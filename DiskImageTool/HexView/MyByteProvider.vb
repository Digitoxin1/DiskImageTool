Imports DiskImageTool.DiskImage
Imports DiskImageTool.Hb.Windows.Forms

Public Class MyByteProvider
    Implements IByteProvider

    Private ReadOnly _FloppyImage As IFloppyImage
    Private ReadOnly _Offset As UInteger
    Private ReadOnly _Size As UInteger
    Private _hasChanges As Boolean
    Public Event Changed As EventHandler Implements IByteProvider.Changed
    Public Event LengthChanged As EventHandler Implements IByteProvider.LengthChanged

    Public Sub New(FloppyImage As IFloppyImage, Offset As UInteger, Size As UInteger)
        _FloppyImage = FloppyImage
        _Offset = Offset
        _Size = Size

        AddHandler FloppyImage.History.DataChanged, AddressOf DataBytes_DataChanged
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        RemoveHandler _FloppyImage.History.DataChanged, AddressOf DataBytes_DataChanged
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
        Return _FloppyImage.GetByte(_Offset + index)
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
        _FloppyImage.SetBytes(value, Offset)
    End Sub

    Private Sub DataBytes_DataChanged(sender As Object, e As EventArgs)
        _hasChanges = True
        RaiseEvent Changed(Me, EventArgs.Empty)
    End Sub
End Class

