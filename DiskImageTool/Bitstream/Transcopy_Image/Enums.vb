Namespace Transcopy
    Public Enum TransCopyTrackFlags As UInt16
        KeepTrackLength = 1
        CopyAcrossIndex = 2
        CopyWeakBits = 4
        VerifyWrite = 8
        NoAddressMarks = 128
    End Enum

    Public Enum TransCopyDiskType As Byte
        Unknown = &HFF
        MFMHighDensity = &H2
        MFMDoubleDensity360RPM = &H3
        AppleIIGCR = &H4
        FMSengleDensity = &H5
        CommodoreGCR = &H6
        MFMDoubleDensity = &H7
        CommodoreAmiga = &H8
        AtariFM = &HC
    End Enum
End Namespace

