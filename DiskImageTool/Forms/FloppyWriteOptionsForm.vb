Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Public Class FloppyWriteOptionsForm
    Public Structure FloppyWriteOptions
        Dim Format As Boolean
        Dim Verify As Boolean
        Dim Cancelled As Boolean
    End Structure

    Private _WriteOptions As FloppyWriteOptions
    Private _Drive As FloppyDriveEnum
    Private _FloppyDrive As FloppyInterface

    Public Sub New(FloppyDrive As FloppyInterface, Drive As FloppyDriveEnum, DiskFormat As FloppyDiskFormat)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()

        _FloppyDrive = FloppyDrive
        _Drive = Drive

        _WriteOptions.Cancelled = True

        Dim ImageFormatName = String.Format(My.Resources.Label_Floppy, FloppyDiskFormatGetName(DiskFormat))

        lblImageType.Text = ImageFormatName
        CheckFormat.Checked = True
        CheckVerify.Checked = True

        ResizeForm()
    End Sub

    Private Sub ResizeForm()
        Dim ColWidthLeft = Math.Max(lblImageTypeLabel.Width, CheckFormat.Width)
        Dim ColWidthRight = Math.Max(lblImageType.Width, CheckVerify.Width)

        lblImageType.Left = lblImageTypeLabel.Left + ColWidthLeft + 6
        CheckVerify.Left = CheckFormat.Left + ColWidthLeft + 6

        GroupBox1.Width = lblImageType.Left + ColWidthRight + 15
        GroupBox2.Width = GroupBox1.Width
    End Sub

    Private Sub LocalizeForm()
        BtnCancel.Text = My.Resources.Menu_Cancel
        BtnOK.Text = My.Resources.Menu_Ok
        GroupBox1.Text = My.Resources.Label_Options
        lblImageTypeLabel.Text = My.Resources.Label_ImageType & ":"
        CheckVerify.Text = My.Resources.Label_VerifyWrites
        CheckFormat.Text = My.Resources.Label_FormatDisk
        Me.Text = My.Resources.Label_DiskWriteOptions
    End Sub

    Public ReadOnly Property WriteOptions As FloppyWriteOptions
        Get
            Return _WriteOptions
        End Get
    End Property

    Public Shared Function Display(FloppyDrive As FloppyInterface, Drive As FloppyDriveEnum, DiskFormat As FloppyDiskFormat) As FloppyWriteOptions
        Using dlg As New FloppyWriteOptionsForm(FloppyDrive, Drive, DiskFormat)
            dlg.ShowDialog(App.CurrentFormInstance)

            Return dlg.WriteOptions
        End Using
    End Function

    Private Function CheckDisk() As Boolean
        Dim DriveLetter = FloppyInterface.GetDriveLetter(_Drive)
        Dim Probe = _FloppyDrive.ProbeMedia()

        Select Case Probe.State
            Case FloppyMediaState.DriveUnavailable
                MsgBox(My.Resources.Dialog_DiskWriteError, MsgBoxStyle.Exclamation)
                Return False
            Case FloppyMediaState.NoDisk
                MsgBox(String.Format(My.Resources.Dialog_FloppyNoDisk, DriveLetter, Environment.NewLine), MsgBoxStyle.Exclamation)
                Return False
            Case FloppyMediaState.Blank
                'CheckFormat.Checked = True
                Return True
            Case Else
                Dim Msg = String.Format(My.Resources.Dialog_DiskNotEmptyWarning, DriveLetter, Environment.NewLine)

                Dim MsgBoxResult = MsgBox(Msg, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkCancel Or MsgBoxStyle.DefaultButton2)

                Return MsgBoxResult = MsgBoxResult.Ok
        End Select
    End Function

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        If Not _FloppyDrive.OpenWrite(_Drive) Then
            MsgBox(My.Resources.Dialog_DiskWriteError, MsgBoxStyle.Exclamation)
            Me.DialogResult = DialogResult.None
            Exit Sub
        End If

        If Not CheckDisk() Then
            _FloppyDrive.Close()
            Me.DialogResult = DialogResult.None
            Exit Sub
        End If

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