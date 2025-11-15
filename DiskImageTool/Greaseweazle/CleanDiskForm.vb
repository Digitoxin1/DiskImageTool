Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Greaseweazle
    Public Class CleanDiskForm
        Inherits BaseForm
        Private WithEvents ButtonProcess As Button
        Private WithEvents ButtonReset As Button
        Private WithEvents ComboImageDrives As ComboBox
        Private WithEvents Process As ConsoleProcessRunner
        Private NumericCyls As NumericUpDown
        Private NumericPasses As NumericUpDown
        Private NumericLinger As NumericUpDown

        Public Sub New()
            MyBase.New(False)
            InitializeControls()

            Process = New ConsoleProcessRunner With {
                .EventContext = Threading.SynchronizationContext.Current
            }

            Me.Text = My.Resources.Label_CleanDisk
            PopulateDrives(ComboImageDrives, FloppyMediaType.MediaUnknown)
        End Sub

        Private Sub InitializeControls()
            Dim DriveLabel = New Label With {
                .Text = My.Resources.Label_Drive,
                .Anchor = AnchorStyles.Right,
                .AutoSize = True
            }

            ComboImageDrives = New ComboBox With {
                .Anchor = AnchorStyles.Left Or AnchorStyles.Right,
                .Width = 180
            }

            Dim CylsLabel = New Label With {
                .Text = "Cylinders",
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(6, 3, 3, 3)
            }

            NumericCyls = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 45,
                .Minimum = 1,
                .Maximum = 80,
                .Value = CommandLineBuilder.DEFAULT_CYLS
            }

            Dim PassesLabel = New Label With {
                .Text = "Passes",
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(6, 3, 3, 3)
            }

            NumericPasses = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 45,
                .Minimum = 1,
                .Maximum = 9,
                .Value = CommandLineBuilder.DEFAULT_PASSES
            }

            Dim LingerLabel = New Label With {
                .Text = "Linger",
                .Anchor = AnchorStyles.Left,
                .AutoSize = True,
                .Margin = New Padding(6, 3, 3, 3)
            }

            NumericLinger = New NumericUpDown With {
                .Anchor = AnchorStyles.Left,
                .Width = 60,
                .Minimum = 1,
                .Maximum = 1000,
                .Value = CommandLineBuilder.DEFAULT_LINGER
            }

            Dim ButtonContainer = New FlowLayoutPanel With {
                .FlowDirection = FlowDirection.RightToLeft,
                .AutoSize = True,
                .Anchor = AnchorStyles.Right,
                .Margin = New Padding(3, 3, 0, 3)
            }

            ButtonProcess = New Button With {
                .Width = 75,
                .Margin = New Padding(3, 3, 3, 3),
                .Text = My.Resources.Label_Erase
            }

            ButtonReset = New Button With {
                .Width = 75,
                .Margin = New Padding(3, 3, 0, 3),
                .Text = My.Resources.Label_Reset
            }

            ButtonContainer.Controls.Add(ButtonReset)
            ButtonContainer.Controls.Add(ButtonProcess)

            ButtonOk.Visible = False
            ButtonCancel.Text = My.Resources.Label_Close

            Dim Row As Integer

            With TableLayoutPanelMain
                .SuspendLayout()
                .RowCount = 2
                .ColumnCount = 9
                .Dock = DockStyle.Fill

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

                .ColumnStyles(2).SizeType = SizeType.Percent
                .ColumnStyles(2).Width = 100

                Row = 0
                .Controls.Add(DriveLabel, 0, Row)
                .Controls.Add(ComboImageDrives, 1, Row)

                .Controls.Add(CylsLabel, 3, Row)
                .Controls.Add(NumericCyls, 4, Row)

                .Controls.Add(PassesLabel, 5, Row)
                .Controls.Add(NumericPasses, 6, Row)

                .Controls.Add(LingerLabel, 7, Row)
                .Controls.Add(NumericLinger, 8, Row)

                Row = 1
                .Controls.Add(ButtonContainer, 5, Row)
                .SetColumnSpan(ButtonContainer, 4)

                .ResumeLayout()
                .Left = (.Parent.ClientSize.Width - .Width) \ 2
            End With
        End Sub
    End Class
End Namespace
