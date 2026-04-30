Imports System.ComponentModel
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Public Class SettingsPanel
        Private ReadOnly ToolTip1 As New ToolTip()
        Private _Initialized As Boolean = False

        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            LocalizeForm()
        End Sub

        Private Sub LocalizeForm()
            LabelTracks0.Text = My.Resources.Label_Tracks
            LabelTracks1.Text = My.Resources.Label_Tracks
            LabelTracks2.Text = My.Resources.Label_Tracks
            LabelLogFile.Text = My.Resources.Label_LogFilename
            ButtonInfo.Text = My.Resources.Label_Info
            LabelPort.Text = My.Resources.Label_Port
            LabelDriveInterface.Text = My.Resources.Label_DriveInterface
            LabelDefaultRevs.Text = My.Resources.Label_DefaultRevs
        End Sub

        Public ReadOnly Property Initialized As Boolean
            Get
                Return _Initialized
            End Get
        End Property

        Public Sub Initialize()
            InitializeFields()
            _Initialized = True
        End Sub

        Public Sub UpdateSettings()
            If Not _Initialized Then
                Exit Sub
            End If

            Settings.Interface = ComboInterface.SelectedValue
            Settings.ComPort = ComboPorts.SelectedValue
            Settings.Drives(0).SetDrive(ComboDriveType0.SelectedValue, NumericTracks0.Value)
            Settings.Drives(1).SetDrive(ComboDriveType1.SelectedValue, NumericTracks1.Value)

            If DirectCast(ComboInterface.SelectedValue, GreaseweazleSettings.GreaseweazleInterface) = GreaseweazleSettings.GreaseweazleInterface.Shugart Then
                Settings.Drives(2).SetDrive(ComboDriveType2.SelectedValue, NumericTracks2.Value)
            Else
                Settings.Drives(2).SetDrive(FloppyDriveType.DriveUnknown, 0)
            End If
            Settings.DefaultRevs = NumericDefaultRevs.Value
            Settings.LogFileName = TextBoxLogFile.Text.Trim
        End Sub

        Private Function GetComboDriveType(index As Byte) As ComboBox
            Select Case index
                Case 0
                    Return ComboDriveType0
                Case 1
                    Return ComboDriveType1
                Case Else
                    Return ComboDriveType2
            End Select
        End Function

        Private Function GetNumericTracks(index As Byte) As NumericUpDown
            Select Case index
                Case 0
                    Return NumericTracks0
                Case 1
                    Return NumericTracks1
                Case Else
                    Return NumericTracks2
            End Select
        End Function

        Private Sub InitializeDriveType(index As Byte)
            Dim Combo = GetComboDriveType(index)
            Dim CurrentValue = Settings.Drives(index).Type

            Dim DriveList As New List(Of KeyValuePair(Of String, FloppyDriveType))

            For Each FloppyType As FloppyDriveType In [Enum].GetValues(GetType(FloppyDriveType))
                DriveList.Add(New KeyValuePair(Of String, FloppyDriveType)(
                    GreaseweazleFloppyTypeDescription(FloppyType), FloppyType)
                )
            Next

            InitializeCombo(Combo, DriveList, CurrentValue)
        End Sub

        Private Sub InitializeFields()
            InitializeInterfaceName(ComboInterface, Settings.Interface)
            InitializePorts(Settings.ComPort)
            For i = 0 To 2
                InitializeDriveType(i)
                InitializeTracks(i)
            Next

            NumericDefaultRevs.Minimum = MIN_REVS
            NumericDefaultRevs.Maximum = MAX_REVS
            NumericDefaultRevs.Value = Settings.DefaultRevs

            TextBoxLogFile.Text = Settings.LogFileName
        End Sub

        Private Sub InitializeInput(index As Byte, Type As FloppyDriveType, Tracks As Byte)
            Dim Input = GetNumericTracks(index)

            Dim MinMax = DriveSettings.GetMinMax(Type)

            Input.Minimum = MinMax.Min
            Input.Maximum = MinMax.Max
            If Tracks = 0 Then
                Input.Value = MinMax.Max
            Else
                Input.Value = Math.Min(Math.Max(Tracks, MinMax.Min), MinMax.Max)
            End If
        End Sub

        Private Sub InitializeInterfaceName(Combo As ComboBox, CurrentValue As GreaseweazleSettings.GreaseweazleInterface)
            Dim DriveList As New List(Of KeyValuePair(Of String, GreaseweazleSettings.GreaseweazleInterface))

            For Each InterfaceType As GreaseweazleSettings.GreaseweazleInterface In [Enum].GetValues(GetType(GreaseweazleSettings.GreaseweazleInterface))
                DriveList.Add(New KeyValuePair(Of String, GreaseweazleSettings.GreaseweazleInterface)(
                    GreaseweazleInterfaceName(InterfaceType), InterfaceType)
                )
            Next

            InitializeCombo(Combo, DriveList, CurrentValue)

            RefreshDriveTypes(CurrentValue)
        End Sub

        Private Sub InitializePorts(CurrentValue As String)
            Dim PortList As New List(Of KeyValuePair(Of String, String)) From {
                New KeyValuePair(Of String, String)(
                   "Auto", "")
            }

            Dim ports = IO.Ports.SerialPort.GetPortNames()

            If CurrentValue <> "" Then
                If Not ports.Contains(CurrentValue, StringComparer.OrdinalIgnoreCase) Then
                    ports = ports.Append(CurrentValue).ToArray()
                End If
            End If

            Dim SortedPorts = ports.OrderBy(Function(name)
                                                Dim num As Integer = Integer.Parse(name.Substring(3))
                                                Return num
                                            End Function)

            For Each p In SortedPorts
                PortList.Add(New KeyValuePair(Of String, String)(
                   p, p)
               )
            Next

            InitializeCombo(ComboPorts, PortList, CurrentValue)
        End Sub

        Private Sub InitializeTracks(index As Byte)
            Dim Type = Settings.Drives(index).Type
            Dim Tracks = Settings.Drives(index).Tracks

            InitializeInput(index, Type, Tracks)
        End Sub

        Private Sub RefreshDriveTypes(DriveInterface As GreaseweazleSettings.GreaseweazleInterface)
            If DriveInterface = GreaseweazleSettings.GreaseweazleInterface.Shugart Then
                LabelDriveType0.Text = String.Format(My.Resources.Label_DriveType, "0")
                LabelDriveType1.Text = String.Format(My.Resources.Label_DriveType, "1")
                LabelDriveType2.Text = String.Format(My.Resources.Label_DriveType, "2")

                LabelDriveType2.Visible = True
                ComboDriveType2.Visible = True

                LabelTracks2.Visible = True
                NumericTracks2.Visible = True
            Else
                LabelDriveType0.Text = String.Format(My.Resources.Label_DriveType, "A")
                LabelDriveType1.Text = String.Format(My.Resources.Label_DriveType, "B")

                LabelDriveType2.Visible = False
                ComboDriveType2.Visible = False

                LabelTracks2.Visible = False
                NumericTracks2.Visible = False
            End If
        End Sub

        Private Sub RefreshInput(index As Byte)
            Dim CurrentType As FloppyDriveType = GetComboDriveType(index).SelectedValue

            Dim Tracks As Byte = 0

            If CurrentType = Settings.Drives(index).Type Then
                Tracks = Settings.Drives(index).Tracks
            End If

            InitializeInput(index, CurrentType, Tracks)

            GetNumericTracks(index).Enabled = (CurrentType <> FloppyDriveType.DriveUnknown)
        End Sub
