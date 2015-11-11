<%@ Page language="VB" Src="users_update_loginname.aspx.vb" Inherits="User_RenameLoginname"  %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - User hotline support" id="cammWebManager" SecurityObject="System - User Administration - Users" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminUserInfoDetails" Src="../../sysdata/admin/users_additionalinformation.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminUserInfoAdditionalFlags" Src="../../sysdata/admin/users_additionalflags.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
	<h3><font face="Arial">Administration - User loginname renaming after e.g. wedding/renamed family name</font></h3>
		<asp:Label runat="server" id="MsgSuccessLabel" />
		<asp:Label runat="server" id="MsgEMailTemplate">
			<p>Use following e-mail as an example/template and customize especially your congratulations (see 1st paragraph)</p>
			<p><code>
				Hallo ...,<br />
				<br />
				an dieser Stelle darf auch ich Ihnen herzlichen Glückwunsch zu Ihrer Hochzeit wünschen! Gute gemeinsame Jahre und viel Freude mögen vertraute Begleiter für Sie sein :-)<br />
				<br />
				Soeben habe ich Ihr Benutzerkonto umbenannt. Bitte melden Sie sich zukünftig nur noch mit dem neuen Benutzernamen &quot;???????????????&quot; an.<br />
				<br />
				Beste Grüße
			</code></p>
		</asp:Label>
		<TABLE id="tableUserDetails" runat="server">
		<TBODY>
		<TR>
			<TD valign="top">
				<h4><font face="Arial" size="3">Previous user details</font></h4>
				<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
				<TBODY>
				<TR>
					<TD vAlign="top">
						<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
						<TBODY>
							<TR>
								<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Previous user login:</b></FONT></P></TD>
							</TR>
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Enter login &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox id="txtLoginNamePrevious" runat="server" />&nbsp;<asp:Button id="btnShowUserDetailsPrevious" style="width: 200" text="Refresh" runat="server" /></FONT></P></TD>
							</TR>			
								<TR id="trErrMsgPrevious" runat="server" >
								<TD ColSpan="2" VAlign="Top"><P><FONT face="Arial" color="red" size="2"><asp:Label id="lblErrMsgPrevious" runat="server" /> <a id="ancUserListPrevious" runat="server" target="_blank"></a>&nbsp;</FONT></P></TD>
							</TR>						
							<TR>
								<TD ColSpan="2" VAlign="Top"><P> &nbsp;</P></TD>
							</TR>
						<asp:Repeater id="rptUserShowPrevious" runat="Server" >				
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
							<camm:WebManagerAdminUserInfoAdditionalFlags id="cammWebManagerAdminUserInfoAdditionalFlagsPrevious" runat="server" />
							<camm:WebManagerAdminUserInfoDetails id="cammWebManagerAdminUserInfoDetailsPrevious" runat="server" />
							<camm:WebManagerAdminBlockFooter runat="server" />													
					  </TBODY></TABLE>
					</TD>
				</TR>
				</TBODY></TABLE>
			</TD>
			<TD valign="top">
				<h4><font face="Arial" size="3">Future user details</font></h4>
				<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
				<TBODY>
				<TR>
					<TD vAlign="top">
						<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
						<TBODY>
							<TR>
								<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Future user login:</b></FONT></P></TD>
							</TR>
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Enter login &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox id="txtLoginNameFuture" runat="server" /> <font color="green"><asp:Label id="lblFutureNameAvailable" runat="server" visible="false" enableviewstate=false>available</asp:Label></font>&nbsp;<asp:Button id="btnShowUserDetailsFuture" style="width: 200" text="Refresh" runat="server" /></FONT></P></TD>
							</TR>			
								<TR id="trErrMsgFuture" runat="server" >
								<TD ColSpan="2" VAlign="Top"><P><FONT face="Arial" color="red" size="2"><asp:Label id="lblErrMsgFuture" runat="server" /> <a id="ancUserListFuture" runat="server" target="_blank"></a>&nbsp;</FONT></P></TD>
							</TR>						
							<TR>
								<TD ColSpan="2" VAlign="Top"><P> &nbsp;</P></TD>
							</TR>
						<asp:Repeater id="rptUserShowFuture" runat="Server" >				
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
							<camm:WebManagerAdminUserInfoAdditionalFlags id="cammWebManagerAdminUserInfoAdditionalFlagsFuture" runat="server" />
							<camm:WebManagerAdminUserInfoDetails id="cammWebManagerAdminUserInfoDetailsFuture" runat="server" />
							<camm:WebManagerAdminBlockFooter runat="server" />													
					  </TBODY></TABLE>
					</TD>
				</TR>
				</TBODY></TABLE>
			</TD>
		</TR>
		</TBODY></TABLE>
<h3>Renaming of user login name</h3>
<p>The camm Web-Manager user account will be renamed, NOT transferred to another existing account. Therefore, the future login name must not exist.</p>
<h4>Related user accounts of sub-systems won't be changed</h4>
<p><ul>
<li>ADS user login: rename user account in ADS, then rename user name in field &quot;external user account&quot; to match the new login name</li>
<li>Other 3rd party systems using SSO: rename user account in external system, then rename related additional flag for user name matching the new login name</li>
</ul></p>

<% If cammWebManagerAdminUserInfoDetailsFuture.MyUserInfo IsNot Nothing AndAlso cammWebManagerAdminUserInfoDetailsFuture.MyUserInfo.EMailAddress.StartsWith ("noreply") Then %>
<p><font color="red" face="Arial" size="3"><strong>WARNING:</strong> target E-Mail address still references to noreply@..... It's recommended to wait until the E-Mail address has been set up correctly.</font></p>
<% End If %>
		<asp:Button id="btnStartTransfer" runat="server" text="Rename user login name now!" visible="false" />
	
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->