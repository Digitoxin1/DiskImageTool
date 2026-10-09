Namespace ImageForm
    Friend Module ImageFormShared
        Public Const InterfaceModeDisabled As Byte = &HFE

        Public ReadOnly InterfaceModes() As Byte = {
            &H0, &H1, &H8, &H2, &H3, &H4, &H5, &H6, &H7, &H9, &HA, &HB, &HC, &HD, &HE, &HF, &H10, InterfaceModeDisabled
        }

        Public Sub EnableDoubleBuffer(Grid As DataGridView)
            GetType(DataGridView).InvokeMember(
                "DoubleBuffered",
                Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.SetProperty,
                Nothing,
                Grid,
                New Object() {True})
        End Sub

        Public Sub AttachNumericTextBox(Box As TextBox)
            AddHandler Box.KeyPress, AddressOf NumericTextBox_KeyPress
            AddHandler Box.TextChanged, AddressOf NumericTextBox_TextChanged
            AddHandler Box.LostFocus, AddressOf NumericTextBox_LostFocus
        End Sub

        Public Sub AddCheckColumn(Grid As DataGridView, Name As String, HeaderText As String, Optional Width As Integer? = Nothing, Optional Editable As Boolean = False)
            Dim Column As New DataGridViewCheckBoxColumn With {
                .Name = Name,
                .HeaderText = HeaderText,
                .ReadOnly = Not Editable,
                .DataPropertyName = Name,
                .SortMode = DataGridViewColumnSortMode.NotSortable,
                .MinimumWidth = 60
            }

            If Width.HasValue Then
                Column.Width = Width
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            Else
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            End If

            Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Grid.Columns.Add(Column)
        End Sub

        Public Sub AddTextColumn(Grid As DataGridView, Name As String, HeaderText As String, Width As Integer, Alignment As DataGridViewContentAlignment, Optional Format As String = "", Optional Padding As Integer = 0)
            Dim Column As New DataGridViewTextBoxColumn With {
                .Name = Name,
                .HeaderText = HeaderText,
                .ReadOnly = True,
                .DataPropertyName = Name,
                .Width = Width,
                .SortMode = DataGridViewColumnSortMode.NotSortable
            }
            Column.DefaultCellStyle.Alignment = Alignment

            If Format <> "" Then
                Column.DefaultCellStyle.Format = Format
            End If

            If Padding <> 0 Then
                Column.DefaultCellStyle.Padding = New Padding(0, 0, Padding, 0)
            End If

            Grid.Columns.Add(Column)
        End Sub

        Public Sub AddDataColumn(Table As DataTable, Name As String, DataType As Type)
            Table.Columns.Add(New DataColumn(Name, DataType))
        End Sub

        Public Sub PopulateByteCombo(Combo As ComboBox, Values As IEnumerable(Of Byte), ValueToSelect As Byte, Caption As Func(Of Byte, String))
            Combo.BeginUpdate()
            Combo.Items.Clear()

            Dim Selected As ByteListItem = Nothing
            For Each ItemValue In Values
                Dim Item As New ByteListItem(ItemValue, Caption(ItemValue))
                Combo.Items.Add(Item)
                If ItemValue = ValueToSelect Then
                    Selected = Item
                End If
            Next

            If Selected Is Nothing Then
                Selected = New ByteListItem(ValueToSelect, ValueToSelect.ToString("X2"))
                Combo.Items.Insert(0, Selected)
            End If

            Combo.SelectedItem = Selected
            Combo.EndUpdate()
        End Sub

        Public Sub ResizeGridWidth(Grid As DataGridView)
            Dim NewWidth As Integer = Grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible)

            If Grid.RowHeadersVisible Then
                NewWidth += Grid.RowHeadersWidth
            End If

            If Grid.Controls.OfType(Of VScrollBar)().Any(Function(s) s.Visible) Then
                NewWidth += SystemInformation.VerticalScrollBarWidth
            End If

            NewWidth += Grid.Width - Grid.ClientSize.Width

            Grid.Width = NewWidth + 4
        End Sub

        Public Function InterfaceModeCaption(Mode As Byte) As String
            Select Case Mode
                Case &H0
                    Return My.Resources.FloppyInterface_IbmPcDd
                Case &H1
                    Return My.Resources.FloppyInterface_IbmPcHd
                Case &H2
                    Return My.Resources.FloppyInterface_AtariStDd
                Case &H3
                    Return My.Resources.FloppyInterface_AtariStHd
                Case &H4
                    Return My.Resources.FloppyInterface_AmigaDd
                Case &H5
                    Return My.Resources.FloppyInterface_AmigaHd
                Case &H6
                    Return My.Resources.FloppyInterface_CpcDd
                Case &H7
                    Return My.Resources.FloppyInterface_ShugartDd
                Case &H8
                    Return My.Resources.FloppyInterface_IbmPcEd
                Case &H9
                    Return My.Resources.FloppyInterface_Msx2Dd
                Case &HA
                    Return My.Resources.FloppyInterface_C64Dd
                Case &HB
                    Return My.Resources.FloppyInterface_EmuShugart
                Case &HC
                    Return My.Resources.FloppyInterface_S950Dd
                Case &HD
                    Return My.Resources.FloppyInterface_S950Hd
                Case &HE
                    Return My.Resources.FloppyInterface_S950Auto
                Case &HF
                    Return My.Resources.FloppyInterface_IbmPcAuto
                Case &H10
                    Return My.Resources.FloppyInterface_QuickDisk
                Case InterfaceModeDisabled
                    Return My.Resources.FloppyInterface_Disabled
                Case Else
                    Return Mode.ToString("X2")
            End Select
        End Function

        Private Sub NumericTextBox_KeyPress(sender As Object, e As KeyPressEventArgs)
            If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
                e.Handled = True
            End If
        End Sub

        Private Sub NumericTextBox_TextChanged(sender As Object, e As EventArgs)
            Dim Box = DirectCast(sender, TextBox)
            Dim Digits = New String(Box.Text.Where(Function(Character) Char.IsDigit(Character)).ToArray())
            If Digits = Box.Text Then
                Exit Sub
            End If

            Dim SelectionStart = Math.Min(Box.SelectionStart, Digits.Length)
            Box.Text = Digits
            Box.SelectionStart = SelectionStart
        End Sub

        Private Sub NumericTextBox_LostFocus(sender As Object, e As EventArgs)
            Dim Box = DirectCast(sender, TextBox)
            Dim Parsed As ULong
            If Not ULong.TryParse(Box.Text, Parsed) Then
                Exit Sub
            End If

            If Parsed > UShort.MaxValue Then
                Box.Text = UShort.MaxValue.ToString()
            End If
        End Sub

        Public Class ByteListItem
            Public Sub New(Value As Byte, Caption As String)
                Me.Value = Value
                _Caption = Caption
            End Sub

            Public ReadOnly Property Value As Byte

            Public Overrides Function ToString() As String
                Return _Caption
            End Function

            Private ReadOnly _Caption As String
        End Class
    End Module
End Namespace
