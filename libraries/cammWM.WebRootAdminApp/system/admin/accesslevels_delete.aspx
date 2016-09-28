<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.AccessLevelsDelete" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Erase existing access level" id="cammWebManager" SecurityObject="System - User Administration - AccessLevels" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

		<h3><font face="Arial">Erase existing access level</font></h3>
		<p><font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErrMsg" /></font></p>
		<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
		  <TBODY>
		  <TR>
	        <TD vAlign="top">
		      <TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
		        <TBODY>
					<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>The following access level would be deleted:</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">ID</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblFieldID" runat="server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Title</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblFieldTitle" runat="server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Remarks</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblFieldRemarks" runat="server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Related users will be set to alternative access level</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:dropdownlist AutoPostBack="true" id="DropdownAlternativeAccessLevels" runat="server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD ColSpan="2" VAlign="Top"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>ATTENTION!!!</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" colspan="2"><P><FONT face="Arial" size="2">This step will modify all of the following objects:<br><br>
					<ul>
						<li>Related <b>users</b> will be <b>MODIFIED</b> to use the alternative access level.</li>
						<li>Content of Web Editors of this server will <b>STAY</b> permanently (other servers might reference that content).</li>
						<li><b>Server groups</b> referencing this access level as default for new users will be <b>MODIFIED</b>. <b>Please check for suitable defaults after this change!</b></li>
					</ul>
					</FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Are you really sure?</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><a href="accesslevels_delete.aspx?ID=<%= Me.AccessLevelIDToRemove %>&AltID=<%= Me.SelectedAlternativeAccessLevel %>&DEL=NOW">Yes, delete it!</a></FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><a href="accesslevels.aspx">No! Don't touch it!</a></FONT></P></TD>
					</TR>
		        </TBODY></TABLE></TD></TR>
	      </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="accesslevels.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->