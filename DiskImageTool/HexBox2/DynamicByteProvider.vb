Namespace Hb.Windows.Forms
    ''' <summary>
    ''' Byte provider for a small amount of data.
    ''' </summary>
    Public Class DynamicByteProvider
        Implements Forms.IByteProvider
        ''' <summary>
        ''' Contains a byte collection.
        ''' </summary>
        Private ReadOnly _bytes As List(Of Byte)

        ''' <summary>
        ''' Contains information about changes.
        ''' </summary>
        Private _hasChanges As Boolean

        ''' <summary>
        ''' Initializes a new instance of the DynamicByteProvider class.
        ''' </summary>
        ''' <paramname="data"></param>
        Public Sub New(data As Byte())
            Me.New(New List(Of Byte)(data))
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the DynamicByteProvider class.
        ''' </summary>
        ''' <paramname="bytes"></param>
        Public Sub New(bytes As List(Of Byte))
            _bytes = bytes
        End Sub

        ''' <summary>
        ''' Gets the byte collection.
        ''' </summary>
        Public ReadOnly Property Bytes As List(Of Byte)
            Get
                Return _bytes
            End Get
        End Property

        ''' <summary>
        ''' Raises the Changed event.
        ''' </summary>
        Private Sub OnChanged(e As EventArgs)
            _hasChanges = True

            RaiseEvent Changed(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the LengthChanged event.
        ''' </summary>
        Private Sub OnLengthChanged(e As EventArgs)
            RaiseEvent LengthChanged(Me, e)
        End Sub

#Region "IByteProvider Members"
        ''' <summary>
        ''' Occurs, when the write buffer contains new changes.
        ''' </summary>
        Public Event Changed As EventHandler Implements Forms.IByteProvider.Changed

        ''' <summary>
        ''' Occurs, when InsertBytes or DeleteBytes method is called.
        ''' </summary>
        Public Event LengthChanged As EventHandler Implements Forms.IByteProvider.LengthChanged

        ''' <summary>
        ''' Gets the length of the bytes in the byte collection.
        ''' </summary>
        Public ReadOnly Property Length As Long Implements Forms.IByteProvider.Length
            Get
                Return _bytes.Count
            End Get
        End Property

        ''' <summary>
        ''' Applies changes.
        ''' </summary>
        Public Sub ApplyChanges() Implements Forms.IByteProvider.ApplyChanges
            _hasChanges = False
        End Sub

        ''' <summary>
        ''' Deletes bytes from the byte collection.
        ''' </summary>
        ''' <paramname="index">the start index of the bytes to delete.</param>
        ''' <paramname="length">the length of bytes to delete.</param>
        Public Sub DeleteBytes(index As Long, length As Long) Implements Forms.IByteProvider.DeleteBytes
            Dim internal_index As Integer = Math.Max(0, index)
            Dim internal_length As Integer = Math.Min(CInt(Me.Length), length)
            _bytes.RemoveRange(internal_index, internal_length)

            OnLengthChanged(EventArgs.Empty)
            OnChanged(EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' True, when changes are done.
        ''' </summary>
        Public Function HasChanges() As Boolean Implements Forms.IByteProvider.HasChanges
            Return _hasChanges
        End Function

        ''' <summary>
        ''' Inserts byte into the byte collection.
        ''' </summary>
        ''' <paramname="index">the start index of the bytes in the byte collection</param>
        ''' <paramname="bs">the byte array to insert</param>
        Public Sub InsertBytes(index As Long, bs As Byte()) Implements Forms.IByteProvider.InsertBytes
            _bytes.InsertRange(index, bs)

            OnLengthChanged(EventArgs.Empty)
            OnChanged(EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Reads a byte from the byte collection.
        ''' </summary>
        ''' <paramname="index">the index of the byte to read</param>
        ''' <returns>the byte</returns>
        Public Function ReadByte(index As Long) As Byte Implements Forms.IByteProvider.ReadByte
            Return _bytes(index)
        End Function

        ''' <summary>
        ''' Applies changes.
        ''' </summary>
        Public Sub SaveAsChanges(s As String)
            _hasChanges = False
        End Sub

        ''' <summary>
        ''' Returns true
        ''' </summary>
        Public Function SupportsDeleteBytes() As Boolean Implements Forms.IByteProvider.SupportsDeleteBytes
            Return True
        End Function

        ''' <summary>
        ''' Returns true
        ''' </summary>
        Public Function SupportsInsertBytes() As Boolean Implements Forms.IByteProvider.SupportsInsertBytes
            Return True
        End Function

        ''' <summary>
        ''' Returns true
        ''' </summary>
        Public Function SupportsWriteByte() As Boolean Implements Forms.IByteProvider.SupportsWriteByte
            Return True
        End Function

        ''' <summary>
        ''' Write a byte into the byte collection.
        ''' </summary>
        ''' <paramname="index">the index of the byte to write.</param>
        ''' <paramname="value">the byte</param>
        Public Sub WriteByte(index As Long, value As Byte) Implements Forms.IByteProvider.WriteByte
            _bytes(index) = value
            OnChanged(EventArgs.Empty)
        End Sub
#End Region

    End Class
End Namespace
