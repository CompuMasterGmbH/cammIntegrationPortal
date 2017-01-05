<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ctrl_action.ascx.vb" Inherits="CompuMaster.camm.WebManager.Controls.Administration.MailQueueMonitorActionControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table border="0" cellpadding="0" cellspacing="0" width="100%">
	<tr>
		<td><nobr><asp:HyperLink Runat="server" ID="HyperLinkShowEmailText" NavigateUrl="#" >e-mail text</asp:HyperLink></nobr></td>
	</tr>
	<tr>
		<td><nobr><asp:LinkButton Runat="server" ID="LinkbuttonResend" Visible=False >Resend</asp:LinkButton></nobr></td>
	</tr>
	<tr>
		<td><nobr><asp:LinkButton Runat="server" ID="LinkbuttonFailure" Visible=False >Failure</asp:LinkButton></nobr></td>
	</tr>
	<tr>
		<td><nobr><asp:LinkButton Runat="server" ID="LinkbuttonSendThisEmailToMe" Visible=False >Send to me</asp:LinkButton></nobr></td>
	</tr>
</table>
