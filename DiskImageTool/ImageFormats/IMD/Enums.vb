Namespace ImageFormats.IMD
    Public Enum TrackMode As Byte
        FM500kbps = 0
        FM300kbps = 1
        FM250kbps = 2
        MFM500kbps = 3
        MFM300kbps = 4
        MFM250kbps = 5
    End Enum

    Public Enum SectorSize As Byte
        SectorSize128 = 0
        SectorSize256 = 1
        Sectorsize512 = 2
        SectorSize1024 = 3
        SectorSize2048 = 4
        SectorSize4096 = 5
        SectorSize8192 = 6
    End Enum

    Public Enum DataFormat
        Unavailable = 0
        Normal = 1
        NormalCompressed = 2
        Deleted = 3
        DeletedCompressed = 4
        NormalError = 5
        NormalErrorCompressed = 6
        DeletedError = 7
        DeletedErrorCompressed = 8
    End Enum
End Namespace

