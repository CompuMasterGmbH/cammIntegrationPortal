<%@ Control Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Controls.Administration.UsersAdditionalInformation" %>
<%
	if Not MyUserInfo Is Nothing Then 
		Dim MyGroupInfos As CompuMaster.camm.WebManager.WMSystem.GroupInformation() = MyUserInfo.Memberships
		if Not MyGroupInfos Is Nothing then 
			Array.Sort(MyGroupInfos, AddressOf SortMemberships)
			%>
			<TR>
			<TD VAlign="Top" ColSpan="2"><P>&nbsp; </P></TD>
			</TR><TR>
			<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size=2><b>Memberships:</b></FONT></P></TD>
			</TR>
			<%
			Dim sortDt as new Data.DataTable("sortDT")
				sortDt.Columns.Add("GroupInfo", GetType(CompuMaster.camm.WebManager.WMSystem.GroupInformation))
				sortDt.Columns.Add("GroupName")
            For Each tmpMyGroupInfo As CompuMaster.camm.WebManager.WMSystem.GroupInformation In MyGroupInfos
				Dim tmpRow as Data.DataRow = sortDt.NewRow()
				tmpRow("GroupInfo") = tmpMyGroupInfo
				tmpRow("GroupName") = tmpMyGroupInfo.InternalName
				sortDt.Rows.Add(tmpRow)
            Next
				sortDt.DefaultView.Sort = "GroupName"		
				
		For Each row As Data.DataRow In sortDt.Rows
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
						<TD VAlign="Top" Width="200"><P><FONT face="Arial" size=2><a href="groups_update.aspx?ID=<%= ID %>"><%= Server.HtmlEncode(MyGroupInfo.InternalName) %></a></FONT></P></TD>
						<TD VAlign="Top" Width="200"><P><FONT face="Arial" size=2><%= Server.HtmlEncode(DisplayName) %></FONT></P></TD>
						</TR>
				<% 
	        Next
	        sortDt.Dispose()
	    end if


        Dim Auths As CompuMaster.camm.WebManager.WMSystem.Authorizations
        Auths = New CompuMaster.camm.WebManager.WMSystem.Authorizations(Nothing, cammWebManager, Nothing, Nothing, MyUserInfo.IDLong)
        Dim MyUserAuths As CompuMaster.camm.WebManager.WMSystem.Authorizations.UserAuthorizationInformation() 
		MyUserAuths = Auths.UserAuthorizationInformations(MyUserInfo.ID)
	    If Not MyUserAuths Is Nothing Then
				Array.Sort(MyUserAuths, AddressOf SortAuthorizations)
				%>
				<TR>
				<TD VAlign="Top" ColSpan="2"><P>&nbsp; </P></TD>
				</TR><TR>
				<TD BGCOLOR="#C1C1C1" ColSpan="2"><P><FONT face="Arial" size=2><b>Authorizations:</b></FONT></P></TD>
				</TR>
				<%
			For Each MyUserAuthInfo As CompuMaster.camm.WebManager.WMSystem.Authorizations.UserAuthorizationInformation In MyUserAuths
				           
		Try
			Dim SecObjID As String = MyUserAuthInfo.SecurityObjectInfo.ID
			Dim SecObjName As String = MyUserAuthInfo.SecurityObjectInfo.DisplayName
					%>
						<TR>
						<TD colspan="2"><TABLE WIDTH="100%" BORDER="0" CELLPADDING="0" CELLSPACING="0"><TR>
						<TD width="160" VAlign="Top"><P><FONT face="Arial" size=2>ID <%= SecObjID %></FONT></P></TD>
						<TD width="240" VAlign="Top"><P><FONT face="Arial" size=2><%= Server.HtmlEncode(SecObjName) %></FONT></P></TD>
						</TR></TABLE></TD>
						</TR>
					<% 
		Catch
			cammWebManager.Log.Warn ("Missing security object with ID " & MyUserAuthInfo.SecurityObjectID & " in authorizations for group ID " & MyUserInfo.ID)
					%>
						<TR>
						<TD colspan="2"><TABLE WIDTH="100%" BORDER="0" CELLPADDING="0" CELLSPACING="0"><TR>
						<TD width="160" VAlign="Top"><P><FONT face="Arial" size=2><em>ID <%= MyUserAuthInfo.SecurityObjectID %></em></FONT></P></TD>
						<TD width="240" VAlign="Top"><P><FONT face="Arial" size=2><em>Missing security object</em></FONT></P></TD>
						</TR></TABLE></TD>
						</TR>
					<% 
		End Try
            Next
        End If
	End If

%>