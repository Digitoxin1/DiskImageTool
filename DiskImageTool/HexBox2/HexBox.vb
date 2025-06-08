Imports System.ComponentModel
Imports System.Security.Permissions
Imports System.Windows.Forms.VisualStyles
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Runtime.InteropServices
Imports System.IO

Namespace Hb.Windows.Forms
    ''' <summary>
    ''' Represents a hex box control.
    ''' </summary>

    Public Class HexBox
        Inherits Control
#Region "IKeyInterpreter interface"
        ''' <summary>
        ''' Defines a user input handler such as for mouse and keyboard input
        ''' </summary>
        Friend Interface IKeyInterpreter
            ''' <summary>
            ''' Activates mouse events
            ''' </summary>
            Sub Activate()
            Sub ClearShiftDown()

            ''' <summary>
            ''' Deactivate mouse events
            ''' </summary>
            Sub Deactivate()

            ''' <summary>
            ''' Gives some information about where to place the caret.
            ''' </summary>
            ''' <paramname="byteIndex">the index of the byte</param>
            ''' <returns>the position where the caret is to place.</returns>
            Function GetCaretPointF(byteIndex As Long) As PointF

            ''' <summary>
            ''' Preprocesses WM_CHAR window message.
            ''' </summary>
            ''' <paramname="m">the Message object to process.</param>
            ''' <returns>True, if the message was processed.</returns>
            Function PreProcessWmChar(ByRef m As Message) As Boolean

            ''' <summary>
            ''' Preprocesses WM_KEYDOWN window message.
            ''' </summary>
            ''' <paramname="m">the Message object to process.</param>
            ''' <returns>True, if the message was processed.</returns>
            Function PreProcessWmKeyDown(ByRef m As Message) As Boolean

            ''' <summary>
            ''' Preprocesses WM_KEYUP window message.
            ''' </summary>
            ''' <paramname="m">the Message object to process.</param>
            ''' <returns>True, if the message was processed.</returns>
            Function PreProcessWmKeyUp(ByRef m As Message) As Boolean
        End Interface
#End Region

#Region "EmptyKeyInterpreter class"
        ''' <summary>
        ''' Represents an empty input handler without any functionality. 
        ''' If is set ByteProvider to null, then this interpreter is used.
        ''' </summary>
        Friend Class EmptyKeyInterpreter
            Implements IKeyInterpreter

            Private ReadOnly _hexBox As HexBox

            Public Sub New(hexBox As HexBox)
                _hexBox = hexBox
            End Sub

#Region "IKeyInterpreter Members"
            Public Sub Activate() Implements IKeyInterpreter.Activate
            End Sub
            Public Sub ClearShiftDown() Implements IKeyInterpreter.ClearShiftDown
            End Sub
            Public Sub Deactivate() Implements IKeyInterpreter.Deactivate
            End Sub

            Public Function GetCaretPointF(byteIndex As Long) As PointF Implements IKeyInterpreter.GetCaretPointF
                Return New PointF()
            End Function

            Public Function PreProcessWmChar(ByRef m As Message) As Boolean Implements IKeyInterpreter.PreProcessWmChar
                Return _hexBox.BasePreProcessMessage(m)
            End Function

            Public Function PreProcessWmKeyDown(ByRef m As Message) As Boolean Implements IKeyInterpreter.PreProcessWmKeyDown
                Return _hexBox.BasePreProcessMessage(m)
            End Function

            Public Function PreProcessWmKeyUp(ByRef m As Message) As Boolean Implements IKeyInterpreter.PreProcessWmKeyUp
                Return _hexBox.BasePreProcessMessage(m)
            End Function
#End Region
        End Class
#End Region

#Region "KeyInterpreter class"
        ''' <summary>
        ''' Handles user input such as mouse and keyboard input during hex view edit
        ''' </summary>
        Friend Class KeyInterpreter
            Implements IKeyInterpreter

            ''' <summary>
            ''' Delegate for key-down processing.
            ''' </summary>
            ''' <paramname="m">the message object contains key data information</param>
            ''' <returns>True, if the message was processed</returns>
            Friend Delegate Function MessageDelegate(ByRef m As Message) As Boolean

#Region "Fields"
            ''' <summary>
            ''' Contains the parent HexBox control
            ''' </summary>
            Protected _hexBox As HexBox

            ''' <summary>
            ''' Contains True, if shift key is down
            ''' </summary>
            Protected _shiftDown As Boolean
            ''' <summary>
            ''' Contains the current mouse selection position info
            ''' </summary>
            Private _bpi As Forms.BytePositionInfo

            ''' <summary>
            ''' Contains the selection start position info
            ''' </summary>
            Private _bpiStart As Forms.BytePositionInfo

            ''' <summary>
            ''' Contains all message handlers of key interpreter key down message
            ''' </summary>
            Private _messageHandlers As Dictionary(Of Keys, MessageDelegate)

            ''' <summary>
            ''' Contains True, if mouse is down
            ''' </summary>
            Private _mouseDown As Boolean
#End Region

#Region "Ctors"
            Public Sub New(hexBox As HexBox)
                _hexBox = hexBox
            End Sub
#End Region

#Region "Activate, Deactive methods"
            Public Overridable Sub Activate() Implements IKeyInterpreter.Activate
                AddHandler _hexBox.MouseDown, New MouseEventHandler(AddressOf BeginMouseSelection)
                AddHandler _hexBox.MouseMove, New MouseEventHandler(AddressOf UpdateMouseSelection)
                AddHandler _hexBox.MouseUp, New MouseEventHandler(AddressOf EndMouseSelection)
            End Sub

            Public Overridable Sub Deactivate() Implements IKeyInterpreter.Deactivate
                RemoveHandler _hexBox.MouseDown, New MouseEventHandler(AddressOf BeginMouseSelection)
                RemoveHandler _hexBox.MouseMove, New MouseEventHandler(AddressOf UpdateMouseSelection)
                RemoveHandler _hexBox.MouseUp, New MouseEventHandler(AddressOf EndMouseSelection)
            End Sub
#End Region

#Region "Mouse selection methods"
            Private Sub BeginMouseSelection(sender As Object, e As MouseEventArgs)
                Debug.WriteLine("BeginMouseSelection()", "KeyInterpreter")

                If e.Button <> MouseButtons.Left Then Return

                _mouseDown = True

                If Not _shiftDown Then
                    _bpiStart = New Forms.BytePositionInfo(_hexBox._bytePos, _hexBox._byteCharacterPos)
                    _hexBox.ReleaseSelection()
                Else
                    UpdateMouseSelection(Me, e)
                End If
            End Sub

            Private Sub EndMouseSelection(sender As Object, e As MouseEventArgs)
                _mouseDown = False
            End Sub

            Private Sub UpdateMouseSelection(sender As Object, e As MouseEventArgs)
                If Not _mouseDown Then Return

                _bpi = GetBytePositionInfo(New Point(e.X, e.Y))
                Dim selEnd As Long = _bpi.Index
                Dim realselStart As Long
                Dim realselLength As Long

                If selEnd < _bpiStart.Index Then
                    realselStart = selEnd
                    realselLength = _bpiStart.Index - selEnd
                ElseIf selEnd > _bpiStart.Index Then
                    realselStart = _bpiStart.Index
                    realselLength = selEnd - realselStart
                Else
                    realselStart = _hexBox._bytePos
                    realselLength = 0
                End If

                If realselStart <> _hexBox._bytePos OrElse realselLength <> _hexBox._selectionLength Then
                    _hexBox.InternalSelect(realselStart, realselLength)
                    _hexBox.ScrollByteIntoView(_bpi.Index)
                End If
            End Sub
#End Region

#Region "PrePrcessWmKeyDown methods"
            Public Overridable Function PreProcessWmKeyDown(ByRef m As Message) As Boolean
                Debug.WriteLine("PreProcessWmKeyDown(ref Message m)", "KeyInterpreter")

                Dim vc As Keys = CType(m.WParam.ToInt32(), Keys)
                Dim keyData As Keys = vc Or Control.ModifierKeys

                ' detect whether key down event should be raised
                Dim hasMessageHandler As Boolean = Me.MessageHandlers.ContainsKey(keyData)
                If hasMessageHandler AndAlso RaiseKeyDown(keyData) Then
                    Return True
                End If

                Dim messageHandler As MessageDelegate
                If hasMessageHandler Then
                    messageHandler = Me.MessageHandlers(keyData)
                Else
                    messageHandler = AddressOf PreProcessWmKeyDown_Default
                End If

                Return messageHandler(m)
            End Function

            Protected Overridable Function PreProcessWmKeyDown_Back(ByRef m As Message) As Boolean
                If Not _hexBox._byteProvider.SupportsDeleteBytes() Then Return True

                If _hexBox.ReadOnly Then Return True

                Dim pos = _hexBox._bytePos
                Dim sel = _hexBox._selectionLength
                Dim cp = _hexBox._byteCharacterPos

                Dim startDelete = If(cp = 0 AndAlso sel = 0, pos - 1, pos)
                If startDelete < 0 AndAlso sel < 1 Then Return True

                Dim bytesToDelete = If(sel > 0, sel, 1)
                _hexBox._byteProvider.DeleteBytes(Math.Max(0, startDelete), bytesToDelete)
                _hexBox.UpdateScrollSize()

                If sel = 0 Then PerformPosMoveLeftByte()

                _hexBox.ReleaseSelection()
                _hexBox.Invalidate()

                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_ControlC(ByRef m As Message) As Boolean
                _hexBox.Copy()
                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_ControlShiftC(ByRef m As Message) As Boolean
                _hexBox.CopyHex()
                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_ControlShiftV(ByRef m As Message) As Boolean
                _hexBox.PasteHex()
                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_ControlV(ByRef m As Message) As Boolean
                _hexBox.Paste()
                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_ControlX(ByRef m As Message) As Boolean
                _hexBox.Cut()
                Return True
            End Function

            Protected Function PreProcessWmKeyDown_Default(ByRef m As Message) As Boolean
                _hexBox.ScrollByteIntoView()
                Return _hexBox.BasePreProcessMessage(m)
            End Function

            Protected Overridable Function PreProcessWmKeyDown_Delete(ByRef m As Message) As Boolean
                If Not _hexBox._byteProvider.SupportsDeleteBytes() Then Return True

                If _hexBox.ReadOnly Then Return True

                Dim pos = _hexBox._bytePos
                Dim sel = _hexBox._selectionLength

                If pos >= _hexBox._byteProvider.Length Then Return True

                Dim bytesToDelete = If(sel > 0, sel, 1)
                _hexBox._byteProvider.DeleteBytes(pos, bytesToDelete)

                _hexBox.UpdateScrollSize()
                _hexBox.ReleaseSelection()
                _hexBox.Invalidate()

                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_Down(ByRef m As Message) As Boolean
                Dim pos = _hexBox._bytePos
                Dim cp = _hexBox._byteCharacterPos

                If pos = _hexBox._byteProvider.Length AndAlso cp = 0 Then Return True

                pos = Math.Min(_hexBox._byteProvider.Length, pos + _hexBox._iHexMaxHBytes)

                If pos = _hexBox._byteProvider.Length Then cp = 0

                _hexBox.SetPosition(pos, cp)

                If pos > _hexBox._endByte - 1 Then
                    _hexBox.PerformScrollLineDown()
                End If

                _hexBox.UpdateCaret()
                _hexBox.ScrollByteIntoView()
                _hexBox.ReleaseSelection()
                _hexBox.Invalidate()

                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_End(ByRef m As Message) As Boolean
                Dim pos = _hexBox._bytePos
                Dim cp As Integer

                If pos >= _hexBox._byteProvider.Length - 1 Then Return True

                pos = _hexBox._byteProvider.Length
                cp = 0
                _hexBox.SetPosition(pos, cp)

                _hexBox.ScrollByteIntoView()
                _hexBox.UpdateCaret()
                _hexBox.ReleaseSelection()

                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_Home(ByRef m As Message) As Boolean
                Dim pos = _hexBox._bytePos
                Dim cp As Integer

                If pos < 1 Then Return True

                pos = 0
                cp = 0
                _hexBox.SetPosition(pos, cp)

                _hexBox.ScrollByteIntoView()
                _hexBox.UpdateCaret()
                _hexBox.ReleaseSelection()

                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_Left(ByRef m As Message) As Boolean
                Return PerformPosMoveLeft()
            End Function

            Protected Overridable Function PreProcessWmKeyDown_PageDown(ByRef m As Message) As Boolean
                Dim pos = _hexBox._bytePos
                Dim cp = _hexBox._byteCharacterPos

                If pos = _hexBox._byteProvider.Length AndAlso cp = 0 Then Return True

                pos = Math.Min(_hexBox._byteProvider.Length, pos + _hexBox._iHexMaxBytes)

                If pos = _hexBox._byteProvider.Length Then cp = 0

                _hexBox.SetPosition(pos, cp)

                If pos > _hexBox._endByte - 1 Then
                    _hexBox.PerformScrollPageDown()
                End If

                _hexBox.ReleaseSelection()
                _hexBox.UpdateCaret()
                _hexBox.Invalidate()

                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_PageUp(ByRef m As Message) As Boolean
                Dim pos = _hexBox._bytePos
                Dim cp = _hexBox._byteCharacterPos

                If pos = 0 AndAlso cp = 0 Then Return True

                pos = Math.Max(0, pos - _hexBox._iHexMaxBytes)
                If pos = 0 Then Return True

                _hexBox.SetPosition(pos)

                If pos < _hexBox._startByte Then
                    _hexBox.PerformScrollPageUp()
                End If

                _hexBox.ReleaseSelection()
                _hexBox.UpdateCaret()
                _hexBox.Invalidate()
                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_Right(ByRef m As Message) As Boolean
                Return PerformPosMoveRight()
            End Function

            Protected Overridable Function PreProcessWmKeyDown_ShiftDown(ByRef m As Message) As Boolean
                Dim pos = _hexBox._bytePos
                Dim sel = _hexBox._selectionLength

                Dim max As Long = _hexBox._byteProvider.Length

                If pos + sel + _hexBox._iHexMaxHBytes > max Then Return True

                If _bpiStart.Index <= pos Then
                    sel += _hexBox._iHexMaxHBytes
                    _hexBox.InternalSelect(pos, sel)
                    _hexBox.ScrollByteIntoView(pos + sel)
                Else
                    sel -= _hexBox._iHexMaxHBytes
                    If sel < 0 Then
                        pos = _bpiStart.Index
                        sel = -sel
                    Else
                        pos += _hexBox._iHexMaxHBytes
                        'sel -= _hexBox._iHexMaxHBytes;
                    End If

                    _hexBox.InternalSelect(pos, sel)
                    _hexBox.ScrollByteIntoView()
                End If

                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_ShiftLeft(ByRef m As Message) As Boolean
                Dim pos = _hexBox._bytePos
                Dim sel = _hexBox._selectionLength

                If pos + sel < 1 Then Return True

                If pos + sel <= _bpiStart.Index Then
                    If pos = 0 Then Return True

                    pos -= 1
                    sel += 1
                Else
                    sel = Math.Max(0, sel - 1)
                End If

                _hexBox.ScrollByteIntoView()
                _hexBox.InternalSelect(pos, sel)

                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_ShiftRight(ByRef m As Message) As Boolean
                Dim pos = _hexBox._bytePos
                Dim sel = _hexBox._selectionLength

                If pos + sel >= _hexBox._byteProvider.Length Then Return True

                If _bpiStart.Index <= pos Then
                    sel += 1
                    _hexBox.InternalSelect(pos, sel)
                    _hexBox.ScrollByteIntoView(pos + sel)
                Else
                    pos += 1
                    sel = Math.Max(0, sel - 1)
                    _hexBox.InternalSelect(pos, sel)
                    _hexBox.ScrollByteIntoView()
                End If

                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_ShiftShiftKey(ByRef m As Message) As Boolean
                If _mouseDown Then Return True
                If _shiftDown Then Return True

                _shiftDown = True

                If _hexBox._selectionLength > 0 Then Return True

                _bpiStart = New Forms.BytePositionInfo(_hexBox._bytePos, _hexBox._byteCharacterPos)

                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_ShiftTab(ByRef m As Message) As Boolean
                If TypeOf _hexBox._keyInterpreter Is StringKeyInterpreter Then
                    _shiftDown = False
                    _hexBox.ActivateKeyInterpreter()
                    _hexBox.ScrollByteIntoView()
                    _hexBox.ReleaseSelection()
                    _hexBox.UpdateCaret()
                    _hexBox.Invalidate()
                    Return True
                End If

                If _hexBox.Parent Is Nothing Then Return True
                _hexBox.Parent.SelectNextControl(_hexBox, False, True, True, True)
                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_ShiftUp(ByRef m As Message) As Boolean
                Dim pos = _hexBox._bytePos
                Dim sel = _hexBox._selectionLength

                If pos - _hexBox._iHexMaxHBytes < 0 AndAlso pos <= _bpiStart.Index Then Return True

                If _bpiStart.Index >= pos + sel Then
                    pos -= _hexBox._iHexMaxHBytes
                    sel += _hexBox._iHexMaxHBytes
                    _hexBox.InternalSelect(pos, sel)
                    _hexBox.ScrollByteIntoView()
                Else
                    sel -= _hexBox._iHexMaxHBytes
                    If sel < 0 Then
                        pos = _bpiStart.Index + sel
                        sel = -sel
                        _hexBox.InternalSelect(pos, sel)
                        _hexBox.ScrollByteIntoView()
                    Else
                        sel -= _hexBox._iHexMaxHBytes
                        _hexBox.InternalSelect(pos, sel)
                        _hexBox.ScrollByteIntoView(pos + sel)
                    End If
                End If

                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_Tab(ByRef m As Message) As Boolean
                If _hexBox._stringViewVisible AndAlso _hexBox._keyInterpreter.GetType() Is GetType(KeyInterpreter) Then
                    _hexBox.ActivateStringKeyInterpreter()
                    _hexBox.ScrollByteIntoView()
                    _hexBox.ReleaseSelection()
                    _hexBox.UpdateCaret()
                    _hexBox.Invalidate()
                    Return True
                End If

                If _hexBox.Parent Is Nothing Then Return True
                _hexBox.Parent.SelectNextControl(_hexBox, True, True, True, True)
                Return True
            End Function

            Protected Overridable Function PreProcessWmKeyDown_Up(ByRef m As Message) As Boolean
                Dim pos = _hexBox._bytePos
                Dim cp = _hexBox._byteCharacterPos

                If Not (pos = 0 AndAlso cp = 0) Then
                    pos = Math.Max(-1, pos - _hexBox._iHexMaxHBytes)
                    If pos = -1 Then Return True

                    _hexBox.SetPosition(pos)

                    If pos < _hexBox._startByte Then
                        _hexBox.PerformScrollLineUp()
                    End If

                    _hexBox.UpdateCaret()
                    _hexBox.Invalidate()
                End If

                _hexBox.ScrollByteIntoView()
                _hexBox.ReleaseSelection()

                Return True
            End Function

            Protected Function RaiseKeyDown(keyData As Keys) As Boolean
                Dim e As New KeyEventArgs(keyData)
                _hexBox.OnKeyDown(e)
                Return e.Handled
            End Function
