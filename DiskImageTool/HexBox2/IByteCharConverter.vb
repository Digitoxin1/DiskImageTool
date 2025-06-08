Namespace Hb.Windows.Forms
    ''' <summary>
    ''' The interface for objects that can translate between characters and bytes.
    ''' </summary>
    Public Interface IByteCharConverter
        ''' <summary>
        ''' Returns the byte to use when the character passed across is entered during editing.
        ''' </summary>
        ''' <paramname="c"></param>
        ''' <returns></returns>
        Function ToByte(c As Char) As Byte

        ''' <summary>
        ''' Returns the character to display for the byte passed across.
        ''' </summary>
        ''' <paramname="b"></param>
        ''' <returns></returns>
        Function ToChar(b As Byte) As Char
    End Interface
End Namespace
