<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.User_Resetpw"%>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" PageTitle="Administration - Reset password" SecurityObject="System - User Administration - Users - Reset password" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<h3><font face="Arial">Administration - Reset password</font></h3>
<p><font face="Arial" size="2" color="red"><asp:Label id="lblMsg" runat="server" /></font></p>
<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
  <TBODY>
	  <TR>
        <TD vAlign=top>
	      <TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
	        <TBODY>
				<TR>
					<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Account details:</b></FONT></P></TD>
				</TR>
				<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">Login <font color="#C1C1C1"> *</font> </FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox id="txtLoginName" runat="server" style="width: 200px" /></FONT></P></TD>
				</TR>
				<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2">New password <font color="#C1C1C1"> *</font> </FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size="2"><asp:TextBox id="txtNewPassword" runat="server" style="width: 200px" autocomplete="new-password" /></FONT></P></TD>
				</TR>
				<TR>
					<input type="text" style="display:none" />
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size="2"><asp:Button id="btnSubmit" text="Set password and send e-mail" runat="server" /></FONT></P></TD>
					<TD VAlign="Middle" Align="Right" Width="240"><P><font face="Arial" size="2" color="#C1C1C1">* required fields</font></P></TD>
				</TR>
	       </TBODY></TABLE>
	      </TD>
	    </TR>
 </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu id="cammWebManagerAdminMenu" runat="server"></camm:WebManagerAdminMenu>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
