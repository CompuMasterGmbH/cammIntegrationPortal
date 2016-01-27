<%@ Page MasterPageFile="~/portal/MasterPage.master" ValidateRequest="false" Inherits="CompuMaster.camm.WebManager.Pages.UserAccount.ChangeUserPassword" Language="VB" %>

<%@ Import Namespace="CompuMaster.camm.WebManager.WMSystem" %>
<%@ Import Namespace="CompuMaster.camm.WebManager" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager ID="cammWebManager" SecurityObject="@@Public" runat="server" />

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <%
        If HideForm = False Then
    %>
    <table cellspacing="10" cellpadding="0" bgcolor="#ffffff" border="0">
        <tbody>
            <tr>
                <td valign="top">
                    <p><font face="Arial" size="3"><b><%= cammWebManager.Internationalization.UpdatePW_Descr_Title %></b></font></p>
                    <% If ErrMsg <> "" Then Response.Write("<p><font face=""Arial"" size=""2"" color=""red"">" & ErrMsg & "</font></p>") %>
                    <table cellspacing="0" cellpadding="0" bgcolor="#ffffff" border="0" bordercolor="#C1C1C1">
                        <tbody>
                            <tr>
                                <td valign="top">
                                    <no-more-form method="POST" action="EX-SCRIPT" id="form1" name="form1">EX-SCRIPT=%%%= Response.ApplyAppPathModifier(Request.ServerVariables("SCRIPT_NAME")) %%%
                                        <input type="hidden" name="loginname" value="<%= cammWebManager.CurrentUserLoginName %>" />
                                        <table cellspacing="0" cellpadding="3" width="100%" border="0" bordercolor="#FFFFFF">
                                            <tbody>
                                                <tr>
                                                    <td bgcolor="#C1C1C1" colspan="2">
                                                        <p><font face="Arial" size="2"><b><%= cammWebManager.Internationalization.UpdatePW_Descr_PleaseSpecifyCurrendAndOldPW %></b></font></p>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="Top" width="160">
                                                        <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.UpdatePW_Descr_CurrentPW %> <font color="#C1C1C1"> *</font> </font></p>
                                                    </td>
                                                    <td valign="Top" width="240">
                                                        <p><font face="Arial" size="2"><input style="width: 200px" type="password" name="oldpassword" value="" /></font></p>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="Top" width="160">
                                                        <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.UpdatePW_Descr_NewPW %> <font color="#C1C1C1"> *</font> </font></p>
                                                    </td>
                                                    <td valign="Top" width="240">
                                                        <p><font face="Arial" size="2"><input style="width: 200px" type="password" name="newpassword1" value="" /></font></p>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="Top" width="160">
                                                        <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.UpdatePW_Descr_NewPWConfirm %> <font color="#C1C1C1"> *</font> </font></p>
                                                    </td>
                                                    <td valign="Top" width="240">
                                                        <p><font face="Arial" size="2"><input style="width: 200px" type="password" name="newpassword2" value="" /></font></p>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="Top" width="160">
                                                        <p><font face="Arial" size="2"><input type="submit" name="submit" value="<%= Server.HtmlEncode(cammWebManager.Internationalization.UpdatePW_Descr_Submit) %>" /></font></p>
                                                    </td>
                                                    <td valign="Middle" align="Right" width="240">
                                                        <p><font face="Arial" size="2" color="#C1C1C1"><%= cammWebManager.Internationalization.UpdatePW_Descr_RequiredFields %></font></p>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </no-more-form>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
    <%
        Else
    %>
    <asp:Literal runat="server" ID="Message" />
    <%
        End If
    %>
</asp:Content>
