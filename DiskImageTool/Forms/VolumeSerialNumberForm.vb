Public Class VolumeSerialNumberForm
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()
    End Sub

    Public Shared Function Display() As (Result As Boolean, Value As Date)
        Using dlg As New VolumeSerialNumberForm()
            dlg.ShowDialog(App.CurrentFormInstance)

            Return (dlg.DialogResult = DialogResult.OK, dlg.GetValue())
        End Using
    End Function

    Public Function GetValue() As Date
        Return New Date(DTDate.Value.Year, DTDate.Value.Month, DTDate.Value.Day, DTTime.Value.Hour, DTTime.Value.Minute, DTTime.Value.Second, CInt(NumMS.Value))
    End Function

    Private Sub LocalizeForm()
        BtnCancel.Text = My.Resources.Menu_Cancel
        BtnOK.Text = My.Resources.Menu_Ok
        LabelDate.Text = My.Resources.Label_Date
        LabelMilliseconds.Text = My.Resources.Label_Milliseconds
        LabelTime.Text = My.Resources.Label_Time
        Me.Text = My.Resources.Caption_GenerateVolumeSerialNumber
    End Sub

    Private Sub VolumeSerialNumberForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        DTDate.Value = Date.Today
        DTTime.Value = Date.Now
        NumMS.Value = Date.Now.Millisecond
    End Sub
End Class