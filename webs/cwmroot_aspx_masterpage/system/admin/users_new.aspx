<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.New_Users" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" PageTitle="Administration - Create new user account" SecurityObject="System - User Administration - Users" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
	<h3><font face="Arial">Administration - Create new user account</font></h3>
	<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
	  <TBODY>
	   <TR>
          <TD vAlign="top">
			<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
		        <TBODY>
						<TR>
							<TD ColSpan="2"><P><FONT face="Arial" color="red" size="2"> <asp:Label id="lblMsg" runat="server" maxlength="20" /> </FONT></P></TD>
						</TR>
			             <TR>
							<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>User login</b></FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top" WIDTH="190"><P><FONT face="Arial" size="2">Login <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
							<TD VAlign="Top" Width="210"><P><FONT face="Arial" size="2"><asp:TextBox id="txtLoginName" runat="server" maxlength="20" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">New password <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtPassword1" runat="server" maxLength="64" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">Confirm new password <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtPassword2" runat="server" maxLength="64" style="width: 200px" />&nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top" ColSpan="2"><P>&nbsp; </P></TD>
						</TR>
						 <TR>
							<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Personal data:</b></FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">Company &nbsp;</FONT></P></TD>
              				<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtCompany" runat="server" maxLength="50" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><nobr>Salutation &nbsp;</nobr></FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:dropdownlist id="cmbAnrede" runat="Server" style="width: 200px" size="1" /></FONT></P></TD>								
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">Academic Title (e. g. "Dr.")&nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtTitle" runat="server" maxLength="20" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">First name &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtVorname" runat="server" maxLenght="30" style="width:  200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">Last name &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtNachname" runat="server" maxLength="30" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">Name affix &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtNamenszusatz" runat="server" maxLength="20" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">e-mail <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtemail" runat="server" maxLength="50" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">Street &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtStrasse" runat="server" maxLength="30" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">Zipcode &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtPLZ" runat="server" maxLength="10" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">Location &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtOrt" runat="server" maxLength="50" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">State &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtState" runat="server" maxLength="30" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">Country &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtLand" runat="server" maxLength="30" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">Phone &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtPhone" runat="server" maxLength="30" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">Fax &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtFax" runat="server" maxLength="30" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">Mobile &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtMobile" runat="server" maxLength="30" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">Position &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:TextBox id="txtPosition" runat="server" maxLength="30" style="width: 200px" /> &nbsp;</FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top" ColSpan="2"><P>&nbsp; </P></TD>
						</TR>
						 <TR>
							<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Zone adjustment:</b></FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">Access level <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:dropdownlist id="cmbAccountAccessability" runat="Server" style="width: 200px" size="1" /></font></p></td>								
						</TR>
						 <TR>
							<TD VAlign="Top" ColSpan="2"><P>&nbsp; </P></TD>
						</TR>
						 <TR>
							<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Language/market preferences:</b></FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">1st preferred language/market <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:dropdownlist id="cmbFirstPreferredLanguage" runat="Server" style="width: 200px" size="1" /></FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">2nd preferred language/market &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:dropdownlist id="cmbSecondPreferredLanguage" runat="Server" style="width: 200px" size="1" /></FONT></P></TD>
						</TR>
						 <TR>
							<TD VAlign="Top"><P><FONT face="Arial" size="2">3rd preferred language/market &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:dropdownlist id="cmbThirdPreferredLanguage" runat="Server" style="width: 200px" size="1" /></FONT></P></TD>
						</TR>
						<TR>
							<TD VAlign="Top"><P><FONT face="Arial" size=2> &nbsp;</FONT></P></TD>
							<TD VAlign="Top"><P> &nbsp;</P></TD>
						</TR>
						<TR>
							<input type="text" style="display:none" />
							<TD VAlign="Top"><P><FONT face="Arial" size=2><asp:button id="btnSubmit" runat="server" text="Create Account" /></FONT></P></TD>
							<TD VAlign="Middle" Align="Right"><P><font face="Arial" size="2" color="#C1C1C1">* required fields</font></P></TD>
						</TR>
	        </TBODY></TABLE></TD></TR>
      </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu href="users.aspx" id="cammWebManagerAdminMenu" runat="server"></camm:WebManagerAdminMenu>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->