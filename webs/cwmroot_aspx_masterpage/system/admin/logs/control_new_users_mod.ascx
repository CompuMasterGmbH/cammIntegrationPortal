<%@ Control Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NewUsersDetailsControl" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.Drawing" %>

<P align="center"><asp:Table runat=server Width="100%" CellPadding=0 CellSpacing=0>
		<asp:TableRow Runat=server>
			<asp:TableCell ID="ReportNameCell" Runat=server></asp:TableCell>
		</asp:TableRow>
	</asp:Table></P>
<P></P>
<P align="center"><asp:table id="DataValues" runat="server" Width="90%" CellPadding="2" CellSpacing="2">
		<asp:TableRow ForeColor="Black" BackColor="#C0C0FF">
			<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="New User Name "></asp:TableCell>
			<asp:TableCell VerticalAlign="Middle" Font-Bold="True" HorizontalAlign="Center" Text="Registration dates"></asp:TableCell>
			<asp:TableCell Font-Bold="True" HorizontalAlign="Center" Text="Access level"></asp:TableCell>
			<asp:TableCell Font-Bold="True" HorizontalAlign="Center" Text="Current memberships"></asp:TableCell>
			<asp:TableCell Font-Bold="True" HorizontalAlign="Center" Text="Contact details"></asp:TableCell>
		</asp:TableRow>
	</asp:table></P>
<P align="center"><asp:label id="LabelIsData" font-bold="True" runat="server" font-size="Larger"></asp:label></P>
<P align="center"><asp:label id="LabelDescription" runat="server" Width="80%">Description</asp:label></P>
