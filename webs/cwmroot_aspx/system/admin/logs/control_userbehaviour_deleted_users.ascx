<%@ Control Language="VB" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.DeletedUsersListControl" %>
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
		<asp:TableRow BackColor="#C0C0FF">
			<asp:TableCell VerticalAlign="Middle" BackColor="#C0C0FF" ForeColor="Black" Font-Bold="True" HorizontalAlign="Center"
				Text="Deleted User"></asp:TableCell>
			<asp:TableCell VerticalAlign="Middle" BackColor="#C0C0FF" ForeColor="Black" Font-Bold="True" HorizontalAlign="Center"
				Text="Date of deleting"></asp:TableCell>
		</asp:TableRow>
	</asp:Table></P>
<P align="center">
	<asp:Label id="LabelIsData" runat="server" Font-Bold="True" Font-Size="Larger"></asp:Label></P>
<P align="center">
	<asp:Label id="LabelDescription" runat="server" Width="80%">Description</asp:Label></P>
