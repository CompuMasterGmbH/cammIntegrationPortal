<%@ Page MasterPageFile="~/portal/MasterPage.master" ValidateRequest="False" Inherits="CompuMaster.camm.WebManager.Pages.Login.LoginForm" Language="VB" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager ID="cammWebManager" runat="server" />

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <table border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td>
                <%
                    If ErrorMessageForUser <> "" Then Response.Write(ErrorMessageForUser)
                %>
                <table border="0" width="100%" cellspacing="0" cellpadding="0">
                    <tr>
                        <td><font face="Arial" size="4"><%= cammWebManager.Internationalization.Logon_BodyTitle %></font>
                            <br />
                            &nbsp;
                        <hr>
                            <font face="Arial" size="2"><%= cammWebManager.Internationalization.Logon_BodyPrompt2User %></font>
                            <hr>
                        </td>
                        <td width="140" valign="bottom">
                            <img border="0" src="<%= cammWebManager.System_GetServerGroupImageSmallAddr(cammWebManager.CurrentServerIdentString) %>" align="right" width="100" height="94"></td>
                    </tr>
                </table>
                <form name="formlogin" method="post" action="<%= cammWebManager.Internationalization.User_Auth_Validation_CheckLoginURL %>">
                    <table cellspacing="2" cellpadding="0" align="left" border="0">
                        <tr>
                            <td width="100"><font face="Arial" size="3"><nobr><%= cammWebManager.Internationalization.Logon_BodyFormUserName %>:&nbsp;</nobr></font></td>
                            <td><font face="Arial" size="3"><input name="Username" /></font></td>
                        </tr>
                        <tr>
                            <td width="100"><font face="Arial" size="3"><nobr><%= cammWebManager.Internationalization.Logon_BodyFormUserPassword %>:&nbsp;</nobr></font></td>
                            <td><font face="Arial" size="3"><input type="password" name="Passcode" /></font></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <p>
                                    <input type="submit" value="<%= Server.HtmlEncode(cammWebManager.Internationalization.Logon_BodyFormSubmit) %>" name="submit" />
                                    &nbsp;<font face="Arial" size="3"><input type="button" value="<%= Server.HtmlEncode(cammWebManager.Internationalization.Logon_BodyFormCreateNewAccount) %>" onclick="document.location='<%= cammWebManager.Internationalization.User_Auth_Validation_CreateUserAccountInternalURL %>    ';" /></font>
                                </p>
                            </td>
                        </tr>
                    </table>
                </form>
                <script lang="JavaScript">
                <!--
    document.formlogin.Username.focus()
    //-->
                </script>
            </td>
        </tr>
        <tr>
            <td><%= cammWebManager.Internationalization.Logon_BodyExplanation %></td>
        </tr>
    </table>
</asp:Content>
