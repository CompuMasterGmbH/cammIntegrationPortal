<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.ApplicationClone" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Clone application" id="cammWebManager" SecurityObject="System - User Administration - Applications" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_wo_form.aspx"-->

		<h3><font face="Arial">Administration - Clone application</font></h3>
		
		<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
		  <TBODY>
		  <TR>
	        <TD vAlign="top"><FORM METHOD="POST" ACTION="<%= Response.ApplyAppPathModifier(Request.ServerVariables("SCRIPT_NAME")) %>?<%= Request.QueryString %>" id=form1 name=form1>
		      <TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
		        <TBODY>
					<TR>
					<TD colspan="2" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Cloning method:</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Cloning options</FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><input type="radio" name="CloneType" <%= IIf(cint(Request.Form("CloneType")) = 1, "checked ","") %>value="1">Clone application and all related authorizations<br>
					<input type="radio" name="CloneType" <%= IIf(cint(Request.Form("CloneType")) <> 1, "checked ","") %> value="2">Clone application and inherit authorizations from that application</FONT></P></TD>
					</TR>
					<tr>
					<td>&nbsp;</td>
					<td><input type="checkbox" name="CopyAdminSecurityDelegates" value="1"><FONT face="Arial" size="2">Copy security delegates</font></td>
					</tr>
					<TR>
					<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><input type="submit" name="submit" value="Clone application"></FONT></P></TD>
					<TD VAlign="Top" Width="240"><P> &nbsp;</P></TD>
					</TR>
		        </TBODY></TABLE></FORM></TD></TR>
	      </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="apps.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_wo_form.aspx"-->
