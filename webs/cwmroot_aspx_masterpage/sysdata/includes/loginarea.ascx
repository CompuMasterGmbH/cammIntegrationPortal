<%@ Control Language="VB" Inherits="CompuMaster.camm.WebManager.Controls.UserControl" %>
<script runat="server">
	Sub PageOnPreRender (sender as object, e as eventargs) Handles MyBase.PreRender
		Me.DataBind
	End Sub
</script>
<font size="2">Here you could place your custom login area. Please see /sysdata/includes/loginarea.aspx !<%

If Request("LoginMessage") <> "" Then Response.Write (Request("LoginMessage"))

If Session("System_Username") <> "" Then
	%><p align="left"><font size="2" face="Arial"><b>Hallo <%= cammWebManager.System_GetUserAddresses (cammWebManager.System_GetCurUserID) %> <%= cammWebManager.System_GetUserDetail (cammWebManager.System_GetCurUserID, "NameAddition") %> <%= cammWebManager.System_GetUserDetail (cammWebManager.System_GetCurUserID, "LastName") %>!</b><br>Wir freuen uns jedes mal über Ihren Besuch!<br>
	<br>Folgende zusätzlichen Seiten stehen Ihnen zur Verfügung:<br><ul><li><a href="topsecret.aspx">TopSecret!!</a></li></ul><br>
	<br><a href="<%= cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL & "?Action=Logout" %>">Abmelden</a>
	<br><a href="/sysdata/account_updatepassword.aspx?ID=<%= cammWebManager.System_GetCurUserID %>">Passwort ändern</a>
	<br><a href="/sysdata/account_updateprofile.aspx?ID=<%= cammWebManager.System_GetCurUserID %>">Profil ändern</a></font><%
Else
	%><form name="formlogin" method="post" action="<%= cammWebManager.Internationalization.User_Auth_Validation_CheckLoginURL %>"><%= cammWebManager.Internationalization.StatusLineUsername %>:<br><INPUT name="Username" size="11">
	<br><%= cammWebManager.Internationalization.StatusLinePassword %>:<br><INPUT type="password" size="11" name="Passcode"><br>
		<INPUT type="submit" value="<%= Server.HtmlEncode(cammWebManager.Internationalization.StatusLineSubmit) %>" name="submit">
	</form>
	<br><a href="/sysdata/account_register.aspx">Zur Erstregistrierung</a>
	<br><a href="/sysdata/account_sendpassword.aspx">Passwort vergessen?</a>
	<%
End If

%></font>
<iframe name="iframe_refreshlogin" width=100 height=100 frameborder=0 scrolling=no src="<%= cammWebManager.Internationalization.User_Auth_Config_Paths_Login %>refreshlogin.aspx">
</iframe>