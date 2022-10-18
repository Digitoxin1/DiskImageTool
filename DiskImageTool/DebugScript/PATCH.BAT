@echo off
echo Which drive contains the disk to be patched?
TOOLS\CHOICE /C:AB
IF errorlevel 2 GOTO DRIVE_B
IF errorlevel 1 GOTO DRIVE_A
GOTO EXIT

:DRIVE_A
ECHO Reading Root Directory on Drive A
ECHO.
DEBUG < DATA\DIRDUMP0.TXT > nul
TOOLS\CRC32 < DATA\ROOTDIR.CRC > nul
if errorlevel 1 GOTO WRONGA
ECHO Patching Disk in Drive A
ECHO.
DEBUG < DATA\DRIVE0.TXT > nul
ECHO Patch applied successfully
ECHO.
GOTO EXIT

:DRIVE_B
ECHO Reading Root Directory on Drive B
ECHO.
DEBUG < DATA\DIRDUMP1.TXT > nul
TOOLS\CRC32 < DATA\ROOTDIR.CRC > nul
IF errorlevel 1 GOTO WRONGB
ECHO Patching Disk in Drive B
ECHO.
DEBUG < DATA\DRIVE1.TXT > nul
ECHO Pach applied successfully
ECHO.
GOTO EXIT

:WRONGA
ECHO Error: The wrong disk appears to be in Drive A
ECHO.
GOTO EXIT

:WRONGB
ECHO Error: The wrong disk appears to be in Drive B
ECHO.
GOTO EXIT

:EXIT
IF EXIST ROOTDIR.TMP DEL ROOTDIR.TMP
