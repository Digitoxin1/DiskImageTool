Imports System.Runtime.CompilerServices

Module TableLayoutExtensions
    <Extension>
    Public Sub AddWithSpan(controls As TableLayoutControlCollection, control As Control, column As Integer, row As Integer, columnSpan As Integer)
        controls.Add(control, column, row)

        Dim panel = DirectCast(control.Parent, TableLayoutPanel)
        panel.SetColumnSpan(control, columnSpan)
    End Sub

End Module
