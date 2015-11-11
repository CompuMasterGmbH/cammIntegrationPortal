<%@ Page Inherits="CompuMaster.camm.WebManager.Pages.Login.ForceLogin" language="VB" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<%
If cammWebManager.Internationalization.OfficialServerGroup_Title = "" Then
	'Database connection error/server configuration error
	CompuMaster.camm.WebManager.Utils.RedirectTemporary(Me.Context, cammWebManager.Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & Server.URLEncode(25))
End If
%>
<html>
<head>
<!--#include virtual="/sysdata/includes/metalink.aspx"-->
<title><%= Server.HtmlEncode(cammWebManager.Internationalization.Logon_HeadTitle) %></title>
<link rel="stylesheet" type="text/css" href="<%= cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData %>style_standard.css">
</head>

<BODY vLink="#585888" aLink="#000080" link="#000080" leftMargin="0" topMargin="0" marginwidth="0" marginheight="0" bgcolor="#FFFFFF">
<table border="0" width="100%" height="100%" cellspacing="0" cellpadding="0">
<tr><td align="center"><table border="0" cellspacing="40" cellpadding="0"><tr><td align="center">

<TABLE bgColor=#cccccc border=0 cellSpacing=0 height=91 width=400>
	<TR>
		<TD align="center" height=85>
			<TABLE border=0 width="400">
				<TR>
					<TD align=left bgColor=#DFDFDF vAlign=top width=400 height="80">
						<TABLE border=0 height=21 width="100%" >
								<TR>
										<TD colspan=5 class="titlebar"><%= cammWebManager.Internationalization.Logon_BodyTitle %></TD>
								</TR>
						    <TR>
										<TD width=10% colspan=2 align=center valign=middle><BR><IMG src="<%= cammWebManager.System_GetServerGroupImageSmallAddr(cammWebManager.CurrentServerIdentString) %>"></TD>
						        <TD class="normal"><P><%= cammWebManager.Internationalization.Logon_AskForForcingLogon %></P>
						        </TD>
						        <TD width="3%">&nbsp;</TD>
						    </TR>
						    <TR>
						        <TD colspan=3 align="center" height=50><TABLE cellSpacing="2" cellPadding="0" align="center" align="left" border="0">
									  <TR>
									    <TD ColSpan="2"><form name="formlogin" target="_top" method="post" action="<%= CheckLoginUrl %>">
									<input type="hidden" name="ForceLogin" value="1">
									<INPUT type="hidden" name="Username" value="<%= server.htmlencode(Session("System_Logon_Buffer_Username")) %>">
									<INPUT type="hidden" name="Passcode" value="<%= server.htmlencode(Session("System_Logon_Buffer_Passcode")) %>">
									<INPUT type="submit" value="<%= Server.HtmlEncode(cammWebManager.Internationalization.SystemButtonYes) %>" name="submit"></form></td><td><form method="post" target="_top" action="<%= cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL %>" id=form1 name=form1><INPUT type="hidden" name="Username" value="<%= Session("System_Logon_Buffer_Username") %>"><input type="submit" value="<%= Server.HtmlEncode(cammWebManager.Internationalization.SystemButtonNo) %>" id=cancel name=cancel></FORM></TD>
									  </TR>
									</TABLE></TD>
						    </TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>
		</TD>
	</TR>
</TABLE>

</td></tr>
</table></td></tr>
</table>
</body>
</html>