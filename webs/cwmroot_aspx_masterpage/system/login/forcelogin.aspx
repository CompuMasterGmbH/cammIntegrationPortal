<%@ Page Inherits="CompuMaster.camm.WebManager.Pages.Login.ForceLogin" Language="VB" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager ID="cammWebManager" runat="server" />
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<%
    If cammWebManager.Internationalization.OfficialServerGroup_Title = "" Then
        'Database connection error/server configuration error
        CompuMaster.camm.WebManager.Utils.RedirectTemporary(Me.Context, cammWebManager.Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & Server.URLEncode(25))
    End If
%>
<html>
<head>
    <title><%= Server.HtmlEncode(cammWebManager.Internationalization.Logon_HeadTitle) %></title>
    <link rel="stylesheet" type="text/css" href="<%= cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData %>style_standard.css" />
</head>

<body vlink="#585888" alink="#000080" link="#000080" leftmargin="0" topmargin="0" marginwidth="0" marginheight="0" bgcolor="#FFFFFF">
    <table border="0" width="100%" height="100%" cellspacing="0" cellpadding="0">
        <tr>
            <td align="center">
                <table border="0" cellspacing="40" cellpadding="0">
                    <tr>
                        <td align="center">

                            <table bgcolor="#cccccc" border="0" cellspacing="0" height="91" width="400">
                                <tr>
                                    <td align="center" height="85">
                                        <table border="0" width="400">
                                            <tr>
                                                <td align="left" bgcolor="#DFDFDF" valign="top" width="400" height="80">
                                                    <table border="0" height="21" width="100%">
                                                        <tr>
                                                            <td colspan="5" class="titlebar"><%= cammWebManager.Internationalization.Logon_BodyTitle %></td>
                                                        </tr>
                                                        <tr>
                                                            <td width="10%" colspan="2" align="center" valign="middle">
                                                                <br />
                                                                <img src="<%= cammWebManager.System_GetServerGroupImageSmallAddr(cammWebManager.CurrentServerIdentString) %>"></td>
                                                            <td class="normal">
                                                                <p><%= cammWebManager.Internationalization.Logon_AskForForcingLogon %></p>
                                                            </td>
                                                            <td width="3%">&nbsp;</td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="3" align="center" height="50">
                                                                <table cellspacing="2" cellpadding="0" align="center" align="left" border="0">
                                                                    <tr>
                                                                        <td colspan="2">
                                                                            <form name="formlogin" target="_top" method="post" action="<%= CheckLoginUrl %>">
                                                                                <input type="hidden" name="ForceLogin" value="1">
                                                                                <input type="hidden" name="Username" value="<%= server.htmlencode(Session("System_Logon_Buffer_Username")) %>">
                                                                                <input type="hidden" name="Passcode" value="<%= server.htmlencode(Session("System_Logon_Buffer_Passcode")) %>">
                                                                                <input type="submit" value="<%= Server.HtmlEncode(cammWebManager.Internationalization.SystemButtonYes) %>" name="submit">
                                                                            </form>
                                                                        </td>
                                                                        <td>
                                                                            <form method="post" target="_top" action="<%= cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL %>" id="form1" name="form1">
                                                                                <input type="hidden" name="Username" value="<%= Session("System_Logon_Buffer_Username") %>"><input type="submit" value="<%= Server.HtmlEncode(cammWebManager.Internationalization.SystemButtonNo) %>" id="cancel" name="cancel"></form>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>

                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>
