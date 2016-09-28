<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.MarketActivations" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" PageTitle="Administration - Markets and languages" SecurityObject="System - Administration - Markets" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_wo_form.aspx"-->
	<h3><font face="Arial">Administration - Markets and languages</font></h3>
 	<TABLE cellSpacing=0 cellPadding=0 bgColor=#ffffff border=0 bordercolor="#C1C1C1">
	  <TBODY>
	  <TR>
        <TD vAlign=top>

	<asp:DataGrid Font-Name="Arial" Font-Size="10pt" runat="server" id="Markets" CellPadding="3">

	        <HeaderStyle Font-Bold="true" BackColor="#C1C1C1" />

	        <AlternatingItemStyle BackColor="#E1E1E1" />

		<columns>
			<asp:BoundColumn runat="server" DataField="ID"></asp:BoundColumn>
			<asp:BoundColumn runat="server" DataField="Name"></asp:BoundColumn>
			<asp:TemplateColumn runat="server">
				<ItemTemplate><a href="markets.aspx?market=<%# DataBinder.Eval(Container.DataItem, "ID") %>&marketactivated=<%# Server.UrlEncode(CType(Not CompuMaster.camm.WebManager.Utils.Nz(DataBinder.Eval(Container.DataItem, "Activated"),False), Integer)) %>"><%# Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(DataBinder.Eval(Container.DataItem, "Activated"),False).ToString) %></a></ItemTemplate>
			</asp:TemplateColumn>
		</columns>
	</asp:DataGrid>

	</TD></TR>
      </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_wo_form.aspx"-->