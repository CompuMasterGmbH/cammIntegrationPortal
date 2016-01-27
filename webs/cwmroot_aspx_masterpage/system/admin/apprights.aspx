<%@ Page language="VB" EnableViewState="False" Inherits="CompuMaster.camm.WebManager.Pages.Administration.AppRightsList" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Authorizations" id="cammWebManager" SecurityObject="System - User Administration - Authorizations" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

<html>
<head>
    <style>
        TD{vertical-align:top;}
        .boldFont{font-face:Arial;font-size:12;font-weight:bold;}
        .boldFontHeader{font-face:Arial;font-size:12;font-weight:bold;background-color:#E1E1E1;}
        .normalFont{font-face:Arial;font-size:12;}
        .boldFontSize1{font-face:Arial;font-size:6;font-weight:bold;}
    </style>
</head>
<body>
<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
	<TBODY>		
		<TR>
			<TD ColSpan="<%= iFieldCount %>">
				<h3><font face="Arial">Administration - Authorizations</font></h3>
				<table width="100%" cellspacing="0" cellpadding="0" border="0">
					<tr>
						<td width="31%" id="tdSearchApp" runat="server" >
							Search application:
							<asp:Textbox EnableViewState="True" BorderColor="#00446E" BorderStyle="Dotted" BorderWidth="1px" BackColor="White" runat="server" id="txtSearchApp" />&nbsp;&nbsp;
						</td>
						<td>
							<input type="text" style="display:none" />
							Search groups/users:
							<asp:Textbox EnableViewState="True" BorderColor="#00446E" BorderStyle="Dotted" BorderWidth="1px" BackColor="White" runat="server" id="txtSearchUser" /> 
							<asp:button runat="server" id="btnSubmit" text="Go" BorderColor="#00446E" BorderStyle="Solid" BorderWidth="1px" style="CURSOR:pointer" BackColor="White" />
						</td>
					</tr>
				</table>	
			</TD>
		</TR>
		<TR>
			<TD  align="right" runat="server" id="tdAddLinks" />
		</TR>

			<asp:Repeater id="rptAppList" runat="Server" >
				<ItemTemplate>					
					<TR style="padding-top:20px;margin:0px;">
						<TD style="margin:0px;">
							<TABLE WIDTH="700" CELLSPACING="0" CELLPADDING="2" border="0" style="margin:0px;display:inline;" align="left">
								<TR>
									<TD BGCOLOR="#C1C1C1" Width="30">
										<P class="boldFont">
											<a class="None" name="Application<%# container.dataitem("ID_Application") %>" />
											<a id="ancAuthsAsAppID" runat="server">ID<br>
											<%# cammWebManager.System_Nz(container.dataitem("ID_Application")) %>
											<span id="gcDisabled" runat="server" title="Disabled" style="display:none">(D)</span></a>
										</P>
									</TD>
									<TD BGCOLOR="#C1C1C1">
										<P title="<%# container.dataitem("NavURL") %>">
											<nobr>
											<asp:hyperlink runat="server" id="hypTitle" CssClass="boldFont" /> 
											<em><asp:hyperlink runat="server" id="ancAdditionalAuth" CssClass="normalFont" /></em>
										</P>
									</TD>
									<TD BGCOLOR="#C1C1C1" Width="130">
										<P class="boldFont">Release date<br>
										    <%# Server.HtmlEncode(cammWebManager.System_Nz(container.dataitem("AppReleasedOn"))) %> 
										    
										</P>
									</TD>
									<TD BGCOLOR="#C1C1C1" Width="150">
										<P class="boldFont">Released by<br>
										    <a runat="server" id="ancReleasedBy" />
										</P>
									</TD>
								</TR>
								<TR runat="server" id="trAddUserGroupDetails">
									<TD runat="server" id="tdAddUserGroupDetails" colspan="4"></TD>
								</TR>
								
								<TR runat="server" id="trAddUserDetails">
									<TD runat="server" id="tdAddUserDetails" colspan="4"></TD>
								</TR>
							</TABLE>
							<table BGCOLOR="#C1C1C1" CELLSPACING="0" CELLPADDING="2" border="0" style="margin:0px;display:inline;" Width="110">
								<tr>
									<TD>
										<P class="boldFont">
											Actions<br />
											<a runat="server" id="ancAddGroupShowDetails" />
											<a runat="server" id="ancAddUser" />
											<a runat="server" id="ancSecurity" title="Adjust administrative delegates">Security</a>
											<asp:hyperlink id="ancTransmission" runat="server" />
											<asp:hyperlink runat="server" id="ancExport" />
											<asp:hyperlink runat="server" id="ancBatchUserFlagEditor" text="Batch flag-edit" visible="false" />
										</P>
									</td>
								</tr>
							</table>
						</TD>
					</TR>
					
					
				</ItemTemplate>
			</asp:Repeater>
		
			<TR><TD runat="server" id="tdErrMsg"></TD></TR>
		</TBODY>
	</TABLE>
	
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu  id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
</body>
</html>