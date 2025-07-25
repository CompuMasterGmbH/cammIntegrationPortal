﻿Option Explicit On 
Option Strict On

Imports System.Reflection

<Assembly: AssemblyVersion("4.12.2117.150")>
<Assembly: AssemblyInformationalVersion("4.12.2117.150")>

Namespace CompuMaster.camm.WebManager.Setup
    ''' <summary>
    ''' Required class because code security in ASP.NET 2 environments denys the access to the file version API of windows and the GetAssemblyFileVersion method would fail with a SecurityException
    ''' </summary>
    Friend Class AssemblyVersion
        Public Const Version As String = "4.12.2117.150"
    End Class

End Namespace