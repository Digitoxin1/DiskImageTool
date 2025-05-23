﻿Namespace ImageFormats
    Namespace PSI
        Public Class IBMSectorHeader
            Private ReadOnly _ChunkData() As Byte

            Public Sub New()
                _ChunkData = New Byte(5) {}
            End Sub

            Public Sub New(ChunkData() As Byte)
                _ChunkData = ChunkData
            End Sub

            Public ReadOnly Property ChunkData As Byte()
                Get
                    Return _ChunkData
                End Get
            End Property

            Public Property Cylinder As Byte
                Get
                    Return _ChunkData(0)
                End Get
                Set(value As Byte)
                    _ChunkData(0) = value
                End Set
            End Property

            Public Property DataFieldCRCError() As Boolean
                Get
                    Return (Flags And MFMSectorFlags.DataFieldCRCError) > 0
                End Get
                Set(value As Boolean)
                    Flags = MyBitConverter.ToggleBit(Flags, MFMSectorFlags.DataFieldCRCError, value)
                End Set
            End Property

            Public Property DeletedDAM() As Boolean
                Get
                    Return (Flags And MFMSectorFlags.DeletedDAM) > 0
                End Get
                Set(value As Boolean)
                    Flags = MyBitConverter.ToggleBit(Flags, MFMSectorFlags.DeletedDAM, value)
                End Set
            End Property

            Public Property EncodingSubType As MFMEncodingSubtype
                Get
                    Return _ChunkData(5)
                End Get
                Set(value As MFMEncodingSubtype)
                    _ChunkData(5) = value
                End Set
            End Property

            Public Property Flags As MFMSectorFlags
                Get
                    Return _ChunkData(4)
                End Get
                Set(value As MFMSectorFlags)
                    _ChunkData(4) = value
                End Set
            End Property

            Public Property Head As Byte
                Get
                    Return _ChunkData(1)
                End Get
                Set(value As Byte)
                    _ChunkData(1) = value
                End Set
            End Property

            Public Property IDFieldCRCError() As Boolean
                Get
                    Return (Flags And MFMSectorFlags.IDFieldCRCError) > 0
                End Get
                Set(value As Boolean)
                    Flags = MyBitConverter.ToggleBit(Flags, MFMSectorFlags.IDFieldCRCError, value)
                End Set
            End Property

            Public Property MissingDAM() As Boolean
                Get
                    Return (Flags And MFMSectorFlags.MissingDAM) > 0
                End Get
                Set(value As Boolean)
                    Flags = MyBitConverter.ToggleBit(Flags, MFMSectorFlags.MissingDAM, value)
                End Set
            End Property

            Public Property Sector As Byte
                Get
                    Return _ChunkData(2)
                End Get
                Set(value As Byte)
                    _ChunkData(2) = value
                End Set
            End Property

            Public Property Size As Byte
                Get
                    Return _ChunkData(3)
                End Get
                Set(value As Byte)
                    _ChunkData(3) = value
                End Set
            End Property
        End Class
    End Namespace
End Namespace
