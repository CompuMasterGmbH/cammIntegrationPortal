<%@ Control Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Controls.Administration.UsersAdditionalInformation" %>
<%
    Dim MyGroupInfos As CompuMaster.camm.WebManager.WMSystem.GroupInformation() = MyUserInfo.Memberships
    Dim DisplayName As String = ""
    If Not MyGroupInfos Is Nothing Then
	Array.Sort(MyGroupInfos, AddressOf SortMemberships)
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
            <font face="Arial" size="2"><b>Memberships:</b></font></p>
    </td>
    <td bgcolor="#C1C1C1">
        Copy checked memberships
    </td>
</tr>
<%
    
    For Each MyGroupInfo As CompuMaster.camm.WebManager.WMSystem.GroupInformation In MyGroupInfos
        DisplayName = ""
        Dim ID As String = ""
        Try
            DisplayName = MyGroupInfo.Description
            ID = MyGroupInfo.ID
        Catch
            DisplayName = "<em>(error)</em>"
        End Try
        
        Dim HtmlCode As New StringBuilder
        HtmlCode.Append("<tr>" & vbNewLine)
        HtmlCode.Append("    <td valign=""Top"" width=""200"">" & vbNewLine)
        HtmlCode.Append("        <p>" & vbNewLine)
        HtmlCode.Append("            <font face=""Arial"" size=""2""><a href=""groups_update.aspx?ID=" & ID & """>" & vbNewLine)
        HtmlCode.Append(Server.HtmlEncode(MyGroupInfo.Name) & "</a></font></p>" & vbNewLine)
        HtmlCode.Append("    </td>" & vbNewLine)
        HtmlCode.Append("    <td valign=""Top"" width=""200"">" & vbNewLine)
        HtmlCode.Append("        <p>" & vbNewLine)
        HtmlCode.Append("            <font face=""Arial"" size=""2"">" & vbNewLine)
        HtmlCode.Append(Server.HtmlEncode(DisplayName) & "</font></p>" & vbNewLine)
        HtmlCode.Append("    </td>" & vbNewLine)
        HtmlCode.Append("    <td>" & vbNewLine)
        
        PnlGroupsInformation.Controls.Add(New LiteralControl(HtmlCode.ToString))
        
        Dim Chk As New CheckBox
        Chk.ID = "ChkMemberships_" & MyGroupInfo.id
        Chk.Text = "copy"
        Chk.Checked = True
        PnlGroupsInformation.Controls.Add(Chk)
        
        HtmlCode = New StringBuilder
        HtmlCode.Append("    </td>" & vbNewLine)
        HtmlCode.Append("</tr>" & vbNewLine)
        
        PnlGroupsInformation.Controls.Add(New LiteralControl(HtmlCode.ToString))


    Next

End If
%>
<asp:Panel runat="server" ID="PnlGroupsInformation" />
<% 
	Dim Auths As CompuMaster.camm.WebManager.WMSystem.Authorizations
    Auths = New CompuMaster.camm.WebManager.WMSystem.Authorizations(Nothing, cammWebManager, Nothing, Nothing, MyUserInfo.IDLong)

    Dim MyUserAuths As CompuMaster.camm.WebManager.WMSystem.Authorizations.UserAuthorizationInformation()
    MyUserAuths = Auths.UserAuthorizationInformations(MyUserInfo.ID)
	If Not MyUserAuths Is Nothing Then
		Array.Sort(MyUserAuths, AddressOf SortAuthorizations)
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
            <font face="Arial" size="2"><b>Authorizations:</b></font></p>
    </td>
    <td bgcolor="#C1C1C1">
        Copy checked authorizations
    </td>
</tr>
<%
    For Each MyUserAuthInfo As CompuMaster.camm.WebManager.WMSystem.Authorizations.UserAuthorizationInformation In MyUserAuths
        Try
            Dim SecObjID As String = MyUserAuthInfo.SecurityObjectInfo.ID
            Dim SecObjName As String = MyUserAuthInfo.SecurityObjectInfo.DisplayName
            
            Dim HtmlStr As New System.Text.StringBuilder
            HtmlStr.Append("<tr>" & vbNewLine)
            HtmlStr.Append("    <td colspan=""2"">" & vbNewLine)
            HtmlStr.Append("        <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"">" & vbNewLine)
            HtmlStr.Append("            <tr>" & vbNewLine)
            HtmlStr.Append("                <td width=""160"" valign=""Top"">" & vbNewLine)
            HtmlStr.Append("                    <p>" & vbNewLine)
            HtmlStr.Append("                        <font face=""Arial"" size=""2"">ID" & vbNewLine)
            HtmlStr.Append(SecObjID & "</font></p>" & vbNewLine)
            HtmlStr.Append("                </td>" & vbNewLine)
            HtmlStr.Append("                <td width=""240"" valign=""Top"">" & vbNewLine)
            HtmlStr.Append("                    <p>" & vbNewLine)
            HtmlStr.Append("                        <font face=""Arial"" size=""2"">" & vbNewLine)
            HtmlStr.Append(Server.HtmlEncode(SecObjName) & "</font></p>" & vbNewLine)
            HtmlStr.Append("                </td>" & vbNewLine)
            HtmlStr.Append("                <td valign=""top"">" & vbNewLine)
            PnlAuth.Controls.Add(New LiteralControl(HtmlStr.ToString))
            
            Dim Chk As New CheckBox
            Chk.ID = "ChkAuth_" & SecObjID
            Chk.Text = "copy"
            Chk.Checked = True
            PnlAuth.Controls.Add(Chk)
            
            HtmlStr = New StringBuilder
            HtmlStr.Append("                </td>" & vbNewLine)
            HtmlStr.Append("            </tr>" & vbNewLine)
            HtmlStr.Append("        </table>" & vbNewLine)
            HtmlStr.Append("    </td>" & vbNewLine)
            HtmlStr.Append("</tr>" & vbNewLine)
            PnlAuth.Controls.Add(New LiteralControl(HtmlStr.ToString))
            
%>
<asp:Panel ID="PnlAuth" runat="server" />
<% 
Catch
    cammWebManager.Log.Warn("Missing security object with ID " & MyUserAuthInfo.SecurityObjectID & " in authorizations for group ID " & MyUserInfo.ID)
%>
<tr>
    <td colspan="2">
        <table width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td width="160" valign="Top">
                    <p>
                        <font face="Arial" size="2"><em>ID
                            <%= MyUserAuthInfo.SecurityObjectID %></em></font></p>
                </td>
                <td width="240" valign="Top">
                    <p>
                        <font face="Arial" size="2"><em>Missing security object</em></font></p>
                </td>
            </tr>
        </table>
    </td>
</tr>
<% 
End Try
Next
End If

%>