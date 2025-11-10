Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Text
Imports DiskImageTool.DiskImage

Module Utility
    Public Structure FileParts
        Dim Name As String
        Dim Extension As String
    End Structure

    Public Sub ApplyResourcesToControls(ctrl As Control, res As System.ComponentModel.ComponentResourceManager)
        res.ApplyResources(ctrl, ctrl.Name)

        For Each child As Control In ctrl.Controls
            ApplyResourcesToControls(child, res)
        Next
    End Sub

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

    Public Function FormatLabelPair(Label As String, Value As String, Optional Separator As String = ": ") As String
        If Label.Length > 0 Then
            Return Label & Separator & Value
        Else
            Return Value
        End If
    End Function

    Public Function FormatThousands(Value As Object) As String
        Return Format(Value, "N0")
    End Function

    Public Function FormatTrackSide(Caption As String, Track As String, Side As String) As String
        Return Caption & " " & Track & "." & Side
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

    Public Function Quoted(Value As String) As String
        Const q As Char = ControlChars.Quote

        If Value Is Nothing Then
            Return New String({q, q})
        End If

        Return q & Value.Replace(q, q & q) & q
    End Function

    Public Function GenerateUniqueFileName(FilePath As String, FileName As String) As String
        Dim NewFileName As String
        Dim Suffix As Integer = 1
        Dim FilePart = IO.Path.GetFileNameWithoutExtension(FileName)
        Dim ExtPart = IO.Path.GetExtension(FileName)

        Do
            NewFileName = IO.Path.Combine(FilePath, FilePart & If(Suffix = 1, "", " " & Suffix) & ExtPart)
            Suffix += 1
        Loop Until Not IO.File.Exists(NewFileName)

        Return NewFileName
    End Function

    Public Function GetAvailableLanguages() As List(Of CultureInfo)
        Dim result As New List(Of CultureInfo)()
        Dim baseDir As String = AppDomain.CurrentDomain.BaseDirectory
        Dim assemblyName As String = Reflection.Assembly.GetExecutingAssembly().GetName().Name

        For Each dir As String In IO.Directory.GetDirectories(baseDir)
            Try
                Dim cultureName As String = IO.Path.GetFileName(dir)
                Dim culture As New CultureInfo(cultureName)

                ' Check if satellite assembly exists for the culture
                Dim satPath As String = IO.Path.Combine(dir, $"{assemblyName}.resources.dll")
                If IO.File.Exists(satPath) Then
                    result.Add(culture)
                End If
            Catch ex As CultureNotFoundException
                ' Ignore folders that aren't valid cultures
            End Try
        Next

        Return result
    End Function

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

    Public Function InParens(text As String) As String
        Return "(" & text & ")"
    End Function

    Public Function IsBinaryData(data As Byte(), Optional BytesToCheck As Integer = 4096) As Boolean
        Dim allowedControlChars As Byte() = {7, 8, 9, 10, 11, 12, 13, 26, 27}
        Dim maxBytesToCheck As Integer = Math.Min(BytesToCheck, data.Length)

        For i As Integer = 0 To maxBytesToCheck - 1
            Dim b As Byte = data(i)

            If (b < 32 AndAlso Not allowedControlChars.Contains(b)) OrElse b = 0 Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Function DeleteFileIfExists(FilePath As String) As Boolean
        Try
            If IO.File.Exists(FilePath) Then
                IO.File.Delete(FilePath)
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function IsFileReadOnly(fileName As String) As Boolean
        Dim fInfo As New IO.FileInfo(fileName)
        Return fInfo.IsReadOnly
    End Function

    Public Function IsZipArchive(FileName As String) As IO.Compression.ZipArchive
        Try
            Dim Buffer(1) As Byte
            Using fs = New IO.FileStream(FileName, IO.FileMode.Open, IO.FileAccess.Read)
                Dim BytesRead = fs.Read(Buffer, 0, Buffer.Length)
                fs.Close()
            End Using

            If Buffer(0) = &H50 And Buffer(1) = &H4B Then
                Return IO.Compression.ZipFile.OpenRead(FileName)
            End If
        Catch ex As Exception
            DebugException(ex)
            Return Nothing
        End Try

        Return Nothing
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

    Public Function ReadFileIntoBuffer(FileInfo As IO.FileInfo, FileSize As UInteger, FillChar As Byte) As Byte()
        Dim FileBuffer(FileSize - 1) As Byte
        Dim n As Integer
        Using fs = FileInfo.OpenRead()
            n = fs.Read(FileBuffer, 0, Math.Min(FileInfo.Length, FileBuffer.Length))
        End Using
        For Counter As Integer = n To FileBuffer.Length - 1
            FileBuffer(Counter) = FillChar
        Next

        Return FileBuffer
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

    Public Function SplitFilename(FileName As String) As FileParts
        Dim FileParts As FileParts

        FileParts.Name = IO.Path.GetFileNameWithoutExtension(FileName)
        FileParts.Extension = IO.Path.GetExtension(FileName)

        If FileParts.Extension.Length > 0 Then
            FileParts.Extension = FileParts.Extension.Substring(1)
        End If

        Return FileParts
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