<%@ Page Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Pages.ViewTraceLog" %>
<%@ import Namespace="System.Collections" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ Register TagPrefix="uc1" TagName="control_view_trace_log" Src="control_view_trace_log.ascx" %>
<%@ Register TagPrefix="uc1" TagName="menu" Src="menu.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<camm:webmanager id="cammWebManager" runat="server"></camm:webmanager>
<HTML>
	<HEAD>
		<title>view_trace_log</title>
		<link rel="stylesheet" type="text/css" href="/system/admin/logs/styles.css">
			
	</HEAD>
	<body>
		

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


			<div align="center"><a href="#" id="PrintPage" onclick="window.print();return false;" runat=server>Print</a>
				<br>
				<br>
				<br>
				<asp:linkbutton id="LinkButtonPrintView" onclick="LinkButton_Click" runat="server" CssClass="Link">print view</asp:linkbutton>
				<br>
				<br>
				<br>
				<asp:table id="printViewTable" runat="server" Visible="False">
					<asp:TableRow>
						<asp:TableCell VerticalAlign="Top" Font-Bold="True" Text="Server groups:" ID="cellServer"></asp:TableCell>
						<asp:TableCell ID="cellServerGroupsPrintTable"></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="2" HorizontalAlign="Center" ID="cellButton"></asp:TableCell>
					</asp:TableRow>
				</asp:table>
				<TABLE id="SelectTable" cellSpacing="1" cellPadding="1" width="200" border="0" runat="server">
					<TR>
						<TD>
							<P align="center"><STRONG>Server groups</STRONG></P>
						</TD>
					</TR>
					<TR>
						<TD id="CheckBoxCell"></TD>
					</TR>
					<tr>
						<td align="center">
							<asp:dropdownlist id="UserList" runat="server"></asp:dropdownlist></td>
					</tr>
					<TR>
						<TD align="center">
							<P>&nbsp;</P>
							<P>
								<asp:Button id="ButtonShowReport" onclick="Button_Click" runat="server" Text="Show report"></asp:Button></P>
						</TD>
					</TR>
				</TABLE>
			</div>
			<br>
			<br>
			<div align="center"><uc1:control_view_trace_log id="ViewTraceLogControl" runat="server"></uc1:control_view_trace_log>
				<asp:Label id="LabelDescription" runat="server" Width="80%">Label</asp:Label></div>

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
		
	</body>
</HTML>
