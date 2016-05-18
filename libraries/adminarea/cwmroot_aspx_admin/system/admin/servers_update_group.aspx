<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.UpdateServerGroup"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify server group" id="cammWebManager" SecurityObject="System - User Administration - Applications" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<h3><font face="Arial">Administration - Modify server group</font></h3>
<p><font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErrMsg" /></font></p>
<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
	<TBODY>
		<TR>
			<TD vAlign="top">
				
					<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
						<TBODY>
							<TR><TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Server details</b></FONT></P></TD></TR>
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Server group ID</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="lblFieldID" /></FONT></P></TD>
							</TR>
							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Official title in continuous text</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:textbox id="txtFieldServerGroup" runat="server" style="width:200px"/></FONT></P></TD>
							</TR>
							
							<TR title="You've got the possibilty to set up a separate title for the navigation frame because you might run in &quot;optical trouble&quot; with too long titles.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Displayed title in the navigation frame</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaNavTitle" runat="server" /></FONT></P></TD>
							</TR>
							
							<TR><TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD></TR>
							<TR><TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Special user groups</b></FONT></P></TD></TR>
							
							<TR title="Applications authorized for this anonymous user group are available for all visitors without the need for a login.">
								<TD VAlign="Top" WIDTH="160"><P>
									<FONT face="Arial" size="2">Anonymous access</FONT><br><em>
									<FONT face="Arial" size="2">This user group collects all authorizations for users who aren't logged in.</font></em></P>
								</TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
									<asp:HyperLink id="hypIdGroupAnonymous" runat="server" />
									<INPUT TYPE="HIDDEN" id="hiddenTxt_GroupAnonymous" runat="server" />
								</TD>
							</TR>
							
							<TR title="All applications authorized for this guest login group are available for all users who have created an account. The security administraton team only needs to set up authorizations for additional applications or add memberships to other user groups.">
								<TD VAlign="Top" WIDTH="160">
									<P><FONT face="Arial" size="2">Guest login</FONT><br><em>
									<FONT face="Arial" size="2">This is the group with authorizations for all logged in users.</font></em></P>
								</TD>
								<TD VAlign="Top" Width="240">
									<P><FONT face="Arial" size="2"><a runat="server" id="ancGroupUpdate" />
									<asp:HyperLink id="hypIdGroupPublic" runat="server" />
									</FONT></P>
									<INPUT TYPE="HIDDEN" Id="hiddenTxt_ID_Group_Public" Runat="server" VALUE="">
								</TD>
							</TR>
							
							<TR title="Every user has got an access level. Set up here which access level a user gets as default.">
								<TD VAlign="Top" WIDTH="160">
									<P><FONT face="Arial" size="2">Standard access level</FONT><br><em>
									<FONT face="Arial" size="2">This default value will be used when a visitor creates his own user account.</font></em></P>
								</TD>
								<TD VAlign="Top" Width="240">
									<P><FONT face="Arial" size="2">
									<asp:DropDownList style="width: 200px" runat="server" id="cmbAccessLevelDefault" />
									
								</TD>
							</TR>
							
							<TR><TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD></TR>
							<TR>
								<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Essential servers</b></FONT></P></TD>
							</TR>
							
							<TR title="The master server processes all the logins from users and its name will be displayed in the browser's address bar.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Master server</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
									<asp:DropDownList style="width: 200px" runat="server" id="cmbMasterServer" />
									
								</TD>
							</TR>
							
							
							
							<TR title="The administration server don't need to be a member of the current server group. It is recommended to run an admin server behind a firewall. Servers in the world wide web are potentially more unsecure than a protected intranet server.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Administration server</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
									<asp:DropDownList  style="width: 200px" id="cmbUserAdminServer" runat="server" />
									
								</TD>
							</TR>
							
							<TR><TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD></TR>
							
							<TR><TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Logos</b></FONT></P></TD></TR>
							
							<TR title="This logo will be displayed e. g. at the login form.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Logo of this website area (100 x 94 pixels)</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaButton" runat="server" /></FONT></P></TD>
							</TR>
							
							<TR title="This image can be referenced by text documents, e. g. the welcome page.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Logo of this website area (243 x 242 pixels)</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaImage" runat="server"/></FONT></P></TD>
							</TR>
							
							<TR><TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD></TR>
							<TR><TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Copyright and publisher details</b></FONT></P></TD></TR>
							
							<TR title="Example: YourCompany Ltd.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Formal company title</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" type="text" id="txtAreaCompanyFormerTitle" runat="server"/></FONT></P></TD>
							</TR>
							
							
							
							<TR title="Example: YourCompany">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Company title (short version)</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaCompanyTitle"  runat="server"/></FONT></P></TD>
							</TR>
							
							<TR title="This is the year since you've reserved the copyright.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Copyright reserved since</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaCopyRightSinceYear"  runat="server"/></FONT></P></TD>
							</TR>
							
							<TR title="Example: http://www.yourcompany.com/">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Official website address</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaCompanyWebSiteURL"  runat="server" /></FONT></P></TD>
							</TR>
							
							<TR title="Example: YourCompany Ltd. - Homepage">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Official website title</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaCompanyWebSiteTitle"  runat="server"/></FONT></P></TD>
							</TR>
							
							
							
							
							<TR><TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD></TR>
							<TR title="The security administration team manages the user administrations and memberships.">
								<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Security administration contact</b></FONT></P></TD>
							</TR>
							
							<TR title="The security administration team manages the user administrations and memberships.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">e-mail address</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaSecurityContactEMail"  runat="server"/></FONT></P></TD>
							</TR>
							
							<TR title="The security administration team manages the user administrations and memberships.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Title/name</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaSecurityContactTitle"   runat="server"/></FONT></P></TD>
							</TR>
							
							<TR><TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD></TR>
							<TR title="Feedbacks and inquiries about the content will be forwarded to the content manager.">
								<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Content Manager contact</b></FONT></P></TD>
							</TR>
							
							
							
							<TR title="Feedbacks and inquiries about the content will be forwarded to the content manager.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">e-mail address</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaContentManagementContactEMail"   runat="server"/></FONT></P></TD>
							</TR>
							
							<TR title="Feedbacks and inquiries about the content will be forwarded to the content manager.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Title/name</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaContentManagementContactTitle"  runat="server" /></FONT></P></TD>
							</TR>
							
							<TR><TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD></TR>
							<TR title="The developer will be informed in cases of errors.">
								<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Developer contact</b></FONT></P></TD>
							</TR>
							
							
							
							<TR title="The developer will be informed in cases of errors.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">e-mail address</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaDevelopmentContactEMail"  runat="server"/></FONT></P></TD>
							</TR>
							
							<TR title="The developer will be informed in cases of errors.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Title/name</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaDevelopmentContactTitle"  runat="server" /></FONT></P></TD>
							</TR>
							
							<TR><TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD></TR>
							<TR title="Every e-mail with no further specification or e-mails sent by the camm WebManager system will contain this e-mail address.">
								<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>General inquiries contact</b></FONT></P></TD>
							</TR>
							
							
							
							<TR title="Every e-mail with no further specification or e-mails sent by the camm WebManager system will contain this e-mail address.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">e-mail address</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaUnspecifiedContactEMail"  runat="server"/></FONT></P></TD>
							</TR>
							
							<TR title="Every e-mail with no further specification or e-mails sent by the camm WebManager system will contain this e-mail address.">
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Title/name</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox style="width:200px" id="txtAreaUnspecifiedContactTitle"  runat="server"/></FONT></P></TD>
							</TR>
							
							<TR><TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD></TR>
							<TR>
								<TD VAlign="Top" WIDTH="160"><P>
								<asp:Button id="btnSubmit" runat="server" Text="Update server group"/>
								<FONT face="Arial" size="2"></FONT></P></TD>
								<TD VAlign="Top" Width="240"><P> &nbsp;</P></TD>
							</TR>
							
		        </TBODY></TABLE></TD></TR>
	      </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="servers.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
