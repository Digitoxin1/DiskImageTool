Imports System.Security.Cryptography

''' <summary>
''' HashAlgorithm implementation for CRC-32.
''' </summary>
Public Class CRC32Hash
    Inherits HashAlgorithm
    ' Shared, pre-computed lookup table for efficiency
    Private Shared ReadOnly _crc32Table As UInteger()

    ' Current hash value
    Private _crc32Value As UInteger

    ' True if HashCore has been called
    Private _hashCoreCalled As Boolean

    ' True if HashFinal has been called
    Private _hashFinalCalled As Boolean

    Overloads Shared Function Create() As CRC32Hash
        Return New CRC32Hash()
    End Function

    ''' <summary>
    ''' Initializes the shared lookup table.
    ''' </summary>
    Shared Sub New()
        Dim poly As UInteger = &HEDB88320UI
        _crc32Table = New UInteger(255) {}
        Dim temp As UInteger
        For i As UInteger = 0 To _crc32Table.Length - 1
            temp = i
            For j As Integer = 8 To 1 Step -1
                If (temp And 1) = 1 Then
                    temp = CUInt((temp >> 1) Xor poly)
                Else
                    temp >>= 1
                End If
            Next
            _crc32Table(i) = temp
        Next
    End Sub
    ''' <summary>
    ''' Initializes a new instance.
    ''' </summary>
    Public Sub New()
        InitializeVariables()
    End Sub

    ''' <summary>
    ''' Returns the hash as an array of bytes.
    ''' </summary>
    Public Overrides ReadOnly Property Hash As Byte()
        Get
            If Not _hashCoreCalled Then
                Throw New NullReferenceException()
            End If

            If Not _hashFinalCalled Then
                ' Note: Not CryptographicUnexpectedOperationException because
                ' that can't be instantiated on Silverlight 4
                Throw New CryptographicException("Hash must be finalized before the hash value is retrieved.")
            End If

            ' Convert complement of hash code to byte array
            Dim bytes = BitConverter.GetBytes(Not _crc32Value)

            ' Reverse for proper endianness, and return
            Array.Reverse(bytes)
            Return bytes
        End Get
    End Property

    ' Return size of hash in bits.
    Public Overrides ReadOnly Property HashSize As Integer
        Get
            Return 32
        End Get
    End Property

    ''' <summary>
    ''' Initializes internal state.
    ''' </summary>
    Public Overrides Sub Initialize()
        InitializeVariables()
    End Sub

    ''' <summary>
    ''' Updates the hash code for the provided data.
    ''' </summary>
    ''' <paramname="array">Data.</param>
    ''' <paramname="ibStart">Start position.</param>
    ''' <paramname="cbSize">Number of bytes.</param>
    Protected Overrides Sub HashCore(array As Byte(), ibStart As Integer, cbSize As Integer)
        If array Is Nothing Then
            Throw New ArgumentNullException("array")
        End If

        If _hashFinalCalled Then
            Throw New CryptographicException("Hash not valid for use in specified state.")
        End If

        _hashCoreCalled = True

        For i As Integer = ibStart To ibStart + cbSize - 1
            Dim index As Byte = CByte(((_crc32Value) And &HFF) Xor array(i))
            _crc32Value = CUInt((_crc32Value >> 8) Xor _crc32Table(index))
        Next
    End Sub

    ''' <summary>
    ''' Finalizes the hash code and returns it.
    ''' </summary>
    ''' <returns></returns>
    Protected Overrides Function HashFinal() As Byte()
        _hashFinalCalled = True

        Return Hash
    End Function

    ''' <summary>
    ''' Initializes variables.
    ''' </summary>
    Private Sub InitializeVariables()
        _crc32Value = &HFFFFFFFFUI
        _hashCoreCalled = False
        _hashFinalCalled = False
    End Sub
End Class
