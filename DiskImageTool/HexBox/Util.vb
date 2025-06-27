Namespace Hb.Windows.Forms
    Friend Module Util
        ''' <summary>
        ''' Contains true, if we are in design mode of Visual Studio
        ''' </summary>
        Private ReadOnly _designMode As Boolean

        ''' <summary>
        ''' Initializes an instance of Util class
        ''' </summary>
        Sub New()
            ' design mode is true if host process is: Visual Studio, Visual Studio Express Versions (C#, VB, C++) or SharpDevelop
            Dim designerHosts = New List(Of String)() From {
                "devenv",
                "vcsexpress",
                "vbexpress",
                "vcexpress",
                "sharpdevelop"
            }
            Using process = Diagnostics.Process.GetCurrentProcess()
                Dim processName = process.ProcessName.ToLower()
                _designMode = designerHosts.Contains(processName)
            End Using
        End Sub

        ''' <summary>
        ''' Gets true, if we are in design mode of Visual Studio
        ''' </summary>
        ''' <remarks>
        ''' In Visual Studio 2008 SP1 the designer is crashing sometimes on windows forms. 
        ''' The DesignMode property of Control class is buggy and cannot be used, so use our own implementation instead.
        ''' </remarks>
        Public ReadOnly Property DesignMode As Boolean
            Get
                Return _designMode
            End Get
        End Property
    End Module
End Namespace
