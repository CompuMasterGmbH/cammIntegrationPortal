<%@ Page language="VB" ValidateRequest="False" Inherits="CompuMaster.camm.WebManager.Pages.Administration.MembershipsNew" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Create new membership" id="cammWebManager" SecurityObject="System - User Administration - Memberships" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

		<h3><font face="Arial">Administration - Create new membership</font></h3>		
		<link href="/system/admin/style/baseStyle.css" type="text/css" rel="stylesheet" />
		<link href="/system/admin/style/combobox.css" type="text/css" rel="stylesheet" />
		
		<font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErr" text="" /></font>
		<br><FONT face="Arial" Color="Green" size=2><asp:Label runat="Server" widht="100%" id="lblMsg"></asp:Label></Font>
		<TABLE cellSpacing="0" cellPadding="0" bgColor=#ffffff border="0" bordercolor="#C1C1C1">
		  <TBODY>
		  <TR>
	        <TD vAlign="top">
		      <TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
		        <TBODY>
		        <TR>
					<TD ColSpan="2" align="right">
						<p>
						<font face="Arial" size="2">
							<a id="grp" href="groups_update.aspx?ID=<%=drp_groups.SelectedValue%>">Go to selected group</a>
							<a id="grp" href="memberships.aspx?groupID=<%=drp_groups.SelectedValue%>">Go to selected group memberships</a>
						</font>
						</p>
					</TD>
					</TR>
					<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Membership details:</b></FONT></P></TD>
					</TR>
					<TR>
					<TD VAlign="Top" ><P><FONT face="Arial" size="2">Group name</FONT></P></TD>
					<TD VAlign="Top" ><P><FONT face="Arial" size="2"><div>
						<asp:dropdownlist runat="server" id="drp_groups" />
						</div></FONT></P>
					</TD>
					</TR>
					<TR>
						<TD><P><FONT face="Arial" size="2">User account</FONT></P></TD>
						<TD><P><FONT face="Arial" size="2">
							Search users:
							<asp:Textbox EnableViewState="True" BorderColor="#00446E" BorderStyle="Dotted" BorderWidth="1px" BackColor="White" runat="server" id="SearchUsersTextBox" /> 
							<asp:TextBox runat="server" style="visibility:hidden;display:none;" />
							<asp:button runat="server" text="Go" BorderColor="#00446E" BorderStyle="Solid" BorderWidth="1px" style="CURSOR: pointer" BackColor="White" id="searchUserBtn" />
							&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Checkbox runat="server" EnableViewState="True" autopostback="true" Checked="True" id="CheckBoxTop50Results" text="Top 50 results only" />
						</TD>
					</TR>
					<tr>
					<td colspan="2">
					<table width="100%">
						<asp:Repeater id="rptUserList" runat="server" >
						<HeaderTemplate>
							<TR>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>ID</b></FONT></P></TD>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Name</b></FONT></P></TD>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Login name</b></FONT></P></TD>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Company</b></FONT></P></TD>
								<TD VAlign="Top" bgcolor="#C1C1C1"><P><FONT face="Arial" size="2"><b>Actions</b></FONT></P></TD>
							</TR>
						</HeaderTemplate>
						<ItemTemplate>
							<TR>
								<TD VAlign="Top" WIDTH="35"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="lblID" /><span runat="server" id="gcDisabled" /></FONT></P></TD>
								<TD VAlign="Top" Width="160"><P><FONT face="Arial" size="2"><a runat="server" id="ancUserNameComplete" /></FONT></P></TD>
								<TD VAlign="Top" Width="130"><P><FONT face="Arial" size="2"><a runat="server" id="lblLoginName" /></FONT></P></TD>
								<TD VAlign="Top" Width="130"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="lblCompany" /></FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:checkbox runat="server" id="chk_user" text="Add membership" /> <asp:checkbox runat="server" id="chk_denyuser" text="Deny membership" /></FONT></P></TD>
							</TR>
						</ItemTemplate>
						<AlternatingItemTemplate>
							<TR>
								<TD BGCOLOR="#E1E1E1" VAlign="Top" WIDTH="35"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="lblID" /><span runat="server" id="gcDisabled" /></FONT></P></TD>
								<TD BGCOLOR="#E1E1E1" VAlign="Top" Width="160"><P><FONT face="Arial" size="2"><a runat="server" id="ancUserNameComplete" /></FONT></P></TD>
								<TD BGCOLOR="#E1E1E1" VAlign="Top" Width="130"><P><FONT face="Arial" size="2"><a runat="server" id="lblLoginName" /></FONT></P></TD>
								<TD BGCOLOR="#E1E1E1" VAlign="Top" Width="130"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="lblCompany" /></FONT></P></TD>
								<TD BGCOLOR="#E1E1E1" VAlign="Top"><P><FONT face="Arial" size="2"><asp:checkbox runat="server" id="chk_user" text="Add membership" /> <asp:checkbox runat="server" id="chk_denyuser" text="Deny membership" /></FONT></P></TD>
							</TR>
						</AlternatingItemTemplate>
					</asp:Repeater>
					<asp:label runat="server" EnableViewState="False" id="lblNoRecMsg" />
						</div></FONT></P>
					</TD>
					</TR>
					</TBODY></TABLE>
					<asp:Button runat="Server" ID="btnOK" Text="Create membership rules" />
		        
		        </TD></TR>
	      </TBODY></TABLE>
	      
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="memberships.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
