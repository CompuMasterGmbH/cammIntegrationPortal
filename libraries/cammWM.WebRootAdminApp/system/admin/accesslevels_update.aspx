<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.AccessLevelsUpdate" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify access level" id="cammWebManager" SecurityObject="System - User Administration - AccessLevels" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

<h3><font face="Arial">Administration - Modify access level</font></h3>
<p><font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErrMsg" /></font></p>
<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
	<TBODY>
		<TR>
			<TD vAlign="top">

					<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
						<TBODY>
							<TR>
								<TD BGCOLOR="#C1C1C1" ColSpan="2">
									<P><FONT face="Arial" size="2"><b>Access level details:</b></FONT></P>
								</TD>
							</TR>
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">ID</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
									<asp:label id="lblID" runat="server" /></FONT></P>
								</TD>
							</TR>
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Title</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
									<asp:textbox runat="server" style="width:200px" id="txtTitle" /></FONT></P>
								</TD>
							</TR>
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Description</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2">
									<asp:textbox id="txtRemarks" textmode="multiline" runat="server" style="width:366px;height:150px;" /></FONT></P>
								</TD>
							</TR>
							<TR>
								<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
							</TR>
							<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">
									<asp:button id="btnSubmit" runat="server" /></FONT></P>
								</TD>
								<TD VAlign="Top" Width="240"><P> &nbsp;</P></TD>
							</TR>
						</TBODY>
					</TABLE>	
			
			</TD>
		</TR>
	</TBODY>
</TABLE>

<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="accesslevels.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
