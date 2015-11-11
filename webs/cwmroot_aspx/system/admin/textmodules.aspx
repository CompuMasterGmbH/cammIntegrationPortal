<%@ Page Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Modules.Text.Administration.Pages.TextModules"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Register TagPrefix="TextModule" TagName="TextBlock" Src="textmodules_textblock.ascx" %>
<%@ Register TagPrefix="TextModule" TagName="Variable" Src="textmodules_variable.ascx" %>
<%@ Register TagPrefix="TextModule" TagName="Template" Src="textmodules_template.ascx" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<camm:WebManager id="cammWebmanager" runat="server" SecurityObject="System - TextModules" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<script language="javascript" type="text/javascript">
function ValidateAddVariable() {
	return Validate(document.forms[0].CtrlVariable_TextBoxNewVariableName);
}

function ValidateAddTextblock() {
	return Validate(document.forms[0].CtrlTextBlock_TextBoxNewTextblockName);
}

function ValidateAddTemplate() {
	if ( !Validate(document.forms[0].CtrlTemplate_TextBoxNewTemplateName) ) { 
		return false;
	}
	return true;
}

function Validate(textbox) {
	if ( ((textbox.value) == '') ) 
	{
		alert('Please enter valid value!');
		textbox.focus();
		textbox.select();
		return false;
	}
	
	if ( ((textbox.value) != '') ) 
	{
		var key = textbox.value;
		var invalidCharacters = "\"!§$%&/\\()=?'|#+~-:;,.><";
		for (var i = 0; i < key.length; i++) {
			var ch = key.charAt(i);
			if ( invalidCharacters.indexOf(ch) >= 0 ) {
				alert('Please do not use special characters.\nAllowed special charaters ( _ )');
				textbox.focus();
				textbox.select();
				return false;
			}
		}
		if ( ( key.indexOf('!') > 0 ) || ( key.indexOf('"') > 0 ) || ( key.indexOf('§') > 0 ) || ( key.indexOf('$') > 0 )  || ( key.indexOf('&') > 0 )  || ( key.indexOf('/') > 0 ) || ( key.indexOf('(') > 0 ) || ( key.indexOf(')') > 0 )  || ( key.indexOf(')') > 0 ) || ( key.indexOf('=') > 0 ) || ( key.indexOf('=') > 0 ) || ( key.indexOf('?') > 0 ) || ( key.indexOf('+') > 0 ) || ( key.indexOf('-') > 0 ) || ( key.indexOf('#') > 0 ) || ( key.indexOf('|') > 0 ) || ( key.indexOf('~') > 0 ) ) {
		}
		var x = parseInt(key.charAt(0));
		if ( !isNaN(x) ) {
			alert('Please enter valid value.');
			textbox.focus();
			textbox.select();
			return false;
		}
		for (var j = 0; j < keyArray.length ; j++ ) {
			if ( keyArray[j] == key.toLowerCase() ) {
				alert('Name "' + key + '" existiert bereits.');
				textbox.focus();
				textbox.select();
				return false;
			}
		}
	}
	
	return true;	
}

function ToggleTextBlockVisibility() {
	if ( document.forms[0].CtrlTextBlock_RadioButtonHtmlText.checked ) {
		ToggleVisibility('CtrlTextBlock_PanelHtmlEditor');
		ToggleVisibility('CtrlTextBlock_PanelPlainText');
	}
	if ( document.forms[0].CtrlTextBlock_RadiobuttonPlainText.checked ) {
		ToggleVisibility('CtrlTextBlock_PanelHtmlEditor');
		ToggleVisibility('CtrlTextBlock_PanelPlainText');
	}
	return true;
}

