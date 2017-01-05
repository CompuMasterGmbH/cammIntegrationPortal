<%@ Page ValidateRequest="False" Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.SystemCheckup" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - System checkup" id="cammWebManager"
    SecurityObject="System - User Administration - Applications" runat="server" />
<%@ Register TagPrefix="camm" Namespace="CompuMaster.camm.WebManager.Controls" Assembly="cammWM" %>

<script runat="server">
    Sub PageOnPreRender(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
        Me.DataBind()
    End Sub
</script>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <link rel="stylesheet" type="text/css" href="<%= cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData %>style_standard.css">
    <title>
        <%= cammWebManager.PageTitle %></title>
    <camm:META ID="META" runat="server" />
    <%= cammWebManager.PageAdditionalHeaders %>
</head>
<body bgcolor="#ffffff" leftmargin="0" topmargin="10" marginwidth="0" marginheight="10"
    <%= compumaster.camm.webmanager.Utils.JoinNameValueCollectionToString(cammWebManager.PageAdditionalBodyAttributes, " ", "=""", """") %>>
    <h3>
        System checkup</h3>
        
    <a href="/sysdata/admin/check_flags_not_required.aspx">List additional flags not required by any security object</a>
    <br />    
    <a href="/sysdata/admin/check_file_consistency.aspx">Check file consistency</a>
            
            
    <%@ register tagprefix="camm" tagname="WebManagerAdminMenu" src="adminmenu.ascx" %>
    <camm:WebManagerAdminMenu HRef="apps.aspx" ID="cammWebManagerAdminMenu" runat="server" />
    <!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
