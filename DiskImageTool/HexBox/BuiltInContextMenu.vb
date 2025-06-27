Imports System.ComponentModel

Namespace Hb.Windows.Forms
    ''' <summary>
    ''' Defines a build-in ContextMenuStrip manager for HexBox control to show Copy, Cut, Paste menu in contextmenu of the control.
    ''' </summary>
    <TypeConverterAttribute(GetType(ExpandableObjectConverter))>
    Public NotInheritable Class BuiltInContextMenu
        Inherits Component
        ''' <summary>
        ''' Contains the HexBox control.
        ''' </summary>
        Private ReadOnly _hexBox As Forms.HexBox

        ''' <summary>
        ''' Contains the ContextMenuStrip control.
        ''' </summary>
        Private _contextMenuStrip As ContextMenuStrip

        Private _copyMenuItemImage As Image = Nothing
        Private _copyMenuItemText As String

        ''' <summary>
        ''' Contains the "Copy"-ToolStripMenuItem object.
        ''' </summary>
        Private _copyToolStripMenuItem As ToolStripMenuItem

        Private _cutMenuItemImage As Image = Nothing
        Private _cutMenuItemText As String

        ''' <summary>
        ''' Contains the "Cut"-ToolStripMenuItem object.
        ''' </summary>
        Private _cutToolStripMenuItem As ToolStripMenuItem

        Private _pasteMenuItemImage As Image = Nothing
        Private _pasteMenuItemText As String

        ''' <summary>
        ''' Contains the "Paste"-ToolStripMenuItem object.
        ''' </summary>
        Private _pasteToolStripMenuItem As ToolStripMenuItem

        Private _selectAllMenuItemImage As Image = Nothing
        Private _selectAllMenuItemText As String = Nothing

        ''' <summary>
        ''' Contains the "Select All"-ToolStripMenuItem object.
        ''' </summary>
        Private _selectAllToolStripMenuItem As ToolStripMenuItem

        ''' <summary>
        ''' Initializes a new instance of BuildInContextMenu class.
        ''' </summary>
        ''' <paramname="hexBox">the HexBox control</param>
        Friend Sub New(hexBox As Forms.HexBox)
            _hexBox = hexBox
            AddHandler _hexBox.ByteProviderChanged, New EventHandler(AddressOf HexBox_ByteProviderChanged)
        End Sub

        ''' <summary>
        ''' Gets or sets the image of the "Copy" ContextMenuStrip item.
        ''' </summary>
        <Category("BuiltIn-ContextMenu")>
        Public Property CopyMenuItemImage As Image
            Get
                Return _copyMenuItemImage
            End Get
            Set(value As Image)
                _copyMenuItemImage = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the custom text of the "Copy" ContextMenuStrip item.
        ''' </summary>
        <Category("BuiltIn-ContextMenu"), Localizable(True)>
        Public Property CopyMenuItemText As String
            Get
                Return _copyMenuItemText
            End Get
            Set(value As String)
                _copyMenuItemText = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the image of the "Cut" ContextMenuStrip item.
        ''' </summary>
        <Category("BuiltIn-ContextMenu")>
        Public Property CutMenuItemImage As Image
            Get
                Return _cutMenuItemImage
            End Get
            Set(value As Image)
                _cutMenuItemImage = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the custom text of the "Cut" ContextMenuStrip item.
        ''' </summary>
        <Category("BuiltIn-ContextMenu"), Localizable(True)>
        Public Property CutMenuItemText As String
            Get
                Return _cutMenuItemText
            End Get
            Set(value As String)
                _cutMenuItemText = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the image of the "Paste" ContextMenuStrip item.
        ''' </summary>
        <Category("BuiltIn-ContextMenu")>
        Public Property PasteMenuItemImage As Image
            Get
                Return _pasteMenuItemImage
            End Get
            Set(value As Image)
                _pasteMenuItemImage = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the custom text of the "Paste" ContextMenuStrip item.
        ''' </summary>
        <Category("BuiltIn-ContextMenu"), Localizable(True)>
        Public Property PasteMenuItemText As String
            Get
                Return _pasteMenuItemText
            End Get
            Set(value As String)
                _pasteMenuItemText = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the image of the "Select All" ContextMenuStrip item.
        ''' </summary>
        <Category("BuiltIn-ContextMenu")>
        Public Property SelectAllMenuItemImage As Image
            Get
                Return _selectAllMenuItemImage
            End Get
            Set(value As Image)
                _selectAllMenuItemImage = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the custom text of the "Select All" ContextMenuStrip item.
        ''' </summary>
        <Category("BuiltIn-ContextMenu"), Localizable(True)>
        Public Property SelectAllMenuItemText As String
            Get
                Return _selectAllMenuItemText
            End Get
            Set(value As String)
                _selectAllMenuItemText = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the text of the "Copy" ContextMenuStrip item.
        ''' </summary>
        Friend ReadOnly Property CopyMenuItemTextInternal As String
            Get
                Return If(Not String.IsNullOrEmpty(CopyMenuItemText), CopyMenuItemText, "Copy")
            End Get
        End Property

        ''' <summary>
        ''' Gets the text of the "Cut" ContextMenuStrip item.
        ''' </summary>
        Friend ReadOnly Property CutMenuItemTextInternal As String
            Get
                Return If(Not String.IsNullOrEmpty(CutMenuItemText), CutMenuItemText, "Cut")
            End Get
        End Property

        ''' <summary>
        ''' Gets the text of the "Paste" ContextMenuStrip item.
        ''' </summary>
        Friend ReadOnly Property PasteMenuItemTextInternal As String
            Get
                Return If(Not String.IsNullOrEmpty(PasteMenuItemText), PasteMenuItemText, "Paste")
            End Get
        End Property

        ''' <summary>
        ''' Gets the text of the "Select All" ContextMenuStrip item.
        ''' </summary>
        Friend ReadOnly Property SelectAllMenuItemTextInternal As String
            Get
                Return If(Not String.IsNullOrEmpty(SelectAllMenuItemText), SelectAllMenuItemText, "SelectAll")
            End Get
        End Property

        ''' <summary>
        ''' Before opening the ContextMenuStrip, we manage the availability of the items.
        ''' </summary>
        ''' <paramname="sender">the sender object</param>
        ''' <paramname="e">the event data</param>
        Private Sub BuildInContextMenuStrip_Opening(sender As Object, e As CancelEventArgs)
            _cutToolStripMenuItem.Enabled = _hexBox.CanCut()
            _copyToolStripMenuItem.Enabled = _hexBox.CanCopy()
            _pasteToolStripMenuItem.Enabled = _hexBox.CanPaste()
            _selectAllToolStripMenuItem.Enabled = _hexBox.CanSelectAll()
        End Sub

        ''' <summary>
        ''' Assigns the ContextMenuStrip control to the HexBox control.
        ''' </summary>
        Private Sub CheckBuiltInContextMenu()
            If Forms.Util.DesignMode Then Return

            If _contextMenuStrip Is Nothing Then
                Dim cms As New ContextMenuStrip()
                _cutToolStripMenuItem = New ToolStripMenuItem(CutMenuItemTextInternal, CutMenuItemImage, New EventHandler(AddressOf CutMenuItem_Click))
                cms.Items.Add(_cutToolStripMenuItem)
                _copyToolStripMenuItem = New ToolStripMenuItem(CopyMenuItemTextInternal, CopyMenuItemImage, New EventHandler(AddressOf CopyMenuItem_Click))
                cms.Items.Add(_copyToolStripMenuItem)
                _pasteToolStripMenuItem = New ToolStripMenuItem(PasteMenuItemTextInternal, PasteMenuItemImage, New EventHandler(AddressOf PasteMenuItem_Click))
                cms.Items.Add(_pasteToolStripMenuItem)

                cms.Items.Add(New ToolStripSeparator())

                _selectAllToolStripMenuItem = New ToolStripMenuItem(SelectAllMenuItemTextInternal, SelectAllMenuItemImage, New EventHandler(AddressOf SelectAllMenuItem_Click))
                cms.Items.Add(_selectAllToolStripMenuItem)
                AddHandler cms.Opening, New CancelEventHandler(AddressOf BuildInContextMenuStrip_Opening)

                _contextMenuStrip = cms
            End If

            If _hexBox.ByteProvider Is Nothing AndAlso _hexBox.ContextMenuStrip Is _contextMenuStrip Then
                _hexBox.ContextMenuStrip = Nothing
            ElseIf _hexBox.ByteProvider IsNot Nothing AndAlso _hexBox.ContextMenuStrip Is Nothing Then
                _hexBox.ContextMenuStrip = _contextMenuStrip
            End If
        End Sub

        ''' <summary>
        ''' The handler for the "Copy"-Click event
        ''' </summary>
        ''' <paramname="sender">the sender object</param>
        ''' <paramname="e">the event data</param>
        Private Sub CopyMenuItem_Click(sender As Object, e As EventArgs)
            _hexBox.Copy()
        End Sub

        ''' <summary>
        ''' The handler for the "Cut"-Click event
        ''' </summary>
        ''' <paramname="sender">the sender object</param>
        ''' <paramname="e">the event data</param>
        Private Sub CutMenuItem_Click(sender As Object, e As EventArgs)
            _hexBox.Cut()
        End Sub

        ''' <summary>
        ''' If ByteProvider
        ''' </summary>
        ''' <paramname="sender">the sender object</param>
        ''' <paramname="e">the event data</param>
        Private Sub HexBox_ByteProviderChanged(sender As Object, e As EventArgs)
            CheckBuiltInContextMenu()
        End Sub
        Private Sub InitializeComponent()

        End Sub

        ''' <summary>
        ''' The handler for the "Paste"-Click event
        ''' </summary>
        ''' <paramname="sender">the sender object</param>
        ''' <paramname="e">the event data</param>
        Private Sub PasteMenuItem_Click(sender As Object, e As EventArgs)
            _hexBox.Paste()
        End Sub

        ''' <summary>
        ''' The handler for the "Select All"-Click event
        ''' </summary>
        ''' <paramname="sender">the sender object</param>
        ''' <paramname="e">the event data</param>
        Private Sub SelectAllMenuItem_Click(sender As Object, e As EventArgs)
            _hexBox.SelectAll()
        End Sub
    End Class
End Namespace
