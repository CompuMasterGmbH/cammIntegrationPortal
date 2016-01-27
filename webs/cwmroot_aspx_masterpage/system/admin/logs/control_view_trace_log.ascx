<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.Drawing" %>
<%@ Control Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.ViewTraceLogControl" %>

<camm:webmanager id="cammWebManager" runat="server"></camm:webmanager>
<P align="center"><asp:label id="LabelTitle" runat="server" Font-Bold="True" Font-Size="Larger" CssClass="header2"></asp:label></P>
<P align="center"><asp:table id="traceTable" runat="server" CellPadding="2" CellSpacing="2">
		<asp:TableRow BackColor="#C0C0FF" Font-Bold="True">
			<asp:TableCell Text="Application name" HorizontalAlign="Center"></asp:TableCell>
			<asp:TableCell Width="30%" Text="URL" HorizontalAlign="Center"></asp:TableCell>
			<asp:TableCell Text="Date" HorizontalAlign="Center"></asp:TableCell>
			<asp:TableCell Text="Remote IP" HorizontalAlign="Center"></asp:TableCell>
			<asp:TableCell Text="Description" HorizontalAlign="Center"></asp:TableCell>
		</asp:TableRow>
	</asp:table></P>
<P align="center">
	<asp:Label id="LabelIsData" runat="server" Font-Size="Larger" Font-Bold="True"></asp:Label></P>
