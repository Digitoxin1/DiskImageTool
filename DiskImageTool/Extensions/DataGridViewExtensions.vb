Imports System.Runtime.CompilerServices

Module DataGridViewExtensions
    <Extension()>
    Public Sub DoubleBuffer(DataGridViewControl As DataGridView)
        DataGridViewControl.GetType() _
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic) _
            .SetValue(DataGridViewControl, True, Nothing)
    End Sub
End Module
