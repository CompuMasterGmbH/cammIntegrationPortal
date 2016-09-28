<%@ Page Language="vb" AutoEventWireup="false" Codebehind="mailqueuemonitor.aspx.vb" Inherits="CompuMaster.camm.WebManager.Pages.Administration.MailQueueMonitor"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" SecurityObject="System - Mail Queue Monitor" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<script runat="server">
	'Sub PageOnLoad (sender as object, e as eventargs) Handles MyBase.Load
	'	HyperLinkCalendarTo.Attributes("onClick")="window.open('mailqueue_monitor_datepicker.aspx?tbn=TextboxTo&FormName=" & Server.UrlEncode(ServerFormClientID) & "', 'mainqueue_monitor_datepicker_to', 'alwaysRaised=yes, left=200, top=200, screenX=200, screenY= 200, width=200, height=195, resizable=no scrollbars=no').focus(); return (false);"
	'	HyperLinkCalendarFrom.Attributes("onClick")="window.open('mailqueue_monitor_datepicker.aspx?tbn=TextboxFrom&FormName=" & Server.UrlEncode(ServerFormClientID) & "', 'mainqueue_monitor_datepicker_from', 'alwaysRaised=yes, left=200, top=200, screenX=200, screenY= 200, width=200, height=195, resizable=no scrollbars=no').focus(); return (false);"
	'End Sub
</script>
		<style type="text/css">
		.HideDisplay {
			display:none;
		}
		</style>
		<script language="javascript">
function checkFilterNowValidity() {
	if ( ((document.<%= ServerFormClientID %>.TextBoxID.value) != '') ) 
	{
		var x = parseInt(document.<%= ServerFormClientID %>.TextBoxID.value);
		if ( isNaN(x) ) {
			alert('Please enter valid value');
			document.<%= ServerFormClientID %>.TextBoxID.focus();
			document.<%= ServerFormClientID %>.TextBoxID.select();
			return false;
		}
	}

	return true;
}

