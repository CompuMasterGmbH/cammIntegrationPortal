<%@ Register TagPrefix="cammWebEdit" Namespace="CompuMaster.camm.WebManager.Modules.WebEdit.Controls" Assembly="cammWM" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>sWcms Live Demo</title>
		<camm:webmanager id="cammWebManager" runat="server"></camm:webmanager>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
		
    <h1>
        Smart Web Content Management System (sWcms) Control
    </h1>

<p><A href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/smartwcms/sources.src">View source code of the page</A></p>


<h3>Live demo</h3>

			<cammWebEdit:SmartWcms id="DemoSWcms" MarketLookupMode="Market" runat="server" 
				SecurityObjectEditMode="@@Public" 
				Docs="" 
				DocsReadOnly="" 
				Media="" 
				MediaReadOnly="" 
				Images="" 
				ImagesReadOnly="" 
			>This is some predefined code in the ASPX file. This content will be overlapped by the first, released version of new content stored in the CWM database.</cammWebEdit:SmartWcms>
		</form>
	</body>
</HTML>
