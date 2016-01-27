<%@ Page MasterPageFile="~/portal/MasterPage.master" Inherits="CompuMaster.camm.WebManager.Pages.UserAccount.SendUserPassword" Language="VB" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager ID="cammWebManager" runat="server" />

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <% If Request.Form ("loginname") = "" and Request.Form ("email") = "" then %>
    <table border="0" width="100%" cellspacing="0" cellpadding="0">
        <tr>
            <td>
                <table border="0" cellspacing="40" cellpadding="0">
                    <tr>
                        <td>
                            <p><font face="Arial" size="4"><%= cammWebManager.Internationalization.SendPassword_Descr_Title %></font></p>
                            <p>
                                <hr>
                            </p>
                            <table cellspacing="0" cellpadding="0" bgcolor="#ffffff" border="0" bordercolor="#c1c1c1">
                                <tbody>
                                    <tr>
                                        <td valign="top">
                                            <form action="<%= Response.ApplyAppPathModifier(Request.ServerVariables("SCRIPT_NAME")) %>" method="POST" id="form1" name="form1">
                                                <table cellspacing="0" cellpadding="3" width="100%" border="0" bordercolor="#ffffff">
                                                    <tbody>
                                                        <tr>
                                                            <td valign="top" width="160">
                                                                <p><font face="Arial" size="3"><%= cammWebManager.Internationalization.SendPassword_Descr_LoginName %> <font color="#C1C1C1"> *</font> </font></p>
                                                            </td>
                                                            <td valign="top" width="240">
                                                                <p><font face="Arial" size="3"><input name="loginname" width="30" ></font></p>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td valign="top" width="160">
                                                                <p><font face="Arial" size="3"><%= cammWebManager.Internationalization.SendPassword_Descr_EMail %> <font color="#C1C1C1"> *</font> </font></p>
                                                            </td>
                                                            <td valign="top" width="240">
                                                                <p><font face="Arial" size="3"><input name="email" width="30" ></font></p>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td valign="top" width="160">
                                                                <p><font face="Arial" size="3"><input type="submit" name="submit" value="<%= Server.htmlencode(cammWebManager.Internationalization.SendPassword_Descr_Submit) %>" width="30"></font></p>
                                                            </td>
                                                            <td valign="Middle" align="Right" width="240">
                                                                <p><font face="Arial" size="2" color="#C1C1C1">* <%= cammWebManager.Internationalization.SendPassword_Descr_RequiredFields %></font></p>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td valign="top">
                                                                <p><font face="Arial" size="2"><br /><a href="<%= cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL %>"><%= cammWebManager.Internationalization.SendPassword_Descr_BackToLogin %></a><br /> &nbsp;</font></p>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </form>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                </table>
                <% Else %>
                <%
            'Message verschicken
            Dim SuccessMessage As String
            Try
                ValidateInputAndSendMail(Trim(Request.Form("loginname")), Trim(Request.Form("email")))
                SuccessMessage = String.Format(cammWebManager.Internationalization.SendPassword_Descr_PasswordSentTo, Request.Form("email"))
            Catch ex As Exception
                SuccessMessage = "<font face=""Arial"" size=""3"" color=""red"">" & ex.Message & "</font>"
            End Try
                %>
                <table border="0" width="100%" cellspacing="0" cellpadding="0">
                    <tr>
                        <td>
                            <table border="0" cellspacing="40" cellpadding="0">
                                <tr>
                                    <td>
                                        <p><font face="Arial" size="3"><b><%= cammWebManager.Internationalization.SendPassword_Descr_Title %></b></font></p>
                                        <table cellspacing="0" cellpadding="0" bgcolor="#ffffff" border="0" bordercolor="#c1c1c1">
                                            <tbody>
                                                <tr>
                                                    <td valign="top">
                                                        <table cellspacing="0" cellpadding="3" width="100%" border="0" bordercolor="#ffffff">
                                                            <tbody>
                                                                <tr>
                                                                    <td valign="top">
                                                                        <p><font face="Arial" size="2"><%= SuccessMessage %><br /> &nbsp;</font></p>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td valign="top">
                                                                        <p><font face="Arial" size="2"><%= String.Format(cammWebManager.Internationalization.SendPassword_Descr_FurtherCommentWithContactAddress, cammWebManager.StandardEMailAccountAddress, cammWebManager.StandardEMailAccountAddress) %></font></p>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td valign="top">
                                                                        <p><font face="Arial" size="2"><br /><a href="<%= cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL %>"><%= cammWebManager.Internationalization.SendPassword_Descr_BackToLogin %></a><br /> &nbsp;</font></p>
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
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <%
End If %>
</asp:Content>
