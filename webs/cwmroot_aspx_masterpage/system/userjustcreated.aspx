<%@ Page MasterPageFile="~/portal/MasterPage.master" Language="VB" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager ID="cammWebManager" runat="server" SecurityObject="@@Public" />
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
    <%
        dim AreaImage
        AreaImage = cammWebManager.System_GetServerGroupImageBigAddr(cammWebManager.CurrentServerIdentString)

        cammWebManager.PageTitle = Server.htmlencode(cammWebManager.Internationalization.UserJustCreated_Descr_Title)
    %>

    <table cellspacing="0" cellpadding="0" width="100%" border="0">
        <tbody>
            <tr>
                <td valign="top">
                    <h2>
                        <img src="<%= AreaImage %>" align="right" border="0"><%= cammWebManager.Internationalization.UserJustCreated_Descr_AccountCreated %></h2>
                    <em><font face="Arial" size="3"><%= cammWebManager.Internationalization.UserJustCreated_Descr_LookAroundNow %></font></em>
                    <br />
                    </p>
		<p><font face="Arial" size="3"><%= cammWebManager.Internationalization.UserJustCreated_Descr_PleaseNote %></font></p>
                </td>
            </tr>
        </tbody>
    </table>

</asp:Content>
