<%@ Control Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Controls.Administration.GroupsAdditionalInformation" %>
<%

	'====================================================================================
	'In opposite to users_additionalinformation.ascx, this control doesn't contain 
	'the complete list of members, because this would increase this document size too much
	'====================================================================================


		Dim MyGroupAuths As CompuMaster.camm.WebManager.WMSystem.SecurityObjectAuthorizationForGroup() 
		MyGroupAuths = Me.CombineAuthorizations(MyGroupInfo)
        If Not MyGroupAuths Is Nothing Then
				Array.Sort(MyGroupAuths, AddressOf SortAuthorizations)
				%>
				<TR>
				<TD VAlign="Top" ColSpan="2"><P>&nbsp; </P></TD>
				</TR><TR>
				
				<TD colspan="2" bgcolor="#C1C1C1"><table width="100%" cellspacing="0" cellpadding="0" border="0"><tr><td><P><FONT face="Arial" size=2><b>Authorizations:</b></font></p></td><td align="right"><p><FONT face="Arial" size=2><a runat="server" id="ancPreview"></a></FONT></P></TD></tr></table></td>
				</TR>
				<%
            For Each MyGroupAuthInfo As CompuMaster.camm.WebManager.WMSystem.SecurityObjectAuthorizationForGroup In MyGroupAuths
			Dim GrantDenyInfo As String = ""
			Dim IsDevInfo As String = ""
		Try
			Dim SecObjID As String = MyGroupAuthInfo.SecurityObjectInfo.ID
			Dim SecObjName As String = MyGroupAuthInfo.SecurityObjectInfo.DisplayName
			If MyGroupAuthInfo.IsDenyRule = False Then
				GrantDenyInfo = "GRANT: "
			Else
				GrantDenyInfo = "DENY: "
			End If
			If MyGroupAuthInfo.IsDevRule = True Then
				IsDevInfo = "{Dev}"
			End If
					%>
						<TR>
						<TD colspan="2"><TABLE WIDTH="100%" BORDER="0" CELLPADDING="0" CELLSPACING="0"><TR>
						<TD width="160" VAlign="Top"><P><FONT face="Arial" size=2><%= GrantDenyInfo %> ID <%= Server.HTMLEncode(SecObjID) %><strong> <%= IsDevInfo %></strong></FONT></P></TD>
						<TD width="240" VAlign="Top"><P><FONT face="Arial" size=2><%= Server.HTMLEncode(SecObjName) %></FONT></P></TD>
						</TR></TABLE></TD>
						</TR>
					<% 
		Catch
			cammWebManager.Log.Warn ("Missing security object with ID " & MyGroupAuthInfo.SecurityObjectID & " in authorizations for group ID " & MyGroupInfo.ID)
					%>
						<TR>
						<TD colspan="2"><TABLE WIDTH="100%" BORDER="0" CELLPADDING="0" CELLSPACING="0"><TR>
						<TD width="160" VAlign="Top"><P><FONT face="Arial" size=2><%= GrantDenyInfo %> <em>ID <%= MyGroupAuthInfo.SecurityObjectID %></em><strong> <%= IsDevInfo %></strong></FONT></P></TD>
						<TD width="240" VAlign="Top"><P><FONT face="Arial" size=2><em>Missing security object</em></FONT></P></TD>
						</TR></TABLE></TD>
						</TR>
					<% 
		End Try
            Next
        End If

%>