Namespace ImageFormats
    Namespace HFE
        Public Module Enums
            Public Enum HFEFloppyinterfaceMode As Byte
                IBMPC_DD_FLOPPYMODE = &H0
                IBMPC_HD_FLOPPYMODE = &H1
                ATARIST_DD_FLOPPYMODE = &H2
                ATARIST_HD_FLOPPYMODE = &H3
                AMIGA_DD_FLOPPYMODE = &H4
                AMIGA_HD_FLOPPYMODE = &H5
                CPC_DD_FLOPPYMODE = &H6
                GENERIC_SHUGGART_DD_FLOPPYMODE = &H7
                IBMPC_ED_FLOPPYMODE = &H8
                MSX2_DD_FLOPPYMODE = &H9
                C64_DD_FLOPPYMODE = &HA
                EMU_SHUGART_FLOPPYMODE = &HB
                S950_DD_FLOPPYMODE = &HC
                S950_HD_FLOPPYMODE = &HD
                DISABLE_FLOPPYMODE = &HFE
            End Enum

            Public Enum HFETrackEncoding As Byte
                ISOIBM_MFM_ENCODING = &H0
                AMIGA_MFM_ENCODING = &H1
                ISOIBM_FM_ENCODING = &H2
                EMU_FM_ENCODING = &H3
                UNKNOWN_ENCODING = &HFF
            End Enum
        End Module
    End Namespace
End Namespace
