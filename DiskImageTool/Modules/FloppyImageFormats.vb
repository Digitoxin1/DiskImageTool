Imports DiskImageTool.DiskImage

Public NotInheritable Class FloppyImageFormats

    Public Const IMZ_EXTENSION As String = ".imz"

    Private Shared ReadOnly _Formats As New Dictionary(Of FloppyImageType, FloppyImageFormatInfo) From {
        {FloppyImageType.BasicSectorImage, New FloppyImageFormatInfo(
            FloppyImageType.BasicSectorImage,
            ".ima",
            Function() My.Resources.FloppyImageType_BasicSectorImage)},
        {FloppyImageType.D86FImage, New FloppyImageFormatInfo(
            FloppyImageType.D86FImage,
            ".86f",
            Function() String.Format(My.Resources.FloppyImageType_Image, My.Resources.FloppyImageType_86F))},
        {FloppyImageType.HFEImage, New FloppyImageFormatInfo(
            FloppyImageType.HFEImage,
            ".hfe",
            Function() String.Format(My.Resources.FloppyImageType_Image, My.Resources.FloppyImageType_HFE))},
        {FloppyImageType.IMDImage, New FloppyImageFormatInfo(
            FloppyImageType.IMDImage,
            ".imd",
            Function() String.Format(My.Resources.FloppyImageType_SectorImage, My.Resources.FloppyImageType_IMD))},
        {FloppyImageType.MFMImage, New FloppyImageFormatInfo(
            FloppyImageType.MFMImage,
            ".mfm",
            Function() String.Format(My.Resources.FloppyImageType_Image, My.Resources.FloppyImageType_MFM))},
        {FloppyImageType.PRIImage, New FloppyImageFormatInfo(
            FloppyImageType.PRIImage,
            ".pri",
            Function() String.Format(My.Resources.FloppyImageType_BitstreamImage, My.Resources.FloppyImageType_PCE))},
        {FloppyImageType.PSIImage, New FloppyImageFormatInfo(
            FloppyImageType.PSIImage,
            ".psi",
            Function() String.Format(My.Resources.FloppyImageType_SectorImage, My.Resources.FloppyImageType_PCE))},
        {FloppyImageType.TD0Image, New FloppyImageFormatInfo(
            FloppyImageType.TD0Image,
            ".td0",
            Function() String.Format(My.Resources.FloppyImageType_SectorImage, My.Resources.FloppyImageType_TD0))},
        {FloppyImageType.TranscopyImage, New FloppyImageFormatInfo(
            FloppyImageType.TranscopyImage,
            ".tc",
            Function() String.Format(My.Resources.FloppyImageType_Image, My.Resources.FloppyImageType_TC))}
    }

    Private Shared ReadOnly _SectorAliases As SectorAlias() = {
        New SectorAlias(".img", Function() My.Resources.FloppyImageType_BasicSectorImage, True, True, False),
        New SectorAlias(IMZ_EXTENSION, Function() My.Resources.FileType_IMZ, False, False, False),
        New SectorAlias(".vfd", Function() My.Resources.FileType_VFD, True, False, True),
        New SectorAlias(".flp", Function() My.Resources.FileType_VFD, True, False, True),
        New SectorAlias(".dsk", Function() My.Resources.FloppyImageType_BasicSectorImage, True, True, False)
    }

    Private Shared ReadOnly _ByExtension As Dictionary(Of String, FloppyImageFormatInfo) = _Formats.Values.ToDictionary(Function(f) f.Extension, Function(f) f, StringComparer.OrdinalIgnoreCase)

    Public Shared ReadOnly AdvancedSectorFileExtensions As New List(Of String) From {
        _Formats.Item(FloppyImageType.IMDImage).Extension,
        _Formats.Item(FloppyImageType.PSIImage).Extension,
        _Formats.Item(FloppyImageType.TD0Image).Extension}

    Public Shared ReadOnly BitstreamFileExtensions As New List(Of String) From {
        _Formats.Item(FloppyImageType.D86FImage).Extension,
        _Formats.Item(FloppyImageType.HFEImage).Extension,
        _Formats.Item(FloppyImageType.MFMImage).Extension,
        _Formats.Item(FloppyImageType.PRIImage).Extension,
        _Formats.Item(FloppyImageType.TranscopyImage).Extension}

    Public Shared ReadOnly AllFileExtensions As New List(Of String)
    Public Shared ReadOnly BasicSectorFileExtensions As New List(Of String)
    Public Shared ReadOnly BasicSectorFileExtensionsFlux As New List(Of String)
    Public Shared ReadOnly BasicSectorFileExtensionsSave As New List(Of String)
    Public Shared ReadOnly VFDFileExtensions As New List(Of String)

    Shared Sub New()
        InitExtensions()
    End Sub

    Public Shared Function GetInfo(ImageType As FloppyImageType) As FloppyImageFormatInfo
        Dim Info As FloppyImageFormatInfo = Nothing

        If _Formats.TryGetValue(ImageType, Info) Then
            Return Info
        End If

        Return _Formats.Item(FloppyImageType.BasicSectorImage)
    End Function

    Public Shared Function GetInfoByExtension(Extension As String) As FloppyImageFormatInfo
        Dim Info As FloppyImageFormatInfo = Nothing

        If _ByExtension.TryGetValue(Extension, Info) Then
            Return Info
        End If

        Return Nothing
    End Function

    Private Shared Sub AddBasicSectorExtension(Extension As String, Name As Func(Of String))
        If Not _ByExtension.ContainsKey(Extension) Then
            _ByExtension.Add(Extension, New FloppyImageFormatInfo(
                FloppyImageType.BasicSectorImage,
                Extension,
                Name))
        End If
    End Sub

    Private Shared Sub AddBasicSectorExtension(Extension As String)
        AddBasicSectorExtension(Extension, Function() My.Resources.FloppyImageType_BasicSectorImage)
    End Sub

    Private Shared Sub InitExtensions()
        BasicSectorFileExtensions.Add(_Formats.Item(FloppyImageType.BasicSectorImage).Extension)
        BasicSectorFileExtensionsFlux.Add(_Formats.Item(FloppyImageType.BasicSectorImage).Extension)
        BasicSectorFileExtensionsSave.Add(_Formats.Item(FloppyImageType.BasicSectorImage).Extension)

        For Each SectorAlias In _SectorAliases
            BasicSectorFileExtensions.Add(SectorAlias.Extension)
            If SectorAlias.InFlux Then
                BasicSectorFileExtensionsFlux.Add(SectorAlias.Extension)
            End If
            If SectorAlias.InSave Then
                BasicSectorFileExtensionsSave.Add(SectorAlias.Extension)
            End If
            If SectorAlias.InVfd Then
                VFDFileExtensions.Add(SectorAlias.Extension)
            End If

            AddBasicSectorExtension(SectorAlias.Extension, SectorAlias.Name)
        Next

        Dim Items = System.Enum.GetValues(GetType(FloppyDiskFormat))
        For Each Item As Integer In Items
            Dim FileExt = FloppyDiskFormatGetParams(Item).FileExtension
            If FileExt <> "" Then
                If Not BasicSectorFileExtensions.Contains(FileExt) Then
                    BasicSectorFileExtensions.Add(FileExt)
                    AddBasicSectorExtension(FileExt)
                End If
            End If
        Next

        For Each Item In FloppyImageFormats.BasicSectorFileExtensions
            If Not AllFileExtensions.Contains(Item) Then
                AllFileExtensions.Add(Item)
            End If
        Next

        For Each Item In FloppyImageFormats.AdvancedSectorFileExtensions
            If Not AllFileExtensions.Contains(Item) Then
                AllFileExtensions.Add(Item)
            End If
        Next

        For Each Item In FloppyImageFormats.BitstreamFileExtensions
            If Not AllFileExtensions.Contains(Item) Then
                AllFileExtensions.Add(Item)
            End If
        Next
    End Sub

    Private NotInheritable Class SectorAlias
        Public ReadOnly Property Extension As String
        Public ReadOnly Property Name As Func(Of String)
        Public ReadOnly Property InFlux As Boolean
        Public ReadOnly Property InSave As Boolean
        Public ReadOnly Property InVfd As Boolean
        Public Sub New(extension As String, name As Func(Of String), inFlux As Boolean, inSave As Boolean, inVfd As Boolean)
            Me.Extension = extension
            Me.Name = name
            Me.InFlux = inFlux
            Me.InSave = inSave
            Me.InVfd = inVfd
        End Sub
    End Class
End Class

Public Class FloppyImageFormatInfo
    Public ReadOnly Property ImageType As FloppyImageType
    Public ReadOnly Property Extension As String

    Private ReadOnly _NameProvider As Func(Of String)

    Public Sub New(ImageType As FloppyImageType, Extension As String, NameProvider As Func(Of String))
        Me.ImageType = ImageType
        Me.Extension = Extension
        _NameProvider = NameProvider
    End Sub

    Public ReadOnly Property Name As String
        Get
            Return _NameProvider()
        End Get
    End Property
End Class
