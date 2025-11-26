Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Public Class FloppyWriteOptionsForm
    Public Structure FloppyWriteOptions
        Dim Format As Boolean
        Dim Verify As Boolean
        Dim Cancelled As Boolean
    End Structure

    Private _WriteOptions As FloppyWriteOptions

    Public Sub New(DoFormat As Boolean, DetectedFormat As FloppyDiskFormat, DiskFormat As FloppyDiskFormat)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()
        _WriteOptions.Cancelled = True
        Dim ImageFormatName = String.Format(My.Resources.Label_Floppy, FloppyDiskFormatGetName(DiskFormat))

        Dim DetectedFormatName As String
        If DetectedFormat = -2 Then
            DetectedFormatName = My.Resources.Label_Unformatted
        ElseIf DetectedFormat = -1 Then
            DetectedFormatName = My.Resources.Label_Unknown
        Else
            DetectedFormatName = String.Format(My.Resources.Label_Floppy, FloppyDiskFormatGetName(DetectedFormat))
        End If
        lblImageType.Text = ImageFormatName
        lblDiskFormat.Text = DetectedFormatName
        CheckFormat.Checked = DoFormat
        CheckVerify.Checked = True
    End Sub

    Private Sub LocalizeForm()
        BtnCancel.Text = My.Resources.Menu_Cancel
        BtnOK.Text = My.Resources.Menu_Ok
        GroupBox1.Text = My.Resources.Label_Options
        Label2.Text = My.Resources.Label_ImageType & ":"
        Label1.Text = My.Resources.Label_DiskFormat & ":"
    End Sub

    Public ReadOnly Property WriteOptions As FloppyWriteOptions
        Get
            Return _WriteOptions
        End Get
    End Property

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        _WriteOptions.Format = CheckFormat.Checked
        _WriteOptions.Verify = CheckVerify.Checked
        _WriteOptions.Cancelled = False
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        _WriteOptions.Format = CheckFormat.Checked
        _WriteOptions.Verify = CheckVerify.Checked
        _WriteOptions.Cancelled = True
    End Sub


End Class