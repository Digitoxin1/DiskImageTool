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

        Me.Text = My.Resources.Label_ImagePreview & " - " & Caption

        Dim CurrentImage = New DiskImageContainer(_Disk, New ImageData(""))

        Dim MD5 As String = ""
        If FullDisplay Then
            MD5 = CurrentImage.Disk.Image.GetMD5Hash
        Else
            Me.Width = Me.MinimumSize.Width
        End If

        OKButton.Text = My.Resources.Menu_Ok

        PanelSpacer.Visible = FullDisplay
        HashPanel1.Visible = FullDisplay

        _SummaryPanel.Populate(CurrentImage, App.BootstrapDB, App.TitleDB, MD5, Not FullDisplay)
        If FullDisplay Then
            HashPanel1.Populate(Disk, MD5)
        End If
        FilePanelMain.Load(CurrentImage, False, FullDisplay)
    End Sub

    Public Shared Function Display(FileName As String, ImageParams As DiskImage.FloppyDiskParams, Caption As String, owner As IWin32Window) As Boolean
        Try
            Dim Size = ImageParams.BPBParams.SizeInBytes

            Dim Buffer(Size - 1) As Byte

            For i As Integer = 0 To Buffer.Length - 1
                Buffer(i) = &HF6
            Next

            Dim FileBytes = IO.File.ReadAllBytes(FileName)

            Dim ToCopy As Integer = Math.Min(FileBytes.Length, Buffer.Length)
            Array.Copy(FileBytes, 0, Buffer, 0, ToCopy)

            Dim FloppyImage As New DiskImage.BasicSectorImage(Buffer)
            Dim Disk As New DiskImage.Disk(FloppyImage, 0)

            Display(Disk, False, Caption, owner)
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Public Shared Function Display(ImageData As ImageData, Caption As String, owner As IWin32Window) As Boolean
        Try
            Dim Disk = DiskImageLoadFromImageData(ImageData)

            Display(Disk, True, Caption, owner)
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Public Shared Sub Display(Disk As DiskImage.Disk, FullDisplay As Boolean, Caption As String, owner As IWin32Window)
        With New ImagePreview(Disk, FullDisplay, Caption)
            .ShowDialog(owner)
        End With
    End Sub
End Class