<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ import Namespace="System.Web.UI.WebControls" %>
<%@ import Namespace="System.IO" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.Drawing" %>
<%@ Control Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.EventLogControl" %>
<%@ Register TagPrefix="uc1" TagName="eventlog_datagrid" Src="eventlog_datagrid.ascx" %>

<div align="center"><asp:Table runat=server Width="100%" CellPadding=0 CellSpacing=0>
		<asp:TableRow Runat=server>
			<asp:TableCell ID="ReportNameCell" Runat=server></asp:TableCell>
		</asp:TableRow>
	</asp:Table></div>
<br>
<DIV align="center">
	<TABLE id="Table1" height="93" cellSpacing="1" cellPadding="1" border="0" runat="server">
		<TR>
			<TD colSpan="2" height="22"><asp:checkbox id="CheckboxRuntimeWarnings" runat="server"></asp:checkbox></TD>
			<TD colSpan="2" height="22"><asp:checkbox id="CheckboxApplicationWarnings" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD colSpan="2"><asp:checkbox id="CheckboxRuntimeExceptions" runat="server"></asp:checkbox></TD>
			<TD colSpan="2"><asp:checkbox id="CheckboxApplicationExceptions" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD colSpan="2"><asp:checkbox id="CheckboxRuntimeInformation" runat="server"></asp:checkbox></TD>
			<TD colSpan="2"><asp:checkbox id="CheckboxApplicationInformation" runat="server"></asp:checkbox></TD>
		</TR>
		<TR align="center">
			<td><asp:checkbox id="CheckboxDebug" runat="server"></asp:checkbox></td>
			<td colSpan="2"><asp:checkbox id="CheckboxLogin" runat="server"></asp:checkbox></td>
			<td><asp:checkbox id="ChecboxkLogout" runat="server"></asp:checkbox></td>
		</TR>
	</TABLE>
</DIV>
