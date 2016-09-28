<%@ Control Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Controls.Administration.UsersAdditionalInformation" %>
<%
	if Not MyUserInfo Is Nothing Then 
		Dim MyGroupInfosAllowRule As CompuMaster.camm.WebManager.WMSystem.GroupInformation() = MyUserInfo.MembershipsByRule.AllowRule
		Dim MyGroupInfosDenyRule As CompuMaster.camm.WebManager.WMSystem.GroupInformation() = MyUserInfo.MembershipsByRule.DenyRule
		if (Not MyGroupInfosAllowRule Is Nothing AndAlso MyGroupInfosAllowRule.Length <> 0) OrElse (Not MyGroupInfosDenyRule Is Nothing AndAlso MyGroupInfosDenyRule.Length <> 0) then 
			Array.Sort(MyGroupInfosAllowRule, AddressOf SortMemberships)
			Array.Sort(MyGroupInfosDenyRule, AddressOf SortMemberships)
			%>
			<TR>
			<TD VAlign="Top" ColSpan="2"><P>&nbsp; </P></TD>
			</TR><TR>
			<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size=2><b>Memberships:</b></FONT></P></TD>
			</TR>
			<%
			For Each row As Data.DataRow In SortedGroups(MyGroupInfosAllowRule).Rows
				Dim MyGroupInfo As CompuMaster.camm.WebManager.WMSystem.GroupInformation = row("GroupInfo")
				Dim DisplayName as string = ""
				Dim ID as string = ""
				Try
					DisplayName = MyGroupInfo.Description
					ID = MyGroupInfo.ID
				Catch
					DisplayName = "<em>(error)</em>"
				End Try
				%>
						<TR>
						<TD VAlign="Top" Width="200"><P><FONT face="Arial" size=2><%= "GRANT: " %> <a href="groups_update.aspx?ID=<%= ID %>"><%= Server.HtmlEncode(MyGroupInfo.InternalName) %></a></FONT></P></TD>
						<TD VAlign="Top" Width="200"><P><FONT face="Arial" size=2><%= Server.HtmlEncode(DisplayName) %></FONT></P></TD>
						</TR>
				<% 
	        Next
			For Each row As Data.DataRow In SortedGroups(MyGroupInfosDenyRule).Rows
				Dim MyGroupInfo As CompuMaster.camm.WebManager.WMSystem.GroupInformation = row("GroupInfo")
				Dim DisplayName as string = ""
				Dim ID as string = ""
				Try
					DisplayName = MyGroupInfo.Description
					ID = MyGroupInfo.ID
				Catch
					DisplayName = "<em>(error)</em>"
				End Try
				%>
						<TR>
						<TD VAlign="Top" Width="200"><P><FONT face="Arial" size=2><%= "DENY: " %> <a href="groups_update.aspx?ID=<%= ID %>"><%= Server.HtmlEncode(MyGroupInfo.InternalName) %></a></FONT></P></TD>
						<TD VAlign="Top" Width="200"><P><FONT face="Arial" size=2><%= Server.HtmlEncode(DisplayName) %></FONT></P></TD>
						</TR>
				<% 
	        Next
	    end if


        Dim MyUserAuths As CompuMaster.camm.WebManager.WMSystem.SecurityObjectAuthorizationForUser() 
		MyUserAuths = CombineAuthorizations(MyUserInfo)
	    If Not MyUserAuths Is Nothing Then
				Array.Sort(MyUserAuths, AddressOf SortAuthorizations)
				%>
				<TR>
				<TD VAlign="Top" ColSpan="2"><P>&nbsp; </P></TD>
				</TR><TR>
				<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size=2><b>Authorizations:</b></FONT></P></TD>
				</TR>
				<%
			For Each MyUserAuthInfo As CompuMaster.camm.WebManager.WMSystem.SecurityObjectAuthorizationForUser In MyUserAuths
			Dim GrantDenyInfo As String = ""
			Dim IsDevInfo As String = ""
				           
		Try
			Dim SecObjID As String = MyUserAuthInfo.SecurityObjectInfo.ID
			Dim SecObjName As String = MyUserAuthInfo.SecurityObjectInfo.DisplayName
			If MyUserAuthInfo.IsDenyRule = False Then
				GrantDenyInfo = "GRANT: "
			Else
				GrantDenyInfo = "DENY: "
			End If
			If MyUserAuthInfo.IsDeveloperAuthorization = True Then
				IsDevInfo = "{Dev}"
			End If
					%>
						<TR>
						<TD colspan="2"><TABLE WIDTH="100%" BORDER="0" CELLPADDING="0" CELLSPACING="0"><TR>
						<TD width="160" VAlign="Top"><P><FONT face="Arial" size=2><%= GrantDenyInfo %> ID <%= SecObjID %><strong> <%= IsDevInfo %></strong></FONT></P></TD>
						<TD width="240" VAlign="Top"><P><FONT face="Arial" size=2><%= Server.HtmlEncode(SecObjName) %></FONT></P></TD>
						</TR></TABLE></TD>
						</TR>
					<% 
		Catch
			cammWebManager.Log.Warn ("Missing security object with ID " & MyUserAuthInfo.SecurityObjectID & " in authorizations for group ID " & MyUserInfo.ID)
					%>
						<TR>
						<TD colspan="2"><TABLE WIDTH="100%" BORDER="0" CELLPADDING="0" CELLSPACING="0"><TR>
						<TD width="160" VAlign="Top"><P><FONT face="Arial" size=2><%= GrantDenyInfo %> <em>ID <%= MyUserAuthInfo.SecurityObjectID %></em><strong> <%= IsDevInfo %></strong></FONT></P></TD>
						<TD width="240" VAlign="Top"><P><FONT face="Arial" size=2><em>Missing security object</em></FONT></P></TD>
						</TR></TABLE></TD>
						</TR>
					<% 
		End Try
            Next
        End If
	End If

%>