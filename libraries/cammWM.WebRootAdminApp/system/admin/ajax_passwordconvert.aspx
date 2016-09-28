<%@ Page Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.PasswordConvertAjax" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify user account" id="cammWebManager"
    SecurityObject="System - User Administration - ServerSetup" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<% Response.CacheControl = "no-cache" %>
<% Response.AddHeader("Pragma", "no-cache") %>
<% Response.Expires = -1 %>