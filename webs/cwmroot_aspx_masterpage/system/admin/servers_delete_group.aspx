<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.DeleteServerGroup"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Erase existing server group" id="cammWebManager" SecurityObject="System - User Administration - ServerSetup" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

		<h3><font face="Arial">Erase existing server group</font></h3>
		<p><font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErrMsg" /></font></p>
		<TABLE cellSpacing=0 cellPadding=0 bgColor=#ffffff border=0 bordercolor="#C1C1C1">
		  <TBODY>
		  <TR>
	        <TD vAlign=top>
		      <TABLE cellSpacing=0 cellPadding=3 width="100%" border="0" bordercolor="#FFFFFF">
		        <TBODY>
					<TR>
					<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size=2><b>Server group</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2>Server group ID</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2>
						<asp:label id="lblServerGroupId" runat="Server" />
						</FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2>Server group name</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2>
					<asp:label id="lblServerGroupName" runat="Server" />
					
					</FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size=2><b>ATTENTION!!!</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" colspan="2"><P><FONT face="Arial" size=2>This step will modify all of the following objects:<br><br><ul><li>All currently connected <b>servers</b> will be <b>DELETED</b> permanently.</li><li>The corresponding <b>public user group</b> will be <b>DELETED</b>.</li><li>Relations between access levels and the server group will be DELETED.</li><li>Script engines of connected servers will be UNREGISTERED.</li><li>Related <b>logs</b> will be <b>DELETED</b> permanently.</li><li>Related <b>applications</b> and their <b>authorizations</b> will be <b>DELETED</b> permanently.</li></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size=2><b>Are you really sure to delete this server group?</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><a href="servers_delete_group.aspx?ID=<%= Request.QueryString ("ID") %>&DEL=NOW&token=<%= Session.SessionID %>">Yes, delete it!</a></FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><a href="servers.aspx">No! Don't touch it!</a></FONT></P></TD>
					</TR>
		        </TBODY></TABLE></TD></TR>
	      </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="servers.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
