<%@ Page Language="vb" AutoEventWireup="false" Codebehind="showemail.aspx.vb" Inherits="CompuMaster.camm.WebManager.Pages.Administration.MailQueueMonitorShowEmail"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" SecurityObject="System - Mail Queue Monitor" />
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>Mail Queue</title>
		<link rel="stylesheet" type="text/css" href="/sysdata/style_standard.css">
	</HEAD>
	<body marginleft="0">
		<form id="Form1" method="post" runat="server">
			<table border="0" cellpadding="5" cellspacing="0" width="100%">
				<tr>
					<td><asp:Label Runat="server" ID="LabelTitle" Font-Bold="True" /></td>
				</tr>
				<tr><td runat=server id="AreaHtmlText">
					<table border="0" cellpadding="5" cellspacing="0" width="100%">
						<tr bgcolor="#e0e0e0"><td>
							<asp:Label Runat="server" ID="LabelMessageHtmlFormat" Font-Bold="True">Message (HTML format)</asp:Label></td></tr>
						<tr>
							<td align="left"><asp:Label Runat="server" ID="LabelHtmlText" /><br></td>
						</tr>
							</table>
						</td></tr>
				<tr bgcolor="#e0e0e0"><td>
					<asp:Label Runat="server" ID="LabelMessageTextFormat" Font-Bold="True">Message (TEXT format)</asp:Label></td></tr>
				<tr>
					<td><pre><code><asp:Label Runat="server" ID="LabelEmailText" /></code></pre></td>
				</tr>
				<tr>
					<td align="center"><asp:Button Runat="server" ID="ButtonClose" Font-Bold="True" Width="110" Text="Close" /></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
