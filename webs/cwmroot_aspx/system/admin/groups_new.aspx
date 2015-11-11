<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.GroupNew" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Create new user group" id="cammWebManager" SecurityObject="System - User Administration - Groups" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

		<h3><font face="Arial">Administration - Create new user group</font></h3>
		<p><font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErrMsg" /></font></p>
		<TABLE cellSpacing="0" cellPadding="0" bgColor=#ffffff border="0" bordercolor="#C1C1C1">
		  <TBODY>
		  <TR>
	        <TD vAlign="top">
	       
		      <TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
		        <TBODY>
					<TR>
						<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Group details:</b></FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Group name</FONT></P></TD>
						<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Textbox runat="server" id="textName" style="width: 200px" /></FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Description</FONT></P></TD>
						<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:Textbox runat="server" id="textDescription" style="width: 200px" /></FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">
						<asp:Button ID="btnSubmit" runat="server" Text="Create group" />
						</FONT></P></TD>
						<TD VAlign="Top" Width="240"><P> &nbsp;</P></TD>
					</TR>
		        </TBODY>
		       </TABLE>
		    
		      </TD>
		     </TR>
	      </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="groups.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
