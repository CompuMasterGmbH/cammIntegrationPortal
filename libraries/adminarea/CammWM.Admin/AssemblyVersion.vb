Imports System.Reflection

<Assembly: AssemblyVersion("4.10.202.117")>

Namespace CompuMaster.camm.WebManager.Administration
    ''' <summary>
    ''' Required class because code security in ASP.NET 2 environments deny the access to the file version API of windows and the GetAssemblyFileVersion method would fail with a SecurityException
    ''' </summary>
    Friend Class AssemblyVersion
        Public Const Version As String = "4.10.202.117"
    End Class
End Namespace