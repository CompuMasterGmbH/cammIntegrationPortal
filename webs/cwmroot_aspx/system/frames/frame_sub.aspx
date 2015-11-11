<%@ Page language="VB" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<%
Try
	If Request.QueryString("Relogon") = 1 Then
		'Roundtrip to all server/script engine combinations to refresh their session information
		If Session("System_Username") <> "" Then
			dim System_RedirectURI
			Response.Write ("<!--" & System_RedirectURI & "-->")
			System_RedirectURI = cammWebManager.System_GetNextLogonURI (Session("System_Username"))
			If System_RedirectURI <> "" Then
				CompuMaster.camm.WebManager.Utils.RedirectTemporary(Me.Context, System_RedirectURI & "&redirectto=" & Server.URLEncode (cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData & "frames/frame_sub.aspx"))
			End If
		End If
	End If
Catch
	'for example: network errors, database connection errors
End Try
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<!--<%= Now %> - <%= Session.SessionID %> -->
<head>
<!--#include virtual="/sysdata/includes/metalink.aspx"-->
<meta http-equiv="refresh" content="300; URL=<%= Response.ApplyAppPathModifier(Request.ServerVariables("SCRIPT_NAME")) %>?Relogon=1">
<link rel="stylesheet" type="text/css" href="<%= cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData %>style_standard.css">
<title>Footer</title>
<base target="_self">
</head>

<body topmargin="0" leftmargin="0" marginheight="0" marginwidth="0" bgcolor="#FFFF66" style="BACKGROUND-COLOR: #ffff66">
<!--#include virtual="/sysdata/frames/cont_statusline.aspx"-->
<iframe name="iframe_refreshlogin" width=1 height=1 frameborder=0 scrolling=no src="<%= cammWebManager.Internationalization.User_Auth_Config_Paths_Login %>refreshlogin.aspx">
</iframe>
<script language="javascript">

function refreshWindow() {
	var myiframe; 
	try  
	{ 
		myiframe = iframe_refreshlogin; 
	} 
	catch (ex) 
	{ 
		myiframe = document.getElementsByName('iframe_refreshlogin')[0]; 
	}; 
	myiframe.location = '<%= cammWebManager.Internationalization.User_Auth_Config_Paths_Login %>refreshlogin.aspx';
	self.setTimeout('refreshWindow()', 250000);
}

self.setTimeout('refreshWindow()', 250000);

</script>
</body>

</html>