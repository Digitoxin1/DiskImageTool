Class SelectablePanel
    Inherits Panel

    Public Sub New()
        Me.SetStyle(ControlStyles.Selectable, True)
        Me.TabStop = True
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        Me.Focus()
        MyBase.OnMouseDown(e)
    End Sub

    Protected Overrides Function IsInputKey(ByVal keyData As Keys) As Boolean
        If keyData = Keys.Up OrElse keyData = Keys.Down Then Return True
        If keyData = Keys.Left OrElse keyData = Keys.Right Then Return True
        Return MyBase.IsInputKey(keyData)
    End Function

    Protected Overrides Sub OnEnter(ByVal e As EventArgs)
        Me.Invalidate()
        MyBase.OnEnter(e)
    End Sub

    Protected Overrides Sub OnLeave(ByVal e As EventArgs)
        Me.Invalidate()
        MyBase.OnLeave(e)
    End Sub

    'Protected Overrides Sub OnPaint(ByVal pe As PaintEventArgs)
    '    MyBase.OnPaint(pe)

    '    If Me.Focused Then
    '        Dim rc = Me.ClientRectangle
    '        rc.Inflate(-2, -2)
    '        ControlPaint.DrawFocusRectangle(pe.Graphics, rc)
    '    End If
    'End Sub
End Class