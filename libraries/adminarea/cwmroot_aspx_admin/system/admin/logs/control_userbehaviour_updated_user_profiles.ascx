<%@ Control Language="VB" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.UpdatedProfilesListControl" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.Drawing" %>

<P align="center">
	<asp:Table runat=server Width="100%" CellPadding=0 CellSpacing=0>
		<asp:TableRow Runat=server>
			<asp:TableCell ID="ReportNameCell" Runat=server></asp:TableCell>
		</asp:TableRow>
	</asp:Table></P>
<P></P>
<P align="center">
	<asp:Table id="DataValues" runat="server" CellPadding="2" CellSpacing="2">
		<asp:TableRow ForeColor="Black" BackColor="#C0C0FF">
			<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="Updated User Name "></asp:TableCell>
			<asp:TableCell Font-Bold="True" HorizontalAlign="Center" Text="Date of update"></asp:TableCell>
			<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="Kind of change"></asp:TableCell>
		</asp:TableRow>
	</asp:Table></P>
<P align="center">
	<asp:Label id="LabelIsData" font-bold="True" runat="server" font-size="Larger"></asp:Label></P>
<P align="center">
	<asp:Label id="LabelDescription" runat="server" Width="80%">Description</asp:Label></P>
