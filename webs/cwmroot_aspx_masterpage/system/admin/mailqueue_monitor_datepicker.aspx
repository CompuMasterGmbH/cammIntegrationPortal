<%@ Page Language="vb" AutoEventWireup="false" Codebehind="DatePicker.aspx.vb" Inherits="CompuMaster.camm.WebManager.Pages.Administration.MailQueueMonitorDatePicker"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" SecurityObject="System - Mail Queue Monitor" />
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
	<head>
		<title>Choose date</title>
		<style type="text/css">
		A:link { TEXT-DECORATION: none }
		A:visited { TEXT-DECORATION: none }
		A:active { TEXT-DECORATION: none }
		A:hover { TEXT-DECORATION: none }
		</style>
	</head>
	<body leftmargin="0" rightmargin="0" topmargin="0" bottommargin="0" onload="javascript:ReturnDate();">
		<form id="Form1" method="post" runat="server">
			<asp:Panel Runat="server" ID="PanelShowDatePicker">
				<asp:Calendar id="CalendarDatePicker" Runat="server" BackColor="#eaeaea" BorderStyle="None" BorderWidth="0"
					CellPadding="0" CellSpacing="0" DayHeaderStyle-BackColor="#d2d2d2" DayHeaderStyle-BorderStyle="None"
					DayHeaderStyle-ForeColor="#0000ff" DayHeaderStyle-Height="20" Font-Name="Ms Sens Serif, Verdana"
					Font-Size="9" Font-Underline="False" ForeColor="#0000ff" Height="170" NextMonthText=">>>"
					NextPrevFormat="CustomText" NextPrevStyle-BackColor="#808080" NextPrevStyle-BorderStyle="None"
					NextPrevStyle-Font-Bold="True" NextPrevStyle-Font-Underline="False" NextPrevStyle-ForeColor="#ffffff"
					NextPrevStyle-Height="20px" OnSelectionChanged="Calendar_SelectionChanged" OtherMonthDayStyle-ForeColor="#add8e6"
					PrevMonthText="<<<" SelectedDayStyle-BackColor="#3366ff" SelectedDayStyle-Font-Bold="True"
					TitleFormat="MonthYear" TitleStyle-BackColor="#808080" TitleStyle-BorderStyle="None" TitleStyle-Font-Bold="True"
					TitleStyle-ForeColor="#ffffff" TitleStyle-Height="25" TodayDayStyle-BorderStyle="Solid" TodayDayStyle-BorderWidth="1"
					TodayDayStyle-BorderColor="#0000ff" WeekendDayStyle-BackColor="#e6e6e6" WeekendDayStyle-ForeColor="#ff3300"
					Width="200"></asp:Calendar>
			</asp:Panel>
			<table border="0" cellpadding="0" cellspacing="0" width="100%">
				<tr>
					<td width="100%" align="center" bgcolor="#808080" height="25" valign="middle">
						<asp:Button Runat="server" ID="ButtonOK" Text=" OK " Width="70" font-bold="True" />
					</td>
				</tr>
			</table>
			<br>
			<asp:TextBox Runat="server" ID="TextDate" BackColor="#ffffff" ReadOnly="True" style="BORDER-TOP-STYLE:none;BORDER-RIGHT-STYLE:none;BORDER-LEFT-STYLE:none;BORDER-BOTTOM-STYLE:none"
				ForeColor="#ffffff" />
		</form>
	</body>
</HTML>
