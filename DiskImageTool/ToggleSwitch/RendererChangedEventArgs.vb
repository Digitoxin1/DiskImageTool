Imports DiskImageTool.JCS

Namespace ToggleSwitch
    Public Class BeforeRenderingEventArgs
        Public Property Renderer As ToggleSwitchRendererBase

        Public Sub New(ByVal renderer As ToggleSwitchRendererBase)
            Me.Renderer = renderer
        End Sub
    End Class
End Namespace
