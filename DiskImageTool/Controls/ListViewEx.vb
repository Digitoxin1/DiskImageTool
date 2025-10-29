Public Class ListViewEx
    Inherits ListView

    Public Event ItemSelectionEnd As EventHandler
    Private _pending As Boolean

    Protected Overrides Sub OnItemSelectionChanged(e As ListViewItemSelectionChangedEventArgs)
        MyBase.OnItemSelectionChanged(e)
        If Not _pending Then
            _pending = True
            BeginInvoke(New Action(
                Sub()
                    _pending = False
                    RaiseEvent ItemSelectionEnd(Me, EventArgs.Empty)
                End Sub))
        End If
    End Sub
End Class
