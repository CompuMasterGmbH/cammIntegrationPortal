Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' Default Tag-Prefix for the designer when dragging&dropping controls into a page
<Assembly: System.Web.UI.TagPrefix("CompuMaster.camm.WebManager.Controls.Administration", "cammWM")> 

<Assembly: AssemblyTitle("camm Web-Manager Administration Area")> 
<Assembly: AssemblyDescription("")> 
<Assembly: AssemblyCompany("CompuMaster GmbH")> 
<Assembly: AssemblyProduct("camm Web-Manager")>
<Assembly: AssemblyCopyright("2001-2016 CompuMaster GmbH")>
<Assembly: AssemblyTrademark("")>
<Assembly: CLSCompliant(True)> 

'Die folgende GUID ist für die ID der Typbibliothek, wenn dieses Projekt in COM angezeigt wird
<Assembly: Guid("E86F0C45-44AC-469D-874B-23D1B00864E9")> 

<Assembly: AssemblyDelaySign(False)> 
#If SIGNASSEMBLY Then
<Assembly: AssemblyKeyFile("R:\Shared\Technik\Company Keys\camm.biz.snk")> 
#End If
<Assembly: System.Security.AllowPartiallyTrustedCallers()> 