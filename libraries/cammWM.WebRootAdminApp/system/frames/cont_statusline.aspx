<table bgcolor="#FFFF66" cellspacing="0" cellpadding="2" border="0" height="30" width="100%">
  <tr>
    <td bgcolor="#FFFF66" width="33%" height="30" valign="top"><font size="1" face="Arial"><% 
    dim CopyRightSinceYear 
    CopyRightSinceYear = cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "AreaCopyRightSinceYear")
    If CopyRightSinceYear = Year(Now) Then
		Response.Write (server.htmlencode("© " & Year(Now)))
	Else
		Response.Write (server.htmlencode("© " & CopyRightSinceYear & "-" & Year(Now)))
	End If %> <a href="<%= cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "AreaCompanyWebSiteURL") %>" target="_blank"><%= server.htmlencode(cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "AreaCompanyFormerTitle")) %></a><br><%= cammWebManager.Internationalization.StatusLineCopyright_AllRightsReserved %></font></td>
    <td bgcolor="#FFFF66" height="30" valign="top"><% 

If Session("System_Username") <> "" Then 
	%><table border="0" cellpadding="0" cellspacing="0"><tr><td valign="top" align="center"><font size="1" face="Arial"><%= cammWebManager.Internationalization.StatusLineLoggedInAs %><br><a href="<%= Response.ApplyAppPathModifier("/sysdata/account_updateprofile.aspx") %>?ID=<%= cammWebManager.System_GetUserID (Session("System_Username")) %>" target="frame_main"><%= cammWebManager.System_GetUserDetail (cammWebManager.System_GetUserID (Session("System_Username")), "CompleteName") & " (" & Session("System_Username") & ")" %></a></font></td><td>&nbsp; &nbsp; </td><td valign="top" align="left"><img src="/sysdata/images/space.gif" border="0" width="39" height="6"><br><a href="/sysdata/logon.aspx?Action=Logout" target="frame_main"><img src="/sysdata/images/basestyle/but_logout.gif" border="0" width="39" height="14"></a></td><tr></table><% 
Else
	%><TABLE cellSpacing="2" cellPadding="0" align="left" border="0"><form target="frame_main" name="formlogin" method="post" action="<%= cammWebManager.Internationalization.User_Auth_Validation_CheckLoginURL %>">
	  <TR>
	    <TD WIDTH="100"><nobr><FONT face=Arial size="1"><%= cammWebManager.Internationalization.StatusLineUsername %>:&nbsp;</font><INPUT name="Username" size="11">
		<FONT face=Arial size="1"><%= cammWebManager.Internationalization.StatusLinePassword %>:&nbsp;</font><INPUT type="password" size="11" name="Passcode">
		<INPUT type="submit" value="<%= Server.HtmlEncode(cammWebManager.Internationalization.StatusLineSubmit) %>" name="submit"></nobr></TD>
	  </TR>
	</form></TABLE>
	<%
End If 

	%></td>
    <td bgcolor="#FFFF66" width="33%" height="30" valign="top">
      <p align="right"><font size="1" face="Arial"><b><a href="<%= Response.ApplyAppPathModifier("/sysdata/editorial.aspx") %>" target="frame_main"><%= server.htmlencode(cammWebManager.Internationalization.StatusLineEditorial) %></a>
      |&nbsp;<a href="<%= Response.ApplyAppPathModifier("/sysdata/disclaimer.aspx") %>" target="frame_main"><%= server.htmlencode(cammWebManager.Internationalization.StatusLineLegalNote) %></a></b><br>Powered by <a href="http://www.compumaster.de/" target="_blank">CompuMaster
      GmbH</a></font></td>
  </tr>
</table>