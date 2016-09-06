<%@ Page Inherits="CompuMaster.camm.WebManager.Pages.UserAccount.UpdateUserProfile" Language="VB" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" SecurityObject="@@Public" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<script runat="server">

	Private Sub CustomValidatorMotivation_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
		args.IsValid = IsValidFieldMotivation
	End Sub
	
	Protected Overridable Function IsValidFieldMotivation() As Boolean
		For Each item As Web.UI.WebControls.ListItem In Me.CheckboxListMotivation.Items
			If item.Selected Then
				Return True
			End If
		Next
		Return False
	End Function

</script>
	<h3><font face="Arial" size="3"><%= cammWebManager.Internationalization.UpdateProfile_Descr_Title %>: <asp:Label runat="server" id="LabelLoginName" /></font></h3>
	<TABLE cellSpacing=0 cellPadding=0 bgColor="#ffffff" border=0 bordercolor="#C1C1C1">
	  <TBODY>
	  <TR>
            <TD vAlign=top>
	      <TABLE cellSpacing=0 cellPadding=3 width="100%" border="0" bordercolor="#FFFFFF">
	        <TBODY>
			<% If ValidatorSummary.Visible Then %><TR><TD ColSpan="2"><P><FONT face="Arial" color="red" size=2><asp:ValidationSummary runat="server" id="ValidatorSummary" /></FONT></P></TD></TR><% End If %>
                                        <tr>
                                            <td bgcolor="#C1C1C1" colspan="2">
                                                <p>
                                                    <font face="Arial" size="2"><b>
                                                        <%= cammWebManager.Internationalization.UpdateProfile_Descr_Address %>:</b></font></p>
                                            </td>
                                        </tr>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_Company %> <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxCompany" maxLength="100" style="width: 200px" /> 
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorCompany" ControlToValidate="TextboxCompany" Text="<%# Me.LocalizedTextRequiredField %>" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><nobr><%= cammWebManager.Internationalization.UpdateProfile_Descr_Addresses %> <font color="#C1C1C1"> *</font> &nbsp;</nobr></FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:DropdownList runat="server" id="DropdownSalutation" style="width: 200px" /> 
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorSalutation" ControlToValidate="DropdownSalutation" Text="<%# Me.LocalizedTextRequiredField %>" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_AcademicTitle %> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxAcademicTitle" maxLength="40" style="width: 200px" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_FirstName %> <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxFirstName" maxLength="60" style="width: 200px" />
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorFirstName" ControlToValidate="TextboxFirstName" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_LastName %> <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxLastName" maxLength="60" style="width: 200px" />
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorLastName" ControlToValidate="TextboxLastName" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_NameAddition %> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxNameAffix" maxLength="40" style="width: 200px" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_EMail %> <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxEMail" maxLength="50" style="width: 200px" />
								<asp:CustomValidator Display="Dynamic" runat="server" id="ValidatorEMail" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_Street %> <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxStreet" maxLength="60" style="width: 200px" />
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorStreet" ControlToValidate="TextboxStreet" Text="<%# Me.LocalizedTextRequiredField %>" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_ZIPCode %> <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxZipCode" maxLength="20" style="width: 200px" />
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorZipCode" ControlToValidate="TextboxZipCode" Text="<%# Me.LocalizedTextRequiredField %>" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_Location %> <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxLocation" maxLength="100" style="width: 200px" />
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorLocation" ControlToValidate="TextboxLocation" Text="<%# Me.LocalizedTextRequiredField %>" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_State %> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxState" maxLength="60" style="width: 200px" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_Country %><font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:DropdownList width="200px" runat="server" id="DropdownCountry" Visible="False" /><asp:Textbox runat="server" id="TextboxCountry" maxLength="60" style="width: 200px" />
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorCountry" ControlToValidate="TextboxCountry" Text="<%# Me.LocalizedTextRequiredField %>" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_Phone %> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxPhone" maxLength="255" style="width: 200px" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_Fax %> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxFax" maxLength="255" style="width: 200px" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_Mobile %> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxMobile" maxLength="255" style="width: 200px" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_PositionInCompany %> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxPositionInCompany" maxLength="255" style="width: 200px" /> &nbsp;</FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" ColSpan="2"><P>&nbsp; </P></TD>
								</TR>
								<TR>
								<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size=2><b><%= cammWebManager.Internationalization.CreateAccount_Descr_Motivation %>: <font color="#FFFFFF"> *</font> </b></FONT></P></TD>
								</TR>								
								<TR><TD VAlign="Top" ColSpan="2"><FONT face=Arial size=2><asp:CheckboxList runat="server" id="CheckboxListMotivation" Font-Size="10pt" /></FONT></TD></TR>
								<TR><TD VAlign="Top" ColSpan="2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<FONT face=Arial size=2><asp:Textbox runat="server" id="MotivationOtherText" maxLength="255" style="width: 200px" /> 
								<asp:CustomValidator Display="Dynamic" runat="server" id="ValidatorMotivation" Text="<%# Me.LocalizedTextRequiredField %>" OnServerValidate="CustomValidatorMotivation_ServerValidate" /></FONT></TD></TR>
								<TR>
								<TD VAlign="Top" ColSpan="2"><P>&nbsp; </P></TD>
								</TR>
								<TR>
								<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size=2><b><%= cammWebManager.Internationalization.UpdateProfile_Descr_UserDetails %>:</b></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.CreateAccount_Descr_1stPreferredLanguage %> <font color="#C1C1C1"> *</font> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:DropdownList runat="server" id="Dropdown1stPreferredLanguage" style="width: 200px" /> 
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="Validator1stPreferredLanguage" ControlToValidate="Dropdown1stPreferredLanguage" Text="<%# Me.LocalizedTextRequiredField %>" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.CreateAccount_Descr_2ndPreferredLanguage %> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:DropdownList runat="server" id="Dropdown2ndPreferredLanguage" style="width: 200px" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.CreateAccount_Descr_3rdPreferredLanguage %> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:DropdownList runat="server" id="Dropdown3rdPreferredLanguage" style="width: 200px" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" ColSpan="2"><P>&nbsp; </P></TD>
								</TR>
								<TR>
								<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size=2><b><%= cammWebManager.Internationalization.UpdateProfile_Descr_Authentification %>:</b></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= cammWebManager.Internationalization.UpdateProfile_Descr_Password %> *&nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxPassword1" TextMode="Password" maxLength="64" style="width: 200px" autocomplete="new-password" />
								<asp:RequiredFieldValidator Display="Dynamic" runat="server" id="ValidatorPassword" ControlToValidate="TextboxPassword1" Text="<%# Me.LocalizedTextRequiredField %>" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P> &nbsp;</P></TD>
								</TR>
								<TR>
								<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size=2><b><%= cammWebManager.Internationalization.CreateAccount_Descr_Comments %>:</b></FONT></P></TD>
								</TR><TR>
								<TD VAlign="Top" COLSPAN="2"><P><FONT face="Arial" size=2><asp:Textbox runat="server" id="TextboxComment" TextMode="MultiLine" style="width: 400px; height: 150px;" /></FONT></P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2> &nbsp;</FONT></P></TD>
								<TD VAlign="Top" Width="240"><P> &nbsp;</P></TD>
								</TR>
								<TR>
								<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><asp:Button runat="server" id="SubmitButton" /></FONT></P></TD>
								<TD VAlign="Middle" Align="Right" Width="240"><P><font face="Arial" size="2" color="#C1C1C1"><%= cammWebManager.Internationalization.UpdateProfile_Descr_RequiredFields %></font></P></TD>
								</TR>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
