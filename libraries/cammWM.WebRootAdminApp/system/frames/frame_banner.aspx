<%@ Page language="VB" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<% on error resume next %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>

<head>
<!--#include virtual="/sysdata/includes/metalink.aspx"-->
<link rel="stylesheet" type="text/css" href="<%= cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData %>style_standard.css">
<meta http-equiv="refresh" content="3600; URL=<%= Response.ApplyAppPathModifier(Request.ServerVariables("SCRIPT_NAME")) %>?Lang=<%= Request.QueryString("Lang") %>">
<title>Banner</title>
<base target="frame_main">
</head>

<body topmargin="0" leftmargin="0" marginheight="0" marginwidth="0" bgcolor="#FFFF66" style="BACKGROUND-COLOR: #ffff66">
<!--#include virtual="/sysdata/frames/cont_banner.aspx"-->
</body>

</html>