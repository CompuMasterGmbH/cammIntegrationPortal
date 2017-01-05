Option Explicit On 
Option Strict On

Imports System.Reflection
Imports System.Runtime.InteropServices

' Default Tag-Prefix for the designer when dragging&dropping controls into a page
<Assembly: System.Web.UI.TagPrefix("CompuMaster.camm.SmartWebEditor.Controls", "cammWebEdit")>

<Assembly: AssemblyTitle("camm SmartEditor PlainText")>
<Assembly: AssemblyDescription("Provides the camm SmartEditor for plain text")>
<Assembly: AssemblyCompany("CompuMaster GmbH")>
<Assembly: AssemblyProduct("camm Integration Portal (base on camm Web-Manager) SmartEditor")>
<Assembly: AssemblyCopyright("2001-2016 CompuMaster GmbH")>
<Assembly: AssemblyTrademark("camm")>
<Assembly: CLSCompliant(True)>
<Assembly: ComVisible(False)>
<Assembly: System.Security.AllowPartiallyTrustedCallers()>

'Die folgende GUID ist für die ID der Typbibliothek, wenn dieses Projekt in COM angezeigt wird
<Assembly: Guid("3924AF70-E56D-4BA2-BF3B-D7A98979B368")>

<Assembly: AssemblyDelaySign(False)>
#If SIGNASSEMBLY Then
<Assembly: AssemblyKeyFile("R:\Shared\Technik\Company Keys\camm.biz.snk")>
#End If