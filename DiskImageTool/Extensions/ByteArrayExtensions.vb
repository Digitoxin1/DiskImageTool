Imports System.Runtime.CompilerServices

Module ByteArrayExtensions
    <Extension()>
    Public Function CompareTo(b1() As Byte, b2() As Byte, Optional IgnoreLength As Boolean = False) As Boolean
        If Not IgnoreLength AndAlso b1.Length <> b2.Length Then
            Return False
        End If

        Dim Length = Math.Min(b1.Length, b2.Length)

        For Counter = Length - 1 To 0 Step -1
            If b1(Counter) <> b2(Counter) Then
                Return False
            End If
        Next

        Return True
    End Function
End Module
