Imports System.Runtime.CompilerServices

Module StringExtensions
    <Extension()>
    Public Function Pluralize(Value As String, Count As Integer) As String
        Return Value & IIf(Count <> 1, "s", "")
    End Function
End Module