#End Region

#Region "PreProcessWmChar methods"
            Public Overridable Function PreProcessWmChar(ByRef m As Message) As Boolean Implements IKeyInterpreter.PreProcessWmChar
                If ModifierKeys = Keys.Control Then
                    Return _hexBox.BasePreProcessMessage(m)
                End If

                Dim sw As Boolean = _hexBox._byteProvider.SupportsWriteByte()
                Dim si As Boolean = _hexBox._byteProvider.SupportsInsertBytes()
                Dim sd As Boolean = _hexBox._byteProvider.SupportsDeleteBytes()

                Dim pos = _hexBox._bytePos
                Dim sel = _hexBox._selectionLength
                Dim cp = _hexBox._byteCharacterPos

                If Not sw AndAlso pos <> _hexBox._byteProvider.Length OrElse Not si AndAlso pos = _hexBox._byteProvider.Length Then
                    Return _hexBox.BasePreProcessMessage(m)
                End If

                Dim c As Char = Microsoft.VisualBasic.ChrW(m.WParam.ToInt32())

                If Uri.IsHexDigit(c) Then
                    If RaiseKeyPress(c) Then Return True

                    If _hexBox.ReadOnly Then Return True

                    Dim isInsertMode As Boolean = pos = _hexBox._byteProvider.Length

                    ' do insert when insertActive = true
                    If Not isInsertMode AndAlso si AndAlso _hexBox.InsertActive AndAlso cp = 0 Then isInsertMode = True

                    If sd AndAlso si AndAlso sel > 0 Then
                        _hexBox._byteProvider.DeleteBytes(pos, sel)
                        isInsertMode = True
                        cp = 0
                        _hexBox.SetPosition(pos, cp)
                    End If

                    _hexBox.ReleaseSelection()

                    Dim currentByte As Byte
                    If isInsertMode Then
                        currentByte = 0
                    Else
                        currentByte = _hexBox._byteProvider.ReadByte(pos)
                    End If

                    Dim sCb = currentByte.ToString("X", Threading.Thread.CurrentThread.CurrentCulture)
                    If sCb.Length = 1 Then sCb = "0" & sCb

                    Dim sNewCb As String = c.ToString()
                    If cp = 0 Then
                        sNewCb += sCb.Substring(1, 1)
                    Else
                        sNewCb = sCb.Substring(0, 1) & sNewCb
                    End If
                    Dim newcb = Byte.Parse(sNewCb, Globalization.NumberStyles.AllowHexSpecifier, Threading.Thread.CurrentThread.CurrentCulture)

                    If isInsertMode Then
                        _hexBox._byteProvider.InsertBytes(pos, New Byte() {newcb})
                    Else
                        Dim prevByte As Byte = _hexBox._byteProvider.ReadByte(pos)
                        _hexBox._byteProvider.WriteByte(pos, newcb)
                        If prevByte <> newcb Then
                            _hexBox.OnByteChanged(New ByteChangedArgs(pos, prevByte, newcb))
                        End If

                    End If
                    PerformPosMoveRight()

                    _hexBox.Invalidate()
                    Return True
                Else
                    Return _hexBox.BasePreProcessMessage(m)
                End If
            End Function

            Protected Function RaiseKeyPress(keyChar As Char) As Boolean
                Dim e As New KeyPressEventArgs(keyChar)
                _hexBox.OnKeyPress(e)
                Return e.Handled
            End Function
#End Region

#Region "PreProcessWmKeyUp methods"
            Public Overridable Function PreProcessWmKeyUp(ByRef m As Message) As Boolean Implements IKeyInterpreter.PreProcessWmKeyUp
                Debug.WriteLine("PreProcessWmKeyUp(ref Message m)", "KeyInterpreter")

                Dim vc As Keys = CType(m.WParam.ToInt32(), Keys)

                Dim keyData = vc Or ModifierKeys

                Select Case keyData
                    Case Keys.ShiftKey, Keys.Insert
                        If RaiseKeyUp(keyData) Then Return True
                End Select

                Select Case keyData
                    Case Keys.ShiftKey
                        _shiftDown = False
                        Return True
                    Case Keys.Insert
                        Return PreProcessWmKeyUp_Insert(m)
                    Case Else
                        Return _hexBox.BasePreProcessMessage(m)
                End Select
            End Function

            Protected Overridable Function PreProcessWmKeyUp_Insert(ByRef m As Message) As Boolean
                _hexBox.InsertActive = Not _hexBox.InsertActive
                Return True
            End Function

            Protected Function RaiseKeyUp(keyData As Keys) As Boolean
                Dim e As New KeyEventArgs(keyData)
                _hexBox.OnKeyUp(e)
                Return e.Handled
            End Function
#End Region

#Region "Misc"
            Private ReadOnly Property MessageHandlers As Dictionary(Of Keys, MessageDelegate)
                Get
                    If _messageHandlers Is Nothing Then
                        _messageHandlers = New Dictionary(Of Keys, MessageDelegate) From {
                            {Keys.Left, New MessageDelegate(AddressOf PreProcessWmKeyDown_Left)}, ' move left
                            {Keys.Up, New MessageDelegate(AddressOf PreProcessWmKeyDown_Up)}, ' move up
                            {Keys.Right, New MessageDelegate(AddressOf PreProcessWmKeyDown_Right)}, ' move right
                            {Keys.Down, New MessageDelegate(AddressOf PreProcessWmKeyDown_Down)}, ' move down
                            {Keys.PageUp, New MessageDelegate(AddressOf PreProcessWmKeyDown_PageUp)}, ' move pageup
                            {Keys.PageDown, New MessageDelegate(AddressOf PreProcessWmKeyDown_PageDown)}, ' move page down
                            {Keys.Left Or Keys.Shift, New MessageDelegate(AddressOf PreProcessWmKeyDown_ShiftLeft)}, ' move left with selection
                            {Keys.Up Or Keys.Shift, New MessageDelegate(AddressOf PreProcessWmKeyDown_ShiftUp)}, ' move up with selection
                            {Keys.Right Or Keys.Shift, New MessageDelegate(AddressOf PreProcessWmKeyDown_ShiftRight)}, ' move right with selection
                            {Keys.Down Or Keys.Shift, New MessageDelegate(AddressOf PreProcessWmKeyDown_ShiftDown)}, ' move down with selection
                            {Keys.Tab, New MessageDelegate(AddressOf PreProcessWmKeyDown_Tab)}, ' switch to string view
                            {Keys.Back, New MessageDelegate(AddressOf PreProcessWmKeyDown_Back)}, ' back
                            {Keys.Delete, New MessageDelegate(AddressOf PreProcessWmKeyDown_Delete)}, ' delete
                            {Keys.Home, New MessageDelegate(AddressOf PreProcessWmKeyDown_Home)}, ' move to home
                            {Keys.End, New MessageDelegate(AddressOf PreProcessWmKeyDown_End)}, ' move to end
                            {Keys.ShiftKey Or Keys.Shift, New MessageDelegate(AddressOf PreProcessWmKeyDown_ShiftShiftKey)}, ' begin selection process
                            {Keys.C Or Keys.Control, New MessageDelegate(AddressOf PreProcessWmKeyDown_ControlC)}, ' copy 
                            {Keys.X Or Keys.Control, New MessageDelegate(AddressOf PreProcessWmKeyDown_ControlX)}, ' cut
                            {Keys.V Or Keys.Control, New MessageDelegate(AddressOf PreProcessWmKeyDown_ControlV)}, ' paste
                            {Keys.C Or Keys.Control Or Keys.Shift, New MessageDelegate(AddressOf PreProcessWmKeyDown_ControlShiftC)}, ' copy hex
                            {Keys.V Or Keys.Control Or Keys.Shift, New MessageDelegate(AddressOf PreProcessWmKeyDown_ControlShiftV)} ' paste hex
                            }
                    End If
                    Return _messageHandlers
                End Get
            End Property

            Public Overridable Function GetCaretPointF(byteIndex As Long) As PointF Implements IKeyInterpreter.GetCaretPointF
                Debug.WriteLine("GetCaretPointF()", "KeyInterpreter")

                Return _hexBox.GetBytePointF(byteIndex)
            End Function

            Protected Overridable Function GetBytePositionInfo(p As Point) As Forms.BytePositionInfo
                Return _hexBox.GetHexBytePositionInfo(p)
            End Function

            Protected Overridable Function PerformPosMoveLeft() As Boolean
                Dim pos = _hexBox._bytePos
                Dim sel = _hexBox._selectionLength
                Dim cp = _hexBox._byteCharacterPos

                If sel <> 0 Then
                    cp = 0
                    _hexBox.SetPosition(pos, cp)
                    _hexBox.ReleaseSelection()
                Else
                    If pos = 0 AndAlso cp = 0 Then Return True

                    If cp > 0 Then
                        cp -= 1
                    Else
                        pos = Math.Max(0, pos - 1)
                        cp += 1
                    End If

                    _hexBox.SetPosition(pos, cp)

                    If pos < _hexBox._startByte Then
                        _hexBox.PerformScrollLineUp()
                    End If
                    _hexBox.UpdateCaret()
                    _hexBox.Invalidate()
                End If

                _hexBox.ScrollByteIntoView()
                Return True
            End Function
            Protected Overridable Function PerformPosMoveLeftByte() As Boolean
                Dim pos = _hexBox._bytePos
                Dim cp As Integer

                If pos = 0 Then Return True

                pos = Math.Max(0, pos - 1)
                cp = 0

                _hexBox.SetPosition(pos, cp)

                If pos < _hexBox._startByte Then
                    _hexBox.PerformScrollLineUp()
                End If
                _hexBox.UpdateCaret()
                _hexBox.ScrollByteIntoView()
                _hexBox.Invalidate()

                Return True
            End Function

            Protected Overridable Function PerformPosMoveRight() As Boolean
                Dim pos = _hexBox._bytePos
                Dim cp = _hexBox._byteCharacterPos
                Dim sel = _hexBox._selectionLength

                If sel <> 0 Then
                    pos += sel
                    cp = 0
                    _hexBox.SetPosition(pos, cp)
                    _hexBox.ReleaseSelection()
                Else
                    If Not (pos = _hexBox._byteProvider.Length AndAlso cp = 0) Then

                        If cp > 0 Then
                            pos = Math.Min(_hexBox._byteProvider.Length, pos + 1)
                            cp = 0
                        Else
                            cp += 1
                        End If

                        _hexBox.SetPosition(pos, cp)

                        If pos > _hexBox._endByte - 1 Then
                            _hexBox.PerformScrollLineDown()
                        End If
                        _hexBox.UpdateCaret()
                        _hexBox.Invalidate()
                    End If
                End If

                _hexBox.ScrollByteIntoView()
                Return True
            End Function
            Protected Overridable Function PerformPosMoveRightByte() As Boolean
                Dim pos = _hexBox._bytePos
                Dim cp As Integer

                If pos = _hexBox._byteProvider.Length Then Return True

                pos = Math.Min(_hexBox._byteProvider.Length, pos + 1)
                cp = 0

                _hexBox.SetPosition(pos, cp)

                If pos > _hexBox._endByte - 1 Then
                    _hexBox.PerformScrollLineDown()
                End If
                _hexBox.UpdateCaret()
                _hexBox.ScrollByteIntoView()
                _hexBox.Invalidate()

                Return True
            End Function
            Private Sub ClearShiftDown() Implements IKeyInterpreter.ClearShiftDown
                _shiftDown = False
            End Sub

            Private Function IKeyInterpreter_PreProcessWmKeyDown(ByRef m As Message) As Boolean Implements IKeyInterpreter.PreProcessWmKeyDown
                Return PreProcessWmKeyDown(m)
            End Function
#End Region
        End Class
#End Region

