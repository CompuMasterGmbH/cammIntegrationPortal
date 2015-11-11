<%@ Control Language="VB" Inherits="CompuMaster.camm.WebManager.Controls.UserControl" %>
<script runat="server">
	Sub PageOnPreRender (sender as object, e as eventargs) Handles MyBase.PreRender
		Me.DataBind
	End Sub

Function CopyRightYears as string
	dim CopyRightSinceYear as string
	CopyRightSinceYear = cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "AreaCopyRightSinceYear")
	If CopyRightSinceYear = Year(Now).ToString Or Convert.ToString(CopyRightSinceYear) = "" Then
		return (server.htmlencode("© " & Year(Now)))
	Else
		return (server.htmlencode("© " & CopyRightSinceYear & "-" & Year(Now)))
	End If 
end function
</script>
		</td>
	</tr>
</table>

<table border="0" cellpadding="1" cellspacing="0" width="100%" height="25">
	<tr>
		<td width="100%"><div align="center"><font face="Verdana" size="1"><%# CopyRightYears %> <a title="<%# cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "AreaCompanyWebSiteTitle") %>" href="<%# cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "AreaCompanyWebSiteURL") %>" target="_blank"><%# server.htmlencode(cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "AreaCompanyFormerTitle")) %></a> - <%# cammWebManager.Internationalization.StatusLineCopyright_AllRightsReserved %></font></div></td>
	</tr>
</table>