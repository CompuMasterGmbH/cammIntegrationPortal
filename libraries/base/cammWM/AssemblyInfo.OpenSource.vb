Option Explicit On 
Option Strict On

Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' Default Tag-Prefix for the designer when dragging&dropping controls into a page
<Assembly: System.Web.UI.TagPrefix("CompuMaster.camm.WebManager.Controls", "camm")>

#If Not Linux Then
<Assembly: AssemblyTitle("camm Web-Manager OpenSource Edition")>
#Else
<Assembly: AssemblyTitle("camm Web-Manager OpenSource Mono Edition")> 
#End If
<Assembly: AssemblyDescription("camm Web-Manager Main Library")>
<Assembly: AssemblyCompany("CompuMaster GmbH")>
<Assembly: AssemblyProduct("camm Web-Manager")>
<Assembly: AssemblyCopyright("2001-2016 CompuMaster GmbH")>
<Assembly: AssemblyTrademark("camm")>

<Assembly: CLSCompliant(True)>
<Assembly: ComVisibleAttribute(False)>
<Assembly: System.Security.AllowPartiallyTrustedCallers()>
<Assembly: Guid("077F0C95-42AC-4A9D-884B-03D1E00564E6")>

'<Assembly: AssemblyDelaySign(False)>
#If SIGNASSEMBLY Then
<Assembly: AssemblyKeyFile("R:\Shared\Technik\Company Keys\camm.biz.snk")>
#End If
'<Assembly: AssemblyKeyFile("cammWM.snk")>
'<Assembly: AssemblyKeyName("")>

#If NetFramework <> "1_1" Then
<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ConsoleApplication2")>
<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo("cammWM.Test")>
'<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo("cammWM.Admin")>
'<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo("cammWM.Test, PublicKey=00240000048000009400000006020000002400005253413100040000010001008922e685cb343c6070cd3ed73bbff6d7eeef006efe0ff092e673aca6a0a20f7c4bf6608ad93b1d230a8e2f50eb09d12899cfe5766cfc6df5305bcc00eb7eb0257933adecf21aa940831a0aeb7de127ec60040f4854fe72f7729995610cd8b7d162173d52bda6777b403908fe4b2f2a12dd9372cc21ca9b9b569b4d1014bbddbf")>
'<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo("cammWM.Test.dll, PublicKey=00240000048000009400000006020000002400005253413100040000010001008922e685cb343c6070cd3ed73bbff6d7eeef006efe0ff092e673aca6a0a20f7c4bf6608ad93b1d230a8e2f50eb09d12899cfe5766cfc6df5305bcc00eb7eb0257933adecf21aa940831a0aeb7de127ec60040f4854fe72f7729995610cd8b7d162173d52bda6777b403908fe4b2f2a12dd9372cc21ca9b9b569b4d1014bbddbf")>
'<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo("cammWM.Test, PublicKey=00240000048000009400000006020000002400005253413100040000010001008922E685CB343C6070CD3ED73BBFF6D7EEEF006EFE0FF092E673ACA6A0A20F7C4BF6608AD93B1D230A8E2F50EB09D12899CFE5766CFC6DF5305BCC00EB7EB0257933ADECF21AA940831A0AEB7DE127EC60040F4854FE72F7729995610CD8B7D162173D52BDA6777B403908FE4B2F2A12DD9372CC21CA9B9B569B4D1014BBDDBF")>
'<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo("cammWM.Test.dll, PublicKey=00240000048000009400000006020000002400005253413100040000010001008922E685CB343C6070CD3ED73BBFF6D7EEEF006EFE0FF092E673ACA6A0A20F7C4BF6608AD93B1D230A8E2F50EB09D12899CFE5766CFC6DF5305BCC00EB7EB0257933ADECF21AA940831A0AEB7DE127EC60040F4854FE72F7729995610CD8B7D162173D52BDA6777B403908FE4B2F2A12DD9372CC21CA9B9B569B4D1014BBDDBF")>
#End If