Option Explicit On 
Option Strict On

Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' Default Tag-Prefix for the designer when dragging&dropping controls into a page
<Assembly: System.Web.UI.TagPrefix("CompuMaster.camm.WebManager.Controls", "camm")>

#If Not Linux Then
<Assembly: AssemblyTitle("camm Integration Portal")>
#Else
<Assembly: AssemblyTitle("camm Integration Portal Mono Edition")> 
#End If
<Assembly: AssemblyDescription("camm Integration Portal Core Library")>
<Assembly: AssemblyCompany("CompuMaster GmbH")>
<Assembly: AssemblyProduct("camm Integration Portal (based on camm Web-Manager)")>
<Assembly: AssemblyCopyright("2001-2016 CompuMaster GmbH")>
<Assembly: AssemblyTrademark("camm")>

<Assembly: CLSCompliant(True)>
<Assembly: ComVisibleAttribute(False)>
<Assembly: System.Security.AllowPartiallyTrustedCallers()>
<Assembly: Guid("077F0C95-42AC-4A9D-884B-03D1E00564E6")>

<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ConsoleApplication2")>
<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo("cammWM.Test")>
