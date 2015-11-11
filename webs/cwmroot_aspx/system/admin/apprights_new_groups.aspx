<%@ Page language="VB" ValidateRequest="False" Inherits="CompuMaster.camm.WebManager.Pages.Administration.AppRightsNewGroups" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Create new user authorization" id="cammWebManager" SecurityObject="System - User Administration - Authorizations" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

		<h3><font face="Arial">Administration - Create new group authorization</font></h3>
		<link href="/system/admin/style/baseStyle.css" type="text/css" rel="stylesheet" />
		<link href="/system/admin/style/combobox.css" type="text/css" rel="stylesheet" />
		
		<font face="Arial" size="2" color="red"><asp:Label runat="Server" id="lblErr" /></font>
		<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
		  <TBODY>
		  <TR>
	        <TD vAlign=top>
		      <TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
		        <TBODY>
					<TR>
					<TD ColSpan="2" align="right">
						<p>
						<font face="Arial" size="2">
							<a href="apprights.aspx?Application=<%=drp_apps.SelectedValue%>&AuthsAsAppID=<%=CInt(request.querystring("AuthsAsAppID"))%>">Go to selected application</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
							<a id="user" href="groups_update.aspx?ID=<%=drp_groups.SelectedValue%>">Go to selected Group</a>
						</font>
						</p>
					</TD>
					</TR>
					<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Authorization details:</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" ><P><FONT face="Arial" size="2">Application</FONT></P></TD>
					<TD VAlign="Top" ><P><FONT face="Arial" size="2">
					
					<div>
						<asp:dropdownlist runat="server" id="drp_apps" />
						</div></FONT></P>
					</TD>
					</TR>
					<TR>
					<TD VAlign="Top"><P><FONT face="Arial" size="2">Group name</FONT></P></TD>
					<TD VAlign="Top"><P><FONT face="Arial" size="2">
					
					<div>
						<asp:dropdownlist runat="server" id="drp_groups" />
						</div>
						</FONT></P>
					</TD>
					</TR>					
					<TR>
					<TD VAlign="Top" ><P><FONT face="Arial" size="2">
						<input type="text" style="display:none" />
						<asp:Button runat="Server" ID="btnOK" Text="Create authorization" ></asp:Button></FONT></P></TD>
					<TD VAlign="Top" >&nbsp;</P></TD>
					</TR>
					
		        </TBODY></TABLE>
		        <br><FONT face="Arial" Color="Green" size="2"><asp:Label runat="Server" widht="100%" id="lblMsg"></asp:Label></Font>
		        </TD></TR>
	      </TBODY></TABLE>
						
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="apprights.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
