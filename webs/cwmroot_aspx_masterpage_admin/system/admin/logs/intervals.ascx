<%@ Register TagPrefix="uc1" TagName="barchart" Src="barchart.ascx" %>
<%@ Register TagPrefix="uc1" TagName="linechart" Src="linechart.ascx" %>
<%@ Control Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NumberClicksDifferentIntervals" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ import Namespace="System.Drawing" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Web.UI.WebControls" %>

<P align="center"><STRONG><asp:Table runat=server Width="100%" CellPadding=0 CellSpacing=0>
		<asp:TableRow Runat=server>
			<asp:TableCell ID="ReportNameCell" Runat=server></asp:TableCell>
		</asp:TableRow>
	</asp:Table></STRONG></P>
<P align="center"><asp:table id="intervalTable" runat="server" Visible="False">
		<asp:TableRow>
			<asp:TableCell ID="cellApp"></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell ID="cellInt"></asp:TableCell>
		</asp:TableRow>
	</asp:table></P>
<P align="center"><asp:dropdownlist id="ApplicationDropList" runat="server"></asp:dropdownlist></P>
<P></P>
<P align="center"><asp:dropdownlist id="IntervalDropList" runat="server">
		<asp:ListItem Value="Day">Day</asp:ListItem>
		<asp:ListItem Value="Weekday">Weekday</asp:ListItem>
		<asp:ListItem Value="Month">Month</asp:ListItem>
		<asp:ListItem Value="Year">Year</asp:ListItem>
	</asp:dropdownlist></P>
<P align="center"><asp:label id="LabelIsPeriodTooLong" runat="server" ForeColor="Red" Font-Size="Larger" Font-Bold="True"></asp:label></P>
<P align="center"><uc1:barchart id="Barchart1" runat="server"></uc1:barchart><uc1:linechart id="Linechart1" runat="server"></uc1:linechart></P>
<P align="center">
	<asp:Label id="LabelDescription" runat="server" Width="80%">Description</asp:Label></P>