#Region "StringKeyInterpreter class"
        ''' <summary>
        ''' Handles user input such as mouse and keyboard input during string view edit
        ''' </summary>
        Friend Class StringKeyInterpreter
            Inherits KeyInterpreter
#Region "Ctors"
            Public Sub New(hexBox As HexBox)
                MyBase.New(hexBox)
                _hexBox._byteCharacterPos = 0
            End Sub
#End Region

#Region "PreProcessWmKeyDown methods"
            Public Overrides Function PreProcessWmKeyDown(ByRef m As Message) As Boolean
                Dim vc As Keys = CType(m.WParam.ToInt32(), Keys)

                Dim keyData = vc Or ModifierKeys

                Select Case keyData
                    Case Keys.Tab Or Keys.Shift, Keys.Tab
                        If RaiseKeyDown(keyData) Then Return True
                End Select

                Select Case keyData
                    Case Keys.Tab Or Keys.Shift
                        Return MyBase.PreProcessWmKeyDown_ShiftTab(m)
                    Case Keys.Tab
                        Return MyBase.PreProcessWmKeyDown_Tab(m)
                    Case Else
                        Return MyBase.PreProcessWmKeyDown(m)
                End Select
            End Function

            Protected Overrides Function PreProcessWmKeyDown_Left(ByRef m As Message) As Boolean
                Return MyBase.PerformPosMoveLeftByte()
            End Function

            Protected Overrides Function PreProcessWmKeyDown_Right(ByRef m As Message) As Boolean
                Return MyBase.PerformPosMoveRightByte()
            End Function

#End Region

#Region "PreProcessWmChar methods"
            Public Overrides Function PreProcessWmChar(ByRef m As Message) As Boolean
                If ModifierKeys = Keys.Control Then
                    Return _hexBox.BasePreProcessMessage(m)
                End If

                Dim sw As Boolean = _hexBox._byteProvider.SupportsWriteByte()
                Dim si As Boolean = _hexBox._byteProvider.SupportsInsertBytes()
                Dim sd As Boolean = _hexBox._byteProvider.SupportsDeleteBytes()

                Dim pos = _hexBox._bytePos
                Dim sel = _hexBox._selectionLength
                Dim cp = _hexBox._byteCharacterPos

                If Not sw AndAlso pos <> _hexBox._byteProvider.Length OrElse Not si AndAlso pos = _hexBox._byteProvider.Length Then
                    Return _hexBox.BasePreProcessMessage(m)
                End If

                Dim c As Char = Microsoft.VisualBasic.ChrW(m.WParam.ToInt32())

                If RaiseKeyPress(c) Then Return True

                If _hexBox.ReadOnly Then Return True

                Dim isInsertMode As Boolean = pos = _hexBox._byteProvider.Length

                ' do insert when insertActive = true
                If Not isInsertMode AndAlso si AndAlso _hexBox.InsertActive Then isInsertMode = True

                If sd AndAlso si AndAlso sel > 0 Then
                    _hexBox._byteProvider.DeleteBytes(pos, sel)
                    isInsertMode = True
                    cp = 0
                    _hexBox.SetPosition(pos, cp)
                End If

                _hexBox.ReleaseSelection()

                Dim b As Byte = _hexBox.ByteCharConverter.ToByte(c)
                If isInsertMode Then
                    _hexBox._byteProvider.InsertBytes(pos, New Byte() {b})
                Else
                    Dim PrevByte As Byte = _hexBox._byteProvider.ReadByte(pos)
                    _hexBox._byteProvider.WriteByte(pos, b)
                    If PrevByte <> b Then
                        _hexBox.OnByteChanged(New ByteChangedArgs(pos, PrevByte, b))
                    End If
                End If

                MyBase.PerformPosMoveRightByte()
                _hexBox.Invalidate()

                Return True
            End Function
#End Region

#Region "Misc"
            Public Overrides Function GetCaretPointF(byteIndex As Long) As PointF
                Debug.WriteLine("GetCaretPointF()", "StringKeyInterpreter")

                Dim gp = _hexBox.GetGridBytePoint(byteIndex)
                Return _hexBox.GetByteStringPointF(gp)
            End Function

            Protected Overrides Function GetBytePositionInfo(p As Point) As Forms.BytePositionInfo
                Return _hexBox.GetStringBytePositionInfo(p)
            End Function
#End Region
        End Class
#End Region

#Region "Fields"
        ''' <summary>
        ''' Contains the thumptrack delay for scrolling in milliseconds.
        ''' </summary>
        Const THUMPTRACKDELAY As Integer = 50

        ''' <summary>
        ''' Hightlighted regions
        ''' </summary>
        Private ReadOnly _highlightRegions As New List(Of HighlightRegion)()

        ''' <summary>
        ''' Contains string format information for text drawing
        ''' </summary>
        Private ReadOnly _stringFormat As StringFormat

        ''' <summary>
        ''' Contains a timer for thumbtrack scrolling
        ''' </summary>
        Private ReadOnly _thumbTrackTimer As New Timer()

        ''' <summary>
        ''' Contains a vertical scroll
        ''' </summary>
        Private ReadOnly _vScrollBar As VScrollBar

        ''' <summary>
        ''' Contains true, if the find (Find method) should be aborted.
        ''' </summary>
        Private _abortFind As Boolean

        ''' <summary>
        ''' Contains the current char position in one byte
        ''' </summary>
        ''' <example>
        ''' "1A"
        ''' "1" = char position of 0
        ''' "A" = char position of 1
        ''' </example>
        Private _byteCharacterPos As Integer

        ''' <summary>
        ''' Contains the current byte position
        ''' </summary>
        Private _bytePos As Long = -1

        ''' <summary>
        ''' Contains True if caret is visible
        ''' </summary>
        Private _caretVisible As Boolean

        ''' <summary>
        ''' Contains an empty key interpreter without functionality
        ''' </summary>
        Private _eki As EmptyKeyInterpreter

        ''' <summary>
        ''' Contains the index of the last visible byte
        ''' </summary>
        Private _endByte As Long

        ''' <summary>
        ''' Contains a value of the current finding position.
        ''' </summary>
        Private _findingPos As Long

        ''' <summary>
        ''' Contains string format information for hex values
        ''' </summary>
        Private _hexStringFormat As String = "X"

        ''' <summary>
        ''' Contains the maximum of visible bytes.
        ''' </summary>
        Private _iHexMaxBytes As Integer

        ''' <summary>
        ''' Contains the maximum of visible horizontal bytes
        ''' </summary>
        Private _iHexMaxHBytes As Integer

        ''' <summary>
        ''' Contains the maximum of visible vertical bytes
        ''' </summary>
        Private _iHexMaxVBytes As Integer

        ''' <summary>
        ''' Contains a state value about Insert or Write mode. When this value is true and the ByteProvider SupportsInsert is true bytes are inserted instead of overridden.
        ''' </summary>
        Private _insertActive As Boolean

        ''' <summary>
        ''' Contains the current key interpreter
        ''' </summary>
        Private _keyInterpreter As IKeyInterpreter

        ''' <summary>
        ''' Contains the default key interpreter
        ''' </summary>
        Private _ki As KeyInterpreter

        ''' <summary>
        ''' Contains the Enviroment.TickCount of the last refresh
        ''' </summary>
        Private _lastThumbtrack As Integer = Environment.TickCount

        ''' <summary>
        ''' Contains the border bottom shift
        ''' </summary>
        Private _recBorderBottom As Integer = SystemInformation.Border3DSize.Height

        ''' <summary>
        ''' Contains the border´s left shift
        ''' </summary>
        Private _recBorderLeft As Integer = SystemInformation.Border3DSize.Width

        ''' <summary>
        ''' Contains the border´s right shift
        ''' </summary>
        Private _recBorderRight As Integer = SystemInformation.Border3DSize.Width

        ''' <summary>
        ''' Contains the border´s top shift
        ''' </summary>
        Private _recBorderTop As Integer = SystemInformation.Border3DSize.Height

        ''' <summary>
        ''' Contains the column info header rectangle bounds
        ''' </summary>
        Private _recColumnInfo As Rectangle

        ''' <summary>
        ''' Contains the hole content bounds of all text
        ''' </summary>
        Private _recContent As Rectangle

        ''' <summary>
        ''' Contains the hex data bounds
        ''' </summary>
        Private _recHex As Rectangle

        ''' <summary>
        ''' Contains the line info bounds
        ''' </summary>
        Private _recLineInfo As Rectangle

        ''' <summary>
        ''' Contains the string view bounds
        ''' </summary>
        Private _recStringView As Rectangle

        ''' <summary>
        ''' Contains the scroll bars maximum value
        ''' </summary>
        Private _scrollVmax As Long

        ''' <summary>
        ''' Contains the scroll bars minimum value
        ''' </summary>
        Private _scrollVmin As Long

        ''' <summary>
        ''' Contains the scroll bars current position
        ''' </summary>
        Private _scrollVpos As Long

        ''' <summary>
        ''' Seleced region
        ''' </summary>
        Private _selectedRegion As HighlightRegion

        ''' <summary>
        ''' Contains the string key interpreter
        ''' </summary>
        Private _ski As StringKeyInterpreter

        ''' <summary>
        ''' Contains the index of the first visible byte
        ''' </summary>
        Private _startByte As Long

        ''' <summary>
        ''' Contains the thumbtrack scrolling position
        ''' </summary>
        Private _thumbTrackPosition As Long

        Public ReadOnly Property EndByte As Long
            Get
                Return _endByte
            End Get
        End Property

        Public ReadOnly Property StartByte As Long
            Get
                Return _startByte
            End Get
        End Property

        ''' <summary>
        ''' Hightlight region entry
        ''' </summary>
        Public Class HighlightRegion
            Implements IEquatable(Of HighlightRegion)

            ''' <summary>
            ''' Hightlight region end index
            ''' </summary>
            Public Property [End] As Long

            ''' <summary>
            ''' Hightlight region background color
            ''' </summary>
            Public Property BackColor As Color

            ''' <summary>
            ''' Hightlight region foreground color
            ''' </summary>
            Public Property ForeColor As Color

            ''' <summary>
            ''' Hightlight region label
            ''' </summary>
            Public Property Label As String

            ''' <summary>
            ''' Hightlight region start index
            ''' </summary>
            Public Property Start As Long

            ''' <summary>
            ''' Checks two hightlight regions have same coordinates
            ''' </summary>
            Public Shadows Function Equals(other As HighlightRegion) As Boolean Implements IEquatable(Of HighlightRegion).Equals
                Return other.Start = Start AndAlso other.End = [End]
            End Function

            ''' <summary>
            ''' Checks hightlight region within position index
            ''' </summary>
            Public Function IsWithin(t As Long) As Boolean
                Return t >= Start AndAlso t <= [End]
            End Function

            ''' <summary>
            ''' Merge two hightlight regions
            ''' </summary>
            Public Sub Merge(other As HighlightRegion)
                Start = Math.Min(Start, other.Start)
                [End] = Math.Max([End], other.End)
            End Sub

            ''' <summary>
            ''' Checks if hightlight regions overlap
            ''' </summary>
            Public Function Overlaps(other As HighlightRegion) As Boolean
                Return IsWithin(other.Start) OrElse IsWithin(other.End) OrElse other.IsWithin(Start)
            End Function
        End Class
#End Region

#Region "Events"
        Public Event AfterPaint As AfterPaintEventHandler

        ''' <summary>
        ''' Occurs, when the value of BorderStyle property has changed.
        ''' </summary>
        <Description("Occurs, when the value of BorderStyle property has changed.")>
        Public Event BorderStyleChanged As EventHandler

        Public Event ByteChanged As ByteChangeEventHandler

        ''' <summary>
        ''' Occurs, when the value of ByteProvider property has changed.
        ''' </summary>
        <Description("Occurs, when the value of ByteProvider property has changed.")>
        Public Event ByteProviderChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of BytesPerLine property has changed.
        ''' </summary>
        <Description("Occurs, when the value of BytesPerLine property has changed.")>
        Public Event BytesPerLineChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the CharSize property has changed
        ''' </summary>
        <Description("Occurs, when the CharSize property has changed")>
        Public Event CharSizeChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of ColumnInfoVisibleChanged property has changed.
        ''' </summary>
        <Description("Occurs, when the value of ColumnInfoVisibleChanged property has changed.")>
        Public Event ColumnInfoVisibleChanged As EventHandler

        ''' <summary>
        ''' Occurs, when Copy method was invoked and ClipBoardData changed.
        ''' </summary>
        <Description("Occurs, when Copy method was invoked and ClipBoardData changed.")>
        Public Event Copied As EventHandler

        ''' <summary>
        ''' Occurs, when CopyHex method was invoked and ClipBoardData changed.
        ''' </summary>
        <Description("Occurs, when CopyHex method was invoked and ClipBoardData changed.")>
        Public Event CopiedHex As EventHandler

        ''' <summary>
        ''' Occurs, when the value of CurrentLine property has changed.
        ''' </summary>
        <Description("Occurs, when the value of CurrentLine property has changed.")>
        Public Event CurrentLineChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of CurrentPositionInLine property has changed.
        ''' </summary>
        <Description("Occurs, when the value of CurrentPositionInLine property has changed.")>
        Public Event CurrentPositionInLineChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of GroupSeparatorVisibleChanged property has changed.
        ''' </summary>
        <Description("Occurs, when the value of GroupSeparatorVisibleChanged property has changed.")>
        Public Event GroupSeparatorVisibleChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of ColumnWidth property has changed.
        ''' </summary>
        <Description("Occurs, when the value of GroupSize property has changed.")>
        Public Event GroupSizeChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of HexCasing property has changed.
        ''' </summary>
        <Description("Occurs, when the value of HexCasing property has changed.")>
        Public Event HexCasingChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the highlight region is added
        ''' </summary>
        <Description("Occurs, when the highlight region is added")>
        Public Event HighlightRegionAdded As EventHandler

        ''' <summary>
        ''' Occurs, when the highlight region is selected
        ''' </summary>
        <Description("Occurs, when LineInfoFormat property Changed")>
        Public Event HighlightRegionSelected As EventHandler

        ''' <summary>
        ''' Occurs, when the value of HorizontalByteCount property has changed.
        ''' </summary>
        <Description("Occurs, when the value of HorizontalByteCount property has changed.")>
        Public Event HorizontalByteCountChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of InsertActive property has changed.
        ''' </summary>
        <Description("Occurs, when the value of InsertActive property has changed.")>
        Public Event InsertActiveChanged As EventHandler
        ''' <summary>
        ''' Occurs,  when LineInfoFormat property Changed
        ''' </summary>
        <Description("Occurs, when LineInfoFormat property Changed")>
        Public Event LineInfoFormatChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of LineInfoVisible property has changed.
        ''' </summary>
        <Description("Occurs, when the value of LineInfoVisible property has changed.")>
        Public Event LineInfoVisibleChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of ReadOnly property has changed.
        ''' </summary>
        <Description("Occurs, when the value of ReadOnly property has changed.")>
        Public Event ReadOnlyChanged As EventHandler
        ''' <summary>
        ''' Occurs, when the RequiredWidth property changes
        ''' </summary>
        <Description("Occurs, when the RequiredWidth property changes")>
        Public Event RequiredWidthChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of SelectionLength property has changed.
        ''' </summary>
        <Description("Occurs, when the value of SelectionLength property has changed.")>
        Public Event SelectionLengthChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of SelectionStart property has changed.
        ''' </summary>
        <Description("Occurs, when the value of SelectionStart property has changed.")>
        Public Event SelectionStartChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of StringViewVisible property has changed.
        ''' </summary>
        <Description("Occurs, when the value of StringViewVisible property has changed.")>
        Public Event StringViewVisibleChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of UseFixedBytesPerLine property has changed.
        ''' </summary>
        <Description("Occurs, when the value of UseFixedBytesPerLine property has changed.")>
        Public Event UseFixedBytesPerLineChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of VerticalByteCount property has changed.
        ''' </summary>
        <Description("Occurs, when the value of VerticalByteCount property has changed.")>
        Public Event VerticalByteCountChanged As EventHandler

        Public Event VisibilityBytesChanged As EventHandler

        ''' <summary>
        ''' Occurs, when the value of VScrollBarVisible property has changed.
        ''' </summary>
        <Description("Occurs, when the value of VScrollBarVisible property has changed.")>
        Public Event VScrollBarVisibleChanged As EventHandler

        Public Delegate Sub AfterPaintEventHandler(source As Object, e As PaintEventArgs)

        Public Delegate Sub ByteChangeEventHandler(source As Object, e As ByteChangedArgs)

        Public Class ByteChangedArgs
            Inherits EventArgs
            Public Sub New(Index As Long, PrevValue As Byte, Value As Byte)
                Me.Index = Index
                Me.PrevValue = PrevValue
                Me.Value = Value
            End Sub
            Public Property Index As Long
            Public Property PrevValue As Byte
            Public Property Value As Byte
        End Class
#End Region

