<%@ import Namespace="System.Drawing" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Control Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NewUserListControl" %>

<!-- Insert content here -->
<p align="center">
</p>
<p align="center">
	<asp:Table runat=server Width="100%" CellPadding=0 CellSpacing=0>
		<asp:TableRow Runat=server>
			<asp:TableCell ID="ReportNameCell" Runat=server></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</p>
<p align="center">
	<asp:Table id="DataValues" runat="server" CellPadding="2" CellSpacing="2">
		<asp:TableRow ForeColor="Black" BackColor="#C0C0FF">
			<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="New User Name "></asp:TableCell>
			<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="Registration dates"></asp:TableCell>
			<asp:TableCell Font-Bold="True" HorizontalAlign="Center" Text="Created by"></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</p>
<p align="center">
	<asp:Label id="LabelIsData" runat="server" font-bold="True" font-size="Larger"></asp:Label>
</p>
<p align="center">
	<asp:Label id="LabelDescription" runat="server" Width="80%">Description</asp:Label>
</p>
