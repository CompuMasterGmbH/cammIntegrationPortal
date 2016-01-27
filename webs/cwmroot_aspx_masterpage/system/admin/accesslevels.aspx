<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.AccessLevelsList" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Access Levels" id="cammWebManager" SecurityObject="System - User Administration - AccessLevels" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

	<h3><font face="Arial">Administration - Access Levels</font></h3>
	<p><font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErrMsg" /></font></p>
 	<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
	  <TBODY>
	  <TR>
        <TD vAlign="top">
			<asp:Repeater runat="server" id="rptAccessRights">
				<HeaderTemplate>
					<TABLE cellSpacing=0 cellPadding=3 width="100%" border="0" bordercolor="#FFFFFF">
						<TBODY>
							<TR>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>ID</b></FONT></P></TD>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Name</b></FONT></P></TD>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Remarks</b></FONT></P></TD>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Actions</b><br><a href="accesslevels_new.aspx">New</a></FONT></P></TD>
							</TR>
				</HeaderTemplate>
				<ItemTemplate>
							<TR>
								<TD VAlign="Top" WIDTH="35"><P><FONT face="Arial" size="2"><asp:Label id="lblID" runat="server" /></FONT></P></TD>
								<TD VAlign="Top" Width="160"><P><FONT face="Arial" size="2"><asp:HyperLink id="hypTitle" runat="server" /></FONT></P></TD>
								<TD VAlign="Top" WIDTH="200"><P><FONT face="Arial" size="2"><asp:Label id="lblRemarks" runat="server" /></FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:HyperLink id="hypUpdate" runat="server" />&nbsp;<asp:HyperLink id="hypDelete" runat="server" />&nbsp;</FONT></P></TD>
							</TR>
				</ItemTemplate>		
				<AlternatingItemTemplate>
							<TR>
								<TD VAlign="Top" WIDTH="35" BGCOLOR="#E1E1E1"><P><FONT face="Arial" size="2"><asp:Label id="lblID" runat="server" /></FONT></P></TD>
								<TD VAlign="Top" Width="160" BGCOLOR="#E1E1E1"><P><FONT face="Arial" size="2"><asp:HyperLink id="hypTitle" runat="server" /></FONT></P></TD>
								<TD VAlign="Top" WIDTH="200" BGCOLOR="#E1E1E1"><P><FONT face="Arial" size="2"><asp:Label id="lblRemarks" runat="server" /></FONT></P></TD>
								<TD VAlign="Top" BGCOLOR="#E1E1E1"><P><FONT face="Arial" size="2"><asp:HyperLink id="hypUpdate" runat="server" />&nbsp;<asp:HyperLink id="hypDelete" runat="server" />&nbsp;</FONT></P></TD>
							</TR>
				</AlternatingItemTemplate>			
				<FooterTemplate>
						</TBODY>
					</TABLE>
				</FooterTemplate>
			</asp:Repeater>
	    </TD>
	  </TR>
      </TBODY>
    </TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
