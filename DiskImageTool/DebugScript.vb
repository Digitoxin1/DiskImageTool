Imports System.IO
Imports System.IO.Compression
Imports System.Text

Module DebugScript

    Public Sub GenerateDebugPackage(Disk As DiskImage.Disk, ImageData As LoadedImageData)
        Dim TempPath As String = Path.GetTempPath() & Guid.NewGuid().ToString()
        Dim ContentPath As String = Path.Combine(TempPath, "CONTENT")
        Dim ToolsPath As String = Path.Combine(ContentPath, "TOOLS")
        Dim DataPath As String = Path.Combine(ContentPath, "DATA")
        Dim ZipPath As String = Path.Combine(TempPath, "PATCH.ZIP")

        Directory.CreateDirectory(DataPath)
        Directory.CreateDirectory(ToolsPath)

        'GenerateDebugScripts(DataPath, Disk, ImageData.SessionModifications)
        GenerateDirectoryDump(DataPath, Disk, ImageData.CachedRootDir)

        WriteResourceToFile("CHOICE.COM", Path.Combine(ToolsPath, "CHOICE.COM"))
        WriteResourceToFile("CRC32.COM", Path.Combine(ToolsPath, "CRC32.COM"))
        WriteResourceToFile("PATCH.BAT", Path.Combine(ContentPath, "PATCH.BAT"))

        ZipContent(ZipPath, ContentPath)

        Dim Dialog = New SaveFileDialog With {
            .FileName = Path.GetFileName(ZipPath),
            .Filter = "Zip Archive (*.zip)|*.zip"
        }
        If Dialog.ShowDialog = DialogResult.OK Then
            File.Copy(ZipPath, Dialog.FileName, True)
        End If

        Directory.Delete(TempPath, True)
    End Sub

    Public Sub GenerateDebugScripts(DataPath As String, Disk As DiskImage.Disk, Modifications As Hashtable)
        Dim ScriptModifications As New Hashtable
        Dim SectorDict As New Dictionary(Of UInteger, List(Of DebugSegment))
        Dim SegmentList As List(Of DebugSegment)
        Dim ByteLenPerLine As Byte = 16

        Dim DriveAFile As String = Path.Combine(DataPath, "DRIVE0.TXT")
        Dim DriveBFile As String = Path.Combine(DataPath, "DRIVE1.TXT")

        Dim SB_A = New StringBuilder()
        Dim SB_B = New StringBuilder()

        For Each Offset As UInteger In Modifications.Keys
            ScriptModifications.Item(Offset) = Modifications.Item(Offset)
        Next
        'For Each Offset As UInteger In Disk.Modifications.Keys
        'ScriptModifications.Item(Offset) = Disk.Modifications.Item(Offset)
        ' Next

        Dim OffsetArray(ScriptModifications.Keys.Count - 1) As UInteger
        ScriptModifications.Keys.CopyTo(OffsetArray, 0)
        Array.Sort(OffsetArray)

        For Each Offset As UInteger In OffsetArray
            Dim Sector As UInteger = DiskImage.OffsetToSector(Offset)

            If SectorDict.ContainsKey(Sector) Then
                SegmentList = SectorDict.Item(Sector)
            Else
                SegmentList = New List(Of DebugSegment)
                SectorDict.Add(Sector, SegmentList)
            End If

            Dim SectorOffset As UInteger = DiskImage.SectorToBytes(Sector)
            Dim Value = ScriptModifications.Item(Offset)
            SegmentList.Add(GetSegment(Offset - SectorOffset, Value))
        Next

        For Each Sector In SectorDict.Keys
            SB_A.AppendLine(DebugLoad(0, 0, Sector, 1))
            SB_B.AppendLine(DebugLoad(0, 1, Sector, 1))
            SegmentList = SectorDict.Item(Sector)
            For Each Segment In SegmentList
                For I = 0 To Math.Ceiling(Segment.Data.Length / ByteLenPerLine) - 1
                    Dim SourceIndex As Integer = I * ByteLenPerLine
                    Dim SegmentOffset As Integer = Segment.Offset + SourceIndex
                    Dim Length As Integer = Math.Min(ByteLenPerLine, Segment.Data.Length - SourceIndex)
                    Dim Block(Length - 1) As Byte
                    Array.Copy(Segment.Data, SourceIndex, Block, 0, Length)
                    SB_A.AppendLine(DebugEnter(SegmentOffset, Block))
                    SB_B.AppendLine(DebugEnter(SegmentOffset, Block))
                Next
            Next
            SB_A.AppendLine(DebugWrite(0, 0, Sector, 1))
            SB_B.AppendLine(DebugWrite(0, 1, Sector, 1))
        Next
        SB_A.AppendLine("Q")
        SB_B.AppendLine("Q")

        File.WriteAllText(DriveAFile, SB_A.ToString)
        File.WriteAllText(DriveBFile, SB_B.ToString)
    End Sub

    Public Sub GenerateDirectoryDump(DataPath As String, Disk As DiskImage.Disk, CachedRootDir() As Byte)
        Dim FileName As String

        Dim SectorStart = Disk.BootSector.RootDirectoryRegionStart
        Dim SectorEnd = Disk.BootSector.DataRegionStart
        Dim Length = DiskImage.SectorToBytes(SectorEnd - SectorStart)
        Dim Offset As UInteger = 256

        For Index = 0 To 1
            FileName = Path.Combine(DataPath, "DIRDUMP" & Index & ".TXT")
            Dim SB = New StringBuilder()

            SB.AppendLine("N ROOTDIR.TMP")
            SB.AppendLine(DebugLoad(Offset, Index, SectorStart, SectorEnd - SectorStart))
            SB.AppendLine("RCX")
            SB.AppendLine(Length.ToString("X4"))
            SB.AppendLine("RBX")
            SB.AppendLine("0")
            SB.AppendLine("W " & Offset.ToString("X4"))
            SB.AppendLine("Q")

            File.WriteAllText(FileName, SB.ToString)
        Next Index

        FileName = Path.Combine(DataPath, "ROOTDIR.CRC")
        File.WriteAllText(FileName, "ROOTDIR.TMP " & Crc32.ComputeChecksum(Disk.Directory.GetContent()).ToString("X8"))
    End Sub

    Private Function ConvertByteToByteArray(Value As Byte) As Byte()
        Dim Data(0) As Byte
        Data(0) = Value
        Return Data
    End Function

    Private Function ConvertUintegerToByteArray(Value As UInteger) As Byte()
        Return BitConverter.GetBytes(Value)
    End Function

    Private Function ConvertUShortToByteArray(Value As UShort) As Byte()
        Return BitConverter.GetBytes(Value)
    End Function

    Private Function DebugDump(Address As UInteger, Length As UInteger) As String
        Dim Response As String

        Response = "D " & Address.ToString("X4") & " " & (Address + Length - 1).ToString("X4")

        Return Response
    End Function

    Private Function DebugEnter(Address As UInteger, Data() As Byte) As String
        Dim Response As String

        Response = "E " & Address.ToString("X4") & " " & BitConverter.ToString(Data).Replace("-", " ")

        Return Response
    End Function

    Private Function DebugLoad(Address As UInteger, Drive As Byte, FirstSector As UInteger, Number As UShort) As String
        Dim Response As String

        Response = "L " & Address.ToString("X4") & " " & Drive & " " & FirstSector.ToString("X") & " " & Number.ToString("X")

        Return Response
    End Function

    Private Function DebugWrite(Address As UInteger, Drive As Byte, FirstSector As UInteger, Number As UShort) As String
        Dim Response As String

        Response = "W " & Address.ToString("X4") & " " & Drive & " " & FirstSector.ToString("X") & " " & Number.ToString("X")

        Return Response
    End Function

    Private Function GetSegment(Offset As UInteger, Value As Object) As DebugSegment
        Dim Segment As DebugSegment

        Segment.Offset = Offset
        If TypeOf Value Is System.UInt16 Then
            Segment.Data = ConvertUShortToByteArray(Value)
        ElseIf TypeOf Value Is System.UInt32 Then
            Segment.Data = ConvertUintegerToByteArray(Value)
        ElseIf TypeOf Value Is System.Byte Then
            Segment.Data = ConvertByteToByteArray(Value)
        Else
            Segment.Data = Value
        End If

        Return Segment
    End Function

    Private Sub WriteResourceToFile(ResourceName As String, FileName As String)
        Dim AppName = System.Diagnostics.Process.GetCurrentProcess().ProcessName
        Using Resource = Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(AppName & "." & ResourceName)
            Using File = New FileStream(FileName, FileMode.Create, FileAccess.Write)
                Resource.CopyTo(File)
            End Using
        End Using
    End Sub

    Private Sub ZipContent(ZipPath As String, ContentPath As String)
        Using ZipStream As Stream = New FileStream(ZipPath, FileMode.Create, FileAccess.Write)
            Using Archive As New ZipArchive(ZipStream, ZipArchiveMode.Create)
                For Each FilePath In Directory.GetFiles(ContentPath, "*.*", SearchOption.AllDirectories)
                    Dim RelativePath = FilePath.Replace(ContentPath & "\", String.Empty)
                    Using FileStream As Stream = New FileStream(FilePath, FileMode.Open, FileAccess.Read)
                        Using FileStreamInZip As Stream = Archive.CreateEntry(RelativePath).Open()
                            FileStream.CopyTo(FileStreamInZip)
                        End Using
                    End Using
                Next
            End Using
        End Using
    End Sub

    Public Structure DebugSegment
        Dim Data() As Byte
        Dim Offset As UInteger
    End Structure
End Module