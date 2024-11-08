Imports System.Runtime.InteropServices
Imports System.Text
Imports DiskImageTool.DiskImage

Module Utility

    Public Function ByteArrayToString(b() As Byte) As String
        Dim SB As New Text.StringBuilder(b.Length)
        For counter = 0 To b.Length - 1
            SB.Append(Chr(b(counter)))
        Next
        Return SB.ToString
    End Function

    Public Function CleanFileName(FileName As String) As String
        Return CleanString(FileName, IO.Path.GetInvalidFileNameChars(), "_")
    End Function

    Public Function CleanPathName(FileName As String) As String
        Return CleanString(FileName, IO.Path.GetInvalidPathChars(), "_")
    End Function

    Public Sub DebugException(ex As Exception)
        Debug.Write("Caught Exception: ")
        Debug.Write("0x" & ex.HResult.ToString("X8"))
        Debug.WriteLine(" - " & ex.Message)
        Debug.WriteLine(ex.StackTrace)
    End Sub
    Public Function DuplicateHashTable(Table As Hashtable) As Hashtable
        Dim NewTable As New Hashtable
        For Each Key In Table.Keys
            NewTable.Item(Key) = Table.Item(Key)
        Next

        Return NewTable
    End Function

    Public Function FileDialogAppendFilter(FileFilter As String, Description As String, Extension As String) As String
        Return FileFilter & IIf(FileFilter = "", "", "|") & FileDialogGetFilter(Description, Extension)
    End Function

    Public Function FileDialogAppendFilter(FileFilter As String, Description As String, ExtensionList As List(Of String)) As String
        Return FileFilter & IIf(FileFilter = "", "", "|") & FileDialogGetFilter(Description, ExtensionList)
    End Function

    Public Function FileDialogGetFilter(Description As String, Extension As String) As String
        Return Description & " (*" & Extension & ")|" & "*" & Extension
    End Function

    Public Function FileDialogGetFilter(Description As String, ExtensionList As List(Of String)) As String
        Dim Extensions = ExtensionList.ToArray

        For Counter = 0 To Extensions.Length - 1
            Extensions(Counter) = "*" & Extensions(Counter)
        Next

        Return Description & " (" & String.Join("; ", Extensions) & ")|" & String.Join(";", Extensions)
    End Function

    Public Function FillArray(Length As UInteger, FillChar As Byte) As Byte()
        Dim b(Length - 1) As Byte
        For i = 0 To Length - 1
            b(i) = FillChar
        Next

        Return b
    End Function

    Public Sub FillArray(b() As Byte, FillChar As Byte)
        For i = 0 To b.Length - 1
            b(i) = FillChar
        Next
    End Sub

    Public Function GetDownloadsFolder() As String

        Dim Result As String = ""
        Dim ppszPath As IntPtr
        Dim rfid = New Guid("{374DE290-123F-4565-9164-39C4925E467B}")

        If SHGetKnownFolderPath(rfid, 0, 0, ppszPath) = 0 Then
            Result = Marshal.PtrToStringUni(ppszPath)
            Marshal.FreeCoTaskMem(ppszPath)
        End If

        Return Result
    End Function

    Public Function GetImageTypeFromHeader(Data() As Byte) As FloppyImageType
        If Encoding.UTF8.GetString(Data, 0, 8) = "HXCPICFE" Then
            Return FloppyImageType.HFEImage
        ElseIf Encoding.UTF8.GetString(Data, 0, 8) = "HXCHFEV3" Then
            Return FloppyImageType.HFEImage
        ElseIf Encoding.UTF8.GetString(Data, 0, 4) = "86BF" Then
            Return FloppyImageType.D86FImage
        ElseIf Encoding.UTF8.GetString(Data, 0, 6) = "HXCMFM" Then
            Return FloppyImageType.MFMImage
        ElseIf Encoding.UTF8.GetString(Data, 0, 4) = "PSI " Then
            Return FloppyImageType.PSIImage
        ElseIf Encoding.UTF8.GetString(Data, 0, 4) = "PRI " Then
            Return FloppyImageType.PRIImage
        ElseIf Encoding.UTF8.GetString(Data, 0, 4) = "IMD " Then
            Return FloppyImageType.IMDImage
        ElseIf BitConverter.ToUInt16(Data, 0) = &HA55A Then
            Return FloppyImageType.TranscopyImage
        Else
            Return FloppyImageType.BasicSectorImage
        End If
    End Function

    Public Function GetImageTypeFromFileName(FileName As String) As FloppyImageType
        Dim FileExt = IO.Path.GetExtension(FileName).ToLower

        If FileExt = ".tc" Then
            Return FloppyImageType.TranscopyImage
        ElseIf FileExt = ".psi" Then
            Return FloppyImageType.PSIImage
        ElseIf FileExt = ".pri" Then
            Return FloppyImageType.PRIImage
        ElseIf FileExt = ".mfm" Then
            Return FloppyImageType.MFMImage
        ElseIf FileExt = ".hfe" Then
            Return FloppyImageType.HFEImage
        ElseIf FileExt = ".86f" Then
            Return FloppyImageType.D86FImage
        ElseIf FileExt = ".imd" Then
            Return FloppyImageType.IMDImage
        Else
            Return FloppyImageType.BasicSectorImage
        End If
    End Function

    Public Function GetVersionString() As String
        Dim Version = FileVersionInfo.GetVersionInfo(Application.ExecutablePath)
        Return Version.FileMajorPart & "." & Version.FileMinorPart.ToString.PadLeft(2, "0") & "." & Version.FilePrivatePart.ToString.PadLeft(2, "0")
    End Function

    Public Function HexStringToBytes(ByVal HexString As String) As Byte()
        Dim b(HexString.Length / 2 - 1) As Byte

        For i As Integer = 0 To HexString.Length - 1 Step 2
            b(i / 2) = Convert.ToByte(HexString.Substring(i, 2), 16)
        Next

        Return b
    End Function

    Public Function IsFileReadOnly(fileName As String) As Boolean
        Dim fInfo As New IO.FileInfo(fileName)
        Return fInfo.IsReadOnly
    End Function

    Public Function MsgBoxQuestion(Prompt As String, Optional Title As Object = Nothing) As Boolean
        Return MsgBox(Prompt, MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, Title) = MsgBoxResult.Yes
    End Function

    Public Function PathAddBackslash(Path As String) As String
        If Len(Path) > 0 Then
            If Not Path.EndsWith("\") Then
                Path &= "\"
            End If
        End If
        Return Path
    End Function

    Public Sub ResizeArray(ByRef b() As Byte, Length As UInteger, Padding As Byte)
        Dim Size = b.Length - 1
        If Size <> Length - 1 Then
            ReDim Preserve b(Length - 1)
            For Counter As UInteger = Size + 1 To Length - 1
                b(Counter) = Padding
            Next
        End If
    End Sub

    Public Function SaveByteArrayToFile(FilePath As String, Data() As Byte) As Boolean
        Try
            IO.File.WriteAllBytes(FilePath, Data)
        Catch ex As Exception
            DebugException(ex)
            Return False
        End Try

        Return True
    End Function

    Private Function CleanString(Value As String, InvalidChars() As Char, ReplaceWith As String) As String
        Dim NewValue As String = ""
        For Counter = 0 To Value.Length - 1
            Dim FileChar = Value.Substring(Counter, 1)
            If InvalidChars.Contains(FileChar) Then
                NewValue &= ReplaceWith
            Else
                NewValue &= FileChar
            End If
        Next

        Return NewValue
    End Function

    <DllImport("shell32.dll")>
    Private Function SHGetKnownFolderPath(<MarshalAs(UnmanagedType.LPStruct)> ByVal rfid As Guid,
        ByVal dwFlags As UInt32,
        ByVal hToken As IntPtr,
        ByRef pszPath As IntPtr) As Int32
    End Function

End Module