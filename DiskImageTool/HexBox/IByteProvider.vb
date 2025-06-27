Namespace Hb.Windows.Forms
    ''' <summary>
    ''' Defines a byte provider for HexBox control
    ''' </summary>
    Public Interface IByteProvider
        ''' <summary>
        ''' Occurs, when bytes are changed.
        ''' </summary>
        Event Changed As EventHandler

        ''' <summary>
        ''' Occurs, when the Length property changed.
        ''' </summary>
        Event LengthChanged As EventHandler

        ''' <summary>
        ''' Returns the total length of bytes the byte provider is providing.
        ''' </summary>
        ReadOnly Property Length As Long

        ''' <summary>
        ''' Applies changes.
        ''' </summary>
        Sub ApplyChanges()

        ''' <summary>
        ''' Deletes bytes from the provider
        ''' </summary>
        ''' <paramname="index">the start index of the bytes to delete</param>
        ''' <paramname="length">the length of the bytes to delete</param>
        ''' <remarks>This method must raise the LengthChanged event.</remarks>
        Sub DeleteBytes(index As Long, length As Long)

        ''' <summary>
        ''' True, when changes are done.
        ''' </summary>
        Function HasChanges() As Boolean

        ''' <summary>
        ''' Inserts bytes into the provider
        ''' </summary>
        ''' <paramname="index"></param>
        ''' <paramname="bs"></param>
        ''' <remarks>This method must raise the LengthChanged event.</remarks>
        Sub InsertBytes(index As Long, bs As Byte())

        ''' <summary>
        ''' Reads a byte from the provider
        ''' </summary>
        ''' <paramname="index">the index of the byte to read</param>
        ''' <returns>the byte to read</returns>
        Function ReadByte(index As Long) As Byte

        ''' <summary>
        ''' Returns a value if the DeleteBytes methods is supported by the provider.
        ''' </summary>
        ''' <returns>True, when it´s supported.</returns>
        Function SupportsDeleteBytes() As Boolean

        ''' <summary>
        ''' Returns a value if the InsertBytes methods is supported by the provider.
        ''' </summary>
        ''' <returns>True, when it´s supported.</returns>
        Function SupportsInsertBytes() As Boolean

        ''' <summary>
        ''' Returns a value if the WriteByte methods is supported by the provider.
        ''' </summary>
        ''' <returns>True, when it´s supported.</returns>
        Function SupportsWriteByte() As Boolean

        ''' <summary>
        ''' Writes a byte into the provider
        ''' </summary>
        ''' <paramname="index">the index of the byte to write</param>
        ''' <paramname="value">the byte to write</param>
        Sub WriteByte(index As Long, value As Byte)
    End Interface
End Namespace
