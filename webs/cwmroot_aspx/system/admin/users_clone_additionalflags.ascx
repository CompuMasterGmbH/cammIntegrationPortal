<%@ Control Language="vb" AutoEventWireup="false" %>
<script language="Vb" runat="server">
public MyUserInfo as CompuMaster.camm.WebManager.WMSystem.UserInformation
public cammWebManager as CompuMaster.camm.WebManager.WMSystem
</script>
<%
        If Not MyUserInfo.AdditionalFlags Is Nothing Then
			%>
			<TR>
			<TD VAlign="Top" ColSpan="2"><P>&nbsp; </P></TD>
			</TR><TR>
			<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size=2><b>Additional flags:</b></FONT></P></TD>
			<td BGCOLOR="#C1C1C1">Copy checked user flags</td>
			</TR>
			<%
            For MyCounter As Integer = 1 To MyUserInfo.AdditionalFlags.Count
		Dim MyKeyName as string = MyUserInfo.AdditionalFlags.Keys.Item(mycounter-1)
		Dim MyItemValue as string = MyUserInfo.AdditionalFlags.Item(mycounter-1)

		If MyItemValue = "" OrElse LCase(MyKeyName) = "isdeleteduser" OrElse LCase(MyKeyName) = "phone" OrElse LCase(MyKeyName) = "mobile" OrElse LCase(MyKeyName) = "fax" OrElse LCase(MyKeyName) = "position" Then
			%><!-- <TR><TD VAlign="Top" Width="200"><P><FONT face="Arial" size=2><%= Server.HtmlEncode(MyKeyName) %></FONT></P></TD><TD VAlign="Top" Width="200"><P><FONT face="Arial" size=2><a target="_blank" href="users_update_flag.aspx<%= "?ID=" & Request.QueryString ("ID") & "&Type=" & Server.URLEncode(MyKeyName) %>"><em>(empty)</em></a></FONT></P></TD> --><%
		Else
			%><TR><TD VAlign="Top" Width="200"><P><FONT face="Arial" size=2><%= Server.HtmlEncode(MyKeyName) %></FONT></P></TD><TD VAlign="Top" Width="200"><P><FONT face="Arial" size=2><a target="_blank" href="users_update_flag.aspx<%= "?ID=" & Request.QueryString ("ID") & "&Type=" & Server.URLEncode(MyKeyName) %>"><%= Server.HtmlEncode(MyItemValue) %></a></FONT></P></TD>
			<td valign="top"><asp:CheckBox runat="server" Text="copy" Checked="true" /></td><%
		End If

            Next
			%></TR>
			<TR><TD VAlign="Top" Width="200"><P><FONT face="Arial" size=2><a target="_blank" href="users_update_flag.aspx?ID=<%= Request.QueryString ("ID") %>">New</a></FONT></P></TD><TD VAlign="Top" Width="200"><P></P></TD></TR>
			<%

        End If
%>