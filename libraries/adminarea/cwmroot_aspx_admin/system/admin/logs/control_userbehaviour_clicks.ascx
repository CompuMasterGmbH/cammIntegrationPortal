<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.Drawing" %>
<%@ Control Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.UserClicksControl" %>
<%@ Register TagPrefix="uc1" TagName="piechart" Src="piechart.ascx" %>

<!-- Insert content here -->
<div align="center">
	<asp:Table runat=server Width="100%" CellPadding=0 CellSpacing=0>
		<asp:TableRow Runat=server>
			<asp:TableCell ID="ReportNameCell" Runat=server></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
<div align="center">
	<uc1:piechart id="Piechart1" runat="server"></uc1:piechart>
</div>
<br>
<div align="center">
	<asp:Table id="DataValues" runat="server" CellPadding="2" CellSpacing="2">
		<asp:TableRow ForeColor="Black" BackColor="#C0C0FF">
			<asp:TableCell VerticalAlign="Middle" Width="30%" Font-Bold="True" HorizontalAlign="Center" Text="User Name"></asp:TableCell>
			<asp:TableCell VerticalAlign="Middle" Width="60%" Font-Bold="True" HorizontalAlign="Center" Text="Click rate"></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
<br>
<div align="center">
	<asp:Label id="LabelIsData" runat="server" Font-Bold="True" Font-Size="Larger"></asp:Label></div>
<br>
<div align="center">
	<asp:Label id="LabelDescription" runat="server" Width="80%">Description</asp:Label>
</div>
