Namespace DiskImage
    Public Class SectorData
        Private ReadOnly _BlockList As List(Of SectorBlock)
        Private ReadOnly _FloppyImage As IFloppyImage
        Private ReadOnly _SectorData As IFloppyImage
        Private ReadOnly _SectorList As Dictionary(Of UInteger, SectorBlock)

        Sub New(FloppyImage As IFloppyImage)
            _BlockList = New List(Of SectorBlock)
            _SectorData = New BasicSectorImage({})
            _FloppyImage = FloppyImage
            _SectorList = New Dictionary(Of UInteger, SectorBlock)
        End Sub

        Public ReadOnly Property BlockCount As Integer
            Get
                Return _BlockList.Count
            End Get
        End Property

        Public ReadOnly Property SectorData As IFloppyImage
            Get
                Return _SectorData
            End Get
        End Property

        Public ReadOnly Property SectorList As List(Of UInteger)
            Get
                Return _SectorList.Keys.ToList
            End Get
        End Property

        Public Sub AddBlock(SectorStart As UInteger, SectorCount As UInteger)
            AddBlock(SectorStart, SectorCount, "")
        End Sub

        Public Sub AddBlock(SectorStart As UInteger, SectorCount As UInteger, Description As String)
            Dim Offset As UInteger = _SectorData.Length
            Dim SourceOffset = Disk.SectorToBytes(SectorStart)
            Dim Size As UInteger = Disk.SectorToBytes(SectorCount)

            If SourceOffset + Size > _FloppyImage.Length Then
                Size = Math.Max(0, _FloppyImage.Length - SourceOffset)
            End If

            If Size > 0 Then
                _SectorData.Append(_FloppyImage.GetBytes(SourceOffset, Size))

                _BlockList.Add(New SectorBlock(_BlockList.Count, SectorStart, SectorCount, Offset, Size, Description))

                Do While Size > 0
                    Dim SectorSize = Math.Min(Size, Disk.BYTES_PER_SECTOR)
                    _SectorList.Add(SectorStart, New SectorBlock(_SectorList.Count, SectorStart, 1, Offset, SectorSize, ""))
                    Offset += SectorSize
                    Size -= SectorSize
                    SectorStart += 1
                Loop
            End If
        End Sub

        Public Sub AddBlockByOffset(Offset As UInteger, Length As UInteger)
            AddBlockByOffset(Offset, Length, "")
        End Sub

        Public Sub AddBlockByOffset(Offset As UInteger, Length As UInteger, Description As String)
            Dim SectorStart = Disk.OffsetToSector(Offset)
            Dim SectorCount = Disk.BytesToSector(Length)

            AddBlock(SectorStart, SectorCount, Description)
        End Sub

        Public Sub AddBlocksByChain(SectorChain As List(Of UInteger))
            Dim SectorStart As UInteger
            Dim LastSector As UInteger = 0
            Dim StartLoop As Boolean = True
            For Each Sector In SectorChain
                If StartLoop Then
                    SectorStart = Sector
                ElseIf Sector <> LastSector + 1 Then
                    AddBlock(SectorStart, LastSector - SectorStart + 1)
                    SectorStart = Sector
                End If
                LastSector = Sector
                StartLoop = False
            Next
            AddBlock(SectorStart, LastSector - SectorStart + 1)
        End Sub

        Public Sub CommitChanges()
            Dim UseBatchEditMode = Not _FloppyImage.History.BatchEditMode

            If UseBatchEditMode Then
                _FloppyImage.History.BatchEditMode = True
            End If

            For Each SectorBlock In _SectorList.Values
                Dim Offset = Disk.SectorToBytes(SectorBlock.SectorStart)
                Dim OriginalData = _FloppyImage.GetBytes(Offset, SectorBlock.Size)
                Dim NewData = _SectorData.GetBytes(SectorBlock.Offset, SectorBlock.Size)
                If Not OriginalData.CompareTo(NewData) Then
                    _FloppyImage.SetBytes(NewData, Offset)
                End If
            Next

            If UseBatchEditMode Then
                _FloppyImage.History.BatchEditMode = False
            End If
        End Sub

        Public Function GetBlock(Index As Integer) As SectorBlock
            Return _BlockList(Index)
        End Function

        Public Function GetBlockData(Index As Integer) As Byte()
            Dim SectorBlock = _BlockList(Index)

            Return _SectorData.GetBytes(SectorBlock.Offset, SectorBlock.Size)
        End Function

        Public Function GetSectorData(Sector As UInteger) As Byte()
            Dim SectorBlock = _SectorList(Sector)

            Return _SectorData.GetBytes(SectorBlock.Offset, SectorBlock.Size)
        End Function
    End Class
End Namespace
