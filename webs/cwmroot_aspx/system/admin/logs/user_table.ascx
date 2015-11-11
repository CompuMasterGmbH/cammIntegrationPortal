<%@ import Namespace="System.Drawing" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Control Language="VB" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.UserTable" %>

<camm:WebManager id="cammWebManager" runat="server"></camm:WebManager>
<p align="center">
	<!-- Insert content here -->
	<asp:Table id="DataValues" runat="server" CellPadding="2" CellSpacing="2">
	</asp:Table>
</p>
<p align="center">
	<asp:Label id="LabelIsVisits" runat="server" font-size="Larger" font-bold="True"></asp:Label>
</p>
<P align="center">
	<asp:Label id="LabelDescription" runat="server" Width="80%">Description</asp:Label></P>
