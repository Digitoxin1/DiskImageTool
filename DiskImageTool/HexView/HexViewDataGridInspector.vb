Imports DiskImageTool.DiskImage

Namespace HexView
    Public Enum DataRowEnum
        File
        Description
        CRC16
        CRC32
        Binary
        Int8
        UInt8
        Int16
        UInt16
        Int24
        UInt24
        Int32
        UInt32
        Int64
        UInt64
        DOSDate
        DOSTime
        DOSTimeDate
    End Enum

    Class HexViewDataGridInspector
        Private WithEvents DataGridView As DataGridView
        Public Sub New(DataGridView As DataGridView)
            Me.DataGridView = DataGridView

            Me.DataGridView.DoubleBuffer()

            Initialize()
        End Sub

        Public Sub CopyValueToClipboard()
            If DataGridView.SelectedRows.Count > 0 Then
                Dim Row = DataGridView.SelectedRows.Item(0)
                Dim Value = Row.Cells.Item("DataGridValue").Value
                Dim Invalid = Row.Cells.Item("DataGridInvalid").Value
                If Not Invalid Then
                    Clipboard.SetText(Value)
                End If
            End If
        End Sub

        Public Sub Initialize()
            DataGridView.Rows.Clear()

            AddRow("File", 0, DataRowEnum.File, False)
            AddRow("Region", 0, DataRowEnum.Description, False)
            AddRow("CRC16", 0, DataRowEnum.CRC16, False)
            AddRow("CRC32", 0, DataRowEnum.CRC32, False)
            AddRow("Binary (8 bit)", 1, DataRowEnum.Binary,, 8)
            AddRow("Int8", 1, DataRowEnum.Int8,, 4)
            AddRow("UInt8", 1, DataRowEnum.UInt8,, 3)
            AddRow("Int16", 2, DataRowEnum.Int16,, 6)
            AddRow("UInt16", 2, DataRowEnum.UInt16,, 5)
            AddRow("Int24", 3, DataRowEnum.Int24,, 9)
            AddRow("UInt24", 3, DataRowEnum.UInt24,, 8)
            AddRow("Int32", 4, DataRowEnum.Int32,, 11)
            AddRow("UInt32", 4, DataRowEnum.UInt32,, 10)
            AddRow("Int64", 8, DataRowEnum.Int64,, 21)
            AddRow("UInt64", 8, DataRowEnum.UInt64,, 20)
            AddRow("DOS Date", 2, DataRowEnum.DOSDate,, 10)
            AddRow("DOS Time", 2, DataRowEnum.DOSTime,, 8)
            AddRow("DOS Time & Date", 4, DataRowEnum.DOSTimeDate,, 19)
        End Sub

        Public Sub Refresh(Bytes() As Byte, SelectedBytes() As Byte, [ReadOnly] As Boolean, BigEndien As Boolean, ShowCRC16 As Boolean)
            Dim CRC16Checksum As Object = Nothing
            Dim CRC32Checksum As Object = Nothing
            Dim Binary As Object = Nothing
            Dim Int8 As Object = Nothing
            Dim UInt8 As Object = Nothing
            Dim Int16 As Object = Nothing
            Dim UInt16 As Object = Nothing
            Dim Int24 As Object = Nothing
            Dim UInt24 As Object = Nothing
            Dim Int32 As Object = Nothing
            Dim UInt32 As Object = Nothing
            Dim Int64 As Object = Nothing
            Dim UInt64 As Object = Nothing
            Dim DOSDate As Object = Nothing
            Dim DosTime As Object = Nothing
            Dim DosTimeDate As Object = Nothing

            If Bytes IsNot Nothing Then
                Dim DT As ExpandedDate

                If ShowCRC16 Then
                    CRC16Checksum = Bitstream.IBM_MFM.MFMCRC16(SelectedBytes).ToString("X4")
                End If
                CRC32Checksum = CRC32.ComputeChecksum(SelectedBytes).ToString("X8")
                Binary = Convert.ToString(Bytes(0), 2).PadLeft(8, "0")
                Int8 = MyBitConverter.ToSByte(Bytes(0))
                UInt8 = Bytes(0)
                If Bytes.Length >= 2 Then
                    Int16 = MyBitConverter.ToInt16(Bytes, BigEndien)
                    UInt16 = MyBitConverter.ToUInt16(Bytes, BigEndien)
                    DT = ExpandDate(CType(UInt16, UInt16))
                    If DT.IsValidDate Then
                        DOSDate = DT.DateObject.ToString("yyyy-MM-dd")
                    End If
                    DT = ExpandTime(CType(UInt16, UInt16))
                    If DT.IsValidDate Then
                        DosTime = DT.DateObject.ToString("HH:mm:ss")
                    End If
                End If
                If Bytes.Length >= 3 Then
                    Int24 = MyBitConverter.ToInt24(Bytes, BigEndien)
                    UInt24 = MyBitConverter.ToUInt24(Bytes, BigEndien)
                End If
                If Bytes.Length >= 4 Then
                    Int32 = MyBitConverter.ToInt32(Bytes, BigEndien)
                    UInt32 = MyBitConverter.ToUInt32(Bytes, BigEndien)
                    DT = ExpandTimeDate(CType(UInt32, UInt32))
                    If DT.IsValidDate Then
                        DosTimeDate = DT.DateObject.ToString("yyyy-MM-dd HH:mm:ss")
                    End If
                End If
                If Bytes.Length >= 8 Then
                    Int64 = MyBitConverter.ToInt64(Bytes, BigEndien)
                    UInt64 = MyBitConverter.ToUInt64(Bytes, BigEndien)
                End If
            End If

            SetDataRow(DataRowEnum.CRC16, CRC16Checksum, True, Not ShowCRC16)
            SetDataRow(DataRowEnum.CRC32, CRC32Checksum, True)
            SetDataRow(DataRowEnum.Binary, Binary, [ReadOnly])
            SetDataRow(DataRowEnum.Int8, Int8, [ReadOnly])
            SetDataRow(DataRowEnum.UInt8, UInt8, [ReadOnly])
            SetDataRow(DataRowEnum.Int16, Int16, [ReadOnly])
            SetDataRow(DataRowEnum.UInt16, UInt16, [ReadOnly])
            SetDataRow(DataRowEnum.Int24, Int24, [ReadOnly])
            SetDataRow(DataRowEnum.UInt24, UInt24, [ReadOnly])
            SetDataRow(DataRowEnum.Int32, Int32, [ReadOnly])
            SetDataRow(DataRowEnum.UInt32, UInt32, [ReadOnly])
            SetDataRow(DataRowEnum.Int64, Int64, [ReadOnly])
            SetDataRow(DataRowEnum.UInt64, UInt64, [ReadOnly])
            SetDataRow(DataRowEnum.DOSDate, DOSDate, [ReadOnly])
            SetDataRow(DataRowEnum.DOSTime, DosTime, [ReadOnly])
            SetDataRow(DataRowEnum.DOSTimeDate, DosTimeDate, [ReadOnly])
        End Sub

        Public Sub SetDataRow(Index As DataRowEnum, Value As Object, [ReadOnly] As Boolean, Optional HideIfEmpty As Boolean = False)
            Dim RowValue As Object
            Dim ForeColor As Color
            Dim Visible As Boolean
            Dim Invalid As Boolean

            If Value Is Nothing Then
                RowValue = "Invalid"
                ForeColor = SystemColors.GrayText
                Visible = Not HideIfEmpty
                Invalid = True
            Else
                RowValue = Value
                ForeColor = SystemColors.ControlText
                Visible = True
                Invalid = False
            End If

            With DataGridView.Rows.Item(Index)
                .Visible = Visible
                With .Cells.Item(1)
                    .Value = RowValue
                    .Style.ForeColor = ForeColor
                End With
                With .Cells.Item(3)
                    .Value = Invalid
                End With
                .ReadOnly = [ReadOnly]
            End With
        End Sub

        Public Sub Validate(e As DataGridViewCellValidatingEventArgs)
            Dim Row = DataGridView.Rows(e.RowIndex)
            Dim CellValue As String = Row.Cells.Item("DataGridValue").Value
            Dim DataType As DataRowEnum = Row.Cells.Item("DataGridType").Value
            Dim Invalid As Boolean = Row.Cells.Item("DataGridInvalid").Value
            Dim Result As Boolean
            Dim ErrorMsg As String = ""

            If e.FormattedValue = "Invalid" And Invalid Then
                Exit Sub
            End If

            Select Case DataType
                Case DataRowEnum.Binary
                    ErrorMsg = "Please enter a valid 8-bit binary value"
                    Dim cValue As Byte
                    Try
                        cValue = Convert.ToByte(e.FormattedValue, 2)
                        Result = True
                    Catch
                        Result = False
                    End Try
                Case DataRowEnum.Int8
                    ErrorMsg = "Please enter a valid integer between -128 and 127"
                    Dim cValue As SByte
                    Result = SByte.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.UInt8
                    ErrorMsg = "Please enter a valid integer between 0 and 255"
                    Dim cValue As Byte
                    Result = Byte.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.Int16
                    ErrorMsg = "Please enter a valid integer between -32,768 and 32,767"
                    Dim cValue As Int16
                    Result = Int16.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.UInt16
                    ErrorMsg = "Please enter a valid integer between 0 and 65,535"
                    Dim cValue As UInt16
                    Result = UInt16.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.Int32
                    ErrorMsg = "Please enter a valid integer between -2,147,483,648 and 2,147,483,647"
                    Dim cValue As Int32
                    Result = Int32.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.Int24
                    ErrorMsg = "Please enter a valid integer between -8,388,608 and 8,388,607"
                    Dim cValue As Int32
                    Result = Int32.TryParse(e.FormattedValue, cValue)
                    If Result Then
                        Result = cValue >= -8388608 And cValue <= 8388607
                    End If
                Case DataRowEnum.UInt24
                    ErrorMsg = "Please enter a valid integer between 0 and 16,777,215"
                    Dim cValue As UInt32
                    Result = UInt32.TryParse(e.FormattedValue, cValue)
                    If Result Then
                        Result = cValue >= 0 And cValue <= 16777215
                    End If
                Case DataRowEnum.UInt32
                    ErrorMsg = "Please enter a valid integer between 0 and 4,294,967,295"
                    Dim cValue As UInt32
                    Result = UInt32.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.Int64
                    ErrorMsg = "Please enter a valid integer between -9,223,372,036,854,775,808 and 9,223,372,036,854,775,807"
                    Dim cValue As Int64
                    Result = Int64.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.UInt64
                    ErrorMsg = "Please enter a valid integer between 0 and 18,446,744,073,709,551,615"
                    Dim cValue As UInt64
                    Result = UInt64.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.DOSDate
                    ErrorMsg = "Please enter a valid date between 1980-01-01 and 2107-12-31"
                    Dim cValue As Date
                    Result = Date.TryParse(e.FormattedValue, cValue)
                    If Result Then
                        If cValue.Year < 1980 Or cValue.Year > 2107 Then
                            Result = False
                        End If
                    End If
                Case DataRowEnum.DOSTime
                    ErrorMsg = "Please enter a valid time"
                    Dim cValue As Date
                    Result = Date.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.DOSTimeDate
                    ErrorMsg = "Please enter a valid date and time between 1980-01-01 and 2107-12-31"
                    Dim cValue As Date
                    Result = Date.TryParse(e.FormattedValue, cValue)
                    If Result Then
                        If cValue.Year < 1980 Or cValue.Year > 2107 Then
                            Result = False
                        End If
                    End If
                Case Else
                    Result = True
            End Select

            If Not Result Then
                MsgBox(ErrorMsg, MsgBoxStyle.Exclamation)

                e.Cancel = True
                DataGridView.CancelEdit()
            End If
        End Sub

        Private Sub AddRow(Name As String, Length As Integer, Type As DataRowEnum, Optional Editable As Boolean = True, Optional MaxInputLength As Integer = 32767)
            Dim Row = DataGridView.Rows.Add(Name, "", Length, True, Editable, Type)
            Dim Cell As DataGridViewTextBoxCell = DataGridView.Rows.Item(Row).Cells.Item("DataGridValue")
            Cell.MaxInputLength = MaxInputLength
        End Sub

        Private Sub DataGridView_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles DataGridView.CellValidating
            If Not DataGridView.IsCurrentCellInEditMode Then
                Exit Sub
            End If

            Validate(e)
        End Sub

        Private Sub DataGridView_LostFocus(sender As Object, e As EventArgs) Handles DataGridView.LostFocus
            DataGridView.ClearSelection()
        End Sub
    End Class
End Namespace
