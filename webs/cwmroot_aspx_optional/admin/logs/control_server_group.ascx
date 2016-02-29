<%@ import Namespace="System.Drawing" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Control Language="VB" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.ServerGroupsControl" %>
<%@ Register TagPrefix="uc1" TagName="areachart" Src="areachart.ascx" %>
<P align="center">
	<asp:Table runat=server Width="100%" CellPadding=0 CellSpacing=0>
		<asp:TableRow Runat=server>
			<asp:TableCell ID="nameCell" Runat=server></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</P>
<P align="center">
	<uc1:areachart id="Areachart1" runat="server"></uc1:areachart></P>
<P align="center">
	<asp:Label id="LabelDescription" runat="server" Width="80%">Description</asp:Label></P>
