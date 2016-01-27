<h3>Auth state</h3>
Request.IsAuthenticated=<%= Request.IsAuthenticated %><br>
Page.User.Identity.IsAuthenticated=<%= Page.User.Identity.IsAuthenticated %><br>
Page.User.Identity.Name=<%= Page.User.Identity.Name %><br>
Page.User.Identity.AuthenticationType=<%= Page.User.Identity.AuthenticationType %><br>

<h3>Auth request</h3>
<%
If Request.Browser.Browser = "IE" Then
	Session("testcounter") += 1
	If Not Page.User.Identity.IsAuthenticated Then
		Response.Clear
		Response.Buffer = False
		'Response.AddHeader ("WWW-Authenticate", "Negotiate,NTLM")
		'If Request.Headers("NovINet") <> Nothing Then
		'	Response.AddHeader ("WWW-Authenticate", "NetIdentity")
		'End If
		response.statuscode = 401
	End If
	Response.Write ("Counter=" & Session("testcounter") & "<br>" & vbnewline)
Else
	%>No IE - <a href="/default/">Start w/o SSO</a><br><%
End If
If Request.Headers("Via") <> Nothing Then
	%>Proxy access via= <%= Request.Headers("Via") %><br><%
End If
If Request.Headers("NovINet") <> Nothing Then
	%>Novell NetIdentity Authentication (for iChain) possible=<%= Request.Headers("NovINet") %><br><%
End If
%>
<h3>Headers</h3>
<%
For Each Key as string in Request.Headers
	Response.Write (Key & "=" & Request.Headers(Key) & "<br>" & vbnewline)
Next
%>
<h3>Browser</h3>
<%= Request.Browser.Browser %>
<h3>Links</h3>
<a href="test.aspx">test.aspx</a><br>
<!--
<%
If Not Page.User.Identity.IsAuthenticated Then
	Response.End
End If
%>
