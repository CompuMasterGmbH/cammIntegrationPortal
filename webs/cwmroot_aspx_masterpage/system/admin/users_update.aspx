<%@ Page language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.UsersUpdate" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify user account" id="cammWebManager" SecurityObject="System - User Administration - Users" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
	<h3><font face="Arial">Administration - Modify user account</font></h3>
	<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
	  <TBODY>
	  <TR>
            <TD vAlign="top">
	      <TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
	        <TBODY>
			<p><font face="Arial" size="2" color="red"><asp:label runat="server" id="lblErrMsg" /></font></p>
								<TR>
								<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>User login</b></FONT></P></TD>
								</TR><TR>
								<TD VAlign="Top" Width="190"><P><FONT face="Arial" size="2">ID &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="210"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="Field_ID" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Login &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="Field_LoginName" /> &nbsp;</FONT></P></TD>
								</TR>
								<asp:panel runat="server" id="pnlSpecialUsers" >
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">&nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><a href="users.aspx?action=export&userid=<%= UserInfo.IDLong %>">Export to CSV</a> </FONT></P></TD>
								</TR>
								<camm:WebManagerAdminBlockFooter runat="server" /><TR>
								<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Personal data:</b></FONT></P></TD>
								</TR><TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Company &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" id="Field_Company" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><nobr>Gender &nbsp;</nobr></FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">
									<asp:dropdownlist id="cmbAnrede" runat="Server" style="width: 200px" size="1" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Academic Title (e. g. "Dr.")&nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="20" id="Field_Titel" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">First name &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="30" id="Field_Vorname" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Last name &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="30" id="Field_Nachname" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Name addition &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="20" id="Field_Namenszusatz" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">e-mail <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="50" id="Field_e_mail" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Street &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="30" id="Field_Strasse" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Zipcode &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="10" id="Field_PLZ" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Location &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="50" id="Field_Ort" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">State &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="30" id="Field_State" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Country &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="30" id="Field_Land" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Phone &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="30" id="Field_Phone" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Fax &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="30" id="Field_Fax" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Mobile &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="30" id="Field_Mobile" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Position &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="30" id="Field_Position" /> &nbsp;</FONT></P></TD>
								</TR>
								<camm:WebManagerAdminBlockFooter runat="server" /><TR>
								<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Statistics and restrictions:</b></FONT></P></TD>
								</TR><TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Page views &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="Field_LoginCount" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Failed logins &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="Field_LoginFailures" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Login blocked until &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="20" id="Field_LoginLockedTill" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Login disabled <font color="#C1C1C1"> </font> &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">
									<asp:dropdownlist id="cmbLoginDisabled" runat="Server" style="width: 200px" size="1" /></font></p></td>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Account access level <font color="#C1C1C1"> </font> &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">
									<asp:dropdownlist id="cmbAccountAccessable" runat="Server" style="width: 200px" size="1" /></font></p></td>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Date of creation &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="Field_CreatedOn" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Last modification date &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="Field_ModifiedOn" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">Last login date &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Label runat="server" id="Field_LastLoginOn" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">External account &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"><asp:Textbox width="200px" runat="server" maxLength="255" id="Field_ExternalAccount" /> &nbsp;</FONT></P></TD>
								</TR>
								<camm:WebManagerAdminBlockFooter runat="server" /><TR>
								<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size="2"><b>Language/market preferences:</b></FONT></P></TD>
								</TR><TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">1st preferred language/market <font color="#C1C1C1"> </font> &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">
									<asp:dropdownlist id="cmb1stPreferredLanguage" runat="Server" style="width: 200px" size="1" /></FONT></P></TD></TR><TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">2nd preferred language/market &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">
									<asp:dropdownlist id="cmb2ndPreferredLanguage" runat="Server" style="width: 200px" size="1" /></FONT></P></TD></TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">3rd preferred language/market &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">
									<asp:dropdownlist id="cmb3rdPreferredLanguage" runat="Server" style="width: 200px" size="1" /></FONT></P></TD>
								</TR>
<%
cammWebManagerAdminUserInfoDetails.cammWebManager = cammWebManager
cammWebManagerAdminUserInfoDetails.MyUserInfo = UserInfo
cammWebManagerAdminUserInfoFlags.cammWebManager = cammWebManager
cammWebManagerAdminUserInfoFlags.MyUserInfo = UserInfo
%>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminUserInfoFlags" Src="../../sysdata/admin/users_additionalflags.ascx" %>
<camm:WebManagerAdminUserInfoFlags id="cammWebManagerAdminUserInfoFlags" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminUserInfoDetails" Src="../../sysdata/admin/users_additionalinformation.ascx" %>
<camm:WebManagerAdminUserInfoDetails id="cammWebManagerAdminUserInfoDetails" runat="server" />
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2"> &nbsp;</FONT></P></TD>
								<TD VAlign="Top"><P> &nbsp;</P></TD>
								</TR>
								<TR>
								<TD VAlign="Top"><P><FONT face="Arial" size="2">
								<input type="text" style="display:none" />
								<asp:Button runat="server" id="Button_Submit" text="Update account" /></FONT></P></TD>
								<TD VAlign="Middle" Align="Right"><P><font face="Arial" size="2" color="#C1C1C1">* required fields</font></P></TD>
								</TR>
								</asp:panel>
	        </TBODY></TABLE></TD></TR>
      </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="users.aspx" id="cammWebManagerAdminMenu" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
