<%@ Page ValidateRequest="False" Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.CheckNotRequiredAdditionalFlags" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify application" id="cammWebManager"
    SecurityObject="System - User Administration - ServerSetup" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<h3>Additional flags not required by any security object</h3>
<asp:datagrid runat="server" id="NotRequiredFlags" autogeneratecolumns="false" width="600">
<HeaderStyle backcolor="#c1c1c1" />
<AlternatingItemStyle backcolor="#e1e1e1" />
<columns>
<asp:BoundColumn HeaderText="Not required flag" DataField="FlagName" />
<asp:HyperlinkColumn HeaderText="Action" DataNavigateUrlField="FlagName" text="List users" DataNavigateUrlFormatString="users_batchuserflageditor.aspx?Flag={0}&EditMode=1" DataTextFormatString="{0:c}"  />
</columns>
</asp:datagrid>

<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="apps.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->