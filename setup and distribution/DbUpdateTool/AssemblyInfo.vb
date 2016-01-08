Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

<Assembly: AssemblyTitle("camm Web-Manager Database Setup Tool")>
<Assembly: AssemblyDescription("")>
<Assembly: AssemblyCompany("CompuMaster")>
<Assembly: AssemblyProduct("camm Web-Manager")>
<Assembly: AssemblyCopyright("2003-2016 CompuMaster GmbH")>
<Assembly: AssemblyTrademark("")>
<Assembly: CLSCompliant(False)>

<Assembly: Guid("efd61116-7bd9-4d98-b42c-e03177573980")>

<Assembly: AssemblyVersion("4.10.206.1001")>
#If UseLocalSQLs = True Then
Namespace Setup
    ''' <summary>
    ''' Required class because code security in ASP.NET 2 environments denys the access to the file version API of windows and the GetAssemblyFileVersion method would fail with a SecurityException
    ''' </summary>
    Friend Class AssemblyVersion
        Public Const Version As String = "4.10.206.1001"
    End Class
End Namespace
#End If
