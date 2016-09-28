<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.User_Update_Flag"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify user flags" id="cammWebManager" SecurityObject="System - User Administration - Users" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<h3><font face="Arial">Administration - Modify user flags</font></h3>
<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
	<TBODY>
		<TR>
		<TD vAlign=top>
			<TABLE cellSpacing=0 cellPadding=3 width="100%" border="0" bordercolor="#FFFFFF">
				<TBODY>
					<TR>
						<TD ColSpan="2"><P><FONT face="Arial" color="red" size="2"><asp:Label id="lblMsg" runat="server" /><br> &nbsp;</FONT></P></TD>
					</TR>
					<TR>
						<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b><asp:Label id="lblFlagUser" runat="server" /></b></FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><asp:TextBox id="txtType" maxLength="50" style="width: 200px" runat="server" /> &nbsp;</FONT></P></TD>
						<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox id="txtValue" maxLength="255" style="width: 200px" runat="server" /> &nbsp;</FONT></P></TD>
					</TR>
					<TR>
						<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"> &nbsp;</FONT></P></TD>
						<TD VAlign="Top" Width="240"><P> &nbsp;</P></TD>
					</TR>
					<TR>
						<input type="text" style="display:none" />
						<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><asp:Button id="btnSubmit" text="Save flag" runat="server" /></FONT></P></TD>
						<TD VAlign="Middle" Align="Right" Width="240">&nbsp; </TD>
					</TR>
			</TBODY></TABLE>
		</TD></TR>
</TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="users.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
