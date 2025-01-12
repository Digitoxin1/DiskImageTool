Imports System.ComponentModel
Imports System.Windows.Forms.Design

<ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip), DebuggerStepThrough()>
Public Class ToolStripSpringTextBox
    Inherits ToolStripTextBox

    Private _MaxWidth As Single

    <Category("Layout")>
    Public Property MaxWidth As Single
        Get
            Return _MaxWidth
        End Get
        Set(value As Single)
            _MaxWidth = value
        End Set
    End Property

    Public Overrides Function GetPreferredSize(constrainingSize As Size) As Size
        ' Use the default size if the text box is on the overflow menu
        ' or is on a vertical ToolStrip.
        If IsOnOverflow Or Owner.Orientation = Orientation.Vertical Then
            Return DefaultSize
        End If

        ' Declare a variable to store the total available width as 
        ' it is calculated, starting with the display width of the 
        ' owning ToolStrip.
        Dim width As Int32 = Owner.DisplayRectangle.Width

        ' Subtract the width of the overflow button if it is displayed. 
        If Owner.OverflowButton.Visible Then
            width = width - Owner.OverflowButton.Width -
                Owner.OverflowButton.Margin.Horizontal()
        End If

        ' Declare a variable to maintain a count of ToolStripSpringTextBox 
        ' items currently displayed in the owning ToolStrip. 
        Dim springBoxCount As Int32 = 0

        For Each item As ToolStripItem In Owner.Items

            ' Ignore items on the overflow menu.
            If item.IsOnOverflow Or Not item.Visible Then Continue For

            If TypeOf item Is ToolStripSpringTextBox Then
                ' For ToolStripSpringTextBox items, increment the count and 
                ' subtract the margin width from the total available width.
                springBoxCount += 1
                width -= item.Margin.Horizontal
            Else
                ' For all other items, subtract the full width from the total
                ' available width.
                width = width - item.Width - item.Margin.Horizontal
            End If
        Next

        ' If there are multiple ToolStripSpringTextBox items in the owning
        ' ToolStrip, divide the total available width between them. 
        If springBoxCount > 1 Then width = CInt(width / springBoxCount)

        ' If the available width is less than the default width, use the
        ' default width, forcing one or more items onto the overflow menu.
        If width < DefaultSize.Width Then width = DefaultSize.Width

        ' Retrieve the preferred size from the base class, but change the
        ' width to the calculated width. 
        Dim preferredSize As Size = MyBase.GetPreferredSize(constrainingSize)
        If _MaxWidth > 0 And width > MaxWidth Then
            width = MaxWidth
        End If
        preferredSize.Width = width

        Return preferredSize
    End Function
End Class