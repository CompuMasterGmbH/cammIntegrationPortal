<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.User_Delete" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" PageTitle="Erase existing user" SecurityObject="System - User Administration - Users" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminUserInfoDetails" Src="../../sysdata/admin/users_additionalinformation.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<h3><font face="Arial">Erase existing user</font></h3>
<p><font face="Arial" size="2" color="red"><asp:Label id="lblMsg" runat="server" /></font></p>
	<TABLE cellSpacing="0" cellPadding="0" bgColor=#ffffff border="0" bordercolor="#C1C1C1">
		<TBODY>
			<TR>
			<TD vAlign="top">
				<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
							<TBODY>	        
				    <asp:Repeater id="rptUserDelete" runat="Server" >				    
					    <ItemTemplate>						
							<camm:WebManagerAdminBlockHeader Header="User login" runat="server" />
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">ID &nbsp;</FONT></P></TD>
				        		<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblID" runat="server" maxLength="20" /> &nbsp;</FONT></P></TD>
							</TR>						
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Login &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblLoginName" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>	
							<camm:WebManagerAdminBlockFooter runat="server" />
							<TR>
								<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Personal data:</b></FONT></P></TD>
							</TR>
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Company &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblCompany" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>	
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><nobr>Salutation (Mr., Ms., etc.) &nbsp;</nobr></FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblAnrede" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Academic Title (e. g. Dr.)&nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblTitel" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">First name &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblVorname" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Last name &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblNachname" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>						
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Name addition &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblNamenszusatz" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">e-mail &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><a id="ancEmail" runat="server" ></a> &nbsp;</FONT></P></TD>
							</TR>							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Street &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblStrasse" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Zipcode &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblPLZ" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Location &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblORT" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">State &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblState" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Country &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblLand" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>
							<camm:WebManagerAdminBlockFooter runat="server" />
							<TR>
								<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Statistics and restrictions:</b></FONT></P></TD>
							</TR>
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Page views &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblLoginCount" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Login failures &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblLoginFailures" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Login blocked until &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblLoginLockedTill" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Login disabled &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblLoginDisabled" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>	
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Access level &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblAccountAccessability" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Date of creation &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblCreatedOn" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>	
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Last modification date &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblModifiedOn" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>	
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Last login date &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblLastLoginOn" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Last known IP address &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblLastLoginViaremoteIP" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">External account &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblExtAccount" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>							
							<camm:WebManagerAdminBlockFooter runat="server" />
							<TR>
								<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Language/market preferences:</b></FONT></P></TD>
							</TR>
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">1st preferred language/market &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblFirstPreferredLanguage" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">2nd preferred language/market &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblSecondPreferredLanguage" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>							
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">3rd preferred language/market &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblThirdPreferredLanguage" runat="server" /> &nbsp;</FONT></P></TD>
							</TR>								
						</ItemTemplate>						
					</asp:Repeater>	
						
					<camm:WebManagerAdminUserInfoDetails id="cammWebManagerAdminUserInfoDetails" runat="server" />
					<camm:WebManagerAdminBlockFooter runat="server" />
					<asp:placeholder runat="server" id="phConfirmDeletion">
					<TR>
						<TD BGCOLOR="#C1C1C1" ColSpan="2">
						<P><FONT face="Arial" size="2"><b>Are you really sure to delete this user?</b></FONT></P>
						</TD>
					</TR>
					<TR>
						<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><a id="ancDelete" runat="server">Yes, delete it!</a></FONT></P></TD>
						<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><a id="ancTouch" runat="server">No! Don't touch it!</a></FONT></P></TD>
					</TR>
					</asp:placeholder>
				</TBODY></TABLE>					
			</TD>
			</TR>
	      </TBODY>
	  </TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="users.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
