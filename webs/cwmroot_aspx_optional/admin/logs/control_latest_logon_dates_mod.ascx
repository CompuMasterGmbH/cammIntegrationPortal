<%@ Control Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.LastLoginDatesControl" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
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
	</asp:Table></div>
<br>
<div align="center">
	<asp:Table id="DataValues" runat="server" CellPadding="2" CellSpacing="2">
		<asp:TableRow BackColor="#C0C0FF">
			<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="Timeperiod"></asp:TableCell>
			<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="Number of users"></asp:TableCell>
		</asp:TableRow>
	</asp:Table></div>
<br>
<br>
<div align="center">
	<asp:Label id="LabelDescription" runat="server" Width="80%">Description</asp:Label></div>
