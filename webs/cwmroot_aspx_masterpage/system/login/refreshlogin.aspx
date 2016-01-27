<%@ Page language="VB" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<%
	dim TestSessionTermination as boolean
	TestSessionTermination = False
	'Roundtrip to all server/script engine combinations to refresh their session information
	If Session("System_Username") <> "" AndAlso Request.QueryString("RoundTrip") <> "completed" Then
		dim System_RedirectURI as string
		System_RedirectURI = cammWebManager.System_GetNextLogonURI (Session("System_Username"))
		If System_RedirectURI <> "" Then
			CompuMaster.camm.WebManager.Utils.RedirectTemporary(Me.Context, System_RedirectURI & "&redirectto=" & Server.URLEncode (cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_Login & "refreshlogin.aspx"))
		End If
		TestSessionTermination = cammWebManager.System_IsSessionTerminated (Session("System_Username"))
	End If
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<% If TestSessionTermination = True Then %>
<script language=javascript>
confirm ('<%= cammWebManager.Internationalization.ErrorTimoutOrLoginFromAnotherStation %> (' + new Date + ')');
window.top.location = '<%= cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL & "?Action=logout" %>';
</script>
<% End If %>
<!--<%= Now %>-->
<head>
<!--#include virtual="/sysdata/includes/metalink.aspx"-->
<meta http-equiv="refresh" content="200; URL=<%= Response.ApplyAppPathModifier(Request.ServerVariables("SCRIPT_NAME")) %>?Relogon=1">
<link rel="stylesheet" type="text/css" href="<%= cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData %>style_standard.css">
<title>RefreshLogin</title>
<base target="_self">
</head>

<body topmargin="0" leftmargin="0" marginheight="0" marginwidth="0" bgcolor="#FFFFFF" style="BACKGROUND-COLOR: #ffffff">
</body>

</html>