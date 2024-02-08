Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Public Class FloppyWriteOptionsForm
    Public Structure FloppyWriteOptions
        Dim Format As Boolean
        Dim Verify As Boolean
        Dim Cancelled As Boolean
    End Structure

    Private _WriteOptions As FloppyWriteOptions

    Public Sub New(DoFormat As Boolean, DetectedType As FloppyDiskType, ImageType As FloppyDiskType)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _WriteOptions.Cancelled = True
        Dim ImageTypeName = GetFloppyDiskTypeName(ImageType) & " Floppy"

        Dim DetectedTypeName As String
        If DetectedType = -2 Then
            DetectedTypeName = "Unformatted"
        ElseIf DetectedType = -1 Then
            DetectedTypeName = "Unknown"
        Else
            DetectedTypeName = GetFloppyDiskTypeName(DetectedType) & " Floppy"
        End If
        lblImageType.Text = ImageTypeName
        lblDiskFormat.Text = DetectedTypeName
        CheckFormat.Checked = DoFormat
        CheckVerify.Checked = True
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