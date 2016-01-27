<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.Membership_Delete"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Erase existing membership" id="cammWebManager" SecurityObject="System - User Administration - Memberships" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

<h3><font face="Arial">Erase existing membership</font></h3>
<p><font face="Arial" size="2" color="red"><asp:Label id="lblErrMsg" runat="server" /></font></p>
<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
	<TBODY>
		<TR>
		<TD vAlign="top">
			<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
				<TBODY>
					<TR>
						<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>The following membership would be deleted:</b></FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Membership ID</FONT></P></TD>
						<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblMembershipID" runat="server" /></FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Group name</FONT></P></TD>
						<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblGroupName" runat="server" /> </FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Group description</FONT></P></TD>
						<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblGroupDescription" runat="server" /></FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">User login</FONT></P></TD>
						<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblLoginName" runat="server" /></FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">User name</FONT></P></TD>
						<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Label id="lblCompleteName" runat="server" /></FONT></P></TD>
					</TR>
					<TR>
						<TD ColSpan="2" VAlign="Top"><P> &nbsp;</P></TD>
					</TR>
					<TR>
						<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Are you really sure?</b></FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><a  id="ancDelete" runat="server">Yes, delete it!</a></FONT></P></TD>
						<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><a id="ancTouch" runat="server">No! Don't touch it!</a></FONT></P></TD>
					</TR>
				</TBODY>
			</TABLE></TD></TR>
</TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="memberships.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
