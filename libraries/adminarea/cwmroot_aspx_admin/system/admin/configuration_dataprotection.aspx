<%@ Page Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.ConfigurationDataProtection" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify data protection rules" id="cammWebManager"
    SecurityObject="System - User Administration - ServerSetup" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>

<script runat="server">
	
</script>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<h3>
<font face="Arial">Administration - Data protection rules</font></h3>
<asp:label runat="server" id="lblMsg" forecolor="green" />
Choose the types that should be <u>deleted</u> in the log table after user deletion<br>
<asp:Placeholder runat="server" id="ltrlTypeList" />
<br>	
Delete deactivated users after<br>
<asp:TextBox runat="server" id="txtBoxDeleteAfterDays" /> days<br>
<asp:CompareValidator runat="server" id="validatetxtBoxDeleteAfterDays" ControlTovalidate="txtBoxDeleteAfterDays" ErrorMessage="field must be a number" Type="Integer" Operator="DataTypeCheck" />
<br>

Delete mails in mail queue after n days (affects cancelled, ultimately failed and successful messages)<br>
<asp:TextBox runat="server" id="txtBoxDeleteMailsAfterDays" /> days<br>
<asp:CompareValidator runat="server" id="validatetxtBoxDeleteMailsAfterDays" ControlTovalidate="txtBoxDeleteMailsAfterDays" ErrorMessage="field must be a number" Type="Integer" Operator="DataTypeCheck" />
<br>

Anonymize IP addresses after<br>
<asp:TextBox runat="server" id="txtBoxAnonymizeIPs" /> days<br>
<asp:CompareValidator runat="server" id="validatetxtBoxAnonymizeIPs" ControlToValidate="txtBoxAnonymizeIPs" ErrorMessage="field must be a number" Type="Integer" Operator="DataTypeCheck" />
<br><br>

<asp:Button runat="server" id="btnSaveSettings" Text="Save settings" />

<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="configuration.aspx" id="cammWebManagerAdminMenu"
    runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
