<%@ Page Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.ConfigurationPasswordEncryption" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Advanced Configuration" id="cammWebManager"
    SecurityObject="System - User Administration - ServerSetup" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>

<script runat="server">
	
</script>

<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<h3>
<font face="Arial">Administration - Password encryption configuration</font></h3>
<asp:label runat="server" id="lblMsg" forecolor="green" />
<asp:panel runat="server" id="pnlPage">
<br /><br />
<b>Algorithm</b><br>
Choose the algorithm the user passwords must be encrypted/hashed with:<br />
<asp:DropDownList runat="server" id="cmbAlgorithms"/> <asp:Button runat="server" id="btnSubmitAlgo" text="Update" />
<hr>
<b>Recovery behavior</b><br>
<asp:DropDownList runat="server" id="cmbResetBehavior"/> <asp:Button runat="server" id="btnSubmitRecovery" text="Update" />
<hr>
<b>Convert existing passwords</b><br>
Usually, existing passwords for users will be left untouched until an administrator sets the password or the user changes it himself.<br>
<a href="passwordconvert.aspx">Begin conversion of all existing user passwords</a>
</asp:panel>

<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="configuration.aspx" id="cammWebManagerAdminMenu"
    runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
