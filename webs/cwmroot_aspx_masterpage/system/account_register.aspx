<%@ Page Title="Register account" MasterPageFile="~/portal/MasterPage.master" Inherits="CompuMaster.camm.WebManager.Pages.UserAccount.CreateUserProfile" Language="VB" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <camm:WebManager ID="cammWebManager" runat="server" />
</asp:Content>

<script runat="server">

    Private Sub CustomValidatorComesFrom_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
        args.IsValid = IsValidFieldComesFrom()
    End Sub
    Private Sub CustomValidatorMotivation_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
        args.IsValid = IsValidFieldMotivation()
    End Sub

    Protected Overridable Function IsValidFieldMotivation() As Boolean
        For Each item As Web.UI.WebControls.ListItem In Me.CheckboxListMotivation.Items
            If item.Selected Then
                Return True
            End If
        Next
        Return False
    End Function

    Protected Overridable Function IsValidFieldComesFrom() As Boolean
        For Each item As Web.UI.WebControls.ListItem In Me.RadioListComesFrom.Items
            If item.Selected Then
                Return True
            End If
        Next
        Return False
    End Function


</script>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <h3><font face="Arial" size="3"><%= Server.htmlencode(cammWebManager.Internationalization.OfficialServerGroup_Title) %><br />-&nbsp;<%= cammWebManager.Internationalization.CreateAccount_Descr_PageTitle %>&nbsp;-</font></h3>
    <table cellspacing="0" cellpadding="0" bgcolor="#ffffff" border="0" bordercolor="#C1C1C1">
        <tbody>
            <tr>
                <td valign="top">
                    <table cellspacing="0" cellpadding="3" width="100%" border="0" bordercolor="#FFFFFF">
                        <tbody>


                            <% If ValidatorSummary.Visible Then %><tr>
                                <td colspan="2">
                                    <p><font face="Arial" color="red" size="2"><asp:ValidationSummary runat="server" id="ValidatorSummary" /></font></p>
                                </td>
                            </tr>
                            <% End If %>
                            <tr>
                                <td bgcolor="#C1C1C1" colspan="2">
                                    <p><font face="Arial" size="2"><b><%= cammWebManager.Internationalization.CreateAccount_Descr_UserLogin %>:</b></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_NewLoginName %> <font color="#C1C1C1"> *</font> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p><font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxLoginName" maxLength="20" style="width: 200px" />&nbsp;<asp:CustomValidator Display="Dynamic" runat="server" id="ValidatorLoginName" /></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_NewLoginPassword %> <font color="#C1C1C1"> *</font> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p><font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxPassword1" TextMode="Password" maxLength="64" style="width: 200px" />&nbsp;<asp:CustomValidator arunat="server" id="ValidatorCreationErrors" /><asp:CustomValidator Display="Dynamic" runat="server" id="ValidatorPassword1" /></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_NewLoginPasswordConfirmation %> <font color="#C1C1C1"> *</font> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p><font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxPassword2" TextMode="Password" maxLength="64" style="width: 200px" />&nbsp;<asp:CustomValidator Display="Dynamic" runat="server" id="ValidatorPassword2" /></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" colspan="2">
                                    <p>&nbsp; </p>
                                </td>
                            </tr>
                            <tr>
                                <td bgcolor="#C1C1C1" colspan="2">
                                    <p><font face="Arial" size="2"><b><%= cammWebManager.Internationalization.CreateAccount_Descr_Address %>:</b></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_Company %> <font color="#C1C1C1"> *</font> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p>
                                        <font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxCompany" maxLength="50" style="width: 200px" /> 
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorCompany" ControlToValidate="TextboxCompany" Text="<%# Me.LocalizedTextRequiredField %>" /></font>
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><nobr><%= cammWebManager.Internationalization.CreateAccount_Descr_Addresses %> <font color="#C1C1C1"> *</font> &nbsp;</nobr></font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p>
                                        <font face="Arial" size="2"><asp:DropdownList runat="server" id="DropdownSalutation" style="width: 200px" /> 
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorSalutation" ControlToValidate="DropdownSalutation" Text="<%# Me.LocalizedTextRequiredField %>" /></font>
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_AcademicTitle %> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p><font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxAcademicTitle" maxLength="20" style="width: 200px" /> &nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_FirstName %> <font color="#C1C1C1"> *</font> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p>
                                        <font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxFirstName" maxLength="30" style="width: 200px" />
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorFirstName" ControlToValidate="TextboxFirstName" /></font>
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_LastName %> <font color="#C1C1C1"> *</font> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p>
                                        <font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxLastName" maxLength="30" style="width: 200px" />
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorLastName" ControlToValidate="TextboxLastName" /></font>
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_NameAddition %> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p><font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxNameAffix" maxLength="20" style="width: 200px" /> &nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_EMail %> <font color="#C1C1C1"> *</font> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p>
                                        <font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxEMail" maxLength="50" style="width: 200px" />
								<asp:CustomValidator Display="Dynamic" runat="server" id="ValidatorEMail" /></font>
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_Street %> <font color="#C1C1C1"> *</font> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p>
                                        <font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxStreet" maxLength="30" style="width: 200px" />
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorStreet" ControlToValidate="TextboxStreet" Text="<%# Me.LocalizedTextRequiredField %>" /></font>
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_ZIPCode %> <font color="#C1C1C1"> *</font> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p>
                                        <font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxZipCode" maxLength="10" style="width: 200px" />
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorZipCode" ControlToValidate="TextboxZipCode" Text="<%# Me.LocalizedTextRequiredField %>" /></font>
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_Location %> <font color="#C1C1C1"> *</font> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p>
                                        <font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxLocation" maxLength="50" style="width: 200px" />
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorLocation" ControlToValidate="TextboxLocation" Text="<%# Me.LocalizedTextRequiredField %>" /></font>
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_State %> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p><font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxState" maxLength="30" style="width: 200px" /> &nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_Country %><font color="#C1C1C1"> *</font> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p>
                                        <font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxCountry" maxLength="30" style="width: 200px" />
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorCountry" ControlToValidate="TextboxCountry" Text="<%# Me.LocalizedTextRequiredField %>" /></font>
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.UpdateProfile_Descr_Phone %> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p><font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxPhone" maxLength="30" style="width: 200px" /> &nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.UpdateProfile_Descr_Fax %> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p><font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxFax" maxLength="30" style="width: 200px" /> &nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.UpdateProfile_Descr_Mobile %> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p><font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxMobile" maxLength="30" style="width: 200px" /> &nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.UpdateProfile_Descr_PositionInCompany %> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p><font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxPositionInCompany" maxLength="30" style="width: 200px" /> &nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" colspan="2">
                                    <p>&nbsp; </p>
                                </td>
                            </tr>
                            <tr>
                                <td bgcolor="#C1C1C1" colspan="2">
                                    <p><font face="Arial" size="2"><b><%= cammWebManager.Internationalization.CreateAccount_Descr_Motivation %>: <font color="#FFFFFF"> *</font> </b></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" colspan="2"><font face="Arial" size="2"><asp:CheckboxList runat="server" id="CheckboxListMotivation" Font-Size="10pt" /></font></td>
                            </tr>
                            <tr>
                                <td valign="Top" colspan="2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<font face="Arial" size="2"><asp:Textbox runat="server" id="MotivationOtherText" style="width: 200px" /> 
								<asp:CustomValidator Display="Dynamic" runat="server" id="ValidatorMotivation" Text="<%# Me.LocalizedTextRequiredField %>" OnServerValidate="CustomValidatorMotivation_ServerValidate" /></font></td>
                            </tr>
                            <tr>
                                <td valign="Top" colspan="2">
                                    <p>&nbsp; </p>
                                </td>
                            </tr>
                            <tr>
                                <td bgcolor="#C1C1C1" colspan="2">
                                    <p><font face="Arial" size="2"><b><%= cammWebManager.Internationalization.CreateAccount_Descr_WhereHeard %>: <font color="#FFFFFF"> *</font> </b></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" colspan="2"><font face="Arial" size="2"><asp:RadioButtonList runat="server" id="RadioListComesFrom" Font-Size="10pt" /></font></td>
                            </tr>
                            <tr>
                                <td valign="Top" colspan="2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<font face="Arial" size="2"><asp:Textbox runat="server" id="ComesFromOtherText" style="width: 200px" /> 
								<asp:CustomValidator Display="Dynamic" runat="server" id="ValidatorComesFrom" Text="<%# Me.LocalizedTextRequiredField %>" OnServerValidate="CustomValidatorComesFrom_ServerValidate" /></font></td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p>&nbsp;</p>
                                </td>
                            </tr>
                            <tr>
                                <td bgcolor="#C1C1C1" colspan="2">
                                    <p><font face="Arial" size="2"><b><%= cammWebManager.Internationalization.CreateAccount_Descr_UserDetails %>:</b></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_1stPreferredLanguage %> <font color="#C1C1C1"> *</font> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p>
                                        <font face="Arial" size="2"><asp:DropdownList runat="server" id="Dropdown1stPreferredLanguage" style="width: 200px" /> 
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="Validator1stPreferredLanguage" ControlToValidate="Dropdown1stPreferredLanguage" Text="<%# Me.LocalizedTextRequiredField %>" /></font>
                                    </p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_2ndPreferredLanguage %> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p><font face="Arial" size="2"><asp:DropdownList runat="server" id="Dropdown2ndPreferredLanguage" style="width: 200px" /></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.CreateAccount_Descr_3rdPreferredLanguage %> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p><font face="Arial" size="2"><asp:DropdownList runat="server" id="Dropdown3rdPreferredLanguage" style="width: 200px" /></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p>&nbsp;</p>
                                </td>
                            </tr>
                            <tr>
                                <td bgcolor="#C1C1C1" colspan="2">
                                    <p><font face="Arial" size="2"><b><%= cammWebManager.Internationalization.CreateAccount_Descr_Comments %>:</b></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" colspan="2">
                                    <p><font face="Arial" size="2"><asp:Textbox runat="server" id="TextboxComment" TextMode="MultiLine" style="width: 400px; height: 150px;" /></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"> &nbsp;</font></p>
                                </td>
                                <td valign="Top" width="240">
                                    <p>&nbsp;</p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="Top" width="160">
                                    <p><font face="Arial" size="2"><asp:Button runat="server" id="SubmitButton" /></font></p>
                                </td>
                                <td valign="Middle" align="Right" width="240">
                                    <p><font face="Arial" size="2" color="#C1C1C1">* <%= cammWebManager.Internationalization.CreateAccount_Descr_RequiredFields %></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">&nbsp;</td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <p><font face="Arial" size="2"><br /><a href="<%= cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL %>"><%= cammWebManager.Internationalization.CreateAccount_Descr_BackToLogin %></a><br /> &nbsp;</font></p>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
</asp:Content>