function ToggleVisibility(panel) {
	if (document.getElementById)
	{
		var myStyle = document.getElementById(panel).style;
		if (myStyle.display == 'none' ) {
			myStyle.display = 'block';
		}
		else if (myStyle.display == 'block' ) {
			myStyle.display = 'none';
		}
	}
	else if (document.all)
	{
		var myStyle = document.all[panel].style;
		if (myStyle.display == 'none' ) {
			myStyle.display = 'block';
		}
		else if (myStyle.display == 'block' ) {
			myStyle.display = 'none';
		}
	}
	else if (document.layers)
	{
		var myStyle = document.layers[panel].style;
		if (myStyle.display == 'none' ) {
			myStyle.display = 'block';
		}
		else if (myStyle.display == 'block' ) {
			myStyle.display = 'none';
		}
	}
}

		</script>
			<table border="0" cellpadding="3" cellspacing="0" width="100%">
				<tr>
					<td colspan="2" width="100%">
						<table border="0" cellpadding="2" cellspacing="0" width="100%">
							<tr valign="top">
								<td><P><font face="Arial" size="3"><b><asp:Label ID="LabelTextAdministration" Runat="server"></asp:Label></b></font></P>
								</td>
								<td></td>
								<td align="right">
								<% if IsDevelopmentVersion then%>
									Version 2.0
								<%end if%>
								</td>
							</tr>
                        <tr valign="top">
                            <td colspan="3" align="right">
                                <a href="textmodules_overview.aspx">Textmodules overview</a>
                            </td>
                        </tr>							
						</table>
					</td>
				</tr>
				<tr>
					<td><COMPONENTART:TABSTRIP id="TabStrip" runat="server" CssClass="TopGroup" DefaultItemLookId="DefaultTabLook" EnableViewState="False"
							DefaultSelectedItemLookId="SelectedTabLook" DefaultDisabledItemLookId="DisabledTabLook" DefaultGroupTabSpacing="1"
							ImagesBaseUrl="images/" MultiPageId="MultiPage">
							<Tabs>
								<componentart:TabStripTab ID="TabStripTextModule" PageViewId="PageViewTextModule" CausesValidation="False" />
								<componentart:TabStripTab ID="TabStripVariable" PageViewId="PageViewVariable" CausesValidation="False" />
								<componentart:TabStripTab ID="TabStripTextBlock" PageViewId="PageViewTextBlock" CausesValidation="False" />
							</Tabs>
							<itemlooks>
								<componentart:itemlook LookId="DefaultTabLook" CssClass="DefaultTab" HoverCssClass="DefaultTabHover" LabelPaddingLeft="10"
									LabelPaddingRight="10" LabelPaddingTop="5" LabelPaddingBottom="4" />
								<componentart:itemlook LookId="SelectedTabLook" CssClass="SelectedTab" LabelPaddingLeft="10" LabelPaddingRight="10"
									LabelPaddingTop="4" LabelPaddingBottom="4" />
								<componentart:itemlook LookId="DisabledTabLook" CssClass="DisabledTab" LabelPaddingLeft="10" LabelPaddingRight="10"
									LabelPaddingTop="4" LabelPaddingBottom="4" />
							</itemlooks>
						</COMPONENTART:TABSTRIP>
					</td>
					<td align="right"></td>
				</tr>
				<tr>
					<td colspan="2">
						<COMPONENTART:MULTIPAGE id="MultiPage" runat="server" width="100%" CssClass="MultiPage">
							<ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageViewTextModule">
								<table border="0" cellpadding="5" cellspacing="0" width="100%">
									<tr>
										<td>
											<TextModule:Template runat="server" ID="CtrlTemplate" /></td>
									</tr>
								</table>
							</ComponentArt:PageView>
							<ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageViewVariable">
								<table border="0" cellpadding="5" cellspacing="0" width="100%">
									<tr>
										<td>
											<TextModule:Variable runat="server" ID="CtrlVariable" /></td>
									</tr>
								</table>
							</ComponentArt:PageView>
							<ComponentArt:PageView CssClass="PageContent" runat="server" ID="PageViewTextBlock">
								<table border="0" cellpadding="5" cellspacing="0" width="100%">
									<tr>
										<td>
											<TextModule:TextBlock runat="server" ID="CtrlTextBlock" /></td>
									</tr>
								</table>
							</ComponentArt:PageView>
						</COMPONENTART:MULTIPAGE>
					</td>
				</tr>
			</table>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->