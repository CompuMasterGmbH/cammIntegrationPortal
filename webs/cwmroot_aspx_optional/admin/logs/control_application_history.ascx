<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.Drawing" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Register TagPrefix="uc1" TagName="linechart" Src="linechart.ascx" %>
<%@ Control Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.ApplicationHistoryControl" %>
<P align="center">
</P>
<P align="center">
	<uc1:linechart id="Linechart1" runat="server"></uc1:linechart></P>
<P align="center">
	<asp:Label id="LabelDescription" runat="server" Width="80%">Description</asp:Label></P>
