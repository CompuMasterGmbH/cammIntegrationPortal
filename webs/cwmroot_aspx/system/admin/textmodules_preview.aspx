<%@ Page Language="vb" AutoEventWireup="false" EnableViewState="False" Inherits="CompuMaster.camm.WebManager.Modules.Text.Administration.Pages.TextModules_Preview"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebmanager" runat="server" SecurityObject="System - TextModules" />
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title id="PageTitle" runat=server ></title>
		<link rel="stylesheet" type="text/css" href="/sysdata/style_standard.css">
	</head>
	<body topmargin="0" bottommargin="0" leftmargin="0" rightmargin="0">
		<form id="Form1" method="post" runat="server">
			<table border="0" cellpadding="3" cellspacing="0" width="100%">
				<tr>
					<td width="100%" height="30">
						<table border="0" cellpadding="2" cellspacing="0" width="100%">
							<tr valign=top>
								<td><font face="Arial" size="3"><b><asp:Label ID="LabelTextAdministration" Runat=server ></asp:Label></b></font></td>
								<td></td>
								<td align="right">
								<% if IsDevelopmentVersion then%>
									Version 2.0
								<%end if%>
								</td>
							</tr>
                        <tr valign="top">
                            <td colspan="3" align="right">
                                <a href="textmodules_overview.aspx">Textmodules overview</a>
                            </td>
                        </tr>							
						</table>
					</td>
				</tr>
				<tr bgcolor="#808080">
					<td height="20"><asp:Label ID="LabelTitle" Font-Bold=True ForeColor="#ffffff" Runat=server ></asp:Label></td>
				</tr>
				<tr height="350" valign=top >
					<td ID="LabelKeyPreview" Runat=server >
					</td>
				</tr>
				<tr height="30" valign=middle >
					<td align=center><asp:Button ID="ButtonClose" Width="110" Runat=server></asp:Button> </td>
				</tr>
				<tr>
					<td align="right">
					<% if IsDevelopmentVersion then%>
						[Form: 107]&nbsp;&nbsp;
					<%end if%>
					</td>
				</tr>
			</table>
		</form>
	</body>
</html>