#Region "Events"
        Private Sub ButtonInfo_Click(sender As Object, e As EventArgs) Handles ButtonInfo.Click
            Me.Cursor = Cursors.WaitCursor

            Application.DoEvents()

            Dim Content = GetInfo()

            Me.Cursor = Cursors.Default

            TextBoxInfo.Text = Content
        End Sub

        Private Sub ComboDriveType0_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboDriveType0.SelectedIndexChanged
            RefreshInput(0)
        End Sub

        Private Sub ComboDriveType1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboDriveType1.SelectedIndexChanged
            RefreshInput(1)
        End Sub

        Private Sub ComboDriveType2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboDriveType2.SelectedIndexChanged
            RefreshInput(2)
        End Sub

        Private Sub ComboInterface_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboInterface.SelectedIndexChanged
            RefreshDriveTypes(DirectCast(ComboInterface.SelectedValue, GreaseweazleSettings.GreaseweazleInterface))
        End Sub

        Private Sub TextBoxLogFile_Validating(sender As Object, e As CancelEventArgs) Handles TextBoxLogFile.Validating
            Dim name = TextBoxLogFile.Text.Trim()

            If Not IsValidFileName(name) Then
                e.Cancel = True   ' Prevent leaving the field
                MessageBox.Show(
                    My.Resources.Dialog_InvalidFilename,
                    My.Resources.Label_InvalidFilename,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )
                ' Optionally select all text so user can fix it immediately:
                TextBoxLogFile.SelectAll()
            End If
        End Sub
#End Region
    End Class
End Namespace
