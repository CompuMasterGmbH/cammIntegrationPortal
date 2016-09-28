<%@ Page language="VB" EnableViewState="False" Inherits="CompuMaster.camm.WebManager.Pages.Administration.MembershipList" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Memberships" id="cammWebManager" SecurityObject="System - User Administration - Memberships" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

<html>
<head>
    <style>
        TD{vertical-align:top;}
        .boldFont{font-face:Arial;font-size:12;font-weight:bold;}
        .boldFontHeader{font-face:Arial;font-size:12;font-weight:bold;background-color:#E1E1E1;}
        .normalFont{font-face:Arial;font-size:12;}
    </style>
</head>
<body>
<h3><font face="Arial">Administration - Memberships</font></h3>
<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
	<TBODY>
		<TR>
			<TD>
				<asp:Label runat="server" id="SearchGrpLabel">Search groups:</asp:Label>
				<asp:Textbox EnableViewState="True" BorderColor="#00446E" BorderStyle="Dotted" BorderWidth="1px" BackColor="White" runat="server" id="SearchGroupTextBox" />
				<span runat="server" id="gcSearchGroupSpace">&nbsp;&nbsp;</span>
				<asp:Label runat="server" id="SearchUserLabel">Search users:</asp:Label>
				<asp:Textbox EnableViewState="True" BorderColor="#00446E" BorderStyle="Dotted" BorderWidth="1px" BackColor="White" runat="server" id="SearchUserTextBox" /> 
				<input type="text" style="display:none" />
				<asp:button id="btnSubmit" runat="server" text="Go" BorderColor="#00446E" BorderStyle="Solid" BorderWidth="1px" style="CURSOR:pointer" BackColor="White" />
			</TD>
		</TR>
		<TR>
			<TD Align="right">
				<p><font size="2"><a href="memberships.aspx?ShowSystemGrp=1" runat="server" id="ancShowSysGroups" /></font></p>
				<font face="Arial" size="2"><a href="memberships.aspx?Showall=1" runat="server" id="ancShowAll" /></font>
				<font face="Arial" size="2"><a href="memberships.aspx" runat="server" id="ancHideAll" /></a></font>
			</TD>
		</TR>
		
		<TR>
			<TD>
				<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
					<TBODY>					
						<asp:Repeater id="rptMembershipList" runat="server">
							<ItemTemplate>
								<TR style="padding-top:17px;">
									<TD>
										<TABLE WIDTH="100%" BGCOLOR="#C1C1C1" CELLSPACING="0" CELLPADDING="2" border="0">
											<TR>
												<TD Width="30"><P class="boldFont"><a class="None" runat="server" id="ancGroupIDName" /><a runat="server" id="ancGroupID">ID<br><asp:Label runat="server" id="lblGroupID" /></a>&nbsp;</P></TD>
												<TD Width="200"><P class="boldFont"><a id="ancName" runat="server" />&nbsp;</P></TD>
												<TD Width="200"><P class="boldFont"><asp:Label runat="server" id="lblDescription" />&nbsp;</P></TD>
												<TD Width="130"><P class="boldFont">Release date<br><asp:Label runat="server" id="lblReleasedOn" />&nbsp;</P></TD>
												<TD Width="150"><P class="boldFont">Released by<br><a runat="server" id="ancReleasedBy" />&nbsp;</P></TD>
												<TD><P class="normalFont"><a runat="server" id="ancAddUserShowDetails" />&nbsp;<br><a runat="server" id="ancExportCSV"></a>&nbsp;</P></TD>
											</TR>
										</TABLE>
									</TD>
								</TR>
								
								<TR runat="server" id="trDetails"><TD id="tdDetails" runat="server" /></TR>								
							</ItemTemplate>
						</asp:Repeater>
				
						<TR><TD><asp:Label runat="server" id="lblErrMsg" /></TD></TR>
					</TBODY>
				</TABLE>
			</TD>
		</TR>
	</TBODY>
</TABLE>

<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
</body>
</html>