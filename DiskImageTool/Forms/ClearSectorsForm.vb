Public Class ClearSectorsForm
    Public Structure ClearSectorsFormResult
        Dim FillChar As Byte
        Dim Cancelled As Boolean
    End Structure

    Private _Result As ClearSectorsFormResult

    Public Sub New(Caption As String, Title As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LblCaption.Text = Caption
        Me.Text = Title
        _Result.Cancelled = True
        _Result.FillChar = &H0
    End Sub

    Public ReadOnly Property Result As ClearSectorsFormResult
        Get
            Return _Result
        End Get
    End Property

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        If RadioFillF6.Checked Then
            _Result.FillChar = &HF6
        Else
            _Result.FillChar = &H0
        End If
        _Result.Cancelled = False
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        If RadioFillF6.Checked Then
            _Result.FillChar = &HF6
        Else
            _Result.FillChar = &H0
        End If
        _Result.Cancelled = True
    End Sub

    Private Sub ClearSectorsForm_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        e.Graphics.DrawImage(SystemIcons.Question.ToBitmap, 22, 24)
    End Sub
End Class