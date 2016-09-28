<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.Apprights_New_Transmession"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Create new transmission" id="cammWebManager" SecurityObject="System - User Administration - Authorizations" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

<h3><font face="Arial">Administration - Create new transmission</font></h3>
<p><font face="Arial" size="2" color="red"><asp:Label id="lblErrMsg" runat="server" /></font></p>
<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
<TBODY>
	<TR>
		<TD vAlign=top>
			<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
			<TBODY>
				<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Transmission details:</b></FONT></P></TD>
				</TR>
				<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Application</FONT></P></TD>
					<TD VAlign="Top" Width="440"><P><FONT face="Arial" size="2"><asp:DropDownList id="cmbApplicationID" style="width: 400px" size="1" runat="server" /></FONT></P></TD>
				</TR>
				<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Inherits from application</FONT></P></TD>
					<TD VAlign="Top" Width="440"><P><FONT face="Arial" size="2"><asp:DropDownList id="cmbInheritsApplicationID" style="width: 400px" size="1" runat="server" /></FONT></P></TD>
				</TR>
				<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><input type="text" style="display:none">
					<asp:Button id="btnCreateTransmission" text="Create transmission" runat="server" /></FONT></P></TD>
					<TD VAlign="Top" Width="240"><P> &nbsp;</P></TD>
				</TR>
			</TBODY></TABLE></FORM>
		</TD>
	</TR>
</TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="apprights.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
