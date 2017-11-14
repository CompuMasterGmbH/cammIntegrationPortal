<%@ Page language="VB" ValidateRequest="False" Inherits="CompuMaster.camm.WebManager.Pages.Administration.CleanUpUserAuthorization" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Create new user authorization" id="cammWebManager" SecurityObject="System - User Administration - Authorizations" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

		<h3><font face="Arial">Administration - Cleanup user authorization</font></h3>
		<link href="/system/admin/style/baseStyle.css" type="text/css" rel="stylesheet" />
		<link href="/system/admin/style/combobox.css" type="text/css" rel="stylesheet" />
		<p><font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErr" /></font></p>
				<a href="apprights.aspx?Application=<%=CInt(Request.QueryString("ID"))%>&AuthsAsAppID=<%=CInt(Request.QueryString("AuthsAsAppID"))%>">Retun to Administration - Authorizations</a><br/><br/>
		<asp:panel runat="server" id="pnlAuthCleanUp">
			<asp:label runat="server" id="InfoLbl" />
		</asp:panel>
		<p><asp:Button runat="server" id="cleanUpBtn" Text="Cleanup all user authorization" /></p>
		<br />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="apprights.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->