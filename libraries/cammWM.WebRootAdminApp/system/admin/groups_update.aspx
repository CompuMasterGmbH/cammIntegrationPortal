<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.GroupUpdate" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify user group" id="cammWebManager" SecurityObject="System - User Administration - Groups" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
	<h3><font face="Arial">Administration - Modify user group</font></h3>
		<p><font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErrMsg" /></font></p>
		<TABLE cellSpacing="0" cellPadding="0" border="0">
		  <TBODY>
		  <TR>
	        <TD vAlign="top">
		      <TABLE cellSpacing="0" cellPadding="3" width="100%" border="0">
		        <TBODY>
					<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Group details</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Group name</FONT></P></TD>
					<TD VAlign="Top" Width="240">
						<P>
							<FONT face="Arial" size="2">
								<asp:Textbox id="txtGroupName" runat="Server" width="200px"/>
								<asp:Label id="lblGroupName" runat="Server" width="200px"/>
							</FONT>
						</P>
					</TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Description</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
						<asp:Textbox id="txtDescription" runat="Server" width="200px"/> </FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Statistics</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Date of creation</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
						<asp:label id="lblCreationDate" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Created by</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
						<asp:hyperlink id="hypCreatedBy" runat="server" /></a></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Last modification date</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
						<asp:label id="lblModificationDate" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Last modification by</FONT></P></TD>
					<TD VAlign="Top" Width="240">
						<P><FONT face="Arial" size="2">
						<asp:hyperlink id="hypModifiedBy" runat="server"/> </a></FONT></P></TD>
					</TR>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminDelegates" Src="administrative_delegates.ascx" %>
<camm:WebManagerAdminDelegates id="cammWebManagerAdminDelegates" runat="server" />
					<TR>
					<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
					</TR>
					<TR Runat="server" id="trMemberList">
						<TD colspan="2" bgcolor="#C1C1C1">
							<P>
								<FONT face="Arial" size="2"><b>Members e-mail list (read only)</b></FONT>
							</P>
						</TD>
					</TR>
					<TR Runat="server" id="trMemberList2">
						<TD VAlign="Top" WIDTH="160">
							<P><FONT face="Arial" size="2">Please use for mass mailings as BCC</FONT></P>
						</TD>
						<TD VAlign="Top" Width="240">
							<P><FONT face="Arial" size="2">
							<asp:Textbox  id="txtBccMail"  runat="server" style="width: 300px; height: 150px;" TextMode="Multiline" /></FONT></P>
						</TD>
					</TR>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminGroupInfoDetails" Src="groups_additionalinformation.ascx" %>
<camm:WebManagerAdminGroupInfoDetails id="cammWebManagerAdminGroupInfoDetails" runat="server" />
					<TR Runat="server" id="trMemberList3">
					<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
					</TR>


					<TR id="trUpdateGroup" Runat="server">
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">
					<input type="text" style="display:none" />
					<asp:Button runat="server" id="btnSubmit" text="Update group" /></FONT></P></TD>
					<TD VAlign="Top" Width="240"><P> &nbsp;</P></TD>
					</TR>

		        </TBODY></TABLE></FORM></TD></TR>
	      </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="groups.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
