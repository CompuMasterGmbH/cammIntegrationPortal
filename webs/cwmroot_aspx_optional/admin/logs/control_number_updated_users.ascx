<%@ import Namespace="System.Drawing" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Control Language="VB" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NumberUpdatedUsersControl" %>

<!-- Insert content here -->
<p align="center"></p>
<p align="center"><asp:Table runat=server Width="100%" CellPadding=0 CellSpacing=0>
		<asp:TableRow Runat=server>
			<asp:TableCell ID="ReportNameCell" Runat=server></asp:TableCell>
		</asp:TableRow>
	</asp:Table></p>
<p align="center"><asp:table id="DataValues" runat="server" CellPadding="2" CellSpacing="2">
		<asp:TableRow ForeColor="Black" BackColor="#C0C0FF">
			<asp:TableCell Font-Bold="True" HorizontalAlign="Center" Text="Access level"></asp:TableCell>
			<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="Updated user profiles "></asp:TableCell>
		</asp:TableRow>
	</asp:table></p>
<p align="center"><asp:label id="LabelDescription" runat="server" Width="80%">Description</asp:label></p>
