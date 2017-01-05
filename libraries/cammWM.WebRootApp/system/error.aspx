<%@ Page Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Pages.Application.ErrorPage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title runat="server" id="PageTitle" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<link href="/sysdata/style_standard.css" type="text/css" rel="stylesheet" />
	</HEAD>
	<body style="FONT-SIZE: 10px; FONT-FAMILY: Arial, Verdana" leftmargin="20" topmargin="20" marginheight="20" marginwidth="20">
		<table cellspacing="0" cellpadding="0" width="100%" border="0">
			<tbody>
				<tr>
					<td valign="top">
						<H1><asp:label id="ErrorPageTitle" runat="server">Label of error page</asp:label></H1>
						<asp:label id="ErrorDescription" runat="server"></asp:label>
						<P><small><asp:label id="TimeStamp" runat="server"></asp:label></small></P>
					</td>
				</tr>
			</tbody>
		</table>
	</body>
</HTML>