<%@ Page Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.ConfigurationAllowedValuesUserProfileFieldCountry" %>

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
<font face="Arial">Administration - Allowed values - User profile field: Country</font></h3>
<asp:label runat="server" id="LabelInfoMessage" forecolor="green" /><asp:label runat="server" id="LabelErrorMessage" forecolor="red" />

<p><b>Allowed values</b><br>
<asp:RadioButtonList ID="RadioButtonListSetupOfAllowRule" runat="server" AutoPostBack="True">
	<asp:ListItem Value="0" Text="Allow all values" />
	<asp:ListItem Value="1" Text="Allow specified values only" />
</asp:RadioButtonList>
</p>

<asp:panel runat="server" id="PanelPage" Visible="False">
<p><b>Please specify the allowed values:</b><br />
<asp:RadioButtonList ID="RadioButtonListValueSeparator" runat="server" AutoPostBack="True" RepeatDirection="Horizontal">
	<asp:ListItem Value="0" Text="Show each value in a separate line" Selected />
	<asp:ListItem Value="1" Text="Show values separated by pipe character (&quot;|&quot;)" />
</asp:RadioButtonList>
<asp:TextBox runat="server" ID="TextboxAllowedValues" TextMode="MultiLine" style="width:100%; box-sizing: border-box; height: 150px; resize: none;" /><br />
<asp:Checkbox runat="server" ID="CheckboxAllowEmptyValue" Text="Allow empty value" />
</p>

<p><b>Quick import of allowed values</b><br>
PLEASE NOTE: before importing these external data sources, you have to accept the license of the external party.
<ul>
<li>https://github.com/datasets/country-codes, column &quot;CLDR display name&quot; <asp:Button runat="server" id="ButtonImportGithubDatasetsCountryCodesByName" text="Import" /></li>
<li>https://github.com/datasets/country-codes, column &quot;ISO3166-1-Alpha-2&quot; <asp:Button runat="server" id="ButtonImportGithubDatasetsCountryCodesByISO3166_1_Alpha2" text="Import" /></li>
</ul>
</p>

</asp:panel>

<asp:Button runat="server" id="ButtonSave" text="Save" />

<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="configuration.aspx" id="cammWebManagerAdminMenu"
    runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
