<%@ Page Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Pages.ListOfUsers" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ import Namespace="System.Drawing" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<camm:webmanager id="cammWebManager" runat="server"></camm:webmanager>
<HTML>
	<HEAD>
		<title>Last user login dates</title>
		<link rel="stylesheet" type="text/css" href="/system/admin/logs/styles.css">
			
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<div align="center">
				<asp:Label id="LabelTitle" runat="server" font-bold="True" Font-Size="Larger">Title</asp:Label></div>
			<br>
			<div align="center">
				<asp:Table id="DataValues" runat="server" Width="100%" CellPadding="2" CellSpacing="2">
					<asp:TableRow BackColor="#C0C0FF">
						<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="User name"></asp:TableCell>
						<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="Creation date"></asp:TableCell>
						<asp:TableCell Font-Bold="True" HorizontalAlign="Center" Text="Current memberships"></asp:TableCell>
						<asp:TableCell Font-Bold="True" HorizontalAlign="Center" Text="Contact details"></asp:TableCell>
					</asp:TableRow>
				</asp:Table></div>
				<br>
			<div align="center">
				<asp:Label id="LabelIsUsers" runat="server" Font-Size="Larger" Font-Bold="True"></asp:Label></div>
		</form>
	</body>
</HTML>
