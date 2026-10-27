Public Class ReadDiskDestination
    Private Sub ReadDiskDestinationFlux_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        If Me.Height <> 153 Then
            Me.Height = 153
        End If
    End Sub

    Public Sub ToggleView(DisplayFlux As Boolean)
        GroupBoxDestination.SuspendLayout()
        TableLayoutPanelDestinationFlux.Visible = DisplayFlux
        TableLayoutPanelDestinationImage.Visible = Not DisplayFlux
        GroupBoxDestination.ResumeLayout()
    End Sub
End Class
