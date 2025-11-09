Module FloppyGrid
    Public Function FloppyGridInit(TLP As TableLayoutPanel, LabelText As String, Tracks As UShort) As TableLayoutPanel
        Const COL_COUNT As Integer = 10
        Const ROW_COUNT As Integer = 8
        Const LABEL_WIDTH As Integer = 17
        Const COL_WIDTH As Integer = 20
        Const ROW_HEIGHT As Integer = 22
        Const PADDING As Integer = 2

        Dim objLabel As Label

        ' Prepare outer table
        TLP.SuspendLayout()
        TLP.Controls.Clear()
        TLP.ColumnStyles.Clear()
        TLP.RowStyles.Clear()

        TLP.AutoSize = True
        TLP.AutoSizeMode = AutoSizeMode.GrowAndShrink
        TLP.GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        TLP.TabStop = False

        TLP.ColumnCount = COL_COUNT + 2 ' header + data + spacer
        TLP.RowCount = ROW_COUNT + 3    ' header + data + spacer + footer

        TLP.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, LABEL_WIDTH))

        For Col = 1 To COL_COUNT
            TLP.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, COL_WIDTH))
            objLabel = AddLabel(Col - 1, SystemColors.Control)
            TLP.Controls.Add(objLabel, Col, 0)
        Next

        TLP.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, PADDING))

        TLP.RowStyles.Add(New RowStyle(SizeType.Absolute, ROW_HEIGHT - 1))

        For Row = 1 To ROW_COUNT
            TLP.RowStyles.Add(New RowStyle(SizeType.Absolute, ROW_HEIGHT))
            objLabel = AddLabel(Row - 1, SystemColors.Control)
            TLP.Controls.Add(objLabel, 0, Row)
        Next

        TLP.RowStyles.Add(New RowStyle(SizeType.Absolute, PADDING))
        TLP.RowStyles.Add(New RowStyle(SizeType.AutoSize))

        objLabel = AddLabel(LabelText, SystemColors.Control)
        TLP.Controls.Add(objLabel, 1, ROW_COUNT + 2)
        TLP.SetColumnSpan(objLabel, COL_COUNT + 1)

        Dim InnerPanel As New TableLayoutPanel With {
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
            .Margin = New Padding(0),
            .Padding = New Padding(0),
            .GrowStyle = TableLayoutPanelGrowStyle.FixedSize,
            .ColumnCount = COL_COUNT,
            .RowCount = ROW_COUNT,
            .TabStop = False
        }

        Try
            InnerPanel.GetType() _
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic) _
            .SetValue(InnerPanel, True, Nothing)
        Catch
        End Try

        InnerPanel.SuspendLayout()
        InnerPanel.ColumnStyles.Clear()
        InnerPanel.RowStyles.Clear()

        For Col = 0 To COL_COUNT - 1
            InnerPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, COL_WIDTH - 1))
        Next

        For Row = 0 To ROW_COUNT - 1
            InnerPanel.RowStyles.Add(New RowStyle(SizeType.Absolute, ROW_HEIGHT - 1))
        Next

        For Row = 0 To ROW_COUNT - 1
            For Col = 0 To COL_COUNT - 1
                Dim CurrentTrack As UShort = Row * COL_COUNT + Col

                Dim TrackColor As Color
                If CurrentTrack < Tracks Then
                    TrackColor = Color.White
                Else
                    TrackColor = Color.LightGray
                End If
                objLabel = AddLabel("", TrackColor)
                InnerPanel.Controls.Add(objLabel, Col, Row)
            Next
        Next
        InnerPanel.ResumeLayout(False)

        TLP.Controls.Add(InnerPanel, 1, 1)
        TLP.SetColumnSpan(InnerPanel, COL_COUNT + 1)
        TLP.SetRowSpan(InnerPanel, ROW_COUNT + 1)

        TLP.ResumeLayout(True)

        Return InnerPanel
    End Function

    Public Sub FloppyGridSetLabel(TLP As TableLayoutPanel, Track As UShort, Text As String, BackColor As Color)
        Dim Row As Integer = Track \ 10
        Dim Col As Integer = Track Mod 10
        Dim objLabel As Label = TLP.GetControlFromPosition(Col, Row)
        objLabel.Text = Text
        objLabel.BackColor = BackColor
    End Sub

    Private Function AddLabel(Text As String, BackColor As Color) As Label
        Dim objLabel As New Label With {
            .BorderStyle = BorderStyle.None,
            .Margin = New Padding(0),
            .TextAlign = ContentAlignment.MiddleCenter,
            .UseMnemonic = False,
            .AutoSize = False,
            .Dock = DockStyle.Fill,
            .BackColor = BackColor,
            .Text = Text
        }

        Return objLabel
    End Function
End Module