#Region "Ctors"

        ''' <summary>
        ''' Initializes a new instance of a HexBox class.
        ''' </summary>
        Public Sub New()
            _vScrollBar = New VScrollBar()
            AddHandler _vScrollBar.Scroll, New ScrollEventHandler(AddressOf _vScrollBar_Scroll)

            _builtInContextMenu = New Forms.BuiltInContextMenu(Me)

            BackColor = Color.White
            Font = SystemFonts.MessageBoxFont
            _stringFormat = New StringFormat(StringFormat.GenericTypographic) With {
                .FormatFlags = StringFormatFlags.MeasureTrailingSpaces
            }

            ActivateEmptyKeyInterpreter()

            SetStyle(ControlStyles.UserPaint, True)
            SetStyle(ControlStyles.DoubleBuffer, True)
            SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            SetStyle(ControlStyles.ResizeRedraw, True)

            _thumbTrackTimer.Interval = 50
            AddHandler _thumbTrackTimer.Tick, New EventHandler(AddressOf PerformScrollThumbTrack)
        End Sub

#End Region

#Region "Scroll methods"
        Public Sub PerformScrollToLine(pos As Long)
            If pos < _scrollVmin OrElse pos > _scrollVmax OrElse pos = _scrollVpos Then Return

            _scrollVpos = pos

            UpdateVScroll()
            UpdateVisibilityBytes()
            UpdateCaret()
            Invalidate()
        End Sub

        ''' <summary>
        ''' Scrolls the selection start byte into view
        ''' </summary>
        Public Sub ScrollByteIntoView()
            Debug.WriteLine("ScrollByteIntoView()", "HexBox")

            ScrollByteIntoView(_bytePos)
        End Sub

        ''' <summary>
        ''' Scrolls the specific byte into view
        ''' </summary>
        ''' <paramname="index">the index of the byte</param>
        Public Sub ScrollByteIntoView(index As Long)
            Debug.WriteLine("ScrollByteIntoView(long index)", "HexBox")

            If _byteProvider Is Nothing OrElse _keyInterpreter Is Nothing Then Return

            If index < _startByte Then
                Dim line As Long = Math.Floor(index / _iHexMaxHBytes)
                PerformScrollThumpPosition(line)
            ElseIf index > _endByte Then
                Dim line As Long = Math.Floor(index / _iHexMaxHBytes)
                line -= _iHexMaxVBytes - 1
                PerformScrollThumpPosition(line)
            End If
        End Sub

        <CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification:="<Pending>")>
        Private Sub _vScrollBar_Scroll(sender As Object, e As ScrollEventArgs)
            Select Case e.Type
                Case ScrollEventType.Last
                Case ScrollEventType.EndScroll
                Case ScrollEventType.SmallIncrement
                    PerformScrollLineDown()
                Case ScrollEventType.SmallDecrement
                    PerformScrollLineUp()
                Case ScrollEventType.LargeIncrement
                    PerformScrollPageDown()
                Case ScrollEventType.LargeDecrement
                    PerformScrollPageUp()
                Case ScrollEventType.ThumbPosition
                    Dim lPos = FromScrollPos(e.NewValue)
                    PerformScrollThumpPosition(lPos)
                Case ScrollEventType.ThumbTrack
                    ' to avoid performance problems use a refresh delay implemented with a timer
                    If _thumbTrackTimer.Enabled Then _thumbTrackTimer.Enabled = False ' stop old timer

                    ' perform scroll immediately only if last refresh is very old
                    Dim currentThumbTrack = Environment.TickCount
                    If currentThumbTrack - _lastThumbtrack > THUMPTRACKDELAY Then
                        PerformScrollThumbTrack(Nothing, Nothing)
                        _lastThumbtrack = currentThumbTrack
                        Exit Select
                    End If

                    ' start thumbtrack timer 
                    _thumbTrackPosition = FromScrollPos(e.NewValue)
                    _thumbTrackTimer.Enabled = True
                Case ScrollEventType.First
                Case Else
            End Select

            e.NewValue = ToScrollPos(_scrollVpos)
        End Sub

        Private Function FromScrollPos(value As Integer) As Long
            Dim max = 65535
            If _scrollVmax < max Then
                Return value
            Else
                Dim valperc = value / max * 100
                Dim res As Long = CInt(Math.Floor(_scrollVmax / 100 * valperc))
                Return res
            End If
        End Function

        Private Sub PerformScrollLineDown()
            PerformScrollLines(1)
        End Sub

        Private Sub PerformScrollLines(lines As Integer)
            Dim pos As Long
            If lines > 0 Then
                pos = Math.Min(_scrollVmax, _scrollVpos + lines)
            ElseIf lines < 0 Then
                pos = Math.Max(_scrollVmin, _scrollVpos + lines)
            Else
                Return
            End If

            PerformScrollToLine(pos)
        End Sub

        Private Sub PerformScrollLineUp()
            PerformScrollLines(-1)
        End Sub

        Private Sub PerformScrollPageDown()
            PerformScrollLines(_iHexMaxVBytes)
        End Sub

        Private Sub PerformScrollPageUp()
            PerformScrollLines(-_iHexMaxVBytes)
        End Sub

        ''' <summary>
        ''' Performs the thumbtrack scrolling after an delay.
        ''' </summary>
        Private Sub PerformScrollThumbTrack(sender As Object, e As EventArgs)
            _thumbTrackTimer.Enabled = False
            PerformScrollThumpPosition(_thumbTrackPosition)
            _lastThumbtrack = Environment.TickCount
        End Sub

        Private Sub PerformScrollThumpPosition(pos As Long)
            ' Bug fix: Scroll to end, do not scroll to end
            Dim difference = If(_scrollVmax > 65535, 10, 9)

            If ToScrollPos(pos) = ToScrollMax(_scrollVmax) - difference Then pos = _scrollVmax
            ' End Bug fix


            PerformScrollToLine(pos)
        End Sub

        Private Function ToScrollMax(value As Long) As Integer
            Dim max As Long = 65535
            If value > max Then
                Return max
            Else
                Return value
            End If
        End Function

        Private Function ToScrollPos(value As Long) As Integer
            Dim max = 65535

            If _scrollVmax < max Then
                Return value
            Else
                Dim valperc = value / _scrollVmax * 100
                Dim res As Integer = Math.Floor(max / 100 * valperc)
                res = CInt(Math.Max(_scrollVmin, res))
                res = CInt(Math.Min(_scrollVmax, res))
                Return res
            End If
        End Function

        Private Sub UpdateScrollSize()
            Debug.WriteLine("UpdateScrollSize()", "HexBox")

            ' calc scroll bar info
            If VScrollBarVisible AndAlso _byteProvider IsNot Nothing AndAlso _byteProvider.Length > 0 AndAlso _iHexMaxHBytes <> 0 Then
                Dim scrollmax = CLng(Math.Ceiling(CDbl(_byteProvider.Length + 1) / CDbl(_iHexMaxHBytes) - CDbl(_iHexMaxVBytes)))
                scrollmax = Math.Max(0, scrollmax)

                Dim scrollpos As Long = _startByte / _iHexMaxHBytes

                If scrollmax < _scrollVmax Then
                    ' Data size has been decreased. 
                    ' Scroll one line up if we at bottom. 
                    If _scrollVpos = _scrollVmax Then PerformScrollLineUp()
                End If

                If scrollmax = _scrollVmax AndAlso scrollpos = _scrollVpos Then Return

                _scrollVmin = 0
                _scrollVmax = scrollmax
                _scrollVpos = Math.Min(scrollpos, scrollmax)
                UpdateVScroll()
            ElseIf VScrollBarVisible Then
                ' disable scroll bar
                _scrollVmin = 0
                _scrollVmax = 0
                _scrollVpos = 0
                UpdateVScroll()
            End If
        End Sub

        Private Sub UpdateVScroll()
            Debug.WriteLine("UpdateVScroll()", "HexBox")

            Dim max = ToScrollMax(_scrollVmax)

            If max > 0 Then
                _vScrollBar.Minimum = 0
                _vScrollBar.Maximum = max
                _vScrollBar.Value = ToScrollPos(_scrollVpos)
                _vScrollBar.Visible = True
                _thumbTrackPosition = FromScrollPos(_vScrollBar.Value)
            Else
                _vScrollBar.Visible = False
            End If
        End Sub
#End Region

#Region "Selection methods"
        ''' <summary>
        ''' Selects the hex box.
        ''' </summary>
        ''' <paramname="start">the start index of the selection</param>
        ''' <paramname="length">the length of the selection</param>
        Public Shadows Sub [Select](start As Long, length As Long)
            If ByteProvider Is Nothing Then Return
            If Not Enabled Then Return

            InternalSelect(start, length)
            ScrollByteIntoView()
        End Sub

        ''' <summary>
        ''' Returns true if Select method could be invoked.
        ''' </summary>
        Public Function CanSelectAll() As Boolean
            If Not Enabled Then Return False
            If _byteProvider Is Nothing Then Return False

            Return True
        End Function

        ''' <summary>
        ''' Selects all bytes.
        ''' </summary>
        Public Sub SelectAll()
            If ByteProvider Is Nothing Then Return
            Me.Select(0, ByteProvider.Length)
        End Sub

        Private Sub InternalSelect(start As Long, length As Long)
            Dim pos = start
            Dim sel = length
            Dim cp = 0

            If sel > 0 AndAlso _caretVisible Then
                DestroyCaret()
            ElseIf sel = 0 AndAlso Not _caretVisible Then
                CreateCaret()
            End If

            SetPosition(pos, cp)
            SetSelectionLength(sel)

            UpdateCaret()
            Invalidate()
        End Sub

        Private Sub ReleaseSelection()
            Debug.WriteLine("ReleaseSelection()", "HexBox")

            If _selectionLength = 0 Then Return
            _selectionLength = 0
            OnSelectionLengthChanged(EventArgs.Empty)

            If Not _caretVisible Then
                CreateCaret()
            Else
                UpdateCaret()
            End If

            Invalidate()
        End Sub
#End Region

#Region "Key interpreter methods"
        Private Sub ActivateEmptyKeyInterpreter()
            If _eki Is Nothing Then _eki = New EmptyKeyInterpreter(Me)

            If _eki Is _keyInterpreter Then Return

            If _keyInterpreter IsNot Nothing Then _keyInterpreter.Deactivate()

            _keyInterpreter = _eki
            _keyInterpreter.Activate()
        End Sub

        Private Sub ActivateKeyInterpreter()
            If _ki Is Nothing Then _ki = New KeyInterpreter(Me)

            If _ki Is _keyInterpreter Then Return

            If _keyInterpreter IsNot Nothing Then _keyInterpreter.Deactivate()

            _keyInterpreter = _ki
            _keyInterpreter.Activate()
        End Sub

        Private Sub ActivateStringKeyInterpreter()
            If _ski Is Nothing Then _ski = New StringKeyInterpreter(Me)

            If _ski Is _keyInterpreter Then Return

            If _keyInterpreter IsNot Nothing Then _keyInterpreter.Deactivate()

            _keyInterpreter = _ski
            _keyInterpreter.Activate()
        End Sub
#End Region

#Region "Caret methods"
        Public Sub SetCaretPosition(p As Point)
            Debug.WriteLine("SetCaretPosition()", "HexBox")

            If _byteProvider Is Nothing OrElse _keyInterpreter Is Nothing Then Return

            Dim pos As Long
            Dim cp As Integer

            If _recHex.Contains(p) Then
                Dim bpi As Forms.BytePositionInfo = GetHexBytePositionInfo(p)
                pos = bpi.Index
                cp = bpi.CharacterPosition

                SetPosition(pos, cp)

                ActivateKeyInterpreter()
                UpdateCaret()
                Invalidate()
            ElseIf _recStringView.Contains(p) Then
                Dim bpi As Forms.BytePositionInfo = GetStringBytePositionInfo(p)
                pos = bpi.Index
                cp = bpi.CharacterPosition

                SetPosition(pos, cp)

                ActivateStringKeyInterpreter()
                UpdateCaret()
                Invalidate()
            End If
        End Sub

        Private Sub CreateCaret()
            If _byteProvider Is Nothing OrElse _keyInterpreter Is Nothing OrElse _caretVisible OrElse Not Focused Then Return

            Debug.WriteLine("CreateCaret()", "HexBox")

            ' define the caret width depending on InsertActive mode
            Dim caretWidth = If(InsertActive, 1, CInt(_charSize.Width))
            Dim caretHeight As Integer = _charSize.Height
            Forms.NativeMethods.CreateCaret(Handle, IntPtr.Zero, caretWidth, caretHeight)

            UpdateCaret()

            Forms.NativeMethods.ShowCaret(Handle)

            _caretVisible = True
        End Sub

        Private Sub DestroyCaret()
            If Not _caretVisible Then Return

            Debug.WriteLine("DestroyCaret()", "HexBox")

            Call Forms.NativeMethods.DestroyCaret()
            _caretVisible = False
        End Sub

        Private Function GetHexBytePositionInfo(p As Point) As Forms.BytePositionInfo
            Debug.WriteLine("GetHexBytePositionInfo()", "HexBox")

            Dim bytePos As Long
            Dim byteCharaterPos As Integer

            Dim x = (p.X - _recHex.X) / _charSize.Width
            Dim y = (p.Y - _recHex.Y) / _charSize.Height
            Dim iX As Integer = x
            Dim iY As Integer = y

            Dim hPos As Integer = iX / 3 + 1

            bytePos = Math.Min(_byteProvider.Length, _startByte + (_iHexMaxHBytes * (iY + 1) - _iHexMaxHBytes) + hPos - 1)
            byteCharaterPos = iX Mod 3
            If byteCharaterPos > 1 Then byteCharaterPos = 1

            If bytePos = _byteProvider.Length Then byteCharaterPos = 0

            If bytePos < 0 Then Return New Forms.BytePositionInfo(0, 0)
            Return New Forms.BytePositionInfo(bytePos, byteCharaterPos)
        End Function

        Private Function GetStringBytePositionInfo(p As Point) As Forms.BytePositionInfo
            Debug.WriteLine("GetStringBytePositionInfo()", "HexBox")

            Dim bytePos As Long
            Dim byteCharacterPos As Integer

            Dim x = (p.X - _recStringView.X) / _charSize.Width
            Dim y = (p.Y - _recStringView.Y) / _charSize.Height
            Dim iX As Integer = x
            Dim iY As Integer = y

            Dim hPos = iX + 1

            bytePos = Math.Min(_byteProvider.Length, _startByte + (_iHexMaxHBytes * (iY + 1) - _iHexMaxHBytes) + hPos - 1)
            byteCharacterPos = 0

            If bytePos < 0 Then Return New Forms.BytePositionInfo(0, 0)
            Return New Forms.BytePositionInfo(bytePos, byteCharacterPos)
        End Function

        Private Sub UpdateCaret()
            If _byteProvider Is Nothing OrElse _keyInterpreter Is Nothing Then Return

            Debug.WriteLine("UpdateCaret()", "HexBox")

            Dim byteIndex = _bytePos - _startByte
            Dim p = _keyInterpreter.GetCaretPointF(byteIndex)
            p.X += _byteCharacterPos * _charSize.Width
            Forms.NativeMethods.SetCaretPos(p.X, p.Y)
        End Sub
#End Region

#Region "PreProcessMessage methods"
        ''' <summary>
        ''' Preprocesses windows messages.
        ''' </summary>
        ''' <paramname="m">the message to process.</param>
        ''' <returns>true, if the message was processed</returns>
        <SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode:=True), SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode:=True)>
        Public Overrides Function PreProcessMessage(ByRef m As Message) As Boolean
            Select Case m.Msg
                Case Forms.NativeMethods.WM_KEYDOWN
                    Return _keyInterpreter.PreProcessWmKeyDown(m)
                Case Forms.NativeMethods.WM_CHAR
                    Return _keyInterpreter.PreProcessWmChar(m)
                Case Forms.NativeMethods.WM_KEYUP
                    Return _keyInterpreter.PreProcessWmKeyUp(m)
                Case Else
                    Return MyBase.PreProcessMessage(m)
            End Select
        End Function

        Private Function BasePreProcessMessage(ByRef m As Message) As Boolean
            Return MyBase.PreProcessMessage(m)
        End Function
#End Region

#Region "Find methods"
        ''' <summary>
        ''' Gets a value that indicates the current position during Find method execution.
        ''' </summary>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property CurrentFindingPosition As Long
            Get
                Return _findingPos
            End Get
        End Property

        ''' <summary>
        ''' Aborts a working Find method.
        ''' </summary>
        Public Sub AbortFind()
            _abortFind = True
        End Sub

        ''' <summary>
        ''' Searches the current ByteProvider
        ''' </summary>
        ''' <paramname="options">contains all find options</param>
        ''' <returns>the SelectionStart property value if find was successfull or
        ''' -1 if there is no match
        ''' -2 if Find was aborted.</returns>
        Public Function Find(options As Forms.FindOptions) As Long
            Dim startIndex = SelectionStart + SelectionLength
            Dim match = 0

            Dim buffer1 As Byte() = Nothing
            Dim buffer2 As Byte() = Nothing
            If options.Type = Forms.FindType.Text AndAlso options.MatchCase Then
                If options.FindBuffer Is Nothing OrElse options.FindBuffer.Length = 0 Then Throw New ArgumentException("FindBuffer can not be null when Type: Text and MatchCase: false")
                buffer1 = options.FindBuffer
            ElseIf options.Type = Forms.FindType.Text AndAlso Not options.MatchCase Then
                If options.FindBufferLowerCase Is Nothing OrElse options.FindBufferLowerCase.Length = 0 Then Throw New ArgumentException("FindBufferLowerCase can not be null when Type is Text and MatchCase is true")
                If options.FindBufferUpperCase Is Nothing OrElse options.FindBufferUpperCase.Length = 0 Then Throw New ArgumentException("FindBufferUpperCase can not be null when Type is Text and MatchCase is true")
                If options.FindBufferLowerCase.Length <> options.FindBufferUpperCase.Length Then Throw New ArgumentException("FindBufferUpperCase and FindBufferUpperCase must have the same size when Type is Text and MatchCase is true")
                buffer1 = options.FindBufferLowerCase
                buffer2 = options.FindBufferUpperCase
            ElseIf options.Type = Forms.FindType.Hex Then
                If options.Hex Is Nothing OrElse options.Hex.Length = 0 Then Throw New ArgumentException("Hex can not be null when Type is Hex")
                buffer1 = options.Hex
            End If

            Dim buffer1Length = buffer1.Length

            _abortFind = False

            For pos As Long = startIndex To _byteProvider.Length - 1
                If _abortFind Then Return -2

                If pos Mod 1000 = 0 Then Call Application.DoEvents() ' for performance reasons: DoEvents only 1 times per 1000 loops

                Dim compareByte As Byte = _byteProvider.ReadByte(pos)
                Dim buffer1Match = compareByte = buffer1(match)
                Dim hasBuffer2 = buffer2 IsNot Nothing
                Dim buffer2Match = hasBuffer2 AndAlso compareByte = buffer2(match)
                Dim isMatch = buffer1Match OrElse buffer2Match
                If Not isMatch Then
                    pos -= match
                    match = 0
                    _findingPos = pos
                    Continue For
                End If

                match += 1

                If match = buffer1Length Then
                    Dim bytePos = pos - buffer1Length + 1
                    [Select](bytePos, buffer1Length)
                    ScrollByteIntoView(_bytePos + _selectionLength)
                    ScrollByteIntoView(_bytePos)

                    Return bytePos
                End If
            Next

            Return -1
        End Function
