Imports System.Text

Namespace Hb.Windows.Forms
    ''' <summary>
    ''' A byte char provider that can translate bytes encoded in selected codepage.
    ''' Class initializes with code page identifier.
    ''' </summary>
    Public Class CodePageByteCharProvider
        Implements IByteCharConverter

        Private Shared ddd As Integer
        Private ReadOnly _ebcdicEncoding As Encoding = Encoding.GetEncoding(ddd)

        ''' <summary>
        ''' Code page encoding. Note that some code pages is not always supported by .NET,
        ''' the underlying platform has to provide support for it.
        ''' </summary>
        Public Sub New(cp As Integer)
            ddd = cp
        End Sub

        ''' <summary>
        ''' Returns the byte corresponding to the EBCDIC character passed across.
        ''' </summary>
        ''' <paramname="c"></param>
        ''' <returns></returns>
        Public Overridable Function ToByte(c As Char) As Byte Implements IByteCharConverter.ToByte
            Dim decoded = _ebcdicEncoding.GetBytes(New Char() {c})
            Return If(decoded.Length > 0, decoded(0), CByte(0))
        End Function

        ''' <summary>
        ''' Returns the EBCDIC character corresponding to the byte passed across.
        ''' </summary>
        ''' <paramname="b"></param>
        ''' <returns></returns>
        Public Overridable Function ToChar(b As Byte) As Char Implements IByteCharConverter.ToChar
            Dim encoded = _ebcdicEncoding.GetString(New Byte() {b})
            Return If(encoded.Length > 0, encoded(0), "."c)
        End Function

        ''' <summary>
        ''' Returns a description of the byte char provider.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return "Select Code Page"
        End Function
    End Class

    ''' <summary>
    ''' Creates .net codepage name, display name and identifiers list. List is one byte character code pages
    ''' </summary>
    Public Class CodePageNames
        ''' <summary>
        ''' This is .net codepage display names list. Shows long names, ist type string
        ''' </summary>
        Public displayNames As New List(Of String)()

        ''' <summary>
        ''' This is .net codepage identifiers list values as integer.
        ''' </summary>
        Public numbers As New List(Of Integer)()

        ''' <summary>
        ''' This is .net codepage names list. List type string
        ''' </summary>
        Public pages As New List(Of String)()

        ''' <summary>
        ''' List Creating in initalization state.
        ''' </summary>
        Public Sub New()
            For Each ei In Encoding.GetEncodings()
                Dim e As Encoding = ei.GetEncoding()
                If e.IsSingleByte Then ' Only one byte code pages
                    pages.Add(ei.Name)
                    numbers.Add(ei.CodePage)
                    'added at version 2.0.1
                    displayNames.Add(ei.DisplayName)
                End If
            Next
        End Sub
    End Class

    ''' <summary>
    ''' The default <seecref="IByteCharConverter"/> implementation.
    ''' </summary>
    Public Class DefaultByteCharConverter
        Implements IByteCharConverter

        ''' <summary>
        ''' Returns the byte to use for the character passed across.
        ''' </summary>
        ''' <paramname="c"></param>
        ''' <returns></returns>
        Public Overridable Function ToByte(c As Char) As Byte Implements IByteCharConverter.ToByte
            Return Microsoft.VisualBasic.AscW(c)
        End Function

        ''' <summary>
        ''' Returns the character to display for the byte passed across.
        ''' </summary>
        ''' <paramname="b"></param>
        ''' <returns></returns>
        Public Overridable Function ToChar(b As Byte) As Char Implements IByteCharConverter.ToChar
            Return If(b > &H1F AndAlso Not (b > &H7E AndAlso b < &HA0), Microsoft.VisualBasic.ChrW(b), "."c)
        End Function

        ''' <summary>
        ''' Returns a description of the byte char provider.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return "ANSI (Default)"
        End Function
    End Class
End Namespace
