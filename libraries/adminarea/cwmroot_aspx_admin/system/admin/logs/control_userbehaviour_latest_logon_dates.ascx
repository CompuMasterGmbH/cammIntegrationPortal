<%@ import Namespace="System.Drawing" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Control Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.LatestLogonDatesControl" %>

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
<div align="center"><asp:Label id="LabelUsers" Font-Bold="True" runat="server"></asp:Label></div>
<p align="center">
	<asp:Table id="DataValues" runat="server" CellPadding="2" CellSpacing="2">
		<asp:TableRow BackColor="#C0C0FF">
			<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="User Name"></asp:TableCell>
			<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="Last login on"></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</p>
<p align="center">
	<asp:Label id="LabelIsData" runat="server" Font-Bold="True" Font-Size="Larger"></asp:Label>
</p>
<div align="center">
	<asp:Label id="LabelDescription" runat="server" Width="80%">Description</asp:Label>
</div>
