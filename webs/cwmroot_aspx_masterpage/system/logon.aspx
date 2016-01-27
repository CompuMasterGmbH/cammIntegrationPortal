<%@ Page validateRequest=false Inherits="CompuMaster.camm.WebManager.Pages.Login.LoginForm" language="VB" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top.aspx"-->
<table border="0" cellspacing="0" cellpadding="0"><tr><td>
<%
If ErrorMessageForUser <> "" Then Response.Write (ErrorMessageForUser)
%>
<table border="0" width="100%" cellspacing="0" cellpadding="0">
  <tr>
    <td><FONT face=Arial size=4><%= cammWebManager.Internationalization.Logon_BodyTitle %></FONT><BR>&nbsp;
<HR><FONT face=Arial size=2><%= cammWebManager.Internationalization.Logon_BodyPrompt2User %></FONT><HR></td>
    <td width="140" valign="bottom"><img border="0" src="<%= cammWebManager.System_GetServerGroupImageSmallAddr(cammWebManager.CurrentServerIdentString) %>" align="right" width="100" height="94"></td>
  </tr>
</table>
<form name="formlogin" method="post" action="<%= cammWebManager.Internationalization.User_Auth_Validation_CheckLoginURL %>">
<TABLE cellSpacing="2" cellPadding="0" align="left" border="0">
  <TR>
    <TD WIDTH="100"><FONT face=Arial size="3"><nobr><%= cammWebManager.Internationalization.Logon_BodyFormUserName %>:&nbsp;</nobr></FONT> </TD>
    <TD><FONT face=Arial size="3"><INPUT name="Username" ></FONT></TD></TR>
  <TR>
    <TD WIDTH="100"><FONT face=Arial size="3"><nobr><%= cammWebManager.Internationalization.Logon_BodyFormUserPassword %>:&nbsp;</nobr></FONT></TD>
    <TD><FONT face=Arial size="3"><INPUT type="password" name="Passcode"></FONT></TD></TR>
  <TR>
    <TD ColSpan="2"><p><INPUT type="submit" value="<%= Server.HtmlEncode(cammWebManager.Internationalization.Logon_BodyFormSubmit) %>" name="submit"> &nbsp;<FONT face=Arial size="3"><input type="button" value="<%= Server.HtmlEncode(cammWebManager.Internationalization.Logon_BodyFormCreateNewAccount) %>" onclick="document.location='<%= cammWebManager.Internationalization.User_Auth_Validation_CreateUserAccountInternalURL %>';"></FONT></P></TD>
  </TR>
</TABLE>
</form>
<script language="JavaScript">
<!--
document.formlogin.Username.focus()
//-->
</script>
</td></tr><tr><td><%= cammWebManager.Internationalization.Logon_BodyExplanation %></td></tr></table>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom.aspx"-->