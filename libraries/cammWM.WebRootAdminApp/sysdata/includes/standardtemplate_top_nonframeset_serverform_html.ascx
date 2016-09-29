<%@ Control Language="VB" Inherits="CompuMaster.camm.WebManager.Controls.UserControl" %>
<%@ Register TagPrefix="camm" Namespace="CompuMaster.camm.WebManager.Controls" Assembly="cammWM" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>

<head>
<%
If Not cammWebManager Is Nothing Then
%>
<link rel="stylesheet" type="text/css" href="<%= cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData %>style_standard.css">
<title><%= cammWebManager.PageTitle %></title>
<camm:META id="META" runat="server" />
<%= cammWebManager.PageAdditionalHeaders %>
<%
End If
%>
</head>

<%
If cammWebManager Is Nothing Then
%>
<body bgcolor="#ffffff" leftmargin="0" topmargin="10" marginwidth="0" marginheight="10">
<%
Else
%>
<body bgcolor="#ffffff" leftmargin="0" topmargin="10" marginwidth="0" marginheight="10"<%= compumaster.camm.webmanager.Utils.JoinNameValueCollectionToString(cammWebManager.PageAdditionalBodyAttributes, " ", "=""", """") %>>
<%
End If
%>