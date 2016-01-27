<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.AppRightsDeleteGroups" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Erase existing authorization" id="cammWebManager" SecurityObject="System - User Administration - Authorizations" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

		<h3><font face="Arial">Erase existing authorization</font></h3>
		<p><font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErrMsg" /></font></p>
		<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
		  <TBODY>
		  <TR>
	        <TD vAlign=top>
		      <TABLE cellSpacing=0 cellPadding=3 width="100%" border="0" bordercolor="#FFFFFF">
		        <TBODY>
					<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>The following authorization would be deleted:</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">ID Authorization</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label runat="Server" id="lblField_ID" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Application title</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label runat="Server" id="lblField_AppTitle" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Group ID</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label runat="Server" id="lblField_GroupID" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Group name</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label runat="Server" id="lblField_Groupname" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Group description</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label runat="Server" id="lblField_Description" /></FONT></P></TD>
					</TR>
					<TR>
					<TD ColSpan="2" VAlign="Top"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Are you really sure?</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><a id="ancDelete" runat="server">Yes, delete it!</a></FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><a id="ancDontDelete" runat="Server">No! Don't touch it!</a></FONT></P></TD>
					</TR>
		        </TBODY></TABLE></TD></TR>
	      </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="apprights.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
