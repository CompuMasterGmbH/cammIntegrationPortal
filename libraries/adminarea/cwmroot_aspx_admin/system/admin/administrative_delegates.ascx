<%@ Control debug=true Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Controls.Administration.AdministrativeDelegates" %>
<% 
If Not GroupInfo Is Nothing then 
%>
					<TR>
					<TD colspan="2" bgcolor="#C1C1C1"><table width="100%" cellspacing="0" cellpadding="0" border="0"><tr><td><P><FONT face="Arial" size=2><b>Administrative delegates</b></font></p></td>
					<% If Me.CurrentAdminIsPrivilegedForItemAdministration(CompuMaster.camm.WebManager.Pages.Administration.Page.AdministrationItemType.Groups, CompuMaster.camm.WebManager.Pages.Administration.Page.AuthorizationTypeEffective.Update, CInt(Request.QueryString("ID"))) = True Then %>
					<td align="right"><p><FONT face="Arial" size=2><a href="adjust_delegates.aspx<%= "?ID=" & GroupInfo.ID & "&Type=Groups&Title=" & Server.URLEncode(GroupInfo.Name) %>"><%= Iif(CInt(Val(Request.Querystring("view") & ""))=1,"","Adjust") %></a></FONT></P></TD>
					<% Else %>
					<td align="right"><p><FONT face="Arial" size=2>&nbsp;</FONT></P></TD>
					<% End If %>
					</tr></table></td>
					</TR>
<%
	'Create connection and recordset
	Dim MyConn as new System.Data.SqlClient.SqlConnection(cammWebManager.ConnectionString)
	myconn.open
	dim MyCmd as new System.Data.SqlClient.sqlcommand
	mycmd.connection = myconn
	mycmd.commandtext = "SELECT userid, authorizationtype FROM System_SubSecurityAdjustments WHERE TableName = 'Groups' AND TablePrimaryIDValue = " & GroupInfo.ID & " ORDER BY authorizationtype, userid"
	Dim MyRec As System.Data.SqlClient.SqlDataReader
	'MyConn.Open (cammWebManager.ConnectionString)
	myrec = mycmd.executereader
			While MyRec.Read 
%>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= MyRec("AuthorizationType") %></FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><%=Server.HtmlEncode(Me.SafeLookupUserFullName(CLng(MyRec("UserID"))))%></TEXTAREA></FONT></P></TD>
					</TR>
<%
			End While
	MyRec.Close
	mycmd.dispose
	myconn.close
	myconn.dispose
%>
				<TR>
				<TD VAlign="Top" ColSpan="2"><P>&nbsp; </P></TD>
				</TR>
<% 
End If

If Not SecurityObjectInfo Is Nothing then 
%>
					<TR>
					<TD VAlign="Top" Colspan="2"><P> &nbsp;</P></TD>
					</TR>
					<TR>
					<TD colspan="2" bgcolor="#C1C1C1"><table width="100%" cellspacing="0" cellpadding="0" border="0"><tr><td><P><FONT face="Arial" size=2><b>Administrative delegates</b></font></p></td>
					<% If Me.CurrentAdminIsPrivilegedForItemAdministration(CompuMaster.camm.WebManager.Pages.Administration.Page.AdministrationItemType.Applications, CompuMaster.camm.WebManager.Pages.Administration.Page.AuthorizationTypeEffective.Update, CInt(Request.QueryString("ID"))) = True Then %>
					<td align="right"><p><FONT face="Arial" size=2><a href="adjust_delegates.aspx<%= "?ID=" & SecurityObjectInfo.ID & "&Type=Applications&Title=" & Server.URLEncode(SecurityObjectInfo.DisplayName) %>">Adjust</a></FONT></P></TD>
					<% Else %>
					<td align="right"><p><FONT face="Arial" size=2>&nbsp;</FONT></P></TD>
					<% End If %>
					</tr></table></td>
					</TR>
<%
	'Create connection and recordset
	Dim MyConn as new System.Data.SqlClient.SqlConnection(cammWebManager.ConnectionString)
	myconn.open
	dim MyCmd as new System.Data.SqlClient.sqlcommand
	mycmd.connection = myconn
	mycmd.commandtext = "SELECT userid, authorizationtype FROM System_SubSecurityAdjustments WHERE TableName = 'Applications' AND TablePrimaryIDValue = " & SecurityObjectInfo.ID & " ORDER BY authorizationtype, userid"
	Dim MyRec As System.Data.SqlClient.SqlDataReader
	'MyConn.Open (cammWebManager.ConnectionString)
	myrec = mycmd.executereader

			While MyRec.Read 
%>
					<TR>
					<TD VAlign="Top" WIDTH="160"><P><FONT face="Arial" size=2><%= MyRec("AuthorizationType") %></FONT></P></TD>
					<TD VAlign="Top" Width="240"><P><FONT face="Arial" size=2><%=Server.HtmlEncode(Me.SafeLookupUserFullName(CLng(MyRec("UserID"))))%></TEXTAREA></FONT></P></TD>
					</TR>
<%
			End While 

	MyRec.Close
	mycmd.dispose
	myconn.close
	myconn.dispose

End If
%>