Public Class ImagePreview
    Private ReadOnly _Disk As DiskImage.Disk
    Private ReadOnly _SummaryPanel As SummaryPanel
    Private WithEvents FilePanelMain As FilePanel
    Public Sub New(Disk As DiskImage.Disk, FullDisplay As Boolean, Caption As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Disk = Disk
        _SummaryPanel = New SummaryPanel(ListViewSummary)
        FilePanelMain = New FilePanel(ListViewFiles, True, FullDisplay)

        Me.Text = My.Resources.Label_ImagePreview
        If Not String.IsNullOrEmpty(Caption) Then
            Me.Text &= " - " & Caption
        End If

        Dim CurrentImage = New DiskImageContainer(_Disk, New ImageData(""))

        Dim MD5 As String = ""
        If FullDisplay Then
            MD5 = _Disk.Image.GetMD5Hash
        Else
            Me.Width = Me.MinimumSize.Width
        End If

        OKButton.Text = My.Resources.Menu_Ok

        PanelSpacer.Visible = FullDisplay
        HashPanel1.Visible = FullDisplay

        _SummaryPanel.Populate(CurrentImage, App.BootstrapDB, App.TitleDB, MD5, Not FullDisplay)
        If FullDisplay Then
            HashPanel1.Populate(_Disk, MD5)
        End If
        FilePanelMain.Load(CurrentImage, False, FullDisplay)
    End Sub

    Public Shared Function Display(FileName As String, ImageParams As DiskImage.FloppyDiskParams, Caption As String, owner As IWin32Window) As Boolean
        Dim OpenResponse = FileOpenBinary(FileName)

        If Not OpenResponse.Result Then
            Dim Msg = My.Resources.Dialog_ImagePreviewFail & vbNewLine & vbNewLine & OpenResponse.ErrorMsg
            MsgBox(Msg, MsgBoxStyle.Exclamation)

            Return False
        End If

        Try
            Dim Size = ImageParams.BPBParams.SizeInBytes

            Dim Buffer(Size - 1) As Byte

            For i As Integer = 0 To Buffer.Length - 1
                Buffer(i) = &HF6
            Next

            Dim ToCopy As Integer = Math.Min(OpenResponse.Data.Length, Buffer.Length)
            Array.Copy(OpenResponse.Data, 0, Buffer, 0, ToCopy)

            Dim FloppyImage As New DiskImage.BasicSectorImage(Buffer)
            Dim Disk As New DiskImage.Disk(FloppyImage, 0)

            Display(Disk, False, Caption, owner)
        Catch ex As Exception
            MsgBox(My.Resources.Dialog_ImagePreviewFail, MsgBoxStyle.Exclamation)

            Return False
        End Try

        Return True
    End Function

    Public Shared Function Display(ImageData As ImageData, Caption As String, owner As IWin32Window) As Boolean
        Dim Disk = DiskImageLoadFromImageData(ImageData)

        If Disk Is Nothing Then
            MsgBox(My.Resources.Dialog_ImagePreviewFail, MsgBoxStyle.Exclamation)
            Return False
        End If

        Display(Disk, True, Caption, owner)

        Return True
    End Function

    Public Shared Sub Display(Disk As DiskImage.Disk, FullDisplay As Boolean, Caption As String, owner As IWin32Window)
        Using Dialog As New ImagePreview(Disk, FullDisplay, Caption)
            Dialog.ShowDialog(owner)
        End Using
    End Sub
End Class