<%@ Page Inherits="CompuMaster.camm.WebManager.Pages.UserAccount.ChangeUserProfile"
    Language="VB" %>

<%@ Import Namespace="CompuMaster.camm.WebManager.WMSystem" %>
<%@ Import Namespace="CompuMaster.camm.WebManager" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" SecurityObject="@@Public" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<%

    If Request.Form("loginname") = "" Or Trim(Request.Form("loginpw")) = "" Or ErrMsg <> "" Then

%>
<table cellspacing="10" cellpadding="0" bgcolor="#ffffff" border="0">
    <tbody>
        <tr>
            <td valign="top">
                <p>
                    <font face="Arial" size="3"><b>
                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_Title %>:
                        <%= MyUserInfo.LOGINNAME %></b></font></p>
                <% If ErrMsg <> "" Then Response.Write("<p><font face=""Arial"" size=""2"" color=""red"">" & ErrMsg & "</font></p>")%>
                <table cellspacing="0" cellpadding="0" bgcolor="#ffffff" border="0" bordercolor="#C1C1C1">
                    <tbody>

                        <script language="javascript">
			function checkMustFields()
			{
				if (document.UpdateProfile.company.value == '')
				{
					confirm('<%= Replace((cammWebManager.Internationalization.UpdateProfile_ErrMsg_InsertAllRequiredFields), "'", "\'") %>');
					document.UpdateProfile.company.focus();
					return (false);
				}
				if (document.UpdateProfile.anrede.value == '')
				{
					confirm('<%= Replace((cammWebManager.Internationalization.UpdateProfile_ErrMsg_InsertAllRequiredFields), "'", "\'") %>');
					document.UpdateProfile.anrede.focus();
					return (false);
				}
				if (document.UpdateProfile.vorname.value == '')
				{
					confirm('<%= Replace((cammWebManager.Internationalization.UpdateProfile_ErrMsg_InsertAllRequiredFields), "'", "\'") %>');
					document.UpdateProfile.vorname.focus();
					return (false);
				}
				if (document.UpdateProfile.nachname.value == '')
				{
					confirm('<%= Replace((cammWebManager.Internationalization.UpdateProfile_ErrMsg_InsertAllRequiredFields), "'", "\'") %>');
					document.UpdateProfile.nachname.focus();
					return (false);
				}
				if (document.UpdateProfile.elements['e-mail'].value == '')
				{
					confirm('<%= Replace((cammWebManager.Internationalization.UpdateProfile_ErrMsg_InsertAllRequiredFields), "'", "\'") %>');
					document.UpdateProfile.elements['e-mail'].focus();
					return (false);
				}
				if (document.UpdateProfile.strasse.value == '')
				{
					confirm('<%= Replace((cammWebManager.Internationalization.UpdateProfile_ErrMsg_InsertAllRequiredFields), "'", "\'") %>');
					document.UpdateProfile.strasse.focus();
					return (false);
				}
				if (document.UpdateProfile.plz.value == '')
				{
					confirm('<%= Replace((cammWebManager.Internationalization.UpdateProfile_ErrMsg_InsertAllRequiredFields), "'", "\'") %>');
					document.UpdateProfile.plz.focus();
					return (false);
				}
				if (document.UpdateProfile.ort.value == '')
				{
					confirm('<%= Replace((cammWebManager.Internationalization.UpdateProfile_ErrMsg_InsertAllRequiredFields), "'", "\'") %>');
					document.UpdateProfile.ort.focus();
					return (false);
				}
				if (document.UpdateProfile.land.value == '')
				{
					confirm('<%= Replace((cammWebManager.Internationalization.UpdateProfile_ErrMsg_InsertAllRequiredFields), "'", "\'") %>');
					document.UpdateProfile.land.focus();
					return (false);
				}
				if (document.UpdateProfile.elements['1stPreferredLanguage'].value == '')
				{
					confirm('<%= Replace((cammWebManager.Internationalization.UpdateProfile_ErrMsg_InsertAllRequiredFields), "'", "\'") %>');
					document.UpdateProfile.elements['1stPreferredLanguage'].focus();
					return (false);
				}
				if (document.UpdateProfile.loginpw.value == '')
				{
					confirm('<%= cammWebManager.Internationalization.UpdateProfile_ErrMsg_PWRequired %>');
					document.UpdateProfile.loginpw.focus();
					return (false);
				}
				return (true);
			}
                        </script>

                        <tr>
                            <td valign="top">
                                <table cellspacing="0" cellpadding="3" width="100%" border="0" bordercolor="#FFFFFF">
                                    <tbody>
                                        <%  If True Then

                                        %><input type="hidden" name="loginname" value="<%= Server.HTMLEncode(myuserinfo.loginname) %>"><%
                                        %><tr>
                                            <td bgcolor="#C1C1C1" colspan="2">
                                                <p>
                                                    <font face="Arial" size="2"><b>
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_Address %>:</b></font></p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_Company %>
                                                        <font color="#C1C1C1">*</font> &nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="company" maxlength="50" value="<%= Server.HTMLEncode(MyUserInfo.Company) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_Addresses %>
                                                        <font color="#C1C1C1">*</font> &nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <select style="width: 200px" size="1" name="anrede">
                                                            <option <%= IIf(MyUserInfo.Gender = WMSystem.Sex.Masculin,"selected","") %> value="Mr.">
                                                                <%= cammWebManager.Internationalization.UpdateProfile_Abbrev_Mister %></option>
                                                            <option <%= IIf(MyUserInfo.Gender = WMSystem.Sex.Feminin,"selected","") %> value="Ms.">
                                                                <%= cammWebManager.Internationalization.UpdateProfile_Abbrev_Miss %></option>
                                                        </select></font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_AcademicTitle %>&nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="titel" maxlength="20" value="<%= Server.HTMLEncode(MyUserInfo.AcademicTitle) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_FirstName %><font color="#C1C1C1">
                                                            *</font> &nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="vorname" maxlength="30" value="<%= Server.HTMLEncode(MyUserInfo.FirstName) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_LastName %><font color="#C1C1C1">
                                                            *</font> &nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="nachname" maxlength="30" value="<%= Server.HTMLEncode(MyUserInfo.LastName) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_NameAddition %>&nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="namenszusatz" maxlength="20" value="<%= Server.HTMLEncode(MyUserInfo.NameAddition) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_EMail %>
                                                        <font color="#C1C1C1">*</font> &nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="e-mail" maxlength="50" value="<%= Server.HTMLEncode(MyUserInfo.EMailAddress) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_Street %>
                                                        <font color="#C1C1C1">*</font> &nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="strasse" maxlength="30" value="<%= Server.HTMLEncode(MyUserInfo.Street) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_ZIPCode %><font color="#C1C1C1">
                                                            *</font> &nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="plz" maxlength="10" value="<%= Server.HTMLEncode(MyUserInfo.ZipCode) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_Location %><font color="#C1C1C1">
                                                            *</font> &nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="ort" maxlength="50" value="<%= Server.HTMLEncode(MyUserInfo.Location) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_State %>&nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="state" maxlength="30" value="<%= Server.HTMLEncode(MyUserInfo.State) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_Country %><font color="#C1C1C1">
                                                            *</font> &nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="land" maxlength="30" value="<%= Server.HTMLEncode(MyUserInfo.Country) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_Phone %>
                                                        &nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="Phone" maxlength="30" value="<%= Server.HTMLEncode(MyUserInfo.PhoneNumber) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_Fax %>
                                                        &nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="Fax" maxlength="30" value="<%= Server.HTMLEncode(MyUserInfo.FaxNumber) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_Mobile %>
                                                        &nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="Mobile" maxlength="30" value="<%= Server.HTMLEncode(MyUserInfo.MobileNumber) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_PositionInCompany %>
                                                        &nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="text" name="PositionInCompany" maxlength="30" value="<%= Server.HTMLEncode(MyUserInfo.Position) %>">
                                                        &nbsp;</font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %>
                                        <tr>
                                            <td valign="Top" colspan="2">
                                                <p>
                                                    &nbsp;
                                                </p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td bgcolor="#C1C1C1" colspan="2">
                                                <p>
                                                    <font face="Arial" size="2"><b>
                                                        <%= cammWebManager.Internationalization.CreateAccount_Descr_Motivation %>: <font
                                                            color="#FFFFFF">*</font> </b></font>
                                                </p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="Top" colspan="2">
                                                <font face="Arial" size="2">
                                                    <asp:checkboxlist runat="server" id="CheckboxListMotivation" font-size="10pt" />
                                                </font>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="Top" colspan="2">
                                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<font face="Arial" size="2"><asp:textbox runat="server"
                                                    id="MotivationOtherText" style="width: 200px" />
                                                </font>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="Top" colspan="2">
                                                <p>
                                                    &nbsp;
                                                </p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="Top" colspan="2">
                                                <p>
                                                    &nbsp;
                                                </p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td bgcolor="#C1C1C1" colspan="2">
                                                <p>
                                                    <font face="Arial" size="2"><b>
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_UserDetails %>:</b></font></p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_1stLanguage %><font color="#C1C1C1">
                                                            *</font> &nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <select style="width: 200px" size="1" name="1stPreferredLanguage">
                                                            <%= MarketsListOptions (1) %>
                                                        </select></font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_2ndLanguage %>&nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <select style="width: 200px" size="1" name="2ndPreferredLanguage">
                                                            <option value="">&nbsp;</option>
                                                            <%= MarketsListOptions (2) %>
                                                        </select></font></p>
                                            </td>
                                        </tr>
                                        <%
                                        %><tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_3rdLanguage %>&nbsp;</font></p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <select style="width: 200px" size="1" name="3rdPreferredLanguage">
                                                            <option value="">&nbsp;</option>
                                                            <%= MarketsListOptions (3) %>
                                                        </select></font></p>
                                            </td>
                                        </tr>
                                        <%
                                        End If%>
                                        <tr>
                                            <td valign="Top" colspan="2">
                                                <p>
                                                    &nbsp;
                                                </p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td bgcolor="#C1C1C1" colspan="2">
                                                <p>
                                                    <font face="Arial" size="2"><b>
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_Authentification %>:</b></font></p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_Password %><font color="#C1C1C1">
                                                            *</font> </font>
                                                </p>
                                            </td>
                                            <td valign="Top" width="240">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input style="width: 200px" type="password" name="loginpw" value=""></font></p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="Top" width="160">
                                                <p>
                                                    <font face="Arial" size="2">
                                                        <input type="submit" name="submit" value="<%= cammWebManager.Internationalization.UpdateProfile_Descr_Submit %>"></font></p>
                                            </td>
                                            <td valign="Middle" align="Right" width="240">
                                                <p>
                                                    <font face="Arial" size="2" color="#C1C1C1">
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_RequiredFields %></font></p>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
<%

End If
%>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
