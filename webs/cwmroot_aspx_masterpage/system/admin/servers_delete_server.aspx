<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.DeleteServer"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Delete existing server" id="cammWebManager" SecurityObject="System - User Administration - ServerSetup" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
		<h3><font face="Arial">Delete existing server</font></h3>
		<p><font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErrMsg" /></font></p>
		<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
		  <TBODY>
		  <TR>
	        <TD vAlign="top">
		      <TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
		        <TBODY>
					<TR>
					<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Server</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Server ID</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblServer" runat="server" />
					
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">IP / Host Header</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
					<asp:label id="lblIP" runat="server" />
					
					</FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Description</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
					<asp:label id="lblDescription" runat="server" />
					</FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Address</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
					<asp:label id="lblAddress" runat="server" />
					</FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>ATTENTION!!!</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" colspan="2"><P><FONT face="Arial" size="2">This step will modify all of the following objects:<br><br><ul><li>The currently selected <b>server</b> will be <b>DELETED</b> permanently.</li><li>Script engines of connected servers will be UNREGISTERED.</li><li>Related <b>logs</b> will be <b>DELETED</b> permanently.</li><li>Web Editor Content of this server will be <b>DELETED</b> permanently.</li><li>Related <b>applications</b> and their <b>authorizations</b> will be <b>DELETED</b> permanently.</li></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Are you really sure to delete this server?</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">
					<asp:HyperLink id="hypDelete" runat="server" />
					
					</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><a href="servers.aspx">No! Don't touch it!</a></FONT></P></TD>
					</TR>
		        </TBODY></TABLE></TD></TR>
	      </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="servers.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
