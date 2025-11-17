Imports System.Text.RegularExpressions
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux
    Namespace Greaseweazle
        Module GreaseweazleLib
            Public GreaseweazleSettings As New Settings

            Public Function GetTrackHeads(StartHead As Integer, Optional EndHead As Integer = -1) As CommandLineBuilder.TrackHeads
                If EndHead = -1 Then
                    EndHead = StartHead
                End If

                If StartHead = 0 And EndHead = 0 Then
                    Return CommandLineBuilder.TrackHeads.head0
                ElseIf StartHead = 1 And EndHead = 1 Then
                    Return CommandLineBuilder.TrackHeads.head1
                Else
                    Return CommandLineBuilder.TrackHeads.both
                End If
            End Function

            Public Sub BandwidthDisplay(ParentForm As Form)
                If Not GreaseweazleSettings.IsPathValid Then
                    DisplayInvalidApplicationPathMsg()
                    Exit Sub
                End If

                ParentForm.Cursor = Cursors.WaitCursor
                Application.DoEvents()

                Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.bandwidth) With {
                .Device = GreaseweazleSettings.COMPort
            }

                Dim Arguments = Builder.Arguments

                Dim Content As String = ""
                Try
                    Dim Result = ConsoleProcessRunner.RunProcess(GreaseweazleSettings.AppPath, Arguments)
                    Content = Result.CombinedOutput
                Finally
                    ParentForm.Cursor = Cursors.Default
                End Try

                Dim frmTextView = New TextViewForm("Greaseweazle - " & My.Resources.Label_Bandwidth, Content, False, True, "GreaseweazleBandwidth.txt")
                frmTextView.ShowDialog(ParentForm)
            End Sub

            Public Function BuildRanges(values As HashSet(Of UShort)) As List(Of (StartTrack As UShort, EndTrack As UShort))
                Dim result As New List(Of (UShort, UShort))()

                If values Is Nothing OrElse values.Count = 0 Then
                    Return result
                End If

                ' Sort the values first
                Dim sorted = values.OrderBy(Function(v) v).ToList()

                Dim rangeStart As UShort = sorted(0)
                Dim rangeEnd As UShort = sorted(0)

                For i As Integer = 1 To sorted.Count - 1
                    Dim v = sorted(i)

                    If v = rangeEnd + 1US Then
                        ' extend range
                        rangeEnd = v
                    Else
                        ' push previous range
                        result.Add((rangeStart, rangeEnd))
                        ' start new range
                        rangeStart = v
                        rangeEnd = v
                    End If
                Next

                ' add final range
                result.Add((rangeStart, rangeEnd))

                Return result
            End Function

            Public Sub CleanDisk(ParentForm As Form)
                Dim Form As New CleanDiskForm()
                Form.ShowDialog(ParentForm)
            End Sub

            Public Function ConvertFirstTrack(FilePath As String) As (Result As Boolean, FileName As String)
                Dim TempPath = InitTempImagePath()

                If TempPath = "" Then
                    Return (False, "")
                End If

                Dim FileName = GenerateUniqueFileName(TempPath, "temp.ima")

                Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.convert) With {
                .InFile = FilePath,
                .OutFile = FileName,
                .Format = "ibm.scan",
                .Heads = CommandLineBuilder.TrackHeads.head0
            }
                Builder.AddCylinder(0)

                ConsoleProcessRunner.RunProcess(GreaseweazleSettings.AppPath, Builder.Arguments, captureOutput:=False, captureError:=False)

                Return (IO.File.Exists(FileName), FileName)
            End Function

            Public Sub DisplayInvalidApplicationPathMsg()
                MessageBox.Show(My.Resources.Dialog_InvalidApplicationPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Sub

            Public Sub EraseDisk(ParentForm As Form)
                Dim Form As New EraseDiskForm()
                Form.ShowDialog(ParentForm)
            End Sub

            Public Function GetFirstRawFile(FilePath As String) As String
                Dim RawFileName As String = ""

                For Each file In IO.Directory.EnumerateFiles(FilePath, "*.raw", IO.SearchOption.TopDirectoryOnly)
                    Dim name = IO.Path.GetFileName(file)
                    Dim PrefixMatch = Regex.Match(name, REGEX_RAW_FILE, RegexOptions.IgnoreCase)
                    If PrefixMatch.Success Then
                        RawFileName = file
                        Exit For
                    End If
                Next

                Return RawFileName
            End Function

            Public Function ImportFluxImage(FilePath As String, ParentForm As Form) As (Result As Boolean, OutputFile As String, NewFileName As String)
                Dim FileExt = IO.Path.GetExtension(FilePath).ToLower
                Dim TrackCount As Integer = 0
                Dim SideCount As Integer = 0

                If FileExt = ".raw" Then
                    Dim Response = GetTrackCountRaw(FilePath)
                    If Not Response.Result Then
                        MsgBox(My.Resources.Dialog_InvalidKryofluxFile, MsgBoxStyle.Exclamation)
                        Return (False, "", "")
                    Else
                        TrackCount = Response.Tracks
                        SideCount = Response.Sides
                    End If
                ElseIf FileExt = ".scp" Then
                    Dim Response = GetTrackCountSCP(FilePath)
                    If Not Response.Result Then
                        MsgBox(My.Resources.Dialog_InvalidSCPFile, MsgBoxStyle.Exclamation)
                    Else
                        TrackCount = Response.Tracks
                        SideCount = Response.Sides
                    End If
                Else
                    MsgBox(My.Resources.Dialog_InvalidFileType, MsgBoxStyle.Exclamation)
                    Return (False, "", "")
                End If

                If SideCount > 2 Then
                    SideCount = 2
                End If

                If TrackCount > 42 And TrackCount < 80 Then
                    TrackCount = 80
                End If

                Dim Form As New ImageImportForm(FilePath, TrackCount, SideCount)
                If Form.ShowDialog(ParentForm) = DialogResult.OK Then
                    If Not String.IsNullOrEmpty(Form.OutputFilePath) Then
                        Return (True, Form.OutputFilePath, Form.GetNewFileName)
                    End If
                End If

                Return (False, "", "")
            End Function

            Public Sub InfoDisplay(ParentForm As Form)
                If Not GreaseweazleSettings.IsPathValid Then
                    DisplayInvalidApplicationPathMsg()
                    Exit Sub
                End If

                ParentForm.Cursor = Cursors.WaitCursor
                Application.DoEvents()

                Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.info) With {
                .Device = GreaseweazleSettings.COMPort
            }
                Dim Arguments = Builder.Arguments

                Dim Content As String = ""
                Try
                    Dim Result = ConsoleProcessRunner.RunProcess(GreaseweazleSettings.AppPath, Arguments)
                    Content = Result.CombinedOutput
                Finally
                    ParentForm.Cursor = Cursors.Default
                End Try

                Dim frmTextView = New TextViewForm("Greaseweazle - " & My.Resources.Label_Info, Content, False, True, "GreaseweazleInfo.txt")
                frmTextView.ShowDialog(ParentForm)
            End Sub

            Public Sub InitializeCombo(Combo As ComboBox, DataSource As Object, CurrentValue As Object)
                Combo.DisplayMember = "Key"
                Combo.ValueMember = "Value"
                Combo.DataSource = DataSource
                Combo.DropDownStyle = ComboBoxStyle.DropDownList

                If CurrentValue IsNot Nothing Then
                    Combo.SelectedValue = CurrentValue
                End If
            End Sub

            Public Function OpenFluxImage(ParentForm As Form) As String
                Using Dialog As New OpenFileDialog With {
                .Title = "Open Flux Image",
                .Filter = "Flux dumps (*.raw;*.scp)|*.raw;*.scp|" &
                    "KryoFlux RAW (*.raw)|*.raw|" &
                    "SuperCard Pro (*.scp)|*.scp",
                .FilterIndex = 1,
                .CheckFileExists = True,
                .AddExtension = True,
                .Multiselect = False
            }

                    If Dialog.ShowDialog(ParentForm) = DialogResult.OK Then
                        Return Dialog.FileName
                    End If
                End Using

                Return Nothing
            End Function

            Public Sub PopulateDrives(Combo As ComboBox, Format As FloppyMediaType)
                Dim DriveList As New List(Of DriveOption)

                Dim SelectedOption As DriveOption = Nothing

                Dim AddItem As Action(Of String, String, Byte) =
                Sub(labelPrefix As String, id As String, index As Byte)
                    Dim t = GreaseweazleSettings.DriveType(index)
                    If t = FloppyMediaType.MediaUnknown Then
                        Exit Sub
                    End If

                    Dim opt = New DriveOption With {
                        .Id = id,
                        .Type = t,
                        .Tracks = GreaseweazleSettings.TrackCount(index),
                        .Label = $"{labelPrefix}:   {GreaseweazleFloppyTypeDescription(t)}"
                    }
                    DriveList.Add(opt)

                    If SelectedOption Is Nothing AndAlso t = Format Then
                        SelectedOption = opt
                    End If
                End Sub

                If GreaseweazleSettings.Interface = Settings.GreaseweazleInterface.Shugart Then
                    AddItem("DS0", "0", 0)
                    AddItem("DS1", "1", 1)
                    AddItem("DS2", "2", 2)
                Else
                    AddItem("A", "A", 0)
                    AddItem("B", "B", 1)
                End If

                Dim placeholder As DriveOption = Nothing
                If DriveList.Count <> 1 Then
                    placeholder = New DriveOption With {
                    .Id = "",
                    .Type = FloppyMediaType.MediaUnknown,
                    .Tracks = 0,
                    .Label = IIf(DriveList.Count = 0, My.Resources.Label_NoDrivesFound, My.Resources.Label_PleaseSelect)
                }
                    DriveList.Insert(0, placeholder)
                End If

                With Combo
                    .DropDownStyle = ComboBoxStyle.DropDownList
                    .DataSource = DriveList
                    .DisplayMember = NameOf(DriveOption.Label)
                    .ValueMember = ""
                    .SelectedItem = If(SelectedOption, placeholder)
                    If .SelectedIndex = -1 Then
                        .SelectedIndex = 0
                    End If
                End With
            End Sub

            Public Function ReadFirstTrack(DriveId As String) As (Result As Boolean, FileName As String, Output As String)
                Dim TempPath = InitTempImagePath()

                If TempPath = "" Then
                    Return (False, "", "")
                End If

                Dim FileName = GenerateUniqueFileName(TempPath, "temp.ima")

                Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.read) With {
                .Device = GreaseweazleSettings.COMPort,
                .Drive = DriveId,
                .File = FileName,
                .Format = "ibm.scan",
                .Heads = CommandLineBuilder.TrackHeads.head0
            }
                Builder.AddCylinder(0)

                Dim Result = ConsoleProcessRunner.RunProcess(GreaseweazleSettings.AppPath, Builder.Arguments, captureOutput:=True, captureError:=True)

                Return (IO.File.Exists(FileName), FileName, Result.CombinedOutput)
            End Function
            Public Function ReadFluxImage(ParentForm As Form) As (Result As Boolean, OutputFile As String, NewFileName As String)
                Dim Form As New ReadDiskForm()
                If Form.ShowDialog(ParentForm) = DialogResult.OK Then
                    If Not String.IsNullOrEmpty(Form.OutputFilePath) Then
                        Return (True, Form.OutputFilePath, Form.GetNewFileName)
                    End If
                End If

                Return (False, "", "")
            End Function
            Public Sub Reset(TextBox As TextBox)
                Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.reset) With {
                .Device = GreaseweazleSettings.COMPort
            }

                Dim Arguments = Builder.Arguments
                Dim Result = ConsoleProcessRunner.RunProcess(GreaseweazleSettings.AppPath, Arguments)
                TextBox.Text = Result.CombinedOutput

                MsgBox(My.Resources.Dialog_GreaseweazleReset, MsgBoxStyle.Information)
            End Sub

            Public Sub WriteImageToDisk(ParentForm As Form, Image As DiskImageContainer)
                Dim Form As New WriteDiskForm(Image.Disk, Image.ImageData.FileName)
                Form.ShowDialog(ParentForm)
            End Sub

        End Module
    End Namespace
End Namespace