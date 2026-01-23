' VistaFolderPicker.vb
Option Strict On
Option Explicit On

Imports System.Runtime.InteropServices

Public Module VistaFolderPicker

    ''' <summary>
    ''' Shows the Vista+ folder picker (IFileDialog with FOS_PICKFOLDERS).
    ''' Returns "" on cancel or failure.
    ''' </summary>
    Public Function BrowseFolderVista(Optional initialPath As String = "", Optional ownerHwnd As IntPtr = Nothing) As String
        Dim dialog As IFileDialog = Nothing
        Dim initItem As IShellItem = Nothing
        Dim resultItem As IShellItem = Nothing

        Try
            dialog = CType(New FileOpenDialog(), IFileDialog)

            ' Options
            Dim options As FOS
            dialog.GetOptions(options)
            options = options Or FOS.FOS_PICKFOLDERS Or FOS.FOS_FORCEFILESYSTEM Or FOS.FOS_PATHMUSTEXIST
            dialog.SetOptions(options)

            ' Set initial folder (best-effort)
            If Not String.IsNullOrWhiteSpace(initialPath) Then
                Dim full = IO.Path.GetFullPath(initialPath)
                If IO.Directory.Exists(full) Then
                    Dim hr = SHCreateItemFromParsingName(full, IntPtr.Zero, GetType(IShellItem).GUID, initItem)
                    If hr = 0 AndAlso initItem IsNot Nothing Then
                        dialog.SetFolder(initItem)
                    End If
                End If
            End If

            ' Show dialog
            Dim showHr As Integer = dialog.Show(ownerHwnd)

            ' Cancel returns HRESULT 0x800704C7
            If showHr <> 0 Then
                Return ""
            End If

            ' Get result
            dialog.GetResult(resultItem)
            If resultItem Is Nothing Then Return ""

            Dim pszPath As IntPtr = IntPtr.Zero
            resultItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, pszPath)

            If pszPath = IntPtr.Zero Then Return ""

            Dim path As String = Marshal.PtrToStringUni(pszPath)
            Marshal.FreeCoTaskMem(pszPath)

            Return If(path, "")
        Catch
            ' Any COM/interop issue -> fail soft
            Return ""
        Finally
            If resultItem IsNot Nothing Then
                Marshal.FinalReleaseComObject(resultItem)
            End If
            If initItem IsNot Nothing Then
                Marshal.FinalReleaseComObject(initItem)
            End If
            If dialog IsNot Nothing Then
                Marshal.FinalReleaseComObject(dialog)
            End If
        End Try
    End Function

#Region "Interop"

    <ComImport, Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7")>
    Private Class FileOpenDialog
    End Class

    <Flags>
    Private Enum FOS As UInteger
        FOS_OVERWRITEPROMPT = &H2UI
        FOS_STRICTFILETYPES = &H4UI
        FOS_NOCHANGEDIR = &H8UI
        FOS_PICKFOLDERS = &H20UI
        FOS_FORCEFILESYSTEM = &H40UI
        FOS_ALLNONSTORAGEITEMS = &H80UI
        FOS_NOVALIDATE = &H100UI
        FOS_ALLOWMULTISELECT = &H200UI
        FOS_PATHMUSTEXIST = &H800UI
        FOS_FILEMUSTEXIST = &H1000UI
        FOS_CREATEPROMPT = &H2000UI
        FOS_SHAREAWARE = &H4000UI
        FOS_NOREADONLYRETURN = &H8000UI
        FOS_NOTESTFILECREATE = &H10000UI
        FOS_HIDEMRUPLACES = &H20000UI
        FOS_HIDEPINNEDPLACES = &H40000UI
        FOS_NODEREFERENCELINKS = &H100000UI
        FOS_OKBUTTONNEEDSINTERACTION = &H200000UI
        FOS_DONTADDTORECENT = &H2000000UI
        FOS_FORCESHOWHIDDEN = &H10000000UI
        FOS_DEFAULTNOMINIMODE = &H20000000UI
        FOS_FORCEPREVIEWPANEON = &H40000000UI
        FOS_SUPPORTSTREAMABLEITEMS = &H80000000UI
    End Enum

    Private Enum SIGDN As UInteger
        SIGDN_NORMALDISPLAY = 0UI
        SIGDN_FILESYSPATH = &H80058000UI
    End Enum

    <ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("42F85136-DB7E-439C-85F1-E4075D135FC8")>
    Private Interface IFileDialog
        ' IModalWindow
        <PreserveSig>
        Function Show(<[In]> hwndOwner As IntPtr) As Integer

        ' IFileDialog
        Sub SetFileTypes(ByVal cFileTypes As UInteger, ByVal rgFilterSpec As IntPtr)
        Sub SetFileTypeIndex(ByVal iFileType As UInteger)
        Sub GetFileTypeIndex(ByRef piFileType As UInteger)
        Sub Advise(ByVal pfde As IntPtr, ByRef pdwCookie As UInteger)
        Sub Unadvise(ByVal dwCookie As UInteger)
        Sub SetOptions(ByVal fos As FOS)
        Sub GetOptions(ByRef pfos As FOS)
        Sub SetDefaultFolder(ByVal psi As IShellItem)
        Sub SetFolder(ByVal psi As IShellItem)
        Sub GetFolder(ByRef ppsi As IShellItem)
        Sub GetCurrentSelection(ByRef ppsi As IShellItem)
        Sub SetFileName(<MarshalAs(UnmanagedType.LPWStr)> ByVal pszName As String)
        Sub GetFileName(<MarshalAs(UnmanagedType.LPWStr)> ByRef pszName As String)
        Sub SetTitle(<MarshalAs(UnmanagedType.LPWStr)> ByVal pszTitle As String)
        Sub SetOkButtonLabel(<MarshalAs(UnmanagedType.LPWStr)> ByVal pszText As String)
        Sub SetFileNameLabel(<MarshalAs(UnmanagedType.LPWStr)> ByVal pszLabel As String)
        Sub GetResult(ByRef ppsi As IShellItem)
        Sub AddPlace(ByVal psi As IShellItem, ByVal fdap As UInteger)
        Sub SetDefaultExtension(<MarshalAs(UnmanagedType.LPWStr)> ByVal pszDefaultExtension As String)
        Sub Close(ByVal hr As Integer)
        Sub SetClientGuid(ByRef guid As Guid)
        Sub ClearClientData()
        Sub SetFilter(ByVal pFilter As IntPtr)
    End Interface

    <ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")>
    Private Interface IShellItem
        Sub BindToHandler(ByVal pbc As IntPtr, ByRef bhid As Guid, ByRef riid As Guid, <Out> ByRef ppv As IntPtr)
        Sub GetParent(<Out> ByRef ppsi As IShellItem)
        Sub GetDisplayName(ByVal sigdnName As SIGDN, <Out> ByRef ppszName As IntPtr)
        Sub GetAttributes(ByVal sfgaoMask As UInteger, <Out> ByRef psfgaoAttribs As UInteger)
        Sub Compare(ByVal psi As IShellItem, ByVal hint As UInteger, <Out> ByRef piOrder As Integer)
    End Interface

    <DllImport("shell32.dll", CharSet:=CharSet.Unicode, PreserveSig:=True)>
    Private Function SHCreateItemFromParsingName(
        <MarshalAs(UnmanagedType.LPWStr)> pszPath As String,
        pbc As IntPtr,
        <MarshalAs(UnmanagedType.LPStruct)> riid As Guid,
        <Out> ByRef ppv As IShellItem) As Integer
    End Function

#End Region

End Module
