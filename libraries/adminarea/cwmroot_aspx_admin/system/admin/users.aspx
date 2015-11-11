<%@ Page language="VB" EnableViewState="False" Inherits="CompuMaster.camm.WebManager.Pages.Administration.UserList" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" PageTitle="Administration - User accounts" SecurityObject="System - User Administration - Users" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

<script language="javascript">
</script>

	<h3><font face="Arial">Administration - User accounts</font></h3>
	<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
		<TBODY>
			<TR>
				<TD vAlign="top">
					<p>
						<asp:Label runat="server" id="SearchUsersLabel">Search users:</asp:Label>
						<asp:Textbox EnableViewState="True" BorderColor="#00446E" BorderStyle="Dotted" BorderWidth="1px" BackColor="White" runat="server" id="SearchUsersTextBox" /> 
						<asp:button runat="server" text="Go" BorderColor="#00446E" BorderStyle="Solid" BorderWidth="1px" style="CURSOR: pointer" BackColor="White" />
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Checkbox runat="server" EnableViewState="True" autopostback="true" Checked="True" id="CheckBoxTop50Results" text="Top 50 results only" />
					</p>
					<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
						<TBODY>
					<p><font face="Arial" size="2" color="black"><asp:label runat="server" EnableViewState="False" id="lblErrMsg" /></font></p>
					<asp:Repeater id="rptUserList" runat="server" >
						<HeaderTemplate>
							<TR>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>ID</b></FONT></P></TD>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Name<br>Company</b></FONT></P></TD>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Login name<br>Access level</b></FONT></P></TD>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Country<br>State</b></FONT></P></TD>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Last login date<br>Last known IP</b></FONT></P></TD>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Account created on<br>Last modification date</b></FONT></P></TD>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Actions</b><br><a href="users_new.aspx">New</a> <a href="users_import.aspx">Import</a><br><a href="users.aspx?action=export">Export all users</a></FONT></P></TD>
							</TR>
						</HeaderTemplate>
						<ItemTemplate>
							<TR>
								<TD VAlign="Top" WIDTH="35"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="lblID" /><span runat="server" id="gcDisabled" /></FONT></P></TD>
								<TD VAlign="Top" Width="160"><P><FONT face="Arial" size="2"><a runat="server" id="ancUserNameComplete" /><br><asp:Label runat="server" id="lblCompany" /></FONT></P></TD>
								<TD VAlign="Top" Width="130"><P><FONT face="Arial" size="2"><a runat="server" id="lblLoginName" /><br><asp:Label runat="server" id="lblAccessLevelTitle" /></FONT></P></TD>
								<TD VAlign="Top" Width="130"><P><FONT face="Arial" size="2"><nobr><asp:Label runat="server" id="lblLand" /><br><asp:Label runat="server" id="lblState" />&nbsp;</nobr></FONT></P></TD>
								<TD VAlign="Top" Width="130"><P><FONT face="Arial" size="2"><nobr><asp:Label runat="server" id="lblLastLoginOn" /><br><asp:Label runat="server" id="lblLastLoginViaRemoteIP" />&nbsp;</nobr></FONT></P></TD>
								<TD VAlign="Top" Width="130"><P><FONT face="Arial" size="2"><nobr><asp:Label runat="server" id="lblCreatedOn" /><br><asp:Label runat="server" id="lblModifiedOn" />&nbsp;</nobr></FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><a runat="server" id="ancUpdate">Update</a>&nbsp;<a runat="server" id="ancDelete">Delete</a>&nbsp;<a runat="server" id="ancClone">Clone</a>&nbsp;</FONT></P></TD>
							</TR>
						</ItemTemplate>
						<AlternatingItemTemplate>
							<TR>
								<TD BGCOLOR="#E1E1E1" VAlign="Top" WIDTH="35"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="lblID" /><span runat="server" id="gcDisabled" /></FONT></P></TD>
								<TD BGCOLOR="#E1E1E1" VAlign="Top" Width="160"><P><FONT face="Arial" size="2"><a runat="server" id="ancUserNameComplete" /><br><asp:Label runat="server" id="lblCompany" /></FONT></P></TD>
								<TD BGCOLOR="#E1E1E1" VAlign="Top" Width="130"><P><FONT face="Arial" size="2"><a runat="server" id="lblLoginName" /><br><asp:Label runat="server" id="lblAccessLevelTitle" /></FONT></P></TD>
								<TD BGCOLOR="#E1E1E1" VAlign="Top" Width="130"><P><FONT face="Arial" size="2"><nobr><asp:Label runat="server" id="lblLand" /><br><asp:Label runat="server" id="lblState" />&nbsp;</nobr></FONT></P></TD>
								<TD BGCOLOR="#E1E1E1" VAlign="Top" Width="130"><P><FONT face="Arial" size="2"><nobr><asp:Label runat="server" id="lblLastLoginOn" /><br><asp:Label runat="server" id="lblLastLoginViaRemoteIP" />&nbsp;</nobr></FONT></P></TD>
								<TD BGCOLOR="#E1E1E1" VAlign="Top" Width="130"><P><FONT face="Arial" size="2"><nobr><asp:Label runat="server" id="lblCreatedOn" /><br><asp:Label runat="server" id="lblModifiedOn" />&nbsp;</nobr></FONT></P></TD>
								<TD BGCOLOR="#E1E1E1" VAlign="Top"><P><FONT face="Arial" size="2"><a runat="server" id="ancUpdate">Update</a>&nbsp;<a runat="server" id="ancDelete">Delete</a>&nbsp;<a runat="server" id="ancClone">Clone</a>&nbsp;</FONT></P></TD>
							</TR>
						</AlternatingItemTemplate>
					</asp:Repeater>
					
						</TBODY>
					</TABLE>
					<font face="Arial" size="2" color="black"><asp:label runat="server" EnableViewState="False" id="lblNoRecMsg" /></font>
				</td>
			</tr>
		</TBODY>
	</TABLE>
	
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu id="cammWebManagerAdminMenu" runat="server"></camm:WebManagerAdminMenu>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
