<%@ Page ValidateRequest="False" Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.SystemCheckup" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify application" id="cammWebManager"
    SecurityObject="System - User Administration - ServerSetup" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<h3>Check file consistency</h3>
<asp:label runat="server" id="infolbl" />
<br />
<table runat="server" id="resultTable">
<tr>
<td><b>File</b></td><td><b>Found</b></td>
</tr>
</table>

<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="about.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->