function toggleHideDisplay(hideDisplayArea, toggleButton)
{
	if (document.getElementById)
	{
		var myStyle = document.getElementById(hideDisplayArea).style;
		myStyle.display = myStyle.display? "":"block";
		var myButton = document.getElementById(toggleButton);
		if ( myButton.value == '+' ) 
		{
			myButton.value = '-';
		}
		else
		{
			myButton.value = '+';
		}
	}
	else if (document.all)
	{
		var myStyle = document.all[hideDisplayArea].style;
		myStyle.display = myStyle.display? "":"block";
		var myButton = document.all[toggleButton];
		if ( myButton.value == '+' ) 
		{
			myButton.value = '-';
		}
		else
		{
			myButton.value = '+';
		}
	}
	else if (document.layers)
	{
		var myStyle = document.layers[hideDisplayArea].style;
		myStyle.display = myStyle.display? "":"block";
		var myButton = document.layers[toggleButton];
		if ( myButton.value == '+' ) 
		{
			myButton.value = '-';
		}
		else
		{
			myButton.value = '+';
		}
	}
}

		</script>
			<table border="0" cellpadding="0" cellspacing="0" width="100%">
				<tr>
					<td>
						<P><font face="Arial" size="3"><b>Mail Queue Monitor</b></font></P>
					</td>
				</tr>
				<tr>
					<td><br>
					</td>
				</tr>
				<tr>
					<td><asp:Label Runat="server" ID="LabelPossibilitiesToFiler">Posibilities to Filter:</asp:Label></td>
				</tr>
				<tr>
					<td>
						<table border="0" cellpadding="5" cellspacing="0" width="100%">
							<tr>
								<td align="right">Subject:</td>
								<td>
									<asp:TextBox Runat="server" ID="TextboxSubject" />
								</td>
								<td align="right"><nobr>Sender:</nobr></td>
								<td>
									<asp:TextBox Runat="server" ID="TextboxFromAddress" />
								</td>
								<td align="right">
									ID:
								</td>
								<td>
									<asp:TextBox Runat="server" ID="TextBoxID" Columns="11" /><asp:TextBox Runat="server" ID="TextboxMoreAddressNo" Columns="5" visible=false />
								</td>
							</tr>
							<tr valign=top>
								<td align="right"><nobr>Time Frame: From</nobr></td>
								<td>
									<asp:TextBox Runat="server" ID="TextboxFrom" ReadOnly="True" Columns="15" />
									<asp:HyperLink Runat="server" ID="HyperlinkCalendarFrom" ImageUrl="../../system/admin/images/mailqueue_calendar.gif" NavigateUrl="#" />
								</td>
								<td align="right"><nobr>Receipient:</nobr></td>
								<td>
									<asp:TextBox Runat="server" ID="TextboxToAddress" />
								</td>
								<td align="right"></td>
								<td>
								</td>
							</tr>
							<tr>
								<td align="right">
									To</td>
								<td>
									<asp:TextBox Runat="server" ID="TextboxTo" ReadOnly="True" Columns="15" />
									<asp:HyperLink Runat="server" ID="HyperLinkCalendarTo" ImageUrl="../../system/admin/images/mailqueue_calendar.gif" NavigateUrl="#" />
								</td>
								<td align="right">Type:</td>
								<td><nobr>
									<asp:RadioButtonList Runat="server" ID="RadioButtonListToCcBcc" RepeatDirection="Horizontal" /></nobr>
								</td>
								<td align="right"></td>
								<td></td>
							</tr>
							<tr valign=top>
								<td align="right">State:</td>
								<td colspan="5">
									<asp:CheckBoxList Runat=server ID="CheckBoxListState" RepeatLayout=Flow RepeatDirection=Horizontal RepeatColumns=5 />
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr><td><br></td></tr>
				<tr>
					<td><asp:Button Runat="server" ID="ButtonFilterNow" Width="170" Text="Filter Now" Font-Bold=True />&nbsp;&nbsp;
						<asp:Button Runat="server" ID="ButtonReset" Width="110" Text="Reset" Font-Bold=True />&nbsp;&nbsp;
						<asp:Button Runat="server" ID="ButtonExportReport" Width="170" Text="Export Report" Font-Bold=True />
					</td>
				</tr>
				<tr><td><br></td></tr>
				<tr>
					<td style="border: solid 1 #808080">
						<asp:Table Runat=server ID="TableAnalysis" BorderStyle=none BorderWidth="0" CellPadding="5" CellSpacing="0" Width="100%" >
							<asp:TableRow Runat=server  BackColor="#e0e0e0" Height="25">
								<asp:TableCell Runat=server Width="20" Font-Bold=True></asp:TableCell>
								<asp:TableCell Runat=server Font-Bold=True>Action</asp:TableCell>
								<asp:TableCell Runat=server Font-Bold=True><asp:LinkButton Runat=server ID="LinkButtonState" Font-Bold=True ForeColor="#000000" ToolTip="Click to sort ...">State</asp:LinkButton></asp:TableCell>
								<asp:TableCell Runat=server Font-Bold=True>Receipient</asp:TableCell>
								<asp:TableCell Runat=server Font-Bold=True><asp:LinkButton Runat=server ID="LinkButtonSender" Font-Bold=True ForeColor="#000000" ToolTip="Click to sort ...">Sender</asp:LinkButton></asp:TableCell>
								<asp:TableCell Runat=server Font-Bold=True><asp:LinkButton Runat=server ID="LinkButtonSubject" Font-Bold=True ForeColor="#000000" ToolTip="Click to sort ...">Subject</asp:LinkButton></asp:TableCell>
								<asp:TableCell Runat=server Font-Bold=True><asp:LinkButton Runat=server ID="LinkButtonID" Font-Bold=True ForeColor="#000000" ToolTip="Click to sort ...">ID</asp:LinkButton></asp:TableCell>
								<asp:TableCell Runat=server Font-Bold=True><asp:LinkButton Runat=server ID="LinkButtonSentTime" Font-Bold=True ForeColor="#000000" ToolTip="Click to sort ..."><nobr>Sent Time</nobr></asp:LinkButton></asp:TableCell>
								<asp:TableCell Runat=server Font-Bold=True><nobr>Error Details</nobr></asp:TableCell>
							</asp:TableRow>
						</asp:Table>
					</td>
				</tr>
			</table>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->