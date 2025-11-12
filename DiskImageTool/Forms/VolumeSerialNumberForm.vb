Public Class VolumeSerialNumberForm
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Function GetValue() As Date
        Return New Date(DTDate.Value.Year, DTDate.Value.Month, DTDate.Value.Day, DTTime.Value.Hour, DTTime.Value.Minute, DTTime.Value.Second, CInt(NumMS.Value))
    End Function

    Private Sub VolumeSerialNumberForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        DTDate.Value = Date.Today
        DTTime.Value = Date.Now
        NumMS.Value = Date.Now.Millisecond
    End Sub
End Class