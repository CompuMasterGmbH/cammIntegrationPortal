<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.Users_Navbar_Preview" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" PageTitle="Administration - Navigation preview of special users" SecurityObject="System - User Administration - NavPreview" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<Script language="JavaScript">
<!--
function OpenNavDemo(LangID, ServerGroupID, UserID, GroupID)
{
	//window.open('/sysdata/nav/index.aspx?Mode=Preview&ID=' + UserID + '&PreviewLang=' + LangID + '&Server=' + ServerGroupID + '&Group=' + GroupID,'navPreview','width=200, resizable, scrollbars', 'navPreview');
	var url = '/sysdata/nav/index.aspx?Mode=Preview&ID=' + UserID + '&PreviewLang=' + LangID + '&GroupID=' + GroupID + '&Server=' + ServerGroupID;
	window.open(url,'navPreview','width=200, resizable, scrollbars', 'navPreview');
	return false;
}
//-->
</Script>
<h3><font face="Arial"><asp:Label runat="server" id="lblHeading" Text="Administration - Navigation preview of special users"/>
</font></h3>
<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
<TBODY>
	<TR>
		<TD vAlign="top">
			<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
			<TBODY>
				<TR>
					<TD id="tdAnonymous" runat="server" > </TD>
				</TR>				
				<TR style="height:20px" ><TD></TD></TR>
				<TR>
					<TD id="tdPublic" runat="server" ></TD>
				</TR>
			</TBODY></TABLE>
		</TD>
	</TR>
</TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->