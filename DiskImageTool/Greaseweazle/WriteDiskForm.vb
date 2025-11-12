Namespace Greaseweazle
    Public Class WriteDiskForm
        Inherits BaseForm
        Private WithEvents ButtonProcess As Button
        Private WithEvents ComboImageDrives As ComboBox
        Private WithEvents NumericRetries As NumericUpDown
        Private WithEvents CheckBoxPreErase As CheckBox
        Private WithEvents Process As ConsoleProcessRunner
        Private ReadOnly _DiskImage As DiskImageContainer
        Private _ProcessRunning As Boolean = False
        Private ReadOnly _TrackCount As UShort
        Private ReadOnly _SideCount As Byte
        Private ReadOnly _TrackStatus As Dictionary(Of String, ConsoleOutputParser.WriteTrackInfo)

        Public Sub New(DiskImage As DiskImageContainer)
            MyBase.New()

            InitializeControls()

            _DiskImage = DiskImage
            _TrackStatus = New Dictionary(Of String, ConsoleOutputParser.WriteTrackInfo)

            Process = New ConsoleProcessRunner With {
                .EventContext = Threading.SynchronizationContext.Current
            }

            Me.Text = "Write Disk - " & DiskImage.ImageData.FileName

            If DiskImage.Disk.Image.IsBitstreamImage Then
                _TrackCount = DiskImage.Disk.Image.TrackCount
                _SideCount = DiskImage.Disk.Image.SideCount
            Else
                _TrackCount = DiskImage.Disk.BPB.SectorToTrack(DiskImage.Disk.BPB.SectorCount)
                _SideCount = DiskImage.Disk.BPB.NumberOfHeads
            End If

            NumericRetries.Value = CommandLineBuilder.DEFAULT_RETRIES

            TrackGridInit(_TrackCount, _SideCount)
            PopulateDrives(DiskImage.Disk.DiskFormat)
            ResetStatusBar()
        End Sub

        Private Sub InitializeControls()
            Dim DriveLabel = New Label With {
                .Text = "Drive",
                .Anchor = AnchorStyles.Left,
                .AutoSize = True
            }

            ComboImageDrives = New ComboBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Width = 180
            }

            Dim RetriesLabel = New Label With {
                .Text = "Retries",
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            NumericRetries = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 45,
                .Minimum = 0,
                .Maximum = 99
            }

            CheckBoxPreErase = New CheckBox With {
                .Text = "Pre-Erase",
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(12, 3, 3, 3)
            }

            ButtonProcess = New Button With {
                .Width = 75,
                .Margin = New Padding(12, 24, 3, 3),
                .Text = "Write"
            }

            ButtonOk.Visible = False

            With TableLayoutPanelMain
                .SuspendLayout()

                .RowCount = 2
                .ColumnCount = 7

                While .RowStyles.Count < .RowCount
                    .RowStyles.Add(New RowStyle())
                End While
                For i As Integer = 0 To .RowCount - 1
                    .RowStyles(i).SizeType = SizeType.AutoSize
                Next

                While .ColumnStyles.Count < .ColumnCount
                    .ColumnStyles.Add(New ColumnStyle())
                End While
                For j As Integer = 0 To .ColumnCount - 1
                    .ColumnStyles(j).SizeType = SizeType.AutoSize
                Next

                TableLayoutPanelMain.Controls.Add(TableSide0Outer, 0, 1)
                TableLayoutPanelMain.SetColumnSpan(TableSide0Outer, 2)

                TableLayoutPanelMain.Controls.Add(TableSide1Outer, 2, 1)
                TableLayoutPanelMain.SetColumnSpan(TableSide1Outer, 4)

                TableLayoutPanelMain.Controls.Add(DriveLabel, 0, 0)
                TableLayoutPanelMain.Controls.Add(ComboImageDrives, 1, 0)

                TableLayoutPanelMain.Controls.Add(RetriesLabel, 2, 0)
                TableLayoutPanelMain.Controls.Add(NumericRetries, 3, 0)

                TableLayoutPanelMain.Controls.Add(CheckBoxPreErase, 4, 0)

                TableLayoutPanelMain.Controls.Add(ButtonProcess, 6, 1)

                .ResumeLayout()
                .Left = (.Parent.ClientSize.Width - .Width) \ 2
            End With

        End Sub

        Private Sub RefreshButtonState()
            Dim Drive As String = ComboImageDrives.SelectedValue

            ComboImageDrives.Enabled = Not _ProcessRunning

            ButtonProcess.Enabled = Drive <> ""
            If _ProcessRunning Then
                ButtonProcess.Text = My.Resources.Label_Abort
            Else
                ButtonProcess.Text = "Write"
            End If
        End Sub

        Private Sub ToggleProcessRunning(Value As Boolean)
            _ProcessRunning = Value
            RefreshButtonState()
        End Sub

        Private Sub PopulateDrives(DiskFormat As DiskImage.FloppyDiskFormat)
            Dim IsShugart As Boolean = String.Equals(My.Settings.GW_Interface, "Shugart", StringComparison.OrdinalIgnoreCase)

            Dim Drive0Type = GreaseweazleFloppyTypeFromName(My.Settings.GW_DriveType0)
            Dim Drive1Type = GreaseweazleFloppyTypeFromName(My.Settings.GW_DriveType1)
            Dim Drive2Type = GreaseweazleFloppyTypeFromName(My.Settings.GW_DriveType2)

            Dim AvailableTypes = Drive0Type Or Drive1Type
            If IsShugart Then
                AvailableTypes = AvailableTypes Or Drive2Type
            End If

            Dim SelectedFormat = GreaseweazleFindCompatibleFloppyType(DiskFormat, AvailableTypes)

            Dim DriveList As New List(Of KeyValuePair(Of String, String)) From {
                New KeyValuePair(Of String, String)("Please Select", "")
            }

            Dim SelectedValue As String = Nothing

            Dim AddItem As Action(Of String, String, GreaseweazleFloppyType) =
                Sub(labelPrefix As String, value As String, t As GreaseweazleFloppyType)
                    If t <> GreaseweazleFloppyType.None Then
                        DriveList.Add(New KeyValuePair(Of String, String)(
                            labelPrefix & ":   " & GreaseweazleFloppyTypeDescription(t), value))
                        If SelectedValue Is Nothing AndAlso t = SelectedFormat Then
                            SelectedValue = value
                        End If
                    End If
                End Sub

            If IsShugart Then
                AddItem("DS0", "0", Drive0Type)
                AddItem("DS1", "1", Drive1Type)
                AddItem("DS2", "2", Drive2Type)
            Else
                AddItem("A", "A", Drive0Type)
                AddItem("B", "B", Drive1Type)
            End If

            InitializeCombo(ComboImageDrives, DriveList, SelectedValue)
        End Sub

        Private Function GetTrackInfo(Track As UShort, Side As Byte, Retry As Integer, Failed As Boolean) As ConsoleOutputParser.WriteTrackInfo
            Dim Key = Track & "." & Side

            Dim TrackInfo As ConsoleOutputParser.WriteTrackInfo

            If _TrackStatus.ContainsKey(Key) Then
                TrackInfo = _TrackStatus.Item(Key)
            Else
                TrackInfo = New ConsoleOutputParser.WriteTrackInfo With {
                    .SourceTrack = Track,
                    .SourceSide = Side
                }
                _TrackStatus.Add(Key, TrackInfo)
            End If

            TrackInfo.Retry = Math.Max(Retry, TrackInfo.Retry)
            If Failed Then
                TrackInfo.Failed = Failed
            End If

            Return TrackInfo
        End Function

        Private Sub ProcessTrackStatus(TrackInfo As ConsoleOutputParser.WriteTrackInfo)
            Dim Table As TableLayoutPanel

            If TrackInfo.SourceSide = 0 Then
                Table = TableSide0
            ElseIf TrackInfo.SourceSide = 1 Then
                Table = TableSide1
            Else
                Table = Nothing
            End If

            If Table IsNot Nothing Then

                Dim BackColor As Color
                Dim Text As String = ""

                If TrackInfo.Retry > 0 Then
                    Text = TrackInfo.Retry
                End If

                If TrackInfo.Failed Then
                    BackColor = Color.Red
                ElseIf TrackInfo.Retry > 0 Then
                    BackColor = Color.Yellow
                Else
                    BackColor = Color.LightGreen
                End If

                FloppyGridSetLabel(Table, TrackInfo.SourceTrack, Text, BackColor)

                StatusTrack.Text = My.Resources.Label_Track & " " & TrackInfo.SourceTrack
                StatusSide.Text = My.Resources.Label_Side & " " & TrackInfo.SourceSide
            End If
        End Sub

        Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
            If Process.IsRunning Then
                Process.Cancel()
                Exit Sub
            End If

            Dim Drive As String = ComboImageDrives.SelectedValue
            Dim FileExt As String
            If _DiskImage.Disk.Image.IsBitstreamImage Then
                FileExt = ".hfe"
            Else
                FileExt = ".ima"
            End If

            Dim TempPath = InitTempImagePath()
            Dim FileName = "New Image" & FileExt

            If TempPath = "" Then
                MsgBox(My.Resources.Dialog_TempPathError, MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            Dim FilePath = GenerateUniqueFileName(TempPath, FileName)

            Dim Response = ImageIO.SaveDiskImageToFile(_DiskImage.Disk, FilePath, False)
            If Response = SaveImageResponse.Success Then
                Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.write) With {
                    .InFile = FilePath,
                    .Drive = Drive,
                    .Retries = NumericRetries.Value,
                    .PreErase = CheckBoxPreErase.Checked
                }
                If FileExt = ".ima" Then
                    Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(_DiskImage.Disk.DiskFormat)
                    Builder.Format = GreaseweazleImageFormatCommandLine(ImageFormat)
                End If
                TextBoxConsole.Clear()
                Dim Arguments = Builder.Arguments
                TextBoxConsole.AppendText(Arguments)
                ResetStatusBar()
                _TrackStatus.Clear()
                TrackGridReset(_TrackCount, _SideCount)
                ToggleProcessRunning(True)
                Process.StartAsync(My.Settings.GW_Path, Arguments)
            End If
        End Sub

        Private Sub ResetStatusBar()
            StatusType.Text = ""
            StatusTrack.Text = ""
            StatusSide.Text = ""
        End Sub

        Private Sub ProcessOutputLine(line As String)
            If TextBoxConsole.Text.Length > 0 Then
                TextBoxConsole.AppendText(Environment.NewLine)
            End If
            TextBoxConsole.AppendText(line)

            Dim TrackInfo = Parser.ParseWriteTrackInfo(line)
            If TrackInfo IsNot Nothing Then
                TrackInfo = GetTrackInfo(TrackInfo.SourceTrack, TrackInfo.SourceSide, TrackInfo.Retry, TrackInfo.Failed)
                ProcessTrackStatus(TrackInfo)
                Return
            End If
        End Sub

        Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
            If Process.IsRunning Then
                Try
                    Process.Cancel()
                Catch ex As Exception
                End Try
            Else
                '
            End If
        End Sub

        Private Sub WriteDiskForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
            If Process.IsRunning Then
                Try
                    Process.Cancel()
                Catch ex As Exception
                End Try
            ElseIf e.CloseReason = CloseReason.UserClosing Then
                ' 
            End If
        End Sub

        Private Sub Process_ErrorLineReceived(line As String) Handles Process.ErrorLineReceived
            ProcessOutputLine(line)
        End Sub

        Private Sub Process_ProcessExited(exitCode As Integer) Handles Process.ProcessExited
            If exitCode = -1 Then
                StatusType.Text = My.Resources.Label_Aborted
            Else
                StatusType.Text = My.Resources.Label_Complete
            End If
            ToggleProcessRunning(False)
        End Sub

        Private Sub Process_ProcessFailed(message As String, ex As Exception) Handles Process.ProcessFailed
            StatusType.Text = My.Resources.Label_Failed
            ToggleProcessRunning(False)
        End Sub

        Private Sub Process_ProcessStarted(exePath As String, arguments As String) Handles Process.ProcessStarted
            StatusType.Text = My.Resources.Label_Writing
        End Sub
    End Class
End Namespace
