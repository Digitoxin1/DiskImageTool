Imports System.Runtime.Remoting.Messaging

Namespace ImageFormats
    Namespace IMD
        Public Class IMDSector
            Public Sub New(Size As UShort)
                _Data = New Byte(Size - 1) {}
            End Sub

            Public Property SectorId As Byte
            Public Property Cylinder As Byte
            Public Property Head As Byte
            Public Property Data As Byte()
            Public Property Unavailable As Boolean
            Public Property Compressed As Boolean
            Public Property CompressedValue As Boolean
            Public Property Deleted As Boolean
            Public Property ChecksumError As Boolean

            Public Function GetFormat() As DataFormat
                If _Unavailable Then
                    Return DataFormat.Unavailable
                ElseIf _Compressed And _Deleted And _ChecksumError Then
                    Return DataFormat.DeletedErrorCompressed
                ElseIf _Compressed And _Deleted Then
                    Return DataFormat.DeletedCompressed
                ElseIf _Compressed And _ChecksumError Then
                    Return DataFormat.NormalErrorCompressed
                ElseIf _Compressed Then
                    Return DataFormat.NormalCompressed
                ElseIf _Deleted And _ChecksumError Then
                    Return DataFormat.DeletedError
                ElseIf _Deleted Then
                    Return DataFormat.Deleted
                ElseIf _ChecksumError Then
                    Return DataFormat.NormalError
                Else
                    Return DataFormat.Normal
                End If
            End Function

        End Class
    End Namespace
End Namespace
