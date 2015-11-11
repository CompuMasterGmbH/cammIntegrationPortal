		</td>
	</tr>
</table>

<table border="0" cellpadding="1" cellspacing="0" width="100%" height="25">
	<tr>
		<td width="2" background="/sysdata/images/hg_navi_quer_links.gif"></td>
		<td background="/sysdata/images/hg_navi_quer.gif"><font face="Tahoma" size="1"><i>ca</i></font><font face="Tahoma" color="#3a72c6" size="1"><i><b>m</b></i></font><font face="Tahoma" color="#777d86" size="1"><i>m </i></font><font face="Tahoma" color="black" size="1">Unternehmensbereich</font></td>
		<td background="/sysdata/images/hg_navi_quer.gif"></td>
		<td background="/sysdata/images/hg_navi_quer.gif"></td>
		<td background="/sysdata/images/hg_navi_quer.gif"></td>
		<td background="/sysdata/images/hg_navi_quer.gif" width="658"><div align="right"><font face="Verdana" size="1"><%

dim CopyRightSinceYear
CopyRightSinceYear = System_GetServerConfig(GetCurrentServerIdentString, "AreaCopyRightSinceYear")
If CopyRightSinceYear = Year(Now) Or CopyRightSinceYear = "" Then
	Response.Write (server.htmlencode("© " & Year(Now)))
Else
	Response.Write (server.htmlencode("© " & CopyRightSinceYear & "-" & Year(Now)))
End If 

%> <a title="<%= System_GetServerConfig(GetCurrentServerIdentString, "AreaCompanyWebSiteTitle") %>" href="<%= System_GetServerConfig(GetCurrentServerIdentString, "AreaCompanyWebSiteURL") %>" target="_blank"><%= server.htmlencode(System_GetServerConfig(GetCurrentServerIdentString, "AreaCompanyFormerTitle")) %></a> - <%= StatusLineCopyright_AllRightsReserved %></font></div></td>
		<td width="3" background="/sysdata/images/hg_navi_quer_rechts.gif">
		</td>
	</tr>
</table>

</body>
</html>