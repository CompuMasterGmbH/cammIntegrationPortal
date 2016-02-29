<%@ Control Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.OnlineMomentControl" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.Drawing" %>

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
	<asp:Table id="Users" runat="server" CellPadding="2" CellSpacing="2">
		<asp:TableRow ForeColor="White" BackColor="Navy">
			<asp:TableCell VerticalAlign="Middle" ForeColor="White" ColumnSpan="4" Font-Size="Larger" Font-Bold="True"
				HorizontalAlign="Center" ID="cellTotalUsers"></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow id="rowTotalUsers" BackColor="#C0C0FF">
			<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="User Name"></asp:TableCell>
			<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="Remote IP"></asp:TableCell>
			<asp:TableCell Font-Bold="True" HorizontalAlign="Center" Text="E-mail"></asp:TableCell>
			<asp:TableCell Font-Bold="True" HorizontalAlign="Center" Text="Activity log"></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</p>
<p align="center">
	<asp:Label id="LabelDescription" runat="server" Width="80%">Description</asp:Label>
</p>
