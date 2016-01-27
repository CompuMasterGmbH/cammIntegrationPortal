<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.User_Hotline_Support"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - User hotline support" id="cammWebManager" SecurityObject="System - User Administration - Users" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminUserInfoDetails" Src="../../sysdata/admin/users_additionalinformation.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<Script language="JavaScript">
<!--
function OpenNavDemo(LangID, ServerGroupID, UserID)
{
	window.open('/sysdata/nav/index.aspx?Mode=Preview&ID=' + UserID + '&Lang=' + LangID + '&Server=' + ServerGroupID,'navPreview','width=200, resizable, scrollbars', 'navPreview');
	return false;
}
//-->
</Script>
	<h3><font face="Arial">Administration - User hotline support</font></h3>
		<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
		<TBODY>
			<TR>
				<TD vAlign="top">
				<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
				<TBODY>
					<TR>
						<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Corresponding user login:</b></FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Enter login &nbsp;</FONT></P></TD>
						<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox id="txtLoginName" runat="server" />&nbsp;</FONT></P></TD>
					</TR>			
						<TR id="trErrMsg" runat="server" >
						<TD ColSpan="2" VAlign="Top"><P><FONT face="Arial" color="red" size="2"><asp:Label id="lblErrMsg" runat="server" /><a id="ancUserList" runat="server" ></a>&nbsp;</FONT></P></TD>
					</TR>						
				</TBODY></TABLE>
				</TD>
			</TR>		        
			<TR>
				<TD ColSpan="2" VAlign="Top"><P> &nbsp;<br><FONT face="Arial" size="2">
				<input type="text" style="display:none" /> 
				<asp:Button id="btnSubmit" runat="server" text="View user details" />
				<asp:Button id="btnUnlockAccount" runat="server" text="Reset lock/logon status" style="width: 200" />
				<asp:Button id="btnShowUserDetails" style="width: 200" text="View user profile" runat="server" /></FONT></P></TD>
			</TR>
			<TR>
				<TD ColSpan="2" VAlign="Top"><P> &nbsp;</P></TD>
			</TR>
		</TBODY></TABLE>
     
		<TABLE id="tableNavigationBarPreview" runat="server"  >
		<TBODY>
		<TR>
			<TD>
				<h4><font face="Arial" size="3">Navigation bar preview</font></h4>
					<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
					<TBODY>
						<TR>
							<TD vAlign="top">
								<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
								<TBODY>				
								<TR>
									<TD id="tdAddDataTable" runat="server"  />
								</TR>
								</TBODY></TABLE>
							</TD>
						</TR>
				</TBODY></TABLE>
			</TD>
		</TR>
		</TBODY></TABLE>	 
	
		<TABLE id="tableUserDetails" runat="server">
		<TBODY>
		<TR>
			<TD>
				<h4><font face="Arial" size="3">User details</font></h4>
				<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
				<TBODY>
				<TR>
					<TD vAlign="top">
						<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
						<TBODY>
						<asp:Repeater id="rptUserShow" runat="Server" >				
						<ItemTemplate>
								<camm:WebManagerAdminBlockHeader Header="User login" runat="server" />
								<TR>
									<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">ID &nbsp;</FONT></P></TD>
									<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblID" runat="server" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
									<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Login &nbsp;</FONT></P></TD>
									<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><a id="ancLoginName" runat="server" ></a> &nbsp;</FONT></P></TD>
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
									<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><a id="ancEmail" runat="server"></a> &nbsp;</FONT></P></TD>
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
								<camm:WebManagerAdminBlockFooter runat="server" />
								<TR>
									<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b><asp:Label id="lblCustomerHadline" runat="server" /></b></FONT></P></TD>
								</TR>
								<TR>										    
									<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><asp:Label id="lblUpdateCustomerNO" runat="server" /> &nbsp;</FONT></P></TD>
									<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblCustomerNO" runat="server" /> </FONT></P></TD>
								</TR>
								<TR>
									<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><asp:Label id="lblUpdateSupplierNO" runat="server" /> &nbsp;</FONT></P></TD>
									<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblSupplierNO" runat="server" /></FONT></P></TD>
								</TR>										
						</ItemTemplate>						
						</asp:Repeater>															
							<camm:WebManagerAdminUserInfoDetails id="cammWebManagerAdminUserInfoDetails" runat="server" />
							<camm:WebManagerAdminBlockFooter runat="server" />													
					  </TBODY></TABLE>
					</TD>
				</TR>
				</TBODY></TABLE>
			</TD>
		</TR>
		</TBODY></TABLE>
		
		<TABLE id="tableUserResetted" runat="server" >
		<TBODY>
		<TR>
			<TD>
				<h4><font face="Arial" size="3">User status (resetted)</font></h4>		
				<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
				<TBODY>
				<TR>
					<TD vAlign="top">
						<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
						<TBODY>
						<TR>
							<TD BGCOLOR="#C1C1C1" ColSpan="2"  ><P><FONT face="Arial" size="2"><b>Memberships:</b></FONT></P></TD>
						</TR>					
						<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Temporary status &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblTemporaryStatus" runat="server" /> &nbsp;</FONT></P></TD>
						</TR>
						<TR>
							<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Permanent status &nbsp;</FONT></P></TD>
							<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblPermanentStatus" runat="server" /><a id="ancPermanentStatus" runat="server" /> &nbsp;</FONT></P></TD>
						</TR>
						<TR>
							<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Login accessability &nbsp;</FONT></P></TD>
							<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblLoginAccessability" runat="server" /><br><em><asp:Label id="lblLoginAccountAccessability" runat="server" /></em> &nbsp;</FONT></P></TD>
						</TR>
						<TR>
							<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Logon status &nbsp;</FONT></P></TD>
							<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblLogonStatus" runat="server" /><br>User is logged out, now &nbsp;</FONT></P></TD>
						</TR>					
						</TBODY></TABLE>
					</TD>
				</TR>
				</TBODY></TABLE>
			</TD>
		</TR>
		</TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
