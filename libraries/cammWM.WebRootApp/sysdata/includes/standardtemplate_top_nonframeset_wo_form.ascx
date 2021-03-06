<%@ Control Language="VB" Inherits="CompuMaster.camm.WebManager.Controls.UserControl" %>
<%@ Register TagPrefix="camm" Namespace="CompuMaster.camm.WebManager.Controls" Assembly="cammWM" %>
<%@ Register TagPrefix="camm" TagName="LoginArea" Src="~/sysdata/includes/loginarea.ascx" %>
<script runat="server">
	Sub PageOnPreRender (sender as object, e as eventargs) Handles MyBase.PreRender
		Me.DataBind
	End Sub
</script>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>

<head>
<link rel="stylesheet" type="text/css" href="<%# cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData %>style_standard.css">
<title><%# cammWebManager.PageTitle %></title>
<camm:META id="META" runat="server" />
<%# cammWebManager.PageAdditionalHeaders %>
</head>

<body bgcolor="#ffffff" leftmargin="0" topmargin="10" marginwidth="0" marginheight="10"<%= compumaster.camm.webmanager.Utils.JoinNameValueCollectionToString(cammWebManager.PageAdditionalBodyAttributes, " ", "=""", """") %>>
<table border="0" cellspacing="0" cellpadding="0" width="100%">
	<tr>
		<td valign="top" width="12"></td>
		<td valign="top" width="140"><camm:LoginArea id="LoginArea" runat="server" /></td>
		<td valign="top" width="20"></td>
		<td valign="top">