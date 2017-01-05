<%@ Page Language="vb" Inherits="CompuMaster.camm.WebManager.Pages.Administration.DisplayText" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" SecurityObject="System - Mail Queue Monitor" />
<html>	
	<head>
	<title>Mail Queue</title>
	<link rel="stylesheet" type="text/css" href="/sysdata/style_standard.css">
	</head>
	<body leftmargin="0" rightmargin="0" topmargin="0" bottommargin="0" onload="javascript:ReturnDate();">
		<form id="Form1" method="post" runat="server">			
			<table border="0" cellpadding="5" cellspacing="0" width="100%" height="100%">
				<tr bgcolor="#e0e0e0">
					<td><asp:Label Runat="server" ID="LabelTitle" Font-Bold="True" Text="Error Details" /></td>
				</tr>
				<tr>
					<td valign="top" align="Left"><asp:Label Runat="server" ID="txtMsg" width="500" height="200" /></td>
				</tr>
				<tr>
					<td align="center"><asp:Button Runat="server" ID="ButtonClose" Font-Bold="True" Width="110" Text="Close" /></td>
				</tr>			
			</table>
			<br>			
		</form>
	</body>
</HTML>
