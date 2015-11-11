Option Explicit On 
Option Strict On

Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' Default Tag-Prefix for the designer when dragging&dropping controls into a page
<Assembly: System.Web.UI.TagPrefix("CompuMaster.camm.WebManager.Controls", "cammWM")>

#If Not Linux Then
<Assembly: AssemblyTitle("camm Web-Manager")>
#Else
<Assembly: AssemblyTitle("camm Web-Manager Mono Edition")> 
#End If
<Assembly: AssemblyDescription("")>
<Assembly: AssemblyCompany("CompuMaster GmbH")>
<Assembly: AssemblyProduct("camm Web-Manager")>
<Assembly: AssemblyCopyright("2001-2015 CompuMaster GmbH")>
<Assembly: AssemblyTrademark("camm")>
<Assembly: CLSCompliant(True)>
<Assembly: ComVisibleAttribute(True)>
<Assembly: System.Security.AllowPartiallyTrustedCallers()>

'Die folgende GUID ist für die ID der Typbibliothek, wenn dieses Projekt in COM angezeigt wird
<Assembly: Guid("077F0C95-42AC-4A9D-884B-03D1E00564E6")>

<Assembly: AssemblyDelaySign(False)>
#If SIGNASSEMBLY Then
<Assembly: AssemblyKeyFile("R:\Shared\Technik\Company Keys\camm.biz.snk")>
#End If