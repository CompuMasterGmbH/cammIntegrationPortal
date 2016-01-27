<%@ Register TagPrefix="uc1" TagName="control_new_users_mod" Src="control_new_users_mod.ascx" %>
<%@ Page Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Pages.DeletedUsersList" %>
<%@ Register TagPrefix="uc1" TagName="menu" Src="menu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="control_userbehaviour_deleted_users" Src="control_userbehaviour_deleted_users.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<camm:webmanager id="cammWebManager" runat="server"></camm:webmanager>
<HTML>
	<HEAD>
		<title>deleted_users_list</title>
		<link rel="stylesheet" type="text/css" href="/system/admin/logs/styles.css">			
			
	</HEAD>
	<BODY>
		

<form id="Form1" method="post" runat="server">
	<asp:Table Runat="server" ID="BorderTable" BackColor="#F7F7F7" CellPadding="0" CellSpacing="0" BorderColor="#C0C0C0" BorderWidth="0" Height="100%" Width="100%">
		<asp:TableRow Runat="server" ID="TopBorderTRow">
			<asp:TableCell Runat="server" ID="TopLeftTCell" Height="4" Width="4" BorderWidth="0"><img src="/system/admin/logs/images/img_style_corner_left-top_4x4.gif" border="0" height="4" width="4" /></asp:TableCell>
			<asp:TableCell Runat="server" ID="TopTCell" background="/system/admin/logs/images/img_style_border_top_1x4.gif" Height="4"	Width="100%" BorderWidth="0"><img src="/system/admin/logs/images/img_style_space_1x1.gif" border="0" height="4" width="4" /></asp:TableCell>
			<asp:TableCell Runat="server" ID="TopRightTCell" Height="4" Width="4" BorderWidth="0"><img src="/system/admin/logs/images/img_style_corner_right-top_4x4.gif" border="0" height="4" width="4" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow Runat="server" ID="LeftRightBorderTRow">
			<asp:TableCell Runat="server" ID="LeftTCell" background="/system/admin/logs/images/img_style_border_left_4x1.gif"	Height="100%" Width="4" BorderWidth="0" />
			<asp:TableCell Runat="server" ID="HereGoesChildTable" Height="100%" Width="100%" HorizontalAlign="Left"	VerticalAlign="Top">
				<asp:table id="Table2" CellSpacing="0" CellPadding="0" BorderWidth="0" Runat="server" Width="100%">
					<asp:TableRow Runat="server" ID="TitleTRow">
						<asp:TableCell Runat="server" ID="TitleTCell" Width="100%" Height="20" />
					</asp:TableRow>
					<asp:TableRow Runat="server" ID="MenuBarTRow">
						<asp:TableCell Runat="server" ID="MenuBarTCell" Width="100%" Height="25"> <uc1:menu id="Menu1" runat="server"></uc1:menu></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow Runat="server" ID="ContentRow">
						<asp:TableCell Runat="server" ID="ContentCell">								
							
							<asp:Table runat="server" width="100%">
								<asp:TableRow runat="server">
									<asp:TableCell id="AppNameCell" runat="server" cssclass="header1" horizontalalign="center"></asp:TableCell>
								</asp:TableRow>
							</asp:Table>
							<!-- ---------Content--------- -->


			<DIV align="center"><a href="#" id="PrintPage" onclick="window.print();return false;" runat=server>Print</a>
			<br>
			<br>
			
				<asp:LinkButton id="LinkButtonPrintView" onclick="LinkButton_Click" runat="server" CssClass="Link">print view</asp:LinkButton>
				<asp:table id="printViewTable" runat="server" Visible="False">
					<asp:TableRow>
						<asp:TableCell ColumnSpan="2" ID="cellFrom"></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="2" ID="cellTo"></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="Top" Font-Bold="True" Text="Server groups:" ID="cellServer"></asp:TableCell>
						<asp:TableCell ID="CellServerGroupsPrintTable"></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="2" HorizontalAlign="Center" ID="cellButton"></asp:TableCell>
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
						<TD id="cellServ" vAlign="bottom" colSpan="3" height="40"><INPUT id="TextBox_hidden" type="hidden" name="TextBox_hidden" runat="server"><STRONG>Server 
								groups:</STRONG></TD>
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
								<asp:button id="ButtonShowReport" onclick="Button_Click" runat="server" Text="Show report"></asp:button></P>
							<P>
								<asp:Label id="LabelInterval" runat="server" Font-Bold="True" ForeColor="Red">Label</asp:Label></P>
						</TD>
					</TR>
				</TABLE>
			</DIV>
			<br>
			<br>
			<div align="center">
				<uc1:control_userbehaviour_deleted_users id="DeletedUsersListControl" runat="server"></uc1:control_userbehaviour_deleted_users></div>
	

							<!-- ------------------------------------ -->
						</asp:TableCell>
					</asp:TableRow>
				</asp:table>
			</asp:TableCell>
			<asp:TableCell Runat="server" ID="RightTCell" background="/system/admin/logs/images/img_style_border_right_4x1.gif" Height="100%" Width="4" BorderWidth="0" />
		</asp:TableRow>
		<asp:TableRow Runat="server" ID="cammlogo" BackColor="LightGray">
			<asp:TableCell Runat="server" background="/system/admin/logs/images/img_style_border_left_4x1.gif" Height="100%" Width="4"	BorderWidth="0" ID="Tablecell3" />
			<asp:TableCell Runat="server" HorizontalAlign="Right" Width="100%" ID="Tablecell6">
				<asp:Table Runat="server" ID="cammLogo2" BorderWidth="0" CellPadding="0" CellSpacing="0">
					<asp:TableRow Runat="server" ID="Tablerow13" >
						<asp:TableCell Runat="server" Width="100%" ID="Tablecell7" />
						<asp:TableCell Runat="server" Font-Name="Arial" Font-Size="8" ID="Tablecell8">:::&nbsp;</asp:TableCell>
						<asp:TableCell Runat="server" VerticalAlign="Bottom" ID="Tablecell9">
							<asp:Image Runat="server" ImageUrl="/system/admin/logs/images/cammLogoImg.gif" BorderStyle="None" ID="Image1" />
						</asp:TableCell>
						<asp:TableCell Runat="server" Font-Name="Arial" Font-Size="8" ID="Tablecell10">
							<nobr>&nbsp;Statistics</nobr></asp:TableCell>
						<asp:TableCell Runat="server" Font-Name="Arial" Font-Size="8" ID="Tablecell11">&nbsp;:::</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
			<asp:TableCell Runat="server" background="/system/admin/logs/images/img_style_border_right_4x1.gif" Height="100%"
				Width="4" BorderWidth="0" ID="Tablecell12" />
		</asp:TableRow>
		<asp:TableRow Runat="server" ID="BottomBorderTRow">
			<asp:TableCell Runat="server" ID="BottomLeftTCell" Height="4" Width="4" BorderWidth="0">
				<img src="/system/admin/logs/images/img_style_corner_left-bottom_4x4.gif" border="0" height="4" width="4" /></asp:TableCell>
			<asp:TableCell Runat="server" ID="BottomTCell" background="/system/admin/logs/images/img_style_border_bottom_1x4.gif"
				Height="4" Width="100%">
				<img src="/system/admin/logs/images/img_style_space_1x1.gif" border="0" height="4" width="4" /></asp:TableCell>
			<asp:TableCell Runat="server" ID="BottomRightTCell" Height="4" Width="4" BorderWidth="0">
				<img src="/system/admin/logs/images/img_style_corner_right-bottom_4x4.gif" border="0" height="4" width="4" /></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</form>

	</BODY>
</HTML>
