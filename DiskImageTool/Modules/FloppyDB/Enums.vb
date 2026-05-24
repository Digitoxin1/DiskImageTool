Namespace FloppyDB
    Public Module Enums
        Public Enum FloppyDBStatus As Byte
            Unknown
            Unverified
            Verified
            Modified
        End Enum

        Public Function GetFloppyDBStatus(Status As String) As FloppyDBStatus
            Select Case Status
                Case "V"
                    Return FloppyDBStatus.Verified
                Case "U"
                    Return FloppyDBStatus.Unverified
                Case "M"
                    Return FloppyDBStatus.Modified
                Case Else
                    Return FloppyDBStatus.Unknown
            End Select
        End Function
    End Module
End Namespace
