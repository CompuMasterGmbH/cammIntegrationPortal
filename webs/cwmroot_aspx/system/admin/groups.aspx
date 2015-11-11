<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.GroupList" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - User groups" id="cammWebManager" SecurityObject="System - User Administration - Groups" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

<h3><font face="Arial">Administration - User groups</font></h3>
<TABLE cellSpacing=0 cellPadding=0 bgColor=#ffffff border=0>
	<TBODY>
		<TR>
			<TD vAlign=top>
				<TABLE cellSpacing=0 cellPadding=3 width="100%" border="0" bordercolor="#FFFFFF">
					<TBODY>
					
				<asp:Repeater id="rptGroupList" runat="Server" >
					<HeaderTemplate>
						<TR>
							<TD bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>ID<br>&nbsp;</b></FONT></P></TD>
							<TD bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Name<br>Description</b></FONT></P></TD>
							<TD bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Released by<br>Release date</b></FONT></P></TD>
							<TD bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Actions</b><br><a href="groups_new.aspx" runat="server" id="ancNew">New</a>&nbsp;
								<a runat="server" id="ancSecurity" href="adjust_delegates.aspx?ID=0&Type=Groups&Title=All+groups" title="Adjust administrative delegates">Security</a></FONT></P>
							</TD>
						</TR>
					</HeaderTemplate>
					<ItemTemplate>
						<TR>
							<TD valign="Top"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="lblID" />&nbsp;</FONT></P></TD>
							<TD valign="Top"><P><FONT face="Arial" size="2"><a id="ancName" runat="server" /><br><asp:Label runat="server" id="lblDescription" />&nbsp;</FONT></P></TD>
							<TD valign="Top" Width="120"><P><FONT face="Arial" size="2"><a runat="server" id="ancReleasedByID" />&nbsp;<br><nobr><asp:Label runat="server" id="lblReleasedOn" />&nbsp;</nobr></FONT></P></TD>
							<TD valign="Top">
								<FONT face="Arial" size="2">
									<a runat="server" id="ancUpdate" />
									<a runat="server" id="ancDelete" />
									<a id="ancCheckMembership" runat="server" />
									<a id="ancNavPreview" runat="server" />
								</FONT>
							</TD>
						</TR>
					</ItemTemplate>
					<AlternatingItemTemplate>
						<TR>
							<TD BGCOLOR="#E1E1E1" valign="Top"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="lblID" />&nbsp;</FONT></P></TD>
							<TD BGCOLOR="#E1E1E1" valign="Top"><P><FONT face="Arial" size="2"><a id="ancName" runat="server" /><br><asp:Label runat="server" id="lblDescription" />&nbsp;</FONT></P></TD>
							<TD BGCOLOR="#E1E1E1" valign="Top" Width="120"><P><FONT face="Arial" size="2"><a runat="server" id="ancReleasedByID" />&nbsp;<br><nobr><asp:Label runat="server" id="lblReleasedOn" />&nbsp;</nobr></FONT></P></TD>
							<TD BGCOLOR="#E1E1E1" valign="Top">
								<FONT face="Arial" size="2">
									<a runat="server" id="ancUpdate" />
									<a runat="server" id="ancDelete" />
									<a id="ancCheckMembership" runat="server" />
									<a id="ancNavPreview" runat="server" />
								</FONT>
							</TD>
						</TR>
					</AlternatingItemTemplate>
				</asp:Repeater>
				
						<TR><TD><asp:Label runat="server" id="lblErrMsg" /></TD></TR>
					</TBODY>
				</TABLE>
			</TD>
		</TR>
    </TBODY>
</TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu  id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->