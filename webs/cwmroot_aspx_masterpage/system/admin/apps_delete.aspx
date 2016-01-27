<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.ApplicationDelete" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Erase existing application" id="cammWebManager" SecurityObject="System - User Administration - Applications" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

		<h3><font face="Arial">Administration - Erase existing application</font></h3>
		<p><font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErrMsg" /></font></p>
		<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
		  <TBODY>
		  <TR>
	        <TD vAlign="top">
		      <TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
		        <TBODY>
					<TR>
					<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Application details</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">ID</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblField_ID" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Name</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblField_Name" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Title (for internal use only)</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblField_Title" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Location</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblField_LocationID" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Market</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblField_Language" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Navigation details</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Navigation title in level 1</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblField_Level1Title" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Navigation title in level 2</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblField_Level2Title" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Navigation title in level 3</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblField_Level3Title" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">URL</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblField_NavURL" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Target frame</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblField_NavFrame" runat="Server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Statistics</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Created on</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblCreatedOn" runat="server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Created by</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:hyperlink id="hypCreatedBy" runat="server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Last modification on</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:label id="lblField_ModifiedOn" runat="server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Last modification by</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:hyperlink id="hypModifiedBy" runat="server" /></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Authorizations</b></FONT></P></TD>
					</TR>
					<tr>
						<TD id="tdAddLinks" runat="server" VAlign="Top" Colspan="2" />
					</tr>
					<TR>
					<TD ColSpan="2" VAlign="Top"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Are you really sure to delete this application?</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><asp:hyperlink id="hypDelete" runat="server" text="Yes, delete it!" /></FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:hyperlink id="hypDontDelete" runat="server" text="No! Don't touch it!" /></FONT></P></TD>
					</TR>
		        </TBODY></TABLE></TD></TR>
	      </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="apps.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->

