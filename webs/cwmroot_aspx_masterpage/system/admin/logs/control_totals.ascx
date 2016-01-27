<%@ Control Language="VB" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.ApplicationTotalsControl" %>
<%@ Register TagPrefix="uc1" TagName="piechart" Src="piechart.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ import Namespace="System.Runtime.Serialization" %>
<%@ import Namespace="System.Runtime.Serialization.Formatters.Binary" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.Drawing" %>
<div align="center">
	<asp:Table runat=server Width="100%" CellPadding=0 CellSpacing=0>
		<asp:TableRow Runat=server>
			<asp:TableCell ID="ReportNameCell" Runat=server></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
<div align="center">
	<uc1:piechart id="Piechart1" runat="server" Visible="False"></uc1:piechart>
</div>
<br>
<div align="center">
	<asp:Table id="DataValues" runat="server" BorderWidth="0px" CellPadding="2" CellSpacing="2">
		<asp:TableRow BackColor="#C0C0FF">
			<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="Application"></asp:TableCell>
			<asp:TableCell VerticalAlign="Middle" Width="60%" Font-Bold="True" HorizontalAlign="Center" Text="Click rate"></asp:TableCell>
			<asp:TableCell Font-Bold="True" HorizontalAlign="Center" Text="%"></asp:TableCell>
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
