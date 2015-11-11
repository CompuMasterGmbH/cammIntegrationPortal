<%@ Control Language="VB" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NumberDeletedUsersControl" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.Drawing" %>

<!-- Insert content here -->
<p align="center">
<asp:Table runat=server Width="100%" CellPadding=0 CellSpacing=0>
		<asp:TableRow Runat=server>
			<asp:TableCell ID="ReportNameCell" Runat=server></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</p>
<p align="center">
	<asp:Label id="LabelNumberDeletedUsersControl" runat="server" CssClass="header2"></asp:Label></p>
<p align="center"><asp:label id="LabelDescription" runat="server" Width="80%">Description</asp:label></p>
