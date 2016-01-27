<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.GroupDelete" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Erase existing group" id="cammWebManager" SecurityObject="System - User Administration - Groups" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

		<h3><font face="Arial">Erase existing group</font></h3>
		<p><font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErrMsg" /></font></p>
		<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
		  <TBODY>
		  <TR>
	        <TD vAlign="top">
		      <TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
		        <TBODY>
					<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>The following group would be deleted</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Group ID</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblGroupID" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Group name</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblGroupName" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Description</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblDescription" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Statistics</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Created on</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblCreatedOn" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Created by</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:hyperlink id="hypCreatedBy" runat="server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Last modification on</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblLastModificationOn" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Last modification by</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:hyperlink id="hypLastModificationBy" runat="server" /></FONT></P></TD>
					</TR>
					

<%@ Register TagPrefix="camm" TagName="WebManagerAdminGroupInfoDetails" Src="groups_additionalinformation.ascx" %>
<camm:WebManagerAdminGroupInfoDetails id="cammWebManagerAdminGroupInfoDetails" runat="server" />
					<TR>
					<TD ColSpan="2" VAlign="Top"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Are you really sure?</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><asp:hyperlink id="hypDeleteConfirmation" runat="server" /></FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><a href="groups.aspx">No! Don't touch it!</a></FONT></P></TD>
					</TR>
		        </TBODY></TABLE></TD></TR>
	      </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="groups.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