#End Region

#Region "Copy, Cut, Paste, Insert and Fill methods"
        Private sourc As Byte()

        ''' <summary>
        ''' Return true if Copy method could be invoked.
        ''' </summary>
        Public Function CanCopy() As Boolean
            If _selectionLength < 1 OrElse _byteProvider Is Nothing Then Return False

            Return True
        End Function

        ''' <summary>
        ''' Return true if Cut method could be invoked.
        ''' </summary>
        Public Function CanCut() As Boolean
            If [ReadOnly] OrElse Not Enabled Then Return False
            If _byteProvider Is Nothing Then Return False
            If _selectionLength < 1 OrElse Not _byteProvider.SupportsDeleteBytes() Then Return False

            Return True
        End Function

        ''' <summary>
        ''' Return true if Insert method could be invoked.
        ''' </summary>
        Public Function CanInsert() As Boolean
            If [ReadOnly] OrElse Not Enabled Then Return False

            If _byteProvider Is Nothing OrElse Not _byteProvider.SupportsInsertBytes() Then Return False

            If Not _byteProvider.SupportsDeleteBytes() AndAlso _selectionLength > 0 Then Return False

            Return True
        End Function

        ''' <summary>
        ''' Return true if Paste method could be invoked.
        ''' </summary>
        Public Function CanPaste() As Boolean
            If Not CanInsert() Then Return False

            Dim da As IDataObject = Clipboard.GetDataObject()
            If da.GetDataPresent("BinaryData") Then
                Return True
            ElseIf da.GetDataPresent(GetType(String)) Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Return true if PasteHex method could be invoked.
        ''' </summary>
        Public Function CanPasteHex() As Boolean
            If Not CanPaste() Then Return False

            Dim buffer As Byte()
            Dim da As IDataObject = Clipboard.GetDataObject()
            If da.GetDataPresent(GetType(String)) Then
                Dim hexString = CStr(da.GetData(GetType(String)))
                buffer = ConvertHexToBytes(hexString)
                Return buffer IsNot Nothing
            End If
            Return False
        End Function

        ''' <summary>
        ''' Copies the current selection in the hex box to the Clipboard.
        ''' </summary>
        Public Sub Copy()
            If Not CanCopy() Then Return

            ' put bytes into buffer
            Dim buffer As Byte() = GetCopyData()

            Dim da As New DataObject()

            ' set string buffer clipbard data
            Dim sBuffer = Encoding.ASCII.GetString(buffer, 0, buffer.Length)
            da.SetData(GetType(String), sBuffer)

            'set memorystream (BinaryData) clipboard data
            Dim ms As New MemoryStream(buffer, 0, buffer.Length, False, True)
            da.SetData("BinaryData", ms)

            Clipboard.SetDataObject(da, True)
            UpdateCaret()
            ScrollByteIntoView()
            Invalidate()

            OnCopied(EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Copies the current selection in the hex box to the Clipboard in hex format.
        ''' </summary>
        Public Sub CopyHex()
            If Not CanCopy() Then Return

            ' put bytes into buffer
            Dim buffer As Byte() = GetCopyData()

            Dim da As New DataObject()

            ' set string buffer clipbard data
            Dim hexString = ConvertBytesToHex(buffer)
            da.SetData(GetType(String), hexString)

            'set memorystream (BinaryData) clipboard data
            Dim ms As New MemoryStream(buffer, 0, buffer.Length, False, True)
            da.SetData("BinaryData", ms)

            Clipboard.SetDataObject(da, True)
            UpdateCaret()
            ScrollByteIntoView()
            Invalidate()

            OnCopiedHex(EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Moves the current selection in the hex box to the Clipboard.
        ''' </summary>
        Public Sub Cut()
            If Not CanCut() Then Return

            Copy()

            _byteProvider.DeleteBytes(_bytePos, _selectionLength)
            _byteCharacterPos = 0
            UpdateCaret()
            ScrollByteIntoView()
            ReleaseSelection()
            Invalidate()
            MyBase.Refresh()
        End Sub

        ''' <summary>
        ''' Fills bytes current position with given count. It can in hex string or text according to mod
        ''' if (mod=0) fills as hex string if (mod=1) fills as text.
        ''' </summary>
        ''' <paramname="fill"></param>
        ''' <paramname="count"></param>
        ''' <paramname="mod"></param>
        Public Sub FillBytes(fill As String, count As Long, [mod] As Integer)
            _selectionLength = count
            InsertBytes(fill, count, [mod])
        End Sub

        ''' <summary>
        ''' Inserts bytes current position with given count. It can be in hex string or text according to mod
        ''' if mod=0 inserts as hex string if mod=1 inserts as text.
        ''' </summary>
        ''' <paramname="fill"></param>
        ''' <paramname="count"></param>
        ''' <paramname="mod"></param>
        Public Sub InsertBytes(fill As String, count As Long, [mod] As Integer)
            If Not CanInsert() Then Return

            If [mod] = 0 Then sourc = ConvertHexToBytes(fill)
            If [mod] = 1 Then sourc = Encoding.ASCII.GetBytes(fill)
            If sourc Is Nothing Then
                MessageBox.Show("Bad filling format")
                Return
            End If

            If _selectionLength > 0 Then _byteProvider.DeleteBytes(_bytePos, _selectionLength)
            Dim coefficient As Long = count / sourc.LongLength
            Dim remains = count - coefficient * sourc.LongLength
            Dim buffer = New Byte(count - 1) {}
            For i = 0 To coefficient - 1
                Array.Copy(sourc, 0, buffer, i * sourc.LongLength, sourc.LongLength)
            Next
            'Copy remaining bytes
            Array.Copy(sourc, 0, buffer, coefficient * sourc.LongLength, remains)

            _byteProvider.InsertBytes(_bytePos, buffer)

            SetPosition(_bytePos + buffer.Length, 0)

            ReleaseSelection()
            ScrollByteIntoView()
            UpdateCaret()
            Invalidate()
        End Sub

        ''' <summary>
        ''' Replaces the current selection in the hex box with the contents of the Clipboard.
        ''' </summary>
        Public Sub Paste()
            If Not CanPaste() Then Return

            If _selectionLength > 0 Then _byteProvider.DeleteBytes(_bytePos, _selectionLength)

            Dim buffer As Byte()
            Dim da As IDataObject = Clipboard.GetDataObject()
            If da.GetDataPresent("BinaryData") Then
                Dim ms = CType(da.GetData("BinaryData"), MemoryStream)
                buffer = New Byte(ms.Length - 1) {}
                ms.Read(buffer, 0, buffer.Length)
            ElseIf da.GetDataPresent(GetType(String)) Then
                Dim sBuffer = CStr(da.GetData(GetType(String)))
                buffer = Encoding.ASCII.GetBytes(sBuffer)
            Else
                Return
            End If

            _byteProvider.InsertBytes(_bytePos, buffer)

            SetPosition(_bytePos + buffer.Length, 0)

            ReleaseSelection()
            ScrollByteIntoView()
            UpdateCaret()
            Invalidate()
        End Sub

        ''' <summary>
        ''' Replaces the current selection in the hex box with the hex string data of the Clipboard.
        ''' </summary>
        Public Sub PasteHex()
            If Not CanPaste() Then Return

            Dim buffer As Byte()
            Dim da As IDataObject = Clipboard.GetDataObject()
            If da.GetDataPresent(GetType(String)) Then
                Dim hexString = CStr(da.GetData(GetType(String)))
                buffer = ConvertHexToBytes(hexString)
                If buffer Is Nothing Then Return
            Else
                Return
            End If

            If _selectionLength > 0 Then _byteProvider.DeleteBytes(_bytePos, _selectionLength)

            _byteProvider.InsertBytes(_bytePos, buffer)

            SetPosition(_bytePos + buffer.Length, 0)

            ReleaseSelection()
            ScrollByteIntoView()
            UpdateCaret()
            Invalidate()
        End Sub

        Private Function GetCopyData() As Byte()
            If Not CanCopy() Then Return New Byte(-1) {}

            ' put bytes into buffer
            Dim buffer = New Byte(_selectionLength - 1) {}
            Dim id = -1
            For i = _bytePos To _bytePos + _selectionLength - 1
                id += 1

                buffer(id) = _byteProvider.ReadByte(i)
            Next
            Return buffer
        End Function
#End Region

#Region "Highlight methods"
        ''' <summary>
        ''' Highlights bytes
        ''' </summary>
        ''' <paramname="start">the start index of the selection</param>
        ''' <paramname="size">the length of the selection</param>
        ''' <paramname="fColor">Fore color</param>
        ''' <paramname="bColor">Back color</param>
        ''' <paramname="label">Region label</param>
        Public Sub Highlight(start As Long, size As Long, fColor As Color, bColor As Color, Optional label As String = Nothing)
            ' Create new region
            Dim region As New HighlightRegion() With {
                .Start = start,
                .End = start + size - 1,
                .ForeColor = fColor,
                .BackColor = bColor,
                .Label = label
            }

            ' Only add if not already present
            If Not _highlightRegions.Contains(region) Then
                Dim found As HighlightRegion = Nothing

                ' Merge any overlapping regions
                Do
                    found = _highlightRegions.Find(Function(item) item.Overlaps(region))
                    If found IsNot Nothing Then
                        region.Merge(found)
                        _highlightRegions.Remove(found)
                    End If
                Loop While found IsNot Nothing

                ' Add the (possibly merged) region
                _highlightRegions.Add(region)

                ' Raise event and refresh
                OnHighlightRegionAdded(EventArgs.Empty)
                Invalidate()
            End If
        End Sub


        ''' <summary>
        ''' Highlights bytes
        ''' </summary>
        ''' <paramname="start">the start index of the selection</param>
        ''' <paramname="size">the length of the selection</param>
        Public Sub Highlight(start As Long, size As Long)
            Highlight(start, size, HighlightForeColor, HighlightBackColor)
        End Sub

        ''' <summary>
        ''' Highlights selected bytes
        ''' </summary>
        Public Sub HighlightSelected()
            If SelectionLength > 0 Then
                Highlight(SelectionStart, SelectionLength)
            End If
        End Sub

        ''' <summary>
        ''' Remove highlighted region by position
        ''' </summary>
        ''' /// <paramname="pos">highlight position</param>
        Public Sub Unhighlight(pos As Long)
            Dim found = _highlightRegions.Find(Function(item) item.IsWithin(pos))
            If found IsNot Nothing Then
                _highlightRegions.Remove(found)
                Invalidate()
            End If
        End Sub

        ''' <summary>
        ''' Remove highlighted region by the caret position
        ''' </summary>
        Public Sub Unhighlight()
            Unhighlight(_bytePos)
        End Sub
#End Region

#Region "Paint methods"
        ''' <summary>
        ''' Paints the hex box.
        ''' </summary>
        ''' <paramname="e">A PaintEventArgs that contains the event data.</param>
        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)

            If _byteProvider Is Nothing Then Return

            Debug.WriteLine("OnPaint " & Date.Now.ToString(), "HexBox")

            ' draw only in the content rectangle, so exclude the border and the scrollbar.
            Dim r As New Region(ClientRectangle)
            r.Exclude(_recContent)
            e.Graphics.ExcludeClip(r)

            UpdateVisibilityBytes()


            If _lineInfoVisible Then PaintLineInfo(e.Graphics, _startByte, _endByte)

            If Not _stringViewVisible Then
                PaintHex(e.Graphics, _startByte, _endByte)
            Else
                PaintHexAndStringView(e.Graphics, _startByte, _endByte)
                If _shadowSelectionVisible Then PaintCurrentBytesSign(e.Graphics)
            End If
            If _columnInfoVisible Then PaintHeaderRow(e.Graphics)
            If _groupSeparatorVisible Then PaintColumnSeparator(e.Graphics)

            RaiseEvent AfterPaint(Me, e)
        End Sub

        ''' <summary>
        ''' Paints the background.
        ''' </summary>
        ''' <paramname="e">A PaintEventArgs that contains the event data.</param>
        Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
            Select Case _borderStyle
                Case BorderStyle.Fixed3D
                    If TextBoxRenderer.IsSupported Then
                        Dim state = VisualStyleElement.TextBox.TextEdit.Normal
                        Dim backColor = Me.BackColor

                        If Enabled Then
                            If [ReadOnly] Then
                                state = VisualStyleElement.TextBox.TextEdit.ReadOnly
                            ElseIf Focused Then
                                state = VisualStyleElement.TextBox.TextEdit.Focused
                            End If
                        Else
                            state = VisualStyleElement.TextBox.TextEdit.Disabled
                            backColor = BackColorDisabled
                        End If

                        Dim vsr As New VisualStyleRenderer(state)
                        vsr.DrawBackground(e.Graphics, ClientRectangle)

                        Dim rectContent = vsr.GetBackgroundContentRectangle(e.Graphics, ClientRectangle)
                        e.Graphics.FillRectangle(New SolidBrush(backColor), rectContent)
                    Else
                        ' draw background
                        e.Graphics.FillRectangle(New SolidBrush(BackColor), ClientRectangle)

                        ' draw default border
                        ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, Border3DStyle.Sunken)
                    End If

                    Exit Select
                Case BorderStyle.FixedSingle
                    ' draw background
                    e.Graphics.FillRectangle(New SolidBrush(BackColor), ClientRectangle)

                    ' draw fixed single border
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Black, ButtonBorderStyle.Solid)
                    Exit Select
                Case Else
                    ' draw background
                    e.Graphics.FillRectangle(New SolidBrush(BackColor), ClientRectangle)
                    Exit Select
            End Select
        End Sub
        Private Function GetDefaultForeColor() As Color
            If Enabled Then
                Return ForeColor
            Else
                Return Color.Gray
            End If
        End Function

        Private Sub PaintColumnInfo(g As Graphics, b As Byte, brush As Brush, col As Integer)
            Dim formatStruct = {16, 10, 8}
            Dim headerPointF = GetColumnInfoPointF(col)
            Dim sB As String
            If _lineInfoFormat = 0 Then
                sB = ConvertByteToHex(b)
            Else
                sB = Convert.ToString(col, formatStruct(_lineInfoFormat))
            End If
            If sB.Length = 1 Then sB = "0" & sB
            g.DrawString(sB.Substring(0, 1), Font, brush, headerPointF, _stringFormat)
            headerPointF.X += _charSize.Width
            g.DrawString(sB.Substring(1, 1), Font, brush, headerPointF, _stringFormat)
        End Sub

        Private Sub PaintColumnSeparator(g As Graphics)
            Dim col = GroupSize

            While col < _iHexMaxHBytes
                Dim pen = New Pen(New SolidBrush(InfoTextColor), 1)
                Dim headerPointF = GetColumnInfoPointF(col)
                headerPointF.X -= _charSize.Width / 2
                g.DrawLine(pen, headerPointF, New PointF(headerPointF.X, headerPointF.Y + _recColumnInfo.Height + _recHex.Height))
                If StringViewVisible Then
                    Dim byteStringPointF As PointF = GetByteStringPointF(New Point(col, 0))
                    headerPointF.X -= 2
                    g.DrawLine(pen, New PointF(byteStringPointF.X, byteStringPointF.Y), New PointF(byteStringPointF.X, byteStringPointF.Y + _recHex.Height))
                End If

                col += GroupSize
            End While
        End Sub

        Private Sub PaintCurrentByteSign(g As Graphics, rec As Rectangle)
            ' stack overflowexception on big files - workaround
            If rec.Top < 0 OrElse rec.Left < 0 OrElse rec.Width <= 0 OrElse rec.Height <= 0 Then Return

            Dim myBitmap As New Bitmap(rec.Width, rec.Height)
            Dim bitmapGraphics = Graphics.FromImage(myBitmap)

            Dim greenBrush As New SolidBrush(_shadowSelectionColor)

            bitmapGraphics.FillRectangle(greenBrush, 0, 0, rec.Width, rec.Height)

            g.CompositingQuality = Drawing2D.CompositingQuality.GammaCorrected

            g.DrawImage(myBitmap, rec.Left, rec.Top)
        End Sub

        Private Sub PaintCurrentBytesSign(g As Graphics)
            If _keyInterpreter IsNot Nothing AndAlso _bytePos <> -1 AndAlso Enabled Then
                If _keyInterpreter.GetType() Is GetType(KeyInterpreter) Then
                    If _selectionLength = 0 Then
                        Dim gp = GetGridBytePoint(_bytePos - _startByte)
                        Dim pf = GetByteStringPointF(gp)
                        Dim s As New Size(_charSize.Width, _charSize.Height)
                        Dim r As New Rectangle(pf.X, pf.Y, s.Width, s.Height)
                        If r.IntersectsWith(_recStringView) Then
                            r.Intersect(_recStringView)
                            PaintCurrentByteSign(g, r)
                        End If
                    Else
                        Dim lineWidth As Integer = _recStringView.Width - _charSize.Width

                        Dim startSelGridPoint = GetGridBytePoint(_bytePos - _startByte)
                        Dim startSelPointF = GetByteStringPointF(startSelGridPoint)

                        Dim endSelGridPoint = GetGridBytePoint(_bytePos - _startByte + _selectionLength - 1)
                        Dim endSelPointF = GetByteStringPointF(endSelGridPoint)

                        Dim multiLine = endSelGridPoint.Y - startSelGridPoint.Y
                        If multiLine = 0 Then

                            Dim singleLine As New Rectangle(startSelPointF.X, startSelPointF.Y, endSelPointF.X - startSelPointF.X + _charSize.Width, _charSize.Height)
                            If singleLine.IntersectsWith(_recStringView) Then
                                singleLine.Intersect(_recStringView)
                                PaintCurrentByteSign(g, singleLine)
                            End If
                        Else
                            Dim firstLine As New Rectangle(startSelPointF.X, startSelPointF.Y, _recStringView.X + lineWidth - startSelPointF.X + _charSize.Width, _charSize.Height)
                            If firstLine.IntersectsWith(_recStringView) Then
                                firstLine.Intersect(_recStringView)
                                PaintCurrentByteSign(g, firstLine)
                            End If

                            If multiLine > 1 Then
                                Dim betweenLines As New Rectangle(_recStringView.X, startSelPointF.Y + _charSize.Height, _recStringView.Width, _charSize.Height * (multiLine - 1))
                                If betweenLines.IntersectsWith(_recStringView) Then
                                    betweenLines.Intersect(_recStringView)
                                    PaintCurrentByteSign(g, betweenLines)
                                End If

                            End If

                            Dim lastLine As New Rectangle(_recStringView.X, endSelPointF.Y, endSelPointF.X - _recStringView.X + _charSize.Width, _charSize.Height)
                            If lastLine.IntersectsWith(_recStringView) Then
                                lastLine.Intersect(_recStringView)
                                PaintCurrentByteSign(g, lastLine)
                            End If
                        End If
                    End If
                Else
                    If _selectionLength = 0 Then
                        Dim gp = GetGridBytePoint(_bytePos - _startByte)
                        Dim pf = GetBytePointF(gp)
                        Dim s As New Size(CInt(_charSize.Width) * 2, _charSize.Height)
                        Dim r As New Rectangle(pf.X, pf.Y, s.Width, s.Height)
                        PaintCurrentByteSign(g, r)
                    Else
                        Dim lineWidth As Integer = _recHex.Width - _charSize.Width * 5

                        Dim startSelGridPoint = GetGridBytePoint(_bytePos - _startByte)
                        Dim startSelPointF = GetBytePointF(startSelGridPoint)

                        Dim endSelGridPoint = GetGridBytePoint(_bytePos - _startByte + _selectionLength - 1)
                        Dim endSelPointF = GetBytePointF(endSelGridPoint)

                        Dim multiLine = endSelGridPoint.Y - startSelGridPoint.Y
                        If multiLine = 0 Then
                            Dim singleLine As New Rectangle(startSelPointF.X, startSelPointF.Y, endSelPointF.X - startSelPointF.X + _charSize.Width * 2, _charSize.Height)
                            If singleLine.IntersectsWith(_recHex) Then
                                singleLine.Intersect(_recHex)
                                PaintCurrentByteSign(g, singleLine)
                            End If
                        Else
                            Dim firstLine As New Rectangle(startSelPointF.X, startSelPointF.Y, _recHex.X + lineWidth - startSelPointF.X + _charSize.Width * 2, _charSize.Height)
                            If firstLine.IntersectsWith(_recHex) Then
                                firstLine.Intersect(_recHex)
                                PaintCurrentByteSign(g, firstLine)
                            End If

                            If multiLine > 1 Then
                                Dim betweenLines As New Rectangle(_recHex.X, startSelPointF.Y + _charSize.Height, lineWidth + _charSize.Width * 2, _charSize.Height * (multiLine - 1))
                                If betweenLines.IntersectsWith(_recHex) Then
                                    betweenLines.Intersect(_recHex)
                                    PaintCurrentByteSign(g, betweenLines)
                                End If

                            End If

                            Dim lastLine As New Rectangle(_recHex.X, endSelPointF.Y, endSelPointF.X - _recHex.X + _charSize.Width * 2, _charSize.Height)
                            If lastLine.IntersectsWith(_recHex) Then
                                lastLine.Intersect(_recHex)
                                PaintCurrentByteSign(g, lastLine)
                            End If
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub PaintHeaderRow(g As Graphics)
            Dim brush As Brush = New SolidBrush(InfoTextColor)
            Dim back As Brush = New SolidBrush(InfoBackColor)
            g.FillRectangle(back, 0, 0, ClientRectangle.Width, _recColumnInfo.Height)

            For col = 0 To _iHexMaxHBytes - 1
                PaintColumnInfo(g, col, brush, col)
            Next
        End Sub

        Private Sub PaintHex(g As Graphics, startByte As Long, endByte As Long)
            Dim brush As Brush = New SolidBrush(GetDefaultForeColor())
            Dim selBrush As Brush = New SolidBrush(_selectionTextColor)
            Dim selBrushBack As Brush = New SolidBrush(_selectionBackColor)

            Dim counter = -1
            Dim intern_endByte As Long = Math.Min(_byteProvider.Length - 1, endByte + _iHexMaxHBytes)

            Dim isKeyInterpreterActive As Boolean = _keyInterpreter Is Nothing OrElse _keyInterpreter.GetType() Is GetType(KeyInterpreter)

            For i = startByte To intern_endByte + 1 - 1
                counter += 1
                Dim gridPoint = GetGridBytePoint(counter)
                Dim b As Byte = _byteProvider.ReadByte(i)

                Dim isSelectedByte = i >= _bytePos AndAlso i <= _bytePos + _selectionLength - 1 AndAlso _selectionLength <> 0

                If isSelectedByte AndAlso isKeyInterpreterActive Then
                    PaintHexStringSelected(g, b, selBrush, selBrushBack, gridPoint)
                Else
                    PaintHexString(g, b, brush, gridPoint)
                End If
            Next
        End Sub

        Private Sub PaintHexAndStringView(g As Graphics, startByte As Long, endByte As Long)
            Dim brush As Brush = New SolidBrush(GetDefaultForeColor())
            Dim brushStr As Brush = New SolidBrush(_hexStringTextColor)
            Dim brushStrBack As Brush = New SolidBrush(_hexStringBackColor)
            Dim selBrush As Brush = New SolidBrush(_selectionTextColor)
            Dim selBrushBack As Brush = New SolidBrush(_selectionBackColor)

            Dim counter = -1
            Dim intern_endByte As Long = Math.Min(_byteProvider.Length - 1, endByte + _iHexMaxHBytes)

            Dim isKeyInterpreterActive As Boolean = _keyInterpreter Is Nothing OrElse _keyInterpreter.GetType() Is GetType(KeyInterpreter)
            Dim isStringKeyInterpreterActive As Boolean = _keyInterpreter IsNot Nothing AndAlso _keyInterpreter.GetType() Is GetType(StringKeyInterpreter)
            g.FillRectangle(brushStrBack, _recStringView)
            For i = startByte To intern_endByte + 1 - 1
                counter += 1
                Dim gridPoint = GetGridBytePoint(counter)
                Dim byteStringPointF = GetByteStringPointF(gridPoint)
                Dim b As Byte = _byteProvider.ReadByte(i)

                Dim isSelectedByte = i >= _bytePos AndAlso i <= _bytePos + _selectionLength - 1 AndAlso _selectionLength <> 0

                Dim current = i
                Dim hl = _highlightRegions.Find(Function(item) item.IsWithin(current))

                If isSelectedByte AndAlso isKeyInterpreterActive Then
                    PaintHexStringSelected(g, b, selBrush, selBrushBack, gridPoint)
                ElseIf Nothing IsNot hl Then
                    PaintHexStringSelected(g, b, New SolidBrush(hl.ForeColor), New SolidBrush(hl.BackColor), gridPoint)
                Else
                    PaintHexString(g, b, brush, gridPoint)
                End If

                Dim s As New [String](ByteCharConverter.ToChar(b), 1)

                If isSelectedByte AndAlso isStringKeyInterpreterActive Then
                    g.FillRectangle(selBrushBack, byteStringPointF.X, byteStringPointF.Y, _charSize.Width, _charSize.Height)
                    g.DrawString(s, Font, selBrush, byteStringPointF, _stringFormat)
                ElseIf Nothing IsNot hl Then
                    g.FillRectangle(New SolidBrush(hl.BackColor), byteStringPointF.X, byteStringPointF.Y, _charSize.Width, _charSize.Height)
                    g.DrawString(s, Font, New SolidBrush(hl.ForeColor), byteStringPointF, _stringFormat)
                Else
                    g.DrawString(s, Font, brushStr, byteStringPointF, _stringFormat)
                End If
            Next
        End Sub

        Private Sub PaintHexString(g As Graphics, b As Byte, brush As Brush, gridPoint As Point)
            Dim bytePointF = GetBytePointF(gridPoint)

            Dim sB = ConvertByteToHex(b)

            g.DrawString(sB.Substring(0, 1), Font, brush, bytePointF, _stringFormat)
            bytePointF.X += _charSize.Width
            g.DrawString(sB.Substring(1, 1), Font, brush, bytePointF, _stringFormat)
        End Sub

        Private Sub PaintHexStringSelected(g As Graphics, b As Byte, brush As Brush, brushBack As Brush, gridPoint As Point)
            Dim sB = b.ToString(_hexStringFormat, Threading.Thread.CurrentThread.CurrentCulture)
            If sB.Length = 1 Then sB = "0" & sB

            Dim bytePointF = GetBytePointF(gridPoint)

            Dim isLastLineChar = gridPoint.X + 1 = _iHexMaxHBytes
            Dim bcWidth = If(isLastLineChar, _charSize.Width * 2, _charSize.Width * 3)

            g.FillRectangle(brushBack, bytePointF.X, bytePointF.Y, bcWidth, _charSize.Height)
            g.DrawString(sB.Substring(0, 1), Font, brush, bytePointF, _stringFormat)
            bytePointF.X += _charSize.Width
            g.DrawString(sB.Substring(1, 1), Font, brush, bytePointF, _stringFormat)
        End Sub

        Private Sub PaintLineInfo(g As Graphics, startByte As Long, endByte As Long)
            Dim formatStruct = {16, 10, 8}
            ' Ensure endByte isn't > length of array.
            endByte = Math.Min(_byteProvider.Length - 1, endByte)

            Dim lineInfoColor = If(InfoTextColor <> Color.Empty, InfoTextColor, ForeColor)
            Dim brush As Brush = New SolidBrush(lineInfoColor)
            Dim back As Brush = New SolidBrush(InfoBackColor)
            g.FillRectangle(back, 0, 0, _recLineInfo.Width, ClientRectangle.Height)

            Dim maxLine = GetGridBytePoint(endByte - startByte).Y + 1

            For i = 0 To maxLine - 1
                Dim firstLineByte = startByte + _iHexMaxHBytes * i + _lineInfoOffset

                Dim bytePointF As PointF = GetBytePointF(New Point(0, 0 + i))
                Dim info As String
                If _lineInfoFormat = 0 Then
                    info = firstLineByte.ToString(_hexStringFormat, Threading.Thread.CurrentThread.CurrentCulture)
                Else
                    info = Convert.ToString(firstLineByte, formatStruct(_lineInfoFormat))
                End If
                Dim nulls = 8 - info.Length
                Dim formattedInfo As String
                If nulls > -1 Then
                    formattedInfo = New String("0"c, 8 - info.Length) & info
                Else
                    formattedInfo = New String("~"c, 8)
                End If

                g.DrawString(formattedInfo, Font, brush, New PointF(_recLineInfo.X, bytePointF.Y), _stringFormat)
            Next
        End Sub
        Private Sub UpdateVisibilityBytes()
            If _byteProvider Is Nothing OrElse _byteProvider.Length = 0 Then Return

            Dim prevStartByte = _startByte
            Dim prevEndByte = _endByte

            _startByte = (_scrollVpos + 1) * _iHexMaxHBytes - _iHexMaxHBytes
            _endByte = CLng(Math.Min(_byteProvider.Length - 1, _startByte + _iHexMaxBytes))

            If prevStartByte <> _startByte OrElse prevEndByte <> _endByte Then
                OnVisibilityBytesChanged(EventArgs.Empty)
            End If
        End Sub
#End Region

#Region "Positioning methods"
        Private Function GetBytePointF(byteIndex As Long) As PointF
            Dim gp = GetGridBytePoint(byteIndex)

            Return GetBytePointF(gp)
        End Function

        Private Function GetBytePointF(gp As Point) As PointF
            Dim x = 3 * _charSize.Width * gp.X + _recHex.X
            Dim y = (gp.Y + 1) * _charSize.Height - _charSize.Height + _recHex.Y

            Return New PointF(x, y)
        End Function

        Private Function GetByteStringPointF(gp As Point) As PointF
            Dim x = _charSize.Width * gp.X + _recStringView.X
            Dim y = (gp.Y + 1) * _charSize.Height - _charSize.Height + _recStringView.Y

            Return New PointF(x, y)
        End Function

        Private Function GetColumnInfoPointF(col As Integer) As PointF
            Dim gp = GetGridBytePoint(col)
            Dim x = 3 * _charSize.Width * gp.X + _recColumnInfo.X
            Dim y As Single = _recColumnInfo.Y

            Return New PointF(x, y)
        End Function

        Private Function GetGridBytePoint(byteIndex As Long) As Point
            Dim row As Integer = Math.Floor(byteIndex / _iHexMaxHBytes)
            Dim column As Integer = byteIndex + _iHexMaxHBytes - _iHexMaxHBytes * (row + 1)

            Dim res As New Point(column, row)
            Return res
        End Function

        Private Sub UpdateRectanglePositioning()
            ' calc char size
            Dim charSize As SizeF
            Using graphics = CreateGraphics()
                charSize = CreateGraphics().MeasureString("A", Font, 100, _stringFormat)
            End Using
            Me.CharSize = New SizeF(Math.Ceiling(charSize.Width), Math.Ceiling(charSize.Height))

            Dim requiredWidth = _recBorderRight + _recBorderLeft

            ' calc content bounds
            _recContent = ClientRectangle
            _recContent.X += _recBorderLeft
            _recContent.Y += _recBorderTop
            _recContent.Width -= _recBorderRight + _recBorderLeft
            _recContent.Height -= _recBorderBottom + _recBorderTop

            If _vScrollBarVisible Then
                _recContent.Width -= _vScrollBar.Width
                _vScrollBar.Left = _recContent.X + _recContent.Width
                _vScrollBar.Top = _recContent.Y
                _vScrollBar.Height = _recContent.Height
                requiredWidth += _vScrollBar.Width
            End If

            Dim marginLeft = 4

            ' calc line info bounds
            If _lineInfoVisible Then
                _recLineInfo = New Rectangle(_recContent.X + marginLeft, _recContent.Y, _charSize.Width * 10, _recContent.Height)
                requiredWidth += _recLineInfo.Width
            Else
                _recLineInfo = Rectangle.Empty
                _recLineInfo.X = marginLeft
                requiredWidth += marginLeft
            End If

            ' calc Column info bounds
            _recColumnInfo = New Rectangle(_recLineInfo.X + _recLineInfo.Width, _recContent.Y, _recContent.Width - _recLineInfo.Width, CInt(charSize.Height) + 4)
            If _columnInfoVisible Then
                _recLineInfo.Y += CInt(charSize.Height) + 4
                _recLineInfo.Height -= CInt(charSize.Height) + 4
            Else
                _recColumnInfo.Height = 0
            End If

            ' calc hex bounds and grid
            _recHex = New Rectangle(_recLineInfo.X + _recLineInfo.Width, _recLineInfo.Y, _recContent.Width - _recLineInfo.Width, _recContent.Height - _recColumnInfo.Height)

            If UseFixedBytesPerLine Then
                SetHorizontalByteCount(_bytesPerLine)
                _recHex.Width = CInt(Math.Floor(CDbl(_iHexMaxHBytes) * _charSize.Width * 3 + 2 * _charSize.Width))
                requiredWidth += _recHex.Width
            Else
                Dim hmax As Integer = Math.Floor(_recHex.Width / CDbl(_charSize.Width))
                If _stringViewVisible Then
                    hmax -= 2
                    If hmax > 1 Then
                        SetHorizontalByteCount(Math.Floor(hmax / 4))
                    Else
                        SetHorizontalByteCount(1)
                    End If
                Else
                    If hmax > 1 Then
                        SetHorizontalByteCount(Math.Floor(hmax / 3))
                    Else
                        SetHorizontalByteCount(1)
                    End If
                End If
                _recHex.Width = CInt(Math.Floor(CDbl(_iHexMaxHBytes) * _charSize.Width * 3 + 2 * _charSize.Width))
                requiredWidth += _recHex.Width
            End If

            If _stringViewVisible Then
                _recStringView = New Rectangle(_recHex.X + _recHex.Width, _recHex.Y, _charSize.Width * _iHexMaxHBytes, _recHex.Height)
                requiredWidth += _recStringView.Width
            Else
                _recStringView = Rectangle.Empty
            End If

            Me.RequiredWidth = requiredWidth

            Dim vmax As Integer = Math.Floor(_recHex.Height / CDbl(_charSize.Height))
            SetVerticalByteCount(vmax)

            _iHexMaxBytes = _iHexMaxHBytes * _iHexMaxVBytes

            UpdateScrollSize()
        End Sub
#End Region

#Region "Overridden properties"
        ''' <summary>
        ''' Gets or sets the background color for the control.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        <DefaultValue(GetType(Color), "White"), Browsable(False)>
        Public Overrides Property BackColor As Color
            Get
                Return MyBase.BackColor
            End Get
            Set(value As Color)
                MyBase.BackColor = value
            End Set
        End Property

        ''' <summary>
        ''' The font used to display text in the hexbox.
        ''' </summary>
        Public Overrides Property Font As Font
            Get
                Return MyBase.Font
            End Get
            Set(value As Font)
                If value Is Nothing Then Return

                MyBase.Font = value
                UpdateRectanglePositioning()
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the foreground color for the control.
        ''' </summary>
        <EditorBrowsable(EditorBrowsableState.Never)>
        <DefaultValue(GetType(Color), "Black"), Browsable(False)>
        Public Overrides Property ForeColor As Color
            Get
                Return MyBase.ForeColor
            End Get
            Set(value As Color)
                MyBase.ForeColor = value
            End Set
        End Property
        ''' <summary>
        ''' Not used.
        ''' </summary>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Bindable(False)>
        Public Overrides Property RightToLeft As RightToLeft
            Get
                Return MyBase.RightToLeft
            End Get
            Set(value As RightToLeft)
                MyBase.RightToLeft = value
            End Set
        End Property

        ''' <summary>
        ''' Not used.
        ''' </summary>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Bindable(False)>
        Public Overrides Property Text As String
            Get
                Return MyBase.Text
            End Get
            Set(value As String)
                MyBase.Text = value
            End Set
        End Property
#End Region

#Region "Properties"

        Private ReadOnly _builtInContextMenu As Forms.BuiltInContextMenu

        Private _borderStyle As BorderStyle = BorderStyle.Fixed3D

        Private _byteCharConverter As Forms.IByteCharConverter

        Private _byteProvider As Forms.IByteProvider

        Private _bytesPerLine As Integer = 16

        Private _charSize As SizeF

        Private _currentLine As Long

        Private _currentPositionInLine As Integer

        Private _groupSize As Integer = 4

        Private _lineInfoFormat As Integer = 0

        Private _lineInfoOffset As Long = 0

        Private _readOnly As Boolean

        Private _requiredWidth As Integer

        Private _selectionLength As Long

        Private _useFixedBytesPerLine As Boolean

        ''' <summary>
        ''' Gets or Sets HexBox read only property. If true it is not editable.
        ''' </summary>
        ''' <remarks>
        ''' When set to True, hexbox edit methods lock.
        ''' </remarks>
        <DefaultValue(False), Category("Hex"), Description("Gets or Sets HexBox read only property. If true it is not editable.")>
        Public Property [ReadOnly] As Boolean
            Get
                Return _readOnly
            End Get
            Set(value As Boolean)
                If _readOnly = value Then Return

                _readOnly = value
                OnReadOnlyChanged(EventArgs.Empty)
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the hex box´s border style.
        ''' </summary>
        <DefaultValue(GetType(BorderStyle), "Fixed3D"), Category("Hex"), Description("Gets or sets the hex box´s border style.")>
        Public Property BorderStyle As BorderStyle
            Get
                Return _borderStyle
            End Get
            Set(value As BorderStyle)
                If _borderStyle = value Then Return

                _borderStyle = value
                Select Case _borderStyle
                    Case BorderStyle.None
                        _recBorderLeft = 0
                        _recBorderTop = 0
                        _recBorderRight = 0
                        _recBorderBottom = 0

                    Case BorderStyle.Fixed3D
                        _recBorderLeft = SystemInformation.Border3DSize.Width
                        _recBorderRight = SystemInformation.Border3DSize.Width
                        _recBorderTop = SystemInformation.Border3DSize.Height
                        _recBorderBottom = SystemInformation.Border3DSize.Height

                    Case BorderStyle.FixedSingle
                        _recBorderLeft = 1
                        _recBorderTop = 1
                        _recBorderRight = 1
                        _recBorderBottom = 1
                End Select

                UpdateRectanglePositioning()

                OnBorderStyleChanged(EventArgs.Empty)

            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the built-in context menu.
        ''' </summary>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
        Public ReadOnly Property BuiltInContextMenu As Forms.BuiltInContextMenu
            Get
                Return _builtInContextMenu
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the converter that will translate between byte and character values.
        ''' </summary>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ByteCharConverter As Forms.IByteCharConverter
            Get
                If _byteCharConverter Is Nothing Then _byteCharConverter = New Forms.DefaultByteCharConverter()
                Return _byteCharConverter
            End Get
            Set(value As Forms.IByteCharConverter)
                If value IsNot Nothing Then
                    _byteCharConverter = value
                    Invalidate()
                End If
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the ByteProvider.
        ''' </summary>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property ByteProvider As Forms.IByteProvider
            Get
                Return _byteProvider
            End Get
            Set(value As Forms.IByteProvider)
                If _byteProvider Is value Then Return

                If value Is Nothing Then
                    ActivateEmptyKeyInterpreter()
                Else
                    ActivateKeyInterpreter()
                End If

                If _byteProvider IsNot Nothing Then RemoveHandler _byteProvider.LengthChanged, New EventHandler(AddressOf _byteProvider_LengthChanged)

                _byteProvider = value
                If _byteProvider IsNot Nothing Then AddHandler _byteProvider.LengthChanged, New EventHandler(AddressOf _byteProvider_LengthChanged)

                OnByteProviderChanged(EventArgs.Empty)

                If value Is Nothing Then ' do not raise events if value is null
                    _bytePos = -1
                    _byteCharacterPos = 0
                    _selectionLength = 0

                    DestroyCaret()
                Else
                    SetPosition(0, 0)
                    SetSelectionLength(0)

                    If _caretVisible AndAlso MyBase.Focused Then
                        UpdateCaret()
                    Else
                        CreateCaret()
                    End If
                End If

                _highlightRegions.Clear()

                CheckCurrentLineChanged()
                CheckCurrentPositionInLineChanged()

                _scrollVpos = 0

                UpdateVisibilityBytes()
                UpdateRectanglePositioning()

                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the maximum count of bytes in one line.
        ''' </summary>
        ''' <remarks>
        ''' UseFixedBytesPerLine property no longer has to be set to true for this to work
        ''' </remarks>
        <DefaultValue(16), Category("Hex"), Description("Gets or sets the maximum count of bytes in one line.")>
        Public Property BytesPerLine As Integer
            Get
                Return _bytesPerLine
            End Get
            Set(value As Integer)
                If _bytesPerLine = value Then Return

                _bytesPerLine = value
                OnBytesPerLineChanged(EventArgs.Empty)

                UpdateRectanglePositioning()
                Invalidate()
            End Set
        End Property
        ''' <summary>
        ''' Contains the size of a single character in pixel
        ''' </summary>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property CharSize As SizeF
            Get
                Return _charSize
            End Get
            Private Set(value As SizeF)
                If _charSize = value Then Return
                _charSize = value
                RaiseEvent CharSizeChanged(Me, EventArgs.Empty)
            End Set
        End Property

        ''' <summary>
        ''' Gets the current line
        ''' </summary>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property CurrentLine As Long
            Get
                Return _currentLine
            End Get
        End Property

        ''' <summary>
        ''' Gets the current position in the current line
        ''' </summary>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property CurrentPositionInLine As Long
            Get
                Return _currentPositionInLine
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the number of bytes in a group. Used to show the group separator line (if GroupSeparatorVisible is true)
        ''' </summary>
        ''' <remarks>
        ''' GroupSeparatorVisible property must set to true
        ''' </remarks>
        <DefaultValue(4), Category("Hex"), Description("Gets or sets the byte-count between group separators (if visible).")>
        Public Property GroupSize As Integer
            Get
                Return _groupSize
            End Get
            Set(value As Integer)
                If _groupSize = value Then Return

                _groupSize = value
                OnGroupSizeChanged(EventArgs.Empty)

                UpdateRectanglePositioning()
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether the HexBox control displays the hex characters in upper or lower case.
        ''' </summary>
        <ComponentModel.DefaultValueAttribute(GetType(Forms.HexCasing), "Upper"), Category("Hex"), Description("Gets or sets whether the HexBox control displays the hex characters in upper or lower case.")>
        Public Property HexCasing As Forms.HexCasing
            Get
                If Equals(_hexStringFormat, "X") Then
                    Return Forms.HexCasing.Upper
                Else
                    Return Forms.HexCasing.Lower
                End If
            End Get
            Set(value As Forms.HexCasing)
                Dim format As String
                If value = Forms.HexCasing.Upper Then
                    format = "X"
                Else
                    format = "x"
                End If

                If Equals(_hexStringFormat, format) Then Return

                _hexStringFormat = format
                OnHexCasingChanged(EventArgs.Empty)

                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets the number bytes drawn horizontally.
        ''' </summary>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property HorizontalByteCount As Integer
            Get
                Return _iHexMaxHBytes
            End Get
        End Property

        ''' <summary>
        ''' Gets the a value if true insertion mode active false overwrite mode.
        ''' </summary>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property InsertActive As Boolean
            Get
                Return _insertActive
            End Get
            Set(value As Boolean)
                If _insertActive = value Then Return

                _insertActive = value

                ' recreate caret
                DestroyCaret()
                CreateCaret()

                ' raise change event
                OnInsertActiveChanged(EventArgs.Empty)
            End Set
        End Property

        ''' <summary>
        ''' Changes line info format to hex, decimal or octal(0=Hex, 1=Decimal, 2=Octal).
        ''' </summary>
        ''' <remarks>
        ''' remark
        ''' </remarks>
        <DefaultValue(0), Category("Hex"), Description("Changes line info format to hex, decimal or octal (0=Hex, 1=Decimal, 2=Octal)")>
        Public Property LineInfoFormat As Integer
            Get
                Return _lineInfoFormat
            End Get
            Set(value As Integer)
                If _lineInfoFormat = value OrElse value > 2 OrElse value < 0 Then Return
                _lineInfoFormat = value
                OnLineInfoFormatChanged(EventArgs.Empty)
                Invalidate()
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the offset of a line info.
        ''' </summary>
        <DefaultValue(CLng(0)), Category("Hex"), Description("Gets or sets the offset of the line info.")>
        Public Property LineInfoOffset As Long
            Get
                Return _lineInfoOffset
            End Get
            Set(value As Long)
                If _lineInfoOffset = value Then Return

                _lineInfoOffset = value

                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets the width required for the content
        ''' </summary>
        <DefaultValue(0), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property RequiredWidth As Integer
            Get
                Return _requiredWidth
            End Get
            Private Set(value As Integer)
                If _requiredWidth = value Then Return
                _requiredWidth = value
                RaiseEvent RequiredWidthChanged(Me, EventArgs.Empty)
            End Set
        End Property

        ''' <summary>
        ''' Gets currently selected region
        ''' </summary>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property SelectedRegion As HighlightRegion
            Get
                Return _selectedRegion
            End Get
        End Property

        ''' <summary>
        ''' Gets and sets the number of bytes selected in the hex box.
        ''' </summary>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectionLength As Long
            Get
                Return _selectionLength
            End Get
            Set(value As Long)
                SetSelectionLength(value)
                ScrollByteIntoView()
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets and sets the starting point of the bytes selected in the hex box.
        ''' </summary>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Property SelectionStart As Long
            Get
                Return _bytePos
            End Get
            Set(value As Long)
                SetPosition(value, 0)
                ScrollByteIntoView()
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets if the count of bytes in one line is fix.
        ''' </summary>
        ''' <remarks>
        ''' When set to True, BytesPerLine property determine the maximum count of bytes in one line.
        ''' </remarks>
        <DefaultValue(False), Category("Hex"), Description("Gets or sets if the count of bytes in one line is fix.")>
        Public Property UseFixedBytesPerLine As Boolean
            Get
                Return _useFixedBytesPerLine
            End Get
            Set(value As Boolean)
                If _useFixedBytesPerLine = value Then Return

                _useFixedBytesPerLine = value
                OnUseFixedBytesPerLineChanged(EventArgs.Empty)

                UpdateRectanglePositioning()
                Invalidate()
            End Set
        End Property
#Region "Hex Appearance Colors"
        Private _hexStringBackColor As Color = Color.White

        Private _hexStringTextColor As Color = Color.Black

        Private _hlBackColor As Color = Color.Yellow

        Private _hlForeColor As Color = Color.Black

        Private _infoBackColor As Color = Color.White

        Private _infoTextColor As Color = Color.Gray

        Private _selectionBackColor As Color = Color.Blue

        Private _selectionTextColor As Color = Color.White

        Private _shadowSelectionColor As Color = Color.FromArgb(100, 60, 188, 255)

        ''' <summary>
        ''' Gets or sets the background color for the disabled HexBox.
        ''' </summary>
        <Category("HexAppearance"), DefaultValue(GetType(Color), "WhiteSmoke"), Description("Gets or sets the background color for the disabled HexBox")>
        Public Property BackColorDisabled As Color = Color.WhiteSmoke

        ''' <summary>
        ''' Gets or sets the  Hex string view back color.
        ''' </summary>
        <DefaultValue(GetType(Color), "White"), Category("HexAppearance"), Description("Gets or sets the Hex string view back color")>
        Public Property HexStringBackColor As Color
            Get
                Return _hexStringBackColor
            End Get
            Set(value As Color)
                _hexStringBackColor = value
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the Hex string view text color.
        ''' </summary>
        <DefaultValue(GetType(Color), "Black"), Category("HexAppearance"), Description("Gets or sets the Hex string view text color")>
        Public Property HexStringTextColor As Color
            Get
                Return _hexStringTextColor
            End Get
            Set(value As Color)
                _hexStringTextColor = value
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the Hex View  back color used for Hex View area.
        ''' </summary>
        <DefaultValue(GetType(Color), "White"), Category("HexAppearance"), Description("Gets or sets the Hex View back color.")>
        Public Property HexViewBackColor As Color
            Get
                Return MyBase.BackColor
            End Get
            Set(value As Color)
                MyBase.BackColor = value
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the Hex View text color used Hex View area . When this property is null, then ForeColor property is used.
        ''' </summary>
        <DefaultValue(GetType(Color), "Gray"), Category("HexAppearance"), Description("Gets or sets the Hex View  color. When this property is null, then ForeColor property is used.")>
        Public Property HexViewTextColor As Color
            Get
                Return MyBase.ForeColor
            End Get
            Set(value As Color)
                MyBase.ForeColor = value
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the background color for the highlighted bytes.
        ''' </summary>
        <DefaultValue(GetType(Color), "Yellow"), Category("HexAppearance"), Description("Highlight background color.")>
        Public Property HighlightBackColor As Color
            Get
                Return _hlBackColor
            End Get
            Set(value As Color)
                _hlBackColor = value
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the foreground color for the highlighted bytes.
        ''' </summary>
        <DefaultValue(GetType(Color), "Black"), Category("HexAppearance"), Description("Highlight foreground color.")>
        Public Property HighlightForeColor As Color
            Get
                Return _hlForeColor
            End Get
            Set(value As Color)
                _hlForeColor = value
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the info back color used for column info and line info. When this property is null, then ForeColor property is used.
        ''' </summary>
        <DefaultValue(GetType(Color), "White"), Category("HexAppearance"), Description("Gets or sets the line info color. When this property is null, then ForeColor property is used.")>
        Public Property InfoBackColor As Color
            Get
                Return _infoBackColor
            End Get
            Set(value As Color)
                _infoBackColor = value
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the info color used for column info and line info. When this property is null, then ForeColor property is used.
        ''' </summary>
        <DefaultValue(GetType(Color), "Gray"), Category("HexAppearance"), Description("Gets or sets the line info color. When this property is null, then ForeColor property is used.")>
        Public Property InfoTextColor As Color
            Get
                Return _infoTextColor
            End Get
            Set(value As Color)
                _infoTextColor = value
                Invalidate()
            End Set
        End Property
        'Color _infoTextColor = Color.Gray;
        'Color _infoBackColor = Color.White;
        ''' <summary>
        ''' Gets or sets the background color for the selected bytes.
        ''' </summary>
        <DefaultValue(GetType(Color), "Blue"), Category("HexAppearance"), Description("Gets or sets the background color for the selected bytes.")>
        Public Property SelectionBackColor As Color
            Get
                Return _selectionBackColor
            End Get
            Set(value As Color)
                _selectionBackColor = value
                Invalidate()
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the foreground color for the selected bytes.
        ''' </summary>
        <DefaultValue(GetType(Color), "White"), Category("HexAppearance"), Description("Gets or sets the foreground color for the selected bytes.")>
        Public Property SelectionTextColor As Color
            Get
                Return _selectionTextColor
            End Get
            Set(value As Color)
                _selectionTextColor = value
                Invalidate()
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the color of the shadow selection. 
        ''' </summary>
        ''' <remarks>
        ''' A alpha component must be given! 
        ''' Default alpha = 100
        ''' </remarks>
        <Category("HexAppearance"), Description("Gets or sets the color of the shadow selection.")>
        Public Property ShadowSelectionColor As Color
            Get
                Return _shadowSelectionColor
            End Get
            Set(value As Color)
                _shadowSelectionColor = value
                Invalidate()
            End Set
        End Property
#End Region

#Region "HexBox elements visibility"
        Private _columnInfoVisible As Boolean = False

        Private _groupSeparatorVisible As Boolean = False

        Private _lineInfoVisible As Boolean = False

        Private _shadowSelectionVisible As Boolean = True

        Private _stringViewVisible As Boolean

        Private _vScrollBarVisible As Boolean

        ''' <summary>
        ''' Gets or sets the visibility of the column info
        ''' </summary>
        <DefaultValue(False), Category("HexBoxVisibility"), Description("Gets or sets the visibility of header row.")>
        Public Property ColumnInfoVisible As Boolean
            Get
                Return _columnInfoVisible
            End Get
            Set(value As Boolean)
                If _columnInfoVisible = value Then Return

                _columnInfoVisible = value
                OnColumnInfoVisibleChanged(EventArgs.Empty)

                UpdateRectanglePositioning()
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the visibility of the group separator.
        ''' </summary>
        <DefaultValue(False), Category("HexBoxVisibility"), Description("Gets or sets the visibility of a separator vertical line.")>
        Public Property GroupSeparatorVisible As Boolean
            Get
                Return _groupSeparatorVisible
            End Get
            Set(value As Boolean)
                If _groupSeparatorVisible = value Then Return

                _groupSeparatorVisible = value
                OnGroupSeparatorVisibleChanged(EventArgs.Empty)

                UpdateRectanglePositioning()
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the visibility of a line info.
        ''' </summary>
        <DefaultValue(False), Category("HexBoxVisibility"), Description("Gets or sets the visibility of a line info.")>
        Public Property LineInfoVisible As Boolean
            Get
                Return _lineInfoVisible
            End Get
            Set(value As Boolean)
                If _lineInfoVisible = value Then Return

                _lineInfoVisible = value
                OnLineInfoVisibleChanged(EventArgs.Empty)

                UpdateRectanglePositioning()
                Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the visibility of a shadow selection.
        ''' </summary>
        <DefaultValue(True), Category("HexBoxVisibility"), Description("Gets or sets the visibility of a shadow selection.")>
        Public Property ShadowSelectionVisible As Boolean
            Get
                Return _shadowSelectionVisible
            End Get
            Set(value As Boolean)
                If _shadowSelectionVisible = value Then Return
                _shadowSelectionVisible = value
                Invalidate()
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the visibility of the string view.
        ''' </summary>
        <DefaultValue(False), Category("HexBoxVisibility"), Description("Gets or sets the visibility of the string view.")>
        Public Property StringViewVisible As Boolean
            Get
                Return _stringViewVisible
            End Get
            Set(value As Boolean)
                If _stringViewVisible = value Then Return

                _stringViewVisible = value
                OnStringViewVisibleChanged(EventArgs.Empty)

                UpdateRectanglePositioning()
                Invalidate()
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the visibility of a vertical scroll bar.
        ''' </summary>
        <DefaultValue(True), Category("HexBoxVisibility"), Description("Gets or sets the visibility of a vertical scroll bar.")>
        Public Property VScrollBarVisible As Boolean
            Get
                Return _vScrollBarVisible
            End Get
            Set(value As Boolean)
                If _vScrollBarVisible = value Then Return

                _vScrollBarVisible = value

                If _vScrollBarVisible Then
                    Controls.Add(_vScrollBar)
                Else
                    Controls.Remove(_vScrollBar)
                End If

                UpdateRectanglePositioning()
                UpdateScrollSize()

                OnVScrollBarVisibleChanged(EventArgs.Empty)
            End Set
        End Property
#End Region
        ''' <summary>
        ''' Gets the number bytes drawn vertically.
        ''' </summary>
        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property VerticalByteCount As Integer
            Get
                Return _iHexMaxVBytes
            End Get
        End Property
#End Region

#Region "Misc"
        ''' <summary>
        ''' Raises the BorderStyleChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnBorderStyleChanged(e As EventArgs)
            RaiseEvent BorderStyleChanged(Me, e)
        End Sub

        Protected Overridable Sub OnByteChanged(e As ByteChangedArgs)
            RaiseEvent ByteChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the ByteProviderChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnByteProviderChanged(e As EventArgs)
            RaiseEvent ByteProviderChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the BytesPerLineChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnBytesPerLineChanged(e As EventArgs)
            RaiseEvent BytesPerLineChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the OnColumnInfoVisibleChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnColumnInfoVisibleChanged(e As EventArgs)
            RaiseEvent ColumnInfoVisibleChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the Copied event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnCopied(e As EventArgs)
            RaiseEvent Copied(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the CopiedHex event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnCopiedHex(e As EventArgs)
            RaiseEvent CopiedHex(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the CurrentLineChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnCurrentLineChanged(e As EventArgs)
            RaiseEvent CurrentLineChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the CurrentPositionInLineChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnCurrentPositionInLineChanged(e As EventArgs)
            RaiseEvent CurrentPositionInLineChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the GotFocus event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overrides Sub OnGotFocus(e As EventArgs)
            Debug.WriteLine("OnGotFocus()", "HexBox")

            MyBase.OnGotFocus(e)

            CreateCaret()
        End Sub

        ''' <summary>
        ''' Raises the ColumnSeparatorVisibleChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnGroupSeparatorVisibleChanged(e As EventArgs)
            RaiseEvent GroupSeparatorVisibleChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the GroupSizeChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnGroupSizeChanged(e As EventArgs)
            RaiseEvent GroupSizeChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the HexCasingChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnHexCasingChanged(e As EventArgs)
            RaiseEvent HexCasingChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the HighlightRegionAdded event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnHighlightRegionAdded(e As EventArgs)
            RaiseEvent HighlightRegionAdded(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the HighlightRegionSelected event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnHighlightRegionSelected(e As EventArgs)
            RaiseEvent HighlightRegionSelected(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the HorizontalByteCountChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnHorizontalByteCountChanged(e As EventArgs)
            RaiseEvent HorizontalByteCountChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the InsertActiveChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnInsertActiveChanged(e As EventArgs)
            RaiseEvent InsertActiveChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the OnLineInfoFormat Changed event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnLineInfoFormatChanged(e As EventArgs)
            RaiseEvent LineInfoFormatChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the LineInfoVisibleChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnLineInfoVisibleChanged(e As EventArgs)
            RaiseEvent LineInfoVisibleChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the LostFocus event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overrides Sub OnLostFocus(e As EventArgs)
            Debug.WriteLine("OnLostFocus()", "HexBox")

            MyBase.OnLostFocus(e)

            DestroyCaret()
            If _keyInterpreter IsNot Nothing Then _keyInterpreter.ClearShiftDown()

        End Sub

        ''' <summary>
        ''' Raises the MouseDown event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
            Debug.WriteLine("OnMouseDown()", "HexBox")

            If Not MyBase.Focused Then Focus()

            If e.Button = MouseButtons.Left Then SetCaretPosition(New Point(e.X, e.Y))

            MyBase.OnMouseDown(e)
        End Sub

        ''' <summary>
        ''' Raises the MouseWhell event
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)
            Dim linesToScroll As Integer = -(e.Delta * SystemInformation.MouseWheelScrollLines / 120)
            PerformScrollLines(linesToScroll)

            MyBase.OnMouseWheel(e)
        End Sub

        ''' <summary>
        ''' Raises the ReadOnlyChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnReadOnlyChanged(e As EventArgs)
            RaiseEvent ReadOnlyChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the Resize event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overrides Sub OnResize(e As EventArgs)
            MyBase.OnResize(e)
            UpdateRectanglePositioning()
        End Sub

        ''' <summary>
        ''' Raises the SelectionLengthChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnSelectionLengthChanged(e As EventArgs)
            RaiseEvent SelectionLengthChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the SelectionStartChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnSelectionStartChanged(e As EventArgs)
            RaiseEvent SelectionStartChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the StringViewVisibleChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnStringViewVisibleChanged(e As EventArgs)
            RaiseEvent StringViewVisibleChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the UseFixedBytesPerLineChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnUseFixedBytesPerLineChanged(e As EventArgs)
            RaiseEvent UseFixedBytesPerLineChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the VerticalByteCountChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnVerticalByteCountChanged(e As EventArgs)
            RaiseEvent VerticalByteCountChanged(Me, e)
        End Sub

        Protected Overridable Sub OnVisibilityBytesChanged(e As EventArgs)
            RaiseEvent VisibilityBytesChanged(Me, e)
        End Sub

        ''' <summary>
        ''' Raises the VScrollBarVisibleChanged event.
        ''' </summary>
        ''' <paramname="e">An EventArgs that contains the event data.</param>
        Protected Overridable Sub OnVScrollBarVisibleChanged(e As EventArgs)
            RaiseEvent VScrollBarVisibleChanged(Me, e)
        End Sub

        <CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification:="<Pending>")>
        Private Sub _byteProvider_LengthChanged(sender As Object, e As EventArgs)
            UpdateScrollSize()
        End Sub

        Private Sub CheckCurrentLineChanged()
            Dim currentLine = CLng(Math.Floor(_bytePos / _iHexMaxHBytes)) + 1

            If _byteProvider Is Nothing AndAlso _currentLine <> 0 Then
                _currentLine = 0
                OnCurrentLineChanged(EventArgs.Empty)
            ElseIf currentLine <> _currentLine Then
                _currentLine = currentLine
                OnCurrentLineChanged(EventArgs.Empty)
            End If
        End Sub

        Private Sub CheckCurrentPositionInLineChanged()
            Dim gb = GetGridBytePoint(_bytePos)
            Dim currentPositionInLine = gb.X + 1

            If _byteProvider Is Nothing AndAlso _currentPositionInLine <> 0 Then
                _currentPositionInLine = 0
                OnCurrentPositionInLineChanged(EventArgs.Empty)
            ElseIf currentPositionInLine <> _currentPositionInLine Then
                _currentPositionInLine = currentPositionInLine
                OnCurrentPositionInLineChanged(EventArgs.Empty)
            End If
        End Sub

        ''' <summary>
        ''' Converts a byte array to a hex string. For example: {10,11} = "0A 0B"
        ''' </summary>
        ''' <paramname="data">the byte array</param>
        ''' <returns>the hex string</returns>
        Private Function ConvertBytesToHex(data As Byte()) As String
            Dim sb As New StringBuilder()
            For Each b In data
                Dim hex = ConvertByteToHex(b)
                sb.Append(hex)
                sb.Append(" ")
            Next
            If sb.Length > 0 Then sb.Remove(sb.Length - 1, 1)
            Dim result As String = sb.ToString()
            Return result
        End Function
        ''' <summary>
        ''' Converts the byte to a hex string. For example: "10" = "0A";
        ''' </summary>
        ''' <paramname="b">the byte to format</param>
        ''' <returns>the hex string</returns>
        Private Function ConvertByteToHex(b As Byte) As String
            Dim sB = b.ToString(_hexStringFormat, Threading.Thread.CurrentThread.CurrentCulture)
            If sB.Length = 1 Then sB = "0" & sB
            Return sB
        End Function
        Private Function ConvertHexToByte(hex As String, <Out> ByRef b As Byte) As Boolean
            Dim isByte = Byte.TryParse(hex, Globalization.NumberStyles.HexNumber, Threading.Thread.CurrentThread.CurrentCulture, b)
            Return isByte
        End Function

        ''' <summary>
        ''' Converts the hex string to an byte array. The hex string must be separated by a space char ' ' or no spaces. If there is any invalid hex information in the string the result will be null.
        ''' </summary>
        ''' <paramname="hex">the hex string separated by ' '. For example: "0A 0B 0C" or "0A0B0C"</param>
        ''' <returns>the byte array. null if hex is invalid or empty</returns>
        Private Function ConvertHexToBytes(hex As String) As Byte()
            If String.IsNullOrEmpty(hex) Then Return Nothing

            hex = hex.Trim()

            Dim regex = New Regex("^[0-9A-F]*$", RegexOptions.IgnoreCase)
            Dim regexSpaces = New Regex("^([0-9A-F]{1,2} )*([0-9A-F]{1,2})?$", RegexOptions.IgnoreCase)

            Dim hexArray As String() 'hex.Split(' ');
            'added parse hexstring no whitespaces version 2.0.1
            If regexSpaces.IsMatch(hex) Then
                hexArray = hex.Split()
            ElseIf regex.IsMatch(hex) Then
                If hex.Length Mod 2 = 1 Then
                    hex = "0" & hex
                End If
                hexArray = New String(CInt(Math.Round(hex.Length / 2.0R - 1.0R + 1)) - 1) {}
                Dim i = 0, loopTo As Integer = Math.Round(hex.Length / 2.0R - 1.0R)

                While i <= loopTo
                    hexArray(i) = hex.Substring(i * 2, 2)
                    i += 1
                End While
            Else
                Return Nothing
            End If


            Dim byteArray = New Byte(hexArray.Length - 1) {}
            For i = 0 To hexArray.Length - 1
                Dim hexValue = hexArray(i)

                Dim b As Byte
                Dim isByte = ConvertHexToByte(hexValue, b)
                If Not isByte Then Return Nothing
                byteArray(i) = b
            Next

            Return byteArray
        End Function
        Private Sub SetHorizontalByteCount(value As Integer)
            If _iHexMaxHBytes = value Then Return

            _iHexMaxHBytes = value
            OnHorizontalByteCountChanged(EventArgs.Empty)
        End Sub

        Private Sub SetPosition(bytePos As Long)
            SetPosition(bytePos, _byteCharacterPos)
        End Sub

        Private Sub SetPosition(bytePos As Long, byteCharacterPos As Integer)
            If _byteCharacterPos <> byteCharacterPos Then
                _byteCharacterPos = byteCharacterPos
            End If

            If bytePos <> _bytePos Then
                _bytePos = bytePos
                CheckCurrentLineChanged()
                CheckCurrentPositionInLineChanged()

                OnSelectionStartChanged(EventArgs.Empty)

                _selectedRegion = _highlightRegions.Find(Function(item) item.IsWithin(_bytePos))
                If Nothing IsNot _selectedRegion Then
                    OnHighlightRegionSelected(EventArgs.Empty)
                End If
            End If
        End Sub

        Private Sub SetSelectionLength(selectionLength As Long)
            If selectionLength <> _selectionLength Then
                _selectionLength = selectionLength
                OnSelectionLengthChanged(EventArgs.Empty)
            End If
        End Sub
        Private Sub SetVerticalByteCount(value As Integer)
            If _iHexMaxVBytes = value Then Return

            _iHexMaxVBytes = value
            OnVerticalByteCountChanged(EventArgs.Empty)
        End Sub
#End Region

#Region "Scaling Support for High DPI resolution screens"
        ''' <summary>
        ''' For high resolution screen support
        ''' </summary>
        ''' <paramname="factor">the factor</param>
        ''' <paramname="specified">bounds</param>
        Protected Overrides Sub ScaleControl(factor As SizeF, specified As BoundsSpecified)
            MyBase.ScaleControl(factor, specified)

            BeginInvoke(New MethodInvoker(Sub()
                                              UpdateRectanglePositioning()
                                              If _caretVisible Then
                                                  DestroyCaret()
                                                  CreateCaret()
                                              End If
                                              Invalidate()
                                          End Sub))
        End Sub

#End Region
    End Class
End Namespace
