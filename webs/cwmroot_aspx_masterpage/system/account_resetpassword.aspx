<%@ Page MasterPageFile="~/portal/MasterPage.master" ValidateRequest="false" Inherits="CompuMaster.camm.WebManager.Pages.UserAccount.ResetUserPassword" Language="VB" %>

<%@ Import Namespace="CompuMaster.camm.WebManager.WMSystem" %>
<%@ Import Namespace="CompuMaster.camm.WebManager" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<asp:content id="Content1" contentplaceholderid="head" runat="server">
<camm:WebManager id="cammWebManager" runat="server" />
</asp:content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <table cellspacing="10" cellpadding="0" bgcolor="#ffffff" border="0">
        <tbody>
            <tr>
                <td valign="top">
                    <p><font face="Arial" size="3"><b><%= cammWebManager.Internationalization.UpdatePW_Descr_Title %></b></font></p>
                    <asp:Literal runat="server" ID="Message" />
                    <%
                        If Not HideForm Then
                    %>
                    <table cellspacing="0" cellpadding="0" bgcolor="#ffffff" border="0" bordercolor="#C1C1C1">
                        <tbody>
                            <tr>
                                <td valign="top">
                                    <form runat="server" method="POST" action="" id="form1" name="form1">
                                        <input type="hidden" name="loginname" value="<%= cammWebManager.CurrentUserLoginName %>">
                                        <table cellspacing="0" cellpadding="3" width="100%" border="0" bordercolor="#FFFFFF">
                                            <tbody>
                                                <tr>
                                                    <td bgcolor="#C1C1C1" colspan="2">
                                                        <p><font face="Arial" size="2"><b><%= cammWebManager.Internationalization.ResetPW_Descr_PleaseSpecifyNewPW %></b></font></p>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="Top" width="160">
                                                        <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.UpdatePW_Descr_NewPW %> <font color="#C1C1C1"> *</font> </font></p>
                                                    </td>
                                                    <td valign="Top" width="240">
                                                        <p><font face="Arial" size="2"><asp:TextBox id="NewPassword" TextMode="password" runat="server" /></asp></font></p>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="Top" width="160">
                                                        <p><font face="Arial" size="2"><%= cammWebManager.Internationalization.UpdatePW_Descr_NewPWConfirm %> <font color="#C1C1C1"> *</font> </font></p>
                                                    </td>
                                                    <td valign="Top" width="240">
                                                        <p><font face="Arial" size="2"><asp:TextBox id="NewPasswordConfirm" TextMode="password" runat="server"/></font></p>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="Top" width="160">
                                                        <p><font face="Arial" size="2"><input type="submit" name="submit" value="<%= Server.htmlencode(cammWebManager.Internationalization.UpdatePW_Descr_Submit) %>"></font></p>
                                                    </td>
                                                    <td valign="Middle" align="Right" width="240">
                                                        <p><font face="Arial" size="2" color="#C1C1C1"><%= cammWebManager.Internationalization.UpdatePW_Descr_RequiredFields %></font></p>
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
        </tbody>
    </table>
    <%
        End If
    %>
</asp:Content>
