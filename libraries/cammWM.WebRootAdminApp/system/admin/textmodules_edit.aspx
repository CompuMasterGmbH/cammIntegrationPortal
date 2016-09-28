<%@ Page Language="vb" ValidateRequest="False" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Modules.Text.Administration.Pages.TextModules_Edit"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Register TagPrefix="cammWebEdit" Namespace="CompuMaster.camm.SmartWebEditor.Controls" Assembly="cammWM.SmartEditor" %>
<camm:WebManager id="cammWebmanager" runat="server" SecurityObject="System - TextModules" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
		<script language="javascript" type="text/javascript">

function ToggleTextBlockVisibility() {
	if ( document.forms[0].RadioButtonHtmlText.checked ) {
		ToggleVisibility('PanelEditorHtml');
		ToggleVisibility('PanelPlainText');
	}
	if ( document.forms[0].RadiobuttonPlainText.checked ) {
		ToggleVisibility('PanelEditorHtml');
		ToggleVisibility('PanelPlainText');
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
					<td width="100%" height="30">
						<table border="0" cellpadding="2" cellspacing="0" width="100%">
							<tr valign="top">
								<td><font face="Arial" size="3"><b><asp:Label ID="LabelTextAdministration" Runat="server"></asp:Label></b></font></td>
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
					<td height="30"><asp:Label ID="LabelTitle" Runat=server></asp:Label></td>
				</tr>
				<tr>
					<td>
						<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0">
							<tr>
								<TD colSpan="2"><nobr>
										<asp:Label id="LabelVersion" Runat="server" ForeColor="#0000ff"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
										<asp:Label id="LabelDropDownlistVersion" Runat="server"></asp:Label>:&nbsp;
										<asp:DropDownList id="DropDownListVersion" Width="150" Runat="server" AutoPostBack="True"></asp:DropDownList>
									</nobr>
								</TD>
							</tr>
							<TR>
								<TD colSpan="2"><nobr>
										<asp:RadioButton ID="RadioButtonHtmlText" GroupName="HtmlOrPlainText" Runat="server"></asp:RadioButton>
										<asp:RadioButton ID="RadiobuttonPlainText" GroupName="HtmlOrPlainText" Runat="server"></asp:RadioButton>
									</nobr>
								</TD>
							</TR>
							<TR>
								<TD align="right" width="5%">
									<asp:Label id="LabelName" Runat="server"></asp:Label>:</TD>
								<TD>
									<asp:Label id="LabelKey" Runat="server" ></asp:Label></TD>
							</TR>
							<TR valign="top">
								<TD align="right">
									<asp:Label id="LabelValue" Runat="server"></asp:Label>:</TD>
								<TD height="350">
									<asp:Panel ID="PanelPlainText" Runat="server">
										<asp:TextBox id="TextboxEditValue" Runat="server" TextMode="MultiLine" Columns="70" Rows="12"></asp:TextBox>
									</asp:Panel>
									<asp:Panel ID="PanelEditorHtml" Runat="server">
										<cammWebEdit:SmartPlainHtmlEditor id="EditorHtml" style="FONT-SIZE: smaller" Runat="server" hasPermission="True" 
											Editable="True" width="580px" height="200px" ToolsWidth="580px" ToolsHeight="70px" SaveInFile="False"
											ImagesPaths="" UploadImagesPaths="" DeleteImagesPaths="" ToolsOnPage="True" MaxDocumentSize="1512000"
											Scheme="Custom"></cammWebEdit:SmartPlainHtmlEditor>
									</asp:Panel>
								</TD>
							</TR>
							<TR>
								<TD align="right">
								</TD>
								<TD>
									<asp:Button id="ButtonSave" Width="110" Runat="server" o></asp:Button>
									<asp:Button id="ButtonPublish" Width="110" Runat="server"></asp:Button>
									<asp:Button id="ButtonPreview" Width="110" Runat="server"></asp:Button>
									<asp:Button id="ButtonBack" Width="110" Runat="server"></asp:Button></TD>
							</TR>
						</TABLE>
					</td>
				</tr>
				<tr>
					<td align="right">
						<% if IsDevelopmentVersion then%>
						[Form: 108]&nbsp;&nbsp;
						<%end if%>
					</td>
				</tr>
			</table>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->