<%@ Page ValidateRequest="False" Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.AppCheckUsersForMissingFlags" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify application" id="cammWebManager" SecurityObject="System - User Administration - Applications" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<h3>
	Users with missing required flags for application <a href="apps_update.aspx?ID=<%=request.querystring("ID")%>">
		<%=New CompuMaster.camm.WebManager.WMSystem.SecurityObjectInformation(CInt(Trim(request.querystring("ID"))), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem)).DisplayName %></a></h3>
<asp:datagrid runat="server" id="UsersWithMissingFlagsGrid" autogeneratecolumns="false" width="600">
<HeaderStyle backcolor="#c1c1c1" />
<AlternatingItemStyle backcolor="#e1e1e1" />
<columns>
<asp:BoundColumn HeaderText="Username" DataField="UserName" />
<asp:BoundColumn HeaderText="Missing flags" DataField="MissingFlags" />
</columns>
</asp:datagrid>
<asp:literal runat="server" id="ltrInfo" />
<p>
	<h4>
		Edit Flags with Batchfalgeditor</h4>
	<ul>
		<%	For Each Flag As String In Me.GetRequiredFlagsByAppID(Request.QueryString("ID"))%>
		<li><a target="_blank" href="users_batchuserflageditor.aspx?AppID=<%=Request.QueryString("ID")%>&Flag=<%=server.urlencode(Flag)%>&EditMode=2">Edit Flag '<%=Server.HtmlEncode(Flag)%>' with Batchuserflageditor</a></li>
		<%	Next%>
	</ul>
</p>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="apps.aspx" ID="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
