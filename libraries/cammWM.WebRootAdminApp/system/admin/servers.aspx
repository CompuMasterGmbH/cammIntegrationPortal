<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.ServerList" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Server groups" id="cammWebManager" SecurityObject="System - User Administration - ServerSetup" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

	<h3><font face="Arial">Administration - Server groups</font></h3>
	<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
		<TBODY>
			<TR>
				<TD vAlign="top">
					<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
						<TBODY>
						
					<asp:Repeater id="rptServerList" runat="server">
						<ItemTemplate>
							<TR runat="server" id="trShowDetails">
								<td>
									<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
										<TR runat="server" id="trAddBlank"><TD>&nbsp;</TD></TR>
										<TR>
											<TD>
												<TABLE WIDTH="100%" BGCOLOR="#C1C1C1" CELLSPACING="0" CELLPADDING="2" border="0" bordercolor="#FFFFFF">
													<TR>
														<TD VAlign="Top" Width="30"><P><FONT face="Arial" size="2"><b>ID<a class="None"  runat="server" id="ancGroupID" /><br><asp:Label runat="server" id="lblGroupID" />&nbsp;</b></FONT></P></TD>
														<TD VAlign="Top" Width="290"><P><FONT face="Arial" size="2"><b><nobr><a runat="server" id="ancServerGroup" /></nobr><br><nobr><asp:Label runat="server" id="lblAreaNavTitle" /></nobr>&nbsp;</b><br /><em>Guest user without login: <asp:Hyperlink id="hypGroupAnonymousName" runat="server" /></em></FONT></P></TD>
														<TD VAlign="Top" Width="290"><P><FONT face="Arial" size="2"><em>Admin Server: <a id="ancAdminServer" runat="server" />&nbsp;<br>Master: <a runat="server" id="ancMasterServer" />&nbsp;<br>Guest user with login: <asp:Hyperlink id="hypGroupPublicName" runat="server" /></em></FONT></P></TD>
														<TD VAlign="Top"><P><FONT face="Arial" size="2"><nobr><a href="servers_new_group.aspx">New Server Group</a>&nbsp;</nobr><br><nobr><a runat="server" id="ancDeleteServerGroup" />&nbsp;</nobr></FONT></P></TD>
													</TR>
												</TABLE>
											</TD>
										</TR>
										<TR>
											<TD>
												<TABLE WIDTH="100%" CELLSPACING="0" CELLPADDING="3" border="0" bordercolor="#FFFFFF">
													<TR>
														<TD WIDTH="30"> &nbsp;</TD>
														<TD WIDTH="60" BGCOLOR="#E1E1E1"><P><FONT face="Arial" size="2"><b>ID</b></FONT></P></TD>
														<TD WIDTH="170" BGCOLOR="#E1E1E1"><P><FONT face="Arial" size="2"><b>Access Levels</b>&nbsp;</FONT></P></TD>
														<TD WIDTH="200" BGCOLOR="#E1E1E1"><P><FONT face="Arial" size="2"><b>Remarks</b>&nbsp;</FONT></P></TD>
														<TD BGCOLOR="#E1E1E1"><P><FONT face="Arial" size="2"><b>Action</b>&nbsp;<nobr><a runat="server" id="ancAdd">Add</a></nobr></FONT></P></TD>
													</TR>
												</TABLE>
											</TD>
										</TR>
										
										<asp:Repeater id="rptServerSubList" runat="server">
											<ItemTemplate>
										<TR>
											<TD>
												<TABLE WIDTH="100%" CELLSPACING="0" CELLPADDING="3" border="0" bordercolor="#FFFFFF">
													<TR>
														<TD VAlign="Top" WIDTH="30"> &nbsp;</TD>
														<TD VAlign="Top" WIDTH="60"><P><FONT face="Arial" size="2"><%# container.dataitem("AccessLevels_ID") %>&nbsp;</FONT></P></TD>
														<TD VAlign="Top" WIDTH="170"><P><FONT face="Arial" size="2"><%# container.dataitem("AccessLevels_Title") %></FONT></P></TD>
														<TD VAlign="Top" WIDTH="200"><P><FONT face="Arial" size="2"><%# container.dataitem("AccessLevels_Remarks") %>&nbsp;</FONT></P></TD>
														<TD><P><FONT face="Arial" size="2"><a href="servers_delete_accesslevelrelation.aspx?ID=<%# container.dataitem("ID") %>">Deny access</a>&nbsp;</FONT></P></TD>
													</TR>
												</TABLE>
											</TD>
										</TR>
											</ItemTemplate>
										</asp:Repeater>
									
										<TR runat="server" id="trShowMsg" style="display:none">
											<TD>
												<TABLE WIDTH="100%" CELLSPACING="0" CELLPADDING="3" border="0" bordercolor="#FFFFFF">
													<TR>
														<TD VAlign="Top" WIDTH="30"> &nbsp;</TD>
														<TD><FONT FACE="Arial" size="2"><em>To get a successfull connection to this server group you have to add at least one access level.</em></font></TD>
													</TR>
												</TABLE>
											</TD>
										</TR>

										<TR>
											<TD>
												<TABLE WIDTH="100%" CELLSPACING="0" CELLPADDING="3" border="0" bordercolor="#FFFFFF">
													<TR>
														<TD WIDTH="30"> &nbsp;</TD>
														<TD WIDTH="60" BGCOLOR="#E1E1E1"><P><FONT face="Arial" size="2"><b>Srv. ID</b></FONT></P></TD>
														<TD WIDTH="170" BGCOLOR="#E1E1E1"><P><FONT face="Arial" size="2"><b>Server Name</b> &nbsp;</FONT></P></TD>
														<TD WIDTH="200" BGCOLOR="#E1E1E1"><P><FONT face="Arial" size="2"><b>IP / Host Header</b> &nbsp;</FONT></P></TD>
														<TD BGCOLOR="#E1E1E1"><P><FONT face="Arial" size="2"><b>Action</b>&nbsp;<nobr><a runat="server" id="ancNew">New</a></nobr></FONT></P></TD>
													</TR>
												</TABLE>
											</TD>
										</TR>
									</table>
								</td>
							</tr>

							<TR>
								<TD>
									<TABLE WIDTH="100%" CELLSPACING="0" CELLPADDING="3" border="0" bordercolor="#FFFFFF">
										<TR>
											<TD VAlign="Top" WIDTH="30"> &nbsp;</TD>
											<TD VAlign="Top" WIDTH="60"><P><FONT face="Arial" size="2"><asp:Label id="lblMasterServerID" runat="server" />&nbsp;<span runat="server" id="gcDisabled" />&nbsp;</FONT></P></TD>
											<TD VAlign="Top" WIDTH="170"><P><FONT face="Arial" size="2"><a runat="server" id="ancMemberServerDesc" /></FONT></P></TD>
											<TD VAlign="Top" WIDTH="200"><P><FONT face="Arial" size="2"><asp:Label id="lblMemberServerID2" runat="server" />&nbsp;</FONT></P></TD>
											<TD><P><FONT face="Arial" size="2"><a runat="server" id="ancDeleteServer"></a>&nbsp;</FONT></P></TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
						</ItemTemplate>
					</asp:Repeater>
					
						</TBODY>
					</TABLE>
				</TD>
			</TR>
		</TBODY>
	</TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu  id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
