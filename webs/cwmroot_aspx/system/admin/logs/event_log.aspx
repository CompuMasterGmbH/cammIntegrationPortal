<%@ Page Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Pages.EventLog" %>
<%@ Register TagPrefix="uc1" TagName="eventlog_datagrid" Src="event_log_datagrid.ascx" %>
<%@ Register TagPrefix="uc1" TagName="control_event_log" Src="control_event_log.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<camm:webmanager id="cammWebManager" runat="server"></camm:webmanager>
<HTML>
	<HEAD>
		<title>Event Log</title>
		<link rel="stylesheet" type="text/css" href="/system/admin/logs/styles.css">
	</HEAD>
	<body>
		

<form id="Form1" method="post" runat="server">
	<h1>Event Log</h1>
	Navigate to: <a href="event_log.aspx">Event Log</a> | <a href="redirections.aspx">Redirections</a>
	<asp:Table Runat="server" ID="BorderTable" BackColor="#F7F7F7" CellPadding="0" CellSpacing="0" BorderColor="#C0C0C0" BorderWidth="0" Height="100%" Width="100%">
		<asp:TableRow Runat="server" ID="LeftRightBorderTRow">
			<asp:TableCell Runat="server" ID="HereGoesChildTable" Height="100%" Width="100%" HorizontalAlign="Left"	VerticalAlign="Top">
				<asp:table id="Table2" CellSpacing="0" CellPadding="0" BorderWidth="0" Runat="server" Width="100%">
					<asp:TableRow Runat="server" ID="TitleTRow">
						<asp:TableCell Runat="server" ID="TitleTCell" Width="100%" Height="20" />
					</asp:TableRow>
					<asp:TableRow Runat="server" ID="MenuBarTRow">
						<asp:TableCell Runat="server" ID="MenuBarTCell" Width="100%" Height="25"> </asp:TableCell>
					</asp:TableRow>
					<asp:TableRow Runat="server" ID="ContentRow">
						<asp:TableCell Runat="server" ID="ContentCell">								
							<br>
							<!-- ---------Content--------- -->
							<br>
							<br>
							<div align="center">
								<TABLE id="SelectTable" style="HEIGHT: 71px" width="100%" align="center" border="0" runat="server">
									<TR align="center">
										<TD id="cellF" width="30%" height="40">
											<P align="center"><asp:label id="LabelFrom" runat="server" font-bold="True" width="45px">From </asp:label><asp:textbox id="TextBoxDateFrom" runat="server" Width="72px"></asp:textbox><asp:label id="LabelDateFromError" runat="server" font-bold="True" width="46px" forecolor="Red"></asp:label></P>
										</TD>
										<TD id="cellServ" vAlign="bottom" colSpan="3" height="40"><INPUT id="TextBox_hidden" type="hidden" name="TextBox_hidden" runat="server"><STRONG>Server 
												groups:</STRONG></TD>
										<TD id="cellT" width="30%" height="40"><asp:label id="LabelTo" runat="server" font-bold="True" width="25px">To </asp:label><asp:textbox id="TextBoxDateTo" runat="server" Width="72px"></asp:textbox><asp:label id="LabelDateToError" runat="server" font-bold="True" width="46px" forecolor="Red"></asp:label></TD>
									</TR>
									<TR align="center">
										<TD id="cellCalFrom" vAlign="top" rowSpan="2"><asp:calendar id="CalendarFrom" runat="server" OnSelectionChanged="CalendarFrom_SelectionChanged"></asp:calendar></TD>
										<TD width="10%">&nbsp;</TD>
										<TD id="CheckBoxCell" vAlign="top" align="left"></TD>
										<TD width="10%">&nbsp;</TD>
										<TD id="cellCalTo" vAlign="top" rowSpan="2"><asp:calendar id="CalendarTo" runat="server" OnSelectionChanged="CalendarTo_SelectionChanged"></asp:calendar></TD>
									</TR>
									<TR>
										<TD vAlign="top" align="center" colSpan="3">
											<br>
											<div align="center"><STRONG>Events:</STRONG><br>
												<uc1:control_event_log id="EventLogControl" runat="server"></uc1:control_event_log></div>
											<P><asp:button id="ButtonShowReport" onclick="Button_Click" runat="server" Text="Show report"></asp:button></P>
											<P><asp:label id="LabelInterval" runat="server" Font-Bold="True" ForeColor="Red">Label</asp:label></P>
										</TD>
									</TR>
								</TABLE>
							</div>
							<br>
							<br>
							<div align="center"><uc1:eventlog_datagrid id="EventlogDatagridControl" runat="server"></uc1:eventlog_datagrid></div>
							<!-- ---------/Content--------- -->
						</asp:TableCell>
					</asp:TableRow>
				</asp:table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</form>

	</body>
</HTML>
