Imports System.Text

Namespace Hb.Windows.Forms
    ''' <summary>
    ''' Defines the type of the Find operation.
    ''' </summary>
    Public Enum FindType
        ''' <summary>
        ''' Used for Text Find operations
        ''' </summary>
        Text

        ''' <summary>
        ''' Used for Hex Find operations
        ''' </summary>
        Hex
    End Enum

    ''' <summary>
    ''' Defines all state information nee
    ''' </summary>
    Public Class FindOptions
        ''' <summary>
        ''' Gets the Find buffer used for case insensitive Find operations. This is the binary representation of Text.
        ''' </summary>
        Private _FindBuffer As Byte()

        ''' <summary>
        ''' Gets the Find buffer used for case sensitive Find operations. This is the binary representation of Text in lower case format.
        ''' </summary>
        Private _FindBufferLowerCase As Byte()

        ''' <summary>
        ''' Gets the Find buffer used for case sensitive Find operations. This is the binary representation of Text in upper case format.
        ''' </summary>        
        Private _FindBufferUpperCase As Byte()

        ''' <summary>
        ''' Contains the MatchCase value
        ''' </summary>
        Private _matchCase As Boolean

        ''' <summary>
        ''' Contains the text that should be found.
        ''' </summary>
        Private _text As String

        ''' <summary>
        ''' Gets or sets the hex buffer that should be found. Only used, when Type is FindType.Hex.
        ''' </summary>
        Public Property Hex As Byte()

        ''' <summary>
        ''' Gets or sets whether the Find options are valid
        ''' </summary>
        Public Property IsValid As Boolean

        ''' <summary>
        ''' Gets or sets the value, whether the Find operation is case sensitive or not.
        ''' </summary>
        Public Property MatchCase As Boolean
            Get
                Return _matchCase
            End Get
            Set(value As Boolean)
                _matchCase = value
                UpdateFindBuffer()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the text that should be found. Only used, when Type is FindType.Hex.
        ''' </summary>
        Public Property Text As String
            Get
                Return _text
            End Get
            Set(value As String)
                _text = value
                UpdateFindBuffer()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the type what should be searched.
        ''' </summary>
        Public Property Type As FindType

        Friend Property FindBuffer As Byte()
            Get
                Return _FindBuffer
            End Get
            Private Set(value As Byte())
                _FindBuffer = value
            End Set
        End Property

        Friend Property FindBufferLowerCase As Byte()
            Get
                Return _FindBufferLowerCase
            End Get
            Private Set(value As Byte())
                _FindBufferLowerCase = value
            End Set
        End Property

        Friend Property FindBufferUpperCase As Byte()
            Get
                Return _FindBufferUpperCase
            End Get
            Private Set(value As Byte())
                _FindBufferUpperCase = value
            End Set
        End Property

        ''' <summary>
        ''' Updates the find buffer.
        ''' </summary>
        Private Sub UpdateFindBuffer()
            Dim text = If(Not Equals(Me.Text, Nothing), Me.Text, String.Empty)
            FindBuffer = Encoding.ASCII.GetBytes(text)
            FindBufferLowerCase = Encoding.ASCII.GetBytes(text.ToLower())
            FindBufferUpperCase = Encoding.ASCII.GetBytes(text.ToUpper())
        End Sub
    End Class
End Namespace
