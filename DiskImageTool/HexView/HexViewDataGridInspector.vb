Imports DiskImageTool.DiskImage

Namespace HexView
    Public Enum DataRowEnum
        Area
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
        Bitstream
        WeakBits
    End Enum

    Class HexViewDataGridInspector
        Public Const COLUMN_NAME As String = "DataGridName"
        Public Const COLUMN_VALUE As String = "DataGridValue"
        Public Const COLUMN_LENGTH As String = "DataGridLength"
        Public Const COLUMN_INVALID As String = "DataGridInvalid"
        Public Const COLUMN_EDITABLE As String = "DataGridEditable"
        Public Const COLUMN_TYPE As String = "DataGridType"

        Private WithEvents DataGridView As DataGridView
        Public Sub New(DataGridView As DataGridView)
            Me.DataGridView = DataGridView

            Me.DataGridView.DoubleBuffer()

            Initialize()
        End Sub

        Public Sub CopyValueToClipboard()
            If DataGridView.SelectedRows.Count > 0 Then
                Dim Row = DataGridView.SelectedRows.Item(0)
                Dim Value = Row.Cells.Item(COLUMN_VALUE).Value
                Dim Invalid = Row.Cells.Item(COLUMN_INVALID).Value
                If Not Invalid Then
                    Clipboard.SetText(Value)
                End If
            End If
        End Sub

        Public Sub Initialize()
            DataGridView.Rows.Clear()
            DataGridView.Columns.Clear()

            DataGridView.ColumnHeadersVisible = False
            DataGridView.RowHeadersVisible = False
            DataGridView.AllowUserToAddRows = False
            DataGridView.AllowUserToDeleteRows = False
            DataGridView.AllowUserToResizeColumns = False
            DataGridView.AllowUserToResizeRows = False
            DataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable
            DataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            DataGridView.EditMode = DataGridViewEditMode.EditOnF2
            DataGridView.MultiSelect = False
            DataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
            DataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            DataGridView.StandardTab = True


            DataGridView.Columns.Add(New DataGridViewTextBoxColumn With {
                .Name = COLUMN_NAME,
                .ReadOnly = True,
                .Resizable = False,
                .SortMode = DataGridViewColumnSortMode.NotSortable
            })
            DataGridView.Columns.Add(New DataGridViewTextBoxColumn With {
                .Name = COLUMN_VALUE,
                .Resizable = False,
                .SortMode = DataGridViewColumnSortMode.NotSortable,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            })
            DataGridView.Columns.Add(New DataGridViewTextBoxColumn With {
                .Visible = False,
                .Name = COLUMN_LENGTH,
                .ReadOnly = True,
                .Resizable = False,
                .SortMode = DataGridViewColumnSortMode.NotSortable
            })
            DataGridView.Columns.Add(New DataGridViewTextBoxColumn With {
                .Visible = False,
                .Name = COLUMN_INVALID,
                .ReadOnly = True,
                .Resizable = False,
                .SortMode = DataGridViewColumnSortMode.NotSortable
            })
            DataGridView.Columns.Add(New DataGridViewTextBoxColumn With {
                .Visible = False,
                .Name = COLUMN_EDITABLE,
                .ReadOnly = True,
                .Resizable = False,
                .SortMode = DataGridViewColumnSortMode.NotSortable
            })
            DataGridView.Columns.Add(New DataGridViewTextBoxColumn With {
                .Visible = False,
                .Name = COLUMN_TYPE,
                .ReadOnly = True,
                .Resizable = False,
                .SortMode = DataGridViewColumnSortMode.NotSortable
            })

            AddRow(My.Resources.DataInspector_Label_Area, 0, DataRowEnum.Area, False)
            AddRow(My.Resources.DataInspector_Label_File, 0, DataRowEnum.File, False)
            AddRow(My.Resources.DataInspector_Label_Region, 0, DataRowEnum.Description, False)
            AddRow(My.Resources.DataInspector_Label_CRC16, 0, DataRowEnum.CRC16, False)
            AddRow(My.Resources.DataInspector_Label_CRC32, 0, DataRowEnum.CRC32, False)
            AddRow(My.Resources.DataInspector_Label_Binary, 1, DataRowEnum.Binary,, 8)
            AddRow(My.Resources.DataInspector_Label_Int8, 1, DataRowEnum.Int8,, 4)
            AddRow(My.Resources.DataInspector_Label_UInt8, 1, DataRowEnum.UInt8,, 3)
            AddRow(My.Resources.DataInspector_Label_Int16, 2, DataRowEnum.Int16,, 6)
            AddRow(My.Resources.DataInspector_Label_UInt16, 2, DataRowEnum.UInt16,, 5)
            AddRow(My.Resources.DataInspector_Label_Int24, 3, DataRowEnum.Int24,, 9)
            AddRow(My.Resources.DataInspector_Label_UInt24, 3, DataRowEnum.UInt24,, 8)
            AddRow(My.Resources.DataInspector_Label_Int32, 4, DataRowEnum.Int32,, 11)
            AddRow(My.Resources.DataInspector_Label_UInt32, 4, DataRowEnum.UInt32,, 10)
            AddRow(My.Resources.DataInspector_Label_Int64, 8, DataRowEnum.Int64,, 21)
            AddRow(My.Resources.DataInspector_Label_UInt64, 8, DataRowEnum.UInt64,, 20)
            AddRow(My.Resources.DataInspector_Label_DOSDate, 2, DataRowEnum.DOSDate,, 10)
            AddRow(My.Resources.DataInspector_Label_DOSTime, 2, DataRowEnum.DOSTime,, 8)
            AddRow(My.Resources.DataInspector_Label_DOSDateTime, 4, DataRowEnum.DOSTimeDate,, 19)
            AddRow(My.Resources.DataInspector_Label_Bitstream, 0, DataRowEnum.Bitstream, False)
            AddRow(My.Resources.DataInspector_Label_WeakBits, 0, DataRowEnum.WeakBits, False)

            SetDataRow(DataRowEnum.Bitstream, Nothing, True, True)
            SetDataRow(DataRowEnum.WeakBits, Nothing, True, True)
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
                    DT = ExpandedDate.ExpandDate(CType(UInt16, UInt16))
                    If DT.IsValidDate Then
                        DOSDate = DT.DateObject.ToString(My.Resources.DataInspector_ISODate)
                    End If
                    DT = ExpandedDate.ExpandTime(CType(UInt16, UInt16))
                    If DT.IsValidDate Then
                        DosTime = DT.DateObject.ToString(My.Resources.DataInspector_ISOTime)
                    End If
                End If
                If Bytes.Length >= 3 Then
                    Int24 = MyBitConverter.ToInt24(Bytes, BigEndien)
                    UInt24 = MyBitConverter.ToUInt24(Bytes, BigEndien)
                End If
                If Bytes.Length >= 4 Then
                    Int32 = MyBitConverter.ToInt32(Bytes, BigEndien)
                    UInt32 = MyBitConverter.ToUInt32(Bytes, BigEndien)
                    DT = ExpandedDate.ExpandTimeDate(CType(UInt32, UInt32))
                    If DT.IsValidDate Then
                        DosTimeDate = DT.DateObject.ToString(My.Resources.DataInspector_ISODateTime)
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

        Public Sub SetDataRow(Index As DataRowEnum, Value As Object, [ReadOnly] As Boolean, Optional HideIfEmpty As Boolean = False, Optional Caption As String = "")
            Dim RowValue As Object
            Dim ForeColor As Color
            Dim Visible As Boolean
            Dim Invalid As Boolean

            If Value Is Nothing Then
                RowValue = My.Resources.DataInspector_Label_Invalid
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
                If Caption <> "" Then
                    .Cells.Item(0).Value = Caption
                End If
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
            Const INT24_MIN As Int32 = -8388608
            Const INT24_MAX As Int32 = 8388607
            Const UINT24_MIN As Int32 = 0
            Const UINT24_MAX As Int32 = 16777215
            Const YEAR_MIN As Integer = 1980
            Const YEAR_MAX As Integer = 2107

            Dim Row = DataGridView.Rows(e.RowIndex)
            Dim CellValue As String = Row.Cells.Item(COLUMN_VALUE).Value
            Dim DataType As DataRowEnum = Row.Cells.Item(COLUMN_TYPE).Value
            Dim Invalid As Boolean = Row.Cells.Item(COLUMN_INVALID).Value
            Dim Result As Boolean
            Dim ErrorMsg As String = ""

            If e.FormattedValue = My.Resources.DataInspector_Label_Invalid And Invalid Then
                Exit Sub
            End If

            Select Case DataType
                Case DataRowEnum.Binary
                    ErrorMsg = My.Resources.DataInspector_Error_Binary
                    Dim cValue As Byte
                    Try
                        cValue = Convert.ToByte(e.FormattedValue, 2)
                        Result = True
                    Catch
                        Result = False
                    End Try
                Case DataRowEnum.Int8
                    ErrorMsg = String.Format(My.Resources.DataInspector_Error_Integer, FormatThousands(SByte.MinValue), FormatThousands(SByte.MaxValue))
                    Dim cValue As SByte
                    Result = SByte.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.UInt8
                    ErrorMsg = String.Format(My.Resources.DataInspector_Error_Integer, FormatThousands(Byte.MinValue), FormatThousands(Byte.MaxValue))
                    Dim cValue As Byte
                    Result = Byte.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.Int16
                    ErrorMsg = String.Format(My.Resources.DataInspector_Error_Integer, FormatThousands(Int16.MinValue), FormatThousands(Int16.MaxValue))
                    Dim cValue As Int16
                    Result = Int16.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.UInt16
                    ErrorMsg = String.Format(My.Resources.DataInspector_Error_Integer, FormatThousands(UInt16.MinValue), FormatThousands(UInt16.MaxValue))
                    Dim cValue As UInt16
                    Result = UInt16.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.Int32
                    ErrorMsg = String.Format(My.Resources.DataInspector_Error_Integer, FormatThousands(Int32.MinValue), FormatThousands(Int32.MaxValue))
                    Dim cValue As Int32
                    Result = Int32.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.Int24
                    ErrorMsg = String.Format(My.Resources.DataInspector_Error_Integer, FormatThousands(INT24_MIN), FormatThousands(INT24_MAX))
                    Dim cValue As Int32
                    Result = Int32.TryParse(e.FormattedValue, cValue)
                    If Result Then
                        Result = cValue >= INT24_MIN And cValue <= INT24_MAX
                    End If
                Case DataRowEnum.UInt24
                    ErrorMsg = String.Format(My.Resources.DataInspector_Error_Integer, FormatThousands(UINT24_MIN), FormatThousands(UINT24_MAX))
                    Dim cValue As UInt32
                    Result = UInt32.TryParse(e.FormattedValue, cValue)
                    If Result Then
                        Result = cValue >= UINT24_MIN And cValue <= UINT24_MAX
                    End If
                Case DataRowEnum.UInt32
                    ErrorMsg = String.Format(My.Resources.DataInspector_Error_Integer, FormatThousands(UInt32.MinValue), FormatThousands(UInt32.MaxValue))
                    Dim cValue As UInt32
                    Result = UInt32.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.Int64
                    ErrorMsg = String.Format(My.Resources.DataInspector_Error_Integer, FormatThousands(Int64.MinValue), FormatThousands(Int64.MaxValue))
                    Dim cValue As Int64
                    Result = Int64.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.UInt64
                    ErrorMsg = String.Format(My.Resources.DataInspector_Error_Integer, FormatThousands(UInt64.MinValue), FormatThousands(UInt64.MaxValue))
                    Dim cValue As UInt64
                    Result = UInt64.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.DOSDate
                    ErrorMsg = My.Resources.DataInspector_Error_DOSDate
                    Dim cValue As Date
                    Result = Date.TryParse(e.FormattedValue, cValue)
                    If Result Then
                        If cValue.Year < YEAR_MIN Or cValue.Year > YEAR_MAX Then
                            Result = False
                        End If
                    End If
                Case DataRowEnum.DOSTime
                    ErrorMsg = My.Resources.DataInspector_Error_DOSTime
                    Dim cValue As Date
                    Result = Date.TryParse(e.FormattedValue, cValue)
                Case DataRowEnum.DOSTimeDate
                    ErrorMsg = My.Resources.DataInspector_Error_DOSDateTime
                    Dim cValue As Date
                    Result = Date.TryParse(e.FormattedValue, cValue)
                    If Result Then
                        If cValue.Year < YEAR_MIN Or cValue.Year > YEAR_MAX Then
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
            Dim Cell As DataGridViewTextBoxCell = DataGridView.Rows.Item(Row).Cells.Item(COLUMN_VALUE)
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
