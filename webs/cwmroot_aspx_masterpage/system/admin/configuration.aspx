<%@ Page Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.ConfigurationOverview" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify user account" id="cammWebManager"
    SecurityObject="System - User Administration - ServerSetup" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>

<script runat="server">
	
</script>

<!--#include virtual="/sysdata/includes/standardtemplate_top_wo_form.aspx"-->
<h3>
    <font face="Arial">Administration - Advanced configuration</font></h3>
    <%'TODO: Strukturierte und benutzerfreundliche Auflistung der versch. Konfigurationsmöglichkeiten erstellen %>
    
    <a href="configuration_logging.aspx">Logging</a><br>
	<a href="configuration_passwordencryption.aspx">Password Encryption</a><br>
	<a href="configuration_dataprotection.aspx">Data protection rules</a><br>

<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="about.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_wo_form.aspx"-->
