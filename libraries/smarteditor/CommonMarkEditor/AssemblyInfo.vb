Option Explicit On
Option Strict On

Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' Default Tag-Prefix for the designer when dragging&dropping controls into a page
<Assembly: System.Web.UI.TagPrefix("CompuMaster.camm.WebManager.Controls", "cammWM")>

<Assembly: AssemblyTitle("camm Web-Manager SmartEditor with PlainTextEditor")>
<Assembly: AssemblyDescription("")>
<Assembly: AssemblyCompany("CompuMaster GmbH")>
<Assembly: AssemblyProduct("camm Web-Manager SmartEditor")>
<Assembly: AssemblyCopyright("2001-2016 CompuMaster GmbH")>
<Assembly: AssemblyTrademark("camm")>
<Assembly: CLSCompliant(True)>
<Assembly: ComVisible(False)>
'<Assembly: System.Security.AllowPartiallyTrustedCallers()>

'Die folgende GUID ist für die ID der Typbibliothek, wenn dieses Projekt in COM angezeigt wird
<Assembly: Guid("7748f679-7402-4b91-a29a-0f88b48869e7")>

<Assembly: AssemblyDelaySign(False)>
#If SIGNASSEMBLY Then
<Assembly: AssemblyKeyFile("R:\Shared\Technik\Company Keys\camm.biz.snk")>
#End If