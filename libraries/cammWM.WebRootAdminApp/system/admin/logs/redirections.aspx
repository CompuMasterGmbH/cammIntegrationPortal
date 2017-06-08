<%@ Register TagPrefix="uc1" TagName="control_redirections" Src="control_redirections.ascx" %>
<%@ Page Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Pages.Redirections" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<camm:webmanager id="cammWebManager" runat="server"></camm:webmanager>
<HTML>
	<HEAD>
		<title>Redirections</title>
		<link rel="stylesheet" type="text/css" href="/system/admin/logs/styles.css">
	</HEAD>
	<body>
			

<form id="Form1" method="post" runat="server">
	<h1>Redirections</h1>
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
							
							<asp:Table runat="server" width="100%">
								<asp:TableRow runat="server">
									<asp:TableCell id="AppNameCell" runat="server" cssclass="header1" horizontalalign="center"></asp:TableCell>
								</asp:TableRow>
							</asp:Table>
							<!-- ---------Content--------- -->
							<div align="center"><input type="button" id="PrintPage" onclick="window.print();return false;" runat=server value="Print" />
							<br>
							<br>
							
								<asp:table id="printViewTable" runat="server" Visible="False">
									<asp:TableRow>
										<asp:TableCell ColumnSpan="2" ID="cellFrom"></asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell ColumnSpan="2" ID="cellTo"></asp:TableCell>
									</asp:TableRow>
								</asp:table>
								<TABLE id="SelectTable" style="HEIGHT: 71px" width="100%" align="center" runat="server">
									<TR align="center">
										<TD id="cellF" width="30%" height="40">
											<P align="center">
												<asp:label id="LabelFrom" runat="server" width="45px" font-bold="True">From </asp:label>
												<asp:textbox id="TextBoxDateFrom" runat="server" Width="72px"></asp:textbox>
												<asp:label id="LabelDateFromError" runat="server" width="46px" font-bold="True" forecolor="Red"></asp:label></P>
										</TD>
										<TD id="cellServ" vAlign="bottom" colSpan="3" height="40"><STRONG></STRONG></TD>
										<TD id="cellT" width="30%" height="40">
											<asp:label id="LabelTo" runat="server" width="25px" font-bold="True">To </asp:label>
											<asp:textbox id="TextBoxDateTo" runat="server" Width="72px"></asp:textbox>
											<asp:label id="LabelDateToError" runat="server" width="46px" font-bold="True" forecolor="Red"></asp:label></TD>
									</TR>
									<TR align="center">
										<TD id="cellCalFrom" vAlign="top" rowSpan="2">
											<asp:calendar id="CalendarFrom" runat="server" OnSelectionChanged="CalendarFrom_SelectionChanged"></asp:calendar></TD>
										<TD width="10%">&nbsp;</TD>
										<TD id="CheckBoxCell" vAlign="top" align="left"></TD>
										<TD width="10%">&nbsp;</TD>
										<TD id="cellCalTo" vAlign="top" rowSpan="2">
											<asp:calendar id="CalendarTo" runat="server" OnSelectionChanged="CalendarTo_SelectionChanged"></asp:calendar></TD>
									</TR>
									<TR>
										<TD vAlign="top" align="center" colSpan="3">
											<P>&nbsp;</P>
											<P>
												<asp:button id="ButtonShowReport" onclick="Button_Click" runat="server" Text="Show report"></asp:button>
												<asp:button id="ButtonPrintView" onclick="LinkButton_Click" runat="server" text="Print view" />
												</P>
											<P>
												<asp:label id="LabelInterval" runat="server" Font-Bold="True" ForeColor="Red">Label</asp:label></P>
										</TD>
									</TR>
								</TABLE>
							</div>
							<br>
							<br>
							<div align="center">
								<uc1:control_redirections id="RedirectionsControl" runat="server"></uc1:control_redirections></div>
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
