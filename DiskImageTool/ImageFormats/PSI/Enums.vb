Namespace ImageFormats
    Namespace PSI
        Public Enum DefaultSectorFormat As UInt16
            IBM_FM = 256
            IBM_MFM_DD = 512
            IBM_MFM_HD = 513
            IBM_MFM_ED = 514
            MAG_GCR = 768
        End Enum

        Public Enum SectorFlags As Byte
            Compressed = 1
            AlternateSector = 2
            DataCRCError = 4
        End Enum

        Public Enum MFMSectorFlags As Byte
            IDFieldCRCError = 1
            DataFieldCRCError = 2
            DeletedDAM = 4
            MissingDAM = 8
        End Enum

        Public Enum GCRSectorFlags As Byte
            IDFieldChecksumError = 1
            DataFieldCChecksumError = 2
            MissingDataMark = 4
        End Enum

        Public Enum MFMEncodingSubtype As Byte
            DoubleDensity = 0
            HighDensity = 1
            ExtraDensity = 2
        End Enum

        Public Structure ReadResponse
            Dim Offset As UInt32
            Dim ChecksumVerified As Boolean
        End Structure
    End Namespace
End Namespace