<%@ Page Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Pages.User15Minutes" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.Drawing" %>
<camm:WebManager id="cammWebManager" runat="server"></camm:WebManager>
<HTML>
	<HEAD>
		<title>Last 15 minutes</title>
		<link rel="stylesheet" type="text/css" href="Styles.css">
			
	</HEAD>
	<body>
		<form runat="server">
			<div align="center">
				<asp:Label id="Label1" runat="server" font-bold="True">Last 15 minutes</asp:Label>
			</div>
			<br>
			<div align="center">
				<asp:Table id="DataValues" runat="server" CellPadding="2" CellSpacing="2">
					<asp:TableRow ForeColor="White" BackColor="Navy">
						<asp:TableCell VerticalAlign="Middle" ForeColor="White" ColumnSpan="4" Font-Size="Larger" Font-Bold="True"
							HorizontalAlign="Center" ID="cellTitle"></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow BackColor="#C0C0FF">
						<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="Application name"></asp:TableCell>
						<asp:TableCell Font-Bold="True" HorizontalAlign="Center" Text="Conflict description"></asp:TableCell>
<asp:TableCell Font-Bold="True" HorizontalAlign="Center" Text="Login date"></asp:TableCell>
<asp:TableCell Font-Bold="True" HorizontalAlign="Center" Text="Remote IP"></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</div>
			<!-- Insert content here -->
		</form>
	</body>
</HTML>
